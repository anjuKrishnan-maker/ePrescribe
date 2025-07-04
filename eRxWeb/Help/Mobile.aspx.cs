using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Allscripts.ePrescribe.Common;

namespace eRxWeb
{
    public partial class Help_Mobile : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.Form.Attributes.Add("autocomplete", "off");
                ((HelpMasterPage)this.Master).CurrentPage = HelpMasterPage.HelpPage.MOBILE;
                if (string.IsNullOrEmpty(SessionUserID))
                {
                    pIPhone.InnerText = "To access the iphone user guide, Please login above";
                    pIPhone.Style.Add("font-weight", "Bold");
                }
            }
        }

        protected void PurchaseLink_ServerClick(object sender, EventArgs e)
        {
            string pageName = Constants.PageNames.INTEGRATION_SOLUTIONS_LIST + "?Module=DELUXE";
            Response.Redirect($"{ConfigurationManager.AppSettings["appurl"]}/{Constants.PageNames.SPA_LANDING}?page={pageName}");            
        }
    }

}