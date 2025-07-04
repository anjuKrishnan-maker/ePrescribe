using System;
using eRxWeb.AppCode;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Allscripts.ePrescribe.Test.ERxWebTests.AppCodeTests.DateTimeHelperTests
{
    [TestClass]
    public class GetValidFutureDateOrTodayTests
    {
        [TestMethod]
        public void should_return_todays_date_if_object_is_null()
        {
            //arrange
            object date = null;

            //act
            var result = DateTimeHelper.GetValidFutureDateOrToday(date);

            //assert
            Assert.AreEqual(DateTime.Today, result);
        }

        [TestMethod]
        public void should_return_todays_date_if_object_is_empty()
        {
            //arrange
            object date = "";

            //act
            var result = DateTimeHelper.GetValidFutureDateOrToday(date);

            //assert
            Assert.AreEqual(DateTime.Today, result);
        }

        [TestMethod]
        public void should_return_todays_date_if_object_is_numbers()
        {
            //arrange
            object date = "3423423";

            //act
            var result = DateTimeHelper.GetValidFutureDateOrToday(date);

            //assert
            Assert.AreEqual(DateTime.Today, result);
        }

        [TestMethod]
        public void should_return_todays_date_if_object_is_alphanumeric()
        {
            //arrange
            object date = "832kkds89923";

            //act
            var result = DateTimeHelper.GetValidFutureDateOrToday(date);

            //assert
            Assert.AreEqual(DateTime.Today, result);
        }

        [TestMethod]
        public void should_return_correct_date()
        {
            //arrange
            object date = DateTime.Today.AddDays(+20).ToString("d");

            //act
            var result = DateTimeHelper.GetValidFutureDateOrToday(date);

            //assert
            Assert.AreEqual(DateTime.Today.AddDays(+20), result);
        }
    }
}
