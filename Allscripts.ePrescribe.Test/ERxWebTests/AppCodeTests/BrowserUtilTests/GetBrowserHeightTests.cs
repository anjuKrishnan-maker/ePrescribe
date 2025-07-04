using System;
using eRxWeb.AppCode;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Allscripts.ePrescribe.Test.ERxWebTests.AppCodeTests.BrowserUtilTests
{
    [TestClass]
    public class GetBrowserHeightTests
    {
        [TestMethod]
        public void should_return_height_of_1000_if_rawHeight_is_null()
        {
            //act
            var result = BrowserUtil.GetBrowserHeight(null);

            //assert
            Assert.AreEqual(1000, result);
        }

        [TestMethod]
        public void should_return_height_of_1000_if_rawHeight_is_empty()
        {
            //act
            var result = BrowserUtil.GetBrowserHeight(string.Empty);

            //assert
            Assert.AreEqual(1000, result);
        }

        [TestMethod]
        public void should_return_height_of_1000_if_rawHeight_is_not_a_number()
        {
            //act
            var result = BrowserUtil.GetBrowserHeight("NotANumber");

            //assert
            Assert.AreEqual(1000, result);
        }

        [TestMethod]
        public void should_return_height_of_1000_if_rawHeight_is_less_than_505()
        {
            //act
            var result = BrowserUtil.GetBrowserHeight("504");

            //assert
            Assert.AreEqual(1000, result);
        }

        [TestMethod]
        public void should_return_int_rawHeight_if_rawHeight_is_equal_to_505()
        {
            //act
            var result = BrowserUtil.GetBrowserHeight("505");

            //assert
            Assert.AreEqual(505, result);
        }

        [TestMethod]
        public void should_return_int_rawHeight_if_rawHeight_is_greater_than_505()
        {
            //act
            var result = BrowserUtil.GetBrowserHeight("781");

            //assert
            Assert.AreEqual(781, result);
        }
    }
}
