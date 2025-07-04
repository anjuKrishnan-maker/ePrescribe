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
    public partial class Controls_ePAInitiationQuestionNumeric : BaseControl
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
            questionHolder.Attributes.Add("ePACurrentQuestionType", "NU");
            questionHolder.Attributes.Add("ePADefaultNextQuestionID", _baseQuestion.DefaultNextQuestionID);
            questionHolder.Attributes.Add("ePANumberAnswerControlClientID", txtNumAnswer.ClientID);

            questionTitle.InnerText = _baseQuestion.QuestionText;
            questionProgress.InnerText = _baseQuestionSet.SequenceOf(_baseQuestion.QuestionID).ToString() + " / " + (_baseQuestionSet.Questions.Count).ToString();

            txtNumAnswer.Attributes.Add("ePAAnswerChoiceID", "numAnswer");
            txtNumAnswer.Attributes.Add("ePAParentQuestionID", _baseQuestion.QuestionID);
            txtNumAnswer.Attributes.Add("ePADefaultNextQuestionID", _baseQuestion.DefaultNextQuestionID);
            txtNumAnswer.Attributes.Add("onkeyup", "checkNumberValue('" + txtNumAnswer.ClientID + "','" + _baseQuestion.QuestionID + "')");
            txtNumAnswer.Attributes.Add("onpaste", "return validateDecimalValuePaste(this, event);");
            txtNumAnswer.Attributes.Add("onkeypress", "return limitKeyForDecimalValue(this, event);");

            if (_baseQuestion.Completed)
            {
                txtNumAnswer.Value = _baseQuestion.NumericAnswer.ToString();
            }

            for (int i = 0; i < _baseQuestion.Answers.Count; i++)
            {
                HtmlInputHidden logicChecker = new HtmlInputHidden();

                logicChecker.Attributes.Add("ePANumberCheckQuestionID", _baseQuestion.QuestionID);
                logicChecker.Attributes.Add("ePANumberCheckOneComp", _baseQuestion.Answers[i].NumericCheckOneComp.ToString());
                logicChecker.Attributes.Add("ePANumberCheckTwoComp", _baseQuestion.Answers[i].NumericCheckTwoComp.ToString());
                logicChecker.Attributes.Add("ePANumberCheckOneVal", _baseQuestion.Answers[i].NumericCheckOneValue.ToString());
                logicChecker.Attributes.Add("ePANumberCheckTwoVal", _baseQuestion.Answers[i].NumericCheckTwoValue.ToString());
                logicChecker.Attributes.Add("ePANumberCheckLogic", _baseQuestion.Answers[i].ComparisonType.ToString());
                logicChecker.Attributes.Add("ePANextQuestionID", _baseQuestion.Answers[i].NextQuestionID);

                questionMiddle.Controls.Add(logicChecker);
            }
        }
    } 
}