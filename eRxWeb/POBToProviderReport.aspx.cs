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
using Allscripts.Impact.Utilities.Win32;
using Allscripts.ePrescribe.Common;

namespace eRxWeb
{
public partial class POBToProviderReport : BasePage
{
    protected const string reportID = "POBToProvider";

    protected void Page_Load(object sender, EventArgs e)
    {

        if (!Page.IsPostBack)
        {
            BindData();
            ddlPOB.Focus();

            if (Page.Request.QueryString["ReportID"] != null && Page.Request.QueryString["ReportID"] == "POBToProvider")
            {
                Page.Title = lblReportTitle.Text = "POB to Provider Association Report";
            }
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
            if (Request.QueryString.Count > 0 && Request.QueryString["ReportID"] != null)
            {
                Server.Transfer(Constants.PageNames.MULTIPLE_VIEW_REPORT + "?providerID=" + ddlProvider.SelectedValue.ToString() + "&pobID=" + ddlPOB.SelectedValue.ToString() + "&pobType=" + ddlPOBType.SelectedValue.ToString() + "&ReportID=" + Request.QueryString["ReportID"] + "&PobName=" + ddlPOB.SelectedItem.Text + "&ProviderName=" + ddlProvider.SelectedItem.Text + "&POBTypeName=" + ddlPOBType.SelectedItem.Text);
            }
            else
            {
                Server.Transfer(Constants.PageNames.MULTIPLE_VIEW_REPORT + "?providerID=" + ddlProvider.SelectedValue.ToString() + "&pobID=" + ddlPOB.SelectedValue.ToString() + "&pobType=" + ddlPOBType.SelectedValue.ToString() + "&ReportID=" + reportID + "&PobName=" + ddlPOB.SelectedItem.Text + "&ProviderName=" + ddlProvider.SelectedItem.Text + "&POBTypeName=" + ddlPOBType.SelectedItem.Text);
                }
        }
    }

    protected void btnClose_Click(object sender, EventArgs e)
    {
        Response.Redirect(Constants.PageNames.REDIRECT_TO_ANGULAR + "?componentName=" + Constants.PageNames.REPORTS);
    }

    private void BindData()
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

        ddlProvider.Items.Insert(0, new ListItem("All Providers", "00000000-0000-0000-0000-000000000000"));
        ddlPOB.Items.Insert(0, new ListItem("All POBs", "00000000-0000-0000-0000-000000000000"));
    }
}
}