using System.Web;
using Allscripts.ePrescribe.Shared.Data;
using eRxWeb.State;

namespace eRxWeb.AppCode
{
    public class AsyncContext
    {
        public IStateContainer Session { get; set; }
        public LoggingInfo LoggingInfo { get; set; }
        public string PageName { get; set; }
        public HttpContext HttpContext { get; set; }
        public string TimerNamePrefix { get; set; }

        public AsyncContext(IStateContainer session)
        {
            Session = session;
        }
        public AsyncContext(HttpContext context, LoggingInfo logContextInfo, string pageName, string timerNamePrefix)
        {
            HttpContext = context;
            Session = new StateContainer(context?.Session);
            LoggingInfo = logContextInfo;
            PageName = pageName;
            TimerNamePrefix = timerNamePrefix;
        }

    }
}