using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.DatabaseSelector;
using Allscripts.Impact;
using eRxWeb.AppCode;
using eRxWeb.State;
using System;
using System.Configuration;
using System.Data;
using Allscripts.ePrescribe.Shared.Logging;
using CompulsoryBasicUtil = eRxWeb.AppCode.CompulsoryBasicUtil;
using ConfigKeys = Allscripts.Impact.ConfigKeys;

namespace eRxWeb
{
    public partial class Spa : BasePage
    {
        public IStateContainer PageState { get; set; }
        private readonly ILoggerEx logger = LoggerEx.GetLogger();
        public bool IsDevelopmentMode { get; } = ConfigurationManager.AppSettings["DevelopmentMode"].AsBool(false);
        public string GaAccountId { get; } = ConfigurationManager.AppSettings["GaAccountId"].As("");
        public bool IsGaEnabled { get; } = ConfigurationManager.AppSettings["IsGaEnabled"].AsBool(false);
        public ApplicationLicense SessionLicense
        {
            get
            {
                if (PageState["SessionLicense"] == null)
                {
                    if (PageState["DBID"] == null)
                        PageState["DBID"] = AppCode.ApiHelper.GetDBID(PageState);

                    PageState["SessionLicense"] = new ApplicationLicense(string.Empty, 0, string.Empty, (ConnectionStringPointer)PageState["DBID"]);
                }

                return (ApplicationLicense)PageState["SessionLicense"];
            }
        }
        protected void Page_Init(object sender, EventArgs e)
        {
            PageState = new StateContainer(Session);
            // lnkDefaultStyleSheet.Href += "?version=" + SessionAppVersion;
            if (!Convert.ToBoolean(Session["HasAcceptedEULA"]) 
                && !Request.RawUrl.ToLower().Contains(Constants.PageNames.HOME_ADDRESS.ToLower()))
            {
                logger.Debug($"User Not Accepted EULA. Redirecting to USER_EULA");
                Response.Redirect(Constants.PageNames.USER_EULA + "?TargetUrl=" + Request.Url.PathAndQuery);
            }
            else if (BillingUtil.ShouldUserFinishAPayment(PageState)
                    && Page.AppRelativeVirtualPath.ToUpper() != "~/" + Constants.PageNames.DELUXE_ACCOUNT_MANAGEMENT.ToUpper()
                    && Page.AppRelativeVirtualPath.ToUpper() != "~/" + Constants.PageNames.DELUXE_FEATURE_SELECTION.ToUpper()
                    && Page.AppRelativeVirtualPath.ToUpper() != "~/" + Constants.PageNames.FEATURE_COMPARISON.ToUpper())
            {
                DataTable dt = DeluxePurchaseManager.GetDeluxeAccountInfo(ApiHelper.GetSessionLicenseID(PageState), ApiHelper.GetDBID(PageState)).Tables[0];
                bool hasPastPaymentRecords = dt.Rows.Count > 0;
                bool hasPaymentProfileID = dt.Rows[0]["PaymentProfileID"] != DBNull.Value;
                bool hasPaidUserCount = dt.Rows[0]["PaidUserCount"] != DBNull.Value;
                
                if (hasPastPaymentRecords && hasPaymentProfileID && hasPaidUserCount)
                {
                    logger.Debug($"User has PaymentProfileID and PaidUsers(LicenseTransaction). Redirecting to DELUXE_ACCOUNT_MANAGEMENT");
                    RedirectPage = Constants.PageNames.DELUXE_ACCOUNT_MANAGEMENT;
                }
                else
                {
                    logger.Debug($"Either PaymentProfileID or PaidUsers(LicenseTransaction), or GetDeluxeAccountInfo Result is null, Redirecting to DELUXE_FEATURE_SELECTION");
                    RedirectPage = Constants.PageNames.DELUXE_FEATURE_SELECTION;
                }
            }

        }

        public string RedirectPage { get; set; }
        

        private bool IsExternalUri(string uri)
        {
            return Uri.IsWellFormedUriString(uri, UriKind.Absolute);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["USERID"] == null)
            {
                Response.Redirect(Constants.PageNames.LOGIN);
            }
            if (!IsPostBack)
            {
                PageState[Constants.SessionVariables.AppComponentAlreadyInitialized] = true;

                var qStringPath = String.IsNullOrWhiteSpace(RedirectPage) ? Request.QueryString["page"].ToQueryStringEncode() : RedirectPage;
                if (!string.IsNullOrEmpty(qStringPath))
                {
                    RedirectPage = qStringPath;
                }
            }
            lnkStyleSheet.Href = getStylePath();
        }


        //Refactor once iframe is removed
        private string getStylePath()
        {
            string stylePath = "";
            if (Session["Theme"] == null)
            {
                stylePath = "Style/Style.css";
            }
            else
            {
                stylePath = Session["Theme"].ToString();
                stylePath += "?v=" + ApiHelper.SessionAppVersion(PageState); ;

            }
            return stylePath;
        }
        public string Version
        {
            get
            {
                return ApiHelper.SessionAppVersion(PageState); ;
            }
        }
    }
}