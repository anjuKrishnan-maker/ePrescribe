using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.DatabaseSelector;
using Allscripts.ePrescribe.Objects;
using Allscripts.Impact;
using eRxWeb.State;
using System;
using Allscripts.ePrescribe.Common;
using DeaLicenseType = eRxWeb.ePrescribeSvc.DeaLicenseType;
using RxUser = Allscripts.Impact.RxUser;

namespace eRxWeb.AppCode.StateUtils
{
    public static class UserInfo
    {
        public static string GetSessionUserID(IStateContainer session)
        {
            if (session["USERID"] == null)
                return null;

            return session["USERID"].ToString();
        }

        public static string GetSessionLicenseID(IStateContainer session)
        {
            if (session["LICENSEID"] == null)
                return null;

            return session["LICENSEID"].ToString();
        }

        public static string GetSessionPracticeState(IStateContainer session)
        {
            if (session["PRACTICESTATE"] == null)
                return null;

            return session["PRACTICESTATE"].ToString();
        }

        public static Constants.UserCategory GetSessionUserType(IStateContainer session)
        {
            return session.Cast("UserType", Constants.UserCategory.GENERAL_USER);
        }

        public static ApplicationLicense GetSessionLicense(IStateContainer state)
        {
            if (state["SessionLicense"] == null)
            {
                if (state["DBID"] == null)
                {
                    state["SessionLicense"] = new ApplicationLicense(string.Empty, 0, string.Empty, ConnectionStringPointer.ERXDB_DEFAULT);
                }
                else
                {
                    state["SessionLicense"] = new ApplicationLicense(string.Empty, 0, string.Empty, (ConnectionStringPointer)state["DBID"]);
                }
            }

            return (ApplicationLicense)state["SessionLicense"];
        }

        public static bool IsPOBUser(IStateContainer session)
        {
            Constants.UserCategory userCategory = GetSessionUserType(session);
            return (userCategory == Constants.UserCategory.POB_LIMITED
                    || userCategory == Constants.UserCategory.POB_REGULAR
                    || userCategory == Constants.UserCategory.POB_SUPER);
        }

        public static DateTime GetLicenseCreationDate(IStateContainer session)
        {
            DateTime licenseCreatedDate = session.GetDateTimeOrNow(Constants.SessionVariables.LicenseCreationDate);
            return licenseCreatedDate;
        }

        /// <summary>
        /// This is the provider selected when a POB user has logged in and selected a provider or unsupervised physician assistant to write perscriptions on their behalf,
        /// OR when a supervised physician assistant has logged in selected a provider or unsupervised physician assistant to write perscriptions on their behalf (which should be changed to a Supervising Provider)
        /// </summary>
        public static RxUser GetDelegateProvider(IStateContainer session)
        {
            if (session["DelegateProviderID"] != null)
                return new RxUser(session["DelegateProviderID"].ToString(), (ConnectionStringPointer)session["DBID"]);
            else
                return null;
        }

        public static string GetDelegateProviderName(IStateContainer session)
        {
            string providerName = string.Empty;
            RxUser provider = GetDelegateProvider(session);
            if (provider != null)
            {
                providerName = provider.Title + " " + provider.FirstName + " " + provider.LastName;
            }
            return providerName;
        }

        public static string AppSettings(string key)
        {
            return System.Configuration.ConfigurationManager.AppSettings[key].ToString();
        }

        public static string GetDEANumberToBeUsed(IStateContainer state)
        {
            string deaNumberToBeUsed = string.Empty;

            if (state["DEA"] != null)
            {
                deaNumberToBeUsed = state["DEA"].ToString();
            }
            else if (IsPOBUser(state))
            {
                string delegateProviderID = state["DelegateProviderID"].ToString().Trim();
                deaNumberToBeUsed = getProviderDEA(deaNumberToBeUsed, delegateProviderID, (ConnectionStringPointer)state["DBID"]);
            }

            return deaNumberToBeUsed;
        }

        private static string getProviderDEA(string deaNumberToBeUsed, string providerID, ConnectionStringPointer dbID)
        {
            ePrescribeSvc.DEALicense[] deaList = EPSBroker.GetProviderDEALicenses(providerID, dbID);

            foreach (ePrescribeSvc.DEALicense deaNumber in deaList)
            {
                if (deaNumber.DeaLicenseTypeId == DeaLicenseType.DefaultDea)
                {
                    deaNumberToBeUsed = deaNumber.DEANumber;
                    break;
                }
            }
            return deaNumberToBeUsed;
        }

        public static bool CanApplyFinancialOffers(IStateContainer state)
        {
            bool canApplyOffers = false;

            if (state["APPLYFINANCIALOFFERS"] != null && state["APPLYFINANCIALOFFERS"].ToString().Equals(true.ToString()))
            {
                canApplyOffers = true;
            }

            return canApplyOffers;
        }


        /// <summary>
        /// This is the provider selected when a POB user has logged in and selected a supervised physician assistant to write prescription on their behalf.
        /// Also, when a POB user has selected a refill request designated for a supervised physician assistant
        /// </summary>
        public static RxUser GetSupervisingProvider(IStateContainer session)
        {
            if (session["SUPERVISING_PROVIDER_ID"] != null)
                return new RxUser(session["SUPERVISING_PROVIDER_ID"].ToString(), (ConnectionStringPointer)session["DBID"]);
            else
                return null;
        }

        public static string GetSupervisingProviderName(IStateContainer session)
        {
            string providerName = string.Empty;
            RxUser provider = GetSupervisingProvider(session);
            if (provider != null)
            {
                providerName = provider.Title + " " + provider.FirstName + " " + provider.LastName;
            }
            return providerName;
        }

        public static ePrescribeSvc.ShieldTenantIDProofingModel GetIdProofingMode(IStateContainer session)
        {
            if (session.EqualsToEnum(Constants.SessionVariables.TenantIdProofingModel, ePrescribeSvc.ShieldTenantIDProofingModel.Institutional))
            {
                return ePrescribeSvc.ShieldTenantIDProofingModel.Institutional;
            }
            return ePrescribeSvc.ShieldTenantIDProofingModel.Individual;

        }

        public static bool IsPermissionGranted(string permissionName, IStateContainer session)
        {
            var appPermissions = session.Cast(Constants.SessionVariables.UserAppPermissions, new ePrescribeSvc.Permission[0]);

            if (appPermissions == null)
            {
                return false;
            }

            return Helper.DoesClaimExist(appPermissions, permissionName);
        }

        public static bool IsIdProofingRequired(IStateContainer session)
        {
            if (session.GetBooleanOrFalse(Constants.SessionVariables.IsIdProofingRequired))
            {
                if (eRxWeb.Users.UpdateUserIdproofingStatus(session))
                {
                    session[Constants.SessionVariables.IsIdProofingRequired] = false;
                }
            }
            return session.GetBooleanOrFalse(Constants.SessionVariables.IsIdProofingRequired);
        }
    }
}