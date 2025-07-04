/*****************************************************************************************************
**Change History
******************************************************************************************************
**  Date:         Author:                    Description:
**----------------------------------------------------------------------------------------------------
**03/30/2010    Subhasis Nayak           #3371:Update rxdetail startdate and expiration date.
**04/07/2010	Anand Kumar Krishnan    Defect#3380: "Send to Site Pharmacy" and "Print and Send to Site Pharmacy"
 *                                      processing script types are added.
**05/28/2010    Sonal                   Mark a PBM reported med as �Received In Error� on clicking EIE 
**05/31/2010    Subhasis Nayak          #3471:Added code to distinguish brand/generic med diaplay.
**10/13/21011   Ravi Majji              artf747176 : Remove the current restrictions of formulary, casID and state
******************************************************************************************************/
using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Allscripts.Impact;
using System.Collections.Generic; 
using System.Xml;
using Telerik.Web.UI;
using Allscripts.ePrescribe.Common;
using System.Text;
using ScriptMessage = Allscripts.Impact.ScriptMsg.ScriptMessage;

namespace eRxWeb
{
public partial class TaskScriptList : BasePage
{
    #region Member Variables

    bool hasCSMeds = false;

    #endregion

    #region Properites

    #endregion

    #region Event Handlers

    protected void Page_Load(object sender, EventArgs e)
    {
        base.SetSingleClickButton(btnChangePharm);
        //base.SetSingleClickButton(btnProcess);

        ucPatMissingInfo.OnPatEditComplete += UcPatMissingInfoOnComplete;

        if (!Page.IsPostBack)
        {
            grdListScirpt.Height = ((PhysicianMasterPage)Master).getTableHeight();
            btnChangePharm.Visible = true;
            grdListScirpt.MasterTableView.GetColumn("EPAPortalLink").Visible = false;

            if (Request.QueryString.Count > 0 &&
                Request.QueryString["PatID"] != null &&
                Request.QueryString["ProvID"] != null &&
                Request.QueryString["TaskType"] != null)
            {
                Session["TL_PatientId"] = Request.QueryString["PatID"].ToString();
                Session["TL_ProviderID"] = Request.QueryString["ProvID"].ToString();
                Session["TL_TaskType"] = Request.QueryString["TaskType"].ToString();

                if (Request.QueryString["TaskType"].ToString() == "7")
                {
                    grdListScirpt.MasterTableView.GetColumn("EPAPortalLink").Visible = true;
                }
            }

            if (Session["SHOW_SEND_TO_ADM"] != null && (bool)Session["SHOW_SEND_TO_ADM"] == false)
            {
                grdListScirpt.MasterTableView.GetColumn("SendtoADM").Visible = false;
            }

            if (Session["IsPASupervised"] != null && Convert.ToBoolean(Session["IsPASupervised"]))
            {
                setSupervisingProviderMessage();
            }
        }

        Helper.SetHelpTextForPane(paneHelp, this.AppRelativeVirtualPath);

        if (Session["TL_PatientID"] != null)
        {
            ((PhysicianMasterPage)Master).SetPatientInfo(Session["TL_PatientID"].ToString());
        }

        if (Session["Message"] != null)
        {
            ucMessage.MessageText = Session["Message"].ToString();
            ucMessage.Icon = Controls_Message.MessageType.INFORMATION;
            ucMessage.Visible = true;
            Session.Remove("Message");
        }
    }

    private void UcPatMissingInfoOnComplete(PatientDemographicsEditEventArgs patDemoEventArgs)
    {
        if (patDemoEventArgs.Success)
        {
            SaveScripts();
        }
        else
        {
            if (!String.IsNullOrWhiteSpace(patDemoEventArgs.Message))
            {
                ucMessage.Visible = true;
                ucMessage.MessageText = patDemoEventArgs.Message;
                ucMessage.Icon = Controls_Message.MessageType.ERROR;
            }
        }
    }

    protected void Page_LoadComplete(object sender, EventArgs e)
    {
        int tasks = 0;
        
        ((PhysicianMasterPage)Master).toggleTabs("tasks", tasks);        
    }

    protected void RxScriptListObjDataSource_Selected(object sender, ObjectDataSourceStatusEventArgs e)
    {
        DataSet ds = (DataSet)e.ReturnValue;
        if (ds == null || ds.Tables[0].Rows.Count == 0)
        {
            Server.Transfer(Constants.PageNames.LIST_SEND_SCRIPTS + "?tasktype=" + Session["TL_TaskType"].ToString());
        }
    }

    protected void grdListScirpt_DataBound(object sender, EventArgs e)
    {
        base.AuditLogPatientInsert(ePrescribeSvc.AuditAction.PATIENT_TASK_VIEW, base.SessionPatientID);

        lblCSLegend.Visible = hasCSMeds;
    }

    protected void grdListScirpt_ItemDataBound(object sender, GridItemEventArgs e)
    {
        if (e.Item is GridDataItem)
        {
            GridDataItem dataItem = (GridDataItem)e.Item;

            string ControlledSubstanceCode = dataItem.GetDataKeyValue("ControlledSubstanceCode").ToString().Trim();
            string stateControlledSubstanceCode = dataItem.GetDataKeyValue("StateControlledSubstanceCode").ToString().Trim();
            string spi = dataItem.GetDataKeyValue("SenderId").ToString();
            string MessageData = dataItem.GetDataKeyValue("MessageData").ToString();
            string ddi = dataItem.GetDataKeyValue("DDI").ToString();
            bool pharmacyIsElectronicEnabled = bool.Parse(dataItem.GetDataKeyValue("PharmacyIsElectronicEnabled").ToString());
            var formDescription = Convert.ToString(dataItem.GetDataKeyValue("FormDescription"));

                populateRow(ControlledSubstanceCode, stateControlledSubstanceCode, spi, MessageData, pharmacyIsElectronicEnabled, ref e);
            
            Label lblMedicationSig = (Label)e.Item.FindControl("lblMedicationSig");
            if (lblMedicationSig != null)
            {
                StringBuilder description = new StringBuilder();

                description.Append(dataItem.GetDataKeyValue("MedicationName").ToString().Trim());
                description.Append(" ");

                if (!string.IsNullOrWhiteSpace(dataItem.GetDataKeyValue("DDI").ToString()))
				{
                    description.Append(dataItem.GetDataKeyValue("Strength").ToString());
                    description.Append(" ");
				    description.Append(dataItem.GetDataKeyValue("StrengthUOM").ToString());                   
				}

                if (!string.IsNullOrWhiteSpace(formDescription) && !description.ToString().Contains(formDescription)) description.Append($" {formDescription}");

                 description.Append(" - ");
                 description.Append(dataItem.GetDataKeyValue("SIGText"));

               if ((!string.IsNullOrWhiteSpace(dataItem.GetDataKeyValue("DAW").ToString()))
                    && (dataItem.GetDataKeyValue("DAW").ToString().Trim().ToUpper()) == "Y")
               {
                    description.Append(" - DAW ");
               }

                decimal dQuantity = Convert.ToDecimal(dataItem.GetDataKeyValue("Quantity"));
                string quantity = Allscripts.Impact.Utilities.StringHelper.TrimDecimal(dQuantity);

                description.Append(" <br />QUANTITY ");
                description.Append(quantity);
				description.Append(" ");
                //Reach to script pad for only new rx CS, new RX non CS, refill CS or change rx CS, so show DISPENSE only when refill CS since change Rx CS is new rx
                string dispensedRefillText = PageState.GetBooleanOrFalse(Constants.SessionVariables.IsCsRefReqWorkflow)
                ? "DISPENSE"
                : "REFILL";
                description.Append(" - "+ dispensedRefillText+" ");
                description.Append(dataItem.GetDataKeyValue("RefillQuantity").ToString());
               

                description.Append(" - Days Supply - ");
                description.Append(dataItem.GetDataKeyValue("DaysSupply").ToString());                

             if (!string.IsNullOrWhiteSpace(dataItem.GetDataKeyValue("PharmacyNotes").ToString()))
               {
                   description.Append(" - Pharm Notes: ");
                   description.Append(dataItem.GetDataKeyValue("PharmacyNotes").ToString());
               }

                    lblMedicationSig.Text = description.ToString();
		    }
            //display brand med detail in bold.
            if (!(string.IsNullOrEmpty(ddi)))
            {
                bool isGeneric = Allscripts.Impact.Medication.IsGenericMed(ddi, base.DBID);
                if (!isGeneric)
                {
                    lblMedicationSig.Font.Bold = true;
                }
            }

            if (Session["TL_TaskType"] != null && Session["TL_TaskType"].ToString() == "7")
            {
                HyperLink ePAPortalLink = (HyperLink)dataItem["EPAPortalLink"].Controls[0];
                if (ePAPortalLink != null)
                {
                    ePAPortalLink.Text = "Sign into Caremark ePA Portal";
                    ePAPortalLink.NavigateUrl = "SAML2/" + Constants.PageNames.INTERSITE_TRANSFER_CAREMARK_EPA;
                }
            }
        }
    }
    private bool IsSendingMedToMailOrder()
    {
        foreach (GridDataItem dataItem in grdListScirpt.MasterTableView.Items)
        {
            RadComboBox ddl = (RadComboBox)dataItem.FindControl("ddlDest");
            if (ddl.SelectedValue == "MOB" || ddl.SelectedValue == "DOCMOB")
            {
                return true;
            }
        }

        return false;
    }

    protected void btnProcess_Click(object sender, EventArgs e)
    {
        if (IsSendingMedToMailOrder())
        {
            if (PageState.GetStringOrEmpty("PATIENTADDRESS1") == string.Empty ||
                    PageState.GetStringOrEmpty("PATIENTCITY") == string.Empty)
            {
                ucPatMissingInfo.Show();
            }
            else
            {
                SaveScripts();
            }

        }
        else
        {
            SaveScripts();   
        }
    }

    private void SaveScripts()
    {
        Hashtable htScriptTaskRxID = new Hashtable();
        ArrayList dispProcess = new ArrayList();

        if (Convert.ToInt32(Session["UserType"]) == (int) Constants.UserCategory.POB_SUPER ||
            Convert.ToInt32(Session["UserType"]) == (int) Constants.UserCategory.POB_REGULAR)
        {
            setDelegateProvider();
        }
 
        string tiRxIDS = string.Empty;
        string peRxIDS = string.Empty;
        string licenseID = Session["LICENSEID"].ToString();
        string userID = Session["USERID"].ToString();

        RxUser user = new RxUser(base.SessionUserID, base.DBID);
        CheckBox cb;

        //spin through the rows and send or print as required
        foreach (GridDataItem dataItem in grdListScirpt.MasterTableView.Items)
        {
            RadComboBox ddl = (RadComboBox) dataItem.FindControl("ddlDest");
            string rxID = dataItem.GetDataKeyValue("RxID").ToString();
            int lineNumber = Convert.ToInt32(dataItem.GetDataKeyValue("LineNumber").ToString());
            Int64 taskID = Convert.ToInt64(dataItem.GetDataKeyValue("TaskID").ToString());
            bool IsControl = dataItem.GetDataKeyValue("ControlledSubstanceCode").ToString() == string.Empty ||
                             dataItem.GetDataKeyValue("ControlledSubstanceCode").ToString().ToUpper() == "U";
            string controlledSubstanceCode = dataItem.GetDataKeyValue("ControlledSubstanceCode").ToString();
            bool CanSendEDI = dataItem.GetDataKeyValue("SenderId").ToString() != string.Empty;
            string pharmacyNotes = dataItem.GetDataKeyValue("PharmacyNotes").ToString();

            bool bPrintPESheets = true;

            cb = dataItem.FindControl("cbSendToADM") as CheckBox;
            if (cb != null && cb.Checked)
            {
                ScriptMessage.SendNotificationTask(rxID, Session["UserID"].ToString(), Session["LicenseID"].ToString(),
                    Session["PatientID"].ToString(), null, base.DBID);
            }

            //3371:Update rxdetail startdate and expiration date.
            if (ddl.SelectedValue == "PHARM" || ddl.SelectedValue == "MOB" ||
                ddl.SelectedValue == "PRINT")
            {
                Prescription.UpdateRxDetailDates(rxID, base.DBID);
            }

            switch (ddl.SelectedValue)
            {
                case "PHARM":
                    //update status and send this message!
                    updateProviderOfRecord(rxID, controlledSubstanceCode, ref user);

                    if (Session["PHARMACYID"] == null || Session["PHARMACYID"].ToString() == "")
                    {
                        if (Session["LASTPHARMACYID"] != null && Session["LASTPHARMACYID"].ToString() != "")
                        {
                            updatePrescription(rxID, taskID, Session["LASTPHARMACYID"].ToString(), false, "SENTTOPHARMACY");
                        }
                    }
                    else
                    {
                        updatePrescription(rxID, taskID, Session["PHARMACYID"].ToString(), false, "SENTTOPHARMACY");
                    }
                    base.AuditLogPatientRxInsert(ePrescribeSvc.AuditAction.PATIENT_TASK_PROCESS, base.SessionPatientID,
                    rxID);
                    break;
                case "MOB":
                    updateProviderOfRecord(rxID, controlledSubstanceCode, ref user);
                    sendPrescriptionToMOB(rxID, taskID, "SENTTOPHARMACY");
                    base.AuditLogPatientRxInsert(ePrescribeSvc.AuditAction.PATIENT_TASK_PROCESS, base.SessionPatientID,
                    rxID);
                    break;
                case "PRINTANDSITEPHARM":
                    //when it comes here it will always have site pharmacyid
                    Prescription.UpdatePharmacyID(rxID, base.SessionSitePharmacyID, base.DBID);
                    htScriptTaskRxID.Add(taskID, rxID);
                    break;
                case "SENDSITEPHARM":
                    //when it comes here it will always have site pharmacyid
                    sendPrescriptionToSitePharm(rxID, taskID, base.SessionSitePharmacyID, "SENTTOPHARMACY");
                    base.AuditLogPatientRxInsert(ePrescribeSvc.AuditAction.PATIENT_TASK_PROCESS, base.SessionPatientID,
                    rxID);
                    break;
                case "EIE":
                    bPrintPESheets = false;
                    string extFacilityCode = Session["ExtFacilityCd"] != null ? Session["ExtFacilityCd"].ToString() : null;
                    string extGroupID = Session["ExtGroupID"] != null ? Session["ExtGroupID"].ToString() : null;
                    Prescription.EnteredInError(rxID, false, base.SessionUserID, base.SessionLicenseID, extFacilityCode,
                        extGroupID, base.DBID);
                    Prescription.UpdateRxTask(taskID, "", "", 1, Constants.PrescriptionStatus.PENDING_APPROVAL,
                        Session["USERID"].ToString(), Session["LICENSEID"].ToString(), base.DBID);

                    base.AuditLogPatientRxInsert(ePrescribeSvc.AuditAction.PATIENT_TASK_PROCESS, base.SessionPatientID,
                    rxID);
                    break;
                case "PRINT":
                    Allscripts.Impact.Prescription.UpdatePharmacyID(rxID, System.Guid.Empty.ToString(), base.DBID);
                    htScriptTaskRxID.Add(taskID, rxID);
                    updateProviderOfRecord(rxID, controlledSubstanceCode, ref user);
                    base.AuditLogPatientRxInsert(ePrescribeSvc.AuditAction.PATIENT_TASK_PROCESS, base.SessionPatientID,
                    rxID);
                    break;
            }

            if (bPrintPESheets)
            {
                if (string.IsNullOrEmpty(peRxIDS))
                {
                    peRxIDS = rxID;
                }
                else
                {
                    peRxIDS = rxID + ";" + rxID;
                }
            }
        }


        foreach (DictionaryEntry  rxid in htScriptTaskRxID)
        {
            base.AuditLogPatientRxInsert(ePrescribeSvc.AuditAction.PATIENT_TASK_PROCESS, base.SessionPatientID,
                rxid.Value.ToString());
        }

        Session["CameFrom"] = "";

        if (htScriptTaskRxID.Count > 0)
        {
            Session["CameFrom"] = Constants.PageNames.LIST_SEND_SCRIPTS + "?tasktype=" + Session["TL_TaskType"].ToString();
            Session["REMOVETASKATPRINT"] = "Y";
            Session["HTTaskRxID"] = htScriptTaskRxID;
            Response.Redirect(
                Constants.PageNames.CSS_DETECT + "?PrintScript=YES&To=" + Constants.PageNames.LIST_SEND_SCRIPTS +
                "?tasktype=" + Session["TL_TaskType"].ToString(), true);
        }
        else
        {
            Session["CameFrom"] = Constants.PageNames.LIST_SEND_SCRIPTS + "?tasktype=" + Session["TL_TaskType"].ToString();
            Response.Redirect(
                Constants.PageNames.LIST_SEND_SCRIPTS + "?tasktype=" + Session["TL_TaskType"].ToString() +
                "&TaskProcessed=true", true);
        }
    }

    protected void btnChangePharm_Click(object sender, EventArgs e)
    {
        Response.Redirect(Constants.PageNames.PHARMACY + "?from=" + Constants.PageNames.TASK_SCRIPT_LIST.ToLower());
    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
            string redirectUrl = Constants.PageNames.DOC_REFILL_MENU;
            switch ((Constants.UserCategory)Session["UserType"])
            { 
                case Constants.UserCategory.GENERAL_USER:
                case Constants.UserCategory.POB_SUPER:
                case Constants.UserCategory.POB_REGULAR:
                case Constants.UserCategory.POB_LIMITED:
                    redirectUrl = Constants.PageNames.LIST_SEND_SCRIPTS;
                    break;
                default:
                    break;
            }
            Response.Redirect(redirectUrl);
    }

    #endregion

    #region Custom Methods

    private void populateRow(string controlledSubstanceCode, string stateControlledSubstanceCode, string spi, string MessageData, bool pharmacyIsElectronicEnabled, ref GridItemEventArgs e)
    {
        RadComboBox ddl = (RadComboBox)e.Item.FindControl("ddlDest");
        Label lbl = null;
        int i = 0;
        bool eRxEnabled = false;
        
        bool sendToPharm = false;
        bool sendToMOB = false;
        bool print = false;
        bool sendToSitePharm = false;
        bool printAndSendToSitePharm = false;

        lbl = (Label)e.Item.FindControl("lblPayment");
        Label lblCSMed = (Label)e.Item.FindControl("lblCSMed");

        //check both MediSpan and state-specific controlled substance values. If either is a CS, then this med is a CS.
        bool isControlledSubstance = (!string.IsNullOrEmpty(controlledSubstanceCode) && !controlledSubstanceCode.Equals("U", StringComparison.OrdinalIgnoreCase)) ||
            (!string.IsNullOrEmpty(stateControlledSubstanceCode) && !stateControlledSubstanceCode.Equals("U", StringComparison.OrdinalIgnoreCase));        
        if (isControlledSubstance)
        {
            hasCSMeds = true;
            lblCSMed.Visible = true;
        }

        if (!isControlledSubstance && spi != "")
        {
            //if the user is a provider or PA, they will not have erx capability if they don't haved a spi. Even if the original script
            //was sent by a doc that has an spi, this provider will be the provider of record.
            if (Session["UserType"].ToString() == "1" || Convert.ToBoolean(Session["IsPA"]))
            {
                if (Session["SPI"] != null && Session["SPI"].ToString() != "")
                {
                    eRxEnabled = true;
                }
            }
            else if (spi != "")
            {
                eRxEnabled = true;
            }

            if (eRxEnabled)
            {
                btnChangePharm.Enabled = true;
                if ((Session["PHARMACYID"] != null && Session["PHARMACYID"].ToString() != "") ||
                    (Session["LASTPHARMACYID"] != null && Session["LASTPHARMACYID"].ToString() != ""))
                {
                    sendToPharm = true;
                }

                //only allow Mail order if the patient has it
                if (Session["PatientHasMOBCoverage"] != null && Session["PatientHasMOBCoverage"].ToString().Trim() == "Y" &&
                    Session["MOB_NABP"] != null && Session["MOB_NABP"].ToString().Trim() != "X")
                {
                    sendToMOB = true;
                }

                if (!string.IsNullOrEmpty(base.SessionSitePharmacyID))
                {
                    sendToSitePharm = true;
                }
            }
        }

        print = true;

        //now add all the items to the dropdown in the correct order
        if (sendToPharm)
            ddl.Items.Insert(i++, new RadComboBoxItem("Send to Pharmacy", "PHARM"));

        if (sendToMOB)
            ddl.Items.Insert(i++, new RadComboBoxItem("Send to Mail Order", "MOB"));
        
        if (printAndSendToSitePharm)
            ddl.Items.Insert(i++, new RadComboBoxItem("Print and Send to Site Pharmacy", "PRINTANDSITEPHARM"));

        if (sendToSitePharm)
            ddl.Items.Insert(i++, new RadComboBoxItem("Send to Site Pharmacy", "SENDSITEPHARM"));

        if (print)
            ddl.Items.Insert(i++, new RadComboBoxItem("Print", "PRINT"));
        
        ddl.Items.Insert(i++, new RadComboBoxItem("Leave in Task List", "LEAVE"));
        ddl.Items.Insert(i++, new RadComboBoxItem("Entered in Error", "EIE"));
    }

    

    protected void sendPrescriptionToSitePharm(string TaskRxID, Int64 TaskID, string pharmacyID, string rxStatus)
    {
        if (Session["LICENSEID"] != null)
        {
            string licenseID;
            licenseID = Session["LICENSEID"].ToString();

            Allscripts.Impact.Prescription.UpdatePharmacyID(TaskRxID, pharmacyID, false, true,false, base.DBID); // Used to update the Pharmacy Id  

            //Update the prescription status to NEW
            Prescription.UpdatePrescriptionStatus(
                TaskRxID,
                1,
                Constants.PrescriptionStatus.NEW,
                Session["USERID"].ToString(),
                base.DBID);

            string extFacilityCode = Session["ExtFacilityCd"] != null ? Session["ExtFacilityCd"].ToString() : null;
            Allscripts.Impact.Prescription.UpdateRxDetailStatus(Session["LICENSEID"].ToString(), Session["USERID"].ToString(), TaskRxID, rxStatus, extFacilityCode, base.DBID);

            try
            {
                string scriptId = ScriptMessage.CreateScriptMessage(TaskRxID, 1, Constants.MessageTypes.NEWRX, Session["LICENSEID"].ToString(), Session["USERID"].ToString(), base.ShieldSecurityToken,base.SessionSiteID, base.DBID);

                if (Session["STANDING"].ToString() == "1")
                {
                    ScriptMessage.SendThisMessage(scriptId, Session["LicenseID"].ToString(), Session["USERID"].ToString(), base.DBID);
                }
                scriptId = ScriptMessage.CreateDUREVTScriptMessage(TaskRxID, 1, Session["LICENSEID"].ToString(), Session["USERID"].ToString(), string.Empty, base.DBID);
                if ((Session["STANDING"].ToString() == "1") && (!string.IsNullOrEmpty(scriptId)))
                {
                    ScriptMessage.SendOutboundInfoScriptMessage(scriptId, Session["LICENSEID"].ToString(), Session["USERID"].ToString(), base.DBID);
                }
            }
            catch (Exception)
            {
                //Need to log failures in the future.
            }

            Prescription.UpdateRxTask(TaskID, string.Empty, string.Empty, 1, Constants.PrescriptionStatus.PENDING_APPROVAL, Session["USERID"].ToString(), Session["LICENSEID"].ToString(), base.DBID);
        }
    }

    private void sendPrescriptionToMOB(string rxID, Int64 taskID, string rxStatus)
    {
        string mopharmID;
        DataSet mobDS = Allscripts.Impact.Pharmacy.LoadPharmacyByNABP(Session["MOB_NABP"].ToString(), base.DBID);
        if (mobDS.Tables[0].Rows.Count > 0)
        {
            mopharmID = mobDS.Tables[0].Rows[0]["PharmacyID"].ToString();
            updatePrescription(rxID, taskID, mopharmID, true, rxStatus);
        }
    }

    private void setSupervisingProviderMessage()
    {
        ucSupervisingProvider.MessageText = "Tasks are being processed under the supervision of " + base.DelegateProviderName + ".";
        ucSupervisingProvider.Icon = Controls_Message.MessageType.INFORMATION;
        ucSupervisingProvider.Visible = true;
    }

    protected void updatePrescription(string TaskRxID, Int64 TaskID, string pharmacyID, bool isMailOrder, string rxStatus)
    {
        if (Session["LICENSEID"] != null)
        {
            string licenseID;
            licenseID = Session["LICENSEID"].ToString();

            Allscripts.Impact.Prescription.UpdatePharmacyID(TaskRxID, pharmacyID, isMailOrder, base.DBID); // Used to update the Pharmacy Id  

            //Update the prescription status to NEW
            Prescription.UpdatePrescriptionStatus(
                TaskRxID,
                1,
                Constants.PrescriptionStatus.NEW,
                Session["USERID"].ToString(),
                base.DBID);

            string extFacilityCode = Session["ExtFacilityCd"] != null ? Session["ExtFacilityCd"].ToString() : null;
            Allscripts.Impact.Prescription.UpdateRxDetailStatus(Session["LICENSEID"].ToString(), Session["USERID"].ToString(), TaskRxID, rxStatus, extFacilityCode, base.DBID);

            try
            {
                string scriptId = ScriptMessage.CreateScriptMessage(TaskRxID, 1, Constants.MessageTypes.NEWRX, Session["LICENSEID"].ToString(), Session["USERID"].ToString(), base.ShieldSecurityToken, base.SessionSiteID,base.DBID);

                if (Session["STANDING"].ToString() == "1")
                {
                    ScriptMessage.SendThisMessage(scriptId, Session["LicenseID"].ToString(), Session["USERID"].ToString(), base.DBID);
                }

                scriptId = ScriptMessage.CreateDUREVTScriptMessage(TaskRxID, 1, Session["LICENSEID"].ToString(), Session["USERID"].ToString(), string.Empty, base.DBID);
                if ((Session["STANDING"].ToString() == "1") && (!string.IsNullOrEmpty(scriptId)))
                {
                    ScriptMessage.SendOutboundInfoScriptMessage(scriptId, Session["LICENSEID"].ToString(), Session["USERID"].ToString(), base.DBID);
                }
            }
            catch (Exception)
            {
                //Need to log failures in the future.
            }

            Prescription.UpdateRxTask(TaskID, string.Empty, string.Empty, 1, Constants.PrescriptionStatus.PENDING_APPROVAL, Session["USERID"].ToString(), Session["LICENSEID"].ToString(), base.DBID);
        }
    }

    private void updateProviderOfRecord(string rxID, string controlledSubstanceCode, ref RxUser user)
    {
        bool update = false;
        //logic to determine whether or not to update the providerid and authorizebyid to the current user
        if (Session["UserType"] != null)
        {
            if (Convert.ToInt32(Session["UserType"]) == (int)Constants.UserCategory.PROVIDER ||
                Convert.ToInt32(Session["UserType"]) == (int)Constants.UserCategory.PHYSICIAN_ASSISTANT ||
                Convert.ToInt32(Session["UserType"]) == (int)Constants.UserCategory.PHYSICIAN_ASSISTANT_SUPERVISED)
            {
                if (controlledSubstanceCode != null && controlledSubstanceCode.Trim().Length > 0 && controlledSubstanceCode != "U")
                {
                    //we have a controlled substance. if has the DEA schedule, then update. otherwise keep original provider.
                    if (user.DEAScheduleAllowed(int.Parse(controlledSubstanceCode)))
                    {
                        update = true;
                    }
                    else if (Convert.ToInt32(Session["UserType"]) == (int)Constants.UserCategory.PHYSICIAN_ASSISTANT_SUPERVISED)
                    {
                        //if this is a PA that requires supervision, check the supervising provider. if they have the appropriate schedule, then update.
                        if (base.DelegateProvider != null && base.DelegateProvider.DEAScheduleAllowed(int.Parse(controlledSubstanceCode)))
                        {
                            update = true;
                        }
                    }
                }
                else
                {
                    update = true;
                }
            }

            if (update)
            {
                string authorizeByID = base.SessionUserID;

                if (Convert.ToInt32(Session["UserType"]) == (int)Constants.UserCategory.PHYSICIAN_ASSISTANT_SUPERVISED)
                    authorizeByID = Session["DelegateProviderID"] != null ? Session["DelegateProviderID"].ToString() : base.SessionUserID;

                Prescription.UpdateProviderOfRecord(rxID, base.SessionUserID, authorizeByID, base.SessionUserID, base.SessionLicenseID, base.DBID);
            }
        }
    }

    protected void setDelegateProvider()
    {
        if (Session["DelegateProviderID"] == null && Request.QueryString["ProvID"] != null)
        {
            Session["DelegateProviderID"] = Request.QueryString["ProvID"].ToString();
        }
    }   

    #endregion
}

}