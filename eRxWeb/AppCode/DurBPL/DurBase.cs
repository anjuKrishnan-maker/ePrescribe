using System.Collections.Generic;
using System.Web;
using eRxWeb.State;
using UserInfo = eRxWeb.AppCode.StateUtils.UserInfo;
using Constants = Allscripts.ePrescribe.Common.Constants;
using Rx = Allscripts.Impact.Rx;
using eRxWeb.AppCode.StateUtils;
using DURSettings = Allscripts.ePrescribe.Objects.DURSettings;
using Allergy = Allscripts.ePrescribe.Medispan.Clinical.Model.Allergy;
using Allscripts.Impact.Tasks;
using Allscripts.ePrescribe.DatabaseSelector;
using System;
using eRxWeb.ServerModel;
using System.Web.Script.Serialization;
using Allscripts.Impact;
using Allscripts.ePrescribe.Medispan.Clinical.Model.Response;

namespace eRxWeb.AppCode.DurBPL
{
    public class DurBase /* Keep only properties*/
    {
        protected const string DURIdKey = "DurIndex";
        protected const string FullWarningTextKey = "FullWarningText";
        protected const string LineNumberKey = "LineNumber";
        protected const string RxIdKey = "RxID";
       
        public List<string> MedsWithDURs
        {
            get
            {
                List<string> ret = new List<string>();
                if (PageState[Constants.SessionVariables.CurrentMedsWithDur] != null)
                {
                    ret = PageState[Constants.SessionVariables.CurrentMedsWithDur] as List<string>;
                }
                return ret;
            }
            set
            {
                PageState[Constants.SessionVariables.CurrentMedsWithDur] = value;
            }
        }
        public RxUser DelegateProvider => UserInfo.GetDelegateProvider(PageState);
        public void ClearMedicationInfo(bool clearDx)
        {
            AppCode.StateUtils.MedicationInfo.ClearMedInfo(PageState, clearDx);
        }

        

        public int SessionSiteID => StateUtils.SiteInfo.GetSessionSiteID(PageState);
       
        public ChangeRxRequestedMedCs ChangeRxRequestedMedCs
        {
            get
            {
                return PageState[Constants.SessionVariables.ChangeRxRequestedMedCs] as ChangeRxRequestedMedCs;
            }
            set
            {
                PageState[Constants.SessionVariables.ChangeRxRequestedMedCs] = value;
            }
        }
        public List<string> SiteEPCSAuthorizedSchedules
        {
            get
            {
                List<string> siteEPCSAuthorizedSchedules = new List<string>();

                if (PageState[Constants.SessionVariables.SiteEPCSAuthorizedSchedules] != null)
                {
                    siteEPCSAuthorizedSchedules = (List<string>)PageState[Constants.SessionVariables.SiteEPCSAuthorizedSchedules];
                }

                return siteEPCSAuthorizedSchedules;
            }
        }
        public ApplicationLicense SessionLicense => UserInfo.GetSessionLicense(PageState);
       
        public bool IsEnterpriseClientEPCSEnabled=> PageState.GetBooleanOrFalse(Constants.SessionVariables.IsEnterpriseEpcsEnabled);
        
        public bool IsLicenseEPCSPurchased => EPCSWorkflowUtils.IsLicenseEpcsPurchased(SessionLicense);
        public bool IsLicenseShieldEnabled => PageState.GetBooleanOrFalse(Constants.SessionVariables.IsLicenseShieldEnabled);
       
        public bool IsUserEPCSEnabled => DoesUserHavePermission(UserPermissions.EpcsCanPrescribe);
        

        public bool DoesUserHavePermission(string permissionUri)
        {
            bool hasPermission = false;

            if (PageState[Constants.SessionVariables.UserAppPermissions] != null)
            {
                ePrescribeSvc.Permission[] appPermissions = (ePrescribeSvc.Permission[])PageState[Constants.SessionVariables.UserAppPermissions];

                if (string.IsNullOrEmpty(permissionUri))
                {
                    throw new ApplicationException("permissionUri cannot be null or empty");
                }

                if (appPermissions != null)
                {
                    //now check the claim from the permissions collection
                    hasPermission = Helper.DoesClaimExist(appPermissions, permissionUri);
                }
            }

            return hasPermission;
        }
        public bool CanTryEPCS
        {
            get
            {
                bool canTryEPCS = false;

                if ((SessionLicense.LicenseDeluxeStatus == Constants.DeluxeFeatureStatus.On ||
                  SessionLicense.LicenseDeluxeStatus == Constants.DeluxeFeatureStatus.AlwaysOn ||
                      SessionLicense.LicenseDeluxeStatus == Constants.DeluxeFeatureStatus.Cancelled)
                && (this.IsEnterpriseClientEPCSEnabled || this.IsLicenseEPCSPurchased)
                && this.IsLicenseShieldEnabled
                    && this.IsUserEPCSEnabled)
                {
                    canTryEPCS = true;
                }

                return canTryEPCS;
            }
        }
        public RxTaskModel RxTask
        {
            get
            {
                return PageState[Constants.SessionVariables.RxTask] as RxTaskModel;
            }
            set
            {
                PageState[Constants.SessionVariables.RxTask] = value;
            }
        }
        private IStateContainer pageState;
        public IStateContainer PageState { get {if (pageState == null)
                            pageState = new StateContainer(HttpContext.Current.Session);
                return pageState;
            } set { pageState = value; } }
        public string ShieldSecurityToken
        {
            get
            {
                return ShieldInfo.GetShieldSecurityToken(PageState);
            }
        }
        public List<Rx> ScriptPadMeds
        {
            get
            {
               return eRxWeb.AppCode.StateUtils.MedicationInfo.GetScriptPadMeds(PageState);
            }
            set
            {
                PageState["CURRENT_SCRIPT_PAD_MEDS"] = value;
            }
        }
        public Constants.UserCategory SessionUserType => UserInfo.GetSessionUserType(PageState);
        public bool IsUserAPrescribingUserWithCredentials
        {
            get
            {
                return
                    this.SessionUserType == Constants.UserCategory.PROVIDER ||
                    this.SessionUserType == Constants.UserCategory.PHYSICIAN_ASSISTANT ||
                    this.SessionUserType == Constants.UserCategory.PHYSICIAN_ASSISTANT_SUPERVISED;
            }
        }
        /// <summary>
        /// Returns the first rx in the RxList Session variable
        /// </summary>
        public Rx CurrentRx
        {
            get
            {
                return StateUtils.MedicationInfo.CurrentRx(PageState);
            }
        }
        public bool CanApplyFinancialOffers
        {
            get
            {
                return UserInfo.CanApplyFinancialOffers(PageState);
            }
        }
        public DURSettings DURSettings => PageState.Cast(Constants.SessionVariables.DURSettings, new DURSettings());
        public bool IsPOBUser => UserInfo.IsPOBUser(PageState);
        public string SessionPatientID => PatientInfo.GetSessionPatientID(PageState);
        public List<KeyValuePair<Guid, int>> DDIToRxIDMap
        {
            get
            {                
                return (List<KeyValuePair<Guid, int>>)PageState[Constants.SessionVariables.DdiToRxIdMap];
            }
            set
            {
                PageState[Constants.SessionVariables.DdiToRxIdMap] = value;
            }
        }

        public List<Allergy> DurPatientAllergies => DurInfo.GetDurPatientAllergies(PageState);

        public string SessionLicenseID => UserInfo.GetSessionLicenseID(PageState);

        public string SessionUserID => UserInfo.GetSessionUserID(PageState);
        public ConnectionStringPointer DBID
        {
            get
            {
               ConnectionStringPointer _dbID = ConnectionStringPointer.ERXDB_DEFAULT;

                if (PageState["DBID"] != null)
                {
                    _dbID = (ConnectionStringPointer)PageState["DBID"];
                }
                else
                {
                    //try looking it up
                    if (PageState["UserID"] != null)
                    {
                        DataManagerSvc.EntityIdentifier entityID = new DataManagerSvc.EntityIdentifier();
                        entityID.Type = DataManagerSvc.IdentifierType.UserGUID;
                        entityID.Value = PageState["UserID"].ToString();

                        DataManagerSvc.DataManagerSvc dmSvc = new DataManagerSvc.DataManagerSvc();
                        dmSvc.Url = AppSettings("DataManagerSvc.DataManagerSvc");

                        _dbID = (ConnectionStringPointer)dmSvc.GetDatabasePointer(entityID);
                        PageState["DBID"] = _dbID;
                    }
                    else if (PageState["LicenseID"] != null)
                    {
                        DataManagerSvc.EntityIdentifier entityID = new DataManagerSvc.EntityIdentifier();
                        entityID.Type = DataManagerSvc.IdentifierType.LicenseGUID;
                        entityID.Value = PageState["LicenseID"].ToString();

                        DataManagerSvc.DataManagerSvc dmSvc = new DataManagerSvc.DataManagerSvc();
                        dmSvc.Url = AppSettings("DataManagerSvc.DataManagerSvc");

                        _dbID = (ConnectionStringPointer)dmSvc.GetDatabasePointer(entityID);
                        PageState["DBID"] = _dbID;
                    }
                }

                return _dbID;
            }
        }
        public string AppSettings(string key)
        {
            return System.Configuration.ConfigurationManager.AppSettings[key].ToString();
        }
        public string RedirectToSelectPatient(string queryString, SelectPatientComponentParameters selectPatientComponentParameters)
        {
            string redirectUrl = string.Empty;
            if ((queryString != null && queryString.Contains("StartOver=Y")) ||
                PageState.GetBooleanOrFalse(Constants.SessionVariables.AppComponentAlreadyInitialized))//since it is coming from Angular component)
            {
                //WHEN APP COMPONENT ALREADY EXISTS
                var componentParameters = new JavaScriptSerializer().Serialize(selectPatientComponentParameters);
                redirectUrl = Constants.PageNames.REDIRECT_TO_ANGULAR + "?componentName=" + Constants.PageNames.SELECT_PATIENT + "&componentParameters=" + componentParameters;
            }
            else
            {
                redirectUrl = AngularStringUtil.CreateInitComponentUrl(Constants.PageNames.SELECT_PATIENT);
            }
            return redirectUrl;
        }
    }
}