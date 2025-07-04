using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using eRxWeb.State;
using System.Web;
using System.Net.Http;
using System.Web.Http;
using eRxWeb.ServerModel;
using eRxWeb.AppCode;
using System.Web.SessionState;
using Allscripts.ePrescribe.Shared.Logging;
using eRxWeb.ServerModel.Registration;
using eRxWeb.ePrescribeSvc;
using Allscripts.ePrescribe.Common;


namespace eRxWeb.Controller
{
    [RoutePrefix("api/anonymous/user-activation")]
    [AllowAnonymous]
    public class UserActivationController : ApiController
    {
        private static readonly ILoggerEx logger = LoggerEx.GetLogger();
        
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
        [HttpPost]
        [Route("validate-activation-code")]
        public ApiResponse validateActivationCode(ValidateActivatonCodeRequest validateActivatonCodeRequest)
        {
            var response = new ApiResponse();
            using (var timer = logger.StartTimer("user-activation - activation-code"))
            {
                try
                {

                    ValidateActivatonCodeModel validateActivatonCodeModel = new ValidateActivatonCodeModel();
                    validateActivatonCodeModel.IsValidCaptcha = IsCaptchaValid(validateActivatonCodeRequest.CaptchaText);
                    if (validateActivatonCodeModel.IsValidCaptcha)
                    {
                        validateActivatonCodeModel.IsValid = EPSBroker.DoesActivationCodeExist(validateActivatonCodeRequest.ActivationCode);
                        if (validateActivatonCodeModel.IsValid)
                        {
                            PageState[Constants.SessionVariables.ActivationCode] = validateActivatonCodeRequest.ActivationCode;
                        }
                    }

                    response.Payload = validateActivatonCodeModel;

                    timer.Message = string.Format($"<Response>{response.ToLogString()}</Response>");
                }
                catch (Exception ex)
                {
                    var errorMessage = ApiHelper.AuditException(ex.ToString(), PageState);
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

        private bool IsCaptchaValid(string captchaText)
        {
            return PageState.GetStringOrEmpty(Constants.SessionVariables.CaptchaText).Equals(captchaText, StringComparison.OrdinalIgnoreCase);
        }

        [HttpPost]
        [Route("intialize-workflow")]
        public ApiResponse IntializeWorkflow(IntializeWorkFlowRequest intializeWorkFlowRequest)
        {
            using (var timer = logger.StartTimer("user-activation - set-workflow"))
            {
                var response = new ApiResponse();
                try
                {
                    Constants.Workflow activationWorkflow;
                    string ActivationCodeFromSSO = PageState.GetStringOrEmpty(Constants.SessionVariables.ActivationCodeFromSSO);                   
                    if (!string.IsNullOrWhiteSpace(intializeWorkFlowRequest.AppType) 
                        && intializeWorkFlowRequest.AppType == "Util")
                    {
                        activationWorkflow = Constants.Workflow.Utilities;
                    }
                    else if (!string.IsNullOrWhiteSpace(ActivationCodeFromSSO))
                    {
                        activationWorkflow = Constants.Workflow.Sso;
                        PageState[Constants.SessionVariables.ActivationCode] = PageState[Constants.SessionVariables.ActivationCodeFromSSO];
                        PageState.Remove(Constants.SessionVariables.ActivationCodeFromSSO);
                    }
                    else
                    {
                        activationWorkflow = Constants.Workflow.Main;
                    }
                    PageState[Constants.SessionVariables.ActivationWorkflow] = activationWorkflow;
                    response.Payload = activationWorkflow;
                    timer.Message = string.Format($"<Response>{response.ToLogString()}</Response>");
                }
                catch (Exception ex)
                {
                    var errorMessage = ApiHelper.AuditException(ex.ToString(), PageState);
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
        [Route("link-existing-user")]
        public ApiResponse LinkToExistingUser(LinkUserRequest linkAccountRequest)
        {
            using (var timer = logger.StartTimer("user-activation - link-existing-user"))
            {
                string userHostAddress = Request.Headers.Host;
                LinkUserModel linkUserModel;
                Constants.Workflow activationWorkflow = PageState.Cast(Constants.SessionVariables.ActivationWorkflow, Constants.Workflow.None);
                string activationCode = PageState.GetStringOrEmpty(Constants.SessionVariables.ActivationCode);
                var response = new ApiResponse();
                try
                {
                    LinkExistingShieldUserToeRxUserResponse linkResponse;

                    if (activationWorkflow == Constants.Workflow.Utilities)
                    {
                        linkResponse = EPSBroker.LinkExistingShieldUserToUtilitiesUser(activationCode, userHostAddress, 
                            linkAccountRequest.ShieldUserName, linkAccountRequest.ShieldPassword);
                        
                    }
                    else
                    {
                        linkResponse = EPSBroker.LinkExistingShieldUserToeRxUser(activationCode, userHostAddress, 
                            linkAccountRequest.ShieldUserName, linkAccountRequest.ShieldPassword);
                        
                        if (linkResponse.Success)
                        {
                            if (activationWorkflow == Constants.Workflow.Sso)
                            {
                                EPSBroker.SaveUserAppProperty(
                                linkResponse.EprescribeUserGuid,
                                Constants.UserPropertyNames.USERNAME_AND_PASSWORD_IS_SETUP,
                                Constants.CommonAbbreviations.YES,
                                (Allscripts.ePrescribe.DatabaseSelector.ConnectionStringPointer)linkResponse.DbId);
                            }
                            else if (activationWorkflow == Constants.Workflow.Main)
                            {
                                new ApiAuthenticationProcessor().SetUserAuthenticationContext(
                                                        linkAccountRequest.ShieldUserName,
                                                        linkAccountRequest.ShieldPassword);
                            }
                        }
                    }

                    if (linkResponse.Success)
                    {
                        PageState.Remove(Constants.SessionVariables.ActivationCode);
                        PageState.Remove(Constants.SessionVariables.ActivationWorkflow);
                    }

                    linkUserModel = new LinkUserModel
                    {
                        AccountName = linkResponse.AccountName,
                        FirstName = linkResponse.FirstName,
                        LastName = linkResponse.LastName,
                        Email = linkResponse.Email,
                        IsValid = linkResponse.Success,
                        ErrorReason = linkResponse.ErrorMessage,
                        WorkflowType = activationWorkflow
                    };

                    response.Payload = linkUserModel;
                    timer.Message = string.Format($"<Response>{response.ToLogString()}</Response>");
                }
                catch (Exception ex)
                {
                    var errorMessage = ApiHelper.AuditException(ex.ToString(), PageState);
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

    }
}
