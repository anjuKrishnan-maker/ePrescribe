using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.UI;
using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.ExtensionMethods;
using Allscripts.ePrescribe.Objects;
using Allscripts.ePrescribe.Objects.PPTPlus;
using Allscripts.ePrescribe.Shared.Logging;
using ConnectionStringPointer = Allscripts.ePrescribe.DatabaseSelector.ConnectionStringPointer;
using Allscripts.Impact;
using IPatient = Allscripts.Impact.Interfaces.IPatient;
using Patient = Allscripts.Impact.Patient;
using Allscripts.Impact.ePrescribeSvc;
using Cassini;
using ComponentSpace.SAML.Protocol;
using eRxWeb.AppCode;
using eRxWeb.ServerModel;
using eRxWeb.ServerModel.Request;
using eRxWeb.State;
using GetMessageTrackingAcksResponse = eRxWeb.ePrescribeSvc.GetMessageTrackingAcksResponse;
using MessageIcon = eRxWeb.ServerModel.MessageIcon;
using PatientInfo = eRxWeb.AppCode.StateUtils.PatientInfo;
using Provider = Allscripts.Impact.Provider;
using eRxWeb.AppCode.Interfaces;
using eRxWeb.ePrescribeSvc;
using Height = Allscripts.ePrescribe.Objects.Height;


namespace eRxWeb.Controller
{
    public partial class SelectPatientApiController : ApiController
    {
        private static readonly ILoggerEx logger = LoggerEx.GetLogger();

        IStateContainer _session = new StateContainer(HttpContext.Current.Session);

        private ApplicationLicense SessionLicense
        {
            get
            {
                IStateContainer session = new StateContainer(HttpContext.Current.Session);
                if (session["SessionLicense"] == null)
                {
                    if (session["DBID"] == null)
                    {
                        session["SessionLicense"] = new ApplicationLicense(string.Empty, 0, string.Empty, ConnectionStringPointer.ERXDB_DEFAULT);
                    }
                    else
                    {
                        session["SessionLicense"] = new ApplicationLicense(string.Empty, 0, string.Empty, (ConnectionStringPointer)session["DBID"]);
                    }
                }

                return (ApplicationLicense)session["SessionLicense"];
            }
        }

        [HttpPost]
        public ApiResponse CheckInPatient([FromBody]CheckInPatientDataRequest checkInPatientDataRequest)
        {
            IStateContainer session = new StateContainer(HttpContext.Current.Session);

            return checkInPatient(checkInPatientDataRequest, session);
        }

        private ApiResponse checkInPatient(CheckInPatientDataRequest checkInPatientDataRequest, IStateContainer session)
        {
            using (var timer = logger.StartTimer("CheckInPatient"))
            {
                var dbid = session.Cast(Constants.SessionVariables.DbId, ConnectionStringPointer.ERXDB_DEFAULT);
                var userId = session.GetStringOrEmpty(Constants.SessionVariables.UserId);
                var licenseId = session.GetStringOrEmpty(Constants.SessionVariables.LicenseId);
                var patientId = session.GetStringOrEmpty(Constants.SessionVariables.PatientId);
                var siteId = session.GetInt(Constants.SessionVariables.SiteId, 1);

                var checkInPatientDataResponse = new CheckInPatientDataResponse();
                var response = new ApiResponse();

                try
                {
                    Patient.AddPatientToSchedule(licenseId, checkInPatientDataRequest.ProviderId, patientId, DateTime.Now, "Arrived", string.Empty, string.Empty, userId, dbid);

                    sendPatientEligibilityRequest(patientId, licenseId, siteId, userId, dbid);

                    checkInPatientDataResponse.CheckedInMessage = session.GetStringOrEmpty(Constants.SessionVariables.PatientName) + " has been checked in.";
                    checkInPatientDataResponse.CheckedInMessageIcon = "Information";
                    checkInPatientDataResponse.CheckedInMessageVisibility = true;

                    this.updateSessionAfterCheckin(checkInPatientDataResponse.CheckedInMessage, patientId);

                    response.Payload = checkInPatientDataResponse;
                }
                catch (Exception ex)
                {
                    var errorMessage = ApiHelper.AuditException(ex.ToString(), session);
                    logger.Error("checkInPatient Exception: " + ex.ToString());
                    response.ErrorContext = new ErrorContextModel()
                    {
                        Error = ErrorTypeEnum.ServerError,
                        Message = errorMessage
                    };
                }

                timer.Message = string.Format("<Response>{0}</Response>", response.ToLogString());

                return response;
            }
        }

        private static void sendPatientEligibilityRequest(string patientId, string licenseId, int siteId, string userId, ConnectionStringPointer dbid)
        {
            string lastProviderID = null;

            Patient p = new Patient(patientId, licenseId, userId, dbid);

            if (p.DS.Tables["PatientPrescription"].Rows.Count > 0)
            {
                DataRow rxRow = p.DS.Tables["PatientPrescription"].Rows[0];
                lastProviderID = rxRow["ProviderID"].ToString();
            }
            else
            {
                //no Hx for this patient...let's grab any doc in this site
                DataSet dsDoc = Allscripts.Impact.Provider.GetProviders(licenseId, siteId, dbid);
                if (dsDoc.Tables[0].Rows.Count > 0)
                {
                    lastProviderID = dsDoc.Tables[0].Rows[0]["ProviderID"].ToString();
                }
            }

            if (lastProviderID != null)
            {
                Patient.AddPatientEligibilityRequest(licenseId, lastProviderID, patientId, null, userId, dbid);
            }
        }

        private void updateSessionAfterCheckin(string message, string patientId)
        {
            HttpContext.Current.Session["CHKDIN_NAME"] = message;

            //To store all the patients, they are checked in.
            if (HttpContext.Current.Session["PATIENTCODE"] != null)
                HttpContext.Current.Session["PATIENTCODE"] = HttpContext.Current.Session["PATIENTCODE"] + "," + patientId;
            else
                HttpContext.Current.Session["PATIENTCODE"] = patientId;
        }
        public static void ResetWorkflows(IStateContainer session)
        {
            MasterPage(session).RxTask = null;
            session[Constants.SessionVariables.TaskScriptMessageId] = null;
            session.Remove(Constants.SessionVariables.DURRefillDS);
            session.Remove("CURRENT_DUR_WARNINGS");
            if (session[Constants.SessionVariables.IsCsRefReqWorkflow] != null || 
                session[Constants.SessionVariables.ChangeRxRequestedMedCs] != null)
            {
                //reset workflows and remove any script pad items
                ScriptPadUtil.RemoveAllRxFromScriptPad(session);
                session.Remove(Constants.SessionVariables.IsCsRefReqWorkflow);
                session.Remove(Constants.SessionVariables.ChangeRxRequestedMedCs);
            }
            session.Remove(Constants.SessionVariables.TaskType);
        }

        [HttpPost]
        public ApiResponse GetStartupParameters(GetStartupParametersRequest getStartupParametersRequest)
        {
            IStateContainer session = new StateContainer(HttpContext.Current.Session);
            var ip = HttpContext.Current.Request.UserIpAddress();
            var response = new ApiResponse();
            try
            {

                using (var timer = logger.StartTimer("AuditAccessAndGetStartupParameters"))
                {
                    response.Payload = getStartupParameters(session, getStartupParametersRequest, ip);


                    timer.Message = string.Format("<Request>{0}</Request><Response>{1}</Response>", ip.ToString(), response.ToLogString());
                }
            }
            catch (Exception ex)
            {
                var errorMessage = Audit.AddApiException(ApiHelper.GetSessionUserID(session), ApiHelper.GetSessionLicenseID(session), ex.ToString(), ApiHelper.GetDBID(session));
                logger.Error(ex.ToString());
                response.ErrorContext = new ErrorContextModel()
                {
                    Error = ErrorTypeEnum.ServerError,
                    Message = errorMessage
                };
            }

            return response;
        }

        internal static SelectPatientStartupParameters GetStartupParameters(IStateContainer session)
        {
            return getStartupParameters(session, new GetStartupParametersRequest(), HttpContext.Current.Request.UserIpAddress());
        }
        internal static SelectPatientStartupParameters getStartupParameters(IStateContainer session, GetStartupParametersRequest getStartupParametersRequest, string ip)
        {
                var dbid = session.Cast(Constants.SessionVariables.DbId, ConnectionStringPointer.ERXDB_DEFAULT);
                var userId = session.GetStringOrEmpty(Constants.SessionVariables.UserId);
                var licenseId = session.GetStringOrEmpty(Constants.SessionVariables.LicenseId);
                var patientId = session.GetStringOrEmpty(Constants.SessionVariables.PatientId);
                var siteId = session.GetInt(Constants.SessionVariables.SiteId, 1);

                var startupParameters = new SelectPatientStartupParameters();
                
                    ResetWorkflows(session);
                    if(!String.IsNullOrWhiteSpace(patientId))
                    {   //Do not audit if patient is not present
                        EPSBroker.AuditLogPatientInsert(ePrescribeSvc.AuditAction.PATIENT_HEADER_VIEW, licenseId, userId, patientId, ip, dbid);
                    }


                    bool isRestrictedUserOverridden = session.GetBooleanOrFalse("RestrictedUserOverridden");

                    if (!string.IsNullOrWhiteSpace(patientId))
                    {
                        bool isUserRestricted = checkIfUserRestricted(session, patientId);
                        session["IsRestrictedUser"] = isUserRestricted ? true : false;

                        if (isUserRestricted)
                        {
                            if (isRestrictedUserOverridden)
                            {
                                setPatientInfo(patientId, session);

                                // the question is, when will this get set again? New rx click, select dx click, review history?
                                session.Remove("RestrictedUserOverridden");
                            }
                            else
                            {
                                ClearPatientInfo(session);
                                patientId = null;
                                startupParameters.IsPrivacyOverrideDisplayed = true;
                            }
                        }
                        else
                        {
                            setPatientInfo(patientId, session);
                        }
                    }

                    startupParameters.PatientID = patientId;

                    startupParameters.IsShowAddPatient = session.GetBooleanOrFalse("AddPatient");
                    var sessionLicense = ApiHelper.SessionLicense(session);
                    startupParameters.IsAddDiagnosisVisible = sessionLicense.EnterpriseClient?.AddDiagnosis == true &&
                                                              session.GetBooleanOrFalse("AddDiagnosis");
                    startupParameters.UserID = userId;
                    startupParameters.UserType = GetUserType(session);
                    startupParameters.UserCategory = GetUserCategory(session);

                    if (startupParameters.UserType == SelectPatientUserType.PAwithSupervision)
                    {
                        startupParameters.Providers = LoadProviders(session, licenseId, siteId, userId, dbid);
                        startupParameters.DelegateProviderId = Allscripts.Impact.RxUser.GetPOBProviderID(licenseId, siteId, userId, dbid);
                    }

                    string ssoMode = session.GetStringOrEmpty("SSOMode");

                    if (ssoMode == Constants.SSOMode.PATIENTLOCKDOWNMODE)
                    {
                        startupParameters.IsSsoLockdownMode = true;

                        int tasks = Patient.GetTaskCountForPatient(licenseId, patientId, dbid);
                        if (tasks > 0)
                        {
                            startupParameters.Message = new MessageModel("Task Alert!  Patient has pending tasks to be reviewed.", MessageIcon.Information);
                            startupParameters.Message.Tag = "taskAlert";
                        }
                    }

                    if (string.IsNullOrWhiteSpace(session.GetStringOrEmpty("DisableIntroductoryPopup")))
                    {
                        session["DisableIntroductoryPopup"] = true; //Show only once per login

                        if (sessionLicense.EnterpriseClient.ShowWelcomeTour)
                        {
                            GetMessageTrackingAcksResponse acksResponse =
                                EPSBroker.GetMessageTrackingAcks(new Guid(session["USERID"].ToString()));

                            string tourPath;
                            int tourType;

                            if (IsAnyTourToBeShown(acksResponse, Allscripts.Impact.ConfigKeys.WelcomeTourNewUserPath,
                                Allscripts.Impact.ConfigKeys.WelcomeTourNewReleasePath,
                                Allscripts.Impact.ConfigKeys.ShowWelcomeTourNewUser,
                                Allscripts.Impact.ConfigKeys.ShowWelcomeTourNewRelease, out tourPath, out tourType))
                            {
                                WelcomeTourModel welcomeTourModel = new WelcomeTourModel();
                                welcomeTourModel.SourceUrl = tourPath;
                                welcomeTourModel.TourType = tourType;                                
                                startupParameters.IsWelcomeTourDisplayed = true;
                                startupParameters.WelcomeTourModel = welcomeTourModel;
                            }
                            else
                            {
                                startupParameters.IsWelcomeTourDisplayed = false;
                            }
                        }
                        else
                        {
                            startupParameters.IsWelcomeTourDisplayed = false;
                        }

                        if (!startupParameters.IsWelcomeTourDisplayed && HttpContext.Current.Request.Cookies[Constants.Cookies.TeaserAdShown] == null)
                        {
                            DeluxeTeaserAdModel deluxeTeaserAdModel = new DeluxeTeaserAdModel();

                            if (AppCode.CompulsoryBasicUtil.DisplayPromoTeaserAd(sessionLicense.IsEnterpriseCompulsoryBasic(),
                                sessionLicense.IsPricingStructureBasic(), Allscripts.Impact.ConfigKeys.CompulsoryBasicStartDate)) //Display for both Admin and Non Admins
                            {
                                //Compulsory Basic Promo Ad
                                deluxeTeaserAdModel.TeaserAdContent = null;
                                deluxeTeaserAdModel.IsCompulsaryBasic = true;
                                deluxeTeaserAdModel.Cookie = CreateTeaserAdCookieString(24);
                                startupParameters.IsDeluxeTeaserAdDisplayed = true;
                            }
                            else 
                            {
                                List<ePrescribeSvc.TeaserAdResponse> teaserAdContent = BuildTeaserAd(session, dbid);
                                deluxeTeaserAdModel.TeaserAdContent = teaserAdContent;

                                if (teaserAdContent.Count > 0)
                                {
                                    startupParameters.IsDeluxeTeaserAdDisplayed = true;
                                    deluxeTeaserAdModel.Cookie = CreateTeaserAdCookieString(24);
                                }
                                else
                                {
                                    startupParameters.IsDeluxeTeaserAdDisplayed = false;
                                    deluxeTeaserAdModel.Cookie = CreateTeaserAdCookieString(3); //Dont check for three more hours
                                }
                            }

                            startupParameters.DeluxeTeaserAdModel = deluxeTeaserAdModel;
                        }

                        if (!startupParameters.IsWelcomeTourDisplayed &&
                            !startupParameters.IsDeluxeTeaserAdDisplayed)
                        {
                            if (CreditCard.ShouldShowExpiringWarning(
                                session.GetBooleanOrFalse(Constants.SessionVariables.DidCheckForCreditCardExpired),
                                licenseId.ToGuidOr0x0(), new Allscripts.ePrescribe.Data.CreditCard(),
                                dbid))
                            {
                                startupParameters.IsCreditCardExpiring = true;
                                session[Constants.SessionVariables.DidCheckForCreditCardExpired] = true;
                            }
                        }
                    }

                    startupParameters.SearchPatientResponse = loadPatients(licenseId, patientId, ssoMode, isRestrictedUserOverridden, getStartupParametersRequest.PatientDemographics, session);

                return startupParameters;
            
        }

       
        private static SearchPatientResponse loadPatients(string licenseId, string patientId, string ssoMode, bool isRestrictedUserOverridden, PatientDemographics patientDemographics, IStateContainer session)
        {
            if (ssoMode != null && ssoMode == Constants.SSOMode.PATIENTLOCKDOWNMODE && patientId != null)
            {
                //only show specific patient in list. do not show all checked in patients.
                SearchPatientsRequest searchPatientsRequest = new SearchPatientsRequest();
                searchPatientsRequest.PatientGuid = patientId;
                return getSearchPatientResults(searchPatientsRequest, session);
            }
            else if (!Convert.ToBoolean(session.GetBooleanOrFalse("SHOW_CHECKED_IN_PTS")))
            {
                //if the license pref is set to not show checked in patients, first check if there's a patientID in session.
                //if there isn't a patientID in session, initialize the datasource so no patients are shown
                if (patientId != null)
                {
                    SearchPatientsRequest searchPatientsRequest = new SearchPatientsRequest();
                    searchPatientsRequest.PatientGuid = patientId;
                    return getSearchPatientResults(searchPatientsRequest, session);
                }
                else
                {
                    return null;
                }
            }
            else if (isRestrictedUserOverridden)
            {
                SearchPatientsRequest searchPatientsRequest = new SearchPatientsRequest();
                searchPatientsRequest.PatientGuid = patientId;
                return getSearchPatientResults(searchPatientsRequest, session);
            }
            else if (patientDemographics!=null)
            {
                return GetSimilarPatients(patientDemographics, session) ;
            }
            else
            {
                return getScheduledPatients(session);
            }
        }

        private static SearchPatientResponse GetSimilarPatients(PatientDemographics patientDemographics, IStateContainer session)
        {
            using (var timer = logger.StartTimer("GetSimilarPatients"))
            {
                SearchPatientResponse searchPatientResponse = new SearchPatientResponse();

                try
                {
                    string licenseID = session.GetStringOrEmpty(Constants.SessionVariables.LicenseId);
                    string lastName = patientDemographics.LastName.Trim();
                    string firstName = patientDemographics.FirstName.Trim();
                    string address = patientDemographics.Address1.Trim();
                    string zip = patientDemographics.Zip.Trim();
                    DateTime dob = Convert.ToDateTime(patientDemographics.Dob.Trim());

                    int matchThrashold = 100; //Highest score is 240 
                    DataSet dsPatients = Allscripts.Impact.CHPatient.SearchSimilarPatients(licenseID, firstName, lastName, address, dob, zip, matchThrashold, session.GetStringOrEmpty(Constants.SessionVariables.UserId), false, session.Cast(Constants.SessionVariables.DbId, ConnectionStringPointer.ERXDB_DEFAULT));

                    if (dsPatients != null && dsPatients.Tables.Count > 0 && dsPatients.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow drPatient in dsPatients.Tables[0].Rows)
                        {
                            var patientItem = new PatientItemModel(drPatient, true);
                            searchPatientResponse.Patients.Add(patientItem);
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.Error("GetSimilarPatients Exception: " + ex);
                }

                return searchPatientResponse;
            }
        }
        [HttpPost]
        public ApiResponse LoadProvidersForSupervisedPA(LoadProvidersForSupervisedPARequest request)
        {
            using (var timer = logger.StartTimer("LoadProvidersForSupervisedPA"))
            {
                var response = new ApiResponse();

                try
                {
                    response.Payload = loadProvidersForSupervisedPA(request.ProviderId);
                }
                catch (Exception ex)
                {
                    var errorMessage = ApiHelper.AuditException(ex.ToString(), _session);
                    logger.Error("LoadProvidersForSupervisedPA Exception: " + ex.ToString());

                    response.ErrorContext = new ErrorContextModel()
                    {
                        Error = ErrorTypeEnum.UserMessage,
                        Message = errorMessage
                    };
                }

                timer.Message = string.Format("<Response>{0}</Response>", response.ToLogString());
                return response;
            }
        }

        protected List<SelectPatientProvider> loadProvidersForSupervisedPA(string providerId)
        {
            DataSet dsListProvider = new DataSet();
            List<SelectPatientProvider> providers = new List<SelectPatientProvider>();

            var dbid = _session.Cast(Constants.SessionVariables.DbId, ConnectionStringPointer.ERXDB_DEFAULT);
            var licenseId = _session.GetStringOrEmpty(Constants.SessionVariables.LicenseId);
            var siteId = _session.GetInt(Constants.SessionVariables.SiteId, 1);

            dsListProvider = Allscripts.Impact.Provider.GetProviders(licenseId, siteId, dbid);

            foreach (DataRow drProvider in dsListProvider.Tables[0].Rows)
            {
                if (!Convert.ToString(drProvider["Active"]).GetBooleanFromYOrN())// skip not active
                {
                    continue;
                }

                var userType = Convert.ToString(drProvider["UserType"]);
                if (userType != "1")
                {
                    continue; // skip all but providers for POB with Sup.
                }

                providers.Add(new SelectPatientProvider()
                {
                    ProviderId = Convert.ToString(drProvider["ProviderID"]),
                    ProviderName = Convert.ToString(drProvider["ProviderName"])
                    //, UserCategory = (SelectPatientUserCategory)userType;
                });
            }

            return providers;
        }

        [HttpPost]
        public ApiResponse SetSupervisingProviderInfo(SupervisorProviderInfoRequest supervisorProviderInfoRequest)
        {            
            using (var timer = logger.StartTimer("SetSupervisingProviderInfo"))
            {
                var response = new ApiResponse();

                try
                {
                    response.Payload = SetSupervisingProviderInformation(supervisorProviderInfoRequest);
                }
                catch (Exception ex)
                {
                    var errorMessage = ApiHelper.AuditException(ex.ToString(), _session);
                    logger.Error("SetSupervisingProviderInfo Exception: " + ex.ToString());

                    response.ErrorContext = new ErrorContextModel()
                    {
                        Error = ErrorTypeEnum.UserMessage,
                        Message = errorMessage
                    };
                }

                timer.Message += string.Format("<Response>{0}</Response>", response.ToLogString());
                return response;
            }
        }

        private bool IsValidSupervisor(string providerId, ConnectionStringPointer dbId)
        {
            var supervisorsList = loadProvidersForSupervisedPA(providerId);
            return supervisorsList.FindIndex(x => x.ProviderId == providerId) >= 0 ? true : false;
        }

        private SupervisorProviderInfoResponse SetSupervisingProviderInformation(SupervisorProviderInfoRequest supervisorProviderInfoRequest)
        {
            SupervisorProviderInfoResponse supervisorProviderInfoResponse = new SupervisorProviderInfoResponse
            {
                IsSupervisorProviderInfoSet = true,
                Message = string.Empty,
                MessageIcon = MessageIcon.Information
            };

            var dbid = _session.Cast(Constants.SessionVariables.DbId, ConnectionStringPointer.ERXDB_DEFAULT);

            if (!IsValidSupervisor(supervisorProviderInfoRequest.SupervisorProviderId, dbid)) 
            {
                throw new Exception($"Provider with ID:{supervisorProviderInfoRequest.SupervisorProviderId} forbidden to be set as supervising provider."); 
            }            
            
            Allscripts.Impact.RxUser superVisiorProvider = new Allscripts.Impact.RxUser(supervisorProviderInfoRequest.SupervisorProviderId, dbid);

            if (superVisiorProvider.HasNPI)
            {
                setSupervisingProvider(superVisiorProvider.UserID);

                List<string> deaSchedule = new List<string>();
                for (int i = 1; i < 6; i++)
                {
                    if (superVisiorProvider.DEAScheduleAllowed(i))
                    {
                        deaSchedule.Add(i.ToString());
                    }
                }

                _session["PASUPERVISOR_DEASCHEDULESALLOWED"] = deaSchedule;

                deaSchedule.Sort();
                int minsched = 0;

                if (deaSchedule.Count > 0)
                {
                    minsched = Convert.ToInt32(deaSchedule[0]);
                }

                _session["PASUPERVISIOR_MINSCHEDULEALLOWED"] = minsched.ToString();

                supervisorProviderInfoResponse.IsSupervisorProviderInfoSet = true;
            }
            else
            {
                supervisorProviderInfoResponse.Message = "Please select a provider with a valid NPI.";
                supervisorProviderInfoResponse.MessageIcon = MessageIcon.Error;
                supervisorProviderInfoResponse.IsSupervisorProviderInfoSet = false;
            }

            return supervisorProviderInfoResponse;
        }

        private void setSupervisingProvider(string providerId)
        {
            _session["SUPERVISING_PROVIDER_ID"] = providerId;
        }
        
        [HttpPost]
        public ApiResponse SetProviderInformation(SetProviderInformationRequest request)
        {
            using (var timer = logger.StartTimer("SetProviderInformation"))
            {
                var response = new ApiResponse();

                try
                {                    
                    response.Payload = setProviderInformation(request?.ProviderId);
                }
                catch (Exception ex)
                {
                    var errorMessage = ApiHelper.AuditException(ex.ToString(), _session);
                    logger.Error("SetProviderInformation Exception: " + ex.ToString());

                    response.ErrorContext = new ErrorContextModel()
                    {
                        Error = ErrorTypeEnum.UserMessage,
                        Message = errorMessage
                    };
                }

                timer.Message += string.Format("<Response>{0}</Response>", response.ToLogString());
                return response;
            }
        }
                
        private bool setProviderInformation(string providerId)
        {
            var dbid = _session.Cast(Constants.SessionVariables.DbId, ConnectionStringPointer.ERXDB_DEFAULT);
            
            Allscripts.Impact.RxUser selectedProvider = new Allscripts.Impact.RxUser(providerId, dbid);

            if (selectedProvider.HasNPI)
            {
                List<string> deaSchedule = new List<string>();
                for (int i = 1; i < 6; i++)
                {
                    if (selectedProvider.DEAScheduleAllowed(i))
                    {
                        deaSchedule.Add(i.ToString());
                    }
                }

                _session["DEASCHEDULESALLOWED_SUPERVISOR"] = deaSchedule;

                deaSchedule.Sort();
                int minsched = 0;

                if (deaSchedule.Count > 0)
                {
                    minsched = Convert.ToInt32(deaSchedule[0]);
                }

                _session["MINSCHEDULEALLOWED_SUPERVISOR"] = minsched.ToString();

                SetDelegateProvider(selectedProvider.UserID, _session);

                if (!selectedProvider.IsPhysicianAssistantSupervised)
                {
                    _session.Remove("SUPERVISING_PROVIDER_ID");
                }

                // save the value in session so it's not forcing loading full object of DelegateProvider every time
                _session[Constants.SessionVariables.DelegateProviderNPI] = selectedProvider.NPI;
                _session[Constants.SessionVariables.ShouldPrintOfferAutomatically] = selectedProvider.ShouldPrintOfferAutomatically;
                _session[Constants.SessionVariables.ShouldShowTherapeuticAlternativeAutomatically] = selectedProvider.ShouldShowTherapeuticAlternativeAutomatically;
                _session[Constants.SessionVariables.ShouldShowPpt] = selectedProvider.ShouldShowPpt;
                _session[Constants.SessionVariables.ShouldShowRtbi] = selectedProvider.ShouldShowRtbi;

                var provInfo = Allscripts.ePrescribe.Objects.CommonComponent.ProviderInfo.Create(selectedProvider.UserID.ToGuidOr0x0(), selectedProvider.FirstName, selectedProvider.LastName, selectedProvider.DEA, selectedProvider.NPI, selectedProvider.Email);
                _session[Constants.SessionVariables.CommonCompProviderInfo] = provInfo;

                return true;
            }
            else
            {
                return false;
            }
        }

        private void setSPI(string providerID, IStateContainer session)
        {
            var dbid = session.Cast(Constants.SessionVariables.DbId, ConnectionStringPointer.ERXDB_DEFAULT);

            DataSet dsSPI = Allscripts.Impact.Provider.GetSPI(providerID, dbid);

            if (dsSPI.Tables.Count > 0)
            {
                DataRow[] drSPI = dsSPI.Tables[0].Select("ScriptSwId='SURESCRIPTS'");

                //should only be one row for SURESCRIPTS...grab the first and only
                if (drSPI.Length > 0 && drSPI[0] != null && drSPI[0]["SenderId"] != DBNull.Value && drSPI[0]["SenderId"].ToString() != "")
                {
                    session["SPI"] = drSPI[0]["SenderID"].ToString();
                }
                else
                {
                    session["SPI"] = null;
                }
            }
            else
            {
                session["SPI"] = null;
            }
        }

        private void SetDelegateProvider(string providerID, IStateContainer session)
        {
            var userId = session.GetStringOrEmpty(Constants.SessionVariables.UserId);
            var licenseId = session.GetStringOrEmpty(Constants.SessionVariables.LicenseId);
            var dbid = session.Cast(Constants.SessionVariables.DbId, ConnectionStringPointer.ERXDB_DEFAULT);
            var siteId = session.GetInt(Constants.SessionVariables.SiteId, 1);

            Allscripts.Impact.RxUser.UpdatePOBProviderUsage(licenseId, siteId, userId, providerID, dbid);

            session["DelegateProviderID"] = providerID;

            if (session["IsPASupervised"] != null && Convert.ToBoolean(session["IsPASupervised"]))
            {
                //the PA is the one whose SPI needs to be set
                setSPI(session["USERID"].ToString(), session);
            }
            else
            {
                setSPI(providerID, session);
            }
        }

        private static SearchPatientResponse getScheduledPatients(IStateContainer session)
        {
            using (var timer = logger.StartTimer("getScheduledPatients"))
            {
                SearchPatientResponse searchPatientResponse = new SearchPatientResponse();

                try
                {
                    var licenseId = session.GetStringOrEmpty(Constants.SessionVariables.LicenseId);
                    var providerId = "{00000000-0000-0000-0000-000000000000}";
                    var startDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, 0, 0, 0);
                    var endDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, 23, 59, 0);
                    var dbid = session.Cast(Constants.SessionVariables.DbId, ConnectionStringPointer.ERXDB_DEFAULT);
                    var userId = session.GetStringOrEmpty(Constants.SessionVariables.UserId);
                    var userType = (Constants.UserCategory)Enum.Parse(typeof(Constants.UserCategory), session.GetStringOrEmpty("UserType"));

                    DataSet dsPatients = Allscripts.Impact.CHPatient.GetScheduledList(
                        licenseId,
                        providerId,
                        startDate,
                        endDate,
                        userId,
                        dbid,
                        userType);

                    if (dsPatients != null && dsPatients.Tables.Count > 0 && dsPatients.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow drPatient in dsPatients.Tables[0].Rows)
                        {
                            var patientItem = new PatientItemModel(drPatient);
                            searchPatientResponse.Patients.Add(patientItem);
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.Error("getScheduledPatients Exception: " + ex.ToString());
                }

                return searchPatientResponse;
            }
        }

        [HttpPost]
        public ApiResponse SearchPatients([FromBody]SearchPatientsRequest searchPatientsRequest)
        {
            ApiResponse response = new ApiResponse();
            response.Payload = getSearchPatientResults(searchPatientsRequest, new StateContainer(HttpContext.Current.Session));
            return response;
        }

        private static SearchPatientResponse getSearchPatientResults(SearchPatientsRequest searchPatientsRequest, IStateContainer session)
        {
            using (var timer = logger.StartTimer("GetSearchPatientResults"))
            {
                SearchPatientResponse searchPatientResponse = new SearchPatientResponse();

                try
                {
                    var dbid = session.Cast(Constants.SessionVariables.DbId, ConnectionStringPointer.ERXDB_DEFAULT);
                    var userId = session.GetStringOrEmpty(Constants.SessionVariables.UserId);
                    var licenseId = session.GetStringOrEmpty(Constants.SessionVariables.LicenseId);
                    var hasVIPPatients = ApiHelper.SessionLicense(session).hasVIPPatients;

                    DataSet dsPatients = Allscripts.Impact.CHPatient.SearchPatient(
                        licenseId,
                        searchPatientsRequest.LastName,
                        searchPatientsRequest.FirstName,
                        searchPatientsRequest.DateOfBirth,
                        searchPatientsRequest.ChartId,
                        searchPatientsRequest.WildCard,
                        userId,
                        hasVIPPatients,
                        searchPatientsRequest.UserType,
                        searchPatientsRequest.PatientGuid,
                        searchPatientsRequest.IncludeInactive,
                        dbid);

                    if (dsPatients != null && dsPatients.Tables.Count > 0 && dsPatients.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow drPatient in dsPatients.Tables[0].Rows)
                        {
                            var patientItem = new PatientItemModel(drPatient);
                            searchPatientResponse.Patients.Add(patientItem);
                        }
                    }
                    else
                    {
                        searchPatientResponse.Messages.Add(new MessageModel() { Message = $"No patients found matching search criteria.", Icon = MessageIcon.Information });
                    }
                }
                catch (Exception ex)
                {
                    //var errorMessage = ApiHelper.AuditException(ex.ToString(), _session);
                    logger.Error("GetSearchPatientResults Exception: " + ex.ToString());

                    //response.ErrorContext = new ErrorContextModel()
                    //{
                    //    Error = ErrorTypeEnum.UserMessage,
                    //    Message = errorMessage
                    //};
                }

                timer.Message = string.Format("<Response>{0}</Response>", searchPatientResponse.ToLogString());
                return searchPatientResponse;
            }
        }

        [HttpPost]
        public ApiResponse SetPatientInfo([FromBody]string patientId)
        {
            IStateContainer session = new StateContainer(HttpContext.Current.Session);
            var ip = HttpContext.Current.Request.UserIpAddress();
            return SetPatientInfo(session, patientId, ip);
        }

        private static bool IsAvailableSession(string key)
        {
            return HttpContext.Current.Session[key] != null;
        }

        private static void GetActiveProblemList(IStateContainer PageState, PatientHeader pt)
        {
            Allscripts.ePrescribe.Data.IPatientDiagnosisProvider diagnosis = new Allscripts.ePrescribe.Data.PatientDiagnosisProvider();
            var data = diagnosis.GetActiveDiagnosis(PageState.GetStringOrEmpty("PATIENTID"), PageState.Cast<ConnectionStringPointer>("DBID", ConnectionStringPointer.AUDIT_ERXDB_SERVER_1));

            foreach (var v in data)
            {
                pt.ActiveDignosises.Add(new ActiveDignosis()
                {
                    Diagnosis = v.Description,
                    StartDate = v.StartDate
                });
            }
        }

        private static void GetActiveAllergy(IStateContainer PageState, PatientHeader pt)
        {
            var data = Patient.GetPatientAllergy(PageState.GetStringOrEmpty("PATIENTID"), PageState.GetStringOrEmpty("LICENSEID"), PageState.GetStringOrEmpty("USERID"), PageState.Cast<ConnectionStringPointer>("DBID", ConnectionStringPointer.AUDIT_ERXDB_SERVER_1));

            foreach (DataRow v in data.Tables[0].Rows)
            {
                pt.ActiveAllergies.Add(new ActiveAllergy()
                {
                    Name = v["AllergyName"].ToString(),
                    StartDate = v["StartDate"].ToString()
                });
            }
        }

        private static void GetActiveMeds(IStateContainer PageState, PatientHeader pt)
        {
            var data = Patient.GetPatientActiveMedication(PageState.GetStringOrEmpty("PATIENTID"), PageState.GetStringOrEmpty("LICENSEID"), PageState.GetStringOrEmpty("USERID"), PageState.Cast<ConnectionStringPointer>("DBID", ConnectionStringPointer.AUDIT_ERXDB_SERVER_1));

            foreach (DataRow v in data.Tables[0].Rows)
            {
                pt.ActiveMeds.Add(new ActiveMeds()
                {
                    Name = v["Medication"].ToString(),
                    StartDate = v["StartDate"].ToString()
                });
            }
        }

        public static bool ShowPharmacy(ApplicationLicense SessionLicense, IStateContainer PageState)
        {

            return true;// (SessionLicense.EnterpriseClient.ShowPharmacy && !PageState.ContainsKey(Constants.SessionVariables.RefReq));

        }

        public static bool IsPharmacyEPCSEnabled(IStateContainer PageState)
        {

            bool isPharmacyEPCSEnabled = false;

            if (IsAvailableSession("ISPHARMACYEPCSENABLED"))
            {
                isPharmacyEPCSEnabled = (bool)PageState.GetBooleanOrNull("ISPHARMACYEPCSENABLED");
            }

            return isPharmacyEPCSEnabled;

        }

        public static bool IsMOPharmacyEPCSEnabled(IStateContainer PageState)
        {
            bool isMOEPCSEnabled = false;

            if (IsAvailableSession("MOB_ISEPCSENABLED"))
            {
                isMOEPCSEnabled = (bool)PageState.GetBooleanOrNull("MOB_ISEPCSENABLED");
            }

            return isMOEPCSEnabled;
        }

        private static void SetPatientActiveMedsControl(IStateContainer PageState, PatientHeader pHdr)
        {
            if (PageState[Constants.SessionVariables.PatientId] == null) return;

            var activeMedsSession = PageState.GetStringOrEmpty("ACTIVEMEDICATIONS");

            pHdr.MoreActiveMedVisible = false;
            if (PageState.GetBooleanOrFalse("PATIENTNoActiveMed"))
            {
                pHdr.ActiveMed = "No Active Medications";
            }
            else if (activeMedsSession.Length > 0)
            {
                if (activeMedsSession.Length > 100)
                {
                    pHdr.ActiveMed = activeMedsSession.Substring(0, 100);
                    pHdr.MoreActiveMedVisible = true;
                    GetActiveMeds(PageState, pHdr);
                }
                else
                {
                    pHdr.ActiveMed = activeMedsSession;
                }
            }
            else
            {
                pHdr.ActiveMed = "None entered";
            }
        }

        private static void displayMailOrder(bool show, IStateContainer PageState, ApplicationLicense sessionLicense, PatientHeader pHdr)
        {
            if (ShowPharmacy(sessionLicense, PageState))
            {
                if (!IsAvailableSession("MOB_NABP") || PageState.GetStringOrEmpty("MOB_NABP").Trim() == "")
                {
                    show = false;
                }

                if (show)
                {
                    if (IsAvailableSession("MOB_Name"))
                    {
                        if (PageState.GetStringOrEmpty("MOB_Name").Length > 48)
                        {
                            pHdr.PrefMOP = PageState.GetStringOrEmpty("MOB_Name").Substring(0, 48);
                            pHdr.MoreMailOrderPharmVisible = true;
                        }
                        else
                        {
                            pHdr.PrefMOP = PageState.GetStringOrEmpty("MOB_Name");
                        }
                    }
                    else
                    {
                        pHdr.PrefMOP = "None entered";
                    }

                    // new EPCS image logic
                    if (IsMOPharmacyEPCSEnabled(PageState))
                    {
                        pHdr.IsMoEpcs = true;
                    }

                    // end mail order pharmacy image logic
                }
                else
                {
                    if (IsAvailableSession("PatientID"))
                    {
                        pHdr.PrefMOP = "None entered";
                        pHdr.MoreMailOrderPharmVisible = false;
                        pHdr.IsMoEpcs = false;
                    }
                }
            }
        }

        private static void SetEditOption(IStateContainer Pagestate, PatientHeader pt)
        {
            pt.AllowPatientEdit = Pagestate.GetBooleanOrFalse("EditPatient");
            pt.AllowAllergyEdit = Pagestate.GetBooleanOrFalse("EditAllergy");
            pt.AllowDiagnosisEdit = Pagestate.GetBooleanOrFalse("EditDiagnosis");
            pt.AllowPharmacyEdit = Pagestate.GetBooleanOrFalse("EditPharmacy");
        }

        public static bool AllowPharmacyEdit(IStateContainer PageState)
        {
            return PageState.GetBooleanOrFalse("EditPharmacy");
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

            if (IsAvailableSession("PATIENTID") && IsAvailableSession("PATIENTFIRSTNAME") && IsAvailableSession("PATIENTLASTNAME") && IsAvailableSession("PATIENTDOB"))
            {
                pHdr.PatientID = PageState.GetStringOrEmpty(Constants.SessionVariables.PatientId);
                pHdr.FirstName = PageState.GetStringOrEmpty(Constants.SessionVariables.PatientFirstName) + " " + PageState.GetStringOrEmpty(Constants.SessionVariables.PatientMiddleName);
                pHdr.LastName = PageState.GetStringOrEmpty(Constants.SessionVariables.PatientLastName).ToUpper() + ", ";

                pHdr.IsRestrictedUser = PageState.GetBooleanOrFalse("IsRestrictedUser");
                pHdr.IsRestrictedPatient = PageState.GetBooleanOrFalse("IsRestrictedPatient");
                pHdr.IsVipPatient = PageState.GetBooleanOrFalse("IsVIPPatient");
                patientId = PageState.GetStringOrEmpty("PATIENTID");
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

                var weight = PageState.Cast<Allscripts.ePrescribe.Objects.Weight>(Constants.SessionVariables.PatientWeight, null);
                var height = PageState.Cast<Height>(Constants.SessionVariables.PatientHeight, null);

                pHdr.WeightLabel = Allscripts.ePrescribe.Objects.Weight.ConvertToLabel(weight);
                pHdr.HeightLabel = Height.ConvertToLabel(height);

                //imgMoreActiveProblem.Visible = false;
                if (IsAvailableSession("ACTIVEDX") && PageState.GetStringOrEmpty("ACTIVEDX").Length > 0)
                {
                    if (PageState.GetStringOrEmpty("ACTIVEDX").Length > 100)
                    {
                        pHdr.Dx = PageState.GetStringOrEmpty("ACTIVEDX").Substring(0, 100);
                        pHdr.MoreActiveProblem = true;
                        GetActiveProblemList(PageState, pHdr);
                    }
                    else
                    {
                        pHdr.Dx = PageState.GetStringOrEmpty("ACTIVEDX");
                    }
                }
                else
                {
                    pHdr.Dx = "None entered";
                }

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

                if (IsAvailableSession("PATIENTNKA") && PageState.GetStringOrEmpty("PATIENTNKA").Equals("Y"))
                {
                    pHdr.Allergy = "No Known Allergies";
                }
                else if (IsAvailableSession("ALLERGY") && PageState.GetStringOrEmpty("ALLERGY").Length > 0)
                {
                    if (PageState.GetStringOrEmpty("ALLERGY").Length > 100)
                    {
                        pHdr.Allergy = PageState.GetStringOrEmpty("ALLERGY").Substring(0, 100);
                        pHdr.MoreActiveAllergy = true;
                        GetActiveAllergy(PageState, pHdr);
                    }
                    else
                    {
                        pHdr.Allergy = PageState.GetStringOrEmpty("ALLERGY");
                    }

                }
                else
                {
                    pHdr.Allergy = "None entered";
                }

                if (ShowPharmacy(sessionLicense, PageState))
                {
                    if (IsAvailableSession("LASTPHARMACYNAME"))
                    {
                        if (PageState.GetStringOrEmpty("LASTPHARMACYNAME").Length > 48)
                        {
                            pHdr.LastPharmacyName = PageState.GetStringOrEmpty("LASTPHARMACYNAME").Substring(0, 48);
                            pHdr.MoreRetailPharm = true;
                        }
                        else
                        {
                            pHdr.LastPharmacyName = PageState.GetStringOrEmpty("LASTPHARMACYNAME");
                        }
                    }
                    else
                    {
                        pHdr.LastPharmacyName = "None entered";
                    }

                    if (IsPharmacyEPCSEnabled(PageState) && pHdr.LastPharmacyName.ToString() != "None entered")
                    {
                        pHdr.IsRetailEpcs = true;
                    }
                }

                SetPatientActiveMedsControl(PageState, pHdr);

                if (ShowPharmacy(sessionLicense, PageState))
                {
                    pHdr.RemMOPharmVisible = AllowPharmacyEdit(PageState) && (IsAvailableSession("MOB_NABP"));
                    pHdr.RemPharmacyVisible = AllowPharmacyEdit(PageState) && (IsAvailableSession("LASTPHARMACYNAME"));
                }

                displayMailOrder(true, PageState, sessionLicense, pHdr);
            }
            else
            {
                // change this to return empty string, let the component display [No Patient Selected] when necessary
                pHdr.FirstName = "[No Patient Selected]";
            }

            SetEditOption(PageState, pHdr);

            return pHdr;
        }

        private static PhysicianMasterPage MasterPage(IStateContainer pageState)
        {
            var ms = new PhysicianMasterPage();
            ms.PageState = pageState;
            return ms;
        }

        private ApiResponse SetPatientInfo(IStateContainer session, string patientId,  string ip)
        {
            if (!string.IsNullOrEmpty(patientId))
            {
                setPatientInfo(patientId, session);
            }
            else
            {
                ClearPatientInfo(session);
            }

            return GetPatientHeader(patientId);
        }

        private static void setPatientInfo(string patientId, IStateContainer session)
        {
            MasterPage(session).SetPatientInfo(patientId);
            var dbid = session.Cast(Constants.SessionVariables.DbId, ConnectionStringPointer.ERXDB_DEFAULT);
            var userId = session.GetStringOrEmpty(Constants.SessionVariables.UserId);
            var licenseId = session.GetStringOrEmpty(Constants.SessionVariables.LicenseId);
            var siteId = session.GetInt(Constants.SessionVariables.SiteId, 1);

            if (AppCode.StateUtils.UserInfo.IsPOBUser(session))
            {
                sendPatientEligibilityRequest(patientId, licenseId, siteId, userId, dbid);
            }
            else
            {
                Allscripts.Impact.Patient.AddPatientEligibilityRequest(
                    licenseId, userId, patientId, null, userId, dbid);
            }
            
            Allscripts.Impact.Patient.AddPatientToSchedule(
                licenseId, userId, patientId,
                DateTime.Now, string.Empty, string.Empty, String.Empty,
                userId, dbid);

            session.Remove(Constants.SessionVariables.TaskScriptMessageId);
        }


        //[HttpPost]
        //public ApiResponse CheckIfUserIsRestricted([FromBody]string patientId)
        //{
        //    IStateContainer session = new StateContainer(HttpContext.Current.Session);

        //    return checkIfUserRestricted(session, patientId);
        //}

        private static bool checkIfUserRestricted(IStateContainer session, string patientId)
        {
            var dbid = session.Cast(Constants.SessionVariables.DbId, ConnectionStringPointer.ERXDB_DEFAULT);
            var userId = session.GetStringOrEmpty(Constants.SessionVariables.UserId);

            session["PrivacyPatientID"] = patientId;

            //
            // where/when should this session variable be set
            //
            //session["IsRestrictedUser"] = isUserRestricted = true;
            //session["IsRestrictedPatient"] = ?;

            DataTable dtPatientPrivacyRequestID = new Allscripts.Impact.PatientPrivacy().GetPatientPrivacyRequestID(patientId, userId, dbid);

            if (dtPatientPrivacyRequestID.Rows.Count > 0 && dtPatientPrivacyRequestID.Rows[0]["ID"] != DBNull.Value)
            {
                Int32 patientPrivacyRequestID = Convert.ToInt32(dtPatientPrivacyRequestID.Rows[0]["ID"].ToString());
                session["PatientPrivacyRequestID"] = patientPrivacyRequestID.ToString();

                return true;
            }
            else
            {
                // check if this is really needed
                session.Remove("IsUserRestrictedForPatient");
                return false;
            }
        }

        private static void ClearPatientInfo(IStateContainer pageState)
        {
            var ssoMode = pageState.GetString(Constants.SessionVariables.SSOMode, null);
            if (ssoMode == null || 
                (ssoMode != Constants.SSOMode.PATIENTLOCKDOWNMODE && 
                 ssoMode != Constants.SSOMode.EPAPATIENTLOCKDOWNTASKMODE &&
                 ssoMode != Constants.SSOMode.PatientContext))
            {
                PatientInfo.ClearPatientInfo(pageState);
            }
        }

        private static SelectPatientUserType GetUserType(IStateContainer session)
        {
            if (AppCode.StateUtils.UserInfo.IsPOBUser(session) || session.GetBooleanOrFalse("IsPASupervised"))
                return SelectPatientUserType.PAwithSupervision;

            if (session.GetBooleanOrFalse("ISPROVIDER") || session.GetBooleanOrFalse("IsPA"))
            {
                return SelectPatientUserType.Provider;
            }
            return SelectPatientUserType.Staff;
        }

        private static SelectPatientUserCategory GetUserCategory(IStateContainer session)
        {
            return (SelectPatientUserCategory)session["UserType"];
        }

        private static List<SelectPatientProvider> LoadProviders(IStateContainer session, string licenseId, int siteId, string userId, ConnectionStringPointer dbid)
        {
            // if a user is POB, load all providers only if IsPOBViewAllProviders is true
            // otherwise it's PA w/ sup, load all providers, too
            var loadAllActiveProviders = session.GetBooleanOrFalse("IsPOBViewAllProviders") || !AppCode.StateUtils.UserInfo.IsPOBUser(session);
            // if false, only selected providers are loaded for the POB.

            var Providers = new List<SelectPatientProvider>();
            DataSet ds = loadAllActiveProviders
                ? Allscripts.Impact.Provider.GetProviders(licenseId, siteId, dbid)
                : Allscripts.Impact.Provider.GetProviders(licenseId, siteId, userId, dbid);

            // TODO: rewrite with linq, or get another sproc.
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                if (!string.IsNullOrEmpty(row.ToStringVal("Active")) && !row.ToStringVal("Active").GetBooleanFromYOrN())// do not show disabled users in the ddl
                {
                    continue;
                }

                if (!string.IsNullOrEmpty(row.ToStringVal("IDPValidation")) && !row.ToStringVal("IDPValidation").GetBooleanFromYOrN()) // do not show Invaild ID Profing status users in ddl
                {
                    continue;
                }

                if (!string.IsNullOrEmpty(row.ToStringVal("StateLicenseValidation")) && !row.ToStringVal("StateLicenseValidation").GetBooleanFromYOrN()) // do not show Invaild ID Profing status users in ddl
                {
                    continue;
                }

                var userType = Convert.ToString(row["UserType"]);
                if (AppCode.StateUtils.UserInfo.GetSessionUserType(session) == Constants.UserCategory.PHYSICIAN_ASSISTANT_SUPERVISED && userType != "1")
                {
                    continue; // skip all but providers for POB with Sup.
                }

                SelectPatientUserCategory userCategory = default(SelectPatientUserCategory);
                Enum.TryParse(userType, true, out userCategory);

                Providers.Add(new SelectPatientProvider()
                {
                    ProviderId = Convert.ToString(row["ProviderID"]),
                    ProviderName = Convert.ToString(row["ProviderName"]),
                    UserCategory = userCategory
                });
            }

            Providers.Insert(0, new SelectPatientProvider() { ProviderId = "", ProviderName = "Select Provider" });

            Providers = GetUniqueProviders(Providers);

            return Providers;
        }

        public static List<SelectPatientProvider> GetUniqueProviders(List<SelectPatientProvider> providers)
        {
            return providers.GroupBy(Provider => Provider.ProviderId).Select(Provider => Provider.First()).ToList();
        }

        private static bool IsAnyTourToBeShown(GetMessageTrackingAcksResponse acks, string newUser, string newRelease,
                        Constants.WelcomeTourNewUser showNewUserTour, Constants.WelcomeTourNewRelease showNewReleaseTour, out string tourPath, out int tourType)
        {
            bool returnValue = false;
            tourPath = "";
            tourType = (int)Constants.WelcomeTourType.NewUser;
            try
            {
                if (acks != null)
                {
                    var ackList = acks.UserMessageTrackingAckList;
                    if (showNewUserTour == Constants.WelcomeTourNewUser.ON &&
                        ackList.FirstOrDefault(l => l.ConfigKey.ToString() == "d7687d09-07ea-458d-8546-97d6c195f89d") == null) //No new user tour ack present 
                    {
                        returnValue = true;
                        tourPath = newUser;
                        tourType = (int)Constants.WelcomeTourType.NewUser;
                    }
                    else if (showNewReleaseTour == Constants.WelcomeTourNewRelease.ON &&
                        ackList.FirstOrDefault(l => l.ConfigKey.ToString() == "7593cd82-5b81-4ede-a80e-05a6223f2cc4") == null) //And new release tour ack not present 
                    {
                        returnValue = true;
                        tourPath = newRelease;
                        tourType = (int)Constants.WelcomeTourType.NewRelease;
                    }
                }
            }
            catch
            {
                returnValue = false;
                tourPath = "";
            }
            return returnValue;
        }

        public static List<ePrescribeSvc.TeaserAdResponse> BuildTeaserAd(IStateContainer session, ConnectionStringPointer dbid)
        {
            var teaserAdResponseList = new List<ePrescribeSvc.TeaserAdResponse>();

            try
            {
                DateTime currentDate = DateTime.Now;
                bool isUserAdmin = session["IsAdmin"] != null ? Convert.ToString(session["IsAdmin"]).ToLower().Equals("true") : false;
                bool isFirstBusinessDayOfMonth = AppCode.DateUtils.IsFirstBusinessDayOfMonth(currentDate);
                var sessionLicense = ApiHelper.SessionLicense(session);
                teaserAdResponseList = EPSBroker.GetTeaserAdFromServiceAlert(
                    currentDate,
                    session.GetStringOrEmpty(Constants.SessionVariables.LicenseId),
                    session.GetStringOrEmpty(Constants.SessionVariables.PracticeState),
                    sessionLicense.EnterpriseClient.ID,
                    Convert.ToInt32(sessionLicense.DeluxePricingStructure),
                    isFirstBusinessDayOfMonth,
                    isUserAdmin);
            }
            catch (Exception ex)
            {
                Audit.AddException(
                    Guid.Empty.ToString(),
                    Guid.Empty.ToString(),
                    string.Concat("Main App: BasePage: Exception in BuildTeaserAd: ", ex.ToString()),
                    HttpContext.Current.Request.UserIpAddress(),
                    null,
                    null,
                    dbid);
            }
            
            return teaserAdResponseList;
        }

        private static string CreateTeaserAdCookieString(int hours)
        {
            //expected format: "eRxTeaserAdShown=; expires=31 Dec 2019 10:38:33 UTC; path=/"
            string expireDate = DateTime.UtcNow.AddHours(hours).ToString("dd MMM yyyy hh:mm:ss UTC");
            string cookie = $"{ Constants.Cookies.TeaserAdShown}=; expires={expireDate}; path=/";
            return cookie;
        }
    }
}