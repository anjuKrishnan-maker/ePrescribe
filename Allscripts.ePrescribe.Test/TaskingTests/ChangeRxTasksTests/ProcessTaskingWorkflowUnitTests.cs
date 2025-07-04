using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using eRxWeb.AppCode.Tasks;
using Allscripts.Impact.Interfaces;
using Allscripts.Impact;
using Allscripts.Impact.Tasks;

namespace Allscripts.ePrescribe.Test.TaskingTests
{
    [TestClass]
    public class ProcessTaskingWorkflowUnitTests
    {
        [TestMethod]
        public void should_return_invalid_task_TaskResponse_when_null_PharmacyTask_supplied()
        {
            //Arrange
            TaskResponse taskResponseExpected = new TaskResponse
            {
                ExpectedDUR = DURWorkflowExpected.NO,
                ExpectedEPCS = EPCSWorkflowExpected.NO,
                RefillMessage = "ChangeRxTask ProcessTaskingWorkflow: invalid action type."
            };
            ChangeRxTask changeRxTask = new ChangeRxTask();

            //Act
            TaskResponse taskResponseActual = changeRxTask.ProcessTaskingWorkflow().TaskResponse;

            //Assert
            Assert.AreEqual(taskResponseExpected.ExpectedDUR, taskResponseActual.ExpectedDUR);
            Assert.AreEqual(taskResponseExpected.ExpectedEPCS, taskResponseActual.ExpectedEPCS);
            Assert.AreEqual(taskResponseExpected.RefillMessage, taskResponseActual.RefillMessage);
        }
        [TestMethod]
        public void should_return_invalid_task_TaskResponse_when_invalid_current_Task_Action_supplied()
        {
            //Arrange
            TaskResponse taskResponseExpected = new TaskResponse
            {
                ExpectedDUR = DURWorkflowExpected.NO,
                ExpectedEPCS = EPCSWorkflowExpected.NO,
                RefillMessage = "ChangeRxTask ProcessTaskingWorkflow: invalid action type."
            };
            RxTaskModel currentTask = new RxTaskModel { RxRequestType = RequestType.UNKNOWN };
            ChangeRxTask changeRxTask = new ChangeRxTask(currentTask);

            //Act
            TaskResponse taskResponseActual = changeRxTask.ProcessTaskingWorkflow().TaskResponse;

            //Assert
            Assert.AreEqual(taskResponseExpected.ExpectedDUR, taskResponseActual.ExpectedDUR);
            Assert.AreEqual(taskResponseExpected.ExpectedEPCS, taskResponseActual.ExpectedEPCS);
            Assert.AreEqual(taskResponseExpected.RefillMessage, taskResponseActual.RefillMessage);
        }
    }
}
