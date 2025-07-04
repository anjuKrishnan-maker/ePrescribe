//Revision History
/******************************************************************************
* Change History
* Date:      Author:                    Description:
* -----------------------------------------------------------------------------
* 06/29/2009   Dharani Reddem         I# 2728 -Merge Patient - Search patient with one character
*                                     returns results without any error message .    
* 07/01/2009   Dharani Reddem         Added aditional validation - .,',- for lastname and firstname fields .
* 08/13/2009   Dharani Reddem         Issue# 2785 - Merge Patients -Fixed- When two patients are merged and the Merge 
*                                     Patients Cofirmation popup page closes, the message "Patients successfully merged." does not display
*******************************************************************************/
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
using Telerik.Web.UI;
using Allscripts.Impact;
using Allscripts.Impact.Utilities.Win32;
using Allscripts.Impact.Utilities;
using System.Text.RegularExpressions;
using Allscripts.ePrescribe.Common;
using eRxWeb.AppCode;

namespace eRxWeb
{
public partial class MergePatients : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        cvConfirmChecked.EnableClientScript = false;
        
        Pat1ObjDataSource.SelectParameters["HasVIPPatients"].DefaultValue = base.SessionLicense.hasVIPPatients.ToString();
        Pat2ObjDataSource.SelectParameters["HasVIPPatients"].DefaultValue = base.SessionLicense.hasVIPPatients.ToString();

            if (!Page.IsPostBack)
        {
            string panelContent = Helper.GetHelpText(this.AppRelativeVirtualPath);
            txtSearchPat1LastName.Focus();
        }
    }

    protected void Page_LoadComplete(object sender, EventArgs e)
    {
        ((PhysicianMasterPageBlank)Master).hideTabs();
    }

	protected void grdViewPat1_ItemDataBound(object sender, GridItemEventArgs e)
    {
        if (e.Item is GridDataItem)
        {
            e.Item.Style["cursor"] = "pointer";

            try
            {
                int isRestricted= 0;
                if ((bool)((DataRowView)e.Item.DataItem).Row["IsVIPPatient"] || (bool)((DataRowView)e.Item.DataItem).Row["IsRestrictedPatient"])
                    {
                        Image confidentialIcon = (Image)e.Item.FindControl("confidentialIcon");
                        confidentialIcon.Visible = true;
                        isRestricted = 1;
                    }

                    e.Item.Attributes.Add("onclick", "onRowClick1(this,'" + isRestricted + "')") ;

                    Label lblLastChange = (Label) e.Item.FindControl("lblLastChange");
                lblLastChange.Text = SystemConfig.GetLocalTime(Session["TimeZone"].ToString(), ((DataRowView)e.Item.DataItem).Row["LastChange"].ToString()).ToString();

                int statusID = int.Parse(((DataRowView)e.Item.DataItem).Row["StatusID"].ToString());
                if (statusID == 0)
                {
                    //inactive patient!
                    Label lblPatientName = (Label)e.Item.FindControl("lblPatientName");
                    lblPatientName.Text = ((DataRowView)e.Item.DataItem).Row["Name"].ToString() + " (INACTIVE)";
                    lblPatientName.Font.Italic = true;
                    lblPatientName.ForeColor = System.Drawing.Color.Gray;
                }
            }
            catch
            {
            }
        }
    }

	protected void grdViewPat2_ItemDataBound(object sender, GridItemEventArgs e)
    {
        if (e.Item is GridDataItem)
        {
            e.Item.Style["cursor"] = "pointer";
            try
            {
                int isRestricted = 0;
                if ((bool)((DataRowView)e.Item.DataItem).Row["IsVIPPatient"] || (bool)((DataRowView)e.Item.DataItem).Row["IsRestrictedPatient"])
                {
                    Image confidentialIcon = (Image)e.Item.FindControl("confidentialIcon1");
                    confidentialIcon.Visible = true;
                    isRestricted = 1;

                }

                e.Item.Attributes.Add("onclick", "onRowClick2(this,'" + isRestricted + "')");

                Label lblLastChange = (Label)e.Item.FindControl("lblLastChange");
                lblLastChange.Text = SystemConfig.GetLocalTime(Session["TimeZone"].ToString(), ((DataRowView)e.Item.DataItem).Row["LastChange"].ToString()).ToString();

                int statusID = int.Parse(((DataRowView)e.Item.DataItem).Row["StatusID"].ToString());
                if (statusID == 0)
                {
                    //inactive patient!
                    Label lblPatientName = (Label)e.Item.FindControl("lblPatientName");
                    lblPatientName.Text = ((DataRowView)e.Item.DataItem).Row["Name"].ToString() + " (INACTIVE)";
                    lblPatientName.Font.Italic = true;
                    lblPatientName.ForeColor = System.Drawing.Color.Gray;
                }
            }
            catch
            {
            }
        }
    }

    protected void btnSearch1_Click(object sender, EventArgs e)
    {
        bool valid = (System.Text.RegularExpressions.Regex.Replace(txtSearchPat1LastName.Text, @"[\s]+", " ").Length >= 2
                      || System.Text.RegularExpressions.Regex.Replace(txtSearchPat1FirstName.Text, @"[\s]+", " ").Length >= 2
                      || System.Text.RegularExpressions.Regex.Replace(txtSearchPat1PatientID.Text, @"[\s]+", " ").Length >= 2);

        Regex regexPattern = new Regex(@"([a-zA-Z0-9]{2,})|[a-zA-Z0-9]{1}([\.|\'|\-]{1})");

        bool match = true;
        
        if (!txtSearchPat1LastName.Text.Trim().Equals(string.Empty))
            match = regexPattern.IsMatch(txtSearchPat1LastName.Text);

        if (match && !txtSearchPat1FirstName.Text.Trim().Equals(string.Empty))
            match = regexPattern.IsMatch(txtSearchPat1FirstName.Text);
             
        if ((!valid) || (!match))
        {
            ucMessage.Visible = true;
            ucMessagePanel.Visible = true;
            grdViewPat1.Visible = false;
            grdViewPat1.DataSourceID = null;
        }
        else
        {
            ucMessage.Visible = false;
            ucMessagePanel.Visible = false;
            grdViewPat1.Visible = true;
            txtSearchPat2LastName.Focus();
            cvMergePatients.Enabled = false;
            grdViewPat1.DataSourceID = "Pat1ObjDataSource";
        }
    }
    protected void btnSearch2_Click(object sender, EventArgs e)
    {
        bool valid = (System.Text.RegularExpressions.Regex.Replace(txtSearchPat2LastName.Text, @"[\s]+", " ").Length >= 2
              || System.Text.RegularExpressions.Regex.Replace(txtSearchPat2FirstName.Text, @"[\s]+", " ").Length >= 2
              || System.Text.RegularExpressions.Regex.Replace(txtSearchPat2PatientID.Text, @"[\s]+", " ").Length >= 2);

        Regex regexPattern = new Regex(@"([a-zA-Z0-9]{2,})|[a-zA-Z0-9]{1}([\.|\'|\-]{1})");

        bool match = true;

        if (!txtSearchPat2LastName.Text.Trim().Equals(string.Empty))
            match = regexPattern.IsMatch(txtSearchPat2LastName.Text);

        if (match && !txtSearchPat2FirstName.Text.Trim().Equals(string.Empty))
            match = regexPattern.IsMatch(txtSearchPat2FirstName.Text);

        if ((!valid) || (!match))
        {
            ucMessage.Visible = true;
            ucMessagePanel.Visible = true;
            grdViewPat2.Visible = false;            
            grdViewPat2.DataSourceID = null;
        }
        else
        {
            ucMessage.Visible = false;
            ucMessagePanel.Visible = false;
            grdViewPat2.Visible = true;
            cvMergePatients.Enabled = false;
            grdViewPat2.DataSourceID = "Pat2ObjDataSource";
        }

    }
    protected void btnYes_Click(object sender, EventArgs e)
    {
        //merge 'em!
        try
        {
            Patient.MergePatients(hiddenPat2ID.Value, hiddenPat1ID.Value, base.SessionLicenseID, base.SessionUserID, base.DBID);
            ucMessage.Icon = Controls_Message.MessageType.SUCCESS;
            ucMessage.MessageText = "Patients successfully merged.";
            ucMessage.Visible = true;
            ucMessagePanel.Visible = true;
            txtSearchPat1LastName.Text = txtSearchPat1FirstName.Text = txtSearchPat1PatientID.Text = string.Empty;
            txtSearchPat2LastName.Text = txtSearchPat2FirstName.Text = txtSearchPat2PatientID.Text = string.Empty;
            Session["PATIENTID"] = "";

            base.AuditLogPatientInsert(ePrescribeSvc.AuditAction.PATIENT_MERGE, hiddenPat1ID.Value);
            base.AuditLogPatientInsert(ePrescribeSvc.AuditAction.PATIENT_MERGE, hiddenPat2ID.Value);
        }
        catch(Exception ex)
        {
            Audit.AddException(base.SessionUserID, base.SessionLicenseID, "Error merging patients: " + ex.ToString(), Request.UserIpAddress(), string.Empty, string.Empty, base.DBID);
            ucMessage.Icon = Controls_Message.MessageType.ERROR;
            ucMessage.MessageText = "Error merging patients. Patients not merged.";
            ucMessage.Visible = true;
            ucMessagePanel.Visible = true;

        }

        txtSearchPat1LastName.Focus();
    }
    protected void btnCancel_Click(object sender, EventArgs e)
    {
        cvConfirmChecked.EnableClientScript = false;
    }
    protected void btnMerge1_Click(object sender, EventArgs e)
    {
        cvMergePatients.Enabled = true;
        Page.Validate();
        
        if (Page.IsValid)
        {
          
                    string selectedValue1 = string.Empty;
                    string selectedValue2 = string.Empty;

                    if (grdViewPat1.SelectedValue != null)
                        selectedValue1 = grdViewPat1.SelectedValue.ToString();
                    else
                        selectedValue1 = Request.Form["rdSelectRow1"];

                    if (grdViewPat2.SelectedValue != null)
                        selectedValue2 = grdViewPat2.SelectedValue.ToString();
                    else
                        selectedValue2 = Request.Form["rdSelectRow2"];

                    //set hidden fields so overlay can read them
                    hiddenPat1ID.Value = selectedValue1;
                    hiddenPat2ID.Value = selectedValue2;

                    //load up patient 1
                    Patient pat1 = new Patient(selectedValue1, true, false, Session["LICENSEID"].ToString(), Session["USERID"].ToString(), base.DBID);
                    DataSet pat1DS = pat1.DS;
                    lblPat1Allergies.Text = "None entered";
                    if (pat1DS.Tables["Patient"].Rows.Count > 0)
                    {
                        lblPat1Name.Text = StringUtil.formatDisplayName(
                            pat1DS.Tables["Patient"].Rows[0]["LastName"].ToString(),
                            pat1DS.Tables["Patient"].Rows[0]["FirstName"].ToString(),
                            pat1DS.Tables["Patient"].Rows[0]["MiddleName"].ToString());

                        lblPat1Gender.Text = pat1DS.Tables["Patient"].Rows[0]["SEX"].ToString();
                        lblPat1DOB.Text = pat1DS.Tables["Patient"].Rows[0]["DOB"].ToString();
                        lblPat1ZIP.Text = pat1DS.Tables["Patient"].Rows[0]["Zip"].ToString();
                        lblPat1MRN.Text = pat1DS.Tables["Patient"].Rows[0]["ChartID"].ToString();
                    }

                    if (pat1DS.Tables["PatientAllergy"].Rows.Count > 0)
                    {
                        DataRow[] pat1Allergy = pat1DS.Tables["PatientAllergy"].Select("Active='Y'");
                        if (pat1Allergy.Length > 0)
                        {
                            string allergy1 = string.Empty;
                            foreach (DataRow dr in pat1Allergy)
                            {
                                allergy1 = allergy1 + dr["AllergyName"].ToString() + ",";
                            }
                            if (allergy1.EndsWith(","))
                                allergy1 = allergy1.Substring(0, allergy1.Length - 1);
                            lblPat1Allergies.Text = allergy1;
                        }
                    }

                    DataSet activeMeds1 = Patient.GetPatientActiveMedications(selectedValue1, Session["LICENSEID"].ToString(), Session["USERID"].ToString(), base.DBID);
                    lblPat1Meds.Text = "None entered";
                    if (activeMeds1.Tables["Medications"].Rows.Count > 0)
                    {
                        StringBuilder activeMedications1 = new StringBuilder();
                        foreach (DataRow dr in activeMeds1.Tables["Medications"].Rows)
                        {
                            if (activeMedications1.Length > 0)
                                activeMedications1.Append(", ");
                            activeMedications1.Append(dr["MedicationName"].ToString().Trim());
                        }
                        lblPat1Meds.Text = activeMedications1.ToString();
                    }
                    else if (Convert.ToInt32(pat1DS.Tables["Patient"].Rows[0]["NoActiveMedication"]) == 1) // No Active Medication marked for patient.
                    {
                        lblPat1Meds.Text = "** No Active Medications **";
                    }

                    DataSet activeDx1 = Patient.PatientDiagnosis(selectedValue1, Session["LICENSEID"].ToString(), Session["USERID"].ToString(), base.DBID);
                    lblPat1Problems.Text = "None entered";
                    if (activeDx1.Tables["PatientDiagnosis"].Rows.Count > 0)
                    {
                        StringBuilder activeDiagnosis1 = new StringBuilder();
                        DataRow[] pat1Problems = activeDx1.Tables["PatientDiagnosis"].Select("Active='Y'");
                        if (pat1Problems.Length > 0)
                        {
                            foreach (DataRow dr in pat1Problems)
                            {
                                if (activeDiagnosis1.Length > 0)
                                    activeDiagnosis1.Append(", ");

                                activeDiagnosis1.Append(dr["Description"].ToString().Trim());
                            }
                            lblPat1Problems.Text = activeDiagnosis1.ToString();
                        }
                    }

                    //load up patient 2
                    Patient pat2 = new Patient(selectedValue2, true, false, Session["LICENSEID"].ToString(), Session["USERID"].ToString(), base.DBID);
                    DataSet pat2DS = pat2.DS;
                    lblPat2Allergies.Text = "None entered";
                    if (pat2DS.Tables["Patient"].Rows.Count > 0)
                    {
                        lblPat2Name.Text = StringUtil.formatDisplayName(
                            pat2DS.Tables["Patient"].Rows[0]["LastName"].ToString(),
                            pat2DS.Tables["Patient"].Rows[0]["FirstName"].ToString(),
                            pat2DS.Tables["Patient"].Rows[0]["MiddleName"].ToString());

                        lblPat2Gender.Text = pat2DS.Tables["Patient"].Rows[0]["SEX"].ToString();
                        lblPat2DOB.Text = pat2DS.Tables["Patient"].Rows[0]["DOB"].ToString();
                        lblPat2ZIP.Text = pat2DS.Tables["Patient"].Rows[0]["Zip"].ToString();
                        lblPat2MRN.Text = pat2DS.Tables["Patient"].Rows[0]["ChartID"].ToString();
                    }

                    if (pat2DS.Tables["PatientAllergy"].Rows.Count > 0)
                    {
                        DataRow[] pat2Allergy = pat2DS.Tables["PatientAllergy"].Select("Active='Y'");
                        if (pat2Allergy.Length > 0)
                        {
                            string allergy2 = string.Empty;
                            foreach (DataRow dr in pat2Allergy)
                            {
                                allergy2 = allergy2 + dr["AllergyName"].ToString() + ",";
                            }
                            if (allergy2.EndsWith(","))
                                allergy2 = allergy2.Substring(0, allergy2.Length - 1);
                            lblPat2Allergies.Text = allergy2;
                        }
                    }

                    DataSet activeMeds2 = Patient.GetPatientActiveMedications(selectedValue2, Session["LICENSEID"].ToString(), Session["USERID"].ToString(), base.DBID);
                    lblPat2Meds.Text = "None entered";
                    if (activeMeds2.Tables["Medications"].Rows.Count > 0)
                    {
                        StringBuilder activeMedications2 = new StringBuilder();
                        foreach (DataRow dr in activeMeds2.Tables["Medications"].Rows)
                        {
                            if (activeMedications2.Length > 0)
                                activeMedications2.Append(", ");
                            activeMedications2.Append(dr["MedicationName"].ToString().Trim());
                        }
                        lblPat2Meds.Text = activeMedications2.ToString();
                    }
                    else if (Convert.ToInt32(pat2DS.Tables["Patient"].Rows[0]["NoActiveMedication"]) == 1) // No Active Medication marked for patient.
                    {
                        lblPat2Meds.Text = "** No Active Medications **";
                    }

                    DataSet activeDx2 = Patient.PatientDiagnosis(selectedValue2, Session["LICENSEID"].ToString(), Session["USERID"].ToString(), base.DBID);
                    lblPat2Problems.Text = "None entered";
                    if (activeDx2.Tables["PatientDiagnosis"].Rows.Count > 0)
                    {
                        StringBuilder activeDiagnosis2 = new StringBuilder();
                        DataRow[] pat2Problems = activeDx2.Tables["PatientDiagnosis"].Select("Active='Y'");
                        if (pat2Problems.Length > 0)
                        {
                            foreach (DataRow dr in pat2Problems)
                            {
                                if (activeDiagnosis2.Length > 0)
                                    activeDiagnosis2.Append(", ");

                                activeDiagnosis2.Append(dr["Description"].ToString().Trim());
                            }
                            lblPat2Problems.Text = activeDiagnosis2.ToString();
                        }
                    }

                    cvConfirmChecked.EnableClientScript = true;
                    chkConfirm.Checked = false;
                    modalConfirmationPopup.Show();
                }
    }
    protected void cvMergePatients_ServerValidate(object source, ServerValidateEventArgs args)
    {
        bool validMerge = true;

        string selectedValue1 = string.Empty;
        string selectedValue2 = string.Empty;
        string errorMessage = string.Empty;

        if (grdViewPat1.SelectedValue != null)
            selectedValue1 = grdViewPat1.SelectedValue.ToString();
        else
            selectedValue1 = Request.Form["rdSelectRow1"];

        if (grdViewPat2.SelectedValue != null)
            selectedValue2 = grdViewPat2.SelectedValue.ToString();
        else
            selectedValue2 = Request.Form["rdSelectRow2"];

        if (selectedValue1 == null || selectedValue2 == null)
        {
            validMerge = false;
            errorMessage = "Please select one patient on the left and one patient on the right.";
        }
        else if (selectedValue1 == selectedValue2)
        {
            validMerge = false;
            errorMessage = "You cannot merge the same two patients.";
        }

        if (hiddenPat1Restriction.Value == "1" || hiddenPat2Restriction.Value == "1")
        {
            validMerge = false;
            errorMessage = "Cannot Merge Restricted patients";
        }

            if (!validMerge)
        {
            ucMessage.Icon = Controls_Message.MessageType.ERROR;
            ucMessage.MessageText = errorMessage;
            ucMessage.Visible = true;
            ucMessagePanel.Visible = true;

        }
        else
        {
            ucMessage.MessageText = string.Empty;
            ucMessage.Visible = false;
            ucMessagePanel.Visible = false; 
        }

        args.IsValid = validMerge;
    }

    protected void btnHiddenCancel_Click(object sender, EventArgs e)
    {
        base.AuditLogPatientInsert(ePrescribeSvc.AuditAction.PATIENT_MERGE_CANCELLED, hiddenPat1ID.Value);
        base.AuditLogPatientInsert(ePrescribeSvc.AuditAction.PATIENT_MERGE_CANCELLED, hiddenPat2ID.Value);
    }

    protected void btnBack_OnClick(object sender, EventArgs e)
    {
        Response.Redirect(AngularStringUtil.CreateUrl(Constants.PageNames.SETTINGS));
    }
}



}