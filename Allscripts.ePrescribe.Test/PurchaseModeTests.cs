using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Allscripts.Impact;
using eRxWeb;
using Allscripts.ePrescribe.Common;
using Rhino.Mocks;
using eRxWeb.State;

namespace Allscripts.ePrescribe.Test
{

    public class PurchaseModeTests
    {
        [TestClass]
        public class GetPurchaseModeTests
        {
            [TestMethod, TestCategory("Purchase Mode")]
            public void should_assign_module_change_purchase_when_selected_pricing_structure_and_current_deluxe_subscription_is_not_null_current_deluxe_subscription_is_not_equal_to_selected_pricing_structure()
            {
                //arrange
                var purchaseMode = Constants.DeluxePurchaseType.MODULE_NEW_PURCHASE;
                var pricingStructure = string.Empty;
                var orderProductName = string.Empty;
                var orderProductCount = 0;
                var userPropMock = MockRepository.GenerateMock<IStateContainer>();
                userPropMock.Stub(x => x.GetStringOrEmpty("OrderProductName")).Return("Hard Token");
                userPropMock.Stub(x => x.GetStringOrEmpty("OrderProductCount")).Return("4");
                userPropMock.Stub(x => x.GetStringOrEmpty("SelectedFeature")).Return("deluxeepcsepalogrx");
                userPropMock.Stub(x => x.GetStringOrEmpty("CurrentDeluxeSubscription")).Return("deluxeepcslogrx");


                //act

                PurchaseModeRetriever.GetPurchaseMode(out pricingStructure, out purchaseMode, out orderProductName, out orderProductCount, userPropMock);

                //assert
                Assert.AreEqual("deluxeepcsepalogrx", pricingStructure);
                Assert.AreEqual(Constants.DeluxePurchaseType.MODULE_CHANGE_PURCHASE, purchaseMode);
                Assert.AreEqual("Hard Token", orderProductName);
                Assert.AreEqual(4, orderProductCount);

            }
            [TestMethod, TestCategory("Purchase Mode")]
            public void should_assign_module_change_purchase_when_selected_pricingstructure_and_current_deluxe_subscription_is_not_null_and_orderProductCount_is_zero()
            {
                //arrange
                var purchaseMode = Constants.DeluxePurchaseType.MODULE_NEW_PURCHASE;
                var pricingStructure = string.Empty;
                var orderProductName = string.Empty;
                var orderProductCount = 0;
                var userPropMock = MockRepository.GenerateMock<IStateContainer>();
                userPropMock.Stub(x => x.GetStringOrEmpty("OrderProductName")).Return("");
                userPropMock.Stub(x => x.GetStringOrEmpty("OrderProductCount")).Return("0");
                userPropMock.Stub(x => x.GetStringOrEmpty("SelectedFeature")).Return("deluxeepalogrx");
                userPropMock.Stub(x => x.GetStringOrEmpty("CurrentDeluxeSubscription")).Return("deluxeepcslogrx");


                //act
                PurchaseModeRetriever.GetPurchaseMode(out pricingStructure, out purchaseMode, out orderProductName, out orderProductCount, userPropMock);

                //assert
                Assert.AreEqual("deluxeepalogrx", pricingStructure);
                Assert.AreEqual(Constants.DeluxePurchaseType.MODULE_CHANGE_PURCHASE, purchaseMode);
                Assert.AreEqual("", orderProductName);
                Assert.AreEqual(0, orderProductCount);

            }
            [TestMethod, TestCategory("Purchase Mode")]
            public void should_assign_module_change_purchase_when_selected_pricing_structure_and_current_deluxe_subscription_is_not_null_and_orderproductname_is_null()
            {
                //arrange
                var purchaseMode = Constants.DeluxePurchaseType.MODULE_NEW_PURCHASE;
                var pricingStructure = string.Empty;
                var orderProductName = string.Empty;
                var orderProductCount = 0;
                var userPropMock = MockRepository.GenerateMock<IStateContainer>();
                userPropMock.Stub(x => x.GetStringOrEmpty("OrderProductName")).Return("");
                userPropMock.Stub(x => x.GetStringOrEmpty("OrderProductCount")).Return("0");
                userPropMock.Stub(x => x.GetStringOrEmpty("SelectedFeature")).Return("deluxe");
                userPropMock.Stub(x => x.GetStringOrEmpty("CurrentDeluxeSubscription")).Return("deluxeepcslogrx");


                //act
                PurchaseModeRetriever.GetPurchaseMode(out pricingStructure, out purchaseMode, out orderProductName, out orderProductCount, userPropMock);

                //assert
                Assert.AreEqual("deluxe", pricingStructure);
                Assert.AreEqual(Constants.DeluxePurchaseType.MODULE_CHANGE_PURCHASE, purchaseMode);
                Assert.AreEqual("", orderProductName);
                Assert.AreEqual(0, orderProductCount);

            }
            [TestMethod, TestCategory("Purchase Mode")]
            public void should_assign_module_edit_when_selected_pricing_structure_and_current_deluxe_subscription_is_equal_and_not_null()
            {
                //arrange
                var purchaseMode = Constants.DeluxePurchaseType.MODULE_NEW_PURCHASE;
                var pricingStructure = string.Empty;
                var orderProductName = string.Empty;
                var orderProductCount = 0;
                var userPropMock = MockRepository.GenerateMock<IStateContainer>();
                userPropMock.Stub(x => x.GetStringOrEmpty("OrderProductName")).Return("");
                userPropMock.Stub(x => x.GetStringOrEmpty("OrderProductCount")).Return("0");
                userPropMock.Stub(x => x.GetStringOrEmpty("SelectedFeature")).Return("deluxeepcslogrx");
                userPropMock.Stub(x => x.GetStringOrEmpty("CurrentDeluxeSubscription")).Return("deluxeepcslogrx");

                //act
                PurchaseModeRetriever.GetPurchaseMode(out pricingStructure, out purchaseMode, out orderProductName, out orderProductCount, userPropMock);

                //assert
                Assert.AreEqual("deluxeepcslogrx", pricingStructure);
                Assert.AreEqual(Constants.DeluxePurchaseType.MODULE_EDIT, purchaseMode);
                Assert.AreEqual("", orderProductName);
                Assert.AreEqual(0, orderProductCount);

            }
            [TestMethod, TestCategory("Purchase Mode")]
            public void should_assign_module_change_purchase_when_selected_pricing_structure_and_current_deluxe_subscription_is_not_null_and_current_deluxe_subscription_equal_to_selected_pricing_structure_and_orderproductname_in_not_null()
            {
                //arrange);
                var purchaseMode = Constants.DeluxePurchaseType.MODULE_NEW_PURCHASE;
                var pricingStructure = string.Empty;
                var orderProductName = string.Empty;
                var orderProductCount = 0;
                var userPropMock = MockRepository.GenerateMock<IStateContainer>();
                userPropMock.Stub(x => x.GetStringOrEmpty("OrderProductName")).Return("Hard Token");
                userPropMock.Stub(x => x.GetStringOrEmpty("OrderProductCount")).Return("0");
                userPropMock.Stub(x => x.GetStringOrEmpty("SelectedFeature")).Return("deluxeepcslogrx");
                userPropMock.Stub(x => x.GetStringOrEmpty("CurrentDeluxeSubscription")).Return("deluxeepcslogrx");


                //act
                PurchaseModeRetriever.GetPurchaseMode(out pricingStructure, out purchaseMode, out orderProductName, out orderProductCount, userPropMock);

                //assert
                Assert.AreEqual("deluxeepcslogrx", pricingStructure);
                Assert.AreEqual(Constants.DeluxePurchaseType.PRODUCT_ONLY_PURCHASE, purchaseMode);
                Assert.AreEqual("Hard Token", orderProductName);
                Assert.AreEqual(0, orderProductCount);

            }

            [TestMethod, TestCategory("Purchase Mode")]
            public void should_assign_module_change_purchase_when_selected_pricing_structure_and_current_deluxe_subscription_is_not_null_and_orderproductname_and_orderproductcount_is_not_null_and_currentdeluxesubscription_is_not_basic()
            {
                //arrange
                var purchaseMode = Constants.DeluxePurchaseType.MODULE_NEW_PURCHASE;
                var pricingStructure = string.Empty;
                var orderProductName = string.Empty;
                var orderProductCount = 0;
                var userPropMock = MockRepository.GenerateMock<IStateContainer>();
                userPropMock.Stub(x => x.GetStringOrEmpty("OrderProductName")).Return("Hard Token");
                userPropMock.Stub(x => x.GetStringOrEmpty("OrderProductCount")).Return("4");
                userPropMock.Stub(x => x.GetStringOrEmpty("SelectedFeature")).Return("deluxeepcslogrx");
                userPropMock.Stub(x => x.GetStringOrEmpty("CurrentDeluxeSubscription")).Return("deluxeepa");


                //act
                PurchaseModeRetriever.GetPurchaseMode(out pricingStructure, out purchaseMode, out orderProductName, out orderProductCount, userPropMock);

                //assert
                Assert.AreEqual("deluxeepcslogrx", pricingStructure);
                Assert.AreEqual(Constants.DeluxePurchaseType.MODULE_CHANGE_PURCHASE, purchaseMode);
                Assert.AreEqual("Hard Token", orderProductName);
                Assert.AreEqual(4, orderProductCount);
            }

            [TestMethod, TestCategory("Purchase Mode")]
            public void should_assign_module_new_purchase_when_selected_pricing_structure_and_current_deluxe_subscription_is_not_null_and_not_equal_and_currentdeluxesubscription_is_basic()
            {
                //arrange
                var purchaseMode = Constants.DeluxePurchaseType.MODULE_NEW_PURCHASE;
                var pricingStructure = string.Empty;
                var orderProductName = string.Empty;
                var orderProductCount = 0;
                var userPropMock = MockRepository.GenerateMock<IStateContainer>();
                userPropMock.Stub(x => x.GetStringOrEmpty("OrderProductName")).Return("");
                userPropMock.Stub(x => x.GetStringOrEmpty("OrderProductCount")).Return("0");
                userPropMock.Stub(x => x.GetStringOrEmpty("SelectedFeature")).Return("deluxeepcslogrx");
                userPropMock.Stub(x => x.GetStringOrEmpty("CurrentDeluxeSubscription")).Return("basic");

                //act
                PurchaseModeRetriever.GetPurchaseMode(out pricingStructure, out purchaseMode, out orderProductName, out orderProductCount, userPropMock);

                //assert
                Assert.AreEqual("deluxeepcslogrx", pricingStructure);
                Assert.AreEqual(Constants.DeluxePurchaseType.MODULE_NEW_PURCHASE, purchaseMode);
                Assert.AreEqual("", orderProductName);
                Assert.AreEqual(0, orderProductCount);
            }

            [TestMethod, TestCategory("Purchase Mode")]
            public void should_assign_module_new_purchase_when_selected_pricing_structure_is_not_null_and_current_deluxe_subscription_is_null()
            {
                //arrange
                var purchaseMode = Constants.DeluxePurchaseType.MODULE_NEW_PURCHASE;
                var pricingStructure = string.Empty;
                var orderProductName = string.Empty;
                var orderProductCount = 0;
                var userPropMock = MockRepository.GenerateMock<IStateContainer>();
                userPropMock.Stub(x => x.GetStringOrEmpty("OrderProductName")).Return("Hard Token");
                userPropMock.Stub(x => x.GetStringOrEmpty("OrderProductCount")).Return("4");
                userPropMock.Stub(x => x.GetStringOrEmpty("SelectedFeature")).Return("deluxeepcslogrx");
                userPropMock.Stub(x => x.GetStringOrEmpty("CurrentDeluxeSubscription")).Return("");


                //act
                PurchaseModeRetriever.GetPurchaseMode(out pricingStructure, out purchaseMode, out orderProductName, out orderProductCount, userPropMock);

                //assert
                Assert.AreEqual("deluxeepcslogrx", pricingStructure);
                Assert.AreEqual(Constants.DeluxePurchaseType.MODULE_NEW_PURCHASE, purchaseMode);
                Assert.AreEqual("Hard Token", orderProductName);
                Assert.AreEqual(4, orderProductCount);

            }
            private IStateContainer arrangement(string OrderProductName, string OrderProductCount, string SelectedFeature, string CurrentDeluxeSubscription)
            {

                var userPropMock = MockRepository.GenerateMock<IStateContainer>();
                userPropMock.Stub(x => x.GetStringOrEmpty("OrderProductName")).Return(OrderProductName);
                userPropMock.Stub(x => x.GetStringOrEmpty("OrderProductCount")).Return(OrderProductCount);
                userPropMock.Stub(x => x.GetStringOrEmpty("SelectedFeature")).Return(SelectedFeature);
                userPropMock.Stub(x => x.GetStringOrEmpty("CurrentDeluxeSubscription")).Return(CurrentDeluxeSubscription);

                return userPropMock;

            }
        }
    }
}

