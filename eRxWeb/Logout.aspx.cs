using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Allscripts.ePrescribe.Common;
using System.Text.RegularExpressions;
using Allscripts.Impact;
using Allscripts.Impact.Utilities;
using eRxWeb.AppCode;
using Microsoft.Security.Application;

// tej - 11/26/2007 - Added Window close for Shared Health

namespace eRxWeb
{
public partial class Logout : BasePage 
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            string redirect_url = string.Empty;

            if (SessionUserID != null && SessionLicenseID != null)
            {
                AuditLogUserInsert(ePrescribeSvc.AuditAction.USER_LOGOUT, SessionUserID);
                Session["UserID"] = null;
                Session["LICENSEID"] = null;
            }

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

            if (Request.QueryString["SSOAccept"] != null && bool.Parse(Request.QueryString["SSOAccept"]))
            {
                lblLogoutMsg.Text ="You have completed the ID Proofing portion of your registration. You must now log back in.";
                return;
            }

            if (Request.QueryString["SSOIDDone"] != null && bool.Parse(Request.QueryString["SSOIDDone"]))
            {
                lblLogoutMsg.Text = "You have already completed the ID proofing portion of your registration. Please log in with a different SSO mode.";
                return;
            }

            if (Request.QueryString["SSOActivationSuccess"] != null)
            {
                if (bool.Parse(Request.QueryString["SSOActivationSuccess"]))
                {
                    lblLogoutMsg.Text = "You have completed setting up your shield account. You must now log back in.";
                }
                else
                {
                    lblLogoutMsg.Text = "There was an error activating your shield account.";
                }
                return;
            }

            Login.LogUserOut(this, isUserSessionTimedOut, shouldForceReauthentication, hasAuthFailedForEPCS,ref redirect_url);

            if (string.IsNullOrEmpty(redirect_url))
            {
				if (Request.QueryString["msg"] != null)
				{
                    // Fortify regex validating.
                    string msg = Encoder.UrlEncode(Request.QueryString["msg"].ToString());
                    Response.Redirect(string.Concat(Constants.PageNames.LOGIN, "?msg=", msg));
				}
				else
				{
					Response.Redirect(Constants.PageNames.LOGIN);
				}
            }
            else
            {
				if (Request.QueryString["msg"] != null)
				{
                    // Fortify regex validating.
                    string msg = Encoder.UrlEncode(Request.QueryString["msg"].ToString());
                    Response.Redirect(string.Concat(redirect_url, "?msg=", msg));
				}
				else
				{
					Response.Redirect(redirect_url);
				}
            }
        }
        // Google Analytics
        PlacementResponse = TieUtility.GetAdPlacement(Request.Cookies, new TieUtility(), new TIEServiceManager(), new ConfigurationManager());
    }
}

}