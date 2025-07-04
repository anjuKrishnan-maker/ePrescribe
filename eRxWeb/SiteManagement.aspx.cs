using System;
using System.Collections;
using System.Data;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Allscripts.ePrescribe.Common;
using Allscripts.Impact;
using Allscripts.Impact.Utilities.Win32;
using eRxWeb.AppCode;
using eRxWeb.ePrescribeSvc;
using Telerik.Web.UI;
using Preference = Allscripts.Impact.Preference;
using PreferenceCategory = Allscripts.Impact.PreferenceCategory;
using RxUser = Allscripts.Impact.RxUser;
using System.Configuration;
using System.Web;
using Allscripts.ePrescribe.ExtensionMethods;
using Provider = Allscripts.Impact.Provider;


namespace eRxWeb
{
    public partial class SiteManagement : BasePage
    {

        #region Handled Events

        protected void Page_Load(object sender, EventArgs e)
        {
            Helper.SetHelpTextForPane(paneHelp, this.AppRelativeVirtualPath);

            //if we're updating universal settings, no need to preload
            if (Request.Form["__EVENTTARGET"] != null && Request.Form["__EVENTTARGET"] == btnUniversal.ClientID.Replace("_", ":"))
            {
                if (gvSite.DataSourceIsAssigned)
                {
                    gvSite.DataSource = null;
                }

                return;
            }

            if (!Page.IsPostBack)
            {
                if (Request.QueryString["msg"] != null)
                {
                    ucMessage.MessageText = HttpUtility.UrlDecode(Request.QueryString["msg"]);
                    ucMessage.Icon = Controls_Message.MessageType.SUCCESS;
                    ucMessage.Visible = true;
                }

                gvSite.Columns.FindByUniqueName("colSiteName").Visible = true;
                gvSite.Columns.FindByUniqueName("colSiteInfo").Visible = false;
                gvSite.Columns.FindByUniqueName("colEditPharmFavs").Visible = base.SessionLicense.EnterpriseClient.ShowPharmacy;

                var licensePreferences = PageState.Cast("LicensePreferences", new LicensePreferences());
                chkShowCheckedInPatients.Checked = licensePreferences.ShowCheckedInPatients;

                // Set visibility of show patient education checkbox based on enterprise setting.
                chkShowRxInfo.Visible = SessionLicense.EnterpriseClient.ShowRxInfo;
                chkShowRxInfo.Checked = licensePreferences.ShowRxInfo;

                //SHOW RePA                
                chkShowRePA.Visible = IsShowREpa();
                chkShowRePA.Checked = licensePreferences.ShowRetrospectiveEpa;

                //PrebuiltPrescriptions
                if (SessionLicense.EnterpriseClient.EnablePrebuiltPrescriptions &&
                          (SessionLicense.LicenseDeluxeStatus == Constants.DeluxeFeatureStatus.On ||
                            SessionLicense.LicenseDeluxeStatus == Constants.DeluxeFeatureStatus.AlwaysOn ||
                            SessionLicense.LicenseDeluxeStatus == Constants.DeluxeFeatureStatus.Cancelled))
                {
                    chkShowPrebuiltRx.Visible = true;
                    chkShowPrebuiltRx.Checked = licensePreferences.ShowPrebuildPrescriptions;
                }
                else
                {
                    chkShowPrebuiltRx.Visible = false;
                }

                //Show PDMP
                if (SessionLicense.EnterpriseClient.ShowPDMP)
                {
                    rowPdmp.Visible = true;
                    lblPDMP.Text = licensePreferences.ShowPDMP ? "Available" : "Not Available";
                }
                else
                {
                    rowPdmp.Visible = false;
                    lblPDMP.Text = "";
                }

                //EnableLexicomp
                if (SessionLicense.EnterpriseClient.EnableLexicomp &&
                          (SessionLicense.LicenseDeluxeStatus == Constants.DeluxeFeatureStatus.On ||
                            SessionLicense.LicenseDeluxeStatus == Constants.DeluxeFeatureStatus.AlwaysOn ||
                            SessionLicense.LicenseDeluxeStatus == Constants.DeluxeFeatureStatus.Cancelled))
                {
                    MedicationReferrenceSearchVisibility(true);
                    if (PageState.GetBooleanOrFalse("ShowLexicompDefault"))
                    {
                        if (SessionLicense.EnterpriseClient.EnableLexicompDefault)
                            rdBtnLexicomp.Checked = true;
                        else
                            rdBtnWK.Checked = true;
                    }
                    else
                    {
                        if (PageState.GetBooleanOrFalse(Constants.LicensePreferences.SHOW_LEXICOMP))
                        {
                            rdBtnLexicomp.Checked = true;
                        }
                        else
                        {
                            rdBtnWK.Checked = true;
                        }
                    }
                }
                else
                {
                    MedicationReferrenceSearchVisibility(false);
                    rdBtnWK.Checked = true;
                }

                if (Session["LicenseName"] != null)
                {
                    txtLicenseName.Text = Session["LicenseName"].ToString();
                }

                if (base.IsBackdoorUser)
                {
                    txtLicenseName.Enabled = false;
                }

                base.AuditLogLicenseInsert(ePrescribeSvc.AuditAction.LICENSE_SITES_DEMO_VIEW_ALL, 0);
            }
        }

        public bool IsShowREpa()
        {
            return (((base.SessionLicense.EnterpriseClient.ShowEPA && PageState.GetBooleanOrFalse(Constants.LicensePreferences.SHOW_EPA))) && SessionLicense.EnterpriseClient.ShowRePA);
        }

        private void MedicationReferrenceSearchVisibility(bool value)
        {
            rdBtnLexicomp.Visible = value;
            rdBtnWK.Visible = value;
            lblMedicationReferrenceSearch.Visible = value;
        }

        protected void Page_LoadComplete(object sender, EventArgs e)
        {
            ((PhysicianMasterPageBlank)Master).hideTabs();
        }

        protected void gvSite_ItemDataBound(object sender, GridItemEventArgs e)
        {
            string selectedState = string.Empty;
            if (e.Item is GridDataItem)
            {
                GridEditableItem gei = (GridEditableItem)e.Item;
                ////make sure the state is correct
                selectedState = ((DataRowView)gei.DataItem).Row["State"].ToString();
                gei["colState"].Text = selectedState;

                int siteID = Convert.ToInt32(gvSite.MasterTableView.DataKeyValues[e.Item.ItemIndex]["SiteID"]);
                if (siteID == 1)
                {
                    gei["colDefaultSite"].Text = "*";
                }
                //show status message
                string activeSite = ((DataRowView)gei.DataItem).Row["Active"].ToString();
                if (activeSite == "Y")
                {
                    gei["colSiteStatus"].Text = "Active";
                }
                else
                {
                    gei["colSiteStatus"].Text = "Inactive";
                    gei["colEditPharmFavs"].Enabled = false;
                }
            }

            if (e.Item is Telerik.Web.UI.GridCommandItem)
            {
                if (PageState.GetStringOrEmpty("SSOMode") == Constants.SSOMode.UTILITYMODE && !SessionLicense.EnterpriseClient.AllowAddSiteForSSOUtilityMode)
                {
                    LinkButton lbAddSite = (LinkButton)e.Item.FindControl("btnAddNew");
                    if (lbAddSite != null)
                    {
                        lbAddSite.Visible = false;
                    }
                }
            }
        }

        protected void gvSite_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            gvSite.DataSource = odsLicense;
        }

        protected void gvSite_ItemCommand(object source, GridCommandEventArgs e)
        {
            //just clear the message
            ucMessage.Visible = false;

            if (e.CommandName == "EditPharmFavs")
            {
                GridDataItem item = (GridDataItem)e.Item;
                Response.Redirect(Constants.PageNames.PHARMACY_FAVORITES + "?siteid=" + item.OwnerTableView.DataKeyValues[item.ItemIndex]["SiteID"].ToString());
            }
            else if (e.CommandName == "Edit")
            {
                GridDataItem item = (GridDataItem)e.Item;
                base.AuditLogLicenseInsert(ePrescribeSvc.AuditAction.LICENSE_SITE_DEMO_VIEW, int.Parse(item.OwnerTableView.DataKeyValues[item.ItemIndex]["SiteID"].ToString()));
            }
            else if (e.CommandName == "InitInsert")
            {
                gvSite.Columns[1].Visible = true;
                gvSite.Columns[2].Visible = false;
            }
        }

        protected void gvSite_CancelCommand(object source, GridCommandEventArgs e)
        {
            gvSite.Columns.FindByUniqueName("colSiteInfo").Visible = false;
            gvSite.Columns.FindByUniqueName("colSiteName").Visible = true;
        }

        protected void gvSite_EditCommand(object source, GridCommandEventArgs e)
        {
            gvSite.Columns.FindByUniqueName("colSiteName").Visible = true;
            gvSite.Columns.FindByUniqueName("colSiteInfo").Visible = true;
            gvSite.Columns[2].Visible = false;
        }

        protected void gvSite_UpdateCommand(object source, GridCommandEventArgs e)
        {
            GridEditFormItem gridEditFormItem = (GridEditFormItem)(e.Item);
            int siteID = Convert.ToInt32(gvSite.MasterTableView.DataKeyValues[e.Item.ItemIndex]["SiteID"]);
            string siteName = ((TextBox)(gridEditFormItem["colSiteInfo"].FindControl("txtPracticeName"))).Text;
            string addr1 = ((TextBox)gridEditFormItem["colAddress1"].Controls[0]).Text;
            string addr2 = ((TextBox)gridEditFormItem["colAddress2"].Controls[0]).Text;
            string city = ((TextBox)gridEditFormItem["colCity"].Controls[0]).Text;
            string state = ((Telerik.Web.UI.RadComboBox)gridEditFormItem["colState"].Controls[0]).SelectedItem.Value;
            string zipCode = ((TextBox)gridEditFormItem["colZIP"].Controls[0]).Text;
            string phoneNumber = ((TextBox)gridEditFormItem.FindControl("txtPhone")).Text;
            string faxNumber = ((TextBox)gridEditFormItem.FindControl("txtFax")).Text;
            string strTimeZone = ((Telerik.Web.UI.RadComboBox)gridEditFormItem["colTimeZone"].Controls[0]).SelectedItem.Value;
            bool allowAdmin = ((CheckBox)gridEditFormItem.FindControl("chkAllowAdmin")).Checked;
            bool genericEquivlentSearch = ((CheckBox)gridEditFormItem.FindControl("chkGenericEquivalentSearch")).Checked;
            bool inactiveSite = ((CheckBox)gridEditFormItem.FindControl("chkInactiveSite")).Checked;
            bool needEnrollment = false;

            //only see if we're editing the default site
            if (siteID == 1)
            {
                if (Convert.ToBoolean(Session["ISADMIN"]) && inactiveSite)
                {
                    ucMessage.MessageText = "You are trying to make default site inactive. You cannot make this site Inactive.";
                    ucMessage.Icon = Controls_Message.MessageType.ERROR;
                    ucMessage.Visible = true;
                    return;
                }
                needEnrollment = isAnyEnrollmentChanges(SessionLicenseID, siteID, siteName, addr1, addr2, city, state, zipCode, phoneNumber, faxNumber, strTimeZone);
            }

            DataSet providerDS = Provider.GetProvidersByFaxSiteID(SessionLicenseID, siteID, DBID);

            if (siteID != 1 && Convert.ToBoolean(Session["ISADMIN"]) && inactiveSite)
            {
                if (SessionSiteID == siteID)
                {
                    ucMessage.MessageText = "You are trying to make your current site inactive. Please change your current site and try again.";
                    ucMessage.Icon = Controls_Message.MessageType.ERROR;
                    ucMessage.Visible = true;
                    return;
                }
                else if (providerDS != null && providerDS.Tables.Count > 0 && providerDS.Tables[0].Rows.Count == 0)
                {
                    ApplicationLicense.EditSite(SessionLicenseID, siteID, siteName, addr1, addr2, city, state, zipCode, phoneNumber, faxNumber, false, false, 1, strTimeZone, allowAdmin, string.Empty, DBID);
                }
                else
                {
                    ucMessage.MessageText = "You are trying to make default fax site inactive. You cannot make this site Inactive as you have active users associated with this site.";
                    ucMessage.Icon = Controls_Message.MessageType.ERROR;
                    ucMessage.Visible = true;
                    return;
                }
            }
            else
            {

                needEnrollment = isAnyEnrollmentChanges(SessionLicenseID, siteID, siteName, addr1, addr2, city, state, zipCode, phoneNumber, faxNumber, strTimeZone);
                ApplicationLicense.EditSite(SessionLicenseID, siteID, siteName, addr1, addr2, city, state, zipCode, phoneNumber, faxNumber, true, false, 1, strTimeZone, allowAdmin, string.Empty, DBID);
                UserManagementPortal.UpdateLicenseToUMPByLicenseID(SessionLicenseID, DBID);
            }

            //if not a test site enroll with SureScripts
            if (Session["STANDING"].ToString() == "1")
            {
                //DataSet providerDS = Provider.GetProvidersByFaxSiteID(SessionLicenseID, siteID, DBID);

                if (providerDS != null && providerDS.Tables.Count > 0)
                {
                    SPI enrollmentService = new SPI();
                    if (needEnrollment)
                    {
                        foreach (DataRow dr in providerDS.Tables[0].Rows)
                        {
                            //Remove all Inactive users BEFORE Passing to GetSPI
                            ePrescribeSvc.GetUserResponse getUserRes = EPSBroker.GetRxUser(
                                ePrescribeSvc.ValueType.UserGUID,
                                dr["ProviderID"].ToString(),
                                SessionLicenseID,
                                Session["UserID"].ToString(),
                                SessionLicenseID,
                                DBID
                                );
                            if (getUserRes.RxUser.Active)
                            {
                                enrollmentService.GetSpi(getUserRes.RxUser, Session["UserID"].ToString().ToGuidOr0x0(), Convert.ToBoolean(Session["EnableRenewals"].ToString()), siteID);
                            }
                        }
                    }
                }
            }

            bool bSameSite = base.SessionSiteID == siteID;
            string printingPreference = ((RadioButtonList)gridEditFormItem.FindControl("rdoPrintingOption")).SelectedValue;
            if (!string.IsNullOrEmpty(printingPreference))
            {
                if (bSameSite)
                {
                    Session["PRINTINGPREFERENCE"] = printingPreference;
                }
                SitePreference.SetPreference(SessionLicenseID, siteID, "PRINTINGPREFERENCE", printingPreference, base.DBID);
            }

            if (bSameSite)
            {
                Session["GenericEquivalentSearch"] = genericEquivlentSearch ? "Y" : "N";
            }
            SitePreference.SetPreference(SessionLicenseID, siteID, "GenericEquivalentSearch", genericEquivlentSearch ? "Y" : "N", base.DBID);

            string paperPreference = ((RadioButtonList)gridEditFormItem.FindControl("rdoPaperOption")).SelectedValue;
            if (!string.IsNullOrEmpty(paperPreference))
            {
                if (paperPreference == "P")
                {
                    SitePreference.SetPreference(SessionLicenseID, siteID, "NONCSPLAINPAPER", "True", base.DBID);
                }
                else
                {
                    SitePreference.SetPreference(SessionLicenseID, siteID, "NONCSPLAINPAPER", "False", base.DBID);
                }
            }

            CheckBox chkFinancialOffers = (CheckBox)gridEditFormItem.FindControl("chkFinancialOffers");
            if (chkFinancialOffers != null)
            {
                SitePreference.SetPreference(SessionLicenseID, siteID, "APPLYFINANCIALOFFERS", chkFinancialOffers.Checked.ToString(), base.DBID);
            }

            if (SessionLicense.EnterpriseClient.ShowSponsoredLinks)
            {
                CheckBox chkInfoScripts = (CheckBox)gridEditFormItem.FindControl("chkInfoScripts");
                if (chkInfoScripts != null)
                {
                    SitePreference.SetPreference(SessionLicenseID, siteID, "DISPLAYINFOSCRIPTS", chkInfoScripts.Checked.ToString(), base.DBID);
                }
            }

            CheckBox chkAllowPatientReceipt = (CheckBox)gridEditFormItem.FindControl("chkAllowPatientReceipt");
            if (chkAllowPatientReceipt != null)
            {
                SitePreference.SetPreference(SessionLicenseID, siteID, "ALLOWPATIENTRECEIPT", chkAllowPatientReceipt.Checked.ToString(), base.DBID);
            }
            CheckBox chkAllowMDD = (CheckBox)gridEditFormItem.FindControl("chkAllowMDD");
            if (chkAllowMDD != null)
            {
                bool allowMDD = chkAllowMDD.Checked;
                SitePreference.SetPreference(SessionLicenseID, siteID, "ALLOWMDD", allowMDD.ToString(), base.DBID);
                RadioButtonList rblMDDOption = ((RadioButtonList)gridEditFormItem.FindControl("rboMDDOption"));
                if (allowMDD)
                {
                    if (rblMDDOption != null)
                    {
                        if (rblMDDOption.SelectedValue == "CS")
                        {
                            SitePreference.SetPreference(SessionLicenseID, siteID, "CSMEDSONLY", "True", base.DBID);
                        }
                        else
                        {
                            SitePreference.SetPreference(SessionLicenseID, siteID, "CSMEDSONLY", "False", base.DBID);
                        }
                    }
                    else
                    {
                        SitePreference.SetPreference(SessionLicenseID, siteID, "CSMEDSONLY", "True", base.DBID);
                    }
                }
                else
                {
                    if (rblMDDOption != null)
                    {
                        rblMDDOption.Visible = false;
                    }
                }
            }

            ucMessage.MessageText = siteName + " successfully saved.";
            ucMessage.Icon = Controls_Message.MessageType.SUCCESS;
            ucMessage.Visible = true;
            gvSite.Rebind();

            base.AuditLogLicenseInsert(ePrescribeSvc.AuditAction.LICENSE_SITE_DEMO_MODIFY, siteID);
        }

        protected void gvSite_InsertCommand(object source, GridCommandEventArgs e)
        {
            GridEditFormInsertItem gridEditInsertFormItem = (GridEditFormInsertItem)(gvSite.MasterTableView.GetInsertItem());
            string siteName = ((TextBox)gridEditInsertFormItem["colSiteInfo"].FindControl("txtPracticeName")).Text;
            string addr1 = ((TextBox)gridEditInsertFormItem["colAddress1"].Controls[0]).Text;
            string addr2 = ((TextBox)gridEditInsertFormItem["colAddress2"].Controls[0]).Text;
            string city = ((TextBox)gridEditInsertFormItem["colCity"].Controls[0]).Text;
            string state = ((Telerik.Web.UI.RadComboBox)gridEditInsertFormItem["colState"].Controls[0]).SelectedValue;
            string zipCode = ((TextBox)gridEditInsertFormItem["colZIP"].Controls[0]).Text;
            string phoneNumber = ((TextBox)gridEditInsertFormItem.FindControl("txtPhone")).Text;
            string faxNumber = ((TextBox)gridEditInsertFormItem.FindControl("txtFax")).Text;
            string strTimeZone = ((Telerik.Web.UI.RadComboBox)gridEditInsertFormItem["colTimeZone"].Controls[0]).SelectedValue;
            bool allowAdmin = ((CheckBox)gridEditInsertFormItem.FindControl("chkAllowAdmin")).Checked;
            bool genericEquivalentSearch = ((CheckBox)gridEditInsertFormItem.FindControl("chkGenericEquivalentSearch")).Checked;
            bool inactiveSite = ((CheckBox)gridEditInsertFormItem.FindControl("chkInactiveSite")).Checked;
            int newSiteID = 0;
            if (inactiveSite)
            {
                newSiteID = ApplicationLicense.EditSite(SessionLicenseID, -1, siteName, addr1, addr2, city, state, zipCode, phoneNumber, faxNumber, false, false, 1, strTimeZone, allowAdmin, string.Empty, DBID);
            }
            else
            {
                newSiteID = ApplicationLicense.EditSite(SessionLicenseID, -1, siteName, addr1, addr2, city, state, zipCode, phoneNumber, faxNumber, true, false, 1, strTimeZone, allowAdmin, string.Empty, DBID);
            }

            CheckBox chkFinancialOffers = (CheckBox)gridEditInsertFormItem.FindControl("chkFinancialOffers");
            if (chkFinancialOffers != null)
            {
                SitePreference.SetPreference(SessionLicenseID, newSiteID, "APPLYFINANCIALOFFERS", chkFinancialOffers.Checked.ToString(), base.DBID);
            }
            else
            {
                SitePreference.SetPreference(SessionLicenseID, newSiteID, "APPLYFINANCIALOFFERS", false.ToString(), DBID);
            }

            CheckBox chkInfoScripts = (CheckBox)gridEditInsertFormItem.FindControl("chkInfoScripts");
            if (chkInfoScripts != null)
            {
                SitePreference.SetPreference(SessionLicenseID, newSiteID, "DISPLAYINFOSCRIPTS", chkInfoScripts.Checked.ToString(), base.DBID);
            }
            else
            {
                SitePreference.SetPreference(SessionLicenseID, newSiteID, "DISPLAYINFOSCRIPTS", false.ToString(), base.DBID);
            }

            SitePreference.SetPreference(SessionLicenseID, newSiteID, "GenericEquivalentSearch", genericEquivalentSearch ? "Y" : "N", base.DBID);
            RadioButtonList rblPaperPreference = ((RadioButtonList)gridEditInsertFormItem.FindControl("rdoPaperOption"));
            if (rblPaperPreference != null)
            {
                if (rblPaperPreference.SelectedValue == "P")
                {
                    SitePreference.SetPreference(SessionLicenseID, newSiteID, "NONCSPLAINPAPER", "True", base.DBID);
                }
                else
                {
                    SitePreference.SetPreference(SessionLicenseID, newSiteID, "NONCSPLAINPAPER", "False", base.DBID);
                }
            }
            else
            {
                SitePreference.SetPreference(SessionLicenseID, newSiteID, "NONCSPLAINPAPER", "False", base.DBID);
            }

            CheckBox chkAllowPatientReceipt = (CheckBox)gridEditInsertFormItem.FindControl("chkAllowPatientReceipt");
            if (chkAllowPatientReceipt != null)
            {
                SitePreference.SetPreference(SessionLicenseID, newSiteID, "ALLOWPATIENTRECEIPT", chkAllowPatientReceipt.Checked.ToString(), base.DBID);
            }
            else
            {
                SitePreference.SetPreference(SessionLicenseID, newSiteID, "ALLOWPATIENTRECEIPT", false.ToString(), base.DBID);
            }

            CheckBox chkAllowMDD = (CheckBox)gridEditInsertFormItem.FindControl("chkAllowMDD");
            if (chkAllowMDD != null)
            {
                bool allowMDD = chkAllowMDD.Checked;
                SitePreference.SetPreference(SessionLicenseID, newSiteID, "ALLOWMDD", allowMDD.ToString(), base.DBID);
                if (allowMDD)
                {
                    RadioButtonList rblMDDOption = ((RadioButtonList)gridEditInsertFormItem.FindControl("rboMDDOption"));
                    if (rblMDDOption != null)
                    {
                        if (rblMDDOption.SelectedValue == "CS")
                        {
                            SitePreference.SetPreference(SessionLicenseID, newSiteID, "CSMEDSONLY", "True", base.DBID);
                        }
                        else
                        {
                            SitePreference.SetPreference(SessionLicenseID, newSiteID, "CSMEDSONLY", "False", base.DBID);
                        }
                    }
                    else
                    {
                        SitePreference.SetPreference(SessionLicenseID, newSiteID, "CSMEDSONLY", "True", base.DBID);
                    }
                }
            }

            RadioButtonList rblPrintingPreference = ((RadioButtonList)gridEditInsertFormItem.FindControl("rdoPrintingOption"));
            if (rblPrintingPreference != null)
            {
                SitePreference.SetPreference(SessionLicenseID, newSiteID, "PRINTINGPREFERENCE", rblPrintingPreference.SelectedValue, base.DBID);
            }
            else
            {
                SitePreference.SetPreference(SessionLicenseID, newSiteID, "PRINTINGPREFERENCE", "1", base.DBID);
            }

            ucMessage.MessageText = siteName + " successfully added";
            ucMessage.Icon = Controls_Message.MessageType.SUCCESS;
            ucMessage.Visible = true;
        }

        protected void gvSite_ItemCreated(object sender, GridItemEventArgs e)
        {
            //customize the edit form
            if (e.Item.IsInEditMode && e.Item is GridEditFormItem)
            {
                GridEditableItem item = e.Item as GridEditableItem;

                GridTextBoxColumnEditor siteEditor = (GridTextBoxColumnEditor)item.EditManager.GetColumnEditor("colSiteName");
                TableCell siteCell = (TableCell)siteEditor.TextBoxControl.Parent;
                siteEditor.TextBoxControl.Width = 235;
                siteEditor.TextBoxControl.ID = siteCell.Controls[0].UniqueID.Substring(siteCell.Controls[0].UniqueID.LastIndexOf("$") + 1);

                GridTemplateColumnEditor siteInfoEditor = (GridTemplateColumnEditor)item.EditManager.GetColumnEditor("colSiteInfo");
                TextBox siteBox = (TextBox)siteInfoEditor.ContainerControl.FindControl("txtPracticeName");
                siteBox.Width = 190;

                if (!(e.Item is GridEditFormInsertItem) && PageState.GetStringOrEmpty("SSOMode") == Constants.SSOMode.UTILITYMODE && !SessionLicense.EnterpriseClient.AllowEditSiteForSSOUtilityMode)
                {
                    siteBox.Enabled = false;
                }
                else
                {
                    TableCell siteBoxCell = (TableCell)siteBox.Parent;
                    RequiredFieldValidator siteValidator = new RequiredFieldValidator();
                    siteValidator.ControlToValidate = siteBox.ID;
                    siteValidator.ErrorMessage = "Please enter a valid Site Name.";
                    siteValidator.Text = "*";
                    if (e.Item is GridEditFormInsertItem)
                    {
                        siteValidator.ValidationGroup = "addingValidationGroup";
                        ((e.Item as GridEditFormItem).FindControl("PerformInsertButton") as Button).ValidationGroup = "addingValidationGroup";
                    }
                    else
                    {
                        siteValidator.ValidationGroup = "editingValidationGroup";
                        ((e.Item as GridEditFormItem).FindControl("UpdateButton") as Button).ValidationGroup = "editingValidationGroup";
                    }
                    siteBoxCell.Controls.Add(siteValidator);
                }

                CheckBox chkInactiveSite = (CheckBox)siteInfoEditor.ContainerControl.FindControl("chkInactiveSite");
                if (e.Item is GridEditFormInsertItem)
                {
                    chkInactiveSite.Visible = false;
                    chkInactiveSite.Checked = false;
                }
                else
                {
                    chkInactiveSite.Visible = true;
                }

                if (e.Item.DataItem is DataRowView)
                {
                    if (((DataRowView)e.Item.DataItem) != null)
                    {
                        DataRowView drv = (DataRowView)e.Item.DataItem;
                        if (drv.Row["Active"] != DBNull.Value)
                        {
                            if (((DataRowView)e.Item.DataItem).Row["Active"].ToString().ToUpper() != "Y")
                            {
                                chkInactiveSite.Checked = true;
                            }
                            else
                            {
                                chkInactiveSite.Checked = false;
                            }
                        }
                    }
                }

                if (!(e.Item is GridEditFormInsertItem) && PageState.GetStringOrEmpty("SSOMode") == Constants.SSOMode.UTILITYMODE && !SessionLicense.EnterpriseClient.AllowEditSiteForSSOUtilityMode)
                {
                    chkInactiveSite.Enabled = false;
                }

                GridTextBoxColumnEditor addr1Editor = (GridTextBoxColumnEditor)item.EditManager.GetColumnEditor("colAddress1");
                TableCell addrCell = (TableCell)addr1Editor.TextBoxControl.Parent;
                addr1Editor.TextBoxControl.Width = 250;
                addr1Editor.TextBoxControl.ID = addrCell.Controls[0].UniqueID.Substring(addrCell.Controls[0].UniqueID.LastIndexOf("$") + 1);

                if (!(e.Item is GridEditFormInsertItem) && PageState.GetStringOrEmpty("SSOMode") == Constants.SSOMode.UTILITYMODE && !SessionLicense.EnterpriseClient.AllowEditSiteForSSOUtilityMode)
                {
                    addr1Editor.TextBoxControl.Enabled = false;
                }
                else
                {
                    RequiredFieldValidator addrValidator = new RequiredFieldValidator();
                    addrValidator.ControlToValidate = addr1Editor.TextBoxControl.ID;
                    addrValidator.ErrorMessage = "Please enter a valid Address.";
                    addrValidator.Text = "*";
                    if (e.Item is GridEditFormInsertItem)
                    {
                        addrValidator.ValidationGroup = "addingValidationGroup";
                    }
                    else
                    {
                        addrValidator.ValidationGroup = "editingValidationGroup";

                    }
                    addrCell.Controls.Add(addrValidator);
                }

                GridTextBoxColumnEditor addr2Editor = (GridTextBoxColumnEditor)item.EditManager.GetColumnEditor("colAddress2");
                addr2Editor.TextBoxControl.Width = 250;
                ((GridTableRow)item["colAddress2"].Parent).Cells[0].Text = "&nbsp;";

                if (!(e.Item is GridEditFormInsertItem) && PageState.GetStringOrEmpty("SSOMode") == Constants.SSOMode.UTILITYMODE && !SessionLicense.EnterpriseClient.AllowEditSiteForSSOUtilityMode)
                {
                    addr2Editor.TextBoxControl.Enabled = false;
                }

                GridTextBoxColumnEditor cityEditor = (GridTextBoxColumnEditor)item.EditManager.GetColumnEditor("colCity");
                TableCell cityCell = (TableCell)cityEditor.TextBoxControl.Parent;
                cityEditor.TextBoxControl.ID = cityCell.Controls[0].UniqueID.Substring(cityCell.Controls[0].UniqueID.LastIndexOf("$") + 1);

                if (!(e.Item is GridEditFormInsertItem) && PageState.GetStringOrEmpty("SSOMode") == Constants.SSOMode.UTILITYMODE && !SessionLicense.EnterpriseClient.AllowEditSiteForSSOUtilityMode)
                {
                    cityEditor.TextBoxControl.Enabled = false;
                }
                else
                {
                    RequiredFieldValidator cityValidator = new RequiredFieldValidator();
                    cityValidator.ControlToValidate = cityEditor.TextBoxControl.ID;
                    cityValidator.ErrorMessage = "Please enter a valid City.";
                    cityValidator.Text = "*";
                    if (e.Item is GridEditFormInsertItem)
                    {
                        cityValidator.ValidationGroup = "addingValidationGroup";

                    }
                    else
                    {
                        cityValidator.ValidationGroup = "editingValidationGroup";

                    }
                    cityCell.Controls.Add(cityValidator);
                }

                GridDropDownListColumnEditor stateEditor = (GridDropDownListColumnEditor)item.EditManager.GetColumnEditor("colState");
                Telerik.Web.UI.RadComboBox ddlState = stateEditor.ComboBoxControl;

                string selectedState = Session["PRACTICESTATE"].ToString();
                if (e.Item.DataItem is DataRowView)
                {
                    if (((DataRowView)e.Item.DataItem) != null)
                    {
                        selectedState = ((DataRowView)e.Item.DataItem).Row["State"].ToString();
                    }
                }
                populateStates(ddlState, selectedState);

                if (!(e.Item is GridEditFormInsertItem) && PageState.GetStringOrEmpty("SSOMode") == Constants.SSOMode.UTILITYMODE && !SessionLicense.EnterpriseClient.AllowEditSiteForSSOUtilityMode)
                {
                    ddlState.Enabled = false;
                }
                else
                {
                    // Handle the state selection change on server
                    RadComboBox list = (e.Item as GridEditableItem)["colState"].Controls[0] as RadComboBox;
                    list.CausesValidation = false;
                    list.AutoPostBack = true;
                    list.SelectedIndexChanged += new RadComboBoxSelectedIndexChangedEventHandler(ComboBoxControl_SelectedIndexChanged);
                }

                GridTextBoxColumnEditor zipEditor = (GridTextBoxColumnEditor)item.EditManager.GetColumnEditor("colZIP");
                TableCell zipCell = (TableCell)zipEditor.TextBoxControl.Parent;
                zipEditor.TextBoxControl.Width = 70;
                zipEditor.TextBoxControl.ID = zipCell.Controls[0].UniqueID.Substring(zipCell.Controls[0].UniqueID.LastIndexOf("$") + 1);

                if (!(e.Item is GridEditFormInsertItem) && PageState.GetStringOrEmpty("SSOMode") == Constants.SSOMode.UTILITYMODE && !SessionLicense.EnterpriseClient.AllowEditSiteForSSOUtilityMode)
                {
                    zipEditor.TextBoxControl.Enabled = false;
                }
                else
                {
                    zipEditor.TextBoxControl.Attributes.Add("onkeypress", "return onlyAllowNumericZipPress(event);");

                    RequiredFieldValidator zipValidator = new RequiredFieldValidator();
                    zipValidator.ControlToValidate = zipEditor.TextBoxControl.ID;
                    zipValidator.ErrorMessage = "Please enter a valid ZIP code.";
                    zipValidator.Text = "*";
                    if (e.Item is GridEditFormInsertItem)
                    {
                        zipValidator.ValidationGroup = "addingValidationGroup";

                    }
                    else
                    {
                        zipValidator.ValidationGroup = "editingValidationGroup";

                    }
                    zipCell.Controls.Add(zipValidator);

                    RegularExpressionValidator revZip = new RegularExpressionValidator();
                    revZip.ControlToValidate = zipEditor.TextBoxControl.ID;
                    revZip.ValidationExpression = @"^\d{5,9}$";
                    revZip.ErrorMessage = "Please enter a valid ZIP code";
                    revZip.Text = "*";
                    if (e.Item is GridEditFormInsertItem)
                    {
                        revZip.ValidationGroup = "addingValidationGroup";

                    }
                    else
                    {
                        revZip.ValidationGroup = "editingValidationGroup";

                    }
                    zipCell.Controls.Add(revZip);
                }

                TextBox txtPhone = (TextBox)item.FindControl("txtPhone");
                txtPhone.Width = 100;
                if (e.Item.DataItem is DataRowView)
                {
                    if (((DataRowView)e.Item.DataItem) != null)
                    {
                        if (((DataRowView)e.Item.DataItem).Row["PhoneNumber"].ToString().Trim() != "")
                        {
                            txtPhone.Text = Allscripts.Impact.Utilities.StringHelper.FormatPhone(((DataRowView)e.Item.DataItem).Row["PhoneAreaCode"].ToString(), ((DataRowView)e.Item.DataItem).Row["PhoneNumber"].ToString());
                        }
                    }
                }

                if (!(e.Item is GridEditFormInsertItem) && PageState.GetStringOrEmpty("SSOMode") == Constants.SSOMode.UTILITYMODE && !SessionLicense.EnterpriseClient.AllowEditSiteForSSOUtilityMode)
                {
                    txtPhone.Enabled = false;
                }
                else
                {
                    TableCell phoneCell = (TableCell)txtPhone.Parent;
                    RequiredFieldValidator phoneValidator = new RequiredFieldValidator();
                    phoneValidator.ControlToValidate = txtPhone.ID;
                    phoneValidator.ErrorMessage = "Please enter a valid Phone Number.";
                    phoneValidator.Text = "*";
                    if (e.Item is GridEditFormInsertItem)
                    {
                        phoneValidator.ValidationGroup = "addingValidationGroup";

                    }
                    else
                    {
                        phoneValidator.ValidationGroup = "editingValidationGroup";

                    }
                    phoneCell.Controls.Add(phoneValidator);

                    RegularExpressionValidator revPhone = new RegularExpressionValidator();
                    revPhone.ControlToValidate = txtPhone.ID;
                    revPhone.ValidationExpression = @"^[01]?[- .]?(\([2-9]\d{2}\)|[2-9]\d{2})[- .]?\d{3}[- .]?\d{4}$";
                    revPhone.ErrorMessage = "Please enter a valid 10-digit Phone Number";
                    revPhone.Text = "*";
                    if (e.Item is GridEditFormInsertItem)
                    {
                        revPhone.ValidationGroup = "addingValidationGroup";

                    }
                    else
                    {
                        revPhone.ValidationGroup = "editingValidationGroup";

                    }
                    phoneCell.Controls.Add(revPhone);
                }

                TextBox txtFax = (TextBox)item.FindControl("txtFax");
                txtFax.Width = 100;
                if (e.Item.DataItem is DataRowView)
                {
                    if (((DataRowView)e.Item.DataItem) != null)
                    {
                        if (((DataRowView)e.Item.DataItem).Row["FaxNumber"].ToString().Trim() != "")
                        {
                            txtFax.Text = Allscripts.Impact.Utilities.StringHelper.FormatPhone(((DataRowView)e.Item.DataItem).Row["FaxAreaCode"].ToString(), ((DataRowView)e.Item.DataItem).Row["FaxNumber"].ToString());
                        }
                    }
                }

                if (!(e.Item is GridEditFormInsertItem) && PageState.GetStringOrEmpty("SSOMode") == Constants.SSOMode.UTILITYMODE && !SessionLicense.EnterpriseClient.AllowEditSiteForSSOUtilityMode)
                {
                    txtFax.Enabled = false;
                }
                else
                {
                    TableCell faxCell = (TableCell)txtFax.Parent;
                    RequiredFieldValidator faxValidator = new RequiredFieldValidator();
                    faxValidator.ControlToValidate = txtFax.ID;
                    faxValidator.ErrorMessage = "Please enter a valid Fax Number.";
                    faxValidator.Text = "*";
                    if (e.Item is GridEditFormInsertItem)
                    {
                        faxValidator.ValidationGroup = "addingValidationGroup";
                    }
                    else
                    {
                        faxValidator.ValidationGroup = "editingValidationGroup";
                    }
                    faxCell.Controls.Add(faxValidator);

                    RegularExpressionValidator revFax = new RegularExpressionValidator();
                    revFax.ControlToValidate = txtFax.ID;
                    revFax.ValidationExpression = @"^[01]?[- .]?(\([2-9]\d{2}\)|[2-9]\d{2})[- .]?\d{3}[- .]?\d{4}$";
                    revFax.ErrorMessage = "Please enter a valid 10-digit Fax Number";
                    revFax.Text = "*";
                    if (e.Item is GridEditFormInsertItem)
                    {
                        revFax.ValidationGroup = "addingValidationGroup";
                    }
                    else
                    {
                        revFax.ValidationGroup = "editingValidationGroup";
                    }
                    faxCell.Controls.Add(revFax);
                }

                GridDropDownListColumnEditor ddlEditor = (GridDropDownListColumnEditor)item.EditManager.GetColumnEditor("colTimeZone");
                Telerik.Web.UI.RadComboBox ddl = ddlEditor.ComboBoxControl;
                string selectedTZ = "Central Standard Time";

                if (e.Item.DataItem is DataRowView)
                {
                    if (((DataRowView)e.Item.DataItem) != null)
                    {
                        selectedTZ = ((DataRowView)e.Item.DataItem).Row["TimeZone"].ToString();
                    }
                }

                populateTimeZone(ddl, selectedTZ);

                if (!(e.Item is GridEditFormInsertItem) && PageState.GetStringOrEmpty("SSOMode") == Constants.SSOMode.UTILITYMODE && !SessionLicense.EnterpriseClient.AllowEditSiteForSSOUtilityMode)
                {
                    ddl.Enabled = false;
                }

                ((GridTableRow)item["colAdmin"].Parent).Cells[0].Text = "&nbsp;";

                CheckBox chkAllowAdmin = (CheckBox)item.FindControl("chkAllowAdmin");
                if (e.Item.DataItem is DataRowView)
                {
                    if (((DataRowView)e.Item.DataItem) != null)
                    {
                        DataRowView drv = (DataRowView)e.Item.DataItem;
                        if (drv.Row["AllowAdmin"] != DBNull.Value)
                        {
                            if (Convert.ToBoolean(((DataRowView)e.Item.DataItem).Row["AllowAdmin"]))
                            {
                                chkAllowAdmin.Checked = true;
                            }
                        }
                        else
                        {
                            chkAllowAdmin.Checked = false;
                        }
                    }
                }

                ((GridTableRow)item["colFinancialOffers"].Parent).Cells[0].Text = "&nbsp;";

                CheckBox chkFinancialOffers = (CheckBox)item.FindControl("chkFinancialOffers");
                if (chkFinancialOffers != null)
                {
                    chkFinancialOffers.Visible = base.SessionLicense.EnterpriseClient.ShowECoupon;

                    if (e.Item.ItemIndex >= 0 && !string.IsNullOrEmpty(gvSite.MasterTableView.DataKeyValues[e.Item.ItemIndex]["SiteID"].ToString()))
                    {

                        int siteID = Convert.ToInt32(gvSite.MasterTableView.DataKeyValues[e.Item.ItemIndex]["SiteID"]);

                        string applyOffers = SitePreference.GetPreference(SessionLicenseID, siteID, "APPLYFINANCIALOFFERS", base.DBID);

                        if (applyOffers == true.ToString())
                        {
                            chkFinancialOffers.Checked = true;
                        }
                        else
                        {
                            chkFinancialOffers.Checked = false;
                        }
                    }
                }

                ((GridTableRow)item["colInfoScripts"].Parent).Cells[0].Text = "&nbsp;";

                CheckBox chkInfoScripts = (CheckBox)item.FindControl("chkInfoScripts");
                if (chkInfoScripts != null)
                {
                    chkInfoScripts.Visible = base.SessionLicense.EnterpriseClient.ShowSponsoredLinks;

                    if (e.Item.ItemIndex >= 0 && !string.IsNullOrEmpty(gvSite.MasterTableView.DataKeyValues[e.Item.ItemIndex]["SiteID"].ToString()))
                    {
                        int siteID = Convert.ToInt32(gvSite.MasterTableView.DataKeyValues[e.Item.ItemIndex]["SiteID"]);

                        string displayInfoScripts = SitePreference.GetPreference(SessionLicenseID, siteID, "DISPLAYINFOSCRIPTS", base.DBID);

                        if (displayInfoScripts == true.ToString())
                        {
                            chkInfoScripts.Checked = true;
                        }
                        else
                        {
                            chkInfoScripts.Checked = false;
                        }
                    }
                }

                ((GridTableRow)item["colPatientReceipt"].Parent).Cells[0].Text = "&nbsp;";

                CheckBox chkAllowPatientReceipt = (CheckBox)item.FindControl("chkAllowPatientReceipt");
                if (chkAllowPatientReceipt != null)
                {

                    if (e.Item.ItemIndex >= 0 && !string.IsNullOrEmpty(gvSite.MasterTableView.DataKeyValues[e.Item.ItemIndex]["SiteID"].ToString()))
                    {
                        int siteID = Convert.ToInt32(gvSite.MasterTableView.DataKeyValues[e.Item.ItemIndex]["SiteID"]);

                        string allowPatientReceipts = SitePreference.GetPreference(SessionLicenseID, siteID, "ALLOWPATIENTRECEIPT", base.DBID);

                        if (allowPatientReceipts == true.ToString())
                        {
                            chkAllowPatientReceipt.Checked = true;
                        }
                        else
                        {
                            chkAllowPatientReceipt.Checked = false;
                        }
                    }
                }

                ((GridTableRow)item["colDefaultSite"].Parent).Cells[0].Text = "&nbsp;";

                CheckBox chkGenericEquivlentSearch = (CheckBox)item.FindControl("chkGenericEquivalentSearch");
                if (e.Item.ItemIndex >= 0 && !string.IsNullOrEmpty(gvSite.MasterTableView.DataKeyValues[e.Item.ItemIndex]["SiteID"].ToString()))
                {
                    var licensePreferences = PageState.Cast("LicensePreferences", new LicensePreferences());
                    if (PageState.GetStringOrEmpty("GenericEquivalentSearch") == "Y" || PageState.GetStringOrEmpty("GenericEquivalentSearch") == string.Empty)
                    {
                        chkGenericEquivlentSearch.Checked = licensePreferences.GenericEquivalentSearch;
                    }
                    else
                    {
                        chkGenericEquivlentSearch.Checked = false;
                    }
                }

                ((GridTableRow)item["colAllowMDD"].Parent).Cells[0].Text = "&nbsp;";

                CheckBox chkAllowMDD = (CheckBox)item.FindControl("chkAllowMDD");
                RadioButtonList rdoMDDgOption = (RadioButtonList)item.FindControl("rboMDDOption");
                if (rdoMDDgOption != null)
                {
                    rdoMDDgOption.Visible = false;
                }
                if (chkAllowMDD != null)
                {
                    if (e.Item.ItemIndex >= 0 && !string.IsNullOrEmpty(gvSite.MasterTableView.DataKeyValues[e.Item.ItemIndex]["SiteID"].ToString()))
                    {
                        int siteID = Convert.ToInt32(gvSite.MasterTableView.DataKeyValues[e.Item.ItemIndex]["SiteID"]);

                        string MDDoption = SitePreference.GetPreference(SessionLicenseID, siteID, "ALLOWMDD", base.DBID);
                        string CSmedOnly = SitePreference.GetPreference(SessionLicenseID, siteID, "CSMEDSONLY", base.DBID);


                        if (!String.IsNullOrWhiteSpace(MDDoption))
                        {
                            chkAllowMDD.Checked = Convert.ToBoolean(MDDoption);
                            if (chkAllowMDD.Checked)
                            {
                                if (rdoMDDgOption != null)
                                {
                                    rdoMDDgOption.Visible = true;
                                    if (String.IsNullOrWhiteSpace(CSmedOnly))
                                    {
                                        rdoMDDgOption.SelectedValue = "CS";
                                    }
                                    else
                                    {
                                        if (CSmedOnly == true.ToString())
                                        {
                                            rdoMDDgOption.SelectedValue = "CS";
                                        }
                                        else
                                        {
                                            rdoMDDgOption.SelectedValue = "AL";
                                        }
                                    }
                                }
                            }

                        }
                        else
                        {
                            chkAllowMDD.Checked = false;
                        }
                    }
                }

                RadioButtonList rdoPrintingOption = (RadioButtonList)item.FindControl("rdoPrintingOption");
                RadioButtonList rdoPaperOption = (RadioButtonList)item.FindControl("rdoPaperOption");

                if (base.SessionLicense.EnterpriseClient.ShowPrintingPreferences)
                {
                    ComboBoxControl_SelectedIndexChanged(ddlState, new RadComboBoxSelectedIndexChangedEventArgs(ddlState.SelectedItem.Text, selectedState, ddlState.SelectedItem.Value, selectedState));
                }
                else if (rdoPrintingOption != null && rdoPaperOption != null)
                {
                    gvSite.Columns.FindByUniqueName("colPrintingOption").Visible = false;
                    ((GridTableRow)item["colPrintingOption"].Parent).Cells[0].Visible = false;
                    gvSite.Columns.FindByUniqueName("colPrintingPaperOption").Visible = false;
                    ((GridTableRow)item["colPrintingPaperOption"].Parent).Cells[0].Visible = false;
                    rdoPrintingOption.Visible = false;
                    rdoPaperOption.Visible = false;
                }
            }
        }

        void ComboBoxControl_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            //one can reference the edited grid item through the NamingContainer attribute
            GridEditableItem editedItem = (sender as RadComboBox).NamingContainer as GridEditableItem;
            RadioButtonList rdoPrintingOption = (RadioButtonList)editedItem.FindControl("rdoPrintingOption");
            RadioButtonList rdoPaperOption = (RadioButtonList)editedItem.FindControl("rdoPaperOption");

            bool bInsertMode = false;
            string selectedState = string.Empty;
            int siteID = 0;
            gvSite.Columns.FindByUniqueName("colState").Visible = true;
            gvSite.Columns[6].Visible = true;

            RadComboBox cmbState = sender as RadComboBox;
            selectedState = cmbState.SelectedValue;
            if (editedItem is GridEditFormInsertItem || editedItem is GridDataInsertItem)
            {
                //insert mode
                bInsertMode = true;
            }
            else
            {
                bInsertMode = false;
                siteID = int.Parse(editedItem.GetDataKeyValue("SiteID").ToString());
            }

            bool bTogglePaper = false;

            bool bPrefPlain = false;
            string curFormatPref = string.Empty;

            //set print format
            int statePrintFormats = SystemConfig.GetStatePrintFormats(selectedState);
            bool bFourUp = SessionLicense.GetFeatureStatus(Constants.DeluxeFeatureType.FourUpPrinting) == Constants.DeluxeFeatureStatus.On && SystemConfig.AllowPrintFormat(Constants.PrintFormats.FOURUP, statePrintFormats);
            bool bFourRow = SystemConfig.AllowPrintFormat(Constants.PrintFormats.FOURROW, statePrintFormats);
            bool bOneUp = SystemConfig.AllowPrintFormat(Constants.PrintFormats.STANDARD, statePrintFormats);
            bool bMultiFormats = SystemConfig.AllowsMultipleFormats(statePrintFormats, bFourUp);

            if (!bMultiFormats || (SessionLicense.LicenseDeluxeStatus == Constants.DeluxeFeatureStatus.Off && selectedState == "PR"))
            {
                rdoPrintingOption.Visible = false;
                ((GridTableRow)editedItem["colPrintingOption"].Parent).Cells[0].Visible = false;
            }
            else
            {
                rdoPrintingOption.Visible = true;
                ((GridTableRow)editedItem["colPrintingOption"].Parent).Cells[0].Visible = true;

                if (!bInsertMode)
                {
                    curFormatPref = SitePreference.GetPreference(SessionLicenseID, siteID, "PRINTINGPREFERENCE", base.DBID);
                }

                if (rdoPrintingOption.Items.Count > 0)
                {   //when changing state in drop down list, items should be removed and re-created.
                    rdoPrintingOption.Items.Clear();
                }

                if (rdoPrintingOption.Items.Count == 0)
                {
                    rdoPrintingOption.Items.Add(new ListItem("1Up", "1"));
                    rdoPrintingOption.Items.Add(new ListItem("4Up", "4"));
                    rdoPrintingOption.Items.Add(new ListItem("4Row", "R"));
                }

                (rdoPrintingOption.Items.FindByText("4Up") as ListItem).Selected = curFormatPref == "4" && bFourUp;
                (rdoPrintingOption.Items.FindByText("4Row") as ListItem).Selected = curFormatPref == "R" && bFourRow;
                (rdoPrintingOption.Items.FindByText("1Up") as ListItem).Selected = !((curFormatPref == "R" && bFourRow) || (curFormatPref == "4" && bFourUp));

                if (!bOneUp)
                    rdoPrintingOption.Items.RemoveAt(rdoPrintingOption.Items.IndexOf(rdoPrintingOption.Items.FindByText("1Up")));

                if (!bFourUp)
                    rdoPrintingOption.Items.RemoveAt(rdoPrintingOption.Items.IndexOf(rdoPrintingOption.Items.FindByText("4Up")));

                if (!bFourRow)
                    rdoPrintingOption.Items.RemoveAt(rdoPrintingOption.Items.IndexOf(rdoPrintingOption.Items.FindByText("4Row")));
            }

            //set plain paper, security paper

            RadComboBox comboboxState = sender as RadComboBox;
            selectedState = comboboxState.SelectedValue;
            bTogglePaper = showPrintingPaper(selectedState);
            if (!bTogglePaper)
            {
                rdoPaperOption.Visible = false;
                gvSite.Columns.FindByUniqueName("colPrintingPaperOption").Visible = false;
                ((GridTableRow)editedItem["colPrintingPaperOption"].Parent).Cells[0].Visible = false;
            }
            else
            {
                gvSite.Columns.FindByUniqueName("colPrintingPaperOption").Visible = true;
                ((GridTableRow)editedItem["colPrintingPaperOption"].Parent).Cells[0].Visible = true;

                rdoPaperOption.Visible = true;

                if (!bInsertMode)
                {
                    bPrefPlain = Convert.ToBoolean(SitePreference.GetPreference(SessionLicenseID, siteID, "NONCSPLAINPAPER", base.DBID));
                    (rdoPaperOption.Items.FindByText("Plain Paper") as ListItem).Selected = bPrefPlain;
                    (rdoPaperOption.Items.FindByText("Security Paper") as ListItem).Selected = !bPrefPlain;
                }
                else
                {
                    //default selection
                    (rdoPaperOption.Items.FindByText("Plain Paper") as ListItem).Selected = true;
                    (rdoPaperOption.Items.FindByText("Security Paper") as ListItem).Selected = false;
                }
            }
        }

        void textbox_ServerValidate(object source, ServerValidateEventArgs args)
        {
            if (source is TextBox)
            {
                if (((TextBox)source).Text.Trim() == "")
                {
                    args.IsValid = false;
                }
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            Server.Transfer(AngularStringUtil.CreateUrl(Constants.PageNames.SETTINGS));
        }

        protected void chkMDDCheckBoxChange(object sender, EventArgs e)
        {
            var chkbox = (CheckBox)sender;
            var row = (GridEditFormItem)chkbox.NamingContainer;
            row.FindControl("rboMDDOption").Visible = chkbox.Checked;
            row.Selected = true;
        }

        protected void btnUniversal_Click(object sender, EventArgs e)
        {
            bool isUniversalSaveSuccessful = true;
            string exceptionID = null;
            var licId = PageState.GetStringOrEmpty("LICENSEID");
            var usrId = PageState.GetStringOrEmpty("USERID");

            PageState["LicenseName"] = txtLicenseName.Text.Trim();
            ApplicationLicense.UpdateLicenseName(SessionLicenseID, txtLicenseName.Text.Trim(), DBID);
            //get appInstanceID from SAML token in BasePage.cs

            try
            {
                EPSBroker.UpdateShieldTenant(licId, txtLicenseName.Text.Trim(), base.ShieldInternalTenantID, usrId, DBID);
            }
            catch (Exception exTenantUpdate)
            {
                Audit.AddException(usrId, licId, "Exception saving shield tenant name: " + exTenantUpdate.ToString(), "", "", "", DBID);
            }

            bool isRetrospectiveEPAChanged = IsRetrospectiveEPAChanged(chkShowRePA.Checked);

            var licensePreferences = CreatePreferencesFromCheckboxes();

            Preference[] preferences = new Preference[licensePreferences.Count];
            PageState[Constants.SessionVariables.LicensePreferences] = new LicensePreferences(licensePreferences);
            licensePreferences.Values.CopyTo(preferences, 0);
            try
            {
                if (!string.IsNullOrEmpty(licId))
                    Preference.SaveLicensePreferences(PreferenceCategory.LICENSE_OPTIONS, preferences, licId, usrId, DBID);
            }
            catch (Exception ex)
            {
                isUniversalSaveSuccessful = false;
                exceptionID = Audit.AddException(usrId, licId, "Exception saving license preference: " + ex.ToString(), "", "", "", base.DBID);
            }

            if (isUniversalSaveSuccessful)
            {
                ucMessage.MessageText = "Universal account settings successfully updated.";
                ucMessage.Icon = Controls_Message.MessageType.SUCCESS;
            }
            else
            {
                ucMessage.MessageText = "An error occurred while saving all of your Universal Account Settings.";

                if (!string.IsNullOrEmpty(exceptionID))
                {
                    ucMessage.MessageText = string.Concat(ucMessage.MessageText, " Error ID: ", exceptionID);
                }

                ucMessage.Icon = Controls_Message.MessageType.SUCCESS;
            }

            if (isRetrospectiveEPAChanged)
            {
                EPSBroker.InsertAllProvidersEnrollmentInLicense(SessionLicenseID, 0, DBID);
            }

            ucMessage.Visible = true;
        }

        private Hashtable CreatePreferencesFromCheckboxes()
        {
            Hashtable licensePreferences = new Hashtable();

            CreateAndAddPreference(chkShowCheckedInPatients.Checked, Constants.LicensePreferences.SHOW_CHECKED_IN_PATIENTS, ref licensePreferences);

            CreateAndAddPreference(chkShowRxInfo.Checked, Constants.LicensePreferences.SHOW_RX_INFO, ref licensePreferences);

            CreateAndAddPreference(chkShowRePA.Checked, Constants.LicensePreferences.SHOW_RETROSPECTIVE_EPA, ref licensePreferences);

            CreateAndAddPreference(chkShowPrebuiltRx.Checked, Constants.LicensePreferences.SHOW_PREBUILT_PRESCRIPTIONS, ref licensePreferences);

            CreateAndAddPreference(rdBtnLexicomp.Checked, Constants.LicensePreferences.SHOW_LEXICOMP, ref licensePreferences);

            return licensePreferences;
        }

        private void CreateAndAddPreference(bool prefValue, string prefName, ref Hashtable licensePreferences)
        {
            var pref = Preference.CreatePreference(string.Empty, prefName, PreferenceCategory.LICENSE_OPTIONS, false, prefValue.GetYorN());
            licensePreferences.Add(prefName, pref);
            PageState[prefName] = prefValue;
        }

        #endregion

        #region Custom Methods

        private void populateStates(Telerik.Web.UI.RadComboBox ddl, string selectedValue)
        {
            ddl.Items.Clear();
            DataTable dtStates = RxUser.ChGetState(base.DBID);
            foreach (DataRow dr in dtStates.Rows)
            {
                RadComboBoxItem rcbi = new RadComboBoxItem(dr["State"].ToString(), dr["State"].ToString());

                if (selectedValue == rcbi.Value)
                {
                    rcbi.Selected = true;
                }

                ddl.Items.Add(rcbi);
            }
        }

        private void populateTimeZone(Telerik.Web.UI.RadComboBox ddl, string selectedValue)
        {
            DataTable dt = SystemConfig.GetTimeZones(base.DBID);

            foreach (DataRow dr in dt.Rows)
            {
                RadComboBoxItem rcbi = new RadComboBoxItem(dr["DisplayName"].ToString(), dr["StandardName"].ToString());

                if (selectedValue == rcbi.Value)
                {
                    rcbi.Selected = true;
                }

                ddl.Items.Add(rcbi);
            }
        }

        private bool isAnyEnrollmentChanges(string licenseID, int siteID, string siteName, string addr1, string addr2, string city, string state,
                                              string zipCode, string phoneNumber, string faxNumber, string strTimeZone)
        {
            DataSet siteDS = ApplicationLicense.SiteLoad(licenseID, siteID, base.DBID);

            if (siteDS.Tables[0].Rows[0]["SiteName"].ToString().ToUpper().Trim() != siteName.ToUpper().Trim())
                return true;

            if (siteDS.Tables[0].Rows[0]["Address1"].ToString().ToUpper().Trim() != addr1.ToUpper().Trim())
                return true;

            if (siteDS.Tables[0].Rows[0]["Address2"].ToString().ToUpper().Trim() != addr2.ToUpper().Trim())
                return true;

            if (siteDS.Tables[0].Rows[0]["City"].ToString().ToUpper().Trim() != city.ToUpper().Trim())
                return true;

            if (siteDS.Tables[0].Rows[0]["State"].ToString().ToUpper().Trim() != state.ToUpper().Trim())
                return true;

            if (siteDS.Tables[0].Rows[0]["ZipCode"].ToString() != zipCode)
                return true;

            if (Allscripts.Impact.Utilities.StringHelper.FormatPhone(siteDS.Tables[0].Rows[0]["PhoneAreaCode"].ToString(), siteDS.Tables[0].Rows[0]["PhoneNumber"].ToString())
                != Allscripts.Impact.Utilities.StringHelper.FormatPhone(phoneNumber))
                return true;

            if (Allscripts.Impact.Utilities.StringHelper.FormatPhone(siteDS.Tables[0].Rows[0]["FaxAreaCode"].ToString(), siteDS.Tables[0].Rows[0]["FaxNumber"].ToString())
                != Allscripts.Impact.Utilities.StringHelper.FormatPhone(faxNumber))
                return true;

            if (siteDS.Tables[0].Rows[0]["TimeZone"].ToString() != strTimeZone)
                return true;

            return false;
        }

        private bool IsRetrospectiveEPAChanged(bool screenREpaValue)
        {
            return (screenREpaValue != PageState.GetBooleanOrFalse(Constants.LicensePreferences.SHOW_RETROSPECTIVE_EPA));
        }

        private bool showPrintingPaper(string selectedState)
        {
            int statePrintFormats = SystemConfig.GetStatePrintFormats(selectedState);
            bool bTogglePaper = false;
            switch (selectedState)
            {
                case "CA":
                case "IN":
                case "KY":
                    bTogglePaper = true;
                    break;
            }

            return bTogglePaper;
        }
        #endregion

        protected void btnBack_OnClick(object sender, EventArgs e)
        {
            Response.Redirect(AngularStringUtil.CreateUrl(Constants.PageNames.SETTINGS));
        }
    }

}