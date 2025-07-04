using System;
using Allscripts.ePrescribe.Objects;
using eRxWeb.AppCode.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Allscripts.ePrescribe.Test.ERxWebTests.AppCodeTests.TaskDisplayTests
{
    [TestClass]
    public class GetFormattedRxDetailsTests
    {
        [TestMethod]
        public void should_not_have_bolded_drug_description()
        {
            //arrange
            var model = new TaskRxDetailsModel
            {
                DrugDescription = "Drug"
            };

            //act
            var result = TaskDisplay.GetFormattedRxDetails("", true, model);

            //assert
            Assert.AreEqual("<b>Original Medication Prescribed: </b>Drug<BR><b>SIG:</b> <BR><b>Total number of refills requested:</b> , <b>Quantity:</b> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;", result);
        }

        [TestMethod]
        public void should_have_bolded_drug_description()
        {
            //arrange
            var model = new TaskRxDetailsModel
            {
                DrugDescription = "Drug"
            };

            //act
            var result = TaskDisplay.GetFormattedRxDetails("", false, model);

            //assert
            Assert.AreEqual("<b>Original Medication Prescribed: Drug</b><BR><b>SIG:</b> <BR><b>Total number of refills requested:</b> , <b>Quantity:</b> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;", result);
        }

        [TestMethod]
        public void should_add_patient_name()
        {
            //arrange
            var model = new TaskRxDetailsModel
            {
            };

            //act
            var result = TaskDisplay.GetFormattedRxDetails("Patient A", false, model);

            //assert
            Assert.AreEqual("Patient A<BR><b>Original Medication Prescribed: </b><BR><b>SIG:</b> <BR><b>Total number of refills requested:</b> , <b>Quantity:</b> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;", result);
        }

        [TestMethod]
        public void should_add_sig()
        {
            //arrange
            var model = new TaskRxDetailsModel
            {
                SigText = "Siggy Sig"
            };

            //act
            var result = TaskDisplay.GetFormattedRxDetails("", false, model);

            //assert
            Assert.AreEqual("<b>Original Medication Prescribed: </b><BR><b>SIG:</b> Siggy Sig<BR><b>Total number of refills requested:</b> , <b>Quantity:</b> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;", result);
        }

        [TestMethod]
        public void should_add_refills()
        {
            //arrange
            var model = new TaskRxDetailsModel
            {
                Refills = "23"
            };

            //act
            var result = TaskDisplay.GetFormattedRxDetails("", false, model);

            //assert
            Assert.AreEqual("<b>Original Medication Prescribed: </b><BR><b>SIG:</b> <BR><b>Total number of refills requested:</b> 23, <b>Quantity:</b> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;", result);
        }

        [TestMethod]
        public void should_add_quantity()
        {
            //arrange
            var model = new TaskRxDetailsModel
            {
                Quantity = "30"
            };

            //act
            var result = TaskDisplay.GetFormattedRxDetails("", false, model);

            //assert
            Assert.AreEqual("<b>Original Medication Prescribed: </b><BR><b>SIG:</b> <BR><b>Total number of refills requested:</b> , <b>Quantity:</b> 30&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;", result);
        }

        [TestMethod]
        public void should_add_daysSupply()
        {
            //arrange
            var model = new TaskRxDetailsModel
            {
                DaysSupply = "90"
            };

            //act
            var result = TaskDisplay.GetFormattedRxDetails("", false, model);

            //assert
            Assert.AreEqual("<b>Original Medication Prescribed: </b><BR><b>SIG:</b> <BR><b>Total number of refills requested:</b> , <b>Quantity:</b> , <b>Days Supply:</b> 90&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;", result);
        }

        [TestMethod]
        public void should_add_daw_if_value_is_Y()
        {
            //arrange
            var model = new TaskRxDetailsModel
            {
                Daw = "Y"
            };

            //act
            var result = TaskDisplay.GetFormattedRxDetails("", false, model);

            //assert
            Assert.AreEqual("<b>Original Medication Prescribed: </b><BR><b>SIG:</b> <BR><b>Total number of refills requested:</b> , <b>Quantity:</b> , <b>DAW</b>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;", result);
        }

        [TestMethod]
        public void should_add_daw_if_value_is_1()
        {
            //arrange
            var model = new TaskRxDetailsModel
            {
                Daw = "1"
            };

            //act
            var result = TaskDisplay.GetFormattedRxDetails("", false, model);

            //assert
            Assert.AreEqual("<b>Original Medication Prescribed: </b><BR><b>SIG:</b> <BR><b>Total number of refills requested:</b> , <b>Quantity:</b> , <b>DAW</b>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;", result);
        }

        [TestMethod]
        public void should_RxNotes()
        {
            //arrange
            var model = new TaskRxDetailsModel
            {
                RxNotes = "Notes Notes"
            };

            //act
            var result = TaskDisplay.GetFormattedRxDetails("", false, model);

            //assert
            Assert.AreEqual("<b>Original Medication Prescribed: </b><BR><b>SIG:</b> <BR><b>Total number of refills requested:</b> , <b>Quantity:</b> <BR><b>Notes: Notes Notes</b>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;", result);
        }

        [TestMethod]
        public void should_add_PharmacyDetails()
        {
            //arrange
            var model = new TaskRxDetailsModel
            {
                PharmacyDetails = "Pharm 123 lane"
            };

            //act
            var result = TaskDisplay.GetFormattedRxDetails("", false, model);

            //assert
            Assert.AreEqual("<b>Original Medication Prescribed: </b><BR><b>SIG:</b> <BR><b>Total number of refills requested:</b> , <b>Quantity:</b> &nbsp;&nbsp;&nbsp;<BR><b>Pharmacy: </b>Pharm 123 lane&nbsp;&nbsp;&nbsp;", result);
        }

        [TestMethod]
        public void should_add_Provider()
        {
            //arrange
            var model = new TaskRxDetailsModel
            {
                ProviderOfRecord = "Daffy"
            };

            //act
            var result = TaskDisplay.GetFormattedRxDetails("", false, model);

            //assert
            Assert.AreEqual("<b>Original Medication Prescribed: </b><BR><b>SIG:</b> <BR><b>Total number of refills requested:</b> , <b>Quantity:</b> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<BR><b>Provider of record:</b> Daffy&nbsp;&nbsp;&nbsp;", result);
        }

        [TestMethod]
        public void should_add_createdDate()
        {
            //arrange
            var model = new TaskRxDetailsModel
            {
                CreatedDate = "3/4/2016"
            };

            //act
            var result = TaskDisplay.GetFormattedRxDetails("", false, model);

            //assert
            Assert.AreEqual("<b>Original Medication Prescribed: </b><BR><b>SIG:</b> <BR><b>Total number of refills requested:</b> , <b>Quantity:</b> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<b>Original Rx Date:</b> 3/4/2016&nbsp;&nbsp;&nbsp;", result);
        }

        [TestMethod]
        public void should_add_lastFillDate()
        {
            //arrange
            var model = new TaskRxDetailsModel
            {
                LastFillDate = "3/4/2016"
            };

            //act
            var result = TaskDisplay.GetFormattedRxDetails("", false, model);

            //assert
            Assert.AreEqual("<b>Original Medication Prescribed: </b><BR><b>SIG:</b> <BR><b>Total number of refills requested:</b> , <b>Quantity:</b> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<b>Last Fill Date:</b> 3/4/2016&nbsp;&nbsp;&nbsp;", result);
        }


        [TestMethod]
        public void should_return_bold_comments_if_refreq()
        {
            //arrange
            var model = new TaskRxDetailsModel
            {
                RxNotes = "These notes should be bold"
            };

            //act
            var result = TaskDisplay.GetFormattedRxDetails("", false, model);

            //assert
            Assert.AreEqual("<b>Original Medication Prescribed: </b><BR><b>SIG:</b> <BR><b>Total number of refills requested:</b> , <b>Quantity:</b> <BR><b>Notes: These notes should be bold</b>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;", result);
        }
    }
}
