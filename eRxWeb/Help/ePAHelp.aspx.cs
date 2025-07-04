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
using Allscripts.Impact.Ilearn;

namespace eRxWeb
{
public partial class Help_ePAHelp :BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ((HelpMasterPage)this.Master).CurrentPage = HelpMasterPage.HelpPage.EPA;
            lnkILearn.NavigateUrl = ILearnConfigurationManager.GetErxILearnPageUrl("ePA");
        }
    }
}
}