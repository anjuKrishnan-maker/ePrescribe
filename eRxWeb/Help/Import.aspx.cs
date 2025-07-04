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
using Allscripts.Impact;
using Allscripts.ePrescribe.Common;

namespace eRxWeb
{
    public partial class Help_Import : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);

            this.Form.Attributes.Add("autocomplete", "off");

            adControl.FeaturedModule = Module.ModuleType.DELUXE;
            adControl1.FeaturedModule = Module.ModuleType.DELUXE;

            if (!SessionLicense.EnterpriseClient.ShowRequestInterfaceLink)
            {
                autoLink.Visible = false;
            }
            else
            {
                if ((SessionLicense.DeluxeFeatureStatusDisplay.Contains("Platinum")) &&
                    (SessionLicense.EnterpriseClient.ID.ToUpper() != "F702FABB-C77E-4F49-9F1E-50C7B844BBA6"))
                {
                    if ((SessionLicense.LicenseDeluxeStatus == Constants.DeluxeFeatureStatus.On ||
                       SessionLicense.LicenseDeluxeStatus == Constants.DeluxeFeatureStatus.AlwaysOn ||
                       SessionLicense.LicenseDeluxeStatus == Constants.DeluxeFeatureStatus.Cancelled)
                       && SessionLicense.DeluxePricingStructure != Constants.DeluxePricingStructure.CompulsoryBasic)
                    {
                        autoLink.Visible = true;
                    }
                    else
                    {
                        autoLink.Visible = false;
                    }
                }
                else
                {
                    if ((SessionLicense.LicenseDeluxeStatus == Constants.DeluxeFeatureStatus.On ||
                       SessionLicense.LicenseDeluxeStatus == Constants.DeluxeFeatureStatus.AlwaysOn ||
                       SessionLicense.LicenseDeluxeStatus == Constants.DeluxeFeatureStatus.Cancelled)
                       && SessionLicense.DeluxePricingStructure != Constants.DeluxePricingStructure.CompulsoryBasic)
                    {
                        autoLink.Visible = true;
                    }
                    else
                    {
                        mpeAd1.TargetControlID = "autoLink";
                    }
                }
            }
            if (!IsPostBack)
            {
                if (!base.SessionLicense.HasInterface)
                {
                    divGetStarted.Style["display"] = "inline";
                    divRestricted.Style["display"] = "none";
                }
                else
                {
                    divGetStarted.Style["display"] = "none";
                    divRestricted.Style["display"] = "inline";
                }
            }
        }
    }
}

