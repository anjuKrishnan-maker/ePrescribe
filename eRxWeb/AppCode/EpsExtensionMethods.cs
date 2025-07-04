using System;
using System.Web;
using Allscripts.ePrescribe.Shared.Data;
using LoggingInfo = eRxWeb.ePrescribeSvc.LoggingInfo;

namespace eRxWeb.AppCode
{
    public static class EpsExtensionMethods
    {
        public static LoggingInfo Init(this LoggingInfo loggingInfo)
        {
            if (HttpContext.Current != null && HttpContext.Current.Session != null)
            {
                loggingInfo = new LoggingInfo
                {
                    CallingApplication = LogConstants.MAIN_APP,
                    CallingUserGuid =
                        HttpContext.Current.Session["USERID"] != null
                            ? new Guid(HttpContext.Current.Session["USERID"].ToString())
                            : (Guid?) null,
                    CallingUserLicenseGuid =
                        HttpContext.Current.Session["LICENSEID"] != null
                            ? new Guid(HttpContext.Current.Session["LICENSEID"].ToString())
                            : (Guid?) null,
                    AspnetSessionID = HttpContext.Current.Session.SessionID,
                    EnableLogging =
                        HttpContext.Current.Session[LogConstants.ENABLE_LOGGING_KEY] != null &&
                        (bool) HttpContext.Current.Session[LogConstants.ENABLE_LOGGING_KEY]
                };
            }
            return loggingInfo;
        }
    }
}