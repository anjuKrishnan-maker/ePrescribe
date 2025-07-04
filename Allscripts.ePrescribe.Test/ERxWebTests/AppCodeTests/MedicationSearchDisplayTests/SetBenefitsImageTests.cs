using System;
using System.Web.UI.WebControls;
using eRxWeb.AppCode;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Allscripts.ePrescribe.Test.AppCodeTests.MedicationSearchDisplayTests
{
    [TestClass]
    public class SetBenefitsImageTests
    {
        [TestMethod]
        public void should_set_text_to_only_financial_savings_message()
        {
            //arrange
            var img = new Image();

            //act
            MedicationSearchDisplay.SetBenefitsImage(ref img, false, true, false, true);

            //assert
            Assert.AreEqual("Patient is Eligible for Financial Savings.", img.ToolTip);
        }

        [TestMethod]
        public void should_set_img_src_to_dollar()
        {
            //arrange
            var img = new Image();

            //act
            MedicationSearchDisplay.SetBenefitsImage(ref img, false, true, false, true);

            //assert
            Assert.AreEqual("images/dollar.png", img.ImageUrl);
        }

        [TestMethod]
        public void should_set_text_to_only_specialty_med_text()
        {
            //arrange
            var img = new Image();

            //act
            MedicationSearchDisplay.SetBenefitsImage(ref img, false, false, true, false);

            //assert
            Assert.AreEqual("Prior Authorization and Patient Access Services available.  ", img.ToolTip);
        }

        [TestMethod]
        public void should_set_text_to_only_benifit_med_text()
        {
            //arrange
            var img = new Image();

            //act
            MedicationSearchDisplay.SetBenefitsImage(ref img, true, false, false, false);

            //assert
            Assert.AreEqual("Benefit information available. Click to update right panel.", img.ToolTip);
        }

        [TestMethod]
        public void should_set_text_to_financial_offers_and_specialty_med()
        {
            //arrange
            var img = new Image();

            //act
            MedicationSearchDisplay.SetBenefitsImage(ref img, false, true, true, true);

            //assert
            Assert.AreEqual("Prior Authorization and Patient Access Services available.  Patient is Eligible for Financial Savings.", img.ToolTip);
        }

        [TestMethod]
        public void should_set_text_to_financial_offers_and_benifit_med()
        {
            //arrange
            var img = new Image();

            //act
            MedicationSearchDisplay.SetBenefitsImage(ref img, true, true, false, true);

            //assert
            Assert.AreEqual("Patient is Eligible for Financial Savings. Benefit information available. Click to update right panel.", img.ToolTip);
        }
    }
}
