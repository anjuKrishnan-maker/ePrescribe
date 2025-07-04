using System;
using Allscripts.ePrescribe.Common;
using Allscripts.Impact;

namespace eRxWeb
{
    public partial class IDologyFinalError : BasePage
    {
        private String sToken;
        //private InfoPostBack InfoPostBack = new InfoPostBack();
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
                else
                {
                    if (Session["IDologyNumber"] == null)
                    {
                        Session["IDologyNumber"] = "Invalid IDProofing Information";
                    }

                    Provider.UpdateIDProofingFailure(this.SessionUserID, Session["IDologyNumber"].ToString(), base.DBID);
                    sToken = Request.QueryString["U"].ToString();
                    GUIDToken = new Guid(sToken);
                    Audit.AddLogEntryIdology(Request.UserHostName, Page.ToString(), GUIDToken, String.Format("Maximum retry reached, user has to go through Manual ID Proofing"));
                }
            }
        }

        protected void btnRegisterManually_Click(object sender, EventArgs e)
        {
            Response.Redirect(Constants.PageNames.MANUAL_ID_PROOFING_FORM);
        }

        protected void btnRetry_Click(object sender, EventArgs e)
        {
            Response.Redirect(Constants.PageNames.USER_INTERROGATION);
        }
    }
}