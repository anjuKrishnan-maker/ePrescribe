using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.Objects;
using Allscripts.ePrescribe.Shared.Logging;
using Allscripts.Impact;
using eRxWeb.ePrescribeSvc;

namespace eRxWeb
{
    public partial class AssistRxLaunchPage : BasePage
    {
        private static ILoggerEx logger = LoggerEx.GetLogger();
        protected void Page_Load(object sender, EventArgs e)
        {
            var targetUrl = Request.QueryString["TargetUrl"];
            var samlToken = PageState.Cast(Constants.SessionVariables.iAssistTokenObject, new GetiAssistSamlTokenResponse()).Base64SamlToken;

            if (!string.IsNullOrEmpty(targetUrl) && !string.IsNullOrEmpty(samlToken))
            {
                // Create the HTML form and return it to the browser.
                var hiddenForms = new List<HiddenFormElement>
                {
                    new HiddenFormElement {Name = "Authorization", Value =  samlToken}
                };

                var html = HtmlUtility.BuildHtmlPost(targetUrl, hiddenForms);

                var streamWriter = new StreamWriter(Response.OutputStream);
                logger.Debug($"AssistRxLaunchPage HTML from AssistRx = <TargetUrl>{targetUrl}</TargetUrl><HTMLResponse>{html}</HTMLResponse>");
                streamWriter.Write(html);
                streamWriter.Close();
            }
        }
    }
}