using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.Data;
using Allscripts.ePrescribe.Medispan.Clinical.Model.Settings;
using Allscripts.Impact;
using eRxWeb.AppCode;
using eRxWeb.State;
using Allscripts.ePrescribe.ExtensionMethods;
using ConfigKeys = Allscripts.Impact.ConfigKeys;
using Constants = Allscripts.ePrescribe.Common.Constants;
using DURSettings = Allscripts.ePrescribe.Medispan.Clinical.Model.Settings.DURSettings;
using Allscripts.ePrescribe.Medispan.Clinical.Model.Response;
using Allscripts.ePrescribe.Medispan.Clinical.Model.Request;
using Allscripts.Impact.ePrescribeSvc;
using DUR = Allscripts.Impact.DUR;
using Module = Allscripts.Impact.Module;
using Prescription = Allscripts.Impact.Prescription;
using ScriptMessage = Allscripts.Impact.ScriptMsg.ScriptMessage;
using eRxWeb.AppCode.PptPlusBPL;
using FormularyStatus = Allscripts.Impact.FormularyStatus;
using Rx = Allscripts.Impact.Rx;
using Allscripts.ePrescribe.Data.CommonComponent;
using Allscripts.ePrescribe.Objects;
using Allscripts.ePrescribe.Shared.Logging;

namespace eRxWeb
{
    public partial class NurseSig : BasePage
    {
        private static readonly ILoggerEx logger = LoggerEx.GetLogger();

        private string DosageFormCode = String.Empty;
        private bool _isCSMed = false;
        private bool _allowMDD = false;
        private bool _MDDCSMedOnly = false;
        //This is the sig id that is associated with all the free text sigs. 
        public static string FREETEXT_SIG_ID = "00000000-0000-0000-0000-000000000000";
        const string DurPreferDateRange = "DurPreferDateRange";
        const string DurPreferDurDateWarning = "DurPreferDurDateWarning";
        const string DispenseLabel = "Total number of dispenses approved: ";
        #region Properties

        private string _refillRequestProviderID
        {
            get
            {
                string refillRequestProviderID = string.Empty;

                if (ViewState["RefillRequestProviderID"] != null)
                {
                    refillRequestProviderID = ViewState["RefillRequestProviderID"].ToString();
                }

                return refillRequestProviderID;
            }
            set { ViewState["RefillRequestProviderID"] = value; }
        }

        private bool _isOnlyDupDUR
        {
            get
            {
                bool isOnlyDupDUR = false;

                if (ViewState["IsOnlyDupDUR"] != null)
                {
                    isOnlyDupDUR = Convert.ToBoolean(ViewState["IsOnlyDupDUR"].ToString());
                }

                return isOnlyDupDUR;
            }
            set { ViewState["IsOnlyDupDUR"] = value; }
        }

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            InputValidation.SetRegexValidator(ref revtxtQuantity, RegularExpressions.MedicationQuantityLimit, Constants.ErrorMessages.MedicationQuantityInvalid);
            if (!Convert.ToBoolean(PageState[Constants.SessionVariables.IsPatientHealthPlanDisclosed]))
            {
                hiddenPharmacyNote.Value = Constants.PharmacyNoteText.CASH_PATIENT;
            }
            else
            {
                hiddenPharmacyNote.Value = string.Empty;
            }
            //there should only be 1 item in the list here
            Rx rx = base.CurrentRx;
            setMDDInfo(rx.ControlledSubstanceCode);
          
            adControl.FeaturedModule = Module.ModuleType.DELUXE;
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
                                        string lexicompURL = string.Format(ConfigKeys.LexicompAdminDosageURL,
                                            scriptPadRx.NDCNumber);
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

            ucMedicationHistoryCompletion.CurrentPage = Constants.PageNames.NURSE_SIG;
            base.SetSingleClickButton(btnAddReview);
            base.SetSingleClickButton(btnChooseDestination);

            // add client side event handlers to controls
            var maxSigText = 1000;
            txtFreeTextSig.Attributes.Add("onkeydown", $"return LimitInput(this, {maxSigText}, event);");
            txtFreeTextSig.Attributes.Add("onkeyup", $"return LimitInput(this, {maxSigText}, event);");
            txtFreeTextSig.Attributes.Add("onpaste", $"return LimitPaste(this, {maxSigText}, event);");
            txtFreeTextSig.Attributes.Add("onchange", $"return LimitChange(this, {maxSigText}, event);");

            txtPharmComments.Attributes.Add("onkeydown", "return LimitInput(this,'210', event);");
            txtPharmComments.Attributes.Add("onkeyup", "return LimitInput(this,'210', event);");
            txtPharmComments.Attributes.Add("onpaste", "return LimitPaste(this,'210', event);");
            txtPharmComments.Attributes.Add("onchange", "return LimitChange(this, '210', event);");

            txtMDD.Attributes.Add("onkeydown", $"if(!AllowMDDChange({maxSigText}, event, {txtMDD.ClientID}, {txtFreeTextSig.ClientID}, {lblMDDError.ClientID})) return false;");
            txtMDD.Attributes.Add("onkeyup", "UpdateFreeTextSIG()");
            txtMDD.Attributes.Add("onpaste", "return false");

            LstSig.DataBound += new EventHandler(LstSig_DataBound);
            if (Session[Constants.SessionVariables.PhysicianId] == null)
            {
                Session[Constants.SessionVariables.PhysicianId] = DelegateProvider.UserID;
            }

            if (!Page.IsPostBack)
            {
                if (Request.QueryString["from"] != null && Convert.ToString(Request.QueryString["from"]).Equals(Constants.PageNames.PHARMACY_REFILL_SUMMARY))
                {
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), "RefreshPatientHeader", $"RefreshPatientHeader('{PageState.GetStringOrEmpty(Constants.SessionVariables.PatientId)}'); ", true);
                }

                btnCheckRegistry.OnClientClick = "OpenNewWindow" + "('" + PageState.GetStringOrEmpty("STATEREGISTRYURL") + "')";
                // inject javascript to initialize orig sig, days supply, etc.
                ClientScript.RegisterStartupScript(this.GetType(), "InitializeTrackingVariables",
                    "InitializeTrackingVariables();", true);

                // add client side event handlers to controls
                rdgPrefer.Attributes.Add("onclick", "ToggleSIGs('P');");
                rdbAllSig.Attributes.Add("onclick", "ToggleSIGs('A');");
                rdbFreeTextSig.Attributes.Add("onclick", "ToggleSIGs('F');");                

                //EAK Added for REFEQ
                if (PageState.ContainsKey(Constants.SessionVariables.TaskScriptMessageId))
                {
                    setRefillInfo(Session[Constants.SessionVariables.TaskScriptMessageId].ToString());
                    btnChooseDestination.Text = "Send to Pharmacy";
                    btnChooseDestination.Enabled = true;
                    btnChangeMed.Visible = false;
                    btnAddReview.Visible = false;
                    btnPatientEdu.Visible = false;
                    _isCSMed = !string.IsNullOrWhiteSpace(rx.ControlledSubstanceCode);
                }

                //Addition as on Oct 05 by AKS (ID 73)
                if ((Request.QueryString["CameFrom"] != null) && (Request.QueryString["CameFrom"] == Constants.PageNames.FREE_FORM_DRUG || Request.QueryString["CameFrom"] == "FreeForm"))
                    // Comming from Free Form Drug page
                {
                    rdbFreeTextSig.Checked = true;
                    rdbAllSig.Checked = false;
                    rdgPrefer.Checked = false;
                    rdbAllSig.Enabled = false;
                    rdgPrefer.Enabled = false;
                    if (Request.QueryString["CameFrom"] != "FreeForm")
                    {
                        HttpContext.Current.RewritePath(Request.Path, Request.PathInfo, string.Empty);
                        // remove the query string
                        Session["CameFrom"] = Constants.PageNames.FREE_FORM_DRUG;
                    }
                }

                //// This code is for Add Comments...Comming from Review History Page
                ////-------------------------------------------------------------------------------------------------------------

                if (Request.QueryString["RxID"] != null) // Comming for Review History PAge
                {
                    string RxID = Request.QueryString["RxID"].ToString();
                    setRxInfo(RxID);
                    //Start Added this code on July 04 2006 ... to display these values got from database
                    rx = base.CurrentRx;
                    //Start Added this code on July 04 2006 ... to display these values got from database
                    if (rx.Quantity != 0)
                        txtQuantity.Text = Convert.ToDouble(rx.Quantity).ToString();
                    if (rx.Refills != 0)
                        txtRefill.Text = rx.Refills.ToString();

                    chkDAW.Checked = rx.DAW;

                    if (_allowMDD)
                    {
                        txtMDD.Text = rx.MDD;
                        txtMDD.Text = TrimValue(rx.MDD);
                    }

                    if (rx.SigID != null)
                    {
                        string sigID = rx.SigID;

                        if (rx.SigTypeId == (int)SigTypeEnum.SigTypeFreeForm)
                        {
                            if (rx.SigText != null)
                            {
                                txtFreeTextSig.Text = rx.SigText;
                                rdbFreeTextSig.Checked = true;
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

                    if (rx.DaysSupply != 0)
                        txtDaysSupply.Text = rx.DaysSupply.ToString();
                } // ******* End Added the code on July 4th 2006 AKS

                if (rx.DDI != null) //coming from freeform drug
                {
                    DataSet ds = Allscripts.Impact.Medication.Load(rx.DDI, null, base.DBID);

                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        string gpi = ds.Tables[0].Rows[0]["GPI"].ToString();
                        string RouteOfAdmincode = ds.Tables[0].Rows[0]["RouteOfAdminCode"].ToString();
                        DosageFormCode = ds.Tables[0].Rows[0]["DosageFormCode"].ToString();
                        string ProviderId = base.Session[Constants.SessionVariables.PhysicianId] != null ? base.Session[Constants.SessionVariables.PhysicianId].ToString() : System.Guid.Empty.ToString();
                        PrefSigObjectDataSource.SelectParameters.Add("gpi", gpi);
                        PrefSigObjectDataSource.SelectParameters.Add("routeOfAdminCode", RouteOfAdmincode);
                        PrefSigObjectDataSource.SelectParameters.Add("dosageFormCode", DosageFormCode);
                        PrefSigObjectDataSource.SelectParameters.Add("providerID", ProviderId);

                        LstPreferedSig.DataSourceID = "PrefSigObjectDataSource";
                        PrefSigObjectDataSource.Select();
                    }

                    if (rx.SigID != null)
                    {
                        string sigID = rx.SigID;

                        if (rx.SigTypeId == (int)SigTypeEnum.SigTypeFreeForm)
                        {
                            if (rx.SigText != null)
                            {
                                txtFreeTextSig.Text = rx.SigText;
                                rdbFreeTextSig.Checked = true;
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

                    if (rx.Notes != null)
                    {
                        txtPharmComments.Text = rx.Notes;
                    }

                    //******** Added code on July 31 to make sure the patiend edu is not shown when the valeu sare not there
                    DataSet dsPatientEdu = CHPatient.GetPatientEducation(rx.DDI, Session["LICENSEID"].ToString(),
                        Session["USERID"].ToString(), base.DBID);
                    if (dsPatientEdu.Tables[0].Rows.Count <= 0)
                        btnPatientEdu.Visible = false;
                }

                if (Request.FilePath.ToLower().Contains(Constants.PageNames.NURSE_FULL_SCRIPT.ToLower()))
                {
                    if (rx.Quantity != 0)
                        txtQuantity.Text = Convert.ToDouble(rx.Quantity).ToString();

                    txtRefill.Text = rx.Refills.ToString();

                    chkDAW.Checked = rx.DAW;

                    if (rx.DaysSupply != 0)
                        txtDaysSupply.Text = rx.DaysSupply.ToString();
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
                    }
                    else
                        pnlNonPillMed.Visible = false;
                }
                else
                {
                    pnlNonPillMed.Visible = false;
                }

                //--------------------------------------------------------------------------------------------------------------
                if (Session["PATIENTID"] == null)
                {
                    btnChooseDestination.Enabled = false; //  AKS SEP 18th  2006
                }

                //dhiraj 26 oct 06
                //for controlledsubstance drug,send To Admin and Choose Pharmacy button should be disable
                //only we can take a print out.
                if (!string.IsNullOrEmpty(rx.ControlledSubstanceCode))
                {
                    string controlledSubstanceCode = rx.ControlledSubstanceCode;

                    if (controlledSubstanceCode.Trim() == "2")
                    {
                        rvDaysSupply.MinimumValue = "1";
                        rvDaysSupply.MaximumValue = "90";
                        rvDaysSupply.ErrorMessage = "For Schedule II drugs, days supply must be a whole number between 1 and 90.";
                        if (!string.IsNullOrEmpty(PageState.GetStringOrEmpty(Constants.SessionVariables.TaskScriptMessageId)))
                        {
                            txtRefill.Text = "1";
                            lblRefills.Text = "Dispenses: ";
                            RangeValidatorRefill.MinimumValue = "1";
                            RangeValidatorRefill.MaximumValue = "1";
                            RangeValidatorRefill.ErrorMessage = "For Schedule II drugs, dispenses must be 1.";
                        }
                        else
                        {
                            txtRefill.Text = "0";
                            RangeValidatorRefill.MaximumValue = "0";
                            RangeValidatorRefill.ErrorMessage = "For Schedule II drugs, refills must be 0.";
                        }
                    }
                    else if (controlledSubstanceCode.Trim() == "3" || controlledSubstanceCode.Trim() == "4" ||
                             controlledSubstanceCode.Trim() == "5")
                    {
                        int daysupply = (DateTime.Now.AddMonths(6) - DateTime.Now.Date).Days;
                        rvDaysSupply.MinimumValue = "1";
                        rvDaysSupply.MaximumValue = daysupply.ToString();
                        rvDaysSupply.ErrorMessage =
                            "For Schedule III, IV and V drugs, days supply must be a whole number between 1 and " +
                            daysupply.ToString() + ".";

                        if (!string.IsNullOrEmpty(PageState.GetStringOrEmpty(Constants.SessionVariables.TaskScriptMessageId)))
                        {
                            lblRefills.Text = "Dispenses: ";
                            RangeValidatorRefill.MinimumValue = "1";
                            RangeValidatorRefill.MaximumValue = "6";
                            RangeValidatorRefill.ErrorMessage = "For Schedule III, IV and V drugs, dispenses must be a whole number between 1 and 6.";
                        }
                        else
                        {
                            RangeValidatorRefill.MaximumValue = "5";
                            RangeValidatorRefill.ErrorMessage = "For Schedule III, IV and V drugs, refills must be a whole number between 0 and 5.";
                        }

                    }
                }
                string med = getMedName();
                if (med != string.Empty)
                {
                    lblMedInfo.Text = "Choose or write a SIG for " + med + ":";
                }
                else
                {
                    lblMedInfo.Text = "Choose or write a SIG:";
                }

                Parameter pddi = SigAllObjDataSource.SelectParameters["DDI"];
                if (pddi != null)
                    SigAllObjDataSource.SelectParameters.Remove(pddi);

                SigAllObjDataSource.SelectParameters.Add("DDI", base.CurrentRx.DDI);

                pddi = LatinSigObjDataSource.SelectParameters["ddi"];
                if (pddi != null)
                    LatinSigObjDataSource.SelectParameters.Remove(pddi);

                LatinSigObjDataSource.SelectParameters.Add("ddi", base.CurrentRx.DDI);

                toggleSIGPanels();
            }

            if (string.IsNullOrEmpty(rx.DDI))
            {
                ddlUnit.Visible = true;
            }

            if (base.IsPOBUser)
            {
                if (base.SupervisingProvider != null)
                {
                    ucMessage.Visible = true;
                    ucMessage.MessageText = "Prescription being written under the supervision of " +
                                            base.SupervisingProviderName + ".";
                    ucMessage.Icon = Controls_Message.MessageType.INFORMATION;
                }
                else
                {
                    ucMessage.Visible = true;
                    ucMessage.MessageText = "Prescription written on behalf of " + base.DelegateProviderName + ".";
                    ucMessage.Icon = Controls_Message.MessageType.INFORMATION;
                }
            }
            else
            {
                if (Convert.ToBoolean(Session["IsPA"]) || Convert.ToBoolean(Session["IsPASupervised"]))
                {
                    ucMessage.Visible = true;
                    ucMessage.MessageText = string.Format("Prescription being written under the supervision of {0}.", DelegateProviderName);


                    ucMessage.Icon = Controls_Message.MessageType.INFORMATION;
                }
                else
                {
                    ucMessage.Visible = true;
                    ucMessage.MessageText =string.Format("Prescription written on behalf of {0}." , DelegateProviderName);
                    ucMessage.Icon = Controls_Message.MessageType.INFORMATION;
                }
            }

            //Check for if editing SIG info --subhasis       
            if (Request.QueryString["Mode"] == "Edit")
            {
                if (Request.QueryString["To"] != null &&
                    !Request.QueryString["To"].Equals(Constants.PageNames.SCRIPT_PAD, StringComparison.OrdinalIgnoreCase))
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
                btnChangeMed.Visible = false; //Remove Change Med button for Edit mode-#2945 
            }

        }

        private string getMedName()
        {
            string med = string.Empty;

            if (Session["RxList"] != null)
            {
                //there should only be 1 item in the list here
                ArrayList rxList = (ArrayList) Session["RxList"];
                Rx rx = (Rx) rxList[0];

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

        protected void Page_PreRenderComplete(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (_isCSMed)
                {
                    if (PageState.GetStringOrEmpty(Constants.SessionVariables.TaskScriptMessageId).Equals(string.Empty))
                    {
                        chkRegistryChecked.Visible = false;
                    }
                    else
                    {
                        chkRegistryChecked.Visible = CsMedUtil.ShouldShowCsRegistryControls(PageState);
                    }

                }
                else
                {
                    chkRegistryChecked.Visible = false;
                }
                btnCheckRegistry.Visible = chkRegistryChecked.Visible;

            }
        }

        protected void Page_LoadComplete(object sender, EventArgs e)
        {
            ((PhysicianMasterPage) Master).hideTabs();
        }

        //From Erik code , integratedon 3 oct 06
        private void LstSig_DataBound(object sender, EventArgs e)
        {
            if (Session[Constants.SessionVariables.TaskScriptMessageId] != null)
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
            }
            if (LstSig.Items.Count == 1)
            {
                LstSig.SelectedIndex = 0;
            }
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
                rx.SigID = drRX["SIGID"].ToString().Trim();
                rx.SigTypeId = Convert.ToInt32(drRX["SIGType"].ToString());
                string mddText = String.Empty;
                //Remove maximum daily dosage frp, sigtext if exist than
                rx.SigText = getParsedSigText(drRX["SIGTEXT"].ToString(), out mddText);
                rx.MDD = mddText;
                if (String.IsNullOrWhiteSpace(mddText))
                {
                    //if mdd exists on a script set the field to visible regardless of site preference
                    SetMDDFieldsVisablity(true);
                }
                rx.MedicationName = drRX["MedicationName"].ToString();
                rx.Strength = drRX["STRENGTH"].ToString();
                rx.StrengthUOM = drRX["STRENGTHUOM"].ToString();
                rx.DosageFormDescription = drRX["DosageForm"].ToString();
                rx.RouteOfAdminDescription = drRX["RouteofAdmin"].ToString();
                rx.ICD9Code = drRX["ICD9code"].ToString();
                rx.ICD10Code = drRX["ICD10code"].ToString();
                Session["DIAGNOSIS"] = string.Empty; //Added April 24 after discussion with PD  

                Session["SIGTEXT"] = rx.SigText; //Added for issue#2763
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
                rx.DaysSupply = Convert.ToInt32(drRX["DaysSupply"].ToString().Trim());
                    //Added on July 04 AKS for displaying the daystosupply
                rx.ControlledSubstanceCode = drRX["ControlledSubstanceCode"].ToString(); //added by dhiraj on 19/11/06
                rx.OriginalRxID = rxID;
                rx.PackageDescription = drRX["PackageDescription"] != DBNull.Value
                    ? drRX["PackageDescription"].ToString().Trim()
                    : string.Empty;

                setMDDInfo(rx.ControlledSubstanceCode);

                //added for editing SIG info --subhasis
                if (Request.QueryString["Mode"] == "Edit")
                {
                    rx.DAW = drRX["DAW"].ToString().Equals("Y") ? true : false;
                    rx.Notes = drRX["PharmacyNotes"].ToString().Trim();
                }

                ArrayList rxList = new ArrayList();
                rxList.Add(rx);
                Session["RxList"] = rxList;
            }
        }

        private string TrimValue(string value)
        {
            string result = value.TrimEnd();
            result = result.TrimStart();
            return result;
        }

        private void setRefillInfo(string scriptMessageID)
        {
            ScriptMessage sm = new ScriptMessage(scriptMessageID, Session["LICENSEID"].ToString(),
                Session["USERID"].ToString(), base.DBID);

            // save provider ID for further processing
            _refillRequestProviderID = sm.ProviderID;

            defaultResponseType.Value = "A";
            lblRefills.Text = DispenseLabel; // if pharmacy refill workflow, change label
            RangeValidatorRefill.MinimumValue = "1";
            RangeValidatorRefill.ErrorMessage = "Dispenses must be a whole number between 1 and 99";


            ucMessageHeader.Icon = Controls_Message.MessageType.INFORMATION;
            ucMessageHeader.MessageText = "Renewal Request Pharmacy :  " +
                                          Allscripts.Impact.Utilities.StringHelper.GetPharmacyName(
                                              sm.PharmacyName,
                                              sm.PharmacyAddress1,
                                              sm.PharmacyAddress2,
                                              sm.PharmacyCity,
                                              sm.PharmacyState,
                                              sm.PharmacyZip,
                                              sm.PharmacyPhoneNumber);
            ucMessageHeader.Visible = true;

            //EAK added to handle case where different med is selected
            if (sm.DBDDID == base.CurrentRx.DDI || string.IsNullOrWhiteSpace(base.CurrentRx.DDI) ||
                string.IsNullOrWhiteSpace(sm.DBDDID))
            {
                Session["SIGTEXT"] = txtFreeTextSig.Text = sm.DispensedRxSIGText;
                Session["MEDICATIONNAME"] = sm.DispensedRxDrugDescription;
                Session["DIAGNOSIS"] = string.Empty;
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

            Session["QUANTITY"] = txtQuantity.Text = !string.IsNullOrEmpty(sm.DispensedRxQuantity) ? Convert.ToDouble(sm.DispensedRxQuantity).ToString() : string.Empty;
            Session["DaysSupply"] = txtDaysSupply.Text = sm.DispensedDaysSupply;

            if (sm.DispensedDaw.ToUpper() == Constants.CommonAbbreviations.YES || sm.DispensedDaw == "1")
            {
                chkDAW.Checked = true;
            }
            else
            {
                chkDAW.Checked = false;
            }

            if (sm.DispensedRxSIGText.Length > 0)
            {
                rdgPrefer.Checked = false;
                rdbAllSig.Checked = true;
                //RDR ID 2589 - Unchecked other radio button with in  the group .
                rdbFreeTextSig.Checked = false;
                LstLatinSig.SelectedIndex = 0;
            }

            lblDaysSupplyAsterisk.Visible = false;
            rfvDaysSupply.Enabled = false;
            rvDaysSupply.Enabled = false;
            revRefillReq.ErrorMessage = "Please enter total number of dispenses approved.";

            rdbFreeTextSig.Checked = true;
            rdbAllSig.Checked = false;
            rdgPrefer.Checked = false;

            toggleSIGPanels();
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
                Session[Constants.SessionVariables.SIGID] = rx.SigID = SigID;
            }

            rx.Notes = txtPharmComments.Text;
            rx.Quantity = decimal.Parse(txtQuantity.Text.Trim());
            rx.Refills = int.Parse(txtRefill.Text.Trim());
            if (lblRefills.Text.Equals(DispenseLabel))
            {
                //If text is dispenses, make sure to only save refills (i.e. subtract 1)
                rx.Refills = int.Parse(txtRefill.Text) - 1;
            }

            rx.DAW = chkDAW.Checked;
            if (!string.IsNullOrWhiteSpace(txtDaysSupply.Text))
            {
                rx.DaysSupply = int.Parse(txtDaysSupply.Text.Trim());
            }

            if (PageState.GetStringOrEmpty(Constants.SessionVariables.SIGID) == string.Empty)
            {
                Session[Constants.SessionVariables.SIGID] = System.Guid.Empty.ToString();
            }

            if (Session["Package"] != null && ddlCustomPack.SelectedItem != null)
            {
                DataTable dtPackage = Session["Package"] as DataTable;
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

            if (_allowMDD)
            {
                rx.MDD = txtMDD.Text.Trim();
            }

            ArrayList rxList = new ArrayList();
            rxList.Add(rx);
            Session["RxList"] = rxList;

            Session["PHYSICIANID"] = DelegateProvider.UserID;
        }

        protected void LstLatinSig_SelectedIndexChanged(object sender, EventArgs e)
        {
            SigAllObjDataSource.Select();
        }

        protected string CalculateDays(string SigID)
        {
            //Added by AKS on July 15th 
            //Modified this on July 24th 

            string QtyValue = string.Empty;
            if (SigID != null && SigID != string.Empty) // If Sig ID is there then proceed..
            {
                DataSet ds = Allscripts.Impact.Medication.ChGetDailyQuanity(SigID, base.DBID);
                decimal DailyQty = 0;
                if (ds.Tables[0].Rows.Count > 0)
                {
                    DailyQty = Convert.ToDecimal(ds.Tables[0].Rows[0]["DailyQuantity"].ToString());
                    //Got the daily quantity.
                }
                if (DailyQty == 0)
                    return "0";
                //Lets get the package size and package quantity this is availaible in view state.
                decimal PackSize = 1;
                int PackQuantity = 1;

                if (ddlCustomPack.SelectedIndex > -1)
                {
                    DataTable dtPackage = (DataTable) Session["Package"];
                    DataRow[] drPackage =
                        dtPackage.Select("PackageDescription ='" + ddlCustomPack.SelectedItem.Text + "'");
                    if (drPackage.Length > 0)
                    {
                        PackSize = Convert.ToDecimal(drPackage[0]["PackageSize"]);
                        PackQuantity = Convert.ToInt32(drPackage[0]["PackageQuantity"]);

                    }

                }
                int DaysSupply = 0;
                if (txtDaysSupply.Text.Length > 0)
                    DaysSupply = Convert.ToInt32((txtDaysSupply.Text));
                if (DaysSupply > 0)
                {
                    QtyValue = Convert.ToDecimal((DailyQty*DaysSupply)/(PackQuantity*PackSize)).ToString();

                }

            }
            return QtyValue;
        }

        #region Maximum Daily Dosage

        private void setMDDInfo(string hasControlledSustanceCode)
        {
            bool isACSMed = !String.IsNullOrWhiteSpace(hasControlledSustanceCode);
            _allowMDD = PageState.GetBooleanOrFalse("ALLOWMDD");
            if (_allowMDD)
            {
                _MDDCSMedOnly = PageState.GetBooleanOrFalse("CSMEDSONLY");
            }
            if (_allowMDD)
            {
                if (_MDDCSMedOnly)
                {
                    if (isACSMed)
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

        private void SaveMaximumDailyDosage(string rxID, string MDDValue)
        {
            Allscripts.Impact.Prescription.SaveMaximumDailyDosage(rxID, MDDValue, base.DBID);
        }

        #endregion Maximum Daily Dosage

        protected void PrefSigObjectDataSource_Selected(object sender, ObjectDataSourceStatusEventArgs e)
        {
            //for refreq workflow, we should always show free form sig
            if (Session[Constants.SessionVariables.TaskScriptMessageId] == null)
            {
                if (((DataSet) e.ReturnValue).Tables[0].Rows.Count == 0)
                {
                    rdbAllSig.Checked = true;
                    rdgPrefer.Enabled = false;
                }
                if (((DataSet) e.ReturnValue).Tables[0].Rows.Count == 1)
                {
                    LstPreferedSig.SelectedIndex = 0;
                }

                toggleSIGPanels();
            }
            else
            {
                rdgPrefer.Enabled = false;
            }
        }

        protected void btnChangeMed_Click(object sender, EventArgs e)
        {
            Response.Redirect(Constants.PageNames.REDIRECT_TO_ANGULAR + "?componentName=" + Constants.PageNames.SELECT_MEDICATION, true);
        }

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
         
            //Added if condition for editing SIG info ---subhasis
            if (Request.QueryString["Mode"] == "Edit")
            {
                editSIG();

                Session.Remove("NOTES");
                Session.Remove("Package");
                ValidationSummary1.Enabled = false;

                if (Request.QueryString["To"] != null)
                {
                    if (Request.QueryString["To"].Contains("SelectMedication"))
                    {
                        Server.Transfer(Constants.PageNames.RX_PROCESSOR + "?From=" + Constants.PageNames.NURSE_SIG + 
                            "&To=" + Constants.PageNames.REDIRECT_TO_ANGULAR + "?componentName=" + Constants.PageNames.SELECT_MEDICATION + "&Action=EditScriptPad&RxID=" + 
                            Request.QueryString["RxId"]);
                    }
                    else
                    {
                        Server.Transfer(Constants.PageNames.RX_PROCESSOR + "?From=" + Constants.PageNames.NURSE_SIG + 
                            "&To=" + Request.QueryString["To"] + "&Action=EditScriptPad&RxID=" + 
                            Request.QueryString["RxId"]);
                    }
                }
                else
                {
                    List<Rx> rxList = new List<Rx>();
                    Rx rx = new Rx();
                    rx.DDI = base.CurrentRx.DDI;
                    rxList.Add(rx);

                    DataSet dsActiveScripts = Prescription.CheckPatientRxHistoryForMultiSelect(base.CurrentRx.DDI,
                        base.SessionPatientID, base.DBID);

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

                if (((Button) sender).ID == btnAddReview.ID)
                {
                    Session["AddAndReview"] = true;
                }
                else
                {
                    Session.Remove("AddAndReview");
                }

                if (validateSig())
                {
                    HttpContext.Current.RewritePath(Request.Path, Request.PathInfo, string.Empty);

                    if (isNewRxWorkflow)
                    {
                        processNEWRX();
                    }
                    else
                    {
                        processTaskingWorkflow();
                    }
                }
            }
        }

        private bool isNewRxWorkflow
        {
            get { return Session[Constants.SessionVariables.TaskScriptMessageId] == null; }
        }

        private void processNEWRX()
        {
            List<Rx> rxList = base.ScriptPadMeds;
            rxList.Add(base.CurrentRx);
            ValidationSummary1.Enabled = false;

            if (PptPlusRequestInfo != null && base.CurrentRx.DDI != null)
            {
                PPTPlus.RequestTransactionForScriptPadMed(PageState, CurrentRx, new PptPlusData(), 
                    new PptPlus(), new Allscripts.ePrescribe.Data.Medication(), new PptPlusServiceBroker()
                    ,new CommonComponentData());
            }

            if (Session["AddAndReview"] != null && Convert.ToBoolean(Session["AddAndReview"]) == true)
            {
                //var ddiList = string.Join(",", from rx in rxList select rx.DDI);

                DataSet dsActiveScripts = Prescription.CheckPatientRxHistoryForMultiSelect(base.CurrentRx.DDI,
                    base.SessionPatientID, base.DBID);
                if (dsActiveScripts.Tables.Count > 1)
                {
                    if ((Convert.ToString(dsActiveScripts.Tables[1].Rows[0][DurPreferDateRange]) == "Y" && Convert.ToString(dsActiveScripts.Tables[1].Rows[0][DurPreferDurDateWarning]) == "Y"))
                    {
                        PageState[Constants.SessionVariables.CheckDur] = true;
                    }
                }
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
            }
            else
            { 
                Server.Transfer(Constants.PageNames.RX_PROCESSOR + "?To=" + (Constants.PageNames.REDIRECT_TO_ANGULAR + "?componentName=" + Constants.PageNames.SELECT_MEDICATION));
            }
        }

        private void processTaskingWorkflow()
        {
            // Check for DURs first.
            // Note: If this is a controlled substance med, the CS check will occur on the DUR page.            
            DURCheckResponse durCheckResponse = DURWarnings;
            bool hasDUR = DUR.HasWarnings(durCheckResponse);
            List<Rx> currentRxList = new List<Rx>();
            currentRxList = PageState.Cast(Constants.SessionVariables.RxList, new ArrayList()).ToList<Rx>();
            List<Rx> rxList = new List<Rx>();

            var taskType = DURMedispanUtils.RetrieveTaskType(MasterPage.RxTask, PageState.Cast(Constants.SessionVariables.TaskType, Constants.PrescriptionTaskType.DEFAULT));
            rxList = DURMedispanUtils.RetrieveDrugsListBasedOnWorkflowType(
                currentRxList,
                ScriptPadMeds,
                taskType);

            if (hasDUR || (DURMedispanUtils.RetrieveFreeFormDrugs(rxList).HasItems() && DURMedispanUtils.IsAnyDurSettingOn(DURSettings)))
            {            
                
                // We can only allow a PA with Supervision or Super POB to process a REFREQ that has a DUR. Regular POBs must send scripts with a DUR to a provider, and that doesn't make sense in the
                // renewal workflow. Limited POBs can't even see the task tab.
                //
                // This page is currently only used by PA's with supervision and all types of POBs                

                if (chkRegistryChecked.Visible)
                {
                    Session["isCSRegistryChecked"] = chkRegistryChecked.Checked;
                }

                // Check here if it is only DUP DUR then allow POB with Some review chance to process and complete the med.

                bool hasDUP = durCheckResponse.DuplicateTherapy.Results.HasItems();
                bool hasOtherDUR = DURMedispanUtils.HasMoreThanDupDUR(durCheckResponse);
                _isOnlyDupDUR = hasDUP && !hasOtherDUR;                

                if (base.SessionUserType == Constants.UserCategory.PHYSICIAN_ASSISTANT_SUPERVISED ||
                    base.SessionUserType == Constants.UserCategory.POB_SUPER ||
                    (base.SessionUserType == Constants.UserCategory.POB_REGULAR && _isOnlyDupDUR))
                {
                    // if DUR found, go to RxDURReviewMultiSelect.aspx for further processing
                    ValidationSummary1.Enabled = false; // "true" will cause the overlay to fail

                    rxList = new List<Rx>();
                    Rx rx = new Rx();
                    rx.DDI = base.CurrentRx.DDI;
                    rxList.Add(rx);

                    if (base.SessionUserType == Constants.UserCategory.PHYSICIAN_ASSISTANT_SUPERVISED)
                    {
                        Session["DUR_GO_PREVIOUS"] = Constants.PageNames.DOC_REFILL_MENU;
                        Session["CameFrom"] = Constants.PageNames.DOC_REFILL_MENU;
                    }
                    else
                    {
                        Session["DUR_GO_PREVIOUS"] = Constants.PageNames.PHARMACY_REFILL_SUMMARY;
                        Session["CameFrom"] = Constants.PageNames.PHARMACY_REFILL_SUMMARY;
                    }

                    Session["POBRefReq"] = true;
                    Session["DUPDUR"] = true;
                        //As it is only DUP DUR (_isOnlyDupDUR) We dont need to show DUR page on complete medication overlay if all the meds are checked.

                    DataSet dsActiveScripts = Prescription.CheckPatientRxHistoryForMultiSelect(base.CurrentRx.DDI,
                        base.SessionPatientID, base.DBID);

                    if (dsActiveScripts.Tables[0].Rows.Count > 0)
                    {
                        if (isCurrentRxAControlledSubstance())
                        {
                            ucMedicationHistoryCompletion.IsCSMed = true;
                        }

                        ucMedicationHistoryCompletion.ActiveScripts = dsActiveScripts;
                        ucMedicationHistoryCompletion.SearchValue = string.Empty;
                        ucMedicationHistoryCompletion.HasOtherDUR = hasOtherDUR;
                        ucMedicationHistoryCompletion.LoadHistory();
                    }
                    else
                    {
                        redirectToNextPage(true);
                    }
                }
                else
                {
                    ucMessage.Icon = Controls_Message.MessageType.ERROR;
                    if (taskType == Constants.PrescriptionTaskType.RXCHG)
                    {
                        ucMessage.MessageText =
                            "You do not have permission to process this Rx Change request because a DUR alert is presented.";
                    }
                    else if (taskType == Constants.PrescriptionTaskType.REFREQ)
                    {
                        ucMessage.MessageText =
                            "You do not have permission to process this renewal request because a DUR alert is presented.";
                    }
                    ucMessage.Visible = true;
                }
            }
            else if (isCurrentRxAControlledSubstance())
            {
                //
                // if no DURs are found, check if this is a controlled substance. If it is, pop the overlay.
                //

                ValidationSummary1.Enabled = false;
                modalCSWarningPopup.Show();
                return;
            }
            else
            {
                //
                // if no DURs are found and the med is not a CS, process normally.
                //

                decimal metricQuantity = base.CurrentRx.Quantity;

                if ((base.CurrentRx.PackageSize * base.CurrentRx.PackageQuantity) > 0)
                {
                    metricQuantity = base.CurrentRx.Quantity * base.CurrentRx.PackageSize *
                                     base.CurrentRx.PackageQuantity;
                }

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
                    delegateProviderID = Session["DelegateProviderID"] != null
                        ? Session["DelegateProviderID"].ToString()
                        : base.SessionUserID;
                }

                long serviceTaskID = -1;

                if (string.IsNullOrWhiteSpace(base.CurrentRx.DDI))
                {
                    ScriptMessage.ApproveFreeFormMessage(
                        Session[Constants.SessionVariables.TaskScriptMessageId].ToString(),
                        base.CurrentRx.MedicationName,
                        base.CurrentRx.DaysSupply, metricQuantity, base.CurrentRx.Refills,
                        base.CurrentRx.SigText, chkDAW.Checked, base.SessionUserID, base.CurrentRx.Notes,
                        Constants.PrescriptionTransmissionMethod.SENT,
                        base.SessionLicenseID, base.SessionUserID, base.SessionSiteID, base.ShieldSecurityToken,
                        base.CurrentRx.IsCompoundMed, base.CurrentRx.HasSupplyItem, delegateProviderID,
                        out serviceTaskID, CurrentRx.MDD, CurrentRx.SigID, base.DBID);
                }
                else
                {
                    if (base.IsPOBUser && Session["SUPERVISING_PROVIDER_ID"] != null)
                    {
                        //
                        // pass in the correct provider for POB user when processing refill request for PA with supervision (and selected supervising provider)
                        //
                        ScriptMessage.ApproveMessage(
                            Session[Constants.SessionVariables.TaskScriptMessageId].ToString(), base.CurrentRx.DDI,
                            base.CurrentRx.DaysSupply, metricQuantity,
                            base.CurrentRx.Refills,
                            base.CurrentRx.SigText, chkDAW.Checked, _refillRequestProviderID, base.CurrentRx.Notes,
                            Constants.PrescriptionTransmissionMethod.SENT,
                            base.SessionLicenseID, base.SessionUserID, base.SessionSiteID, base.ShieldSecurityToken,
                            delegateProviderID, out serviceTaskID, CurrentRx.MDD, CurrentRx.SigID, base.DBID);
                    }
                    else
                    {
                        ScriptMessage.ApproveMessage(
                            Session[Constants.SessionVariables.TaskScriptMessageId].ToString(), base.CurrentRx.DDI,
                            base.CurrentRx.DaysSupply, metricQuantity,
                            base.CurrentRx.Refills,
                            base.CurrentRx.SigText, chkDAW.Checked, delegateProviderID, base.CurrentRx.Notes,
                            Constants.PrescriptionTransmissionMethod.SENT,
                            base.SessionLicenseID, base.SessionUserID, base.SessionSiteID, base.ShieldSecurityToken,
                            delegateProviderID, out serviceTaskID, CurrentRx.MDD, CurrentRx.SigID, base.DBID);
                    }
                }
                string message = string.Empty;
                if (taskType == Constants.PrescriptionTaskType.RXCHG)
                {
                    message = "Rx Change for " + base.CurrentRx.MedicationName + " approved for " +
                              Session["PATIENTNAME"].ToString().Trim() + ".";
                }
                else if(taskType == Constants.PrescriptionTaskType.REFREQ)
                {
                    message = "Refill for " + base.CurrentRx.MedicationName + " approved for " +
                              Session["PATIENTNAME"].ToString().Trim() + ".";
                }
                clearMedicationInfo();
                Session.Remove("NOTES");
                Session.Remove(Constants.SessionVariables.TaskScriptMessageId);
                Session.Remove("Package");
                Session.Remove("SUPERVISING_PROVIDER_ID");

                if (Session["CameFrom"] != null && Session["CameFrom"].ToString() != string.Empty)
                {
                    string cameFrom = Session["CameFrom"].ToString();
                    Session.Remove("CameFrom");
                    Response.Redirect(cameFrom, true);
                }
                else
                {
                    if (base.IsUserAPrescribingUserWithCredentials)
                    {
                        Response.Redirect(Constants.PageNames.APPROVE_REFILL_TASK + "?Msg=" + message, true);
                    }
                    else
                    {
                        Response.Redirect(Constants.PageNames.PHARMACY_REFILL_SUMMARY + "?Msg=" + message, true);
                    }
                }
            }
        }

        /// <summary>
        /// Checks the CurrentRx federal and state controlled substance code and reconciles them to determine if the currentRx is a controlled substance.
        /// </summary>
        /// <returns>True if the med is a controlled substance; else false.</returns>
        private bool isCurrentRxAControlledSubstance()
        {
            bool isCS = false;

            if (!string.IsNullOrEmpty(base.CurrentRx.ControlledSubstanceCode) ||
                !string.IsNullOrEmpty(base.CurrentRx.StateControlledSubstanceCode))
            {
                string reconciledControlledSubstanceCode =
                    Prescription.ReconcileControlledSubstanceCodes(base.CurrentRx.ControlledSubstanceCode,
                        base.CurrentRx.StateControlledSubstanceCode);

                if (
                    !(string.IsNullOrEmpty(reconciledControlledSubstanceCode) ||
                      reconciledControlledSubstanceCode.Equals("U")))
                {
                    isCS = true;
                }
            }
            else if (base.CurrentRx.IsFreeFormMedControlSubstance)
            {
                isCS = true;
            }

            return isCS;
        }

        private bool validateSig()
        {
            if (Session["SIGTEXT"] == null || Session["SIGTEXT"].ToString() == string.Empty)
            {
                toggleSIGPanels();
                lblSigErrorMsg.Visible = true;
                lblSigErrorMsg.Text = "Please choose a SIG or write free text SIG.";
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
                    lblSigErrorMsg.Text = "Please choose a SIG or write free text SIG.";
                    return false;
                }
            }

            return true;
        }
        
        protected void btnSendToPhy_Click(object sender, EventArgs e)
        {
            setSigInfo();

            if (validateSig())
            {
                HttpContext.Current.RewritePath(Request.Path, Request.PathInfo, string.Empty);
                Session["TASKTYPE"] = Constants.PrescriptionTaskType.APPROVAL_REQUEST;
                Session["SentTo"] = DelegateProviderName;
                if (Session["CameFrom"] != null && Session["CameFrom"].ToString() == Constants.PageNames.FREE_FORM_DRUG)
                {
                    Server.Transfer(Constants.PageNames.RX_PROCESSOR + "?From=" + (Constants.PageNames.REDIRECT_TO_ANGULAR + "?componentName=" + Constants.PageNames.SELECT_MEDICATION) +
                                   "&To=" + Constants.PageNames.START_NEW_RX_PROCESS); //DUR Page Workflow change                
                }
                else
                {
                    Server.Transfer(Constants.PageNames.RX_PROCESSOR + "?From=" + Constants.PageNames.REDIRECT_TO_ANGULAR + "?componentName=" + Constants.PageNames.SELECT_MEDICATION + "&To=" +
                                    Constants.PageNames.START_NEW_RX_PROCESS); //DUR Page Workflow change
                }
            }
        }

        protected void btnSendToPhyPharm_Click(object sender, EventArgs e)
        {
            setSigInfo();

            if (validateSig())
            {
                HttpContext.Current.RewritePath(Request.Path, Request.PathInfo, string.Empty);
                Session["TASKTYPE"] = Constants.PrescriptionTaskType.APPROVAL_REQUEST;
                Session["SentTo"] = DelegateProviderName;
                if (Session["CameFrom"] != null && Session["CameFrom"].ToString() == Constants.PageNames.FREE_FORM_DRUG)
                {
                    // FORTIFY: Not considered an open re-direct as already redirecting to local page
                    Server.Transfer(Constants.PageNames.RX_PROCESSOR + "?From=" + (Constants.PageNames.REDIRECT_TO_ANGULAR + "?componentName=" + Constants.PageNames.SELECT_MEDICATION) +
                                    "&To=" + Constants.PageNames.START_NEW_RX_PROCESS + "&PatPharm=Y"); //DUR Page Workflow change
                }
                else
                {
                    // FORTIFY: Not considered an open re-direct as already redirecting to local page
                    Server.Transfer(Constants.PageNames.RX_PROCESSOR + "?From=" + Constants.PageNames.REDIRECT_TO_ANGULAR + "?componentName=" + Constants.PageNames.SELECT_MEDICATION + "&To=" +
                                    Constants.PageNames.START_NEW_RX_PROCESS + "&PatPharm=Y");
                        //DUR Page Workflow change
                }
            }
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
                    Response.Redirect(Constants.PageNames.UrlForRedirection(Request.QueryString["To"]));
                else
                    Response.Redirect(Constants.PageNames.SCRIPT_PAD);
            }
            else
            {
                clearMedicationInfo();
                Session.Remove("NOTES");
                Session.Remove("Package");
                //Remove all the parameters before transfer to the new page. 
                HttpContext.Current.RewritePath(Request.Path, Request.PathInfo, string.Empty);
                if (Session[Constants.SessionVariables.TaskScriptMessageId] != null)
                {
                    Session.Remove(Constants.SessionVariables.TaskScriptMessageId);

                    if (base.IsUserAPrescribingUserWithCredentials)
                    {
                        Response.Redirect(Constants.PageNames.DOC_REFILL_MENU);
                    }
                    else
                    {
                        Response.Redirect(Constants.PageNames.PHARMACY_REFILL_SUMMARY);
                    }
                }
                else
                {
                    Response.Redirect(Constants.PageNames.REDIRECT_TO_ANGULAR + "?componentName=" + Constants.PageNames.SELECT_PATIENT);                  
                }
            }
        }

        private void clearMedicationInfo()
        {
            ClearMedicationInfo(false);
        }

        protected void btnPrint_Click(object sender, EventArgs e)
        {
            setSigInfo();
            if (validateSig())
            {
                HttpContext.Current.RewritePath(Request.Path, Request.PathInfo, string.Empty);
                Session["SentTo"] = "Printer";
                Response.Redirect(Constants.PageNames.RX_PROCESSOR + "?From=" + (Constants.PageNames.REDIRECT_TO_ANGULAR + "?componentName=" + Constants.PageNames.SELECT_MEDICATION) +
                    "&PrintScript=YES", true);
            }
        }

        protected void btnSendToPatPharm_Click(object sender, EventArgs e)
        {
            setSigInfo();
            if (validateSig())
            {
               
                HttpContext.Current.RewritePath(Request.Path, Request.PathInfo, string.Empty);
                Response.Redirect(Constants.PageNames.RX_PROCESSOR + "?From=" + (Constants.PageNames.REDIRECT_TO_ANGULAR + "?componentName=" + Constants.PageNames.SELECT_MEDICATION) + "&To=" +
                   Constants.PageNames.START_NEW_RX_PROCESS + "&PatPharm=Y", true);//DUR Page Workflow change
            }
        }

        protected void ddlCustomPack_PreRender(object sender, EventArgs e)
        {
            string package = string.Empty;
            if (Session["PackageSize"] != null && Session["PackageQuantity"] != null)
                package = "[PZ=" + Session["PackageSize"].ToString() + "](PQ=" + Session["PackageQuantity"].ToString() +
                          ")";
            if (package != string.Empty)
                if (ddlCustomPack.Items.FindByValue(package) != null)
                    ddlCustomPack.Items.FindByValue(package).Selected = true;
        }

        protected void btnYes_Click(object sender, EventArgs e)
        {
            Session["isCSRefillNotAllowed"] = true;
            ScriptMessage sm = new ScriptMessage(Session[Constants.SessionVariables.TaskScriptMessageId].ToString(), base.SessionLicenseID, base.SessionUserID,
                base.DBID);

            string rxID = Guid.NewGuid().ToString();

            StringBuilder pharmComments = new StringBuilder();
            DataSet pharmDS = Allscripts.Impact.Pharmacy.LoadPharmacy(sm.DBPharmacyID, base.DBID);

            pharmComments.Append(txtPharmComments.Text.Trim());

            string providerID = base.SessionUserID;

            if (Session["DelegateProviderID"] != null)
            {
                providerID = Session["DelegateProviderID"].ToString();
            }

            string deaNumber = GetDEANumberToBeUsed(PageState);

            Prescription rx = new Prescription();

            string stateCSCodeForPharmacy =
                Allscripts.Impact.Prescription.GetStateControlledSubstanceCode(base.CurrentRx.DDI, null,
                    pharmDS.Tables[0].Rows[0]["State"].ToString(), base.DBID);

            rx.SetHeaderInformation(base.SessionLicenseID, rxID, DateTime.UtcNow.ToString(),
                sm.DBPatientID, providerID, Guid.Empty.ToString(),
                string.Empty, string.Empty, string.Empty, Constants.PrescriptionType.NEW, false, string.Empty,
                Convert.ToInt32(Session["SiteID"].ToString()), Constants.ERX_NOW_RX, null, base.DBID);

            string sigText = getSigText();
            string formattedFreeTextSig = Allscripts.Impact.Sig.ToSigFormat(sigText);
            FreeFormSigInfo sig = Allscripts.Impact.Sig.SaveFreeFormSigTextOrGetInfo(formattedFreeTextSig, Allscripts.ePrescribe.DatabaseSelector.ConnectionStringPointer.SHARED_DB);
            string  SigID = sig.SigID;
            int sigTypeId = sig.SigTypeID;

            rx.AddMedication(
                base.SessionLicenseID, //EAK added
                0, // RxNumber
                sm.DBDDID,
                base.CurrentRx.MedicationName, base.CurrentRx.RouteOfAdminCode, base.CurrentRx.DosageFormCode,
                base.CurrentRx.Strength, base.CurrentRx.StrengthUOM,
                SigID,
                base.CurrentRx.SigText, base.CurrentRx.Refills, base.CurrentRx.Quantity, base.CurrentRx.DaysSupply,
                string.Empty, //gppc
                Convert.ToDecimal(1), "EA", 1, "EA",
                //packageSize, packageUOM, packageQuantity,  packageDescription         
                base.CurrentRx.DAW, //daw
                DateTime.Today.ToString(), //startDate
                Constants.PrescriptionStatus.NEW, //status
                Constants.PrescriptionTransmissionMethod.SENT, //transmissionMethod
                string.Empty, //originalDDI
                0, //originalFormStatusCode
                "N", //originalIsListed
                0, //formStatusCode
                "N", //isListed
                FormularyStatus.NONE,
                Session["PERFORM_FORMULARY"].ToString(),
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
                base.CurrentRx.ControlledSubstanceCode, // medispan (or federal) controlled substance code
                Session["DelegateProviderID"].ToString(),
                null, // lastFillDate
                null, // authorizeByID
                Session["RX_WORKFLOW"] != null
                    ? ((PrescriptionWorkFlow) Convert.ToInt32(Session["RX_WORKFLOW"]))
                    : PrescriptionWorkFlow.STANDARD,
                Session["ExtFacilityCd"] != null ? Session["ExtFacilityCd"].ToString() : null,
                Session["ExtGroupID"] != null ? Session["ExtGroupID"].ToString() : null,
                0, //"coverageID"
                -1, //alternativeIgnoreReason
                stateCSCodeForPharmacy,
                "N", "N", "N", "N", "N", "N", "N", "N",
                //formuAltsShown, preDURDose, preDURPAR, preDURDrugFood, preDURDrugAlcohol, preDURDrugDrug, preDURDUP, preDURDisease 
                false, // priorAuthRequired
                base.CurrentRx.IsCompoundMed,
                false, // isFreeFormMedControlSubstance
                0, // rxScheduleUsed
                base.CurrentRx.HasSupplyItem,
                deaNumber,
                 sigTypeId );

            bool? isCSRegistryChecked = null;

            if (chkRegistryChecked.Visible)
            {
                isCSRegistryChecked = chkRegistryChecked.Checked;
            }

            rx.Save(Convert.ToInt32(Session["SiteID"].ToString()), base.SessionLicenseID,
                Session["DelegateProviderID"].ToString(), true, isCSRegistryChecked, base.DBID);

            EPSBroker.ECouponPrintRefillRequest(rxID, sm.DBDDID, base.CanApplyFinancialOffers, ConfigKeys.AutoSendCoupons, base.DBID);

            SaveMaximumDailyDosage(rxID, base.CurrentRx.MDD);

            ArrayList alProcess = new ArrayList();
            alProcess.Add(rxID);
            Session["ProcessList"] = alProcess;

            if (base.IsPOBUser && !string.IsNullOrWhiteSpace(PageState.GetStringOrEmpty("SUPERVISING_PROVIDER_ID")))
            {
                // update the authorized id as the supervising physician's id
                Prescription.UpdateRxDetailStatus(base.SessionLicenseID, PageState.GetStringOrEmpty("SUPERVISING_PROVIDER_ID"), rxID, "AUTHORIZEBY", base.DBID);
            }

            if (base.IsUserAPrescribingUserWithCredentials)
            {
                Server.Transfer(Constants.PageNames.CSS_DETECT + "?To=" +
                                HttpUtility.HtmlEncode(Constants.PageNames.DOC_REFILL_MENU));
            }
            else
            {
                Server.Transfer(Constants.PageNames.CSS_DETECT + "?To=" +
                                HttpUtility.HtmlEncode(Constants.PageNames.PHARMACY_REFILL_SUMMARY));
            }
        }

        private string getSigText()
        {
            if (!string.IsNullOrEmpty(Session["SIGTEXT"].ToString().Trim()))
            {
                return Session["SIGTEXT"].ToString().Trim();
            }

            if (!string.IsNullOrEmpty(LstSig.SelectedItem.Text.Trim()))
            {
                return LstSig.SelectedItem.Text.Trim();
            }

            if (!string.IsNullOrEmpty(LstPreferedSig.SelectedItem.Text.Trim()))
            {
                return LstPreferedSig.SelectedItem.Text.Trim();
            }

            if (!string.IsNullOrEmpty(txtFreeTextSig.Text.Trim()))
            {
                return txtFreeTextSig.Text.Trim();
            }
            else
            {
                return string.Empty;
            }
        }

        protected void btnContactMe_Click(object sender, EventArgs e)
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

            Session["REFILLMSG"] = "Refill denied.";

            Response.Redirect(Constants.PageNames.PHARMACY_REFILL_SUMMARY);
        }

        private void editSIG()
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
                realQuantity = rx.Quantity*rx.PackageQuantity*rx.PackageSize;
            }
            catch (Exception ex)
            {
                Audit.AddException(base.SessionUserID, base.SessionLicenseID,
                    "Exception when editing SIG from script pad: RxID: " + Request.QueryString["RxID"].ToString() +
                    ex.ToString(), null, null, null, base.DBID);
            }

            //#2650-Added for updating provider favorite med list.
            string ProviderID = Session["PHYSICIANID"].ToString(); //Modified for#2844

            Allscripts.Impact.Prescription.UpdateRxSIG(rx.RxID, rx.SigID, rx.SigText,
                rx.Refills, realQuantity,
                rx.DaysSupply, rx.GPPC, rx.PackageSize, rx.PackageQuantity, rx.PackageUOM, rx.PackageDescription,
                rx.DAW ? "True" : "False",
                rx.Notes, ProviderID, rx.DDI, rx.MedicationName, rx.RouteOfAdminCode, rx.DosageFormCode, rx.Strength,
                rx.StrengthUOM, rx.ICD10Code, rx.SigTypeId, 
                                                  1, base.DBID);
            if (_allowMDD)
            {
                Allscripts.Impact.Prescription.SaveMaximumDailyDosage(Request.QueryString["RxID"].ToString(), rx.MDD,
                    base.DBID);
            }

            if (PptPlusRequestInfo != null && !string.IsNullOrWhiteSpace(CurrentRx.DDI))
            {
                PPTPlus.RequestTransactionForScriptPadMed(PageState, rx, new PptPlusData(), new PptPlus(), 
                    new Allscripts.ePrescribe.Data.Medication(), new PptPlusServiceBroker(), new CommonComponentData());
                PPTPlus.UpdateTransactionIdToRx(PageState, rx.OriginalRxID, new PptPlusData(), base.DBID);
            }
        }

        protected void LstPreferedSig_PreRender(object sender, EventArgs e)
        {
            var sigText = PageState.GetStringOrEmpty("SIGTEXT");

            if (!string.IsNullOrWhiteSpace(sigText))
            {
                var selectedSig = LstPreferedSig.Items.FindByText(sigText.Trim());

                if (selectedSig != null)
                {
                    selectedSig.Selected = true;
                }
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

        protected void lblShim_PreRender(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "shim", ";", true);
        }

        protected void ucMedicationHistoryCompletion_OnMedHistoryComplete(MedHistoryCompletionEventArgs eventArgs)
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
                    refreshTaskDUR();
                    redirectToNextPage(eventArgs.HasOtherDur);
                }
            }
            else if (SessionUserType == Constants.UserCategory.PHYSICIAN_ASSISTANT_SUPERVISED
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

        private void refreshTaskDUR()
        {
            if (!isNewRxWorkflow)
            {
                this.DURWarnings = null;
                // calling the property will reset the task warning.
                var durWarnings = this.DURWarnings;
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
            // POB users, any type, can only print CS med refill requests
            ValidationSummary1.Enabled = false;
            modalCSWarningPopup.Show();
        }

        private void redirectToNextPage(bool isDurFound)
        {
            if (isNewRxWorkflow)
            {
                if (PageState.GetBooleanOrFalse("AddAndReview"))
                {
                    Session.Remove("AddAndReview");
                    Response.Redirect(Constants.PageNames.RX_PROCESSOR + "?From=" + Constants.PageNames.NURSE_SIG +
                                      "&To=" + Constants.PageNames.SCRIPT_PAD + "&Search=" + "?");
                }
                else if (PageState.GetBooleanOrFalse(Constants.SessionVariables.ReviewScriptPadRedirection))
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
                        string UrlForRedirection = Constants.PageNames.UrlForRedirection(Request.QueryString["To"]);

                        if (string.IsNullOrEmpty(UrlForRedirection))
                        {
                            UrlForRedirection = Constants.PageNames.SCRIPT_PAD;
                        }

                        Response.Redirect(
                            Constants.PageNames.RX_PROCESSOR + "?From=" + Constants.PageNames.NURSE_SIG + "&To=" +
                            UrlForRedirection + "&Action=EditScriptPad&RxID=" + rxId, true);
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
                    Response.Redirect(Constants.PageNames.RX_PROCESSOR + "?From=" + Constants.PageNames.NURSE_SIG +
                        "&To=" + Constants.PageNames.SCRIPT_PAD);
                }
            }
            else
            {
                // POBRefReq workflow

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
    }
}
