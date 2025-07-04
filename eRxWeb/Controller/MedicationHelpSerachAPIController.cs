using Allscripts.ePrescribe.Common;
using Allscripts.Impact;
using eRxWeb.AppCode;
using eRxWeb.ServerModel;
using eRxWeb.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using Allscripts.ePrescribe.Shared.Logging;
using System.Web.Http;

namespace eRxWeb.Controllers
{
    public partial class MedicationHelpSerachAPIController : ApiController
    {
        private static ILoggerEx logger = LoggerEx.GetLogger();

        [HttpPost]
        public ApiResponse GetSearchUrl()
        {
            using (var timer = logger.StartTimer("GetSearchUrl"))
            {
                IStateContainer pageState = new StateContainer(HttpContext.Current.Session);
                var searchHelp = new MedicationSearchHelp();
                var response = new ApiResponse();
                try
                {
                    searchHelp = getSerachUrl(pageState);
                    response.Payload = searchHelp;
                }
                catch (Exception ex)
                {
                    var errorMessage = ApiHelper.AuditException(ex.ToString(), pageState);
                    logger.Error("GetSearchUrl Exception: " + ex.ToString());

                    response.ErrorContext = new ErrorContextModel()
                    {
                        Error = ErrorTypeEnum.ServerError,
                        Message = errorMessage
                    };
                    timer.Message = string.Format("<ErrorContext>{0}</ErrorContext>", response.ErrorContext.ToLogString());
                }
                timer.Message = string.Format("<Response>{0}</Response>", response.ToLogString());
                return response;
            }
        }

        internal static MedicationSearchHelp getSerachUrl(IStateContainer pageState)
        {
            var searchHelp = new MedicationSearchHelp();
            searchHelp.IsAllowed = false;
            if (AppCode.ApiHelper.SessionLicense(pageState).GetFeatureStatus(Constants.DeluxeFeatureType.iFC) == Constants.DeluxeFeatureStatus.On)
            {
                searchHelp.Url=$"{Constants.PageNames.LIBRARY}?search=";
                searchHelp.IsAllowed = true;
            }
           
            return searchHelp;
        }
    }

}