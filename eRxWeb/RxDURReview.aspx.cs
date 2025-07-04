/*****************************************************************************************************
**Change History
******************************************************************************************************
**  Date:         Author:                    Description:
**----------------------------------------------------------------------------------------------------
**03/30/2010    Subhasis Nayak           #3371:Update rxdetail startdate and expiration date. 
******************************************************************************************************/
using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Allscripts.Impact;
using System.Text;
using System.Globalization;
using System.Collections.Generic;
using Allscripts.ePrescribe.ExtensionMethods;
using Allscripts.ePrescribe.Medispan.Clinical.Model.Response;
using eRxWeb.AppCode;
using eRxWeb.AppCode.StateUtils;
using Allergy = Allscripts.ePrescribe.Medispan.Clinical.Model.Allergy;
using Constants = Allscripts.ePrescribe.Common.Constants;
using ScriptMessage = Allscripts.Impact.ScriptMsg.ScriptMessage;
using eRxWeb.Controller;

namespace eRxWeb
{
    public partial class RxDURReview : BasePage
    {
        private DataSet _dsMscDataSet = null;
        private DataTable _warningsTable = null;
        private DataRow[] _warnings = null;
        Prescription rx = null;        
        private Constants.PrescriptionTaskType? rxtasktype
        {
            get
            {
                return ViewState["rxtasktype"] != null ? (Constants.PrescriptionTaskType?)ViewState["rxtasktype"] : null;
            }
            set
            {
                ViewState["rxtasktype"] = value;
            }
        }

        public DURCheckResponse DURWarnings
        {
            get
            {
                var durWarnings = PageState[Constants.SessionVariables.DURCheckResponse];
                if (durWarnings == null)
                {
                    var request = DURMedispanUtils.ConstructDurCheckRequest(PageState.GetStringOrEmpty(Constants.SessionVariables.PatientDOB),
                                                                            PageState.GetStringOrEmpty(Constants.SessionVariables.Gender),
                                                                            PageState.Cast(Constants.SessionVariables.RxList, new List<Rx>()),
                                                                            PageState.Cast(Constants.SessionVariables.ACTIVEMEDDDILIST, new List<string>()),
                                                                            PageState.Cast(Constants.SessionVariables.DurPatientAllergies, new List<Allergy>()),
                                                                            DURSettings);

                    DDIToRxIDMap = new List<KeyValuePair<Guid, int>>();
                    request.DrugsToCheck.ForEach(drug =>
                    {
                        DDIToRxIDMap.Add(new KeyValuePair<Guid, int>(drug.ExternalId.ToGuidOr0x0(), drug.DDI));
                    });

                    durWarnings = DURMSC.PerformDURCheck(request);
                    DURCheckResponse durCheckResponse = (DURCheckResponse)durWarnings;
                    return durCheckResponse;
                }
                return (DURCheckResponse)durWarnings;
            }
            set { PageState[Constants.SessionVariables.DURCheckResponse] = value; }
        }

        public List<KeyValuePair<Guid, int>> DDIToRxIDMap
        {
            get
            {
                return (List<KeyValuePair<Guid, int>>)ViewState["DDIToRxIDMap"];
            }
            set
            {
                ViewState["DDIToRxIDMap"] = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            base.SetSingleClickButton(btnContinue);
            if (Request.Form[btnBack.ClientID.Replace("_", "$")] != null || Request.Form[btnContinue.ClientID.Replace("_", "$")] != null)
            {
                return;
            }

            // subscribe the OnDigitalSigning event handler
            this.ucEPCSDigitalSigning.OnDigitalSigningComplete += new Controls_EPCSDigitalSigning.DigitalSigningCompleteHandeler(ucEPCSDigitalSigning_OnDigitalSigningComplete);
            this.ucCSMedRefillRequestNotAllowed.OnPrintRefillRequest += new EventHandler(ucCSMedRefillRequestNotAllowed_OnPrintRefillRequest);
            this.ucCSMedRefillRequestNotAllowed.OnCancel += new EventHandler(ucCSMedRefillRequestNotAllowed_OnCancelRequest);


            base.SetSingleClickButton(btnContinue);

            if (!Page.IsPostBack)
            {
                if (Session["ReturnPath"] == null)
                    Session["ReturnPath"] = Request.FilePath;// Request.UrlReferrer.AbsolutePath;

                if (Session["RxID"] != null)
                {
                    rx = new Prescription();
                    rx.LoadFromExistingMed(Session["RXID"].ToString(), DurInfo.RetrievePrescriptionDurHandler(PageState), SessionLicenseID, SessionUserID, base.DBID);

                    List<Rx> currentRxs = new List<Rx>();
                    currentRxs.Add(new Rx(rx));
                    PageState[Constants.SessionVariables.RxList] = currentRxs;
                    DURCheckResponse durResponse = DURWarnings;
                    PageState[Constants.SessionVariables.DURCheckResponse] = durResponse;
                }
                else if (MasterPage.RxTask != null && MasterPage.RxTask.TaskType == Constants.PrescriptionTaskType.RXCHG)
                {
                    var chgRx = MasterPage.RxTask.Rx as Rx;
                    if (chgRx != null)
                    {
                        rx = new Prescription();
                        rx.LoadFromExistingMed(chgRx.RxID, DurInfo.RetrievePrescriptionDurHandler(PageState), SessionLicenseID, SessionUserID, base.DBID);
                    }
                }

                //load up the medications
                _dsMscDataSet = DURMedispanWarningAdapter.PopulateResponseData(PageState.Cast(Constants.SessionVariables.RxList, new List<Rx>()), PageState.Cast(Constants.SessionVariables.DURCheckResponse, new DURCheckResponse()));
                Session["ds"] = _dsMscDataSet;
                gridMedication.DataSource = _dsMscDataSet.Tables["DURMedication"];
                gridMedication.DataBind();

                if (gridMedication.Rows.Count > 0)
                {
                    gridMedication.SelectedIndex = 0;
                    gridMedication_SelectedIndexChanged(gridMedication, e);
                }
            }
        }     

        protected void Page_LoadComplete(object sender, EventArgs e)
        {
            ((PhysicianMasterPage)Master).hideTabs();
        }
        protected void gridMedication_SelectedIndexChanged(object sender, EventArgs e)
        {
            //get the line number
            if (Session["ds"] != null)
                _dsMscDataSet = Session["ds"] as DataSet;

            lstWarning.Items.Clear();
            txtDetails.Text = "";

            if (gridMedication.SelectedIndex < 0)
                return;

            string lineNumber = gridMedication.SelectedRow.Cells[0].Text;
            _warningsTable = _dsMscDataSet.Tables["DURWarning"];
            _warnings = _warningsTable.Select("lineNumber = " + lineNumber);

            if (_warnings.Length != 0)
            {
                foreach (DataRow dr in _warnings)
                {
                    string warningType = "";

                    switch ((DURWarningType)dr["WarningType"])
                    {
                        case DURWarningType.INTERACTION:
                            warningType = "Drug Interaction";
                            break;
                        case DURWarningType.DOSAGE:
                            warningType = "Dosage Check";
                            break;
                        case DURWarningType.DUPLICATE_THERAPY:
                            warningType = "Duplicate Therapy";
                            break;
                        case DURWarningType.PAR:
                            warningType = "Prior Adverse Reaction";
                            break;
                        case DURWarningType.FOOD_INTERACTION:
                            warningType = "Food Interaction";
                            break;
                        case DURWarningType.ALCOHOL_INTERACTION:
                            warningType = "Alcohol Interaction";
                            break;
                        case DURWarningType.CUSTOM_MED:
                            warningType = "Custom Medication";
                            break;
                    }
                    ListItem lstitem = new ListItem(warningType, dr[1].ToString());
                    lstWarning.Items.Add(lstitem);
                }

                //select the first one
                lstWarning.Items[0].Selected = true;
                lstWarning_SelectedIndexChanged(lstWarning, e);
            }
        }
        protected void lstWarning_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtDetails.Text = "";
            if (Session["ds"] != null)
                _dsMscDataSet = Session["ds"] as DataSet;
            string lineNumber = gridMedication.SelectedRow.Cells[0].Text;
            _warnings = _dsMscDataSet.Tables["DURWarning"].Select("LineNumber = " + lineNumber);
            if (_warnings == null || _warnings.Length == 0)
                return;

            if (lstWarning.SelectedIndex < 0)
                return;

            DataRow dr = _warnings[lstWarning.SelectedIndex];
            dr.BeginEdit();
            dr["Read"] = "Y";
            dr.EndEdit();
            Session["ds"] = _dsMscDataSet;
            txtDetails.Text = htmlEncode(dr["WarningText"].ToString());
        }
        protected void gridMedication_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Style["cursor"] = "pointer";
                e.Row.Attributes.Add("onclick", ClientScript.GetPostBackEventReference(gridMedication,
                      "Select$" + e.Row.RowIndex.ToString()));
            }
        }

        private string htmlEncode(string text)
        {
            if (text == null)
                return null;
            StringBuilder sb = new StringBuilder(text.Length);
            int len = text.Length;
            for (int i = 0; i < len; i++)
            {
                switch (text[i])
                {
                    case '<':
                        sb.Append("&lt;");
                        break;
                    case '>':
                        sb.Append("&gt;");
                        break;
                    case '"':
                        sb.Append("&quot;");
                        break;
                    case '&':
                        sb.Append("&amp;");
                        break;
                    default:
                        if (text[i] > 159)
                        {
                            // decimal numeric entity    
                            sb.Append("&#");
                            sb.Append(((int)text[i]).ToString(CultureInfo.InvariantCulture));
                            sb.Append(";");
                        }
                        else
                            sb.Append(text[i]);
                        break;
                }
            }
            return sb.ToString();
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            if (Session["RxID"] != null)
            {
                rx = new Prescription();
                rx.LoadFromExistingMed(Session["RXID"].ToString(), DurInfo.RetrievePrescriptionDurHandler(PageState), SessionLicenseID, SessionUserID, base.DBID);
            }


            if (rx != null)
            {
                DataSet patds = Patient.GetPatientData(rx.PatientID, Session["LICENSEID"].ToString(), Session["USERID"].ToString(), base.DBID);
                DataRow patdr = patds.Tables[0].Rows[0];

                ////not serializable!
                //RxUser currentUser = ((PhysicianMasterPage)Master).CurrentUser;

                //EAK--ONLY DELETE THE RX IF THIS IS NOT A TASK...IF TASK, CANCELING HERE SHOULD NOT NOT NOT DELETE DATA!!!!!!!!!!
                if (Session["TASKID"] == null ||
                        (Session["TASKTYPE"] != null && Session["TASKTYPE"].ToString() == "SEND_TO_ADMIN") ||
                        (Convert.ToBoolean(Session["IsDelegateProvider"]) && Session["TASKTYPE"] != null && Session["TASKTYPE"].ToString() == "RX_APPROVAL")
                    )
                {
                    //JMT--Don't delete if this is an edit script pad workflow either
                    if (Request.QueryString["Action"] != "EditScriptPad")
                    {
                        Session["REFILLMSG"] = "Rx for " + rx.DS.Tables["RxDetail"].Rows[0]["MedicationName"].ToString() + " cancelled for " + patdr["FirstName"].ToString() + " " + patdr["LastName"].ToString() + " due to DUR Warnings.";
                        rx.Delete();
                    }
                }
                else
                {
                    Session["REFILLMSG"] = "Refill cancelled for " + patdr["FirstName"].ToString() + " " + patdr["LastName"].ToString() + " due to DUR Warnings.";
                    //if this is a task, we need to deny it
                }

                if (Session[Constants.SessionVariables.TaskScriptMessageId] != null)
                {  //AG is Refill not appropriate
                    ScriptMessage.RejectMessage(Session[Constants.SessionVariables.TaskScriptMessageId].ToString(), "AG", "", Session["USERID"].ToString(), Session["LICENSEID"].ToString(), Session["RXID"] != null ? Session["RXID"].ToString() : null, base.ShieldSecurityToken, base.SessionSiteID, base.DBID);
                }
                //EAK...PLEASE CONSULT WITH ME PRIOR TO CHANGING ABOVE CODE UNDER ANY CIRCUMSTANCES!                        
                rx.SaveDURWarning(base.DBID);
            }

            if (Session["RefillTaskData"] != null)
                Session.Remove("RefillTaskData");
            Session.Remove("TASKID");
            Session.Remove("TASKTYPE");

            Session.Remove(Constants.SessionVariables.TaskScriptMessageId);
            Session.Remove("RXID");
            Session.Remove("RX");

            ClearMedicationInfo(false);

            if (Request.QueryString["From"] != null)
            {
                // Fortify issue checking redirect validation.
                string rxId = ValidateRxGuid();
                if (Request.QueryString["From"].Equals(Constants.PageNames.SIG, StringComparison.OrdinalIgnoreCase))
                {
                    //edit script pad workflow
                    Response.Redirect(Constants.PageNames.SIG + "?Mode=Edit&RxId=" + rxId);
                }
                else if (Request.QueryString["From"].Equals(Constants.PageNames.NURSE_SIG, StringComparison.OrdinalIgnoreCase))
                {
                    //edit script pad workflow
                    Response.Redirect(Constants.PageNames.NURSE_SIG + "?Mode=Edit&RxId=" + rxId);
                }
                else
                {
                    string UrlForRedirection = Constants.PageNames.UrlForRedirection(Request.QueryString["From"].ToString());
                    if (!string.IsNullOrEmpty(UrlForRedirection))
                    {
                        Response.Redirect(UrlForRedirection);
                    }
                }
            }

        }

        private string ValidateRxGuid()
        {
            Guid rxGuid = new Guid();
            string rxId = string.Empty;
            if (Request.QueryString["RxId"] != null)
            {
                if (Guid.TryParse(Request.QueryString["RxId"].ToString(), out rxGuid))
                {
                    rxId = rxGuid.ToString();
                }

            }
            return Microsoft.Security.Application.Encoder.UrlEncode(rxId);
        }
        protected void btnContinue_Click(object sender, EventArgs e)
        {
            //make sure they have seen all of the warnings and/or confirm they want to continue
            //also confirm deletions
            string message = "";
            bool isDigitalSigningOverlayShown = false;
            bool isShowPrintOptionOverlay = false;

            if (Session["ds"] != null)
                _dsMscDataSet = Session["ds"] as DataSet;
            if (Session["RxID"] != null)
            {
                rx = new Prescription();
                rx.LoadFromExistingMed(Session["RXID"].ToString(), DurInfo.RetrievePrescriptionDurHandler(PageState), SessionLicenseID, SessionUserID, base.DBID);
            }
            DataRow[] unread = _dsMscDataSet.Tables["DURWarning"].Select("Read = 'N'");
            if (unread.Length > 0)
            {
                message += "** There are [" + unread.Length + "] warnings that you have not viewed.\r\n\r\n";
            }

            bool deleted = false;
            ArrayList deletedItems = new ArrayList();
            string deletedMessages = "";

            if (deleted)
                message += "** You have decided to delete the following medications from this prescription:\r\n" + deletedMessages;

            if (message != "")
            {
                message += "\r\n\r\nAre you sure you want to continue?";

                //now mark the deleted ones
                if (deletedItems.Count > 0)
                {
                    DataTable dt = _dsMscDataSet.Tables["DURMedication"];

                    foreach (string lineNumber in deletedItems)
                    {
                        DataRow[] rowMatch = dt.Select("LineNumber = " + lineNumber);
                        if (rowMatch.Length == 1)
                        {
                            rowMatch[0].BeginEdit();
                            rowMatch[0]["Deleted"] = "Y";
                            rowMatch[0].EndEdit();
                        }
                    }
                }
            }

            //When user clicks on the Ignore button, the refill message should not be present anymore. User is not cancelling the script. 
            Session.Remove("REFILLMSG");

            if (!rx.DURResults.Tables.Contains("DURMedication") && !rx.DURResults.Tables.Contains("DURWarning"))
            {
                rx.DURResults = _dsMscDataSet;
            }
            //Check here before inserting into RXd_DETAIL table for the CreatedbyId.
            //JMT - 05/08/2009 - Why are we saving here? Seems unneccesary so commenting out.
            //rx.Save(Convert.ToInt32(Session["SITEID"]), Session["LICENSEID"].ToString(), Session["USERID"].ToString());
            //*******************************************************************************

            rx.SaveDURWarning(base.DBID);

            RxUser user = new RxUser(base.SessionUserID, base.DBID);

            if (Convert.ToBoolean(Session["IsProvider"]) || Convert.ToBoolean(Session["IsPA"])) //For providers added today 17 APril AKS
            {
                string RxStatus = "AUTHORIZEBY";
                Allscripts.Impact.Prescription.UpdateRxDetailStatus(Session["LICENSEID"].ToString(), Session["USERID"].ToString(), Session["RXID"].ToString(), RxStatus, base.DBID);
            }
            //*******************************************************************************
            if (Session["Tasktype"] != null)
            {
                Constants.PrescriptionTaskType tasktype = (Constants.PrescriptionTaskType)Session["TASKTYPE"];
                Session.Remove("TASKTYPE");

                rxtasktype = tasktype;

                Prescription.UpdateRxDetailDates(Session["RXID"].ToString(), base.DBID); //3371:Update rxdetail startdate and expiration date

                if (Convert.ToBoolean(Session["ISPROVIDER"]) || Convert.ToBoolean(Session["ISPA"]) || Convert.ToBoolean(Session["ISPASupervised"]))
                {
                    if (tasktype == Constants.PrescriptionTaskType.APPROVAL_REQUEST || tasktype == Constants.PrescriptionTaskType.RENEWAL_REQUEST)
                    {
                        string controlledSubstanceCode = rx.ControlledSubstanceCodeReconciled;

                        //check both MediSpan and state-specific controlled substance values. If either is a CS, then this med is a CS.
                        bool isControlledSubstance = (!string.IsNullOrEmpty(controlledSubstanceCode) && !controlledSubstanceCode.Equals("U", StringComparison.OrdinalIgnoreCase)) ||
                            (!string.IsNullOrEmpty(rx.StateControlledSubstanceCode) && !rx.StateControlledSubstanceCode.Equals("U", StringComparison.OrdinalIgnoreCase));

                        //If the medication is controlled substance, the medication can't be sent to pharmacy, must print the script. 
                        if (!isControlledSubstance)
                        {
                            if (tasktype == Constants.PrescriptionTaskType.APPROVAL_REQUEST || tasktype == Constants.PrescriptionTaskType.RENEWAL_REQUEST)
                            {
                                string pharmacyID = rx.DS.Tables["RxHeader"].Rows[0]["PharmacyID"].ToString();
                                Patient pat = new Patient(rx.PatientID, Session["LICENSEID"].ToString(), Session["USERID"].ToString(), base.DBID);

                                base.AuditLogPatientRxInsert(ePrescribeSvc.AuditAction.PATIENT_TASK_PROCESS, rx.PatientID, rx.rxID);

                                //If no pharmacy for the prescription, then reconcile the pharmacy
                                if (string.IsNullOrEmpty(pharmacyID) || pharmacyID.Equals(Guid.Empty.ToString()))
                                {
                                    Session["PATIENTZIP"] = pat.DS.Tables[0].Rows[0]["ZIP"].ToString();

                                    updateProviderOfRecord(rx.ID, controlledSubstanceCode, ref user, true);
                                    Session["TASKTYPE"] = tasktype;

                                    ArrayList currRxList = new ArrayList();
                                    Rx epcsRx = new Rx(rx);
                                    currRxList.Add(epcsRx);

                                    Session["RxList"] = currRxList;

                                    if (Request.QueryString["from"] != null && Request.QueryString["from"].StartsWith(Constants.PageNames.APPROVE_REFILL_TASK))
                                    {
                                        Response.Redirect(Constants.PageNames.PHARMACY + "?" + Request.QueryString);
                                    }
                                    else
                                    {
                                        Response.Redirect(Constants.PageNames.PHARMACY + "?From=" + Constants.PageNames.APPROVE_REFILL_TASK + "&To=" + Constants.PageNames.APPROVE_REFILL_TASK);
                                    }
                                }
                                else
                                {
                                    updateProviderOfRecord(rx.ID, controlledSubstanceCode, ref user, true);

                                    Prescription.ApprovePrescription(rx.ID, 1, Session["LICENSEID"].ToString(), Session["USERID"].ToString(), base.DBID);

                                    string RxStatus = "SENTTOPHARMACY";
                                    Allscripts.Impact.Prescription.UpdateRxDetailStatus(Session["LICENSEID"].ToString(), Session["USERID"].ToString(), rx.ID.ToString(), RxStatus, base.DBID);

                                    string scriptID = ScriptMessage.CreateScriptMessage(rx.ID, 1, "NEWRX", Session["LICENSEID"].ToString(), Session["USERID"].ToString(), base.ShieldSecurityToken, base.SessionSiteID, base.DBID);

                                    if (Session["STANDING"].ToString() == "1")
                                    {
                                        ScriptMessage.SendThisMessage(scriptID, Session["LicenseID"].ToString(), Session["USERID"].ToString(), base.DBID);
                                    }

                                    scriptID = ScriptMessage.CreateDUREVTScriptMessage(rx.ID, 1, Session["LicenseID"].ToString(), Session["USERID"].ToString(), string.Empty, base.DBID);
                                    if ((Session["STANDING"].ToString() == "1") && (!string.IsNullOrEmpty(scriptID)))
                                    {
                                        ScriptMessage.SendOutboundInfoScriptMessage(scriptID, Session["LicenseID"].ToString(), Session["USERID"].ToString(), base.DBID);
                                    }


                                    //Need to update the tasks
                                    if (tasktype == Constants.PrescriptionTaskType.APPROVAL_REQUEST)
                                    {
                                        Int64 taskID = Convert.ToInt64(Session["TASKID"]);
                                        Prescription.UpdateRxTask(taskID, Constants.PrescriptionTaskType.APPROVAL_REQUEST, Constants.PrescriptionTaskStatus.PROCESSED, Constants.PrescriptionStatus.NEW, null, string.Empty, Session["USERID"].ToString(), base.DBID);
                                        Session["REFILLMSG"] = "Rx for " + rx.DS.Tables["RxDetail"].Rows[0]["MedicationName"].ToString() + " approved for " + pat.FullName + ".";
                                    }
                                    else if (tasktype == Constants.PrescriptionTaskType.RENEWAL_REQUEST)
                                    {
                                        if (Session["RefilltaskData"] != null)
                                        {
                                            RxTaskModel refilltask = (RxTaskModel)Session["RefilltaskData"];
                                            Constants.PrescriptionTaskStatus taskstatus = (refilltask.RxRequestType == RequestType.APPROVE ? Constants.PrescriptionTaskStatus.ONE : Constants.PrescriptionTaskStatus.PROCESSED);

                                            Prescription.UpdateRxTask(refilltask.RxTaskId, refilltask.PhysicianComments, refilltask.IsPatientVisitRq, (int)taskstatus, Constants.PrescriptionStatus.NEW, Session["USERID"].ToString(), Session["LICENSEID"].ToString(), base.DBID);
                                            Session.Remove("RefilltaskData");
                                        }

                                        Session["REFILLMSG"] = "Rx for " + rx.DS.Tables["RxDetail"].Rows[0]["MedicationName"].ToString() + " approved for " + pat.FullName + ".";

                                        if (Request.QueryString["To"] != null)
                                            Server.Transfer(Request.QueryString["To"]);
                                    }
                                }
                            }
                            else if (Session[Constants.SessionVariables.TaskScriptMessageId] != null)
                            {

                                string ddi = "";
                                string sigText = "";
                                string sigID = string.Empty;
                                decimal quantity = 0;
                                int refillQuantity = 0;
                                int daySupply = 0;
                                bool daw = false;
                                string notes = "";
                                DataTable rxDetail = rx.DS.Tables["RxDetail"];

                                ddi = rxDetail.Rows[0]["DDI"].ToString();
                                daySupply = Convert.ToInt32(rxDetail.Rows[0]["DaysSupply"]);
                                quantity = Convert.ToDecimal(rxDetail.Rows[0]["Quantity"]);
                                refillQuantity = Convert.ToInt32(rxDetail.Rows[0]["RefillQuantity"]);
                                sigText = rxDetail.Rows[0]["SIGText"].ToString();
                                sigID = rxDetail.Rows[0]["SIGID"].ToString();
                                daw = (rxDetail.Rows[0]["DAW"].ToString() == "Y" ? true : false);
                                if (Session["NOTES"] != null)
                                {
                                    notes = Session["NOTES"].ToString().Trim();
                                }
                                var mddValue = Prescription.GetMaximumDailyDosage(rx.rxID, DBID);

                                string delegateProviderID = Session["DelegateProviderID"] != null ? Session["DelegateProviderID"].ToString() : base.SessionUserID;
                                ScriptMessage.ApproveMessage(Session[Constants.SessionVariables.TaskScriptMessageId].ToString(), ddi, daySupply, quantity, refillQuantity, sigText, daw, rx.ProviderID,
                                    notes, Constants.PrescriptionTransmissionMethod.SENT, Session["LICENSEID"].ToString(), Session["USERID"].ToString(), Convert.ToInt32(Session["SITEID"]),
                                    base.ShieldSecurityToken, delegateProviderID, mddValue, sigID, base.DBID);
                                Session.Remove(Constants.SessionVariables.TaskScriptMessageId);
                                Session.Remove("NOTES");

                                base.AuditLogPatientRxInsert(ePrescribeSvc.AuditAction.PATIENT_TASK_PROCESS, base.SessionPatientID, rx.rxID);

                                DataSet patds = Patient.GetPatientData(rx.PatientID, Session["LICENSEID"].ToString(), Session["USERID"].ToString(), base.DBID);
                                DataRow patdr = patds.Tables[0].Rows[0];
                                Session["REFILLMSG"] = "Rx for " + rx.DS.Tables["RxDetail"].Rows[0]["MedicationName"].ToString() + " approved for " + patdr["PatientName"].ToString() + ".";

                                if (Request.QueryString["To"] != null)
                                    Server.Transfer(Request.QueryString["To"]);
                            }
                        }
                        else if (isControlledSubstance && tasktype == Constants.PrescriptionTaskType.APPROVAL_REQUEST)
                        {
                            string pharmacyID = rx.DS.Tables["RxHeader"].Rows[0]["PharmacyID"].ToString();
                            Patient pat = new Patient(rx.PatientID, Session["LICENSEID"].ToString(), Session["USERID"].ToString(), base.DBID);

                            base.AuditLogPatientRxInsert(ePrescribeSvc.AuditAction.PATIENT_TASK_PROCESS, rx.PatientID, rx.rxID);

                            if (Session["SPI"] == null || !base.CanTryEPCS)
                            {
                                updateProviderOfRecord(rx.ID, controlledSubstanceCode, ref user, false);
                                //Show overlay dialog here.
                                ucCSMedRefillRequestNotAllowed.ShowPopUp();
                                //redirect to print page where we'll update, send and print
                                Hashtable htTaskRxID = new Hashtable();
                                htTaskRxID.Add(Convert.ToInt64(Session["TASKID"]), rx.ID);
                                Session["HTTaskRxID"] = htTaskRxID;

                                Session["REMOVETASKATPRINT"] = "Y";
                                isShowPrintOptionOverlay = true;
                            }
                            else if (string.IsNullOrEmpty(pharmacyID) || pharmacyID.Equals(Guid.Empty.ToString()))
                            {
                                //If no pharmacy for the prescription, then reconcile the pharmacy
                                Session["PATIENTZIP"] = pat.DS.Tables[0].Rows[0]["ZIP"].ToString();

                                updateProviderOfRecord(rx.ID, controlledSubstanceCode, ref user, true);
                                Session["TASKTYPE"] = tasktype;

                                ArrayList currRxList = new ArrayList();
                                Rx epcsRx = new Rx(rx);
                                currRxList.Add(epcsRx);

                                Session["RxList"] = currRxList;

                                if (Request.QueryString["from"] != null && Request.QueryString["from"].StartsWith(Constants.PageNames.APPROVE_REFILL_TASK))
                                {
                                    Response.Redirect(Constants.PageNames.PHARMACY + "?" + Request.QueryString);
                                }
                                else
                                {
                                    Response.Redirect(Constants.PageNames.PHARMACY + "?From=" + Constants.PageNames.APPROVE_REFILL_TASK + "&To=" + Constants.PageNames.APPROVE_REFILL_TASK);
                                }
                            }
                            else
                            {
                                updateProviderOfRecord(rx.ID, controlledSubstanceCode, ref user, true);
                                ///Align all stars here same as Pharmacy page. 
                                ///if every thing goes well then show signing overlay otherwise just redirect accordingly.

                                bool isPharmacyEPCSEnabled = false;

                                DataSet dsPharmacy = null;
                                if (!string.IsNullOrEmpty(pharmacyID))
                                {
                                    dsPharmacy = Allscripts.Impact.Pharmacy.LoadPharmacy(pharmacyID, DBID);
                                    if (dsPharmacy != null && dsPharmacy.Tables != null && dsPharmacy.Tables.Count > 0)
                                    {
                                        isPharmacyEPCSEnabled = Convert.ToBoolean(dsPharmacy.Tables[0].Rows[0]["EpcsEnabled"]);
                                    }
                                }


                                if (Session["SPI"] != null && base.CanTryEPCS)
                                {
                                    if (!isPharmacyEPCSEnabled)
                                    {
                                        Session["PATIENTZIP"] = pat.DS.Tables[0].Rows[0]["ZIP"].ToString();

                                        updateProviderOfRecord(rx.ID, controlledSubstanceCode, ref user, true);
                                        Session["TASKTYPE"] = tasktype;

                                        ArrayList currRxList = new ArrayList();
                                        Rx epcsRx = new Rx(rx);
                                        currRxList.Add(epcsRx);

                                        Session["RxList"] = currRxList;

                                        if (Request.QueryString["from"] != null && Request.QueryString["from"].StartsWith(Constants.PageNames.APPROVE_REFILL_TASK))
                                        {
                                            Response.Redirect(Constants.PageNames.PHARMACY + "?" + Request.QueryString);
                                        }
                                        else
                                        {
                                            Response.Redirect(Constants.PageNames.PHARMACY + "?From=" + Constants.PageNames.APPROVE_REFILL_TASK + "&To=" + Constants.PageNames.APPROVE_REFILL_TASK);
                                        }
                                    }
                                    else
                                    {
                                        string sControlledSubstanceCode = string.Empty;

                                        if (!string.IsNullOrEmpty(rx.DDI))
                                        {
                                            DataSet dsCSMed = Allscripts.Impact.Medication.Load(rx.DDI, Guid.Empty.ToString(), base.DBID);

                                            if (dsCSMed != null && dsCSMed.Tables != null && dsCSMed.Tables.Count > 0 && dsCSMed.Tables[0].Rows != null && dsCSMed.Tables[0].Rows.Count > 0)
                                            {
                                                sControlledSubstanceCode = dsCSMed.Tables[0].Rows[0]["ControlledSubstanceCode"].ToString().Trim();
                                            }
                                        }
                                        else
                                        {
                                            DataSet dsRxDetailCS = Prescription.GetCSCodeFromRxDetailCS(rx.rxID, base.DBID);
                                            if (dsRxDetailCS != null && dsRxDetailCS.Tables.Count > 0 && dsRxDetailCS.Tables[0].Rows.Count > 0)
                                            {
                                                sControlledSubstanceCode = dsRxDetailCS.Tables[0].Rows[0]["RxScheduleUsed"].ToString();
                                            }
                                        }

                                        string stateCSCodeForSite = Prescription.GetStateControlledSubstanceCode(rx.DDI, Session["PRACTICESTATE"].ToString(), null, base.DBID);
                                        string stateCSCodeForPharmacy = Allscripts.Impact.Prescription.GetStateControlledSubstanceCode(rx.DDI, null,
                                                                                            dsPharmacy.Tables[0].Rows[0]["State"].ToString(), base.DBID);

                                        int scheduleUsed = 0;
                                        int.TryParse(Prescription.ReconcileControlledSubstanceCodes(sControlledSubstanceCode, stateCSCodeForPharmacy, stateCSCodeForSite), out scheduleUsed);
                                        //controlSubstanceCode = scheduleUsed;

                                        if (Prescription.IsCSMedEPCSEligible(sControlledSubstanceCode, stateCSCodeForPharmacy, stateCSCodeForSite)) /*|| rx.IsFreeFormMedControlSubstance*/
                                        {
                                            // get EPCS authorized schedules for pharmacy
                                            List<string> authorizedSchedules = new List<string>();
                                            DataTable dtSchedules = Allscripts.Impact.Pharmacy.GetEPCSAuthorizedSchedulesForPharmacy(pharmacyID, DBID);

                                            foreach (DataRow drSchedule in dtSchedules.Rows)
                                            {
                                                authorizedSchedules.Add(drSchedule[0].ToString());
                                            }

                                            Session["PharmacyEPCSAuthorizedSchdules"] = authorizedSchedules;

                                            if (Session["PharmacyEPCSAuthorizedSchdules"] != null && ((List<string>)Session["PharmacyEPCSAuthorizedSchdules"]).Contains(scheduleUsed.ToString()))
                                            {
                                                if (base.SiteEPCSAuthorizedSchedules.Contains(scheduleUsed.ToString()))
                                                {
                                                    List<Rx> rxList = new List<Rx>();
                                                    Rx epcsRx = new Rx(rx);

                                                    bool MOBPharmacy = false;
                                                    MOBPharmacy = Convert.ToBoolean(dsPharmacy.Tables[0].Rows[0]["MOBFlag"]);
                                                    if (MOBPharmacy)
                                                    {
                                                        epcsRx.Destination = Patient.MOB;
                                                    }
                                                    else
                                                    {
                                                        epcsRx.Destination = Patient.PHARM;
                                                    }

                                                    rxList.Add(epcsRx);

                                                    //Show EPCS Overlay and sign process.
                                                    ucEPCSDigitalSigning.PharmacyTask = MasterPage.RxTask;
                                                    ucEPCSDigitalSigning.IsScriptForNewRx = false;
                                                    ucEPCSDigitalSigning.IsApprovalRequestWorkflow = true;
                                                    ucEPCSDigitalSigning.EPCSMEDList = rxList;
                                                    isDigitalSigningOverlayShown = true;

                                                    //ArrayList currRxList = new ArrayList();
                                                    //currRxList.Add(epcsRx);

                                                    //Session["RxList"] = currRxList;
                                                }
                                                else
                                                {
                                                    //Show overlay dialog here.
                                                    isShowPrintOptionOverlay = true;
                                                }
                                            }
                                            else
                                            {
                                                //Show overlay dialog here.
                                                isShowPrintOptionOverlay = true;
                                            }
                                        }
                                        else
                                        {
                                            //Show overlay dialog here.
                                            isShowPrintOptionOverlay = true;
                                        }
                                    }
                                }
                                else
                                {
                                    //Show overlay dialog here.
                                    isShowPrintOptionOverlay = true;
                                }

                                //Same workflow here show dialog and redirect.
                                //In case of EPCS workflow do all this approve thing on complete event.
                                if (isDigitalSigningOverlayShown)
                                {
                                    ucEPCSDigitalSigning.ShouldShowEpcsSignAndSendScreen();
                                }

                                if (isShowPrintOptionOverlay)
                                {

                                    //Show overlay dialog here.
                                    ucCSMedRefillRequestNotAllowed.ShowPopUp();
                                    //redirect to print page where we'll update, send and print
                                    Hashtable htTaskRxID = new Hashtable();
                                    htTaskRxID.Add(Convert.ToInt64(Session["TASKID"]), rx.ID);
                                    Session["HTTaskRxID"] = htTaskRxID;

                                    Session["REMOVETASKATPRINT"] = "Y";
                                }

                            }

                            Session["REFILLMSG"] = "Rx for " + rx.DS.Tables["RxDetail"].Rows[0]["MedicationName"].ToString() + " approved for " + pat.FullName + ".";
                        }
                        else
                        {
                            updateProviderOfRecord(rx.ID, controlledSubstanceCode, ref user, false);

                            Hashtable htTaskRxID = new Hashtable();
                            htTaskRxID.Add(Convert.ToInt64(Session["TASKID"]), rx.ID);
                            Session["HTTaskRxID"] = htTaskRxID;

                            Session["REMOVETASKATPRINT"] = "Y";
                            string destinationUrl;
                            if (Request.QueryString["To"] != null)
                                destinationUrl = Request.QueryString["To"];
                            else
                                destinationUrl = Constants.PageNames.APPROVE_REFILL_TASK;

                            //Mark the prescription as approved status (NEW)
                            Prescription.ApprovePrescription(rx.ID, 1, Session["LICENSEID"].ToString(), Session["USERID"].ToString(), base.DBID);

                            //Server.Transfer("MultipleView.aspx?To=" + HttpUtility.HtmlEncode(destinationUrl));
                            Server.Transfer(Constants.PageNames.CSS_DETECT + "?To=" + HttpUtility.HtmlEncode(destinationUrl));
                        }
                    }
                }
            }

            ////RxUser currentUser = ((RxIdentity)Session["UserIdentity"]).User;
            //RxUser currentUser = CheckCurrentUser;
            if (!isDigitalSigningOverlayShown && !isShowPrintOptionOverlay)
            {

                if (Request.QueryString["PrintScript"] != null && Request.QueryString["PrintScript"] == "YES")
                {
                    //Allow the print script to go through only if the current user is a provider
                    if (Convert.ToBoolean(Session["IsProvider"]))
                    {
                        //Hashtable htTaskRxID = new Hashtable();
                        //htTaskRxID.Add(0, rx.ID);
                        //Session["HTTaskRxID"] = htTaskRxID;

                        ArrayList alProcess = new ArrayList();
                        alProcess.Add(rx.ID);
                        Session["ProcessList"] = alProcess;

                        //Server.Transfer("MultipleView.aspx");
                        Server.Transfer(Constants.PageNames.CSS_DETECT);
                    }
                    else if (Convert.ToBoolean(Session["IsDelegateProvider"]))
                    {
                        //If the user is a delegate provider, then send the script to physician to review
                        //This is the end of the script generation, redirect user to the New Rx process. 

                        //this is now done in Scriptpad!
                        //sendRxToProvider(rx, DelegateProvider); //This is required to send the script to provider. 

                        Server.Transfer(Constants.PageNames.START_NEW_RX_PROCESS);
                    }
                }
                if (Request.QueryString["To"] != null)
                {
                    if (Convert.ToBoolean(Session["IsDelegateProvider"]))
                    {
                        string url = Request.QueryString["To"];
                        if (url.ToLower().Contains(Constants.PageNames.START_NEW_RX_PROCESS.ToLower()))
                        {
                            if (Session["TASKID"] == null)
                            {
                                //We do this from ScriptPad now!
                                //sendRxToProvider(rx, DelegateProvider);  //The task is created for the provider. 
                            }
                        }
                        Server.Transfer(url);
                    }
                    else
                    {
                        base.ClearMedicationInfo(false);
                        Response.Redirect(Constants.PageNames.UrlForRedirection(Request.QueryString["To"]));
                        //Server.Transfer(Request.QueryString["To"]);
                    }
                }
            }
            else
            {
                Session["ApproveWorkflowRefillTaskId"] = Session["TASKID"];
                Session["ApproveWorkflowRefillRxId"] = rx.rxID;
            }
        }

        private void sendRxToProvider(Prescription rx, RxUser providerUser)
        {
            Session["TASKID"] = Prescription.SendToPhysicianForApproval(rx.ID, Session["LICENSEID"].ToString(), Session["USERID"].ToString(), providerUser.UserID, base.DBID);
            Session["SentTo"] = DelegateProviderName;
        }

        private void updateProviderOfRecord(string rxID, string controlledSubstanceCode, ref RxUser user, bool attemptingToSendToPharmacy)
        {
            bool updateProviderOfRecord = false;
            bool updateAuthorizeByOnly = false;

            //logic to determine whether or not to update the providerid and/or authorizebyid to the current user
            if (Session["UserType"] != null)
            {
                if (Convert.ToInt32(Session["UserType"]) == (int)Constants.UserCategory.PROVIDER ||
                    Convert.ToInt32(Session["UserType"]) == (int)Constants.UserCategory.PHYSICIAN_ASSISTANT ||
                    Convert.ToInt32(Session["UserType"]) == (int)Constants.UserCategory.PHYSICIAN_ASSISTANT_SUPERVISED)
                {
                    if (attemptingToSendToPharmacy && (Session["SPI"] == null || Session["SPI"].ToString() == ""))
                    {
                        updateAuthorizeByOnly = true;
                    }
                    else if (controlledSubstanceCode != null && controlledSubstanceCode.Trim().Length > 0 && controlledSubstanceCode != "U")
                    {
                        //we have a controlled substance. if has the DEA schedule, then update. otherwise keep original provider.
                        if (user.DEAScheduleAllowed(int.Parse(controlledSubstanceCode)))
                        {
                            updateProviderOfRecord = true;
                        }
                        else if (Convert.ToInt32(Session["UserType"]) == (int)Constants.UserCategory.PHYSICIAN_ASSISTANT_SUPERVISED)
                        {
                            //if this is a PA that requires supervision, check the supervising provider. if they have the appropriate schedule, then update.
                            if (base.DelegateProvider != null && base.DelegateProvider.DEAScheduleAllowed(int.Parse(controlledSubstanceCode)))
                            {
                                updateProviderOfRecord = true;
                            }
                        }
                    }
                    else
                    {
                        updateProviderOfRecord = true;
                    }
                }

                if (updateProviderOfRecord)
                {
                    string authorizeByID = base.SessionUserID;

                    if (Convert.ToInt32(Session["UserType"]) == (int)Constants.UserCategory.PHYSICIAN_ASSISTANT_SUPERVISED)
                        authorizeByID = Session["DelegateProviderID"] != null ? Session["DelegateProviderID"].ToString() : base.SessionUserID;

                    Prescription.UpdateProviderOfRecord(rxID, base.SessionUserID, authorizeByID, base.SessionUserID, base.SessionLicenseID, base.DBID);
                }
                else if (updateAuthorizeByOnly)
                {
                    Prescription.UpdateRxDetailStatus(base.SessionLicenseID, base.SessionUserID, rxID, "AUTHORIZEBY", base.DBID);
                }
            }
        }

        void ucEPCSDigitalSigning_OnDigitalSigningComplete(DigitalSigningEventArgs dsEventArgs)
        {
            if (dsEventArgs.Success)
            {
                if (ucEPCSDigitalSigning.IsApprovalRequestWorkflow)
                {
                    //Copy and paste all the required code and just execute it
                    //Check the redirection logic.
                    if (Session["ApproveWorkflowRefillTaskId"] != null)
                    {
                        long taskID = Convert.ToInt64(Session["ApproveWorkflowRefillTaskId"]);
                        string rxID = Session["ApproveWorkflowRefillRxId"].ToString();
                        //If the task type is to approve, then mark the rx to approved status (NEW)
                        Prescription.ApprovePrescription(rxID, 1, Session["LICENSEID"].ToString(), Session["USERID"].ToString(), base.DBID);

                        long serviceTaskID = -1;
                        foreach (KeyValuePair<string, string> kvp in dsEventArgs.SignedMeds)
                        {
                            if (kvp.Key.Equals(rxID))
                            {
                                if (!string.IsNullOrEmpty(kvp.Value) && Session["STANDING"].ToString() == "1")
                                {
                                    serviceTaskID = ScriptMessage.SendThisEPCSMessage(kvp.Value, base.SessionLicenseID, base.SessionUserID, base.DBID);
                                }
                            }
                        }

                        //Update the task status
                        Prescription.UpdateRxTask(taskID, Constants.PrescriptionTaskType.APPROVAL_REQUEST, Constants.PrescriptionTaskStatus.PROCESSED, Constants.PrescriptionStatus.NEW, null, string.Empty, Session["USERID"].ToString(), base.DBID);

                        base.AuditLogPatientRxInsert(ePrescribeSvc.AuditAction.PATIENT_TASK_PROCESS, base.SessionPatientID, rxID);

                        string RxStatus = "SENTTOPHARMACY"; //Setting the status of the Prescription April 3 2007..
                        Allscripts.Impact.Prescription.UpdateRxDetailStatus(Session["LICENSEID"].ToString(), Session["USERID"].ToString(), rxID, RxStatus, base.DBID);

                        setSuccessMessage(false, rxID);
                        Session["REFILLMSG"] = null;

                        //RxUser user = new RxUser();

                        Session["ApproveWorkflowRefillTaskId"] = null;
                        Session["ApproveWorkflowRefillRxId"] = null;

                        if (Request.QueryString["To"] != null &&
                                (Request.QueryString["To"].ToString().Contains(Constants.PageNames.APPROVE_REFILL_TASK)))
                        {
                            Response.Redirect(Constants.PageNames.APPROVE_REFILL_TASK + "?Msg=" + Server.UrlEncode(Session["SuccessMsg"].ToString()));
                        }

                        if (Request.QueryString["To"] != null)
                        {
                            Response.Redirect(Constants.PageNames.UrlForRedirection(Request.QueryString["To"].ToString()));
                        }
                        else
                        {
                            Server.Transfer(Constants.PageNames.APPROVE_REFILL_TASK);
                        }

                    }
                }
            }
            else
            {
                if (dsEventArgs.ForceLogout)
                {
                    //force the user to log out if they've entered invalid credentials 3 times in a row
                    Response.Redirect(Constants.PageNames.LOGOUT);
                }
                else if (string.IsNullOrEmpty(dsEventArgs.Message))
                {
                    Session["REFILLMSG"] = "Controlled substance meds being sent electronically could not be digitally signed.  Please try again or print.";
                }
                else
                {
                    Session["REFILLMSG"] = dsEventArgs.Message;
                }
            }
        }

        void ucCSMedRefillRequestNotAllowed_OnPrintRefillRequest(object sender, EventArgs e)
        {
            //it will come here only incase of approve request workflow.

            long taskID = Convert.ToInt64(Session["ApproveWorkflowRefillTaskId"]);
            string rxID = Session["ApproveWorkflowRefillRxId"].ToString();

            //If the task type is to approve, then mark the rx to approved status (NEW)
            Prescription.ApprovePrescription(rxID, 1, Session["LICENSEID"].ToString(), Session["USERID"].ToString(), base.DBID);

            //Update the task status
            Prescription.UpdateRxTask(taskID, Constants.PrescriptionTaskType.APPROVAL_REQUEST, Constants.PrescriptionTaskStatus.PROCESSED, Constants.PrescriptionStatus.NEW, null, string.Empty, Session["USERID"].ToString(), base.DBID);


            Session["ApproveWorkflowRefillTaskId"] = null;
            Session["ApproveWorkflowRefillRxId"] = null;


            Session["REFILLMSG"] = "Controlled substance prescription has been printed.";

            base.AuditLogPatientRxInsert(ePrescribeSvc.AuditAction.PATIENT_TASK_PROCESS, base.SessionPatientID, rxID);

            rxtasktype = null;

            //Server.Transfer(Constants.PageNames.CSS_DETECT + "?To=" + HttpUtility.HtmlEncode(Constants.PageNames.APPROVE_REFILL_TASK));
            Server.Transfer(string.Concat(Constants.PageNames.CSS_DETECT, Request.QueryString != null ? string.Concat("?", Request.QueryString.ToString()) : string.Empty));
        }

        void ucCSMedRefillRequestNotAllowed_OnCancelRequest(object sender, EventArgs e)
        {
            //assigning it back because it was removed from the session while processing DUR.
            if (rxtasktype != null)
            {
                Session["Tasktype"] = rxtasktype;
                rxtasktype = null;
            }
        }

        protected void setSuccessMessage(bool msgType, string rxID)
        {
            if (Session["RXID"] != null)
            {
                if (msgType)
                {
                    DataSet dsMedName = Prescription.ChGetRXDetails(rxID, base.DBID);

                    if (Request.QueryString["Patient"] != null)
                    {
                        Session["SuccessMsg"] = "Rx for " + dsMedName.Tables[0].Rows[0]["MedicationName"].ToString() + " approved and successfully sent to default printer for " + Request.QueryString["Patient"].ToString() + ".";
                    }
                    else if (Session["PATIENTID"] != null)
                    {
                        DataSet dsPat = CHPatient.PatientSearchById(Session["PATIENTID"].ToString(), Session["LICENSEID"].ToString(), string.Empty, base.DBID);
                        Session["SuccessMsg"] = "Rx for " + dsMedName.Tables[0].Rows[0]["MedicationName"].ToString() + " approved and successfully sent to default printer for " + dsPat.Tables["Patient"].Rows[0]["LastName"].ToString() + ", " + dsPat.Tables["Patient"].Rows[0]["FirstName"].ToString() + ".";
                    }

                }
                else if (Session["SuccessMsg"] == null)
                {
                    if (Request.QueryString["Patient"] != null)
                    {
                        DataSet ds = Prescription.Load(rxID, base.DBID);
                        Session["SuccessMsg"] = "Rx for " + ds.Tables["RxDetail"].Rows[0]["MedicationName"].ToString() + " approved for " + Request.QueryString["Patient"].ToString() + ".";
                    }
                    else if (Session["PATIENTID"] != null)
                    {
                        DataSet ds = Prescription.Load(rxID, base.DBID);
                        DataSet dsPat = CHPatient.PatientSearchById(Session["PATIENTID"].ToString(), Session["LICENSEID"].ToString(), string.Empty, base.DBID);
                        Session["SuccessMsg"] = "Rx for " + ds.Tables["RxDetail"].Rows[0]["MedicationName"].ToString() + " approved for " + dsPat.Tables["Patient"].Rows[0]["LastName"].ToString() + ", " + dsPat.Tables["Patient"].Rows[0]["FirstName"].ToString() + ".";
                    }
                }
            }
        }
    }
}
