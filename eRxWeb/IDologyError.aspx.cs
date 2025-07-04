using System;
using Allscripts.ePrescribe.Common;
using Allscripts.Impact;
using eRxWeb.AppCode;
using Allscripts.ePrescribe.Objects;
using Provider = Allscripts.Impact.Provider;

namespace eRxWeb
{
    public partial class IDologyError : BasePage
    {
        private String sToken;
        int tryCount = 0;
        int maxTry = 2;
        private Guid GUIDToken;
        protected void Page_Init(object sender, EventArgs e)
        {
            lnkDefaultStyleSheet.Href += "?version=" + SessionAppVersion;
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (base.CurrentUser == null)
            {
                Response.Redirect(Constants.PageNames.LOGOUT);
            }

            if (Session["TryCount"] != null)
            {
                tryCount = 1;
                int.TryParse(Session["TryCount"].ToString(), out tryCount);
                Session["TryCount"] = tryCount;
            }

            if (tryCount >= maxTry)
            {
                string RedirectPage = Constants.PageNames.IDOLOGY_FINAL_ERROR;
                sToken = Request.QueryString["U"].ToString();
                GUIDToken = new Guid(sToken);
                //Update in DB with status = 1 (Verified)
                Allscripts.ePrescribe.Data.Provider.UpdateHomeAddressCheckStatus(
                                                                                    PageState.GetStringOrEmpty(Constants.SessionVariables.UserId),
                                                                                    Convert.ToInt32(HOME_ADDRESS_CHECK_STATUS.LOCKED),
                                                                                    DBID
                                                                                );
                Response.Redirect(String.Format("{0}?U={1}", RedirectPage, GUIDToken.ToString()));
                // Response.Redirect(Constants.PageNames.IDOLOGY_FINAL_ERROR);
            }

            String ErrorHeader;
            String ErrorDetail;
            String CallForSupport = "Please contact support via the information at the bottom of this screen.";

            int i;
            try
            {
                i = (int)Session["ErrorNumber"];
            }
            catch
            {
                i = 1020;
            }

            IDologyClass IDVerify = new IDologyClass(new AppConfig(), new AppCode.ConfigKeys());
            if (Session["ShieldLoginID"] != null)
                IDVerify.GetIDologyErrorDetails(CallForSupport, i, out ErrorHeader, out ErrorDetail, Session["ShieldLoginID"].ToString());
            else
                IDVerify.GetIDologyErrorDetails(CallForSupport, i, out ErrorHeader, out ErrorDetail, null);

            if (Session["PartnerGUID"] != null)
            {
                pnlRegisterManually.Visible = false;
                pnlRegisterManually.Enabled = false;
                try
                {
                    sToken = Request.QueryString["U"].ToString();
                }
                catch (Exception)
                {
                    sToken = String.Empty;
                }

                //Get InfoPostBack Address, MessageID, PartnerID, & LicenseID
                //Create Post Message
                //InfoPostBack.SendInfoMessagePost(sToken, Request.UserHostName, Page.ToString(), sMessages);
            }
        }

        protected void btnRegisterManually_Click(object sender, EventArgs e)
        {
            if (Session["IDologyNumber"] == null)
            {
                Session["IDologyNumber"] = "Invalid IDProofing Information";
            }

            Provider.UpdateIDProofingFailure(this.SessionUserID, Session["IDologyNumber"].ToString(), base.DBID);
            Response.Redirect(Constants.PageNames.MANUAL_ID_PROOFING_FORM);
        }

        protected void btnRetry_Click(object sender, EventArgs e)
        {
            Response.Redirect(Constants.PageNames.IDPROOFING_REQUIRED_INFO);
        }
    }
}