using System;
using System.Collections.Generic;
using System.Web;
using Allscripts.ePrescribe.Common;
using Allscripts.Impact;
using Allscripts.Impact.EPSSupport;
using Allscripts.Impact.Interfaces;
using Allscripts.Impact.Tasks;
using Allscripts.Impact.Tasks.Interfaces;
using eRxWeb.AppCode.Interfaces;
using eRxWeb.AppCode.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using Rhino.Mocks.Constraints;
using ScriptMessage = Allscripts.Impact.ScriptMsg.ScriptMessage;

namespace Allscripts.ePrescribe.Test.ERxWebTests.AppCodeTests.TasksTests.RefReqTaskTests
{
    [TestClass]
    public class ProcessApprovalTests
    {
        [TestMethod]
        public void should_return_empty_response_if_currentTask_RxGUID_isNot_NullOrEmpty()
        {
            //arrange
            var currentTask = new RxTaskModel
            {
                RxGUID = "B429C7DA-BB14-48D3-9700-407A853DFFF3",
                Rx = new Rx { MedicationName = "Super Drug" },
                ScriptMessage = new ScriptMessage()
            };

            var taskResponse = new TaskResponse();
            var scriptMessageMock = MockRepository.GenerateMock<IScriptMessage>();
            scriptMessageMock.Stub(x => x.ApproveMessage(ref currentTask)).IgnoreArguments().Return(1);

            //act
            var result = new RefillRequestRxTask().ProcessApproval(currentTask, null, null, scriptMessageMock, null, null, new AuditLogAgent(new EPSBroker(), new Audit()));


            //assert
            Assert.AreEqual(taskResponse.ExpectedEPCS, result.TaskResponse.ExpectedEPCS);
            Assert.AreEqual(taskResponse.ExpectedDUR, result.TaskResponse.ExpectedDUR);
            Assert.AreEqual(taskResponse.RefillMessage, result.TaskResponse.RefillMessage);
        }

        [TestMethod]        
        public void should_set_RedirectUrl_and_bool_value_in_response_if_db_ddi_is_nullOrEmpty()
        {
            //arrange
            var currentTask = new RxTaskModel();

            var smXmlString = "<AllscriptsMessageEnvelope><MessageContent><PrescriptionMessage><Prescription><Strength>20</Strength><DrugDescription>Super Drug</DrugDescription></Prescription></PrescriptionMessage></MessageContent></AllscriptsMessageEnvelope>";
            var drMedDictionary = new Dictionary<string, string> { { "DDI", "" } };
            var sm = ScriptMessage.CreateScriptMessageForTest(smXmlString, drMedDictionary);

            var scriptMessageMock = MockRepository.GenerateMock<IScriptMessage>();
            scriptMessageMock.Stub(x => x.GetScriptMessage(null)).IgnoreArguments().Return(sm);

            var rx = new Rx();
            var rxMock = MockRepository.GenerateMock<IRx>();
            rxMock.Stub(x => x.CreateRx(null)).IgnoreArguments().Return(rx);

            //act
            var result = new RefillRequestRxTask().ProcessApproval(currentTask, null, rxMock, scriptMessageMock, null, new ErxHttpUtility(), new AuditLogAgent(new Impact.EPSSupport.EPSBroker(), new Audit()));


            //assert
            Assert.AreEqual(Constants.PageNames.REDIRECT_TO_ANGULAR + "?componentName=" + Constants.PageNames.SELECT_MEDICATION + "?Search=A&searchText=" + System.Web.Security.AntiXss.AntiXssEncoder.UrlEncode("Super Drug" + " " + "20"), result.TaskResponse.RedirectUrl);
            Assert.AreEqual(true, result.TaskResponse.IsRedirectNeeded);
        }

        [TestMethod]        
        public void should_not_call_isDURExpected_if_db_ddi_is_nullOrEmpty()
        {
            //arrange
            var currentTask = new RxTaskModel();

            var smXmlString = "<AllscriptsMessageEnvelope><MessageContent><PrescriptionMessage><Prescription><Strength>20</Strength><DrugDescription>Super Drug</DrugDescription></Prescription></PrescriptionMessage></MessageContent></AllscriptsMessageEnvelope>";
            var drMedDictionary = new Dictionary<string, string> { { "DDI", "" } };
            var sm = ScriptMessage.CreateScriptMessageForTest(smXmlString, drMedDictionary);

            var scriptMessageMock = MockRepository.GenerateMock<IScriptMessage>();
            scriptMessageMock.Stub(x => x.GetScriptMessage(null)).IgnoreArguments().Return(sm);

            var rx = new Rx();
            var rxMock = MockRepository.GenerateMock<IRx>();
            rxMock.Stub(x => x.CreateRx(null)).IgnoreArguments().Return(rx);

            var taskingWorkflowMock = MockRepository.GenerateMock<IRxTask>();
            taskingWorkflowMock.Stub(x => x.LoadDUR()).IgnoreArguments();

            //act
            new RefillRequestRxTask().ProcessApproval(currentTask, null, rxMock, scriptMessageMock, null, new ErxHttpUtility(), new AuditLogAgent(new Allscripts.Impact.EPSSupport.EPSBroker(), new Audit()));

            //assert
            taskingWorkflowMock.AssertWasNotCalled(x => x.LoadDUR());
        }

        [TestMethod]
        public void should_call_isDURExpected_if_db_ddi_is_not_nullorEmpty()
        {
            //arrange
            var currentTask = new RxTaskModel();

            var smXmlString = "<AllscriptsMessageEnvelope><MessageContent><PrescriptionMessage><Prescription><Strength>20</Strength><DrugDescription>Super Drug</DrugDescription></Prescription></PrescriptionMessage></MessageContent></AllscriptsMessageEnvelope>";
            var drMedDictionary = new Dictionary<string, string> { { "DDI", "12345" } };
            var sm = ScriptMessage.CreateScriptMessageForTest(smXmlString, drMedDictionary);

            var scriptMessageMock = MockRepository.GenerateMock<IScriptMessage>();
            scriptMessageMock.Stub(x => x.GetScriptMessage(null)).IgnoreArguments().Return(sm);

            var rx = new Rx();
            var rxMock = MockRepository.GenerateMock<IRx>();
            rxMock.Stub(x => x.CreateRx(null)).IgnoreArguments().Return(rx);

            var taskingWorkflowMock = MockRepository.GenerateMock<IRxTask>();
            taskingWorkflowMock.Stub(x => x.LoadDUR()).IgnoreArguments().Return(DURWorkflowExpected.YES);

            var auditLogAgent = MockRepository.GenerateMock<IAuditLogAgent>();
            auditLogAgent.Stub(x => x.AddToAuditLog(null, 0)).IgnoreArguments();

            //act
            new RefillRequestRxTask(currentTask).ProcessApproval(currentTask, taskingWorkflowMock, rxMock, scriptMessageMock, null, new ErxHttpUtility(), auditLogAgent);

            //assert
            taskingWorkflowMock.AssertWasCalled(x => x.LoadDUR());
        }

        [TestMethod]
        public void should_return_IsDURExpected_Yes_in_response_object_if_DUR_is_found()
        {
            //arrange
            var currentTask = new RxTaskModel();

            var smXmlString = "<AllscriptsMessageEnvelope><MessageContent><PrescriptionMessage><Prescription><Strength>20</Strength><DrugDescription>Super Drug</DrugDescription></Prescription></PrescriptionMessage></MessageContent></AllscriptsMessageEnvelope>";
            var drMedDictionary = new Dictionary<string, string> { { "DDI", "12345" } };
            var sm = ScriptMessage.CreateScriptMessageForTest(smXmlString, drMedDictionary);

            var scriptMessageMock = MockRepository.GenerateMock<IScriptMessage>();
            scriptMessageMock.Stub(x => x.GetScriptMessage(null)).IgnoreArguments().Return(sm);

            var rx = new Rx();
            var rxMock = MockRepository.GenerateMock<IRx>();
            rxMock.Stub(x => x.CreateRx(null)).IgnoreArguments().Return(rx);

            var taskingWorkflowMock = MockRepository.GenerateMock<IRxTask>();
            taskingWorkflowMock.Stub(x => x.LoadDUR()).IgnoreArguments().Return(DURWorkflowExpected.YES);

            //act
            var result = new RefillRequestRxTask().ProcessApproval(currentTask, taskingWorkflowMock, rxMock, scriptMessageMock, null, new ErxHttpUtility(), new AuditLogAgent(new Allscripts.Impact.EPSSupport.EPSBroker(), new Audit()));

            //assert
            Assert.AreEqual(DURWorkflowExpected.YES, result.TaskResponse.ExpectedDUR);
            Assert.AreEqual(EPCSWorkflowExpected.UNKNOWN, result.TaskResponse.ExpectedEPCS);
            Assert.AreEqual(string.Empty, result.TaskResponse.RefillMessage);
        }

        [TestMethod]
        public void should_return_IsDURExpected_No_in_response_object_if_DUR_is_Not_found()
        {
            //arrange
            var currentTask = new RxTaskModel();

            var smXmlString = "<AllscriptsMessageEnvelope><MessageContent><PrescriptionMessage><Prescription><Strength>20</Strength><DrugDescription>Super Drug</DrugDescription></Prescription></PrescriptionMessage></MessageContent></AllscriptsMessageEnvelope>";
            var drMedDictionary = new Dictionary<string, string> { { "DDI", "12345" } };
            var sm = ScriptMessage.CreateScriptMessageForTest(smXmlString, drMedDictionary);

            var scriptMessageMock = MockRepository.GenerateMock<IScriptMessage>();
            scriptMessageMock.Stub(x => x.GetScriptMessage(null)).IgnoreArguments().Return(sm);

            var rx = new Rx();
            var rxMock = MockRepository.GenerateMock<IRx>();
            rxMock.Stub(x => x.CreateRx(null)).IgnoreArguments().Return(rx);

            var taskingWorkflowMock = MockRepository.GenerateMock<IRxTask>();
            taskingWorkflowMock.Stub(x => x.LoadDUR()).IgnoreArguments().Return(DURWorkflowExpected.NO);

            var auditLogAgent = MockRepository.GenerateMock<IAuditLogAgent>();
            auditLogAgent.Stub(x => x.AddToAuditLog(null, 0)).IgnoreArguments();

            //act
            var result = new RefillRequestRxTask().ProcessApproval(currentTask, taskingWorkflowMock, rxMock, scriptMessageMock, null, new ErxHttpUtility(), auditLogAgent);

            //assert
            Assert.AreEqual(DURWorkflowExpected.NO, result.TaskResponse.ExpectedDUR);
        }

        [TestMethod]
        public void should_call_IsEPCSWorkflowExpected_if_med_is_a_controlled_substance()
        {
            //arrange
            var currentTask = new RxTaskModel
            {
                IsControlledSubstance = true,
                PatientFirstName = "Peedee",
                PatientLastName = "Pirate",
                Rx = new Rx
                {
                    MedicationName = "Fancy Med"
                }
            };

            var smXmlString = "<AllscriptsMessageEnvelope><MessageContent><PrescriptionMessage><Prescription><Strength>20</Strength><DrugDescription>Super Drug</DrugDescription></Prescription></PrescriptionMessage></MessageContent></AllscriptsMessageEnvelope>";
            var drMedDictionary = new Dictionary<string, string> { { "DDI", "12345" } };
            var sm = ScriptMessage.CreateScriptMessageForTest(smXmlString, drMedDictionary);

            var scriptMessageMock = MockRepository.GenerateMock<IScriptMessage>();
            scriptMessageMock.Stub(x => x.GetScriptMessage(null)).IgnoreArguments().Return(sm);

            var rx = new Rx();
            var rxMock = MockRepository.GenerateMock<IRx>();
            rxMock.Stub(x => x.CreateRx(null)).IgnoreArguments().Return(rx);

            var taskingWorkflowMock = MockRepository.GenerateMock<IRxTask>();
            taskingWorkflowMock.Stub(x => x.LoadDUR()).IgnoreArguments().Return(DURWorkflowExpected.NO);
            taskingWorkflowMock.Stub(x => x.LoadEPCS()).IgnoreArguments().Return(EPCSWorkflowExpected.YES);

            //act
            new RefillRequestRxTask().ProcessApproval(currentTask, taskingWorkflowMock, rxMock, scriptMessageMock, null, new ErxHttpUtility(), new AuditLogAgent(new Allscripts.Impact.EPSSupport.EPSBroker(), new Audit()));

            //assert
            taskingWorkflowMock.AssertWasCalled(x => x.LoadEPCS());
        }

        [TestMethod]
        public void should_not_call_ApproveMessage_if_med_is_a_controlled_substance()
        {
            //arrange
            var currentTask = new RxTaskModel
            {
                IsControlledSubstance = true,
                PatientFirstName = "Peedee",
                PatientLastName = "Pirate",
                Rx = new Rx
                {
                    MedicationName = "Fancy Med"
                }
            };

            var smXmlString = "<AllscriptsMessageEnvelope><MessageContent><PrescriptionMessage><Prescription><Strength>20</Strength><DrugDescription>Super Drug</DrugDescription></Prescription></PrescriptionMessage></MessageContent></AllscriptsMessageEnvelope>";
            var drMedDictionary = new Dictionary<string, string> { { "DDI", "12345" } };
            var sm = ScriptMessage.CreateScriptMessageForTest(smXmlString, drMedDictionary);

            var scriptMessageMock = MockRepository.GenerateMock<IScriptMessage>();
            scriptMessageMock.Stub(x => x.GetScriptMessage(null)).IgnoreArguments().Return(sm);

            var rx = new Rx();
            var rxMock = MockRepository.GenerateMock<IRx>();
            rxMock.Stub(x => x.CreateRx(null)).IgnoreArguments().Return(rx);

            var taskingWorkflowMock = MockRepository.GenerateMock<IRxTask>();
            taskingWorkflowMock.Stub(x => x.LoadDUR()).IgnoreArguments().Return(DURWorkflowExpected.NO);
            taskingWorkflowMock.Stub(x => x.LoadEPCS()).IgnoreArguments().Return(EPCSWorkflowExpected.YES);

            //act
            new RefillRequestRxTask().ProcessApproval(currentTask, taskingWorkflowMock, rxMock, scriptMessageMock, null, new ErxHttpUtility(), new AuditLogAgent(new Allscripts.Impact.EPSSupport.EPSBroker(), new Audit()));

            //assert
            scriptMessageMock.AssertWasNotCalled(x => x.ApproveMessage(ref Arg<RxTaskModel>.Ref(Is.Anything(), null).Dummy));
        }

        [TestMethod]
        public void should_return_EPCSWorkflowExpected_YES_and_message_is_approved_if_canStartEpcs_is_true()
        {
            //arrange
            var currentTask = new RxTaskModel
            {
                IsControlledSubstance = true,
                PatientFirstName = "Peedee",
                PatientLastName = "Pirate",
                Rx = new Rx
                {
                    MedicationName = "Fancy Med"
                }

            };

            var smXmlString = "<AllscriptsMessageEnvelope><MessageContent><PrescriptionMessage><Prescription><Strength>20</Strength><DrugDescription>Super Drug</DrugDescription></Prescription></PrescriptionMessage></MessageContent></AllscriptsMessageEnvelope>";
            var drMedDictionary = new Dictionary<string, string> { { "DDI", "12345" } };
            var sm = ScriptMessage.CreateScriptMessageForTest(smXmlString, drMedDictionary);

            var scriptMessageMock = MockRepository.GenerateMock<IScriptMessage>();
            scriptMessageMock.Stub(x => x.GetScriptMessage(null)).IgnoreArguments().Return(sm);

            var rx = new Rx()
            {
                MedicationName = "Fancy Med"
            };
            var rxMock = MockRepository.GenerateMock<IRx>();
            rxMock.Stub(x => x.CreateRx(null)).IgnoreArguments().Return(rx);

            var taskingWorkflowMock = MockRepository.GenerateMock<IRxTask>();
            taskingWorkflowMock.Stub(x => x.LoadDUR()).IgnoreArguments().Return(DURWorkflowExpected.NO);
            taskingWorkflowMock.Stub(x => x.LoadEPCS()).IgnoreArguments().Return(EPCSWorkflowExpected.YES);

            //act
            var result = new RefillRequestRxTask().ProcessApproval(currentTask, taskingWorkflowMock, rxMock, scriptMessageMock, null, new ErxHttpUtility(), new AuditLogAgent(new Allscripts.Impact.EPSSupport.EPSBroker(), new Audit()));

            //assert
            Assert.AreEqual(EPCSWorkflowExpected.YES, result.TaskResponse.ExpectedEPCS);
            Assert.AreEqual("Refill for Fancy Med approved for Peedee Pirate.", result.TaskResponse.RefillMessage);
        }

        [TestMethod]
        public void should_return_EPCSWorkflowExpected_NO_and_message_is_not_approved_if_canStartEpcs_is_false()
        {
            //arrange
            var currentTask = new RxTaskModel
            {
                IsControlledSubstance = true,
                PatientFirstName = "Peedee",
                PatientLastName = "Pirate",
                Rx = new Rx
                {
                    MedicationName = "Fancy Med"
                }
            };

            var smXmlString = "<AllscriptsMessageEnvelope><MessageContent><PrescriptionMessage><Prescription><Strength>20</Strength><DrugDescription>Super Drug</DrugDescription></Prescription></PrescriptionMessage></MessageContent></AllscriptsMessageEnvelope>";
            var drMedDictionary = new Dictionary<string, string> { { "DDI", "12345" } };
            var sm = ScriptMessage.CreateScriptMessageForTest(smXmlString, drMedDictionary);

            var scriptMessageMock = MockRepository.GenerateMock<IScriptMessage>();
            scriptMessageMock.Stub(x => x.GetScriptMessage(null)).IgnoreArguments().Return(sm);

            var rx = new Rx()
            {
                MedicationName = "Fancy Med"
            };
            var rxMock = MockRepository.GenerateMock<IRx>();
            rxMock.Stub(x => x.CreateRx(null)).IgnoreArguments().Return(rx);

            var taskingWorkflowMock = MockRepository.GenerateMock<IRxTask>();
            taskingWorkflowMock.Stub(x => x.LoadDUR()).IgnoreArguments().Return(DURWorkflowExpected.NO);
            taskingWorkflowMock.Stub(x => x.LoadEPCS()).IgnoreArguments().Return(EPCSWorkflowExpected.NO);

            //act
            var result = new RefillRequestRxTask().ProcessApproval(currentTask, taskingWorkflowMock, rxMock, scriptMessageMock, null, new ErxHttpUtility(), new AuditLogAgent(new Allscripts.Impact.EPSSupport.EPSBroker(), new Audit()));

            //assert
            Assert.AreEqual(EPCSWorkflowExpected.NO, result.TaskResponse.ExpectedEPCS);
            Assert.AreEqual("Refill for Fancy Med cannot be approved for Peedee Pirate.", result.TaskResponse.RefillMessage);
        }

        [TestMethod]
        public void should_not_call_IsEPCSWorkflowExpected_if_med_is_not_a_controlled_substance()
        {
            //arrange
            var currentTask = new RxTaskModel
            {
                IsControlledSubstance = false,
                PatientFirstName = "Peedee",
                PatientLastName = "Pirate",
                Rx = new Rx
                {
                    MedicationName = "Fancy Med"
                }
            };

            var smXmlString = "<AllscriptsMessageEnvelope><MessageContent><PrescriptionMessage><Prescription><Strength>20</Strength><DrugDescription>Super Drug</DrugDescription></Prescription></PrescriptionMessage></MessageContent></AllscriptsMessageEnvelope>";
            var drMedDictionary = new Dictionary<string, string> { { "DDI", "12345" } };
            var sm = ScriptMessage.CreateScriptMessageForTest(smXmlString, drMedDictionary);

            var scriptMessageMock = MockRepository.GenerateMock<IScriptMessage>();
            scriptMessageMock.Stub(x => x.GetScriptMessage(null)).IgnoreArguments().Return(sm);

            var rx = new Rx();
            var rxMock = MockRepository.GenerateMock<IRx>();
            rxMock.Stub(x => x.CreateRx(null)).IgnoreArguments().Return(rx);

            var taskingWorkflowMock = MockRepository.GenerateMock<IRxTask>();
            taskingWorkflowMock.Stub(x => x.LoadDUR()).IgnoreArguments().Return(DURWorkflowExpected.NO);
            taskingWorkflowMock.Stub(x => x.LoadEPCS()).IgnoreArguments().Return(EPCSWorkflowExpected.NO);

            var auditLogAgent = MockRepository.GenerateMock<IAuditLogAgent>();
            auditLogAgent.Stub(x => x.AddToAuditLog(null, 0)).IgnoreArguments();

            //act
            new RefillRequestRxTask().ProcessApproval(currentTask, taskingWorkflowMock, rxMock, scriptMessageMock, null, new ErxHttpUtility(), auditLogAgent);

            //assert
            taskingWorkflowMock.AssertWasNotCalled(x => x.LoadEPCS());
        }

        [TestMethod]
        public void should_call_ApproveMessage_if_med_is_not_a_controlled_substance()
        {
            //arrange
            var currentTask = new RxTaskModel
            {
                IsControlledSubstance = false,
                PatientFirstName = "Peedee",
                PatientLastName = "Pirate",
                Rx = new Rx
                {
                    MedicationName = "Fancy Med"
                }
            };

            var smXmlString = "<AllscriptsMessageEnvelope><MessageContent><PrescriptionMessage><Prescription><Strength>20</Strength><DrugDescription>Super Drug</DrugDescription></Prescription></PrescriptionMessage></MessageContent></AllscriptsMessageEnvelope>";
            var drMedDictionary = new Dictionary<string, string> { { "DDI", "12345" } };
            var sm = ScriptMessage.CreateScriptMessageForTest(smXmlString, drMedDictionary);

            var scriptMessageMock = MockRepository.GenerateMock<IScriptMessage>();
            scriptMessageMock.Stub(x => x.GetScriptMessage(null)).IgnoreArguments().Return(sm);

            var rx = new Rx();
            var rxMock = MockRepository.GenerateMock<IRx>();
            rxMock.Stub(x => x.CreateRx(null)).IgnoreArguments().Return(rx);

            var taskingWorkflowMock = MockRepository.GenerateMock<IRxTask>();
            taskingWorkflowMock.Stub(x => x.LoadDUR()).IgnoreArguments().Return(DURWorkflowExpected.NO);
            taskingWorkflowMock.Stub(x => x.LoadEPCS()).IgnoreArguments().Return(EPCSWorkflowExpected.YES);

            var auditLogAgent = MockRepository.GenerateMock<IAuditLogAgent>();
            auditLogAgent.Stub(x => x.AddToAuditLog(null, 0)).IgnoreArguments();

            //act
            new RefillRequestRxTask().ProcessApproval(currentTask, taskingWorkflowMock, rxMock, scriptMessageMock, null, new ErxHttpUtility(), auditLogAgent);

            //assert
            scriptMessageMock.AssertWasCalled(x => x.ApproveMessage(ref Arg<RxTaskModel>.Ref(Is.Anything(), null).Dummy));
        }

        [TestMethod]
        public void should_set_epcs_and_dur_expected_to_no_and_set_message_if_not_cs_med()
        {
            //arrange
            var currentTask = new RxTaskModel
            {
                IsControlledSubstance = false,
                Rx = new Rx { MedicationName = "Super Drug" }
            };

            var smXmlString = "<AllscriptsMessageEnvelope><MessageContent><PrescriptionMessage><Prescription><Strength>20</Strength><DrugDescription>Super Drug</DrugDescription></Prescription><Patient><LastName>Pirate</LastName><FirstName>Peedee</FirstName></Patient></PrescriptionMessage></MessageContent></AllscriptsMessageEnvelope>";
            var drMedDictionary = new Dictionary<string, string> { { "DDI", "12345" }, { "DBPatientFirstName", "Peedee" }, { "DBPatientLastName", "Pirate" }, { "DBDrugName", "Super Drug" } };
            var sm = ScriptMessage.CreateScriptMessageForTest(smXmlString, drMedDictionary);

            var scriptMessageMock = MockRepository.GenerateMock<IScriptMessage>();
            scriptMessageMock.Stub(x => x.GetScriptMessage(null)).IgnoreArguments().Return(sm);

            var rx = new Rx();
            var rxMock = MockRepository.GenerateMock<IRx>();
            rxMock.Stub(x => x.CreateRx(null)).IgnoreArguments().Return(rx);

            var taskingWorkflowMock = MockRepository.GenerateMock<IRxTask>();
            taskingWorkflowMock.Stub(x => x.LoadDUR()).IgnoreArguments().Return(DURWorkflowExpected.NO);
            taskingWorkflowMock.Stub(x => x.LoadEPCS()).IgnoreArguments().Return(EPCSWorkflowExpected.YES);

            var auditLogAgent = MockRepository.GenerateMock<IAuditLogAgent>();
            auditLogAgent.Stub(x => x.AddToAuditLog(null, 0)).IgnoreArguments();

            //act
            var result = new RefillRequestRxTask().ProcessApproval(currentTask, taskingWorkflowMock, rxMock, scriptMessageMock, null, new ErxHttpUtility(), auditLogAgent);

            //assert
            Assert.AreEqual(DURWorkflowExpected.NO, result.TaskResponse.ExpectedDUR);
            Assert.AreEqual(EPCSWorkflowExpected.NO, result.TaskResponse.ExpectedEPCS);
            Assert.AreEqual("Refill for Super Drug approved for Peedee Pirate.", result.TaskResponse.RefillMessage);
        }
    }
}
