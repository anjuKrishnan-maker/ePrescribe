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
    public class HasScriptPadMedTests
    {
        private IStateContainer _pageState;
        [TestInitialize]
        public void init()
        {
            _pageState = MockRepository.GenerateStub<IStateContainer>();
        }
        [TestMethod, TestCategory("Select Medication")]
        public void should_return_true_when_script_pad_med_exist()
        {
            // arrange
            var selectMedController = new SelectMedicationApiController();
            List<Rx> rxList = new List<Rx>();
            rxList.Add(new Rx());
            _pageState[Constants.SessionVariables.CurrentScriptPadMeds] = rxList;

            selectMedController.PageState = _pageState;

            //act
            var hasScriptPadMed = selectMedController.HasScriptPadMed();


            //assert
            Assert.IsTrue(Convert.ToBoolean(hasScriptPadMed.Payload));
        }

        [TestMethod, TestCategory("Select Medication")]
        public void should_return_false_when_script_pad_med_not_exist()
        {
            // arrange
            var selectMedController = new SelectMedicationApiController();
            List<Rx> rxList = new List<Rx>();
            _pageState[Constants.SessionVariables.CurrentScriptPadMeds] = rxList;

            selectMedController.PageState = _pageState;

            //act
            var hasScriptPadMed = selectMedController.HasScriptPadMed();


            //assert
            Assert.IsFalse(Convert.ToBoolean(hasScriptPadMed.Payload));
        }
    }
}
