using System;
using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.Data.Model;
using eRxWeb.ServerModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Allscripts.ePrescribe.Test.ERxWebTests.ServerModelTests.ReviewHistoryModelCreatorTests
{
    [TestClass]
    public class CreatePrescriptionTextTests
    {
        [TestMethod]
        public void should_not_append_any_text()
        {
            //arrange
            var prescriptionText = "Some prescription";

            var model = new ReviewHistoryDataModel
            {
                IsRegistryChecked = false,
                ControlledSubstanceCode = "  ",
                Prescription = prescriptionText
            };

            //act
            var result = ReviewHistoryModelCreator.CreatePrescriptionText(model);

            //assert
            Assert.AreEqual(prescriptionText, result);
        }

        [TestMethod]
        public void should_append_registry_checked()
        {
            //arrange
            var prescriptionText = "Some prescription";

            var model = new ReviewHistoryDataModel
            {
                IsRegistryChecked = true,
                ControlledSubstanceCode = "  ",
                Prescription = prescriptionText
            };

            //act
            var result = ReviewHistoryModelCreator.CreatePrescriptionText(model);

            //assert
            Assert.AreEqual(prescriptionText+ "&nbsp;&nbsp;<b>CS Registry Checked</b>", result);
        }

        [TestMethod]
        public void should_append_effective_date()
        {
            //arrange
            var prescriptionText = "Some prescription";
            var effectiveDate = DateTime.Today.AddDays(2);

            var model = new ReviewHistoryDataModel
            {
                IsRegistryChecked = false,
                ControlledSubstanceCode = "2  ",
                StartDate = effectiveDate,
                Prescription = prescriptionText,
                TransmissionMethod = Constants.PrescriptionTransmissionMethod.SENT
            };

            //act
            var result = ReviewHistoryModelCreator.CreatePrescriptionText(model);

            //assert
            Assert.AreEqual(prescriptionText+ $"&nbsp;&nbsp;Effective Date - {effectiveDate:d}", result);
        }

        [TestMethod]
        public void should_append_effective_date_and_registry_checked()
        {
            //arrange
            var prescriptionText = "Some prescription";
            var effectiveDate = DateTime.Today.AddDays(2);

            var model = new ReviewHistoryDataModel
            {
                IsRegistryChecked = true,
                ControlledSubstanceCode = "2  ",
                StartDate = effectiveDate,
                Prescription = prescriptionText,
                TransmissionMethod = Constants.PrescriptionTransmissionMethod.SENT
            };

            //act
            var result = ReviewHistoryModelCreator.CreatePrescriptionText(model);

            //assert
            Assert.AreEqual(prescriptionText + "&nbsp;&nbsp;<b>CS Registry Checked</b>" + $"&nbsp;&nbsp;Effective Date - {effectiveDate:d}", result);
        }

        [TestMethod]
        public void should_not_append_effective_date_if_not_sent()
        {
            //arrange
            var prescriptionText = "Some prescription";
            var effectiveDate = DateTime.Today.AddDays(2);

            var model = new ReviewHistoryDataModel
            {
                IsRegistryChecked = false,
                ControlledSubstanceCode = "2  ",
                StartDate = effectiveDate,
                Prescription = prescriptionText,
                TransmissionMethod = Constants.PrescriptionTransmissionMethod.PRINTED
            };

            //act
            var result = ReviewHistoryModelCreator.CreatePrescriptionText(model);

            //assert
            Assert.AreEqual(prescriptionText, result);
        }
    }
}
