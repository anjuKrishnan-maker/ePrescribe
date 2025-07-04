using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.Objects.Registrant;
using Allscripts.ePrescribe.Shared.Logging;
using Allscripts.Impact;
using eRxWeb.AppCode.Interfaces;
using eRxWeb.ePrescribeSvc;
using eRxWeb.State;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using System.Web.Http.Filters;
using System.Web.SessionState;
using RegistrantDataInfo = Allscripts.ePrescribe.Objects.Registrant.RegistrantInfo;
using RegistrantInfo = eRxWeb.ePrescribeSvc.RegistrantInfo;

namespace eRxWeb.AppCode
{
    public class ApiAuthenticationProcessor
    {
        private ILoggerEx Logger { get; set; }

        public ApiAuthenticationProcessor()
        {
            Logger = LoggerEx.GetLogger();
        }

        public void SetUserAuthenticationContext(RegistrantDataInfo registrantInfo)
        {
            //Login user.
            var pageContext = new PageContext
            {
                Session = HttpContext.Current.Session,
                Request = HttpContext.Current.Request,
                Response = HttpContext.Current.Response
            };

            Authenticator logonProcessor = new Authenticator(Logger, HttpContext.Current.Session, new Login(pageContext));
            logonProcessor.SetRegistrantUserContext(registrantInfo);
        }

        public void SetUserAuthenticationContext(string userName, string password)
        {
            var pageContext = new PageContext
            {
                Session = HttpContext.Current.Session,
                Request = HttpContext.Current.Request,
                Response = HttpContext.Current.Response
            };

            Login login = new Login(pageContext);

            string redirectUrl = "~/" + Constants.PageNames.LOGIN.ToLower();
            string msg = string.Empty;
            string accountID = string.Empty;

            var authRequest = new AuthRequestDTO
            {
                DbId = Allscripts.ePrescribe.DatabaseSelector.ConnectionStringPointer.ERXDB_DEFAULT,
                IpAddress = HttpContext.Current.Request.UserIpAddress(),
                UserName = userName,
                IsPasswordResetSuccess = true
            };
            Authenticator authenticatorObj = new Authenticator(Logger, HttpContext.Current.Session, login);
            authenticatorObj.AuthenticateUser(authRequest,
                                            ref redirectUrl,
                                            ref msg,
                                            ref accountID,
                                            password: password);

            var userContextRequest = new UserContextRequestDTO
            {
                Request = HttpContext.Current.Request,
                Response = HttpContext.Current.Response,
                AccountID = accountID,
                UserName = userName,
            };
            new UserContextProcessor(Logger,
                HttpContext.Current.Session,
                login).SetUserContext(userContextRequest);

            var PageState = new StateContainer(HttpContext.Current.Session);
            PageState[Constants.SessionVariables.AutoLogin2nUser] = true;
            PageState[Constants.SessionVariables.RedirectUrl2n] = redirectUrl;
        }
    }

    public class PageContext : IPageState
    {
        public HttpSessionState Session { get; set; }

        public HttpRequest Request { get; set; }

        public HttpResponse Response { get; set; }
    }
}