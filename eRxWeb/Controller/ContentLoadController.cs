using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Allscripts.ePrescribe.Shared.Logging;
using Allscripts.Impact.Ilearn;
using eRxWeb.AppCode;
using eRxWeb.AppCode.Interfaces;
using eRxWeb.Controllers;
using eRxWeb.ServerModel;
using eRxWeb.ServerModel.Request;
using eRxWeb.State;

namespace eRxWeb.Controller
{
    public class ContentLoadController : ApiController
    {
        private static readonly ILoggerEx _logger = LoggerEx.GetLogger();

        [HttpPost]
        public ApiResponse RetrieveRefreshPayload([FromBody] string pageName)
        {
            using (_logger.StartTimer("RetrieveRefreshPayload"))
            {
                return new ApiResponse { Payload = RetrieveRefreshPayload(pageName, new StateContainer(HttpContext.Current.Session)) };
            }
        }

        [HttpPost]
        public ApiResponse RetrieveInitialPayload([FromBody] string pageName)
        {
            using (_logger.StartTimer("RetrieveInitialPayload"))
            {
                return new ApiResponse { Payload = RetrieveInitialPayload(pageName, new StateContainer(HttpContext.Current.Session)) };
            }
        }

        [HttpPost]
        public ApiResponse RetrieveMedicationLoadedPayload(MedicationSelectedRequest medicationSelectedRequest)
        {
            using (_logger.StartTimer("RetrieveMedicationLoadedPayload"))
            {
                return new ApiResponse { Payload = RetrieveMedicationLoadedPayload(medicationSelectedRequest, new AsyncContext(HttpContext.Current, LocalLogContext.LogContextInfo, string.Empty, "ContentLoad")) };
            }
        }

        internal static InitialContentPayload RetrieveInitialPayload(string pageName, IStateContainer session)
        {
            var asyncContext = new AsyncContext(HttpContext.Current, LocalLogContext.LogContextInfo, pageName, "ContentLoad");
            var result = BuildInitialContentTaskList(new LoadAsyncUtil(asyncContext, _logger));

            try
            {
                Task.WaitAll(result.Item1.ToArray());
            }
            catch (AggregateException e)
            {
                ApiHelper.AuditException("RetrieveInitialPayload: " + e, asyncContext.Session);
            }

            return result.Item2;
        }

        internal static Tuple<List<Task>, InitialContentPayload> BuildInitialContentTaskList(ILoadAsyncUtil asyncUtil)
        {
            var asyncTasks = new List<Task>();
            var payload = new InitialContentPayload();

            switch (asyncUtil.AsyncContext.PageName.ToLower())
            {
                case "selectpatient":

                    asyncTasks.Add(asyncUtil.CreateTask(SelectPatientApiController.GetStartupParameters, p => payload.SelectPatientPayload = p));
                    break;
                case "reviewhistory":
                    //ReviewHistoryApiController.GetStartupParams
                    break;
                case "selectmedication":
                    //SelectMedicationApiController.GetStartupParams
                    break;
            }

            asyncTasks.Add(asyncUtil.CreateTask(RetrieveRefreshPayload, p => payload.ContentRefreshPayload = p));

            asyncTasks.Add(asyncUtil.CreateTask(UserPreferenceApiController.GetUserPreference, p => payload.UserPreferencePayload = p));

            asyncTasks.Add(asyncUtil.CreateTask(AppBuildUpAPIController.GetSiteInfo, p => payload.SitePayload = p));

            asyncTasks.Add(asyncUtil.CreateTask(CommonUiSetup.RetrieveUrls, p => payload.CommonUiInitialPayload = p));

            asyncTasks.Add(asyncUtil.CreateTask(ApiHelper.SessionAppVersion, p => payload.FooterPayload = new AppInfo { VersionNumber = p }));

            asyncTasks.Add(asyncUtil.CreateTask(MenuApiController.PopulateMenuItmes, p => payload.MenuPayload = p));

            asyncTasks.Add(asyncUtil.CreateTask(UserSessionApiController.TimeRemaining, p => payload.SessionTimeoutPayload = p));

            asyncTasks.Add(asyncUtil.CreateTask(MedicationHelpSerachAPIController.getSerachUrl, p => payload.HelpSearchPayload = p));

            return new Tuple<List<Task>, InitialContentPayload>(asyncTasks, payload);
        }
        internal static ContentRefreshPayload RetrieveRefreshPayload(string pageName, IStateContainer session)
        {
            var asyncContext = new AsyncContext(HttpContext.Current, LocalLogContext.LogContextInfo, pageName, "ContentLoad");
            var result = BuildRefreshTaskList(new LoadAsyncUtil(asyncContext, _logger));
           
            try
            {
                Task.WaitAll(result.Item1.ToArray());
            }
            catch (AggregateException e)
            {
                ApiHelper.AuditException("RetrieveRefreshPayload: " + e, asyncContext.Session);
            }

            return result.Item2;
        }

        internal static Tuple<List<Task>, ContentRefreshPayload> BuildRefreshTaskList(ILoadAsyncUtil asyncUtil)
        {
            var payload = new ContentRefreshPayload();
            var asyncTasks = new List<Task>();
            payload.ILearnPayload = new ILearnModel();

            asyncTasks.Add(asyncUtil.CreateTask(RightPanelController.RetrievePayload, p => payload.RightPanelPayload = p));

            asyncTasks.Add(asyncUtil.CreateTask(ILearnConfigurationManager.GetErxILearnPageUrl, p => payload.ILearnPayload.Url = p));

            //Do not delete this code. This code requires reimplementation and can be deleted if required then.
            //asyncTasks.Add(asyncUtil.CreateTask(ILearnAPIController.GetILearnVideoNotification, p => payload.ILearnPayload.ILearnNotification = p));

            return new Tuple<List<Task>, ContentRefreshPayload>(asyncTasks, payload);
        }

        internal static MedicationSelectedPayload RetrieveMedicationLoadedPayload(MedicationSelectedRequest medicationSelected, AsyncContext asyncContext)
        {
            var result = BuildMedSelectedTaskList(medicationSelected, new LoadAsyncUtil(asyncContext, _logger));

            try
            {
                Task.WaitAll(result.Item1.ToArray());
            }
            catch (AggregateException e)
            {
                ApiHelper.AuditException("RetrieveMedicationLoadedPayload: " + e, asyncContext.Session);
            }

            return result.Item2;
        }

        internal static Tuple<List<Task>, MedicationSelectedPayload> BuildMedSelectedTaskList(MedicationSelectedRequest medicationSelected, ILoadAsyncUtil asyncUtil)
        {
            var payload = new MedicationSelectedPayload();
            var asyncTasks = new List<Task>();

            asyncTasks.Add(asyncUtil.CreateTask(RobustLinkApiController.GetRobustLinksData, medicationSelected, p => payload.RobustLinkPayload = p));

            asyncTasks.Add(asyncUtil.CreateTask(CopayCoverageInfoApiController.GetCopayCoverageList, medicationSelected, p => payload.CopayCoveragePayload = p));

            asyncTasks.Add(asyncUtil.CreateTask(FormularyAlternativesApiController.GetFormularyAlternatives, medicationSelected, p => payload.FormularyAlternativesPayload = p));

            asyncTasks.Add(asyncUtil.CreateTask(GenericAlternativeApiController.getGenericAlternatives, medicationSelected, p => payload.GenericAlternativesPayload = p));

            return new Tuple<List<Task>, MedicationSelectedPayload>(asyncTasks, payload);
        }
    }
}