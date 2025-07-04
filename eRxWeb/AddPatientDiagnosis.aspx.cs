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
using Allscripts.ePrescribe.Common;
using System.Collections.Generic;
using Allscripts.Impact;
using eRxWeb.State;

namespace eRxWeb
{
public partial class AddPatientDiagnosis : BasePage 
{
    public  bool ShowSnoMedCode {
            get
            {
                bool showSnoMedCode = false;
                if (ViewState["IsPlatinumOrDeluxeUser"] == null)
                {
                    bool value = IsPlatinumOrDeluxeUser(PageState);
                    ViewState["IsPlatinumOrDeluxeUser"] = value;
                    showSnoMedCode = value;
                }
                else
                    showSnoMedCode = (bool)ViewState["IsPlatinumOrDeluxeUser"];

                return showSnoMedCode && SessionLicense.EnterpriseClient.EnableATPDiagnosisSearch;
            }
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            //txtStartDate.Text = DateTime.Now.Date.ToShortDateString();
            radDatePickerStartDate.SelectedDate = DateTime.Now.Date;
            radDatePickerStartDate.MaxDate = DateTime.Now.Date;
            radDatePickerResolvedDate.MaxDate = DateTime.Now.Date;
            if (Session["PATIENTDOB"] != null)
            {
                radDatePickerStartDate.MinDate = Convert.ToDateTime(Session["PATIENTDOB"].ToString());
                radDatePickerResolvedDate.MinDate = Convert.ToDateTime(Session["PATIENTDOB"].ToString());
                rvStartDate.MinimumValue = Session["PATIENTDOB"].ToString();
                rvStartDate.MaximumValue = DateTime.Now.ToString("d");
                rvStartDate.ErrorMessage = string.Format(rvStartDate.ErrorMessage, rvStartDate.MinimumValue, rvStartDate.MaximumValue);
                RangeValResolvedDate.MinimumValue = Session["PATIENTDOB"].ToString();
                RangeValResolvedDate.MaximumValue = DateTime.Now.ToString("d");
                RangeValResolvedDate.ErrorMessage = string.Format(RangeValResolvedDate.ErrorMessage, RangeValResolvedDate.MinimumValue, RangeValResolvedDate.MaximumValue);
                
            }

            if (ShowSnoMedCode)
            {
                grdViewDx.DataSourceID = "ObjDataSourceGetATPDxList";
                aceMed.Enabled = true;
                hiddenFiledIsPlatinumOrDeluxe.Value = "true";
                compareValResolveDate.Enabled = true;
                RequiredFieldValidator2.Enabled = true;
                RangeValResolvedDate.Enabled = true;
            }
            else
            {
                grdViewDx.Columns[3].Visible = false;
                hiddenFiledIsPlatinumOrDeluxe.Value = "false";
                compareValResolveDate.Enabled = false;
                RequiredFieldValidator2.Enabled = false;
                RangeValResolvedDate.Enabled = false;
            }
        }


    }
    public  string getRowHeight()
    {
        if (hiddenFiledIsPlatinumOrDeluxe.Value.Equals("true",StringComparison.OrdinalIgnoreCase))
        {
            return "65";
        }
        else
            return "18";
    }
    private static bool IsPlatinumOrDeluxeUser(IStateContainer state)
    {
        ApplicationLicense applicationLicense = (ApplicationLicense)state["SessionLicense"];

        if (applicationLicense.LicenseDeluxeStatus == Constants.DeluxeFeatureStatus.On ||
                  applicationLicense.LicenseDeluxeStatus == Constants.DeluxeFeatureStatus.AlwaysOn ||
                  applicationLicense.LicenseDeluxeStatus == Constants.DeluxeFeatureStatus.Cancelled)
        {
            return true;
        }
        else
        {
            return false;
        }
 
    }
    protected void Page_LoadComplete(object sender, EventArgs e)
    {       
        ((PhysicianMasterPage)Master).hideTabs();
    }
    protected void btnCancel_ServerClick(object sender, EventArgs e)
    {
        Server.Transfer(Constants.PageNames.PATIENT_DIAGNOSIS);
    }


    protected void btnSave_Click(object sender, EventArgs e)
    {
        bool isSaveSucessfull = false;
        bool isPageValid = false;

        if (ShowSnoMedCode && rdBtnActive.Checked)
        {
            if (RequiredFieldValidator1.IsValid)
            {
                isPageValid = true;
            }
        }

        if (isPageValid || Page.IsValid)   // Added this on AKS oct 18th 200
        {
            string[] cmdArg = Request.Form["rdSelectRow"].ToString().Split(';');
            string selectedValue;
            if (ShowSnoMedCode)
            {
                //SNOMedCTC
                selectedValue = cmdArg[1];
                foreach (GridViewRow row in grdViewDx.Rows)
                {
                    HiddenField hdnTermId = (HiddenField)grdViewDx.Rows[row.RowIndex].FindControl("hdnTermId");
                    if (hdnTermId.Value.ToString() == selectedValue)
                    {
                        grdViewDx.SelectedIndex = row.RowIndex;
                        break;
                    }
                }

            }
            else
            {
                // Legacy ICD9
                selectedValue = cmdArg[0];
                foreach (GridViewRow row in grdViewDx.Rows)
                {
                    if (grdViewDx.Rows[row.RowIndex].Cells[2].Text == selectedValue)
                    {
                        grdViewDx.SelectedIndex = row.RowIndex;
                        break;
                    }
                }
            }
            if (grdViewDx.SelectedIndex > -1)
            {
                HiddenField hdnTermId = (HiddenField)grdViewDx.Rows[grdViewDx.SelectedIndex].FindControl("hdnTermId");
                string termId = hdnTermId.Value.ToString();


                string patientDiagnosisID = System.Guid.NewGuid().ToString();
                string patientId = string.Empty;
                string userid = string.Empty;
                if (Session["PATIENTID"] != null)
                    patientId = Session["PATIENTID"].ToString(); //"{B27E4168-DB3C-435F-9A9D-442349F9CDC4}";
                else
                    Server.Transfer(Constants.PageNames.LOGOUT + "?TIMEOUT=YES");
                if (Session["USERID"] != null)
                    userid = Session["USERID"].ToString();
                else
                    Server.Transfer(Constants.PageNames.LOGOUT + "?TIMEOUT=YES");

                string diagnosisID = grdViewDx.SelectedRow.Cells[2].Text;
                bool active = chkActive.Checked;

                DateTime start = Convert.ToDateTime(radDatePickerStartDate.SelectedDate);  // Start date

                base.AuditLogPatientInsert(ePrescribeSvc.AuditAction.PATIENT_DX_CREATED, patientId);

                if (ShowSnoMedCode)
                {
                    Allscripts.ePrescribe.Data.PatientDiagnosisProvider saveDiagnosis = new Allscripts.ePrescribe.Data.PatientDiagnosisProvider();
                    if (rdBtnActive.Checked)
                    {
                        isSaveSucessfull = saveDiagnosis.SavePatientDiagnosis(patientId, termId, start, base.SessionUserID, base.DBID);
                        if (isSaveSucessfull)
                        {
                            Server.Transfer(Constants.PageNames.PATIENT_DIAGNOSIS);
                        }
                        else
                        {
                            // Show Message that record already exists.

                            ucMessage.Visible = true;
                            divUrgentMessage.Style.Add("display", "block");
                            lblMsg.Style.Add("display", "none");
                            ucMessage.MessageText = string.Format("The selected diagnosis is already active");
                            ucMessage.Icon = Controls_Message.MessageType.ERROR;
                        }
                    }
                    else
                    {
                        
                        DateTime resolvedDate = Convert.ToDateTime(radDatePickerResolvedDate.SelectedDate);  // resolved Date 
                        saveDiagnosis.SavePatientResolvedDiagnosis(patientId, termId, start, base.SessionUserID, resolvedDate, base.DBID);
                        Server.Transfer(Constants.PageNames.PATIENT_DIAGNOSIS);
                    }

                }
                else
                {
                    isSaveSucessfull = Allscripts.Impact.PatientDiagnosis.Save(patientDiagnosisID, patientId, diagnosisID, active, start.ToString(), base.SessionUserID, base.SessionLicenseID, base.DBID);
                    if (isSaveSucessfull)
                    {
                        Server.Transfer(Constants.PageNames.PATIENT_DIAGNOSIS);
                    }
                    else
                    {
                        // Show Message that record already exists.

                        ucMessage.Visible = true;
                        divUrgentMessage.Style.Add("display", "block");
                        lblMsg.Style.Add("display", "none");
                        ucMessage.MessageText = string.Format("The selected diagnosis is already active");
                        ucMessage.Icon = Controls_Message.MessageType.ERROR;
                    }
                }
            }

        } 
 
    }
    protected void btnGo_Click(object sender, EventArgs e)
    {
        divUrgentMessage.Style.Add("display", "none");
        lblMsg.Style.Add("display", "block");
        grdViewDx.PageIndex = 0;
        grdViewDx.DataBind();
    }
    protected void grdViewDx_RowDataBound(object sender, GridViewRowEventArgs e)
    {

        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            e.Row.Style["cursor"] = "pointer";
            e.Row.Attributes.Add("onclick", "onRowClick(this)");
        }
    }


    protected void grdViewDx_RowCreated(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.Pager)
        {
            Table t = e.Row.Cells[0].Controls[0] as Table;
            TableRow r = t.Rows[0];
            foreach (TableCell cell in r.Cells)
            {
                object ctl = cell.Controls[0];
                if (ctl is Label)
                {
                    ((Label)ctl).Text = "Page " + ((Label)ctl).Text;
                    ((Label)ctl).CssClass = "CurrentPage";
                }
                else
                {

                    ((LinkButton)ctl).Text = "[" + ((LinkButton)ctl).Text + "]";

                }
            }
        }
        if (e.Row != null && e.Row.RowType == DataControlRowType.Header)
        {
            foreach (TableCell cell in e.Row.Cells)
            {
                if (cell.HasControls())
                {
                    LinkButton button = cell.Controls[0] as LinkButton;
                    if (button != null)
                    {
                        Image image = new Image();
                        image.ImageUrl = "images/sort-sortable.gif";
                        if (((System.Web.UI.WebControls.GridView)(sender)).SortExpression == button.CommandArgument)
                        {
                            if (((System.Web.UI.WebControls.GridView)(sender)).SortDirection == SortDirection.Ascending)
                                image.ImageUrl = "images/sort-ascending.gif";
                            else
                                image.ImageUrl = "images/sort-Descending.gif";
                        }
                        cell.Controls.Add(image);
                    }
                }
            }
        }
   }

    protected void ObjDataSourceGetATPDxList_Selecting(object sender, ObjectDataSourceSelectingEventArgs e)
    {
        if (e.InputParameters["Phrase"].ToString() == "Enter ICD9 or Partial or Full Dx")
        {
            e.InputParameters["Phrase"] = "";
            //e.Cancel = true;
        }
    }

    protected void ObjDataSourceGetATPDxList_Selected(object sender, ObjectDataSourceStatusEventArgs e)
	{
        DataTable dt = (DataTable)e.ReturnValue;
        if (dt.Rows.Count > 50)
        {
            lblMsg.Text = "Your search returned " + dt.Rows.Count + " results.  Consider refining your search.";
        }
        else
        {
            lblMsg.Text = "";
        }
	}

    protected void ObjDataSourceGetDxList_Selecting(object sender, ObjectDataSourceSelectingEventArgs e)
    {
        if (e.InputParameters["Phrase"].ToString() == "Enter ICD9 or Partial or Full Dx")
        {
            e.InputParameters["Phrase"] = "";
            //e.Cancel = true;
        }
    }

    protected void ObjDataSourceGetDxList_Selected(object sender, ObjectDataSourceStatusEventArgs e)
    {
        DataTable ds = (DataTable)e.ReturnValue;
        if (ds.Rows.Count > 50)
        {
            lblMsg.Text = "Your search returned " + ds.Rows.Count + " results.  Consider refining your search.";
        }
        else
        {
            lblMsg.Text = "";
        }
        ds.Dispose();
    }


}

}