#region Referencess
using System;
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
using Allscripts.Impact;
using Allscripts.Impact.Utilities;
using Telerik.Web.UI;
using System.Text;
using Allscripts.ePrescribe.Common;
using eRxWeb.ServerModel;
using System.Web.Script.Serialization;
using eRxWeb.AppCode;
using ScriptMessage = Allscripts.Impact.ScriptMsg.ScriptMessage;

#endregion

namespace eRxWeb
{
    public partial class MessageQueueTx : BasePage
    {

        #region Page Variables

        private const string _defaultValue = "All";
        private bool _forceDefaults = true;
        private bool _failedRxLinked = false;

        #endregion

        #region Page Events

        protected void Page_Load(object sender, EventArgs e)
        {
            //User Controls Ajax Settings
            RadGrid grdViewPatients = (RadGrid) ucPatientSearchWithResults.FindControl("grdViewPatients");
            Button btnCancelAndClear = (Button) ucPatientSearchWithResults.FindControl("btnCancelAndClear");
            Panel pnlMainSearchPatient = (Panel) ucPatientSearchWithResults.FindControl("pnlMainSearchPatient");

            RadAjaxManager1.AjaxSettings.AddAjaxSetting(grdViewPatients, pnlMsgQueueSearch);
            RadAjaxManager1.AjaxSettings.AddAjaxSetting(btnCancelAndClear, pnlMsgQueueSearch);
            RadAjaxManager1.AjaxSettings.AddAjaxSetting(lnkSelectPatient, grdViewPatients);
            RadAjaxManager1.AjaxSettings.AddAjaxSetting(lnkSelectPatient, pnlMainSearchPatient);

            this.Page.Form.DefaultButton = btnSearch.UniqueID;
            var minDate = DateTime.Now.AddDays(-60);

            if (!Page.IsPostBack)
            {
                if (Session["SiteName"] != null)
                {
                    lblHeader.Text += Session["SiteName"].ToString();
                }

                if (Request.QueryString["Start"] != null)
                {
                    DateTime reqStart = DateTime.Now;

                    if (!DateTime.TryParse(Request.QueryString["Start"], out reqStart))
                    {
                        startDate.MinDate = minDate;
                    }
                    else
                    {
                        //Use the minimume start date.
                        startDate.MinDate = DateTime.Compare(reqStart, minDate) < 0
                            ? reqStart
                            : minDate;
                    }
                }
                else
                {
                    startDate.MinDate = minDate;
                }

                startDate.Focus();
                endDate.MinDate = minDate;
                startDate.SelectedDate = DateTime.Now;
                endDate.SelectedDate = DateTime.Now;

                if (Cache[msgQueueObjDS.CacheKeyDependency] == null)
                {
                    Cache[msgQueueObjDS.CacheKeyDependency] = new object();
                }

                if (Request.QueryString["View"] != null &&
                    Request.QueryString["View"].Equals("Errors", StringComparison.OrdinalIgnoreCase))
                {
                    _failedRxLinked = true;
                    ddlView.SelectedIndex = ddlView.FindItemIndexByText("Error");
                }

                loadAuthorizers();
                loadOriginators();

                if (Request.QueryString["Auth"] != null)
                {
                    RadComboBoxItem item = ddlAuthorizer.FindItemByValue(Request.QueryString["Auth"]);

                    if (item != null)
                    {
                        item.Selected = true;
                    }
                }
                else
                {
                    RadComboBoxItem item = ddlAuthorizer.FindItemByText(_defaultValue);
                    if (item != null)
                    {
                        item.Selected = true;
                    }
                }


                if (Request.QueryString["Orig"] != null)
                {
                    RadComboBoxItem item = ddlOriginator.FindItemByValue(Request.QueryString["Orig"]);

                    if (item != null)
                    {
                        item.Selected = true;
                    }
                }
                else
                {
                    RadComboBoxItem item = ddlOriginator.FindItemByText(_defaultValue);
                    if (item != null)
                    {
                        item.Selected = true;
                    }
                }

                if (Request.QueryString["Start"] != null)
                {
                    DateTime reqStart = DateTime.Now;

                    if (!DateTime.TryParse(Request.QueryString["Start"], out reqStart))
                    {
                        reqStart = DateTime.UtcNow;
                    }

                    startDate.SelectedDate = reqStart;
                }

                compareValidatorStartDate.ValueToCompare = DateTime.Now.ToShortDateString();
                compareValidatorEndDate.ValueToCompare = DateTime.Now.ToShortDateString();
            }

            grdMessageQueue.Attributes.Add("SelectedScriptMessage", string.Empty);


            //if (SessionLicense.LicenseDeluxeStatus != Constants.DeluxeFeatureStatus.On)
            //{
            //    // Get AD placement from TIE service.
            //    GetAdPlacement(Constants.TIELocationPage.MessageQueueTx_Page);
            //}
            if (SessionLicense.LicenseDeluxeStatus != Constants.DeluxeFeatureStatus.On)
            {
                //divHideTools_Help.Style.Add("display", "none");
                // Hiding HelpContent if the User is a Basic user (to highlight LogRx ads)
            }
        }

        protected void Page_LoadComplete(object sender, EventArgs e)
        {
            ((PhysicianMasterPageBlank) Master).hideTabs();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                btnConfirm.Enabled = false;
                _forceDefaults = false;
                ucMessage.Visible = false;
                clearRightPanel();
                grdMessageQueue.Rebind();
            }
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            if (Session["SSOMode"] != null && Session["SSOMode"].ToString() == Constants.SSOMode.PATIENTLOCKDOWNMODE)
            {
                if (Session["DefaultPatientLockDownPage"] != null)
                {
                    Response.Redirect(Session["DefaultPatientLockDownPage"].ToString());
                }
                else
                {
                    SelectPatientComponentParameters selectPatientComponentParameters = new SelectPatientComponentParameters
                    {
                        PatientId = PageState.GetStringOrEmpty(Constants.SessionVariables.PatientId)
                    };
                    var componentParameters = new JavaScriptSerializer().Serialize(selectPatientComponentParameters);
                    Response.Redirect(Constants.PageNames.REDIRECT_TO_ANGULAR + "?componentName=" + Constants.PageNames.SELECT_PATIENT + "&componentParameters=" + componentParameters);
                }
            }
            else if (Session["SSOMode"] != null && Session["SSOMode"].ToString() == Constants.SSOMode.TASKMODE)
            {
                if (Convert.ToBoolean(Session["IsProvider"]) || Convert.ToBoolean(Session["IsPA"]))
                    Response.Redirect("~/" + Constants.PageNames.DOC_REFILL_MENU);
                else if ((Constants.UserCategory) Session["UserType"] == Constants.UserCategory.GENERAL_USER ||
                         ((Constants.UserCategory) Session["UserType"] == Constants.UserCategory.POB_LIMITED))
                    Response.Redirect(((PhysicianMasterPageBlank) Master).ReportsLinkURL);
                else
                    Response.Redirect("~/" + Constants.PageNames.LIST_SEND_SCRIPTS);
            }
            else if (Session["SSOMode"] != null &&
                     Session["SSOMode"].ToString() == Constants.SSOMode.EPAPATIENTLOCKDOWNTASKMODE)
            {
                if (Convert.ToBoolean(Session["IsProvider"]) || Convert.ToBoolean(Session["IsPA"]))
                    Response.Redirect("~/" + Constants.PageNames.DOC_REFILL_MENU);
                else if ((Constants.UserCategory) Session["UserType"] == Constants.UserCategory.GENERAL_USER ||
                         ((Constants.UserCategory) Session["UserType"] == Constants.UserCategory.POB_LIMITED))
                    Response.Redirect(((PhysicianMasterPageBlank) Master).ReportsLinkURL);
                else
                    Response.Redirect("~/" + Constants.PageNames.LIST_SEND_SCRIPTS);
            }
            else
            {
                if (Request.QueryString["From"] != null && !Request.QueryString["From"].ToLower().Contains(Constants.PageNames.REPORTS.ToLower()))
                {
                    Response.Redirect(RedirectHelper.GetRedirectToSPAUrl(Request.QueryString["From"], true));
                }
                else
                {
                    Response.Redirect(RedirectHelper.GetRedirectToSPAUrl(((PhysicianMasterPageBlank)Master).ReportsLinkURL, true));
                }
            }
        }

        protected void btnConfirm_Click(object sender, EventArgs e)
        {
            if (grdMessageQueue.SelectedItems.Count > 0)
            {
                RxError.SaveRxErrorMQTx(
                    grdMessageQueue.SelectedValues["ScriptMessageID"].ToString(),
                    string.Empty,
                    base.SessionUserID,
                    base.DBID);

                Prescription.UpdateTransmissionStatus(grdMessageQueue.SelectedValues["RxID"].ToString(),
                    (int) Constants.TransmissionStatus.PHONED_IN, base.DBID);

                Cache[msgQueueObjDS.CacheKeyDependency] = new object();
                grdMessageQueue.Attributes["SelectedScriptMessage"] =
                    grdMessageQueue.SelectedValues["ScriptMessageID"].ToString();
                grdMessageQueue.Rebind();

                btnConfirm.Enabled = false;
                ucMessage.MessageText = "Rx error confirmed.";
                ucMessage.Icon = Controls_Message.MessageType.SUCCESS;
                ucMessage.Visible = true;
            }
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            startDate.SelectedDate = DateTime.Now;
            endDate.SelectedDate = DateTime.Now;
            ddlView.SelectedIndex = 0;
            loadAuthorizers();
            loadOriginators();

            ddlAuthorizer.SelectedIndex = 0;
            ddlOriginator.SelectedIndex = 0;

            clearPatientInformation();
        }

        protected void rbtnPrescriptions_OnCheckedChanged(object sender, EventArgs e)
        {
            Server.Transfer(Constants.PageNames.MESSAGE_QUEUE_TX);
        }

        protected void lnkSelectPatient_Click(object sender, EventArgs e)
        {
            ucPatientSearchWithResults.Show(true);
        }

        protected void hiddenSelect_Click(object sender, EventArgs e)
        {
            _forceDefaults = false;

            setSelectedScript(HiddenFieldscriptMessageID.Value.Trim(),
                HiddenFieldErrorConfirmedByName.Value.Trim(),
                HiddenFieldErrorConfirmedDateTime.Value.Trim(),
                HiddenFieldDisplayType.Value.Trim(),
                HiddenFieldDisplayStatus.Value.Trim());
        }

        protected void patientSelectEvent(object sender, PatientSelectEventArgs e)
        {
            if (e.Action == PatientSelectEventArgs.ActionType.SELECT_PATIENT)
            {
                setPatientInformation(e.PatientID, e.Name);
            }
            else if (e.Action == PatientSelectEventArgs.ActionType.CLEAR_PATIENT)
            {
                hdnPatientID.Value = e.PatientID;
                clearPatientInformation();
            }
        }

        #endregion

        #region Grid Events

        protected void grdMessageQueue_ItemDataBound(object sender, Telerik.Web.UI.GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                GridDataItem tempDataItem = (GridDataItem) e.Item;
                tempDataItem.Style["cursor"] = "pointer";

                Label lblDisplayStatus = (Label) tempDataItem.FindControl("lblDisplayStatus");
                Label lblStatusMessage = (Label) tempDataItem.FindControl("lblStatusMessage");
                string errorConfirmedByName = tempDataItem.GetDataKeyValue("ErrorConfirmedByName").ToString().Trim();

                HtmlInputRadioButton rbSelect = tempDataItem.FindControl("rbSelect") as HtmlInputRadioButton;

                if (rbSelect != null)
                {
                    rbSelect.Attributes.Add("ScriptMessageID",
                        tempDataItem.GetDataKeyValue("ScriptMessageID").ToString());
                    rbSelect.Attributes.Add("onclick",
                        "rbSelect_OnClick('" + tempDataItem.GetDataKeyValue("ScriptMessageID").ToString() + "','" +
                        tempDataItem.GetDataKeyValue("ErrorConfirmedByName").ToString() + "','" +
                        tempDataItem.GetDataKeyValue("ErrorConfirmedDateTime").ToString() + "','" +
                        tempDataItem.GetDataKeyValue("DisplayType").ToString() + "','" +
                        tempDataItem.GetDataKeyValue("DisplayStatus").ToString() + "')");
                }

                if (string.Compare(grdMessageQueue.Attributes["SelectedScriptMessage"],
                    tempDataItem.GetDataKeyValue("ScriptMessageID").ToString(), true) == 0)
                {
                    tempDataItem.Selected = true;
                    setSelectedScript(tempDataItem.GetDataKeyValue("ScriptMessageID").ToString().Trim(),
                        errorConfirmedByName,
                        tempDataItem.GetDataKeyValue("ErrorConfirmedDateTime").ToString().Trim(),
                        tempDataItem.GetDataKeyValue("DisplayType").ToString().Trim(),
                        tempDataItem.GetDataKeyValue("DisplayStatus").ToString().Trim());

                }

                if ((bool)tempDataItem.GetDataKeyValue("IsVIPPatient") || (bool)tempDataItem.GetDataKeyValue("IsRestrictedPatient"))
                {
                    Image img = new Image();
                    img.ImageUrl = @"~\images\PrivacyImages\sensitivehealth-global-16-x-16.png";
                    tempDataItem["ImgColumn"].Controls.Add(img);
                    Label lbl = new Label();
                    lbl.Text = " " + tempDataItem.Cells[7].Text;
                    if (tempDataItem.GetDataKeyValue("DisplayStatus").ToString().Trim().Equals("ERROR", StringComparison.OrdinalIgnoreCase))
                    {
                        lbl.ForeColor = System.Drawing.Color.Red;
                    }
                    tempDataItem["ImgColumn"].Controls.Add(lbl);
                }

                if (tempDataItem.GetDataKeyValue("DisplayStatus")
                    .ToString()
                    .Trim()
                    .Equals("ERROR", StringComparison.OrdinalIgnoreCase))
                {
                    ((Label) tempDataItem.FindControl("lblDisplayStatus")).ForeColor = System.Drawing.Color.Red;
                    ((Label) tempDataItem.FindControl("lblDisplayType")).ForeColor = System.Drawing.Color.Red;
                    ((Label) tempDataItem.FindControl("lblStatusMessage")).ForeColor = System.Drawing.Color.Red;

                    lblDisplayStatus.Text = (string.IsNullOrEmpty(errorConfirmedByName) ? "Unconfirmed Error" : "Error");

                    foreach (TableCell cell in tempDataItem.Cells)
                    {
                        cell.ForeColor = System.Drawing.Color.Red;
                    }

                    ((Image) tempDataItem.FindControl("imgAlert")).ImageUrl = "images/warning-16x16.png";
                }

                switch (lblDisplayStatus.Text.Trim().ToUpper())
                {
                    case "PENDING":
                        lblStatusMessage.Text = "Message pending. Awaiting send.";
                        break;
                    case "SENT":
                        lblStatusMessage.Text = "Message sent. Awaiting confirmation.";
                        break;
                    case "CONFIRMED":
                        lblStatusMessage.Text = "Message sent and confirmed.";
                        break;
                    default:
                        break;
                }
            }
        }

        #endregion

        #region Private Methods

        private void setSelectedScript(string scriptMessageId, string errorConfirmedByName,
            string errorConfirmedDateTime, string displayType, string displayStatus)
        {
            //panelMsgDetail.Visible = true;
            //trPharmacyNotes.Visible = false;
            //trPharmacyNotesHr.Visible = false;

            ScriptMessage sm = new ScriptMessage();

            if (displayStatus.Equals("ERROR", StringComparison.OrdinalIgnoreCase))
            {
                sm = new ScriptMessage(scriptMessageId, base.SessionLicenseID, base.SessionUserID, base.DBID);

                //check if it was confirmed and by whom
                if (!string.IsNullOrEmpty(errorConfirmedByName))
                {
                   // panelConfirmedBy.Visible = true;
                    //lblConfirmedBy.Text = errorConfirmedByName;
                   // lblConfirmedDateTime.Text = errorConfirmedDateTime;
                }
                else
                {
                    //panelConfirmedBy.Visible = false;
                }
            }
            else
            {
                sm = new ScriptMessage(scriptMessageId, base.SessionLicenseID, base.SessionUserID, base.DBID);
               // panelConfirmedBy.Visible = false;
            }

            btnConfirm.Enabled = string.IsNullOrEmpty(errorConfirmedByName) &&
                                 displayStatus.Equals("ERROR", StringComparison.OrdinalIgnoreCase);

            if (sm.MessageType.Trim().Equals(Constants.MessageTypes.CANRX, StringComparison.OrdinalIgnoreCase))
            {
                //lblPharmacyNotes.Text = "No response received from pharmacy.";
                //trPharmacyNotes.Visible = true;
                //trPharmacyNotesHr.Visible = true;

                /*DataRow cancelRxResponseStatus = ScriptMessage.GetCancelRxResponseStatus(scriptMessageId, base.DBID);

                if (cancelRxResponseStatus != null)
                {
                    if (cancelRxResponseStatus["Status"].ToString() == "A")
                    {
                       // lblPharmacyNotes.Text = "Approved - " + cancelRxResponseStatus["Message"].ToString();
                    }
                    else if (cancelRxResponseStatus["Status"].ToString() == "D")
                    {
                        if (cancelRxResponseStatus["DenialReason"] == null)
                        {
                            //lblPharmacyNotes.Text = "Denied - " + cancelRxResponseStatus["Message"].ToString();
                        }
                        else
                        {
                            //lblPharmacyNotes.Text = "Denied - " + cancelRxResponseStatus["DenialReason"].ToString();
                        }
                    }
                }*/
            }

            // This looks like code from the legacy message queue, only outbound script messages are placed in the message_queue_tx table
            if ((sm.MessageType.Equals("VERIFY", StringComparison.OrdinalIgnoreCase) ||
                 sm.MessageType.Equals("CANRES", StringComparison.OrdinalIgnoreCase))
                && sm.LinkedTransCtrlNo.Length > 0)
            {
                //Only get the linked message when it's a verify message
                sm = new ScriptMessage(sm.LinkedTransCtrlNo, base.SessionLicenseID, base.SessionUserID, base.DBID);
            }

            //lblPtName.Text = sm.PatientFirstName + " " + sm.PatientLastName;
            //lblPtAddress.Text = sm.PatientAddress1;
            //lblPtCity.Text = sm.PatientCity;
            //lblPtState.Text = sm.PatientState;
            //lblPtZIP.Text = sm.PatientZip;
            //lblPtPhone.Text = Allscripts.Impact.Utilities.StringHelper.FormatPhone(sm.PatientPhoneNumber);
            //lblPtGender.Text = sm.PatientGender;
            //lblRxDate.Text = sm.DBCreatedLocal.ToString();
            //lblRxDesc.Text = sm.RxDrugDescription;
            //lblSig.Text = sm.RxSIGText;
            //lblQty.Text = sm.RxQuantity != String.Empty ? Convert.ToDouble(sm.RxQuantity).ToString() : String.Empty;
            //lblRefills.Text = sm.RxRefills;
            //lblDAW.Text = sm.RxDAW;
            //lblNotes.Text = sm.RxPharmacyNotes;
            //lblPharmName.Text = sm.PharmacyName;

            if (sm.PharmacistFirstName.Length > 0)
            {
                //panelPharmicistName.Visible = true;
                //lblPharmacist.Text = sm.PharmacistFirstName + " " + sm.PharmacistLastName;
            }
            else
            {
                //panelPharmicistName.Visible = false;
            }

            //lblPharmNABP.Text = sm.DBPharmacyNetworkID;
            //lblPharmAddress.Text = sm.PharmacyAddress1;

            if (sm.PharmacyAddress2.Length > 0)
            {
                //lblPharmAddress.Text += ", " + sm.PharmacyAddress2;
            }

            //lblPharmCity.Text = sm.PharmacyCity;
            //lblPharmState.Text = sm.PharmacyState;
            //lblPharmZIP.Text = sm.PharmacyZip;

            //lblPharmPhone.Text = Allscripts.Impact.Utilities.StringHelper.FormatPhone(sm.PharmacyPhoneNumber);
            //lblPharmFax.Text = Allscripts.Impact.Utilities.StringHelper.FormatPhone(sm.PharmacyFaxNumber);

            if (!string.IsNullOrWhiteSpace(sm.DBPatientID) && sm.DBPatientID != "00000000-0000-0000-0000-000000000000")
            {
                base.AuditLogPatientInsert(ePrescribeSvc.AuditAction.PATIENT_RX_VIEW_VIA_MSG_QUEUE, sm.DBPatientID);
            }
        }

        private void setPatientInformation(string patientID, string patientName)
        {
            lnkSelectPatient.Text = "Change";
            hdnPatientID.Value = patientID;
            lblPatient.Text = patientName;
            Cache[msgQueueObjDS.CacheKeyDependency] = new object();
        }

        private void clearPatientInformation()
        {
            hdnPatientID.Value = string.Empty;
            lnkSelectPatient.Text = "Search Patient";
            lblPatient.Text = string.Empty;
            Cache[msgQueueObjDS.CacheKeyDependency] = new object();
        }

        private void clearRightPanel()
        {
            //panelMsgDetail.Visible = false;
        }

        private void loadAuthorizers()
        {
            ddlAuthorizer.DataTextField = "ProviderName";
            ddlAuthorizer.DataValueField = "ProviderID";
            DataSet drListProvider = getProviders();

            DataView activeProvidersView = drListProvider.Tables["Provider"].DefaultView;
            activeProvidersView.RowFilter = "UserType IN ('1','1000','1001') AND Active='Y'";

            ddlAuthorizer.DataSource = activeProvidersView;
            ddlAuthorizer.DataBind();

            if (ddlAuthorizer.Items.Count == 0 || ddlAuthorizer.Items[0].Text != _defaultValue)
            {
                ddlAuthorizer.Items.Insert(0, new Telerik.Web.UI.RadComboBoxItem(_defaultValue));
            }
        }

        private void loadOriginators()
        {
            ddlOriginator.DataTextField = "UserFullName";
            ddlOriginator.DataValueField = "UserID";

            DataSet drListProviders = getProviders();
            DataSet drListPOBs = getPOBs();

            DataSet dsOriginators = new DataSet();
            DataRow drOriginator;

            dsOriginators.Tables.Add(new DataTable());
            dsOriginators.Tables[0].Columns.Add(new DataColumn("UserFullName", typeof (string)));
            dsOriginators.Tables[0].Columns.Add(new DataColumn("UserID", typeof (string)));

            DataRow[] allTypeProviders =
                drListProviders.Tables["Provider"].Select("UserType IN ('1','1000','1001') AND Active='Y'");

            foreach (DataRow provider in allTypeProviders)
            {
                drOriginator = dsOriginators.Tables[0].NewRow();
                drOriginator["UserFullName"] = provider["ProviderName"].ToString();
                drOriginator["UserID"] = provider["ProviderID"].ToString();
                dsOriginators.Tables[0].Rows.Add(drOriginator);
            }

            foreach (DataRow pob in drListPOBs.Tables[0].Rows)
            {
                drOriginator = dsOriginators.Tables[0].NewRow();
                drOriginator["UserFullName"] = pob["POBName"].ToString();
                drOriginator["UserID"] = pob["POBID"].ToString();
                dsOriginators.Tables[0].Rows.Add(drOriginator);
            }

            DataView originatorsView = dsOriginators.Tables[0].DefaultView;
            originatorsView.Sort = "UserFullName ASC";

            ddlOriginator.DataSource = originatorsView;
            ddlOriginator.DataBind();

            if (ddlOriginator.Items.Count == 0 || ddlOriginator.Items[0].Text != _defaultValue)
            {
                ddlOriginator.Items.Insert(0, new Telerik.Web.UI.RadComboBoxItem(_defaultValue));
            }
        }

        private DataSet getProviders()
        {
            DataSet dsProviders = null;

            if (ViewState["Providers"] == null)
            {
                dsProviders = Provider.GetProviders(base.SessionLicenseID, base.SessionSiteID, base.DBID);
                ViewState["Providers"] = dsProviders;
            }
            else
            {
                dsProviders = (DataSet) ViewState["Providers"];
            }

            return dsProviders;
        }

        private DataSet getPOBs()
        {
            DataSet dsPOBs = null;

            if (ViewState["POBs"] == null)
            {
                dsPOBs = Provider.GetPOBs(base.SessionLicenseID, base.DBID);
                ViewState["POBs"] = dsPOBs;
            }
            else
            {
                dsPOBs = (DataSet) ViewState["POBs"];
            }

            return dsPOBs;
        }

        #endregion

        #region Object Data Source Events

        protected void msgQueueObjDS_Selected(object sender, ObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception == null)
            {
                ePrescribeSvc.MessageQueue[] items = (ePrescribeSvc.MessageQueue[]) e.ReturnValue;

                int maxRows = int.Parse(System.Configuration.ConfigurationManager.AppSettings["MessageQueueMaxRows"]);

                if (items != null && items.Length > maxRows)
                {
                    ucMessage.MessageText = string.Concat("Your search returned more than ", maxRows.ToString(),
                        " results. Please consider refining your search.");
                    ucMessage.Icon = Controls_Message.MessageType.INFORMATION;
                    ucMessage.Visible = true;
                }
                else
                {
                    ucMessage.Visible = false;
                }
            }
            else
            {
                ucMessage.MessageText = "Could not complete search.  Please check your search criteria and try again.";
                ucMessage.Icon = Controls_Message.MessageType.ERROR;
                ucMessage.Visible = true;

                e.ExceptionHandled = true;
            }

            if (Session["SSOMode"] != null && Session["SSOMode"].ToString() == Constants.SSOMode.PATIENTLOCKDOWNMODE)
            {
                if (_forceDefaults)
                {
                    DateTime today = DateTime.Today;
                    startDate.SelectedDate = today.AddDays(-7);

                    if (Session["PATIENTID"] != null && Session["PATIENTNAME"] != null)
                    {
                        setPatientInformation(Session["PATIENTID"].ToString(), Session["PATIENTNAME"].ToString());
                    }
                }
            }
        }

        protected void msgQueueObjDS_Selecting(object sender, ObjectDataSourceSelectingEventArgs e)
        {
            //Do not populate grid on intial page load, unless it is from the failed Rx Link
            if (!Page.IsPostBack && !_failedRxLinked)
            {
                e.Cancel = true;
            }
            else
            {
                if (Session["SSOMode"] != null && Session["SSOMode"].ToString() == Constants.SSOMode.PATIENTLOCKDOWNMODE)
                {
                    if (_forceDefaults)
                    {
                        DateTime today = DateTime.Today;
                        startDate.SelectedDate = today.AddDays(-7);
                        e.InputParameters["startDate"] = startDate.SelectedDate;
                    }
                }

                e.InputParameters["patientID"] = hdnPatientID.Value;
            }
        }

        #endregion
    }
}