using System;
using System.Collections.Generic;
using Allscripts.Impact;
using eRxWeb;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using Sig = eRxWeb.Sig;

namespace Allscripts.ePrescribe.Test.SigTests
{
    [TestClass]
    public class GetPharmacyNotesTests
    {
        [TestMethod]
        public void getPharmNotes_should_call_generateMassOpiateMessage_if_state_is_MA_and_mode_is_not_Edit_and_rxTask_is_not_null()
        {
            {
                var mockOpiateMessage = MockRepository.GenerateMock<IMassOpiateMessage>();
                mockOpiateMessage.Stub(x => x.GenerateMassOpiateMessage("taskCode", "taskNotes", "MA", "GPI", false, new List<string> { "1", "2", "3", "4" })).Return("BaseMessage");
                mockOpiateMessage.Stub(x => x.GenerateMassOpiateMessage("rxCode", null, "MA", "GPI", false, new List<string> { "1", "2", "3", "4" })).Return("NewRxMessage");

                RxTaskModel taskModel = new RxTaskModel();
                taskModel.Rx = new Rx();
                taskModel.Rx.ControlledSubstanceCode = "taskCode";
                taskModel.Notes = "taskNotes";
                Rx rx = new Rx();
                rx.ControlledSubstanceCode = "rxCode";

                var rtrnValue = new Sig().GetPharmacyNotes("NotEdit", "MA", taskModel, rx, mockOpiateMessage, "GPI", false, new List<string> { "1", "2", "3", "4" });
                
                Assert.AreEqual("BaseMessage", rtrnValue);
                mockOpiateMessage.AssertWasNotCalled(x => x.GenerateMassOpiateMessage("rxCode", null, "MA", "GPI", false, new List<string> { "1", "2", "3", "4" }));
                mockOpiateMessage.AssertWasCalled(x => x.GenerateMassOpiateMessage("taskCode", "taskNotes", "MA", "GPI", false, new List<string> { "1", "2", "3", "4" }));

            }
        }

        [TestMethod]
        public void getPharmNotes_should_call_generateMassOpiateMessage_with_null_notes_if_state_is_MA_and_mode_is_not_Edit_and_rxTask_is_null()
        {
            {
                var mockOpiateMessage = MockRepository.GenerateMock<IMassOpiateMessage>();
                mockOpiateMessage.Stub(x => x.GenerateMassOpiateMessage("taskCode", "taskNotes", "MA", "GPI", false, new List<string> { "1", "2", "3", "4" })).Return("BaseMessage");
                mockOpiateMessage.Stub(x => x.GenerateMassOpiateMessage("rxCode", null, "MA", "GPI", false, new List<string> { "1", "2", "3", "4" })).Return("NewRxMessage");

                RxTaskModel taskModel = new RxTaskModel();
                taskModel.Rx = new Rx();
                taskModel.Rx.ControlledSubstanceCode = "taskCode";
                taskModel.Notes = "taskNotes";
                Rx rx = new Rx();
                rx.ControlledSubstanceCode = "rxCode";

                var rtrnValue = new Sig().GetPharmacyNotes("NotEdit", "MA", null, rx, mockOpiateMessage, "GPI", false, new List<string> { "1", "2", "3", "4" });

                Assert.AreEqual("NewRxMessage", rtrnValue);
                mockOpiateMessage.AssertWasCalled(x => x.GenerateMassOpiateMessage("rxCode", null, "MA", "GPI", false, new List<string> { "1", "2", "3", "4" }));
                mockOpiateMessage.AssertWasNotCalled(x => x.GenerateMassOpiateMessage("taskCode", "taskNotes", "MA", "GPI", false, new List<string> { "1", "2", "3", "4" }));
            }
        }

        [TestMethod]
        public void getPharmNotes_should_not_call_generateMassOpiateMessage_with_non_null_notes_if_state_is_not_MA_and_mode_is_not_Edit()
        {
            {
                var mockOpiateMessage = MockRepository.GenerateMock<IMassOpiateMessage>();
                mockOpiateMessage.Stub(x => x.GenerateMassOpiateMessage("taskCode", "taskNotes", "MA", "GPI", false, new List<string> { "1", "2", "3", "4" })).Return("BaseMessage");
                mockOpiateMessage.Stub(x => x.GenerateMassOpiateMessage("rxCode", null, "MA", "GPI", false, new List<string> { "1", "2", "3", "4" })).Return("NewRxMessage");

                RxTaskModel taskModel = new RxTaskModel();
                taskModel.Rx = new Rx();
                taskModel.Rx.ControlledSubstanceCode = "taskCode";
                taskModel.Notes = "taskNotes";
                Rx rx = new Rx();
                rx.ControlledSubstanceCode = "rxCode";

                var rtrnValue = new Sig().GetPharmacyNotes("NotEdit", "NC", null, rx, mockOpiateMessage, "GPI", false, new List<string> { "1", "2", "3", "4" });

                Assert.AreEqual("", rtrnValue);
                mockOpiateMessage.AssertWasNotCalled(x => x.GenerateMassOpiateMessage("rxCode", null, "MA", "GPI", false, new List<string> { "1", "2", "3", "4" }));
                mockOpiateMessage.AssertWasNotCalled(x => x.GenerateMassOpiateMessage("taskCode", "taskNotes", "MA", "GPI", false, new List<string> { "1", "2", "3", "4" }));
            }
        }

        [TestMethod]
        public void getPharmNotes_should_not_call_generateMassOpiateMessage_if_mode_is_edit()
        {
            {
                var mockOpiateMessage = MockRepository.GenerateMock<IMassOpiateMessage>();
                mockOpiateMessage.Stub(x => x.GenerateMassOpiateMessage("taskCode", "taskNotes", "MA", "GPI", false, new List<string> { "1", "2", "3", "4" })).Return("BaseMessage");
                mockOpiateMessage.Stub(x => x.GenerateMassOpiateMessage("rxCode", null, "MA", "GPI", false, new List<string> { "1", "2", "3", "4" })).Return("NewRxMessage");

                RxTaskModel taskModel = new RxTaskModel();
                taskModel.Rx = new Rx();
                taskModel.Rx.ControlledSubstanceCode = "taskCode";
                taskModel.Notes = "taskNotes";
                Rx rx = new Rx();
                rx.ControlledSubstanceCode = "rxCode";
                rx.Notes = "rxNotes";

                var rtrnValue = new Sig().GetPharmacyNotes("Edit", "MA", null, rx, mockOpiateMessage, "GPI", false, new List<string> { "1", "2", "3", "4" });

                Assert.AreEqual("rxNotes", rtrnValue);
                mockOpiateMessage.AssertWasNotCalled(x => x.GenerateMassOpiateMessage("rxCode", null, "MA", "GPI", false, new List<string> { "1", "2", "3", "4" }));
                mockOpiateMessage.AssertWasNotCalled(x => x.GenerateMassOpiateMessage("taskCode", "taskNotes", "MA", "GPI", false, new List<string> { "1", "2", "3", "4" }));
            }
        }
    }
}
