using System;
using eRxWeb.AppCode;
using eRxWeb.AppCode.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Allscripts.ePrescribe.Test.ERxWebTests.AppCodeTests.TieUtilityTests
{
    [TestClass]
    public class ParseAbsoluteUrlForPageNameTests
    {
        public ITieUtility TieUtility => new TieUtility();

        [TestMethod]
        public void should_return_page_name_from_absolute_url_path_of_default()
        {
            //arrange
            var urlAbsolutePath = "/SelectPatient";

            //act
            var result = TieUtility.ParseAbsoluteUrlForPageName(urlAbsolutePath);

            //assert
            Assert.AreEqual("selectpatient", result);
        }

        [TestMethod]
        public void should_return_page_name_from_absolute_url_path_of_addPatient()
        {
            //arrange
            var urlAbsolutePath = "/AddPatient.aspx";

            //act
            var result = TieUtility.ParseAbsoluteUrlForPageName(urlAbsolutePath);

            //assert
            Assert.AreEqual("addpatient", result);
        }
    }
}
