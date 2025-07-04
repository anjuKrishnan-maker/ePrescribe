/******************************************************************************
**Change History
*******************************************************************************
**Date:      Author:                    Description:
**-----------------------------------------------------------------------------
* 08/14/2009   Anand Kumar Krishnan   Defect#2678 - CoverageID passed to AddMedication 
 *                                    call in savePrescription method
*******************************************************************************/
using System;
using System.Xml;
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
using System.Threading;
using Allscripts.Impact;
using Allscripts.ePrescribe.ExtensionMethods;
using Allscripts.ePrescribe.Shared.Logging;
using DAL = Allscripts.ePrescribe.Data;
using Allscripts.ePrescribe.Objects;
using ScriptMessage = Allscripts.Impact.ScriptMsg.ScriptMessage;
using Rx = Allscripts.Impact.Rx;
using Patient = Allscripts.Impact.Patient;
using RxUser = Allscripts.Impact.RxUser;
using Allscripts.Impact.Interfaces;
using System.Threading.Tasks;
using Allscripts.ePrescribe.Medispan.Clinical.Model;
using Allscripts.ePrescribe.Medispan.Clinical.Model.Request;
using Allscripts.ePrescribe.Medispan.Clinical.Model.Response;
using Allscripts.ePrescribe.Medispan.Clinical.Model.Settings;
using Allscripts.Impact.ePrescribeSvc;
using eRxWeb.AppCode;
using eRxWeb.AppCode.StateUtils;
using Constants = Allscripts.ePrescribe.Common.Constants;
using eRxWeb.AppCode.PptPlusBPL;
using FormularyStatus = Allscripts.Impact.FormularyStatus;
using eRxWeb.State;

namespace eRxWeb
{
    public partial class RxProcessor : BasePage
    {
        private static readonly ILoggerEx logger = LoggerEx.GetLogger();
        public Prescription _rx;
        string patientID = string.Empty;
        string rxID = Guid.NewGuid().ToString();
        bool _formularyActive = false;

        string providerID = string.Empty;
        string pharmacyID = string.Empty;

        //DUR Check
        bool _performDosageChecks = false;
        bool _performInteractions = false;
        bool _performDuplicateTherapy = false;
        bool _performPAR = false;
        bool _performFoodCheck = false;
        bool _performAlcoholCheck = false;

        protected void Page_Load(object sender, EventArgs e)
        {
            //OK, so we've been handed some information
            //save the script

            //Presently business rule is to write script without selecting patient 19th May 2006
            //EAK added for pre-checked DUR
            if (
                    (Session["Tasktype"] != null &&
                    (
                        (Constants.PrescriptionTaskType)Session["Tasktype"] == Constants.PrescriptionTaskType.APPROVAL_REQUEST ||
                        (Constants.PrescriptionTaskType)Session["Tasktype"] == Constants.PrescriptionTaskType.RENEWAL_REQUEST
                    ) &&
                    Session["RXID"] != null &&
                    Session["taskID"] != null) ||
                    Request.QueryString["Action"] == "EditScriptPad"
                )
            {
                Prescription p = new Prescription();

                if (Request.QueryString["Action"] == "EditScriptPad")
                {
                    Session["RXID"] = Request.QueryString["RxId"];
                }

                p.LoadFromExistingMed(Session["RXID"].ToString(), DurInfo.RetrievePrescriptionDurHandler(PageState), SessionLicenseID, SessionUserID, base.DBID);

                _rx = p;

                if (PageState.Cast(Constants.SessionVariables.TaskType, Constants.PrescriptionTaskType.DEFAULT) == Constants.PrescriptionTaskType.APPROVAL_REQUEST)
                {
                    PageState[Constants.SessionVariables.RxList] = new ArrayList { new Rx(p) };
                }

                patientID = _rx.PatientID;

                LoadProviderPreference();

                base.ScriptPadMeds = null;

                durCheck();

                if (Request.QueryString["To"] != null)
                {
                    Response.Redirect(Constants.PageNames.UrlForRedirection(Request.QueryString["To"]));
                }
            }

            //go to the correct destination
            using (logger.StartTimer("savePrescriptions"))
            {
                savePrescriptions();
            }

            //Dhiraj on 21/02/07 shift update task from approverefilltask to here
            if (Session["Tasktype"] != null && (Constants.PrescriptionTaskType)Session["Tasktype"] == Constants.PrescriptionTaskType.RENEWAL_REQUEST)
            {
                if (Session["RefilltaskData"] != null)
                {
                    RxTaskModel refilltask = (RxTaskModel)Session["RefilltaskData"];
                    Constants.PrescriptionTaskStatus taskstatus = (refilltask.RxRequestType == RequestType.APPROVE ? Constants.PrescriptionTaskStatus.ONE : Constants.PrescriptionTaskStatus.PROCESSED);

                    Prescription.UpdateRxTask(refilltask.RxTaskId, refilltask.PhysicianComments, refilltask.IsPatientVisitRq, (int)taskstatus, Constants.PrescriptionStatus.NEW, base.SessionUserID, base.SessionLicenseID, base.DBID);
                    Session.Remove("RefilltaskData");
                }
            }

            if (Request.QueryString["PrintScript"] != null && Request.QueryString["PrintScript"] == "YES")
            {
                ArrayList alProcess = new ArrayList();
                alProcess.Add(rxID);
                Session["ProcessList"] = alProcess;
                Server.Transfer(Constants.PageNames.CSS_DETECT);
            }

            // Google Analytics
            PlacementResponse = TieUtility.GetAdPlacement(Request.Cookies, new TieUtility(), new TIEServiceManager(), new Allscripts.Impact.Utilities.ConfigurationManager());

            //go back to the same page to process other remaining task in case of ApproveRefillTask.aspx page
            // dhiraj 07/06/06
            if (Request.QueryString["To"] != null)
            {
                Response.Redirect(Constants.PageNames.UrlForRedirection(Request.QueryString["To"]) + "?from=" + Constants.PageNames.RX_PROCESSOR);
            }
        }

        public string GetApprovalMessageAndRouteChangeRxWorkflow(RxTaskModel changeRxWorkflow, IScriptMessage iScriptMessage, bool IsPOBUser, string SupervisorProviderID, string PatientName, string authToken)
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
        public void savePrescriptions()
        {
            Constants.PrescriptionStatus status = Constants.PrescriptionStatus.SCRIPT_PAD;
            string rxDate = DateTime.UtcNow.ToString();
            string startDate = DateTime.Today.ToShortDateString();

            //setting transmission method to NULL b/c it hasn't been transmitted yet. Only set transmission method on script pad page or as final step in processing task.
            string transmissionMethod = null;
            FormularyStatus formularyStatus = FormularyStatus.ACCEPTED;
            int originalLineNumber = Session["OriginalRxID"] != null ? 1 : 0;
            PrescriptionWorkFlow rxWorkFlow = Session["RX_WORKFLOW"] != null ? ((PrescriptionWorkFlow)Convert.ToInt32(Session["RX_WORKFLOW"])) : PrescriptionWorkFlow.STANDARD;
            string extFacilityCode = null;
            string extGroupID = null;

            var rxList = PageState.Cast("RxList", new ArrayList());
            if (rxList.Count == 0) LogError();
            logger.Debug($"savePrescriptions(): <RxList>{rxList.ToList<Rx>().ToLogString()}</RxList>");

            foreach (Rx rx in rxList)
            {
                if (string.IsNullOrEmpty(rx.RxID) || rx.RxID == Guid.Empty.ToString())
                {
                    rx.RxID = Guid.NewGuid().ToString();
                }

                Session["OriginalRxID"] = null;

                if (!string.IsNullOrEmpty(base.SessionPatientID))
                    patientID = base.SessionPatientID;
                else
                    patientID = System.Guid.Empty.ToString();

                if (Request.QueryString["PatPharm"] != null && Request.QueryString["PatPharm"] == "Y")
                    Session["PHARMACYID"] = (Session["LASTPHARMACYID"] != null ? Session["LASTPHARMACYID"].ToString() : System.Guid.Empty.ToString());

                // this snippet of code is most likely obsolete but leaving in just in case, no instances of RxProcessor page being loaded with a query string param of MailOrder
                if (Request.QueryString["MailOrder"] != null && Request.QueryString["MailOrder"] == "Y")
                {
                    string mopharmID = string.Empty;
                    if (Session["MOB_PHARMACY_ID"] != null)
                    {
                        mopharmID = Session["MOB_PHARMACY_ID"].ToString();
                    }
                    else
                    {
                        DataSet mobDS = Allscripts.Impact.Pharmacy.LoadPharmacyByNABP(Session["MOB_NABP"].ToString(), base.DBID);
                        if (mobDS.Tables[0].Rows.Count > 0)
                        {
                            mopharmID = mobDS.Tables[0].Rows[0]["PharmacyID"].ToString();
                        }
                    }

                    if (!string.IsNullOrEmpty(mopharmID))
                    {
                        Session["PHARMACYID"] = mopharmID;
                    }
                }

                pharmacyID = (Session["PHARMACYID"] != null ? Session["PHARMACYID"].ToString() : System.Guid.Empty.ToString());

                formularyStatus = (rx.FormularyStatus != null ? (FormularyStatus)Convert.ToInt32(rx.FormularyStatus) : FormularyStatus.NONE);

                providerID = base.SessionUserID;

                if (Session["FormularyActive"] != null)
                    _formularyActive = (Session["FormularyActive"].ToString() == "Y" ? true : false);

                LoadProviderPreference();

                if (Session["ExtFacilityCd"] != null)
                    extFacilityCode = Session["ExtFacilityCd"].ToString();

                if (Session["ExtGroupID"] != null)
                    extGroupID = Session["ExtGroupID"].ToString();


                if (Convert.ToBoolean(Session["IsDelegateProvider"])) //For delegate providers
                {
                    //If the TASKTYPE is to send to the script to provider for approval, 
                    //then change the script status from NEW to PENDING_APPROVAL
                    if (Session["TASKTYPE"] != null)
                    {
                        Constants.PrescriptionTaskType tasktype = (Constants.PrescriptionTaskType)Session["TASKTYPE"];
                        if (tasktype == Constants.PrescriptionTaskType.APPROVAL_REQUEST)
                        {
                            status = Constants.PrescriptionStatus.PENDING_APPROVAL;
                        }
                    }

                    //check both session variables here; we've seen instances of the PHYSICIANID being null but the DelegateProviderID being populated; they should be the same
                    if (Session["PHYSICIANID"] != null)
                    {
                        providerID = Session["PHYSICIANID"].ToString();
                    }
                    else if (Session["DelegateProviderID"] != null)
                    {
                        providerID = Session["DelegateProviderID"].ToString();
                    }
                    else
                    {
                        Server.Transfer(
                            string.Concat(
                                "SelectPatient?Msg=",
                                Server.UrlEncode("We apologize, but it appears that your supervising provider has not been selected. Please select a supervising provider from the list and continue."),
                                "&MsgType=Error"));
                    }
                }
                else
                {
                    if (Session["TASKTYPE"] != null)
                    {
                        //If the provider is sending the script to assistant, then mark the script as pending transmission. 
                        Constants.PrescriptionTaskType taskType = (Constants.PrescriptionTaskType)Session["TASKTYPE"];
                        if (taskType == Constants.PrescriptionTaskType.SEND_TO_ADMIN)
                        {
                            status = Constants.PrescriptionStatus.PENDING_TRANSMISSION;
                        }
                    }
                }

                string authorizer = base.SessionUserID;

                //Authorizer : Delegate provider, 
                if (Convert.ToBoolean(Session["IsDelegateProvider"]) || Convert.ToBoolean(Session["IsPASupervised"]))
                {
                    if (base.DelegateProvider.UserType == (int)Constants.UserCategory.PHYSICIAN_ASSISTANT_SUPERVISED)
                    {
                        authorizer = Session["SUPERVISING_PROVIDER_ID"].ToString();
                    }
                    else
                    {
                        authorizer = Session["DelegateProviderID"].ToString();
                    }
                }

                if (DelegateProvider != null)
                {
                    logger.Debug("<RxProcessor-Authoriser>" + authorizer + "<RxProcessor-Authoriser>" + "<IsDelegateProvider>" + Convert.ToBoolean(Session["IsDelegateProvider"]).ToLogString() + "<IsDelegateProvider>"
                    + "<IsPASupervised>" + Convert.ToBoolean(Session["IsPASupervised"]).ToLogString() + " " + DelegateProvider.UserType + "<IsPASupervised>");
                }

                string deaNumber = GetDEANumberToBeUsed(PageState);
                string creationType = GetRxCreationType(PageState); //If refill req or changeRx, creationtype will be R

                _rx = new Prescription();

                _rx.SetHeaderInformation(base.SessionLicenseID, rx.RxID, rxDate, patientID, providerID, pharmacyID, rx.PlanID, rx.GroupID,
                    rx.FormularyID, "N", false, string.Empty, base.SessionSiteID, Constants.ERX_NOW_RX, null, base.DBID);

                _rx.AddMedication(base.SessionLicenseID, 0, rx.DDI, rx.MedicationName, rx.RouteOfAdminCode, rx.DosageFormCode, rx.Strength, rx.StrengthUOM, rx.SigID, rx.SigText,
                    rx.Refills, rx.Quantity, rx.DaysSupply, rx.GPPC, rx.PackageSize, rx.PackageUOM, rx.PackageQuantity, rx.PackageDescription, rx.DAW,
                    startDate, status, transmissionMethod, rx.OriginalDDI, rx.OriginalFormularyStatusCode, rx.OriginalIsListed, rx.SourceFormularyStatus, "N",
                    formularyStatus, Session["PERFORM_FORMULARY"].ToString(),
                    DURSettings.CheckPerformDose.ToChar(),
                    DURSettings.CheckDrugToDrugInteraction.ToChar(),
                    DURSettings.CheckDuplicateTherapy.ToChar(),
                    DURSettings.CheckPar.ToChar(),
                    Session["PRACTICESTATE"].ToString(), rx.Notes, rx.OriginalRxID, originalLineNumber, creationType, null, rx.ICD10Code, rx.ControlledSubstanceCode, base.SessionUserID,
                    "", authorizer, rxWorkFlow, extFacilityCode, extGroupID, rx.CoverageID, rx.AlternativeIgnoreReason, rx.StateControlledSubstanceCode, rx.FormularyAlternativeShown,
                    rx.PreDURDose, rx.PreDURPAR, rx.PreDURDrugFood, rx.PreDURDrugAlcohol, rx.PreDURDrugDrug, rx.PreDURDUP, rx.PreDURDisease, rx.PriorAuthRequired,
                    rx.IsCompoundMed, rx.IsFreeFormMedControlSubstance, rx.ScheduleUsed, rx.HasSupplyItem, deaNumber, rx.SigTypeId);

                bool isNewRx = false;
                if (Session[Constants.SessionVariables.TaskScriptMessageId] == null && PageState[Constants.SessionVariables.RxTask] == null 
                    || base.IsCsTaskWorkflow)  // For non-cs refills prescription save should be thru ScriptMessage.ApproveMessage
                {
                    isNewRx = true;
                    _rx.Save(base.SessionSiteID, base.SessionLicenseID, base.SessionUserID, base.IsCsTaskWorkflow, base.DBID);
                    PPTPlus.UpdatePharmacyIdToRx(PageState, _rx.rxID, base.DBID);
                    PPTPlus.UpdateTransactionIdToRx(PageState, _rx.rxID, new DAL.PptPlusData(), base.DBID);
                    if (!String.IsNullOrEmpty(rx.MDD))
                    {
                        Allscripts.Impact.Prescription.SaveMaximumDailyDosage(rx.RxID, rx.MDD.Trim(), base.DBID);
                    }
                }

                if (base.CanApplyFinancialOffers)
                {
                    // check if perscription has a coupon offer available
                    if (rx.IsCouponAvailable)
                    {
                        try
                        {
                            Task task = Task.Run(() => requestECoupon(rx));
                            task.Wait(2000);
                        }
                        catch (Exception ex)
                        {
                            Audit.AddException(base.SessionUserID, base.SessionLicenseID, "Attempt to spawn thread for eCoupon Request Matching offer failed: " + ex.ToString(), string.Empty, string.Empty, string.Empty, base.DBID);
                        }
                    }
                }

                if (isNewRx)
                {
                    CheckForRxInfo(rx);
                }

                string taskId = string.Empty;
                if (Session["TASKTYPE"] != null)
                {
                    Constants.PrescriptionTaskType tasktype = (Constants.PrescriptionTaskType)Session["TASKTYPE"];
                    switch (tasktype)
                    {
                        case Constants.PrescriptionTaskType.SEND_TO_ADMIN:
                            Session["TASKID"] = Prescription.SendToAssistant(rx.RxID, base.SessionLicenseID, base.SessionUserID, base.DBID);
                            break;
                        case Constants.PrescriptionTaskType.APPROVAL_REQUEST:
                            Session["TASKID"] = Prescription.SendToPhysicianForApproval(rx.RxID, base.SessionLicenseID, base.SessionUserID, providerID, base.DBID);
                            break;
                    }
                }

                //Check DUR only if Patient has been selected and this is tasking; otherwise DUR will be triggered from the script pad
                if (Session["PATIENTID"] != null && Session["Tasktype"] != null)
                {
                    durCheck();
                }

                if (Session[Constants.SessionVariables.TaskScriptMessageId] != null
                    && Session["TASKTYPE"] != null
                    && Session["TASKTYPE"].ToString() == Constants.PrescriptionTaskType.REFREQ.ToString() && Session[Constants.SessionVariables.IsCsRefReqWorkflow] == null && MasterPage.ChangeRxRequestedMedCs == null)
                {
                    if (base.IsPOBUser || base.SessionUserType == Constants.UserCategory.PHYSICIAN_ASSISTANT_SUPERVISED)
                    {
                        string delegateProviderID = string.Empty;
                        if (Session["SUPERVISING_PROVIDER_ID"] != null)
                        {
                            // get the correct delegate provider for refill request for PA with supervision when refill request is being processed by a POB user
                            delegateProviderID = Session["SUPERVISING_PROVIDER_ID"].ToString();
                        }
                        else
                        {
                            // get the correct delegate provider for refill request for PA with supervision when refill request is being processed by a POB user
                            delegateProviderID = Session["DelegateProviderID"] != null ? Session["DelegateProviderID"].ToString() : base.SessionUserID;
                        }

                        var sm = new ScriptMessage(Session[Constants.SessionVariables.TaskScriptMessageId].ToString(), SessionLicenseID, SessionUserID, DBID);
                        rxID = ScriptMessage.ApproveMessage(sm, rx.DDI, rx.RxID, rx.DaysSupply, rx.Quantity, rx.Refills, rx.SigText, rx.DAW,
                            providerID, rx.Notes, Constants.PrescriptionTransmissionMethod.SENT, base.SessionLicenseID,
                            base.SessionUserID, base.SessionSiteID, base.ShieldSecurityToken, rx.IsCompoundMed, rx.HasSupplyItem, delegateProviderID, rx.MDD, rx.SigID, base.DBID);
                        Session["REFILLMSG"] = "Refill for " + base.CurrentRx.MedicationName + " approved for " + Session["PATIENTNAME"].ToString().Trim() + ".";
                        Session.Remove(Constants.SessionVariables.TaskScriptMessageId);
                    }
                    else
                    {
                        string delegateProviderID = Session["DelegateProviderID"] != null ? Session["DelegateProviderID"].ToString() : base.SessionUserID;

                        var sm = new ScriptMessage(Session[Constants.SessionVariables.TaskScriptMessageId].ToString(), SessionLicenseID, SessionUserID, DBID);
                        rxID = ScriptMessage.ApproveMessage(sm, rx.DDI, rx.RxID, rx.DaysSupply, rx.Quantity, rx.Refills, rx.SigText, rx.DAW,
                            providerID, rx.Notes, Constants.PrescriptionTransmissionMethod.SENT, base.SessionLicenseID,
                            base.SessionUserID, base.SessionSiteID, base.ShieldSecurityToken, rx.IsCompoundMed, rx.HasSupplyItem, delegateProviderID, rx.MDD, rx.SigID, base.DBID);
                        Session["REFILLMSG"] = "Refill for " + base.CurrentRx.MedicationName + " approved for " + Session["PATIENTNAME"].ToString().Trim() + ".";
                        Session.Remove(Constants.SessionVariables.TaskScriptMessageId);
                    }
                }

                var rxTask = PageState.Cast(Constants.SessionVariables.RxTask, new RxTaskModel());
                if (rxTask.TaskType == Constants.PrescriptionTaskType.RXCHG || rxTask.TaskType == Constants.PrescriptionTaskType.RXCHG_PRIORAUTH)
                {
                    rxTask.DelegateProviderId = PageState.GetStringOrEmpty("DelegateProviderID").ToGuidOr0x0();
                    rxTask.UserId = new Guid(base.SessionUserID);
                    PageState["REFILLMSG"] = GetApprovalMessageAndRouteChangeRxWorkflow(rxTask, new ScriptMessage(),
                        base.IsPOBUser, PageState.GetStringOrEmpty("SUPERVISING_PROVIDER_ID"),
                        PageState.GetStringOrEmpty("PATIENTNAME"), PageState[Constants.SessionVariables.ShieldSendRxAuthToken]?.ToString());
                    PageState[Constants.SessionVariables.RxTask] = null;
                    PageState[Constants.SessionVariables.TaskScriptMessageId] = null;
                }

                if (Session[Constants.SessionVariables.IsCsRefReqWorkflow] == null && MasterPage.ChangeRxRequestedMedCs == null) //Added for back/forward navigation for new cs reqref workflow
                    base.ClearMedicationInfo(false);
            }

            base.ScriptPadMeds = null;
        }

        private void LogError()
        {
            logger.Error($"The RxList is empty. Debug info: <SessionInfo>{Session.GetSessionContentString()}</SessionInfo>");
        }

        private void CheckForRxInfo(Rx rx)
        {
            if (base.IsShowRxInfo)
            {
                if (EPSBroker.IsRxInfoAvailableForDDI(rx.DDI, base.DBID))
                {
                    try
                    {
                        // spawn a thread to call for getting RxInfo
                        Task task = Task.Run(() => requestRxInfo(rx));
                        task.Wait(2000);
                    }
                    catch (Exception ex)
                    {
                        Audit.AddException(base.SessionUserID, base.SessionLicenseID, "Attempt to spawn thread for RxInfo Requestfailed : " + ex.ToString(), string.Empty, string.Empty, string.Empty, base.DBID);
                    }
                }
            }
        }
        private void LoadProviderPreference()
        {
            _performPAR = DURSettings.CheckPar == YesNoSetting.Yes;
            _performInteractions = DURSettings.CheckDrugToDrugInteraction == YesNoSetting.Yes;
            _performDosageChecks = DURSettings.CheckPerformDose == YesNoSetting.Yes;
            _performDuplicateTherapy = DURSettings.CheckDuplicateTherapy == YesNoSetting.Yes;
            _performFoodCheck = DURSettings.CheckFoodInteraction == YesNoSetting.Yes;
            _performAlcoholCheck = DURSettings.CheckAlcoholInteraction == YesNoSetting.Yes;
        }

        private void durCheck()
        {
            int patientAge = -1;
            int patientAgeCategory = -1;
            bool hasDURWarnings = false;

            if (_performPAR || _performDuplicateTherapy || _performDosageChecks || _performInteractions || _performFoodCheck || _performAlcoholCheck)
            {
                if (Session["PATIENT_YEARS_OLD"] == null || Session["PATIENT_AGE_CATEGORY"] == null || Session["SEX"] == null
                    || !int.TryParse(Session["PATIENT_YEARS_OLD"].ToString(), out patientAge)
                    || !int.TryParse(Session["PATIENT_AGE_CATEGORY"].ToString(), out patientAgeCategory))
                {
                    Allscripts.Impact.Patient _patient = new Patient(patientID, base.SessionLicenseID, base.SessionUserID, base.DBID);
                    patientAge = _patient.YearsOld;
                    patientAgeCategory = Convert.ToInt32(_patient.AgeCategory);
                }
                DURCheckResponse durCheckResponse = ConstructAndSendDURRequest();
                hasDURWarnings = DUR.HasWarnings(durCheckResponse);
                List<Rx> currentRxList = new List<Rx>();
                currentRxList = PageState.Cast(Constants.SessionVariables.RxList, new ArrayList()).ToList<Rx>();
                List<Rx> rxList = new List<Rx>();

                var taskType = DURMedispanUtils.RetrieveTaskType(MasterPage.RxTask, PageState.Cast(Constants.SessionVariables.TaskType, Constants.PrescriptionTaskType.DEFAULT));
                rxList = DURMedispanUtils.RetrieveDrugsListBasedOnWorkflowType(
                    currentRxList,
                    ScriptPadMeds,
                    taskType);

                if (hasDURWarnings || (DURMedispanUtils.RetrieveFreeFormDrugs(rxList).HasItems() && DURMedispanUtils.IsAnyDurSettingOn(DURSettings)))
                {
                    ////not serializable...
                    base.MedsWithDURs.Add(rxID);
                    if (Request.QueryString.Count > 0)
                    {
                        if (Request.QueryString["To"] != null &&
                            (Request.QueryString["To"].Equals(Constants.PageNames.SCRIPT_PAD) ||
                            Request.QueryString["To"].Equals(Constants.PageNames.RX_DUR_REVIEW_MULTI_SELECT, StringComparison.OrdinalIgnoreCase)))
                        {
                            if (!Request.QueryString["To"].Equals(Constants.PageNames.RX_DUR_REVIEW_MULTI_SELECT, StringComparison.OrdinalIgnoreCase))
                                Session["DUR_GO_NEXT"] = Request.QueryString["To"];

                            Server.Transfer(Constants.PageNames.RX_DUR_REVIEW_MULTI_SELECT + "?" + Request.QueryString, true);
                        }
                        else if (Request.QueryString["To"] != null &&
                            (Request.QueryString["To"].Equals(Constants.PageNames.REDIRECT_TO_ANGULAR + "?componentName=" + Constants.PageNames.SELECT_MEDICATION, StringComparison.OrdinalIgnoreCase)))
                        {
                            if (Request.QueryString["To"].Contains("Full"))
                                Server.Transfer(Constants.PageNames.REDIRECT_TO_ANGULAR + "?componentName=" + Constants.PageNames.SELECT_MEDICATION);
                        }
                        else
                        {
                            if (Convert.ToBoolean(Request.QueryString["POBRefReq"]) || (Session[Constants.SessionVariables.TaskScriptMessageId] != null) || (PageState[Constants.SessionVariables.RxTask] != null))
                            {
                                Server.Transfer(Constants.PageNames.RX_DUR_REVIEW_MULTI_SELECT + "?" + Request.QueryString, true);
                            }
                            else
                            {
                                Server.Transfer(Constants.PageNames.RX_DUR_REVIEW + "?" + Request.QueryString, true);
                            }
                        }
                    }
                    else
                    {
                        Server.Transfer(Constants.PageNames.RX_DUR_REVIEW, true);
                    }
                }
                else
                {
                    if (Session["TASKID"] != null)
                    {
                        Constants.PrescriptionTaskType tasktype = (Constants.PrescriptionTaskType)Session["TASKTYPE"];
                        if (tasktype == Constants.PrescriptionTaskType.RENEWAL_REQUEST)
                        {
                            //EAK added update
                            Prescription.UpdateRxTask(Convert.ToInt64(Session["TASKID"]), Constants.PrescriptionTaskType.RENEWAL_REQUEST, Constants.PrescriptionTaskStatus.PROCESSED, Constants.PrescriptionStatus.NEW, null, string.Empty, base.SessionUserID, base.DBID);
                            //END EAK update   
                            Session.Remove("TASKID");
                        }

                        if (Convert.ToBoolean(Session["ISPROVIDER"]))
                        {
                            if (tasktype == Constants.PrescriptionTaskType.APPROVAL_REQUEST)
                            {
                                if (_rx.DS.Tables["RxHeader"].Rows[0]["PharmacyID"] == Convert.DBNull || _rx.DS.Tables["RxHeader"].Rows[0]["PharmacyID"].ToString() == Guid.Empty.ToString())
                                {
                                    //we still need to reconcile the pharmacy
                                    Patient pat = new Patient(_rx.PatientID, base.SessionLicenseID, base.SessionUserID, base.DBID);
                                    Session["PATIENTZIP"] = pat.DS.Tables[0].Rows[0]["ZIP"].ToString();
                                    Response.Redirect(Constants.PageNames.PHARMACY + "?From=" + Constants.PageNames.APPROVE_REFILL_TASK + "&To=" + Constants.PageNames.APPROVE_REFILL_TASK);
                                }
                                else
                                {
                                    //send it
                                    string scriptId = ScriptMessage.CreateScriptMessage(_rx.ID, 1, Constants.MessageTypes.NEWRX, base.SessionLicenseID, base.SessionUserID, base.ShieldSecurityToken, base.SessionSiteID, base.DBID);

                                    if (Session["STANDING"].ToString() == "1")
                                    {
                                        ScriptMessage.SendThisMessage(scriptId, base.SessionLicenseID, base.SessionUserID, base.DBID);
                                    }
                                    Int64 taskID = Convert.ToInt64(Session["TASKID"]);
                                    Prescription.UpdateRxTask(taskID, Constants.PrescriptionTaskType.APPROVAL_REQUEST, Constants.PrescriptionTaskStatus.PROCESSED, Constants.PrescriptionStatus.NEW, null, string.Empty, base.SessionUserID, base.DBID);

                                    DataSet patds = Patient.GetPatientData(_rx.PatientID, base.SessionLicenseID, base.SessionUserID, base.DBID);
                                    DataRow patdr = patds.Tables[0].Rows[0];
                                    Session["REFILLMSG"] = "Rx for " + _rx.DS.Tables["RxDetail"].Rows[0]["MedicationName"].ToString() + " approved for " + patdr["PatientName"].ToString() + ".";

                                    if (Request.QueryString["To"] != null)
                                    {
                                        Response.Redirect(Constants.PageNames.UrlForRedirection(Request.QueryString["To"]));
                                    }
                                       
                                }
                            }
                        }
                    }
                }
            }
        }

        private DURCheckResponse ConstructAndSendDURRequest()
        {
            DURPatient patient = DURMedispanUtils.RetrieveDURPatient(PageState.GetStringOrEmpty(Constants.SessionVariables.PatientDOB),
        PageState.GetStringOrEmpty(Constants.SessionVariables.Gender));
            List<Rx> selectedRxList = new List<Rx>();
            Rx newRx = new Rx(_rx);
            selectedRxList.Add(newRx);

            List<DrugToCheck> drugList = DURMedispanUtils.RetrieveDrugList(selectedRxList, new DrugToCheckUtils(), DURMedispanUtils.DosageFormCodes);

            List<int> existingMedsList =
              DURMedispanUtils.RetrieveActiveMedsList(Session[Constants.SessionVariables.ACTIVEMEDDDILIST] as List<string>);

            List<Allscripts.ePrescribe.Medispan.Clinical.Model.Allergy> allergyList = DurPatientAllergies;

            List<FreeFormRxDrug> freeFormRxDrugs = DURMedispanUtils.RetrieveFreeFormDrugs(selectedRxList);
            Allscripts.ePrescribe.Medispan.Clinical.Model.Settings.DURSettings DURsettingsToUse =
                DURMedispanUtils.RetrieveDURSettings(DURSettings);

            InteractionsManagementLevel mgtLevel = InteractionsManagementLevel.PotentialInteractionRisk;  // hard coding!! - JK

            DURCheckResponse durResponse = DURMSC.PerformDURCheck(patient, freeFormRxDrugs, drugList, existingMedsList,
                allergyList, DURsettingsToUse, mgtLevel);
            return durResponse;
        }

        private void requestRxInfo(object rx)
        {
            EPSBroker.RequestRxInfo(((Rx)rx).RxID, base.SessionLicenseID, base.SessionUserID, base.DBID);
        }

        private void requestECoupon(object rx)
        {
            EPSBroker.RequestECoupon(((Rx)rx).RxID, Constants.ECouponWorkFlowType.NEWRX, base.SessionLicenseID, base.SessionUserID, base.DBID);
        }
        internal string GetRxCreationType(IStateContainer session)
        {
            return (session.GetStringOrEmpty(Constants.SessionVariables.IsCsRefReqWorkflow) != null || session.GetStringOrEmpty(Constants.SessionVariables.ChangeRxRequestedMedCs) != null) 
                && !string.IsNullOrEmpty(session.GetStringOrEmpty(Constants.SessionVariables.TaskScriptMessageId)) ? Constants.RxCreationType.REFILL : Constants.RxCreationType.STANDARD_WORKFLOW;
        }
    }
}