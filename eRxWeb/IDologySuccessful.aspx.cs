using System;
using Allscripts.ePrescribe.Common;
using Allscripts.Impact;

namespace eRxWeb
{
    public partial class IDologySuccessful : BasePage
    {
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
                HyperLink2.NavigateUrl = "mailto:" + Allscripts.Impact.ConfigKeys.VeradigmSupportMailAddress;
                if (base.CurrentUser == null)
                {
                    Response.Redirect(Constants.PageNames.LOGOUT);
                }
                else if (Request.QueryString["From"] == "NPICheck")
                {
                    this.Title = "NPI Check Successful";
                    Provider.UpdateNPICheckSuccessful(this.SessionUserID, base.DBID);
                }
                else
                {
                    this.Title = "IDology Successful";
                    Provider.UpdateIDProofingSuccessful(this.SessionUserID, Session["IDologyNumber"].ToString(), base.DBID);
                }

                if (Request.QueryString["From"] != "NPICheck")
                {
                    if (Session["IDNPI"] != null)
                    {
                        int npiTryCnt = 0;
                        int MaxAttempts = 2;
                        string providerFName = string.Empty;
                        string providerLName = string.Empty;
                        string providerNPI = string.Empty;
                        bool npiVerified = false;
                        int validateProviderNPI = 0;
                        sToken = this.SessionUserID.ToString();
                        GUIDToken = new Guid(sToken);
                        if (Session["NPICheck_Attempt"] != null && !string.IsNullOrEmpty(Session["NPICheck_Attempt"].ToString()))
                        {
                            npiTryCnt = 0;
                            int.TryParse(Session["NPICheck_Attempt"].ToString(), out npiTryCnt);
                            npiTryCnt++;
                        }
                        else
                        {
                            npiTryCnt = 1;
                        }

                        Session["NPICheck_Attempt"] = npiTryCnt;
                        Audit.AddLogEntryIdology(Request.UserHostName, Page.ToString(), GUIDToken, String.Format("Max Attempts: {0} - Try Count: {1}", MaxAttempts, npiTryCnt));
                        providerFName = Session["IDFirstName"].ToString();
                        providerLName = Session["IDLastName"].ToString();
                        providerNPI = Session["IDNPI"].ToString();

                        validateProviderNPI = Provider.ValidateProviderNPI(providerFName, providerLName, providerNPI);

                        if (validateProviderNPI > 0)
                        {
                            npiVerified = true;
                        }

                        if (npiVerified)
                        {
                            Provider.UpdateNPICheckSuccessful(this.SessionUserID, base.DBID);
                            Audit.AddLogEntryIdology(Request.UserHostName, Page.ToString(), GUIDToken, String.Format("NPI Check Successful"));
                        }
                        else
                        {
                            Audit.AddLogEntryIdology(Request.UserHostName, Page.ToString(), GUIDToken, String.Format("NPI Check Failed. Max Attempts: {0} - Try Count: {1}", MaxAttempts, npiTryCnt));
                            Response.Redirect(Constants.PageNames.NPI_ERROR + "?From=IDologySuccessful");
                        }
                    }
                }

                if (Session["SSOMode"] != null && Session["SSOMode"].ToString() == Constants.SSOMode.SSOIDPROOFINGMODE)
                {
                    lblCongrats.Text = "You have nearly completed the ID proofing and setup process. <br/> Click Next to read and accept the End User License Agreement, following which you will be logged out.";                  
                }
            }
        }

        protected void btnNext_Click(object sender, EventArgs e)
        {
            Response.Redirect(string.Format("{0}?From=IDologySuccessful&TargetUrl={1}", Constants.PageNames.USER_EULA, Session["TargetUrl"]));
        }
    }
}