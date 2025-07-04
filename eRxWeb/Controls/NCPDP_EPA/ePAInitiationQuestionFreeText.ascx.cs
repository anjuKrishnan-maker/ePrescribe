using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Allscripts.Impact;

namespace eRxWeb
{
    public partial class Controls_ePAInitiationQuestionFreeText : BaseControl
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
            questionHolder.Attributes.Add("ePACurrentQuestionType", "FT");
            questionHolder.Attributes.Add("ePADefaultNextQuestionID", _baseQuestion.DefaultNextQuestionID);
            questionHolder.Attributes.Add("ePAFreeTextAnswerControlClientID", questionFreeText.ClientID);

            questionTitle.InnerText = _baseQuestion.QuestionText;
            questionProgress.InnerText = _baseQuestionSet.SequenceOf(_baseQuestion.QuestionID).ToString() + " / " + (_baseQuestionSet.Questions.Count).ToString();

            questionFreeText.InnerText = _baseQuestion.FreeTextAnswer;
            questionFreeText.Attributes.Add("ePAAnswerChoiceID", "freeTextAnswer");
            questionFreeText.Attributes.Add("ePAParentQuestionID", _baseQuestion.QuestionID);
            questionFreeText.Attributes.Add("onkeyup", "return limitFreeTextQSMaxLength('" + questionFreeText.ClientID + "','" + textCount.ClientID + "', event);");
            questionFreeText.Attributes.Add("onkeydown", "return limitFreeTextQSMaxLength('" + questionFreeText.ClientID + "','" + textCount.ClientID + "', event);");
            questionFreeText.Attributes.Add("onpaste", "return limitFreeTextMaxLengthPaste('" + questionFreeText.ClientID + "','" + textCount.ClientID + "', event);");
        }
    } 
}