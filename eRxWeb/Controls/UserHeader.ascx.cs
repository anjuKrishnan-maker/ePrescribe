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
using Allscripts.ePrescribe.DatabaseSelector;

namespace eRxWeb
{
public partial class Controls_UserHeader : BaseControl
{
    public string LogOutLandingPage
    {
        get
        {
            if (Session["LogOutLandingPage"] == null)
            {
                return string.Empty;
            }
            return Session["LogOutLandingPage"].ToString();
        }
        set
        {
            Session["LogOutLandingPage"] = value;
        }
    }
    public string LogInLandingPage
    {
        get
        {
            if (Session["LogInLandingPage"] == null)
            {
                return string.Empty;
            }
            return Session["LogInLandingPage"].ToString();
        }
        set
        {
            Session["LogInLandingPage"] = value;
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            displayControls();
            this.Page.Form.DefaultButton = lnkLogin.UniqueID;
        }
    }
    protected void lnkLogin_Click(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(txtUserName.Value) && !string.IsNullOrEmpty(txtPassword.Value))
        {
            Session["LogInLandingPage"] = Request.Path;
            Session["LogOutLandingPage"] = Request.Path;
            Login _login = new Login(this.Page);
            string redirect_url = string.Empty;
            string msg = string.Empty;

            ePrescribeSvc.AuthenticateAndAuthorizeUserResponse authenticateAndAuthorizeUserResponse;
            authenticateAndAuthorizeUserResponse = EPSBroker.AuthenticateAndAuthorizeUser(txtUserName.Value, txtPassword.Value, Page.Request.UserIpAddress());

            //this is used to tie the token refresh audit events to the origina login
            Session["AuditLogUserLoginID"] = authenticateAndAuthorizeUserResponse.AuditLogID;

            if (authenticateAndAuthorizeUserResponse.AuthenticationStatus == ePrescribeSvc.AuthenticationStatuses.SHIELD_AUTH)
            {
                bool isShieldPasswordExpired = false;
                foreach (ePrescribeSvc.AuthenticatedShieldUser authenticatedShieldUser in authenticateAndAuthorizeUserResponse.AuthenticatedShieldUsers)
                {
                    if (authenticatedShieldUser.IsPasswordExpired)
                    {
                        isShieldPasswordExpired = true;
                        break;
                    }
                }

                if (!isShieldPasswordExpired)
                {
                    _login.SetAuthenticationCookieForShieldUser(authenticateAndAuthorizeUserResponse.AuthenticatedShieldUsers[0]);
                    _login.SetAccountInfo(authenticateAndAuthorizeUserResponse, ref redirect_url, ref msg);
                    _login.SetUserSessionInfo(authenticateAndAuthorizeUserResponse.AuthenticatedShieldUsers[0], ref redirect_url, ref msg);
                }
            }
            else
            {
                txtPassword.Style["background-color"] = "#F58E8E";
                txtUserName.Style["background-color"] = "#F58E8E";
            }

            displayControls();
            if (!string.IsNullOrEmpty(LogInLandingPage))
            {
                Response.Redirect(LogInLandingPage);
            }
            else
            {
                Response.Redirect("~/Help/Default.aspx");
            }
        }
        else
        {
            if (string.IsNullOrEmpty(txtPassword.Value))
            {
                txtPassword.Style["background-color"] = "#F58E8E";
            }

            if (string.IsNullOrEmpty(txtUserName.Value))
            {
                txtUserName.Style["background-color"] = "#F58E8E";
            }
        }
    }

    protected void lnkLogout_Click(object sender, EventArgs e)
    {
        
        string redirect_url = string.Empty;
        Session["LogInLandingPage"] = Request.Path;
        Session["LogOutLandingPage"] = Request.Path;
        bool isUserSessionTimedOut = false;
        bool shouldForceReauthentication = false;
        bool hasAuthFailedForEPCS = false;

        if (Request.QueryString["Timeout"] != null && Request.QueryString["Timeout"].Equals("Yes", StringComparison.OrdinalIgnoreCase))
        {
            isUserSessionTimedOut = true;
        }

        if (Request.QueryString["reauthenticate"] != null && bool.Parse(Request.QueryString["reauthenticate"]))
        {
            shouldForceReauthentication = true;
        }

        if (Request.QueryString["authfailedforepcs"] != null && bool.Parse(Request.QueryString["authfailedforepcs"]))
        {
            hasAuthFailedForEPCS = true;
        }

        Login.LogUserOut(this.Page, isUserSessionTimedOut, shouldForceReauthentication, hasAuthFailedForEPCS, ref redirect_url);

        displayControls();

        if (!string.IsNullOrEmpty(LogOutLandingPage))
        {
            Response.Redirect(LogOutLandingPage);
        }
        else
        {
            Response.Redirect("~/Help/Default.aspx");
        }
    }

    private void displayControls()
    {
        if (SessionUserID != null)
        {
            userName.InnerText = SessionUserName;
            loggedIn.Style["display"] = "inline";
            loggedOut.Style["display"] = "none";
        }
        else
        {
            loggedIn.Style["display"] = "none";
            loggedOut.Style["display"] = "inline";

            txtPassword.Style.Remove("background-color");
            txtUserName.Style.Remove("background-color");
        }
    }
}

}