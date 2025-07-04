using System;
using Rhino.Mocks;
using Rx = Allscripts.Impact.Rx;
using eRxWeb.AppCode;
using ScriptMessage = Allscripts.Impact.ScriptMsg.ScriptMessage;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using eRxWeb;
using eRxWeb.State;
using Allscripts.ePrescribe.Objects;
using Allscripts.Impact;
using Allscripts.Impact.Interfaces;
using Allscripts.Impact.Tasks;
using Allscripts.Impact.Tasks.Interfaces;
using eRxWeb.AppCode.Tasks;
using IEPCSWorkflowUtils = eRxWeb.AppCode.Interfaces.IEPCSWorkflowUtils;

namespace Allscripts.ePrescribe.Test.ERxWebTests.AppCodeTests.SigAspxTests
{
    [TestClass]
    public class SigChangeRxWorkflowTests
    {
        private const string xml = "<AllscriptsMessageEnvelope><MessageContent><PrescriptionMessage><Patient><FirstName>Ethan</FirstName><LastName>OBrien</LastName></Patient></PrescriptionMessage></MessageContent></AllscriptsMessageEnvelope>";
        [TestMethod]
        public void function_ShouldCurrentWorkflowExit_returns_false_if_MasterPageChangeRxWorkflow_or_ifcChgRx_or_ifcEPCSWorkflow_is_null()
        {
            //arrange
            eRxWeb.Sig sigObj = new eRxWeb.Sig();
            bool bExpected = false;
            var ChangeRxWorkflowObj = new RxTaskModel
            {
                ScriptMessage = ScriptMessage.CreateScriptMessageForTest(xml),
                Rx = new Rx()
            };

            //Act
            bool bActual1 = sigObj.ShouldCurrentWorkflowExit(null, Guid.Empty, new ChangeRxTask(), new EPCSWorkflowUtils(), null, new SigAspx());
            bool bActual2 = sigObj.ShouldCurrentWorkflowExit(ChangeRxWorkflowObj, Guid.Empty, null, new EPCSWorkflowUtils(), null, new SigAspx());
            bool bActual3 = sigObj.ShouldCurrentWorkflowExit(ChangeRxWorkflowObj, Guid.Empty, new ChangeRxTask(), null, null, new SigAspx());
            bool bActual4 = sigObj.ShouldCurrentWorkflowExit(ChangeRxWorkflowObj, Guid.Empty, new ChangeRxTask(), new EPCSWorkflowUtils(), null, null);

            //Asssert
            Assert.AreEqual(bExpected, bActual1);
            Assert.AreEqual(bExpected, bActual2);
            Assert.AreEqual(bExpected, bActual3);
            Assert.AreEqual(bExpected, bActual4);
        }

        [TestMethod]
        public void function_ShouldCurrentWorkflowExit_returns_true_if_EPCSCHGRX_Workflow_Expected()
        {
            //arrange
            eRxWeb.Sig sigObj = new eRxWeb.Sig();
            bool bExpected = true;
            var ChangeRxWorkflowObj = new RxTaskModel
            {
                ScriptMessage = ScriptMessage.CreateScriptMessageForTest(xml),
                Rx = new Rx()
            };
            var changeRxMock = MockRepository.GenerateMock<IChangeRx>();
            EPCSParameters epcsDataFromSigPage =  new EPCSParameters();
            ChangeRxWorkflowObj.TxtDaysSupply = "10";
            ChangeRxWorkflowObj.TxtQuantity = "10";
            ChangeRxWorkflowObj.TxtRefill = "1";
            ChangeRxWorkflowObj.TxtFreeTextSig = "TAKE 1 A DAY";
            ChangeRxWorkflowObj.IsDAW = false;
            ChangeRxWorkflowObj.TxtMDD = "Sample MDD";
            var IEPCSWorkflowUtilsMock = MockRepository.GenerateMock<IEPCSWorkflowUtils>();
            IEPCSWorkflowUtilsMock.Stub(x => x.IsEPCSWorkflowExpected(epcsDataFromSigPage)).IgnoreArguments().Return(true);


            //Act
            bool bActual = sigObj.ShouldCurrentWorkflowExit(ChangeRxWorkflowObj, Guid.Empty, changeRxMock, IEPCSWorkflowUtilsMock, epcsDataFromSigPage, new SigAspx());

            //Asssert
            Assert.AreEqual(bExpected, bActual);
        }
    }
}
