using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Allscripts.Impact;
using Allscripts.ePrescribe.Common;

namespace eRxWeb
{
    public partial class Controls_ePAInitiationQuestionChooseAny : BaseControl
    {
        private EPAInitQuestionSetDetail _baseQuestion = new EPAInitQuestionSetDetail();
        private EPAInitQuestionSet _baseQuestionSet
        {
            get
            {
                EPAInitQuestionSet ret = new EPAInitQuestionSet();
                if (Session["ePACurrentInitiationQuestionSet"] != null)
                {
                    ret = (EPAInitQuestionSet)(Session["ePACurrentInitiationQuestionSet"]);
                }
                else
                {
                    ret = new EPAInitQuestionSet();
                }

                return ret;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            string questionID = this.Attributes["CurrentQuestionID"];
            for (int i = 0; i < _baseQuestionSet.Questions.Count; i++)
            {
                if (string.Compare(questionID, _baseQuestionSet.Questions[i].QuestionID, true) == 0)
                {
                    _baseQuestion = _baseQuestionSet.Questions[i];
                }
            }

            if (_baseQuestionSet.StartingQuestionID == _baseQuestion.QuestionID)
            {
                questionHolder.Style["display"] = "inline";
            }
            else
            {
                questionHolder.Style["display"] = "none";
            }

            questionHolder.Attributes.Add("ePAQuestionID", _baseQuestion.QuestionID);
            questionHolder.Attributes.Add("ePACurrentQuestionType", "CA");
            questionHolder.Attributes.Add("ePADefaultNextQuestionID", _baseQuestion.DefaultNextQuestionID);

            questionTitle.InnerText = _baseQuestion.QuestionText;
            questionProgress.InnerText = _baseQuestionSet.SequenceOf(_baseQuestion.QuestionID).ToString() + " / " + (_baseQuestionSet.Questions.Count).ToString();

            for (int index = 0; index < _baseQuestion.Answers.Count; index++)
            {
                HtmlGenericControl holder = new HtmlGenericControl();
                HtmlGenericControl text = new HtmlGenericControl();
                HtmlInputCheckBox option = new HtmlInputCheckBox();

                HtmlGenericControl answerFreeTextDiv = new HtmlGenericControl();
                HtmlTable answerFreeTextTab = new HtmlTable();
                HtmlTableRow tempRow;
                HtmlTableCell temlCol;
                HtmlTextArea answerFreeText = new HtmlTextArea();
                HtmlGenericControl textCount = new HtmlGenericControl("span");
                HtmlGenericControl staticFreeTextTitle = new HtmlGenericControl();

                staticFreeTextTitle.InnerText = "Additional Comments (2000 character max)";
                
                textCount.InnerText = "[0 of 2000]";
                
                answerFreeText.Attributes.Add("id", "AddlFreeTextCtrl" + _baseQuestion.Answers[index].AnswerChoiceID);
                answerFreeText.Attributes.Add("runat", "server");
                answerFreeText.Attributes.Add("rows", "2");
                answerFreeText.Attributes.Add("cols", "50");
                answerFreeText.Attributes.Add("class", "ePaTextarea");

                tempRow = new HtmlTableRow();
                temlCol = new HtmlTableCell();
                temlCol.Controls.Add(staticFreeTextTitle);
                tempRow.Controls.Add(temlCol);
               
                temlCol = new HtmlTableCell();
                temlCol.Attributes.Add("align", "right");
                temlCol.Controls.Add(textCount);
                tempRow.Controls.Add(temlCol);
                answerFreeTextTab.Controls.Add(tempRow);

                tempRow = new HtmlTableRow();
                temlCol = new HtmlTableCell();
                temlCol.Attributes.Add("colspan", "2");
                temlCol.Attributes.Add("valign", "top");
                temlCol.Controls.Add(answerFreeText);
                if ((!string.IsNullOrWhiteSpace(_baseQuestion.Answers[index].FreeTextIndicator.ToString()))
                        && (_baseQuestion.Answers[index].FreeTextIndicator == "M"))
                {
                    HtmlGenericControl answerFreeTextMandatoryIndicator = new HtmlGenericControl();
                    answerFreeTextMandatoryIndicator.InnerText = " *";
                    answerFreeTextMandatoryIndicator.Attributes.Add("style", "color:Red");
                    temlCol.Controls.Add(answerFreeTextMandatoryIndicator);
                }
                tempRow.Controls.Add(temlCol);
                answerFreeTextTab.Controls.Add(tempRow);

                answerFreeTextDiv.Attributes.Add("runat", "server");
                answerFreeTextDiv.Style["display"] = "none";
                answerFreeTextDiv.Controls.Add(answerFreeTextTab);
                answerFreeTextDiv.Attributes.Add("ePAAnswerIDAddlTextHolder", _baseQuestion.Answers[index].ChoiceID);

                text.InnerText = _baseQuestion.Answers[index].ChoiceText;

                option.Attributes.Add("class", "ePACheckbox");
                option.Attributes.Add("name", "group" + _baseQuestion.QuestionID);
                option.Attributes.Add("ePAAnswerChoiceID", _baseQuestion.Answers[index].AnswerChoiceID);
                option.Attributes.Add("ePAParentQuestionID", _baseQuestion.QuestionID);
                option.Attributes.Add("ePACheckAdditionalText", _baseQuestion.Answers[index].FreeTextIndicator.ToString());
                option.Attributes.Add("ePANextQuestionID", _baseQuestion.Answers[index].NextQuestionID);

                option.Checked = string.Compare(_baseQuestion.Answers[index].CompletedAnswer, _baseQuestion.Answers[index].AnswerChoiceID, true) == 0;

                if (option.Checked)
                {
                    if ((!string.IsNullOrWhiteSpace(_baseQuestion.Answers[index].FreeTextIndicator.ToString()))
                        && (_baseQuestion.Answers[index].FreeTextIndicator != "NA"))
                    {
                        answerFreeTextDiv.Style["display"] = "inline";
                        answerFreeTextDiv.InnerText = _baseQuestion.Answers[index].AdditionalFreeTextValue;
                    }
                }
                holder.Attributes["class"] = "ePAOptionBox";
                holder.Controls.Add(option);
                holder.Controls.Add(text);
                holder.Controls.Add(answerFreeTextDiv);
                questionCheckBoxes.Controls.Add(holder);
                questionCheckBoxes.Controls.Add(new HtmlGenericControl("br"));

                option.Attributes.Add("ePAAdditionalTextControlClientID", answerFreeText.ClientID);
                option.Attributes.Add("ePAAdditionalTextDivControlClientID", answerFreeTextDiv.ClientID);
                option.Attributes["onClick"] = "checkOptionAnswers('" + _baseQuestion.QuestionID + "')";
                answerFreeTextDiv.Attributes.Add("ePAAddlTextCountControlClientID", textCount.ClientID);
                answerFreeText.Attributes.Add("onkeyup", "return limitMaxLength('" + answerFreeText.ClientID + "','" + textCount.ClientID + "', event);");
                answerFreeText.Attributes.Add("onkeydown", "return limitMaxLength('" + answerFreeText.ClientID + "','" + textCount.ClientID + "', event);");
                answerFreeText.Attributes.Add("onpaste", "return limitMaxLengthPaste('" + answerFreeText.ClientID + "','" + textCount.ClientID + "', event);");
                answerFreeText.Attributes.Add("onblur", "minimizeTextAreaControl('" + answerFreeText.ClientID + "');");
                answerFreeText.Attributes.Add("onfocus", "maximizeTextAreaControl('" + answerFreeText.ClientID + "');");
            }
        }
    } 
}