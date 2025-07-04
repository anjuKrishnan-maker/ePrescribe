using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Allscripts.Impact.PreBuildPrescription;
using Allscripts.ePrescribe.Common;
using Allscripts.Impact;
using Allscripts.ePrescribe.Shared.Logging;
using Allscripts.Impact.Utilities;

namespace eRxWeb
{
    public partial class PreBuiltMedSelectSig : BasePage
    {
        private static readonly ILoggerEx logger = LoggerEx.GetLogger();
        string _packsize = string.Empty;
        string _packageDescription = string.Empty;
        string _dosageFormCode = String.Empty;
        string _ddi = string.Empty;
        string _groupID = string.Empty;
        Rx rx;

        protected void Page_Load(object sender, EventArgs e)
        {
            rx = base.CurrentRx;
            _packsize = StringHelper.StripZeros(rx.PackageSize.ToString());
            _packageDescription = rx.PackageDescription;
            // add client side event handlers to controls
            var maxSigText = 1000;
            txtFreeTextSig.Attributes.Add("onkeydown", $"return LimitInput(this, {maxSigText}, event);");
            txtFreeTextSig.Attributes.Add("onkeyup", $"return LimitInput(this, {maxSigText}, event);");
            txtFreeTextSig.Attributes.Add("onpaste", $"return LimitPaste(this, {maxSigText}, event);");
            txtFreeTextSig.Attributes.Add("onchange", $"return LimitChange(this, {maxSigText}, event);");

            LstSig.DataBound += new EventHandler(LstSig_DataBound);

            // inject javascript to initialize orig sig, days supply, etc.
            ClientScript.RegisterStartupScript(this.GetType(), "InitializeTrackingVariables", "InitializeTrackingVariables();", true);

            // add client side event handlers to controls
            rdgPrefer.Attributes.Add("onclick", "ToggleSIGs('P');");
            rdbAllSig.Attributes.Add("onclick", "ToggleSIGs('A');");
            rdbFreeTextSig.Attributes.Add("onclick", "ToggleSIGs('F');");

            if (!Page.IsPostBack)
            {
                loadMedSig(rx);

                if (Request.QueryString["Mode"] == "Edit")
                {
                    setEditSigInfo(rx);
                    btnAddAnotherMed.Visible = false;
                }
            }
        }

        protected void Page_LoadComplete(object sender, EventArgs e)
        {
            ((PhysicianMasterPageBlank)Master).hideTabs();
        }

        void LstSig_DataBound(object sender, EventArgs e)
        {
            if (sigType.Value == "A" && PageState["SIGTEXT"] != null)
            {
                ListItem li = LstSig.Items.FindByText(PageState["SIGTEXT"].ToString());

                if (li == null)
                {
                    rdbAllSig.Checked = false;
                    rdgPrefer.Checked = false;
                    rdbFreeTextSig.Checked = true;
                    txtFreeTextSig.Text = PageState["SIGTEXT"].ToString();
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
        }

        protected void LstLatinSig_SelectedIndexChanged(object sender, EventArgs e)
        {
            SigAllObjDataSource.Select();
        }

        protected void LstPreferedSig_PreRender(object sender, EventArgs e)
        {
            if (PageState["SIGTEXT"] != null && LstPreferedSig.Items.FindByText(PageState["SIGTEXT"].ToString()) != null)
            {
                if (LstPreferedSig.Items.FindByText(PageState["SIGTEXT"].ToString()) != null)
                {
                    rdbFreeTextSig.Checked = false;
                    rdbAllSig.Checked = false;
                    rdgPrefer.Checked = true;
                    toggleSIGPanels();
                    LstPreferedSig.Items.FindByText(PageState["SIGTEXT"].ToString()).Selected = true;
                }
            }
        }

        protected void ddlCustomPack_PreRender(object sender, EventArgs e)
        {
            string package = string.Empty;

            if (rx.PackageQuantity != 0 && rx.PackageSize != 0)
            {
                package = $"[PZ={ rx.PackageQuantity.ToString()} ](PQ={_packsize })";
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

            if (((DataSet)e.ReturnValue).Tables[0].Rows.Count == 0)
            {
                rdbAllSig.Checked = true;
                rdgPrefer.Checked = false;
                rdgPrefer.Enabled = false;
            }

            if (((DataSet)e.ReturnValue).Tables[0].Rows.Count == 1)
            {
                if (LstPreferedSig.Items.Count > 0)
                {
                    LstPreferedSig.SelectedIndex = 0;
                }
            }

            toggleSIGPanels();
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            if (Request.QueryString["Mode"] == "Edit")
            {
                Response.Redirect(Constants.PageNames.PRE_BUILT_PRESCRIPTION_ADD_OR_EDIT + "?GroupId=" + HttpUtility.UrlEncode(PageState["PreBuiltPrescriptionGroupID"].ToString()));
            }
            else
            {
                Response.Redirect(Constants.PageNames.PRE_BUILT_SELECT_MED);
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (validateSig())
            {
                if (Request.QueryString["Mode"] == "Edit")
                {
                    editPreBuiltRxSig();
                    Response.Redirect(Constants.PageNames.PRE_BUILT_PRESCRIPTION_ADD_OR_EDIT + "?GroupId=" + HttpUtility.UrlEncode(PageState["PreBuiltPrescriptionGroupID"].ToString()));
                }
                else
                {
                    savePreBuiltMed();
                    Response.Redirect(Constants.PageNames.PRE_BUILT_PRESCRIPTION_ADD_OR_EDIT + "?GroupId=" + HttpUtility.UrlEncode(PageState["PreBuiltPrescriptionGroupID"].ToString()));
                }
            }
        }

        protected void btnAddAnotherMed_Click(object sender, EventArgs e)
        {

            if (validateSig())
            {
                savePreBuiltMed();
                PageState["AddAnotherMed"] = true;
                Response.Redirect(Constants.PageNames.PRE_BUILT_SELECT_MED);
            }
        }

        #region Private Methods
        private void setSigInfo()
        {
            string Sig = string.Empty;
            string SigID = string.Empty;

            if (sigType.Value == "P")
            {
                if (LstPreferedSig.SelectedIndex > -1)
                {
                    Sig = LstPreferedSig.SelectedItem.Text;
                    SigID = LstPreferedSig.SelectedValue.ToString();
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
                Sig = txtFreeTextSig.Text;
                //rx.SigTypeId = (int)SigTypeEnum.SigTypeFreeForm;

                try
                {
                    SigID = Allscripts.Impact.Sig.SaveFreeFormSigTextOrGetId(Sig, Allscripts.ePrescribe.DatabaseSelector.ConnectionStringPointer.SHARED_DB);
                    PageState["SIGID"] = rx.SigID = SigID;
                }
                catch (Exception ex)
                {
                    logger.Debug("Error getting SIG ID for free form sig" + ex.Message);
                    return;
                }
            }

            if (Sig != string.Empty)
            {
                PageState["SIGTEXT"] = rx.SigText = Sig;
            }

            if (SigID != string.Empty)
            {
                // if SIG has been appended with [DQ=] if so then remove the same ..
                if (SigID.Contains("[DQ="))
                {
                    int DQindex = SigID.IndexOf("[DQ=");
                    SigID = SigID.Substring(0, DQindex);
                }

                PageState["SIGID"] = rx.SigID = SigID;
            }


            rx.Quantity = decimal.Parse(txtQuantity.Text.Trim());
            rx.Refills = int.Parse(txtRefill.Text.Trim());
            rx.DAW = chkDAW.Checked;
            if (!string.IsNullOrWhiteSpace(txtDaysSupply.Text))
            {
                rx.DaysSupply = int.Parse(txtDaysSupply.Text.Trim());
            }

            if (PageState["Package"] != null && ddlCustomPack.SelectedItem != null)
            {
                DataTable dtPackage = PageState["Package"] as DataTable;
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

            PageState["RxList"] = rxList;
        }

        private void toggleSIGPanels()
        {
            pnlPreferedSig.Style["display"] = "none";
            pnlAllSig.Style["display"] = "none";
            pnlFreeTextSig.Style["display"] = "none";

            if (rdgPrefer.Checked)
            {
                sigType.Value = "P";
                pnlPreferedSig.Style["display"] = "inline";
            }
            else if (rdbAllSig.Checked)
            {
                sigType.Value = "A";
                pnlAllSig.Style["display"] = "inline";
            }
            else
            {
                sigType.Value = "F";
                pnlFreeTextSig.Style["display"] = "inline";
                rdbFreeTextSig.Checked = true;
            }
        }

        private void savePreBuiltMed()
        {
            string Sig = string.Empty;
            string SigID = string.Empty;

            if (sigType.Value == "P")
            {
                if (LstPreferedSig.SelectedIndex > -1)
                {
                    Sig = LstPreferedSig.SelectedItem.Text;
                    SigID = LstPreferedSig.SelectedValue.ToString();
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
                Sig = txtFreeTextSig.Text;
                //rx.SigTypeId = (int)SigTypeEnum.SigTypeFreeForm;

                try
                {
                    SigID = Allscripts.Impact.Sig.SaveFreeFormSigTextOrGetId(Sig, Allscripts.ePrescribe.DatabaseSelector.ConnectionStringPointer.SHARED_DB);
                }
                catch (Exception ex)
                {
                    logger.Debug("Error getting SIG ID for free form sig" + ex.Message);
                    return;
                }
            }

            if (SigID != string.Empty)
            {
                // check if SIG has been appended with [DQ=] if so then remove the same ..
                if (SigID.Contains("[DQ="))
                {
                    int DQindex = SigID.IndexOf("[DQ=");
                    SigID = SigID.Substring(0, DQindex);
                }
            }

            decimal Quantity = 0;

            if (!string.IsNullOrWhiteSpace(txtQuantity.Text))
            {
                Quantity = decimal.Parse(txtQuantity.Text.Trim());
            }
            int Refills = 0;

            if (!string.IsNullOrWhiteSpace(txtRefill.Text))
            {
                Refills = int.Parse(txtRefill.Text.Trim());
            }
            bool daw = chkDAW.Checked;
            int DaysSupply = 0;
            decimal packageSize = 1;
            string packageUOM = "EA";
            int packageQuantity = 1;
            string packageDescription = "EA";

            if (!string.IsNullOrWhiteSpace(txtDaysSupply.Text))
            {
                DaysSupply = int.Parse(txtDaysSupply.Text.Trim());
            }

            if (PageState["Package"] != null && ddlCustomPack.SelectedItem != null)
            {
                DataTable dtPackage = PageState["Package"] as DataTable;
                DataRow[] drPackage = dtPackage.Select("PackageDescription ='" + ddlCustomPack.SelectedItem.Text + "'");

                if (drPackage.Length > 0)
                {
                    if (!string.IsNullOrWhiteSpace(drPackage[0]["PackageSize"].ToString()))
                    {
                        packageSize = Convert.ToDecimal(drPackage[0]["PackageSize"].ToString());
                    }
                    packageUOM = drPackage[0]["PackageUOM"].ToString();
                    if (!string.IsNullOrWhiteSpace(drPackage[0]["PackageQuantity"].ToString()))
                    {
                        packageQuantity = Convert.ToInt32(drPackage[0]["PackageQuantity"].ToString());
                    }
                    packageDescription = drPackage[0]["PackageDescription"].ToString();
                }
            }

            if (PageState["RxList"] != null)
            {
                ArrayList rxList = (ArrayList)PageState["RxList"];
                Rx rx = (Rx)rxList[0];

                if (string.IsNullOrEmpty(rx.DDI))
                {
                    packageSize = 1;
                    packageUOM = ddlUnit.SelectedValue;
                    packageQuantity = 1;
                    packageDescription = ddlUnit.SelectedValue;
                }

                PreBuildPrescription preBuildPrescription = new PreBuildPrescription();

                string groupID = Request.QueryString["groupID"].Trim();

                preBuildPrescription.SavePreBuiltPrescription(Convert.ToInt32(groupID), rx.DDI, rx.MedicationName, rx.RouteOfAdminCode, rx.DosageFormCode,
                    rx.Strength, rx.StrengthUOM, packageSize, packageUOM, packageQuantity, packageDescription, SigID, Quantity,
                    Refills, DaysSupply, daw ? "True" : "False", base.SessionUserID, base.DBID);
            }
        }

        private void setEditSigInfo(Rx rx)
        {
            if (rx.Quantity != 0)
            {
                txtQuantity.Text = Convert.ToDouble(rx.Quantity).ToString();
            }

            txtRefill.Text = rx.Refills.ToString();
            chkDAW.Checked = rx.DAW;

            if (rx.DaysSupply != 0)
            {
                txtDaysSupply.Text = rx.DaysSupply.ToString();
            }

            LstLatinSig.SelectedValue = "All";

            if (rx.SigID != null)
            {
                string sigID = rx.SigID;

                if (sigID.Equals(Guid.Empty.ToString()))
                {
                    if (rx.SigText != null)
                    {
                        txtFreeTextSig.Text = rx.SigText;
                        rdbFreeTextSig.Checked = true;
                        rdbAllSig.Checked = false;
                        rdgPrefer.Checked = false;

                        if (rx.DDI != null)
                        {
                            //if the med ddi is not present, then the preBuiltRx is a free text drug,
                            //then disable the prefer sig and all sigs options because they don't apply.
                            if (string.IsNullOrWhiteSpace(rx.DDI))
                            {
                                rdbAllSig.Enabled = false;
                                rdgPrefer.Enabled = false;
                            }
                        }
                    }
                }
            }
        }
        private void editPreBuiltRxSig()
        {
            setSigInfo();
            Rx rx = base.CurrentRx;

            //TODO : check for quantity & real quantity.

            PreBuildPrescription preBuildPrescription = new PreBuildPrescription();
            preBuildPrescription.UpdatePreBuiltPrescriptionSIG(Request.QueryString["ScriptID"].ToString(), rx.SigID, rx.Quantity, rx.Refills,
                rx.DaysSupply, rx.PackageSize, rx.PackageQuantity, rx.PackageUOM, rx.PackageDescription, rx.DAW ? "True" : "False", base.SessionUserID, DBID);

        }
        private void loadMedSig(Rx rx)
        {
            if (!string.IsNullOrWhiteSpace(rx.DDI))
            {
                DataSet dsMed = Allscripts.Impact.Medication.Load(rx.DDI, null, base.DBID);

                if (dsMed.Tables[0].Rows.Count > 0)
                {
                    string gpi = dsMed.Tables[0].Rows[0]["GPI"].ToString();
                    string routeOfAdmincode = dsMed.Tables[0].Rows[0]["RouteOfAdminCode"].ToString();
                    string ndc = dsMed.Tables[0].Rows[0]["NDC"].ToString();
                    _dosageFormCode = dsMed.Tables[0].Rows[0]["DosageFormCode"].ToString();

                    PrefSigObjectDataSource.SelectParameters.Add("gpi", gpi);
                    PrefSigObjectDataSource.SelectParameters.Add("dosageFormCode", _dosageFormCode);
                    PrefSigObjectDataSource.SelectParameters.Add("routeOfAdminCode", routeOfAdmincode);
                    PrefSigObjectDataSource.SelectParameters.Add("providerID", Guid.Empty.ToString());
                    LstPreferedSig.DataSourceID = "PrefSigObjectDataSource";
                    PrefSigObjectDataSource.Select();


                    string med = string.Empty;

                    if (dsMed.Tables[0].Rows[0]["MEDICATIONNAME"] != null)
                    {
                        med = dsMed.Tables[0].Rows[0]["MEDICATIONNAME"].ToString();
                    }
                    if (dsMed.Tables[0].Rows[0]["STRENGTH"] != null)
                    {
                        med = med + ' ' + dsMed.Tables[0].Rows[0]["STRENGTH"].ToString();
                    }
                    if (dsMed.Tables[0].Rows[0]["STRENGTHUOM"] != null)
                    {
                        med = med + ' ' + dsMed.Tables[0].Rows[0]["STRENGTHUOM"].ToString();
                    }

                    if (dsMed.Tables[0].Rows[0]["DosageForm"] != null)
                    {
                        med = med + ' ' + dsMed.Tables[0].Rows[0]["DosageForm"].ToString();
                    }
                    if (dsMed.Tables[0].Rows[0]["RouteofAdmin"] != null)
                    {
                        med = med + ' ' + dsMed.Tables[0].Rows[0]["RouteofAdmin"].ToString();
                    }

                    if (med != string.Empty)
                    {
                        lblMedInfo.Text = "Choose or write a SIG for " + med + ":";
                    }
                    else
                    {
                        lblMedInfo.Text = "Choose or write a SIG:";
                    }

                    setLibraryInfo(ndc);
                }

                DataSet dsPackage = Allscripts.Impact.Medication.GetPackagesForMedication(rx.DDI, rx.DosageFormCode, base.DBID);
                DataTable dtPackage = dsPackage.Tables["Package"];
                if (dtPackage.Rows.Count > 1)
                {
                    DataView dataView = dtPackage.DefaultView;
                    dataView.Sort = "PackageSize";

                    ddlCustomPack.DataSource = dataView;
                    ddlCustomPack.DataBind();
                    PageState["Package"] = dtPackage;

                    if (_packsize != string.Empty && ddlCustomPack.Items.FindByValue(_packsize) != null)
                    {
                        ddlCustomPack.Items.FindByValue(_packsize).Selected = true;
                    }
                    else if (_packageDescription != string.Empty && ddlCustomPack.Items.FindByText(_packageDescription) != null)
                    {
                        ddlCustomPack.Items.FindByText(_packageDescription).Selected = true;
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
                ddlUnit.Visible = true;
                iFCLink.Style["display"] = "none";
            }

            Parameter pddi = SigAllObjDataSource.SelectParameters["DDI"];

            if (pddi != null)
            {
                SigAllObjDataSource.SelectParameters.Remove(pddi);
            }
            _ddi = rx.DDI;
            SigAllObjDataSource.SelectParameters.Add("DDI", rx.DDI);

            pddi = LatinSigObjDataSource.SelectParameters["ddi"];

            if (pddi != null)
            {
                LatinSigObjDataSource.SelectParameters.Remove(pddi);
            }

            LatinSigObjDataSource.SelectParameters.Add("ddi", rx.DDI);

            toggleSIGPanels();
        }

        private void setLibraryInfo(string ndc)
        {
            switch (SessionLicense.GetFeatureStatus(Constants.DeluxeFeatureType.iFC))
            {
                case Constants.DeluxeFeatureStatus.On:

                    if (string.IsNullOrEmpty(ndc))
                    {
                        iFCLink.Style["display"] = "none";
                    }
                    else
                    {
                        if (IsLexicompEnabled)
                        {
                            string lexicompURL = string.Format(ConfigKeys.LexicompAdminDosageURL, ndc);
                            iFCLink.Attributes["onclick"] = "connectToiFC('" + lexicompURL + "');";
                        }
                        else
                        {
                            string factsComparisonsURL = string.Format(ConfigKeys.FactsComparisonsAdminDosageURL, ndc);
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

        }

        private bool validateSig()
        {
            double numQuantity = 0;
            int numDaysSupply = 0;
            int numrefills = 0;

            if ((rdbFreeTextSig.Checked && txtFreeTextSig.Text.Trim().Equals(string.Empty)) ||
                    (rdbAllSig.Checked && LstSig.SelectedIndex < 0) ||
                    (rdgPrefer.Checked && LstPreferedSig.SelectedIndex < 0))
            {
                toggleSIGPanels();
                lblSigErrorMsg.Visible = true;
                lblSigErrorMsg.Text = "Please choose a SIG or write free text SIG.";
                return false;
            }
            if ((txtQuantity.Text.Trim().Equals(String.Empty)) ||
                (!double.TryParse(txtQuantity.Text.Trim(), out numQuantity)) ||
                   (numQuantity.CompareTo(0.0001) < 0) ||
                   (numQuantity.CompareTo(999.9999) > 0)
                )
            {
                return false;
            }

            if ((txtRefill.Text.Trim().Equals(String.Empty)) ||
                (!int.TryParse(txtRefill.Text.Trim(), out numDaysSupply)) ||
                (numDaysSupply.CompareTo(0) < 0) ||
                (numDaysSupply.CompareTo(99) > 0)
                )
            {
                return false;
            }

            if ((txtDaysSupply.Text.Trim().Equals(String.Empty)) ||
                (!int.TryParse(txtDaysSupply.Text.Trim(), out numrefills)) ||
                (numrefills.CompareTo(1) < 0) ||
                (numrefills.CompareTo(365) > 0)
                )
            {
                return false;
            }

            return true;
        }

        #endregion
    }
}