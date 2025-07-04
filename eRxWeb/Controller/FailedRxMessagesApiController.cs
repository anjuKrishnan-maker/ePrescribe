using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.Shared.Logging;
using Allscripts.Impact;
using eRxWeb.AppCode;
using eRxWeb.ServerModel;
using eRxWeb.State;
using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.Http;

namespace eRxWeb.Controller
{
    public class FailedRxMessagesApiController : ApiController
    {
        private static ILoggerEx logger = LoggerEx.GetLogger();
        private static FailedRxMessage GetFailedRxMessages(IStateContainer sessionState)
        {

            FailedRxMessage refRq = null;
            
            if (sessionState["CURRENT_FAILED_RX_ERRORS_API"] == null)
            {
                var failedRx = SystemConfig.GetUserRequests(ApiHelper.GetSessionLicenseID(sessionState), null, 
                    Constants.RequestType.INTERNAL_RX_ERROR,
                    Constants.ResponseType.PUBLIC, Constants.RequestStatus.UNREAD, ApiHelper.GetDBID(sessionState));
              
                refRq = GetFailedRxMessages(failedRx);
                sessionState["CURRENT_FAILED_RX_ERRORS_API"] = refRq;
            }
            else
            {
                refRq = sessionState.Cast<FailedRxMessage>("CURRENT_FAILED_RX_ERRORS_API", null);
            }
            return refRq;

        }
        private static void SetFailedRxMessages(IStateContainer sessionState, FailedRxMessage value)
        {
            sessionState["CURRENT_FAILED_RX_ERRORS_API"] = value;
            sessionState["CURRENT_FAILED_RX_ERRORS"] = value;
        }

        [HttpPost]
        public  ApiResponse GetFailedRxMessages()
        {
            IStateContainer pageState = new StateContainer(HttpContext.Current.Session);
            var response = new ApiResponse();
            try
            {
                using (var timer = logger.StartTimer("GetFailedRxMessages"))
                {
                    FailedRxMessage refRq = GetFailedRxMessages(pageState);
                    timer.Message = $"<Response>{refRq.ToLogString()}</Response>";
                    response.Payload = refRq;
                } 
            }
            catch (Exception ex)
            {
                var errorMessage = ApiHelper.AuditException(ex.ToString(), pageState);
                logger.Error("GetFailedRxMessages Exception: " + ex.ToString());

                response.ErrorContext = new ErrorContextModel()
                {
                    Error = ErrorTypeEnum.ServerError,
                    Message = errorMessage
                };
            }
            return response;

        }
        [HttpPost]
        public  ApiResponse Confirm([FromBody]string requestID)
        {

            IStateContainer pageState = new StateContainer(HttpContext.Current.Session);
            var response = new ApiResponse();
            try
            {
                using (var timer = logger.StartTimer("Confirm"))
                {
                    if (requestID != "")
                    {
                        SystemConfig.UpdateUserRequest(requestID, Constants.RequestStatus.READ, ApiHelper.GetDBID(pageState));
                    }

                    SetFailedRxMessages(pageState, null);
                    FailedRxMessage refRq = GetFailedRxMessages(pageState);
                    timer.Message = $"<RequestID>{requestID}</RequestID><Response>{refRq.ToLogString()}</Response>";
                    response.Payload = refRq;
                } 
            }
            catch (Exception ex)
            {
                var errorMessage = ApiHelper.AuditException(ex.ToString(), pageState);
                logger.Error("Confirm Exception: " + ex.ToString());

                response.ErrorContext = new ErrorContextModel()
                {
                    Error = ErrorTypeEnum.ServerError,
                    Message = errorMessage
                };
            }
            return response;
        }
        private static FailedRxMessage GetFailedRxMessages(DataTable dt)
        {
            FailedRxMessage rxMsg = new FailedRxMessage();
           
                GetDataList(dt, rxMsg.FailedRxMessages);
                
            
            return rxMsg;
        }

        private static void GetDataList(DataTable dt, IList<FailedRxModel> list)
        {

            foreach (DataRow dr in dt.Rows)
            {
                list.Add(ApiHelper.MapObject<FailedRxModel>(dr));
            }
        }

    }
}