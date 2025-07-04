using eRxWeb.AppCode.Users;
using eRxWeb.ServerModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Allscripts.ePrescribe.Test.AppCodeTests.UsersTests.UserSecurityModelHelperTests
{
    [TestClass]
    public class CalculateIfChangePasswordTests
    {
        [TestMethod]
        public void should_return_true_when_nonsso_and_enterprise_true_for_self_edit()
        {
            //arrange
            bool isSsoUser = false;
            bool isPartnerAllowsUserNameAndPassword = true;
            bool isForcePasswordSetupForSSOUser = true;
            bool isEnterpriseShowChangePassword = true;
            UserMode userMode = UserMode.SelfEdit;

            //act
            bool actual = UserSecurityModelHelper.CalculateIfChangePasswordLinkShouldBeShown(isSsoUser, isPartnerAllowsUserNameAndPassword,
                isForcePasswordSetupForSSOUser, isEnterpriseShowChangePassword, userMode);

            //assert
            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void should_return_false_when_nonsso_and_enterprise_false_for_self_edit()
        {
            //arrange
            bool isSsoUser = false;
            bool isPartnerAllowsUserNameAndPassword = true;
            bool isForcePasswordSetupForSSOUser = true;
            bool isEnterpriseShowChangePassword = false;
            UserMode userMode = UserMode.SelfEdit;

            //act
            bool actual = UserSecurityModelHelper.CalculateIfChangePasswordLinkShouldBeShown(isSsoUser, isPartnerAllowsUserNameAndPassword,
                isForcePasswordSetupForSSOUser, isEnterpriseShowChangePassword, userMode);

            //assert
            Assert.IsFalse(actual);
        }

        [TestMethod]
        public void should_return_false_when_is_sso_and_enterprise_false_for_self_edit()
        {
            //arrange
            bool isSsoUser = true;
            bool isPartnerAllowsUserNameAndPassword = true;
            bool isForcePasswordSetupForSSOUser = true;
            bool isEnterpriseShowChangePassword = false;
            UserMode userMode = UserMode.SelfEdit;

            //act
            bool actual = UserSecurityModelHelper.CalculateIfChangePasswordLinkShouldBeShown(isSsoUser, isPartnerAllowsUserNameAndPassword,
                isForcePasswordSetupForSSOUser, isEnterpriseShowChangePassword, userMode);

            //assert
            Assert.IsFalse(actual);
        }

        [TestMethod]
        public void should_return_false_when_is_sso_and_partner_allowed_false_for_self_edit()
        {
            //arrange
            bool isSsoUser = true;
            bool isPartnerAllowsUserNameAndPassword = false;
            bool isForcePasswordSetupForSSOUser = true;
            bool isEnterpriseShowChangePassword = true;
            UserMode userMode = UserMode.SelfEdit;

            //act
            bool actual = UserSecurityModelHelper.CalculateIfChangePasswordLinkShouldBeShown(isSsoUser, isPartnerAllowsUserNameAndPassword,
                isForcePasswordSetupForSSOUser, isEnterpriseShowChangePassword, userMode);

            //assert
            Assert.IsFalse(actual);
        }

        [TestMethod]
        public void should_return_false_when_is_sso_and_force_password_setup_is_true_for_self_edit()
        {
            //arrange
            bool isSsoUser = true;
            bool isPartnerAllowsUserNameAndPassword = true;
            bool isForcePasswordSetupForSSOUser = true;
            bool isEnterpriseShowChangePassword = true;
            UserMode userMode = UserMode.SelfEdit;

            //act
            bool actual = UserSecurityModelHelper.CalculateIfChangePasswordLinkShouldBeShown(isSsoUser, isPartnerAllowsUserNameAndPassword,
                isForcePasswordSetupForSSOUser, isEnterpriseShowChangePassword, userMode);

            //assert
            Assert.IsFalse(actual);
        }

        [TestMethod]
        public void should_return_true_when_sso_partnerallowed_enterprise_are_true_and_force_password_is_false_for_self_edit()
        {
            //arrange
            bool isSsoUser = true;
            bool isPartnerAllowsUserNameAndPassword = true;
            bool isForcePasswordSetupForSSOUser = false;
            bool isEnterpriseShowChangePassword = true;
            UserMode userMode = UserMode.SelfEdit;

            //act
            bool actual = UserSecurityModelHelper.CalculateIfChangePasswordLinkShouldBeShown(isSsoUser, isPartnerAllowsUserNameAndPassword,
                isForcePasswordSetupForSSOUser, isEnterpriseShowChangePassword, userMode);

            //assert
            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void should_return_false_for_edit_other_user()
        {
            //arrange
            bool isSsoUser = true;
            bool isPartnerAllowsUserNameAndPassword = true;
            bool isForcePasswordSetupForSSOUser = false;
            bool isEnterpriseShowChangePassword = true;
            UserMode userMode = UserMode.EditOtherUser;

            //act
            bool actual = UserSecurityModelHelper.CalculateIfChangePasswordLinkShouldBeShown(isSsoUser, isPartnerAllowsUserNameAndPassword,
                isForcePasswordSetupForSSOUser, isEnterpriseShowChangePassword, userMode);

            //assert
            Assert.IsFalse(actual);
        }

        [TestMethod]
        public void should_return_false_for_add_other_user()
        {
            //arrange
            bool isSsoUser = true;
            bool isPartnerAllowsUserNameAndPassword = true;
            bool isForcePasswordSetupForSSOUser = false;
            bool isEnterpriseShowChangePassword = true;
            UserMode userMode = UserMode.AddOtherUser;

            //act
            bool actual = UserSecurityModelHelper.CalculateIfChangePasswordLinkShouldBeShown(isSsoUser, isPartnerAllowsUserNameAndPassword,
                isForcePasswordSetupForSSOUser, isEnterpriseShowChangePassword, userMode);

            //assert
            Assert.IsFalse(actual);
        }
    }
}
