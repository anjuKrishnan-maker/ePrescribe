using Allscripts.ePrescribe.Common;
using Allscripts.Impact;
using eRxWeb.Controller;
using eRxWeb.State;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Allscripts.ePrescribe.Test.ERxWebTests.ControllerTests.SelectMedicationAPITests
{
    [TestClass]
    public class GetParsedSigTextTests
    {
        private IStateContainer _pageState;
        [TestInitialize]
        public void init()
        {
            _pageState = MockRepository.GenerateStub<IStateContainer>();
        }

        [TestMethod, TestCategory("Select Medication")]
        public void should_return_empty_when_no_sigTextMDD()
        {
            // arrange
            var selectMedController = new SelectMedicationApiController();
            string mdd = string.Empty;

            //act
            var actual = selectMedController.GetParsedSigText(string.Empty,out mdd, "1");

            //assert
            Assert.AreEqual(string.Empty, actual);
        }

        [TestMethod, TestCategory("Select Medication")]
        public void should_return_sigTextMDD_when_sigTextMDD_not_start_with_MDD()
        {
            // arrange
            var selectMedController = new SelectMedicationApiController();
            string mdd = string.Empty;

            //act
            var actual = selectMedController.GetParsedSigText("Sample SigText", out mdd, "1");

            //assert
            Assert.AreEqual("Sample SigText", actual);
        }

        [TestMethod, TestCategory("Select Medication")]
        public void should_return_sigText_without_mdd_when_sigTextMDD_with_MDD()
        {
            // arrange
            var selectMedController = new SelectMedicationApiController();
            string mdd = string.Empty;
            _pageState.Stub(x => x.GetBooleanOrFalse("ALLOWMDD")).Return(false);
            _pageState.Stub(x => x.GetBooleanOrFalse("ALLOWMDD")).Return(false);
            selectMedController.PageState = _pageState;

            //act
            var actual = selectMedController.GetParsedSigText("SigText MDD:Sample", out mdd, "1");

            //assert
            Assert.AreEqual("SigText ", actual);
        }

        [TestMethod, TestCategory("Select Medication")]
        public void should_out_mdd_empty_when_sigTextMDD_with_MDD_and_disabled_mddSite()
        {
            // arrange
            var selectMedController = new SelectMedicationApiController();
            _pageState.Stub(x => x.GetBooleanOrFalse("ALLOWMDD")).Return(false);
            selectMedController.PageState = _pageState;
            string mdd = string.Empty;

            //act
            var actual = selectMedController.GetParsedSigText("MDD:Sample SigText", out mdd, "1");

            //assert
            Assert.AreEqual(string.Empty, mdd);
        }

        [TestMethod, TestCategory("Select Medication")]
        public void should_out_mdd_empty_when_sigTextMDD_with_MDD_and_mddSite_and_no_per_day_text()
        {
            // arrange
            var selectMedController = new SelectMedicationApiController();
            _pageState.Stub(x => x.GetBooleanOrFalse("ALLOWMDD")).Return(true);
            selectMedController.PageState = _pageState;
            string mdd = string.Empty;

            //act
            var actual = selectMedController.GetParsedSigText("MDD:Sample SigText", out mdd, "1");

            //assert
            Assert.AreEqual(string.Empty, mdd);
        }

        [TestMethod, TestCategory("Select Medication")]
        public void should_out_mdd_when_sigTextMDD_with_MDD_and_mddSite_and_per_day_text()
        {
            // arrange
            var selectMedController = new SelectMedicationApiController();
            _pageState.Stub(x => x.GetBooleanOrFalse("ALLOWMDD")).Return(true);
            selectMedController.PageState = _pageState;
            string mdd = string.Empty;

            //act
            var actual = selectMedController.GetParsedSigText("SigText MDD:SampleMDD Per Day ", out mdd, "1");

            //assert
            Assert.AreEqual("SampleMDD ", mdd);
        }
    }
}
