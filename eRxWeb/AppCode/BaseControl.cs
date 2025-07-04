using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Text;
using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.Objects;
using Allscripts.ePrescribe.Shared.Logging;
using Allscripts.Impact;
using Allscripts.Impact.ePrescribeSvc;
using eRxWeb.Erx_Response;
using eRxWeb.State;
using Allscripts.Impact.Utilities;
using Patient = Allscripts.Impact.Patient;
using Rx = Allscripts.Impact.Rx;
using RxUser = Allscripts.Impact.RxUser;
using eRxWeb.AppCode;
using eRxWeb.AppCode.StateUtils;
using ConnectionStringPointer = Allscripts.ePrescribe.DatabaseSelector.ConnectionStringPointer;
using Weight = Allscripts.ePrescribe.Objects.Weight;

namespace eRxWeb
{
    /// <summary>
    /// Summary description for BaseControl
    /// </summary>
    public class BaseControl : System.Web.UI.UserControl
    {
        #region Fields

        ConnectionStringPointer _dbID;

        public IStateContainer ControlState { get; set; }

        public IeRxHttpResponse ERxResponse { get; set; }

        #endregion

        #region Constructors

        public BaseControl()
        {
            this.Load += new EventHandler(BasePage_PreInit);
        }

        void BasePage_PreInit(object sender, EventArgs e)
        {
            ControlState = new StateContainer(Session);
            ERxResponse = new eRxHttpResponse(Response);
        }
        #endregion

        #region Properties


        public PhysicianMasterPage MasterPage => (PhysicianMasterPage)Page.Master;

        public DURSettings DURSettings => ControlState.Cast(Constants.SessionVariables.DURSettings, new DURSettings());

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
                        dmSvc.Url = System.Configuration.ConfigurationManager.AppSettings["DataManagerSvc.DataManagerSvc"].ToString();

                        _dbID = (ConnectionStringPointer)dmSvc.GetDatabasePointer(entityID);
                    }
                    else if (Session["LicenseID"] != null)
                    {
                        DataManagerSvc.EntityIdentifier entityID = new DataManagerSvc.EntityIdentifier();
                        entityID.Type = DataManagerSvc.IdentifierType.UserGUID;
                        entityID.Value = Session["UserID"].ToString();

                        DataManagerSvc.DataManagerSvc dmSvc = new DataManagerSvc.DataManagerSvc();
                        dmSvc.Url = System.Configuration.ConfigurationManager.AppSettings["DataManagerSvc.DataManagerSvc"].ToString();

                        _dbID = (ConnectionStringPointer)dmSvc.GetDatabasePointer(entityID);
                    }
                }

                return _dbID;
            }
        }

        /// <summary>
        /// Returns the first rx in the RxList Session variable
        /// </summary>
        public Rx CurrentRx
        {
            get
            {
                Rx rx = new Rx();

                if (Session["RxList"] != null)
                {
                    ArrayList rxList = (ArrayList)Session["RxList"];
                    rx = (Rx)rxList[0];
                }

                return rx;
            }
        }

        public RxUser DelegateProvider
        {
            get
            {
                if (Session["DelegateProviderID"] != null)
                {
                    return new RxUser(Session["DelegateProviderID"].ToString(), DBID);
                }
                else
                {
                    return null;
                }
            }
        }

        public string DelegateProviderName
        {
            get
            {
                string providerName = string.Empty;
                RxUser provider = DelegateProvider;
                if (provider != null)
                {
                    providerName = provider.GetAttribute("Title") + " " + provider.GetAttribute("FirstName") + " " + provider.GetAttribute("LastName");
                }

                return providerName;
            }
        }

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
                        if (module.Advertise)
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

        public string SessionPartnerID
        {
            get
            {
                if (Session["PartnerID"] == null)
                {
                    return string.Empty;
                }

                return Session["PartnerID"].ToString();
            }
        }

        /// <summary>
        /// Shield's External AppInstanceID as specified by ePrescribe to Shield on user creation
        /// </summary>
        public string EprescribeExternalAppInstanceID
        {
            get { return ShieldSettings.ePrescribeExternalAppInstanceId(new StateContainer(Session)); }
        }

        public string ShieldExternalTenantID
        {
            get
            {
                if (!ControlState.ContainsKey("ShieldExternalTenantID"))
                {
                    ControlState["ShieldExternalTenantID"] = EPSBroker.GetShieldExternalTenantID(Session["AHSAccountID"].ToString());
                }

                return ControlState.GetStringOrEmpty("ShieldExternalTenantID");
            }
        }

        public string SessionUserID
        {
            get
            {
                if (Session["USERID"] == null)
                {
                    return null;
                }

                return Session["USERID"].ToString();
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

        public Constants.UserCategory SessionUserType
        {
            get
            {
                Constants.UserCategory ret = Constants.UserCategory.GENERAL_USER;
                if (Session["UserType"] != null)
                {
                    ret = (Constants.UserCategory)Session["UserType"];
                }

                return ret;
            }
        }

        public string SessionLicenseID
        {
            get
            {
                if (Session["LICENSEID"] == null)
                    return null;

                return Session["LICENSEID"].ToString();
            }
        }

        public int SessionSiteID
        {
            get
            {
                int temp = -1;
                if (Session["SiteID"] == null)
                    return temp;

                if (!int.TryParse(Session["SITEID"].ToString(), out temp))
                    temp = -1;

                return temp;
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

        public string SessionPatientID
        {
            get
            {
                if (Session["PATIENTID"] == null)
                    return null;

                return Session["PATIENTID"].ToString();
            }
        }

        public string SessionPracticeState
        {
            get
            {
                if (Session["PRACTICESTATE"] == null)
                    return null;

                return Session["PRACTICESTATE"].ToString();
            }
        }

        public int SessionStanding
        {
            get
            {
                return Convert.ToInt32(Session["STANDING"]);
            }
        }

        public ApplicationLicense SessionLicense
        {
            get
            {
                if (Session["SessionLicense"] == null)
                {
                    Session["SessionLicense"] = new ApplicationLicense(string.Empty, 0, string.Empty, DBID);
                }

                return (ApplicationLicense)Session["SessionLicense"];
            }
        }
        public string ShieldSecurityToken
        {
            get
            {
                string retVal = null;
                if (System.Configuration.ConfigurationManager.AppSettings["SAMLTokenStorageLocation"].ToString().Equals("Session"))
                {
                    if (Session["ShieldSecurityToken"] == null)
                        retVal = null;

                    retVal = Session["ShieldSecurityToken"].ToString();
                }

                /*
                  This is no longer implemented as WIF has been removed
			 
                else
                {
                    IClaimsPrincipal currentPrincipal = Page.User as IClaimsPrincipal;

                    if (currentPrincipal == null)
                        retVal = null;

                    if (currentPrincipal.Identities[0] == null)
                        retVal = null;

                    SecurityToken bootstrapToken = currentPrincipal.Identities[0].BootstrapToken;

                    StringBuilder sb = new StringBuilder();

                    XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
                    xmlWriterSettings.OmitXmlDeclaration = true;

                    using (var writer = XmlWriter.Create(sb, xmlWriterSettings))
                    {
                        new Saml11SecurityTokenHandler(new SamlSecurityTokenRequirement()).WriteToken(writer, bootstrapToken);
                    }

                    retVal = sb.ToString();
                }
                 * */

                return retVal;
            }
        }

        public List<Rx> ScriptPadMeds
        {
            get
            {
                List<Rx> rxList = new List<Rx>();
                string userID = string.Empty;
                userID = SessionUserID;

                if (Session["CURRENT_SCRIPT_PAD_MEDS"] == null)
                {
                    DataTable dt = CHPatient.GetScriptPad(SessionPatientID, SessionLicenseID, userID, SessionPracticeState, DBID);

                    foreach (DataRow dr in dt.Rows)
                    {
                        Rx rx = new Rx(dr);
                        rxList.Add(rx);
                    }

                    Session["CURRENT_SCRIPT_PAD_MEDS"] = rxList;
                }
                else
                {
                    rxList = Session["CURRENT_SCRIPT_PAD_MEDS"] as List<Rx>;
                }

                return rxList;
            }
            set
            {
                Session["CURRENT_SCRIPT_PAD_MEDS"] = value;
            }
        }

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
        /// Check show Rx Info at license level, if exists,  return true or else false.
        /// </summary>
        public bool IsShowRxInfo
        {
            get
            {
                if ((SessionLicense.EnterpriseClient.ShowRxInfo) && (Session["SHOW_RX_INFO"] != null))
                {
                    return Convert.ToBoolean(Session["SHOW_RX_INFO"]);
                }
                else
                {
                    return false;
                }
            }
        }
        /// <summary>
        /// Check show Rx Info at license level, if exists,  return true or else false.
        /// </summary>
        public bool CanApplyFinancialOffers
        {
            get
            {
                return Session["APPLYFINANCIALOFFERS"].RetrieveBooleanValue();
            }
        }

        /// <summary>
        /// Check allow patient receipts
        /// </summary>
        public bool AllowPatientReceipts
        {
            get
            {
                return Session["ALLOWPATIENTRECEIPT"].RetrieveBooleanValue();
            }
        }
        public string ClinicalViewerPostData
        {
            get
            {
                string postData = string.Empty;

                if (Session["ClinicalViewerPostData"] == null)
                {
                    if (Session["PATIENTID"] != null)
                    {
                        postData = EPSBroker.GetClinicalViewerSSOLink(
                                SessionPatientID,
                                SessionLicense.EnterpriseClient.SystemOID,
                                Convert.ToString(Session["UserFirstName"]),
                                Convert.ToString(Session["UserLastName"]),
                                SessionUserName,
                                SessionUserType.ToString(), //"Nurse"
                                SessionLicense.EnterpriseClient.CommunityURL
                            );

                        Session["ClinicalViewerPostData"] = postData;
                    }
                }
                else
                {
                    postData = Session["ClinicalViewerPostData"].ToString();
                }

                return postData;
            }
            set
            {
                Session["ClinicalViewerPostData"] = value;
            }
        }

        public bool IsLicenseShieldEnabled
        {
            get
            {
                if (Session["IsLicenseShieldEnabled"] == null)
                    return false;
                return Convert.ToBoolean(Session["IsLicenseShieldEnabled"]);
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

        public bool IsAdminUser
        {
            get
            {
                bool isAdminUser = false;

                if (Session["IsAdmin"] != null)
                {
                    bool.TryParse(Session["IsAdmin"].ToString(), out isAdminUser);
                }

                return isAdminUser;
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

                    if (ControlState.GetBooleanOrFalse("ShowLexicompDefault"))
                    {
                        if (SessionLicense.EnterpriseClient.EnableLexicompDefault)
                            isEnabled = true;
                    }
                    else
                    {
                        if (ControlState.GetBooleanOrFalse(Constants.LicensePreferences.SHOW_LEXICOMP))
                        {
                            isEnabled = true;
                        }
                    }
                }
                return isEnabled;

            }
        }

        public string ServiceAccountAuthToken
        {
            get
            {
                if (Session[Constants.SessionVariables.ShieldSendRxAuthToken] != null && DateTime.UtcNow < (DateTime)Session[Constants.SessionVariables.RefreshSendRxAuthTokenDateTimeUTC])
                    return Session[Constants.SessionVariables.ShieldSendRxAuthToken].ToString();

                var AuthToken = EPSBroker.RequestShieldAuthToken(Constants.ShieldApplicationRelyingPartyUri.PrescriptionSend, SessionLicenseID, SessionUserID, DBID);
                if (AuthToken != null)
                {
                    Session[Constants.SessionVariables.ShieldSendRxAuthToken] = AuthToken.samlString;
                    Session[Constants.SessionVariables.RefreshSendRxAuthTokenDateTimeUTC] = AuthToken.validTo;
                }

                return (AuthToken != null) ? AuthToken.ToString() : string.Empty;

            }

        }


        #endregion

        #region Custom Methods

        public void DefaultRedirect(bool fromInside = true)
        {
            if (SessionSiteID < 0)
            {
                Response.Redirect("~/" + Constants.PageNames.SELECT_ACCOUNT_AND_SITE);
            }

            bool ssoTaskMode = Session["SSOMode"] != null && Session["SSOMode"].ToString() == Constants.SSOMode.TASKMODE;
            bool epaTaskMode = Session["SSOMode"] != null && Session["SSOMode"].ToString() == Constants.SSOMode.EPAPATIENTLOCKDOWNTASKMODE;
            ControlState[Constants.SessionVariables.AppComponentAlreadyInitialized] = true;
            if (ssoTaskMode || epaTaskMode)
                Response.Redirect(RedirectHelper.GetRedirectToSPAUrl(Constants.PageNames.DOC_REFILL_MENU, fromInside));
            else
                Response.Redirect(AngularStringUtil.CreateInitComponentUrl(Constants.PageNames.SELECT_PATIENT));//Empty string Loads Angular Default Page instead of forcing legacy default ASPX page
        }

        public void ClearPatientInfo()
        {
            PatientInfo.ClearPatientInfo(ControlState);
        }

        public void SetPatientInfo(string patientID)
        {
            ClearPatientInfo();
            Session["IsRemovedCSMedsFromScriptPad"] = "N";
            Session["PATIENTID"] = patientID;
            Patient pat = new Patient(patientID, true, false, true, Session["LICENSEID"].ToString(), Session["USERID"].ToString(), this.DBID);
            DataSet patds = pat.DS;
            string patientSelectedMOB = null;
            if (patds != null && patds.Tables["Patient"] != null && patds.Tables["Patient"].Rows.Count > 0)
            {
                string middleName = Convert.ToString(patds.Tables["Patient"].Rows[0]["MiddleName"]);

                Session["PATIENTNAME"] =
                   StringUtil.formatDisplayName(
                            patds.Tables["Patient"].Rows[0]["LastName"].ToString(),
                            patds.Tables["Patient"].Rows[0]["FirstName"].ToString(),
                            middleName);
                Session["PATIENTFIRSTNAME"] = patds.Tables["Patient"].Rows[0]["FirstName"].ToString();
                Session["PATIENTLASTNAME"] = patds.Tables["Patient"].Rows[0]["LastName"].ToString();
                ControlState["PATIENTMIDDLENAME"] = middleName;

                Session["SEX"] = patds.Tables["Patient"].Rows[0]["SEX"].ToString();
                Session["PATIENTDOB"] = patds.Tables["Patient"].Rows[0]["DOB"].ToString();
                Session["PATIENTZIP"] = patds.Tables["Patient"].Rows[0]["Zip"].ToString();
                Session["PATIENTMRN"] = patds.Tables["Patient"].Rows[0]["ChartID"].ToString();
                Session["PATIENTNKA"] = patds.Tables["Patient"].Rows[0]["NoKnownAllergies"].ToString();
                ControlState[Constants.SessionVariables.PatientWeight] = new Weight(Convert.ToString(patds.Tables["Patient"].Rows[0]["WeightKg"]));

                int patNoActiveMedication = 0;
                if (Int32.TryParse(patds.Tables["Patient"].Rows[0]["NoActiveMedication"].ToString(), out patNoActiveMedication))
                {
                    Session["PATIENTNoActiveMed"] = patNoActiveMedication == 1 ? true : false;
                }
                else
                {
                    Session["PATIENTNoActiveMed"] = false;
                }


                Session["PATIENT_YEARS_OLD"] = pat.YearsOld;
                Session["PATIENT_AGE_CATEGORY"] = Convert.ToInt32(pat.AgeCategory);


                Session["PATIENTSTATUS"] = patds.Tables["Patient"].Rows[0]["statusID"].ToString();
                if (patds.Tables["Patient"].Rows[0]["email"] != DBNull.Value)
                {
                    Session["PATIENTEMAIL"] = patds.Tables["Patient"].Rows[0]["email"].ToString();
                }
                string LastPhamarcyId = patds.Tables["Patient"].Rows[0]["LASTPHARMACYID"].ToString();
                if (LastPhamarcyId != null && LastPhamarcyId != "" && LastPhamarcyId != "00000000-0000-0000-0000-000000000000")
                {
                    Session["LASTPHARMACYID"] = patds.Tables["Patient"].Rows[0]["LASTPHARMACYID"].ToString();
                    Session["LASTPHARMACYNAME"] = patds.Tables["Patient"].Rows[0]["LASTPHARMACYNAME"].ToString();

                    if (patds.Tables["Patient"].Rows[0]["PercScriptSuccess"] != DBNull.Value)
                        Session["PharmacySuccessRate"] = patds.Tables["Patient"].Rows[0]["PercScriptSuccess"].ToString();
                }
                else
                {
                    Session.Remove("LASTPHARMACYID");
                    Session.Remove("LASTPHARMACYNAME");
                    Session.Remove("PharmacySuccessRate");
                }

                //check to see if the patient has manually selected an MOB pharmacy outside of the elig response workflow
                patientSelectedMOB = patds.Tables["Patient"].Rows[0]["MOB_NABP"].ToString();
                if (!string.IsNullOrEmpty(patientSelectedMOB))
                {
                    Session["MOB_NABP"] = patientSelectedMOB;
                    Session["PatientHasMOBCoverage"] = "Y";
                    Session["MOB_PHARMACY_ID"] = patds.Tables["Patient"].Rows[0]["MOB_PharmacyID"].ToString();
                    Session["MOB_Name"] = patds.Tables["Patient"].Rows[0]["MOB_PharmacyName"].ToString().Trim();
                    Session["MOB_IsElectronicEnabled"] = bool.Parse(patds.Tables["Patient"].Rows[0]["MOB_IsElectronicEnabled"].ToString());

                    if (patds.Tables["Patient"].Rows[0]["MOB_PercScriptSuccess"] != DBNull.Value && patds.Tables["Patient"].Rows[0]["MOB_PercScriptSuccess"].ToString().Trim() != "")
                    {
                        Session["MOB_PercScriptSuccess"] = patds.Tables["Patient"].Rows[0]["MOB_PercScriptSuccess"].ToString().Trim();
                    }
                }

                Session["PatientDemographicAlert"] = patds.Tables["Patient"].Rows[0]["DemographicWarning"].ToString();
                Session["PatientSwitchMessageID"] =
                    patds.Tables["Patient"].Rows[0]["SwitchMessageID"].ToString() != "" ?
                        patds.Tables["Patient"].Rows[0]["SwitchMessageID"].ToString() : "0";

            }
            else
            {
                Session.Remove("PATIENTID");
                Session.Remove("SEX");
                Session.Remove("PATIENTDOB");
                Session.Remove("PATIENT_YEARS_OLD");
                Session.Remove("PATIENT_AGE_CATEGORY");
                Session.Remove("PATIENTZIP");
                Session.Remove("PATIENTMRN");
                Session.Remove("PATIENTNKA");
                Session.Remove("LASTPHARMACYID");
                Session.Remove("LASTPHARMACYNAME");
                Session.Remove("PharmacySuccessRate");
                Session.Remove("PatientDemographicAlert");
                Session.Remove("PatientSwitchMessageID");
                Session.Remove("PATIENTNoActiveMed");
            }
            if (patds.Tables["Plan"] != null && patds.Tables["Plan"].Rows.Count > 0)
            {
                Session["INSURANCE"] = patds.Tables["Plan"].Rows[0]["Name"].ToString();
                Session["CoverageID"] = patds.Tables["Plan"].Rows[0]["CoverageID"].ToString();
                Session["CopayID"] = patds.Tables["Plan"].Rows[0]["CopayID"].ToString();

                string planMOB = patds.Tables["Plan"].Rows[0]["MOB_NABP"].ToString();
                Session["SelectedCoverageID"] = patds.Tables["Plan"].Rows[0]["PatientCoverageID"].ToString();
            }
            else
            {
                Session.Remove("INSURANCE");
                Session.Remove("CoverageID");
                Session.Remove("CopayID");

                if (string.IsNullOrEmpty(patientSelectedMOB))
                {
                    Session.Remove("MOB_NABP");
                    Session.Remove("MOB_Name");
                    Session.Remove("MOB_PercScriptSuccess");
                    Session.Remove("MOB_PHARMACY_ID");
                    Session.Remove("PatientHasMOBCoverage");
                }

                Session.Remove("SelectedCoverageID");
            }
            if (patds.Tables["Formulary"] != null && patds.Tables["Formulary"].Rows.Count > 0)
            {
                Session["PlanID"] = patds.Tables["Formulary"].Rows[0]["PlanID"].ToString();
                Session["AltPlanID"] = patds.Tables["Formulary"].Rows[0]["AltPlanID"].ToString();
                Session["FormularyID"] = patds.Tables["Formulary"].Rows[0]["FormularyID"].ToString();
                Session["FormularyActive"] = patds.Tables["Formulary"].Rows[0]["FormularyActive"].ToString();
                Session["OTCCoverage"] = patds.Tables["Formulary"].Rows[0]["OTCCoverage"].ToString();
                Session["GenericDrugPolicy"] = patds.Tables["Formulary"].Rows[0]["GenericDrugPolicy"].ToString();
                Session["UnListedDrugPolicy"] = patds.Tables["Formulary"].Rows[0]["UnlistedDrugPolicy"].ToString();

            }
            else
            {
                Session.Remove("PlanID");
                Session.Remove("AltPlanID");
                Session.Remove("FormularyID");
                Session.Remove("FormularyActive");
                Session.Remove("OTCCoverage");
                Session.Remove("GenericDrugPolicy");
                Session.Remove("UnListedDrugPolicy");
            }
            if (patds != null && patds.Tables["PatientAllergy"] != null && patds.Tables["PatientAllergy"].Rows.Count > 0)
            {
                DataRow[] PatAllergy = patds.Tables["PatientAllergy"].Select("Active='Y'");
                string allergy = "";
                foreach (DataRow dr in PatAllergy)
                {
                    allergy = allergy + dr["AllergyName"].ToString() + ",";
                }
                if (allergy.EndsWith(","))
                    allergy = allergy.Substring(0, allergy.Length - 1);
                Session["ALLERGY"] = allergy;
            }
            else
                Session.Remove("ALLERGY");

            //Retrieve the patient's distinct active medications 
            DataSet activeMeds = Patient.GetPatientActiveMedications(patientID, Session["LICENSEID"].ToString(), Session["USERID"].ToString(), DBID);
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

            //Retrieve the patient's distinct active diagnosis
            DataSet activeDx = Patient.PatientDiagnosis(patientID, Session["LICENSEID"].ToString(), Session["USERID"].ToString(), DBID);
            if (activeDx.Tables["PatientDiagnosis"].Rows.Count > 0)
            {
                StringBuilder activeDiagnosis = new StringBuilder();
                foreach (DataRow dr in activeDx.Tables["PatientDiagnosis"].Select("Active='Y'"))
                {
                    if (activeDiagnosis.Length > 0)
                        activeDiagnosis.Append(", ");

                    activeDiagnosis.Append(dr["ICD_9_CM_DESC"].ToString().Trim());
                }
                Session["ACTIVEDX"] = activeDiagnosis.ToString();
            }
            else
            {
                Session.Remove("ACTIVEDX");
            }

            patds.Clear();
            patds.AcceptChanges();
            patds.Dispose();
            patds = null;
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

                        activeDiagnosis.Append(dr["ICD_9_CM_DESC"].ToString().Trim());
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

        public void ResetModules()
        {
            Session["LICENSE_MODULES_ACTIVE"] = null;
            Session["LICENSE_MODULES_INACTIVE"] = null;
        }

        public ePrescribeSvc.AuditLogPatientRxResponse AuditLogPatientRxInsert(ePrescribeSvc.AuditAction auditAction, string patientID, string rxID, string createdUTC)
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

        public ePrescribeSvc.AuditLogPatientRxCSResponse AuditLogPatientRxCSInsert(ePrescribeSvc.AuditAction auditAction, string patientID, long auditLogPatientRxID, bool isSuccessful, string createdUTC)
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

        #endregion

    }

}
