using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web;
using System.Web.SessionState;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Data;
using System.Net;
using System.Text;
using Allscripts.ePrescribe.DatabaseSelector;
using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.Shared.Logging;
using Allscripts.Impact;
using System.Web.Http;
using System.Web.Routing;
using Allscripts.ePrescribe.Data;
using Allscripts.ePrescribe.Objects;
using eRxWeb.AppCode;
using eRxWeb.State;
using Microsoft.ApplicationInsights.Extensibility;
using Veradigm.AdAgent.Business;
using ConfigKeys = Allscripts.Impact.ConfigKeys;
using System.Configuration;
using Hl7.Fhir.Model;
using ServiceStack;
using static Allscripts.ePrescribe.Common.Constants;
using System.Linq;
using Allscripts.ePrescribe.Shared.Data;

namespace eRxWeb
{
    /// <summary>
    /// Summary description for Global.
    /// </summary>
    public class Global : System.Web.HttpApplication
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        private static readonly ILoggerEx logger = LoggerEx.GetLogger();
        private AdConfigCacheHandler _adConfigCacheHandler;
        private object configHandlerLock = new object();

        public const string App_Partners = "Partners";

        public static string CurrentSessionDBError = string.Empty;

        public void LoadPartners()
        {
            try
            {
                Partners partners = new Partners();

                if (Application["Partners"] != null)
                {
                    Application.Remove("Partners");
                }

                Application["Partners"] = partners;
            }
            catch (Exception ex)
            {
                try
                {
                    //could not load partners
                    Allscripts.Impact.Audit.AddException(Guid.Empty.ToString(), Guid.Empty.ToString(), "Could not load partners from App_Start " + ex.ToString(), "", "", "", ConnectionStringPointer.ERXDB_DEFAULT);
                }
                catch (Exception finalex)
                {
                    try
                    {
                        //we've pretty much caught everything we're gonna catch
                        System.IO.TextWriter writer = new System.IO.StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "\\eRxNOWErrorLog.txt", true);
                        writer.WriteLine(DateTime.Now.ToString() + " An error occurred while creating an error: " + ex.ToString() + " " + finalex.ToString());
                        writer.Close();
                    }
                    catch { }
                }
            }
        }

        private void loadShieldData()
        {
            try
            {
                //load internal AppID for ePrescribe
                if (Application["ShieldInternalAppID"] != null)
                {
                    Application.Remove("ShieldInternalAppID");
                }

                Application["ShieldInternalAppID"] = EPSBroker.GetShieldInternalAppID();
            }
            catch (Exception ex)
            {
                Allscripts.Impact.Audit.AddException(Guid.Empty.ToString(), Guid.Empty.ToString(), "Could not load Shield data from App_Start " + ex.ToString(), "", "", "", ConnectionStringPointer.ERXDB_DEFAULT);
            }
        }

        public Global()
        {
            InitializeComponent();
        }
        void RegisterPageRoutes(RouteCollection routes)
        {
            string spaPage = "~/Spa.aspx";
            routes.MapPageRoute("Prescribe", "patient/{*params}", spaPage, false);
            routes.MapPageRoute("General", "general/{*params}", spaPage, false);
            routes.MapPageRoute("Register", "register/{action}", "~/Register.aspx", false, new RouteValueDictionary { { "action", "subscriptions" } });
        }

        protected void Application_Start(Object sender, EventArgs e)
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            LoadPartners();
            loadShieldData();
            InitLog();
            InitAppInsights();
            RegisterPageRoutes(RouteTable.Routes);
        }

        private void InitAppInsights()
        {
#if !DEBUG
        TelemetryConfiguration.Active.InstrumentationKey = ConfigKeys.AppInsightsKey;
        TelemetryConfiguration.Active.TelemetryInitializers.Add(new TelemetryInitializer("ePrescribe"));
#endif
        }

        private void InitAdAgent(IAdConfigData adConfigData)
        {

            try
            {
                StateContainer container = null;

                if (HttpContext.Current != null)
                {
                    container = new StateContainer(HttpContext.Current.Session);
                }

                var adInit = new AgentInitializer(new AdAgentLogger(container, new Audit()), null);
                var adConfig = adConfigData.Retrieve();


                adInit.LoadAgentConfiguration(adConfig.ConfigJson, adConfig.TargetingDictionaries);

                if (_adConfigCacheHandler == null)
                {
                    lock (configHandlerLock)
                    {
                        if (_adConfigCacheHandler == null)
                        {
                            _adConfigCacheHandler = new AdConfigCacheHandler(TimeSpan.FromHours(1).TotalMilliseconds, adConfigData, adInit);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("InitAdAgent - " + ex);
                Audit.AddException(Guid.Empty.ToString(), Guid.Empty.ToString(),
                    $"Could not init AdAgent {ex}", "", "", "",
                    ConnectionStringPointer.ERXDB_DEFAULT);
            }
        }

        private static void InitLog()
        {
            try
            {
                string logInfo = ConfigKeys.LoggingConfigMainApp;
                LogInitializer.Init(logInfo);
            }
            catch (Exception ex)
            {
                Audit.AddException(Guid.Empty.ToString(), Guid.Empty.ToString(),
                    "Could not init logging data" + ex.ToString(), "", "", "",
                    ConnectionStringPointer.ERXDB_DEFAULT);
            }
        }
        public override void Init()
        {
            this.PostAuthenticateRequest += MvcApplication_PostAuthenticateRequest;
            base.Init();
        }
        protected void MvcApplication_PostAuthenticateRequest(object sender, EventArgs e)
        {
            if (IsWebApiRequest())
            {
                HttpContext.Current.SetSessionStateBehavior(SessionStateBehavior.Required);
            }
        }

        private bool IsWebApiRequest()
        {
            return HttpContext.Current.Request.AppRelativeCurrentExecutionFilePath.ToLower().Contains(WebApiConfig.UrlPrefixRelative);
        }
        protected void Session_Start(Object sender, EventArgs e)
        {
            InitAdAgent(new AdConfigData());
        }

        protected void Application_BeginRequest(Object sender, EventArgs e)
        {
            ThreadContext.IsMainRequestThread = true;
            LocalLogContext.LogContextInfo = new LoggingInfo {UserIpAddress =  HttpContext.Current?.Request.UserIpAddress()};

            //The following code is a hack for stopping a broken image from magically appearing on SSRS reports in chrome, firefox & safari
            //where ever a line is used in the report.
            Uri u = HttpContext.Current.Request.Url;
            //If the request is from a Chrome or firefox or safari browser
            //AND a report is being generated 
            //AND there is no QSP entry named "IterationId"
            if ((HttpContext.Current.Request.Browser.Browser.ToLower().Contains("chrome") || HttpContext.Current.Request.Browser.Browser.ToLower().Contains("firefox")
                || HttpContext.Current.Request.Browser.Browser.ToLower().Contains("safari")) && u.AbsolutePath.ToLower().Contains("reserved.reportviewerwebcontrol.axd") &&
                    !u.Query.ToLower().Contains("iterationid"))
                HttpContext.Current.RewritePath(u.PathAndQuery + "&IterationId=0");
        }

        protected void Application_EndRequest(Object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(Object sender, EventArgs e)
        {
        }

        protected void Application_Error(Object sender, EventArgs e)
        {
            Exception error = Server.GetLastError();

            if (error != null)
            {

                var builder = new StringBuilder();
                var format =
                    @"<b> Error Message: </b>{0}<b> Error Source: </b>{1}<b> Error Stack Trace: </b>{2}<b> Error TargetSite: </b>{3}<br>";

                var maxDepth = 10;
                var depth = 0;

                while (depth < maxDepth)
                {
                    builder.AppendFormat(format, error.Message, error.Source, error.StackTrace, error.TargetSite);

                    error = error.InnerException;
                    if (error == null)
                    {
                        break;
                    }

                    depth++;
                    builder.Append(@"<b>Inner Exception:</b> <br>");

                }

                // when using sessionDB and getting here, Session is null. Using alternative approach to store the error.
                // may consider different implementation if there will be spikes in concurrent errors. 
                if (HttpContext.Current.Session != null)
                {
                    HttpContext.Current.Session[SessionVariables.CURRENT_ERROR] = builder.ToString();
                }
                else
                {
                    CurrentSessionDBError = builder.ToString();
                }

                Server.ClearError();

                Response.Redirect("~/" + Constants.PageNames.EXCEPTION, true);
            }
        }

        protected void Session_End(Object sender, EventArgs e)
        {

        }

        protected void Application_End(Object sender, EventArgs e)
        {
            Application.Remove("Partners");
        }
        void Application_PreSendRequestHeaders(Object sender, EventArgs e)
        {
            if (new string[] { "image", "css", "javascript", "font" }
            .Any((x) => Response.ContentType != null && Response.ContentType.ToLower().IndexOf(x) > -1))
                return;

            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();
            Response.Headers.Add("pragma", "no-cache");
        }


        protected void Application_PostRequestHandlerExecute(Object sender, EventArgs e)
        {
            logger.ReportEndOfRequest();
        }
        /// <summary>
        /// Common error-logging method.  This method is called from Exception.aspx.cs to log page-
        /// level errors, and from ScriptManagerCommon.LogException to log AJAX-related errors.
        /// </summary>
        /// <param name="errorMessage"></param>
        public static void LogError(string errorMessage)
        {
            string servervars = String.Empty;
            string sessionContent = String.Empty;

            HttpContext httpContext = HttpContext.Current;

            httpContext.Session["urlpath"] = httpContext.Request.FilePath;
            try
            {
                servervars = "<serverVariables>";
                for (int i = 0; i < httpContext.Request.ServerVariables.Count; i++)
                {
                    if (httpContext.Request.ServerVariables.Keys[i] != "ALL_HTTP" && httpContext.Request.ServerVariables.Keys[i] != "ALL_RAW")
                    {
                        if (httpContext.Request.ServerVariables[i].ToString().Length > 0)
                        {
                            servervars += "<" + httpContext.Request.ServerVariables.Keys[i].ToString() + ">" + httpContext.Request.ServerVariables[i].ToString() + "</" + httpContext.Request.ServerVariables.Keys[i].ToString() + ">";
                        }
                    }
                }
                servervars += "</serverVariables>";

                sessionContent = "<sessionContent>";
                for (int i = 0; i < httpContext.Session.Keys.Count; i++)
                {
                    if (httpContext.Session.Keys[i].ToString() != SessionVariables.CURRENT_ERROR && httpContext.Session.Keys[i].ToString() != "ShieldSecurityToken") // CurrentError is passed in as errorMessage and ShieldSecurityToken contains confidential data, so exclude it
                    {
                        if (httpContext.Session[i] != null && httpContext.Session[i].ToString().Length > 0)
                        {
                            sessionContent += "<" + httpContext.Session.Keys[i].ToString() + ">" + httpContext.Session[i].ToString() + "</" + httpContext.Session.Keys[i].ToString() + ">";
                        }
                    }
                }
                sessionContent += "</sessionContent>";

                servervars = removeSpecialCharacters(servervars);
                sessionContent = removeSpecialCharacters(sessionContent);

                string userID = Guid.Empty.ToString();
                string licenseID = Guid.Empty.ToString();

                if (httpContext.Session["USERID"] != null)
                {
                    userID = httpContext.Session["USERID"].ToString();
                }

                if (httpContext.Session["LICENSEID"] != null)
                {
                    licenseID = httpContext.Session["LICENSEID"].ToString();
                }

                httpContext.Session["CurrentErrorID"] = Allscripts.Impact.Audit.AddException(
                    userID,
                    licenseID,
                    errorMessage,
                    httpContext.Request.UserIpAddress(),
                    servervars,
                    sessionContent,
                    ((BasePage)httpContext.Handler).DBID);
            }
            catch
            {
                //if an exception is thrown, try to insert bare bone minimum info into the exception log

                httpContext.Session["CurrentErrorID"] = Allscripts.Impact.Audit.AddException(
                    Guid.Empty.ToString(),
                    Guid.Empty.ToString(),
                    errorMessage,
                    httpContext.Request.UserIpAddress(),
                    null,
                    null,
                    ((BasePage)httpContext.Handler).DBID);
            }
        }

        private static string removeSpecialCharacters(string xmlString)
        {
            return xmlString != null ? xmlString.Replace("&", "AMPERSAND").Replace("\"", "DOUBLEQUOTE").Replace("\'", "SINGLEQUOTE") : null;
        }

        #region Web Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
        }
        #endregion
    }

}