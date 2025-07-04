using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.DatabaseSelector;
using Allscripts.ePrescribe.Shared.Logging;
using Allscripts.Impact;
using eRxWeb.ServerModel;
using eRxWeb.State;

namespace eRxWeb.Controller
{
    public partial class EligibilityApiController : ApiController
    {
        private static ILoggerEx logger = LoggerEx.GetLogger();

        [HttpPost]
        public ApiResponse GetEligibilityAndMedHistoryStatus()
        {
            IStateContainer session = new StateContainer(HttpContext.Current.Session);

            return GetEligibilityAndMedHistoryStatus(session);
        }

        private static ApiResponse GetEligibilityAndMedHistoryStatus(IStateContainer session)
        {
            var dbid = session.Cast(Constants.SessionVariables.DbId, ConnectionStringPointer.ERXDB_DEFAULT);
            var userId = session.GetStringOrEmpty(Constants.SessionVariables.UserId);
            var licenseId = session.GetStringOrEmpty(Constants.SessionVariables.LicenseId);
            var patientId = session.GetStringOrEmpty(Constants.SessionVariables.PatientId);
            var siteId = session.Cast(Constants.SessionVariables.SiteId, 1);
            var response = new ApiResponse();
            try
            {
                using (var timer = logger.StartTimer("GetEligibilityAndMedHistoryStatus"))
                {
                    Audit.PatientEligibilityTransactionUILogInsert(userId, dbid);
                    var eligAndMedHxStatusList = EPSBroker.GetEligAndMedHxStatus(patientId, licenseId, siteId, dbid);

                    var data = eligAndMedHxStatusList.Select(item => new EligibilityMedHistoryModel()
                    {
                        Type = item.TypeDescription,
                        Message = item.Message,
                        AuditID = item.SwitchMessageID.ToString(),
                        ProcessedDate = item.DateTimeLocalFormatted
                    }).ToList();

                    response.Payload = data;
                    var eligibiltyData = string.Join(",", data.Select(u => " SiteID: " + siteId + " AuditID: " + u.AuditID + " Message: " + u.Message + " ProcessedDate: " + u.ProcessedDate + " Type: " + u.Type));
                    timer.Message = $"<eligibiltyData>{eligibiltyData}</eligibiltyData>";
                }

                return response;
            }
            catch (Exception ex)
            {
                var errorMessage = Audit.AddApiException(userId, licenseId, ex.ToString(), dbid);
                logger.Error(ex.ToString());
                response.ErrorContext = new ErrorContextModel() { Error = ErrorTypeEnum.ServerError, Message = errorMessage };
                return response;
            }
        }

    }
}