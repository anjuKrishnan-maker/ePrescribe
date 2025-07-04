using Allscripts.ePrescribe.Common;
using Allscripts.Impact;
using eRxWeb.AppCode;
using eRxWeb.State;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using TieServiceClient;
using Allscripts.ePrescribe.Shared.Logging;
using System.Web.Http;
using eRxWeb.ServerModel;

namespace eRxWeb.Controllers
{
    public partial class LogRxApiController : ApiController
    {

        private static ILoggerEx logger = LoggerEx.GetLogger();

        [HttpPost]
        public ApiResponse GetLogRxContent(string pageName)
        {
            using (var timer = logger.StartTimer("GetLogRxContent"))
            {
                IStateContainer pageState = new StateContainer(HttpContext.Current.Session);
                var response = new ApiResponse();

                response.Payload = GetAdCotent(pageState, pageName);// IEServiceManager.TIEPlacementResponse(pageName, targetingKeys);
                timer.Message = $"<pageName>{pageName}</pageName><AdContent>{response.Payload}</AdContent>";

                // IPlacementResponse placementResponse = null;
                return response;
            }
        }

        private static string GetAdCotent(IStateContainer pageState, string pageName)
        {


            IPlacementResponse placementResponse;
            string returmValue = string.Empty;


            if (ApiHelper.IsTieAdPlacementEnabled && pageState.GetBooleanOrFalse(Constants.SessionVariables.ShouldShowAds))
            {
                placementResponse = TieUtility.GetAdForMasterPage($"/{pageName}", HttpContext.Current.Request.Cookies, new TieUtility());
                returmValue = placementResponse.Content["right_hand"];
            }
            return returmValue;
        }
        public static string GetPageName(string pageNames)
        {

            string[] pageNameWithExtn = pageNames.Split('.');
            string pageName = pageNameWithExtn[0].ToLower();
            return pageName;
        }
    }
}