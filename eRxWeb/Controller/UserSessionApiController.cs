using System;
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

namespace eRxWeb.Controller
{
    public partial class UserSessionApiController : ApiController
    {
        private static readonly ILoggerEx logger = LoggerEx.GetLogger();

        [HttpPost]
        public ApiResponse AttemptSamlTokenRefresh()
        {
            IStateContainer session = new StateContainer(HttpContext.Current.Session);

            var response = new ApiResponse();
            try
            {
                using (var timer = logger.StartTimer("AttemptSamlTokenRefresh"))
                {
                    var attemptRefreshResult = UserSession.AttemptSamlTokenRefresh(ShieldSettings.ePrescribeExternalAppInstanceId(session), session, new ConfigurationManager(), new EPSBroker());

                    response.Payload = attemptRefreshResult;
                    timer.Message = $"<AttemptSamlTokenRefresh>{attemptRefreshResult.ToLogString()}</AttemptSamlTokenRefresh>";
                }
            }
            catch (Exception ex)
            {
                var errorMessage = ApiHelper.AuditException(ex.ToString(), session);
                logger.Error("AttemptSamlTokenRefresh Exception: " + ex.ToString());

                response.ErrorContext = new ErrorContextModel()
                {
                    Error = ErrorTypeEnum.ServerError,
                    Message = errorMessage
                };
            }
            return response;

        }

        [HttpPost]
        public ApiResponse RetrieveSessionTimeoutMs()
        {
            IStateContainer session = new StateContainer(HttpContext.Current.Session);

            var response = new ApiResponse();
            try
            {
                using (var timer = logger.StartTimer("RetrieveSessionTimeoutMs"))
                {
                    var msRemaining = TimeRemaining(session);
                    response.Payload = msRemaining;
                    timer.Message = $"<MsRemaining>{msRemaining.ToLogString()}</MsRemaining>";
                }
            }
            catch (Exception ex)
            {
                var errorMessage = ApiHelper.AuditException(ex.ToString(), session);
                logger.Error("RetrieveSessionTimeoutMs Exception: " + ex.ToString());

                response.ErrorContext = new ErrorContextModel()
                {
                    Error = ErrorTypeEnum.ServerError,
                    Message = errorMessage
                };
            }
            return response;
        }

        internal static int TimeRemaining(IStateContainer session)
        {
            return session.GetInt(Constants.SessionVariables.ShieldTokenTimeRemainingMs, 1200000);
        }
    }
}