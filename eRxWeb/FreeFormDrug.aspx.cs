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
using System.Linq;
using System.Collections.Generic;
using Allscripts.ePrescribe.Common;
using System.Text.RegularExpressions;
using Allscripts.Impact;
using Microsoft.Security.Application;
using ScriptMessage = Allscripts.Impact.ScriptMsg.ScriptMessage;
using Allscripts.Impact.Utilities;

namespace eRxWeb
{
public partial class FreeFormDrug : BasePage
{
    public static string FREETEXT_SIG_ID = "00000000-0000-0000-0000-000000000000";

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {

            if (base.SessionUserType != Constants.UserCategory.PROVIDER &&
                base.SessionUserType != Constants.UserCategory.PHYSICIAN_ASSISTANT)
            {
                Response.Redirect(Constants.PageNames.NURSE_FREE_FORM_DRUG);
            }
            ClearMedicationInfo(false);

            if (Request.QueryString["medname"] != null)
			{
				txtFreeForm.Text = HttpUtility.UrlDecode(Request.QueryString["medname"]);
			}

            if (Session[Constants.SessionVariables.TaskScriptMessageId] != null && (Session[Constants.SessionVariables.IsCsRefReqWorkflow] != null || MasterPage.ChangeRxRequestedMedCs != null) )
            {
                ScriptMessage scriptMsg = new ScriptMessage(Session[Constants.SessionVariables.TaskScriptMessageId].ToString(), Session["LICENSEID"].ToString(), Session["USERID"].ToString(), base.DBID);
                string pharmPrefix = ($"{(MasterPage.ChangeRxRequestedMedCs != null ? "Change Rx" : "Renewal Request")} Pharmacy : ");
                ucMessagePharmacyHeader.MessageText = pharmPrefix + Allscripts.Impact.Utilities.StringHelper.GetPharmacyName(
                    scriptMsg.PharmacyName,
                    scriptMsg.PharmacyAddress1,
                    scriptMsg.PharmacyAddress2,
                    scriptMsg.PharmacyCity,
                    scriptMsg.PharmacyState,
                    scriptMsg.PharmacyZip,
                    scriptMsg.PharmacyPhoneNumber);
                ucMessagePharmacyHeader.Visible = true;
                if (Session[Constants.SessionVariables.IsCsRefReqWorkflow] != null)
                {
                    ucMessageRxHeader.MessageText = "Rx Detail : " + Allscripts.Impact.Utilities.StringHelper.GetRxDetails(
                        scriptMsg.DispensedRxDrugDescription,
                        scriptMsg.DispensedRxSIGText,
                        scriptMsg.DispensedRxQuantity,
                        scriptMsg.DispensedDaysSupply,
                        scriptMsg.DispensedRxRefills,
                        scriptMsg.DispensedDaw,
                        scriptMsg.DispensedCreated,
                        scriptMsg.DispensedDateLastFill,
                        scriptMsg.DispensedRxNotes);
                }
                else //ChangeCSRx workflow
                    {
                        var requestedRx = scriptMsg.RequestedRxs[0];
                    ucMessageRxHeader.MessageText = "Rx Detail : " + Allscripts.Impact.Utilities.StringHelper.GetRxDetails(
                        requestedRx.Description,
                        requestedRx.SigText,
                        requestedRx.Quantity.ToString(),
                        requestedRx.DaysSupply.ToString(),
                        requestedRx.Refills,
                        requestedRx.Daw,
                        requestedRx.CreateDate,
                        requestedRx.LastFillDate,
                        requestedRx.Notes);    
                }
                ucMessageRxHeader.Visible = true;
            }

            SetWorkflowSpecificControls();
            
			txtFreeForm.Focus();
            txtFreeForm.Attributes.Add("onkeydown", "checkFieldLength(this,'" + charsRemaining.ClientID + "', 105);");
            txtFreeForm.Attributes.Add("onkeyup", "checkFieldLength(this,'" + charsRemaining.ClientID + "', 105);");
            txtFreeForm.Attributes.Add("onmouseover", "checkFieldLength(this,'" + charsRemaining.ClientID + "', 105);");
            txtFreeForm.Attributes.Add("onchange", "checkFieldLength(this,'" + charsRemaining.ClientID + "', 105);");
        }
    }

    private void SetWorkflowSpecificControls()
    {
        if (MasterPage.RxTask != null && MasterPage.RxTask.TaskType == Constants.PrescriptionTaskType.RXCHG)
        {
                pnlSearchMeds.Visible = false;   
        }
        else
        {
                pnlSearchMeds.Visible = true;
        }
    }

    protected void Page_LoadComplete(object sender, EventArgs e)
    {
        ((PhysicianMasterPage)Master).hideTabs();
    }
    
    protected void btnSelectSig_Click(object sender, EventArgs e)
    {
        ArrayList rxList = new ArrayList();
        
        Rx rx;
        if (MasterPage.RxTask == null)
        {
            rx = new Rx();
            rx.SigID = FREETEXT_SIG_ID;
            rx.SigText = string.Empty;
                rx.SigTypeId = (int)SigTypeEnum.SigTypeFreeForm;
        }
        else
        {
            rx = MasterPage.RxTask.Rx as Rx ?? new Rx();
            rx.DDI = null;
        }

        rx.MedicationName = txtFreeForm.Text.Trim();

        if (PageState.GetStringOrEmpty("ICD10CODE") != string.Empty)
        {
            rx.ICD10Code = Session["ICD10CODE"].ToString();
        }
        
        SetValuesBasedOnCsChkBox(ref rx, chkBoxCSMed.Checked, rblSchedule?.SelectedValue);

        if (rblFreeTextMedType.SelectedValue.Equals("Compound", StringComparison.OrdinalIgnoreCase))
        {
            rx.IsCompoundMed = true;
        }
		else if (rblFreeTextMedType.SelectedValue.Equals("Supply", StringComparison.OrdinalIgnoreCase))
        {
            rx.HasSupplyItem = true;
        }

        rx.DosageFormCode = string.Empty;
        rx.StrengthUOM = string.Empty;

        if (MasterPage.RxTask != null && MasterPage.RxTask.TaskType == Constants.PrescriptionTaskType.RXCHG)
        {
            MasterPage.RxTask.Rx = rx;
        }

        rx.CoverageID = PageState.GetInt(Constants.SessionVariables.CoverageId, 0);

        rxList.Add(rx);
        Session["RxList"] = rxList;

        Server.Transfer(Constants.PageNames.SIG + "?CameFrom=FreeForm");
    }

    public static void SetValuesBasedOnCsChkBox(ref Rx rx, bool isCsMed, string schedule)
    {
        if (rx == null) return;

        if (isCsMed)
        {
            rx.IsFreeFormMedControlSubstance = true;
            rx.ScheduleUsed = Convert.ToInt32(schedule);
            rx.ControlledSubstanceCode = schedule;
        }
        else
        {
            rx.IsFreeFormMedControlSubstance = false;
            rx.ScheduleUsed = default(int);
            rx.ControlledSubstanceCode = default(string);
        }
    }

    protected void btnGo_Click(object sender, EventArgs e)
    {
		if (txtSearchMed.Text.Length > 0)
        {
            // Fortify regex validating.
            string medName = Encoder.UrlEncode(txtSearchMed.Text);
                Response.Redirect((Constants.PageNames.REDIRECT_TO_ANGULAR + "?componentName=" + Constants.PageNames.SELECT_MEDICATION) + "?searchText=" + HttpUtility.UrlEncode(medName));
            }
            else
        {
               Response.Redirect(Constants.PageNames.REDIRECT_TO_ANGULAR + "?componentName=" + Constants.PageNames.SELECT_MEDICATION);
            }
    }

    protected void btnAllMeds_Click(object sender, EventArgs e)
    {
        if (txtSearchMed.Text.Length > 0)
        {
            // Fortify regex validating.
            string medName = Encoder.UrlEncode(txtSearchMed.Text);
            Response.Redirect(Constants.PageNames.REDIRECT_TO_ANGULAR + "?componentName=" + Constants.PageNames.SELECT_MEDICATION + "?searchText=" + HttpUtility.UrlEncode(medName));
        }
        else
        {
            Response.Redirect(Constants.PageNames.REDIRECT_TO_ANGULAR + "?componentName=" + Constants.PageNames.SELECT_MEDICATION);
        }
    }

    protected void btnChooseMed_Click(object sender, EventArgs e)
    {
        //Check if CS refreq workflow
        if (Session[Constants.SessionVariables.TaskScriptMessageId] != null && (Session[Constants.SessionVariables.IsCsRefReqWorkflow] != null || MasterPage.ChangeRxRequestedMedCs != null))
        {
            ScriptMessage scriptMsg = new ScriptMessage(Session[Constants.SessionVariables.TaskScriptMessageId].ToString(), Session["LICENSEID"].ToString(), Session["USERID"].ToString(), base.DBID);
            if (scriptMsg != null)
            {
                var pharmDetails = StringHelper.GetPharmacyName(
                    scriptMsg.PharmacyName,
                    scriptMsg.PharmacyAddress1,
                    scriptMsg.PharmacyAddress2,
                    scriptMsg.PharmacyCity,
                    scriptMsg.PharmacyState,
                    scriptMsg.PharmacyZip,
                    scriptMsg.PharmacyPhoneNumber);
                string rxDetails;
                if (Session[Constants.SessionVariables.IsCsRefReqWorkflow] != null)
                {
                    rxDetails = StringHelper.GetRxDetails(
                        scriptMsg.DispensedRxDrugDescription,
                        scriptMsg.DispensedRxSIGText,
                        scriptMsg.DispensedRxQuantity,
                        scriptMsg.DispensedDaysSupply,
                        scriptMsg.DispensedRxRefills,
                        scriptMsg.DispensedDaw,
                        scriptMsg.DispensedCreated,
                        scriptMsg.DispensedDateLastFill,
                        scriptMsg.DispensedRxNotes);
                }
                else //ChangeCSRx workflow
                {
                    var requestedRx = scriptMsg.RequestedRxs[0];
                    rxDetails = StringHelper.GetRxDetails(
                        requestedRx.Description,
                        requestedRx.SigText,
                        requestedRx.Quantity.ToString(),
                        requestedRx.DaysSupply.ToString(),
                        requestedRx.Refills,
                        requestedRx.Daw,
                        requestedRx.CreateDate,
                        requestedRx.LastFillDate,
                        requestedRx.Notes);
                    }
                Response.Redirect(AppCode.CsMedUtil.RedirectForCSMed(pharmDetails, rxDetails, scriptMsg.DBDrugDescription));
            }
        }

        if (Session[Constants.SessionVariables.TaskScriptMessageId] != null && Request.QueryString["medname"] != null)
        {
            string medName = Encoder.UrlEncode(Request.QueryString["medname"].ToString());
            Response.Redirect(Constants.PageNames.REDIRECT_TO_ANGULAR + "?componentName=" + Constants.PageNames.SELECT_MEDICATION + "?Search=A&searchText=" + medName);
        }        
        if (txtSearchMed.Text.Length > 0)
        {
            string medName = txtSearchMed.Text;
            Response.Redirect((Constants.PageNames.REDIRECT_TO_ANGULAR + "?componentName=" + Constants.PageNames.SELECT_MEDICATION) + "?searchText=" + HttpUtility.UrlEncode(medName));
        }

        Response.Redirect(Constants.PageNames.REDIRECT_TO_ANGULAR + "?componentName=" + Constants.PageNames.SELECT_MEDICATION);
    }

    protected void setSchedule()
    {
        ListItem scheduleII = rblSchedule.Items.FindByValue("2");
        ListItem scheduleIII = rblSchedule.Items.FindByValue("3");
        ListItem scheduleIV = rblSchedule.Items.FindByValue("4");
        ListItem scheduleV = rblSchedule.Items.FindByValue("5");
        List<string> requiredDEASchedule = new List<string>();

        if (Session["DEASCHEDULESALLOWED"] != null && (Session["MINSCHEDULEALLOWED"] != null && Convert.ToInt32(Session["MINSCHEDULEALLOWED"]) > 0))
        {
            requiredDEASchedule = (List<string>)Session["DEASCHEDULESALLOWED"];
            requiredDEASchedule.Sort();

            if (requiredDEASchedule != null && requiredDEASchedule.Count > 0)
            {
                if (!requiredDEASchedule.Contains("2"))
                {
                    scheduleII.Enabled = false;
                }
                if (!requiredDEASchedule.Contains("3"))
                {
                    scheduleIII.Enabled = false;
                }

                if (!requiredDEASchedule.Contains("4"))
                {
                    scheduleIV.Enabled = false;
                }

                if (!requiredDEASchedule.Contains("5"))
                {
                    scheduleV.Enabled = false;
                }

                rblSchedule.SelectedValue = requiredDEASchedule[0];
            }
        }

    }    

    protected void chkBoxCSMed_CheckedChanged(object sender, EventArgs e)
    {
        if (chkBoxCSMed.Checked)
        {
            pnlSchedule.Visible = true;
            setSchedule();
        }
        else
        {
            pnlSchedule.Visible = false;
        }
    }

	protected void rblFreeTextMedType_SelectedIndexChanged(object sender, EventArgs e)
	{

		if (rblFreeTextMedType.SelectedValue.Equals("Compound", StringComparison.OrdinalIgnoreCase))
		{
            if (Session["MINSCHEDULEALLOWED"] != null && Convert.ToInt32(Session["MINSCHEDULEALLOWED"]) > 0)
            {
                //show CSMed check box when Provider has DEA schedule
                chkBoxCSMed.Visible = true;
                pnlControlledSubstance.Visible = true;
            }          
            
			if (!chkBoxCSMed.Checked)
			{
				pnlSchedule.Visible = false;
			}
		}
		else if (rblFreeTextMedType.SelectedValue.Equals("Supply", StringComparison.OrdinalIgnoreCase))
		{
			pnlControlledSubstance.Visible = false;
			pnlSchedule.Visible = false;
			chkBoxCSMed.Checked = false; 
		}
	}
}

}