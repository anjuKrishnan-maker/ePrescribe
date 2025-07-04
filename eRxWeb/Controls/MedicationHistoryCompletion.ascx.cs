using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI.WebControls;
using Allscripts.Impact;

namespace eRxWeb
{
    /// <summary>
    /// Custom Event Arg class for Medication History Completion (aka - duplicate therapy)
    /// </summary>
    public class MedHistoryCompletionEventArgs : EventArgs
    {
        public bool DidCompleteAll { get; set; }

        public List<string> CompletedScripts { get; set; }

        public MedHistoryCompletionStatus CompletionStatus { get; set; }
        public bool HasOtherDur { get; set; }

        public enum MedHistoryCompletionStatus
        {
            Canceled = 0,
            Continue,
            CompleteAndContinue
        }

        public MedHistoryCompletionEventArgs()
        {
            DidCompleteAll = false;
            CompletedScripts = new List<string>();
        }
    }

    public partial class Controls_MedicationHistoryCompletion : BaseControl
    {
        //
        // event declaration - this will be used to close the user control and rasie event to be consumed by parent page
        //
        public delegate void MedHistoryCompletionHandeler(MedHistoryCompletionEventArgs EventArgs);

        public event MedHistoryCompletionHandeler OnMedHistoryComplete;

        #region Fields

        //private string _siblingFavoritesPage = string.Empty;
        //private string _siblingMedPage = string.Empty;
        private string _currentPage = string.Empty;

        #endregion

        #region Properties

        public DataSet ActiveScripts { get; set; }

        public string SearchValue { get; set; }

        public string CurrentPage
        {
            get { return _currentPage; }
            set
            {
                _currentPage = value;
                //if (_currentPage == Constants.PageNames.SIG)
                //{
                //    _siblingFavoritesPage = Constants.PageNames.SELECT_MEDICATION;
                //    _siblingMedPage = Constants.PageNames.MEDICATION;
                //}
                //else if (_currentPage == Constants.PageNames.NURSE_SIG)
                //{
                //    _siblingFavoritesPage = Constants.PageNames.NURSE_FULL_SCRIPT;
                //    _siblingMedPage = Constants.PageNames.NURSE_MED;
                //}
            }
        }

        public bool IsCSMed
        {
            get
            {
                if (ViewState["IsCSMed"] == null)
                {
                    ViewState["IsCSMed"] = false;
                }

                return (bool) ViewState["IsCSMed"];
            }
            set { ViewState["IsCSMed"] = value; }
        }

        public bool HasOtherDUR
        {
            get
            {
                if (ViewState["HasOtherDUR"] == null)
                {
                    ViewState["HasOtherDUR"] = false;
                }

                return (bool) ViewState["HasOtherDUR"];
            }
            set { ViewState["HasOtherDUR"] = value; }
        }

        #endregion

        #region Page Events and Event Handlers

        protected void Page_Load(object sender, EventArgs e)
        {
            ucMessage.Visible = false;
        }

        public void ShowDurNotAllowed()
        {
            ucMessage.Icon = Controls_Message.MessageType.ERROR;
            ucMessage.MessageText = "You do not have permission to process this renewal request because a Duplicate Therapy DUR alert is present. You can resolve the DUR by completing the following patient active med(s).";
            ucMessage.Visible = true;
            mpeHistory.Show();
        }

        protected void btnContinue_Click(object sender, EventArgs e)
        {
            // If continue button is clicked, redirect to DUR Page. Ignore selected medications in DUR overlay
            mpeHistory.Hide();

            MedHistoryCompletionEventArgs eventArgs = new MedHistoryCompletionEventArgs();
            eventArgs.CompletionStatus = MedHistoryCompletionEventArgs.MedHistoryCompletionStatus.Continue;

            if (OnMedHistoryComplete != null)
            {
                OnMedHistoryComplete(eventArgs);
            }
        }

        protected void btnCompleteAndContinue_Click(object sender, EventArgs e)
        {
            mpeHistory.Hide();

            MedHistoryCompletionEventArgs eventArgs = new MedHistoryCompletionEventArgs();
            eventArgs.CompletionStatus = MedHistoryCompletionEventArgs.MedHistoryCompletionStatus.CompleteAndContinue;

            //complete the meds
            foreach (ListItem li in cblHistory.Items)
            {
                if (li.Selected)
                {
                    Prescription.Complete(li.Value, base.SessionUserID, base.SessionLicenseID, ControlState.GetStringOrEmpty("ExtFacilityCd"),
                        ControlState.GetStringOrEmpty("ExtGroupID"), base.DBID);
                    eventArgs.CompletedScripts.Add(li.Value);
                }
            }

            if (eventArgs.CompletedScripts.Count == 0)
            {
                ucMessage.Icon = Controls_Message.MessageType.ERROR;
                ucMessage.MessageText = "Please select at least one medication from the list.";
                ucMessage.Visible = true;
                mpeHistory.Show();
                return;
            }
            else
            {
                base.RefreshPatientActiveMeds();
            }

            eventArgs.DidCompleteAll = cblHistory.Items.Count == eventArgs.CompletedScripts.Count;
            eventArgs.HasOtherDur = HasOtherDUR;

            if (OnMedHistoryComplete != null)
            {
                OnMedHistoryComplete(eventArgs);
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            mpeHistory.Hide();

            MedHistoryCompletionEventArgs eventArgs = new MedHistoryCompletionEventArgs();
            eventArgs.CompletionStatus = MedHistoryCompletionEventArgs.MedHistoryCompletionStatus.Canceled;

            if (OnMedHistoryComplete != null)
            {
                OnMedHistoryComplete(eventArgs);
            }
        }

        #endregion

        #region Custom Methods

        public void LoadHistory()
        {
            chkSelectAll.Checked = false;

            cblHistory.DataSource = ActiveScripts;
            cblHistory.DataTextField = "Medication";
            cblHistory.DataValueField = "RxID";
            cblHistory.DataBind();

            mpeHistory.Show();
        }

        #endregion
    }
}