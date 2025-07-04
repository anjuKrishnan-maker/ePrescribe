#define TRACE
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.DatabaseSelector;
using Allscripts.ePrescribe.Shared.Logging;
using Allscripts.Impact;
using System.Web.Security;
using eRxWeb.AppCode;
using Allscripts.ePrescribe.Objects;
using Allscripts.ePrescribe.Shared.Data;
using SystemConfig = Allscripts.Impact.SystemConfig;
using eRxWeb.ePrescribeSvc;


namespace eRxWeb
{
    public partial class _Default : LoginBasePage
    {
        private static ILoggerEx logger = LoggerEx.GetLogger();
        private readonly string RedirectUrlAfterAuthentication = "redirect_url";
        private readonly string AuthenticateAndAuthorizeUserResponse = "AuthenticateAndAuthorizeUserResponse";
        private readonly string IsPasswordResetSuccess = "IsPasswordResetSuccess";
        protected void Page_Init(object sender, EventArgs e)
        {
            lnkDefaultStyleSheet.Href += "?version=" + SessionAppVersion;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            bool isOnlineRegistrationEnabled = Allscripts.Impact.ConfigKeys.IsOnlineRegistrationEnabled;
            Session["DisableIntroductoryPopup"] = null;
            if (isOnlineRegistrationEnabled)
                lnkRegister.NavigateUrl = "~/" + Constants.PageNames.REGISTER;
            else
                lnkRegister.NavigateUrl = Allscripts.Impact.Utilities.ConfigurationManager.GetValue("eRxFullRegistrationUrl");

            //Make Main App available on Mobile Devices
            //if (Request.Browser.IsMobileDevice || Request.Browser.Platform.Equals("WinCE") || Request.UserAgent.ToLower().Contains("palm") || (Request.Headers["Accept"] != null && Request.Headers["Accept"].ToLower().Contains(".wap.")))// || Request.UserAgent.ToString().ToLower().Contains("iphone"))
            //{
            //    Response.Redirect(Allscripts.Impact.Utilities.ConfigurationManager.GetValue("MobileRedirect"));
            //}

            if (Request.QueryString != null && Request.QueryString["log"] != null)
            {
                bool doLogging = false;
                bool.TryParse(Request.QueryString["log"], out doLogging);

                if (doLogging)
                {
                    Session[LogConstants.ENABLE_LOGGING_KEY] = true;
                }
            }

            lnkStyleSheet.Href = "Style/AllscriptsStyle.css?version=" + SessionAppVersion;
            PageIcon.Href = "images/Allscripts/favicon.ico";

            if (Request.QueryString["UserName"] != null)
            {
                txtUserName.Text = Request.QueryString["UserName"];
            }

            txtPassword.Attributes.Add("onFocus", "select();");

            Page.Title = ConfigurationManager.AppSettings["Appwelcome"].ToString();
            txtUserName.Focus();

            if (Request.QueryString["Timeout"] != null && Request.QueryString["Timeout"] == "YES")
            {
                LoginError.Text = "As a security precaution, you have been logged out of the system due to inactivity.";
            }
            else if (Request.QueryString["msg"] != null)
            {
                string message = Microsoft.Security.Application.Encoder.HtmlEncode(Request.QueryString["msg"]);
                LoginError.Text = message.Trim();
            }
            else if (Request.QueryString["reauthenticate"] != null)
            {
                LoginError.Text = "It appears that your user roles and permissions have changed. You must now log back in to see your new roles and permissions.";
            }
            else if (Request.QueryString["authfailedforepcs"] != null)
            {
                LoginError.Text = "Password not recognized. Please login again.";
            }

            if (!IsPostBack)
            {
                lblVersion.Text = "Version: " + base.SessionAppVersion;

                if (Request.Cookies["eRxNowCookie"] != null)
                {
                    HttpCookie eRxNowCookie = Request.Cookies.Get("eRxNowCookie");
                    if (eRxNowCookie.Values["UserName"] != null)
                    {
                        txtUserName.Text = eRxNowCookie.Values["UserName"];
                        txtPassword.Focus();
                    }
                    if (eRxNowCookie.Values["Remember"] != null)
                    {
                        cbRemeberLogin.Checked = Convert.ToBoolean(eRxNowCookie.Values["Remember"]);
                    }
                }

                if (Session[Constants.SessionVariables.CURRENT_ERROR] != null && !string.IsNullOrEmpty(Session[Constants.SessionVariables.CURRENT_ERROR].ToString()))
                {
                    //if Session["CurrentErrorID"] is null, let's log the error so we have a record of it
                    if (Session["CurrentErrorID"] == null)
                    {
                        Global.LogError(Session[Constants.SessionVariables.CURRENT_ERROR].ToString());
                    }

                    LoginError.Text = string.Concat("We apologize, but a serious error has occurred. Exception Reference ID = ", Session["CurrentErrorID"].ToString());

                    Session.Remove("CurrentErrorID");
                    Session.Remove(Constants.SessionVariables.CURRENT_ERROR);
                }


            }

            checkImportantInfo();
            // setVisibilityOfAdControls(string.IsNullOrEmpty(lblMessages.Text) ? true : false);

            //Get AD placement from TIE service.
            PlacementResponse = TieUtility.GetAdPlacement(Constants.TIELocationPage.Login_Page, Request.Cookies, new TieUtility(), new TIEServiceManager(), new Allscripts.Impact.Utilities.ConfigurationManager());
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            LoginError.Text = string.Empty;
            //TODO: this is now broken with the new login code. I need to fix this - JT
            Login.GetAndSetDimensions(Request.Form["ht"], hiddenBrowserWidth.Value, PageState);

            string redirectUrl = "~/" + Constants.PageNames.LOGIN.ToLower();
            string msg = string.Empty;
            string accountID = string.Empty;
            Login login = new Login(this);

            //figure out if this is a "normal" login or if the user is logging in with a newly changed password
            string newPassword = txtNewPassword.Text.Trim();
            string password = string.IsNullOrEmpty(newPassword) ? txtPassword.Text.Trim() : newPassword;

            var authRequest = new AuthRequestDTO
            {
                DbId = base.DBID,
                IpAddress = Page.Request.UserIpAddress(),
                UserName = txtUserName.Text,
                IsPasswordResetSuccess = CheckReattemptLoginUsingNewPassword()
            };
            Authenticator authenticatorObj = new Authenticator(logger, Session, login);
            AuthResponseDTO authResponse = authenticatorObj
                                                .AuthenticateUser(authRequest,
                                                                  ref redirectUrl, ref msg, ref accountID, password: password);

            if (authResponse.IsShieldPasswordExpired)
            {
                if (authRequest.IsPasswordResetSuccess)
                {
                    LogAutoLoginWithChangedPasswordFailure();
                    return;//Reattempt failed for conecutive 3 attempts.Show message to login.
                }
                lblPasswordExpiredStatus.Text = null;
                lblPasswordHelpText.Text = SystemConfig.GetHelp("~/" + Constants.PageNames.CHANGE_SHIELD_USER_PASSWORD);
                logger.Debug("mpePasswordExpired.Show();");
                mpePasswordExpired.Show();
                return;
            }
            LoginError.Text = authResponse.LoginErrorMessage;
            if (authResponse.HaveShieldAccountAndPendingRestration)
            {
                PageState[AuthenticateAndAuthorizeUserResponse] = authResponse.AuthenticateAndAuthorizeUserResponse;
                mpeRegistrationWorkFlow.Show();
                return;
            }
            
            SetContextAfterAuthentication(redirectUrl, msg, accountID, login, authResponse);
        }

        private void LogAutoLoginWithChangedPasswordFailure()
        {
            string modifiedErrorMessage = $"Attempt to login is taking longer than usual, please re-login with your changed password after 15 minutes";
            string errorId = Audit.AddException(Guid.Empty.ToString(), Guid.Empty.ToString(),
                $"Attempt to login with changed password failed.{modifiedErrorMessage}", string.Empty, string.Empty, string.Empty, DBID);
            LoginError.Text = $"{modifiedErrorMessage}-{errorId}";
        }

        private bool CheckReattemptLoginUsingNewPassword()
        {
            if (ViewState[IsPasswordResetSuccess] == null)
                return false;
            return ViewState[IsPasswordResetSuccess].ToString().As(false);
        }


        private void SetContextAfterAuthentication(string redirectUrl, string msg, string accountID, Login login, AuthResponseDTO authResponse)
        {
            bool hasLoggedIn = authResponse.HasLoggedIn;
            if (hasLoggedIn)
                BrowserAuditLogInsert(hiddenScreenHeight.Value, hiddenScreenWidth.Value, hiddenBrowserHeight.Value,
                                      hiddenBrowserWidth.Value, "ePrescribe-StandardLogin");

            var userContextRequest = new UserContextRequestDTO
            {
                HasLoggedIn = hasLoggedIn,
                RememberLogin = cbRemeberLogin.Checked,
                Request = Request,
                Response = Response,
                AccountID = accountID,
                UserName = txtUserName.Text,
                Message = msg
            };

            UserContextResponseDTO userContextSet = new UserContextProcessor(logger, Session, login)
                                                                .SetUserContext(SessionLicense, userContextRequest, authResponse.IsRegistrantUser);
            if (hasLoggedIn)
                DisplayServiceAlert(redirectUrl, userContextSet);


            if (!string.IsNullOrWhiteSpace(userContextSet.LoginErrorMessage))
                LoginError.Text = userContextSet.LoginErrorMessage;

        }

        protected void btnContinueLoginOrRegistrationClick(object sender, EventArgs e)
        {
            bool shouldProceedRegistration = rdoRegistration.SelectedValue.AsBool(false);
            string redirectUrl = "~/" + Constants.PageNames.LOGIN.ToLower();
            string msg = string.Empty;
            string accountID = string.Empty;
            Login login = new Login(this);
            var authenticateAndAuthorizeUserResponse = PageState.Cast(AuthenticateAndAuthorizeUserResponse, new AuthenticateAndAuthorizeUserResponse());
            if (shouldProceedRegistration)
                authenticateAndAuthorizeUserResponse.AuthenticationStatus = AuthenticationStatuses.SHIELD_AUTH_REGISTRANT;
            var authRequest = new AuthRequestDTO { DbId = base.DBID, IpAddress = Page.Request.UserIpAddress(), UserName = txtUserName.Text };//User name to be from session
            AuthResponseDTO authResponse = new Authenticator(logger, Session, login).ProcessAuthentication(authRequest, authenticateAndAuthorizeUserResponse,
                                                                                                            ref redirectUrl, ref msg, ref accountID, true);
            PageState.Remove(AuthenticateAndAuthorizeUserResponse);
            SetContextAfterAuthentication(redirectUrl, msg, accountID, login, authResponse);
        }


        private void DisplayServiceAlert(string redirectUrl, UserContextResponseDTO userContextSet)
        {
            if (userContextSet.HasServiceAlertsToShow)
            {
                ViewState[RedirectUrlAfterAuthentication] = redirectUrl;
                var serviceAlertsToBeShown = PageState.Cast(Constants.SessionVariables.ServiceAlerts, new List<ServiceAlert>());
                lblServiceAlertTitle.Text = serviceAlertsToBeShown[0].Title;
                lblServiceAlertBody.Text = serviceAlertsToBeShown[0].Body;
                mpeServiceAlert.Show();
            }
            else
            {
                RedirectUser(redirectUrl);
            }
        }

        protected void btnChangePassword_Click(object sender, EventArgs e)
        {
            ePrescribeSvc.ePrescribeSvcResponse response = EPSBroker.ChangeUserPassword(
                txtUserName.Text,
                txtOldPassword.Text.Trim(),
                txtNewPassword.Text.Trim(),
                null,
                null,
                DBID);

            if (response.Success)
            {
                ViewState[IsPasswordResetSuccess] = response.Success;
                logger.Debug("Password reset success.");
                btnLogin_Click(sender, e);
            }
            else
            {
                logger.Debug("Password reset failed.");

                lblPasswordExpiredStatus.Visible = true;
                lblPasswordExpiredStatus.Text = string.Empty;

                foreach (string s in response.Messages)
                {
                    lblPasswordExpiredStatus.Text += $"{s.ToHTMLEncode()}  ";                    
                }
                mpePasswordExpired.Show();
            }
        }

        private void checkImportantInfo()
        {
            bool newEULA = false;
            try
            {
                ePrescribeSvc.ePrescribeSvc eps = new ePrescribeSvc.ePrescribeSvc();
                eps.Url = ConfigurationManager.AppSettings["ePrescribeSvc.eprescribesvc"].ToString();
                newEULA = eps.EULARecentlyUpdated();
            }
            catch (Exception)
            {

            }
            StringBuilder thismsg = new StringBuilder();
            bool isToday = false;
            DataRowCollection drc = SystemConfig.GetImportantInfo(false, true, false, Allscripts.ePrescribe.DatabaseSelector.ConnectionStringPointer.ERXDB_DEFAULT);
            if (drc != null && drc.Count > 0)
            {
                int cnt = drc.Count;
                for (int i = 0; i < drc.Count; i++)
                {
                    if (Convert.ToDateTime(drc[i]["StartDate"]).ToShortDateString() == DateTime.Today.ToShortDateString())
                    {
                        isToday = true;
                    }
                    else
                    {
                        isToday = false;
                    }
                    if (isToday)
                    {
                        thismsg.Append("<i>Today</i><br><B>");
                        thismsg.Append(drc[i]["InfoTitle"].ToString());
                        thismsg.Append("</B><BR>");
                    }
                    else

                    {
                        string date = Convert.ToDateTime(drc[i]["StartDate"]).ToString("dd-MMM-yyyy");
                        thismsg.Append("<span class=\"messageTitleColor\">");
                        thismsg.Append(drc[i]["InfoTitle"].ToString());
                        thismsg.Append("</span>");
                        thismsg.Append("<span style=\"float: right; color: #FFF;\">");
                        thismsg.Append(date);
                        thismsg.Append("</span><BR>");
                    }
                    thismsg.Append(drc[i]["InfoBody"].ToString());
                    thismsg.Append("<BR /><BR />");
                }
                tblMessages.Visible = true;
                lblMessages.Visible = true;
                lblMessages.Text = thismsg.ToString();

            }
            else if (newEULA)
            {
                thismsg.Append("<i>Today</i><br><B>");
                thismsg.Append("End User License Agreement has been updated.");
                thismsg.Append("</B><BR>");
                thismsg.Append("<b>Notice To All Users:</b> The End User License Agreement (EULA) for this software has been updated. Please click <a href='Help/EULA.aspx' target='_blank'>here</a> to review.");
                thismsg.Append("<BR /><BR />");

                tblMessages.Visible = true;
                lblMessages.Visible = true;
                lblMessages.Text = thismsg.ToString();
            }
            else
            {
                tblMessages.Visible = false;
                lblMessages.Visible = false;
            }
        }

        protected void loginScriptManager_AsyncPostBackError(object sender, AsyncPostBackErrorEventArgs e)
        {
            ScriptManagerCommon.LogException(e.Exception);
        }

        protected void btnForgotShieldPassword_Click(object sender, EventArgs e)
        {
            Response.Redirect(Constants.PageNames.FORGOT_PASSWORD_WIZARD);
        }

        protected void btnContinue_Click(object sender, EventArgs e)
        {
            var serviceAlertsToShow = PageState.Cast<List<ServiceAlert>>(Constants.SessionVariables.ServiceAlerts, null);

            if (serviceAlertsToShow != null && serviceAlertsToShow.Any())
            {
                serviceAlertsToShow.RemoveAt(0);
                PageState[Constants.SessionVariables.ServiceAlerts] = serviceAlertsToShow;

                if (serviceAlertsToShow.Any())
                {
                    lblServiceAlertTitle.Text = serviceAlertsToShow[0].Title;
                    lblServiceAlertBody.Text = serviceAlertsToShow[0].Body;
                    mpeServiceAlert.Show();
                    return;
                }
            }

            RedirectUser(Convert.ToString(ViewState[RedirectUrlAfterAuthentication]));
        }
    }


}