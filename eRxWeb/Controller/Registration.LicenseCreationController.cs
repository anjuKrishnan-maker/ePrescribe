using Allscripts.ePrescribe.Objects.Registrant;
using Allscripts.ePrescribe.Shared.Logging;
using eRxWeb.AppCode;
using eRxWeb.ServerModel;
using eRxWeb.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using static Allscripts.ePrescribe.Common.Constants;

namespace eRxWeb.Controller
{
    [RoutePrefix("api/license-creation")]

    public class LicenseCreationController : ApiController
    {
        private ILoggerEx Logger { get; } = LoggerEx.GetLogger();

        [HttpPost]
        [Route("update-practice-details")]
        public ApiResponse UpdateRegistrantPracticeDetails(RegistrantPractice registrantPractice)
        {
            //TODO: the try catch in filter and the time logger 
            var apiResponse = new ApiResponse();
            IStateContainer session = new StateContainer(HttpContext.Current.Session);

            using (var timer = Logger.StartTimer("UpdateRegistrantPracticeDetails"))
            {
                try
                {
                    RegistrantInfo registrantInfo = session.Cast(SessionVariables.RegistrantInfo, new RegistrantInfo());

                    string shieldIdentityToken = session.GetStringOrEmpty(SessionVariables.ShieldIdentityToken);

                    var response = EPSBroker.CompleteRegistration(registrantPractice, registrantInfo, shieldIdentityToken);

                    if (!response.Success)
                    {
                        string message = "Update Registrant Practice Details failed";
                        throw new Exception(message);
                    }
                    session[SessionVariables.IsRegistrantPracticeUpdated] = response.Success;
                    apiResponse.Payload = response.ToString();
                }
                catch (Exception exception)
                {
                    timer.Message = $"<response>{exception.ToString()}</response>";
                    var errorMessage = ApiHelper.AuditException(exception.ToString(), session);
                    apiResponse.ErrorContext = new ErrorContextModel()
                    {
                        Error = ErrorTypeEnum.ServerError,
                        Message = errorMessage
                    };
                }
            }
            return apiResponse;
        }
    }
}
