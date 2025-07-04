using Allscripts.Impact.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Allscripts.ePrescribe.Test.UtilitiesTests.StringHelperTests
{
    [TestClass]
   public class StripZeroAfterDeciamlTests
    {
        [TestMethod]
        public void should_return_int_when_i_pass_string_with_three_zeros()
        {
            string expectedResult = "10";

            //Act
            string actualResult = StringHelper.StripZeros("10.000");

            //Assert
            Assert.AreEqual(expectedResult, actualResult);
        }
        [TestMethod]
        public void should_return_int_when_i_pass_string_with_four_zeros()
        {
            string expectedResult = "10";

            //Act
            string actualResult = StringHelper.StripZeros("10.0000");

            //Assert
            Assert.AreEqual(expectedResult, actualResult);
        }
        [TestMethod]
        public void should_return_same_string_when_i_pass_string_with_five_zeros()
        {
            string expectedResult = "10.00000";

            //Act
            string actualResult = StringHelper.StripZeros("10.00000");

            //Assert
            Assert.AreEqual(expectedResult, actualResult);
        }
        [TestMethod]
        public void should_return_same_number_when_i_pass_string_with_mantissa()
        {
            string expectedResult = "10";

            //Act
            string actualResult = StringHelper.StripZeros("10.12345");

            //Assert
            Assert.AreNotEqual(expectedResult, actualResult);
        }
        [TestMethod]
        public void should_return_same_number_when_i_pass_string_without_zero()
        {
            string expectedResult = "10";

            //Act
            string actualResult = StringHelper.StripZeros("10");

            //Assert
            Assert.AreEqual(expectedResult, actualResult);
        }
    }
}
