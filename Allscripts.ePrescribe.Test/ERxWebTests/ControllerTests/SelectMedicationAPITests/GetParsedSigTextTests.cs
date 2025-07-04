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
    public class isMDDSiteEnabledTests
    {
        private IStateContainer _pageState;
        [TestInitialize]
        public void init()
        {
            _pageState = MockRepository.GenerateStub<IStateContainer>();
        }
        [TestMethod, TestCategory("Select Medication")]
        public void should_return_true_when_allow_mmd()
        {
            // arrange
            var selectMedController = new SelectMedicationApiController();
            _pageState.Stub(x => x.GetBooleanOrFalse("ALLOWMDD")).Return(true);
            selectMedController.PageState = _pageState;

            //act
            var actual = selectMedController.IsMDDSiteEnabled("1");

            //assert
            Assert.IsTrue(actual);
        }

        [TestMethod, TestCategory("Select Medication")]
        public void should_return_true_when_allow_mmd_and_no_cs_meds_only()
        {
            // arrange
            var selectMedController = new SelectMedicationApiController();
            _pageState.Stub(x => x.GetBooleanOrFalse("ALLOWMDD")).Return(true);
            selectMedController.PageState = _pageState;

            //act
            var actual = selectMedController.IsMDDSiteEnabled("1");

            //assert
            Assert.IsTrue(actual);
        }

        [TestMethod, TestCategory("Select Medication")]
        public void should_return_true_when_allow_mmd_and_false_cs_meds_only()
        {
            // arrange
            var selectMedController = new SelectMedicationApiController();
            _pageState.Stub(x => x.GetBooleanOrFalse("ALLOWMDD")).Return(true);
            _pageState.Stub(x => x.GetBooleanOrFalse("CSMEDSONLY")).Return(false);
         
            selectMedController.PageState = _pageState;
            
            //act
            var actual = selectMedController.IsMDDSiteEnabled("1");

            //assert
            Assert.IsTrue(actual);
        }

        [TestMethod, TestCategory("Select Medication")]
        public void should_return_false_when_allow_mmd_and_true_cs_meds_only_and_no_cs_code()
        {
            // arrange
            var selectMedController = new SelectMedicationApiController();
            _pageState.Stub(x => x.GetBooleanOrFalse("ALLOWMDD")).Return(true);
            _pageState.Stub(x => x.GetBooleanOrFalse("CSMEDSONLY")).Return(true);

            selectMedController.PageState = _pageState;

            //act
            var actual = selectMedController.IsMDDSiteEnabled("");

            //assert
            Assert.IsFalse(actual);
        }
        [TestMethod, TestCategory("Select Medication")]
        public void should_return_true_when_allow_mmd_and_true_cs_meds_only_and_cs_code()
        {
            // arrange
            var selectMedController = new SelectMedicationApiController();
            _pageState.Stub(x => x.GetBooleanOrFalse("ALLOWMDD")).Return(true);
            _pageState.Stub(x => x.GetBooleanOrFalse("CSMEDSONLY")).Return(true);

            selectMedController.PageState = _pageState;

            //act
            var actual = selectMedController.IsMDDSiteEnabled("1");

            //assert
            Assert.IsTrue(actual);
        }

        [TestMethod, TestCategory("Select Medication")]
        public void should_return_false_when_no_allow_mmd()
        {
            // arrange
            var selectMedController = new SelectMedicationApiController();
            selectMedController.PageState = _pageState;

            //act
            var actual = selectMedController.IsMDDSiteEnabled("1");

            //assert
            Assert.IsFalse(actual);
        }

        [TestMethod, TestCategory("Select Medication")]
        public void should_return_false_when_false_allow_mmd()
        {
            // arrange
            var selectMedController = new SelectMedicationApiController();
            _pageState.Stub(x => x.GetBooleanOrFalse("ALLOWMDD")).Return(false);
            selectMedController.PageState = _pageState;

            //act
            var actual = selectMedController.IsMDDSiteEnabled("1");

            //assert
            Assert.IsFalse(actual);
        }
    }
}
