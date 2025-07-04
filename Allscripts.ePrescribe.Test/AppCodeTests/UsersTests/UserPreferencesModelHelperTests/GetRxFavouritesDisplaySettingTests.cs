using eRxWeb.AppCode.Users;
using eRxWeb.ServerModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Allscripts.ePrescribe.Test.AppCodeTests.UsersTests.UserPreferencesModelHelperTests
{
    [TestClass]
    public class GetRxFavouritesDisplaySettingTests
    {
        [TestMethod]
        public void should_return_true_when_user_mode_self_edit()
        {
            UserMode mode = UserMode.SelfEdit;

            bool isShouldShowRxFavourites = UserPreferencesModelHelper.GetRxFavouritesDisplaySetting(mode);

            Assert.IsTrue(isShouldShowRxFavourites);
        }

        [TestMethod]
        public void should_return_false_when_user_mode_is_edit_other_user()
        {
            UserMode mode = UserMode.EditOtherUser;

            bool isShouldShowRxFavourites = UserPreferencesModelHelper.GetRxFavouritesDisplaySetting(mode);

            Assert.IsFalse(isShouldShowRxFavourites);
        }

        [TestMethod]
        public void should_return_false_when_user_mode_is_add_user()
        {
            UserMode mode = UserMode.AddOtherUser;

            bool isShouldShowRxFavourites = UserPreferencesModelHelper.GetRxFavouritesDisplaySetting(mode);

            Assert.IsFalse(isShouldShowRxFavourites);
        }
    }
}
