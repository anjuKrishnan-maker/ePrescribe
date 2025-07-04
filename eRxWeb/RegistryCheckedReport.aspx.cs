using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Allscripts.ePrescribe.Common;

namespace eRxWeb
{
    public partial class RegistryCheckedReport : BasePage
    {
        protected const string ReportID = "RegistryCheckedReport";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                DateTime dt = DateTime.Now;
                txtStartDate.Text = dt.ToShortDateString();
                txtEndDate.Text = dt.ToShortDateString();
                txtStartDate.Focus();

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
                Server.Transfer(Constants.PageNames.MULTIPLE_VIEW_REPORT + "?StartDate=" + txtStartDate.Text.Trim() + "&EndDate=" + txtEndDate.Text.Trim() + "&ReportID=" + ReportID.Trim());

            }
        }
        protected void cvverifystartdate_servervalidate(object source, ServerValidateEventArgs args)
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
        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect(Constants.PageNames.REDIRECT_TO_ANGULAR + "?componentName=" + Constants.PageNames.REPORTS);
        }
    }
}