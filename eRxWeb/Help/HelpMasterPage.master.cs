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
using System.Collections.Generic;
using System.Collections.Specialized;
using Allscripts.Impact.Ilearn;
using eRxWeb.AppCode;

namespace eRxWeb
{
    public partial class HelpMasterPage : BaseMasterPage
    {
        public enum HelpPage
        {
            UNKNOWN = -1,
            HOME = 0,
            GETTING_STARTED = 1,
            FAQ = 2,
            TUTORIALS = 3,
            FORUMS = 4,
            TRAINING = 5,
            ADD_ONS = 6,
            IMPORTS = 7,
            CONTACT = 8,
            WEBINAR = 9,
            COMMUNITY = 10,
            INTERFACES = 11,
            FAQ_IMPORT = 12,
            INTERFACE_REQUEST = 13,
            INTERFACE_CONFIRM = 14,
            ADD_ON_REQUEST = 15,
            MOBILE = 16,
            EULA = 17,
            TOOL_TIPS = 18,  /*15.1.3 Changes- New Tool Tips page added- Rags -12/15/10 */
            EPA = 19,
            INTERFACE_PRICE_LIST = 20
        }

        public HelpPage CurrentPage
        {
            get
            {
                if (Session["CurrentHelpPage"] == null)
                {
                    return HelpPage.UNKNOWN;
                }

                int page = -1;
                int.TryParse(Session["CurrentHelpPage"].ToString(), out page);

                return (HelpPage)page;
            }
            set
            {
                Session["CurrentHelpPage"] = value;
                string pageTitle = string.Empty;

                switch (value)
                {
                    case HelpPage.ADD_ONS:
                        pageTitle = "Add-On Features";
                        break;
                    case HelpPage.FAQ:
                        pageTitle = "FAQ's";
                        break;
                    case HelpPage.FAQ_IMPORT:
                        pageTitle = "Importing Patients";
                        break;
                    case HelpPage.FORUMS:
                        pageTitle = "Forum";
                        break;
                    case HelpPage.GETTING_STARTED:
                        pageTitle = "Getting Started";
                        break;
                    case HelpPage.TOOL_TIPS:
                        pageTitle = "Tool Tips";
                        break;
                    case HelpPage.HOME:
                        pageTitle = "Welcome to Help";
                        break;
                    case HelpPage.INTERFACES:
                        pageTitle = "Interface Selection";
                        break;
                    case HelpPage.INTERFACE_PRICE_LIST:
                        pageTitle = "Interface Pricing";
                        break;
                    case HelpPage.TRAINING:
                        pageTitle = "Training";
                        break;
                    case HelpPage.TUTORIALS:
                        pageTitle = "Tutorials";
                        break;
                    case HelpPage.CONTACT:
                        pageTitle = "Contact";
                        break;
                    case HelpPage.WEBINAR:
                        pageTitle = Request.QueryString["Day"] + " Webinar";
                        break;
                    case HelpPage.COMMUNITY:
                        pageTitle = "Community";
                        break;
                    case HelpPage.IMPORTS:
                        pageTitle = "Importing Patients";
                        break;
                    case HelpPage.INTERFACE_REQUEST:
                        pageTitle = "Interface Request";
                        break;
                    case HelpPage.INTERFACE_CONFIRM:
                        pageTitle = "Interface Confirmation";
                        break;
                    case HelpPage.ADD_ON_REQUEST:
                        pageTitle = "Hardware Ordering Request";
                        break;
                    case HelpPage.MOBILE:
                        pageTitle = "ePrescribe Mobile";
                        break;
                    case HelpPage.EULA:
                        pageTitle = "EULA";
                        break;
                    case HelpPage.EPA:
                        pageTitle = "Electronic Prior Authorization Help";
                        break;
                    default:
                        pageTitle = "Untitled Page";
                        break;
                }

                switch (value)
                {
                    case HelpPage.CONTACT:
                        if (SessionLicenseID != null)
                        {
                            if (SessionLicense.GetFeatureStatus(Constants.DeluxeFeatureType.CallSupport) == Constants.DeluxeFeatureStatus.On)
                            {
                                string helpPhone = "Call " + base.SessionLicense.EnterpriseClient.HelpPhoneNumber + " for Customer Service.";
                                callInfo.InnerText = helpPhone;
                                call.Visible = true;
                            }
                            else if (SessionLicense.GetFeatureStatus(Constants.DeluxeFeatureType.CallSupport) == Constants.DeluxeFeatureStatus.Disabled)
                            {
                                string helpPhone = "We're sorry it appears ePrescribe Deluxe subscription has been disabled. Please update your billing information or call " + base.SessionLicense.EnterpriseClient.HelpPhoneNumber + " for Customer Service.";
                                callInfo.InnerText = helpPhone;
                                call.Visible = true;
                            }
                        }
                        else if (Request.QueryString["eid"] != null)
                        {
                            EnterpriseClient ec = new EnterpriseClient(Request.QueryString["eid"]);
                            if (ec.GetFeatureStatus(Constants.DeluxeFeatureType.CallSupport) == Constants.DeluxeFeatureStatus.On)
                            {
                                string helpPhone = "Call " + ec.HelpPhoneNumber + " for Customer Service.";
                                callInfo.InnerText = helpPhone;
                                call.Visible = true;
                            }
                        }
                        quickAnswer.Visible = true;
                        version.Visible = true;
                        break;
                    case HelpPage.FAQ:
                        questions.Visible = true;
                        version.Visible = true;
                        break;
                    default:
                        quickAnswer.Visible = true;
                        version.Visible = true;
                        break;
                }

                userLogin.LogOutLandingPage = null;
                userLogin.LogInLandingPage = null;

                if (Session["PageTitle"] != null)
                {
                    Page.Title = Session["PageTitle"].ToString().Replace("-", "").Trim() + " - " + pageTitle;
                }
                else if (ConfigurationManager.AppSettings["Appname"] != null)
                {
                    Page.Title = ConfigurationManager.AppSettings["Appname"].ToString().Replace("-", "").Trim() + " - " + pageTitle;
                }
                else
                {
                    Page.Title = "Veradigm ePrescribe - " + pageTitle;
                }


                contentHeader.InnerText = pageTitle;
            }
        }

        public string LogOutLandingPage
        {
            get { return userLogin.LogOutLandingPage; }
            set
            {
                userLogin.LogOutLandingPage = value;
            }
        }

        public string LogInLandingPage
        {
            get { return userLogin.LogInLandingPage; }
            set
            {
                userLogin.LogInLandingPage = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {


                lblVersion.Text = base.SessionAppVersion.Substring(0, base.SessionAppVersion.LastIndexOf(".")).ToHTMLEncode();

                ucMessage.Visible = false;
                if (Request.QueryString["M"] != null)
                {
                    int messageType = 0;
                    if (!int.TryParse(Request.QueryString["M"], out messageType))
                    {
                        messageType = 0;
                    }

                    switch (messageType)
                    {
                        case 1:
                            ucMessage.Visible = true;
                            ucMessage.MessageText = "Thank you. Your message has been sent";
                            ucMessage.Icon = Controls_Message.MessageType.SUCCESS;
                            break;
                        case 2:
                            ucMessage.Visible = true;
                            ucMessage.MessageText = "Sorry for inconvenience. Your message has NOT sent due to connectivity issues. Please resend it again";
                            ucMessage.Icon = Controls_Message.MessageType.ERROR;
                            break;
                    }
                }

                if (Session["Theme"] != null)
                {
                    lnkEnterpriseClientStyle.Href = "~/" + Session["Theme"].ToString();
                    if (Session["ShortcutIcon"] != null && Session["ShortcutIcon"].ToString() != "")
                    {
                        PageIcon.Href = Session["ShortcutIcon"].ToString();
                    }
                }
                else if (Request.QueryString["eid"] != null)
                {
                    EnterpriseClient ec = new EnterpriseClient(Request.QueryString["eid"].ToString());
                    Session["Theme"] = ec.StyleSheet;
                    Session["ShortcutIcon"] = ec.ShortCutIcon;
                    if (Session["Theme"] != null)
                    {
                        lnkEnterpriseClientStyle.Href = "~/" + Session["Theme"].ToString();
                        if (Session["ShortcutIcon"] != null && Session["ShortcutIcon"].ToString() != "")
                        {
                            PageIcon.Href = Session["ShortcutIcon"].ToString();
                        }
                    }
                }
            }

            //Show phone contact for deluxe and platinum users
            if (SessionLicense.DeluxeFeatureStatusDisplay.Contains("Platinum") ||
               (SessionLicense.LicenseDeluxeStatus == Constants.DeluxeFeatureStatus.On ||
                 SessionLicense.LicenseDeluxeStatus == Constants.DeluxeFeatureStatus.AlwaysOn ||
                 SessionLicense.LicenseDeluxeStatus == Constants.DeluxeFeatureStatus.Cancelled))
            {
                contactPhone.Visible = true;
            }

            // Get AD placement from TIE service.
            PlacementResponse = TieUtility.GetHelpPageAdPlacement(HttpContext.Current.Request.Url.AbsolutePath, Request.Cookies, new TieUtility());
            if (PlacementResponse != null)
            {
                if (string.IsNullOrEmpty(SessionUserID))// Display log Rx Ads if the user has not logged in.
                {
                    divRightHandAd.Style.Add("display", "block");
                    divBottomAd.Style.Add("display", "block");
                }
                else
                {
                    if (PageState.GetBooleanOrFalse(Constants.SessionVariables.ShouldShowAds))
                    {
                        divRightHandAd.Style.Add("display", "block");
                        divBottomAd.Style.Add("display", "block");
                    }
                    else
                    {
                        divRightHandAd.Style.Add("display", "none");
                        divBottomAd.Style.Add("display", "none");
                    }
                }

            }

            ancILearn.HRef = ILearnConfigurationManager.GetErxILearnPageUrl("All");
            // Release Notes should be behind the login.
            if (string.IsNullOrEmpty(SessionUserID))
            {
                version.Visible = false;

            }
            else
            {
                version.Visible = true;
                ancRelease.HRef = ILearnConfigurationManager.GetErxILearnPageUrl("Help", "What%20is%20New");
            }
        }
    }

}