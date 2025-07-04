using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using eRxWeb.AppCode;
using eRxWeb.AppCode.SpecialtyMedWorkflow;
using System.Data;

namespace Allscripts.ePrescribe.Test.ERxWebTests.AppCodeTests.SpecialtyMedTests
{
    [TestClass]
    public class UpdateDXCRecommendationContextPatientInformationUnitTests
    {
        [TestMethod]
        public void should_update_with_Empty_strings_if_Null_DataSet_supplied()
        {
            //Arrange
            DXCRecommendationContext context = new DXCRecommendationContext();
            SpecialtyMed specialtyMedsWorkflow = new SpecialtyMed();

            //Act
            specialtyMedsWorkflow.UpdateDXCRecommendationContextPatientInformation(null/*REAL TEST*/, string.Empty, string.Empty, string.Empty, ref context);


            //Assert 
            Assert.AreEqual(string.Empty, context.PatientMRN);
            Assert.AreEqual(string.Empty, context.PatientID);
            Assert.AreEqual(Convert.ToString(DateTime.UtcNow), Convert.ToString(context.PatientDOB));
            Assert.AreEqual(string.Empty, context.PatientStreetAddress1);
            Assert.AreEqual(string.Empty, context.PatientStreetAddress2);
            Assert.AreEqual(string.Empty, context.PatientCity);
            Assert.AreEqual(string.Empty, context.PatientState);
            Assert.AreEqual(string.Empty, context.PatientPostalCode);
            Assert.AreEqual(string.Empty, context.PatientTelephone);
            Assert.AreEqual(string.Empty, context.PatientMobile);
            Assert.AreEqual(string.Empty, context.PatientEmail);
            
        }

        [TestMethod]
        public void should_update_with_Empty_strings_if_DataSet_supplied_with_zero_rows()
        {
            //Arrange
            DXCRecommendationContext context = new DXCRecommendationContext();
            SpecialtyMed specialtyMedsWorkflow = new SpecialtyMed();
            DataSet ds = new DataSet();
            DataTable patientsTable = ds.Tables.Add("Patient");
            

            //Act
            specialtyMedsWorkflow.UpdateDXCRecommendationContextPatientInformation(ds/*REAL TEST*/, string.Empty, string.Empty, string.Empty, ref context);


            //Assert 
            Assert.AreEqual(string.Empty, context.PatientMRN);
            Assert.AreEqual(string.Empty, context.PatientID);
            Assert.AreEqual(Convert.ToString(DateTime.UtcNow), Convert.ToString(context.PatientDOB));
            Assert.AreEqual(string.Empty, context.PatientStreetAddress1);
            Assert.AreEqual(string.Empty, context.PatientStreetAddress2);
            Assert.AreEqual(string.Empty, context.PatientCity);
            Assert.AreEqual(string.Empty, context.PatientState);
            Assert.AreEqual(string.Empty, context.PatientPostalCode);
            Assert.AreEqual(string.Empty, context.PatientTelephone);
            Assert.AreEqual(string.Empty, context.PatientMobile);
            Assert.AreEqual(string.Empty, context.PatientEmail);
        }
        [TestMethod]
        public void should_update_with_Empty_strings_if_row_value_is_null()
        {
            //Arrange
            DXCRecommendationContext context = new DXCRecommendationContext();
            SpecialtyMed specialtyMedsWorkflow = new SpecialtyMed();
            DataSet ds = new DataSet();
            DataTable patientsTable = ds.Tables.Add("Patient");
            DataColumn patientCity=  patientsTable.Columns.Add("City", typeof(string));

            //Act
            specialtyMedsWorkflow.UpdateDXCRecommendationContextPatientInformation(ds/*REAL TEST*/, string.Empty, string.Empty, string.Empty, ref context);


            //Assert 
            Assert.AreEqual(string.Empty, context.PatientMRN);
            Assert.AreEqual(string.Empty, context.PatientID);
            Assert.AreEqual(Convert.ToString(DateTime.UtcNow), Convert.ToString(context.PatientDOB));
            Assert.AreEqual(string.Empty, context.PatientStreetAddress1);
            Assert.AreEqual(string.Empty, context.PatientStreetAddress2);
            Assert.AreEqual(string.Empty, context.PatientCity);
            Assert.AreEqual(string.Empty, context.PatientState);
            Assert.AreEqual(string.Empty, context.PatientPostalCode);
            Assert.AreEqual(string.Empty, context.PatientTelephone);
            Assert.AreEqual(string.Empty, context.PatientMobile);
            Assert.AreEqual(string.Empty, context.PatientEmail);
        }
        
    }
}
