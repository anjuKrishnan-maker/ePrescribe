using Allscripts.Impact.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Allscripts.ePrescribe.Test.UtilitiesTests.StringHelperTests
{
    [TestClass]
    public class StringHelperTests
    {
        [TestMethod]
        public void should_return_NotApplicable_if_empty_phoneNumber()
        {
            // Arrange
            const string expectedResult = "N/A";

            //Act
            string actualResult = StringHelper.FormatExternalPhoneNumberForDisplay("");

            //Assert
            Assert.AreEqual(expectedResult, actualResult);
        }

        [TestMethod]
        public void should_return_formatted_10Digit_phoneNumber_withoutExtension_if_phoneNumber_withoutextension()
        {
            // Arrange
            const string expectedResult = "(800) 888-8888";

            //Act
            string actualResult = StringHelper.FormatExternalPhoneNumberForDisplay("(8008888888");

            //Assert
            Assert.AreEqual(expectedResult, actualResult);
        }

        [TestMethod]
        public void should_return_formatted_10Digit_phoneNumber_withExtension_if_phoneNumber_withextension()
        {
            // Arrange
            const string expectedResult = "(800) 888-8888 x3220";

            //Act
            string actualResult = StringHelper.FormatExternalPhoneNumberForDisplay("(8008888888x3220");

            //Assert
            Assert.AreEqual(expectedResult, actualResult);
        }

        [TestMethod]
        public void should_return_formatted_7Digit_phoneNumber_withoutExtension_if_phoneNumber_withoutextension()
        {
            // Arrange
            const string expectedResult = "888-8888";

            //Act
            string actualResult = StringHelper.FormatExternalPhoneNumberForDisplay("(8888888");

            //Assert
            Assert.AreEqual(expectedResult, actualResult);
        }

        [TestMethod]
        public void should_return_formatted_7Digit_phoneNumber_withExtension_if_phoneNumber_withextension()
        {
            // Arrange
            const string expectedResult = "888-8888 x3220";

            //Act
            string actualResult = StringHelper.FormatExternalPhoneNumberForDisplay("(8888888x3220");

            //Assert
            Assert.AreEqual(expectedResult, actualResult);
        }

        [TestMethod]
        public void should_return__phoneNumber_passedvalue_if_phoneNumber_is_not_valid()
        {
            // Arrange
            const string expectedResult = "2344555aaa55555";

            //Act
            string actualResult = StringHelper.FormatExternalPhoneNumberForDisplay("(2344555aaa55555");

            //Assert
            Assert.AreEqual(expectedResult, actualResult);
        }

         [TestMethod]
        public void should_return_default_size_if_parent_size_is_less_than_default_size()
        {
             // Arrange
           
            //Act
            string actualResult = StringHelper.GetRelativePixelSizeAgainstGivenSize(200,300,100);

            //Assert
            Assert.AreEqual("300px", actualResult);
        }


         [TestMethod]
        public void should_return_relative_size_calculated_on_parent_size_and_offset()
        {
            // Arrange
           
            //Act
            string actualResult = StringHelper.GetRelativePixelSizeAgainstGivenSize(450,300,100);

            //Assert
            Assert.AreEqual("350px", actualResult);
        }

        [TestMethod]
        public void should_return_proper_format_PhoneNumber()
        {
            // Arrange
            bool mobile = false;

            //Assert 
            Assert.AreEqual(true, StringHelper.IsValidMobile(mobile, "2128970067"));
            Assert.AreEqual(true, StringHelper.IsValidMobile(mobile, "(212)897-0067"));
            Assert.AreEqual(true, StringHelper.IsValidMobile(mobile, "1(212)897-0067"));
            Assert.AreEqual(false, StringHelper.IsValidMobile(mobile, "001(212)897-0067"));
            Assert.AreEqual(false, StringHelper.IsValidMobile(mobile, "001.212.897.0067"));
            Assert.AreEqual(false, StringHelper.IsValidMobile(mobile, "001(212)897.00671234"));
            Assert.AreEqual(false, StringHelper.IsValidMobile(mobile, "001 212 897 0067"));
        }

        [TestMethod]
        public void should_create_random_code()
        {
            
            //Act            
            int actualResult = StringHelper.CreateRandomCode().Length;
            //Assert
            Assert.AreEqual(4, actualResult);           
            
        }

        [TestMethod]
        public void should_return_refreq_formatted_rx_details()
        {
            //arrange 
            var expected = "Amphetamine-Dextroamphetamine 10 MG Oral Tablet, SIG: TAKE 1 TABLET DAILY., Quantity: 15, Days Supply: 15, Fills: 15, Original Rx Date: 7/15/2019, Last Fill Date: 7/15/2019";

            //act
            var result = StringHelper.GetRxDetails("Amphetamine-Dextroamphetamine 10 MG Oral Tablet", "TAKE 1 TABLET DAILY.", "15", "15", "15", "Y", "7/15/2019", "7/15/2019", null);

            //assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void should_return_refreq_formatted_rx_details_with_daw()
        {
            //arrange 
            var expected = "Amphetamine-Dextroamphetamine 10 MG Oral Tablet, SIG: TAKE 1 TABLET DAILY., Quantity: 15, Days Supply: 15, Fills: 15, DAW, Original Rx Date: 7/15/2019, Last Fill Date: 7/15/2019";

            //act
            var result = StringHelper.GetRxDetails("Amphetamine-Dextroamphetamine 10 MG Oral Tablet", "TAKE 1 TABLET DAILY.", "15", "15", "15", "1", "7/15/2019", "7/15/2019", null);

            //assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void should_return_refreq_formatted_rx_details_with_notes()
        {
            //arrange 
            var expected = "Amphetamine-Dextroamphetamine 10 MG Oral Tablet, SIG: TAKE 1 TABLET DAILY., Quantity: 15, Days Supply: 15, Fills: 15, Original Rx Date: 7/15/2019, Last Fill Date: 7/15/2019, Notes: These are notes";

            //act
            var result = StringHelper.GetRxDetails("Amphetamine-Dextroamphetamine 10 MG Oral Tablet", "TAKE 1 TABLET DAILY.", "15", "15", "15", "Y", "7/15/2019", "7/15/2019", "These are notes");

            //assert
            Assert.AreEqual(expected, result);
        }
    }
}
