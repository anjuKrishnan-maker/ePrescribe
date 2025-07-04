using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eRxWeb
{
    public partial class Controls_RxInfoList : BaseControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (base.IsShowRxInfo)
            {
                string rxID = this.Attributes["RxID"];

                var rxInfoDetailsResponse = EPSBroker.GetRxInfoDetailsLiteByRxID(rxID, base.SessionLicenseID, base.SessionUserID, base.DBID);

                int rxInfoDetailControlIndex = 0;
                foreach (var rxInfoDetail in rxInfoDetailsResponse.RxInfoDetails)
                {
                    ++rxInfoDetailControlIndex;
                    panelRxInfoList.Controls.Add(getRxInfoControl("RxInfo", rxInfoDetail.RxInfoDetailsID, "ucRxInfo" + rxInfoDetailControlIndex.ToString()));
                }
            }
        }


        private Controls_RxInfo getRxInfoControl(string linkButtonText, string rxInfoDetailsId, string uniqueUserControlId)
        {
            var rxInfo = (Controls_RxInfo)Page.LoadControl("Controls/RxInfo.ascx");
            rxInfo.Attributes["LinkButtonText"] = linkButtonText;
            rxInfo.Attributes["RxInfoDetailsID"] = rxInfoDetailsId;
            rxInfo.ID = uniqueUserControlId;

            return rxInfo;
        }

        public List<string> RxInfoPrintList
        {
            get
            {
                List<string> rxInfoPrintList = new List<string>();
                foreach (var item in panelRxInfoList.Controls)
                {
                    var info = item as Controls_RxInfo;
                    if (info != null)
                    {
                        Controls_RxInfo rxInfo = info;
                        if (rxInfo.RxPrintOptionChecked)
                            rxInfoPrintList.Add(rxInfo.RxInfoDetailsID);
                    }
                }
                return rxInfoPrintList;
            }
        }
    }
}