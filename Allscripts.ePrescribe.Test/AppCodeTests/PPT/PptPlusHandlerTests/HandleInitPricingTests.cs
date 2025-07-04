using System;
using System.Collections.Generic;
using System.Web;
using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.Objects.PPTPlus;
using Allscripts.Impact;
using eRxWeb.AppCode;
using eRxWeb.State;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using eRxWeb.AppCode.PptPlusBPL;
using eRxWeb.ePrescribeSvc;
using Allscripts.ePrescribe.Data;
using Allscripts.ePrescribe.Data.CommonComponent;
using eRxWeb.ServerModel.Request;
using eRxWeb.AppCode.Interfaces;

namespace Allscripts.ePrescribe.Test.AppCodeTests.PPT.PptPlusHandlerTests
{
    [TestClass]
    public class HandleInitPricingTests
    {
        private IStateContainer _session;
        private IPptPlus _pptPlus;
        private IPptPlusServiceBroker _pptPlusBroker;
        private IPptPlusData _pptPlusData;
        private ICommonComponentData _commonComponentData;
        private IRtpsHelper _rtpsHelper;

        [TestInitialize]
        public void init()
        {
            _session = MockRepository.GenerateStub<IStateContainer>();
            _pptPlus = MockRepository.GenerateStub<IPptPlus>();
            _pptPlusBroker = MockRepository.GenerateStub<IPptPlusServiceBroker>();
            _pptPlusData = MockRepository.GenerateStub<IPptPlusData>();
            _commonComponentData = MockRepository.GenerateStub<ICommonComponentData>();
            _rtpsHelper = MockRepository.GenerateStub<IRtpsHelper>();
        }

        [TestMethod, TestCategory("PPT")]
        public void should_call_InitiatePricingInquiry_if_candidates_is_null()
        {
            //arrange
            _session.Stub(_ => _.Cast(Constants.SessionVariables.PptPlusResponses, default(PptPlusResponseContainer))).Return(new PptPlusResponseContainer());
            _session.Stub(_ => _.Cast(Constants.SessionVariables.pptPlusTokenObject, default(GetPPTPlusSamlTokenResponse))).Return(new GetPPTPlusSamlTokenResponse { Base64SamlToken = "laksjdflkajsd", TokenRefreshTime = DateTime.Now.AddDays(1) });
            _pptPlus.Stub(_ => _.CreateMedicationInfo(null, null, null, null, null, null, null, null, false, string.Empty, DatabaseSelector.ConnectionStringPointer.SHARED_DB, _pptPlusData)).Return(new MedicationInfo { Name = "test", Strength = "st", StrengthUom = "st" });
            _pptPlus.Stub(_ => _.IsScriptPadCandidateMedInfoChanged(null, 0, "test", "st st", null, null, null, null, null, null, null, null, false, 0, "A1.0")).IgnoreArguments().Return(true);

            var configDictionary = new Dictionary<string, string>()
             {
                 {"PptPlusPricingRequestDelay", "4000"},
             };
            Impact.ConfigKeys.TestInitialize(configDictionary);
            //act
        //    PPTPlus.HandleInitPricing(null, null, null, null, null, null, null, null, null, 0, _session, _pptPlus, _pptPlusBroker, null, _pptPlusData);

            //assert
       //     _pptPlusBroker.AssertWasCalled(_ => _.InitiatePricingInquiry(Arg<string>.Is.Anything, Arg<string>.Is.Anything));
        }
        
        [TestMethod, TestCategory("PPT")]
        public void should_call_InitiatePricingInquiry_if_no_candidate_exists_with_index()
        {
            //arrange
            var responses = new PptPlusResponseContainer
            {
                Candidates = new List<PptPlusScriptPadCandidate>
                {
                    new PptPlusScriptPadCandidate(3, "", false, "", "",Guid.Empty ,"","","", "", "", "", "", "", "", "", "", false, 0,0,string.Empty)
                }
            };
            _session.Stub(_ => _.Cast(Constants.SessionVariables.PptPlusResponses, default(PptPlusResponseContainer))).Return(responses);
            _session.Stub(_ => _.Cast(Constants.SessionVariables.pptPlusTokenObject, default(GetPPTPlusSamlTokenResponse))).Return(new GetPPTPlusSamlTokenResponse { Base64SamlToken = "laksjdflkajsd", TokenRefreshTime = DateTime.Now.AddDays(1) });
            _pptPlus.Stub(_ => _.CreateMedicationInfo(null, null, null, null, null, null, null, null, false, string.Empty, DatabaseSelector.ConnectionStringPointer.SHARED_DB, _pptPlusData)).Return(new MedicationInfo { Name = "test", Strength = "st", StrengthUom = "st" });
            _pptPlus.Stub(_ => _.IsScriptPadCandidateMedInfoChanged(null, 0, "test", "st st", null, null, null, null, null, null, null, null, false,0,"A1.0")).IgnoreArguments().Return(true);

            var configDictionary = new Dictionary<string, string>()
             {
                 {"PptPlusPricingRequestDelay", "4000"}
             };
            Impact.ConfigKeys.TestInitialize(configDictionary);

            //act
       //     PPTPlus.HandleInitPricing(null, null, null, null, null, null, null, null, null, 1,  _session, _pptPlus, _pptPlusBroker, null, _pptPlusData);

            //assert
        //    _pptPlusBroker.AssertWasCalled(_ => _.InitiatePricingInquiry(Arg<string>.Is.Anything, Arg<string>.Is.Anything));
        }

        [TestMethod, TestCategory("PPT")]
        public void should_not_call_InitiatePricingInquiry_if_saml_token_is_null()
        {
            //arrange
            _session.Stub(_ => _.Cast(Constants.SessionVariables.PptPlusResponses, default(PptPlusResponseContainer))).Return(null);
            _session.Stub(_ => _.Cast(Constants.SessionVariables.pptPlusTokenObject, default(GetPPTPlusSamlTokenResponse))).Return(null);
            _session.Stub(_ => _.GetStringOrEmpty(Constants.SessionVariables.ICD10CODE)).Return(string.Empty);
            PptPlusSummaryRequest pptPlusSummaryRequest = new PptPlusSummaryRequest();

            //act
            PPTPlus.HandleInitPricing(pptPlusSummaryRequest,  _session, _pptPlus, _pptPlusBroker, null, _pptPlusData, _commonComponentData, _rtpsHelper);

            //assert
            _pptPlusBroker.AssertWasNotCalled(_ => _.InitiatePricingInquiry(Arg<string>.Is.Anything, Arg<string>.Is.Anything));
        }
    }
}
