using System;
using System.Data;
using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.DatabaseSelector;
using Allscripts.ePrescribe.ExtensionMethods;
using Allscripts.ePrescribe.Objects;
using Allscripts.Impact;
using Allscripts.Impact.Interfaces;
using Allscripts.Impact.Tasks;
using Allscripts.Impact.Tasks.Interfaces;
using eRxWeb.AppCode;
using eRxWeb.AppCode.Interfaces;
using eRxWeb.State;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using Rx = Allscripts.Impact.Rx;
using RxUser = Allscripts.Impact.RxUser;
using ScriptMessage = Allscripts.Impact.ScriptMsg.ScriptMessage;
using Medication = Allscripts.Impact.Medication;
using eRxWeb.AppCode.Tasks;

namespace Allscripts.ePrescribe.Test.ERxWebTests.AppCodeTests.ChangeRxTests
{
    [TestClass]
    public class CompleteApprovePriorAuthWorkflowTests
    {
        private const string xml = "<AllscriptsMessageEnvelope><MessageContent><PrescriptionMessage><Patient><FirstName>Ethan</FirstName><LastName>OBrien</LastName></Patient></PrescriptionMessage></MessageContent></AllscriptsMessageEnvelope>";
        [TestMethod]
        public void should_not_call_setProviderGuid_if_sm_is_null()
        {
            //arrange
            var changeTask = new RxTaskModel();

            var changeRxMock = MockRepository.GenerateStub<IChangeRx>();

            var prescriptionMock = MockRepository.GenerateMock<IPrescription>();

            var scriptMessageMock = MockRepository.GenerateMock<IScriptMessage>();

            var auditMock = MockRepository.GenerateMock<IAudit>();
            
            
            //act
            new ChangeRxTask().CompleteApprovePriorAuthWorkflow(changeTask, changeRxMock, prescriptionMock, scriptMessageMock, auditMock);

            //assert
            changeRxMock.AssertWasNotCalled(x => x.CheckForDelegateProvider(new Guid(), new Guid(), new Guid(), Constants.UserCategory.GENERAL_USER, null, ConnectionStringPointer.SHARED_DB));
        }

        [TestMethod]
        public void should_not_call_setProviderGuid_if_rx_is_null()
        {
            //arrange
            var changeTask = new RxTaskModel { ScriptMessage = new ScriptMessage() };

            var changeRxMock = MockRepository.GenerateStub<IChangeRx>();

            var prescriptionMock = MockRepository.GenerateMock<IPrescription>();

            var scriptMessageMock = MockRepository.GenerateMock<IScriptMessage>();

            var auditMock = MockRepository.GenerateMock<IAudit>();
            
            //act
            new ChangeRxTask().CompleteApprovePriorAuthWorkflow(changeTask, changeRxMock, prescriptionMock, scriptMessageMock, auditMock);

            //assert
            changeRxMock.AssertWasNotCalled(x => x.CheckForDelegateProvider(new Guid(), new Guid(), new Guid(), Constants.UserCategory.GENERAL_USER, null, ConnectionStringPointer.SHARED_DB));
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void should_get_exception_if_sm_is_not_valid()
        {
            //arrange
            var changeTask = new RxTaskModel
            {
                ScriptMessage = new ScriptMessage(),
                Rx = new Rx()
            };

            var changeRxMock = MockRepository.GenerateStub<IChangeRx>();

            var prescriptionMock = MockRepository.GenerateMock<IPrescription>();

            var scriptMessageMock = MockRepository.GenerateMock<IScriptMessage>();

            var auditMock = MockRepository.GenerateMock<IAudit>();
            

            //act
            new ChangeRxTask().CompleteApprovePriorAuthWorkflow(changeTask, changeRxMock, prescriptionMock, scriptMessageMock, auditMock);
        }

        [TestMethod]
        public void should_send_new_message_by_using_script_message_returned_from_create_method()
        {
            //arrange
            var newScriptMessageGuid = new Guid("9EBF9663-E938-44B7-AB7B-D72A095FC8FA");
            var sm = ScriptMessage.CreateScriptMessageForTest(xml);
            var changeTask = new RxTaskModel
            {
                ScriptMessage = sm,
                Rx = new Rx()
            };

            var changeRxMock = MockRepository.GenerateMock<IChangeRx>();

            var prescriptionMock = MockRepository.GenerateMock<IPrescription>();

            var scriptMessageMock = MockRepository.GenerateMock<IScriptMessage>();
            scriptMessageMock.Stub(x => x.createRXCHGRES_ApprovalMessage(null, null, null, null, new Guid(), new Guid(), null, null, null, null, 0, new DateTime(), ConnectionStringPointer.SHARED_DB, Medication.MedType.NON_CS)).IgnoreArguments().Return(newScriptMessageGuid);
           
            var auditMock = MockRepository.GenerateMock<IAudit>();
            
            //act
            new ChangeRxTask().CompleteApprovePriorAuthWorkflow(changeTask, changeRxMock, prescriptionMock, scriptMessageMock, auditMock);

            //assert
            var args = scriptMessageMock.GetArgumentsForCallsMadeOn(x => x.SendThisMessage(new Guid(), new Guid(), new Guid(), ConnectionStringPointer.SHARED_DB));
            Assert.AreEqual(newScriptMessageGuid, args[0][0]);
        }
        
        [TestMethod]
        public void should_use_provider_id_from_checkForDelegate_provider_to_update_rxDetail_status()
        {
            //arrange
            var providerId = new Guid("BB054A0F-9BA6-4341-BB81-193DA8DB2415");
            var sm = ScriptMessage.CreateScriptMessageForTest(xml);
            var changeTask = new RxTaskModel
            {
                ScriptMessage = sm,
                Rx = new Rx()
            };

            var changeRxMock = MockRepository.GenerateMock<IChangeRx>();
            changeRxMock.Stub(x => x.CheckForDelegateProvider(new Guid(), new Guid(), new Guid(), Constants.UserCategory.GENERAL_USER, null, ConnectionStringPointer.SHARED_DB)).IgnoreArguments().Return(providerId);

            var prescriptionMock = MockRepository.GenerateStub<IPrescription>();

            var scriptMessageMock = MockRepository.GenerateMock<IScriptMessage>();

            var auditMock = MockRepository.GenerateMock<IAudit>();
            
            //act
            new ChangeRxTask().CompleteApprovePriorAuthWorkflow(changeTask, changeRxMock, prescriptionMock, scriptMessageMock, auditMock);

            //assert
            var args = prescriptionMock.GetArgumentsForCallsMadeOn(x => x.UpdateRxDetailStatus(new Guid(), new Guid(), null, Constants.RxStatus.AUTHORIZEBY, ConnectionStringPointer.SHARED_DB));

            Assert.AreEqual(providerId, args[0][1]);
        }

        [TestMethod]
        public void should_first_update_rxdetails_with_authorizedBY_then_update_the_rx_with_sent_to_phamacy()
        {
            //arrange

            var sm = ScriptMessage.CreateScriptMessageForTest(xml);
            var changeTask = new RxTaskModel
            {
                ScriptMessage = sm,
                Rx = new Rx()
            };

            var changeRxMock = MockRepository.GenerateMock<IChangeRx>();

            var prescriptionMock = MockRepository.GenerateStub<IPrescription>();

            var scriptMessageMock = MockRepository.GenerateMock<IScriptMessage>();

            var auditMock = MockRepository.GenerateMock<IAudit>();
            
            //act
            new ChangeRxTask().CompleteApprovePriorAuthWorkflow(changeTask,  changeRxMock, prescriptionMock, scriptMessageMock, auditMock);

            //assert
            var args = prescriptionMock.GetArgumentsForCallsMadeOn(x => x.UpdateRxDetailStatus(new Guid(), new Guid(), null, Constants.RxStatus.AUTHORIZEBY, ConnectionStringPointer.SHARED_DB));

            Assert.AreEqual(Constants.RxStatus.AUTHORIZEBY, args[0][3]);
            Assert.AreEqual(Constants.RxStatus.SENTTOPHARMACY, args[1][3]);
        }

        [TestMethod]
        public void should_update_rxTask_to_new()
        {
            //arrange
            var sm = ScriptMessage.CreateScriptMessageForTest(xml);
            var changeTask = new RxTaskModel
            {
                ScriptMessage = sm,
                Rx = new Rx()
            };

            var changeRxMock = MockRepository.GenerateMock<IChangeRx>();

            var prescriptionMock = MockRepository.GenerateStub<IPrescription>();

            var scriptMessageMock = MockRepository.GenerateMock<IScriptMessage>();

            var auditMock = MockRepository.GenerateMock<IAudit>();
            
            //act
            new ChangeRxTask().CompleteApprovePriorAuthWorkflow(changeTask, changeRxMock, prescriptionMock, scriptMessageMock, auditMock);

            //assert
            var args = scriptMessageMock.GetArgumentsForCallsMadeOn(x => x.updateRxTask(new Guid(), null, Constants.PrescriptionStatus.NEW, new Guid(), ConnectionStringPointer.SHARED_DB));

            Assert.AreEqual(Constants.PrescriptionStatus.NEW, args[0][2]);
        }

        [TestMethod]
        public void should_call_audit_insert_if_serviceTaskId_is_not_negative1()
        {
            //arrange
            var sm = ScriptMessage.CreateScriptMessageForTest(xml);
            var changeTask = new RxTaskModel
            {
                ScriptMessage = sm,
                Rx = new Rx()
            };

            var changeRxMock = MockRepository.GenerateMock<IChangeRx>();

            var prescriptionMock = MockRepository.GenerateStub<IPrescription>();

            var scriptMessageMock = MockRepository.GenerateMock<IScriptMessage>();
            scriptMessageMock.Stub(x => x.SendThisMessage(new Guid(), new Guid(), new Guid(), ConnectionStringPointer.SHARED_DB)).IgnoreArguments().Return(89);

            var auditMock = MockRepository.GenerateStub<IAudit>();
            
            //act
            new ChangeRxTask().CompleteApprovePriorAuthWorkflow(changeTask, changeRxMock, prescriptionMock, scriptMessageMock, auditMock);

            //assert
            auditMock.AssertWasCalled(x => x.InsertAuditLogPatientServiceTask(Arg<int>.Is.Anything, Arg<string>.Is.Anything, Arg<ConnectionStringPointer>.Is.Anything));
        }

        [TestMethod]
        public void should_not_call_audit_insert_if_serviceTaskId_is_negative1()
        {
            //arrange
            var sm = ScriptMessage.CreateScriptMessageForTest(xml);
            var changeTask = new RxTaskModel
            {
                ScriptMessage = sm,
                Rx = new Rx()
            };

            var changeRxMock = MockRepository.GenerateMock<IChangeRx>();

            var prescriptionMock = MockRepository.GenerateStub<IPrescription>();

            var scriptMessageMock = MockRepository.GenerateMock<IScriptMessage>();
            scriptMessageMock.Stub(x => x.SendThisMessage(new Guid(), new Guid(), new Guid(), ConnectionStringPointer.SHARED_DB)).IgnoreArguments().Return(-1);

            var auditMock = MockRepository.GenerateStub<IAudit>();
            
            //act
            new ChangeRxTask().CompleteApprovePriorAuthWorkflow(changeTask, changeRxMock, prescriptionMock, scriptMessageMock, auditMock);

            //assert
            auditMock.AssertWasNotCalled(x => x.InsertAuditLogPatientServiceTask(Arg<int>.Is.Anything, Arg<string>.Is.Anything, Arg<ConnectionStringPointer>.Is.Anything));
        }

        [TestMethod]
        public void should_set_refillMsg_in_session()
        {
            //arrange
            var sm = ScriptMessage.CreateScriptMessageForTest(xml);
            var changeTask = new RxTaskModel
            {
                ScriptMessage = sm,
                Rx = new Rx { MedicationName = "MedName"},
                PatientFirstName = "Ethan",
                PatientLastName = "OBrien"
            };

            var changeRxMock = MockRepository.GenerateMock<IChangeRx>();

            var prescriptionMock = MockRepository.GenerateStub<IPrescription>();

            var scriptMessageMock = MockRepository.GenerateMock<IScriptMessage>();
            scriptMessageMock.Stub(x => x.SendThisMessage(new Guid(), new Guid(), new Guid(), ConnectionStringPointer.SHARED_DB)).IgnoreArguments().Return(-1);

            var auditMock = MockRepository.GenerateStub<IAudit>();
            
            //act
            var result = new ChangeRxTask().CompleteApprovePriorAuthWorkflow(changeTask, changeRxMock, prescriptionMock, scriptMessageMock, auditMock);

            //assert
            Assert.AreEqual("Change request for MedName approved for Ethan OBrien.", result);
        }
    }
}
