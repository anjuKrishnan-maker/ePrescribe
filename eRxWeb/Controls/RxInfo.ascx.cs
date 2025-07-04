using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Allscripts.ePrescribe.Common;
using Allscripts.Impact.Utilities;

namespace eRxWeb
{
    public partial class Controls_RxInfo : BaseControl
    {
        public string RxInfoDetailsID 
        { 
            get 
            {
                return hiddenRxInfoDetailsID.Value;
            }
            set
            {
                hiddenRxInfoDetailsID.Value = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            int pageHeight = 500;
            int pageWidth = 600;

            if (Session["PAGEHEIGHT"] != null && Session["PAGEWIDTH"] != null)
            {
                Int32.TryParse(Session["PAGEHEIGHT"].ToString(), out pageHeight);
                Int32.TryParse(Session["PAGEWIDTH"].ToString(), out pageWidth);
            }

            lbViewRxInfo.Text = this.Attributes["LinkButtonText"];
            RxInfoDetailsID = this.Attributes["RxInfoDetailsID"];
        }

        public bool RxPrintOptionChecked
        {
            get { return chkRxInfoPrint.Checked; }
            set { chkRxInfoPrint.Checked = value; }
        }

        protected void lbViewRxInfo_Click(object sender, EventArgs e)
        {
            List<string> rxInfoDetailsIdList = new List<string>();
            rxInfoDetailsIdList.Add(RxInfoDetailsID);
            Session["RxInfoDetailsIdList"] = rxInfoDetailsIdList;

            ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "OpenUrlInModal", $"OpenUrlInModal('{Constants.PageNames.PRINT_RX_INFO}', 'RxInfo');", true);
        }
    }
}