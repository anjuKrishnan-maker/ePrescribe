using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Web;
using Allscripts.ePrescribe.DatabaseSelector;
using Allscripts.ePrescribe.ExtensionMethods;
using Allscripts.ePrescribe.Medispan.Clinical.Model;
using Allscripts.ePrescribe.Medispan.Clinical.Model.Response;
using Allscripts.ePrescribe.Test.Common;
using Allscripts.Impact;
using eRxWeb.AppCode.DurBPL;
using eRxWeb.State;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using static Allscripts.ePrescribe.Common.Constants;
using static Allscripts.Impact.IgnoreReason;

namespace Allscripts.ePrescribe.Test.DurTests
{
    [TestClass]
    public class DurWarningMultiSelectTests
    {
        MockedObjects mockedObjects = new MockedObjects();
        IStateContainer _pageState;
       
        [TestMethod]
        public void Get_Dur_Rx_List__When_IsAnyDurSetting_On()
        {            
             HttpContext.Current = MockedObjects.MockHttpContext();
            _pageState = new StateContainer(HttpContext.Current.Session);
            var appCodeWrapperMock = MockRepository.GenerateStub<IeRxWebAppCodeWrapper>();
            var rxList = mockedObjects.SetScriptsPadMeds();
            var lstFreeFormDrug = mockedObjects.SetFreeFormRxDrug();  
            DURResponse durResponse = new DURResponse();
            var durWarnings = mockedObjects.SetupDurWarnings();
            durResponse.DurCheckResponse = durWarnings;
            durResponse.MedispanCopyright = "copyright";
            var activeMedList = mockedObjects.SetActiveMedList();
            var activeAllergies = mockedObjects.SetDurPatientAllergies();
            var durSettings = mockedObjects.SetDurSettings(); 
            
            _pageState[SessionVariables.PatientDOB] = "12/12/1990";
            _pageState[SessionVariables.Gender] = "M";      
            _pageState[SessionVariables.PatientId] = "0ef45408-7c2f-41c9-b610-49eac407f86f";          
            _pageState[SessionVariables.ACTIVEMEDDDILIST] = activeMedList;
            _pageState["CURRENT_SCRIPT_PAD_MEDS"] = rxList;
            _pageState[SessionVariables.RxList] = rxList;
            _pageState[SessionVariables.DurPatientAllergies] = activeAllergies;
            _pageState[SessionVariables.DURSettings] = durSettings;
            RxTaskModel taskModel = mockedObjects.SetRxTaskModel();
            _pageState[SessionVariables.RxTask] = taskModel;
           
            PrescriptionTaskType taskType=PrescriptionTaskType.APPROVAL_REQUEST;
            appCodeWrapperMock.Stub(x => x.GetDurWarnings("12/12/1990","M", rxList, activeMedList, activeAllergies, durSettings)).Return(durResponse);
            appCodeWrapperMock.Stub(x => x.IsAnyDurSettingOn(durSettings)).Return(true);
            appCodeWrapperMock.Stub(x => x.RetrieveTaskType(taskModel, taskType)).Return(taskType);
            appCodeWrapperMock.Stub(x => x.RetrieveDrugsListBasedOnWorkflowType(_pageState.Cast(SessionVariables.RxList, new ArrayList()).ToList<Rx>(), rxList, taskType)).Return(rxList);
            appCodeWrapperMock.Stub(x => x.DurWarningsHasItems(durWarnings)).Return(true);
            appCodeWrapperMock.Stub(x => x.RetrieveFreeFormDrugsHasItems(rxList)).Return(true);
            appCodeWrapperMock.Stub(x => x.FoodInteractionsHasItems(durWarnings)).Return(true);
            appCodeWrapperMock.Stub(x => x.AlcoholInteractionsHasItems(durWarnings)).Return(true);
            appCodeWrapperMock.Stub(x => x.PriorAdverseReactionsHasItems(durWarnings)).Return(true);
            appCodeWrapperMock.Stub(x => x.DuplicateTherapyHasItems(durWarnings)).Return(true);
            appCodeWrapperMock.Stub(x => x.DrugInteractionsHasItems(durWarnings)).Return(true);
            appCodeWrapperMock.Stub(x => x.DosageCheckMedicationsHasItems(durWarnings)).Return(true);

            DurMultiSelect durMultiSelect = new DurMultiSelect();
            durMultiSelect.SetPageState(_pageState);  
            var res=durMultiSelect.GetDurRxList(appCodeWrapperMock);
            Assert.IsNotNull(res);
        }

        [TestMethod]
        public void Get_Dur_When_Session_not_empty()
        {
            HttpContext.Current = MockedObjects.MockHttpContext();
            _pageState = new StateContainer(HttpContext.Current.Session);
            var appCodeWrapperMock = MockRepository.GenerateStub<IeRxWebAppCodeWrapper>();
            var rxList = mockedObjects.SetScriptsPadMeds();           
            DURResponse durResponse = new DURResponse();
            var durWarnings = mockedObjects.SetupDurWarnings();
            durResponse.DurCheckResponse = durWarnings;
            durResponse.MedispanCopyright = "copyright";
            var activeMedList = mockedObjects.SetActiveMedList();
            var activeAllergies = mockedObjects.SetDurPatientAllergies();
            var durSettings = mockedObjects.SetDurSettings();

            _pageState[SessionVariables.PatientDOB] = "12/12/1990";
            _pageState[SessionVariables.Gender] = "M";
            _pageState[SessionVariables.PatientId] = "0ef45408-7c2f-41c9-b610-49eac407f86f";
            _pageState[SessionVariables.ACTIVEMEDDDILIST] = activeMedList;
            _pageState["CURRENT_SCRIPT_PAD_MEDS"] = rxList;
            _pageState[SessionVariables.RxList] = rxList;
            _pageState[SessionVariables.DurPatientAllergies] = activeAllergies;
            _pageState[SessionVariables.DURSettings] = durSettings;         
            appCodeWrapperMock.Stub(x => x.GetDurWarnings("12/12/1990", "M", rxList, activeMedList, activeAllergies, durSettings)).Return(durResponse);
            DurMultiSelect durMultiSelect = new DurMultiSelect();
            durMultiSelect.SetPageState(_pageState);
            var res = durMultiSelect.GetDurWarnings(appCodeWrapperMock);
            Assert.IsNotNull(res);
        }

        [TestMethod]
        public void When_session_go_next_not_empty_get_redirect_url_as_set_in_session() 
        {
            HttpContext.Current = MockedObjects.MockHttpContext();
            _pageState = new StateContainer(HttpContext.Current.Session);        
            _pageState[SessionVariables.DUR_GO_NEXT] = "gonext";           
            DurMultiSelect durMultiSelect = new DurMultiSelect();
            durMultiSelect.SetPageState(_pageState);
            var res = durMultiSelect.GetWarningRedirectDetails();
            Assert.IsNotNull(res);
            Assert.AreEqual(res.RedirectUrl, "gonext");            
        }
        [TestMethod]
        public void When_session_camefrom_not_empty_get_redirect_url_as_set_in_session()
        {
            HttpContext.Current = MockedObjects.MockHttpContext();
            _pageState = new StateContainer(HttpContext.Current.Session);
            _pageState[SessionVariables.CameFrom] = "camefrom";
            DurMultiSelect durMultiSelect = new DurMultiSelect();
            durMultiSelect.SetPageState(_pageState);
            var res = durMultiSelect.GetWarningRedirectDetails();
            Assert.IsNotNull(res);
            Assert.AreEqual(res.RedirectUrl, "camefrom");
        }
        [TestMethod]
        public void get_ignore_reasons_by_category_and_usertype_when_session_not_empty()
        {
            HttpContext.Current = MockedObjects.MockHttpContext();
            _pageState = new StateContainer(HttpContext.Current.Session);
            var impactDurWrapper= MockRepository.GenerateStub<IImpactDurWraper>();
            IgnoreCategory ignoreCategory = IgnoreCategory.DUR_PAR;
            DataTable dtIgnoreReasons = mockedObjects.SetIgnoreReasonsDataTable();

            _pageState[SessionVariables.DbId] = ConnectionStringPointer.ERXDB_SERVER_1;
            _pageState[SessionVariables.UserType] = UserCategory.PROVIDER;

            impactDurWrapper.Stub(x => x.GetIgnoreReasons(ignoreCategory, UserCategory.PROVIDER, ConnectionStringPointer.ERXDB_SERVER_1)).Return(dtIgnoreReasons);         
            DurMultiSelect durMultiSelect = new DurMultiSelect();
            durMultiSelect.SetPageState(_pageState);
            var res = durMultiSelect.GetIgnoreReasonsByCategoryAndUserType(ignoreCategory,impactDurWrapper);
            Assert.IsNotNull(res);
        }
    }
}
