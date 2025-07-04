using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.DatabaseSelector;
using Allscripts.Impact;
using eRxWeb.AppCode.AngularHelpers;
using eRxWeb.ServerModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace eRxWeb.AppCode.Users
{
    public static class UserPreferencesModelHelper
    {
        public static UserPreferencesModel SetUserPreferencesModel(ePrescribeSvc.RxUser rxUser, string editeeUserID, string licenseID, UserMode userMode, ConnectionStringPointer dbID)
        {
            var model = new UserPreferencesModel();
            model.IsFaxSiteInvalid = false;
            if (userMode != UserMode.AddOtherUser && rxUser?.DefaultFaxSiteID != 0)
            {
                string isSiteActive = string.Empty;
                DataSet ds = ApplicationLicense.SiteLoad(licenseID, rxUser.DefaultFaxSiteID, dbID);

                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    isSiteActive = ds.Tables[0].Rows[0]["Active"].ToString();
                }

                if (isSiteActive == "Y")
                {
                    model.DefaultFaxSiteID = rxUser.DefaultFaxSiteID.ToString();
                }
                else
                {
                    model.IsFaxSiteInvalid = true;
                }
            }
            else
            {
                model.DefaultFaxSiteID = "1";
            }
            model.SiteList = ConvertSiteDataTableToList(ApplicationLicense.LoadSites(licenseID, dbID));

            var preferences = Preference.LoadUserPreferences(PreferenceCategory.PROVIDER_OPTIONS, editeeUserID, licenseID, dbID);

            if (preferences != null)
            {
                foreach (string key in preferences.Keys)
                {
                    switch (key)
                    {
                        case Constants.ProviderPreferences.SHOW_RTB:
                            model.RTBPreference = setPreference(preferences, key);
                            break;
                        case Constants.ProviderPreferences.SHOW_PPT:
                            model.PPTPreference = setPreference(preferences, key);
                            break;
                        case Constants.ProviderPreferences.SHOW_RTPS:
                            model.RTPSPreference = setPreference(preferences, key);
                            break;
                        case Constants.ProviderPreferences.SHOW_PPT_OFFER:
                            model.PPTOffersPreference = setPreference(preferences, key);
                            break;
                        case Constants.ProviderPreferences.SHOW_RTB_AUTO_THERAPEUTIC_ALTERNATIVE:
                            model.AutoTherapeuticAlternativesPreference = setPreference(preferences, key);
                            break;
                        default: break;
                    }
                }
            }

            model.IsShowRxFavourites = GetRxFavouritesDisplaySetting(userMode);

            return model;
        }

        public static bool GetRxFavouritesDisplaySetting(UserMode userMode)
        {
            return userMode == UserMode.SelfEdit;
        }

        private static bool setPreference(Hashtable preferences, string key)
        {
            Preference pref = (Preference)preferences[key];
            string userValue = (string)pref.UserValue;
            string defaultValue = (string)pref.Value;
            return string.IsNullOrWhiteSpace(userValue) || userValue.Trim().Equals("NULL", StringComparison.OrdinalIgnoreCase)
                                      ? defaultValue.Equals(Constants.CommonAbbreviations.YES, StringComparison.OrdinalIgnoreCase)
                                      : userValue.Equals(Constants.CommonAbbreviations.YES, StringComparison.OrdinalIgnoreCase);
        }

        public static List<DropDownListElement> ConvertSiteDataTableToList(DataTable dtSite)
        {
            var siteList = new List<DropDownListElement>();
            foreach (DataRow row in dtSite.Rows)
            {
                siteList.Add(new DropDownListElement
                {
                    Value = row["SiteID"].ToString(),
                    Description = row["SiteName"].ToString()
                });
            }
            return siteList;
        }
    }
}