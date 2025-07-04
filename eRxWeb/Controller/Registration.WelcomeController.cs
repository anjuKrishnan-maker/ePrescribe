using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.Objects.Registrant;
using Allscripts.ePrescribe.Shared.Logging;
using eRxWeb.AppCode;
using eRxWeb.ServerModel;
using eRxWeb.State;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using static Allscripts.ePrescribe.Common.Constants;

namespace eRxWeb.Controller
{
    [RoutePrefix("api/welcome")]

    public class WelcomeController : ApiController
    {
        private ILoggerEx Logger { get; } = LoggerEx.GetLogger();

        [HttpPost]
        [Route("geturl-for-navigation-to-csp")]
        public ApiResponse GetURLForNavigationToCSP()
        {
             var apiResponse = new ApiResponse();
            IStateContainer session = new StateContainer(HttpContext.Current.Session);
            var registrantContext = new RegistrantContext();
            using (var timer = Logger.StartTimer("UpdateRegistrantPracticeDetails"))
            {
                try
                {
                    RegistrantInfo registrantInfo = session.Cast(SessionVariables.RegistrantInfo, new RegistrantInfo());
                    registrantContext.IseRxUser = !string.IsNullOrEmpty(session.GetStringOrEmpty(SessionVariables.UserId));
                    registrantContext.CspUrl = GetCspURL(registrantContext, session);
                    apiResponse.Payload = registrantContext;
                 if (registrantInfo?.RegistrantId != null)
                    { 
                    EPSBroker.LogRegistrationStep(registrantInfo.RegistrantId, 
                        true, 
                        RegistrationStepDescription.NavigatedToIdentityFlow,
                        RegistrationStep.UserIdentityCheck);
                    }
                    
                }
                catch (Exception exception)
                {
                    var errorMessage = ApiHelper.AuditException(exception.ToString(), session);
                    apiResponse.ErrorContext = new ErrorContextModel()
                    {
                        Error = ErrorTypeEnum.ServerError,
                        Message = $"{errorMessage} : Log Entry Error"
                    };
                }
            }
            return apiResponse;
        }

        private string GetCspURL(RegistrantContext registrantContext, IStateContainer session)
        {
            if (registrantContext.IseRxUser)
            {
              string idProofingUrl =  new EPSBroker().GetIdProofingUrl(AppCode.StateUtils.UserInfo.GetIdProofingMode(session),
                    session.GetStringOrEmpty(SessionVariables.SessionShieldUserName));

                return $"{idProofingUrl}&FastForward=1&ReturnURL={ConfigurationManager.AppSettings[AppConfigVariables.AppUrl]?.Trim()}/{Constants.PageNames.USER_CSP_UPDATER}";
            }
            else
            {
                return $"{Allscripts.Impact.ConfigKeys.CSPLoginUrl.Trim()}&ReturnURL={ConfigurationManager.AppSettings[AppConfigVariables.AppUrl]?.Trim()}/{Constants.PageNames.CREATE_LICENSE}";
            }
        }
    }
}
