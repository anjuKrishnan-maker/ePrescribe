using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Http;
using System.Web.Services;
using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.Data;
using Allscripts.ePrescribe.DatabaseSelector;
using Allscripts.ePrescribe.Shared.Logging;
using Allscripts.Impact;
using eRxWeb.AppCode;
using eRxWeb.ServerModel;
using eRxWeb.State;

namespace eRxWeb.Controllers
{
    public partial class AuditViewInfoApiController : ApiController
    {
        private static ILoggerEx logger = LoggerEx.GetLogger();

        [HttpPost]
        public ApiResponse GetAuditLogUserExtendedInfo([FromBody]string auditLogId)
        {
            var session = new StateContainer(HttpContext.Current.Session);

            var response = new ApiResponse();
            try
            {
                string extInfo;
                using (var timer = logger.StartTimer("GetAuditLogUserExtendedInfo"))
                {
                    extInfo = AuditLog.GetAuditLogUserExtendedInfo(auditLogId, (ConnectionStringPointer)session[Constants.SessionVariables.DbId]);
                    timer.Message = $"<AuditLogId>{auditLogId}</AuditLogId><Response>{extInfo}</Response>";
                }
                response.Payload = extInfo;
            }
            catch (Exception ex)
            {
                var errorMessage = ApiHelper.AuditException(ex.ToString(), session);
                logger.Error("GetAuditLogUserExtendedInfo Exception: " + ex.ToString());

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