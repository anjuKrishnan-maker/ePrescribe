using System.Web;
using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.DatabaseSelector;
using Allscripts.ePrescribe.Shared.Logging;
using Allscripts.Impact;
using Allscripts.Impact.Interfaces;
using eRxWeb.State;
using Veradigm.AdAgent.Business.Interfaces;

namespace eRxWeb.AppCode
{
    public class AdAgentLogger : ILogger
    {
        private static ILoggerEx _logger;
        private static IAudit _audit;
        private static IStateContainer _session;

        public AdAgentLogger(IStateContainer session, IAudit audit, ILoggerEx logger = null)
        {
            _logger = logger ?? LoggerEx.GetLogger();
            _audit = audit;
            _session = session;
        }

        public void Log(LogLevel level, string message)
        {
            if (_session == null && HttpContext.Current != null)
            {
                _session = new StateContainer(HttpContext.Current.Session);
            }

            if (_session == null)
            {
                return;
            }

          
            switch (level)
            {
                case LogLevel.Debug:
                    _logger.Debug(message);
                    break;
                case LogLevel.Info:
                    _logger.Info(message);
                    break;
                case LogLevel.Error:
                    _logger.Error(message);

                    var licenseId = _session.GetStringOrEmpty(Constants.SessionVariables.LicenseId);
                    var userId = _session.GetStringOrEmpty(Constants.SessionVariables.UserId);
                    var dbId = _session.Cast(Constants.SessionVariables.DbId, ConnectionStringPointer.ERXDB_DEFAULT);
                    _audit.AddException(userId, licenseId, message, string.Empty, string.Empty, string.Empty, dbId);

                    break;
                case LogLevel.Trace:
                    _logger.Trace(message);
                    break;
                case LogLevel.Warning:
                    _logger.Warn(message);
                    break;
            }
        }
    }
}