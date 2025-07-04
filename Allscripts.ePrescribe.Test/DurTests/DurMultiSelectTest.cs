using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using eRxWeb.AppCode.DurBPL;
using Rhino.Mocks;
using eRxWeb.State;
using Allscripts.ePrescribe.Common;
using Allscripts.Impact;
using eRxWeb.AppCode.DurBPL.RequestModel;
using Allscripts.ePrescribe.Objects;
using System.Web;
using eRxWeb.AppCode.DurBPL.ResponseModel;
using Allscripts.ePrescribe.Test.Common;
using Rx = Allscripts.Impact.Rx;
using eRxWeb;
using static Allscripts.ePrescribe.Common.Constants;
using Allscripts.ePrescribe.DatabaseSelector;
using System.Data;
using System.Collections;
using Allscripts.Impact.Tasks;

namespace Allscripts.ePrescribe.Test.DurTests
{
    /// <summary>
    /// Summary description for RxDurReviewApiController
    /// </summary>
    [TestClass]
    public class DurMultiSelectTest
    {
        MockedObjects mockedObjects = new MockedObjects();
        IStateContainer _pageState;
        [TestMethod]
        public void Is_Post_CSMedRefillRequestNotAllowedOnPrintRefillRequest_Sucessfull()
        {
            HttpContext.Current = MockedObjects.MockHttpContext();
            var impactDur = MockRepository.GenerateStub<IImpactDurWraper>();
            Allscripts.Impact.ScriptMsg.ScriptMessage scriptsMessage = Impact.ScriptMsg.ScriptMessage.CreateScriptMessageForTest(MockedObjects.xmlScriptMessage);// MockRepository.GenerateStub<Allscripts.Impact.ScriptMsg.ScriptMessage>();
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
            var requestOnPrintRefillRequestMock = MockRepository.GenerateStub<CSMedRefillNotAllowedPrintRefillRequest>();
            CSMedRefillNotAllowedPrintRefillResponse res = durMultiSelect.CSMedRefillRequestNotAllowedOnPrintRefillRequest(requestOnPrintRefillRequestMock, impactDur);
            Assert.IsNotNull(res);
        }
        [TestMethod]
        public void Is_Post_with_ExternalGroupID_is_null_CSMedRefillRequestNotAllowedOnPrintRefillRequest_Sucessfull()
        {
            HttpContext.Current = MockedObjects.MockHttpContext();
            var impactDur = MockRepository.GenerateStub<IImpactDurWraper>();
            Allscripts.Impact.ScriptMsg.ScriptMessage scriptsMessage = Impact.ScriptMsg.ScriptMessage.CreateScriptMessageForTest(MockedObjects.xmlScriptMessage);// MockRepository.GenerateStub<Allscripts.Impact.ScriptMsg.ScriptMessage>();
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
            DurMultiSelect durMultiSelect = new DurMultiSelect();
            durMultiSelect.SetPageState(_pageState);
            var requestOnPrintRefillRequestMock = MockRepository.GenerateStub<CSMedRefillNotAllowedPrintRefillRequest>();
            CSMedRefillNotAllowedPrintRefillResponse res = durMultiSelect.CSMedRefillRequestNotAllowedOnPrintRefillRequest(requestOnPrintRefillRequestMock, impactDur);
            Assert.IsNotNull(res);
        }
        [TestMethod]
        public void Is_Post_with_ExternalFacilityCd_is_null_CSMedRefillRequestNotAllowedOnPrintRefillRequest_Sucessfull()
        {
            HttpContext.Current = MockedObjects.MockHttpContext();
            var impactDur = MockRepository.GenerateStub<IImpactDurWraper>();
            Allscripts.Impact.ScriptMsg.ScriptMessage scriptsMessage = Impact.ScriptMsg.ScriptMessage.CreateScriptMessageForTest(MockedObjects.xmlScriptMessage);// MockRepository.GenerateStub<Allscripts.Impact.ScriptMsg.ScriptMessage>();
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
            _pageState[SessionVariables.ExternalGroupID] = "ExternalGroupID";
            DurMultiSelect durMultiSelect = new DurMultiSelect();
            durMultiSelect.SetPageState(_pageState);
            var requestOnPrintRefillRequestMock = MockRepository.GenerateStub<CSMedRefillNotAllowedPrintRefillRequest>();
            CSMedRefillNotAllowedPrintRefillResponse res = durMultiSelect.CSMedRefillRequestNotAllowedOnPrintRefillRequest(requestOnPrintRefillRequestMock, impactDur);
            Assert.IsNotNull(res);
        }
        [TestMethod]
        public void Is_Post_RxWorkFlow_is_null_CSMedRefillRequestNotAllowedOnPrintRefillRequest_Sucessfull()
        {
            HttpContext.Current = MockedObjects.MockHttpContext();
            var impactDur = MockRepository.GenerateStub<IImpactDurWraper>();
            Allscripts.Impact.ScriptMsg.ScriptMessage scriptsMessage = Impact.ScriptMsg.ScriptMessage.CreateScriptMessageForTest(MockedObjects.xmlScriptMessage);// MockRepository.GenerateStub<Allscripts.Impact.ScriptMsg.ScriptMessage>();
            var scriptMessage = impactDur.Stub(imD => imD.CreateNewScriptMessage(mockedObjects.SetUpTaskScriptMessageId().ToString(), mockedObjects.GetGuid().ToString(), mockedObjects.GetGuid().ToString(), mockedObjects.SetUpDBId())).Return(scriptsMessage);
            _pageState = new StateContainer(HttpContext.Current.Session);
            _pageState[SessionVariables.TaskScriptMessageId] = mockedObjects.GetGuid();
            _pageState[SessionVariables.LicenseId] = mockedObjects.GetGuid();
            _pageState[SessionVariables.UserId] = mockedObjects.GetGuid();
            _pageState[SessionVariables.DbId] = mockedObjects.SetUpDBId();
            _pageState[SessionVariables.SiteId] = 1;
            _pageState[SessionVariables.isCSRegistryChecked] = false;
            _pageState[SessionVariables.PerformFormulary] = "PerformFormulary";
            _pageState[SessionVariables.ExternalFacilityCd] = "ExternalFacilityCd";
            _pageState[SessionVariables.ExternalGroupID] = "ExternalGroupID";
            DurMultiSelect durMultiSelect = new DurMultiSelect();
            durMultiSelect.SetPageState(_pageState);
            var requestOnPrintRefillRequestMock = MockRepository.GenerateStub<CSMedRefillNotAllowedPrintRefillRequest>();
            CSMedRefillNotAllowedPrintRefillResponse res = durMultiSelect.CSMedRefillRequestNotAllowedOnPrintRefillRequest(requestOnPrintRefillRequestMock, impactDur);
            Assert.IsNotNull(res);
        }
        [TestMethod]
        public void Is_Post_CSRegistryChecked_CSMedRefillRequestNotAllowedOnPrintRefillRequest_Sucessfull()
        {
            HttpContext.Current = MockedObjects.MockHttpContext();
            var impactDur = MockRepository.GenerateStub<IImpactDurWraper>();
            Allscripts.Impact.ScriptMsg.ScriptMessage scriptsMessage = Impact.ScriptMsg.ScriptMessage.CreateScriptMessageForTest(MockedObjects.xmlScriptMessage);// MockRepository.GenerateStub<Allscripts.Impact.ScriptMsg.ScriptMessage>();
            var scriptMessage = impactDur.Stub(imD => imD.CreateNewScriptMessage(mockedObjects.SetUpTaskScriptMessageId().ToString(), mockedObjects.GetGuid().ToString(), mockedObjects.GetGuid().ToString(), mockedObjects.SetUpDBId())).Return(scriptsMessage);
            _pageState = new StateContainer(HttpContext.Current.Session);
            _pageState[SessionVariables.TaskScriptMessageId] = mockedObjects.GetGuid();
            _pageState[SessionVariables.LicenseId] = mockedObjects.GetGuid();
            _pageState[SessionVariables.UserId] = mockedObjects.GetGuid();
            _pageState[SessionVariables.DbId] = mockedObjects.SetUpDBId();
            _pageState[SessionVariables.SiteId] = 1;
            _pageState[SessionVariables.isCSRegistryChecked] = true;
            _pageState[SessionVariables.PerformFormulary] = "PerformFormulary";
            _pageState[SessionVariables.RxWorkFlow] = PrescriptionWorkFlow.AFTER_FIRST_FILL;
            _pageState[SessionVariables.ExternalFacilityCd] = "ExternalFacilityCd";
            _pageState[SessionVariables.ExternalGroupID] = "ExternalGroupID";
            DurMultiSelect durMultiSelect = new DurMultiSelect();
            durMultiSelect.SetPageState(_pageState);
            var requestOnPrintRefillRequestMock = MockRepository.GenerateStub<CSMedRefillNotAllowedPrintRefillRequest>();
            CSMedRefillNotAllowedPrintRefillResponse res = durMultiSelect.CSMedRefillRequestNotAllowedOnPrintRefillRequest(requestOnPrintRefillRequestMock, impactDur);
            Assert.IsNotNull(res);
        }

        [TestMethod]
        public void Is_Post_CSMedRefillRequestNotAllowedOnContactProvider_Sucessfull()
        {
            HttpContext.Current = MockedObjects.MockHttpContext();
            var impactDur = MockRepository.GenerateStub<IImpactDurWraper>();
            DurMultiSelect durMultiSelect = new DurMultiSelect();
            _pageState = new StateContainer(HttpContext.Current.Session);
            _pageState[SessionVariables.TaskScriptMessageId] = mockedObjects.GetGuid();
            _pageState[SessionVariables.LicenseId] = mockedObjects.GetGuid();
            _pageState[SessionVariables.UserId] = mockedObjects.GetGuid();
            _pageState[SessionVariables.DbId] = mockedObjects.SetUpDBId();
            ICSMedRefillNotAllowedContactRequest request = MockRepository.GenerateMock<CSMedRefillNotAllowedContactRequest>(Guid.NewGuid().ToString());
            CSMedRefillNotAllowedContactResponse res = durMultiSelect.CSMedRefillRequestNotAllowedOnContactProvider(request, impactDur);
            Assert.IsNotNull(res);
        }

        [TestMethod]
        public void Is_Post_For_Provider_CSMedRefillRequestNotAllowedOnContactProvider_Sucessfull()
        {
            HttpContext.Current = MockedObjects.MockHttpContext();
            var impactDur = MockRepository.GenerateStub<IImpactDurWraper>();
            DurMultiSelect durMultiSelect = new DurMultiSelect();
            _pageState = new StateContainer(HttpContext.Current.Session);
            _pageState[SessionVariables.UserType] = Constants.UserCategory.PROVIDER;
            _pageState[SessionVariables.TaskScriptMessageId] = mockedObjects.GetGuid();
            _pageState[SessionVariables.LicenseId] = mockedObjects.GetGuid();
            _pageState[SessionVariables.UserId] = mockedObjects.GetGuid();
            _pageState[SessionVariables.DbId] = mockedObjects.SetUpDBId();
            ICSMedRefillNotAllowedContactRequest request = MockRepository.GenerateMock<CSMedRefillNotAllowedContactRequest>(Guid.NewGuid().ToString());
            CSMedRefillNotAllowedContactResponse res = durMultiSelect.CSMedRefillRequestNotAllowedOnContactProvider(request, impactDur);
            Assert.IsNotNull(res);
        }

        [TestMethod]
        public void Is_Post_For_POB_CSMedRefillRequestNotAllowedOnContactProvider_Sucessfull()
        {
            HttpContext.Current = MockedObjects.MockHttpContext();
            var impactDur = MockRepository.GenerateStub<IImpactDurWraper>();
            DurMultiSelect durMultiSelect = new DurMultiSelect();
            _pageState = new StateContainer(HttpContext.Current.Session);
            _pageState[SessionVariables.UserType] = Constants.UserCategory.POB_LIMITED;
            _pageState[SessionVariables.TaskScriptMessageId] = mockedObjects.GetGuid();
            _pageState[SessionVariables.LicenseId] = mockedObjects.GetGuid();
            _pageState[SessionVariables.UserId] = mockedObjects.GetGuid();
            _pageState[SessionVariables.DbId] = mockedObjects.SetUpDBId();
            ICSMedRefillNotAllowedContactRequest request = MockRepository.GenerateMock<CSMedRefillNotAllowedContactRequest>(Guid.NewGuid().ToString());
            CSMedRefillNotAllowedContactResponse res = durMultiSelect.CSMedRefillRequestNotAllowedOnContactProvider(request, impactDur);
            Assert.IsNotNull(res);
        }


        [TestMethod]
        public void is_dur_warnings_saved()
        {
            _pageState = MockRepository.GenerateMock<IStateContainer>();
            var impactDur = MockRepository.GenerateStub<IImpactDurWraper>();
            DurMultiSelect durMultiSelect = new DurMultiSelect();
            _pageState[SessionVariables.SuperPobToProvider] = mockedObjects.SetupRxToProviderList();
            var warning = new DurWarningSaveRecord(Guid.NewGuid(), int.MinValue, (int)Constants.DURWarningType.ALCOHOL_INTERACTION, (int)Constants.DUR_severity.Minor, "Test Warning", "false", int.MinValue, int.MinValue, "test warningdata", int.MinValue);
            var warningList = new List<DurWarningSaveRecord>();
            warningList.Add(warning);
            ISaveDurRequest request = MockRepository.GenerateStub<SaveDurRequest>(mockedObjects.SetupRxToProviderList(), mockedObjects.SetupRxIDList(), warningList);
            durMultiSelect.SetPageState(_pageState);
            var result = durMultiSelect.SaveDurWarnings(request, impactDur);
            Assert.AreEqual(true, result);
        }

        [TestMethod]
        public void is_dur_warnings_saved_for_POB()
        {
            _pageState = MockRepository.GenerateMock<IStateContainer>();
            var impactDur = MockRepository.GenerateStub<IImpactDurWraper>();
            DurMultiSelect durMultiSelect = new DurMultiSelect();
            _pageState[SessionVariables.UserType] = UserCategory.POB_LIMITED;
            _pageState[SessionVariables.SuperPobToProvider] = mockedObjects.SetupRxToProviderList();
            var warning = new DurWarningSaveRecord(Guid.NewGuid(), int.MinValue, (int)Constants.DURWarningType.ALCOHOL_INTERACTION, (int)Constants.DUR_severity.Minor, "Test Warning", "false", int.MinValue, int.MinValue, "test warningdata", int.MinValue);
            var warningList = new List<DurWarningSaveRecord>();
            warningList.Add(warning);
            ISaveDurRequest request = MockRepository.GenerateStub<SaveDurRequest>(mockedObjects.SetupRxToProviderList(), mockedObjects.SetupRxIDList(), warningList);
            durMultiSelect.SetPageState(_pageState);
            var result = durMultiSelect.SaveDurWarnings(request, impactDur);
            Assert.AreEqual(true, result);
        }


        [TestMethod]
        public void set_meds_with_dur()
        {
            _pageState = MockRepository.GenerateMock<IStateContainer>();
            var impactDur = MockRepository.GenerateStub<IImpactDurWraper>();
            DurMultiSelect durMultiSelect = new DurMultiSelect();
            _pageState[SessionVariables.SuperPobToProvider] = mockedObjects.SetupRxToProviderList();
            var warning = new DurWarningSaveRecord(Guid.NewGuid(), int.MinValue, (int)Constants.DURWarningType.ALCOHOL_INTERACTION, (int)Constants.DUR_severity.Minor, "Test Warning", "false", int.MinValue, int.MinValue, "test warningdata", int.MinValue);
            var warningList = new List<DurWarningSaveRecord>();
            warningList.Add(warning);
            ISaveDurRequest request = new SaveDurRequest(mockedObjects.SetupRxToProviderList(), mockedObjects.SetupRxIDList(), warningList);
            durMultiSelect.SetPageState(_pageState);
            var result = durMultiSelect.SaveDurWarnings(request, impactDur);
            Assert.AreEqual(true, result);
        }

        [TestMethod]
        public void set_medsWithDur_when_durList_is_null()
        {
            _pageState = MockRepository.GenerateMock<IStateContainer>();
            var impactDur = MockRepository.GenerateStub<IImpactDurWraper>();
            DurMultiSelect durMultiSelect = new DurMultiSelect();
            _pageState[SessionVariables.SuperPobToProvider] = mockedObjects.SetupRxToProviderList();
            var warning = new DurWarningSaveRecord(Guid.NewGuid(), int.MinValue, (int)Constants.DURWarningType.ALCOHOL_INTERACTION, (int)Constants.DUR_severity.Minor, "Test Warning", "false", int.MinValue, int.MinValue, "test warningdata", int.MinValue);
            var warningList = new List<DurWarningSaveRecord>();
            warningList.Add(warning);
            ISaveDurRequest request = new SaveDurRequest(mockedObjects.SetupRxToProviderList(), null, warningList);
            durMultiSelect.SetPageState(_pageState);
            var result = durMultiSelect.SaveDurWarnings(request, impactDur);
            Assert.AreEqual(true, result);
        }
        [TestMethod]
        public void set_medsWithDur_when_durList_is_empty()
        {
            _pageState = MockRepository.GenerateMock<IStateContainer>();
            var impactDur = MockRepository.GenerateStub<IImpactDurWraper>();
            DurMultiSelect durMultiSelect = new DurMultiSelect();
            _pageState[SessionVariables.SuperPobToProvider] = mockedObjects.SetupRxToProviderList();
            var warning = new DurWarningSaveRecord(Guid.NewGuid(), int.MinValue, (int)Constants.DURWarningType.ALCOHOL_INTERACTION, (int)Constants.DUR_severity.Minor, "Test Warning", "false", int.MinValue, int.MinValue, "test warningdata", int.MinValue);
            var warningList = new List<DurWarningSaveRecord>();
            warningList.Add(warning);
            ISaveDurRequest request = new SaveDurRequest(mockedObjects.SetupRxToProviderList(), new List<Guid>(), warningList);
            durMultiSelect.SetPageState(_pageState);
            var result = durMultiSelect.SaveDurWarnings(request, impactDur);
            Assert.AreEqual(true, result);
        }
        [TestMethod]
        public void is_dur_null_warnings_saved()
        {
            _pageState = MockRepository.GenerateMock<IStateContainer>();
            var impactDur = MockRepository.GenerateStub<IImpactDurWraper>();
            DurMultiSelect durMultiSelect = new DurMultiSelect();
            _pageState[SessionVariables.SuperPobToProvider] = mockedObjects.SetupRxToProviderList();
            ISaveDurRequest request = MockRepository.GenerateStub<SaveDurRequest>(mockedObjects.SetupRxToProviderList(), mockedObjects.SetupRxIDList(), new List<DurWarningSaveRecord>());
            durMultiSelect.SetPageState(_pageState);
            var result = durMultiSelect.SaveDurWarnings(request, impactDur);
            Assert.AreEqual(true, result);
        }

        [TestMethod]
        public void go_back_when_IsCsRefReqWorkflow_is_null_Test()
        {
            HttpContext.Current = MockedObjects.MockHttpContext();
            _pageState = new StateContainer(HttpContext.Current.Session);
            _pageState[SessionVariables.LicenseId] = mockedObjects.GetGuid();
            _pageState[SessionVariables.UserId] = mockedObjects.GetGuid();
            _pageState[SessionVariables.IsCsRefReqWorkflow] = null;
            _pageState[SessionVariables.DUR_GO_PREVIOUS] = string.Empty;
            _pageState[SessionVariables.DbId] = mockedObjects.SetUpDBId();
            _pageState[SessionVariables.PatientId] = "Patient1";
            Rx rx = new Rx();
            List<Rx> rxList = new List<Rx>();
            rxList.Add(rx);
            _pageState[SessionVariables.CurrentScriptPadMeds] = rxList;
            DurMultiSelect durMultiSelect = new DurMultiSelect();
            durMultiSelect.SetPageState(_pageState);
            GoBackRequest goBackRequest = new GoBackRequest(new Impact.Tasks.ChangeRxRequestedMedCs());
            var res = durMultiSelect.GoBack(goBackRequest);
            Assert.IsNotNull(res);

        }

        [TestMethod]
        public void go_back_when_IsCsRefReqWorkflow_is_notnull_Test()
        {
            HttpContext.Current = MockedObjects.MockHttpContext();
            _pageState = new StateContainer(HttpContext.Current.Session);
            _pageState[SessionVariables.LicenseId] = mockedObjects.GetGuid();
            _pageState[SessionVariables.UserId] = mockedObjects.GetGuid();
            _pageState[SessionVariables.IsCsRefReqWorkflow] = true;
            _pageState[SessionVariables.DUR_GO_PREVIOUS] = string.Empty;
            _pageState[SessionVariables.DbId] = mockedObjects.SetUpDBId();
            _pageState[SessionVariables.PatientId] = "Patient1";
            Rx rx = new Rx();
            List<Rx> rxList = new List<Rx>();
            rxList.Add(rx);
            _pageState[SessionVariables.CurrentScriptPadMeds] = rxList;
            DurMultiSelect durMultiSelect = new DurMultiSelect();
            durMultiSelect.SetPageState(_pageState);
            GoBackRequest goBackRequest = new GoBackRequest(new Impact.Tasks.ChangeRxRequestedMedCs());
            var res = durMultiSelect.GoBack(goBackRequest);
            Assert.IsNotNull(res);

        }

        [TestMethod]
        public void go_back_when_IsCsRefReqWorkflow_and_ChangeRxRequestedMedCs_is_null_Test()
        {
            HttpContext.Current = MockedObjects.MockHttpContext();
            _pageState = new StateContainer(HttpContext.Current.Session);
            _pageState[SessionVariables.LicenseId] = mockedObjects.GetGuid();
            _pageState[SessionVariables.UserId] = mockedObjects.GetGuid();
            _pageState[SessionVariables.DbId] = mockedObjects.SetUpDBId();
            _pageState[SessionVariables.PatientId] = "Patient1";
            Rx rx = new Rx();
            List<Rx> rxList = new List<Rx>();
            rxList.Add(rx);
            _pageState[SessionVariables.CurrentScriptPadMeds] = rxList;
            DurMultiSelect durMultiSelect = new DurMultiSelect();
            durMultiSelect.SetPageState(_pageState);
            GoBackRequest goBackRequest = new GoBackRequest(null);
            var res = durMultiSelect.GoBack(goBackRequest);
            Assert.IsNotNull(res);

        }

        [TestMethod]
        public void go_back_when_DUR_GO_PREVIOUS_is_null_Test()
        {
            HttpContext.Current = MockedObjects.MockHttpContext();
            _pageState = new StateContainer(HttpContext.Current.Session);
            _pageState[SessionVariables.LicenseId] = mockedObjects.GetGuid();
            _pageState[SessionVariables.UserId] = mockedObjects.GetGuid();
            _pageState[SessionVariables.DbId] = mockedObjects.SetUpDBId();
            _pageState[SessionVariables.PatientId] = "Patient1";
            Rx rx = new Rx();
            List<Rx> rxList = new List<Rx>();
            rxList.Add(rx);
            _pageState[SessionVariables.CurrentScriptPadMeds] = rxList;
            DurMultiSelect durMultiSelect = new DurMultiSelect();
            durMultiSelect.SetPageState(_pageState);
            GoBackRequest goBackRequest = new GoBackRequest(new Impact.Tasks.ChangeRxRequestedMedCs());
            var res = durMultiSelect.GoBack(goBackRequest);
            Assert.IsNotNull(res);

        }
        [TestMethod]
        public void EPCSDigitalSigningOnDigitalSigningComplete_when_ChangeRxRequestedMedCs_is_Null_And_TaskType_isRXCHG_Test()
        {
            HttpContext.Current = MockedObjects.MockHttpContext();
            var impactDur = MockRepository.GenerateStub<IImpactDurWraper>();
            _pageState = new StateContainer(HttpContext.Current.Session);
            _pageState[SessionVariables.LicenseId] = mockedObjects.GetGuid();
            _pageState[SessionVariables.UserId] = mockedObjects.GetGuid();
            _pageState[SessionVariables.DbId] = mockedObjects.SetUpDBId();
            _pageState[SessionVariables.PatientId] = "Patient1";
            Rx rx = new Rx();
            List<Rx> rxList = new List<Rx>();
            rxList.Add(rx);
            _pageState[SessionVariables.CurrentScriptPadMeds] = rxList;
            DurMultiSelect durMultiSelect = new DurMultiSelect();
            durMultiSelect.SetPageState(_pageState);
            EPCSDigitalSigningRequest request = new EPCSDigitalSigningRequest(new DigitalSigningEventArgs());
            var res = durMultiSelect.EPCSDigitalSigningOnDigitalSigningComplete(request, impactDur);
            Assert.IsNotNull(res);
        }
        [TestMethod]
        public void EPCSDigitalSigningOnDigitalSigningComplete_when_forcedLogedOut_Test()
        {
            HttpContext.Current = MockedObjects.MockHttpContext();
            var impactDur = MockRepository.GenerateStub<IImpactDurWraper>();
            _pageState = new StateContainer(HttpContext.Current.Session);
            _pageState[SessionVariables.LicenseId] = mockedObjects.GetGuid();
            _pageState[SessionVariables.UserId] = mockedObjects.GetGuid();
            _pageState[SessionVariables.DbId] = mockedObjects.SetUpDBId();
            _pageState[SessionVariables.PatientId] = "Patient1";
            Rx rx = new Rx();
            List<Rx> rxList = new List<Rx>();
            rxList.Add(rx);
            _pageState[SessionVariables.CurrentScriptPadMeds] = rxList;
            DurMultiSelect durMultiSelect = new DurMultiSelect();
            durMultiSelect.SetPageState(_pageState);
            DigitalSigningEventArgs args = new DigitalSigningEventArgs();
            args.ForceLogout = true;
            EPCSDigitalSigningRequest request = new EPCSDigitalSigningRequest(args);
            var res = durMultiSelect.EPCSDigitalSigningOnDigitalSigningComplete(request, impactDur);
            Assert.IsNotNull(res);
        }
        [TestMethod]
        public void EPCSDigitalSigningOnDigitalSigningComplete_when_not_forcedLogedOut_and_MessageText_is_null_Test()
        {
            HttpContext.Current = MockedObjects.MockHttpContext();
            var impactDur = MockRepository.GenerateStub<IImpactDurWraper>();
            _pageState = new StateContainer(HttpContext.Current.Session);
            _pageState[SessionVariables.LicenseId] = mockedObjects.GetGuid();
            _pageState[SessionVariables.UserId] = mockedObjects.GetGuid();
            _pageState[SessionVariables.DbId] = mockedObjects.SetUpDBId();
            _pageState[SessionVariables.PatientId] = "Patient1";
            Rx rx = new Rx();
            List<Rx> rxList = new List<Rx>();
            rxList.Add(rx);
            _pageState[SessionVariables.CurrentScriptPadMeds] = rxList;
            DurMultiSelect durMultiSelect = new DurMultiSelect();
            durMultiSelect.SetPageState(_pageState);
            DigitalSigningEventArgs args = new DigitalSigningEventArgs();
            args.ForceLogout = false;
            args.Message = null;
            EPCSDigitalSigningRequest request = new EPCSDigitalSigningRequest(args);
            var res = durMultiSelect.EPCSDigitalSigningOnDigitalSigningComplete(request, impactDur);
            Assert.IsNotNull(res);
        }
        [TestMethod]
        public void EPCSDigitalSigningOnDigitalSigningComplete_when_not_forcedLogedOut_and_MessageText_is_not_null_Test()
        {
            HttpContext.Current = MockedObjects.MockHttpContext();
            var impactDur = MockRepository.GenerateStub<IImpactDurWraper>();
            _pageState = new StateContainer(HttpContext.Current.Session);
            _pageState[SessionVariables.LicenseId] = mockedObjects.GetGuid();
            _pageState[SessionVariables.UserId] = mockedObjects.GetGuid();
            _pageState[SessionVariables.DbId] = mockedObjects.SetUpDBId();
            _pageState[SessionVariables.PatientId] = "Patient1";
            Rx rx = new Rx();
            List<Rx> rxList = new List<Rx>();
            rxList.Add(rx);
            _pageState[SessionVariables.CurrentScriptPadMeds] = rxList;
            DurMultiSelect durMultiSelect = new DurMultiSelect();
            durMultiSelect.SetPageState(_pageState);
            DigitalSigningEventArgs args = new DigitalSigningEventArgs();
            args.ForceLogout = false;
            args.Message = "Test";
            EPCSDigitalSigningRequest request = new EPCSDigitalSigningRequest(args);
            var res = durMultiSelect.EPCSDigitalSigningOnDigitalSigningComplete(request, impactDur);
            Assert.IsNotNull(res);
        }
        [TestMethod]
        public void EPCSDigitalSigningOnDigitalSigningComplete_when_DsEventArgs_Sucessfull_And_ChangeRxRequestedMedCs_is_Null_And_TaskType_isRXCHG_Test()
        {
            HttpContext.Current = MockedObjects.MockHttpContext();
            var impactDur = MockRepository.GenerateStub<IImpactDurWraper>();
            _pageState = new StateContainer(HttpContext.Current.Session);
            _pageState[SessionVariables.LicenseId] = mockedObjects.GetGuid();
            _pageState[SessionVariables.UserId] = mockedObjects.GetGuid();
            _pageState[SessionVariables.DbId] = mockedObjects.SetUpDBId();
            _pageState[SessionVariables.PatientId] = "Patient1";
            Rx rx = new Rx();
            List<Rx> rxList = new List<Rx>();
            rxList.Add(rx);
            _pageState[SessionVariables.CurrentScriptPadMeds] = rxList;
            RxTaskModel rxTaskModel = new RxTaskModel();
            rxTaskModel.Rx = new Rx();
            rxTaskModel.TaskType = PrescriptionTaskType.RXCHG;
            _pageState[SessionVariables.RxTask] = rxTaskModel;
            DurMultiSelect durMultiSelect = new DurMultiSelect();
            durMultiSelect.SetPageState(_pageState);
            DigitalSigningEventArgs args = new DigitalSigningEventArgs();
            args.Success = true;
            Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();
            keyValuePairs.Add("cs1", "med1");
            keyValuePairs.Add("cs2", "med2");
            args.SignedMeds = keyValuePairs;
            EPCSDigitalSigningRequest request = new EPCSDigitalSigningRequest(args);
            var res = durMultiSelect.EPCSDigitalSigningOnDigitalSigningComplete(request, impactDur);
            Assert.IsNotNull(res);
        }


        [TestMethod]
        public void EPCSDigitalSigningOnDigitalSigningComplete_when_DsEventArgs_Sucessfull_And_standing_is_one()
        {
            HttpContext.Current = MockedObjects.MockHttpContext();
            var impactDur = MockRepository.GenerateStub<IImpactDurWraper>();
            _pageState = new StateContainer(HttpContext.Current.Session);
            _pageState[SessionVariables.LicenseId] = mockedObjects.GetGuid();
            _pageState[SessionVariables.UserId] = mockedObjects.GetGuid();
            _pageState[SessionVariables.DbId] = mockedObjects.SetUpDBId();
            _pageState[SessionVariables.PatientId] = "Patient1";
            _pageState[SessionVariables.SiteId] = 1;
            List<Rx> rxList = mockedObjects.SetScriptsPadMeds();
            _pageState[SessionVariables.CurrentScriptPadMeds] = rxList;
            RxTaskModel rxTaskModel = new RxTaskModel();
            rxTaskModel.Rx = new Rx();
            rxTaskModel.TaskType = PrescriptionTaskType.RXCHG;
            _pageState[SessionVariables.RxTask] = rxTaskModel;
            _pageState[SessionVariables.STANDING] = 1;
            _pageState[SessionVariables.CameFrom] = "AnyPage";
            _pageState[SessionVariables.ChangeRxRequestedMedCs] = "test";
            DurMultiSelect durMultiSelect = new DurMultiSelect();
            durMultiSelect.SetPageState(_pageState);
            DigitalSigningEventArgs args = new DigitalSigningEventArgs();
            args.Success = true;
            Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();
            keyValuePairs.Add("cs1", "med1");
            keyValuePairs.Add("cs2", "med2");
            args.SignedMeds = keyValuePairs;
            EPCSDigitalSigningRequest request = new EPCSDigitalSigningRequest(args);
            var res = durMultiSelect.EPCSDigitalSigningOnDigitalSigningComplete(request, impactDur);
            Assert.IsNotNull(res);
        }
       
        [TestMethod]
        public void EPCSDigitalSigningOnDigitalSigningComplete_when_DsEventArgs_Sucessfull_And_ChangeRxRequestedMedCs_is_NotNull_And_TaskType_isRXCHG_Test()
        {
            HttpContext.Current = MockedObjects.MockHttpContext();
            var impactDur = MockRepository.GenerateStub<IImpactDurWraper>();
            _pageState = new StateContainer(HttpContext.Current.Session);
            _pageState[SessionVariables.LicenseId] = mockedObjects.GetGuid();
            _pageState[SessionVariables.UserId] = mockedObjects.GetGuid();
            _pageState[SessionVariables.DbId] = mockedObjects.SetUpDBId();
            _pageState[SessionVariables.PatientId] = "Patient1";
            _pageState[SessionVariables.SiteId] = 1;
            List<Rx> rxList =mockedObjects.SetScriptsPadMeds();
            _pageState[SessionVariables.CurrentScriptPadMeds] = rxList;
            RxTaskModel rxTaskModel = new RxTaskModel();
            rxTaskModel.Rx = new Rx();
            rxTaskModel.TaskType = PrescriptionTaskType.RXCHG;
            _pageState[SessionVariables.RxTask] = rxTaskModel;
            _pageState[SessionVariables.ChangeRxRequestedMedCs] = "test";
            DurMultiSelect durMultiSelect = new DurMultiSelect();
            durMultiSelect.SetPageState(_pageState);
            DigitalSigningEventArgs args = new DigitalSigningEventArgs();
            args.Success = true;
            Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();
            keyValuePairs.Add("cs1", "med1");
            keyValuePairs.Add("cs2", "med2");
            args.SignedMeds = keyValuePairs;
            EPCSDigitalSigningRequest request = new EPCSDigitalSigningRequest(args);
            var res = durMultiSelect.EPCSDigitalSigningOnDigitalSigningComplete(request, impactDur);
            Assert.IsNotNull(res);
        }
        [TestMethod]
        public void EPCSDigitalSigningOnDigitalSigningComplete_when_DsEventArgs_Sucessfull_And_ChangeRxRequestedMedCs_is_Null_And_TaskType_not_RXCHG_Test()
        {
            HttpContext.Current = MockedObjects.MockHttpContext();
            var impactDur = MockRepository.GenerateStub<IImpactDurWraper>();
            _pageState = new StateContainer(HttpContext.Current.Session);
            _pageState[SessionVariables.LicenseId] = mockedObjects.GetGuid();
            _pageState[SessionVariables.UserId] = mockedObjects.GetGuid();
            _pageState[SessionVariables.DbId] = mockedObjects.SetUpDBId();
            _pageState[SessionVariables.PatientId] = "Patient1";
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
            EPCSDigitalSigningRequest request = new EPCSDigitalSigningRequest(args);
            var res = durMultiSelect.EPCSDigitalSigningOnDigitalSigningComplete(request, impactDur);
            Assert.IsNotNull(res);
        }
        [TestMethod]
        public void submitDur_Test()
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
            var res = durMultiSelect.SaveDurForm(new SubmitDurRequest(), impactDur);
            Assert.IsNotNull(res);
        }
        [TestMethod]
        public void submitDur_Test_IsCapturedReasons_is_not_set()
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
            var res = durMultiSelect.SaveDurForm(new SubmitDurRequest(), impactDur);
            Assert.IsNotNull(res);
        }
        [TestMethod]
        public void submitDur_Test_IsCapturedReasons_is_not_set_AND_POBUser()
        {
            HttpContext.Current = MockedObjects.MockHttpContext();
            var impactDur = MockRepository.GenerateStub<IImpactDurWraper>();
            _pageState = new StateContainer(HttpContext.Current.Session);
            _pageState[SessionVariables.LicenseId] = mockedObjects.GetGuid();
            _pageState[SessionVariables.UserId] = mockedObjects.GetGuid();
            _pageState[SessionVariables.DbId] = mockedObjects.SetUpDBId();
            _pageState[SessionVariables.PatientId] = "Patient1";
            _pageState[SessionVariables.TaskScriptMessageId] = mockedObjects.GetGuid();
            _pageState[SessionVariables.UserType] = UserCategory.POB_LIMITED;
            Rx rx = new Rx();
            List<Rx> rxList = new List<Rx>();
            rxList.Add(rx);
            _pageState[SessionVariables.CurrentScriptPadMeds] = rxList;
            DurMultiSelect durMultiSelect = new DurMultiSelect();
            durMultiSelect.SetPageState(_pageState);
            var res = durMultiSelect.SaveDurForm(new SubmitDurRequest(), impactDur);
            Assert.IsNotNull(res);
        }

        [TestMethod]
        public void submitDur_Test_IsCapturedReasons_is_not_set_AND_POBUser_With_NextPageSet()
        {
            HttpContext.Current = MockedObjects.MockHttpContext();
            var impactDur = MockRepository.GenerateStub<IImpactDurWraper>();
            _pageState = new StateContainer(HttpContext.Current.Session);
            _pageState[SessionVariables.LicenseId] = mockedObjects.GetGuid();
            _pageState[SessionVariables.UserId] = mockedObjects.GetGuid();
            _pageState[SessionVariables.DbId] = mockedObjects.SetUpDBId();
            _pageState[SessionVariables.PatientId] = "Patient1";
            _pageState[SessionVariables.TaskScriptMessageId] = mockedObjects.GetGuid();
            _pageState[SessionVariables.UserType] = UserCategory.POB_LIMITED;
            _pageState[SessionVariables.DUR_GO_NEXT] = "AnyPageSet";
            Rx rx = new Rx();
            List<Rx> rxList = new List<Rx>();
            rxList.Add(rx);
            _pageState[SessionVariables.CurrentScriptPadMeds] = rxList;
            DurMultiSelect durMultiSelect = new DurMultiSelect();
            durMultiSelect.SetPageState(_pageState);
            var res = durMultiSelect.SaveDurForm(new SubmitDurRequest(), impactDur);
            Assert.IsNotNull(res);
        }

        [TestMethod]
        public void submitDur_for_CsMeds_And_IsCapturedReasons_is_set_to_true_Test()
        {
            HttpContext.Current = MockedObjects.MockHttpContext();
            var impactDur = MockRepository.GenerateStub<IImpactDurWraper>();
            _pageState = new StateContainer(HttpContext.Current.Session);
            _pageState[SessionVariables.LicenseId] = mockedObjects.GetGuid();
            _pageState[SessionVariables.UserId] = mockedObjects.GetGuid();
            _pageState[SessionVariables.DbId] = mockedObjects.SetUpDBId();
            _pageState[SessionVariables.PatientId] = "Patient1";
            _pageState[SessionVariables.TaskScriptMessageId] = mockedObjects.GetGuid();
            _pageState[SessionVariables.SiteId] = 1;
            _pageState["TaskScriptMessageId"] = mockedObjects.GetGuid();
            List<Rx> rxList = mockedObjects.SetScriptsPadCsMeds();
            _pageState[SessionVariables.CurrentScriptPadMeds] = rxList;
            DurMultiSelect durMultiSelect = new DurMultiSelect();
            durMultiSelect.SetPageState(_pageState);
            SubmitDurRequest submitDurRequest = new SubmitDurRequest();
            submitDurRequest.IsCapturedReasons = true;
            submitDurRequest.ProviderId = mockedObjects.GetGuidInString();
            var res = durMultiSelect.SaveDurForm(submitDurRequest, impactDur);
            Assert.IsNotNull(res);
        }

        [TestMethod]
        public void submitDur_for_empty_rxtask_and_nextpage_is_set_Test()
        {
            HttpContext.Current = MockedObjects.MockHttpContext();
            var impactDur = MockRepository.GenerateStub<IImpactDurWraper>();
            _pageState = new StateContainer(HttpContext.Current.Session);
            _pageState[SessionVariables.LicenseId] = mockedObjects.GetGuid();
            _pageState[SessionVariables.UserId] = mockedObjects.GetGuid();
            _pageState[SessionVariables.DbId] = mockedObjects.SetUpDBId();
            _pageState[SessionVariables.PatientId] = "Patient1";
            _pageState[SessionVariables.DUR_GO_NEXT] = "AnyPage";
            _pageState[SessionVariables.SiteId] = 1;
            
            DurMultiSelect durMultiSelect = new DurMultiSelect();
            durMultiSelect.SetPageState(_pageState);
            SubmitDurRequest submitDurRequest = new SubmitDurRequest();
            submitDurRequest.IsCapturedReasons = true;
            submitDurRequest.ProviderId = mockedObjects.GetGuidInString();
            var res = durMultiSelect.SaveDurForm(submitDurRequest, impactDur);
            Assert.IsNotNull(res);
        }

        [TestMethod]
        public void submitDur_for_empty_rxtask_and_empty_nextpageTest()
        {
            HttpContext.Current = MockedObjects.MockHttpContext();
            var impactDur = MockRepository.GenerateStub<IImpactDurWraper>();
            _pageState = new StateContainer(HttpContext.Current.Session);
            _pageState[SessionVariables.LicenseId] = mockedObjects.GetGuid();
            _pageState[SessionVariables.UserId] = mockedObjects.GetGuid();
            _pageState[SessionVariables.DbId] = mockedObjects.SetUpDBId();
            _pageState[SessionVariables.PatientId] = "Patient1";
            _pageState[SessionVariables.SiteId] = 1;

            DurMultiSelect durMultiSelect = new DurMultiSelect();
            durMultiSelect.SetPageState(_pageState);
            SubmitDurRequest submitDurRequest = new SubmitDurRequest();
            submitDurRequest.IsCapturedReasons = true;
            submitDurRequest.ProviderId = mockedObjects.GetGuidInString();
            var res = durMultiSelect.SaveDurForm(submitDurRequest, impactDur);
            Assert.IsNotNull(res);
        }

        [TestMethod]
        public void submitDur_for_refill_forPob_reconsonsiled_CsMeds_And_IsCapturedReasons_is_set_to_true_Test()
        {
            HttpContext.Current = MockedObjects.MockHttpContext();
            var impactDur = MockRepository.GenerateStub<IImpactDurWraper>();
            _pageState = new StateContainer(HttpContext.Current.Session);
            _pageState[SessionVariables.SupervisingProviderId] = mockedObjects.GetGuidInString();
            _pageState[SessionVariables.LicenseId] = mockedObjects.GetGuid();
            _pageState[SessionVariables.UserId] = mockedObjects.GetGuid();
            _pageState[SessionVariables.DbId] = mockedObjects.SetUpDBId();
            _pageState[SessionVariables.PatientId] = "Patient1";
            _pageState[SessionVariables.TaskScriptMessageId] = mockedObjects.GetGuid();
            _pageState[SessionVariables.SiteId] = 1;
            _pageState["TaskScriptMessageId"] = mockedObjects.GetGuid();
            List<Rx> rxList = mockedObjects.SetScriptsPadCsMeds();
            _pageState[SessionVariables.RxList] = mockedObjects.SetRxList();
            _pageState[SessionVariables.CurrentScriptPadMeds] = rxList;
            _pageState[SessionVariables.UserType] = UserCategory.POB_LIMITED;
            string token = mockedObjects.GetShieldToken();
            RxTaskModel rxTask = new RxTaskModel();
            _pageState[SessionVariables.RxTask] = rxTask;
            rxTask.TaskType = PrescriptionTaskType.RXCHG;
            _pageState[SessionVariables.TaskType] = Constants.ePATaskType.NON_PRIOR_AUTH_INITIATED;
            _pageState["SessionLicense"] = new ApplicationLicense();
            DurMultiSelect durMultiSelect = new DurMultiSelect();
            durMultiSelect.SetPageState(_pageState);
            impactDur.Stub(x => x.GetStateControlledSubstanceCode("123", "CA", "CA", mockedObjects.SetUpDBId())).Return("CA");
            impactDur.Stub(x => x.ReconcileControlledSubstanceCodes("cs01", "CA")).Return("c001");
            SubmitDurRequest submitDurRequest = new SubmitDurRequest();
            submitDurRequest.IsCapturedReasons = true;
            submitDurRequest.ProviderId = mockedObjects.GetGuidInString();
            var res = durMultiSelect.SaveDurForm(submitDurRequest, impactDur);
            Assert.IsNotNull(res);

        }

        [TestMethod]
        public void submitDur_for_refill_forPob_without_tasktype_Test()
        {
            HttpContext.Current = MockedObjects.MockHttpContext();
            var impactDur = MockRepository.GenerateStub<IImpactDurWraper>();
            _pageState = new StateContainer(HttpContext.Current.Session);
            _pageState[SessionVariables.SupervisingProviderId] = mockedObjects.GetGuidInString();
            _pageState[SessionVariables.LicenseId] = mockedObjects.GetGuid();
            _pageState[SessionVariables.UserId] = mockedObjects.GetGuid();
            _pageState[SessionVariables.DbId] = mockedObjects.SetUpDBId();
            _pageState[SessionVariables.PatientId] = "Patient1";
            _pageState[SessionVariables.TaskScriptMessageId] = mockedObjects.GetGuid();
            _pageState[SessionVariables.SiteId] = 1;
            _pageState["TaskScriptMessageId"] = mockedObjects.GetGuid();
            List<Rx> rxList = mockedObjects.SetScriptsPadCsMeds();
            _pageState[SessionVariables.RxList] = mockedObjects.SetRxList();
            _pageState[SessionVariables.CurrentScriptPadMeds] = rxList;
            _pageState[SessionVariables.UserType] = UserCategory.POB_LIMITED;
            string token = mockedObjects.GetShieldToken();
            RxTaskModel rxTask = new RxTaskModel();
            rxTask.TaskType = PrescriptionTaskType.RXCHG;
            _pageState[SessionVariables.RxTask] = rxTask;
            _pageState["SessionLicense"] = new ApplicationLicense();
            DurMultiSelect durMultiSelect = new DurMultiSelect();
            durMultiSelect.SetPageState(_pageState);
            impactDur.Stub(x => x.GetStateControlledSubstanceCode("123", "CA", "CA", mockedObjects.SetUpDBId())).Return("CA");
            impactDur.Stub(x => x.ReconcileControlledSubstanceCodes("cs01", "CA")).Return("c001");
            SubmitDurRequest submitDurRequest = new SubmitDurRequest();
            submitDurRequest.IsCapturedReasons = true;
            submitDurRequest.ProviderId = mockedObjects.GetGuidInString();
            var res = durMultiSelect.SaveDurForm(submitDurRequest, impactDur);
            Assert.IsNotNull(res);

        }
        [TestMethod]
        public void submitDur_for_refill_forPob_Test()
        {
            HttpContext.Current = MockedObjects.MockHttpContext();
            var impactDur = MockRepository.GenerateStub<IImpactDurWraper>();
            _pageState = new StateContainer(HttpContext.Current.Session);
            _pageState[SessionVariables.SupervisingProviderId] = mockedObjects.GetGuidInString();
            _pageState[SessionVariables.LicenseId] = mockedObjects.GetGuid();
            _pageState[SessionVariables.UserId] = mockedObjects.GetGuid();
            _pageState[SessionVariables.DbId] = mockedObjects.SetUpDBId();
            _pageState[SessionVariables.PatientId] = "Patient1";
            _pageState[SessionVariables.TaskScriptMessageId] = mockedObjects.GetGuid();
            _pageState[SessionVariables.SiteId] = 1;
            _pageState["TaskScriptMessageId"] = mockedObjects.GetGuid();
            List<Rx> rxList = mockedObjects.SetScriptsPadCsMeds();
            _pageState[SessionVariables.RxList] = mockedObjects.SetRxList();
            _pageState[SessionVariables.CurrentScriptPadMeds] = rxList;
            _pageState[SessionVariables.UserType] = UserCategory.POB_LIMITED;
            string token = mockedObjects.GetShieldToken();
            RxTaskModel rxTask = new RxTaskModel();
            rxTask.TaskType = PrescriptionTaskType.RXCHG;
            _pageState[SessionVariables.RxTask] = rxTask;
            _pageState[SessionVariables.TaskType] = rxTask.TaskType;
            _pageState["SessionLicense"] = new ApplicationLicense();
            DurMultiSelect durMultiSelect = new DurMultiSelect();
            durMultiSelect.SetPageState(_pageState);
            impactDur.Stub(x => x.GetStateControlledSubstanceCode("123", "CA", "CA", mockedObjects.SetUpDBId())).Return("CA");
            impactDur.Stub(x => x.ReconcileControlledSubstanceCodes("cs01", "CA")).Return("c001");
            SubmitDurRequest submitDurRequest = new SubmitDurRequest();
            submitDurRequest.IsCapturedReasons = true;
            submitDurRequest.ProviderId = mockedObjects.GetGuidInString();
            var res = durMultiSelect.SaveDurForm(submitDurRequest, impactDur);
            Assert.IsNotNull(res);

        }

        [TestMethod]
        public void submitDur_for_refill_for_delegateProvider_is_set_to_true_Test()
        {
            HttpContext.Current = MockedObjects.MockHttpContext();
            var impactDur = MockRepository.GenerateStub<IImpactDurWraper>();
            _pageState = new StateContainer(HttpContext.Current.Session);
            _pageState[SessionVariables.LicenseId] = mockedObjects.GetGuid();
            _pageState[SessionVariables.UserId] = mockedObjects.GetGuid();
            _pageState[SessionVariables.DbId] = mockedObjects.SetUpDBId();
            _pageState[SessionVariables.PatientId] = "Patient1";
            _pageState[SessionVariables.TaskScriptMessageId] = mockedObjects.GetGuid();
            _pageState[SessionVariables.SiteId] = 1;
            _pageState["TaskScriptMessageId"] = mockedObjects.GetGuid();
            List<Rx> rxList = mockedObjects.SetScriptsPadCsMeds();
            _pageState[SessionVariables.RxList] = mockedObjects.SetRxList();
            _pageState[SessionVariables.CurrentScriptPadMeds] = rxList;
            _pageState[SessionVariables.UserType] = UserCategory.POB_LIMITED;
            string token = mockedObjects.GetShieldToken();
            RxTaskModel rxTask = new RxTaskModel();
            _pageState[SessionVariables.RxTask] = rxTask;
            _pageState[SessionVariables.DelegateProviderId] = mockedObjects.GetGuidInString();
            _pageState["SessionLicense"] = new ApplicationLicense();
            DurMultiSelect durMultiSelect = new DurMultiSelect();
            durMultiSelect.SetPageState(_pageState);
            impactDur.Stub(x => x.GetStateControlledSubstanceCode("123", "CA", "CA", mockedObjects.SetUpDBId())).Return("CA");
            impactDur.Stub(x => x.ReconcileControlledSubstanceCodes("cs01", "CA")).Return("c001");
            SubmitDurRequest submitDurRequest = new SubmitDurRequest();
            submitDurRequest.IsCapturedReasons = true;
            submitDurRequest.ProviderId = mockedObjects.GetGuidInString();
            var res = durMultiSelect.SaveDurForm(submitDurRequest, impactDur);
            Assert.IsNotNull(res);

        }

        [TestMethod]
        public void submitDur_for_refill_reconsonsiled_CsMeds_And_IsCapturedReasons_is_set_to_true_Test()
        {
            HttpContext.Current = MockedObjects.MockHttpContext();
            var impactDur = MockRepository.GenerateStub<IImpactDurWraper>();
            _pageState = new StateContainer(HttpContext.Current.Session);
            _pageState[SessionVariables.LicenseId] = mockedObjects.GetGuid();
            _pageState[SessionVariables.UserId] = mockedObjects.GetGuid();
            _pageState[SessionVariables.DbId] = mockedObjects.SetUpDBId();
            _pageState[SessionVariables.PatientId] = "Patient1";
            _pageState[SessionVariables.TaskScriptMessageId] = mockedObjects.GetGuid();
            _pageState[SessionVariables.SiteId] = 1;
            _pageState["TaskScriptMessageId"] = mockedObjects.GetGuid();
            List<Rx> rxList = mockedObjects.SetScriptsPadCsMeds();
            _pageState[SessionVariables.RxList] = mockedObjects.SetRxList();
            _pageState[SessionVariables.CurrentScriptPadMeds] = rxList;
            _pageState[SessionVariables.UserType] = UserCategory.POB_LIMITED;
            string token = mockedObjects.GetShieldToken();
            RxTaskModel rxTask = new RxTaskModel();
            _pageState[SessionVariables.RxTask] = rxTask;
            rxTask.TaskType = PrescriptionTaskType.RXCHG;
            _pageState["SessionLicense"] = new ApplicationLicense();
            DurMultiSelect durMultiSelect = new DurMultiSelect();
            durMultiSelect.SetPageState(_pageState);
            impactDur.Stub(x => x.GetStateControlledSubstanceCode("123", "CA", "CA", mockedObjects.SetUpDBId())).Return("CA");
            impactDur.Stub(x => x.ReconcileControlledSubstanceCodes("cs01", "CA")).Return("c001");
            SubmitDurRequest submitDurRequest = new SubmitDurRequest();
            submitDurRequest.IsCapturedReasons = true;
            submitDurRequest.ProviderId = mockedObjects.GetGuidInString();
            var res = durMultiSelect.SaveDurForm(submitDurRequest, impactDur);
            Assert.IsNotNull(res);

        }

        [TestMethod]
        public void submitDur_for_reconsonsiled_CsMeds_And_IsCapturedReasons_is_set_to_true_Test()
        {
            HttpContext.Current = MockedObjects.MockHttpContext();
            var impactDur = MockRepository.GenerateStub<IImpactDurWraper>();
            _pageState = new StateContainer(HttpContext.Current.Session);
            _pageState[SessionVariables.LicenseId] = mockedObjects.GetGuid();
            _pageState[SessionVariables.UserId] = mockedObjects.GetGuid();
            _pageState[SessionVariables.DbId] = mockedObjects.SetUpDBId();
            _pageState[SessionVariables.PatientId] = "Patient1";
            _pageState[SessionVariables.TaskScriptMessageId] = mockedObjects.GetGuid();
            _pageState[SessionVariables.SiteId] = 1;
            _pageState["TaskScriptMessageId"] = mockedObjects.GetGuid();
            List<Rx> rxList = mockedObjects.SetScriptsPadCsMeds();
            _pageState[SessionVariables.RxList] = mockedObjects.SetRxList() ;
            _pageState[SessionVariables.CurrentScriptPadMeds] = rxList;
            DurMultiSelect durMultiSelect = new DurMultiSelect();
            durMultiSelect.SetPageState(_pageState);
            impactDur.Stub(x => x.GetStateControlledSubstanceCode("123", "CA", "CA", mockedObjects.SetUpDBId())).Return("CA");
            impactDur.Stub(x => x.ReconcileControlledSubstanceCodes("cs01", "CA")).Return("c001");
            SubmitDurRequest submitDurRequest = new SubmitDurRequest();
            submitDurRequest.IsCapturedReasons = true;
            submitDurRequest.ProviderId = mockedObjects.GetGuidInString();
            var res = durMultiSelect.SaveDurForm(submitDurRequest, impactDur);
            Assert.IsNotNull(res);

        }

        [TestMethod]
        public void submitDur_for_rec_DDI_IsNull_CsMeds_And_IsCapturedReasons_is_set_to_true_Test()
        {
            HttpContext.Current = MockedObjects.MockHttpContext();
            var impactDur = MockRepository.GenerateStub<IImpactDurWraper>();
            _pageState = new StateContainer(HttpContext.Current.Session);
            _pageState[SessionVariables.LicenseId] = mockedObjects.GetGuid();
            _pageState[SessionVariables.UserId] = mockedObjects.GetGuid();
            _pageState[SessionVariables.DbId] = mockedObjects.SetUpDBId();
            _pageState[SessionVariables.PatientId] = "Patient1";
            _pageState[SessionVariables.TaskScriptMessageId] = mockedObjects.GetGuid();
            _pageState[SessionVariables.SiteId] = 1;
            _pageState[SessionVariables.IsLicenseShieldEnabled] = true;

            string token = mockedObjects.GetShieldToken();
            _pageState["SessionLicense"] =new ApplicationLicense();

            eRxWeb.ePrescribeSvc.Permission permission = new eRxWeb.ePrescribeSvc.Permission();
            eRxWeb.ePrescribeSvc.Permission[] permissions = { permission };

            _pageState["TaskScriptMessageId"] = mockedObjects.GetGuid();
            List<Rx> rxList = mockedObjects.SetScriptsPadCsMeds();
            _pageState[SessionVariables.RxList] = mockedObjects.SetRxListWithoutDDI();
            _pageState[SessionVariables.CurrentScriptPadMeds] = rxList;
            DurMultiSelect durMultiSelect = new DurMultiSelect();
            durMultiSelect.SetPageState(_pageState);
            impactDur.Stub(x => x.GetStateControlledSubstanceCode("123", "CA", "CA", mockedObjects.SetUpDBId())).Return("CA");
            impactDur.Stub(x => x.ReconcileControlledSubstanceCodes("cs01", "CA")).Return("c001");
            SubmitDurRequest submitDurRequest = new SubmitDurRequest();
            submitDurRequest.IsCapturedReasons = true;
            submitDurRequest.ProviderId = mockedObjects.GetGuidInString();
            var res = durMultiSelect.SaveDurForm(submitDurRequest, impactDur);
            Assert.IsNotNull(res);
        }


        [TestMethod]
        public void submitDur_for_reconsileCode_is_two_And_IsCapturedReasons_is_set_to_true_Test()
        {
            HttpContext.Current = MockedObjects.MockHttpContext();
            var impactDur = MockRepository.GenerateStub<IImpactDurWraper>();
            _pageState = new StateContainer(HttpContext.Current.Session);
            _pageState[SessionVariables.LicenseId] = mockedObjects.GetGuid();
            _pageState[SessionVariables.UserId] = mockedObjects.GetGuid();
            _pageState[SessionVariables.DbId] = mockedObjects.SetUpDBId();
            _pageState[SessionVariables.PatientId] = "Patient1";
            _pageState[SessionVariables.TaskScriptMessageId] = mockedObjects.GetGuid();
            _pageState[SessionVariables.SiteId] = 1;
            _pageState[SessionVariables.IsLicenseShieldEnabled] = true;

            string token = mockedObjects.GetShieldToken();
            ApplicationLicense licience = new ApplicationLicense();
            licience.LicenseDeluxeStatus = DeluxeFeatureStatus.On;
            licience.DeluxePricingStructure = DeluxePricingStructure.DeluxeEpcsEpaLogRx2017;
            licience.EnterpriseDeluxeFeatureStatus = DeluxeFeatureStatus.AlwaysOn;
            _pageState["SessionLicense"] = licience;
            eRxWeb.ePrescribeSvc.Permission permission = new eRxWeb.ePrescribeSvc.Permission();
            permission.ClaimType = UserPermissionNames.EPCS_CAN_APPROVE;
            permission.Value = UserPermissionNames.EPCS_CAN_APPROVE;
            eRxWeb.ePrescribeSvc.Permission perPrescribe = new eRxWeb.ePrescribeSvc.Permission();
            perPrescribe.ClaimType = perPrescribe.Value = UserPermissionNames.EPCS_CAN_PRESCRIBE;
            eRxWeb.ePrescribeSvc.Permission[] permissions = { permission, perPrescribe };
            _pageState[SessionVariables.UserAppPermissions] = permissions;
            List<Rx> rxList = mockedObjects.SetScriptsPadCsMeds();
            _pageState[SessionVariables.RxList] = mockedObjects.SetRxListWithScheduleUsedTwo();
            _pageState[SessionVariables.CurrentScriptPadMeds] = rxList;
            DurMultiSelect durMultiSelect = new DurMultiSelect();
            durMultiSelect.SetPageState(_pageState);
            impactDur.Stub(x => x.GetStateControlledSubstanceCode("123", "CA", "CA", mockedObjects.SetUpDBId())).Return("CA");
            impactDur.Stub(x => x.ReconcileControlledSubstanceCodes("cs01", "CA")).Return("c001");
            SubmitDurRequest submitDurRequest = new SubmitDurRequest();
            submitDurRequest.IsCapturedReasons = true;
            submitDurRequest.ProviderId = mockedObjects.GetGuidInString();
            submitDurRequest.starsAlign = true;
            var res = durMultiSelect.SaveDurForm(submitDurRequest, impactDur);
            Assert.IsNotNull(res);
        }

        [TestMethod]
        public void submitDur_when_med_prescribed_And_capturedReasons_is_set_to_true__Test()
        {
            UserPermissions userPermissions = new UserPermissions();
            HttpContext.Current = MockedObjects.MockHttpContext();
            var impactDur = MockRepository.GenerateStub<IImpactDurWraper>();
            _pageState = new StateContainer(HttpContext.Current.Session);
            _pageState[SessionVariables.LicenseId] = mockedObjects.GetGuid();
            _pageState[SessionVariables.UserId] = mockedObjects.GetGuid();
            _pageState[SessionVariables.DbId] = mockedObjects.SetUpDBId();
            _pageState[SessionVariables.PatientId] = "Patient1";
            _pageState[SessionVariables.SiteId] = 1;
            _pageState[SessionVariables.PracticeState] = "ca";
            _pageState[SessionVariables.IsLicenseShieldEnabled] = true;
            _pageState[SessionVariables.IsEnterpriseEpcsEnabled] = true;
            ArrayList list = mockedObjects.SetRxListIsFreeFormMedControlSubstance();
            _pageState[SessionVariables.RxList] = list;
            string token = mockedObjects.GetShieldToken();
            ApplicationLicense licience = new ApplicationLicense();
            licience.LicenseDeluxeStatus = DeluxeFeatureStatus.On;
            licience.DeluxePricingStructure = DeluxePricingStructure.DeluxeEpcsEpaLogRx2017;
            licience.EnterpriseDeluxeFeatureStatus = DeluxeFeatureStatus.AlwaysOn;
            _pageState["SessionLicense"] = licience;
            eRxWeb.ePrescribeSvc.Permission permission = new eRxWeb.ePrescribeSvc.Permission();
            permission.ClaimType = Constants.UserPermissionNames.EPCS_CAN_APPROVE;
            permission.Value = Constants.UserPermissionNames.EPCS_CAN_APPROVE;
            eRxWeb.ePrescribeSvc.Permission perPrescribe = new eRxWeb.ePrescribeSvc.Permission();
            perPrescribe.ClaimType = perPrescribe.Value = UserPermissionNames.EPCS_CAN_PRESCRIBE;
            eRxWeb.ePrescribeSvc.Permission[] permissions = { permission, perPrescribe };
            _pageState[SessionVariables.UserAppPermissions] = permissions;
            List<Rx> rxList = mockedObjects.SetScriptsPadCsMedsFreeFromCsMed();
            _pageState[SessionVariables.RxList] = mockedObjects.SetRxListWithoutDDI();
            _pageState[SessionVariables.RxTask] = new RxTaskModel();
            _pageState[SessionVariables.CurrentScriptPadMeds] = rxList;
            DurMultiSelect durMultiSelect = new DurMultiSelect();
            durMultiSelect.SetPageState(_pageState);
            SubmitDurRequest submitDurRequest = new SubmitDurRequest();
            submitDurRequest.IsCapturedReasons = true;
            submitDurRequest.ProviderId = mockedObjects.GetGuidInString();
            var res = durMultiSelect.SaveDurForm(submitDurRequest, impactDur);
            Assert.IsNotNull(res);
        }

        [TestMethod]
        public void submitDur_for_EpcsIsTrue_DDI_IsNull_CsMeds_And_IsCapturedReasons_is_set_to_true__Test()
        {
            UserPermissions userPermissions = new UserPermissions();
            HttpContext.Current = MockedObjects.MockHttpContext();
            var impactDur = MockRepository.GenerateStub<IImpactDurWraper>();
            _pageState = new StateContainer(HttpContext.Current.Session);
            _pageState[SessionVariables.LicenseId] = mockedObjects.GetGuid();
            _pageState[SessionVariables.UserId] = mockedObjects.GetGuid();
            _pageState[SessionVariables.DbId] = mockedObjects.SetUpDBId();
            _pageState[SessionVariables.PatientId] = "Patient1";
            _pageState[SessionVariables.TaskScriptMessageId] = mockedObjects.GetGuid();
            _pageState[SessionVariables.SiteId] = 1;
            _pageState[SessionVariables.PracticeState] = "ca";
            _pageState[SessionVariables.IsLicenseShieldEnabled] = true;
            _pageState[SessionVariables.IsEnterpriseEpcsEnabled] = true;
            string token = mockedObjects.GetShieldToken();
            ApplicationLicense licience = new ApplicationLicense();
            licience.LicenseDeluxeStatus = DeluxeFeatureStatus.On;
            licience.DeluxePricingStructure = DeluxePricingStructure.DeluxeEpcsEpaLogRx2017;
            licience.EnterpriseDeluxeFeatureStatus = DeluxeFeatureStatus.AlwaysOn;
            _pageState["SessionLicense"] = licience;
            eRxWeb.ePrescribeSvc.Permission permission = new eRxWeb.ePrescribeSvc.Permission();
            permission.ClaimType = Constants.UserPermissionNames.EPCS_CAN_APPROVE;
            permission.Value = Constants.UserPermissionNames.EPCS_CAN_APPROVE;
            eRxWeb.ePrescribeSvc.Permission perPrescribe = new eRxWeb.ePrescribeSvc.Permission();
            perPrescribe.ClaimType = perPrescribe.Value = UserPermissionNames.EPCS_CAN_PRESCRIBE;
            eRxWeb.ePrescribeSvc.Permission[] permissions = { permission, perPrescribe };
            _pageState[SessionVariables.UserAppPermissions] = permissions;
            List<Rx> rxList = mockedObjects.SetScriptsPadCsMedsFreeFromCsMed();
            _pageState[SessionVariables.RxList] = mockedObjects.SetRxListWithoutDDI();
            _pageState[SessionVariables.CurrentScriptPadMeds] = rxList;
            DurMultiSelect durMultiSelect = new DurMultiSelect();
            durMultiSelect.SetPageState(_pageState);
            SubmitDurRequest submitDurRequest = new SubmitDurRequest();
            submitDurRequest.IsCapturedReasons = true;
            submitDurRequest.ProviderId = mockedObjects.GetGuidInString();
            var res = durMultiSelect.SaveDurForm(submitDurRequest, impactDur);
            Assert.IsNotNull(res);
        }

        [TestMethod]
        public void submitDur_for_rec_EpcsIsTrue_DDI_IsNull_CsMeds_And_IsCapturedReasons_is_set_to_true__Test()
        {
            UserPermissions userPermissions = new UserPermissions();
            HttpContext.Current = MockedObjects.MockHttpContext();
            var impactDur = MockRepository.GenerateStub<IImpactDurWraper>();
            _pageState = new StateContainer(HttpContext.Current.Session);
            _pageState[SessionVariables.LicenseId] = mockedObjects.GetGuid();
            _pageState[SessionVariables.UserId] = mockedObjects.GetGuid();
            _pageState[SessionVariables.DbId] = mockedObjects.SetUpDBId();
            _pageState[SessionVariables.PatientId] = "Patient1";
            _pageState[SessionVariables.TaskScriptMessageId] = mockedObjects.GetGuid();
            _pageState[SessionVariables.SiteId] = 1;
            _pageState[SessionVariables.PracticeState] = "ca";
            _pageState[SessionVariables.IsLicenseShieldEnabled] = true;
            _pageState[SessionVariables.IsEnterpriseEpcsEnabled] = true;
            string token = mockedObjects.GetShieldToken();
            ApplicationLicense licience = new ApplicationLicense();
            licience.LicenseDeluxeStatus = DeluxeFeatureStatus.On;
            licience.DeluxePricingStructure = DeluxePricingStructure.DeluxeEpcsEpaLogRx2017;
            licience.EnterpriseDeluxeFeatureStatus = DeluxeFeatureStatus.AlwaysOn;
            _pageState["SessionLicense"] = licience;
            eRxWeb.ePrescribeSvc.Permission permission = new eRxWeb.ePrescribeSvc.Permission();
            permission.ClaimType = Constants.UserPermissionNames.EPCS_CAN_APPROVE;
            permission.Value = Constants.UserPermissionNames.EPCS_CAN_APPROVE;
            eRxWeb.ePrescribeSvc.Permission perPrescribe = new eRxWeb.ePrescribeSvc.Permission();
            perPrescribe.ClaimType = perPrescribe.Value = UserPermissionNames.EPCS_CAN_PRESCRIBE;
            eRxWeb.ePrescribeSvc.Permission[] permissions = { permission, perPrescribe };
            _pageState[SessionVariables.UserAppPermissions] = permissions;
            List<Rx> rxList = mockedObjects.SetScriptsPadCsMeds();
            _pageState[SessionVariables.RxList] = mockedObjects.SetRxListWithoutDDI();
            _pageState[SessionVariables.CurrentScriptPadMeds] = rxList;
            DurMultiSelect durMultiSelect = new DurMultiSelect();
            durMultiSelect.SetPageState(_pageState);
            SubmitDurRequest submitDurRequest = new SubmitDurRequest();
            submitDurRequest.IsCapturedReasons = true;
            submitDurRequest.ProviderId = mockedObjects.GetGuidInString();
            var res = durMultiSelect.SaveDurForm(submitDurRequest, impactDur);
            Assert.IsNotNull(res);
        }
        [TestMethod]
        public void submitDur_for_refill_rec_EpcsIsTrue_DDI_IsNull_CsMeds_And_IsCapturedReasons_is_set_to_true_Test()
        {
            UserPermissions userPermissions = new UserPermissions();
            HttpContext.Current = MockedObjects.MockHttpContext();
            var impactDur = MockRepository.GenerateStub<IImpactDurWraper>();
            _pageState = new StateContainer(HttpContext.Current.Session);
            _pageState[SessionVariables.LicenseId] = mockedObjects.GetGuid();
            _pageState[SessionVariables.UserId] = mockedObjects.GetGuid();
            _pageState[SessionVariables.DbId] = mockedObjects.SetUpDBId();
            _pageState[SessionVariables.PatientId] = "Patient1";
            _pageState[SessionVariables.TaskScriptMessageId] = mockedObjects.GetGuid();
            _pageState[SessionVariables.SiteId] = 1;
            _pageState[SessionVariables.PracticeState] = "ca";
            _pageState[SessionVariables.IsLicenseShieldEnabled] = true;
            _pageState[SessionVariables.IsEnterpriseEpcsEnabled] = true;
            string token = mockedObjects.GetShieldToken();
            ApplicationLicense licience = new ApplicationLicense();
            licience.LicenseDeluxeStatus = DeluxeFeatureStatus.On;
            licience.DeluxePricingStructure = DeluxePricingStructure.DeluxeEpcsEpaLogRx2017;
            licience.EnterpriseDeluxeFeatureStatus = DeluxeFeatureStatus.AlwaysOn;
            _pageState["SessionLicense"] = licience;
            eRxWeb.ePrescribeSvc.Permission permission = new eRxWeb.ePrescribeSvc.Permission();
            permission.ClaimType = Constants.UserPermissionNames.EPCS_CAN_APPROVE;
            permission.Value = Constants.UserPermissionNames.EPCS_CAN_APPROVE;
            eRxWeb.ePrescribeSvc.Permission perPrescribe = new eRxWeb.ePrescribeSvc.Permission();
            perPrescribe.ClaimType = perPrescribe.Value= UserPermissionNames.EPCS_CAN_PRESCRIBE;
            eRxWeb.ePrescribeSvc.Permission[] permissions = { permission,perPrescribe };
            _pageState[SessionVariables.UserAppPermissions] = permissions;
            _pageState["TaskScriptMessageId"] = mockedObjects.GetGuid();
            List<Rx> rxList = mockedObjects.SetScriptsPadCsMeds();
            _pageState[SessionVariables.RxList] = mockedObjects.SetRxListWithoutDDI();
            _pageState[SessionVariables.CurrentScriptPadMeds] = rxList;
            DurMultiSelect durMultiSelect = new DurMultiSelect();
            durMultiSelect.SetPageState(_pageState);
            SubmitDurRequest submitDurRequest = new SubmitDurRequest();
            submitDurRequest.IsCapturedReasons = true;
            submitDurRequest.ProviderId = mockedObjects.GetGuidInString();
            var res = durMultiSelect.SaveDurForm(submitDurRequest, impactDur);
            Assert.IsNotNull(res);
        }

        [TestMethod]
        public void submitDur_When_currentMed_IsFreeFormMedControlSubstance_DDI_IsNull_CsMeds_And_IsCapturedReasons_is_set_to_true_Test()
        {
            UserPermissions userPermissions = new UserPermissions();
            HttpContext.Current = MockedObjects.MockHttpContext();
            var impactDur = MockRepository.GenerateStub<IImpactDurWraper>();
            _pageState = new StateContainer(HttpContext.Current.Session);
            _pageState[SessionVariables.LicenseId] = mockedObjects.GetGuid();
            _pageState[SessionVariables.UserId] = mockedObjects.GetGuid();
            _pageState[SessionVariables.DbId] = mockedObjects.SetUpDBId();
            _pageState[SessionVariables.PatientId] = "Patient1";
            _pageState[SessionVariables.TaskScriptMessageId] = mockedObjects.GetGuid();
            _pageState[SessionVariables.SiteId] = 1;
            _pageState[SessionVariables.PracticeState] = "ca";
            _pageState[SessionVariables.IsLicenseShieldEnabled] = true;
            _pageState[SessionVariables.IsEnterpriseEpcsEnabled] = true;
            
            string token = mockedObjects.GetShieldToken();
            ApplicationLicense licience = new ApplicationLicense();
            licience.LicenseDeluxeStatus = DeluxeFeatureStatus.On;
            licience.DeluxePricingStructure = DeluxePricingStructure.DeluxeEpcsEpaLogRx2017;
            licience.EnterpriseDeluxeFeatureStatus = DeluxeFeatureStatus.AlwaysOn;
            _pageState["SessionLicense"] = licience;
            eRxWeb.ePrescribeSvc.Permission permission = new eRxWeb.ePrescribeSvc.Permission();
            permission.ClaimType = Constants.UserPermissionNames.EPCS_CAN_APPROVE;
            permission.Value = Constants.UserPermissionNames.EPCS_CAN_APPROVE;
            eRxWeb.ePrescribeSvc.Permission perPrescribe = new eRxWeb.ePrescribeSvc.Permission();
            perPrescribe.ClaimType = perPrescribe.Value = UserPermissionNames.EPCS_CAN_PRESCRIBE;
            eRxWeb.ePrescribeSvc.Permission[] permissions = { permission, perPrescribe };
            _pageState[SessionVariables.UserAppPermissions] = permissions;
            _pageState["TaskScriptMessageId"] = mockedObjects.GetGuid();
            List<Rx> rxList = mockedObjects.SetScriptsPadCsMeds();
            ArrayList list = mockedObjects.SetRxListIsFreeFormMedControlSubstance();
            _pageState[SessionVariables.RxList] = list;
            _pageState[SessionVariables.CurrentScriptPadMeds] = rxList;
            List<string> ls = new List<string>();
            ls.Add("1");
            _pageState[SessionVariables.SiteEPCSAuthorizedSchedules] = ls;
            DurMultiSelect durMultiSelect = new DurMultiSelect();
            durMultiSelect.SetPageState(_pageState);
            SubmitDurRequest submitDurRequest = new SubmitDurRequest();
            submitDurRequest.IsCapturedReasons = true;
            submitDurRequest.starsAlign = true;
            submitDurRequest.ProviderId = mockedObjects.GetGuidInString();
            var res = durMultiSelect.SaveDurForm(submitDurRequest, impactDur);
            Assert.IsNotNull(res);
        }
        [TestMethod]
        public void submitDur_CsMeds_With_csCode_Zero_And_IsCapturedReasons_is_set_to_true_Test()
        {
            UserPermissions userPermissions = new UserPermissions();
            HttpContext.Current = MockedObjects.MockHttpContext();
            var impactDur = MockRepository.GenerateStub<IImpactDurWraper>();
            _pageState = new StateContainer(HttpContext.Current.Session);
            _pageState[SessionVariables.LicenseId] = mockedObjects.GetGuid();
            _pageState[SessionVariables.UserId] = mockedObjects.GetGuid();
            _pageState[SessionVariables.DbId] = mockedObjects.SetUpDBId();
            _pageState[SessionVariables.PatientId] = "Patient1";
            _pageState[SessionVariables.TaskScriptMessageId] = mockedObjects.GetGuid();
            _pageState[SessionVariables.SiteId] = 1;
            _pageState[SessionVariables.PracticeState] = "ca";
            _pageState[SessionVariables.IsLicenseShieldEnabled] = true;
            _pageState[SessionVariables.IsEnterpriseEpcsEnabled] = true;
            RxTaskModel rxTask = new RxTaskModel();
            rxTask.TaskType = PrescriptionTaskType.RXCHG;
            _pageState[SessionVariables.RxTask] = rxTask;
            _pageState[SessionVariables.TaskType] = rxTask.TaskType;
            string token = mockedObjects.GetShieldToken();
            ApplicationLicense licience = new ApplicationLicense();
            licience.LicenseDeluxeStatus = DeluxeFeatureStatus.On;
            licience.DeluxePricingStructure = DeluxePricingStructure.DeluxeEpcsEpaLogRx2017;
            licience.EnterpriseDeluxeFeatureStatus = DeluxeFeatureStatus.AlwaysOn;
            _pageState["SessionLicense"] = licience;
            eRxWeb.ePrescribeSvc.Permission permission = new eRxWeb.ePrescribeSvc.Permission();
            permission.ClaimType = Constants.UserPermissionNames.EPCS_CAN_APPROVE;
            permission.Value = Constants.UserPermissionNames.EPCS_CAN_APPROVE;
            eRxWeb.ePrescribeSvc.Permission perPrescribe = new eRxWeb.ePrescribeSvc.Permission();
            perPrescribe.ClaimType = perPrescribe.Value = UserPermissionNames.EPCS_CAN_PRESCRIBE;
            eRxWeb.ePrescribeSvc.Permission[] permissions = { permission, perPrescribe };
            _pageState[SessionVariables.UserAppPermissions] = permissions;
            _pageState["TaskScriptMessageId"] = mockedObjects.GetGuid();
            List<Rx> rxList = mockedObjects.SetScriptsPadCsMeds();
            ArrayList list = mockedObjects.SetRxListWithScheduledAsZero();
            _pageState[SessionVariables.RxList] = list;
            _pageState[SessionVariables.CurrentScriptPadMeds] = rxList;
            List<string> ls = new List<string>();
            ls.Add("1");
            _pageState[SessionVariables.SiteEPCSAuthorizedSchedules] = ls;
            _pageState[SessionVariables.ChangeRxRequestedMedCs] = true;
            DurMultiSelect durMultiSelect = new DurMultiSelect();
            durMultiSelect.SetPageState(_pageState);
            SubmitDurRequest submitDurRequest = new SubmitDurRequest();
            submitDurRequest.IsCapturedReasons = true;
            submitDurRequest.starsAlign = true;
            submitDurRequest.ProviderId = mockedObjects.GetGuidInString();
            var res = durMultiSelect.SaveDurForm(submitDurRequest, impactDur);
            Assert.IsNotNull(res);
        }
        [TestMethod]
        public void submitDur_for_refill_by_provider_Test()
        {
            UserPermissions userPermissions = new UserPermissions();
            HttpContext.Current = MockedObjects.MockHttpContext();
            ApplicationLicense licience = new ApplicationLicense();
            RxTaskModel rxTask = new RxTaskModel();
            List<Rx> rxList = mockedObjects.SetScriptsPadCsMeds();
            ArrayList list = mockedObjects.SetRxListWithScheduledAsZero();
            var impactDur = MockRepository.GenerateStub<IImpactDurWraper>();
            _pageState = new StateContainer(HttpContext.Current.Session);
            _pageState[SessionVariables.LicenseId] = mockedObjects.GetGuid();
            _pageState[SessionVariables.UserId] = mockedObjects.GetGuid();
            _pageState[SessionVariables.DbId] = mockedObjects.SetUpDBId();
            _pageState[SessionVariables.PatientId] = "Patient1";
            _pageState[SessionVariables.TaskScriptMessageId] = mockedObjects.GetGuid();
            _pageState[SessionVariables.SiteId] = 1;
            _pageState[SessionVariables.PracticeState] = "ca";
            _pageState[SessionVariables.IsLicenseShieldEnabled] = true;
            _pageState[SessionVariables.IsEnterpriseEpcsEnabled] = true;
            rxTask.TaskType = PrescriptionTaskType.RXCHG;
            _pageState[SessionVariables.RxTask] = rxTask;
            _pageState[SessionVariables.TaskType] = rxTask.TaskType;
            string token = mockedObjects.GetShieldToken();
            
            licience.LicenseDeluxeStatus = DeluxeFeatureStatus.On;
            licience.DeluxePricingStructure = DeluxePricingStructure.DeluxeEpcsEpaLogRx2017;
            licience.EnterpriseDeluxeFeatureStatus = DeluxeFeatureStatus.AlwaysOn;
            _pageState["SessionLicense"] = licience;
            eRxWeb.ePrescribeSvc.Permission permission = new eRxWeb.ePrescribeSvc.Permission();
            permission.ClaimType = Constants.UserPermissionNames.EPCS_CAN_APPROVE;
            permission.Value = Constants.UserPermissionNames.EPCS_CAN_APPROVE;
            eRxWeb.ePrescribeSvc.Permission perPrescribe = new eRxWeb.ePrescribeSvc.Permission();
            perPrescribe.ClaimType = perPrescribe.Value = UserPermissionNames.EPCS_CAN_PRESCRIBE;
            eRxWeb.ePrescribeSvc.Permission[] permissions = { permission, perPrescribe };
            _pageState[SessionVariables.UserAppPermissions] = permissions;
            _pageState[SessionVariables.UserType] = UserCategory.PROVIDER;
            _pageState["TaskScriptMessageId"] = mockedObjects.GetGuid();
            
            _pageState[SessionVariables.RxList] = list;
            _pageState[SessionVariables.CurrentScriptPadMeds] = rxList;
            List<string> ls = new List<string>();
            ls.Add("1");
            _pageState[SessionVariables.SiteEPCSAuthorizedSchedules] = ls;
            _pageState[SessionVariables.ChangeRxRequestedMedCs] = true;
            _pageState[Constants.SessionVariables.CameFrom] = "AnyPage";
            DurMultiSelect durMultiSelect = new DurMultiSelect();
            durMultiSelect.SetPageState(_pageState);
            SubmitDurRequest submitDurRequest = new SubmitDurRequest();
            submitDurRequest.IsCapturedReasons = true;
            submitDurRequest.starsAlign = true;
            submitDurRequest.ProviderId = mockedObjects.GetGuidInString();
            var res = durMultiSelect.SaveDurForm(submitDurRequest, impactDur);
            Assert.IsNotNull(res);
        }

        [TestMethod]
        public void submitDur_for_NoSupervisionUsers_when_RxTaskType_is_not_RXCHG_Test()
        {
            UserPermissions userPermissions = new UserPermissions();
            HttpContext.Current = MockedObjects.MockHttpContext();
            var impactDur = MockRepository.GenerateStub<IImpactDurWraper>();
            _pageState = new StateContainer(HttpContext.Current.Session);
            _pageState[SessionVariables.LicenseId] = mockedObjects.GetGuid();
            _pageState[SessionVariables.UserId] = mockedObjects.GetGuid();
            _pageState[SessionVariables.DbId] = mockedObjects.SetUpDBId();
            _pageState[SessionVariables.PatientId] = "Patient1";
            _pageState[SessionVariables.TaskScriptMessageId] = mockedObjects.GetGuid();
            _pageState[SessionVariables.SiteId] = 1;
            _pageState[SessionVariables.PracticeState] = "ca";
            _pageState[SessionVariables.IsLicenseShieldEnabled] = true;
            _pageState[SessionVariables.IsEnterpriseEpcsEnabled] = true;
            RxTaskModel rxTask = new RxTaskModel();
            _pageState[SessionVariables.RxTask] = rxTask;
            string token = mockedObjects.GetShieldToken();
          
            ApplicationLicense licience = new ApplicationLicense();
            licience.LicenseDeluxeStatus = DeluxeFeatureStatus.On;
            licience.DeluxePricingStructure = DeluxePricingStructure.DeluxeEpcsEpaLogRx2017;
            licience.EnterpriseDeluxeFeatureStatus = DeluxeFeatureStatus.AlwaysOn;
            _pageState["SessionLicense"] = licience;
            eRxWeb.ePrescribeSvc.Permission permission = new eRxWeb.ePrescribeSvc.Permission();
            permission.ClaimType = Constants.UserPermissionNames.EPCS_CAN_APPROVE;
            permission.Value = Constants.UserPermissionNames.EPCS_CAN_APPROVE;
            eRxWeb.ePrescribeSvc.Permission perPrescribe = new eRxWeb.ePrescribeSvc.Permission();
            perPrescribe.ClaimType = perPrescribe.Value = UserPermissionNames.EPCS_CAN_PRESCRIBE;
            eRxWeb.ePrescribeSvc.Permission[] permissions = { permission, perPrescribe };
            _pageState[SessionVariables.UserAppPermissions] = permissions;
            _pageState["TaskScriptMessageId"] = mockedObjects.GetGuid();
            List<Rx> rxList = mockedObjects.SetScriptsPadCsMeds();
            ArrayList list = mockedObjects.SetRxListWithScheduledAsZero();
            _pageState[SessionVariables.RxList] = list;
            _pageState[SessionVariables.CurrentScriptPadMeds] = rxList;
            List<string> ls = new List<string>();
            ls.Add("1");
            _pageState[SessionVariables.SiteEPCSAuthorizedSchedules] = ls;
            _pageState[SessionVariables.ChangeRxRequestedMedCs] = true;
            ChangeRxRequestedMedCs changeRxRequestedMedCs = new ChangeRxRequestedMedCs();
            changeRxRequestedMedCs.ScriptMessageGuid = mockedObjects.GetGuidInString();
            _pageState[Constants.SessionVariables.ChangeRxRequestedMedCs] = changeRxRequestedMedCs;
            DurMultiSelect durMultiSelect = new DurMultiSelect();
            durMultiSelect.SetPageState(_pageState);
            SubmitDurRequest submitDurRequest = new SubmitDurRequest();
            submitDurRequest.IsCapturedReasons = true;
            submitDurRequest.starsAlign = true;
            submitDurRequest.ProviderId = mockedObjects.GetGuidInString();
            var res = durMultiSelect.SaveDurForm(submitDurRequest, impactDur);
            Assert.IsNotNull(res);
        }
        [TestMethod]
        public void submitDur_When_currentMed_IsFreeFormMedControlSubstance_And_IsCsRefReqWorkflow()
        {
            UserPermissions userPermissions = new UserPermissions();
            HttpContext.Current = MockedObjects.MockHttpContext();
            var impactDur = MockRepository.GenerateStub<IImpactDurWraper>();
            _pageState = new StateContainer(HttpContext.Current.Session);
            _pageState[SessionVariables.LicenseId] = mockedObjects.GetGuid();
            _pageState[SessionVariables.UserId] = mockedObjects.GetGuid();
            _pageState[SessionVariables.DbId] = mockedObjects.SetUpDBId();
            _pageState[SessionVariables.PatientId] = "Patient1";
            _pageState[SessionVariables.TaskScriptMessageId] = mockedObjects.GetGuid();
            _pageState[SessionVariables.SiteId] = 1;
            _pageState[SessionVariables.PracticeState] = "ca";
            _pageState[SessionVariables.IsLicenseShieldEnabled] = true;
            _pageState[SessionVariables.IsEnterpriseEpcsEnabled] = true;

            string token = mockedObjects.GetShieldToken();
            ApplicationLicense licience = new ApplicationLicense();
            licience.LicenseDeluxeStatus = DeluxeFeatureStatus.On;
            licience.DeluxePricingStructure = DeluxePricingStructure.DeluxeEpcsEpaLogRx2017;
            licience.EnterpriseDeluxeFeatureStatus = DeluxeFeatureStatus.AlwaysOn;
            _pageState["SessionLicense"] = licience;
            eRxWeb.ePrescribeSvc.Permission permission = new eRxWeb.ePrescribeSvc.Permission();
            permission.ClaimType = UserPermissionNames.EPCS_CAN_APPROVE;
            permission.Value = UserPermissionNames.EPCS_CAN_APPROVE;
            eRxWeb.ePrescribeSvc.Permission perPrescribe = new eRxWeb.ePrescribeSvc.Permission();
            perPrescribe.ClaimType = perPrescribe.Value = UserPermissionNames.EPCS_CAN_PRESCRIBE;
            eRxWeb.ePrescribeSvc.Permission[] permissions = { permission, perPrescribe };
            _pageState[SessionVariables.UserAppPermissions] = permissions;
            _pageState["TaskScriptMessageId"] = mockedObjects.GetGuid();
            List<Rx> rxList = mockedObjects.SetScriptsPadCsMeds();
            ArrayList list = mockedObjects.SetRxListIsFreeFormMedControlSubstance();
            _pageState[SessionVariables.RxList] = list;
            _pageState[SessionVariables.CurrentScriptPadMeds] = rxList;
            List<string> ls = new List<string>();
            ls.Add("1");
            _pageState[SessionVariables.SiteEPCSAuthorizedSchedules] = ls;
            _pageState[SessionVariables.IsCsRefReqWorkflow] = true;
            DurMultiSelect durMultiSelect = new DurMultiSelect();
            durMultiSelect.SetPageState(_pageState);
            SubmitDurRequest submitDurRequest = new SubmitDurRequest();
            submitDurRequest.IsCapturedReasons = true;
            submitDurRequest.starsAlign = true;
            submitDurRequest.ProviderId = mockedObjects.GetGuidInString();
            var res = durMultiSelect.SaveDurForm(submitDurRequest, impactDur);
            Assert.IsNotNull(res);
        }
        [TestMethod]
        public void Is_With_deligate_provider_CSMedRefillRequestNotAllowedOnPrintRefillRequest_Sucessfull()
        {
            HttpContext.Current = MockedObjects.MockHttpContext();
            var impactDur = MockRepository.GenerateStub<IImpactDurWraper>();
            Allscripts.Impact.ScriptMsg.ScriptMessage scriptsMessage = Impact.ScriptMsg.ScriptMessage.CreateScriptMessageForTest(MockedObjects.xmlScriptMessage);// MockRepository.GenerateStub<Allscripts.Impact.ScriptMsg.ScriptMessage>();
            var scriptMessage = impactDur.Stub(imD => imD.CreateNewScriptMessage(mockedObjects.SetUpTaskScriptMessageId().ToString(), mockedObjects.GetGuid().ToString(), mockedObjects.GetGuid().ToString(), mockedObjects.SetUpDBId())).Return(scriptsMessage);
            _pageState = new StateContainer(HttpContext.Current.Session);
            _pageState[SessionVariables.TaskScriptMessageId] = mockedObjects.GetGuid();
            _pageState[SessionVariables.LicenseId] = mockedObjects.GetGuid();
            _pageState[SessionVariables.UserId] = mockedObjects.GetGuid();
            _pageState[SessionVariables.DbId] = mockedObjects.SetUpDBId();
            _pageState[SessionVariables.SiteId] = 1;
            _pageState[SessionVariables.isCSRegistryChecked] = true;
            _pageState[SessionVariables.PerformFormulary] = "PerformFormulary";
            _pageState[SessionVariables.RxWorkFlow] = PrescriptionWorkFlow.AFTER_FIRST_FILL;
            _pageState[SessionVariables.ExternalFacilityCd] = "ExternalFacilityCd";
            _pageState[SessionVariables.ExternalGroupID] = "ExternalGroupID";
            _pageState["DEA"] = "CA";
            _pageState[SessionVariables.DelegateProviderId] = mockedObjects.GetGuidInString();
            _pageState[SessionVariables.UserType] = UserCategory.POB_LIMITED;
            DurMultiSelect durMultiSelect = new DurMultiSelect();
            durMultiSelect.SetPageState(_pageState);
            var requestOnPrintRefillRequestMock = MockRepository.GenerateStub<CSMedRefillNotAllowedPrintRefillRequest>();
            CSMedRefillNotAllowedPrintRefillResponse res = durMultiSelect.CSMedRefillRequestNotAllowedOnPrintRefillRequest(requestOnPrintRefillRequestMock, impactDur);
            Assert.IsNotNull(res);
        }


        [TestMethod]
        public void Is_With_provider_CSMedRefillRequestNotAllowedOnPrintRefillRequest_Sucessfull()
        {
            HttpContext.Current = MockedObjects.MockHttpContext();
            var impactDur = MockRepository.GenerateStub<IImpactDurWraper>();
            Allscripts.Impact.ScriptMsg.ScriptMessage scriptsMessage = Impact.ScriptMsg.ScriptMessage.CreateScriptMessageForTest(MockedObjects.xmlScriptMessage);// MockRepository.GenerateStub<Allscripts.Impact.ScriptMsg.ScriptMessage>();
            var scriptMessage = impactDur.Stub(imD => imD.CreateNewScriptMessage(mockedObjects.SetUpTaskScriptMessageId().ToString(), mockedObjects.GetGuid().ToString(), mockedObjects.GetGuid().ToString(), mockedObjects.SetUpDBId())).Return(scriptsMessage);
            _pageState = new StateContainer(HttpContext.Current.Session);
            _pageState[SessionVariables.TaskScriptMessageId] = mockedObjects.GetGuid();
            _pageState[SessionVariables.LicenseId] = mockedObjects.GetGuid();
            _pageState[SessionVariables.UserId] = mockedObjects.GetGuid();
            _pageState[SessionVariables.DbId] = mockedObjects.SetUpDBId();
            _pageState[SessionVariables.SiteId] = 1;
            _pageState[SessionVariables.isCSRegistryChecked] = true;
            _pageState[SessionVariables.PerformFormulary] = "PerformFormulary";
            _pageState[SessionVariables.RxWorkFlow] = PrescriptionWorkFlow.AFTER_FIRST_FILL;
            _pageState[SessionVariables.ExternalFacilityCd] = "ExternalFacilityCd";
            _pageState[SessionVariables.ExternalGroupID] = "ExternalGroupID";
            _pageState["DEA"] = "CA";
            _pageState[SessionVariables.DelegateProviderId] = mockedObjects.GetGuidInString();
            _pageState[SessionVariables.UserType] = UserCategory.PROVIDER;
            DurMultiSelect durMultiSelect = new DurMultiSelect();
            durMultiSelect.SetPageState(_pageState);
            var requestOnPrintRefillRequestMock = MockRepository.GenerateStub<CSMedRefillNotAllowedPrintRefillRequest>();
            CSMedRefillNotAllowedPrintRefillResponse res = durMultiSelect.CSMedRefillRequestNotAllowedOnPrintRefillRequest(requestOnPrintRefillRequestMock, impactDur);
            Assert.IsNotNull(res);
        }

    }
}
