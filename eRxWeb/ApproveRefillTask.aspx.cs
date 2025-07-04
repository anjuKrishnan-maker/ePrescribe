/******************************************************************************
**Change History
*******************************************************************************
**Date:         Author:                    Description:
**-----------------------------------------------------------------------------
**04/13/2009    Anand Kumar Krishnan	Modified to show Patient as [Unregistered] 
**                                      when patient is not found in our database (issue#2584)
**09/17/2009    Dharani Reddem          Defect#2855 Display Unregistered patients tasks instead all.
**03/30/2010    Subhasis Nayak          #3371:Update rxdetail startdate and expiration date. 
**04/07/2010    Subhasis Nayak          #3354:Added code for CS med indicator.
**05/28/2010    Subhasis Nayak          #3471:Added code as per brand/generic med display changes.    
**12/22/2014    Ethan O'Brien           Added check for CS med when displaying CSRegistryCheckbox.
*******************************************************************************/
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
using System.Text.RegularExpressions;
using Allscripts.Impact;
using System.Xml;
using System.Text;
using Telerik.Web.UI;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.DatabaseSelector;
using Allscripts.ePrescribe.ExtensionMethods;
using Allscripts.ePrescribe.Medispan.Clinical.Model;
using Allscripts.ePrescribe.Medispan.Clinical.Model.Request;
using Allscripts.ePrescribe.Medispan.Clinical.Model.Response;
using Allscripts.ePrescribe.Medispan.Clinical.Model.Settings;
using Allscripts.ePrescribe.Objects;
using Allscripts.ePrescribe.Shared.Logging;
using Allscripts.Impact.Interfaces;
using Allscripts.Impact.Tasks;
using Allscripts.Impact.Tasks.Interfaces;
using ComponentSpace.SAML.Protocol;
using eRxWeb.AppCode;
using eRxWeb.AppCode.Interfaces;
using eRxWeb.AppCode.StateUtils;
using eRxWeb.AppCode.Tasks;
using NLog;
using ServiceStack;
using Image = System.Web.UI.WebControls.Image;
using Patient = Allscripts.Impact.Patient;
using Rx = Allscripts.Impact.Rx;
using RxUser = Allscripts.Impact.RxUser;
using ConfigKeys = Allscripts.Impact.ConfigKeys;
using eRxWeb.State;
using iTextSharp.text;
using ServiceStack.Text;
using Allergy = Allscripts.ePrescribe.Medispan.Clinical.Model.Allergy;
using Constants = Allscripts.ePrescribe.Common.Constants;
using DURSettings = Allscripts.ePrescribe.Medispan.Clinical.Model.Settings.DURSettings;
using ListItem = System.Web.UI.WebControls.ListItem;
using PatientCoverage = Allscripts.Impact.PatientCoverage;
using TasksFactory = eRxWeb.AppCode.Tasks.TasksFactory;
using Telerik = Allscripts.Impact.Telerik;
using ScriptMessage = Allscripts.Impact.ScriptMsg.ScriptMessage;
using Allscripts.ePrescribe.Data;
using Allscripts.Impact.ePrescribeSvc;
using DUR = Allscripts.Impact.DUR;
using Prescription = Allscripts.Impact.Prescription;
using Allscripts.Impact.Utilities;
using ConnectionStringPointer = Allscripts.ePrescribe.DatabaseSelector.ConnectionStringPointer;
using FormularyStatus = Allscripts.Impact.FormularyStatus;

namespace eRxWeb
{
    public partial class ApproveRefillTask : BasePage
    {
        private static ILoggerEx logger = LoggerEx.GetLogger();
        #region Private Variables
        bool _formularyActive = false;
        string _savedMsg = string.Empty;
        string _schedPrintOnly = string.Empty;
        bool _hasCSMeds = false;
        protected bool _checkAltCopay = false;
        DataTable dtRefillRequest = null;
        Dictionary<string, string> _signedMeds = null;
        ArrayList _unsignedMeds = null;
        private IPrescription iPrescription = new Prescription();

        #endregion

        #region Properties

        public Rx SelectedRx { get; private set; }
        public List<Rx> SelectedRxList { get; private set; }

        private List<string> csRxList
        {
            get
            {
                //List<Rx> epcsMedList = new List<Rx>();

                if (ViewState["csRxList"] == null)
                {
                    ViewState["csRxList"] = new List<string>();
                }

                return (List<string>)ViewState["csRxList"];
            }
            set
            {
                ViewState["csRxList"] = value;
            }
        }

        private List<Rx> EpcsMedList
        {
            get
            {
                //List<Rx> epcsMedList = new List<Rx>();

                if (ViewState["EPCSMEDLIST"] == null)
                {
                    ViewState["EPCSEMEDLIST"] = new List<string>();
                }

                return (List<Rx>)ViewState["EPCSMEDLIST"];
            }
            set
            {
                ViewState["EPCSMEDLIST"] = value;
            }
        }

        #endregion

        #region Page Events

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.Form[btnChangePatient.ClientID.Replace("_", "$")] != null)
            {
                return;
            }

            setCoverageIDAvailbility();
            setRightPanelVisibility();
            //JV - commenting out single click, causing state of radio buttons to get wiped, task not processed
            //base.SetSingleClickButton(btnRefillRequest);
            if (Request.QueryString["action"] != null)
            {
                if (Request.QueryString["action"].ToString() == "del" && Request.QueryString["tid"] != null && !string.IsNullOrEmpty(Request.QueryString["tid"]))
                {
                    Int64 rxTaskID = Int64.Parse(Request.QueryString["tid"]);
                    int tasktype = int.Parse(Request.QueryString["type"]);
                    Allscripts.Impact.Prescription.UpdateRxTask(rxTaskID, (Constants.PrescriptionTaskType)tasktype, Constants.PrescriptionTaskStatus.ADMIN_CANCELED, Constants.PrescriptionStatus.ADMIN_CANCELED,
                        base.SessionUserID, string.Empty, base.SessionUserID, base.DBID);
                }
            }

            if (Request.QueryString["msg"] != null)
            {
                ucMessage2.MessageText = HttpUtility.UrlDecode(Request.QueryString["msg"]);
                ucMessage2.Icon = Controls_Message.MessageType.INFORMATION;
                ucMessage2.Visible = true;
            }

            Session.Remove("SuccessMsg");

            _schedPrintOnly = Allscripts.Impact.SystemConfig.GetSystemAttribute("SchedPrintOnly", Session["LICENSEID"].ToString(), Session["USERID"].ToString(), base.DBID);

            // subscribe the OnDigitalSigning event handler
            logger.Debug($"ucEPCSDigitalSigning.OnDigitalSigningComplete event");
            ucEPCSDigitalSigning.OnDigitalSigningComplete += new Controls_EPCSDigitalSigning.DigitalSigningCompleteHandeler(ucEPCSDigitalSigning_OnDigitalSigningComplete);

            this.ucCSMedRefillRequestNotAllowed.OnPrintRefillRequest += new EventHandler(ucCSMedRefillRequestNotAllowed_OnPrintRefillRequest);
            this.ucCSMedRefillRequestNotAllowed.OnContactProvider += new EventHandler(ucCSMedRefillRequestNotAllowed_OnContactProvider);
            ucMedicationHistoryCompletion.OnMedHistoryComplete += ucMedicationHistoryCompletion_OnMedHistoryComplete;

            if (!Page.IsPostBack)
            {
                PageState.Remove(Constants.SessionVariables.RxTask);
                PageState.Remove(Constants.SessionVariables.TaskType);
                PageState.Remove(Constants.SessionVariables.ChangeRxRequestedMedCs);
                PageState.Remove(Constants.SessionVariables.IsCsRefReqWorkflow);
                PageState.Remove(Constants.SessionVariables.IsReconcileREFREQNonCS);
                //Setting the height of row before setting height of grid.
                gridrow.Height = (((PhysicianMasterPage)Master).getTableHeight()).ToString();
                grdApproveRefillTask.Height = ((PhysicianMasterPage)Master).getTableHeight();
                if (Session["IsPASupervised"] != null && Convert.ToBoolean(Session["IsPASupervised"]))
                {
                    setSupervisingProviderMessage();
                }

                //added this to only capture PATIENT_TASK_VIEW audit actions on load from main task page.
                if ((Request.UrlReferrer != null && Request.UrlReferrer.ToString().ToLower().Contains(Constants.PageNames.DOC_REFILL_MENU.ToLower())) || Request.Url.ToString().ToLower().Contains(Constants.PageNames.DOC_REFILL_MENU.ToLower()))
                {
                    if (!string.IsNullOrEmpty(Request.QueryString["PatientId"]) && Guid.Parse(Request.QueryString["PatientId"]) != Guid.Empty)
                    {
                        base.AuditLogPatientInsert(ePrescribeSvc.AuditAction.PATIENT_TASK_VIEW, Request.QueryString["PatientId"]);
                    }
                }

                if (Request.QueryString["PatientId"] != null)
                {
                    Session["REFILLPATIENTID"] = Request.QueryString["PatientId"];
                    if (Request.QueryString["PatientId"] != Guid.Empty.ToString() && Request.QueryString["PatientId"].ToString() != string.Empty)
                    {
                        setBannerInfo(Request.QueryString["PatientId"]);
                        ClientScript.RegisterStartupScript(this.GetType(), "", string.Format("RefreshPatientHeader('{0}');", Request.QueryString["PatientId"].ToGuid()), true);
                    }
                    else
                    {
                        ((PhysicianMasterPage)Master).ClearPatientInfo();
                    }
                }
                //Keep the Parameter PhysId in session variable 
                if (Request.QueryString["PhyId"] != null)
                {
                    Session["REFILLPHYSID"] = Request.QueryString["PhyId"];
                }

                btnCheckRegistry.OnClientClick = "OpenNewWindow" + "('" + PageState.GetStringOrEmpty("STATEREGISTRYURL") + "')";

                resetSession();
                if (PageState[Constants.SessionVariables.PharmacyTasks] != null)
                {
                    processTasks(false, (List<IRxTask>)PageState[Constants.SessionVariables.PharmacyTasks]);
                }
                //processTasks(false, new ChangeRxTask());
                coverageDropDown.DataBind();
            }

            //question...is FormularyActive specific to patient?
            if (Session["FormularyActive"] != null)
                _formularyActive = (Session["FormularyActive"].ToString() == "Y" ? true : false);

            if (Page.IsPostBack)
            {
                _savedMsg = string.Empty;

                //
                // why do we need to check if __EVENTTARGET is empty?
                //
                if (Request.Params.Get("__EVENTTARGET") == string.Empty)
                {
                    bool bFound = false;
                    //a button got us here
                    foreach (string ctl in Request.Form)
                    {
                        Control c = FindControlInContainer(Page, ctl);
                        if (c is Button)
                        {
                            if (((Button)c).Text == "Process Tasks")
                            {
                                //do nothing as it's already handled
                                bFound = true;
                                break;
                            }
                        }
                    }

                    if (!bFound)
                    {
                        grdApproveRefillTask.DataBind();
                    }
                }
                else
                {
                    //This is to improve performance to avoid the grid load for row selection event
                    if (hdnReBindGrid.Value != string.Empty && Convert.ToBoolean(hdnReBindGrid.Value))
                        grdApproveRefillTask.DataBind();

                    Control c = FindControlInContainer(Page, Request.Form["__EVENTTARGET"]);

                    //build the new qstring for better nav
                    string newqstring = string.Empty;
                    foreach (string s in Request.QueryString)
                    {
                        if (s.ToLower() != "from" && s.ToLower() != "to" && s.ToLower() != "msg")
                        {
                            if (!string.IsNullOrWhiteSpace(newqstring))
                                newqstring += "&";

                            newqstring += s + "=" + Request.QueryString[s];
                        }
                    }

                    if (c is LinkButton)
                    {
                        if (((LinkButton)c).HasAttributes)
                        {
                            Session["TASKID"] = ((LinkButton)c).Attributes["taskid"];
                            string needs = ((LinkButton)c).Attributes["needs"];
                            int tasktype = Convert.ToInt32(((LinkButton)c).Attributes["tasktype"]);
                            Prescription p = new Prescription();

                            switch (needs)
                            {
                                case "dur":
                                    #region
                                    Session["RXID"] = ((LinkButton)c).Attributes["rxid"];
                                    Session["TASKTYPE"] = (Constants.PrescriptionTaskType)tasktype;// ServiceTaskType.RX_APPROVAL;
                                    if (Constants.PrescriptionTaskType.APPROVAL_REQUEST == (Constants.PrescriptionTaskType)tasktype)
                                    {
                                        p.LoadFromExistingMed(Session["RXID"].ToString(), DurInfo.RetrievePrescriptionDurHandler(PageState), SessionLicenseID, SessionUserID, base.DBID);
                                        Session["PHARMACYID"] = p.DS.Tables["RxHeader"].Rows[0]["PharmacyID"].ToString();
                                        ///not serializable
                                        //Session["Rx"] = p;                                
                                    }
                                    else if (Constants.PrescriptionTaskType.RENEWAL_REQUEST == (Constants.PrescriptionTaskType)tasktype)
                                    {
                                        RxTaskModel rtask = new RxTaskModel();
                                        rtask.RxGUID = ((LinkButton)c).Attributes["rxid"];
                                        rtask.RxTaskId = Convert.ToInt64(((LinkButton)c).Attributes["taskid"]);
                                        Session["RefillTaskData"] = rtask;

                                        p.LoadFromExistingMed(Session["RXID"].ToString(), DurInfo.RetrievePrescriptionDurHandler(PageState), SessionLicenseID, SessionUserID, base.DBID);
                                        Session["PHARMACYID"] = p.DS.Tables["RxHeader"].Rows[0]["PharmacyID"].ToString();
                                        ///not serializable
                                        //Session["Rx"] = p;
                                    }


                                    int rxCsCode;
                                    if (Int32.TryParse(p.ControlledSubstanceCodeReconciled, out rxCsCode))
                                    {
                                        Prescription.SaveCSDetails(Session["RXID"].ToString(), rxCsCode, (Constants.ControlledSubstanceTypes)p.CS_TypeID, base.GetDEANumberToBeUsed(PageState), null, base.DBID);
                                    }

                                    base.MedsWithDURs = null;
                                    if (newqstring.Length > 0)
                                    {
                                        newqstring = "?" + newqstring.ToUrlEncode();
                                        Response.Redirect(Constants.PageNames.RX_PROCESSOR + "?From=" + Constants.PageNames.APPROVE_REFILL_TASK + newqstring + "&To=" + Constants.PageNames.APPROVE_REFILL_TASK + newqstring, true);
                                    }
                                    else
                                    {
                                        Response.Redirect(Constants.PageNames.RX_PROCESSOR + "?From=" + Constants.PageNames.APPROVE_REFILL_TASK + "&To=" + Constants.PageNames.APPROVE_REFILL_TASK, true);
                                    }
                                    break;
                                #endregion
                                case "pharm":
                                    #region
                                    Session["RXID"] = ((LinkButton)c).Attributes["rxid"];
                                    p.LoadFromExistingMed(Session["RXID"].ToString(), DurInfo.RetrievePrescriptionDurHandler(PageState), SessionLicenseID, SessionUserID, base.DBID);

                                    Patient pat = new Patient(p.PatientID, Session["LICENSEID"].ToString(), Session["USERID"].ToString(), base.DBID);
                                    Session["PATIENTZIP"] = pat.DS.Tables[0].Rows[0]["ZIP"].ToString();
                                    Session["TASKTYPE"] = (Constants.PrescriptionTaskType)tasktype;

                                    ArrayList rList = new ArrayList();
                                    Rx selectedRx = new Rx(p);
                                    rList.Add(selectedRx);

                                    Session["RxList"] = rList;

                                    if (Constants.PrescriptionTaskType.RENEWAL_REQUEST == (Constants.PrescriptionTaskType)tasktype)
                                    {
                                        RxTaskModel rtask = new RxTaskModel();
                                        rtask.RxGUID = ((LinkButton)c).Attributes["rxid"];
                                        rtask.RxTaskId = Convert.ToInt64(((LinkButton)c).Attributes["taskid"]);
                                        Session["RefillTaskData"] = rtask;
                                    }

                                    int rxSchedule;
                                    if (Int32.TryParse(p.ControlledSubstanceCodeReconciled, out rxSchedule))
                                    {
                                        Prescription.SaveCSDetails(Session["RXID"].ToString(), rxSchedule, (Constants.ControlledSubstanceTypes)p.CS_TypeID, base.GetDEANumberToBeUsed(PageState), null, base.DBID);
                                    }

                                    if (newqstring.Length > 0)
                                    {
                                        newqstring = "?" + newqstring.ToUrlEncode();
                                        Response.Redirect(Constants.PageNames.PHARMACY + "?From=" + Constants.PageNames.APPROVE_REFILL_TASK + newqstring + "&To=" + Constants.PageNames.APPROVE_REFILL_TASK + newqstring);
                                    }
                                    else
                                    {
                                        Response.Redirect(Constants.PageNames.PHARMACY + "?From=" + Constants.PageNames.APPROVE_REFILL_TASK + "&To=" + Constants.PageNames.APPROVE_REFILL_TASK);
                                    }

                                    break;
                                #endregion
                                case "med":
                                    Session["TASKTYPE"] = Constants.PrescriptionTaskType.RENEWAL_REQUEST; //.NON_CHEETAH_RX_REFILL;
                                    Session["PATIENTID"] = ((LinkButton)c).Attributes["patientID"];
                                    Response.Redirect(Constants.PageNames.REDIRECT_TO_ANGULAR + "?componentName=" + Constants.PageNames.SELECT_MEDICATION + $"&componentParameters={JsonHelper.ConvertUrlParameters($"SearchText=" + ((LinkButton)c).Attributes["search"])}", true);
                                    break;
                                case "refreq":
                                    Session["TASKTYPE"] = (Constants.PrescriptionTaskType)tasktype; //ServiceTaskType.RX_REFILL;
                                    Session[Constants.SessionVariables.TaskScriptMessageId] = ((LinkButton)c).Attributes["smid"];
                                    Session["PATIENTID"] = ((LinkButton)c).Attributes["patientID"];
                                    Session["FORMULARYSTATUS"] = ((LinkButton)c).Attributes["FormularyStatus"];
                                    Session["LEVELOFPREFEREDNESS"] = ((LinkButton)c).Attributes["LevelOfPreferedness"];
                                    Session["NOTES"] = ((LinkButton)c).Attributes["Notes"];
                                    var redirect = RefReq.SetDispensedAsCurrentAndReturnRedirect(((LinkButton)c).Attributes["smid"], DBID, PageState, iPrescription);
                                    Response.Redirect(redirect, true);

                                    break;
                                case "rec":
                                    Session["TASKTYPE"] = (Constants.PrescriptionTaskType)tasktype;
                                    Response.Redirect(Constants.PageNames.APPROVE_SCRIPT_MESSAGE_PATIENT + "?smid=" + ((LinkButton)c).Attributes["smid"] + "&From=" + Constants.PageNames.APPROVE_REFILL_TASK, true);
                                    break;
                                case "change":
                                    Session["TASKTYPE"] = (Constants.PrescriptionTaskType)tasktype;
                                    Response.Redirect(Constants.PageNames.APPROVE_SCRIPT_MESSAGE_PATIENT + "?smid=" + ((LinkButton)c).Attributes["smid"] + "&From=" + Constants.PageNames.APPROVE_REFILL_TASK + "&Action=ChangePatient", true);
                                    break;
                                case "print":
                                    #region
                                    string rxID = ((LinkButton)c).Attributes["rxid"];
                                    Int64 taskID = Convert.ToInt64(((LinkButton)c).Attributes["taskid"]);

                                    p.LoadFromExistingMed(rxID, DurInfo.RetrievePrescriptionDurHandler(PageState), SessionLicenseID, SessionUserID, base.DBID);

                                    string controlledSubstanceCode = p.ControlledSubstanceCodeReconciled;
                                    if (Int32.TryParse(controlledSubstanceCode, out rxSchedule))
                                    {
                                        Prescription.SaveCSDetails(rxID, rxSchedule, (Constants.ControlledSubstanceTypes)p.CS_TypeID, base.GetDEANumberToBeUsed(PageState), null, base.DBID);
                                    }

                                    RxUser user = new RxUser(base.SessionUserID, base.DBID);
                                    updateProviderOfRecord(rxID, controlledSubstanceCode, ref user, false);
                                    //Mark the prescription as approved status (NEW)
                                    Prescription.ApprovePrescription(rxID, 1, Session["LICENSEID"].ToString(), Session["USERID"].ToString(), base.DBID);

                                    Session["REMOVETASKATPRINT"] = "Y";

                                    Hashtable htTaskRxID = new Hashtable();
                                    htTaskRxID.Add(taskID, rxID);
                                    Session["HTTaskRxID"] = htTaskRxID;

                                    string destinationUrl;
                                    destinationUrl = Constants.PageNames.APPROVE_REFILL_TASK;

                                    //Server.Transfer("MultipleView.aspx?To=" + HttpUtility.HtmlEncode(destinationUrl));
                                    Response.Redirect(Constants.PageNames.CSS_DETECT + "?To=" + HttpUtility.HtmlEncode(destinationUrl), true);
                                    break;
                                #endregion
                                case "printcsrefreq":
                                    #region
                                    {
                                        Session[Constants.SessionVariables.TaskScriptMessageId] = ((LinkButton)c).Attributes["smid"];
                                        Session["PRINTCSREFREQ"] = "Y";
                                        ScriptMessage sm = new ScriptMessage(Session[Constants.SessionVariables.TaskScriptMessageId].ToString(), base.SessionLicenseID, base.SessionUserID, base.DBID);

                                        ArrayList rxList = new ArrayList();
                                        Rx rx = new Rx();

                                        DataSet ds = Allscripts.Impact.Medication.Load(sm.DBDDID, Guid.Empty.ToString(), base.DBID);

                                        decimal quantityParsed;
                                        Decimal.TryParse(sm.RxQuantity, out quantityParsed);
                                        int daysSupplyParsed;
                                        int.TryParse(sm.RxDaysSupply, out daysSupplyParsed);
                                        int refillsParsed;
                                        int.TryParse(sm.RxRefills, out refillsParsed);
                                        DUR durChecker = new DUR
                                        {
                                            CurrentRxDDI = sm.DBDDID,
                                            CurrentRxQuantity = quantityParsed,
                                            CurrentRxDaysSupply = daysSupplyParsed,
                                            CurrentRxSigID = null,
                                            CurrentRxRefills = refillsParsed,
                                            CurrentRxMedicationName = sm.DBDrugName,
                                            CurrentRxStrength = sm.DBDrugStrength,
                                            CurrentRxStrengthUOM = sm.DBDrugStrengthUOM,
                                            CurrentRxDosageFormCode = sm.DBDrugDosageFormCode,
                                            SessionLicenseID = base.SessionLicenseID,
                                            SessionPatientID = base.SessionPatientID,
                                            SessionSex = PageState.GetStringOrEmpty("SEX"),
                                            SessionDBID = base.DBID
                                        };

                                        DURCheckResponse response =
                                            ConstructAndSendDURRequest();
                                        PageState[Constants.SessionVariables.DURCheckResponse] = response;

                                        PageState[Constants.SessionVariables.DURRefillDS] = durChecker.GetDUR(DURSettings);

                                        if (DUR.HasWarnings(response))
                                        {

                                            Session["DUR_GO_PREVIOUS"] = Constants.PageNames.APPROVE_REFILL_TASK;
                                            Session[Constants.SessionVariables.TaskScriptMessageId] = Session[Constants.SessionVariables.TaskScriptMessageId].ToString();
                                            Session["TaskID"] = ((LinkButton)c).Attributes["taskid"];
                                            Session["Tasktype"] = (Constants.PrescriptionTaskType)Convert.ToInt32(((LinkButton)c).Attributes["tasktype"]);

                                            rx = new Rx(sm, new Allscripts.Impact.Medication());

                                            //DataSet ds = Allscripts.Impact.Medication.Load(sm.DBDDID, Guid.Empty.ToString(), base.DBID);

                                            if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows != null && ds.Tables[0].Rows.Count > 0)
                                            {
                                                rx.ControlledSubstanceCode = ds.Tables[0].Rows[0]["ControlledSubstanceCode"].ToString().Trim();
                                            }

                                            rxList.Add(rx);
                                            Session["RxList"] = rxList;

                                            DataSet dsDUR = (DataSet)PageState[Constants.SessionVariables.DURRefillDS];
                                            bool hasDUP = false;

                                            if (dsDUR != null)
                                            {
                                                hasDUP = dsDUR.Tables["DUPS"].Rows.Count > 0;
                                            }

                                            if (hasDUP)
                                            {
                                                Session["DUPDUR"] = true;
                                                DataSet dsActiveScripts = Prescription.CheckPatientRxHistoryForMultiSelect(base.CurrentRx.DDI, base.SessionPatientID, base.DBID);

                                                if (dsActiveScripts.Tables[0].Rows.Count > 0)
                                                {
                                                    ucMedicationHistoryCompletion.ActiveScripts = dsActiveScripts;
                                                    ucMedicationHistoryCompletion.IsCSMed = true;
                                                    ucMedicationHistoryCompletion.SearchValue = string.Empty;
                                                    ucMedicationHistoryCompletion.LoadHistory();
                                                }
                                                return;
                                            }
                                            else
                                            {
                                                Response.Redirect(Constants.PageNames.RX_DUR_REVIEW_MULTI_SELECT, true);
                                            }
                                        }
                                        else
                                        {
                                            rx.ScriptMessageID = sm.DBScriptMessageID;
                                            rx.MedicationName = sm.DBDrugName;
                                            rx.DDI = sm.DBDDID;

                                            rx.Strength = sm.DBDrugStrength;
                                            rx.StrengthUOM = sm.DBDrugStrengthUOM;
                                            rx.DosageFormCode = sm.DBDrugDosageFormCode;
                                            rx.RouteOfAdminCode = sm.DBDrugRouteOfAdminCode;

                                            if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows != null && ds.Tables[0].Rows.Count > 0)
                                            {
                                                rx.ControlledSubstanceCode = ds.Tables[0].Rows[0]["ControlledSubstanceCode"].ToString().Trim();
                                            }

                                            int daysSupply;
                                            int.TryParse(sm.RxDaysSupply, out daysSupply);
                                            rx.DaysSupply = daysSupply;

                                            int refills;
                                            int.TryParse(sm.RxRefills, out refills);
                                            rx.Refills = refills;

                                            rx.SigText = sm.RxSIGText;

                                            var sigInfo = Allscripts.Impact.Sig.SaveFreeFormSigTextOrGetInfo(rx.SigText, Allscripts.ePrescribe.DatabaseSelector.ConnectionStringPointer.SHARED_DB);
                                            rx.SigID = Convert.ToString(sigInfo.SigID);
                                            rx.SigTypeId = (sigInfo.SigTypeID > 0) ? sigInfo.SigTypeID : (int)SigTypeEnum.SigTypeFreeForm;

                                            if (sm.RxDAW.Equals("Y", StringComparison.OrdinalIgnoreCase))
                                            {
                                                rx.DAW = true;
                                            }
                                            else
                                            {
                                                rx.DAW = false;
                                            }

                                            decimal qty;
                                            decimal.TryParse(sm.RxQuantity, out qty);

                                            rx.Quantity = qty;

                                            rx.EffectiveDate = DateTime.Now.Date;
                                            rx.Destination = "PHARM";

                                            if (iPrescription.IsValidMassOpiate(
                                                PageState.GetStringOrEmpty(Constants.SessionVariables.PracticeState),
                                                sm.DBGPI,
                                                rx.ControlledSubstanceCode,
                                                Convert.ToBoolean(Session["HasExpiredDEA"]),
                                                (List<string>)(Session["DEASCHEDULESALLOWED"]))
                                                )
                                            {
                                                rx.Notes = Constants.PartialFillUponPatientRequest;
                                            }

                                            rxList.Add(rx);
                                            Session["RxList"] = rxList;

                                            ucCSMedRefillRequestNotAllowed.ShowPopUp();
                                        }
                                    }
                                    #endregion
                                    break;
                            }
                        }
                    }
                }
            }
        }

        protected void Page_LoadComplete(object sender, EventArgs e)
        {
            int tasks = 0;

            var refillMessage = PageState.GetStringOrEmpty("REFILLMSG");
            if (!string.IsNullOrEmpty(refillMessage))
            {
                ucMessage2.MessageText = refillMessage;

                if (refillMessage.Equals(Constants.TaskingMessages.DENIAL_REASON_ERROR, StringComparison.OrdinalIgnoreCase))
                {
                    ucMessage2.Icon = Controls_Message.MessageType.ERROR;
                }
                else
                {
                    ucMessage2.Icon = Controls_Message.MessageType.INFORMATION;
                }

                ucMessage2.Visible = true;
                _savedMsg = refillMessage;
                PageState.Remove("REFILLMSG");
            }

            if (Session["SSOMode"] != null && Session["SSOMode"].ToString() == Constants.SSOMode.PATIENTLOCKDOWNMODE)
            {

                tasks = Allscripts.Impact.Patient.GetTaskCountForPatient(Session["LicenseID"].ToString(), Session["PatientID"].ToString(), base.DBID);

                ((PhysicianMasterPage)Master).toggleTabs("tasks", tasks);
            }
            else
            {

                ((PhysicianMasterPage)Master).toggleTabs("tasks", tasks);
            }
        }

        protected void Page_PreRenderComplete(Object o, EventArgs e)
        {

            if (_hasCSMeds)
            {
                chkRegistryChecked.Visible = CsMedUtil.ShouldShowCsRegistryControls(PageState);
            }

            btnCheckRegistry.Visible = chkRegistryChecked.Visible;


            if (dtRefillRequest != null)
            {
                ucMessage.Visible = true;
                ucMessage.MessageText = "'Deny' to refuse the request. 'Approve' to approve as it was originally written.";
                ucMessage.Icon = Controls_Message.MessageType.INFORMATION;
            }
        }

        void ucCSMedRefillRequestNotAllowed_OnContactProvider(object sender, EventArgs e)
        {
            ScriptMessage.RejectMessage(
                PageState.GetStringOrEmpty(Constants.SessionVariables.TaskScriptMessageId),
                string.Empty,
                "Contact provider by alternate methods regarding controlled medications",
                base.SessionUserID,
                base.SessionLicenseID,
                Guid.Empty.ToString(),
                base.ShieldSecurityToken,
                base.SessionSiteID,
                base.DBID);

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

            Session["isCSRefillNotAllowed"] = true;

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

            //medicationInfo.GPI
            //
            // TODO: add logic to add partial fill text for Mass Opiates
            //
            if (true
                //iPrescription.IsValidMassOpiate(
                //PageState.GetStringOrEmpty(Constants.SessionVariables.PracticeState),
                //gridItem.OwnerTableView.DataKeyValues[gridItem.ItemIndex]["GPI"].ToString(),
                //rx.ControlledSubstanceCode,
                //Convert.ToBoolean(Session["HasExpiredDEA"]),
                //(List<string>)(Session["DEASCHEDULESALLOWED"]))
                )
            {
                //rx.PharmacyNotes = Constants.PartialFillUponPatientRequest;
            }

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
                base.CurrentRx.SigTypeId);

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

            Hashtable htTaskRxID = new Hashtable();
            htTaskRxID.Add(Convert.ToInt64(Session["TaskID"]), rxID);
            Session["HTTaskRxID"] = htTaskRxID;

            if (base.IsUserAPrescribingUserWithCredentials)
            {
                Response.Redirect(Constants.PageNames.CSS_DETECT + "?To=" + HttpUtility.HtmlEncode(Constants.PageNames.APPROVE_REFILL_TASK), true);
            }
            else
            {
                Response.Redirect(Constants.PageNames.CSS_DETECT + "?To=" + HttpUtility.HtmlEncode(Constants.PageNames.PHARMACY_REFILL_SUMMARY), true);
            }
        }

        void ucEPCSDigitalSigning_OnDigitalSigningComplete(DigitalSigningEventArgs dsEventArgs)
        {
            logger.Debug($"ucEPCSDigitalSigning.OnDigitalSigningComplete event called{dsEventArgs.ToLogString()}");
            if (dsEventArgs.Success)
            {
                logger.Debug($"ucEPCSDigitalSigning.OnDigitalSigningComplete: IsApprovalRequestWorkflow:{ucEPCSDigitalSigning.IsApprovalRequestWorkflow}");

                if (ucEPCSDigitalSigning.IsApprovalRequestWorkflow)
                {
                    _signedMeds = dsEventArgs.SignedMeds;
                    _unsignedMeds = new ArrayList(dsEventArgs.UnsignedMeds);
                    logger.Debug($"ucEPCSDigitalSigning.OnDigitalSigningComplete: IsApprovalRequestWorkflow:{Session["ApproveWorkflowRefillTasks"].ToLogString()}");
                    if (Session["ApproveWorkflowRefillTasks"] != null)
                    {
                        List<RxTaskModel> refilltasks = (List<RxTaskModel>)Session["ApproveWorkflowRefillTasks"];
                        logger.Debug($"ucEPCSDigitalSigning.OnDigitalSigningComplete: refilltasks:{refilltasks.ToLogString()}");

                        foreach (RxTaskModel refilltask in refilltasks)
                        {
                            logger.Debug($"ucEPCSDigitalSigning.OnDigitalSigningComplete: unsignedMeds:{_unsignedMeds.ToLogString()}, refilltask: {refilltask.RxGUID}");
                            if (_unsignedMeds.Contains(refilltask.RxGUID))
                                continue;

                            logger.Debug($"ucEPCSDigitalSigning.OnDigitalSigningComplete: UpdateRxTask");
                            Prescription.UpdateRxTask(refilltask.RxTaskId, refilltask.PhysicianComments, refilltask.IsPatientVisitRq, (int)Constants.PrescriptionTaskStatus.PROCESSED, Constants.PrescriptionStatus.NEW, Session["USERID"].ToString(), Session["LICENSEID"].ToString(), base.DBID);
                            //update rx to new
                            Prescription.ApprovePrescription(refilltask.RxGUID, 1, Session["LICENSEID"].ToString(), Session["USERID"].ToString(), base.DBID);

                            long serviceTaskID = -1;
                            foreach (KeyValuePair<string, string> kvp in dsEventArgs.SignedMeds)
                            {
                                if (kvp.Key.Equals(refilltask.RxGUID))
                                {
                                    if (!string.IsNullOrEmpty(kvp.Value) && Session["STANDING"].ToString() == "1")
                                    {
                                        serviceTaskID = ScriptMessage.SendThisEPCSMessage(kvp.Value, base.SessionLicenseID, base.SessionUserID, base.DBID);
                                    }
                                }
                            }

                            string RxStatus = "SENTTOPHARMACY"; //Setting the status of the Prescription SENTBYID April 3 2007..
                            string extFacilityCode = Session["ExtFacilityCd"] != null ? Session["ExtFacilityCd"].ToString() : null;
                            Allscripts.Impact.Prescription.UpdateRxDetailStatus(Session["LICENSEID"].ToString(), Session["USERID"].ToString(), refilltask.RxGUID, RxStatus, extFacilityCode, base.DBID);
                            RxUser user = new RxUser();
                            updateProviderOfRecord(refilltask.RxGUID, null, ref user, true);
                        }

                        var approvalCount = PageState.Cast(Constants.SessionVariables.TaskCompletionCount, new TaskCompletionCounts(""));
                        PageState[Constants.SessionVariables.RefillMsg] = approvalCount.GetMessageAfterEpcsComplete(_signedMeds.Count);

                        Session["ApproveWorkflowRefillTasks"] = null;
                        Session["IsApprovalInitialized"] = null;

                        MasterPage.UpdatePatientActiveMeds();
                        if (Session["CameFrom"] != null && Session["CameFrom"].ToString() != string.Empty)
                        {
                            string cameFrom = Session["CameFrom"].ToString();

                            if (_unsignedMeds != null && _unsignedMeds.Count > 0)
                            {
                                var refillTasksToPrint = new List<RxTaskModel>();
                                foreach (var unsignedMed in _unsignedMeds)
                                {
                                    var refilltask = refilltasks.Find(x => x.RxGUID.Equals(unsignedMed.ToString(), StringComparison.InvariantCultureIgnoreCase));
                                    refillTasksToPrint.Add(refilltask);
                                    Session["RefillTasksToPrint"] = refillTasksToPrint;
                                }
                                Session["ProcessList"] = _unsignedMeds;
                                var postProcess = Constants.PageNames.CSS_DETECT + "?PrintScript=YES&From=" + cameFrom + "?Msg=" + Server.UrlEncode("Script pad successfully processed.");
                                Response.Redirect(postProcess);
                            }

                            Session.Remove("CameFrom");
                            Response.Redirect(cameFrom, true);
                        }
                        else
                        {
                            Response.Redirect(Constants.PageNames.APPROVE_REFILL_TASK, true);
                        }

                    }
                }
                else if (ucEPCSDigitalSigning.PharmacyTask != null && ucEPCSDigitalSigning.PharmacyTask.TaskType == Constants.PrescriptionTaskType.RXCHG)
                {
                    string originalRefReqScriptMessageID = null;
                    foreach (KeyValuePair<string, string> signedMed in dsEventArgs.SignedMeds)
                    {
                        saveRxChangeRequestForEPCS(signedMed.Value, signedMed.Key, originalRefReqScriptMessageID, dsEventArgs.SignedMeds.Count);
                    }
                }
                else
                {
                    string originalRefReqScriptMessageID = null;

                    foreach (KeyValuePair<string, string> signedMed in dsEventArgs.SignedMeds)
                    {
                        originalRefReqScriptMessageID = ucEPCSDigitalSigning.RefReqCrossReference[signedMed.Value];

                        saveRefillRequestForEPCS(signedMed.Value, signedMed.Key, originalRefReqScriptMessageID, dsEventArgs.SignedMeds.Count);
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
                    string errormessage = "Controlled substance meds being sent electronically could not be digitally signed.  Please try again or print.";
                    if (ucEPCSDigitalSigning.IsApprovalRequestWorkflow)
                    {
                        ucMessage2.MessageText = errormessage;
                    }
                    else
                    {
                        ucMessage.MessageText = errormessage;
                    }
                }
                else
                {
                    string errormessage = dsEventArgs.Message;

                    if (ucEPCSDigitalSigning.IsApprovalRequestWorkflow)
                    {
                        ucMessage2.Icon = Controls_Message.MessageType.ERROR;
                        ucMessage2.MessageText = dsEventArgs.Message;
                    }
                    else
                    {
                        ucMessage.Icon = Controls_Message.MessageType.ERROR;
                        ucMessage.MessageText = dsEventArgs.Message;
                    }
                }
                if (ucEPCSDigitalSigning.IsApprovalRequestWorkflow)
                {
                    ucMessage2.Icon = Controls_Message.MessageType.ERROR;
                    ucMessage2.Visible = true;
                }
                else
                {
                    ucMessage.Icon = Controls_Message.MessageType.ERROR;
                    ucMessage.Visible = true;
                }
            }
        }

        #endregion

        #region Grid Events

        protected void grdApproveRefillTask_RowDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                #region define some vars
                bool isPatientAutoRegistered = false;
                bool isPatientManuallyRegistered = false;
                bool isGeneric = true;
                bool isCSmed = false;
                bool isActionLinkButtonARequiredLink = false;
                string linkButtonTextToShowUser = string.Empty;
                bool isValidDEASchedule = true;
                GridDataItem tempDataItem = (GridDataItem)e.Item;

                int controlSubstanceCode = 6; //fake it at 6 as no such schedule exist.

                //put in the type
                Label lblType = (Label)tempDataItem.FindControl("lblType");
                var taskType = tempDataItem.GetDataKeyValue("tasktype").ToEnum<Constants.PrescriptionTaskType>();
                Session["GPI"] = tempDataItem.GetDataKeyValue("GPI").ToString();
                var scriptMessageGuid = Convert.ToString(tempDataItem.GetDataKeyValue("ScriptMessageID")).ToGuidOr0x0();

                string formularyStatus = tempDataItem.GetDataKeyValue("FormularyStatus").ToString();
                string levelOfPreferedness = tempDataItem.GetDataKeyValue("LevelOfPreferedness").ToString();
                string rxTaskID = tempDataItem.GetDataKeyValue("RxTaskId").ToString();

                var prescriptionModel = new TaskRxDetailsModel();
                DispensedRx dispensedRx = null;
                ChangeRxSmInfo changeRxInfo = null;
                var changeRxPrescriberAuth = new ChgRxPrescriberAuth(new ChangeRequestSubCodes());
                #endregion

                switch (taskType)
                {
                    case Constants.PrescriptionTaskType.APPROVAL_REQUEST:
                        lblType.Text = Constants.TaskTypeDescriptions.APPROVAL;
                        break;
                    case Constants.PrescriptionTaskType.RENEWAL_REQUEST:
                        lblType.Text = Constants.TaskTypeDescriptions.RENEWAL;
                        break;
                    case Constants.PrescriptionTaskType.REFREQ:
                        lblType.Text = Constants.TaskTypeDescriptions.PHARMACY_RENEWAL;
                        break;
                }

                displayFormularyStatus(tempDataItem);

                RadioButton rdbYes = (RadioButton)tempDataItem.FindControl("rdbYes");
                RadioButton rdbNo = (RadioButton)tempDataItem.FindControl("rdbNo");

                DropDownList ddl = (DropDownList)tempDataItem.FindControl("DenialCodes");
                Label notesLabel = (Label)tempDataItem.FindControl("notesLabel");
                TextBox notes = (TextBox)tempDataItem.FindControl("notesText");
                HtmlControl divMaxCharacters = (HtmlControl)tempDataItem.FindControl("divMaxCharacters");
                var txtPaApprovalCode = tempDataItem.FindControl("txtPAAprroveCode") as TextBox;
                var lblPaApproveCode = tempDataItem.FindControl("lblPAApproveCode") as Label;

                ddl.Style["Display"] = "none";
                ddl.DataSource = EPSBroker.GetDenialReasons(taskType, base.DBID);
                ddl.DataTextField = "Name";
                ddl.DataValueField = "ID";
                ddl.DataBind();
                ddl.Items.Insert(0, new ListItem(AppCode.ApproveRefillTask.SELECT_DENIAL_REASON, "-1"));

                //now derive prescription from unlinked Rxs
                XmlDocument messageDoc = new XmlDocument();

                //
                // why are we getting the grid's DataTable from the current DataItem and then getting that exact same info stored in the DataItem
                // from the DataTable collection via the DataRow
                //
                dtRefillRequest = ((DataRowView)tempDataItem.DataItem).DataView.Table;
                DataRow dr = dtRefillRequest.Rows[tempDataItem.ItemIndex];

                if (tempDataItem.GetDataKeyValue("MessageData") != null &&
                    !string.IsNullOrEmpty(tempDataItem.GetDataKeyValue("MessageData").ToString()))
                {
                    messageDoc.LoadXml(tempDataItem.GetDataKeyValue("MessageData").ToString());
                }

                if (messageDoc.DocumentElement != null)
                {
                    #region parse messageDoc

                    var smNodes = new ScriptMessageNodes(messageDoc);
                    if (taskType == Constants.PrescriptionTaskType.RXCHG)
                    {
                        if (smNodes.Prescription != null)
                        {
                            dispensedRx = new DispensedRx(smNodes.Prescription);
                        }
                        changeRxInfo = new ChangeRxSmInfo(smNodes, scriptMessageGuid, changeRxPrescriberAuth);

                        lblType.Text = changeRxInfo.SubType.Description;
                        if (rdbYes != null && rdbNo != null && changeRxInfo.SubType.Code != RxChangeSubTypeCode.PriorAuth)
                        {
                            rdbYes.Visible = false;
                        }
                        else if (rdbYes != null && changeRxInfo.SubType.Code == RxChangeSubTypeCode.PriorAuth)
                        {
                            ChangeRxTask.AddOnclickForRadioButtons(rdbYes, rdbNo, txtPaApprovalCode, lblPaApproveCode, ddl);
                        }
                    }
                    else
                    {
                        if (smNodes.DispensedRx != null)
                        {
                            dispensedRx = new DispensedRx(smNodes.DispensedRx);
                            rdbYes.Enabled = StringUtil.GetIntFromString(dispensedRx?.Refills, 0) > 0;
                        }
                    }

                    prescriptionModel = TaskDisplay.CreateRxDetail(smNodes, Convert.ToString(dr["FormDescription"]), Allscripts.Impact.Utilities.StringHelper.GetPharmacyName(messageDoc));

                    if (dispensedRx != null)
                    {
                        dispensedRx.ProviderOfRecord = prescriptionModel.ProviderOfRecord;
                        dispensedRx.PharmacyDetails = prescriptionModel.PharmacyDetails;
                    }

                    #endregion
                }

                var isReceivedPrescriptionInfoAvailable = IsReceivedPrescriptionInfoAvailable(changeRxInfo?.RequestedRxs, dispensedRx);

                var medicationRquested = (Label)tempDataItem.FindControl("lblRxDetails");

                //
                // this is the patient reconciliation section
                //
                if (dr["PatientID"] != DBNull.Value && dr["PatientID"].ToString() == Guid.Empty.ToString())
                {
                    //
                    // we are to assume that since there is no patient that this is a refill request and not an approval task (or renewal task)
                    //
                    #region Patient Reconciliation

                    lblMessage.Visible = false;
                    Label lblcomments = (Label)tempDataItem.FindControl("lblPatientComments");

                    string patientName = string.Concat("<b>Patient:</b> ", dr["PatientFirstName"].ToString().ToHTMLEncode(), "&nbsp;", dr["PatientLastName"].ToString().ToHTMLEncode(), " [Unreconciled]");

                    ScriptMessage sm = new ScriptMessage(dr["ScriptMessageID"].ToString(), base.SessionLicenseID, base.SessionUserID, base.DBID);

                    Image imgCSMed = (Image)tempDataItem.FindControl("imgCSMed");

                    isCSmed = showControlledSubstanceIndicator(sm.DBDDID, sm.PharmacyState);

                    if (isCSmed)
                    {
                        _hasCSMeds = isCSmed;
                        imgCSMed.Visible = isCSmed;

                        //
                        // TODO: refactor this code to get controlledSubstanceCode here and elsewhere in this method.
                        //       we are already making calls to Medication.Load and Prescription.GetStateConrolledSubstanceCode in the showControlledSubtanceIndicator method.
                        //
                        string sControlledSubstanceCode = string.Empty;
                        DataSet dsCSMed = Allscripts.Impact.Medication.Load(sm.DBDDID, Guid.Empty.ToString(), base.DBID);

                        if (dsCSMed != null && dsCSMed.Tables != null && dsCSMed.Tables.Count > 0 && dsCSMed.Tables[0].Rows != null && dsCSMed.Tables[0].Rows.Count > 0)
                        {
                            sControlledSubstanceCode = dsCSMed.Tables[0].Rows[0]["ControlledSubstanceCode"].ToString().Trim();
                        }

                        string stateCSCodeForSite = Prescription.GetStateControlledSubstanceCode(sm.DBDDID, Session["PRACTICESTATE"].ToString(), null, base.DBID);
                        string stateCSCodeForPharmacy = Allscripts.Impact.Prescription.GetStateControlledSubstanceCode(base.CurrentRx.DDI, null, sm.PharmacyState, base.DBID);

                        int scheduleUsed = 0;
                        int.TryParse(Prescription.ReconcileControlledSubstanceCodes(sControlledSubstanceCode, stateCSCodeForPharmacy, stateCSCodeForSite), out scheduleUsed);
                        controlSubstanceCode = scheduleUsed;
                    }

                    //display brand med detail in bold.
                    isGeneric = Allscripts.Impact.Medication.IsGenericMed(sm.DBDDID, base.DBID);

                    if (rdbYes != null)
                    {
                        rdbYes.Enabled = false;
                        chkRegistryChecked.Visible = false;
                    }

                    if (rdbNo != null)
                    {
                        rdbNo.Visible = true;
                    }

                    if (lblcomments != null)
                    {
                        lblcomments.Visible = false;
                    }

                    if (taskType == Constants.PrescriptionTaskType.REFREQ)
                    {
                        medicationRquested.Text = RefillTaskDisplay.BuildDispensedMedMarkup(patientName, dispensedRx);
                    }
                    else
                    {
                        medicationRquested.Text = TaskDisplay.GetFormattedRxDetails(patientName, isGeneric, prescriptionModel);
                    }

                    if (isReceivedPrescriptionInfoAvailable)
                    {
                        Label dispensedLabel = (Label)tempDataItem.FindControl("lblDispensedPrescription");

                        if (dispensedLabel != null && rdbNo != null && rdbYes != null && ddl != null)
                        {
                            if (taskType == Constants.PrescriptionTaskType.REFREQ)
                            {
                                dispensedLabel.Text = RefillTaskDisplay.BuildOriginalPrescriptionMarkup(prescriptionModel);
                            }
                            else
                            {
                                dispensedLabel.Text = ChangeRxTask.GetDispensedLabelText(taskType, changeRxInfo, dispensedRx, 
                                    AppCode.ApproveRefillTask.BoxUpUiControls(tempDataItem), rdbYes.Enabled, 
                                    UserInfo.IsPermissionGranted(UserPermissions.EpcsCanPrescribe, PageState), sm.PracticeState, sm.PharmacyState, 
                                    new ChangeRxTask(), new ChgRxPrescriberAuth(new ChangeRequestSubCodes()), prescriptionModel, DBID);
                            }
                        }
                    }

                    if (!isCSmed || (isCSmed && ((List<string>)(Session["DEASCHEDULESALLOWED"])).Contains(controlSubstanceCode.ToString())))
                    {
                        LinkButton lb = createActionLinkButton(Constants.ActionButtonText.RECONCILE_PATIENT, true, "rec", dr, taskType, null);

                        tempDataItem.Cells[8].Controls.Add(lb);
                    }

                    if (taskType == Constants.PrescriptionTaskType.REFREQ && isCSmed)
                    {
                        rdbYes.Visible = rdbYes.Enabled = false;
                    }

                    if (rdbNo != null && rdbNo.Attributes["onclick"] == null)
                    {
                        if (taskType != Constants.PrescriptionTaskType.APPROVAL_REQUEST)
                        {
                            rdbNo.Attributes.Add("onclick", "disableProcessBtn(); setControlStateForRow('" + ddl.ClientID + "', " + rdbNo.ClientID + ", '" + notesLabel.ClientID + "', '" + notes.ClientID + "', '" + divMaxCharacters.ClientID + "', " + (rdbYes.Visible ? rdbYes.ClientID : "null") + ",'" + scriptMessageGuid + "','" + formularyStatus + "','" + levelOfPreferedness + "','" + taskType.ToString() + "','" + rxTaskID + "','" + sm.DBDDID + "');");
                        }
                        else
                        {
                            rdbNo.Attributes.Add("onclick", "enableProcessBtn(); setControlStateForRow('" + ddl.ClientID + "', " + rdbNo.ClientID + ", '" + notesLabel.ClientID + "', '" + notes.ClientID + "', '" + divMaxCharacters.ClientID + "', " + (rdbYes.Visible ? rdbYes.ClientID : "null") + ",'" + scriptMessageGuid + "','" + formularyStatus + "','" + levelOfPreferedness + "','" + taskType.ToString() + "','" + rxTaskID + "','" + sm.DBDDID + "');");
                        }
                    }

                    #endregion
                }
                else // Patient ID is OK
                {
                    if (dr["RxID"] == Convert.DBNull
                        || taskType == Constants.PrescriptionTaskType.RXCHG
                        || dispensedRx?.Quantity < 1)
                    {
                        //REFREQ since it has no RxID yet
                        //Or ChangeRx

                        #region Medication Reconcilation

                        try
                        {
                            if (dr["RegisterStatus"].ToString().ToUpper() == "MANUALLY RECONCILED" && taskType == Constants.PrescriptionTaskType.REFREQ)
                            {
                                isPatientManuallyRegistered = true;
                            }

                            if (dr["RegisterStatus"].ToString().ToUpper() != "MANUALLY RECONCILED" && taskType == Constants.PrescriptionTaskType.REFREQ)
                            {
                                isPatientAutoRegistered = true;
                            }

                            ScriptMessage sm = new ScriptMessage(dr["ScriptMessageID"].ToString(), base.SessionLicenseID, base.SessionUserID, base.DBID);

                            if (sm.DBDDID != null && sm.DBDDID != string.Empty)
                            {
                                isCSmed = showControlledSubstanceIndicator(sm.DBDDID, sm.PharmacyState);

                                //display brand med detail in bold.
                                isGeneric = Allscripts.Impact.Medication.IsGenericMed(sm.DBDDID, base.DBID);
                            }
                            else if ((string.IsNullOrEmpty(sm.DBDDID)) && (sm.IsRxCompound != null && sm.IsRxCompound.Equals("Y"))) // Assume its free form medication and check for CS
                            {
                                if (sm.RxControlledSubstanceCode != string.Empty)
                                {
                                    isCSmed = true;
                                }
                            }

                            if (isCSmed)
                            {
                                Image imgCSMed = (Image)tempDataItem.FindControl("imgCSMed");
                                _hasCSMeds = isCSmed;
                                imgCSMed.Visible = isCSmed;

                                string sControlledSubstanceCode = string.Empty;
                                DataSet dsCSMed = Allscripts.Impact.Medication.Load(sm.DBDDID, Guid.Empty.ToString(), base.DBID);

                                if (dsCSMed != null && dsCSMed.Tables != null && dsCSMed.Tables.Count > 0 && dsCSMed.Tables[0].Rows != null && dsCSMed.Tables[0].Rows.Count > 0)
                                {
                                    sControlledSubstanceCode = dsCSMed.Tables[0].Rows[0]["ControlledSubstanceCode"].ToString().Trim();
                                }
                                else
                                {
                                    sControlledSubstanceCode = sm.RxControlledSubstanceCode;
                                }

                                string stateCSCodeForSite = Prescription.GetStateControlledSubstanceCode(sm.DBDDID, Session["PRACTICESTATE"].ToString(), null, base.DBID);
                                string stateCSCodeForPharmacy = Allscripts.Impact.Prescription.GetStateControlledSubstanceCode(base.CurrentRx.DDI, null, sm.PharmacyState, base.DBID);

                                int scheduleUsed = 0;
                                int.TryParse(Prescription.ReconcileControlledSubstanceCodes(sControlledSubstanceCode, stateCSCodeForPharmacy, stateCSCodeForSite), out scheduleUsed);
                                controlSubstanceCode = scheduleUsed;

                                if (!((List<string>)(Session["DEASCHEDULESALLOWED"])).Contains(controlSubstanceCode.ToString()))
                                {
                                    isValidDEASchedule = false;
                                }
                            }

                            if (taskType == Constants.PrescriptionTaskType.REFREQ)
                            {
                                medicationRquested.Text = RefillTaskDisplay.BuildDispensedMedMarkup(null, dispensedRx);
                            }
                            else
                            {
                                medicationRquested.Text = TaskDisplay.GetFormattedRxDetails(null, isGeneric, prescriptionModel);
                            }

                            if (isReceivedPrescriptionInfoAvailable)
                            {
                                Label dispensedLabel = (Label)tempDataItem.FindControl("lblDispensedPrescription");
                                var txtPhysicianComments = tempDataItem.FindControl("notesText") as TextBox;

                                if (dispensedLabel != null &&
                                    rdbNo != null &&
                                    rdbYes != null &&
                                    ddl != null &&
                                    txtPhysicianComments != null)
                                {
                                    if (taskType == Constants.PrescriptionTaskType.REFREQ)
                                    {
                                        dispensedLabel.Text = RefillTaskDisplay.BuildOriginalPrescriptionMarkup(prescriptionModel);
                                    }
                                    else if (isNonPriorAuthChangeRx(changeRxInfo))
                                    {
                                        var pharmDetails = StringHelper.GetPharmacyName(
                                            sm.PharmacyName,
                                            sm.PharmacyAddress1,
                                            sm.PharmacyAddress2,
                                            sm.PharmacyCity,
                                            sm.PharmacyState,
                                            sm.PharmacyZip,
                                            sm.PharmacyPhoneNumber);

                                        dispensedLabel.Text = ChangeRxTask.GetDispensedLabelText(taskType, changeRxInfo, dispensedRx, 
                                            AppCode.ApproveRefillTask.BoxUpUiControls(tempDataItem), rdbYes.Enabled, 
                                            UserInfo.IsPermissionGranted(UserPermissions.EpcsCanPrescribe, PageState), SessionPracticeState, 
                                            sm.PharmacyState, new ChangeRxTask(), changeRxPrescriberAuth, prescriptionModel, DBID);
                                    }
                                }
                            }

                            if (rdbYes != null)
                            {
                                if (
                                        !string.IsNullOrEmpty(sm.DBDDID) &&
                                        (!string.IsNullOrWhiteSpace(prescriptionModel.Refills) && !prescriptionModel.Refills.Equals("PRN") && !prescriptionModel.Refills.Equals(CommonTerms.NotSpecified)) &&
                                        (isPatientAutoRegistered || isPatientManuallyRegistered)
                                    )
                                {
                                    //
                                    // the patient was auto registered, so allow the user to simply approve or change
                                    //
                                    linkButtonTextToShowUser = Constants.ActionButtonText.CHANGE_REQUEST;
                                    isActionLinkButtonARequiredLink = false;

                                    if (isCSmed)
                                    {
                                        // the provider can only approve/print a cs med refill request if they have a valid DEA schedule
                                        if (isValidDEASchedule)
                                        {
                                            // if this is a CS med REFREQ and the user cannot try EPCS, disable the ProcessApproval radio button and show the print option
                                            if (!base.CanTryEPCS)
                                            {
                                                rdbYes.Enabled = false;
                                                LinkButton lbPrintCS = createActionLinkButton(Constants.ActionButtonText.PRINT_CS_RX, true, "printcsrefreq", dr, taskType, prescriptionModel.DrugDescription);
                                                tempDataItem.Cells[8].Controls.Add(lbPrintCS);
                                            }
                                        }
                                        else
                                        {
                                            rdbYes.Enabled = false;
                                            chkRegistryChecked.Visible = false;
                                        }
                                    }
                                }
                                else
                                {
                                    //
                                    // not a coded request...provide a link and make them reconcile 
                                    //
                                    rdbYes.Enabled = false;
                                    chkRegistryChecked.Visible = false;
                                    linkButtonTextToShowUser = Constants.ActionButtonText.RECONCILE_RX_NO_BREAK;
                                    isActionLinkButtonARequiredLink = true;
                                }

                                // user can only proceed to "Choose Sig" page if refill request is not or CS med or it is a CS med and the user has a valid DEA schedule and is not a RefReq task
                                if (!isCSmed || (isCSmed && isValidDEASchedule) && (taskType != Constants.PrescriptionTaskType.RXCHG && taskType != Constants.PrescriptionTaskType.REFREQ))
                                {
                                    var description = taskType == Constants.PrescriptionTaskType.REFREQ ? dispensedRx.Description : prescriptionModel.DrugDescription;
                                    LinkButton lb = createActionLinkButton(linkButtonTextToShowUser, isActionLinkButtonARequiredLink, "refreq", dr, taskType, description);
                                    tempDataItem.Cells[8].Controls.Add(lb);
                                }
                            }

                            // user can only proceed to "Reconcile Patient" page if refill request is not or CS med or it is a CS med and the user has a valid DEA schedule
                            // allowing the user to reconcile the patient would also let them proceed to the "Choose Sig" page, which we want to prohibit
                            if (((!isCSmed && (isPatientManuallyRegistered || isPatientAutoRegistered)) || (isCSmed && isValidDEASchedule && (isPatientManuallyRegistered || isPatientAutoRegistered))) && taskType != Constants.PrescriptionTaskType.RXCHG)
                            {
                                LinkButton lblPatient = createActionLinkButton(Constants.ActionButtonText.CHANGE_PATIENT, false, "change", dr, taskType, null);
                                tempDataItem.Cells[8].Controls.Add(lblPatient);
                            }


                            if (taskType == Constants.PrescriptionTaskType.REFREQ && isCSmed && UserInfo.IsPermissionGranted(UserPermissions.EpcsCanPrescribe, PageState))
                            {
                                rdbYes.Visible = rdbYes.Enabled = false;
                                LinkButton lb = createActionLinkButton(Constants.ActionButtonText.CREATE_NEW_SCRIPT, isActionLinkButtonARequiredLink, "refreq", dr, taskType, dispensedRx.Description);
                                tempDataItem.Cells[8].Controls.Add(lb);
                            }

                            if (rdbNo != null && rdbNo.Attributes["onclick"] == null)
                            {
                                if (taskType != Constants.PrescriptionTaskType.APPROVAL_REQUEST) 
                                { 
                                    rdbNo.Attributes.Add("onclick", "disableProcessBtn(); setControlStateForRow('" + ddl.ClientID + "', " + rdbNo.ClientID + ", '" + notesLabel.ClientID + "', '" + notes.ClientID + "', '" + divMaxCharacters.ClientID + "', " + (rdbYes.Visible ? rdbYes.ClientID : "null") + ",'" + scriptMessageGuid + "','" + formularyStatus + "','" + levelOfPreferedness + "','" + taskType.ToString() + "','" + rxTaskID + "','" + sm.DBDDID + "');");
                                }
                                else
                                {
                                    rdbNo.Attributes.Add("onclick", "enableProcessBtn(); setControlStateForRow('" + ddl.ClientID + "', " + rdbNo.ClientID + ", '" + notesLabel.ClientID + "', '" + notes.ClientID + "', '" + divMaxCharacters.ClientID + "', " + (rdbYes.Visible ? rdbYes.ClientID : "null") + ",'" + scriptMessageGuid + "','" + formularyStatus + "','" + levelOfPreferedness + "','" + taskType.ToString() + "','" + rxTaskID + "','" + sm.DBDDID + "');");
                                }
                            }

                            if (rdbYes != null)
                            {
                                string smid = dr["ScriptMessageID"].ToString();

                                if (Session[Constants.SessionVariables.TaskScriptMessageId] != null)
                                {
                                    if (Session[Constants.SessionVariables.TaskScriptMessageId].ToString() == smid)
                                    {
                                        rdbYes.Checked = true;
                                    }
                                }

                                if (rdbYes.Attributes["onclick"] == null)
                                {
                                    rdbYes.Attributes.Add("onclick",
                                        "enableProcessBtn(); setControlStateForRow('" + ddl.ClientID + "', " + rdbNo.ClientID + ", '" + notesLabel.ClientID +
                                        "', '" + notes.ClientID + "', '" + divMaxCharacters.ClientID + "', " +
                                        (rdbYes.Visible ? rdbYes.ClientID : "null") + ",'" + scriptMessageGuid + "','" + formularyStatus +
                                        "','" + levelOfPreferedness + "','" + taskType.ToString() + "','" + rxTaskID +
                                        "','" + sm.DBDDID + "');");
                                }
                            }

                        }
                        catch (Exception esm)
                        {
                            //something is wrong with the script message--most likely that the pharmacy isn't setup properly
                            Console.Write(esm.Message);

                            //this means that there's no corresponding script message for the task--Non-Cheetah Refill!                    
                            try
                            {
                                if (rdbYes != null)
                                {
                                    rdbYes.Enabled = false;
                                    chkRegistryChecked.Visible = false;

                                    LinkButton lb = createActionLinkButton(Constants.ActionButtonText.RECONCILE_RX, true, "med", dr, taskType, dr["Prescription"].ToString());
                                    tempDataItem.Cells[8].Controls.Add(lb);

                                }
                            }
                            catch (Exception nowrong)
                            {
                                Console.WriteLine(nowrong.Message);
                            }
                        }

                        #endregion
                    }
                    else // PatientID and RxID are OK
                    {
                        //
                        // This section is for Approval Task, ChangeRx (or Renewal Task?) workflow because we have an RxID
                        //

                        Image imgCSMed = (Image)tempDataItem.FindControl("imgCSMed");

                        //check if it's a free-form med
                        Prescription prescription = new Prescription();
                        prescription.LoadFromExistingMed(dr["rxID"].ToString(), DurInfo.RetrievePrescriptionDurHandler(PageState), SessionLicenseID, SessionUserID, base.DBID);

                        SelectedRx = new Rx(prescription);

                        SelectedRxList = new List<Rx>();
                        SelectedRxList.Add(SelectedRx);

                        string pharmacyID = prescription.DS.Tables["RxHeader"].Rows[0]["PharmacyID"].ToString().Trim();
                        #region set some vars
                        bool hasPharmacy = false;
                        if (!(string.IsNullOrEmpty(pharmacyID) || pharmacyID.Equals(Guid.Empty.ToString())))
                        {
                            hasPharmacy = true;
                        }

                        string contolledSubstanceCode = prescription.ControlledSubstanceCodeReconciled;

                        if (!(string.IsNullOrEmpty(contolledSubstanceCode) || contolledSubstanceCode.Equals("U")))
                        {
                            isCSmed = true;
                            _hasCSMeds = true;
                            imgCSMed.Visible = true;
                            controlSubstanceCode = int.Parse(contolledSubstanceCode);
                        }

                        bool isFreeFormMedControlSubstance = false;
                        string freeFormMedCSCode = string.Empty;
                        if (!string.IsNullOrWhiteSpace(contolledSubstanceCode) && string.IsNullOrWhiteSpace(prescription.DDI))
                        {
                            isFreeFormMedControlSubstance = true;
                            freeFormMedCSCode = contolledSubstanceCode;
                        }

                        bool canSendToPharmacy = false;
                        if ((!isCSmed || !_schedPrintOnly.Equals("1")))
                        {
                            canSendToPharmacy = true;
                        }
                        else
                        {
                            canSendToPharmacy = false;
                        }

                        if (isCSmed && taskType == (int)Constants.PrescriptionTaskType.APPROVAL_REQUEST && hasPharmacy)
                        {
                            canSendToPharmacy = true;
                        }

                        //display brand med detail in bold.
                        if (!(string.IsNullOrEmpty(prescription.DDI)))
                        {
                            isGeneric = Allscripts.Impact.Medication.IsGenericMed(prescription.DDI, base.DBID);
                        }
                        #endregion
                        if (taskType != Constants.PrescriptionTaskType.REFREQ)
                        {
                            medicationRquested.Text = getApprovalPrescription(dr, isGeneric, pharmacyID, canSendToPharmacy, hasPharmacy);
                        }

                        //Commented by dhiraj on 19/02/07
                        // need to check DUR every times because sometime from nurse workflow Rx has been created and before approving such rx, Physician may create another Rx.
                        //Also required to check DUR for refill task.

                        //we have an RxID here   
                        bool hasDUR = DUR.HasWarnings(DURWarnings);

                        if (!(PageState.Cast(Constants.SessionVariables.DeaSchedulesAllowed, new List<string>()).Contains(contolledSubstanceCode.ToString())))
                        {
                            isValidDEASchedule = false;
                        }

                        if (hasDUR || (string.IsNullOrEmpty(prescription.DDI) && DURMedispanUtils.IsAnyDurSettingOn(DURSettings)))
                        {
                            if (rdbYes != null)
                            {
                                rdbYes.Enabled = false;
                                chkRegistryChecked.Visible = false;

                                if (!isCSmed || (isCSmed && isValidDEASchedule))
                                {
                                    LinkButton lb = createActionLinkButton(Constants.ActionButtonText.RESOLVE_DUR, true, "dur", dr, taskType, null);
                                    tempDataItem.Cells[8].Controls.Add(lb);
                                }
                            }
                        }
                        else if (isCSmed && taskType == (int)Constants.PrescriptionTaskType.APPROVAL_REQUEST && isValidDEASchedule)
                        {
                            #region Approval request for CS med with valid DEA
                            bool isPharmacyEPCSEnabled = false;
                            bool isShowPrintOption = false;
                            DataSet dsPharmacy = null;

                            if (hasPharmacy)
                            {
                                dsPharmacy = Allscripts.Impact.Pharmacy.LoadPharmacy(pharmacyID, DBID);
                                if (dsPharmacy != null && dsPharmacy.Tables != null && dsPharmacy.Tables[0].Rows.Count > 0)
                                {
                                    isPharmacyEPCSEnabled = Convert.ToBoolean(dsPharmacy.Tables[0].Rows[0]["EpcsEnabled"]);
                                }
                            }

                            if (Session["SPI"] != null && base.CanTryEPCS)
                            {
                                if (!isPharmacyEPCSEnabled)
                                {
                                    if (rdbYes != null)
                                    {
                                        rdbYes.Enabled = false;
                                        chkRegistryChecked.Visible = false;
                                        if ((((List<string>)(Session["DEASCHEDULESALLOWED"])).Contains(controlSubstanceCode.ToString())))
                                        {
                                            LinkButton lb = createActionLinkButton(Constants.ActionButtonText.SELECT_PHARMACY, true, "pharm", dr, taskType, null);
                                            tempDataItem.Cells[8].Controls.Add(lb);
                                        }
                                        ///Set Rx and other details into session so that it can be used on pharmacy.aspx page.
                                        ArrayList rxlist = new ArrayList();
                                        Rx rx = new Rx(prescription);
                                        rxlist.Add(rx);

                                        Session["RxList"] = rxlist;
                                    }
                                }
                                else
                                {
                                    string sControlledSubstanceCode = string.Empty;

                                    if (!string.IsNullOrWhiteSpace(prescription.DDI))
                                    {
                                        DataSet dsCSMed = Allscripts.Impact.Medication.Load(prescription.DDI, Guid.Empty.ToString(), base.DBID);

                                        if (dsCSMed != null && dsCSMed.Tables != null && dsCSMed.Tables.Count > 0 && dsCSMed.Tables[0].Rows != null && dsCSMed.Tables[0].Rows.Count > 0)
                                        {
                                            sControlledSubstanceCode = dsCSMed.Tables[0].Rows[0]["ControlledSubstanceCode"].ToString().Trim();
                                        }
                                    }
                                    else //This is a free form prescription.
                                    {
                                        sControlledSubstanceCode = freeFormMedCSCode;
                                    }

                                    string stateCSCodeForSite = Prescription.GetStateControlledSubstanceCode(prescription.DDI, Session["PRACTICESTATE"].ToString(), null, base.DBID);
                                    string stateCSCodeForPharmacy = Allscripts.Impact.Prescription.GetStateControlledSubstanceCode(prescription.DDI, null,
                                                                                        dsPharmacy.Tables[0].Rows[0]["State"].ToString(), base.DBID);

                                    int scheduleUsed = 0;
                                    int.TryParse(Prescription.ReconcileControlledSubstanceCodes(sControlledSubstanceCode, stateCSCodeForPharmacy, stateCSCodeForSite), out scheduleUsed);
                                    controlSubstanceCode = scheduleUsed;

                                    if (Prescription.IsCSMedEPCSEligible(sControlledSubstanceCode, stateCSCodeForPharmacy, stateCSCodeForSite) || isFreeFormMedControlSubstance)
                                    #region EPCS
                                    {
                                        // get EPCS authorized schedules for pharmacy
                                        List<string> authorizedSchedules = new List<string>();
                                        DataTable dtSchedules = Allscripts.Impact.Pharmacy.GetEPCSAuthorizedSchedulesForPharmacy(pharmacyID, DBID);

                                        foreach (DataRow drSchedule in dtSchedules.Rows)
                                        {
                                            authorizedSchedules.Add(drSchedule[0].ToString());
                                        }

                                        Session["PharmacyEPCSAuthorizedSchdules"] = authorizedSchedules;

                                        if (Session["PharmacyEPCSAuthorizedSchdules"] != null && ((List<string>)Session["PharmacyEPCSAuthorizedSchdules"]).Contains(controlSubstanceCode.ToString()))
                                        {
                                            if (base.SiteEPCSAuthorizedSchedules.Contains(controlSubstanceCode.ToString()))
                                            {


                                                Rx rx = new Rx(prescription);

                                                bool MOBPharmacy = false;
                                                MOBPharmacy = Convert.ToBoolean(dsPharmacy.Tables[0].Rows[0]["MOBFlag"]);
                                                if (MOBPharmacy)
                                                {
                                                    rx.Destination = Patient.MOB;
                                                }
                                                else
                                                {
                                                    rx.Destination = Patient.PHARM;
                                                }

                                                if (!csRxList.Contains(rx.RxID))
                                                    csRxList.Add(rx.RxID);

                                                //ShouldShouldEpcsSignAndSendScreen EPCS Overlay and sign process.
                                                ucEPCSDigitalSigning.IsScriptForNewRx = false;
                                                ucEPCSDigitalSigning.PharmacyTask = MasterPage.RxTask;
                                                ucEPCSDigitalSigning.IsApprovalRequestWorkflow = true;
                                                //ucEPCSDigitalSigning.EPCSMEDList = rxList;
                                                Session["IsApprovalInitialized"] = true;
                                            }
                                            else
                                            {
                                                //Still have to discuss with anu.
                                                isShowPrintOption = true;
                                            }
                                        }
                                        else
                                        {
                                            ///Pharmacy is not epcs authorized for this controlled substance code. So allow them to select new pharmacy.
                                            if (rdbYes != null)
                                            {
                                                rdbYes.Enabled = false;
                                                chkRegistryChecked.Visible = false;
                                                if ((((List<string>)(Session["DEASCHEDULESALLOWED"])).Contains(controlSubstanceCode.ToString())))
                                                {
                                                    LinkButton lb = createActionLinkButton(Constants.ActionButtonText.SELECT_PHARMACY, true, "pharm", dr, taskType, null);
                                                    tempDataItem.Cells[8].Controls.Add(lb);
                                                }
                                                ///Set Rx and other details into session so that it can be used on pharmacy.aspx page.
                                                ArrayList rxlist = new ArrayList();
                                                Rx rx = new Rx(prescription);
                                                rxlist.Add(rx);

                                                Session["RxList"] = rxlist;
                                            }
                                        }
                                    }
                                    #endregion
                                    else
                                    {
                                        isShowPrintOption = true;
                                    }
                                }
                            }
                            else
                            {
                                isShowPrintOption = true;
                            }

                            if (isShowPrintOption)
                            {
                                if (rdbYes != null)
                                {
                                    rdbYes.Enabled = false;
                                    chkRegistryChecked.Visible = false;

                                    if ((((List<string>)(Session["DEASCHEDULESALLOWED"])).Contains(controlSubstanceCode.ToString())))
                                    {
                                        //This is a scheduled medication, must be printed by the provider. 
                                        LinkButton lb = createActionLinkButton(Constants.ActionButtonText.PRINT_CS_RX, true, "print", dr, taskType, null);

                                        tempDataItem.Cells[8].Controls.Add(lb);
                                    }
                                }
                            }
                        }
                        #endregion
                        else if (isCSmed && _schedPrintOnly.Equals("1"))
                        {
                            //Controlled substance must to printed
                            if (rdbYes != null)
                            {
                                rdbYes.Enabled = false;
                                chkRegistryChecked.Visible = false;

                                if ((((List<string>)(Session["DEASCHEDULESALLOWED"])).Contains(controlSubstanceCode.ToString())))
                                {
                                    //This is a scheduled medication, must be printed by the provider. 
                                    LinkButton lb = createActionLinkButton(Constants.ActionButtonText.PRINT_CS_RX, true, "print", dr, taskType, null);

                                    tempDataItem.Cells[8].Controls.Add(lb);
                                }
                            }
                        }
                        else if (!hasPharmacy)
                        {
                            //no pharmacy selected....make them select it!
                            if (rdbYes != null)
                            {
                                rdbYes.Enabled = false;
                                chkRegistryChecked.Visible = false;
                                LinkButton lb = createActionLinkButton(Constants.ActionButtonText.SELECT_PHARMACY, true, "pharm", dr, taskType, null);
                                tempDataItem.Cells[8].Controls.Add(lb);

                            }
                        }



                        //
                        // we should not be here if this grid data item/row is for a refill request
                        //
                        if (taskType == Constants.PrescriptionTaskType.RENEWAL_REQUEST)
                        #region RENEWAL_REQUEST
                        {
                            //this must be a REFREQ

                            try
                            {
                                if (dr["RegisterStatus"].ToString().ToUpper() == "MANUALLY RECONCILED" && taskType == Constants.PrescriptionTaskType.REFREQ)
                                {
                                    isPatientManuallyRegistered = true;
                                }

                                if (dr["RegisterStatus"].ToString().ToUpper() != "MANUALLY RECONCILED" && taskType == Constants.PrescriptionTaskType.REFREQ)
                                {
                                    isPatientAutoRegistered = true;
                                }

                                if ((((List<string>)(Session["DEASCHEDULESALLOWED"])).Contains(controlSubstanceCode.ToString())))
                                {

                                    if (isPatientManuallyRegistered || isPatientAutoRegistered)
                                    {
                                        LinkButton lblPatient = createActionLinkButton(Constants.ActionButtonText.CHANGE_PATIENT_NO_BREAK, false, "change", dr, taskType, null);
                                        tempDataItem.Cells[8].Controls.Add(lblPatient);
                                    }

                                    if (
                                            (!string.IsNullOrWhiteSpace(prescriptionModel.Refills) && !prescriptionModel.Refills.Equals("PRN") && !prescriptionModel.Refills.Equals("0"))
                                            &&
                                            (isPatientAutoRegistered || isPatientManuallyRegistered)
                                        )
                                    {
                                        //
                                        // the patient was auto registered, so allow the user to simply approve or change
                                        //
                                        linkButtonTextToShowUser = Constants.ActionButtonText.CHANGE_REQUEST;
                                        isActionLinkButtonARequiredLink = false;
                                    }
                                    else
                                    {
                                        //
                                        // not a coded request...provide a link and make them reconcile 
                                        //
                                        rdbYes.Enabled = false;
                                        chkRegistryChecked.Visible = false;
                                        linkButtonTextToShowUser = Constants.ActionButtonText.RECONCILE_RX;
                                        isActionLinkButtonARequiredLink = true;
                                    }

                                    LinkButton lb = createActionLinkButton(linkButtonTextToShowUser, isActionLinkButtonARequiredLink, "refreq", dr, taskType, dispensedRx.Description);
                                    tempDataItem.Cells[8].Controls.Add(lb);
                                }

                                ScriptMessage sm = new ScriptMessage(dr["ScriptMessageID"].ToString(), base.SessionLicenseID, base.SessionUserID, base.DBID);

                                if (sm.DBDDID != null && sm.DBDDID != string.Empty)
                                {
                                    imgCSMed = (Image)tempDataItem.FindControl("imgCSMed");

                                    isCSmed = showControlledSubstanceIndicator(sm.DBDDID, sm.PharmacyState);

                                    if (isCSmed)
                                    {
                                        _hasCSMeds = true;
                                        imgCSMed.Visible = true;

                                        string sControlledSubstanceCode = string.Empty;
                                        DataSet dsCSMed = Allscripts.Impact.Medication.Load(sm.DBDDID, Guid.Empty.ToString(), base.DBID);

                                        if (dsCSMed != null && dsCSMed.Tables != null && dsCSMed.Tables.Count > 0 && dsCSMed.Tables[0].Rows != null && dsCSMed.Tables[0].Rows.Count > 0)
                                        {
                                            sControlledSubstanceCode = dsCSMed.Tables[0].Rows[0]["ControlledSubstanceCode"].ToString().Trim();
                                        }

                                        string stateCSCodeForSite = Prescription.GetStateControlledSubstanceCode(sm.DBDDID, Session["PRACTICESTATE"].ToString(), null, base.DBID);
                                        string stateCSCodeForPharmacy = Allscripts.Impact.Prescription.GetStateControlledSubstanceCode(base.CurrentRx.DDI, null, sm.PharmacyState, base.DBID);

                                        int scheduleUsed = 0;
                                        int.TryParse(Prescription.ReconcileControlledSubstanceCodes(sControlledSubstanceCode, stateCSCodeForPharmacy, stateCSCodeForSite), out scheduleUsed);
                                        controlSubstanceCode = scheduleUsed;
                                    }

                                    if (taskType == Constants.PrescriptionTaskType.REFREQ)
                                    {
                                        medicationRquested.Text = RefillTaskDisplay.BuildDispensedMedMarkup(null, dispensedRx);
                                    }
                                    else
                                    {
                                        medicationRquested.Text = TaskDisplay.GetFormattedRxDetails(null, isGeneric, prescriptionModel);
                                    }
                                }

                                if (isReceivedPrescriptionInfoAvailable)
                                {
                                    Label dispensedLabel = (Label)tempDataItem.FindControl("lblDispensedPrescription");
                                    var txtPhysicianComments = tempDataItem.FindControl("notesText") as TextBox;

                                    if (dispensedLabel != null && rdbNo != null && rdbYes != null && ddl != null && txtPhysicianComments != null)
                                    {
                                        if (taskType == Constants.PrescriptionTaskType.REFREQ)
                                        {
                                            dispensedLabel.Text = RefillTaskDisplay.BuildOriginalPrescriptionMarkup(prescriptionModel);
                                        }
                                        else
                                        {
                                            ScriptMessage scriptMessage = new ScriptMessage(Session[Constants.SessionVariables.TaskScriptMessageId].ToString(), Session["LICENSEID"].ToString(), Session["USERID"].ToString(), base.DBID);
                                            if (scriptMessage != null)
                                            {
                                                var pharmDetails = StringHelper.GetPharmacyName(
                                                    scriptMessage.PharmacyName,
                                                    scriptMessage.PharmacyAddress1,
                                                    scriptMessage.PharmacyAddress2,
                                                    scriptMessage.PharmacyCity,
                                                    scriptMessage.PharmacyState,
                                                    scriptMessage.PharmacyZip,
                                                    scriptMessage.PharmacyPhoneNumber);
                                                var rxDetails = StringHelper.GetRxDetails(
                                                    scriptMessage.RxDrugDescription,
                                                    scriptMessage.RxSIGText,
                                                    scriptMessage.RxQuantity,
                                                    scriptMessage.RxDaysSupply,
                                                    scriptMessage.DispensedRxQuantity,
                                                    scriptMessage.RxDAW,
                                                    scriptMessage.RxDateCreated,
                                                    scriptMessage.RxDateLastFill,
                                                    scriptMessage.RxNotes);
                                            }
                                            dispensedLabel.Text = ChangeRxTask.GetDispensedLabelText(taskType, changeRxInfo, dispensedRx, 
                                                AppCode.ApproveRefillTask.BoxUpUiControls(tempDataItem), rdbYes.Enabled, 
                                                UserInfo.IsPermissionGranted(UserPermissions.EpcsCanPrescribe, PageState), SessionPracticeState, 
                                                sm.PharmacyState, new ChangeRxTask(), changeRxPrescriberAuth, prescriptionModel, DBID);
                                        }
                                    }
                                }
                            }
                            catch
                            {
                                //TODO: add exception logic
                            }

                        }
                        #endregion
                    }


                }

                AppCode.ApproveRefillTask.VerifyDispensedQuantityAndUpdateUi(dispensedRx, tempDataItem["ActionColumn"]?.Controls, rdbYes, new Allscripts.Impact.Telerik(tempDataItem), new AppCode.ApproveRefillTask());

                UiHelper.SetStyleForControls("Display", "none", ddl, notesLabel, notes, divMaxCharacters, txtPaApprovalCode, lblPaApproveCode);

                if (divMaxCharacters.Visible)
                {
                    HtmlControl charsRemaining = (HtmlControl)tempDataItem.FindControl("charsRemaining");
                    notes.Attributes.Add("onkeydown", "checkFieldLength(this,'" + charsRemaining.ClientID + "', 35);");
                    notes.Attributes.Add("onkeyup", "checkFieldLength(this,'" + charsRemaining.ClientID + "', 35);");
                    notes.Attributes.Add("onmouseover", "checkFieldLength(this,'" + charsRemaining.ClientID + "', 35);");
                    notes.Attributes.Add("onchange", "checkFieldLength(this,'" + charsRemaining.ClientID + "', 35);");
                }

                //if (controlSubstanceCode != 6)
                if (isCSmed)
                {
                    if (!PageState.Cast(Constants.SessionVariables.DeaSchedulesAllowed, new List<string>()).Contains(controlSubstanceCode.ToString()))
                    #region CS med not allowed
                    {
                        ucProfileMessage.Visible = true;
                        ucProfileMessage.Icon = Controls_Message.MessageType.ERROR;

                        // set a generic message since there could be several refill requets for the selected patient and those refill requests could be for different CS schedules
                        ucProfileMessage.MessageText = "Your profile is not set up with the necessary DEA Schedule to prescribe one or more of the refill requests.";

                        //if (controlSubstanceCode == 1)
                        //    ucProfileMessage.MessageText += " I ";
                        //else if (controlSubstanceCode == 2)
                        //    ucProfileMessage.MessageText += " II ";
                        //else if (controlSubstanceCode == 3)
                        //    ucProfileMessage.MessageText += " III ";
                        //else if (controlSubstanceCode == 4)
                        //    ucProfileMessage.MessageText += " IV ";
                        //else if (controlSubstanceCode == 5)
                        //    ucProfileMessage.MessageText += " V ";

                        //ucProfileMessage.MessageText += " medications.";
                    }
                    #endregion

                    //if (Session["IsPASupervised"] != null && Convert.ToBoolean(Session["IsPASupervised"]))
                    //{
                    //    if (!((List<string>)(Session["DEASCHEDULESALLOWED_SUPERVISOR"])).Contains(controlSubstanceCode.ToString()))
                    //    {
                    //        ucSupervisingProviderProfileMessage.Visible = true;
                    //        ucSupervisingProviderProfileMessage.Icon = Controls_Message.MessageType.ERROR;
                    //        ucSupervisingProviderProfileMessage.MessageText = "Your Supervising Provider's profile is not set up to prescribe schedule ";

                    //        if (controlSubstanceCode == 1)
                    //            ucSupervisingProviderProfileMessage.MessageText += " I ";
                    //        else if (controlSubstanceCode == 2)
                    //            ucSupervisingProviderProfileMessage.MessageText += " II ";
                    //        else if (controlSubstanceCode == 3)
                    //            ucSupervisingProviderProfileMessage.MessageText += " III ";
                    //        else if (controlSubstanceCode == 4)
                    //            ucSupervisingProviderProfileMessage.MessageText += " IV ";
                    //        else if (controlSubstanceCode == 5)
                    //            ucSupervisingProviderProfileMessage.MessageText += " V ";

                    //        ucSupervisingProviderProfileMessage.MessageText += " medications.";
                    //    }
                    //}
                }

                int qty = 0;
                int.TryParse(prescriptionModel.Quantity, out qty);
                if (dr["PatientID"] != DBNull.Value
                    && dr["PatientID"].ToGuid() != Guid.Empty
                    && changeRxInfo?.SubType?.Code == RxChangeSubTypeCode.PriorAuth
                    && qty > 0
                    && rdbYes != null)
                {
                    rdbYes.Enabled = true;
                }

                if (rdbYes.Enabled && rdbYes.Visible)
                {
                    if (rdbNo != null && rdbYes.Attributes["onclick"] == null)
                    {
                        if (taskType != Constants.PrescriptionTaskType.APPROVAL_REQUEST)
                        {
                            rdbNo.Attributes.Add("onclick", "disableProcessBtn(); setControlStateForRow('" + ddl.ClientID + "', " + rdbNo.ClientID + ", '" + notesLabel.ClientID + "', '" + notes.ClientID + "', '" + divMaxCharacters.ClientID + "', " + (rdbYes.Visible ? rdbYes.ClientID : "null") + ",'" + scriptMessageGuid + "','" + formularyStatus + "','" + levelOfPreferedness + "','" + taskType.ToString() + "','" + rxTaskID + "','null');");
                        }
                        else
                        {
                            rdbNo.Attributes.Add("onclick", "enableProcessBtn(); setControlStateForRow('" + ddl.ClientID + "', " + rdbNo.ClientID + ", '" + notesLabel.ClientID + "', '" + notes.ClientID + "', '" + divMaxCharacters.ClientID + "', " + (rdbYes.Visible ? rdbYes.ClientID : "null") + ", '" + scriptMessageGuid + "', '" + formularyStatus + "', '" + levelOfPreferedness + "', '" + taskType.ToString() + "', '" + rxTaskID + "', 'null'); ");
                        }
                    }

                    if (rdbYes != null && rdbYes.Attributes["onclick"] == null)
                    {
                        rdbYes.Attributes.Add("onclick", "enableProcessBtn(); setControlStateForRow('" + ddl.ClientID + "', " + rdbNo.ClientID + ", '" + notesLabel.ClientID + "', '" + notes.ClientID + "', '" + divMaxCharacters.ClientID + "', " + (rdbYes.Visible ? rdbYes.ClientID : "null") + ",'" + scriptMessageGuid + "','" + formularyStatus + "','" + levelOfPreferedness + "','" + taskType.ToString() + "','" + rxTaskID + "','null');");
                    }
                }
                else
                {
                    if (rdbNo != null && rdbNo.Attributes["onclick"] == null)
                    {
                        if (taskType == Constants.PrescriptionTaskType.APPROVAL_REQUEST)
                        {
                            rdbNo.Attributes.Add("onclick", "enableProcessBtn(); setControlStateForRow('" + ddl.ClientID + "', " + rdbNo.ClientID + ", null, null, null, null,'" + scriptMessageGuid + "','" + formularyStatus + "','" + levelOfPreferedness + "','" + taskType.ToString() + "','" + rxTaskID + "','null');");
                        }
                        else
                        {
                            rdbNo.Attributes.Add("onclick", "disableProcessBtn(); setControlStateForRow('" + ddl.ClientID + "', " + rdbNo.ClientID + ", null, null, null, null,'" + scriptMessageGuid + "','" + formularyStatus + "','" + levelOfPreferedness + "','" + taskType.ToString() + "','" + rxTaskID + "','null');");
                        }
                    }
                }
            }
        }

        private bool isNonPriorAuthChangeRx(ChangeRxSmInfo changeRxInfo)
        {
            if (changeRxInfo == null)
            {
                return false;
            }

            if (changeRxInfo.SubType != null && changeRxInfo.SubType.Code == RxChangeSubTypeCode.PriorAuth)
                return false;

            return true;
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
                return (DURCheckResponse)durWarnings;
            }
            set { PageState[Constants.SessionVariables.DURCheckResponse] = value; }
        }

        private DURCheckResponse ConstructAndSendDURRequest()
        {
            var request = DURMedispanUtils.ConstructDurCheckRequest(PageState.GetStringOrEmpty(Constants.SessionVariables.PatientDOB),
                                                                    PageState.GetStringOrEmpty(Constants.SessionVariables.Gender),
                                                                    SelectedRxList,
                                                                    PageState.Cast(Constants.SessionVariables.ACTIVEMEDDDILIST, new List<string>()),
                                                                    DurPatientAllergies,
                                                                    DURSettings);

            DURCheckResponse durResponse = DURMSC.PerformDURCheck(request);
            return durResponse;
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

        protected void grdApproveRefillTask_ItemCreated(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                GridDataItem tempDataItem = (GridDataItem)e.Item;

                //no RxID
                if (tempDataItem.GetDataKeyValue("RxID").ToString() == string.Empty)
                {
                    RadioButton rdb = e.Item.FindControl("rdbYes") as RadioButton;
                    if (rdb != null)
                    {
                        rdb.ValidationGroup = tempDataItem.GetDataKeyValue("RxTaskId").ToString();
                    }
                }

                //if the user is in HelpDesk, make the delete task row visible
                if (Session["USERGROUP"] != null && Session["USERGROUP"].ToString().Contains("HelpDesk"))
                {
                    HyperLink hl = tempDataItem.FindControl("lnkDelete") as HyperLink;
                    if (hl != null)
                    {
                        hl.Visible = true;
                    }
                }
            }
        }

        protected void grdApproveRefillTask_PreRender(object sender, System.EventArgs e)
        {
            foreach (GridColumn col in grdApproveRefillTask.MasterTableView.RenderColumns)
            {
                if ((bool)Session["SHOW_SEND_TO_ADM"] == false && col.UniqueName == "colSendToADM")
                {
                    col.Visible = false;
                }
            }
            bool starsAlignForAllTask = false;

            if (Session["SPI"] != null && base.CanTryEPCS)
            {
                starsAlignForAllTask = true;
            }

            lblCSLegend.Visible = _hasCSMeds && !starsAlignForAllTask;
        }
        #endregion

        #region Protected Events
        protected void CoverageDataSource_Selected(object sender, ObjectDataSourceStatusEventArgs e)
        {
            DataSet ds = (DataSet)e.ReturnValue;
            DataTable dt = ds.Tables[0];
            DataRow dr;

            int count = dt.Rows.Count;

            if (count == 0)
            {
                coverageDropDown.Visible = false;
                lblCoverageName.Visible = true;
                lblCoverageName.Text = "None Available";
                Session["SelectedCoverageID"] = null;
            }
            else if (count == 1)
            {
                dr = dt.Rows[0];
                coverageDropDown.Visible = false;
                lblCoverageName.Visible = true;
                lblCoverageName.Text = dr["DisplayField"].ToString();
                Session["SelectedCoverageID"] = dr["SelectedCoverageID"].ToString();
            }
            else if (count > 1)
            {
                coverageDropDown.Visible = true;
                lblCoverageName.Visible = false;
                dr = dt.Rows[0];
                Session["SelectedCoverageID"] = dr["SelectedCoverageID"].ToString();

                if (Session["SelectedCoverageID"] != null)
                {
                    coverageDropDown.SelectedValue = Session["SelectedCoverageID"].ToString();
                }
                else
                {
                    for (int i = 0; i < count; i++)
                    {
                        dr = dt.Rows[i];

                        if (dr["MailOrderBenefit"] != DBNull.Value)
                            if ("Y" == dr["MailOrderBenefit"].ToString())
                            {
                                coverageDropDown.SelectedIndex = i;
                                break;
                            }
                    }
                }
            }

            if (Session["SelectedCoverageID"] != null)
                getSelectedCoverageDetail(Session["SelectedCoverageID"].ToString());

            if (selectedFormularyStatus != null)
                selectedFormularyStatus.Value = string.Empty;

            ((PhysicianMasterPage)Master).hideTabs();
        }

        protected void coverageDropDown_SelectedIndexChanged(object sender, EventArgs e)
        {
            Session["SelectedCoverageID"] = coverageDropDown.SelectedValue;

            getSelectedCoverageDetail(coverageDropDown.SelectedValue);
        }

        protected void hiddenSelect_Click(object sender, EventArgs e)
        {
            Session[Constants.SessionVariables.TaskScriptMessageId] = hdnScriptMessageID.Value;

            if (!string.IsNullOrWhiteSpace(selectedFormularyStatus.Value) && !string.IsNullOrWhiteSpace(hdnDDI.Value))
            {
                //ucFormularyAlts.DDI = hdnDDI.Value;
                //ucFormularyAlts.ShowModelPopup = true;
                //ucFormularyAlts.CurrentFormularyStatus = Convert.ToInt32(selectedFormularyStatus.Value);
                //ucFormularyAlts.Update();
            }

            if (string.IsNullOrWhiteSpace(hdnDDI.Value))
            {
                //  ucFormularyNotes.Clear();
            }
            else
            {
                //ucFormularyNotes.Notes = selectedNotes.Value;
                //ucFormularyNotes.Update();
            }

            int formularyStatus = 100;
            int levelOfPreferredNess = 100;

            if (!int.TryParse(selectedFormularyStatus.Value, out formularyStatus))
                formularyStatus = 100;

            if (!int.TryParse(selectedLOP.Value, out levelOfPreferredNess))
                levelOfPreferredNess = 100;

            if (!string.IsNullOrWhiteSpace(hdnDDI.Value))
            {
                //ucCopyAndCoverage.DDI = hdnDDI.Value;
                //ucCopyAndCoverage.LevelOfPreferredness = levelOfPreferredNess;
                //ucCopyAndCoverage.FormularyStatus = formularyStatus;
                //ucCopyAndCoverage.Update();
            }
        }

        protected void SaveViewState(Object o, EventArgs e)
        {

        }

        protected void btnModify_Click(object sender, EventArgs e)
        {
            Response.Redirect(Constants.PageNames.REDIRECT_TO_ANGULAR + "?componentName=" + Constants.PageNames.SELECT_MEDICATION, true);
        }

        protected void RefillTaskObjDataSource_Selected(object sender, ObjectDataSourceStatusEventArgs e)
        {
            if (e.ReturnValue != null)
            {
                if (((DataSet)e.ReturnValue).Tables[0].Rows.Count == 0)
                {
                    resetSession();

                    if (!Page.IsPostBack && _savedMsg.Length > 0)
                    {
                        Session["REFILLMSG"] = _savedMsg;
                    }
                    string redirectUrl = Constants.PageNames.DOC_REFILL_MENU + ((Request.QueryString["Msg"] != null) ? "?Msg=" + Request.QueryString["Msg"].ToString() : "");
                    Response.Redirect(redirectUrl, true);
                }
                else
                {
                    string PatientName = string.Empty;
                    if (Request.QueryString["Patient"].ToString().IndexOf("[") > 0)
                    {
                        PatientName = Request.QueryString["Patient"].ToString().Substring(0, Request.QueryString["Patient"].ToString().IndexOf("[")).Trim();
                    }
                    else
                    {
                        PatientName = Request.QueryString["Patient"].ToString().Trim();
                    }

                    DataTable dt = ((DataSet)e.ReturnValue).Tables[0];
                    foreach (DataRow dr in dt.Rows)
                    {
                        if (dr["PatientID"].ToString() == Guid.Empty.ToString() && (!string.IsNullOrEmpty(dr["PatientID"].ToString())))
                        {
                            string nameLeft = Regex.Replace(dr["Patient"].ToString(), @"[^a-zA-Z0-9]", string.Empty);
                            string nameRight = Regex.Replace(PatientName, @"[^a-zA-Z0-9]", string.Empty);
                            if (string.Compare(nameLeft, nameRight, true) != 0)
                            {
                                dr.Delete();
                            }
                        }
                    }
                    dt.AcceptChanges();
                }
            }
        }

        protected void btnChangePatient_Click(object sender, EventArgs e)
        {
            resetSession();

            string returnUrl;
            if (Request.QueryString["From"] != null)
            {
                if (Request.QueryString["From"].ToLower().Contains(Constants.PageNames.APPROVE_REFILL_TASK.ToLower()))
                    returnUrl = Constants.PageNames.DOC_REFILL_MENU;
                else
                    returnUrl = Request.QueryString["From"];
            }
            else
            {
                returnUrl = Constants.PageNames.DOC_REFILL_MENU;
            }

            Response.Redirect(returnUrl, true);
        }

        protected void btnProcessTasks_Click(object sender, EventArgs e)
        {

            EpcsMedList = null;
            var selectedItems = grdApproveRefillTask.MasterTableView.Items;
            PageState[Constants.SessionVariables.PharmacyTasks] = BuildPharmacyTaskList(selectedItems);
            if (chkRegistryChecked.Visible)
            {
                PageState[Constants.SessionVariables.isCSRegistryChecked] = chkRegistryChecked.Checked;
            }
            PageState[Constants.SessionVariables.TaskScriptMessageId] = null;
            if (PageState[Constants.SessionVariables.PharmacyTasks] != null)
            {
                processTasks(true, (List<IRxTask>)PageState[Constants.SessionVariables.PharmacyTasks]);
            }
            RefillTaskObjDataSource.Select();
            grdApproveRefillTask.DataSourceID = "RefillTaskObjDataSource";
        }

        public List<IRxTask> BuildPharmacyTaskList(GridDataItemCollection selectedItems)
        {
            var rxTasksModelList = new List<RxTaskModel>();
            var RxTaskList = new List<IRxTask>();
            string patientName = Request.QueryString["Patient"]?.ToString().Trim();
            using (var timer = logger.StartTimer("BuildPharmacyTaskList"))
            {
                var approvalCount = new TaskCompletionCounts(PageState.GetStringOrEmpty("PATIENTNAME"));
                foreach (GridDataItem item in selectedItems)
                {
                    var rxTaskModel = RxTaskModel.PopulateFromRowData(SessionLicenseID.ToGuid(), SessionUserID.ToGuid(), ShieldSecurityToken, SessionSiteID, SessionPatientID, new Allscripts.Impact.Telerik(item), new ScriptMessage(), new Allscripts.Impact.Medication(), DBID);
                    var allergies = new List<Allergy>();

                    if (!(string.IsNullOrWhiteSpace(SessionPatientID) && Guid.Empty.ToString() != SessionPatientID))// restricting call to dur patient allergies for unreconcile patient 
                    {
                        allergies = DurPatientAllergies;
                    }
                    rxTaskModel.UpdatePatientData(
                                                        PageState.GetStringOrEmpty(Constants.SessionVariables.PatientDOB),
                                                        PageState.GetStringOrEmpty(Constants.SessionVariables.PatientFirstName),
                                                        PageState.GetStringOrEmpty(Constants.SessionVariables.PatientLastName),
                                                        patientName,
                                                        PageState.Cast(Constants.SessionVariables.ACTIVEMEDDDILIST, new List<string>()),
                                                        allergies);
                    rxTaskModel.UpdateProviderData(
                                                        PageState.GetStringOrEmpty(Constants.SessionVariables.DelegateProviderId),
                                                        PageState.GetStringOrEmpty(Constants.SessionVariables.SEX),
                                                        (Constants.UserCategory)PageState[Constants.SessionVariables.UserType],
                                                        base.DelegateProvider,
                                                        PageState.GetStringOrEmpty(Constants.SessionVariables.SPI));
                    rxTaskModel.UpdateSiteData(
                                                    PageState.GetStringOrEmpty(Constants.SessionVariables.ExternalFacilityCd),
                                                    PageState.GetStringOrEmpty(Constants.SessionVariables.ExternalGroupID),
                                                    (List<string>)PageState[Constants.SessionVariables.SiteEPCSAuthorizedSchedules],
                                                    PageState.GetStringOrEmpty(Constants.SessionVariables.PracticeState),
                                                    Convert.ToInt32(PageState[Constants.SessionVariables.STANDING]));
                    rxTaskModel.UpdateEPCSData(
                                                    base.CanTryEPCS,
                                                    Convert.ToBoolean(PageState[Constants.SessionVariables.isCSRegistryChecked]),
                                                    EpcsMedList,
                                                    csRxList,
                                                    PageState.GetStringOrEmpty(Constants.SessionVariables.PracticeState),
                                                    Convert.ToBoolean(Session["HasExpiredDEA"]),
                                                    (List<string>)(Session["DEASCHEDULESALLOWED"]));
                    rxTaskModel.UpdateURLData(
                                                    PageState.GetStringOrEmpty(Constants.SessionVariables.URLTo),
                                                    PageState.GetStringOrEmpty(Constants.SessionVariables.URLFrom));
                    rxTaskModel.UpdateSessionData(
                                                        DURSettings,
                                                        PageState.GetBooleanOrFalse(Constants.SessionVariables.IsApprovalInitialized),
                                                        (List<RxTaskModel>)PageState[Constants.SessionVariables.ApproveWorkflowRefillTasks]);

                    rxTaskModel.IpAddress = Request.UserIpAddress();

                    if ((rxTaskModel.TaskType == Constants.PrescriptionTaskType.RXCHG || rxTaskModel.TaskType == Constants.PrescriptionTaskType.RXCHG_PRIORAUTH) && rxTaskModel.ScriptMessage.RequestedRxs.Any())
                    {
                        var rdb = item.FindControl("rdbNo") as RadioButton;
                        var selectedRdbId = Request.Form[UiHelper.GetDynamicNameSuffixOfAspControl(rdb?.ClientID).Replace('_', '$') + "select"];
                        rxTaskModel = new ChangeRxTask().SetRequestedRxIndexAndActionForChangeRx(item, selectedRdbId, rxTaskModel);
                        rxTaskModel.Rx = new Rx(rxTaskModel.ScriptMessage.RequestedRxs[rxTaskModel.RequestedRxIndexSelected]);

                        //"Refills" gets set to the requested med refills, which is actually dispenses for a RXCHG
                        rxTaskModel.Rx.Refills--;

                        rxTaskModel.Rx.AddMDDFromPriorRX(rxTaskModel.RxGUID, base.DBID);
                        rxTaskModel.Rx.AddRxIdFromScriptMessage(rxTaskModel.ScriptMessage);
                        rxTaskModel.IsControlledSubstance = !string.IsNullOrWhiteSpace(rxTaskModel.Rx.ControlledSubstanceCode);
                        rxTaskModel.Rx.GPI = item.GetDataKeyValue("GPI").ToString();

                        if (PageState.GetStringOrEmpty(Constants.SessionVariables.PracticeState) == "MA")
                        {
                            rxTaskModel.Rx.Notes = AddMassachusettsOpiateMessage(rxTaskModel, item.GetDataKeyValue("GPI").ToString());
                        }
                    }
                    if (rxTaskModel.RxRequestType != RequestType.UNKNOWN)
                    {
                        rxTasksModelList.Add(rxTaskModel);
                    }
                }

                timer.Message = $"<rxTasksModelList>{rxTasksModelList.ToLogString()}</rxTasksModelList>";
            }

            foreach (RxTaskModel task in rxTasksModelList)
            {
                if (TasksFactory.GetTask(task) != null)
                {
                    RxTaskList.Add(TasksFactory.GetTask(task));
                }
            }

            return RxTaskList;
        }

        private string AddMassachusettsOpiateMessage(RxTaskModel rxTaskModel, string gpi)
        {
            StringBuilder sbComments = new StringBuilder();
            IPrescription Prescription = new Prescription();

            if (Prescription.IsValidMassOpiate(
                 PageState.GetStringOrEmpty(Constants.SessionVariables.PracticeState),
                 gpi,
                 rxTaskModel.Rx.ControlledSubstanceCode,
                 Convert.ToBoolean(Session["HasExpiredDEA"]),
                 (List<string>)(Session["DEASCHEDULESALLOWED"])))
            {
                sbComments.Append(Constants.PartialFillUponPatientRequest);
            }

            if (!string.IsNullOrEmpty(rxTaskModel.PhysicianComments))
            {
                sbComments.Append(" " + rxTaskModel.PhysicianComments);
            }

            //rxTaskModel.PhysicianComments = sbComments.ToString();

            return sbComments.ToString();
        }

        protected void btnYes_Click(object sender, EventArgs e)
        {

        }

        protected void btnContactMe_Click(object sender, EventArgs e)
        {

        }
        #endregion

        #region Private Methods

        private LinkButton createActionLinkButton(string textToShowUser, bool isRequired, string needs, DataRow dr, Constants.PrescriptionTaskType taskType, string drugDescription)
        {
            LinkButton lb = new LinkButton();

            if (taskType == Constants.PrescriptionTaskType.RXCHG && !IsValidActionForChangeRx(textToShowUser))
            {
                lb.Visible = false;
                return lb;
            }

            lb.Text = textToShowUser;

            if (isRequired)
            {
                lb.ForeColor = System.Drawing.Color.Red;
                lb.Font.Bold = true;
            }
            else
            {
                lb.ForeColor = System.Drawing.Color.Gray;
                lb.Font.Bold = false;
            }

            if (dr["rxID"] != DBNull.Value && !string.IsNullOrWhiteSpace(dr["rxID"].ToString()))
            {
                lb.Attributes.Add("rxid", dr["rxID"].ToString());
            }
            else
            {
                lb.Attributes.Add("rxid", "null");
            }

            lb.Attributes.Add("taskid", dr["RxTaskID"].ToString());
            lb.Attributes.Add("smid", dr["ScriptMessageID"].ToString());
            lb.Attributes.Add("needs", needs);
            lb.Attributes.Add("tasktype", Convert.ToInt32(taskType).ToString());

            if (!string.IsNullOrWhiteSpace(drugDescription))
            {
                lb.Attributes.Add("search", HttpUtility.UrlEncode(drugDescription));
            }

            if (dr["patientID"] != DBNull.Value)
            {
                lb.Attributes.Add("patientID", dr["patientID"].ToString());
            }

            if (dr["FormularyStatus"] != DBNull.Value)
            {
                lb.Attributes.Add("FormularyStatus", dr["FormularyStatus"].ToString());
            }

            if (dr["LevelOfPreferedness"] != DBNull.Value)
            {
                lb.Attributes.Add("LevelOfPreferedness", dr["LevelOfPreferedness"].ToString());
            }

            lb.Attributes.Add("onclick", "return SetGridBindRequirment();");

            return lb;
        }

        private bool IsValidActionForChangeRx(string textToShowUser)
        {
            var allowedActions = new List<string> { Constants.ActionButtonText.RECONCILE_PATIENT, Constants.ActionButtonText.PRINT_CS_RX, Constants.ActionButtonText.RESOLVE_DUR };

            return allowedActions.Contains(textToShowUser);
        }

        private bool showControlledSubstanceIndicator(string ddi, string pharmacyState)
        {
            bool showControlledSubstanceIndicator = false;

            string sControlledSubstanceCode = string.Empty;
            int iControlledSubstanceCode = 0;

            DataSet ds = Allscripts.Impact.Medication.Load(ddi, Guid.Empty.ToString(), base.DBID);

            if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows != null && ds.Tables[0].Rows.Count > 0)
            {
                sControlledSubstanceCode = ds.Tables[0].Rows[0]["ControlledSubstanceCode"].ToString().Trim();

                if (!string.IsNullOrEmpty(sControlledSubstanceCode) && !sControlledSubstanceCode.Equals("U", StringComparison.OrdinalIgnoreCase))
                {
                    iControlledSubstanceCode = int.Parse(sControlledSubstanceCode);
                }
            }

            string stateControlledSubstanceCode = Prescription.GetStateControlledSubstanceCode(ddi, Session["PRACTICESTATE"].ToString(), pharmacyState, base.DBID);

            showControlledSubstanceIndicator = (!string.IsNullOrEmpty(sControlledSubstanceCode) && !sControlledSubstanceCode.Equals("U", StringComparison.OrdinalIgnoreCase)) ||
                (!string.IsNullOrEmpty(stateControlledSubstanceCode) && !stateControlledSubstanceCode.Equals("U", StringComparison.OrdinalIgnoreCase));

            return showControlledSubstanceIndicator;
        }

        private void saveRefillRequestForEPCS(string newScriptMessageID, string rxID, string refReqScriptMessageID, int taskCount)
        {
            long serviceTaskID = ScriptMessage.SendRenewalScriptMessageForEPCS(newScriptMessageID, rxID, base.SessionLicenseID, base.SessionUserID, refReqScriptMessageID, string.Empty, base.DBID);
            //No need to create it here as we have created this in overlay dialog.
            //base.AuditLogPatientRxInsert(ePrescribeSvc.AuditAction.PATIENT_TASK_PROCESS, base.SessionPatientID, rxID);

            string auditLogPatientID = ucEPCSDigitalSigning.AuditLogPatientID.ContainsKey(rxID) ? ucEPCSDigitalSigning.AuditLogPatientID[rxID] : null;

            ///Check here if it is not -1 then insert it in newly created table. you also have auditLogPatientID.
            ///This will be used from service manager and added last audit log when message is sent to hub.
            if (serviceTaskID != -1 && !string.IsNullOrEmpty(auditLogPatientID))
            {
                Audit.InsertAuditLogPatientServiceTask(serviceTaskID, auditLogPatientID, base.DBID);
            }

            if (taskCount > 1)
            {
                Session["REFILLMSG"] = "Refills approved for " + Session["PATIENTNAME"].ToString().Trim() + ".";
            }
            else
            {
                Session["REFILLMSG"] = "Refill approved for " + Session["PATIENTNAME"].ToString().Trim() + ".";
            }

            Session.Remove(Constants.SessionVariables.TaskScriptMessageId); //make sure we close out the task
            Session.Remove("NOTES");
            MasterPage.UpdatePatientActiveMeds();
            if (Session["CameFrom"] != null && Session["CameFrom"].ToString() != string.Empty)
            {
                string cameFrom = Session["CameFrom"].ToString();
                Session.Remove("CameFrom");
                Response.Redirect(cameFrom, true);
            }
            else
            {
                Response.Redirect(Constants.PageNames.APPROVE_REFILL_TASK, true);
            }
        }

        private void saveRxChangeRequestForEPCS(string newScriptMessageID, string rxID, string refReqScriptMessageID, int taskCount)
        {
            long serviceTaskID = ScriptMessage.SendCHGRESScriptMessageForEPCS(newScriptMessageID, rxID, base.SessionLicenseID, base.SessionUserID, MasterPage.RxTask.ScriptMessageGUID, base.CurrentRx.Notes, base.DBID);
            PageState["REFILLMSG"] = "ChangeRx for " + base.CurrentRx.MedicationName + " approved for " + PageState.GetStringOrEmpty("PATIENTNAME").Trim() + ".";
            PageState.Remove(Constants.SessionVariables.Notes);
            string auditLogPatientID = ucEPCSDigitalSigning.AuditLogPatientID.ContainsKey(rxID) ? ucEPCSDigitalSigning.AuditLogPatientID[rxID] : null;

            ///Check here if it is not -1 then insert it in newly created table. you also have auditLogPatientID.
            ///This will be used from service manager and added last audit log when message is sent to hub.
            if (serviceTaskID != -1 && !string.IsNullOrEmpty(auditLogPatientID))
            {
                Audit.InsertAuditLogPatientServiceTask(serviceTaskID, auditLogPatientID, base.DBID);
            }
            if (taskCount > 1)
            {
                PageState["REFILLMSG"] = "Rx Changes approved for " + Session["PATIENTNAME"].ToString().Trim() + ".";
            }
            else
            {
                PageState["REFILLMSG"] = "Rx Change approved for " + Session["PATIENTNAME"].ToString().Trim() + ".";
            }

            PageState.Remove(Constants.SessionVariables.Notes);
            MasterPage.UpdatePatientActiveMeds();
            if (PageState["CameFrom"] != null && PageState.GetStringOrEmpty("CameFrom") != string.Empty)
            {
                string cameFrom = PageState.GetStringOrEmpty("CameFrom");
                PageState.Remove(Constants.SessionVariables.CameFrom);
                Response.Redirect(cameFrom);
            }
            else
            {
                Server.Transfer(Constants.PageNames.APPROVE_REFILL_TASK);
            }
        }
        private void resetSession()
        {
            ClearMedicationInfo(true);

            Session.Remove("NOTES");
            Session.Remove("RXID");
            Session.Remove(Constants.SessionVariables.TaskScriptMessageId);
        }

        private void setSupervisingProviderMessage()
        {
            ucSupervisingProvider.MessageText = "Tasks are being processed under the supervision of " + base.DelegateProviderName + ".";
            ucSupervisingProvider.Icon = Controls_Message.MessageType.INFORMATION;
            ucSupervisingProvider.Visible = true;
        }

        private Control FindControlInContainer(Control container, string controlId)
        {
            if (container.FindControl(controlId) != null)
                return container.FindControl(controlId);
            foreach (Control c in container.Controls)
            {
                Control controlFound = FindControlInContainer(c, controlId);
                if (controlFound != null)
                    return controlFound;
            }
            return null;
        }

        private void setBannerInfo(string patientID)
        {
            ((PhysicianMasterPage)Master).SetPatientInfo(patientID);
        }

        public void SetSessionInfo(RxTaskModel task)
        {
            if (task != null)
            {
                PageState[Constants.SessionVariables.RefillMsg] = task.TaskResponse.RefillMessage;
                PageState[Constants.SessionVariables.RxTask] = task;
                PageState[Constants.SessionVariables.DURCheckResponse] = task.DURWarnings;
                PageState[Constants.SessionVariables.IsApprovalInitialized] = task.IsApprovalInitialized;
                PageState[Constants.SessionVariables.HTTaskRxID] = task.TaskResponse.HTTaskRxID;
                PageState[Constants.SessionVariables.REMOVETASKATPRINT] = task.TaskResponse.RemoveTaskAtPrint;
                PageState[Constants.SessionVariables.ApproveWorkflowRefillTasks] = AddApproveRefillTask(task.TaskResponse.ApproveWorkflowRefillTasks);
                if ((task.ScriptMessage != null) && (task.ScriptMessage.PharmacyName != null))
                {
                    PageState[Constants.SessionVariables.PatientId] = task.ScriptMessage.DBPatientID;
                    PageState[Constants.SessionVariables.TaskScriptMessageId] = task.ScriptMessage.DBScriptMessageID;
                    PageState[Constants.SessionVariables.DestinationPharmacy] = task.ScriptMessage.PharmacyName;
                }

                if ((EpcsMedList != null) && (EpcsMedList.Count > 0))
                    PageState[Constants.SessionVariables.IsApprovalInitialized] = true;
            }
        }

        private List<RxTaskModel> AddApproveRefillTask(List<RxTaskModel> taskResponseApproveWorkflowRefillTasks)
        {
            var sessionApproveTasks = PageState.Cast(Constants.SessionVariables.ApproveWorkflowRefillTasks, new List<RxTaskModel>());
            if (taskResponseApproveWorkflowRefillTasks != null && taskResponseApproveWorkflowRefillTasks.Any())
            {
                sessionApproveTasks.AddRange(taskResponseApproveWorkflowRefillTasks);
            }
            return sessionApproveTasks;
        }

        public bool ShallExitAfterProcessingTaskResponse(RxTaskModel task, ref TaskCompletionCounts approvalCount)
        {
            bool shallProcessTaskResponseExit = false;
            SetSessionInfo(task);
            if (task.TaskResponse.IsRedirectNeeded)
            {
                Response.Redirect(task.TaskResponse.RedirectUrl, true);
            }
            switch (task.TaskResponse.RefillMessage)
            {
                case Constants.TaskingMessages.DENIAL_REASON_ERROR:
                    {
                        shallProcessTaskResponseExit = true;
                        return shallProcessTaskResponseExit;
                    }
            }
            switch (task.TaskResponse.ProcessedAction)
            {
                case ProcessedActionType.UNKNOWN:
                    {
                        break;
                    }
                case ProcessedActionType.APPROVED:
                    {
                        approvalCount.Approved++;
                        break;
                    }
                case ProcessedActionType.DENIED:
                    {
                        approvalCount.Denied++;
                        break;
                    }
            }
            switch (task.TaskResponse.ExpectedDUR)
            {
                case DURWorkflowExpected.NO:
                    {
                        break;
                    }
                case DURWorkflowExpected.YES:
                    {
                        if (ProccessDur(task))
                        {
                            shallProcessTaskResponseExit = true;
                            return shallProcessTaskResponseExit;
                        }
                        break;
                    }
                case DURWorkflowExpected.UNKNOWN:
                    {
                        break;
                    }
            }
            switch (task.TaskResponse.ExpectedEPCS)
            {
                case EPCSWorkflowExpected.NO:
                    {
                        break;
                    }
                case EPCSWorkflowExpected.YES:
                    {
                        EpcsMedList = AppCode.ApproveRefillTask.UpdateEpcsMedList(task, EpcsMedList);
                        if (MasterPage.RxTask != null && MasterPage.RxTask.TaskType != Constants.PrescriptionTaskType.APPROVAL_REQUEST)
                        {
                            ucEPCSDigitalSigning.IsScriptForNewRx = false;
                            ucEPCSDigitalSigning.PharmacyTask = MasterPage.RxTask;
                            ucEPCSDigitalSigning.EPCSMEDList = EpcsMedList;
                            if (!ucEPCSDigitalSigning.ShouldShowEpcsSignAndSendScreen())
                            {
                                shallProcessTaskResponseExit = true;
                                return shallProcessTaskResponseExit;
                            }
                        }
                        break;
                    }
                case EPCSWorkflowExpected.UNKNOWN:
                    {
                        break;
                    }
            }
            //  MasterPage.RefreshActiveMeds();
            return shallProcessTaskResponseExit;
        }
        private void PostTaskResponseProcessing(bool manuallySetSelectParameters, TaskCompletionCounts approvalCount, List<IRxTask> iRxTaskList)
        {
            var shouldGotoEPCSSigning = false;

            if (MasterPage.RxTask != null && MasterPage.RxTask.TaskType == Constants.PrescriptionTaskType.APPROVAL_REQUEST)
            {
                if (EpcsMedList != null && EpcsMedList.Count > 0)
                {
                    ucEPCSDigitalSigning.IsScriptForNewRx = false;
                    ucEPCSDigitalSigning.PharmacyTask = MasterPage.RxTask;
                    ucEPCSDigitalSigning.EPCSMEDList = EpcsMedList;
                    approvalCount.Approved -= EpcsMedList.Count;
                    PageState[Constants.SessionVariables.TaskCompletionCount] = approvalCount;
                    shouldGotoEPCSSigning = ucEPCSDigitalSigning.ShouldShowEpcsSignAndSendScreen();
                }
            }


            //if ((Convert.ToBoolean(Session["IsApprovalInitialized"]) ) && (EpcsMedList != null && EpcsMedList.Count > 0))
            //{
            //    ucEPCSDigitalSigning.IsScriptForNewRx = false;
            //  //  ucEPCSDigitalSigning.IsRenewalApprovalWorkflow = true;
            //  //  ucEPCSDigitalSigning.ChangeRxWorkflowObject = MasterPage.ChangeRxWorkflow;
            //    ucEPCSDigitalSigning.EPCSMEDList = EpcsMedList;
            //    approvalCount.Approved -= EpcsMedList.Count;
            //    PageState[Constants.SessionVariables.TaskCompletionCount] = approvalCount;
            //    ucEPCSDigitalSigning.ShouldShouldEpcsSignAndSendScreen();
            //}




            if (iRxTaskList != null && iRxTaskList.Count == 0)
            {
                PageState.Remove(Constants.SessionVariables.PharmacyTasks);
                PageState.Remove("IsApprovalInitialized");
            }

            if (approvalCount != null && (approvalCount.Approved > 0 || approvalCount.Denied > 0))
                PageState[Constants.SessionVariables.RefillMsg] = approvalCount.BuildSuccessMsg();
            Session["To"] = Constants.PageNames.APPROVE_REFILL_TASK;

            if (
                    //MasterPage.RxTask!=null &&
                    //MasterPage.RxTask.TaskType==Constants.PrescriptionTaskType.REFREQ && 
                    !string.IsNullOrEmpty(PageState.GetStringOrEmpty(Constants.SessionVariables.RefillPatientId)) &&
                    manuallySetSelectParameters)
            {
                RefillTaskObjDataSource.SelectParameters["patientID"] = new Parameter("patientID", TypeCode.String, PageState.GetStringOrEmpty(Constants.SessionVariables.RefillPatientId));
                if (!string.IsNullOrEmpty(PageState.GetStringOrEmpty(Constants.SessionVariables.RefillPhyId)))
                    RefillTaskObjDataSource.SelectParameters["physicianId"] = new Parameter("physicianId", TypeCode.String, PageState.GetStringOrEmpty(Constants.SessionVariables.RefillPhyId));
                RefillTaskObjDataSource.Select();
                grdApproveRefillTask.DataSourceID = "RefillTaskObjDataSource";

                if (!shouldGotoEPCSSigning)
                {
                    redirectToNextPage(false);
                }
            }

            MasterPage.RefreshActiveMeds();

        }

        private void processTasks(bool manuallySetSelectParameters, List<IRxTask> rxTaskList)
        {
            PageState.Remove(Constants.SessionVariables.ApproveWorkflowRefillTasks);
            var approvalCount = new TaskCompletionCounts(PageState.GetStringOrEmpty(Constants.SessionVariables.PatientName));
            while (rxTaskList.Count > 0)
            {
                IRxTask iRxTask = rxTaskList[0];
                rxTaskList.RemoveAt(0);
                PageState[Constants.SessionVariables.PharmacyTasks] = rxTaskList;
                if (ShallExitAfterProcessingTaskResponse(iRxTask.ProcessTaskingWorkflow(), ref approvalCount))
                    return;
            }
            PostTaskResponseProcessing(manuallySetSelectParameters, approvalCount, rxTaskList);
        }


        private static bool ConvertValuesFromScriptMessage(ScriptMessage sm, out int daysSupply, out decimal quantity,
        out int refills)
        {
            var daw = sm.RxDAW == "Y" || sm.RxDAW == "1";

            //Convert data type ahead of time. 
            daysSupply = 0;
            quantity = 0;
            refills = 0;

            //Convert the days of supply
            try
            {
                daysSupply = Convert.ToInt32(sm.RxDaysSupply);
            }
            catch (Exception)
            {
                daysSupply = 0;
            }

            //Convert the quantity
            try
            {
                quantity = Convert.ToDecimal(sm.RxQuantity);
            }
            catch (Exception)
            {
                quantity = 0;
            }

            //Convert the refills
            try
            {
                refills = Convert.ToInt32(sm.RxRefills);
            }
            catch (Exception)
            {
                refills = 0;
            }
            return daw;
        }

        private bool ProccessDur(RxTaskModel currentTask)
        {
            switch (currentTask.TaskType)
            {
                case Constants.PrescriptionTaskType.REFREQ:
                    PageState[Constants.SessionVariables.TaskScriptMessageId] = currentTask.ScriptMessageGUID;
                    break;
                case Constants.PrescriptionTaskType.RXCHG:
                    break;
                    //MasterPage.RxTaskModel has already been set prior
            }

            PageState["TaskID"] = currentTask.RxTaskId;
            PageState["Tasktype"] = currentTask.TaskType;
            PageState["DUR_GO_PREVIOUS"] = Constants.PageNames.APPROVE_REFILL_TASK;
            PageState["CameFrom"] = Constants.PageNames.APPROVE_REFILL_TASK;

            currentTask.Rx.Notes = AddMassachusettsOpiateMessage(currentTask, currentTask.Rx.GPI);

            var rxList = new ArrayList();
            rxList.Add(currentTask.Rx);
            PageState["RxList"] = rxList;

            DataSet ds = currentTask.DURDataSet;
            bool hasOtherDUR = false;
            bool hasDUP = false;

            hasDUP = currentTask.DURWarnings.DuplicateTherapy.Results.HasItems();

            hasOtherDUR = DURMedispanUtils.HasMoreThanDupDUR(currentTask.DURWarnings);

            if (hasDUP)
            {
                Session["DUPDUR"] = true;
            }

            var ddiList = string.Join(",", from script in rxList.Cast<Rx>().ToList() select script.DDI);
            DataSet dsActiveScripts = Prescription.CheckPatientRxHistoryForMultiSelect(ddiList, base.SessionPatientID, base.DBID);

            if (dsActiveScripts.Tables[0].Rows.Count > 0)
            {
                if (currentTask.IsControlledSubstance)
                {
                    ucMedicationHistoryCompletion.IsCSMed = true;
                    ucMedicationHistoryCompletion.HasOtherDUR = hasOtherDUR;
                }

                ucMedicationHistoryCompletion.ActiveScripts = dsActiveScripts;
                ucMedicationHistoryCompletion.SearchValue = string.Empty;
                ucMedicationHistoryCompletion.LoadHistory();
                return true;
            }

            Response.Redirect(Constants.PageNames.RX_DUR_REVIEW_MULTI_SELECT, true);

            return false;
        }

        public EPCSParameters EPCSWorkflowParameters()
        {
            EPCSParameters epcsDataFromSigPage = new EPCSParameters();
            epcsDataFromSigPage.DBID = base.DBID;
            var sm = new ScriptMessage();
            var rx = new Rx();
            if (MasterPage.RxTask != null && MasterPage.RxTask.TaskType == Constants.PrescriptionTaskType.RXCHG)
            {
                sm = MasterPage.RxTask.ScriptMessage as ScriptMessage;
                rx = MasterPage.RxTask.Rx as Rx;
            }
            else if (PageState[Constants.SessionVariables.TaskScriptMessageId] != null)
            {
                //TODO: User story for Workflows shall handle sm, rx for Refreq's like RxTaskModel
                sm = new ScriptMessage(PageState.GetStringOrEmpty(Constants.SessionVariables.TaskScriptMessageId), Session["LICENSEID"].ToString(), Session["USERID"].ToString(), base.DBID); ;
                rx = new Rx(sm, new Allscripts.Impact.Medication());
            }
            epcsDataFromSigPage.FedCSCode = rx.ControlledSubstanceCode;
            epcsDataFromSigPage.DsPharmacy = Allscripts.Impact.Pharmacy.LoadPharmacy(sm.DBPharmacyID, base.DBID);
            epcsDataFromSigPage.StateCSCodeForPharmacy = Allscripts.Impact.Prescription.GetStateControlledSubstanceCode(rx.DDI, null, epcsDataFromSigPage.DsPharmacy.Tables[0].Rows[0]["State"].ToString(), base.DBID);
            epcsDataFromSigPage.ReconciledControlledSubstanceCode = Prescription.ReconcileControlledSubstanceCodes(epcsDataFromSigPage.FedCSCode, epcsDataFromSigPage.StateCSCodeForPharmacy);
            epcsDataFromSigPage.StateCSCodeForPractice = Allscripts.Impact.Prescription.GetStateControlledSubstanceCode(rx.DDI, PageState.GetStringOrEmpty("PracticeState"), base.DBID);
            epcsDataFromSigPage.IsFreeFormMedControlSubstance = base.CurrentRx.IsFreeFormMedControlSubstance;
            epcsDataFromSigPage.CanTryEPCS = base.CanTryEPCS;
            epcsDataFromSigPage.SiteEPCSAuthorizedSchedulesList = base.SiteEPCSAuthorizedSchedules;
            return epcsDataFromSigPage;
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
                    if (attemptingToSendToPharmacy && (Session["SPI"] == null || Session["SPI"].ToString() == string.Empty))
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
                    string extFacilityCode = Session["ExtFacilityCd"] != null ? Session["ExtFacilityCd"].ToString() : null;
                    Prescription.UpdateRxDetailStatus(base.SessionLicenseID, base.SessionUserID, rxID, "AUTHORIZEBY", extFacilityCode, base.DBID);
                }
            }
        }

        private void getSelectedCoverageDetail(string coverageID)
        {
            DataSet ds = PatientCoverage.GetCoverage(Int64.Parse(coverageID), base.DBID);

            DataTable dtCoverage = ds.Tables["PatientCoverage"];
            DataTable dtPlan = ds.Tables["PatientPlan"];
            DataRow drCoverage = null;
            DataRow drPlan = null;

            if (dtPlan != null && dtPlan.Rows.Count > 0)
                drPlan = dtPlan.Rows[0];

            if (drPlan != null)
            {
                Session["PlanID"] = drPlan["PlanID"].ToString();
                Session["FormularyID"] = drPlan["FormularyID"].ToString();
                Session["FormularyActive"] = drPlan["Active"].ToString();
                Session["OTCCoverage"] = drPlan["OTCCoverage"].ToString();
                Session["GenericDrugPolicy"] = drPlan["GenericDrugPolicy"].ToString();
                Session["UnListedDrugPolicy"] = drPlan["UnlistedDrugPolicy"].ToString();
            }

            if (dtCoverage != null && dtCoverage.Rows.Count > 0)
                drCoverage = dtCoverage.Rows[0];

            if (drCoverage != null)
            {
                Session["CoverageID"] = drCoverage["coverageID"] != DBNull.Value ? drCoverage["coverageID"].ToString() : string.Empty;
                Session["CopayID"] = drCoverage["copayID"] != DBNull.Value ? drCoverage["copayID"].ToString() : string.Empty;
                Session["AltPlanID"] = drCoverage["AltPlanID"] != DBNull.Value ? drCoverage["AltPlanID"].ToString() : string.Empty;
                Session["InfoSourcePayerID"] = drCoverage["InfoSourcePayerID"] != DBNull.Value ? drCoverage["InfoSourcePayerID"].ToString() : string.Empty;
            }

            setCoverageIDAvailbility();
            setRightPanelVisibility();
        }

        private void setCoverageIDAvailbility()
        {
            _checkAltCopay = base.CheckFormAltsAndCopay;
            checkCopay.Value = _checkAltCopay.ToString();
        }

        private void setRightPanelVisibility()
        {
            //ucFormularyNotes.HasFormularyID = (Session["FORMULARYID"] != null && Session["FORMULARYID"].ToString().Trim() != string.Empty);
            //ucFormularyAlts.HasFormularyID = (Session["FORMULARYID"] != null && Session["FORMULARYID"].ToString().Trim() != string.Empty);
            //ucCopyAndCoverage.HasCopayAndCoverageIDs = (Session["COVERAGEID"] != null && Session["COVERAGEID"].ToString().Trim() != string.Empty) || (Session["COPAYID"] != null && Session["COPAYID"].ToString().Trim() != string.Empty);
        }

        private string getApprovalPrescription(DataRow dr, bool isGeneric, string pharmacyID, bool canSendToPharmacy, bool hasPharmacy)
        {
            StringBuilder prescription = new StringBuilder();

            if (dr != null)
            {
                if (dr["Patient"] != DBNull.Value)
                    prescription.Append("<b>Patient:</b> ").Append(dr["Patient"].ToString().Trim());

                if (dr["MedicationName"] != DBNull.Value)
                {
                    if (isGeneric)
                    {
                        prescription.Append("<BR>").Append("<b>Rx Detail:</b> ").Append(dr["MedicationName"].ToString().Trim());

                        if (dr["Strength"] != DBNull.Value)
                            prescription.Append(" ").Append(dr["Strength"].ToString().Trim());

                        if (dr["StrengthUom"] != DBNull.Value)
                            prescription.Append(" ").Append(dr["StrengthUom"].ToString().Trim());

                        if (dr["FormDescription"] != DBNull.Value)
                            prescription.Append(" ").Append(dr["FormDescription"].ToString().Trim());
                    }
                    else
                    {
                        //mark the brand in bold
                        prescription.Append("<BR>").Append("<b>Rx Detail: ").Append(dr["MedicationName"].ToString().Trim());

                        if (dr["Strength"] != DBNull.Value)
                            prescription.Append(" ").Append(dr["Strength"].ToString().Trim());

                        if (dr["StrengthUom"] != DBNull.Value)
                            prescription.Append(" ").Append(dr["StrengthUom"].ToString().Trim());

                        if (dr["FormDescription"] != DBNull.Value)
                            prescription.Append(" ").Append(dr["FormDescription"].ToString().Trim());

                        prescription.Append("</b>");
                    }
                }

                prescription.Append("<BR>").Append("<b>SIG: </b>");

                if (dr["sigtext"] != DBNull.Value)
                    prescription.Append(dr["sigtext"].ToString().Trim());

                if (dr["quantity"] != DBNull.Value)
                {
                    prescription.Append(", ").Append("<b>Quantity: </b>");
                    prescription.Append(Convert.ToDouble(dr["quantity"].ToString().Trim()).ToString());
                }

                if (dr["PackageUOM"] != DBNull.Value)
                    prescription.Append(" ").Append(dr["PackageUOM"].ToString().Trim());

                if (dr["Dayssupply"] != DBNull.Value)
                    prescription.Append(", ").Append("<b>Days:</b>").Append(" ").Append(dr["Dayssupply"].ToString().Trim());

                if (dr["refillquantity"] != DBNull.Value)
                {
                    prescription.Append(", ").Append("<b>Refill:</b> ").Append(dr["refillquantity"].ToString().Trim());
                }

                if (dr["DAW"] != DBNull.Value)
                {
                    if (dr["DAW"].ToString() == "Y")
                    {
                        prescription.Append(", ").Append("<b>DAW</b> ");
                    }
                }

                if (dr["PharmacyNotes"] != DBNull.Value && dr["PharmacyNotes"].ToString() != string.Empty)
                {
                    prescription.Append("<BR>").Append("<b>Notes:</b> ");
                    prescription.Append(dr["PharmacyNotes"].ToString().Trim());
                }

                if (canSendToPharmacy && hasPharmacy)
                {
                    DataSet pharmacyDS = Allscripts.Impact.Pharmacy.LoadPharmacy(pharmacyID, base.DBID);

                    if (pharmacyDS.Tables[0].Rows.Count > 0)
                    {
                        prescription.Append("<BR>").Append("<b>Pharmacy:</b> ");
                        prescription.Append(pharmacyDS.Tables[0].Rows[0]["Name"].ToString().Trim());
                        prescription.Append(", ").Append(pharmacyDS.Tables[0].Rows[0]["Address1"].ToString().Trim());
                        prescription.Append(" ").Append(pharmacyDS.Tables[0].Rows[0]["Address2"].ToString().Trim());
                        prescription.Append(", ").Append(pharmacyDS.Tables[0].Rows[0]["City"].ToString().Trim());
                        prescription.Append(", ").Append(pharmacyDS.Tables[0].Rows[0]["State"].ToString().Trim());
                    }
                }

                if (dr["CreatedBy"] != Convert.DBNull && dr["CreatedBy"].ToString() != string.Empty)
                {
                    prescription.Append("<BR>").Append("<b>Created by:</b> ");
                    prescription.Append(dr["CreatedBy"].ToString().Trim());
                }
            }

            return prescription.ToString();
        }

        private void displayFormularyStatus(GridDataItem tempDataItem)
        {
            Image img = tempDataItem.FindControl("Image1") as Image;
            Label lblLevelOfPreferedness = tempDataItem.FindControl("lblLevelOfPreferedness") as Label;

            if (img != null)
            {
                img.Style["cursor"] = "pointer";

                int ahsFormularyStatus = Convert.ToInt32("0" + tempDataItem.GetDataKeyValue("FormularyStatus").ToString());

                ahsFormularyStatus = _formularyActive ? ahsFormularyStatus : 0;

                bool isOTC = false;
                if (tempDataItem.GetDataKeyValue("IsOTC") != null)
                    isOTC = tempDataItem.GetDataKeyValue("IsOTC").ToString() == "Y";

                string imagePath = string.Empty;
                string toolTip = string.Empty;
                MedicationSearchDisplay.GetFormularyImagePathWithTooltip(ahsFormularyStatus, isOTC, out imagePath, out toolTip);

                img.ImageUrl = imagePath;
                img.ToolTip = toolTip;

                if (_checkAltCopay)
                    img.ToolTip = img.ToolTip + ". Click to update right panel formulary information.";
            }
        }

        void ucMedicationHistoryCompletion_OnMedHistoryComplete(MedHistoryCompletionEventArgs eventArgs)
        {
            if (eventArgs.CompletionStatus == MedHistoryCompletionEventArgs.MedHistoryCompletionStatus.Continue)
            {
                handlePossibleDur();
            }
            else if (eventArgs.CompletionStatus == MedHistoryCompletionEventArgs.MedHistoryCompletionStatus.CompleteAndContinue)
            {
                if (PageState.GetBooleanOrFalse(Constants.SessionVariables.DupDur) && eventArgs.DidCompleteAll)
                {
                    PageState.Remove(Constants.SessionVariables.DupDur);
                    if (ucMedicationHistoryCompletion.IsCSMed && !ucMedicationHistoryCompletion.HasOtherDUR)
                    {
                        processCsMed();
                    }
                    else
                    {
                        redirectToNextPage(refreshTaskDUR());
                    }
                }
                else
                {
                    handlePossibleDur();
                }
            }
        }
        private bool refreshTaskDUR()
        {
            var iRxTask = TasksFactory.GetTask(MasterPage.RxTask);
            MasterPage.RxTask.ActiveMedList = PageState.Cast(Constants.SessionVariables.ACTIVEMEDDDILIST, new List<string>());
            iRxTask.LoadDUR();
            SetSessionInfo(MasterPage.RxTask);
            return DURMedispanUtils.DoesContainDUR(MasterPage.RxTask.DURWarnings);
        }
        private void handlePossibleDur()
        {
            if (DURSettings.IsDURCheckSet && DURMedispanUtils.HasMoreThanDupDUR(DURWarnings))
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
            bool canStartEpcs = new EPCSWorkflowUtils().IsEPCSWorkflowExpected(EPCSWorkflowParameters());
            if (EpcsMedList == null)
            {
                EpcsMedList = new List<Rx>();
            }
            EPCSWorkflowUtils epcsWorkflow = new EPCSWorkflowUtils();
            Rx rx = (MasterPage.RxTask != null && MasterPage.RxTask.TaskType == Constants.PrescriptionTaskType.RXCHG) ? MasterPage.RxTask.Rx as Rx : base.CurrentRx;
            EpcsMedList = epcsWorkflow.UpdateAndReturnEpcsMedList(EpcsMedList, rx);
            if (epcsWorkflow.ShouldLaunchEPCSWorkflow(canStartEpcs, EpcsMedList))
            {
                ucEPCSDigitalSigning.IsScriptForNewRx = false;
                ucEPCSDigitalSigning.PharmacyTask = MasterPage.RxTask;
                ucEPCSDigitalSigning.EPCSMEDList = EpcsMedList;
                ucEPCSDigitalSigning.ShouldShowEpcsSignAndSendScreen();
            }
            else
            {
                // here we are assuming we need to finish processing cs meds and we the stars do not align
                ucCSMedRefillRequestNotAllowed.ShowPopUp();
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
                Response.Redirect($"{Constants.PageNames.RX_PROCESSOR}?From={cameFrom}&To={cameFrom}", true);
            }
        }

        #endregion

        protected void coverageDropDown_DataBound(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["SelectedCoverageID"] != null &&
                    !string.IsNullOrEmpty((Session["SelectedCoverageID"].ToString())))
                {
                    coverageDropDown.SelectedIndex =
                        coverageDropDown.Items.IndexOf(
                            coverageDropDown.Items.FindByValue(Session["SelectedCoverageID"].ToString()));
                    coverageDropDown_SelectedIndexChanged(sender, e);
                }
            }
        }
    }
}
