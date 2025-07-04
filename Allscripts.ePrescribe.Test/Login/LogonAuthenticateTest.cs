using System.Web.UI;
using eRxWeb.AppCode;
using eRxWeb.State;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using Allscripts.ePrescribe.Shared.Logging;
using System.Web.SessionState;
using System.Web;
using eRxWeb.AppCode.Interfaces;
using eRxWeb.ePrescribeSvc;
using eRxWeb;
using System;
using Rhino.Mocks.Constraints;
using Allscripts.ePrescribe.Common;

namespace Allscripts.ePrescribe.Test.Authenticate
{
    /// <summary>
    /// Not done : 
    /// * Should_set_enable_logging_in_session_context
    /// </summary>
    [TestClass]
    public class LogonAuthenticateTest
    {
        public eRxWeb.AppCode.Authenticator LogonProcessor { get; set; }
        private IStateContainer Session { get; set; }
        private ILoggerEx Logger { get; set; }

        private HttpSessionState HttpSessionState { get; set; }

        private Page PageMock { get; set; }

        private ILogin Login { get; set; }

        private IEPSBroker ePSBroker { get; set; }

        [TestInitialize]
        public void InitializeLogon()
        {
            HttpContext.Current = MockHelpers.FakeHttpContext();

            Logger = MockRepository.GenerateMock<ILoggerEx>();

            Session = MockRepository.GenerateMock<IStateContainer>();

            PageMock = MockRepository.GenerateStub<Page>();
            PageMock.Stub((x) => x.Session).Return(HttpSessionState);

            Login = MockRepository.GenerateMock<ILogin>();

            ePSBroker = MockRepository.GenerateStub<IEPSBroker>();
        }


        [TestMethod]
        public void Should_call_authenticate_authorize_when_password_present()
        {
            //arrange            
            ePSBroker.Stub(x => x.AuthenticateAndAuthorizeUser("user", "password", "ipaddress"))
                     .IgnoreArguments()
                     .Return(new AuthenticateAndAuthorizeUserResponse { Success = true });

            LogonProcessor = new Authenticator(Logger, HttpSessionState, Login, ePSBroker);

            //act
            AuthenticateAndAuthorizeUserResponse userResponse = LogonProcessor.AuthenticateAuthorizeUser("user", new AuthRequestDTO(), password: "password");

            //assert
            Assert.IsNotNull(userResponse);
            Assert.IsTrue(userResponse.Success);
            ePSBroker.AssertWasCalled(_ => _.AuthenticateAndAuthorizeUser(Arg<string>.Is.Anything, Arg<string>.Is.Anything, Arg<string>.Is.Anything));
        }

        [TestMethod]
        public void Should_call_authorize_when_shieldtokenid_present()
        {
            //arrange            
            ePSBroker.Stub(x => x.AuthorizeUser("user", "shieldIdentityToken", "ipaddress"))
                     .IgnoreArguments()
                     .Return(new AuthenticateAndAuthorizeUserResponse { Success = false });//Just to ensure it is not a repetition

            LogonProcessor = new Authenticator(Logger, HttpSessionState, Login, ePSBroker);

            //act
            AuthenticateAndAuthorizeUserResponse userResponse = LogonProcessor.AuthenticateAuthorizeUser("user", new AuthRequestDTO(), shieldIdentityToken: "shieldIdentityToken");

            //assert
            Assert.IsNotNull(userResponse);
            Assert.IsFalse(userResponse.Success);
            ePSBroker.AssertWasCalled(_ => _.AuthorizeUser(Arg<string>.Is.Anything, Arg<string>.Is.Anything, Arg<string>.Is.Anything));
        }

        [TestMethod]
        public void Should_fail_when_no_password_nor_shieldtokenid_present()
        {
            //arrange            
            ePSBroker.Stub(x => x.AuthenticateAndAuthorizeUser("user", "password", "ipaddress"))
                   .IgnoreArguments()
                   .Return(new AuthenticateAndAuthorizeUserResponse { Success = true });
            ePSBroker.Stub(x => x.AuthorizeUser("user", "shieldIdentityToken", "ipaddress"))
                     .IgnoreArguments()
                     .Return(new AuthenticateAndAuthorizeUserResponse { Success = false });//Just to ensure it is not a repetition

            LogonProcessor = new Authenticator(Logger, HttpSessionState, Login, ePSBroker);

            //act
            AuthenticateAndAuthorizeUserResponse userResponse = LogonProcessor.AuthenticateAuthorizeUser("user", new AuthRequestDTO());

            //assert
            Assert.IsFalse(userResponse.Success);
            ePSBroker.AssertWasNotCalled(_ => _.AuthorizeUser(Arg<string>.Is.Anything, Arg<string>.Is.Anything, Arg<string>.Is.Anything));
            ePSBroker.AssertWasNotCalled(_ => _.AuthenticateAndAuthorizeUser(Arg<string>.Is.Anything, Arg<string>.Is.Anything, Arg<string>.Is.Anything));
        }

        #region SHIELD_AUTH

        /// <summary>
        /// SHIELD_AUTH : is the status code by shield on succesful login
        /// </summary>
        [TestMethod]
        public void Should_enable_logging_if_autheticated_SHIELD_AUTH()
        {
            //arrange
            string redirectUrl = "";
            string msg = "";
            string accountId = "";
            var authenticateAndAuthorizeUserResponse = new AuthenticateAndAuthorizeUserResponse
            {
                AuthenticationStatus = AuthenticationStatuses.SHIELD_AUTH,
                AuthenticatedShieldUsers = new AuthenticatedShieldUser[] { new AuthenticatedShieldUser { IsPasswordExpired = true } },

            };
            Logger.Stub(x => x.Debug(""))
                 .IgnoreArguments();
            LogonProcessor = new Authenticator(Logger, HttpContext.Current.Session, Login, ePSBroker);

            //act
            var authenticationProcessResponse = LogonProcessor.ProcessAuthentication(new AuthRequestDTO(), authenticateAndAuthorizeUserResponse,
                                                                                     ref redirectUrl, ref msg, ref accountId);

            //assert
            Logger.AssertWasCalled(_ => _.Debug(Arg<string>.Is.Anything));
        }

        [TestMethod]
        public void Should_set_in_session_IsLicenseShieldEnabled_AuditLogUserLoginID_if_autheticated_SHIELD_AUTH()
        {
            //arrange
            string redirectUrl = "";
            string msg = "";
            string accountId = "";
            var authenticateAndAuthorizeUserResponse = new AuthenticateAndAuthorizeUserResponse
            {
                AuthenticationStatus = AuthenticationStatuses.SHIELD_AUTH,
                AuthenticatedShieldUsers = new AuthenticatedShieldUser[] { new AuthenticatedShieldUser { IsPasswordExpired = true } },
                AuditLogID = "aaa"
            };
            Logger.Stub(x => x.Debug(""))
                 .IgnoreArguments();
            LogonProcessor = new Authenticator(Logger, HttpContext.Current.Session, Login, ePSBroker);

            //act
            var authenticationProcessResponse = LogonProcessor.ProcessAuthentication(new AuthRequestDTO(), authenticateAndAuthorizeUserResponse,
                                                                                     ref redirectUrl, ref msg, ref accountId);

            //assert
            var isLicenseShieldEnabled = HttpContext.Current.Session["IsLicenseShieldEnabled"];
            var auditloginId = HttpContext.Current.Session["AuditLogUserLoginID"];

            Assert.AreEqual(isLicenseShieldEnabled, true);
            Assert.AreEqual(auditloginId, "aaa");
        }

        [TestMethod]
        public void Should_set_IsShieldPasswordExpired_and_log_if_autheticated_SHIELD_AUTH_password_expired()
        {

            //arrange
            string redirectUrl = "";
            string msg = "";
            string accountId = "";
            var authenticateAndAuthorizeUserResponseWithOneExpired = new AuthenticateAndAuthorizeUserResponse
            {
                AuthenticationStatus = AuthenticationStatuses.SHIELD_AUTH,
                AuthenticatedShieldUsers = new AuthenticatedShieldUser[] { new AuthenticatedShieldUser { IsPasswordExpired = false },
                                                                           new AuthenticatedShieldUser { IsPasswordExpired = false },
                                                                           new AuthenticatedShieldUser { IsPasswordExpired = true }}
            };
            var authenticateAndAuthorizeUserResponseWithNoneExpired = new AuthenticateAndAuthorizeUserResponse
            {
                AuthenticationStatus = AuthenticationStatuses.SHIELD_AUTH,
                AuthenticatedShieldUsers = new AuthenticatedShieldUser[] { new AuthenticatedShieldUser { IsPasswordExpired = false },
                                                                           new AuthenticatedShieldUser { IsPasswordExpired = false } }
            };
            Logger.Stub(x => x.Debug(""))
                 .IgnoreArguments();
            LogonProcessor = new Authenticator(Logger, HttpContext.Current.Session, Login, ePSBroker);

            //act
            var authenticationProcessResponse = LogonProcessor.ProcessAuthentication(new AuthRequestDTO(), authenticateAndAuthorizeUserResponseWithOneExpired,
                                                                                     ref redirectUrl, ref msg, ref accountId);

            //assert
            Assert.AreEqual(authenticationProcessResponse.IsShieldPasswordExpired, true);
            Logger.AssertWasCalled(_ => _.Debug(Arg<string>.Is.Anything), options => options.Repeat.Times(1));

        }

        [TestMethod]
        public void Should_flag_login_false_if_autheticated_SHIELD_AUTH_but_password_expired()
        {
            //arrange
            string redirectUrl = "";
            string msg = "";
            string accountId = "";
            var authenticateAndAuthorizeUserResponseWithOneExpired = new AuthenticateAndAuthorizeUserResponse
            {
                AuthenticationStatus = AuthenticationStatuses.SHIELD_AUTH,
                AuthenticatedShieldUsers = new AuthenticatedShieldUser[] { new AuthenticatedShieldUser { IsPasswordExpired = true } }
            };
            var authenticateAndAuthorizeUserResponseWithNoneExpired = new AuthenticateAndAuthorizeUserResponse
            {
                AuthenticationStatus = AuthenticationStatuses.SHIELD_AUTH,
                AuthenticatedShieldUsers = new AuthenticatedShieldUser[] { new AuthenticatedShieldUser { IsPasswordExpired = false },
                                                                           new AuthenticatedShieldUser { IsPasswordExpired = false } }
            };
            Logger.Stub(x => x.Debug(""))
                 .IgnoreArguments();
            LogonProcessor = new Authenticator(Logger, HttpContext.Current.Session, Login, ePSBroker);

            //act
            var authenticationProcessResponse = LogonProcessor.ProcessAuthentication(new AuthRequestDTO(), authenticateAndAuthorizeUserResponseWithOneExpired,
                                                                                     ref redirectUrl, ref msg, ref accountId);

            //assert
            Assert.AreEqual(authenticationProcessResponse.HasLoggedIn, false);
            Assert.AreEqual(authenticationProcessResponse.IsShieldPasswordExpired, true);
            Assert.AreEqual(authenticationProcessResponse.HaveShieldAccountAndPendingRestration, false);
        }

        [TestMethod]
        public void Should_Not_Set_Authentication_Cookie_And_Not_Set_Account_Info_For_Shield_User_when_SHIELD_AUTH_password_expired()
        {
            //arrange
            string redirectUrl = "";
            string msg = "";
            string accountId = "";
            var authenticateAndAuthorizeUserResponseWithNoneExpired = new AuthenticateAndAuthorizeUserResponse
            {
                AuthenticationStatus = AuthenticationStatuses.SHIELD_AUTH,
                AuthenticatedShieldUsers = new AuthenticatedShieldUser[] { new AuthenticatedShieldUser { IsPasswordExpired = true } }
            };
            Login.Stub(x => x.SetAuthenticationCookieForShieldUser(new AuthenticatedShieldUser()))
                 .IgnoreArguments();
            Login.Stub(x => x.SetAccountInfo(null, ref redirectUrl, ref msg))
                 .IgnoreArguments();

            LogonProcessor = new Authenticator(Logger, HttpContext.Current.Session, Login, ePSBroker);

            //act
            var authenticationProcessResponseNoneExpired = LogonProcessor.ProcessAuthentication(new AuthRequestDTO(), authenticateAndAuthorizeUserResponseWithNoneExpired,
                                                                                     ref redirectUrl, ref msg, ref accountId);

            //assert            
            Login.AssertWasNotCalled(_ => _.SetAuthenticationCookieForShieldUser(Arg<AuthenticatedShieldUser>.Is.Anything));
            Login.AssertWasNotCalled(_ => _.SetAccountInfo(Arg<AuthenticateAndAuthorizeUserResponse>.Is.Anything,
                                                        ref Arg<string>.Ref(Is.Equal("input"), "output").Dummy,
                                                        ref Arg<string>.Ref(Is.Equal("input"), "output").Dummy), (x) => x.IgnoreArguments());
        }

        #region SHIELD_AUTH_Password_Not_Expired
        /// <summary>
        /// Login.cs : SetAccountInfo should be considering the unit test for what it does
        /// This will just ensure session for the following: 
        ///     AuthenticatedShieldUsers
        ///     AuthenticatedShieldUsersCount
        /// Guaranteeing the method is entered into.
        /// </summary>
        [TestMethod]
        public void Should_Set_Authentication_Cookie_And_Set_Account_Info_For_Shield_User_when_SHIELD_AUTH_password_not_expired()
        {
            //arrange
            string redirectUrl = "";
            string msg = "";
            string accountId = "";
            var authenticateAndAuthorizeUserResponseWithNoneExpired = new AuthenticateAndAuthorizeUserResponse
            {
                AuthenticationStatus = AuthenticationStatuses.SHIELD_AUTH,
                AuthenticatedShieldUsers = new AuthenticatedShieldUser[] { new AuthenticatedShieldUser { IsPasswordExpired = false } }
            };
            Login.Stub(x => x.SetAuthenticationCookieForShieldUser(new AuthenticatedShieldUser()))
                 .IgnoreArguments();
            Login.Stub(x => x.SetAccountInfo(null, ref redirectUrl, ref msg))
                 .IgnoreArguments();

            LogonProcessor = new Authenticator(Logger, HttpContext.Current.Session, Login, ePSBroker);

            //act
            var authenticationProcessResponseNoneExpired = LogonProcessor.ProcessAuthentication(new AuthRequestDTO(), authenticateAndAuthorizeUserResponseWithNoneExpired,
                                                                                     ref redirectUrl, ref msg, ref accountId);

            //assert            
            Login.AssertWasCalled(_ => _.SetAuthenticationCookieForShieldUser(Arg<AuthenticatedShieldUser>.Is.Anything));
            Login.AssertWasCalled(_ => _.SetAccountInfo(Arg<AuthenticateAndAuthorizeUserResponse>.Is.Anything,
                                                        ref Arg<string>.Ref(Is.Equal("input"), "output").Dummy,
                                                        ref Arg<string>.Ref(Is.Equal("input"), "output").Dummy), (x) => x.IgnoreArguments());
        }

        [TestMethod]
        public void Should_Set_Login_Time_For_Shield_User_when_no_error_on_setting_account_info_and_SHIELD_AUTH_password_not_expired()
        {
            //arrange
            string redirectUrl = "";
            string msg = "";
            string accountId = "";
            var currentDate = DateTime.Now;
            var authenticateAndAuthorizeUserResponseWithNoneExpired = new AuthenticateAndAuthorizeUserResponse
            {
                AuthenticationStatus = AuthenticationStatuses.SHIELD_AUTH,
                AuthenticatedShieldUsers = new AuthenticatedShieldUser[] { new AuthenticatedShieldUser { IsPasswordExpired = false,
                    LastLoginDateUTC = currentDate} }
            };
            Login.Stub(x => x.UpdateUserLastLoginDate(new AuthenticateAndAuthorizeUserResponse()))
                 .IgnoreArguments();

            LogonProcessor = new Authenticator(Logger, HttpContext.Current.Session, Login, ePSBroker);

            //act
            var authenticationProcessResponseNoneExpired = LogonProcessor.ProcessAuthentication(new AuthRequestDTO(), authenticateAndAuthorizeUserResponseWithNoneExpired,
                                                                                     ref redirectUrl, ref msg, ref accountId);
            var lastLoginDate = HttpContext.Current.Session["LastLoginDateUTC"];

            //assert            
            Login.AssertWasCalled(_ => _.UpdateUserLastLoginDate(Arg<AuthenticateAndAuthorizeUserResponse>.Is.Anything));
            Assert.AreEqual(lastLoginDate, currentDate);
        }

        //Note: Should it if there is an error in auth context setting?
        [TestMethod]
        public void Should_AddILearnUser_For_Shield_User_when_Session_LICENSEID_Is_Not_Empty_And_SHIELD_AUTH_password_not_expired()
        {
            //arrange
            string redirectUrl = "";
            string msg = "";
            string accountId = "";
            var currentDate = DateTime.Now;
            var authenticateAndAuthorizeUserResponseWithNoneExpired = new AuthenticateAndAuthorizeUserResponse
            {
                AuthenticationStatus = AuthenticationStatuses.SHIELD_AUTH,
                AuthenticatedShieldUsers = new AuthenticatedShieldUser[] { new AuthenticatedShieldUser { IsPasswordExpired = false } }
            };

            ePSBroker.Stub(x => x.AddILearnUser(Arg<string>.Is.Anything, Arg<string>.Is.Anything,
                                                Arg<string>.Is.Anything, Arg<string>.Is.Anything, Arg<Allscripts.ePrescribe.DatabaseSelector.ConnectionStringPointer>.Is.Anything))
                    .IgnoreArguments();

            HttpContext.Current.Session["LICENSEID"] = "aaa";
            LogonProcessor = new Authenticator(Logger, HttpContext.Current.Session, Login, ePSBroker);

            //act
            var authenticationProcessResponseNoneExpired = LogonProcessor.ProcessAuthentication(new AuthRequestDTO(), authenticateAndAuthorizeUserResponseWithNoneExpired,
                                                                                     ref redirectUrl, ref msg, ref accountId);

            //assert            
            ePSBroker.AssertWasCalled(_ => _.AddILearnUser(Arg<string>.Is.Anything, Arg<string>.Is.Anything, Arg<string>.Is.Anything,
                                               Arg<string>.Is.Anything, Arg<Allscripts.ePrescribe.DatabaseSelector.ConnectionStringPointer>.Is.Anything),
                         (x) => x.IgnoreArguments());
        }
        //Didnt trust the rhino mock AssertWas called :) 
        [TestMethod]
        public void Should_NotAddILearnUser_For_Shield_User_when_Session_LICENSEID_Is_Empty_And_SHIELD_AUTH_password_not_expired()
        {
            //arrange
            string redirectUrl = "";
            string msg = "";
            string accountId = "";
            var currentDate = DateTime.Now;
            var authenticateAndAuthorizeUserResponseWithNoneExpired = new AuthenticateAndAuthorizeUserResponse
            {
                AuthenticationStatus = AuthenticationStatuses.SHIELD_AUTH,
                AuthenticatedShieldUsers = new AuthenticatedShieldUser[] { new AuthenticatedShieldUser { IsPasswordExpired = false } }
            };

            ePSBroker.Stub(x => x.AddILearnUser(Arg<string>.Is.Anything, Arg<string>.Is.Anything,
                                                Arg<string>.Is.Anything, Arg<string>.Is.Anything, Arg<Allscripts.ePrescribe.DatabaseSelector.ConnectionStringPointer>.Is.Anything))
                    .IgnoreArguments();

            HttpContext.Current.Session["LICENSEID"] = "";
            LogonProcessor = new Authenticator(Logger, HttpContext.Current.Session, Login, ePSBroker);

            //act
            var authenticationProcessResponseNoneExpired = LogonProcessor.ProcessAuthentication(new AuthRequestDTO(), authenticateAndAuthorizeUserResponseWithNoneExpired,
                                                                                     ref redirectUrl, ref msg, ref accountId);

            //assert            
            ePSBroker.AssertWasNotCalled(_ => _.AddILearnUser(Arg<string>.Is.Anything, Arg<string>.Is.Anything, Arg<string>.Is.Anything,
                                               Arg<string>.Is.Anything, Arg<Allscripts.ePrescribe.DatabaseSelector.ConnectionStringPointer>.Is.Anything),
                         (x) => x.IgnoreArguments());
        }

        public void Should_Not_Set_Authentication_Cookie_And_Not_Set_Account_Info_For_Shield_User_when_SetContextOnValidShieldAuth_Set_False()
        {
            //arrange
            string redirectUrl = "";
            string msg = "";
            string accountId = "";
            var authenticateAndAuthorizeUserResponseWithNoneExpired = new AuthenticateAndAuthorizeUserResponse
            {
                AuthenticationStatus = AuthenticationStatuses.SHIELD_AUTH,
                AuthenticatedShieldUsers = new AuthenticatedShieldUser[] { new AuthenticatedShieldUser { IsPasswordExpired = false } }
            };
            Login.Stub(x => x.SetAuthenticationCookieForShieldUser(new AuthenticatedShieldUser()))
                 .IgnoreArguments();
            Login.Stub(x => x.SetAccountInfo(null, ref redirectUrl, ref msg))
                 .IgnoreArguments();

            LogonProcessor = new Authenticator(Logger, HttpContext.Current.Session, Login, ePSBroker);

            //act
            var authenticationProcessResponseSetContextOnValidShieldAuth = LogonProcessor.ProcessAuthentication(new AuthRequestDTO(), authenticateAndAuthorizeUserResponseWithNoneExpired,
                                                                                     ref redirectUrl, ref msg, ref accountId, setContextOnValidShieldAuth: false);

            //assert            
            Login.AssertWasNotCalled(_ => _.SetAuthenticationCookieForShieldUser(Arg<AuthenticatedShieldUser>.Is.Anything));
            Login.AssertWasNotCalled(_ => _.SetAccountInfo(Arg<AuthenticateAndAuthorizeUserResponse>.Is.Anything,
                                                        ref Arg<string>.Ref(Is.Equal("input"), "output").Dummy,
                                                        ref Arg<string>.Ref(Is.Equal("input"), "output").Dummy), (x) => x.IgnoreArguments());
        }

        public void Should_Not_Set_Authentication_Cookie_And_Not_Set_Account_Info_For_Shield_User_when_SetContextOnValidShieldAuth_NotExplicitlySet()
        {
            //arrange
            string redirectUrl = "";
            string msg = "";
            string accountId = "";
            var authenticateAndAuthorizeUserResponseWithNoneExpired = new AuthenticateAndAuthorizeUserResponse
            {
                AuthenticationStatus = AuthenticationStatuses.SHIELD_AUTH,
                AuthenticatedShieldUsers = new AuthenticatedShieldUser[] { new AuthenticatedShieldUser { IsPasswordExpired = false, AccountID = "111" } }
            };
            Login.Stub(x => x.SetAuthenticationCookieForShieldUser(new AuthenticatedShieldUser()))
                 .IgnoreArguments();
            Login.Stub(x => x.SetAccountInfo(null, ref redirectUrl, ref msg))
                 .IgnoreArguments();

            LogonProcessor = new Authenticator(Logger, HttpContext.Current.Session, Login, ePSBroker);

            //act
            var authenticationProcessResponseSetContextOnValidShieldAuth = LogonProcessor.ProcessAuthentication(new AuthRequestDTO(), authenticateAndAuthorizeUserResponseWithNoneExpired,
                                                                                     ref redirectUrl, ref msg, ref accountId);

            //assert            
            Login.AssertWasNotCalled(_ => _.SetAuthenticationCookieForShieldUser(Arg<AuthenticatedShieldUser>.Is.Anything));
            Login.AssertWasNotCalled(_ => _.SetAccountInfo(Arg<AuthenticateAndAuthorizeUserResponse>.Is.Anything,
                                                        ref Arg<string>.Ref(Is.Equal("input"), "output").Dummy,
                                                        ref Arg<string>.Ref(Is.Equal("input"), "output").Dummy), (x) => x.IgnoreArguments());
            Assert.AreEqual(accountId, "");
        }

        public void Should_Not_Set_Authentication_Cookie_And_Not_Set_Account_Info_For_Shield_User_when_ValidRegistrantLogin_IsTrue_And_ValidShieldLogin_IsTrue()
        {
            //arrange
            string redirectUrl = "";
            string msg = "";
            string accountId = "";
            var authenticateAndAuthorizeUserResponseWithNoneExpired = new AuthenticateAndAuthorizeUserResponse
            {
                AuthenticationStatus = AuthenticationStatuses.SHIELD_AUTH,
                AuthenticatedShieldUsers = new AuthenticatedShieldUser[] { new AuthenticatedShieldUser { IsPasswordExpired = false } },
                ValidRegistrantLogin = true,
                ValidShieldLogin = true
            };
            Login.Stub(x => x.SetAuthenticationCookieForShieldUser(new AuthenticatedShieldUser()))
                 .IgnoreArguments();
            Login.Stub(x => x.SetAccountInfo(null, ref redirectUrl, ref msg))
                 .IgnoreArguments();

            LogonProcessor = new Authenticator(Logger, HttpContext.Current.Session, Login, ePSBroker);

            //act
            var authenticationProcessResponseSetContextOnValidShieldAuth = LogonProcessor.ProcessAuthentication(new AuthRequestDTO(), authenticateAndAuthorizeUserResponseWithNoneExpired,
                                                                                     ref redirectUrl, ref msg, ref accountId);

            //assert            
            Login.AssertWasNotCalled(_ => _.SetAuthenticationCookieForShieldUser(Arg<AuthenticatedShieldUser>.Is.Anything));
            Login.AssertWasNotCalled(_ => _.SetAccountInfo(Arg<AuthenticateAndAuthorizeUserResponse>.Is.Anything,
                                                        ref Arg<string>.Ref(Is.Equal("input"), "output").Dummy,
                                                        ref Arg<string>.Ref(Is.Equal("input"), "output").Dummy), (x) => x.IgnoreArguments());
        }

        //A valid login case: Happy Scenario.
        public void Should_Set_Authentication_Cookie_And_Set_Account_Info_For_Shield_User_when_ValidRegistrantLogin_IsFalse_And_ValidShieldLogin_IsTrue()
        {
            //arrange
            string redirectUrl = "";
            string msg = "";
            string accountId = "";
            var authenticateAndAuthorizeUserResponseWithNoneExpired = new AuthenticateAndAuthorizeUserResponse
            {
                AuthenticationStatus = AuthenticationStatuses.SHIELD_AUTH,
                AuthenticatedShieldUsers = new AuthenticatedShieldUser[] { new AuthenticatedShieldUser { IsPasswordExpired = false, AccountID = "111" } },
                ValidRegistrantLogin = false,
                ValidShieldLogin = true
            };
            Login.Stub(x => x.SetAuthenticationCookieForShieldUser(new AuthenticatedShieldUser()))
                 .IgnoreArguments();
            Login.Stub(x => x.SetAccountInfo(null, ref redirectUrl, ref msg))
                 .IgnoreArguments();

            LogonProcessor = new Authenticator(Logger, HttpContext.Current.Session, Login, ePSBroker);

            //act
            var authenticationProcessResponseSetContextOnValidShieldAuth = LogonProcessor.ProcessAuthentication(new AuthRequestDTO(), authenticateAndAuthorizeUserResponseWithNoneExpired,
                                                                                     ref redirectUrl, ref msg, ref accountId);

            //assert            
            Login.AssertWasCalled(_ => _.SetAuthenticationCookieForShieldUser(Arg<AuthenticatedShieldUser>.Is.Anything));
            Login.AssertWasCalled(_ => _.SetAccountInfo(Arg<AuthenticateAndAuthorizeUserResponse>.Is.Anything,
                                                        ref Arg<string>.Ref(Is.Equal("input"), "output").Dummy,
                                                        ref Arg<string>.Ref(Is.Equal("input"), "output").Dummy), (x) => x.IgnoreArguments());

            Assert.AreEqual(accountId, "111");
        }

        //Dont see any scenario where it will be called like this.
        public void Should_Set_Authentication_Cookie_And_Set_Account_Info_For_Shield_User_when_ValidRegistrantLogin_IsTrue_And_ValidShieldLogin_IsFalse()
        {
            //arrange
            string redirectUrl = "";
            string msg = "";
            string accountId = "";
            var authenticateAndAuthorizeUserResponseWithNoneExpired = new AuthenticateAndAuthorizeUserResponse
            {
                AuthenticationStatus = AuthenticationStatuses.SHIELD_AUTH,
                AuthenticatedShieldUsers = new AuthenticatedShieldUser[] { new AuthenticatedShieldUser { IsPasswordExpired = false, AccountID = "111" } },
                ValidRegistrantLogin = true,
                ValidShieldLogin = false
            };
            Login.Stub(x => x.SetAuthenticationCookieForShieldUser(new AuthenticatedShieldUser()))
                 .IgnoreArguments();
            Login.Stub(x => x.SetAccountInfo(null, ref redirectUrl, ref msg))
                 .IgnoreArguments();

            LogonProcessor = new Authenticator(Logger, HttpContext.Current.Session, Login, ePSBroker);

            //act
            var authenticationProcessResponseSetContextOnValidShieldAuth = LogonProcessor.ProcessAuthentication(new AuthRequestDTO(), authenticateAndAuthorizeUserResponseWithNoneExpired,
                                                                                     ref redirectUrl, ref msg, ref accountId);

            //assert            
            Login.AssertWasCalled(_ => _.SetAuthenticationCookieForShieldUser(Arg<AuthenticatedShieldUser>.Is.Anything));
            Login.AssertWasCalled(_ => _.SetAccountInfo(Arg<AuthenticateAndAuthorizeUserResponse>.Is.Anything,
                                                        ref Arg<string>.Ref(Is.Equal("input"), "output").Dummy,
                                                        ref Arg<string>.Ref(Is.Equal("input"), "output").Dummy), (x) => x.IgnoreArguments());

            Assert.AreEqual(accountId, "111");
        }

        //Case on Rgistration Mediator
        public void Should_Set_Authentication_Cookie_And_Set_Account_Info_For_Shield_User_when_SetContextOnValidShieldAuth_IsTrue_But_Also_ValidRegistrantLogin_IsTrue_And_ValidShieldLogin_IsTrue()
        {
            //arrange
            string redirectUrl = "";
            string msg = "";
            string accountId = "";
            var authenticateAndAuthorizeUserResponseWithNoneExpired = new AuthenticateAndAuthorizeUserResponse
            {
                AuthenticationStatus = AuthenticationStatuses.SHIELD_AUTH,
                AuthenticatedShieldUsers = new AuthenticatedShieldUser[] { new AuthenticatedShieldUser { IsPasswordExpired = false } },
                ValidRegistrantLogin = true,
                ValidShieldLogin = true
            };
            Login.Stub(x => x.SetAuthenticationCookieForShieldUser(new AuthenticatedShieldUser()))
                 .IgnoreArguments();
            Login.Stub(x => x.SetAccountInfo(null, ref redirectUrl, ref msg))
                 .IgnoreArguments();

            LogonProcessor = new Authenticator(Logger, HttpContext.Current.Session, Login, ePSBroker);

            //act
            var authenticationProcessResponseSetContextOnValidShieldAuth = LogonProcessor.ProcessAuthentication(new AuthRequestDTO(), authenticateAndAuthorizeUserResponseWithNoneExpired,
                                                                                     ref redirectUrl, ref msg, ref accountId);

            //assert            
            Login.AssertWasCalled(_ => _.SetAuthenticationCookieForShieldUser(Arg<AuthenticatedShieldUser>.Is.Anything));
            Login.AssertWasCalled(_ => _.SetAccountInfo(Arg<AuthenticateAndAuthorizeUserResponse>.Is.Anything,
                                                        ref Arg<string>.Ref(Is.Equal("input"), "output").Dummy,
                                                        ref Arg<string>.Ref(Is.Equal("input"), "output").Dummy), (x) => x.IgnoreArguments());

            Assert.AreEqual(accountId, "111");
        }

        public void Should_Set_Authentication_Cookie_And_Not_Set_Account_Info_For_Shield_User_when_SetContextOnValidShieldAuth_Set_True()
        {
            //arrange
            string redirectUrl = "";
            string msg = "";
            string accountId = "";
            var authenticateAndAuthorizeUserResponseWithNoneExpired = new AuthenticateAndAuthorizeUserResponse
            {
                AuthenticationStatus = AuthenticationStatuses.SHIELD_AUTH,
                AuthenticatedShieldUsers = new AuthenticatedShieldUser[] { new AuthenticatedShieldUser { IsPasswordExpired = false, AccountID = "111" } }
            };
            Login.Stub(x => x.SetAuthenticationCookieForShieldUser(new AuthenticatedShieldUser()))
                 .IgnoreArguments();
            Login.Stub(x => x.SetAccountInfo(null, ref redirectUrl, ref msg))
                 .IgnoreArguments();

            LogonProcessor = new Authenticator(Logger, HttpContext.Current.Session, Login, ePSBroker);

            //act
            var authenticationProcessResponseSetContextOnValidShieldAuth = LogonProcessor.ProcessAuthentication(new AuthRequestDTO(), authenticateAndAuthorizeUserResponseWithNoneExpired,
                                                                                     ref redirectUrl, ref msg, ref accountId, setContextOnValidShieldAuth: true);

            //assert            
            Login.AssertWasCalled(_ => _.SetAuthenticationCookieForShieldUser(Arg<AuthenticatedShieldUser>.Is.Anything));
            Login.AssertWasCalled(_ => _.SetAccountInfo(Arg<AuthenticateAndAuthorizeUserResponse>.Is.Anything,
                                                        ref Arg<string>.Ref(Is.Equal("input"), "output").Dummy,
                                                        ref Arg<string>.Ref(Is.Equal("input"), "output").Dummy), (x) => x.IgnoreArguments());
            Assert.AreEqual(accountId, "111");
        }
        #endregion SHIELD_AUTH_Password_Not_Expired

        #region SHIELD_AUTH_Password_Expired
        [TestMethod]
        public void Should_not_reattempt_authenticate_if_IsShieldPasswordExpired_and_Not_IsPasswordResetSuccess()
        {
            string redirectUrl = "";
            string msg = "";
            string accountId = "";
            //arrange
            var authenticateAndAuthorizeUserResponseWithOneExpired = new AuthenticateAndAuthorizeUserResponse
            {
                AuthenticationStatus = AuthenticationStatuses.SHIELD_AUTH,
                AuthenticatedShieldUsers = new AuthenticatedShieldUser[] { new AuthenticatedShieldUser { IsPasswordExpired = true } }
            };


            var authResponseOnReattempt = new AuthenticateAndAuthorizeUserResponse
            {
                AuthenticationStatus = AuthenticationStatuses.SHIELD_AUTH,
                AuthenticatedShieldUsers = new AuthenticatedShieldUser[] { new AuthenticatedShieldUser { IsPasswordExpired = false } }
            };

            ePSBroker.Stub(x => x.AuthenticateAndAuthorizeUser("user", "password", "ipaddress"))
                     .IgnoreArguments()
                     .Return(authenticateAndAuthorizeUserResponseWithOneExpired).Repeat.Once();//Just to ensure it is not a repetition

            ePSBroker.Stub(x => x.AuthenticateAndAuthorizeUser("user", "password", "ipaddress"))
                   .IgnoreArguments()
                   .Return(authResponseOnReattempt).Repeat.Once();//Just to ensure it is not a repetition

            LogonProcessor = new Authenticator(Logger, HttpContext.Current.Session, Login, ePSBroker);


            //act
            var authenticationProcessResponse = LogonProcessor.AuthenticateUser(new AuthRequestDTO() { UserName = "username" }, ref redirectUrl, ref msg, ref accountId, password: "xxxx", excludeLoggingCheck: true);

            //assert- Because on second attempt the response is true as IsPasswordResetSuccess is set
            Assert.IsTrue(authenticationProcessResponse.IsShieldPasswordExpired);
        }

        [TestMethod]
        public void Should_not_reattempt_authenticate_if_not_IsPasswordResetSuccess()
        {
            string redirectUrl = "";
            string msg = "";
            string accountId = "";
            //arrange
            var firstResponse = new AuthenticateAndAuthorizeUserResponse
            {
                AuthenticationStatus = AuthenticationStatuses.SHIELD_AUTH,
                AuthenticatedShieldUsers = new AuthenticatedShieldUser[] { new AuthenticatedShieldUser { IsPasswordExpired = false } }
            };

            ePSBroker.Stub(x => x.AuthenticateAndAuthorizeUser("user", "password", "ipaddress"))
                     .IgnoreArguments()
                     .Return(firstResponse).Repeat.Once();//Just to ensure it is not a repetition

            LogonProcessor = new Authenticator(Logger, HttpContext.Current.Session, Login, ePSBroker);

            //act
            var authenticationProcessResponse = LogonProcessor.AuthenticateUser(new AuthRequestDTO() { UserName = "username", IsPasswordResetSuccess = false }, ref redirectUrl, ref msg, ref accountId, password: "xxxx", excludeLoggingCheck: true);

            //assert- what was the initial response
            Assert.IsFalse(authenticationProcessResponse.IsShieldPasswordExpired);//Inspite of first response it will never be null
        }

        [TestMethod]
        public void Should_reattempt_authenticate_if_IsPasswordResetSuccess()
        {
            string redirectUrl = "";
            string msg = "";
            string accountId = "";
            //arrange
            var authenticateAndAuthorizeUserResponseWithOneExpired = new AuthenticateAndAuthorizeUserResponse
            {
                AuthenticationStatus = AuthenticationStatuses.SHIELD_AUTH,
                AuthenticatedShieldUsers = new AuthenticatedShieldUser[] { new AuthenticatedShieldUser { IsPasswordExpired = true } }
            };


            var authResponseOnReattempt = new AuthenticateAndAuthorizeUserResponse
            {
                AuthenticationStatus = AuthenticationStatuses.SHIELD_AUTH,
                AuthenticatedShieldUsers = new AuthenticatedShieldUser[] { new AuthenticatedShieldUser { IsPasswordExpired = false } }
            };

            ePSBroker.Stub(x => x.AuthenticateAndAuthorizeUser("user", "password", "ipaddress"))
                     .IgnoreArguments()
                     .Return(authenticateAndAuthorizeUserResponseWithOneExpired).Repeat.Once();//Just to ensure it is not a repetition

            ePSBroker.Stub(x => x.AuthenticateAndAuthorizeUser("user", "password", "ipaddress"))
                   .IgnoreArguments()
                   .Return(authResponseOnReattempt).Repeat.Once();//Just to ensure it is not a repetition

            LogonProcessor = new Authenticator(Logger, HttpContext.Current.Session, Login, ePSBroker);


            //act
            var authenticationProcessResponse = LogonProcessor.AuthenticateUser(new AuthRequestDTO() { UserName = "username", IsPasswordResetSuccess = true }, ref redirectUrl, ref msg, ref accountId, password: "xxxx", excludeLoggingCheck: true);

            //assert- Because on second attempt the response is true as IsPasswordResetSuccess is set
            Assert.IsFalse(authenticationProcessResponse.IsShieldPasswordExpired);
        }
        #endregion
        #endregion SHIELD_AUTH

        #region SHIELD_AUTH_REGISTRANT

        [TestMethod]
        public void Should_Set_RegistrantContext_For_Registrant_User_when_SHIELD_AUTH_REGISTRANT()
        {
            //arrange
            string redirectUrl = "";
            string msg = "";
            string accountId = "";

            var authenticateAndAuthorizeUserResponseWithRegistrant = new AuthenticateAndAuthorizeUserResponse
            {
                AuthenticationStatus = AuthenticationStatuses.SHIELD_AUTH_REGISTRANT,
                AuthenticatedRegistrant = new RegistrantInfo { ShieldUserName = "aaa" },
                AuthenticatedShieldUsers = new AuthenticatedShieldUser[] { new AuthenticatedShieldUser { IsPasswordExpired = false } }
            };
            LogonProcessor = new Authenticator(Logger, HttpContext.Current.Session, Login, ePSBroker);

            //act
            var authenticationProcessResponseRegistrant = LogonProcessor.ProcessAuthentication(new AuthRequestDTO(), authenticateAndAuthorizeUserResponseWithRegistrant,
                                                                                     ref redirectUrl, ref msg, ref accountId);
            var registrantInfo = HttpContext.Current.Session[Constants.SessionVariables.RegistrantInfo] as Objects.Registrant.RegistrantInfo;

            //assert            
            Assert.IsNotNull(registrantInfo);
            Assert.IsTrue(authenticationProcessResponseRegistrant.IsRegistrantUser);
            Assert.IsTrue(authenticationProcessResponseRegistrant.HasLoggedIn);
            Assert.AreEqual(registrantInfo.ShieldUserName, "aaa");
        }


        [TestMethod]
        public void Should_Set_RedirectUrl_To_Registrant_Welcome_For_Registrant_User_when_SHIELD_AUTH_REGISTRANT()
        {
            //arrange
            string redirectUrl = "";
            string msg = "";
            string accountId = "";

            var authenticateAndAuthorizeUserResponseWithRegistrant = new AuthenticateAndAuthorizeUserResponse
            {
                AuthenticationStatus = AuthenticationStatuses.SHIELD_AUTH_REGISTRANT,
                AuthenticatedRegistrant = new RegistrantInfo { ShieldUserName = "aaa" },
                AuthenticatedShieldUsers = new AuthenticatedShieldUser[] { new AuthenticatedShieldUser { IsPasswordExpired = false } }
            };
            LogonProcessor = new Authenticator(Logger, HttpContext.Current.Session, Login, ePSBroker);

            //act
            var authenticationProcessResponseRegistrant = LogonProcessor.ProcessAuthentication(new AuthRequestDTO(), authenticateAndAuthorizeUserResponseWithRegistrant,
                                                                                     ref redirectUrl, ref msg, ref accountId);

            //assert                              
            Assert.AreEqual("~/" + Constants.PageNames.REGISTER_WELCOME, redirectUrl);
        }


        [TestMethod]
        public void Should_Set_ShieldIdentityToken_And_Registration_Enterprise_ClientID_For_Registrant_User_when_SHIELD_AUTH_REGISTRANT()
        {
            //arrange
            string redirectUrl = "";
            string msg = "";
            string accountId = "";

            var authenticateAndAuthorizeUserResponseWithRegistrant = new AuthenticateAndAuthorizeUserResponse
            {
                AuthenticationStatus = AuthenticationStatuses.SHIELD_AUTH_REGISTRANT,
                AuthenticatedRegistrant = new RegistrantInfo { ShieldUserName = "aaa", EnterpriseClientID = "registrationEnterpriseClientID" },
                AuthenticatedShieldUsers = new AuthenticatedShieldUser[] { new AuthenticatedShieldUser { IsPasswordExpired = false } },
                IdentToken = "IdentToken"

            };
            LogonProcessor = new Authenticator(Logger, HttpContext.Current.Session, Login, ePSBroker);

            //act
            var authenticationProcessResponseRegistrant = LogonProcessor.ProcessAuthentication(new AuthRequestDTO(), authenticateAndAuthorizeUserResponseWithRegistrant,
                                                                                     ref redirectUrl, ref msg, ref accountId);
            var shieldIdentityToken = HttpContext.Current.Session[Constants.SessionVariables.ShieldIdentityToken] as string;
            var registrationEnterpriseClientID = HttpContext.Current.Session[Constants.SessionVariables.RegistrationEnterpriseClientID] as string;


            //assert            
            Assert.AreEqual(shieldIdentityToken, "IdentToken");
            Assert.AreEqual(registrationEnterpriseClientID, "registrationEnterpriseClientID");
        }

        #endregion

        #region NOT_AUTHORIZED/AUTH_SUCCESSFUL_BUT_NOT_ALLOWED
        [TestMethod]
        public void Should_Log_And_Get_DisplayErrorMessage_when_NOT_AUTHORIZED_OR_AUTH_SUCCESSFUL_BUT_NOT_ALLOWED()
        {
            //arrange
            string redirectUrl = "";
            string msg = "";
            string accountId = "";
            var NOT_AUTHORIZED = new AuthenticateAndAuthorizeUserResponse
            {
                AuthenticationStatus = AuthenticationStatuses.NOT_AUTHORIZED,
                DisplayMessage = "NOT_AUTHORIZED"
            };

            var AUTH_SUCCESSFUL_BUT_NOT_ALLOWED = new AuthenticateAndAuthorizeUserResponse
            {
                AuthenticationStatus = AuthenticationStatuses.AUTH_SUCCESSFUL_BUT_NOT_ALLOWED,
                DisplayMessage = "AUTH_SUCCESSFUL_BUT_NOT_ALLOWED"
            };

            Logger.Stub(x => x.Debug(""))
                 .IgnoreArguments();

            LogonProcessor = new Authenticator(Logger, HttpContext.Current.Session, Login, ePSBroker);

            //act
            var NOT_AUTHORIZED_RESPONSE = LogonProcessor.ProcessAuthentication(new AuthRequestDTO(), NOT_AUTHORIZED,
                                                                                     ref redirectUrl, ref msg, ref accountId);

            //assert
            Assert.IsFalse(NOT_AUTHORIZED_RESPONSE.HasLoggedIn);
            Logger.AssertWasCalled(_ => _.Debug(Arg<string>.Is.Anything));
            Assert.AreEqual(NOT_AUTHORIZED_RESPONSE.LoginErrorMessage, "NOT_AUTHORIZED");

            //act
            var AUTH_SUCCESSFUL_BUT_NOT_ALLOWED_RESPONSE = LogonProcessor.ProcessAuthentication(new AuthRequestDTO(), AUTH_SUCCESSFUL_BUT_NOT_ALLOWED,
                                                                                     ref redirectUrl, ref msg, ref accountId);

            //assert
            Assert.IsFalse(AUTH_SUCCESSFUL_BUT_NOT_ALLOWED_RESPONSE.HasLoggedIn);
            Logger.AssertWasCalled(_ => _.Debug(Arg<string>.Is.Anything));
            Assert.AreEqual(AUTH_SUCCESSFUL_BUT_NOT_ALLOWED_RESPONSE.LoginErrorMessage, "AUTH_SUCCESSFUL_BUT_NOT_ALLOWED");
        }

        #endregion


        [TestMethod]
        public void Should_Get_error_message_for_display_when_NO_status_from_shield()
        {
            //arrange
            string redirectUrl = "";
            string msg = "";
            string accountId = "";
            var NON_EXISTANT = new AuthenticateAndAuthorizeUserResponse
            {
                AuthenticationStatus = (AuthenticationStatuses)10
            };

            Logger.Stub(x => x.Debug(""))
                 .IgnoreArguments();

            LogonProcessor = new Authenticator(Logger, HttpContext.Current.Session, Login, ePSBroker);

            //act
            var NON_EXISTANT_RESPONSE = LogonProcessor.ProcessAuthentication(new AuthRequestDTO(), NON_EXISTANT,
                                                                                     ref redirectUrl, ref msg, ref accountId);

            //assert
            Assert.IsFalse(NON_EXISTANT_RESPONSE.HasLoggedIn);
            StringAssert.Contains(NON_EXISTANT_RESPONSE.LoginErrorMessage.ToLower(), "Unrecognized credentials".ToLower());
        }

        #region Set_Registrant_User_Context
        [TestMethod]
        public void Should_Set_authentication_cookie()
        {
            //arrange
            Login.Stub(x => x.SetAuthenticationCookie(""))
                 .IgnoreArguments();

            LogonProcessor = new Authenticator(Logger, HttpContext.Current.Session, Login, ePSBroker);

            //act
            LogonProcessor.SetRegistrantUserContext(new Objects.Registrant.RegistrantInfo { });

            //assert            
            Login.AssertWasCalled(_ => _.SetAuthenticationCookie(Arg<string>.Is.Anything));
        }

        [TestMethod]
        public void Should_Set_registrant_info_in_session()
        {
            //arrange
            LogonProcessor = new Authenticator(Logger, HttpContext.Current.Session, Login, ePSBroker);

            //act
            LogonProcessor.SetRegistrantUserContext(new Objects.Registrant.RegistrantInfo { ShieldUserName = "aaa" });

            //assert            
            var registrantInfo = HttpContext.Current.Session[Constants.SessionVariables.RegistrantInfo] as Objects.Registrant.RegistrantInfo;
            Assert.IsNotNull(registrantInfo);
            Assert.AreEqual(registrantInfo.ShieldUserName, "aaa");
        }
        #endregion

        #region ReattemptAuthentication
        [TestMethod]
        public void Should_ReattemptAuthentication_Make_3_attempts()
        {
            string redirectUrl = "";
            string msg = "";
            string accountId = "";
            //arrange
            var authFirstResponse = new AuthenticateAndAuthorizeUserResponse
            {
                AuthenticationStatus = AuthenticationStatuses.SHIELD_AUTH,
                DisplayMessage = "first",//Not relevant in functionality
                AuthenticatedShieldUsers = new AuthenticatedShieldUser[] { new AuthenticatedShieldUser { IsPasswordExpired = true } }
            };


            var authSecondResponse = new AuthenticateAndAuthorizeUserResponse
            {
                AuthenticationStatus = AuthenticationStatuses.SHIELD_AUTH,
                DisplayMessage = "second",//Not relevant in functionality
                AuthenticatedShieldUsers = new AuthenticatedShieldUser[] { new AuthenticatedShieldUser { IsPasswordExpired = true } }
            };

            var authThirdResponse = new AuthenticateAndAuthorizeUserResponse
            {
                AuthenticationStatus = AuthenticationStatuses.SHIELD_AUTH,
                DisplayMessage = "third",//Not relevant in functionality
                AuthenticatedShieldUsers = new AuthenticatedShieldUser[] { new AuthenticatedShieldUser { IsPasswordExpired = true } }
            };


            ePSBroker.Stub(x => x.AuthenticateAndAuthorizeUser("user", "password", "ipaddress"))
                     .IgnoreArguments()
                     .Return(authFirstResponse).Repeat.Once();//Just to ensure it is not a repetition

            ePSBroker.Stub(x => x.AuthenticateAndAuthorizeUser("user", "password", "ipaddress"))
                   .IgnoreArguments()
                   .Return(authSecondResponse).Repeat.Once();//Just to ensure it is not a repetition

            ePSBroker.Stub(x => x.AuthenticateAndAuthorizeUser("user", "password", "ipaddress"))
                   .IgnoreArguments()
                   .Return(authThirdResponse).Repeat.Once();//Just to ensure it is not a repetition

            LogonProcessor = new Authenticator(Logger, HttpContext.Current.Session, Login, ePSBroker);


            //act
            var authenticationProcessResponse = LogonProcessor.ReattemptAuthentication(new AuthRequestDTO() { UserName = "username" }, password: "xxxx");

            //assert- Only two attempts weill be made
            Assert.IsTrue(Equals(authenticationProcessResponse.DisplayMessage, "third"));
        }

        [TestMethod]
        public void Should_Not_ReattemptAuthentication_IF_first_attempt_returned_passwordexpired_as_false()
        {
            string redirectUrl = "";
            string msg = "";
            string accountId = "";
            //arrange
            var authFirstResponse = new AuthenticateAndAuthorizeUserResponse
            {
                AuthenticationStatus = AuthenticationStatuses.SHIELD_AUTH,
                DisplayMessage = "first",//Not relevant in functionality
                AuthenticatedShieldUsers = new AuthenticatedShieldUser[] { new AuthenticatedShieldUser { IsPasswordExpired = false } }
            };


            var authSecondResponse = new AuthenticateAndAuthorizeUserResponse
            {
                AuthenticationStatus = AuthenticationStatuses.SHIELD_AUTH,
                DisplayMessage = "second",//Not relevant in functionality
                AuthenticatedShieldUsers = new AuthenticatedShieldUser[] { new AuthenticatedShieldUser { IsPasswordExpired = true } }
            };

            ePSBroker.Stub(x => x.AuthenticateAndAuthorizeUser("user", "password", "ipaddress"))
                     .IgnoreArguments()
                     .Return(authFirstResponse).Repeat.Once();//Just to ensure it is not a repetition

            ePSBroker.Stub(x => x.AuthenticateAndAuthorizeUser("user", "password", "ipaddress"))
                   .IgnoreArguments()
                   .Return(authSecondResponse).Repeat.Once();//Just to ensure it is not a repetition


            LogonProcessor = new Authenticator(Logger, HttpContext.Current.Session, Login, ePSBroker);


            //act
            var authenticationProcessResponse = LogonProcessor.ReattemptAuthentication(new AuthRequestDTO() { UserName = "username" }, password: "xxxx");

            //assert- Only two attempts weill be made
            Assert.IsTrue(Equals(authenticationProcessResponse.DisplayMessage, "first"));
        }

        [TestMethod]
        public void Should_ReattemptAuthentication_If_first_attempt_returned_not_SHIELD_AUTH()
        {
            string redirectUrl = "";
            string msg = "";
            string accountId = "";
            //arrange
            var authFirstResponse = new AuthenticateAndAuthorizeUserResponse
            {
                AuthenticationStatus = AuthenticationStatuses.AUTH_SUCCESSFUL_BUT_NOT_ALLOWED,
                DisplayMessage = "first",//Not relevant in functionality
                AuthenticatedShieldUsers = new AuthenticatedShieldUser[] { new AuthenticatedShieldUser { IsPasswordExpired = false } }
            };


            var authSecondResponse = new AuthenticateAndAuthorizeUserResponse
            {
                AuthenticationStatus = AuthenticationStatuses.SHIELD_AUTH,
                DisplayMessage = "second",//Not relevant in functionality
                AuthenticatedShieldUsers = new AuthenticatedShieldUser[] { new AuthenticatedShieldUser { IsPasswordExpired = true } }
            };

            ePSBroker.Stub(x => x.AuthenticateAndAuthorizeUser("user", "password", "ipaddress"))
                     .IgnoreArguments()
                     .Return(authFirstResponse).Repeat.Once();//Just to ensure it is not a repetition

            ePSBroker.Stub(x => x.AuthenticateAndAuthorizeUser("user", "password", "ipaddress"))
                   .IgnoreArguments()
                   .Return(authSecondResponse).Repeat.Twice();


            LogonProcessor = new Authenticator(Logger, HttpContext.Current.Session, Login, ePSBroker);


            //act
            var authenticationProcessResponse = LogonProcessor.ReattemptAuthentication(new AuthRequestDTO() { UserName = "username" }, password: "xxxx");

            //assert- Only two attempts weill be made
            Assert.IsTrue(Equals(authenticationProcessResponse.DisplayMessage, "second"));
        }
        #endregion
    }

}