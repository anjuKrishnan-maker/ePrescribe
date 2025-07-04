using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.DatabaseSelector;
using Allscripts.ePrescribe.Shared.Logging;
using Allscripts.Impact;
using eRxWeb.AppCode;
using eRxWeb.ServerModel;
using eRxWeb.State;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using Allscripts.ePrescribe.Data;
using Allscripts.Impact.Interfaces;
using EPS = eRxWeb.ePrescribeSvc;
using SystemConfig = Allscripts.Impact.SystemConfig;
using UserProperties = eRxWeb.AppCode.UserProperties;

namespace eRxWeb.Controllers
{
    public partial class UrgentMessageApiController : ApiController
    {
        private static readonly ILoggerEx logger = LoggerEx.GetLogger();

        [HttpPost]
        public ApiResponse GetUrgentMessage()
        {
            using (var timer = logger.StartTimer("GetUrgentMessage"))
            {
                IStateContainer pageState = new StateContainer(HttpContext.Current.Session);
                var response = new ApiResponse();
                try
                {
                    response.Payload = checkUserMessages(pageState);
                    timer.Message = string.Format("<Response>{0}</Response>", response.ToLogString());
                }
                catch (Exception ex)
                {
                    var errorMessage = ApiHelper.AuditException(ex.ToString(), pageState);
                    logger.Error("GetUrgentMessage Exception: " + ex.ToString());

                    response.ErrorContext = new ErrorContextModel()
                    {
                        Error = ErrorTypeEnum.ServerError,
                        Message = errorMessage
                    };
                    timer.Message = string.Format("<ErrorContext>{0}</ErrorContext>", response.ErrorContext.ToLogString());
                }
                return response;
            }
        }

        [HttpPost]
        public ApiResponse GetFailedRxMessage()
        {
            using (var timer = logger.StartTimer("GetFailedRxMessage"))
            {
                IStateContainer pageState = new StateContainer(HttpContext.Current.Session);
                var response = new ApiResponse();
                try
                {
                    response.Payload = getFailedRegMessages(pageState);
                    timer.Message = string.Format("<Response>{0}</Response>", response.ToLogString());
                }
                catch (Exception ex)
                {
                    var errorMessage = ApiHelper.AuditException(ex.ToString(), pageState);
                    logger.Error("GetFailedRxMessage Exception: " + ex.ToString());

                    response.ErrorContext = new ErrorContextModel()
                    {
                        Error = ErrorTypeEnum.ServerError,
                        Message = errorMessage
                    };
                    timer.Message = string.Format("<ErrorContext>{0}</ErrorContext>", response.ErrorContext.ToLogString());
                }
                return response;
            }
                
        }
        internal static List<FaildRegMsg> ConstructFailedRegMessages(DataTable failedRegMessages)
        {
            List<FaildRegMsg> _msgs = new List<FaildRegMsg>();
            using (var timer = logger.StartTimer("ConstructFailedRegMessages"))
            {
                for (int i = 0; i < failedRegMessages?.Rows.Count; i++)
                {
                    var data = new FaildRegMsg();
                    data.Name = $"{failedRegMessages.Rows[i]["Firstname"].ToString()} {failedRegMessages.Rows[i]["LastName"].ToString()}";
                    data.RequestCreated = failedRegMessages.Rows[i]["RequestCreated"].ToString();
                    data.RequestID = failedRegMessages.Rows[i]["RequestID"].ToString();
                    timer.Message = $"<Name>{data.Name}</Name><RequestCreated>{data.RequestCreated}</RequestCreated><RequestID>{data.RequestID}</RequestID>";
                    _msgs.Add(data);

                }
            }
            return _msgs;
        }
        [HttpPost]
        public ApiResponse ConfirmFilRegMessage([FromUri]string requestID)
        {
            using (var timer = logger.StartTimer("ConfirmFilRegMessage"))
            {
                IStateContainer pageState = new StateContainer(HttpContext.Current.Session);
                var response = new ApiResponse();

                try
                {
                    if (requestID != "")
                    {
                        ConnectionStringPointer connectionStringPointer = ApiHelper.GetDBID(pageState);
                        SystemConfig.UpdateUserRequest(requestID, Constants.RequestStatus.READ, connectionStringPointer);
                        DataTable failedRegMsgModel = SystemConfig.GetFailedRegistrationMessages(ApiHelper.GetSessionLicenseID(pageState), connectionStringPointer);
                        pageState["CURRENT_FAILED_REG_ERRORS"] = failedRegMsgModel;
                        response.Payload = ConstructFailedRegMessages(failedRegMsgModel);
                        timer.Message = string.Format("<Response>{0}</Response>", response.ToLogString());
                        logger.Debug("Current Failed Reg Errors");
                    }
                    else
                    {
                        logger.Debug("Request ID is missing");
                        response.ErrorContext = new ErrorContextModel()
                        {
                            Error = ErrorTypeEnum.UserMessage,
                            Message = "Request id is missing!!"//may be different text
                        };
                    }
                }
                catch (Exception ex)
                {
                    var errorMessage = ApiHelper.AuditException(ex.ToString(), pageState);
                    logger.Error("ConfirmFilRegMessage Exception: " + ex.ToString());

                    response.ErrorContext = new ErrorContextModel()
                    {
                        Error = ErrorTypeEnum.ServerError,
                        Message = errorMessage
                    };
                    timer.Message = string.Format("<ErrorContext>{0}</ErrorContext>", response.ErrorContext.ToLogString());
                }
                timer.Message = string.Format("<Response>{0}</Response>", response.ToLogString());
                return response;
            }
        }
        private static List<FaildRegMsg> getFailedRegMessages(IStateContainer pageState)
        {
            List<FaildRegMsg> _msgs = new List<FaildRegMsg>();
            DataTable failedRegMessages = null;
            using (var timer = logger.StartTimer("getFailedRegMessages"))
            {
                if (pageState["CURRENT_FAILED_REG_ERRORS"] == null)
                {
                    failedRegMessages = SystemConfig.GetFailedRegistrationMessages(ApiHelper.GetSessionLicenseID(pageState), ApiHelper.GetDBID(pageState));
                    pageState["CURRENT_FAILED_REG_ERRORS"] = failedRegMessages;
                }
                else
                {
                    failedRegMessages = (pageState["CURRENT_FAILED_REG_ERRORS"] as DataTable);
                }

                for (int i = 0; i < failedRegMessages.Rows.Count; i++)
                {
                    var data = new FaildRegMsg();

                    data.Name = $"{failedRegMessages.Rows[i]["Firstname"].ToString()} {failedRegMessages.Rows[i]["LastName"].ToString()}";
                    data.RequestCreated = failedRegMessages.Rows[i]["RequestCreated"].ToString();
                    data.RequestID = failedRegMessages.Rows[i]["RequestID"].ToString();
                    timer.Message = $"<Response><Name>{data.Name}</Name><RequestCreated>{data.RequestCreated}</RequestCreated><RequestID>{data.RequestID}</RequestID></Response>";
                    _msgs.Add(data);
                }
            }
            return _msgs;
        }
        internal static UrgentMessageModel checkUserMessages(IStateContainer pageState)
        {
            var data = new UrgentMessageModel();
            var asyncTasks = new List<Task>();
            var asyncUtil = new LoadAsyncUtil(new AsyncContext(HttpContext.Current, LocalLogContext.LogContextInfo, "", "CheckUserMessagesAsync"), logger);

            asyncTasks.Add(asyncUtil.CreateTask(checkDeluxeMessages, p => data.IsDeluxMessageVisible = p));

            asyncTasks.Add(asyncUtil.CreateTask(checkSPI, p => data.EnableEprescribeMsg = p));

            asyncTasks.Add(asyncUtil.CreateTask(checkFailedMessages, data));

            asyncTasks.Add(asyncUtil.CreateTask(checkRegistrationMessages, data));

            asyncTasks.Add(asyncUtil.CreateTask(checkStateLicenseAndDeaExpiry, data));

            asyncTasks.Add(asyncUtil.CreateTask(checkEPrescribeTrialExpiry, data));

            //asyncTasks.Add(asyncUtil.CreateTask(checkClinicalMessagesCount, messageModel));

            asyncTasks.Add(asyncUtil.CreateTask(ShowEpcsLinks, data));

            asyncTasks.Add(asyncUtil.CreateTask(ShowPasswordExpires, data));

            try
            {
                Task.WaitAll(asyncTasks.ToArray());
            }
            catch (AggregateException e)
            {
                var msg = $"checkUserMessagesException: {e}";
                logger.Error(msg);
                AuditLog.ExceptionAdd(pageState.GetStringOrEmpty(Constants.SessionVariables.UserId), pageState.GetStringOrEmpty(Constants.SessionVariables.LicenseId), msg, "", "", "", pageState.Cast(Constants.SessionVariables.DbId, ConnectionStringPointer.ERXDB_DEFAULT));
            }
            return data;
        }

        private static void checkEPrescribeTrialExpiry(IStateContainer pageState, UrgentMessageModel data)
        {
            bool isTrialPeriodExpiryVisible = false;
            string trialPeriodExpiryMessage = "";

            using (var timer = logger.StartTimer("checkEPrescribeTrialExpiry"))
            {
                try
                {
                    ApplicationLicense license = SessionLicense(pageState);
                    if (license.IsEnterpriseForceCompulsoryBasic() && license.IsPricingStructureBasic())
                    {
                        int daysLeft, daysSinceLicenseCreation;
                        DateTime licenseCreationDate = AppCode.StateUtils.UserInfo.GetLicenseCreationDate(pageState);
                        daysSinceLicenseCreation = Convert.ToInt32(DateTime.Now.Subtract(licenseCreationDate).TotalDays);
                        daysLeft = Allscripts.Impact.ConfigKeys.EPrescribeTrialDays - daysSinceLicenseCreation;
                        if (daysLeft >= 0)
                        {
                            isTrialPeriodExpiryVisible = true;
                            trialPeriodExpiryMessage = $"You have {daysLeft} {(daysLeft == 1 ? "day" : "days")} of ePrescribe Basic trial left";
                        }
                    }
                }
                catch
                {

                }
                data.IsTrialPeriodExpiryVisible = isTrialPeriodExpiryVisible;
                data.TrialPeriodExpiryMessage = trialPeriodExpiryMessage;
                timer.Message = $"<Response><IsTrialPeriodExpiryVisible>{data.IsTrialPeriodExpiryVisible}</IsTrialPeriodExpiryVisible><TrialPeriodExpiryMessage>{data.TrialPeriodExpiryMessage}</TrialPeriodExpiryMessage></Response>";
            }

        }

        private static void checkStateLicenseAndDeaExpiry(IStateContainer pageState, UrgentMessageModel data)
        {
            bool isStateLicenseExpiryVisible = false;
            bool isDeaExpiryVisible = false;
            using (var timer = logger.StartTimer("checkStateLicenseAndDeaExpiry"))
            {
                try
                {
                    var expiryDates = EPSBroker.GetStateLicenseAndDeaExpiryDateForUser(Guid.Parse(pageState.GetStringOrEmpty("UserID")), pageState.GetStringOrEmpty("PRACTICESTATE"), GetDBID(pageState));
                    if (expiryDates != null)
                    {
                        if (expiryDates.StateLicenseExpiryDate != null)
                        {
                            int dateDifferenceStateLicense = (expiryDates.StateLicenseExpiryDate - DateTime.UtcNow).Days;
                            if (dateDifferenceStateLicense <= 10 && dateDifferenceStateLicense >= 0)
                            {
                                data.StateLicenseExpiryMessage = $"Your State License is set to expire in {dateDifferenceStateLicense} day(s)";
                                isStateLicenseExpiryVisible = true;
                                timer.Message = $"<StateLicenseExpiryMessage>{data.StateLicenseExpiryMessage}</StateLicenseExpiryMessage>";
                            }
                        }
                        if (expiryDates.DeaExpiryDate != null)
                        {
                            int dateDifferenceDea = (expiryDates.DeaExpiryDate - DateTime.UtcNow).Days;
                            if (dateDifferenceDea <= 10 && dateDifferenceDea >= 0)
                            {
                                data.DeaExpiryMessage = $"Your DEA License Number is set to expire in {dateDifferenceDea} day(s)";
                                isDeaExpiryVisible = true;
                                timer.Message = $"<DeaExpiryMessage>{data.DeaExpiryMessage}</DeaExpiryMessage>";
                            }
                        }
                    }
                }
                catch
                {
                    //Nothing to log
                }
            }

            data.IsStateLicenseExpiryVisible = isStateLicenseExpiryVisible;
            data.IsDeaExpiryVisible = isDeaExpiryVisible;
        }

        private static string checkSPI(IStateContainer pageState)
        {
            string spimsg = string.Empty;
            using (var timer = logger.StartTimer("checkSPI"))
            {
                if (string.IsNullOrWhiteSpace(pageState.GetStringOrEmpty("SPI")))
                {
                    if (pageState.GetBooleanOrFalse("IsPASupervised"))
                    {
                        spimsg = "<b>Your account is currently not enabled for <br>e-prescribing.  You can only print Rxs at this time.</b><br> Please call " + SessionLicense(pageState).EnterpriseClient.HelpPhoneNumber + " for Veradigm Technical Support.<br><hr>";
                    }
                    else if (!string.IsNullOrEmpty(DelegateProviderName(pageState)))
                    {
                        spimsg = "<b>" + DelegateProviderName(pageState) + "'s account is currently not enabled for <br>e-prescribing.  You can only print Rxs at this time.</b><br>To reach Veradigm Technical Support for assistance, please <a target=_helpwin href=\"" + ConfigurationManager.AppSettings["HelpURL"] + "/" + Constants.PageNames.CONTACT_US + "\">contact us</a>.<br><hr>";
                    }
                    timer.Message = $"<spimsg>{spimsg}</spimsg";
                    //if (!string.IsNullOrEmpty(spimsg))
                    //{
                    //    panelMessage.Controls.Add(new LiteralControl(spimsg));
                    //}
                }
            }
            return spimsg;
        }

        private static bool checkDeluxeMessages(IStateContainer pageState)
        {
            var style = false;
            if (SessionLicense(pageState).LicenseDeluxeStatus == Constants.DeluxeFeatureStatus.Disabled)
            {

                if (SessionLicense(pageState).AdvertiseDeluxe)
                {
                    style = true;
                }

            }

            return style;
        }
        private static void checkFailedMessages(IStateContainer pageState, UrgentMessageModel data)
        {
            //we only care about messages for the entire site...not just for current user
            Constants.UserCategory userType = (Constants.UserCategory)pageState.Cast<int>("UserType", 0);
            bool isPrescriber = (userType == Constants.UserCategory.PROVIDER || userType == Constants.UserCategory.PHYSICIAN_ASSISTANT);

            int userCount = CurrentRxError(pageState).UserErrorCount;
            int licenseSiteCount = CurrentRxError(pageState).LicenseSiteErrorCount;

            using (var timer = logger.StartTimer("checkFailedMessages"))
            {
                if (userCount > 0 || licenseSiteCount > 0 || GetFailedRxCount(pageState) > 0)
                {
                    //_showPanel = true;
                    data.IsFailedRxMessageVisible = true;
                    if (userCount > 0)
                    {
                        data.IsFailedUserMessageVisible = true;
                        string userFilter = null;
                        //if (isPrescriber)
                        //    userFilter = "&Auth=" + base.SessionUserID;
                        //else
                        logger.Debug("Failed Rx User Message");
                        userFilter = "&Orig=" + GetSessionUserID(pageState); // Urgent message count from DB is recieved based on originator, hence filter criteria here will be on the originator.

                        data.FailedRxUserMessage = string.Format("You have {0} failed Rx {1}", userCount, userCount == 1 ? "message" : "messages");
                        data.FailedRxUserMessageUrl = "../" + Constants.PageNames.MESSAGE_QUEUE_TX + "?From=" + Constants.PageNames.SELECT_PATIENT + "&View=Errors" + userFilter + "&Start=" + CurrentRxError(pageState).UserErrorMinDate.ToString();

                    }
                    else
                    {
                        data.IsFailedUserMessageVisible = false;
                    }
                    timer.Message = $"<FailedRxUserMessage>{data.FailedRxUserMessage}</FailedRxUserMessage><FailedRxUserMessageUrl>{data.FailedRxUserMessageUrl}</FailedRxUserMessageUrl>";
                    if (licenseSiteCount > 0)
                    {
                        logger.Debug("Failed License Message");
                        data.IsFailedLicenseMessageVisible = true;
                        data.FailedLicenseMessage = string.Format("The site has {0} failed Rx {1}", licenseSiteCount, licenseSiteCount == 1 ? "message" : "messages");
                        data.FailedLicenseMessageUrl = "../" + Constants.PageNames.MESSAGE_QUEUE_TX + "?From=" + Constants.PageNames.SELECT_PATIENT + "&View=Errors&Filter=Site&Start=" + CurrentRxError(pageState).LicenseSiteErrorMinDate.ToString();

                    }
                    else
                    {
                        data.IsFailedLicenseMessageVisible = false;
                    }

                    timer.Message = $"<IsFailedLicenseMessageVisible>{data.IsFailedLicenseMessageVisible}</IsFailedLicenseMessageVisible><FailedLicenseMessage>{data.FailedLicenseMessage}</FailedLicenseMessage><FailedLicenseMessageUrl>{data.FailedLicenseMessageUrl}</FailedLicenseMessageUrl>";
                    if (GetFailedRxCount(pageState) > 0)
                    {
                        logger.Debug("Failed Rx Message");
                        data.IsFailedRxMessageVisible = true;

                        if (GetFailedRxCount(pageState) == 1)
                            data.FailedRxMessage = "There is 1 Rx failure warning message.";
                        else
                            data.FailedRxMessage = string.Format("There are {0} Rx failure warning messages.", GetFailedRxCount(pageState).ToString());

                        data.FailedRxMessageLinkText = string.Format("View warning {0}", GetFailedRxCount(pageState) == 1 ? "message" : "messages");
                        //  modalShowFailedRxMessages.CancelControlID = ucFailedRxMessages.FindControl("btnClose").UniqueID;

                        //if (iModalUp == 1)
                        //{
                        //    modalShowFailedRxMessages.Show();
                        //}
                        //else
                        //{
                        //    modalShowFailedRxMessages.Hide();
                        //}
                    }
                    else
                    {
                        data.IsFailedRxMessageVisible = false;
                    }

                    timer.Message = $"<IsFailedRxMessageVisible>{data.IsFailedRxMessageVisible}</IsFailedRxMessageVisible><FailedRxMessage>{data.FailedRxMessage}</FailedRxMessage><FailedRxMessageLinkText>{data.FailedRxMessageLinkText}</FailedRxMessageLinkText>";
                }
                else
                {
                    data.IsFailedRxMessageVisible = false;
                }
                var _deniedRefReqCount = GetDeniedRefReqCount(pageState);
                if (_deniedRefReqCount > 0)
                {
                    //_showPanel = true;
                    logger.Debug("deniel refill request message");
                    data.IsDeniedRefReqMessagesVisible = true;
                    if (_deniedRefReqCount == 1)
                        data.DeniedRefReqMessages = "There is 1 denied refill request message.<br />";
                    else
                        data.DeniedRefReqMessages = string.Format("There are {0} denied refill request messages.", _deniedRefReqCount.ToString());

                    timer.Message = $"<IsDeniedRefReqMessagesVisible>{data.IsDeniedRefReqMessagesVisible}</IsDeniedRefReqMessagesVisible><DeniedRefReqMessages>{data.DeniedRefReqMessages}</DeniedRefReqMessages>";
                    // lblDeniedRefReqs.Font.Bold = true;
                    data.DeniedRefReqMessagesLinkText = string.Format("View denied {0}", _deniedRefReqCount == 1 ? "message" : "messages");
                    timer.Message = $"<DeniedRefReqMessagesLinkText>{data.DeniedRefReqMessagesLinkText}</DeniedRefReqMessagesLinkText>";
                    //modalShowDeniedRefReqMessages.CancelControlID = ucDeniedRefReqMessages.FindControl("btnClose").UniqueID;

                    //if (iModalUp == 2)
                    //{
                    //    modalShowDeniedRefReqMessages.Show();
                    //}
                    //else
                    //{
                    //    modalShowDeniedRefReqMessages.Hide();
                    //}
                }
                else
                {
                    data.IsDeniedRefReqMessagesVisible = false;
                }
            }


        }
        private static void checkRegistrationMessages(IStateContainer pageState, UrgentMessageModel data)
        {
            var _failedRegistrationCount = GetFailedRegistrationCount(pageState);
            using (var timer = logger.StartTimer("checkRegistrationMessages"))
            {
                if (_failedRegistrationCount > 0)
                {
                    logger.Debug("Failed registration Message");
                    data.IsrRgistrationMessagesVisible = true;
                    if (_failedRegistrationCount == 1)
                        data.FailedRegistrationMessage = "There is 1 failed registration message.";
                    else
                        data.FailedRegistrationMessage = string.Format("There are {0} failed registration messages.", _failedRegistrationCount.ToString());

                    data.FailedegistrationMessageLinkText = string.Format("View failed {0}", _failedRegistrationCount == 1 ? "message" : "messages");

                    //modalShowFailedRegMessages.CancelControlID = ucFailedRegMessages.FindControl("btnClose").UniqueID;

                    //if (iModalUp == 3)
                    //{
                    //    modalShowFailedRegMessages.Show();
                    //}
                    //else
                    //{
                    //    modalShowFailedRegMessages.Hide();
                    //}
                }
                else
                {
                    data.IsrRgistrationMessagesVisible = false;
                }

                timer.Message = $"<IsrRgistrationMessagesVisible>{data.IsrRgistrationMessagesVisible}</IsrRgistrationMessagesVisible><FailedRegistrationMessage>{data.FailedRegistrationMessage}</FailedRegistrationMessage><FailedegistrationMessageLinkText>{data.FailedegistrationMessageLinkText}</FailedegistrationMessageLinkText>";
            }

        }

        private static void ShowEpcsLinks(IStateContainer session, UrgentMessageModel messageModel)
        {
            var shieldTraitInfo = GetShieldTraitsFromSession(session);

            if (shieldTraitInfo != null)
            {
                ConstructEpcsLinks(session, messageModel, shieldTraitInfo, new EPSBroker());
            }
        }

        internal static void ConstructEpcsLinks(IStateContainer session, UrgentMessageModel messageModel, 
            List<EPS.ShieldTraitInfo> shieldTraitInfo, eRxWeb.AppCode.Interfaces.IEPSBroker epsBroker)
        {
            var canEnroll = IsUserEPCSShieldTrait(shieldTraitInfo, EPS.ShieldTraitName.CanEnroll);
            var canApprove = IsUserEPCSShieldTrait(shieldTraitInfo, EPS.ShieldTraitName.CanApprove);
            var canPrescribe = IsUserEPCSShieldTrait(shieldTraitInfo, EPS.ShieldTraitName.CanPrescribe);
            var isIdentityCompromised = IsUserEPCSShieldTrait(shieldTraitInfo, EPS.ShieldTraitName.IsIdentityCompromised);

            // use the vars
            messageModel.IsEpcsCanEnrollVisible = false;
            messageModel.IsEpcsApproverVisible = false;

            if (isIdentityCompromised)
            {
                return;
            }

            if (canEnroll)
            {
                var identityProofingMode = AppCode.StateUtils.UserInfo.GetIdProofingMode(session);


                messageModel.EpcsCanEnrollText = "Start/Resume EPCS Registration";
                switch (identityProofingMode)
                {
                    case EPS.ShieldTenantIDProofingModel.Institutional:
                        messageModel.IsEpcsCanEnrollVisible = !canPrescribe;
                        break;
                    default: // individual
                        messageModel.IsEpcsCanEnrollVisible = !(canPrescribe || canApprove);
                        break;
                }

                if (messageModel.IsEpcsCanEnrollVisible)
                {
                    messageModel.EpcsCanEnrollUrl = epsBroker.GetIdProofingUrl(AppCode.StateUtils.UserInfo.GetIdProofingMode(session), session.GetStringOrEmpty(Constants.SessionVariables.SessionShieldUserName));  
                }
            }

            if (canApprove)
            {
                messageModel.IsEpcsApproverVisible = true;
            }
        }
      
        internal static List<EPS.ShieldTraitInfo> GetShieldTraitsFromSession(IStateContainer session)
        {
            var result = new List<EPS.ShieldTraitInfo>();

            if (AppCode.StateUtils.UserInfo.IsPermissionGranted(UserPermissions.EpcsCanEnroll, session))
            {
                result.Add(new EPS.ShieldTraitInfo()
                    {TraitName = EPS.ShieldTraitName.CanEnroll, TraitValueEnum = EPS.ShieldTraitValue.YES});
            }

            if (AppCode.StateUtils.UserInfo.IsPermissionGranted(UserPermissions.EpcsCanApprove, session))
            {
                result.Add(new EPS.ShieldTraitInfo()
                    {TraitName = EPS.ShieldTraitName.CanApprove, TraitValueEnum = EPS.ShieldTraitValue.YES});
            }

            if (AppCode.StateUtils.UserInfo.IsPermissionGranted(UserPermissions.EpcsCanPrescribe, session))
            {
                result.Add(new EPS.ShieldTraitInfo()
                    {TraitName = EPS.ShieldTraitName.CanPrescribe, TraitValueEnum = EPS.ShieldTraitValue.YES});
            }

            if (UserProperties.GetBoolOrFalseUserPropertyValue(Constants.UserPropertyNames.COMPROMISED_IDENTITY, session, new UserProperties()))
            {
                result.Add(new EPS.ShieldTraitInfo()
                    {TraitName = EPS.ShieldTraitName.IsIdentityCompromised, TraitValueEnum = EPS.ShieldTraitValue.YES});
            }
            else
            {
                result.Add(new EPS.ShieldTraitInfo()
                    {TraitName = EPS.ShieldTraitName.IsIdentityCompromised, TraitValueEnum = EPS.ShieldTraitValue.NO});
            }

            return result;
        }

        private static bool IsUserEPCSShieldTrait(List<EPS.ShieldTraitInfo> traits, EPS.ShieldTraitName shieldTraitName)
        {
            bool traitValue = false;

            var trait = traits.Find(x => x.TraitName == shieldTraitName);

            if (trait != null && trait.TraitValueEnum == EPS.ShieldTraitValue.YES)
            {
                traitValue = true;
            }
            return traitValue;
        }

    
        private static string GetSessionShieldUserName(IStateContainer pageState)
        {
            if (pageState["SessionShieldUserName"] != null)
            {
                return pageState["SessionShieldUserName"].ToString();
            }

            return string.Empty;

        }

        private static string GetShieldSecurityToken(IStateContainer pageState)
        {

            string retVal = null;
            if (System.Configuration.ConfigurationManager.AppSettings["SAMLTokenStorageLocation"].ToString().Equals("Session"))
            {
                if (pageState["ShieldSecurityToken"] == null)
                    retVal = null;

                retVal = pageState["ShieldSecurityToken"].ToString();
            }
            return retVal;
        }

        private static ApplicationLicense SessionLicense(IStateContainer pageState)
        {

            if (pageState["SessionLicense"] == null)
            {
                pageState["SessionLicense"] = new ApplicationLicense(string.Empty, 0, string.Empty, GetDBID(pageState));
            }

            return pageState.Cast<ApplicationLicense>("SessionLicense", null);
        }

        private static string DelegateProviderName(IStateContainer pageState)
        {
            string providerName = string.Empty;
            RxUser provider = pageState.Cast<RxUser>("DelegateProviderID", null);
            if (provider != null)
            {
                providerName = provider.GetAttribute("Title") + " " + provider.GetAttribute("FirstName") + " " + provider.GetAttribute("LastName");
            }

            return providerName;
        }

        private static int GetDeniedRefReqCount(IStateContainer pageState)
        {
            int deniedRefReqCount = 0;
            var data = pageState.Cast<DataSet>("CURRENT_DENIED_ERROR_REF_REQ_ERRORS", null);
            if (data == null)
            {
                DataSet ds = SystemConfig.GetRefRefDeniesAndError(GetSessionLicenseID(pageState), GetSessionUserID(pageState), GetDBID(pageState));
                deniedRefReqCount = ds.Tables[0].Rows.Count + ds.Tables[1].Rows.Count;
            }
            else
            {
                deniedRefReqCount = data.Tables[0].Rows.Count + data.Tables[1].Rows.Count;
            }
            return deniedRefReqCount;
        }
        private static int GetFailedRxCount(IStateContainer pageState)
        {
            int failedRxCount = 0;
            var data = pageState.Cast<DataTable>("CURRENT_FAILED_RX_ERRORS", null);
            if (data == null)
            {
                data = SystemConfig.GetUserRequests(GetSessionLicenseID(pageState), null, Constants.RequestType.INTERNAL_RX_ERROR, Constants.ResponseType.PUBLIC, Constants.RequestStatus.UNREAD, GetDBID(pageState));
            }

            failedRxCount = data.Rows.Count;

            return failedRxCount;
        }
        private static RxError CurrentRxError(IStateContainer pageState)
        {


            RxError rxError = pageState.Cast<RxError>("CURRENT_RX_ERROR", null);

            if (rxError == null)
            {
                rxError = new RxError(GetSessionUserID(pageState), GetSessionLicenseID(pageState), GetSessionSiteID(pageState), true, GetDBID(pageState));
            }

            return rxError;

        }
        private static int GetFailedRegistrationCount(IStateContainer pageState)
        {
            int failedRegistrationCount = 0;
            var data = pageState.Cast<DataTable>("CURRENT_FAILED_REG_ERRORS", null);
            if (data == null)
            {
                data = SystemConfig.GetFailedRegistrationMessages(GetSessionLicenseID(pageState), GetDBID(pageState));
            }

            failedRegistrationCount = data.Rows.Count;
            return failedRegistrationCount;
        }

        private static void ShowPasswordExpires(IStateContainer pageState, UrgentMessageModel data)
        {
            using (var timer = logger.StartTimer("ShowPasswordExpires"))
            {
                if (!pageState.GetBooleanOrFalse(Constants.SessionVariables.IsSSOUser) && string.IsNullOrWhiteSpace(pageState.GetStringOrEmpty(Constants.SessionVariables.DaysLeftBeforePasswordExpires)))
                {
                    pageState[Constants.SessionVariables.DaysLeftBeforePasswordExpires] = EPSBroker.GetShieldPasswordExpirationDays(pageState.GetStringOrEmpty("SessionShieldUserName"));
                }

                if (!string.IsNullOrWhiteSpace(pageState.GetStringOrEmpty(Constants.SessionVariables.DaysLeftBeforePasswordExpires)))
                {                    
                    int daysLeftBeforePasswordExpires =
                        Convert.ToInt32(pageState.GetStringOrEmpty(Constants.SessionVariables.DaysLeftBeforePasswordExpires));
                    logger.Debug("Show Password Expiry");
                    if (daysLeftBeforePasswordExpires <= Allscripts.Impact.ConfigKeys.DisplayPasswordExpirationWarningDays)
                    {
                        data.IsPasswordExpiryNotice = true;

                        if (daysLeftBeforePasswordExpires == 0)
                        {
                            data.DaysLeftBeforePasswordExpires = "today.";
                        }
                        if (daysLeftBeforePasswordExpires < 0)
                        {
                            data.DaysLeftBeforePasswordExpires = "expired";
                        }
                        else
                        {
                            data.DaysLeftBeforePasswordExpires = "in " + daysLeftBeforePasswordExpires.ToString() + " days.";
                        }
                    }
                    else
                    {
                        data.IsPasswordExpiryNotice = false;
                    }

                    timer.Message = $"<IsPasswordExpiryNotice>{data.IsPasswordExpiryNotice}</IsPasswordExpiryNotice><DaysLeftBeforePasswordExpires>{data.DaysLeftBeforePasswordExpires}</DaysLeftBeforePasswordExpires>";
                }                            
            }
        }


        public static string GetSessionUserID(IStateContainer pageState)
        {
            return pageState.GetStringOrEmpty("USERID");
        }

        public static string GetSessionLicenseID(IStateContainer pageState)
        {
            return pageState.GetStringOrEmpty("LICENSEID");
        }

        public static int GetSessionSiteID(IStateContainer pageState)
        {
            return pageState.GetInt("SITEID", 0);
        }

        public static ConnectionStringPointer GetDBID(IStateContainer pageState)
        {
            return pageState.Cast<ConnectionStringPointer>("DBID", ConnectionStringPointer.ERXDB_DEFAULT);
        }

        public static Constants.UserCategory GetUserType(IStateContainer pageState)
        {
            return pageState.Cast<Constants.UserCategory>("UserType", Constants.UserCategory.GENERAL_USER);
        }
    }

}

