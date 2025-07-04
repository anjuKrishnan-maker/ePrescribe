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
    public class ProcessApprovalUnitTests
    {
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
            ChangeRxTask changeRxTask = new ChangeRxTask(null);

            //Act
            TaskResponse taskResponseActual = changeRxTask.ProcessApproval().TaskResponse;

            //Assert
            Assert.AreEqual(taskResponseExpected.RefillMessage, taskResponseActual.RefillMessage);
        }
    }
}
