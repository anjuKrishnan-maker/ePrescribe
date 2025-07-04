using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.Medispan.Clinical.Model.Response;
using Allscripts.ePrescribe.Test.Common;
using eRxWeb.AppCode.DurBPL;
using eRxWeb.AppCode.DurBPL.ResponseModel;
using eRxWeb.Controller;
using eRxWeb.ServerModel;
using eRxWeb.State;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Web;
using Allscripts.Impact;
using Constants = Allscripts.ePrescribe.Common.Constants;
using static Allscripts.ePrescribe.Common.Constants;
using static Allscripts.Impact.IgnoreReason;
using Allscripts.ePrescribe.DatabaseSelector;

namespace Allscripts.ePrescribe.Test.ERxWebTests.ControllerTests.RxDurReviewApiControllerTest
{
    [TestClass]
    public class GetDurWarningTests
    {
        MockedObjects mockedObjects = new MockedObjects();        
        IStateContainer sessionMock;

        [TestInitialize]
        public void init()
        {
            sessionMock = MockRepository.GenerateStub<IStateContainer>();
        }
       
       [TestMethod]
        public void When_RxTaskModel_Not_Empty_Return_Dur()
        {
            HttpContext.Current = MockedObjects.MockHttpContext();
            sessionMock = new StateContainer(HttpContext.Current.Session);
            sessionMock[SessionVariables.RxList] = mockedObjects.SetupRxList();          
            var durWrapperMock = MockRepository.GenerateMock<IeRxWebAppCodeWrapper>();
            var multiSelectMock = MockRepository.GenerateMock<IDur>();
            RxDurResponseModel rxList = mockedObjects.SetupRxList();
             mockedObjects.SetSessionVariable();
            sessionMock[SessionVariables.TaskType] = PrescriptionTaskType.DEFAULT;
            sessionMock[SessionVariables.DURCheckResponse] = mockedObjects.SetupDurWarnings();          
            multiSelectMock.Stub(x => x.GetDurRxList(durWrapperMock)).Return(rxList);
            var rxDurReviewApiController = new RxDurReviewApiController(DurType.MULTI_SELECT);

            //Act
            var result = rxDurReviewApiController.DurRxList(durWrapperMock);

            //Assert           
            Assert.IsNotNull(result.Payload);
        }
        [TestMethod]
        public void When_Session_Is_Null_RxTaskModel_Throws_Exception()
        {
            var multiSelectMock = MockRepository.GenerateMock<IDur>();
            var durWrapperMock = MockRepository.GenerateMock<IeRxWebAppCodeWrapper>();
            var durBaseMock = MockRepository.GenerateMock<DurBase>();
            var impactDur = MockRepository.GenerateStub<IImpactDurWraper>();
            DurMultiSelect durMultiSelect = new DurMultiSelect();
            HttpContext.Current = MockedObjects.MockHttpContext();
            sessionMock = new StateContainer(HttpContext.Current.Session);
            sessionMock[SessionVariables.DbId] = mockedObjects.SetUpDBId();
            durMultiSelect.SetPageState(sessionMock);
            ErrorContextModel ErrorContext = new ErrorContextModel()
            {
                Error = ErrorTypeEnum.ServerError,
                Message = "Reference ID: UNKNOWN"
            };            
            Exception ex = new Exception();          
            multiSelectMock.Stub(x => x.GetDurRxList(durWrapperMock)).Throw(ex);  
            var rxDurReviewApiController = new RxDurReviewApiController(DurType.MULTI_SELECT);

            //Act
            var result = rxDurReviewApiController.DurRxList(durWrapperMock);

            //Assert
            Assert.AreEqual(((DURCheckResponse)result.Payload), null, "Payload not equal");
            Assert.AreEqual(ErrorContext.Message.ToString().Trim(), result.ErrorContext.Message.ToString().Trim(), "ErrorContext not equal");
        }
        
        [TestMethod]
        public void When_set_Dur_Warnings_Then_get_Dur_all_properties_match()
        {
            HttpContext.Current = MockedObjects.MockHttpContext();
            sessionMock = new StateContainer(HttpContext.Current.Session);
            var durWrapperMock = MockRepository.GenerateMock<IeRxWebAppCodeWrapper>();
            //var multiSelectMock = MockRepository.GenerateMock<IDur>();           
            mockedObjects.SetSessionVariable();
            sessionMock[SessionVariables.DbId] = mockedObjects.SetUpDBId();            
            DURCheckResponse durWarnings = mockedObjects.SetupDurWarnings();
            sessionMock[SessionVariables.DURCheckResponse] = durWarnings;
            //multiSelectMock.Stub(x => x.GetDurWarnings(durWrapperMock)).Return(durWarnings);         
            var rxDurReviewApiController =  new RxDurReviewApiController(DurType.MULTI_SELECT);
          
            //Act
            var result = rxDurReviewApiController.DurWarnings(durWrapperMock);

            //Assert
            Assert.AreEqual(((DURCheckResponse)result.Payload).Dosage.DosageCheckMedications.Count, durWarnings.Dosage.DosageCheckMedications.Count, "Count not equal");
            Assert.AreEqual(((DURCheckResponse)result.Payload).Dosage.DosageCheckMedications[0].DDI, durWarnings.Dosage.DosageCheckMedications[0].DDI, "NDCNumber not equal");

        }
        
        [TestMethod]
        public void When_session_is_set_Get_Dur_Should_Return_DurWarnings()
        {
            HttpContext.Current = MockedObjects.MockHttpContext();
            sessionMock = new StateContainer(HttpContext.Current.Session);
            var durWrapperMock = MockRepository.GenerateMock<IeRxWebAppCodeWrapper>();            
            sessionMock[SessionVariables.DURCheckResponse] = mockedObjects.SetupDurWarnings();  
            DurMultiSelect durMultiSelect = new DurMultiSelect();          
            durMultiSelect.SetPageState(sessionMock);
            mockedObjects.SetSessionVariable();           
            var rxDurReviewApiController = new RxDurReviewApiController(DurType.MULTI_SELECT);

            //Act
            var result = rxDurReviewApiController.DurWarnings(durWrapperMock);

            //Assert
            Assert.IsNotNull(((DURCheckResponse)result.Payload), "Count not equal");
            Assert.AreEqual(((DURCheckResponse)result.Payload).Dosage.DosageCheckMedications[0].DDI,184865, "NDCNumber not equal");
        }

        [TestMethod]
        public void when_session_is_empty_get_Dur_warnings_throws_exception()
        {
            var durWrapperMock = MockRepository.GenerateMock<IeRxWebAppCodeWrapper>();
            var multiSelectMock = MockRepository.GenerateMock<IDur>();
            DurMultiSelect durMultiSelect = new DurMultiSelect();
            HttpContext.Current = MockedObjects.MockHttpContext();
            sessionMock = new StateContainer(HttpContext.Current.Session);
            sessionMock[SessionVariables.DbId] = mockedObjects.SetUpDBId();

            durMultiSelect.SetPageState(sessionMock);
            ErrorContextModel ErrorContext = new ErrorContextModel()
            {
                Error = ErrorTypeEnum.ServerError,
                Message = "Reference ID: UNKNOWN"
            };
            Exception ex = new Exception();
            multiSelectMock.Stub(x => x.GetDurWarnings(durWrapperMock)).Throw(ex);
            var rxDurReviewApiController = new RxDurReviewApiController(DurType.MULTI_SELECT);

            //Act
            var result = rxDurReviewApiController.DurWarnings(durWrapperMock);

            //Assert
            Assert.AreEqual(((DURCheckResponse)result.Payload), null, "Payload not equal");
            Assert.AreEqual(ErrorContext.Message.ToString().Trim(), result.ErrorContext.Message.ToString().Trim(), "ErrorContext not equal");
        }
              
        [TestMethod]
        public void get_Dur_sets_same_session_as_passed()
        {
            DurMultiSelect durMultiSelect = new DurMultiSelect();
            HttpContext.Current = MockedObjects.MockHttpContext();
            sessionMock = new StateContainer(HttpContext.Current.Session);
            sessionMock[Constants.SessionVariables.DbId] = mockedObjects.SetUpDBId();
            durMultiSelect.SetPageState(sessionMock);
            var multiSelectMock = MockRepository.GenerateMock<IDur>();
            DURCheckResponse durWarnings = mockedObjects.SetupDurWarnings();
            multiSelectMock.Stub(x => x.SetDurWarnings(durWarnings));
            var rxDurReviewApiController =  new RxDurReviewApiController(Constants.DurType.MULTI_SELECT);   
            
            //Act
            rxDurReviewApiController.DurWarnings(durWarnings);            
            DURCheckResponse sessionDurWarnings = ((DURCheckResponse)sessionMock[Constants.SessionVariables.DURCheckResponse]);

            //Assert
            Assert.AreEqual(sessionDurWarnings, durWarnings, "Count not equal");
        }
        
        [TestMethod]
        public void When_session_is_set_Get_IgnoreDurReasonsByCategoryandUserType_Should_Return_Ignore_Reasons()
        {
            UserCategory userCategory = UserCategory.PROVIDER;
            HttpContext.Current = MockedObjects.MockHttpContext();
            sessionMock = new StateContainer(HttpContext.Current.Session);            
            var impactDurWrapperMock = MockRepository.GenerateMock<IImpactDurWraper>();
            var multiSelectMock = MockRepository.GenerateMock<IDur>();
            sessionMock[Constants.SessionVariables.UserType] = userCategory;
            DurMultiSelect durMultiSelect = new DurMultiSelect();
            sessionMock[Constants.SessionVariables.DbId] = ConnectionStringPointer.ERXDB_SERVER_1;
            List<IgnoreReasonsResponse> ignoreReasonList = mockedObjects.BuildIgnoreReasons();
            var ignoreReasonMock = MockRepository.GenerateMock<IgnoreReason>();
            impactDurWrapperMock.Stub(x => x.GetIgnoreReasons(IgnoreCategory.DUR_PAR, UserCategory.PROVIDER, ConnectionStringPointer.ERXDB_SERVER_1)).Return(mockedObjects.SetIgnoreReasonsDataTable());
            multiSelectMock.Stub(x => x.GetIgnoreReasonsByCategoryAndUserType(IgnoreCategory.DUR_PAR, impactDurWrapperMock)).Return(ignoreReasonList);
            durMultiSelect.SetPageState(sessionMock);            
            var rxDurReviewApiController = new RxDurReviewApiController(DurType.MULTI_SELECT);           

            //Act
            var result = rxDurReviewApiController.IgnoreDurReasonsByCategoryandUserType(IgnoreCategory.DUR_PAR, impactDurWrapperMock);

            //Assert
            Assert.IsNotNull(result.Payload, "Count not equal"); 
        }
        [TestMethod]
        public void When_session_is_empty_Get_IgnoreDurReasonsByCategoryandUserType_Should_Throw_Error()
        {
          
            HttpContext.Current = MockedObjects.MockHttpContext();           
            var impactDurWrapperMock = MockRepository.GenerateMock<IImpactDurWraper>();
         
            ErrorContextModel ErrorContext = new ErrorContextModel()
            {
                Error = ErrorTypeEnum.ServerError,
                Message = "Reference ID: UNKNOWN"
            };
           
            var rxDurReviewApiController = new RxDurReviewApiController(Constants.DurType.MULTI_SELECT);

            //Act
            var result = rxDurReviewApiController.IgnoreDurReasonsByCategoryandUserType(IgnoreCategory.DUR_INT, impactDurWrapperMock);

            //Assert
            Assert.AreEqual((result.Payload), null, "Payload not equal");
            Assert.AreEqual(ErrorContext.Message.ToString().Trim(), result.ErrorContext.Message.ToString().Trim(), "ErrorContext not equal");    
        }        
      
        [TestMethod]
        public void When_session_is_empty_Get_DurRedirectDetails_Should_Return_Select_Patient_Url()
        {
            HttpContext.Current = MockedObjects.MockHttpContext();   
            var rxDurReviewApiController = new RxDurReviewApiController(Constants.DurType.MULTI_SELECT);

            //Act
            var result = rxDurReviewApiController.DurRedirectDetails();

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(((DurRedirectModel)result.Payload).RedirectUrl, "Spa.aspx", "Url not equal");           
        }
        [TestMethod]
        public void When_session_Go_Next_Is_Set_Get_DurRedirectDetails_Should_Return_Set_Url()
        {            
            HttpContext.Current = MockedObjects.MockHttpContext();
            sessionMock = new StateContainer(HttpContext.Current.Session);            
            sessionMock[SessionVariables.DUR_GO_NEXT] = "someurl";          
            var rxDurReviewApiController = new RxDurReviewApiController(DurType.MULTI_SELECT);
          
            //Act
            var result = rxDurReviewApiController.DurRedirectDetails();

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(((DurRedirectModel)result.Payload).RedirectUrl, "someurl", "Url not equal");           
        }
        [TestMethod]
        public void When_session_Came_From_Is_Set_Get_DurRedirectDetails_Should_Return_Set_Url()
        {
            HttpContext.Current = MockedObjects.MockHttpContext();
            sessionMock = new StateContainer(HttpContext.Current.Session);           
            sessionMock[SessionVariables.CameFrom] = "camefromurl";            
            var rxDurReviewApiController = new RxDurReviewApiController(Constants.DurType.MULTI_SELECT);
           
            //Act
            var result = rxDurReviewApiController.DurRedirectDetails();

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(((DurRedirectModel)result.Payload).RedirectUrl, "camefromurl", "Url not equal");            
        }       
    }
}
