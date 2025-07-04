using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.ExtensionMethods;
using Allscripts.ePrescribe.Objects;
using Telerik.Web.UI;

namespace eRxWeb
{
    public partial class PatientAmendments : BasePage
    {
        protected const string RdbAcceptedText = "Accepted";

        protected void Page_LoadComplete(object sender, EventArgs e)
        {
            ((PhysicianMasterPageBlank)Master)?.hideTabs();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                txtDescription.Attributes.Add("onkeyup", $"SetCharsRemaining(this,{charsRemaining.ClientID},1000);");
                lblPatientName.Text = PageState.GetStringOrEmpty(Constants.SessionVariables.PatientName);
                lblTodaysDate.Text = DateTime.Now.ToShortDateString();
            }
        }

        protected void btnBack_OnClick(object sender, EventArgs e)
        {
            Response.Redirect($"{Constants.PageNames.ADD_PATIENT}?Mode=Edit");
        }

        protected void grdAmendments_OnNeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            grdAmendments.DataSource = Allscripts.ePrescribe.Data.Patient.GetPatientAmendments(SessionPatientID.ToGuidOr0x0(), DBID);
        }

        protected void btnSave_OnClick(object sender, EventArgs e)
        {
            if (IsValidData())
            {
                Allscripts.ePrescribe.Data.Patient.InsertPatientAmendment(SessionPatientID.ToGuidOr0x0(), txtDescription.Text.Trim(), rdbResolution.SelectedItem.Text == RdbAcceptedText, SessionUserID.ToGuidOr0x0(), DBID);
                rdbResolution.SelectedIndex = -1;
                txtDescription.Text = "";
                grdAmendments.Rebind();
            }
        }

        private bool IsValidData()
        {
            var isValid = true;
            if(string.IsNullOrWhiteSpace(txtDescription.Text) || Regex.IsMatch(txtDescription.Text, RegularExpressions.GeneralTextNoLimit))
            {
                ucMessage.MessageText = "Please enter a valid description.";
                isValid = false;
            }
            
            if(rdbResolution.SelectedIndex < 0)
            {
                isValid = false;
                ucMessage.MessageText = "Please select a resolution reason.";
            }

            if(!isValid)
            {
                ucMessage.Visible = true;
                mpeAddAmendment.Show();
            }
            else
            {
                ucMessage.Visible = false;
            }

            return isValid;
        }
    }
}