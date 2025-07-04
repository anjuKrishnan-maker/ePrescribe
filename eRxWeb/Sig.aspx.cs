using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.Data;
using Allscripts.ePrescribe.Objects;
using Allscripts.Impact;
using eRxWeb.AppCode;
using eRxWeb.State;
using Medication = Allscripts.Impact.Medication;
using Rx = Allscripts.Impact.Rx;
using ScriptMessage = Allscripts.Impact.ScriptMsg.ScriptMessage;
using Allscripts.ePrescribe.DatabaseSelector;
using Allscripts.ePrescribe.ExtensionMethods;
using Allscripts.ePrescribe.Medispan.Clinical.Model.Request;
using Allscripts.ePrescribe.Medispan.Clinical.Model.Response;
using Allscripts.ePrescribe.Medispan.Clinical.Model.Settings;
using Allscripts.Impact.ePrescribeSvc;
using Allscripts.Impact.Interfaces;
using Allscripts.Impact.Tasks;
using Allscripts.Impact.Tasks.Interfaces;
using Patient = Allscripts.Impact.Patient;
using eRxWeb.AppCode.Interfaces;
using ConfigKeys = Allscripts.Impact.ConfigKeys;
using eRxWeb.AppCode.Tasks;
using Module = Allscripts.Impact.Module;
using Prescription = Allscripts.Impact.Prescription;
using eRxWeb.AppCode.PptPlusBPL;
using FormularyStatus = Allscripts.Impact.FormularyStatus;
using eRxWeb.ServerModel;
using System.Web.Script.Serialization;
using Allscripts.ePrescribe.Data.CommonComponent;
using Allscripts.Impact.Utilities;
using Allscripts.ePrescribe.Shared.Logging;
using Allscripts.ePrescribe.Objects;

namespace eRxWeb
{
    public partial class Sig : BasePage
    {
        #region Member Fields
        private static readonly ILoggerEx logger = LoggerEx.GetLogger();

        //This is the sig id that is associated with all the free text sigs. 
        public static string FREETEXT_SIG_ID = "00000000-0000-0000-0000-000000000000";
        CookieContainer ccContainer;
        string packsize = string.Empty;
        string packageDescription = string.Empty;
        string DosageFormCode = String.Empty;
        private bool _isCSMed = false;
        private bool _allowMDD = false;
        private bool _MDDCSMedOnly = false;
        private ScriptMessage sm;

        #endregion

        #region Page Events and Event Handlers

        protected void Page_Load(object sender, EventArgs e)
        {
            btnCheckRegistry.OnClientClick = "OpenNewWindow" + "('" + PageState.GetStringOrEmpty("STATEREGISTRYURL") + "')";
            InputValidation.SetRegexValidator(ref revtxtQuantity, RegularExpressions.MedicationQuantityLimit, Constants.ErrorMessages.MedicationQuantityInvalid);

            if (!Convert.ToBoolean(PageState[Constants.SessionVariables.IsPatientHealthPlanDisclosed]))
            {
                hiddenPharmacyNote.Value = Constants.PharmacyNoteText.CASH_PATIENT;
            }
            else
            {
                hiddenPharmacyNote.Value = string.Empty;
            }

            var rx = SetRx();
            var workflow = GetWorkflow();
            if (workflow == Constants.PrescriptionTaskType.RXCHG)
            {
                if (MasterPage.RxTask != null)
                {
                    MasterPage.RxTask.TaskType = Constants.PrescriptionTaskType.RXCHG;
                }
                PageState["Tasktype"] = Constants.PrescriptionTaskType.RXCHG;
            }

            adControl.FeaturedModule = Module.ModuleType.DELUXE;
            setMDD();
            switch (SessionLicense.GetFeatureStatus(Constants.DeluxeFeatureType.iFC))
            {
                case Constants.DeluxeFeatureStatus.On:

                    if (string.IsNullOrEmpty(rx.NDCNumber))
                    {
                        if (Request.QueryString["RxID"] != null)
                        {
                            string rxID = Request.QueryString["RxID"].ToString();
                            foreach (Rx scriptPadRx in base.ScriptPadMeds)
                            {
                                if (string.Compare(rxID, scriptPadRx.RxID, true) == 0)
                                {
                                    if (IsLexicompEnabled)
                                    {
                                        string lexicompURL = string.Format(ConfigKeys.LexicompAdminDosageURL, scriptPadRx.NDCNumber);
                                        iFCLink.Attributes["onclick"] = "connectToiFC('" + lexicompURL + "');";
                                    }
                                    else
                                    {
                                        string factsComparisonsURL = string.Format(ConfigKeys.FactsComparisonsAdminDosageURL, scriptPadRx.NDCNumber);
                                        iFCLink.Attributes["onclick"] = "connectToiFC('" + factsComparisonsURL + "');";
                                    }
                                }
                            }
                        }
                        else
                        {
                            iFCLink.Style["display"] = "none";
                        }
                    }
                    else
                    {
                        if (IsLexicompEnabled)
                        {
                            string lexicompURL = string.Format(ConfigKeys.LexicompAdminDosageURL, rx.NDCNumber);
                            iFCLink.Attributes["onclick"] = "connectToiFC('" + lexicompURL + "');";
                        }
                        else
                        {
                            string factsComparisonsURL = string.Format(ConfigKeys.FactsComparisonsAdminDosageURL, rx.NDCNumber);
                            iFCLink.Attributes["onclick"] = "connectToiFC('" + factsComparisonsURL + "');";
                        }
                    }
                    break;
                case Constants.DeluxeFeatureStatus.Disabled:
                case Constants.DeluxeFeatureStatus.Off:
                    mpeAd.TargetControlID = "iFCLink";
                    break;
                case Constants.DeluxeFeatureStatus.Hide:
                    iFCLink.Style["display"] = "none";
                    break;
            }

            ucMedicationHistoryCompletion.CurrentPage = Constants.PageNames.SIG;
            base.SetSingleClickButton(btnAddReview);
            base.SetSingleClickButton(btnChooseDestination);

            ccContainer = new CookieContainer();

            // add client side event handlers to controls
            var maxSigText = 1000;
            txtFreeTextSig.Attributes.Add("onkeydown", $"return LimitInput(this, {maxSigText}, event);");
            txtFreeTextSig.Attributes.Add("onkeyup", $"return LimitInput(this, {maxSigText}, event);");
            txtFreeTextSig.Attributes.Add("onpaste", $"return LimitPaste(this, {maxSigText}, event);");
            txtFreeTextSig.Attributes.Add("onchange", $"return LimitChange(this, {maxSigText}, event);");

            txtPharmComments.Attributes.Add("onkeydown", "return LimitInput(this, 210, event);");
            txtPharmComments.Attributes.Add("onkeyup", "return LimitInput(this, 210, event);");
            txtPharmComments.Attributes.Add("onpaste", "return LimitPaste(this, 210, event);");
            txtPharmComments.Attributes.Add("onchange", "return LimitChange(this, 210, event);");

            txtMDD.Attributes.Add("onkeydown", $"if(!AllowMDDChange({maxSigText}, event, {txtMDD.ClientID}, {txtFreeTextSig.ClientID}, {lblMDDError.ClientID})) return false;");
            txtMDD.Attributes.Add("onkeyup", "UpdateFreeTextSIG()");
            txtMDD.Attributes.Add("onpaste", "return false");

            LstSig.DataBound += new EventHandler(LstSig_DataBound);

            this.ucCSMedRefillRequestNotAllowed.OnPrintRefillRequest += new EventHandler(ucCSMedRefillRequestNotAllowed_OnPrintRefillRequest);
            this.ucCSMedRefillRequestNotAllowed.OnContactProvider += new EventHandler(ucCSMedRefillRequestNotAllowed_OnContactProvider);

            // subscribe the OnDigitalSigning event handler
            this.ucEPCSDigitalSigning.OnDigitalSigningComplete += new Controls_EPCSDigitalSigning.DigitalSigningCompleteHandeler(ucEPCSDigitalSigning_OnDigitalSigningComplete);

            // inject javascript to initialize orig sig, days supply, etc.
            ClientScript.RegisterStartupScript(this.GetType(), "InitializeTrackingVariables", "InitializeTrackingVariables();", true);

            // add client side event handlers to controls
            rdgPrefer.Attributes.Add("onclick", "ToggleSIGs('P');");
            rdbAllSig.Attributes.Add("onclick", "ToggleSIGs('A');");
            rdbFreeTextSig.Attributes.Add("onclick", "ToggleSIGs('F');");

            if (!Page.IsPostBack)//484251-added session variable to avoid page refresh after returning from Patient Edu window       
            {
                SetWorkflowControls(workflow);

                if (Request.QueryString.HasKeys() && Request.QueryString["from"] == Constants.PageNames.APPROVE_REFILL_TASK)
                {
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), "RefreshPatientHeader", $"RefreshPatientHeader('{PageState.GetStringOrEmpty(Constants.SessionVariables.PatientId)}'); ", true);
                }

                if (Session[Constants.SessionVariables.TaskScriptMessageId] != null && !isCSRefReq())
                {
                    defaultResponseType.Value = "A";
                    lblRefills.Text = "Total number of dispenses approved: ";  // if pharmacy refill workflow, change label
                    RangeValidatorRefill.MinimumValue = "1";
                    RangeValidatorRefill.ErrorMessage = "Dispenses must be a whole number between 1 and 99";

                }

                if ((Session["SelectedCoverageAndCopay"] == null || Session["SelectedCoverageAndCopay"].ToString().Trim() == string.Empty)
                    && (rx.FormularyStatus == null || rx.FormularyStatus.ToString() == string.Empty))
                {
                    panelCovCopayHeader.Visible = false;
                    panelCovCopay.Visible = false;
                }
                else
                {
                    panelCovCopayHeader.Visible = true;
                    panelCovCopay.Visible = true;

                    if (!string.IsNullOrEmpty(rx.FormularyStatus))
                    {
                        bool isOTC = false;
                        if (!string.IsNullOrEmpty(rx.IsOTC))
                        {
                            isOTC = rx.IsOTC == "Y";
                        }

                        string imgPath = string.Empty;
                        string toolTip = string.Empty;

                        MedicationSearchDisplay.GetFormularyImagePathWithTooltip(Convert.ToInt32(CurrentRx.FormularyStatus), isOTC, out imgPath, out toolTip);

                        imgFormularyStatus.ImageUrl = imgPath;
                        imgFormularyStatus.ToolTip = toolTip;
                    }

                    lblLevelOfPreferedness.Text = rx.LevelOfPreferedness != null ? rx.LevelOfPreferedness : string.Empty;
                    lblMedName.Text = getMedName();
                    lblCovCopay.Text = Session["SelectedCoverageAndCopay"] != null ? Session["SelectedCoverageAndCopay"].ToString() : string.Empty;
                }

                //EAK Added for Pharmacy Tasks
                if (Session[Constants.SessionVariables.TaskScriptMessageId] != null && workflow != Constants.PrescriptionTaskType.RXCHG)
                {
                    ScriptMessage sm = new ScriptMessage(Session[Constants.SessionVariables.TaskScriptMessageId].ToString(), Session["LICENSEID"].ToString(), Session["USERID"].ToString(), base.DBID);

                    if (sm != null)
                    {
                        if (sm.DBPharmacyID == null || sm.DBPharmacyID == "")
                        {
                            lblSigErrorMsg.Text = "Unfortunately this Refill Request does not have a proper pharmacy attached to it.  NCPDP:" + sm.DBPharmacyNetworkID + "  Please contact <a href='http://erxnowhelp.allscripts.com/contact.asp'>Veradigm Support</a>.";
                            lblSigErrorMsg.Visible = true;
                            ScriptMessage.RejectMessage(sm.DBScriptMessageID, "", "NCPDPID not in dictionary", SessionUserID, SessionLicenseID, string.Empty, base.ShieldSecurityToken, base.SessionSiteID, base.DBID);
                            btnChooseDestination.Enabled = false;
                            return;
                        }

                        // when coming to sig page from a cs refreq workflow we will be creating a new rx so we will use the sig info from the script message
                        if (!PageState.GetBooleanOrFalse(Constants.SessionVariables.IsCsRefReqWorkflow))
                        {
                            setRefillInfo(sm);
                        }

                        ucMessageHeader.Icon = Controls_Message.MessageType.INFORMATION;
                        string pharmPrefix = ($"{(MasterPage.ChangeRxRequestedMedCs != null ? "Change Rx" : "Renewal Request")} Pharmacy : ");
                        ucMessageHeader.MessageText = pharmPrefix + StringHelper.GetPharmacyName(
                            sm.PharmacyName,
                            sm.PharmacyAddress1,
                            sm.PharmacyAddress2,
                            sm.PharmacyCity,
                            sm.PharmacyState,
                            sm.PharmacyZip,
                            sm.PharmacyPhoneNumber);
                        ucMessageHeader.Visible = true;

                        int formularyStatus = 100;
                        int levelOfPreferredNess = 100;

                        if (!int.TryParse(rx.FormularyStatus, out formularyStatus))
                        {
                            formularyStatus = 100;
                        }

                        if (!int.TryParse(rx.LevelOfPreferedness, out levelOfPreferredNess))
                        {
                            levelOfPreferredNess = 100;
                        }

                        btnAddReview.Visible = false;
                        btnPatientEdu.Visible = false;
                        btnChangeMed.Visible = false;

                        if (Session[Constants.SessionVariables.IsCsRefReqWorkflow] != null)
                        {
                            ucMessageRxHeader.Icon = Controls_Message.MessageType.INFORMATION;
                            ucMessageRxHeader.MessageText =
                                "Rx Detail : " + StringHelper.GetRxDetails(
                                    sm.DispensedRxDrugDescription,
                                    sm.DispensedRxSIGText,
                                    sm.DispensedRxQuantity,
                                    sm.DispensedDaysSupply,
                                    sm.DispensedRxRefills,
                                    sm.DispensedDaw,
                                    sm.DispensedCreated,
                                    sm.DispensedDateLastFill,
                                    sm.DispensedRxNotes);
                            ucMessageRxHeader.Visible = true;
                            btnAddReview.Visible = btnAddReview.Enabled = true;
                            btnChooseDestination.Enabled = btnChooseDestination.Visible = false;
                        }
                        else if (MasterPage.ChangeRxRequestedMedCs != null)
                        {
                            ucMessageRxHeader.Icon = Controls_Message.MessageType.INFORMATION;
                            ucMessageRxHeader.MessageText = "Rx Detail : " + MasterPage.ChangeRxRequestedMedCs.RxDetails;
                            ucMessageRxHeader.Visible = true;
                            btnAddReview.Visible = btnAddReview.Enabled = true;
                            btnChooseDestination.Enabled = btnChooseDestination.Visible = false;
                        }
                        else
                        {
                            btnChooseDestination.Text = "Send to Pharmacy";
                            btnChooseDestination.Enabled = true;
                        }
                    }
                    else
                    {
                        lblSigErrorMsg.Text = "Could not find refill request. Please contact <a href='http://erxnowhelp.allscripts.com/contact.asp'>Veradigm Support</a>.";
                        lblSigErrorMsg.Visible = true;
                        btnChooseDestination.Enabled = false;
                        return;
                    }
                }

                if ((Request.QueryString["CameFrom"] != null) && (Request.QueryString["CameFrom"] == Constants.PageNames.FREE_FORM_DRUG || Request.QueryString["CameFrom"] == "FreeForm"))// Comming from Free Form Drug page
                {
                    rdbFreeTextSig.Checked = true;
                    rdbAllSig.Checked = false;
                    rdgPrefer.Checked = false;
                    rdbAllSig.Enabled = false;
                    rdgPrefer.Enabled = false;
                    if (Request.QueryString["CameFrom"] != "FreeForm")
                    {
                        HttpContext.Current.RewritePath(Request.Path, Request.PathInfo, ""); // remove the query string
                        Session["CameFrom"] = Constants.PageNames.FREE_FORM_DRUG;
                    }
                }

                //// This code is for Add Comments...Comming from Review History Page
                ////-------------------------------------------------------------------------------------------------------------

                if (Request.QueryString["RxID"] != null)  // Comming for Review History PAge
                {
                    string RxID = Request.QueryString["RxID"].ToString();
                    setRxInfo(RxID);
                    rx = base.CurrentRx;

                    //Start Added this code on July 04 2006 ... to display these values got from database
                    if (rx.Quantity != 0)
                    {
                        txtQuantity.Text = Convert.ToDouble(rx.Quantity).ToString();
                    }

                    if (rx.Refills != 0)
                    {
                        txtRefill.Text = rx.Refills.ToString();
                    }

                    chkDAW.Checked = rx.DAW;

                    if (rx.DaysSupply != 0)
                    {
                        txtDaysSupply.Text = rx.DaysSupply.ToString();
                    }

                    if (_allowMDD)
                    {
                        txtMDD.Text = TrimValue(rx.MDD);
                    }

                    if (!String.IsNullOrEmpty(rx.SigID))
                    {
                        string sigID = rx.SigID;

                        if (rx.SigTypeId == (int)SigTypeEnum.SigTypeFreeForm)
                        {
                            if (rx.SigText != null)
                            {
                                txtFreeTextSig.Text = rx.SigText;
                                rdbFreeTextSig.Checked = true;
                                // RDR - ID# 2589 - Unchecked other radio buttons with in the group .
                                rdbAllSig.Checked = false;
                                rdgPrefer.Checked = false;

                                if (rx.DDI != null)
                                {
                                    string med_ddi = rx.DDI.Trim();

                                    //if the med ddi is not present, then the prescription is a free text drug,
                                    //then disable the prefer sig and all sigs options because they don't apply.
                                    if (string.IsNullOrEmpty(med_ddi))
                                    {
                                        rdbAllSig.Enabled = false;
                                        rdgPrefer.Enabled = false;
                                    }
                                }
                            }
                        }
                    }
                }

                if (rx.DDI != null) //coming from freeform drug
                {
                    DataSet ds = Allscripts.Impact.Medication.Load(rx.DDI, null, base.DBID);

                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        string gpi = ds.Tables[0].Rows[0]["GPI"].ToString();
                        string RouteOfAdmincode = ds.Tables[0].Rows[0]["RouteOfAdminCode"].ToString();
                        DosageFormCode = ds.Tables[0].Rows[0]["DosageFormCode"].ToString();
                        string ProviderId = base.Session[Constants.SessionVariables.UserId] != null? base.Session[Constants.SessionVariables.UserId].ToString() : System.Guid.Empty.ToString();
                        PrefSigObjectDataSource.SelectParameters.Add("gpi", gpi);
                        PrefSigObjectDataSource.SelectParameters.Add("routeOfAdminCode", RouteOfAdmincode);
                        PrefSigObjectDataSource.SelectParameters.Add("dosageFormCode", DosageFormCode);
                        PrefSigObjectDataSource.SelectParameters.Add("providerID", ProviderId);
                        LstPreferedSig.DataSourceID = "PrefSigObjectDataSource";
                        PrefSigObjectDataSource.Select();
                    }

                    if (!String.IsNullOrEmpty(rx.SigID))
                    {
                        string sigID = rx.SigID;

                        if (rx.SigTypeId == (int)SigTypeEnum.SigTypeFreeForm)
                        {
                            if (rx.SigText != null)
                            {
                                txtFreeTextSig.Text = rx.SigText;
                                rdbFreeTextSig.Checked = true;
                                // RDR - ID# 2589 - Unchecked other radio buttons with in  the group .
                                rdbAllSig.Checked = false;
                                rdgPrefer.Checked = false;
                                string med_ddi = rx.DDI;

                                //if the med ddi is not present, then the prescription is a free text drug,
                                //then disable the prefer sig and all sigs options because they don't apply.
                                if (string.IsNullOrEmpty(med_ddi))
                                {
                                    rdbAllSig.Enabled = false;
                                    rdgPrefer.Enabled = false;
                                }
                            }
                        }
                    }


                    //******** Added code on July 31 to make sure the patiend edu is not shown when the valeu sare not there
                    DataSet dsPatientEdu = CHPatient.GetPatientEducation(rx.DDI, Session["LICENSEID"].ToString(), Session["USERID"].ToString(), base.DBID);

                    if (dsPatientEdu.Tables[0].Rows.Count <= 0)
                    {
                        btnPatientEdu.Visible = false;
                    }
                }
                else
                {
                    Session["SIGTEXT"] = rx.SigText;
                }

                if (Request.FilePath.ToLower().Contains(Constants.PageNames.SELECT_MEDICATION.ToLower()))
                {
                    if (rx.Quantity != 0)
                    {
                        txtQuantity.Text = Convert.ToDouble(rx.Quantity).ToString();
                    }

                    if (rx.Refills != 0)
                    {
                        txtRefill.Text = rx.Refills.ToString();
                    }

                    chkDAW.Checked = rx.DAW;

                    if (rx.DaysSupply != 0)
                    {
                        txtDaysSupply.Text = rx.DaysSupply.ToString();
                    }
                }

                if (!string.IsNullOrEmpty(rx.DDI))
                {
                    btnPatientEdu.Disabled = false;

                }
                else
                {
                    btnPatientEdu.Disabled = true;

                }

                //check for non- pill meds : 02/06/06
                // this is to display for bottle /custom bottle if there.
                if (rx.DDI != null)
                {
                    DataSet ds = Allscripts.Impact.Medication.GetPackagesForMedication(rx.DDI, DosageFormCode, base.DBID);
                    DataTable dt = ds.Tables["Package"];
                    if (dt.Rows.Count > 1)
                    {
                        DataView dataView = dt.DefaultView;
                        dataView.Sort = "PackageSize";

                        ddlCustomPack.DataSource = dataView;
                        ddlCustomPack.DataBind();
                        Session["Package"] = dt;
                        //Added by AKS on Aug 11th 2006 commented aug 18
                        if (packsize != string.Empty && ddlCustomPack.Items.FindByValue(packsize) != null)
                        {
                            ddlCustomPack.Items.FindByValue(packsize).Selected = true;
                        }
                        else if (packageDescription != string.Empty && ddlCustomPack.Items.FindByText(packageDescription) != null)
                        {
                            ddlCustomPack.Items.FindByText(packageDescription).Selected = true;
                        }
                    }
                    else
                    {
                        pnlNonPillMed.Visible = false;
                    }
                }
                else
                {
                    pnlNonPillMed.Visible = false;
                }

                //--------------------------------------------------------------------------------------------------------------
                if (Session["PATIENTID"] == null)
                {
                    btnChooseDestination.Enabled = false;  //  AKS SEP 18th  2006
                }
                //dhiraj 26 oct 06
                //for controlledsubstance drug,send To Admin and Choose Pharmacy button should be disable
                //only we can take a print out.
                if (!string.IsNullOrEmpty(rx.ControlledSubstanceCode))
                {
                    string ControlledSubstanceCode = rx.ControlledSubstanceCode;

                    if (ControlledSubstanceCode.Trim() == "2")
                    {
                        txtRefill.Text = "0";
                        rvDaysSupply.MinimumValue = "1";
                        rvDaysSupply.MaximumValue = "90";
                        rvDaysSupply.ErrorMessage = "For Schedule II drugs, days supply must be a whole number between 1 and 90.";
                        RangeValidatorRefill.MaximumValue = "0";
                        RangeValidatorRefill.ErrorMessage = "For Schedule II drugs, refills must be 0.";
                        _isCSMed = true;
                    }
                    else if (ControlledSubstanceCode.Trim() == "3" || ControlledSubstanceCode.Trim() == "4" || ControlledSubstanceCode.Trim() == "5")
                    {
                        int daysupply = (DateTime.Now.AddMonths(6) - DateTime.Now.Date).Days;
                        rvDaysSupply.MinimumValue = "1";
                        rvDaysSupply.MaximumValue = daysupply.ToString();
                        rvDaysSupply.ErrorMessage = "For Schedule III, IV and V drugs, days supply must be a whole number between 1 and " + daysupply.ToString() + ".";
                        RangeValidatorRefill.MaximumValue = "5";
                        RangeValidatorRefill.ErrorMessage = "For Schedule III, IV and V drugs, refills must be a whole number between 0 and 5.";
                        _isCSMed = true;
                    }
                    else
                    {
                        _isCSMed = false;
                    }
                }
                else
                {
                    _isCSMed = false;
                }

                setMDD();

                Parameter pddi = SigAllObjDataSource.SelectParameters["DDI"];

                if (pddi != null)
                {
                    SigAllObjDataSource.SelectParameters.Remove(pddi);
                }

                SigAllObjDataSource.SelectParameters.Add("DDI", base.CurrentRx.DDI);

                pddi = LatinSigObjDataSource.SelectParameters["ddi"];

                if (pddi != null)
                {
                    LatinSigObjDataSource.SelectParameters.Remove(pddi);
                }

                LatinSigObjDataSource.SelectParameters.Add("ddi", base.CurrentRx.DDI);

                toggleSIGPanels();

                txtPharmComments.Text = GetPharmacyNotes(
                        Request.QueryString["Mode"],
                        PageState.GetStringOrEmpty(Constants.SessionVariables.PracticeState),
                        MasterPage.RxTask,
                        rx,
                        new MassOpiateMessage(),
                        PageState.GetStringOrEmpty(Constants.SessionVariables.GPI),
                        PageState.GetBooleanOrFalse(Constants.SessionVariables.HasExpiredDEA),
                        PageState.Cast(Constants.SessionVariables.DeaSchedulesAllowed, new List<string>())
                    );
            }

            var med = rx.IsFreeFormMed ? rx.MedicationName : getMedName();

            if (med != string.Empty)
            {
                lblMedInfo.Text = "Choose or write a SIG for " + med + ":";
            }
            else
            {
                lblMedInfo.Text = "Choose or write a SIG:";
            }

            if (string.IsNullOrEmpty(rx.DDI))
            {
                ddlUnit.Visible = true;
            }

            //Check for if editing SIG info --subhasis       
            if (Request.QueryString["Mode"] == "Edit")
            {
                if (Request.QueryString["To"] != null && !Request.QueryString["To"].Equals(Constants.PageNames.SCRIPT_PAD, StringComparison.OrdinalIgnoreCase))
                {
                    btnAddReview.Text = "Save";
                    btnAddReview.ToolTip = "Save Sig and return to previous page";
                }
                else
                {
                    btnAddReview.Text = "Save & Review";
                    btnAddReview.ToolTip = "Save Sig and Review";
                }

                btnChooseDestination.Visible = false;
            }

            if (Request.QueryString["Mode"] == "Edit")
            {
                btnChangeMed.Visible = false;
            }
        }

        public string GetPharmacyNotes(string mode, string stateAbbr, RxTaskModel rxTask, Rx rx, IMassOpiateMessage massOpiateMessage,
            string GPI, bool hasExpiredDEA, List<string> allowedSchedules)
        {
            if (mode == "Edit")
            {
                return rx.Notes;
            }
            if (stateAbbr == "MA")
            {
                if (rxTask != null)
                {
                    return massOpiateMessage.GenerateMassOpiateMessage(rxTask.Rx.ControlledSubstanceCode, rxTask.Notes, stateAbbr, GPI, hasExpiredDEA, allowedSchedules);
                }
                else //NewRx
                {
                    return massOpiateMessage.GenerateMassOpiateMessage(rx.ControlledSubstanceCode, null, stateAbbr, GPI, hasExpiredDEA, allowedSchedules);
                }
            }
            else
            {
                return "";
            }
        }

        private void SetWorkflowControls(Constants.PrescriptionTaskType workflow)
        {
            if (workflow == Constants.PrescriptionTaskType.RXCHG)
            {
                btnChangeMed.Text = "Select Med";
                btnChooseDestination.Text = "Approve Request";
                btnAddReview.Visible = false;
                btnPatientEdu.Visible = false;
                btnChangeMed.Visible = true;
                btnChangeMed.Enabled = PageState[Constants.SessionVariables.PatientId] != null;
                lblRefills.Text = "Total Dispenses: ";
                RangeValidatorRefill.MinimumValue = "1";
                RangeValidatorRefill.ErrorMessage = "Dispenses must be a whole number between 1 and 99";
                SetRefillInfo(CurrentRx);
                SigAspx.CheckAndSetControlsForEpcs(CanTryEPCS, CurrentRx.ControlledSubstanceCode, btnChooseDestination, ucMessageHeader);
            }
        }

        private Constants.PrescriptionTaskType GetWorkflow()
        {
            var workflowType = Request.QueryString["tasktype"];
            string workflow = workflowType;
            if ((MasterPage.RxTask != null && MasterPage.RxTask.TaskType == Constants.PrescriptionTaskType.RXCHG) ||
                    workflow == "RXCHG" ||
                    PageState.Cast(Constants.SessionVariables.TaskType, Constants.PrescriptionTaskType.DEFAULT) == Constants.PrescriptionTaskType.RXCHG
                )
            {
                return Constants.PrescriptionTaskType.RXCHG;
            }

            return Constants.PrescriptionTaskType.DEFAULT;
        }

        private Rx SetRx()
        {
            var reqRx = Request.QueryString["reqRx"];
            int reqIndex;
            var smId = Request.QueryString["smid"];
            string messageIDSessionPrior = smId;
            //Coming from approve task screen wanting to change a ChangeRxMed
            if (reqRx != null && smId != null && int.TryParse(reqRx, out reqIndex))
            {
                Rx rx = null;
                if (!Page.IsPostBack)
                {
                    PageState[Constants.SessionVariables.TaskScriptMessageId] = smId;
                    var sm = new ScriptMessage(smId, SessionLicenseID, SessionUserID, DBID);
                    rx = new Rx(RequestedRx.GetRequestedRxByOrdinal(sm.XmlMessage, reqIndex));
                    MasterPage.RxTask = new RxTaskModel
                    {
                        ScriptMessage = sm,
                        ScriptMessageGUID = messageIDSessionPrior,
                        RequestedRxIndexSelected = reqIndex,
                        RequestedRx = RequestedRx.GetRequestedRxByOrdinal(sm.XmlMessage, reqIndex),
                        Rx = rx,
                        UserId = SessionUserID.ToGuid(),
                        DbId = DBID,
                        LicenseId = SessionLicenseID.ToGuid(),
                        SiteId = SessionSiteID,
                        ShieldSecurityToken = ShieldSecurityToken,
                        ExternalFacilityCd = PageState.GetStringOrEmpty(Constants.SessionVariables.ExternalFacilityCd),
                        ExternalGroupID = PageState.GetStringOrEmpty(Constants.SessionVariables.ExternalGroupID),
                        UserType = SessionUserType,
                        DelegateProviderId = SessionDelegateProviderID.ToGuidOr0x0()
                    };
                }

                PageState[Constants.SessionVariables.RxList] = new ArrayList { MasterPage.RxTask.Rx };
            }
            return CurrentRx;
        }

        private void setMDD()
        {
            _allowMDD = PageState.GetBooleanOrFalse("ALLOWMDD");
            if (_allowMDD)
            {
                _MDDCSMedOnly = PageState.GetBooleanOrFalse("CSMEDSONLY");
            }
            if (_allowMDD)
            {
                if (_MDDCSMedOnly)
                {
                    if (!string.IsNullOrWhiteSpace(base.CurrentRx.ControlledSubstanceCode))
                    {
                        SetMDDFieldsVisablity(true);
                    }
                    else
                    {
                        SetMDDFieldsVisablity(false);
                    }
                }
                else
                {
                    SetMDDFieldsVisablity(true);
                }
            }
            else
            {
                SetMDDFieldsVisablity(false);
            }
        }

        private void SetMDDFieldsVisablity(bool visable)
        {
            lblMDD.Visible = visable;
            lblPerDay.Visible = visable;
            txtMDD.Visible = visable;
        }

        protected void Page_LoadComplete(object sender, EventArgs e)
        {
            ((PhysicianMasterPage)Master).hideTabs();
        }

        protected void Page_PreRenderComplete(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["ISCSREGISTRYCHECKREQ"] == null ||
                    string.IsNullOrWhiteSpace(Session["ISCSREGISTRYCHECKREQ"].ToString()) &&
                    (string.IsNullOrEmpty(Session["STATEREGISTRYURL"].ToString()) ||
                     (string.CompareOrdinal(Session["ShowCSRegistry"].ToString(), "Y") != 0)))
                {
                    chkRegistryChecked.Visible = false;
                }
                else
                {
                    if (Session[Constants.SessionVariables.TaskScriptMessageId] == null || string.IsNullOrWhiteSpace(Session[Constants.SessionVariables.TaskScriptMessageId].ToString()))
                    {
                        chkRegistryChecked.Visible = false;
                    }
                    else
                    {
                        if (_isCSMed && (Session[Constants.SessionVariables.IsCsRefReqWorkflow] == null && MasterPage.ChangeRxRequestedMedCs == null))
                        {
                            chkRegistryChecked.Visible = CsMedUtil.ShouldShowCsRegistryControls(PageState);
                        }
                    }
                }
            }

            btnCheckRegistry.Visible = chkRegistryChecked.Visible;
        }

        void LstSig_DataBound(object sender, EventArgs e)
        {
            if (Session["SIGTEXT"] == null)
            {
                rdbAllSig.Checked = false;
                rdbFreeTextSig.Checked = false;
                rdgPrefer.Checked = true;
                toggleSIGPanels();
            }
            else if (sigType.Value == "A")
            {
                ListItem li = LstSig.Items.FindByText(Session["SIGTEXT"].ToString());

                if (li == null)
                {
                    //I suppose we should change this to a freetext
                    rdbAllSig.Checked = false;
                    rdgPrefer.Checked = false;
                    rdbFreeTextSig.Checked = true;
                    txtFreeTextSig.Text = Session["SIGTEXT"].ToString();
                }
                else
                {
                    LstSig.SelectedValue = li.Value;
                }
            }

            if (LstSig.Items.Count == 1)
            {
                LstSig.SelectedIndex = 0;
            }

            toggleSIGPanels();
        }

        protected void LstLatinSig_SelectedIndexChanged(object sender, EventArgs e)
        {
            SigAllObjDataSource.Select();
        }

        protected void LstPreferedSig_PreRender(object sender, EventArgs e)
        {
            var sigText = PageState.GetStringOrEmpty("SIGTEXT");

            if (!string.IsNullOrWhiteSpace(sigText))
            {
                var selectedSig = LstPreferedSig.Items.FindByText(sigText);

                if (selectedSig != null && Session[Constants.SessionVariables.IsCsRefReqWorkflow] == null && MasterPage.ChangeRxRequestedMedCs == null)
                {
                    rdbFreeTextSig.Checked = false;
                    rdbAllSig.Checked = false;
                    rdgPrefer.Checked = true;
                    toggleSIGPanels();
                    selectedSig.Selected = true;
                }
            }
        }

        protected void ddlCustomPack_PreRender(object sender, EventArgs e)
        {
            string package = string.Empty;

            if (Session["PackageSize"] != null && Session["PackageQuantity"] != null)
            {
                package = "[PZ=" + Session["PackageSize"].ToString() + "](PQ=" + Session["PackageQuantity"].ToString() + ")";
            }

            //if we already have a selected item, no need to reselect
            if (ddlCustomPack.SelectedItem == null)
            {
                if (package != string.Empty)
                {
                    if (ddlCustomPack.Items.FindByValue(package) != null)
                    {
                        ddlCustomPack.Items.FindByValue(package).Selected = true;
                    }
                }
            }
        }

        protected void PrefSigObjectDataSource_Selected(object sender, ObjectDataSourceStatusEventArgs e)
        {
            //for pharmacy task workflow, we should always show free form sig
            if (Session[Constants.SessionVariables.TaskScriptMessageId] == null)
            {
                if (((DataSet)e.ReturnValue)?.Tables[0].Rows.Count == 0)
                {
                    rdbAllSig.Checked = true;
                    rdgPrefer.Enabled = false;
                }

                if (((DataSet)e.ReturnValue)?.Tables[0].Rows.Count == 1)
                {
                    if (LstPreferedSig.Items.Count > 0)
                    {
                        LstPreferedSig.SelectedIndex = 0;
                    }
                }

                toggleSIGPanels();
            }
            else if (Session[Constants.SessionVariables.TaskScriptMessageId] != null && (Session[Constants.SessionVariables.IsCsRefReqWorkflow] != null || MasterPage.ChangeRxRequestedMedCs != null))
            {
                rdgPrefer.Enabled = true;
            }
            else
            {
                rdgPrefer.Enabled = false;
            }
        }

        protected void btnChangeMed_Click(object sender, EventArgs e)
        {
            if (MasterPage.RxTask != null && MasterPage.RxTask.TaskType == Constants.PrescriptionTaskType.RXCHG)
            {
                var sm = MasterPage.RxTask.ScriptMessage as ScriptMessage;
                if (sm != null)
                {
                    PageState[Constants.SessionVariables.From] = string.Format("{0}?smid={1}&reqRx={2}", Constants.PageNames.SIG, sm.DBScriptMessageID, MasterPage.RxTask.RequestedRxIndexSelected);
                    string parentPage = Constants.PageNames.APPROVE_REFILL_TASK;
                    if (SessionUserType == Constants.UserCategory.POB_REGULAR || SessionUserType == Constants.UserCategory.POB_SUPER || SessionUserType == Constants.UserCategory.POB_LIMITED)
                    {
                        parentPage = Constants.PageNames.PHARMACY_REFILL_SUMMARY;
                    }
                    Response.Redirect(Constants.PageNames.REDIRECT_TO_ANGULAR + "?componentName=" + Constants.PageNames.SELECT_MEDICATION + $"&componentParameters={JsonHelper.ConvertUrlParameters($"SearchText=" + CurrentRx.MedicationName.Trim() + "&from=" + parentPage + "&smid=" + PageState.GetStringOrEmpty(Constants.SessionVariables.TaskScriptMessageId) + "&tasktype=" + PageState.GetStringOrEmpty(Constants.SessionVariables.TaskType))}", true);
                }
            }
            Response.Redirect(Constants.PageNames.REDIRECT_TO_ANGULAR + "?componentName=" + Constants.PageNames.SELECT_MEDICATION + $"&componentParameters={JsonHelper.ConvertUrlParameters($"SearchText=" + CurrentRx.MedicationName.Trim())}", true);
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            //Added if condition for canceling the edit SIG info---subhasis
            if (Request.QueryString["Mode"] == "Edit")
            {
                Session.Remove("RxList");
                Session.Remove("NOTES");
                Session.Remove("Package");

                if (Request.QueryString["To"] != null)
                {
                    Response.Redirect(Constants.PageNames.UrlForRedirection(Request.QueryString["To"]));
                }
                else
                {
                    Response.Redirect(Constants.PageNames.SCRIPT_PAD);
                }
            }
            else
            {
                var redir = getRedirectBack();

                base.ClearPatientInfo();
                base.ClearMedicationInfo(false);

                PageState.Remove(Constants.SessionVariables.RxTask);
                Session.Remove(Constants.SessionVariables.TaskScriptMessageId);

                //Remove all the parameters before transfer to the new page. 
                HttpContext.Current.RewritePath(Request.Path, Request.PathInfo, "");
                if (!string.IsNullOrEmpty(redir))
                {
                    Response.Redirect(redir, true);
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
        }

        private string getRedirectBack()
        {
            string redir = string.Empty;
            if (Session[Constants.SessionVariables.TaskScriptMessageId] != null || MasterPage.RxTask != null)
            {
                if (SessionUserType == Constants.UserCategory.POB_REGULAR || SessionUserType == Constants.UserCategory.POB_SUPER)
                {
                    Response.Redirect($"{Constants.PageNames.PHARMACY_REFILL_DETAILS}" +
                                          $"?MessageID={PageState.GetStringOrEmpty(Constants.SessionVariables.TaskScriptMessageId)}" +
                                          $"&PatientID={PageState.GetStringOrEmpty(Constants.SessionVariables.PatientId)}" +
                                          $"&ProviderID={PageState.GetStringOrEmpty(Constants.SessionVariables.PhysicianId)}");
                }
                if (PageState[Constants.SessionVariables.PatientId] == null)
                {
                    redir = Constants.PageNames.DOC_REFILL_MENU;
                }
                else if (MasterPage.ChangeRxRequestedMedCs != null)
                {
                    ScriptMessage sm = new ScriptMessage(Session[Constants.SessionVariables.TaskScriptMessageId].ToString(), Session["LICENSEID"].ToString(), Session["USERID"].ToString(), base.DBID);

                    if (sm != null)
                    {
                        var pharmDetails = StringHelper.GetPharmacyName(
                            sm.PharmacyName,
                            sm.PharmacyAddress1,
                            sm.PharmacyAddress2,
                            sm.PharmacyCity,
                            sm.PharmacyState,
                            sm.PharmacyZip,
                            sm.PharmacyPhoneNumber);
                        var rxDetails = MasterPage.ChangeRxRequestedMedCs.RxDetails;
                        Response.Redirect(CsMedUtil.RedirectForCSMedChangeRx(pharmDetails, rxDetails, MasterPage.ChangeRxRequestedMedCs.RequestedRxDrugDescription, Session[Constants.SessionVariables.TaskScriptMessageId].ToString()));
                    }
                }
                else if (Session[Constants.SessionVariables.IsCsRefReqWorkflow] != null)
                {
                    ScriptMessage sm = new ScriptMessage(Session[Constants.SessionVariables.TaskScriptMessageId].ToString(), Session["LICENSEID"].ToString(), Session["USERID"].ToString(), base.DBID);

                    if (sm != null)
                    {
                        var pharmDetails = StringHelper.GetPharmacyName(
                            sm.PharmacyName,
                            sm.PharmacyAddress1,
                            sm.PharmacyAddress2,
                            sm.PharmacyCity,
                            sm.PharmacyState,
                            sm.PharmacyZip,
                            sm.PharmacyPhoneNumber);
                        var rxDetails = StringHelper.GetRxDetails(
                            sm.DispensedRxDrugDescription,
                            sm.DispensedRxSIGText,
                            sm.DispensedRxQuantity,
                            sm.DispensedDaysSupply,
                            sm.DispensedRxRefills,
                            sm.DispensedDaw,
                            sm.DispensedCreated,
                            sm.DispensedDateLastFill,
                            sm.DispensedRxNotes);
                        Response.Redirect(CsMedUtil.RedirectForCSMed(pharmDetails, rxDetails, sm.DBDrugDescription));
                    }
                }
                else
                {
                    redir = $"{Constants.PageNames.APPROVE_REFILL_TASK}?" +
                      $"PatientID={PageState.GetStringOrEmpty(Constants.SessionVariables.PatientId)}&" +
                      $"Patient={PageState.GetStringOrEmpty(Constants.SessionVariables.PatientLastName)},{PageState.GetStringOrEmpty(Constants.SessionVariables.PatientFirstName)}&" +
                      $"PhyId={PageState.GetStringOrEmpty(Constants.SessionVariables.PhysicianId)}";
                }
            }
            return redir;
        }
        private string getRedirectAfterProcessingChangeRx()
        {
            string redir = string.Empty;
            if (MasterPage.RxTask != null && MasterPage.RxTask.TaskType == Constants.PrescriptionTaskType.RXCHG)
            {
                if (SessionUserType == Constants.UserCategory.POB_REGULAR || SessionUserType == Constants.UserCategory.POB_SUPER)
                {
                    redir = Constants.PageNames.LIST_SEND_SCRIPTS;
                }
                else
                {
                    redir = Constants.PageNames.DOC_REFILL_MENU;
                }
            }
            return redir;
        }
        public void UpdateSessionSigText(string sigText)
        {
            PageState["SIGTEXT"] = sigText;
        }
        public void UpdateSessionSigID(string sigType, string sigID)
        {
            if (sigType == "F")
            {
                PageState["SIGID"] = Guid.Empty.ToString();
            }

            if (sigID.Contains("[DQ="))
            {
                int DQindex = sigID.IndexOf("[DQ=");
                sigID = sigID.Substring(0, DQindex);
            }
            PageState["SIGID"] = sigID;
        }

        public RxTaskModel PopulateRxWorflowDataFromUserInput(ISigAspx iSigAspx)
        {
            string LstPreferedSigSelectedItemText = (sigType.Value == "P") ? (LstPreferedSig.SelectedItem == null ? string.Empty : LstPreferedSig.SelectedItem.Text) : string.Empty;
            string LstSigSelectedItemText = (sigType.Value == "A") ? (LstSig.SelectedItem == null ? string.Empty : LstSig.SelectedItem.Text) : string.Empty;

            string LstPreferedSigSelectedValue = (sigType.Value == "P") ? (LstPreferedSig.SelectedValue == null ? string.Empty : LstPreferedSig.SelectedValue.ToString()) : string.Empty;
            string LstSigSelectedValue = (sigType.Value == "A") ? (LstSig.SelectedValue == null ? string.Empty : LstSig.SelectedValue.ToString()) : string.Empty;

            int LstPreferedSigSelectedIndex = (sigType.Value == "P") ? (LstPreferedSig == null ? -1 : LstPreferedSig.SelectedIndex) : -1;
            int LstSigSelectedIndex = (sigType.Value == "A") ? (LstSig == null ? -1 : LstSig.SelectedIndex) : -1;

            Dictionary<string, string> sigTextDictionary = new Dictionary<string, string>
            {
                { "P", LstPreferedSigSelectedItemText},
                { "A", LstSigSelectedItemText },
                { "F", txtFreeTextSig.Text}
            };

            Dictionary<string, string> sigIDDictionary = new Dictionary<string, string>
            {
                { "P", LstPreferedSigSelectedValue},
                { "A", LstSigSelectedValue },
                { "F", Allscripts.Impact.Sig.SaveFreeFormSigTextOrGetId(txtFreeTextSig.Text, Allscripts.ePrescribe.DatabaseSelector.ConnectionStringPointer.SHARED_DB)}
            };

            Dictionary<string, int> sigTextSelectedIndexDictionary = new Dictionary<string, int>
            {
                { "P", LstPreferedSigSelectedIndex},
                { "A", LstSigSelectedIndex }
            };

            RxTaskModel uiDataFromSigPage = MasterPage.RxTask;

            uiDataFromSigPage.TxtQuantity = txtQuantity.Text;

            if (lblRefills.Text == "Total number of dispenses approved: ")
            {
                //If text is dispenses, make sure to only save refills (i.e. subtract 1)
                uiDataFromSigPage.TxtRefill = (int.Parse(txtRefill.Text) - 1).ToString();
            }
            else
            {
                uiDataFromSigPage.TxtRefill = txtRefill.Text;
            }

            uiDataFromSigPage.TxtDaysSupply = txtDaysSupply.Text;
            uiDataFromSigPage.TxtFreeTextSig = iSigAspx.ComputeSigText(sigType.Value, sigTextDictionary, sigTextSelectedIndexDictionary);
            uiDataFromSigPage.SigID = iSigAspx.ComputeSigID(sigType.Value, sigIDDictionary, sigTextSelectedIndexDictionary);
            uiDataFromSigPage.IschkRegistryCheckedVisible = chkRegistryChecked.Visible;
            uiDataFromSigPage.IschkRegistryCheckedChecked = chkRegistryChecked.Checked;
            uiDataFromSigPage.IsDAW = chkDAW.Checked;
            uiDataFromSigPage.TxtMDD = txtMDD.Text;
            uiDataFromSigPage.Notes = txtPharmComments.Text;

            UpdateSessionSigText(uiDataFromSigPage.TxtFreeTextSig);
            UpdateSessionSigID(sigType.Value, uiDataFromSigPage.SigID);

            //Update Package from SetSigInfo
            if (PageState["Package"] != null && ddlCustomPack.SelectedItem != null)
            {
                DataTable dtPackage = PageState["Package"] as DataTable;
                DataRow[] drPackage = dtPackage.Select("PackageDescription ='" + ddlCustomPack.SelectedItem.Text + "'");

                if (drPackage.Length > 0)
                {
                    uiDataFromSigPage.GPPC = drPackage[0]["GPPC"].ToString();
                    if (!string.IsNullOrWhiteSpace(drPackage[0]["PackageSize"].ToString()))
                    {
                        uiDataFromSigPage.PackageSize = Convert.ToDecimal(drPackage[0]["PackageSize"].ToString());
                    }
                    uiDataFromSigPage.PackageUOM = drPackage[0]["PackageUOM"].ToString();
                    if (!string.IsNullOrWhiteSpace(drPackage[0]["PackageQuantity"].ToString()))
                    {
                        uiDataFromSigPage.PackageQuantity = Convert.ToInt32(drPackage[0]["PackageQuantity"].ToString());
                    }
                    uiDataFromSigPage.PackageDescription = drPackage[0]["PackageDescription"].ToString();
                }
            }
            Rx rx = base.CurrentRx;
            if (string.IsNullOrEmpty(rx.DDI))
            {
                uiDataFromSigPage.GPPC = string.Empty;
                uiDataFromSigPage.PackageSize = 1;
                uiDataFromSigPage.PackageUOM = ddlUnit.SelectedValue;
                uiDataFromSigPage.PackageQuantity = 1;
                uiDataFromSigPage.PackageDescription = ddlUnit.SelectedValue;
            }
            return uiDataFromSigPage;
        }

        public EPCSParameters EPCSWorkflowParameters()
        {
            EPCSParameters epcsDataFromSigPage = new EPCSParameters();
            epcsDataFromSigPage.FedCSCode = base.CurrentRx.ControlledSubstanceCode;
            epcsDataFromSigPage.DBID = base.DBID;
            var sm = MasterPage.RxTask.ScriptMessage as ScriptMessage;
            epcsDataFromSigPage.DsPharmacy = Allscripts.Impact.Pharmacy.LoadPharmacy(sm.DBPharmacyID, base.DBID);
            epcsDataFromSigPage.StateCSCodeForPharmacy = Allscripts.Impact.Prescription.GetStateControlledSubstanceCode(base.CurrentRx.DDI, null, epcsDataFromSigPage.DsPharmacy.Tables[0].Rows[0]["State"].ToString(), base.DBID);
            epcsDataFromSigPage.ReconciledControlledSubstanceCode = Prescription.ReconcileControlledSubstanceCodes(epcsDataFromSigPage.FedCSCode, epcsDataFromSigPage.StateCSCodeForPharmacy);
            epcsDataFromSigPage.StateCSCodeForPractice = Allscripts.Impact.Prescription.GetStateControlledSubstanceCode(base.CurrentRx.DDI, PageState.GetStringOrEmpty("PracticeState"), base.DBID);
            epcsDataFromSigPage.IsFreeFormMedControlSubstance = base.CurrentRx.IsFreeFormMedControlSubstance;
            epcsDataFromSigPage.CanTryEPCS = base.CanTryEPCS;
            epcsDataFromSigPage.SiteEPCSAuthorizedSchedulesList = base.SiteEPCSAuthorizedSchedules;
            return epcsDataFromSigPage;
        }

        public void ExecuteEPCSChangeRxWorkflow(EPCSParameters epcsDataFromSigPage)
        {
            base.CurrentRx.EffectiveDate = DateTime.Now.Date;
            base.CurrentRx.Destination = "PHARM";

            if (epcsDataFromSigPage.ReconciledControlledSubstanceCode == "2")
            {
                base.CurrentRx.CanEditEffectiveDate = true;
            }

            base.CurrentRx.ScheduleUsed = (string.IsNullOrEmpty(epcsDataFromSigPage.ReconciledControlledSubstanceCode)) ? 0 : Convert.ToInt32(epcsDataFromSigPage.ReconciledControlledSubstanceCode);

            List<Rx> epcsMedList = new List<Rx>();
            epcsMedList.Add(base.CurrentRx);
            ValidationSummary1.Enabled = false;
            ucEPCSDigitalSigning.IsScriptForNewRx = false;
            ucEPCSDigitalSigning.EPCSMEDList = epcsMedList;
            ucEPCSDigitalSigning.PharmacyTask = MasterPage.RxTask;
            ucEPCSDigitalSigning.ShouldShowEpcsSignAndSendScreen();
            MasterPage.RefreshActiveMeds();
        }
        public void ExecuteNonCSChangeRxWorkflow(IChangeRx ifcChgRx)
        {
            string rxID = ifcChgRx.ApprovedChangeRxResponseRxId(MasterPage.RxTask, new ScriptMessage(), PageState[Constants.SessionVariables.ShieldSendRxAuthToken]?.ToString());
            string auditLogPatientId = string.Empty;
            var sm = MasterPage.RxTask.ScriptMessage as ScriptMessage;
            var rxResponse = AuditLogPatientRxInsert(ePrescribeSvc.AuditAction.PATIENT_TASK_PROCESS, sm.DBPatientID, rxID, DateTime.UtcNow.ToString("s"));
            if (rxResponse.Success)
            {
                auditLogPatientId = rxResponse.AuditLogPatientID;
            }

            //Check here if it is not -1 then insert it in newly created table. you also have auditLogPatientID.
            //This will be used from service manager and added last audit log when message is sent to hub.
            if (!string.IsNullOrEmpty(auditLogPatientId))
            {
                base.AuditLogPatientRxInsert(ePrescribeSvc.AuditAction.PATIENT_TASK_PROCESS, base.SessionPatientID, rxID, DateTime.UtcNow.ToString("s"));
            }
            var rx = MasterPage.RxTask.Rx as Rx;
            PageState["REFILLMSG"] = "Change Request for " + rx.MedicationName + " approved for " + sm.PatientFirstName + " " + sm.PatientLastName + ".";
            MasterPage.RefreshActiveMeds();
            var redir = getRedirectAfterProcessingChangeRx();
            base.ClearPatientInfo();
            base.ClearMedicationInfo(false);
            //Reset RxChange And pharmacy task 
            PageState.Remove(Constants.SessionVariables.RxTask);
            Session.Remove(Constants.SessionVariables.TaskScriptMessageId);
            //Remove all the parameters before transfer to the new page. 
            HttpContext.Current.RewritePath(Request.Path, Request.PathInfo, "");
            if (!string.IsNullOrEmpty(redir))
            {
                Response.Redirect(redir, true);
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

        public bool ShouldCurrentWorkflowExit(RxTaskModel chgRxIn, Guid rxIdToDiscontinueFromSelectMed, IChangeRx ifcChgRx, AppCode.Interfaces.IEPCSWorkflowUtils ifcEPCSRx, EPCSParameters epcsDataFromSigPage, ISigAspx ifcSigAspx)
        {
            bool bShouldExit = false;
            if (chgRxIn == null || ifcChgRx == null || ifcEPCSRx == null || ifcSigAspx == null)
            {
                return bShouldExit;
            }
            else
            {
                var sm = chgRxIn.ScriptMessage as ScriptMessage;
                //Launch Workflow
                RxTaskModel rxChgUpdated = ifcChgRx.UserInputUpdatedRxChangeWorkflow(chgRxIn, ifcSigAspx);

                if (rxIdToDiscontinueFromSelectMed == Guid.Empty)
                {
                    ifcChgRx.DiscontinuePriorMedBeforeCHGRX(rxChgUpdated);
                }
                else
                {
                    Prescription.Discontinue(rxIdToDiscontinueFromSelectMed.ToString(), "1", DateTime.Today.ToShortDateString(), string.Empty, string.Empty, chgRxIn.UserId.ToString(), chgRxIn.LicenseId.ToString(), chgRxIn.ExternalFacilityCd, chgRxIn.ExternalGroupID, chgRxIn.DbId);
                    PageState.Remove(Constants.SessionVariables.RxIdToDiscontinue);
                }

                if (ifcEPCSRx.IsEPCSWorkflowExpected(epcsDataFromSigPage))
                {
                    bShouldExit = true;
                }
            }
            return bShouldExit;
        }

        private ChangeRxTask.WorkflowType RouteChangeRxWorkflowAndDetermineWorkflowType()
        {
            var result = ChangeRxTask.WorkflowType.RxChangeWithNonCsMed;
            if (MasterPage.RxTask != null && MasterPage.RxTask.TaskType == Constants.PrescriptionTaskType.RXCHG)
            {
                MasterPage.RxTask = PopulateRxWorflowDataFromUserInput(new SigAspx());

                if (PageState.GetStringOrEmpty(Constants.SessionVariables.PracticeState) == "MA")
                {
                    MasterPage.RxTask.Notes = new MassOpiateMessage().GenerateMassOpiateMessage(
                        MasterPage.RxTask.Rx.ControlledSubstanceCode,
                        MasterPage.RxTask.Notes,
                        PageState.GetStringOrEmpty(Constants.SessionVariables.PracticeState),
                        PageState.GetStringOrEmpty(Constants.SessionVariables.GPI),
                        PageState.GetBooleanOrFalse(Constants.SessionVariables.HasExpiredDEA),
                        PageState.Cast(Constants.SessionVariables.DeaSchedulesAllowed, new List<string>())
                        );
                }

                //Since this is coming from Sig Page, the type shall be set here
                MasterPage.RxTask.RxRequestType = RequestType.APPROVE_WITH_CHANGE;
                if (ShouldCurrentWorkflowExit(MasterPage.RxTask, PageState.GetGuidOr0x0(Constants.SessionVariables.RxIdToDiscontinue), new ChangeRxTask(), new EPCSWorkflowUtils(), EPCSWorkflowParameters(), new SigAspx()))
                {
                    ExecuteEPCSChangeRxWorkflow(EPCSWorkflowParameters());
                    result = ChangeRxTask.WorkflowType.RxChangeWithCsMed;
                }
                else
                {
                    ExecuteNonCSChangeRxWorkflow(new ChangeRxTask());
                }
            }
            return result;
        }

        protected void routeChangeRxWorkflowFromDUR(IChangeRx ifcChgRx, ISigAspx ifcSigAspx)
        {

            if (MasterPage.RxTask != null && MasterPage.RxTask.TaskType == Constants.PrescriptionTaskType.RXCHG)
            {
                MasterPage.RxTask = PopulateRxWorflowDataFromUserInput(new SigAspx());
                //Since this is coming from Sig Page, the type shall be set here
                MasterPage.RxTask.RxRequestType = RequestType.APPROVE_WITH_CHANGE;
                MasterPage.RxTask = ifcChgRx.UserInputUpdatedRxChangeWorkflow(MasterPage.RxTask, ifcSigAspx);
            }

        }
        public string RetrieveReturningPage(Allscripts.ePrescribe.Common.Constants.UserCategory userType)
        {
            if (userType == Allscripts.ePrescribe.Common.Constants.UserCategory.POB_SUPER)
            {
                return Constants.PageNames.PHARMACY_REFILL_SUMMARY;
            }
            else
            {
                return Constants.PageNames.DOC_REFILL_MENU;//Only Provider Workflow
            }
        }
        /// <summary>
        /// The sig page can be arrived from NewRx workflow (Fullscript/Medication/Script Pad page) RefReq workflow
        /// When NewRx, then Session["AddAndReview"] = true
        /// When NewRx but editing script coming from the Script Pad page, then Request.QueryString["Mode"] == "Edit"
        /// When RefReq, then Session[Constants.SessionVariables.TaskScriptMessageId] = script message id
        /// when ChgRx, then ???
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnChooseDestination_Click(object sender, EventArgs e)
        {
            if (rdbFreeTextSig.Checked && CurrentRx.RouteOfAdminCode?.Trim() == "OR")
            {
                if (Allscripts.Impact.Sig.CheckForUnapprovedFreeTextWords(txtFreeTextSig.Text.Split(' '), ref litFreeTextError, new Allscripts.Impact.Sig(), DBID))
                {
                    toggleSIGPanels();
                    return;
                }
            }
            if (txtMDD.Text.Length > 0 && (txtMDD.Text.Length + " MDD:  Per Day".Length + txtFreeTextSig.Text.Length > 1000))
            {
                return;
            }

            var pptPlus = new PPTPlus();

            //Added if condition for editing SIG info ---subhasis
            if (Request.QueryString["Mode"] == "Edit")
            {
                if (!ValidateSig())
                {
                    return;
                }

                editSIG(pptPlus);

                ValidationSummary1.Enabled = false;

                Session.Remove("NOTES");
                Session.Remove("Package");

                if (Request.QueryString["To"] != null)
                {
                    if (Request.QueryString["To"].Contains("SelectMedication"))
                    {
                        Server.Transfer(Constants.PageNames.RX_PROCESSOR + "?From=" + Constants.PageNames.SIG + "&To=" + Constants.PageNames.REDIRECT_TO_ANGULAR + "?componentName=" + Constants.PageNames.SELECT_MEDICATION + "&Action=EditScriptPad&RxID=" + Request.QueryString["RxId"]);
                    }
                    else
                    {
                        Server.Transfer(Constants.PageNames.RX_PROCESSOR + "?From=" + Constants.PageNames.SIG + "&To=" + Request.QueryString["To"] + "&Action=EditScriptPad&RxID=" + Request.QueryString["RxId"]);
                    }

                }
                else
                {
                    List<Rx> rxList = new List<Rx>();
                    Rx rx = new Rx();
                    rx = base.CurrentRx;
                    rxList.Add(rx);

                    DataSet dsActiveScripts = Prescription.CheckPatientRxHistoryForMultiSelect(base.CurrentRx.DDI, base.SessionPatientID, base.DBID);

                    if (dsActiveScripts.Tables[0].Rows.Count > 0)
                    {
                        ucMedicationHistoryCompletion.ActiveScripts = dsActiveScripts;
                        ucMedicationHistoryCompletion.SearchValue = string.Empty;
                        ucMedicationHistoryCompletion.LoadHistory();
                    }
                    else
                    {
                        redirectToNextPage(false);
                    }

                    return;
                }
            }
            else
            {
                setSigInfo();

                if (((Button)sender).ID == btnAddReview.ID)
                {
                    Session["AddAndReview"] = true;
                }
                else
                {
                    Session.Remove("AddAndReview");
                }

                if (ValidateSig())
                {
                    HttpContext.Current.RewritePath(Request.Path, Request.PathInfo, "");
                    if (Session[Constants.SessionVariables.TaskScriptMessageId] == null && MasterPage.RxTask == null || Session[Constants.SessionVariables.IsCsRefReqWorkflow] != null || MasterPage.ChangeRxRequestedMedCs != null)//Non Tasking Workflow or CS RefReq new Rx
                    {
                        if (PptPlusRequestInfo != null && base.CurrentRx.DDI != null)
                        {
                            PPTPlus.RequestTransactionForScriptPadMed(PageState, CurrentRx, new PptPlusData(), new PptPlus(),
                                new Allscripts.ePrescribe.Data.Medication(), new PptPlusServiceBroker(), new CommonComponentData());
                        }

                        List<Rx> rxList = base.ScriptPadMeds;
                        rxList.Add(base.CurrentRx);

                        ValidationSummary1.Enabled = false;

                        if (((Button)sender).ID == btnAddReview.ID)
                        {
                            var ddiList = string.Join(",", from rx in rxList select rx.DDI);
                            DataSet dsActiveScripts = Prescription.CheckPatientRxHistoryForMultiSelect(ddiList, base.SessionPatientID, base.DBID);

                            if (dsActiveScripts.Tables[0].Rows.Count > 0)
                            {
                                ucMedicationHistoryCompletion.ActiveScripts = dsActiveScripts;
                                ucMedicationHistoryCompletion.SearchValue = string.Empty;
                                ucMedicationHistoryCompletion.LoadHistory();
                            }
                            else
                            {
                                bool shouldCheckDur = DURMedispanUtils.IsAnyDurSettingOn(DURSettings);
                                if (shouldCheckDur)
                                {
                                    PageState["CHECK_DUR"] = true;
                                }
                                redirectToNextPage(shouldCheckDur);
                            }
                        }
                        else
                        {
                            redirectToNextPage(false);
                        }
                    }
                    else //Non cs Tasking Workflow
                    {
                        base.CurrentRx.AppendMDDToSig();
                        if (CurrentRx.IsFreeFormMed)//FreeForm Med Tasking Workflow
                        {
                            if (!(base.SessionUserType == Allscripts.ePrescribe.Common.Constants.UserCategory.POB_LIMITED || base.SessionUserType == Allscripts.ePrescribe.Common.Constants.UserCategory.POB_REGULAR))
                            {
                                if (chkRegistryChecked.Visible)
                                {
                                    Session["isCSRegistryChecked"] = chkRegistryChecked.Checked;
                                }
                                ValidationSummary1.Enabled = false; // "true" will cause the overlay to fail
                                PageState[Constants.SessionVariables.DUR_GO_PREVIOUS] = RetrieveReturningPage(base.SessionUserType);
                                //Should this even go to next DUR review page if DUR settings are off
                                bool bShouldRedirectToNextPage = true;
                                if (!DURMedispanUtils.IsAnyDurSettingOn(DURSettings))
                                {
                                    bShouldRedirectToNextPage = Handle_FreeFormMed_EpcsOn_DurOff_Workflow();//Why would anyone even have this setup ???
                                }
                                if (bShouldRedirectToNextPage)
                                {
                                    redirectToNextPage(DURMedispanUtils.IsAnyDurSettingOn(DURSettings));
                                }
                            }
                            else
                            {
                                ucMessageHeader.MessageText = "You do not have permission to process this request.";
                                ucMessageHeader.Icon = Controls_Message.MessageType.ERROR;
                                ucMessageHeader.Visible = true;
                                return;
                            }
                        }
                        else
                        {
                            // check for DURs
                            var durWarnings = GetTaskDUR();
                            if (DURMedispanUtils.DoesContainDUR(durWarnings))
                            {
                                PageState[Constants.SessionVariables.DURCheckResponse] = durWarnings;
                                if (!(base.SessionUserType == Allscripts.ePrescribe.Common.Constants.UserCategory.POB_LIMITED || base.SessionUserType == Allscripts.ePrescribe.Common.Constants.UserCategory.POB_REGULAR))
                                {
                                    if (chkRegistryChecked.Visible)
                                    {
                                        Session["isCSRegistryChecked"] = chkRegistryChecked.Checked;
                                    }

                                    // if DUR found, go to RxDURReviewMultiSelect.aspx for further processing
                                    ValidationSummary1.Enabled = false; // "true" will cause the overlay to fail

                                    List<Rx> rxList = new List<Rx>();
                                    Rx rx = new Rx();
                                    rx.DDI = base.CurrentRx.DDI;
                                    rxList.Add(rx);
                                    PageState[Constants.SessionVariables.DUR_GO_PREVIOUS] = RetrieveReturningPage(base.SessionUserType);

                                    DataSet dsActiveScripts = Prescription.CheckPatientRxHistoryForMultiSelect(base.CurrentRx.DDI, base.SessionPatientID, base.DBID);

                                    if (dsActiveScripts.Tables[0].Rows.Count > 0)
                                    {
                                        ucMedicationHistoryCompletion.ActiveScripts = dsActiveScripts;
                                        ucMedicationHistoryCompletion.SearchValue = string.Empty;
                                        ucMedicationHistoryCompletion.LoadHistory();
                                    }
                                    else
                                    {
                                        redirectToNextPage(true);
                                    }
                                    //check for DUP DUR
                                    bool hasDUP = durWarnings.DuplicateTherapy.Results.HasItems();
                                    bool hasOtherDUR = DURMedispanUtils.HasMoreThanDupDUR(durWarnings);

                                    ucMedicationHistoryCompletion.HasOtherDUR = hasOtherDUR;

                                    if (hasDUP)
                                    {
                                        Session["DUPDUR"] = true;

                                        if ((PageState.ContainsKey(Constants.SessionVariables.TaskScriptMessageId) && isCSRefReq()) || rx.IsFreeFormMedControlSubstance)
                                        {
                                            ucMedicationHistoryCompletion.IsCSMed = true;
                                        }
                                    }


                                }
                                else
                                {
                                    ucMessageHeader.MessageText = "You do not have permission to process this request because a DUR alert is presented.";
                                    ucMessageHeader.Icon = Controls_Message.MessageType.ERROR;
                                    ucMessageHeader.Visible = true;
                                    return;
                                }
                            }
                            else
                            {
                                if (RouteChangeRxWorkflowAndDetermineWorkflowType() == ChangeRxTask.WorkflowType.RxChangeWithCsMed)
                                    return;
                                // check if med is a CS med
                                string reconciledControlledSubstanceCode = string.Empty;
                                string stateCSCodeForPractice = string.Empty;
                                string stateCSCodeForPharmacy = string.Empty;
                                string fedCSCode = base.CurrentRx.ControlledSubstanceCode;

                                ScriptMessage sm = new ScriptMessage(
                                    Session[Constants.SessionVariables.TaskScriptMessageId].ToString(),
                                    base.SessionLicenseID,
                                    base.SessionUserID,
                                    base.DBID);

                                // get pharmacy details based on the refreq's pharmacy not the Session["LastPharmacyID"]
                                DataSet dsPharmacy = Allscripts.Impact.Pharmacy.LoadPharmacy(sm.DBPharmacyID, base.DBID);

                                stateCSCodeForPharmacy = Allscripts.Impact.Prescription.GetStateControlledSubstanceCode(base.CurrentRx.DDI, null, dsPharmacy.Tables[0].Rows[0]["State"].ToString(), base.DBID);

                                reconciledControlledSubstanceCode = Prescription.ReconcileControlledSubstanceCodes(fedCSCode, stateCSCodeForPharmacy);

                                // Check if med is a CS med
                                if (
                                        (!string.IsNullOrWhiteSpace(reconciledControlledSubstanceCode) &&
                                        reconciledControlledSubstanceCode.ToUpper() != "U" &&
                                        reconciledControlledSubstanceCode != "0")
                                        ||
                                        (base.CurrentRx.IsFreeFormMedControlSubstance)
                                    )
                                {
                                    if (chkRegistryChecked.Visible)
                                    {
                                        Session["isCSRegistryChecked"] = chkRegistryChecked.Checked;
                                    }

                                    // check if stars align for EPCS
                                    bool starsAlign = false;

                                    //
                                    // "CanTryEPCS" is true so that means the physician is EPCS authorized and the 
                                    // Enterprise Client associated with this license is EPCS enabled
                                    //
                                    if (base.CanTryEPCS)
                                    {
                                        //
                                        // check if Med is Federal controlled substance (schedule 2,3,4,5) OR
                                        // Med is a state controlled substance in the state the provider's practice is in AND 
                                        // Med is a state controlled substance in the state of the pharmacy where the script is being sent
                                        //
                                        stateCSCodeForPractice = Allscripts.Impact.Prescription.GetStateControlledSubstanceCode(base.CurrentRx.DDI, Session["PracticeState"].ToString(), base.DBID);

                                        if (Prescription.IsCSMedEPCSEligible(fedCSCode, stateCSCodeForPharmacy, stateCSCodeForPractice))
                                        {
                                            reconciledControlledSubstanceCode = Prescription.ReconcileControlledSubstanceCodes(fedCSCode, stateCSCodeForPharmacy, stateCSCodeForPractice);

                                            //
                                            // check if the state in which the site's practice is in, is EPCS authorized for the 
                                            // CS schedule of the selected med
                                            //
                                            if (base.SiteEPCSAuthorizedSchedules.Contains(reconciledControlledSubstanceCode))
                                            {
                                                //
                                                // check if pharmacy is EPCS enabled
                                                //
                                                if (dsPharmacy.Tables[0].Rows[0]["EpcsEnabled"].ToString() == "1")
                                                {
                                                    //
                                                    // get EPCS authorized schedules for pharmacy
                                                    //
                                                    List<string> authorizedSchedules = new List<string>();

                                                    DataTable dtSchedules = Allscripts.Impact.Pharmacy.GetEPCSAuthorizedSchedulesForPharmacy(dsPharmacy.Tables[0].Rows[0]["PharmacyID"].ToString(), DBID);

                                                    foreach (DataRow drSchedule in dtSchedules.Rows)
                                                    {
                                                        authorizedSchedules.Add(drSchedule[0].ToString());
                                                    }

                                                    //
                                                    // check if the state in which the pharmacy is in, is EPCS authorized for the 
                                                    // CS schedule of the selected med
                                                    //
                                                    if (authorizedSchedules.Contains(reconciledControlledSubstanceCode))
                                                    {
                                                        starsAlign = true;
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    if (starsAlign)
                                    {
                                        ValidationSummary1.Enabled = false;

                                        // show EPCS Signing popup
                                        base.CurrentRx.EffectiveDate = DateTime.Now.Date;
                                        base.CurrentRx.Destination = "PHARM";

                                        if (reconciledControlledSubstanceCode == "2")
                                        {
                                            base.CurrentRx.CanEditEffectiveDate = true;
                                        }

                                        base.CurrentRx.ScheduleUsed = Convert.ToInt32(reconciledControlledSubstanceCode);

                                        List<Rx> epcsMedList = new List<Rx>();
                                        epcsMedList.Add(base.CurrentRx);
                                        ucEPCSDigitalSigning.PharmacyTask = MasterPage.RxTask;
                                        ucEPCSDigitalSigning.IsScriptForNewRx = false;

                                        ucEPCSDigitalSigning.EPCSMEDList = epcsMedList;
                                        ucEPCSDigitalSigning.ShouldShowEpcsSignAndSendScreen();
                                    }
                                    else
                                    {
                                        ValidationSummary1.Enabled = false;
                                        ucCSMedRefillRequestNotAllowed.ShowPopUp();
                                    }
                                }
                                else
                                {
                                    if (
                                        MasterPage.RxTask != null &&
                                        (
                                            PageState.Cast(Constants.SessionVariables.TaskType, PrescriptionTaskType.DEFAULT) == PrescriptionTaskType.RXCHG ||
                                            PageState.Cast(Constants.SessionVariables.TaskType, PrescriptionTaskType.DEFAULT) == PrescriptionTaskType.RXCHG_PRIORAUTH
                                        )
                                    )
                                    {
                                        if (RouteChangeRxWorkflowAndDetermineWorkflowType() == ChangeRxTask.WorkflowType.RxChangeWithCsMed)
                                        {
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        saveRefillRequest();
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    Session.Remove("AddAndReview");
                }
            }

            if (!PageState.GetBooleanOrFalse(Constants.SessionVariables.IsCsRefReqWorkflow) && MasterPage.ChangeRxRequestedMedCs == null)
            {
                base.CurrentRx.AppendMDDToSig();
            }
        }

        public bool Handle_FreeFormMed_EpcsOn_DurOff_Workflow()
        {
            bool bShouldRedirectToNextPage = true;
            if (RouteChangeRxWorkflowAndDetermineWorkflowType() == ChangeRxTask.WorkflowType.RxChangeWithCsMed)
                return false;
            //TODO: Please use PBI 2803541 + also use the new EPCSWorkflowUtils that has been freshly created for Mobile Services
            //For now just copying the code for CS checks to allow correct routing for freeform CS meds
            string reconciledControlledSubstanceCode = string.Empty;
            string stateCSCodeForPractice = string.Empty;
            string stateCSCodeForPharmacy = string.Empty;
            string fedCSCode = base.CurrentRx.ControlledSubstanceCode;

            ScriptMessage sm = new ScriptMessage(
                Session[Constants.SessionVariables.TaskScriptMessageId].ToString(),
                base.SessionLicenseID,
                base.SessionUserID,
                base.DBID);

            // get pharmacy details based on the refreq's pharmacy not the Session["LastPharmacyID"]
            DataSet dsPharmacy = Allscripts.Impact.Pharmacy.LoadPharmacy(sm.DBPharmacyID, base.DBID);

            stateCSCodeForPharmacy = Prescription.GetStateControlledSubstanceCode(base.CurrentRx.DDI, null, dsPharmacy.Tables[0].Rows[0]["State"].ToString(), base.DBID);

            reconciledControlledSubstanceCode = Prescription.ReconcileControlledSubstanceCodes(fedCSCode, stateCSCodeForPharmacy);

            // Check if med is a CS med
            if (
                (!string.IsNullOrWhiteSpace(reconciledControlledSubstanceCode) &&
                 reconciledControlledSubstanceCode.ToUpper() != "U" &&
                 reconciledControlledSubstanceCode != "0")
                ||
                (base.CurrentRx.IsFreeFormMedControlSubstance)
            )
            {
                if (chkRegistryChecked.Visible)
                {
                    Session["isCSRegistryChecked"] = chkRegistryChecked.Checked;
                }

                // check if stars align for EPCS
                bool starsAlign = false;

                //
                // "CanTryEPCS" is true so that means the physician is EPCS authorized and the 
                // Enterprise Client associated with this license is EPCS enabled
                //
                if (base.CanTryEPCS)
                {
                    //
                    // check if Med is Federal controlled substance (schedule 2,3,4,5) OR
                    // Med is a state controlled substance in the state the provider's practice is in AND 
                    // Med is a state controlled substance in the state of the pharmacy where the script is being sent
                    //
                    stateCSCodeForPractice = Prescription.GetStateControlledSubstanceCode(base.CurrentRx.DDI, Session["PracticeState"].ToString(), base.DBID);

                    if (Prescription.IsCSMedEPCSEligible(fedCSCode, stateCSCodeForPharmacy, stateCSCodeForPractice))
                    {
                        reconciledControlledSubstanceCode = Prescription.ReconcileControlledSubstanceCodes(fedCSCode, stateCSCodeForPharmacy, stateCSCodeForPractice);

                        //
                        // check if the state in which the site's practice is in, is EPCS authorized for the 
                        // CS schedule of the selected med
                        //
                        if (base.SiteEPCSAuthorizedSchedules.Contains(reconciledControlledSubstanceCode))
                        {
                            //
                            // check if pharmacy is EPCS enabled
                            //
                            if (dsPharmacy.Tables[0].Rows[0]["EpcsEnabled"].ToString() == "1")
                            {
                                //
                                // get EPCS authorized schedules for pharmacy
                                //
                                List<string> authorizedSchedules = new List<string>();

                                DataTable dtSchedules = Allscripts.Impact.Pharmacy.GetEPCSAuthorizedSchedulesForPharmacy(dsPharmacy.Tables[0].Rows[0]["PharmacyID"].ToString(), DBID);

                                foreach (DataRow drSchedule in dtSchedules.Rows)
                                {
                                    authorizedSchedules.Add(drSchedule[0].ToString());
                                }

                                //
                                // check if the state in which the pharmacy is in, is EPCS authorized for the 
                                // CS schedule of the selected med
                                //
                                if (authorizedSchedules.Contains(reconciledControlledSubstanceCode))
                                {
                                    starsAlign = true;
                                }
                            }
                        }
                    }
                }

                if (starsAlign)
                {
                    bShouldRedirectToNextPage = false;
                    ValidationSummary1.Enabled = false;

                    // show EPCS Signing popup
                    base.CurrentRx.EffectiveDate = DateTime.Now.Date;
                    base.CurrentRx.Destination = "PHARM";

                    if (reconciledControlledSubstanceCode == "2")
                    {
                        base.CurrentRx.CanEditEffectiveDate = true;
                    }

                    base.CurrentRx.ScheduleUsed = Convert.ToInt32(reconciledControlledSubstanceCode);

                    List<Rx> epcsMedList = new List<Rx>();
                    epcsMedList.Add(base.CurrentRx);
                    ucEPCSDigitalSigning.PharmacyTask = MasterPage.RxTask;
                    ucEPCSDigitalSigning.IsScriptForNewRx = false;

                    ucEPCSDigitalSigning.EPCSMEDList = epcsMedList;
                    ucEPCSDigitalSigning.ShouldShowEpcsSignAndSendScreen();
                }
                else
                {
                    ValidationSummary1.Enabled = false;
                    ucCSMedRefillRequestNotAllowed.ShowPopUp();
                }
            }
            return bShouldRedirectToNextPage;
        }

        private DURCheckResponse GetTaskDUR()
        {
            var durSettings = DURMedispanUtils.CloneDURSettings(DURSettings);
            durSettings.CheckDuplicateTherapy = YesNoSetting.Yes; //always perform a Dup Therapy check, regardless of site settings so the overlay appears.

            var request = DURMedispanUtils.ConstructDurCheckRequest(
                    PageState.GetStringOrEmpty(Constants.SessionVariables.PatientDOB),
                    PageState.GetStringOrEmpty(Constants.SessionVariables.Gender),
                    new List<Rx> { CurrentRx },
                    PageState.Cast(Constants.SessionVariables.ACTIVEMEDDDILIST, new List<string>()),
                    DurPatientAllergies,
                    durSettings);
            DURCheckResponse durCheckResponse = DURMSC.PerformDURCheck(request);
            return durCheckResponse;
        }

        protected void btnSendToAdmin_Click(object sender, EventArgs e)
        {
            setSigInfo();

            if (ValidateSig())
            {
                Session["TASKTYPE"] = Constants.PrescriptionTaskType.SEND_TO_ADMIN;
                Session["SentTo"] = "Assistant";

                if (Session["CameFrom"] != null && Session["CameFrom"].ToString() == Constants.PageNames.FREE_FORM_DRUG)
                {
                    Session.Remove("CameFrom");
                    //DUR Page Workflow change
                    Server.Transfer(Constants.PageNames.RX_PROCESSOR + "?From=" + (Constants.PageNames.REDIRECT_TO_ANGULAR + "?componentName=" + Constants.PageNames.SELECT_MEDICATION) + "&To=" + Constants.PageNames.START_NEW_RX_PROCESS); //DUR Page Workflow change
                }
                else
                {
                    Server.Transfer(Constants.PageNames.RX_PROCESSOR + "?From=" + Constants.PageNames.REDIRECT_TO_ANGULAR + "?componentName=" + Constants.PageNames.SELECT_MEDICATION + "&To=" + Constants.PageNames.START_NEW_RX_PROCESS); //DUR Page Workflow change
                }
            }
        }

        protected void btnSendToPatPharm_Click(object sender, EventArgs e)
        {
            setSigInfo();

            if (ValidateSig())
            {
                if (Session["CameFrom"] != null && Session["CameFrom"].ToString() == Constants.PageNames.FREE_FORM_DRUG)
                {
                    Session.Remove("CameFrom");

                    //DUR Page Workflow change
                    Server.Transfer(Constants.PageNames.RX_PROCESSOR + "?From=" + (Constants.PageNames.REDIRECT_TO_ANGULAR + "?componentName=" + Constants.PageNames.SELECT_MEDICATION) + "&To=" + Constants.PageNames.START_NEW_RX_PROCESS + "&PatPharm=Y"); //DUR Page Workflow change
                }
                else
                {
                    Server.Transfer(Constants.PageNames.RX_PROCESSOR + "?From=" + Constants.PageNames.REDIRECT_TO_ANGULAR + "?componentName=" + Constants.PageNames.SELECT_MEDICATION + "&To=" + Constants.PageNames.START_NEW_RX_PROCESS + "&PatPharm=Y"); //DUR Page Workflow change
                }
            }
        }

        protected void btnPrint_Click(object sender, EventArgs e)
        {
            setSigInfo();

            if (ValidateSig())
            {
                Session["SentTo"] = "Printer";

                if (Session["CameFrom"] != null && Session["CameFrom"].ToString() == Constants.PageNames.FREE_FORM_DRUG)
                {
                    Session.Remove("CameFrom");
                    Server.Transfer(Constants.PageNames.RX_PROCESSOR + "?From=" + (Constants.PageNames.REDIRECT_TO_ANGULAR + "?componentName=" + Constants.PageNames.SELECT_MEDICATION) + "&PrintScript=YES");
                }
                else
                {
                    Server.Transfer(Constants.PageNames.RX_PROCESSOR + "?From=" + Constants.PageNames.REDIRECT_TO_ANGULAR + "?componentName=" + Constants.PageNames.SELECT_MEDICATION + "&PrintScript=YES");
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
            // base.AuditLogPatientRxInsert(ePrescribeSvc.AuditAction.PATIENT_TASK_PROCESS, base.SessionPatientID, base.CurrentRx.RxID);

            Response.Redirect(Constants.PageNames.APPROVE_REFILL_TASK);
        }

        void ucCSMedRefillRequestNotAllowed_OnPrintRefillRequest(object sender, EventArgs e)
        {

            Session["isCSRefillNotAllowed"] = true;

            ScriptMessage sm = new ScriptMessage(Session[Constants.SessionVariables.TaskScriptMessageId].ToString(), base.SessionLicenseID, base.SessionUserID, base.DBID);

            string rxID = Guid.NewGuid().ToString();

            StringBuilder pharmComments = new StringBuilder();
            DataSet pharmDS = Allscripts.Impact.Pharmacy.LoadPharmacy(sm.DBPharmacyID, base.DBID);

            pharmComments.Append(txtPharmComments.Text.Trim());

            Prescription rx = new Prescription();

            string stateCSCodeForPharmacy = Allscripts.Impact.Prescription.GetStateControlledSubstanceCode(base.CurrentRx.DDI, null, pharmDS.Tables[0].Rows[0]["State"].ToString(), base.DBID);
            //stateCSCodeForPractice = Allscripts.Impact.Prescription.GetStateControlledSubstanceCode(base.CurrentRx.DDI, Session["PracticeState"].ToString(), base.DBID);

            rx.SetHeaderInformation(base.SessionLicenseID, rxID, DateTime.UtcNow.ToString(),
                sm.DBPatientID, base.SessionUserID, Guid.Empty.ToString(),
                string.Empty, string.Empty, string.Empty, Constants.PrescriptionType.NEW, false, string.Empty,
                Convert.ToInt32(Session["SiteID"].ToString()), Constants.ERX_NOW_RX, null, base.DBID);

            var sigInfo = Allscripts.Impact.Sig.SaveFreeFormSigTextOrGetInfo(CurrentRx.SigText, Allscripts.ePrescribe.DatabaseSelector.ConnectionStringPointer.SHARED_DB);
            CurrentRx.SigID = Convert.ToString(sigInfo.SigID);
            CurrentRx.SigTypeId = (sigInfo.SigTypeID > 0) ? sigInfo.SigTypeID : (int)SigTypeEnum.SigTypeFreeForm;


            rx.AddMedication(
                base.SessionLicenseID, //EAK added
                0, // RxNumber
                sm.DBDDID,
                base.CurrentRx.MedicationName, //sm.RxDrugDescription,
                base.CurrentRx.RouteOfAdminCode, // routeOfAdminCode
                base.CurrentRx.DosageFormCode, // sm.RxDosageFormCode, // dosageFormCode
                base.CurrentRx.Strength,
                base.CurrentRx.StrengthUOM,
                CurrentRx.SigID,
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
                base.SessionUserID,
                null, // lastFillDate
                null, // authorizeByID
                Session["RX_WORKFLOW"] != null ? ((PrescriptionWorkFlow)Convert.ToInt32(Session["RX_WORKFLOW"])) : PrescriptionWorkFlow.STANDARD,
                Session["ExtFacilityCd"] != null ? Session["ExtFacilityCd"].ToString() : null,
                Session["ExtGroupID"] != null ? Session["ExtGroupID"].ToString() : null,
                base.CurrentRx.CoverageID, //coverageID
                -1, // alternativeIgnoreReason
                stateCSCodeForPharmacy,
                Session["DEA"] == null ? null : Session["DEA"].ToString(),
                CurrentRx.SigTypeId);

            bool? isCSRegistryChecked = null;
            if (chkRegistryChecked.Visible)
            {
                isCSRegistryChecked = chkRegistryChecked.Checked;
            }

            rx.Save(base.SessionSiteID, base.SessionLicenseID, base.SessionUserID, true, isCSRegistryChecked, base.DBID);
            var couponResponse = EPSBroker.ECouponPrintRefillRequest(rxID, sm.DBDDID, base.CanApplyFinancialOffers, ConfigKeys.AutoSendCoupons, base.DBID);
            if (couponResponse.IsPharmacyNotesUpdatedToRx)
            {
                List<string> couponDetailIds = new List<string>();
                couponDetailIds.Add(couponResponse.ECouponDetailID.ToString());

                PageState["PrintOnlyCoupons"] = couponDetailIds;
            }
            Hashtable htTaskRxID = new Hashtable();
            htTaskRxID.Add(Convert.ToInt64(Session["TaskID"]), rxID);
            Session["HTTaskRxID"] = htTaskRxID;

            Response.Redirect(Constants.PageNames.CSS_DETECT + "?To=" + HttpUtility.HtmlEncode(Constants.PageNames.APPROVE_REFILL_TASK), true);
        }

        void ucEPCSDigitalSigning_OnDigitalSigningComplete(DigitalSigningEventArgs dsEventArgs)
        {
            //Update Active medications at the end
            MasterPage.RefreshActiveMeds();
            if (dsEventArgs.Success)
            {
                foreach (KeyValuePair<string, string> signedMed in dsEventArgs.SignedMeds)
                {
                    saveRefillRequestForEPCS(signedMed.Value, signedMed.Key);
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
                    ucMessageHeader.MessageText = "Controlled substance meds being sent electronically could not be digitally signed.  Please try again or print.";
                }
                else
                {
                    ucMessageHeader.MessageText = dsEventArgs.Message;
                }

                ucMessageHeader.Icon = Controls_Message.MessageType.ERROR;
                ucMessageHeader.Visible = true;
            }
        }

        protected void ucMedicationHistoryCompletion_OnMedHistoryComplete(MedHistoryCompletionEventArgs eventArgs)
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
                        refreshTaskDUR();   // this method calls GetTaskDUR which adds the DUP DUR check if it is not currently set
                        redirectToNextPage(eventArgs.HasOtherDur);
                    }
                }
                else
                {
                    handlePossibleDur();
                }
            }
        }

        #endregion

        #region Private Methods
        private void refreshTaskDUR()
        {
            if (Session[Constants.SessionVariables.TaskScriptMessageId] != null)
            {
                var durWarnings = GetTaskDUR();     // this method adds the DUP DUR check if it is not currently set
                PageState[Constants.SessionVariables.DURCheckResponse] = durWarnings;
            }
        }
        private string getMedName()
        {
            string med = string.Empty;

            if (Session["RxList"] != null)
            {
                //there should only be 1 item in the list here
                ArrayList rxList = (ArrayList)Session["RxList"];
                Rx rx = (Rx)rxList[0];

                if (!string.IsNullOrEmpty(rx.MedicationName))
                {
                    med = rx.MedicationName;
                }
                if (!string.IsNullOrEmpty(rx.Strength))
                {
                    med = med + ' ' + rx.Strength;
                }
                if (!string.IsNullOrEmpty(rx.StrengthUOM))
                {
                    med = med + ' ' + rx.StrengthUOM;
                }

                if (!string.IsNullOrEmpty(rx.DosageFormDescription))
                {
                    med = med + ' ' + rx.DosageFormDescription;
                }
                if (!string.IsNullOrEmpty(rx.RouteOfAdminDescription))
                {
                    med = med + ' ' + rx.RouteOfAdminDescription;
                }
            }
            else
            {
                if (Session["MEDICATIONNAME"] != null)
                {
                    med = Session["MEDICATIONNAME"].ToString();
                }
                if (Session["STRENGTH"] != null)
                {
                    med = med + ' ' + Session["STRENGTH"].ToString();
                }
                if (Session["STRENGTHUOM"] != null)
                {
                    med = med + ' ' + Session["STRENGTHUOM"].ToString();
                }

                if (Session["DosageForm"] != null)
                {
                    med = med + ' ' + Session["DosageForm"].ToString();
                }
                if (Session["RouteofAdmin"] != null)
                {
                    med = med + ' ' + Session["RouteofAdmin"].ToString();
                }
            }

            return med;
        }

        private void setRxInfo(string rxID)
        {
            DataSet drRxDetails;
            drRxDetails = Allscripts.Impact.Prescription.ChGetRXDetails(rxID, base.DBID); //returns a dataset
            DataTable dtRx = drRxDetails.Tables[0];

            if (dtRx.Rows.Count > 0)
            {
                Rx rx = new Rx();
                DataRow drRX = dtRx.Rows[0];
                rx.DDI = drRX["DDI"].ToString().Trim();
                string mddText = String.Empty;
                //Remove maximum daily dosage frp, sigtext if exist than
                rx.SigText = getParsedSigText(drRX["SIGTEXT"].ToString(), out mddText);
                rx.MDD = mddText;
                if (String.IsNullOrWhiteSpace(mddText))
                {
                    //if mdd exists on a script set the field to visible regardless of site preference
                    SetMDDFieldsVisablity(true);
                }
                rx.SigID = drRX["SIGID"].ToString().Trim();
                rx.SigTypeId = Convert.ToInt32(drRX["SIGType"].ToString());
                rx.MedicationName = drRX["MedicationName"].ToString();
                rx.Strength = drRX["STRENGTH"].ToString();
                rx.StrengthUOM = drRX["STRENGTHUOM"].ToString();
                rx.DosageFormDescription = drRX["DosageForm"].ToString();
                rx.RouteOfAdminDescription = drRX["RouteofAdmin"].ToString();
                rx.ICD9Code = drRX["ICD9code"].ToString();
                rx.ICD10Code = drRX["ICD10code"].ToString();
                Session["DIAGNOSIS"] = string.Empty; //Added April 24 after discussion with PD 

                Session["SIGTEXT"] = rx.SigText; //Added for issue#2763
                //Added DosageFormCode and RouteOfAdminCode for issue#2833
                rx.DosageFormCode = drRX["DosageFormCode"].ToString();
                rx.RouteOfAdminCode = drRX["RouteOfAdminCode"].ToString();

                if (Request.QueryString["Refill"] != null && Convert.ToBoolean(Request.QueryString["Refill"]))
                {
                    rx.Refills = Convert.ToInt32(drRX["RefillQuantity"].ToString());
                    Session["RX_WORKFLOW"] = PrescriptionWorkFlow.AFTER_FIRST_FILL;
                }
                else
                {
                    rx.Refills = Convert.ToInt32(drRX["RefillQuantity"].ToString().Trim());
                    Session["RX_WORKFLOW"] = PrescriptionWorkFlow.STANDARD;
                }

                rx.Quantity = Convert.ToDecimal(drRX["Quantity"].ToString().Trim());
                rx.DaysSupply = Convert.ToInt32(drRX["DaysSupply"].ToString().Trim()); //Added on July 04 AKS for displaying the daystosupply
                rx.ControlledSubstanceCode = drRX["ControlledSubstanceCode"].ToString(); //added by dhiraj on 19/11/06
                rx.OriginalRxID = rxID;
                packsize = drRX["PSPQ"].ToString().Trim();
                rx.PackageDescription = packageDescription = drRX["PackageDescription"] != DBNull.Value ? drRX["PackageDescription"].ToString().Trim() : string.Empty;

                //added for editing SIG info --subhasis
                if (Request.QueryString["Mode"] == "Edit")
                {
                    rx.DAW = drRX["DAW"].ToString().Equals("Y") ? true : false;
                    rx.Notes = drRX["PharmacyNotes"].ToString().Trim();
                    rx.OriginalSigText = rx.SigText;
                }

                rx.NDCNumber = Convert.ToString(drRX["NDCNumber"]);

                ArrayList rxList = new ArrayList();
                rxList.Add(rx);
                Session["RxList"] = rxList;
            }
        }

        #region getMDDValue
        private string getMDDValue(string rxID, string controlledSubstanceCode)
        {
            string mddValue = String.Empty;

            if (_allowMDD)
            {
                string MDD = Allscripts.Impact.Prescription.GetMaximumDailyDosage(rxID, base.DBID);
                if (_MDDCSMedOnly)
                {
                    if (!String.IsNullOrEmpty(controlledSubstanceCode))
                    {
                        mddValue = MDD;
                    }
                }
                else
                {
                    mddValue = MDD;
                }
            }
            return mddValue;
        }
        #endregion getMDDValue

        private string getParsedSigText(string sigTextWithMDD, out string mddText)
        {
            mddText = String.Empty;

            if (String.IsNullOrWhiteSpace(sigTextWithMDD))
            {
                return sigTextWithMDD;
            }

            string sigText = string.Empty;
            int indexMDD = sigTextWithMDD.IndexOf("MDD:");
            if (indexMDD > 0)
            {
                sigText = sigTextWithMDD.Substring(0, indexMDD);
                string mddString = sigTextWithMDD.Substring(indexMDD + 4);
                int indexPerDay = mddString.IndexOf("Per Day");
                if (indexPerDay > 0)
                {
                    mddText = mddString.Substring(0, indexPerDay);
                }
            }
            else
            {
                sigText = sigTextWithMDD;
            }

            return sigText;
        }

        private void SetRefillInfo(Rx rx)
        {
            PageState["SIGTEXT"] = txtFreeTextSig.Text = rx.SigText;
            PageState["MEDICATIONNAME"] = rx.FullDrugDescription;
            PageState["DIAGNOSIS"] = string.Empty; //Added April 24 after discussion with PD
            PageState["REFILL"] = txtRefill.Text = rx.Refills.ToString();
            PageState["QUANTITY"] = txtQuantity.Text = rx.Quantity.ToString();
            PageState["DaysSupply"] = txtDaysSupply.Text = rx.DaysSupply.ToString();
            chkDAW.Checked = rx.DAW;
            packsize = rx.DosageFormDescription;
            ResetSigBuilder();
            toggleSIGPanels();
        }

        private bool GetDaw(string daw)
        {
            if (daw.ToUpper() == "Y" || daw.ToUpper() == "YES" || daw == "1")
            {
                return true;
            }
            return false;
        }

        private void setRefillInfo(ScriptMessage sm)
        {
            //EAK added to handle case where different med is selected
            if (sm.DBDDID == base.CurrentRx.DDI || string.IsNullOrWhiteSpace(base.CurrentRx.DDI))
            {
                Session["SIGTEXT"] = txtFreeTextSig.Text = sm.DispensedRxSIGText;
                Session["MEDICATIONNAME"] = sm.DispensedRxDrugDescription;
                Session["DIAGNOSIS"] = string.Empty; //Added April 24 after discussion with PD   
            }

            base.CurrentRx.SigText = sm.DispensedRxSIGText;

            if (base.CurrentRx.ControlledSubstanceCode != null && base.CurrentRx.ControlledSubstanceCode.Equals("2"))
            {
                Session["REFILL"] = txtRefill.Text = "0";
            }
            else
            {
                Session["REFILL"] = txtRefill.Text = sm.DispensedRxRefills;
            }

            if ((base.CurrentRx.ControlledSubstanceCode != null || sm.RxControlledSubstanceCode != null) && (Session[Constants.SessionVariables.IsCsRefReqWorkflow] != null || MasterPage.ChangeRxRequestedMedCs != null))
            {
                Session["SIGTEXT"] = txtFreeTextSig.Text = string.Empty;
                Session["REFILL"] = txtRefill.Text = "0";
            }
            else
            {
                Session["QUANTITY"] = txtQuantity.Text = Convert.ToDouble(sm.DispensedRxQuantity).ToString();
                Session["DaysSupply"] = txtDaysSupply.Text = sm.DispensedDaysSupply;
            }

            chkDAW.Checked = GetDaw(sm.DispensedDaw);

            if (sm.DispensedRxSIGText.Length > 0)
            {
                ResetSigBuilder();
            }

            if (sm.DispensedQuantityQualifier != null)
            {
                packsize = sm.DispensedQuantityQualifier;
            }

            revRefillReq.ErrorMessage = "Please enter total number of dispenses approved.";

            toggleSIGPanels();
        }

        private void ResetSigBuilder()
        {
            rdgPrefer.Checked = false;
            rdbAllSig.Checked = true;
            //RDR ID 2589 - Unchecked other radio button with in  the group .
            rdbFreeTextSig.Checked = false;
            LstLatinSig.SelectedIndex = 0;
        }

        private void setSigInfo()
        {
            Rx rx = base.CurrentRx;
            string Sig = string.Empty;
            string SigID = string.Empty;
            rx.SigTypeId = (int)SigTypeEnum.SigTypeStandard;

            if (sigType.Value == "P")
            {
                if (LstPreferedSig.SelectedIndex > -1)
                {
                    Sig = LstPreferedSig.SelectedItem.Text;
                    string formattedFreeTextSig = Allscripts.Impact.Sig.ToSigFormat(Sig);                   
                    FreeFormSigInfo ffsigInfo = Allscripts.Impact.Sig.SaveFreeFormSigTextOrGetInfo(formattedFreeTextSig, Allscripts.ePrescribe.DatabaseSelector.ConnectionStringPointer.SHARED_DB);
                    rx.SigTypeId = ffsigInfo.SigTypeID;
                    SigID = ffsigInfo.SigID;
                }
            }
            else if (sigType.Value == "A")
            {
                if (LstSig.SelectedIndex > -1)
                {
                    Sig = LstSig.SelectedItem.Text;
                    SigID = LstSig.SelectedValue.ToString();
                }
            }
            else if (sigType.Value == "F")
            {
                string formattedFreeTextSig = Allscripts.Impact.Sig.ToSigFormat(txtFreeTextSig.Text);

                if (string.IsNullOrWhiteSpace(formattedFreeTextSig))
                {
                    return;
                }

                FreeFormSigInfo sig = Allscripts.Impact.Sig.SaveFreeFormSigTextOrGetInfo(formattedFreeTextSig, Allscripts.ePrescribe.DatabaseSelector.ConnectionStringPointer.SHARED_DB);    
                SigID = sig.SigID;

                if(string.IsNullOrEmpty(SigID))
                {
                    logger.Debug("Error getting SIG ID for free form sig");
                    ucMessageHeader.MessageText = "Could not save the free form sig text. Please try again.";
                    ucMessageHeader.Icon = Controls_Message.MessageType.ERROR;
                    ucMessageHeader.Visible = true;
                    return;
                }

                rx.SigTypeId = sig.SigTypeID;
                Sig = sig.SigText;
                
            }

            if (Sig != string.Empty)
            {
                Session["SIGTEXT"] = rx.SigText = Sig;
            }

            if (SigID != string.Empty)
            {
                // ADDED JULY 31st check if SIG has been appended with [DQ=] if so then remove the same ..
                if (SigID.Contains("[DQ="))
                {
                    int DQindex = SigID.IndexOf("[DQ=");
                    SigID = SigID.Substring(0, DQindex);
                }

                Session["SIGID"] = rx.SigID = SigID;
            }

            rx.Notes = txtPharmComments.Text;
            rx.Quantity = decimal.Parse(txtQuantity.Text.Trim());

            int txtRefillValue = int.Parse(txtRefill.Text.Trim());
            //Is the text box refills or dispenses?
            if (lblRefills.Text == "Total number of dispenses approved: ")
            {
                rx.Refills = txtRefillValue - 1;
            }
            else
            {
                rx.Refills = txtRefillValue;
            }

            rx.DAW = chkDAW.Checked;

            int daysSupply;
            if (!string.IsNullOrWhiteSpace(txtDaysSupply.Text) && int.TryParse(txtDaysSupply.Text.Trim(), out daysSupply))
            {
                rx.DaysSupply = daysSupply;
            }

            if (_allowMDD)
            {
                rx.MDD = txtMDD.Text.Trim();
            }
            if (Session["Package"] != null && ddlCustomPack.SelectedItem != null)
            {
                DataTable dtPackage = Session["Package"] as DataTable;
                //DataView dv = dtPackage.DefaultView;
                //dv.RowFilter = "PackageSize ='" + ddlCustomPack.SelectedValue + "'";
                DataRow[] drPackage = dtPackage.Select("PackageDescription ='" + ddlCustomPack.SelectedItem.Text + "'");

                if (drPackage.Length > 0)
                {
                    rx.GPPC = drPackage[0]["GPPC"].ToString();
                    if (!string.IsNullOrWhiteSpace(drPackage[0]["PackageSize"].ToString()))
                    {
                        rx.PackageSize = Convert.ToDecimal(drPackage[0]["PackageSize"].ToString());
                    }
                    rx.PackageUOM = drPackage[0]["PackageUOM"].ToString();
                    if (!string.IsNullOrWhiteSpace(drPackage[0]["PackageQuantity"].ToString()))
                    {
                        rx.PackageQuantity = Convert.ToInt32(drPackage[0]["PackageQuantity"].ToString());
                    }
                    rx.PackageDescription = drPackage[0]["PackageDescription"].ToString();
                }
            }

            if (string.IsNullOrEmpty(rx.DDI))
            {
                rx.GPPC = string.Empty;
                rx.PackageSize = 1;
                rx.PackageUOM = ddlUnit.SelectedValue;
                rx.PackageQuantity = 1;
                rx.PackageDescription = ddlUnit.SelectedValue;
            }

            ArrayList rxList = new ArrayList();
            rxList.Add(rx);

            Session["RxList"] = rxList;
        }

        private bool ValidateSig()
        {
            lblSigErrorMsg.Text = CurrentRx.IsFreeFormMed ? "Please write a valid free text SIG." : "Please choose a SIG or write free text SIG.";

            if (Session["SIGTEXT"] == null || Session["SIGTEXT"].ToString().Trim() == string.Empty)
            {
                toggleSIGPanels();
                lblSigErrorMsg.Visible = true;
                //setMDD();
                return false;
            }
            else
            {
                if ((rdbFreeTextSig.Checked && txtFreeTextSig.Text.Trim().Equals(string.Empty)) ||
                    (rdbAllSig.Checked && LstSig.SelectedIndex < 0) ||
                    (rdgPrefer.Enabled && rdgPrefer.Checked && LstPreferedSig.SelectedIndex < 0))
                {
                    toggleSIGPanels();
                    lblSigErrorMsg.Visible = true;
                    //setMDD();
                    return false;
                }
            }

            return true;
        }

        private void editSIG(PPTPlus pptPlus)
        {
            setSigInfo();
            Rx rx = base.CurrentRx;
            rx.RxID = Request.QueryString["RxId"] != null ? Request.QueryString["RxId"] : string.Empty;

            if (Session["Package"] == null)
            {
                DataSet ds = Prescription.Load(rx.RxID, base.DBID);

                if (ds.Tables[1].Rows.Count > 0)
                {
                    rx.GPPC = ds.Tables[1].Rows[0]["GPPC"].ToString();
                    rx.PackageSize = Convert.ToDecimal(ds.Tables[1].Rows[0]["PackageSize"].ToString());
                    rx.PackageUOM = ds.Tables[1].Rows[0]["PackageUOM"].ToString();
                    rx.PackageQuantity = Convert.ToInt32(ds.Tables[1].Rows[0]["PackageQuantity"].ToString());
                    rx.PackageDescription = ds.Tables[1].Rows[0]["PackageDescription"].ToString();
                }
            }

            decimal realQuantity = 0;

            try
            {
                realQuantity = Allscripts.Impact.Sig.CalculateDrugQuantity(rx.Quantity, rx.PackageQuantity, rx.PackageSize);
            }
            catch (Exception ex)
            {
                Audit.AddException(base.SessionUserID, base.SessionLicenseID, "Exception when editing SIG from script pad: RxID: " + Request.QueryString["RxID"].ToString() + ex.ToString(), null, null, null, base.DBID);
            }

            //#2650-Added for updating provider favorite med list.
            string ProviderID = Session["UserID"].ToString();

            Allscripts.Impact.Prescription.UpdateRxSIG(rx.RxID, rx.SigID, rx.SigText, rx.Refills, realQuantity,
                rx.DaysSupply, rx.GPPC, rx.PackageSize, rx.PackageQuantity, rx.PackageUOM, rx.PackageDescription, rx.DAW ? "True" : "False",
                rx.Notes, ProviderID, rx.DDI, rx.MedicationName, rx.RouteOfAdminCode, rx.DosageFormCode, rx.Strength, rx.StrengthUOM, rx.ICD10Code, rx.SigTypeId, 1, base.DBID);
            if (_allowMDD)
            {
                Allscripts.Impact.Prescription.SaveMaximumDailyDosage(Request.QueryString["RxID"].ToString(), rx.MDD, base.DBID);
            }

            if (rx.SigText != rx.OriginalSigText)
            {
                Prescription.SetBaseRxIsDirty(Request.QueryString["RxID"].ToString(), base.DBID);
            }

            if (PptPlusRequestInfo != null && !string.IsNullOrWhiteSpace(rx.DDI))
            {
                PPTPlus.RequestTransactionForScriptPadMed(PageState, rx, new PptPlusData(), new PptPlus(),
                    new Allscripts.ePrescribe.Data.Medication(), new PptPlusServiceBroker(), new CommonComponentData());
                PPTPlus.UpdateTransactionIdToRx(PageState, rx.OriginalRxID, new PptPlusData(), base.DBID);
            }
        }

        private void toggleSIGPanels()
        {
            pnlPreferedSig.Style["display"] = "none";
            pnlAllSig.Style["display"] = "none";
            pnlFreeTextSig.Style["display"] = "none";

            if (rdgPrefer.Checked && rdgPrefer.Enabled)
            {
                sigType.Value = "P";
                heading.InnerText = "Choose Sig : Preferred ";
                pnlPreferedSig.Style["display"] = "inline";
            }
            else if (rdbAllSig.Checked)
            {
                sigType.Value = "A";
                heading.InnerText = "Choose Sig : All ";
                pnlAllSig.Style["display"] = "inline";
            }
            else
            {
                sigType.Value = "F";
                heading.InnerText = "Choose Sig : Free Text ";
                pnlFreeTextSig.Style["display"] = "inline";
                rdbFreeTextSig.Checked = true;
            }
        }

        private string TrimValue(string value)
        {
            string result = value.TrimEnd();
            result = result.TrimStart();
            return result;
        }

        private void saveRefillRequest()
        {
            decimal metricQuantity = base.CurrentRx.Quantity;

            if ((base.CurrentRx.PackageSize * base.CurrentRx.PackageQuantity) > 0)
            {
                metricQuantity = base.CurrentRx.Quantity * base.CurrentRx.PackageSize * base.CurrentRx.PackageQuantity;
            }

            string delegateProviderID = Session["DelegateProviderID"] != null ? Session["DelegateProviderID"].ToString() : base.SessionUserID;
            long serviceTaskID = -1;

            if (string.IsNullOrWhiteSpace(base.CurrentRx.DDI))
            {
                ScriptMessage.ApproveFreeFormMessage(Session[Constants.SessionVariables.TaskScriptMessageId].ToString(), base.CurrentRx.MedicationName, base.CurrentRx.DaysSupply, metricQuantity, base.CurrentRx.Refills,
                    base.CurrentRx.SigText, chkDAW.Checked, base.SessionUserID, base.CurrentRx.Notes, Constants.PrescriptionTransmissionMethod.SENT,
                    base.SessionLicenseID, base.SessionUserID, base.SessionSiteID, base.ShieldSecurityToken, base.CurrentRx.IsCompoundMed, base.CurrentRx.HasSupplyItem, delegateProviderID, out serviceTaskID, CurrentRx.MDD, CurrentRx.SigID, base.DBID);
            }
            else
            {
                ScriptMessage.ApproveMessage(
                    Session[Constants.SessionVariables.TaskScriptMessageId].ToString(), base.CurrentRx.DDI, base.CurrentRx.DaysSupply, metricQuantity, base.CurrentRx.Refills,
                    base.CurrentRx.SigText, chkDAW.Checked, base.SessionUserID, base.CurrentRx.Notes, Constants.PrescriptionTransmissionMethod.SENT,
                    base.SessionLicenseID, base.SessionUserID, base.SessionSiteID, base.ShieldSecurityToken, delegateProviderID, out serviceTaskID, CurrentRx.MDD, CurrentRx.SigID, base.DBID);
            }

            string auditLogPatientID = string.Empty;
            ePrescribeSvc.AuditLogPatientRxResponse rxResponse = base.AuditLogPatientRxInsert(ePrescribeSvc.AuditAction.PATIENT_TASK_PROCESS, base.SessionPatientID, base.CurrentRx.RxID);
            if (rxResponse.Success)
            {
                auditLogPatientID = rxResponse.AuditLogPatientID;
            }

            ///Check here if it is not -1 then insert it in newly created table. you also have auditLogPatientID.
            ///This will be used from service manager and added last audit log when message is sent to hub.
            if (serviceTaskID != -1 && !string.IsNullOrEmpty(auditLogPatientID))
            {
                Audit.InsertAuditLogPatientServiceTask(serviceTaskID, auditLogPatientID, base.DBID);
            }

            Session["REFILLMSG"] = "Refill for " + base.CurrentRx.MedicationName + " approved for " + Session["PATIENTNAME"].ToString().Trim() + ".";
            Session.Remove(Constants.SessionVariables.TaskScriptMessageId); //make sure we close out the task
            Session.Remove("NOTES");

            if (IsValidReturnPage())
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

        private bool IsValidReturnPage()
        {
            return !string.IsNullOrEmpty(PageState.GetStringOrEmpty(Constants.SessionVariables.CameFrom)) && PageState.GetStringOrEmpty(Constants.SessionVariables.CameFrom) != Constants.PageNames.FREE_FORM_DRUG;
        }

        private void saveRefillRequestForEPCS(string scriptMessageID, string rxID)
        {
            long serviceTaskID = -1;
            if (MasterPage.RxTask != null && MasterPage.RxTask.TaskType == Constants.PrescriptionTaskType.RXCHG)
            {
                var sm = MasterPage.RxTask.ScriptMessage as ScriptMessage;
                if (sm != null) Prescription.Discontinue(sm.DBRxID, "1", DateTime.Today.ToShortDateString(), string.Empty, string.Empty, SessionUserID, SessionLicenseID, PageState.GetString("ExtFacilityCd", null), PageState.GetString("ExtGroupID", null), DBID);

                serviceTaskID = ScriptMessage.SendCHGRESScriptMessageForEPCS(scriptMessageID, rxID, base.SessionLicenseID, base.SessionUserID, MasterPage.RxTask.ScriptMessageGUID, base.CurrentRx.Notes, base.DBID);
            }
            else
            {
                serviceTaskID = ScriptMessage.SendRenewalScriptMessageForEPCS(scriptMessageID, rxID, base.SessionLicenseID, base.SessionUserID, PageState.GetStringOrEmpty(Constants.SessionVariables.TaskScriptMessageId), base.CurrentRx.Notes, base.DBID);
            }
            ///No need here since we already created audit in EPCS overlay dialog.
            //base.AuditLogPatientRxInsert(ePrescribeSvc.AuditAction.PATIENT_TASK_PROCESS, base.SessionPatientID, rxID);
            string auditLogPatientID = ucEPCSDigitalSigning.AuditLogPatientID.ContainsKey(rxID) ? ucEPCSDigitalSigning.AuditLogPatientID[rxID] : null;

            ///Check here if it is not -1 then insert it in newly created table. you also have auditLogPatientID.
            ///This will be used from service manager and added last audit log when message is sent to hub.
            if (serviceTaskID != -1 && !string.IsNullOrEmpty(auditLogPatientID))
            {
                Audit.InsertAuditLogPatientServiceTask(serviceTaskID, auditLogPatientID, base.DBID);
            }
            if (MasterPage.RxTask != null && MasterPage.RxTask.TaskType == Constants.PrescriptionTaskType.RXCHG)
            {
                PageState["REFILLMSG"] = "Rx Change for " + base.CurrentRx.MedicationName + " approved for " + PageState.GetStringOrEmpty("PATIENTNAME").Trim() + ".";
            }
            else
            {
                PageState["REFILLMSG"] = "Refill for " + base.CurrentRx.MedicationName + " approved for " + PageState.GetStringOrEmpty("PATIENTNAME").Trim() + ".";
            }
            PageState.Remove(Constants.SessionVariables.TaskScriptMessageId); //make sure we close out the refreq
            PageState.Remove(Constants.SessionVariables.Notes);

            if (PageState.GetStringOrEmpty("CameFrom") != string.Empty)
            {
                string cameFrom = PageState.GetStringOrEmpty("CameFrom");
                PageState.Remove(Constants.SessionVariables.CameFrom);
                Response.Redirect(cameFrom, true);
            }
            else
            {
                Response.Redirect(Constants.PageNames.APPROVE_REFILL_TASK, true);
            }
        }

        /// <summary>
        /// Checks if the REFREQ med is a controlled substance.
        /// </summary>
        /// <returns>True if REFREQ med is a controlled substance.Else false.</returns>
        private bool isCSRefReq()
        {
            bool isCSRefReq = false;

            string reconciledControlledSubstanceCode = string.Empty;
            string stateCSCodeForPharmacy = string.Empty;
            string fedCSCode = base.CurrentRx.ControlledSubstanceCode;

            sm = new ScriptMessage(
                PageState.GetStringOrEmpty(Constants.SessionVariables.TaskScriptMessageId),
                base.SessionLicenseID,
                base.SessionUserID,
                base.DBID);

            // get pharmacy details based on the refreq's pharmacy not the Session["LastPharmacyID"]
            DataSet dsPharmacy = Allscripts.Impact.Pharmacy.LoadPharmacy(sm.DBPharmacyID, base.DBID);

            stateCSCodeForPharmacy = Allscripts.Impact.Prescription.GetStateControlledSubstanceCode(base.CurrentRx.DDI, null, dsPharmacy.Tables[0].Rows[0]["State"].ToString(), base.DBID);

            reconciledControlledSubstanceCode = Prescription.ReconcileControlledSubstanceCodes(fedCSCode, stateCSCodeForPharmacy);

            if (
                    (!string.IsNullOrWhiteSpace(reconciledControlledSubstanceCode) && reconciledControlledSubstanceCode.ToUpper() != "U" && reconciledControlledSubstanceCode != "0")
                    ||
                    (base.CurrentRx.IsFreeFormMedControlSubstance)
                )
            {
                isCSRefReq = true;
            }

            return isCSRefReq;
        }

        private void processCsMed()
        {
            if (MasterPage.RxTask != null && MasterPage.RxTask.TaskType == Constants.PrescriptionTaskType.RXCHG)
            {
                if ((RouteChangeRxWorkflowAndDetermineWorkflowType() == ChangeRxTask.WorkflowType.RxChangeWithCsMed))
                    return;
            }
            else
            {
                // check if med is a CS med
                string reconciledControlledSubstanceCode = string.Empty;
                string stateCSCodeForPractice = string.Empty;
                string stateCSCodeForPharmacy = string.Empty;
                string fedCSCode = base.CurrentRx.ControlledSubstanceCode;

                ScriptMessage sm = new ScriptMessage(
                    Session[Constants.SessionVariables.TaskScriptMessageId].ToString(),
                    base.SessionLicenseID,
                    base.SessionUserID,
                    base.DBID);

                // get pharmacy details based on the refreq's pharmacy not the Session["LastPharmacyID"]
                DataSet dsPharmacy = Allscripts.Impact.Pharmacy.LoadPharmacy(sm.DBPharmacyID, base.DBID);

                stateCSCodeForPharmacy = Allscripts.Impact.Prescription.GetStateControlledSubstanceCode(base.CurrentRx.DDI, null, dsPharmacy.Tables[0].Rows[0]["State"].ToString(), base.DBID);

                reconciledControlledSubstanceCode = Prescription.ReconcileControlledSubstanceCodes(fedCSCode, stateCSCodeForPharmacy);

                // Check if med is a CS med
                if (
                    (!string.IsNullOrWhiteSpace(reconciledControlledSubstanceCode) &&
                     reconciledControlledSubstanceCode.ToUpper() != "U" &&
                     reconciledControlledSubstanceCode != "0")
                    ||
                    (base.CurrentRx.IsFreeFormMedControlSubstance)
                    )
                {
                    // check if stars align for EPCS
                    bool starsAlign = false;

                    //
                    // "CanTryEPCS" is true so that means the physician is EPCS authorized and the 
                    // Enterprise Client associated with this license is EPCS enabled
                    //
                    if (base.CanTryEPCS)
                    {
                        //
                        // check if Med is Federal controlled substance (schedule 2,3,4,5) OR
                        // Med is a state controlled substance in the state the provider's practice is in AND 
                        // Med is a state controlled substance in the state of the pharmacy where the script is being sent
                        //
                        stateCSCodeForPractice =
                            Allscripts.Impact.Prescription.GetStateControlledSubstanceCode(base.CurrentRx.DDI,
                                Session["PracticeState"].ToString(), base.DBID);

                        if (Prescription.IsCSMedEPCSEligible(fedCSCode, stateCSCodeForPharmacy, stateCSCodeForPractice))
                        {
                            reconciledControlledSubstanceCode = Prescription.ReconcileControlledSubstanceCodes(fedCSCode,
                                stateCSCodeForPharmacy, stateCSCodeForPractice);

                            //
                            // check if the state in which the site's practice is in, is EPCS authorized for the 
                            // CS schedule of the selected med
                            //
                            if (base.SiteEPCSAuthorizedSchedules.Contains(reconciledControlledSubstanceCode))
                            {
                                //
                                // check if pharmacy is EPCS enabled
                                //
                                if (dsPharmacy.Tables[0].Rows[0]["EpcsEnabled"].ToString() == "1")
                                {
                                    //
                                    // get EPCS authorized schedules for pharmacy
                                    //
                                    List<string> authorizedSchedules = new List<string>();

                                    DataTable dtSchedules =
                                        Allscripts.Impact.Pharmacy.GetEPCSAuthorizedSchedulesForPharmacy(
                                            dsPharmacy.Tables[0].Rows[0]["PharmacyID"].ToString(), DBID);

                                    foreach (DataRow drSchedule in dtSchedules.Rows)
                                    {
                                        authorizedSchedules.Add(drSchedule[0].ToString());
                                    }

                                    //
                                    // check if the state in which the pharmacy is in, is EPCS authorized for the 
                                    // CS schedule of the selected med
                                    //
                                    if (authorizedSchedules.Contains(reconciledControlledSubstanceCode))
                                    {
                                        starsAlign = true;
                                    }
                                }
                            }
                        }
                    }

                    if (starsAlign)
                    {
                        ValidationSummary1.Enabled = false;

                        // show EPCS Signing popup
                        base.CurrentRx.EffectiveDate = DateTime.Now.Date;
                        base.CurrentRx.Destination = "PHARM";

                        if (reconciledControlledSubstanceCode == "2")
                        {
                            base.CurrentRx.CanEditEffectiveDate = true;
                        }

                        base.CurrentRx.ScheduleUsed = Convert.ToInt32(reconciledControlledSubstanceCode);

                        List<Rx> epcsMedList = new List<Rx>();
                        //Explicitly set the ScriptMessageID when ChangeRequest REFREQ or ReconcilePatient REFREQ case
                        if (PageState[Constants.SessionVariables.TaskScriptMessageId] != null)
                        {
                            base.CurrentRx.ScriptMessageID = PageState.GetStringOrEmpty(Constants.SessionVariables.TaskScriptMessageId);
                        }
                        epcsMedList.Add(base.CurrentRx);

                        ucEPCSDigitalSigning.PharmacyTask = MasterPage.RxTask;
                        ucEPCSDigitalSigning.IsScriptForNewRx = false;

                        ucEPCSDigitalSigning.EPCSMEDList = epcsMedList;
                        ucEPCSDigitalSigning.ShouldShowEpcsSignAndSendScreen();
                    }
                    else
                    {
                        ValidationSummary1.Enabled = false;
                        ucCSMedRefillRequestNotAllowed.ShowPopUp();
                    }
                }
            }
        }

        private string ParseRxGuidQueryString()
        {
            Guid rxGuid;
            string rxId = string.Empty;
            if (Guid.TryParse(Request.QueryString["RxId"], out rxGuid))
            {
                rxId = rxGuid.ToString();
            }

            return Microsoft.Security.Application.Encoder.UrlEncode(rxId);
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

        private void redirectToNextPage(bool isDurFound)
        {
            if (!PageState.ContainsKey(Constants.SessionVariables.TaskScriptMessageId) && MasterPage.RxTask == null || Session[Constants.SessionVariables.IsCsRefReqWorkflow] != null || MasterPage.ChangeRxRequestedMedCs != null)
            {
                if (Session["AddAndReview"] != null && Convert.ToBoolean(Session["AddAndReview"]) == true)
                {
                    Session.Remove("AddAndReview");
                    Response.Redirect(Constants.PageNames.RX_PROCESSOR + "?From=" + Constants.PageNames.SIG + "&To=" +
                                      Constants.PageNames.SCRIPT_PAD + "&Search=" + "?");
                }
                else if (Session[Constants.SessionVariables.ReviewScriptPadRedirection] != null && Convert.ToBoolean(Session[Constants.SessionVariables.ReviewScriptPadRedirection]) == true)
                {
                    Session.Remove(Constants.SessionVariables.ReviewScriptPadRedirection);
                    Response.Redirect(Constants.PageNames.SCRIPT_PAD);
                }
                else if (Request.QueryString["Mode"] == "Edit")
                {
                    //Edit script pad workflow. User has come to Sig.aspx or NurseSig.aspx to edit the Sig of the script.
                    string rxId = ParseRxGuidQueryString();

                    if (Request.QueryString["To"] != null)
                    {
                        string urlForRedirection = Constants.PageNames.UrlForRedirection(Request.QueryString["To"]);

                        if (string.IsNullOrEmpty(urlForRedirection))
                        {
                            urlForRedirection = Constants.PageNames.SCRIPT_PAD;
                        }

                        Response.Redirect(Constants.PageNames.RX_PROCESSOR + "?From=" + Constants.PageNames.NURSE_SIG + "&To=" +
                            urlForRedirection + "&Action=EditScriptPad&RxID=" + rxId, true);
                    }
                    else
                    {
                        Response.Redirect(
                            Constants.PageNames.RX_PROCESSOR + "?From=" + Constants.PageNames.NURSE_SIG + "&To=" +
                            Constants.PageNames.SCRIPT_PAD + "&Action=EditScriptPad&RxID=" + rxId, true);
                    }
                }
                else
                {
                    Response.Redirect(
                            Constants.PageNames.RX_PROCESSOR + "?From=" + Constants.PageNames.SIG + "&To=" +
                            (Constants.PageNames.REDIRECT_TO_ANGULAR + "?componentName=" + Constants.PageNames.SELECT_MEDICATION), true);
                }
            }
            else
            {
                if (isDurFound)
                {
                    if (MasterPage.RxTask != null && MasterPage.RxTask.TaskType == Constants.PrescriptionTaskType.RXCHG)
                    {
                        routeChangeRxWorkflowFromDUR(new ChangeRxTask(), new SigAspx());
                    }
                    Response.Redirect(Constants.PageNames.RX_DUR_REVIEW_MULTI_SELECT);
                }
                else
                {
                    if (MasterPage.RxTask != null && MasterPage.RxTask.TaskType == Constants.PrescriptionTaskType.RXCHG)
                    {
                        if ((RouteChangeRxWorkflowAndDetermineWorkflowType() == ChangeRxTask.WorkflowType.RxChangeWithCsMed))
                            return;
                    }
                    else
                    {
                        saveRefillRequest();
                    }
                }
            }
        }

        #endregion


    }
}