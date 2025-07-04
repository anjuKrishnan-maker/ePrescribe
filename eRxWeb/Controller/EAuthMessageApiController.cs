using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.DatabaseSelector;
using Allscripts.ePrescribe.Shared.Logging;
using eRxWeb.AppCode;
using eRxWeb.AppCode.StateUtils;
using eRxWeb.ServerModel;
using eRxWeb.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eRxWeb.Controller
{
    public partial class EAuthMessageApiController : ApiController
    {
        private static ILoggerEx logger = LoggerEx.GetLogger();

        [HttpPost]
        public ApiResponse GetEauthMessage()
        {
            IStateContainer pageState = new StateContainer(HttpContext.Current.Session);
            var response = new ApiResponse();
            EAuthMessageModel data = new EAuthMessageModel();
            try
            {
                using (var timer = logger.StartTimer("GetEauthMessage"))
                {
                    data = getEauthMessage(pageState);
                    timer.Message = $"<EAuthMessageModel>{data.ToLogString()}</EAuthMessageModel>";
                    response.Payload = data;
                }
            }
            catch (Exception ex)
            {
                var errorMessage = ApiHelper.AuditException(ex.ToString(), pageState);
                logger.Error("GetEauthMessage Exception: " + ex.ToString());

                response.ErrorContext = new ErrorContextModel()
                {
                    Error = ErrorTypeEnum.ServerError,
                    Message = errorMessage
                };
            }
            return response;
        }
        internal static EAuthMessageModel getEauthMessage(IStateContainer pageState)
        {
            int openTask = 0, resolvedTask = 0;
            Constants.UserCategory usercategory = pageState.Cast<Constants.UserCategory>("UserType",
                Constants.UserCategory.GENERAL_USER);


            bool IsPOBUser = (usercategory == Constants.UserCategory.POB_REGULAR || usercategory == Constants.UserCategory.POB_SUPER) ? true : false;
            if (!(usercategory == Constants.UserCategory.POB_LIMITED))
            {
                GetEPAStatus(ref openTask, ref resolvedTask, pageState, usercategory);
            }
            EAuthMessageModel data = new EAuthMessageModel();

            data.EPAOpenMsgVisible = true;
            data.EPAResolvedMsgVisible = true;
            if (openTask > 0 && resolvedTask > 0)
            {
                data.EPAOpenMsg = (IsPOBUser ? "There are " : "You have ") + openTask.ToString() + (openTask > 1 ? " open ePA tasks" : " open ePA task");
                data.EPAOpenMsgUrl = "../" + Constants.PageNames.TASK_SUMMARY + "?ePATaskStatus=1" + "&TaskType=" + Convert.ToInt16(Constants.PrescriptionTaskType.EPA).ToString();
                data.EPAResolvedMsg = (IsPOBUser ? "There are " : "You have ") + resolvedTask.ToString() + (resolvedTask > 1 ? " resolved ePA tasks" : " resolved ePA task");
                data.EPAResolvedMsgUrl = "../" + Constants.PageNames.TASK_SUMMARY + "?ePATaskStatus=2" + "&TaskType=" + Convert.ToInt16(Constants.PrescriptionTaskType.EPA).ToString();

            }
            else
            {
                if (openTask > 0 && resolvedTask == 0)
                {
                    data.EPAOpenMsg = (IsPOBUser ? "There are " : "You have ") + openTask.ToString() + (openTask > 1 ? " open ePA tasks" : " open ePA task");
                    data.EPAOpenMsgUrl = "../" + Constants.PageNames.TASK_SUMMARY + "?ePATaskStatus=1" + "&TaskType=" + Convert.ToInt16(Constants.PrescriptionTaskType.EPA).ToString();
                    data.EPAResolvedMsgVisible = false;
                }
                else
                {
                    if (openTask == 0 && resolvedTask > 0)
                    {
                        data.EPAResolvedMsg = (IsPOBUser ? "There are " : "You have ") + resolvedTask.ToString() + (resolvedTask > 1 ? " resolved ePA tasks" : " resolved ePA task");
                        data.EPAResolvedMsgUrl = "../" + Constants.PageNames.TASK_SUMMARY + "?ePATaskStatus=2&TaskType=" + Convert.ToInt16(Constants.PrescriptionTaskType.EPA).ToString();
                        data.EPAOpenMsgVisible = false;
                    }
                    else
                    {
                        data.EPAResolvedMsgVisible = false;
                        data.EPAOpenMsgVisible = false;
                    }
                }
            }


            return data;
        }

        private static void GetEPAStatus(ref int open, ref int resolved, IStateContainer pageState, Constants.UserCategory usercategory)
        {

            string physicianID = (usercategory == Constants.UserCategory.POB_SUPER || usercategory == Constants.UserCategory.POB_REGULAR) ? null : pageState.GetString("USERID", "");

            if (!MenuApiHelper.IsEpaPatientLockDownMode(
                pageState.GetStringOrEmpty(Constants.SessionVariables.SSOMode),
                pageState.GetStringOrEmpty(Constants.SessionVariables.PatientId)
                ))
            {

                Allscripts.Impact.ePA.GetePAMsgTaskStatus(pageState.Cast<ConnectionStringPointer>("DBID", ConnectionStringPointer.ERXDB_SERVER_1), pageState.GetStringOrEmpty("LICENSEID"), physicianID, pageState.GetStringOrEmpty("USERID"), ref open, ref resolved, pageState.GetString("USERID", ""));
            }
            else
            {
                if (AppCode.StateUtils.UserInfo.IsPOBUser(pageState))
                {
                    open = Allscripts.Impact.ePA.GetePATaskCount(ApiHelper.GetSessionLicenseID(pageState), ApiHelper.GetSessionDelegateProviderID(pageState), ApiHelper.GetSessionUserID(pageState), ApiHelper.GetSessionPatientId(pageState), pageState.GetStringOrEmpty(Constants.SessionVariables.SSOMode), ApiHelper.GetDBID(pageState), ApiHelper.GetSessionUserID(pageState));

                }
                else
                {
                    open = Allscripts.Impact.ePA.GetePATaskCount(ApiHelper.GetSessionLicenseID(pageState), string.Empty, string.Empty, ApiHelper.GetSessionPatientId(pageState), pageState.GetStringOrEmpty(Constants.SessionVariables.SSOMode), ApiHelper.GetDBID(pageState), ApiHelper.GetSessionUserID(pageState));

                }
            }


        }
    }
    
}