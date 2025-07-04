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
using eRxWeb.State;

namespace eRxWeb
{
public partial class SSOError : BasePage
{
    public string SessionAppVersion => Convert.ToString(Session[Constants.SessionVariables.AppVersion]);

    protected void Page_Init(object sender, EventArgs e)
    {
        lnkDefaultStyleSheet.Href += "?version=" + SessionAppVersion;
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["SSOErrorMessage"] != null)
        {
            lblSSOErrorMessage.Text = Session["SSOErrorMessage"].ToString();
        }
    }
}

}