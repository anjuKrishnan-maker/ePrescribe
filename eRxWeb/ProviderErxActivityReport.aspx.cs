using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Allscripts.ePrescribe.Common;
using System.Data;
using Allscripts.Impact;

namespace eRxWeb
{
public partial class ProviderErxActivityReport : BasePage
{
    protected const string reportID = "ProvidereRxActivityReport";

    #region Events

    /// <summary>
    /// Page load event. Load all data  controls here.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            DateTime dt = DateTime.Now;
            txtStartDate.Text = dt.ToShortDateString();
            txtEndDate.Text = dt.ToShortDateString();
            txtStartDate.Focus();

            BindGrid();

            //ddlProvider.Items.Insert(0, new ListItem("All Providers", "00000000-0000-0000-0000-000000000000"));
        }
    }

    /// <summary>
    /// Hide all tabs once load is complete as we dont need to show tabs on report page.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_LoadComplete(object sender, EventArgs e)
    {
        ((PhysicianMasterPageBlank)Master).hideTabs();
    }

    /// <summary>
    /// When report show button is clicked this event will be fired.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnReport_Click(object sender, EventArgs e)
    {
        if (Page.IsValid)
        {
            Server.Transfer(Constants.PageNames.MULTIPLE_VIEW_REPORT
                + "?StartDate=" + txtStartDate.Text
                + "&EndDate=" + txtEndDate.Text
                + "&ProviderID=" + ddlProvider.SelectedValue.ToString()
                + "&ProviderName=" + ddlProvider.SelectedItem.Text
                + "&Include=" + ddlInclude.SelectedValue
                + "&ReportID=" + reportID);
        }
    }

    /// <summary>
    /// On close(back) is clicked this event will be fired.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnClose_Click(object sender, EventArgs e)
    {
        Response.Redirect(Constants.PageNames.REDIRECT_TO_ANGULAR + "?componentName=" + Constants.PageNames.REPORTS);
    }

    /// <summary>
    /// Validating the start date if it is not greater that current date.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="args"></param>
    protected void cvVerifyStartDate_ServerValidate(object source, ServerValidateEventArgs args)
    {

        DateTime dtstartDate = DateTime.Parse(txtStartDate.Text);  // Convert to date time for comparision.
        DateTime dtcurrentDate = DateTime.Parse(DateTime.Now.ToShortDateString()); //Get current date to validate with..

        if (dtstartDate > dtcurrentDate)

            args.IsValid = false;
    }

    /// <summary>
    /// Validating end date.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="args"></param>
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

    #endregion

    #region Private Methods

    protected void BindGrid()
    {
        ddlProvider.DataTextField = "ProviderName";
        ddlProvider.DataValueField = "ProviderID";
        string LicenseID = Session["LICENSEID"].ToString();
        DataSet drListProvider = Provider.GetProviders(LicenseID, Convert.ToInt32(Session["SITEID"]), base.DBID);
        ddlProvider.DataSource = drListProvider;
        ddlProvider.DataBind();
    }

    #endregion
}
}