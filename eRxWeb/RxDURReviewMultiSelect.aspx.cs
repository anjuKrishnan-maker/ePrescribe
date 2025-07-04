using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.ExtensionMethods;
using Allscripts.ePrescribe.Medispan.Clinical.Model.Response;
using Allscripts.ePrescribe.Medispan.Clinical.Model.Settings;
using Allscripts.ePrescribe.Objects;
using Allscripts.ePrescribe.Objects.StoredProcReturnObjects;
using Allscripts.Impact;
using Allscripts.Impact.Interfaces;
using eRxWeb.AppCode.DurBPL;
using eRxWeb.AppCode.DurBPL.RequestModel;
using eRxWeb.AppCode.DurBPL.ResponseModel;
using eRxWeb.Controller;
using Telerik.Web.UI;
using Constants = Allscripts.ePrescribe.Common.Constants;
using Rx = Allscripts.Impact.Rx;
using ScriptMessage = Allscripts.Impact.ScriptMsg.ScriptMessage;
using static Allscripts.Impact.IgnoreReason;

namespace eRxWeb
{
    public partial class RxDURReviewMultiSelect : BasePage
    {
        #region Properties

        public const string DURIdKey = "DurIndex";
        public const string FullWarningTextKey = "FullWarningText";
        public const string LineNumberKey = "LineNumber";
        public const string RxIdKey = "RxID";
        public const string Display = "display";
        public const string Inline = "inline";
        public const string MessageText= "Please select an ignore reason for all DUR warnings.";
        IPrescription iPrescription = new Prescription();
        static RxDurResponseModel rxDurResponseModel;
        DURCheckResponse durWarWarnings;
        RxDurReviewApiController _rxDurRevMultiSelectApiController;
        IEnumerable<PatientActiveMedDetail> ActiveMedications
        {
            get
            {
                if (PageState[Constants.SessionVariables.ActiveMedicationDetails] == null)
                {
                    PageState[Constants.SessionVariables.ActiveMedicationDetails] = Allscripts.Impact.Patient.GetActiveMedicationDetails(SessionPatientID.ToGuidOr0x0(), DBID);
                }

                return PageState[Constants.SessionVariables.ActiveMedicationDetails] == null ? null: PageState[Constants.SessionVariables.ActiveMedicationDetails] as IEnumerable<PatientActiveMedDetail>; ;
            }
        }
   
        string RefillRequestProviderID
        {
            get
            {
                string refillRequestProviderID = string.Empty;

                if (PageState["RefillRequestProviderID"] != null)
                {
                    refillRequestProviderID = PageState["RefillRequestProviderID"].ToString();
                }

                return refillRequestProviderID;
            }
            set
            {
                PageState["RefillRequestProviderID"] = value;
            }
        }
        
        public RxDurReviewApiController RxDurRevMultiSelectApiController
        {
            get
            {
                if (_rxDurRevMultiSelectApiController == null)
                    _rxDurRevMultiSelectApiController = new RxDurReviewApiController(Constants.DurType.MULTI_SELECT);
                return _rxDurRevMultiSelectApiController;
            }
        }

               #endregion

        #region Page Events and Event Handlers

        protected void Page_PreRenderComplete(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                for (int i = 0; i < grdDoseList.Items.Count; i++)
                {
                    grdDoseList.Items[i].ExpandHierarchyToTop();
                }
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
             base.SetSingleClickButton(btnSubmit);
            this.ucCSMedRefillRequestNotAllowed.OnPrintRefillRequest += new EventHandler(ucCSMedRefillRequestNotAllowed_OnPrintRefillRequest);
            this.ucCSMedRefillRequestNotAllowed.OnContactProvider += new EventHandler(ucCSMedRefillRequestNotAllowed_OnContactProvider);
            ucMainMessage.Visible = false;
            ucErrorMessage.Visible = false;

            if (!IsPostBack)
            {
                if ((base.SessionUserType != Constants.UserCategory.POB_SUPER
                     && DURSettings.IgnoreReasons == DURIgnoreReason.NOT_REQUIRED)
                    || base.SessionUserType == Constants.UserCategory.POB_LIMITED)
                {
                    grdParList.Columns[grdParList.Columns.Count - 1].Visible = false;
                    grdIntList.Columns[grdIntList.Columns.Count - 1].Visible = false;
                    grdDupList.Columns[grdDupList.Columns.Count - 1].Visible = false;
                    grdDoseList.Columns[grdDoseList.Columns.Count - 1].Visible = false;
                    grdAlcList.Columns[grdAlcList.Columns.Count - 1].Visible = false;
                    grdFoodList.Columns[grdFoodList.Columns.Count - 1].Visible = false;
                    grdCustList.Columns[grdCustList.Columns.Count - 1].Visible = false;
                }

                warningError.Style["display"] = "none";
                
                ucMainMessage.MessageText = "The medications you have prescribed have created the following Drug Utilization Warnings (DUR). For Duplicate Therapy Warnings, there may be one or more similar medications on the patient’s active medication list. For all other DUR warnings, there are Drug interactions that have been identified with medications you have just prescribed. Please review the warnings listed and determine if current prescriptions need to be completed or if the warnings presented are acceptable to continue with therapy.";
                ucMainMessage.Icon = Controls_Message.MessageType.INFORMATION;
                ucMainMessage.Visible = true;

                ucErrorMessage.MessageText = "Please select an ignore reason for all DUR warnings.";
                ucErrorMessage.Icon = Controls_Message.MessageType.ERROR;
                ucErrorMessage.Visible = true;

                rxDurResponseModel = (RxDurResponseModel)RxDurRevMultiSelectApiController.DurRxList(new eRxWebAppCodeWrapper()).Payload;

                if (Convert.ToBoolean(Request.QueryString["SimpleShowDUR"]))
                {
                    SetAndShowDURGrids();
                    return;
                }

                if (PageState[Constants.SessionVariables.TaskScriptMessageId] != null || MasterPage.RxTask != null)
                {
                    SetPharmReqWorkflowFormatting();
                }
                else
                {
                    MedsWithDURs = null;
                    DURCheckResponse durWarnings = null;
                    RxDurRevMultiSelectApiController.DurWarnings(durWarnings);
                }                
                bool warnings = false;               

                if (rxDurResponseModel.SetAndShowDurWarnings)
                {
                    warnings = SetAndShowDURGrids();
                }
                if (!warnings)
                {
                    DurRedirectModel durRedirectModel = (DurRedirectModel)RxDurRevMultiSelectApiController.DurRedirectDetails().Payload;
                    Response.Redirect(durRedirectModel.RedirectUrl);
                }
            }
        }

        private bool SetAndShowDURGrids()
        {
            var warnings = false;
            if (rxDurResponseModel.WarningsListHasItems)
            {
                SetAndShowControls("grdWarningsList", ref warningsPanelDetail, ref warningsPanelHeader, ref radAjaxManager1);
                warnings = true;
            }            
            if (rxDurResponseModel.CustListFreeFormDrugs)
            {
                SetAndShowControls("grdCustList", ref custPanelDetail, ref custPanelHeader, ref radAjaxManager1);
                warnings = true;
            }
            if (rxDurResponseModel.FoodListInteractions)
            {
                SetAndShowControls("grdFoodList", ref foodPanelDetail, ref foodPanelHeader, ref radAjaxManager1);
                warnings = true;
            }

            if (rxDurResponseModel.AlcoholListInteractions)
            {
                SetAndShowControls("grdAlcList", ref alcPanelDetail, ref alcPanelHeader, ref radAjaxManager1);
                warnings = true;
            }

            if (rxDurResponseModel.HasPriorAdverseReactions)
            {
                SetAndShowControls("grdParList", ref parPanelDetail, ref parPanelHeader, ref radAjaxManager1);
                warnings = true;
            }

            if (rxDurResponseModel.HasDuplicateTherapyItems)
            {
                SetAndShowControls("grdDupList", ref dupPanelDetail, ref dupPanelHeader, ref radAjaxManager1);
                warnings = true;
            }

            if (rxDurResponseModel.HasDrugInteractionsItems)
            {
                SetAndShowControls("grdIntList", ref intPanelDetail, ref intPanelHeader, ref radAjaxManager1);
                warnings = true;
            }

            if (rxDurResponseModel.HasDosageCheckMedicationsItems)
            {
                SetAndShowControls("grdDoseList", ref dosePanelDetail, ref dosePanelHeader, ref radAjaxManager1);
                warnings = true;
            }
            return warnings;
        }


        private void SetAndShowControls(string gridName, ref Panel pnlDetail, ref Panel pnlHeader, ref RadAjaxManagerProxy radAjaxManagerProxy)
        {
            pnlDetail.Visible = true;
            pnlHeader.Visible = true;
            var newUpdateControl = new AjaxUpdatedControl
            {
                ControlID = gridName,
                LoadingPanelID = "LoadingPanel1"
            };
            radAjaxManagerProxy.AjaxSettings[0].UpdatedControls.Add(newUpdateControl);
        }

        private void SetPharmReqWorkflowFormatting()
        {
            ScriptMessage sm = null;
            if (PageState.Cast("Tasktype", Constants.PrescriptionTaskType.DEFAULT) == Constants.PrescriptionTaskType.REFREQ && PageState.ContainsKey(Constants.SessionVariables.TaskScriptMessageId))
            {
                sm = new ScriptMessage(PageState.GetStringOrEmpty(Constants.SessionVariables.TaskScriptMessageId), SessionLicenseID, SessionUserID, DBID);
            }
            else if (MasterPage.RxTask != null && MasterPage.RxTask.TaskType == Constants.PrescriptionTaskType.RXCHG)
            {
                sm = MasterPage.RxTask.ScriptMessage as ScriptMessage;
            }

            if (sm != null)
            {
                RefillRequestProviderID = sm.ProviderID;                
            }
        }

        protected void Page_LoadComplete(object sender, EventArgs e)
        {
            ((PhysicianMasterPage)Master).hideTabs();

            if (Session["Tasktype"] != null && (Constants.PrescriptionTaskType)Session["Tasktype"] == Constants.PrescriptionTaskType.REFREQ)   // refill?
            {
            }
            else if (PageState.GetStringOrEmpty(Constants.SessionVariables.DUR_GO_PREVIOUS) != Constants.PageNames.SELF_REPORTED_MED)
            {
                if (base.PatientHasScriptPadItems)
                {
                    if (!this.hasDURWarnings)
                    {
                        btnBack.Visible = false;
                        btnSubmit.Text = "Continue";
                        btnSubmit.ToolTip = "Click to continue";
                        lblCopyright.Text = string.Empty;
                        durWarWarnings = RxDurRevMultiSelectApiController.DurWarnings(new eRxWebAppCodeWrapper()).Payload;
                        bool? hasUnexpectedWarnings = durWarWarnings?.Warnings?.Exists(x => x.Contains("A Drug Interaction Check could not be performed on this medication"));
                        ucMainMessage.MessageText = hasUnexpectedWarnings.GetValueOrDefault()? "A Drug Interaction check could not be performed on one of the medication(s) on the script pad.  Click continue to proceed." :
                            "No DUR warnings were found for the prescriptions currently on the script pad. Click Continue to proceed.";
                        ucMainMessage.Icon = Controls_Message.MessageType.INFORMATION;
                        ucMainMessage.Visible = true;
                    }
                    else
                    {
                        ucMainMessage.Icon = Controls_Message.MessageType.INFORMATION;
                        ucMainMessage.Visible = true;
                        lblCopyright.Text = PageState.GetStringOrEmpty(Constants.SessionVariables.MedispanCopyright);
                    }

                }
                else
                {          
                    if (Session["DelegateProviderID"] != null)
                    btnSubmit.PostBackUrl = (Constants.PageNames.REDIRECT_TO_ANGULAR + "?componentName=" + Constants.PageNames.SELECT_MEDICATION);
                    else
                    btnSubmit.PostBackUrl = (Constants.PageNames.REDIRECT_TO_ANGULAR + "?componentName=" + Constants.PageNames.SELECT_MEDICATION);

                    if (PageState.GetStringOrEmpty(Constants.SessionVariables.TaskScriptMessageId).Equals(string.Empty) && MasterPage.RxTask == null)
                    {
                        btnBack.Visible = false;
                        ucMainMessage.MessageText = "There are no more items in the script pad. Click Continue to proceed.";
                    }
                    ucMainMessage.Icon = Controls_Message.MessageType.INFORMATION;
                    ucMainMessage.Visible = true;
                    btnSubmit.Text = "Continue";
                    btnSubmit.ToolTip = "Click to continue";
                    lblCopyright.Text = string.Empty;
                }
            }
        }
        //this need to be moved to api code
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            SubmitDurRequest submitRequest = new SubmitDurRequest();
            if (captureReasons())
            {
                submitRequest.IsCapturedReasons = true;
            }
            submitRequest.UserHostAddress = Request.UserIpAddress();
            SubmitDurResponse submitResponse = (SubmitDurResponse)RxDurRevMultiSelectApiController.SubmitDurForm(submitRequest, new ImpactDurWraper()).Payload;
            if (submitResponse.IsGatherWarningsAndInfoToWriteToDB)
                GatherWarningsAndInfoToWriteToDB(submitResponse.RxID);
            
            if (!submitResponse.IsstarsAlign && submitResponse.IsCSMedRefillRequestNotAllowed)
            {
                ucCSMedRefillRequestNotAllowed.ShowPopUp();
                return;
            }
          
            if (submitResponse.IsIgnoreReasonsNotSelected)
            {
                warningError.Style[Display] = Inline;
                ucMainMessage.Visible = true;
                /* If no reasons selected, display the error message */
                ucErrorMessage.MessageText = MessageText;
                ucErrorMessage.Icon = Controls_Message.MessageType.ERROR;
                ucErrorMessage.Visible = true;
            }
            if (submitResponse.selectPatientComponentParameters != null)
            {
                RedirectToSelectPatient(null, submitResponse.selectPatientComponentParameters);
            }
            else if (!string.IsNullOrEmpty(submitResponse.Url))
            {
                Response.Redirect(submitResponse.Url);
            }
           
            if (durWarWarnings != null) SetAndShowDURGrids();
        }

        private string AddMassachusettsOpiateMessage(string csCode, string gpi)
        {
            StringBuilder sbComments = new StringBuilder();
            IPrescription Prescription = new Prescription();

            if (Prescription.IsValidMassOpiate(
                 PageState.GetStringOrEmpty(Constants.SessionVariables.PracticeState),
                 gpi,
                 csCode,
                 Convert.ToBoolean(Session["HasExpiredDEA"]),
                 (List<string>)(Session["DEASCHEDULESALLOWED"])))
            {
                sbComments.Append(Constants.PartialFillUponPatientRequest);
            }
            return sbComments.ToString();
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            GoBackRequest backRequest = new GoBackRequest(MasterPage.ChangeRxRequestedMedCs);
            GoBackResponse backResponse=(GoBackResponse)RxDurRevMultiSelectApiController.GoBack(backRequest).Payload;
            Response.Redirect(backResponse.Url);
            
        }

        protected void grdParList_OnNeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            FetchDurWarnings();
            grdParList.DataSource = durWarWarnings.PriorAdverseReactions.Reactions;
        }

        protected void grdParList_ItemDataBound(object sender, GridItemEventArgs e)
        {
            var item = e.Item as GridDataItem;            
            if (item != null)
            {               
                SetInfoIcon(ref item);
            }
        }
        protected void grdParList_ItemCreated(Object sender, GridItemEventArgs e)
        {
            if (e.Item != null)
            {
                if (e.Item is GridHeaderItem)
                {
                    RadComboBox ddlHeaderReasons = (RadComboBox)e.Item.FindControl("ddlHeaderReasons");
                    ddlHeaderReasons.DataSource = RxDurRevMultiSelectApiController.IgnoreDurReasonsByCategoryandUserType(IgnoreCategory.DUR_PAR,new ImpactDurWraper()).Payload;
                    ddlHeaderReasons.DataBind();
                }
                if (e.Item is GridDataItem)
                {
                    RadComboBox ddlReasons = (RadComboBox)e.Item.FindControl("ddlReasons");
                    ddlReasons.DataSource = RxDurRevMultiSelectApiController.IgnoreDurReasonsByCategoryandUserType(IgnoreCategory.DUR_PAR, new ImpactDurWraper()).Payload;
                    ddlReasons.DataBind();
                }
            }
        }


        private static void SetInfoIcon(ref GridDataItem item)
        {
            var infoIcon = item["GetInfo"].FindControl("info") as Image;
            infoIcon?.Attributes.Add(RxIdKey, Convert.ToString(item.GetDataKeyValue(RxIdKey)));
            infoIcon?.Attributes.Add(LineNumberKey, Convert.ToString(item.GetDataKeyValue(LineNumberKey)));
            infoIcon?.Attributes.Add("onClick", $"showDurDetailsOverlay('{Convert.ToBase64String(Encoding.UTF8.GetBytes(Convert.ToString(item.GetDataKeyValue(FullWarningTextKey))))}');");
        }

        protected void grdDupList_OnNeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            FetchDurWarnings();
            grdDupList.DataSource = durWarWarnings.DuplicateTherapy.Results;
        }

        protected void grdDupList_ItemDataBound(object sender, GridItemEventArgs e)
        {
            var item = e.Item as GridDataItem;
            if (item != null)
            {
                SetInfoIcon(ref item);
            }
        }
        protected void grdDupList_ItemCreated(Object sender, GridItemEventArgs e)
        {
            if (e.Item != null)
            {
                if (e.Item is GridHeaderItem)
                {
                    RadComboBox ddlHeaderReasons = (RadComboBox)e.Item.FindControl("ddlHeaderReasons");
                    ddlHeaderReasons.DataSource = RxDurRevMultiSelectApiController.IgnoreDurReasonsByCategoryandUserType(IgnoreCategory.DUR_DUP, new ImpactDurWraper()).Payload;
                    ddlHeaderReasons.DataBind();
                }
                if (e.Item is GridDataItem)
                {
                    RadComboBox ddlReasons = (RadComboBox)e.Item.FindControl("ddlReasons");
                    ddlReasons.DataSource = RxDurRevMultiSelectApiController.IgnoreDurReasonsByCategoryandUserType(IgnoreCategory.DUR_DUP, new ImpactDurWraper()).Payload;
                    ddlReasons.DataBind();
                }
            }
        }
        protected void grdCustList_OnNeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            var rxList=((RxDurResponseModel)(RxDurRevMultiSelectApiController.DurRxList(new eRxWebAppCodeWrapper()).Payload)).RxList;          
            grdCustList.DataSource = DURMedispanUtils.RetrieveFreeFormDrugs(rxList);
        }

        protected void grdCustList_ItemDataBound(object sender, GridItemEventArgs e)
        {
        }
        protected void grdCustList_ItemCreated(Object sender, GridItemEventArgs e)
        {
            if (e.Item != null)
            {
                if (e.Item is GridHeaderItem)
                {
                    RadComboBox ddlHeaderReasons = (RadComboBox)e.Item.FindControl("ddlHeaderReasons");
                    ddlHeaderReasons.DataSource = RxDurRevMultiSelectApiController.IgnoreDurReasonsByCategoryandUserType(IgnoreCategory.DUR_CUS, new ImpactDurWraper()).Payload;
                    ddlHeaderReasons.DataBind();
                }
                if (e.Item is GridDataItem)
                {
                    RadComboBox ddlReasons = (RadComboBox)e.Item.FindControl("ddlReasons");
                    ddlReasons.DataSource = RxDurRevMultiSelectApiController.IgnoreDurReasonsByCategoryandUserType(IgnoreCategory.DUR_CUS, new ImpactDurWraper()).Payload;
                    ddlReasons.DataBind();
                }
            }
        }

        protected void grdDoseList_OnNeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            FetchDurWarnings();
            grdDoseList.DataSource = durWarWarnings.Dosage.DosageCheckMedications;
        }

        protected void grdDoseList_ItemDataBound(object sender, GridItemEventArgs e)
        {

        }

        protected void grdAlcList_OnNeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            FetchDurWarnings();
            grdAlcList.DataSource = durWarWarnings.AlcoholInteractions.Interactions;
        }

        protected void grdAlcList_ItemDataBound(object sender, GridItemEventArgs e)
        {
            var item = e.Item as GridDataItem;
            if (item != null)
            {
                SetInfoIcon(ref item);
            }
        }
        protected void grdAlcList_ItemCreated(object sender, GridItemEventArgs e)
        {
            if (e.Item != null)
            {
                if (e.Item is GridHeaderItem)
                {
                    RadComboBox ddlHeaderReasons = (RadComboBox)e.Item.FindControl("ddlHeaderReasons");
                    ddlHeaderReasons.DataSource = RxDurRevMultiSelectApiController.IgnoreDurReasonsByCategoryandUserType(IgnoreCategory.DUR_ALC, new ImpactDurWraper()).Payload;
                    ddlHeaderReasons.DataBind();
                }
                if (e.Item is GridDataItem)
                {
                    RadComboBox ddlReasons = (RadComboBox)e.Item.FindControl("ddlReasons");
                    ddlReasons.DataSource = RxDurRevMultiSelectApiController.IgnoreDurReasonsByCategoryandUserType(IgnoreCategory.DUR_ALC, new ImpactDurWraper()).Payload;
                    ddlReasons.DataBind();
                }
            }
        }

        protected void grdFoodList_OnNeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            FetchDurWarnings();
            grdFoodList.DataSource = durWarWarnings.FoodInteractions.Interactions;
        }

        protected void grdFoodList_ItemDataBound(object sender, GridItemEventArgs e)
        {
            var item = e.Item as GridDataItem;
            if (item != null)
            {
                SetInfoIcon(ref item);
            }
        }
        protected void grdFoodList_ItemCreated(object sender, GridItemEventArgs e)
        {
            if (e.Item != null)
            {
                if (e.Item is GridHeaderItem)
                {
                    RadComboBox ddlHeaderReasons = (RadComboBox)e.Item.FindControl("ddlHeaderReasons");
                    ddlHeaderReasons.DataSource = RxDurRevMultiSelectApiController.IgnoreDurReasonsByCategoryandUserType(IgnoreCategory.DUR_FOO, new ImpactDurWraper()).Payload;
                    ddlHeaderReasons.DataBind();
                }
                if (e.Item is GridDataItem)
                {
                    RadComboBox ddlReasons = (RadComboBox)e.Item.FindControl("ddlReasons");
                    ddlReasons.DataSource = RxDurRevMultiSelectApiController.IgnoreDurReasonsByCategoryandUserType(IgnoreCategory.DUR_FOO, new ImpactDurWraper()).Payload;
                    ddlReasons.DataBind();
                }
            }
        }
        protected void grdDoseList_ItemCreated(Object sender, GridItemEventArgs e)
        {
            if (e.Item != null)
            {
                if (e.Item is GridHeaderItem)
                {
                    RadComboBox ddlHeaderReasons = (RadComboBox)e.Item.FindControl("ddlHeaderReasons");
                    ddlHeaderReasons.DataSource = RxDurRevMultiSelectApiController.IgnoreDurReasonsByCategoryandUserType(IgnoreCategory.DUR_DOS, new ImpactDurWraper()).Payload;
                    ddlHeaderReasons.DataBind();
                }
                if (e.Item is GridDataItem)
                {
                    RadComboBox ddlReasons = (RadComboBox)e.Item.FindControl("ddlReasons");
                    ddlReasons.DataSource = RxDurRevMultiSelectApiController.IgnoreDurReasonsByCategoryandUserType(IgnoreCategory.DUR_DOS, new ImpactDurWraper()).Payload;
                    ddlReasons.DataBind();
                }
            }
        }
        protected void grdIntList_OnNeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
           FetchDurWarnings();
            grdIntList.DataSource = durWarWarnings.DrugInteractions.Interactions;
        }

        protected void grdIntList_ItemDataBound(object sender, GridItemEventArgs e)
        {
            var item = e.Item as GridDataItem;           
            if (item != null)
            {
                SetInfoIcon(ref item);
            }
        }
        protected void grdIntList_ItemCreated(Object sender, GridItemEventArgs e)
        {
            if (e.Item != null)
            {
                if (e.Item is GridHeaderItem)
                {
                    RadComboBox ddlHeaderReasons = (RadComboBox)e.Item.FindControl("ddlHeaderReasons");
                    ddlHeaderReasons.DataSource = RxDurRevMultiSelectApiController.IgnoreDurReasonsByCategoryandUserType(IgnoreCategory.DUR_INT, new ImpactDurWraper()).Payload;
                    ddlHeaderReasons.DataBind();
                }
                if (e.Item is GridDataItem)
                {
                    RadComboBox ddlReasons = (RadComboBox)e.Item.FindControl("ddlReasons");
                    ddlReasons.DataSource = RxDurRevMultiSelectApiController.IgnoreDurReasonsByCategoryandUserType(IgnoreCategory.DUR_INT, new ImpactDurWraper()).Payload;
                    ddlReasons.DataBind();
                }
            }
        }

        protected void grdWarningsList_OnNeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            FetchDurWarnings();
            grdWarningsList.DataSource = durWarWarnings.Warnings;
        }

        protected void grdParList_DataBound(object sender, EventArgs e)
        {
            parPanelHeader.Visible = parPanelDetail.Visible = grdParList.Items.Count > 0;
        }

        protected void grdIntList_DataBound(object sender, EventArgs e)
        {
            intPanelHeader.Visible = intPanelDetail.Visible = grdIntList.Items.Count > 0;
        }

        protected void grdDupList_DataBound(object sender, EventArgs e)
        {
            dupPanelHeader.Visible = dupPanelDetail.Visible = grdDupList.Items.Count > 0;
        }

        protected void grdDoseList_DataBound(object sender, EventArgs e)
        {
            dosePanelHeader.Visible = dosePanelDetail.Visible = grdDoseList.Items.Count > 0;
        }

        protected void grdAlcList_DataBound(object sender, EventArgs e)
        {
            alcPanelHeader.Visible = alcPanelDetail.Visible = grdAlcList.Items.Count > 0;
        }

        protected void grdFoodList_DataBound(object sender, EventArgs e)
        {
            foodPanelHeader.Visible = foodPanelDetail.Visible = grdFoodList.Items.Count > 0;
        }

        protected void grdCustList_DataBound(object sender, EventArgs e)
        {
            custPanelHeader.Visible = custPanelDetail.Visible = grdCustList.Items.Count > 0;
        }
        protected void grdWarningsList_DataBound(object sender, EventArgs e)
        {
            warningsPanelHeader.Visible = warningsPanelDetail.Visible = grdWarningsList.Items.Count > 0;
            lblCopyright.Text = PageState.GetStringOrEmpty(Constants.SessionVariables.MedispanCopyright);
        }

        void ucCSMedRefillRequestNotAllowed_OnContactProvider(object sender, EventArgs e)
        {
            CSMedRefillNotAllowedContactResponse response =(CSMedRefillNotAllowedContactResponse)RxDurRevMultiSelectApiController.CSMedRefillRequestNotAllowedOnContactProvider(new CSMedRefillNotAllowedContactRequest(base.ShieldSecurityToken), new ImpactDurWraper()).Payload;
            Response.Redirect(response.RedirectUrl);
        }

        void ucCSMedRefillRequestNotAllowed_OnPrintRefillRequest(object sender, EventArgs e)
        {
            CSMedRefillNotAllowedPrintRefillResponse responseOnPrintRefillRequest = (CSMedRefillNotAllowedPrintRefillResponse)RxDurRevMultiSelectApiController.CSMedRefillRequestNotAllowedOnPrintRefill(new CSMedRefillNotAllowedPrintRefillRequest(), new ImpactDurWraper()).Payload;
            GatherWarningsAndInfoToWriteToDB(responseOnPrintRefillRequest.RxId);
            Server.Transfer(responseOnPrintRefillRequest.Url);
        }
        #endregion

        #region Private Methods

        private bool captureReasons()
        {
            if (base.SessionUserType == Constants.UserCategory.POB_SUPER
               || DURSettings.IgnoreReasons == DURIgnoreReason.REQUIRED)
            {
                int i = 0;
                int headerReasonID = getHeaderReason(grdParList);
                for (i = 0; i < grdParList.Items.Count; i++)
                {
                    if (grdParList.Items[i] is GridDataItem)
                    {
                        if (grdParList.Items[i]["Reason"].FindControl("ddlReasons") != null
                            && (grdParList.Items[i]["Reason"].FindControl("ddlReasons") as RadComboBox).SelectedIndex > 0)
                        {

                        }
                        else if (grdParList.Items[i]["Reason"].FindControl("ddlReasons") != null && headerReasonID == -1)
                        {
                            return false;
                        }
                    }
                }

                headerReasonID = getHeaderReason(grdIntList);
                for (i = 0; i < grdIntList.Items.Count; i++)
                {
                    if (grdIntList.Items[i] is GridDataItem)
                    {
                        if (grdIntList.Items[i]["Reason"].FindControl("ddlReasons") != null
                        && (grdIntList.Items[i]["Reason"].FindControl("ddlReasons") as RadComboBox).SelectedIndex > 0)
                        {

                        }
                        else if (grdIntList.Items[i]["Reason"].FindControl("ddlReasons") != null && headerReasonID == -1)
                        {
                            return false;
                        }
                    }
                }

                headerReasonID = getHeaderReason(grdDupList);
                for (i = 0; i < grdDupList.Items.Count; i++)
                {
                    if (grdDupList.Items[i] != null && grdDupList.Items[i].OwnerID == "ctl00_ContentPlaceHolder1_grdDupList_ctl00")
                    {
                        if (grdDupList.Items[i]["Reason"].FindControl("ddlReasons") != null
                            && (grdDupList.Items[i]["Reason"].FindControl("ddlReasons") as RadComboBox).SelectedIndex > 0)
                        {

                        }
                        else if (grdDupList.Items[i]["Reason"].FindControl("ddlReasons") != null && headerReasonID == -1)
                        {
                            return false;
                        }
                    }
                }

                headerReasonID = getHeaderReason(grdDoseList);
                for (i = 0; i < grdDoseList.Items.Count; i++)
                {
                    if (grdDoseList.Items[i] != null && grdDoseList.Items[i].OwnerID == "ctl00_ContentPlaceHolder1_grdDoseList_ctl00")
                    {
                        if (grdDoseList.Items[i]["Reason"].FindControl("ddlReasons") != null
                            && (grdDoseList.Items[i]["Reason"].FindControl("ddlReasons") as RadComboBox).SelectedIndex > 0)
                        {

                        }
                        else if (grdDoseList.Items[i]["Reason"].FindControl("ddlReasons") != null && headerReasonID == -1)
                        {
                            return false;
                        }
                    }
                }

                headerReasonID = getHeaderReason(grdAlcList);
                for (i = 0; i < grdAlcList.Items.Count; i++)
                {
                    if (grdAlcList.Items[i] != null)
                    {
                        if (grdAlcList.Items[i]["Reason"].FindControl("ddlReasons") != null
                        && (grdAlcList.Items[i]["Reason"].FindControl("ddlReasons") as RadComboBox).SelectedIndex > 0)
                        {

                        }
                        else if (grdAlcList.Items[i]["Reason"].FindControl("ddlReasons") != null && headerReasonID == -1)
                        {
                            return false;
                        }
                    }
                }

                headerReasonID = getHeaderReason(grdFoodList);
                for (i = 0; i < grdFoodList.Items.Count; i++)
                {
                    if (grdFoodList.Items[i] != null)
                    {
                        if (grdFoodList.Items[i]["Reason"].FindControl("ddlReasons") != null
                        && (grdFoodList.Items[i]["Reason"].FindControl("ddlReasons") as RadComboBox).SelectedIndex > 0)
                        {

                        }
                        else if (grdFoodList.Items[i]["Reason"].FindControl("ddlReasons") != null && headerReasonID == -1)
                        {
                            return false;
                        }
                    }
                }

                headerReasonID = getHeaderReason(grdCustList);
                if (base.SessionUserType != Constants.UserCategory.POB_SUPER && headerReasonID == -1) // Removed PA/w sup from the condition to change hdrreasonID to 0 for #710948
                    headerReasonID = 0;

                for (i = 0; i < grdCustList.Items.Count; i++)
                {
                    if (grdCustList.Items[i] != null)
                    {
                        var radComboBox = grdCustList.Items[i]["Reason"].FindControl("ddlReasons") as RadComboBox;
                        if (radComboBox != null && (grdCustList.Items[i]["Reason"].FindControl("ddlReasons") != null
                                                                                                                && radComboBox.SelectedIndex > 0))
                        {

                        }
                        else if (grdCustList.Items[i]["Reason"].FindControl("ddlReasons") != null && headerReasonID == -1)
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        private void clearDataGridDataSources()
        {
            Session["CURRENT_DUR_WARNINGS"] = null;
            grdParList.DataSource = null;
            grdDupList.DataSource = null;
            grdCustList.DataSource = null;
            grdDoseList.DataSource = null;
            grdAlcList.DataSource = null;
            grdFoodList.DataSource = null;
            grdIntList.DataSource = null;


            grdParList.Rebind();
            grdDupList.Rebind();
            grdCustList.Rebind();
            grdDoseList.Rebind();
            grdAlcList.Rebind();
            grdFoodList.Rebind();
            grdIntList.Rebind();
        }

        protected void DeletedScript(object sender, EventArgs e)
        {
            clearDataGridDataSources();
        }

        private int getHeaderReason(RadGrid currentGrid)
        {
            int reasonID = -1;
            if (currentGrid.MasterTableView.GetItems(GridItemType.Header).Length > 0)
            {
                GridHeaderItem headerItem = currentGrid.MasterTableView.GetItems(GridItemType.Header)[0] as GridHeaderItem;
                object reasons = headerItem == null ? null : headerItem.FindControl("ddlHeaderReasons");

                if ((reasons as RadComboBox) != null
                    && (reasons as RadComboBox).SelectedIndex > 0)
                {
                    reasonID = Convert.ToInt32((reasons as RadComboBox).SelectedValue);
                }
            }

            return reasonID;
        }

        protected void GetInfo(object sender, EventArgs e)
        {
            string rxID = (sender as ImageButton).Attributes[RxIdKey].ToString();
            string lineNumber = (sender as ImageButton).Attributes[LineNumberKey].ToString();
            litDetails.Text = (sender as ImageButton).Attributes[FullWarningTextKey].ToString();
            mpeDurDetail.Show();
        }

        private bool hasDURWarnings
        {
            get
            {
                return
                    grdParList.Items.Count > 0 ||
                    grdIntList.Items.Count > 0 ||
                    grdDupList.Items.Count > 0 ||
                    grdDoseList.Items.Count > 0 ||
                    grdAlcList.Items.Count > 0 ||
                    grdFoodList.Items.Count > 0 ||
                    grdCustList.Items.Count > 0;
            }
        }

        private bool isMedControlledSubstanceForEPCS(string fedCSCode, string pharmacyCSCode, string practiceCSCode)
        {
            bool retunVal = false;

            if ((!string.IsNullOrEmpty(fedCSCode) && !fedCSCode.Equals("U", StringComparison.OrdinalIgnoreCase)) ||
                ((!string.IsNullOrEmpty(pharmacyCSCode) && !pharmacyCSCode.Equals("U", StringComparison.OrdinalIgnoreCase) && !pharmacyCSCode.Equals("0", StringComparison.OrdinalIgnoreCase)) &&
                (!string.IsNullOrEmpty(practiceCSCode) && !practiceCSCode.Equals("U", StringComparison.OrdinalIgnoreCase) && !practiceCSCode.Equals("0", StringComparison.OrdinalIgnoreCase))))
            {
                retunVal = true;
            }

            return retunVal;
        }

        /// <summary>
        /// returns string for a given column in a given datarow
        /// </summary>
        /// <param name="parmValue">Column Name</param>
        /// <param name="dr">DataRow</param>
        /// <returns></returns>
        private string getStringFromDataRow(string parmValue, DataRow dr)
        {
            string retVal = string.Empty;

            if (dr[parmValue] != DBNull.Value)
            {
                retVal = dr[parmValue].ToString();
            }

            return retVal;
        }

        private List<Guid> GetScreenedRxIds(DURWarningType durType, int durIndex)
        {
            var rxIds = new List<Guid>();
            FetchDurWarnings();
            switch (durType)
            {
                case DURWarningType.ALCOHOL_INTERACTION:
                    rxIds.Add(durWarWarnings.AlcoholInteractions.Interactions.Find(x => x.DurIndex == durIndex).ExternalId.ToGuidOr0x0());
                    break;
                case DURWarningType.DOSAGE:
                    rxIds.Add(durWarWarnings.Dosage.DosageCheckMedications.Find(x => x.DurIndex == durIndex).ExternalId.ToGuidOr0x0());
                    break;
                case DURWarningType.DUPLICATE_THERAPY:
                    var dupList = durWarWarnings.DuplicateTherapy.Results.Find(x => x.DurIndex == durIndex);
                    //Need to load all dup RxIds
                    if (dupList?.DupDrugs != null)
                    {
                        dupList.DupDrugs.ForEach(dup =>
                        {
                            if (dup != null && !rxIds.Contains(dup.ExternalId.ToGuidOr0x0())) rxIds.Add(dup.ExternalId.ToGuidOr0x0());
                        });
                    }
                    break;
                case DURWarningType.FOOD_INTERACTION:
                    rxIds.Add(durWarWarnings.FoodInteractions.Interactions.Find(x => x.DurIndex == durIndex).ExternalId.ToGuidOr0x0());
                    break;
                case DURWarningType.INTERACTION:
                    rxIds.Add(durWarWarnings.DrugInteractions.Interactions.Find(x => x.DurIndex == durIndex).ExternalId.ToGuidOr0x0());
                    break;
                case DURWarningType.PAR:
                    var category = durWarWarnings.PriorAdverseReactions.Reactions.Find(x => x.DurIndex == durIndex);
                    //Need to load all meds that could have reacted together.
                    category.LineItems.ForEach(lineItem => {rxIds.Add(lineItem.ExternalId.ToGuidOr0x0()); });
                    break;
            }
            return rxIds;
        }
        public void AddWarningsToSave(ref List<DurWarningSaveRecord> warningsToSave, RadGrid durGrid, DURWarningType durType, ref ArrayList rxToProviderList, ref List<Guid> durList, string taskingRxId)
        {
            FetchDurWarnings();
            var headerReasonID = getHeaderReason(durGrid);
            if (SessionUserType != Constants.UserCategory.POB_SUPER
                && SessionUserType != Constants.UserCategory.PHYSICIAN_ASSISTANT_SUPERVISED
                && headerReasonID == -1
                && durType == DURWarningType.CUSTOM_MED)
            {
                headerReasonID = 0;
            }

            foreach (GridDataItem durLineItem in durGrid.Items)
            {

                var ddlReasons = durLineItem["Reason"].FindControl("ddlReasons") as RadComboBox;
                int reasonID;

                var durIndex = durType != DURWarningType.CUSTOM_MED ?Convert.ToInt32(durLineItem.GetDataKeyValue(DURIdKey)):0;
                var rxIds = string.IsNullOrWhiteSpace(taskingRxId) ? GetScreenedRxIds(durType, durIndex) : new List<Guid> {Guid.Parse(taskingRxId) };

                var warningText = durType != DURWarningType.CUSTOM_MED
                    ? Convert.ToString(durLineItem.GetDataKeyValue(FullWarningTextKey))
                    : Constants.DURMessages.DURCustomMedWarning;

                reasonID = ComputeReasonIDAndUpdateRxToProviderList(ddlReasons?.SelectedIndex, Allscripts.ePrescribe.ExtensionMethods.ExtensionMethods.ToIntOrZero(ddlReasons.SelectedValue), headerReasonID, rxToProviderList, rxIds);
                
                var lineNumber = 1; //always = 1 in RX_DETAIL. This has to match to show up in POB DUR report

                List<Guid> list = durList;
                foreach (var rxId in rxIds)
                {
                    if (rxId == Guid.Empty) continue;
                    var warningData = DURMedispanUtils.GetWarningText(durWarWarnings, ActiveMedications, rxId, durIndex, durType);

                    list.Add(rxId);
                    warningsToSave.Add(new DurWarningSaveRecord(rxId, lineNumber, (int)durType, 0, warningText, "Y", 0, reasonID, warningData, durIndex));
                }
            }

        }

        /// <summary>
        /// To Compute Which Reason to Select When Multiple Reasons Are Provided. 
        /// Also Update rxToProviderList When Reason Is Send To Provider
        /// </summary>
        /// <returns>DUR Reason ID to Consider</returns>
        public int ComputeReasonIDAndUpdateRxToProviderList(int? selectedIndex, int selectedReasonID, int headerReasonID, ArrayList rxToProviderList, List<Guid> rxIDs)
        {
            // Reason -2 => Hardcoded 'Send to Provider'
            int reasonID;
            if (selectedIndex > 0) //Index > 0 => something selected 
            {
                reasonID = selectedReasonID; 
            }            
            else
            {
                reasonID = headerReasonID;
            }

            if(reasonID == -2)
            {
                rxToProviderList.AddRange(rxIDs);
            }

            return reasonID;
        }

        private void GatherWarningsAndInfoToWriteToDB(string rxId)
        {
            var rxToProviderList = new ArrayList();
            var durList = new List<Guid>();
            var warningsToSave = new List<DurWarningSaveRecord>();
            AddWarningsToSave(ref warningsToSave, grdParList, DURWarningType.PAR, ref rxToProviderList, ref durList, rxId);
            AddWarningsToSave(ref warningsToSave, grdIntList, DURWarningType.INTERACTION, ref rxToProviderList, ref durList, rxId);
            AddWarningsToSave(ref warningsToSave, grdDupList, DURWarningType.DUPLICATE_THERAPY, ref rxToProviderList, ref durList, rxId);
            AddWarningsToSave(ref warningsToSave, grdDoseList, DURWarningType.DOSAGE, ref rxToProviderList, ref durList, rxId);
            AddWarningsToSave(ref warningsToSave, grdAlcList, DURWarningType.ALCOHOL_INTERACTION, ref rxToProviderList, ref durList, rxId);
            AddWarningsToSave(ref warningsToSave, grdFoodList, DURWarningType.FOOD_INTERACTION, ref rxToProviderList, ref durList, rxId);
            AddWarningsToSave(ref warningsToSave, grdCustList, DURWarningType.CUSTOM_MED, ref rxToProviderList, ref durList, rxId);
            RxDurRevMultiSelectApiController.SaveDurWarnings(new SaveDurRequest(rxToProviderList, durList, warningsToSave), new ImpactDurWraper());
        }

        #endregion

        protected void btnHndReload_Click(object sender, EventArgs e)
        {
            PageState.Remove(Constants.SessionVariables.DURCheckResponse);
            clearDataGridDataSources();
        }
        private void FetchDurWarnings()
        {
            if (durWarWarnings == null)
                durWarWarnings = RxDurRevMultiSelectApiController.DurWarnings(new eRxWebAppCodeWrapper()).Payload;
        }
    }

}
