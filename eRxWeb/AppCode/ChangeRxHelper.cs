using Allscripts.ePrescribe.Common;
using Allscripts.Impact;
using Allscripts.Impact.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eRxWeb.AppCode
{
    public class ChangeRxHelper
    {
        public static string GetApprovalMessageAndRouteChangeRxWorkflow(RxTaskModel changeRxWorkflow, IScriptMessage iScriptMessage, bool IsPOBUser, string SupervisorProviderID, string PatientName, string authToken, ref  string rxID)
        {
            string message = string.Empty;
            var rx = changeRxWorkflow.Rx as Rx;
            if ((rx != null) && (iScriptMessage != null))
            {
                if (IsPOBUser || changeRxWorkflow.UserType == Constants.UserCategory.PHYSICIAN_ASSISTANT_SUPERVISED)
                {
                    string delegateProviderID = string.Empty;
                    if (!string.IsNullOrEmpty(SupervisorProviderID))
                    {
                        // get the correct delegate provider for ChangeRx request for PA with supervision when refill request is being processed by a POB user
                        delegateProviderID = SupervisorProviderID;
                    }
                    else
                    {
                        // get the correct delegate provider for ChangeRx request for PA with supervision when refill request is being processed by a POB user
                        delegateProviderID = (!string.IsNullOrEmpty(changeRxWorkflow.DelegateProviderId.ToString())) ? changeRxWorkflow.DelegateProviderId.ToString() : changeRxWorkflow.UserId.ToString();
                    }
                    rxID = iScriptMessage.ApproveRxChangeMessage(changeRxWorkflow, delegateProviderID, Constants.PrescriptionTransmissionMethod.SENT);
                }
                else
                {
                    rxID = iScriptMessage.ApproveRxChangeMessage(changeRxWorkflow, changeRxWorkflow.UserId.ToString(), Constants.PrescriptionTransmissionMethod.SENT);
                }
                if (!string.IsNullOrEmpty(rx.MedicationName))//custom med --> empty medname (wrong NDC case)
                {
                    message = "Rx Change for " + rx.MedicationName + " approved for " + PatientName.Trim() + ".";
                }
                else
                {
                    message = "Rx Change approved for " + PatientName.Trim() + ".";
                }
            }
            return message;
        }
    }
}