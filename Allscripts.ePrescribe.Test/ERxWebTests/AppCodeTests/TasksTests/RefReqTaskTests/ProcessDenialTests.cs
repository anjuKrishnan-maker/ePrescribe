using System;
using Allscripts.ePrescribe.Common;
using Allscripts.Impact;
using Allscripts.Impact.Tasks;
using eRxWeb.AppCode.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ApproveRefillTask = eRxWeb.AppCode.ApproveRefillTask;
using ScriptMessage = Allscripts.Impact.ScriptMsg.ScriptMessage;

namespace Allscripts.ePrescribe.Test.ERxWebTests.AppCodeTests.TasksTests.RefReqTaskTests
{
    [TestClass]
    public class ProcessDenialTests
    {
        [TestMethod]
        public void should_return_denial_reason_error_in_refill_message_if_denialCode_is_neg_1()
        {
            //arrange
            var currentTask = new RxTaskModel
            {
                DenialCode = "- 1"
            };

            //act
            var result = new RefillRequestRxTask().ProcessDenial(currentTask, new ScriptMessage(), new RefillRequestRxTask(), new ApproveRefillTask());

            //assert
            Assert.AreEqual(Constants.TaskingMessages.DENIAL_REASON_ERROR, result.TaskResponse.RefillMessage);
        }
    }
}
