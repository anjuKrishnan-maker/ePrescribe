using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Allscripts.ePrescribe.Test.AppCodeTests.CompulsoryBasicUtilTests
{
    public partial class CompulsoryBasicUtilTests
    {
        [TestClass]
        public class HasCompulsoryBasicStartedTests
        {
            [TestMethod]
            public void should_return_true_for_startdate_before_today()
            {
                //arrange
                DateTime startDate = DateTime.Now.AddDays(-1); //Before Current Date

                //act
                bool result = eRxWeb.AppCode.CompulsoryBasicUtil.HasCompulsoryBasicStarted(startDate);

                //assert
                bool expected = true;
                Assert.AreEqual(expected, result);
            }

            [TestMethod]
            public void should_return_false_for_startdate_after_today()
            {
                //arrange
                DateTime startDate = DateTime.Now.AddDays(1); //After Current Date

                //act
                bool result = eRxWeb.AppCode.CompulsoryBasicUtil.HasCompulsoryBasicStarted(startDate);

                //assert
                bool expected = false;
                Assert.AreEqual(expected, result);
            }
           
        }
    }
}