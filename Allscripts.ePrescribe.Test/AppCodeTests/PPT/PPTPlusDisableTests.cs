using Allscripts.ePrescribe.Common;
using eRxWeb.AppCode.PptPlusBPL;
using eRxWeb.State;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;

namespace Allscripts.ePrescribe.Test.AppCodeTests.PPT
{
    [TestClass]
    public class PPTPlusDisableTests
    {

        [TestMethod]
        public void should_disable_since_changerx()
        {
            var sessionMock = MockRepository.GenerateMock<IStateContainer>();
            sessionMock.Stub(_ => _.GetStringOrEmpty(Constants.SessionVariables.TaskType)).Return(Constants.MessageTypes.CHANGERX_REQUEST);

            var result = PPTPlus.ShouldDisableWindow(sessionMock);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void should_disable_since_refreq()
        {
            var sessionMock = MockRepository.GenerateMock<IStateContainer>();
            sessionMock.Stub(_ => _.GetStringOrEmpty(Constants.SessionVariables.TaskType)).Return(Constants.MessageTypes.REFILL_REQUEST);

            var result = PPTPlus.ShouldDisableWindow(sessionMock);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void should_disable_since_Cs_changerx()
        {
            var sessionMock = MockRepository.GenerateMock<IStateContainer>();
            sessionMock.Stub(_ => _.GetStringOrEmpty(Constants.SessionVariables.ChangeRxRequestedMedCs)).Return("Allscripts.Impact.Tasks.ChangeRxRequestedMedCs");

            var result = PPTPlus.ShouldDisableWindow(sessionMock);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void should_disable_since_Cs_changerx_not_empty_session()
        {
            var sessionMock = MockRepository.GenerateMock<IStateContainer>();
            sessionMock.Stub(_ => _.GetStringOrEmpty(Constants.SessionVariables.ChangeRxRequestedMedCs)).Return("Allscripts.Impact.Tasks.ChangeRxRequestedMedCs");
            sessionMock.Stub(_ => _.GetStringOrEmpty(Constants.SessionVariables.TaskType)).Return(Constants.MessageTypes.NEWRX);

            var result = PPTPlus.ShouldDisableWindow(sessionMock);

            Assert.IsTrue(result);
        }
        [TestMethod]
        public void should_not_disable_since_empty()
        {
            var sessionMock = MockRepository.GenerateMock<IStateContainer>();

            var result = PPTPlus.ShouldDisableWindow(sessionMock);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void should_not_disable_since_newRx()
        {
            var sessionMock = MockRepository.GenerateMock<IStateContainer>();

            sessionMock.Stub(_ => _.GetStringOrEmpty(Constants.SessionVariables.TaskType)).Return(Constants.MessageTypes.NEWRX);

            var result = PPTPlus.ShouldDisableWindow(sessionMock);

            Assert.IsFalse(result);
        }
    }
}
