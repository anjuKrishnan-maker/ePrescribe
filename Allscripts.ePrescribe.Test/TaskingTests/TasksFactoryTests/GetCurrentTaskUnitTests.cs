using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using eRxWeb.AppCode;
using eRxWeb.AppCode.Tasks;
using Allscripts.ePrescribe.Common;
using Allscripts.Impact.Tasks;
using TasksFactory = eRxWeb.AppCode.Tasks.TasksFactory;

namespace Allscripts.ePrescribe.Test.TaskingTests.TasksFactoryTests
{
    [TestClass]
    public class GetCurrentTaskUnitTests
    {
        [TestMethod]
        public void should_return_task_processing_object_based_on_supplied_tasktype()
        {
            //Arrange
            
            //Act

            //Assert
            Assert.IsInstanceOfType(TasksFactory.GetTask(new Impact.RxTaskModel { TaskType = Constants.PrescriptionTaskType.APPROVAL_REQUEST }), typeof(ApproveRxTask));
            Assert.IsInstanceOfType(TasksFactory.GetTask(new Impact.RxTaskModel { TaskType = Constants.PrescriptionTaskType.RXCHG }), typeof(ChangeRxTask));
            Assert.IsInstanceOfType(TasksFactory.GetTask(new Impact.RxTaskModel { TaskType = Constants.PrescriptionTaskType.RXCHG_PRIORAUTH }), typeof(ChangeRxTask));
            Assert.IsInstanceOfType(TasksFactory.GetTask(new Impact.RxTaskModel { TaskType = Constants.PrescriptionTaskType.REFREQ }), typeof(RefillRequestRxTask));
        }
    }
}
