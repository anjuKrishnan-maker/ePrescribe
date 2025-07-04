using System.Web;
using eRxWeb.AppCode;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Allscripts.ePrescribe.Test.ERxWebTests.AppCodeTests.TieUtilityTests
{
    [TestClass]
    public class GetTargetKeysTests
    {
        [TestMethod]
        public void should_return_empty_dictionary_if_there_are_no_keys_in_the_cookie_collection()
        {
            //Arrange
            var cookie = new HttpCookie("TestCookieYum");

            //act
            var result = new TieUtility().GetTargetKeys(cookie);

            //assert
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void should_not_add_key_value_to_list_if_key_is_null_from_cookie()
        {
            //Arrange
            var cookie = new HttpCookie("TestCookieYum");
            cookie.Values.Add(null, "Value");

            //act
            var result = new TieUtility().GetTargetKeys(cookie);

            //assert
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void should_split_dea_numbers_from_cookie_and_add_it_to_dictionary_as_string_array()
        {
            //Arrange
            var cookie = new HttpCookie("TestCookieYum");
            var deaNumbers = "2,3,4,5";
            cookie.Values.Add("DEASchedule", deaNumbers);

            //act
            var result = new TieUtility().GetTargetKeys(cookie);

            //assert
            Assert.AreEqual("3", ((string[])result["DEASchedule"])[1]);
            Assert.AreEqual("5", ((string[])result["DEASchedule"])[3]);
            Assert.AreEqual("2", ((string[])result["DEASchedule"])[0]);
        }

        [TestMethod]
        public void should_add_key_and_value_to_dictionary_from_cookie_if_key_is_not_null()
        {
            //Arrange
            var cookie = new HttpCookie("TestCookieYum");
            cookie.Values.Add("AddMe", "YayAdded");

            //act
            var result = new TieUtility().GetTargetKeys(cookie);

            //assert
            Assert.AreEqual("YayAdded", ((string[])result["AddMe"])[0]);
        }
    }
}
