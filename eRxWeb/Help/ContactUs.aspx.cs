using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Net.Mail;
using Allscripts.Impact;
using Allscripts.ePrescribe.Common;
using eRxWeb.AppCode;
using System.Web.Services;
using eRxWeb.State;

namespace eRxWeb
{
    public partial class ContactUs : BasePage
    {
        
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.Form.Attributes.Add("autocomplete", "off");
                ((HelpMasterPage)this.Master).CurrentPage = HelpMasterPage.HelpPage.CONTACT;


                if (!string.IsNullOrEmpty(base.SessionUserID))
                {
                    DataRow drUser = Allscripts.Impact.RxUser.GetUserBasicInfo(base.SessionUserID, base.DBID);

                    if (drUser != null)
                    {
                        user_name.Value = drUser["UserName"].ToString();
                        first_name.Value = drUser["FirstName"].ToString();
                        last_name.Value = drUser["LastName"].ToString();
                        email.Value = drUser["Email"].ToString();
                        account_name.Value = drUser["LicenseName"].ToString();
                        account_number.Value = drUser["AccountID"].ToString();

                        disableControls();
                    }
                }
                else
                {
                    enableControls();
                    resetButtons();
                }
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (IsValidData())
            {
                SendSupportEmail();
            }
        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            resetButtons();
        }

        private void disableControls()
        {
            first_name.Disabled = true;
            last_name.Disabled = true;
            email.Disabled = true;
            user_name.Disabled = true;
            account_name.Disabled = true;
            account_number.Disabled = true;
        }

        private void enableControls()
        {
            first_name.Disabled = false;
            last_name.Disabled = false;
            email.Disabled = false;
            user_name.Disabled = false;
            account_name.Disabled = false;
            account_number.Disabled = false;
        }

        private void resetButtons()
        {
            if (!string.IsNullOrEmpty(base.SessionUserID))
            {
                ShortDesc.Value = string.Empty;
                comments.Value = string.Empty;
                ucMessage.MessageText = string.Empty;
                ucMessage.Visible = false;
                txtCapchaResponse.Text = string.Empty;
            }
            else
            {
                first_name.Value = string.Empty;
                last_name.Value = string.Empty;
                email.Value = string.Empty;
                user_name.Value = string.Empty;
                account_name.Value = string.Empty;
                account_number.Value = string.Empty;
                ShortDesc.Value = string.Empty;
                comments.Value = string.Empty;
                ucMessage.MessageText = string.Empty;
                ucMessage.Visible = false;
                txtCapchaResponse.Text = string.Empty;
            }
        }

        private void SendSupportEmail()
        {
            string exceptionmessage = string.Empty;
            ucMessage.MessageText = string.Empty;
            ucMessage.Visible = false;
            try
            {
                string emailAddress = email.Value;
                if (string.IsNullOrEmpty(emailAddress))
                {
                    emailAddress = ConfigurationManager.AppSettings["EmailFrom"].ToString();
                }

                string body = string.Empty;

                body = body + "First Name: " + first_name.Value + "\n\r";
                body = body + "Last Name: " + last_name.Value + "\n\r";
                body = body + "Account Name: " + account_name.Value + "\n\r";
                body = body + "E-mail: " + email.Value + "\n\r";
                body = body + "Short Description: " + ShortDesc.Value + "\n\r";
                body = body + "Message: " + comments.Value + "\n\r";

                MailMessage mail = new MailMessage();
                MailAddress from = new MailAddress(email.Value);
                mail.From = from;
                mail.Sender = from;
                mail.To.Add("eprescribesupport@allscripts.com");
                mail.Subject = "Veradigm ePrescribe Contact Submission";
                mail.IsBodyHtml = true;
                mail.Body = body;
                MailNotifier.SendMail(mail);
            }
            catch (Exception ex)
            {
                exceptionmessage = ex.Message;
                ucMessage.MessageText = exceptionmessage;
                ucMessage.Visible = true;
                ucMessage.Icon = Controls_Message.MessageType.ERROR;

            }

            if (exceptionmessage == String.Empty)
            {
                //this is /help/default.aspx, so no need to update
                Response.Redirect(Constants.PageNames.HELP_DEFAULT + "?M=1");
            }
        }

        public void ResetCaptcha(object sender, EventArgs e)
        {
            imgCapcha.Src = "../CreateCaptcha.aspx";
        }
        private bool IsValidCaptcha(string text)
        {
            return PageState.GetStringOrEmpty(Constants.CaptchaText).Equals(text, StringComparison.OrdinalIgnoreCase);
        }

        private bool IsValidData()
        {
            var isValid = true;
            if (string.IsNullOrWhiteSpace(comments.Value) && (string.IsNullOrWhiteSpace(ShortDesc.Value)))
            {
                ucMessage.MessageText = "Please enter short description and comments.";
                isValid = false;
                ucMessage.Icon = Controls_Message.MessageType.ERROR;
            }
            else if (string.IsNullOrWhiteSpace(comments.Value))
            {
                ucMessage.MessageText = "Please enter comments.";
                isValid = false;
                ucMessage.Icon = Controls_Message.MessageType.ERROR;
            }
            else if (string.IsNullOrWhiteSpace(ShortDesc.Value))
            {
                ucMessage.MessageText = "Please enter short description.";
                isValid = false;
                ucMessage.Icon = Controls_Message.MessageType.ERROR;
            }
            else if (string.IsNullOrWhiteSpace(txtCapchaResponse.Text))
            {
                ucMessage.MessageText = "Please enter captcha.";
                isValid = false;
                ucMessage.Icon = Controls_Message.MessageType.ERROR;
            }
            else if (!IsValidCaptcha(txtCapchaResponse.Text))
            {
                ucMessage.MessageText = "Please enter valid captcha";
                isValid = false;
                ucMessage.Icon = Controls_Message.MessageType.ERROR;
            }

            if (!isValid)
            {
                ucMessage.Visible = true;
            }
            else
            {
                ucMessage.Visible = false;
            }

            return isValid;
        }
    }
}