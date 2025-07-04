using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.DatabaseSelector;
using Allscripts.ePrescribe.ExtensionMethods;
using Allscripts.ePrescribe.Shared.Logging;
using Allscripts.Impact;
using eRxWeb.AppCode;
using eRxWeb.AppCode.StateUtils;
using eRxWeb.ServerModel;
using eRxWeb.State;
using System;
using System.Data;
using System.Web;
using System.Web.Http;
using System.Collections.Generic;
using Allscripts.Impact.Interfaces;
namespace eRxWeb.Controller
{
    public class SupervisingProviderApiController : ApiController
    {
        private static readonly ILoggerEx logger = LoggerEx.GetLogger();
        private IStateContainer pageState;
        public SupervisingProviderApiController()
        {
            this.pageState = new StateContainer(HttpContext.Current.Session);
        }
        [HttpPost]
        public ApiResponse GetSupervisingProviderList()
        {
            var response = new ApiResponse();
            try
            {
                DataSet dsListProvider = new DataSet();
                DataView activeProvidersView = new DataView();
                dsListProvider = Provider.GetProviders(pageState.GetStringOrEmpty("LICENSEID"), Convert.ToInt16(pageState.GetStringOrEmpty("SiteID")), ApiHelper.GetDBID(pageState));

                activeProvidersView = dsListProvider.Tables["Provider"].DefaultView;
                activeProvidersView.RowFilter = Constants.FILTER_ONLY_ACTIVE_PROVIDERS;

                List<SupProviderModel> supList = new List<SupProviderModel>();
                foreach (DataRowView dr in activeProvidersView)
                {
                    SupProviderModel sup = new SupProviderModel();
                    sup.ProviderID = dr["ProviderID"].ToString();
                    sup.ProviderName = dr["ProviderName"].ToString();
                    supList.Add(sup);
                }
                response.Payload = supList;
            }
            catch(Exception ex)
            {
                var errorMessage = ApiHelper.AuditException(ex.ToString(), pageState);
                logger.Error("GetSupervisingProviderList Exception: " + ex.ToString());
                response.ErrorContext = new ErrorContextModel()
                {
                    Error = ErrorTypeEnum.ServerError,
                    Message = errorMessage
                };
                return response;
            }
            return response;
        }

        [HttpPost]
        public ApiResponse SetSupervisingProvider([FromBody]string provID)
        {
            var response = new ApiResponse();
            IStateContainer session = new StateContainer(HttpContext.Current.Session);
            var dbid = session.Cast(Constants.SessionVariables.DbId, ConnectionStringPointer.ERXDB_DEFAULT);
            try
            {
                var supervisingProvider = new RxUser(provID, dbid);

                var errorMessage = SupervisingProvider.SetInSession(supervisingProvider, session, new DeaScheduleUtility());

                if (!string.IsNullOrWhiteSpace(errorMessage))
                {
                    response.ErrorContext = new ErrorContextModel
                    {
                        Error = ErrorTypeEnum.UserMessage,
                        Message = errorMessage
                    };
                }
            }
            catch (Exception ex)
            {
                var errorMessage = ApiHelper.AuditException(ex.ToString(), pageState);
                logger.Error("SetSupervisingProvider Exception: " + ex.ToString());
                response.ErrorContext = new ErrorContextModel()
                {
                    Error = ErrorTypeEnum.ServerError,
                    Message = errorMessage
                };
                return response;
            }
            return response;
        }
    }
}
