using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web.UI;
using System.Web;
using System.Web.UI.WebControls;
using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.DatabaseSelector;
using Allscripts.ePrescribe.Shared.Logging;
using Allscripts.Impact;
using Microsoft.IdentityModel.Claims;
using Microsoft.IdentityModel.Tokens.Saml11;
using Microsoft.IdentityModel.Tokens;
using System.Xml;
using TieServiceClient;
using System.Web.UI.HtmlControls;
using eRxWeb.AppCode.Interfaces;
using eRxWeb.State;
using System.Globalization;
using Allscripts.ePrescribe.ExtensionMethods;
using Allscripts.ePrescribe.Objects;
using Allscripts.ePrescribe.Medispan.Clinical.Model.Settings;
using Allscripts.ePrescribe.Objects.PPTPlus;
using eRxWeb.AppCode;
using eRxWeb.AppCode.PptPlusBPL;
using eRxWeb.AppCode.StateUtils;
using Allergy = Allscripts.ePrescribe.Medispan.Clinical.Model.Allergy;
using Patient = Allscripts.Impact.Patient;
using Rx = Allscripts.Impact.Rx;
using RxUser = Allscripts.Impact.RxUser;
using UserProperties = Allscripts.Impact.UserProperties;
using ConfigKeys = Allscripts.Impact.ConfigKeys;
using DURSettings = Allscripts.ePrescribe.Objects.DURSettings;
using GetMessageTrackingAcksResponse = eRxWeb.ePrescribeSvc.GetMessageTrackingAcksResponse;
using PatientInfo = eRxWeb.AppCode.StateUtils.PatientInfo;
using WelcomeTourNewUser = Allscripts.ePrescribe.Common.Constants.WelcomeTourNewUser;
using WelcomeTourNewRelease = Allscripts.ePrescribe.Common.Constants.WelcomeTourNewRelease;
using UserInfo = eRxWeb.AppCode.StateUtils.UserInfo;
using eRxWeb.ServerModel;
using System.Web.Script.Serialization;
using PatientPrivacy = Allscripts.Impact.PatientPrivacy;
using eRxWeb.AppCode.Authorize;

namespace eRxWeb
{
    public class BasePage : System.Web.UI.Page, IBasePage
    {
        /// <summary>
        /// Should only be used on basePage, access not to be other than private
        /// </summary>
        private static ILoggerEx logger = LoggerEx.GetLogger();
        public bool IsCsTaskWorkflow => (PageState[Constants.SessionVariables.TaskScriptMessageId] != null && (PageState[Constants.SessionVariables.IsCsRefReqWorkflow] != null || MasterPage.ChangeRxRequestedMedCs != null));

        public PhysicianMasterPage MasterPage
        {
            get
            {
                var ms = Page.Master as PhysicianMasterPage;
                if (ms == null)
                {
                    ms = new PhysicianMasterPage();
                    ms.PageState = this.PageState;
                }

                return ms;

            }
        }
        #region Member Variables



        //this for AJAX compatible refresh checking (though EAK has his doubts!)
        bool _isrefresh = false;
        string _tname = "__Ticket";

        ConnectionStringPointer _dbID;

        private IPlacementResponse _placementResponse;

        #endregion

        #region Properties

        public IStateContainer PageState { get; set; }

        public string ClinicalViewerPostData
        {
            get
            {
                string postData = string.Empty;
                postData = EPSBroker.GetClinicalViewerSSOLink(
                    SessionPatientID,
                    SessionLicense.EnterpriseClient.SystemOID,
                    Convert.ToString(Session["UserFirstName"]),
                    Convert.ToString(Session["UserLastName"]),
                    SessionUserName,
                    SessionUserType.ToString(), //"Nurse"
                    SessionLicense.EnterpriseClient.CommunityURL
                    );

                return postData;
            }
            set
            {
                Session["ClinicalViewerPostData"] = value;
            }
        }

        public int PageExpirationTimerMs => PageState.GetInt(Constants.SessionVariables.ShieldTokenTimeRemainingMs, 1200000);

        /// <summary>
        /// Shield's External AppInstanceID as specified by ePrescribe to Shield on user creation
        /// </summary>
        public string EprescribeExternalAppInstanceID
        {
            get { return ShieldSettings.ePrescribeExternalAppInstanceId(new StateContainer(Session)); }
        }

        /// <summary>
        /// Shield's internal database Teneant ID that is used in some Provisioning Service calls
        /// </summary>
        public int ShieldInternalTenantID
        {
            get
            {
                int internalTenantID = 0;

                if (this.IsLicenseShieldEnabled)
                {
                    if (Session["ShieldInternalTenantID"] == null)
                    {
                        Session["ShieldInternalTenantID"] = EPSBroker.GetShieldInternalTenantID(Session["AHSAccountID"].ToString());
                    }

                    internalTenantID = Convert.ToInt32(Session["ShieldInternalTenantID"]);
                }

                return internalTenantID;
            }
        }

        public string ShieldExternalTenantID
        {
            get
            {
                return ShieldSettings.GetShieldExternalTenantID(PageState);
            }
        }

        /// <summary>
        /// Shield's internal database Application ID that is used in some Provisioning Service calls
        /// </summary>
        public int ShieldInternalAppID
        {
            get
            {
                return ShieldSettings.GetShieldInternalAppID(PageState);
            }
        }

        /// <summary>
        /// Shield's internal database Environment ID that is used in some Provisioning Service calls
        /// </summary>
        public int ShieldInternalEnvironmentID
        {
            get
            {
                return ShieldSettings.GetShieldInternalEnvironmentID(PageState);
            }
        }

        /// <summary>
        /// Shield's internal database AppInstance ID that is used in some Provisioning Service calls
        /// </summary>
        public int ShieldInternalAppInstanceID
        {
            get
            {
                return ShieldSettings.GetShieldInternalAppInstanceID(PageState);
            }
        }

        /// <summary>
        /// Returns Shield's internal user profile ID. If it doesn't already exist in Session, it will look it up. 
        /// Requires the userID and licenseID to already be set in session.
        /// </summary>
        public int ShieldInternalUserProfileID
        {
            get
            {
                if (Session["ShieldProfileUserID"] == null)
                {
                    ePrescribeSvc.GetUserResponse getUserRes = EPSBroker.GetRxUser(
                        ePrescribeSvc.ValueType.UserGUID,
                        this.SessionUserID,
                        this.SessionLicenseID,
                        this.SessionUserID,
                        this.SessionLicenseID,
                        this.DBID);

                    if (getUserRes.RxUser != null)
                    {
                        Session["ShieldProfileUserID"] = getUserRes.RxUser.ShieldProfileUserID;
                    }
                    else
                    {
                        throw new ApplicationException("Cannot set ShieldProfileUserID");
                    }
                }

                return Convert.ToInt32(Session["ShieldProfileUserID"]);
            }
        }

        public bool IsPasswordSetupRequiredForSSOUser
        {
            get
            {
                return (Session[Constants.SessionVariables.ForcePasswordSetupForSSOUser] != null && bool.Parse(Session[Constants.SessionVariables.ForcePasswordSetupForSSOUser].ToString())) ||
                        (Session["PasswordExpiredForSSOUser"] != null && bool.Parse(Session["PasswordExpiredForSSOUser"].ToString()));
            }
        }


        public bool CanApplyFinancialOffers
        {
            get
            {
                return UserInfo.CanApplyFinancialOffers(PageState);
            }
        }

        public List<Allergy> DurPatientAllergies => DurInfo.GetDurPatientAllergies(PageState);


        public bool CanDisplayInfoScripts
        {
            get
            {
                bool canDisplayInfoScripts = false;

                if (Session["DisplayInfoScripts"] != null)
                {
                    canDisplayInfoScripts = bool.Parse(Session["DisplayInfoScripts"].ToString());
                }
                else
                {
                    Session["DisplayInfoScripts"] = canDisplayInfoScripts;
                }

                return canDisplayInfoScripts;
            }
        }

        public bool CanAllowPatientReceipts
        {
            get
            {
                bool canAllowPatientReceipts = false;

                if (Session["ALLOWPATIENTRECEIPT"] != null)
                {
                    canAllowPatientReceipts = bool.Parse(Session["ALLOWPATIENTRECEIPT"].ToString());
                }
                else
                {
                    Session["ALLOWPATIENTRECEIPT"] = canAllowPatientReceipts;
                }

                return canAllowPatientReceipts;
            }
        }

        /// <summary>
        /// Checks all the conditions that are necessary for a user to be able to attempt an EPCS transaction.
        /// Conditions include:
        ///   1) license must be Shield on
        ///   2) the enterprise client must be EPCS enabled or License has purchased EPCS
        ///   3) the user must have the EPCS permission
        ///   4) User's EPCS purchase is not cancelled
        ///   5) license Deluxe status should be On/AlwaysOn/Cancelled
        /// </summary>
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
                    && UserInfo.IsPermissionGranted(UserPermissions.EpcsCanPrescribe, PageState))
                {
                    canTryEPCS = true;
                }

                return canTryEPCS;
            }
        }

        public bool CheckFormAltsAndCopay
        {
            get
            {
                return ((Session["FORMULARYID"] != null && Session["FORMULARYID"].ToString().Trim() != string.Empty)
                    || (Session["COVERAGEID"] != null && Session["COVERAGEID"].ToString().Trim() != string.Empty)
                    || (Session["COPAYID"] != null && Session["COPAYID"].ToString().Trim() != string.Empty));
            }
        }

        /// <summary>
        /// Returns the first rx in the RxList Session variable
        /// </summary>
        public Rx CurrentRx
        {
            get
            {
                return eRxWeb.AppCode.StateUtils.MedicationInfo.CurrentRx(PageState);
            }
        }

        /// <summary>
        /// Gets and sets the refill request details for the current workflow in session
        /// </summary>
        public RefReq CurrentRefReq
        {
            get
            {
                RefReq refReq = new RefReq();

                if (Session["RefReqObject"] != null)
                {
                    refReq = (RefReq)Session["RefReqObject"];
                }

                return refReq;
            }
            set
            {
                Session["RefReqObject"] = value;
            }
        }

        public ConnectionStringPointer DBID
        {
            get
            {
                _dbID = ConnectionStringPointer.ERXDB_DEFAULT;

                if (Session["DBID"] != null)
                {
                    _dbID = (ConnectionStringPointer)Session["DBID"];
                }
                else
                {
                    //try looking it up
                    if (Session["UserID"] != null)
                    {
                        DataManagerSvc.EntityIdentifier entityID = new DataManagerSvc.EntityIdentifier();
                        entityID.Type = DataManagerSvc.IdentifierType.UserGUID;
                        entityID.Value = Session["UserID"].ToString();

                        DataManagerSvc.DataManagerSvc dmSvc = new DataManagerSvc.DataManagerSvc();
                        dmSvc.Url = AppSettings("DataManagerSvc.DataManagerSvc");

                        _dbID = (ConnectionStringPointer)dmSvc.GetDatabasePointer(entityID);
                        Session["DBID"] = _dbID;
                    }
                    else if (Session["LicenseID"] != null)
                    {
                        DataManagerSvc.EntityIdentifier entityID = new DataManagerSvc.EntityIdentifier();
                        entityID.Type = DataManagerSvc.IdentifierType.LicenseGUID;
                        entityID.Value = Session["LicenseID"].ToString();

                        DataManagerSvc.DataManagerSvc dmSvc = new DataManagerSvc.DataManagerSvc();
                        dmSvc.Url = AppSettings("DataManagerSvc.DataManagerSvc");

                        _dbID = (ConnectionStringPointer)dmSvc.GetDatabasePointer(entityID);
                        Session["DBID"] = _dbID;
                    }
                }

                return _dbID;
            }
        }

        /// <summary>
        /// This is the provider selected when a POB user has logged in and selected a provider or unsupervised physician assistant to write perscriptions on their behalf,
        /// OR when a supervised physician assistant has logged in selected a provider or unsupervised physician assistant to write perscriptions on their behalf (which should be changed to a Supervising Provider)
        /// </summary>
        public RxUser DelegateProvider => UserInfo.GetDelegateProvider(PageState);

        public string DelegateProviderName => UserInfo.GetDelegateProviderName(PageState);

        /// <summary>
        /// This is the provider selected when a POB user has logged in and selected a supervised physician assistant to write prescription on their behalf.
        /// Also, when a POB user has selected a refill request designated for a supervised physician assistant
        /// </summary>

        public RxUser SupervisingProvider => UserInfo.GetSupervisingProvider(PageState);
        public string SupervisingProviderName => UserInfo.GetSupervisingProviderName(PageState);

        public DURSettings DURSettings => PageState.Cast(Constants.SessionVariables.DURSettings, new DURSettings());

        public DUR DURChecker => new DUR
        {
            CurrentRxDDI = CurrentRx.DDI,
            CurrentRxQuantity = CurrentRx.Quantity,
            CurrentRxDaysSupply = CurrentRx.DaysSupply,
            CurrentRxSigID = CurrentRx.SigID,
            CurrentRxRefills = CurrentRx.Refills,
            CurrentRxMedicationName = CurrentRx.MedicationName,
            CurrentRxStrength = CurrentRx.Strength,
            CurrentRxStrengthUOM = CurrentRx.StrengthUOM,
            CurrentRxDosageFormCode = CurrentRx.DosageFormCode,
            SessionLicenseID = this.SessionLicenseID,
            SessionPatientID = this.SessionPatientID,
            SessionSex = PageState.GetStringOrEmpty("SEX"),
            SessionDBID = DBID
        };

        public Module FeaturedModule
        {
            get
            {
                if (Session["InterstitialAd"] == null)
                {
                    Session["InterstitialAd"] = new Module(Module.ModuleType.UNKNOWN, DBID);

                    string cookieName = string.Empty;
                    foreach (Module module in Module.GetModules(SessionLicenseID, SessionUserID, 2, SessionLicense.AdvertiseDeluxe, DBID))
                    {
                        if (module.Advertise    //Module to be advertised
                            && ((module.Type == Module.ModuleType.DELUXE //Module is Deluxe but user not already using it
                                    && SessionLicense.AdvertiseDeluxe
                                    && SessionLicense.LicenseDeluxeStatus != Constants.DeluxeFeatureStatus.On
                                    && SessionLicense.LicenseDeluxeStatus != Constants.DeluxeFeatureStatus.Cancelled
                                    && SessionLicense.EnterpriseDeluxeFeatureStatus != Constants.DeluxeFeatureStatus.Force) //Dont show for forced enterprises
                                || (module.Type != Module.ModuleType.DELUXE //Or Module is not Deluxe, applicable for enterprise and also not already enabled
                                    && SessionLicense.EnterpriseClient.ShowIntegrationSolutions
                                    && !module.Enabled)))
                        {
                            cookieName = "eRxNow" + module.ModuleID;
                            if (Request.Cookies[cookieName] == null)
                            {
                                Session["InterstitialAd"] = module;
                            }
                        }
                    }
                }

                return (Module)Session["InterstitialAd"];
            }
            set
            {
                Session["InterstitialAd"] = null;
            }
        }

        public bool IsEnterpriseClientEPCSEnabled
        {
            get
            {
                bool isEPCSEnabled = false;

                if (Session["ISENTCLIENTEPCSENABLED"] != null)
                {
                    isEPCSEnabled = (bool)Session["ISENTCLIENTEPCSENABLED"];
                }

                return isEPCSEnabled;
            }
        }

        public bool IsMOPharmacyEPCSEnabled
        {
            get
            {
                bool isEPCSEnabled = false;

                if (Session["MOB_ISEPCSENABLED"] != null)
                {
                    isEPCSEnabled = (bool)Session["MOB_ISEPCSENABLED"];
                }

                return isEPCSEnabled;
            }
        }

        public bool IsPharmacyEPCSEnabled
        {
            get
            {
                bool isPharmacyEPCSEnabled = false;

                if (Session["ISPHARMACYEPCSENABLED"] != null)
                {
                    isPharmacyEPCSEnabled = (bool)Session["ISPHARMACYEPCSENABLED"];
                }

                return isPharmacyEPCSEnabled;
            }
        }

        public bool IsPOBUser => UserInfo.IsPOBUser(PageState);

        public bool IsPOBViewAllProviders
        {
            get
            {
                bool isPOBViewAllProviders = false;

                if (Session["IsPOBViewAllProviders"] != null && Session["IsPOBViewAllProviders"].ToString().Equals(true.ToString()))
                {
                    isPOBViewAllProviders = true;
                }

                return isPOBViewAllProviders;
            }
        }

        public List<string> MedsWithDURs
        {
            get
            {
                List<string> ret = new List<string>();
                if (Session["CURRENT_MEDS_WITH_DUR"] != null)
                {
                    ret = Session["CURRENT_MEDS_WITH_DUR"] as List<string>;
                }
                return ret;
            }
            set
            {
                Session["CURRENT_MEDS_WITH_DUR"] = value;
            }
        }

        public bool PatientActive
        {
            get
            {
                bool ret = true;
                if (Session["PatientActive"] == null)
                {
                    ret = false;
                }
                else
                {
                    ret = Convert.ToBoolean(Session["PatientActive"]);
                }

                return ret;
            }
        }



        public bool PatientHasScriptPadItems
        {
            get
            {
                return ScriptPadMeds.Count > 0;
            }
        }

        public List<Rx> ScriptPadMeds
        {
            get
            {
                List<Rx> rxList = new List<Rx>();
                return rxList = eRxWeb.AppCode.StateUtils.MedicationInfo.GetScriptPadMeds(PageState);
            }
            set
            {
                Session["CURRENT_SCRIPT_PAD_MEDS"] = value;
            }
        }

        public Constants.UserCategory SessionUserType => UserInfo.GetSessionUserType(PageState);

        public ApplicationLicense SessionLicense
        {
            get
            {
                return Settings.GetSessionLicense(PageState);
            }
        }

        public string SessionAppVersion
        {
            get
            {
                string ret = string.Empty;
                if (Session["EPRESCRIBE_APP_VERSION"] != null)
                {
                    ret = Session["EPRESCRIBE_APP_VERSION"].ToString();
                }
                else
                {
                    try
                    {
                        ePrescribeSvc.ePrescribeSvc eps = new ePrescribeSvc.ePrescribeSvc();
                        eps.Url = ConfigurationManager.AppSettings["ePrescribeSvc.eprescribesvc"].ToString();
                        ePrescribeSvc.ApplicationVersionRequest appVerReq = new ePrescribeSvc.ApplicationVersionRequest();
                        appVerReq.ApplicationID = ePrescribeSvc.ePrescribeApplication.MainApplication;
                        Session["EPRESCRIBE_APP_VERSION"] = eps.GetFullAppversion(appVerReq);

                        ret = Session["EPRESCRIBE_APP_VERSION"].ToString();
                    }
                    catch (Exception)
                    {
                        ret = string.Empty;
                    }
                }
                return ret;
            }
        }

        public string SessionPartnerID
        {
            get
            {
                if (Session["PartnerID"] == null)
                    return string.Empty;

                return Session["PartnerID"].ToString();
            }
        }

        public string SessionUserID => UserInfo.GetSessionUserID(PageState);

        public string SessionDelegateProviderID
        {
            get
            {
                if (Session["DelegateProviderID"] == null)
                    return null;

                return Session["DelegateProviderID"].ToString();
            }
        }

        public string SessionUserName
        {
            get
            {
                if (Session["UserName"] != null)
                {
                    return Session["UserName"].ToString();
                }

                return string.Empty;
            }
        }

        public string SessionShieldUserName
        {
            get
            {
                if (Session["SessionShieldUserName"] != null)
                {
                    return Session["SessionShieldUserName"].ToString();
                }

                return string.Empty;
            }
        }

        public string SessionEprescribeUserName
        {
            get
            {
                if (Session["EprescribeUsername"] != null)
                {
                    return Session["EprescribeUsername"].ToString();
                }

                return string.Empty;
            }
        }

        public string SessionLoginID
        {
            get
            {
                string loginID = string.Empty;

                if (Session["USERCODE"] != null)
                {
                    loginID = Session["USERCODE"].ToString();
                }

                return loginID;
            }
        }

        public string SessionLicenseID => UserInfo.GetSessionLicenseID(PageState);

        public int SessionSiteID
        {
            get
            {
                return AppCode.StateUtils.SiteInfo.GetSessionSiteID(PageState);
            }
        }

        public string SessionSitePharmacyID
        {
            get
            {
                if (Session["SITE_PHARMACY_ID"] == null)
                    return null;

                return Session["SITE_PHARMACY_ID"].ToString();
            }
        }

        public string SessionPatientID => PatientInfo.GetSessionPatientID(PageState);

        public string SessionSSOMode
        {
            get
            {
                if (Session["SSOMode"] == null)
                    return string.Empty;

                return Session["SSOMode"].ToString();
            }
        }

        public string SessionPracticeState => UserInfo.GetSessionPracticeState(PageState);

        public int SessionStanding
        {
            get
            {
                return Convert.ToInt32(Session["STANDING"]);
            }
        }

        public bool SiteHasAdmin
        {
            get
            {
                bool ret = false;
                if (Session["SITE_HAS_ADMIN"] != null)
                {
                    ret = Convert.ToBoolean(Session["SITE_HAS_ADMIN"]);
                }
                else
                {
                    ret = ApplicationLicense.SiteHasAdmin(SessionLicenseID, DBID);
                    Session["SITE_HAS_ADMIN"] = ret;
                }

                return ret;
            }
        }

        public List<string> SiteEPCSAuthorizedSchedules
        {
            get
            {
                List<string> siteEPCSAuthorizedSchedules = new List<string>();

                if (Session["SiteEPCSAuthorizedSchedules"] != null)
                {
                    siteEPCSAuthorizedSchedules = (List<string>)Session["SiteEPCSAuthorizedSchedules"];
                }

                return siteEPCSAuthorizedSchedules;
            }
        }

        public bool ShowGenericAlternatives
        {
            get
            {
                bool showGenericAlternatives = false;
                if (Session["FORMULARYID"] == null)
                {
                    showGenericAlternatives = true;
                }
                else if (string.IsNullOrEmpty(Session["FORMULARYID"].ToString()))
                {
                    showGenericAlternatives = true;
                }

                return showGenericAlternatives;
            }

        }


        public bool ShowEPA
        {
            get
            {
                return SessionLicense.EnterpriseClient.ShowEPA;
            }
        }


        /// <summary>
        /// Check show Rx Info at license level, if exists,  return true or else false.
        /// </summary>
        public bool IsShowRxInfo
        {
            get
            {
                return RxInfo.IsShowRxInfo(PageState);
            }
        }

        public bool IsSendSMSEnabled
        {
            get { return Settings.IsSendSMSEnabled(new StateContainer(Session)); }
        }

        public bool IsPPTPlusEnabled
        {
            get
            {
                return SessionLicense.EnterpriseClient.ShowPPTPlus;
            }
        }

        public bool IsPDMPEnabled
        {
            get
            {
                return SessionLicense.EnterpriseClient.ShowPDMP;
            }
        }

        public bool IsPreBuiltPrescriptionsEnabled
        {
            get
            {
                return (SessionLicense.EnterpriseClient.EnablePrebuiltPrescriptions &&
                   PageState.GetBooleanOrFalse("ShowPrebuiltPrescriptions") &&
                   (SessionLicense.LicenseDeluxeStatus == Constants.DeluxeFeatureStatus.On ||
                   SessionLicense.LicenseDeluxeStatus == Constants.DeluxeFeatureStatus.AlwaysOn ||
                   SessionLicense.LicenseDeluxeStatus == Constants.DeluxeFeatureStatus.Cancelled));

            }
        }

        public bool IsLexicompEnabled
        {
            get
            {
                var isEnabled = false;
                if (SessionLicense.EnterpriseClient.EnableLexicomp &&
                              (SessionLicense.LicenseDeluxeStatus == Constants.DeluxeFeatureStatus.On ||
                                SessionLicense.LicenseDeluxeStatus == Constants.DeluxeFeatureStatus.AlwaysOn ||
                                SessionLicense.LicenseDeluxeStatus == Constants.DeluxeFeatureStatus.Cancelled))
                {

                    if (PageState.GetBooleanOrFalse("ShowLexicompDefault"))
                    {
                        if (SessionLicense.EnterpriseClient.EnableLexicompDefault)
                            isEnabled = true;
                    }
                    else
                    {
                        if (PageState.GetBooleanOrFalse(Constants.LicensePreferences.SHOW_LEXICOMP))
                        {
                            isEnabled = true;
                        }
                    }
                }
                return isEnabled;

            }
        }

        public ePrescribeSvc.Role[] ShieldAppRoles
        {
            get
            {
                return ShieldSettings.GetShieldAppRoles(PageState);
            }
        }

        public string ShieldSecurityToken
        {
            get
            {
                return ShieldInfo.GetShieldSecurityToken(PageState);

                /*
                 This is no longer implemented as WIF has been removed

                else
                {
                    IClaimsPrincipal currentPrincipal = Thread.CurrentPrincipal as IClaimsPrincipal;

                    if (currentPrincipal != null && currentPrincipal.Identities[0] != null)
                    {
                        SecurityToken bootstrapToken = currentPrincipal.Identities[0].BootstrapToken;

                        if (bootstrapToken != null)
                        {
                            StringBuilder sb = new StringBuilder();

                            XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
                            xmlWriterSettings.OmitXmlDeclaration = true;

                            using (var writer = XmlWriter.Create(sb, xmlWriterSettings))
                            {
                                new Saml11SecurityTokenHandler(new SamlSecurityTokenRequirement()).WriteToken(writer, bootstrapToken);
                            }

                            retVal = sb.ToString();
                        }
                    }
                }
                */

            }
        }

        /// <summary>
        /// Checks the license's Shield status. True if Shield On, false otherwise.
        /// </summary>
        public bool IsLicenseShieldEnabled
        {
            get
            {
                return Settings.IsLicenseShieldEnabled(PageState);
            }
        }
        /// <summary>
        /// Checks the license's EPCS purchase status. True if EPCS On, false otherwise.
        /// </summary>
        public bool IsLicenseEPCSPurchased => EPCSWorkflowUtils.IsLicenseEpcsPurchased(SessionLicense);

        /// <summary>
        /// Checks if the user has had the EPCS permission in the last 2 years. True if the user has, false otherwise.
        /// </summary>
        public bool UserHasHadEpcsPermissionInTheLastTwoYears
        {
            get
            {
                bool hasHadPerm = false;

                if (Session["UserHasHadEpcsPermissionInTheLastTwoYears"] != null)
                {
                    hasHadPerm = Convert.ToBoolean(Session["UserHasHadEpcsPermissionInTheLastTwoYears"]);
                }

                return hasHadPerm;
            }
        }


        /// <summary>
        /// Returns boolean indicating if the user is a provider, PA, or PA with supervision. Each of these user roles has professional credentials (e.g. a DEA and/or NPI).
        /// </summary>
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
        /// Checks the users token for the DeaRegistrant property value. If exists, returns the value of that property.
        /// </summary>
        public string UserDeaRegistrantStatus
        {
            get
            {
                string deaRegStatus = null;

                deaRegStatus = GetClaimPropertyValue(UserProperties.DeaRegistrant);

                return deaRegStatus;
            }
        }


        /// <summary>
        /// returns a string containing the Rx image path
        /// </summary>
        public string Current_Rx_Rightside_Display_Image_Path
        {
            get
            {
                return get_current_Rx_Rightside_Display_Image_Text("Image");
            }
        }

        /// <summary>
        /// returns a string containing the Rx image Tooltip text
        /// </summary>
        public string Current_Rx_Rightside_Display_Image_ToolTip
        {
            get
            {
                return get_current_Rx_Rightside_Display_Image_Text("ToolTip");
            }
        }

        private string get_current_Rx_Rightside_Display_Image_Text(string returnType)
        {
            string retval = string.Empty;
            string imgPath = string.Empty;
            string imgToolTip = string.Empty;

            if (!string.IsNullOrEmpty(CurrentRx.FormularyStatus))
            {
                bool isOTC = false;

                if (!string.IsNullOrEmpty(CurrentRx.IsOTC))
                {
                    isOTC = (CurrentRx.IsOTC == "Y");
                }

                MedicationSearchDisplay.GetFormularyImagePathWithTooltip(Convert.ToInt32(CurrentRx.FormularyStatus), isOTC, out imgPath, out imgToolTip);
            }

            switch (returnType)
            {
                case "Image":
                    retval = imgPath;
                    break;
                case "ToolTip":
                    retval = imgToolTip;
                    break;
            }

            return retval;
        }
        /// <summary>
        /// returns string of the current rx formatted for display on the right side of the page
        /// </summary>
        public string CurrentRxRightsideDisplayText
        {
            get
            {
                StringBuilder sbDisplayText = new StringBuilder();

                if (this.CurrentRx != null)
                {
                    if (!string.IsNullOrWhiteSpace(this.CurrentRx.MedicationName))
                    {
                        sbDisplayText.Append(this.CurrentRx.MedicationName.Trim());
                        sbDisplayText.Append(" ");
                    }

                    if (!string.IsNullOrWhiteSpace(this.CurrentRx.Strength))
                    {
                        sbDisplayText.Append(this.CurrentRx.Strength.Trim());
                        sbDisplayText.Append(" ");
                    }

                    if (!string.IsNullOrWhiteSpace(this.CurrentRx.StrengthUOM))
                    {
                        sbDisplayText.Append(this.CurrentRx.StrengthUOM.Trim());
                        sbDisplayText.Append(" ");
                    }

                    if (!string.IsNullOrWhiteSpace(this.CurrentRx.DosageFormDescription))
                    {
                        sbDisplayText.Append(this.CurrentRx.DosageFormDescription.Trim());
                        sbDisplayText.Append(" ");
                    }

                    if (!string.IsNullOrWhiteSpace(this.CurrentRx.RouteOfAdminDescription))
                    {
                        sbDisplayText.Append(this.CurrentRx.RouteOfAdminDescription.Trim());
                    }
                }

                return sbDisplayText.ToString();
            }
        }

        public bool IsBackdoorUser
        {
            get
            {
                bool isBackdoorUser = false;

                if (Session["IsBackDoorUser"] != null)
                {
                    isBackdoorUser = true;
                }

                return isBackdoorUser;
            }
        }

        public IPlacementResponse PlacementResponse
        {
            get { return _placementResponse; }
            set { _placementResponse = value; }
        }

        /// <summary>
        ///  return global setting for ID Proofing is turn on /off .
        /// </summary>
        /// <returns></returns>
        public bool IsIDProofingEnabled
        {
            get
            {
                bool isIDProofingEnabled = false;
                if (System.Configuration.ConfigurationManager.AppSettings["IDProofingRequired"] != null &&
                    bool.Parse(System.Configuration.ConfigurationManager.AppSettings["IDProofingRequired"]))
                {
                    isIDProofingEnabled = true;
                }
                return isIDProofingEnabled;
            }

        }

        public string NcpdpEpaUserShieldSecurityToken
        {
            get
            {
                if (IsLicenseShieldEnabled)
                {
                    if (Session["ShieldSecurityToken"] == null)
                        return null;

                    return Session["ShieldSecurityToken"].ToString();
                }
                else
                {
                    return EPSBroker.EPA.GetServiceAccountShieldToken();
                }
            }
        }

        /// <summary>
        /// returns global setting for NPI Check is turned on /off .
        /// </summary>
        /// <returns></returns>
        public bool IsNPICheckEnabled
        {
            get
            {
                bool isNPICheckEnabled = false;
                if (System.Configuration.ConfigurationManager.AppSettings["NPICheckRequired"] != null &&
                    bool.Parse(System.Configuration.ConfigurationManager.AppSettings["NPICheckRequired"]))
                {
                    isNPICheckEnabled = true;
                }
                return isNPICheckEnabled;
            }
        }

        public RxUser CurrentUser
        {
            get
            {
                RxUser currentUser;

                if (Session["UserIdentity"] != null)
                {
                    currentUser = ((RxIdentity)Session["UserIdentity"]).User;
                }
                else
                {
                    if (Session["USERID"] == null)
                    {
                        Response.Redirect(Constants.PageNames.LOGIN);
                    }
                    currentUser = null;
                    currentUser = new RxUser(SessionUserID, DBID);
                }

                return currentUser;
            }
        }

        public bool IsEnterpriseClientPreBuiltRxEnabled
        {
            get
            {
                return PageState.GetBooleanOrFalse("EnablePrebuiltPrescriptions");
            }
        }

        public PptPlusResponseContainer PptPlusResponses
        {
            get { return PageState.Cast(Constants.SessionVariables.PptPlusResponses, default(PptPlusResponseContainer)); }
            set { PageState[Constants.SessionVariables.PptPlusResponses] = value; }
        }

        public PptPlusRequestInfo PptPlusRequestInfo => PPTPlus.GetPptRequestInfo(PageState);

        #endregion

        #region Page Methods and Events

        public BasePage()
        {
            this.PreInit += new EventHandler(BasePage_PreInit);
        }

        void BasePage_PreInit(object sender, EventArgs e)
        {
            PageState = new StateContainer(Session);
            // Change null to 0 (prevents errors)
            if (Session[_tname + "_LastServed"] == null || Request[_tname] == null) { Session[_tname + "_LastServed"] = 0; }
            // Get current and last ticket served
            int ticket = 0;
            if (int.TryParse(Request[_tname], out ticket)) ticket++;

            int lastticket = 0;
            if (int.TryParse(PageState.GetStringOrEmpty(_tname + "_LastServed"), out lastticket))

                // Refresh detection
                if (ticket > lastticket || (ticket == 0 && lastticket == 0))
                {
                    Session[_tname + "_LastServed"] = ticket;
                }
                else
                {
                    Session[_tname + "_LastServed"] = 0;
                    _isrefresh = true;
                }
            OnAuthorization();

            SessionTracker.AddTrack();

            logger.Debug("Loaded Pre-Init");

        }

        /// <summary>Outputs hidden field with ticket id.</summary>
        /// <param name="output">Html Text Writer</param>
        protected override void Render(HtmlTextWriter output)
        {
            //Checks if the page is using Ajax
            if (ScriptManager.GetCurrent(this) != null)
                ScriptManager.RegisterHiddenField(this, _tname, Session[_tname + "_LastServed"].ToString());
            else if (Session[_tname + "_LastServed"] != null)
                Page.ClientScript.RegisterHiddenField(_tname, Session[_tname + "_LastServed"].ToString());

            base.Render(output);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the value of a property claim from the user's claim collection
        /// </summary>
        /// <param name="propertyUri">Uri of the property claim</param>
        /// <returns>Value of the property claim</returns>
        public string GetClaimPropertyValue(string propertyUri)
        {
            string propertyValue = null;

            var principal = Thread.CurrentPrincipal as IClaimsPrincipal;

            if (principal == null)
            {
                throw new ApplicationException("IClaimsPrincipal is null");
            }
            else if (principal.Identities == null)
            {
                throw new ApplicationException("IClaimsPrincipal.Identities is null");
            }
            else if (principal.Identities.Count == 0)
            {
                throw new ApplicationException("IClaimsPrincipal.Identities count is zero");
            }
            else if (principal.Identities.Count > 1)
            {
                throw new ApplicationException("IClaimsPrincipal.Identities count is greater than one");
            }

            if (string.IsNullOrEmpty(propertyUri))
            {
                throw new ApplicationException("propertyUri cannot be null or empty");
            }

            //now check the claim from the Identity claim collection
            propertyValue = Helper.GetClaimValue(principal.Identities[0].Claims, propertyUri);

            if (string.IsNullOrEmpty(propertyValue) || propertyValue.Equals(Helper.NO_CLAIM_FOUND_MESSAGE))
            {
                propertyValue = null;
            }

            return propertyValue;
        }

        public string AppSettings(string key)
        {
            return System.Configuration.ConfigurationManager.AppSettings[key].ToString();
        }

        public void AuditLogPatientInsert(ePrescribeSvc.AuditAction auditAction, string patientID)
        {
            EPSBroker.AuditLogPatientInsert(
                auditAction,
                SessionLicenseID,
                SessionUserID,
                patientID,
                Request.UserIpAddress(),
                DBID);
        }

        public ePrescribeSvc.AuditLogPatientRxResponse AuditLogPatientRxInsert(ePrescribeSvc.AuditAction auditAction, string patientID, string rxID, string ipAddress, string createdUTC = null)
        {
            return EPSBroker.AuditLogPatientRxInsert(
                auditAction,
                SessionLicenseID,
                SessionUserID,
                patientID,
                ipAddress,
                rxID,
                DBID,
                createdUTC);
        }
        public ePrescribeSvc.AuditLogPatientRxResponse AuditLogPatientRxInsert(ePrescribeSvc.AuditAction auditAction, string patientID, string rxID, string createdUTC = null)
        {
            return EPSBroker.AuditLogPatientRxInsert(
                auditAction,
                SessionLicenseID,
                SessionUserID,
                patientID,
                Request.UserIpAddress(),
                rxID,
                DBID,
                createdUTC);
        }

        public ePrescribeSvc.AuditLogPatientRxCSResponse AuditLogPatientRxInsert(ePrescribeSvc.AuditAction auditAction, string patientID, long auditLogPatientRxID, bool isSuccessful, string createdUTC = null)
        {
            return EPSBroker.AuditLogPatientRxCSInsert(
                auditAction,
                auditLogPatientRxID,
                isSuccessful,
                SessionLicenseID,
                SessionUserID,
                patientID,
                Request.UserIpAddress(),
                DBID,
                createdUTC);
        }

        public void AuditLogUserInsert(ePrescribeSvc.AuditAction auditAction, string userIDActionPerformedOn)
        {
            EPSBroker.AuditLogUserInsert(
                auditAction,
                SessionLicenseID,
                SessionUserID,
                userIDActionPerformedOn,
                Request.UserIpAddress(),
                DBID);
        }

        public void AuditLogLicenseInsert(ePrescribeSvc.AuditAction auditAction, int siteIDActionPerformedOn)
        {
            EPSBroker.AuditLogLicenseInsert(
                auditAction,
                SessionLicenseID,
                SessionUserID,
                siteIDActionPerformedOn,
                Request.UserIpAddress(),
                DBID);
        }

        public void BrowserAuditLogInsert(string screenHeight, string screenWidth, string browserHeight, string browserWidth, string app)
        {
            try
            {
                string sBrowser = Request.Browser.Browser;
                string sType = Request.Browser.Type;
                string sUserAgent = Request.UserAgent;
                string sPlatform = UserAgentParser.GetPlatformFromUserAgent(Request.Browser.Platform, Request.UserAgent);
                var layoutEngine = UserAgentParser.GetLayoutEngineFromUserAgent(Request.UserAgent);
                bool bSupportsJavascript = true;

                //since Google Chrome was built on the same webkit as Safari and since we're still using .net 2.0, we need to fake out Browser setting.
                //if we don't do this, 'AppleMAC-Safari' will show for both Safari and Chrome
                if (sUserAgent.ToLower().Contains("chrome"))
                {
                    sBrowser = "Chrome";
                    sType = "Chrome" + Request.Browser.Version;
                }

                Session["Browser"] = sBrowser;

                Version ecmaVersion = Request.Browser.EcmaScriptVersion;
                bSupportsJavascript = ecmaVersion.Major >= 1;

                var userGuid = GetUserGuidForBrowserLog(PageState);

                Audit.BrowserAuditLogInsert(
                    sBrowser,
                    Request.Browser.Version,
                    Request.Browser.MajorVersion.ToString(),
                    Request.Browser.MinorVersion.ToString(),
                    sType,
                    sPlatform,
                    Request.Browser.IsMobileDevice,
                    Request.Browser.InputType,
                    Request.Browser.Cookies,
                    Request.Browser.Frames,
                    bSupportsJavascript,
                    ((string.IsNullOrEmpty(screenHeight)) ? 0 : int.Parse(screenHeight)),
                    ((string.IsNullOrEmpty(screenWidth)) ? 0 : int.Parse(screenWidth)),
                    ((string.IsNullOrEmpty(browserHeight)) ? 0 : int.Parse(browserHeight)),
                    ((string.IsNullOrEmpty(browserWidth)) ? 0 : int.Parse(browserWidth)),
                    app,
                    layoutEngine,
                    userGuid,
                    Request.UserAgent,
                    DBID);
            }
            catch (Exception ex)
            {
                Audit.AddException(Guid.Empty.ToString(), Guid.Empty.ToString(), "Exception writing to BROWSER_AUDIT_LOG: " + ex.ToString(), string.Empty, string.Empty, string.Empty, DBID);
            }
        }

        internal static string GetUserGuidForBrowserLog(IStateContainer pageState)
        {
            // regular login -AuditLogUserLoginID is populated in session
            var userGuid = pageState.GetStringOrEmpty("AuditLogUserLoginID");
            if (userGuid == string.Empty) // this is SSO login - USERID is populated in session
            {
                userGuid = pageState.GetStringOrEmpty("USERID");
            }
            return userGuid;
        }

        private void OnAuthorization()
        {
            string strCurrentPage = GetCurrentPageName();
            if (Pages.ShouldSkipAuthorization(strCurrentPage))
                return;
            bool isAuthorized = AuthorizationManager.Process(PageState, strCurrentPage);

            if (!isAuthorized)
                Response.Redirect("~/" + Constants.PageNames.EXCEPTION, true);
        }

        private string GetCurrentPageName()
        {
            return Path.GetFileName(HttpContext.Current.Request.Path);
        }

        public void ClearMedicationInfo(bool clearDx)
        {
            AppCode.StateUtils.MedicationInfo.ClearMedInfo(PageState, clearDx);
        }

        public void ClearPatientInfo()
        {
            if (Session["SSOMode"] == null || (Session["SSOMode"] != null && (Session["SSOMode"].ToString() != Constants.SSOMode.PATIENTLOCKDOWNMODE && Session["SSOMode"].ToString() != Constants.SSOMode.EPAPATIENTLOCKDOWNTASKMODE)))
            {
                PatientInfo.ClearPatientInfo(PageState);
            }
        }

        public void RedirectToInterstitialAdIfNeeded(string redirect_url)
        {
            if (Session["SSOMode"] == null)
            {
                if (FeaturedModule.Type != Module.ModuleType.UNKNOWN)
                {
                    if (SessionLicense.DeluxePricingStructure == Constants.DeluxePricingStructure.Basic && (!SessionLicense.DeluxeFeatureStatusDisplay.Contains("Platinum")))// Show Interstetial Ads only for Basic User...
                    {
                        if (!String.IsNullOrWhiteSpace(redirect_url))
                        {
                            Response.Redirect("~/" + Constants.PageNames.INTERSTITIAL_AD + "?TargetUrl=" + redirect_url.Replace("~/", ""));
                        }
                        else
                        {
                            Response.Redirect("~/" + Constants.PageNames.INTERSTITIAL_AD);
                        }
                    }
                }
            }
        }

        public void DefaultRedirect(bool fromInside = true)
        {
            Response.Redirect(RedirectHelper.GetDefaultRedirectUrl(SessionSiteID, new StateContainer(HttpContext.Current.Session),
                (string)Request.QueryString["TargetURL"], fromInside));
        }

        public void DefaultRedirect(string queryString)
        {
            if (queryString == null || queryString.Length == 0)
            {
                DefaultRedirect();
            }
            if (queryString.StartsWith("?"))
            {
                queryString = queryString.Substring(1);
            }

            NameValueCollection qscoll = HttpUtility.ParseQueryString(queryString);
            StringBuilder sb = new StringBuilder();
            SelectPatientComponentParameters selectPatientComponentParameters = new SelectPatientComponentParameters
            {
                PatientId = PageState.GetStringOrEmpty(Constants.SessionVariables.PatientId)
            };
            foreach (var v in qscoll.AllKeys)
            {
                sb.Append(v + "=" + Microsoft.Security.Application.Encoder.UrlEncode(qscoll[v]) + "&");
                if (v.ToUpper().StartsWith("MSG"))
                {
                    selectPatientComponentParameters.MessageText = qscoll[v];
                    selectPatientComponentParameters.MessageIcon = "Success";
                }
            }
            sb = sb.ToString().EndsWith("&") ? sb.Remove((sb.Length - 1), 1) : sb;
            queryString = sb.ToString();

            RedirectToSelectPatient(queryString, selectPatientComponentParameters);
        }

        public void RedirectToSelectPatient(string queryString, SelectPatientComponentParameters selectPatientComponentParameters)
        {
            Response.Redirect(RedirectHelper.GetRedirectToSelectPatientUrl(queryString, selectPatientComponentParameters, PageState));
        }

        /// <summary>Returns true if page has been refreshed, otherwise false.</summary>
        public bool IsRefresh
        {
            get { return _isrefresh; }
        }

        public void ResetModules()
        {
            Session["LICENSE_MODULES_ACTIVE"] = null;
            Session["LICENSE_MODULES_INACTIVE"] = null;
        }

        public void RefreshPatientActiveDiagnosis()
        {
            if (!string.IsNullOrEmpty(SessionPatientID))
            {
                //Retrieve the patient's distinct active diagnosis
                DataSet activeDx = Patient.PatientDiagnosis(SessionPatientID, SessionLicenseID, SessionUserID, DBID);
                if (activeDx.Tables["PatientDiagnosis"].Rows.Count > 0)
                {
                    StringBuilder activeDiagnosis = new StringBuilder();
                    foreach (DataRow dr in activeDx.Tables["PatientDiagnosis"].Select("Active='Y'"))
                    {
                        if (activeDiagnosis.Length > 0)
                            activeDiagnosis.Append(", ");

                        activeDiagnosis.Append(dr["Description"].ToString().Trim());
                    }
                    Session["ACTIVEDX"] = activeDiagnosis.ToString();
                }
                else
                {
                    Session.Remove("ACTIVEDX");
                }
            }
        }

        public void RefreshPatientActiveMeds()
        {
            if (!string.IsNullOrEmpty(SessionPatientID))
            {
                //Retrieve the patient's distinct active medications 
                DataSet activeMeds = Patient.GetPatientActiveMedications(SessionPatientID, SessionLicenseID, SessionUserID, DBID);
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

                    Session["ACTIVEMEDICATIONS"] = activeMedications.ToString();
                    Session["ACTIVEMEDDDILIST"] = activeMedDDIList;
                }
                else
                {
                    Session.Remove("ACTIVEMEDICATIONS");
                    Session.Remove("ACTIVEMEDDDILIST");
                }
            }
        }

        /// <summary>
        /// Modifies a asp.net button to a Single-Click button to prohibit double clicking.
        /// Prevents a user from submitting a page more than once.
        /// </summary>
        /// <param name="button">System.Web.UI.WebControls button to single-click enable</param>
        public void SetSingleClickButton(Button button)
        {
            StringBuilder sb = new StringBuilder();
            //This forces the page validation, if any, to execute.
            sb.Append("if (typeof(Page_ClientValidate) == 'function') { ");
            sb.Append("if (Page_ClientValidate('') == false) { return false; }} ");
            //Changes the text of the button. Gives the user a processing message.
            sb.Append("this.value = 'Processing...';");
            //Prevent the button from being pressed a second time.
            sb.Append("this.disabled = true;");
            //Forces the page to postback.
            sb.Append(ClientScript.GetPostBackEventReference(button, null));
            sb.Append(";");
            button.Attributes.Add("onclick", sb.ToString());
        }

        /// <summary>
        /// Modifies a asp.net button to a Single-Click image button to prohibit double clicking.
        /// Prevents a user from submitting a page more than once.
        /// </summary>
        /// <param name="imageButton">System.Web.UI.WebControls button to single-click enable</param>
        public void SetSingleClickImageButton(ImageButton imageButton)
        {
            StringBuilder sb = new StringBuilder();
            //This forces the page validation, if any, to execute.
            sb.Append("if (typeof(Page_ClientValidate) == 'function') { ");
            sb.Append("if (Page_ClientValidate('') == false) { return false; }} ");
            //Prevent the button from being pressed a second time.
            sb.Append("this.disabled = true;");
            //Forces the page to postback.
            sb.Append(ClientScript.GetPostBackEventReference(imageButton, null));
            sb.Append(";");
            imageButton.Attributes.Add("onclick", sb.ToString());
        }

        public string GetClaimValue(string claimType)
        {
            // Cast the Thread.CurrentPrincipal
            IClaimsPrincipal icp = Thread.CurrentPrincipal as IClaimsPrincipal;

            if (icp == null)
            {
                throw new ApplicationException("Current principal is null.");
            }

            // Access IClaimsIdentity which contains claims
            IClaimsIdentity claimsIdentity = (IClaimsIdentity)icp.Identity;

            string claimValue = null;

            try
            {
                claimValue = (from c in claimsIdentity.Claims
                              where c.ClaimType == claimType
                              select c.Value).Single();
            }
            catch (InvalidOperationException)
            {
                claimValue = "Claim wasn't found or there were more than one claims found";
            }

            return claimValue;
        }

        /// <summary>
        /// Adds a user to the cached dictionary of logged in users. Each active user is represented here with their GUID and associated login audit ID.
        /// </summary>
        /// <param name="userGUID">UserGUID of the logged in user</param>
        /// <param name="auditLogUserLoginID">Unique identifier of the audited login event from the AUDIT_LOG_USER table</param>
        public void AddUserToActiveUserCache(string userGUID, string auditLogUserLoginID)
        {
            Dictionary<string, string> loggedInUsersDictionary = null;

            if (Application["ActiveUsers"] == null)
            {
                loggedInUsersDictionary = new Dictionary<string, string>();

                Application["ActiveUsers"] = loggedInUsersDictionary;
            }
            else
            {
                loggedInUsersDictionary = Application["ActiveUsers"] as Dictionary<string, string>;
            }

            if (loggedInUsersDictionary != null && !loggedInUsersDictionary.ContainsKey(userGUID))
            {
                loggedInUsersDictionary.Add(userGUID, auditLogUserLoginID);
            }
        }

        /// <summary>
        /// Removes a user from the cached dictionary of logged in users.
        /// </summary>
        /// <param name="userGUID">UserGUID of the logged in user</param>
        public void RemoveUserFromActiveUserCache(string userGUID)
        {
            Dictionary<string, string> loggedInUsersDictionary = Application["ActiveUsers"] as Dictionary<string, string>;

            if (loggedInUsersDictionary != null)
            {
                loggedInUsersDictionary.Remove(userGUID);
            }
        }

        public static bool IsAnyTourToBeShown(GetMessageTrackingAcksResponse acks, string newUser, string newRelease,
                        WelcomeTourNewUser showNewUserTour, WelcomeTourNewRelease showNewReleaseTour, out string tourPath, out int tourType)
        {
            bool returnValue = false;
            tourPath = "";
            tourType = (int)Constants.WelcomeTourType.NewUser;
            try
            {
                if (acks != null)
                {
                    var ackList = acks.UserMessageTrackingAckList;
                    if (showNewUserTour == WelcomeTourNewUser.ON &&
                        ackList.FirstOrDefault(l => l.ConfigKey.ToString() == "d7687d09-07ea-458d-8546-97d6c195f89d") == null) //No new user tour ack present 
                    {
                        returnValue = true;
                        tourPath = newUser;
                        tourType = (int)Constants.WelcomeTourType.NewUser;
                    }
                    else if (showNewReleaseTour == WelcomeTourNewRelease.ON &&
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

        public bool checkIfUserRestricted(string patientID, string userID, ConnectionStringPointer dbID)
        {
            DataTable dtPatientPrivacyRequestID = new PatientPrivacy().GetPatientPrivacyRequestID(patientID, userID, dbID);
            if (dtPatientPrivacyRequestID.Rows.Count > 0 && dtPatientPrivacyRequestID.Rows[0]["ID"] != DBNull.Value)
            {
                Int32 PatientPrivacyRequestID = Convert.ToInt32(dtPatientPrivacyRequestID.Rows[0]["ID"].ToString());
                Session["PatientPrivacyRequestID"] = PatientPrivacyRequestID.ToString();
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        public string getModuleSubscribed(string productName, ConnectionStringPointer dbID)
        {
            DataTable dtDeluxeNoteAndDescription = DeluxePurchaseManager.GetDeluxeNoteAndDescription(null, productName, dbID);
            string headerText = dtDeluxeNoteAndDescription.Rows[0]["DetailDesc"].ToString();
            return headerText;
        }

        public string GetDEANumberToBeUsed(IStateContainer state)
        {
            return UserInfo.GetDEANumberToBeUsed(state);
        }
        public void ResetWorkflows()
        {
            MasterPage.RxTask = null;
            PageState[Constants.SessionVariables.TaskScriptMessageId] = null;
            PageState.Remove(Constants.SessionVariables.DURRefillDS);
            PageState.Remove("CURRENT_DUR_WARNINGS");
        }

        /// <summary>
        /// Q: why this method is here
        /// A: it's called only from SSO. In case of production we use SSL offloading, which casese the requests
        /// to be redirected to non-HTTP address, and therefore users getting '301' error.
        /// Note that we cannot repro this in QA/INT due to lack of SSL offloading.
        /// </summary>
        /// <returns></returns>
        public static string GetHttpsSiteUrl()
        {
            string url;
            if (HttpContext.Current.Request.ApplicationPath == "/")
            {
                string servname = HttpContext.Current.Request.ServerVariables["SERVER_NAME"];
                url = "https://" + servname;
            }
            else
            {
                url = HttpContext.Current.Request.Url.AbsoluteUri;
                int end = url.IndexOf(HttpContext.Current.Request.ApplicationPath) +
                      HttpContext.Current.Request.ApplicationPath.Length;
                url = url.Substring(0, end);
            }
            return url;
        }

        public static string GetSiteUrl()
        {
            string url;
            if (HttpContext.Current.Request.ApplicationPath == "/")
            {
                string servname = HttpContext.Current.Request.ServerVariables["SERVER_NAME"];

                if (HttpContext.Current.Request.ServerVariables["HTTPS"] == "off")
                {
                    url = "http://" + servname;
                }
                else
                {
                    url = "https://" + servname;
                }
            }
            else
            {
                url = HttpContext.Current.Request.Url.AbsoluteUri;
                int end = url.IndexOf(HttpContext.Current.Request.ApplicationPath) +
                          HttpContext.Current.Request.ApplicationPath.Length;
                url = url.Substring(0, end);
            }
            return url;
        }

        string IBasePage.GetSiteUrl()
        {
            return GetSiteUrl();
        }

    }
}