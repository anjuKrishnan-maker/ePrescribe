using System.Collections.Generic;
using System.Web;
using Allscripts.Impact.Interfaces;
using Allscripts.Impact.Utilities;
using TieServiceClient;

namespace eRxWeb.AppCode.Interfaces
{
    public interface ITieUtility
    {
        Dictionary<string, IEnumerable<string>> GetTargetKeys(HttpCookie eRxTieCookie);
        IPlacementResponse GetAdPlacement(string locationId, HttpCookieCollection cookies, ITieUtility iTieUtility, ITIEServiceManager iTieServiceManager, IConfigurationManager iConfigurationManager);
        string ParseAbsoluteUrlForPageName(string urlAbsolutePath);
        string GetHelpPageTieLocationName(string pageName);
        string GetMasterPageTieLocationName(string pageName);
    }
}