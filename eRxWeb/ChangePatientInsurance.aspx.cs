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
using Telerik.Web.UI;
using Allscripts.ePrescribe.Common;

namespace eRxWeb
{
public partial class ChangePatientInsurance : BasePage 
{
	protected void Page_Load(object sender, EventArgs e)
	{
        this.Form.DefaultButton = btnSearhInsurance.UniqueID;
		if (!Page.IsPostBack)
		{

            string licenseID = Session["LICENSEID"].ToString();  // GET IT FROM THE SESSION.
			string patientID = Session["PATIENTID"].ToString();

			DataRow patientDataRow = getPatientData(licenseID, patientID);

            if (Request.QueryString["Mode"] != null && Request.QueryString["Mode"] == "EditPlan")
            {
                insuranceSearchPanel.Visible = false;

                DataSet ds = Allscripts.Impact.PatientCoverage.GetCoverage(Convert.ToInt64(Session["SelectedPlanID"]), base.DBID);                    
                DataTable dtCoverage = ds.Tables["PatientCoverage"];
                DataTable dtPlan = ds.Tables["PatientPlan"];
                DataRow drCoverage = null;
                DataRow drPlan = null;

                if (dtPlan != null && dtPlan.Rows.Count > 0)
                    drPlan = dtPlan.Rows[0];

                if (drPlan != null)
                {
                    this.PlanDescription = drPlan["PlanName"] != DBNull.Value ? drPlan["PlanName"].ToString() : string.Empty;
                    this.PlanID = drPlan["PlanID"] != DBNull.Value ? drPlan["PlanID"].ToString() : string.Empty;
                    this.PlanID = drPlan["PlanID"] != DBNull.Value ? drPlan["PlanID"].ToString() : "";
                }

                if (dtCoverage != null && dtCoverage.Rows.Count > 0)
                    drCoverage = dtCoverage.Rows[0];

                if (drCoverage != null)
                {
                    this.GroupNumber = drCoverage["GroupNumber"] != DBNull.Value ? drCoverage["GroupNumber"].ToString() : string.Empty;
                    this.MemberNumber = drCoverage["CardHolderID"] != DBNull.Value ? drCoverage["CardHolderID"].ToString() : string.Empty;
                    this.RelationshipToCardHolder = drCoverage["RelToCardHolder"] != DBNull.Value ? drCoverage["RelToCardHolder"].ToString() : string.Empty;
                    this.CardHolderLastName = drCoverage["CardHolderLastName"] != DBNull.Value ? drCoverage["CardHolderLastName"].ToString() : string.Empty;
                    this.CardHolderFirstName = drCoverage["CardHolderFirstName"] != DBNull.Value ? drCoverage["CardHolderFirstName"].ToString() : string.Empty;

                    if (drPlan["CardHolderInstructions"] != DBNull.Value)
                        txtMemberNo.ToolTip = drPlan["CardHolderInstructions"].ToString();

                    if (drPlan["GroupNumberInstructions"] != DBNull.Value)
                        txtGroupNo.ToolTip = drPlan["GroupNumberInstructions"].ToString();
                }
            }
            else
            {
                rdTopTenPlans.Checked = true;
                LoadTopTenInsurancePlans();
            }
		}
	}
    protected void Page_LoadComplete(object sender, EventArgs e)
    {
        ((PhysicianMasterPage)Master).hideTabs();
    }
	private void LoadTopTenInsurancePlans()
	{
		string licenseID = Session["LICENSEID"].ToString();
		DataSet top10InsurancePlans = Allscripts.Impact.FavoriteList.GetTop10Plans(licenseID, base.DBID);
		lbInsurancePlan.DataSource = top10InsurancePlans;
		lbInsurancePlan.DataTextField = "PlanName";
		lbInsurancePlan.DataValueField = "PlanID";
		lbInsurancePlan.DataBind();
	}
	protected void btnSearhInsurance_Click(object sender, EventArgs e)
	{
		if (rdTopTenPlans.Checked)
		{
			LoadTopTenInsurancePlans();
		}
		else if (rdSearchByName.Checked)
		{
			string name = txtInsuranceName.Text.Trim();

            DataSet matchedInsurancePlans = Allscripts.Impact.Plan.Search(name, string.Empty, base.DBID);
			lbInsurancePlan.DataSource = matchedInsurancePlans;
			lbInsurancePlan.DataTextField = "PlanName";
			lbInsurancePlan.DataValueField = "PlanID";
			lbInsurancePlan.DataBind();
		}
	}
	protected void lbInsurancePlan_DataBound(object sender, EventArgs e)
	{
		if (lbInsurancePlan.Items.Count == 0)
		{
			lbInsurancePlan.Items.Insert(0, new ListItem("No insurance plan found. Please search again.", string.Empty));
		}
	}
	protected void btnUpdatePatientInsurance_Click(object sender, EventArgs e)
	{
        if (Request.QueryString["Mode"] != null &&
            Request.QueryString["Mode"] == "EditPlan")
        {
            updatePatientInsurance();
                if (Convert.ToBoolean(isFromAngularModal.Value))
                {
                    
                    // ClientScript.RegisterClientScriptBlock(this.GetType(), "", "  window.parent.ParentPopupClose(true, 'patient-info');", true);
                    Server.Transfer(Constants.PageNames.ADD_PATIENT + "?Mode=Edit");
                }
                else
                {
                    DefaultRedirect();
                }
        }
        else
        {
            if (lbInsurancePlan.SelectedIndex >= 0 &&
            lbInsurancePlan.SelectedItem.Value.Length > 0)
            {
                updatePatientInsurance();
                if (Convert.ToBoolean(isFromAngularModal.Value))
                {
                        // ClientScript.RegisterClientScriptBlock(this.GetType(), "", "  window.parent.ParentPopupClose(true, 'patient-info');", true);
                        Server.Transfer(Constants.PageNames.ADD_PATIENT + "?Mode=Edit");
                    }
                else
                {
                    DefaultRedirect();
                }

            }
            else
            {
                lblStatus.Visible = true;
                lblStatus.Text = "Please select an insurance plan.";
            }
        }
	}
	protected void txtInsuranceName_Init(object sender, EventArgs e)
	{
		//Register client side event handler
		txtInsuranceName.Attributes.Add("onfocus", "insuranceName_onFocus()");
	}

	private DataRow getPatientData(string licenseID, string patientID)
	{
		DataSet patientDataSet = Allscripts.Impact.CHPatient.PatientSearchById(patientID, licenseID, Session["USERID"].ToString(), base.DBID);

		DataRow patientDataRow = null; 

		if (patientDataSet.Tables["Patient"].Rows.Count > 0)
			patientDataRow = patientDataSet.Tables["Patient"].Rows[0];

		return patientDataRow; 
	}
	private void updatePatientInsurance()
	{
		// This method is used for SAVING the edited details ...

		string licenseID = Session["LICENSEID"].ToString();  // GET IT FROM THE SESSION.
		string userId = Session["USERID"].ToString();
		string patientID = Session["PATIENTID"].ToString();

		DataRow patientDataRow = getPatientData(licenseID, patientID);

		if (patientDataRow != null)
		{
			string chartID = patientDataRow["ChartID"].ToString();
			string lastName = patientDataRow["LastName"].ToString();
			string paternalName = patientDataRow["PaternalName"].ToString();
			string maternalName = patientDataRow["MaternalName"].ToString();
			string firstName = patientDataRow["FirstName"].ToString();
			string middleName = patientDataRow["MiddleName"].ToString();
			string address1 = patientDataRow["Address1"].ToString();
			string address2 = patientDataRow["Address2"].ToString();
			string city = patientDataRow["City"].ToString();
			string state = patientDataRow["State"].ToString();
			string zip = patientDataRow["Zip"].ToString();
			string phone = patientDataRow["Phone"].ToString();
            string mobilePhone = patientDataRow["MobilePhone"].ToString();
            string dob = patientDataRow["Dob"].ToString();
			string account = string.Empty;
			string sex = patientDataRow["Sex"].ToString();
			int active = 1; // 1 = active patient
            if (Session["PATIENTSTATUS"] != null && Session["PATIENTSTATUS"].ToString() != string.Empty)
            {
                active = Convert.ToInt32(Session["PATIENTSTATUS"].ToString());
            }
			string notes = string.Empty; // **
			string planID = string.Empty;
            string planName = string.Empty;

			//Set the plan id only if user has selected an insurance plan and 
			//the selected item's value is not empty string. 
            if (lbInsurancePlan.SelectedIndex >= 0 &&
                lbInsurancePlan.SelectedItem.Value.Length > 0)
            {
                planID = lbInsurancePlan.SelectedItem.Value;
                planName = lbInsurancePlan.SelectedItem.Text;
            }
            else
            {
                planID = this.PlanID;
            }

			string groupNumber = this.GroupNumber;
			string cardHolderID = this.MemberNumber;
			string relToCardHolder = this.RelationshipToCardHolder;
            string _payorID = string.Empty;
            string personCode = relToCardHolder;
			string apptNumber = string.Empty;
			string apptTime = string.Empty;
			string ext_PMS_Code = string.Empty;
			string duration = string.Empty;
			string status = string.Empty;
			string comments = string.Empty;
			string email = patientDataRow["Email"].ToString();
            string weight = Convert.ToString(patientDataRow["WeightKg"]);
            string height = Convert.ToString(patientDataRow["HeightCm"]);
            bool isHealthPlanDisclosable = Convert.ToBoolean(patientDataRow["isHealthPlanDisclosureAllowed"]);

            EPSBroker.SavePatient(licenseID, userId, patientID, chartID, 
				lastName, firstName, middleName, address1, address2, city, state, zip,
				phone, dob, account, sex, active, notes, ext_PMS_Code, comments, email,
                paternalName, maternalName, null, null, mobilePhone, null, string.Empty, weight, height, isHealthPlanDisclosable, base.DBID);

            Int64 selectedPlanID = Session["SelectedPlanID"] != null ?
                Int64.Parse(Session["SelectedPlanID"].ToString()) : 0;

            Allscripts.Impact.PatientCoverage.AddPatientCoverage(
                selectedPlanID,
                patientID,
                licenseID,
                userId,
                planID,
                planName,
                groupNumber,
                cardHolderID,
                relToCardHolder,
                personCode,
                _payorID,
                this.CardHolderLastName,
                this.CardHolderFirstName,
                base.DBID);

			Session["INSURANCE"] = planName;
            Session.Remove("SelectedPlanID");
		}
	}
	protected void btnCancelUpdate_Click(object sender, EventArgs e)
	{
        Session.Remove("SelectedPlanID");
		Server.Transfer(Constants.PageNames.ADD_PATIENT + "?Mode=Edit");
	}
	public string PlanDescription
	{
		get { return insurancePlanLabel.Text.Trim(); }
		set { insurancePlanLabel.Text = value; }
	}
	public string GroupNumber
	{
		get { return txtGroupNo.Text.Trim(); }
		set { txtGroupNo.Text = value; }
	}
	public string MemberNumber
	{
		get { return txtMemberNo.Text.Trim(); }
		set { txtMemberNo.Text = value; }
	}
    public string PlanID
    {
        get { return lblPlanID.Text.Trim(); }
        set { lblPlanID.Text = value; }
    }
    public string CardHolderFirstName
    {
        get { return txtFirstName.Text.Trim(); }
        set { txtFirstName.Text = value; }
    }
    public string CardHolderLastName
    {
        get { return txtLastName.Text.Trim(); }
        set { txtLastName.Text = value; }
    }

    
        
    
	public string RelationshipToCardHolder
	{
		get
		{
			string relToCardHolder = string.Empty;

			if (rdMember.Checked == true)
				relToCardHolder = "1";
			else if (rdSpouse.Checked == true)
				relToCardHolder = "2";
			else if (RdChild.Checked == true)
				relToCardHolder = "3";
			else if (RdOther.Checked == true)
				relToCardHolder = "0";

			return relToCardHolder;
		}
		set
		{
			int relIndicator;
			try
			{
				relIndicator = int.Parse(value);
			}
			catch
			{
				relIndicator = -1;
			}

			if (relIndicator == 1)
				rdMember.Checked = true;
			else if (relIndicator == 2)
				rdSpouse.Checked = true;
			else if (relIndicator == 3)
				RdChild.Checked = true;
			else if (relIndicator != -1)
				RdOther.Checked = true;
		}
	}
}

}