using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.Shared.Logging;
using Allscripts.Impact.Utilities;
using eRxWeb.AppCode;
using eRxWeb.ServerModel;
using eRxWeb.State;
using System.Xml;
using System.Xml.Xsl;
using System.Net.Mail;
using System.IO;
using Allscripts.Impact;
using ConfigKeys = Allscripts.Impact.ConfigKeys;
using Allscripts.ePrescribe.Objects;
using Allscripts.ePrescribe.ExtensionMethods;
using eRxWeb.ePrescribeSvc;
using eRxWeb.AppCode.Interfaces;
using eRxWeb.AppCode.Users;
using static Allscripts.ePrescribe.Common.Constants;
using System.Data;
using Provider = Allscripts.Impact.Provider;
using RxUser = Allscripts.Impact.RxUser;
using System.Collections;
using DeaLicenseType = eRxWeb.ServerModel.DeaLicenseType;
using DEALicense = eRxWeb.ServerModel.DEALicense;
using eRxWeb.ServerModel.Request;
using System.Web.UI;
using SecretQuestion = eRxWeb.ePrescribeSvc.SecretQuestion;


namespace eRxWeb.Controller
{
    public partial class UserApiController : ApiController
    {
        private static readonly ILoggerEx logger = LoggerEx.GetLogger();
        IStateContainer session;

        public IStateContainer Session
        {
            get
            {
                if (session == null)
                    session = new StateContainer(HttpContext.Current.Session);
                return session;
            }
            set
            {
                session = value;
            }
        }

        [HttpPost]
        public ApiResponse ProcessNewUser(UserActivationInfoModel userActivationInfo)
        {
            //Error handling and logging?
            var response = new ApiResponse();
            if (userActivationInfo != null)  
            {
                response.Payload = ProcessNewUserCreation(new NewUserActivationCode(), new AppCode.Users.Email(), userActivationInfo);
            }
            return response;
        }

        //Move this to app code
        internal MessageModel ProcessNewUserCreation(INewUserActivationCode _newUserActivationCode, IEmail _email, UserActivationInfoModel userActivationInfo)
        {
            //Move to a class in app code
            string usersName = $"{userActivationInfo.FirstName} {userActivationInfo.LastName}";
            if (userActivationInfo.UserCommunicationStatus.IsEmailChecked)
            {
                userActivationInfo.UserCommunicationStatus.IsEmailAttempted = true;
                userActivationInfo.UserCommunicationStatus.IsEmailSentSuccessfully = _newUserActivationCode.EmailActivationCode(_email, usersName, userActivationInfo);
            }
            if (userActivationInfo.UserCommunicationStatus.IsPrintChecked)
            {
                // Storing ActivationCode and userName for PDF
                SetSessionValueForPDF(new StateContainer(HttpContext.Current.Session), usersName, userActivationInfo.ActivationCode);
            }
            return new MessageModel(GetResponseMessage(userActivationInfo));
        }

        internal void SetSessionValueForPDF(IStateContainer Session, string usersName, string activationCode)
        {
            //Move to a class in app code
            Session[ActivationCode.CodeName] = usersName;
            Session[ActivationCode.Code] = activationCode;
        }

        //Move this to app code
        internal string GetResponseMessage(UserActivationInfoModel newUserActivationInfo)
        {
            //Move to a class in app code
            string message = string.Empty;
            string usersName = string.Empty;
            if (newUserActivationInfo != null)
            {
                usersName = string.Concat(newUserActivationInfo.FirstName + " " + newUserActivationInfo.LastName);
                if (newUserActivationInfo.UserCommunicationStatus != null)
                {
                    if (newUserActivationInfo.UserCommunicationStatus.IsEmailChecked)
                    {
                        if (!newUserActivationInfo.UserCommunicationStatus.IsPrintChecked)
                        {
                            if (newUserActivationInfo.UserCommunicationStatus.IsEmailAttempted && newUserActivationInfo.UserCommunicationStatus.IsEmailSentSuccessfully)
                                message = $"User {usersName} has been successfully saved.Activation code sent to {newUserActivationInfo.PersonalEmail}";
                            else if (newUserActivationInfo.UserCommunicationStatus.IsEmailAttempted && !newUserActivationInfo.UserCommunicationStatus.IsEmailSentSuccessfully)
                                message = $"User {usersName} has been successfully saved.However,there was an error in emailing the Activation Code. Please provide this Activation Code to the user. Activation Code = { newUserActivationInfo.ActivationCode}";
                        }
                        else
                        {
                            if (newUserActivationInfo.UserCommunicationStatus.IsEmailSentSuccessfully)
                                message = $"User {usersName} has been successfully saved.Activation code printed and email sent to {newUserActivationInfo.PersonalEmail}";
                            else
                                message = $"User {usersName} has been successfully saved.The Activation code was printed, however there was an error in emailing the Activation Code. Please provide this Activation Code to the user. Activation Code = { newUserActivationInfo.ActivationCode}";
                        }

                    }
                    else
                    {
                        message = $"User {usersName} has been successfully saved.Activation code printed.";
                    }
                }                                
            }
            return message;

        }        

        #region DEA License API methods

        [HttpPost]
        public ApiResponse GetUserDEALicenses(DEALicenseRequest dEALicenseRequest)
        {
            using (var timer = logger.StartTimer("GetDeaLicenseDetails"))
            {
                IStateContainer session = new StateContainer(HttpContext.Current.Session);
                var response = new ApiResponse();

                try
                {
                    response.Payload = new UserDEALicense().GetDEALicenses(session, dEALicenseRequest.userID);
                    timer.Message = string.Format($"<Response>{response.ToLogString()}</Response>");
                }
                catch (Exception ex)
                {
                    var errorMessage = ApiHelper.AuditException(ex.ToString(), session);
                    logger.Error("Exception while fetching data: " + ex.ToString());

                    response.ErrorContext = new ErrorContextModel()
                    {
                        Error = ErrorTypeEnum.ServerError,
                        Message = errorMessage
                    };
                    timer.Message = string.Format($"<ErrorContext>{response.ErrorContext.ToLogString()}</ErrorContext>");
                }
                return response;
            }
        }

        [HttpPost]
        public ApiResponse AddUserDEALicense(DEALicenseRequest dEALicenseRequest)
        {
            using (var timer = logger.StartTimer("AddUserDEALicense"))
            {
                IStateContainer session = new StateContainer(HttpContext.Current.Session);
                MessageModel messageModel = new MessageModel();
                var response = new ApiResponse();

                try
                {
                    new UserDEALicense().AddUserDEALicense(session, dEALicenseRequest.userID, dEALicenseRequest.dEALicense);
                    messageModel.Message = $"DEA License {dEALicenseRequest.dEALicense.DEANumber} successfully inserted.";
                    messageModel.Icon = MessageIcon.Success;
                }
                catch (Exception ex)
                {
                    var errorMessage = ApiHelper.AuditException(ex.ToString(), session);
                    messageModel.Message = "Failed to insert DEA license. Reason" + ex.Message.ToString();
                    messageModel.Icon = MessageIcon.Error;
                    logger.Error("AddUserDEALicense: " + ex.ToString());
                    response.ErrorContext = new ErrorContextModel()
                    {
                        Error = ErrorTypeEnum.ServerError,
                        Message = errorMessage
                    };
                    timer.Message = string.Format($"<ErrorContext>{response.ErrorContext.ToLogString()}</ErrorContext>");
                }
                response.Payload = new MessageModel(messageModel.Message, messageModel.Icon,false);
                return response;
            }
        }

        [HttpPost]
        public ApiResponse UpdateDEALicense(DEALicenseRequest dEALicenseRequest)
        {
            using (var timer = logger.StartTimer("UpdateDEALicense"))
            {
                IStateContainer session = new StateContainer(HttpContext.Current.Session);
                MessageModel messageModel = new MessageModel();
                var response = new ApiResponse();

                try
                {
                    new UserDEALicense().UpdateDEALicense(session, dEALicenseRequest.userID, dEALicenseRequest.dEALicense);
                    messageModel.Message = $"DEA License {dEALicenseRequest.dEALicense.DEANumber} successfully updated.";
                    messageModel.Icon = MessageIcon.Success;
                }
                catch (Exception ex)
                {
                    var errorMessage = ApiHelper.AuditException(ex.ToString(), session);
                    messageModel.Message = "Failed to update DEA License. Reason:" + ex.Message.ToString();
                    messageModel.Icon = MessageIcon.Error;
                    logger.Error("UpdateDEALicense: " + ex.ToString());
                    response.ErrorContext = new ErrorContextModel()
                    {
                        Error = ErrorTypeEnum.ServerError,
                        Message = errorMessage
                    };
                    timer.Message = string.Format($"<ErrorContext>{response.ErrorContext.ToLogString()}</ErrorContext>");
                }
                response.Payload = new MessageModel(messageModel.Message, messageModel.Icon, false);
                return response;
            }
        }

        [HttpPost]
        public ApiResponse DeleteProviderDEALicense(DEALicenseRequest dEALicenseRequest)
        {
            using (var timer = logger.StartTimer("DeleteProviderDEALicense"))
            {
                IStateContainer session = new StateContainer(HttpContext.Current.Session);

                var response = new ApiResponse();
                MessageModel messageModel = new MessageModel();
                try
                {
                    new UserDEALicense().DeleteDEALicense(session, dEALicenseRequest.userID, dEALicenseRequest.dEALicense);
                    messageModel.Message = $"DEA License {dEALicenseRequest.dEALicense.DEANumber} successfully deleted.";
                    messageModel.Icon = MessageIcon.Success;
                }
                catch (Exception ex)
                {
                    var errorMessage = ApiHelper.AuditException(ex.ToString(), session);
                    messageModel.Message = "Could not delete DEA license" + ex.Message.ToString();
                    messageModel.Icon = MessageIcon.Error;
                    logger.Error("DeleteProviderDEALicense: " + ex.ToString());
                    response.ErrorContext = new ErrorContextModel()
                    {
                        Error = ErrorTypeEnum.ServerError,
                        Message = errorMessage
                    };
                    timer.Message = string.Format($"<ErrorContext>{response.ErrorContext.ToLogString()}</ErrorContext>");
                }
                response.Payload = new MessageModel(messageModel.Message, messageModel.Icon, false);
                return response;
            }
        }

        #endregion

        #region State License API Methods

        [HttpPost]
        public ApiResponse GetUserStateLicenses(UserStateLicenseRequest userStateLicenseRequest)
        {
            using (var timer = logger.StartTimer("GetUserStateLicense"))
            {
                IStateContainer session = new StateContainer(HttpContext.Current.Session);

                var response = new ApiResponse();

                try
                {
                    response.Payload = new UserStateLicense().GetStateLicenses(session, userStateLicenseRequest.userID);
                    timer.Message = string.Format($"<Response>{response.ToLogString()}</Response>");
                }
                catch (Exception ex)
                {
                    var errorMessage = ApiHelper.AuditException(ex.ToString(), session);
                    logger.Error("Exception while fetching data: " + ex.Message.ToString());

                    response.ErrorContext = new ErrorContextModel()
                    {
                        Error = ErrorTypeEnum.ServerError,
                        Message = errorMessage
                    };
                    timer.Message = string.Format($"<ErrorContext>{response.ErrorContext.ToLogString()}</ErrorContext>");
                }
                return response;
            }
        }

        [HttpPost]
        public ApiResponse DeleteUserStateLicense(UserStateLicenseRequest userStateLicenseRequest)
        {
            using (var timer = logger.StartTimer("DeleteUserStateLicense"))
            {
                IStateContainer session = new StateContainer(HttpContext.Current.Session);
                MessageModel messageModel = new MessageModel();
                var response = new ApiResponse();

                try
                {
                    new UserStateLicense().DeleteProviderLicense(session, userStateLicenseRequest.userID, userStateLicenseRequest.currentUserStateLicense);
                    messageModel.Message = $"State License {userStateLicenseRequest.currentUserStateLicense.LicenseNo} successfully deleted.";
                    messageModel.Icon = MessageIcon.Success;
                }
                catch (Exception ex)
                {
                    var errorMessage = ApiHelper.AuditException(ex.ToString(), session);
                    messageModel.Message = "Failed to delete State License. Reason" + ex.Message.ToString();
                    messageModel.Icon = MessageIcon.Error;
                    logger.Error("Exception while deleting data: " + ex.Message.ToString());

                    response.ErrorContext = new ErrorContextModel()
                    {
                        Error = ErrorTypeEnum.ServerError,
                        Message = errorMessage
                    };
                    timer.Message = string.Format($"<ErrorContext>{response.ErrorContext.ToLogString()}</ErrorContext>");
                }
                response.Payload = new MessageModel(messageModel.Message, messageModel.Icon, false);
                return response;
            }
        }

        [HttpPost]
        public ApiResponse UpdateUserStateLicense(UserStateLicenseRequest userStateLicenseRequest)
        {
            using (var timer = logger.StartTimer("UpdateUserStateLicense"))
            {
                IStateContainer session = new StateContainer(HttpContext.Current.Session);
                MessageModel messageModel = new MessageModel();
                var response = new ApiResponse();

                try
                {
                    new UserStateLicense().UpdateProviderLicense(session, userStateLicenseRequest.userID, userStateLicenseRequest.oldUserStateLicense, userStateLicenseRequest.currentUserStateLicense);
                    messageModel.Message = $"State License {userStateLicenseRequest.currentUserStateLicense.LicenseNo} successfully updated.";
                    messageModel.Icon = MessageIcon.Success;
                }
                catch (Exception ex)
                {
                    var errorMessage = ApiHelper.AuditException(ex.ToString(), session);
                    logger.Error("Exception while updating data: " + ex.Message.ToString());

                    response.ErrorContext = new ErrorContextModel()
                    {
                        Error = ErrorTypeEnum.ServerError,
                        Message = errorMessage
                    };
                    timer.Message = string.Format($"<ErrorContext>{response.ErrorContext.ToLogString()}</ErrorContext>");
                }
                response.Payload = new MessageModel(messageModel.Message, messageModel.Icon, false);
                return response;
            }
        }

        [HttpPost]
        public ApiResponse AddUserStateLicense(UserStateLicenseRequest userStateLicenseRequest)
        {
            using (var timer = logger.StartTimer("AddUserStateLicense"))
            {
                IStateContainer session = new StateContainer(HttpContext.Current.Session);
                MessageModel messageModel = new MessageModel();
                var response = new ApiResponse();

                try
                {
                    new AppCode.Users.UserStateLicense().AddProviderLicense(session, userStateLicenseRequest.userID, userStateLicenseRequest.currentUserStateLicense);
                    messageModel.Message = $"State License {userStateLicenseRequest.currentUserStateLicense.LicenseNo} successfully added.";
                    messageModel.Icon = MessageIcon.Success;
                }
                catch (Exception ex)
                {
                    var errorMessage = ApiHelper.AuditException(ex.ToString(), session);
                    logger.Error("Exception while adding license: " + ex.Message.ToString());

                    response.ErrorContext = new ErrorContextModel()
                    {
                        Error = ErrorTypeEnum.ServerError,
                        Message = errorMessage
                    };
                    timer.Message = string.Format($"<ErrorContext>{response.ErrorContext.ToLogString()}</ErrorContext>");
                }
                response.Payload = new MessageModel(messageModel.Message, messageModel.Icon, false);
                return response;
            }
        }

        #endregion

        #region Secret Questions API Methods

        [HttpPost]
        public ApiResponse SaveSecretQuestionAnswers(SaveSecretQuestionAnswersRequest saveSecretQuestionAnswersRequest)
        {
            using (var timer = logger.StartTimer("SaveSecretQuestionAnswers"))
            {
                IStateContainer session = new StateContainer(HttpContext.Current.Session);

                var response = new ApiResponse();

                try
                {
                    response.Payload = new EditUserSecretQuestions().SaveSecretQuestionAnswers(saveSecretQuestionAnswersRequest, session);
                    timer.Message = string.Format($"<Response>{response.ToLogString()}</Response>");
                }
                catch (Exception ex)
                {
                    var errorMessage = ApiHelper.AuditException(ex.ToString(), session);
                    logger.Error("Exception while fetching data: " + ex.ToString());

                    response.ErrorContext = new ErrorContextModel()
                    {
                        Error = ErrorTypeEnum.ServerError,
                        Message = errorMessage
                    };
                    timer.Message = string.Format($"<ErrorContext>{response.ErrorContext.ToLogString()}</ErrorContext>");
                }
                return response;
            }
        }

        #endregion

        [HttpPost]
        public ApiResponse UserInit(UserRequest userRequest)
        {
            using (var timer = logger.StartTimer("UserApi - UserInit"))
            {
                IStateContainer session = new StateContainer(HttpContext.Current.Session);
                var response = new ApiResponse();

                try
                {
                    string ipAddress = HttpContext.Current.Request.UserIpAddress();
                    response.Payload = new AppCode.Users.User().GetUserInitData(session, userRequest.UserMode, userRequest.UserID, ipAddress, userRequest.IsNoLic);
                    timer.Message = string.Format("<Response>{0}</Response>", response.ToLogString()); //not using interpolation because chars like { or } in site names throws exception
                }
                catch (Exception ex)
                {
                    var errorMessage = ApiHelper.AuditException(ex.ToString(), session);
                    logger.Error("UserApi - UserInit: Exception while fetching data: " + ex.ToString());

                    response.ErrorContext = new ErrorContextModel()
                    {
                        Error = ErrorTypeEnum.ServerError,
                        Message = errorMessage
                    };
                    timer.Message = string.Format($"<ErrorContext>{response.ErrorContext.ToLogString()}</ErrorContext>");
                }
                return response;
            }
        }

        [HttpPost]
        public ApiResponse SaveUser(UserModel saveUserRequest)
        {
            using (var timer = logger.StartTimer("UserApi - SaveUser"))
            {
                IStateContainer session = new StateContainer(HttpContext.Current.Session);
                ILoggerEx logger = LoggerEx.GetLogger();
                var response = new ApiResponse();

                try
                {
                    string ipAddress = HttpContext.Current.Request.UserIpAddress();
                    string appUrl = ConfigurationManager.GetValue(AppConfigVariables.AppUrl);
                    response.Payload = new AppCode.Users.User().SaveUser(saveUserRequest, session, ipAddress, appUrl, logger);
                    timer.Message = string.Format($"<Response>{response.ToLogString()}</Response>");
                }
                catch (Exception ex)
                {
                    var errorMessage = ApiHelper.AuditException(ex.ToString(), session);
                    logger.Error("UserApi - SaveUser: Exception while saving data: " + ex.ToString());

                    response.ErrorContext = new ErrorContextModel()
                    {
                        Error = ErrorTypeEnum.ServerError,
                        Message = errorMessage
                    };
                    timer.Message = string.Format($"<ErrorContext>{response.ErrorContext.ToLogString()}</ErrorContext>");
                }
                return response;
            }
        }        

        //Move this to app code
        public Allscripts.ePrescribe.DatabaseSelector.ConnectionStringPointer GetDbId()
        {
            Allscripts.ePrescribe.DatabaseSelector.ConnectionStringPointer dbID;
            dbID = Allscripts.ePrescribe.DatabaseSelector.ConnectionStringPointer.ERXDB_DEFAULT;

            if (Session["DBID"] != null)
            {
                dbID = (Allscripts.ePrescribe.DatabaseSelector.ConnectionStringPointer)Session["DBID"];
            }
            else
            {
                //try looking it up
                if (Session["UserID"] != null)
                {
                    DataManagerSvc.EntityIdentifier entityID = new DataManagerSvc.EntityIdentifier();
                    entityID.Type = DataManagerSvc.IdentifierType.UserGUID;
                    entityID.Value = Session["UserID"].ToString();

                    DataManagerSvc.DataManagerSvc dmSvc = new DataManagerSvc.DataManagerSvc();
                    dmSvc.Url = ConfigurationManager.GetValue("DataManagerSvc.DataManagerSvc");

                    dbID = (Allscripts.ePrescribe.DatabaseSelector.ConnectionStringPointer)dmSvc.GetDatabasePointer(entityID);
                    Session["DBID"] = dbID;
                }
                else if (Session["LicenseID"] != null)
                {
                    DataManagerSvc.EntityIdentifier entityID = new DataManagerSvc.EntityIdentifier();
                    entityID.Type = DataManagerSvc.IdentifierType.LicenseGUID;
                    entityID.Value = Session["LicenseID"].ToString();

                    DataManagerSvc.DataManagerSvc dmSvc = new DataManagerSvc.DataManagerSvc();
                    dmSvc.Url = ConfigurationManager.GetValue("DataManagerSvc.DataManagerSvc");

                    dbID = (Allscripts.ePrescribe.DatabaseSelector.ConnectionStringPointer)dmSvc.GetDatabasePointer(entityID);
                    Session["DBID"] = dbID;
                }
            }
            return dbID;
        }

        #region Mobile Registration API methods
        [HttpPost]
        public ApiResponse SendMessageTwilio(VerificationCodeSMSModel verificationCodeSMS)
        {
            using (var timer = logger.StartTimer("SendMessageTwilio"))
            {
                IStateContainer session = new StateContainer(HttpContext.Current.Session);
                var response = new ApiResponse();

                try
                {
                    response.Payload = new UserMobileRegistration().SendMessageTwilio(verificationCodeSMS, session);
                    timer.Message = string.Format($"<Response>{response.ToLogString()}</Response>");
                }
                catch (Exception ex)
                {
                    var errorMessage = ApiHelper.AuditException(ex.ToString(), session);
                    logger.Error("Exception while sending message: " + ex.ToString());

                    response.ErrorContext = new ErrorContextModel()
                    {
                        Error = ErrorTypeEnum.ServerError,
                        Message = errorMessage
                    };
                    timer.Message = string.Format($"<ErrorContext>{response.ErrorContext.ToLogString()}</ErrorContext>");
                }
                return response;
            }            
        }

        [HttpPost]
        public ApiResponse SaveUserMobileNumber(VerificationCodeSMSModel verificationCodeSMS) 
        {
            using (var timer = logger.StartTimer("SaveUserMobileNumber"))
            {
                IStateContainer session = new StateContainer(HttpContext.Current.Session);
                var dbId = ApiHelper.GetDBID(session);
                var response = new ApiResponse();

                try
                {
                    response.Payload = new UserMobileRegistration().SaveUserMobileNumber(verificationCodeSMS.userID, verificationCodeSMS.phoneNumber, dbId);
                    timer.Message = string.Format($"<Response>{response.ToLogString()}</Response>");
                }
                catch (Exception ex)
                {
                    var errorMessage = ApiHelper.AuditException(ex.ToString(), session);
                    logger.Error("Exception while saving data: " + ex.ToString());

                    response.ErrorContext = new ErrorContextModel()
                    {
                        Error = ErrorTypeEnum.ServerError,
                        Message = errorMessage
                    };
                    timer.Message = string.Format($"<ErrorContext>{response.ErrorContext.ToLogString()}</ErrorContext>");
                }
                return response;
            }
        }

        [HttpPost]
        public ApiResponse GetUserMobileMessageSettings(VerificationCodeSMSModel verificationCodeSMS) {
            using (var timer = logger.StartTimer("SaveUserMobileNumber"))
            {
                IStateContainer session = new StateContainer(HttpContext.Current.Session);
                var dbId = ApiHelper.GetDBID(session);
                var response = new ApiResponse();

                try
                {
                    response.Payload = new UserMobileRegistration().GetUserMobileMessageSettings(verificationCodeSMS.userID, dbId);
                    timer.Message = string.Format($"<Response>{response.ToLogString()}</Response>");
                }
                catch (Exception ex)
                {
                    var errorMessage = ApiHelper.AuditException(ex.ToString(), session);
                    logger.Error("Exception while fetching data: " + ex.ToString());

                    response.ErrorContext = new ErrorContextModel()
                    {
                        Error = ErrorTypeEnum.ServerError,
                        Message = errorMessage
                    };
                    timer.Message = string.Format($"<ErrorContext>{response.ErrorContext.ToLogString()}</ErrorContext>");
                }
                return response;
            }
        }

        [HttpPost]
        public ApiResponse MobileMessageSettingAuditlog(VerificationCodeSMSModel verificationCodeSMS) 
        {
            using (var timer = logger.StartTimer("SaveUserMobileNumber"))
            {
                IStateContainer session = new StateContainer(HttpContext.Current.Session);
                var dbId = ApiHelper.GetDBID(session);
                var response = new ApiResponse();

                try
                {
                    new UserMobileRegistration().InsertAuditLog(session, verificationCodeSMS);
                    response.Payload = true;
                    session.Remove("VerificationCode");
                }
                catch (Exception ex)
                {
                    var errorMessage = ApiHelper.AuditException(ex.ToString(), session);
                    logger.Error("Exception while fetching data: " + ex.ToString());

                    response.ErrorContext = new ErrorContextModel()
                    {
                        Error = ErrorTypeEnum.ServerError,
                        Message = errorMessage
                    };
                    timer.Message = string.Format($"<ErrorContext>{response.ErrorContext.ToLogString()}</ErrorContext>");
                }

                return response;
            }
        }

        [HttpPost]
        public ApiResponse DeleteUserMobileNumber(VerificationCodeSMSModel verificationCodeSMS)
        {
            using (var timer = logger.StartTimer("SaveUserMobileNumber"))
            {
                IStateContainer session = new StateContainer(HttpContext.Current.Session);
                var dbId = ApiHelper.GetDBID(session);
                var response = new ApiResponse();

                try
                {
                    new UserMobileRegistration().DeleteUserMobileNumber(verificationCodeSMS.userID, dbId);
                    response.Payload = true;
                }
                catch (Exception ex)
                {
                    var errorMessage = ApiHelper.AuditException(ex.ToString(), session);
                    logger.Error("Exception while fetching data: " + ex.ToString());

                    response.ErrorContext = new ErrorContextModel()
                    {
                        Error = ErrorTypeEnum.ServerError,
                        Message = errorMessage
                    };
                    timer.Message = string.Format($"<ErrorContext>{response.ErrorContext.ToLogString()}</ErrorContext>");
                }
                return response;
            }
        }
        #endregion
    }
}