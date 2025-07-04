using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Http;
using System.Web.UI;
using System.Web.UI.WebControls;
using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.DatabaseSelector;
using Allscripts.ePrescribe.Shared.Logging;
using Allscripts.Impact;
using eRxWeb.ServerModel;
using eRxWeb.State;
using Allscripts.ePrescribe.Data.Application;
using eRxWeb.AppCode;
using Allscripts.ePrescribe.Data.HomeAddress;
using Allscripts.ePrescribe.Objects;

namespace eRxWeb.Controllers
{
    public partial class HomeAddressApiController : ApiController
    {
        private static ILoggerEx logger = LoggerEx.GetLogger();

        [HttpGet]
        public ApiResponse GetStartUpData()
        {
            using (var timer = logger.StartTimer("HomeAddressApiController - GetStartUpData"))
            {
                IStateContainer session = new StateContainer(HttpContext.Current.Session);
                var response = new ApiResponse();

                try
                {
                    HomeAddressStartUpModel homeAddressStartUpModel = new HomeAddressStartUpModel();
                    homeAddressStartUpModel.States = new ApplicationData().GetStates(session.Cast(Constants.SessionVariables.DbId, ConnectionStringPointer.ERXDB_DEFAULT));
                    response.Payload = homeAddressStartUpModel;
                    timer.Message = string.Format($"<Response>{response.ToLogString()}</Response>");
                }
                catch (Exception ex)
                {
                    var errorMessage = ApiHelper.AuditException(ex.ToString(), session);
                    logger.Error("HomeAddressApiController - GetStartUpData: Exception while fetching data: " + ex.ToString());

                    response.ErrorContext = new ErrorContextModel()
                    {
                        Error = ErrorTypeEnum.ServerError,
                        Message = errorMessage
                    };
                    timer.Message = string.Format($"<ErrorContext>{response.ErrorContext.ToLogString()}</ErrorContext>");
                }
                return response;
            }
        }

        [HttpPost]
        public ApiResponse SaveHomeAddress(HomeAddressSaveRequest homeAddressSaveRequest)
        {
            using (var timer = logger.StartTimer("HomeAddressApiController - SaveHomeAddress"))
            {
                IStateContainer session = new StateContainer(HttpContext.Current.Session);
                var response = new ApiResponse();
                HomeAddressSaveModel homeAddressSaveModel = new HomeAddressSaveModel();
                try
                {
                    homeAddressSaveModel.RedirectUrl = Constants.PageNames.USER_EULA;

                    new HomeAddresssData().UpdateProviderHomeAddress(
                        new HomeAddressDataModel() { 
                            UserID = session.GetStringOrEmpty(Constants.SessionVariables.UserId),
                            HomeAddress = homeAddressSaveRequest.HomeAddress,
                            HomeAddress2 = homeAddressSaveRequest.HomeAddress2,
                            City = homeAddressSaveRequest.City,
                            State = homeAddressSaveRequest.State,
                            ZipCode = homeAddressSaveRequest.ZipCode,
                            HomeAddressCheckStatus = HOME_ADDRESS_CHECK_STATUS.COMPLETED
                        }, session.Cast(Constants.SessionVariables.DbId, ConnectionStringPointer.ERXDB_DEFAULT));
                    session[Constants.SessionVariables.HomeAddressCheckStatus] = HOME_ADDRESS_CHECK_STATUS.COMPLETED;
                    homeAddressSaveModel.IsSaveSuccessful = true;
                    response.Payload = homeAddressSaveModel;

                    timer.Message = string.Format($"<Response>{response.ToLogString()}</Response>");
                }
                catch (Exception ex)
                {
                    var errorMessage = ApiHelper.AuditException(ex.ToString(), session);
                    logger.Error("HomeAddressApiController - SaveHomeAddress: Exception while fetching data: " + ex.ToString());

                    response.ErrorContext = new ErrorContextModel()
                    {
                        Error = ErrorTypeEnum.ServerError,
                        Message = errorMessage
                    };
                    timer.Message = string.Format($"<ErrorContext>{response.ErrorContext.ToLogString()}</ErrorContext>");
                }
                return response;
            }
        }
    }
}