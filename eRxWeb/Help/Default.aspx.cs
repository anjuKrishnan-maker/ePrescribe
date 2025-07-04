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
using Allscripts.Impact.Ilearn;

namespace eRxWeb
{
    public partial class Help_Default : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(PageState.GetStringOrEmpty("USERID")))
            {
                lnkIlearn.HRef = ILearnConfigurationManager.GetErxILearnPageUrl(Page.AppRelativeVirtualPath);
                lnkWhatNew.HRef = ILearnConfigurationManager.GetErxILearnPageUrl("Help", "What%20is%20New");

            }
            else
            {
                lnkIlearn.HRef = "~/ " + Constants.PageNames.LOGIN;
                lnkWhatNew.HRef = "~/ " + Constants.PageNames.LOGIN;
            }
            if (!IsPostBack)
            {
                this.Form.Attributes.Add("autocomplete", "off");
                ((HelpMasterPageNew)this.Master).CurrentPage = HelpMasterPageNew.HelpPage.HOME;

            }
        }
    }

}