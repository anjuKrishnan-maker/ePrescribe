using System;
using Rhino.Mocks;
using eRxWeb.AppCode.Tasks.Interfaces;
using Allscripts.Impact.Interfaces;
using Allscripts.Impact;
using Allscripts.ePrescribe.Common;
using eRxWeb.AppCode.Tasks;
using Allscripts.ePrescribe.DatabaseSelector;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using eRxWeb.AppCode.Interfaces;

namespace Allscripts.ePrescribe.Test.TaskingTests.ApproveRxTaskTests
{
    [TestClass]
    public class ProcessDenialUnitTests
    {
        [TestMethod]
        public void should_Return_RefillMessage_As_Denied_When_ProcessDenial_With_Parameters_Executes()
        {
            //Arrange
            TaskResponse taskResponseExpected = new TaskResponse
            {
                ExpectedDUR = DURWorkflowExpected.NO,
                ExpectedEPCS = EPCSWorkflowExpected.NO,
                RefillMessage = "ApproveRxTask: Denied.",
                ProcessedAction = ProcessedActionType.DENIED
            };
            RxTaskModel pharmacyTask = new RxTaskModel();
            pharmacyTask.TaskType = Constants.PrescriptionTaskType.APPROVAL_REQUEST;
            ApproveRxTask approveRxTask = new ApproveRxTask(pharmacyTask);
            var prescriptionMock = MockRepository.GenerateMock<IPrescription>();
            prescriptionMock.Stub(x => x.UpdateRxTask(0, Constants.PrescriptionTaskType.RENEWAL_REQUEST, Constants.PrescriptionTaskStatus.REFUSE, Constants.PrescriptionStatus.REJECTED, null, null, null, ConnectionStringPointer.SHARED_DB)).IgnoreArguments().Return(0);
            prescriptionMock.Stub(x => x.LoadFromExisting(null, new Objects.Dur(), null, null, ConnectionStringPointer.SHARED_DB)).IgnoreArguments();

            var epsBrokerMock = MockRepository.GenerateMock<IEPSBroker>();
            epsBrokerMock.Stub(x => x.UpdatePrescriptionStatus(null, null, 0, Constants.PrescriptionStatus.REJECTED, null, ConnectionStringPointer.SHARED_DB)).IgnoreArguments();

            //Act
            TaskResponse taskResponseActual = approveRxTask.ProcessDenial(prescriptionMock, epsBrokerMock).TaskResponse;

            //Assert
            Assert.AreEqual(taskResponseExpected.ExpectedDUR, taskResponseActual.ExpectedDUR);
            Assert.AreEqual(taskResponseExpected.ExpectedEPCS, taskResponseActual.ExpectedEPCS);
            Assert.AreEqual(taskResponseExpected.RefillMessage, taskResponseActual.RefillMessage);
            Assert.AreEqual(taskResponseExpected.ProcessedAction, taskResponseActual.ProcessedAction);
        }

        [TestMethod]
        public void should_Return_RefillMessage_As_invalid_action_type_When_ProcessDenial_Executes_With_Null_parameters()
        {
            TaskResponse taskResponseExpected = new TaskResponse
            {
                ExpectedDUR = DURWorkflowExpected.NO,
                ExpectedEPCS = EPCSWorkflowExpected.NO,
                RefillMessage = "ApproveRxTask ProcessTaskingWorkflow: invalid action type.",
                ProcessedAction = ProcessedActionType.DENIED
            };
            //Arrange
            RxTaskModel pharmacyTask = new RxTaskModel();
            pharmacyTask.TaskType = Constants.PrescriptionTaskType.APPROVAL_REQUEST;
            ApproveRxTask approveRxTask = new ApproveRxTask(pharmacyTask);

            //Act
            TaskResponse taskResponseActual = approveRxTask.ProcessDenial(null,null).TaskResponse;

            //Assert
            Assert.AreEqual(taskResponseExpected.ExpectedDUR, taskResponseActual.ExpectedDUR);
            Assert.AreEqual(taskResponseExpected.ExpectedEPCS, taskResponseActual.ExpectedEPCS);
            Assert.AreEqual(taskResponseExpected.RefillMessage, taskResponseActual.RefillMessage);
            Assert.AreEqual(taskResponseExpected.ProcessedAction, taskResponseActual.ProcessedAction);
        }
    }
}
