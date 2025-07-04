using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Allscripts.Impact;
using System.Text;
using Allscripts.ePrescribe.DatabaseSelector;
using Microsoft.IdentityModel.Claims;
using TieServiceClient;
using System.Collections.Specialized;
using Allscripts.ePrescribe.ExtensionMethods;
using Allscripts.ePrescribe.Medispan.Clinical.Model;
using Allscripts.ePrescribe.Objects;
using eRxWeb.AppCode.StateUtils;
using eRxWeb.State;
using Constants = Allscripts.ePrescribe.Common.Constants;
using eRxWeb.AppCode.PptPlusBPL;
using Patient = Allscripts.Impact.Patient;
using Rx = Allscripts.Impact.Rx;

namespace eRxWeb
{
    /// <summary>
    /// Summary description for BaseMasterPage
    /// </summary>
    public class BaseMasterPage : System.Web.UI.MasterPage
    {
        ConnectionStringPointer _dbID;

        public IStateContainer PageState { get; set; }

        /// <summary>
        /// Holds the Header, Footer and Ad content. 
        /// </summary>
        public IPlacementResponse PlacementResponse { get; set; }

        public BaseMasterPage()
        {            
        }

        protected void Page_Init()
        {
            PageState = new StateContainer(Session);
        }
        public void ClearPatientInfo()
        {
            PatientInfo.ClearPatientInfo(PageState);
        }

        public void SetPatientInfo(string patientID)
        {
            PatientInfo.SetPatientInfo(patientID, PageState, DBID);
        }
                
        protected List<Allscripts.ePrescribe.Data.PatientDiagnosis> GetActivePatientDiagnosisList()
        {
            Allscripts.ePrescribe.Data.IPatientDiagnosisProvider diagnosis=  new Allscripts.ePrescribe.Data.PatientDiagnosisProvider();
            return diagnosis.GetActiveDiagnosis(SessionPatientID, DBID);

        }

        public void UpdatePatientActiveMeds()
        {
            if (PageState["PATIENTID"] != null)
            {
                var activeMeds = Patient.GetActiveMedicationsByPatientId(PageState.GetGuidOr0x0("PATIENTID"), PageState.GetGuidOr0x0("LICENSEID"), PageState.GetGuidOr0x0("USERID"), DBID);

                if (activeMeds != null && activeMeds.ActiveMedDDIList?.Count > 0)
                {
                    PageState["ACTIVEMEDICATIONS"] = activeMeds.ActiveMedicationsString;
                    PageState["ACTIVEMEDDDILIST"] = activeMeds.ActiveMedDDIList;
                    PageState[Constants.SessionVariables.ActiveMedicationsTable] = activeMeds.ActiveMedsTable;
                }
                else
                {
                    PageState.Remove("ACTIVEMEDICATIONS");
                    PageState.Remove("ACTIVEMEDDDILIST");
                    PageState.Remove(Constants.SessionVariables.ActiveMedicationsTable);
                }
            }
        }
        
        public string SessionLicenseID
        {
            get
            {
                if (PageState["LICENSEID"] == null)
                    return null;

                return PageState["LICENSEID"].ToString();
            }
        }

        public ApplicationLicense SessionLicense
        {
            get
            {
                if (PageState["SessionLicense"] == null)
                {
                    if (PageState["DBID"] == null)
                        PageState["DBID"] = DBID;

                    PageState["SessionLicense"] = new ApplicationLicense(string.Empty, 0, string.Empty, (ConnectionStringPointer)PageState["DBID"]);
                }

                return (ApplicationLicense)PageState["SessionLicense"];
            }
        }

        public ConnectionStringPointer DBID
        {
            get
            {
                _dbID = ConnectionStringPointer.ERXDB_DEFAULT;

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
                        dmSvc.Url = System.Configuration.ConfigurationManager.AppSettings["DataManagerSvc.DataManagerSvc"].ToString();

                        _dbID = (ConnectionStringPointer)dmSvc.GetDatabasePointer(entityID);
                    }
                    else if (PageState["LicenseID"] != null)
                    {
                        DataManagerSvc.EntityIdentifier entityID = new DataManagerSvc.EntityIdentifier();
                        entityID.Type = DataManagerSvc.IdentifierType.UserGUID;
                        entityID.Value = PageState["UserID"].ToString();

                        DataManagerSvc.DataManagerSvc dmSvc = new DataManagerSvc.DataManagerSvc();
                        dmSvc.Url = System.Configuration.ConfigurationManager.AppSettings["DataManagerSvc.DataManagerSvc"].ToString();

                        _dbID = (ConnectionStringPointer)dmSvc.GetDatabasePointer(entityID);
                    }
                }

                return _dbID;
            }
        }

        public string SessionUserID
        {
            get
            {
                if (PageState["USERID"] == null)
                    return null;

                return PageState["USERID"].ToString();
            }
        }

        public string SessionPatientID
        {
            get
            {
                if (PageState["PATIENTID"] == null)
                    return null;

                return PageState["PATIENTID"].ToString();
            }
        }

        public string SessionPracticeState
        {
            get
            {
                if (PageState["PRACTICESTATE"] == null)
                    return null;

                return PageState["PRACTICESTATE"].ToString();
            }
        }

        public List<Rx> ScriptPadMeds
        {
            get
            {
                List<Rx> rxList = new List<Rx>();
                string userID = string.Empty;
                userID = SessionUserID;

                if (PageState["CURRENT_SCRIPT_PAD_MEDS"] == null)
                {
                    DataTable dt = CHPatient.GetScriptPad(SessionPatientID, SessionLicenseID, userID, SessionPracticeState, DBID);

                    foreach (DataRow dr in dt.Rows)
                    {
                        Rx rx = new Rx(dr);
                        rxList.Add(rx);
                    }

                    PageState["CURRENT_SCRIPT_PAD_MEDS"] = rxList;
                }
                else
                {
                    rxList = PageState["CURRENT_SCRIPT_PAD_MEDS"] as List<Rx>;
                }

                return rxList;
            }
            set
            {
                PageState["CURRENT_SCRIPT_PAD_MEDS"] = value;
            }
        }

        public string SessionUserName
        {
            get
            {
                if (PageState["UserName"] != null)
                {
                    return PageState["UserName"].ToString();
                }

                return string.Empty;
            }
        }

        public Constants.UserCategory SessionUserType
        {
            get
            {
                Constants.UserCategory ret = Constants.UserCategory.GENERAL_USER;
                if (PageState["UserType"] != null)
                {
                    ret = (Constants.UserCategory)PageState["UserType"];
                }

                return ret;
            }
        }

        public string SessionBrowser
        {
            get
            {
                return PageState["Browser"] != null ? PageState["Browser"].ToString() : string.Empty;
            }
        }

        public bool IsIEBrowser
        {
            get
            {
                return SessionBrowser.Contains("IE");
            }
        }

        public bool IsFirefoxBrowser
        {
            get
            {
                return SessionBrowser.Contains("Firefox");
            }
        }

        public bool IsOperaBrowser
        {
            get
            {
                return SessionBrowser.Contains("Opera");
            }
        }

        public bool IsChromeBrowser
        {
            get
            {
                return SessionBrowser.Contains("Chrome");
            }
        }

        public string SessionAppVersion
        {
            get
            {
                string ret = string.Empty;
                if (PageState["EPRESCRIBE_APP_VERSION"] != null)
                {
                    ret = PageState["EPRESCRIBE_APP_VERSION"].ToString();
                }
                else
                {
                    try
                    {
                        ePrescribeSvc.ePrescribeSvc eps = new ePrescribeSvc.ePrescribeSvc();
                        eps.Url = ConfigurationManager.AppSettings["ePrescribeSvc.eprescribesvc"].ToString();
                        ePrescribeSvc.ApplicationVersionRequest appVerReq = new ePrescribeSvc.ApplicationVersionRequest();
                        appVerReq.ApplicationID = ePrescribeSvc.ePrescribeApplication.MainApplication;
                        PageState["EPRESCRIBE_APP_VERSION"] = eps.GetFullAppversion(appVerReq);

                        ret = PageState["EPRESCRIBE_APP_VERSION"].ToString();
                    }
                    catch (Exception)
                    {
                        ret = string.Empty;
                    }
                }
                return ret;
            }
        }

        public bool IsLicenseShieldEnabled
        {
            get
            {
                if (PageState["IsLicenseShieldEnabled"] == null)
                    return false;
                return Convert.ToBoolean(PageState["IsLicenseShieldEnabled"]);
            }
        }

        public bool IsEntClientEPCSEnabled
        {
            get
            {
                bool isEntClientEPCSEnabled = false;

                if (PageState["ISENTCLIENTEPCSENABLED"] != null && PageState["ISENTCLIENTEPCSENABLED"].ToString().Equals(true.ToString()))
                {
                    isEntClientEPCSEnabled = true;
                }

                return isEntClientEPCSEnabled;
            }
        }

        public bool IsPharmacyEPCSEnabled
        {
            get
            {
                bool isPharmacyEPCSEnabled = false;

                if (PageState["ISPHARMACYEPCSENABLED"] != null)
                {
                    isPharmacyEPCSEnabled = (bool)PageState["ISPHARMACYEPCSENABLED"];
                }

                return isPharmacyEPCSEnabled;
            }
        }

        public bool IsMOPharmacyEPCSEnabled
        {
            get
            {
                bool isEPCSEnabled = false;

                if (PageState["MOB_ISEPCSENABLED"] != null)
                {
                    isEPCSEnabled = (bool)PageState["MOB_ISEPCSENABLED"];
                }

                return isEPCSEnabled;
            }
        }

        public bool IsSSOUser
        {
            get
            {
                bool isSSOUser = false;

                if (PageState["IsSSOUser"] != null)
                {
                    isSSOUser = Convert.ToBoolean(PageState["IsSSOUser"]);
                }

                return isSSOUser;
            }
        }

        /// <summary>
        /// Checks the license's LogRx setting
        /// </summary>
        public bool IsLicenseLogRxEnabled
        {
            get
            {
                bool isLicenseLogRxEnabled = false;

                if (SessionLicense.DeluxePricingStructure == Constants.DeluxePricingStructure.Basic ||
                    SessionLicense.DeluxePricingStructure == Constants.DeluxePricingStructure.DeluxeEpcsLogRx ||
                    SessionLicense.DeluxePricingStructure == Constants.DeluxePricingStructure.DeluxeEpaLogRx ||
                    SessionLicense.DeluxePricingStructure == Constants.DeluxePricingStructure.LegacyDeluxeEpaLogRx ||
                    SessionLicense.DeluxePricingStructure == Constants.DeluxePricingStructure.LegacyDeluxeEpcsLogRx ||
                    SessionLicense.DeluxePricingStructure == Constants.DeluxePricingStructure.DeluxeEpcsEpaLogRx ||
                    SessionLicense.DeluxePricingStructure == Constants.DeluxePricingStructure.DeluxeEpcsEpaLogRx2017)
                    isLicenseLogRxEnabled = true;

                return isLicenseLogRxEnabled;
            }
        }

        public string getCurrentPageName()
        {
            string strPath = System.Web.HttpContext.Current.Request.Url.AbsolutePath;
            System.IO.FileInfo fInfo = new System.IO.FileInfo(strPath);
            string strRet = fInfo.Name;
            return strRet.ToLower();
        }
        
        /// <summary>
        ///  return Clinical Viewer Post Data.
        /// </summary>
        /// <returns></returns>
        public string ClinicalViewerPostData
        {
            get
            {
                string postData = string.Empty;
                postData = EPSBroker.GetClinicalViewerSSOLink(
                    SessionPatientID,
                    SessionLicense.EnterpriseClient.SystemOID,
                    Convert.ToString(PageState["UserFirstName"]),
                    Convert.ToString(PageState["UserLastName"]),
                    SessionUserName,
                    SessionUserType.ToString(), //"Nurse"
                    SessionLicense.EnterpriseClient.CommunityURL
                    );

                return postData;
            }
            set
            {
                PageState["ClinicalViewerPostData"] = value;
            }
        }
        public bool IsAngularMode
        {
            get { return Helper.IsAngularMode; }
        }
        /// <summary>
        /// Checks whether ppt plus shield token to be refreshed.
        /// </summary>
        public void CheckForPptPlusTokenRefresh()
        {
            if (SessionLicense.EnterpriseClient.ShowPPTPlus)
            {
                if (!Request.Path.ToLower().Contains(Constants.PageNames.SELECT_ACCOUNT_AND_SITE.ToLower()))
                {
                    var pptPlusTokenObject = PPTPlus.GetPptPlusSamlToken(PageState);
                }
            }
        }
    }
}