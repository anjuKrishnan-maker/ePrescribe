using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.Objects.Registrant;
using Allscripts.ePrescribe.Shared.Logging;
using eRxWeb.AppCode;
using System;
using System.Collections.Generic;
using System.Web.Security;
using static Allscripts.ePrescribe.Common.Constants;

namespace eRxWeb
{
    public partial class RegistrantMediator : LoginBasePage
    {
        private static ILoggerEx logger = LoggerEx.GetLogger();
        protected void Page_Init(object sender, EventArgs e)
        {
            Login _login = new Login(this);
            RegistrantInfo registrantInfo = PageState.Cast(SessionVariables.RegistrantInfo, new RegistrantInfo());
            string shieldIdentityToken = PageState.GetStringOrEmpty(SessionVariables.ShieldIdentityToken);
            string redirectUrl = "~/" + PageNames.LOGIN.ToLower();
            string msg = string.Empty;
            string accountID = string.Empty;
            string userName = registrantInfo.ShieldUserName;
            bool isRegistrantPracticeUpdated = PageState.Cast(SessionVariables.IsRegistrantPracticeUpdated, false);
            if (!string.IsNullOrWhiteSpace(userName) &&
                !string.IsNullOrWhiteSpace(shieldIdentityToken) &&
                isRegistrantPracticeUpdated)
            {
                var authRequest = new AuthRequestDTO { DbId = DBID, IpAddress = Page.Request.UserIpAddress(), UserName = userName };
                //Note: setContextOnValidShieldAuth:- is specifically set true because.. If the registration was set then the context on SHIELD_AUTH would havn't been set for the user. 
                AuthResponseDTO authResponse = new Authenticator(logger, Session, _login)
                                                    .AuthenticateUser(authRequest, ref redirectUrl, ref msg, ref accountID,
                                                    shieldIdentityToken: shieldIdentityToken, setContextOnValidShieldAuth: true);


                bool hasLoggedIn = authResponse.HasLoggedIn;
                var userContextRequest = new UserContextRequestDTO
                {
                    HasLoggedIn = hasLoggedIn,
                    RememberLogin = false,
                    Request = Request,
                    Response = Response,
                    AccountID = accountID,
                    UserName = userName,
                    Message = msg
                };

                UserContextResponseDTO userContextSet = new UserContextProcessor(logger, Session, _login)
                                                                    .SetUserContext(SessionLicense, userContextRequest, authResponse.IsRegistrantUser);
                PageState.Remove(SessionVariables.IsRegistrantPracticeUpdated);
            }

            if (!isRegistrantPracticeUpdated)
            {
                PageState.Abandon(); //Not doing user logout specifically but full session abandon.
                FormsAuthentication.SignOut();
            }

            RedirectUser(redirectUrl);
        }
    }
}