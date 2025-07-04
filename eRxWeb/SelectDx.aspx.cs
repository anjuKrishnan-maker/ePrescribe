/******************************************************************************
**Change History
*******************************************************************************
**Date:      Author:                    Description:
**-----------------------------------------------------------------------------
**09/17/2009 Anand Kumar Krishnan       Defect#2689 New overlay validation message is coded 
 *                                      when user clicks on "Select Med" button without selecting the DX
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
using Allscripts.ePrescribe.Common;
using eRxWeb.ServerModel;
using System.Web.Script.Serialization;

namespace eRxWeb
{
public partial class SelectDx : BasePage 
{
    protected void Page_Load(object sender, EventArgs e)
    {
        this.Form.DefaultButton = btnGo.UniqueID;

        Helper.SetHelpTextForPane(paneHelp, this.AppRelativeVirtualPath);

            if (!Page.IsPostBack)
        {
            pnlSearchDiagnosis.Visible = Session["AddDiagnosis"].ToString().Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase);

            }

        //if (SessionLicense.LicenseDeluxeStatus != Constants.DeluxeFeatureStatus.On)
        //{
        //    // Get AD placement from TIE service.
        //    GetAdPlacement(Constants.TIELocationPage.SelectDx_Page);
        //}
        if (SessionLicense.LicenseDeluxeStatus != Constants.DeluxeFeatureStatus.On)
        {
            divHideTools_Help.Style.Add("display", "none");// Hiding HelpContent if the User is a Basic user (to highlight LogRx ads)
        }

    }
   
    protected void Page_LoadComplete(object sender, EventArgs e)
    {
        LinkButton lnk = Master.FindControl("lnkDiagnosis") as LinkButton;
        if (lnk != null)
            lnk.Enabled = false;
        ((PhysicianMasterPage)Master).hideTabs();

    }
    protected void btnGo_Click(object sender, EventArgs e)
    {
		grdViewDx.PageIndex = 0;
		grdViewDx.DataBind();
    }
    protected void grdViewDx_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        //if (e.Row.RowType == DataControlRowType.DataRow)
        //{
        //    e.Row.Style["cursor"] = "pointer";
        //    e.Row.Attributes.Add("onclick", ClientScript.GetPostBackEventReference(grdViewDx,
        //          "Select$" + e.Row.RowIndex.ToString()));

        //}
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            e.Row.Style["cursor"] = "pointer";
            e.Row.Attributes.Add("onclick", "onRowClick(this)");
        }
    }
  
    
    protected void grdViewDx_RowCreated(object sender, GridViewRowEventArgs e)
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
    protected void btnReviewHistory_Click(object sender, EventArgs e)
    {
        if (Session["PATIENTID"] != null)
        {
            string componentName = Allscripts.ePrescribe.Common.Constants.PageNames.REVIEW_HISTORY;
            string componentParameters = string.Empty;
                Response.Redirect(Constants.PageNames.REDIRECT_TO_ANGULAR + "?componentName=" + componentName + "&componentParameters=" + componentParameters);
            }
    }
    protected void btnSelectMed_Click(object sender, EventArgs e)
    {
        string selectedValue = Request.Form["rdSelectRow"];
        foreach (GridViewRow row in grdViewDx.Rows)
        {
            if (grdViewDx.Rows[row.RowIndex].Cells[1].Text == selectedValue)
            {
                grdViewDx.SelectedIndex = row.RowIndex;
                break;
            }
        }

        if (grdViewDx.SelectedIndex > -1)
        {
            Session["ICD10CODE"] = grdViewDx.SelectedRow.Cells[1].Text;
            Session["DIAGNOSIS"] = grdViewDx.SelectedRow.Cells[2].Text;
            Response.Redirect(Constants.PageNames.REDIRECT_TO_ANGULAR + "?componentName=" + Constants.PageNames.SELECT_MEDICATION);
        }
        else
        {
            modalSelectMedPopup.Show();
        }
    }
  
    protected void btnContinueWithoutDX_Click(object sender, EventArgs e)
    {
          Response.Redirect(Constants.PageNames.REDIRECT_TO_ANGULAR + "?componentName=" + Constants.PageNames.SELECT_MEDICATION);
    }

    protected void btnCancel_ServerClick(object sender, EventArgs e)
    {
        base.ClearPatientInfo();
        SelectPatientComponentParameters selectPatientComponentParameters = new SelectPatientComponentParameters
        {
            PatientId = PageState.GetStringOrEmpty(Constants.SessionVariables.PatientId)
        };
        RedirectToSelectPatient(null, selectPatientComponentParameters);
    }
    
    protected void DxObjDataSource_Selecting(object sender, ObjectDataSourceSelectingEventArgs e)
    {
        if (e.InputParameters["Phrase"].ToString() == "Enter Diagnosis or ICD9")
        {
            e.InputParameters["Phrase"] = "";
           //e.Cancel = true;
        }
    }
}

}