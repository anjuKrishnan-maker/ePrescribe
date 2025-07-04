using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using Allscripts.Impact;
using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.DatabaseSelector;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Allscripts.ePrescribe.Data;
using Allscripts.ePrescribe.ExtensionMethods;
using eRxWeb.AppCode;
using eRxWeb.State;
using EnterpriseClient = Allscripts.Impact.EnterpriseClient;
using SystemConfig = Allscripts.Impact.SystemConfig;
using eRxWeb.ServerModel;
using System.Web.Script.Serialization;
using Allscripts.ePrescribe.Objects;
using RxUser = Allscripts.Impact.RxUser;
using static Allscripts.ePrescribe.Common.Constants;

namespace eRxWeb
{
    public partial class SelectAccountAndSite : BasePage
    {
        private string _allscriptsEnterpriseClientID = "283AA2AD-707A-4D0A-A2CE-F88C018DAB46";
        private string redirectURL = string.Empty;

        void Page_PreRender(object sender, EventArgs e)
        {
            string itemID = string.Empty;
            string accountID = string.Empty;
            string siteID = string.Empty;
            ListItem sItem = new ListItem();
            string sValue = string.Empty;

            foreach (RadPanelItem item1 in radPanelBarAccounts.GetAllItems())
            {
                if (item1.Value != string.Empty)
                {
                    itemID = item1.Value;
                }
                RadioButtonList rblSites = (RadioButtonList)item1.FindControl(itemID);
                if (rblSites != null)
                {
                    rblSites.Attributes["onclick"] = "SelectSingleSite(" + itemID + ")";
                }
            }
        }

        protected void hiddenSelect_Click(object sender, EventArgs e)
        {
            //Session.Remove("LICENSEID");
            //Session.Remove("SITEID");

            string itemID = string.Empty;
            string accountID = string.Empty;
            string siteID = string.Empty;
            ListItem sItem = new ListItem();
            string sValue = string.Empty;

            foreach (RadPanelItem item1 in radPanelBarAccounts.GetAllItems())
            {
                if (item1.Value != string.Empty)
                {
                    itemID = item1.Value;
                }

                RadioButtonList rblSites = (RadioButtonList)item1.FindControl(itemID);

                if (rblSites != null)
                {
                    sItem = rblSites.SelectedItem;
                    if (sItem != null)
                    {
                        if (itemID == HiddenField1.Value)
                        {
                            sItem.Selected = true;
                            Session["AHSAccountID"] = HiddenField1.Value;
                            Session["SITEID"] = sItem.Value;
                        }
                        else
                        {
                            sItem.Selected = false;
                        }

                        siteID = sItem.Value;
                        accountID = itemID;
                    }
                }
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.ClientScript.IsStartupScriptRegistered("SingleSite"))
            {
                //script to register
                String scriptStr = "<script language=\"JavaScript\"> function SelectSingleSite(panelItem) {";
                scriptStr += "var valueSelect=document.getElementById('" + hiddenSelect.ClientID + "');";
                scriptStr += "var hiddenField=document.getElementById('" + HiddenField1.ClientID + "');";
                scriptStr += "if (hiddenField != null) { hiddenField.value = panelItem; }";
                scriptStr += "if (valueSelect != null) { valueSelect.click(); }";
                scriptStr += "}<";
                scriptStr += "/";
                scriptStr += "script>";
                Page.ClientScript.RegisterStartupScript(typeof(Page), "SingleSite", scriptStr);
            }
            if (!string.IsNullOrEmpty(Request.QueryString["TargetUrl"]))
            {
                string UrlForRedirection = Constants.PageNames.UrlForRedirection(Request.QueryString["TargetUrl"].ToString());
                if (string.IsNullOrEmpty(UrlForRedirection))
                {
                    redirectURL = string.Empty;
                }
                else
                {
                    redirectURL = UrlForRedirection;
                }
            }
            else
            {
                redirectURL = string.Empty;
            }

            //  redirectURL = (Request.QueryString["TargetUrl"] != null ) ? Request.QueryString["TargetUrl"].ToString() : string.Empty;
        }

        protected void Page_LoadComplete(object sender, EventArgs e)
        {
            ((PhysicianMasterPageBlank)Master).hideTabs();
            ((PhysicianMasterPageBlank)Master).hideMessageQueue();
            ((PhysicianMasterPageBlank)Master).HideHelp();
            ((PhysicianMasterPageBlank)Master).HideILearn();
            ((PhysicianMasterPageBlank)Master).HideSiteAndUser();
        }

        protected void btnSelect_Click(object sender, EventArgs e)
        {
            int licenseCount = 1;
            ePrescribeSvc.AuthenticatedShieldUser[] authenticatedShieldUsers = null;
            ePrescribeSvc.AuthenticatedShieldUser authenticatedShieldUser = null;
            bool isUserLogInfoSet = false;

            if (Session["AuthenticatedShieldUsersCount"] != null)
            {
                licenseCount = Convert.ToInt32(Session["AuthenticatedShieldUsersCount"]);
                authenticatedShieldUsers = new ePrescribeSvc.AuthenticatedShieldUser[licenseCount];
                authenticatedShieldUser = new ePrescribeSvc.AuthenticatedShieldUser();
                authenticatedShieldUsers = (ePrescribeSvc.AuthenticatedShieldUser[])Session["AuthenticatedShieldUsers"];
            }

            Session.Remove("InterstitialAd");
            Session.Remove("AdvertiseDeluxe");

            //string redirectURL = "../login.aspx";        
            string msg = string.Empty;
            Login _login = new Login(this);

            string itemID = string.Empty;
            string accountID = string.Empty;
            string siteID = string.Empty;
            ListItem sItem = new ListItem();
            string sValue = string.Empty;

            foreach (RadPanelItem item1 in radPanelBarAccounts.GetAllItems())
            {
                if (item1.Value != string.Empty)
                {
                    itemID = item1.Value;
                }

                RadioButtonList rblSites = (RadioButtonList)item1.FindControl(itemID);

                if (rblSites != null)
                {
                    sItem = rblSites.SelectedItem;
                    if (sItem != null)
                    {
                        siteID = sItem.Value;
                        accountID = itemID;
                    }
                }
            }

            if (!string.IsNullOrEmpty(accountID) && !string.IsNullOrEmpty(siteID))
            {
                ePrescribeSvc.License license = new ePrescribeSvc.License();

                if (authenticatedShieldUsers != null)
                {
                    authenticatedShieldUser = authenticatedShieldUsers.Single(u => u.AccountID == accountID);
                    Session["LastLoginDateUTC"] = authenticatedShieldUser.LastLoginDateUTC;
                    license = EPSBroker.GetLicenseByAccountID(accountID, (ConnectionStringPointer)authenticatedShieldUser.DBID);
                }
                else
                {
                    license = EPSBroker.GetLicenseByAccountID(accountID, base.DBID);
                }

                ePrescribeSvc.LicenseCreationDateResponse licCreationDateResponse = EPSBroker.GetLicenseCreationDate(license.LicenseID);
                PageState[SessionVariables.LicenseCreationDate] = (licCreationDateResponse?.LicenseCreationDate != null) ?
                                                                    licCreationDateResponse.LicenseCreationDate : DateTime.Now;

                Session["SITEID"] = siteID;

                var session = PageState;


                //The license hasn't been set or
                //User has selected a different account, Create RxPrincipal and set session variables
                if (Session["LicenseID"] == null ||
                !Session["LicenseID"].ToString().Equals(license.LicenseID, StringComparison.OrdinalIgnoreCase))
                {
                    //Create RxPrincipal and set session variables
                    if (authenticatedShieldUsers != null)
                    {
                        PageState.Remove(Constants.SessionVariables.ShieldInternalTenantID);
                        PageState.Remove(Constants.SessionVariables.ShieldExternalTenantID);
                        PageState.Remove(Constants.SessionVariables.AppInstanceID);

                        isUserLogInfoSet = _login.SetUserSessionInfo(authenticatedShieldUser, ref redirectURL, ref msg);
                    }

                    if (!isUserLogInfoSet)
                    {
                        Response.Redirect(string.Concat(Constants.PageNames.UrlForRedirection(redirectURL) + "?msg=" + msg));
                    }
                }

                new Login(this.Page).AsyncInitCommonCompInfoProviderAndSite(authenticatedShieldUser.UserGUID, Convert.ToInt32(siteID), DBID);

                // 
                // Check to see if this manual sign in is allowed.
                // This check is already done for Shield Off users and Shield On users that have only 1 ePrescribe practice tied to their Shield account.
                // However, we need to do this check here for for users that have multiple ePrescribe practices tied to their Shield account since we only
                // know the userGUID of their selected practice after they select it.
                //
                if (authenticatedShieldUsers != null && authenticatedShieldUsers.Length > 1)
                {
                    bool isAllowedToSignIn = RxUser.IsAllowedToSignInWithUsernameAndPassword(base.SessionUserID, base.DBID);

                    if (!isAllowedToSignIn)
                    {
                        Response.Redirect(string.Concat(Constants.PageNames.LOGOUT, "?msg=", Constants.VALID_CREDENTIALS_BUT_NOT_AUTHORIZED_BY_PARTNER));
                    }
                }

                string redirect_url = string.Empty;

                if (!string.IsNullOrEmpty(redirectURL))
                {
                    redirect_url = redirectURL;
                }

                DataSet dsSite = ApplicationLicense.SiteLoad(Session["LicenseID"].ToString(), Convert.ToInt32(siteID), base.DBID);
                Session["PRACTICESTATE"] = dsSite.Tables[0].Rows[0]["State"];
                Session["ISCSREGISTRYCHECKREQ"] = dsSite.Tables[0].Rows[0]["IsCSRegistryCheckRequired"];
                Session["STATEREGISTRYURL"] = dsSite.Tables[0].Rows[0]["StateRegistryURL"];
                Session["SITENAME"] = dsSite.Tables[0].Rows[0]["SiteName"];
                Session["SITE_PHARMACY_ID"] = dsSite.Tables[0].Rows[0]["PharmacyID"];
                Session["CURRENT_SITE_ZONE"] = dsSite.Tables[0].Rows[0]["TimeZone"];
                // 
                // if the state of the selected site has any EPCS authrozied schedules, save them for later reference
                //
                List<string> siteEPCSAuthorizedSchedules = new List<string>();


                if (dsSite.Tables[1] != null & dsSite.Tables[1].Rows.Count > 0)
                {
                    foreach (DataRow drSchedule in dsSite.Tables[1].Rows)
                    {
                        siteEPCSAuthorizedSchedules.Add(drSchedule["Schedule"].ToString());
                    }
                }

                if (!string.IsNullOrEmpty(PageState.GetStringOrEmpty("LicenseID")))
                {
                    EPSBroker.AddILearnUser(authenticatedShieldUser?.ShieldLoginID, PageState.GetStringOrEmpty("LicenseID"), authenticatedShieldUser?.FirstName, authenticatedShieldUser?.LastName, base.DBID);
                }
                Session["SiteEPCSAuthorizedSchedules"] = siteEPCSAuthorizedSchedules;

                //set the cookie
                HttpCookie eRxNowSiteCookie;
                if (Request.Cookies["eRxNowSiteCookie"] != null)
                {
                    eRxNowSiteCookie = Request.Cookies.Get("eRxNowSiteCookie");
                    string cLicenseID = eRxNowSiteCookie["License"] != null ? eRxNowSiteCookie["License"] : string.Empty;
                    string cSiteID = eRxNowSiteCookie["SaveSite"] != null ? eRxNowSiteCookie["SaveSite"] : string.Empty;
                    eRxNowSiteCookie.Values.Remove("SaveSite");
                    eRxNowSiteCookie.Values.Remove("License");
                    Response.Cookies.Remove(eRxNowSiteCookie.Name);
                    if (Session["LICENSEID"].ToString() != cLicenseID || siteID != cSiteID)
                    {
                        eRxNowSiteCookie = new HttpCookie("eRxNowSiteCookie");
                        eRxNowSiteCookie.Expires = DateTime.Parse(DateTime.Today.ToShortDateString()).AddDays(-1);
                        Response.Cookies.Add(eRxNowSiteCookie);
                    }
                }


                if (chkCookie.Checked)
                {
                    eRxNowSiteCookie = new HttpCookie("eRxNowSiteCookie");
                    eRxNowSiteCookie.Values.Add("SaveSite", siteID);
                    eRxNowSiteCookie.Values.Add("License", Session["LICENSEID"].ToString());
                    eRxNowSiteCookie.Expires = DateTime.Parse(DateTime.Today.ToShortDateString()).AddDays(1);
                    //Replace the eRxNow cookie
                    Response.Cookies.Add(eRxNowSiteCookie);
                }
                else if (chkDefault.Checked)
                {
                    eRxNowSiteCookie = new HttpCookie("eRxNowSiteCookie");
                    eRxNowSiteCookie.Values.Add("SaveSite", siteID);
                    eRxNowSiteCookie.Values.Add("License", Session["LICENSEID"].ToString());
                    eRxNowSiteCookie.Expires = DateTime.Parse(DateTime.Today.ToShortDateString()).AddYears(1);
                    //Replace the eRxNow cookie
                    Response.Cookies.Add(eRxNowSiteCookie);
                }

                if (Response.Cookies["eRxTIECookie"] != null) Response.Cookies.Remove("eRxTIECookie");

                Response.Cookies.Add(TieUtility.CreateTieCookie(
                        dsSite,
                        PageState.GetStringOrEmpty("SpecialtyCode1"),
                        PageState.GetStringOrEmpty("SpecialtyCode2"),
                        license.EnterpriseClientID.ToGuidOr0x0(),
                        license.LicenseID.ToGuidOr0x0(),
                        PageState.GetInt(Constants.SessionVariables.LogRxUserGuid, 0),
                        PageState,
                        DBID));

                if ((Convert.ToInt32(Session["UserType"]) == Convert.ToInt32(Constants.UserCategory.PROVIDER)) ||
                (Convert.ToInt32(Session["UserType"]) == Convert.ToInt32(Constants.UserCategory.PHYSICIAN_ASSISTANT)) ||
                (Convert.ToInt32(Session["UserType"]) == Convert.ToInt32(Constants.UserCategory.PHYSICIAN_ASSISTANT_SUPERVISED)))
                {
                    //but now we need to check if this user has a valid state license for this site/state
                    if (!RxUser.VerifyUserStateLicense(Session["USERID"].ToString(), Session["PRACTICESTATE"].ToString(), base.DBID))
                    {
                        if (!redirect_url.ToLower().Contains(Constants.PageNames.EDIT_USER.ToLower()))
                        {
                            redirect_url = Constants.PageNames.EDIT_USER + "?Status=NoLic&TargetUrl=" + redirect_url.Replace("~/", "");
                            redirect_url = $"{Constants.PageNames.SPA_LANDING}?page={redirect_url}";
                        }
                    }
                }

                string printingOption = "1";

                //first check to see if the state of the site is eligible for 4up.
                if (SystemConfig.GetStatePrintFormats(Session["PRACTICESTATE"].ToString()) > 0)
                {
                    string printingPreference = SitePreference.GetPreference(SessionLicenseID, int.Parse(Session["SITEID"].ToString()), "PRINTINGPREFERENCE", base.DBID);
                    if (printingPreference != null)
                    {
                        printingOption = printingPreference;
                    }
                }

                Session["PRINTINGPREFERENCE"] = printingOption;
                Session["GenericEquivalentSearch"] = SitePreference.GetPreference(Session["LICENSEID"].ToString(), Convert.ToInt32(Session["SITEID"].ToString()), "GenericEquivalentSearch", base.DBID);
                PageState["ALLOWPATIENTRECEIPT"] = SitePreference.GetPreference(PageState.GetStringOrEmpty("LICENSEID"), int.Parse(PageState.GetString("SITEID", "1")), "ALLOWPATIENTRECEIPT", base.DBID);
                PageState["ALLOWMDD"] = SitePreference.GetPreference(PageState.GetStringOrEmpty("LICENSEID"), int.Parse(PageState.GetString("SITEID", "1")), "ALLOWMDD", base.DBID);
                PageState["CSMEDSONLY"] = SitePreference.GetPreference(PageState.GetStringOrEmpty("LICENSEID"), int.Parse(PageState.GetString("SITEID", "1")), "CSMEDSONLY", base.DBID);

                if (base.SessionLicense.EnterpriseClient.ShowECoupon)
                {
                    Session["APPLYFINANCIALOFFERS"] = SitePreference.GetPreference(base.SessionLicenseID, base.SessionSiteID, "APPLYFINANCIALOFFERS", base.DBID);
                }

                if (base.SessionLicense.EnterpriseClient.ShowSponsoredLinks)
                {
                    Session["DisplayInfoScripts"] = SitePreference.GetPreference(base.SessionLicenseID, base.SessionSiteID, "DISPLAYINFOSCRIPTS", base.DBID);
                }

                if (redirect_url.Trim().Length == 0)
                {
                    base.RedirectToInterstitialAdIfNeeded(string.Empty);
                    DefaultRedirect(false);
                }
                else
                {
                    base.RedirectToInterstitialAdIfNeeded(redirect_url);
                    if (redirect_url.ToLower().Contains("status=expdea") &&
                            (redirect_url.StartsWith("~/" + Constants.PageNames.EDIT_USER, StringComparison.OrdinalIgnoreCase) || redirect_url.StartsWith(Constants.PageNames.EDIT_USER, StringComparison.OrdinalIgnoreCase)))
                    {
                        if (String.Compare(redirect_url.Substring(0, 2), "~/", true) == 0)
                        {
                            redirect_url = redirect_url.Substring(2, redirect_url.Length - 2);  //Remove "~/"
                        }
                        PageState[Constants.SessionVariables.AppComponentAlreadyInitialized] = true;
                        Response.Redirect(RedirectHelper.GetRedirectToSPAUrl(redirect_url, false));
                    }
                    else if (redirect_url.ToLower().Contains(Constants.PageNames.USER_EULA.ToLower()) && redirect_url.ToLower().Contains("status=expdea"))
                    {
                        if (String.Compare(redirect_url.Substring(0, 2), "~/", true) == 0)
                        {
                            redirect_url = redirect_url.Substring(2, redirect_url.Length - 2);  //Remove "~/"
                        }
                        Response.Redirect(RedirectHelper.GetRedirectToSPAUrl(redirect_url, false));
                    }
                    else
                    {
                        Response.Redirect(Constants.PageNames.UrlForRedirection(redirect_url));
                    }
                }

                if (redirect_url != string.Empty)
                {
                    Response.Redirect(Constants.PageNames.UrlForRedirection(redirect_url));
                }

                else
                {
                    var homeAddressCheckStatus = session.Cast(Constants.SessionVariables.HomeAddressCheckStatus, HOME_ADDRESS_CHECK_STATUS.NOT_VERIFIED);
                    var userId = session.GetStringOrEmpty(Constants.SessionVariables.UserId);
                    switch (homeAddressCheckStatus)
                    {
                        case HOME_ADDRESS_CHECK_STATUS.NOT_VERIFIED:
                            Audit.AddLogEntryIdology(/*Request.UserHostName*/string.Empty, "SelectPatientApiController", userId.ToGuidOr0x0(), "Home address status not verified, sending to IdProofingRequiredInfo");
                            session[Constants.SessionVariables.AddressVerifyRetryCount] = 1;
                            Response.Redirect(Constants.PageNames.IDPROOFING_REQUIRED_INFO);
                            break;
                        case HOME_ADDRESS_CHECK_STATUS.LOCKED:
                            Audit.AddLogEntryIdology(/*Request.UserHostName*/string.Empty, "SelectPatientApiController", userId.ToGuidOr0x0(), "Home address status locked, sending to Address error page.");
                            Response.Redirect(Constants.PageNames.ADDRESS_VERIFY_ERROR);
                            break;
                        default:
                            SelectPatientComponentParameters selectPatientComponentParameters = new SelectPatientComponentParameters
                            {
                                PatientId = PageState.GetStringOrEmpty(Constants.SessionVariables.PatientId)
                            };
                            RedirectToSelectPatient(null, selectPatientComponentParameters);
                            break;
                    }

                }
            }
        }

        protected void radPanelBarAccounts_Init(object sender, EventArgs e)
        {
            Constants.DeluxeFeatureStatus deluxeStatus = Constants.DeluxeFeatureStatus.Off;
            bool isSiteSelected = false;
            int pointer = 0;

            if (Session["AHSAccountID"] != null && !string.IsNullOrEmpty(PageState.GetStringOrEmpty(Constants.SessionVariables.SiteId)))
            {
                isSiteSelected = true;
            }

            int licenseCount = 1;
            ePrescribeSvc.AuthenticatedShieldUser[] authenticatedShieldUsers = null;

            if (Session["AuthenticatedShieldUsersCount"] != null)
            {
                licenseCount = Convert.ToInt32(Session["AuthenticatedShieldUsersCount"]);
                authenticatedShieldUsers = new ePrescribeSvc.AuthenticatedShieldUser[licenseCount];
                authenticatedShieldUsers = (ePrescribeSvc.AuthenticatedShieldUser[])Session["AuthenticatedShieldUsers"];
            }

            ePrescribeSvc.License[] licenses = new ePrescribeSvc.License[licenseCount];

            if (Session["AuthenticatedShieldUsersCount"] != null)
            {
                foreach (ePrescribeSvc.AuthenticatedShieldUser authenticatedShieldUser in authenticatedShieldUsers)
                {
                    ePrescribeSvc.License license = EPSBroker.GetLicenseByAccountID(authenticatedShieldUser.AccountID, (ConnectionStringPointer)authenticatedShieldUser.DBID);
                    licenses[pointer] = license;
                    pointer++;
                }
            }

            pointer = 0;
            foreach (ePrescribeSvc.License license in licenses)
            {
                if (!string.IsNullOrEmpty(license.EnterpriseClientID) &&
                    !license.EnterpriseClientID.Equals(_allscriptsEnterpriseClientID, StringComparison.OrdinalIgnoreCase))
                {
                    EnterpriseClient enterpriseClient = new EnterpriseClient(license.EnterpriseClientID);
                    Session["EnterpriseID"] = enterpriseClient.ID;

                    deluxeStatus = (Constants.DeluxeFeatureStatus)license.LicenseDeluxeStatus;

                    if ((deluxeStatus == Constants.DeluxeFeatureStatus.On || deluxeStatus == Constants.DeluxeFeatureStatus.Cancelled)
                        && !string.IsNullOrEmpty(enterpriseClient.DeluxeStyleSheet))
                    {
                        Session["Theme"] = enterpriseClient.DeluxeStyleSheet;
                    }
                    else
                    {
                        Session["Theme"] = enterpriseClient.StyleSheet;
                    }

                    radPanelBarAccounts.Skin = "Default";

                    Session["PageTitle"] = enterpriseClient.PageTitle + " ";
                }

                RadPanelItem rootItem = new RadPanelItem();
                RadioButtonList rblSites = new RadioButtonList();

                rblSites.ID = license.AccountID.ToHTMLEncode();

                foreach (ePrescribeSvc.LicenseSite licSite in license.LicenseSites)
                {
                    if (licSite.Active)
                    {
                        ListItem siteList = new ListItem();
                        siteList.Text = licSite.SiteName;
                        siteList.Selected = false;
                        if (Session["AHSAccountID"] != null && Session["AHSAccountID"].ToString() == license.AccountID)
                        {
                            if (!string.IsNullOrEmpty(PageState.GetStringOrEmpty(Constants.SessionVariables.SiteId)) && Convert.ToInt32(Session["SITEID"]) == licSite.SiteID)
                            {
                                siteList.Selected = true;
                            }
                        }

                        if (!isSiteSelected)
                        {
                            siteList.Selected = true;
                            isSiteSelected = true;
                        }

                        siteList.Value = licSite.SiteID.ToString().ToHTMLEncode();

                        rblSites.Items.Add(siteList);
                    }
                }

                RadPanelItem childItem = new RadPanelItem();
                childItem.Controls.Add(rblSites);
                rootItem.Items.Add(childItem);

                rootItem.Value = license.AccountID;
                rootItem.Text = license.LicenseName;

                if (authenticatedShieldUsers != null)
                {
                    //Check to see if the Tenant Name on Shield is different than the License Name in eRxDB. If different, let's update eRxDB.
                    if (!license.LicenseName.Equals(authenticatedShieldUsers[pointer].ShieldTenantName))
                    {
                        ApplicationLicense.UpdateLicenseName(
                            license.LicenseID,
                            authenticatedShieldUsers[pointer].ShieldTenantName,
                            (ConnectionStringPointer)authenticatedShieldUsers[pointer].DBID);

                        rootItem.Text = authenticatedShieldUsers[pointer].ShieldTenantName;
                    }
                }

                if (Session["AHSAccountID"] != null)
                {
                    if (rootItem.Value == Session["AHSAccountID"].ToString())
                    {
                        rootItem.Expanded = true;
                    }
                }

                if (Session["AHSAccountID"] == null)
                {
                    rootItem.Expanded = true;
                }

                radPanelBarAccounts.Items.Add(rootItem);

                pointer++;
            }

            int panelHeight = licenseCount * 50;
            if (panelHeight > 200)
            {
                radPanelBarAccounts.Height = panelHeight;
            }
        }
    }
}