using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.Shared.Logging;
using Allscripts.Impact;
using eRxWeb.AppCode;
using eRxWeb.AppCode.Interfaces;
using eRxWeb.ePrescribeSvc;
using eRxWeb.ServerModel;
using eRxWeb.State;
using System;
using System.Web;
using System.Web.Http;
using EPS = eRxWeb.ePrescribeSvc;

namespace eRxWeb.Controller
{
    public class IdmeTeaserAdApiController : ApiController
    {
        private static readonly ILoggerEx logger = LoggerEx.GetLogger();

        [HttpPost]
        public ApiResponse GetEpcsRegistrationLink()
        {
            var response = new ApiResponse();
            IStateContainer pageState = new StateContainer(HttpContext.Current.Session);
            try
            {
                using (var timer = logger.StartTimer("GetEpcsRegistrationLink"))
                {
                    response.Payload = new EPSBroker().GetIdProofingUrl(AppCode.StateUtils.UserInfo.GetIdProofingMode(pageState), 
                                                                               pageState.GetStringOrEmpty(Constants.SessionVariables.SessionShieldUserName));  
                    timer.Message = $"<Url>{response.Payload}</Url>";
                }
            }
            catch (Exception ex)
            {
                var errorMessage = ApiHelper.AuditException(ex.ToString(), pageState);
                logger.Error("GetEpcsRegistrationLink Exception: " + ex.ToString());

                response.ErrorContext = new ErrorContextModel()
                {
                    Error = ErrorTypeEnum.ServerError,
                    Message = errorMessage
                };
            }
            return response;
        }
    }
}