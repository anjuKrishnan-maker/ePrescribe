using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Text;
using System.Xml;
using Telerik.Web.UI;
using Allscripts.Impact;
using Allscripts.Impact.Interfaces;
using Allscripts.ePrescribe.Common;
using System.Linq;
using Allscripts.ePrescribe.Data;
using Allscripts.ePrescribe.ExtensionMethods;
using Allscripts.ePrescribe.Medispan.Clinical.Model.Settings;
using Allscripts.ePrescribe.Objects;
using eRxWeb.AppCode;
using eRxWeb.AppCode.Interfaces;
using Allscripts.ePrescribe.Shared.Logging;
using Patient = Allscripts.Impact.Patient;
using Provider = Allscripts.Impact.Provider;
using Rx = Allscripts.Impact.Rx;
using ScriptMessage = Allscripts.Impact.ScriptMsg.ScriptMessage;
using ConfigKeys = Allscripts.Impact.ConfigKeys;
using eRxWeb.AppCode.Tasks;
using Allergy = Allscripts.ePrescribe.Medispan.Clinical.Model.Allergy;
using Constants = Allscripts.ePrescribe.Common.Constants;
using DURSettings = Allscripts.ePrescribe.Medispan.Clinical.Model.Settings.DURSettings;
using Allscripts.ePrescribe.Medispan.Clinical.Model.Response;
using Allscripts.ePrescribe.Medispan.Clinical.Model.Request;
using Allscripts.Impact.ePrescribeSvc;
using Allscripts.Impact.Tasks;
using Allscripts.Impact.Tasks.Interfaces;
using ConnectionStringPointer = Allscripts.ePrescribe.DatabaseSelector.ConnectionStringPointer;
using DenialReason = eRxWeb.ePrescribeSvc.DenialReason;
using DUR = Allscripts.Impact.DUR;
using FormularyStatus = Allscripts.Impact.FormularyStatus;
using PatientCoverage = Allscripts.Impact.PatientCoverage;
using Prescription = Allscripts.Impact.Prescription;
using Allscripts.Impact.Utilities;

namespace eRxWeb
{
    public partial class PharmRefillDetails : BasePage
    {
        #region Private Variables

        private const string SELECT_DENIAL_REASON = "Select a Denial Reason";
        private const string DENIAL_REASON_ERROR = "Please select a Denial Reason.";
        private const string PRIOR_AUTH_CODE_ERROR = "Please enter a valid prior auth code.";
        string hdnDDI = string.Empty;
        private static ILoggerEx logger = LoggerEx.GetLogger();
        // bool isCMed = false;
        string selectedFormularyStatus = string.Empty;
        private IPrescription iPrescription = new Prescription();

        private DataSet taskResults
        {
            get
            {
                DataSet ds = null;

                if (ViewState["TaskResults"] == null)
                {
                    ds = Provider.GetPharmRefillDetails(Request.Params["MessageID"], base.SessionLicenseID, base.SessionUserID, base.DBID);
                    ViewState["TaskResults"] = ds;
                }

                return (DataSet)ViewState["TaskResults"];
            }
            set
            {
                ViewState["TaskResults"] = value;
            }
        }


        #endregion Private Variables
        public enum POBWorkflowType
        {
            POB_REGULAR_DUP_ONLY_DUR,
            POB_SOME_OR_ALL_REVIEW_REQUIRED_DUR,
            POB_ANY_NON_DUR,
            POB_UNEXPECTED_WORKFLOW

        }
        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                PageState.Remove(Constants.SessionVariables.IsReconcileREFREQNonCS);

                btnCheckRegistry.OnClientClick = "OpenNewWindow" + "('" + PageState.GetStringOrEmpty("STATEREGISTRYURL") + "')";

                GetRefillTask();
                grdPharmRefillDetails.Height = ((PhysicianMasterPage)Master).getTableHeight();

                ucMessage.Visible = true;
                ucMessage.MessageText = "'Deny' to refuse the refill request. 'Approve' to approve as it was originally written.";
                ucMessage.Icon = Controls_Message.MessageType.INFORMATION;

                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "RefreshPatientHeader", $"RefreshPatientHeader('{Request.QueryString[Constants.QueryStringParameters.PatientID]}'); ", true);

                if(Request.QueryString["RefillMessage"]== PRIOR_AUTH_CODE_ERROR)
                {
                    ucMessage2.Visible = true;
                    ucMessage2.MessageText = PRIOR_AUTH_CODE_ERROR;
                    ucMessage2.Icon = Controls_Message.MessageType.INFORMATION;
                }
                // refill request is for a supervised physician assistant, show the select supervising provider
                if (Request.QueryString["ShowSupProv"] != null && Request.QueryString["ShowSupProv"].ToString() == "Y")
                {
                    setSupervisingProviderMessage();
                }
            }

            this.ucCSMedRefillRequestNotAllowed.OnPrintRefillRequest += new EventHandler(ucCSMedRefillRequestNotAllowed_OnPrintRefillRequest);
            this.ucCSMedRefillRequestNotAllowed.OnContactProvider += new EventHandler(ucCSMedRefillRequestNotAllowed_OnContactProvider);

            this.ucMedicationHistoryCompletion.OnMedHistoryComplete += new Controls_MedicationHistoryCompletion.MedHistoryCompletionHandeler(ucMedicationHistoryCompletion_OnMedHistoryComplete);

        }


        protected void Page_LoadComplete(object sender, EventArgs e)
        {
            int tasks = 0;

            ((PhysicianMasterPage)Master).toggleTabs("tasks", tasks);
        }

        protected void grdPharmRefillDetails_OnNeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            grdPharmRefillDetails.DataSource = taskResults;

            if (taskResults != null && taskResults.Tables != null && taskResults.Tables.Count > 0)
            {
                if (taskResults.Tables[0].Rows.Count == 1)
                {
                    secureProcessingControl.MessageText = Controls_SecureProcessing.SecureProcessingMessages.SingleRx;
                }
                else if (taskResults.Tables[0].Rows.Count > 1)
                {
                    secureProcessingControl.MessageText = Controls_SecureProcessing.SecureProcessingMessages.MultipleRx;
                }
            }
        }

        protected void btnProcess_Click(object sender, EventArgs e)
        {
            GridDataItem item = grdPharmRefillDetails.MasterTableView.Items[0];

            if (item != null)
            {
                string action = string.Empty;

                RadioButton rdbYes = (RadioButton)item.FindControl("rdbYes");
                RadioButton rdbNo = (RadioButton)item.FindControl("rdbNo");


                RxTaskModel pharmacyTask = new RxTaskModel {DbId = DBID};
                if ((item.FindControl("lblType") as Label)?.Text == RxChangeSubTypeDesciption.PriorAuth)
                {
                    pharmacyTask.TaskType = Constants.PrescriptionTaskType.RXCHG_PRIORAUTH;
                }
                else
                {
                    pharmacyTask.TaskType = (Constants.PrescriptionTaskType)Convert.ToInt32(item.GetDataKeyValue("TaskType"));
                }
                if (rdbNo != null && rdbNo.Checked)
                {
                    action = CommonTerms.No;
                }
                else
                {
                    if (pharmacyTask.TaskType == Constants.PrescriptionTaskType.RXCHG)
                    {
                        var selectedRdbId = Request.Form[UiHelper.GetDynamicNameSuffixOfAspControl(rdbNo.ClientID).Replace('_', '$') + "select"];
                        if (selectedRdbId != null && !selectedRdbId.Equals("rdbNo"))
                        {
                            action = CommonTerms.Yes;
                            pharmacyTask.RequestedRxIndexSelected = Convert.ToInt16(selectedRdbId.Substring(16));
                        }
                    }
                    else
                    {
                        if (rdbYes != null && rdbYes.Checked)
                        {
                            action = CommonTerms.Yes;
                        }
                    }
                }

                if (!string.IsNullOrEmpty(action))
                {

                    DropDownList ddl = item.FindControl("DenialCodes") as DropDownList;
                    if (ddl != null)
                    {
                        pharmacyTask.DenialCode = ddl.SelectedItem.Value;
                        pharmacyTask.DenialText = ddl.SelectedItem.Text;
                    }

                    if (action.Equals(CommonTerms.No) && pharmacyTask.DenialText == SELECT_DENIAL_REASON)
                    {
                        ucMessage2.MessageText = DENIAL_REASON_ERROR;
                        ucMessage2.Icon = Controls_Message.MessageType.ERROR;
                        ucMessage2.Visible = true;

                        ddl.Style["Display"] = "inline";
                    }
                    else
                    {
                        pharmacyTask.RxGUID = string.Empty;
                        pharmacyTask.RxTaskId = Int64.Parse(item.GetDataKeyValue("RxTaskID").ToString());
                        pharmacyTask.ScriptMessageGUID = item.GetDataKeyValue("scriptMessageID").ToString();
                        pharmacyTask.PatientId = item.GetDataKeyValue("patientID").ToString();
                        TextBox notesBox = item.FindControl("notesText") as TextBox;
                        pharmacyTask.PhysicianComments = (notesBox != null)?notesBox.Text:string.Empty;
                        pharmacyTask.IsPatientVisitRq = Constants.CommonAbbreviations.NO;
                        if (action == CommonTerms.No)
                        {
                            pharmacyTask.RxRequestType = RequestType.DENY;
                        }
                        else if (action == CommonTerms.Yes)
                        {
                            pharmacyTask.RxRequestType = RequestType.APPROVE;
                        }
                        var paCode = item.FindControl("txtPAAprroveCode") as TextBox;
                        pharmacyTask.PriorAuthCode = paCode?.Text;
                        Image imgCSMed = (Image)item.FindControl("imgCSMed");

                        //determine if this is a CS med
                        if (imgCSMed != null && imgCSMed.Visible)
                        {
                            pharmacyTask.IsControlledSubstance = true;

                            if (imgCSMed.Attributes["ReconciledCSCode"] != null)
                            {
                                pharmacyTask.ReconciledControlledSubstanceCode = int.Parse(imgCSMed.Attributes["ReconciledCSCode"].ToString());
                            }
                        }

                        CheckBox cb = item.FindControl("cbSendToADM") as CheckBox;

                        if (cb != null && cb.Checked)
                        {
                            ScriptMessage.SendNotificationTask(null, Session["UserID"].ToString(), Session["LicenseID"].ToString(), pharmacyTask.PatientId, pharmacyTask.ScriptMessageGUID, base.DBID);
                        }

                        processTask(pharmacyTask, item, PageState[Constants.SessionVariables.ShieldSendRxAuthToken]?.ToString());
                    }
                }
                else
                {
                    ucMessage2.MessageText = "Please select an action to process the refill.";
                    ucMessage2.Icon = Controls_Message.MessageType.ERROR;
                    ucMessage2.Visible = true;
                }
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            base.ClearPatientInfo();
            Response.Redirect(Constants.PageNames.PHARMACY_REFILL_SUMMARY, true);
        }

        private GridDataItem _tempDataItem;
        public LinkButton lbRegisterPatient => _tempDataItem?.FindControl("lbRegisterPatient") as LinkButton;
        public LinkButton lbReconcileRx => _tempDataItem?.FindControl("lbReconcileRx") as LinkButton;
        public LinkButton lbPrintCsRx => _tempDataItem?.FindControl("lbPrintCsRx") as LinkButton;
        public LinkButton lbChangeRequest => _tempDataItem?.FindControl("lbChangeRequest") as LinkButton;
        public LinkButton lbChangePatient => _tempDataItem?.FindControl("lbChangePatient") as LinkButton;
        public Label notesLabel => _tempDataItem?.FindControl("notesLabel") as Label;
        public TextBox notes => _tempDataItem?.FindControl("notesText") as TextBox;
        public HtmlControl divMaxCharacters => _tempDataItem?.FindControl("divMaxCharacters") as HtmlControl;
        public Label lblType => _tempDataItem?.FindControl("lblType") as Label;
        public RadioButton rdbYes => _tempDataItem.FindControl("rdbYes") as RadioButton;
        public RadioButton rdbNo => _tempDataItem.FindControl("rdbNo") as RadioButton;
        public DropDownList ddlDenialCodes => _tempDataItem.FindControl("DenialCodes") as DropDownList;
        public TextBox txtPaApprovalCode => _tempDataItem.FindControl("txtPAAprroveCode") as TextBox;
        public Label lblPaApproveCode => _tempDataItem.FindControl("lblPAApproveCode") as Label;

        protected void grdPharmRefillDetails_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                GridDataItem tempDataItem = _tempDataItem = (GridDataItem)e.Item;
                if (IsAnyControlNull())
                    return;

                #region initVars
                var prescriptionModel = new TaskRxDetailsModel();
                string pharmacyState = string.Empty;
                DataTable dtRefillRequest = ((DataRowView)tempDataItem.DataItem).DataView.Table;
                DataRow dr = dtRefillRequest.Rows[tempDataItem.ItemIndex];
                DispensedRx dispensedRx = null;
                ChangeRxSmInfo changeRxInfo = null;
                var changeRxPrescriberAuth = new ChgRxPrescriberAuth(new ChangeRequestSubCodes());
                #endregion


                SetVisibilityOnControls(false);

                var taskType = tempDataItem.GetDataKeyValue("TaskType").ToEnum<Constants.PrescriptionTaskType>();

                bool isCSMed = false;
                displayFormularyStatus(tempDataItem);

                XmlDocument messageDoc = new XmlDocument();

                if (tempDataItem.GetDataKeyValue("MessageData") != null &&
                    !string.IsNullOrEmpty(tempDataItem.GetDataKeyValue("MessageData").ToString()))
                {
                    messageDoc.LoadXml(tempDataItem.GetDataKeyValue("MessageData").ToString());
                }

                Int64 taskID = Int64.Parse(tempDataItem.GetDataKeyValue("RxTaskID").ToString());

                var smNodes = new ScriptMessageNodes(messageDoc);

                if (taskType == Constants.PrescriptionTaskType.RXCHG)
                {
                    if (smNodes.Prescription != null)
                    {
                        dispensedRx = new DispensedRx(smNodes.Prescription);
                    }
                    changeRxInfo = new ChangeRxSmInfo(smNodes, dr.GetValue("ScriptMessageID").ToGuidOr0x0(), changeRxPrescriberAuth);

                    lblType.Text = changeRxInfo.SubType.Description;
                    if (rdbYes != null && rdbNo != null && changeRxInfo.SubType.Code != RxChangeSubTypeCode.PriorAuth)
                    {
                        rdbYes.Visible = false;
                    }
                    else if (rdbYes != null && changeRxInfo.SubType.Code == RxChangeSubTypeCode.PriorAuth)
                    {
                        ChangeRxTask.AddOnclickForRadioButtons(rdbYes, rdbNo, txtPaApprovalCode, lblPaApproveCode, ddlDenialCodes);
                    }
                }
                else
                {
                    if (smNodes.DispensedRx != null)
                    {
                        dispensedRx = new DispensedRx(smNodes.DispensedRx);
                    }
                }

                prescriptionModel = TaskDisplay.CreateRxDetail(smNodes, Convert.ToString(dr["FormDescription"]), Allscripts.Impact.Utilities.StringHelper.GetPharmacyName(messageDoc));

                //Add pharmacy and prescriber to dispensedRx Node
                dispensedRx.PharmacyDetails = prescriptionModel.PharmacyDetails;
                dispensedRx.ProviderOfRecord = prescriptionModel.ProviderOfRecord;
                
                string patientID = tempDataItem.GetDataKeyValue("patientID").ToString().Trim();

                bool isGeneric = true;

                string ddi = dr["DDI"].ToString();
                if (!(string.IsNullOrEmpty(ddi)))
                {
                    isGeneric = Allscripts.Impact.Medication.IsGenericMed(ddi, base.DBID);
                }


                Label medicationRquested = (Label)tempDataItem.FindControl("lblRxDetails");

                Image imgCSMed = (Image)tempDataItem.FindControl("imgCSMed");

                if (imgCSMed != null)
                {
                    int reconciledCSCode = 0;
                    isCSMed = showControlledSubstanceIndicator(ddi, pharmacyState, out reconciledCSCode);
                    imgCSMed.Visible = isCSMed;
                    lblCSMed.Visible = isCSMed;

                    if (isCSMed && taskType == Constants.PrescriptionTaskType.REFREQ)
                    {
                        setCSRegistryCheckbox();
                        imgCSMed.Attributes.Add("ReconciledCSCode", reconciledCSCode.ToString());
                        rdbYes.Enabled = false;

                        if (patientID != Guid.Empty.ToString())
                        {
                            lbPrintCsRx.Visible = isCSMed;
                            lbPrintCsRx.Attributes.Add("messageID", dr["ScriptMessageID"].ToString());
                            lbPrintCsRx.Attributes.Add("taskID", taskID.ToString());
                            lbPrintCsRx.Attributes.Add("taskType", Convert.ToString((int)Constants.PrescriptionTaskType.REFREQ));
                            lbPrintCsRx.Attributes.Add("DDI", ddi);
                            lbPrintCsRx.Attributes.Add("DelegateProviderID", dr["ProviderID"].ToString());
                        }

                        //
                        // TODO: check if the provider on the refill has DEA permissions for CS schedule for the med on refill request
                        //
                    }
                    else
                    {
                        chkRegistryChecked.Visible = false;
                    }
                }

                string patientName = string.Concat(dr["PatientFirstName"].ToString().ToHTMLEncode(), " ", dr["PatientLastName"].ToString().ToHTMLEncode());

                if (string.IsNullOrEmpty(patientID) || patientID.Equals(Guid.Empty.ToString()))
                {
                    patientName += " [Unreconciled]";
                }
                else
                {
                    //set the patientName to null because it will already be displayed in the header
                    patientName = null;
                }

                if (taskType == Constants.PrescriptionTaskType.REFREQ)
                {
                    medicationRquested.Text = RefillTaskDisplay.BuildDispensedMedMarkup(patientName, dispensedRx);
                    rdbYes.Enabled = StringUtil.GetIntFromString(dispensedRx?.Refills, 0) > 0;
                }
                else
                {
                    medicationRquested.Text = TaskDisplay.GetFormattedRxDetails(patientName, isGeneric, prescriptionModel);
                }

                if (!string.IsNullOrEmpty(patientID) && patientID.Equals(Guid.Empty.ToString()))
                {
                    //
                    //patient needs reconcilation
                    //

                    if (rdbYes != null)
                    {
                        rdbYes.Enabled = false;
                        chkRegistryChecked.Visible = false;
                    }

                    if (rdbNo != null)
                    {
                        rdbNo.Enabled = true;
                    }

                    //inject specific attributes into the linkbutton for the downstream logic to consume
                    lbRegisterPatient.Visible = true;
                    lbRegisterPatient.Attributes.Add("messageID", dr["ScriptMessageID"].ToString());
                    lbRegisterPatient.Attributes.Add("taskType", Convert.ToString((int)Constants.PrescriptionTaskType.REFREQ));
                }
                else if (dr["DDI"].ToString() == Convert.DBNull.ToString() || dr["DDI"].ToString().Length == 0)
                {
                    //
                    // med needs reconciliation; we couldn't match the request to a DDI in our system
                    //

                    if (rdbYes != null)
                    {
                        rdbYes.Enabled = false;
                        //chkRegistryChecked.Visible = false;
                    }

                    lbReconcileRx.Visible = true;
                    lbReconcileRx.Attributes.Add("messageID", dr["ScriptMessageID"].ToString());
                    lbReconcileRx.Attributes.Add("taskType", Convert.ToString((int)taskType));
                    lbReconcileRx.Attributes.Add("DDI", dr["DDI"].ToString());
                    lbReconcileRx.Attributes.Add("DelegateProviderID", dr["ProviderID"].ToString());
                }
                else if (dr["DDI"].ToString().Length > 0)
                {
                    lbChangeRequest.Visible = true;
                    lbChangeRequest.Attributes.Add("messageID", dr["ScriptMessageID"].ToString());

                    lbChangePatient.Visible = true;
                    lbChangePatient.Attributes.Add("taskType", Convert.ToString((int)Constants.PrescriptionTaskType.REFREQ));
                    lbChangePatient.Attributes.Add("messageID", dr["ScriptMessageID"].ToString());
                }

                if (IsReceivedPrescriptionInfoAvailable(changeRxInfo?.RequestedRxs, dispensedRx))
                {
                    Label dispensedLabel = (Label)tempDataItem.FindControl("lblDispensedRxDetails");

                    if (dispensedLabel != null && rdbNo != null && rdbYes != null && ddlDenialCodes != null)
                    {
                        if (taskType == Constants.PrescriptionTaskType.REFREQ)
                        {
                            dispensedLabel.Text = RefillTaskDisplay.BuildOriginalPrescriptionMarkup(prescriptionModel);
                        }
                        else
                        {
                            dispensedLabel.Text = ChangeRxTask.GetDispensedLabelText(taskType, changeRxInfo, dispensedRx, 
                                AppCode.ApproveRefillTask.BoxUpUiControls(tempDataItem), rdbYes.Enabled,
                                AppCode.StateUtils.UserInfo.IsPermissionGranted(UserPermissions.EpcsCanPrescribe, PageState), SessionPracticeState, 
                                pharmacyState, new ChangeRxTask(), changeRxPrescriberAuth, prescriptionModel, DBID);
                        }
                    }
                }

                ddlDenialCodes.Style["Display"] = "none";
                ddlDenialCodes.DataSource = EPSBroker.GetDenialReasons(taskType, base.DBID);
                ddlDenialCodes.DataTextField = "Name";
                ddlDenialCodes.DataValueField = "ID";
                ddlDenialCodes.DataBind();
                ddlDenialCodes.Items.Insert(0, new ListItem(SELECT_DENIAL_REASON, "-1"));

                if (divMaxCharacters.Visible)
                {
                    HtmlControl charsRemaining = (HtmlControl)tempDataItem.FindControl("charsRemaining");
                    notes.Attributes.Add("onkeydown", "checkFieldLength(this,'" + charsRemaining.ClientID + "', 70);");
                    notes.Attributes.Add("onkeyup", "checkFieldLength(this,'" + charsRemaining.ClientID + "', 70);");
                    notes.Attributes.Add("onmouseover", "checkFieldLength(this,'" + charsRemaining.ClientID + "', 70);");
                    notes.Attributes.Add("onchange", "checkFieldLength(this,'" + charsRemaining.ClientID + "', 70);");
                }
                AppCode.ApproveRefillTask.VerifyDispensedQuantityAndUpdateUi(dispensedRx, tempDataItem["ActionColumn"]?.Controls, rdbYes, new Allscripts.Impact.Telerik(tempDataItem), new AppCode.ApproveRefillTask());

                int formularyStatus = -1;
                var taskScriptMessageID = string.Empty;

                if (Session["DRRefillTask"] != null)
                {
                    var rtl = PageState.Cast("DRRefillTask", new RefillTaskLight());
                    formularyStatus = rtl.FormularyStatus;
                    taskScriptMessageID = rtl.ScriptMessageID;
                }

                if (rdbNo != null && rdbNo.Attributes["onclick"] == null)
                {
                    rdbNo.Attributes.Add("onclick", "setControlStateForRow('" + ddlDenialCodes.ClientID + "', " + rdbNo.ClientID + ", '" + notesLabel.ClientID + "', '" + notes.ClientID + "', '" + divMaxCharacters.ClientID + "'," + (!isCSMed && rdbYes.Visible  ? rdbYes.ClientID : "null") + ",'" + ddi + "','"+ formularyStatus + "','"+ taskScriptMessageID + "','" + taskType.ToString() + "');");
                }

                if (rdbYes != null && rdbYes.Attributes["onclick"] == null)
                {
                    rdbYes.Attributes.Add("onclick", "setControlStateForRow('" + ddlDenialCodes.ClientID + "', document.getElementById('" + rdbNo.ClientID + "').checked, '" + notesLabel.ClientID + "', '" + notes.ClientID + "', '" + divMaxCharacters.ClientID + "'," + (rdbYes.Visible ? rdbYes.ClientID : "null") + ",'" + ddi + "','" + formularyStatus + "','" + taskScriptMessageID + "');");
                }
                if (taskType == Constants.PrescriptionTaskType.RXCHG || taskType == Constants.PrescriptionTaskType.RXCHG_PRIORAUTH)
                {
                    lbChangePatient.Visible = false;
                    lbChangeRequest.Visible = false;
                }
                //Remove POB user's ability to approve CS Meds from task page
                if (isCSMed && UserRoles.IsPob((Constants.UserCategory)Session["UserType"]))
                {
                    rdbYes.Enabled = false;
                    rdbYes.Visible = false;           
                }
            }
        }

        private bool IsReceivedPrescriptionInfoAvailable(List<RequestedRx> requestedRxList, DispensedRx dispensedRx)
        {
            bool isAvailable = false;

            if (requestedRxList != null && requestedRxList.Count > 0)
            {
                isAvailable = requestedRxList.Any(
                            x => !string.IsNullOrEmpty(x.Description) ||
                            x.Quantity != 0 ||
                            !string.IsNullOrEmpty(x.Refills) ||
                            !string.IsNullOrEmpty(x.SigText));
            }
            else
            {
                if (dispensedRx != null)
                {
                    isAvailable = !string.IsNullOrEmpty(dispensedRx.Description) ||
                                    dispensedRx.Quantity != 0 ||
                                    !string.IsNullOrEmpty(dispensedRx.Refills) ||
                                    !string.IsNullOrEmpty(dispensedRx.SigText);
                }
            }

            return isAvailable;
        }

        private void SetVisibilityOnControls(bool visible)
        {
            var display = visible ? "inline" : "none";
            lbRegisterPatient.Visible = visible;
            lbReconcileRx.Visible = visible;
            lbPrintCsRx.Visible = visible;
            lbChangeRequest.Visible = visible;
            lbChangePatient.Visible = visible;
            lblPaApproveCode.Style["Display"] = display;
            txtPaApprovalCode.Style["Display"] = display;
            notesLabel.Style["Display"] = display;
            notes.Style["Display"] = display;
            divMaxCharacters.Style["Display"] = display;
        }

        private bool IsAnyControlNull()
        {
            return lbRegisterPatient == null
                    || lbReconcileRx == null
                    || lbPrintCsRx == null
                    || lbChangeRequest == null
                    || lbChangePatient == null
                    || notesLabel == null
                    || notes == null
                    || divMaxCharacters == null
                    || lblType == null
                    || rdbNo == null
                    || rdbYes == null
                    || ddlDenialCodes == null;
        }

        private void displayFormularyStatus(GridDataItem tempDataItem)
        {
            Image img = tempDataItem.FindControl("Image1") as Image;

            if (img != null)
            {
                bool isOTC;
                int ahsFormularyStatus;

                isOTC = false;
                ahsFormularyStatus = 0;
                Guid ScriptMessageID = new Guid((Request.Params["MessageID"]));
                if (Session["DRRefillTask"] != null)
                {
                    var rtl = Session["DRRefillTask"] as RefillTaskLight;

                    isOTC = rtl.IsOTC;
                    ahsFormularyStatus = rtl.FormularyStatus;
                }

                img.Style["cursor"] = "pointer";

                string imgPath = string.Empty;
                string toolTip = string.Empty;

                MedicationSearchDisplay.GetFormularyImagePathWithTooltip(ahsFormularyStatus, isOTC, out imgPath, out toolTip);

                img.ImageUrl = imgPath;
                img.ToolTip = toolTip;                
            }
        }

        private void GetRefillTask()
        {
            Session.Remove("DRRefillTask");
            DataSet ds = PatientCoverage.GetCoverageByPatientID(Convert.ToString(Session["PATIENTID"]), base.SessionLicenseID, base.SessionUserID, base.DBID);
            if (ds.Tables[0].Rows.Count > 0)
            {
                string scoverageid = ds.Tables[0].Rows[0]["SelectedCoverageID"].ToString();
                string sformularyID = Convert.ToString(Session["FormularyID"]);
                string sotcCoverage = Convert.ToString(Session["OTCCoverage"]);
                string sgenericDrugPolicy = Convert.ToString(Session["GenericDrugPolicy"]);
                string sunlistedDrugPolicy = Convert.ToString(Session["UnListedDrugPolicy"]);
                string sLICENSEID = Convert.ToString(Session["LICENSEID"]);
                string sPATIENTID = Convert.ToString(Session["PATIENTID"]);
                string sProviderID = string.Empty;
                if (Request.QueryString["ProviderID"] != null)
                {
                    sProviderID = Request.QueryString["ProviderID"].ToString();
                }
                DataRow DRRefillTask = Provider.GetRefillDetailByScriptMessageID(sLICENSEID, new Guid((Request.Params["MessageID"])).ToString(), "N", scoverageid, sformularyID, sotcCoverage, sgenericDrugPolicy, sunlistedDrugPolicy, base.DBID);
                Session["DRRefillTask"] = RefillTaskLight.FromDataRow(DRRefillTask);
            }
        }

        protected void grdPharmRefillDetails_ItemCommand(object source, GridCommandEventArgs e)
        {
            LinkButton lb = (LinkButton)e.CommandSource;

            if (e.CommandName == "RegisterPatient")
            {
                if (lb.HasAttributes)
                {
                    string scriptMessageID = lb.Attributes["messageID"];
                    Allscripts.ePrescribe.Common.Constants.PrescriptionTaskType taskType = (Allscripts.ePrescribe.Common.Constants.PrescriptionTaskType)Convert.ToInt32(lb.Attributes["tasktype"]);

                    Session[Constants.SessionVariables.TaskScriptMessageId] = scriptMessageID;
                    Session["TASKTYPE"] = taskType;

                    Response.Redirect(string.Concat(Constants.PageNames.APPROVE_SCRIPT_MESSAGE_PATIENT + "?smid=", scriptMessageID, "&From=" + Constants.PageNames.PHARMACY_REFILL_SUMMARY), true);
                }
            }
            else if (e.CommandName == "ReconcileRx")
            {
                if (lb.HasAttributes)
                {
                    RxTaskModel pharmacyTask = new RxTaskModel { DbId = DBID };
                    Session["TASKTYPE"] = pharmacyTask.TaskType = (Allscripts.ePrescribe.Common.Constants.PrescriptionTaskType)Convert.ToInt32(((GridEditableItem)e.Item).GetDataKeyValue("TaskType"));
                    pharmacyTask.RxGUID = string.Empty;
                    pharmacyTask.RxTaskId = (long)((GridEditableItem)e.Item).GetDataKeyValue("RxTaskID");
                    pharmacyTask.ScriptMessageGUID = Convert.ToString(((GridEditableItem)e.Item).GetDataKeyValue("scriptMessageID"));
                    pharmacyTask.ScriptMessage = new ScriptMessage().GetScriptMessage(pharmacyTask.ScriptMessageGUID.ToGuid(), 
                        PageState.GetStringOrEmpty(Constants.SessionVariables.LicenseId).ToGuidOr0x0(), 
                        PageState.GetStringOrEmpty(Constants.SessionVariables.UserId).ToGuidOr0x0(), 
                        PageState.Cast(Constants.SessionVariables.DbId, ConnectionStringPointer.ERXDB_DEFAULT));
                    pharmacyTask.PatientId = Convert.ToString(((GridEditableItem)e.Item).GetDataKeyValue("patientID"));
                    pharmacyTask.PhysicianComments = Convert.ToString(((GridEditableItem)e.Item).GetDataKeyValue("notesText"));
                    pharmacyTask.IsPatientVisitRq = Constants.CommonAbbreviations.NO;
                    pharmacyTask.RxRequestType = RequestType.APPROVE_WITH_CHANGE;
                    if (pharmacyTask.TaskType == Constants.PrescriptionTaskType.RXCHG ||
                        pharmacyTask.TaskType == Constants.PrescriptionTaskType.RXCHG_PRIORAUTH)
                    {
                        UpdateChangeRxTaskModelWithSessionInformation(pharmacyTask);
                    }
                    ArrayList rxList = new ArrayList();
                    Rx rx = new Rx();
                    string scriptMessageID = lb.Attributes["messageID"];

                    
                    Session[Constants.SessionVariables.TaskScriptMessageId] = scriptMessageID;
                    rx.DDI = lb.Attributes["DDI"];

                    //if the med is codified, move to sig otherwise match the med
                    if (rx.DDI == null || rx.DDI.Trim() == string.Empty)
                    {
                        ScriptMessage sm = new ScriptMessage(scriptMessageID, base.SessionLicenseID, base.SessionUserID, base.DBID);
                        switch ((Constants.UserCategory)Session["UserType"])
                        {
                            case Constants.UserCategory.PROVIDER:
                            case Constants.UserCategory.PHYSICIAN_ASSISTANT:
                                Response.Redirect(Constants.PageNames.REDIRECT_TO_ANGULAR + "?componentName=" + Constants.PageNames.SELECT_MEDICATION + $"&componentParameters={JsonHelper.ConvertUrlParameters($"Search=A&SearchText=" + sm.RxDrugDescription.Split(' ').FirstOrDefault() + " " + sm.RxStrength + "&from=" + Constants.PageNames.APPROVE_REFILL_TASK)}", true);
                                break;
                            case Constants.UserCategory.POB_SUPER:
                            case Constants.UserCategory.POB_REGULAR:
                            case Constants.UserCategory.PHYSICIAN_ASSISTANT_SUPERVISED:
                            case Constants.UserCategory.POB_LIMITED:
                                var pharmDetails = StringHelper.GetPharmacyName(
                                    sm.PharmacyName,
                                    sm.PharmacyAddress1,
                                    sm.PharmacyAddress2,
                                    sm.PharmacyCity,
                                    sm.PharmacyState,
                                    sm.PharmacyZip,
                                    sm.PharmacyPhoneNumber
                                );

                                var rxDetails = StringHelper.GetRxDetails(
                                    sm.DispensedRxDrugDescription,
                                    sm.DispensedRxSIGText,
                                    sm.DispensedRxQuantity,
                                    sm.DispensedDaysSupply,
                                    sm.DispensedRxRefills,
                                    sm.DispensedDaw,
                                    sm.DispensedCreated,
                                    sm.DispensedDateLastFill,
                                    sm.DispensedRxNotes
                                );
                                PageState[Constants.SessionVariables.IsReconcileREFREQNonCS] = true;
                                Response.Redirect(Constants.PageNames.REDIRECT_TO_ANGULAR + "?componentName=" + Constants.PageNames.SELECT_MEDICATION + $"&componentParameters={JsonHelper.ConvertUrlParameters($"RefillPharmacy={pharmDetails}&RxDetails={rxDetails}&from={Constants.PageNames.APPROVE_REFILL_TASK}&SearchText=" + HttpUtility.UrlEncode(sm.DispensedRxDrugDescription.Split(' ').FirstOrDefault()))}", true);
                                break;
                            default:
                                Response.Redirect(Constants.PageNames.REDIRECT_TO_ANGULAR + "?componentName=" + Constants.PageNames.SELECT_MEDICATION + $"&componentParameters={JsonHelper.ConvertUrlParameters($"Search=A&SearchText=" + sm.RxDrugDescription.Split(' ').FirstOrDefault() + " " + sm.RxStrength + "&from=" + Constants.PageNames.PHARMACY_REFILL_SUMMARY)}", true);
                                break;
                        }
                    }
                    else
                    {
                        DataSet ds = Allscripts.Impact.Medication.Load(rx.DDI, rx.SigID, base.DBID);
                        DataTable dtRx = ds.Tables["Medication"];

                        if (ds.Tables["Medication"].Rows.Count > 0)
                        {
                            rx.MedicationName = ds.Tables["Medication"].Rows[0]["MedicationName"].ToString();
                            rx.Strength = ds.Tables["Medication"].Rows[0]["STRENGTH"].ToString();
                            rx.ControlledSubstanceCode = ds.Tables["Medication"].Rows[0]["ControlledSubstanceCode"].ToString();
                            rx.StrengthUOM = ds.Tables["Medication"].Rows[0]["STRENGTHUOM"].ToString();
                            rx.RouteOfAdminCode = ds.Tables["Medication"].Rows[0]["RouteofAdminCode"].ToString();
                            rx.RouteOfAdminDescription = ds.Tables["Medication"].Rows[0]["RouteofAdmin"].ToString();
                            rx.DosageFormCode = ds.Tables["Medication"].Rows[0]["DosageFormCode"].ToString();
                            rx.DosageFormDescription = ds.Tables["Medication"].Rows[0]["DosageForm"].ToString();
                        }

                        rx.StateControlledSubstanceCode = Prescription.GetStateControlledSubstanceCode(rx.DDI, Session["PRACTICESTATE"].ToString(), base.DBID);

                        if (!string.IsNullOrEmpty(lb.Attributes["DelegateProviderID"]))
                        {
                            setDelegateProvider(lb.Attributes["DelegateProviderID"]);
                        }

                        string notes = string.Empty;
                        AddMassachusettsOpiateMessage(ds.Tables["Medication"].Rows[0]["GPI"].ToString(), rx.ControlledSubstanceCode, rx.StateControlledSubstanceCode, out notes);
                        rx.Notes = notes;

                        rxList.Add(rx);
                        Session["RxList"] = rxList;
                        Response.Redirect(Constants.PageNames.NURSE_SIG);
                    }
                }
            }
            else if (e.CommandName == "ChangePatient")
            {
                Session["TASKTYPE"] = (Constants.PrescriptionTaskType)Convert.ToInt32(lb.Attributes["tasktype"]);
                Response.Redirect(Constants.PageNames.APPROVE_SCRIPT_MESSAGE_PATIENT + "?smid=" + lb.Attributes["messageID"] + "&From=" + Constants.PageNames.PHARMACY_REFILL_SUMMARY + "&Action=ChangePatient", true);
            }
            else if (e.CommandName == "ChangeRequest")
            {
                string scriptMessageID = lb.Attributes["messageID"];
                Session["TASKTYPE"] = Constants.PrescriptionTaskType.REFREQ;
                Session[Constants.SessionVariables.TaskScriptMessageId] = scriptMessageID;
                var redirect = RefReq.SetDispensedAsCurrentAndReturnRedirect(scriptMessageID, DBID, PageState, iPrescription);
                setDelegateProvider(CurrentRx.ProviderID);
                Response.Redirect(redirect, true);
            }
            else if (e.CommandName == "PrintCSRx")
            {
                if (lb.HasAttributes)
                {
                    ArrayList rxList = new ArrayList();
                    Rx rx = new Rx();
                    string scriptMessageID = lb.Attributes["messageID"];
                    Session["TaskID"] = lb.Attributes["taskID"];
                    Session["TASKTYPE"] = (Allscripts.ePrescribe.Common.Constants.PrescriptionTaskType)Convert.ToInt32(lb.Attributes["tasktype"]);
                    Session[Constants.SessionVariables.TaskScriptMessageId] = scriptMessageID;
                    rx.DDI = lb.Attributes["DDI"];

                    ScriptMessage sm = new ScriptMessage(scriptMessageID, base.SessionLicenseID, base.SessionUserID, base.DBID);

                    DataSet ds = Allscripts.Impact.Medication.Load(rx.DDI, rx.SigID, base.DBID);
                    DataTable dtRx = ds.Tables["Medication"];

                    if (ds.Tables["Medication"].Rows.Count > 0)
                    {
                        rx.MedicationName = ds.Tables["Medication"].Rows[0]["MedicationName"].ToString();
                        rx.Strength = ds.Tables["Medication"].Rows[0]["STRENGTH"].ToString();
                        rx.ControlledSubstanceCode = ds.Tables["Medication"].Rows[0]["ControlledSubstanceCode"].ToString();
                        rx.StrengthUOM = ds.Tables["Medication"].Rows[0]["STRENGTHUOM"].ToString();
                        rx.RouteOfAdminCode = ds.Tables["Medication"].Rows[0]["RouteofAdminCode"].ToString();
                        rx.RouteOfAdminDescription = ds.Tables["Medication"].Rows[0]["RouteofAdmin"].ToString();
                        rx.DosageFormCode = ds.Tables["Medication"].Rows[0]["DosageFormCode"].ToString();
                        rx.DosageFormDescription = ds.Tables["Medication"].Rows[0]["DosageForm"].ToString();
                    }

                    int daysSupply;
                    int.TryParse(sm.DispensedDaysSupply, out daysSupply);
                    rx.DaysSupply = daysSupply;

                    int refills;
                    int.TryParse(sm.DispensedRxRefills, out refills);
                    rx.Refills = refills;

                    decimal quantity;
                    decimal.TryParse(sm.DispensedRxQuantity, out quantity);
                    rx.Quantity = quantity;

                    rx.SigText = sm.DispensedRxSIGText;

                    rx.StateControlledSubstanceCode = Prescription.GetStateControlledSubstanceCode(rx.DDI, Session["PRACTICESTATE"].ToString(), base.DBID);

                    var sigInfo = Allscripts.Impact.Sig.SaveFreeFormSigTextOrGetInfo(rx.SigText, Allscripts.ePrescribe.DatabaseSelector.ConnectionStringPointer.SHARED_DB);
                    rx.SigID = Convert.ToString(sigInfo.SigID);
                    rx.SigTypeId = (sigInfo.SigTypeID > 0) ? sigInfo.SigTypeID : (int)SigTypeEnum.SigTypeFreeForm;

                    //
                    // TODO: change this for POB/PA with sup workflow?
                    //
                    if (!string.IsNullOrEmpty(lb.Attributes["DelegateProviderID"]))
                    {
                        setDelegateProvider(lb.Attributes["DelegateProviderID"]);
                    }

                    string notes = string.Empty;
                    AddMassachusettsOpiateMessage(ds.Tables["Medication"].Rows[0]["GPI"].ToString(), rx.ControlledSubstanceCode, rx.StateControlledSubstanceCode, out notes);
                    rx.Notes = notes;

                    rxList.Add(rx);
                    Session["RxList"] = rxList;

                    bool hasDUR = DUR.HasWarnings(DURWarnings);
                    if (hasDUR)
                    {
                        Session["DUR_GO_PREVIOUS"] = Constants.PageNames.PHARMACY_REFILL_SUMMARY;

                        // Check for DUP DUR                    
                        bool isOnlyDUPDUR = false;
                        bool hasMoreThanDuplicateDUR = false;

                        bool hasDuplicateTherapyCheckResults = DURWarnings.DuplicateTherapy.Results.HasItems();
                        hasMoreThanDuplicateDUR = DURMedispanUtils.HasMoreThanDupDUR(DURWarnings);
                        isOnlyDUPDUR = hasDuplicateTherapyCheckResults && !hasMoreThanDuplicateDUR;

                        if (chkRegistryChecked.Visible)
                        {
                            Session["isCSRegistryChecked"] = chkRegistryChecked.Checked;
                        }

                        //
                        // We can only allow a Super POB to process a REFREQ that has a DUR. Regular POBs must send scripts with a DUR to a provider, and that doesn't make sense in the
                        // renewal workflow. Limited POBs can't even see the task tab.
                        // Regular POBs can now process REFREQ having only DUP DUR.
                        if (base.SessionUserType == Constants.UserCategory.POB_SUPER || (base.SessionUserType == Constants.UserCategory.POB_REGULAR && isOnlyDUPDUR))
                        {
                            if (hasDuplicateTherapyCheckResults)
                            {
                                Session["POBRefReq"] = true;

                                var ddiList = string.Join(",", from script in rxList.Cast<Rx>().ToList() select script.DDI);

                                DataSet dsActiveScripts = Prescription.CheckPatientRxHistoryForMultiSelect(ddiList, base.SessionPatientID, base.DBID);

                                if (dsActiveScripts.Tables[0].Rows.Count > 0)
                                {
                                    ucMedicationHistoryCompletion.ActiveScripts = dsActiveScripts;
                                    ucMedicationHistoryCompletion.IsCSMed = true;
                                    ucMedicationHistoryCompletion.SearchValue = string.Empty;
                                    ucMedicationHistoryCompletion.LoadHistory();
                                }
                            }
                            else
                            {
                                Response.Redirect(Constants.PageNames.RX_DUR_REVIEW_MULTI_SELECT);
                            }
                        }
                        else
                        {
                            ucMessage2.MessageText = "You do not have permission to process this renewal request because a DUR alert is presented.";
                            ucMessage2.Icon = Controls_Message.MessageType.ERROR;
                            ucMessage2.Visible = true;
                        }
                    }
                    else
                    {
                        ucCSMedRefillRequestNotAllowed.ShowPopUp();
                    }
                }
            }
        }

        private void AddMassachusettsOpiateMessage(string gpi, string federalCSCode, string stateCSCode, out string notes)
        {
            string csCode = Prescription.ReconcileControlledSubstanceCodes(federalCSCode, stateCSCode);
            notes = string.Empty;

            if (base.IsPOBUser)
            {
                List<string> deaSchedulesAllowed = new List<string>();
                if (base.DelegateProvider.DEAScheduleAllowed(2))
                {
                    deaSchedulesAllowed.Add("2");
                }

                if (Session["SUPERVISING_PROVIDER_ID"] == null)
                {
                    if (iPrescription.IsValidMassOpiate(
                        PageState.GetStringOrEmpty(Constants.SessionVariables.PracticeState),
                        gpi,
                        csCode,
                        base.DelegateProvider.IsDEAExpired(),
                        deaSchedulesAllowed
                        ))
                    {
                        notes = Constants.PartialFillUponPatientRequest;
                    }
                }
                else
                {
                    // POB user that selected a PA with Supervision as a delegate provider, so now we need a supervising provider too
                    if (iPrescription.IsValidMassOpiate(
                        PageState.GetStringOrEmpty(Constants.SessionVariables.PracticeState),
                        gpi,
                        csCode,
                        base.DelegateProvider.IsDEAExpired(),
                        base.SupervisingProvider.IsDEAExpired(),
                        (List<string>)(Session["DEASCHEDULESALLOWED_SUPERVISOR"]),
                        (List<string>)(Session["PASUPERVISOR_DEASCHEDULESALLOWED"])))
                    {
                        notes = Constants.PartialFillUponPatientRequest;
                    }
                }
            }
            else if (base.SessionUserType == Allscripts.ePrescribe.Common.Constants.UserCategory.PHYSICIAN_ASSISTANT_SUPERVISED)
            {
                //PA and delegate provider both should have the schedule to proceed. 

                if (iPrescription.IsValidMassOpiate(
                    PageState.GetStringOrEmpty(Constants.SessionVariables.PracticeState),
                    gpi,
                    csCode,
                    Convert.ToBoolean(Session["HasExpiredDEA"]),
                    base.DelegateProvider.IsDEAExpired(),
                    (List<string>)(Session["DEASCHEDULESALLOWED"]),
                    (List<string>)(Session["DEASCHEDULESALLOWED_SUPERVISOR"])))
                {
                    notes = Constants.PartialFillUponPatientRequest;
                }
            }
        }

        protected void grdPharmRefillDetails_ItemCreated(object sender, GridItemEventArgs e)
        {
            foreach (GridColumn col in grdPharmRefillDetails.MasterTableView.RenderColumns)
            {
                if ((bool)Session["SHOW_SEND_TO_ADM"] == false && col.UniqueName == "colSendToADM")
                {
                    col.Visible = false;
                }
            }
        }

        void ucCSMedRefillRequestNotAllowed_OnContactProvider(object sender, EventArgs e)
        {
            ScriptMessage.RejectMessage(
                Session[Constants.SessionVariables.TaskScriptMessageId].ToString(),
                string.Empty,
                "Contact provider by alternate methods regarding controlled medications",
                base.SessionUserID,
                base.SessionLicenseID,
                Guid.Empty.ToString(),
                base.ShieldSecurityToken,
                base.SessionSiteID,
                base.DBID);

            // RxId is not created at this moment as rejecting REFREQ message.
            //base.AuditLogPatientRxInsert(ePrescribeSvc.AuditAction.PATIENT_TASK_PROCESS, base.SessionPatientID, base.CurrentRx.RxID);

            Session["REFILLMSG"] = "Controlled substance refill denied.";

            if (base.IsUserAPrescribingUserWithCredentials)
            {
                Response.Redirect(Constants.PageNames.APPROVE_REFILL_TASK);
            }
            else
            {
                Response.Redirect(Constants.PageNames.PHARMACY_REFILL_SUMMARY);
            }
        }

        void ucCSMedRefillRequestNotAllowed_OnPrintRefillRequest(object sender, EventArgs e)
        {
            ScriptMessage sm = new ScriptMessage(Session[Constants.SessionVariables.TaskScriptMessageId].ToString(), base.SessionLicenseID, base.SessionUserID, base.DBID);

            string rxID = Guid.NewGuid().ToString();

            StringBuilder pharmComments = new StringBuilder();

            DataSet pharmDS = Allscripts.Impact.Pharmacy.LoadPharmacy(sm.DBPharmacyID, base.DBID);

            pharmComments.Append(base.CurrentRx.Notes);

            Prescription rx = new Prescription();

            string stateCSCodeForPharmacy = Allscripts.Impact.Prescription.GetStateControlledSubstanceCode(base.CurrentRx.DDI, null, pharmDS.Tables[0].Rows[0]["State"].ToString(), base.DBID);
            //stateCSCodeForPractice = Allscripts.Impact.Prescription.GetStateControlledSubstanceCode(base.CurrentRx.DDI, Session["PracticeState"].ToString(), base.DBID);

            string providerID = base.SessionUserID;

            if (Session["DelegateProviderID"] != null)
            {
                providerID = Session["DelegateProviderID"].ToString();
            }

            string deaNumber = GetDEANumberToBeUsed(PageState);

            rx.SetHeaderInformation(base.SessionLicenseID, rxID, DateTime.UtcNow.ToString(),
                sm.DBPatientID, providerID, Guid.Empty.ToString(),
                string.Empty, string.Empty, string.Empty, Constants.PrescriptionType.NEW, false, string.Empty,
                Convert.ToInt32(Session["SiteID"].ToString()), Constants.ERX_NOW_RX, null, base.DBID);

            rx.AddMedication(
                base.SessionLicenseID, //EAK added
                0, // RxNumber
                sm.DBDDID,
                base.CurrentRx.MedicationName, //sm.RxDrugDescription,
                base.CurrentRx.RouteOfAdminCode, // routeOfAdminCode
                base.CurrentRx.DosageFormCode, // sm.RxDosageFormCode, // dosageFormCode
                base.CurrentRx.Strength,
                base.CurrentRx.StrengthUOM,
                base.CurrentRx.SigID,
                base.CurrentRx.SigText, // sigText
                base.CurrentRx.Refills, //refills
                base.CurrentRx.Quantity,
                base.CurrentRx.DaysSupply,
                string.Empty, //gppc
                Convert.ToDecimal(1), //packageSize
                "EA", //packageUOM
                1, //packageQuantity
                "EA", //packageDescription
                base.CurrentRx.DAW, //daw
                DateTime.Today.ToString(),
                Constants.PrescriptionStatus.NEW, //status
                Constants.PrescriptionTransmissionMethod.SENT, //transmissionMethod
                string.Empty, //originalDDI
                0, //originalFormStatusCode
                "N", //originalIsListed
                0, //formStatusCode
                "N", //isListed
                FormularyStatus.NONE, //formularyStatus
                Session["PERFORM_FORMULARY"].ToString(), //_performFormulary
                DURSettings.CheckPerformDose.ToChar(),
                DURSettings.CheckDrugToDrugInteraction.ToChar(),
                DURSettings.CheckDuplicateTherapy.ToChar(),
                DURSettings.CheckPar.ToChar(),
                sm.PracticeState, //rxFormat
                pharmComments.ToString(), //notes
                Guid.Empty.ToString(), //originalRxID
                0, //originalLineNumber
                "R", //creationType
                Guid.Empty.ToString(),
                null, //ICD9 EAK
                base.CurrentRx.ControlledSubstanceCode,  // medispan (or federal) controlled substance code
                providerID,
                null, // lastFillDate
                null, // authorizeByID
                Session["RX_WORKFLOW"] != null ? ((PrescriptionWorkFlow)Convert.ToInt32(Session["RX_WORKFLOW"])) : PrescriptionWorkFlow.STANDARD,
                Session["ExtFacilityCd"] != null ? Session["ExtFacilityCd"].ToString() : null,
                Session["ExtGroupID"] != null ? Session["ExtGroupID"].ToString() : null,
                base.CurrentRx.CoverageID, //coverageID
                -1, // alternativeIgnoreReason
                stateCSCodeForPharmacy,
                deaNumber,
                base.CurrentRx.SigTypeId
                );

            bool? isCSRegistryChecked = null;
            if (chkRegistryChecked.Visible)
            {
                isCSRegistryChecked = chkRegistryChecked.Checked;
            }

            rx.Save(Convert.ToInt32(Session["SiteID"].ToString()), base.SessionLicenseID, base.SessionUserID, true, isCSRegistryChecked, base.DBID);

            var couponResponse =
            EPSBroker.ECouponPrintRefillRequest(rxID,
            sm.DBDDID, base.CanApplyFinancialOffers,
            ConfigKeys.AutoSendCoupons, base.DBID);

            if (couponResponse.IsPharmacyNotesUpdatedToRx)
            {
                List<string> couponDetailIds = new List<string>();
                couponDetailIds.Add(couponResponse.ECouponDetailID.ToString());

                PageState["PrintOnlyCoupons"] = couponDetailIds;
            }
            Session["isCSRefillNotAllowed"] = true;

            Hashtable htTaskRxID = new Hashtable();
            htTaskRxID.Add(Convert.ToInt64(Session["TaskID"]), rxID);
            Session["HTTaskRxID"] = htTaskRxID;

            if (base.IsPOBUser && !string.IsNullOrWhiteSpace(PageState.GetStringOrEmpty("SUPERVISING_PROVIDER_ID")))
            {
                // update the authorized id as the supervising physician's id
                Prescription.UpdateRxDetailStatus(base.SessionLicenseID, PageState.GetStringOrEmpty("SUPERVISING_PROVIDER_ID"), rxID, "AUTHORIZEBY", base.DBID);
            }

            if (base.IsUserAPrescribingUserWithCredentials)
            {
                Server.Transfer(Constants.PageNames.CSS_DETECT + "?To=" + HttpUtility.HtmlEncode(Constants.PageNames.APPROVE_REFILL_TASK));
            }
            else
            {
                Server.Transfer(Constants.PageNames.CSS_DETECT + "?To=" + HttpUtility.HtmlEncode(Constants.PageNames.PHARMACY_REFILL_SUMMARY));
            }
        }

        #endregion Events

        #region Private Methods

        private void setDelegateProvider(string providerID)
        {
            if (!string.IsNullOrEmpty(providerID))
            {
                providerID = providerID.Trim();
            }

            Session["DelegateProviderID"] = providerID;
            setSPI(providerID);
        }

        private void setSPI(string providerID)
        {
            DataSet dsSPI = Provider.GetSPI(providerID, base.DBID);
            if (dsSPI.Tables.Count > 0)
            {
                DataRow[] drSPI = dsSPI.Tables[0].Select("ScriptSwId='SURESCRIPTS'");
                //should only be one row for SURESCRIPTS...grab the first and only
                if (drSPI.Length > 0 && drSPI[0] != null && drSPI[0]["SenderId"] != DBNull.Value && drSPI[0]["SenderId"].ToString() != string.Empty)
                {
                    Session["SPI"] = drSPI[0]["SenderID"].ToString();
                }
                else
                {
                    Session["SPI"] = null;
                }
            }
            else
            {
                Session["SPI"] = null;
            }
        }
        public DURCheckResponse DURWarnings
        {
            get
            {
                var durWarnings = PageState[Constants.SessionVariables.DURCheckResponse];
                if (durWarnings == null)
                {
                    durWarnings = ConstructAndSendDURRequest();

                }
                PageState[Constants.SessionVariables.DURCheckResponse] = durWarnings;
                return (DURCheckResponse)durWarnings;
            }
            set { PageState[Constants.SessionVariables.DURCheckResponse] = value; }
        }

        private DURCheckResponse ConstructAndSendDURRequest()
        {
            DURCheckRequest durCheckRequest = ConstructDURRequest();
            DURCheckResponse durCheckResponse = DURMSC.PerformDURCheck(durCheckRequest);
            return durCheckResponse;
        }

        private DURCheckRequest ConstructDURRequest()
        {
            var request = new DURCheckRequest();
            request.Patient = DURMedispanUtils.RetrieveDURPatient(PageState.GetStringOrEmpty(Constants.SessionVariables.PatientDOB),
                PageState.GetStringOrEmpty(Constants.SessionVariables.Gender));
            ArrayList rxArrayList = PageState.Cast(Constants.SessionVariables.RxList, new ArrayList());
            request.DrugsToCheck = DURMedispanUtils.RetrieveDrugList(rxArrayList.ToList<Rx>(), new DrugToCheckUtils(), DURMedispanUtils.DosageFormCodes);
            request.ExistingMedsDDIs = DURMedispanUtils.RetrieveActiveMedsList(Session[Constants.SessionVariables.ACTIVEMEDDDILIST] as List<string>);
            request.Allergies = DurPatientAllergies;
            var durSettings = DURMedispanUtils.CloneDURSettings(DURSettings);
            durSettings.CheckDuplicateTherapy = YesNoSetting.Yes; //always perform a Dup Therapy check, regardless of site settings so the overlay appears.
            request.Settings = durSettings;
            request.MinimumDocumentationLevelForModerateSeverity = request.Settings.InteractionDocumentType;
            request.MinimumDocumentationLevelForMinorSeverity = request.Settings.InteractionDocumentType;
            request.MinimumDocumentationLevelForMajorSeverity = request.Settings.InteractionDocumentType;
            request.MinimumManagementLevel = InteractionsManagementLevel.PotentialInteractionRisk;
            request.MinimumSeverity = request.Settings.InteractionSeverityCheckType;
            return request;
        }

        public string RetrieveDosageFormCode(IScriptMessage iScriptMessage, RxTaskModel task)
        {
            ScriptMessage sm = iScriptMessage.GetScriptMessage(task.ScriptMessageGUID.ToGuid(), task.LicenseId, task.UserId, task.DbId);
            return sm.DBDrugDosageFormCode;
        }
        private POBWorkflowType GetPOBWorkflowTypeAndRouteChangeRx(RxTaskModel pharmacyTask, GridDataItem gridDataItem, IChangeRx iChangeRx, string authToken)
        {
            POBWorkflowType currentPOBWorkflowType = POBWorkflowType.POB_ANY_NON_DUR;
            //The following portion is straight copied from Refreq workflow to allow correct DUR Processing
            //***************************************************************************************
            ArrayList rxArrayList = new ArrayList();
            Rx rx = new Rx();

            string refillMedDDI = gridDataItem.GetDataKeyValue("DDI").ToString();
            rx.DDI = refillMedDDI;

            string refillDrugDesc = gridDataItem.GetDataKeyValue("drugDescription").ToString();
            rx.MedicationName = refillDrugDesc;

            string refillDrugStrength = gridDataItem.GetDataKeyValue("strength").ToString();
            rx.Strength = refillDrugStrength;

            TextBox notes = gridDataItem.FindControl("notesText") as TextBox;
            string notestext = string.Empty;
            if (notes == null || notes.Text.Length == 0)
            {
                notestext = string.Empty;
            }
            else
            {
                notestext = notes.Text;
            }

            rx.Notes = notestext;

            if (pharmacyTask.IsControlledSubstance)
            {
                rx.ControlledSubstanceCode = pharmacyTask.ReconciledControlledSubstanceCode.ToString();
                rx.ScheduleUsed = pharmacyTask.ReconciledControlledSubstanceCode;
            }

            if (!string.IsNullOrEmpty(refillMedDDI))
            {
                bool bDAW;
                string refillDAW = gridDataItem.GetDataKeyValue("DAW").ToString();

                if (refillDAW.Equals("Y"))
                {
                    bDAW = true;
                }
                else
                {
                    bDAW = false;
                }

                rx.DAW = bDAW;

                //Convert data type ahead of time. 
                int daysSupply = 0;
                decimal quantity = 0;
                int refills = 0;

                //Convert the days of supply
                try
                {
                    daysSupply = Convert.ToInt32(gridDataItem.GetDataKeyValue("daysSupply").ToString());
                }
                catch (Exception)
                {
                    daysSupply = 0;
                }

                rx.DaysSupply = daysSupply;

                //Convert the quantity
                try
                {
                    quantity = Convert.ToDecimal(gridDataItem.GetDataKeyValue("quantity").ToString());
                }
                catch (Exception)
                {
                    quantity = 0;
                }

                rx.Quantity = quantity;

                try
                {
                    refills = Convert.ToInt32(gridDataItem.GetDataKeyValue("refills").ToString());
                }
                catch (Exception)
                {
                    refills = 0;
                }

                rx.Refills = refills;

                string sigText = gridDataItem.GetDataKeyValue("sigText").ToString();
                string providerID = gridDataItem.GetDataKeyValue("providerID").ToString();

                rx.SigText = sigText;

                ScriptMessage sm = new ScriptMessage(pharmacyTask.ScriptMessageGUID, MasterPage.RxTask.LicenseId.ToString(),
                MasterPage.RxTask.UserId.ToString(), MasterPage.RxTask.DbId);
                RequestedRx rxAtSelectedIndex = sm.RequestedRxs[pharmacyTask.RequestedRxIndexSelected];
                var rxToApprove = new Rx(rxAtSelectedIndex);
                rxArrayList.Add(rxToApprove);
                Session["RxList"] = rxArrayList;

                //***************************************************************************************
                DURCheckResponse durCheckResponse = DURWarnings;
                bool hasDUR = DUR.HasWarnings(durCheckResponse);
                if (hasDUR)
                {
                    // if DUR found, go to RxDURReviewMultiSelect.aspx for further processing
                    //ValidationSummary1.Enabled = false; // "true" will cause the overlay to fail                    

                    currentPOBWorkflowType = POBWorkflowType.POB_REGULAR_DUP_ONLY_DUR;

                    // Check for DUP DUR
                    bool isOnlyDUPDUR = false;
                    bool hasDUP = durCheckResponse.DuplicateTherapy.Results.HasItems();
                    bool hasOtherDUR = DURMedispanUtils.HasMoreThanDupDUR(durCheckResponse);

                    //
                    // We can only allow a Super POB to process a REFREQ that has a DUR. Regular POBs must send scripts with a DUR to a provider, and that doesn't make sense in the
                    // renewal workflow. Limited POBs can't even see the task tab.
                    // Regular POBs can now process REFREQ having only DUP DUR.            
                    if (base.SessionUserType == Allscripts.ePrescribe.Common.Constants.UserCategory.POB_SUPER || (base.SessionUserType == Constants.UserCategory.POB_REGULAR && isOnlyDUPDUR))
                    {
                        PageState[Constants.SessionVariables.TaskScriptMessageId] = pharmacyTask.ScriptMessageGUID;
                        PageState["TaskID"] = pharmacyTask.RxTaskId;
                        PageState["Tasktype"] = pharmacyTask.TaskType;
                        PageState["DUR_GO_PREVIOUS"] = Constants.PageNames.PHARMACY_REFILL_SUMMARY;
                        PageState["CameFrom"] = Constants.PageNames.PHARMACY_REFILL_SUMMARY;
                        if (hasDUP)
                        {
                            List<Rx> rxList = new List<Rx>();
                            Rx r = new Rx();
                            r.DDI = base.CurrentRx.DDI;
                            rxList.Add(r);

                            PageState["POBRefReq"] = true;

                            var ddiList = string.Join(",", from script in rxList.Cast<Rx>().ToList() select script.DDI);

                            DataSet dsActiveScripts = Prescription.CheckPatientRxHistoryForMultiSelect(ddiList, base.SessionPatientID, base.DBID);

                            if (dsActiveScripts.Tables[0].Rows.Count > 0)
                            {
                                ucMedicationHistoryCompletion.ActiveScripts = dsActiveScripts;
                                ucMedicationHistoryCompletion.SearchValue = string.Empty;
                                ucMedicationHistoryCompletion.LoadHistory();
                                if (rxAtSelectedIndex == null)
                                {
                                    currentPOBWorkflowType = POBWorkflowType.POB_SOME_OR_ALL_REVIEW_REQUIRED_DUR;
                                }
                                else
                                {
                                    MasterPage.RxTask.ScriptMessage = sm;
                                    MasterPage.RxTask = iChangeRx.UpdateChangeRxWorkflowFromPharmacyTask(MasterPage.RxTask, sm.XmlMessage, rxToApprove);
                                }
                            }
                            else
                            {
                                MasterPage.RxTask.ScriptMessage = sm;
                                MasterPage.RxTask = iChangeRx.UpdateChangeRxWorkflowFromPharmacyTask(MasterPage.RxTask, sm.XmlMessage, rxToApprove);
                                Response.Redirect(Constants.PageNames.RX_DUR_REVIEW_MULTI_SELECT);
                            }
                        }
                        else
                        {
                            ArrayList rxList = new ArrayList();
                            if (rxAtSelectedIndex == null)
                            {
                                currentPOBWorkflowType = POBWorkflowType.POB_SOME_OR_ALL_REVIEW_REQUIRED_DUR;
                            }
                            else
                            {
                                MasterPage.RxTask.ScriptMessage = sm;
                                MasterPage.RxTask = iChangeRx.UpdateChangeRxWorkflowFromPharmacyTask(MasterPage.RxTask, sm.XmlMessage, rxToApprove);
                                ProccessDur(pharmacyTask, rxToApprove, rxList);
                            }
                        }
                    }
                    else
                    {
                        currentPOBWorkflowType = POBWorkflowType.POB_SOME_OR_ALL_REVIEW_REQUIRED_DUR;
                    }
                }
                else if (pharmacyTask.IsControlledSubstance)
                {
                    Session[Constants.SessionVariables.TaskScriptMessageId] = pharmacyTask.ScriptMessageGUID;
                    Session["TaskID"] = pharmacyTask.RxTaskId;

                    ucCSMedRefillRequestNotAllowed.ShowPopUp();
                }
                else
                {
                    string delegateProviderID = string.Empty;

                    if (base.IsPOBUser && Session["SUPERVISING_PROVIDER_ID"] != null)
                    {
                        //
                        // get the correct delegate provider for refill request for PA with supervision when refill request is being processed by a POB user
                        //
                        delegateProviderID = Session["SUPERVISING_PROVIDER_ID"].ToString();
                    }
                    else
                    {
                        delegateProviderID = Session["DelegateProviderID"] != null ? Session["DelegateProviderID"].ToString() : base.SessionUserID;
                    }

                    long serviceTaskID = -1;
                    MasterPage.RxTask.ScriptMessage = sm;
                    MasterPage.RxTask = iChangeRx.UpdateChangeRxWorkflowFromPharmacyTask(MasterPage.RxTask, sm.XmlMessage, rxToApprove);
                    serviceTaskID = iChangeRx.ApproveRxChangeWorkflow(MasterPage.RxTask, serviceTaskID, authToken);

                    string auditLogPatientId = string.Empty;
                    var rxResponse = AuditLogPatientRxInsert(ePrescribeSvc.AuditAction.PATIENT_TASK_PROCESS, Request.QueryString["PatientId"], CurrentRx.RxID);
                    if (rxResponse.Success)
                    {
                        auditLogPatientId = rxResponse.AuditLogPatientID;
                    }

                    //Check here if it is not -1 then insert it in newly created table. you also have auditLogPatientID.
                    //This will be used from service manager and added last audit log when message is sent to hub.
                    if (serviceTaskID != -1 && !string.IsNullOrEmpty(auditLogPatientId))
                    {
                        Audit.InsertAuditLogPatientServiceTask(serviceTaskID, auditLogPatientId, DBID);
                    }

                    MasterPage.RefreshActiveMeds();
                }
            }
            return currentPOBWorkflowType;
        }
        private void UpdateChangeRxTaskModelWithSessionInformation(RxTaskModel pharmacyTask)
        {
            pharmacyTask.UserId = SessionUserID.ToGuid();
            pharmacyTask.DbId = DBID;
            pharmacyTask.LicenseId = SessionLicenseID.ToGuid();
            pharmacyTask.SiteId = SessionSiteID;
            pharmacyTask.ShieldSecurityToken = ShieldSecurityToken;
            pharmacyTask.ExternalFacilityCd = PageState.GetStringOrEmpty(Constants.SessionVariables.ExternalFacilityCd);
            pharmacyTask.ExternalGroupID = PageState.GetStringOrEmpty(Constants.SessionVariables.ExternalGroupID);
            pharmacyTask.UserType = SessionUserType;
            pharmacyTask.DelegateProviderId = SessionDelegateProviderID.ToGuidOr0x0();
            MasterPage.RxTask = pharmacyTask;
        }

        public string RejectREFREQDisplayMessage(RxTaskModel pharmacyTask, string patientFirstName, string patientLastName, string UserID, string LicenseID)
        {
            StringBuilder messageBuilder = new StringBuilder();
            ScriptMessage.RejectMessage(
                    pharmacyTask.ScriptMessageGUID,
                    pharmacyTask.DenialCode,
                    pharmacyTask.DenialText,
                    UserID,
                    LicenseID,
                    pharmacyTask.RxTaskId.ToString(),
                    base.ShieldSecurityToken,
                    base.SessionSiteID,
                    base.DBID);

            messageBuilder.Append("Refill denied (").Append(pharmacyTask.DenialText).Append(") for ");
            messageBuilder.Append(patientFirstName).Append(" ");
            messageBuilder.Append(patientLastName);
            return messageBuilder.ToString();
        }

        public string RejectRXCHGPriorAuthDisplayMessage(RxTaskModel pharmacyTask, IChangeRx iChangeRx, string patientFirstName, string patientLastName)
        {
            StringBuilder messageBuilder = new StringBuilder();
            iChangeRx.RejectPriorAuthWorkflow(pharmacyTask);
            messageBuilder.Append("Rx Change Prior Auth denied (").Append(pharmacyTask.DenialText).Append(") for ");
            messageBuilder.Append(patientFirstName).Append(" ");
            messageBuilder.Append(patientLastName);
            return messageBuilder.ToString();
        }


        public string RejectRXCHGDisplayMessage(RxTaskModel pharmacyTask, IChangeRx iChangeRx, string patientFirstName, string patientLastName)
        {
            StringBuilder messageBuilder = new StringBuilder();
            iChangeRx.RejectRxChangeWorkflow(pharmacyTask, new ScriptMessage());
            messageBuilder.Append("Rx Change denied (").Append(pharmacyTask.DenialText).Append(") for ");
            messageBuilder.Append(patientFirstName).Append(" ");
            messageBuilder.Append(patientLastName);
            return messageBuilder.ToString();
        }

        public POBWorkflowType GetPOBWorkflowTypeAndRouteRefReq(RxTaskModel pharmacyTask, GridDataItem gridDataItem, string patientFirstName, string patientLastName)
        {
            POBWorkflowType currentPOBWorkflowType = POBWorkflowType.POB_ANY_NON_DUR;
            StringBuilder messageBuilder = new StringBuilder();

            ArrayList rxArrayList = new ArrayList();
            Rx rx = new Rx();

            string refillMedDDI = gridDataItem.GetDataKeyValue("DDI").ToString();
            rx.DDI = refillMedDDI;

            string refillDrugDesc = gridDataItem.GetDataKeyValue("drugDescription").ToString();
            rx.MedicationName = refillDrugDesc;

            string refillDrugStrength = gridDataItem.GetDataKeyValue("strength").ToString();
            rx.Strength = refillDrugStrength;

            TextBox notes = gridDataItem.FindControl("notesText") as TextBox;
            string notestext = string.Empty;
            if (notes == null || notes.Text.Length == 0)
            {
                notestext = string.Empty;
            }
            else
            {
                notestext = notes.Text;
            }

            rx.Notes = notestext;

            if (pharmacyTask.IsControlledSubstance)
            {
                rx.ControlledSubstanceCode = pharmacyTask.ReconciledControlledSubstanceCode.ToString();
                rx.ScheduleUsed = pharmacyTask.ReconciledControlledSubstanceCode;
            }

            if (string.IsNullOrEmpty(refillMedDDI))
            {
                Session["PATIENTID"] = pharmacyTask.PatientId;
                Session[Constants.SessionVariables.TaskScriptMessageId] = pharmacyTask.ScriptMessageGUID;
                Session["SentTo"] = gridDataItem.GetDataKeyValue("pharmacyName").ToString();
                Server.Transfer(Constants.PageNames.REDIRECT_TO_ANGULAR + "?componentName=" + Constants.PageNames.SELECT_MEDICATION + "?searchText=" + HttpUtility.UrlEncode(refillDrugDesc + " " + refillDrugStrength));
            }
            else
            {
                bool bDAW = false;
                string refillDAW = gridDataItem.GetDataKeyValue("DAW").ToString();

                bDAW = (refillDAW != null && refillDAW.Equals("1") ? true : false);

                rx.DAW = bDAW;

                //Convert data type ahead of time. 
                int daysSupply = 0;
                decimal quantity = 0;
                int dispensesRequested = 0;

                //Convert the days of supply
                try
                {
                    daysSupply = Convert.ToInt32(gridDataItem.GetDataKeyValue("daysSupply").ToString());
                }
                catch (Exception)
                {
                    daysSupply = 0;
                }

                rx.DaysSupply = daysSupply;

                //Convert the quantity
                try
                {
                    quantity = Convert.ToDecimal(gridDataItem.GetDataKeyValue("quantity").ToString());
                }
                catch (Exception)
                {
                    quantity = 0;
                }

                rx.Quantity = quantity;

                try
                {
                    dispensesRequested = Convert.ToInt32(gridDataItem.GetDataKeyValue("refills").ToString());
                }
                catch (Exception)
                {
                    dispensesRequested = 1;
                }

                rx.Refills = dispensesRequested - 1;

                string sigText = gridDataItem.GetDataKeyValue("sigText").ToString();
                string sigID = Allscripts.Impact.Sig.SaveFreeFormSigTextOrGetId(sigText, Allscripts.ePrescribe.DatabaseSelector.ConnectionStringPointer.SHARED_DB);
                string providerID = gridDataItem.GetDataKeyValue("providerID").ToString();

                rx.SigText = sigText;
                rx.SigID = sigID;
                rx.SigTypeId = (int)SigTypeEnum.SigTypeFreeForm;
                rx.DosageFormCode = RetrieveDosageFormCode(new ScriptMessage(), pharmacyTask);

                rxArrayList.Add(rx);
                Session["RxList"] = rxArrayList;

                DURCheckResponse durCheckResponse = DURWarnings;
                bool hasDUR = DUR.HasWarnings(durCheckResponse);
                bool isOnlyDUPDUR = false;
                bool DDImismatch = false;

                if (hasDUR)
                {
                    // if DUR found, go to RxDURReviewMultiSelect.aspx for further processing
                    //ValidationSummary1.Enabled = false; // "true" will cause the overlay to fail                    

                    currentPOBWorkflowType = POBWorkflowType.POB_REGULAR_DUP_ONLY_DUR;

                    // Check for DUP DUR                    
                    bool hasDUP = durCheckResponse.DuplicateTherapy.Results.HasItems();
                    bool hasOtherDUR = DURMedispanUtils.HasMoreThanDupDUR(durCheckResponse);

                    isOnlyDUPDUR = hasDUP && !hasOtherDUR;

                    //
                    // We can only allow a Super POB to process a REFREQ that has a DUR. Regular POBs must send scripts with a DUR to a provider, and that doesn't make sense in the
                    // renewal workflow. Limited POBs can't even see the task tab.
                    // Regular POBs can now process REFREQ having only DUP DUR.
                    if (base.SessionUserType == Allscripts.ePrescribe.Common.Constants.UserCategory.POB_SUPER || (base.SessionUserType == Constants.UserCategory.POB_REGULAR && isOnlyDUPDUR))
                    {
                        Session[Constants.SessionVariables.TaskScriptMessageId] = pharmacyTask.ScriptMessageGUID;
                        Session["TaskID"] = pharmacyTask.RxTaskId;
                        Session["Tasktype"] = pharmacyTask.TaskType;

                        Session["DUR_GO_PREVIOUS"] = Constants.PageNames.PHARMACY_REFILL_SUMMARY;
                        Session["CameFrom"] = Constants.PageNames.PHARMACY_REFILL_SUMMARY;

                        if (hasDUP)
                        {
                            List<Rx> rxList = new List<Rx>();
                            Rx r = new Rx();
                            r.DDI = base.CurrentRx.DDI;
                            rxList.Add(r);

                            Session["POBRefReq"] = true;

                            var ddiList = string.Join(",", from script in rxList.Cast<Rx>().ToList() select script.DDI);

                            DataSet dsActiveScripts = Prescription.CheckPatientRxHistoryForMultiSelect(ddiList, base.SessionPatientID, base.DBID);

                            if (dsActiveScripts.Tables[0].Rows.Count > 0)
                            {
                                ucMedicationHistoryCompletion.ActiveScripts = dsActiveScripts;
                                ucMedicationHistoryCompletion.SearchValue = string.Empty;
                                ucMedicationHistoryCompletion.LoadHistory();
                            }
                            else
                            {
                                if (DURMedispanUtils.IsAnyDurSettingOn(DURSettings) && DURWarnings != null)
                                    Response.Redirect(Constants.PageNames.RX_DUR_REVIEW_MULTI_SELECT, true);
                                else
                                    DDImismatch = true;

                            }
                        }  // hasDUP
                        else
                        {
                            if (DURMedispanUtils.IsAnyDurSettingOn(DURSettings) && DURWarnings != null)
                                Response.Redirect(Constants.PageNames.RX_DUR_REVIEW_MULTI_SELECT);
                        }
                    } //(base.SessionUserType == Allscripts.ePrescribe.Common.Constants.UserCategory.POB_SUPER || (base.SessionUserType == Constants.UserCategory.POB_REGULAR && isOnlyDUPDUR))
                    else
                    {
                        currentPOBWorkflowType = POBWorkflowType.POB_SOME_OR_ALL_REVIEW_REQUIRED_DUR;
                    }
                } //hasDUR
                else if (pharmacyTask.IsControlledSubstance)
                {
                    Session[Constants.SessionVariables.TaskScriptMessageId] = pharmacyTask.ScriptMessageGUID;
                    Session["TaskID"] = pharmacyTask.RxTaskId;

                    ucCSMedRefillRequestNotAllowed.ShowPopUp();
                } //controlledSubstance
                  // else

                if ((isOnlyDUPDUR && DDImismatch && !pharmacyTask.IsControlledSubstance) || !hasDUR)
                {
                    string delegateProviderID = string.Empty;

                    if (base.IsPOBUser && Session["SUPERVISING_PROVIDER_ID"] != null)
                    {
                        //
                        // get the correct delegate provider for refill request for PA with supervision when refill request is being processed by a POB user
                        //
                        delegateProviderID = Session["SUPERVISING_PROVIDER_ID"].ToString();
                    }
                    else
                    {
                        delegateProviderID = Session["DelegateProviderID"] != null ? Session["DelegateProviderID"].ToString() : base.SessionUserID;
                    }

                    ScriptMessage.ApproveMessage(
                        pharmacyTask.ScriptMessageGUID,
                        refillMedDDI,
                        daysSupply,
                        quantity,
                        dispensesRequested - 1,
                        sigText,
                        bDAW,
                        providerID,
                        notestext,
                        Constants.PrescriptionTransmissionMethod.SENT,
                        base.SessionLicenseID,
                        base.SessionUserID,
                        Convert.ToInt32(Session["SITEID"]),
                        base.ShieldSecurityToken,
                        delegateProviderID,
                        string.Empty,
                        sigID,
                        base.DBID);

                    base.ClearMedicationInfo(false);
                    Session.Remove("NOTES");
                    Session.Remove(Constants.SessionVariables.TaskScriptMessageId);
                    Session.Remove("Package");
                    Session.Remove("SUPERVISING_PROVIDER_ID");


                }
            }
            return currentPOBWorkflowType; ;
        }

        public string ApproveRXCHGPriorAuthDisplayMessage(RxTaskModel pharmacyTask, IChangeRx iChangeRx, string patientFirstName, string patientLastName)
        {
            string strReturnMessage = string.Empty;
            strReturnMessage = iChangeRx.ApprovePriorAuthWorkflow(pharmacyTask);
            return strReturnMessage;
        }

        private void ProccessDur(RxTaskModel refilltask, Rx rx, ArrayList rxList)
        {
            PageState["TaskID"] = refilltask.RxTaskId;
            PageState["Tasktype"] = refilltask.TaskType;
            rx.Notes = refilltask.PhysicianComments;

            rxList.Add(rx);
            PageState["RxList"] = rxList;
            Response.Redirect(Constants.PageNames.RX_DUR_REVIEW_MULTI_SELECT, true);
        }


        private void processTask(RxTaskModel pharmacyTask, GridDataItem gridDataItem, string authToken)
        {
            POBWorkflowType currentPOBApproveWorkflow = POBWorkflowType.POB_ANY_NON_DUR;
            StringBuilder messageBuilder = new StringBuilder();
            string patientFirstName = string.Empty;
            string patientLastName = string.Empty;
            if (!string.IsNullOrEmpty(PageState.GetStringOrEmpty("PATIENTFIRSTNAME")))
            {
                patientFirstName = PageState.GetStringOrEmpty("PATIENTFIRSTNAME");
            }
            else
            {
                patientFirstName = gridDataItem.GetDataKeyValue("patientFirstName").ToString();
            }
            if (!string.IsNullOrEmpty(PageState.GetStringOrEmpty("PATIENTLASTNAME")))
            {
                patientLastName = PageState.GetStringOrEmpty("PATIENTLASTNAME");
            }
            else
            {
                patientLastName = gridDataItem.GetDataKeyValue("patientLastName").ToString();
            }
            if (pharmacyTask.RxRequestType == RequestType.DENY)
            {
                switch (pharmacyTask.TaskType)
                {
                    case Constants.PrescriptionTaskType.REFREQ:
                        {
                            messageBuilder.Append(RejectREFREQDisplayMessage(pharmacyTask, patientFirstName, patientLastName, PageState.GetStringOrEmpty("USERID"), PageState.GetStringOrEmpty("LICENSEID")));
                            break;
                        }
                    case Constants.PrescriptionTaskType.RXCHG_PRIORAUTH:
                        {
                            UpdateChangeRxTaskModelWithSessionInformation(pharmacyTask);
                            messageBuilder.Append(RejectRXCHGPriorAuthDisplayMessage(pharmacyTask, new ChangeRxTask(), patientFirstName, patientLastName));
                            break;
                        }
                    case Constants.PrescriptionTaskType.RXCHG:
                        {
                            UpdateChangeRxTaskModelWithSessionInformation(pharmacyTask);
                            messageBuilder.Append(RejectRXCHGDisplayMessage(MasterPage.RxTask, new ChangeRxTask(), patientFirstName, patientLastName));
                            break;
                        }
                }
            }
            else if (pharmacyTask.RxRequestType== RequestType.APPROVE || pharmacyTask.RxRequestType == RequestType.APPROVE_WITH_CHANGE)
            {
                switch (pharmacyTask.TaskType)
                {
                    case Constants.PrescriptionTaskType.REFREQ:
                        {
                            currentPOBApproveWorkflow = GetPOBWorkflowTypeAndRouteRefReq(pharmacyTask, gridDataItem, patientFirstName, patientLastName);
                            if ((currentPOBApproveWorkflow == POBWorkflowType.POB_ANY_NON_DUR) || (currentPOBApproveWorkflow == POBWorkflowType.POB_REGULAR_DUP_ONLY_DUR))
                            {
                                string refillDrugDesc = gridDataItem.GetDataKeyValue("drugDescription").ToString();
                                messageBuilder.Append(new RefReq().GetREFREQApprovalDisplayMessage(refillDrugDesc, patientFirstName, patientLastName));
                            }
                            break;
                        }
                    case Constants.PrescriptionTaskType.RXCHG_PRIORAUTH:
                        {
                            UpdateChangeRxTaskModelWithSessionInformation(pharmacyTask);
                            if (!ChangeRxTask.IsValidPaCode(pharmacyTask.PriorAuthCode))
                            {
                                Response.Redirect(Constants.PageNames.PHARMACY_REFILL_DETAILS +
                                    "?MessageID="+ pharmacyTask.ScriptMessageGUID+"&PatientID="+pharmacyTask.PatientId+"&ProviderID="+pharmacyTask.UserId + "&RefillMessage=" + PRIOR_AUTH_CODE_ERROR, true);
                                PageState["REFILLMSG"] = PRIOR_AUTH_CODE_ERROR;
                            }
                            using (var timer = logger.StartTimer("CompleteApprovePriorAuthWorkflow"))
                            {
                                messageBuilder.Append(ApproveRXCHGPriorAuthDisplayMessage(MasterPage.RxTask, new ChangeRxTask(), patientFirstName, patientLastName));
                                timer.Message = $"<RefillTask>{pharmacyTask.ToLogString()}</RefillTask><RxTaskModel>{MasterPage.RxTask.ToLogString()}</RxTaskModel>";
                            }
                            break;
                        }
                    case Constants.PrescriptionTaskType.RXCHG:
                        {
                            UpdateChangeRxTaskModelWithSessionInformation(pharmacyTask);
                            ChangeRxTask changeRx = new ChangeRxTask();
                            currentPOBApproveWorkflow = GetPOBWorkflowTypeAndRouteChangeRx(pharmacyTask, gridDataItem, changeRx, authToken);
                            if (currentPOBApproveWorkflow == POBWorkflowType.POB_ANY_NON_DUR)
                            {
                                messageBuilder.Append(new ChangeRxTask().GetRXCHGApprovalDisplayMessage(MasterPage.RxTask, patientFirstName, patientLastName));
                            }
                            break;
                        }
                }
            }

            MasterPage.RefreshActiveMeds();

            if (messageBuilder.Length > 0 && (currentPOBApproveWorkflow == POBWorkflowType.POB_ANY_NON_DUR  || 
                (currentPOBApproveWorkflow == POBWorkflowType.POB_REGULAR_DUP_ONLY_DUR
                && ucMedicationHistoryCompletion.ActiveScripts == null)))
            {
                Response.Redirect(string.Concat(Constants.PageNames.PHARMACY_REFILL_SUMMARY + "?Msg=", messageBuilder.ToString().ToHTMLEncode()), true);
            }
            else if (currentPOBApproveWorkflow == POBWorkflowType.POB_SOME_OR_ALL_REVIEW_REQUIRED_DUR)
            {
                messageBuilder.Append("You do not have permission to process this request because a DUR alert is presented.");
                ucMessage2.Visible = false;
                ucMessage.MessageText = messageBuilder.ToString();
                ucMessage.Icon = Controls_Message.MessageType.ERROR;
                ucMessage.Visible = true;
            }
            //POB_REGULAR_DUP_ONLY_DUR will go through by presenting DUR overlay
            //POB_UNEXPECTED_WORKFLOW will not move
        }
        
        private bool showControlledSubstanceIndicator(string ddi, string pharmacyState, out int reconciledControlledSubstanceCode)
        {
            bool showControlledSubstanceIndicator = false;
            reconciledControlledSubstanceCode = 0;

            string sControlledSubstanceCode = string.Empty;
            int iControlledSubstanceCode = 0;

            DataSet ds = Allscripts.Impact.Medication.Load(ddi, Guid.Empty.ToString(), base.DBID);

            //check the federal code via Medispan dictionary
            if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows != null && ds.Tables[0].Rows.Count > 0)
            {
                sControlledSubstanceCode = ds.Tables[0].Rows[0]["ControlledSubstanceCode"].ToString().Trim();

                if (!string.IsNullOrEmpty(sControlledSubstanceCode) && !sControlledSubstanceCode.Equals("U", StringComparison.OrdinalIgnoreCase))
                {
                    iControlledSubstanceCode = int.Parse(sControlledSubstanceCode);
                }
            }

            //check the state code for the pharmacy and the site
            string stateControlledSubstanceCode = Prescription.GetStateControlledSubstanceCode(ddi, Session["PRACTICESTATE"].ToString(), pharmacyState, base.DBID);

            showControlledSubstanceIndicator = (!string.IsNullOrEmpty(sControlledSubstanceCode) && !sControlledSubstanceCode.Equals("U", StringComparison.OrdinalIgnoreCase)) ||
                (!string.IsNullOrEmpty(stateControlledSubstanceCode) && !stateControlledSubstanceCode.Equals("U", StringComparison.OrdinalIgnoreCase));

            //if this is a CS, get the reconciled CS
            if (showControlledSubstanceIndicator)
            {
                string sReconciledCSCode = Prescription.ReconcileControlledSubstanceCodes(sControlledSubstanceCode, stateControlledSubstanceCode);
                reconciledControlledSubstanceCode = int.Parse(sReconciledCSCode);
            }

            return showControlledSubstanceIndicator;
        }

        private void setSupervisingProviderMessage()
        {
            ucSupervisingProvider.MessageText = "Tasks are being processed under the supervision of " + base.SupervisingProviderName + ".";
            ucSupervisingProvider.Icon = Controls_Message.MessageType.INFORMATION;
            ucSupervisingProvider.Visible = true;
        }

        void ucMedicationHistoryCompletion_OnMedHistoryComplete(MedHistoryCompletionEventArgs eventArgs)
        {
            PageState.Remove(Constants.SessionVariables.PobRefReq);

            if (eventArgs.DidCompleteAll)
            {
                if (ucMedicationHistoryCompletion.IsCSMed)
                {
                    processCsMed();
                }
                else
                {
                    redirectToNextPage(eventArgs.HasOtherDur);
                }
            }
            else if(   SessionUserType == Constants.UserCategory.PHYSICIAN_ASSISTANT_SUPERVISED
                    || SessionUserType == Constants.UserCategory.POB_SUPER
                    || SessionUserType == Constants.UserCategory.POB_REGULAR)
            {
                // If the user is a PA w/super or a Super POB or a Regular POB, they don't have to select all the meds to be let through b/c they can process meds with DURs
                handlePossibleDur();
            }
            else
            {
                ucMedicationHistoryCompletion.ShowDurNotAllowed();
            }
        }

        private void redirectToNextPage(bool isDurFound)
        {
            if (isDurFound)
            {
                Response.Redirect(Constants.PageNames.RX_DUR_REVIEW_MULTI_SELECT, true);
            }
            else
            {
                var cameFrom = PageState.GetStringOrEmpty(Constants.SessionVariables.CameFrom);
                Response.Redirect($"{Constants.PageNames.RX_PROCESSOR}?From={cameFrom}&To={cameFrom}&POBRefReq=true", true);
            }
        }

        private void handlePossibleDur()
        {
            if (DURSettings.IsDURCheckSet)
            {
                redirectToNextPage(true);
            }
            else
            {
                if (ucMedicationHistoryCompletion.IsCSMed)
                {
                    processCsMed();
                }
                else
                {
                    redirectToNextPage(false);
                }
            }
        }

        private void processCsMed()
        {
            ucCSMedRefillRequestNotAllowed.ShowPopUp();
        }

        void setCSRegistryCheckbox()
        {

            chkRegistryChecked.Visible = CsMedUtil.ShouldShowCsRegistryControls(PageState);
            btnCheckRegistry.Visible = chkRegistryChecked.Visible;

        }
        #endregion Private Methods

    }
}