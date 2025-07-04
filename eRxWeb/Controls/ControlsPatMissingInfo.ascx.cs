using System;
using System.Data;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.Objects;
using Allscripts.ePrescribe.Shared.Logging;
using Allscripts.Impact;
using Allscripts.Impact.Interfaces;
using Allscripts.Impact.Utilities;
using eRxWeb.Controls;
using eRxWeb.Controls.Interfaces;
using static Allscripts.ePrescribe.Objects.SystemConfig;
using SystemConfig = Allscripts.Impact.SystemConfig;
using Weight = Allscripts.ePrescribe.Objects.Weight;

namespace eRxWeb
{
    public partial class ControlsPatMissingInfo : BaseControl, IControlsPatMissingInfo
    {
        public delegate void PatientDemographicsEditCompleteHandeler(PatientDemographicsEditEventArgs patDemoEventArgs);
        public event PatientDemographicsEditCompleteHandeler OnPatEditComplete;

        private static readonly ILoggerEx logger = LoggerEx.GetLogger();

        public static string DateFormat = "yyyy-MM-dd HH:mm:ss.fff";

        #region Page Events and Event Handlers

        #region Properties
        #endregion
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        public void Show()
        {
            if (PatientUpdate.AllowEditsForMissingDemographics(((BasePage)Page).SessionLicense.EnterpriseClient,
                ((BasePage)Page).SessionLicense.EnterpriseClient.EditPatient,
                ControlState.GetInt(Constants.SessionVariables.PatientYearsOld, 0),
                ControlState.Cast<Weight>(Constants.SessionVariables.PatientWeight, null)?.Kgs,
                ControlState.Cast<Height>(Constants.SessionVariables.PatientHeight, null)?.Cm))
            {
                pnlPatientDemographicsEditPopUp.Visible = true;
                panelEnterpriseEditPatientOff.Visible = false;
                panelPatientDemographics.Visible = true;
                SystemConfig.GetStateList(ddlState);
                ResetErrors();
                SetPatientDemo();
                PopulateImperialMeasurements();
            }
            else
            {
                pnlPatientDemographicsEditPopUp.Visible = true;
                panelPatientDemographics.Visible = false;
                panelEnterpriseEditPatientOff.Visible = true;
            }

            logger.Debug("Patient missing address 1 or city");

            mpePatientDemoEdit.Show();
        }

        private void SetPatientDemo()
        {
            txtFirstName.Text = ControlState.GetStringOrEmpty("PATIENTFIRSTNAME");
            txtLastName.Text = ControlState.GetStringOrEmpty("PATIENTLASTNAME");

            var dob = ControlState.GetDateTimeOrMin("PATIENTDOB");
            if (dob != DateTime.MinValue)
            {
                txtDOB.SelectedDate = dob;
            }
            ddlGender.SelectedValue = ControlState["SEX"] == null ? "U" : ControlState.GetStringOrEmpty("SEX");
            txtAddress.Text = ControlState.GetStringOrEmpty("PATIENTADDRESS1");
            txtAddress2.Text = ControlState.GetStringOrEmpty("PATIENTADDRESS2");
            txtCity.Text = ControlState.GetStringOrEmpty("PATIENTCITY");
            ddlState.SelectedValue = string.IsNullOrWhiteSpace(ControlState.GetStringOrEmpty(Constants.SessionVariables.PatientState).Trim()) ? ddlState.Items[0].Value : ControlState.GetStringOrEmpty("PATIENTSTATE");
            txtZip.Text = ControlState.GetStringOrEmpty("PATIENTZIP");
            txtHeightCm.Text = ControlState.Cast<Height>(Constants.SessionVariables.PatientHeight, null)?.Cm;
            txtWeightKg.Text = ControlState.Cast<Weight>(Constants.SessionVariables.PatientWeight, null)?.Kgs;

            PopulateImperialMeasurements();

            if (ControlState.GetInt(Constants.SessionVariables.PatientYearsOld, 0) <= 18)
            {
                errWeight.Visible = true;
                errHeight.Visible = true;
            }

            AuditPatientObject auditPatient = new AuditPatientObject(ePrescribeApplication.MainApplication, SessionUserID, SessionLicenseID, Request.UserIpAddress(), SessionPatientID, string.Empty, string.Empty, string.Empty, string.Empty, DBID);
            IAuditPatient audit = new AuditPatient();
            audit.InsertPatDemoAuditRow(auditPatient);

        }

        protected void btnSavePatientDemo_Click(object sender, EventArgs e)
        {
            ResetErrors();

            var errorMessage = ValidatePatInfo();

            if (errorMessage.Length > 0)
            {
                logger.Error("Validation of patient info failed: {0}", errorMessage.ToString());

                SetErrorMessage(errorMessage);

                panelPatientDemographics.Visible = true;
                pnlPatientDemographicsEditPopUp.Visible = true;
                panelEnterpriseEditPatientOff.Visible = false;

                if (ControlState.GetInt(Constants.SessionVariables.PatientYearsOld, 0) <= 18)
                {
                    errWeight.Visible = true;
                    errHeight.Visible = true;
                }

                mpePatientDemoEdit.Show();
            }
            else
            {
                var eventArgs = new PatientDemographicsEditEventArgs(true);
                try
                {
                    string dob = Convert.ToDateTime(txtDOB.SelectedDate).ToString("MM/dd/yyyy");

                    var patient = CHPatient.PatientSearchById(SessionPatientID, SessionLicenseID, SessionUserID, DBID).Tables["Patient"].Rows[0];

                    string origHeight = ControlState.Cast<Height>(Constants.SessionVariables.PatientHeight, null)?.Cm;
                    string origWeight = ControlState.Cast<Weight>(Constants.SessionVariables.PatientWeight, null)?.Kgs;

                    EPSBroker.SavePatient(SessionLicenseID, SessionUserID, SessionPatientID, patient["ChartID"].ToString(),
                            txtLastName.Text, txtFirstName.Text, patient["MiddleInitial"].ToString(), txtAddress.Text, txtAddress2.Text, txtCity.Text, ddlState.SelectedValue,
                            txtZip.Text, patient["Phone"].ToString(), dob, string.Empty, ddlGender.SelectedValue, Convert.ToInt32(patient["StatusID"]),
                            string.Empty, string.Empty, string.Empty, patient["Email"].ToString(), patient["PaternalName"].ToString(), patient["MaternalName"].ToString(),
                            null, null, patient["MobilePhone"].ToString(), null, string.Empty, txtWeightKg.Text, txtHeightCm.Text, null, base.DBID);
                    //We need to handle this in DB. There is already a backlog PBI to revert checking through application
                    ControlState[Constants.SessionVariables.OriginalWeight] = txtWeightKg.Text;
                    ControlState[Constants.SessionVariables.OriginalHeight] = txtHeightCm.Text;
                    AuditPatientObject auditPatient = new AuditPatientObject(ePrescribeApplication.MainApplication, SessionUserID, SessionLicenseID, Request.UserIpAddress(), SessionPatientID, origHeight, txtHeightCm.Text, origWeight, txtWeightKg.Text, DBID);
                    IAuditPatient audit= new AuditPatient();
                    audit.InsertPatDemoAuditRow(auditPatient);


                    ResetPatSession(dob);
                }
                catch (Exception ex)
                {
                    string exceptionId = Audit.AddException(SessionUserID, SessionLicenseID, "Error updating Patient via popup: " + ex.ToString(), string.Empty, string.Empty, string.Empty, DBID);

                    logger.Error("Exception while saving patient. ExceptionId = {0} Exception: {1}", exceptionId, ex.ToString());

                    eventArgs = new PatientDemographicsEditEventArgs(false)
                    {
                        Message = "Patient information could not be updated at this time.  Please contact support with Exception ID: " + exceptionId
                    };
                }

                if (OnPatEditComplete != null)
                {
                    OnPatEditComplete(eventArgs);
                }
            }
        }

        private void ResetPatSession(string dob)
        {
            ControlState["PATIENTFIRSTNAME"] = txtFirstName.Text;
            ControlState["PATIENTLASTNAME"] = txtLastName.Text;
            ControlState["SEX"] = ddlGender.SelectedValue;
            ControlState["PATIENTDOB"] = dob;
            ControlState["PATIENTADDRESS1"] = txtAddress.Text;
            ControlState["PATIENTADDRESS2"] = txtAddress2.Text;
            ControlState["PATIENTCITY"] = txtCity.Text;
            ControlState["PATIENTSTATE"] = ddlState.SelectedValue;
            ControlState["PATIENTZIP"] = txtZip.Text;
            ControlState[Constants.SessionVariables.PatientWeight] = new Weight(Convert.ToString(txtWeightKg.Text));
            ControlState[Constants.SessionVariables.PatientHeight] = new Height(Convert.ToString(txtHeightCm.Text));
        }

        private void SetErrorMessage(StringBuilder errorMessage)
        {
            msgErr.MessageText = errorMessage.ToString();
            msgErr.Visible = true;
            msgErr.Icon = Controls_Message.MessageType.ERROR;
        }

        private StringBuilder ValidatePatInfo()
        {
            StringBuilder sbMessage = new StringBuilder();

            double height = 0.0;
            double weight = 0.00;
            int feet = 0;
            int inches = 0;
            int lbs = 0;
            int ounces = 0;

            if (string.IsNullOrWhiteSpace(txtFirstName.Text))
            {
                sbMessage.AppendLine("First Name is required. ");
                errFirstName.Visible = true;
            }
            else if (!Regex.Match(txtFirstName.Text, @"^([a-zA-Z0-9]+[\s-'.]{0,35})*$").Success)
            {
                sbMessage.AppendLine("Please enter a valid first name. ");
                errFirstName.Visible = true;
            }

            if (string.IsNullOrWhiteSpace(txtLastName.Text))
            {
                sbMessage.AppendLine("Last Name is required. ");
                errLastName.Visible = true;
            }
            else if (!Regex.Match(txtLastName.Text, @"^([a-zA-Z0-9]+[\s-'.]{0,35})*$").Success)
            {
                sbMessage.AppendLine("Please enter a valid last name. ");
                errLastName.Visible = true;
            }

            if (txtDOB.IsEmpty)
            {
                sbMessage.AppendLine("Date of Birth is required. ");
                errDOB.Visible = true;
            }

            if (string.IsNullOrEmpty(txtAddress.Text))
            {
                sbMessage.AppendLine("Address is required. ");
                errAddress1.Visible = true;
            }

            if (string.IsNullOrEmpty(txtCity.Text.Trim()))
            {
                sbMessage.AppendLine("City is required.");
                errCity.Visible = true;
            }
            // adding ECMAScript option .NET regex works like javascirpt regex, and adding $ meta character to close string
            else if (!Regex.Match(txtCity.Text, @"^([a-zA-Z]+[\s-'.]{0,20})*$", RegexOptions.ECMAScript).Success)
            {
                sbMessage.AppendLine("Please enter a valid city.");
                errCity.Visible = true;
            }

            if (string.IsNullOrWhiteSpace(txtZip.Text))
            {
                sbMessage.AppendLine("Zip is required.");
                errZip.Visible = true;
            }
            else if (!Regex.Match(txtZip.Text, @"^\d{5}$").Success)
            {
                sbMessage.AppendLine("Please enter a valid 5-digit ZIP code.");
                errZip.Visible = true;
            }

            if (Height.IsValidHeightFormat(Convert.ToString(txtHeightCm.Text)))
            {
                if (txtHeightCm.Text.Equals("."))
                {
                    sbMessage.AppendLine("Height format invalid");
                    errCm.Visible = true;
                }
                else
                {
                    feet = string.IsNullOrEmpty(txtHeightFt.Text) ? 0 : Convert.ToInt16(txtHeightFt.Text);
                    inches = string.IsNullOrEmpty(txtHeightIn.Text) ? 0 : Convert.ToInt16(txtHeightIn.Text);
                }

                try
                {
                    height = Convert.ToDouble(Height.GetCm(new Height(txtHeightCm.Text)));
                }
                catch
                {
                    if(!sbMessage.ToString().Contains("Height format invalid"))
                        sbMessage.AppendLine("Height format invalid");
                    errCm.Visible = true;
                }

                if (ControlState.GetInt(Constants.SessionVariables.PatientYearsOld, 0) <= 18)
                {
                    if (height <= 0)
                    {
                        sbMessage.AppendLine("Height must be greater than 0");
                        errCm.Visible = true;
                    }
                }
            }
            else
            {
                sbMessage.AppendLine("Cm format invalid");
                errWeight.Visible = true;
            }

            if (Weight.IsValidWeightFormat(Convert.ToString(txtWeightKg.Text)))
            {
                if(txtWeightKg.Text.Equals("."))
                {
                    sbMessage.AppendLine("Weight format invalid");
                    errKg.Visible = true;
                }
                else
                {
                    lbs = string.IsNullOrEmpty(txtWeightLbs.Text) ? 0 : Convert.ToInt16(txtWeightLbs.Text);
                    ounces = string.IsNullOrEmpty(txtWeightOzs.Text) ? 0 : Convert.ToInt16(txtWeightOzs.Text);
                }

                try
                {
                    weight = Convert.ToDouble(Weight.GetKg(new Weight(txtWeightKg.Text)));

                }
                catch
                {
                    if(!sbMessage.ToString().Contains("Weight format invalid"))
                        sbMessage.AppendLine("Weight format invalid");
                    errKg.Visible = true;
                }

                if (ControlState.GetInt(Constants.SessionVariables.PatientYearsOld, 0) <= 18)
                {
                    if (weight <= 0.00)
                    {
                        sbMessage.AppendLine("Weight must be greater than 0");
                        errKg.Visible = true;
                    }
                }
            }
            else
            {
                sbMessage.AppendLine("Kg format invalid");
                errHeight.Visible = true;
            }

            return sbMessage;
        }

        private void ResetErrors()
        {
            msgErr.Visible = false;
            errFirstName.Visible = false;
            errLastName.Visible = false;
            errDOB.Visible = false;
            errAddress1.Visible = false;
            errCity.Visible = false;
            errZip.Visible = false;
            errHeight.Visible = false;
            errWeight.Visible = false;
            errHeightFt.Visible = false;
            errHeightIn.Visible = false;
            errWeightLb.Visible = false;
            errWeightOz.Visible = false;
            errCm.Visible = false;
            errKg.Visible = false;
        }

        protected void btnCancelFromPatientDemo_Click(object sender, EventArgs e)
        {
            mpePatientDemoEdit.Hide();
        }

        protected void btnEnterpriseEditPatientOffOk_Click(object sender, EventArgs e)
        {
            if (OnPatEditComplete != null)
            {
                OnPatEditComplete(new PatientDemographicsEditEventArgs(false));
            }
        }

        #endregion
        protected void PopulateImperialMeasurements()
        {
            if (txtWeightKg.Text != "")
            {
                var kgValue = Convert.ToDouble(txtWeightKg.Text);
                var ozs = Math.Round(((kgValue * 2.20462) % 1) * 16);
                var lbs = Math.Floor(kgValue * 2.20462);
                if (ozs == 16)
                {
                    ozs = 0;
                    lbs++;
                }
                txtWeightLbs.Text = Convert.ToString(lbs);
                txtWeightOzs.Text = Convert.ToString(ozs);
            }
            else
            {
                txtWeightLbs.Text = "0";
                txtWeightOzs.Text = "0";
                txtWeightKg.Text = "0.00";
            }

            if (txtHeightCm.Text != "")
            {
                var cmValue = Convert.ToDouble(txtHeightCm.Text);
                cmValue /= 2.54;
                var feet = Math.Floor(cmValue / 12);
                var inches = Math.Round(cmValue % 12);
                if (inches == 12)
                {
                    inches = 0;
                    feet++;
                }
                txtHeightFt.Text = Convert.ToString(feet);
                txtHeightIn.Text = Convert.ToString(inches);
            }
            else
            {
                txtHeightFt.Text = "0";
                txtHeightIn.Text = "0";
                txtHeightCm.Text = "0.0";
            }
        }


    }
}