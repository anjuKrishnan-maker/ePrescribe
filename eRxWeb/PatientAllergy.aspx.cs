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
using Allscripts.ePrescribe.Common;
using Allscripts.Impact;

namespace eRxWeb
{
public partial class PatientAllergy : BasePage 
{
    protected void Page_Load(object sender, EventArgs e)
    {
        btnAddAllergy.Visible = btnNKA.Visible = Session["AddAllergy"].ToString().Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase);

        if (!Page.IsPostBack)
        {
            BindData();
        }

    }
    protected void Page_LoadComplete(object sender, EventArgs e)
    {
        LinkButton lnk = Master.FindControl("lnkPatient") as LinkButton;
        if (lnk != null)
            lnk.Enabled = false;
        lnk = Master.FindControl("lnkMedication") as LinkButton;
        if (lnk != null)
            lnk.Enabled = false;
        lnk = Master.FindControl("lnkDiagnosis") as LinkButton;
        if (lnk != null)
            lnk.Enabled = false;

        if (Session["SSOMode"] != null)
        {
            if (Session["SSOMode"].ToString() == Constants.SSOMode.PATIENTLOCKDOWNMODE)
            {
                ((PhysicianMasterPage)Master).hideTabs();
            }
            else
            {
                int tasks = 0;
            
                ((PhysicianMasterPage)Master).toggleTabs("patient", tasks);
            }
        }
        else
        {
            int tasks = 0;
          
            ((PhysicianMasterPage)Master).toggleTabs("patient", tasks);
        }

        base.AuditLogPatientInsert(ePrescribeSvc.AuditAction.PATIENT_ALLERGY_VIEW, base.SessionPatientID);
    }

    private int GetePATaskCount()
    {
        int returnValue = 0;

        if (base.IsPOBUser)
        {
            returnValue = Allscripts.Impact.ePA.GetePATaskCount(base.SessionLicenseID, base.SessionDelegateProviderID, base.SessionUserID, base.SessionPatientID, base.SessionSSOMode, base.DBID, base.SessionUserID);
        }
        else
        {
            returnValue = Allscripts.Impact.ePA.GetePATaskCount(base.SessionLicenseID, string.Empty, string.Empty, base.SessionPatientID, base.SessionSSOMode, base.DBID, base.SessionUserID);
        }

        return returnValue;
    }

    void BindData()
    {
		btFilterAllergy_CheckedChanged(null, null);
    }
    protected void grdViewPatAllergy_RowCommand(object sender, GridViewCommandEventArgs e)
    {

        switch (e.CommandName)
        {
            case "Edit":
                Server.Transfer(Constants.PageNames.UPDATE_ALLERGY + "?PatientAllergyID=" + e.CommandArgument.ToString());
                break;
            case "EIE":
                Allscripts.Impact.PatientAllergy.EIE(e.CommandArgument.ToString(), Session["USERID"].ToString(), base.DBID);
                BindData();
				((PhysicianMasterPage)Master).displayPatientHeader();
                PageState[Constants.SessionVariables.DurPatientAllergies] = DURMedispanUtils.RetrieveAllergy(Session["PATIENTID"].ToString(), Session["LICENSEID"].ToString(), Session["USERID"].ToString(), base.DBID);

                base.AuditLogPatientInsert(ePrescribeSvc.AuditAction.PATIENT_ALLERGY_MODIFY, base.SessionPatientID);
                break;
             
        }
        
    }

    public string getAllergyReaction(string PatientAllergyID)
    {
        System.Text.StringBuilder reaction = new System.Text.StringBuilder();
        
        DataTable dt = Allscripts.Impact.PatientAllergy.LoadPatientAllergyReaction(PatientAllergyID, base.DBID);
        foreach (DataRow dr in dt.Rows)
        {
            reaction.Append(dr["Name"].ToString() + ",");
        }
                
        if (reaction.Length > 0)
            return reaction.ToString().Substring(0, reaction.Length - 1);
        else
            return reaction.ToString();
        
    }
    
    protected void grdViewPatAllergy_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        e.Cancel = true;
    }
    protected void grdViewPatAllergy_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        e.Cancel = true;
    }
    
    protected void grdViewPatAllergy_RowDataBound(object sender, GridViewRowEventArgs e)
    {

        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            string status = e.Row.Cells[3].Text;
            if (status == "E")
            {
                //This code is added to explicitly assign the CssClass for each of the cells.
                //to address the issues in the FireFox Browser Feb 06
                //******************Start*********************
                e.Row.Cells[0].CssClass = "txtEIE";
                Label lblbtn = e.Row.FindControl("lblReaction") as Label;
                lblbtn.CssClass = "txtEIE"; // For the Reaction column
                e.Row.Cells[2].CssClass = "txtEIE";
                e.Row.Cells[3].CssClass = "txtEIE";
                e.Row.Cells[4].CssClass = "txtEIE";
                e.Row.Cells[5].CssClass = "txtEIE";
                //******************End*********************
                LinkButton lnkbtn = e.Row.FindControl("lnkbtnEdit") as LinkButton;
                lnkbtn.Enabled = false;
                lnkbtn = e.Row.FindControl("lnkbtnEIE") as LinkButton;
                lnkbtn.Enabled = false;
                // e.Row.CssClass = "txtEIE"; Commented.
            }

            if (grdViewPatAllergy.DataKeys[e.Row.RowIndex]["AllergyType"].ToString().ToUpper() == "C"
                && grdViewPatAllergy.DataKeys[e.Row.RowIndex]["ClassActiveStatus"].ToString().ToUpper() == "N")
            {
                e.Row.Cells[0].ForeColor = System.Drawing.Color.Red;

                Label lblbtn = e.Row.FindControl("lblReaction") as Label;
                lblbtn.ForeColor = System.Drawing.Color.Red; // For the Reaction column

                e.Row.Cells[2].ForeColor = System.Drawing.Color.Red;
                e.Row.Cells[3].ForeColor = System.Drawing.Color.Red;
                e.Row.Cells[4].ForeColor = System.Drawing.Color.Red;
                e.Row.Cells[5].ForeColor = System.Drawing.Color.Red;
            }
        }
    }
    protected void btnAddAllergy_Click(object sender, EventArgs e)
    {
        Response.Redirect(Constants.PageNames.ADD_ALLERGY);
    }

    protected void grdViewPatAllergy_RowCreated(object sender, GridViewRowEventArgs e)
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
    protected void grdViewPatAllergy_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        grdViewPatAllergy.PageIndex =  e.NewPageIndex;
        BindData();
    }
	protected void btnBack_Click(object sender, EventArgs args)
	{
        if (Session["PatientAllergyCalledFrom"] != null)
        {
            string redirecturl = Session["PatientAllergyCalledFrom"].ToString();
            Session.Remove("PatientAllergyCalledFrom");
            Response.Redirect(redirecturl);
        }
        else
        {
            DefaultRedirect();
        }
	}
	protected void btFilterAllergy_CheckedChanged(object sender, EventArgs args)
	{

		if (Session["PATIENTID"] != null)
		{
			DataView allergiesDV = (DataView)PatAllergyObjDataSource.Select();
			if (rbActive.Checked)
				allergiesDV.RowFilter = "Active = 'Y'";
			else if (rbInactive.Checked)
				allergiesDV.RowFilter = "Active = 'N'";

			//Update Active allergies
			DataRow[] PatAllergy = allergiesDV.Table.Select("Active='Y'");
			string allergy = "";
			foreach (DataRow dr in PatAllergy)
			{
				allergy = allergy + dr["AllergyName"].ToString() + ",";
			}
			if (allergy.EndsWith(","))
				allergy = allergy.Substring(0, allergy.Length - 1);
			Session["ALLERGY"] = allergy;

			grdViewPatAllergy.DataSource = allergiesDV;

			grdViewPatAllergy.DataBind();
		}
	}
	protected void btnNKA_Click(object sender, EventArgs e)
	{
		if (Session["PATIENTID"] != null)
		{
			try
			{
				Allscripts.Impact.Patient.MarkNKA(Session["PATIENTID"].ToString(), Session["LICENSEID"].ToString(), Session["USERID"].ToString(), base.DBID);
				Session["PATIENTNKA"] = "Y";
				//Remove the patient allergy
				Session.Remove("ALLERGY");
                Session.Remove(Constants.SessionVariables.DurPatientAllergies);
                ((PhysicianMasterPage)Master).displayPatientHeader();

                base.AuditLogPatientInsert(ePrescribeSvc.AuditAction.PATIENT_ALLERGY_CREATED, base.SessionPatientID);

				grdViewPatAllergy.DataBind();
			}
			catch (Exception)
			{

			}
		}
	}
	protected void grdViewPatAllergy_OnDataBinding(object sender, EventArgs e)
	{
		if (Session["PATIENTNKA"] != null)
		{
			if (Session["PATIENTNKA"].ToString().Equals("Y"))
				grdViewPatAllergy.EmptyDataText = "No Known Allergies";
			else
				grdViewPatAllergy.EmptyDataText = "No Allergies";
		}
	}
}

}