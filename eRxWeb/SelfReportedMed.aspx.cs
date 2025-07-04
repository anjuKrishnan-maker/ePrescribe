/******************************************************************************
**Change History
*******************************************************************************
**Date:      Author:                    Description:
**-----------------------------------------------------------------------------
* 08/17/2009  Dharani Reddem           Added strength and Units  for self reported Medications .
*                                     
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
using Telerik.Web.UI;
using Allscripts.Impact;
using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.Medispan.Clinical.Model.Response;
using Allscripts.ePrescribe.Medispan.Clinical.Model.Settings;
using eRxWeb.AppCode;

namespace eRxWeb
{
public partial class SelfReportedMed : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        txtSearchMed.Focus();
    }

    protected void Page_LoadComplete(object sender, EventArgs e)
    {
        ((PhysicianMasterPage)Master).hideTabs();
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        grdViewMed.DataSourceID  = "medDataSource";
        grdViewMed.MasterTableView.CurrentPageIndex = 0; 
    }

    protected void medDataSource_Selecting(object sender, ObjectDataSourceSelectingEventArgs e)
    {
        if (e.InputParameters["drugName"] != null)
        {
            if (e.InputParameters["drugname"].ToString() == string.Empty)
            {
                e.Cancel = true;
                txtSearchMed.Focus();
            }
            else
            {
                string prevalue = e.InputParameters["drugName"].ToString();
                prevalue = prevalue.Replace("'", string.Empty);
                prevalue = prevalue.Replace("\"", string.Empty);
                e.InputParameters["drugName"] = prevalue;
            }
        }
    }

    protected void grdViewMed_ItemDataBound(object sender, GridItemEventArgs e)
    {
        if (e.Item is GridDataItem)
        {
            e.Item.Style["cursor"] = "pointer";
            e.Item.Attributes.Add("onclick", "onRowClick(this)");
        }
    }

	protected void gridRadioSelect_CheckedChanged(object sender, EventArgs e)
	{
		grdViewMed.SelectedIndexes.Clear();

		//Select the row whose RadioButton is selected
		RadioButton radio = sender as RadioButton;
		(radio.NamingContainer as GridItem).Selected = radio.Checked;

		this.selectedDDI = grdViewMed.SelectedValues["DDI"].ToString();
	}

    protected void btnBack_Click(object sender, EventArgs e)
    {
        Response.Redirect(Constants.PageNames.REDIRECT_TO_ANGULAR + "?componentName=" + Constants.PageNames.REVIEW_HISTORY);
    }
    protected void btnSave_Click(object sender, EventArgs e)
    {
        string ddi = string.Empty;
        string medicationName = string.Empty;
        string strength = string.Empty;
        string strengthUOM = string.Empty;
        string dosageForm = string.Empty;
        string RouteofAdmin = string.Empty;
        string sigText = string.Empty;

        foreach (GridItem gridItem in grdViewMed.Items)
        {
			if (gridItem.OwnerTableView.DataKeyValues[gridItem.ItemIndex]["DDI"].ToString() == this.selectedDDI)
            {
                ddi = gridItem.OwnerTableView.DataKeyValues[gridItem.ItemIndex]["DDI"].ToString();
                medicationName = gridItem.OwnerTableView.DataKeyValues[gridItem.ItemIndex]["Name"].ToString();
                strength = gridItem.OwnerTableView.DataKeyValues[gridItem.ItemIndex]["Strength"].ToString();
                strengthUOM = gridItem.OwnerTableView.DataKeyValues[gridItem.ItemIndex]["StrengthUOM"].ToString();
                dosageForm = gridItem.OwnerTableView.DataKeyValues[gridItem.ItemIndex]["DosageForm"].ToString();
                RouteofAdmin = gridItem.OwnerTableView.DataKeyValues[gridItem.ItemIndex]["RouteofAdminCode"].ToString();                                
                TextBox txtSIG = (TextBox)gridItem.FindControl("txtSIG");
                if (txtSIG != null)
                    sigText = txtSIG.Text;
                break;
            }
        }

        if ( (!string.IsNullOrWhiteSpace(sigText)) && (System.Text.RegularExpressions.Regex.IsMatch(sigText, @"^([0-9a-zA-Z@.\s-,()&%/]{1,140})$")))
        {
            var rx = saveRx(ddi, medicationName, sigText, strength, strengthUOM, dosageForm, RouteofAdmin);
            string componentName = Allscripts.ePrescribe.Common.Constants.PageNames.REVIEW_HISTORY;
            string medName = medicationName.TrimEnd();
            string componentParameters =Server.UrlEncode( medName) + " has been successfully added to this patient\\'s record as a Self Reported Med.";
            var revHxPageRedirect = Constants.PageNames.REDIRECT_TO_ANGULAR + "?componentName=" + componentName + "&componentParameters=" + componentParameters;

            if (DoesHaveDurs(new List<Rx>{ new Rx(rx) }))
            {
                MasterPage.UpdatePatientActiveMeds();
                PageState[Constants.SessionVariables.DUR_GO_NEXT] = revHxPageRedirect;
                PageState[Constants.SessionVariables.DUR_GO_PREVIOUS] = Constants.PageNames.SELF_REPORTED_MED;
                Response.Redirect(Constants.PageNames.RX_DUR_REVIEW_MULTI_SELECT + "?SimpleShowDUR=True");
            }
            else
            {
                MasterPage.UpdatePatientActiveMeds();
                Response.Redirect(revHxPageRedirect);
            }
        }
        else
        {
            ucMessage.MessageText = "Invalid SIG. Please enter a valid SIG.";
            ucMessage.Visible = true;
            ucMessage.Icon = Controls_Message.MessageType.INFORMATION;
            txtSearchMed.Text = string.Empty;
        }
    }

    private bool DoesHaveDurs(List<Rx> rxs)
    {
        var durSettings = DURMedispanUtils.CloneDURSettings(DURSettings);
        durSettings.CheckDuplicateTherapy = YesNoSetting.Yes; //always perform a Dup Therapy check, regardless of site settings so the overlay appears.
        durSettings.CheckPerformDose = YesNoSetting.No;

        var request = DURMedispanUtils.ConstructDurCheckRequest(
            PageState.GetStringOrEmpty(Constants.SessionVariables.PatientDOB),
            PageState.GetStringOrEmpty(Constants.SessionVariables.Gender),
            rxs,
            PageState.Cast(Constants.SessionVariables.ACTIVEMEDDDILIST, new List<string>()),
            DurPatientAllergies,
            durSettings);

        var response = DURMSC.PerformDURCheck(request);
        if (DURMedispanUtils.DoesContainDUR(response))
        {
            PageState[Constants.SessionVariables.DURCheckResponse] = response;
            return true;
        }

        return false;
    }

    protected void btnSaveAndAddAnother_Click(object sender, EventArgs e)
    {
        string ddi = string.Empty;
        string medicationName = string.Empty;
        string strength = string.Empty;
        string strengthUOM = string.Empty;
        string dosageForm = string.Empty;
        string RouteofAdmin = string.Empty;
        string sigText = string.Empty;

        foreach (GridItem gridItem in grdViewMed.Items)
        {
            if (gridItem.OwnerTableView.DataKeyValues[gridItem.ItemIndex]["DDI"].ToString() == this.selectedDDI)
            {
                ddi = gridItem.OwnerTableView.DataKeyValues[gridItem.ItemIndex]["DDI"].ToString();
                medicationName = gridItem.OwnerTableView.DataKeyValues[gridItem.ItemIndex]["Name"].ToString();

                strength = gridItem.OwnerTableView.DataKeyValues[gridItem.ItemIndex]["Strength"].ToString();
                strengthUOM = gridItem.OwnerTableView.DataKeyValues[gridItem.ItemIndex]["StrengthUOM"].ToString();
                dosageForm = gridItem.OwnerTableView.DataKeyValues[gridItem.ItemIndex]["DosageForm"].ToString();
                RouteofAdmin = gridItem.OwnerTableView.DataKeyValues[gridItem.ItemIndex]["RouteofAdmin"].ToString();

                TextBox txtSIG = (TextBox)gridItem.FindControl("txtSIG");
                if (txtSIG != null)
                    sigText = txtSIG.Text;
                break;
            }
        }

        if (System.Text.RegularExpressions.Regex.IsMatch(sigText, @"^([0-9a-zA-Z@.\s-,()&%/]{1,140})$"))
        {
            var rx = saveRx(ddi, medicationName, sigText, strength, strengthUOM, dosageForm, RouteofAdmin);

            ucMessage.MessageText = medicationName + " has been successfully added to this patient's record as a Self Reported Med.";
            ucMessage.Visible = true;
            ucMessage.Icon = Controls_Message.MessageType.SUCCESS;

            if (DoesHaveDurs(new List<Rx> { new Rx(rx) }))
            {
                MasterPage.UpdatePatientActiveMeds();
                PageState[Constants.SessionVariables.DUR_GO_PREVIOUS] = PageState[Constants.SessionVariables.DUR_GO_NEXT] = Constants.PageNames.SELF_REPORTED_MED;
                Response.Redirect(Constants.PageNames.RX_DUR_REVIEW_MULTI_SELECT + "?SimpleShowDUR=True");
            }
            MasterPage.UpdatePatientActiveMeds();
        }
        else
        {
            ucMessage.MessageText = "Invalid SIG. Please enter a valid SIG.";
            ucMessage.Visible = true;
            ucMessage.Icon = Controls_Message.MessageType.INFORMATION;
        }

        txtSearchMed.Text = string.Empty;
    }

    private Prescription saveRx(string ddi, string medicationName, string sigText, string strength, string strengthUOM, string dosageForm, string RouteofAdmin)
    {
        string providerID = Guid.Empty.ToString();

        Prescription rx = new Prescription();

        //if (Session["IsDelegateProvider"] != null && Convert.ToBoolean(Session["IsDelegateProvider"]))
        //{
        //    if (Session["PHYSICIANID"] != null)
        //        providerID = Session["PHYSICIANID"].ToString();
        //}
        //else
        //{
        //    if (Session["USERID"] != null)
        //        providerID = Session["USERID"].ToString();
        //}

        string extFacilityCode = null;
        if (Session["ExtFacilityCd"] != null)
            extFacilityCode = Session["ExtFacilityCd"].ToString();

        string extGroupID = null;
        if (Session["ExtGroupID"] != null)
            extGroupID = Session["ExtGroupID"].ToString();

        string rxID = Guid.NewGuid().ToString();

        rx.SetHeaderInformation(
           Session["LicenseID"].ToString(), rxID, DateTime.UtcNow.ToString(), Session["PATIENTID"].ToString(),
           providerID, Guid.Empty.ToString(), null, null, null, Constants.PrescriptionType.REPORTED, false,
           "Patient self reported", Convert.ToInt32(Session["SITEID"]), Constants.ERX_NOW_RX, null, base.DBID);

        rx.AddMedication(Session["LicenseID"].ToString(), 0, ddi, medicationName, RouteofAdmin, dosageForm, strength, strengthUOM, Guid.Empty.ToString(), sigText,
                    0, 0, 0, null, 0, null, 0, null, false, DateTime.Now.ToString(), Constants.PrescriptionStatus.NEW, Constants.PrescriptionTransmissionMethod.RECORDED, null, 0, null, 0, null,
                    FormularyStatus.NONE, "N", "N", "N", "N", "N", null, null, null, 0, "Q", null, null, Session["UserID"].ToString(), PrescriptionWorkFlow.STANDARD, extFacilityCode, extGroupID,
					false, false, (int)SigTypeEnum.SigTypeFreeForm);

        rx.Save(Convert.ToInt32(Session["SITEID"]), Session["LICENSEID"].ToString(), Session["USERID"].ToString(), base.DBID);
            return rx;
    }

	private string selectedDDI
	{
		get
		{
			string selectedDDI = null;

			if (ViewState["SelectedDDI"] != null)
			{
				selectedDDI = ViewState["SelectedDDI"].ToString();
			}

			return selectedDDI;
		}
		set
		{
			ViewState["SelectedDDI"] = value;
		}
	}
}

}