using System;
using Allscripts.Impact;
using Allscripts.ePrescribe.Common;

namespace eRxWeb
{
    public partial class NPIError : BasePage
    {
        int tryCount = 0;
        int maxTry = 2;
        private String sToken;
        private Guid GUIDToken;
        protected void Page_Init(object sender, EventArgs e)
        {
            lnkDefaultStyleSheet.Href += "?version=" + SessionAppVersion;
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (base.CurrentUser == null)
                {
                    Response.Redirect(Constants.PageNames.LOGOUT);
                }

                if (Session["NPICheck_Attempt"] != null)
                {
                    tryCount = 1;
                    int.TryParse(Session["NPICheck_Attempt"].ToString(), out tryCount);
                }
                else
                {
                    tryCount = 1;
                }

                Session["NPICheck_Attempt"] = tryCount;

                if (tryCount >= maxTry)
                {
                    sToken = this.SessionUserID.ToString();
                    GUIDToken = new Guid(sToken);
                    lblErrorTitle.Text = "Error-02";
                    divNPIFailed.Visible = true;
                    divNPIRetry.Visible = false;
                    btnRetry.Enabled = false;
                    Provider.UpdateNPICheckFailure(this.SessionUserID, base.DBID);
                    Audit.AddLogEntryIdology(Request.UserHostName, Page.ToString(), GUIDToken, String.Format("NPI Checking Failed.Max Attempts: {0} - Try Count: {1}. Maximum retry reached, user has to go through Manual NPI Checking,", maxTry, tryCount));
                }
            }
        }

        protected void btnRetry_Click(object sender, EventArgs e)
        {
            Response.Redirect(Constants.PageNames.NPI_CHECK);
        }

        protected void btnRegisterManually_Click(object sender, EventArgs e)
        {
            if (Convert.ToInt32(Session["NPICheck_Attempt"].ToString()) < maxTry)
            {
                Provider.UpdateNPICheckFailure(this.SessionUserID, base.DBID);
            }
            Response.Redirect(Constants.PageNames.MANUAL_NPI_CHECK_FORM);
        }
    }
}