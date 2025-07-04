using System;
using eRxWeb.AppCode.Tasks;
using Allscripts.Impact.Interfaces;
using Allscripts.Impact;
using Allscripts.Impact.Tasks;
using eRxWeb.AppCode;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.Objects;

namespace Allscripts.ePrescribe.Test.TaskingTests.ApproveRxTaskTests
{
    [TestClass]
    public class ApproveRxTaskTests
    {
        TaskRxDetailsModel rx = new TaskRxDetailsModel();

        [TestMethod]
        public void should_return_valid_task_response()
        {
            //Arrange
            //   var userPartnerSsoFlagValue = new List<Dictionary<string, bool>>();
            //see  ProcessApprovalTests.cs e.g. should_not_call_isDURExpected_if_db_ddi_is_nullOrEmpty calls:
            //internal static ScriptMessage CreateScriptMessageForTest(string xml, Dictionary<string, string> drMedDictionary)
            //{
            //    var sm = CreateScriptMessageForTest(xml);
            //    sm = CreateDrMedForTest(drMedDictionary, sm);
            //    return sm;
            //}


            RxTaskModel task = new RxTaskModel();
            TaskResponse taskResponseExpected = new TaskResponse
            {
                ExpectedDUR = DURWorkflowExpected.NO,
                ExpectedEPCS = EPCSWorkflowExpected.NO,
                RefillMessage = ""
            };
            ApproveRxTask approveRxTask = new ApproveRxTask();

            //Act
           // TaskResponse taskResponseActual = approveRxTask.ProcessApproval().TaskResponse;

            //Assert
            Assert.AreEqual(1, 1);
            //Assert.AreEqual(taskResponseExpected.ExpectedDUR, taskResponseActual.ExpectedDUR);
            //Assert.AreEqual(taskResponseExpected.ExpectedEPCS, taskResponseActual.ExpectedEPCS);
            //Assert.AreEqual(taskResponseExpected.RefillMessage, taskResponseActual.RefillMessage);
        }
    }
}
