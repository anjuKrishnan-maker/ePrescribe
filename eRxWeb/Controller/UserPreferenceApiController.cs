using System;
using System.Linq;
using System.Web;
using System.Web.Services;
using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.Objects.PPTPlus;
using Allscripts.Impact;
using eRxWeb.AppCode;
using eRxWeb.State;
using eRxWeb.AppCode.PptPlusBPL;
using eRxWeb.ServerModel;
using ConfigKeys = Allscripts.Impact.ConfigKeys;
using Allscripts.ePrescribe.Data;
using Allscripts.ePrescribe.Shared.Logging;
using System.Web.Http;
using eRxWeb.AppCode.PdmpBPL;

namespace eRxWeb.Controllers
{
    public partial class UserPreferenceApiController : ApiController
    {
        private static ILoggerEx logger = LoggerEx.GetLogger();

        [HttpPost]
        public ApiResponse GetUserPreference()
        {
            using (var timer = logger.StartTimer("GetUserPreference"))
            {
                var response = new ApiResponse();
                IStateContainer session = new StateContainer(HttpContext.Current.Session);

                try
                {
                    response.Payload = GetUserPreference(session);
                    timer.Message = string.Format("<Response>{0}</Response>", response.ToLogString());
                }
                catch (Exception ex)
                {
                    response.ErrorContext = new ErrorContextModel() { Error = ErrorTypeEnum.ServerError, Message = ex.Message };
                    timer.Message = string.Format("<ErrorContext>{0}</ErrorContext>", response.ErrorContext.ToLogString());
                }
                return response;
            }
        }

        internal static UserPreferenceModel GetUserPreference(IStateContainer session)
        {
            var userPrefModel = new UserPreferenceModel();

            var sessionLicense = session.Cast("SessionLicense", default(ApplicationLicense));
            var provAllowsPpt = session.GetBooleanOrFalse(Constants.SessionVariables.ShouldShowPpt);
            var provAllowsRtbi = session.GetBooleanOrFalse(Constants.SessionVariables.ShouldShowRtbi);
            userPrefModel.IsPPTPlusEnabled = (sessionLicense.EnterpriseClient.ShowPPTPlus && (provAllowsPpt || provAllowsRtbi));
            userPrefModel.IsShowPDMP = PDMP.IsShowPDMP(session);
            return userPrefModel;
        }
    }
}