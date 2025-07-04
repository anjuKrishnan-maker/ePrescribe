#region References
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using Telerik.Web.UI;
using Allscripts.Impact.Utilities;
#endregion

namespace eRxWeb
{
public partial class Controls_PatientSearchWithResults : BaseControl
{
    #region Page Variables
    public PatientSelectEventArgs eventArgs;
    public event PatientSelectEventHandler PatientSelectEvent;

    #endregion

    #region Page Properties
    public void Show(bool resetSearch)
    {
        mpePatientSearch.Show();

        if (resetSearch)
        {
            ucPatientSearch.ResetSearchFilters();
            resetSearchResult();
        }
    }

    #endregion

    #region Page Events
    protected void Page_Load(object sender, EventArgs e)
    {
        //User Controls Ajax Settings
        Button btnSearch = (Button)ucPatientSearch.FindControl("btnSearch");
        RadAjaxManager1.AjaxSettings.AddAjaxSetting(btnSearch, grdViewPatients);
        RadAjaxManager1.AjaxSettings.AddAjaxSetting(btnSearch, ucMessage);

        RadContextMenu searchTypeMenu = (RadContextMenu)ucPatientSearch.FindControl("searchTypeMenu");
        RadAjaxManager1.AjaxSettings.AddAjaxSetting(searchTypeMenu, grdViewPatients);

        if (!Page.IsPostBack)
        {
            resetSearchResult();
        }
    }
    protected void btnCancelAndClear_OnClick(object sender, EventArgs e)
    {
        ThrowEvent(PatientSelectEventArgs.ActionType.CLEAR_PATIENT, string.Empty, string.Empty);
    }
    protected void btnCancel_Click(object sender, EventArgs e)
    {
        mpePatientSearch.Hide();
    }
    #endregion

    #region Grid Events
    protected void PatObjDataSource_Selected(object sender, ObjectDataSourceStatusEventArgs e)
    {
        if (e.Exception == null)
        {
            DataSet ds = (DataSet)e.ReturnValue;
            if (ds.Tables[0].Rows.Count > 50)
            {
                ucMessage.MessageText = "Your search returned more than 50 results. Please consider refining your search.";
                ucMessage.Icon = Controls_Message.MessageType.INFORMATION;
                ucMessage.Visible = true;
            }
            else
            {
                ucMessage.Visible = false;
            }

            if (ds.Tables[0].Rows.Count > 13)
            {
                grdViewPatients.Height = System.Web.UI.WebControls.Unit.Empty;
            }
            ds.Dispose();
        }
    }

    protected void grdViewPatients_RowDataBound(object sender, GridItemEventArgs e)
    {
        if (e.Item is GridDataItem)
        {
            GridDataItem tempDataItem = (GridDataItem)e.Item;
            
            int statusID = int.Parse(tempDataItem.GetDataKeyValue("StatusID").ToString());

            if ((bool)tempDataItem.GetDataKeyValue("IsVIPPatient") || (bool)tempDataItem.GetDataKeyValue("IsRestrictedPatient"))
            {
                Image img = new Image();
                img.ImageUrl = @"~\images\PrivacyImages\sensitivehealth-global-16-x-16.png";
                tempDataItem["ImgColumn"].Controls.Add(img);
                Label lbl = new Label();
                lbl.Text = " " + StringHelper.ConvertToUxName(tempDataItem.Cells[4].Text);
                tempDataItem["ImgColumn"].Controls.Add(lbl);
            }

            if (statusID == 0)
            {
                //inactive patient!
                tempDataItem.Cells[1].Font.Italic = true;
                tempDataItem.Cells[1].ForeColor = System.Drawing.Color.Gray;
                tempDataItem.Cells[2].Font.Italic = true;
                tempDataItem.Cells[2].ForeColor = System.Drawing.Color.Gray;
                tempDataItem.Cells[3].Font.Italic = true;
                tempDataItem.Cells[3].ForeColor = System.Drawing.Color.Gray;
                tempDataItem.Cells[4].Font.Italic = true;
                tempDataItem.Cells[4].ForeColor = System.Drawing.Color.Gray;
                tempDataItem.Cells[5].Font.Italic = true;
                tempDataItem.Cells[5].ForeColor = System.Drawing.Color.Gray;

                grdViewPatients.Columns[0].ItemStyle.Width = System.Web.UI.WebControls.Unit.Pixel(90);

                Label inactive = tempDataItem.FindControl("inactive") as Label;
                if (inactive != null)
                {
                    inactive.Style["display"] = "inline";
                }
            }
        }

        if (e.Item is GridPagerItem)
        {
            GridPagerItem pager = (GridPagerItem)e.Item;
            Label lbl = (Label)pager.FindControl("ChangePageSizeLabel");
            lbl.Visible = false;

            RadComboBox combo = (RadComboBox)pager.FindControl("PageSizeComboBox");
            combo.Visible = false;
        }
    }

    protected void grdViewPatients_ItemCommand(object source, Telerik.Web.UI.GridCommandEventArgs e)
    {
        if (e.Item is GridDataItem)
        {
            GridDataItem item = (GridDataItem)e.Item;

            string patientID = item.OwnerTableView.DataKeyValues[item.ItemIndex]["PatientID"].ToString();
            string patientName = item.OwnerTableView.DataKeyValues[item.ItemIndex]["Name"].ToString();

            if (e.CommandName == "SelectPatient")
            {
                ThrowEvent(PatientSelectEventArgs.ActionType.SELECT_PATIENT, patientName, patientID);
            }
        }
    }

    protected void grdViewPatients_PageIndexChanged(object source, Telerik.Web.UI.GridPageChangedEventArgs e)
    {
        Show(false);
        grdViewPatients.Rebind();
    }


    #endregion

    #region Private Methods
    /// <summary>
    /// Reset the page controls in default state.
    /// </summary>
    private void resetSearchResult()
    {
        ucMessage.Visible = false;

        foreach (Parameter SelectParameter in PatObjDataSource.SelectParameters)
        {
            SelectParameter.DefaultValue = string.Empty;
        }

        grdViewPatients.DataSourceID = "PatObjDataSource";
        grdViewPatients.MasterTableView.NoMasterRecordsText = "No records found for the search criteria";
        grdViewPatients.DataBind();
    }

    #endregion

    #region Control Events
    protected virtual void OnPatientSelectEvent(PatientSelectEventArgs e)
    {
        if (PatientSelectEvent != null)
        {
            PatientSelectEvent(this, e);
        }
    }

    protected void patientSearchEvent(object sender, PatientSearchEventArgs e)
    {
        if (e.Action != PatientSearchEventArgs.ActionType.ADD_PATIENT && e.Action != PatientSearchEventArgs.ActionType.ERROR)
        {
            ucMessage.Visible = false;        
            PatObjDataSource.SelectParameters["LastName"].DefaultValue = e.LastName;
            PatObjDataSource.SelectParameters["FirstName"].DefaultValue = e.FirstName;
            PatObjDataSource.SelectParameters["DateOfBirth"].DefaultValue = e.DateOfBirth;
            PatObjDataSource.SelectParameters["ChartID"].DefaultValue = e.PatientID;
            PatObjDataSource.SelectParameters["WildCard"].DefaultValue = e.WildCard;
            PatObjDataSource.SelectParameters["HasVIPPatients"].DefaultValue = base.SessionLicense.hasVIPPatients.ToString();

            if (e.Action == PatientSearchEventArgs.ActionType.SEARCH_ACTIVE)
            {
                PatObjDataSource.SelectParameters["includeInactive"].DefaultValue = "false";
            }
            else
            {
                PatObjDataSource.SelectParameters["includeInactive"].DefaultValue = "true";
            }

            grdViewPatients.CurrentPageIndex = 0;
            grdViewPatients.Rebind();

        }
        Show(false);
    }

    public void ThrowEvent(PatientSelectEventArgs.ActionType actionType, string name, string patientID)
    {
        mpePatientSearch.Hide();

        if (actionType == PatientSelectEventArgs.ActionType.SELECT_PATIENT)
        {
            OnPatientSelectEvent(new PatientSelectEventArgs(name, patientID, actionType));

        }
        else if (actionType == PatientSelectEventArgs.ActionType.CLEAR_PATIENT)
        {
            OnPatientSelectEvent(new PatientSelectEventArgs(name, patientID, actionType));
        }
    }

    #endregion
}
}