using Allscripts.ePrescribe.Shared.Logging;
using Allscripts.Impact;
using eRxWeb.AppCode;
using eRxWeb.ServerModel;
using eRxWeb.State;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Http;

namespace eRxWeb.Controller
{
    public partial class FailedRefReqMessagesApiController : ApiController
    {
        private static ILoggerEx logger = LoggerEx.GetLogger();
        private static DeniedRefReqMessages GetrefReqDenyError(IStateContainer sessionState)
        {

            DeniedRefReqMessages refRq= null;
            
            if (sessionState["CURRENT_DENIED_ERROR_REF_REQ_ERRORS_API"] == null)
            {
                var refReqDenyError = SystemConfig.GetRefRefDeniesAndError(ApiHelper.GetSessionLicenseID(sessionState),
                    ApiHelper.GetSessionUserID(sessionState), ApiHelper.GetDBID(sessionState));
                refRq = GetDeniedRefReqMessages(refReqDenyError);
                sessionState["CURRENT_DENIED_ERROR_REF_REQ_ERRORS_API"] = refRq;
            }
            else
            {
                refRq = sessionState.Cast< DeniedRefReqMessages>("CURRENT_DENIED_ERROR_REF_REQ_ERRORS_API",null);
            }
            return refRq;

        }

        [HttpPost]
        public ApiResponse GetDeniedRefReqMessages()
        {
            IStateContainer pageState = new StateContainer(HttpContext.Current.Session);
            var response = new ApiResponse();
            try
            {
                using (var timer = logger.StartTimer("GetDeniedRefReqMessages"))
                {
                    DeniedRefReqMessages refRq = GetrefReqDenyError(pageState);
                    timer.Message = $"<Response>{refRq.ToLogString()}</Response>";
                    response.Payload = refRq;
                }
            }
            catch (Exception ex)
            {
                var errorMessage = ApiHelper.AuditException(ex.ToString(), pageState);
                logger.Error("GetDeniedRefReqMessages Exception: " + ex.ToString());

                response.ErrorContext = new ErrorContextModel()
                {
                    Error = ErrorTypeEnum.ServerError,
                    Message = errorMessage
                };
            }
            return response;

        }

        private static DeniedRefReqMessages GetDeniedRefReqMessages(DataSet ds)
        {
            DeniedRefReqMessages refRq = new DeniedRefReqMessages();
            if (ds.Tables.Count > 1)
            {
                GetDataList(ds.Tables[0], refRq.DeniedRefReqs);
                GetDataList(ds.Tables[1], refRq.RefReqErrors);
            }
            return refRq;
        }

        private static void GetDataList(DataTable dt, IList<FailedRefReqModel> list)
        {

            foreach (DataRow dr in dt.Rows)
            {
                list.Add(ApiHelper.MapObject<FailedRefReqModel>(dr));
            }
        }

    }
}