using Allscripts.ePrescribe.Common;
using eRxWeb.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Allscripts.ePrescribe.Shared.Logging;
using System.Web.Services;
using eRxWeb.ServerModel;
using eRxWeb.AppCode;
using System.Web.Http;
using eRxWeb.ServerModel.Request;

namespace eRxWeb.Controller
{
    public partial class ChangePasswordApiController : ApiController
    {
        private static ILoggerEx logger = LoggerEx.GetLogger();

        [HttpPost]
        public ApiResponse ChangePassword(ChangePasswordRequest CP)
        {
            IStateContainer pageState = new StateContainer(HttpContext.Current.Session);
            var response = new ApiResponse();
            try
            {
                response.Payload = GetChangePassword(CP.OldPassword, CP.NewPassword,pageState);
            }
            catch (Exception ex)
            {
                var errorMessage = ApiHelper.AuditException(ex.ToString(), pageState);
                logger.Error("ChangePassword Exception: " + ex.ToString());

                response.ErrorContext = new ErrorContextModel()
                {
                    Error = ErrorTypeEnum.ServerError,
                    Message = errorMessage
                };
            }
            return response;
        }


        private static ChangePasswordModel GetChangePassword(string OldPassword, string NewPassword,IStateContainer pageState )
        {

            ChangePasswordModel model = new ChangePasswordModel();
            model.PasswordHelpText = Allscripts.Impact.SystemConfig.GetHelp("~/" + Constants.PageNames.CHANGE_SHIELD_USER_PASSWORD);
            try
            {
                using (var timer = logger.StartTimer("GetChangePassword"))
                {
                    eRxWeb.ePrescribeSvc.ePrescribeSvcResponse response = EPSBroker.ChangeUserPassword(
                    pageState.GetStringOrEmpty("SessionShieldUserName"),
                    OldPassword.Trim(),
                    NewPassword.Trim(),
                    pageState.GetStringOrEmpty("ShieldSecurityToken"),
                    pageState.GetStringOrEmpty("ShieldExternalTenantID"),
                    AppCode.ApiHelper.GetDBID(pageState));
               
                    if (response.Success)
                    {
                        logger.Debug("Password reset success.");
                        model.Status = "Password successfully changed. Please logout and log back in to update user settings.";
                        pageState.Remove(Constants.SessionVariables.DaysLeftBeforePasswordExpires);
                        model.Success = true;
                    }
                    else
                    {
                        logger.Debug("Password reset failed.");

                        foreach (string s in response.Messages)
                        {
                            model.Status += s;
                            model.Status += " ";
                        }
                        model.Success = false;
                    }
                    timer.Message = $"<SessionShieldUserName>{pageState.GetStringOrEmpty("SessionShieldUserName")}</SessionShieldUserName><ChangePasswordStatus>{model.Status}</ChangePasswordStatus><ChangePasswordSuccess>{model.Success}</ChangePasswordSuccess>";
                }
            }
            catch (Exception ex)
            {
                logger.Debug("Password reset failed." + Environment.NewLine + "Exception message: " + ex);
                model.Status = "Password could not be updated.  Please call Allscript Support for assistance.";
            }

            return model;

        }
    }
}