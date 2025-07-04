using System;
using System.Text;
using eRxWeb;
using Allscripts.Impact;
using Allscripts.Impact.Interfaces;
using eRxWeb.AppCode;
using eRxWeb.AppCode.Interfaces;
using Allscripts.ePrescribe.Common;
using Rhino.Mocks;
using Allscripts.ePrescribe.Objects;
using Allscripts.Impact.Tasks.Interfaces;
using Rx = Allscripts.Impact.Rx;
using RxUser = Allscripts.Impact.RxUser;
using ScriptMessage = Allscripts.Impact.ScriptMsg.ScriptMessage;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Allscripts.ePrescribe.Test.ERxWebTests.AppCodeTests.PharmRefillDetailsTests
{
    [TestClass]
    public class PharmRefillDetailsUnitTests
    {
        private const string xml = "<AllscriptsMessageEnvelope><MessageContent><PrescriptionMessage><Patient><FirstName>Ethan</FirstName><LastName>OBrien</LastName></Patient></PrescriptionMessage></MessageContent></AllscriptsMessageEnvelope>";
        [TestMethod]
        public void function_RejectREFREQDisplayMessage_returns_correct_Display_string_when_valid_parameters_supplied()
        {

            //Assert
        }

        [TestMethod]        
        public void function_RejectRXCHGPriorAuthDisplayMessage_returns_correct_Display_string_when_valid_parameters_supplied()
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
            pTask.ScriptMessage = ScriptMessage.CreateScriptMessageForTest(xml);
            pTask.Rx = new Rx();

            string patientFirstName = "Ankit";
            string patientLastName = "Singh";
            StringBuilder messageBuilder = new StringBuilder();
            messageBuilder.Append("Rx Change Prior Auth denied (").Append(pTask.DenialText).Append(") for ");
            messageBuilder.Append(patientFirstName).Append(" ");
            messageBuilder.Append(patientLastName);
            string expectedString = messageBuilder.ToString();

            var changeRxMock = MockRepository.GenerateStub<IChangeRx>();
            changeRxMock.Stub(x => x.RejectPriorAuthWorkflow(pTask)).IgnoreArguments();


            //Act
            string actualString = new PharmRefillDetails().RejectRXCHGPriorAuthDisplayMessage(pTask, changeRxMock, "Ankit", "Singh");

            //Assert
            Assert.AreEqual(expectedString, actualString);
        }

        [TestMethod]
        public void function_RejectRXCHGDisplayMessage_returns_correct_Display_string_when_valid_parameters_supplied()
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
            pTask.ScriptMessage = ScriptMessage.CreateScriptMessageForTest(xml);
            pTask.Rx = new Rx();

            string patientFirstName = "Ankit";
            string patientLastName = "Singh";
            StringBuilder messageBuilder = new StringBuilder();
            messageBuilder.Append("Rx Change denied (").Append(pTask.DenialText).Append(") for ");
            messageBuilder.Append(patientFirstName).Append(" ");
            messageBuilder.Append(patientLastName);
            string expectedString = messageBuilder.ToString();

            var changeRxMock = MockRepository.GenerateStub<IChangeRx>();

            changeRxMock.Stub(x => x.RejectRxChangeWorkflow(pTask, new ScriptMessage())).IgnoreArguments();


            //Act
            string actualString = new PharmRefillDetails().RejectRXCHGDisplayMessage(pTask, changeRxMock, "Ankit", "Singh");

            //Assert
            Assert.AreEqual(expectedString, actualString);
        }

        [TestMethod]
        public void function_ApproveREFREQDisplayMessage_returns_correct_Display_string_when_valid_parameters_supplied()
        {
        }

        [TestMethod]
        public void function_ApproveRXCHGPriorAuthDisplayMessage_returns_correct_Display_string_when_valid_parameters_supplied()
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
            pTask.ScriptMessage = ScriptMessage.CreateScriptMessageForTest(xml);
            pTask.Rx = new Rx();

            string patientFirstName = "Ankit";
            string patientLastName = "Singh";
            string medicationName = "Humira";

            string expectedString = $"Change request for {medicationName} approved for {patientFirstName} {patientLastName}.";

            var changeRxMock = MockRepository.GenerateStub<IChangeRx>();


            //act
            changeRxMock.Stub(x => x.ApprovePriorAuthWorkflow(pTask)).IgnoreArguments().Return(expectedString);
            string actualString = new PharmRefillDetails().ApproveRXCHGPriorAuthDisplayMessage(pTask, changeRxMock, "Ankit", "Singh");

            //Assert
            Assert.AreEqual(expectedString, actualString);
        }



    }
}
