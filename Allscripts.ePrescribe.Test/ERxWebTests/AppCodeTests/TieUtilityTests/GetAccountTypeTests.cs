using System;
using Allscripts.Impact;
using eRxWeb.AppCode;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Allscripts.ePrescribe.Test.ERxWebTests.AppCodeTests.TieUtilityTests
{
    [TestClass]
    public class GetAccountTypeTests
    {
        [TestMethod]
        public void should_return_platinum_if_feature_status_display_contains_platinum()
        {
            //act
            var result = TieUtility.GetAccountType("Platinum Status");

            //assert
            Assert.AreEqual("Platinum", result);
        }

        [TestMethod]
        public void should_return_deluxe_if_feature_status_display_contains_deluxe()
        {
            //act
            var result = TieUtility.GetAccountType("Deluxe Status");

            //assert
            Assert.AreEqual("Deluxe", result);
        }

        [TestMethod]
        public void should_return_basic_if_feature_status_display_does_not_contain_deluxe_or_platinum()
        {
            //act
            var result = TieUtility.GetAccountType("different status");

            //assert
            Assert.AreEqual("Basic", result);
        }
    }
}
