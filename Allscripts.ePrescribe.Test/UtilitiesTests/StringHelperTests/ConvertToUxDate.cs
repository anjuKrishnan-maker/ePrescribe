using System;
using Allscripts.Impact.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Allscripts.ePrescribe.Test.UtilitiesTests.StringHelperTests
{
    [TestClass]
    public class ConvertToUxDate
    {
        [TestMethod]
        public void should_convert_datetime_to_ux_date()
        {
            //Arrange
            var orgDate = new DateTime(1990, 06, 27);

            //Act
            var uxDate = StringHelper.ConvertToUxDate(orgDate);

            //Assert
            Assert.AreEqual("27-Jun-1990", uxDate);
        }

        [TestMethod]
        public void should_convert_string_of_datetime_to_ux_date()
        {
            //Arrange
            var orgDate = new DateTime(1990, 06, 27);

            //Act
            var uxDate = StringHelper.ConvertToUxDate(orgDate.ToString("MM-dd-yyyy"));

            //Assert
            Assert.AreEqual("27-Jun-1990", uxDate);
        }

        [TestMethod]
        public void should_return_original_input_as_string_if_the_input_cannot_be_converted_to_datetime()
        {
            //Arrange
            var orgDate = 6;

            //Act
            var uxDate = StringHelper.ConvertToUxDate(orgDate);

            //Assert
            Assert.AreEqual("6", uxDate);
        }
    }
}
