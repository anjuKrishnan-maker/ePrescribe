using System;
using System.Linq;
using System.Web;
using System.Web.Services;
using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.Objects.PPTPlus;
using Allscripts.Impact;
using eRxWeb.AppCode;
using eRxWeb.State;
using eRxWeb.AppCode.PptPlusBPL;
using eRxWeb.ServerModel;
using ConfigKeys = Allscripts.Impact.ConfigKeys;
using Allscripts.ePrescribe.Data;
using Newtonsoft.Json;
using System.Web.Script.Serialization;
using Allscripts.ePrescribe.DatabaseSelector;
using System.Web.Http;
using Allscripts.ePrescribe.Shared.Logging;
using eRxWeb.ServerModel.Request;


using System.Collections.Generic;
using System.Threading.Tasks;
using eRxWeb.AppCode.PdmpBPL;
using Allscripts.ePrescribe.Data.CommonComponent;
using Allscripts.Impact.PdmpBPL;
using System.Threading;
using Allscripts.ePrescribe.Data.Model;

namespace eRxWeb.Controllers
{
    public partial class PdmpApiController : ApiController
    {
        private static readonly ILoggerEx logger = LoggerEx.GetLogger();

        [HttpPost]
        public ApiResponse GetPdmpSummary()
        {
            IStateContainer session = new StateContainer(HttpContext.Current.Session);

            var response = new ApiResponse();
            using (var timer = logger.StartTimer("GetPdmpSummary"))
            {
                try
                {
                    response.Payload = PDMP.GetPdmpSummary(session, new Pdmp(), new PdmpServiceBroker(), new CommonComponentData());
                }
                catch (Exception ex)
                {
                    response.ErrorContext = new ErrorContextModel() { Error = ErrorTypeEnum.ServerError, Message = ex.Message };
                }
                timer.Message = $"<Request></Request><Response>{response.ToLogString()}</Response>";
            }
            return response;
        }

        [HttpPost]
        public ApiResponse GetCommonUIUrl()
        {
            using (var timer = logger.StartTimer("GetCommonUIUrl"))
            {
                return new ApiResponse { Payload = ConfigKeys.PdmpCommonUiEndpoint };
            }
        }

        [HttpPost]
        public ApiResponse PDMPDetailsButtonHandler([FromBody]string userEventName)
        {
            var session = new StateContainer(HttpContext.Current.Session);
            var response = new ApiResponse();
            try
            {
                using (var timer = logger.StartTimer("PDMPDetailsButtonHandler"))
                {
                    PDMP.PDMPDetailsButtonHandler(userEventName, session);
                    timer.Message = $"<request>{userEventName}</request>";
                }
            }
            catch (Exception ex)
            {
                var errorMessage = ApiHelper.AuditException(ex.ToString(), session);
                logger.Error("PDMPDetailsButtonHandler Exception: " + ex);

                response.ErrorContext = new ErrorContextModel()
                {
                    Error = ErrorTypeEnum.ServerError,
                    Message = errorMessage
                };
            }
            return response;
        }

        [HttpPost]

        public ApiResponse GetPdmpEnrollmentFormInfo()
        {
            var session = new StateContainer(HttpContext.Current.Session);
            var response = new ApiResponse();
            try
            {
                using (var timer = logger.StartTimer("GetPdmpEnrollmentFormInfo"))
                {
                    response.Payload = new PdmpEnrollment().GetPdmpEnrollmentFormInfo(session);
                    timer.Message = $"<Request></Request><Response>{response.ToLogString()}</Response>";
                }
            }
            catch (Exception ex)
            {
                var errorMessage = ApiHelper.AuditException(ex.ToString(), session);
                logger.Error("GetPdmpEnrollmentFormInfo Exception: " + ex);
                response.ErrorContext = new ErrorContextModel() { Error = ErrorTypeEnum.ServerError, Message = errorMessage };
            }
            return response;
        }

        [HttpPost]

        public ApiResponse PdmpEnrollmentFormSubmit([FromBody] PdmpEnrollmentModel enrollmentData)
        {
            var session = new StateContainer(HttpContext.Current.Session);
            var response = new ApiResponse();
            try
            {
                using (var timer = logger.StartTimer("PdmpEnrollmentFormSubmit"))
                {
                    response.Payload = new PdmpEnrollment().SubmitPdmpEnrollment(session, enrollmentData);
                    timer.Message = $"<Request>{enrollmentData.ToLogString()}</Request><Response>{response.ToLogString()}</Response>";
                }
            }
            catch (Exception ex)
            {
                var errorMessage = ApiHelper.AuditException(ex.ToString(), session);
                logger.Error("PdmpEnrollmentFormSubmit Exception: " + ex);
                response.ErrorContext = new ErrorContextModel() { Error = ErrorTypeEnum.ServerError, Message = errorMessage };
            }
            return response;
        }
    }
}