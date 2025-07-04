using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.DatabaseSelector;
using Allscripts.ePrescribe.Shared.Logging;
using Allscripts.Impact;
using eRxWeb.ServerModel;
using eRxWeb.State;
using System;
using System.Data;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.Http;

namespace eRxWeb.Controllers
{
    public class PatientMedRecApiController : ApiController
    {
        private static ILoggerEx logger = LoggerEx.GetLogger();

        [HttpPost]
        public ApiResponse GetMedReconciliationInfo()
        {
            IStateContainer session = new StateContainer(HttpContext.Current.Session);

            return GetMedReconciliationInfo(session);
        }

       
        private static ApiResponse GetMedReconciliationInfo(IStateContainer session)
        {
            using (var timer = logger.StartTimer("GetMedReconciliationInfo"))
            {
                var dbid = session.Cast(Constants.SessionVariables.DbId, ConnectionStringPointer.ERXDB_DEFAULT);
                var userId = session.GetStringOrEmpty(Constants.SessionVariables.UserId);
                var licenseId = session.GetStringOrEmpty(Constants.SessionVariables.LicenseId);
                var patientId = session.GetStringOrEmpty(Constants.SessionVariables.PatientId);
                var response = new ApiResponse();
                try
                {

                    var patientMedRecDetail = Patient.GetPatientMedRecDetail(patientId, licenseId, dbid);
                    var model = new PatientMedRecDetailModel();


                    if (patientMedRecDetail.Rows.Count == 1)
                    {
                        model = ConstructMedRecModel(patientMedRecDetail.Rows[0]);
                    }
                    else
                    {
                        model = new PatientMedRecDetailModel() { ReconciliationMessage = "Med Reconciliation Status: Medication & Med Allergy reconciliation has not yet been performed on this patient." };
                    }

                    model.DoesPatientHaveValidMedAndAllergy = DoesPatientHasValidMedAndAllergy(session);

                    response.Payload = model;
                    timer.Message = string.Format("<Response>{0}</Response>", response.ToLogString());
                    return response;

                }
                catch (Exception ex)
                {
                    var errorMessage = Audit.AddApiException(userId, licenseId, ex.ToString(), dbid);
                    logger.Error(ex.ToString());
                    response.ErrorContext = new ErrorContextModel()
                    {
                        Error = ErrorTypeEnum.ServerError,
                        Message = errorMessage
                    };
                    timer.Message = string.Format("<ErrorContext>{0}</ErrorContext>", response.ErrorContext.ToLogString());
                    return response;
                }
            }
        }

        private static PatientMedRecDetailModel ConstructMedRecModel(DataRow row)
        {
            var actionText = "";
            var type = Convert.ToString(row["Type"]);
            switch (type)
            {
                case "TC":
                    {
                        actionText = "Transfer of Care";
                        break;
                    }
                case "MR":
                    {
                        actionText = "Med Rec";
                        break;
                    }
            }
            var date = Convert.ToString(row["LastDoneDate"]);
            var firstName = Convert.ToString(row["FirstName"]);
            var lastName = Convert.ToString(row["LastName"]);
            var medRecText = $"Medication and Med Allergies reconciled on: {date} - {firstName} {lastName} - {actionText}";

            return new PatientMedRecDetailModel
            {
                Type = type, 
                ReconciliationMessage = medRecText
            };
        }

    private static bool DoesPatientHasValidMedAndAllergy(IStateContainer session)
        {
            //Allergies – at least one allergy OR No Known Allergies must be present in the Active allergies section
            //AND
            //Medications – at least one medication OR No Active Medications must be present in the Active Medications list.
            if ((session.GetBooleanOrFalse("PATIENTNoActiveMed") || session.GetStringOrEmpty("ACTIVEMEDICATIONS").Length > 0)
                && (session.GetStringOrEmpty("PATIENTNKA").Equals("Y") || session.GetStringOrEmpty("ALLERGY").Length > 0))
            {
                return true;
            }
            return false;
        }


        [HttpPost]
        public ApiResponse UpdatePatientMedRecDetail([FromBody]string type )
        {
            IStateContainer session = new StateContainer(HttpContext.Current.Session);
            return UpdatePatientMedRecDetail(session, type);
        }

        [HttpPost]
        private static ApiResponse UpdatePatientMedRecDetail(IStateContainer session, string type)
        {
            using (var timer = logger.StartTimer("UpdatePatientMedRecDetail"))
            {
                var dbid = session.Cast(Constants.SessionVariables.DbId, ConnectionStringPointer.ERXDB_DEFAULT);
                var userId = session.GetStringOrEmpty(Constants.SessionVariables.UserId);
                var licenseId = session.GetStringOrEmpty(Constants.SessionVariables.LicenseId);
                var patientId = session.GetStringOrEmpty(Constants.SessionVariables.PatientId);
                var response = new ApiResponse();
                try
                {

                    Patient.SavePatientMedRecDetail(licenseId, patientId, userId, type, dbid);

                    // now get the latest back
                    response = GetMedReconciliationInfo(session);
                    timer.Message = $"<Response>" + response.ToLogString() + "</Response>";

                    return response;
                }
                catch (Exception ex)
                {
                    var errorMessage = Audit.AddApiException(userId, licenseId, ex.ToString(), dbid);
                    logger.Error(ex.ToString());
                    response.ErrorContext = new ErrorContextModel() { Error = ErrorTypeEnum.ServerError, Message = errorMessage };
                    timer.Message = string.Format("<ErrorContext>{0}</ErrorContext>", response.ErrorContext.ToLogString());
                    return response;
                }
            }
        }
    }
}