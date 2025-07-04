using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Web.Management;
using Allscripts.Impact;
using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.Medispan.Clinical.Model.Settings;
using eRxWeb.State;

namespace eRxWeb
{
public partial class Controls_RxCopy : BaseControl
{
	#region Public delegates and events

	public delegate void RxCopyCompleteHandler(RxCopyEventArgs rxCopyEventArgs);
	public event RxCopyCompleteHandler OnRxCopyComplete;

	#endregion Public delegate and events

	#region Private members

	private const string _instructions = "On clicking <b>Save & Review</b>, you will be creating another prescription for <b>{0}</b> that can be sent to a different pharmacy.";
    private bool _allowMDD = false;
    private bool _MDDCSMedOnly = false;
    private IStateContainer PageState; 

	#endregion Private members

	#region Page Events

	protected void Page_Load(object sender, EventArgs e)
    {
		txtPharmComments.Attributes.Add("onkeydown", "checkFieldLength(this,'" + charsRemaining.ClientID + "', 210);");
		txtPharmComments.Attributes.Add("onkeyup", "checkFieldLength(this,'" + charsRemaining.ClientID + "', 210);");
		txtPharmComments.Attributes.Add("onmouseover", "checkFieldLength(this,'" + charsRemaining.ClientID + "', 210);");
		txtPharmComments.Attributes.Add("onchange", "checkFieldLength(this,'" + charsRemaining.ClientID + "', 210);");
	    PageState = new StateContainer(this.Session);
    }

	protected void btnSaveAndReviewRxCopy_Click(object sender, EventArgs e)
	{
		bool isValid = validateUserInput();

		if (isValid)
		{
			saveCopiedRx();
            clearSessionContents();
            mpeRxCopy.Hide();
		}
		else
		{
			mpeRxCopy.Show();
		}
	}

	protected void btnCancelRxCopy_Click(object sender, EventArgs e)
	{
		RxCopyEventArgs rxCopyEventArgs = new RxCopyEventArgs();
		rxCopyEventArgs.IsCancel = true;
		rxCopyEventArgs.IsSuccessful = false;

		if (OnRxCopyComplete != null)
		{
			OnRxCopyComplete(rxCopyEventArgs);
		}
        clearSessionContents();
		mpeRxCopy.Hide();
	}

    private void clearSessionContents()
    {
         Session.Remove("RxCopied");
         Session.Remove("RxIDBase");
    }
	protected void ddlCustomPack_PreRender(object sender, EventArgs e)
	{
		string package = string.Empty;

		if (Session["PackageSize"] != null && Session["PackageQuantity"] != null)
		{
			package = "[PZ=" + Session["PackageSize"].ToString() + "](PQ=" + Session["PackageQuantity"].ToString() + ")";
		}

		//if we already have a selected item, no need to reselect
		if (ddlCustomPack.SelectedItem == null)
		{
			if (package != string.Empty)
			{
				if (ddlCustomPack.Items.FindByValue(package) != null)
				{
					ddlCustomPack.Items.FindByValue(package).Selected = true;
				}
			}
		}
	}

	#endregion Page Events

	#region Public properties

	/// <summary>
	/// Cloned Rx object. Set by calling page.
	/// </summary>
	public Rx RxCopy 
	{
		get
		{
			return (Rx)Session["RxCopied"];
		}
		set
		{
            Session["RxCopied"] = value;
		}
	}

	public string RxIDBase
	{
		get
		{
            return (string)Session["RxIDBase"];
		}
		set
		{
            Session["RxIDBase"] = value;
		}
	}

	#endregion Public properties

	#region Public methods

	public void Show()
	{
		setVariables();

		mpeRxCopy.Show();
	}

	#endregion Public methods

	#region Private methods

	private void setVariables()
	{
		this.packageSizeViewState = null;
		lblError.Text = null;

		lblInstructions.Text = string.Format(_instructions, this.RxCopy.MedicationName);
		lblMedication.Text = formattedMedName;
		lblSig.Text = this.RxCopy.SigText;
		txtDaysSupply.Text = "90";
	    txtMDD.Text = RxCopy.MDD;

		if (this.RxCopy.Quantity != 0)
		{
			txtQuantity.Text = Convert.ToDouble(this.RxCopy.Quantity).ToString();
		}
		else
		{
			txtQuantity.Text = string.Empty;
			txtQuantity.Focus();
		}
		
		txtPharmComments.Text = string.Empty;
		chkDAW.Checked = this.RxCopy.DAW;

		if (this.RxCopy.ControlledSubstanceCode != null && this.RxCopy.ControlledSubstanceCode.Equals("2"))
		{
			txtRefill.Text = "0";
			txtRefill.Enabled = false;
		}
		else
		{
			txtRefill.Text = "3";
		}

		checkForPillVsNonPillRx();
	    checkMDDSite();
	    if (_allowMDD)
	    {
	        SetMDDFieldsVisablity(true);

	        if (_MDDCSMedOnly && String.IsNullOrWhiteSpace(this.RxCopy.ControlledSubstanceCode))
	        {
	            SetMDDFieldsVisablity(false);
	        }
	    }
	    else
	    {
	        SetMDDFieldsVisablity(false);
	    }
	}

    private void checkMDDSite()
    {
        _allowMDD = PageState.GetBooleanOrFalse("ALLOWMDD");
        if (_allowMDD)
        {
            _MDDCSMedOnly = PageState.GetBooleanOrFalse("CSMEDSONLY");          
        }
    }

    private void SetMDDFieldsVisablity(bool visable)
    {
        lblMDD.Visible = visable;
        lblPerDay.Visible = visable;
        txtMDD.Visible = visable;
    }

	private void checkForPillVsNonPillRx()
	{
		if (this.RxCopy.DDI != null)
		{
			//this should really already be on the Rx object I would think... argh
			DataSet drRxDetails = Allscripts.Impact.Prescription.ChGetRXDetails(this.RxIDBase, base.DBID);
			DataTable dtRx = drRxDetails.Tables[0];

			string packageSize = null;
			string packageDescription = null;

			if (dtRx.Rows.Count > 0)
			{
				DataRow drRX = dtRx.Rows[0];

				packageSize = drRX["PSPQ"].ToString().Trim();
				packageDescription = drRX["PackageDescription"] != DBNull.Value ? drRX["PackageDescription"].ToString().Trim() : string.Empty;
			}

			DataSet ds = Allscripts.Impact.Medication.GetPackagesForMedication(this.RxCopy.DDI, this.RxCopy.DosageFormCode, base.DBID);
			DataTable dt = ds.Tables["Package"];

			if (dt.Rows.Count > 1)
			{
				DataView dataView = dt.DefaultView;
				dataView.Sort = "PackageSize";

				ddlCustomPack.DataSource = dataView;
				ddlCustomPack.DataBind();
				this.packageSizeViewState = dt;

				if (packageSize != null && ddlCustomPack.Items.FindByValue(packageSize) != null)
				{
					ddlCustomPack.Items.FindByValue(packageSize).Selected = true;
				}
				else if (packageDescription != string.Empty && ddlCustomPack.Items.FindByText(packageDescription) != null)
				{
					ddlCustomPack.Items.FindByText(packageDescription).Selected = true;
				}

				pnlNonPillMed.Visible = true;
			}
			else
			{
				pnlNonPillMed.Visible = false;
			}
		}
		else
		{
			pnlNonPillMed.Visible = false;
		}
	}

	private bool validateUserInput()
	{
		bool isValid = true;
		string message = null;

		int daysSupply = int.Parse(txtDaysSupply.Text.Trim());
		int refills = int.Parse(txtRefill.Text.Trim());

	    var medValidator = new MedicationValidator();
        try
		{
			if (!string.IsNullOrEmpty(this.RxCopy.ControlledSubstanceCode))
			{
				string controlledSubstanceCode = this.RxCopy.ControlledSubstanceCode;

				if (controlledSubstanceCode.Trim() == "2")
				{
					if (!medValidator.IsValidCs2DaysSupply(daysSupply))
					{
						message = Constants.ErrorMessages.MedicationCs2DaysSupplyInvalid;
					}

					if (!medValidator.IsValidCs2Refills(refills))
					{
						message = Constants.ErrorMessages.MedicationCs2RefillsInvalid;
					}
				}
				else if (controlledSubstanceCode.Trim() == "3" || controlledSubstanceCode.Trim() == "4" || controlledSubstanceCode.Trim() == "5")
				{
				    string errorMsg;
				    if (!medValidator.IsValidCs3Or4Or5DaysSupply(daysSupply, out errorMsg))
                    {
						message = errorMsg;
					}

					if (!medValidator.IsValidCs3Or4Or5Refills(refills))
					{
						message = Constants.ErrorMessages.MedicationCs3Or4Or5RefillInvalid;
					}
				}

			}
			if (isValid)
			{
				decimal qty;
				if (decimal.TryParse(txtQuantity.Text.Trim(), out qty))
				{
					decimal packSize = 1M;
					int packQuantity = 1;

					if (!string.IsNullOrWhiteSpace(ddlCustomPack.SelectedValue))
					{
						var pspqValue = ddlCustomPack.SelectedValue;
						if (pspqValue.IndexOf("[PZ=") != -1)     //Get the package size ...
						{
							var pZstIndex = pspqValue.IndexOf("[PZ");
							var pZendIndex = pspqValue.IndexOf("]");
							if (!decimal.TryParse(pspqValue.Substring(pZstIndex + 4, pZendIndex - 4 - pZstIndex), out packSize))
							{
								packSize = 1;
							}
						}
						if (pspqValue.IndexOf("(PQ=") != -1)           //Get the package quantity
						{
							var pQstIndex = pspqValue.IndexOf("(PQ");
							var PQendIndex = pspqValue.IndexOf(")");
							if (!int.TryParse(pspqValue.Substring(pQstIndex + 4, PQendIndex - pQstIndex - 4), out packQuantity))
							{
								packQuantity = 1;
							}
						}
					}

					if (medValidator.IsValidQuantity(txtQuantity.Text))
					{
					    if (!medValidator.IsValidCalculatedQuantity(qty, packQuantity, packSize))
					    {
					        message = "The calculated quantity of your copied medication is > 9999. Please review your selected medication for accuracy of package size selection.";
					        isValid = false;
					    }
					}
					else
					{
					    message = Constants.ErrorMessages.MedicationQuantityInvalid;
					    isValid = false;
                    }

				}
				if (isValid && !medValidator.IsValidRefills(refills))
				{
					message = Constants.ErrorMessages.MedicationRefillsInvalid;
					isValid = false;
				}
				if (isValid && !medValidator.IsValidDaysSupply(daysSupply))
				{
					message = Constants.ErrorMessages.MedicationDaysSupplyInvalid;
					isValid = false;
				}
			}
		}

		catch (Exception ex)
		{
			message = ex.Message;
			isValid = false;
		}

		if (!string.IsNullOrEmpty(message))
		{
			isValid = false;
			lblError.Text = message;
		}
		return isValid;
	}

	private void saveCopiedRx()
	{
		RxCopyEventArgs rxCopyEventArgs = new RxCopyEventArgs();

		try
		{
			Prescription rx = new Prescription();

			string providerID = getProviderID();
			string authorizerID = getAuthorizerID();
			string rxDate = DateTime.UtcNow.ToString();
			string startDate = DateTime.Today.ToShortDateString();
			string pharmacyID = System.Guid.Empty.ToString();
			Constants.PrescriptionStatus status = Constants.PrescriptionStatus.SCRIPT_PAD;
			string transmissionMethod = null; //setting transmission method to NULL b/c it hasn't been transmitted yet. Only set transmission method on script pad page or as final step in processing task.
			FormularyStatus formularyStatus = (this.RxCopy.FormularyStatus != null ? (FormularyStatus)Convert.ToInt32(this.RxCopy.FormularyStatus) : FormularyStatus.NONE);
			string extFacilityCode = null;
			string extGroupID = null;

			if (Session["ExtFacilityCd"] != null)
			{
				extFacilityCode = Session["ExtFacilityCd"].ToString();
			}

			if (Session["ExtGroupID"] != null)
			{
				extGroupID = Session["ExtGroupID"].ToString();
			}

			if (this.packageSizeViewState != null && ddlCustomPack.SelectedItem != null)
			{
				DataTable dtPackage = this.packageSizeViewState;
				DataRow[] drPackage = dtPackage.Select("PackageDescription ='" + ddlCustomPack.SelectedItem.Text + "'");

				if (drPackage.Length > 0)
				{
					this.RxCopy.GPPC = drPackage[0]["GPPC"].ToString();
					this.RxCopy.PackageSize = Convert.ToDecimal(drPackage[0]["PackageSize"].ToString());
					this.RxCopy.PackageUOM = drPackage[0]["PackageUOM"].ToString();
					this.RxCopy.PackageQuantity = Convert.ToInt32(drPackage[0]["PackageQuantity"].ToString());
					this.RxCopy.PackageDescription = drPackage[0]["PackageDescription"].ToString();
				}
			}

			rx.SetHeaderInformation(base.SessionLicenseID, this.RxCopy.RxID, rxDate, base.SessionPatientID, providerID, pharmacyID, this.RxCopy.PlanID, this.RxCopy.GroupID,
				this.RxCopy.FormularyID, Constants.CommonAbbreviations.NO, false, string.Empty, base.SessionSiteID, Constants.ERX_NOW_RX, null, base.DBID);

			rx.AddMedication(base.SessionLicenseID, 0, this.RxCopy.DDI, this.RxCopy.MedicationName, this.RxCopy.RouteOfAdminCode, this.RxCopy.DosageFormCode, this.RxCopy.Strength,
				this.RxCopy.StrengthUOM, this.RxCopy.SigID, this.RxCopy.SigText, int.Parse(txtRefill.Text), Decimal.Parse(txtQuantity.Text), int.Parse(txtDaysSupply.Text), this.RxCopy.GPPC, this.RxCopy.PackageSize,
				this.RxCopy.PackageUOM, this.RxCopy.PackageQuantity, this.RxCopy.PackageDescription, chkDAW.Checked, startDate, status, transmissionMethod, this.RxCopy.OriginalDDI,
				this.RxCopy.OriginalFormularyStatusCode, this.RxCopy.OriginalIsListed, this.RxCopy.SourceFormularyStatus, Constants.CommonAbbreviations.NO,
				formularyStatus, Session["PERFORM_FORMULARY"].ToString(),
                DURSettings.CheckPerformDose.ToChar(),
                DURSettings.CheckDrugToDrugInteraction.ToChar(),
                DURSettings.CheckDuplicateTherapy.ToChar(),
                DURSettings.CheckPar.ToChar(),
                Session["PRACTICESTATE"].ToString(), txtPharmComments.Text, this.RxCopy.OriginalRxID, 0, Constants.RxCreationType.COPIED_RX, null,
				this.RxCopy.ICD10Code, this.RxCopy.ControlledSubstanceCode, base.SessionUserID, string.Empty, authorizerID, PrescriptionWorkFlow.STANDARD, extFacilityCode, extGroupID, this.RxCopy.CoverageID,
				this.RxCopy.AlternativeIgnoreReason, this.RxCopy.StateControlledSubstanceCode, this.RxCopy.FormularyAlternativeShown, this.RxCopy.PreDURDose, this.RxCopy.PreDURPAR,
				this.RxCopy.PreDURDrugFood, this.RxCopy.PreDURDrugAlcohol, this.RxCopy.PreDURDrugDrug, this.RxCopy.PreDURDUP, this.RxCopy.PreDURDisease, this.RxCopy.PriorAuthRequired,
				this.RxCopy.IsCompoundMed, this.RxCopy.IsFreeFormMedControlSubstance, this.RxCopy.ScheduleUsed, this.RxCopy.HasSupplyItem, null, this.RxCopy.SigTypeId);

			rx.Save(base.SessionSiteID, base.SessionLicenseID, base.SessionUserID, base.DBID);
            Prescription.SaveMaximumDailyDosage(RxCopy.RxID, txtMDD.Text, base.DBID);
		    

			Prescription.RxCopiedInsert(this.RxCopy.RxID, this.RxIDBase, base.DBID);

			rxCopyEventArgs.IsSuccessful = true;
		}
		catch (Exception ex)
		{
			string exceptionID = Audit.AddException(base.SessionUserID, base.SessionLicenseID, ex.ToString(), Request.UserIpAddress(), null, null, base.DBID);

			rxCopyEventArgs.ExceptionReferenceID = exceptionID;
			rxCopyEventArgs.IsSuccessful = false;
		}

		if (OnRxCopyComplete != null)
		{
			OnRxCopyComplete(rxCopyEventArgs);
		}
	}

	private string getProviderID()
	{
		string providerID = null;

		providerID = base.SessionUserID;

		if (Convert.ToBoolean(Session["IsDelegateProvider"]))
		{
			if (Session["PHYSICIANID"] != null)
			{
				providerID = Session["PHYSICIANID"].ToString();
			}
			else if (Session["DelegateProviderID"] != null)
			{
				providerID = Session["DelegateProviderID"].ToString();
			}
		}

		return providerID;
	}

	private string getAuthorizerID()
	{
		string authorizerID = base.SessionUserID;

		if (Convert.ToBoolean(Session["IsDelegateProvider"]) || Convert.ToBoolean(Session["IsPASupervised"]))
		{
			if (base.DelegateProvider.UserType == (int)Constants.UserCategory.PHYSICIAN_ASSISTANT_SUPERVISED)
			{
				authorizerID = Session["SUPERVISING_PROVIDER_ID"].ToString();
			}
			else
			{
				authorizerID = Session["DelegateProviderID"].ToString();
			}
		}

		return authorizerID;
	}

	#endregion Private methods

	#region Private properties

	private string formattedMedName
	{
		get
		{
			string formattedMedName = string.Empty;

			if (this.RxCopy != null)
			{
				if (!string.IsNullOrEmpty(this.RxCopy.MedicationName))
				{
					formattedMedName = this.RxCopy.MedicationName;
				}

				if (!string.IsNullOrEmpty(this.RxCopy.Strength))
				{
					formattedMedName = string.Concat(formattedMedName, " ", this.RxCopy.Strength);
				}

				if (!string.IsNullOrEmpty(this.RxCopy.StrengthUOM))
				{
					formattedMedName = string.Concat(formattedMedName, " ", this.RxCopy.StrengthUOM);
				}

				if (!string.IsNullOrEmpty(this.RxCopy.DosageFormDescription))
				{
					formattedMedName = string.Concat(formattedMedName, " ", this.RxCopy.DosageFormDescription);
				}

				if (!string.IsNullOrEmpty(this.RxCopy.RouteOfAdminDescription))
				{
					formattedMedName = string.Concat(formattedMedName, " ", this.RxCopy.RouteOfAdminDescription);
				}
			}

			return formattedMedName;
		}
	}

	private DataTable packageSizeViewState
	{
		get
		{
			return (DataTable)ViewState["RxCopyPackageSize"];
		}
		set
		{
			ViewState["RxCopyPackageSize"] = value;
		}
	}

	#endregion Private properties
}
}