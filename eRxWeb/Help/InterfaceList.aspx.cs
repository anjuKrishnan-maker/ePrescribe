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
using Telerik.Web.UI;
using Allscripts.ePrescribe.Common;

namespace eRxWeb
{
public partial class Help_InterfaceList : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            this.Form.Attributes.Add("autocomplete", "off");
            ((HelpMasterPageNew)this.Master).CurrentPage = HelpMasterPageNew.HelpPage.INTERFACES;
        }
    }
    protected void btnContinueBottom_Click(object sender, EventArgs e)
    {
        goToRequest(false);
    }

    protected void grdInterfaces_ItemDataBound(object sender, GridItemEventArgs e)
    {
        if (e.Item is GridDataItem)
        {
            GridDataItem tempDataItem = (GridDataItem)e.Item;
            HtmlInputRadioButton rbSelect = tempDataItem.FindControl("rbSelect") as HtmlInputRadioButton;
            if (rbSelect != null)
            {
                rbSelect.Attributes.Add("InterfaceID", tempDataItem.GetDataKeyValue("InterfaceID").ToString());
                rbSelect.Attributes.Add("onclick", "GridSingleSelect(" + tempDataItem.GetDataKeyValue("InterfaceID").ToString() + ", 'InterfaceID', '" + grdInterfaces.MasterTableView.ClientID + "', true)");
            }

            HtmlContainerControl nameDiv = tempDataItem.FindControl("nameDiv") as HtmlContainerControl;
            if (nameDiv != null)
            {
                if (string.IsNullOrEmpty(base.SessionLicense.EnterpriseClient.InterfaceBillingParty))
                {
                    decimal costFactor = 1;
                    try
                    {
                        costFactor = Convert.ToDecimal(tempDataItem.GetDataKeyValue("TierOneBase")) +
                                     Convert.ToDecimal(tempDataItem.GetDataKeyValue("TierOneMonth")) +
                                     Convert.ToDecimal(tempDataItem.GetDataKeyValue("TierTwoBase")) +
                                     Convert.ToDecimal(tempDataItem.GetDataKeyValue("TierTwoMonth"));
                    }
                    catch (Exception)
                    {
                        costFactor = 1;
                    }

                    if (costFactor == 0)
                    {
                        nameDiv.InnerText = tempDataItem.GetDataKeyValue("Name").ToString() + " (Free)";
                    }
                    else
                    {
                        nameDiv.InnerText = tempDataItem.GetDataKeyValue("Name").ToString();
                    }
                }
                else
                {
                    nameDiv.InnerText = tempDataItem.GetDataKeyValue("Name").ToString();
                }
            }
        }
    }

    private void goToRequest(bool other)
    {
        if (!other)
        {
            for (int i = 0; i < grdInterfaces.Items.Count; i++)
            {
                if (grdInterfaces.Items[i] is GridDataItem)
                {
                    GridDataItem tempdataItem = (grdInterfaces.Items[i] as GridDataItem);
                    if (tempdataItem.Selected)
                    {
                        Response.Redirect(Constants.PageNames.HELP_INTERFACE_REQUEST + "?interfaceid=" + tempdataItem.GetDataKeyValue("InterfaceID").ToString());
                        return;
                    }
                }
            }

            ucMessage.MessageText = "Must select practice management system or select Other before continuing.";
            ucMessage.Icon = Controls_Message.MessageType.ERROR;
            ucMessage.Visible = true;
        }
        else
        {
            Response.Redirect(Constants.PageNames.HELP_INTERFACE_REQUEST + "?interfaceid=109");
        }
    }

    protected void btnNotInList_Click(object sender, EventArgs e)
    {
        goToRequest(true);
    }
}

}