using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Allscripts.ePrescribe.Data;
using Allscripts.ePrescribe.DatabaseSelector;
using Allscripts.ePrescribe.Objects;
using Rhino.Mocks;
using Rx = Allscripts.Impact.Rx;
using ScriptMessage = Allscripts.Impact.ScriptMsg.ScriptMessage;
using Allscripts.Impact.Interfaces;
using eRxWeb.AppCode;
using eRxWeb.AppCode.Interfaces;
using eRxWeb.State;
using eRxWeb.AppCode.Tasks;
using Allscripts.Impact;
using Allscripts.Impact.Tasks;
using Allscripts.Impact.Tasks.Interfaces;

namespace Allscripts.ePrescribe.Test.ERxWebTests.AppCodeTests.ChangeRxWorkflowTests
{
    [TestClass]
    public class ChangeRxWorkFlowTests : eRxWeb.Sig
    {
        private const string xml = "<AllscriptsMessageEnvelope><MessageContent><PrescriptionMessage><Patient><FirstName>Ethan</FirstName><LastName>OBrien</LastName></Patient></PrescriptionMessage></MessageContent></AllscriptsMessageEnvelope>";
        [TestMethod]
        public void function_ApprovedChangeRxResponseRxId_returns_empty_string_when_null_ChangeRxWorkflow_object_supplied()
        {
            //Arrange
            var scriptMessageMock = MockRepository.GenerateMock<IScriptMessage>();

            //Act
            string expectedValue = string.Empty;
            string actualValue = new ChangeRxTask().ApprovedChangeRxResponseRxId(null, scriptMessageMock, string.Empty);

            //Assert                 
            Assert.AreEqual(expectedValue, actualValue);
            
        }

        [TestMethod]
        public void function_ApprovedChangeRxResponseRxId_returns_valid_string_when_valid_ChangeRxWorkflow_and_valid_RxChangeUIParameters_objects_supplied()
        {
            //Arrange
            var scriptMessageMock = MockRepository.GenerateMock<IScriptMessage>();
            var changeRxObj= new ChangeRxTask();
            var ChangeRxWorkflowObj  = new RxTaskModel
            {
                ScriptMessage = ScriptMessage.CreateScriptMessageForTest(xml),
                Rx = new Rx(),
                DbId = ConnectionStringPointer.SHARED_DB
            };
            string expectedValue = "SampleRxID";
            scriptMessageMock.Stub(x => x.ApproveRxChangeMessage(null, null, null)).IgnoreArguments().Return(expectedValue);

            //Act
            string actualValue = changeRxObj.ApprovedChangeRxResponseRxId(ChangeRxWorkflowObj, scriptMessageMock, string.Empty);

            //Assert 
            Assert.AreEqual(expectedValue, actualValue);

        }

        [TestMethod]        
        public void function_UserInputUpdatedRxChangeWorkflow_returns_valid_Object_when_valid_parameters_supplied()
        {
            //Arrange
            var changeRxObj = new ChangeRxTask();
            var ChangeRxWorkflowObj = new RxTaskModel
            {
                ScriptMessage = ScriptMessage.CreateScriptMessageForTest(xml),
                Rx = new Rx(),
                DbId = ConnectionStringPointer.SHARED_DB
            };
            ChangeRxWorkflowObj.TxtDaysSupply = "10";
            ChangeRxWorkflowObj.TxtQuantity = "10";
            ChangeRxWorkflowObj.TxtRefill = "1";
            ChangeRxWorkflowObj.TxtFreeTextSig = "TAKE 1 A DAY";
            ChangeRxWorkflowObj.IsDAW = false;
            ChangeRxWorkflowObj.TxtMDD = "SAMPLE MDD";

            //Act
            RxTaskModel modifiedRxChangeObject = changeRxObj.UserInputUpdatedRxChangeWorkflow(ChangeRxWorkflowObj, new SigAspx() );

            //Assert 
            Rx modifiedRx = modifiedRxChangeObject.Rx as Rx;
            string expectedQuantity = modifiedRx.Quantity.ToString();
            string expectedTxtDaysSupply = modifiedRx.DaysSupply.ToString();
            string expectedSigText = modifiedRx.SigText.ToString();
            string expectedTxtRefill = modifiedRx.Refills.ToString();
            bool expectedDAW = modifiedRx.DAW;
            string expectedmddVal = modifiedRx.MDD;
            Assert.AreEqual(expectedQuantity, ChangeRxWorkflowObj.TxtQuantity.ToString());
            Assert.AreEqual(expectedTxtDaysSupply, ChangeRxWorkflowObj.TxtDaysSupply.ToString());
            Assert.AreEqual(expectedSigText, ChangeRxWorkflowObj.TxtFreeTextSig.ToString()+" MDD: "+ ChangeRxWorkflowObj.TxtMDD.ToString()+" Per Day");
            Assert.AreEqual(expectedTxtRefill, ChangeRxWorkflowObj.TxtRefill.ToString());
            Assert.AreEqual(expectedDAW, ChangeRxWorkflowObj.IsDAW);
            Assert.AreEqual(expectedmddVal, ChangeRxWorkflowObj.TxtMDD);
        }


        [TestMethod]
        public void function_UserInputUpdatedRxChangeWorkflow_returns_valid_Object_with_non_EmptyStringText_when_emptySigText_supplied()
        {
            //Arrange
            var SigAspxMock = MockRepository.GenerateMock<ISigAspx>();
            var changeRxObj = new ChangeRxTask();
            var ChangeRxWorkflowObj = new RxTaskModel
            {
                ScriptMessage = ScriptMessage.CreateScriptMessageForTest(xml),
                Rx = new Rx(),
                DbId = ConnectionStringPointer.SHARED_DB
            };
            ChangeRxWorkflowObj.TxtDaysSupply = "10";
            ChangeRxWorkflowObj.TxtQuantity = "10";
            ChangeRxWorkflowObj.TxtRefill = "1";
            ChangeRxWorkflowObj.TxtFreeTextSig = "";
            ChangeRxWorkflowObj.IsDAW = false;
            ChangeRxWorkflowObj.TxtMDD = "Sample MDD";
            string expectedText = " MDD: Sample MDD Per Day";
            SigAspxMock.Stub(x => x.GetEnglishDescriptionFromSigID("", ConnectionStringPointer.SHARED_DB)).IgnoreArguments().Return(expectedText);

            //Act
            RxTaskModel modifiedRxChangeObject = changeRxObj.UserInputUpdatedRxChangeWorkflow(ChangeRxWorkflowObj, SigAspxMock);


            //Assert 
            Rx modifiedRx = modifiedRxChangeObject.Rx as Rx;
            string expectedSigText = modifiedRx.SigText.ToString();
            Assert.AreEqual(expectedSigText, expectedText);
        }
    }
}
