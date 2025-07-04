using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.Objects;
using Allscripts.ePrescribe.Shared.Logging;
using Allscripts.Impact;
using eRxWeb.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;

namespace eRxWeb.AppCode
{
    public class UserContextProcessor
    {

        private ILoggerEx logger { get; set; }
        private IStateContainer PageState { get; set; }

        private Login _login { get; set; }

        public UserContextProcessor(ILoggerEx loggerEx, HttpSessionState pageState, Login login)
        {
            logger = loggerEx;
            PageState = new StateContainer(pageState);
            _login = login;
        }
        public UserContextResponseDTO SetUserContext(ApplicationLicense SessionLicense, UserContextRequestDTO userContext, bool isRegistrantUser = false)
        {
            UserContextResponseDTO contextSetProcessor = new UserContextResponseDTO();
            if (userContext.HasLoggedIn)
            {
                SetUserContext(userContext);

                if (!isRegistrantUser)
                {
                    var serviceAlertsToBeShown = Announcements
                        .CheckForServiceAlertsToShow(userContext.AccountID, PageState.GetString("CURRENT_SITE_ZONE", "Pacific Standard Time"),
                        userContext.Request.UserAgent, SessionLicense.EnterpriseClient.ID, _login, new BrowserUtil(),
                        Convert.ToInt32(SessionLicense.DeluxePricingStructure));

                    if (!serviceAlertsToBeShown.Any())
                        logger.Debug("Login: No Service Alerts To Be Displayed");
                    else
                        PageState[Constants.SessionVariables.ServiceAlerts] = serviceAlertsToBeShown;


                    contextSetProcessor.HasServiceAlertsToShow = serviceAlertsToBeShown.Any();
                }

            }
            else if (userContext.Message.Trim().Length > 0)
            {
                //We should swallow those SQL exceptions that appear on login after a database connection error
                if (userContext.Message.StartsWith("SYSERROR") || userContext.Message.Contains("Sql") || PageState["CurrentErrorID"] != null)
                {
                    contextSetProcessor.LoginErrorMessage = "We apologize, but a serious error has occurred.";

                    if (PageState["CurrentErrorID"] != null)
                    {
                        contextSetProcessor.LoginErrorMessage = $"{contextSetProcessor.LoginErrorMessage} Exception reference ID ={PageState["CurrentErrorID"].ToString() }";
                    }
                }
                else
                {
                    contextSetProcessor.LoginErrorMessage = userContext.Message;
                }
                PageState.Abandon();
                FormsAuthentication.SignOut();
            }
            return contextSetProcessor;
        }

        public void SetUserContext(UserContextRequestDTO userContext)
        {
            HttpCookie eRxNowCookie;
            if (userContext.Request.Cookies["eRxNowCookie"] != null)
            {
                eRxNowCookie = userContext.Request.Cookies.Get("eRxNowCookie");
            }
            else
            {
                eRxNowCookie = new HttpCookie("eRxNowCookie");
            }

            if (userContext.RememberLogin)
            {
                eRxNowCookie.Values.Remove("UserName");
                eRxNowCookie.Values.Add("UserName", userContext.UserName);
                eRxNowCookie.Expires = DateTime.Today.AddMonths(6);
            }
            else
            {
                eRxNowCookie.Values.Remove("UserName");
                eRxNowCookie.Expires = DateTime.Today.AddMonths(6);
            }

            eRxNowCookie.Values.Remove("Remember");
            eRxNowCookie.Values.Add("Remember", userContext.RememberLogin.ToString());

            //Replace the eRxNow cookie
            userContext.Response.Cookies.Remove(eRxNowCookie.Name);
            userContext.Response.Cookies.Add(eRxNowCookie);
        }
    }

    public class UserContextRequestDTO
    {
        public string UserName { get; set; }
        public string AccountID { get; set; }
        public HttpRequest Request { get; set; }
        public HttpResponse Response { get; set; }
        public bool HasLoggedIn { get; set; }
        public bool RememberLogin { get; set; }
        public string Message { get; set; }
    }

    public class UserContextResponseDTO
    {
        public string LoginErrorMessage { get; set; }
        public bool HasServiceAlertsToShow { get; set; }
    }
}