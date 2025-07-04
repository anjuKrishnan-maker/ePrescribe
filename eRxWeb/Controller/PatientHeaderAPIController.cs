using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.DatabaseSelector;
using Allscripts.ePrescribe.ExtensionMethods;
using Allscripts.ePrescribe.Shared.Logging;
using Allscripts.Impact;
using eRxWeb.AppCode;
using eRxWeb.AppCode.StateUtils;
using eRxWeb.ServerModel;
using eRxWeb.State;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Web;
using System.Web.Http;
using Allscripts.ePrescribe.Data;
using Allscripts.ePrescribe.Objects;
using ServiceStack;
using IPatient = Allscripts.Impact.Interfaces.IPatient;
using Patient = Allscripts.Impact.Patient;
using PatientPrivacy = Allscripts.Impact.PatientPrivacy;
using MedicationInfo = eRxWeb.ServerModel.MedicationInfo;


namespace eRxWeb.Controllers
{

    public class PatientHeaderAPIController : ApiController
    {
        private static readonly ILoggerEx logger = LoggerEx.GetLogger();

        [HttpPost]
        public ApiResponse GetPatientHeaderData([FromBody]string id)
        {
            ApiResponse response = null;
            using (var timer = logger.StartTimer("GetPatientHeaderData"))
            {
                if (id == null || id == "null")
                    response = GetPatientHeader(string.Empty);
                else
                    response = GetPatientHeader(id.ToString());

                timer.Message = $"<Request>{id}</Request><Response>{response.ToLogString()}</Response>";
            }

            return response;
        }

        [HttpPost]
        public ApiResponse GetPatientFromSession()
        {
            ApiResponse response = null;
            IStateContainer session = new StateContainer(HttpContext.Current.Session);
            using (var timer = logger.StartTimer("GetPatientFromSession"))
            {
                response = GetPatientHeader(session.GetStringOrEmpty(Constants.SessionVariables.PatientId));
                timer.Message = $"<Response>{response.ToLogString()}</Response>";
            }

            return response;
        }

        [HttpPost]
        public ApiResponse GetCurrentPatient()
        {
            using (var timer = logger.StartTimer("GetCurrentPatient"))
            {
                IStateContainer requestState = new StateContainer(HttpContext.Current.Session);
                PatientResponse patientResponse = new PatientResponse();
                var response = new ApiResponse();
                try
                {

                    patientResponse.PatientId = requestState.GetStringOrEmpty(Constants.SessionVariables.PatientId);
                    response.Payload = patientResponse;

                }
                catch (Exception ex)
                {
                    var errorMessage = ApiHelper.AuditException(ex.ToString(), requestState);
                    logger.Error("GetCurrentPatient Exception: " + ex.ToString());

                    response.ErrorContext = new ErrorContextModel()
                    {
                        Error = ErrorTypeEnum.ServerError,
                        Message = errorMessage
                    };
                    timer.Message = string.Format("<ErrorContext>{0}</ErrorContext>", response.ErrorContext.ToLogString());
                }
                timer.Message = string.Format("<Response>{0}</Response>", response.ToLogString());
                return response;
            }
        }

        [HttpPost]
        [ExcludeFromCodeCoverage]
        public ApiResponse GetPatientActiveMeds()
        {
            using (var timer = logger.StartTimer("GetPatientActiveMeds"))
            {
                IStateContainer session = new StateContainer(HttpContext.Current.Session);
                var response = new ApiResponse();
                try
                {
                    response.Payload = GetPatientActiveMeds(session, new Patient());
                }
                catch (Exception ex)
                {
                    var errorMessage = ApiHelper.AuditException(ex.ToString(), session);
                    logger.Error("GetPatientActiveMeds Exception: " + ex);

                    response.ErrorContext = new ErrorContextModel
                    {
                        Error = ErrorTypeEnum.ServerError,
                        Message = errorMessage
                    };
                }
                timer.Message = $"<Response>{response.ToLogString()}</Response>";
                return response;
            }
        }

        [HttpPost]
        [ExcludeFromCodeCoverage]
        public ApiResponse GetPatientPharmacy()
        {
            using (var timer = logger.StartTimer("GetPatientPharmacy"))
            {
                IStateContainer session = new StateContainer(HttpContext.Current.Session);
                var response = new ApiResponse();
                try
                {
                    response.Payload = GetPharmacyInfo(session);
                }
                catch (Exception ex)
                {
                    var errorMessage = ApiHelper.AuditException(ex.ToString(), session);
                    logger.Error("GetPatientPharmacy Exception: " + ex);

                    response.ErrorContext = new ErrorContextModel
                    {
                        Error = ErrorTypeEnum.ServerError,
                        Message = errorMessage
                    };
                }
                timer.Message = $"<Response>{response.ToLogString()}</Response>";
                return response;
            }
        }

        [HttpPost]
        [ExcludeFromCodeCoverage]
        public ApiResponse GetPatientDiagnosis()
        {
            using (var timer = logger.StartTimer("GetPatientDiagnosis"))
            {
                IStateContainer session = new StateContainer(HttpContext.Current.Session);
                var response = new ApiResponse();
                try
                {
                    response.Payload = GetDiagnosisInfo(session, new PatientDiagnosisProvider());
                }
                catch (Exception ex)
                {
                    var errorMessage = ApiHelper.AuditException(ex.ToString(), session);
                    logger.Error("GetPatientDiagnosis Exception: " + ex);

                    response.ErrorContext = new ErrorContextModel
                    {
                        Error = ErrorTypeEnum.ServerError,
                        Message = errorMessage
                    };
                }
                timer.Message = $"<Response>{response.ToLogString()}</Response>";
                return response;
            }
        }

        [HttpPost]
        [ExcludeFromCodeCoverage]
        public ApiResponse GetPatientAllergies()
        {
            using (var timer = logger.StartTimer("GetPatientAllergies"))
            {
                IStateContainer session = new StateContainer(HttpContext.Current.Session);
                var response = new ApiResponse();
                try
                {
                    response.Payload = GetAllergyInfo(session, new Patient());
                }
                catch (Exception ex)
                {
                    var errorMessage = ApiHelper.AuditException(ex.ToString(), session);
                    logger.Error("GetPatientAllergies Exception: " + ex);

                    response.ErrorContext = new ErrorContextModel
                    {
                        Error = ErrorTypeEnum.ServerError,
                        Message = errorMessage
                    };
                }
                timer.Message = $"<Response>{response.ToLogString()}</Response>";
                return response;
            }
        }

        private static PhysicianMasterPage MasterPage(IStateContainer pageState)
        {


            var ms = new PhysicianMasterPage();
            ms.PageState = pageState;


            return ms;


        }

        public static ApiResponse GetPatientHeader(string patientId)
        {
            IStateContainer PageState = new StateContainer(HttpContext.Current.Session);

            var response = new ApiResponse();
            try
            {
                var pHdr = GetPatientHeaderData(patientId, PageState);
                response.Payload = pHdr;
            }
            catch (Exception ex)
            {
                var errorMessage = ApiHelper.AuditException(ex.ToString(), PageState);
                logger.Error("GetPatientHeader Exception: " + ex.ToString());
                response.ErrorContext = new ErrorContextModel()
                {
                    Error = ErrorTypeEnum.ServerError,
                    Message = errorMessage
                };
                return response;
            }
            logger.Debug("<patientId>" + patientId + "</patientId><Response>" + response.ToLogString() + "</Response>");
            return response;
        }
        
        private static dynamic GetPatientHeaderData(string patientId, IStateContainer PageState)
        {
            PatientHeader pHdr = new PatientHeader();


            var sessionLicense = PageState.Cast<ApplicationLicense>("SessionLicense", null);
            SetPatientInfo(patientId, true, PageState);//because looks like data is not yet set

            if (!string.IsNullOrEmpty(patientId) && IsAvailableSession("PATIENTID") && IsAvailableSession("PATIENTFIRSTNAME") && IsAvailableSession("PATIENTLASTNAME") && IsAvailableSession("PATIENTDOB"))
            {
                pHdr.FirstName = PageState.GetStringOrEmpty(Constants.SessionVariables.PatientFirstName) + " " + PageState.GetStringOrEmpty(Constants.SessionVariables.PatientMiddleName);
                pHdr.LastName = PageState.GetStringOrEmpty(Constants.SessionVariables.PatientLastName).ToUpper() + ", ";

                pHdr.IsRestrictedUser = PageState.GetBooleanOrFalse("IsRestrictedUser");
                pHdr.IsRestrictedPatient = PageState.GetBooleanOrFalse("IsRestrictedPatient");
                pHdr.IsVipPatient = PageState.GetBooleanOrFalse("IsVIPPatient");
                patientId = PageState.GetStringOrEmpty("PATIENTID");
                pHdr.PatientID = patientId;
                if (pHdr.IsVipPatient)
                {
                    pHdr.ToolTip = pHdr.IsRestrictedUser ? "Not authorized to view real name" : "Click here to view real name";
                    DataRow dr = Patient.GetPatientRealName(patientId, ApiHelper.GetDBID(PageState));
                    pHdr.RealName = dr != null ? dr[1].ToString() + ", " + dr[0].ToString() : null;

                }

                if (!pHdr.IsVipPatient && pHdr.IsRestrictedPatient)
                {
                    pHdr.ToolTip = "";
                }
                if (IsAvailableSession("PATIENTMRN"))
                {
                    pHdr.MRN = " " + PageState.GetStringOrEmpty("PATIENTMRN");
                }


                var dob = PageState.GetStringOrEmpty("PATIENTDOB");
                var dobDt = dob == string.Empty ? DateTime.MinValue : Convert.ToDateTime(dob);
                if (dobDt != DateTime.MinValue)
                {
                    pHdr.DOB = dobDt.ToString("MMMM dd, yyyy");
                    pHdr.DOB += " (" + StringUtil.CalculateAge(dob) + ") | ";
                    pHdr.DOB += StringUtil.SexToFullValue(PageState.GetStringOrEmpty("SEX"));
                }

                var weight = PageState.Cast<Weight>(Constants.SessionVariables.PatientWeight, null);
                pHdr.WeightLabel = Weight.ConvertToLabel(weight);

                var height = PageState.Cast<Height>(Constants.SessionVariables.PatientHeight, null);
                pHdr.HeightLabel = Height.ConvertToLabel(height);

                var diagInfo = GetDiagnosisInfo(PageState, new PatientDiagnosisProvider());
                pHdr.Dx = diagInfo.Dx;
                pHdr.MoreActiveProblem = diagInfo.MoreActiveProblem;
                pHdr.ActiveDignosises = diagInfo.ActiveDiagnosis;

                if (IsAvailableSession("PATIENTMAI_IND") && IsAvailableSession("PATIENTMAI_PER"))
                {
                    int maiIndicator = -1;
                    if (!int.TryParse(PageState.GetStringOrEmpty("PATIENTMAI_IND"), out maiIndicator))
                        maiIndicator = -1;

                    decimal maiPercentage = -1;
                    if (!decimal.TryParse(PageState.GetStringOrEmpty("PATIENTMAI_PER"), out maiPercentage))
                        maiPercentage = -1;
                    pHdr.MaiIndicator = maiIndicator;
                }
                else
                {
                    pHdr.MaiIndicator = -1;
                }

                var allergyInfo = GetAllergyInfo(PageState, new Patient());
                pHdr.Allergy = allergyInfo.Allergy;
                pHdr.ActiveAllergies = allergyInfo.ActiveAllergies;
                pHdr.MoreActiveAllergy = allergyInfo.MoreActiveAllergy;

                var pharmInfo = GetPharmacyInfo(PageState);
                pHdr.LastPharmacyName = pharmInfo.LastPharmacyName;
                pHdr.IsMoEpcs = pharmInfo.IsMoEpcs;
                pHdr.MoreMailOrderPharmVisible = pharmInfo.MoreMailOrderPharmVisible;
                pHdr.MoreRetailPharm = pharmInfo.MoreRetailPharm;
                pHdr.IsRetailEpcs = pharmInfo.IsRetailEpcs;
                pHdr.PrefMOP = pharmInfo.PrefMOP;
                pHdr.RemMOPharmVisible = pharmInfo.RemMOPharmVisible;
                pHdr.RemPharmacyVisible = pharmInfo.RemPharmacyVisible;

                var medInfo = GetPatientActiveMeds(PageState, new Patient());
                pHdr.ActiveMed = medInfo.ActiveMed;
                pHdr.MoreActiveMedVisible = medInfo.MoreActiveMedVisible;
                pHdr.ActiveMeds = medInfo.ActiveMeds;

            }
            else
            {
                pHdr.FirstName = "[No Patient Selected]";
            }
            SetEditOption(PageState, pHdr);
            return pHdr;

        }

        public static AllergyInfo GetAllergyInfo(IStateContainer session, IPatient patient)
        {
            var allergyInfo = new AllergyInfo();

            var allergy = session.GetStringOrEmpty(Constants.SessionVariables.Allergy);
            if (session.GetStringOrEmpty(Constants.SessionVariables.PatientNka).Equals("Y"))
            {
                allergyInfo.Allergy = "No Known Allergies";
            }
            else if (!string.IsNullOrWhiteSpace(allergy))
            {
                if (allergy.Length > 100)
                {
                    allergyInfo.Allergy = allergy.Substring(0, 100);
                    allergyInfo.MoreActiveAllergy = true;
                    var data = patient.GetPatientAllergy(session.GetStringOrEmpty(Constants.SessionVariables.PatientId), session.GetStringOrEmpty(Constants.SessionVariables.LicenseId), session.GetStringOrEmpty(Constants.SessionVariables.UserId), session.Cast(Constants.SessionVariables.DbId, ConnectionStringPointer.ERXDB_DEFAULT));

                    allergyInfo.ActiveAllergies = new List<ActiveAllergy>();
                    foreach (DataRow v in data.Tables[0].Rows)
                    {
                        allergyInfo.ActiveAllergies.Add(new ActiveAllergy()
                        {
                            Name = v["AllergyName"].ToString(),
                            StartDate = v["StartDate"].ToString()
                        });
                    }
                }
                else
                {
                    allergyInfo.Allergy = allergy;
                }

            }
            else
            {
                allergyInfo.Allergy = "None entered";
            }

            return allergyInfo;
        }

        public static DiagnosisInfo GetDiagnosisInfo(IStateContainer session, IPatientDiagnosisProvider diagnosisData)
        {
            var diagInfo = new DiagnosisInfo();

            var activeDx = session.GetStringOrEmpty(Constants.SessionVariables.ActiveDx);
            if (!string.IsNullOrWhiteSpace(activeDx))
            {
                if (activeDx.Length > 100)
                {
                    diagInfo.Dx = activeDx.Substring(0, 100);
                    diagInfo.MoreActiveProblem = true;
                    var data = diagnosisData.GetActiveDiagnosis(session.GetStringOrEmpty(Constants.SessionVariables.PatientId), session.Cast(Constants.SessionVariables.DbId, ConnectionStringPointer.ERXDB_DEFAULT));

                    diagInfo.ActiveDiagnosis = new List<ActiveDignosis>();
                    foreach (var v in data)
                    {
                        diagInfo.ActiveDiagnosis.Add(new ActiveDignosis
                        {
                            Diagnosis = v.Description,
                            StartDate = v.StartDate
                        });
                    }
                }
                else
                {
                    diagInfo.Dx = activeDx;
                }
            }
            else
            {
                diagInfo.Dx = "None entered";
            }

            return diagInfo;
        }

        public static PharmacyInfo GetPharmacyInfo(IStateContainer session)
        {
            var info = new PharmacyInfo();

            var canEditPharm = session.GetBooleanOrFalse(Constants.SessionVariables.EditPharmacy);

            var lastPharmName = session.GetStringOrEmpty(Constants.SessionVariables.LastPharmacyName);
            if (string.IsNullOrWhiteSpace(lastPharmName))
            {
                info.LastPharmacyName = "None entered";
            }
            else
            {
                if (lastPharmName.Length > 48)
                {
                    info.LastPharmacyName = lastPharmName.Substring(0, 48);
                    info.MoreRetailPharm = true;
                }
                else
                {
                    info.LastPharmacyName = lastPharmName;
                }

                info.RemPharmacyVisible = canEditPharm;
                info.IsRetailEpcs = session.GetBooleanOrFalse(Constants.SessionVariables.IsRetailEpcsEnabled);
            }

            var mobNabp = session.GetStringOrEmpty(Constants.SessionVariables.MOB_NABP);
            var mobName = session.GetStringOrEmpty(Constants.SessionVariables.MobName);
            if (string.IsNullOrWhiteSpace(mobNabp) || string.IsNullOrWhiteSpace(mobName))
            {
                info.PrefMOP = "None entered";
            }
            else
            {
                if (mobName.Length > 48)
                {
                    info.PrefMOP = mobName.Substring(0, 48);
                    info.MoreMailOrderPharmVisible = true;
                }
                else
                {
                    info.PrefMOP = mobName;
                }

                info.RemMOPharmVisible = canEditPharm;
                info.IsMoEpcs = session.GetBooleanOrFalse(Constants.SessionVariables.IsMobEpcsEnabled);
            }

            return info;
        }

        private static void SetPatientInfo(string patientID, bool fullDataRefresh, IStateContainer PageState)
        {
            if (!patientID.ToGuidOr0x0().Equals(Guid.Empty))
            {
                bool isUserRestricted = checkIfUserRestricted(patientID, ApiHelper.GetSessionUserID(PageState), ApiHelper.GetDBID(PageState), PageState);
                PageState["IsRestrictedUser"] = isUserRestricted ? true : false;
                if (fullDataRefresh)
                {
                    MasterPage(PageState).SetPatientInfo(patientID);
                }
                if (fullDataRefresh)
                {
                    Patient.AddPatientEligibilityRequest(ApiHelper.GetSessionLicenseID(PageState), ApiHelper.GetSessionUserID(PageState),
                        patientID, null, ApiHelper.GetSessionUserID(PageState), ApiHelper.GetDBID(PageState));
                    Patient.AddPatientToSchedule(ApiHelper.GetSessionLicenseID(PageState), ApiHelper.GetSessionUserID(PageState), patientID, DateTime.Now,
                        string.Empty, string.Empty, string.Empty, ApiHelper.GetSessionUserID(PageState), ApiHelper.GetDBID(PageState));
                }
            }
            else
            {
                ClearPatientInfo(PageState);


            }
        }
        private static bool checkIfUserRestricted(string patientID, string userID, ConnectionStringPointer dbID, IStateContainer pageState)
        {
            DataTable dtPatientPrivacyRequestID = new PatientPrivacy().GetPatientPrivacyRequestID(patientID, userID, dbID);
            if (dtPatientPrivacyRequestID.Rows.Count > 0 && dtPatientPrivacyRequestID.Rows[0]["ID"] != DBNull.Value)
            {
                Int32 PatientPrivacyRequestID = Convert.ToInt32(dtPatientPrivacyRequestID.Rows[0]["ID"].ToString());
                pageState["PatientPrivacyRequestID"] = PatientPrivacyRequestID.ToString();
                return true;
            }
            else
            {
                return false;
            }
        }
        private static void ClearPatientInfo(IStateContainer pageState)
        {
            if (pageState["SSOMode"] == null || (pageState["SSOMode"] != null && (pageState["SSOMode"].ToString() != Constants.SSOMode.PATIENTLOCKDOWNMODE && pageState["SSOMode"].ToString() != Constants.SSOMode.EPAPATIENTLOCKDOWNTASKMODE)))
            {
                PatientInfo.ClearPatientInfo(pageState);
            }
        }

        public static MedicationInfo GetPatientActiveMeds(IStateContainer session, IPatient patient)
        {
            var medInfo = new MedicationInfo();

            if (string.IsNullOrWhiteSpace(session.GetStringOrEmpty(Constants.SessionVariables.PatientId))) return medInfo;

            var activeMedsSession = session.GetStringOrEmpty(Constants.SessionVariables.ActiveMedications);

            medInfo.MoreActiveMedVisible = false;
            if (session.GetBooleanOrFalse(Constants.SessionVariables.PatientNoActiveMed))
            {
                medInfo.ActiveMed = "No Active Medications";
            }
            else if (activeMedsSession?.Length > 0)
            {
                if (activeMedsSession.Length > 100)
                {
                    medInfo.ActiveMed = activeMedsSession.Substring(0, 100);
                    medInfo.MoreActiveMedVisible = true;

                    medInfo.ActiveMeds = new List<ActiveMeds>();
                    var data = patient.GetPatientActiveMedication(session.GetStringOrEmpty(Constants.SessionVariables.PatientId), session.GetStringOrEmpty(Constants.SessionVariables.LicenseId), session.GetStringOrEmpty("USERID"), session.Cast(Constants.SessionVariables.DbId, ConnectionStringPointer.AUDIT_ERXDB_SERVER_1));
                    foreach (DataRow v in data.Tables[0].Rows)
                    {
                        medInfo.ActiveMeds.Add(new ActiveMeds()
                        {
                            Name = v["Medication"].ToString(),
                            StartDate = v["StartDate"].ToString()
                        });
                    }
                }
                else
                {
                    medInfo.ActiveMed = activeMedsSession;
                }
            }
            else
            {
                medInfo.ActiveMed = "None entered";
            }

            return medInfo;
        }

        private static bool IsAvailableSession(string key)
        {
            return HttpContext.Current.Session[key] != null;
        }

        private static void GetPharmacyDetails(IStateContainer PageState, PatientHeader pt)
        {
            var phmObj = new Allscripts.Impact.Pharmacy();

            var data = Allscripts.Impact.Pharmacy.LoadPharmacy(PageState.GetStringOrEmpty("LASTPHARMACYID"), PageState.Cast<ConnectionStringPointer>("DBID", ConnectionStringPointer.AUDIT_ERXDB_SERVER_1));

            foreach (DataRow v in data.Tables[0].Rows)
            {
                pt.ActiveMeds.Add(new ActiveMeds()
                {
                    Name = v["Medication"].ToString(),
                    StartDate = v["StartDate"].ToString()
                });
            }
        }

        private static void SetEditOption(IStateContainer Pagestate, PatientHeader pt)
        {
            pt.AllowPatientEdit = Pagestate.GetBooleanOrFalse("EditPatient");
            pt.AllowAllergyEdit = Pagestate.GetBooleanOrFalse("EditAllergy");
            pt.AllowDiagnosisEdit = Pagestate.GetBooleanOrFalse("EditDiagnosis");
            pt.AllowPharmacyEdit = Pagestate.GetBooleanOrFalse("EditPharmacy");
        }

    }
}