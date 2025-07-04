using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.DatabaseSelector;
using Allscripts.ePrescribe.ExtensionMethods;
using Allscripts.ePrescribe.Objects;
using Allscripts.ePrescribe.Shared.Logging;
using Allscripts.Impact;
using Allscripts.Impact.Interfaces;
using Allscripts.Impact.Tasks.Interfaces;
using eRxWeb.AppCode.Interfaces;
using eRxWeb.State;
using Telerik.Web.UI;
using Medication = Allscripts.Impact.Medication;
using Rx = Allscripts.Impact.Rx;
using RxUser = Allscripts.Impact.RxUser;
using ScriptMessage = Allscripts.Impact.ScriptMsg.ScriptMessage;

namespace eRxWeb.AppCode
{
    public class ApproveRefillTask: IApproveRefillTask
    {
        private static ILoggerEx logger = LoggerEx.GetLogger();
        

        public readonly static string SELECT_DENIAL_REASON = "Select a Denial Reason";

        Medication.MedType IApproveRefillTask.GetMedTypeEnum(RequestedRx requestedRx, string practiceState, string pharmState, IMedication imed,
            IPrescription iPrescription, ConnectionStringPointer dbId)
        {
            return IsCsMed(requestedRx, practiceState, pharmState, imed, iPrescription, dbId) ? Medication.MedType.CS : Medication.MedType.NON_CS;
        }

        public static bool IsCsMed(RequestedRx requestedRx, string practiceState, string pharmState, IMedication imed, IPrescription iPrescription, ConnectionStringPointer dbId)
        {
            var loadedMed = imed.LoadByNDC(requestedRx.NDCNumber, dbId);
            string ddi = String.Empty;
            string csMedCode = String.Empty;
            if (loadedMed != null && loadedMed.Tables.Count > 0 && loadedMed.Tables[0] != null && loadedMed.Tables[0].Rows != null && loadedMed.Tables[0].Rows.Count > 0)
            {
                ddi = loadedMed.Tables[0].Rows[0].GetValue("DDI");
                csMedCode = loadedMed.Tables[0].Rows[0].GetValue("ControlledSubstanceCode");
            }
            var csStateCode = iPrescription.GetStateControlledSubstanceCode(ddi, practiceState, pharmState, dbId);
            return (!String.IsNullOrWhiteSpace(csMedCode) && !csMedCode.ToUpper().Equals("U")) || (!String.IsNullOrWhiteSpace(csStateCode) && !csStateCode.ToUpper().Equals("U"));
        }


        public string GetDenyMessageTypeText(Constants.PrescriptionTaskType taskType)
        {
            switch (taskType)
            {
                case Constants.PrescriptionTaskType.RXCHG:
                case Constants.PrescriptionTaskType.RXCHG_PRIORAUTH:
                    return "Change Rx";
                case Constants.PrescriptionTaskType.REFREQ:
                case Constants.PrescriptionTaskType.RENEWAL_REQUEST:
                    return "Renewal";
                default:
                    return "Rx";
            }
        }
        
        public static string GetDenyMessage(RxTaskModel pharmacyTask, DataRow patdr, Guid licenseId, Guid userId, IApproveRefillTask iApproveRefillTask, IScriptMessage iScriptMessage, ConnectionStringPointer dbId)
        {

            var messageTypeText = iApproveRefillTask.GetDenyMessageTypeText(pharmacyTask.TaskType);
            var patientName = patdr == null ?
                iScriptMessage.GetPatientName(pharmacyTask.ScriptMessageGUID.ToGuid(), licenseId, userId, dbId) :
                patdr["PatientName"];

            var denyReason = pharmacyTask.DenialCode == "-1" ? String.Empty : $" ({pharmacyTask.DenialText})";
            return $"{messageTypeText} denied{denyReason} for {patientName}.";
        }

        public string GetDenyMessage(RxTaskModel pharmacyTask, string patientname, Guid licenseId, Guid userId, IApproveRefillTask iApproveRefillTask, IScriptMessage iScriptMessage, ConnectionStringPointer dbId)
        {

            var messageTypeText = iApproveRefillTask.GetDenyMessageTypeText(pharmacyTask.TaskType);
            var patientName = patientname ?? iScriptMessage.GetPatientName(pharmacyTask.ScriptMessageGUID.ToGuid(), licenseId, userId, dbId);

            var denyReason = pharmacyTask.DenialCode == "-1" ? String.Empty : $" ({pharmacyTask.DenialText})";
            return $"{messageTypeText} denied{denyReason} for {patientName}.";
        }

        public static PharmacyTaskRowControlValues BoxUpUiControls(GridDataItem row)
        {
            var rdbNo = row.FindControl("rdbNo") as RadioButton;
            var ddl = row.FindControl("DenialCodes") as DropDownList;
            var notesLabel = row.FindControl("notesLabel") as Label;
            var notes = row.FindControl("notesText") as TextBox;
            var divMaxCharacters = row.FindControl("divMaxCharacters") as HtmlGenericControl;
        
            return new PharmacyTaskRowControlValues
            {
                DdlDenialClientId = ddl?.ClientID,
                LblNotesClientId = notesLabel?.ClientID,
                LblNotesCountClientId = divMaxCharacters?.ClientID,
                RdbNoClientId = rdbNo?.ClientID,
                TxtPhysicianCommentsClientId = notes?.ClientID
            };
        }

        public static List<Rx> UpdateEpcsMedList(RxTaskModel task, List<Rx> epcsMedList)
        {
            epcsMedList = epcsMedList ?? new List<Rx>();
            if (task.EpcsMedList?.Count > 0)
            {
                epcsMedList.Add(task.EpcsMedList[0]);
            }
            return epcsMedList;
        }

        public bool DoesChangeRequestLinkExist(ControlCollection actionLinks)
        {
            if (actionLinks == null) return false;

            foreach (Control actionLink in actionLinks)
            {
                var actionText = (actionLink as LinkButton)?.Text;
                if (actionText == Constants.ActionButtonText.CHANGE_REQUEST || actionText == Constants.ActionButtonText.CHANGE_REQUEST_NO_BREAK) return true;
            }
            return false;
        }

        /// <summary>
        /// Returns true if there is an error with the quantity
        /// </summary>
        /// <param name="dispensedRx"></param>
        /// <param name="controls"></param>
        /// <param name="rdbApprove"></param>
        /// <param name="taskRowDataItem"></param>
        /// <param name="iApproveRefillTask"></param>
        /// <returns></returns>
        public static void VerifyDispensedQuantityAndUpdateUi(DispensedRx dispensedRx, ControlCollection controls, RadioButton rdbApprove, ITelerik taskRowDataItem, IApproveRefillTask iApproveRefillTask)
        {
            if (dispensedRx?.Quantity < 1 && iApproveRefillTask.DoesChangeRequestLinkExist(controls) && rdbApprove != null)
            {
                var lblQuantityError = taskRowDataItem.FindControl("lblQuantityError") as Label;
                if (lblQuantityError != null) lblQuantityError.Visible = true;
                rdbApprove.Enabled = false;
            }
        }
    }
}