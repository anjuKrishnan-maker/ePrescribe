using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.DatabaseSelector;
using Allscripts.Impact;
using eRxWeb.ServerModel;
using eRxWeb.State;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Xml;
using System.Xml.Xsl;

namespace eRxWeb.AppCode.PdmpBPL
{
    public class PdmpEnrollment
    {
        public PdmpEnrollmentModel GetPdmpEnrollmentFormInfo(IStateContainer session)
        {
            ConnectionStringPointer dbID = session.Cast(Constants.SessionVariables.DbId, default(ConnectionStringPointer));
            var response = new EPSBroker().GetPdmpEnrollmentFormInfo(session.GetStringOrEmpty(Constants.SessionVariables.UserId), session.GetStringOrEmpty(Constants.SessionVariables.LicenseId), dbID);

            var model = new PdmpEnrollmentModel
            {
                Product = Constants.AppNames.ProductName,
                AccountName = response.AccountName,
                AccountNumber = response.AccountID,
                Address1 = response.Address1,
                Address2 = response.Address2,
                City = response.City,
                State = response.State,
                ZipCode = response.Zipcode,
                ContactEmail = session.GetStringOrEmpty(Constants.SessionVariables.UserEmail),
                ContactFax = $"{response.FaxAreaCode}-{response.FaxNumber}",
                ContactPhoneNo = $"{response.PhoneAreaCode}-{response.PhoneNumber}",
                ContactNameAndTitle = $"{session.GetStringOrEmpty(Constants.SessionVariables.UserFirstName)} {session.GetStringOrEmpty(Constants.SessionVariables.UserLastName)}",
                NumberOfProviders = response.ProviderCount,
                TermsAndConditions = response.TermsAndConditions,
                TermsAndConditionsID = response.TermsAndConditionsID
            };

            return model;
        }

        public PdmpEnrollmentSubmissionResponse SubmitPdmpEnrollment(IStateContainer session, PdmpEnrollmentModel enrollmentData)
        {
            try
            {
                bool isEnrollmentAllGood = true;
                Int64 OrderID = InsertAudits(session, enrollmentData);

                string OrderConfirmMessage = Allscripts.Impact.ConfigKeys.PdmpEnrollmentSubmissionMessage;

                string EmailFrom = ConfigurationManager.AppSettings[Constants.AppConfigVariables.EmailFrom].ToString();
                string EmailFromDisplayName = ConfigurationManager.AppSettings[Constants.AppConfigVariables.EmailFromDisplayName].ToString();
                string SmtpServer = ConfigurationManager.AppSettings[Constants.AppConfigVariables.SMTPServer].ToString();

                SendEnrollmentEmails(Allscripts.Impact.ConfigKeys.PdmpEnrollmentEmailConfig, EmailFrom, EmailFromDisplayName, SmtpServer, OrderID, enrollmentData);
                
                //Temporarily disabling customer confirmation mail 
                //SendCustomerEmail(EmailFrom, EmailFromDisplayName, SmtpServer, OrderID, OrderConfirmMessage, enrollmentData);

                return new PdmpEnrollmentSubmissionResponse
                {
                    IsSubmissionSuccessful = isEnrollmentAllGood,
                    EnrollmentSuccessMessage = CombineOrderIDWithMessage(OrderID, OrderConfirmMessage)
                };
            }
            catch (Exception ex)
            {
                return new PdmpEnrollmentSubmissionResponse
                {
                    IsSubmissionSuccessful = false,
                    EnrollmentSuccessMessage = ex.Message
                };
            }
        }

        private string CombineOrderIDWithMessage(Int64 orderID, string message)
        {
            return $"OrderID: {orderID} <br/><br/>{message}";
        }

        private void SendCustomerEmail(string efrom, string fromDisplayName, string smptServer, Int64 orderID, string OrderConfirmMessage, PdmpEnrollmentModel enrollmentData)
        {
            MailMessage mailMessage = new MailMessage();
            MailAddress emailfrom = new MailAddress(efrom, fromDisplayName);
            mailMessage.From = emailfrom;
            mailMessage.Sender = emailfrom;
            string xslPath = HttpContext.Current.Request.MapPath("~/EmailTemplates/PdmpEnrollmentCustomerMail.xslt");
            mailMessage.To.Add(enrollmentData.ContactEmail);

            mailMessage.Subject = "PDMP Enrollment - Order Confirmation";
            mailMessage.IsBodyHtml = true;

            string content = string.Empty;

            using (StringWriter writer = new StringWriter())
            {
                XsltArgumentList xslArg = new XsltArgumentList();
                XslCompiledTransform transformContent = new XslCompiledTransform();
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml("<Root><Name></Name><PdmpEnrollmentCustomerMail></PdmpEnrollmentCustomerMail></Root>");
                xslArg.AddParam("OrderID", "", Convert.ToString(orderID));
                xslArg.AddParam("Product", "", enrollmentData.Product);
                xslArg.AddParam("AccountName", "", enrollmentData.AccountName);
                xslArg.AddParam("AccountNumber", "", enrollmentData.AccountNumber);
                xslArg.AddParam("ContactNameAndTitle", "", enrollmentData.ContactNameAndTitle);
                xslArg.AddParam("OrderConfirmMessage", "", OrderConfirmMessage);
                xslArg.AddParam("NumberOfProviders", "", enrollmentData.NumberOfProviders);
                transformContent.Load(xslPath);
                transformContent.Transform(xmlDoc, xslArg, writer);
                content = writer.ToString();
            }

            mailMessage.Body = content;

            MailNotifier.SendMail(mailMessage);
        }

        private void SendEnrollmentEmails(EmailConfiguration config, string efrom, string fromDisplayName, string smptServer, Int64 orderID, PdmpEnrollmentModel enrollmentData)
        {
            MailMessage mailMessage = new MailMessage();
            MailAddress emailfrom = new MailAddress(efrom, fromDisplayName);
            mailMessage.From = emailfrom;
            mailMessage.Sender = emailfrom;
            string xslPath = HttpContext.Current.Request.MapPath("~/EmailTemplates/PdmpEnrollmentSupportMail.xslt");
            foreach (var toAddress in config.To)
            {
                mailMessage.To.Add(toAddress);
            }
            foreach (var ccAddress in config.CC)
            {
                mailMessage.CC.Add(ccAddress);
            }
            mailMessage.Subject = $"{config.Subject} - {fromDisplayName} - Account Number: { enrollmentData.AccountNumber} ";
            mailMessage.IsBodyHtml = true;

            string content = string.Empty;

            using (StringWriter writer = new StringWriter())
            {
                XsltArgumentList xslArg = new XsltArgumentList();
                XslCompiledTransform transformContent = new XslCompiledTransform();
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml("<Root><Name></Name><PdmpEnrollmentSupportMail></PdmpEnrollmentSupportMail></Root>");
                xslArg.AddParam("OrderID", "", Convert.ToString(orderID));
                xslArg.AddParam("Product", "", enrollmentData.Product);
                xslArg.AddParam("AccountName", "", enrollmentData.AccountName);
                xslArg.AddParam("AccountNumber", "", enrollmentData.AccountNumber);
                xslArg.AddParam("Address1", "", enrollmentData.Address1);
                xslArg.AddParam("Address2", "", enrollmentData.Address2 ?? string.Empty);
                xslArg.AddParam("City", "", enrollmentData.City);
                xslArg.AddParam("State", "", enrollmentData.State);
                xslArg.AddParam("ZipCode", "", enrollmentData.ZipCode);
                xslArg.AddParam("ContactNameAndTitle", "", enrollmentData.ContactNameAndTitle);
                xslArg.AddParam("ContactNumber", "", enrollmentData.ContactPhoneNo);
                xslArg.AddParam("ContactFax", "", enrollmentData.ContactFax);
                xslArg.AddParam("ContactEmail", "", enrollmentData.ContactEmail);
                xslArg.AddParam("NumberOfProviders", "", enrollmentData.NumberOfProviders);
                transformContent.Load(xslPath);
                transformContent.Transform(xmlDoc, xslArg, writer);
                content = writer.ToString();
            }

            mailMessage.Body = content;

            MailNotifier.SendMail(mailMessage);
        }

        public Int64 InsertAudits(IStateContainer session, PdmpEnrollmentModel enrollmentData)
        {
            ConnectionStringPointer dbID = session.Cast(Constants.SessionVariables.DbId, default(ConnectionStringPointer));
            return EPSBroker.InsertAudits(session.GetStringOrEmpty(Constants.SessionVariables.UserId), session.GetStringOrEmpty(Constants.SessionVariables.LicenseId), enrollmentData, dbID);
        }

    }
}