using Allscripts.ePrescribe.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace eRxWeb.AppCode
{
    public class DeluxeAccountUtil
    {
        public static void ConstructCancelReasons(Constants.DeluxePricingStructure type, RadioButtonList cancelReason)
        {
            if (type == Constants.DeluxePricingStructure.CompulsoryBasic)
            {
                cancelReason.Items.AddRange(new ListItem[] { new ListItem("I liked the Basic features but it is too expensive", "1"),
                                            new ListItem("I didn't need or use the Basic features", "2") });
            }
            else
            {
                cancelReason.Items.AddRange(new ListItem[] { new ListItem("I liked the Deluxe features but it is too expensive", "1"),
                                            new ListItem("I didn't need or use the Deluxe features", "2") });
            }
            cancelReason.Items.AddRange(new ListItem[] { new ListItem("I had a support issue or question that was never answered", "3"),
                                        new ListItem("I am using another application", "4"),
                                        new ListItem("Other reason: please specify below", "5") });
        }

        /// <summary>
        /// Check if epcs setup fee should be charged. Epcs setup fee is charged only if new subscription is epcs and the previous subscription isn't
        /// </summary>
        /// <param name="previousSubscription"></param>
        /// <param name="newSubscription"></param>
        /// <returns></returns>
        public static bool IsEpcsSetupFeesRequired(string previousSubscription, string newSubscription)
        {
            bool shouldChargeSetupFees = false;

            if(newSubscription.ToLower().Contains("epcs") && !previousSubscription.ToLower().Contains("epcs"))
            {
                shouldChargeSetupFees = true;
            }

            return shouldChargeSetupFees;
        }
    }
}