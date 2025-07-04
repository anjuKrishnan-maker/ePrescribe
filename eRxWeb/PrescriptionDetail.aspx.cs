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
using Allscripts.ePrescribe.Common;

namespace eRxWeb
{
public partial class PrescriptionDetail : BasePage 
{
    protected const string reportID = "PrescriptionDetail";

    protected void Page_Load(object sender, EventArgs e)
    {
     
        if (!Page.IsPostBack)
        {
            DateTime dt = DateTime.Now;
            txtStartDate.Text = dt.ToShortDateString();
            txtEndDate.Text = dt.ToShortDateString();
            txtStartDate.Focus();
            btnPrint.Enabled = false;

            BindGrid();
            //ddlProvider.Items.Insert(0, new ListItem("--Please pick a provider--", "none"));
            //AKS nov 16 for sending a uniqueid for comparion in the SP ,
            ddlProvider.Items.Insert(0, new ListItem("All Providers", "00000000-0000-0000-0000-000000000000")); //Modified this to accomodate the change for UserCode AKS Nov 16
        }

    }
    protected void BindGrid()
    {
        ddlProvider.DataTextField = "ProviderName";
        ddlProvider.DataValueField = "ProviderID";
        string LicenseID = Session["LICENSEID"].ToString() ;
        DataSet drListProvider = Provider.GetProviders(LicenseID, Convert.ToInt32(Session["SITEID"]), base.DBID);
        ddlProvider.DataSource = drListProvider;
        ddlProvider.DataBind();

    }
    protected void Page_LoadComplete(object sender, EventArgs e)
    {
        ((PhysicianMasterPageBlank)Master).hideTabs();
    }
    protected void btnReport_Click(object sender, EventArgs e)
    {
        if (Page.IsValid)
        {
            Server.Transfer(Constants.PageNames.MULTIPLE_VIEW_REPORT 
				+ "?StartDate=" + txtStartDate.Text 
				+ "&EndDate=" + txtEndDate.Text 
				+ "&ProviderID=" + ddlProvider.SelectedValue.ToString() 
				+ "&ProviderName=" + ddlProvider.SelectedItem.Text 
				+ "&ReportID=" + reportID);
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
        DateTime dtcurrentDate = DateTime.Parse(DateTime.Now.ToShortDateString()); //Get current date to validate with..

        if (dtstartDate > dtcurrentDate)
            
            args.IsValid = false;
        }
    protected void cvVerifyEndDate_ServerValidate(object source, ServerValidateEventArgs args)
    {
        //Added this piece of code on Feb 18  1 for handling start date problems with current date..
        DateTime dtEndDate;
        bool checkdate = DateTime.TryParse(txtEndDate.Text.Trim(), out dtEndDate);
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