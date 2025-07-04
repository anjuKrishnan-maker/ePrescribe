using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.DatabaseSelector;
using Allscripts.Impact;
using eRxWeb.AppCode;
using eRxWeb.State;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Services;
using Allscripts.ePrescribe.ExtensionMethods;
using Allscripts.ePrescribe.Shared.Logging;
using eRxWeb.AppCode.StateUtils;
using eRxWeb.ServerModel;
using Microsoft.IdentityModel.Protocols.WSFederation.Metadata;
using System.Web.Http;
using eRxWeb.ServerModel.Request;

namespace eRxWeb.Controllers
{
    public partial class ReviewHistoryApiController : ApiController
    {
        private static ILoggerEx logger = LoggerEx.GetLogger();

        [HttpPost]
        public ApiResponse GetPatientReviewHistory(GetReviewHistory data)
        {
            IStateContainer session = new StateContainer(HttpContext.Current.Session);

            return GetPatientReviewHistory(session, data);
        }

        private static ApiResponse GetPatientReviewHistory(IStateContainer session, GetReviewHistory data)
        {
            using (var timer = logger.StartTimer("GetPatientReviewHistory"))
            {
                var dbid = session.Cast(Constants.SessionVariables.DbId, ConnectionStringPointer.ERXDB_DEFAULT);
                var userId = session.GetStringOrEmpty(Constants.SessionVariables.UserId);
                var licenseId = session.GetStringOrEmpty(Constants.SessionVariables.LicenseId);
                var patientId = session.GetStringOrEmpty(Constants.SessionVariables.PatientId);
                var response = new ApiResponse();
                try
                {
                    var reviewHistoryDataModels = Prescription.GetPatientReviewHistory(
                    patientId, licenseId, data.statusFilter.ToDatabaseConstant(), data.dataRetrievalContext.SortColumnName,
                    data.dataRetrievalContext.SortDirection.ToString(), data.dataRetrievalContext.SkipRows,
                    data.dataRetrievalContext.FetchRows, dbid);

                    var result = new List<ReviewHistoryModel>();

                    var rowCount = reviewHistoryDataModels.Count;
                    var moreRowsAvailable = rowCount > data.dataRetrievalContext.FetchRows;

                    for (int rowCounter = 0; rowCounter < Math.Min(data.dataRetrievalContext.FetchRows, rowCount); rowCounter++)
                    {
                        var item = ReviewHistoryModelCreator.Create(reviewHistoryDataModels[rowCounter], dbid, session);
                        result.Add(item);

                    }
                    var activeMedsPresent = session.GetStringOrEmpty("ACTIVEMEDICATIONS").Length > 0;
                    response.Payload = new GetPatientReviewHistoryResponse() { HistoryItems = result, MoreRowsAvailable = moreRowsAvailable, ActiveMedsPresent = activeMedsPresent };
                }
                catch (Exception ex)
                {
                    var errorMessage = Audit.AddApiException(userId, licenseId, ex.ToString(), dbid);
                    logger.Error(ex.ToString());
                    response.ErrorContext = new ErrorContextModel() { Error = ErrorTypeEnum.ServerError, Message = errorMessage };
                    timer.Message = string.Format("<ErrorContext>{0}</ErrorContext>", response.ErrorContext.ToLogString());
                }
                timer.Message = string.Format("<Request>{0}</Request><Response>{1}</Response>", $"<dbid>{dbid.ToLogString()}</dbid><patientId>{patientId.ToLogString()}</patientId>", response.ToLogString());
                return response;
            }
        }

        [HttpPost]
        public ApiResponse GetFillHistoryData([FromBody]string rxId)
        {
            IStateContainer session = new StateContainer(HttpContext.Current.Session);

            return GetFillHistoryData(rxId, session);
        }

        private static ApiResponse GetFillHistoryData(string rxId, IStateContainer session)
        {
            using (var timer = logger.StartTimer("GetFillHistoryData"))
            {
                var dbid = session.Cast(Constants.SessionVariables.DbId, ConnectionStringPointer.ERXDB_DEFAULT);
                var userId = session.GetStringOrEmpty(Constants.SessionVariables.UserId);
                var licenseId = session.GetStringOrEmpty(Constants.SessionVariables.LicenseId);
                var response = new ApiResponse();

                try
                {
                    var fillRecords = Allscripts.ePrescribe.Data.RxFill.GetFillDatesForRxId(rxId, dbid).ToList();

                    var result = new List<FillHistoryTimeLineModel>();

                    foreach (var fillRecord in fillRecords)
                    {
                        var item = new FillHistoryTimeLineModel(fillRecord);
                        result.Add(item);
                    }
                    response.Payload = result;
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
                }
                timer.Message = string.Format("<Request>{0}</Request><Response>{1}</Response>", $"<dbid>{dbid.ToLogString()}</dbid><rxId>{rxId.ToLogString()}</rxId>", response.ToLogString());
                return response;
            }
        }

        [HttpPost]
        public ApiResponse ExecuteCompleteActionMethod(List<string> rxIdList)
        {
            IStateContainer pageState = new StateContainer(HttpContext.Current.Session);
            var ip = HttpContext.Current.Request.UserIpAddress();
            return ExecuteCompleteAction(pageState, ip, rxIdList);
        }

        private static ApiResponse ExecuteCompleteAction(IStateContainer session, string ip, List<string> rxIdList)
        {
            using (var timer = logger.StartTimer("ExecuteCompleteAction"))
            {
                var extFacilityCode = session.GetStringOrEmpty("ExtFacilityCd");
                var extGroupID = session.GetStringOrEmpty("ExtGroupID");
                var dbid = session.Cast(Constants.SessionVariables.DbId, ConnectionStringPointer.ERXDB_DEFAULT);
                var userId = session.GetStringOrEmpty(Constants.SessionVariables.UserId);
                var licenseId = session.GetStringOrEmpty(Constants.SessionVariables.LicenseId);
                var patientId = session.GetStringOrEmpty(Constants.SessionVariables.PatientId);
                var response = new ApiResponse();
                try
                {
                    foreach (var rxId in rxIdList)
                    {
                        Prescription.Complete(rxId, userId, licenseId, extFacilityCode, extGroupID, dbid);
                        EPSBroker.AuditLogPatientInsert(ePrescribeSvc.AuditAction.PATIENT_RX_MODIFY, licenseId, userId, patientId, ip, dbid);
                        timer.Message = string.Format("<Request>{0}</Request><Response>{1}</Response>", $"<dbid>{dbid.ToLogString()}</dbid><rxId>{rxId.ToLogString()}</rxId><ip>{ip.ToLogString()}</ip><extFacilityCode>{extFacilityCode.ToLogString()}</extFacilityCode><patientId>{patientId.ToLogString()}</patientId>", response.ToLogString());
                    }

                    UpdatePatientActiveMeds(session, dbid, patientId, userId, licenseId);
                }
                catch (Exception ex)
                {
                    var errorMessage = Audit.AddApiException(userId, licenseId, ex.ToString(), dbid);
                    logger.Error(ex.ToString());
                    response.ErrorContext = new ErrorContextModel() { Error = ErrorTypeEnum.ServerError, Message = errorMessage };
                    timer.Message = string.Format("<ErrorContext>{0}</ErrorContext>", response.ErrorContext.ToLogString());
                }
                return response;
            }
        }

        [HttpPost]
        public ApiResponse ExecuteDiscontinueAction(string[] rxIdList)
        {
            IStateContainer session = new StateContainer(HttpContext.Current.Session);
            var ip = HttpContext.Current.Request.UserIpAddress();

            return ExecuteDiscontinueAction(session, ip, rxIdList);
        }

        private static ApiResponse ExecuteDiscontinueAction(IStateContainer session, string ip, string[] rxIdList)
        {
            using (var timer = logger.StartTimer("ExecuteDiscontinueAction"))
            {
                var extFacilityCode = session.GetStringOrEmpty("ExtFacilityCd");
                var extGroupID = session.GetStringOrEmpty("ExtGroupID");
                var dbid = session.Cast(Constants.SessionVariables.DbId, ConnectionStringPointer.ERXDB_DEFAULT);
                var userId = session.GetStringOrEmpty(Constants.SessionVariables.UserId);
                var licenseId = session.GetStringOrEmpty(Constants.SessionVariables.LicenseId);
                var patientId = session.GetStringOrEmpty(Constants.SessionVariables.PatientId);
                var response = new ApiResponse();
                try
                {
                    foreach (var rxId in rxIdList)
                    {
                        Prescription.Discontinue(rxId, "1", DateTime.Today.ToShortDateString(), string.Empty, string.Empty,
                            userId, licenseId, extFacilityCode, extGroupID, dbid);
                        EPSBroker.AuditLogPatientInsert(ePrescribeSvc.AuditAction.PATIENT_RX_MODIFY, licenseId, userId, patientId, ip, dbid);
                        timer.Message = string.Format("<Request>{0}</Request><Response>{1}</Response>", $"<dbid>{dbid.ToLogString()}</dbid><rxId>{rxId.ToLogString()}</rxId><ip>{ip.ToLogString()}</ip><extFacilityCode>{extFacilityCode.ToLogString()}</extFacilityCode><patientId>{patientId.ToLogString()}</patientId>", response.ToLogString());
                    }

                    UpdatePatientActiveMeds(session, dbid, patientId, userId, licenseId);
                }
                catch (Exception ex)
                {
                    var errorMessage = Audit.AddApiException(userId, licenseId, ex.ToString(), dbid);
                    logger.Error(ex.ToString());
                    response.ErrorContext = new ErrorContextModel() { Error = ErrorTypeEnum.ServerError, Message = errorMessage };
                    timer.Message = string.Format("<ErrorContext>{0}</ErrorContext>", response.ErrorContext.ToLogString());
                }
                return response;
            }

        }

        [HttpPost]
        public ApiResponse ExecuteEieAction(EieActionRequestModel[] eieItemList)
        {
            IStateContainer session = new StateContainer(HttpContext.Current.Session);
            var ip = HttpContext.Current.Request.UserIpAddress();

            return ExecuteEieAction(session, ip, eieItemList);
        }

        private static ApiResponse ExecuteEieAction(IStateContainer session, string ip, EieActionRequestModel[] eieItemList)
        {
            using (var timer = logger.StartTimer("ExecuteEieAction"))
            {
                var extFacilityCode = session.GetStringOrEmpty("ExtFacilityCd");
                var extGroupID = session.GetStringOrEmpty("ExtGroupID");
                var dbid = session.Cast(Constants.SessionVariables.DbId, ConnectionStringPointer.ERXDB_DEFAULT);
                var userId = session.GetStringOrEmpty(Constants.SessionVariables.UserId);
                var licenseId = session.GetStringOrEmpty(Constants.SessionVariables.LicenseId);
                var patientId = session.GetStringOrEmpty(Constants.SessionVariables.PatientId);
                var response = new ApiResponse();
                try
                {
                    foreach (var eieItem in eieItemList)
                    {
                        Prescription.EnteredInError(eieItem.RxID, eieItem.IsPbmMed, userId, licenseId, extFacilityCode, extGroupID, dbid);
                        EPSBroker.AuditLogPatientInsert(ePrescribeSvc.AuditAction.PATIENT_RX_MODIFY, licenseId, userId, patientId, ip, dbid);
                        timer.Message = string.Format("<Request>{0}</Request><Response>{1}</Response>", $"<dbid>{dbid.ToLogString()}</dbid><PBMMed>{eieItem.IsPbmMed.ToLogString()}</PBMMed><rxId>{eieItem.RxID.ToLogString()}</rxId><ip>{ip.ToLogString()}</ip><extFacilityCode>{extFacilityCode.ToLogString()}</extFacilityCode><patientId>{patientId.ToLogString()}</patientId>", response.ToLogString());
                    }

                    UpdatePatientActiveMeds(session, dbid, patientId, userId, licenseId);
                }

                catch (Exception ex)
                {
                    var errorMessage = Audit.AddApiException(userId, licenseId, ex.ToString(), dbid);
                    logger.Error(ex.ToString());
                    response.ErrorContext = new ErrorContextModel() { Error = ErrorTypeEnum.ServerError, Message = errorMessage };
                    timer.Message = string.Format("<ErrorContext>{0}</ErrorContext>", response.ErrorContext.ToLogString());
                }
                return response;
            }
        }

        private static void UpdatePatientActiveMeds(IStateContainer session, ConnectionStringPointer dbId, string patientId, string userId, string licenseId)
        {
            // this taken 'as is' from legacy page. Needs TLC

            if (!string.IsNullOrEmpty(patientId))
            {
                string patientID = session["PATIENTID"].ToString();
                //Retrieve the patient's distinct active medications 
                DataSet activeMeds = Patient.GetPatientActiveMedications(patientID, licenseId, userId, dbId);
                if (activeMeds.Tables["Medications"].Rows.Count > 0)
                {
                    StringBuilder activeMedications = new StringBuilder();
                    List<string> activeMedDDIList = new List<string>();

                    foreach (DataRow dr in activeMeds.Tables["Medications"].Rows)
                    {
                        if (activeMedications.Length > 0)
                        {
                            activeMedications.Append(", ");
                        }

                        activeMedications.Append(dr["MedicationName"].ToString().Trim());
                        activeMedDDIList.Add(dr["DDI"].ToString());
                    }

                    session["ACTIVEMEDICATIONS"] = activeMedications.ToString();
                    session["ACTIVEMEDDDILIST"] = activeMedDDIList;

                }
                else
                {
                    session.Remove("ACTIVEMEDICATIONS");
                    session.Remove("ACTIVEMEDDDILIST");

                }
            }
        }

        [HttpPost]
        public ApiResponse AuditAccessAndGetStartupParameters()
        {
            IStateContainer session = new StateContainer(HttpContext.Current.Session);
            var ip = HttpContext.Current.Request.UserIpAddress();

            return  AuditAccessAndGetStartupParameters(session, ip);
        }

        private static ApiResponse AuditAccessAndGetStartupParameters(IStateContainer session, string ip)
        {
            using (var timer = logger.StartTimer("AuditAccessAndGetStartupParameters"))
            {
                var dbid = session.Cast(Constants.SessionVariables.DbId, ConnectionStringPointer.ERXDB_DEFAULT);
                var userId = session.GetStringOrEmpty(Constants.SessionVariables.UserId);
                var licenseId = session.GetStringOrEmpty(Constants.SessionVariables.LicenseId);
                var patientId = session.GetStringOrEmpty(Constants.SessionVariables.PatientId);
                var siteId = session.GetInt(Constants.SessionVariables.SiteId, 1);
                var response = new ApiResponse();
                try
                {

                    EPSBroker.AuditLogPatientInsert(ePrescribeSvc.AuditAction.PATIENT_HEADER_VIEW, licenseId, userId, patientId, ip, dbid);
                    EPSBroker.AuditLogPatientInsert(ePrescribeSvc.AuditAction.PATIENT_RX_VIEW, licenseId, userId, patientId, ip, dbid);

                    var startupParameters = new ReviewHistoryStartupParameters();

                    startupParameters.IsRestrictedPatient = session.GetBooleanOrFalse("IsVIPPatient") || session.GetBooleanOrFalse("IsRestrictedPatient");

                    // copied comments from legacy:
                    // removing RestrictedUserOverridden from session, as it is not required here (if the restricted user has provided the override reason & landed to this page
                    session.Remove(Constants.SessionVariables.RestrictedUserOverridden);

                    // Add Diagnosis
                    var license = session.Cast("SessionLicense", new ApplicationLicense());
                    startupParameters.IsAddDiagnosisVisible = license.EnterpriseClient?.AddDiagnosis == true &&
                                                              session.GetBooleanOrFalse("AddDiagnosis");

                    // lockdown mode
                    if (session.GetStringOrEmpty("SSOMode") == Constants.SSOMode.PATIENTLOCKDOWNMODE)
                    {
                        PatientInfo.SetPatientInfo(patientId, session, dbid);
                        startupParameters.IsSsoLockdownMode = true;
                    }

                    startupParameters.IsInactivePatient = session.GetStringOrEmpty("PATIENTSTATUS") == "0";
                    startupParameters.UserType = GetUserType(session);

                    if (startupParameters.UserType == ReviewHistoryUserType.PAwithSupervision)
                    {
                        startupParameters.Providers = LoadProviders(session, licenseId, siteId, userId, dbid);

                        startupParameters.DelegateProviderId = session.GetStringOrEmpty("DelegateProviderID");
                        if (startupParameters.DelegateProviderId == string.Empty)
                        {
                            startupParameters.DelegateProviderId = RxUser.GetPOBProviderID(licenseId, siteId, userId, dbid);
                            session["DelegateProviderID"] = startupParameters.DelegateProviderId;
                        }
                    }

                    response.Payload = startupParameters;
                    
                    // Audit

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
                }
                timer.Message = string.Format("<Request>{0}</Request><Response>{1}</Response>", ip.ToString(), response.ToLogString());
                return response;

            }

        }

        private static ReviewHistoryUserType GetUserType(IStateContainer session)
        {
            if (AppCode.StateUtils.UserInfo.IsPOBUser(session) || session.GetBooleanOrFalse("IsPASupervised"))
                return ReviewHistoryUserType.PAwithSupervision;

            if (session.GetBooleanOrFalse("ISPROVIDER") || session.GetBooleanOrFalse("IsPA"))
            {
                return ReviewHistoryUserType.Provider;
            }

            return ReviewHistoryUserType.Staff;

        }

        private static List<ReviewHistoryProvider> LoadProviders(IStateContainer session, string licenseId, int siteId, string userId, ConnectionStringPointer dbid)
        {
            // if a user is POB, load all providers only if IsPOBViewAllProviders is true
            // otherwise it's PA w/ sup, load all providers, too
            var loadAllActiveProviders = session.GetBooleanOrFalse("IsPOBViewAllProviders") || !AppCode.StateUtils.UserInfo.IsPOBUser(session);
            // if false, only selected providers are loaded for the POB.


            var result = new List<ReviewHistoryProvider>();
            DataSet ds = loadAllActiveProviders
                ? Provider.GetProviders(licenseId, siteId, dbid) 
                : Provider.GetProviders(licenseId, siteId, userId, dbid);
            // TODO: rewrite with linq, or get another sproc.
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                if (!Convert.ToString(row["Active"]).GetBooleanFromYOrN())// skip not active
                {
                    continue;
                }
                
                var userType = Convert.ToString(row["UserType"]);
                if (AppCode.StateUtils.UserInfo.GetSessionUserType(session) == Constants.UserCategory.PHYSICIAN_ASSISTANT_SUPERVISED && userType != "1")
                {
                    continue; // skip all but providers for POB with Sup.
                }

                result.Add(new ReviewHistoryProvider() {
                    ProviderId = Convert.ToString(row["ProviderID"]),
                    ProviderName = Convert.ToString(row["ProviderName"]),
                    UserTypeID = Convert.ToInt16(userType)
               }); 
            }

            result.Insert(0, new ReviewHistoryProvider() {ProviderId = "", ProviderName = "Select Provider"});

            return result;


        }

        [HttpPost]
        public ApiResponse SetNoActiveMedsFlag()
        {
            IStateContainer session = new StateContainer(HttpContext.Current.Session);
            var ip = HttpContext.Current.Request.UserIpAddress();

            return SetNoActiveMedsFlag(session, ip);
        }

        private static ApiResponse SetNoActiveMedsFlag(IStateContainer session, string ip)
        {
            using (var timer = logger.StartTimer("SetNoActiveMedsFlag"))
            {
                var dbid = session.Cast(Constants.SessionVariables.DbId, ConnectionStringPointer.ERXDB_DEFAULT);
                var userId = session.GetStringOrEmpty(Constants.SessionVariables.UserId);
                var licenseId = session.GetStringOrEmpty(Constants.SessionVariables.LicenseId);
                var patientId = session.GetStringOrEmpty(Constants.SessionVariables.PatientId);
                var response = new ApiResponse();
                try
                {

                    if (!string.IsNullOrEmpty(patientId))
                    {

                        Patient.MarkNoActiveMed(patientId, licenseId, userId, dbid);
                        session["PATIENTNoActiveMed"] = true;
                        EPSBroker.AuditLogPatientInsert(ePrescribeSvc.AuditAction.PATIENT_RX_CREATED, licenseId, userId, patientId, ip, dbid);
                    }
                }
                catch (Exception ex)
                {
                    var errorMessage = Audit.AddApiException(userId, licenseId, ex.ToString(), dbid);
                    logger.Error(ex.ToString());
                    response.ErrorContext = new ErrorContextModel() { Error = ErrorTypeEnum.ServerError, Message = errorMessage };
                    timer.Message = string.Format("<ErrorContext>{0}</ErrorContext>", response.ErrorContext.ToLogString());
                }
                timer.Message = string.Format("<Request>{0}</Request><Response>{1}</Response>", $"<dbid>{dbid.ToLogString()}</dbid><ip>{ip.ToLogString()}</ip><patientId>{patientId.ToLogString()}</patientId>", response.ToLogString());
                return response;
            }
        }

        [HttpPost]
        public ApiResponse IsAnyOfSelectedMedsAssociatedWithActiveEpaTask(List<string> rxIdList)
        {
                IStateContainer session = new StateContainer(HttpContext.Current.Session);
                var ip = HttpContext.Current.Request.UserIpAddress();
            return IsAnyOfSelectedMedsAssociatedWithActiveEpaTask(session, ip, rxIdList);
       }

        private static ApiResponse IsAnyOfSelectedMedsAssociatedWithActiveEpaTask(IStateContainer session, string ip, List<string> rxIdList)
        {
            using (var timer = logger.StartTimer("IsAnyOfSelectedMedsAssociatedWithActiveEpaTask"))
            {
                var response = new ApiResponse();

                var dbid = session.Cast(Constants.SessionVariables.DbId, ConnectionStringPointer.ERXDB_DEFAULT);
                var userId = session.GetStringOrEmpty(Constants.SessionVariables.UserId);
                var licenseId = session.GetStringOrEmpty(Constants.SessionVariables.LicenseId);

                var epaTaskExist = false;
                try
                {
                    foreach (var rxId in rxIdList)
                    {
                        if (ePA.IsRxIdAssociatedWithActiveEpaTask(rxId, dbid))
                        {
                            epaTaskExist = true;
                            timer.Message = string.Format("<Request>{0}</Request><Response>{1}</Response>", $"<dbid>{dbid.ToLogString()}</dbid><epaTaskExist>{epaTaskExist.ToLogString()}</epaTaskExist><rxId>{rxId.ToLogString()}</rxId><ip>{ip.ToLogString()}</ip>", response.ToLogString());
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    var errorMessage = Audit.AddApiException(userId, licenseId, ex.ToString(), dbid);
                    logger.Error(ex.ToString());
                    response.ErrorContext = new ErrorContextModel() { Error = ErrorTypeEnum.ServerError, Message = errorMessage };
                    timer.Message = string.Format("<ErrorContext>{0}</ErrorContext>", response.ErrorContext.ToLogString());
                    return response;
                }

                response.Payload = epaTaskExist;
                return response;
            }
        }


        [HttpPost]
        public ApiResponse AssignSupervisingProvider([FromBody]string providerId)
        {
            IStateContainer session = new StateContainer(HttpContext.Current.Session);
            var response = new ApiResponse();
            var dbid = session.Cast(Constants.SessionVariables.DbId, ConnectionStringPointer.ERXDB_DEFAULT);
            try
            {
                var prescribingDelegate = new RxUser(providerId, dbid);

                var errorMessage = PrescriberOnRecord.SetInSession(prescribingDelegate, session, new RxUser(), new SPI(), new DeaScheduleUtility());

                if (!string.IsNullOrWhiteSpace(errorMessage))
                {
                    response.ErrorContext = new ErrorContextModel
                    {
                        Error = ErrorTypeEnum.UserMessage,
                        Message = errorMessage
                    };
                }
            }
            catch (Exception ex)
            {
                var userId = session.GetStringOrEmpty(Constants.SessionVariables.UserId);
                var licenseId = session.GetStringOrEmpty(Constants.SessionVariables.LicenseId);
                var errorMessage = Audit.AddApiException(userId, licenseId, ex.ToString(), dbid);
                logger.Error(ex.ToString());
                response.ErrorContext = new ErrorContextModel() {Error = ErrorTypeEnum.ServerError, Message = errorMessage};
            }

            return response;
        }
    }
}