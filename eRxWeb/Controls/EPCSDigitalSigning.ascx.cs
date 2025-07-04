using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using Allscripts.ePrescribe.Shared.Logging;
using Allscripts.Impact;
using Allscripts.ePrescribe.Common;
using Telerik.Web.UI;
using System.Text.RegularExpressions;
using System.Reflection;
using Allscripts.ePrescribe.ExtensionMethods;
using Allscripts.ePrescribe.Objects.PPTPlus;
using eRxWeb.Controls.Interfaces;
using Allscripts.Impact.Utilities;
using eRxWeb.AppCode.PptPlusBPL;
using eRxWeb.ePrescribeSvc;
using eRxWeb.State;
using ConfigurationManager = System.Configuration.ConfigurationManager;
using Rx = Allscripts.Impact.Rx;
using RxUser = Allscripts.Impact.RxUser;
using RxTaskModel = Allscripts.Impact.RxTaskModel;
using AMEMOMMessageInfo = Allscripts.Impact.AMEMOMMessageInfo;
using Allscripts.Impact.ScriptMsg;

namespace eRxWeb
{
    public partial class Controls_EPCSDigitalSigning : BaseControl, IControls_EPCSDigitalSigning
    {
        public delegate void DigitalSigningCompleteHandeler(DigitalSigningEventArgs DSEventArgs);

        private static readonly ILoggerEx logger = LoggerEx.GetLogger();
        //
        // event declaration - this will be used to close the user control and rasie event to be consumed by parent page
        //
        public event DigitalSigningCompleteHandeler OnDigitalSigningComplete;

        public static string dateFormat = "yyyy-MM-dd HH:mm:ss.fff";

        bool _showEffectiveDateColumn = false;
        bool _showPharmacyInGrid = false;
        private Dictionary<string, string> _auditLogPatientID = new Dictionary<string, string>();
        private string _errorMessage = "Controlled substance meds being sent electronically could not be digitally signed.  Please try again or print.";
        private bool _hasPharmacyChanges = false;
        #region Properties

        public RxTaskModel PharmacyTask
        {
            get
            {
                return (RxTaskModel)Session["RxTaskModel"];
            }
            set
            {
                Session["RxTaskModel"] = value;
            }
        }
        public bool IsCSRefReqWorkflow
        {
            get
            {
                return ControlState.GetBooleanOrFalse(Constants.SessionVariables.IsCsRefReqWorkflow);
            }
        }
        public bool IsCsChangeRxWorkflow => Session[Constants.SessionVariables.ChangeRxRequestedMedCs] != null;

        public bool IsScriptForNewRx
        {
            get
            {
                if (ViewState["IsScriptForNewRx"] == null)
                {
                    ViewState["IsScriptForNewRx"] = true;
                }

                return (bool)ViewState["IsScriptForNewRx"];
            }
            set
            {
                ViewState["IsScriptForNewRx"] = value;
            }
        }

        public bool IsRenewalApprovalWorkflow
        {
            get
            {
                if (PharmacyTask == null)
                {
                    return Convert.ToBoolean(ViewState["IsRenewalApprovalWorkflow"]);
                }

                return PharmacyTask.TaskType == Constants.PrescriptionTaskType.REFREQ;
            }
        }

        public bool IsApprovalRequestWorkflow
        {
            get
            {
                if (ViewState["IsApprovalRequestWorkflow"] == null)
                {
                    ViewState["IsApprovalRequestWorkflow"] = false;
                }

                return (bool)ViewState["IsApprovalRequestWorkflow"];
            }
            set
            {
                ViewState["IsApprovalRequestWorkflow"] = value;
            }
        }

        public List<Rx> EPCSMEDList
        {
            get
            {
                return (List<Rx>)ViewState["EPCSMEDList"];
            }
            set
            {
                ViewState["EPCSMEDList"] = value;
            }
        }

        /// <summary>
        /// Contains a key/value pair of the newly created REFRES ScriptMessageID (the key) and it's corresponding original REFREQ ScriptMessageID (the value)
        /// </summary>
        public Dictionary<string, string> RefReqCrossReference
        {
            get
            {
                if (ViewState["RefReqCrossReference"] == null)
                {
                    ViewState["RefReqCrossReference"] = new Dictionary<string, string>();
                }

                return (Dictionary<string, string>)ViewState["RefReqCrossReference"];
            }
            set
            {
                ViewState["RefReqCrossReference"] = value;
            }
        }

        public string OtpFormTransactionID
        {
            get
            {
                return ViewState["OtpFormTransactionID"].ToString();
            }
            set
            {
                ViewState["OtpFormTransactionID"] = value;
            }
        }

        public string OtpIdentityName
        {
            get
            {
                return ViewState["OtpIdentityName"].ToString();
            }
            set
            {
                ViewState["OtpIdentityName"] = value;
            }
        }

        public OTPForm[] OtpFormList
        {
            get
            {
                return (OTPForm[])ViewState["OtpFormList"];
            }
            set
            {
                ViewState["OtpFormList"] = value;
            }
        }

        public bool ShouldShowOtpForm
        {
            get
            {
                bool shouldShow = true;

                if (ViewState["ShouldShowOtpForm"] != null)
                {
                    shouldShow = bool.Parse(ViewState["ShouldShowOtpForm"].ToString());
                }

                return shouldShow;
            }
            set
            {
                ViewState["ShouldShowOtpForm"] = value;
            }
        }

        public OTPForm OtpForm
        {
            get
            {
                return (OTPForm)ViewState["OtpForm"];
            }
            set
            {
                ViewState["OtpForm"] = value;
            }
        }

        private ePrescribeSvc.DEALicense[] _deaList
        {
            get
            {
                return (ePrescribeSvc.DEALicense[])ViewState["DEALIST"];
            }
            set
            {
                ViewState["DEALIST"] = value;
            }
        }

        private int AuthRetryCount
        {
            get
            {
                if (ViewState["AuthRetryCount"] == null)
                {
                    ViewState["AuthRetryCount"] = 0;
                }

                return Convert.ToInt32(ViewState["AuthRetryCount"]);
            }
            set
            {
                ViewState["AuthRetryCount"] = value;
            }
        }

        public Dictionary<string, string> AuditLogPatientID
        {
            get
            {
                return _auditLogPatientID;
            }
        }

        #endregion

        #region Page Events and Event Handlers

        protected void Page_Load(object sender, EventArgs e)
        {
            logger.Debug("Loaded EPCSDigitalSigning");
        }

        protected void btnSavePatientDemo_Click(object sender, EventArgs e)
        {

            try
            {
                string dateOfBirth = ((DateTime)tbDOB.SelectedDate).ToString("MM/dd/yyyy");

                // update patient data
                DataSet dsPatient;
                dsPatient = Allscripts.Impact.CHPatient.PatientSearchById(base.SessionPatientID, base.SessionLicenseID, base.SessionUserID, base.DBID);
                DataTable dtPatValue = dsPatient.Tables["Patient"];
                DataRow drPat = dtPatValue.Rows[0];

                EPSBroker.SavePatient(base.SessionLicenseID, base.SessionUserID, base.SessionPatientID, drPat["ChartID"].ToString(),
                        tbLastName.Text, tbFirstName.Text, drPat["MiddleInitial"].ToString(), tbAddress.Text, tbAddress2.Text, tbCity.Text, ddlState.SelectedValue,
                        tbZip.Text, drPat["Phone"].ToString(), dateOfBirth, string.Empty, ddlGender.SelectedValue, Convert.ToInt32(drPat["StatusID"]),
                        string.Empty, string.Empty, string.Empty, drPat["Email"].ToString(), drPat["PaternalName"].ToString(), drPat["MaternalName"].ToString(),
                        null, null, drPat["MobilePhone"].ToString(), null, string.Empty, Convert.ToString(drPat["WeightKg"]), Convert.ToString(drPat["HeightCm"]), null, base.DBID);

                ControlState["PATIENTFIRSTNAME"] = tbFirstName.Text;
                ControlState["PATIENTLASTNAME"] = tbLastName.Text;
                ControlState["SEX"] = ddlGender.SelectedValue;
                ControlState["PATIENTDOB"] = dateOfBirth;
                ControlState["PATIENTADDRESS1"] = tbAddress.Text;
                ControlState["PATIENTADDRESS2"] = tbAddress2.Text;
                ControlState["PATIENTCITY"] = tbCity.Text;
                ControlState["PATIENTSTATE"] = ddlState.SelectedValue;
                ControlState["PATIENTZIP"] = tbZip.Text;

                this.ShouldShowEpcsSignAndSendScreen();
            }
            catch (Exception ex)
            {
                string exceptionID = Audit.AddException(base.SessionUserID, base.SessionLicenseID, "Error updating Patient via EPCS popup: " + ex.ToString(), string.Empty, string.Empty, string.Empty, base.DBID);

                DigitalSigningEventArgs eventArgs = new DigitalSigningEventArgs(false);
                eventArgs.Message = "Patient information could not be updated at this time.  Please contact support with Exception ID: " + exceptionID;

                if (OnDigitalSigningComplete != null)
                {
                    OnDigitalSigningComplete(eventArgs);
                }
            }

        }

        protected void btnCancelFromPatientDemo_Click(object sender, EventArgs e)
        {
            // just return to script pad page
            this.ShouldShowOtpForm = true;
            this.OtpFormList = null;
            //this.btnSubmitEPCS.Enabled = false;
            mpeEPCS.Hide();
        }

        protected void btnEpcsEnterpriseEditPatientOffOk_Click(object sender, EventArgs e)
        {
            DigitalSigningEventArgs eventArgs = new DigitalSigningEventArgs(false);
            eventArgs.IsAddressCityMissing = true;

            if (OnDigitalSigningComplete != null)
            {
                OnDigitalSigningComplete(eventArgs);
            }
        }

        protected void grdEPCSMedList_ItemDataBound(object sender, Telerik.Web.UI.GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                GridDataItem tempDataItem = (GridDataItem)e.Item;
                Rx epcsMed = ((Rx)e.Item.DataItem);


                var checkBox = ((CheckBox) tempDataItem["ClientSelectColumn"].Controls[0]);

                var picker = (RadDatePicker)tempDataItem.FindControl("radDatePickerEffectiveDate");

                checkBox.Attributes.Add("onClick",$"disableEffectiveDate('{checkBox.ClientID}','{picker.ClientID}')");


                Literal litMedicationAndSig = tempDataItem.FindControl("litMedicationAndSig") as Literal;
                if (litMedicationAndSig != null)
                {
                    StringBuilder medicationAndSig = new StringBuilder();
                    if (epcsMed.IsBrandNameMed)
                    {
                        medicationAndSig.Append("<span style=\"font-weight: bold\">").Append(epcsMed.MedicationName).Append("</span>");
                    }
                    else
                    {
                        medicationAndSig.Append(epcsMed.MedicationName);
                    }

                    string sigText = epcsMed.GetMDDAppendedSig();
                    medicationAndSig.Append(" ").Append(epcsMed.Strength).Append(" ").Append(epcsMed.StrengthUOM).Append(" ").Append(epcsMed.DosageFormDescription).Append(" ").Append(sigText);

                    if (!string.IsNullOrWhiteSpace(epcsMed.Notes))
                    {
                        medicationAndSig.Append("  ").Append("<span style=\"font-weight: bold\">Pharmacy Notes: </span>").Append(epcsMed.Notes);
                    }

                    if (_showPharmacyInGrid)
                    {
                        if (epcsMed.Destination == "PHARM")
                        {
                            var lastPharmacyName = epcsMed.PharmacyName != null ? epcsMed.PharmacyName : ControlState.GetStringOrEmpty("LASTPHARMACYNAME");
                            if (lastPharmacyName != string.Empty)
                            {
                                medicationAndSig.Append("<br/>").Append("<span style=\"font-weight: bold\">Pharmacy: </span>").Append(lastPharmacyName);
                            }
                        }
                        else if (epcsMed.Destination == "MOB")
                        {
                            var mob_Name = ControlState.GetStringOrEmpty("MOB_Name");
                            if (mob_Name != string.Empty)
                            {
                                medicationAndSig.Append("<br/>").Append("<span style=\"font-weight: bold\">Pharmacy: </span>").Append(mob_Name);
                            }
                        }
                    }

                    litMedicationAndSig.Text = medicationAndSig.ToString();
                }

                RadDatePicker effectiveDate = tempDataItem.FindControl("radDatePickerEffectiveDate") as RadDatePicker;
                if (effectiveDate != null)
                {
                    if (epcsMed.CanEditEffectiveDate)
                    {
                        effectiveDate.Enabled = true;
                        effectiveDate.MinDate = DateTime.Now;
                        _showEffectiveDateColumn = true;
                    }
                    else
                    {
                        effectiveDate.Visible = false;
                    }
                }
            }
        }

        protected void btnCancelEPCS_Click(object sender, EventArgs e)
        {
            // just return to script pad page
            this.ShouldShowOtpForm = true;
            this.OtpForm = null;
            this.OtpFormList = null;
            //this.btnSubmitEPCS.Enabled = false;
            mpeEPCS.Hide();
        }


        protected void btnSubmitEPCS_Click(object sender, EventArgs e)
        {
            using (var timer = logger.StartTimer("btnSubmitEPCS_Click"))
            {
                //
                // re-authenticate Shield user, get users EPCS authorization status, and validate users 2nd factor authentication form type
                //
                this.ShouldShowOtpForm = true;
                this.OtpFormList = null;
                this.RefReqCrossReference = null;

                bool isAuditSuccess = true;
                bool isMismatch = false;
                Dictionary<string, long> rxIDAudit = new Dictionary<string, long>();
                if (this.IsCSRefReqWorkflow || IsCsChangeRxWorkflow)
                    this.IsScriptForNewRx = false;

                ///This cachedAudit is only used in case of REFREQ workflow. String key is ScriptMessageID. 
                Dictionary<string, List<ePrescribeSvc.AuditPatientRxCSInsertRequest>> cachedAudit
                        = new Dictionary<string, List<ePrescribeSvc.AuditPatientRxCSInsertRequest>>();

                ///stores rxid to script Message ID mapping for REFREQ workflow.
                Dictionary<string, string> selectedMeds = new Dictionary<string, string>();
                ///This will contain mapping from script message id to RxId. Script message ID here will be same as we have in cachedAudit.
                Dictionary<string, string> scriptMessageID2RxId = new Dictionary<string, string>();

                DigitalSigningEventArgs eventArgs = new DigitalSigningEventArgs();

                try
                {
                    foreach (Rx epcsMed in EPCSMEDList)
                    {
                        string rxID = epcsMed.RxID;

                        if (this.IsScriptForNewRx || this.IsApprovalRequestWorkflow || this.IsCSRefReqWorkflow || IsCsChangeRxWorkflow)
                        {
                            ePrescribeSvc.AuditLogPatientRxResponse rxResponse = base.AuditLogPatientRxInsert(ePrescribeSvc.AuditAction.PATIENT_RX_CREATED, base.SessionPatientID, rxID, DateTime.UtcNow.ToString(dateFormat));
                            rxIDAudit.Add(rxID, rxResponse.AuditLogPatientRxID);
                            //here also get AuditLogPatientID and assign it to _auditLogPatientID.
                            _auditLogPatientID.Add(rxID, rxResponse.AuditLogPatientID);
                            if (rxResponse.Success)
                            {
                                ePrescribeSvc.AuditLogPatientRxCSResponse rxCsResp = base.AuditLogPatientRxCSInsert(ePrescribeSvc.AuditAction.PATIENT_RX_CS_READY_TO_SIGN, base.SessionPatientID, rxResponse.AuditLogPatientRxID, true, DateTime.UtcNow.ToString(dateFormat));
                                if (rxCsResp.Success)
                                {
                                    ControlState["AuditCreatedRxSet"] = true;
                                }
                                else
                                {
                                    isAuditSuccess = false;
                                    break;
                                }
                            }
                            else
                            {
                                isAuditSuccess = false;
                                break;
                            }
                        }
                        else
                        {
                            List<ePrescribeSvc.AuditPatientRxCSInsertRequest> list = new List<ePrescribeSvc.AuditPatientRxCSInsertRequest>();
                            ePrescribeSvc.AuditPatientRxCSInsertRequest cReq = new ePrescribeSvc.AuditPatientRxCSInsertRequest();
                            cReq.AuditAction = ePrescribeSvc.AuditAction.PATIENT_RX_CS_READY_TO_SIGN;
                            cReq.IsSuccessful = true;
                            cReq.CreatedUTC = DateTime.UtcNow.ToString();
                            list.Add(cReq);
                            if (!string.IsNullOrEmpty(epcsMed.ScriptMessageID))
                            {
                                cachedAudit.Add(epcsMed.ScriptMessageID, list);
                            }
                            else
                            {
                                cachedAudit.Add(ControlState.GetStringOrEmpty(Constants.SessionVariables.TaskScriptMessageId), list);
                            }
                        }
                    }


                    List<Rx> rxList = new List<Rx>();

                    if (this.IsScriptForNewRx || this.IsApprovalRequestWorkflow || this.IsCSRefReqWorkflow || IsCsChangeRxWorkflow)
                    {
                        if (this.IsScriptForNewRx)
                        {
                            DataTable medFromDB = Allscripts.ePrescribe.Data.Prescription.GetScriptPad(
                            base.SessionLicenseID,
                            base.SessionSiteID,
                            base.SessionUserID,
                            base.SessionPatientID,
                            ControlState.GetStringOrEmpty("PracticeState"),
                            Constants.CommonAbbreviations.NO,
                            DBID);

                            rxList = new List<Rx>(medFromDB.Rows.Count);

                            foreach (DataRow dr in medFromDB.Rows)
                            {
                                Rx rx = new Rx(dr);
                                rxList.Add(rx);
                            }
                        }
                        else
                        {
                            if (rxList.Count == 0)
                            {
                                //This is approval request workflow.
                                foreach (Rx epcsMed in EPCSMEDList)
                                {
                                    rxList.Add(epcsMed);
                                }
                            }
                        }

                        if (rxList.Count > 0)
                        {
                            foreach (Rx epcsMed in EPCSMEDList)
                            {
                                Rx originalMed = rxList.Find(x => x.RxID.Equals(epcsMed.RxID));

                                string retVal = isMismatchMed(epcsMed, originalMed);
                                if (!string.IsNullOrEmpty(retVal))
                                {
                                    isMismatch = true;
                                    //Allscripts.ePrescribe.Data.Prescription.Delete(originalMed.RxID, DBID);
                                    ePrescribeSvc.AuditLogPatientRxCSResponse rxCsResp = base.AuditLogPatientRxCSInsert(ePrescribeSvc.AuditAction.PATIENT_RX_CS_SIGN_EPCS_RX, base.SessionPatientID, rxIDAudit[epcsMed.RxID], false, DateTime.UtcNow.ToString());
                                    Allscripts.Impact.Audit.AddException(base.SessionUserID, base.SessionLicenseID, "MessageID: " + epcsMed.RxID + " " + string.Format("Rxid {0} details has been changed for {1}", epcsMed.RxID, retVal), null, null, null, DBID);
                                    break;
                                }
                            }

                            if (isMismatch)
                            {
                                foreach (Rx med in rxList)
                                {
                                    Allscripts.ePrescribe.Data.Prescription.Delete(med.RxID, DBID);
                                }
                            }
                        }
                        else
                        {
                            //impossible case.
                        }
                    }

                    var shieldCorrelationId = Guid.NewGuid();

                    if (isAuditSuccess && !isMismatch)
                    {
                        if (OtpForm != null)
                        {
                            var otpAuthResponse = EPSBroker.AuthenicateOTPForSigning(OtpFormTransactionID, txtOTP.Text, OtpIdentityName, OtpForm,
                                litUserName.Text, txtPassword.Text, SessionLicenseID, SessionUserID, Request.UserIpAddress(), EprescribeExternalAppInstanceID, shieldCorrelationId);

                            if (otpAuthResponse.Success)
                            {
                                if (IsScriptForNewRx || IsApprovalRequestWorkflow || IsCSRefReqWorkflow || IsCsChangeRxWorkflow)
                                {
                                    foreach (KeyValuePair<string, long> kvp in rxIDAudit)
                                    {
                                        AuditLogPatientRxCSResponse rxCsResp = AuditLogPatientRxCSInsert(AuditAction.PATIENT_RX_CS_VALIDATE_2ND_FACTOR, SessionPatientID, kvp.Value, true, otpAuthResponse.OtpValidationUtc);
                                        var shieldEvent = EPSBroker.LogShieldAuditEvent(ControlState.GetGuidOr0x0(Constants.SessionVariables.UserId), ControlState.GetGuidOr0x0(Constants.SessionVariables.LicenseId),
                                            kvp.Key.ToGuidOr0x0(), ShieldSecurityToken, EprescribeExternalAppInstanceID, ShieldExternalTenantID, shieldCorrelationId, true, "Success", DBID);
                                        if (!rxCsResp.Success || shieldEvent.Result == SimpleResult.FAILED)
                                        {
                                            isAuditSuccess = false;
                                            break;
                                        }

                                        rxCsResp = AuditLogPatientRxCSInsert(AuditAction.PATIENT_RX_CS_VALIDATE_EPCS_PERM, SessionPatientID, kvp.Value, true, otpAuthResponse.EpcsPermValidationUtc);
                                        if (!rxCsResp.Success)
                                        {
                                            isAuditSuccess = false;
                                            break;
                                        }

                                        rxCsResp = AuditLogPatientRxCSInsert(AuditAction.PATIENT_RX_CS_VALIDATE_CREDENTIALS, SessionPatientID, kvp.Value, true, otpAuthResponse.CredentialValidationUtc);
                                        if (!rxCsResp.Success)
                                        {
                                            isAuditSuccess = false;
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    foreach (KeyValuePair<string, List<AuditPatientRxCSInsertRequest>> cAudit in cachedAudit)
                                    {
                                        List<AuditPatientRxCSInsertRequest> list = cAudit.Value;
                                        AuditPatientRxCSInsertRequest cReq = new AuditPatientRxCSInsertRequest();
                                        cReq.AuditAction = AuditAction.PATIENT_RX_CS_VALIDATE_2ND_FACTOR;
                                        cReq.IsSuccessful = true;
                                        cReq.CreatedUTC = otpAuthResponse.OtpValidationUtc;
                                        list.Add(cReq);

                                        cReq = new AuditPatientRxCSInsertRequest();
                                        cReq.AuditAction = AuditAction.PATIENT_RX_CS_VALIDATE_EPCS_PERM;
                                        cReq.IsSuccessful = true;
                                        cReq.CreatedUTC = otpAuthResponse.EpcsPermValidationUtc;
                                        list.Add(cReq);

                                        cReq = new AuditPatientRxCSInsertRequest();
                                        cReq.AuditAction = AuditAction.PATIENT_RX_CS_VALIDATE_CREDENTIALS;
                                        cReq.IsSuccessful = true;
                                        cReq.CreatedUTC = otpAuthResponse.CredentialValidationUtc;
                                        list.Add(cReq);
                                    }
                                }

                                if (isAuditSuccess)
                                {

                                    this.AuthRetryCount = 0;

                                    //
                                    // find RxIDs that the user has selected for EPCS digital signing
                                    //
                                    List<string> scriptMessageIDs = new List<string>();
                                    List<string> unsignedMeds = new List<string>();

                                    foreach (GridDataItem epcsMed in grdEPCSMedList.MasterTableView.Items)
                                    {
                                        string rxID = null;
                                        string scriptMessageID = null;
                                        if (epcsMed.GetDataKeyValue("RxID") != null)
                                        {
                                            rxID = epcsMed.GetDataKeyValue("RxID").ToString();
                                        }

                                        if (epcsMed.GetDataKeyValue("ScriptMessageID") != null)
                                        {
                                            scriptMessageID = epcsMed.GetDataKeyValue("ScriptMessageID").ToString();
                                        }

                                        if (epcsMed.Selected)
                                        {
                                            string newScriptMessageID = string.Empty;

                                            Rx script = EPCSMEDList.Find(r => r.RxID == rxID && r.ScriptMessageID == scriptMessageID);

                                            if (script != null)
                                            {
                                                if (this.IsScriptForNewRx)
                                                {
                                                    //
                                                    // TODO: confirm whether or not we need to update the patient's LastPharmacyID
                                                    //
                                                    if (script.Destination == "PHARM")
                                                    {
                                                        var pharmacyID = ControlState.GetStringOrEmpty("PHARMACYID");
                                                        if (pharmacyID.Equals(string.Empty))
                                                        {
                                                            var lastPharmacyID = string.IsNullOrWhiteSpace(script.PharmacyID) ? ControlState.GetStringOrEmpty("LASTPHARMACYID") : script.PharmacyID;
                                                            if (!string.IsNullOrWhiteSpace(lastPharmacyID))
                                                            {
                                                                Prescription.UpdatePharmacyIDForEPCS(rxID, lastPharmacyID,_hasPharmacyChanges, base.DBID);
                                                            }
                                                        }
                                                        else
                                                        {
                                                            Prescription.UpdatePharmacyIDForEPCS(rxID, pharmacyID, _hasPharmacyChanges, base.DBID);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        DataSet mobDS = Allscripts.Impact.Pharmacy.LoadPharmacyByNABP(ControlState.GetStringOrEmpty("MOB_NABP"), base.DBID);
                                                        if (mobDS.Tables[0].Rows.Count > 0)
                                                        {
                                                            Prescription.UpdatePharmacyID(rxID, mobDS.Tables[0].Rows[0]["PharmacyID"].ToString(), true, base.DBID);
                                                        }
                                                    }

                                                    //save effective date for each med, where applicable
                                                    RadDatePicker effectiveDate = epcsMed.FindControl("radDatePickerEffectiveDate") as RadDatePicker;

                                                    if (effectiveDate != null)
                                                    {
                                                        Prescription.UpdateEffectiveDate(rxID, effectiveDate.SelectedDate.Value, base.SessionUserID, base.SessionLicenseID, base.DBID);
                                                    }

                                                    // save DEA number and reconciled schedule
                                                    Prescription.SaveCSDetails(rxID, script.ScheduleUsed, litDEAList.Text, base.DBID);

                                                    // create script message for new Rx
                                                    var surescriptsPptMessageId = PPTPlus.RetrieveSurescriptsResponseMessageIdOrEmpty(rxID.ToGuidOr0x0(), ControlState.Cast(Constants.SessionVariables.PptPlusResponses, default(PptPlusResponseContainer)), SessionUserID.ToGuidOr0x0(), SessionLicenseID.ToGuidOr0x0(), DBID, new Audit());
                                                    newScriptMessageID = ScriptMessage.CreateShieldScriptMessage(rxID, 1, Constants.MessageTypes.NEWRX, base.SessionLicenseID, base.SessionUserID, base.ShieldSecurityToken, surescriptsPptMessageId, base.SessionSiteID, Constants.IsEPCSMed.YES, base.DBID);

                                                    if (string.IsNullOrEmpty(newScriptMessageID))
                                                    {
                                                        ePrescribeSvc.AuditLogPatientRxCSResponse rxCsResp = base.AuditLogPatientRxCSInsert(ePrescribeSvc.AuditAction.PATIENT_RX_CS_INSERT_SCRIPT_MESSAGE, base.SessionPatientID, rxIDAudit[rxID], false, DateTime.UtcNow.ToString());
                                                        if (!rxCsResp.Success)
                                                        {
                                                            isAuditSuccess = false;
                                                            eventArgs.Success = false;
                                                            eventArgs.Message = "Controlled substance meds being sent electronically could not be digitally signed.  Please try again or print.";
                                                            break;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        ePrescribeSvc.AuditLogPatientRxCSResponse rxCsResp = base.AuditLogPatientRxCSInsert(ePrescribeSvc.AuditAction.PATIENT_RX_CS_INSERT_SCRIPT_MESSAGE, base.SessionPatientID, rxIDAudit[rxID], true, DateTime.UtcNow.ToString());
                                                        if (!rxCsResp.Success)
                                                        {
                                                            isAuditSuccess = false;
                                                            eventArgs.Success = false;
                                                            eventArgs.Message = "Controlled substance meds being sent electronically could not be digitally signed.  Please try again or print.";
                                                            break;
                                                        }
                                                    }

                                                    scriptMessageIDs.Add(newScriptMessageID);
                                                    selectedMeds.Add(rxID, newScriptMessageID);
                                                    scriptMessageID2RxId.Add(newScriptMessageID, rxID);
                                                }
                                                else if (this.IsCSRefReqWorkflow)
                                                {
                                                    //save effective date for each med, where applicable
                                                    RadDatePicker effectiveDate = epcsMed.FindControl("radDatePickerEffectiveDate") as RadDatePicker;

                                                    if (effectiveDate != null)
                                                    {
                                                        Prescription.UpdateEffectiveDate(rxID, effectiveDate.SelectedDate.Value, base.SessionUserID, base.SessionLicenseID, base.DBID);
                                                    }

                                                    // save DEA number and reconciled schedule
                                                    Prescription.SaveCSDetails(rxID, script.ScheduleUsed, litDEAList.Text, base.DBID);

                                                    var scriptMessage = new ScriptMessage(ControlState.GetStringOrEmpty(Constants.SessionVariables.TaskScriptMessageId), base.SessionLicenseID, base.SessionUserID, base.DBID);

                                                    bool patDemoOrProviderChanged = PatDemoOrProviderChanged(scriptMessage, base.SessionUserID);


                                                    //Remove when createREFRESMessageForReplace is refactored
                                                    if (script.IsFreeFormMed)
                                                    {
                                                        AMEMOMMessageInfo messageInfo = new AMEMOMMessageInfo
                                                        {
                                                            sm = scriptMessage,
                                                            script = script,
                                                            type = "R",
                                                            codequal = string.Empty,
                                                            userID = base.SessionUserID,
                                                            prescriberOrderNumber = rxID,
                                                            shieldSecurityToken = base.ShieldSecurityToken,
                                                            deaNumberUsed = litDEAList.Text,
                                                            effectiveDate = effectiveDate.SelectedDate.Value,
                                                            siteID = base.SessionSiteID,
                                                            dbID = base.DBID,
                                                            isEPCSMed = Constants.IsEPCSMed.YES,
                                                            rxDate = script.RxDateLocal.ToString()
                                                        };
                                                        newScriptMessageID = ScriptMessage.CreateREFRESMessageInAMEMOMFormat(messageInfo);
                                                    }
                                                    else
                                                    {
                                                        if (patDemoOrProviderChanged)
                                                        {
                                                             newScriptMessageID = ScriptMessage.CreateREFRESMessage(scriptMessage, -1, Constants.ResponseTypeDenial, Constants.ResponseTypeDenialCodePatientUnknown, Constants.ResponseDenialReasonUnknownPatient, base.SessionUserID, Guid.Empty.ToString(), base.ShieldSecurityToken, null, null, script.DAW, null, base.SessionSiteID, script.RxDateLocal.ToString(), string.Empty, base.DBID);
                                                           // this will queue up sending the denial
                                                            ScriptMessage.SendThisMessage(newScriptMessageID, base.SessionLicenseID, base.SessionUserID, base.DBID);
                                                           //then create a newRx
                                                            newScriptMessageID = ScriptMessage.CreateShieldScriptMessage(rxID, 1, Constants.MessageTypes.NEWRX, base.SessionLicenseID, base.SessionUserID, base.ShieldSecurityToken, string.Empty, base.SessionSiteID, Constants.IsEPCSMed.YES, base.DBID);

                                                            AuditLogPatientRxCSResponse rxCsResp = base.AuditLogPatientRxCSInsert(ePrescribeSvc.AuditAction.PATIENT_RX_CS_INSERT_SCRIPT_MESSAGE, base.SessionPatientID, rxIDAudit[rxID], true, DateTime.UtcNow.ToString());
                                                            if (!rxCsResp.Success)
                                                            {
                                                                isAuditSuccess = false;
                                                                eventArgs.Success = false;
                                                                eventArgs.Message = "Controlled substance meds being sent electronically could not be digitally signed.  Please try again or print.";
                                                                break;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            // create a new RefRes-replace msg for CS renewal 
                                                            newScriptMessageID = ScriptMessage.CreateREFRESMessageForReplace(scriptMessage, script.Refills + 1, "R", string.Empty, script.Notes, base.SessionUserID, rxID,
                                                                base.ShieldSecurityToken, litDEAList.Text, script.ControlledSubstanceCode, script.DAW, script.DaysSupply.ToString(), script.QuantityFormattedString.ToString(), script.SigText, script.Notes, effectiveDate.SelectedDate.Value, base.SessionSiteID, script.RxDateLocal.ToString(), base.DBID, Constants.IsEPCSMed.YES);
                                                        }
                                                    }

                                                    if (string.IsNullOrEmpty(newScriptMessageID))
                                                    {
                                                        ePrescribeSvc.AuditLogPatientRxCSResponse rxCsResp = base.AuditLogPatientRxCSInsert(ePrescribeSvc.AuditAction.PATIENT_RX_CS_INSERT_SCRIPT_MESSAGE, base.SessionPatientID, rxIDAudit[rxID], false, DateTime.UtcNow.ToString());
                                                        if (!rxCsResp.Success)
                                                        {
                                                            isAuditSuccess = false;
                                                            eventArgs.Success = false;
                                                            eventArgs.Message = "Controlled substance meds being sent electronically could not be digitally signed.  Please try again or print.";
                                                            break;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        ePrescribeSvc.AuditLogPatientRxCSResponse rxCsResp = base.AuditLogPatientRxCSInsert(ePrescribeSvc.AuditAction.PATIENT_RX_CS_INSERT_SCRIPT_MESSAGE, base.SessionPatientID, rxIDAudit[rxID], true, DateTime.UtcNow.ToString());
                                                        if (!rxCsResp.Success)
                                                        {
                                                            isAuditSuccess = false;
                                                            eventArgs.Success = false;
                                                            eventArgs.Message = "Controlled substance meds being sent electronically could not be digitally signed.  Please try again or print.";
                                                            break;
                                                        }
                                                    }

                                                    scriptMessageIDs.Add(newScriptMessageID);
                                                    selectedMeds.Add(rxID, newScriptMessageID);
                                                    scriptMessageID2RxId.Add(newScriptMessageID, rxID);
                                                }
                                                else if (IsCsChangeRxWorkflow)
                                                {
                                                    var taskScriptMessageId = ControlState.GetStringOrEmpty(Constants.SessionVariables.TaskScriptMessageId);
                                                    //save effective date for each med, where applicable
                                                    RadDatePicker effectiveDate = epcsMed.FindControl("radDatePickerEffectiveDate") as RadDatePicker;

                                                    if (effectiveDate != null)
                                                    {
                                                        Prescription.UpdateEffectiveDate(rxID, effectiveDate.SelectedDate.Value, base.SessionUserID, base.SessionLicenseID, base.DBID);
                                                    }

                                                    // save DEA number and reconciled schedule
                                                    Prescription.SaveCSDetails(rxID, script.ScheduleUsed, litDEAList.Text, base.DBID);

                                                    Dictionary<string, string> renewalRequest = new Dictionary<string, string>();
                                                    // create a new Approve with Change msg for CS renewal 
                                                    var sm = new ScriptMessage(ControlState.GetStringOrEmpty(Constants.SessionVariables.TaskScriptMessageId), SessionLicenseID, SessionUserID, DBID);
                                                    Guid delegateProviderId = base.DelegateProvider != null
                                                        ? base.DelegateProvider.UserID.ToGuidOr0x0()
                                                        : Guid.Empty;
                                                    this.PharmacyTask = new RxTaskModel
                                                    {
                                                        ScriptMessage = sm,
                                                        ScriptMessageGUID = ControlState.GetStringOrEmpty(Constants.SessionVariables.TaskScriptMessageId),
                                                        Rx = CurrentRx,
                                                        UserId = SessionUserID.ToGuid(),
                                                        DbId = DBID,
                                                        LicenseId = SessionLicenseID.ToGuid(),
                                                        SiteId = SessionSiteID,
                                                        ShieldSecurityToken = ShieldSecurityToken,
                                                        ExternalFacilityCd = ControlState.GetStringOrEmpty(Constants.SessionVariables.ExternalFacilityCd),
                                                        ExternalGroupID = ControlState.GetStringOrEmpty(Constants.SessionVariables.ExternalGroupID),
                                                        UserType = SessionUserType,
                                                        DelegateProviderId = delegateProviderId
                                                    };
                                                    this.PharmacyTask.RxRequestType = RequestType.APPROVE_WITH_CHANGE;
                                                    renewalRequest = ScriptMessage.ApproveRXCHGMessageForEPCS(this.PharmacyTask, base.SessionUserID, base.CurrentRx.Notes, Constants.PrescriptionTransmissionMethod.SENT,
                                                        base.SessionLicenseID, base.SessionUserID, base.SessionSiteID, base.ShieldSecurityToken, script.ScheduleUsed, litDEAList.Text, effectiveDate.SelectedDate.Value,
                                                        base.CurrentRx.IsCompoundMed, base.CurrentRx.HasSupplyItem, delegateProviderId.ToString(), true, script.MDD, base.DBID);

                                                    KeyValuePair<string, string> renewalKeyValuePair = renewalRequest.First();
                                                    newScriptMessageID = renewalKeyValuePair.Value;

                                                    if (string.IsNullOrEmpty(newScriptMessageID))
                                                    {
                                                        ePrescribeSvc.AuditLogPatientRxCSResponse rxCsResp = base.AuditLogPatientRxCSInsert(ePrescribeSvc.AuditAction.PATIENT_RX_CS_INSERT_SCRIPT_MESSAGE, base.SessionPatientID, rxIDAudit[rxID], false, DateTime.UtcNow.ToString());
                                                        if (!rxCsResp.Success)
                                                        {
                                                            isAuditSuccess = false;
                                                            eventArgs.Success = false;
                                                            eventArgs.Message = "Controlled substance meds being sent electronically could not be digitally signed.  Please try again or print.";
                                                            break;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        ePrescribeSvc.AuditLogPatientRxCSResponse rxCsResp = base.AuditLogPatientRxCSInsert(ePrescribeSvc.AuditAction.PATIENT_RX_CS_INSERT_SCRIPT_MESSAGE, base.SessionPatientID, rxIDAudit[rxID], true, DateTime.UtcNow.ToString());
                                                        if (!rxCsResp.Success)
                                                        {
                                                            isAuditSuccess = false;
                                                            eventArgs.Success = false;
                                                            eventArgs.Message = "Controlled substance meds being sent electronically could not be digitally signed.  Please try again or print.";
                                                            break;
                                                        }
                                                    }

                                                    scriptMessageIDs.Add(newScriptMessageID);
                                                    selectedMeds.Add(rxID, newScriptMessageID);
                                                    scriptMessageID2RxId.Add(newScriptMessageID, rxID);
                                                }
                                                else if (this.IsApprovalRequestWorkflow)
                                                {
                                                    //save effective date for each med, where applicable
                                                    RadDatePicker effectiveDate = epcsMed.FindControl("radDatePickerEffectiveDate") as RadDatePicker;

                                                    if (effectiveDate != null)
                                                    {
                                                        Prescription.UpdateEffectiveDate(rxID, effectiveDate.SelectedDate.Value, base.SessionUserID, base.SessionLicenseID, base.DBID);
                                                    }

                                                    // save DEA number and reconciled schedule
                                                    Prescription.SaveCSDetails(rxID, script.ScheduleUsed, litDEAList.Text, base.DBID);

                                                    // create script message for new Rx
                                                    newScriptMessageID = ScriptMessage.CreateShieldScriptMessage(rxID, 1, Constants.MessageTypes.NEWRX, base.SessionLicenseID, base.SessionUserID, base.ShieldSecurityToken, string.Empty, base.SessionSiteID, Constants.IsEPCSMed.YES, base.DBID);

                                                    if (string.IsNullOrEmpty(newScriptMessageID))
                                                    {
                                                        ePrescribeSvc.AuditLogPatientRxCSResponse rxCsResp = base.AuditLogPatientRxCSInsert(ePrescribeSvc.AuditAction.PATIENT_RX_CS_INSERT_SCRIPT_MESSAGE, base.SessionPatientID, rxIDAudit[rxID], false, DateTime.UtcNow.ToString());
                                                        if (!rxCsResp.Success)
                                                        {
                                                            isAuditSuccess = false;
                                                            eventArgs.Success = false;
                                                            eventArgs.Message = "Controlled substance meds being sent electronically could not be digitally signed.  Please try again or print.";
                                                            break;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        ePrescribeSvc.AuditLogPatientRxCSResponse rxCsResp = base.AuditLogPatientRxCSInsert(ePrescribeSvc.AuditAction.PATIENT_RX_CS_INSERT_SCRIPT_MESSAGE, base.SessionPatientID, rxIDAudit[rxID], true, DateTime.UtcNow.ToString());
                                                        if (!rxCsResp.Success)
                                                        {
                                                            isAuditSuccess = false;
                                                            eventArgs.Success = false;
                                                            eventArgs.Message = "Controlled substance meds being sent electronically could not be digitally signed.  Please try again or print.";
                                                            break;
                                                        }
                                                    }


                                                    scriptMessageIDs.Add(newScriptMessageID);
                                                    selectedMeds.Add(rxID, newScriptMessageID);
                                                    scriptMessageID2RxId.Add(newScriptMessageID, rxID);
                                                }
                                                else if (this.IsRenewalApprovalWorkflow)
                                                {
                                                    //
                                                    // this workflow allows for multiple renewal approvals for a single workflow
                                                    //

                                                    bool? isCSRegistryChecked = ControlState.GetBooleanOrNull("isCSRegistryChecked");
                                                    ControlState.Remove("isCSRegistryChecked");

                                                    DateTime effectiveDate = DateTime.UtcNow;

                                                    RadDatePicker radDatePickerEffectiveDate = epcsMed.FindControl("radDatePickerEffectiveDate") as RadDatePicker;
                                                    if (radDatePickerEffectiveDate != null)
                                                    {
                                                        effectiveDate = radDatePickerEffectiveDate.SelectedDate.Value;
                                                    }

                                                    // create script message for refill request
                                                    string delegateProviderID = ControlState.GetStringOrEmpty("DelegateProviderID");

                                                    Dictionary<string, string> renewalRequest = ScriptMessage.ApproveMessageForEPCS(
                                                        scriptMessageID, script.DDI, script.DaysSupply, script.Quantity, script.Refills, script.SigText, script.DAW, base.SessionUserID, script.Notes,
                                                        Constants.PrescriptionTransmissionMethod.SENT, base.SessionLicenseID, base.SessionUserID, base.SessionSiteID, base.ShieldSecurityToken, script.ScheduleUsed,
                                                        litDEAList.Text, effectiveDate, script.IsCompoundMed, script.HasSupplyItem, delegateProviderID, isCSRegistryChecked, script.MDD, script.SigID, string.Empty, base.DBID);

                                                    KeyValuePair<string, string> renewalKeyValuePair = renewalRequest.First();
                                                    scriptMessageIDs.Add(renewalKeyValuePair.Value);
                                                    selectedMeds.Add(renewalKeyValuePair.Key, renewalKeyValuePair.Value);
                                                    scriptMessageID2RxId.Add(scriptMessageID, renewalKeyValuePair.Key);

                                                    if (string.IsNullOrEmpty(renewalKeyValuePair.Value))
                                                    {
                                                        List<ePrescribeSvc.AuditPatientRxCSInsertRequest> list = cachedAudit[scriptMessageID];
                                                        ePrescribeSvc.AuditPatientRxCSInsertRequest cReq = new ePrescribeSvc.AuditPatientRxCSInsertRequest();
                                                        cReq.AuditAction = ePrescribeSvc.AuditAction.PATIENT_RX_CS_INSERT_SCRIPT_MESSAGE;
                                                        cReq.IsSuccessful = false;
                                                        cReq.CreatedUTC = DateTime.UtcNow.ToString(dateFormat);
                                                        list.Add(cReq);

                                                    }
                                                    else
                                                    {
                                                        List<ePrescribeSvc.AuditPatientRxCSInsertRequest> list = cachedAudit[scriptMessageID];
                                                        ePrescribeSvc.AuditPatientRxCSInsertRequest cReq = new ePrescribeSvc.AuditPatientRxCSInsertRequest();
                                                        cReq.AuditAction = ePrescribeSvc.AuditAction.PATIENT_RX_CS_INSERT_SCRIPT_MESSAGE;
                                                        cReq.IsSuccessful = true;
                                                        cReq.CreatedUTC = DateTime.UtcNow.ToString(dateFormat);
                                                        list.Add(cReq);
                                                    }

                                                    this.RefReqCrossReference.Add(renewalKeyValuePair.Value, scriptMessageID);
                                                }
                                                else
                                                {
                                                    //
                                                    // this is for processing renewal tasks that had to be reconciled from the SIG page
                                                    //
                                                    bool? isCSRegistryChecked = ControlState.GetBooleanOrNull("isCSRegistryChecked");
                                                    ControlState.Remove("isCSRegistryChecked");

                                                    DateTime effectiveDate = DateTime.UtcNow;

                                                    RadDatePicker radDatePickerEffectiveDate = epcsMed.FindControl("radDatePickerEffectiveDate") as RadDatePicker;
                                                    if (radDatePickerEffectiveDate != null)
                                                    {
                                                        effectiveDate = radDatePickerEffectiveDate.SelectedDate.Value;
                                                    }

                                                    decimal metricQuantity = base.CurrentRx.Quantity;

                                                    if ((base.CurrentRx.PackageSize * base.CurrentRx.PackageQuantity) > 0)
                                                    {
                                                        metricQuantity = base.CurrentRx.Quantity * base.CurrentRx.PackageSize * base.CurrentRx.PackageQuantity;
                                                    }

                                                    Dictionary<string, string> renewalRequest = new Dictionary<string, string>();


                                                    string delegateProviderID = ControlState.GetStringOrEmpty("DelegateProviderID");
                                                    var taskScriptMessageId = ControlState.GetStringOrEmpty(Constants.SessionVariables.TaskScriptMessageId);

                                                    if (string.IsNullOrWhiteSpace(base.CurrentRx.DDI))
                                                    {
                                                        if (this.PharmacyTask != null && this.PharmacyTask.TaskType== Constants.PrescriptionTaskType.RXCHG)
                                                        {
                                                            renewalRequest = ScriptMessage.ApproveRXCHGFreeFormMessageForEPCS(this.PharmacyTask, base.SessionUserID, base.CurrentRx.Notes, Constants.PrescriptionTransmissionMethod.SENT,
                                                                                base.SessionLicenseID, base.SessionUserID, base.SessionSiteID, base.ShieldSecurityToken, Convert.ToInt32(script.ControlledSubstanceCode), litDEAList.Text, effectiveDate,
                                                                                base.CurrentRx.IsCompoundMed, base.CurrentRx.HasSupplyItem, delegateProviderID, isCSRegistryChecked, CurrentRx.MDD, base.DBID);
                                                        }
                                                        else
                                                        {
                                                            //free form med refres workflow
                                                            renewalRequest = ScriptMessage.ApproveFreeFormMessageForEPCS(taskScriptMessageId, base.CurrentRx.MedicationName, base.CurrentRx.DaysSupply, base.CurrentRx.Quantity,
                                                                                base.CurrentRx.Refills, base.CurrentRx.SigText, base.CurrentRx.DAW, base.SessionUserID, base.CurrentRx.Notes, Constants.PrescriptionTransmissionMethod.SENT,
                                                                                base.SessionLicenseID, base.SessionUserID, base.SessionSiteID, base.ShieldSecurityToken, script.ScheduleUsed, litDEAList.Text, effectiveDate,
                                                                                base.CurrentRx.IsCompoundMed, base.CurrentRx.HasSupplyItem, delegateProviderID, isCSRegistryChecked, CurrentRx.MDD, CurrentRx.SigID, base.DBID);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (this.PharmacyTask != null && this.PharmacyTask.TaskType == Constants.PrescriptionTaskType.RXCHG)
                                                        {
                                                            renewalRequest = ScriptMessage.ApproveRXCHGMessageForEPCS(this.PharmacyTask, base.SessionUserID, base.CurrentRx.Notes, Constants.PrescriptionTransmissionMethod.SENT,
                                                                        base.SessionLicenseID, base.SessionUserID, base.SessionSiteID, base.ShieldSecurityToken, script.ScheduleUsed, litDEAList.Text, effectiveDate,
                                                                        base.CurrentRx.IsCompoundMed, base.CurrentRx.HasSupplyItem, delegateProviderID, isCSRegistryChecked, script.MDD, base.DBID);
                                                        }
                                                        else
                                                        {
                                                            // create script message for refill request
                                                            renewalRequest = ScriptMessage.ApproveMessageForEPCS(
                                                                        taskScriptMessageId, base.CurrentRx.DDI, base.CurrentRx.DaysSupply, metricQuantity, base.CurrentRx.Refills,
                                                                        base.CurrentRx.SigText, base.CurrentRx.DAW, base.SessionUserID, base.CurrentRx.Notes, Constants.PrescriptionTransmissionMethod.SENT,
                                                                        base.SessionLicenseID, base.SessionUserID, base.SessionSiteID, base.ShieldSecurityToken, script.ScheduleUsed, litDEAList.Text, effectiveDate,
                                                                        base.CurrentRx.IsCompoundMed, base.CurrentRx.HasSupplyItem, delegateProviderID, isCSRegistryChecked, script.MDD, script.SigID, base.ServiceAccountAuthToken, base.DBID);
                                                        }
                                                    }

                                                    KeyValuePair<string, string> renewalKeyValuePair = renewalRequest.First();
                                                    scriptMessageIDs.Add(renewalKeyValuePair.Value);
                                                    selectedMeds.Add(renewalKeyValuePair.Key, renewalKeyValuePair.Value);
                                                    scriptMessageID2RxId.Add(taskScriptMessageId, renewalKeyValuePair.Key);

                                                    if (string.IsNullOrEmpty(renewalKeyValuePair.Value))
                                                    {
                                                        List<ePrescribeSvc.AuditPatientRxCSInsertRequest> list = cachedAudit[taskScriptMessageId];
                                                        ePrescribeSvc.AuditPatientRxCSInsertRequest cReq = new ePrescribeSvc.AuditPatientRxCSInsertRequest();
                                                        cReq.AuditAction = ePrescribeSvc.AuditAction.PATIENT_RX_CS_INSERT_SCRIPT_MESSAGE;
                                                        cReq.IsSuccessful = false;
                                                        cReq.CreatedUTC = DateTime.UtcNow.ToString();
                                                        list.Add(cReq);
                                                    }
                                                    else
                                                    {
                                                        List<ePrescribeSvc.AuditPatientRxCSInsertRequest> list = cachedAudit[taskScriptMessageId];
                                                        ePrescribeSvc.AuditPatientRxCSInsertRequest cReq = new ePrescribeSvc.AuditPatientRxCSInsertRequest();
                                                        cReq.AuditAction = ePrescribeSvc.AuditAction.PATIENT_RX_CS_INSERT_SCRIPT_MESSAGE;
                                                        cReq.IsSuccessful = true;
                                                        cReq.CreatedUTC = DateTime.UtcNow.ToString();
                                                        list.Add(cReq);
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            unsignedMeds.Add(rxID);
                                        }
                                    }

                                    //
                                    // call EPS method to digitally sign the selected CS meds
                                    //
                                    SignPrescriptionsResponse signingResponse = EPSBroker.SignPrescriptionAndInsertSignedScriptMessage(
                                        otpAuthResponse.OtpAuthenticatedSecurityToken,
                                        scriptMessageIDs.ToArray(),
                                        SessionLicenseID,
                                        SessionUserID,
                                        SessionShieldUserName,
                                        SessionEprescribeUserName,
                                        otpAuthResponse.IdentitySecurityToken,
                                        EprescribeExternalAppInstanceID,
                                        shieldCorrelationId,
                                        DBID);

                                    if (signingResponse.Success)
                                    {
                                        //
                                        // since success from EPS signing method means all select rx/script messages were signed we can pass the selectedMeds dictionary back to consumer
                                        //
                                        eventArgs.SignedMeds = selectedMeds;
                                        eventArgs.Success = true;

                                        if (this.IsScriptForNewRx || this.IsApprovalRequestWorkflow || this.IsCSRefReqWorkflow || IsCsChangeRxWorkflow)
                                        {
                                            foreach (KeyValuePair<string, long> kvp in rxIDAudit)
                                            {
                                                ePrescribeSvc.AuditLogPatientRxCSResponse rxCsResp = base.AuditLogPatientRxCSInsert(ePrescribeSvc.AuditAction.PATIENT_RX_CS_SIGN_EPCS_RX, base.SessionPatientID, kvp.Value, true, DateTime.UtcNow.ToString(dateFormat));
                                                if (!rxCsResp.Success)
                                                {
                                                    isAuditSuccess = false;
                                                    eventArgs.Success = false;
                                                    eventArgs.Message = _errorMessage;
                                                    break;
                                                }

                                                rxCsResp = base.AuditLogPatientRxCSInsert(ePrescribeSvc.AuditAction.PATIENT_RX_CS_INSERT_SIGNED_SCRIPT_MESSAGE, base.SessionPatientID, kvp.Value, true, DateTime.UtcNow.ToString(dateFormat));
                                                if (!rxCsResp.Success)
                                                {
                                                    isAuditSuccess = false;
                                                    eventArgs.Success = false;
                                                    eventArgs.Message = _errorMessage;
                                                    break;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            foreach (KeyValuePair<string, List<ePrescribeSvc.AuditPatientRxCSInsertRequest>> cAudit in cachedAudit)
                                            {
                                                List<ePrescribeSvc.AuditPatientRxCSInsertRequest> list = cAudit.Value;
                                                ePrescribeSvc.AuditPatientRxCSInsertRequest cReq = new ePrescribeSvc.AuditPatientRxCSInsertRequest();
                                                cReq.AuditAction = ePrescribeSvc.AuditAction.PATIENT_RX_CS_SIGN_EPCS_RX;
                                                cReq.IsSuccessful = true;
                                                cReq.CreatedUTC = DateTime.UtcNow.ToString(dateFormat);
                                                list.Add(cReq);

                                                cReq = new ePrescribeSvc.AuditPatientRxCSInsertRequest();
                                                cReq.AuditAction = ePrescribeSvc.AuditAction.PATIENT_RX_CS_INSERT_SIGNED_SCRIPT_MESSAGE;
                                                cReq.IsSuccessful = true;
                                                cReq.CreatedUTC = DateTime.UtcNow.ToString(dateFormat);
                                                list.Add(cReq);
                                            }
                                        }

                                        foreach (var scriptMessageID in scriptMessageIDs)
                                        {
                                            ScriptMessage.InsertMessageQueueTx(scriptMessageID, SessionSiteID, DBID);
                                        }
                                    }
                                    else
                                    {

                                        eventArgs.Success = false;
                                        eventArgs.Message = _errorMessage;

                                        if (!signingResponse.IsPrescriptionSigned)
                                        {
                                            if (this.IsScriptForNewRx || this.IsApprovalRequestWorkflow || this.IsCSRefReqWorkflow || IsCsChangeRxWorkflow)
                                            {
                                                foreach (KeyValuePair<string, long> kvp in rxIDAudit)
                                                {
                                                    ePrescribeSvc.AuditLogPatientRxCSResponse rxCsResp = base.AuditLogPatientRxCSInsert(ePrescribeSvc.AuditAction.PATIENT_RX_CS_SIGN_EPCS_RX, base.SessionPatientID, kvp.Value, false, DateTime.UtcNow.ToString(dateFormat));
                                                    if (!rxCsResp.Success)
                                                    {
                                                        isAuditSuccess = false;
                                                        break;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                foreach (KeyValuePair<string, List<ePrescribeSvc.AuditPatientRxCSInsertRequest>> cAudit in cachedAudit)
                                                {
                                                    List<ePrescribeSvc.AuditPatientRxCSInsertRequest> list = cAudit.Value;
                                                    ePrescribeSvc.AuditPatientRxCSInsertRequest cReq = new ePrescribeSvc.AuditPatientRxCSInsertRequest();
                                                    cReq.AuditAction = ePrescribeSvc.AuditAction.PATIENT_RX_CS_SIGN_EPCS_RX;
                                                    cReq.IsSuccessful = false;
                                                    cReq.CreatedUTC = DateTime.UtcNow.ToString(dateFormat);
                                                    list.Add(cReq);
                                                }
                                            }
                                        }

                                        if (!signingResponse.IsSignedScriptMessageInserted)
                                        {
                                            if (this.IsScriptForNewRx || this.IsApprovalRequestWorkflow || this.IsCSRefReqWorkflow || IsCsChangeRxWorkflow)
                                            {
                                                foreach (KeyValuePair<string, long> kvp in rxIDAudit)
                                                {
                                                    ePrescribeSvc.AuditLogPatientRxCSResponse rxCsResp = base.AuditLogPatientRxCSInsert(ePrescribeSvc.AuditAction.PATIENT_RX_CS_INSERT_SIGNED_SCRIPT_MESSAGE, base.SessionPatientID, kvp.Value, false, DateTime.UtcNow.ToString(dateFormat));
                                                    if (!rxCsResp.Success)
                                                    {
                                                        isAuditSuccess = false;
                                                        break;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                foreach (KeyValuePair<string, List<ePrescribeSvc.AuditPatientRxCSInsertRequest>> cAudit in cachedAudit)
                                                {
                                                    List<ePrescribeSvc.AuditPatientRxCSInsertRequest> list = cAudit.Value;

                                                    ePrescribeSvc.AuditPatientRxCSInsertRequest cReq = new ePrescribeSvc.AuditPatientRxCSInsertRequest();
                                                    cReq.AuditAction = ePrescribeSvc.AuditAction.PATIENT_RX_CS_INSERT_SIGNED_SCRIPT_MESSAGE;
                                                    cReq.IsSuccessful = false;
                                                    cReq.CreatedUTC = DateTime.UtcNow.ToString(dateFormat);
                                                    list.Add(cReq);
                                                }
                                            }
                                        }


                                        EPSBroker.DeleteScriptMessages(scriptMessageIDs, base.DBID);
                                    }

                                    eventArgs.UnsignedMeds = unsignedMeds;
                                    //this.btnSubmitEPCS.Enabled = false;
                                    mpeEPCS.Hide();

                                }
                                else
                                {
                                    eventArgs.Success = false;
                                    eventArgs.Message = _errorMessage;
                                }
                            }
                            else
                            {
                                //check if the user is still EPCS authorized, if not go back to script pad destination page without Send to Pharmacy/Send to Mail Order options
                                //check to see if Shield authentication failed, if it is third attempt then loguser out

                                if (!otpAuthResponse.IsUserIdentified)
                                {
                                    //increment the retry count
                                    this.AuthRetryCount++;

                                    //after 3 invalid attempts, log the user out the system
                                    if (this.AuthRetryCount > 2)
                                    {
                                        eventArgs.Success = false;
                                        eventArgs.ForceLogout = true;
                                    }
                                    else
                                    {
                                        lblAuthenticationStatus.Visible = true;
                                        btnSubmitEPCS.Enabled = true;
                                        lblAuthenticationStatus.Text = "Authentication failed. Please re-enter your password.";

                                        eventArgs.Success = false;
                                        eventArgs.ForceLogout = false;
                                    }

                                    if (IsScriptForNewRx || IsApprovalRequestWorkflow || this.IsCSRefReqWorkflow || IsCsChangeRxWorkflow)
                                    {
                                        foreach (KeyValuePair<string, long> kvp in rxIDAudit)
                                        {
                                            AuditLogPatientRxCSResponse rxCsResp = AuditLogPatientRxCSInsert(AuditAction.PATIENT_RX_CS_VALIDATE_CREDENTIALS, SessionPatientID, kvp.Value, false, otpAuthResponse.CredentialValidationUtc);
                                            EPSBroker.LogShieldAuditEvent(ControlState.GetGuidOr0x0(Constants.SessionVariables.UserId), ControlState.GetGuidOr0x0(Constants.SessionVariables.LicenseId),
                                                kvp.Key.ToGuidOr0x0(), ShieldSecurityToken, EprescribeExternalAppInstanceID, ShieldExternalTenantID, shieldCorrelationId, false, "Authentication Failed", DBID);

                                            if (!rxCsResp.Success)
                                            {
                                                isAuditSuccess = false;
                                                break;
                                            }
                                            ///No need to check if auditing fail because it will change the original message if we handle it.
                                        }
                                    }
                                    else
                                    {
                                        foreach (KeyValuePair<string, List<AuditPatientRxCSInsertRequest>> cAudit in cachedAudit)
                                        {
                                            List<AuditPatientRxCSInsertRequest> list = cAudit.Value;
                                            AuditPatientRxCSInsertRequest cReq = new AuditPatientRxCSInsertRequest();
                                            cReq.AuditAction = AuditAction.PATIENT_RX_CS_VALIDATE_CREDENTIALS;
                                            cReq.IsSuccessful = false;
                                            cReq.CreatedUTC = otpAuthResponse.CredentialValidationUtc;
                                            list.Add(cReq);
                                        }
                                    }
                                }
                                else if (!otpAuthResponse.IsUserEpcsAuthorized)
                                {
                                    //TODO: log exception and add id to the message shown to the user
                                    eventArgs.Message = "You no longer have permissions to send controlled substances electronically. You can only print controlled substances at this time.";
                                    eventArgs.EpcsRightsRemoved = true;
                                    foreach (KeyValuePair<string, long> kvp in rxIDAudit)
                                    {
                                        EPSBroker.LogShieldAuditEvent(ControlState.GetGuidOr0x0(Constants.SessionVariables.UserId),ControlState.GetGuidOr0x0(Constants.SessionVariables.LicenseId),kvp.Key.ToGuidOr0x0(), ShieldSecurityToken, EprescribeExternalAppInstanceID,
                                            ShieldExternalTenantID, shieldCorrelationId, false, "EPCS Not Allowed",
                                            DBID);
                                    }

                                    if (IsScriptForNewRx || IsApprovalRequestWorkflow)
                                    {
                                        foreach (KeyValuePair<string, long> kvp in rxIDAudit)
                                        {
                                            AuditLogPatientRxCSResponse rxCsResp = AuditLogPatientRxCSInsert(AuditAction.PATIENT_RX_CS_VALIDATE_EPCS_PERM, SessionPatientID, kvp.Value, false, otpAuthResponse.EpcsPermValidationUtc);
                                            if (!rxCsResp.Success)
                                            {
                                                isAuditSuccess = false;
                                                break;
                                            }
                                            ///No need to check if auditing fail because it will change the original message if we handle it.
                                        }
                                    }
                                    else
                                    {
                                        foreach (KeyValuePair<string, List<AuditPatientRxCSInsertRequest>> cAudit in cachedAudit)
                                        {
                                            List<AuditPatientRxCSInsertRequest> list = cAudit.Value;
                                            AuditPatientRxCSInsertRequest cReq = new AuditPatientRxCSInsertRequest();
                                            cReq.AuditAction = AuditAction.PATIENT_RX_CS_VALIDATE_EPCS_PERM;
                                            cReq.IsSuccessful = false;
                                            cReq.CreatedUTC = otpAuthResponse.EpcsPermValidationUtc;
                                            list.Add(cReq);
                                        }
                                    }
                                }
                                else if (!otpAuthResponse.IsOtpValidated)
                                {
                                    lblAuthenticationStatus.Visible = true;
                                    lblAuthenticationStatus.Text = "One Time Password validation failed. Please try again.";

                                    foreach (KeyValuePair<string, long> kvp in rxIDAudit)
                                    {
                                        EPSBroker.LogShieldAuditEvent(ControlState.GetGuidOr0x0(Constants.SessionVariables.UserId), ControlState.GetGuidOr0x0(Constants.SessionVariables.LicenseId), kvp.Key.ToGuidOr0x0(), ShieldSecurityToken, EprescribeExternalAppInstanceID,
                                            ShieldExternalTenantID, shieldCorrelationId, false, "One Time Password Failed",
                                            DBID);
                                    }

                                    if (IsScriptForNewRx || IsApprovalRequestWorkflow)
                                    {
                                        foreach (KeyValuePair<string, long> kvp in rxIDAudit)
                                        {
                                            AuditLogPatientRxCSResponse rxCsResp = AuditLogPatientRxCSInsert(AuditAction.PATIENT_RX_CS_VALIDATE_2ND_FACTOR, SessionPatientID, kvp.Value, false, otpAuthResponse.OtpValidationUtc);
                                            if (!rxCsResp.Success)
                                            {
                                                isAuditSuccess = false;
                                                break;
                                            }
                                            ///No need to check if auditing fail because it will change the original message if we handle it.
                                        }
                                    }
                                    else
                                    {
                                        foreach (KeyValuePair<string, List<AuditPatientRxCSInsertRequest>> cAudit in cachedAudit)
                                        {
                                            List<AuditPatientRxCSInsertRequest> list = cAudit.Value;
                                            AuditPatientRxCSInsertRequest cReq = new AuditPatientRxCSInsertRequest();
                                            cReq.AuditAction = AuditAction.PATIENT_RX_CS_VALIDATE_2ND_FACTOR;
                                            cReq.IsSuccessful = false;
                                            cReq.CreatedUTC = otpAuthResponse.OtpValidationUtc;
                                            list.Add(cReq);
                                        }
                                    }
                                }
                                btnSubmitEPCS.Enabled = true;
                                mpeEPCS.Show();
                            }
                        }
                        else
                        {
                            eventArgs.Success = false;
                            eventArgs.Message = "There is no second factor form tied to this user account. Controlled substances can only be printed at this time.";

                            if (this.IsScriptForNewRx || this.IsApprovalRequestWorkflow)
                            {
                                foreach (KeyValuePair<string, long> kvp in rxIDAudit)
                                {
                                    ePrescribeSvc.AuditLogPatientRxCSResponse rxCsResp = base.AuditLogPatientRxCSInsert(ePrescribeSvc.AuditAction.PATIENT_RX_CS_VALIDATE_2ND_FACTOR, base.SessionPatientID, kvp.Value, false, DateTime.UtcNow.ToString());
                                    if (!rxCsResp.Success)
                                    {
                                        isAuditSuccess = false;
                                        break;
                                    }
                                    ///No need to check if auditing fail because it will change the original message if we handle it.
                                }
                            }
                            else
                            {
                                foreach (KeyValuePair<string, List<ePrescribeSvc.AuditPatientRxCSInsertRequest>> cAudit in cachedAudit)
                                {
                                    List<ePrescribeSvc.AuditPatientRxCSInsertRequest> list = cAudit.Value;
                                    ePrescribeSvc.AuditPatientRxCSInsertRequest cReq = new ePrescribeSvc.AuditPatientRxCSInsertRequest();
                                    cReq.AuditAction = ePrescribeSvc.AuditAction.PATIENT_RX_CS_VALIDATE_2ND_FACTOR;
                                    cReq.IsSuccessful = false;
                                    cReq.CreatedUTC = DateTime.UtcNow.ToString();
                                    list.Add(cReq);
                                }
                            }
                        }
                    }
                    else
                    {
                        eventArgs.Success = false;
                        eventArgs.IsMismatch = isMismatch;
                        StringBuilder mismatchErrorMessage = new StringBuilder();
                        if (isMismatch)
                        {
                            var medlist = from rx in rxList
                                          select rx.MedicationName;
                            if (rxList.Count == 1)
                            {
                                mismatchErrorMessage.Append(string.Format("A data error has occurred with your script. Please recreate the following script and try again ({0})."
                                                                                    , string.Join(", ", medlist.ToArray())));
                            }
                            else
                            {
                                mismatchErrorMessage.Append(string.Format("A data error has occurred with your scripts. Please recreate the following scripts and try again ({0})."
                                                                                    , string.Join(", ", medlist.ToArray())));
                            }
                        }

                        eventArgs.Message = isMismatch ? mismatchErrorMessage.ToString() : _errorMessage;
                    }
                }
                catch (Exception ex)
                {
                    string exceptionID = Audit.AddException(base.SessionUserID, base.SessionLicenseID, "Error Signing the Prescription: " + ex.ToString(), string.Empty, string.Empty, string.Empty, base.DBID);
                    eventArgs.Message = _errorMessage;
                    eventArgs.Success = false;
                }
                finally
                {
                    if (!this.IsScriptForNewRx && !this.IsApprovalRequestWorkflow && !isMismatch && this.PharmacyTask==null && !this.IsRenewalApprovalWorkflow && !this.IsCSRefReqWorkflow && !IsCsChangeRxWorkflow)
                    {
                        bool IsAuditLogSuccess = true;
                        foreach (KeyValuePair<string, List<ePrescribeSvc.AuditPatientRxCSInsertRequest>> cAudit in cachedAudit)
                        {
                            string rxID = scriptMessageID2RxId.ContainsKey(cAudit.Key) ? scriptMessageID2RxId[cAudit.Key] : null;
                            List<ePrescribeSvc.AuditPatientRxCSInsertRequest> list = cAudit.Value;
                            ePrescribeSvc.AuditLogPatientRxResponse rxResponse = base.AuditLogPatientRxInsert(ePrescribeSvc.AuditAction.PATIENT_TASK_PROCESS, base.SessionPatientID, rxID, DateTime.UtcNow.ToString());
                            if (rxResponse.Success && !string.IsNullOrEmpty(rxID))
                            {
                                long AuditlogPatientRxID = rxResponse.AuditLogPatientRxID;
                                _auditLogPatientID.Add(rxID, rxResponse.AuditLogPatientID);
                                foreach (ePrescribeSvc.AuditPatientRxCSInsertRequest item in list)
                                {
                                    ePrescribeSvc.AuditLogPatientRxCSResponse rxCSResponse =
                                        base.AuditLogPatientRxCSInsert(item.AuditAction, base.SessionPatientID,
                                            AuditlogPatientRxID, item.IsSuccessful, item.CreatedUTC);
                                    if (!rxCSResponse.Success)
                                    {
                                        IsAuditLogSuccess = false;
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                //placing instrumentation logs for checking why sometimes rxId is null, 
                                //In local debugging its always valid, logging cAudit.key will help me lookup rxId in ScriptMessage table
                                logger.Debug("cAudit.Key = " + cAudit.Key);

                            }
                            if (!IsAuditLogSuccess)
                                break;
                        }

                        if (!IsAuditLogSuccess)
                        {
                            eventArgs.Success = false;
                            eventArgs.Message = _errorMessage;
                        }
                    }
                }

                if (OnDigitalSigningComplete != null && eventArgs.Success) //If CS refreq workflow sucess, update task to be complete and add refill 
                {
                    if ((this.IsCSRefReqWorkflow || IsCsChangeRxWorkflow) && Session[Constants.SessionVariables.TaskScriptMessageId] != null)
                    {
                        string providerID = Session["UserID"].ToString();

                        if (!string.IsNullOrEmpty(providerID))
                        {
                            ScriptMessage.updateRxTask(Session[Constants.SessionVariables.TaskScriptMessageId].ToString(), string.Empty, Constants.PrescriptionStatus.COMPLETE, providerID, base.DBID);
                            string taskType = this.IsCSRefReqWorkflow ? "Refill" : "Rx Change";
                            Session["REFILLMSG"] = taskType +" for " + base.CurrentRx.MedicationName + " approved for " + Session["PATIENTNAME"].ToString().Trim() + ".";
                        }
                    }
                }

                if (OnDigitalSigningComplete != null)
                {
                    logger.Debug($"Calling back OnDigitalSigningComplete{eventArgs.ToLogString()}");
                    OnDigitalSigningComplete(eventArgs);
                }

            }

        }

        private bool PatDemoOrProviderChanged(ScriptMessage sm, string userid)
        {
            string requestPatFirstName = sm.PatientFirstName;
            string requestPatLastName = sm.PatientLastName;
            string providerIDToCompare = sm.ProviderID;
            DateTime requestPatDOB = Convert.ToDateTime(sm.PatientDOB);
            string currentPatFirstName = sm.DBPatientFirstName;
            string currentPatLastName = sm.DBPatientLastName;
            DateTime currentPatDOB = Convert.ToDateTime(sm.DBPatientDOB);

            bool patDemoChanged =
                (!requestPatFirstName.Equals(currentPatFirstName, StringComparison.OrdinalIgnoreCase)) ||
                !requestPatLastName.Equals(currentPatLastName, StringComparison.OrdinalIgnoreCase) ||
                DateTime.Compare(requestPatDOB, currentPatDOB) != 0;

            bool providerChanged = !sm.DBProviderID.Equals(providerIDToCompare, StringComparison.OrdinalIgnoreCase) ||
                                   !sm.DBProviderID.Equals(userid, StringComparison.OrdinalIgnoreCase);
            return patDemoChanged || providerChanged;
        }

        private string isMismatchMed(Rx inMemoryMed, Rx originalMed)
        {
            string causeProperty = string.Empty;
            List<string> checkProperties = new string[] { "DDI", "OriginalDDI", "LineNumber", "FormularyStatus", "OriginalFormularyStatusCode",
            "OriginalIsListed", "PriorAuthRequired", "SourceFormularyStatus", "LevelOfPreferedness", "IsOTC", "SigID", "SigText",
            "ControlledSubstanceCode", "StateControlledSubstanceCode", "StateCSCodeForPractice", "GPPC", "PackageSize", "PackageUOM",
            "PackageQuantity", "PackageDescription", "Quantity", "QuantityFormattedString", "Refills", "DAW", "DaysSupply",
            "MedicationName", "Strength", "StrengthUOM", "RouteOfAdminCode", "RouteOfAdminDescription", "DosageFormCode",
            "DosageFormDescription", "Notes", "ICD9Code", "FormularyID",
            "FullDrugDescription", "NDC", "HasSupplyItem",
            "IsCompoundMed", "IsFreeFormMedControlSubstance","Status" }.ToList();
            PropertyInfo[] properties = typeof(Rx).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            //string code = string.Empty;
            //foreach (PropertyInfo p in properties)
            //{
            //    code += string.Format("{0}, ", p.Name);
            //}


            foreach (PropertyInfo propertyInfo in properties)
            {
                if (checkProperties.Contains(propertyInfo.Name))
                {
                    if (propertyInfo.CanRead)
                    {
                        object firstValue = propertyInfo.GetValue(inMemoryMed, null);
                        object secondValue = propertyInfo.GetValue(originalMed, null);
                        if (!object.Equals(firstValue, secondValue))
                        {
                            causeProperty = propertyInfo.Name;
                            break;
                        }
                    }
                }
            }


            return causeProperty;
        }

        protected void btnSelectOtpForm_Click(object sender, EventArgs e)
        {
            string exceptionID = null;
            bool showEPCSOverlay = true;
            OTPForm otpSelected = OtpFormList.ToList().Find(x => x.FormId == rblOtpForms.SelectedValue);

            //if the user has a 2FA that requires delivery and the user selected one of those options, trigger the delivery
            if (otpSelected.AllowsRequest)
            {
                bool requestOtpCallSucceeded;

                if (otpSelected != null)
                {
                    requestOtpCallSucceeded = EPSBroker.RequestOTP(
                    SessionLicenseID,
                    SessionUserID,
                    SessionShieldUserName,
                    otpSelected,
                    OtpFormTransactionID,
                    Request.UserIpAddress(),
                    ShieldSecurityToken,
                    EprescribeExternalAppInstanceID,
                    DBID,
                    out exceptionID);
                }
                else
                {
                    requestOtpCallSucceeded = false;
                }


                if (!requestOtpCallSucceeded)
                {
                    //
                    // handle failed request OTP call
                    //
                    showEPCSOverlay = false;

                    DigitalSigningEventArgs eventArgs = new DigitalSigningEventArgs(false);
                    string.Concat("The system could not send you your OTP at this time.  Please try again later. Exception Reference ID = ", exceptionID);

                    if (OnDigitalSigningComplete != null)
                    {
                        OnDigitalSigningComplete(eventArgs);
                    }
                }
            }

            if (showEPCSOverlay)
            {
                OtpForm = otpSelected;

                this.ShouldShowEpcsSignAndSendScreen();
            }
        }

        protected void btnCancelSecondFactorForm_Click(object sender, EventArgs e)
        {
            // just return to script pad page
            this.ShouldShowOtpForm = true;
            this.OtpFormList = null;
        }

        #endregion

        #region Public Custom Methods

        public bool ShouldShowEpcsSignAndSendScreen()
        {
            SessionTracker.AddControlTrack(this.ToString());
            lblAuthenticationStatus.Visible = false;
            bool showOverlay = true;

            // check if epcs med(s) have a ppt pharmacy
            if (IsScriptForNewRx)
            {
                _hasPharmacyChanges = PPTPlus.CheckForPharmacyChange(ControlState, DBID);
            }
            else if (IsApprovalRequestWorkflow)
            {
                var lastPharmacyID = ControlState.GetStringOrEmpty("LASTPHARMACYID");
                _hasPharmacyChanges = EPCSMEDList.Exists(x => x.PharmacyID != lastPharmacyID);
            }

            GetAndSetOtpFormsForUser();

            if (UserHasAtLeastOneValidOtpForm)
            {
                if (isPatientAddress1CityMissing())
                {
                    if (((BasePage)Page).SessionLicense.EnterpriseClient.EditPatient)
                    {
                        panelPatientDemographics.Visible = true;
                        panelEpcsEnterpriseEditPatientOff.Visible = false;
                    }
                    else
                    {
                        panelPatientDemographics.Visible = false;
                        panelEpcsEnterpriseEditPatientOff.Visible = true;
                    }

                    panelEPCSDigitalSigning.Visible = false;
                }
                else if (ShouldShowOtpFormAlert())
                {
                    BindOtpFormList();
                    pnlSecondFactorAlertPopUp.Visible = true;
                    panelPatientDemographics.Visible = false;
                    panelEpcsEnterpriseEditPatientOff.Visible = false;
                    panelEPCSDigitalSigning.Visible = false;
                }
                else
                {
                    showOverlay = initializeSecondFactorForm();
                    loadEPCSDigitalSigning();
                    panelPatientDemographics.Visible = false;
                    panelEpcsEnterpriseEditPatientOff.Visible = false;
                    pnlSecondFactorAlertPopUp.Visible = false;
                    panelEPCSDigitalSigning.Visible = true;
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), "disableKey", "disableBtnSubmitEPCS();", true);
                }

                if (showOverlay)
                {
                    mpeEPCS.Show();
                }
            }
            else
            {
                DigitalSigningEventArgs eventArgs = new DigitalSigningEventArgs();
                eventArgs.Success = false;
                eventArgs.Message = "Your second factor authentication forms could not be found.  You cannot electronically send controlled substances at this time.";
                if(MasterPage.RxTask?.TaskType == Constants.PrescriptionTaskType.REFREQ)
                {
                    ControlState["REFILLMSG"] = eventArgs.Message;
                }
                OnDigitalSigningComplete?.Invoke(eventArgs);
                showOverlay = false;
            }
            return showOverlay;
        }

        #endregion

        #region Private Custom Methods

        private void bindStateList()
        {
            DataTable dtLisState = RxUser.ChGetState(base.DBID);
            ddlState.DataSource = dtLisState;
            ddlState.DataTextField = "State";
            ddlState.DataValueField = "State";
            ddlState.DataBind();
        }

        private string buildAddress(object address1, object address2)
        {
            StringBuilder sbAddress = new StringBuilder();

            // Address1
            if (address1 != null && address1.ToString() != string.Empty)
            {
                sbAddress.Append(address1.ToString());
            }

            // Address2
            if (address2 != null && address2.ToString() != string.Empty)
            {
                if (sbAddress.Length > 0)
                {
                    sbAddress.Append(" ");
                }
                sbAddress.Append(address2.ToString());
            }

            return sbAddress.ToString();
        }

        private string buildCityStateZip(object city, object state, object zip)
        {
            StringBuilder sbCityStateZip = new StringBuilder();

            // City
            if (city != null && city.ToString() != string.Empty)
            {
                if (sbCityStateZip.Length > 0)
                {
                    sbCityStateZip.Append(", ");
                }
                sbCityStateZip.Append(city.ToString());
            }

            // State
            if (state != null && state.ToString() != string.Empty)
            {
                if (sbCityStateZip.Length > 0)
                {
                    sbCityStateZip.Append(", ");
                }
                sbCityStateZip.Append(state.ToString());
            }

            // Zip
            if (zip != null && zip.ToString() != string.Empty)
            {
                if (sbCityStateZip.Length > 0)
                {
                    sbCityStateZip.Append(", ");
                }
                sbCityStateZip.Append(zip.ToString());
            }

            return sbCityStateZip.ToString();
        }

        private bool isPatientAddress1CityMissing()
        {
            bool retval = false;
            var patientAddress1 = ControlState.GetStringOrEmpty("PATIENTADDRESS1");
            var patientCity = ControlState.GetStringOrEmpty("PATIENTCITY");

            if (patientAddress1.Equals(string.Empty) || patientCity.Equals(string.Empty))
            {
                bindStateList();

                tbFirstName.Text = ControlState.GetStringOrEmpty("PATIENTFIRSTNAME");
                tbLastName.Text = ControlState.GetStringOrEmpty("PATIENTLASTNAME");
                ddlGender.SelectedValue = ControlState["SEX"] == null ? "U" : ControlState.GetStringOrEmpty("SEX");

                tbDOB.MinDate = DateTime.Today.AddYears(-120);
                tbDOB.MaxDate = DateTime.Today;
                tbDOB.Culture = System.Globalization.CultureInfo.CurrentCulture;
                tbDOB.IncrementSettings.InterceptArrowKeys = false;
                tbDOB.IncrementSettings.InterceptMouseWheel = false;

                DateTime dateOfBirth = DateTime.MinValue;
                DateTime.TryParse(ControlState.GetStringOrEmpty("PATIENTDOB"), out dateOfBirth);

                if (dateOfBirth != DateTime.MinValue)
                {
                    tbDOB.SelectedDate = dateOfBirth;
                }

                tbAddress.Text = patientAddress1;
                tbAddress2.Text = ControlState.GetStringOrEmpty("PATIENTADDRESS2");
                tbCity.Text = patientCity;
                var state = ControlState.GetStringOrEmpty("PATIENTSTATE");
                if (ddlState.Items.FindByValue(state)  != null)
                {
                    ddlState.SelectedValue = state;
                }

                tbZip.Text = ControlState.GetStringOrEmpty("PATIENTZIP");

                retval = true;
            }

            return retval;
        }

        private bool ShouldShowOtpFormAlert()
        {
            bool shouldShow = false;

            if (ShouldShowOtpForm)
            {
                if (OtpFormList != null)
                {
                    if (OtpFormList.Any(form => form.Description.Equals(string.Empty)))
                    {
                        var optForms = new List<OTPForm>(OtpFormList);
                        optForms.RemoveAll(form => form.Description.Equals(string.Empty));
                        OtpFormList = optForms.ToArray();
                    }
                    if (OtpFormList.Length > 1)
                    {
                        shouldShow = true;
                    }
                }

                ShouldShowOtpForm = false;
            }

            return shouldShow;
        }

        private bool UserHasAtLeastOneValidOtpForm
        {
            get
            {
                return this.OtpFormList != null && this.OtpFormList.Length > 0;
            }
        }

        /// <summary>
        /// Gets the Verizon otp forms authentication types for the user and sets this control's viewstate properties
        /// </summary>
        private void GetAndSetOtpFormsForUser()
        {
            if (OtpFormList == null)
            {
                var otpResponse = EPSBroker.GetOtpFormsForSigning(ShieldSecurityToken, Request.UserIpAddress(), EprescribeExternalAppInstanceID, SessionShieldUserName);

                if (otpResponse.Success)
                {
                    OtpFormList = otpResponse.OTPForms;

                    if (OtpFormList.Length > 0)
                    {
                        OtpFormTransactionID = otpResponse.TransactionID;
                        OtpIdentityName = otpResponse.IdentityName;
                    }
                }
                else
                {
                    OtpFormList = null;
                    OtpFormTransactionID = null;
                }
            }
        }

        private void BindOtpFormList()
        {
            rblOtpForms.Items.Clear();
            string otpFormDescription = string.Empty;

            foreach (var otpForm in OtpFormList)
            {
                if (otpForm.ErxOtpType.Equals(OtpType.ZentryEmail) || otpForm.ErxOtpType.Equals(OtpType.ZentrySMS))
                {
                    otpFormDescription = GetOtpFormDescriptionWithDetails(otpForm);
                }
                else
                {
                    string displayName = !string.IsNullOrWhiteSpace(otpForm?.FormId) && otpForm.FormId.StartsWith("(Expires Soon)")
                                 ? ("(Expires Soon) " + otpForm.DisplayName)
                                 : otpForm.DisplayName;

                    otpFormDescription = displayName;
                }

                rblOtpForms.Items.Add(new ListItem(otpFormDescription, otpForm.FormId));
            }

            rblOtpForms.SelectedIndex = 0;

            rblOtpForms.DataBind();
        }

        private string GetOtpFormDescriptionWithDetails(OTPForm otpForm)
        {
            string newSecondFactorDescription = otpForm.Description;

		try
		{
			if (!string.IsNullOrWhiteSpace(otpForm.FormId))
			{
				string extraDetails = null;
                string otpFormId = otpForm.FormId.Replace("(Expires Soon)", "").Trim();

                    //
                    // Examples:
                    // 2625551234 = (xxx)xxx-1234
                    // john.smith@email.com = j****h@emaildomain.com
                    //
                if (otpForm.ErxOtpType.Equals(OtpType.ZentrySMS))
				{
                    extraDetails = string.Concat("(xxx)xxx-", otpFormId.Substring(8, 4));
                }
				else if (otpForm.ErxOtpType.Equals(OtpType.ZentryEmail))
				{
                    extraDetails = string.Concat(otpFormId.Substring(0, 1), "...", otpFormId.Substring(otpFormId.IndexOf("@") - 1, otpFormId.Length - (otpFormId.IndexOf("@") - 1)));
                }

				newSecondFactorDescription = string.Concat(newSecondFactorDescription, " ", extraDetails);
			}
		}
		catch
		{
			newSecondFactorDescription = otpForm.Description;
		}
        newSecondFactorDescription = !string.IsNullOrWhiteSpace(otpForm?.FormId) && otpForm.FormId.StartsWith("(Expires Soon)")
                                ? ("(Expires Soon) " + newSecondFactorDescription)
                                : newSecondFactorDescription;

        return newSecondFactorDescription;
	}

        /// <summary>
        /// Sets the second factor form. If the desired second factor form needs to be delivered to the client (e.g. via SMS test or email), this method will trigger that delivery.
        /// If the desired second factor form doesn't require delivery from Verizon (e.g. hard token device), this method sets the appropriate viewstate properties for later consumption.
        /// </summary>
        private bool initializeSecondFactorForm()
        {
            bool isInitializationSuccessful = false;

            if (OtpForm != null)
            {
                isInitializationSuccessful = true;
            }
            else if (OtpFormList != null && OtpFormList.Length == 1)
            {
                isInitializationSuccessful = true;

                OtpForm = OtpFormList[0];

                //if the desired second factor form needs to be delivered to the user, do it here
                if (OtpForm.AllowsRequest)
                {
                    string exceptionID = null;

                    var requestOtpCallSucceeded = EPSBroker.RequestOTP(
                        SessionLicenseID,
                        SessionUserID,
                        SessionShieldUserName,
                        OtpForm,
                        OtpFormTransactionID,
                        Request.UserIpAddress(),
                        ShieldSecurityToken,
                        EprescribeExternalAppInstanceID,
                        DBID,
                        out exceptionID);

                    if (!requestOtpCallSucceeded)
                    {
                        //
                        // handle failed request OTP call
                        //
                        DigitalSigningEventArgs eventArgs = new DigitalSigningEventArgs(false);
                        eventArgs.Message = string.Concat("The system could not send you your OTP at this time.  Please try again later. Exception Reference ID = ", exceptionID);

                        if (OnDigitalSigningComplete != null)
                        {
                            OnDigitalSigningComplete(eventArgs);
                        }

                        isInitializationSuccessful = false;
                    }
                }
            }

            return isInitializationSuccessful;
        }

        private void loadEPCSDigitalSigning()
        {
            // show date
            litDate.Text = StringHelper.ConvertToUxDate(DateTime.Now);

            // load DEA numbers
            _deaList = EPSBroker.GetProviderDEALicenses(base.SessionUserID, base.DBID);

            foreach (ePrescribeSvc.DEALicense deaNumber in _deaList)
            {
                if (deaNumber.DeaLicenseTypeId == DeaLicenseType.DefaultDea)
                {
                    litDEAList.Text = deaNumber.DEANumber.ToHTMLEncode();
                }
            }

            // load provder info
            DataSet dsSite = ApplicationLicense.SiteLoad(base.SessionLicenseID, base.SessionSiteID, base.DBID);

            string practiceInfo = string.Concat(
                dsSite.Tables[0].Rows[0]["SiteName"].ToString(),
                ", ",
                buildAddress(dsSite.Tables[0].Rows[0]["Address1"], dsSite.Tables[0].Rows[0]["Address2"]),
                ", ",
                buildCityStateZip(dsSite.Tables[0].Rows[0]["City"], dsSite.Tables[0].Rows[0]["State"], dsSite.Tables[0].Rows[0]["ZipCode"]));

            litProviderInfo.Text = string.Concat(base.SessionUserName, ", ", practiceInfo);

            // load supervisor info
            if (base.SessionUserType == Constants.UserCategory.PHYSICIAN_ASSISTANT_SUPERVISED)
            {
                litSupervisorInfo.Text = string.Concat(base.DelegateProviderName, ", ", practiceInfo);
                divSupervisor.Visible = true;
            }
            else
            {
                divSupervisor.Visible = false;
            }

            // load patient info
            litPatientInfo.Text = string.Concat(
                ControlState.GetStringOrEmpty("PATIENTFIRSTNAME"),
                " ",
                ControlState.GetStringOrEmpty("PATIENTLASTNAME"),
                ", ",
                buildAddress(ControlState["PATIENTADDRESS1"], ControlState["PATIENTADDRESS2"]),
                ", ",
                buildCityStateZip(ControlState["PATIENTCITY"], ControlState["PATIENTSTATE"], ControlState["PATIENTZIP"]));

            // load pharmacy
            if (EPCSMEDList.Any(e => e.Destination == "PHARM") & EPCSMEDList.Any(e => e.Destination == "MOB") || _hasPharmacyChanges)
            {
                // if there are multiple cs meds and they are being sent to both Retail and Mail Order pharmacies then show pharmacy info in grid rows.
                divPharmacy.Visible = false;
                _showPharmacyInGrid = true;
            }
            else
            {
                _showPharmacyInGrid = false;

                var taskScriptMessageId = ControlState.GetStringOrEmpty(Constants.SessionVariables.TaskScriptMessageId);
                if (taskScriptMessageId != string.Empty || PharmacyTask != null) //refreq or ChangeRx workflow
                {
                    ScriptMessage sm = null;
                    if (taskScriptMessageId != string.Empty)
                    {
                        sm = new ScriptMessage(taskScriptMessageId, ControlState.GetStringOrEmpty("LICENSEID"), ControlState.GetStringOrEmpty("USERID"), base.DBID);
                        ViewState["IsRenewalApprovalWorkflow"] = true;
                    }
                    else if (PharmacyTask != null)
                    {
                        sm = PharmacyTask.ScriptMessage as ScriptMessage;
                    }

                    if (sm != null)
                    {
                        litPharmacy.Text = Allscripts.Impact.Utilities.StringHelper.GetPharmacyName(
                            sm.PharmacyName,
                            sm.PharmacyAddress1,
                            sm.PharmacyAddress2,
                            sm.PharmacyCity,
                            sm.PharmacyState,
                            sm.PharmacyZip,
                            sm.PharmacyPhoneNumber).ToHTMLEncode();

                        divPharmacy.Visible = true;
                    }
                    else if (!string.IsNullOrWhiteSpace(ControlState.GetStringOrEmpty(Constants.SessionVariables.LastPharmacyName)))
                    {
                        litPharmacy.Text = ControlState.GetStringOrEmpty(Constants.SessionVariables.LastPharmacyName);
                        divPharmacy.Visible = true;
                    }
                }
                else
                {
                    if (EPCSMEDList.Any(e => e.Destination == "PHARM"))
                    {
                        // if cs meds are being sent to Retail pharmacy then display pharmacy info, as long as it is in the session variable
                        var lastPharmacyName = ControlState.GetStringOrEmpty(Constants.SessionVariables.LastPharmacyName);
                        if (lastPharmacyName != string.Empty)
                        {
                            litPharmacy.Text = lastPharmacyName;
                            divPharmacy.Visible = true;
                        }
                    }
                    else if (EPCSMEDList.Any(e => e.Destination == "MOB"))
                    {
                        // if cs meds are being sent to Mail Order pharmacy then display pharmacy info, as long as it is in the session variable
                        var mob_Name = ControlState.GetStringOrEmpty("MOB_Name");
                        if (mob_Name != string.Empty)
                        {
                            litPharmacy.Text = mob_Name;
                            divPharmacy.Visible = true;
                        }
                    }
                }
            }

            // sort the list to show CS meds being sent to pharmacy first
            if (_showPharmacyInGrid)
            {
                //epcsMedList.Sort((med1, med2) => string.Compare(med2.Destination, med1.Destination, true));
                grdEPCSMedList.DataSource = EPCSMEDList.OrderByDescending(med => med.Destination).ThenBy(med => med.MedicationName);
            }
            else
            {
                grdEPCSMedList.DataSource = EPCSMEDList.OrderBy(med => med.MedicationName);
            }

            grdEPCSMedList.DataBind();

            grdEPCSMedList.MasterTableView.GetColumnSafe("ClientSelectColumn").HeaderStyle.Width = System.Web.UI.WebControls.Unit.Percentage(5);
            grdEPCSMedList.MasterTableView.GetColumnSafe("Quantity").HeaderStyle.Width = System.Web.UI.WebControls.Unit.Percentage(10);
            grdEPCSMedList.MasterTableView.GetColumnSafe("DAW").HeaderStyle.Width = System.Web.UI.WebControls.Unit.Percentage(7);
            grdEPCSMedList.MasterTableView.GetColumnSafe("Refills").HeaderStyle.Width = System.Web.UI.WebControls.Unit.Percentage(10);
            grdEPCSMedList.MasterTableView.GetColumnSafe("Days").HeaderStyle.Width = System.Web.UI.WebControls.Unit.Percentage(7);

            // check if we need to show Effective Date column
            if (_showEffectiveDateColumn)
            {
                grdEPCSMedList.MasterTableView.GetColumnSafe("EffectiveDate").HeaderStyle.Width = System.Web.UI.WebControls.Unit.Percentage(20);
                grdEPCSMedList.MasterTableView.GetColumnSafe("EffectiveDate").Display = true;
            }
            else
            {
                grdEPCSMedList.MasterTableView.GetColumnSafe("EffectiveDate").Display = false;
            }

            // load username
            litUserName.Text = ((BasePage)Page).SessionLoginID;
        }


        #endregion
        protected void grdEPCSMedList_PreRender(object sender, EventArgs e)
        {
            RadGrid grid = (RadGrid)sender;
            GridTableView masterTableView = grid.MasterTableView;
            if (masterTableView.Height.Value > 150 || (_showPharmacyInGrid && masterTableView.Items.Count > 2) || (!_showPharmacyInGrid && masterTableView.Items.Count > 4))
            {
                grid.ClientSettings.Scrolling.AllowScroll = true;
                grid.CssClass = "epcs-sign";
                grid.ClientSettings.Scrolling.UseStaticHeaders = true;
            }
            foreach (GridHeaderItem header in grdEPCSMedList.MasterTableView.GetItems(GridItemType.Header))
            {
                CheckBox chkbx = (CheckBox)header["ClientSelectColumn"].Controls[0];

                chkbx.Visible = false;
            }
        }
    }

}