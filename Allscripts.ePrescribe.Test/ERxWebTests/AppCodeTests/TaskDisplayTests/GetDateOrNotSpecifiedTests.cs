using System;
using Allscripts.ePrescribe.Common;
using eRxWeb.AppCode.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Allscripts.ePrescribe.Test.ERxWebTests.AppCodeTests.TaskDisplayTests
{
    [TestClass]
    public class GetDateOrNotSpecifiedTests
    {
        [TestMethod]
        public void should_return_NoSpecified_if_date_is_null()
        {
            //act
            var result = TaskDisplay.GetDateOrNotSpecified(null);

            //assert
            Assert.AreEqual(CommonTerms.NotSpecified, result);
        }

        [TestMethod]
        public void should_return_NoSpecified_if_date_is_whitespace()
        {
            //act
            var result = TaskDisplay.GetDateOrNotSpecified("  ");

            //assert
            Assert.AreEqual(CommonTerms.NotSpecified, result);
        }

        [TestMethod]
        public void should_return_NoSpecified_if_date_minValue()
        {
            //act
            var result = TaskDisplay.GetDateOrNotSpecified("NotADate");

            //assert
            Assert.AreEqual(CommonTerms.NotSpecified, result);
        }

        [TestMethod]
        public void should_return_shortdate()
        {
            //act
            var result = TaskDisplay.GetDateOrNotSpecified("12/15/2018 16:43:00");

            //assert
            Assert.AreEqual("12/15/2018", result);
        }
    }
}
