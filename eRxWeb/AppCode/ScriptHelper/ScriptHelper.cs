using Allscripts.ePrescribe.Common;
using Allscripts.Impact;
using eRxWeb.State;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Allscripts.ePrescribe.ExtensionMethods;
using Allscripts.Impact.Interfaces;
using System.Threading.Tasks;
using Allscripts.ePrescribe.Medispan.Clinical.Model.Settings;
using eRxWeb.AppCode;
using eRxWeb.AppCode.StateUtils;
using Constants = Allscripts.ePrescribe.Common.Constants;
using eRxWeb.AppCode.PptPlusBPL;
using Allscripts.ePrescribe.Shared.Logging;
using DAL = Allscripts.ePrescribe.Data;
using Allscripts.ePrescribe.Objects;
using ScriptMessage = Allscripts.Impact.ScriptMsg.ScriptMessage;
using Rx = Allscripts.Impact.Rx;
using Patient = Allscripts.Impact.Patient;
using RxUser = Allscripts.Impact.RxUser;
using System.Data;
using Allscripts.ePrescribe.DatabaseSelector;
using Allscripts.Impact.ePrescribeSvc;
using ConnectionStringPointer = Allscripts.ePrescribe.DatabaseSelector.ConnectionStringPointer;
using FormularyStatus = Allscripts.Impact.FormularyStatus;

namespace eRxWeb.AppCode.AddRx
{
    public class ScriptHelper
    {
        private static readonly ILoggerEx logger = LoggerEx.GetLogger();
        public Prescription _rx;
        string patientID = string.Empty;
        string rxID = Guid.NewGuid().ToString();
        bool _formularyActive = false;
        IStateContainer PageState = new StateContainer(HttpContext.Current.Session);

        string providerID = string.Empty;
        string pharmacyID = string.Empty;
        string SessionPatientID => PatientInfo.GetSessionPatientID(PageState);
        string SessionUserID => UserInfo.GetSessionUserID(PageState);
        private ConnectionStringPointer SessionDbId => ApiHelper.GetDBID(PageState);
        RxUser DelegateProvider => UserInfo.GetDelegateProvider(PageState);
        string SessionLicenseID => UserInfo.GetSessionLicenseID(PageState);
        int SessionSiteID => SiteInfo.GetSessionSiteID(PageState);

        Allscripts.ePrescribe.Objects.DURSettings durSetting => DurInfo.GetDURSettings(PageState);
        bool CanApplyFinancialOffers => UserInfo.CanApplyFinancialOffers(PageState);
        bool IsPOBUser => UserInfo.IsPOBUser(PageState);
        Constants.UserCategory SessionUserType => UserInfo.GetSessionUserType(PageState);
        string ShieldSecurityToken => ShieldInfo.GetShieldSecurityToken(PageState);
        Rx CurrentRx => MedicationInfo.CurrentRx(PageState);
        List<Rx> ScriptPadMeds => MedicationInfo.GetScriptPadMeds(PageState);


        //DUR Check
        bool _performDosageChecks = false;
        bool _performInteractions = false;
        bool _performDuplicateTherapy = false;
        bool _performPAR = false;
        bool _performFoodCheck = false;
        bool _performAlcoholCheck = false;
        public void savePrescriptions()
        {
            Constants.PrescriptionStatus status = Constants.PrescriptionStatus.SCRIPT_PAD;
            string rxDate = DateTime.UtcNow.ToString();
            string SIGID = string.Empty;
            string startDate = DateTime.Today.ToShortDateString();
            //setting transmission method to NULL b/c it hasn't been transmitted yet. Only set transmission method on script pad page or as final step in processing task.
            string transmissionMethod = null;
            FormularyStatus formularyStatus = FormularyStatus.ACCEPTED;
            int originalLineNumber = PageState["OriginalRxID"] != null ? 1 : 0;
            PrescriptionWorkFlow rxWorkFlow = PageState["RX_WORKFLOW"] != null ? ((PrescriptionWorkFlow)Convert.ToInt32(PageState["RX_WORKFLOW"])) : PrescriptionWorkFlow.STANDARD;
            string extFacilityCode = null;
            string extGroupID = null;
            var rxList = PageState.Cast("RxList", new ArrayList());
            if (rxList.Count == 0)
            {
                logger.Error($"The RxList is empty. Debug info: <SessionInfo>{PageState.GetSessionContentString()}</SessionInfo>");
            }
            logger.Debug($"savePrescriptions(): <RxList>{rxList.ToList<Rx>().ToLogString()}</RxList>");

            foreach (Rx rx in rxList)
            {
                if (string.IsNullOrEmpty(rx.RxID) || rx.RxID == Guid.Empty.ToString())
                {
                    rx.RxID = Guid.NewGuid().ToString();
                }

                PageState["OriginalRxID"] = null;

                if (!string.IsNullOrEmpty(SessionPatientID))
                    patientID = SessionPatientID;
                else
                    patientID = System.Guid.Empty.ToString();

                //if (Request.QueryString["PatPharm"] != null && Request.QueryString["PatPharm"] == "Y")
                //    Session["PHARMACYID"] = (Session["LASTPHARMACYID"] != null ? Session["LASTPHARMACYID"].ToString() : System.Guid.Empty.ToString());

              
                pharmacyID = (PageState["PHARMACYID"] != null ? PageState["PHARMACYID"].ToString() : System.Guid.Empty.ToString());

                formularyStatus = (rx.FormularyStatus != null ? (FormularyStatus)Convert.ToInt32(rx.FormularyStatus) : FormularyStatus.NONE);

                providerID = SessionUserID;

                if (PageState["FormularyActive"] != null)
                    _formularyActive = (PageState["FormularyActive"].ToString() == "Y" ? true : false);

                LoadProviderPreference();

                if (PageState["ExtFacilityCd"] != null)
                    extFacilityCode = PageState["ExtFacilityCd"].ToString();

                if (PageState["ExtGroupID"] != null)
                    extGroupID = PageState["ExtGroupID"].ToString();


                if (Convert.ToBoolean(PageState["IsDelegateProvider"])) //For delegate providers
                {
                    //If the TASKTYPE is to send to the script to provider for approval, 
                    //then change the script status from NEW to PENDING_APPROVAL
                    if (PageState["TASKTYPE"] != null)
                    {
                        Constants.PrescriptionTaskType tasktype = (Constants.PrescriptionTaskType)PageState["TASKTYPE"];
                        if (tasktype == Constants.PrescriptionTaskType.APPROVAL_REQUEST)
                        {
                            status = Constants.PrescriptionStatus.PENDING_APPROVAL;
                        }
                    }

                    //check both session variables here; we've seen instances of the PHYSICIANID being null but the DelegateProviderID being populated; they should be the same
                    if (PageState["PHYSICIANID"] != null)
                    {
                        providerID = PageState["PHYSICIANID"].ToString();
                    }
                    else if (PageState["DelegateProviderID"] != null)
                    {
                        providerID = PageState["DelegateProviderID"].ToString();
                    }
                }
                else
                {
                    if (PageState["TASKTYPE"] != null)
                    {
                        //If the provider is sending the script to assistant, then mark the script as pending transmission. 
                        Constants.PrescriptionTaskType taskType = (Constants.PrescriptionTaskType)PageState["TASKTYPE"];
                        if (taskType == Constants.PrescriptionTaskType.SEND_TO_ADMIN)
                        {
                            status = Constants.PrescriptionStatus.PENDING_TRANSMISSION;
                        }
                    }
                }

                string authorizer = SessionUserID;

                //Authorizer : Delegate provider, 
                if (Convert.ToBoolean(PageState["IsDelegateProvider"]) || Convert.ToBoolean(PageState["IsPASupervised"]))
                {
                    if (DelegateProvider.UserType == (int)Constants.UserCategory.PHYSICIAN_ASSISTANT_SUPERVISED)
                    {
                        authorizer = PageState["SUPERVISING_PROVIDER_ID"].ToString();
                    }
                    else
                    {
                        authorizer = PageState["DelegateProviderID"].ToString();
                    }
                }

                string deaNumber = UserInfo.GetDEANumberToBeUsed(PageState);

                _rx = new Prescription();

                _rx.SetHeaderInformation(SessionLicenseID, rx.RxID, rxDate, patientID, providerID, pharmacyID, rx.PlanID, rx.GroupID,
                    rx.FormularyID, "N", false, string.Empty, SessionSiteID, Constants.ERX_NOW_RX, null, ApiHelper.GetDBID(PageState));

                _rx.AddMedication(SessionLicenseID, 0, rx.DDI, rx.MedicationName, rx.RouteOfAdminCode, rx.DosageFormCode, rx.Strength, rx.StrengthUOM, rx.SigID, rx.SigText,
                    rx.Refills, rx.Quantity, rx.DaysSupply, rx.GPPC, rx.PackageSize, rx.PackageUOM, rx.PackageQuantity, rx.PackageDescription, rx.DAW,
                    startDate, status, transmissionMethod, rx.OriginalDDI, rx.OriginalFormularyStatusCode, rx.OriginalIsListed, rx.SourceFormularyStatus, "N",
                    formularyStatus, PageState["PERFORM_FORMULARY"].ToString(),
                    durSetting.CheckPerformDose.ToChar(),
                    durSetting.CheckDrugToDrugInteraction.ToChar(),
                    durSetting.CheckDuplicateTherapy.ToChar(),
                    durSetting.CheckPar.ToChar(),
                    PageState["PRACTICESTATE"].ToString(), rx.Notes, rx.OriginalRxID, originalLineNumber, "Q", null, rx.ICD10Code, rx.ControlledSubstanceCode, SessionUserID,
                    "", authorizer, rxWorkFlow, extFacilityCode, extGroupID, rx.CoverageID, rx.AlternativeIgnoreReason, rx.StateControlledSubstanceCode, rx.FormularyAlternativeShown,
                    rx.PreDURDose, rx.PreDURPAR, rx.PreDURDrugFood, rx.PreDURDrugAlcohol, rx.PreDURDrugDrug, rx.PreDURDUP, rx.PreDURDisease, rx.PriorAuthRequired,
                    rx.IsCompoundMed, rx.IsFreeFormMedControlSubstance, rx.ScheduleUsed, rx.HasSupplyItem, deaNumber, rx.SigTypeId);

                bool isNewRx = false;
                if (PageState[Constants.SessionVariables.TaskScriptMessageId] == null && PageState[Constants.SessionVariables.RxTask] == null)  // For refills prescription save should be thru ScriptMessage.ApproveMessage
                {
                    isNewRx = true;
                    _rx.Save(SessionSiteID, SessionLicenseID, SessionUserID, ApiHelper.GetDBID(PageState));
                    PPTPlus.UpdatePharmacyIdToRx(PageState, _rx.rxID, ApiHelper.GetDBID(PageState));
                    PPTPlus.UpdateTransactionIdToRx(PageState, _rx.rxID, new DAL.PptPlusData(), ApiHelper.GetDBID(PageState));
                    if (!String.IsNullOrEmpty(rx.MDD))
                    {
                        Allscripts.Impact.Prescription.SaveMaximumDailyDosage(rx.RxID, rx.MDD.Trim(), ApiHelper.GetDBID(PageState));
                    }
                }

                if (CanApplyFinancialOffers)
                {
                    // check if perscription has a coupon offer available
                    if (rx.IsCouponAvailable)
                    {
                        try
                        {
                            Task task = Task.Run(() => RXeCoupon.requestECoupon(rx,PageState));
                            task.Wait(2000);
                        }
                        catch (Exception ex)
                        {
                            Audit.AddException(SessionUserID, SessionLicenseID, "Attempt to spawn thread for eCoupon Request Matching offer failed: " +ex.ToString(), string.Empty, string.Empty, string.Empty, ApiHelper.GetDBID(PageState));
                        }
                    }
                }

                if (isNewRx)
                {
                    RxInfo.CheckForRxInfo(PageState, rx);
                }

                string taskId = string.Empty;
                if (PageState["TASKTYPE"] != null)
                {
                    Constants.PrescriptionTaskType tasktype = (Constants.PrescriptionTaskType)PageState["TASKTYPE"];
                    switch (tasktype)
                    {
                        case Constants.PrescriptionTaskType.SEND_TO_ADMIN:
                            PageState["TASKID"] = Prescription.SendToAssistant(rx.RxID, SessionLicenseID, SessionUserID, ApiHelper.GetDBID(PageState));
                            break;
                        case Constants.PrescriptionTaskType.APPROVAL_REQUEST:
                            PageState["TASKID"] = Prescription.SendToPhysicianForApproval(rx.RxID, SessionLicenseID,SessionUserID, providerID, ApiHelper.GetDBID(PageState));
                            break;

                    }
                }
                
                if (PageState[Constants.SessionVariables.TaskScriptMessageId] != null
                    && PageState["TASKTYPE"] != null
                    && PageState["TASKTYPE"].ToString() == Constants.PrescriptionTaskType.REFREQ.ToString())
                {
                    if (IsPOBUser || SessionUserType == Constants.UserCategory.PHYSICIAN_ASSISTANT_SUPERVISED)
                    {
                        string delegateProviderID = string.Empty;
                        if (PageState["SUPERVISING_PROVIDER_ID"] != null)
                        {
                            // get the correct delegate provider for refill request for PA with supervision when refill request is being processed by a POB user
                            delegateProviderID = PageState["SUPERVISING_PROVIDER_ID"].ToString();
                        }
                        else
                        {
                            // get the correct delegate provider for refill request for PA with supervision when refill request is being processed by a POB user
                            delegateProviderID = PageState["DelegateProviderID"] != null ? PageState["DelegateProviderID"].ToString() : SessionUserID;
                        }

                        var sm = new ScriptMessage(PageState[Constants.SessionVariables.TaskScriptMessageId].ToString(), SessionLicenseID, SessionUserID, ApiHelper.GetDBID(PageState));
                        rxID = ScriptMessage.ApproveMessage(sm, rx.DDI, rx.RxID, rx.DaysSupply, rx.Quantity, rx.Refills, rx.SigText, rx.DAW,
                            providerID, rx.Notes, Constants.PrescriptionTransmissionMethod.SENT, SessionLicenseID,
                            SessionUserID, SessionSiteID, ShieldSecurityToken, rx.IsCompoundMed, rx.HasSupplyItem, delegateProviderID, rx.MDD, rx.SigID, ApiHelper.GetDBID(PageState));
                        PageState["REFILLMSG"] = "Refill for " + CurrentRx.MedicationName + " approved for " + PageState["PATIENTNAME"].ToString().Trim() + ".";
                        PageState.Remove(Constants.SessionVariables.TaskScriptMessageId);
                    }
                    else
                    {
                        string delegateProviderID = PageState["DelegateProviderID"] != null ? PageState["DelegateProviderID"].ToString() : SessionUserID;

                        var sm = new ScriptMessage(PageState[Constants.SessionVariables.TaskScriptMessageId].ToString(), SessionLicenseID, SessionUserID, ApiHelper.GetDBID(PageState));
                        rxID = ScriptMessage.ApproveMessage(sm, rx.DDI, rx.RxID, rx.DaysSupply, rx.Quantity, rx.Refills, rx.SigText, rx.DAW,
                            providerID, rx.Notes, Constants.PrescriptionTransmissionMethod.SENT, SessionLicenseID,
                            SessionUserID, SessionSiteID, ShieldSecurityToken, rx.IsCompoundMed, rx.HasSupplyItem, delegateProviderID, rx.MDD, rx.SigID, ApiHelper.GetDBID(PageState));
                        PageState["REFILLMSG"] = "Refill for " + CurrentRx.MedicationName + " approved for " + PageState["PATIENTNAME"].ToString().Trim() + ".";
                        PageState.Remove(Constants.SessionVariables.TaskScriptMessageId);
                    }
                }

                var rxTask = PageState.Cast(Constants.SessionVariables.RxTask, new RxTaskModel());
                if (rxTask.TaskType == Constants.PrescriptionTaskType.RXCHG || rxTask.TaskType == Constants.PrescriptionTaskType.RXCHG_PRIORAUTH)
                {
                    rxTask.DelegateProviderId = PageState.GetStringOrEmpty("DelegateProviderID").ToGuidOr0x0();
                    rxTask.UserId = new Guid(SessionUserID);
                    ChangeRxHelper chlp = new ChangeRxHelper();
                    PageState["REFILLMSG"] = ChangeRxHelper.GetApprovalMessageAndRouteChangeRxWorkflow(rxTask, new ScriptMessage(), IsPOBUser, PageState.GetStringOrEmpty("SUPERVISING_PROVIDER_ID"), PageState.GetStringOrEmpty("PATIENTNAME"), PageState.GetStringOrEmpty(Constants.SessionVariables.ShieldSendRxAuthToken), ref rxID);
                    PageState[Constants.SessionVariables.RxTask] = null;
                    PageState[Constants.SessionVariables.TaskScriptMessageId] = null;
                }

                MedicationInfo.ClearMedInfo(PageState, false);
            }

            PageState["CURRENT_SCRIPT_PAD_MEDS"] = null;
        }

        private void LoadProviderPreference()
        {
            _performPAR = durSetting.CheckPar == YesNoSetting.Yes;
            _performInteractions = durSetting.CheckDrugToDrugInteraction == YesNoSetting.Yes;
            _performDosageChecks = durSetting.CheckPerformDose == YesNoSetting.Yes;
            _performDuplicateTherapy = durSetting.CheckDuplicateTherapy == YesNoSetting.Yes;
            _performFoodCheck = durSetting.CheckFoodInteraction == YesNoSetting.Yes;
            _performAlcoholCheck = durSetting.CheckAlcoholInteraction == YesNoSetting.Yes;

        }

    }
}