//Revision History
/******************************************************************************
* Change History
* Date:      Author:                    Description:
* -----------------------------------------------------------------------------
* 06/17/2009   Dharani Reddem           QC# 3373 - Reset the SiteID session variable to default .          
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
using Allscripts.Impact;
using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.DatabaseSelector;
using Allscripts.ePrescribe.ExtensionMethods;
using eRxWeb.ServerModel;
using System.Web.Script.Serialization;


namespace eRxWeb
{
public partial class helpdesk : BasePage
{
	protected void Page_Load(object sender, EventArgs e)
	{
		if (Session["USERGROUP"] == null || !Session["USERGROUP"].ToString().Contains("HelpDesk"))
		{
			Audit.AddException(base.SessionUserID, base.SessionLicenseID, "POTENTIAL HACK ATTEMPT: Someone is trying to access HelpDesk.aspx without being in the HelpDesk user group.", Request.UserIpAddress(), null, null, base.DBID);
			Session[Constants.SessionVariables.CURRENT_ERROR] = "You are not authorized to view this page.";
            Response.Redirect(Constants.PageNames.LOGIN);
		}
	}

    protected void Page_LoadComplete(object sender, EventArgs e)
    {
        ((PhysicianMasterPageBlank)Master).hideTabs();
    }

    protected void btnSelect_Click(object sender, EventArgs e)
    {
        //since we're selecting another site, set the Session Licenseid and nullify the Session DBID. The base class DBID will then handle the rest.
		DataManagerSvc.EntityIdentifier entityID = new DataManagerSvc.EntityIdentifier();
		entityID.Type = DataManagerSvc.IdentifierType.LicenseGUID;
		entityID.Value = lstSites.SelectedValue;

		DataManagerSvc.DataManagerSvc dmSvc = new DataManagerSvc.DataManagerSvc();
		dmSvc.Url = AppSettings("DataManagerSvc.DataManagerSvc");

		PageState["DBID"] = (ConnectionStringPointer)dmSvc.GetDatabasePointer(entityID);

        //split the coumn value LicenseIDWithSiteID to LicenseID and SiteID
        string[] selectedValues = lstSites.SelectedValue.Split(';');

        string licenseId = selectedValues[0];
        int siteId = Convert.ToInt32(selectedValues[1]);

        //swap out the LicenseID!
        DataSet ds = Allscripts.Impact.ApplicationLicense.Load(licenseId, base.DBID);
        PageState["AHSAccountID"] = ds.Tables[0].Rows[0]["AccountID"];
        PageState["LicenseName"] = ds.Tables[0].Rows[0]["LicenseName"];
		
		bool isShieldEnabled = false;
		bool.TryParse(ds.Tables[0].Rows[0]["IsShieldEnabled"].ToString(), out isShieldEnabled);
		PageState["IsLicenseShieldEnabled"] = isShieldEnabled;
		
        ApplicationLicense license = new ApplicationLicense(licenseId, siteId, ds.Tables[0].Rows[0]["EnterpriseClientID"].ToString(), base.DBID);
        PageState["SessionLicense"] = license;
        PageState[Constants.SessionVariables.LicensePreferences] = new LicensePreferences(licenseId.ToGuidOr0x0(), DBID);

        EnterpriseClient ec = license.EnterpriseClient;
        if (ec != null)
        {
            PageState["EnterpriseID"] = ec.ID;

            if ((license.LicenseDeluxeStatus == Constants.DeluxeFeatureStatus.On ||
                 license.LicenseDeluxeStatus == Constants.DeluxeFeatureStatus.Cancelled)
                && !string.IsNullOrEmpty(ec.DeluxeStyleSheet))
            {
                PageState["Theme"] = ec.DeluxeStyleSheet;
            }
            else
            {
                PageState["Theme"] = ec.StyleSheet;
            }

            PageState["PageTitle"] = ec.PageTitle + " ";

            if (ec.RightBoxIsVisible)
            {
                PageState["RightBoxIsVisible"] = ec.RightBoxIsVisible;
                PageState["RightBoxHeaderText"] = ec.HeaderText;
                PageState["RightBoxImageURL"] = ec.ImageURL;
                PageState["RightBoxBodyText"] = ec.BodyText;
                PageState["RightBoxLinks"] = ec.Links;
            }

            if (!string.IsNullOrEmpty(ec.LogoutURL))
                PageState["EnterpriseClientLogoutURL"] = ec.LogoutURL;

            PageState["HelpURL"] = ec.HelpURL;
            PageState["AddUser"] = ec.AddUser;
            PageState["EditUser"] = ec.EditUser;
            PageState["MergePatients"] = ec.MergePatients;
            PageState["AddPatient"] = ec.AddPatient;
            PageState["EditPatient"] = ec.EditPatient;
            PageState["ViewPatient"] = ec.ViewPatient;
            PageState["AddAllergy"] = ec.AddAllergy;
            PageState["EditAllergy"] = ec.EditAllergy;
            PageState["AddDiagnosis"] = ec.AddDiagnosis;
            PageState["EditDiagnosis"] = ec.EditDiagnosis;
            PageState["EditPharmacy"] = ec.EditPharmacy;
            PageState["ShortcutIcon"] = ec.ShortCutIcon;
            PageState["EnableRenewals"] = ec.EnableRenewals;

                
            if (!string.IsNullOrEmpty(ec.DefaultPatientLockDownPage))
                PageState["DefaultPatientLockDownPage"] = ec.DefaultPatientLockDownPage;

            PageState["SITEID"] = siteId.ToString();
        }


        PageState["LICENSEID"] = licenseId;
        PageState["SITENAME"] = Allscripts.Impact.ApplicationLicense.SiteLoad(licenseId, siteId, base.DBID).Tables[0].Rows[0]["SiteName"].ToString();
        PageState["STANDING"] = ds.Tables[0].Rows[0]["Standing"].ToString();
        PageState["IsBackDoorUser"] = true;

		EPSBroker.CreateBackdoorUserAuditLogEntry(
			base.SessionUserID,
			base.SessionLicenseID,
			base.SessionSiteID,
			Session.SessionID,
			"ePrescribe",
			base.DBID);

        if (string.IsNullOrEmpty(Request.QueryString["TargetUrl"]))
        {
            SelectPatientComponentParameters selectPatientComponentParameters = new SelectPatientComponentParameters
            {
                PatientId = PageState.GetStringOrEmpty(Constants.SessionVariables.PatientId)
            };
            RedirectToSelectPatient(null, selectPatientComponentParameters);
        }
        else
        {
            string UrlForRedirection = Constants.PageNames.UrlForRedirection(Request.QueryString["TargetUrl"].ToString());
            if (string.IsNullOrEmpty(UrlForRedirection))
            {
                SelectPatientComponentParameters selectPatientComponentParameters = new SelectPatientComponentParameters
                {
                    PatientId = PageState.GetStringOrEmpty(Constants.SessionVariables.PatientId)
                };
                RedirectToSelectPatient(null, selectPatientComponentParameters);
            }
            else
            {
                Server.Transfer(UrlForRedirection);
            }
        }
    }

    protected void btnRegular_Click(object sender, EventArgs e)
    {
        DefaultRedirect(false);
    }
}

}