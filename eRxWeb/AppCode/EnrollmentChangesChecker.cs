using Allscripts.ePrescribe.Common;
using eRxWeb.State;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;
using Allscripts.ePrescribe.DatabaseSelector;
using Allscripts.ePrescribe.ExtensionMethods;
using Allscripts.ePrescribe.Objects;
using Allscripts.ePrescribe.Shared.Logging;
using Allscripts.Impact;
using Allscripts.Impact.Interfaces;
using eRxWeb.AppCode.Interfaces;

namespace eRxWeb.AppCode
{
    public class EnrollmentChangesChecker:BasePage
    {        
        public static bool AnyEnrollmentChangesOccured(Constants.DeluxePurchaseType purchaseMode, IStateContainer pageState)
        {
            
            string selectedPricingStructure = string.Empty;
            string currentDeluxeSubscription = string.Empty;           
            selectedPricingStructure = pageState.GetStringOrEmpty("SelectedFeature").ToString().ToLower();
            currentDeluxeSubscription = pageState.GetStringOrEmpty("CurrentDeluxeSubscription").ToString();

            if ((purchaseMode == Constants.DeluxePurchaseType.MODULE_NEW_PURCHASE) && (selectedPricingStructure.Contains("epa")))
                return true;

            if ((purchaseMode == Constants.DeluxePurchaseType.MODULE_CHANGE_PURCHASE) && (((currentDeluxeSubscription.Contains("epa")) && (!(selectedPricingStructure.Contains("epa")))) || ((selectedPricingStructure.Contains("epa")) && (!(currentDeluxeSubscription.Contains("epa"))))))
                return true;

            return false;
        }

       
    }
}
