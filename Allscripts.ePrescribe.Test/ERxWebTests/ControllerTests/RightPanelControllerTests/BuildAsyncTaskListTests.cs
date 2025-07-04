using System;
using System.Collections.Generic;
using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.Shared.Data;
using eRxWeb.AppCode;
using eRxWeb.AppCode.Interfaces;
using eRxWeb.Controller;
using eRxWeb.ServerModel;
using eRxWeb.State;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;

namespace Allscripts.ePrescribe.Test.ERxWebTests.ControllerTests.RightPanelControllerTests
{
    [TestClass]
    public class BuildAsyncTaskListTests
    {
        [TestMethod]
        public void should_exit_if_page_name_doesnt_exist()
        {
            //arrange
            var asyncContext = new AsyncContext(null, new LoggingInfo(), "PageDoesntExist", "");

            //act
            var result = RightPanelController.BuildAsyncTaskList(new LoadAsyncUtil(asyncContext, null));

            //assert
            Assert.AreEqual(0, result.Item1.Count);
        }

        [TestMethod]
        public void should_create_selectPatient_tasks()
        {
            //arrange
            var asyncContext = new AsyncContext(null, new LoggingInfo(), Constants.PageNames.SELECT_PATIENT, "");

            var asyncUtilMock = MockRepository.GenerateMock<ILoadAsyncUtil>();
            asyncUtilMock.Stub(_ => _.AsyncContext).Return(asyncContext);

            //act
            var result = RightPanelController.BuildAsyncTaskList(asyncUtilMock);

            //assert
            AssertUrgentMessagesPanel(asyncUtilMock);
            AssertEpcsNoticePanel(asyncUtilMock);
            AssertImportantInfoPanel(asyncUtilMock);
            AssertEAuthMessagesPanel(asyncUtilMock);
            AssertRightBoxPanel(asyncUtilMock);
            AssertHelpContentPanel(asyncUtilMock);

            Assert.AreEqual(6, result.Item1.Count);
        }

        [TestMethod]
        public void should_create_selectMedication_tasks()
        {
            //arrange
            var asyncContext = new AsyncContext(null, new LoggingInfo(), Constants.PageNames.SELECT_MEDICATION, "");

            var asyncUtilMock = MockRepository.GenerateMock<ILoadAsyncUtil>();
            asyncUtilMock.Stub(_ => _.AsyncContext).Return(asyncContext);

            //act
            var result = RightPanelController.BuildAsyncTaskList(asyncUtilMock);

            //assert
            AssertScriptPadPanel(asyncUtilMock);
            AssertRightBoxPanel(asyncUtilMock);
            AssertHelpContentPanel(asyncUtilMock);

            Assert.AreEqual(3, result.Item1.Count);
        }

        [TestMethod]
        public void should_create_approveRefillTask_tasks()
        {
            //arrange
            var asyncContext = new AsyncContext(null, new LoggingInfo(), Constants.PageNames.APPROVE_REFILL_TASK, "");

            var asyncUtilMock = MockRepository.GenerateMock<ILoadAsyncUtil>();
            asyncUtilMock.Stub(_ => _.AsyncContext).Return(asyncContext);

            //act
            var result = RightPanelController.BuildAsyncTaskList(asyncUtilMock);

            //assert
            AssertHelpContentPanel(asyncUtilMock);

            Assert.AreEqual(1, result.Item1.Count);
        }

        [TestMethod]
        public void should_create_docRefillMenu_tasks()
        {
            //arrange
            var asyncContext = new AsyncContext(null, new LoggingInfo(), Constants.PageNames.DOC_REFILL_MENU, "");

            var asyncUtilMock = MockRepository.GenerateMock<ILoadAsyncUtil>();
            asyncUtilMock.Stub(_ => _.AsyncContext).Return(asyncContext);

            //act
            var result = RightPanelController.BuildAsyncTaskList(asyncUtilMock);

            //assert
            AssertUrgentMessagesPanel(asyncUtilMock);
            AssertEAuthMessagesPanel(asyncUtilMock);
            AssertHelpContentPanel(asyncUtilMock);

            Assert.AreEqual(3, result.Item1.Count);
        }

        [TestMethod]
        public void should_create_listSendScripts_tasks()
        {
            //arrange
            var asyncContext = new AsyncContext(null, new LoggingInfo(), Constants.PageNames.LIST_SEND_SCRIPTS, "");

            var asyncUtilMock = MockRepository.GenerateMock<ILoadAsyncUtil>();
            asyncUtilMock.Stub(_ => _.AsyncContext).Return(asyncContext);

            //act
            var result = RightPanelController.BuildAsyncTaskList(asyncUtilMock);

            //assert
            AssertUrgentMessagesPanel(asyncUtilMock);
            AssertEAuthMessagesPanel(asyncUtilMock);
            AssertHelpContentPanel(asyncUtilMock);

            Assert.AreEqual(3, result.Item1.Count);
        }

        [TestMethod]
        public void should_create_taskSummary_tasks()
        {
            //arrange
            var asyncContext = new AsyncContext(null, new LoggingInfo(), Constants.PageNames.TASK_SUMMARY, "");

            var asyncUtilMock = MockRepository.GenerateMock<ILoadAsyncUtil>();
            asyncUtilMock.Stub(_ => _.AsyncContext).Return(asyncContext);

            //act
            var result = RightPanelController.BuildAsyncTaskList(asyncUtilMock);

            //assert
            AssertUrgentMessagesPanel(asyncUtilMock);
            AssertEAuthMessagesPanel(asyncUtilMock);

            Assert.AreEqual(2, result.Item1.Count);
        }

        [TestMethod]
        public void should_create_reviewHistory_tasks()
        {
            //arrange
            var asyncContext = new AsyncContext(null, new LoggingInfo(), Constants.PageNames.REVIEW_HISTORY, "");

            var asyncUtilMock = MockRepository.GenerateMock<ILoadAsyncUtil>();
            asyncUtilMock.Stub(_ => _.AsyncContext).Return(asyncContext);

            //act
            var result = RightPanelController.BuildAsyncTaskList(asyncUtilMock);

            //assert
            AssertEpcsNoticePanel(asyncUtilMock);
            AssertImportantInfoPanel(asyncUtilMock);
            AssertHelpContentPanel(asyncUtilMock);

            Assert.AreEqual(3, result.Item1.Count);
        }

        [TestMethod]
        public void should_create_nurseSig_tasks()
        {
            //arrange
            var asyncContext = new AsyncContext(null, new LoggingInfo(), Constants.PageNames.NURSE_SIG, "");

            var asyncUtilMock = MockRepository.GenerateMock<ILoadAsyncUtil>();
            asyncUtilMock.Stub(_ => _.AsyncContext).Return(asyncContext);

            //act
            var result = RightPanelController.BuildAsyncTaskList(asyncUtilMock);

            //assert
            AssertScriptPadPanel(asyncUtilMock);
            AssertEAuthMessagesPanel(asyncUtilMock);
            AssertSigCopayPanel(asyncUtilMock);
            AssertHelpContentPanel(asyncUtilMock);

            Assert.AreEqual(4, result.Item1.Count);
        }

        [TestMethod]
        public void should_create_sig_tasks()
        {
            //arrange
            var asyncContext = new AsyncContext(null, new LoggingInfo(), Constants.PageNames.SIG, "");

            var asyncUtilMock = MockRepository.GenerateMock<ILoadAsyncUtil>();
            asyncUtilMock.Stub(_ => _.AsyncContext).Return(asyncContext);

            //act
            var result = RightPanelController.BuildAsyncTaskList(asyncUtilMock);

            //assert
            AssertScriptPadPanel(asyncUtilMock);
            AssertSigCopayPanel(asyncUtilMock);
            AssertHelpContentPanel(asyncUtilMock);

            Assert.AreEqual(3, result.Item1.Count);
        }

        [TestMethod]
        public void should_create_rxDurReviewMultiSelect_tasks()
        {
            //arrange
            var asyncContext = new AsyncContext(null, new LoggingInfo(), Constants.PageNames.RX_DUR_REVIEW_MULTI_SELECT, "");

            var asyncUtilMock = MockRepository.GenerateMock<ILoadAsyncUtil>();
            asyncUtilMock.Stub(_ => _.AsyncContext).Return(asyncContext);

            //act
            var result = RightPanelController.BuildAsyncTaskList(asyncUtilMock);

            //assert
            AssertScriptPadPanel(asyncUtilMock);

            Assert.AreEqual(1, result.Item1.Count);
        }

        [TestMethod]
        public void should_create_pharmacy_tasks()
        {
            //arrange
            var asyncContext = new AsyncContext(null, new LoggingInfo(), Constants.PageNames.PHARMACY, "");

            var asyncUtilMock = MockRepository.GenerateMock<ILoadAsyncUtil>();
            asyncUtilMock.Stub(_ => _.AsyncContext).Return(asyncContext);

            //act
            var result = RightPanelController.BuildAsyncTaskList(asyncUtilMock);

            //assert
            AssertHelpContentPanel(asyncUtilMock);

            Assert.AreEqual(1, result.Item1.Count);
        }

        [TestMethod]
        public void should_create_messageQueue_tasks()
        {
            //arrange
            var asyncContext = new AsyncContext(null, new LoggingInfo(), Constants.PageNames.MESSAGE_QUEUE_TX, "");

            var asyncUtilMock = MockRepository.GenerateMock<ILoadAsyncUtil>();
            asyncUtilMock.Stub(_ => _.AsyncContext).Return(asyncContext);

            //act
            var result = RightPanelController.BuildAsyncTaskList(asyncUtilMock);

            //assert
            AssertHelpContentPanel(asyncUtilMock);

            Assert.AreEqual(1, result.Item1.Count);
        }

        [TestMethod]
        public void should_create_scriptPad_tasks()
        {
            //arrange
            var asyncContext = new AsyncContext(null, new LoggingInfo(), Constants.PageNames.SCRIPT_PAD, "");

            var asyncUtilMock = MockRepository.GenerateMock<ILoadAsyncUtil>();
            asyncUtilMock.Stub(_ => _.AsyncContext).Return(asyncContext);

            //act
            var result = RightPanelController.BuildAsyncTaskList(asyncUtilMock);

            //assert
            AssertEpcsSendToPharmPanel(asyncUtilMock);
            AssertSpecialtyMedInfoPanel(asyncUtilMock);
            AssertHelpContentPanel(asyncUtilMock);
            AssertGetEpcsPanel(asyncUtilMock);

            Assert.AreEqual(4, result.Item1.Count);
        }

        [TestMethod]
        public void should_create_myProfile_tasks()
        {
            //arrange
            var asyncContext = new AsyncContext(null, new LoggingInfo(), Constants.PageNames.MY_PROFILE, "");

            var asyncUtilMock = MockRepository.GenerateMock<ILoadAsyncUtil>();
            asyncUtilMock.Stub(_ => _.AsyncContext).Return(asyncContext);

            //act
            var result = RightPanelController.BuildAsyncTaskList(asyncUtilMock);

            //assert
            AssertHelpContentPanel(asyncUtilMock);

            Assert.AreEqual(1, result.Item1.Count);
        }


        [TestMethod]
        public void should_create_settings_tasks()
        {
            //arrange
            var asyncContext = new AsyncContext(null, new LoggingInfo(), Constants.PageNames.SETTINGS, "");

            var asyncUtilMock = MockRepository.GenerateMock<ILoadAsyncUtil>();
            asyncUtilMock.Stub(_ => _.AsyncContext).Return(asyncContext);

            //act
            var result = RightPanelController.BuildAsyncTaskList(asyncUtilMock);

            //assert
            AssertHelpContentPanel(asyncUtilMock);

            Assert.AreEqual(1, result.Item1.Count);
        }

        [TestMethod]
        public void should_create_nursePharmacy_tasks()
        {
            //arrange
            var asyncContext = new AsyncContext(null, new LoggingInfo(), Constants.PageNames.NURSE_PHARMACY, "");

            var asyncUtilMock = MockRepository.GenerateMock<ILoadAsyncUtil>();
            asyncUtilMock.Stub(_ => _.AsyncContext).Return(asyncContext);

            //act
            var result = RightPanelController.BuildAsyncTaskList(asyncUtilMock);

            //assert
            AssertHelpContentPanel(asyncUtilMock);

            Assert.AreEqual(1, result.Item1.Count);
        }

        [TestMethod]
        public void should_create_reports_tasks()
        {
            //arrange
            var asyncContext = new AsyncContext(null, new LoggingInfo(), Constants.PageNames.REPORTS, "");

            var asyncUtilMock = MockRepository.GenerateMock<ILoadAsyncUtil>();
            asyncUtilMock.Stub(_ => _.AsyncContext).Return(asyncContext);

            //act
            var result = RightPanelController.BuildAsyncTaskList(asyncUtilMock);

            //assert
            AssertHelpContentPanel(asyncUtilMock);

            Assert.AreEqual(1, result.Item1.Count);
        }

        [TestMethod]
        public void should_create_taskScriptList_tasks()
        {
            //arrange
            var asyncContext = new AsyncContext(null, new LoggingInfo(), Constants.PageNames.TASK_SCRIPT_LIST, "");

            var asyncUtilMock = MockRepository.GenerateMock<ILoadAsyncUtil>();
            asyncUtilMock.Stub(_ => _.AsyncContext).Return(asyncContext);

            //act
            var result = RightPanelController.BuildAsyncTaskList(asyncUtilMock);

            //assert
            AssertHelpContentPanel(asyncUtilMock);

            Assert.AreEqual(1, result.Item1.Count);
        }

        [TestMethod]
        public void should_create_multipleView_tasks()
        {
            //arrange
            var asyncContext = new AsyncContext(null, new LoggingInfo(), Constants.PageNames.MULTIPLE_VIEW, "");

            var asyncUtilMock = MockRepository.GenerateMock<ILoadAsyncUtil>();
            asyncUtilMock.Stub(_ => _.AsyncContext).Return(asyncContext);

            //act
            var result = RightPanelController.BuildAsyncTaskList(asyncUtilMock);

            //assert
            AssertHelpContentPanel(asyncUtilMock);

            Assert.AreEqual(1, result.Item1.Count);
        }

        [TestMethod]
        public void should_create_multipleViewCss_tasks()
        {
            //arrange
            var asyncContext = new AsyncContext(null, new LoggingInfo(), Constants.PageNames.MULTIPLE_VIEW_CSS, "");

            var asyncUtilMock = MockRepository.GenerateMock<ILoadAsyncUtil>();
            asyncUtilMock.Stub(_ => _.AsyncContext).Return(asyncContext);

            //act
            var result = RightPanelController.BuildAsyncTaskList(asyncUtilMock);

            //assert
            AssertHelpContentPanel(asyncUtilMock);

            Assert.AreEqual(1, result.Item1.Count);
        }

        [TestMethod]
        public void should_create_changePatInsurance_tasks()
        {
            //arrange
            var asyncContext = new AsyncContext(null, new LoggingInfo(), Constants.PageNames.CHANGE_PATIENT_INSURANCE, "");

            var asyncUtilMock = MockRepository.GenerateMock<ILoadAsyncUtil>();
            asyncUtilMock.Stub(_ => _.AsyncContext).Return(asyncContext);

            //act
            var result = RightPanelController.BuildAsyncTaskList(asyncUtilMock);

            //assert
            AssertHelpContentPanel(asyncUtilMock);

            Assert.AreEqual(1, result.Item1.Count);
        }

        [TestMethod]
        public void should_create_changeShieldPassword_tasks()
        {
            //arrange
            var asyncContext = new AsyncContext(null, new LoggingInfo(), Constants.PageNames.CHANGE_SHIELD_USER_PASSWORD, "");

            var asyncUtilMock = MockRepository.GenerateMock<ILoadAsyncUtil>();
            asyncUtilMock.Stub(_ => _.AsyncContext).Return(asyncContext);

            //act
            var result = RightPanelController.BuildAsyncTaskList(asyncUtilMock);

            //assert
            AssertHelpContentPanel(asyncUtilMock);

            Assert.AreEqual(1, result.Item1.Count);
        }

        [TestMethod]
        public void should_create_editUser_tasks()
        {
            //arrange
            var asyncContext = new AsyncContext(null, new LoggingInfo(), Constants.PageNames.EDIT_USER, "");

            var asyncUtilMock = MockRepository.GenerateMock<ILoadAsyncUtil>();
            asyncUtilMock.Stub(_ => _.AsyncContext).Return(asyncContext);

            //act
            var result = RightPanelController.BuildAsyncTaskList(asyncUtilMock);

            //assert
            AssertHelpContentPanel(asyncUtilMock);

            Assert.AreEqual(1, result.Item1.Count);
        }

        [TestMethod]
        public void should_create_nurseFreeFormDrug_tasks()
        {
            //arrange
            var asyncContext = new AsyncContext(null, new LoggingInfo(), Constants.PageNames.NURSE_FREE_FORM_DRUG, "");

            var asyncUtilMock = MockRepository.GenerateMock<ILoadAsyncUtil>();
            asyncUtilMock.Stub(_ => _.AsyncContext).Return(asyncContext);

            //act
            var result = RightPanelController.BuildAsyncTaskList(asyncUtilMock);

            //assert
            AssertHelpContentPanel(asyncUtilMock);

            Assert.AreEqual(1, result.Item1.Count);
        }

        [TestMethod]
        public void should_create_freeFormDrug_tasks()
        {
            //arrange
            var asyncContext = new AsyncContext(null, new LoggingInfo(), Constants.PageNames.FREE_FORM_DRUG, "");

            var asyncUtilMock = MockRepository.GenerateMock<ILoadAsyncUtil>();
            asyncUtilMock.Stub(_ => _.AsyncContext).Return(asyncContext);

            //act
            var result = RightPanelController.BuildAsyncTaskList(asyncUtilMock);

            //assert
            AssertHelpContentPanel(asyncUtilMock);

            Assert.AreEqual(1, result.Item1.Count);
        }

        [TestMethod]
        public void should_create_patMedHistory_tasks()
        {
            //arrange
            var asyncContext = new AsyncContext(null, new LoggingInfo(), Constants.PageNames.PATIENT_MED_HISTORY, "");

            var asyncUtilMock = MockRepository.GenerateMock<ILoadAsyncUtil>();
            asyncUtilMock.Stub(_ => _.AsyncContext).Return(asyncContext);

            //act
            var result = RightPanelController.BuildAsyncTaskList(asyncUtilMock);

            //assert
            AssertHelpContentPanel(asyncUtilMock);

            Assert.AreEqual(1, result.Item1.Count);
        }

        [TestMethod]
        public void should_create_pharmRefillSummary_tasks()
        {
            //arrange
            var asyncContext = new AsyncContext(null, new LoggingInfo(), Constants.PageNames.PHARMACY_REFILL_SUMMARY, "");

            var asyncUtilMock = MockRepository.GenerateMock<ILoadAsyncUtil>();
            asyncUtilMock.Stub(_ => _.AsyncContext).Return(asyncContext);

            //act
            var result = RightPanelController.BuildAsyncTaskList(asyncUtilMock);

            //assert
            AssertHelpContentPanel(asyncUtilMock);

            Assert.AreEqual(1, result.Item1.Count);
        }

        [TestMethod]
        public void should_create_myEpcsReports_tasks()
        {
            //arrange
            var asyncContext = new AsyncContext(null, new LoggingInfo(), Constants.PageNames.MY_EPCS_REPORTS, "");

            var asyncUtilMock = MockRepository.GenerateMock<ILoadAsyncUtil>();
            asyncUtilMock.Stub(_ => _.AsyncContext).Return(asyncContext);

            //act
            var result = RightPanelController.BuildAsyncTaskList(asyncUtilMock);

            //assert
            AssertHelpContentPanel(asyncUtilMock);

            Assert.AreEqual(1, result.Item1.Count);
        }

        [TestMethod]
        public void should_create_prebuildMedSelectSig_tasks()
        {
            //arrange
            var asyncContext = new AsyncContext(null, new LoggingInfo(), Constants.PageNames.PRE_BUILT_MED_SELECT_SIG, "");

            var asyncUtilMock = MockRepository.GenerateMock<ILoadAsyncUtil>();
            asyncUtilMock.Stub(_ => _.AsyncContext).Return(asyncContext);

            //act
            var result = RightPanelController.BuildAsyncTaskList(asyncUtilMock);

            //assert
            AssertHelpContentPanel(asyncUtilMock);

            Assert.AreEqual(1, result.Item1.Count);
        }

        [TestMethod]
        public void should_create_rxDurSelectSig_tasks()
        {
            //arrange
            var asyncContext = new AsyncContext(null, new LoggingInfo(), Constants.PageNames.RX_DUR_REVIEW, "");

            var asyncUtilMock = MockRepository.GenerateMock<ILoadAsyncUtil>();
            asyncUtilMock.Stub(_ => _.AsyncContext).Return(asyncContext);

            //act
            var result = RightPanelController.BuildAsyncTaskList(asyncUtilMock);

            //assert
            AssertHelpContentPanel(asyncUtilMock);

            Assert.AreEqual(1, result.Item1.Count);
        }

        [TestMethod]
        public void should_create_selectDx_tasks()
        {
            //arrange
            var asyncContext = new AsyncContext(null, new LoggingInfo(), Constants.PageNames.SELECT_DX, "");

            var asyncUtilMock = MockRepository.GenerateMock<ILoadAsyncUtil>();
            asyncUtilMock.Stub(_ => _.AsyncContext).Return(asyncContext);

            //act
            var result = RightPanelController.BuildAsyncTaskList(asyncUtilMock);

            //assert
            AssertHelpContentPanel(asyncUtilMock);

            Assert.AreEqual(1, result.Item1.Count);
        }

        [TestMethod]
        public void should_create_siteManagement_tasks()
        {
            //arrange
            var asyncContext = new AsyncContext(null, new LoggingInfo(), Constants.PageNames.SITE_MANAGEMENT, "");

            var asyncUtilMock = MockRepository.GenerateMock<ILoadAsyncUtil>();
            asyncUtilMock.Stub(_ => _.AsyncContext).Return(asyncContext);

            //act
            var result = RightPanelController.BuildAsyncTaskList(asyncUtilMock);

            //assert
            AssertHelpContentPanel(asyncUtilMock);

            Assert.AreEqual(1, result.Item1.Count);
        }

        [TestMethod]
        public void should_create_approveScriptMessagePat_tasks()
        {
            //arrange
            var asyncContext = new AsyncContext(null, new LoggingInfo(), Constants.PageNames.APPROVE_SCRIPT_MESSAGE_PATIENT, "");

            var asyncUtilMock = MockRepository.GenerateMock<ILoadAsyncUtil>();
            asyncUtilMock.Stub(_ => _.AsyncContext).Return(asyncContext);

            //act
            var result = RightPanelController.BuildAsyncTaskList(asyncUtilMock);

            //assert
            AssertHelpContentPanel(asyncUtilMock);

            Assert.AreEqual(1, result.Item1.Count);
        }

        [TestMethod]
        public void should_create_patientDiagnosis_tasks()
        {
            //arrange
            var asyncContext = new AsyncContext(null, new LoggingInfo(), Constants.PageNames.PATIENT_DIAGNOSIS, "");

            var asyncUtilMock = MockRepository.GenerateMock<ILoadAsyncUtil>();
            asyncUtilMock.Stub(_ => _.AsyncContext).Return(asyncContext);

            //act
            var result = RightPanelController.BuildAsyncTaskList(asyncUtilMock);

            //assert
            AssertHelpContentPanel(asyncUtilMock);

            Assert.AreEqual(1, result.Item1.Count);
        }

        [TestMethod]
        public void should_create_patientAllergy_tasks()
        {
            //arrange
            var asyncContext = new AsyncContext(null, new LoggingInfo(), Constants.PageNames.PATIENT_ALLERGY, "");

            var asyncUtilMock = MockRepository.GenerateMock<ILoadAsyncUtil>();
            asyncUtilMock.Stub(_ => _.AsyncContext).Return(asyncContext);

            //act
            var result = RightPanelController.BuildAsyncTaskList(asyncUtilMock);

            //assert
            AssertHelpContentPanel(asyncUtilMock);

            Assert.AreEqual(1, result.Item1.Count);
        }

        public void AssertUrgentMessagesPanel(ILoadAsyncUtil asyncUtil)
        {
            asyncUtil.AssertWasCalled(_ => _.CreateTask(Arg<Func<IStateContainer, UrgentMessageModel>>
                                                            .Matches(f => f.Method.Name == "checkUserMessages"),
                                                        Arg<Action<UrgentMessageModel>>.Is.Anything));
        }

        public void AssertEpcsNoticePanel(ILoadAsyncUtil asyncUtil)
        {
            asyncUtil.AssertWasCalled(_ => _.CreateTask(Arg<Func<IStateContainer, EPCSNotice>>
                                                            .Matches(f => f.Method.Name == "GetAvailableReportsCount"),
                                                        Arg<Action<EPCSNotice>>.Is.Anything));
        }

        public void AssertImportantInfoPanel(ILoadAsyncUtil asyncUtil)
        {
            asyncUtil.AssertWasCalled(_ => _.CreateTask(Arg<Func<IStateContainer, List<ImportantInfoModel>>>
                                                            .Matches(f => f.Method.Name == "checkImportantInfo"),
                                                        Arg<Action<List<ImportantInfoModel>>>.Is.Anything));
        }

        public void AssertEAuthMessagesPanel(ILoadAsyncUtil asyncUtil)
        {
            asyncUtil.AssertWasCalled(_ => _.CreateTask(Arg<Func<IStateContainer, EAuthMessageModel>>
                                                            .Matches(f => f.Method.Name == "getEauthMessage"),
                                                        Arg<Action<EAuthMessageModel>>.Is.Anything));
        }

        public void AssertRightBoxPanel(ILoadAsyncUtil asyncUtil)
        {
            asyncUtil.AssertWasCalled(_ => _.CreateTask(Arg<Func<IStateContainer, RightBoxModel>>
                                                            .Matches(f => f.Method.Name == "PrepareRightBoxData"),
                                                        Arg<Action<RightBoxModel>>.Is.Anything));
        }

        public void AssertHelpContentPanel(ILoadAsyncUtil asyncUtil)
        {
            asyncUtil.AssertWasCalled(_ => _.CreateTask(Arg<Func<string, IStateContainer, string>>
                                                            .Matches(f => f.Method.Name == "GetHelpContent"),
                                                        Arg<Action<string>>.Is.Anything));
        }

        public void AssertScriptPadPanel(ILoadAsyncUtil asyncUtil)
        {
            asyncUtil.AssertWasCalled(_ => _.CreateTask(Arg<Func<string, IStateContainer, List<ScriptPadModel>>>
                                                            .Matches(f => f.Method.Name == "GetCurrentScriptPadData"),
                                                        Arg<Action<List<ScriptPadModel>>>.Is.Anything));
        }

        public void AssertSigCopayPanel(ILoadAsyncUtil asyncUtil)
        {
            asyncUtil.AssertWasCalled(_ => _.CreateTask(Arg<Func<IStateContainer, FormularyAlternative>>
                                                            .Matches(f => f.Method.Name == "getFormularyAlternativesFromSession"),
                                                        Arg<Action<FormularyAlternative>>.Is.Anything));
        }

        public void AssertEpcsSendToPharmPanel(ILoadAsyncUtil asyncUtil)
        {
            asyncUtil.AssertWasCalled(_ => _.CreateTask(Arg<Func<IStateContainer, GetEpcsSendToPharmacyApiController.EpcsSendToPharmacyModel>>
                                                            .Matches(f => f.Method.Name == "getEpcsSendToPharm"),
                                                        Arg<Action<GetEpcsSendToPharmacyApiController.EpcsSendToPharmacyModel>>.Is.Anything));
        }

        public void AssertSpecialtyMedInfoPanel(ILoadAsyncUtil asyncUtil)
        {
            asyncUtil.AssertWasCalled(_ => _.CreateTask(Arg<Func<IStateContainer, bool>>
                                                            .Matches(f => f.Method.Name == "isScriptPadHasSpecMed"),
                                                        Arg<Action<bool>>.Is.Anything));
        }

        public void AssertGetEpcsPanel(ILoadAsyncUtil asyncUtil)
        {
            asyncUtil.AssertWasCalled(_ => _.CreateTask(Arg<Func<IStateContainer, bool>>
                                                            .Matches(f => f.Method.Name == "displayEpcsLink"),
                                                        Arg<Action<bool>>.Is.Anything));
        }
    }
}
