// Revision History
//HA/Dhiraj 102: Favourites Page error messages
/******************************************************************************
**Change History
*******************************************************************************
**Date:      Author:                    Description:
**-----------------------------------------------------------------------------
*
**05/03/2010   Sonal                	Defect#3401 - User which has no NPI# can Bypass the NPI#
 *                                      entry on the Edit user page via the Change Password link.
 * *******************************************************************************/
using System;
using System.Data;
using System.Text;
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
using eRxWeb.AppCode;

namespace eRxWeb
{
    public partial class FullScriptEdit : BasePage
    {
        protected bool _checkAltCopay = false;
        protected string sortExpression = "Medication ASC";

        protected void Page_Load(object sender, EventArgs e)
        {
            ucMessage.Visible = false;

            //Parameter ppM = MedObjDataSource.SelectParameters["PatientID"];
            //MedObjDataSource.SelectParameters.Remove(ppM);
            //MedObjDataSource.SelectParameters.Add("PatientID", DBNull.Value.ToString());

            if (!IsPostBack)
            {
                if (Request.QueryString["searchText"] != null && !IsPostBack)
                {
                    txtSearchMed.Text = HttpUtility.UrlDecode(Request.QueryString["searchText"].ToString());
                }

                if (Request.QueryString["msg"] != null && Request.QueryString["msg"].ToString() != "")
                {
                    ucMessage.Visible = true;
                    ucMessage.MessageText = HttpUtility.UrlDecode(Request.QueryString["msg"].ToString());
                    ucMessage.Icon = Controls_Message.MessageType.INFORMATION;
                }
            }

            if (Request.Form[btnCancel.ClientID.Replace("_", "$")] != null)
            {
                return;
            }

            Page.PreRender += new EventHandler(Page_PreRender);
            grdViewMed.DataBound += new EventHandler(grdViewMed_DataBound);
            this.Form.DefaultButton = btnGo.UniqueID;
            if (!Page.IsPostBack)
            {
                txtSearchMed.Focus();
            }
        }

        void Page_PreRender(object sender, EventArgs e)
        {
        }

        void grdViewMed_DataBound(object sender, EventArgs e)
        {

        }

        protected void Page_LoadComplete(object sender, EventArgs e)
        {
            ((PhysicianMasterPageBlank)Master).hideTabs();
        }

        protected void btnGo_Click(object sender, EventArgs e)
        {

            if (txtSearchMed.Text == "Enter Partial or Full Medication")
            {
                return;
            }
            //Parameter ppM = MedObjDataSource.SelectParameters["PatientID"];
            //MedObjDataSource.SelectParameters.Remove(ppM);
            //MedObjDataSource.SelectParameters.Add("PatientID", DBNull.Value.ToString());
        }

        protected void grdViewMed_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                //e.Row.Style["cursor"] = "pointer";           

                string isGeneric = grdViewMed.DataKeys[e.Row.RowIndex][3].ToString();

                if (isGeneric == "N")
                {
                    e.Row.Cells[1].Font.Bold = true;
                }

                TextBox txtQuantity = e.Row.FindControl("txtQuantity") as TextBox;
                if (txtQuantity != null)
                {
                    txtQuantity.Text = stripZeroes(txtQuantity.Text);
                }

                Literal litMedicationAndSig = (Literal)e.Row.FindControl("litMedicationAndSig");
                if (litMedicationAndSig != null)
                {
                    string medication = grdViewMed.DataKeys[e.Row.RowIndex]["Medication"].ToString();
                    string sigText = grdViewMed.DataKeys[e.Row.RowIndex]["SIGText"].ToString();

                    StringBuilder description = new StringBuilder();
                    description.Append(medication);
                    description.Append(sigText);
                    litMedicationAndSig.Text = description.ToString();
                }
            }
        }

        protected void grdViewMed_OnSorting(object sender, GridViewSortEventArgs e)
        {
            DataTable dataTable = grdViewMed.DataSource as DataTable;
            if (dataTable != null)
            {
                DataView dataView = new DataView(dataTable);
                dataView.Sort = e.SortExpression + " ASC";
                grdViewMed.DataSource = dataView;
                grdViewMed.DataBind();
            }
        }

        protected void MedObjDataSource_Selected(object sender, ObjectDataSourceStatusEventArgs e)
        {
            if (e.ReturnValue == null)
            {
                //hm
            }
            else
            {
                DataSet ds = (DataSet)e.ReturnValue;

                if (ds.Tables[0].Rows.Count > 50 && IsPostBack)
                {
                    ucMessage.Visible = true;
                    ucMessage.MessageText = "Your usual Rx list contains more than 50 results.  Consider searching with more specific terms.";
                    ucMessage.Icon = Controls_Message.MessageType.INFORMATION;
                }

                DataView dv = ds.DefaultViewManager.CreateDataView(ds.Tables[0]);
                dv.Sort = "Medication";
                dv.ApplyDefaultSort = true;

                ds.Tables.Clear();
                ds.Tables.Add(dv.ToTable());
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

        protected void MedObjDataSource_Selecting(object sender, ObjectDataSourceSelectingEventArgs e)
        {
            if (e.InputParameters["MedName"] != null)
            {
                if (e.InputParameters["MedName"].ToString() == "Enter Partial or Full Medication")
                {
                    e.InputParameters["MedName"] = "";
                }

                string prevalue = e.InputParameters["MedName"].ToString();
                prevalue = prevalue.Replace("'", "");
                prevalue = prevalue.Replace("\"", "");
                e.InputParameters["MedName"] = prevalue;
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            if (Request.QueryString["Status"] != null)
            {
                string status = Request.QueryString["Status"];
                string componentParameter = $"{{\"mode\": \"Edit\",\"status\": \"{status}\"}}";
                Response.Redirect($"{ Constants.PageNames.REDIRECT_TO_ANGULAR}?componentName={Constants.PageNames.EDIT_USER}&componentParameters={componentParameter}");
            }
            else
            {
                string cameFrom = Constants.PageNames.SELECT_PATIENT;
                if (Request.QueryString["CameFrom"] != null)
                {
                    cameFrom = Request.QueryString["CameFrom"];
                }
                string componentParameter = $"{{\"mode\": \"Edit\",\"CameFrom\": \"{cameFrom}\"}}";
                Response.Redirect($"{ Constants.PageNames.REDIRECT_TO_ANGULAR}?componentName={Constants.PageNames.EDIT_USER}&componentParameters={componentParameter}");
            }
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            if (Request.Form["rdSelectRow"] != null)
            {
                //delete and go back to selectmedication
                char[] charsep = ",".ToCharArray();
                string[] arrRows = Request.Form["rdSelectRow"].Split(charsep);
                foreach (string s in arrRows)
                {
                    Allscripts.Impact.Medication.DeleteUsage(s, base.DBID);
                }
            }
            else
            {
                ucMessage.Visible = true;
                ucMessage.MessageText = "Please select Rx to delete.";
                ucMessage.Icon = Controls_Message.MessageType.ERROR;
            }
            grdViewMed.DataBind();
        }

        protected string stripZeroes(string original)
        {
            if (original.EndsWith(".00"))
            {
                return original.Substring(0, original.Length - 3);
            }
            return original;
        }
    }


}