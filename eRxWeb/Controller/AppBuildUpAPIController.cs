using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using Allscripts.ePrescribe.DatabaseSelector;
using Allscripts.Impact.Ilearn;
using eRxWeb.State;
using Allscripts.ePrescribe.Common;
using Allscripts.Impact;
using System.Data;
using System.Text;
using eRxWeb.AppCode;
using System.Configuration;
using eRxWeb.ServerModel;
using Allscripts.ePrescribe.Shared.Logging;
using System.Web.Http;

namespace eRxWeb.Controller
{
    public partial class AppBuildUpAPIController : ApiController
    {
        private static ILoggerEx logger = LoggerEx.GetLogger();

        [HttpPost]
        public ApiResponse GetISiteInfo()
        {
            IStateContainer requestState = new StateContainer(HttpContext.Current.Session);
            var response = new ApiResponse();
            try
            {
                SiteInfo sInfo = new SiteInfo();
                using (var timer = logger.StartTimer("GetISiteInfo"))
                {
                    sInfo = GetSiteInfo(requestState);
                    timer.Message = $"<Response>{sInfo.ToLogString()}</Response>";
                }
                response.Payload = sInfo;
            }
            catch (Exception ex)
            {
                var errorMessage = ApiHelper.AuditException(ex.ToString(), requestState);
                logger.Error("GetISiteInfo Exception: " + ex.ToString());

                response.ErrorContext = new ErrorContextModel()
                {
                    Error = ErrorTypeEnum.ServerError,
                    Message = errorMessage
                };
            }
            return response;

        }

        internal static SiteInfo GetSiteInfo(IStateContainer session)
        {
            var sInfo = new SiteInfo();
            var usr = GetUserDetails(session);
            var lastLoginDateUtc = session.Cast<DateTime>("LastLoginDateUTC", DateTime.MinValue);
            if (lastLoginDateUtc != DateTime.MinValue && session["CURRENT_SITE_ZONE"] != null)
            {
                // Last login Details
                DateTime other = DateTime.SpecifyKind(lastLoginDateUtc, DateTimeKind.Utc);
                var date = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(other,
                    session["CURRENT_SITE_ZONE"].ToString());
                usr.Login.lastLogin = sInfo.LastLoginDate = date.ToString("MMM dd, yyyy - hh:mm ") + date.ToString("tt").ToLower();
            }

            sInfo.shieldStatus = session.GetBooleanOrFalse("IsLicenseShieldEnabled")
                ? "shield-enabled"
                : "shiedl-disabled";
            string siteId = session.GetString("siteID", "1");
            if (string.IsNullOrEmpty(siteId))
            {
                siteId = "1";
            }

            setSiteInfo(sInfo, session.GetStringOrEmpty("LICENSEID"), Convert.ToInt32(siteId), session);
            sInfo.IsMultipleSite = session.GetBooleanOrFalse("MULTIPLESITES");
            sInfo.user = usr;
            sInfo.IsTieAdsEnabled = ApiHelper.IsTieAdPlacementEnabled && session.GetBooleanOrFalse(Constants.SessionVariables.ShouldShowAds);
            sInfo.IsRestrictedMenu = BillingUtil.ShouldUserFinishAPayment(session);
            return sInfo;
        }

        [HttpPost]
        public ApiResponse GetAppInfo()
        {
            IStateContainer RequestState;
            RequestState = new StateContainer(HttpContext.Current.Session);
            var response = new ApiResponse();
            try
            {
                AppInfo info = new AppInfo();
                using (var timer = logger.StartTimer("GetAppInfo"))
                {
                    info.VersionNumber = ApiHelper.SessionAppVersion(RequestState);
                    timer.Message = info.VersionNumber;
                }
                response.Payload = info;
            }
            catch (Exception ex)
            {
                var errorMessage = ApiHelper.AuditException(ex.ToString(), RequestState);
                logger.Error("GetAppInfo Exception: " + ex.ToString());

                response.ErrorContext = new ErrorContextModel()
                {
                    Error = ErrorTypeEnum.ServerError,
                    Message = errorMessage
                };
            }
            return response;

        }

        private static User GetUserDetails(IStateContainer requestState)
        {
            ePrescribeSvc.GetUserResponse getUserRes = EPSBroker.GetRxUser(
                    ePrescribeSvc.ValueType.UserGUID,
                   requestState.GetStringOrEmpty("USERID"),
                    requestState.GetStringOrEmpty("LICENSEID"),
                    requestState.GetStringOrEmpty("USERID"),
                   requestState.GetStringOrEmpty("LICENSEID"),
                    requestState.Cast<ConnectionStringPointer>("DBID", ConnectionStringPointer.UTILITIES_DB));

            ePrescribeSvc.RxUser rxUser = getUserRes.RxUser;

            User u = new User();
            bool isAdmin = false;
            u.UserType = getUserType(rxUser.AppRoles, ref isAdmin);
            u.IsAdmin = isAdmin;

            u.Login.FirstName = rxUser.FirstName;
            u.Login.LastName = rxUser.LastName;
            u.Login.MI = rxUser.MiddleInitial;
            u.Login.Email = u.Login.ConfirmEmail = rxUser.Email;
            u.Login.IsActive = rxUser.Active;
            u.Login.LoginId = rxUser.ShieldUserName;

            u.UserCredential.Title = rxUser.Title;
            u.UserCredential.Suffix = rxUser.Suffix;
            u.UserCredential.GenericLicense = rxUser.GenericLicense;
            u.UserCredential.Spaciality1 = rxUser.SpecialtyCode1;
            u.UserCredential.Spaciality2 = rxUser.SpecialtyCode2;
            u.UserCredential.NPI = rxUser.NPI;
            return u;
        }

        private static void setSiteInfo(SiteInfo info, string licenseID, int siteID, IStateContainer pageState)
        {
            StringBuilder siteTooltipText = new StringBuilder();
            if (licenseID != null && siteID != 0)
            {
                pageState["SiteID"] = siteID;
                if (pageState["SITE_TOOLTIP_TEXT"] == null)
                {
                    pageState["SITE_TOOLTIP_TEXT"] = getSiteTooltipText(ApiHelper.GetSessionLicenseID(pageState), siteID);
                }

                string siteHoverText = pageState.GetStringOrEmpty("SITE_TOOLTIP_TEXT");
                if (pageState["AHSAccountID"] != null)
                {
                    if (Convert.ToBoolean(ConfigurationManager.AppSettings["IsDeluxeStatusDisplayEnabled"]))
                    {
                        if (ApiHelper.SessionLicense(pageState).DeluxeFeatureStatusDisplay.ToLower().Contains("platinum"))
                        {
                            siteTooltipText.Append(string.Format("{0}.\n {1}, {2}, {3}", siteHoverText,
                                pageState["AHSAccountID"].ToString(), Convert.ToInt16(ApiHelper.GetDBID(pageState)).ToString(), ApiHelper.SessionLicense(pageState).DeluxeFeatureStatusDisplay));
                        }
                        else
                        {
                            siteTooltipText.Append(string.Format("{0}.\n {1}, {2}, {3}", siteHoverText, pageState["AHSAccountID"].ToString(), Convert.ToInt16(ApiHelper.GetDBID(pageState)).ToString().ToString(), ApiHelper.SessionLicense(pageState).DeluxePricingStructureDisplay));
                        }
                    }
                    else
                    {
                        siteTooltipText.Append(string.Format("{0}.\n {1}, {2}", siteHoverText, pageState["AHSAccountID"].ToString(), Convert.ToInt16(ApiHelper.GetDBID(pageState)).ToString()));
                    }
                }

                ePrescribeSvc.LicenseSite licenseSite = EPSBroker.GetLicenseSiteByID(licenseID, siteID);


                info.siteName = licenseSite.SiteName;
            }

            info.siteDetails = siteTooltipText.ToString();
            info.SelectSiteUrl = $"/{Constants.PageNames.SELECT_ACCOUNT_AND_SITE}?TargetURL={Constants.PageNames.SPA_LANDING}";
        }

        private static Constants.UserCategory getUserType(string[] appRoles, ref bool isAdmin)
        {
            Constants.UserCategory userType = Constants.UserCategory.GENERAL_USER;

            foreach (string appRole in appRoles)
            {
                if (appRole.ToLower() == "admin")
                {
                    //set admin flag
                    isAdmin = true;
                }
                else
                {
                    if (appRole.ToLower() == "provider")
                    {
                        userType = Constants.UserCategory.PROVIDER;
                    }
                    if (appRole.ToLower() == "physician assistant")
                    {
                        userType = Constants.UserCategory.PHYSICIAN_ASSISTANT;
                    }
                    if (appRole.ToLower() == "physician assistant supervised")
                    {
                        userType = Constants.UserCategory.PHYSICIAN_ASSISTANT_SUPERVISED;
                    }
                    if (appRole.ToLower() == "pob - no review")
                    {
                        userType = Constants.UserCategory.POB_SUPER;
                    }
                    if (appRole.ToLower() == "pob - some review")
                    {
                        userType = Constants.UserCategory.POB_REGULAR;
                    }
                    if (appRole.ToLower() == "pob - all review")
                    {
                        userType = Constants.UserCategory.POB_LIMITED;
                    }
                    if (appRole.ToLower() == "general user")
                    {
                        userType = Constants.UserCategory.GENERAL_USER;
                    }
                }
            }

            return userType;
        }

        private static string getSiteTooltipText(string licenseID, int siteID)
        {
            StringBuilder siteTooltipText = new StringBuilder();

            if (licenseID != null && siteID != 0)
            {
                ePrescribeSvc.LicenseSite licenseSite = EPSBroker.GetLicenseSiteByID(licenseID, siteID);
                siteTooltipText.Append(licenseSite.Address1 + "\n");

                if (licenseSite.Address2 != null && licenseSite.Address2 != "")
                {
                    siteTooltipText.Append(licenseSite.Address2 + "\n");
                }

                siteTooltipText.Append(licenseSite.City + ", ");
                siteTooltipText.Append(licenseSite.State + " ");
                siteTooltipText.Append(licenseSite.ZIPCode + "\n");
                siteTooltipText.Append(Allscripts.Impact.Utilities.StringHelper.FormatPhone(licenseSite.PhoneAreaCode, licenseSite.PhoneNumber));
            }

            return siteTooltipText.ToString();
        }
    }
}