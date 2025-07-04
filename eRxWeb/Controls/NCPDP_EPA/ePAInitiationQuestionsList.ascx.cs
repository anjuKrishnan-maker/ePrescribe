using Allscripts.ePrescribe.Common;
using Allscripts.Impact;
using eRxWeb.AppCode;
using System;
using System.Web.Script.Serialization;
using System.Web.UI;

namespace eRxWeb
{
    public partial class Controls_ePAInitiationQuestionsList : BaseControl
    {
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

        public EPAInitiationQuestionSetEventArgs eventArgs;

        public event EPAInitiationQuestionSetEventHandler EPAUIEvent;

        protected virtual void OnReviewAction(EPAInitiationQuestionSetEventArgs e)
        {
            if (this.EPAUIEvent != null)
                this.EPAUIEvent(this, e);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            loadQuestions();
        }

        protected void btnReview_Click(object sender, EventArgs e)
        {
            saveAnswers();
            OnReviewAction(new EPAInitiationQuestionSetEventArgs(Constants.EPAInitiationQuestionSetUIEvents.Review));
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            saveAnswers();
            OnReviewAction(new EPAInitiationQuestionSetEventArgs(Constants.EPAInitiationQuestionSetUIEvents.Cancel));
        }

        protected void btnStartOver_Click(object sender, EventArgs e)
        {
            _baseQuestionSet.ClearAnswers();
            OnReviewAction(new EPAInitiationQuestionSetEventArgs(Constants.EPAInitiationQuestionSetUIEvents.StartOver));
            btnStartOverDisp.Value = "Start Over";
            btnStartOverDisp.Style["cursor"] = "pointer";
            btnStartOverDisp.Disabled = true;
        }

        protected void btnSaveFinishLater_Click(object sender, EventArgs e)
        {
            saveAnswers();
            OnReviewAction(new EPAInitiationQuestionSetEventArgs(Constants.EPAInitiationQuestionSetUIEvents.SaveForLater));
        }

        public void Hide()
        {
            questionListHolder.Style["display"] = "none";
        }

        public void Show()
        {
            loadQuestions();
            questionListHolder.Style["display"] = "inline";
        }

        private void loadQuestions()
        {
            questionList.Controls.Clear();
            string controlPath = string.Empty;
            string defaultNextID = string.Empty;
            string startingQID = _baseQuestionSet.StartingQuestionID;

            btnSaveFinishLater.Attributes.Remove("ePAQuestionAnswered");
            btnSaveFinishLater.Attributes.Add("ePAQuestionAnswered", "NO");

            if (string.Compare("END", startingQID, true) == 0)
            {
                OnReviewAction(new EPAInitiationQuestionSetEventArgs(Constants.EPAInitiationQuestionSetUIEvents.Review));
            }
            else
            {
                int startingQIndex = -1;
                int i = 0;
                for (i = 0; i < _baseQuestionSet.Questions.Count; i++)
                {
                    if (string.Compare(_baseQuestionSet.Questions[i].QuestionID, startingQID, true) == 0)
                    {
                        startingQIndex = i;
                        defaultNextID = _baseQuestionSet.Questions[i].DefaultNextQuestionID;
                    }

                    controlPath = string.Empty;
                    switch (_baseQuestionSet.Questions[i].QuestionType)
                    {
                        case Allscripts.ePrescribe.Common.Constants.ePAQuestionType.CA:
                            controlPath = "Controls/NCPDP_EPA/ePAInitiationQuestionChooseAny.ascx";
                            break;

                        case Allscripts.ePrescribe.Common.Constants.ePAQuestionType.CO:
                            controlPath = "Controls/NCPDP_EPA/ePAInitiationQuestionChooseOne.ascx";
                            break;

                        case Allscripts.ePrescribe.Common.Constants.ePAQuestionType.DT:
                            controlPath = "Controls/NCPDP_EPA/ePAInitiationQuestionDateTime.ascx";
                            break;

                        case Allscripts.ePrescribe.Common.Constants.ePAQuestionType.FT:
                            controlPath = "Controls/NCPDP_EPA/ePAInitiationQuestionFreeText.ascx";
                            break;

                        case Allscripts.ePrescribe.Common.Constants.ePAQuestionType.NU:
                            controlPath = "Controls/NCPDP_EPA/ePAInitiationQuestionNumeric.ascx";
                            break;
                    }

                    if (!string.IsNullOrEmpty(controlPath))
                    {
                        UserControl uc = (UserControl)Page.LoadControl(controlPath);
                        uc.ID = "ePAControl" + _baseQuestionSet.Questions[i].QuestionID;
                        uc.Attributes["CurrentQuestionID"] = _baseQuestionSet.Questions[i].QuestionID;
                        questionList.Controls.Add(uc);
                    }
                }

                if (startingQIndex == 0)
                {
                    btnStartOverDisp.Disabled = true;
                }
                else
                {
                    btnStartOverDisp.Disabled = false;
                }

                if (startingQIndex < _baseQuestionSet.Questions.Count && startingQIndex >= 0)
                {
                    btnNext.Attributes.Add("ePACurrentQuestionID", _baseQuestionSet.Questions[startingQIndex].QuestionID);
                    btnNext.Attributes.Add("ePACurrentQuestionType", _baseQuestionSet.Questions[startingQIndex].QuestionType.ToString());
                }

                btnNext.Disabled = true;
                btnSaveFinishLater.Disabled = true;

                if (!string.IsNullOrEmpty(defaultNextID))
                {
                    btnNext.Attributes.Add("ePANextQuestionID", _baseQuestionSet.Questions[startingQIndex].DefaultNextQuestionID);
                }
            }
        }

        private void saveAnswers()
        {
            string answerChoiceID = string.Empty;
            string questionID = string.Empty;
            string value = string.Empty;
            string addText = string.Empty;
            string jsonString = hiddenFormAnswerList.Value;

            JavaScriptSerializer js = new JavaScriptSerializer();
            ePAQuestionListModel questionAnswer = js.Deserialize<ePAQuestionListModel>(jsonString);

            foreach (ePAQuestionModel answer in questionAnswer.Answers)
            {
                answerChoiceID = string.Empty;
                questionID = string.Empty;
                value = string.Empty;
                addText = string.Empty;

                if (!string.IsNullOrEmpty(answer.ParentQuestionID))
                    questionID = answer.ParentQuestionID;

                if (!string.IsNullOrEmpty(answer.AnswerChoiceID))
                    answerChoiceID = answer.AnswerChoiceID;

                if (!string.IsNullOrEmpty(answer.Value))
                    value = answer.Value;

                if (!string.IsNullOrWhiteSpace(answer.AddText))
                    addText = answer.AddText;

                for (int i = 0; i < _baseQuestionSet.Questions.Count; i++)
                {
                    if (string.Compare(_baseQuestionSet.Questions[i].QuestionID, questionID, true) == 0)
                    {
                        _baseQuestionSet.Questions[i].AddAnswer(answerChoiceID, value, addText);
                        break;
                    }
                }
            }

            _baseQuestionSet.SaveQuestionSet(base.SessionUserID, base.SessionSiteID);
        }
    }
}