using System;
using ScriptMessage = Allscripts.Impact.ScriptMsg.ScriptMessage;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml.Linq;

namespace Allscripts.ePrescribe.Test.ERxWebTests.AppCodeTests.ApproveRefillTaskTests
{
    [TestClass]
    public class UpdateOrRemovePharmNotesTests
    {
        [TestMethod]
        public void Should_Update_PharmacyNotes_When_Pharmacy_Notes_Is_Not_Null()
        {           
            string pahrmacyNotes = "$Coupon: test";
            var prescriptionElement = XElement.Parse("<Prescription><PharmacyNotes></PharmacyNotes></Prescription>");
            var retVal=ScriptMessage.UpdateOrRemovePharmNotes(pahrmacyNotes, prescriptionElement);           
            var expectedVal = XElement.Parse("<Prescription><PharmacyNotes>"+pahrmacyNotes+"</PharmacyNotes></Prescription>");
            Assert.AreEqual(expectedVal.ToString().Trim(), retVal.ToString().Trim());
        }
        [TestMethod]
        public void Should_Remove_PharmacyNotes_Node_When_Pharmacy_Notes_Is_Empty()
        {
            string pahrmacyNotes = "";
            var prescriptionElement = XElement.Parse("<Prescription><PharmacyNotes></PharmacyNotes></Prescription>");
            var retVal = ScriptMessage.UpdateOrRemovePharmNotes(pahrmacyNotes, prescriptionElement);
            var expectedVal = "<Prescription />";
            Assert.AreEqual(expectedVal.ToString().Trim(), retVal.ToString().Trim());
        }
        [TestMethod]
        public void Should_Not_Change_PharmacyNotes_When_Pharmacy_Notes_Is_Null()
        {
            string pahrmacyNotes = null;
            var prescriptionElement = XElement.Parse("<Prescription><PharmacyNotes>something</PharmacyNotes></Prescription>");
            var retVal = ScriptMessage.UpdateOrRemovePharmNotes(pahrmacyNotes, prescriptionElement);
            var expectedVal = XElement.Parse("<Prescription><PharmacyNotes>something</PharmacyNotes></Prescription>");
            Assert.AreEqual(expectedVal.ToString().Trim(), retVal.ToString().Trim());
        }
        [TestMethod]
        public void Should_Not_Add_PharmacyNotes_When_Pharmacy_Notes_Is_Null()
        {
            string pahrmacyNotes = null;
            var prescriptionElement = XElement.Parse("<Prescription></Prescription>");
            var retVal = ScriptMessage.UpdateOrRemovePharmNotes(pahrmacyNotes, prescriptionElement);
            var expectedVal = XElement.Parse("<Prescription></Prescription>");
            Assert.AreEqual(expectedVal.ToString().Trim(), retVal.ToString().Trim());
        }

    }
}
  