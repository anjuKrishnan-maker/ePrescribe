
/******************************************************************************
**Change History
*******************************************************************************
**Date:      Author:                    Description:
**-----------------------------------------------------------------------------
 ** 05/25/2010   Sonal                   #3469-Change "cancel/done" button inconsistencies
 ** 11/09/2011   Narendra Meena          Removed SSN in edit and Add patient Process
*******************************************************************************/
using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Net;
using System.Collections.Generic;
using System.Text;
using Telerik.Web.UI;
using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.Data;
using Allscripts.ePrescribe.ExtensionMethods;
using eRxWeb.AppCode;
using eRxWeb.ePrescribeSvc;
using RxUser = Allscripts.Impact.RxUser;
using eRxWeb.AppCode.SpecialtyMedWorkflow;
using eRxWeb.AppCode.Interfaces;
using ConnectionStringPointer = Allscripts.ePrescribe.DatabaseSelector.ConnectionStringPointer;
using Allscripts.ePrescribe.Objects;
using eRxWeb.ServerModel;

//Added May 24 the required Field Validator for DOB, ZIP , Phone  
//Changed the DOB reqularexpression to accept m/d/yyyy June 5th 2006
//Removed the formatting of the DOB (mm/dd/yy) JULY06
//Extended the address1 and address2 field in patients table to 50characters
// HA AKS Id 89 : Added a blank value for displaying in the formulary list. 

namespace eRxWeb
{
public partial class AddPatient : BasePage 
{
    
	private string nextPageUrl; //set the url of the next page that needs to be rendered
    private bool isPatientSaved;

    public string WeightOnLoad
    {
        get { return Convert.ToString(ViewState["WeightOnLoad"]); }
        set { ViewState["WeightOnLoad"] = value; }
    }

        public string HeightOnLoad
        {
            get { return Convert.ToString(ViewState["HeightOnLoad"]); }
            set { ViewState["HeightOnLoad"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
    {       
       txtDOB.MinDate = DateTime.Today.AddYears(-120);
       txtDOB.MaxDate = DateTime.Today;
       txtDOB.Culture = System.Globalization.CultureInfo.CurrentCulture;
       txtDOB.IncrementSettings.InterceptArrowKeys = false;
       txtDOB.IncrementSettings.InterceptMouseWheel = false;

       
            //check for test patient
            if (Session["PATIENTSTATUS"] != null && Session["PATIENTSTATUS"].ToString() == "99")
       {
           txtFName.Enabled = false;
           txtLName.Enabled = false;
           txtMRN.Enabled = false;
           pnlPatientStatus.Visible = false;
           rblStatus.Visible = false;
       }
       if(Request.QueryString["status"] == "InvalidMRN")
        {
                ucMessage.Visible = true;
                ucMessage.MessageText = "The specified MRN already exists. Please select a different MRN";
                ucMessage.Icon = Controls_Message.MessageType.ERROR;
        }

        if (!Page.IsPostBack)
        {
            Session["urlpath"] = Request.FilePath;
            GetStateList(); // Added by AKS as on OCT 05

            GetPreferredLanguageList(new Allscripts.ePrescribe.Data.Patient());
            
            setPaternalMaternalFields();

            string modeQuery = Convert.ToString(Request.QueryString["Mode"]);
            if (modeQuery == "Edit" || modeQuery == "ViewOnly")
            {
                //imgAddAllergy.Enabled = true; 
                btnAddAllergy.Enabled = true;//  AKS SEP 18th  2006 

                heading.InnerText = "Edit Patient";
                    //ImgPatientAdd.Visible = false;                      
                string LicenseID = Session["LICENSEID"].ToString();
                string Patientid = Session["PATIENTID"].ToString();

                base.AuditLogPatientInsert(ePrescribeSvc.AuditAction.PATIENT_DEMO_VIEW, Patientid);

                DataSet dsPatient;
                dsPatient = Allscripts.Impact.CHPatient.PatientSearchById(Patientid, LicenseID, Session["USERID"].ToString(), base.DBID);
                DataTable dtPatValue = dsPatient.Tables["Patient"];
                DataRow drPat = dtPatValue.Rows[0];

                txtLName.Text = drPat["LastName"].ToString();
                txtPaternalName.Text = drPat["PaternalName"] != DBNull.Value ? drPat["PaternalName"].ToString() : string.Empty;
                txtMaternalName.Text = drPat["MaternalName"] != DBNull.Value ? drPat["MaternalName"].ToString() : string.Empty;
                txtFName.Text = drPat["FirstName"].ToString();

                txtMName.Text = Convert.ToString(drPat["MiddleName"]).Trim();
                if (string.IsNullOrWhiteSpace(txtMName.Text))
                    txtMName.Text = Convert.ToString(drPat["MiddleInitial"]).Trim();
                
                txtAddress1.Text = drPat["Address1"].ToString();
                txtAddress2.Text = drPat["Address2"].ToString();
                txtCity.Text = drPat["City"].ToString();
                ddlMedHistory.SelectedValue = drPat["IsMedicationHistoryExcluded"].ToString();
                ddlPlanDisclosure.SelectedValue = drPat["IsHealthPlanDisclosureAllowed"].ToString().ToLower();

                Session["LastName"] = txtLName.Text;
                Session["FirstName"] = txtFName.Text;
                Session["MiddleName"] = txtMName.Text;
                Session["Address1"] = txtAddress1.Text;
                Session["Address2"] = txtAddress2.Text;
                Session["City"] = txtCity.Text;                               
                Session["MaternalName"] = txtMaternalName.Text;
                Session["MedicationHistory"] = ddlMedHistory.SelectedValue;
                Session[Constants.SessionVariables.IsPatientHealthPlanDisclosed] = ddlPlanDisclosure.SelectedValue;
                Session["PaternalName"] = txtPaternalName.Text;
                Session["MaternalName"] = txtMaternalName.Text;                

                var weight = new Allscripts.ePrescribe.Objects.Weight(drPat["WeightKg"] != DBNull.Value ? drPat["WeightKg"].ToString() : string.Empty);
                PageState["WeightKg"] = WeightOnLoad = weight.Kgs;
                txtWeightKg.Text = weight.Kgs == "" ? "0.00" : weight.Kgs;
                txtWeightLbs.Text = weight.Lbs;
                txtWeightOzs.Text = weight.Ozs;

                var height = new Allscripts.ePrescribe.Objects.Height(Convert.ToString(drPat["HeightCm"]));
                PageState["HeightCm"] = height.Cm;
                txtHeightCm.Text = string.IsNullOrEmpty(height.Cm) ? "0.0" : height.Cm;
                HeightOnLoad = txtHeightCm.Text;
                txtHeightFt.Text = height.Ft;
                txtHeightIn.Text = height.In;

                PageState[Constants.SessionVariables.OriginalHeight] = txtHeightCm.Text;
                PageState[Constants.SessionVariables.OriginalWeight] = txtWeightKg.Text;
                rblStatus.SelectedValue = drPat["StatusID"].ToString();

                if (drPat["State"] != DBNull.Value && ddlState.Items.FindByValue(drPat["State"].ToString()) != null)
                {
                    ddlState.Items.FindByValue(drPat["State"].ToString()).Selected = true;
                }

                txtZip.Text = drPat["Zip"].ToString();
                txtPhone.Text = drPat["Phone"].ToString();
                txtMobilePhone.Text = drPat["MobilePhone"].ToString();

                DateTime dateOfBirth = DateTime.MinValue;
                DateTime.TryParse(drPat["DOB"].ToString(), out dateOfBirth);
                if (dateOfBirth != DateTime.MinValue)
                {
                    try { txtDOB.SelectedDate = dateOfBirth; }
                    catch
                        { rfvDOB.Validate(); }                                         
                }

                txtEmail.Text = drPat["Email"].ToString();

                Session["Weight"] = drPat["WeightKg"].ToString();
                Session["Status"] = rblStatus.SelectedValue;
                Session["Zip"] = txtZip.Text;
                Session["Phone"] = txtPhone.Text;
                Session["MobilePhone"] = txtMobilePhone.Text;
                Session["DOB"] = drPat["DOB"].ToString();
                Session["Email"] = txtEmail.Text;
                Session["State"] = ddlState.Items.FindByValue(drPat["State"].ToString())?.Value;

                if (Request.QueryString["Mode"] == "ViewOnly" || Session["ViewPatient"] != null && bool.Parse(Session["ViewPatient"].ToString()))
                {
                    txtLName.Enabled = false;
                    txtPaternalName.Enabled = false;
                    txtMaternalName.Enabled = false;
                    txtFName.Enabled = false;
                    txtMName.Enabled = false;
                    txtDOB.Enabled = false;
                    txtMRN.Enabled = false;
                    txtAddress1.Enabled = false;
                    txtAddress2.Enabled = false;
                    txtCity.Enabled = false;
                    txtZip.Enabled = false;
                    txtPhone.Enabled = false;
                    txtMobilePhone.Enabled = false;
                    txtEmail.Enabled = false;
                    rblStatus.Enabled = false;
                    DDLGender.Enabled = false;
                    ddlState.Enabled = false;
                    ddlMedHistory.Enabled = false;
                    ddlPlanDisclosure.Enabled = false;
                    txtWeightLbs.Enabled = txtWeightOzs.Enabled = txtWeightKg.Enabled = false;
                    ddlPatientPreferredLang.Enabled = false;
                    txtHeightFt.Enabled = false;
                    txtHeightCm.Enabled = false;
                    txtHeightIn.Enabled = false;
                    }

                    if (Session["LASTPHARMACYNAME"] != null && Session["LASTPHARMACYNAME"].ToString().Trim() != string.Empty)
                {
                    lblPharmacy.Text = Session["LASTPHARMACYNAME"].ToString();
                }

                if (drPat["SEX"] != DBNull.Value && DDLGender.Items.FindByValue(drPat["SEX"].ToString().Trim().ToUpper()) != null)
                {
                    DDLGender.Items.FindByValue(drPat["SEX"].ToString().Trim().ToUpper()).Selected = true;
                }

                txtMRN.Text = drPat["ChartID"].ToString();

                Session["LASTPHARMACYNAME"] = lblPharmacy.Text;
                Session["ChartId"] = txtMRN.Text;
                Session["Gender"] = DDLGender.Items.FindByValue(drPat["SEX"].ToString().Trim().ToUpper()).Value;

                btnSaveAndPrescribe.Text = "Save & Prescribe";

                if (Session["ISPROVIDER"] != null && Convert.ToBoolean(Session["ISPROVIDER"]))
                {
                    btnSaveAndPrescribe.Visible = true;
                }

                btnEditSave.Visible = true;
                btnPatientAdd.Visible = false;

                gridCoverages.Visible = true;
                gridCoverages.DataBind();                    

                if (Request.QueryString["Msg"] != null)
                {
                    ucMessage.Visible = true;
                    ucMessage.MessageText = HttpUtility.UrlDecode(Request.QueryString["Msg"].ToString());
                    ucMessage.Icon = Controls_Message.MessageType.SUCCESS;
                }

                ddlPatientPreferredLang.Items.FindByText(drPat["LanguageName"].ToString()).Selected = true;
                Session["LanguageName"] = ddlPatientPreferredLang.Items.FindByValue("eng").Value;
                
                //View only screen should look like Edit screen with a few tweaks
                if(Request.QueryString["Mode"] == "ViewOnly")
                    {
                        btnAddAllergy.Visible = false;
                        btnEditSave.Visible = false;
                        btnAddAllergy.Visible = false;
                        btnAmendments.Visible = false;
                        btnSaveAndPrescribe.Visible = false;
                        btChangeInsurance.Visible = false;
                        lnkEditPharmacy.Visible = false;
                        heading.InnerText = "Patient Demographics";
                    }
            }
            else
            {
                btnAmendments.Visible = false;
                heading.InnerText = "Add New Patient";

                pnlPatientStatus.Visible = false;      
                rblStatus.Visible = false;

                //Set the default state as the site's practice state. 
                string practiceState = PageState.GetStringOrEmpty("PRACTICESTATE");                

                if (!string.IsNullOrEmpty(practiceState))
                {
                    ListItem stateItem = ddlState.Items.FindByValue(practiceState);

                    if (stateItem != null)
                    {
                        stateItem.Selected = true;
                    }
                }
                
                if (ddlPatientPreferredLang.Items.Count > 0)
                ddlPatientPreferredLang.Items.FindByValue("eng").Selected = true;

               if (Session["ISPROVIDER"] != null && Convert.ToBoolean(Session["ISPROVIDER"]))              
               {
                    btnSaveAndPrescribe.Visible = false;
               }

                btnEditSave.Visible = false;
                btnPatientAdd.Visible = true;
                base.ClearPatientInfo();
                }

            foreach (GridItem item in gridCoverages.Items)
            {
                if (item.CanExpand)
                {
                    item.Expanded = true;
                }
            }
            //if(eRxWeb.Helper.IsAngularMode == true && IsFromIframe)
            //    ClientScript.RegisterClientScriptBlock(this.GetType(), "", $"$(document).ready(function(){{RegisterBackButton('{btnBack.ClientID}');}});",true);
        }
    }
        
    public void GetPreferredLanguageList(IPatient patientDataAccess)
    {
        var langList = Allscripts.Impact.Patient.GetPrefferedLanguageList(patientDataAccess, DBID);
        ddlPatientPreferredLang.DataSource = langList;      
        ddlPatientPreferredLang.DataTextField = "LanguageName";
        ddlPatientPreferredLang.DataValueField = "LanguageValue";
        ddlPatientPreferredLang.DataBind();
    }

    protected void Page_LoadComplete(object sender, EventArgs e)
    {
        ((PhysicianMasterPageBlank)Master).hideTabs();
    }

    private void setPaternalMaternalFields()
    {
        bool isPaternalMaternalState = (Session["PRACTICESTATE"] != null && Session["PRACTICESTATE"].ToString() == "PR");
       
        pnlMaternalPaternal.Visible = isPaternalMaternalState;
        rfvPaternalName.Enabled = isPaternalMaternalState;
        revPaternalName.Enabled = isPaternalMaternalState;
        revMaternalName.Enabled = isPaternalMaternalState;
        txtLName.Enabled = !isPaternalMaternalState;
        rfvLastName.Enabled = !isPaternalMaternalState;
        revLastName.Enabled = !isPaternalMaternalState;
        lblLastNameAsterisk.Visible = !isPaternalMaternalState;
    }

    protected void GetStateList()
    {
        DataTable dtLisState = RxUser.ChGetState(base.DBID);
        ddlState.DataSource = dtLisState;
        ddlState.DataTextField = "State";
        ddlState.DataValueField = "State";
        ddlState.DataBind();
    }    

    protected void btnEditSave_Click(object sender, EventArgs e)
    {
        savePatient();
        if (isPatientSaved)
        {
            if (Convert.ToBoolean(isFromAngularModal.Value))
            {
                string patientId = PageState.GetStringOrEmpty(Constants.SessionVariables.PatientId);
                ClientScript.RegisterClientScriptBlock(this.GetType(), "",string.Format("window.parent.SaveEditedPatient(true, '{0}');", patientId), true);
            }
            else
                DefaultRedirect();
        }
        else
        {
            ucMessage.Visible = true;
            ucMessage.MessageText = "The specified MRN already exists. Please select a different MRN";
            ucMessage.Icon = Controls_Message.MessageType.ERROR;
        }
    }
        public bool IsPatientConsentAudit(bool auditPatientConsent, bool? isPatientHistoryExcluded,bool? isHealthPlanDisclosable, string patientID)
        {
            if ((Convert.ToBoolean(Session["MedicationHistory"])!= isPatientHistoryExcluded && Convert.ToBoolean(Session["MedicationHistory"]) == false && isPatientHistoryExcluded == true))
            {
                auditPatientConsent = true;
                base.AuditLogPatientInsert(ePrescribeSvc.AuditAction.PATIENT_CONSENT_MODIFIED_DO_NOT_ALLOW_MEDICATION_HISTORY, patientID);                
            }
            if ((Convert.ToBoolean(Session["MedicationHistory"]) != isPatientHistoryExcluded && Convert.ToBoolean(Session["MedicationHistory"]) == true && isPatientHistoryExcluded == false))
            {
                auditPatientConsent = true;
                base.AuditLogPatientInsert(ePrescribeSvc.AuditAction.PATIENT_CONSENT_MODIFIED_ALLOW_MEDICATION_HISTORY, patientID);
            }
            if ((Convert.ToBoolean(Session[Constants.SessionVariables.IsPatientHealthPlanDisclosed]) != isHealthPlanDisclosable && Convert.ToBoolean(Session["HealthPlan"]) == false && isHealthPlanDisclosable == true))
            {
                auditPatientConsent = true;
                base.AuditLogPatientInsert(ePrescribeSvc.AuditAction.PATIENT_CONSENT_MODIFIED_ALLOW_DISCLOSURES_TO_HEALTH_PLAN, patientID);
            }
            if ((Convert.ToBoolean(Session[Constants.SessionVariables.IsPatientHealthPlanDisclosed]) != isHealthPlanDisclosable && Convert.ToBoolean(Session[Constants.SessionVariables.IsPatientHealthPlanDisclosed]) == true && isHealthPlanDisclosable == false))
            {
                auditPatientConsent = true;
                base.AuditLogPatientInsert(ePrescribeSvc.AuditAction.PATIENT_CONSENT_MODIFIED_DO_NOT_ALLOW_DISCLOSURES_TO_HEALTH_PLAN, patientID);
            }
            return auditPatientConsent;
        }
    private void savePatient()
    {
        Page.Validate();
            if (Page.IsValid)
            {                
                bool auditPatientConsent = false;
                bool auditPatientDemoModify = false;
                string licenseID = Session["LICENSEID"].ToString();
                string userId = Session["USERID"].ToString();
                string patientID = Session["PATIENTID"].ToString();

                DataSet dsPatient;
                dsPatient = Allscripts.Impact.CHPatient.PatientSearchById(patientID, licenseID, Session["USERID"].ToString(), base.DBID);
                DataTable dtPatValue = dsPatient.Tables["Patient"];
                DataRow drPat = dtPatValue.Rows[0];

                string chartID = this.MRN; //**            

                string lastName = txtLName.Text.Trim();
                string firstName = txtFName.Text.Trim();
                string middleName = this.MiddleName;
                string address1 = txtAddress1.Text.Trim();
                string address2 = txtAddress2.Text.Trim();
                string city = txtCity.Text.Trim();
                string state = ddlState.SelectedValue.ToString();

                string zip = txtZip.Text.Trim();
                string phone = txtPhone.Text.Trim();
                string mobilePhone = txtMobilePhone.Text.Trim();
                bool? isPatientHistoryExcluded = Convert.ToBoolean(ddlMedHistory.SelectedValue);
                bool? isHealthPlanDisclosable = Convert.ToBoolean(ddlPlanDisclosure.SelectedValue);

                if ((Convert.ToBoolean(Session["MedicationHistory"]) != isPatientHistoryExcluded) || (Convert.ToBoolean(Session[Constants.SessionVariables.IsPatientHealthPlanDisclosed]) != isHealthPlanDisclosable))
                {
                    auditPatientConsent = true;
                }

                DateTime dateOfBirth = (DateTime)txtDOB.SelectedDate;
                string dob = dateOfBirth.Date.ToString("MM/dd/yyyy");

                string account = string.Empty;

                string sex = DDLGender.SelectedValue.ToString(); 

                int active = 1;

                try
                {
                    active = int.Parse(rblStatus.SelectedValue);
                }
                catch { active = 1; }

                string notes = string.Empty;

                string ext_PMS_Code = string.Empty;
                string comments = string.Empty;

                string email = txtEmail.Text.Trim();

                string paternalName = string.Empty;
                string maternalName = string.Empty;

                if (pnlMaternalPaternal.Visible)
                {
                    paternalName = txtPaternalName.Text.Trim();
                    maternalName = !string.IsNullOrEmpty(txtMaternalName.Text.Trim()) ? txtMaternalName.Text.Trim() : string.Empty;
                    lastName = paternalName + " " + maternalName;
                }

                string weight = txtWeightKg.Text;
                string height = txtHeightCm.Text;

                WeightUtil.LogIfModified(weight, WeightOnLoad, SessionPatientID, Request.UserIpAddress(), new EPSBroker(), PageState);

                HeightUtil.LogIfModified(height, HeightOnLoad, SessionPatientID, Request.UserIpAddress(), new EPSBroker(), PageState);

                string preferredLanguageID = ddlPatientPreferredLang.SelectedValue;

                if (Session["LastName"].ToString() != lastName || Session["FirstName"].ToString() != firstName || Session["MiddleName"].ToString()!= middleName || Session["Address1"].ToString()!= address1 || Session["Address2"].ToString()!= address2||
                    Session["City"].ToString()!= city || Session["PaternalName"].ToString()!= paternalName || Session["MaternalName"].ToString()!= maternalName || Convert.ToUInt32(Session["Status"])!= active || Session["Zip"].ToString()!= zip ||
                    Session["Phone"].ToString()!= phone || Session["MobilePhone"].ToString()!= mobilePhone || Session["DOB"].ToString()!= dob || Session["Email"].ToString()!= email || Session["State"].ToString()!= state || Session["ChartId"].ToString()!= chartID ||
                    Session["Gender"].ToString()!=sex || Session["LanguageName"].ToString()!= preferredLanguageID || Session["Weight"].ToString()!= weight)
                {
                    auditPatientDemoModify = true;
                }

                isPatientSaved = EPSBroker.SavePatient(licenseID, userId, patientID, chartID, lastName, firstName, middleName, address1, address2, city, state, zip,phone, dob, account, sex, active, notes, ext_PMS_Code,
                 comments, email, paternalName, maternalName, null, null, mobilePhone, isPatientHistoryExcluded, preferredLanguageID, weight, height, isHealthPlanDisclosable, base.DBID);

                PageState[Constants.SessionVariables.OriginalWeight] = weight;
                PageState[Constants.SessionVariables.OriginalHeight] = height;
                if (auditPatientConsent && auditPatientDemoModify)
                {
                    IsPatientConsentAudit(auditPatientConsent,isPatientHistoryExcluded,isHealthPlanDisclosable,patientID);
                    base.AuditLogPatientInsert(ePrescribeSvc.AuditAction.PATIENT_DEMO_MODIFY, patientID);
                }
                else if(auditPatientDemoModify)
                {
                    base.AuditLogPatientInsert(ePrescribeSvc.AuditAction.PATIENT_DEMO_MODIFY, patientID);
                }
                else if(auditPatientConsent)
                {
                    IsPatientConsentAudit(auditPatientConsent, isPatientHistoryExcluded, isHealthPlanDisclosable, patientID);
                }
                else
                {
                    base.AuditLogPatientInsert(ePrescribeSvc.AuditAction.PATIENT_DEMO_MODIFY, patientID);
                }
                
            
            Session["PATIENTID"] = patientID;
            Session["PATIENTNAME"] = lastName + ", " + firstName + " " + middleName?.SubstringOrEmpty(0, 1);
            Session["SEX"] = sex;
            Session["PATIENTDOB"] = dob;
            Session["PATIENTEMAIL"] = email;
            Session["PATIENTMOBILEPHONE"] = mobilePhone;
            Session["PATIENTZIP"] = zip;
            Session["PATIENTMRN"] = chartID;
            Session["PATIENTSTATUS"] = active;
            ((PhysicianMasterPageBlank)Master).SetPatientInfo(patientID);
        }

    }
    private DataTable RetrievePatientAccessServiceDocs(DataTable ds, int num)
    {
        //AS 9/9/2016 11:44 AM Blocked by DXC and CDS
        //if (num != 0)
        //{
        //    DataRow drBlank = default(DataRow);             
        //    for (int i = 0; i <num; i++)
        //    {                
        //        drBlank = ds.NewRow();
        //        ds.Rows.Add(drBlank); 
        //    }
        //}
        return ds;
        
    }
    protected void btnPatientAdd_Click(object sender, EventArgs e)
    {
        if(Page.IsValid)
		    Server.Transfer(Constants.PageNames.PROCESS_PATIENT);
    }
	protected void btnAddAllergy_Click(object sender, EventArgs e)
    {
        if (Request.QueryString["Mode"] != null && Request.QueryString["Mode"] == "Edit")
        {
            savePatient();
            if(isPatientSaved)
            {
                Server.Transfer(Constants.PageNames.PATIENT_ALLERGY);
            }
            else
            {
                ucMessage.Visible = true;
                ucMessage.MessageText = "The specified MRN already exists. Please select a different MRN";
                ucMessage.Icon = Controls_Message.MessageType.ERROR;
            }
               
        }
        else
        {
            nextPageUrl = Constants.PageNames.PATIENT_ALLERGY;
            Server.Transfer(Constants.PageNames.PROCESS_PATIENT);
        }
    }

    public string LastName
	{
		get { return txtLName.Text.Trim(); }
	}
    public string PaternalName
    {
        get { return txtPaternalName.Text.Trim(); }
    }
    public string MaternalName
    {
        get { return txtMaternalName.Text.Trim(); }
    }
	public string FirstName
	{
		get { return txtFName.Text.Trim(); }
	}
	public string DOB
	{
		get { return txtDOB.Text.Replace("-00-00-00", string.Empty).Trim(); }
	}
    
	public string Address1
	{
		get { return txtAddress1.Text.Trim(); }
	}
	public string Address2
	{
		get { return txtAddress2.Text.Trim(); }
	}
	public string City
	{
		get { return txtCity.Text.Trim(); }
	}
	public string State
	{
		get { return ddlState.SelectedValue.ToString(); }
	}
	public string Zip
	{
		get { return txtZip.Text.Trim(); }
	}
	public string Phone
	{
		get { return txtPhone.Text.Trim(); }
	}
    public string MobilePhone
    {
        get { return txtMobilePhone.Text.Trim(); }
    }
	public string Gender
	{
		get { return DDLGender.SelectedValue.ToString(); }
	}
	public string Email
	{
		get { return txtEmail.Text.Trim(); }
	}
	public string NextPageUrl
	{
		get { return nextPageUrl; }
	}
	public string MRN
	{
		get { return txtMRN.Text.Trim(); }
	}
	public string MiddleName
	{
		get { return txtMName.Text.Trim(); }
	}
    public string IsPatientHistoryExcluded
    {
        get { return ddlMedHistory.SelectedValue; }
    }
    public string IsHealthPlanDisclosable
    {
        get { return ddlPlanDisclosure.SelectedValue; }
    }
    public string PreferredLanguageID
    {
        get { return ddlPatientPreferredLang.SelectedValue; }
    }
    public string Weight 
    {
        get { return txtWeightKg.Text; } 
    }
    public string Height
    {
        get { return txtHeightCm.Text; }
    }
        protected void btnChangeInsurance_Click(object sender, EventArgs e)
	{
		if (Request.QueryString["Mode"] != null && Request.QueryString["Mode"] == "Edit")
		{
			savePatient();
            if (isPatientSaved)
            {
                Server.Transfer(Constants.PageNames.CHANGE_PATIENT_INSURANCE);
            }
            else
            {
                ucMessage.Visible = true;
                ucMessage.MessageText = "The specified MRN already exists. Please select a different MRN";
                ucMessage.Icon = Controls_Message.MessageType.ERROR;
            }
		}
		else
		{
            nextPageUrl = Constants.PageNames.CHANGE_PATIENT_INSURANCE;
            Server.Transfer(Constants.PageNames.PROCESS_PATIENT);
		}
	}
	protected void InsurancePlanMultiView_Load(object sender, EventArgs e)
	{
		string sponsored = Session["SPONSORED"].ToString();

		if (sponsored.Equals("1"))
		{
			InsurancePlanMultiView.SetActiveView(SponsoredView);
		}
		else
		{
			InsurancePlanMultiView.SetActiveView(NonSponsoredView);
		}
	}
    protected void btnSaveAndPrescribe_Click(object sender, EventArgs e)
    {
        savePatient();
        if (isPatientSaved)
        {
            if (!string.Equals(rblStatus.SelectedValue, "0"))
            {
                if (Session["ISPROVIDER"] != null && Convert.ToBoolean(Session["ISPROVIDER"]))
                {
                        if (Convert.ToBoolean(isFromAngularModal.Value))
                        {
                            string patientId = PageState.GetStringOrEmpty(Constants.SessionVariables.PatientId);
                            ClientScript.RegisterClientScriptBlock(this.GetType(), "", string.Format("window.parent.ParentRedirectPatientEdit('patient-info','" + (Constants.PageNames.REDIRECT_TO_ANGULAR + "?componentName=" + Constants.PageNames.SELECT_MEDICATION) + '?' + "','{0}');", patientId), true);
                        }
                        else
                            Response.Redirect(Constants.PageNames.REDIRECT_TO_ANGULAR + "?componentName=" + Constants.PageNames.SELECT_MEDICATION);
                }
            }
            else
            {
                if (Convert.ToBoolean(isFromAngularModal.Value))
                    ClientScript.RegisterClientScriptBlock(this.GetType(), "", "  window.parent.ParentPopupClose(true, 'patient-info');", true);
                else
                    DefaultRedirect();
            }
        }
        else
        {
            ucMessage.Visible = true;
            ucMessage.MessageText = "The specified MRN already exists. Please select a different MRN";
            ucMessage.Icon = Controls_Message.MessageType.ERROR;
        }
    }  
   
	private string translateRelationshipCode(string relatshipCode)
	{
		string relationDescription = string.Empty;
		int relIndicator;
        if (relatshipCode == string.Empty)
        {
            relIndicator = -1;
        }
        else
        {
            try
            {
                relIndicator = int.Parse(relatshipCode);
            }
            catch
            {
                relIndicator = -1;
            }
        }

		if (relIndicator == 0)
			relationDescription = "Member";
		else if (relIndicator == 1)
			relationDescription = "Spouse";
		else if (relIndicator == 2)
			relationDescription = "Child";
		else if (relIndicator == 3)
			relationDescription = "Other";

		return relationDescription;
	}

    protected void coverageGridView_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            //translate the relationship code
            e.Row.Cells[4].Text = translateRelationshipCode(e.Row.Cells[4].Text.Trim());
        }
    }

    protected void lnkEditPharmacy_Click(object sender, EventArgs e)
    {
        if (Request.QueryString["Mode"] != null && Request.QueryString["Mode"] == "Edit")
        {
            savePatient();
            if(isPatientSaved)
            {
                Response.Redirect(Constants.PageNames.PHARMACY + "?Mode=Edit&From=" + Constants.PageNames.ADD_PATIENT + Server.UrlEncode("?Mode=Edit"));
            }
            else
            {
                ucMessage.Visible = true;
                ucMessage.MessageText = "The specified MRN already exists. Please select a different MRN";
                ucMessage.Icon = Controls_Message.MessageType.ERROR;
            }
        }
        else
        {
            nextPageUrl = Constants.PageNames.PHARMACY + "?Mode=Edit&From=" + Constants.PageNames.ADD_PATIENT + Server.UrlEncode("?Mode=Edit");
            Server.Transfer(Constants.PageNames.PROCESS_PATIENT);
        }
    }
    protected void gridCoverages_EditCommand(object source, GridCommandEventArgs e)
    {
        GridDataItem item = (GridDataItem)e.Item;
        string selectedCoverageID = item.OwnerTableView.DataKeyValues[item.ItemIndex]["SelectedCoverageID"].ToString();

        savePatient();
            if (isPatientSaved)
            {
                Session["SelectedPlanID"] = selectedCoverageID;
                Response.Redirect(Constants.PageNames.CHANGE_PATIENT_INSURANCE + "?Mode=EditPlan");
            }
            else
            {
                ucMessage.Visible = true;
                ucMessage.MessageText = "The specified MRN already exists. Please select a different MRN";
                ucMessage.Icon = Controls_Message.MessageType.ERROR;
            }
        }
    protected void gridCoverages_DeleteCommand(object source, GridCommandEventArgs e)
    {
        GridDataItem item = (GridDataItem)e.Item;
        Int64 coverageID = Convert.ToInt64(item.OwnerTableView.DataKeyValues[item.ItemIndex]["SelectedCoverageID"].ToString());

        Allscripts.Impact.PatientCoverage.DeletePatientCoverage(
            coverageID,
            Session["LicenseID"].ToString(),
            Session["UserID"].ToString(),
            base.DBID);

        base.AuditLogPatientInsert(ePrescribeSvc.AuditAction.PATIENT_DEMO_MODIFY, base.SessionPatientID);
    }
    protected void gridCoverages_ItemCommand(object source, GridCommandEventArgs e)
    {
        if (e.CommandName == "Expand")
        {
        }
    }
    protected void grdPatientAccessServices_ItemDataBound(object sender, GridItemEventArgs e)
    {
        if (e.Item is GridDataItem)
        {
            gridCoverages.DataBind();
        }
    }
    protected void gridCoverages_ItemDataBound(object sender, GridItemEventArgs e)
    {
        if (e.Item is GridDataItem && e.Item.OwnerID == "ctl00_ContentPlaceHolder1_gridCoverages_ctl00")
        {
            GridDataItem tempDataItem = (GridDataItem)e.Item;
            string modeQuery = Convert.ToString(Request.QueryString["Mode"]);
            
            //Turn off the Edit and Delete button if viewing from script pad
            if (modeQuery == "ViewOnly")
            {
                tempDataItem["Edit"].Visible = false;
                tempDataItem["Delete"].Visible = false;
            }

            Label name = tempDataItem.FindControl("lblPayorName") as Label;
            Label bin = tempDataItem.FindControl("lblBIN") as Label;
            Label pcn = tempDataItem.FindControl("lblPCN") as Label;

            if (tempDataItem.GetDataKeyValue("PayorName").ToString().Trim() != string.Empty)
            {             
                name.Text = "<b>Name: </b>" + tempDataItem.GetDataKeyValue("PayorName").ToString().Trim();
                name.ForeColor = System.Drawing.Color.Black;
                bin.Text = "<b>BIN: </b>" + tempDataItem.GetDataKeyValue("BIN").ToString().Trim();
                pcn.Text = "<b>PCN: </b>" + tempDataItem.GetDataKeyValue("PCN").ToString().Trim();
            }
            else
            {
                name.Text = "<b>Not Set<b>";
                name.ForeColor = System.Drawing.Color.Red;
                bin.Text = string.Empty;
                pcn.Text = string.Empty;         
            }

            Repeater repeaterCoverageInfo = tempDataItem.FindControl("repeaterCoverageInfo") as Repeater;
            List<CoverageInfo> coverageInfoCollection = new List<CoverageInfo>();

            object oPharmacyBenefit = tempDataItem.GetDataKeyValue("PharmacyBenefit");
            object oPharmacyPlanName = tempDataItem.GetDataKeyValue("PlanNamePharmacy");
            object oMOBBenefit = tempDataItem.GetDataKeyValue("MailOrderBenefit");
            object oMOBPlanName = tempDataItem.GetDataKeyValue("PlanNameMOB");
            object oMOBNABP = tempDataItem.GetDataKeyValue("mob_nabp");
            object oLTCBenefit = tempDataItem.GetDataKeyValue("LTC_Benefit");
            object oLTCPlanName = tempDataItem.GetDataKeyValue("PlanNameLTC");
            object oSpecialtyBenefit = tempDataItem.GetDataKeyValue("SpecialtyBenefit");
            object oSpecialtyPlanName = tempDataItem.GetDataKeyValue("PlanNameSpecialty");

            if (oPharmacyBenefit != null && !string.IsNullOrEmpty(oPharmacyBenefit.ToString()))
            {
                coverageInfoCollection.Add(new CoverageInfo("Retail", oPharmacyBenefit.ToString(), oPharmacyPlanName));
            }

            if (oMOBBenefit != null && !string.IsNullOrEmpty(oMOBBenefit.ToString()))
            {                
                    //if the MOB Benefit = "N" but we still have an MOB NABP, overwrite it. MOB NABP takes precendence.
                if (oMOBBenefit.ToString().Equals("N") && oMOBNABP != null && !string.IsNullOrEmpty(oMOBNABP.ToString()))
                {
                    coverageInfoCollection.Add(new CoverageInfo("Mail", "Y", oMOBPlanName));
                }
                else
                {
                    coverageInfoCollection.Add(new CoverageInfo("Mail", oMOBBenefit.ToString(), oMOBPlanName));
                }
            }
            else if (oMOBNABP != null && !string.IsNullOrEmpty(oMOBNABP.ToString()))
            {
                coverageInfoCollection.Add(new CoverageInfo("Mail", "Y", oMOBPlanName));
            }

            if (oLTCBenefit != null && !string.IsNullOrEmpty(oLTCBenefit.ToString()))
            {
                coverageInfoCollection.Add(new CoverageInfo("LTC", oLTCBenefit.ToString(), oLTCPlanName));
            }

            if (oSpecialtyBenefit != null && !string.IsNullOrEmpty(oSpecialtyBenefit.ToString()))
            {
                coverageInfoCollection.Add(new CoverageInfo("Specialty", oSpecialtyBenefit.ToString(), oSpecialtyPlanName));
            }

            repeaterCoverageInfo.DataSource = coverageInfoCollection;
            repeaterCoverageInfo.DataBind();
        }
    }
    
    protected void lnkEditEmployer_Click(object sender, EventArgs e)
    {
        Response.Redirect(Constants.PageNames.EDIT_EMPLOYERS);
    }

    protected void btnBack_Click(object sender, EventArgs e)
    {
        DefaultRedirect();
    }

    protected void btnAmendments_OnClick(object sender, EventArgs e)
    {
        Response.Redirect(Constants.PageNames.PATIENT_AMENDMENTS);
    }
}

internal class CoverageInfo
{
    public CoverageInfo(string coverageType, string coverageStatus, object coveragePlanName)
    {
        CoverageType = coverageType;

        if (coverageStatus != null || coveragePlanName != null)
        {
            if (coveragePlanName != null && !string.IsNullOrEmpty(coveragePlanName.ToString()))
            {
                CoverageDescription = coveragePlanName.ToString();
            }
            else if (!string.IsNullOrEmpty(coverageStatus))
            {
                if (coverageStatus.Equals("Y"))
                {
                    CoverageDescription = "YES";
                }
                else if (coverageStatus.Equals("N"))
                {
                    CoverageDescription = "NO";
                }
            }
        }
    }

    public string CoverageType { get; set; }
    public string CoverageDescription { get; set; }
}
    
    }