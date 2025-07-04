using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.DatabaseSelector;
using Allscripts.ePrescribe.Shared.Logging;
using Allscripts.Impact;
using eRxWeb.State;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using Allscripts.ePrescribe.Objects;
using Rx = Allscripts.Impact.Rx;
using RxUser = Allscripts.Impact.RxUser;
using static Allscripts.ePrescribe.Common.Constants;

namespace eRxWeb.AppCode
{
    public class ApiHelper
    {
        public static bool IsTieAdPlacementEnabled
        {
            get
            {
                bool isShowAdPlacement = false;
                if (System.Configuration.ConfigurationManager.AppSettings["IsTieAdPlacementEnabled"] != null &&
                    bool.Parse(System.Configuration.ConfigurationManager.AppSettings["IsTieAdPlacementEnabled"]))
                {
                    isShowAdPlacement = true;
                }
                return isShowAdPlacement;
            }

        }
        public static ApplicationLicense SessionLicense(IStateContainer pageState)
        {

            if (pageState[Constants.SessionVariables.SessionLicense] == null)
            {
                pageState[Constants.SessionVariables.SessionLicense] = new ApplicationLicense(string.Empty, 0, string.Empty, GetDBID(pageState));
            }

            return pageState.Cast<ApplicationLicense>(Constants.SessionVariables.SessionLicense, null);
        }

        private static string DelegateProviderName(IStateContainer pageState)
        {
            string providerName = string.Empty;
            RxUser provider = pageState.Cast<RxUser>("DelegateProviderID", null);
            if (provider != null)
            {
                providerName = provider.GetAttribute("Title") + " " + provider.GetAttribute("FirstName") + " " + provider.GetAttribute("LastName");
            }

            return providerName;
        }
        public static string GetSessionDelegateProviderID(IStateContainer pageState)
        {
            return pageState.GetStringOrEmpty("DelegateProviderID");
        }
        public static string GetSessionUserID(IStateContainer pageState)
        {
            return pageState.GetStringOrEmpty("USERID");
        }

        public static string GetSessionLicenseID(IStateContainer pageState)
        {
            return pageState.GetStringOrEmpty("LICENSEID");
        }

        public static int GetSessionSiteID(IStateContainer pageState)
        {
            return pageState.GetInt("SITEID", 0);
        }

        public static ConnectionStringPointer GetDBID(IStateContainer pageState)
        {
            return pageState.Cast<ConnectionStringPointer>("DBID", ConnectionStringPointer.ERXDB_DEFAULT);
        }

        public static Constants.UserCategory GetUserType(IStateContainer pageState)
        {
            return pageState.Cast<Constants.UserCategory>("UserType", Constants.UserCategory.GENERAL_USER);
        }

        public static string GetSessionPatientId(IStateContainer pageState)
        {
            return pageState.GetStringOrEmpty("PATIENTID");
        }

        
        public static string GetSessionPracticeState(IStateContainer pageState)
        {
            return pageState.GetStringOrEmpty("PRACTICESTATE");
        }
        public static string SessionAppVersion(IStateContainer pageState)
        {

            string ret = string.Empty;
            if (pageState["EPRESCRIBE_APP_VERSION"] != null)
            {
                ret = pageState["EPRESCRIBE_APP_VERSION"].ToString();
            }
            else
            {
                try
                {
                    ePrescribeSvc.ePrescribeSvc eps = new ePrescribeSvc.ePrescribeSvc();
                    eps.Url = ConfigurationManager.AppSettings["ePrescribeSvc.eprescribesvc"].ToString();
                    ePrescribeSvc.ApplicationVersionRequest appVerReq = new ePrescribeSvc.ApplicationVersionRequest();
                    appVerReq.ApplicationID = ePrescribeSvc.ePrescribeApplication.MainApplication;
                    pageState["EPRESCRIBE_APP_VERSION"] = eps.GetFullAppversion(appVerReq);

                    ret = pageState["EPRESCRIBE_APP_VERSION"].ToString();
                }
                catch (Exception)
                {
                    ret = string.Empty;
                }
            }
            return ret;

        }
        public static bool ShouldDisplayInfoScripts(IStateContainer pageState)
        {
            return pageState.GetBooleanOrFalse("DisplayInfoScripts");
        }
        public static string AuditException(string errorMessage, IStateContainer PageState)
        {
            var dbid = ApiHelper.GetDBID(PageState);
            var userId = ApiHelper.GetSessionUserID(PageState);
            var licenseId = ApiHelper.GetSessionLicenseID(PageState);
            var msg = Audit.AddApiException(userId, licenseId, errorMessage, dbid);
            PageState[SessionVariables.CURRENT_ERROR] = errorMessage;
            return msg;
        }

        public static T MapObject<T>(DataRow data)
        {
            var obj = Activator.CreateInstance<T>();
            foreach (var prop in typeof(T).GetProperties())
            {
                prop.SetValue(obj, data.ToStringValue(prop.Name));
            }
            return obj;
        }

        public static PhysicianMasterPage GetMasterPage(IStateContainer pageState)
        {
            var ms = new PhysicianMasterPage();
            ms.PageState = pageState;
            return ms;
        }

        public static Rx GetCurrentRx(IStateContainer pageState)
        {
            Rx rx = null;
            ArrayList rxList = pageState.Cast<ArrayList>("RxList", null);
            if (rxList != null && rxList.Count > 0)
                rx = (Rx)rxList[0];
            return rx;
        }

        public static EnabledDisabled LexicompView(IStateContainer session)
        {
            var sessionLicense = SessionLicense(session);
            var viewStatus = EnabledDisabled.Disabled;

            if (sessionLicense.EnterpriseClient.EnableLexicomp &&
                (sessionLicense.LicenseDeluxeStatus == Constants.DeluxeFeatureStatus.On ||
                 sessionLicense.LicenseDeluxeStatus == Constants.DeluxeFeatureStatus.AlwaysOn ||
                 sessionLicense.LicenseDeluxeStatus == Constants.DeluxeFeatureStatus.Cancelled))
            {

                if (session.GetBooleanOrFalse(Constants.SessionVariables.ShowLexicompDefault))
                {
                    if (sessionLicense.EnterpriseClient.EnableLexicompDefault)
                    {
                        viewStatus = EnabledDisabled.Enabled;
                    }
                }
                else
                {
                    if (session.GetBooleanOrFalse(Constants.LicensePreferences.SHOW_LEXICOMP))
                    {
                        viewStatus = EnabledDisabled.Enabled;
                    }
                }
            }
            return viewStatus;
        }
        
        public static string GetSessionShieldUserName(IStateContainer pageState)
        {
            if (pageState[Constants.SessionVariables.SessionShieldUserName] != null)
            {
                return pageState[Constants.SessionVariables.SessionShieldUserName].ToString();
            }

            return string.Empty;

        }

        public static string GetEprescribeExternalAppInstanceID(IStateContainer pageState)
        {
            return ShieldSettings.ePrescribeExternalAppInstanceId(pageState);
        }

        public static string GetShieldSecurityToken(IStateContainer pageState)
        {

            string retVal = null;
            if (System.Configuration.ConfigurationManager.AppSettings[Constants.AppConfigVariables.SAMLTokenStorageLocation].ToString().Equals("Session"))
            {
                if (pageState[Constants.SessionVariables.ShieldSecurityToken] == null)
                    retVal = null;

                retVal = pageState[Constants.SessionVariables.ShieldSecurityToken].ToString();
            }
            return retVal;
        }

        public static void AuditLogUserInsert(ePrescribeSvc.AuditAction auditAction, string userIDActionPerformedOn, string licenseID, string userID, string userHostAddress, ConnectionStringPointer dbID)
        {
            EPSBroker.AuditLogUserInsert(
                auditAction,
                licenseID,
                userID,
                userIDActionPerformedOn,
                userHostAddress,
                dbID);
        }
    }
}