using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Allscripts.Impact;
using Allscripts.ePrescribe.Common;
using eRxWeb.AppCode;
using eRxWeb.ServerModel;
using System.Web.Script.Serialization;

namespace eRxWeb
{
    public partial class ForcePasswordSetup : BasePage
    {
        #region Properties

        private string tempShieldPassword
        {
            get
            {
                return (string)ViewState["TempShieldPassword"];
            }
            set
            {
                ViewState["TempShieldPassword"] = value;
            }
        }

        private bool isPasswordSetup
        {
            get
            {
                bool _isPasswordSetup = false;

                if (ViewState["ForcePasswordSetupForSSOUser"] == null)
                {
                    ViewState["ForcePasswordSetupForSSOUser"] = _isPasswordSetup;
                }
                else
                {
                    _isPasswordSetup = bool.Parse(ViewState["ForcePasswordSetupForSSOUser"].ToString());
                }

                return _isPasswordSetup;
            }
            set
            {
                ViewState["ForcePasswordSetupForSSOUser"] = value;
            }
        }

        private bool isPasswordExpired
        {
            get
            {
                bool _isPasswordExpired = false;

                if (ViewState["PasswordExpiredForSSOUser"] == null)
                {
                    ViewState["PasswordExpiredForSSOUser"] = isPasswordExpired;
                }
                else
                {
                    _isPasswordExpired = bool.Parse(ViewState["PasswordExpiredForSSOUser"].ToString());
                }

                return _isPasswordExpired;
            }
            set
            {
                ViewState["PasswordExpiredForSSOUser"] = value;
            }
        }

        #endregion

        #region Page Events

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                isPasswordSetup = Session[Constants.SessionVariables.ForcePasswordSetupForSSOUser] != null && bool.Parse(Session[Constants.SessionVariables.ForcePasswordSetupForSSOUser].ToString());
                isPasswordExpired = Session["PasswordExpiredForSSOUser"] != null && bool.Parse(Session["PasswordExpiredForSSOUser"].ToString());

               // Session.Remove("ForcePasswordSetupForSSOUser");
                Session.Remove("PasswordExpiredForSSOUser");

                if (isPasswordSetup)
                {
                    spanTitle.InnerText = "Password Setup";
                    divPasswordSetup.Visible = true;
                    rfvOldPassword.Enabled = false;

                    bool isInitializationSuccessful = initializePasswordChangeWorkflow();

                    if (!isInitializationSuccessful)
                    {
                        if (string.IsNullOrEmpty(Request.QueryString["targetURL"]))
                        {
                            SelectPatientComponentParameters selectPatientComponentParameters = new SelectPatientComponentParameters
                            {
                                PatientId = PageState.GetStringOrEmpty(Constants.SessionVariables.PatientId)
                            };
                            RedirectToSelectPatient(null, selectPatientComponentParameters);
                        }
                        else
                        {
                            string UrlForRedirection = Constants.PageNames.UrlForRedirection(Request.QueryString["targetURL"].ToString());
                            if (string.IsNullOrEmpty(UrlForRedirection))
                            {
                                SelectPatientComponentParameters selectPatientComponentParameters = new SelectPatientComponentParameters
                                {
                                    PatientId = PageState.GetStringOrEmpty(Constants.SessionVariables.PatientId)
                                };
                                RedirectToSelectPatient(null, selectPatientComponentParameters);
                            }
                            else
                            {
                                Response.Redirect(UrlForRedirection);
                            }
                        }
                    }
                }

                if (isPasswordExpired)
                {
                    spanTitle.InnerText = "Change Password";
                    btnSetupPassword.Text = "Change Password";
                    lblPassword.InnerText = "New Password";
                    divPasswordExpired.Visible = true;
                    divPasswordSetup.Visible = false;
                    trCurrentPassword.Visible = true;
                    trUsername.Visible = false;
                    passwordInstructionDiv.Visible = true;
                    btnSkipSetupPassword.Text = "Skip change Password For Now";
                }
            }
        }

        protected void Page_LoadComplete(object sender, EventArgs e)
        {
            ((PhysicianMasterPageBlank)Master).hideTabs();
        }

        protected void btnSetupPassword_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                string message = null;
                bool isChangePasswordSuccessful = false;

                if (isPasswordSetup)
                {
                    isChangePasswordSuccessful = changePassword(out message);
                }
                else if (isPasswordExpired)
                {
                    isChangePasswordSuccessful = changeExpiredPassword(out message);
                }

                if (isChangePasswordSuccessful)
                {
                    Session.Remove(Constants.SessionVariables.DaysLeftBeforePasswordExpires);
                    updatePasswordSetupUserProperty();                    
                    RedirectNextPage();
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(message))
                    {
                        ucErrorMessage.MessageText = "Password setup failed. Please review the password requirements and try again. If this issue persists, please contact your system administrator.";
                    }
                    else
                    {
                        ucErrorMessage.MessageText = "Password setup failed: " + message;
                    }

                    ucErrorMessage.Icon = Controls_Message.MessageType.ERROR;
                    ucErrorMessage.Visible = true;
                }
            }
        }

        protected void btnSkipSetupPassword_Click(object sender, EventArgs e)
        {
            RedirectNextPage();
        }

        private void RedirectNextPage()
        {
            string UrlForRedirection = Constants.PageNames.UrlForRedirection(Request.QueryString.Get("targetURL"));
            if (!string.IsNullOrWhiteSpace(UrlForRedirection))
            {
                Response.Redirect(RedirectHelper.GetRedirectToSPAUrl(UrlForRedirection, true));
            }
            else
            {
                Response.Redirect(AngularStringUtil.CreateUrl(Constants.PageNames.SELECT_PATIENT));
            }
        }

        protected void cvPassword_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = EPSBroker.IsValidPassword(txtPassword.Text.Trim());
        }

        #endregion Page Events

        #region Private Methods

        private bool initializePasswordChangeWorkflow()
        {
            lblUserName.Text = base.SessionShieldUserName;

            bool isSuccessful = false;

            bool isPasswordResetSuccessful = resetShieldPassword();

            if (isPasswordResetSuccessful)
            {
                isSuccessful = true;
            }

            return isSuccessful;
        }

        private bool resetShieldPassword()
        {
            bool isPasswordResetSuccessful = false;

            ePrescribeSvc.GetNewPasswordResponse response = EPSBroker.ResetAndGeneratePasswordForUser(base.ShieldInternalTenantID, base.ShieldInternalUserProfileID);

            if (response.Success)
            {
                this.tempShieldPassword = response.NewPassword;
                isPasswordResetSuccessful = true;
            }
            else
            {
                Audit.AddException(base.SessionUserID, base.SessionLicenseID, "Unable to reset password for SSO user: " + response.Messages[0], Request.UserIpAddress(), null, null, base.DBID);
            }

            return isPasswordResetSuccessful;
        }

        private bool changeExpiredPassword(out string message)
        {
            message = null;
            bool isChangePasswordSuccessful = false;

            eRxWeb.ePrescribeSvc.ePrescribeSvcResponse response = EPSBroker.ChangeUserPassword(
                base.SessionShieldUserName,
                txtOldPassword.Text.Trim(),
                txtPassword.Text.Trim(),
                null,
                null,
                DBID);

            if (response.Success)
            {
                isChangePasswordSuccessful = true;
            }
            else
            {
                foreach (string s in response.Messages)
                {
                    message += s;
                    message += " ";
                }
            }

            return isChangePasswordSuccessful;
        }

        private bool changePassword(out string message)
        {
            message = null;
            bool isChangePasswordSuccessful = false;

            ePrescribeSvc.ePrescribeSvcResponse response = EPSBroker.ChangeUserPassword(
                base.SessionShieldUserName,
                this.tempShieldPassword,
                txtPassword.Text.Trim(),
                null,
                null,
                DBID);

            isChangePasswordSuccessful = response.Success;

            if (response.Messages != null && response.Messages.Length > 0)
            {
                message = response.Messages[0];
            }

            return isChangePasswordSuccessful;
        }

        private void updatePasswordSetupUserProperty()
        {
            EPSBroker.SaveUserAppProperty(
                base.SessionUserID,
                Constants.UserPropertyNames.USERNAME_AND_PASSWORD_IS_SETUP,
                Constants.CommonAbbreviations.YES,
                base.DBID);
        }

        #endregion Private Methods
    }
}