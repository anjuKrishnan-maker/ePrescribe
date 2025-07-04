using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using Allscripts.ePrescribe.Common;
using Allscripts.Impact.PreBuildPrescription;
using System.Data;
using eRxWeb.AppCode;

namespace eRxWeb
{
    public partial class PreBuiltPrescriptions : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Bind(false);
                Helper.SetHelpTextForPane(paneHelp, this.AppRelativeVirtualPath);

                PageState.Remove("PreBuiltPrescriptionGroupID");
            }
        }

        protected void Page_LoadComplete(object sender, EventArgs e)
        {
            ((PhysicianMasterPageBlank)Master).hideTabs();
        }

        protected void chkboxShowAllGroupd_CheckedChanged(object sender, EventArgs e)
        {
            if (chkboxShowAllGroupd.Checked)
            {
                Bind(true);
            }
            else
            {
                Bind(false);
            }
            gridGroups.DataBind();
        }

        public void Bind(bool showAll)
        {
            PreBuildPrescription preBuilteRx = new PreBuildPrescription();
            bool? showOnlyActive = false;
            if (showAll)
            {
                showOnlyActive = null;
            }
            else
            {
                showOnlyActive = true;
            }

            gridGroups.DataSource = preBuilteRx.GetPreBuiltPrescriptionGroup(base.SessionLicenseID, showOnlyActive, null, base.DBID);

        }

        protected void gridGroups_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            if (chkboxShowAllGroupd.Checked)
            {
                Bind(true);
            }
            else
            {
                Bind(false);
            }
        }

        protected void gridGroups_ItemCommand(object sender, Telerik.Web.UI.GridCommandEventArgs e)
        {
            if (e.CommandName == "Edit")
            {
                GridDataItem item = (GridDataItem)e.Item;
                var res = item.OwnerTableView.DataKeyValues[item.ItemIndex]["ID"].ToString();
                Response.Redirect(Constants.PageNames.PRE_BUILT_PRESCRIPTION_ADD_OR_EDIT + "?GroupId=" + item.OwnerTableView.DataKeyValues[item.ItemIndex]["ID"].ToString());
            }
            else if (e.CommandName == "Delete")
            {
                mpeConfirmDeleteGroup.Show();
                GridDataItem item = (GridDataItem)e.Item;
                var groupdID = item.OwnerTableView.DataKeyValues[item.ItemIndex]["ID"].ToString();
                hdnFldGroupId.Value = groupdID;
                var groupdName = item.OwnerTableView.DataKeyValues[item.ItemIndex]["Name"].ToString();
                lblConfirmDeleteMessage.Text = string.Format("All scripts in group {0} will be deleted. Are you sure?", groupdName);
            }
        }

        protected void gridGroups_PageIndexChanged(object sender, GridPageChangedEventArgs e)
        {
            gridGroups.CurrentPageIndex = e.NewPageIndex;
        }

        protected void btnYes_Click(object sender, EventArgs e)
        {
            var groupdID = hdnFldGroupId.Value.ToString();
            if (!string.IsNullOrEmpty(groupdID))
            {
                PreBuildPrescription preBuilteRx = new PreBuildPrescription();
                preBuilteRx.DeletePrebuilt_PrescriptionGroup(Convert.ToInt32(groupdID), base.DBID);
                Bind(false);
                chkboxShowAllGroupd.Checked = false;
                gridGroups.DataBind();
            }
            mpeConfirmDeleteGroup.Hide();
        }

        protected void btnBack_OnClick(object sender, EventArgs e)
        {
            Response.Redirect(AngularStringUtil.CreateUrl(Constants.PageNames.SETTINGS));
        }
    }
}