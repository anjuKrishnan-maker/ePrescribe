
using System;
using System.Web;
using System.Web.Http;
using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.Objects;
using Allscripts.ePrescribe.Shared.Logging;
using eRxWeb.AppCode;
using eRxWeb.ServerModel;
using eRxWeb.State;
using Preference = Allscripts.Impact.Preference;

namespace eRxWeb.Controller
{
    public class EcouponToPharmUncheckedApiController : ApiController
    {
        private static readonly ILoggerEx logger = LoggerEx.GetLogger();

        [HttpPost]
        public ApiResponse SaveNeverShowPreference()
        {
            IStateContainer session = new StateContainer(HttpContext.Current.Session);
            var response = new ApiResponse();
            try
            {
                Preference.SaveUserPreference(
                    session.GetGuidOr0x0(Constants.SessionVariables.UserId),
                    Constants.ProviderPreferences.EcopuponUncheckedWarningVisibility,
                    Visibility.Hidden.ToString(), ApiHelper.GetDBID(session));
                session[Constants.SessionVariables.EcouponUncheckedWarningVisibility] = Visibility.Hidden;
            }
            catch (Exception ex)
            {
                var errorMessage = ApiHelper.AuditException(ex.ToString(), session);
                logger.Error("SaveNeverShowPreference Exception: " + ex.ToString());

                response.ErrorContext = new ErrorContextModel
                {
                    Error = ErrorTypeEnum.ServerError,
                    Message = errorMessage
                };
            }
            return response;
        }
    }
}