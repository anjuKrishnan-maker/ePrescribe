using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.Data;
using Allscripts.ePrescribe.DatabaseSelector;
using Allscripts.Impact;
using Allscripts.Impact.Interfaces;
using Allscripts.Impact.Utilities;
using eRxWeb.AppCode.Interfaces;
using eRxWeb.State;
using TieServiceClient;
using SystemConfig = Allscripts.Impact.SystemConfig;

namespace eRxWeb.AppCode
{
    public class TieUtility : ITieUtility
    {
        public static HttpCookie CreateTieCookie(string specialtyCode1, string specialtyCode2, Guid enterpriseId, IStateContainer pageState, ConnectionStringPointer dbId)
        {
            var licenseId = pageState.GetGuidOr0x0(Constants.SessionVariables.LicenseId);
            var logRxUserGuid = pageState.GetInt(Constants.SessionVariables.LogRxUserGuid, 0);
                
            var dsSite = ApplicationLicense.SiteLoad(licenseId.ToString(), pageState.Cast("SITEID", 1), dbId);
            
            if (dsSite != null && dsSite.Tables.Count > 0 && dsSite.Tables[0].Rows.Count > 0)
            {
                return CreateTieCookie(dsSite, specialtyCode1, specialtyCode2, enterpriseId, licenseId, logRxUserGuid, pageState, dbId);
            }

            return new HttpCookie("eRxTIECookie");
        }

        public static HttpCookie CreateTieCookie(DataSet site, string specialtyCode1, string specialtyCode2, Guid enterpriseId, Guid licenseId, int logRxUserGuid, IStateContainer pageState, ConnectionStringPointer dbId)
        {
            var logRxCookie = new HttpCookie("eRxTIECookie");

            var applicationLicense = pageState.Cast<ApplicationLicense>("SessionLicense", null);

            var isLicenseAdsEnabled = IsLicenseAdsEnabled(applicationLicense);

            var accountType = GetAccountType(applicationLicense.DeluxeFeatureStatusDisplay);

            var userTypeCode = GetUserTypeCode(pageState.Cast("UserType", Constants.UserCategory.GENERAL_USER));

            var shouldShowAds = ShouldShowAds(pageState.GetBooleanOrFalse("ShowADPlacement"), isLicenseAdsEnabled, applicationLicense?.DeluxeFeatureStatusDisplay, enterpriseId, applicationLicense.EnterpriseDeluxeFeatureStatus);

            pageState[Constants.SessionVariables.ShouldShowAds] = shouldShowAds;

            AddDeaSchedulesToCookie(ref logRxCookie, pageState.Cast("DEASCHEDULESALLOWED", new List<string>()));

            AddSpecialtyCodes(ref logRxCookie, specialtyCode1, specialtyCode2, isLicenseAdsEnabled, userTypeCode, licenseId, dbId);

            logRxCookie.Values.Add("UserType", userTypeCode);
            logRxCookie.Values.Add("SiteState", Convert.ToString(site.Tables[0].Rows[0]["State"]));
            logRxCookie.Values.Add("SiteZip", Convert.ToString(site.Tables[0].Rows[0]["ZipCode"]));
            logRxCookie.Values.Add("AccountType", accountType);
            logRxCookie.Values.Add("AdOptOut", shouldShowAds ? "N" : "Y");
            if (logRxUserGuid > 0)//0 means default not a real value
            {
                logRxCookie.Values.Add("LogRxUserID", Convert.ToString(logRxUserGuid));
            }

            logRxCookie.Expires = DateTime.Parse(DateTime.Today.ToShortDateString()).AddMonths(1);

            return logRxCookie;
        }

        private static void AddSpecialtyCodes(ref HttpCookie eRxTieCookie, string specialtyCode1, string specialtyCode2, bool isLicenseLogRxEnabled, string userTypeCode, Guid licenseId, ConnectionStringPointer dbId)
        {
            if (isLicenseLogRxEnabled && (userTypeCode.Equals("POB") || userTypeCode.Equals("General")))
            {
                AddSpecilaityCodeForPobAndStaff(ref eRxTieCookie, specialtyCode1, specialtyCode2, licenseId, dbId);
            }
            else
            {
                eRxTieCookie.Values.Add("SpecialtyCode1", specialtyCode1);
                eRxTieCookie.Values.Add("SpecialtyCode2", specialtyCode2);
            }

            if (String.IsNullOrEmpty(eRxTieCookie["SpecialtyCode1"])) // 824444:To resolve the issue
            {
                DataTable dtSpecialtiesOnLicense = User.GetSpecialtiesOnLicenseId(licenseId.ToString(), dbId); //Get all the Specialties code under that License
                if (dtSpecialtiesOnLicense.Rows.Count > 0)
                {
                    specialtyCode1 = dtSpecialtiesOnLicense.Rows[0]["Specialty_CD"].ToString();
                    eRxTieCookie.Values.Remove("SpecialtyCode1");
                    eRxTieCookie.Values.Add("SpecialtyCode1", specialtyCode1);
                }
            }
        }

        public static void AddSpecilaityCodeForPobAndStaff(ref HttpCookie eRxTieCookie, string specialtyCode1, string specialtyCode2, Guid licenseId, ConnectionStringPointer dbID)
        {
            DataTable dtSpecialtiesOnLicense = User.GetSpecialtiesOnLicenseId(licenseId.ToString(), dbID);//Get all the Specialties code under that License
            DataTable dtSpecialtyWithAds = User.GetSpecialtyWithAds(ConnectionStringPointer.SHARED_DB);//Get all the Specialties code under SpecialtyWithAds table
            var querySpecialtiesOnLicense = (from j in dtSpecialtiesOnLicense.AsEnumerable()
                                             select j.Field<string>("Specialty_CD")).ToList<string>();
            var querySpecialtyWithAds = (from j in dtSpecialtyWithAds.AsEnumerable()
                                         select j.Field<string>("Specialty_CD")).ToList<string>();
            var query = querySpecialtiesOnLicense.Intersect<string>(querySpecialtyWithAds).ToArray<string>();// Get the Specialities present under that License and as well as in SpecialtyWithAds table.

            if (Convert.ToInt32(query.Count<string>()) > 0)
            {
                eRxTieCookie.Values.Add("SpecialtyCode1", query[0]);
                if (Convert.ToInt32(query.Count<string>()) > 1)
                {
                    eRxTieCookie.Values.Add("SpecialtyCode2", query[1]);
                }
                else
                {
                    eRxTieCookie.Values.Add("SpecialtyCode2", specialtyCode2);
                }

            }
            else
            {
                eRxTieCookie.Values.Add("SpecialtyCode1", specialtyCode1);
                eRxTieCookie.Values.Add("SpecialtyCode2", specialtyCode2);
            }
        }

        public static void AddDeaSchedulesToCookie(ref HttpCookie eRxTieCookie, List<string> deaSchedules)
        {
            if (deaSchedules?.Count > 0)
            {
                foreach (string deaSchedule in deaSchedules)
                {
                    eRxTieCookie.Values.Add("DEASchedule", deaSchedule);
                }
            }
            else
            {
                eRxTieCookie.Values.Add("DEASchedule", String.Empty);
            }
        }

        public static string GetUserTypeCode(Constants.UserCategory userType)
        {
            switch (userType)
            {
                case Constants.UserCategory.PROVIDER:
                    return "Provider";
                case Constants.UserCategory.PHYSICIAN_ASSISTANT:
                case Constants.UserCategory.PHYSICIAN_ASSISTANT_SUPERVISED:
                    return "PA";
                case Constants.UserCategory.POB_SUPER:
                case Constants.UserCategory.POB_REGULAR:
                case Constants.UserCategory.POB_LIMITED:
                    return "POB";
                case Constants.UserCategory.GENERAL_USER:
                    return "General";
            }
            return "General";
        }

        public static bool ShouldShowAds(bool showAdPlacement, bool isLicenseAdsEnabled, string deluxeFeatureDisplay, Guid enterpriseId, Constants.DeluxeFeatureStatus enterpriseDeluxeStatus)
        {
            var shouldShowAds = false;
            var deluxeSalesEnterprise = new Guid("F702FABB-C77E-4F49-9F1E-50C7B844BBA6");
            var compulsoryBasicSales = new Guid("6E55DB44-D9A8-4925-A654-A63708CF34F9");

            if (showAdPlacement || isLicenseAdsEnabled)
            {
                // Veradigm Enterprise (Non Platinum accounts)
                if (!deluxeFeatureDisplay.Contains("Platinum"))
                {
                    shouldShowAds = true;
                }
                else
                {
                    //list of enterprises classified under platinum for whom the ads should be shown
                    if( enterpriseId == compulsoryBasicSales ||
                        enterpriseDeluxeStatus == Constants.DeluxeFeatureStatus.CompulsoryBasic ||
                        enterpriseDeluxeStatus == Constants.DeluxeFeatureStatus.ForceCompulsoryBasic)
                    {
                        shouldShowAds = true;
                    }
                }

                if (enterpriseId == deluxeSalesEnterprise)
                {
                    shouldShowAds = true;
                }
            }            

            // Platinum accounts and it is not under Deluxe Sales
            if (deluxeFeatureDisplay.Contains("Platinum")
                && enterpriseId != deluxeSalesEnterprise
                && showAdPlacement)
            {
                shouldShowAds = true;
            }

            return shouldShowAds;
        }

        public static bool IsLicenseAdsEnabled(ApplicationLicense applicationLicense)
        {
            bool isLicenseLogRxEnabled = applicationLicense.DeluxePricingStructure == Constants.DeluxePricingStructure.Basic ||
                                         applicationLicense.DeluxePricingStructure == Constants.DeluxePricingStructure.DeluxeEpcsLogRx ||
                                         applicationLicense.DeluxePricingStructure == Constants.DeluxePricingStructure.DeluxeEpaLogRx ||
                                         applicationLicense.DeluxePricingStructure == Constants.DeluxePricingStructure.DeluxeEpcsEpaLogRx ||
                                         applicationLicense.DeluxePricingStructure == Constants.DeluxePricingStructure.DeluxeEpcsEpaLogRx2017 ||
                                         applicationLicense.DeluxePricingStructure == Constants.DeluxePricingStructure.LegacyDeluxeEpaLogRx ||
                                         applicationLicense.DeluxePricingStructure == Constants.DeluxePricingStructure.LegacyDeluxeEpcsLogRx ||
                                         applicationLicense.DeluxePricingStructure == Constants.DeluxePricingStructure.CompulsoryBasic;
            return isLicenseLogRxEnabled;
        }

        public static string GetAccountType(string deluxeFeatureStatusDisplay)
        {
            string accountType;

            if (deluxeFeatureStatusDisplay.Contains("Platinum"))
            {
                accountType = "Platinum";
            }
            else if (deluxeFeatureStatusDisplay.Contains("Deluxe"))
            {
                accountType = "Deluxe";
            }
            else
            {
                accountType = "Basic";
            }
            return accountType;
        }

        public Dictionary<string, IEnumerable<string>> GetTargetKeys(HttpCookie eRxTieCookie)
        {
            var targetingKeys = new Dictionary<string, IEnumerable<string>>();
            var cookiesCollection = eRxTieCookie.Values;
            foreach (string key in cookiesCollection.AllKeys)
            {
                if(key == null) continue;

                var values = new[] {eRxTieCookie[key]};
                if (key == "DEASchedule")
                {
                    var deaScheduleArray = values[0].Split(',');

                    targetingKeys.Add(key, deaScheduleArray);
                }
                else
                {
                    targetingKeys.Add(key, values);
                }
            }
            return targetingKeys;
        }

        IPlacementResponse ITieUtility.GetAdPlacement(string locationId, HttpCookieCollection cookies, ITieUtility iTieUtility, ITIEServiceManager iTieServiceManager, IConfigurationManager iConfigurationManager)
        {
            return GetAdPlacement(locationId, cookies, iTieUtility, iTieServiceManager, iConfigurationManager);
        }

        /// <summary>
        ///  Read persistent cookies for AD placement and send it to TIE service, get Ad placement.
        /// </summary>
        /// <param name="locationId">LocationID is the location of page where Ad to be displayed.</param>
        /// <param name="cookies"></param>
        /// <param name="iTieUtility"></param>
        /// <param name="iTieServiceManager"></param>
        /// <param name="iConfigurationManager"></param>
        public static IPlacementResponse GetAdPlacement(string locationId, HttpCookieCollection cookies, ITieUtility iTieUtility, ITIEServiceManager iTieServiceManager, IConfigurationManager iConfigurationManager)
        {
            IPlacementResponse placementResponse = null;

            var targetingKeys = new Dictionary<string, IEnumerable<string>>();

            if (Convert.ToBoolean(iConfigurationManager.GetValue("IsTieAdPlacementEnabled")))
            {
                if (cookies["eRxTIECookie"] != null)
                {
                    var eRxTieCookie = cookies.Get("eRxTIECookie");

                    if (eRxTieCookie != null)
                    {
                        targetingKeys = iTieUtility.GetTargetKeys(eRxTieCookie);
                    }
                }

                if (targetingKeys.ContainsKey("AdOptOut") && targetingKeys["AdOptOut"].FirstOrDefault() == "Y") return placementResponse;
                
                // call TIE service for all login pages.
                placementResponse = iTieServiceManager.TIEPlacementResponse(locationId, targetingKeys, iConfigurationManager.GetValue("TieEnvironment"));
            }

            return placementResponse;
        }

        /// <summary>
        ///  Read persistent cookies for AD placement and send it to TIE service, get Ad placement.
        /// </summary>
        /// <param name="cookies"></param>
        /// <param name="iTieUtility"></param>
        /// <param name="iTieServiceManager"></param>
        /// <param name="iConfigurationManager"></param>
        public static IPlacementResponse GetAdPlacement(HttpCookieCollection cookies, ITieUtility iTieUtility, ITIEServiceManager iTieServiceManager, IConfigurationManager iConfigurationManager)
        {
            return iTieUtility.GetAdPlacement(string.Empty, cookies, iTieUtility, iTieServiceManager, iConfigurationManager);
        }

        /// <summary>
        ///  Read persistent cookies for AD placement and send it to TIE service, get Ad placement.
        /// </summary>
        public static IPlacementResponse GetAdForMasterPage(string urlAbsolutePath, HttpCookieCollection cookies, ITieUtility iTieUtility)
        {
            // get the page name.
            var pageName = iTieUtility.ParseAbsoluteUrlForPageName(urlAbsolutePath);

            var locationName = iTieUtility.GetMasterPageTieLocationName(pageName);

            return iTieUtility.GetAdPlacement(locationName, cookies, iTieUtility, new TIEServiceManager(), new ConfigurationManager());
        }

        public string GetMasterPageTieLocationName(string pageName)
        {
            switch (pageName)
            {
                case "login":
                    pageName = Constants.TIELocationPage.Login_Page;
                    break;
                case "searchpatient":
                case "default":
                case "selectpatient":
                    pageName = Constants.TIELocationPage.SearchPatient_Page;
                    break;
                case "settings":
                    pageName = Constants.TIELocationPage.Settings_Page;
                    break;
                case "manageaccount":
                    pageName = Constants.TIELocationPage.ManageAccount_Page;
                    break;
                case "messagequeuetx":
                case "messagequeuetx?from=reports":
                    pageName = Constants.TIELocationPage.MessageQueueTx_Page;
                    break;
                case "myprofile":
                    pageName = Constants.TIELocationPage.MyProfile_Page;
                    break;
                case "reports":
                    pageName = Constants.TIELocationPage.Reports_Page;
                    break;
                case "diagnosis":
                    pageName = Constants.TIELocationPage.SelectDx_Page;
                    break;
                case "selectaccountandsite":
                    pageName = Constants.TIELocationPage.SelectAccountAndSite_Page;
                    break;
                case "addpatient":
                    pageName = Constants.TIELocationPage.AddPatient_Page;
                    break;
                case "library":
                    pageName = Constants.TIELocationPage.Library_Page;
                    break;
                case "exception":
                    pageName = Constants.TIELocationPage.Exception_Page;
                    break;
                case "approverefilltask":
                case "pharmacy":
                case "multipleviewcss":
                case "taskprocessor":
                case "taskscript":
                    pageName = "NoAnalytics";
                    break;
                default:
                    pageName = string.Empty;
                    break;
            }
            return pageName;
        }

        /// <summary>
        ///  Read persistent cookies for AD placement and send it to TIE service, get Ad placement.
        /// </summary>
        public static IPlacementResponse GetHelpPageAdPlacement(string urlAbsolutePath, HttpCookieCollection cookies, ITieUtility itTieUtility)
        {
            // get the page name.
            var pageName = itTieUtility.ParseAbsoluteUrlForPageName(urlAbsolutePath);

            var locationName = itTieUtility.GetHelpPageTieLocationName(pageName);

            return itTieUtility.GetAdPlacement(locationName, cookies, new TieUtility(), new TIEServiceManager(), new ConfigurationManager());
        }

        public string ParseAbsoluteUrlForPageName(string urlAbsolutePath)
        {
            string[] strarry = urlAbsolutePath.Split('/');
            int len = strarry.Length;
            string sRet = strarry[len - 1];
            string[] pageNameWithExtn = sRet.Split('.');
            string pageName = pageNameWithExtn[0].ToLower();
            return pageName;
        }

        public string GetHelpPageTieLocationName(string pageName)
        {
            switch (pageName)
            {
                case "default":
                    pageName = Constants.TIELocationPage.Home_Page;
                    break;
                case "gettingstarted":
                    pageName = Constants.TIELocationPage.GettingStarted_Page;
                    break;
                case "tooltips":
                    pageName = Constants.TIELocationPage.ToolTips_Page;
                    break;
                case "mobile":
                    pageName = Constants.TIELocationPage.Mobile_Page;
                    break;
                case "faq":
                    pageName = Constants.TIELocationPage.faq_Page;
                    break;
                case "tutorial":
                    pageName = Constants.TIELocationPage.Tutorial_Page;
                    break;
                case "discuss":
                    pageName = Constants.TIELocationPage.Discuss_Page;
                    break; // this scenario not working...
                case "training":
                    pageName = Constants.TIELocationPage.Training_Page;
                    break;
                case "add-on":
                    pageName = Constants.TIELocationPage.AddOn_Page;
                    break;
                case "import":
                    pageName = Constants.TIELocationPage.Import_Page;
                    break;
                case "login":
                    pageName = Constants.TIELocationPage.Home_Page;
                    break;
            }

            return pageName;
        }

        public static int RetrieveLogRxUserId(Guid userGuid)
        {
            return User.RetrieveLogRxUserId(userGuid, ConnectionStringPointer.SHARED_DB);
        }
    }
}