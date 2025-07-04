using System.Collections.Generic;
using Allscripts.ePrescribe.Common;
using eRxWeb.AppCode;
using eRxWeb.ePrescribeSvc;
using eRxWeb.State;
using ShieldTraitName = eRxWeb.ePrescribeSvc.ShieldTraitName;

namespace eRxWeb
{
    public static class Users
    {

        public static bool SetTraitForUser(string shieldUserName, ShieldTraitName shieldTraitName, ShieldTraitValue shieldTraitValue,
            string appInstance, string shieldSecurityToken)
        {
            var trait = new ShieldTraitInfo
            {
                TraitName = shieldTraitName,
                TraitValueEnum = shieldTraitValue
            };

            var result = EPSBroker.SetTraitForUser(shieldUserName, trait,
                appInstance, shieldSecurityToken);

            return result;
        }

        public static bool SetTraitForMultipleUsers(List<UserNameWithUserGuidPair> providersList,
     ShieldTraitName traitName, ShieldTraitValue traitValue, string appInstance, string shieldSecurityToken, string otpToken, string identityToken, string licenseID, string UserID, bool isInstitutional)
        {
            ShieldTraitInfo traitInfo = new ShieldTraitInfo { TraitName = traitName, TraitValueEnum = traitValue };
            return EPSBroker.SetTraitForMultipleProviders(providersList, traitInfo, appInstance, identityToken, otpToken, shieldSecurityToken, licenseID, UserID, isInstitutional);
        }


        public static bool UpdateUserIdproofingStatus(IStateContainer pageState)
        {
            string shieldSecurityToken = pageState.GetStringOrEmpty(Constants.SessionVariables.ShieldSecurityToken);
            string shieldExteralTenantId = pageState.GetStringOrEmpty(Constants.SessionVariables.ShieldExternalTenantID);
            string userId = ApiHelper.GetSessionUserID(pageState);
            string licenseId = ApiHelper.GetSessionLicenseID(pageState);
            var dbid = ApiHelper.GetDBID(pageState);

            GetUserShieldCspStatusInfoResponse getUserShieldCspStatusInfoResponse = EPSBroker.GetUserShieldCspStatusInfo(dbid, shieldSecurityToken, shieldExteralTenantId, userId, licenseId);

            int requiredIdProofingLevel = AppCode.StateUtils.UserInfo.GetIdProofingMode(pageState) == ePrescribeSvc.ShieldTenantIDProofingModel.Institutional ? 1 : 3;
            int.TryParse(getUserShieldCspStatusInfoResponse.IdMeLOAStatusLevel, out int userIdProofingLevel);

            if (userIdProofingLevel >= requiredIdProofingLevel)
            {
                Allscripts.ePrescribe.Data.User.InsertUserIdMeCspLevelOfAssurance(userId, userIdProofingLevel.ToString(), true, dbid);
                return true;
            }

            return false;
        }
    }
}