
/******************************************************************************
**Change History
*******************************************************************************
**Date:         Author:                    Description:
**-----------------------------------------------------------------------------
**04/07/2010	Anand Kumar Krishnan       Defect#3380: Add Site Pharmacy Functionality is added.
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
using Telerik.Web.UI;
using System.Drawing;
using Allscripts.ePrescribe.Common;

namespace eRxWeb
{
public partial class PharmacyFavorites : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            if (Request.QueryString["siteid"] != null)
            {
                ddlSites.SelectedValue = Request.QueryString["siteid"];
            }
        }
    }

    protected void Page_LoadComplete(object sender, EventArgs e)
    {
        ((PhysicianMasterPageBlank)Master).hideTabs();
    }
    protected void btnDelete_Click(object sender, EventArgs e)
    {
        ucMessage.Visible = false;
        bool pharmaciesDeleted = false;
        foreach (GridDataItem item in grdViewPharmacyFavorites.SelectedItems)
        {
            if (item.OwnerTableView.DataKeyValues[item.ItemIndex]["PracticeFavorite"].ToString().ToUpper() == "N")
            {
                Allscripts.Impact.Pharmacy.DeleteSitePharmacy(SessionLicenseID, Convert.ToInt32(ddlSites.SelectedValue), base.DBID);
            }
            else
            {
                Allscripts.Impact.Pharmacy.DeletePharmacyFavorite(Convert.ToInt32(item.OwnerTableView.DataKeyValues[item.ItemIndex]["PharmacyFavoriteID"]), base.DBID);
            }
            
            pharmaciesDeleted = true;
        }

        if (pharmaciesDeleted)
        {
            AuditLogLicenseInsert(ePrescribeSvc.AuditAction.LICENSE_SITE_PHARMACY_FAVORITE_EDIT, Convert.ToInt32(ddlSites.SelectedValue));
            Response.Redirect(Constants.PageNames.SITE_MANAGEMENT + "?msg=" + Server.UrlEncode("The selected pharmacies were successfully deleted."));
        }
        else
        {
            ucMessage.MessageText = "Select a pharmacy to delete.";
            ucMessage.Visible = true;
        }
    }
    protected void btnAddPharmacy_Click(object sender, EventArgs e)
    {
        Response.Redirect(Constants.PageNames.PHARMACY + "?Mode=AddSitePharmacy&siteid=" + ddlSites.SelectedValue.ToString() + "&from=" + Constants.PageNames.PHARMACY_FAVORITES);
    }
   
    protected void grdViewPharmacyFavorites_ItemDataBound(object sender, GridItemEventArgs e)
    {
        if (((e.Item.ItemType == GridItemType.Item) || (e.Item.ItemType == GridItemType.AlternatingItem)) &&
           (e.Item.OwnerTableView.DataKeyValues[e.Item.ItemIndex]["PracticeFavorite"].ToString().ToUpper() == "N"))
        {
            GridDataItem item = (GridDataItem)e.Item;
            item["Name"].Font.Bold = true;
            item["Address1"].Font.Bold = true;
            item["City"].Font.Bold = true;
            item["State"].Font.Bold = true;
        }
    }
    
}

}