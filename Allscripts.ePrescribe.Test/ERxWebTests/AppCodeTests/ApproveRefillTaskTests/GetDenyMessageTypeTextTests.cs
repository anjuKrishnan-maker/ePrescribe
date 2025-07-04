using System;
using Allscripts.ePrescribe.Common;
using eRxWeb.AppCode;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Allscripts.ePrescribe.Test.ERxWebTests.AppCodeTests.ApproveRefillTaskTests
{
    [TestClass]
    public class GetDenyMessageTypeTextTests
    {
        [TestMethod]
        public void should_return_ChangeRx_if_task_type_is_rxchPA()
        {
            //act
            var result = new ApproveRefillTask().GetDenyMessageTypeText(Constants.PrescriptionTaskType.RXCHG_PRIORAUTH);

            //assert
            Assert.AreEqual("Change Rx", result);
        }

        [TestMethod]
        public void should_return_ChangeRx_if_task_type_is_rxch()
        {
            //act
            var result = new ApproveRefillTask().GetDenyMessageTypeText(Constants.PrescriptionTaskType.RXCHG);

            //assert
            Assert.AreEqual("Change Rx", result);
        }

        [TestMethod]
        public void should_return_Renewal_if_task_type_is_refreq()
        {
            //act
            var result = new ApproveRefillTask().GetDenyMessageTypeText(Constants.PrescriptionTaskType.REFREQ);

            //assert
            Assert.AreEqual("Renewal", result);
        }

        [TestMethod]
        public void should_return_Renewal_if_task_type_is_renewalRequest()
        {
            //act
            var result = new ApproveRefillTask().GetDenyMessageTypeText(Constants.PrescriptionTaskType.RENEWAL_REQUEST);

            //assert
            Assert.AreEqual("Renewal", result);
        }

        [TestMethod]
        public void should_return_Rx_if_task_type_is_not_the_above()
        {
            //act
            var result = new ApproveRefillTask().GetDenyMessageTypeText(Constants.PrescriptionTaskType.EPA);

            //assert
            Assert.AreEqual("Rx", result);
        }
    }
}
