using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using Allscripts.ePrescribe.ExtensionMethods;
using Allscripts.ePrescribe.Objects;
using Allscripts.Impact.ePrescribeSvc;
using Allscripts.Impact.Interfaces;
using eRxWeb.ServerModel;
using ConnectionStringPointer = Allscripts.ePrescribe.DatabaseSelector.ConnectionStringPointer;

namespace eRxWeb.AppCode
{
    public class CancelRx
    {
        public static string ProcessCancelRx(
            IScriptMessage scriptMessage, 
            IEPSBroker epsBroker, 
            SendCancelRxRequestModel rx, 
            string ip, 
            string licenseId, 
            string userId, 
            string patientId, 
            int siteId,
            Guid canceledById,
            Guid canceledByProviderId,
            Guid canceledBySupervisingProviderId,
            ConnectionStringPointer dbId)
        {
            epsBroker.InsertRxHeaderCanceled(rx.RxID.ToGuid(), canceledById, canceledByProviderId, canceledBySupervisingProviderId, dbId);
            var cancelRxScriptMessageId = scriptMessage.CreateCancelRxMsg(new Guid(rx.RxID), rx.OriginalNewRxTrnsCtrlNo, licenseId, userId, siteId, dbId);
            scriptMessage.SendThisMessage(cancelRxScriptMessageId, licenseId, userId, dbId);
            epsBroker.AuditLogPatientInsert(Allscripts.ePrescribe.Objects.AuditAction.PATIENT_RX_MODIFY, SystemConfig.ePrescribeApplication.MainApplication, ePrescribeApplication.MainApplication, licenseId, userId, patientId, ip, dbId);
            return cancelRxScriptMessageId;
        }
        public static List<CancelRxEligibleScript> GetScriptsToCancel(List<string> completedScripts, List<CancelRxEligibleScript> cancelRxEligibleList)
        {
            var scriptsToCancel = new List<CancelRxEligibleScript>();

            if (cancelRxEligibleList != null && cancelRxEligibleList.Count > 0 && completedScripts != null && completedScripts.Count > 0)
            {
                // get rx id's of completed active meds and send to CancelRx popup
                foreach (CancelRxEligibleScript script in cancelRxEligibleList)
                {
                    if (completedScripts.Contains(script.RxId.ToString()) && script.OriginalNewRxTrnsCtrlNo != Guid.Empty)
                    {
                        scriptsToCancel.Add(script);
                    }
                }
            }

            return scriptsToCancel;
        }

        /// <summary>
        /// check if script is eligible to send a CancelRx message to the pharmacy
        /// script must have been sent electronically => transmission method is 'S'
        /// and pharmacy must be enrolled in cancel rx
        /// </summary>
        /// <param name="transMethod"></param>
        /// <param name="startDate">This is the date the script starts</param>
        /// <param name="controlledSubstanceCode"></param>
        /// <param name="isPharmacyCancelRxEligible"></param>
        /// <param name="originalNewRxTrnsCtrlNo"></param>
        /// <returns></returns>
        public static bool IsScriptCancelRxEligile(string transMethod, string startDate, string controlledSubstanceCode, string isPharmacyCancelRxEligible, Guid originalNewRxTrnsCtrlNo)
        {
            bool isScriptCancelRxEligible = false;

            if (originalNewRxTrnsCtrlNo == Guid.Empty) return false;

            if (transMethod.Equals("S", StringComparison.CurrentCultureIgnoreCase) &&
                isPharmacyCancelRxEligible.Equals("Y", StringComparison.CurrentCultureIgnoreCase))
            {
                if (!String.IsNullOrWhiteSpace(startDate))
                {
                    DateTime dtRxDate = DateTime.MinValue;

                    if (DateTime.TryParse(startDate, out dtRxDate))
                    {
                        if (String.IsNullOrWhiteSpace(controlledSubstanceCode))
                        {
                            // date written must be less than a year from today's date
                            if (dtRxDate > DateTime.Today.AddYears(-1))
                            {
                                isScriptCancelRxEligible = true;
                            }
                        }
                        else
                        {
                            // date written must be less than 6 months from today's date
                            if (dtRxDate > DateTime.Today.AddMonths(-6))
                            {
                                isScriptCancelRxEligible = true;
                            }
                        }
                    }
                }
            }

            return isScriptCancelRxEligible;
        }
    }
}