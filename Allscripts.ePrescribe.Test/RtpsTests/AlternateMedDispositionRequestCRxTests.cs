using Allscripts.ePrescribe.Common;
using Allscripts.Impact.ePrescribeSvc;
using eRxWeb.ePrescribeSvc;
using eRxWeb.State;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using eRxWeb.AppCode.Interfaces;
using eRxWeb.AppCode.PptPlusBPL;
using Allscripts.ePrescribe.Objects.PPTPlus;
using System.Collections.Generic;
using eRxWeb;
using RtpsCRxDispositionRequest = eRxWeb.ePrescribeSvc.RtpsCRxDispositionRequest;

namespace Allscripts.ePrescribe.Test.RtpsTests
{
    [TestClass]
    public class AlternateMedDispositionRequestCRxTests
    {
        //[TestInitialize]
        //public void init()
        //{
        //    _session = MockRepository.GenerateStub<IStateContainer>();
        //}

        //[TestMethod]
        //public void should_return_false_when_show_rtps_false()
        //{
        //    //arrange
        //    _session.Stub(_ => _.GetBooleanOrFalse(Constants.SessionVariables.ShouldShowRtps)).Return(false);
        //    var pptPlusResponse = _session.Cast(Constants.SessionVariables.PptPlusResponses, default(PptPlusResponseContainer));
        //    var rtpsDispositionMock = MockRepository.GenerateMock<IRtpsDisposition>();
        //    //act
        //    var result = PPTPlus.AlternateMedDispositionRequestCRx("", pptPlusResponse,_session, rtpsDispositionMock, new EPSBroker());

        //    //assert
        //    Assert.AreEqual(false,result);
        //}

        //[TestMethod]
        //public void should_return_true_when_show_rtps_true_and_relatesToTxId_notEmpty()
        //{
        //    //arrange
        //    _session.Stub(_ => _.GetBooleanOrFalse(Constants.SessionVariables.ShouldShowRtps)).Return(true);
        //    var pptPlusResponse = new PptPlusResponseContainer();

        //    var cndidateMedObj = new PptPlusScriptPadCandidate();
        //    cndidateMedObj.TransactionId = "1234";
        //    cndidateMedObj.RelatesToTxId = "1234";
        //    pptPlusResponse.Candidates = new List<PptPlusScriptPadCandidate>();
        //    pptPlusResponse.Candidates.Add(cndidateMedObj);

        //    var requestObj = new RtpsCRxDispositionRequest();
        //    var rtpsDispositionMock = MockRepository.GenerateMock<IRtpsDisposition>();
        //    rtpsDispositionMock.Stub(_ => _.SendAlteredDisposition(_session,"1234", new EPSBroker())).IgnoreArguments().Return(new RtpsCRxDispositionResponse
        //    {
        //        StatusCode = 200,
        //        StatusMessage = "OK"
        //    });

        //    //act
        //    var result = PPTPlus.AlternateMedDispositionRequestCRx("1234", pptPlusResponse, _session, rtpsDispositionMock, new EPSBroker());

        //    //assert
        //    Assert.AreEqual(true, result);
        //}

        //[TestMethod]
        //public void should_return_false_when_show_rtps_true_and_relatesToTxId_Empty()
        //{
        //    //arrange
        //    _session.Stub(_ => _.GetBooleanOrFalse(Constants.SessionVariables.ShouldShowRtps)).Return(true);
        //    var pptPlusResponse = new PptPlusResponseContainer();

        //    var cndidateMedObj = new PptPlusScriptPadCandidate();
        //    cndidateMedObj.TransactionId = "1234";
        //    cndidateMedObj.RelatesToTxId = "";
        //    pptPlusResponse.Candidates = new List<PptPlusScriptPadCandidate>();
        //    pptPlusResponse.Candidates.Add(cndidateMedObj);

        //    var requestObj = new RtpsCRxDispositionRequest();
        //    var rtpsDispositionMock = MockRepository.GenerateMock<IRtpsDisposition>();
        //    rtpsDispositionMock.Stub(_ => _.SendAlteredDisposition(_session,"1234", new EPSBroker())).IgnoreArguments().Return(new RtpsCRxDispositionResponse
        //    {
        //        StatusCode = 200,
        //        StatusMessage = "OK"
        //    });

        //    //act
        //    var result = PPTPlus.AlternateMedDispositionRequestCRx("1234", pptPlusResponse, _session, rtpsDispositionMock, new EPSBroker());

        //    //assert
        //    Assert.AreEqual(false, result);
        //}

        //[TestMethod]
        //public void should_return_false_when_transactionID_not_Equal()
        //{
        //    //arrange
        //    _session.Stub(_ => _.GetBooleanOrFalse(Constants.SessionVariables.ShouldShowRtps)).Return(true);
        //    var pptPlusResponse = new PptPlusResponseContainer();

        //    var cndidateMedObj = new PptPlusScriptPadCandidate();
        //    cndidateMedObj.TransactionId = "1234";
        //    cndidateMedObj.RelatesToTxId = "1234";
        //    pptPlusResponse.Candidates = new List<PptPlusScriptPadCandidate>();
        //    pptPlusResponse.Candidates.Add(cndidateMedObj);

        //    var ScriptPadMeds = new PptPlusScriptPadMed();
        //    ScriptPadMeds.TransactionId = "1234";
        //    ScriptPadMeds.RelatesToTxId = "1234";
        //    pptPlusResponse.ScriptPadMeds = new List<PptPlusScriptPadMed>();
        //    pptPlusResponse.ScriptPadMeds.Add(ScriptPadMeds);

        //    var requestObj = new RtpsCRxDispositionRequest();
        //    var rtpsDispositionMock = MockRepository.GenerateMock<IRtpsDisposition>();
        //    rtpsDispositionMock.Stub(_ => _.SendAlteredDisposition(_session,"123", new EPSBroker())).IgnoreArguments().Return(new RtpsCRxDispositionResponse
        //    {
        //        StatusCode = 200,
        //        StatusMessage = "OK"
        //    });

        //    //act
        //    var result = PPTPlus.AlternateMedDispositionRequestCRx("123", pptPlusResponse, _session, rtpsDispositionMock, new EPSBroker());

        //    //assert
        //    Assert.AreEqual(false, result);
        //}
        
    }
}
