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
    public class GetProviderHistorySearchNoMedMsgTests
    {
        private IStateContainer _pageState;
        [TestInitialize]
        public void init()
        {
            _pageState = MockRepository.GenerateStub<IStateContainer>();
        }
        [TestMethod, TestCategory("Select Medication")]
        public void should_return_self_msg_when_user_is_provider()
        {
            // arrange
            var selectMedController = new SelectMedicationApiController();
            _pageState.Stub(x => x.Cast(Constants.SessionVariables.UserType, Constants.UserCategory.GENERAL_USER)).Return(Constants.UserCategory.PROVIDER);
            selectMedController.PageState = _pageState;

            //act
            var actual = selectMedController.GetProviderHistorySearchNoMedMsg();


            //assert
            Assert.AreEqual("No medications match your favorites.", actual);
        }

        [TestMethod, TestCategory("Select Medication")]
        public void should_return_dr_msg_when_user_is_provider()
        {
            // arrange
            var selectMedController = new SelectMedicationApiController();
            _pageState.Stub(x => x.Cast(Constants.SessionVariables.UserType, Constants.UserCategory.GENERAL_USER)).Return(Constants.UserCategory.POB_REGULAR);
            selectMedController.PageState = _pageState;

            //act
            var actual = selectMedController.GetProviderHistorySearchNoMedMsg();


            //assert
            Assert.AreEqual("No medications match Dr's favorites.", actual);
        }
    }
}
