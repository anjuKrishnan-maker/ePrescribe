using Allscripts.Impact;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Allscripts.ePrescribe.Test.EditUserPageTests
{
    public class EditUserTests
    {
        [TestClass]
        public class UserPreferenceTests
        {
            [TestMethod]
            public void should_return_UserValue_Y_when_preferenceUserValue_true()
            {
                //arrange
                PreferenceCategory preferenceCategory = new PreferenceCategory();

                //act
                Preference preference = Preference.GetPreference(true, preferenceCategory,"");

                //assert
                string result = preference.UserValue;

                Assert.AreEqual("Y", result);
            }

            [TestMethod]
            public void should_return_UserValue_N_when_preferenceUserValue_false()
            {
                //arrange
                PreferenceCategory preferenceCategory = new PreferenceCategory();

                //act
                Preference preference = Preference.GetPreference(false, preferenceCategory, "");

                //assert
                string result = preference.UserValue;

                Assert.AreEqual("N", result);
            }
        }
    }
}
