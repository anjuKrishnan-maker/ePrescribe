using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using eRxWeb.AppCode;
using System.Data;
using eRxWeb.AppCode.SpecialtyMedWorkflow;
using eRxWeb.AppCode.Interfaces;

namespace Allscripts.ePrescribe.Test.ERxWebTests.AppCodeTests.SpecialtyMedTests
{
    [TestClass]
    public class BuildConfirmOfferUnitTests
    {
        [TestMethod]
        public void should_return_a_DXCConfirmOfferRequest_object_populated_with_correct_data_when_valid_dataTable_supplied()
        {
            //Arrange
            SpecialtyMed specialtyMed = new SpecialtyMed();
            DataTable dtConfirmOfferDetails = new DataTable();
            dtConfirmOfferDetails.Columns.Add("ActivityID", typeof(int));
            dtConfirmOfferDetails.Columns.Add("PatientID", typeof(Guid));
            dtConfirmOfferDetails.Columns.Add("FirstName", typeof(string));
            dtConfirmOfferDetails.Columns.Add("LastName", typeof(string));
            dtConfirmOfferDetails.Columns.Add("MobilePhone", typeof(string));
            dtConfirmOfferDetails.Columns.Add("Email", typeof(string));
            dtConfirmOfferDetails.Columns.Add("Sex", typeof(string));
            DataRow dtRow = dtConfirmOfferDetails.NewRow();
            dtRow["ActivityID"] = 22;
            dtRow["PatientID"] = new Guid();
            dtRow["FirstName"] = "John";
            dtRow["LastName"] = "Smith";
            dtRow["MobilePhone"] = "919-000-0000";
            dtRow["Email"] = "test@gmail.com";
            dtRow["Sex"] = "M";
            dtConfirmOfferDetails.Rows.Add(dtRow);


            //Act
            DXCConfirmOffer confirmOffer = specialtyMed.BuildConfirmOffer(dtConfirmOfferDetails);

            //Assert
            Assert.AreEqual(dtRow["ActivityID"], confirmOffer.ActivityID);
            Assert.AreEqual(dtRow["MobilePhone"], confirmOffer.PatientMobilePhone);
            Assert.AreEqual(dtRow["Email"], confirmOffer.PatientEmail);
            Assert.AreEqual(true, confirmOffer.ConfirmOffer);
        }

        [TestMethod]
        public void should_return_an_empty_DXCConfirmOfferRequest_object_when_invalid_data_supplied()
        {
            //Arrange
            SpecialtyMed specialtyMed = new SpecialtyMed();
            DataTable dtConfirmOfferDetails = null;


            //Act
            DXCConfirmOffer confirmOffer = specialtyMed.BuildConfirmOffer(dtConfirmOfferDetails);

            //Assert
            Assert.AreEqual(0, confirmOffer.ActivityID);
            Assert.AreEqual(null, confirmOffer.PatientMobilePhone);
            Assert.AreEqual(null, confirmOffer.PatientEmail);
            Assert.AreEqual(false, confirmOffer.ConfirmOffer);
        }
    }
}
