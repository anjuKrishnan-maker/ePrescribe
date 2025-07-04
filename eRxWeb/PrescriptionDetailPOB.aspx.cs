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
using Microsoft.Reporting.WebForms;
using System.IO;
using Allscripts.Impact.Utilities.Win32;
using Allscripts.Impact.Utilities;
using Allscripts.ePrescribe.Common;

namespace eRxWeb
{
public partial class PrescriptionDetailPOB : BasePage 
{
    protected const string reportID = "PrescriptionDetailPOB";

    protected void Page_Load(object sender, EventArgs e)
    {
     
        if (!Page.IsPostBack)
        {
            DateTime dt = DateTime.Now;
            txtStartDate.Text = dt.ToShortDateString();
            txtEndDate.Text = dt.ToShortDateString();
            txtStartDate.Focus();  
            BindGrid();
            
            //AKS nov 16 for sending a uniqueid for comparion in the SP ,
            ddlProvider.Items.Insert(0, new ListItem("All Providers", "00000000-0000-0000-0000-000000000000")); //Modified this to accomodate the change for UserCode AKS Nov 16
            ddlPOB.Items.Insert(0, new ListItem("All POBs", "00000000-0000-0000-0000-000000000000")); //Modified this to accomodate the change for UserCode AKS Nov 16

            if (Page.Request.QueryString["ReportID"] != null && Page.Request.QueryString["ReportID"] == "SuperPOBDUR")
            {
                Page.Title = lblReportTitle.Text = "Provider DUR Report (Prescribe on Behalf of)";
            }
        }

    }
    protected void BindGrid()
    {
        string LicenseID = Session["LICENSEID"].ToString();
        ddlProvider.DataTextField = "ProviderName";
        ddlProvider.DataValueField = "ProviderID";
        DataSet drListProvider = Provider.GetProviders(base.SessionLicenseID, base.SessionSiteID, base.DBID);
        ddlProvider.DataSource = drListProvider;
        ddlProvider.DataBind();

        ddlPOB.DataTextField = "POBName";
        ddlPOB.DataValueField = "POBID";
        DataSet drListPOB = Provider.GetPOBs(base.SessionLicenseID, base.DBID);
        ddlPOB.DataSource = drListPOB;
        ddlPOB.DataBind();
    }
    protected void Page_LoadComplete(object sender, EventArgs e)
    {
        ((PhysicianMasterPageBlank)Master).hideTabs();
    }
    protected void btnReport_Click(object sender, EventArgs e)
    {
        if (Page.IsValid)
        {
            if (Request.QueryString.Count > 0 && Request.QueryString["ReportID"] != null)
            {
                Server.Transfer(Constants.PageNames.MULTIPLE_VIEW_REPORT + "?StartDate=" + txtStartDate.Text + "&EndDate=" + txtEndDate.Text + "&ProviderID=" + ddlProvider.SelectedValue.ToString() + "&ProviderName=" + ddlProvider.SelectedItem.Text + "&POBID=" + ddlPOB.SelectedValue.ToString() + "&ReportID=" + Request.QueryString["ReportID"] + "&POBName=" +ddlPOB.SelectedItem.Text);
            }
            else
            {
                Server.Transfer(Constants.PageNames.MULTIPLE_VIEW_REPORT + "?StartDate=" + txtStartDate.Text + "&EndDate=" + txtEndDate.Text + "&ProviderID=" + ddlProvider.SelectedValue.ToString() + "&ProviderName=" + ddlProvider.SelectedItem.Text + "&POBID=" + ddlPOB.SelectedValue.ToString() + "&ReportID=" + reportID + "&POBName=" + ddlPOB.SelectedItem.Text);
            }
        }
    }
    protected void btnClose_Click(object sender, EventArgs e)
    {
        Response.Redirect(Constants.PageNames.REDIRECT_TO_ANGULAR + "?componentName=" + Constants.PageNames.REPORTS);
    }

    //Added to verify that start date is less than the current date
    protected void cvVerifyStartDate_ServerValidate(object source, ServerValidateEventArgs args)
    {
		DateTime dtstartDate = DateTime.Parse(txtStartDate.Text);  // Convert to date time for comparision.
		DateTime dtcurrentDate = SystemConfig.GetLocalTime(Session["TimeZone"].ToString(), DateTime.UtcNow); //Get current date to validate with..

		if (dtstartDate > dtcurrentDate)
		{
			args.IsValid = false;
		}
    }

	protected void cvVerifyEndDate_ServerValidate(object source, ServerValidateEventArgs args)
    {
        //Added this piece of code on Feb 18  1 for handling start date problems with current date..
        DateTime dtEndDate;
        bool checkdate = DateTime.TryParse(txtEndDate.Text.Trim(), out dtEndDate);
        if (checkdate)
        {          
            DateTime startDate = DateTime.Parse(txtStartDate.Text);
            if (startDate > dtEndDate)
            {
                args.IsValid = false;
            }
            else
            {
                args.IsValid = true;
            }
        }
        else
            args.IsValid = false;
    }
}


}