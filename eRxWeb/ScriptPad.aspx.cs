using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.Data;
using Allscripts.ePrescribe.Shared.Logging;
using Allscripts.Impact;
using Telerik.Web.UI;
using Allscripts.ePrescribe.Objects;
using Allscripts.Impact.Parameters;
using Allscripts.Impact.ReturnTypes;
using eRxWeb.AppCode;
using eRxWeb.Controls.Interfaces;
using eRxWeb.PageInterfaces;
using eRxWeb.State;
using AuditAction = eRxWeb.ePrescribeSvc.AuditAction;
using ePA = Allscripts.Impact.ePA;
using Patient = Allscripts.Impact.Patient;
using Prescription = Allscripts.Impact.Prescription;
using Rx = Allscripts.Impact.Rx;
using RxUser = Allscripts.Impact.RxUser;
using ScriptMessage = Allscripts.Impact.ScriptMsg.ScriptMessage;
using UserProperties = eRxWeb.AppCode.UserProperties;
using eRxWeb.AppCode.SpecialtyMedWorkflow;
using eRxWeb.AppCode.Interfaces;
using ISpecialtyMed = eRxWeb.AppCode.Interfaces.ISpecialtyMed;
using Allscripts.ePrescribe.ExtensionMethods;
using eRxWeb.AppCode.PptPlusBPL;
using Allscripts.ePrescribe.Objects.PPTPlus;
using PatientCoverage = Allscripts.Impact.PatientCoverage;
using eRxWeb.ServerModel;
using Allscripts.ePrescribe.Data.CommonComponent;
using Allscripts.Impact.ePrescribeSvc;
using eRxWeb.AppCode.PdmpBPL;
using eRxWeb.AppCode.Rtps;
using Allscripts.Impact.Interfaces;
using ConnectionStringPointer = Allscripts.ePrescribe.DatabaseSelector.ConnectionStringPointer;
using Height = Allscripts.ePrescribe.Objects.Height;
using Weight = Allscripts.ePrescribe.Objects.Weight;
using static Allscripts.ePrescribe.Common.Constants;

namespace eRxWeb
{
    [Serializable]
    public struct ScriptPadUserMessage
    {
        public string MessageText { get; set; }
        public Controls_Message.MessageType MessageType { get; set; }
    }

    internal enum SaveScriptWorkflow
    {
        Epcs,
        MailOrder
    }

    public partial class ScriptPad : BasePage, IScriptPad
    {
        private static readonly ILoggerEx logger = LoggerEx.GetLogger();

        #region Member Fields

        private bool hasWarnings = false;
        private bool hasCSMeds = false;
        private Dictionary<string, string> _signedMeds = null;
        private List<string> _unsignedMeds = null;
        private bool IsMismatch = false;
        private bool isSelfReport = false;
        private bool hasPharmacyChanges = false;
        #endregion

        #region Properties

        public List<string> epcsEligibleMedList
        {
            get
            {
                if (PageState["EPCSELIGIBLEMEDLIST"] == null)
                {
                    PageState["EPCSELIGIBLEMEDLIST"] = new List<string>();
                }

                return (List<string>)PageState["EPCSELIGIBLEMEDLIST"];
            }
            set { PageState["EPCSELIGIBLEMEDLIST"] = value; }
        }

        #endregion

        #region Page Events and Event Handlers

        protected void Page_Load(object sender, EventArgs e)
        {
            btnCheckRegistry.OnClientClick = "OpenNewWindow" + "('" + PageState.GetStringOrEmpty("STATEREGISTRYURL") + "')";

            // subscribe the OnDigitalSigning event handler
            this.ucEPCSDigitalSigning.OnDigitalSigningComplete +=
               new Controls_EPCSDigitalSigning.DigitalSigningCompleteHandeler(
                   ucEPCSDigitalSigning_OnDigitalSigningComplete);

            ucSpecialtyMedsUserWelcome.OnSpecMedsWelcomeOK += OnSpecMedsWelcomeComplete;


            ucPatMissingInfo.OnPatEditComplete += UcPatMissingInfoOnComplete;
            this.ucRxCopy.OnRxCopyComplete += new Controls_RxCopy.RxCopyCompleteHandler(ucRxCopy_OnRxCopyComplete);

            //User Controls Ajax Settings
            Panel pnlMainRxCopy = (Panel)ucRxCopy.FindControl("pnlMainRxCopy");
            Button btnSaveAndReviewRxCopy = (Button)ucRxCopy.FindControl("btnSaveAndReviewRxCopy");
            Button btnCancelRxCopy = (Button)ucRxCopy.FindControl("btnCancelRxCopy");

            radAjaxManager.AjaxSettings.AddAjaxSetting(grdScriptPad, pnlMainRxCopy);
            radAjaxManager.AjaxSettings.AddAjaxSetting(btnSaveAndReviewRxCopy, grdScriptPad);
            radAjaxManager.AjaxSettings.AddAjaxSetting(btnSaveAndReviewRxCopy, btnCancelRxCopy);
            radAjaxManager.AjaxSettings.AddAjaxSetting(btnSaveAndReviewRxCopy, ucRxCopyMessage);

            if (!IsPostBack)
            {
                if ((PhysicianMasterPage) Master != null)
                {
                    grdScriptPad.Height = ((PhysicianMasterPage)Master).getTableHeight();
                }

                if (PageState.GetBooleanOrFalse("CHECK_DUR"))
                {
                    if (PageState.GetBooleanOrFalse("IsDelegateProvider"))
                    {
                        PageState["DUR_GO_PREVIOUS"] = Constants.PageNames.REDIRECT_TO_ANGULAR + "?componentName=" + Constants.PageNames.SELECT_MEDICATION;
                    }
                    else
                    {
                        if (PageState.EqualsToEnum("UserType", Constants.UserCategory.PHYSICIAN_ASSISTANT_SUPERVISED))
                        {
                            PageState["DUR_GO_PREVIOUS"] = Constants.PageNames.REDIRECT_TO_ANGULAR + "?componentName=" + Constants.PageNames.SELECT_MEDICATION;
                        }
                        else
                        {
                            PageState["DUR_GO_PREVIOUS"] = Constants.PageNames.REDIRECT_TO_ANGULAR + "?componentName=" + Constants.PageNames.SELECT_MEDICATION;
                        }
                    }

                    PageState["DUR_GO_NEXT"] = Constants.PageNames.SCRIPT_PAD + "?from=" + Constants.PageNames.SCRIPT_PAD;
                    Response.Redirect(Constants.PageNames.RX_DUR_REVIEW_MULTI_SELECT);
                }

                //turn off patient receipt
                //CheckPatientReceipt();
                turnOffPatientReceipt();

                //don't show Choose Patient button in patient lock down since the user can't select another patient
                if (PageState.GetStringOrEmpty("SSOMode") == "PatientLockDownMode")
                {
                    btnCancel.Visible = false;
                }

                if (SessionLicense.EnterpriseClient.ShowPharmacy)
                {
                    var cameFrom = Request.QueryString["from"];
                    //only check if we have non-schedule meds!
                    if (PageState["PHARMACYID"] == null && PageState["LASTPHARMACYID"] == null &&
                        PageState["MOB_NABP"] == null)
                    {
                        string queryUserID = string.Empty;
                        if (PageState.GetBooleanOrFalse("ISDELEGATEPROVIDER"))
                        {
                            queryUserID = PageState.GetStringOrEmpty("DELEGATEPROVIDERID");
                        }
                        else
                        {
                            queryUserID = SessionUserID;
                        }

                        bool hasRetailPotential = false;
                        foreach (Rx med in base.ScriptPadMeds)
                        {
                            if ((string.IsNullOrWhiteSpace(med.ControlledSubstanceCode) ||
                                med.ControlledSubstanceCode.ToUpper() == "U" || base.CanTryEPCS)
                                && (string.IsNullOrWhiteSpace(med.PharmacyID) || med.PharmacyID.ToString() == Guid.Empty.ToString()))
                            {
                                hasRetailPotential = true;
                                break;
                            }
                        }

                        if (hasRetailPotential)
                        {
                            if (Request.QueryString["from"] != null)
                            {
                                if (
                                    Request.QueryString["from"].ToLower()
                                        .IndexOf(Constants.PageNames.PHARMACY.ToLower()) == -1 &&
                                    Request.QueryString["from"].ToLower()
                                        .IndexOf(Constants.PageNames.MULTIPLE_VIEW_CSS.ToLower()) == -1 &&
                                    Request.QueryString["from"].ToLower()
                                        .IndexOf(Constants.PageNames.MULTIPLE_VIEW.ToLower()) == -1)
                                {
                                    Response.Redirect(Constants.PageNames.PHARMACY + "?from=" +
                                                      Constants.PageNames.SCRIPT_PAD.ToLower());
                                }
                            }
                            else
                            {
                                Response.Redirect(Constants.PageNames.PHARMACY + "?from=" +
                                                  Constants.PageNames.SCRIPT_PAD.ToLower());
                            }
                        }
                    }
                    else if (cameFrom == Constants.PageNames.SELECT_MEDICATION || cameFrom == Constants.PageNames.PHARMACY
                        || cameFrom == Constants.PageNames.SCRIPT_PAD || cameFrom == Constants.PageNames.MULTIPLE_VIEW_CSS)
                    {
                        PageState.Remove("PharmacyID");
                    }
                    else
                    {
                        PageState["PHARMACYID"] = GetPharmacyId(PageState);
                    }
                }
                else
                {
                    btnChangePharm.Visible = false;
                }

                if (PageState.GetStringOrEmpty("PATIENTSTATUS") == "99")
                {
                    //no changing of the pharmacy for test patient
                    btnChangePharm.Enabled = false;
                }

                if ((PageState.GetBooleanOrFalse(Constants.SessionVariables.IsCsRefReqWorkflow) == true || MasterPage.ChangeRxRequestedMedCs != null) && PageState[Constants.SessionVariables.TaskScriptMessageId] != null)
                {
                    //Load script msg
                    ScriptMessage scriptMsg = new ScriptMessage(Session[Constants.SessionVariables.TaskScriptMessageId].ToString(), Session["LICENSEID"].ToString(), Session["USERID"].ToString(), base.DBID);

                    ucMessageHeader.Icon = Controls_Message.MessageType.INFORMATION;
                    string pharmPrefix = ($"{(MasterPage.ChangeRxRequestedMedCs != null ? "Change Rx" : "Renewal Request")} Pharmacy : ");
                    ucMessageHeader.MessageText = pharmPrefix + Allscripts.Impact.Utilities.StringHelper.GetPharmacyName(
                            scriptMsg.PharmacyName,
                            scriptMsg.PharmacyAddress1,
                            scriptMsg.PharmacyAddress2,
                            scriptMsg.PharmacyCity,
                            scriptMsg.PharmacyState,
                            scriptMsg.PharmacyZip,
                            scriptMsg.PharmacyPhoneNumber);
                    ucMessageHeader.Visible = true;

                    ucMessageRxHeader.Icon = Controls_Message.MessageType.INFORMATION;
                    if(MasterPage.ChangeRxRequestedMedCs != null)
                    {
                        ucMessageRxHeader.MessageText = "Rx Detail : " + MasterPage.ChangeRxRequestedMedCs.RxDetails;
                    }
                    else
                    {
                        ucMessageRxHeader.MessageText =
                              "Rx Detail : " + Allscripts.Impact.Utilities.StringHelper.GetRxDetails(
                                  scriptMsg.DispensedRxDrugDescription,
                                  scriptMsg.DispensedRxSIGText,
                                  scriptMsg.DispensedRxQuantity,
                                  scriptMsg.DispensedDaysSupply,
                                  scriptMsg.DispensedRxRefills,
                                  scriptMsg.DispensedDaw,
                                  scriptMsg.DispensedCreated,
                                  scriptMsg.DispensedDateLastFill,
                                  scriptMsg.DispensedRxNotes);

                    }
                    ucMessageRxHeader.Visible = true;

                    //set up back button for workflow and remove edit patient/med/pharmacy
                    btnBack.Enabled = btnBack.Visible = true;
                    btnChangePharm.Enabled = btnChangePharm.Visible = false;
                    btnSelectMed.Visible = btnSelectMed.Enabled = false;
                    btnCancel.Visible = btnCancel.Enabled = false;
                }

                //
                // finally, bind the grid to display the scripts
                //
                loadRxGrid();

                if (Request.QueryString["from"] != null && Request.QueryString["from"].Equals(Constants.PageNames.PHARMACY, StringComparison.OrdinalIgnoreCase))
                {
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), "UpdatePharmacyInPatientHeader", "UpdatePharmacyInPatientHeader();", true);
                }

                logger.Debug("ScriptPad Page_Load. Session: {0}", Session.GetSessionContentString());
            }
        }

        protected void grdScriptPad_DataBinding(object sender, EventArgs e)
        {
            this.epcsEligibleMedList = null;
        }

        protected void grdScriptPad_OnDataBound(object sender, EventArgs e)
        {
            if (base.ScriptPadMeds.Count == 0)
            {
                if (!IsMismatch)
                    btnSelectMed_Click(this, new EventArgs());
            }

            if (base.ScriptPadMeds.Count == 1)
            {
                secureProcessingControl.MessageText = Controls_SecureProcessing.SecureProcessingMessages.SingleRx;
            }
            else if (base.ScriptPadMeds.Count > 1)
            {
                secureProcessingControl.MessageText = Controls_SecureProcessing.SecureProcessingMessages.MultipleRx;
            }

            grdScriptPad.Style.Remove("width");
            grdScriptPad.Style.Add("width", "97%");

            lblCSLegend.Visible = hasCSMeds & (epcsEligibleMedList.Count == 0);

            //// if CS med and EPCS stars align, display the EPCS right-side user control
            //ucEpcsSendToPharmacy.Visible = hasCSMeds & (epcsEligibleMedList.Count > 0);
            ucSpecialtyMedsUserWelcome.Visible = PageState.GetBooleanOrFalse("ShouldShowSpecMedsWelcomeMsg");


        }

        protected void grdScriptPad_OnItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                GridDataItem tempDataItem = (GridDataItem)e.Item;
                Rx script = (Rx)tempDataItem.DataItem;
                script.PatientID = base.SessionPatientID;
                Literal litRxDate = (Literal)tempDataItem.FindControl("litRxDate");

                if (litRxDate != null)
                {
                    litRxDate.Text = string.Concat(string.Format("{0:MM/dd/yyyy}", script.RxDateLocal), "<br />",
                        string.Format("{0:hh:mm tt}", script.RxDateLocal));
                }

                Literal medicationAndSig = (Literal)tempDataItem.FindControl("litMedicationAndSig");

                if (medicationAndSig != null)
                {
                    StringBuilder description = new StringBuilder();

                    description.Append(script.MedicationName);

                    if (!string.IsNullOrWhiteSpace(script.DDI))
                    {
                        description.Append(" ")
                            .Append(script.Strength)
                            .Append(" ")
                            .Append(script.StrengthUOM)
                            .Append(" ");
                        if (!string.IsNullOrWhiteSpace(script.RouteOfAdminDescription) &&
                            !string.Equals(script.RouteOfAdminDescription, Constants.PrescriptionCommonTerm.DoesNotApply,
                                StringComparison.OrdinalIgnoreCase))
                            description.Append(script.RouteOfAdminDescription.Trim()).Append(" ");

                        if (
                            !string.Equals(script.DosageFormCode, Constants.PrescriptionCommonTerm.MISC,
                                StringComparison.OrdinalIgnoreCase))
                            description.Append(script.DosageFormDescription).Append(" - ");
                    }
                    else
                    {
                        description.Append(" - ");
                    }

                    description.Append(script.SigText);

                    if (script.DAW)
                    {
                        description.Append(" - DAW");
                    }

                    string quantity = Allscripts.Impact.Utilities.StringHelper.TrimDecimal(script.Quantity);
                    string dispensedRefillText = "REFILL";

                    description.Append(" <br />QUANTITY ")
                    .Append(quantity)
                    .Append(" ")
                    .Append(script.PackageUOM)
                    .Append(" - ")
                    .Append(dispensedRefillText)
                    .Append(" ")
                    .Append(script.Refills.ToString());
                    description.Append(" - Days Supply - ").Append(script.DaysSupply.ToString());

                    if (!string.IsNullOrEmpty(script.Notes))
                    {
                        description.Append(" - Notes: ").Append(script.Notes);
                    }

                    if (script.IsBrandNameMed)
                    {
                        medicationAndSig.Text = string.Concat("<b>", description.ToString(), "</b>");
                    }
                    else
                    {
                        medicationAndSig.Text = description.ToString();
                    }
                }

                if (base.CanApplyFinancialOffers && script.IsCouponAvailable)
                // Checking the CanApplyFinancialOffers, if the site can have coupons
                {
                    Image benefitImg = (Image)tempDataItem.FindControl("BenefitImage");

                    if (benefitImg != null)
                    {
                        benefitImg.Visible = true;
                        benefitImg.Style["cursor"] = "pointer";
                        benefitImg.ImageUrl = "images/dollar.png";
                        benefitImg.ToolTip = "Patient is Eligible for Financial Savings.";
                    }
                }

                if (script.IsCopiedRx || ((PageState.GetBooleanOrFalse(Constants.SessionVariables.IsCsRefReqWorkflow) == true || MasterPage.ChangeRxRequestedMedCs != null) && PageState[Constants.SessionVariables.TaskScriptMessageId] != null))
                {
                    ImageButton btnCanCopy = (ImageButton)tempDataItem.FindControl("btnCopy");

                    if (btnCanCopy != null)
                    {
                        btnCanCopy.Visible = false;
                        btnCanCopy.Enabled = false;
                    }
                }

                //build the dropdownlist programmatically based on schedule, MOB availability, presence of assistant, etc.
                populateDestination(script, ref e);

                displayDateAlertIcon(script, tempDataItem);

                if (script.IsDirty)
                {
                    ucMessage.Visible = true;
                    ucMessage.Icon = Controls_Message.MessageType.INFORMATION;
                    ucMessage.MessageText =
                        "You have modified the original script and you have copied script associated with this prescription.";
                }


                Rx.DisplayEpaIcon(script, new Allscripts.Impact.Telerik(tempDataItem));

                //SpecMedUC.Visible = script.IsSpecialtyMed && PageState.GetBooleanOrFalse(Constants.SessionVariables.IsProviderEnrolledInSpecialtyMed);

                ImageButton imgBtnRemoveScript = (ImageButton)tempDataItem.FindControl("btnRemove");
                base.SetSingleClickImageButton(imgBtnRemoveScript);

            }
        }

        protected void Page_LoadComplete(object sender, EventArgs e)
        {
            int tasks = 0;

            ((PhysicianMasterPage)Master).toggleTabs("patient", tasks);

            if (PageState.GetStringOrEmpty("SSOMode") == "PatientLockDownMode")
            {
                ((PhysicianMasterPage)Master).hideTabs();
            }

            if (PageState["ScriptPadUserMessage"] != null)
            {
                ucRxCopyMessage.Visible = false;
                ScriptPadUserMessage scriptPadUserMessage = (ScriptPadUserMessage)PageState["ScriptPadUserMessage"];
                ucMessage.MessageText = scriptPadUserMessage.MessageText;
                ucMessage.Icon = scriptPadUserMessage.MessageType;
                ucMessage.Visible = true;
                PageState.Remove("ScriptPadUserMessage");
            }

            if (PageState["ScriptPadRxCopyUserMessage"] != null)
            {
                ucMessage.Visible = false;
                ScriptPadUserMessage scriptPadUserMessage =
                    (ScriptPadUserMessage)PageState["ScriptPadRxCopyUserMessage"];
                ucRxCopyMessage.MessageText = scriptPadUserMessage.MessageText;
                ucRxCopyMessage.Icon = scriptPadUserMessage.MessageType;
                ucRxCopyMessage.Visible = true;
                PageState.Remove("ScriptPadRxCopyUserMessage");
            }
        }

        protected void Page_PreRenderComplete(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {// show the Registry checkbox and check registry button if:
             //1) controlled substance exists on script AND 
             //2) either this site's state has ISCSREGISTRYCHECKREQ OR (Enterprise configured for ShowCSRegistry AND the site's state has a PMP URL)
                chkRegistryChecked.Visible = CsMedUtil.ShouldShowCsRegistryControls(hasCSMeds, SessionLicense.EnterpriseClient.ShowCSRegistry, PageState);

                btnCheckRegistry.Visible = chkRegistryChecked.Visible;
                bool chkRegistryVal = false;
                if (chkRegistryChecked.Visible && PDMP.GetStateRegistryCheckedState(PageState, out chkRegistryVal))
                {
                    chkRegistryChecked.Checked = chkRegistryVal;
                }
                if (!PageState.GetBooleanOrFalse("SHOW_SEND_TO_ADM"))
                {
                    grdScriptPad.Columns[3].Visible = false;
                }
            }
        }

        protected void btnReturn_Click(object sender, EventArgs e)
        {
            if (PageState[Constants.SessionVariables.IsCsRefReqWorkflow] != null ||
                MasterPage.ChangeRxRequestedMedCs != null)
            {
                ScriptPadUtil.RemoveAllRxFromScriptPad(PageState);
            }
            Response.Redirect(Constants.PageNames.SIG);
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            DefaultRedirect();
        }

        protected void btnSelectMed_Click(object sender, EventArgs e)
        {
            Response.Redirect(Constants.PageNames.REDIRECT_TO_ANGULAR + "?componentName=" + Constants.PageNames.SELECT_MEDICATION);
        }
        protected void btnChangePharm_Click(object sender, EventArgs e)
        {
            Response.Redirect(Constants.PageNames.PHARMACY + "?from=" + Constants.PageNames.SCRIPT_PAD.ToLower() +
                              "&FromPharmacy=True");
        }

        protected void btnProcess_Click(object sender, EventArgs e)
        {
            if (validateScriptPadDestinationSelection())
            {
                CheckForSecondaryWorkflow(this, epcsEligibleMedList.Count, ucEPCSDigitalSigning, ucPatMissingInfo,
                    PageState, grdScriptPad.Items, ucSpecialtyMedsUserWelcome);
            }
        }

        /// <summary>
        /// Checks for secondary workflow before saving scripts
        /// </summary>
        public void CheckForSecondaryWorkflow(IScriptPad iScriptPad, int epcsMedCount,
            IControls_EPCSDigitalSigning epcsControl, IControlsPatMissingInfo patControl, IStateContainer pageState,
            GridDataItemCollection scriptPadItems, IControls_SpecialtyMedsUserWelcome specMedsWelcome, bool isMissingPatientInformationEntered = false)
        {
            if (ShouldShowPatMissingInfo(pageState))
            {
                patControl.Show();
            }
            else if(epcsMedCount > 0 && iScriptPad.isEPCSDigitalSigningRequired())
            {
                if (!isMissingPatientInformationEntered)
                {
                    epcsControl.ShouldShowEpcsSignAndSendScreen();
                }
                //else, User now has to click the Process Script Pad button again - to avoid kludgy way to show 2 ascx controls after one another
            }
            else if ((scriptPadItems == null || iScriptPad.IsSendingSpecMedsTaskList(scriptPadItems)) &&
                pageState.GetBooleanOrFalse("ShouldShowSpecMedsWelcomeMsg"))
            {
                specMedsWelcome.Show();
            }

            else
            {
                iScriptPad.saveScripts();
            }
        }

        public static bool ShouldShowPatMissingInfo(IStateContainer session)
        {
            return IsMissingRequiredPatientInfo(session);
        }

        private static bool IsMissingRequiredPatientInfo(IStateContainer session)
        {
            return session.GetStringOrEmpty(Constants.SessionVariables.PatientAddress1).Trim() == string.Empty ||
                     session.GetStringOrEmpty(Constants.SessionVariables.PatientZip).Trim() == string.Empty ||
                     session.GetStringOrEmpty(Constants.SessionVariables.PatientState).Trim() == string.Empty ||
                     session.GetStringOrEmpty(Constants.SessionVariables.PatientCity).Trim() == string.Empty;
        }

        public bool IsSendingMedToMailOrder(GridDataItemCollection scriptPadItems)
        {
            foreach (GridDataItem gvr in scriptPadItems)
            {
                if (IsMailOrderDest(gvr.FindControl("ddlDest")))
                {
                    return true;
                }
            }

            return false;
        }

        public bool IsSendingToPharmacy(GridDataItemCollection scriptPadItems)
        {
            foreach (GridDataItem gvr in scriptPadItems)
            {
                if (IsPharmacyDest(gvr.FindControl("ddlDest")))
                {
                    return true;
                }
            }

            return false;
        }

        public bool IsSendingSpecMedsTaskList(GridDataItemCollection scriptPadItems)
        {
            foreach (GridDataItem gvr in scriptPadItems)
            {
                if (IsSpecMedDest(gvr.FindControl("ddlDest")))
                {
                    return true;
                }
            }

            return false;
        }


        public bool IsMailOrderDest(Control cntrl)
        {
            var ddlDest = cntrl as DropDownList;
            if (ddlDest != null &&
                (ddlDest.SelectedValue == "MOB" || ddlDest.SelectedValue == "DOCMOB"))
            {
                return true;
            }
            return false;
        }

        public bool IsPharmacyDest(Control cntrl)
        {
            var ddlDest = cntrl as DropDownList;
            if (ddlDest != null &&
                (ddlDest.SelectedValue == "PHARM"))
            {
                return true;
            }
            return false;
        }

        public bool IsSpecMedDest(Control cntrl)
        {
            var ddlDest = cntrl as DropDownList;
            if (ddlDest != null &&
                (ddlDest.SelectedValue == Patient.SEND_TO_SPECIALTYMED_TASKS_LIST ||
                ddlDest.SelectedValue.ToLower() == "specialtymed" ||
                ddlDest.SelectedValue.ToLower() == "patient access services"))
            {
                return true;
            }
            return false;
        }

        public string PharmacyAlertMessage(Rx rx, string scriptPadDestination, string providerNadean, ConnectionStringPointer dbId, IPharmacy pharmacy)
        {
            string rtrn = "";
            if (rx != null)
            {
                string sigText = rx.SigText;
                string currentPharmID = string.Empty;
                bool checkPharmVersion = false;

                if (scriptPadDestination == PharmacySigAlertConstants.SendToPharmacy 
                    || scriptPadDestination == PharmacySigAlertConstants.SendToPhysicianWithRetail)
                {
                    currentPharmID = rx.PharmacyID;
                    checkPharmVersion = true;
                }
                else if (scriptPadDestination == PharmacySigAlertConstants.MOPharm)
                {
                    currentPharmID = Session["MOB_PHARMACY_ID"].ToString();
                    checkPharmVersion = true;
                }

                if (checkPharmVersion)
                {
                    DataSet dsPharmacy = pharmacy.LoadPharmacyById(currentPharmID, dbId);

                    if (dsPharmacy.Tables[0]?.Rows.Count > 0)
                    {
                        if(!Allscripts.Impact.Pharmacy.Is6XPharmacy(dsPharmacy.Tables[0]?.Rows?[0]))
                        {
                            if (sigText.Length > Constants.PharmacySigAlertConstants.MaxSupportedSigLength)
                            {
                                rtrn = Constants.ErrorMessages.SigAlertOver140ToNot6XPharmacy;
                            }

                            if (!string.IsNullOrEmpty(providerNadean) && !rx.Notes.Contains(providerNadean) && !string.IsNullOrWhiteSpace(rx.ControlledSubstanceCode))
                            {
                                rtrn += " " + Constants.ErrorMessages.NADEANPrependOverflowInNotes;
                            }
                        }
                    }
                }
            }

            return rtrn;
        }
        private bool is6XPharmacy(IPharmacy pharmacy, string pharmacyId, ConnectionStringPointer dbId)
        {
            if (!string.IsNullOrWhiteSpace(pharmacyId))
            {
                DataSet dsPharmacy = pharmacy.LoadPharmacyById(pharmacyId, dbId);
                if (dsPharmacy.Tables[0]?.Rows.Count > 0)
                {
                    return Allscripts.Impact.Pharmacy.Is6XPharmacy(dsPharmacy.Tables[0]?.Rows?[0]);
                }
            }

            return false;
        }
        protected bool validateScriptPadDestinationSelection()
        {
            GridItemCollection gridRows = grdScriptPad.Items;
            foreach (GridDataItem gvr in gridRows)
            {
                DropDownList ddlDest = (DropDownList)gvr.FindControl("ddlDest");
                if (ddlDest != null)
                {
                    string scriptDestination = ddlDest.SelectedValue;
                    if (scriptDestination == Patient.EPA_BLANK)
                    {
                        ucMessage.MessageText = "Please select a destination for all scripts.";
                        ucMessage.Icon = Controls_Message.MessageType.ERROR;
                        ucMessage.Visible = true;
                        return false;
                    }

                    string rxID = gvr.GetDataKeyValue("RxId").ToString();
                    Rx rx = base.ScriptPadMeds.Find(r => r.RxID == rxID);

                    if (scriptDestination == Patient.PHARM &&
                        !string.IsNullOrWhiteSpace(rx.PharmacyID) &&
                        !ScriptPadUtil.IsRefillQuantConsistentWithStateCS(rx, new Allscripts.Impact.Pharmacy(), new Prescription(), base.DBID, out string stateCSCode, out string destinationState, out string refillText))
                    {
                        ucMessage.MessageText = rx.MedicationName + " is schedule " + stateCSCode + " in " + destinationState + ".  Refills must be " + refillText + ", please edit the prescription.";
                        ucMessage.Icon = Controls_Message.MessageType.ERROR;
                        ucMessage.Visible = true;
                        return false;
                    }

                    string providerNadean = Prescription.GetNadean(PageState.GetStringOrEmpty("USERID"), rx.DDI, DBID);
                    string pharmAlertMessage = PharmacyAlertMessage(rx, scriptDestination, providerNadean, base.DBID, new Allscripts.Impact.Pharmacy());
                    if (!string.IsNullOrWhiteSpace(pharmAlertMessage))
                    {
                        ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "ShowPharmacySigAlert", $"ShowPharmacySigAlert(\"" + pharmAlertMessage + "\");", true);
                        return false;
                    }

                }
            }
            return true;
        }

        protected void btnRemove_Click(object sender, EventArgs e)
        {
            // get the row of the script being removed from the script pad
            GridDataItem gvr = (GridDataItem)((WebControl)sender).Parent.Parent;

            // get the script ID
            string rxID = gvr.GetDataKeyValue("RxId").ToString();

            var medToRemove = ScriptPadMeds.Find(r => r.RxID == rxID);

            if (medToRemove != null)
            {

                // delete the script from the session variable
                for (int i = 0; i < base.ScriptPadMeds.Count; i++)
                {
                    if ((base.ScriptPadMeds[i] as Rx).RxID == rxID)
                    {
                        base.ScriptPadMeds.RemoveAt(i);
                        PPTPlus.RemoveScriptPadMedByRxID(PageState, new PptPlus(), Guid.Parse(rxID));
                        break;
                    }
                }

                // delete the script
                Prescription.Delete(rxID, base.DBID);

                
                if (base.MedsWithDURs.Contains(rxID))
                {
                    base.MedsWithDURs.Remove(rxID);
                }
            }

            Response.Redirect(Request.RawUrl);
        }

        private void setUserMessage(string messageText, Controls_Message.MessageType messageType, string sessionKey)
        {
            ScriptPadUserMessage scriptUserMessage = new ScriptPadUserMessage();
            scriptUserMessage.MessageText = messageText;
            scriptUserMessage.MessageType = Controls_Message.MessageType.SUCCESS;

            PageState[sessionKey] = scriptUserMessage;
        }

        protected void btnCopy_Click(object sender, EventArgs e)
        {
            // get the row of the script being removed from the script pad
            GridDataItem gvr = (GridDataItem)((WebControl)sender).Parent.Parent;

            // get the script ID
            string rxID = gvr.GetDataKeyValue("RxId").ToString();

            // Copy the Rx
            Rx copiedRx = new Rx();
            copiedRx.CopyScriptForMailOrder(rxID, SessionUserID, SessionLicenseID, base.DBID);
            copiedRx.CopyScriptMDDForMailOrder(rxID, base.DBID);

            if (base.MedsWithDURs.Contains(rxID))
            {
                base.MedsWithDURs.Add(copiedRx.RxID);
            }

            //Display the overlay
            ucRxCopy.RxCopy = copiedRx;
            ucRxCopy.RxIDBase = rxID;
            ucRxCopy.Show();
        }

        protected void btnEdit_Click(object sender, EventArgs e)
        {
            GridDataItem gvr = (GridDataItem)((WebControl)sender).Parent.Parent;
            string rxID = gvr.GetDataKeyValue("RxId").ToString();

            if (PageState.GetBooleanOrFalse("ISPROVIDER") || PageState.GetBooleanOrFalse("IsPA"))
            {
                Response.Redirect(Constants.PageNames.SIG + "?Mode=Edit&RxId=" + rxID);
            }
            else if (PageState.GetBooleanOrFalse("IsDelegateProvider") || PageState.GetBooleanOrFalse("IsPASupervised"))
            {
                Response.Redirect(Constants.PageNames.NURSE_SIG + "?Mode=Edit&RxId=" + rxID);
            }
        }

        protected void lnkPatientEducation_Click(object sender, EventArgs e)
        {
            string first = "1";
            string RXIDS = string.Empty;

            GridItemCollection gridRows = grdScriptPad.Items;

            //foreach (GridViewRow gvr in grdScriptPad.Rows)
            foreach (GridDataItem gvr in gridRows)
            {
                string rxID = gvr.GetDataKeyValue("RxId").ToString();
                if (first == "1")
                {
                    RXIDS = rxID;
                    first = "0";
                }
                else
                {
                    RXIDS = RXIDS + ";" + rxID;
                }
            }
            string url = Constants.PageNames.PDF + "?RXIDS=" + RXIDS.ToString();

            var pharmacyID = PageState.GetStringOrEmpty("PHARMACYID");
            if (pharmacyID != string.Empty)
            {
                url = url + "&PHARMACYID=" + pharmacyID;
            }

            string newwindow = "<script language = \"Javascript\" type=\"text/javascript\">window.open('" +
                               url.ToString() + "','','');</script>";
            Response.Write(newwindow);
        }

        private void UcPatMissingInfoOnComplete(PatientDemographicsEditEventArgs patDemoEventArgs)
        {
            if (patDemoEventArgs.Success)
            {
                CheckForSecondaryWorkflow(this, epcsEligibleMedList.Count, ucEPCSDigitalSigning, ucPatMissingInfo,
                    PageState, grdScriptPad.Items, ucSpecialtyMedsUserWelcome, true);
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


        private void OnSpecMedsWelcomeComplete(EventArgs EventArgs)
        {
            string queryUserID = string.Empty;
            if (!PageState.GetBooleanOrFalse("ISDELEGATEPROVIDER")) //do nothing for POB. we want provider to see the welcome message
            {
                queryUserID = SessionUserID;
                UpdateProviderSpecMedsWelcome(queryUserID); //so they dont see the welcome message next time
            }

            PageState["ShouldShowSpecMedsWelcomeMsg"] = false;
        }

        private void UpdateProviderSpecMedsWelcome(string queryUserId)
        {
            Allscripts.Impact.Provider.UpdateProviderSpecMedsWelcome(queryUserId, base.DBID);
        }

        private void ucEPCSDigitalSigning_OnDigitalSigningComplete(DigitalSigningEventArgs dsEventArgs)
        {
            if (dsEventArgs.Success)
            {
                _signedMeds = dsEventArgs.SignedMeds;
                _unsignedMeds = dsEventArgs.UnsignedMeds;

                saveScripts();
            }
            else
            {
                if (dsEventArgs.ForceLogout)
                {
                    //force the user to log out if they've entered invalid credentials 3 times in a row
                    Response.Redirect(Constants.PageNames.LOGOUT + "?authfailedforepcs=true");
                }
                else if (dsEventArgs.IsMismatch)
                {
                    //Remove the list of meds from script pad as they are already deleted.
                    IsMismatch = true;
                    setUserMessage("dsEventArgs.Message", Controls_Message.MessageType.ERROR, "ScriptPadUserMessage");
                    string reDirectUrl = string.Empty;
                    if (Request.RawUrl != null && Request.RawUrl.ToString() != string.Empty)
                    {
                        reDirectUrl = RedirectUserToRawUrl(Request.RawUrl.ToString());
                        if(!string.IsNullOrEmpty(reDirectUrl))
                        {
                            Response.Redirect(reDirectUrl);
                        }
                        else
                        {
                            SelectPatientComponentParameters selectPatientComponentParameters = new SelectPatientComponentParameters
                            {
                                PatientId = PageState.GetStringOrEmpty(Constants.SessionVariables.PatientId)
                            };
                            RedirectToSelectPatient(null, selectPatientComponentParameters);
                        }
                    }
                    else
                    {
                        SelectPatientComponentParameters selectPatientComponentParameters = new SelectPatientComponentParameters
                        {
                            PatientId = PageState.GetStringOrEmpty(Constants.SessionVariables.PatientId)
                        };
                        RedirectToSelectPatient(null, selectPatientComponentParameters);
                    }

                }
                else if (dsEventArgs.EpcsRightsRemoved || dsEventArgs.IsAddressCityMissing)
                {
                    // remove send to pharmacy and set destination to print
                    foreach (GridDataItem script in grdScriptPad.MasterTableView.Items)
                    {
                        string rxID = script.GetDataKeyValue("RxId").ToString();

                        if (epcsEligibleMedList.Contains(rxID))
                        {
                            DropDownList ddlDest = (DropDownList)script.FindControl("ddlDest");
                            if (ddlDest != null)
                            {
                                ListItem sendToPharmacy = ddlDest.Items.FindByValue("PHARM");
                                if (sendToPharmacy != null)
                                {
                                    ddlDest.Items.Remove(sendToPharmacy);
                                }

                                ListItem sendToMOB = ddlDest.Items.FindByValue("MOB");
                                if (sendToMOB != null)
                                {
                                    ddlDest.Items.Remove(sendToMOB);
                                }

                                ddlDest.SelectedValue = "PRINT";
                            }
                        }
                    }

                    if (dsEventArgs.EpcsRightsRemoved)
                    {
                        ucMessage.MessageText =
                            "You are currently not EPCS authorized.  Please print any scripts with CS medications.";
                        ucMessage.Icon = Controls_Message.MessageType.INFORMATION;
                        ucMessage.Visible = true;
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(dsEventArgs.Message))
                    {
                        ucMessage.MessageText =
                            "Controlled substance meds being sent electronically could not be digitally signed.  Please try again or print.";
                    }
                    else
                    {
                        ucMessage.MessageText = dsEventArgs.Message;
                    }

                    ucMessage.Icon = Controls_Message.MessageType.ERROR;
                    ucMessage.Visible = true;
                }
            }
        }

        public string RedirectUserToRawUrl(string rawUrl)
        {
            string[] strarry = rawUrl.Split('/');
            int len = strarry.Length;
            string targetUrl = strarry[len - 1];
            string UrlForRedirection = Constants.PageNames.UrlForRedirection(targetUrl);
            return UrlForRedirection;
        }

        private void ucRxCopy_OnRxCopyComplete(RxCopyEventArgs rxCopyEventArgs)
        {
            ucMessage.Visible = false;

            if (rxCopyEventArgs.IsSuccessful)
            {
                setUserMessage("Rx successfully copied.", Controls_Message.MessageType.SUCCESS,
                    "ScriptPadRxCopyUserMessage");
                string reDirectUrl = RedirectUserToRawUrl(Request.RawUrl.ToString());
                if (!string.IsNullOrEmpty(reDirectUrl))
                {
                    Response.Redirect(reDirectUrl);
                }
                else
                {
                    SelectPatientComponentParameters selectPatientComponentParameters = new SelectPatientComponentParameters
                    {
                        PatientId = PageState.GetStringOrEmpty(Constants.SessionVariables.PatientId)
                    };
                    RedirectToSelectPatient(null, selectPatientComponentParameters);
                }
            }
            else if (rxCopyEventArgs.IsCancel)
            {
                ucRxCopyMessage.Visible = true;
                ucRxCopyMessage.Icon = Controls_Message.MessageType.INFORMATION;
                ucRxCopyMessage.MessageText = "Rx copy cancelled.";
            }
            else
            {
                ucRxCopyMessage.Visible = true;
                ucRxCopyMessage.Icon = Controls_Message.MessageType.ERROR;
                ucRxCopyMessage.MessageText =
                    string.Concat("Rx copy failed. Exception Reference ID = " + rxCopyEventArgs.ExceptionReferenceID);
            }
        }

        protected void lbCloseEPASentNotice_Click(object sender, EventArgs e)
        {
            if (cbDoNotShowAgain.Checked)
            {
                Allscripts.ePrescribe.Data.UserProperties.SecUserAppPropertySave(SessionUserID,
                    Constants.UserPropertyNames.DONT_SHOW_EPA_POPUP, bool.TrueString, DBID);
                PageState["DontShowEpaPopup"] = true;
            }

            Response.Redirect(lbCloseEPASentNotice.CommandArgument);
        }

        #endregion

        #region Custom Methods

        private bool checkB64(string check)
        {
            byte[] toEncodeAsBytes = System.Text.ASCIIEncoding.ASCII.GetBytes(check);
            string b64Check = System.Convert.ToBase64String(toEncodeAsBytes);

            if (b64Check ==
                "PEI+VVAgVVAgRE9XTiBET1dOIExFRlQgUklHSFQgTEVGVCBSSUdIVCBCIEEgU0VMRUNUIFNUQVJUIC0gUExBWSBDT05UUkEgV0lUSCBVTkxJTUlURUQgTElWRVM=")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void checkPatientReceipt()
        {
            var sessionLicense = PageState.Cast<ApplicationLicense>("SessionLicense", null);
            if (sessionLicense != null)
            {
                string state = string.Empty;
                state = PageState.GetStringOrEmpty("PRACTICESTATE");
                if (state != "PR")
                {
                    ApplicationLicense license = sessionLicense;
                    if (license.LicenseDeluxeStatus == Constants.DeluxeFeatureStatus.On ||
                        license.LicenseDeluxeStatus == Constants.DeluxeFeatureStatus.AlwaysOn
                        || license.LicenseDeluxeStatus == Constants.DeluxeFeatureStatus.Cancelled)
                    {
                        if (PageState.GetBooleanOrFalse("PatientReceipt"))
                        {
                            ChkPatientReceipt.Visible = true;
                            ChkPatientReceipt.Checked = true;
                        }
                        else
                        {
                            ChkPatientReceipt.Visible = true;
                            ChkPatientReceipt.Checked = false;
                        }
                    }
                    else
                    {
                        ChkPatientReceipt.Visible = false;
                        divPatientReceipt.Attributes.Add("Style", "Display:none");
                    }
                }
                else // PR - State is out of scope
                {
                    ChkPatientReceipt.Visible = false;
                    divPatientReceipt.Attributes.Add("Style", "Display:none");
                }
            }
        }

        private void turnOffPatientReceipt()
        {
            ChkPatientReceipt.Visible = false;
            divPatientReceipt.Attributes.Add("Style", "Display:none");
        }

        /// <summary>
        /// check if any cs meds where eligible to be sent electronically and Send To Pharmacy/Send To Mail Order was selected
        /// </summary>
        /// <returns></returns>
        public bool isEPCSDigitalSigningRequired()
        {
            bool isEPCSDigitalSigningRequired = false;
            List<Rx> epcsMeds = new List<Rx>();

            //
            // loop through the grid looking for CS meds being sent to pharmacy/MOB
            //
            GridDataItemCollection gridRows = grdScriptPad.Items;

            foreach (GridDataItem gvrScript in gridRows)
            {
                string rxID = gvrScript.GetDataKeyValue("RxId").ToString();

                if (epcsEligibleMedList.Contains(rxID))
                {
                    DropDownList ddlDest = (DropDownList)gvrScript.FindControl("ddlDest");

                    if (ddlDest != null)
                    {
                        if (ddlDest.SelectedValue == "PHARM" || ddlDest.SelectedValue == "MOB")
                        {
                            isEPCSDigitalSigningRequired = true;

                            Rx rx = base.ScriptPadMeds.Find(r => r.RxID == rxID);

                            rx.Destination = ddlDest.SelectedValue;
                            rx.EffectiveDate = DateTime.Now.Date;

                            string reconciledControlledSubstanceCode = string.Empty;

                            if (ddlDest.SelectedValue == "PHARM")
                            {
                                if (rx.IsFreeFormMedControlSubstance)
                                {
                                    DataSet dsRxDetailCS = Prescription.GetCSCodeFromRxDetailCS(rx.RxID, base.DBID);
                                    if (dsRxDetailCS != null && dsRxDetailCS.Tables.Count > 0 &&
                                        dsRxDetailCS.Tables[0].Rows.Count > 0)
                                    {
                                        reconciledControlledSubstanceCode =
                                            dsRxDetailCS.Tables[0].Rows[0]["RxScheduleUsed"].ToString();
                                    }
                                }
                                else
                                {
                                    reconciledControlledSubstanceCode =
                                        Prescription.ReconcileControlledSubstanceCodes(rx.ControlledSubstanceCode,
                                            rx.StateControlledSubstanceCode, rx.StateCSCodeForPractice);
                                }
                            }
                            else
                            {
                                string stateCSCodeForMOPharmacy = string.Empty;
                                stateCSCodeForMOPharmacy = Prescription.GetStateControlledSubstanceCode(rx.DDI, null,
                                    PageState.GetStringOrEmpty("MOB_State"), base.DBID);

                                if (rx.IsFreeFormMedControlSubstance)
                                {
                                    DataSet dsRxDetailCS = Prescription.GetCSCodeFromRxDetailCS(rx.RxID, base.DBID);
                                    if (dsRxDetailCS != null && dsRxDetailCS.Tables.Count > 0 &&
                                        dsRxDetailCS.Tables[0].Rows.Count > 0)
                                    {
                                        reconciledControlledSubstanceCode =
                                            dsRxDetailCS.Tables[0].Rows[0]["RxScheduleUsed"].ToString();
                                    }
                                }
                                else
                                {
                                    reconciledControlledSubstanceCode =
                                        Prescription.ReconcileControlledSubstanceCodes(rx.ControlledSubstanceCode,
                                            stateCSCodeForMOPharmacy, rx.StateCSCodeForPractice);
                                }
                            }

                            rx.ScheduleUsed = int.Parse(reconciledControlledSubstanceCode);

                            if (reconciledControlledSubstanceCode == "2")
                            {
                                rx.CanEditEffectiveDate = true;
                            }

                            epcsMeds.Add(rx);
                        }
                    }
                }
            }

            if (isEPCSDigitalSigningRequired)
            {
                ucEPCSDigitalSigning.EPCSMEDList = epcsMeds;
            }

            return isEPCSDigitalSigningRequired;
        }

        private void loadRxGrid()
        {
            List<Rx> rxList = new List<Rx>();

            DataTable scripts = CHPatient.GetFullScriptPad(
                base.SessionPatientID,
                base.SessionLicenseID,
                base.SessionSiteID,
                base.SessionUserID,
                PageState.GetStringOrEmpty("PracticeState"),
                true,
                base.DBID);

            foreach (DataRow script in scripts.Rows)
            {
                Rx rx = new Rx(script);
                rxList.Add(rx);
            }
            PPTPlus.ClearScriptPadCandidates(PageState);
            PPTPlus.CopyExistingMedsToScriptPadMeds(PageState, rxList, new PptPlus());
            PPTPlus.GetAllScriptPadMedPPTInfo(PageState);
            PPTPlus.SetAllScriptPadMedCouponRequestInfo(PageState, new PptPlus());
            PPTPlus.SetAllScriptPadMedPriorAuthFlag(PageState, new PptPlus());
            hasPharmacyChanges = PPTPlus.CheckForPharmacyChange(PageState, DBID);
            ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "ShowAllScriptPadMedPPTSummary", $"ShowAllScriptPadMedPPTSummary();", true);
            // bind the grid and update ScriptPadMeds session variable too
            grdScriptPad.DataSource = base.ScriptPadMeds = rxList;
            grdScriptPad.DataBind();
        }

        private void populateDestination(Rx rx, ref GridItemEventArgs e)
        {
            GridDataItem tempDataItem = (GridDataItem)e.Item;
            ArrayList rxToProviderList = null;

            if (PageState["SUPER_POB_TO_PROVIDER"] == null)
            {
                rxToProviderList = new ArrayList();
            }
            else
            {
                rxToProviderList = (ArrayList)PageState["SUPER_POB_TO_PROVIDER"];
            }

            bool isDelegateProvider = PageState.GetBooleanOrFalse("IsDelegateProvider");
            bool isPrescriptionToProvider = ShouldPrescriptionBeSentToProvider(PageState.GetBooleanOrFalse("IsPrescriptionTOProvider"), rxToProviderList, rx.RxID);

            DropDownList ddlDestination = (DropDownList)tempDataItem.FindControl("ddlDest");
            var btnChangePptPharmacy = tempDataItem.FindControl("btnHiddenTriggerChangePptPharmacy");
            var btnUpdateCouponControl = tempDataItem.FindControl("btnHiddenTriggerUpdateCouponControl");
            ddlDestination.Attributes.Add("onchange", "destionationOptionChanged(" + btnChangePptPharmacy.ClientID + "," + btnUpdateCouponControl.ClientID + ")");

            Label lblCSMed = (Label)tempDataItem.FindControl("lblCSMed");


            //check both MediSpan and state-specific controlled substance values. If either is a CS, then this med is a CS.
            string reconciledControlledSubstanceCode =
                Prescription.ReconcileControlledSubstanceCodes(rx.ControlledSubstanceCode,
                    rx.StateControlledSubstanceCode, rx.StateCSCodeForPractice);

            bool isControlledSubstance = (!string.IsNullOrWhiteSpace(reconciledControlledSubstanceCode) &&
                                          reconciledControlledSubstanceCode.ToUpper() != "U" &&
                reconciledControlledSubstanceCode != "0") || rx.IsFreeFormMedControlSubstance;
            PageState["ISCONTROLSUBSTANCE"] = isControlledSubstance;
            if (isControlledSubstance)
            {
                hasCSMeds = true;
                Image img = (Image)tempDataItem.FindControl("imgCS_Alert");
                img.Visible = true;
                img.ImageUrl = "~/images/ControlSubstance_sm.gif";
                img.ToolTip = string.Empty;
            }

            if (rx.HasMDDinSigText())
            {
                Image img = (Image)tempDataItem.FindControl("imgMDD_Alert");
                img.Visible = true;
                img.ImageUrl = "~/images/MDD.gif";
                img.ToolTip = string.Empty;
            }
            //Declare variable for passing to the method for populating flags.

            bool isUserEligibleToTryEPCS = false;
            if (isDelegateProvider) //Test this condition for all types of user.
            {
                isUserEligibleToTryEPCS = canUserTryEPCS(base.DelegateProvider);
            }
            else
            {
                isUserEligibleToTryEPCS = base.CanTryEPCS;
            }

            string pharmacyID = GetRxPharmacyId(PageState, rx.RxID);
            String lastPharmacyID = PageState.GetString("LastPharmacyID", null);
            string patientHasMOBCoverage = PageState.GetString("PatientHasMOBCoverage", null);
            string mobNABP = PageState.GetString("MOB_NABP", null);
            string isMOBElectronicEnabled = PageState.GetString("MOB_IsElectronicEnabled", null);
            string isPharmacyEPCSEnabled = PageState.GetString("IsPharmacyEPCSEnabled", null);
            bool hasDUR = base.MedsWithDURs.Contains(rx.RxID);
            Constants.UserCategory userType = base.SessionUserType;
            bool showEPA = (base.SessionLicense.EnterpriseClient.ShowEPA && PageState.GetBooleanOrFalse(Constants.LicensePreferences.SHOW_EPA));
            bool IsCurrentMedSpecialtyMed = rx.IsSpecialtyMed;
            bool IsProviderEnrolledInSpecialtyMed = false;

            if (IsCurrentMedSpecialtyMed)
            {
                IsProviderEnrolledInSpecialtyMed = SpecialtyMed.IsProviderEnrolledInSpecialtyMed(PageState);

                if (IsProviderEnrolledInSpecialtyMed)
                {
                    string queryUserID = string.Empty;
                    if (PageState.GetBooleanOrFalse("ISDELEGATEPROVIDER"))
                    {
                        queryUserID = PageState.GetStringOrEmpty("DELEGATEPROVIDERID");
                    }
                    else
                    {
                        queryUserID = SessionUserID;
                    }
                    DataRow drSpecialtyMedProviderDataRow =
                        Allscripts.ePrescribe.Data.Provider.ProviderLoadByID(queryUserID, base.DBID);
                    PageState["ShouldShowSpecMedsWelcomeMsg"] = false;
                    if (drSpecialtyMedProviderDataRow["SpecMedsWelcomeChecked"] != null)
                    {
                        PageState["ShouldShowSpecMedsWelcomeMsg"] = (!(drSpecialtyMedProviderDataRow["SpecMedsWelcomeChecked"].Equals(true)));
                    }
                }
            }

            ConnectionStringPointer dbID = base.DBID;
            bool isSiteAdmin = base.SiteHasAdmin;
            string sessionSitePharmacyID = base.SessionSitePharmacyID;
            List<string> pharmacyEPCSAuthorizedSchdules = PageState.Cast<List<string>>(
                "PharmacyEPCSAuthorizedSchdules", null);
            List<string> mopharmacyEPCSAuthorizedSchdules =
                PageState.Cast<List<string>>("MOPharmacyEPCSAuthorizedSchdules", null);
            List<string> epcsMedList = epcsEligibleMedList;
            bool isMobEPCSenabled = base.IsMOPharmacyEPCSEnabled;
            string mobState = PageState.GetString("MOB_State", null);
            string freeformReconciledCSCode = string.Empty;
            bool isCSmedEPCSEnableRetail = false;
            string reconciledCSCodeRetail = string.Empty;
            bool isCSmedEPCSEnableMO = false;
            string stateCSCodeForMOPharmacy = string.Empty;
            string reconciledCSCodeMo = string.Empty;

            if (isControlledSubstance)
            {
                DataSet dsRxDetailCS = Prescription.GetCSCodeFromRxDetailCS(rx.RxID, dbID);
                if (dsRxDetailCS != null && dsRxDetailCS.Tables.Count > 0 && dsRxDetailCS.Tables[0].Rows.Count > 0)
                {
                    freeformReconciledCSCode = dsRxDetailCS.Tables[0].Rows[0]["RxScheduleUsed"].ToString();
                }

                if (rx.IsFreeFormMedControlSubstance)
                {
                    //reconciledCSCodeRetail = freeformReconciledCSCode;
                    isCSmedEPCSEnableRetail = Prescription.IsCSMedEPCSEligible(freeformReconciledCSCode,
                        rx.StateControlledSubstanceCode, rx.StateCSCodeForPractice);
                    if (isCSmedEPCSEnableRetail)
                    {
                        reconciledCSCodeRetail = Prescription.ReconcileControlledSubstanceCodes(
                            freeformReconciledCSCode, rx.StateControlledSubstanceCode, rx.StateCSCodeForPractice);
                    }
                }
                else
                {
                    isCSmedEPCSEnableRetail = Prescription.IsCSMedEPCSEligible(rx.ControlledSubstanceCode,
                        rx.StateControlledSubstanceCode, rx.StateCSCodeForPractice);
                    if (isCSmedEPCSEnableRetail)
                    {
                        reconciledCSCodeRetail =
                            Prescription.ReconcileControlledSubstanceCodes(rx.ControlledSubstanceCode,
                                rx.StateControlledSubstanceCode, rx.StateCSCodeForPractice);
                    }
                }


                if (mobState != null)
                {
                    stateCSCodeForMOPharmacy = Prescription.GetStateControlledSubstanceCode(rx.DDI, null,
                        mobState.ToString(), dbID);
                }

                if (rx.IsFreeFormMedControlSubstance)
                {
                    isCSmedEPCSEnableMO = Prescription.IsCSMedEPCSEligible(freeformReconciledCSCode,
                        stateCSCodeForMOPharmacy, rx.StateCSCodeForPractice);
                    if (isCSmedEPCSEnableMO)
                    {
                        reconciledCSCodeMo = Prescription.ReconcileControlledSubstanceCodes(freeformReconciledCSCode,
                            stateCSCodeForMOPharmacy, rx.StateCSCodeForPractice);
                    }
                }
                else
                {
                    isCSmedEPCSEnableMO = Prescription.IsCSMedEPCSEligible(rx.ControlledSubstanceCode,
                        stateCSCodeForMOPharmacy, rx.StateCSCodeForPractice);
                    if (isCSmedEPCSEnableMO)
                    {
                        reconciledCSCodeMo = Prescription.ReconcileControlledSubstanceCodes(rx.ControlledSubstanceCode,
                            stateCSCodeForMOPharmacy, rx.StateCSCodeForPractice);
                    }
                }
            }
            bool isVIPPatient = PageState.GetBooleanOrFalse("IsVIPPatient");
            bool isRestrictedUser = PageState.GetBooleanOrFalse("IsRestrictedUser");
            bool isRestrictedPatient = PageState.GetBooleanOrFalse("IsRestrictedPatient");
            bool isRxIdAssociatedWithActiveCoverage = ePA.IsRxIdAssociatedWithActiveCoverage(rx.RxID, base.DBID);
            bool isEAuthExcludedInfoSourcePayer = ePA.IsEAuthExcludedInfoSourcePayer(rx.CoverageID, base.DBID);
            bool isLicenseShieldEnabled = base.IsLicenseShieldEnabled;
            bool isRetailPharmacyAvailable = (pharmacyID != null && pharmacyID.ToString() != string.Empty);
            bool isLastPharmacyAvailable = (lastPharmacyID != null && lastPharmacyID.ToString() != string.Empty);
            bool isPatientEligibleForMailOrderBenefit = (patientHasMOBCoverage != null &&
                                                         patientHasMOBCoverage.ToString() == "Y");
            bool hasSessionSitePharmacyID = !string.IsNullOrEmpty(sessionSitePharmacyID);
            bool hasMobNABP = (mobNABP != null && mobNABP.ToString().Trim() != "X");
            bool isMobNABPX = (mobNABP != null && mobNABP.ToString().Trim() == "X");
            bool isRetailPharmacyEPCSEnabled = (isPharmacyEPCSEnabled != null &&
                                                isPharmacyEPCSEnabled.ToString() != string.Empty &&
                                                Convert.ToBoolean(isPharmacyEPCSEnabled.ToString()));
            bool isMoElectronicEnabled = isMOBElectronicEnabled != null && bool.Parse(isMOBElectronicEnabled.ToString());

            GetScriptPadOptionsParameters parameters = new GetScriptPadOptionsParameters();

            parameters.MailOrderPharmacyEPCSAuthorizedSchedules = mopharmacyEPCSAuthorizedSchdules;
            parameters.RetailPharmacyEPCSAuthorizedSchedules = pharmacyEPCSAuthorizedSchdules;
            parameters.SiteEPCSAuthorizedSchedules = base.SiteEPCSAuthorizedSchedules;
            parameters.UserSPI = PageState.GetStringOrEmpty("SPI");
            parameters.Status = rx.Status;
            parameters.ReconciledCSCodeForMailOrderPharmacy = reconciledCSCodeMo;
            parameters.ReconciledCSCodeForRetailPharmacy = reconciledCSCodeRetail;
            parameters.UserType = userType;
            parameters.ShowEPA = showEPA;
            parameters.IsProviderEnrolledInSpecialtyMed = IsProviderEnrolledInSpecialtyMed;
            parameters.IsSpecialtyMed = IsCurrentMedSpecialtyMed;
            parameters.HasDUR = hasDUR;
            parameters.HasMobNABP = hasMobNABP;
            parameters.IsMobNABPEqualsX = isMobNABPX;
            parameters.IsSiteAdmin = isSiteAdmin;
            parameters.HasSessionSitePharmacyID = hasSessionSitePharmacyID;
            parameters.IsCopiedRx = rx.IsCopiedRx;
            parameters.IsUserEPAEnabled = AppCode.StateUtils.UserInfo.IsPermissionGranted(UserPermissions.EPA, PageState);
            parameters.IsLicenseShieldEnabled = isLicenseShieldEnabled;
            parameters.IsPrescriptionToProvider = isPrescriptionToProvider;
            parameters.IsDelegateProvider = isDelegateProvider;
            parameters.IsPatientEligibleForMailOrderBenefit = isPatientEligibleForMailOrderBenefit;
            parameters.IsMailOrderPharmacyEPCSEnabled = isMobEPCSenabled;
            parameters.IsMailOrderPharmacyElectronicEnabled = isMoElectronicEnabled;
            parameters.IsRetailPharmacyEPCSEnabled = isRetailPharmacyEPCSEnabled;
            parameters.IsLastPharmacyAvailable = isLastPharmacyAvailable;
            parameters.IsRetailPharmacyAvailable = isRetailPharmacyAvailable;
            parameters.IsUserEligibleToTryEPCS = isUserEligibleToTryEPCS;
            parameters.IsRetailPharmacyElectronicEnabled = rx.IsPharmacyElectronicEnabled;
            parameters.IsPriorAuthRequired = rx.PriorAuthRequired || PPTPlus.IsPptPriorAuthRx(PageState, Guid.Parse(rx.RxID));
            parameters.IsCSMedEPCSEligibleForMailOrder = isCSmedEPCSEnableMO;
            parameters.IsCSMedEPCSEligibleForRetail = isCSmedEPCSEnableRetail;
            parameters.IsFreeFormMedControlledSubstance = rx.IsFreeFormMedControlSubstance;
            parameters.IsControlledSubstance = isControlledSubstance;
            parameters.IsRxIdAssociatedWithActiveCoverage = isRxIdAssociatedWithActiveCoverage;
            parameters.IsEAuthExcludedInfoSourcePayer = isEAuthExcludedInfoSourcePayer;
            parameters.isVIPPatient = isVIPPatient;
            parameters.isRestrictedUser = isRestrictedUser;
            parameters.isRestrictedPatient = isRestrictedPatient;
            parameters.HasExpiredDea = PageState.GetBooleanOrFalse(Constants.SessionVariables.HasExpiredDEA);
            parameters.CsTaskInfo = base.IsCsTaskWorkflow ? new CsTaskInfo(rx.PharmacyType) : null;

            ///Pass these flags to core engine to get all destination flags.
            ///And according to those flags create destination items.

            if (PPTPlus.IsPPTPlusPreferenceOn(PageState) && hasPharmacyChanges)
            {
                var responseContainer = PageState.Cast(Constants.SessionVariables.PptPlusResponses, default(PptPlusResponseContainer));
                Guid? defaultPharmacyID = responseContainer?.ScriptPadMeds?.FirstOrDefault(sm => sm.RxId.ToString() == rx.RxID).PharmacyID;

                if (defaultPharmacyID != Guid.Empty)
                {
                    parameters.IsLastPharmacyAvailable = true;
                    DataTable dtSchedules = PPTPlus.GetEPCSCheckAndAuthorizedSchedulesForPharmacy(defaultPharmacyID, new PptPlusData(), DBID);

                    parameters.IsRetailPharmacyEPCSEnabled = dtSchedules != null && dtSchedules.Rows.Count > 0 ? Convert.ToBoolean(dtSchedules.Rows[0]["EpcsEnabled"]) : false;

                    if (parameters.IsRetailPharmacyEPCSEnabled)
                    {
                        List<string> authorizedSchedules = new List<string>();

                        foreach (DataRow drSchedule in dtSchedules.Rows)
                        {
                            authorizedSchedules.Add(drSchedule["Schedule"].ToString());
                        }

                        parameters.RetailPharmacyEPCSAuthorizedSchedules = authorizedSchedules;
                    }
                }
            }

            GetScriptPadOptionsResponse response = Patient.GetScriptPadOptions(parameters);

            logger.Debug("GetScriptPadOption Request = {0}, With Response = {1}", parameters.ToLogString(),
                response.ToLogString());

            hasWarnings = response.HasDURWarning;

            if (!epcsEligibleMedList.Contains(rx.RxID) && response.AddRxIDToEPCSMedList)
            {
                epcsEligibleMedList.Add(rx.RxID);
            }

            if (response.HasDURWarning)
            {
                lblDUR.Visible = true;
                ((LiteralControl)tempDataItem.Cells[2].Controls[2]).Text += "<font color=red>*</font>";
            }

            if (!string.IsNullOrEmpty(response.InformationalText))
            {
                ucMessage.Visible = true;
                ucMessage.MessageText = response.InformationalText;
                ucMessage.Icon = Controls_Message.MessageType.INFORMATION;
            }

            string ePADestinationDefaultValue = string.Empty;
            if (response.ScriptPadOptions.ContainsValue("EPA"))
            {
                Constants.EPASponsoredStatus ePASponsoredStatus = Constants.EPASponsoredStatus.UNKNOWN;
                ePASponsoredStatus = ePA.GetPriorAuthSponsoredInfo(rx.CoverageID, rx.DDI, base.DBID);
                bool shouldToggleEpa = false;
                bool isPptPriorAuthOptionAvailable = PPTPlus.IsPptPriorAuthOptionAvailable(PageState, Guid.Parse(rx.RxID), out shouldToggleEpa);

                if ((ePASponsoredStatus == Constants.EPASponsoredStatus.NOT_SPONSORED) || isPptPriorAuthOptionAvailable)
                {
                    ddlDestination.Items.Add(new ListItem("Select...", Patient.EPA_BLANK));
                    ePADestinationDefaultValue = Patient.EPA_BLANK;
                }

                if (ePASponsoredStatus == Constants.EPASponsoredStatus.SPONSORED)
                {
                    ePADestinationDefaultValue = Patient.EPA;
                }

                if (isPptPriorAuthOptionAvailable)
                {
                    ePADestinationDefaultValue = shouldToggleEpa ? Patient.EPA : Patient.EPA_BLANK;
                }
            }

            foreach (KeyValuePair<string, string> kvp in response.ScriptPadOptions)
            {
                ddlDestination.Items.Add(new ListItem(kvp.Key, kvp.Value));
            }
            var canSendToSpecMed = Patient.IsSendToSpecialtyMedTasksListRequestOptionAvailable(parameters);
            if (canSendToSpecMed)
            {
                ddlDestination.SelectedValue = Patient.SPECIALTYMED;
            }

            if (!string.IsNullOrEmpty(ePADestinationDefaultValue))
            {
                ddlDestination.SelectedValue = ePADestinationDefaultValue;
            }

            Rx.SetSpecialtyMedAlertIcon(ddlDestination.SelectedValue, canSendToSpecMed, new Allscripts.Impact.Telerik(tempDataItem));

            setPptScriptDestination(rx, tempDataItem);
            SetToggleDestinationOptions(tempDataItem, ddlDestination, rx);
        }

        public bool ShouldPrescriptionBeSentToProvider(bool isAllReview, ArrayList rxToProviderList, string rxID)
        {
            bool isRxInSendToProviderList = false;

            if(rxToProviderList != null || !String.IsNullOrWhiteSpace(rxID))
            {
                Guid rxIDGuid;
                bool isParsingSuccessful =  Guid.TryParse(rxID, out rxIDGuid);
                isRxInSendToProviderList = isParsingSuccessful ? rxToProviderList.Contains(rxIDGuid) : false;
            }

            return isAllReview || isRxInSendToProviderList;
        }

        public void SetToggleDestinationOptions(GridDataItem tempDataItem, DropDownList ddlDestination, Rx rx)
        {
            Controls_PptPlusCoupon ctrlGoodRxCoupon = (Controls_PptPlusCoupon)tempDataItem.DetailTemplateItemDataCell.FindControl("ucPptPlusCoupon");
            Controls_eCoupon ctrlEcoupon = (Controls_eCoupon)tempDataItem.DetailTemplateItemDataCell.FindControl("ucEcoupon");
            var btnUpdateCouponControl = (HtmlInputHidden)tempDataItem.FindControl("btnHiddenTriggerUpdateCouponControl");

            //Set actual Client Control Id's here
            ctrlGoodRxCoupon.CheckBoxECouponClientId = ctrlEcoupon.chkOffersPharmacyNotes.ClientID;
            ctrlGoodRxCoupon.CheckBoxGoodRxClientId = ctrlGoodRxCoupon.chkSendOfferToPharmacy.ClientID;


            ctrlEcoupon.CheckBoxGoodRxClientId = ctrlGoodRxCoupon.chkSendOfferToPharmacy.ClientID;
            ctrlEcoupon.CheckBoxECouponClientId = ctrlEcoupon.chkOffersPharmacyNotes.ClientID;


            #region OnlyGoodRxCoupon
            if (PPTPlus.IsPptCouponAvailable(PageState, Guid.Parse(rx.RxID)) && !ctrlEcoupon.HasECoupon)
            {
                if (ddlDestination.SelectedValue != Constants.ScriptPadDestinations.PHARM.ToString() &&
                    ddlDestination.SelectedValue != Constants.ScriptPadDestinations.DOCPHARM.ToString())
                {
                    ctrlGoodRxCoupon.GoodRxCouponPharmacyNotesChecked = false;
                    ctrlGoodRxCoupon.GoodRxCouponPharmacyNotesEnabled = false;
                }
                else
                {
                    ctrlGoodRxCoupon.GoodRxCouponPharmacyNotesEnabled = true;
                }
                var ctlid = string.Empty;
                CheckBox chkbx = (CheckBox)ctrlGoodRxCoupon.FindControl("chkSendOfferToPharmacy");
                if (chkbx != null)
                {
                    ctlid = chkbx.ClientID;
                }

                btnUpdateCouponControl.Attributes.Add("onclick","ToggleSendToPharmacy("+ ddlDestination.ClientID + ",'" + ctlid + "', null);");
            }
            #endregion

            #region OnlyECoupon
            if (ctrlEcoupon.HasECoupon && !PPTPlus.IsPptCouponAvailable(PageState, Guid.Parse(rx.RxID)))
            {
                if (ddlDestination.SelectedValue == Constants.ScriptPadDestinations.PHARM.ToString() ||
                    ddlDestination.SelectedValue == Constants.ScriptPadDestinations.DOCPHARM.ToString())
                {
                    ctrlEcoupon.ECouponPharmacyNotesChecked = true;
                    ctrlEcoupon.ECouponPharmacyNotesEnabled = true;
                }
                else
                {
                    ctrlEcoupon.ECouponPharmacyNotesChecked = false;
                    ctrlEcoupon.ECouponPharmacyNotesEnabled = false;
                }

                var ctlid = string.Empty;
                CheckBox chkbx = (CheckBox)ctrlEcoupon.FindControl("chkOffersPharmacyNotes");
                if (chkbx != null)
                {
                    ctlid = chkbx.ClientID;
                    chkbx.Attributes.Add("onclick", "CheckECoupon_Clicked('', '" + ctlid + "')");
                }
                btnUpdateCouponControl.Attributes.Add("onclick", "ToggleSendToPharmacy(" + ddlDestination.ClientID + ", null, '" + ctlid + "');");
            }
            #endregion

            #region BothGoodRxAndECoupon
            if (PPTPlus.IsPptCouponAvailable(PageState, Guid.Parse(rx.RxID)) && ctrlEcoupon.HasECoupon)
            {
                if (ddlDestination.SelectedValue != Constants.ScriptPadDestinations.PHARM.ToString() &&
                    ddlDestination.SelectedValue != Constants.ScriptPadDestinations.DOCPHARM.ToString())
                {
                    ctrlGoodRxCoupon.GoodRxCouponPharmacyNotesChecked = false;
                    ctrlGoodRxCoupon.GoodRxCouponPharmacyNotesEnabled = false;


                    ctrlEcoupon.ECouponPharmacyNotesChecked = false;
                    ctrlEcoupon.ECouponPharmacyNotesEnabled = false;
                }
                else
                {
                    ctrlGoodRxCoupon.GoodRxCouponPharmacyNotesChecked = false;
                    ctrlGoodRxCoupon.GoodRxCouponPharmacyNotesEnabled = true;


                    ctrlEcoupon.ECouponPharmacyNotesChecked = true;
                    ctrlEcoupon.ECouponPharmacyNotesEnabled = true;
                }
                var ctlidchkSendOfferToPharmacy = string.Empty;
                CheckBox chkbxGoodRxCoupon = (CheckBox)ctrlGoodRxCoupon.FindControl("chkSendOfferToPharmacy");
                if (chkbxGoodRxCoupon != null)
                {
                    ctlidchkSendOfferToPharmacy = chkbxGoodRxCoupon.ClientID;
                }

                var ctlidchkOffersPharmacyNotes = string.Empty;
                CheckBox chkbxECoupon = (CheckBox)ctrlEcoupon.FindControl("chkOffersPharmacyNotes");
                if (chkbxECoupon != null)
                {
                    ctlidchkOffersPharmacyNotes = chkbxECoupon.ClientID;
                }
                btnUpdateCouponControl.Attributes.Add("onclick", "ToggleSendToPharmacy(" + ddlDestination.ClientID + ",'" + ctlidchkSendOfferToPharmacy + "','"+ctlidchkOffersPharmacyNotes+ "');");
                chkbxGoodRxCoupon.Attributes.Add("onclick", "CheckGoodRx_Clicked('" + ctlidchkSendOfferToPharmacy + "','" + ctlidchkOffersPharmacyNotes + "')");
                chkbxECoupon.Attributes.Add("onclick", "CheckECoupon_Clicked('" + ctlidchkSendOfferToPharmacy + "','" + ctlidchkOffersPharmacyNotes + "')");
            }
            #endregion
        }



        /// <summary>
        /// displays the Date Alert Icon in the data grid with tooltip text if the created date does not match the Rx header date
        /// </summary>
        /// <param name="rx">Rx</param>
        /// <param name="tempDataItem">Data Grid Row</param>
        private void displayDateAlertIcon(Rx rx, GridDataItem tempDataItem)
        {
            // compare local time(converted to utc) to rx header -- if dates are different, display warning image with tooltip
            if (!compareDate(rx.RxDateLocal.ToUniversalTime(), rx.RxHeaderCreatedDate))
            {
                Image img = (Image)tempDataItem.FindControl("imgDateAlert");
                img.Visible = true;
                img.ImageUrl = "~/images/warning-16x16.png";
                img.ToolTip = string.Format("Alert: This Rx was initially created on {0}.",
                    rx.RxHeaderCreatedDate.ToShortDateString());
            }
        }

        /// <summary>
        /// return true if dates match
        /// </summary>
        /// <param name="firstDate">first date to compare</param>
        /// <param name="secondDate">second date to compare</param>
        /// <returns></returns>
        private bool compareDate(DateTime firstDate, DateTime secondDate)
        {
            bool retval = false;

            if (firstDate.Date.Equals(secondDate.Date))
            {
                retval = true;
            }

            return retval;
        }
        public DXCPatientQueryResult GeneratePatientQueryResult(Rx rx, ISpecialtyMed iSpecialtyMedWorkflow, IDXCUtils dxcUtils)
        {
            DataSet patientDS = PatientCoverage.GetCoverageByPatientID(
                                                                PageState.GetStringOrEmpty(Constants.SessionVariables.PatientId),
                                                                PageState.GetStringOrEmpty(Constants.SessionVariables.LicenseId),
                                                                PageState.GetStringOrEmpty(Constants.SessionVariables.UserId),
                                                                base.DBID);
            DXCPatientQueryResult patientQueryResult = new DXCPatientQueryResult();
            //Medication
            iSpecialtyMedWorkflow.UpdateDXCPatientQueryRequestMedicationInformation(
                                                            rx,
                                                            Allscripts.ePrescribe.Data.Medication.SpecialtyMedsGetMedicationDetailsByRxID(rx.RxID, base.DBID),
                                                            Allscripts.ePrescribe.Data.Prescription.GetEnglishDescriptionFromSigID(rx.SigID, base.DBID),
                                                            ref patientQueryResult);
            //Insurance
            iSpecialtyMedWorkflow.UpdateDXCPatientQueryRequestInsuranceInformation(
                                                            patientDS,
                                                            ref patientQueryResult);
            //Diagnosis
            iSpecialtyMedWorkflow.UpdateDXCPatientQueryRequestDiagnosisInformation(
                                                                                    PageState.GetStringOrEmpty(Constants.SessionVariables.DIAGNOSIS),
                                                                                    Allscripts.Impact.Prescription.ChGetRXDetails(rx.RxID, base.DBID),
                                                                                    ref patientQueryResult
                                                                                    );


            return patientQueryResult;

        }
        public bool SendToSpecialtyMedTasksList(string rxID, Allscripts.Impact.Rx rx)
        {
            SpecialtyMed specialtyMedWorkflow = new SpecialtyMed();
            DXCUtils dxcUtils = new DXCUtils();

            //1. Make a DXC call to initiate processing
            ePrescribeSvc.InitiationResult processInitiationRequest = specialtyMedWorkflow.SendProcessInitiationRequest(SpecialtyMed.GenerateRecommendationContext(specialtyMedWorkflow, dxcUtils, PageState),
                                                                                        GeneratePatientQueryResult(rx, specialtyMedWorkflow, dxcUtils),
                                                                                        base.DBID);

            logger.Debug($"Initiation Request Sent = <Response>{processInitiationRequest.ToLogString()}</Response>");

            //2. Create RxTask, SpecMedTask and update status
            if (processInitiationRequest?.SubmissionSuccess == true)
            {
                //Create an Rx Task
                Int64 RxTaskIDForSpecialtyMedTask = specialtyMedWorkflow.SendToSpecialtyMedTasksList(rxID, base.SessionLicenseID, base.SessionUserID, base.DBID);

                logger.Debug($"SpecialtyMedTask Sent to Task List - RxId = {RxTaskIDForSpecialtyMedTask}");

                //Send to Specialty Med task list
                specialtyMedWorkflow.SaveInitiationResult(RxTaskIDForSpecialtyMedTask, processInitiationRequest, base.DBID);

                logger.Debug("Initiation result is saved.");

                //Update Status
                Allscripts.ePrescribe.Data.Prescription.UpdateStatus(
                                                   rxID,
                                                   1,
                                                   Convert.ToInt32(Constants.PrescriptionStatus.PENDING_TRANSMISSION),
                                                   PageState.GetStringOrEmpty(Constants.SessionVariables.UserId),
                                                   base.DBID);

                logger.Debug($"Specialty Med task <{rxID}> status updated to {Constants.PrescriptionStatus.PENDING_TRANSMISSION}");
                return true;
            }
            return false;


        }

        public void SavePptCouponInformation(Rx rx, GoodRxCouponPharmacyNotesModel goodRxCouponPharmacyNotesModel, IPptPlusData pptPlusData, ConnectionStringPointer dbId)
        {
            if (goodRxCouponPharmacyNotesModel != null)
            {
                pptPlusData.SavePptCouponInformation(rx.RxID, goodRxCouponPharmacyNotesModel.GroupNumber, 
                    goodRxCouponPharmacyNotesModel.BinValue, goodRxCouponPharmacyNotesModel.Pcn, 
                    goodRxCouponPharmacyNotesModel.MemberId, dbId);
            }
        }

        public void saveScripts()
        {
            ArrayList alProcess = new ArrayList();
            List<string> alPatientReceiptRXIDs = new List<string>();
            List<string> appliedCoupons = new List<string>();
            List<string> notAppliedCoupons = new List<string>();
            List<string> printAppliedCoupons = new List<string>();
            List<string> printNotAppliedCoupons = new List<string>();
            List<string> printOnlyCoupons = new List<string>();
            List<string> printRxInfo = new List<string>();
            List<string> allowPatientReceiptList = new List<string>();
            List<string> printGoodRxCoupons = new List<string>();
            List<string> printOnlyScriptsWithGoodRxCoupons = new List<string>();
            List<string> printOnlyScriptsWithOutGoodRxCoupons = new List<string>();
            List<string> sentToPharmacyGoodRxCoupon = new List<string>();
            List<string> sentToPharmacyCheckedGoodRxCoupon = new List<string>();

            var destCounts = new ScriptDestCounts();
            Dictionary<string, string> epaSentList = new Dictionary<string, string>();

            bool CanSendEDI = PageState["SPI"] != null;

            RadioButton rblQuickPrint = null;
            GridItem[] headerItems = grdScriptPad.MasterTableView.GetItems(GridItemType.Header);
            if (headerItems.Length > 0)
            {
                rblQuickPrint = (RadioButton)headerItems[0].FindControl("rblQuickPrint");
            }

            List<string> pes = new List<string>();

            base.ScriptPadMeds = null;
           

            //save userpreference for printing patient receipt
            bool bPrintPatientReceipt = true;
            if (ChkPatientReceipt.Visible)
            {
                ePrescribeSvc.GetUserResponse getUserRes = EPSBroker.GetRxUser(
                        ePrescribeSvc.ValueType.UserGUID,
                        base.SessionUserID,
                        base.SessionLicenseID,
                        base.SessionUserID,
                        base.SessionLicenseID,
                        base.DBID);

                if (getUserRes.RxUser != null)
                {
                    ePrescribeSvc.RxUser rxUser = getUserRes.RxUser;

                    RxUser.ChSaveUserProfile(rxUser.UserID, rxUser.Title, rxUser.Suffix, rxUser.NPI, string.Empty,
                        string.Empty, string.Empty,
                        string.Empty, string.Empty, rxUser.FirstName, rxUser.LastName, rxUser.MiddleName,
                        rxUser.Email, string.Empty,
                        rxUser.UserType, rxUser.SpecialtyCode1, rxUser.SpecialtyCode2, base.SessionLicenseID,
                        base.SessionUserID,
                        rxUser.GenericLicense, ChkPatientReceipt.Checked, base.DBID);
                }

                PageState["PatientReceipt"] = ChkPatientReceipt.Checked;
            }
            else
            {
                bPrintPatientReceipt = false;
            }

            PageState.Remove("RxInfoDetailsIdList");

            //spin through the rows and send or print as required
            GridItemCollection gridRows = grdScriptPad.Items;
            PageState.Remove(Constants.SessionVariables.SentToPharmacyGoodRxCoupon);
            PageState.Remove(Constants.SessionVariables.SentToPharmacyCheckedGoodRxCoupon);
            PageState.Remove("PrintGoodRxCoupon");

            foreach (GridDataItem gvr in gridRows)
            {
                string rxID = gvr.GetDataKeyValue("RxId").ToString();
                Rx rx = base.ScriptPadMeds.Find(r => r.RxID == rxID);

                if (rx != null)
                {
                    isSelfReport = false;
                    string scriptDestination = "PRINT";

                    DropDownList ddlDest = (DropDownList)gvr.FindControl("ddlDest");
                    if (ddlDest != null)
                    {
                        if (_unsignedMeds != null && _unsignedMeds.Contains(rxID))
                        {
                            scriptDestination = "PRINT";
                        }
                        else
                        {
                            scriptDestination = ddlDest.SelectedValue;
                        }
                    }

                    CheckBox cb = (CheckBox)gvr.FindControl("cbSendToADM");
                    if (cb != null && cb.Checked)
                    {
                        ScriptMessage.SendNotificationTask(rxID, base.SessionUserID, base.SessionLicenseID,
                            base.SessionPatientID, null, base.DBID);
                    }

                    Controls_eCoupon ctrlEcoupon =
                        (Controls_eCoupon)gvr.DetailTemplateItemDataCell.FindControl("ucEcoupon");
                    Controls_RxInfoList ctrlRxInfoList =
                        (Controls_RxInfoList)gvr.DetailTemplateItemDataCell.FindControl("ucRxInfoList");
                    Controls_PptPlusCoupon ctrlGoodRxCoupon =
                        (Controls_PptPlusCoupon)gvr.DetailTemplateItemDataCell.FindControl("ucPptPlusCoupon");

                    if (!Convert.ToBoolean(PageState[Constants.SessionVariables.IsPatientHealthPlanDisclosed]))
                    {
                        Prescription.AddNoDisclosurePharmacyNote(rxID, rx.LineNumber, DBID);
                    }

                    Constants.PPTPriorAuthStatus pptPriorAuthStatus = PPTPlus.GetPptPriorAuthRxStatus(PageState, Guid.Parse(rxID));
                    destCounts.RxProcessed++;
                    string surescriptsPptMessageId;
                    switch (scriptDestination)
                    {
                        case "PHARM":

                            #region [Send to Pharmacy]

                            surescriptsPptMessageId = PPTPlus.RetrieveSurescriptsResponseMessageIdOrEmpty(rx.RxID.ToGuidOr0x0(), PageState.Cast(Constants.SessionVariables.PptPlusResponses, default(PptPlusResponseContainer)), SessionUserID.ToGuidOr0x0(), SessionLicenseID.ToGuidOr0x0(), DBID, new Audit());
                            setSentRxECouponPrintItem(rx.RxID, ctrlEcoupon, ref appliedCoupons, ref notAppliedCoupons);
                            SetSentGoodRxCouponPrintItemForSendToPharmacy(rx.RxID, ctrlGoodRxCoupon, ref printGoodRxCoupons, ref rx, ref sentToPharmacyGoodRxCoupon, ref sentToPharmacyCheckedGoodRxCoupon);
                            addToListOnRxInfoPrintItemAvailable(ctrlRxInfoList, ref printRxInfo);
                            sendPrescription(rxID, "SENTTOPHARMACY", rx.Refills, rx.DaysSupply, rx.PharmacyID, surescriptsPptMessageId, rx.ControlledSubstanceCode);

                            allowPatientReceiptList.Add(rxID);

                            destCounts.RxSent++;

                            #endregion

                            break;
                        case "MOB":

                            #region [Send to Mail Order]

                            surescriptsPptMessageId = PPTPlus.RetrieveSurescriptsResponseMessageIdOrEmpty(rx.RxID.ToGuidOr0x0(), PageState.Cast(Constants.SessionVariables.PptPlusResponses, default(PptPlusResponseContainer)), SessionUserID.ToGuidOr0x0(), SessionLicenseID.ToGuidOr0x0(), DBID, new Audit());
                            //update status and send this message!
                            sendPrescriptionToMOB(rxID, "SENTTOPHARMACY", rx.Refills, rx.DaysSupply, surescriptsPptMessageId);
                            PPTPlus.RemoveScriptPadMedByRxID(PageState, new PptPlus(), Guid.Parse(rxID));
                            allowPatientReceiptList.Add(rxID);

                            destCounts.RxSent++;

                            #endregion

                            break;
                        case "PRINTANDSITEPHARM":

                            #region [Print and Site Pharmacy]

                            surescriptsPptMessageId = PPTPlus.RetrieveSurescriptsResponseMessageIdOrEmpty(rx.RxID.ToGuidOr0x0(), PageState.Cast(Constants.SessionVariables.PptPlusResponses, default(PptPlusResponseContainer)), SessionUserID.ToGuidOr0x0(), SessionLicenseID.ToGuidOr0x0(), DBID, new Audit());
                            setSentRxECouponPrintItem(rx.RxID, ctrlEcoupon, ref appliedCoupons, ref notAppliedCoupons);

                            addToListOnRxInfoPrintItemAvailable(ctrlRxInfoList, ref printRxInfo);
                            SetSentGoodRxCouponPrintItemForSendToPharmacy(rx.RxID, ctrlGoodRxCoupon, ref printGoodRxCoupons, ref rx, ref sentToPharmacyGoodRxCoupon, ref sentToPharmacyCheckedGoodRxCoupon);
                 
                            allowPatientReceiptList.Add(rxID);
                            
                            Prescription.UpdatePharmacyID(rxID, base.SessionSitePharmacyID, base.DBID);
                            alProcess.Add(rxID);
                          

                            destCounts.RxPrintAndSent++;

                            #endregion

                            break;
                        case "SENDSITEPHARM":

                            #region [Send to Site Pharmacy]

                            surescriptsPptMessageId = PPTPlus.RetrieveSurescriptsResponseMessageIdOrEmpty(rx.RxID.ToGuidOr0x0(), PageState.Cast(Constants.SessionVariables.PptPlusResponses, default(PptPlusResponseContainer)), SessionUserID.ToGuidOr0x0(), SessionLicenseID.ToGuidOr0x0(), DBID, new Audit());
                            setSentRxECouponPrintItem(rx.RxID, ctrlEcoupon, ref appliedCoupons, ref notAppliedCoupons);

                            addToListOnRxInfoPrintItemAvailable(ctrlRxInfoList, ref printRxInfo);

                            SetSentGoodRxCouponPrintItemForSendToPharmacy(rx.RxID, ctrlGoodRxCoupon, ref printGoodRxCoupons, ref rx, ref sentToPharmacyGoodRxCoupon, ref sentToPharmacyCheckedGoodRxCoupon);

                            sendPrescriptionToSitePharm(rxID, "SENTTOPHARMACY", rx.Refills, rx.DaysSupply, surescriptsPptMessageId);

                            allowPatientReceiptList.Add(rxID);

                            destCounts.RxSent++;

                            #endregion

                            break;
                        case "PRINT":

                            #region [Print]

                            setPrintRxECouponPrintItem(rx.RxID, ctrlEcoupon, ref printAppliedCoupons, ref printNotAppliedCoupons, ref printOnlyCoupons);

                            addToListOnRxInfoPrintItemAvailable(ctrlRxInfoList, ref printRxInfo);

                            allowPatientReceiptList.Add(rxID);

                         
                            alProcess.Add(rxID);
                            
                            setPrintGoodRxCouponItem(rx.RxID, ctrlGoodRxCoupon, ref printGoodRxCoupons, ref printOnlyScriptsWithGoodRxCoupons, ref printOnlyScriptsWithOutGoodRxCoupons);

                            Prescription.UpdatePharmacyID(rxID, Guid.Empty.ToString(), base.DBID);

                            destCounts.RxPrint++;

                            #endregion

                            break;
                        case "ASSISTANT":

                            #region [Send to Assistant]

                            bPrintPatientReceipt = false;

                            completeECouponItem(rx.RxID, ctrlEcoupon);

                            PPTPlus.RemoveScriptPadMedByRxID(PageState, new PptPlus(), Guid.Parse(rxID));

                            Prescription.SendToAssistant(rxID, base.SessionLicenseID, base.SessionUserID, base.DBID);
                            base.AuditLogPatientRxInsert(AuditAction.PATIENT_RX_CREATED, base.SessionPatientID,
                            rxID);

                            Prescription.UpdatePharmacyID(rxID, Guid.Empty.ToString(), base.DBID);

                            destCounts.AsstTasksSent++;

                            #endregion

                            break;
                        case "SPECIALTYMED":

                            #region [Send to SpecialtyMed Tasks List]

                            bPrintPatientReceipt = false;

                            PPTPlus.RemoveScriptPadMedByRxID(PageState, new PptPlus(), Guid.Parse(rxID));

                            Prescription.UpdatePharmacyID(rxID, Guid.Empty.ToString(), base.DBID);

                            var isInitiationRequestSuccess = SendToSpecialtyMedTasksList(rxID, rx);

                            if (isInitiationRequestSuccess)
                            {
                                destCounts.SpecialtyMedTaksInitiated++;
                            }
                            else
                            {
                                destCounts.SpecialtyMedTasksFailed++;
                            }

                            #endregion

                            break;
                        case "DOC":

                            #region [Send to Physician]

                            bPrintPatientReceipt = false;

                            completeECouponItem(rx.RxID, ctrlEcoupon);

                            PPTPlus.RemoveScriptPadMedByRxID(PageState, new PptPlus(), Guid.Parse(rxID));

                            Prescription.UpdatePharmacyID(rxID, Guid.Empty.ToString(), base.DBID);

                            string trycheck = string.Empty;
                            try
                            {
                                Literal medAndSig = gvr.FindControl("litMedicationAndSig") as Literal;
                                trycheck = medAndSig.Text.ToUpper().Substring(0, 92);
                            }
                            catch
                            {
                            }

                            if (checkB64(trycheck))
                            {
                                //
                                // Q: What is this? 
                                // A: Per Jason, don't delete this. Ever.
                                //
                                Prescription.Delete(rxID, base.DBID);

                                //now display
                                Response.Write("<html><body bgcolor=#000000 style=text-align:center; vertical-align:middle>");
                                Response.Write("<div id=\"hiddenflash\">\n");
                                Response.Write(
                                    "<object classid=\"clsid:d27cdb6e-ae6d-11cf-96b8-444553540000\" codebase=\"http://fpdownload.macromedia.com/pub/shockwave/cabs/flash/swflash.cab#version=9,0,0,0\" width=\"720\" height=\"540\" id=\"final_text2013\" align=\"middle\" quality=\"high\" allowFullScreen=\"true\" wmode=\"transparent\" allowScriptAccess=\"sameDomain\">\n");
                                Response.Write("<param name=\"allowScriptAccess\" value=\"sameDomain\" />\n");
                                Response.Write("<param name=\"movie\" value=\"images/final_text2013.swf\" />\n");
                                Response.Write("<param name=\"quality\" value=\"high\" />\n");
                                Response.Write("<param name=\"allowFullScreen\" value=\"true\" />\n");
                                Response.Write("<param name=\"wmode\" value=\"transparent\" />\n");
                                Response.Write("<param name=\"bgcolor\" value=\"#000000\" />\n");
                                Response.Write(
                                    "<embed src=\"images/final_text2013.swf\" quality=\"high\" bgcolor=\"#000000\" width=\"100%\" height=\"100%\" name=\"final_text2013\" align=\"middle\" allowScriptAccess=\"sameDomain\" type=\"application/x-shockwave-flash\" pluginspage=\"http://www.macromedia.com/go/getflashplayer\" />\n");
                                Response.Write("</object>\n");
                                Response.Write("</div>");
                                Response.Write("<div><a href=\"SelectPatient\">Done</a></div>");
                                Response.Write("</body></html>");
                                Response.End();
                                return;
                            }
                            else
                            {
                                Prescription.SendToPhysicianForApproval(rxID, base.SessionLicenseID, base.SessionUserID,
                                    PageState.GetStringOrEmpty("DelegateProviderID"), base.DBID);

                                Prescription.UpdatePharmacyID(rxID, Guid.Empty.ToString(), base.DBID);
                            }
                            destCounts.RxToDoc++;

                            #endregion

                            break;
                        case "DOCPHARM":

                            #region [Send to Phys. w/Retail]

                            bPrintPatientReceipt = false;

                            completeECouponItem(rx.RxID, ctrlEcoupon);
                            if (ctrlGoodRxCoupon != null)
                            {
                                GoodRxCouponPharmacyNotesModel goodRxCouponPharmacyNotesModel = PPTPlus.RetrieveGoodRxPharmacyNotesModel(PageState, new PptPlus(), rx.RxID.ToGuid());
                                string goodRxPharmacyNotes = RetrieveGoodRxPharmacyNotes(ctrlGoodRxCoupon.GoodRxCouponPharmacyNotesChecked, ctrlGoodRxCoupon.GoodRxCouponPharmacyNotesEnabled, goodRxCouponPharmacyNotesModel);
                                bool canGoodRxCouponPharmacyNotesBeSuccessfullyAppended = CanGoodRxCouponPharmacyNotesBeSuccessfullyAppended(rx.Notes, goodRxPharmacyNotes);
                                if (canGoodRxCouponPharmacyNotesBeSuccessfullyAppended)
                                {
                                    AppendGoodRxDataToPharmacyNotes(goodRxPharmacyNotes, ref rx);
                                }
                            }

                            var pharmacyId = GetRxPharmacyId(PageState, rxID);
                            if (PPTPlus.CheckForPharmacyChange(PageState, DBID))
                            {
                                Prescription.UpdatePharmacyID(rxID, pharmacyId, false, false, true, DBID);
                            }
                            else
                            {
                                Prescription.UpdatePharmacyID(rxID, pharmacyId, base.DBID);
                            }

                            PPTPlus.RemoveScriptPadMedByRxID(PageState, new PptPlus(), Guid.Parse(rxID));

                            Prescription.SendToPhysicianForApproval(rxID, base.SessionLicenseID, base.SessionUserID,
                                PageState.GetStringOrEmpty("DelegateProviderID"), base.DBID);

                            destCounts.RxToDoc++;

                            #endregion

                            break;
                        case "DOCMOB":

                            #region [Send to Phys. w/Mail Order]

                            bPrintPatientReceipt = false;

                            PPTPlus.RemoveScriptPadMedByRxID(PageState, new PptPlus(), Guid.Parse(rxID));

                            sendPrescriptionToDOCMOB(rxID);

                            destCounts.RxToDoc++;

                            #endregion

                            break;
                        case "EPA":

                            #region [Send to EPA]

                            bPrintPatientReceipt = false;

                            PPTPlus.RemoveScriptPadMedByRxID(PageState, new PptPlus(), Guid.Parse(rxID));
                            Prescription.UpdatePharmacyID(rxID, Guid.Empty.ToString(), base.DBID);
                            var ePAPhysicianID = GetPhysicianId(PageState, SessionUserID);
                            var ePAPharmacyID = GetPharmacyId(PageState);


                            SaveEpaWithPrescribing(rxID, ePAPharmacyID, ePAPhysicianID);

                            Literal litSig = (Literal)gvr.FindControl("litMedicationAndSig");
                            if (litSig != null)
                            {
                                epaSentList.Add(rxID, litSig.Text.Replace("<b>", string.Empty).Replace("</b>", string.Empty));
                            }

                            destCounts.EpaTaksInitiated++;

                            #endregion

                            break;
                        case "PTSELFREPORTED":

                            #region [Patient Reported]

                            bPrintPatientReceipt = false;
                            isSelfReport = true;

                            PPTPlus.RemoveScriptPadMedByRxID(PageState, new PptPlus(), Guid.Parse(rxID));
                            Prescription.UpdatePharmacyID(rxID, Guid.Empty.ToString(), base.DBID);

                            Prescription.SetAsSelfReported(rxID, base.SessionUserID, base.DBID);

                            destCounts.SelfRep++;

                            #endregion

                            break;
                        default:
                            bPrintPatientReceipt = false;
                            break;
                    } // end of switch (ddlDest.SelectedValue)

                    //Added logic to add the Rx_Detail_CS rows regardless of the source.
                    saveCSDetails(rxID);

                    pes.Add(rxID);

                    // print patient receipt on a browser
                    if (PageState.GetBooleanOrFalse("PatientReceipt") && bPrintPatientReceipt)
                    {
                        alPatientReceiptRXIDs.Add(rxID);
                    }

                    if ((base.ShowEPA) && (scriptDestination != "SPECIALTYMED"))
                    {
                        Constants.ScriptPadDestinations scriptPadDesinations = Constants.ScriptPadDestinations.UNKNOWN;
                        Enum.TryParse(scriptDestination, true, out scriptPadDesinations);
                        ePA.LogEAuthDetailOnRx(rxID, scriptPadDesinations,
                            pptPriorAuthStatus, base.DBID);
                    }
                }
            } // end for loop

            var successMsg = ScriptDestCounts.BuildSuccessMsg(destCounts);

            PageState["CameFrom"] = Constants.PageNames.SCRIPT_PAD.ToLower();

            string postProcess = string.Empty;

            postProcess = Constants.PageNames.START_OVER.ToLower() + "?Msg=" + Server.UrlEncode(successMsg);

            // There are scirpts that with a Destination of Print and Print Center is not enabled or selected
            if (alProcess != null && alProcess.Count > 0)
            {
                // save list of RxIDs that need to be printed
                PageState["ProcessList"] = alProcess;

                // if there are any coupons that have been applied for the scripts that have been selected for printing, save for processing after printing
                if (appliedCoupons.Count > 0 || notAppliedCoupons.Count > 0
                    || printAppliedCoupons.Count > 0 || printNotAppliedCoupons.Count > 0 || printOnlyCoupons.Count > 0)
                {
                    PageState["AppliedCoupons"] = appliedCoupons;
                    PageState["NotAppliedCoupons"] = notAppliedCoupons;
                    PageState["PrintAppliedCoupons"] = printAppliedCoupons;
                    PageState["PrintNotAppliedCoupons"] = printNotAppliedCoupons;
                    PageState["PrintOnlyCoupons"] = printOnlyCoupons;
                }
                if (printGoodRxCoupons.Count > 0)
                {
                    PageState["PrintGoodRxCoupon"] = printGoodRxCoupons;
                }
                if (printOnlyScriptsWithGoodRxCoupons.Count > 0)
                {
                    PageState["PrintOnlyScriptsWithGoodRxCoupons"] = printOnlyScriptsWithGoodRxCoupons;
                }
                if (printOnlyScriptsWithOutGoodRxCoupons.Count > 0)
                {
                    PageState["PrintOnlyScriptsWithOutGoodRxCoupons"] = printOnlyScriptsWithOutGoodRxCoupons;
                }

                if (sentToPharmacyGoodRxCoupon.Count > 0)
                {
                    PageState[Constants.SessionVariables.SentToPharmacyGoodRxCoupon] = sentToPharmacyGoodRxCoupon;
                }

                if (sentToPharmacyCheckedGoodRxCoupon.Count > 0)
                {
                    PageState[Constants.SessionVariables.SentToPharmacyCheckedGoodRxCoupon] = sentToPharmacyCheckedGoodRxCoupon;
                }

                if (alPatientReceiptRXIDs.Count > 0)
                {
                    PageState["PatientReceiptList"] = alPatientReceiptRXIDs;
                }

                if (printRxInfo.Count > 0)
                {
                    PageState["RxInfoDetailsIdList"] = printRxInfo;
                }

                if (CanAllowPatientReceipts)
                {
                    if (allowPatientReceiptList.Count > 0)
                    {
                        PageState["AllowPatientReceiptsList"] = allowPatientReceiptList;
                    }
                }
                // redirect to printing pages
                postProcess = Constants.PageNames.CSS_DETECT + "?PrintScript=YES&From=" +
                              Constants.PageNames.SCRIPT_PAD.ToLower() + "?Msg=" + Server.UrlEncode(successMsg);

            }
            else
            {
                //
                // If print center is enabled/selected then nothing should have been added to appliedCoupons, a confirmation task
                // will already have been created
                // 
                // these coupons are for meds that have been sent electronically
                if (appliedCoupons.Count > 0 || notAppliedCoupons.Count > 0
                    || printAppliedCoupons.Count > 0 || printNotAppliedCoupons.Count > 0 || printOnlyCoupons.Count > 0)
                {
                    PageState["AppliedCoupons"] = appliedCoupons;
                    PageState["NotAppliedCoupons"] = notAppliedCoupons;
                    PageState["PrintAppliedCoupons"] = printAppliedCoupons;
                    PageState["PrintNotAppliedCoupons"] = printNotAppliedCoupons;
                    PageState["PrintOnlyCoupons"] = printOnlyCoupons;
                    postProcess = Constants.PageNames.CSS_DETECT + "?PrintScript=YES&From=" +
                                  Constants.PageNames.SCRIPT_PAD.ToLower() + "?Msg=" + Server.UrlEncode(successMsg);
                }

                if (printGoodRxCoupons.Count > 0)
                {
                    PageState["PrintGoodRxCoupon"] = printGoodRxCoupons;
                    postProcess = Constants.PageNames.CSS_DETECT + "?From=" + Constants.PageNames.SCRIPT_PAD + "?Msg=" +
                                    Server.UrlEncode(successMsg);
                }
                if (sentToPharmacyGoodRxCoupon.Count > 0)
                {
                    PageState[Constants.SessionVariables.SentToPharmacyGoodRxCoupon] = sentToPharmacyGoodRxCoupon;
                }

                if (sentToPharmacyCheckedGoodRxCoupon.Count > 0)
                {
                    PageState[Constants.SessionVariables.SentToPharmacyCheckedGoodRxCoupon] = sentToPharmacyCheckedGoodRxCoupon;
                }

                if (printRxInfo.Count > 0)
                {
                    PageState["RxInfoDetailsIdList"] = printRxInfo;
                    postProcess = Constants.PageNames.CSS_DETECT + "?From=" + Constants.PageNames.SCRIPT_PAD + "?Msg=" +
                                  Server.UrlEncode(successMsg);
                }

                if (PageState.GetBooleanOrFalse("PatientReceipt") && alPatientReceiptRXIDs.Count > 0)
                {
                    PageState["PatientReceiptList"] = alPatientReceiptRXIDs;
                    postProcess = Constants.PageNames.CSS_DETECT + "?From=" + Constants.PageNames.SCRIPT_PAD + "?Msg=" +
                                  Server.UrlEncode(successMsg);
                }
                if (CanAllowPatientReceipts)
                {
                    if (allowPatientReceiptList.Count > 0)
                    {
                        PageState["AllowPatientReceiptsList"] = allowPatientReceiptList;
                        postProcess = Constants.PageNames.CSS_DETECT + "?From=" + Constants.PageNames.SCRIPT_PAD +
                                      "?Msg=" + Server.UrlEncode(successMsg);
                    }
                }

            }

            if ((Session[Constants.SessionVariables.IsCsRefReqWorkflow] != null || MasterPage.ChangeRxRequestedMedCs != null) && Session[Constants.SessionVariables.TaskScriptMessageId] != null) //If CS refreq gets new RX, update task to be complete and remove session flag
            {
                string providerID = Session["UserID"].ToString();

                if (!string.IsNullOrEmpty(providerID))
                {
                    ScriptMessage.updateRxTask(Session[Constants.SessionVariables.TaskScriptMessageId].ToString(), string.Empty, Constants.PrescriptionStatus.COMPLETE, providerID, base.DBID);
                    Session.Remove(Constants.SessionVariables.IsCsRefReqWorkflow);
                    PageState.Remove(Constants.SessionVariables.ChangeRxRequestedMedCs);
                    Response.Redirect(Constants.PageNames.APPROVE_REFILL_TASK);
                }
            }
            var shouldShowOverlay = !UserProperties.GetBoolOrFalseUserPropertyValue(Constants.UserPropertyNames.DONT_SHOW_EPA_POPUP, PageState, new UserProperties());
            if ((epaSentList.Count > 0) && shouldShowOverlay && !PageState.GetBooleanOrFalse("DontShowEpaPopup"))
            {
                rptEPASentList.DataSource = epaSentList;
                rptEPASentList.DataBind();

                lbCloseEPASentNotice.CommandArgument = postProcess;

                mpeEPASentList.Show();
            }
            else
            {
                Response.Redirect(postProcess);
            }
        }
        public bool CanGoodRxCouponPharmacyNotesBeSuccessfullyAppended(string rxNotes, string goodRxPharmacyNotes)
        {
            return !string.IsNullOrEmpty(goodRxPharmacyNotes) && ((rxNotes + goodRxPharmacyNotes).Length < Constants.PHARMCY_NOTES_MAX_LENGTH) ;
        }
        public string RetrieveGoodRxPharmacyNotes(bool goodRxCouponPharmacyNotesChecked, bool goodRxCouponPharmacyNotesEnabled, GoodRxCouponPharmacyNotesModel goodRxCouponPharmacyNotesModel)
        {
            string pharmNotesGoodRxContents = string.Empty;
            if (goodRxCouponPharmacyNotesChecked && goodRxCouponPharmacyNotesEnabled)
            {
                if (goodRxCouponPharmacyNotesModel != null)
                {
                    pharmNotesGoodRxContents = string.Format("PATIENT CHOSEN PRICE OUTSIDE OF INSURANCE: ${0}; ADJUDICATE WITH: BIN {1} PCN {2} GRP {3} MBR {4}",
                      goodRxCouponPharmacyNotesModel.CouponPrice,
                      goodRxCouponPharmacyNotesModel.BinValue,
                      goodRxCouponPharmacyNotesModel.Pcn,
                      goodRxCouponPharmacyNotesModel.GroupNumber,
                      goodRxCouponPharmacyNotesModel.MemberId
                      );
                }
            }
            return pharmNotesGoodRxContents;
        }
        public void AppendGoodRxDataToPharmacyNotes(string pharmNotesGoodRxContents, ref Rx rx)
        {
            rx.Notes += pharmNotesGoodRxContents;
            Prescription.AddSendOfferToGoodRxPharmacyToPharmNotes(pharmNotesGoodRxContents, rx.RxID, rx.LineNumber, DBID);
        }
        private void setSentRxECouponPrintItem(string rxID, Controls_eCoupon ctrlEcoupon, ref List<string> appliedCoupons, ref List<string> notAppliedCoupons)
        {
            if (base.CanApplyFinancialOffers)
            {
                if ((ctrlEcoupon != null) && (ctrlEcoupon.HasECoupon))
                {
                    bool isPharmNoteUpdated = false;

                    if (ctrlEcoupon.ECouponPharmacyNotesChecked)
                    {
                        // Add to pharmacy note
                        isPharmNoteUpdated = EPSBroker.AreECouponNotesToRxPharmacyNotesUpdated(rxID,
                            ctrlEcoupon.PharmacyNotes, base.SessionLicenseID, base.SessionUserID, base.DBID);
                    }
                    else
                    {
                        Allscripts.ePrescribe.Data.Prescription.SaveRxHeaderOptionalAttribute(Constants.RxHeaderAttributeNames.EcouponSendToPharmUnchecked, rxID.ToGuidOr0x0(), "1", DBID);
                    }

                    if (ctrlEcoupon.ECouponPrintOptionChecked)
                    {

                        if (isPharmNoteUpdated)
                        {
                            // save ECouponDetailID for printing
                            appliedCoupons.Add(ctrlEcoupon.ECouponDetailID); //PharmacyNotePrinted
                        }
                        else
                        {
                            notAppliedCoupons.Add(ctrlEcoupon.ECouponDetailID); //Printed
                        }
          
                    }
                    else  // not printing
                    {
                        if (isPharmNoteUpdated)
                        {
                            EPSBroker.ConfirmECouponDelivery(ctrlEcoupon.ECouponDetailID, Constants.ECouponOfferDeliveryType.PHARMACYNOTE, base.SessionLicenseID, base.SessionUserID, base.DBID);
                        }
                    }
                }
            }
        }
        private void SetSentGoodRxCouponPrintItemForSendToPharmacy(string rxID, 
            Controls_PptPlusCoupon ctrlGoodRxCoupon, 
            ref List<string> printGoodRxCoupons, 
            ref Rx rx,
            ref List<string> sentToPharmacyGoodRxCoupon,
            ref List<string> sentToPharmacyCheckedGoodRxCoupon)
        {
            if (PPTPlus.IsPPTPlusPreferenceOn(PageState))
            {
                bool isGoodRxCouponSentToPharmacy = false;
                bool canGoodRxCouponPharmacyNotesBeSuccessfullyAppended = false;
                if ((ctrlGoodRxCoupon != null) && ctrlGoodRxCoupon.IsPPTCouponAvailable)
                {
                    if (ctrlGoodRxCoupon.SendOfferToPharmacyChecked)
                    {
                        sentToPharmacyCheckedGoodRxCoupon.Add(rxID);

                        GoodRxCouponPharmacyNotesModel goodRxCouponPharmacyNotesModel = PPTPlus.RetrieveGoodRxPharmacyNotesModel(PageState, new PptPlus(), rx.RxID.ToGuid());
                        SavePptCouponInformation(rx, goodRxCouponPharmacyNotesModel, new PptPlusData(), base.DBID);

                        string goodRxPharmacyNotes = RetrieveGoodRxPharmacyNotes(ctrlGoodRxCoupon.GoodRxCouponPharmacyNotesChecked, 
                            ctrlGoodRxCoupon.GoodRxCouponPharmacyNotesEnabled, 
                            goodRxCouponPharmacyNotesModel);
                        
                        canGoodRxCouponPharmacyNotesBeSuccessfullyAppended = CanGoodRxCouponPharmacyNotesBeSuccessfullyAppended(rx.Notes, goodRxPharmacyNotes);
                        if (canGoodRxCouponPharmacyNotesBeSuccessfullyAppended)
                        {
                            AppendGoodRxDataToPharmacyNotes(goodRxPharmacyNotes, ref rx);
                            
                        }
                        if (canGoodRxCouponPharmacyNotesBeSuccessfullyAppended 
                            || ((!string.IsNullOrWhiteSpace(goodRxPharmacyNotes)) 
                                && is6XPharmacy(new Allscripts.Impact.Pharmacy(), rx.PharmacyID, base.DBID))) // info is sent in COO segment of script message.
                        {
                            sentToPharmacyGoodRxCoupon.Add(rxID);
                            isGoodRxCouponSentToPharmacy = true;
                        }
                    }

                    if (ctrlGoodRxCoupon.CouponPrintOptionChecked )
                    {
                        printGoodRxCoupons.Add(rxID);
                    }
                    else
                    {
                        bool isSentToPharmacyCheckedGoodRxCoupon = sentToPharmacyCheckedGoodRxCoupon.Contains(rxID);
                        PPTPlus.SendUsageReport(PageState, new PptPlus(), Guid.Parse(rxID), false,
                            isGoodRxCouponSentToPharmacy, new PptPlusServiceBroker(), new PptPlusData()
                            , new CommonComponentData());
                        PPTPlus.AuditCouponStatus(PageState, Guid.Parse(rxID), true, false,
                            isSentToPharmacyCheckedGoodRxCoupon, isGoodRxCouponSentToPharmacy, new PptPlusData());
                        PPTPlus.RemoveScriptPadMedByRxID(PageState, new PptPlus(), Guid.Parse(rxID));
                    }
                }
                else
                {
                    PPTPlus.SendUsageReport(PageState, new PptPlus(), Guid.Parse(rxID), false,
                        isGoodRxCouponSentToPharmacy, new PptPlusServiceBroker(), new PptPlusData()
                        , new CommonComponentData());
                    PPTPlus.RemoveScriptPadMedByRxID(PageState, new PptPlus(), Guid.Parse(rxID));
                }
            }

        }
        
        private void setPrintGoodRxCouponItem(string rxID, Controls_PptPlusCoupon ctrlGoodRxCoupon, ref List<string> printGoodRxCoupons,
                                             ref List<string> printOnlyScriptsWithGoodRxCoupons, ref List<string> printOnlyScriptsWithOutGoodRxCoupons)
        {
            if (PPTPlus.IsPPTPlusPreferenceOn(PageState))
            {
                if ((ctrlGoodRxCoupon != null) && ctrlGoodRxCoupon.IsPPTCouponAvailable)
                {
                    if (ctrlGoodRxCoupon.CouponPrintOptionChecked /*|| ctrlGoodRxCoupon.SendOfferToPharmacyChecked*/)
                    {
                        printGoodRxCoupons.Add(rxID);
                    }
                    else
                    {
                        printOnlyScriptsWithGoodRxCoupons.Add(rxID);
                    }
                }
                else
                {
                    printOnlyScriptsWithOutGoodRxCoupons.Add(rxID);
                }
            }

        }

        private void setPrintRxECouponPrintItem(string rxID, Controls_eCoupon ctrlEcoupon, ref List<string> printAppliedCoupons, ref List<string> printNotAppliedCoupons, ref List<string> printOnlyCoupons)
        {
            if (base.CanApplyFinancialOffers)
            {
                if ((ctrlEcoupon != null) && (ctrlEcoupon.HasECoupon))
                {
                    if (ctrlEcoupon.ECouponPrintOptionChecked)
                    {
                        // Add to pharmacy note
                        var isUpdated = EPSBroker.AreECouponNotesToRxPharmacyNotesUpdated(rxID, ctrlEcoupon.PharmacyNotes, base.SessionLicenseID, base.SessionUserID, base.DBID);

                        if (isUpdated)
                        {
                            // save ECouponDetailID for printing
                            printAppliedCoupons.Add(ctrlEcoupon.ECouponDetailID); //PharmacyNotePrinted
                        }
                        else
                        {
                            printNotAppliedCoupons.Add(ctrlEcoupon.ECouponDetailID); //Printed
                        }

                    }
                }
            }
        }

        private void completeECouponItem(string rxID, Controls_eCoupon ctrlEcoupon)
        {
            if (base.CanApplyFinancialOffers)
            {
                if ((ctrlEcoupon != null) && (ctrlEcoupon.HasECoupon))
                {
                    if (ctrlEcoupon.ECouponPharmacyNotesChecked)
                    {
                        // add to pharmacy note
                        var isUpdated = EPSBroker.AreECouponNotesToRxPharmacyNotesUpdated(rxID, ctrlEcoupon.PharmacyNotes, base.SessionLicenseID, base.SessionUserID, base.DBID);
                        if (isUpdated)
                        {
                            // confirm it to DXC as PharmacyNotes
                            EPSBroker.ConfirmECouponDelivery(ctrlEcoupon.ECouponDetailID, Constants.ECouponOfferDeliveryType.PHARMACYNOTE, base.SessionLicenseID, base.SessionUserID, base.DBID);
                        }
                    }
                }
            }
        }

        private void addToListOnRxInfoPrintItemAvailable(Controls_RxInfoList ctrlRxInfoList,
            ref List<string> printRxInfo)
        {
            if (base.IsShowRxInfo)
            {
                if (ctrlRxInfoList != null)
                {
                    printRxInfo.AddRange(ctrlRxInfoList.RxInfoPrintList);
                }
            }
        }

        private void SaveEpaWithPrescribing(string rxID, string ePAPharmacyID, string ePAPhysicianID)
        {
            string ePATaskID = EPSBroker.EPA.SendToEPATaskList(base.SessionLicenseID, base.SessionPatientID, rxID,
                                                               ePAPharmacyID,
                                                               Constants.ePATaskType.PRIOR_AUTH_INITIATED,
                                                               Constants.ePATaskStatus.EPA_REQUESTED, ePAPhysicianID,
                                                               base.SessionUserID, Constants.ePANCPDPTaskType.NCPDP,
                                                               Constants.EPAType.PROSPECTIVE_EPA,
                                                               base.DBID);
            string ePARequestID = EPSBroker.EPA.SendToEPARequests(ePATaskID, Constants.ePATransPriority.NOT_URGENT,
                                                                  Constants.ePARequestType.EPAINITIATION_REQUEST,
                                                                  base.SessionUserID, base.DBID);
            EPSBroker.EPA.SaveEPAInitiationRequest(base.SessionLicenseID, base.SessionUserID, ePATaskID, ePARequestID,
                                                   base.NcpdpEpaUserShieldSecurityToken, base.DBID);
        }

        internal static string GetPharmacyId(IStateContainer state)
        {
            string pharmacyID = null;
            var patientPharmacyID = state.GetStringOrEmpty("PHARMACYID");
            var patientLastPharmacyID = state.GetStringOrEmpty("LASTPHARMACYID");

            if (patientPharmacyID.Equals(string.Empty))
            {
                if (patientLastPharmacyID != string.Empty)
                {
                    pharmacyID = patientLastPharmacyID;
                }
            }
            else
            {
                pharmacyID = patientPharmacyID;
            }
            return pharmacyID;
        }

        internal static string GetPhysicianId(IStateContainer state, string sessionUserId)
        {
            string ePAPhysicianID = null;
            var delegateProviderID = state.GetStringOrEmpty("DelegateProviderID");
            if (delegateProviderID != string.Empty)
            {
                if (state.GetBooleanOrFalse("IsPASupervised"))
                {
                    ePAPhysicianID = sessionUserId;
                }
                else
                {
                    ePAPhysicianID = delegateProviderID;
                }
            }
            else
            {
                ePAPhysicianID = sessionUserId;
            }
            return ePAPhysicianID;
        }

        private string GetRxPharmacyId(IStateContainer state, string rxId)
        {
            Rx rx = base.ScriptPadMeds.Find(r => r.RxID == rxId);

            if (rx != null)
            {
                if (string.IsNullOrWhiteSpace(rx.PharmacyID)
                    || rx.PharmacyID.Trim().Equals(Guid.Empty))
                {
                    return GetPharmacyId(state);
                }
                else
                {//Make sure that a pharmacy and the appropriate session vars are in place for building script ddl
                    if (base.IsCsTaskWorkflow)
                    {
                        AppCode.StateUtils.PatientInfo.SetPharmInfo(PageState, rx.PharmacyID, base.DBID);
                    }
                    return rx.PharmacyID;
                }

            }
            return string.Empty;
        }
        private void sendPrescription(string rxID, string rxStatus, int refills, int daysSupply, string RXpharmacyID,
        string surescriptsPptMessageId, string csCode)
        {
            var pharmacyID = GetRxPharmacyId(PageState, rxID);

            Prescription.MarkAsFulfilled(base.SessionLicenseID, base.SessionUserID, rxID, refills, daysSupply, rxStatus,
            pharmacyID, false, false, null, true, base.DBID);

            string smid = string.Empty;
            bool sendSignedCSMed = false;
            bool messageSent = false;

            if (_signedMeds != null && _signedMeds.ContainsKey(rxID))
            {
                sendSignedCSMed = true;
                smid = _signedMeds[rxID];
            }
            else
            {
                if (PageState.GetBooleanOrFalse(Constants.SessionVariables.IsCsRefReqWorkflow))
                {
                    var scriptMessage = new ScriptMessage(PageState.GetStringOrEmpty(Constants.SessionVariables.TaskScriptMessageId), base.SessionLicenseID, base.SessionUserID, base.DBID);
                    bool isCSMed = !string.IsNullOrWhiteSpace(CurrentRx.ControlledSubstanceCode);
                    AMEMOMMessageInfo messageInfo = new AMEMOMMessageInfo
                    {
                        sm = scriptMessage,
                        script = CurrentRx,
                        type = "R",
                        codequal = string.Empty,
                        userID = base.SessionUserID,
                        prescriberOrderNumber = rxID,
                        shieldSecurityToken = base.ShieldSecurityToken,
                        deaNumberUsed = null,
                        effectiveDate = null,
                        siteID = base.SessionSiteID,
                        dbID = base.DBID,
                        isEPCSMed = isCSMed ? IsEPCSMed.YES : IsEPCSMed.NO,
                        rxDate = CurrentRx.RxDateLocal.ToString()
                    };
                    smid = ScriptMessage.CreateREFRESMessageInAMEMOMFormat(messageInfo);

                    //CS -> NonCS refill task
                    if (!isCSMed)
                    {
                        PageState["REFILLMSG"] = "Rx Refill for " + base.CurrentRx.MedicationName + " approved for " + Session["PATIENTNAME"].ToString().Trim() + ".";
                    }
                }
                else if (MasterPage.ChangeRxRequestedMedCs != null)
                {
                    // create a new Approve with Change msg for CS renewal 
                    var sm = new ScriptMessage(PageState.GetStringOrEmpty(Constants.SessionVariables.TaskScriptMessageId), SessionLicenseID, SessionUserID, DBID);
                    Guid delegateProviderId = base.DelegateProvider != null
                        ? base.DelegateProvider.UserID.ToGuidOr0x0()
                        : Guid.Empty;
                    var rxTask = new RxTaskModel
                    {
                        ScriptMessage = sm,
                        ScriptMessageGUID = PageState.GetStringOrEmpty(Constants.SessionVariables.TaskScriptMessageId),
                        Rx = CurrentRx,
                        UserId = SessionUserID.ToGuid(),
                        DbId = DBID,
                        LicenseId = SessionLicenseID.ToGuid(),
                        SiteId = SessionSiteID,
                        ShieldSecurityToken = ShieldSecurityToken,
                        ExternalFacilityCd = PageState.GetStringOrEmpty(Constants.SessionVariables.ExternalFacilityCd),
                        ExternalGroupID = PageState.GetStringOrEmpty(Constants.SessionVariables.ExternalGroupID),
                        UserType = SessionUserType,
                        DelegateProviderId = delegateProviderId
                    };
                    rxTask.RxRequestType = Allscripts.Impact.RequestType.APPROVE_WITH_CHANGE;
                    smid = ScriptMessage.ApproveRXCHGMessage(rxTask, base.SessionUserID, Constants.PrescriptionTransmissionMethod.SENT, true);
                    messageSent = true;
                    PageState["REFILLMSG"] = "Rx Change for " + base.CurrentRx.MedicationName + " approved for " + Session["PATIENTNAME"].ToString().Trim() + ".";

                }
                else
                {
                    smid = ScriptMessage.CreateScriptMessageWithOriginalTransactionId(rxID.ToGuidOr0x0(), surescriptsPptMessageId, 1, Constants.MessageTypes.NEWRX, SessionLicenseID.ToGuidOr0x0(),
                        SessionUserID.ToGuidOr0x0(), ShieldSecurityToken, SessionSiteID, DBID);
                }
            }

            string auditLogPatientID = string.Empty;
            if (!sendSignedCSMed)
            {
                ePrescribeSvc.AuditLogPatientRxResponse rxResponse =
                    base.AuditLogPatientRxInsert(ePrescribeSvc.AuditAction.PATIENT_RX_CREATED, base.SessionPatientID,
                        rxID);
                if (rxResponse.Success)
                {
                    auditLogPatientID = rxResponse.AuditLogPatientID;
                }
            }
            else
            {
                //Get the inserted AuditLogPatientId from session or somewhere.
                if (ucEPCSDigitalSigning != null)
                {
                    if (ucEPCSDigitalSigning.AuditLogPatientID.ContainsKey(rxID))
                        auditLogPatientID = ucEPCSDigitalSigning.AuditLogPatientID[rxID];
                }
            }

            long serviceTaskID = -1;
            if (!string.IsNullOrEmpty(smid) && PageState.GetStringOrEmpty("STANDING") == "1" && !messageSent)
            {
                if (sendSignedCSMed)
                {
                    serviceTaskID = ScriptMessage.SendThisEPCSMessage(smid, base.SessionLicenseID, base.SessionUserID,
                        base.DBID);
                }
                else if (string.IsNullOrWhiteSpace(csCode))
                {
                    serviceTaskID = ScriptMessage.SendThisMessage(smid, base.SessionLicenseID, base.SessionUserID, base.DBID);
                }
            }

            ///Check here if it is not -1 then insert it in newly created table. you also have auditLogPatientID.
            ///This will be used from service manager and added last audit log when message is sent to hub.
            if (serviceTaskID != -1 && !string.IsNullOrEmpty(auditLogPatientID))
            {
                Audit.InsertAuditLogPatientServiceTask(serviceTaskID, auditLogPatientID, base.DBID);
            }

            smid = ScriptMessage.CreateDUREVTScriptMessage(rxID, 1, base.SessionLicenseID, base.SessionUserID,
                string.Empty, base.DBID);

            if ((PageState.GetStringOrEmpty("STANDING") == "1") && (!string.IsNullOrEmpty(smid)))
            {
                ScriptMessage.SendOutboundInfoScriptMessage(smid, base.SessionLicenseID, base.SessionUserID, base.DBID);
            }
        }

        private void sendPrescriptionToSitePharm(string rxID, string rxStatus, int refills, int daysSupply, string surescriptsPptMessageId)
        {
            base.AuditLogPatientRxInsert(ePrescribeSvc.AuditAction.PATIENT_RX_CREATED, base.SessionPatientID, rxID);

            Prescription.MarkAsFulfilled(base.SessionLicenseID, base.SessionUserID, rxID, refills, daysSupply, rxStatus,
                base.SessionSitePharmacyID, false, true, null, false, base.DBID);


            string smid = ScriptMessage.CreateScriptMessageWithOriginalTransactionId(rxID.ToGuidOr0x0(), surescriptsPptMessageId, 1, Constants.MessageTypes.NEWRX, SessionLicenseID.ToGuidOr0x0(),
                SessionUserID.ToGuidOr0x0(), ShieldSecurityToken, SessionSiteID, DBID);

            if (!string.IsNullOrEmpty(smid) && PageState.GetStringOrEmpty("STANDING") == "1")
            {
                ScriptMessage.SendThisMessage(smid, base.SessionLicenseID, base.SessionUserID, base.DBID);
            }

            smid = ScriptMessage.CreateDUREVTScriptMessage(rxID, 1, base.SessionLicenseID, base.SessionUserID,
                string.Empty, base.DBID);

            if ((PageState.GetStringOrEmpty("STANDING") == "1") && (!string.IsNullOrEmpty(smid)))
            {
                ScriptMessage.SendOutboundInfoScriptMessage(smid, base.SessionLicenseID, base.SessionUserID, base.DBID);
            }
        }

        private void sendPrescriptionToMOB(string rxID, string rxStatus, int refills, int daysSupply,
            string surescriptsPptMessageId)
        {
            string mopharmID = PageState.GetStringOrEmpty("MOB_PHARMACY_ID");

            if (string.IsNullOrEmpty(mopharmID))
            {
                DataSet mobDS = Allscripts.Impact.Pharmacy.LoadPharmacyByNABP(PageState.GetStringOrEmpty("MOB_NABP"),
                    base.DBID);

                if (mobDS.Tables[0].Rows.Count > 0)
                {
                    mopharmID = mobDS.Tables[0].Rows[0]["PharmacyID"].ToString();
                }
            }

            if (!string.IsNullOrEmpty(mopharmID))
            {
                Prescription.MarkAsFulfilled(base.SessionLicenseID, base.SessionUserID, rxID, refills, daysSupply,
                    rxStatus, mopharmID, true, false, null, false, base.DBID);

                string smid = string.Empty;
                bool sendSignedCSMed = false;

                if (_signedMeds != null && _signedMeds.ContainsKey(rxID))
                {
                    sendSignedCSMed = true;
                    smid = _signedMeds[rxID];
                }
                else
                {
                    smid = ScriptMessage.CreateScriptMessageWithOriginalTransactionId(rxID.ToGuidOr0x0(), surescriptsPptMessageId, 1, Constants.MessageTypes.NEWRX, SessionLicenseID.ToGuidOr0x0(),
                        SessionUserID.ToGuidOr0x0(), ShieldSecurityToken, SessionSiteID, DBID);
                }

                string auditLogPatientID = string.Empty;
                if (!sendSignedCSMed)
                {
                    ePrescribeSvc.AuditLogPatientRxResponse rxResponse =
                        base.AuditLogPatientRxInsert(ePrescribeSvc.AuditAction.PATIENT_RX_CREATED, base.SessionPatientID,
                            rxID);
                    if (rxResponse.Success)
                    {
                        auditLogPatientID = rxResponse.AuditLogPatientID;
                    }
                }
                else
                {
                    //Get the inserted AuditLogPatientId from session or somewhere.
                    if (ucEPCSDigitalSigning != null)
                    {
                        if (ucEPCSDigitalSigning.AuditLogPatientID.ContainsKey(rxID))
                            auditLogPatientID = ucEPCSDigitalSigning.AuditLogPatientID[rxID];
                    }
                }

                long serviceTaskID = -1;
                if (!string.IsNullOrEmpty(smid) && PageState.GetStringOrEmpty("STANDING") == "1")
                {
                    if (sendSignedCSMed)
                    {
                        serviceTaskID = ScriptMessage.SendThisEPCSMessage(smid, base.SessionLicenseID,
                            base.SessionUserID, base.DBID);
                    }
                    else
                    {
                        serviceTaskID = ScriptMessage.SendThisMessage(smid, base.SessionLicenseID, base.SessionUserID, base.DBID);
                    }
                }

                ///Check here if it is not -1 then insert it in newly created table. you also have auditLogPatientID.
                ///This will be used from service manager and added last audit log when message is sent to hub.
                if (serviceTaskID != -1 && !string.IsNullOrEmpty(auditLogPatientID))
                {
                    Audit.InsertAuditLogPatientServiceTask(serviceTaskID, auditLogPatientID, base.DBID);
                }

                smid = ScriptMessage.CreateDUREVTScriptMessage(rxID, 1, base.SessionLicenseID, base.SessionUserID,
                    string.Empty, base.DBID);

                if ((PageState.GetStringOrEmpty("STANDING") == "1") && (!string.IsNullOrEmpty(smid)))
                {
                    ScriptMessage.SendOutboundInfoScriptMessage(smid, base.SessionLicenseID, base.SessionUserID,
                        base.DBID);
                }
            }
        }

        private void sendPrescriptionToDOCMOB(string rxID)
        {
            string mopharmID = PageState.GetStringOrEmpty("MOB_PHARMACY_ID");

            if (string.IsNullOrEmpty(mopharmID))
            {
                DataSet mobDS = Allscripts.Impact.Pharmacy.LoadPharmacyByNABP(PageState.GetStringOrEmpty("MOB_NABP"),
                    base.DBID);

                if (mobDS.Tables[0].Rows.Count > 0)
                {
                    mopharmID = mobDS.Tables[0].Rows[0]["PharmacyID"].ToString();
                }
            }

            if (!string.IsNullOrEmpty(mopharmID))
            {
                Prescription.UpdatePharmacyID(rxID, mopharmID, true, base.DBID);
                Prescription.SendToPhysicianForApproval(rxID, base.SessionLicenseID, base.SessionUserID,
                    PageState.GetStringOrEmpty("DelegateProviderID"), base.DBID);
            }
        }

        private void saveCSDetails(string rxID)
        {
            Rx rx = base.ScriptPadMeds.Find(r => r.RxID == rxID);

            if (rx != null)
            {
                //check both MediSpan and state-specific controlled substance values. If either is a CS, then this med is a CS.
                string reconciledControlledSubstanceCode =
                    Prescription.ReconcileControlledSubstanceCodes(rx.ControlledSubstanceCode,
                        rx.StateControlledSubstanceCode);

                bool? isCSRegistryChecked = null;

                if (chkRegistryChecked.Visible && !isSelfReport)
                {
                    isCSRegistryChecked = chkRegistryChecked.Checked;
                }
                //
                // should not need to update if the med is a Free Form med, RX_Detail_CS should already be updated
                //
                if ((!string.IsNullOrWhiteSpace(reconciledControlledSubstanceCode) &&
                     reconciledControlledSubstanceCode.ToUpper() != "U" &&
                    reconciledControlledSubstanceCode != "0") && !rx.IsFreeFormMedControlSubstance)
                {
                    Constants.ControlledSubstanceTypes csType = Constants.ControlledSubstanceTypes.FEDERAL;

                    if (reconciledControlledSubstanceCode != rx.ControlledSubstanceCode &
                        reconciledControlledSubstanceCode == rx.StateControlledSubstanceCode)
                    {
                        csType = Constants.ControlledSubstanceTypes.STATE;
                    }

                    string deaNumber = base.GetDEANumberToBeUsed(PageState);
                    // save DEA number and reconciled schedule
                    Prescription.SaveCSDetails(rxID, Convert.ToInt32(reconciledControlledSubstanceCode), csType,
                        deaNumber, isCSRegistryChecked, base.DBID);
                }
                else
                {
                    Prescription.UpdateCSRegistryChecked(rx.RxID, isCSRegistryChecked, base.DBID);
                }
            }
        }

        private string fixName(string name)
        {
            char[] splitter = ",".ToCharArray();
            string[] parts = name.Split(splitter);
            return parts[1] + " " + parts[0];
        }

        private bool canUserTryEPCS(RxUser user)
        {
            bool canTryEPCS = false;

            if (this.IsLicenseShieldEnabled & (this.IsEnterpriseClientEPCSEnabled || this.IsLicenseEPCSPurchased) &
                user.IsUserEPCSEnabled)
            {
                canTryEPCS = true;
            }

            return canTryEPCS;
        }

        #endregion

        private void setPptScriptDestination(Rx rx, GridDataItem tempDataItem)
        {
            if (PPTPlus.IsPPTPlusPreferenceOn(PageState))
            {
                Label lbl = (Label)tempDataItem.FindControl("lblGoodRxPharmacy");
                var ddldest = (DropDownList)tempDataItem.FindControl("ddlDest");
                var btnChangePptPharmacy = (HtmlInputHidden)tempDataItem.FindControl("btnHiddenTriggerChangePptPharmacy");

                if (hasPharmacyChanges)
                {
                    btnChangePptPharmacy.Attributes.Add("onclick", "TogglePharmacy(" + ddldest.ClientID + "," + lbl.ClientID + ");");
                    lbl.Text = rx.PharmacyName;
                    if (PageState.GetStringOrEmpty("MOB_Pharmacy_ID") == rx.PharmacyID)
                    {
                        if ((ddldest.SelectedValue == Patient.DOCPHARM) || (ddldest.SelectedValue == Patient.PHARM))
                        {
                            ddldest.SelectedValue = ddldest.Items?.FindByValue(Patient.MOB) != null ? Patient.MOB :
                            ddldest.Items?.FindByValue(Patient.DOCMOB) != null ? Patient.DOCMOB : ddldest.SelectedValue;
                        }
                    }
                    else
                    {
                        if ((ddldest.SelectedValue != null) && ((ddldest.SelectedValue == Patient.DOCPHARM) || (ddldest.SelectedValue == Patient.PHARM)))
                        {
                            lbl.Style.Add("display", "inline");
                        }
                    }
                }
            }
        }
    }
}
