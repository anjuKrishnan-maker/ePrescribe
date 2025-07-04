using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using eRxWeb;
using Rhino.Mocks;
using eRxWeb.State;
using Allscripts.ePrescribe.Common;

namespace Allscripts.ePrescribe.Test.ERxWebTests.RxProcessorTests
{
    [TestClass]
    public class RxCreationTypeTest
    {
        private IStateContainer _pageState;
        [TestInitialize]
        public void init()
        {
            _pageState = MockRepository.GenerateMock<IStateContainer>();
        }

        [TestMethod]
        public void should_be_standard_newRx_workflow_creationType()
        {
            //Arrange
            _pageState.Stub(x => x.GetStringOrEmpty(Constants.SessionVariables.IsCsRefReqWorkflow)).Return(string.Empty);
            _pageState.Stub(x => x.GetStringOrEmpty(Constants.SessionVariables.ChangeRxRequestedMedCs)).Return(string.Empty);
            _pageState.Stub(x => x.GetStringOrEmpty(Constants.SessionVariables.TaskScriptMessageId)).Return(string.Empty);

            //Act
            string creationType = new RxProcessor().GetRxCreationType(_pageState);

            //Assert
            Assert.AreEqual(Constants.RxCreationType.STANDARD_WORKFLOW, creationType);
        }

        [TestMethod]
        public void should_be_cs_refreq_workflow_creationType()
        {
            //Arrange
            _pageState.Stub(x => x.GetStringOrEmpty(Constants.SessionVariables.IsCsRefReqWorkflow)).Return("IsCsRefReqWorkflow");
            _pageState.Stub(x => x.GetStringOrEmpty(Constants.SessionVariables.ChangeRxRequestedMedCs)).Return(string.Empty);
            _pageState.Stub(x => x.GetStringOrEmpty(Constants.SessionVariables.TaskScriptMessageId)).Return(Guid.NewGuid().ToString());

            //Act
            string creationType = new RxProcessor().GetRxCreationType(_pageState);

            //Assert
            Assert.AreEqual(Constants.RxCreationType.REFILL, creationType);
        }

        [TestMethod]
        public void should_be_cs_chgrx_workflow_creationType()
        {
            //Arrange
            _pageState.Stub(x => x.GetStringOrEmpty(Constants.SessionVariables.IsCsRefReqWorkflow)).Return(string.Empty);
            _pageState.Stub(x => x.GetStringOrEmpty(Constants.SessionVariables.ChangeRxRequestedMedCs)).Return("ChangeRxRequestedMedCs");
            _pageState.Stub(x => x.GetStringOrEmpty(Constants.SessionVariables.TaskScriptMessageId)).Return(Guid.NewGuid().ToString());

            //Act
            string creationType = new RxProcessor().GetRxCreationType(_pageState);

            //Assert
            Assert.AreEqual(Constants.RxCreationType.REFILL, creationType);
        }
    }
}
