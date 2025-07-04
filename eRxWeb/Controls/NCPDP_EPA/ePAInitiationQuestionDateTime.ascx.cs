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
    public partial class Controls_ePAInitiationQuestionDateTime : BaseControl
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
            questionHolder.Attributes.Add("ePACurrentQuestionType", "DT");
            questionHolder.Attributes.Add("ePADefaultNextQuestionID", _baseQuestion.DefaultNextQuestionID);
            questionHolder.Attributes.Add("ePACheckDateTime", _baseQuestion.DateTimeRequired.ToString());
            dateAnswer.Attributes.Add("onkeyup", "OnDateValueChanged('" + dateAnswer.ClientID + "')");
            dateAnswer.Attributes.Add("onpaste", "OnDateValueChanged('" + dateAnswer.ClientID + "')");
            dateTimeAnswer.Attributes.Add("onkeyup", "OnDateValueChanged('" + dateTimeAnswer.ClientID + "')");
            dateTimeAnswer.Attributes.Add("onpaste", "OnDateValueChanged('" + dateTimeAnswer.ClientID + "')");

            if (_baseQuestion.DateTimeRequired == 'Y')
            {
                dateAnswer.Visible = false;
                dateTimeAnswer.Visible = true;
                questionHolder.Attributes.Add("ePADateTimeAnswerControlClientID", dateTimeAnswer.ClientID);
            }
            else
            {
                dateAnswer.Visible = true;
                dateTimeAnswer.Visible = false;
                questionHolder.Attributes.Add("ePADateTimeAnswerControlClientID", dateAnswer.ClientID);
            }
            
            questionHolder.Attributes.Add("ePAAnswerChoiceID", "dateAnswer");
            questionHolder.Attributes.Add("ePAParentQuestionID", _baseQuestion.QuestionID);
            
            questionTitle.InnerText = _baseQuestion.QuestionText;
            questionProgress.InnerText = _baseQuestionSet.SequenceOf(_baseQuestion.QuestionID).ToString() + " / " + (_baseQuestionSet.Questions.Count).ToString();

            if (_baseQuestion.Completed)
            {
                try
                {
                    if (_baseQuestion.DateTimeRequired == 'Y')
                    {
                        dateTimeAnswer.SelectedDate = Convert.ToDateTime(_baseQuestion.DateTimeAnswer);
                    }
                    else
                    {
                        dateAnswer.SelectedDate = Convert.ToDateTime(_baseQuestion.DateTimeAnswer);
                    }
                }
                catch (Exception)
                {

                }
            }
        
        }
    } 
}