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
    public class IsCurrentUserAuthorizedToUpdateProviderOfRecordUnitTests
    {
        [TestMethod]
        public void should_return_true_if_UserType_Equals_Provider()
        {
            //Arrange
            RxTaskModel pharmacyTask = new RxTaskModel();
            pharmacyTask.TaskType = Constants.PrescriptionTaskType.APPROVAL_REQUEST;
            pharmacyTask.UserType = Constants.UserCategory.PROVIDER;
            ApproveRxTask approveRxTask1 = new ApproveRxTask(pharmacyTask);

            //Act
            bool isCurrentUserAuthorizedToUpdateProviderOfRecord = approveRxTask1.IsCurrentUserAuthorizedToUpdateProviderOfRecord();

            //Assert
            Assert.AreEqual(true, isCurrentUserAuthorizedToUpdateProviderOfRecord);
        }

        [TestMethod]
        public void should_return_true_if_UserType_Equals_PA()
        {
            //Arrange
            RxTaskModel pharmacyTask = new RxTaskModel();
            pharmacyTask.TaskType = Constants.PrescriptionTaskType.APPROVAL_REQUEST;
            pharmacyTask.UserType = Constants.UserCategory.PHYSICIAN_ASSISTANT;
            ApproveRxTask approveRxTask = new ApproveRxTask(pharmacyTask);

            //Act
            bool isCurrentUserAuthorizedToUpdateProviderOfRecord = approveRxTask.IsCurrentUserAuthorizedToUpdateProviderOfRecord();

            //Assert
            Assert.AreEqual(true, isCurrentUserAuthorizedToUpdateProviderOfRecord);

        }

        [TestMethod]
        public void should_return_true_if_UserType_Equals_PASupervised()
        {
            //Arrange
            RxTaskModel pharmacyTask = new RxTaskModel();
            pharmacyTask.TaskType = Constants.PrescriptionTaskType.APPROVAL_REQUEST;
            pharmacyTask.UserType = Constants.UserCategory.PHYSICIAN_ASSISTANT_SUPERVISED;
            ApproveRxTask approveRxTask = new ApproveRxTask(pharmacyTask);

            //Act
            bool isCurrentUserAuthorizedToUpdateProviderOfRecord = approveRxTask.IsCurrentUserAuthorizedToUpdateProviderOfRecord();

            //Assert
            Assert.AreEqual(true, isCurrentUserAuthorizedToUpdateProviderOfRecord);

        }
    }
}
