using System;
using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.DatabaseSelector;
using Allscripts.ePrescribe.ExtensionMethods;
using Allscripts.Impact;
using Allscripts.Impact.Interfaces;
using Allscripts.Impact.Tasks;
using eRxWeb.AppCode;
using eRxWeb.State;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using eRxWeb.AppCode.Tasks;
using ScriptMessage = Allscripts.Impact.ScriptMsg.ScriptMessage;

namespace Allscripts.ePrescribe.Test.ERxWebTests.AppCodeTests.ChangeRxTests
{
    [TestClass]
    public class CompleteDenyPriorAuthWorkflowTests
    {
        private const string xml = "<AllscriptsMessageEnvelope><MessageContent><PrescriptionMessage><Patient><FirstName>Ethan</FirstName><LastName>OBrien</LastName></Patient></PrescriptionMessage></MessageContent></AllscriptsMessageEnvelope>";
        [TestMethod]
        public void should_use_new_script_msg_id_as_param_in_send_this_message()
        {
            //arrange
            var pharmTask = new RxTaskModel {ScriptMessageGUID = "7B360D87-9BEB-433B-835F-7BB9C0FEA66B"};

            var sm = ScriptMessage.CreateScriptMessageForTest(xml);
            var rx = new Rx();
            var newScriptMsg = new Guid("74553B14-4781-4C66-8BA8-C8AFDD1F16DF");

            var scriptMessageMock = MockRepository.GenerateMock<IScriptMessage>();
            scriptMessageMock.Stub(x => x.GetScriptMessage(new Guid(), new Guid(), new Guid(), ConnectionStringPointer.SHARED_DB)).IgnoreArguments().Return(sm);
            scriptMessageMock.Stub(x => x.createRXCHGRES_DenialMessage(null, null, null, null, null, null, null, null, null, null, 0, null, ConnectionStringPointer.SHARED_DB, Constants.IsEPCSMed.NO)).IgnoreArguments().Return(newScriptMsg);

            var rxMock = MockRepository.GenerateMock<IRx>();
            rxMock.Stub(x => x.CreateRx(null)).IgnoreArguments().Return(rx);
            
            //act
            new ChangeRxTask().CompleteDenyPriorAuthWorkflow(pharmTask, scriptMessageMock, rxMock);

            //assert
            var args = scriptMessageMock.GetArgumentsForCallsMadeOn(x => x.SendThisMessage(new Guid(), new Guid(), new Guid(), ConnectionStringPointer.SHARED_DB));
            Assert.AreEqual(newScriptMsg, args[0][0]);
        }

        [TestMethod]
        public void should_update_original_task_to_rejected()
        {
            //arrange
            var pharmTask = new RxTaskModel { ScriptMessageGUID = "7B360D87-9BEB-433B-835F-7BB9C0FEA66B" };
            var sm = ScriptMessage.CreateScriptMessageForTest(xml);
            var rx = new Rx();

            var scriptMessageMock = MockRepository.GenerateMock<IScriptMessage>();
            scriptMessageMock.Stub(x => x.GetScriptMessage(new Guid(), new Guid(), new Guid(), ConnectionStringPointer.SHARED_DB)).IgnoreArguments().Return(sm);

            var rxMock = MockRepository.GenerateMock<IRx>();
            rxMock.Stub(x => x.CreateRx(null)).IgnoreArguments().Return(rx);
            
            //act
            new ChangeRxTask().CompleteDenyPriorAuthWorkflow(pharmTask, scriptMessageMock, rxMock);

            //assert
            var args = scriptMessageMock.GetArgumentsForCallsMadeOn(x => x.updateRxTask(new Guid(), null, Constants.PrescriptionStatus.NEW, new Guid(), ConnectionStringPointer.SHARED_DB));
            Assert.AreEqual(sm.DBScriptMessageID.ToGuid(), args[0][0]);
            Assert.AreEqual(Constants.PrescriptionStatus.REJECTED, args[0][2]);
        }

        [TestMethod]
        public void should_return_new_script_message_from_create_deny()
        {
            //arrange
            var pharmTask = new RxTaskModel { ScriptMessageGUID = "7B360D87-9BEB-433B-835F-7BB9C0FEA66B" };
            var sm = ScriptMessage.CreateScriptMessageForTest(xml);
            var rx = new Rx();
            var newScriptMsg = new Guid("74553B14-4781-4C66-8BA8-C8AFDD1F16DF");

            var scriptMessageMock = MockRepository.GenerateMock<IScriptMessage>();
            scriptMessageMock.Stub(x => x.GetScriptMessage(new Guid(), new Guid(), new Guid(), ConnectionStringPointer.SHARED_DB)).IgnoreArguments().Return(sm);
            scriptMessageMock.Stub(x => x.createRXCHGRES_DenialMessage(null, null, null, null, null, null, null, null, null, null, 0, null, ConnectionStringPointer.SHARED_DB, Constants.IsEPCSMed.NO)).IgnoreArguments().Return(newScriptMsg);

            var rxMock = MockRepository.GenerateMock<IRx>();
            rxMock.Stub(x => x.CreateRx(null)).IgnoreArguments().Return(rx);
            
            //act
            var result = new ChangeRxTask().CompleteDenyPriorAuthWorkflow(pharmTask, scriptMessageMock, rxMock);

            //assert
            Assert.AreEqual(newScriptMsg, result);
        }
    }
}
