//using System;
//using Allscripts.ePrescribe.DatabaseSelector;
//using Allscripts.ePrescribe.Objects;
//using Allscripts.ePrescribe.Objects.StoredProcReturnObjects;
//using Allscripts.Impact.Interfaces;
//using eRxWeb.SPA.API;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using Rhino.Mocks;
//using MessageQueue = Allscripts.Impact.MessageQueue;

//namespace Allscripts.ePrescribe.Test.ERxWebTests.SPATests.APITests.MessageQueueApiTests
//{
//    [TestClass]
//    public class GetPharmNotesLabelTests
//    {
//        [TestMethod]
//        public void should_return_no_response_received_message_if_db_return_is_null()
//        {
//            //arrange
//            var scriptMessageMock = MockRepository.GenerateMock<IScriptMessage>();
//            scriptMessageMock.Stub(x => x.GetCancelRxResponseStatus(null, ConnectionStringPointer.SHARED_DB)).IgnoreArguments().Return(null);

//            //act
//            var result = new MessageQueue().GetPharmNotesLabel(null, ConnectionStringPointer.DEBUG_DB, scriptMessageMock);

//            //assert
//            Assert.AreEqual("No response received from pharmacy.", result);
//        }

//        [TestMethod]
//        public void should_return_approve_message_if_response_is_approved()
//        {
//            //arrange
//            var message = "Medication has been cancelled.";
//            var canRxResponse = new CanRxResponse
//            {
//                Message = message,
//                Status = ObjectConstants.SimpleResponse.APPROVE
//            };

//            var scriptMessageMock = MockRepository.GenerateMock<IScriptMessage>();
//            scriptMessageMock.Stub(x => x.GetCancelRxResponseStatus(null, ConnectionStringPointer.SHARED_DB)).IgnoreArguments().Return(canRxResponse);

//            //act
//            var result = new MessageQueue().GetPharmNotesLabel(null, ConnectionStringPointer.DEBUG_DB, scriptMessageMock);

//            //assert
//            Assert.AreEqual($"Approved - {message}", result);
//        }

//        [TestMethod]
//        public void should_return_denied_message_if_response_is_denied_and_denial_reason_is_empty()
//        {
//            //arrange
//            var message = "Medication has been cancelled.";
//            var denialReason = string.Empty;
//            var canRxResponse = new CanRxResponse
//            {
//                Message = message,
//                DenialReason = denialReason,
//                Status = ObjectConstants.SimpleResponse.DENIED
//            };

//            var scriptMessageMock = MockRepository.GenerateMock<IScriptMessage>();
//            scriptMessageMock.Stub(x => x.GetCancelRxResponseStatus(null, ConnectionStringPointer.SHARED_DB)).IgnoreArguments().Return(canRxResponse);

//            //act
//            var result = new MessageQueue().GetPharmNotesLabel(null, ConnectionStringPointer.DEBUG_DB, scriptMessageMock);

//            //assert
//            Assert.AreEqual($"Denied - {message}", result);
//        }

//        [TestMethod]
//        public void should_return_denied_message_if_response_is_denied_and_denial_reason_is_not_empty()
//        {
//            //arrange
//            var message = "Medication has been cancelled.";
//            var denialReason = "Medication has already been dispensed";
//            var canRxResponse = new CanRxResponse
//            {
//                Message = message,
//                DenialReason = denialReason,
//                Status = ObjectConstants.SimpleResponse.DENIED
//            };

//            var scriptMessageMock = MockRepository.GenerateMock<IScriptMessage>();
//            scriptMessageMock.Stub(x => x.GetCancelRxResponseStatus(null, ConnectionStringPointer.SHARED_DB)).IgnoreArguments().Return(canRxResponse);

//            //act
//            var result = new MessageQueue().GetPharmNotesLabel(null, ConnectionStringPointer.DEBUG_DB, scriptMessageMock);

//            //assert
//            Assert.AreEqual($"Denied - {denialReason}", result);
//        }
//    }
//}
