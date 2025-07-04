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
using System.Collections.Generic;

namespace eRxWeb
{
public partial class PatientDiagnosis : BasePage 
{
    protected void Page_Load(object sender, EventArgs e)
    {
        int tasks = 0;
        
        ((PhysicianMasterPage)Master).toggleTabs("patient", tasks);        

        btnAddDiagnosis.Visible = Session["AddDiagnosis"].ToString().Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase);

        if (!Page.IsPostBack)
        {
            BindData();
            base.AuditLogPatientInsert(ePrescribeSvc.AuditAction.PATIENT_DX_VIEW, base.SessionPatientID);
        }
    }
    protected void Page_LoadComplete(object sender, EventArgs e)
    {
        ((PhysicianMasterPage)Master).displayPatientHeader();
    }
    protected void btnCancel_Click(object sender, EventArgs args)
    {
        DefaultRedirect();
    }
    protected void grdViewPatDiagnosis_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        string[] cmdArg = e.CommandArgument.ToString().Split(';');
        switch (e.CommandName)
        {            
            case "Inactivate":
                //Checking if the Diagnosis is coming from ATP
                if (cmdArg[1] != null && !(string.IsNullOrEmpty(cmdArg[1])) && cmdArg[1].Equals("ATP", StringComparison.OrdinalIgnoreCase))
                {
                    Allscripts.ePrescribe.Data.IPatientDiagnosisProvider patientDiagnosis = new Allscripts.ePrescribe.Data.PatientDiagnosisProvider();
                    patientDiagnosis.UpdatePatientDiagnosis(cmdArg[0].ToString(), (int)Constants.PatientProblemStatus.InActive, Session["USERID"].ToString(), DBID);
                }
                else
                {
                    Allscripts.Impact.PatientDiagnosis.Discontinue(e.CommandArgument.ToString(), Session["USERID"].ToString(), base.DBID);
                }
                BindData();
                base.AuditLogPatientInsert(ePrescribeSvc.AuditAction.PATIENT_DX_MODIFY, base.SessionPatientID);
                break;
            case "EIE":
                //Checking if the Diagnosis is coming from ATP
                if (cmdArg[1] != null && !(string.IsNullOrEmpty(cmdArg[1])) && cmdArg[1].Equals("ATP", StringComparison.OrdinalIgnoreCase))
                {
                    Allscripts.ePrescribe.Data.IPatientDiagnosisProvider patientDiagnosis = new Allscripts.ePrescribe.Data.PatientDiagnosisProvider();
                    patientDiagnosis.UpdatePatientDiagnosis(cmdArg[0].ToString(), (int)Constants.PatientProblemStatus.EIE, Session["USERID"].ToString(), DBID);
                }
                else
                {
                    Allscripts.Impact.PatientDiagnosis.EIE(e.CommandArgument.ToString(), Session["USERID"].ToString(), Session["LICENSEID"].ToString(), base.DBID);
                }
                BindData();
                ((PhysicianMasterPage)Master).displayPatientHeader();
                base.AuditLogPatientInsert(ePrescribeSvc.AuditAction.PATIENT_DX_MODIFY, base.SessionPatientID);
                break;
        }
    }
    void BindData()
    {
        if (Session["PATIENTID"] != null)  
        {
            LoadPatientDiagnosis();

            string activeDiagnosis = (new Allscripts.ePrescribe.Data.PatientDiagnosisProvider()).GetActiveDiagnosisDisplayString(Session["PATIENTID"].ToString(), DBID);
            if (!string.IsNullOrWhiteSpace(activeDiagnosis) && activeDiagnosis.Length > 0)
            {
                Session["ACTIVEDX"] = activeDiagnosis.ToString();
            }
            else
            {
                Session.Remove("ACTIVEDX");
            }   
            
        }
    }

    protected void grdViewPatDiagnosis_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        e.Cancel = true;
    }
    protected void grdViewPatDiagnosis_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        grdViewPatDiagnosis.PageIndex = e.NewPageIndex;
        BindData();
    }


    protected void grdViewPatDiagnosis_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        e.Cancel = true;
    }
    protected void grdViewPatDiagnosis_RowCreated(object sender, GridViewRowEventArgs e)
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
    protected void grdViewPatDiagnosis_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            string status = e.Row.Cells[6].Text;
            if (status == "EIE")
            {
                DisableInactivateAndEIELinks(e);
                e.Row.CssClass = "txtEIE";
            }
            else if (status == "Inactive / Resolved")
            {
                DisableInactivateAndEIELinks(e);
            }
            

        }
    }

    private static void DisableInactivateAndEIELinks(GridViewRowEventArgs e)
    {
        LinkButton lnkbtn = e.Row.FindControl("lnkbtnInactivate") as LinkButton;
        lnkbtn.Enabled = false;
        lnkbtn = e.Row.FindControl("lnkbtnEIE") as LinkButton;
        lnkbtn.Enabled = false;
    }
    protected void btnAddDiagnosis_Click(object sender, EventArgs e)
    {
        Server.Transfer(Constants.PageNames.ADD_PATIENT_DIAGNOSIS);
    }

    protected void rb(object sender, EventArgs e)
    {
        Server.Transfer(Constants.PageNames.ADD_PATIENT_DIAGNOSIS);
    }

    protected void rbActive_CheckedChanged(object sender, EventArgs e)
    {
        LoadPatientDiagnosis();
    }

    protected void rbInActive_CheckedChanged(object sender, EventArgs e)
    {
        LoadPatientDiagnosis();
    }

    protected void rbAll_CheckedChanged(object sender, EventArgs e)
    {
        LoadPatientDiagnosis();
    }

    public void LoadPatientDiagnosis()
    {

        List<Allscripts.ePrescribe.Data.PatientDiagnosis> diagnosisList= null;
        Allscripts.ePrescribe.Data.IPatientDiagnosisProvider patientDiagnosis = new Allscripts.ePrescribe.Data.PatientDiagnosisProvider();
        if (rbInActive.Checked)
        {
            diagnosisList = (patientDiagnosis).GetInactiveDiagnosis(SessionPatientID, DBID);
            grdViewPatDiagnosis.Columns[2].Visible = true;           
            grdViewPatDiagnosis.Columns[7].Visible = false;
        }
        else if (rbAll.Checked)
        {
            diagnosisList = (patientDiagnosis).GetAllDiagnosis(SessionPatientID, DBID);
            grdViewPatDiagnosis.Columns[2].Visible = true;
            grdViewPatDiagnosis.Columns[7].Visible = true;
        }
        else
        {
            diagnosisList = (patientDiagnosis).GetActiveDiagnosis(SessionPatientID, DBID);
            grdViewPatDiagnosis.Columns[2].Visible = false;           
            grdViewPatDiagnosis.Columns[7].Visible = true;
        }

        grdViewPatDiagnosis.DataSource = diagnosisList;
        grdViewPatDiagnosis.DataBind();

        

    }

}

}