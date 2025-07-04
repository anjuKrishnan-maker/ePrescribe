using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Allscripts.ePrescribe.Objects;
using eRxWeb.AppCode;
using eRxWeb.State;
using Allscripts.ePrescribe.Common;

namespace Allscripts.ePrescribe.Test.ERxWebTests.AppCodeTests.SpecialtyMedTests
{
    [TestClass]
    public class GetNPIUnitTests
    {
        [TestMethod]
        public void should_return_Empty_String_if_Session_Null()
        {
            //Arrange
            string expectedVal = string.Empty;

            //Act
            string actualVal = new SpecialtyMed().GetNPI(null);

            //Assert
            Assert.AreEqual(expectedVal, actualVal);
        }
        
    }
}
