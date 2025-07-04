using System;
using System.Web.UI.WebControls;
using eRxWeb.AppCode;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Allscripts.ePrescribe.Test.AppCodeTests.MedicationSearchDisplayTests
{
    [TestClass]
    public class SetBenifitImgAsSpecialtyMedTests
    {
        [TestMethod]
        public void should_set_values_for_image()
        {
            //arrange
            var img = new Image();

            //act
            MedicationSearchDisplay.SetBenefitImgAsSpecialtyMed(ref img);

            //assert
            Assert.AreEqual("pointer", img.Style["cursor"]);
            Assert.AreEqual("images/dollar.png", img.ImageUrl);
            Assert.AreEqual("Prior Authorization and Patient Access Services available.  ", img.ToolTip);
        }

        [TestMethod]
        public void should_concat_tooltip_string_if_already_present()
        {
            //arrange
            var img = new Image();
            img.ToolTip = "Hi There!";

            //act
            MedicationSearchDisplay.SetBenefitImgAsSpecialtyMed(ref img);

            //assert
            Assert.AreEqual("Prior Authorization and Patient Access Services available.  Hi There!", img.ToolTip);
        }
    }
}
