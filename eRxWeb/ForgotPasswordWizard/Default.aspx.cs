using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Allscripts.ePrescribe.Common;
using eRxWeb.ePrescribeSvc;
using Telerik.Charting;

namespace eRxWeb.ForgotPasswordWizard
{
    public partial class Default : Page
    {
        private const string Step1 = "Step1";
        private const string Step2 = "Step2";
        private const string Success = "Success";
        private const string QuestionId = "QuestionId";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                btnSubmit.CommandName = Step1;
            }
        }

        protected void btnSubmit_OnClick(object sender, EventArgs e)
        {
            ResetElements();

            switch (btnSubmit.CommandName)
            {
                case Step1:
                {
                    if (IsStepOneValid())
                    {
                        if (!IsValidShieldUsername())
                        {
                            lblOverlayMessage.Text = "The Login ID you entered was not found. Please try again.";
                            mpeConfirmation.Show();
                            return;
                        }

                        if (!LoadSecretQuestions())
                        {
                            lblOverlayMessage.Text = "You cannot restore your password because no recovery questions were specified" +
                                                    " during account registration. Contact your administrator to initiate password recovery.";
                            mpeConfirmation.Show();
                            return;        
                        }

                        SetElementsForStep2();
                        ResetElements();
                    }
                    break;
                }
                case Step2:
                {
                    if (IsStepTwoValid())
                    {
                        var secretAnswers = PackageSecretAnswers();
                        var resetResponse = EPSBroker.ResetForgottenPassword(txtLoginId.Text, txtNewPassword.Text, secretAnswers);

                        if (resetResponse.Success)
                        {
                            btnOk.CommandName = Success;
                            lblOverlayMessage.Text = "Your password has been changed.";
                            mpeConfirmation.Show();
                        }
                        else
                        {
                            lblOverlayMessage.Text = resetResponse.Messages.FirstOrDefault().ToHTMLEncode();
                            mpeConfirmation.Show();
                        }
                    }
                    break;
                }
            }
        }

        private List<SecretAnswer> PackageSecretAnswers()
        {
            var secretAnswers = new List<ePrescribeSvc.SecretAnswer>
            {
                new SecretAnswer {Answer = txtQuestionOne.Text, QuestionID = Convert.ToInt16(txtQuestionOne.Attributes[QuestionId]) },
                new SecretAnswer {Answer = txtQuestionTwo.Text, QuestionID = Convert.ToInt16(txtQuestionTwo.Attributes[QuestionId]) },
                new SecretAnswer {Answer = txtQuestionThree.Text, QuestionID = Convert.ToInt16(txtQuestionThree.Attributes[QuestionId]) }
            };

            return secretAnswers;
        }

        private bool LoadSecretQuestions()
        {
            var questions = EPSBroker.GetUserShieldSecretQuestionsByUsername(txtLoginId.Text);
            if(questions != null && questions.Length >= 3)
            {
                lblQuestionOne.Text = questions[0].Question.ToHTMLEncode();
                txtQuestionOne.Attributes[QuestionId] = Convert.ToString(questions[0].QuestionID);

                lblQuestionTwo.Text = questions[1].Question.ToHTMLEncode();
                txtQuestionTwo.Attributes[QuestionId] = Convert.ToString(questions[1].QuestionID);

                lblQuestionThree.Text = questions[2].Question.ToHTMLEncode();
                txtQuestionThree.Attributes[QuestionId] = Convert.ToString(questions[2].QuestionID);
                return true;
            }
            return false;
        }

        private bool IsValidShieldUsername()
        {
            var userList = EPSBroker.SearchShieldUsers("", "", txtLoginId.Text);
            return userList?.Length > 0;
        }

        private bool IsStepTwoValid()
        {
            var errorMessage = string.Empty;
            errorMessage += ValidateQuestionTextbox(txtQuestionOne);
            errorMessage += ValidateQuestionTextbox(txtQuestionTwo);
            errorMessage += ValidateQuestionTextbox(txtQuestionThree);

            if (string.IsNullOrWhiteSpace(txtNewPassword.Text))
            {
                errorMessage += "Please enter a new password." + Environment.NewLine;
                txtNewPassword.CssClass = "errorBorder";
            }

            if (string.IsNullOrWhiteSpace(txtConfirmPassword.Text))
            {
                errorMessage += "Please confirm the new password." + Environment.NewLine;
                txtConfirmPassword.CssClass = "errorBorder";
            }

            errorMessage += ValidateCaptcha();

            if (!string.IsNullOrEmpty(errorMessage))
            {
                ucMessage.MessageText = errorMessage;
                ucMessage.Visible = true;
                return false;
            }
            return true;
        }

        private string ValidateQuestionTextbox(TextBox textBox)
        {
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.CssClass = "errorBorder";
                return $"Please enter an answer for question {textBox.Attributes["question"]}." + Environment.NewLine;
            }
            return "";
        }

        private void ResetElements()
        {
            ucMessage.Visible = false;
            ucMessage.MessageText = string.Empty;
            txtLoginId.CssClass = "";
            txtCaptcha.CssClass = "";
            txtQuestionOne.CssClass = "";
            txtQuestionTwo.CssClass = "";
            txtQuestionThree.CssClass = "";
            txtConfirmPassword.CssClass = "";
            txtNewPassword.CssClass = "";
        }

        private bool IsStepOneValid()
        {
            var errorMessage = string.Empty;
            if (string.IsNullOrWhiteSpace(txtLoginId.Text))
            {
                errorMessage += "Please enter a login id." + Environment.NewLine;
                txtLoginId.CssClass = "errorBorder";
            }

            errorMessage += ValidateCaptcha();

            if (!string.IsNullOrEmpty(errorMessage))
            {
                ucMessage.MessageText = errorMessage;
                ucMessage.Visible = true;
                return false;
            }
            return true;
        }

        private string ValidateCaptcha()
        {
            if (string.IsNullOrWhiteSpace(txtCaptcha.Text))
            {
                txtCaptcha.CssClass = "errorBorder";
                return "Please enter the captcha code." + Environment.NewLine;
            }

            if (!Convert.ToString(Session["CaptchaText"]).ToLower().Equals(txtCaptcha.Text.ToLower()))
            {
                txtCaptcha.CssClass = "errorBorder";
                txtCaptcha.Text = "";
                return "Invalid captcha text." + Environment.NewLine;
            }

            txtCaptcha.Text = "";
            return "";
        }

        private void SetElementsForStep2()
        {
            pnlStep2.Visible = true;
            pnlStep2Text.Visible = true;
            txtLoginId.Enabled = false;
            imgCaptcha.ImageUrl = imgCaptcha.ImageUrl + "?1";
            pnlQuestions.Visible = true;
            pnlContainer.Height = 554;
            txtCaptcha.Text = "";
            txtLoginId.CssClass = "";
            txtCaptcha.CssClass = "";
            btnSubmit.CommandName = Step2;
        }

        protected void btnCancel_OnClick(object sender, EventArgs e)
        {
            Response.Redirect("~/"+Constants.PageNames.LOGIN);
        }

        protected void btnOk_OnClick(object sender, EventArgs e)
        {
            if(btnOk.CommandName == Success)
            {
                Response.Redirect("~/" + Constants.PageNames.LOGIN);
            }

            mpeConfirmation.Hide();
        }
    }
}