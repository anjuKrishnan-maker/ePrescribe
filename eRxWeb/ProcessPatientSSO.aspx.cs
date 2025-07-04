
/******************************************************************************
**  Change History
*******************************************************************************
**  Date:      Author:                    Description:
**----------------------------------------------------------------------------- 
**  11/09/2011  Narendra Meena      Removed SSN in edit and Add patient Process
*******************************************************************************/

#define TRACE
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
using System.Text;
using Allscripts.Impact;
using Allscripts.Impact.Utilities;
using System.Xml;
using Allscripts.ePrescribe.CCR;
using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.Objects;
using eRxWeb.AppCode;

namespace eRxWeb
{
public partial class ProcessPatientSSO : BasePage 
{
    string _errMsg = string.Empty;

    protected void Page_Init(object sender, EventArgs e)
    {
        System.Diagnostics.Trace.Write("Start Page_Init " + Constants.PageNames.PROCESS_PATIENT_SSO, Session.SessionID);
    }

	protected void Page_Load(object sender, EventArgs e)
	{
        if (!IsPostBack)
        {
            if (!base.SessionLicense.EnterpriseClient.AddDiagnosis)
            {
                btnSelectDx.Style["Display"] = "none";
            }

            System.Diagnostics.Trace.Write("Start Page_Load " + Constants.PageNames.PROCESS_PATIENT_SSO, Session.SessionID);

            if (Request.QueryString["SSOMode"] != null && Request.QueryString["SSOMode"] == "MultipleMRNMatches" && Request.QueryString["MRN"] != null)
            {
                setDuplicateMRNPatients(Session["LicenseID"].ToString(), Request.QueryString["mrn"]);
            }
            else
            {
                PopulatePatientData();
                if (ValidateFields())
                {
					if (PatientExists(_patientGUID.Value))
					{
						//Logic for Patient Update
						HandlePatientSave(); 
					}
					else
					{
						//Logic for patient Add. Only Check patient demographics if a patientGUID is not specified. If it is specified,
                        //then we then don't check demographics b/c the user could select a different patient than the partner specified
                        if (_patientGUID == null || _patientGUID.Value == string.Empty || !IsGuid(_patientGUID.Value))
                        {
                            if (checkPatientDemographics())
                            {
                                //Implement the logic here to stay :)
                                string patientName = _lastName.Value.Trim() + ", " + _firstName.Value.Trim() + " " + _middleName.Value.Trim();

                                ucMessage.Visible = true;
                                ucMessage.MessageText = "Patients with similar demographics are found in the system. Please select an existing patient or add " +
                                    patientName + " as a new patient.";
                                ucMessage.Icon = Controls_Message.MessageType.INFORMATION;

                                btnAddPatient.ToolTip = "Add " + patientName + " as a new patient";
                                if (PageState.GetBooleanOrFalse("MULTIPLESITES") && string.IsNullOrWhiteSpace(PageState["SITEID"].ToString()))
                                {
                                    PageState["SITEID"] = 1;
                                }
                            }
                            else
                            {
                                HandlePatientSave();
                            }
                        }
                        else
                        {
                            //Implement the logic here to add the patient :)
                            HandlePatientSave();
                        }
					}
                }
                else
                {
                    Allscripts.Impact.Audit.AddException(Session["USERID"].ToString(), Session["LICENSEID"].ToString(), "Exception saving patient on ProcessPatientSSO.aspx.cs: " + _errMsg, string.Empty, string.Empty, string.Empty, base.DBID);
                    throw (new ArgumentException(_errMsg));
                }
            }
        }   
	}
    protected void Page_LoadComplete(object sender, EventArgs e)
    {
        ((PhysicianMasterPageBlank)Master).hideTabs();
    }
	private bool checkPatientDemographics()
	{
        System.Diagnostics.Trace.Write("Start checkPatientDemographics()", Session.SessionID);

		bool bPatientFound = false;
        
        //Only check for similar patients if the patientGUID was not specified.
        //The partner is passing us a patientGUID b/c they have already stored it on their system. 
        //If we change the patient at this point, they will have no idea what the patientGUID is.
        if (_patientGUID == null || _patientGUID.Value == string.Empty)
        {
            DataSet patientDataSet = GetSimilarPatients();
            if (patientDataSet.Tables["Patients"].Rows.Count > 0)
            {
                bPatientFound = true;

                grdViewPatients.DataSource = patientDataSet;
                grdViewPatients.DataBind();
            }
        }

        System.Diagnostics.Trace.Write("End checkPatientDemographics()", Session.SessionID);

        return bPatientFound;
	}

    private void setDuplicateMRNPatients(string licenseID, string mrn)
    {
        ucMessage.Visible = true;
        ucMessage.MessageText = "Multiple patients with were found in the system for this MRN. Please select the correct patient below.";
        ucMessage.Icon = Controls_Message.MessageType.INFORMATION;

        btnAddPatient.Visible = false;

        grdViewPatients.DataSource = Allscripts.Impact.Patient.SearchByChartID(mrn, licenseID, base.DBID);
        grdViewPatients.DataBind();
    }

	private DataSet GetSimilarPatients()
	{
		string licenseID = Session["LICENSEID"].ToString();

		string lastName = _lastName.Value.Trim();
		string firstName = _firstName.Value.Trim();
		string address = _address1.Value.Trim();
		string zip = _zip.Value.Trim();
		DateTime dob = Convert.ToDateTime(_dob.Value.Trim());
        int matchThrashold;
        matchThrashold = 100;
		DataSet patientDataSet = Allscripts.Impact.CHPatient.SearchSimilarPatients(licenseID, firstName, lastName, address, dob, zip, matchThrashold, Session["USERID"].ToString(), false, base.DBID);

		return patientDataSet;
	}

    private void PopulatePatientData()
    {
        var patient = PageState.Cast<SsoPatient>(Constants.SessionVariables.AddPatientSso, null);
        PageState.Remove(Constants.SessionVariables.AddPatientSso);
        if (patient != null && !string.IsNullOrWhiteSpace(patient.Name.LastName))
        {
            _lastName.Value = patient.Name.LastName;

            _firstName.Value = patient.Name.FirstName ?? string.Empty;
            _middleName.Value = patient.Name.Middle ?? string.Empty;
            _dob.Value = patient.Dob ?? string.Empty;
            _mrn.Value = patient.Mrn ?? string.Empty;
            _patientGUID.Value = patient.PatientGuid ?? string.Empty;
            _address1.Value = patient.Address.Address1 ?? string.Empty;
            _address2.Value = patient.Address.Address2 ?? string.Empty;
            _city.Value = patient.Address.City ?? string.Empty;
            _state.Value = patient.Address.State ?? string.Empty;
            _zip.Value = patient.Address.ZIPCode ?? string.Empty;
            _phone.Value = patient.Phone ?? string.Empty;
            _gender.Value = patient.Gender ?? string.Empty;
            _planid.Value = patient.PlanId ?? string.Empty;
            _planname.Value = patient.PlanName ?? string.Empty;
            patientdoc.Value = patient.PatientDoc ?? string.Empty;
            _patNCPDPNo.Value = patient.NcpdpNo ?? string.Empty;
            _pharmState.Value = patient.PharmacySt ?? string.Empty;
        }
        else if (Context.Items["LastName"] != null)
        {
            object fld;
            fld = Context.Items["LastName"];
            if (fld != null)
                _lastName.Value = fld.ToString();

            fld = Context.Items["FirstName"];
            if (fld != null)
                _firstName.Value = fld.ToString();

            fld = Context.Items["MiddleName"];
            if (fld != null)
                _middleName.Value = fld.ToString();

            fld = Context.Items["DOB"];
            if (fld != null)
                _dob.Value = fld.ToString();        

            fld = Context.Items["MRN"];
            if (fld != null)
                _mrn.Value = fld.ToString();

            fld = Context.Items["PatientGUID"];
            if (fld != null)
                _patientGUID.Value = fld.ToString();

            fld = Context.Items["Address1"];
            if (fld != null)
                _address1.Value = fld.ToString();

            fld = Context.Items["Address2"];
            if (fld != null)
                _address2.Value = fld.ToString();

            fld = Context.Items["City"];
            if (fld != null)
                _city.Value = fld.ToString();

            fld = Context.Items["State"];
            if (fld != null)
                _state.Value = fld.ToString();

            fld = Context.Items["Zip"];
            if (fld != null)
                _zip.Value = fld.ToString();

            fld = Context.Items["Phone"];
            if (fld != null)
                _phone.Value = fld.ToString();

            fld = Context.Items["Gender"];
            if (fld != null)
                _gender.Value = fld.ToString();

            //fld = (string)Request.Form["Email"];
            //if (fld != null)
            //    _email.Value = fld.ToString();

            fld = Context.Items["NextPageURL"];
            if (fld != null)
                _nextPageUrl.Value = fld.ToString();

            fld = Context.Items["Planid"];
            if (fld != null)
                _planid.Value = fld.ToString();

            fld = Context.Items["Planname"];
            if (fld != null)
                _planname.Value = fld.ToString();
            fld = Context.Items["PatientDoc"];
            if (fld != null)
                patientdoc.Value = fld.ToString();

            //ExtFacilityCd currently only used for Misys EMR implementation
            fld = Context.Items["Facility"];
            if (fld != null)
                Session["ExtFacilityCd"] = fld.ToString();

            //ExtGroupID currently only used for Misys EMR implementation
            fld = Context.Items["Group"];
            if (fld != null)
                Session["ExtGroupID"] = fld.ToString();

            fld = Context.Items["NCPDPNo"];
            if (fld != null)
                _patNCPDPNo.Value = fld.ToString();

            fld = Context.Items["PharmacySt"];
            if (fld != null)
                _pharmState.Value = fld.ToString();
        }
        else
        {
            string fld;
            fld = (string)Request.Form["LastName"];

            if (fld != null)
                _lastName.Value = fld;

            fld = (string)Request.Form["FirstName"];
            if (fld != null)
                _firstName.Value = fld;

            fld = (string)Request.Form["MiddleName"];
            if (fld != null)
                _middleName.Value = fld;

            fld = (string)Request.Form["DOB"];
            if (fld != null)
                _dob.Value = fld;           

            fld = (string)Request.Form["MRN"];
            if (fld != null)
                _mrn.Value = fld;

            fld = (string)Request.Form["PatientGUID"];
            if (fld != null)
                _patientGUID.Value = fld;

            fld = (string)Request.Form["Address1"];
            if (fld != null)
                _address1.Value = fld;

            fld = (string)Request.Form["Address2"];
            if (fld != null)
                _address2.Value = fld;

            fld = (string)Request.Form["City"];
            if (fld != null)
                _city.Value = fld;

            fld = (string)Request.Form["State"];
            if (fld != null)
                _state.Value = fld;

            fld = (string)Request.Form["Zip"];
            if (fld != null)
                _zip.Value = fld;

            fld = (string)Request.Form["Phone"];
            if (fld != null)
                _phone.Value = fld;

            fld = (string)Request.Form["Gender"];
            if (fld != null)
                _gender.Value = fld;

            fld = (string)Request.Form["Email"];
            if (fld != null)
                _email.Value = fld;

            fld = (string)Request.Form["NextPageUrl"];
            if (fld != null)
                _nextPageUrl.Value = fld;

            fld = (string)Request.Form["Planid"];
            if (fld != null)
                _planid.Value = fld;

            fld = (string)Request.Form["Planname"];
            if (fld != null)
                _planname.Value = fld;
            fld = (string)Request.Form["patientdata"];
            if (fld != null)
                patientdoc.Value = fld;

            //ExtFacilityCd currently only used for Misys EMR implementation
            fld = (string)Request.Form["ExtFacilityCd"];
            if (fld != null)
                Session["ExtFacilityCd"] = fld;

            //ExtGroupID currently only used for Misys EMR implementation
            fld = (string)Request.Form["ExtGroupID"];
            if (fld != null)
                Session["ExtGroupID"] = fld;

            fld = (string)Request.Form["NCPDPNo"];
            if (fld != null)
                _patNCPDPNo.Value = fld;

            fld = (string)Request.Form["PharmacySt"];
            if (fld != null)
                _pharmState.Value = fld;
        }
    }

    public bool IsNumeric(string value)
    {
        try
        {
            int.Parse(value);
            return (true);
        }
        catch
        {
            return (false);
        }
    }
    public bool IsDate(string value)
    {
        try
        {
            DateTime.Parse(value);
            return (true);
        }
        catch
        {
            return (false);
        }
    }

	private bool PatientExists(string patientID)
	{
		bool bExists = false;

		if (!string.IsNullOrEmpty(patientID) && IsGuid(patientID))
		{
			DataSet patientDS = CHPatient.PatientSearchById(patientID, Session["LicenseID"].ToString(), Session["UserID"].ToString(), base.DBID);
			if (patientDS.Tables[0].Rows.Count == 1)
			{
				//Patient is found via the given id. 
				bExists = true;
			}
		}

		return bExists; 
	}

    private bool ValidateFields()
    {
        if (_lastName.Value.Length == 0)
            _errMsg = "Lastname is required";
        if (_firstName.Value.Length == 0)
            _errMsg += (_errMsg.Length > 0 ? ", " : string.Empty) + "Firstname is required";
        if (_dob.Value.Length == 0)
        {
            _errMsg += (_errMsg.Length > 0 ? ", " : string.Empty) + "DOB is required";
        }
        else
        {
            if (!IsDate(_dob.Value))
            {
                _dob.Value = _dob.Value.Substring(0, 4) + '-' + _dob.Value.Substring(4, 2) + '-' + _dob.Value.Substring(6, 2);
                if (!IsDate(_dob.Value))
                {
                    _errMsg += (_errMsg.Length > 0 ? ", " : string.Empty) + "DOB is invalid";
                }
            }
        }
        if (_gender.Value.Length == 0)
        {
            _errMsg += (_errMsg.Length > 0 ? ", " : string.Empty) + "Gender is required";
        }
        else
        {
            switch (_gender.Value.Trim())
            {
                case "M":
                case "F":
                case "U":
                case "UN":
                    break;
                default:
                    _errMsg += (_errMsg.Length > 0 ? ", " : string.Empty) + "Gender is invalid, only 'M', 'F', 'U', or 'UN' are valid.";
                    break;
            }
        }
        if (_zip.Value.Length == 0)
            _errMsg += (_errMsg.Length > 0 ? ", " : string.Empty) + "Zipcode is required";

        if (_errMsg.Length > 0)
        {
            _errMsg += ".";
            return false;
        }
        else
            return true;
    }

	private void HandlePatientSave()
	{
		//Clear out the patient session information first
		base.ClearPatientInfo();

		//Add the new patient
		SavePatient();

		string nextPageUrl = _nextPageUrl.Value.Trim();

		//If the previous page already set the next page destination, then redirect to it. 
		//Otherwise, redirect to the correct default page.
		if (!string.IsNullOrEmpty(nextPageUrl))
		{
			Response.Redirect(Constants.PageNames.UrlForRedirection(nextPageUrl));
		}
		else
		{
            DefaultRedirect();
		}
	}

    private bool IsGuid(string guid)
    {
        bool returnValue;
        try
        {
            Guid temp = new Guid(guid);
            returnValue = true;
        }
        catch
        {
            returnValue = false;
        }
        return returnValue;
    }

    private void SavePatient()
    {
        System.Diagnostics.Trace.Write("Start SavePatient()", Session.SessionID);

        string licenseID = Session["LICENSEID"].ToString();  // GET IT FROM THE SESSION.
        string userId = Session["USERID"].ToString();
        string patientID = string.Empty;

        //check if the partner passed in a patient GUID. if so, use it. if not, create a new one.
        if (_patientGUID == null || _patientGUID.Value == string.Empty && !IsGuid(_patientGUID.Value))
            patientID = System.Guid.NewGuid().ToString();
        else
            patientID = _patientGUID.Value;

        string chartID = _mrn.Value.Trim(); //** 
        string lastName = _lastName.Value.Trim();
        string firstName = _firstName.Value.Trim();
        string middleName = _middleName.Value.Trim();   //**
        string address1 = _address1.Value.Trim();
        string address2 = _address2.Value.Trim();
        string city = _city.Value.Trim();
        //string state = txtState.Text.Trim(); //Added by AKS on Oct 05
        string state = _state.Value.Trim();
        string zip = _zip.Value.Trim();
        string phone = _phone.Value.Trim();
        string dob = _dob.Value.Trim();
        string account = string.Empty;

        if (_gender.Value == "UN")
            _gender.Value = "U";

        string sex = _gender.Value.Trim();
        int active = 1;
        string notes = string.Empty;
        string apptNumber = string.Empty;
        string apptTime = string.Empty;
        string ext_PMS_Code = string.Empty;
        string duration = string.Empty;
        string comments = string.Empty;
        string email = _email.Value.Trim();
        string patPharm = _patNCPDPNo.Value.Trim();
        string patPharmState = _pharmState.Value.Trim();
        string mobilePhone = _mobilephone.Value.Trim();

        //create Instance  of the chPatient Get all the values/
        EPSBroker.SavePatient(licenseID, userId, patientID, chartID, 
            lastName, firstName, middleName, address1, address2, city, state, zip,
            phone, dob, account, sex, active, notes, ext_PMS_Code, comments, email,
            null, null, patPharm, patPharmState, mobilePhone, null, string.Empty, string.Empty, string.Empty, true, base.DBID);

        /* Adding these into the session variable for the entire application */
        Session["PATIENTID"] = patientID;
        Session["PATIENTNAME"] = lastName + ", " + firstName + " " + middleName;
        Session["SEX"] = sex;
        Session["PATIENTDOB"] = dob;
        Session["PATIENTZIP"] = zip;
        Session["PATIENTMRN"] = chartID;
        ((PhysicianMasterPageBlank)Master).SetPatientInfo(patientID);
        if  (!string.IsNullOrEmpty(_planid.Value))
        {
            Allscripts.Impact.PatientCoverage.AddPatientCoverage(0, Session["PATIENTID"].ToString(), Session["LICENSEID"].ToString(), Session["USERID"].ToString(), _planid.Value, _planname.Value, string.Empty, string.Empty, string.Empty, string.Empty, base.DBID);
        }

        if ((Session["SharedHealth"] == null) && !string.IsNullOrEmpty(patientdoc.Value))
        {
            System.Diagnostics.Trace.Write("Start CCR processing", Session.SessionID);

            XmlDocument doc = new XmlDocument();

            doc.LoadXml(patientdoc.Value);

            CCRDocument ccrDoc = new CCRDocument(doc);
            CCRDocumentProcessor ccrDocProc = new CCRDocumentProcessor(ccrDoc, patientID, Session["LICENSEID"].ToString());
            ccrDocProc.ProcessCCR(Session.SessionID, base.DBID);

            System.Diagnostics.Trace.Write("End CCR processing", Session.SessionID);
        }

        //Add the new patient to the schedule
        Allscripts.Impact.Patient.AddPatientToSchedule(Session["LICENSEID"].ToString(), Session["USERID"].ToString(), Session["PATIENTID"].ToString(), DateTime.Now, string.Empty, string.Empty, string.Empty, Session["USERID"].ToString(), base.DBID);

        System.Diagnostics.Trace.Write("End SavePatient()", Session.SessionID);
    }
    
	protected void grdViewPatients_RowDataBound(object sender, GridViewRowEventArgs e)
	{
		if (e.Row.RowType == DataControlRowType.DataRow)
		{
			e.Row.Style["cursor"] = "pointer";
			e.Row.Attributes.Add("onclick", "onRowClick(this)");
		}
	}
	protected void btnEditPatient_Click(object sender, EventArgs e)
	{
		SetPatientInfo(false);
		HandleServerTransfer(Constants.PageNames.ADD_PATIENT + "?Mode=Edit");
	}
	protected void btnAddPatient_Click(object sender, EventArgs e)
	{
		HandlePatientSave();
	}
	protected void btnReviewHistory_Click(object sender, EventArgs e)
	{
		SetPatientInfo(true);
        Response.Redirect(AngularStringUtil.CreateUrl(Constants.PageNames.REVIEW_HISTORY));
	}
	protected void btnSelectDx_Click(object sender, EventArgs e)
	{
		SetPatientInfo(true);
        Response.Redirect(Constants.PageNames.SELECT_DX);
	}
	protected void btnSelectMed_Click(object sender, EventArgs e)
	{
		SetPatientInfo(true);
        Response.Redirect(AngularStringUtil.CreateUrl(Constants.PageNames.SELECT_MEDICATION));
	}
	protected void btnFavorite_Click(object sender, EventArgs e)
	{
		SetPatientInfo(true);
	    Response.Redirect(AngularStringUtil.CreateUrl(Constants.PageNames.SELECT_MEDICATION));
    }
   	private void SetPatientInfo(bool fullSelect)
	{
		string selectedValue = Request.Form["rdSelectRow"];
		foreach (GridViewRow row in grdViewPatients.Rows)
		{
			if (grdViewPatients.DataKeys[row.RowIndex].Value.ToString() == selectedValue)
			{
				grdViewPatients.SelectedIndex = row.RowIndex;
				break;
			}
		}

		if (grdViewPatients.SelectedDataKey != null)
		{
			string patientID = grdViewPatients.SelectedDataKey.Value.ToString();

			((PhysicianMasterPageBlank)Master).SetPatientInfo(patientID);
			//DHIRAJ iSSUE nO :51
            //Allscripts.Impact.Prescription.CompleteTherapy(patientID);

			if (fullSelect)
			{
				//EAK added Realtime EligibilityChecking
				Allscripts.Impact.Patient.AddPatientEligibilityRequest(Session["LICENSEID"].ToString(), Session["USERID"].ToString(), Session["PATIENTID"].ToString(), null, Session["USERID"].ToString(), base.DBID);
				//EAK added inserting Patient into schedule
                Allscripts.Impact.Patient.AddPatientToSchedule(Session["LICENSEID"].ToString(), Session["USERID"].ToString(), Session["PATIENTID"].ToString(), DateTime.Now, string.Empty, string.Empty, string.Empty, Session["USERID"].ToString(), base.DBID);
			}
		}
	}
	protected void ControlMultiView_Load(object sender, EventArgs e)
	{
		if (Session["ISPROVIDER"] != null && Convert.ToBoolean(Session["ISPROVIDER"]))
		{
			ControlMultiView.SetActiveView(ProviderControlView);
		}
		else
		{
			ControlMultiView.SetActiveView(NurseControlView);
		}
	}
	protected void grdViewPatients_PageIndexChanging(object sender, GridViewPageEventArgs arg)
	{
		grdViewPatients.DataSource = GetSimilarPatients();
		grdViewPatients.PageIndex = arg.NewPageIndex;
		grdViewPatients.DataBind();
	}
	protected void btnProviderCancel_Click(object sender, EventArgs e)
	{
        HandleServerTransfer(Constants.PageNames.SELECT_PATIENT);
	}
	protected void btnNurseCancel_Click(object sender, EventArgs e)
	{
        switch ((Constants.UserCategory)Session["UserType"])
        {
            case Constants.UserCategory.PHYSICIAN_ASSISTANT:
                HandleServerTransfer(Constants.PageNames.SELECT_PATIENT);
                break;
            default:
                HandleServerTransfer(Constants.PageNames.SELECT_PATIENT);
                break;
        }
	}

	private void HandleServerTransfer(string pageUrl)
	{
		if (Session["PAGEHEIGHT"] == null)
		{
			//If the page height is not set, then redirect to set height page. 
			Server.Transfer(string.Format(Constants.PageNames.SET_HEIGHT + "?dest={0}", pageUrl));
		}
		else
		{
			Server.Transfer(pageUrl); 
		}
	}
}

}