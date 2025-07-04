using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.Shared.Logging;
using eRxWeb.ServerModel;
using eRxWeb.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace eRxWeb.AppCode
{
    public class RedirectHelper
    {
        private static readonly ILoggerEx logger = LoggerEx.GetLogger();
        public static string GetRedirectToSPAUrl(string pageName, bool fromInside)
        {
            pageName = Constants.PageNames.UrlForRedirection(pageName);
            using (var timer = logger.StartTimer("Redirecting Spa"))
            {
                timer.Message = string.Format("<RedirectUrl>{0}</RedirectUrl><fromInside>{1}</fromInside>", pageName, fromInside.ToLogString());
                if (fromInside)
                {
                    if (Angular.PageNames.Contains(pageName.ToLower()))
                    {
                        // FORTIFY: Not considered an open re-direct as already redirecting to local page
                        return AngularStringUtil.CreateUrl(pageName);
                    }
                    else
                    {
                        // FORTIFY: Not considered an open re-direct as already redirecting to local page
                        return $"~/{pageName}";
                    }
                }
                else
                {
                    if (pageName.StartsWith("~/"))
                    {
                        pageName = pageName.Substring(1);//~/ is index of 1
                    }
                    // FORTIFY: Not considered an open re-direct as already redirecting to local page
                    return $"~/{Constants.PageNames.SPA_LANDING}?page={pageName}";
                }
            }
        }

        public static string GetRedirectToSelectPatientUrl(string queryString, SelectPatientComponentParameters selectPatientComponentParameters, IStateContainer session)
        {
            if ((queryString != null && queryString.Contains("StartOver=Y")) ||
                session.GetBooleanOrFalse(Constants.SessionVariables.AppComponentAlreadyInitialized))//since it is coming from Angular component)
            {
                //WHEN APP COMPONENT ALREADY EXISTS
                var componentParameters = new JavaScriptSerializer().Serialize(selectPatientComponentParameters);
                return $"{Constants.PageNames.REDIRECT_TO_ANGULAR}?componentName={ Constants.PageNames.SELECT_PATIENT }&componentParameters={componentParameters}";
            }
            else
            {
                return AngularStringUtil.CreateInitComponentUrl(Constants.PageNames.SELECT_PATIENT);
            }
        }

        public static string GetDefaultRedirectUrl(int sessionSiteID, IStateContainer session, string targetUrl, bool fromInside = true)
        {
            if (sessionSiteID < 0)
            {
                return $"~/{Constants.PageNames.SELECT_ACCOUNT_AND_SITE}";
            }

            bool ssoTaskMode = string.Compare(session.GetStringOrEmpty("SSOMode"), Constants.SSOMode.TASKMODE, true) == 0;
            bool epaTaskMode = string.Compare(session.GetStringOrEmpty("SSOMode"), Constants.SSOMode.EPAPATIENTLOCKDOWNTASKMODE, true) == 0;

            switch ((Constants.UserCategory)session["UserType"])
            {
                case Constants.UserCategory.PROVIDER:
                case Constants.UserCategory.PHYSICIAN_ASSISTANT:
                    session[Constants.SessionVariables.AppComponentAlreadyInitialized] = session[Constants.SessionVariables.AppComponentAlreadyInitialized] != null;
                    if (ssoTaskMode || epaTaskMode)
                        return GetRedirectToSPAUrl(Constants.PageNames.DOC_REFILL_MENU, fromInside);
                    else if ((!string.IsNullOrEmpty(targetUrl)) && targetUrl.Contains("ChangePassword.aspx"))
                    {
                        return GetRedirectToSPAUrl(targetUrl.Trim().ToString(), fromInside);
                    }
                    else
                    {
                        SelectPatientComponentParameters selectPatientComponentParameters = new SelectPatientComponentParameters
                        {
                            PatientId = session.GetStringOrEmpty(Constants.SessionVariables.PatientId)
                        };
                        return GetRedirectToSelectPatientUrl(null, selectPatientComponentParameters, session);
                    }
                case Constants.UserCategory.GENERAL_USER:
                case Constants.UserCategory.POB_LIMITED:
                    if (ssoTaskMode || epaTaskMode)
                    {
                        return GetRedirectToSPAUrl(Constants.PageNames.REPORTS, fromInside);
                    }
                    else
                    {
                        SelectPatientComponentParameters selectPatientComponentParameters = new SelectPatientComponentParameters
                        {
                            PatientId = session.GetStringOrEmpty(Constants.SessionVariables.PatientId)
                        };
                        return GetRedirectToSelectPatientUrl(null, selectPatientComponentParameters, session);
                    }
                default:
                    if (ssoTaskMode || epaTaskMode)
                    {
                        return GetRedirectToSPAUrl(Constants.PageNames.DOC_REFILL_MENU, fromInside);
                    }
                    else
                    {
                        SelectPatientComponentParameters selectPatientComponentParameters = new SelectPatientComponentParameters
                        {
                            PatientId = session.GetStringOrEmpty(Constants.SessionVariables.PatientId)
                        };
                        return GetRedirectToSelectPatientUrl(null, selectPatientComponentParameters, session);
                    }
            }
        }
    }
}