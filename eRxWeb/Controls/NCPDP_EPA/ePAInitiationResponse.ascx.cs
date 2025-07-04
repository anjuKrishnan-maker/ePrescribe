using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Allscripts.Impact;
using Allscripts.ePrescribe.Common;

namespace eRxWeb
{
    public partial class Controls_ePAInitiationResponse : BaseControl
    {
        private EPAInitQuestionSet _baseInitQuestionSet
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
            set
            {
                Session["ePACurrentInitiationQuestionSet"] = value;
            }
        }

        public event EPAInitiationQuestionSetEventHandler EPAUIEvent;

        protected virtual void OnReviewAction(EPAInitiationQuestionSetEventArgs e)
        {
            if (this.EPAUIEvent != null)
                this.EPAUIEvent(this, e);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (_baseInitQuestionSet.Questions.Count > 0)
            {
                this.LoadQS(_baseInitQuestionSet);
            }
        }

        protected void ProcessEPAEvent_IQL(object sender, EPAInitiationQuestionSetEventArgs e)
        {
            switch (e.EventType)
            {
                case Constants.EPAInitiationQuestionSetUIEvents.Cancel:
                    OnReviewAction(new EPAInitiationQuestionSetEventArgs(Constants.EPAInitiationQuestionSetUIEvents.Cancel));
                    break;
                case Constants.EPAInitiationQuestionSetUIEvents.Review:
                    showReview();
                    OnReviewAction(new EPAInitiationQuestionSetEventArgs(Constants.EPAInitiationQuestionSetUIEvents.Review));
                    break;
                case Constants.EPAInitiationQuestionSetUIEvents.SaveForLater:
                    showQuestions();
                    OnReviewAction(new EPAInitiationQuestionSetEventArgs(Constants.EPAInitiationQuestionSetUIEvents.SaveForLater));
                    break;
                case Constants.EPAInitiationQuestionSetUIEvents.StartOver:
                    OnReviewAction(new EPAInitiationQuestionSetEventArgs(Constants.EPAInitiationQuestionSetUIEvents.StartOver));
                    break;
            }
        }

        public void LoadQS(EPAInitQuestionSet newQuestionSet)
        {
            _baseInitQuestionSet = newQuestionSet;
            ePAQuestionResponseTitle.InnerText = _baseInitQuestionSet.QuestionSetTitle;
            ePAQuestionResponseDescription.InnerText = _baseInitQuestionSet.QuestionSetDescription;
            ePAExpirationDate.InnerText = _baseInitQuestionSet.ExpirationDate;
            ePAContactNumber.InnerText = _baseInitQuestionSet.ContactPhoneNumber;
            deniedPhoneNumber.InnerText = _baseInitQuestionSet.ContactPhoneNumber;
            failedPhoneNumber.InnerText = _baseInitQuestionSet.ContactPhoneNumber;
            closedPhoneNumber.InnerText = _baseInitQuestionSet.ContactPhoneNumber;
            ePAPatientName.InnerText = _baseInitQuestionSet.PatientName;
        }

        public void StartOver()
        {
            _baseInitQuestionSet.ClearAnswers();
            _baseInitQuestionSet.ClearAttachments(base.SessionUserID);
            ePA_Message.Style["display"] = "none";
            ePAInitiationQuestionList.Show();
            ePAInitiationQuestionAnswerReview.Hide();
            mpeNCPDPEPA.Show();
        }

        public void Cancel()
        {
            Hide();
        }

        public void Submit()
        {
            // Save EPARequest, Save and send EPADecisionRequestMsg and update the task to EPA_Submitted.
            string ePARequestID = EPSBroker.EPA.SendToEPARequests(_baseInitQuestionSet.TaskID, Constants.ePATransPriority.URGENT,
                                  Constants.ePARequestType.EPA_REQUEST, base.SessionUserID, base.DBID);

            EPSBroker.EPA.UpdateTaskStatus(_baseInitQuestionSet.TaskID, Constants.ePATaskStatus.EPA_SUBMITTED, base.DBID);

            EPSBroker.EPA.SendPAReqquest(base.SessionLicenseID, base.SessionUserID, _baseInitQuestionSet.TaskID, ePARequestID, 
                                          base.NcpdpEpaUserShieldSecurityToken, 
                                          base.DBID);
            
            Hide();
        }

        public void Show(bool alreadyUp)
        {
            if (!alreadyUp)
            {
                switch (_baseInitQuestionSet.TaskStatus)
                {
                    case Constants.ePATaskStatus.EPA_SAVED_NOT_SUBMITTED:
                    case Constants.ePATaskStatus.EPA_READY:
                        ePA_Message.Style["display"] = "none";
                        if (_baseInitQuestionSet.Completed)
                        {
                            showReview();
                        }
                        else
                        {
                            showQuestions();
                        }
                        break;
                    default:
                        toggleMessage(_baseInitQuestionSet.TaskStatus);
                        break;
                }
            }
            mpeNCPDPEPA.Show();
        }

        public void Hide()
        {
            ePAInitiationQuestionList.Hide();
            ePAInitiationQuestionAnswerReview.Hide();
            mpeNCPDPEPA.Hide();
            _baseInitQuestionSet = null;
        }

        private void showReview()
        {
            ePAInitiationQuestionList.Hide();
            ePAInitiationQuestionAnswerReview.Show();
        }

        private void showQuestions()
        {
            ePAInitiationQuestionList.Show();
            ePAInitiationQuestionAnswerReview.Hide();
        }

        private void toggleMessage(Constants.ePATaskStatus taskStatus)
        {
            ePAInitiationQuestionList.Hide();
            ePAInitiationQuestionAnswerReview.Hide();
            ePA_Message.Style["display"] = "inline";
            submittedMessage.Style["display"] = "none";
            deniedMessage.Style["display"] = "none";
            failedMessage.Style["display"] = "none";
            closedCancelledMessage.Style["display"] = "none";
            switch (taskStatus)
            {
                case Constants.ePATaskStatus.EPA_SUBMITTED:
                    submittedMessage.Style["display"] = "inline";
                    break;
                case Constants.ePATaskStatus.EPA_DENIED:
                    deniedMessage.Style["display"] = "inline";
                    break;
                case Constants.ePATaskStatus.EPA_FAILED:
                    failedMessage.Style["display"] = "inline";
                    break;
                case Constants.ePATaskStatus.EPA_CLOSED:
                    closedCancelledMessage.Style["display"] = "inline";
                    break;
            }
        }

        protected void btnClose_Click(object sender, EventArgs e)
        {
            OnReviewAction(new EPAInitiationQuestionSetEventArgs(Constants.EPAInitiationQuestionSetUIEvents.Cancel));
        }
    } 
}