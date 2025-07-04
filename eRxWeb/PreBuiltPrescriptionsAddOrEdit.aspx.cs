using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Web.UI.HtmlControls;
using System.Collections;
using System.Text;
using Telerik.Web.UI;
using Allscripts.Impact.PreBuildPrescription;
using Allscripts.ePrescribe.Common;
using Allscripts.Impact;
using Allscripts.Impact.Utilities;

namespace eRxWeb
{
    public partial class PreBuiltPrescriptionsAddOrEdit : BasePage
    {
        public string groupid
        {
            get
            {
                if (ViewState["GROUPID"] != null && ViewState["GROUPID"].ToString() != string.Empty)
                {
                    return ViewState["GROUPID"].ToString();
                }
                else
                {
                    return string.Empty;
                }
            }
            set
            {
                ViewState["GROUPID"] = value;
            }
        }
        PreBuildPrescription preBuilteRx = new PreBuildPrescription();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request.QueryString["GroupId"] != null && Request.QueryString["GroupId"] != string.Empty)
                {
                    pnlAddDeletPrescription.Enabled = true;
                    groupid = HttpUtility.UrlDecode(Request.QueryString["GroupId"].ToString());
                    PageState["PreBuiltPrescriptionGroupID"] = groupid;
                    LoadData(groupid);
                }
            }
        }

        protected void Page_LoadComplete(object sender, EventArgs e)
        {
            ((PhysicianMasterPageBlank)Master).hideTabs();
        }

        protected void btnSaveGroup_Click(object sender, EventArgs e)
        {
            if (txtGroupName.Text.Trim() != string.Empty)
            {
                bool status = !chkboxInActive.Checked;
                if (groupid != string.Empty)
                {
                    //Edit Mode
                    PageState["PreBuiltPrescriptionGroupID"] = groupid;
                    bool updateStatus = preBuilteRx.UpdatePreBuiltPrescriptionGroup(Convert.ToInt32(groupid), base.SessionLicenseID, txtGroupName.Text.Trim(), base.SessionUserID, status, base.DBID);
                    if (updateStatus)
                    {
                        // success
                        ucMessage.Visible = true;
                        divUrgentMessage.Style.Add("display", "block");
                        ucMessage.MessageText = string.Format("Successfully updated the group");
                        ucMessage.Icon = Controls_Message.MessageType.SUCCESS;
                    }
                    else
                    {
                        // Show Message that record already exists.
                        ucMessage.Visible = true;
                        divUrgentMessage.Style.Add("display", "block");
                        ucMessage.MessageText = string.Format("Error occured, group name is in use");
                        ucMessage.Icon = Controls_Message.MessageType.ERROR;
                    }

                }
                else
                {
                    //Add Mode
                    string newGroupId = preBuilteRx.SavePreBuiltPrescriptionGroup(base.SessionLicenseID, txtGroupName.Text.Trim(), status, base.SessionUserID, base.DBID);
                    if (!string.IsNullOrEmpty(newGroupId) && newGroupId.Equals("failed", StringComparison.OrdinalIgnoreCase))
                    {
                        // Show Message that record already exists.
                        ucMessage.Visible = true;
                        divUrgentMessage.Style.Add("display", "block");
                        ucMessage.MessageText = string.Format("Error occured, group name is in use");
                        ucMessage.Icon = Controls_Message.MessageType.ERROR;
                    }
                    else
                    {
                        // success
                        ucMessage.Visible = true;
                        divUrgentMessage.Style.Add("display", "block");
                        ucMessage.MessageText = string.Format("Successfully created the group");
                        ucMessage.Icon = Controls_Message.MessageType.SUCCESS;

                        PageState["PreBuiltPrescriptionGroupID"] = newGroupId;
                        pnlAddDeletPrescription.Enabled = true;
                        groupid = newGroupId;
                    }
                }
                ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "ClearSelectMedicationPrebuiltList", $"ClearSelectMedicationPrebuiltList();", true);
            }
        }

        public void LoadData(string groupId)
        {
            var groupDetails = preBuilteRx.GetPreBuiltPrescriptionGroup(base.SessionLicenseID, null, Convert.ToInt32(groupId), base.DBID);
            txtGroupName.Text = groupDetails[0].Name.ToString();
            if (groupDetails[0].IsActive.ToString().Equals("Active", StringComparison.OrdinalIgnoreCase))
            {
                chkboxInActive.Checked = false;
            }
            else
            {
                chkboxInActive.Checked = true;
            }
            lblheader.Text = "Edit Prescription Group";
            BindPrescriptionGrid(Convert.ToInt32(groupId));
        }

        public void BindPrescriptionGrid(int? groupId)
        {
            gridGroupPrescriptions.DataSource = preBuilteRx.GetPreBuiltPrescriptionGroupRxByGroup(null, groupId, base.DBID);
            gridGroupPrescriptions.DataBind();
        }

        protected void gridGroupPrescriptions_ItemCommand(object sender, Telerik.Web.UI.GridCommandEventArgs e)
        {
            if (e.CommandName == "Edit")
            {
                GridDataItem selectedMedData = (GridDataItem)e.Item;
                ArrayList rxList = new ArrayList();
                Rx newRx = new Rx();
                decimal packageSize = 0;
                newRx.DDI = selectedMedData.GetDataKeyValue("DDI").ToString();
                newRx.MedicationName = HttpUtility.HtmlDecode(selectedMedData.GetDataKeyValue("MedicationName").ToString());
                newRx.Strength = selectedMedData.GetDataKeyValue("Strength").ToString();
                newRx.StrengthUOM = selectedMedData.GetDataKeyValue("StrengthUOM").ToString();
                newRx.DosageFormCode = selectedMedData.GetDataKeyValue("DosageFormCode").ToString();
                newRx.RouteOfAdminCode = selectedMedData.GetDataKeyValue("RouteOfAdminCode").ToString();
                newRx.SigText = selectedMedData.GetDataKeyValue("SIGText").ToString();
                newRx.SigID = selectedMedData.GetDataKeyValue("SIGID").ToString();
                newRx.Quantity = selectedMedData.GetDataKeyValue("Quantity").RetrieveDecimalValue();
                newRx.PackageDescription = selectedMedData.GetDataKeyValue("PackageDescription").ToString();
                newRx.PackageSize = selectedMedData.GetDataKeyValue("PackageSize").RetrieveDecimalValue();
                newRx.PackageQuantity = selectedMedData.GetDataKeyValue("PackageQuantity").RetrieveIntValue();
                newRx.Refills = Convert.ToInt32(selectedMedData.GetDataKeyValue("RefillQuantity"));
                newRx.DAW = selectedMedData.GetDataKeyValue("DAW").ToString().Equals("Y") ? true : false;
                newRx.DaysSupply = Convert.ToInt32(selectedMedData.GetDataKeyValue("DaysSupply").ToString());
                rxList.Add(newRx);
                PageState["RxList"] = rxList;
                PageState["SIGTEXT"] = newRx.SigText;
                PageState["PreBuiltPrescriptionGroupID"] = selectedMedData.GetDataKeyValue("GroupID").ToString();
                string preBuiltRxId = selectedMedData.GetDataKeyValue("ID").ToString();
                Response.Redirect(Constants.PageNames.PRE_BUILT_MED_SELECT_SIG + "?ScriptID=" + HttpUtility.UrlEncode(preBuiltRxId) + "&Mode=" + HttpUtility.UrlEncode("Edit"));
            }
        }

        protected void gridGroupPrescriptions_RowDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                GridDataItem tempDataItem = (GridDataItem)e.Item;
                string DDI = tempDataItem.GetDataKeyValue("DDI").ToString();
                string MedicationName = tempDataItem.GetDataKeyValue("MedicationName").ToString();
                string Strength = tempDataItem.GetDataKeyValue("Strength").ToString();
                string StrengthUOM = tempDataItem.GetDataKeyValue("StrengthUOM").ToString();
                string DosageFormCode = tempDataItem.GetDataKeyValue("DosageFormCode").ToString();
                string SIGText = tempDataItem.GetDataKeyValue("SIGText").ToString();
                string RefillQuantity = tempDataItem.GetDataKeyValue("RefillQuantity").ToString();                
                Decimal Quantity = tempDataItem.GetDataKeyValue("Quantity").RetrieveDecimalValue();
                string DaysSupply = tempDataItem.GetDataKeyValue("DaysSupply").ToString();
                int PackageQuantity = tempDataItem.GetDataKeyValue("PackageQuantity").RetrieveIntValue();                
                Decimal PackageSize = tempDataItem.GetDataKeyValue("PackageSize").RetrieveDecimalValue();
                string PackageUOM = tempDataItem.GetDataKeyValue("PackageUOM").ToString();
                string DAW = tempDataItem.GetDataKeyValue("DAW").ToString();

                Literal litMedicationAndSig = (Literal)tempDataItem.FindControl("litMedicationAndSig");

                if (litMedicationAndSig != null)
                {
                    StringBuilder description = new StringBuilder();
                    description.Append(MedicationName);
                    if (!string.IsNullOrWhiteSpace(DDI))
                    {
                        description.Append(" ").Append(Strength).Append(" ").Append(StrengthUOM).Append(" ").Append(DosageFormCode).Append(" - ");
                    }
                    else
                    {
                        description.Append(" - ");
                    }

                    description.Append(SIGText);

                    if (DAW.Equals("Y", StringComparison.OrdinalIgnoreCase))
                    {
                        description.Append(" - DAW");
                    }

                    string dispensedRefillText =
                        (PageState[Constants.SessionVariables.ChangeRxRequestedMedCs] != null ||
                         PageState.GetBooleanOrFalse(Constants.SessionVariables.IsCsRefReqWorkflow))
                            ? "DISPENSE"
                            : "REFILL";
                    Decimal totalQuantity = (PackageQuantity * PackageSize) * Quantity;
                    Decimal roundedtotalQuantity = Math.Round(totalQuantity, 4);
                    litMedicationAndSig.Text = $"{description.ToString()}<br/> QUANTITY {roundedtotalQuantity} {PackageUOM}-{dispensedRefillText} " +
                                                    $"{RefillQuantity}  - Days Supply - {DaysSupply}";
                }
            }

            if (e.Item is GridPagerItem)
            {
                GridPagerItem pager = (GridPagerItem)e.Item;
                RadComboBox PageSizeComboBox = (RadComboBox)pager.FindControl("PageSizeComboBox");
                PageSizeComboBox.Visible = false;
            }
        }

        protected void btnAddPrescription_Click(object sender, EventArgs e)
        {
            Response.Redirect(Constants.PageNames.PRE_BUILT_SELECT_MED + "?GroupId=" + groupid);
        }
        protected void btnDeleteSelected_Click(object sender, EventArgs e)
        {
            //DeletePrebuilt_Prescriptions
            List<int> groupRxIdList = new List<int>();

            foreach (GridDataItem items in gridGroupPrescriptions.MasterTableView.Items)
            {
                HtmlInputCheckBox radGridCheckBoxId = (HtmlInputCheckBox)items.FindControl("radGridCheckBoxId");
                if (radGridCheckBoxId != null && radGridCheckBoxId.Checked)
                {
                    groupRxIdList.Add(Convert.ToInt32(items.GetDataKeyValue("ID").ToString()));
                }
            }

            preBuilteRx.DeletePrebuilt_Prescriptions(groupRxIdList, base.DBID);
            LoadData(groupid);
        }

        protected void gridGroupPrescriptions_PageIndexChanged(object source, Telerik.Web.UI.GridPageChangedEventArgs e)
        {
            LoadData(groupid);
        }
    }
}
