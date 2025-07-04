using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using eRxWeb.AppCode;
using Allscripts.Impact.services.SpecialtyMedUtils;
using eRxWeb;

namespace Allscripts.ePrescribe.Test.ERxWebTests.AppCodeTests.SpecialtyMedTests
{
    [TestClass]
    public class RetrieveFormNameUnitTests
    {
        [TestMethod]
        public void should_return_generic_name_if_an_invalid_docType_supplied()
        {
            //Arrange
            SpecialtyMedTasks specialtyMedTasks = new SpecialtyMedTasks();
            string expectedFileName = "PatientForm.pdf";


            //Act
            string actualFileName = specialtyMedTasks.RetrieveFormName((AttachmentTypes)3);

            //Assert
            Assert.AreEqual(expectedFileName, actualFileName);
        }
        [TestMethod]
        public void should_return_expected_name_if_a_valid_docType_supplied()
        {
            //Arrange
            SpecialtyMedTasks specialtyMedTasks = new SpecialtyMedTasks();
            string expectedFileNamePriorAuth = "PriorAuthorizationForm.pdf";
            string expectedFileNameSummaryReport = "SpecialtyMedEnrollmentForm.pdf";


            //Act
            string actualFileNamePriorAuth = specialtyMedTasks.RetrieveFormName(AttachmentTypes.PRIOR_AUTHORIZATION_FILE);
            string actualFileNameSummaryReport = specialtyMedTasks.RetrieveFormName(AttachmentTypes.SPECIALTY_ENROLLMENT_STATUS_FILE);

            //Assert
            Assert.AreEqual(actualFileNamePriorAuth, expectedFileNamePriorAuth);
            Assert.AreEqual(actualFileNameSummaryReport, expectedFileNameSummaryReport);
        }
    }
}
