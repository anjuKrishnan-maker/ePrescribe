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
public partial class PatientAddReport : BasePage
{
    protected const string reportID = "PatientAdd";

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            DateTime dt = DateTime.Now;
            txtStartDate.Text = dt.ToShortDateString();
            txtEndDate.Text = dt.ToShortDateString();
            txtStartDate.Focus();
            btnPrint.Enabled = false;
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
            Server.Transfer(Constants.PageNames.MULTIPLE_VIEW_REPORT + "?StartDate=" + txtStartDate.Text + "&EndDate=" + txtEndDate.Text + "&ReportID=" + reportID);
        }
    }

    protected void btnClose_Click(object sender, EventArgs e)
    {
        Response.Redirect(Constants.PageNames.REDIRECT_TO_ANGULAR + "?componentName=" + Constants.PageNames.REPORTS);
    }

    protected void cvVerifyStartDate_ServerValidate(object source, ServerValidateEventArgs args)
    {

        DateTime dtstartDate = DateTime.Parse(txtStartDate.Text);  
        DateTime dtcurrentDate = DateTime.Parse(DateTime.Now.ToShortDateString()); 

        if (dtstartDate > dtcurrentDate)
            args.IsValid = false;
    }

    protected void cvVerifyEndDate_ServerValidate(object source, ServerValidateEventArgs args)
    {
        DateTime dtEndDate;
        bool checkdate = DateTime.TryParse(txtEndDate.Text.Trim(), out dtEndDate);
        if (checkdate)
        {
            DateTime dtcurrentDate = DateTime.Parse(DateTime.Now.ToShortDateString()); 
            if (dtEndDate > dtcurrentDate)
                args.IsValid = false;
        }
        else
            args.IsValid = false;
    }
}

}