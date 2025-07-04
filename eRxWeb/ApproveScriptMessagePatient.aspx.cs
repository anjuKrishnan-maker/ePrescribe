/******************************************************************************
**Change History
*******************************************************************************
**Date:      Author:                    Description:
**-----------------------------------------------------------------------------
* 06/17/2009   Dharani Reddem           Added PRESCRIPTION_TO_PROVIDER to navigate the correct pages
* 11/09/2011   Narendra Meena           Removed SSN in edit and Add patient Process
*******************************************************************************/
using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Text;
using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.Objects;
using Allscripts.Impact;
using Allscripts.Impact.Interfaces;
using Patient = Allscripts.Impact.Patient;
using PatientCoverage = Allscripts.Impact.PatientCoverage;
using Provider = Allscripts.Impact.Provider;
using Rx = Allscripts.Impact.Rx;
using RxUser = Allscripts.Impact.RxUser;
using ScriptMessage = Allscripts.Impact.ScriptMsg.ScriptMessage;

namespace eRxWeb
{
    public partial class ApproveScriptMessagePatient : BasePage 
    {
        protected ScriptMessage sm;
        private bool isChangePatientRequest = false;
        private bool isPatientAutoSelect = false;
        private IPrescription iPrescription = new Prescription();

        protected void Page_Load(object sender, EventArgs e)
        {
            isChangePatientRequest = (Request.QueryString["Action"] == "ChangePatient");
            isPatientAutoSelect = (Session["AutoSelectPatient"] != null) ? Convert.ToBoolean(Session["AutoSelectPatient"]) : true;
            cvDOB.ValueToCompare = DateTime.Now.ToShortDateString();

            btnUpdate.Click += new EventHandler(btnUpdate_Click);
            Form.DefaultButton = btnSearch.UniqueID;
            int tasks = 0;

            ((PhysicianMasterPageBlank)Master).toggleTabs("tasks", tasks);

            Session[Constants.SessionVariables.TaskScriptMessageId] = Request.QueryString["smid"].ToString();
            sm = new ScriptMessage(Session[Constants.SessionVariables.TaskScriptMessageId].ToString(), Session["LICENSEID"].ToString(), Session["USERID"].ToString(), base.DBID);

            btnAdd.Visible = bool.Parse(Session["AddPatient"].ToString());
            PatObjDataSource.SelectParameters["HasVIPPatients"].DefaultValue = base.SessionLicense.hasVIPPatients.ToString();

            if (!Page.IsPostBack)
            {
                GetStateList();
                loadTask();
                SetControlsForWorkflow();
                if (Session["RestrictedUserOverridden"] != null)
                {
                    GetPatientAllergynMedication();
                    pnlAllergyMedication.Visible = true;
                    // call search method...
                    PatientSearch(Session["selectedPatient"].ToString());
                }
                else
                {
                    Session.Remove("selectedPatient");
                }
            }
        }

        private void SetControlsForWorkflow()
        {
            if (sm.MessageType.Trim() == Constants.PrescriptionTaskType.RXCHG.ToString())
            {
                btnAdd.Visible = false;
            }
            else
            {
                btnAdd.Visible = true;
            }
        }

        void btnUpdate_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(selectedPat.Value.ToString()) && Session["RestrictedUserOverridden"] == null)
            {
                bool isUserRestricted = checkIfUserRestricted(selectedPat.Value.ToString(), base.SessionUserID, base.DBID);
                Session["IsRestrictedUser"] = isUserRestricted ? true : false;
                Session["PrivacyPatientID"] = selectedPat.Value.ToString();
                if (!isUserRestricted)
                {
                    GetPatientAllergynMedication(selectedPat.Value.ToString());
                    pnlAllergyMedication.Visible = true;
                    upData.Update();
                }
                else
                {
                    Session["selectedPatient"] = selectedPat.Value.ToString();
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "", $"ShowPrivacyOverriddenOverlay('{selectedPat.Value.ToString()}')", true);
                    Session["TaskScriptMessageId"] = Session[Constants.SessionVariables.TaskScriptMessageId].ToString();
                    GetPatientAllergynMedication(selectedPat.Value.ToString());
                    pnlAllergyMedication.Visible = true;
                    upData.Update();
                }
            }
            else
            {
                Session.Remove("RestrictedUserOverridden");
                Session.Remove("selectedPatient");
            }
        }

        protected void GetStateList()
        {
            DataTable dtLisState = RxUser.ChGetState(base.DBID);
            ddlState.DataSource = dtLisState;
            ddlState.DataTextField = "State";
            ddlState.DataValueField = "State";
            ddlState.DataBind();
        }

        protected void PatObjDataSource_Selected(object sender, ObjectDataSourceStatusEventArgs e)
        {
            DataSet ds = (DataSet)e.ReturnValue;
            if (ds.Tables[0].Rows.Count > 50)
            {
                ucSearchMessage.MessageText = "Your search returned more than 50 results. Please consider refining your search.";
                ucSearchMessage.Icon = Controls_Message.MessageType.INFORMATION;
                ucSearchMessage.Visible = true;
            }
            else
            {
                ucSearchMessage.Visible = false;
            }
            ds.Dispose();
        }

        void PatDataDataSource_Selected(object sender, ObjectDataSourceStatusEventArgs e)
        {
            if (e.ReturnValue != null)
            {
                DataSet patds = ((DataSet)e.ReturnValue);
                setPatientMedAndAllergyInfo(patds);
            }
        }

        private void setPatientMedAndAllergyInfo(DataSet patds)
        {
            if (patds.Tables["PatientAllergy"].Rows.Count > 0)
            {
                DataRow[] PatAllergy = patds.Tables["PatientAllergy"].Select("Active='Y'");
                string allergy = string.Empty;
                foreach (DataRow dr in PatAllergy)
                {
                    allergy = allergy + dr["AllergyName"].ToString() + ",";
                }
                if (allergy.EndsWith(","))
                    allergy = allergy.Substring(0, allergy.Length - 1);

                if (allergy.Length > 0)
                {
                    lblCurrAllergies.Text = "<b>Active allergies:</b> " + allergy + "<BR>";
                }
                else
                {
                    lblCurrAllergies.Text = "<b>Active allergies:</b> NKA<br>";
                }
            }
            else
                lblCurrAllergies.Text = "<b>Active allergies:</b> NKA<br>";

            //Retrieve the patient's distinct active medications 
            DataSet activeMeds = Patient.GetPatientActiveMedications(patds.Tables[0].Rows[0]["PatientID"].ToString(), Session["LICENSEID"].ToString(), Session["USERID"].ToString(), base.DBID);
            if (activeMeds.Tables["Medications"].Rows.Count > 0)
            {
                StringBuilder activeMedications = new StringBuilder();
                foreach (DataRow dr in activeMeds.Tables["Medications"].Rows)
                {
                    if (activeMedications.Length > 0)
                        activeMedications.Append(", ");
                    activeMedications.Append(dr["MedicationName"].ToString().Trim());
                }
                lblCurrMeds.Text = "<b>Active Medications:</b> " + activeMedications.ToString();
            }
            else
                lblCurrMeds.Text = "<b>Active Medications:</b> No known";
        }

        /// <summary>
        /// In addition to setting control properties in case the user wants to create a new patient, 
        /// this method also displays some of the original patient info in the "Patient info in request"
        /// panel.
        /// </summary>
        private void SetInitialCreatePatientProperties(
            string patientFirstName,
            string patientLastName,
            string patientGender,
            string patientDOB,
            string patientAddress1,
            string patientCity,
            string patientState,
            string patientZip,
            string patientPhoneNumber,
            string pharmacyDetail,
            bool showReferralInfoHeader,
            string referralSenderName,
            string referralSenderPractice)
        {
            bool isPaternalMaternalState = (Session["PRACTICESTATE"] != null && Session["PRACTICESTATE"].ToString() == "PR");

            pnlPaternalMaternal.Visible = isPaternalMaternalState;
            rfvPaternalName.Enabled = isPaternalMaternalState;
            revPaternalName.Enabled = isPaternalMaternalState;
            revMaternalName.Enabled = isPaternalMaternalState;
            txtLName.Enabled = !isPaternalMaternalState;
            rfvLastName.Enabled = !isPaternalMaternalState;

            lblAddress.Text = BuildAddressDisplay(patientAddress1, String.Empty, patientCity, patientState, patientZip, true);
            lblDOB.Text = patientDOB;
            lblPatientName.Text = $"{ patientFirstName.ToHTMLEncode()} {patientLastName.ToHTMLEncode()}";

            // Go ahead and set these properties even though their controls may be hidden (based on showReferralInfoHeader)
            //lblPharmacyName.Text = pharmacyName;

            ucMessageHeader.Icon = Controls_Message.MessageType.INFORMATION;
            ucMessageHeader.MessageText = $"Request Pharmacy :  {pharmacyDetail}";
            ucMessageHeader.Visible = !showReferralInfoHeader;

            //lblPharmacyAddress.Text = pharmacyAddress;
            //lblPharmacyPhone.Text = Allscripts.Impact.Utilities.StringHelper.FormatPhone(pharmacyPhoneNumber);

            lblReferralSenderName.Text = referralSenderName;
            lblReferralSenderPractice.Text = referralSenderPractice;

            // Show/hide either pharmacy header or referral header
            //tblPharmacyHeaderInfo.Visible = !showReferralInfoHeader;
            tblReferralHeaderInfo.Visible = showReferralInfoHeader;
            tblPharmacyHeaderInfo.Visible = false;

            txtLastNameSearch.Text = patientLastName;
            txtFirstNameSearch.Text = patientFirstName;
            btnSearch_Click(this, new EventArgs());

            //preload the add patient data if we want to
            txtAddress1.Text = patientAddress1;
            txtCity.Text = patientCity;
            txtDOB.Text = patientDOB;
            txtFName.Text = patientFirstName;

            if (isPaternalMaternalState)
                txtPaternalName.Text = patientLastName;
            else
                txtLName.Text = patientLastName;

            txtPhone.Text = patientPhoneNumber;
            ddlState.SelectedValue = patientState;
            if (patientZip.Length > 5)
            {
                txtZip.Text = patientZip.Substring(0, 5); //whack any +4
            }
            else
            {
                txtZip.Text = patientZip;
            }
            DDLGender.SelectedValue = patientGender;
        }


        private void loadTask()
        {
            string pharmacyDetail = Allscripts.Impact.Utilities.StringHelper.GetPharmacyName(
                    sm.PharmacyName,
                    sm.PharmacyAddress1,
                    sm.PharmacyAddress2,
                    sm.PharmacyCity,
                    sm.PharmacyState,
                    sm.PharmacyZip,
                    sm.PharmacyPhoneNumber);

            // Initialize data based on ScriptMessage object
            SetInitialCreatePatientProperties(
                sm.PatientFirstName,
                sm.PatientLastName,
                sm.PatientGender,
                sm.PatientDOB,
                sm.PatientAddress1,
                sm.PatientCity,
                sm.PatientState,
                sm.PatientZip,
                sm.PatientPhoneNumber,
                pharmacyDetail,
                false,
                String.Empty,
                String.Empty);
        }

        //Erik code integration on 3 Oct 06
        private Control FindControlInContainer(Control container, string controlId)
        {
            if (container.FindControl(controlId) != null)
                return container.FindControl(controlId);
            foreach (Control c in container.Controls)
            {
                Control controlFound = FindControlInContainer(c, controlId);
                if (controlFound != null)
                    return controlFound;
            }
            return null;
        }

        private void setBannerInfo(string patientID)
        {
            ((PhysicianMasterPageBlank)Master).SetPatientInfo(patientID);
        }

        private void resetSession()
        {
            ClearMedicationInfo(true);

            Session.Remove("RXID");
            Session.Remove(Constants.SessionVariables.TaskScriptMessageId);
        }

        protected void btnChangePatient_Click(object sender, EventArgs e)
        {
            resetSession();
            string returnUrl;

            if (Request.QueryString["From"].ToLower().Contains("approvescriptmessage.aspx"))
                returnUrl = Constants.PageNames.DOC_REFILL_MENU;
            else
                returnUrl = Request.QueryString["From"];

            Server.Transfer(returnUrl);
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            grdViewPatients.DataSourceID = "PatObjDataSource";
            lblCurrAllergies.Text = string.Empty;
            lblCurrMeds.Text = string.Empty;
            grdViewPatients.DataBind();
        }


        protected void grdViewPatients_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    e.Row.Style["cursor"] = "pointer";
                    e.Row.Attributes.Add("onclick", "onRowClickPat(this)");
                }

                if (Session["selectedPatient"] != null && grdViewPatients.DataKeys[e.Row.RowIndex][0].ToString() == Session["selectedPatient"].ToString() && Session["RestrictedUserOverridden"] != null)
                {
                    grdViewPatients.SelectedIndex = e.Row.RowIndex;
                    ClientScript.RegisterStartupScript(this.GetType(), "setSelectedPat", "setSelectedPat('" + grdViewPatients.SelectedDataKey.Value.ToString() + "');", true);
                }
                string patientID = string.Empty;
                if (sm != null)
                {
                    patientID = sm.DBPatientID;
                }

                if (grdViewPatients.DataKeys[e.Row.RowIndex][0].ToString() == patientID && !isChangePatientRequest && isPatientAutoSelect)
                {
                    grdViewPatients.SelectedIndex = e.Row.RowIndex;
                    ClientScript.RegisterStartupScript(this.GetType(), "setSelectedPat", "setSelectedPat('" + grdViewPatients.SelectedDataKey.Value.ToString() + "');", true);
                }
            }
        }

        private string removeLastWord(string word)
        {
            if (word.IndexOf(" ") > -1)
            {
                return word.Substring(0, word.LastIndexOf(" "));
            }
            else
            {
                return null;
            }
        }

        protected void btnSaveAdd_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                if (CheckPatientDemographics())
                {
                    btnAdd.Enabled = false;
                    ucMessage.Icon = Controls_Message.MessageType.ERROR;
                    ucMessage.MessageText = "An identical patient already exists in the database.  Please select that patient.";
                    ucMessage.Visible = true;
                }
                else
                {
                    addNewPatient();
                }
            }
            else
            {
                mpePatient.Show();
            }

            if (Session["PRACTICESTATE"] != null && Session["PRACTICESTATE"].ToString() == "PR")
                txtLastNameSearch.Text = txtPaternalName.Text.Trim() + " " + txtMaternalName.Text.Trim();
            else
                txtLastNameSearch.Text = txtLName.Text;

            txtFirstNameSearch.Text = txtFName.Text;
            btnAdd.Enabled = false;
            btnSearch_Click(this, new EventArgs());
        }

        private bool CheckPatientDemographics()
        {
            bool bPatientFound = false;

            DataSet patientDataSet = GetSimilarPatients();
            if (patientDataSet.Tables["Patients"].Rows.Count > 0)
            {
                bPatientFound = true;
            }
            return bPatientFound;
        }

        private DataSet GetSimilarPatients()
        {
            string licenseID = Session["LICENSEID"].ToString();

            string lastName = txtLName.Text;
            string firstName = txtFName.Text;
            string address = txtAddress1.Text;
            string zip = txtZip.Text;
            DateTime dob = Convert.ToDateTime(txtDOB.Text);

            int matchThreshold = 149; //Highest score is 240 
            DataSet patientDataSet = Allscripts.Impact.CHPatient.SearchSimilarPatients(licenseID, firstName, lastName, address, dob, zip, matchThreshold, Session["USERID"].ToString(), false, base.DBID);

            return patientDataSet;
        }

        private void addNewPatient()
        {
            string licenseID = Session["LICENSEID"].ToString();
            string userId = Session["USERID"].ToString();
            string patientID = System.Guid.NewGuid().ToString();
            string chartID = txtMRN.Text;
            string lastName = txtLName.Text;
            string firstName = txtFName.Text;
            string middleName = txtMName.Text;
            string address1 = txtAddress1.Text;
            string address2 = txtAddress2.Text;
            string city = txtCity.Text;
            string state = ddlState.SelectedValue;
            string zip = txtZip.Text;
            string phone = txtPhone.Text;
            string mobilePhone = txtMobilePhone.Text;
            string dob = txtDOB.Text;
            string account = string.Empty;
            string sex = DDLGender.SelectedValue;
            int active = 1;
            if (Session["PATIENTSTATUS"] != null && Session["PATIENTSTATUS"].ToString() != string.Empty)
            {
                active = Convert.ToInt32(Session["PATIENTSTATUS"].ToString());
            }
            string notes = string.Empty;
            string apptNumber = string.Empty;
            string apptTime = string.Empty;
            string ext_PMS_Code = string.Empty;
            string duration = string.Empty;
            string status = string.Empty;
            string comments = string.Empty;
            string email = txtEmail.Text;
            string paternalName = null;
            string maternalName = null;

            if (pnlPaternalMaternal.Visible)
            {
                paternalName = txtPaternalName.Text.Trim();
                maternalName = !string.IsNullOrEmpty(txtMaternalName.Text.Trim()) ? txtMaternalName.Text.Trim() : null;
                lastName = paternalName + " " + maternalName;
            }

            EPSBroker.SavePatient(licenseID, userId, patientID, chartID,
                lastName.Trim(), firstName, middleName, address1, address2, city, state, zip,
                phone, dob, account, sex, active, notes, ext_PMS_Code, comments, email, paternalName,
                maternalName, null, null, mobilePhone, null, string.Empty, string.Empty, string.Empty, null, base.DBID);

            sm.DBPatientID = patientID;

            base.AuditLogPatientInsert(ePrescribeSvc.AuditAction.PATIENT_TASK_PATIENT_RECONCILE, patientID);

            moveOn(patientID);
        }

        protected void btnNext_Click(object sender, EventArgs e)
        {
            string patientID = GetPatientIDAndUpdateObjectsEPrescribe();

            if (Session["InErrorState"] != null && Convert.ToBoolean(Session["InErrorState"]) == true)
            {
                //we got stuck somewhere
                Session.Remove("InErrorState");
                Response.End();
                return;
            }
            else
            {
                base.AuditLogPatientInsert(ePrescribeSvc.AuditAction.PATIENT_TASK_PATIENT_RECONCILE, patientID);
                moveOn(patientID);
            }
        }

        /// <summary>
        /// Returns patientID as selected in UI, and updates this page's ScriptMessage DBPatientID
        /// property in preparation for further processing (by the moveOn method).
        /// </summary>
        private string GetPatientIDAndUpdateObjectsEPrescribe()
        {
            string patientID = null;
            if (selectedPat.Value != null)
            {
                patientID = selectedPat.Value;
                sm.DBPatientID = patientID;
            }
            else
            {
                if (sm.DBPatientID != null)
                {
                    patientID = sm.DBPatientID;
                }
            }

            return (patientID);
        }

        private void moveOn(string patientID)
        {
            if (patientID != null)
            {
                string sessionPatientID = Session["PATIENTID"] != null ? Session["PATIENTID"].ToString() : string.Empty;
                Session["PATIENTID"] = patientID;

                //Session patient is not the selected patient, then set the header with selected patient's info
                if (!patientID.Equals(sessionPatientID))
                    ((PhysicianMasterPageBlank)Master).SetPatientInfo(patientID);

                ucObsoletePARClassMappingChange.LoadPatientActiveObsoletePARClass();

                if (Request.QueryString["From"] != null)
                {
                    ucObsoletePARClassMappingChange.PatientObsoletedAllergyCheckCalledFrom = (Request.QueryString["From"] == Constants.PageNames.PHARMACY_REFILL_SUMMARY) ?
                                                                                            Constants.PageNames.PHARMACY_REFILL_SUMMARY :
                                                                                            Constants.PageNames.DOC_REFILL_MENU;
                }

                if (!ucObsoletePARClassMappingChange.ShowIfPatientHasActiveObsoletedParClass())
                {
                    if (sm.DBPharmacyID != null && sm.DBPharmacyID != string.Empty)
                    {
                        Session["PHARMACYID"] = sm.DBPharmacyID;
                    }
                    else
                    {
                        //where's the pharmID?  get it from the NABP
                        if (sm.DBPharmacyNetworkID != null && sm.DBPharmacyNetworkID != string.Empty)
                        {
                            DataTable dt = Allscripts.Impact.Pharmacy.LoadPharmacyByNABP(sm.DBPharmacyNetworkID, base.DBID).Tables[0];

                            if (dt != null)
                            {
                                if (dt.Rows.Count > 0)
                                {
                                    Session["PHARMACYID"] = dt.Rows[0]["PharmacyID"].ToString();
                                }
                                else
                                {
                                    //uh oh
                                    Session["InErrorState"] = true;
                                    ucMessage.Icon = Controls_Message.MessageType.ERROR;
                                    ucMessage.MessageText = "There was a problem linking the pharmacy to this refill request. NCPDP" + sm.DBPharmacyNetworkID + "  Please contact <a href='" + ConfigurationManager.AppSettings["HelpURL"] + "/contact.asp' target='_blank'>Veradigm Support</a>.";
                                    ucMessage.Visible = true;
                                    btnNext.Enabled = false;
                                    //close out the task
                                    ScriptMessage.RejectMessage(sm, string.Empty, "NCPDPID not in dictionary", SessionUserID, SessionLicenseID, string.Empty, base.ShieldSecurityToken, base.SessionSiteID, string.Empty, base.DBID);
                                    return;
                                }
                            }
                            else
                            {
                                //uh oh
                                Session["InErrorState"] = true;
                                ucMessage.Icon = Controls_Message.MessageType.ERROR;
                                ucMessage.MessageText = "There was a problem linking the pharmacy to this refill request.  Please contact <a href='" + ConfigurationManager.AppSettings["HelpURL"] + "/contact.asp' target='_blank'>Veradigm Support</a>.";
                                ucMessage.Visible = true;
                                btnNext.Enabled = false;
                                ScriptMessage.RejectMessage(sm, string.Empty, "NCPDPID not in dictionary", SessionUserID, SessionLicenseID, string.Empty, base.ShieldSecurityToken, base.SessionSiteID, string.Empty, base.DBID);
                                return;
                            }
                        }
                        else
                        {
                            //uh oh
                            Session["InErrorState"] = true;
                            ucMessage.Icon = Controls_Message.MessageType.ERROR;
                            ucMessage.MessageText = "There was a problem linking the pharmacy to this refill request.  Please contact <a href='" + ConfigurationManager.AppSettings["HelpURL"] + "/contact.asp' target='_blank'>Veradigm Support</a>.";
                            ucMessage.Visible = true;
                            btnNext.Enabled = false;
                            ScriptMessage.RejectMessage(sm, string.Empty, "NCPDPID not in dictionary", SessionUserID, SessionLicenseID, string.Empty, base.ShieldSecurityToken, base.SessionSiteID, string.Empty, base.DBID);
                            return;
                        }
                    }

                    DataSet pharmacyDS = Allscripts.Impact.Pharmacy.LoadPharmacy(Session["PharmacyID"].ToString(), base.DBID);

                    if (pharmacyDS.Tables[0].Rows.Count > 0)
                        Session["SentTo"] = pharmacyDS.Tables[0].Rows[0]["Name"].ToString();

                    //update pharmacy if patient does not have preferred pharmacy
                    if (Session[Constants.SessionVariables.LastPharmacyID] == null || Session[Constants.SessionVariables.LastPharmacyName] == null)
                    {
                        bool isMOB = Convert.ToBoolean(pharmacyDS.Tables[0].Rows[0]["MOBFlag"]);

                        DataSet ds = Patient.UpdatePharmacyID(
                           Session[Constants.SessionVariables.PatientId].ToString(),
                           Session[Constants.SessionVariables.PHARMACYID].ToString(),
                           isMOB,
                           base.DBID);
                    }

                    Session[Constants.SessionVariables.TaskScriptMessageId] = sm.DBScriptMessageID;

                    ((PhysicianMasterPageBlank)Page.Master).SetPatientInfo(patientID);

                    if (sm.MessageType.Trim() == Constants.PrescriptionTaskType.RXCHG.ToString())
                    {
                        var from = Request.QueryString["From"];
                        if (from != null && from == Constants.PageNames.PHARMACY_REFILL_SUMMARY)
                        {
                            Response.Redirect($"{Constants.PageNames.PHARMACY_REFILL_DETAILS}" +
                                              $"?MessageID={PageState.GetStringOrEmpty(Constants.SessionVariables.TaskScriptMessageId)}" +
                                              $"&PatientID={PageState.GetStringOrEmpty("PATIENTID")}" +
                                              $"&ProviderID={PageState.GetStringOrEmpty("PhysicianId")}");
                        }
                        else
                        {
                            Response.Redirect(Constants.PageNames.APPROVE_REFILL_TASK + string.Format("?PatientID={0}&Patient={1}&PhyId={2}&From={3}",
                                        PageState.GetStringOrEmpty("PATIENTID"),
                                        PageState.GetStringOrEmpty("PatientName"),
                                        PageState.GetStringOrEmpty("PhysicianId"),
                                        Constants.PageNames.DOC_REFILL_MENU));
                        }
                    }

                    if (sm.MessageType.Trim() == Constants.PrescriptionTaskType.REFREQ.ToString())
                    {
                        var from = Request.QueryString["From"];
                        if (from != null && from == Constants.PageNames.PHARMACY_REFILL_SUMMARY)
                        {
                            Response.Redirect($"{Constants.PageNames.PHARMACY_REFILL_DETAILS}" +
                                              $"?MessageID={PageState.GetStringOrEmpty(Constants.SessionVariables.TaskScriptMessageId)}" +
                                              $"&PatientID={PageState.GetStringOrEmpty("PATIENTID")}" +
                                              $"&ProviderID={PageState.GetStringOrEmpty("PhysicianId")}");
                        }
                        else if (!string.IsNullOrEmpty(sm.RxControlledSubstanceCode))
                        {
                            Response.Redirect(Constants.PageNames.APPROVE_REFILL_TASK + string.Format("?PatientID={0}&Patient={1}&PhyId={2}&From={3}",
                                                  PageState.GetStringOrEmpty("PATIENTID"),
                                                  PageState.GetStringOrEmpty("PatientName"),
                                                  PageState.GetStringOrEmpty("PhysicianId"),
                                                  Constants.PageNames.APPROVE_REFILL_TASK));
                        }
                    }

                    //if the med is codified, move to sig otherwise match the med
                    if (sm.DBDDID == null || sm.DBDDID == string.Empty)
                    {
                        switch ((Constants.UserCategory)Session["UserType"])
                        {
                            case Constants.UserCategory.PROVIDER:
                            case Constants.UserCategory.PHYSICIAN_ASSISTANT:
                                Server.Transfer(Constants.PageNames.REDIRECT_TO_ANGULAR + "?componentName=" + Constants.PageNames.SELECT_MEDICATION + "?Search=A&searchText=" + HttpUtility.UrlEncode(sm.RxDrugDescription + " " + sm.RxStrength));
                                break;
                            case Constants.UserCategory.POB_SUPER:
                            case Constants.UserCategory.POB_REGULAR:
                            //
                            // TODO: see if we can send pa w/sup to SELECT MEDICATION instead of NurseSelectMedication, if not we might have to implement cs/epcs functionality on Nurse Sig since it follows NurseFullScript
                            //
                            case Constants.UserCategory.PHYSICIAN_ASSISTANT_SUPERVISED:
                            case Constants.UserCategory.POB_LIMITED:
                                SetDelegateProvider(sm.ProviderID);
                                Server.Transfer(Constants.PageNames.REDIRECT_TO_ANGULAR + "?componentName=" + Constants.PageNames.SELECT_MEDICATION + "?Search=A&searchText=" + HttpUtility.UrlEncode(sm.RxDrugDescription + " " + sm.RxStrength));
                                break;
                            default:
                                Server.Transfer(Constants.PageNames.REDIRECT_TO_ANGULAR + "?componentName=" + Constants.PageNames.SELECT_MEDICATION + "?Search=A&searchText=" + HttpUtility.UrlEncode(sm.RxDrugDescription + " " + sm.RxStrength));
                                break;
                        }
                    }
                    else
                    {
                        ArrayList rxList = new ArrayList();
                        Rx rx = new Rx();

                        rx.ScriptMessageID = Request.QueryString["smid"]?.ToString();
                        rx.DDI = sm.DBDDID;

                        if (!string.IsNullOrEmpty(rx.DDI))
                        {
                            GetPatientCoverage(patientID);

                            string coverageId = (Session["SelectedCoverageID"] != null) ? Session["SelectedCoverageID"].ToString() : "0";
                            string formularyId = (Session["FormularyID"] != null) ? Session["FormularyID"].ToString() : string.Empty;
                            string otcCoverage = (Session["OTCCoverage"] != null) ? Session["OTCCoverage"].ToString() : string.Empty;

                            int genericDrugPolicy = 0;
                            if (Session["GenericDrugPolicy"] != null)
                            {
                                int.TryParse(Session["GenericDrugPolicy"].ToString(), out genericDrugPolicy);
                            }

                            int unListedDrugPolicy = 0;
                            if (Session["UnListedDrugPolicy"] != null)
                            {
                                int.TryParse(Session["UnListedDrugPolicy"].ToString(), out unListedDrugPolicy);
                            }

                            ePrescribeSvc.Medication medicationInfo = EPSBroker.LoadMedicationByDDIAndCoverage(rx.DDI,
                                 coverageId,
                                 formularyId,
                                 otcCoverage,
                                 genericDrugPolicy,
                                 unListedDrugPolicy,
                                 base.DBID);

                            if (medicationInfo != null)
                            {
                                rx.MedicationName = medicationInfo.MedicationName;
                                rx.Strength = medicationInfo.Strength;
                                rx.ControlledSubstanceCode = medicationInfo.ControlledSubstanceCode;
                                rx.StrengthUOM = medicationInfo.StrengthUOM;
                                rx.RouteOfAdminCode = medicationInfo.RouteOfAdminCode;
                                rx.RouteOfAdminDescription = medicationInfo.RouteOfAdmin;
                                rx.DosageFormCode = medicationInfo.DosageFormCode;
                                rx.DosageFormDescription = medicationInfo.DosageForm;
                                rx.DAW = sm.RxDAW == Constants.CommonAbbreviations.YES;

                                rx.StateControlledSubstanceCode = Prescription.GetStateControlledSubstanceCode(rx.DDI, Session["PRACTICESTATE"].ToString(), sm.PharmacyState, base.DBID);

                                if (Session["FormularyActive"] != null && Session["FormularyActive"].ToString() == "Y")
                                {
                                    rx.FormularyStatus = medicationInfo.FormularyStatus;
                                    rx.LevelOfPreferedness = medicationInfo.LevelOfPreferedness;
                                    rx.IsOTC = medicationInfo.IsOTC;
                                }

                                switch ((Constants.UserCategory)Session["UserType"])
                                {
                                    case Constants.UserCategory.PROVIDER:
                                    case Constants.UserCategory.PHYSICIAN_ASSISTANT:
                                    case Constants.UserCategory.PHYSICIAN_ASSISTANT_SUPERVISED:
                                        if (iPrescription.IsValidMassOpiate(
                                            PageState.GetStringOrEmpty(Constants.SessionVariables.PracticeState),
                                            medicationInfo.GPI,
                                            rx.ControlledSubstanceCode,
                                            Convert.ToBoolean(Session["HasExpiredDEA"]),
                                            (List<string>)(Session["DEASCHEDULESALLOWED"]))
                                            )
                                        {
                                            rx.Notes = Constants.PartialFillUponPatientRequest;
                                        }
                                        break;
                                    case Constants.UserCategory.POB_SUPER:
                                    case Constants.UserCategory.POB_REGULAR:
                                    case Constants.UserCategory.POB_LIMITED:
                                        SetDelegateProvider(sm.ProviderID);

                                        List<string> deaSchedulesAllowed = new List<string>();
                                        if (base.DelegateProvider.DEAScheduleAllowed(2))
                                        {
                                            deaSchedulesAllowed.Add("2");
                                        }

                                        if (iPrescription.IsValidMassOpiate(
                                            PageState.GetStringOrEmpty(Constants.SessionVariables.PracticeState),
                                            medicationInfo.GPI,
                                            rx.ControlledSubstanceCode,
                                            base.DelegateProvider.IsDEAExpired(),
                                            deaSchedulesAllowed
                                            ))
                                        {
                                            rx.Notes = Constants.PartialFillUponPatientRequest;
                                        }
                                        break;
                                }

                                rx.CoverageID = (Session["SelectedCoverageID"] != null) ? Convert.ToInt64(Session["SelectedCoverageID"].ToString()) : 0;
                                rx.FormularyID = (Session["FormularyID"] != null) ? Session["FormularyID"].ToString().Trim() : null;
                                rx.PlanID = (Session["PlanID"] != null) ? Session["PlanID"].ToString().Trim() : null;
                            }
                        }

                        rxList.Add(rx);
                        Session["RxList"] = rxList;

                        switch ((Constants.UserCategory)Session["UserType"])
                        {
                            case Constants.UserCategory.PROVIDER:
                            case Constants.UserCategory.PHYSICIAN_ASSISTANT:
                            // since this is a refill request workflow, send PA w/sup to sig page instead of Nurse Sig
                            case Constants.UserCategory.PHYSICIAN_ASSISTANT_SUPERVISED:
                                Server.Transfer(Constants.PageNames.SIG);
                                break;
                            case Constants.UserCategory.POB_SUPER:
                            case Constants.UserCategory.POB_REGULAR:
                            //case Constants.UserCategory.PHYSICIAN_ASSISTANT_SUPERVISED:
                            case Constants.UserCategory.POB_LIMITED:
                                Server.Transfer(Constants.PageNames.NURSE_SIG);
                                break;
                        }
                    }
                }
                else
                {
                    Session["AutoSelectPatient"] = false;
                }
            }
        }

        protected void GetPatientCoverage(string patientID)
        {
            if (!string.IsNullOrEmpty(patientID))
            {
                DataSet dsCoverage = Allscripts.Impact.PatientCoverage.GetPatientCoverages(patientID, base.SessionLicenseID, base.SessionUserID, true, base.DBID);

                if (dsCoverage != null && dsCoverage.Tables.Count > 0 && dsCoverage.Tables[0].Rows.Count > 0)
                {
                    Session["SelectedCoverageID"] = dsCoverage.Tables[0].Rows[0]["SelectedCoverageID"].ToString();

                    DataSet ds = PatientCoverage.GetCoverage(Convert.ToInt64(Session["SelectedCoverageID"].ToString()), base.DBID);

                    DataTable dtCoverage = ds.Tables["PatientCoverage"];
                    DataTable dtPlan = ds.Tables["PatientPlan"];

                    DataRow drCoverage = null;
                    DataRow drPlan = null;

                    if (dtPlan != null && dtPlan.Rows.Count > 0)
                        drPlan = dtPlan.Rows[0];

                    if (drPlan != null)
                    {
                        Session["PlanID"] = drPlan["PlanID"].ToString();
                        Session["FormularyID"] = drPlan["FormularyID"].ToString();
                        Session["FormularyActive"] = drPlan["Active"].ToString();
                        Session["OTCCoverage"] = drPlan["OTCCoverage"].ToString();
                        Session["GenericDrugPolicy"] = drPlan["GenericDrugPolicy"].ToString();
                        Session["UnListedDrugPolicy"] = drPlan["UnlistedDrugPolicy"].ToString();
                    }

                    if (dtCoverage != null && dtCoverage.Rows.Count > 0)
                        drCoverage = dtCoverage.Rows[0];

                    if (drCoverage != null)
                    {
                        Session["CoverageID"] = drCoverage["coverageID"] != DBNull.Value ? drCoverage["coverageID"].ToString() : string.Empty;
                        Session["CopayID"] = drCoverage["copayID"] != DBNull.Value ? drCoverage["copayID"].ToString() : string.Empty;
                        Session["AltPlanID"] = drCoverage["AltPlanID"] != DBNull.Value ? drCoverage["AltPlanID"].ToString() : string.Empty;
                    }
                }
            }
        }

        private void SetDelegateProvider(string providerID)
        {
            if (providerID.Trim() != Session["UserID"].ToString())
            {
                Session["DelegateProviderID"] = providerID;
                setSPI(providerID);
            }
        }

        private void setSPI(string providerID)
        {
            DataSet dsSPI = Provider.GetSPI(providerID, base.DBID);
            if (dsSPI.Tables.Count > 0)
            {
                DataRow[] drSPI = dsSPI.Tables[0].Select("ScriptSwId='SURESCRIPTS'");
                //should only be one row for SURESCRIPTS...grab the first and only
                if (drSPI.Length > 0 && drSPI[0] != null && drSPI[0]["SenderId"] != DBNull.Value && drSPI[0]["SenderId"].ToString() != string.Empty)
                {
                    Session["SPI"] = drSPI[0]["SenderID"].ToString();
                }
                else
                {
                    Session["SPI"] = null;
                }
            }
            else
            {
                Session["SPI"] = null;
            }
        }

        public static string BuildAddressDisplay(string address1, string address2, string city, string state, string zip, bool insertHtmlLineBreaks)
        {
            string lineBreak = insertHtmlLineBreaks ? "<br />\r\n" : "\r\n";

            string displayAddress = String.Empty;

            if (!String.IsNullOrEmpty(address1) && address1.Trim().Length > 0)
            {
                displayAddress = address1.Trim().ToHTMLEncode();
            }

            if (!String.IsNullOrEmpty(address2) && address2.Trim().Length > 0)
            {
                if (!String.IsNullOrEmpty(displayAddress)) { displayAddress += lineBreak; }
                displayAddress += address2.Trim().ToHTMLEncode();
            }

            string cityStateZip = BuildCityStateZipDisplay(city, state, zip);
            if (!String.IsNullOrEmpty(cityStateZip) && cityStateZip.Trim().Length > 0)
            {
                if (!String.IsNullOrEmpty(displayAddress)) { displayAddress += lineBreak; }
                displayAddress += cityStateZip.Trim().ToHTMLEncode();
            }

            return (displayAddress.Trim());
        }

        public static string BuildCityStateZipDisplay(string city, string state, string zip)
        {
            string display = String.Empty;

            if (!String.IsNullOrEmpty(city) && city.Trim().Length > 0)
            {
                display = city.Trim();
            }

            if (!String.IsNullOrEmpty(state) && state.Trim().Length > 0)
            {
                if (!String.IsNullOrEmpty(display)) { display += ", "; }
                display += state;
            }

            if (!String.IsNullOrEmpty(zip) && zip.Trim().Length > 0)
            {
                if (!String.IsNullOrEmpty(display)) { display += " "; }
                display += zip;
            }

            return (display.Trim());
        }

        public void GetPatientAllergynMedication()
        {
            DataSet patds = Allscripts.Impact.CHPatient.LoadPatient(Session["selectedPatient"].ToString(), base.SessionLicenseID, base.SessionUserID, base.DBID);
            setPatientMedAndAllergyInfo(patds);
        }

        public void GetPatientAllergynMedication(string patientID)
        {
            DataSet patds = Allscripts.Impact.CHPatient.LoadPatient(patientID, base.SessionLicenseID, base.SessionUserID, base.DBID);
            setPatientMedAndAllergyInfo(patds);
        }

        public void PatientSearch(string patientID)
        {
            DataSet ds = Allscripts.Impact.CHPatient.SearchPatient(base.SessionLicenseID, null, null, null, null, base.SessionUserID, base.SessionLicense.hasVIPPatients, base.SessionUserType, patientID, false, base.DBID);

            grdViewPatients.DataSourceID = null;
            grdViewPatients.DataSource = ds.Tables[0];
            grdViewPatients.DataBind();
            txtFirstNameSearch.Text = string.Empty;
            txtLastNameSearch.Text = string.Empty;
        }
    }
}