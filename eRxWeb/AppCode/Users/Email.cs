using Allscripts.ePrescribe.Shared.Logging;
using Allscripts.Impact;
using Allscripts.Impact.Utilities;
using eRxWeb.AppCode.Interfaces;
using eRxWeb.State;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Xml;
using System.Xml.Xsl;

namespace eRxWeb.AppCode.Users
{
    public class Email : IEmail
    {
        private static readonly ILoggerEx logger = LoggerEx.GetLogger();
        
        public bool Send(string personalEmail, StringWriter writer,string emailSubject)
        {
            bool successfullyEmailed = false;
            IStateContainer pageState = new StateContainer(HttpContext.Current.Session);
            try
            {
                MailMessage mail = new MailMessage();
                MailAddress from = new MailAddress(ConfigurationManager.GetValue("EmailFrom").ToString(), ConfigurationManager.GetValue("EmailFromDisplayName").ToString());
                mail.From = from;
                mail.Sender = from;
                mail.To.Add(personalEmail);
                mail.Subject = emailSubject;
                mail.IsBodyHtml = true;
                mail.Body = writer.ToString();
                MailNotifier.SendMail(mail);
                successfullyEmailed = true;
            }
            catch (Exception ex)
            {
                var errorMessage = ApiHelper.AuditException(ex.ToString(), pageState);
                logger.Error("Email Sending Exception: " + ex.ToString());
                successfullyEmailed = false;
            }
            return successfullyEmailed;
        }

        public StringWriter CreateXsltTemplate(string xslPath, string userName, string activationCode)
        {
            StringWriter writer = new StringWriter();            
            XslCompiledTransform transformContent = new XslCompiledTransform();
            XsltArgumentList xslArg = new XsltArgumentList();
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml("<Root><Name></Name><ActivationCodeURL></ActivationCodeURL><ActivationCode></ActivationCode></Root>");
            xslArg = CreateXsltArgumentList(userName,activationCode);
            transformContent.Load(xslPath);
            transformContent.Transform(xmlDoc, xslArg , writer);
            return writer;
        }

        internal XsltArgumentList CreateXsltArgumentList(string userName, string activationCode)
        {
            XsltArgumentList xslArg = new XsltArgumentList();
            xslArg.AddParam("name", "", userName);
            xslArg.AddParam("shieldActivationCodeURL", "", string.Concat(ConfigurationManager.GetValue("appurl"), "/register/activateuser"));
            xslArg.AddParam("activationCode", "", activationCode);
            xslArg.AddParam("shieldHelpURL", "", Allscripts.Impact.ConfigKeys.ShieldHelpURL);
            return xslArg;
        }
    }
}