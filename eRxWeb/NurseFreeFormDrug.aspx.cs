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
using Allscripts.Impact;
using System.Collections.Generic;
using Allscripts.ePrescribe.Common;

namespace eRxWeb
{
public partial class NurseFreeFormDrug : BasePage 
{
	
    protected void Page_Init(object sender, EventArgs e)
	{
		//If the session does not have a delegate provider or the delegatedProvider user is not a valid provider 
		//then redirec the user to search patient page. 
		if (Session["DelegateProviderID"] == null)
		{
            Server.Transfer(Constants.PageNames.SELECT_PATIENT);
		}
	}
    
    protected void Page_Load(object sender, EventArgs e)
    {

        if (!Page.IsPostBack)
        {
			if (Request.QueryString["medname"] != null)
			{
				txtFreeForm.Text = HttpUtility.UrlDecode(Request.QueryString["medname"]);
			}

            txtFreeForm.Focus();
            txtFreeForm.Attributes.Add("onkeydown", "checkFieldLength(this,'" + charsRemaining.ClientID + "', 105);");
            txtFreeForm.Attributes.Add("onkeyup", "checkFieldLength(this,'" + charsRemaining.ClientID + "', 105);");
            txtFreeForm.Attributes.Add("onmouseover", "checkFieldLength(this,'" + charsRemaining.ClientID + "', 105);");
            txtFreeForm.Attributes.Add("onchange", "checkFieldLength(this,'" + charsRemaining.ClientID + "', 105);");

           if (Convert.ToBoolean(Session["IsPA"]) || Convert.ToBoolean(Session["IsPASupervised"]))
           {
             ucMessage.Visible = true;
             ucMessage.MessageText = "Prescription being written under the supervision of " + DelegateProviderName;
             ucMessage.Icon = Controls_Message.MessageType.INFORMATION;
           }
           else
           {
             ucMessage.Visible = true;
             ucMessage.MessageText = string.Format("Prescription written on behalf of {0}." , DelegateProviderName);
             ucMessage.Icon = Controls_Message.MessageType.INFORMATION;
           }
        }
    }
    
    protected void Page_LoadComplete(object sender, EventArgs e)
    {
        ((PhysicianMasterPage)Master).hideTabs();
    }
    
    protected void btnSelectSig_Click(object sender, EventArgs e)
    {
        ArrayList rxList = new ArrayList();
        Rx rx = new Rx();

        rx.MedicationName = txtFreeForm.Text.Trim();

        if (PageState.GetStringOrEmpty("ICD10CODE") != string.Empty)
        {

            rx.ICD10Code = Session["ICD10CODE"].ToString();
        }

        if (chkBoxCSMed.Checked)
        {
            rx.IsFreeFormMedControlSubstance = true;
            rx.ScheduleUsed = Convert.ToInt32(rblSchedule.SelectedValue);
        }

		if (rblFreeTextMedType.SelectedValue.Equals("Compound", StringComparison.OrdinalIgnoreCase))
        {
            rx.IsCompoundMed = true;
        }
		else if (rblFreeTextMedType.SelectedValue.Equals("Supply", StringComparison.OrdinalIgnoreCase))
        {
            rx.HasSupplyItem = true;
        }

        rxList.Add(rx);
        Session["RxList"] = rxList;

        Server.Transfer(Constants.PageNames.NURSE_SIG + "?CameFrom=FreeForm");
    }
    
    protected void btnChangeMed_Click(object sender, EventArgs e)
    {
        if (Session[Constants.SessionVariables.TaskScriptMessageId] != null && Request.QueryString["medname"] != null)
        {
            Response.Redirect(Constants.PageNames.NURSE_MED + "?Search=A&searchText=" + Request.QueryString["medname"]);
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

        if (base.SessionUserType == Constants.UserCategory.PHYSICIAN_ASSISTANT_SUPERVISED)
        {
            requiredDEASchedule = findCommonPAandSuperDeaSchedule();
        }
        else 
        {
            if (Session["DEASCHEDULESALLOWED_SUPERVISOR"] != null)
            {
                requiredDEASchedule = (List<string>)Session["DEASCHEDULESALLOWED_SUPERVISOR"];
                requiredDEASchedule.Sort();
            }
        }

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
			if (base.SessionUserType == Allscripts.ePrescribe.Common.Constants.UserCategory.PHYSICIAN_ASSISTANT_SUPERVISED)
            {
                List<string> commonMinDEASchedule =  findCommonPAandSuperDeaSchedule();

                if (commonMinDEASchedule != null && commonMinDEASchedule.Count > 0)
                {
                    //show CSMed check box when Provider and PA has DEA schedule
                    chkBoxCSMed.Visible = true;
                    pnlControlledSubstance.Visible = true;
                }
            }
            else if (Session["MINSCHEDULEALLOWED"] != null || (Session["MINSCHEDULEALLOWED_SUPERVISOR"] != null && Convert.ToInt32(Session["MINSCHEDULEALLOWED_SUPERVISOR"].ToString()) > 0))
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

    private List<string> findCommonPAandSuperDeaSchedule()
    {
        List<string> commonMinDEASchedule = new List<string>();

        if (Session["DEASCHEDULESALLOWED"] != null && Session["DEASCHEDULESALLOWED_SUPERVISOR"] != null)
        {
            List<string> deaLicense = new List<string>();
            List<string> deaLicenseSuper = new List<string>();

            deaLicense = (List<string>)Session["DEASCHEDULESALLOWED"];
            deaLicenseSuper = (List<string>)Session["DEASCHEDULESALLOWED_SUPERVISOR"];

            if (deaLicense.Contains("2") && deaLicenseSuper.Contains("2"))
            {
                commonMinDEASchedule.Add("2");
            }

            if (deaLicense.Contains("3") && deaLicenseSuper.Contains("3"))
            {
                commonMinDEASchedule.Add("3");
            }

            if (deaLicense.Contains("4") && deaLicenseSuper.Contains("4"))
            {
                commonMinDEASchedule.Add("4");
            }           

            if (deaLicense.Contains("5") && deaLicenseSuper.Contains("5"))
            {
                commonMinDEASchedule.Add("5");
            }                        
            commonMinDEASchedule.Sort();        
        }

        return commonMinDEASchedule;
    }

}

}