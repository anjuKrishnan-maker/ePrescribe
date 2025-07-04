using eRxWeb.ServerModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace eRxWeb.AppCode.Users
{
    public static class UserSecurityModelHelper
    {
        public static UserSecurityModel SetUserSecurityModel(bool isEditorAdmin, UserMode userMode, ePrescribeSvc.RxUser rxUser, bool isSsoUser, 
                        bool isPartnerAllowsUserNameAndPassword, bool isForcePasswordSetupForSSOUser, bool isEnterpriseShowChangePassword)
        {
            var model = new UserSecurityModel();            
            bool isEditeeAdmin = userMode == UserMode.EditOtherUser ? rxUser.AppRoles.Contains("Admin") : isEditorAdmin;

            switch (userMode)
            {
                case UserMode.SelfEdit:
                    model.IsAdmin = isEditorAdmin;
                    model.IsAdminEnabled = false;
                    break;
                case UserMode.AddOtherUser:
                    model.IsAdmin = false;
                    model.IsAdminEnabled = true;
                    break;
                case UserMode.EditOtherUser:
                    model.IsAdmin = isEditeeAdmin;
                    model.IsAdminEnabled = true;
                    break;
            }

           if(CalculateIfChangePasswordLinkShouldBeShown(isSsoUser, isPartnerAllowsUserNameAndPassword, isForcePasswordSetupForSSOUser, 
                        isEnterpriseShowChangePassword, userMode))
            {
                model.IsShowChangePassword = true;

            }

            if (CalculateIfEditSecretQuestionsLinkShouldBeShown(userMode, isEnterpriseShowChangePassword))
            {
                model.IsShowEditSecretQuestions = true;
                model.SecretQuestionsList = EPSBroker.GetAllShieldSecretQuestions().ToArray();
                model.UserSecretQuestions = EPSBroker.GetUserShieldSecretQuestionsByUsername(rxUser.ShieldUserName);
            }
            return model;
        }

        public static bool CalculateIfEditSecretQuestionsLinkShouldBeShown(UserMode userMode, bool enterprisePasswordSetting)
        {
            return userMode == UserMode.SelfEdit && enterprisePasswordSetting;
        }

        public static bool CalculateIfChangePasswordLinkShouldBeShown(bool isSsoUser, bool isPartnerAllowsUserNameAndPassword, bool isForcePasswordSetupForSSOUser, bool isEnterpriseShowChangePassword, UserMode userMode)
        {
            bool isShowChangePasswordLink = false;

            if (userMode == UserMode.SelfEdit)
            {
                if (isSsoUser)
                {
                    isShowChangePasswordLink = isEnterpriseShowChangePassword && isPartnerAllowsUserNameAndPassword && !isForcePasswordSetupForSSOUser;
                }
                else
                {
                    isShowChangePasswordLink = isEnterpriseShowChangePassword;
                }
            }

            return isShowChangePasswordLink;
        }

        public static bool IsUserAdminChecked(UserSecurityModel model)
        {
            return model.IsAdmin;
        }

        public static void CheckAndAddAdminRole(eRxWeb.ePrescribeSvc.Role adminRole, List<string> appRoles, UserSecurityModel model)
        {
            if (model.IsAdmin)
            {
                appRoles.Add(adminRole.RoleName);
            }
        }
    }
}