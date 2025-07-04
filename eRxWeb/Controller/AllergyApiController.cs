using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.DatabaseSelector;
using Allscripts.ePrescribe.Shared.Logging;
using Allscripts.Impact;
using eRxWeb.ServerModel;
using eRxWeb.State;
using System;
using System.Data;
using System.Web;
using System.Web.Http;

namespace eRxWeb.Controllers
{
    public partial class AllergyApiController : ApiController
    {
        private static ILoggerEx logger = LoggerEx.GetLogger();

        [HttpPost]
        public ApiResponse DoesCurrentPatientHaveInactivatedAllergies()
        {
            IStateContainer session = new StateContainer(HttpContext.Current.Session);

            return DoesCurrentPatientHaveInactivatedAllergies(session);
        }

        private static ApiResponse DoesCurrentPatientHaveInactivatedAllergies(IStateContainer session)
        {
            using (var timer = logger.StartTimer("DoesCurrentPatientHaveInactivatedAllergies"))
            {
                var dbid = session.Cast(Constants.SessionVariables.DbId, ConnectionStringPointer.ERXDB_DEFAULT);
                var userId = session.GetStringOrEmpty(Constants.SessionVariables.UserId);
                var licenseId = session.GetStringOrEmpty(Constants.SessionVariables.LicenseId);
                var patientId = session.GetStringOrEmpty(Constants.SessionVariables.PatientId);
                var response = new ApiResponse();
                timer.Message = $"<DbId>{dbid}</DbId><userId>{userId}</userId><licenseId>{licenseId}</licenseId><patientId>{patientId}</patientId>";
                try
                {
                    response.Payload = false;
                    // performance improvement - have dedicated stored proce to get a single answer instead of

                    var dataset = Patient.PatientAllergy(patientId, dbid);
                    foreach (DataRow row in dataset.Tables[0].Rows)
                    {
                        if (Convert.ToString(row["Active"]) == "Y"
                            && Convert.ToString(row["AllergyType"]) == "C"
                            && Convert.ToString(row["ClassActiveStatus"]) == "N")
                        {
                            response.Payload = true;
                            break;
                        }
                    }
                    DataTable dt = dataset.Tables[0];
                    timer.Message = $"<dataset>{dt.ToLogString()}</dataset><response>{response.ToLogString()}</response>";
                }
                catch (Exception ex)
                {
                    var errorMessage = Audit.AddApiException(userId, licenseId, ex.ToString(), dbid);
                    timer.Message = $"<response>{ex.ToString()}</response>";
                    response.ErrorContext = new ErrorContextModel() { Error = ErrorTypeEnum.ServerError, Message = errorMessage };
                }
                return response;
            }
        }
    }
}