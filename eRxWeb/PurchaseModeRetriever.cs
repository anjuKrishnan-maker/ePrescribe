using Allscripts.ePrescribe.Common;
using eRxWeb.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eRxWeb
{
    public class PurchaseModeRetriever : BasePage
    {
        internal static void GetPurchaseMode(out string pricingStructure, out Constants.DeluxePurchaseType purchaseMode, out string orderProductName, out int orderProductCount, IStateContainer pageState)
        {
            string selectedPricingStructure = string.Empty;
            Constants.DeluxePurchaseType actionMode = Constants.DeluxePurchaseType.MODULE_NEW_PURCHASE;
            string currentDeluxeSubscription = string.Empty;

            orderProductName = string.Empty;
            orderProductCount = 0;

            if (!string.IsNullOrWhiteSpace(pageState.GetStringOrEmpty("OrderProductName")))
                orderProductName = pageState.GetStringOrEmpty("OrderProductName").ToString();

            Int32.TryParse(pageState.GetStringOrEmpty("OrderProductCount"), out orderProductCount);

            if (!string.IsNullOrWhiteSpace(pageState.GetStringOrEmpty("SelectedFeature")))
                selectedPricingStructure = pageState.GetStringOrEmpty("SelectedFeature").ToString().ToLower();

            if (!string.IsNullOrWhiteSpace(pageState.GetStringOrEmpty("CurrentDeluxeSubscription")))
                currentDeluxeSubscription = pageState.GetStringOrEmpty("CurrentDeluxeSubscription").ToString();

            //If current subscription is empty and selected subscription is not empty => new purchase 
            if ((string.IsNullOrEmpty(currentDeluxeSubscription)) && (!string.IsNullOrEmpty(selectedPricingStructure)))
            {
                actionMode = Constants.DeluxePurchaseType.MODULE_NEW_PURCHASE;
            }
            //If current subscription is not empty and selected subscription is empty
            if ((string.IsNullOrEmpty(selectedPricingStructure)) && (!string.IsNullOrEmpty(currentDeluxeSubscription)))
            {
                selectedPricingStructure = currentDeluxeSubscription;

                if (!string.IsNullOrEmpty(orderProductName))
                    actionMode = Constants.DeluxePurchaseType.PRODUCT_ONLY_PURCHASE;
                else
                    actionMode = Constants.DeluxePurchaseType.MODULE_EDIT;

            }
            //If both current and new subscription are not empty
            if ((!string.IsNullOrEmpty(selectedPricingStructure)) && (!string.IsNullOrEmpty(currentDeluxeSubscription)))
            {
                if ((currentDeluxeSubscription == selectedPricingStructure) && (!string.IsNullOrEmpty(orderProductName)))
                {
                    actionMode = Constants.DeluxePurchaseType.PRODUCT_ONLY_PURCHASE;
                }
                else
                {
                    actionMode = Constants.DeluxePurchaseType.MODULE_EDIT;
                }
                if (currentDeluxeSubscription != selectedPricingStructure)
                {
                    actionMode = Constants.DeluxePurchaseType.MODULE_CHANGE_PURCHASE;
                }
                if (String.Compare(currentDeluxeSubscription,"basic", true) == 0 && String.Compare(selectedPricingStructure, currentDeluxeSubscription, true) != 0)
                {
                    actionMode = Constants.DeluxePurchaseType.MODULE_NEW_PURCHASE;
                }
            }


           

            pricingStructure = selectedPricingStructure;
            purchaseMode = actionMode;
        }
    }
}