using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.Shared.Logging;
using Allscripts.Impact;
using Allscripts.Impact.PreBuildPrescription;
using eRxWeb.AppCode;
using eRxWeb.ServerModel;
using eRxWeb.State;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.UI;

namespace eRxWeb.Controller
{
    public class SelectMedicationSearchApiController : ApiController
    {
        private static readonly ILoggerEx logger = LoggerEx.GetLogger();

        [HttpPost]
        public ApiResponse SearchMedicationName([FromBody]string id)
        {
            using (var timer = logger.StartTimer("SearchMedicationName"))
            {
                var response = new ApiResponse();
                IStateContainer session = new StateContainer(HttpContext.Current.Session);

                try
                {
                    response.Payload = searchMedicationName(id, session);
                }
                catch (Exception ex)
                {
                    response.ErrorContext = new ErrorContextModel() { Error = ErrorTypeEnum.ServerError, Message = ex.Message };
                    logger.Error("SearchMedicationName Exception: " + ex.ToString());
                }
                timer.Message = string.Format("<Response>{0}</Response>", response.ToLogString());
                return response;
            }
        }

        [HttpPost]
        public ApiResponse GetPrebuiltPrescriptionGroupNames()
        {
            using (var timer = logger.StartTimer("GetLicensePrebuiltPrescriptionGroupNames"))
            {
                var response = new ApiResponse();
                IStateContainer session = new StateContainer(HttpContext.Current.Session);

                try
                {
                    response.Payload = getPrebuiltPrescriptionGroupNames(session);
                }
                catch (Exception ex)
                {
                    response.ErrorContext = new ErrorContextModel() { Error = ErrorTypeEnum.ServerError, Message = ex.Message };
                    logger.Error("GetPrebuiltPrescriptionGroupNames Exception: " + ex.ToString());
                }
                timer.Message = string.Format("<Response>{0}</Response>", response.ToLogString());
                return response;
            }

        }
        private static string[] searchMedicationName(string searchText, IStateContainer session)
        {
            var dbId = ApiHelper.GetDBID(session);
            string genericEquivalentSearch  = session.GetStringOrEmpty("GenericEquivalentSearch") == "Y" ? "Y" : "N";
            return Medication.QueryMeds(searchText, 10, genericEquivalentSearch, dbId);
        }


        private string[] getPrebuiltPrescriptionGroupNames(IStateContainer session)
        {
            PreBuildPrescription preBuilteRx = new PreBuildPrescription();

            var dbId = ApiHelper.GetDBID(session);
            var licenseId = ApiHelper.GetSessionLicenseID(session);
            var preBuiltPrescriptionGroups = preBuilteRx.GetPreBuiltPrescriptionGroup(licenseId, true, null, dbId);
            return preBuiltPrescriptionGroups.Select(x => x.Name).ToArray(); ;
        }
        [HttpPost]
        public ApiResponse RetrieveTaskTypeParameters()
        {
            using (var timer = logger.StartTimer("RetrieveTaskTypeParameters"))
            {
                var response = new ApiResponse();
                IStateContainer session = new StateContainer(HttpContext.Current.Session);

                try
                {
                    response.Payload = RetrieveTaskTypeParameters(session);
                }
                catch (Exception ex)
                {
                    response.ErrorContext = new ErrorContextModel() { Error = ErrorTypeEnum.ServerError, Message = ex.Message };
                    logger.Error("RetrieveTaskTypeParameters Exception: " + ex.ToString());
                }
                timer.Message = string.Format("<Response>{0}</Response>", response.ToLogString());
                return response;
            }

        }
        private RetrieveTaskTypeParameters RetrieveTaskTypeParameters(IStateContainer session)
        {
            var taskType = session.Cast(Constants.SessionVariables.TaskType, Constants.PrescriptionTaskType.DEFAULT);
            var isCsRefReqWorkflow = session.GetBooleanOrFalse(Constants.SessionVariables.IsCsRefReqWorkflow);
            var isCsRxChgWorkflow = session[Constants.SessionVariables.ChangeRxRequestedMedCs] != null;
            var isReconcileRxREFREQNonCSWorkflow = session.GetBooleanOrFalse(Constants.SessionVariables.IsReconcileREFREQNonCS);
            return new RetrieveTaskTypeParameters(taskType, isCsRefReqWorkflow, isCsRxChgWorkflow, isReconcileRxREFREQNonCSWorkflow);
        }

        [HttpPost]
        public ApiResponse GetStartUpParameters()
        {
            using (var timer = logger.StartTimer("GetStartupParameters"))
            {
                var response = new ApiResponse();
                IStateContainer session = new StateContainer(HttpContext.Current.Session);

                try
                {
                    response.Payload = getStartupParameters(session);
                }
                catch (Exception ex)
                {
                    response.ErrorContext = new ErrorContextModel() { Error = ErrorTypeEnum.ServerError, Message = ex.Message };
                    logger.Error("GetStartUpParameters Exception: " + ex.ToString());
                }
                timer.Message = string.Format("<Response>{0}</Response>", response.ToLogString());
                return response;
            }

        }

        private SelectMedicationSearchStartUpParameters getStartupParameters(IStateContainer session)
        {
            
            var sessionLicense = session.Cast("SessionLicense", default(ApplicationLicense));

            bool isShowPreBuiltGroup = (sessionLicense.EnterpriseClient.EnablePrebuiltPrescriptions &&
                   session.GetBooleanOrFalse("ShowPrebuiltPrescriptions") &&
                   (sessionLicense.LicenseDeluxeStatus == Constants.DeluxeFeatureStatus.On ||
                   sessionLicense.LicenseDeluxeStatus == Constants.DeluxeFeatureStatus.AlwaysOn ||
                   sessionLicense.LicenseDeluxeStatus == Constants.DeluxeFeatureStatus.Cancelled));

            string historyOptionDisplayText = string.Empty;
            switch (session.Cast<Constants.UserCategory>("UserType", Constants.UserCategory.GENERAL_USER))
            {
                case Constants.UserCategory.PROVIDER:
                case Constants.UserCategory.PHYSICIAN_ASSISTANT:
                case Constants.UserCategory.PHYSICIAN_ASSISTANT_SUPERVISED:
                    historyOptionDisplayText = "My History";
                    break;
                default:
                    historyOptionDisplayText = "Dr History";
                    break;
            }

            bool isCSTask = (session.GetBooleanOrFalse(Constants.SessionVariables.IsCsRefReqWorkflow)
                                    || session[Constants.SessionVariables.ChangeRxRequestedMedCs] != null);

            return new SelectMedicationSearchStartUpParameters(historyOptionDisplayText, isShowPreBuiltGroup, isCSTask);
        }
    }
}
