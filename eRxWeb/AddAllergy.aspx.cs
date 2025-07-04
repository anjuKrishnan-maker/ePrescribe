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
using System.Collections.Generic;
using Allscripts.Impact;
using Allscripts.ePrescribe.Common;

namespace eRxWeb
{
public partial class AddAllergy : BasePage
{
    #region Handled Events
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            rbClass.Attributes.Add("onclick", "setContextKey(this)");
            rbClass.Attributes.Add("value", "class");

            rbMedication.Attributes.Add("onclick", "setContextKey(this)");
            rbMedication.Attributes.Add("value", "meds");

            txtSearchText.Focus();

            aceMed.ContextKey = "class";

            Helper.SetHelpTextForPane(paneHelp, this.AppRelativeVirtualPath);

            grdViewMed.DataSourceID = "";

            GetReactions();

            DateTime dt = DateTime.Now;
            txtStartDate.Text = dt.ToString("d");
            rvStartDate.MaximumValue = dt.ToString("d");

            if (Session["PATIENTDOB"] != null)
            {
                rvStartDate.MinimumValue = Session["PATIENTDOB"].ToString();
                rvStartDate.ErrorMessage = string.Format(rvStartDate.ErrorMessage, rvStartDate.MinimumValue, rvStartDate.MaximumValue);
            }
            else
            {
                Server.Transfer(Constants.PageNames.LOGOUT + "?TIMEOUT=YES");
            }
        }
        else
        {
            if (rbClass.Checked)
            {
                aceMed.ContextKey = "class";
            }
            else
            {
                aceMed.ContextKey = "meds";
            }
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
                    ((PhysicianMasterPage)Master).toggleTabs("patient");
                }        
        }
        else
        {
            int tasks = 0;   
            ((PhysicianMasterPage)Master).toggleTabs("patient", tasks);
        }
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        grdViewMed.DataSourceID = "MedObjDataSource";
    }

    protected void MedObjDataSource_Selecting(object sender, ObjectDataSourceSelectingEventArgs e)
    {
        if (e.InputParameters["MedName"] == null) // Check if the txtsearchmed field is blank then dont get any records
        {
            e.Cancel = true;
        }
    }

    protected void grdViewMed_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            e.Row.Style["cursor"] = "hand";
            e.Row.Attributes.Add("onclick", "onRowClick(this)");
        }
    }

    protected void grdViewMed_RowCreated(object sender, GridViewRowEventArgs e)
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

    protected void btnAddAllergy_Click(object sender, EventArgs e)
    {
        if (IsValid == true)
        {
            string selectedValue = Request.Form["rbSelectRow"];
            foreach (GridViewRow row in grdViewMed.Rows)
            {
                if (grdViewMed.DataKeys[row.RowIndex].Value.ToString() == selectedValue)
                {
                    grdViewMed.SelectedIndex = row.RowIndex;
                    break;
                }
            }

            if (grdViewMed.SelectedDataKey != null)
            {
                string patientAllergyID = System.Guid.NewGuid().ToString();
                string patientId = Session["PATIENTID"].ToString();
                string userid = Session["USERID"].ToString();
                string allergyCat = string.Empty;
                string allergyType = string.Empty;
                string allergyID = grdViewMed.SelectedDataKey.Value.ToString();  // This is DDI value.
                string start = txtStartDate.Text.Trim();
                string end = txtEndDate.Text.Trim();
                string otherNote = txtOther.Text.Trim();
                bool active = chkActive.Checked;
                bool other = (txtOther.Text.Trim().Length > 0 ? true : false);
                List<string> reactions = new List<string>();   //List of ReactionID 

                // set the allergy type parameter
                if (rbClass.Checked)
                {
                    allergyType = "C";
                }
                else if (rbMedication.Checked)
                {
                    allergyType = "M";
                }
                
                // set the allerty category parameter
                if (rbAllergy.Checked)
                {
                    allergyCat = "A"; //Allergy
                }
                else if (rbIntolerance.Checked)
                {
                    allergyCat = "I"; //Intolerance
                }

                // get all the selected feactions
                if (lstReactions.SelectedIndex > -1)
                {
                    foreach (ListItem li in lstReactions.Items)
                    {
                        if (li.Selected == true)
                        {
                            reactions.Add(li.Value.ToString());
                        }
                    }
                }

                Allscripts.Impact.PatientAllergy.Save(patientAllergyID, patientId, allergyType, allergyID, active, start, end, reactions.ToArray(), other, otherNote, Session["USERID"].ToString(), Session["LICENSEID"].ToString(), allergyCat, base.DBID);

                base.AuditLogPatientInsert(ePrescribeSvc.AuditAction.PATIENT_ALLERGY_CREATED, patientId);

				Session["PATIENTNKA"] = "N";
                Session["ALLERGY"] = GetAllAllergies(Session["PATIENTID"].ToString());

                PageState[Constants.SessionVariables.DurPatientAllergies] = DURMedispanUtils.RetrieveAllergy(SessionPatientID, SessionLicenseID, SessionUserID, DBID);
                Server.Transfer(Constants.PageNames.PATIENT_ALLERGY);
            }
        }
    }
    #endregion

    #region Methods

    // Function to Load all the Reactions that are Active from the DB
    private void GetReactions()
    {
        DataSet dsReactions;
        dsReactions = new Allergy().GetAllergyReactions(base.DBID);
        if (dsReactions.Tables[0].Rows.Count > 0)
        {
            //Get and Bind the dataset to the control..
            lstReactions.DataTextField = "Name";
            lstReactions.DataValueField = "AllergenReactionID";
            lstReactions.DataSource = dsReactions.Tables[0];
            lstReactions.DataBind();
        }
    }

    // get ALL the allergies for a specific patient
    protected string GetAllAllergies(string patId)
    {
        Patient pat = new Patient(patId, true, false, Session["LICENSEID"].ToString(), Session["USERID"].ToString(), base.DBID);
        DataSet patds = pat.DS;
        string allergy = "";

        if (patds.Tables["PatientAllergy"].Rows.Count > 0)
        {
            DataRow[] PatAllergy = patds.Tables["PatientAllergy"].Select("Active='Y'");

            foreach (DataRow dr in PatAllergy)
            {
                allergy = allergy + dr["AllergyName"].ToString() + ",";
            }

            if (allergy.EndsWith(","))
            {
                allergy = allergy.Substring(0, allergy.Length - 1);
            }

            return allergy;
        }
        else
        {
            return allergy;
        }
    }

    public int getGridHeight()
    {
       int gridHeight = 0;
       if (Session["PAGEHEIGHT"] != null)
       {
           gridHeight = Convert.ToInt32(Session["PAGEHEIGHT"]) - ((PhysicianMasterPage)Master).getTableHeight()-50;
       }
       return gridHeight;
    }

    #endregion

}

}