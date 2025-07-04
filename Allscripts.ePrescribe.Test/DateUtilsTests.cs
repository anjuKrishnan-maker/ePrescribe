using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Allscripts.ePrescribe.Test
{
    [TestClass]
    public class DateUtilsTests
    {
        [TestClass]
        public class IsFirstBusinessDayOfMonthTests
        {
            [TestMethod]
            public void should_return_true_for_first_business_day_of_month_feb_first_2016()
            {
                //arrange
                DateTime testDate = new DateTime(2016, 2, 1); //First of Feb 2016 is First Business Working Day

                //act
                bool result = eRxWeb.AppCode.DateUtils.IsFirstBusinessDayOfMonth(testDate);

                //assert
                bool expected = true;
                Assert.AreEqual(expected, result);
            }

            [TestMethod]
            public void should_return_false_for_not_first_business_day_of_month_feb_third_2016()
            {
                //arrange
                DateTime testDate = new DateTime(2016, 2, 3); //Third of Feb 2016 is not First Business Working Day

                //act
                bool result = eRxWeb.AppCode.DateUtils.IsFirstBusinessDayOfMonth(testDate);

                //assert
                bool expected = false;
                Assert.AreEqual(expected, result);
            }
        }       
    }
}
