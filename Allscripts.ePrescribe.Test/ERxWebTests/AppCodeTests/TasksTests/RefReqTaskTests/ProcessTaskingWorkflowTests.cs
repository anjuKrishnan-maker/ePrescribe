using System;
using Allscripts.Impact;
using Allscripts.Impact.Tasks;
using Allscripts.Impact.Tasks.Interfaces;
using eRxWeb;
using eRxWeb.AppCode.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;

namespace Allscripts.ePrescribe.Test.ERxWebTests.AppCodeTests.TasksTests.RefReqTaskTests
{
    [TestClass]
    public class ProcessTaskingWorkflowTests
    {
        [TestMethod]
        public void should_return_dummy_task_response_if_currentTask_is_null()
        {
            //arrange
            RxTaskModel currentTask = null;

            //act
            var refReq = new RefillRequestRxTask();
            var result = refReq.ProcessTaskingWorkflow(currentTask, refReq);

            //assert
            Assert.AreEqual(DURWorkflowExpected.NO, result.TaskResponse.ExpectedDUR);
            Assert.AreEqual(EPCSWorkflowExpected.NO, result.TaskResponse.ExpectedEPCS);
            Assert.AreEqual("ChangeRxTask ProcessTaskingWorkflow: invalid action type.", result.TaskResponse.RefillMessage);
        }

        [TestMethod]
        public void should_call_ProcessDenial_if_action_is_no()
        {
            //arrange
            var currentTask = new RxTaskModel
            {
                RxRequestType = RequestType.DENY
            };

            var refReqMock = MockRepository.GenerateMock<IRxTask>();
            refReqMock.Stub(x => x.ProcessDenial()).IgnoreArguments().Return(currentTask);

            //act
            new RefillRequestRxTask().ProcessTaskingWorkflow(currentTask, refReqMock);

            //assert
            refReqMock.AssertWasCalled(x => x.ProcessDenial());
        }

        [TestMethod]
        public void should_not_call_ProcessApproval_if_action_is_no()
        {
            //arrange
            var currentTask = new RxTaskModel
            {
                RxRequestType = RequestType.DENY
            };

            var refReqMock = MockRepository.GenerateMock<IRxTask>();
            refReqMock.Stub(x => x.ProcessDenial()).IgnoreArguments().Return(currentTask);

            //act
            new RefillRequestRxTask().ProcessTaskingWorkflow(currentTask, refReqMock);

            //assert
            refReqMock.AssertWasNotCalled(x => x.ProcessApproval());
        }

        [TestMethod]
        public void should_call_ProcessApproval_if_action_is_Yes()
        {
            //arrange
            var currentTask = new RxTaskModel
            {
                RxRequestType = RequestType.APPROVE
            };

            var refReqMock = MockRepository.GenerateMock<IRxTask>();
            refReqMock.Stub(x => x.ProcessDenial()).IgnoreArguments().Return(currentTask);

            //act
            new RefillRequestRxTask().ProcessTaskingWorkflow(currentTask, refReqMock);

            //assert
            refReqMock.AssertWasCalled(x => x.ProcessApproval());
        }

        [TestMethod]
        public void should_Not_call_ProcessDenial_if_action_is_Yes()
        {
            //arrange
            var currentTask = new RxTaskModel
            {
                RxRequestType = RequestType.APPROVE
            };

            var refReqMock = MockRepository.GenerateMock<IRxTask>();
            refReqMock.Stub(x => x.ProcessDenial()).IgnoreArguments().Return(currentTask);

            //act
            new RefillRequestRxTask().ProcessTaskingWorkflow(currentTask, refReqMock);

            //assert
            refReqMock.AssertWasNotCalled(x => x.ProcessDenial());
        }
    }
}
