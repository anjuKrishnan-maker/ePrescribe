#define TRACE
#define DEBUG
using System;
using System.Collections;
using System.Data;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;
using System.Xml;
using System.Collections.Generic;
using System.Linq;
using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.Shared.Logging;
using Allscripts.Impact;
using ComponentSpace.SAML;
using ComponentSpace.SAML.Assertions;
using ComponentSpace.SAML.Protocol;
using System.Text.RegularExpressions;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using Allscripts.ePrescribe.Objects;
using eRxWeb.AppCode;
using eRxWeb.AppCode.Interfaces;
using eRxWeb.ePrescribeSvc;
using eRxWeb.State;
using ConfigurationManager = System.Configuration.ConfigurationManager;
using ConnectionStringPointer = Allscripts.ePrescribe.DatabaseSelector.ConnectionStringPointer;
using Patient = Allscripts.Impact.Patient;
using RxUser = Allscripts.Impact.RxUser;
using SystemConfig = Allscripts.Impact.SystemConfig;
using System.Threading.Tasks;
using System.Threading;
using eRxWeb.AppCode.StateUtils;
using SsoAttributes = Allscripts.ePrescribe.Objects.SsoAttributes;
using Name = Allscripts.ePrescribe.Objects.Name;
using Address = Allscripts.ePrescribe.Objects.Address;


namespace eRxWeb
{
    public partial class SSO : BasePage
    {
        #region Member Fields

        Login _login = null;
        long _operation;
        string _target = string.Empty;
        string _target_error = string.Empty;
        private string _shieldLoginId = string.Empty;

        // For Formulary setting in fourth attribute 

        XmlDocument doc = new XmlDocument();
        bool passPatientData = false;
        string _planid = string.Empty;
        string _planname = string.Empty;
        string patientdoc = string.Empty;

        private static readonly ILoggerEx logger = LoggerEx.GetLogger();

        #endregion

        #region " Web Form Designer Generated Code "

        //This call is required by the Web Form Designer.
        [System.Diagnostics.DebuggerStepThrough()]
        private void InitializeComponent()
        {

        }


        private void Page_Init(System.Object sender, System.EventArgs e)
        {
            //CODEGEN: This method call is required by the Web Form Designer
            //Do not modify it using the code editor.

            InitializeComponent();
        }

        #endregion

        #region Page Methods

        private void Page_Load(System.Object sender, System.EventArgs e)
        {
            try
            {                
                var sso = new Sso();
                Session.Clear();
                Session["PATIENTID"] = null;
                Session["DisableIntroductoryPopup"] = null;
                Response.ExpiresAbsolute = DateTime.Now.AddDays(-1d);
                Response.CacheControl = "no-cache";
                Response.AddHeader("cache-control", "no-store, must-revalidate, private");
                Response.AddHeader("Pragma", "no-cache");
                Response.Expires = -1;
                Response.Buffer = true;
                //Session["SITEID"] = "1";// #724526,to allow an Sso user to use print center.
                _target = Request.Form["TARGET"];
                if (_target == null)
                {
                    _target = string.Empty;
                }

                _target_error = Request.Form["TARGET_ERROR"];
                if (_target_error == null)
                {
                    _target_error = string.Empty;
                }

                string base64SamlResponse = Request.Form["SAMLResponse"];

                if (base64SamlResponse == null)
                {
                    throw (new ArgumentException("The SAML response message is empty or not present."));
                }

                if (Application["Partners"] == null)
                {
                    sso.LoadPartners(Application, DBID);
                }
                
                ProcessBrowserPost(base64SamlResponse, sso);
            }
            catch (System.Threading.ThreadAbortException)
            {
                //string exceptionID = Audit.AddException(
                //    Guid.Empty.ToString(),
                //    Guid.Empty.ToString(),
                //    string.Concat("Sso Thread Abort Exception: ", threadAbortException.Message, " ", threadAbortException.StackTrace),
                //    Request.UserIpAddress(),
                //    null,
                //    null,
                //    ConnectionStringPointer.ERXDB_DEFAULT);

                //Session["SSOErrorMessage"] = string.Concat("Exception Reference ID: ", exceptionID);
                //Response.Redirect("SSOError.aspx");
            }
            catch (Exception exception)
            {
                Allscripts.Impact.Audit.PartnerInsert(
                    Session["PartnerID"] != null ? Session["PartnerID"].ToString() : Guid.Empty.ToString(),
                    null,
                    Request.UserIpAddress(),
                    "Sso post",
                    exception.Message + " " + exception.StackTrace,
                    ConnectionStringPointer.ERXDB_DEFAULT);

                /* turn the HasAcceptedEula off because it's */
                /* preventing Exception.aspx from coming up */
                if (_target_error != string.Empty)
                {
                    createBrowserErrorRedirect(_target_error, exception.Message);
                }
                else
                {
                    string exceptionID = Audit.AddException(
                        Guid.Empty.ToString(),
                        Guid.Empty.ToString(),
                        string.Concat("Sso post exception: ", exception.ToString()),
                        Request.UserIpAddress(),
                        null,
                        null,
                        ConnectionStringPointer.ERXDB_DEFAULT);

                    Session["SSOErrorMessage"] = string.Concat("Exception Reference ID: ", exceptionID);
                    Response.Redirect(Constants.PageNames.SAML_SSO_ERROR);
                }
            }
        }

        public void Page_Error(object sender, EventArgs e)
        {
            bool err = false;
            Exception objErr = Server.GetLastError().GetBaseException();
            string dispmsg = String.Empty;
            try
            {
                dispmsg += "<html><body>";
                dispmsg += @"<b> Error Message :</b>" + objErr.Message.ToString().ToHTMLEncode();  //Get the error message
                dispmsg += @"</br><b> Error in :</b>" + Request.Url.ToString().ToHTMLEncode();  //Get the URL
                dispmsg += @"</br><b> Error Source :</b>" + objErr.Source.ToString().ToHTMLEncode();  //Source of the message
                dispmsg += @"</br><b> Error Stack Trace :</b>" + objErr.StackTrace.ToString().ToHTMLEncode(); //Stack Trace of the error
                dispmsg += @"</br><b> Error TargetSite :</b>" + objErr.TargetSite.ToString().ToHTMLEncode(); //Method where the error occurred
                dispmsg += "</body></html>";

                System.Exception myError = Server.GetLastError();
                string mess = String.Empty;
                mess += @"<b> Error Message :</b>" + myError.InnerException;  //Get the error message
                mess += @"<b> Error Source :</b>" + myError.Source;  //Source of the message
                mess += @"<b> Error Stack Trace :</b>" + myError.StackTrace; //Stack Trace of the error
                mess += @"<b> Error TargetSite :</b>" + myError.TargetSite; //Method where the error occurred
                Session[Constants.SessionVariables.CURRENT_ERROR] = mess;

                Server.ClearError();
                Server.Transfer("~/" + Constants.PageNames.EXCEPTION);
            }
            catch
            {
                // error trap inside and error routine
                err = true;
            }

            if (err)
            {
                Response.Write(dispmsg);
            }
        }

        #endregion

        #region Custom Methods


        private void createBrowserPostForm(string target)
        {
            string _planid = Request.Form["Planid"];
            string _planname = Request.Form["PlanName"];

            //only get plan info from CCR if the form vars are empty
            if (passPatientData && !string.IsNullOrEmpty(_planid))
            {
                foreach (XmlNode node in doc.GetElementsByTagName("plan-info"))
                {
                    XmlNodeList objXmlNodeList = node.ChildNodes;
                    foreach (XmlNode ChildNode in objXmlNodeList)
                    {
                        if (ChildNode.Name == "planid")
                        {
                            _planid = ChildNode.InnerText;
                        }
                        if (ChildNode.Name == "planname")
                        {
                            _planname = ChildNode.InnerText;
                        }
                    }
                }
            }

            Context.Items["LastName"] = Request.Form["LastName"];
            Context.Items["FirstName"] = Request.Form["FirstName"];
            Context.Items["MiddleName"] = Request.Form["MiddleName"];
            Context.Items["DOB"] = Request.Form["DOB"];
            Context.Items["MRN"] = Request.Form["MRN"];
            Context.Items["PatientGUID"] = Request.Form["PatientGUID"];
            Context.Items["Address1"] = Request.Form["Address1"];
            Context.Items["Address2"] = Request.Form["Address2"];
            Context.Items["City"] = Request.Form["City"];
            Context.Items["State"] = Request.Form["State"];
            Context.Items["Zip"] = Request.Form["Zip"];
            Context.Items["Phone"] = Request.Form["Phone"];
            Context.Items["Gender"] = Request.Form["Gender"];
            Context.Items["Planid"] = _planid;
            Context.Items["PlanName"] = _planname;
            Context.Items["Facility"] = Request.Form["Facility"];
            Context.Items["Group"] = Request.Form["Group"];
            Context.Items["NextPageURL"] = target;
            Context.Items["NCPDPNo"] = Request.Form["NCPDPNo"];
            Context.Items["PharmacySt"] = Request.Form["PharmacySt"];

            if (passPatientData)
            {
                Context.Items["PatientDoc"] = patientdoc;
            }
        }

        private string createBrowserRedirect(string target)
        {
            string returnValue;
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append("<html>");
            stringBuilder.Append("<body onload=\"document.forms.redirect_post.submit()\">");
            stringBuilder.AppendFormat("<form id=\"redirect_post\" method=\"post\" action=\"{0}\">", target.ToUrlEncode());
            stringBuilder.Append("</form>");
            stringBuilder.Append("</body>");
            stringBuilder.Append("</html>");

            returnValue = stringBuilder.ToString();
            return returnValue;
        }

        private string createBrowserErrorRedirect(string target, string errmsg)
        {
            string returnValue;
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append("<html>");
            stringBuilder.Append("<body onload=\"document.forms.err_redirect_post.submit()\">");
            stringBuilder.AppendFormat("<form id=\"err_redirect_post\" method=\"post\" action=\"{0}\">", target);
            stringBuilder.AppendFormat("<input type=\"hidden\" name=\"ErrorMessage\" value=\"{0}\">", HttpUtility.HtmlEncode(errmsg));
            stringBuilder.Append("</form>");
            stringBuilder.Append("</body>");
            stringBuilder.Append("</html>");


            returnValue = stringBuilder.ToString();
            return returnValue;
        }


        private void loadTaskModeSessionVariables()
        {
            if ((string)Request.Form["Facility"] != null)
            {
                Session["ExtFacilityCode"] = (string)Request.Form["Facility"];
            }
        }

        private void ProcessBrowserPost(string base64SamlResponse, ISso sso)
        {
            Session["IsSSOUser"] = true;
            var xmlElement = Sso.CheckSsoIsSigned(base64SamlResponse);

            // check for the partners certificate 
            Response samlResponse = new Response(xmlElement);

            var partners = (Partners)Application[Global.App_Partners];

            var assertion = Sso.GetAssertion(samlResponse);

            var partner = sso.VerifyPartnerAndSetValues(partners, assertion, xmlElement, PageState, Request.UserIpAddress(), DBID);

            var authenticationStatement = Sso.GetAuthenticationStatement(assertion);

            var attributeStatements = SsoAttributes.GetAttributes(assertion.GetAttributeStatements());

            _operation = Sso.GetOperationValue(attributeStatements.Operation);

            _login = new Login(this);

            /* This is being used by Sso utility for finding a partnerID . PartnerID is being sent in SAML Assertion attribute =2  */
            if (assertion.Issuer == "allscripts.com")
            {
                if (attributeStatements.PartnerGuid != default(Guid))
                {
                    partner = sso.FindAndSetPartnerValuesFromGuid(partners, attributeStatements.PartnerGuid, PageState);
                }
            }


            if ((_operation & Constants.SsoOperations.OPERATION_USERGUID) > 0)
            {
                SsoUserGuidOperation(base64SamlResponse, sso, authenticationStatement, attributeStatements, partner);
            }
            else if ((_operation & Constants.SsoOperations.OPERATION_USERNAME) > 1)
            {
                var attrStatements = assertion.GetAttributeStatements();
                string _username = authenticationStatement.Subject.NameIdentifier.Value;
                string _password = string.Empty;

                LogEnabledChecker.CheckLoggingForUser(_username, LogEnabledChecker.UserAttributeType.UserName);
                logger.Debug("processBrowserPost. UserName: {0}, SamlResponse: {1}", _username, base64SamlResponse);
                logger.Debug("Cookies: " + HttpContext.Current.Request.Cookies.ToLogString());

                if (attrStatements.Count > 3)
                {
                    AttributeStatement attributeStatement = (AttributeStatement)attrStatements[3];
                    AttributeValue attributeValue = attributeStatement.Attributes[0].Values[0];
                    _password = attributeValue.Value.ToString();
                }

                string msg = string.Empty;
                string licenseID = null;
                string userID = null;
                string userName = null;
                string deaStatus = null;
                bool isAuthenticatedAndAuthorized = false;
                ConnectionStringPointer dbID = ConnectionStringPointer.ERXDB_DEFAULT;

                logger.Debug("Calling AuthenticateAndAuthorizeUser for userName {0}", _username);
                ePrescribeSvc.AuthenticateAndAuthorizeUserResponse authenticateAndAuthorizeResponse = EPSBroker.AuthenticateAndAuthorizeUser(_username, _password, Request.UserIpAddress());

                if (authenticateAndAuthorizeResponse.AuthenticationStatus == AuthenticationStatuses.SHIELD_AUTH)
                {
                    isAuthenticatedAndAuthorized = true;

                    //this is used to tie the token refresh audit events to the origina login
                    Session["AuditLogUserLoginID"] = authenticateAndAuthorizeResponse.AuditLogID;

                    if (authenticateAndAuthorizeResponse.AuthenticatedShieldUsers.Length == 1)
                    {
                        userID = authenticateAndAuthorizeResponse.AuthenticatedShieldUsers[0].UserGUID;
                        userName = authenticateAndAuthorizeResponse.AuthenticatedShieldUsers[0].ShieldLoginID;
                        dbID = (ConnectionStringPointer)authenticateAndAuthorizeResponse.AuthenticatedShieldUsers[0].DBID;
                    }
                    else if (authenticateAndAuthorizeResponse.AuthenticatedShieldUsers.Length == 0)
                    {
                        isAuthenticatedAndAuthorized = false;
                        msg = "No AuthenticatedShieldUsers found.";
                    }
                    else if (authenticateAndAuthorizeResponse.AuthenticatedShieldUsers.Length > 1)
                    {
                        isAuthenticatedAndAuthorized = false;
                        msg = "Multiple AuthenticatedShieldUsers found.";
                    }

                    if (isAuthenticatedAndAuthorized)
                    {
                        ePrescribeSvc.GetUserResponse getUserResponse = EPSBroker.GetRxUser(
                            ePrescribeSvc.ValueType.UserGUID,
                            userID,
                            string.Empty,
                            null,
                            null,
                            (ConnectionStringPointer)authenticateAndAuthorizeResponse.AuthenticatedShieldUsers[0].DBID);

                        if (string.IsNullOrEmpty(licenseID) && !string.IsNullOrEmpty(getUserResponse.RxUser.LicenseID))
                            licenseID = getUserResponse.RxUser.LicenseID;

                        foreach (ePrescribeSvc.DEALicense deaLicense in getUserResponse.RxUser.DEALicenses)
                        {
                            if (deaLicense.DEAExpirationDate > DateTime.Now)
                            {
                                deaStatus = "ACTIVE";
                            }
                            else
                            {
                                deaStatus = "EXPIRED";
                                break;
                            }
                        }
                    }
                }
                else if (authenticateAndAuthorizeResponse.AuthenticationStatus == AuthenticationStatuses.NOT_AUTHORIZED)
                {
                    isAuthenticatedAndAuthorized = false;
                    msg = "Invalid credentials.";
                }

                Response.ContentType = "text/xml";


                if (isAuthenticatedAndAuthorized)
                {
                    RxUser.PartnerSecUserCheck(Session["PartnerID"].ToString(), licenseID, dbID);
                    
                    Response.Write(createUserGuidResponseXML(userID, userName, deaStatus, msg));
                }
                else
                {
                    Response.Write(createUserGuidResponseXML(string.Empty, string.Empty, string.Empty, msg));
                }

                // end the session - this prevents any further web page calls
                for (int i = 0; i < Request.Cookies.Count; i++)
                {
                    Response.Cookies[Request.Cookies[i].Name].Expires = DateTime.Today.AddYears(-1);
                }

                Session.Clear();
                Session.Abandon();
            }

            _login = null;
        }

        private void SsoUserGuidOperation(string base64SamlResponse, ISso sso, AuthenticationStatement authenticationStatement, SsoAttributes attributeStatements, Partner partner)
        {
            var userGuid = Sso.VerifyUserGuid(authenticationStatement);

            LogEnabledChecker.CheckLoggingForUser(userGuid.ToString(), LogEnabledChecker.UserAttributeType.UserGuid);
            logger.Debug("processBrowserPost. UserGuid: {0}, SamlResponse: {1}", userGuid, base64SamlResponse);
            logger.Debug("Cookies: " + HttpContext.Current.Request.Cookies.ToLogString());

            PageState["UserID"] = userGuid.ToString();
            string redirectUrl = "../" + Constants.PageNames.LOGIN.ToLower();

            Audit.PartnerInsert(PageState.GetStringOrEmpty("PartnerID"), userGuid.ToString(), Request.UserIpAddress(),
                "Sso b64 post", base64SamlResponse, DBID);

            bool isePaPartner = EPSBroker.GetEnterpriseePAStatusByPartnerID(PageState.GetStringOrEmpty("PartnerID"));

            var patientGuid = Guid.Empty;
            if (attributeStatements.PatientId != Guid.Empty //Patient Context/Lockdown is with PatientID
                ||
                !string.IsNullOrWhiteSpace(attributeStatements.PatientMrn) && attributeStatements.LicenseId != Guid.Empty) //Patient Context/Lockdown is with Mrn & LicenseID
            {
                if (!VerifyEpaPartner(isePaPartner, attributeStatements, PageState))
                {
                    Redirect(Constants.PageNames.SAML_SSO_ERROR);
                }

                patientGuid = GetAndSetPatientGuid(attributeStatements);

                if ((patientGuid != Guid.Empty) && ((_operation & Constants.SsoOperations.OPERATION_PATIENT_ADD) == 0))
                {
                    VerifyPatientGuid(patientGuid, userGuid, partner);
                }
            }

            if (attributeStatements.SiteId != 0)
            {
                PageState["SITEID"] = attributeStatements.SiteId;
            }         

            if (attributeStatements.PatientDoc != string.Empty)
            {
                patientdoc = Sso.LoadPatientXmlDoc(attributeStatements.PatientDoc, ref doc, new StateContainer(Session));

                passPatientData = !string.IsNullOrEmpty(patientdoc);
            }

            //the 6th attribute is SSOMode (i.e. PatientLockDownMode or TaskMode)
            if (attributeStatements.SsoMode != string.Empty)
            {
                Sso.SetSsoMode(attributeStatements.SsoMode, PageState);
            }

            Session[Constants.SessionVariables.CURRENT_ERROR] = string.Empty;

            redirectUrl = AuthenticateSsoUser(userGuid, partner, redirectUrl, sso);
            var ssoMode = PageState.GetStringOrEmpty("SSOMode");



            if (_target != string.Empty && !_target.EndsWith(Constants.PageNames.SAML_SSO, StringComparison.OrdinalIgnoreCase))
            {
                if (PageState["PATIENTID"] != null)
                {
                    Allscripts.Impact.Patient.AddPatientToSchedule(base.SessionLicenseID, base.SessionUserID,
                        base.SessionPatientID, DateTime.Now, string.Empty, string.Empty, string.Empty, base.SessionUserID,
                        base.DBID);
                }

                redirectUrl = _target;
                
            }
            else
            {
                if ((_operation & Constants.SsoOperations.OPERATION_PATIENT_SNAPSHOT) > 0)
                {
                    redirectUrl = Sso.GetRedirect(redirectUrl, Constants.PageNames.PATIENT_SNAPSHOT + "?patientGUID=" + patientGuid, IsPasswordSetupRequiredForSSOUser, this, PageState);
                }
                else if ((_operation & Constants.SsoOperations.OPERATION_PATIENT_ADD) > 0)
                {
                    PageState[Constants.SessionVariables.AddPatientSso] = CreateSsoPatient();
                    redirectUrl = Sso.GetRedirect(redirectUrl, Constants.PageNames.PROCESS_PATIENT_SSO, IsPasswordSetupRequiredForSSOUser, this, PageState);
                }
                else if (ssoMode == Constants.SSOMode.TASKMODE)
                {
                    //task mode, so set EditUser to false so they cannot access their profile and redirect to the user type specific task page
                    loadTaskModeSessionVariables();

                    var taskUrl = Sso.GetTaskRedirectUrl(PageState);

                    redirectUrl = Sso.GetRedirect(redirectUrl, taskUrl, IsPasswordSetupRequiredForSSOUser, this, PageState);
                }
                else if (ssoMode == Constants.SSOMode.EPAPATIENTLOCKDOWNTASKMODE)
                {
                    if (PageState["PATIENTID"] != null)
                    {
                        Allscripts.Impact.Patient.AddPatientToSchedule(base.SessionLicenseID, base.SessionUserID,
                            base.SessionPatientID, DateTime.Now, string.Empty, string.Empty, string.Empty, base.SessionUserID,
                            base.DBID);
                    }

                    loadTaskModeSessionVariables();

                    var taskUrl = Sso.GetTaskRedirectForEpaLockDown(PageState);

                    redirectUrl = Sso.GetRedirect(redirectUrl, taskUrl, IsPasswordSetupRequiredForSSOUser, this, PageState);
                }
                else if (ssoMode == Constants.SSOMode.SSOIDPROOFINGMODE && !redirectUrl.Contains("UserEULA.aspx"))
                {
                    redirectUrl = Sso.GetSiteUrl() + "/" + Constants.PageNames.LOGOUT + "?SSOIDDone=true";
                }
                else if (ssoMode == Constants.SSOMode.UTILITYMODE)
                {
                    if (string.IsNullOrWhiteSpace(PageState.GetStringOrEmpty("SITEID")))
                    {
                        PageState["SITEID"] = 1;// setting siteid 1 as default site
                    }

                    redirectUrl = Sso.GetRedirect(redirectUrl, Constants.PageNames.MY_PROFILE, IsPasswordSetupRequiredForSSOUser, this, PageState);
                }
                else //patient context
                {
                    logger.Debug("PatientContext");
                    if (Session["PATIENTID"] != null)
                    {
                        PageState[Constants.SessionVariables.SSOMode] = PageState[Constants.SessionVariables.SSOMode] ?? Constants.SSOMode.PatientContext;
                        Allscripts.Impact.Patient.AddPatientToSchedule(SessionLicenseID, SessionUserID, SessionPatientID,
                            DateTime.Now, string.Empty, string.Empty, string.Empty, SessionUserID, DBID);
                    }

                    //TEJ - 11/15/2007 - Added passing of Health Care Plan 
                    if (passPatientData)
                    {
                        Sso.AddPatientCoverage(doc, PageState, DBID);
                    }

                    var nextPage = Constants.PageNames.SELECT_PATIENT;
                    if (ssoMode == Constants.SSOMode.PATIENTLOCKDOWNMODE)
                    {
                        var defaultPatientLockDownPage = PageState.GetStringOrEmpty(Constants.SessionVariables.DefaultPatientLockDownPage);
                        if (!string.IsNullOrWhiteSpace(defaultPatientLockDownPage))
                        {
                            nextPage = defaultPatientLockDownPage;
                        }
                    }

                    redirectUrl = Sso.GetRedirect(redirectUrl, nextPage, IsPasswordSetupRequiredForSSOUser, this, PageState);                    
                    
                    if (Request.UrlReferrer == null)
                    {
                        int pos = 0;
                        pos = redirectUrl.IndexOf("~");
                        if (pos > 0 || redirectUrl.Substring(0, 1) == "~")
                        {
                            redirectUrl = $"{GetHttpsSiteUrl()}/{redirectUrl.Substring(pos + 1)}";
                        }


                        var htmlText = createBrowserRedirect(redirectUrl);

                        using (var streamWriter = new StreamWriter(Response.OutputStream))
                        {
                            streamWriter.Write(htmlText);
                        }
                    }
                    else
                    {
                        PageState["LoggedInMode"] = "Sso";
                        logger.Debug("processBrowserPost(); Final SSO Redirect PatientContext: {0}", redirectUrl);
                    }
                }
            }



            Redirect(redirectUrl, endRequest: false);
        }

       

        public void Redirect(string location, string errorMsg = null, bool? endRequest = null)
        {
            if (errorMsg != null)
            {
                PageState["SSOErrorMessage"] = errorMsg;
                logger.Error("Redirecting to: {0} with error: {1}", location, errorMsg);
            }
            else
            {
                logger.Debug("Redirecting to: {0}", location);              
            }

            if (endRequest.HasValue)               
                Response.Redirect(Constants.PageNames.UrlForRedirection(location), endRequest.Value);               
           
                
            Response.Redirect(Constants.PageNames.UrlForRedirection(location));
        }

        internal string AuthenticateSsoUser(Guid userGuid, Partner partner, string redirectUrl, ISso sso)
        {
            string redirect = null;

            redirect = sso.CheckForActivationWizardRedirect(partner, userGuid, PageState, new EPSBroker());
            if (redirect != null) Redirect(redirect);

            var linkFederatedUserResponse = LinkAndAuthorizeUser(userGuid, partner);

            redirectUrl = ProcessAuthStatus(partner, redirectUrl, linkFederatedUserResponse);

            return redirectUrl;
        }

        internal string ProcessAuthStatus(Partner partner, string redirectUrl, AuthenticateAndAuthorizeUserResponse linkFederatedUserResponse)
        {
            string errorMsg = null;
            switch (linkFederatedUserResponse.AuthenticationStatus)
            {
                case AuthenticationStatuses.SHIELD_AUTH:
                    {
                        errorMsg = ShieldAuthOnly(partner, ref redirectUrl, linkFederatedUserResponse);
                        if (errorMsg != null) Redirect(Constants.PageNames.SAML_SSO_ERROR, errorMsg);

                        break;
                    }
                case AuthenticationStatuses.NOT_AUTHORIZED:
                    {
                        throw new ApplicationException("Invalid Sso attempt.");
                    }
            }
            return redirectUrl;
        }

        private string ShieldAuthOnly(Partner partner, ref string redirectUrl, AuthenticateAndAuthorizeUserResponse linkFederatedUserResponse)
        {
            PageState["IsLicenseShieldEnabled"] = linkFederatedUserResponse.AuthenticatedShieldUsers[0].IsLicenseShieldEnabled;
            PageState["LastLoginDateUTC"] = linkFederatedUserResponse.AuthenticatedShieldUsers[0].LastLoginDateUTC;
            PageState[Constants.SessionVariables.HomeAddressCheckStatus] = linkFederatedUserResponse.AuthenticatedShieldUsers[0].HomeAddressCheckStatus;
            PageState[Constants.SessionVariables.ShieldIdentityToken] = linkFederatedUserResponse.AuthenticatedShieldUsers[0].ShieldIdentityToken;
            PageState[Constants.SessionVariables.PartnerAllowsUserNameAndPassword] = partner.AllowsUserNameAndPassword;

            _login.UpdateUserLastLoginDate(linkFederatedUserResponse);

            _login.SetAuthenticationCookieForShieldUser(linkFederatedUserResponse.AuthenticatedShieldUsers[0]);

            var errorMessage = string.Empty;
            var isUserSessionSet = _login.SetShieldSSOAccountInfo(linkFederatedUserResponse, PageState.GetStringOrEmpty(Constants.SessionVariables.SiteId), ref redirectUrl, ref errorMessage);
            
            if (!isUserSessionSet)
            {
                return errorMessage;
            }

            PageState.Remove(Constants.SessionVariables.ForcePasswordSetupForSSOUser);
            PageState.Remove("PasswordExpiredForSSOUser");
        
            if (partner.AllowsUserNameAndPassword)
            {
                // Check here if the user is required to setup a password. This is mainly used for EPCS for Sso users.
                if (linkFederatedUserResponse.AuthenticatedShieldUsers[0].IsUsernameAndPasswordSetupNeeded)
                {
                    PageState[Constants.SessionVariables.ForcePasswordSetupForSSOUser] = true;
                }
                else
                {
                    if (linkFederatedUserResponse.AuthenticatedShieldUsers[0].IsPasswordExpired 
                        && UserInfo.IsPermissionGranted(UserPermissions.EpcsCanPrescribe, PageState))
                    {
                        PageState["PasswordExpiredForSSOUser"] = true;                        
                    }
                                        
                    PageState[Constants.SessionVariables.DaysLeftBeforePasswordExpires] = Sso.GetShieldPasswordExpirationDays(linkFederatedUserResponse.AuthenticatedShieldUsers[0].ShieldLoginID);                    
                }
            }

            if (!string.IsNullOrEmpty(redirectUrl) && !redirectUrl.Contains(Constants.PageNames.USER_EULA))
            {
                redirectUrl = getRedirectURLForUser(redirectUrl);
            }
            return null;
        }

        internal AuthenticateAndAuthorizeUserResponse LinkAndAuthorizeUser(Guid userGuid, Partner partner)
        {
            logger.Debug("Calling LinkAndAuthorizeFederatedUser for user Guid {0}", userGuid);
            AuthenticateAndAuthorizeUserResponse federatedUserResponse = null;
            try
            {
                federatedUserResponse = EPSBroker.LinkAndAuthorizeFederatedUser(userGuid.ToString(), Request.UserIpAddress());
            }
            catch (Exception e)
            {
                Audit.PartnerInsert(partner.ID, userGuid.ToString(), Request.UserIpAddress(), "AuthenticateSsoUser", e.ToString(), DBID);
                Redirect(Constants.PageNames.SAML_SSO_ERROR, "Invalid Sso attempt.");
            }
            return federatedUserResponse;
        }

        private void VerifyPatientGuid(Guid patientGuid, Guid userGuid, Partner partner)
        {
            DataSet returnDataSet = RxUser.SSOVerifyPatientGuid(patientGuid.ToString(), userGuid.ToString(), base.DBID);
            if (returnDataSet.Tables[0].Rows.Count == 0)
            {
                string errMsg = "You are attempting to access a patient that does not belong to your practice. Please be sure that both the user and the patient belong to the same license.";
                Audit.PartnerInsert(partner.ID, userGuid.ToString(), Request.UserIpAddress(), "VerifyPatientGuid",
                    errMsg + "; userGuid: " + userGuid.ToString() + "; patientGuid: " + patientGuid.ToString(), DBID);
                Redirect(Constants.PageNames.SAML_SSO_ERROR, errMsg);
            }
        }

        private Guid GetAndSetPatientGuid(SsoAttributes attributeStatements)
        {
            bool isMultiplePatientsFound;
            var patientGuid = Sso.GetPatientGuidGetWithAttribute(attributeStatements, out isMultiplePatientsFound, new Patient(), DBID);

            if (isMultiplePatientsFound)
            {
                RedirectToMultipleMrnMatchesPage(attributeStatements);
            }

            if (patientGuid.ToString() != string.Empty)
            {
                PageState["PATIENTID"] = patientGuid.ToString();
            }
            return patientGuid;
        }

        private void RedirectToMultipleMrnMatchesPage(SsoAttributes attributeStatements)
        {
            var queryString = Sso.GetSiteUrl() + "/" + Constants.PageNames.PROCESS_PATIENT_SSO +
                              "?SSOMode=MultipleMRNMatches&MRN=" + attributeStatements.PatientMrn;
            Response.Redirect(queryString, false);
        }

        internal bool VerifyEpaPartner(bool isePaPartner, SsoAttributes attributeStatements, IStateContainer pageState)
        {
            if (isePaPartner)
            {
                //First check if it is ePAPatientLockdowntaskmode, if its not then dont sso
                if (attributeStatements.SsoMode != string.Empty)
                {
                    if (!Sso.IsEpaTaskMode(attributeStatements.SsoMode))
                    {
                        pageState["SSOErrorMessage"] =
                            "Only Standard Sso mode and ePA Lockdown mode are is supported for ePA Partner.";
                        pageState["SSOErrorStackTrace"] = string.Empty;
                        return false;
                    }
                }
            }
            return true;
        }

        private SsoPatient CreateSsoPatient()
        {
            var patient = new SsoPatient
            {
                Name = new Name(Request.Form["FirstName"], Request.Form["LastName"], Request.Form["MiddleName"]),
                Address = new Address
                {
                    Address1 = Request.Form["Address1"],
                    Address2 = Request.Form["Address2"],
                    City = Request.Form["City"],
                    State = Request.Form["State"],
                    ZIPCode = Request.Form["Zip"]
                },
                Dob = Request.Form["DOB"],
                Mrn = Request.Form["MRN"],
                PatientGuid = Request.Form["PatientGUID"],
                Phone = Request.Form["Phone"],
                MobilePhone = Request.Form["MobilePhone"],
                Gender = Request.Form["Gender"],
                PlanId = Request.Form["Planid"],
                PlanName = Request.Form["PlanName"],
                NcpdpNo = Request.Form["NCPDPNo"],
                PharmacySt = Request.Form["PharmacySt"]
            };

            //only get plan info from CCR if the form vars are empty
            if (passPatientData && !string.IsNullOrEmpty(patient.PlanId))
            {
                foreach (XmlNode node in doc.GetElementsByTagName("plan-info"))
                {
                    XmlNodeList objXmlNodeList = node.ChildNodes;
                    foreach (XmlNode ChildNode in objXmlNodeList)
                    {
                        if (ChildNode.Name == "planid")
                        {
                            patient.PlanId = ChildNode.InnerText;
                        }
                        if (ChildNode.Name == "planname")
                        {
                            patient.PlanName = ChildNode.InnerText;
                        }
                    }
                }
            }

            if (passPatientData)
            {
                patient.PatientDoc = patientdoc;
            }

            return patient;
        }

        private string createUserGuidResponseXML(string userGUID, string userName, string deaStatus, string msg)
        {
            string returnValue;
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append("<?xml version=\"1.0\" ?>");
            stringBuilder.Append("<user-profile>");
            stringBuilder.AppendFormat("<user-guid>{0}</user-guid>", userGUID.ToXmlEncode());
            stringBuilder.AppendFormat("<user-name>{0}</user-name>", userName.ToXmlEncode());
            stringBuilder.AppendFormat("<dea-status>{0}</dea-status>", deaStatus.ToXmlEncode());
            stringBuilder.AppendFormat("<message>{0}</message>", msg.ToXmlEncode());
            stringBuilder.Append("</user-profile>");

            returnValue = stringBuilder.ToString();

            return returnValue;
        }

        private string getRedirectURLForUser(string redirectURL)
        {
            logger.Debug("getRedirectURLForUser");
            HttpCookie eRxNowSiteCookie;
            string licenseID = string.Empty;
            int siteID = 1;

            logger.Debug("<Cookie>{0}</Cookie><SessionAHSAccountID>{1}</SessionAHSAccountID>", Request.Cookies["eRxNowSiteCookie"], Session["AHSAccountID"]);
            if (Request.Cookies["eRxNowSiteCookie"] != null && Session["AHSAccountID"] != null)
            {
                eRxNowSiteCookie = Request.Cookies.Get("eRxNowSiteCookie");
                logger.Debug("<Cookie><License>{0}</License><SaveSite>{1}</SaveSite></Cookie>", eRxNowSiteCookie["License"], eRxNowSiteCookie["SaveSite"]);
                licenseID = eRxNowSiteCookie["License"];
                siteID = Convert.ToInt32(eRxNowSiteCookie["SaveSite"]);
                ePrescribeSvc.License license = EPSBroker.GetLicenseByID(licenseID);

                if (license.AccountID == Session["AHSAccountID"].ToString())
                {
                    logger.Debug("Cookie matches!");
                    //cookie matches with login
                    redirectURL = redirectURL.Replace("~/" + Constants.PageNames.SELECT_ACCOUNT_AND_SITE + "?", string.Empty);

                    DataSet dsSite = ApplicationLicense.SiteLoad(licenseID, siteID, base.DBID);

                    if (dsSite != null && dsSite.Tables.Count > 0 && dsSite.Tables[0].Rows.Count > 0)
                    {
                        logger.Debug("dsSite Loaded: {0}", dsSite.ToLogString());
                        Session["MULTIPLESITES"] = true;
                        //set additional session variable from Cookie
                        Session["SITEID"] = siteID;
                        Session["PRACTICESTATE"] = dsSite.Tables[0].Rows[0]["State"];
                        Session["SITENAME"] = dsSite.Tables[0].Rows[0]["SiteName"];
                        Session["SITE_PHARMACY_ID"] = dsSite.Tables[0].Rows[0]["PharmacyID"];
                        Session["SiteEPCSAuthorizedSchedules"] = GetSiteEpcsSchedules(dsSite);
                        Session["ISCSREGISTRYCHECKREQ"] = dsSite.Tables[0].Rows[0]["IsCSRegistryCheckRequired"];
                        Session["STATEREGISTRYURL"] = dsSite.Tables[0].Rows[0]["StateRegistryURL"];

                        if ((Convert.ToInt32(Session["UserType"]) == Convert.ToInt32(Constants.UserCategory.PROVIDER)) ||
                            (Convert.ToInt32(Session["UserType"]) == Convert.ToInt32(Constants.UserCategory.PHYSICIAN_ASSISTANT)) ||
                            (Convert.ToInt32(Session["UserType"]) == Convert.ToInt32(Constants.UserCategory.PHYSICIAN_ASSISTANT_SUPERVISED)))
                        {
                            bool hasValidStateLicense = false;

                            hasValidStateLicense = RxUser.VerifyUserStateLicense(
                                base.SessionUserID,
                                Session["PRACTICESTATE"].ToString(),
                                base.DBID);

                            if (!hasValidStateLicense)
                            {
                                if (!redirectURL.ToLower().Contains(Constants.PageNames.EDIT_USER.ToLower()))
                                {
                                    redirectURL = string.Concat("~/" + Constants.PageNames.EDIT_USER + "?Status=NoLic&TargetUrl=", redirectURL.Replace("~/", string.Empty));
                                }
                            }
                            else
                            {
                                redirectURL = string.Concat(Sso.GetSiteUrl(), "/" + Constants.PageNames.SELECT_PATIENT.ToLower());
                            }
                        }
                        else
                        {
                            redirectURL = string.Concat(Sso.GetSiteUrl(), "/" + Constants.PageNames.SELECT_PATIENT);
                        }

                       
                        string printingOption = "1";

                        //first check to see if the state of the site is eligible for 4up.
                        if (SystemConfig.GetStatePrintFormats(Session["PRACTICESTATE"].ToString()) > 0)
                        {
                            string printingPreference = SitePreference.GetPreference(SessionLicenseID, base.SessionSiteID, "PRINTINGPREFERENCE", base.DBID);
                            if (printingPreference != null)
                            {
                                printingOption = printingPreference;
                            }
                        }

                        Session["PRINTINGPREFERENCE"] = printingOption;
                        Session["GenericEquivalentSearch"] = SitePreference.GetPreference(Session["LICENSEID"].ToString(), base.SessionSiteID, "GenericEquivalentSearch", base.DBID);

                        if (base.SessionLicense.EnterpriseClient.ShowECoupon)
                        {
                            Session["APPLYFINANCIALOFFERS"] = SitePreference.GetPreference(base.SessionLicenseID, base.SessionSiteID, "APPLYFINANCIALOFFERS", base.DBID);
                        }

                        if (base.SessionLicense.EnterpriseClient.ShowSponsoredLinks)
                        {
                            Session["DisplayInfoScripts"] = SitePreference.GetPreference(base.SessionLicenseID, base.SessionSiteID, "DISPLAYINFOSCRIPTS", base.DBID);
                        }
                    }
                }

                logger.Debug("Cookie did NOT match");
            }

            return redirectURL;
        }

        #endregion

        private static List<string> GetSiteEpcsSchedules(DataSet dsSite)
        {
            List<string> siteEPCSAuthorizedSchedules = new List<string>();
            if (dsSite.Tables[1] != null & dsSite.Tables[1].Rows.Count > 0)
            {
                siteEPCSAuthorizedSchedules.AddRange(from DataRow drSchedule in dsSite.Tables[1].Rows select Convert.ToString(drSchedule["Schedule"]));
            }

            logger.Debug("GetSiteEpcsSchedules: {0}", siteEPCSAuthorizedSchedules.ToLogString());
            return siteEPCSAuthorizedSchedules;
        }
    }
}
