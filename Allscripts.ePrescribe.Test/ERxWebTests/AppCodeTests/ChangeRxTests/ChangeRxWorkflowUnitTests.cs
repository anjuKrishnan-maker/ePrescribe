using System;
using Allscripts.Impact;
using Allscripts.Impact.Interfaces;
using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.Objects;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Allscripts.ePrescribe.DatabaseSelector;
using Allscripts.Impact.Tasks;
using eRxWeb.AppCode;
using Rhino.Mocks;
using Rx = Allscripts.Impact.Rx;
using ScriptMessage = Allscripts.Impact.ScriptMsg.ScriptMessage;
using eRxWeb.AppCode.Tasks;

namespace Allscripts.ePrescribe.Test.ERxWebTests.AppCodeTests.ChangeRxTests
{

    [TestClass]
    public class ChangeRxWorkflowUnitTests
    {


        [TestMethod]        
        public void function_UpdateChangeRxWorkflowFromPharmacyTask_updates_ChangeRxWorkflow_correctly_when_valid_PharmacyTask_provided()
        {
            //Arrange
            RxTaskModel pTask = new RxTaskModel();
            pTask.RxTaskId = 1;
            pTask.RxGUID = "Sample RxID";
            pTask.PatientId = "Sample PatientID";
            pTask.PhysicianComments = "Sample Physician Comments";
            pTask.IsPatientVisitRq = "false";
            pTask.ScriptMessageGUID = " Sample ScriptMessageID";
            pTask.RetailPharmacyId = "Sample Pharmacy ID";
            pTask.PharmacyIsElectronicEnabled = false;
            pTask.DenialCode = "Sample Denial Code";
            pTask.DenialText = "Sample Denial text";
            pTask.RxRequestType = RequestType.UNKNOWN;
            pTask.IsControlledSubstance = false;
            pTask.ReconciledControlledSubstanceCode = 1;
            pTask.RequestedRxIndexSelected = 1;
            pTask.TaskType = Constants.PrescriptionTaskType.APPROVAL_REQUEST;
            pTask.PriorAuthCode = "Sample PriorAuthCode";

            Guid strGUID = new Guid();
            RxTaskModel changeRx = new RxTaskModel
            {
                ScriptMessage = null,
                ScriptMessageGUID = null,
                RequestedRxIndexSelected = 0,
                RequestedRx = null,
                Rx = null,
                UserId = strGUID,
                DbId = ConnectionStringPointer.SHARED_DB,
                LicenseId = strGUID,
                SiteId = 1,
                ShieldSecurityToken = "Sample ShieldSecurityToken",
                ExternalFacilityCd = "Sample External FacilityID",
                ExternalGroupID = "Sample ExternalGroupID",
                UserType = Constants.UserCategory.GENERAL_USER,
                DelegateProviderId = strGUID
            };
            ScriptMessage scriptMessage = new ScriptMessage();
            string scriptMessageXmlMessage = "<Sample></Sample>";
            Rx rx = new Rx();

            //Act
            changeRx = new ChangeRxTask().UpdateChangeRxWorkflowFromPharmacyTask(pTask, scriptMessageXmlMessage, rx);

            //Assert
            Assert.AreEqual(changeRx.ScriptMessageGUID, pTask.ScriptMessageGUID);
            Assert.AreEqual(changeRx.RequestedRxIndexSelected, pTask.RequestedRxIndexSelected);
        }

        [TestMethod, ExpectedException(typeof(ArgumentException))]
        public void function_UpdateChangeRxWorkflowFromPharmacyTask_throws_exception_when_null_ChangeRxWorkflow_provided()
        {

            RxTaskModel changeRx = null;
            string scriptMessageXmlMessage = "<Sample></Sample>";

            //Act
            new ChangeRxTask().UpdateChangeRxWorkflowFromPharmacyTask(changeRx, scriptMessageXmlMessage, new Rx());

        }
    }
}
