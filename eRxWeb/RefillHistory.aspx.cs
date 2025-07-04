//Revision History
//HA/JJ 12-Oct-2k6 134 Workflow--- No "Cancel" button in Prescrition History" Page for a Nurse.
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
using Allscripts.ePrescribe.Common;
using eRxWeb.AppCode.StateUtils;

namespace eRxWeb
{
public partial class RefillHistory : BasePage 
{
    System.Collections.Generic.Dictionary<String, String> rxIDs = new System.Collections.Generic.Dictionary<string, string>();
        
    protected void Page_Load(object sender, EventArgs e)
    {
        
        if (!Page.IsPostBack)
        {
            txtMed.Attributes.Add("onkeypress", "return LimitInput(this,'50', event);");
            txtMed.Attributes.Add("onpaste", "return LimitPaste(this,'50');");
            txtPatientComments.Attributes.Add("onkeypress", "return LimitInput(this,'50', event);");
            txtPatientComments.Attributes.Add("onpaste", "return LimitPaste(this,'50');");

            int tasks = 0;
      
            ((PhysicianMasterPage)Master).toggleTabs("patient", tasks);
                


			int task = 0;
            if (Session["LICENSEID"] != null && Session["PATIENTID"] != null)
                task = Allscripts.Impact.Provider.GetRefillTaskCount(Session["LICENSEID"].ToString(), Session["PATIENTID"].ToString(), (int)Constants.PrescriptionTaskStatus.NEW, Session["USERID"].ToString(), base.DBID);
            if(task >0)
                Literal1.Text = task + " Refill requests " + "are still being reviewed.";
        }
    }
    
    protected void grdReviewHistory_RowDataBound(object sender, GridViewRowEventArgs e)
    {
		if (e.Row.RowType == DataControlRowType.DataRow)
		{
			string controlledSubstanceCode = grdReviewHistory.DataKeys[e.Row.RowIndex].Values["ControlledSubstanceCode"].ToString().Trim();
			string pharm = "Y";

			CheckBox chk = e.Row.FindControl("Select") as CheckBox;
			if (chk != null)
			{
				bool isContolledSubstance = !(string.IsNullOrEmpty(controlledSubstanceCode) || controlledSubstanceCode.Equals("U"));

                Constants.PrescriptionStatus rxStatus = (Constants.PrescriptionStatus)Convert.ToInt32(grdReviewHistory.DataKeys[e.Row.RowIndex].Values["Status"].ToString());

				string providerID = grdReviewHistory.DataKeys[e.Row.RowIndex].Values[1].ToString();
				chk.Attributes.Add("onclick", "SelectPhysician('" + providerID + "',this,'" + grdReviewHistory.ClientID + "','" + btnSubmitRefillRequest.ClientID + "','" + pharm + "')");
			}
			e.Row.Attributes.Add("Pharmacy", pharm);
		}
    }
    protected void grdReviewHistory_RowCreated(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.Pager)
        {
            Table t = e.Row.Cells[0].Controls[0] as Table;
            TableRow r = t.Rows[0];
            foreach (TableCell cell in r.Cells)
            {
                object ctl = cell.Controls[0];
                if (ctl is Label)
                {
                    ((Label)ctl).Text = "Page " + ((Label)ctl).Text;
                    ((Label)ctl).CssClass = "CurrentPage";
                }
                else
                {

                    ((LinkButton)ctl).Text = "[" + ((LinkButton)ctl).Text + "]";

                }
            }
        }
        if (e.Row != null && e.Row.RowType == DataControlRowType.Header)
        {
            foreach (TableCell cell in e.Row.Cells)
            {
                if (cell.HasControls())
                {
                    LinkButton button = cell.Controls[0] as LinkButton;
                    if (button != null)
                    {
                        Image image = new Image();
                        image.ImageUrl = "images/sort-sortable.gif";
                        if (((System.Web.UI.WebControls.GridView)(sender)).SortExpression == button.CommandArgument)
                        {
                            if (((System.Web.UI.WebControls.GridView)(sender)).SortDirection == SortDirection.Ascending)
                                image.ImageUrl = "images/sort-ascending.gif";
                            else
                                image.ImageUrl = "images/sort-Descending.gif";
                        }
                        cell.Controls.Add(image);
                    }
                }
            }
        }
    }
    protected void RxHistoryObjDataSource_Selected(object sender, ObjectDataSourceStatusEventArgs e)
    {
        //EAK--WE ARE NO LONGER SUPPORTING NON-CHEETAH MEDS.  DO NOT ALTER THIS CODE WITHOUT CONSULTING WITH EAK.
        
        if (((DataSet)e.ReturnValue).Tables[0].Rows.Count == 0)
        {
            //PnlMed.Visible = true;
            //btnRefillProcess.Visible = true;
            //btnSelectPharmacy.Visible = false;
            btnSelectPharmacy.Visible = false;
            txtPatientComments.Visible = false;
            //btnRefillProcess.Visible = false;
            ddlProviderList.Visible = false;
            PnlMed.Visible = false;
            lblSelPatDoc.Visible = false;
            lblPatComments.Visible = false;
            lblMaxChar.Visible = false;
        }        
        
        //EAK DO NOT ALTER ABOVE CODE

    }
    protected void btnSelectPharmacy_Click(object sender, EventArgs e)
    {
		string sourceRxID = GetSelectedRxID();

		if (!string.IsNullOrEmpty(sourceRxID))
		{
			System.Collections.Generic.List<string> rxIDList = new System.Collections.Generic.List<string>();
			rxIDList.Add(sourceRxID);
			Session["RXIDLIST"] = rxIDList;
			Session["PHYSICIANID"] = ddlProviderList.SelectedValue;
			Session["PATIENTCOMMENTS"] = txtPatientComments.Text;

            Response.Redirect(Constants.PageNames.PHARMACY);
		}
    }
    //JJ 134
    protected void btnCancel_Click(object sender, EventArgs e)
    {
        Server.Transfer(Constants.PageNames.SELECT_PATIENT);
    }

    protected void grdReviewHistory_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        
    }

	private string GetSelectedRxID()
	{
		string selectedRxID = string.Empty;
		for (int index = 0; index < grdReviewHistory.Rows.Count; index++)
		{
			CheckBox chk = grdReviewHistory.Rows[index].FindControl("Select") as CheckBox;
			if (chk != null && chk.Checked)
			{
				selectedRxID = grdReviewHistory.DataKeys[index].Values[0].ToString();
				break;
			}
		}

		return selectedRxID;
	}

    protected void SubmitRefillRequest_Click(object sender, EventArgs e)
    {
		string sourceRxID = GetSelectedRxID();

		if (!string.IsNullOrEmpty(sourceRxID))
		{
			string pharmacyID = System.Guid.Empty.ToString();
			string patientID = string.Empty;

			Session["PHYSICIANID"] = ddlProviderList.SelectedValue;
			Session["PATIENTCOMMENTS"] = txtPatientComments.Text;

			string physicianId = ddlProviderList.SelectedValue;
			string PatientComments = txtPatientComments.Text;
			if (Session["PATIENTID"] != null)
				patientID = Session["PATIENTID"].ToString();
			else
				patientID = System.Guid.Empty.ToString();

			string licenseID = string.Empty;
			if (Session["LICENSEID"] != null)
				licenseID = Session["LICENSEID"].ToString();

			Prescription sourcePrescription = null;
			Prescription targetPrescription = null;
			
			string message = string.Empty;

			try
			{
				sourcePrescription = new Prescription();
			    sourcePrescription.LoadFromExistingMed(sourceRxID, DurInfo.RetrievePrescriptionDurHandler(PageState), SessionLicenseID, SessionUserID, base.DBID);

                targetPrescription = sourcePrescription.RenewPrescriptionMed(physicianId, Session["PERFORM_FORMULARY"].ToString(), DurInfo.RetrievePrescriptionDurHandler(PageState), SessionLicenseID, SessionUserID, Convert.ToInt32(Session["SITEID"]), base.DBID);
                    //Now the prescription is created successfully, need to pend the prescription
                    if (targetPrescription != null)
				{
                    /*
					Prescription.PendPrescription(targetPrescription.ID, 1, licenseID, Session["USERID"].ToString());
					//Also create task for the physician with the target prescription
					taskID =
						TaskManager.CreateTask(
							ServiceTaskType.RX_REFILL,
							targetPrescription.ID,
							physicianId,
							pharmacyID,
							patientID,
							PatientComments,
                            (int)PrescriptionTaskStatus.NEW,
							"",
							licenseID,
							Session["USERID"].ToString());
                    */
                    Prescription.SendRefillRequestToPhysician(targetPrescription.ID, licenseID, Session["USERID"].ToString(), physicianId, PatientComments, base.DBID);
					message = "Renewed prescription is pending for provider approval.";
				}
				else
				{
					message = "Failed to renew the prescription.";
				}
			}
			catch (Exception)
			{
				//If any exception happend, then we need to undo the changes. 
				if (targetPrescription != null)
				{
					targetPrescription.Delete();
				}

				//TODO
				//Need to implement logic to revert the prescription back to the original status instead of leaving it at complete status. 

				message = "Failed to renew the prescription.";
			}

			Server.Transfer(Constants.PageNames.SELECT_PATIENT + "?Msg=" + Server.UrlEncode(message));
		}
	}
}

}