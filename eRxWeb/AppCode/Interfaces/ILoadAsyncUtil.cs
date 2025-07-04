using eRxWeb.ServerModel.Request;
using eRxWeb.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eRxWeb.ServerModel;

namespace eRxWeb.AppCode.Interfaces
{
    public interface ILoadAsyncUtil
    {
        AsyncContext AsyncContext { get; set; }

        /// <summary>
        /// Action(AsyncContext)
        /// </summary>
        Task CreateTask<T>(Func<AsyncContext, T> panelAction, Action<T> setPayload);

        /// <summary>
        /// Action()
        /// </summary>
        Task CreateTask<T>(Func<T> panelAction, Action<T> setPayload);

        /// <summary>
        /// Action(IStateContainer)
        /// </summary>
        Task CreateTask<T>(Func<IStateContainer, T> panelAction, Action<T> setPayload);

        /// <summary>
        /// Action(PageName, IStateContainer)
        /// </summary>
        Task CreateTask<T>(Func<string, IStateContainer, T> panelAction, Action<T> setPayload);

        /// <summary>
        /// Action(PageName)
        /// </summary>
        Task CreateTask<T>(Func<string, T> panelAction, Action<T> setPayload);

        /// <summary>
        /// Action(param1, IStateContainer)
        /// </summary>
        Task CreateTask<T, T1>(Func<T1, IStateContainer, T> panelAction, T1 param1, Action<T> setPayload);

        Task CreateTask<T>(Action<IStateContainer, T> action, T param1);
    }
}
