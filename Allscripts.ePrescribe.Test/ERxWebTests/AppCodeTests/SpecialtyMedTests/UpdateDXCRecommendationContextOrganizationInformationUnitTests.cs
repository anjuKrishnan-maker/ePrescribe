using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using eRxWeb.AppCode;
using eRxWeb.AppCode.SpecialtyMedWorkflow;
using System.Data;

namespace Allscripts.ePrescribe.Test.ERxWebTests.AppCodeTests.SpecialtyMedTests
{
    [TestClass]
    public class UpdateDXCRecommendationContextOrganizationInformationUnitTests
    {
        [TestMethod]
        public void should_update_with_Empty_strings_if_Null_DataTable_supplied()
        {
            //Arrange
            DXCRecommendationContext context = new DXCRecommendationContext();
            SpecialtyMed specialtyMedsWorkflow = new SpecialtyMed();

            //Act
            specialtyMedsWorkflow.UpdateDXCRecommendationContextOrganizationInformation(null/*REAL TEST*/, 0, ref context);


            //Assert 
            Assert.AreEqual(string.Empty, context.OrganizationName);
            Assert.AreEqual(string.Empty, context.OrganizationStreetAddress1);
            Assert.AreEqual(string.Empty, context.OrganizationStreetAddress2);
            Assert.AreEqual(string.Empty, context.OrganizationCity);
            Assert.AreEqual(string.Empty, context.OrganizationState);
            Assert.AreEqual(string.Empty, context.OrganizationPostalCode);
            Assert.AreEqual(string.Empty, context.OrganizationPhone);
        }

        [TestMethod]
        public void should_update_with_Empty_strings_if_DataTable_supplied_with_zero_rows()
        {
            //Arrange
            DXCRecommendationContext context = new DXCRecommendationContext();
            SpecialtyMed specialtyMedsWorkflow = new SpecialtyMed();
            DataTable patientsTable = new DataTable();


            //Act
            specialtyMedsWorkflow.UpdateDXCRecommendationContextOrganizationInformation(patientsTable/*REAL TEST*/, 0,  ref context);


            //Assert 
            Assert.AreEqual(string.Empty, context.OrganizationName);
            Assert.AreEqual(string.Empty, context.OrganizationStreetAddress1);
            Assert.AreEqual(string.Empty, context.OrganizationStreetAddress2);
            Assert.AreEqual(string.Empty, context.OrganizationCity);
            Assert.AreEqual(string.Empty, context.OrganizationState);
            Assert.AreEqual(string.Empty, context.OrganizationPostalCode);
            Assert.AreEqual(string.Empty, context.OrganizationPhone);
        }
        [TestMethod]
        public void should_update_with_Empty_strings_if_row_value_is_null()
        {
            //Arrange
            DXCRecommendationContext context = new DXCRecommendationContext();
            SpecialtyMed specialtyMedsWorkflow = new SpecialtyMed();
            DataTable patientsTable = new DataTable();
            DataColumn patientPracticeName = patientsTable.Columns.Add("PracticeName", typeof(string));
            DataColumn patientAddress1 = patientsTable.Columns.Add("Address1", typeof(string));
            DataColumn patientAddress2 = patientsTable.Columns.Add("Address2", typeof(string));
            DataColumn patientCity = patientsTable.Columns.Add("City", typeof(string));
            DataColumn patientState = patientsTable.Columns.Add("State", typeof(string));
            DataColumn patientZipCode = patientsTable.Columns.Add("ZipCode", typeof(string));
            DataColumn patientPhoneAreaCode = patientsTable.Columns.Add("PhoneAreaCode", typeof(string));

            //Act
            specialtyMedsWorkflow.UpdateDXCRecommendationContextOrganizationInformation(patientsTable/*REAL TEST*/, 0, ref context);


            //Assert 
            Assert.AreEqual(string.Empty, context.OrganizationName);
            Assert.AreEqual(string.Empty, context.OrganizationStreetAddress1);
            Assert.AreEqual(string.Empty, context.OrganizationStreetAddress2);
            Assert.AreEqual(string.Empty, context.OrganizationCity);
            Assert.AreEqual(string.Empty, context.OrganizationState);
            Assert.AreEqual(string.Empty, context.OrganizationPostalCode);
            Assert.AreEqual(string.Empty, context.OrganizationPhone);
        }

    }
}
