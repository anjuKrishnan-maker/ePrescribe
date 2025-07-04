using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
//using Allscripts.ePrescribe.Objects;
using System.Web.DynamicData;
using System.Web.Services;
using AjaxControlToolkit;
using eRxWeb.State;
using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.Shared.Logging;
using Allscripts.Impact.Utilities;
using ConfigurationManager = System.Configuration.ConfigurationManager;
using eRxWeb.ePrescribeSvc;

namespace eRxWeb
{
    public partial class Controls_DueDiligence : BaseControl
    {
        private static readonly ILoggerEx logger = LoggerEx.GetLogger();

        public delegate void DueDiligenceAcceptedHandler(DueDiligenceEventArgs dSEventArgs);

        public event DueDiligenceAcceptedHandler OnDueDiligenceAccepted;

        private IStateContainer PageState;

        public ePrescribeSvc.OTPForm[] OtpFormsList
        {
            get { return (ePrescribeSvc.OTPForm[])ViewState["OtpFormsList"]; }
            set { ViewState["OtpFormsList"] = value; }
        }


        public string TransactionID
        {
            get { return GetStringValue("TransactionID"); }
            set { ViewState["TransactionID"] = value; }
        }

        public string IdentityName
        {
            get { return GetStringValue("IdentityName"); }
            set { ViewState["IdentityName"] = value; }
        }

        public string IsOTPAuthenicated
        {
            get { return GetStringValue("IsOTPAuthenicated"); }
            set { ViewState["IsOTPAuthenicated"] = value; }
        }
        public ePrescribeSvc.OTPForm OtpForm
        {
            get { return (ePrescribeSvc.OTPForm)ViewState["OtpForm"]; }
            set { ViewState["OtpForm"] = value; }
        }

        public List<UserNameWithUserGuidPair> ListOfUsersToApprove
        {
            get { return (List<UserNameWithUserGuidPair>)ViewState["ListOfUsersToApprove"]; }
            set { ViewState["ListOfUsersToApprove"] = value; }
        }

        private int AuthRetryCount
        {
            get
            {
                if (ViewState["AuthRetryCount"] == null)
                {
                    ViewState["AuthRetryCount"] = 0;
                }

                return Convert.ToInt32(ViewState["AuthRetryCount"]);
            }
            set
            {
                ViewState["AuthRetryCount"] = value;
            }
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            PageState = new StateContainer(Session);
            ucMessage.Visible = false;
        }

        public void Show()
        {
            lblAuthenticationStatus.Visible = false;
            if (AppCode.StateUtils.UserInfo.GetIdProofingMode(PageState) == ShieldTenantIDProofingModel.Institutional)
            {
                panelOTP.Visible = false;

            }
            else
            {
                panelOTP.Visible = true;
                if (isSuccessfulOTPFormLoad())
                {
                    txtUserName.Text = SessionShieldUserName;
                }
                else
                {
                    if (OnDueDiligenceAccepted != null)
                    {
                        OnDueDiligenceAccepted(GetErrorEventArgs("YourOTP authentication forms could not be found."));
                    }
                }

            }
            mpeDueDiligence.Show();
        }



        #region isSuccessfulOTPFormLoad
        private bool isSuccessfulOTPFormLoad()
        {
            bool successfullLoad = false;

            using (logger.StartTimer("GetOTPForms"))
            {
                GetOTPForms();
            }
            if (OtpFormsList == null)
            {
                logger.Debug("isSuccessfulOTPFormLoad(): No otp forms found");
                successfullLoad = false;
                DueDiligenceEventArgs eventArgs = new DueDiligenceEventArgs();
                eventArgs.Success = false;
                eventArgs.Message = "No Second Factor forms found.  Please set up second factor forms.";
                if (OnDueDiligenceAccepted != null)
                {
                    DueDiligenceEventArgs dsEventArgs = GetErrorEventArgs("Error requesting OTP");
                    OnDueDiligenceAccepted(dsEventArgs);

                }
                return false;
            }
            if (OtpFormsList.Length > 0)
            {
                successfullLoad = true;
                bindTokenList();
            }
            else
            {
              refreshOTPControls();
            }

            return successfullLoad;
        }
        #endregion isSuccessfulOTPFormLoad

        #region GetOTPForms
        private void GetOTPForms()
        {
            GetShieldOTPFormsResponse response = EPSBroker.GetOTPForms(base.ShieldSecurityToken, base.EprescribeExternalAppInstanceID);
            TransactionID = response.TransactionID;
            IdentityName = response.IdentityName;
            OtpFormsList = response.OTPForms;
        }
        #endregion GetOTPForms

        #region bindTokenList
        private void bindTokenList()
        {
            tokenList.Items.Clear();
            foreach (ePrescribeSvc.OTPForm otpForm in OtpFormsList)
            {
                string displayName = !string.IsNullOrWhiteSpace(otpForm?.FormId) && otpForm.FormId.StartsWith("(Expires Soon)")
                                    ? ("(Expires Soon) " + otpForm.DisplayName)
                                    : otpForm.DisplayName;
                tokenList.Items.Add((new ListItem(displayName.ToHTMLEncode(), otpForm.FormId.ToHTMLEncode())));
            }
            tokenList.SelectedIndex = 0;
            OtpForm = OtpFormsList[0];
            tokenList.DataBind();
            refreshOTPControls();
        }
        #endregion bindTokenList

        #region DoesSelectedOTPFormAllowRequest

        private bool DoesSelectedOTPFormAllowRequest()
        {
            ListItem listItem;
            bool allowsRequest = false;

            foreach (var otpForm in OtpFormsList)
            {
                listItem = tokenList.SelectedItem;
                if (otpForm.FormId.Equals(listItem.Value))
                {
                    OtpForm = otpForm;
                    allowsRequest = otpForm.AllowsRequest;
                }
            }
            return allowsRequest;
        }
        #endregion DoesSelectedOTPFormAllowRequest




        protected void btnAccept_OnClick(object sender, EventArgs e)
        {
            using (logger.StartTimer("DueDiligence: btnAccept_OnClick()"))
            {
                bool showOverlay = false;
                DueDiligenceEventArgs eventArgs = new DueDiligenceEventArgs();

                if (AppCode.StateUtils.UserInfo.GetIdProofingMode(PageState) == ShieldTenantIDProofingModel.Institutional)
                {
                    AuditCheckAution();
                    showOverlay = false;
                    eventArgs.Success = true;
                    eventArgs.UsersAccepted = ListOfUsersToApprove;
                    eventArgs.Message = "Due Diligence Accepted";
                }
                else
                {
                    if (OtpForm != null)
                    {
                        GetShieldAuthenicateOTPFormsResponse response = EPSBroker.AuthenicateOTP(TransactionID, txtOTP.Text, IdentityName, OtpForm, txtUserName.Text, txtPassword.Text, SessionLicenseID, SessionUserID);
                        if (response.Success)
                        {
                            AuditCheckAution();
                            eventArgs.OtpSecurityToken = response.OtpSecurityToken;
                            eventArgs.IdentitySecurityToken = response.IdentitySecurityToken;
                            eventArgs.Success = true;
                            eventArgs.UsersAccepted = ListOfUsersToApprove;
                            eventArgs.Message = "Due Diligence Accepted";
                            showOverlay = false;
                        }
                        else
                        {
                            AuthRetryCount++;
                            if (AuthRetryCount > 2)
                            {
                                eventArgs.Success = false;
                                eventArgs.forceLogout = true;
                                eventArgs.Message = "Incorrect password entered too many times";
                                showOverlay = false;
                                ResetDialog();
                            }
                            else if (response.isIncorrectShieldPassword)
                            {
                                btnAccept.Enabled = false;
                                ucMessage.Visible = true;
                                ucMessage.MessageText = "Authentication failed. Please re-enter your password and one time password";
                                ucMessage.Icon = Controls_Message.MessageType.ERROR;
                                showOverlay = true;
                            }
                            else if (response.Messages != null && response.Messages.Length > 0)
                            {
                                string errorMsg = response.Messages[0];
                                if (errorMsg.Equals("FACTOR RETRY") || errorMsg.Equals("INVALID CREDENTIAL"))
                                {
                                    btnAccept.Enabled = false;
                                    ucMessage.Visible = true;
                                    ucMessage.MessageText = "Authentication failed. Please re-enter your password and one time password";
                                    ucMessage.Icon = Controls_Message.MessageType.ERROR;
                                    showOverlay = true;
                                }
                                else
                                {
                                    eventArgs.Success = false;
                                    eventArgs.Message = "There was a problem with Authentication";
                                    showOverlay = false;
                                    ResetDialog();
                                }

                            }
                        }
                    }
                    else
                    {
                        eventArgs.Success = false;
                        eventArgs.Message = "There are not any second factor forms tied to this account";
                    }
                }

                if (showOverlay)
                {
                    mpeDueDiligence.Show();
                }
                else
                {
                    mpeDueDiligence.Hide();
                    OnDueDiligenceAccepted?.Invoke(eventArgs);
                }
            }
        }

            

        protected void btnCancel_OnClick(object sender, EventArgs e)
        {
            ResetDialog();

            btnAccept.Enabled = false;
            mpeDueDiligence.Hide();

            if (OnDueDiligenceAccepted != null)
            {
                DueDiligenceEventArgs dsEventArgs = new DueDiligenceEventArgs();
                dsEventArgs.IsCancel = true;
                dsEventArgs.Success = true;
                OnDueDiligenceAccepted(dsEventArgs);
            }
            
        }

        protected void btnRequestOTP_OnClick(object sender, EventArgs e)
        {
            ucMessage.Visible = false;

            GetShieldSendOTPResponse response = EPSBroker.SendOTPRequest(
                base.ShieldSecurityToken,
                base.EprescribeExternalAppInstanceID, 
                TransactionID, 
                OtpForm);

            if (!response.Success)
            { 
                if (OnDueDiligenceAccepted != null)
                {
                    DueDiligenceEventArgs dsEventArgs = GetErrorEventArgs("Error requesting OTP");
                    OnDueDiligenceAccepted(dsEventArgs);
                }
            }
            else
            {
                mpeDueDiligence.Show();
            }
        }



        private DueDiligenceEventArgs GetErrorEventArgs(string error)
        {
            DueDiligenceEventArgs dsEventArgs = new DueDiligenceEventArgs
            {
                Success = false,
                Message = error
            };
            return dsEventArgs;
        }


        private void AuditCheckAution()
        {
            Parallel.ForEach(ListOfUsersToApprove, userApproved => EPSBroker.AuditLogUserInsert(AuditAction.DUE_DILIGENCE_BOXES_CHECKED, SessionLicenseID, SessionUserID, userApproved.UserGuid, Request.UserIpAddress(), DBID));
        }
      
        #region ResetDialog

        private void ResetDialog()
        {
            chkCurrentDEA.Checked = false;
            chkGovID.Checked = false;
            chkHealthCareWorkers.Checked = false;
            chkStateAuthorization.Checked = false;
            txtPassword.Text = string.Empty;
            txtOTP.Text = string.Empty;
            AuthRetryCount = 0;
            btnAccept.Enabled = false;
        }
        #endregion ResetDialog

        protected void tokenList_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            refreshOTPControls();
            mpeDueDiligence.Show();
        }
        private void refreshOTPControls()
        {
            bool allowRequest = DoesSelectedOTPFormAllowRequest();

            if (allowRequest)
            {
                btnRequestOTP.Enabled = true;
            }
            else
            {
                btnRequestOTP.Enabled = false;
            }
        } 
        private string GetStringValue(string property)
        {
            if (ViewState[property] == null)
            {
                return string.Empty;
            }
            else
            {
                return ViewState[property].ToString();
            }
        }
       
    }
}

