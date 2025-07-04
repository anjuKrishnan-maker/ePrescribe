using eRxWeb.AppCode.Users;
using eRxWeb.ServerModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Allscripts.ePrescribe.Test.AppCodeTests.UsersTests.UserSecurityModelHelperTests
{
    [TestClass]
    public class CalculateIfEditSecretQuestionsLinkTests
    {
        [TestMethod]
        public void should_return_true_when_user_mode_self_edit()
        {
            UserMode mode = UserMode.SelfEdit;
            bool isEnterpriseShowChangePassword = true;

            bool isShouldShowEditSecretQuestionsLink = UserSecurityModelHelper.CalculateIfEditSecretQuestionsLinkShouldBeShown(mode, isEnterpriseShowChangePassword);

            Assert.IsTrue(isShouldShowEditSecretQuestionsLink);
        }

        [TestMethod]
        public void should_return_false_when_user_mode_is_edit_other_user()
        {
            UserMode mode = UserMode.EditOtherUser;
            bool isEnterpriseShowChangePassword = true;

            bool isShouldShowEditSecretQuestionsLink = UserSecurityModelHelper.CalculateIfEditSecretQuestionsLinkShouldBeShown(mode, isEnterpriseShowChangePassword);

            Assert.IsFalse(isShouldShowEditSecretQuestionsLink);
        }

        [TestMethod]
        public void should_return_false_when_user_mode_is_add_user()
        {
            UserMode mode = UserMode.AddOtherUser;
            bool isEnterpriseShowChangePassword = true;

            bool isShouldShowEditSecretQuestionsLink = UserSecurityModelHelper.CalculateIfEditSecretQuestionsLinkShouldBeShown(mode, isEnterpriseShowChangePassword);

            Assert.IsFalse(isShouldShowEditSecretQuestionsLink);
        }
    }
}
