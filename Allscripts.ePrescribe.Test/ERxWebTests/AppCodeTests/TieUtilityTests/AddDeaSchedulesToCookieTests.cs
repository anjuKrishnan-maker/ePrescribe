using System;
using System.Collections.Generic;
using System.Web;
using eRxWeb.AppCode;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Allscripts.ePrescribe.Test.ERxWebTests.AppCodeTests.TieUtilityTests
{
    [TestClass]
    public class AddDeaSchedulesToCookieTests
    {
        [TestMethod]
        public void should_add_value_of_string_empty_if_schedules_list_is_null()
        {
            //arrange
            var eRxTieCookie = new HttpCookie("CocoChip");
            
            //act
            TieUtility.AddDeaSchedulesToCookie(ref eRxTieCookie, null);

            //assert
            Assert.AreEqual(eRxTieCookie["DEASchedule"], string.Empty);
        }

        [TestMethod]
        public void should_add_value_of_string_empty_if_schedules_list_is_empty()
        {
            //arrange
            var eRxTieCookie = new HttpCookie("CocoChip");

            //act
            TieUtility.AddDeaSchedulesToCookie(ref eRxTieCookie, new List<string>());

            //assert
            Assert.AreEqual(eRxTieCookie["DEASchedule"], string.Empty);
        }

        [TestMethod]
        public void should_add_schedules_to_cookie()
        {
            //arrange
            var eRxTieCookie = new HttpCookie("CocoChip");
            var schedules = new List<string> {"2", "3", "5"};

            //act
            TieUtility.AddDeaSchedulesToCookie(ref eRxTieCookie, schedules);

            //assert
            Assert.AreEqual(eRxTieCookie["DEASchedule"], "2,3,5");
        }
    }
}
