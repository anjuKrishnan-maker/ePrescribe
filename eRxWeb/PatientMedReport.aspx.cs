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
using Allscripts.Impact;
using Allscripts.ePrescribe.Common;

namespace eRxWeb
{
public partial class PatientMedReport : BasePage 
{
    protected string selectedValue = "";
    protected const string reportID = "PatientMed";


    protected void Page_Load(object sender, EventArgs e)
    {
        
        if (!Page.IsPostBack)
        {
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
    
    protected void btnGo_Click(object sender, EventArgs e)
    {
       
        grdViewDrug.DataSourceID = "odsDrugList";
        
    }

    protected void grdViewDrug_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
           
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Style["cursor"] = "pointer";
                e.Row.Attributes.Add("onclick", "onRowClick(this)");
            }
        }
    }

    protected void btnReport_Click(object sender, EventArgs e)
    {

       
        if (Page.IsValid)
        {
            selectedValue = Request.Form["rdSelectRow"];
            //string strQueryString = "PatientSnapshot.aspx?StartDate=" + txtStartDate.Text + "&EndDate=" + txtEndDate.Text + "&MedName=" + selectedValue + "&ReportID=" + reportID;
            //string strScript = "<script language=JavaScript> window.open('" + strQueryString + "','Report','toolbar=no,width=720,height=605,left=160,top=60,scrollbars=yes,resizable=yes')</Script>";
            //this.RegisterClientScriptBlock("Window", strScript);
            Server.Transfer(Constants.PageNames.MULTIPLE_VIEW_REPORT + "?StartDate=" + txtStartDate.Text + "&EndDate=" + txtEndDate.Text + "&MedName=" + selectedValue + "&ReportID=" + reportID);

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
            bool checkdate = DateTime.TryParse(txtStartDate.Text.Trim(), out dtstartDate);
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
        bool checkdate = DateTime.TryParse(txtEndDate.Text.Trim() , out dtEndDate);
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