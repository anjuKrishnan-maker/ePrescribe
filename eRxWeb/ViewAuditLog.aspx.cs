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
using System.Text;
using Allscripts.Impact;
using Telerik.Web.UI;
using System.Globalization;
using Allscripts.Impact.Utilities;
using Allscripts.ePrescribe.Common;

namespace eRxWeb
{
public partial class ViewAuditLog : BasePage
{
	protected void Page_Load(object sender, EventArgs e)
	{
        if (!Page.IsPostBack)
		{
			startDate.SelectedDate = DateTime.Now;
			endDate.SelectedDate = DateTime.Now;
            startDate.Focus();
            Page.ClientScript.RegisterStartupScript(this.GetType(), "ClearOverrideReason", "togglePrivacyOverride('');", true);
            }
        }

    protected void Page_LoadComplete(object sender, EventArgs e)
    {
        ((PhysicianMasterPageBlank)Master).hideTabs();
    }

	protected void btnSearch_Click(object sender, EventArgs e)
	{
        if (Page.IsValid)
        {

            //Reset the combo box when new search launched
            int exportResetLocation = comboExport.FindItemIndexByText("Export");
            comboExport.SelectedIndex = exportResetLocation;

            ucMessage.MessageText = null;
            //panelPrivacyOverrideReason.Visible = false;
            //lblPrivacyOverrideReason.Text = string.Empty;
            if (string.IsNullOrEmpty(grdAuditLog.DataSourceID))
            {
                grdAuditLog.DataSourceID = "auditLogObjDS";
                comboExport.Visible = true;
            }

            //Patient Search: user typed or copy/pasted text into the box and didn't select from the intellisense list
            if (!string.IsNullOrWhiteSpace(ddlPatient.Text) && string.IsNullOrEmpty(ddlPatient.SelectedValue))
            {
                ucMessage.Icon = Controls_Message.MessageType.INFORMATION;
                ucMessage.MessageText = "The patient you entered (" + ddlPatient.Text + ") could not be identified. Please retype the patient's last name and select the patient from list. Results for your other search criteria are shown below.";
                ucMessage.Visible = true;
            }

            //User Search: user typed or copy/pasted text into the box and didn't select from the intellisense list
            if (!string.IsNullOrWhiteSpace(ddlUser.Text) && string.IsNullOrEmpty(ddlUser.SelectedValue))
            {
                string userMessage = "The user you entered (" + ddlUser.Text + ") could not be identified. Please select a user from the list. Results for your other search criteria are shown below.";

                ucMessage.Icon = Controls_Message.MessageType.INFORMATION;

                if (!string.IsNullOrEmpty(ucMessage.MessageText))
                {
                    ucMessage.MessageText = ucMessage.MessageText + "<br />" + userMessage;
                }
                else
                {
                    ucMessage.MessageText = userMessage;
                }

                ucMessage.Visible = true;
            }

            if (string.IsNullOrEmpty(ucMessage.MessageText))
            {
                ucMessage.Visible = false;
            }

                grdAuditLog.Rebind();
        }
        else
        {
            grdAuditLog.DataSourceID = null;
        }
	}

	protected void ddlUser_ItemsRequested(object o, Telerik.Web.UI.RadComboBoxItemsRequestedEventArgs e)
	{
        ddlUser.DataSource = ApplicationLicense.LoadAllUsers(base.SessionLicenseID, base.DBID);
		ddlUser.DataBind();

		e.Message = "Retrieving results...";
	}

    protected void btnBack_Click(object sender, EventArgs e)
    {
        Response.Redirect(Constants.PageNames.REDIRECT_TO_ANGULAR + "?componentName=" + Constants.PageNames.REPORTS);
    }

    protected void ddlPatient_ItemsRequested(object o, Telerik.Web.UI.RadComboBoxItemsRequestedEventArgs e)
	{
		string searchText;

		if (string.IsNullOrEmpty(e.Text))
		{
			return;
		}
		else
		{
			searchText = e.Text;
		}
        
        ddlPatient.DataSource = ApplicationLicense.PatientSearchAutoComplete(base.SessionLicenseID, searchText,base.SessionUserType,base.SessionUserID, base.DBID);

        ddlPatient.DataBind();
		e.Message = "Retrieving results...";
	}

    protected void cvFilterOptions_ServerValidate(object source, ServerValidateEventArgs args)
    {
        bool isValid = false;

        if (chkPatientFilter.Checked || chkAccountFilter.Checked || chkUserFilter.Checked)
        {
            isValid = true;
        }

        args.IsValid = isValid;
    }

    protected void cvDateSearchOptions_ServerValidate(object source, ServerValidateEventArgs args)
    {
        bool isValid = false;
        cvDateSearchOptions.ErrorMessage = "Please enter valid date filters.";

        if (startDate.SelectedDate != null && endDate.SelectedDate != null)
        {
            isValid = true;    
        }

        if (isValid && endDate.SelectedDate.Value.CompareTo(startDate.SelectedDate.Value) == -1)
        {
            isValid = false;
            cvDateSearchOptions.ErrorMessage = "End date cannot be earlier than start date.";
        }

        args.IsValid = isValid;
    }

    protected void comboExport_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
    {
        grdAuditLog.ExportSettings.ExportOnlyData = true;
        grdAuditLog.ExportSettings.IgnorePaging = true;
        grdAuditLog.ExportSettings.OpenInNewWindow = true;

        switch (comboExport.SelectedValue)
        {
            case "Excel":
                grdAuditLog.MasterTableView.ExportToExcel();
                break;
            case "PDF":
                StringBuilder sbTitle = new StringBuilder();

                sbTitle.Append("Audit Log for ");
                sbTitle.Append(startDate.SelectedDate.Value.ToShortDateString());
                sbTitle.Append(" to ");
                sbTitle.Append(endDate.SelectedDate.Value.ToShortDateString());

                grdAuditLog.ExportSettings.Pdf.Title = sbTitle.ToString();
                grdAuditLog.ExportSettings.Pdf.PageTitle = sbTitle.ToString();
                grdAuditLog.ExportSettings.Pdf.PaperSize = GridPaperSize.Letter;
                grdAuditLog.ExportSettings.Pdf.PageHeight = new Unit(8.5, UnitType.Inch);
                grdAuditLog.ExportSettings.Pdf.PageWidth = new Unit(11, UnitType.Inch);
                grdAuditLog.MasterTableView.ExportToPdf();

                break;
            /*Do nothing if export button pressed, discussed with BA*/
            case "Default":
            default:
                break;

        }
    }

    protected void grdAuditLog_RowCommand(object sender, GridCommandEventArgs e)
    {
        if (e.CommandName == "OverrideReason")
        {
            int index = Convert.ToInt32(e.Item.ItemIndex.ToString());

            string patientId = grdAuditLog.Items[index].GetDataKeyValue("PatientId").ToString();
            string userId = grdAuditLog.Items[index].GetDataKeyValue("UserID").ToString();
           
            GridDataItem item = (GridDataItem)e.Item;
            HiddenField hdnFldAuditCreatedUtc = (HiddenField)item.FindControl("hdnFldAuditCreatedUtc");

            DataTable dtOverrideReason = new PatientPrivacy().GetPatientPrivacyOverrideReason(patientId, userId, hdnFldAuditCreatedUtc.Value, base.DBID);
            if (dtOverrideReason.Rows.Count > 0)
            {
                string OverrideReason = dtOverrideReason.Rows[0]["OverrideReason"].ToString();
                //lblPrivacyOverrideReason.Text = OverrideReason.Trim();
                //panelPrivacyOverrideReason.Visible = true;
            }
        }
    } 

    protected void grdAuditLog_RowDataBound(object sender, GridItemEventArgs e)
    {
        if (e.Item is GridDataItem)
        {
            GridDataItem tempDataItem = (GridDataItem)e.Item;
            Label lblAuditAction = tempDataItem.FindControl("lblAuditAction") as Label;
            if (lblAuditAction.Text.Trim().Equals("Restricted Patient Viewed", StringComparison.OrdinalIgnoreCase))
            {
                var lnkBtnAuditAction = tempDataItem.FindControl("lnkBtnAuditAction") as LinkButton;
                if (lnkBtnAuditAction != null)
                {
                    var patientId = tempDataItem.GetDataKeyValue("PatientId").ToString();
                    var userId = tempDataItem.GetDataKeyValue("UserID").ToString();
                    var hdnFldAuditCreatedUtc = (HiddenField)tempDataItem.FindControl("hdnFldAuditCreatedUtc");

                    lnkBtnAuditAction.OnClientClick = $"togglePrivacyOverride('{patientId}', '{userId}', '{hdnFldAuditCreatedUtc.Value}'); return false;";
                    lnkBtnAuditAction.Visible = true;
                }
                lblAuditAction.Visible = false;
            }

            bool IsVIPPatient = (bool)tempDataItem.GetDataKeyValue("IsVIPPatient");
            if (IsVIPPatient || (bool)tempDataItem.GetDataKeyValue("IsRestrictedPatient"))
            {
                Image shieldImage = (Image)tempDataItem.FindControl("shieldImage");
                shieldImage.Visible = true;
                if (IsVIPPatient)
                {
                    shieldImage.Visible = false;
                    ImageButton shieldImageButton = (ImageButton)tempDataItem.FindControl("shieldImageButton");
                    shieldImageButton.Visible = true;
                    shieldImageButton.ToolTip = "Click here to view real name";
                }
            }

           }
    }

        protected void shieldImageButton_Click(object sender, ImageClickEventArgs e)
        {
            GridDataItem gvr = (GridDataItem)((WebControl)sender).Parent.Parent;
            string patientID = gvr.GetDataKeyValue("PatientId").ToString();

            ImageButton shieldImageButton = (ImageButton)sender;
            RadToolTip rd = (RadToolTip)gvr.FindControl("realNameToolTip");
            rd.TargetControlID = shieldImageButton.ClientID;
            DataRow dr = Patient.GetPatientRealName(patientID, base.DBID);
            rd.Text = dr["LasName"] + ", " + dr["FirstName"];
            rd.Show();
        }
    }

}