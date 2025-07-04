using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Allscripts.ePrescribe.Objects;
using ScriptMessage = Allscripts.Impact.ScriptMsg.ScriptMessage;
using Rx = Allscripts.Impact.Rx;
using Allscripts.Impact.Interfaces;
using Allscripts.ePrescribe.Data;
using Allscripts.ePrescribe.DatabaseSelector;
using eRxWeb;
using eRxWeb.AppCode;
using eRxWeb.AppCode.Interfaces;
using Rhino.Mocks;
using Allscripts.Impact;

namespace Allscripts.ePrescribe.Test.ERxWebTests.RxProcessorTests
{
    [TestClass]
    public class RouteRxChangeWorkflowTests
    {
        [TestMethod]
        public void should_return_empty_string_when_rx_null()
        {
            //Arrange
            var ChangeRxWorkflowObj = new RxTaskModel
            {
                ScriptMessage = new ScriptMessage(),
                Rx = null,
                DbId = ConnectionStringPointer.SHARED_DB
            };

            //Act
            string actualMessage = new RxProcessor().GetApprovalMessageAndRouteChangeRxWorkflow(ChangeRxWorkflowObj, new ScriptMessage(), true, string.Empty, string.Empty, string.Empty);

            //Assert
            Assert.AreEqual(string.Empty, actualMessage);
        }

        [TestMethod]
        public void should_return_empty_string_when_ScriptMessage_null()
        {
            //Arrange
            var ChangeRxWorkflowObj = new RxTaskModel
            {
                ScriptMessage = new ScriptMessage(),
                Rx = new Rx(),
                DbId = ConnectionStringPointer.SHARED_DB
            };

            //Act
            string actualMessage = new RxProcessor().GetApprovalMessageAndRouteChangeRxWorkflow(ChangeRxWorkflowObj, null/*ScriptMessage*/, true, string.Empty, string.Empty, string.Empty);

            //Assert
            Assert.AreEqual(string.Empty, actualMessage);
        }

        [TestMethod]
        public void should_return_expected_string_when_ScriptMessage_And_Rx_And_MedicationName_Are_Not_null()
        {
            //Arrange
            var scriptMessageMock = MockRepository.GenerateMock<IScriptMessage>();
            var ChangeRxWorkflowObj = new RxTaskModel
            {
                ScriptMessage = new ScriptMessage(),
                Rx = new Rx(),
                DbId = ConnectionStringPointer.SHARED_DB
            };
            var rx = ChangeRxWorkflowObj.Rx as Rx;
            rx.MedicationName = "SAMPLEMED";
            ChangeRxWorkflowObj.Rx = rx;
            string expectedValue = "SampleRxID";
            scriptMessageMock.Stub(x => x.ApproveRxChangeMessage(null, null, null)).IgnoreArguments().Return(expectedValue);


            //Act
            string actualMessage = new RxProcessor().GetApprovalMessageAndRouteChangeRxWorkflow(ChangeRxWorkflowObj, scriptMessageMock, true, string.Empty, string.Empty, string.Empty);

            //Assert
            Assert.AreEqual("Rx Change for SAMPLEMED approved for .", actualMessage);
        }

        [TestMethod]
        public void should_return_expected_string_when_ScriptMessage_And_Rx_Are_Not_Null_But_MedicationName_Is_null()
        {
            //Arrange
            var scriptMessageMock = MockRepository.GenerateMock<IScriptMessage>();
            var ChangeRxWorkflowObj = new RxTaskModel
            {
                ScriptMessage = new ScriptMessage(),
                Rx = new Rx(),
                DbId = ConnectionStringPointer.SHARED_DB
            };
            var rx = ChangeRxWorkflowObj.Rx as Rx;
            rx.MedicationName = null;
            ChangeRxWorkflowObj.Rx = rx;
            string expectedValue = "SampleRxID";
            scriptMessageMock.Stub(x => x.ApproveRxChangeMessage(null, null, null)).IgnoreArguments().Return(expectedValue);


            //Act
            string actualMessage = new RxProcessor().GetApprovalMessageAndRouteChangeRxWorkflow(ChangeRxWorkflowObj, scriptMessageMock, true, string.Empty, string.Empty, string.Empty);

            //Assert
            Assert.AreEqual("Rx Change approved for .", actualMessage);
        }
    }
}
