using System;
using System.Threading.Tasks;
using System.Web;
using Allscripts.ePrescribe.Shared.Logging;
using Allscripts.Impact;
using Allscripts.Impact.Interfaces;
using ComponentSpace.SAML.Profiles;
using eRxWeb.AppCode.Interfaces;
using eRxWeb.Controller;
using eRxWeb.ServerModel;
using eRxWeb.ServerModel.Request;
using eRxWeb.State;

namespace eRxWeb.AppCode
{
    public class LoadAsyncUtil : ILoadAsyncUtil
    {
        private readonly ILoggerEx _logger;

        public AsyncContext AsyncContext { get; set; }

        public LoadAsyncUtil(AsyncContext asyncContext, ILoggerEx logger)
        {
            AsyncContext = asyncContext;
            _logger = logger;
        }

        public string GetTimerName(string methodName)
        {
            return $"{AsyncContext.TimerNamePrefix}-{methodName}";
        }

        /// <summary>
        /// Action(AsyncContext)
        /// </summary>
        public Task CreateTask<T>(Func<AsyncContext, T> panelAction, Action<T> setPayload)
        {
            var task = Task.Run(() =>
            {
                HttpContext.Current = AsyncContext.HttpContext;
                LocalLogContext.SetLoggingContext(AsyncContext.LoggingInfo);

                using (_logger.StartTimer(GetTimerName(panelAction.Method.Name)))
                {
                    setPayload(panelAction.Invoke(AsyncContext));
                }
            });
            return task;
        }

        /// <summary>
        /// Action()
        /// </summary>
        public Task CreateTask<T>(Func<T> panelAction, Action<T> setPayload)
        {
            var task = Task.Run(() =>
            {
                HttpContext.Current = AsyncContext.HttpContext;
                LocalLogContext.SetLoggingContext(AsyncContext.LoggingInfo);

                using (_logger.StartTimer(GetTimerName(panelAction.Method.Name)))
                {
                    setPayload(panelAction.Invoke());
                }
            });
            return task;
        }

        /// <summary>
        /// Action(IStateContainer)
        /// </summary>
        public Task CreateTask<T>(Func<IStateContainer, T> panelAction, Action<T> setPayload)
        {
            var task = Task.Run(() =>
            {
                HttpContext.Current = AsyncContext.HttpContext;
                LocalLogContext.SetLoggingContext(AsyncContext.LoggingInfo);

                using (_logger.StartTimer(GetTimerName(panelAction.Method.Name)))
                {
                    setPayload(panelAction.Invoke(AsyncContext.Session));
                }
            });
            return task;
        }

        /// <summary>
        /// Action(PageName, IStateContainer)
        /// </summary>
        public Task CreateTask<T>(Func<string, IStateContainer, T> panelAction, Action<T> setPayload)
        {
            var task = Task.Run(() =>
            {
                HttpContext.Current = AsyncContext.HttpContext;
                LocalLogContext.SetLoggingContext(AsyncContext.LoggingInfo);

                using (_logger.StartTimer(GetTimerName(panelAction.Method.Name)))
                {
                    setPayload(panelAction.Invoke(AsyncContext.PageName, AsyncContext.Session));
                }
            });
            return task;
        }

        /// <summary>
        /// Action(PageName)
        /// </summary>
        public Task CreateTask<T>(Func<string, T> panelAction, Action<T> setPayload)
        {
            var task = Task.Run(() =>
            {
                HttpContext.Current = AsyncContext.HttpContext;
                LocalLogContext.SetLoggingContext(AsyncContext.LoggingInfo);

                using (_logger.StartTimer(GetTimerName(panelAction.Method.Name)))
                {
                    setPayload(panelAction.Invoke(AsyncContext.PageName));
                }
            });
            return task;
        }

        /// <summary>
        /// Action(param1, IStateContainer)`
        /// </summary>
        public Task CreateTask<T, T1>(Func<T1, IStateContainer, T> panelAction, T1 param1, Action<T> setPayload)
        {
            var task = Task.Run(() =>
            {
                HttpContext.Current = AsyncContext.HttpContext;
                LocalLogContext.SetLoggingContext(AsyncContext.LoggingInfo);

                using (_logger.StartTimer(GetTimerName(panelAction.Method.Name)))
                {
                    setPayload(panelAction.Invoke(param1, AsyncContext.Session));
                }
            });
            return task;
        }

        public Task CreateTask<T>(Action<IStateContainer, T> action, T param1)
        {
            var task = Task.Run(() =>
            {
                HttpContext.Current = AsyncContext.HttpContext;
                LocalLogContext.SetLoggingContext(AsyncContext.LoggingInfo);

                using (_logger.StartTimer(GetTimerName(action.Method.Name)))
                {
                    action.Invoke(AsyncContext.Session, param1);
                }
            });
            return task;
        }
    }
}