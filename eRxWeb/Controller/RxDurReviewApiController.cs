using System;
using System.Web.Http;
using Allscripts.ePrescribe.Shared.Logging;
using eRxWeb.AppCode;
using eRxWeb.ServerModel;
using eRxWeb.AppCode.DurBPL;
using eRxWeb.AppCode.DurBPL.RequestModel;
using Allscripts.ePrescribe.Medispan.Clinical.Model.Response;
using IgnoreReason= Allscripts.Impact.IgnoreReason;
using static Allscripts.ePrescribe.Common.Constants;
using eRxWeb.State;
using System.Web;
using System.Web.SessionState;

namespace eRxWeb.Controller
{
    public class RxDurReviewApiController : ApiController
    {
        IDur serviceDur;
        private static readonly ILoggerEx logger = LoggerEx.GetLogger();
        public RxDurReviewApiController(DurType serviceType)
        {
            serviceDur = new DurFactory(serviceType).GetDurObject();
        }

        [HttpGet]
        public ApiResponse DurWarnings(IeRxWebAppCodeWrapper codeWarper)
        {
            using (var timer = logger.StartTimer("GetDurWarnings"))
            {
                var response = new ApiResponse();
                try
                {
                    response.Payload = serviceDur.GetDurWarnings(codeWarper);
                    timer.Message = $"<Response>{response.ToLogString()}</Response>";
                }
                catch (Exception ex)
                {
                    var errorMessage = ApiHelper.AuditException(ex.ToString(), serviceDur.GetPageState());
                    logger.Error("GetDurWarnings Exception: " + ex.ToString());
                    response.ErrorContext = new ErrorContextModel()
                    {
                        Error = ErrorTypeEnum.ServerError,
                        Message = errorMessage
                    };
                    timer.Message = string.Format("<ErrorContext>{0}</ErrorContext>", response.ErrorContext.ToLogString());
                }
                return response;
            }
        }

        [HttpPost]
        public ApiResponse DurWarnings(DURCheckResponse  durWarnings)
        {
            using (var timer = logger.StartTimer("GetDurWarnings"))
            {
                var response = new ApiResponse();
                try
                {
                     serviceDur.SetDurWarnings(durWarnings);
                    response.Payload = null;
                    timer.Message = $"<Response>{response.ToLogString()}</Response>";
                }
                catch (Exception ex)
                {
                    var errorMessage = ApiHelper.AuditException(ex.ToString(), serviceDur.GetPageState());
                    logger.Error("GetDurWarnings Exception: " + ex.ToString());
                    response.ErrorContext = new ErrorContextModel()
                    {
                        Error = ErrorTypeEnum.ServerError,
                        Message = errorMessage
                    };
                    timer.Message = string.Format("<ErrorContext>{0}</ErrorContext>", response.ErrorContext.ToLogString());
                }
                return response;
            }
        }

        [HttpGet]
        public ApiResponse DurRxList(IeRxWebAppCodeWrapper durWrapper)
        {
            using (var timer = logger.StartTimer("DurRxList"))
            {
                var response = new ApiResponse();
                try
                {
                    response.Payload = serviceDur.GetDurRxList(durWrapper);
                }
                catch (Exception ex)
                {
                    var errorMessage = ApiHelper.AuditException(ex.ToString(), serviceDur.GetPageState());
                    logger.Error("DurRxList Exception: " + ex.ToString());
                    response.ErrorContext = new ErrorContextModel()
                    {
                        Error = ErrorTypeEnum.ServerError,
                        Message = errorMessage
                    };
                    timer.Message = string.Format("<ErrorContext>{0}</ErrorContext>", response.ErrorContext.ToLogString());
                }
                return response;
            }
        }

        [HttpPost]

        public ApiResponse CSMedRefillRequestNotAllowedOnPrintRefill(ICSMedRefillNotAllowedPrintRefillRequest requestOnPrintRefillRequest, IImpactDurWraper impactDur)
        {
            using (var timer = logger.StartTimer(string.Format("PostCSMedRefillRequestNotAllowedOnPrintRefill-request:", requestOnPrintRefillRequest.ToLogString())))// todo: checked if ok to log request with instrumentation logging
            {
                var response = new ApiResponse();
                try
                {
                    response.Payload = serviceDur.CSMedRefillRequestNotAllowedOnPrintRefillRequest(requestOnPrintRefillRequest, impactDur);
                }
                catch (Exception ex)
                {
                    var errorMessage = ApiHelper.AuditException(ex.ToString(), serviceDur.GetPageState());
                    logger.Error("PostCSMedRefillRequestNotAllowedOnPrintRefill Exception: " + ex.ToString());

                    response.ErrorContext = new ErrorContextModel()
                    {
                        Error = ErrorTypeEnum.ServerError,
                        Message = errorMessage
                    };
                    timer.Message = string.Format("<ErrorContext>{0}</ErrorContext>", response.ErrorContext.ToLogString());
                }
                timer.Message = string.Format("<Request>{0}</Request><Response>{1}</Response>", requestOnPrintRefillRequest.ToLogString(), response.ToLogString());
                return response;
            }
        }
        [HttpGet]
        public ApiResponse DurRedirectDetails()
        {
            using (var timer = logger.StartTimer("DurRedirectDetails"))
            {
                var response = new ApiResponse();                
                try
                {
                    response.Payload = serviceDur.GetWarningRedirectDetails();
                }
                catch (Exception ex)
                {                    
                    var errorMessage = ApiHelper.AuditException(ex.ToString(), serviceDur.GetPageState());
                    logger.Error("DurRedirectDetails Exception: " + ex.ToString());
                    response.ErrorContext = new ErrorContextModel()
                    {
                        Error = ErrorTypeEnum.ServerError,
                        Message = errorMessage
                    };
                    timer.Message = string.Format("<ErrorContext>{0}</ErrorContext>", response.ErrorContext.ToLogString());
                }
                return response;
            }
        }

        [HttpPost]

        public ApiResponse SubmitDurForm(SubmitDurRequest durRequest, IImpactDurWraper impactDur)
        {
            using (var timer = logger.StartTimer(string.Format("SubmitDurForm-request:", durRequest.ToLogString())))// todo: checked if ok to log request with instrumentation logging
            {
                var response = new ApiResponse();
                try
                {
                    response.Payload = serviceDur.SaveDurForm(durRequest, impactDur);
                }
                catch (Exception ex)
                {
                    var errorMessage = ApiHelper.AuditException(ex.ToString(), serviceDur.GetPageState());
                    logger.Error("SubmitDurForm Exception: " + ex.ToString());

                    response.ErrorContext = new ErrorContextModel()
                    {
                        Error = ErrorTypeEnum.ServerError,
                        Message = errorMessage
                    };
                    timer.Message = string.Format("<ErrorContext>{0}</ErrorContext>", response.ErrorContext.ToLogString());
                }
                timer.Message = string.Format("<Request>{0}</Request><Response>{1}</Response>", durRequest.ToLogString(), response.ToLogString());
                return response;
            }
        }

        [HttpPost]
        public ApiResponse SaveDurWarnings(ISaveDurRequest durRequest, IImpactDurWraper impactDur)
        {
            using (var timer = logger.StartTimer(string.Format("SaveDur-request:", durRequest.ToLogString())))// todo: checked if ok to log request with instrumentation logging
            {
                var response = new ApiResponse();
                try
                {
                    response.Payload = serviceDur.SaveDurWarnings(durRequest, impactDur);
                }
                catch (Exception ex)
                {
                    var errorMessage = ApiHelper.AuditException(ex.ToString(), serviceDur.GetPageState());
                    logger.Error("SaveDur Exception: " + ex.ToString());

                    response.ErrorContext = new ErrorContextModel()
                    {
                        Error = ErrorTypeEnum.ServerError,
                        Message = errorMessage
                    };
                    timer.Message = string.Format("<ErrorContext>{0}</ErrorContext>", response.ErrorContext.ToLogString());
                }
                timer.Message = string.Format("<Request>{0}</Request><Response>{1}</Response>", durRequest.ToLogString(), response.ToLogString());
                return response;
            }
        }

        [HttpPost]
        public ApiResponse CSMedRefillRequestNotAllowedOnContactProvider(ICSMedRefillNotAllowedContactRequest contactRequest, IImpactDurWraper impactDur)
        {
            using (var timer = logger.StartTimer(string.Format("CSMedRefillRequestNotAllowedOnContactProvider-request:", contactRequest.ToLogString())))// todo: checked if ok to log request with instrumentation logging
            {
                var response = new ApiResponse();
                try
                {
                    response.Payload = serviceDur.CSMedRefillRequestNotAllowedOnContactProvider(contactRequest, impactDur);
                }
                catch (Exception ex)
                {
                    var errorMessage = ApiHelper.AuditException(ex.ToString(), serviceDur.GetPageState());
                    logger.Error("CSMedRefillRequestNotAllowedOnContactProvider Exception: " + ex.ToString());

                    response.ErrorContext = new ErrorContextModel()
                    {
                        Error = ErrorTypeEnum.ServerError,
                        Message = errorMessage
                    };
                    timer.Message = string.Format("<ErrorContext>{0}</ErrorContext>", response.ErrorContext.ToLogString());
                }
                timer.Message = string.Format("<Request>{0}</Request><Response>{1}</Response>", string.Empty.ToLogString(), response.ToLogString());
                return response;
            }
        }
        [HttpPost]
        public ApiResponse GoBack(GoBackRequest goBackRequest)
        {
            using (var timer = logger.StartTimer(string.Format("GoBack-request:", goBackRequest.ToLogString())))// todo: checked if ok to log request with instrumentation logging
            {
                var response = new ApiResponse();
                try
                {
                    response.Payload = serviceDur.GoBack(goBackRequest);
                }
                catch (Exception ex)
                {
                    var errorMessage = ApiHelper.AuditException(ex.ToString(), serviceDur.GetPageState());
                    logger.Error("GoBack Exception: " + ex.ToString());

                    response.ErrorContext = new ErrorContextModel()
                    {
                        Error = ErrorTypeEnum.ServerError,
                        Message = errorMessage
                    };
                    timer.Message = string.Format("<ErrorContext>{0}</ErrorContext>", response.ErrorContext.ToLogString());
                }
                timer.Message = string.Format("<Request>{0}</Request><Response>{1}</Response>", string.Empty.ToLogString(), response.ToLogString());
                return response;
            }
        }

        [HttpGet]
        public ApiResponse IgnoreDurReasonsByCategoryandUserType(IgnoreReason.IgnoreCategory category,IImpactDurWraper durWrapper )
        {
            using (var timer = logger.StartTimer(string.Format("IgnoreDurReasonsByCategoryandUserType")))
            {
                var response = new ApiResponse();
                try
                {
                    response.Payload = serviceDur.GetIgnoreReasonsByCategoryAndUserType(category, durWrapper);
                }
                catch (Exception ex)
                {

                    var errorMessage = ApiHelper.AuditException(ex.ToString(), serviceDur.GetPageState());
                    logger.Error("IgnoreDurReasonsByCategoryandUserType Exception: " + ex.ToString());

                    response.ErrorContext = new ErrorContextModel()
                    {
                        Error = ErrorTypeEnum.ServerError,
                        Message = errorMessage
                    };
                    timer.Message = string.Format("<ErrorContext>{0}</ErrorContext>", response.ErrorContext.ToLogString());
                }
                timer.Message = string.Format("<Request>{0}</Request><Response>{1}</Response>", string.Empty.ToLogString(), response.ToLogString());
                return response;
            }
        }
    }
}
