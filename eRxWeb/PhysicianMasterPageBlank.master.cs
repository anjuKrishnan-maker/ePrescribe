/******************************************************************************
**Change History
*******************************************************************************
**  Date:        Author:              Description:
**-----------------------------------------------------------------------------
** 06/08/2009   Dharani Reddem       Issue #2701-Mimic the display of patientcurrentMed report as in report tab instead pop up.
** 06/11/2009   Subhasis Nayak       #2667:Hide tasks tab for general users.
*  06/17/2009   Dharani Reddem       Added PRESCRIPTION_TO_PROVIDER to navigate the correct pages.
** 09/11/2009   Subhasis Nayak       #2799:POB Prescribe Only should not have access to tasks tab.
** 05/14/2010  Sonal                 #3470 Client to see their ePrescribe Account number
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
using System.Drawing;
using Allscripts.Impact;
using System.Text;
using System.Globalization;
using Allscripts.ePrescribe.Common;
using AjaxControlToolkit;
using TieServiceClient;
using System.IO;
using Allscripts.Impact.Ilearn;
using eRxWeb.AppCode;
using CompulsoryBasicUtil = eRxWeb.AppCode.CompulsoryBasicUtil;
using ConfigKeys = Allscripts.Impact.ConfigKeys;

namespace eRxWeb
{
    public partial class PhysicianMasterPageBlank : BaseMasterPage
    {
        //new 
        //protected object sessionHelpPage = null;
        string Link = "Link";
        string Selected = "Selected";
        string UnSelected = "UnSelected";
        string Hide = "Hide";
        string Tab = "Tab";
        string tabL = "tab";

        public string ReportsLinkURL
        {
            get
            {
                if (Session["PrintCurrentMed"] != null && Session["PrintCurrentMed"].ToString().Length > 0)
                {
                    string componentName = Allscripts.ePrescribe.Common.Constants.PageNames.REVIEW_HISTORY;
                    string componentParameters = string.Empty;
                    string navigationUrl = Constants.PageNames.REDIRECT_TO_ANGULAR + "?componentName=" + componentName + "&componentParameters=" + componentParameters;
                    reportsLink.NavigateUrl = navigationUrl;
                }
                else
                {
                    reportsLink.NavigateUrl = Constants.PageNames.REPORTS;
                }

                return reportsLink.NavigateUrl;
            }
        }

        protected void Page_Init(object sender, EventArgs e)
        {
             base.Page_Init();
            lnkDefaultStyleSheet.Href += "?version=" + SessionAppVersion;
            lnkDefaultPrintStyleSheet.Href += "?version=" + SessionAppVersion;

            if (!Request.Path.Contains(Constants.PageNames.EXCEPTION))
            {
                CheckForPptPlusTokenRefresh();

                if (Session["HasAcceptedEula"] != null && !Convert.ToBoolean(Session["HasAcceptedEula"]))
                {
                    if (Request.QueryString.ToString().Contains("ChangePassword.aspx"))
                    {
                        Response.Redirect("~/" + Constants.PageNames.USER_EULA + "?TargetUrl=ChangePassword.aspx?CameFrom=Login");
                    }
                    else if ((Request.CurrentExecutionFilePath.ToLower().Contains(Constants.PageNames.EDIT_USER.ToLower())) && (Request.QueryString["Status"] != null))
                    {
                        Response.Redirect("~/" + Constants.PageNames.USER_EULA + "?TargetUrl=" + Constants.PageNames.EDIT_USER + "?Status=" + Request.QueryString["Status"]);
                    }
                    // I had to take this out because it doesn't redirect the page 
                    // correctly during SSO.
                    //Server.Transfer("~/UserEULA.aspx?TargetUrl=" + Request.CurrentExecutionFilePath);
                }
            }
        }

        public void DoNotCache()
        {
            Response.Cache.SetNoStore();
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.CacheControl = "no-cache";
            Response.ExpiresAbsolute = DateTime.Now.AddDays(-1);
            Response.Expires = -1;
            Response.AddHeader("Pragma", "no-cache");
        }

        protected void Page_Load(object sender, EventArgs e)
        {


            if (Session["DBID"] == null && Session["AuthenticatedShieldUsers"] == null)
            {
                Response.Redirect(Constants.PageNames.LOGOUT);
            }

            var lastLoginDateUTC = PageState.Cast<DateTime>("LastLoginDateUTC", DateTime.MinValue);
            if (lastLoginDateUTC != DateTime.MinValue && PageState["CURRENT_SITE_ZONE"] != null && (string.Compare(GetCurrentPageName(), Constants.PageNames.SELECT_ACCOUNT_AND_SITE, true) != 0))
            {// Last login Details
                lblLastLoginLabel.Visible = true;
                lblLastLogin.Visible = true;
                DateTime other = DateTime.SpecifyKind(lastLoginDateUTC, DateTimeKind.Utc);
                lblLastLogin.Text = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(other, PageState["CURRENT_SITE_ZONE"].ToString()).ToString("MM/dd/yyyy hh:mm tt");
            }
            else
            {
                lblLastLoginLabel.Visible = false;
                lblLastLogin.Visible = false;
            }

            lnkStyleSheet.Href = PageState.GetString("Theme", "Style/AllscriptsStyle.css") + "?v=" + base.SessionAppVersion;

            if (!Request.Path.Contains(Constants.PageNames.EXCEPTION))
            {
                //By Dhiraj on 02/05/2007 to disable back button
                //Response.Cache.SetNoStore();
                //Response.Cache.SetCacheability(HttpCacheability.NoCache);
                if (Session["UserType"] != null)
                {
                    switch ((Constants.UserCategory)Session["UserType"])
                    {
                        case Constants.UserCategory.GENERAL_USER:
                        case Constants.UserCategory.POB_SUPER:
                        case Constants.UserCategory.POB_REGULAR:
                        case Constants.UserCategory.POB_LIMITED:
                            patientsLink.NavigateUrl = Constants.PageNames.SELECT_PATIENT.ToLower();
                            tasksLink.NavigateUrl = Constants.PageNames.LIST_SEND_SCRIPTS;
                            break;
                        case Constants.UserCategory.PHYSICIAN_ASSISTANT_SUPERVISED:
                            patientsLink.NavigateUrl = Constants.PageNames.SELECT_PATIENT.ToLower();
                            break;
                        default:
                            break;
                    }
                }

                // RDR - 06/08/09 - Assign the previous page if the report is being called fom 'ReviewHistory' OR Reports.
                if (Session["PrintCurrentMed"] != null && Session["PrintCurrentMed"].ToString().Length > 0)
                {
                    string componentName = Allscripts.ePrescribe.Common.Constants.PageNames.REVIEW_HISTORY;
                    string componentParameters = string.Empty;
                    string navigationUrl = Constants.PageNames.REDIRECT_TO_ANGULAR + "?componentName=" + componentName + "&componentParameters=" + componentParameters;
                    reportsLink.NavigateUrl = navigationUrl;
                }
                else
                {
                    reportsLink.NavigateUrl = Constants.PageNames.REPORTS;
                }

                if (!Page.IsPostBack)
                {
                    setLogoutInfo();

                    if (Session["PageTitle"] != null)
                    {
                        Page.Title = Session["PageTitle"].ToString() + Page.Title;
                    }
                    else
                    {
                        Page.Title = ConfigurationManager.AppSettings["Appname"].ToString() + Page.Title;
                    }

                    lnkProfile.PostBackUrl = Constants.PageNames.EDIT_USER + "?To=" + Page.AppRelativeVirtualPath;
                    lnkMessageQueue.NavigateUrl = Constants.PageNames.MESSAGE_QUEUE_TX + "?From=" + Page.AppRelativeVirtualPath;

                    lblVersion.Text = base.SessionAppVersion;

                    lnkILearn.NavigateUrl = ILearnConfigurationManager.GetErxILearnPageUrl(Page.AppRelativeVirtualPath);
                    lnkHelp.NavigateUrl = ConfigurationManager.AppSettings["HelpURL"];

                    if (base.IsLicenseShieldEnabled)
                    {
                        lblUser.ToolTip = "Shield-enabled";
                    }
                    else
                    {
                        lblUser.ToolTip = "Shield-disabled";
                    }

                    if (Session["UserName"] != null)
                    {
                        lblUser.Text = Session["UserName"].ToString();

                    }

                    if (!Convert.ToBoolean(ConfigurationManager.AppSettings["IsDeluxeStatusDisplayEnabled"]))
                    {
                        if (Session["AHSAccountID"] != null)
                        {
                            lblSiteName.ToolTip = string.Format("{0}, {1}", Session["AHSAccountID"].ToString(), Convert.ToInt16(base.DBID).ToString());
                        }
                    }

                    if (Session["SITENAME"] != null)
                    {
                        lblSiteName.Text = Session["SITENAME"].ToString();
                        if (lblSiteName.Text.Length > 30)
                        {
                            //step down the font size
                            lblSiteName.Font.Size = new FontUnit(7.5);
                            btnSites.Font.Size = new FontUnit(7.5);
                        }
                    }

                    if (Session["AHSAccountID"] != null)
                    {
                        lblAccountID.Text = "(" + Session["AHSAccountID"].ToString() + ")";
                    }

                }

                if (Session["MULTIPLESITES"] != null && Convert.ToBoolean(Session["MULTIPLESITES"]))
                {
                    btnSites.Visible = true;
                }

                SetSiteChangeButtonVisblity();

                PageIcon.Visible = true;
                if (Session["ShortcutIcon"] != null && Session["ShortcutIcon"].ToString() != "")
                {
                    PageIcon.Href = Session["ShortcutIcon"].ToString();
                }
            }


        
            if (HttpContext.Current.Request.Url.AbsolutePath != $"/{Allscripts.ePrescribe.Common.Constants.PageNames.SELECT_ACCOUNT_AND_SITE}"  
                && !(HttpContext.Current.Request.Url.AbsolutePath == $"/ForcePasswordSetup.aspx"  && !PageState.GetBooleanOrFalse("IsSsoUser"))
                && HttpContext.Current.Request.Url.AbsolutePath != $"/{Allscripts.ePrescribe.Common.Constants.PageNames.HELP_DESK}"
               )
            {
                Header.Visible = Header.Visible == true ? !IsAngularMode : Header.Visible;
                cp2.Visible = cp2.Visible == true ? !IsAngularMode : cp2.Visible;
                footer.Visible = footer.Visible == true ? !IsAngularMode : footer.Visible;
            }
            else
            {
                Header.Visible = true;
                cp2.Visible = true;
                footer.Visible = true;
            }


        }

        public void hideEntireHeader()
        {
            Page.ClientScript.RegisterStartupScript(this.GetType(), "hideheader", "<script>hideHeader();</script>");
        }

        public void hideEntireFooter()
        {
            Page.ClientScript.RegisterStartupScript(this.GetType(), "hidefooter", "<script>hideFooter();</script>");
        }

        public void hideSideBar()
        {
            Page.ClientScript.RegisterStartupScript(this.GetType(), "hidesidebar", "<script>hideSideBar();</script>");
        }

        public void hideMessageQueue()
        {
            messageIcon.Visible = false;
        }

        public void HideHelp()
        {
            helpLink.Visible = false;
        }

        public void HideILearn()
        {
            iLearnLink.Visible = false;
        }

        public void HideSiteAndUser()
        {
            lblAccountID.Visible = false;
            lblSiteName.Visible = false;
            lblUser.Visible = false;
            btnSites.Visible = false;
            lnkProfile.Visible = false;
        }

        private void SetSiteChangeButtonVisblity()
        {
            string currentPath = Request.AppRelativeCurrentExecutionFilePath.ToUpper();
            if (currentPath.StartsWith("~/") && currentPath.Length > 2)
            {
                currentPath = currentPath.Substring(2);
            }

            if ((currentPath.Trim() != Allscripts.ePrescribe.Common.Constants.PageNames.SELECT_PATIENT.ToUpper().Trim())
                && (currentPath.Trim() != Allscripts.ePrescribe.Common.Constants.PageNames.SELECT_PATIENT.ToUpper().Trim()))
            {
                btnSites.Visible = false;
            }
        }

        private void setLogoutInfo()
        {
            string logoutURL = Constants.PageNames.LOGOUT + "?";

            if (Session["PartnerLogoutUrl"] != null && Session["PartnerLogoutUrl"].ToString().Trim() != "")
                logoutURL = Session["PartnerLogoutUrl"].ToString();
            else if (Session["EnterpriseClientLogoutURL"] != null && Session["EnterpriseClientLogoutURL"].ToString().Trim() != "")
                logoutURL = Session["EnterpriseClientLogoutURL"].ToString();

            lnkLogout.Attributes["href"] = logoutURL;

            if (!base.SessionLicense.EnterpriseClient.ShowLogoutIcon)
            {
                lnkLogout.Style["display"] = "none";
            }
        }

        public void hideTabs()
        {
            renderTab("patients", Hide);
            renderTab("tasks", Hide);
            renderTab("reports", Hide);
            renderTab("settings", Hide);
            renderTab("inventory", Hide);
            renderTab(Constants.TabName.MANAGE_ACCOUNT, Hide);
            renderTab("library", Hide);
            renderTab("myerx", Hide);
            renderTab(Constants.TabName.GET_EPCS, Hide);
        }

        public void toggleTabs(string tabname)
        {
            toggleTabs(tabname, 0);
        }

        public void toggleTabs(string tabname, int taskcount)
        {
            bool bAdmin = false;
            bool bIntegration = false;
            bool bLibrary = false;
            bool bReport = true;
            bool bMyErx = false;
            bool bGetEpcs = false;
           
            bool isEnterpriseEpcsApplyToLicense = EPCSWorkflowUtils.IsEnterpriseEpcsLicense(SessionLicense,
                      (bool) Session[Constants.SessionVariables.IsEnterpriseEpcsEnabled]);
            bool isLicenseEPCSPurchased =  EPCSWorkflowUtils.IsLicenseEpcsPurchased(SessionLicense);
            if (!(isEnterpriseEpcsApplyToLicense || isLicenseEPCSPurchased))
            {
                bGetEpcs = true;
            }
            if (Convert.ToBoolean(Session["IsAdmin"]))
                bAdmin = true;

            if (base.SessionLicense.EnterpriseClient.ShowIntegrationSolutions)
                bIntegration = true;

            if (Session["SSOMode"] == null && ConfigurationManager.AppSettings["IsMyErxTabVisible"] != null && Convert.ToBoolean(ConfigurationManager.AppSettings["IsMyErxTabVisible"].ToString()))
            {
                bMyErx = true;
            }

            if (tabname == "patient")
                tabname = "patients";


            if (tabname == "admin")
                tabname = "settings";

            if (Session["SSOMode"] != null && Session["SSOMode"].ToString() == Constants.SSOMode.PATIENTLOCKDOWNMODE)
            {
                renderTab("patients", Hide);

                if ((Constants.UserCategory)Session["UserType"] == Constants.UserCategory.GENERAL_USER ||
                    (Constants.UserCategory)Session["UserType"] == Constants.UserCategory.POB_LIMITED)
                {
                    renderTab("tasks", Hide);
                }
                else
                {
                    renderTab(tabname, Selected, taskcount);
                }

                renderTab("reports", Hide);
                renderTab("myerx", Hide);
                renderTab("settings", Hide);
                renderTab("library", Hide);
                renderTab(Constants.TabName.MANAGE_ACCOUNT, Hide);
                renderTab(Constants.TabName.GET_EPCS, Hide);
            }
            else
            {
                if (Session["SSOMode"] != null && (Session["SSOMode"].ToString() == Constants.SSOMode.TASKMODE || Session["SSOMode"].ToString() == Constants.SSOMode.EPAPATIENTLOCKDOWNTASKMODE))
                {
                    renderTab("patients", Hide);
                }
                else
                {
                    renderTab("patients", UnSelected);
                }

                if ((Constants.UserCategory)Session["UserType"] == Constants.UserCategory.GENERAL_USER ||
                    (Constants.UserCategory)Session["UserType"] == Constants.UserCategory.POB_LIMITED)
                {
                    renderTab("tasks", Hide);
                }
                else
                {
                    renderTab("tasks", UnSelected, taskcount);
                }

                if ((SessionLicense.GetFeatureStatus(Constants.DeluxeFeatureType.iFC) == Constants.DeluxeFeatureStatus.On
                    || SessionLicense.GetFeatureStatus(Constants.DeluxeFeatureType.iFC) == Constants.DeluxeFeatureStatus.Disabled
                    || SessionLicense.GetFeatureStatus(Constants.DeluxeFeatureType.iFC) == Constants.DeluxeFeatureStatus.Off))
                {
                    bLibrary = true;
                }

                renderTab("reports", bReport ? UnSelected : Hide);
                renderTab("settings", bAdmin ? UnSelected : Hide);
                renderTab(Constants.TabName.MANAGE_ACCOUNT, bIntegration ? UnSelected : Hide);
                renderTab("library", bLibrary ? UnSelected : Hide);
                renderTab("myerx", bMyErx ? UnSelected : Hide);
                renderTab(Constants.TabName.GET_EPCS, bGetEpcs? UnSelected:Hide);
                if (tabname == "tasks")
                {
                    renderTab(tabname, Selected, taskcount);
                }
                else
                {
                    renderTab(tabname, Selected);
                }

                renderTab(tabname, Selected);
            }
        }

        protected void renderTab(string tabname, string format)
        {
            renderTab(tabname, format, 0);
        }

        protected void renderTab(string tabname, string format, int count)
        {
            string tabId = tabname.Replace(" ", "");
            string addon = string.Empty;
            if (count > 0)
            {
                addon = " (" + count.ToString() + ")";
            }

            HyperLink link = this.FindControl(tabId + Link) as HyperLink;

            if (link != null)
            {
                link.CssClass = tabL + format + Link;
                switch (format)
                {
                    case "Hide":
                        link.Text = string.Empty;
                        break;

                    case "Selected":
                        if (string.Equals(tabname, "myerx", StringComparison.CurrentCultureIgnoreCase))
                        {
                            link.Text = "My eRx" + addon;
                        }
                        else
                        {
                            link.Text = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(tabname) + addon;
                        }

                        break;

                    case "UnSelected":
                        if (string.Equals(tabname, "myerx", StringComparison.CurrentCultureIgnoreCase))
                        {
                            link.Text = "My eRx" + addon;
                        }
                        else
                        {
                            link.Text = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(tabname) + addon;
                        }

                        break;
                }
               
            }

            Panel panel = this.FindControl(tabId + Tab) as Panel;
            if (panel != null)
            {
                panel.CssClass = tabL + format;
            }
        }

        protected void lnkbtnMyAccount_Click(object sender, EventArgs e)
        {
            Server.Transfer(Constants.PageNames.EDIT_USER);
        }
        public int getTableHeight()
        {
            return getTableHeight(Page);
        }
        public int getTableHeight(Page page)
        {

            int tableHeight = 0;
            if (PageState["PAGEHEIGHT"] != null)
            {
                switch (page.AppRelativeVirtualPath.ToUpper())
                {
                    case "~/SELECTPATIENT":
                        tableHeight = (Convert.ToInt32(PageState["PAGEHEIGHT"]) >= 600)
                           ? (Convert.ToInt32(PageState["PAGEHEIGHT"]) - 270)
                           : Convert.ToInt32(PageState["PAGEHEIGHT"]);
                        break;
                    case "~/SCRIPTPAD.ASPX":
                        tableHeight = Convert.ToInt32(PageState["PAGEHEIGHT"]) - 340;
                        break;
                    case "~/SELECTDX.ASPX":
                        tableHeight = Convert.ToInt32(PageState["PAGEHEIGHT"]) - 220;
                        break;
                    case "~/PHARMACY.ASPX":

                        tableHeight = Convert.ToInt32(PageState["PAGEHEIGHT"]) - 220;
                        break;
                    case "~/PHARMREFILLSUMMARY.ASPX":
                        tableHeight = Convert.ToInt32(PageState["PAGEHEIGHT"]) - 225;
                        break;
                    case "~/DOCREFILLMENU.ASPX":
                        tableHeight = Convert.ToInt32(PageState["PAGEHEIGHT"]) - 250;
                        break;
                    case "~/APPROVEREFILLTASK.ASPX":
                        tableHeight = Convert.ToInt32(PageState["PAGEHEIGHT"]) - 340;
                        break;
                    case "~/MULTIPLEVIEW.ASPX":
                        tableHeight = Convert.ToInt32(PageState["PAGEHEIGHT"]) - 220;
                        break;
                    case "~/LISTSENDSCRIPTS.ASPX":
                        tableHeight = Convert.ToInt32(PageState["PAGEHEIGHT"]) - 270;
                        break;
                    case "~/TASKSCRIPTLIST.ASPX":
                        tableHeight = Convert.ToInt32(PageState["PAGEHEIGHT"]) - 195;
                        break;
                    case "~/REFILLHISTORY.ASPX":
                        tableHeight = Convert.ToInt32(PageState["PAGEHEIGHT"]) - 308;
                        break;                   
                    case "~/PATIENTALLERGY.ASPX":
                        tableHeight = Convert.ToInt32(PageState["PAGEHEIGHT"]) - 200;
                        break;
                    case "~/ADDALLERGY.ASPX":
                        tableHeight = Convert.ToInt32(PageState["PAGEHEIGHT"]) - 210;
                        break;
                    case "~/PATIENTDIAGNOSIS.ASPX":
                        tableHeight = Convert.ToInt32(PageState["PAGEHEIGHT"]) - 200;
                        break;
                    default:
                        tableHeight = Convert.ToInt32(PageState["PAGEHEIGHT"]) - 200;
                        break;

                }
            }
            else
                Server.Transfer(Constants.PageNames.LOGOUT + "?Timeout=YES");
            return tableHeight;
        }

        public int getPageSize()
        {
            int pageSize = 0;
            int tableHeight = 0;
            switch (((System.Web.UI.TemplateControl)(Page)).AppRelativeVirtualPath.ToUpper())
            {
                case "~/PATIENTMEDREPORT.ASPX":
                case "~/PATIENTSNAPSHOTREPORT.ASPX":
                    tableHeight = getTableHeight();
                    pageSize = (int)(tableHeight - 22 - 22) / 22;
                    break;
                case "~/FULLSCRIPTEDIT.ASPX":
                    tableHeight = getTableHeight();
                    pageSize = (int)(tableHeight - 22) / 22;
                    break;
                case "~/MERGEPATIENTS.ASPX":
                    tableHeight = getTableHeight();
                    pageSize = (int)(tableHeight - 22 - 22) / 32;
                    break;
                case "~/MESSAGEQUEUE.ASPX":
                    tableHeight = getTableHeight();
                    pageSize = (int)(tableHeight - 22 - 22) / 22;
                    break;
            }
            return (pageSize <= 0 ? 1 : pageSize);
        }

        protected void lbMyTasks_Click(object sender, EventArgs e)
        {
            if (Session["ISPROVIDER"] != null)
            {
                if (Convert.ToBoolean(Session["ISPROVIDER"]))
                {
                    Server.Transfer(Constants.PageNames.DOC_REFILL_MENU.ToLower());
                }
            }
            else
            {
                Server.Transfer(Constants.PageNames.LIST_SEND_SCRIPTS);
            }
        }

        protected void btnSites_Click(object sender, EventArgs e)
        {
            string from = ((WebControl)sender).Parent.Page.Request.AppRelativeCurrentExecutionFilePath;
            if (from.StartsWith("~/"))
            {
                from = from.Substring(2);
            }

            Response.Redirect(Constants.PageNames.SELECT_ACCOUNT_AND_SITE + "?TargetURL=" + from);
        }

        public string GetCurrentPageName()
        {
            //string urlPath = Request.Url.AbsolutePath;
            FileInfo fileInfo = new FileInfo(Request.Url.AbsolutePath);
            return fileInfo.Name;
        }
    }

}
