using System;
using Allscripts.Impact.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Allscripts.ePrescribe.Test.UtilitiesTests.StringHelperTests
{
    [TestClass]
    public class ConvertToUxNameTests
    {
        [TestMethod]
        public void should_make_lastname_uppercase()
        {
            //Arrange
            var lastFirst = "O'Brien, Ethan";

            //Act
            var uxVersion = StringHelper.ConvertToUxName(lastFirst);

            //Assert
            Assert.AreEqual("O'BRIEN, Ethan", uxVersion);
        }

        [TestMethod]
        public void should_not_make_firstname_uppercase()
        {
            //Arrange
            var lastFirst = "O'Brien, Ethan";

            //Act
            var uxVersion = StringHelper.ConvertToUxName(lastFirst);

            //Assert
            Assert.AreNotEqual("O'Brien, ETHAN", uxVersion);
        }

        [TestMethod]
        public void should_return_original_name_if_length_is_not_2_occurs()
        {
            //Arrange
            var lastFirst = "O'Brien Ethan";
            var lastFirstTwo = "Jim, Jones, Third";

            //Act
            var uxVersion = StringHelper.ConvertToUxName(lastFirst);
            var uxVersionTwo = StringHelper.ConvertToUxName(lastFirstTwo);

            //Assert
            Assert.AreEqual(lastFirst, uxVersion);
            Assert.AreEqual(lastFirstTwo, uxVersionTwo);
        }
    }
}
