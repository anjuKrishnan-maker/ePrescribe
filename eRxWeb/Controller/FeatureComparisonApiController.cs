using eRxWeb.AppCode;
using eRxWeb.ServerModel;
using eRxWeb.State;
using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using Allscripts.ePrescribe.Shared.Logging;

namespace eRxWeb.Controller
{
    public partial class FeatureComparisonApiController : ApiController
    {
        private static ILoggerEx logger = LoggerEx.GetLogger();
        [HttpPost]
        public ApiResponse GetFeatureComparisonImageSource()
        {
            IStateContainer session = new StateContainer(HttpContext.Current.Session);
            var response = new ApiResponse();
            try
            {
                string imageUrl = "";
                using (var timer = logger.StartTimer("GetFeatureComparisonImageSource"))
                {
                    if (ApiHelper.SessionLicense(session).IsEnterpriseCompulsoryBasicOrForceCompulsoryBasic())
                    {
                        imageUrl = @"images/FeatureComparison/FeatureComparisonGrid.PNG";
                    }
                    else
                    {
                        imageUrl = @"images/FeatureComparison/FeatureComparisonGrid.PNG";
                    }
                    timer.Message = $"<Response>{imageUrl}</Response>";
                }
                response.Payload = imageUrl;
            }
            catch (Exception ex)
            {
                var errorMessage = ApiHelper.AuditException(ex.ToString(), session);
                logger.Error("GetFeatureComparisonImageSource Exception: " + ex.ToString());

                response.ErrorContext = new ErrorContextModel()
                {
                    Error = ErrorTypeEnum.ServerError,
                    Message = errorMessage
                };
            }
            return response;
        }
    }
}