using System;
using Allscripts.ePrescribe.Common;
using Allscripts.Impact;
using Allscripts.Impact.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using eRxWeb.AppCode.Tasks;
using ApproveRefillTask = eRxWeb.AppCode.ApproveRefillTask;

namespace Allscripts.ePrescribe.Test.ERxWebTests.AppCodeTests.ApproveRefillTaskTests
{
    [TestClass]
    public class ShowDenialReasonErrorTests
    {
        [TestMethod]
        public void should_return_false_if_task_type_is_not_refreq_or_chgrx()
        {
            //arrange
            var refillTask = new RxTaskModel
            {
                TaskType = Constants.PrescriptionTaskType.EPA
            };

            //act
            var result = ChangeRxTask.ShowDenialReasonError(refillTask);

            //assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void should_return_true_if_task_type_is_refreq__and_denial_code_is_neg1()
        {
            //arrange
            var refillTask = new RxTaskModel
            {
                TaskType = Constants.PrescriptionTaskType.REFREQ,
                DenialCode = "- 1"
            };

            //act
            var result = ChangeRxTask.ShowDenialReasonError(refillTask);

            //assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void should_return_true_if_task_type_is_refreq__and_denial_text_is_selec_denial_reason()
        {
            //arrange
            var refillTask = new RxTaskModel
            {
                TaskType = Constants.PrescriptionTaskType.REFREQ,
                DenialText = ApproveRefillTask.SELECT_DENIAL_REASON
            };

            //act
            var result = ChangeRxTask.ShowDenialReasonError(refillTask);

            //assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void should_return_true_if_task_type_is_rxchg__and_denial_text_is_selec_denial_reason()
        {
            //arrange
            var refillTask = new RxTaskModel
            {
                TaskType = Constants.PrescriptionTaskType.RXCHG,
                DenialText = ApproveRefillTask.SELECT_DENIAL_REASON
            };

            //act
            var result = ChangeRxTask.ShowDenialReasonError(refillTask);

            //assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void should_return_true_if_task_type_is_rxchg__and_denial_code_is_neg1()
        {
            //arrange
            var refillTask = new RxTaskModel
            {
                TaskType = Constants.PrescriptionTaskType.RXCHG,
                DenialCode = "- 1"
            };

            //act
            var result = ChangeRxTask.ShowDenialReasonError(refillTask);

            //assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void should_return_false_if_task_type_is_rxchg__and_denial_code_not_neg1()
        {
            //arrange
            var refillTask = new RxTaskModel
            {
                TaskType = Constants.PrescriptionTaskType.RXCHG,
                DenialCode = "3"
            };

            //act
            var result = ChangeRxTask.ShowDenialReasonError(refillTask);

            //assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void should_return_false_if_task_type_is_rxchg__and_denial_code_not_select_denial_reason()
        {
            //arrange
            var refillTask = new RxTaskModel
            {
                TaskType = Constants.PrescriptionTaskType.RXCHG,
                DenialText = "Just Because"
            };

            //act
            var result = ChangeRxTask.ShowDenialReasonError(refillTask);

            //assert
            Assert.IsFalse(result);
        }
    }
}
