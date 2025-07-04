using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.DatabaseSelector;
using Allscripts.Impact;
using eRxWeb.AppCode;
using eRxWeb.Erx_Response;
using eRxWeb.State;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using Allscripts.ePrescribe.ExtensionMethods;
using eRxWeb.ServerModel;
using Allscripts.ePrescribe.Shared.Logging;
using System.Web.Http;
using eRxWeb.ServerModel.Request;
using PatientPrivacy = Allscripts.Impact.PatientPrivacy;

namespace eRxWeb.Controllers
{
    public partial class PrivacyOverrideApiController : ApiController
    {
        private static readonly ILoggerEx logger = LoggerEx.GetLogger();

        IStateContainer pageState = new StateContainer(HttpContext.Current.Session);

        public string RedirectURL
        {
            get
            {
                return pageState["RedirectURL"] != null ? pageState["RedirectURL"].ToString() : string.Empty;
            }
            set
            {
                pageState["RedirectURL"] = value;
            }
        }

        public string CancelURL
        {
            get
            {
                return pageState["CancelURL"] != null ? pageState["CancelURL"].ToString() : string.Empty;
            }
            set
            {
                pageState["CancelURL"] = value;
            }
        }

        public string PatientName
        {
            get
            {
                return pageState["PatientName"] != null ? pageState["PatientName"].ToString() : string.Empty;
            }
            set
            {
                pageState["PatientName"] = value;
            }
        }

        public string PhysicianId
        {
            get
            {
                return pageState["PhysicianId"] != null ? pageState["PhysicianId"].ToString() : string.Empty;
            }
            set
            {
                pageState["PhysicianId"] = value;
            }
        }

        public string TaskScriptMessageId
        {
            get
            {
                return pageState["TaskScriptMessageId"] != null ? pageState["TaskScriptMessageId"].ToString() : string.Empty;
            }
            set
            {
                pageState["TaskScriptMessageId"] = value;
            }
        }

        public string SessionLicenseID
        {
            get
            {
                if (pageState["LICENSEID"] == null)
                    return null;

                return pageState["LICENSEID"].ToString();
            }
        }

        public string PrivacyPatientID
        {
            get
            {
                return pageState["PrivacyPatientID"] != null ? pageState["PrivacyPatientID"].ToString() : string.Empty;
            }
            set
            {

                pageState["PrivacyPatientID"].ToString();
            }
        }

        public string SelectedProviderID
        {
            get
            {
                return pageState["CLINICAL_MSG_DELEGATE_PROVIDER"] != null ? pageState["CLINICAL_MSG_DELEGATE_PROVIDER"].ToString() : string.Empty;
            }
            set
            {

                pageState["CLINICAL_MSG_DELEGATE_PROVIDER"].ToString();
            }
        }
        public string ProviderID
        {
            get
            {
                return pageState["DelegateProviderID"] != null ? pageState["DelegateProviderID"].ToString() : string.Empty;
            }
            set
            {

                pageState["DelegateProviderID"].ToString();
            }
        }

        public void showOverlay(string patientID)
        {
            pageState["PrivacyPatientID"] = patientID;
        }

        [HttpPost]
        public ApiResponse Cancel(PrivacyRequest request)
        {
            using (var timer = logger.StartTimer("Cancel"))
            {
                string PageName = request.PageName;
                IStateContainer pageState = new StateContainer(HttpContext.Current.Session);
                PrivacyOverrideApiController data = new PrivacyOverrideApiController();
                string smid = data.TaskScriptMessageId;
                var response = new ApiResponse();
                try
                {
                    PrivacyOverrideModel model = new PrivacyOverrideModel();
                    pageState.Remove("RestrictedUserOverridden");
                    pageState.Remove("PATIENTID");
                    pageState.Remove("ToReviewHistory");
                    pageState.Remove("NewRx");
                    pageState.Remove("PhysicianId");
                    pageState.Remove("PatientName");
                    PageName = PageName.ToUpper();
                    //
                    // do ApproveScriptMessagePatient, DocRefillMenu, and ListSendScripts really show Privacy Override popup?
                    //
                    if (Constants.PageNames.APPROVE_SCRIPT_MESSAGE_PATIENT.ToLower() == PageName.ToLower())
                        model.CancelURL = string.Concat(Constants.PageNames.APPROVE_SCRIPT_MESSAGE_PATIENT, "?smid=", smid, "&From=", Constants.PageNames.APPROVE_REFILL_TASK);
                    if (Constants.PageNames.DOC_REFILL_MENU.ToUpper() == PageName)
                        model.CancelURL = Constants.PageNames.DOC_REFILL_MENU;
                    if (Constants.PageNames.LIST_SEND_SCRIPTS.ToUpper() == PageName)
                        model.CancelURL = Constants.PageNames.LIST_SEND_SCRIPTS + "?tasktype=4";
                    model.PageName = PageName;
                    response.Payload = model;
                }
                catch (Exception ex)
                {
                    var errorMessage = ApiHelper.AuditException(ex.ToString(), pageState);
                    logger.Error("Cancel Exception: " + ex.ToString());

                    response.ErrorContext = new ErrorContextModel()
                    {
                        Error = ErrorTypeEnum.ServerError,
                        Message = errorMessage
                    };
                    timer.Message = string.Format("<ErrorContext>{0}</ErrorContext>", response.ErrorContext.ToLogString());
                }
                timer.Message = string.Format("<Request>{0}</Request><Response>{1}</Response>", $"<PageName>{PageName.ToLogString()}</PageName><smid>{smid.ToLogString()}</smid>", response.ToLogString());
                return response;
            }
        }

        [HttpPost]
        public ApiResponse GetOverrideReason(PrivacyRequest request)
        {
            using (var timer = logger.StartTimer("GetOverrideReason"))
            {
                IStateContainer session = new StateContainer(HttpContext.Current.Session);
                var response = new ApiResponse();
                try
                {
                    var createdUtcDt = Convert.ToDateTime(request.createdUtc).ToString("yyyy-MM-dd HH:mm:ss.fff");
                    var dtOverrideReason = new PatientPrivacy().GetPatientPrivacyOverrideReason(request.patientId, request.userId, createdUtcDt, session.Cast(Constants.SessionVariables.DbId, ConnectionStringPointer.ERXDB_DEFAULT));
                    if (dtOverrideReason.Rows.Count > 0)
                    {
                        response.Payload = dtOverrideReason.Rows[0]["OverrideReason"].ToString();
                    }
                    else
                    {
                        response.Payload = "";
                    }
                    
                }
                catch (Exception ex)
                {
                    var errorMessage = ApiHelper.AuditException(ex.ToString(), new StateContainer(HttpContext.Current.Session));
                    logger.Error("GetOverrideReason Exception: " + ex.ToString());

                    response.ErrorContext = new ErrorContextModel()
                    {
                        Error = ErrorTypeEnum.ServerError,
                        Message = errorMessage
                    };
                    timer.Message = string.Format("<ErrorContext>{0}</ErrorContext>", response.ErrorContext.ToLogString());
                }
                timer.Message = string.Format("<Request>{0}</Request><Response>{1}</Response>", $"<patientId>{request.patientId.ToLogString()}</patientId><createdUtc>{request.createdUtc.ToLogString()}</createdUtc>", response.ToLogString());
                return response;
            }
        }

        [HttpPost]
        public ApiResponse SaveOverrideReason(PrivacyRequest request)
        {
            using (var timer = logger.StartTimer("SaveOverrideReason"))
            {
                IPatientPrivacy pp = new PatientPrivacy();
                IStateContainer pageState = new StateContainer(HttpContext.Current.Session);
                var response = new ApiResponse();
                try
                {
                    response.Payload = GetSaveOverrideReason(pageState, pp, request.OverrideText, request.PageName);
                    
                }
                catch (Exception ex)
                {
                    var errorMessage = ApiHelper.AuditException(ex.ToString(), pageState);
                    logger.Error("SaveOverrideReason Exception: " + ex.ToString());

                    response.ErrorContext = new ErrorContextModel()
                    {
                        Error = ErrorTypeEnum.ServerError,
                        Message = errorMessage
                    };
                    timer.Message = string.Format("<ErrorContext>{0}</ErrorContext>", response.ErrorContext.ToLogString());
                }
                timer.Message = string.Format("<Request>{0}</Request><Response>{1}</Response>", $"<OverrideText>{request.OverrideText.ToLogString()}</OverrideText><pp>{pp.ToLogString()}</pp>", response.ToLogString());
                return response;
            }
        }

        public static PrivacyOverrideModel GetSaveOverrideReason(IStateContainer pageState, IPatientPrivacy privacyLogic,string overrideText, string pageName)
        {
            PrivacyOverrideApiController data = new PrivacyOverrideApiController();
            PrivacyOverrideModel model = new PrivacyOverrideModel();
            if (pageState["PatientPrivacyRequestID"] != null && !string.IsNullOrWhiteSpace(overrideText.Trim()))
            {
                string patientID = data.PrivacyPatientID;
                string providerID = data.ProviderID;
                string physicianID = data.PhysicianId;
                string smid = data.TaskScriptMessageId;
                setRestrictedUserOverridden(pageState, "True");
                setPatientId(pageState, patientID);
                if (pageState["NewRx"] == null)
                {
                    data.savePrivacyReason(pageState, privacyLogic, patientID, ApiHelper.GetSessionUserID(pageState), overrideText);
                }
                else
                {
                    data.savePrivacyReason(pageState, privacyLogic, patientID, physicianID, overrideText);
                }

                model.Success = true;
                pageName = pageName.ToUpper();
                model.PageName = pageName;

                if (Constants.PageNames.APPROVE_SCRIPT_MESSAGE_PATIENT.ToUpper() == pageName)
                    model.RedirectURL = string.Concat(Constants.PageNames.APPROVE_SCRIPT_MESSAGE_PATIENT, "?smid=", smid, "&From=", Constants.PageNames.APPROVE_REFILL_TASK);
                if (Constants.PageNames.SELECT_PATIENT.ToUpper() == pageName)
                    model.RedirectURL = Constants.PageNames.SELECT_PATIENT;
                if (Constants.PageNames.DOC_REFILL_MENU.ToUpper() == pageName)
                    model.RedirectURL = HttpContext.Current.Request.UrlReferrer.ToString();
                if (Constants.PageNames.LIST_SEND_SCRIPTS.ToUpper() == pageName)
                    model.RedirectURL = Constants.PageNames.TASK_SCRIPT_LIST + "?PatID=" + HttpContext.Current.Server.UrlEncode(patientID) + "&ProvID=" + HttpContext.Current.Server.UrlEncode(providerID) + "&TaskType=" + Convert.ToInt32(Constants.PrescriptionTaskType.SEND_TO_ADMIN).ToString();
                if (Constants.PageNames.SELECT_PATIENT.ToUpper() == pageName)
                    model.RedirectURL = Constants.PageNames.SELECT_PATIENT;
                if (Constants.PageNames.REVIEW_HISTORY.ToUpper() == pageName)
                    model.RedirectURL = Constants.PageNames.REVIEW_HISTORY;
            }
            return model;
        }

        [HttpPost]
        public ApiResponse CheckIsRestrictedUser(PrivacyRequest request)
        {
            using (var timer = logger.StartTimer("CheckIsRestrictedUser"))
            {
                PrivacyOverrideApiController data = new PrivacyOverrideApiController();
                PrivacyOverrideModel model = new PrivacyOverrideModel();
                IStateContainer session = new StateContainer(HttpContext.Current.Session);
                var response = new ApiResponse();

                session["PrivacyPatientID"] = request.patientId;
                
                try
                {
                    if (checkIfUserRestricted(request.patientId, request.userId, ApiHelper.GetDBID(session), session))
                    {
                        model.IsRestrictedUser = true;
                    }
                    else
                    {
                        model.IsRestrictedUser = false;
                    }

                    //var selectedProvider = data.SelectedProviderID;

                    //if (!String.IsNullOrWhiteSpace(selectedProvider))
                    //{
                    //    if (checkIfUserRestricted(ApiHelper.GetSessionPatientId(session), selectedProvider, ApiHelper.GetDBID(session), session))
                    //        model.IsRestrictedUser = true;
                    //    else
                    //        model.IsRestrictedUser = false;
                    //}
                    //else
                    //{
                    //    model.RedirectURL = Constants.PageNames.REVIEW_HISTORY;
                    //}

                    session["IsRestrictedUser"] = model.IsRestrictedUser;

                    response.Payload = model;
                }
                catch (Exception ex)
                {
                    var errorMessage = ApiHelper.AuditException(ex.ToString(), session);
                    logger.Error("CheckIsRestrictedUser Exception: " + ex.ToString());

                    response.ErrorContext = new ErrorContextModel()
                    {
                        Error = ErrorTypeEnum.ServerError,
                        Message = errorMessage
                    };
                }

                timer.Message = string.Format("<Request>{0}</Request><Response>{1}</Response>", $"<SelectedProviderID>{data.SelectedProviderID.ToLogString()}</SelectedProviderID>", response.ToLogString());
                return response;
            }
        }

        public static bool checkIfUserRestricted(string patientID, string userID, ConnectionStringPointer dbID, IStateContainer pageState)
        {
            DataTable dtPatientPrivacyRequestID = new PatientPrivacy().GetPatientPrivacyRequestID(patientID, userID, dbID);
            if (dtPatientPrivacyRequestID.Rows.Count > 0 && dtPatientPrivacyRequestID.Rows[0]["ID"] != DBNull.Value)
            {
                Int32 PatientPrivacyRequestID = Convert.ToInt32(dtPatientPrivacyRequestID.Rows[0]["ID"].ToString());
                pageState["PatientPrivacyRequestID"] = PatientPrivacyRequestID.ToString();
                return true;
            }
            else
            {
                return false;
            }
        }

        private void savePrivacyReason(IStateContainer pageState, IPatientPrivacy privacyLogic, string patientID, string userID,string txtOverride)
        {

            privacyLogic.SavePatientPrivacyOverrideReason(
                Convert.ToInt32(pageState["PatientPrivacyRequestID"].ToString()), txtOverride.Trim(),
                (int)Allscripts.ePrescribe.Objects.AuditAction.PATIENT_RESTRICTION_OVERRIDDEN,
                (int)ePrescribeSvc.ePrescribeApplication.MainApplication, SessionLicenseID, userID,
                patientID,
                HttpContext.Current.Request.UserIpAddress(), AppCode.ApiHelper.GetDBID(pageState));
        }

        internal static void setRestrictedUserOverridden(IStateContainer state, string value)
        {
            state["RestrictedUserOverridden"] = value;
        }

        internal static void setPatientId(IStateContainer state, string value)
        {
            state["PATIENTID"] = value;
        }
        
    }
}