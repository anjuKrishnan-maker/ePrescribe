/******************************************************************************
**  Change History
*******************************************************************************
**  Date:      Author:                    Description:
**----------------------------------------------------------------------------- 
**  11/09/2011  Narendra Meena      Removed SSN in edit and Add patient Process
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
using Allscripts.ePrescribe.Common;

namespace eRxWeb
{
public partial class PatientMedHistory : BasePage 
{
    protected string selectedValue = "";
    //protected string firstName = "";
    protected const string reportID = "PatientMedHistory";

    protected void Page_Load(object sender, EventArgs e)
    {

        ObjectDSPatient.SelectParameters["HasVIPPatients"].DefaultValue = base.SessionLicense.hasVIPPatients.ToString();
        if (!Page.IsPostBack)
        {
            //txtSearchPatient.Focus();
            //btnPatientReport.Enabled = false;
            DateTime dt = DateTime.Now;
            txtStartDate.Text = dt.ToShortDateString();
            txtEndDate.Text = dt.ToShortDateString();
            txtStartDate.Focus();
            btnReport.Enabled = false;
            btnGo.CausesValidation = false;


        }
    }
    protected void Page_LoadComplete(object sender, EventArgs e)
    {
        ((PhysicianMasterPageBlank)Master).hideTabs();
    }
    protected void btnReport_Click(object sender, EventArgs e)
    {
        if (Page.IsValid)
        {
            var rbValue = Convert.ToString(Request.Form["rdSelectRow"]).Split('+');

            selectedValue = rbValue[0];
            var patientName = rbValue[1];

            Server.Transfer(Constants.PageNames.MULTIPLE_VIEW_REPORT + "?StartDate=" + txtStartDate.Text + "&EndDate=" + txtEndDate.Text + "&PatientID=" + selectedValue + "&ReportID=" + reportID +"&Name=" + patientName);
        }
    }
    /*protected void btnPatientReport_Click(object sender, EventArgs e)
    {
        selectedValue = Request.Form["rdSelectRow"];

        foreach (GridViewRow row in grdViewPatients.Rows)
        {
            if (grdViewPatients.DataKeys[row.RowIndex].Value.ToString() == selectedValue)
            {
                grdViewPatients.SelectedIndex = row.RowIndex;
                break;
            }
        }
        if (grdViewPatients.SelectedDataKey != null)
        {
            firstName = grdViewPatients.SelectedRow.Cells[1].Text;
            firstName = firstName + " " + grdViewPatients.SelectedRow.Cells[2].Text;
        }

        //string strQueryString = "PatientSnapshot.aspx?PatientID=" + selectedValue + "&PatientName=" + firstName + "&ReportID=" + reportID;
        //string strScript = "<script language=JavaScript> window.open('" + strQueryString + "','Report','toolbar=no,width=720,height=605,left=160,top=60,scrollbars=yes,resizable=yes')</Script>";
        //this.RegisterClientScriptBlock("Window", strScript);
        //JJ Nov 09
        Server.Transfer("MultipleViewReport.aspx?PatientID=" + selectedValue + "&PatientName=" + firstName + "&ReportID=" + reportID);


        //added by JJ on 3 Oct 2k6
        string selectedVal = Request.Form["rdSelectRow"];
        foreach (GridViewRow row in grdViewPatients.Rows)
        {
            if (grdViewPatients.DataKeys[row.RowIndex].Value.ToString() == selectedValue)
            {
                grdViewPatients.SelectedIndex = row.RowIndex;
                break;
            }
        }
        if (grdViewPatients.SelectedDataKey != null)
        {
            grdViewPatients.SelectedRow.CssClass = "";
        } 
    }*/
    protected void btnGo_Click(object sender, EventArgs e)
    {
        Page.Validate();
        if (Page.IsValid)
        {
            if (txtFirstNameSearch.Text.Trim().Length >= 2 || txtLastNameSearch.Text.Trim().Length >= 2 || txtPatientIDSearch.Text.Trim().Length >= 2)
            {
                grdViewPatients.DataSourceID = "ObjectDSPatient";
                ucMessage.Visible = false;
            }
            else
            {
                grdViewPatients.DataSourceID = null;
                ucMessage.Icon = Controls_Message.MessageType.ERROR;
                ucMessage.MessageText = "Please enter at least 2 valid search characters.";
                ucMessage.Visible = true;
            }
        }
    }
    protected void grdViewPatients_RowDataBound(object sender, GridViewRowEventArgs e)
    {

        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            int statusID = int.Parse(grdViewPatients.DataKeys[e.Row.RowIndex][1].ToString());
            if (statusID == 0)
            {
                //inactive patient!
                e.Row.Cells[1].Font.Italic = true;
                e.Row.Cells[1].ForeColor = System.Drawing.Color.Gray;
                e.Row.Cells[2].Font.Italic = true;
                e.Row.Cells[2].ForeColor = System.Drawing.Color.Gray;
                e.Row.Cells[2].Text = e.Row.Cells[2].Text + "  (INACTIVE)";
                e.Row.Cells[3].Font.Italic = true;
                e.Row.Cells[3].ForeColor = System.Drawing.Color.Gray;
                e.Row.Cells[4].Font.Italic = true;
                e.Row.Cells[4].ForeColor = System.Drawing.Color.Gray;
                e.Row.Cells[5].Font.Italic = true;
                e.Row.Cells[5].ForeColor = System.Drawing.Color.Gray;
                e.Row.Cells[6].Font.Italic = true;
                e.Row.Cells[6].ForeColor = System.Drawing.Color.Gray;
            }
                int isRestricted = 0;
                if (Convert.ToBoolean(e.Row.Cells[7].Text) || Convert.ToBoolean(e.Row.Cells[8].Text))
                {
                    Image confidentialIcon = (Image)e.Row.FindControl("confidentialIcon");
                    confidentialIcon.Visible = true;
                    isRestricted = 1;

                }

                e.Row.Style["cursor"] = "pointer";
            e.Row.Attributes.Add("onclick", "onRowClick(this,'" + isRestricted + "')");
        }
    }
    protected void grdViewPatients_RowCreated(object sender, GridViewRowEventArgs e)
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
                        if (grdViewPatients.SortExpression == button.CommandArgument)
                        {
                            if (grdViewPatients.SortDirection == SortDirection.Ascending)
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


    protected void btnCancel_Click(object sender, EventArgs e)
    {
        Response.Redirect(Constants.PageNames.REDIRECT_TO_ANGULAR + "?componentName=" + Constants.PageNames.REPORTS);
    }

    protected void cvVerifyStartDate_ServerValidate(object source, ServerValidateEventArgs args)
    {
                
         //Added this piece of code on Feb 18  1 for handling start date problems with current date..
            DateTime dtstartDate;
            bool checkdate = DateTime.TryParse(txtStartDate.Text, out dtstartDate);
            if (checkdate)
            {
                // Convert to date time for comparision.
                DateTime dtcurrentDate = DateTime.Parse(DateTime.Now.ToShortDateString()); //Get current date to validate with..
                 if (dtstartDate > dtcurrentDate)
                    args.IsValid = false;
            }
            else
            args.IsValid = false;
        }
    protected void cvVerifyEndDate_ServerValidate(object source, ServerValidateEventArgs args)
    {


        //Added this piece of code on Feb 18  1 for handling start date problems with current date..
        DateTime dtEndDate;
        bool checkdate = DateTime.TryParse(txtEndDate.Text, out dtEndDate);
        if (checkdate)
        {
            // Convert to date time for comparision.
            DateTime dtcurrentDate = DateTime.Parse(DateTime.Now.ToShortDateString()); //Get current date to validate with..
            if (dtEndDate > dtcurrentDate)
                args.IsValid = false;
        }
        else
            args.IsValid = false;
    }
}


}