using System;
using System.Web.Services;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.Http;
using System.Web.UI.WebControls;
using Allscripts.ePrescribe.DatabaseSelector;
using eRxWeb.State;
using eRxWeb.AppCode;
using eRxWeb.ServerModel;
using Allscripts.ePrescribe.Shared.Logging;
using Allscripts.ePrescribe.Common;
using Allscripts.Impact;

namespace eRxWeb.Controller
{
    public partial class EPCSApiController : ApiController
    {
        private static ILoggerEx logger = LoggerEx.GetLogger();

        [HttpPost]
        public ApiResponse GetAvailableReportsCount()
        {
            IStateContainer PageState = new StateContainer(HttpContext.Current.Session);
            var response = new ApiResponse();
            try
            {
                using (var timer = logger.StartTimer("GetAvailableReportsCount"))
                {
                    response.Payload = GetAvailableReportsCount(PageState);
                    timer.Message = $"<Response>{response.ToLogString()}</Response>";
                }
            }
            catch (Exception ex)
            {
                var errorMessage = ApiHelper.AuditException(ex.ToString(), PageState);
                logger.Error("GetAvailableReportsCount Exception: " + ex.ToString());

                response.ErrorContext = new ErrorContextModel()
                {
                    Error = ErrorTypeEnum.ServerError,
                    Message = errorMessage
                };
            }
            return response;

        }

        internal static EPCSNotice GetAvailableReportsCount(IStateContainer PageState)
        {
            var notice = new EPCSNotice();
            if (ConfigurationManager.AppSettings["IsMyErxTabVisible"] != null &&
                Convert.ToBoolean(ConfigurationManager.AppSettings["IsMyErxTabVisible"].ToString()) &&
                PageState.GetBooleanOrFalse("UserHasHadEpcsPermissionInTheLastTwoYears"))
            {
                notice.Visible = true;
                if (Allscripts.Impact.ConfigKeys.UseCommonProtectedStoreEPCSReports) // only show report count if using legacy protected store
                {
                    notice.Count = "0";
                }
                else
                {
                    notice.Count = Provider.GetAvailableReportsCount(PageState.GetStringOrEmpty("USERID"), ConnectionStringPointer.REPLICA_DB).ToString();
                }
            }
            else
            {
                notice.Visible = false;
            }

            return notice;
        }


        [HttpPost]
        public ApiResponse DisplayEpscLink()
        {
            IStateContainer PageState = null;
            var response = new ApiResponse();
            try
            {
                PageState = new StateContainer(HttpContext.Current.Session);
                using (var timer = logger.StartTimer("DisplayEpscLink"))
                {
                   
                    response.Payload = displayEpcsLink(PageState);
                    timer.Message = $"<Response>{response.ToLogString()}</Response>";
                }
            }
            catch (Exception ex)
            {
                var errorMessage = ApiHelper.AuditException(ex.ToString(), PageState);
                logger.Error("GetAvailableReportsCount Exception: " + ex.ToString());

                response.ErrorContext = new ErrorContextModel()
                {
                    Error = ErrorTypeEnum.ServerError,
                    Message = errorMessage
                };
            }
            return response;

        }

        internal static bool displayEpcsLink(IStateContainer session)
        {
            ApplicationLicense licience = (ApplicationLicense)session[Constants.SessionVariables.SessionLicense];
            bool isAdvertiseDeluxe = licience.AdvertiseDeluxe;
            bool isEnterpriseEpcsApplyToLicense = EPCSWorkflowUtils.IsEnterpriseEpcsLicense(licience,
                session.GetBooleanOrFalse(Constants.SessionVariables.IsEnterpriseEpcsEnabled));
            bool isLicenseEPCSPurchased = EPCSWorkflowUtils.IsLicenseEpcsPurchased(licience);
            bool isCsMedSelected = EPCSWorkflowUtils.IsCSMedIncuded(session.Cast<List<Rx>>("CURRENT_SCRIPT_PAD_MEDS", new List<Rx>()));
            bool isAdmin = session.GetBooleanOrFalse(Constants.SessionVariables.IsAdmin);


            return isCsMedSelected && !(isEnterpriseEpcsApplyToLicense || isLicenseEPCSPurchased) && isAdvertiseDeluxe && isAdmin;
        }
    }

}