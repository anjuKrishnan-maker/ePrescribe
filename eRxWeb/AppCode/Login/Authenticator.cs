using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.Data;
using Allscripts.ePrescribe.Shared.Logging;
using eRxWeb.AppCode.Interfaces;
using eRxWeb.ePrescribeSvc;
using eRxWeb.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using RegistrantInfo = Allscripts.ePrescribe.Objects.Registrant.RegistrantInfo;

namespace eRxWeb.AppCode
{
    public class Authenticator
    {
        private ILoggerEx Logger { get; set; }
        private IStateContainer Session { get; set; }
        private ILogin LoginProcessor { get; set; }
        private IEPSBroker ePSBroker { get; set; }


        public Authenticator(ILoggerEx loggerEx, HttpSessionState pageState, ILogin login, IEPSBroker ePSBroker = null)
        {
            Logger = loggerEx;
            Session = new StateContainer(pageState);
            LoginProcessor = login;
            this.ePSBroker = ePSBroker ?? new EPSBroker();
        }

        /// <param name="setContextOnValidShieldAuth">If passed true, will proceed to context setting if auth status is SHIELD_AUTH</param>
        /// <returns></returns>
        public AuthResponseDTO AuthenticateUser(AuthRequestDTO authRequest,
                                                ref string redirectUrl, ref string msg, ref string accountID,
                                                string password = "", string shieldIdentityToken = "",
                                                bool setContextOnValidShieldAuth = false, bool excludeLoggingCheck = false)
        {
            if (!excludeLoggingCheck)
            {
                LogEnabledChecker.CheckLoggingForUser(authRequest.UserName, LogEnabledChecker.UserAttributeType.UserName);
            }

            Logger.Debug("Cookies: " + HttpContext.Current.Request.Cookies.ToLogString());

            AuthenticateAndAuthorizeUserResponse authResponse;
            if (!authRequest.IsPasswordResetSuccess)
                authResponse = AuthenticateAuthorizeUser(authRequest.UserName, authRequest, password, shieldIdentityToken);
            else//Default authentication cases.
                authResponse = ReattemptAuthentication(authRequest, password);

            var authenticationResponse = ProcessAuthentication(authRequest, authResponse, ref redirectUrl, ref msg, ref accountID, setContextOnValidShieldAuth);
            authenticationResponse.AuthenticateAndAuthorizeUserResponse = authResponse;

            return authenticationResponse;
        }

        internal AuthenticateAndAuthorizeUserResponse ReattemptAuthentication(AuthRequestDTO authRequest, string password)
        {
            RetryHelper retryExecutor;
            Func<AuthenticateAndAuthorizeUserResponse> authenticateAuthorizeUser;
            Func<AuthenticateAndAuthorizeUserResponse, bool> exitStrategy;

            RetryStrategy retryStrategy = new RetryStrategy(maxRetries: 3);
            retryExecutor = new RetryHelper(retryStrategy);
            authenticateAuthorizeUser = () =>
            {
                return AuthenticateAuthorizeUser(authRequest.UserName, authRequest, password);
            };
            exitStrategy = (authUserResponse) =>
            {
                bool isSuccess = authUserResponse.AuthenticationStatus == AuthenticationStatuses.SHIELD_AUTH &&
                 !IsPasswordExpired(authRequest.UserName, authUserResponse.AuthenticatedShieldUsers);
                return isSuccess;
            };

            return retryExecutor.Retry(authenticateAuthorizeUser, exitStrategy);
        }


        /// <param name="setContextOnValidShieldAuth">If passed true, will proceed to context setting if auth status is SHIELD_AUTH</param>
        /// <returns></returns>
        internal AuthResponseDTO ProcessAuthentication(AuthRequestDTO authRequest, AuthenticateAndAuthorizeUserResponse authResponse,
                                                      ref string redirectUrl, ref string msg, ref string accountID,
                                                      bool setContextOnValidShieldAuth = false)
        {
            AuthResponseDTO authenticationResponse = new AuthResponseDTO();
            switch (authResponse.AuthenticationStatus)
            {
                case AuthenticationStatuses.SHIELD_AUTH:
                    {
                        Logger.Debug("SHIELD_AUTH");

                        Session["IsLicenseShieldEnabled"] = true;

                        //this is used to tie the token refresh audit events to the original login
                        Session["AuditLogUserLoginID"] = authResponse.AuditLogID;

                        //when password expires, whatever case SHIELD_AUTH is being set.
                        authenticationResponse.IsShieldPasswordExpired = IsPasswordExpired(authRequest.UserName, authResponse.AuthenticatedShieldUsers);

                        if (authenticationResponse.IsShieldPasswordExpired)
                            break;

                        authenticationResponse.HaveShieldAccountAndPendingRestration = authResponse.ValidRegistrantLogin && authResponse.ValidShieldLogin;
                        if (!setContextOnValidShieldAuth && authenticationResponse.HaveShieldAccountAndPendingRestration)
                            break;

                        //set this to auth token to with the first Shield user's token.
                        //if there are multiple, the auth token will get properly set on SelectAccountAndSite.aspx
                        LoginProcessor.SetAuthenticationCookieForShieldUser(authResponse.AuthenticatedShieldUsers[0]);
                        LoginProcessor.SetAccountInfo(authResponse, ref redirectUrl, ref msg);

                        if (string.IsNullOrEmpty(msg))
                        {
                            Session["LastLoginDateUTC"] = authResponse.AuthenticatedShieldUsers[0].LastLoginDateUTC;
                            LoginProcessor.UpdateUserLastLoginDate(authResponse);
                        }

                        if (!string.IsNullOrEmpty(Session.GetStringOrEmpty("LICENSEID")))
                            ePSBroker.AddILearnUser(authRequest.UserName, Session.GetStringOrEmpty("LICENSEID"),
                                authResponse.AuthenticatedShieldUsers[0]?.FirstName, authResponse.AuthenticatedShieldUsers[0]?.LastName,
                                authRequest.DbId);

                        //TODO: ePA team needs to figure out which AuthenticatedShieldUser to choose here
                        accountID = authResponse.AuthenticatedShieldUsers[0].AccountID;

                        break;
                    }
                case AuthenticationStatuses.SHIELD_AUTH_REGISTRANT:
                    Logger.Debug("SHIELD_AUTH_REGISTRANT");
                    var registrantData = authResponse.AuthenticatedRegistrant;
                    SetRegistrantUserContext(new RegistrantInfo
                    {
                        ShieldUserName = registrantData.ShieldUserName,
                        RegistrantId = registrantData.RegistrantId,
                        LevelOfAssurance = registrantData.LevelOfAssurance,
                        ShieldObjectId = registrantData.ShieldObjectId
                    });
                    redirectUrl = "~/" + Constants.PageNames.REGISTER_WELCOME;
                    authenticationResponse.IsRegistrantUser = true;
                    Session[Constants.SessionVariables.ShieldIdentityToken] = authResponse.IdentToken;
                    Session[Constants.SessionVariables.RegistrationEnterpriseClientID] = registrantData.EnterpriseClientID;
                    break;
                case AuthenticationStatuses.NOT_AUTHORIZED:
                case AuthenticationStatuses.AUTH_SUCCESSFUL_BUT_NOT_ALLOWED:
                    {
                        Logger.Debug("NOT_AUTHORIZED, AUTH_SUCCESSFUL_BUT_NOT_ALLOWED");

                        authenticationResponse.LoginErrorMessage = authResponse.DisplayMessage;
                        break;
                    }
                default:
                    {
                        authenticationResponse.LoginErrorMessage = "Unrecognized credentials.  Please contact support.";
                        break;
                    }
            }
            authenticationResponse.HasLoggedIn = string.IsNullOrWhiteSpace(authenticationResponse.LoginErrorMessage) &&
                                                 string.IsNullOrWhiteSpace(msg) &&
                                                 !authenticationResponse.IsShieldPasswordExpired;
            return authenticationResponse;
        }

        private bool IsPasswordExpired(string userName, AuthenticatedShieldUser[] authenticatedShieldUsers)
        {
            bool isShieldPasswordExpired = false;
            //authenticatedShieldUsers will be there always. Even if the password expired a user will be set otherwise the users will be there.
            foreach (AuthenticatedShieldUser authenticatedShieldUser in authenticatedShieldUsers)
            {
                if (authenticatedShieldUser.IsPasswordExpired)
                {
                    Logger.Debug("Auth user withusername: {0}. Password Expired", userName);
                    isShieldPasswordExpired = true;
                    break;
                }
            }
            return isShieldPasswordExpired;
        }

        internal AuthenticateAndAuthorizeUserResponse AuthenticateAuthorizeUser(string userName, AuthRequestDTO authRequest, string password = "", string shieldIdentityToken = "")
        {
            AuthenticateAndAuthorizeUserResponse authResponse = new AuthenticateAndAuthorizeUserResponse();
            if (!string.IsNullOrWhiteSpace(password))
                authResponse = ePSBroker.AuthenticateAndAuthorizeUser(userName.Trim(), password.Trim(), authRequest.IpAddress);
            if (!string.IsNullOrWhiteSpace(shieldIdentityToken))
                authResponse = ePSBroker.AuthorizeUser(userName.Trim(), shieldIdentityToken, authRequest.IpAddress);
            return authResponse;
        }

        public void SetRegistrantUserContext(RegistrantInfo registrantInformation)
        {
            LoginProcessor.SetAuthenticationCookie(registrantInformation.ShieldUserName);

            Session[Constants.SessionVariables.RegistrantInfo] = registrantInformation;
        }
    }

}