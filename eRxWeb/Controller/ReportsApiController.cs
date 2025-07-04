using eRxWeb.ServerModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using eRxWeb.AppCode.Reports;
using Allscripts.ePrescribe.Shared.Logging;
using eRxWeb.State;
using eRxWeb.AppCode;
using Allscripts.ePrescribe.Common;
using eRxWeb.ServerModel.Reports;
using Allscripts.Impact;
using System.Data;

namespace eRxWeb.Controller
{
    public class ReportsApiController : ApiController
    {
        private static ILoggerEx logger = LoggerEx.GetLogger();

        [HttpPost]
        public ApiResponse GetReportsList()
        {
            var response = new ApiResponse();
            IStateContainer requestState = new StateContainer(HttpContext.Current.Session);

            try
            {
                ApplicationLicense sessionLicense = requestState.Cast(Constants.SessionVariables.SessionLicense, new ApplicationLicense());
                Constants.DeluxeFeatureStatus gCodeReportDeluxeFeatureStatus = Constants.DeluxeFeatureStatus.On;
                if(sessionLicense != null)
                {
                    gCodeReportDeluxeFeatureStatus = sessionLicense.GetFeatureStatus(Constants.DeluxeFeatureType.GCodeReport);
                }
                response.Payload = new ReportMenuUtil().GenerateReportMenu(gCodeReportDeluxeFeatureStatus);
            }
            catch (Exception ex)
            {
                string exceptionString = ex.ToString();
                var errorMessage = ApiHelper.AuditException(exceptionString, requestState);
                logger.Error("GetReportsList Exception: " + exceptionString);

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