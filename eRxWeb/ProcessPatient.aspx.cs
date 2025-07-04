
/******************************************************************************
**  Change History
*******************************************************************************
**  Date:      Author:                    Description:
**----------------------------------------------------------------------------- 
**  11/09/2011  Narendra Meena      Removed SSN in edit and Add patient Process
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
using System.Text;
using Allscripts.Impact;
using Allscripts.Impact.Utilities;
using Allscripts.ePrescribe.Common;
using eRxWeb.ServerModel;
using System.Web.Script.Serialization;
using eRxWeb.AppCode;

namespace eRxWeb
{
public partial class ProcessPatient : BasePage 
{
    private bool isDuplicateChartID;
    string nextPageUrl = string.Empty;


    protected void Page_Load(object sender, EventArgs e)
	{
        if (PreviousPage != null)
		{
			PatientDemographics patientDemographics = populatePatientData();
			if (checkPatientDemographics())
			{
			    //Implement the logic here to stay :)
			    string patientName = _lastName.Value.Trim() + ", " + _firstName.Value.Trim() + " " + _middleName.Value.Trim();

			    ucMessage.Visible = true;
			    ucMessage.MessageText = "Patients with similar demographics are found in the system. Please select an existing patient or add " +
			                            patientName + " as a new patient.";
			    ucMessage.Icon = Controls_Message.MessageType.INFORMATION;
			    btnAddPatient.ToolTip = "Add " + patientName + " as a new patient";
            }
            else
			{
				//Implement the logic here to add the patient :)
				handlePatientAdd();
			}
		}

        if (!IsPostBack)
        {
            if (!base.SessionLicense.EnterpriseClient.AddDiagnosis)
            {
                btnSelectDx.Style["Display"] = "none";
            }
        }
     }
    protected void Page_LoadComplete(object sender, EventArgs e)
    {
        ((PhysicianMasterPageBlank)Master).hideTabs();
    }
	private bool checkPatientDemographics()
	{
		bool bPatientFound = false;

		DataSet patientDataSet = getSimilarPatients();
		if (patientDataSet.Tables["Patients"].Rows.Count > 0)
		{
			bPatientFound = true;

			grdViewPatients.DataSource = patientDataSet;
			grdViewPatients.DataBind();
		}
		return bPatientFound;
	}

	private DataSet getSimilarPatients()
	{
		string licenseID = Session["LICENSEID"].ToString();

		string lastName = _lastName.Value.Trim();
		string firstName = _firstName.Value.Trim();
		string address = _address1.Value.Trim();
		string zip = _zip.Value.Trim();
		DateTime dob = Convert.ToDateTime(_dob.Value.Trim());

		int matchThrashold = 100; //Highest score is 240 
		DataSet patientDataSet = Allscripts.Impact.CHPatient.SearchSimilarPatients(licenseID, firstName, lastName, address, dob, zip, matchThrashold, Session["USERID"].ToString(), false, base.DBID);

		return patientDataSet;
	}

	private PatientDemographics populatePatientData()
	{
        PatientDemographics patientDemographics = new PatientDemographics();
        if (Session["PRACTICESTATE"] != null && Session["PRACTICESTATE"].ToString() == "PR")
    		_lastName.Value = patientDemographics.LastName = PreviousPage.PaternalName + " " + PreviousPage.MaternalName;
        else
    		_lastName.Value = patientDemographics.LastName = PreviousPage.LastName;

        _paternalName.Value = patientDemographics.PaternalName = PreviousPage.PaternalName;
        _maternalName.Value = patientDemographics.MaternalName = PreviousPage.MaternalName;
		_firstName.Value = patientDemographics.FirstName = PreviousPage.FirstName;
		_middleName.Value = patientDemographics.MiddleName = PreviousPage.MiddleName;
		_dob.Value = patientDemographics.Dob = PreviousPage.DOB;		
		_mrn.Value = patientDemographics.Mrn = PreviousPage.MRN;
		_address1.Value = patientDemographics.Address1 = PreviousPage.Address1;
		_address2.Value = patientDemographics.Address2 = PreviousPage.Address2;
		_city.Value = patientDemographics.City = PreviousPage.City;
		_state.Value = patientDemographics.State = PreviousPage.State;
		_zip.Value = patientDemographics.Zip = PreviousPage.Zip;
        _phone.Value = patientDemographics.Phone = PreviousPage.Phone;
        _mobilePhone.Value = patientDemographics.MobilePhone = PreviousPage.MobilePhone;
		_gender.Value = patientDemographics.Gender = PreviousPage.Gender;
		_email.Value = patientDemographics.Email = PreviousPage.Email;
        _isPatientHistoryExcluded.Value = patientDemographics.IsPatientHistoryExcluded = PreviousPage.IsPatientHistoryExcluded;
        _preferredLanguageID.Value = patientDemographics.PreferredLanguageID = PreviousPage.PreferredLanguageID;
        _weight.Value = patientDemographics.Weight = PreviousPage.Weight;
        _height.Value = patientDemographics.Height = PreviousPage.Height;
        _isHealthPlanDisclosable.Value = patientDemographics.IsHealthPlanDisclosable = PreviousPage.IsHealthPlanDisclosable;

        //Only set this parameter if the previous page has set the nextPageUrl
            if (!string.IsNullOrEmpty(PreviousPage.NextPageUrl))
		{
			_nextPageUrl.Value = PreviousPage.NextPageUrl;
		}

	    return patientDemographics;

	}

	private void handlePatientAdd()
	{
		//Clear out the patient session information first
		base.ClearPatientInfo();

		//Add the new patient
		addNewPatient();

        if (nextPageUrl.Length == 0)
        {
            nextPageUrl = _nextPageUrl.Value.Trim();
        }
		//If the previous page already set the next page destination, then redirect to it. 
		//Otherwise, redirect to the correct default page.
		if (!string.IsNullOrEmpty(nextPageUrl))
		{
			Response.Redirect(nextPageUrl);
		}
		else
		{
            DefaultRedirect();
		}
	}

	private void addNewPatient()
	{
		string licenseID = Session["LICENSEID"].ToString();
		string userId = Session["USERID"].ToString();
		string patientID = System.Guid.NewGuid().ToString();
		string chartID = _mrn.Value.Trim();
        string lastName = _lastName.Value.Trim();
        string paternalName = _paternalName.Value.Trim();
        string maternalName = _maternalName.Value.Trim();
		string firstName = _firstName.Value.Trim();
		string middleName = _middleName.Value.Trim();
		string address1 = _address1.Value.Trim();
		string address2 = _address2.Value.Trim();
		string city = _city.Value.Trim();
		string state = _state.Value.Trim();
		string zip = _zip.Value.Trim();
		string phone = _phone.Value.Trim();
		string dob = _dob.Value.Trim();
		string account = string.Empty;
        string mobilePhone = _mobilePhone.Value.Trim();
		string sex = _gender.Value.Trim();
        bool isPatientHistoryExcluded = Convert.ToBoolean(_isPatientHistoryExcluded.Value);
        string preferredLanguage = _preferredLanguageID.Value;
        string weight = _weight.Value;
        string height = (string.IsNullOrEmpty(_height.Value) || _height.Value == "0" || _height.Value == "0.0") ? "0" : _height.Value;
        bool isHealthPlanDisclosable = Convert.ToBoolean(_isHealthPlanDisclosable.Value);


        int active = 1; // active=1, inactive=0, test=99**
        if (Session["PATIENTSTATUS"] != null && Session["PATIENTSTATUS"].ToString() != string.Empty)
        {
            active = Convert.ToInt32(Session["PATIENTSTATUS"].ToString());
        }
        string notes = string.Empty;
		string ext_PMS_Code = string.Empty;
		string comments = string.Empty; 
		string email = _email.Value.Trim();

        isDuplicateChartID = !EPSBroker.SavePatient(licenseID, userId, patientID, chartID, 
			lastName, firstName, middleName, address1, address2, city, state, zip,
			phone, dob, account, sex, active, notes, ext_PMS_Code, comments, email,
            paternalName, maternalName, null, null, mobilePhone, isPatientHistoryExcluded, preferredLanguage, weight, height, isHealthPlanDisclosable, base.DBID);
        if (!isDuplicateChartID)
        {
            /* Adding these into the session variable for the entire application */
            Session["PATIENTID"] = patientID;
            Session["PATIENTNAME"] = lastName + ", " + firstName + " " + middleName;
            Session["SEX"] = sex;
            Session["PATIENTDOB"] = dob;
            Session["PATIENTZIP"] = zip;
            Session["PATIENTMRN"] = chartID;
            Session["PatientActive"] = true;
       
            base.AuditLogPatientInsert(ePrescribeSvc.AuditAction.PATIENT_ADD, patientID);
        }
        else
        {
            nextPageUrl = Constants.PageNames.ADD_PATIENT + "?status=InvalidMRN";
        }
        

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
		Server.Transfer(Constants.PageNames.ADD_PATIENT + "?Mode=Edit");
	}
	protected void btnAddPatient_Click(object sender, EventArgs e)
	{
		handlePatientAdd();
	}
	protected void btnReviewHistory_Click(object sender, EventArgs e)
	{
		SetPatientInfo(true);
        string componentName = Allscripts.ePrescribe.Common.Constants.PageNames.REVIEW_HISTORY;
        string componentParameters = string.Empty;
            Response.Redirect(Constants.PageNames.REDIRECT_TO_ANGULAR + "?componentName=" + componentName + "&componentParameters=" + componentParameters);
        }
	protected void btnSelectDx_Click(object sender, EventArgs e)
	{
		SetPatientInfo(true);
        Server.Transfer(Constants.PageNames.SELECT_DX);
	}
	protected void btnSelectMed_Click(object sender, EventArgs e)
	{
		SetPatientInfo(true);
        Server.Transfer(Constants.PageNames.REDIRECT_TO_ANGULAR + "?componentName=" + Constants.PageNames.SELECT_MEDICATION);
	}
	protected void btnFavorite_Click(object sender, EventArgs e)
	{
		SetPatientInfo(true);
        Server.Transfer(Constants.PageNames.REDIRECT_TO_ANGULAR + "?componentName=" + Constants.PageNames.SELECT_MEDICATION);

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

			if (fullSelect)
			{
				//EAK added Realtime EligibilityChecking
				Patient.AddPatientEligibilityRequest(base.SessionLicenseID, base.SessionUserID, base.SessionPatientID, null, base.SessionUserID, base.DBID);
				//EAK added inserting Patient into schedule
                Patient.AddPatientToSchedule(base.SessionLicenseID, base.SessionUserID, base.SessionPatientID, DateTime.Now, string.Empty, string.Empty, string.Empty, base.SessionUserID, base.DBID);
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
		grdViewPatients.DataSource = getSimilarPatients();
		grdViewPatients.PageIndex = arg.NewPageIndex;
		grdViewPatients.DataBind();
	}
	protected void btnProviderCancel_Click(object sender, EventArgs e)
	{
        SelectPatientComponentParameters selectPatientComponentParameters = new SelectPatientComponentParameters
        {
            PatientId = PageState.GetStringOrEmpty(Constants.SessionVariables.PatientId)
        };
	    RedirectToSelectPatient(null, selectPatientComponentParameters);
    }
	protected void btnNurseCancel_Click(object sender, EventArgs e)
	{
        switch ((Constants.UserCategory)Session["UserType"])
        {
            case Constants.UserCategory.PHYSICIAN_ASSISTANT:
                    {
                        SelectPatientComponentParameters selectPatientComponentParameters = new SelectPatientComponentParameters
                        {
                            PatientId = PageState.GetStringOrEmpty(Constants.SessionVariables.PatientId)
                        };
                        RedirectToSelectPatient(null, selectPatientComponentParameters);
                        break;
                    }
            default:
                    {
                        SelectPatientComponentParameters selectPatientComponentParameters = new SelectPatientComponentParameters
                        {
                            PatientId = PageState.GetStringOrEmpty(Constants.SessionVariables.PatientId)
                        };
                        RedirectToSelectPatient(null, selectPatientComponentParameters);
                        break;
                    }
            }
	}
}

}