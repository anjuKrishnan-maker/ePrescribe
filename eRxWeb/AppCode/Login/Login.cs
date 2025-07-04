#define TRACE
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Xml;
using System.Text;
using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.Shared.Logging;
using Allscripts.Impact;
using System.Collections.Specialized;
using System.Threading;
using Allscripts.ePrescribe.Data;
using Allscripts.ePrescribe.ExtensionMethods;
using Allscripts.ePrescribe.Objects;
using Allscripts.ePrescribe.Objects.PPTPlus;
using eRxWeb.AppCode;
using eRxWeb.AppCode.Interfaces;
using Google.GData.Client;
using eRxWeb.State;
using Microsoft.IdentityModel.Claims;
using Telerik.Web.UI;
using DUR = Allscripts.Impact.DUR;
using EnterpriseClient = Allscripts.Impact.EnterpriseClient;
using PptPlus = Allscripts.Impact.PptPlus;
using RxUser = Allscripts.Impact.RxUser;
using SystemConfig = Allscripts.Impact.SystemConfig;
using eRxWeb.AppCode.PptPlusBPL;
using eRxWeb.AppCode.PdmpBPL;
using Allscripts.ePrescribe.Data.CommonComponent;
using Allscripts.Impact.ePrescribeSvc;
using ConnectionStringPointer = Allscripts.ePrescribe.DatabaseSelector.ConnectionStringPointer;
using System.Security.Principal;
using System.Web.UI;
using eRxWeb.Controller;
using System.Web.SessionState;
using GetUserShieldCspStatusInfoResponse = eRxWeb.ePrescribeSvc.GetUserShieldCspStatusInfoResponse;

namespace eRxWeb
{
    /// <summary>
    /// Summary description for Login
    /// </summary>
    public class Login : ILogin
    {
        private static readonly ILoggerEx logger = LoggerEx.GetLogger();

        RxPrincipal _rxPrincipal = null;
        IPageState _form;
        public IStateContainer PageState { get; set; }

        public Login(Page f)
        {
            PageState = new StateContainer(f.Session);
            _form = new FormContainer(f);
        }

        public Login(PageContext pageContext)
        {
            _form = pageContext;
            PageState = new StateContainer(pageContext.Session);
        }

        public bool ReLogUserIn(string userguid, ref string redirect_url, ref string msg)
        {
            bool isSuccessfulLogin = true;

            ConnectionStringPointer dbID = (ConnectionStringPointer)PageState["DBID"];
            try
            {
                _rxPrincipal = new RxPrincipal(userguid, Guid.Empty.ToString(), 1, dbID);
                if (!(((RxIdentity)_rxPrincipal.Identity).IsAccountActive))
                {
                    msg = "Your account is marked as inactive, please contact the Administrator to activate your account.";
                    return false;
                }
            }
            catch (ApplicationException ex)
            {
                isSuccessfulLogin = false;
                if (ex.Message == "PASSWORDEXPIRED")
                {
                    PageState["USERCODE"] = userguid;
                    redirect_url = "~/ChangePassword.aspx";
                }
                else
                {
                    msg = "Invalid Username or Password"; //Change Error message from system to user defined: By Abdhul 29/08/06
                }
            }
            catch (System.Exception sysEx)
            {
                msg = sysEx.Message;
                isSuccessfulLogin = false;
            }

            if (isSuccessfulLogin)
            {
                isSuccessfulLogin = SetUserSessionInfo(ref redirect_url, ref msg, userguid, dbID);
            }

            return isSuccessfulLogin;
        }

        public bool SetUserSessionInfo(ref string redirect_url, ref string msg, string userguid, ConnectionStringPointer dbID)
        {
            return SetUserSessionInfo(ref redirect_url, ref msg, userguid, null, null, null, null, null, null, null, null, null, ePrescribeSvc.ShieldTenantIDProofingModel.None, dbID);
        }

        public bool SetUserSessionInfo(ePrescribeSvc.AuthenticatedShieldUser authenticatedShieldUser, ref string redirect_url, ref string msg)
        {
            //set the refresh authentication token date time to the "valid from" date plus half of the token lifetime
            TimeSpan tokenLifetime = authenticatedShieldUser.AuthenticationTokenValidTo - authenticatedShieldUser.AuthenticationTokenValidFrom;

            PageState[Constants.SessionVariables.ShieldTokenTimeRemainingMs] = authenticatedShieldUser.AuthenticationTokenValidTo;
            DateTime refreshAuthTokenDateTimeUTC = authenticatedShieldUser.AuthenticationTokenValidFrom.AddMinutes(tokenLifetime.TotalMinutes / 2);

            PageState[Constants.SessionVariables.HomeAddressCheckStatus] = authenticatedShieldUser.HomeAddressCheckStatus;
            PageState[Constants.SessionVariables.ShieldIdentityToken] = authenticatedShieldUser.ShieldIdentityToken;
            PageState["ShieldExternalTenantID"] = authenticatedShieldUser.ShieldExternalTenantID;            

            return SetUserSessionInfo(
            ref redirect_url,
            ref msg,
            authenticatedShieldUser.UserGUID,
            authenticatedShieldUser.FirstName,
            authenticatedShieldUser.LastName,
            authenticatedShieldUser.SecurityToken,
            authenticatedShieldUser.ShieldLoginID,
            authenticatedShieldUser.ShieldADUserID,
            authenticatedShieldUser.AppRoles,
            authenticatedShieldUser.AppPermissions,
            authenticatedShieldUser.AppProperty,
            refreshAuthTokenDateTimeUTC,
            authenticatedShieldUser.TenantIdProofingModel,
            (ConnectionStringPointer)authenticatedShieldUser.DBID);
        }

        /// <summary>
        /// Sets all the session variables for this user's session
        /// </summary>
        /// <returns></returns>
        public bool SetUserSessionInfo(
            ref string redirect_url,
            ref string msg,
            string userGUID,
            string firstName,
            string lastName,
            string securityToken,
            string shieldLoginID,
            string shieldADUserID,
            ePrescribeSvc.Role[] appRoles,
            ePrescribeSvc.Permission[] appPermissions,
            ePrescribeSvc.Property[] appProperties,
            DateTime? refreshAuthTokenDateTimeUTC,
            ePrescribeSvc.ShieldTenantIDProofingModel tenantIdProofingModel,
            ConnectionStringPointer dbID)
        {
            logger.Debug("SetUserSessionInfo");
            bool isSessionSuccessfullySet = true;


            if (_rxPrincipal == null)
            {
                _rxPrincipal = new RxPrincipal(userGUID, string.Empty, 1, dbID);
            }

            try
            {
                if ((((RxIdentity)_rxPrincipal.Identity).CurrentLicense.Status != Constants.LicenseStatus.Online))//process runs
                {
                    msg = ((RxIdentity)_rxPrincipal.Identity).CurrentLicense.StatusMessage;
                    redirect_url = "~/" + Constants.PageNames.LOGIN;
                    PageState.Clear();
                    FormsAuthentication.SignOut();
                    return false;
                }

                PageState["DBID"] = dbID;
                PageState["ShieldADUserID"] = shieldADUserID;
                PageState["RefreshAuthTokenDateTimeUTC"] = refreshAuthTokenDateTimeUTC;

                if (ConfigurationManager.AppSettings["SAMLTokenStorageLocation"].ToString().Equals("Session"))
                {
                    PageState["ShieldSecurityToken"] = securityToken;
                }

                PageState[Constants.SessionVariables.UserAppPermissions] = appPermissions;
                PageState[Constants.SessionVariables.UserAppProperties] = appProperties;

                PageState["UserFirstName"] = firstName;
                PageState["UserLastName"] = lastName;

                if (((RxIdentity)_rxPrincipal.Identity).User.GetAttribute("Title") == null || ((RxIdentity)_rxPrincipal.Identity).User.GetAttribute("Title") == Convert.DBNull || ((RxIdentity)_rxPrincipal.Identity).User.GetAttribute("Title").ToString() == string.Empty)
                {
                    PageState["UserName"] = firstName + " " + lastName;
                }
                else
                {
                    PageState["UserName"] = ((RxIdentity)_rxPrincipal.Identity).User.GetAttribute("Title") + " " + lastName;
                }

                PageState["SessionShieldUserName"] = shieldLoginID;

                ApplicationLicense license = new ApplicationLicense(((RxIdentity)_rxPrincipal.Identity).LicenseID, 1, ((RxIdentity)_rxPrincipal.Identity).CurrentLicense.EnterpriseClientID, dbID);

                PageState["SessionLicense"] = license;

                EnterpriseClient ec = license.EnterpriseClient;
                if (ec != null)
                {
                    PageState["EnterpriseID"] = ec.ID;
                    PageState["ShowCSRegistry"] = ec.ShowCSRegistry;

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
                    PageState["ISENTCLIENTEPCSENABLED"] = ec.IsEPCSEnabled;
                    PageState["ShowADPlacement"] = ec.ShowADPlacement;

                    PageState[Constants.SessionVariables.TenantIdProofingModel] = tenantIdProofingModel;


                    string licenseID = ((RxIdentity)_rxPrincipal.Identity).LicenseID;

                    if (!string.IsNullOrEmpty(ec.DefaultPatientLockDownPage))
                    {
                        PageState["DefaultPatientLockDownPage"] = ec.DefaultPatientLockDownPage;
                    }
                }

                PageState["USEREMAIL"] = ((RxIdentity)_rxPrincipal.Identity).User.GetAttribute("email");

                if (Convert.ToBoolean(PageState["IsLicenseShieldEnabled"]))
                {
                    PageState["USERCODE"] = shieldLoginID;
                }
                else
                {
                    PageState["USERCODE"] = ((RxIdentity)_rxPrincipal.Identity).User.GetAttribute("UserName");
                }

                PageState["EprescribeUsername"] = ((RxIdentity)_rxPrincipal.Identity).User.GetAttribute("UserName");

                PageState["USERID"] = userGUID;
                PageState["ISPROVIDER"] = ((RxIdentity)_rxPrincipal.Identity).User.IsProvider;
                PageState["USERZIP"] = ((RxIdentity)_rxPrincipal.Identity).User.GetAttribute("ZipCode");

                PageState["LICENSEID"] = ((RxIdentity)_rxPrincipal.Identity).LicenseID;
                PageState["AccountID"] = ((RxIdentity)_rxPrincipal.Identity).AccountID;
                PageState["CustAccountID"] = ((RxIdentity)_rxPrincipal.Identity).CustAccountID;
                //JT - hardcoded to since we no longer use this sponsored concept
                PageState["SPONSORED"] = 1;
                //end JT edit
                PageState["PRACTICESTATE"] = ((RxIdentity)_rxPrincipal.Identity).CurrentLicense.PracticeState;
                PageState["ISCSREGISTRYCHECKREQ"] = ((RxIdentity)_rxPrincipal.Identity).CurrentLicense.isCSRegistryCheckRequired;
                PageState["STATEREGISTRYURL"] = ((RxIdentity)_rxPrincipal.Identity).CurrentLicense.StateRegistryURL;
                PageState["SITENAME"] = ((RxIdentity)_rxPrincipal.Identity).SiteName;
                PageState["LicenseName"] = ((RxIdentity)_rxPrincipal.Identity).CurrentLicense.LicenseName;
                PageState["STANDING"] = (((RxIdentity)_rxPrincipal.Identity).CurrentLicense.Standing == 1) ? "1" : "0";
                PageState["PERFORM_FORMULARY"] = ((RxIdentity)_rxPrincipal.Identity).User.PERFORM_FORMULARY;
                PageState["FORM_REQUIRE_REASON"] = ((RxIdentity)_rxPrincipal.Identity).User.FORM_REQUIRE_REASON;

                PageState[Constants.SessionVariables.DURSettings] = DUR.BuildSettingsObject(((RxIdentity)_rxPrincipal.Identity).User);

                PageState["HasAcceptedEula"] = ((RxIdentity)_rxPrincipal.Identity).User.HasAcceptedEULA;
                PageState[Constants.SessionVariables.IsIdProofingRequired] = getUserIdProofingStatus();

                bool isAdmin = false;
                string userGroups = string.Empty;

                foreach (ePrescribeSvc.Role role in appRoles)
                {
                    switch (role.Name.ToLower())
                    {
                        case "admin":
                            isAdmin = true;
                            userGroups += "Administrators" + ",";
                            break;
                        case "provider":
                            PageState[Constants.SessionVariables.UserType] = (int)Constants.UserCategory.PROVIDER;
                            break;
                        case "physician assistant":
                            PageState[Constants.SessionVariables.UserType] = (int)Constants.UserCategory.PHYSICIAN_ASSISTANT;
                            break;
                        case "physician assistant supervised":
                            PageState[Constants.SessionVariables.UserType] = (int)Constants.UserCategory.PHYSICIAN_ASSISTANT_SUPERVISED;
                            break;
                        case "pob - all review":
                            PageState[Constants.SessionVariables.UserType] = (int)Constants.UserCategory.POB_LIMITED;
                            break;
                        case "pob - some review":
                            PageState[Constants.SessionVariables.UserType] = (int)Constants.UserCategory.POB_REGULAR;
                            break;
                        case "pob - no review":
                            PageState[Constants.SessionVariables.UserType] = (int)Constants.UserCategory.POB_SUPER;
                            break;
                        case "backdoor user":
                        case "single enterprise access user":
                            userGroups += "HelpDesk" + ",";
                            break;
                        default:
                            PageState[Constants.SessionVariables.UserType] = (int)Constants.UserCategory.GENERAL_USER;
                            break;
                    }
                }

                PageState["IsAdmin"] = isAdmin;
                PageState["UserGroup"] = userGroups;

                //jt - i don't think we need AppPermissions or AppProperties session vars anymore
                //PageState["AppPermissions"] = appPermissions;
                //PageState["AppProperties"] = appProperties;
                PageState["LicensePreferences"] = new LicensePreferences(((RxIdentity)_rxPrincipal.Identity));

                PageState["AHSAccountID"] = ((RxIdentity)_rxPrincipal.Identity).CurrentLicense.AHSAccountID;
                PageState["IsDelegateProvider"] = ((RxIdentity)_rxPrincipal.Identity).User.IsDelegateProvider;
                PageState["IsPrescriptionTOProvider"] = ((RxIdentity)_rxPrincipal.Identity).User.IsPrescriptionToProvider;
                PageState["IsPasswordExpired"] = ((RxIdentity)_rxPrincipal.Identity).IsPasswordExpired;
                PageState["IsPOBViewAllProviders"] = ((RxIdentity)_rxPrincipal.Identity).User.IsPOBViewAllProviders;
                PageState["TimeZone"] = ((RxIdentity)_rxPrincipal.Identity).CurrentLicense.TimeZone;
                PageState["CURRENT_SITE_ZONE"] = ((RxIdentity)_rxPrincipal.Identity).CurrentLicense.TimeZone;
                PageState["SHOW_CHECKED_IN_PTS"] = ((RxIdentity)_rxPrincipal.Identity).CurrentLicense.ShowCheckedInPatients;
                PageState["SHOW_SEND_TO_ADM"] = ((RxIdentity)_rxPrincipal.Identity).ShowSendToADM;
                PageState["UserHasHadEpcsPermissionInTheLastTwoYears"] = ((RxIdentity)_rxPrincipal.Identity).User.HasHadEpcsPermissionInTheLastTwoYears;
                PageState["SHOW_RX_INFO"] = ((RxIdentity)_rxPrincipal.Identity).CurrentLicense.ShowRxInfo;
                PageState[Constants.LicensePreferences.SHOW_RETROSPECTIVE_EPA] = ((RxIdentity)_rxPrincipal.Identity).CurrentLicense.ShowRePA;
                PageState[Constants.LicensePreferences.SHOW_EPA] = ((RxIdentity)_rxPrincipal.Identity).CurrentLicense.ShowEPA;
                PageState["ShowPrebuiltPrescriptions"] = ((RxIdentity)_rxPrincipal.Identity).CurrentLicense.ShowPrebuiltPrescriptions;
                PageState[Constants.LicensePreferences.SHOW_LEXICOMP] = ((RxIdentity)_rxPrincipal.Identity).CurrentLicense.ShowLexicomp;
                PageState["ShowLexicompDefault"] = ((RxIdentity)_rxPrincipal.Identity).CurrentLicense.ShowLexicompDefault;
                PageState["SpecialtyCode1"] = ((RxIdentity)_rxPrincipal.Identity).User.SpecialtyCode1;
                PageState["SpecialtyCode2"] = ((RxIdentity)_rxPrincipal.Identity).User.SpecialtyCode2;
                PageState["GENERIC_EQUIVALENT_SEARCH"] = ((RxIdentity)_rxPrincipal.Identity).User.GENERIC_EQUIVALENT_SEARCH;
                PageState[Constants.SessionVariables.ShouldShowPpt] = ((RxIdentity)_rxPrincipal.Identity).User.ShouldShowPpt;
                PageState[Constants.SessionVariables.ShouldShowRtbi] = ((RxIdentity)_rxPrincipal.Identity).User.ShouldShowRtbi;
                PageState[Constants.SessionVariables.ShouldShowRtps] = ((RxIdentity)_rxPrincipal.Identity).User.ShouldShowRtps;
                PageState[Constants.SessionVariables.ShouldPrintOfferAutomatically] = ((RxIdentity)_rxPrincipal.Identity).User.ShouldPrintOfferAutomatically;
                PageState[Constants.SessionVariables.ShouldShowTherapeuticAlternativeAutomatically] = ((RxIdentity)_rxPrincipal.Identity).User.ShouldShowTherapeuticAlternativeAutomatically;
                PageState[Constants.LicensePreferences.SHOW_PDMP] = ((RxIdentity)_rxPrincipal.Identity).CurrentLicense.ShowPDMP;
                PageState[Constants.SessionVariables.EcouponUncheckedWarningVisibility] = ((RxIdentity)_rxPrincipal.Identity).User.EcopuponUncheckedWarningVisibility;
                PageState[Constants.LicensePreferences.PatientUpload] = ((RxIdentity)_rxPrincipal.Identity).CurrentLicense.ShowPatientUpload;
                PageState[Constants.LicensePreferences.PatientChartExtract] = ((RxIdentity)_rxPrincipal.Identity).CurrentLicense.ShowChartExtract;

                //turn off patient receipt
                //PageState["PatientReceipt"] = ((RxIdentity)_rxPrincipal.Identity).User.PatientReceipt;
                PageState["PatientReceipt"] = false;
                PageState["IsPA"] = ((RxIdentity)_rxPrincipal.Identity).User.IsPhysicianAssistant;
                PageState["IsPASupervised"] = ((RxIdentity)_rxPrincipal.Identity).User.IsPhysicianAssistantSupervised;

                string redirectUrl = "";

                if (((RxIdentity)_rxPrincipal.Identity).User.HasUnreadTutorials)
                {
                    redirectUrl = "~/" + Constants.PageNames.TUTORIALS;
                    PageState["TutorialAvailable"] = "Y";
                }
                else
                {
                    redirectUrl = FormsAuthentication.GetRedirectUrl(_rxPrincipal.Identity.Name, true);
                }

                PageState["DEASTATUS"] = "ACTIVE";

                switch ((Constants.UserCategory)(PageState[Constants.SessionVariables.UserType]))
                {
                    case Constants.UserCategory.GENERAL_USER:
                        if (PageState["TutorialAvailable"] != null && PageState["TutorialAvailable"].ToString() == "Y")
                        {
                            redirect_url = "~/" + Constants.PageNames.TUTORIALS;
                        }
                        else
                        {
                            redirect_url = "~/" + Constants.PageNames.SELECT_PATIENT;
                        }

                        PageState["SPI"] = null;

                        break;
                    case Constants.UserCategory.PROVIDER:
                    case Constants.UserCategory.PHYSICIAN_ASSISTANT:
                    case Constants.UserCategory.PHYSICIAN_ASSISTANT_SUPERVISED:

                        if (!((RxIdentity)_rxPrincipal.Identity).User.HasNPI)
                        {
                            redirect_url = "~/" + Constants.PageNames.EDIT_USER + "?Status=NoNPI";
                        }
                        else if (((RxIdentity)_rxPrincipal.Identity).User.IsDEAExpired() || ((RxIdentity)_rxPrincipal.Identity).User.IsNADEANExpired())
                        {
                            //PageState["DEASTATUS"] = "EXPIRED";
                            redirect_url = "~/" + Constants.PageNames.EDIT_USER + "?Status=ExpDEA";
                            PageState["HasExpiredDEA"] = true;
                        }
                        //JMT - 11-05-2009 - no longer calling this as it only checks 1 license. The license check after site selection will catch invalid licenses.
                        //else if (((RxIdentity)_rxPrincipal.Identity).User.IsProviderLicenseExpired())
                        //{
                        //    redirect_url = "~/EditUser.aspx?Status=ExpLic";
                        //}
                        else
                        {
                            redirect_url = redirectUrl;
                        }

                        //save DEA to Session
                        PageState["DEA"] = ((RxIdentity)_rxPrincipal.Identity).User.DEA;
                        PageState["NPI"] = ((RxIdentity)_rxPrincipal.Identity).User.NPI;

                        List<string> deaSchedule = new List<string>();

                        //find the minimum allowed schedule
                        int maxsched = 0; //fake it at 6 (no such schedule)
                        int minsched = 0;

                        for (int i = 1; i < 6; i++)
                        {
                            if (((RxIdentity)_rxPrincipal.Identity).User.DEAScheduleAllowed(i))
                            {
                                deaSchedule.Add(i.ToString());
                                maxsched = i;
                            }
                        }

                        PageState["MAXSCHEDULEALLOWED"] = maxsched.ToString();
                        PageState["DEASCHEDULESALLOWED"] = deaSchedule;
                        deaSchedule.Sort();
                        if (deaSchedule.Count > 0)
                        {
                            minsched = Convert.ToInt32(deaSchedule[0]);
                        }
                        PageState["MINSCHEDULEALLOWED"] = minsched.ToString();

                        PageState["SPI"] = ((RxIdentity)_rxPrincipal.Identity).User.SPI;
                        PageState["LastEnrollmentDate"] = ((RxIdentity)_rxPrincipal.Identity).User.LastEnrollmentDate;


                        break;
                    case Constants.UserCategory.POB_SUPER:
                    case Constants.UserCategory.POB_REGULAR:
                    case Constants.UserCategory.POB_LIMITED:
                        if (PageState["TutorialAvailable"] != null && PageState["TutorialAvailable"].ToString() == "Y")
                        {
                            redirect_url = "~/" + Constants.PageNames.TUTORIALS;
                        }
                        else
                        {
                            redirect_url = "~/" + Constants.PageNames.SELECT_PATIENT;
                        }

                        PageState["SPI"] = null;
                        break;

                }

                if (redirect_url.ToLower().Contains(Constants.PageNames.SELECT_PATIENT.ToLower()))
                {
                    redirect_url = "~/";
                }

                if (!((RxIdentity)_rxPrincipal.Identity).User.HasAcceptedEULA)
                {
                    redirect_url = "~/" + Constants.PageNames.USER_EULA + "?TargetUrl=" + redirect_url.Replace("~/", "");
                }

                if (PageState["USERGROUP"] != null && PageState["USERGROUP"].ToString().Contains("HelpDesk"))
                {
                    redirect_url = "~/" + Constants.PageNames.HELP_DESK + "?TargetUrl=" + redirect_url.Replace("~/", "");
                }

                PPTPlus.SetPptPlusSamlToken(PageState);
                PDMP.SetPDMPToken(PageState);

                if (!string.IsNullOrEmpty(Convert.ToString(PageState["SITEID"])))
                {
                    var siteId = int.Parse(PageState["SITEID"].ToString());
                    string printingOption = "1";

                    //first check to see if the state of the site is eligible for 4up.
                    if (SystemConfig.GetStatePrintFormats(PageState["PRACTICESTATE"].ToString()) > 1)
                    {
                        string printingPreference = SitePreference.GetPreference(PageState["LICENSEID"].ToString(), siteId, "PRINTINGPREFERENCE", dbID);
                        if (printingPreference != null)
                        {
                            printingOption = printingPreference;
                        }
                    }
                    PageState.Remove("PRINTINGPREFERENCE");
                    PageState["PRINTINGPREFERENCE"] = printingOption;

                    PageState.Remove("GenericEquivalentSearch");
                    PageState["GenericEquivalentSearch"] = SitePreference.GetPreference(PageState["LICENSEID"].ToString(), siteId, "GenericEquivalentSearch", dbID);

                    if (ec.ShowECoupon)
                    {
                        PageState["APPLYFINANCIALOFFERS"] = SitePreference.GetPreference(PageState["LICENSEID"].ToString(), siteId, "APPLYFINANCIALOFFERS", dbID);
                    }

                    PageState.Remove("DisplayInfoScripts");
                    if (ec.ShowSponsoredLinks)
                    {
                        PageState["DisplayInfoScripts"] = SitePreference.GetPreference(PageState["LICENSEID"].ToString(), siteId, "DISPLAYINFOSCRIPTS", dbID);
                    }

                    PageState.Remove("ALLOWPATIENTRECEIPT");
                    PageState["ALLOWPATIENTRECEIPT"] = SitePreference.GetPreference(PageState["LICENSEID"].ToString(), siteId, "ALLOWPATIENTRECEIPT", dbID);

                    PageState.Remove("ALLOWMDD");
                    PageState["ALLOWMDD"] = SitePreference.GetPreference(PageState.GetStringOrEmpty("LICENSEID"), int.Parse(PageState.GetString("SITEID", "1")), "ALLOWMDD", dbID);
                    PageState.Remove("CSMEDSONLY");
                    PageState["CSMEDSONLY"] = SitePreference.GetPreference(PageState.GetStringOrEmpty("LICENSEID"), int.Parse(PageState.GetString("SITEID", "1")), "CSMEDSONLY", dbID);

                    AsyncInitCommonCompInfoProviderAndSite(userGUID, siteId, dbID);
                }

                // Create AD placement Cookies ...
                //Retrieve LogRxuserGuid from DMS and save in session
                PageState[Constants.SessionVariables.LogRxUserGuid] = TieUtility.RetrieveLogRxUserId(PageState.GetGuidOr0x0(Constants.SessionVariables.UserId));
                if (_form.Response.Cookies["eRxTIECookie"] != null) _form.Response.Cookies.Remove("eRxTIECookie");
                _form.Response.Cookies.Add(TieUtility.CreateTieCookie(
                    ((RxIdentity)_rxPrincipal.Identity).User.SpecialtyCode1,
                    ((RxIdentity)_rxPrincipal.Identity).User.SpecialtyCode2,
                    ec.ID.ToGuidOr0x0(), PageState,
                    dbID));

            }
            catch (System.Exception sysEx)
            {
                msg = sysEx.Message;
                isSessionSuccessfullySet = false;
            }

            return isSessionSuccessfullySet;
        }

        public void AsyncInitCommonCompInfoProviderAndSite(string userGUID, int siteId, ConnectionStringPointer dbID)
        {
            IStateContainer session = PageState;
            

            if (PPTPlus.IsPPTPlusEnterpriseOn(session)
                || PDMP.IsShowPDMP(session))
            {
                HttpContext context = HttpContext.Current;
                var logContext = LocalLogContext.LogContextInfo;

                ThreadPool.QueueUserWorkItem(state =>
                {
                    PptPlusRequestInfo request = null;
                    try
                    {
                        HttpContext.Current = context;
                        LocalLogContext.SetLoggingContext(logContext);

                        var dt = new CommonComponentData().GetProviderAndSiteInfo(userGUID.ToGuidOr0x0(), siteId, dbID);
                        DataRow providerAndSiteRow = dt.Rows[0];
                        session[Constants.SessionVariables.CommonCompProviderInfo] = Allscripts.ePrescribe.Objects.CommonComponent.ProviderInfo.Create(providerAndSiteRow);
                        session[Constants.SessionVariables.CommonCompSiteInfo] = Allscripts.ePrescribe.Objects.CommonComponent.SiteInfo.Create(providerAndSiteRow);
                    }
                    catch (Exception e)
                    {
                        Audit.AddException(userGUID, string.Empty, $"AsyncInitCommonCompInfoProviderAndSite Exception: <Exception>{e}</Exception><UserId>{userGUID}</UserId><SiteId>{siteId}</SiteId><InitProviderAndSiteResonse>{request?.StringifyObject()}</InitProviderAndSiteResonse>", "", null, null, dbID);
                    }
                });
            }
        }

        private void SetIsInstitutionalSessionVar(bool isInstitional)
        {
            PageState["IsInstitutional"] = isInstitional;
        }
        public static void LogUserOut(System.Web.UI.Page form, ref string redirect_url)
        {
            LogUserOut(form, false, false, false, ref redirect_url);
        }

        public static void LogUserOut(System.Web.UI.Page form, bool isUserSessionTimedOut, bool shouldForceReauthentication, bool hasAuthFailedForEPCS, ref string redirect_url)
        {
            string timeoutQueryString = "?Timeout=YES";
            string reauthenticateQueryString = "?reauthenticate=true";
            string authForEPCSQueryString = "?authfailedforepcs=true;";

            if (form.Session["PartnerLogoutUrl"] != null)
            {
                if (isUserSessionTimedOut)
                {
                    redirect_url = string.Concat(form.Session["PartnerLogoutUrl"].ToString(), timeoutQueryString);
                }
                else if (shouldForceReauthentication)
                {
                    redirect_url = string.Concat(form.Session["PartnerLogoutUrl"].ToString(), reauthenticateQueryString);
                }
                else if (hasAuthFailedForEPCS)
                {
                    redirect_url = string.Concat(form.Session["PartnerLogoutUrl"].ToString(), authForEPCSQueryString);
                }
                else
                {
                    redirect_url = form.Session["PartnerLogoutUrl"].ToString();
                }
            }
            else if (form.Session["EnterpriseClientLogoutURL"] != null)
            {
                if (isUserSessionTimedOut)
                {
                    redirect_url = string.Concat(form.Session["EnterpriseClientLogoutURL"].ToString(), timeoutQueryString);
                }
                else if (shouldForceReauthentication)
                {
                    redirect_url = string.Concat(form.Session["EnterpriseClientLogoutURL"].ToString(), reauthenticateQueryString);
                }
                else if (hasAuthFailedForEPCS)
                {
                    redirect_url = string.Concat(form.Session["EnterpriseClientLogoutURL"].ToString(), authForEPCSQueryString);
                }
                else
                {
                    redirect_url = form.Session["EnterpriseClientLogoutURL"].ToString();
                }
            }
            else
            {
                if (isUserSessionTimedOut)
                {
                    redirect_url = string.Concat(Constants.PageNames.LOGIN.ToLower(), timeoutQueryString);
                }
                else if (shouldForceReauthentication)
                {
                    redirect_url = string.Concat(Constants.PageNames.LOGIN.ToLower(), reauthenticateQueryString);
                }
                else if (hasAuthFailedForEPCS)
                {
                    redirect_url = string.Concat(Constants.PageNames.LOGIN.ToLower(), authForEPCSQueryString);
                }
                else
                {
                    redirect_url = Constants.PageNames.LOGIN.ToLower();
                }
            }

            form.Session.Abandon();
            FormsAuthentication.SignOut();
        }

        private void auditLogUserLoginInsert()
        {
            string licenseID = null;
            string userID = null;
            ConnectionStringPointer dbID = ConnectionStringPointer.ERXDB_DEFAULT;

            if (PageState["LicenseID"] != null)
            {
                licenseID = PageState["LicenseID"].ToString();
            }

            if (PageState["UserID"] != null)
            {
                userID = PageState["UserID"].ToString();
            }

            if (PageState["DBID"] != null)
            {
                dbID = (ConnectionStringPointer)PageState["DBID"];
            }

            if (licenseID != null && userID != null)
            {
                EPSBroker.AuditLogUserInsert(
                    ePrescribeSvc.AuditAction.USER_LOGIN,
                    licenseID,
                    userID,
                    userID,
                    _form.Request.UserIpAddress(),
                    dbID);
            }
        }

        public static void LogException(string userGUID, string exception)
        {
            Audit.AddException(
                userGUID,
                Guid.Empty.ToString(),
                exception,
                "0.0.0.0.0",
                null,
                null,
                ConnectionStringPointer.ERXDB_SERVER_1);
        }

        /// Sets the authentication cookie from the Shield authenticated user object
        /// </summary>
        /// <param name="samlToken">string representation of the Shield SAML token</param>
        public void SetAuthenticationCookieForShieldUser(ePrescribeSvc.AuthenticatedShieldUser shieldUser)
        {
            logger.Debug("SetAuthenticationCookieForShieldUser");
            ConnectionStringPointer dbID = (ConnectionStringPointer)shieldUser.DBID;
            PageState["DBID"] = dbID;
            _rxPrincipal = new RxPrincipal(shieldUser.UserGUID, string.Empty, 1, dbID);
            SetAuthenticationCookie(shieldUser.ShieldLoginID);

            System.Security.Principal.IIdentity id = (System.Security.Principal.IIdentity)_rxPrincipal.Identity;

            HttpContext.Current.User = new System.Security.Principal.GenericPrincipal(id, ((RxIdentity)_rxPrincipal.Identity).User.Roles);
        }

        public void SetAuthenticationCookie(string userName)
        {
            FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(
                            1, //  version
                            userName,           // user name
                            DateTime.Now,               //  creation
                            DateTime.Now.AddMinutes(60),// Expiration
                            false,                      // Persistent
                            string.Empty);

            // Now encrypt the ticket.
            string encryptedTicket = FormsAuthentication.Encrypt(authTicket);
            // Create a cookie and add the encrypted ticket to the
            // cookie as data.
            HttpCookie authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);

            // Add the cookie to the outgoing cookies collection.
            _form.Response.Cookies.Add(authCookie);
        }

        private bool checkCookie(ePrescribeSvc.AuthenticateAndAuthorizeUserResponse authResponse)
        {
            //handle if the site selection cookie is saved
            bool isCookieValid = false;
            HttpCookie eRxNowSiteCookie;

            string licenseID = string.Empty;
            int siteID = 1;

            if (_form.Request.Cookies["eRxNowSiteCookie"] != null)
            {
                eRxNowSiteCookie = _form.Request.Cookies.Get("eRxNowSiteCookie");

                licenseID = eRxNowSiteCookie["License"];
                siteID = Convert.ToInt32(eRxNowSiteCookie["SaveSite"]);

                ePrescribeSvc.License license = EPSBroker.GetLicenseByID(licenseID);

                var matches = authResponse.AuthenticatedShieldUsers.SingleOrDefault(u => u.AccountID == license.AccountID);

                if (matches != null)
                {
                    DataSet dsSite = ApplicationLicense.SiteLoad(licenseID, Convert.ToInt32(eRxNowSiteCookie["SaveSite"]), (ConnectionStringPointer)matches.DBID);

                    if (matches != null && dsSite != null && dsSite.Tables.Count > 0 && dsSite.Tables[0].Rows.Count > 0)
                    {
                        //cookie matches with login
                        isCookieValid = true;
                    }
                }
            }

            return isCookieValid;
        }

        /// <summary>
        /// Determines if the practice is single or multi-site; sets some session variables; determines redirect URL for downstream logic
        /// </summary>
        /// <param name="authResponse"></param>
        /// <param name="redirect_url"></param>
        /// <param name="msg"></param>
        public void SetAccountInfo(ePrescribeSvc.AuthenticateAndAuthorizeUserResponse authResponse, ref string redirect_url, ref string msg)
        {
            logger.Debug("SetAccountInfo");
            PageState["AuthenticatedShieldUsers"] = authResponse.AuthenticatedShieldUsers;
            PageState["AuthenticatedShieldUsersCount"] = authResponse.AuthenticatedShieldUsers.Length;

            string shieldLoginID = string.Empty;
            string redirectUrl = string.Empty;
            string accountID = string.Empty;
            string userGUID = string.Empty;
            string licenseID = string.Empty;
            int siteID = 1;
            bool isCookieValid = false;
            bool isSingleSiteUser = false;
            bool isUserLogInfoSet = false;

            ConnectionStringPointer dbID = ConnectionStringPointer.ERXDB_DEFAULT;

            if (authResponse.AuthenticatedShieldUsers.Length == 1)
            {
                //
                // user has single practice
                //
                userGUID = authResponse.AuthenticatedShieldUsers[0].UserGUID;
                accountID = authResponse.AuthenticatedShieldUsers[0].AccountID;
                dbID = (ConnectionStringPointer)authResponse.AuthenticatedShieldUsers[0].DBID;

                ePrescribeSvc.License license = EPSBroker.GetLicenseByAccountID(accountID, dbID);
                ePrescribeSvc.LicenseCreationDateResponse licCreationDateResponse = EPSBroker.GetLicenseCreationDate(license.LicenseID);
                                
                PageState[Allscripts.ePrescribe.Common.Constants.SessionVariables.LicenseCreationDate] = (licCreationDateResponse?.LicenseCreationDate != null) ?
                                                        licCreationDateResponse.LicenseCreationDate : DateTime.Now;

                if (license.LicenseSites.Length == 1)// user has single site
                {
                    //Create RxPrincipal and set session variables
                    isSingleSiteUser = true;

                    PageState["MULTIPLESITES"] = false;
                    PageState["SITEID"] = 1;
                    PageState["DBID"] = dbID;

                    isUserLogInfoSet = SetUserSessionInfo(authResponse.AuthenticatedShieldUsers[0], ref redirect_url, ref msg);
                    if (isUserLogInfoSet)
                    {
                        //check if user has valid state license for site
                        DataSet dsSite = ApplicationLicense.SiteLoad(PageState["LicenseID"].ToString(), 1, dbID);
                        PageState["PRACTICESTATE"] = dsSite.Tables[0].Rows[0]["State"];
                        PageState["ISCSREGISTRYCHECKREQ"] = dsSite.Tables[0].Rows[0]["IsCSRegistryCheckRequired"];
                        PageState["SITENAME"] = dsSite.Tables[0].Rows[0]["SiteName"];
                        PageState["STATEREGISTRYURL"] = dsSite.Tables[0].Rows[0]["StateRegistryURL"];
                        // 
                        // if the state of the selected site has any EPCS authrozied schedules, save them for later reference
                        //
                        if (dsSite.Tables[1] != null & dsSite.Tables[1].Rows.Count > 0)
                        {
                            List<string> siteEPCSAuthorizedSchedules = new List<string>();

                            foreach (DataRow drSchedule in dsSite.Tables[1].Rows)
                            {
                                siteEPCSAuthorizedSchedules.Add(drSchedule["Schedule"].ToString());
                            }

                            logger.Debug("Single Shield User: Setting siteEPCSAuthorizedSchedules to {0}", siteEPCSAuthorizedSchedules.ToLogString());

                            PageState["SiteEPCSAuthorizedSchedules"] = siteEPCSAuthorizedSchedules;
                        }

                        verifyStateLicense(ref redirect_url);                        
                    }
                    else
                    {
                        PageState.Abandon();
                        FormsAuthentication.SignOut();
                    }

                }
                else //user has multiple sites
                {
                    isCookieValid = checkCookie(authResponse);
                }
            }
            else if (authResponse.AuthenticatedShieldUsers.Length > 1)
            {
                //
                // user has valid shield login with multiple practices
                //

                isCookieValid = checkCookie(authResponse);
            }

            if (isCookieValid)
            {
                PageState["MULTIPLESITES"] = true;
                HttpCookie eRxNowSiteCookie;
                eRxNowSiteCookie = _form.Request.Cookies.Get("eRxNowSiteCookie");

                licenseID = eRxNowSiteCookie["License"];
                siteID = Convert.ToInt32(eRxNowSiteCookie["SaveSite"]);
                PageState["SITEID"] = siteID;

                ePrescribeSvc.License license = EPSBroker.GetLicenseByID(licenseID);

                var authenticatedShieldUser = authResponse.AuthenticatedShieldUsers.Single(u => u.AccountID == license.AccountID);
                dbID = (ConnectionStringPointer)authenticatedShieldUser.DBID;
                userGUID = authenticatedShieldUser.UserGUID;
                isUserLogInfoSet = SetUserSessionInfo(authResponse.AuthenticatedShieldUsers[0], ref redirect_url, ref msg);

                if (isUserLogInfoSet)
                {
                    DataSet dsSite = ApplicationLicense.SiteLoad(licenseID, Convert.ToInt32(eRxNowSiteCookie["SaveSite"]), dbID);

                    PageState["DBID"] = (ConnectionStringPointer)dbID;

                    //set additional session variable from Cookie
                    PageState["PRACTICESTATE"] = dsSite.Tables[0].Rows[0]["State"];
                    PageState["ISCSREGISTRYCHECKREQ"] = dsSite.Tables[0].Rows[0]["IsCSRegistryCheckRequired"];
                    PageState["SITENAME"] = dsSite.Tables[0].Rows[0]["SiteName"];
                    PageState["SITE_PHARMACY_ID"] = dsSite.Tables[0].Rows[0]["PharmacyID"];
                    PageState["STATEREGISTRYURL"] = dsSite.Tables[0].Rows[0]["StateRegistryURL"];

                    // 
                    // if the state of the selected site has any EPCS authrozied schedules, save them for later reference
                    //
                    if (dsSite.Tables[1] != null & dsSite.Tables[1].Rows.Count > 0)
                    {
                        List<string> siteEPCSAuthorizedSchedules = new List<string>();

                        foreach (DataRow drSchedule in dsSite.Tables[1].Rows)
                        {
                            siteEPCSAuthorizedSchedules.Add(drSchedule["Schedule"].ToString());
                        }
                        logger.Debug("Multiple Shield Users: Setting siteEPCSAuthorizedSchedules to {0}", siteEPCSAuthorizedSchedules.ToLogString());

                        PageState["SiteEPCSAuthorizedSchedules"] = siteEPCSAuthorizedSchedules;
                    }

                    //but now check if this user has a license for this state
                    verifyStateLicense(ref redirect_url);
                }
                else
                {
                    PageState.Abandon();
                    FormsAuthentication.SignOut();
                }
            }
            else
            {
                if (!isSingleSiteUser)
                {
                    HttpCookie eRxNowSiteCookie;
                    eRxNowSiteCookie = _form.Request.Cookies.Get("eRxNowSiteCookie");
                    if (eRxNowSiteCookie != null) //delete invalid cookie if exists
                    {
                        _form.Request.Cookies.Remove("eRxNowSiteCookie");
                        eRxNowSiteCookie = new HttpCookie("eRxNowSiteCookie");
                        eRxNowSiteCookie.Expires = DateTime.Parse(DateTime.Today.ToShortDateString()).AddDays(-1);
                        _form.Response.Cookies.Add(eRxNowSiteCookie);
                    }
                    PageState["MULTIPLESITES"] = true;

                    ApplicationLicense applicationLicense = null;
                    ePrescribeSvc.License license = new ePrescribeSvc.License();

                    if (authResponse.AuthenticatedShieldUsers != null && authResponse.AuthenticatedShieldUsers.Length > 0)
                    {
                        license = EPSBroker.GetLicenseByAccountID(authResponse.AuthenticatedShieldUsers[0].AccountID, (ConnectionStringPointer)authResponse.AuthenticatedShieldUsers[0].DBID);
                        applicationLicense = new ApplicationLicense(license.LicenseID, 1, string.Empty, (ConnectionStringPointer)authResponse.AuthenticatedShieldUsers[0].DBID);
                    }

                    PageState["SessionLicense"] = applicationLicense;

                    if (applicationLicense != null && applicationLicense.Status != Constants.LicenseStatus.Online)
                    {
                        msg = applicationLicense.StatusMessage;
                        PageState.Abandon();
                        FormsAuthentication.SignOut();
                    }

                    redirect_url = "~/" + Constants.PageNames.SELECT_ACCOUNT_AND_SITE + "?TargetUrl=" + redirect_url.Replace("~/", "");
                }
            }
        }

        public bool SetShieldSSOAccountInfo(ePrescribeSvc.AuthenticateAndAuthorizeUserResponse authResponse, string siteId, ref string redirect_url, ref string msg)
        {
            logger.Debug("SetShieldSSOAccountInfo");

            bool isUserLogInfoSet = false;
            PageState["AuthenticatedShieldUsers"] = authResponse.AuthenticatedShieldUsers;
            PageState["AuthenticatedShieldUsersCount"] = authResponse.AuthenticatedShieldUsers.Length;

            ePrescribeSvc.License license = EPSBroker.GetLicenseByAccountID(
                authResponse.AuthenticatedShieldUsers[0].AccountID,
                (ConnectionStringPointer)authResponse.AuthenticatedShieldUsers[0].DBID);

            ConnectionStringPointer dbID = (ConnectionStringPointer)authResponse.AuthenticatedShieldUsers[0].DBID;
            PageState["DBID"] = dbID;

            HttpCookie eRxNowSiteCookie;
            eRxNowSiteCookie = _form.Request.Cookies.Get("eRxNowSiteCookie");
            bool isCookieValid = checkCookie(authResponse);

            if (!isCookieValid)
            {
                if (eRxNowSiteCookie != null)
                {
                    _form.Request.Cookies.Remove("eRxNowSiteCookie");
                    eRxNowSiteCookie = new HttpCookie("eRxNowSiteCookie");
                    eRxNowSiteCookie.Expires = DateTime.Parse(DateTime.Today.ToShortDateString()).AddDays(-1);
                    _form.Response.Cookies.Add(eRxNowSiteCookie);
                    PageState["SITEID"] = Convert.ToInt32(eRxNowSiteCookie["SaveSite"]);
                }
                else
                    PageState["SITEID"] = siteId;
            }
            else
            {
                PageState["SITEID"] = Convert.ToInt32(eRxNowSiteCookie["SaveSite"]);
            }

            //set session variables
            isUserLogInfoSet = this.SetUserSessionInfo(authResponse.AuthenticatedShieldUsers[0], ref redirect_url, ref msg);
            logger.Debug("IsUserLoginInfoSet = {0}", isUserLogInfoSet);
            if (isUserLogInfoSet)
            {
                if (license.LicenseSites.Length == 1)
                {
                    logger.Debug("Only 1 site");
                    PageState["MULTIPLESITES"] = false;
                    int defaultSiteID = 1;
                    PageState["SITEID"] = defaultSiteID;

                    AsyncInitCommonCompInfoProviderAndSite(PageState["USERID"].ToString(), defaultSiteID, dbID);
                    getSiteAndSetSessionVariables();
                    verifyStateLicense(ref redirect_url);
                }
                else
                {
                    logger.Debug("Multiple Sites Found.  Current SITEID = {0}", PageState["SITEID"]);
                    PageState["MULTIPLESITES"] = true;

                    if (PageState["SITEID"] == null || string.IsNullOrWhiteSpace(PageState["SITEID"].ToString()))
                    {
                        redirect_url = "~/" + Constants.PageNames.SELECT_ACCOUNT_AND_SITE + "?TargetUrl=" + redirect_url.Replace("~/", "");

                        logger.Debug("SetShieldSSOAccountInfo(); redirect set to: {0}", redirect_url);
                    }
                    else
                    {
                        int siteid = int.Parse(PageState["SITEID"].ToString());
                        if ((siteid < 1) || (siteid > license.LicenseSites.Length))
                        {
                            // sso with invalid site id
                            PageState["SITEID"] = 0;
                            redirect_url = "~/" + Constants.PageNames.SELECT_ACCOUNT_AND_SITE + "?TargetUrl=" + redirect_url.Replace("~/", "");
                        }
                        else
                        {
                            getSiteAndSetSessionVariables();
                            verifyStateLicense(ref redirect_url);
                        }
                    }
                }
            }
            return isUserLogInfoSet;
        }

        private void getSiteAndSetSessionVariables()
        {
            logger.Debug("getSiteAndSetSessionVariables");
            //check if user has valid state license for site
            DataSet dsSite = ApplicationLicense.SiteLoad(PageState["LicenseID"].ToString(), Convert.ToInt16(PageState["SITEID"].ToString()), (ConnectionStringPointer)PageState["DBID"]);
            if (dsSite.Tables[0] != null && dsSite.Tables[0].Rows.Count > 0)
            {
                PageState["PRACTICESTATE"] = dsSite.Tables[0].Rows[0]["State"];
                PageState["ISCSREGISTRYCHECKREQ"] = dsSite.Tables[0].Rows[0]["IsCSRegistryCheckRequired"];
                PageState["STATEREGISTRYURL"] = dsSite.Tables[0].Rows[0]["StateRegistryURL"];
                PageState["SITENAME"] = dsSite.Tables[0].Rows[0]["SiteName"];
                PageState["ALLOWPATIENTRECEIPT"] = SitePreference.GetPreference(PageState.GetStringOrEmpty("LICENSEID"), int.Parse(PageState["SITEID"].ToString()), "ALLOWPATIENTRECEIPT", (ConnectionStringPointer)PageState["DBID"]);
                PageState["ALLOWMDD"] = SitePreference.GetPreference(PageState.GetStringOrEmpty("LICENSEID"), int.Parse(PageState.GetString("SITEID", "1")), "ALLOWMDD", (ConnectionStringPointer)PageState["DBID"]);
                PageState["GenericEquivalentSearch"] = SitePreference.GetPreference(PageState.GetStringOrEmpty("LICENSEID"), int.Parse(PageState.GetString("SITEID", "1")), "GenericEquivalentSearch", (ConnectionStringPointer)PageState["DBID"]);

                string printingOption = "1";
                //check to see if the state of the site is eligible for 4up.
                if (SystemConfig.GetStatePrintFormats(PageState.GetStringOrEmpty("PRACTICESTATE")) > 1)
                {
                    string printingPreference = SitePreference.GetPreference(PageState.GetStringOrEmpty("LICENSEID"), int.Parse(PageState.GetStringOrEmpty("SITEID")), "PRINTINGPREFERENCE", (ConnectionStringPointer)PageState["DBID"]);
                    if (printingPreference != null)
                    {
                        printingOption = printingPreference;
                    }
                }
                PageState.Remove("PRINTINGPREFERENCE");
                PageState["PRINTINGPREFERENCE"] = printingOption;

                ApplicationLicense license = new ApplicationLicense(((RxIdentity)_rxPrincipal.Identity).LicenseID, 1, ((RxIdentity)_rxPrincipal.Identity).CurrentLicense.EnterpriseClientID, (ConnectionStringPointer)PageState["DBID"]);
                EnterpriseClient ec = license.EnterpriseClient;
                if (ec != null)
                {
                    if (ec.ShowSponsoredLinks)
                    {
                        PageState["DisplayInfoScripts"] = SitePreference.GetPreference(PageState["LICENSEID"].ToString(), int.Parse(PageState["SITEID"].ToString()), "DISPLAYINFOSCRIPTS", (ConnectionStringPointer)PageState["DBID"]);
                    }
                    if (ec.ShowECoupon)
                    {
                        PageState["APPLYFINANCIALOFFERS"] = SitePreference.GetPreference(PageState.GetStringOrEmpty("LICENSEID"), int.Parse(PageState.GetStringOrEmpty("SITEID")), "APPLYFINANCIALOFFERS", (ConnectionStringPointer)PageState["DBID"]);
                    }
                }

                logger.Debug("getSiteAndSetSessionVariables, SiteTable: {0}", dsSite.Tables[0].ToLogString());
            }
            else
            {
                var error = String.Format("Site ID: {0}  not found for partner license: {1}", PageState["SITEID"], PageState["LicenseID"]);

                logger.Error("getSiteAndSetSessionVariables error {0}", error);
                throw new Exception(error);
            }


            // 
            // if the state of the selected site has any EPCS authrozied schedules, save them for later reference
            //
            if (dsSite.Tables[1] != null & dsSite.Tables[1].Rows.Count > 0)
            {
                logger.Debug("getSiteAndSetSessionVariables, SiteSchedulesTable: {0}", dsSite.Tables[1].ToLogString());
                List<string> siteEPCSAuthorizedSchedules = new List<string>();

                foreach (DataRow drSchedule in dsSite.Tables[1].Rows)
                {
                    siteEPCSAuthorizedSchedules.Add(drSchedule["Schedule"].ToString());
                }

                logger.Debug("Setting SiteEPCSAuthorizedSchedules in session to {0}", siteEPCSAuthorizedSchedules.ToLogString());
                PageState["SiteEPCSAuthorizedSchedules"] = siteEPCSAuthorizedSchedules;
            }
        }

        private void verifyStateLicense(ref string redirect_url)
        {
            int userType = Convert.ToInt32(PageState[Constants.SessionVariables.UserType]);
            if (isProviderOrPA(userType))
            {
                bool allSet = false;
                allSet = RxUser.VerifyUserStateLicense(PageState["USERID"].ToString(), PageState["PRACTICESTATE"].ToString(), (ConnectionStringPointer)PageState["DBID"]);

                if (!allSet)
                {
                    if (!redirect_url.ToLower().Contains(Constants.PageNames.EDIT_USER.ToLower()))
                    {
                        redirect_url = "~/" + Constants.PageNames.EDIT_USER + "?Status=NoLic&TargetUrl=" + redirect_url.Replace("~/", "");
                    }
                }
            }
        }

        private bool isProviderOrPA(int userType)
        {
            if((userType == Convert.ToInt32(Constants.UserCategory.PROVIDER)) || userType == Convert.ToInt32(Constants.UserCategory.PHYSICIAN_ASSISTANT) || userType == Convert.ToInt32(Constants.UserCategory.PHYSICIAN_ASSISTANT_SUPERVISED))
            {
                return true;
            }
            return false;
        }

        public void UpdateUserLastLoginDate(ePrescribeSvc.AuthenticateAndAuthorizeUserResponse authResponse)
        {
            var loginDate = DateTime.UtcNow;
            foreach (ePrescribeSvc.AuthenticatedShieldUser authUser in authResponse.AuthenticatedShieldUsers)
            {
                var userGUID = authUser.UserGUID;
                var dbID = (ConnectionStringPointer)authUser.DBID;

                RxUser.UpdateUserLastLoginDate(userGUID, dbID, loginDate);
            }
        }

        public List<ServiceAlert> ShowServiceAlerts(string enterpriseClient, string dtCurrentTimeZone)
        {
            return RxUser.ShowService_AlertForUser(enterpriseClient, dtCurrentTimeZone, ConnectionStringPointer.SHARED_DB);
        }

        public List<ServiceAlert> ShowServiceAlertsPricing(int pricingStructure, string dtCurrentTimeZone)
        {
            return RxUser.ShowServicePricing_AlertForUser(pricingStructure, dtCurrentTimeZone, ConnectionStringPointer.SHARED_DB);
        }

        public List<ServiceAlert> ShowServiceAlertsLicense(string enterpriseClient, string dtCurrentTimeZone, string accountID)
        {
            var serviceAlerts = new List<ServiceAlert>();
            try
            {
                ConnectionStringPointer dbID = new ConnectionStringPointer();
                if (PageState["DBID"] != null)
                {
                    dbID = (ConnectionStringPointer)PageState["DBID"];
                }

                ePrescribeSvc.License license = EPSBroker.GetLicenseByAccountID(accountID, dbID);

                List<string> lstLicenseSiteStates = new List<string>();

                foreach (ePrescribeSvc.LicenseSite ls in license.LicenseSites)
                {
                    if (!lstLicenseSiteStates.Contains(ls.State))
                    {
                        lstLicenseSiteStates.Add(ls.State);
                    }
                }

                return RxUser.ShowServiceLicenseState_AlertForUser(enterpriseClient, dtCurrentTimeZone, lstLicenseSiteStates, ConnectionStringPointer.SHARED_DB);
            }
            catch
            {
                return serviceAlerts;
            }
        }

        public static void GetAndSetDimensions(string heightVal, string widthVal, IStateContainer pageState)
        {
            using (var timer = logger.StartTimer("GetAndSetDimensions"))
            {
                timer.Message = $"<heightVal>{heightVal}</heightVal><widthVal>{widthVal}</widthVal>";

                int height;
                Int32.TryParse(heightVal, out height);

                int width;
                if (!Int32.TryParse(widthVal, out width)) width = 900;

                //if the height is less than 250, we have an issue (possibly with the particular browser of the user)
                // changed from 500 to 505 b/c login page height has modified to accommodate a blank space b/t Ad placement and grey header. 
                if (height < 505)
                    height = 1000;

                pageState["PAGEHEIGHT"] = height;
                pageState["PAGEWIDTH"] = width;
            }
        }

        private bool getUserIdProofingStatus()
        {
            return (((RxIdentity)_rxPrincipal.Identity).User.IDProofingStatusID
               == Convert.ToInt16(Constants.IDProofingStatusID.IDPROOFING_REQUIRED)
               && !((RxIdentity)_rxPrincipal.Identity).User.IsIDProofingSuccessful);
        }
    }

    public class FormContainer : IPageState
    {
        public FormContainer(Page page)
        {
            this.Request = page.Request;
            this.Response = page.Response;
        }

        public HttpSessionState Session { get; set; }
        public HttpRequest Request { get; set; }
        public HttpResponse Response { get; set; }
    }
}