using Kingdee.BOS.Core.DynamicForm.PlugIn;
using Kingdee.BOS.Core.DynamicForm.PlugIn.Args;
using Kingdee.BOS.ServiceHelper;
using Kingdee.BOS.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YPFY
{
    [HotUpdate]
    [Description("费用计算单")]
    public class FY : AbstractDynamicFormPlugIn
    {
        public override void AfterButtonClick(AfterButtonClickEventArgs e)
        {
            base.AfterButtonClick(e);
            if (e.Key.EqualsIgnoreCase("F_yprl_Button")) 
            {
                string FStartDate = this.Model.GetValue("F_yprl_StartDate").ToString();//起始日期
                string FEndDate = this.Model.GetValue("F_yprl_EndDate").ToString();//截止日期
                string sql = string.Format(@"exec  w_sp_grossprofitRPT_Cost '{0}','{1}'", FStartDate, FEndDate);
                DataSet ds = DBServiceHelper.ExecuteDataSet(Context, sql);
                DataTable dt = ds.Tables[0];
                this.View.Model.DeleteEntryData("FEntity");
                var rEntity = this.View.Model.BusinessInfo.GetEntity("FEntity");
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        this.Model.CreateNewEntryRow(rEntity, i);
                        this.Model.SetValue("F_yprl_Date", dt.Rows[i]["FDATE"], i);//日期
                        string BMSQL = string.Format(@"select DEP.FDEPTID from T_BD_DEPARTMENT_L  DEPL
                join T_BD_DEPARTMENT DEP ON DEPL.FDEPTID = DEP.FDEPTID where FCREATEORGID = 1 AND FNAME = '{0}'", dt.Rows[i]["FNAME"]);
                        DataSet BMDS = DBServiceHelper.ExecuteDataSet(Context, BMSQL);
                        DataTable MBDT = BMDS.Tables[0];
                        this.Model.SetValue("F_yprl_DeptNo", MBDT.Rows[0]["FDEPTID"], i);//部门编码
                        this.Model.SetValue("F_yprl_FYXMBM", dt.Rows[i]["FCOSTID"], i);//费用项目编码
                        this.Model.SetValue("F_yprl_PH", dt.Rows[i]["FLOTText"], i);//批号
                        this.Model.SetValue("F_YPRL_AMOUNT", dt.Rows[i]["FenTanAmount"]);//金额
                    }
                    this.Model.ClearNoDataRow();
                    this.View.UpdateView("FEntity");
                }
            }
            
           
        }
    }
}
