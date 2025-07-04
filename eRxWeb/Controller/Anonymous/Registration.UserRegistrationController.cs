using eRxWeb.ServerModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using eRxWeb.State;
using SecretQuestion = eRxWeb.ePrescribeSvc.SecretQuestion;
using CreateRegistrantUserRequest = eRxWeb.ePrescribeSvc.CreateRegistrantUserRequest;
using System.Web;
using Allscripts.Impact;
using System.Drawing.Imaging;
using System.Net.Http.Headers;
using System.IO;
using eRxWeb.AppCode;
using Allscripts.ePrescribe.DatabaseSelector;
using System.Data;
using Allscripts.ePrescribe.Objects.Registrant;
using eRxWeb.ePrescribeSvc;
using System.Web.Security;
using System.Web.Http.Filters;
using static Allscripts.ePrescribe.Common.Constants;
using RegistrantInfo = Allscripts.ePrescribe.Objects.Registrant.RegistrantInfo;
using Allscripts.ePrescribe.Shared.Logging;
using System.Web.UI;
using eRxWeb.AppCode.Interfaces;
using System.Web.SessionState;
using Allscripts.ePrescribe.Common;
using RxUser = Allscripts.Impact.RxUser;
using eRxWeb.ServerModel.Registration;
using eRxWeb.AppCode.Registration;

namespace eRxWeb.Controller
{
    
    [RoutePrefix("api/anonymous/user-registration")]
    [AllowAnonymous]
    public class UserRegistrationController : ApiController
    {
        private ILoggerEx Logger { get; } = LoggerEx.GetLogger();
        IStateContainer pageState;
        public IStateContainer PageState
        {
            get
            {
                if (pageState == null)
                    pageState = new StateContainer(HttpContext.Current.Session);
                return pageState;
            }
            set
            {
                pageState = value;
            }
        }


        [HttpGet]
        [Route("checkuser/{username}")]
        public ApiResponse DoesUsernameExist(string username)
        {
            var response = new ApiResponse();
            try
            {
                response.Payload = EPSBroker.DoesUsernameExist(username);
            }
            catch (Exception ex)
            {
                var errorMessage = ApiHelper.AuditException(ex.ToString(), PageState);
                response.ErrorContext = new ErrorContextModel()
                {
                    Error = ErrorTypeEnum.ServerError,
                    Message = errorMessage
                };
            }
            return response;
        }

        [HttpGet]
        [Route("setup-information")]
        public ApiResponse GetInitialPageInformation()
        {
            var response = new ApiResponse();
            try
            {
                Allscripts.ePrescribe.DatabaseSelector.ConnectionStringPointer dbId = ApiHelper.GetDBID(PageState);
                Dictionary<string, dynamic> responseArray = new Dictionary<string, dynamic>();
                DataSet specialityDataSet = PaI.GetSpecialtyList(dbId);
                responseArray.Add("States", Allscripts.Impact.RxUser.ChGetState(dbId));
                responseArray.Add("Speciality", specialityDataSet?.Tables[0]);

                response.Payload = responseArray;
            }
            catch (Exception ex)
            {
                var errorMessage = ApiHelper.AuditException(ex.ToString(), PageState);
                response.ErrorContext = new ErrorContextModel()
                {
                    Error = ErrorTypeEnum.ServerError,
                    Message = errorMessage
                };
            }
            return response;
        }

        [HttpGet]
        [Route("setup-security-questions")]
        public ApiResponse GetSecurityQuestions()
        {
            var response = new ApiResponse();
            try
            {
                Allscripts.ePrescribe.DatabaseSelector.ConnectionStringPointer dbId = ApiHelper.GetDBID(PageState);
                Dictionary<string, dynamic> responseArray = new Dictionary<string, dynamic>();
                responseArray.Add("SecurityQuestions", EPSBroker.GetAllShieldSecretQuestions());
                response.Payload = responseArray;
            }
            catch (Exception ex)
            {
                var errorMessage = ApiHelper.AuditException(ex.ToString(), PageState);
                response.ErrorContext = new ErrorContextModel()
                {
                    Error = ErrorTypeEnum.ServerError,
                    Message = errorMessage
                };
            }
            return response;
        }
        [HttpPost]
        [Route("create-user")]
        public ApiResponse CreateUser(ServerModel.Registration.CreateUserRequest newUserActivationInfo)
        {
            var response = new ApiResponse();
            using (var timer = Logger.StartTimer("Add2NUser"))
            {
                try
                {   
                    response.Payload = new RegistrationUser().CreateUser(PageState, newUserActivationInfo);
                }
                catch (Exception ex)
                {
                    var errorMessage = ApiHelper.AuditException(ex.ToString(), PageState);
                    response.ErrorContext = new ErrorContextModel()
                    {
                        Error = ErrorTypeEnum.ServerError,
                        Message = errorMessage
                    };
                }
            }
            return response;
        }
        [HttpGet]
        [Route("getcaptcha")]
        public ApiResponse GenerateCaptcha()
        {
            var response = new ApiResponse();
            try
            {
                string sTemp, sTemp2, sGuid, base64String;
                sGuid = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 6); //.ToUpper();

                sTemp2 = sGuid.Substring(2, 2).ToUpper();
                sTemp = sGuid.Substring(0, 2).ToUpper();

                PageState["CaptchaText"] = sTemp + sTemp2 + sGuid.Substring(4, 2).ToUpper();

                CaptchaImage objCI = new CaptchaImage(PageState["CaptchaText"].ToString(), "Arial", 200, 50);
                using (objCI)
                {
                    System.IO.MemoryStream ms = new MemoryStream();
                    objCI.Bitmap.Save(ms, ImageFormat.Jpeg);
                    byte[] byteImage = ms.ToArray();
                    base64String = Convert.ToBase64String(byteImage); // Get Base64
                }
                response.Payload = base64String;
            }
            catch (Exception ex)
            {
                var errorMessage = ApiHelper.AuditException(ex.ToString(), PageState);
                response.ErrorContext = new ErrorContextModel()
                {
                    Error = ErrorTypeEnum.ServerError,
                    Message = errorMessage
                };
            }
            return response;
        }

        [HttpPost]
        [Route("create-registrant")]
        public ApiResponse SaveUserData(RegistrantUserRequest registrantUser)
        {
            //TODO: exception use error filter & try catch as attribute.
            var response = new ApiResponse();
            using (var timer = Logger.StartTimer("RegistrationSaveUserData"))
            {
                try
                {
                    ValidateRegistrantModel validateRegistrantModel = ValidateRegistrantUserRequest(registrantUser);

                    if (validateRegistrantModel.IsValid)
                    {
                        string enterpriseClientIDFromSession = PageState.GetStringOrEmpty(SessionVariables.RegistrationEnterpriseClientID);
                        registrantUser.RegistrantUser.EnterpriseClientId = Guid.Parse(string.IsNullOrWhiteSpace(enterpriseClientIDFromSession) ?
                                                    Allscripts.Impact.ConfigKeys.DefaultEnterPriseClientId : enterpriseClientIDFromSession);

                        var ipAddress = HttpContext.Current.Request.UserIpAddress();
                        CreateRegistrantUserResponse registrant = EPSBroker.CreateRegistrantUser(registrantUser, ipAddress);

                        if (!registrant.Success)
                        {
                            if (registrant.Messages != null && registrant?.Messages.Count() > 0 &&
                                registrant.Messages.Where(x => x.Contains(Constants.RegistrationStepDescription.IdentifiedUserInShieldError)).ToList().Count > 0)
                            {
                                validateRegistrantModel.isValidExistingShieldUser = false;
                                validateRegistrantModel.IsValid = false;
                            }
                            else if (registrant.Messages != null && registrant.Messages.Count() > 0 &&
                                registrant.Messages.Where(x => x.Contains(Constants.RegistrationStepDescription.CreateNewIdentifyShieldError)).ToList().Count > 0)
                            {

                                validateRegistrantModel.IsMaxRetryFinished = true;
                                validateRegistrantModel.IsValid = false;
                            }
                            else
                            {
                                string message = "User creation failed";
                                throw new Exception(message);
                            }
                        }
                        StashContext(registrantUser, registrant);
                    }
                    response.Payload = validateRegistrantModel;
                }
                catch (Exception exception)
                {
                    timer.Message = $"<response>{exception.ToString()}</response>";
                    var errorMessage = ApiHelper.AuditException(exception.ToString(), PageState);
                    response.ErrorContext = new ErrorContextModel()
                    {
                        Error = ErrorTypeEnum.ServerError,
                        Message = errorMessage
                    };

                }
            }
            return response;
        }
        private ValidateRegistrantModel ValidateRegistrantUserRequest(RegistrantUserRequest registrantUser)
        {
            ValidateRegistrantModel saveRegistrantModel = new ValidateRegistrantModel();
            saveRegistrantModel.IsValid = false;
            saveRegistrantModel.isValidExistingShieldUser = true;
            saveRegistrantModel.IsValidCaptcha = IsCaptchaValid(registrantUser.Captcha);
            if (IsCaptchaValid(registrantUser.Captcha))
            {
                saveRegistrantModel.IsValidNpi = RxUser.IsValidNPI(registrantUser.RegistrantUser.NPI);
                saveRegistrantModel.IsDeaCheck = !string.IsNullOrWhiteSpace(registrantUser.RegistrantUser.DeaNumber);
                if (saveRegistrantModel.IsDeaCheck)
                {
                    saveRegistrantModel.IsValidDEA = RxUser.DEASanityChecked(registrantUser.RegistrantUser.DeaNumber);
                }

                if (saveRegistrantModel.IsValidNpi && (saveRegistrantModel.IsDeaCheck ? (saveRegistrantModel.IsValidDEA) : true))
                {
                    saveRegistrantModel.IsValid = true;
                }
            }
            return saveRegistrantModel;
        }

        public bool IsCaptchaValid(string captchaText)
        {
            return PageState.GetStringOrEmpty(Constants.SessionVariables.CaptchaText).Equals(captchaText, StringComparison.OrdinalIgnoreCase);
        }              


        private void StashContext(RegistrantUserRequest registrantUser, CreateRegistrantUserResponse registrant)
        {
            var newRegistrant = new RegistrantInfo
            {
                RegistrantId = registrant.RegistrantId,
                ShieldObjectId = registrant.ShieldObjectId,
                ShieldUserName = registrantUser.RegistrantUser.ShieldUserName
            };
            //Set User context & Form Auth Cookie
            var authenticationProcessor = new ApiAuthenticationProcessor();
            authenticationProcessor.SetUserAuthenticationContext(newRegistrant);

            //Save registrant info in session
            IStateContainer session = new StateContainer(HttpContext.Current.Session);
            session[SessionVariables.RegistrantInfo] = newRegistrant;
            session[SessionVariables.ShieldIdentityToken] = registrant.ShieldIdentityToken;
        }
        
    }
}