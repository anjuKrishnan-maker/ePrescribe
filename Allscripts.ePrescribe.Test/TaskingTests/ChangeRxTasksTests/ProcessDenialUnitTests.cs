using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using eRxWeb.AppCode.Tasks;
using Allscripts.Impact.Interfaces;
using Allscripts.Impact;
using Allscripts.ePrescribe.Common;
using Allscripts.Impact.Tasks;

namespace Allscripts.ePrescribe.Test.TaskingTests.ChangeRxTasksTests
{
    [TestClass]
    public class ProcessDenialUnitTests
    {
        [TestMethod]
        public void should_return_when_ShowDenialReasonError_Is_True()
        {
            //Arrange
            TaskResponse taskResponseExpected = new TaskResponse
            {
                ExpectedDUR = DURWorkflowExpected.NO,
                ExpectedEPCS = EPCSWorkflowExpected.NO,
                RefillMessage = Constants.TaskingMessages.DENIAL_REASON_ERROR
            };
            RxTaskModel pharmacyTask = new RxTaskModel();
            pharmacyTask.TaskType = Constants.PrescriptionTaskType.RXCHG;
            pharmacyTask.DenialCode = "- 1";
            ChangeRxTask changeRxTask = new ChangeRxTask(pharmacyTask);

            //Act
            TaskResponse taskResponseActual = changeRxTask.ProcessDenial().TaskResponse;

            //Assert
            Assert.AreEqual(taskResponseExpected, taskResponseExpected);
        }

        [TestMethod]
        public void should_return_invalid_message_when_null_task_supplied()
        {
            //Arrange
            TaskResponse taskResponseExpected = new TaskResponse
            {
                ExpectedDUR = DURWorkflowExpected.NO,
                ExpectedEPCS = EPCSWorkflowExpected.NO,
                RefillMessage = "Invalid Pharmacy Task"
            };
            ChangeRxTask changeRxTask = new ChangeRxTask();

            //Act
            TaskResponse taskResponseActual = changeRxTask.ProcessDenial().TaskResponse;

            //Assert
            Assert.AreEqual(taskResponseExpected, taskResponseExpected);
        }
    }
}
