using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using Allscripts.ePrescribe.DatabaseSelector;
using Allscripts.Impact.Ilearn;
using eRxWeb.State;
using Allscripts.ePrescribe.Common;
using Allscripts.Impact;
using System.Data;
using System.Text;
using eRxWeb.AppCode;
using System.Configuration;
using eRxWeb.ServerModel;
using Allscripts.ePrescribe.Shared.Logging;
using System.Web.Http;

namespace eRxWeb.Controller
{
    public partial class ILearnAPIController : ApiController
    {
        internal static string GetILearnVideoNotification(IStateContainer RequestState)
        {
            return GetILearnVideoNotification(RequestState, new ILearnServiceManager(new ILearnClient(new ILearnConfigurationManager())));
        }
        internal static string GetILearnVideoNotification(IStateContainer RequestState, ILearnServiceManager manager)
        {
            return manager.GetNewVideoNotification(RequestState.GetStringOrEmpty("UserCode"), RequestState.GetStringOrEmpty("LicenseID"));
        }
    }
}
