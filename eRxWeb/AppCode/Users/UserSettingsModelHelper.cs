using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.DatabaseSelector;
using eRxWeb.ServerModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http.Formatting;
using System.Web;

namespace eRxWeb.AppCode.Users
{
    public static class UserSettingsModelHelper
    {
        public static UserSettingsModel SetUserSettingsModel(ePrescribeSvc.RxUser rxUser, string userID, string licenseID, UserMode userMode, ConnectionStringPointer dbID)
        {
            var model = new UserSettingsModel();
            int userType = rxUser != null ? getUserType(rxUser.AppRoles): -1;
            model.UserType = (Constants.UserCategory)userType;

            if (userMode == UserMode.AddOtherUser || rxUser.POBViewAllProviders)
            {
                model.SupervisingProviderSelectionMode = SupervisingProviderSelectionMode.All;
            }
            else
            {
                model.SupervisingProviderSelectionMode = SupervisingProviderSelectionMode.Select;
            }

            if (userMode != UserMode.AddOtherUser)
            {
                model.Name = $"{rxUser.FirstName} {rxUser.LastName}"; 
            }

            model.UserSupervisingProvidersList = GetSupervisingProvidersList(userID, licenseID, dbID);
            return model;
        }

        public static bool IsProviderOrPA(UserSettingsModel model)
        {
            var userType = model.UserType;

            if (userType == Constants.UserCategory.PROVIDER || userType == Constants.UserCategory.PHYSICIAN_ASSISTANT
                || userType == Constants.UserCategory.PHYSICIAN_ASSISTANT_SUPERVISED)
            {
                return true;
            }
            return false;
        }

        public static bool IsPOB(UserSettingsModel model)
        {
            var userType = model.UserType;

            if (userType == Constants.UserCategory.POB_SUPER || userType == Constants.UserCategory.POB_REGULAR
                || userType == Constants.UserCategory.POB_LIMITED)
            {
                return true;
            }
            return false;
        }

        public static List<UserSupervisingProviderModel> GetSupervisingProvidersList(string userID, string licenseID, ConnectionStringPointer dbID)
        {
            List<UserSupervisingProviderModel> selectSupervisingProvidersList = new List<UserSupervisingProviderModel>();
            var supervisingProviders = Allscripts.Impact.Provider.LoadProviderSelectionList(userID, licenseID, dbID);

            selectSupervisingProvidersList = (from DataRow dataRow in supervisingProviders.Rows
                                              select new UserSupervisingProviderModel()
                                              {
                                                  ProviderId = dataRow["ProviderId"].ToString(),
                                                  DEA = dataRow["DEANumber"].ToString(),
                                                  NPI = dataRow["NPI"].ToString(),
                                                  FirstName = dataRow["FirstName"].ToString(),
                                                  LastName = dataRow["LastName"].ToString(),
                                                  Selected = Convert.ToBoolean(dataRow["Selected"])
                                              }).ToList();
            return selectSupervisingProvidersList;
        }

        public static bool HasPOBSelectedAProvider(UserSettingsModel model)
        {
            bool isProviderSelectionCriteriaMet = false;
            if(model.SupervisingProviderSelectionMode == SupervisingProviderSelectionMode.All)
            {
                isProviderSelectionCriteriaMet = true;
            }
            else if (model.SupervisingProviderSelectionMode == SupervisingProviderSelectionMode.Select)
            {
                foreach(var provider in model.UserSupervisingProvidersList)
                {
                    if (provider.Selected)
                    {
                        isProviderSelectionCriteriaMet = true;
                        break;
                    }
                }
            }
            return isProviderSelectionCriteriaMet;
        }

        public static void AddRoleToAppRoles(eRxWeb.ePrescribeSvc.Role[] appRolesFromShield, UserSettingsModel model, List<string> appRoles)
        {
            switch (model.UserType)
            {
                case Constants.UserCategory.PROVIDER:
                    var providerRole = appRolesFromShield.Single(r => r.Name.ToLower() == "provider");
                    appRoles.Add(providerRole.RoleName);
                    break;

                case Constants.UserCategory.PHYSICIAN_ASSISTANT:
                    var paRole = appRolesFromShield.Single(r => r.Name.ToLower() == "physician assistant");
                    appRoles.Add(paRole.RoleName);
                    break;

                case Constants.UserCategory.PHYSICIAN_ASSISTANT_SUPERVISED:
                    var pasRole =
                        appRolesFromShield.Single(r => r.Name.ToLower() == "physician assistant supervised");
                    appRoles.Add(pasRole.RoleName);
                    break;

                case Constants.UserCategory.POB_SUPER:
                    var pobSuperRole =
                        appRolesFromShield.Single(r => r.Name.ToLower() == "pob - no review");
                    appRoles.Add(pobSuperRole.RoleName);
                    break;

                case Constants.UserCategory.POB_REGULAR:
                    var pobRegularRole =
                        appRolesFromShield.Single(r => r.Name.ToLower() == "pob - some review");
                    appRoles.Add(pobRegularRole.RoleName);
                    break;

                case Constants.UserCategory.POB_LIMITED:
                    var pobLimitedRole =
                        appRolesFromShield.Single(r => r.Name.ToLower() == "pob - all review");
                    appRoles.Add(pobLimitedRole.RoleName);
                    break;

                default:
                    var generalUserRole =
                            appRolesFromShield.Single(r => r.Name.ToLower() == "general user");
                    appRoles.Add(generalUserRole.RoleName);
                    break;
            }
        }

        private static int getUserType(string[] appRoles)
        {
            int userType = 0;

            foreach (string appRole in appRoles)
            {
                if (appRole.ToLower() == "admin")
                {
                    continue;
                }
                else
                {
                    if (appRole.ToLower() == "provider")
                    {
                        userType = (int)Constants.UserCategory.PROVIDER;
                    }
                    else if (appRole.ToLower() == "physician assistant")
                    {
                        userType = (int)Constants.UserCategory.PHYSICIAN_ASSISTANT;
                    }
                    else if (appRole.ToLower() == "physician assistant supervised")
                    {
                        userType = (int)Constants.UserCategory.PHYSICIAN_ASSISTANT_SUPERVISED;
                    }
                    else if (appRole.ToLower() == "pob - no review")
                    {
                        userType = (int)Constants.UserCategory.POB_SUPER;
                    }
                    else if (appRole.ToLower() == "pob - some review")
                    {
                        userType = (int)Constants.UserCategory.POB_REGULAR;
                    }
                    else if (appRole.ToLower() == "pob - all review")
                    {
                        userType = (int)Constants.UserCategory.POB_LIMITED;
                    }
                    else if (appRole.ToLower() == "general user")
                    {
                        userType = (int)Constants.UserCategory.GENERAL_USER;
                    }
                }
            }

            return userType;
        }
    }
}