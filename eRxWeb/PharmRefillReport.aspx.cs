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
    public partial class PharmRefillReport : BasePage
    {
        protected const string REPORT_ID = "PharmacyRefillReport";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                LoadProviders();
                ddlProvider.Focus();

            }
        }
        protected void Page_LoadComplete(object sender, EventArgs e)
        {
            ((PhysicianMasterPageBlank)Master).hideTabs();
        }
        protected void btnReport_Click(object sender, EventArgs e)
        {
            string toString = string.Empty;
            if (Request.QueryString["To"] != null)
            {
                toString = "&To=" + Constants.PageNames.DOC_REFILL_MENU.ToLower();
            }

            Server.Transfer(Constants.PageNames.MULTIPLE_VIEW_REPORT + "?ProviderID=" + ddlProvider.SelectedValue.ToString() + "&ProviderName=" + ddlProvider.SelectedItem.Text + "&ReportID=" + REPORT_ID + toString);
        }
        protected void btnClose_Click(object sender, EventArgs e)
        {
            Response.Redirect(Constants.PageNames.REDIRECT_TO_ANGULAR + "?componentName=" + Constants.PageNames.REPORTS);
        }
        protected void LoadProviders()
        {           
            ddlProvider.DataTextField = "ProviderName";
            ddlProvider.DataValueField = "ProviderID";
            string licenseID = PageState.GetStringOrEmpty("LICENSEID");                   
            int siteID = PageState.GetInt("SITEID",-1);
            ddlProvider.DataSource = Provider.GetActiveProviders(licenseID, siteID, base.DBID);
            ddlProvider.DataBind();
            //Insert the all providers option
            ddlProvider.Items.Insert(0, new ListItem("All Providers", Guid.Empty.ToString()));
        }      
    }

}