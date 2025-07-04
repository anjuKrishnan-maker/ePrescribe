using System.Collections.Generic;
using System.Web;
using System.Web.SessionState;
using Allscripts.ePrescribe.Objects;
using eRxWeb.ePrescribeSvc;

namespace eRxWeb.AppCode.Interfaces
{
    public interface ILogin
    {
        List<ServiceAlert> ShowServiceAlerts(string enterpriseClient, string dtCurrentTimeZone);
        List<ServiceAlert> ShowServiceAlertsLicense(string enterpriseClient, string dtCurrentTimeZone, string accountID);

        List<ServiceAlert> ShowServiceAlertsPricing(int pricingStructure, string dtCurrentTimeZone);
        //if there are multiple, the auth token will get properly set on SelectAccountAndSite.aspx
        void SetAuthenticationCookieForShieldUser(ePrescribeSvc.AuthenticatedShieldUser authenticatedShieldUser);
        void SetAccountInfo(AuthenticateAndAuthorizeUserResponse authResponse, ref string redirectUrl, ref string msg);

        void UpdateUserLastLoginDate(AuthenticateAndAuthorizeUserResponse authResponse);

        void SetAuthenticationCookie(string shieldUserName);
    }

    public interface IPageState
    {
        HttpSessionState Session { get; set; }

        HttpRequest Request { get; set; }

        HttpResponse Response { get; set; }
    }
}