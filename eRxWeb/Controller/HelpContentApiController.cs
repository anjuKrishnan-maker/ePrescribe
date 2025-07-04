using Allscripts.ePrescribe.Shared.Logging;
using eRxWeb.AppCode;
using eRxWeb.ServerModel;
using eRxWeb.State;
using System;
using System.Web;
using System.Web.Http;

namespace eRxWeb.Controller
{
    public partial class HelpContentApiController : ApiController
    {
        private  ILoggerEx logger = LoggerEx.GetLogger();
        [HttpPost]
        public ApiResponse GetHelp([FromBody]string pageName)
        {
            IStateContainer pageState = new StateContainer(HttpContext.Current.Session);
            var response = new ApiResponse();
            try
            {
                using (var timer = logger.StartTimer("GetHelp"))
                {
                    response.Payload = GetHelpContent(pageName, pageState);
                    timer.Message = $"<PageName>{pageName}</PageName><response>{response.ToLogString()}</response>";
                }   
            }
            catch (Exception ex)
            {
                var errorMessage = ApiHelper.AuditException(ex.ToString(), pageState);
                logger.Error("GetHelp Exception: " + ex.ToString());

                response.ErrorContext = new ErrorContextModel()
                {
                    Error = ErrorTypeEnum.ServerError,
                    Message = errorMessage
                };
            }
            return response;
        }

        internal static string GetHelpContent(string pageName,IStateContainer pageState)
        {
           return Helper.GetHelpText("~/"+pageName);
        }
    }
}