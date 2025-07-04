using Allscripts.ePrescribe.Common;
using Allscripts.Impact;
using eRxWeb;
using eRxWeb.AppCode;
using eRxWeb.State;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;

namespace Allscripts.ePrescribe.Test
{
    public class DeluxeBillingPageTests
    { 
        [TestClass]
        public class AnyEnrollmentChangesOccuredTests
        {           

            [TestMethod, TestCategory("Enrollment Changes")]
            public void should_enroll_when_purchase_mode_is_module_change_purchase_and_selected_pricing_structure_contains_epa()
            {
                //arrange                
                EnrollmentChangesChecker enrollmentChanges = new EnrollmentChangesChecker();                                                       
                var purchaseMode = Constants.DeluxePurchaseType.MODULE_CHANGE_PURCHASE;
                var selectedfeature = "deluxeepcsepalogrx";
                var currentDeluxesubscription = "deluxeepcslogrx";

                var userPropMock = MockRepository.GenerateMock<IStateContainer>();                
                userPropMock.Stub(x => x.GetStringOrEmpty("SelectedFeature")).Return(selectedfeature);
                userPropMock.Stub(x => x.GetStringOrEmpty("CurrentDeluxeSubscription")).Return(currentDeluxesubscription);
                

                enrollmentChanges.PageState = userPropMock;

                //act
                bool enroll = EnrollmentChangesChecker.AnyEnrollmentChangesOccured(purchaseMode,enrollmentChanges.PageState);

                //assert
                Assert.IsTrue(enroll);

            }

            [TestMethod, TestCategory("Enrollment Changes")]
            public void should_enroll_when_purchase_mode_is_module_new_purchase_and_selected_pricing_structure_contains_epa()
            {
                //arrange                
                EnrollmentChangesChecker enrollmentChanges = new EnrollmentChangesChecker();
                var purchaseMode = Constants.DeluxePurchaseType.MODULE_NEW_PURCHASE;
                var selectedfeature = "deluxeepcsepalogrx";
                var currentDeluxesubscription = "deluxeepcslogrx";

                var userPropMock = MockRepository.GenerateMock<IStateContainer>();
                userPropMock.Stub(x => x.GetStringOrEmpty("SelectedFeature")).Return(selectedfeature);
                userPropMock.Stub(x => x.GetStringOrEmpty("CurrentDeluxeSubscription")).Return(currentDeluxesubscription);


                enrollmentChanges.PageState = userPropMock;
                

                //act      
                bool enroll = EnrollmentChangesChecker.AnyEnrollmentChangesOccured(purchaseMode, enrollmentChanges.PageState);

                //assert
                Assert.IsTrue(enroll);

            }

            [TestMethod, TestCategory("Enrollment Changes")]
            public void should_enroll_when_purchase_mode_is_module_new_purchase_and_selected_pricing_structure_does_not_contains_epa()
            {
                //arrange
                EnrollmentChangesChecker enrollmentChanges = new EnrollmentChangesChecker();
                var purchaseMode = Constants.DeluxePurchaseType.MODULE_NEW_PURCHASE;
                var selectedfeature = "deluxe";
                var currentDeluxesubscription = "deluxeepcslogrx";

                var userPropMock = MockRepository.GenerateMock<IStateContainer>();
                userPropMock.Stub(x => x.GetStringOrEmpty("SelectedFeature")).Return(selectedfeature);
                userPropMock.Stub(x => x.GetStringOrEmpty("CurrentDeluxeSubscription")).Return(currentDeluxesubscription);


                enrollmentChanges.PageState = userPropMock;
                                

                //act            
                bool enroll = EnrollmentChangesChecker.AnyEnrollmentChangesOccured(purchaseMode, enrollmentChanges.PageState);

                //assert
                Assert.IsFalse(enroll);

            }

            [TestMethod, TestCategory("Enrollment Changes")]
            public void should_enroll_when_purchase_mode_is_module_new_purchase_and_current_deluxe_subscription_contains_epa()
            {
                //arrange
                EnrollmentChangesChecker enrollmentChanges = new EnrollmentChangesChecker();
                var purchaseMode = Constants.DeluxePurchaseType.MODULE_CHANGE_PURCHASE;
                var selectedfeature = "deluxeepcslogrx";
                var currentDeluxesubscription = "deluxeepcsepalogrx";

                var userPropMock = MockRepository.GenerateMock<IStateContainer>();
                userPropMock.Stub(x => x.GetStringOrEmpty("SelectedFeature")).Return(selectedfeature);
                userPropMock.Stub(x => x.GetStringOrEmpty("CurrentDeluxeSubscription")).Return(currentDeluxesubscription);


                enrollmentChanges.PageState = userPropMock;
                
                //act            
                bool enroll = EnrollmentChangesChecker.AnyEnrollmentChangesOccured(purchaseMode, enrollmentChanges.PageState);

                //assert
                Assert.IsTrue(enroll);

            }
           
        }
    }
}
