using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.Objects;
using Allscripts.ePrescribe.Test.Common;
using Allscripts.Impact;
using eRxWeb;
using eRxWeb.AppCode.DurBPL;
using eRxWeb.AppCode.DurBPL.RequestModel;
using eRxWeb.AppCode.DurBPL.ResponseModel;
using eRxWeb.Controller;
using eRxWeb.ServerModel;
using eRxWeb.State;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using static Allscripts.ePrescribe.Common.Constants;
using Rx = Allscripts.Impact.Rx;

namespace Allscripts.ePrescribe.Test.ERxWebTests.ControllerTests.RxDurReviewApiControllerTest
{
    [TestClass]
    public class RxDurReviewApiControllerTest
    {
        MockedObjects mockedObjects = new MockedObjects();
        IStateContainer _pageState;
        public void init()
        {
            _pageState = MockRepository.GenerateStub<IStateContainer>();
        }
      
        [TestMethod]
        public void is_dur_warnings_saved()
        {
            HttpContext.Current = MockedObjects.MockHttpContext();
            _pageState = new StateContainer(HttpContext.Current.Session);
            var impactDur = MockRepository.GenerateStub<IImpactDurWraper>();
            DurMultiSelect durMultiSelect = new DurMultiSelect();
            _pageState[SessionVariables.SuperPobToProvider] = mockedObjects.SetupRxToProviderList();
            var warning = new DurWarningSaveRecord(Guid.NewGuid(), int.MinValue, (int)Constants.DURWarningType.ALCOHOL_INTERACTION, (int)Constants.DUR_severity.Minor, "Test Warning", "false", int.MinValue, int.MinValue, "test warningdata", int.MinValue);
            var warningList = new List<DurWarningSaveRecord>();
            warningList.Add(warning);
            ISaveDurRequest request = MockRepository.GenerateStub<SaveDurRequest>(mockedObjects.SetupRxToProviderList(), mockedObjects.SetupRxIDList(), warningList);
            durMultiSelect.SetPageState(_pageState);
            RxDurReviewApiController rxDurRevMultiSelectApiController = new RxDurReviewApiController(DurType.MULTI_SELECT);
            var result = rxDurRevMultiSelectApiController.SaveDurWarnings(request, impactDur);
            Assert.IsNotNull(result.Payload);
        }

        [TestMethod]
        public void Exception_when_dur_warnings_saved()
        {
            HttpContext.Current = MockedObjects.MockHttpContext();
            _pageState = new StateContainer(HttpContext.Current.Session);
            IImpactDurWraper wraper = new ImpactDurWraper();
            DurMultiSelect durMultiSelect = new DurMultiSelect();
            _pageState[SessionVariables.SuperPobToProvider] = mockedObjects.SetupRxToProviderList();
            var warning = new DurWarningSaveRecord(Guid.NewGuid(), int.MinValue, (int)Constants.DURWarningType.ALCOHOL_INTERACTION, (int)Constants.DUR_severity.Minor, "Test Warning", "false", int.MinValue, int.MinValue, "test warningdata", int.MinValue);
            var warningList = new List<DurWarningSaveRecord>();
            warningList.Add(warning);
            durMultiSelect.SetPageState(_pageState);
            RxDurReviewApiController rxDurRevMultiSelectApiController = new RxDurReviewApiController(DurType.MULTI_SELECT);
            var result = rxDurRevMultiSelectApiController.SaveDurWarnings(new SaveDurRequest(mockedObjects.SetupRxToProviderList(), mockedObjects.SetupRxIDList(), warningList), wraper);
            Assert.IsNotNull(result.ErrorContext);
        }

        [TestMethod]
        public void is_CSMedRefillRequestNotAllowedOnPrintRefillRequest_posted()
        {
            HttpContext.Current = MockedObjects.MockHttpContext();
            var impactDur = MockRepository.GenerateStub<IImpactDurWraper>();
            Impact.ScriptMsg.ScriptMessage scriptsMessage = Impact.ScriptMsg.ScriptMessage.CreateScriptMessageForTest(MockedObjects.xmlScriptMessage);// MockRepository.GenerateStub<Allscripts.Impact.ScriptMsg.ScriptMessage>();
            var scriptMessage = impactDur.Stub(imD => imD.CreateNewScriptMessage(mockedObjects.SetUpTaskScriptMessageId().ToString(), mockedObjects.GetGuid().ToString(), mockedObjects.GetGuid().ToString(), mockedObjects.SetUpDBId())).Return(scriptsMessage);
            _pageState = new StateContainer(HttpContext.Current.Session);
            _pageState[SessionVariables.TaskScriptMessageId] = mockedObjects.GetGuid();
            _pageState[SessionVariables.LicenseId] = mockedObjects.GetGuid();
            _pageState[SessionVariables.UserId] = mockedObjects.GetGuid();
            _pageState[SessionVariables.DbId] = mockedObjects.SetUpDBId();
            _pageState[SessionVariables.SiteId] = 1;
            _pageState[SessionVariables.isCSRegistryChecked] = false;
            _pageState[SessionVariables.PerformFormulary] = "PerformFormulary";
            _pageState[SessionVariables.RxWorkFlow] = PrescriptionWorkFlow.AFTER_FIRST_FILL;
            _pageState[SessionVariables.ExternalFacilityCd] = "ExternalFacilityCd";
            _pageState[SessionVariables.ExternalGroupID] = "ExternalGroupID";
            DurMultiSelect durMultiSelect = new DurMultiSelect();
            durMultiSelect.SetPageState(_pageState);
            RxDurReviewApiController rxDurRevMultiSelectApiController = new RxDurReviewApiController(DurType.MULTI_SELECT);
            var requestOnPrintRefillRequestMock = MockRepository.GenerateStub<ICSMedRefillNotAllowedPrintRefillRequest>();
            var res = rxDurRevMultiSelectApiController.CSMedRefillRequestNotAllowedOnPrintRefill(requestOnPrintRefillRequestMock, impactDur);
            Assert.IsNotNull(res.Payload);

        }

        [TestMethod]
        public void SingleSelect_CSMedRefillRequestNotAllowedOnPrintRefillRequest_with_PharmactDetails_deails_posted_Test()
        {
            HttpContext.Current = MockedObjects.MockHttpContext();
            var impactDur = MockRepository.GenerateStub<IImpactDurWraper>();
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            DataColumn statePharmacy =
                 dt.Columns.Add("State", typeof(string));
            statePharmacy.DefaultValue = "CA";

            dt.Rows.Add((statePharmacy));
            dt.NewRow();
            dt.Rows[0][0] = "CA";
            ds.Tables.Add(dt);
            Allscripts.Impact.ScriptMsg.ScriptMessage scriptsMessage = Impact.ScriptMsg.ScriptMessage.CreateScriptMessageForTest(MockedObjects.xmlScriptMessage);// MockRepository.GenerateStub<Allscripts.Impact.ScriptMsg.ScriptMessage>();
            var scriptMessage = impactDur.Stub(imD => imD.CreateNewScriptMessage(mockedObjects.SetUpTaskScriptMessageId().ToString(), mockedObjects.GetGuid().ToString(), mockedObjects.GetGuid().ToString(), mockedObjects.SetUpDBId())).IgnoreArguments().Return(scriptsMessage);
            var loadPharamacyResponse = MockRepository.GenerateMock<LoadPharamacyResponse>();
            loadPharamacyResponse.pharmDS = ds;
            impactDur.Stub(imD => imD.LoadPharmacy(mockedObjects.SetUpTaskScriptMessageId().ToString(), mockedObjects.GetGuid().ToString(), mockedObjects.GetGuid().ToString(), mockedObjects.SetUpDBId())).Return(loadPharamacyResponse);

            _pageState = new StateContainer(HttpContext.Current.Session);
            _pageState[SessionVariables.TaskScriptMessageId] = mockedObjects.GetGuid();
            _pageState[SessionVariables.LicenseId] = mockedObjects.GetGuid();
            _pageState[SessionVariables.UserId] = mockedObjects.GetGuid();
            _pageState[SessionVariables.DbId] = mockedObjects.SetUpDBId();
            _pageState[SessionVariables.SiteId] = 1;
            _pageState[SessionVariables.isCSRegistryChecked] = false;
            _pageState[SessionVariables.PerformFormulary] = "PerformFormulary";
            _pageState[SessionVariables.RxWorkFlow] = PrescriptionWorkFlow.AFTER_FIRST_FILL;
            _pageState[SessionVariables.ExternalFacilityCd] = "ExternalFacilityCd";
            _pageState[SessionVariables.ExternalGroupID] = "ExternalGroupID";
            DurMultiSelect durMultiSelect = new DurMultiSelect();
            durMultiSelect.SetPageState(_pageState);
            RxDurReviewApiController rxDurRevMultiSelectApiController = new RxDurReviewApiController(DurType.SINGLE_SELECT);
            var requestOnPrintRefillRequestMock = MockRepository.GenerateStub<ICSMedRefillNotAllowedPrintRefillRequest>();
            var res = rxDurRevMultiSelectApiController.CSMedRefillRequestNotAllowedOnPrintRefill(requestOnPrintRefillRequestMock, impactDur);
            Assert.IsNotNull(res.Payload);

        }

        [TestMethod]
        public void CSMedRefillRequestNotAllowedOnPrintRefillRequest_with_PharmactDetails_deails_posted_Test()
        {
            HttpContext.Current = MockedObjects.MockHttpContext();
            var impactDur = MockRepository.GenerateStub<IImpactDurWraper>();
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            DataColumn statePharmacy =
                 dt.Columns.Add("State", typeof(string));
            statePharmacy.DefaultValue = "CA";
          
            dt.Rows.Add((statePharmacy));
            dt.NewRow();
            dt.Rows[0][0]= "CA";
            ds.Tables.Add(dt);
            Allscripts.Impact.ScriptMsg.ScriptMessage scriptsMessage = Impact.ScriptMsg.ScriptMessage.CreateScriptMessageForTest(MockedObjects.xmlScriptMessage);// MockRepository.GenerateStub<Allscripts.Impact.ScriptMsg.ScriptMessage>();
            var scriptMessage = impactDur.Stub(imD => imD.CreateNewScriptMessage(mockedObjects.SetUpTaskScriptMessageId().ToString(), mockedObjects.GetGuid().ToString(), mockedObjects.GetGuid().ToString(), mockedObjects.SetUpDBId())).IgnoreArguments().Return(scriptsMessage);
            var loadPharamacyResponse = MockRepository.GenerateMock<LoadPharamacyResponse>();
            loadPharamacyResponse.pharmDS = ds;
            impactDur.Stub(imD => imD.LoadPharmacy(mockedObjects.SetUpTaskScriptMessageId().ToString(), mockedObjects.GetGuid().ToString(), mockedObjects.GetGuid().ToString(), mockedObjects.SetUpDBId())).Return(loadPharamacyResponse);

            _pageState = new StateContainer(HttpContext.Current.Session);
            _pageState[SessionVariables.TaskScriptMessageId] = mockedObjects.GetGuid();
            _pageState[SessionVariables.LicenseId] = mockedObjects.GetGuid();
            _pageState[SessionVariables.UserId] = mockedObjects.GetGuid();
            _pageState[SessionVariables.DbId] = mockedObjects.SetUpDBId();
            _pageState[SessionVariables.SiteId] = 1;
            _pageState[SessionVariables.isCSRegistryChecked] = false;
            _pageState[SessionVariables.PerformFormulary] = "PerformFormulary";
            _pageState[SessionVariables.RxWorkFlow] = PrescriptionWorkFlow.AFTER_FIRST_FILL;
            _pageState[SessionVariables.ExternalFacilityCd] = "ExternalFacilityCd";
            _pageState[SessionVariables.ExternalGroupID] = "ExternalGroupID";
            DurMultiSelect durMultiSelect = new DurMultiSelect();
            durMultiSelect.SetPageState(_pageState);
            RxDurReviewApiController rxDurRevMultiSelectApiController = new RxDurReviewApiController(DurType.MULTI_SELECT);
            var requestOnPrintRefillRequestMock = MockRepository.GenerateStub<ICSMedRefillNotAllowedPrintRefillRequest>();
            var res = rxDurRevMultiSelectApiController.CSMedRefillRequestNotAllowedOnPrintRefill(requestOnPrintRefillRequestMock, impactDur);
            Assert.IsNotNull(res.Payload);

        }

        [TestMethod]
        public void Exception_CSMedRefillRequestNotAllowedOnPrintRefillRequest_posted()
        {
            HttpContext.Current = MockedObjects.MockHttpContext();
            var impactDur = MockRepository.GenerateStub<IImpactDurWraper>();
            _pageState = new StateContainer(HttpContext.Current.Session);
            _pageState[SessionVariables.LicenseId] = mockedObjects.GetGuid();
            _pageState[SessionVariables.UserId] = mockedObjects.GetGuid();
            _pageState[SessionVariables.DbId] = mockedObjects.SetUpDBId();
            _pageState[SessionVariables.SiteId] = 1;
            _pageState[SessionVariables.isCSRegistryChecked] = false;
            _pageState[SessionVariables.PerformFormulary] = "PerformFormulary";
            _pageState[SessionVariables.RxWorkFlow] = PrescriptionWorkFlow.AFTER_FIRST_FILL;
            _pageState[SessionVariables.ExternalFacilityCd] = "ExternalFacilityCd";
            _pageState[SessionVariables.ExternalGroupID] = "ExternalGroupID";
            DurMultiSelect durMultiSelect = new DurMultiSelect();
            durMultiSelect.SetPageState(_pageState);
            RxDurReviewApiController rxDurRevMultiSelectApiController = new RxDurReviewApiController(DurType.MULTI_SELECT);
            var requestOnPrintRefillRequestMock = MockRepository.GenerateStub<ICSMedRefillNotAllowedPrintRefillRequest>();
            var res = rxDurRevMultiSelectApiController.CSMedRefillRequestNotAllowedOnPrintRefill(requestOnPrintRefillRequestMock, impactDur);
            Assert.IsNotNull(res.ErrorContext);

        }

        [TestMethod]
        public void Exception_Single_CSMedRefillRequestNotAllowedOnPrintRefillRequest_posted()
        {
            HttpContext.Current = MockedObjects.MockHttpContext();
            var impactDur = MockRepository.GenerateStub<IImpactDurWraper>();
            _pageState = new StateContainer(HttpContext.Current.Session);
            _pageState[SessionVariables.LicenseId] = mockedObjects.GetGuid();
            _pageState[SessionVariables.UserId] = mockedObjects.GetGuid();
            _pageState[SessionVariables.DbId] = mockedObjects.SetUpDBId();
            _pageState[SessionVariables.SiteId] = 1;
            _pageState[SessionVariables.isCSRegistryChecked] = false;
            _pageState[SessionVariables.PerformFormulary] = "PerformFormulary";
            _pageState[SessionVariables.RxWorkFlow] = PrescriptionWorkFlow.AFTER_FIRST_FILL;
            _pageState[SessionVariables.ExternalFacilityCd] = "ExternalFacilityCd";
            _pageState[SessionVariables.ExternalGroupID] = "ExternalGroupID";
            DurSingleSelect single = new DurSingleSelect();
            single.SetPageState(_pageState);
            RxDurReviewApiController rxDurRevMultiSelectApiController = new RxDurReviewApiController(DurType.SINGLE_SELECT);
            ICSMedRefillNotAllowedPrintRefillRequest requestOnPrintRefillRequestMock = null;// MockRepository.GenerateStub<ICSMedRefillNotAllowedPrintRefillRequest>();
            var res = rxDurRevMultiSelectApiController.CSMedRefillRequestNotAllowedOnPrintRefill(requestOnPrintRefillRequestMock, impactDur);
            Assert.IsNotNull(res.ErrorContext);

        }

        [TestMethod]
        public void go_back_when_IsCsRefReqWorkflow_is_null_Test()
        {
            HttpContext.Current = MockedObjects.MockHttpContext();
            _pageState = new StateContainer(HttpContext.Current.Session);
            _pageState[SessionVariables.LicenseId] = mockedObjects.GetGuid();
            _pageState[SessionVariables.UserId] = mockedObjects.GetGuid();
            _pageState[SessionVariables.IsCsRefReqWorkflow] =null;
            _pageState[SessionVariables.DUR_GO_PREVIOUS] = string.Empty;
            _pageState[SessionVariables.DbId] = mockedObjects.SetUpDBId();
            _pageState[SessionVariables.PatientId] = "Patient1";
            Rx rx = new Rx();
            List<Rx> rxList = new List<Rx>();
            rxList.Add(rx);
            _pageState[SessionVariables.CurrentScriptPadMeds] = rxList;
            DurMultiSelect durMultiSelect = new DurMultiSelect();
            durMultiSelect.SetPageState(_pageState);
            RxDurReviewApiController rxDurRevMultiSelectApiController = new RxDurReviewApiController(DurType.MULTI_SELECT);
            GoBackRequest goBackRequest = new GoBackRequest(new Impact.Tasks.ChangeRxRequestedMedCs());
            var res = rxDurRevMultiSelectApiController.GoBack(goBackRequest);
            Assert.IsNotNull(res);

        }

        [TestMethod]
        public void Exception_go_back_when_IsCsRefReqWorkflow_is_null_Test()
        {
            DurMultiSelect durMultiSelect = new DurMultiSelect();
            HttpContext.Current = MockedObjects.MockHttpContext();
            _pageState = new StateContainer(HttpContext.Current.Session);
            durMultiSelect.SetPageState(_pageState);
            RxDurReviewApiController rxDurRevMultiSelectApiController = new RxDurReviewApiController(DurType.MULTI_SELECT);
            GoBackRequest goBackRequest = new GoBackRequest(new Impact.Tasks.ChangeRxRequestedMedCs());
            ApiResponse res = rxDurRevMultiSelectApiController.GoBack(goBackRequest);
            Assert.IsNotNull(res.ErrorContext);

        }

        [TestMethod]
        public void Is_Post_CSMedRefillRequestNotAllowedOnContactProvider_Sucessfull_Test()
        {
            HttpContext.Current = MockedObjects.MockHttpContext();
            var impactDur = MockRepository.GenerateStub<IImpactDurWraper>();
            DurMultiSelect durMultiSelect = new DurMultiSelect();
            _pageState = new StateContainer(HttpContext.Current.Session);
            _pageState[SessionVariables.TaskScriptMessageId] = mockedObjects.GetGuid();
            _pageState[SessionVariables.LicenseId] = mockedObjects.GetGuid();
            _pageState[SessionVariables.UserId] = mockedObjects.GetGuid();
            _pageState[SessionVariables.DbId] = mockedObjects.SetUpDBId();
            durMultiSelect.SetPageState(_pageState);
            RxDurReviewApiController rxDurRevMultiSelectApiController = new RxDurReviewApiController(DurType.MULTI_SELECT);
            ICSMedRefillNotAllowedContactRequest request = MockRepository.GenerateMock<CSMedRefillNotAllowedContactRequest>(Guid.NewGuid().ToString());
            ApiResponse res = rxDurRevMultiSelectApiController.CSMedRefillRequestNotAllowedOnContactProvider(request,impactDur);
            Assert.IsNotNull(res);
        }

        [TestMethod]
        public void Exception_Is_Post_CSMedRefillRequestNotAllowedOnContactProvider_Test()
        {
            HttpContext.Current = MockedObjects.MockHttpContext();
            var impactDur = MockRepository.GenerateStub<IImpactDurWraper>();
            DurMultiSelect durMultiSelect = new DurMultiSelect();
            _pageState = new StateContainer(HttpContext.Current.Session);
            _pageState[SessionVariables.LicenseId] = mockedObjects.GetGuid();
            _pageState[SessionVariables.UserId] = mockedObjects.GetGuid();
            _pageState[SessionVariables.DbId] = mockedObjects.SetUpDBId();
            durMultiSelect.SetPageState(_pageState);
            RxDurReviewApiController rxDurRevMultiSelectApiController = new RxDurReviewApiController(DurType.MULTI_SELECT);
            ICSMedRefillNotAllowedContactRequest request = MockRepository.GenerateMock<CSMedRefillNotAllowedContactRequest>(Guid.NewGuid().ToString());
            ApiResponse res = rxDurRevMultiSelectApiController.CSMedRefillRequestNotAllowedOnContactProvider(request, impactDur);
            Assert.IsNotNull(res.ErrorContext);
        }
        [TestMethod]
        public void given_durIgnoreReasons_should_submitDur_successed_Test()
        {
            HttpContext.Current = MockedObjects.MockHttpContext();
            var impactDur = MockRepository.GenerateStub<IImpactDurWraper>();
            _pageState = new StateContainer(HttpContext.Current.Session);
            _pageState[SessionVariables.LicenseId] = mockedObjects.GetGuid();
            _pageState[SessionVariables.UserId] = mockedObjects.GetGuid();
            _pageState[SessionVariables.DbId] = mockedObjects.SetUpDBId();
            _pageState[SessionVariables.PatientId] = "Patient1";
            _pageState[SessionVariables.TaskScriptMessageId] = mockedObjects.GetGuid();
            Rx rx = new Rx();
            List<Rx> rxList = new List<Rx>();
            rxList.Add(rx);
            _pageState[SessionVariables.CurrentScriptPadMeds] = rxList;
            DurMultiSelect durMultiSelect = new DurMultiSelect();
            durMultiSelect.SetPageState(_pageState);
            DigitalSigningEventArgs args = new DigitalSigningEventArgs();
            args.Success = true;
            Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();
            keyValuePairs.Add("cs1", "med1");
            keyValuePairs.Add("cs2", "med2");
            args.SignedMeds = keyValuePairs;
            RxDurReviewApiController rxDurRevMultiSelectApiController = new RxDurReviewApiController(DurType.MULTI_SELECT);
            var res = rxDurRevMultiSelectApiController.SubmitDurForm(new SubmitDurRequest(), impactDur);
            Assert.IsNotNull(res);
        }

        [TestMethod]
        public void given_durIgnoreReasons_should_submitDur_throw_exception_Test()
        {
            HttpContext.Current = MockedObjects.MockHttpContext();
            var impactDur = MockRepository.GenerateStub<IImpactDurWraper>();
            _pageState = new StateContainer(HttpContext.Current.Session);
            DurMultiSelect durMultiSelect = new DurMultiSelect();
            durMultiSelect.SetPageState(_pageState);
            RxDurReviewApiController rxDurRevMultiSelectApiController = new RxDurReviewApiController(DurType.MULTI_SELECT);
            var res = rxDurRevMultiSelectApiController.SubmitDurForm(null, impactDur);
            Assert.IsNotNull(res.ErrorContext);
        }
    }
}
