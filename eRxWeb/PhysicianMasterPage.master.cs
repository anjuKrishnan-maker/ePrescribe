using Allscripts.ePrescribe.Common;
using Allscripts.Impact;
using Allscripts.Impact.Ilearn;
using eRxWeb.AppCode;
using eRxWeb.ServerModel;
using System;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using Allscripts.Impact.Tasks;
using CompulsoryBasicUtil = eRxWeb.AppCode.CompulsoryBasicUtil;
using ConfigKeys = Allscripts.Impact.ConfigKeys;
using Patient = Allscripts.Impact.Patient;

namespace eRxWeb
{
    public partial class PhysicianMasterPage : BaseMasterPage
    {
        protected internal string editPatientRedirect
        {
            get
            {
                if (AllowPatientEdit)
                {
                    return Constants.PageNames.ADD_PATIENT + "?Mode=Edit";
                }
                return null;
            }
        }

        protected internal string editAllergyRedirect
        {
            get
            {
                if (AllowAllergyEdit)
                {
                    return Constants.PageNames.PATIENT_ALLERGY;
                }
                return null;
            }
        }

        protected internal string editProblemRedirect
        {
            get
            {
                if (AllowDiagnosisEdit)
                {
                    return Constants.PageNames.PATIENT_DIAGNOSIS;
                }
                return null;
            }
        }

        private bool allowPatientEdit = false;
        private bool allowDiagnosisEdit = false;
        private bool allowAllergyEdit = false;
        private bool allowPharmacyEdit = false;

        protected string mobrowspan = "6";
        string Link = "Link";
        string Selected = "Selected";
        string UnSelected = "UnSelected";
        string Hide = "Hide";
        string Tab = "Tab";
        string tabL = "tab";

        DataSet dsAllergies = new DataSet();
        DataSet dsDiagnosis = new DataSet();
        DataSet dsActiveMedication = new DataSet();

        protected internal string EditRetailPharmRedirect
        {
            get
            {
                if (AllowPharmacyEdit)
                {
                    string from = Page.Request.AppRelativeCurrentExecutionFilePath;
                    if (from.StartsWith("~/"))
                    {
                        from = from.Substring(2);
                    }

                    return Constants.PageNames.PHARMACY + "?Mode=Edit&From=" + from;
                }

                return null;
            }
        }

        protected string EditMoPharmRedirect
        {
            get
            {
                if (AllowPharmacyEdit)
                {
                    string from = Page.Request.AppRelativeCurrentExecutionFilePath;
                    if (from.StartsWith("~/"))
                    {
                        from = from.Substring(2);
                    }

                    return Constants.PageNames.PHARMACY + "?Mode=Edit&From=" + from + "&SetMO=true";
                }
                return null;
            }
        }

        public string TasksLinkURL
        {
            get { return tasksLink.NavigateUrl; }
        }

        public void HideLoading()
        {
            loading.Style["display"] = "none";
        }

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

        public ScriptManager MasterScriptManager
        {
            get { return masterScriptManager; }
        }

        /// <summary>
        /// Display Mail and Retail order pharmacy 
        /// </summary>
        public bool ShowPharmacy
        {
            get
            {
                return (base.SessionLicense.EnterpriseClient.ShowPharmacy && !PageState.ContainsKey(Constants.SessionVariables.TaskScriptMessageId));
            }
        }

        public ChangeRxRequestedMedCs ChangeRxRequestedMedCs
        {
            get
            {
                return PageState[Constants.SessionVariables.ChangeRxRequestedMedCs] as ChangeRxRequestedMedCs;
            }
            set
            {
                PageState[Constants.SessionVariables.ChangeRxRequestedMedCs] = value;
            }
        }
        public RxTaskModel RxTask
        {
            get
            {
                return PageState[Constants.SessionVariables.RxTask] as RxTaskModel;
            }
            set
            {
                PageState[Constants.SessionVariables.RxTask] = value;
            }
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            base.Page_Init();
            lnkDefaultStyleSheet.Href += "?version=" + SessionAppVersion;

            CheckForPptPlusTokenRefresh();

            if (!Convert.ToBoolean(Session["HasAcceptedEULA"]))
            {
                Response.Redirect(Constants.PageNames.USER_EULA + "?TargetUrl=" + Request.CurrentExecutionFilePath);
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
            imgVIP.Visible = false;
            var lastLoginDateUTC = PageState.Cast<DateTime>("LastLoginDateUTC", DateTime.MinValue);
            if (lastLoginDateUTC != DateTime.MinValue && PageState["CURRENT_SITE_ZONE"] != null)
            {// Last login Details
                lblLastLoginLabel.Visible = true;
                lblLastLogin.Visible = true;
                DateTime other = DateTime.SpecifyKind(lastLoginDateUTC, DateTimeKind.Utc);
                var date = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(other, PageState["CURRENT_SITE_ZONE"].ToString());
                lblLastLogin.Text = date.ToString("MMM dd, yyyy - hh:mm ") + date.ToString("tt").ToLower();
            }
            else
            {
                lblLastLoginLabel.Visible = false;
                lblLastLogin.Visible = false;
            }

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
                    patientsLink.NavigateUrl = Constants.PageNames.SELECT_PATIENT;
                    break;
                default:
                    break;
            }

            if (SessionLicense.GetFeatureStatus(Constants.DeluxeFeatureType.iFC) == Constants.DeluxeFeatureStatus.On)
            {
                medicationSearch.Visible = true;
            }
            else
            {
                medicationSearch.Visible = false;
            }

            reportsLink.NavigateUrl = Constants.PageNames.REPORTS;

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

                lnkStyleSheet.Href = PageState.GetString("Theme", "Style/Style.css") + "?v=" + base.SessionAppVersion;

                lblVersion.Text = base.SessionAppVersion.ToHTMLEncode();
                
                lnkILearn.NavigateUrl = ILearnConfigurationManager.GetErxILearnPageUrl(Page.AppRelativeVirtualPath); 
                lnkHelp.NavigateUrl = ConfigurationManager.AppSettings["HelpURL"];
                
                lnkProfile.PostBackUrl = Constants.PageNames.EDIT_USER + "?To=" + Page.AppRelativeVirtualPath;
                lnkMessageQueue.NavigateUrl = Constants.PageNames.MESSAGE_QUEUE_TX + "?From=" + Page.AppRelativeVirtualPath;

                if (Session["UserName"] != null)
                {
                    lblUser.Text = Session["UserName"].ToString();
                }

                string siteHoverText = string.Empty;

                if (Session["SITE_TOOLTIP_TEXT"] == null)
                {
                    Session["SITE_TOOLTIP_TEXT"] = getSiteTooltipText(base.SessionLicenseID, Convert.ToInt32(Session["siteID"]));
                }

                siteHoverText = Session["SITE_TOOLTIP_TEXT"].ToString();

                if (Session["AHSAccountID"] != null)
                {
                    if (Convert.ToBoolean(ConfigurationManager.AppSettings["IsDeluxeStatusDisplayEnabled"]))
                    {
                        if (SessionLicense.DeluxeFeatureStatusDisplay.ToLower().Contains("platinum"))
                        {
                            lblSiteName.ToolTip = string.Format("{0}.\n {1}, {2}, {3}", siteHoverText,
                                Session["AHSAccountID"].ToString(), Convert.ToInt16(base.DBID).ToString(), SessionLicense.DeluxeFeatureStatusDisplay);
                        }
                        else
                        {
                            lblSiteName.ToolTip = string.Format("{0}.\n {1}, {2}, {3}", siteHoverText,
                                Session["AHSAccountID"].ToString(), Convert.ToInt16(base.DBID).ToString(), SessionLicense.DeluxePricingStructureDisplay);
                        }
                    }
                    else
                    {
                        lblSiteName.ToolTip = string.Format("{0}.\n {1}, {2}", siteHoverText, Session["AHSAccountID"].ToString(), Convert.ToInt16(base.DBID).ToString());
                    }
                }

                if (base.IsLicenseShieldEnabled)
                {
                    lblUser.ToolTip = "Shield-enabled";
                }
                else
                {
                    lblUser.ToolTip = "Shield-disabled";
                }

                if (Session["SITENAME"] != null)
                {
                    lblSiteName.Text = Session["SITENAME"].ToString();
                    if (lblSiteName.Text.Length > 30)
                    {
                        //step down the font size
                        lblSiteName.Font.Size = new FontUnit(7.5);
                        btnSites.Font.Size = new FontUnit(8);
                    }
                }

                lblAccountID.Text = "(" + Session["AHSAccountID"].ToString() + ")";

                setClinicalViewerVisibility();
            }

            //If the page is set to do partial rendering, then make the sure the header links actually cause a full 
            //postback event, rather than an asyncronous postback. 
            if (masterScriptManager.EnablePartialRendering)
            {
                masterScriptManager.RegisterPostBackControl(lnkEditPatient);
                masterScriptManager.RegisterPostBackControl(lbEditDx);
                masterScriptManager.RegisterPostBackControl(lnkAlleryEdit);

                if (ShowPharmacy)
                {
                    masterScriptManager.RegisterPostBackControl(lnkEditPharmacy);
                    masterScriptManager.RegisterPostBackControl(lnkEditMOPharm);
                }

            }
            displayPatientHeader();

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

            // GA Implementation
            //PlacementResponse = TieUtility.GetAdForMasterPage(HttpContext.Current.Request.Url.AbsolutePath, Request.Cookies, new TieUtility());
            //if (PlacementResponse != null)
            //{
            //    divRightHandAd.Visible = PageState.GetBooleanOrFalse(Constants.SessionVariables.ShouldShowAds);
            //}

            patientPanel.Visible = patientPanel.Visible == true ? !IsAngularMode : patientPanel.Visible;
            cp2.Visible = cp2.Visible == true ? !IsAngularMode : cp2.Visible;
            footer.Visible = footer.Visible == true ? !IsAngularMode : footer.Visible;

        }

        private void setLogoutInfo()
        {
            string logoutURL = Constants.PageNames.LOGOUT;

            if (Session["PartnerLogoutUrl"] != null && Session["PartnerLogoutUrl"].ToString().Trim() != "")
                logoutURL = Session["PartnerLogoutUrl"].ToString();
            else if (Session["EnterpriseClientLogoutURL"] != null && Session["EnterpriseClientLogoutURL"].ToString().Trim() != "")
                logoutURL = Session["EnterpriseClientLogoutURL"].ToString();

            lnkLogout.Attributes["href"] = logoutURL;

            if (!base.SessionLicense.EnterpriseClient.ShowLogoutIcon)
            {
                lnkLogout.Style["display"] = "none";
                splogout.Visible = false;
            }
        }

        private void displayMailOrder(bool show)
        {
            if (ShowPharmacy)
            {
                if (Session["MOB_NABP"] == null || Session["MOB_NABP"].ToString().Trim() == "")
                {
                    show = false;
                }

                if (show)
                {
                    if (ShowPharmacy)
                    {
                        lblPrefMOP.Font.Size = new FontUnit(9);
                        imgMoreMailOrderPharm.Visible = false;
                        if (Session["MOB_Name"] != null)
                        {
                            if (Session["MOB_Name"].ToString().Length > 48)
                            {
                                lblPrefMOP.Text = Session["MOB_Name"].ToString().Substring(0, 48);
                                //lblPrefMOP.ToolTip = Session["MOB_Name"].ToString();
                                imgMoreMailOrderPharm.Visible = true;
                                MPharmInfoObjDataSource.Select();
                            }
                            else
                            {
                                lblPrefMOP.Text = Session["MOB_Name"].ToString();
                            }
                        }
                        else
                        {
                            lblPrefMOP.Text = "None entered";
                        }

                        // new EPSC image logic
                        if (base.IsMOPharmacyEPCSEnabled)
                        {
                            imgMOPRating.ImageUrl = "images/ControlSubstance_sm.gif";
                            imgMOPRating.Visible = true;
                        }
                    }
                    // end mail order pharmacy image logic
                }
                else
                {
                    if (Session["PatientID"] != null)
                    {
                        lblPrefMOP.Text = "None entered";
                        //lblPrefMOP.ToolTip = string.Empty;
                        imgMoreMailOrderPharm.Visible = false;
                        imgMOPRating.Visible = false;
                    }
                }
            }
        }

        public string lnkPatientEditClientID
        {
            get
            {
                return lnkEditPatient.ClientID;
            }
        }

        public string lnkAllergyEditClientID
        {
            get
            {
                return lnkAlleryEdit.ClientID;
            }
        }

        public string lbEditDxClientID
        {
            get
            {
                return lbEditDx.ClientID;
            }
        }

        public void displayPatientHeader()
        {
            imgVIP.Visible = false;
            if (Session["PATIENTID"] != null)
            {
                lblNoPatient.Visible = false;
                pnlPatientDetails.Visible = true;
                lblPatientFirst.Visible = true;
                lblPatientLast.Visible = true;
                lblPatientFirst.Text = StringUtil.formatPatientDisplayName(PageState.GetStringOrEmpty("PATIENTFIRSTNAME") + " " + PageState.GetStringOrEmpty("PATIENTMIDDLENAME")).ToHTMLEncode();
                lblPatientLast.Text = StringUtil.formatPatientDisplayName(PageState.GetStringOrEmpty("PATIENTLASTNAME").ToUpper() +", ").ToHTMLEncode();

                if((bool)PageState.GetBooleanOrFalse("IsVIPPatient") || PageState.GetBooleanOrFalse("IsRestrictedPatient"))
                {
                    imgVIP.Visible = true;
                    if (PageState.GetBooleanOrFalse("IsUserRestrictedForPatient"))
                    {
                        AllowPatientEdit = false;
                        PageState.Remove("IsUserRestrictedForPatient");
                    }
                }
                else
                {
                    imgVIP.Visible = false;
                }

                if (Session["PATIENTMRN"] != null)
                {
                    lblMrn.Text = " " + Session["PATIENTMRN"].ToString();
                }

                var dob = PageState.GetStringOrEmpty("PATIENTDOB");
                var dobDt = dob == string.Empty ? DateTime.MinValue : Convert.ToDateTime(dob);
                if (dobDt != DateTime.MinValue)
                {
                    lblGenderDob.Text = dobDt.ToString("MMMM dd, yyyy");
                    lblGenderDob.Text += " (" + StringUtil.CalculateAge(dob) + ") | ";
                    lblGenderDob.Text += StringUtil.SexToFullValue(PageState.GetStringOrEmpty("SEX"));
                }


                imgMoreActiveProblem.Visible = false;
                if (Session["ACTIVEDX"] != null && Session["ACTIVEDX"].ToString().Length > 0)
                {
                    if (Session["ACTIVEDX"].ToString().Length > 100)
                    {
                        lblDx.Text = Session["ACTIVEDX"].ToString().Substring(0, 100);
                        //lblDx.ToolTip = Session["ACTIVEDX"].ToString();
                        imgMoreActiveProblem.Visible = true;
                        LoadPatientDiagnosis();
                    }
                    else
                    {
                        lblDx.Text = Session["ACTIVEDX"].ToString();
                    }
                }
                else
                {
                    lblDx.Text = "None entered";
                }
                
                imgMoreActiveAllergy.Visible = false;
                if (Session["PATIENTNKA"] != null && Session["PATIENTNKA"].ToString().Equals("Y"))
                {
                    lblAllergy.Text = "No Known Allergies";
                }
                else if (Session["ALLERGY"] != null && Session["ALLERGY"].ToString().Length > 0)
                {
                    if (Session["ALLERGY"].ToString().Length > 100)
                    {
                        lblAllergy.Text = Session["ALLERGY"].ToString().Substring(0, 100);
                        //lblAllergy.ToolTip = Session["ALLERGY"].ToString();
                        imgMoreActiveAllergy.Visible = true;
                        dsAllergies = Patient.GetPatientAllergy(base.SessionPatientID, base.SessionLicenseID, base.SessionUserID, base.DBID);
                        grdViewAllergies.DataSource = dsAllergies;
                        grdViewAllergies.DataBind();
                    }
                    else
                    {
                        lblAllergy.Text = Session["ALLERGY"].ToString();
                    }

                }
                else
                {
                    lblAllergy.Text = "None entered";
                }

                imgPharmacyRating.Visible = false;
                imgMoreRetailPharm.Visible = false;

                if (ShowPharmacy)
                {
                    lblLastPharmacyName.Font.Size = new FontUnit(9);

                    if (Session["LASTPHARMACYNAME"] != null)
                    {
                        if (Session["LASTPHARMACYNAME"].ToString().Length > 48)
                        {
                            lblLastPharmacyName.Text = Session["LASTPHARMACYNAME"].ToString().Substring(0, 48);
                            //lblLastPharmacyName.ToolTip = Session["LASTPHARMACYNAME"].ToString();
                            imgMoreRetailPharm.Visible = true;
                            pharmInfoObjDataSource.Select();
                        }
                        else
                        {
                            lblLastPharmacyName.Text = Session["LASTPHARMACYNAME"].ToString();
                        }
                    }
                    else
                    {
                        lblLastPharmacyName.Text = "None entered";
                        //lblLastPharmacyName.ToolTip = string.Empty;
                    }

                    // new EPSC image logic
                    if (base.IsPharmacyEPCSEnabled && lblLastPharmacyName.Text != "None entered")
                    {
                        imgPharmacyRating.ImageUrl = "images/ControlSubstance_sm.gif";
                        imgPharmacyRating.Visible = true;
                    }
                }

                SetPatientActiveMedsControl();

                if (ShowPharmacy)
                {
                    lnkRemMOPharm.Visible = AllowPharmacyEdit && (Session["MOB_NABP"] != null);
                    lnkRemPharmacy.Visible = AllowPharmacyEdit && (Session["LASTPHARMACYNAME"] != null);
                }

                if (base.SessionLicense.EnterpriseClient.ShowClinicalViewer && !base.IsSSOUser)
                {
                    hlClinicalViewer.Enabled = true;
                }
            }
            else
            {
                lblNoPatient.Visible = true;
                pnlPatientDetails.Visible = false;
                lblMrn.Text = string.Empty;
                lblPatientFirst.Visible = false;
                lblPatientLast.Visible = false;
                lblGenderDob.Text = string.Empty;
                lblAllergy.Text = string.Empty;
                lblDx.Text = string.Empty;
                lblActiveMed.Text = string.Empty;
                imgMoreActiveAllergy.Visible = false;
                imgMoreActiveMed.Visible = false;
                imgMoreActiveProblem.Visible = false;

                if (ShowPharmacy)
                {
                    lblLastPharmacyName.Text = string.Empty;
                    imgMoreMailOrderPharm.Visible = false;
                    imgMoreRetailPharm.Visible = false;
                    lblPrefMOP.Text = string.Empty;
                    lnkRemPharmacy.Visible = false;
                    lnkRemMOPharm.Visible = false;
                    imgPharmacyRating.Visible = false;
                    imgMOPRating.Visible = false;
                }

                if (base.SessionLicense.EnterpriseClient.ShowClinicalViewer && !base.IsSSOUser)
                {
                    hlClinicalViewer.Enabled = false;
                }
            }

            displayMailOrder(true);
        }

        private void SetPatientActiveMedsControl()
        {
            if (PageState[Constants.SessionVariables.PatientId] == null) return;

            var activeMedsSession = PageState.GetStringOrEmpty("ACTIVEMEDICATIONS");

            imgMoreActiveMed.Visible = false;
            if (PageState.GetBooleanOrFalse("PATIENTNoActiveMed"))
            {
                lblActiveMed.Text = "No Active Medications";
            }
            else if (activeMedsSession.Length > 0)
            {
                if (activeMedsSession.Length > 100)
                {
                    lblActiveMed.Text = activeMedsSession.Substring(0, 100).ToHTMLEncode();
                    imgMoreActiveMed.Visible = true;
                    dsActiveMedication = Patient.GetPatientActiveMedication(base.SessionPatientID,
                        base.SessionLicenseID,
                        base.SessionUserID, base.DBID);
                    radgrdActiveMed.DataSource = dsActiveMedication;
                    radgrdActiveMed.DataBind();
                }
                else
                {
                    lblActiveMed.Text = activeMedsSession.ToHTMLEncode();
                }
            }
            else
            {
                lblActiveMed.Text = "None entered";
            }
        }

        public void RefreshActiveMeds()
        {
            UpdatePatientActiveMeds();
            SetPatientActiveMedsControl();
            patientPanel.Update();
        }

        public void LoadPatientDiagnosis()
        {
            var diagnosis = GetActivePatientDiagnosisList();
            radGrdActiveProbs.DataSource = diagnosis;
            radGrdActiveProbs.DataBind();
        }
        public void hideTabs()
        {
            renderTab("patients", Hide);
            renderTab("tasks", Hide);
            renderTab("reports", Hide);
            renderTab("settings", Hide);
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
            bool bTasks = true;
            bool bLibrary = false;
            bool bReport = true;
            bool bMyErx = false;

            if (Convert.ToBoolean(Session["IsAdmin"]))
                bAdmin = true;

            bIntegration = base.SessionLicense.EnterpriseClient.ShowIntegrationSolutions;

            if ((SessionLicense.GetFeatureStatus(Constants.DeluxeFeatureType.iFC) == Constants.DeluxeFeatureStatus.On
                || SessionLicense.GetFeatureStatus(Constants.DeluxeFeatureType.iFC) == Constants.DeluxeFeatureStatus.Disabled
                || SessionLicense.GetFeatureStatus(Constants.DeluxeFeatureType.iFC) == Constants.DeluxeFeatureStatus.Off))
            {
                bLibrary = true;
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
                renderTab("settings", Hide);
                renderTab("tools", Hide);
                renderTab("library", Hide);
                renderTab("myerx", Hide);
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
                    (Constants.UserCategory)Session["UserType"] == Constants.UserCategory.POB_LIMITED ||
                    ((Constants.UserCategory)Session["UserType"] == Constants.UserCategory.POB_LIMITED))
                {
                    renderTab("tasks", Hide);
                }
                else
                {
                    renderTab("tasks", bTasks ? UnSelected : Hide, taskcount);
                }

                

                if (Session["SSOMode"] == null && ConfigurationManager.AppSettings["IsMyErxTabVisible"] != null && Convert.ToBoolean(ConfigurationManager.AppSettings["IsMyErxTabVisible"].ToString()))
                {
                    bMyErx = true;
                }

                renderTab("reports", bReport ? UnSelected : Hide);
                renderTab("settings", bAdmin ? UnSelected : Hide);
                renderTab("tools", bIntegration ? UnSelected : Hide);
                renderTab("library", bLibrary ? UnSelected : Hide);
                renderTab("myerx", bMyErx ? UnSelected : Hide);

                if (tabname == "tasks")
                {
                    renderTab(tabname, Selected, taskcount);
                }
                else
                {
                    renderTab(tabname, Selected);
                }
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

        protected void lnkPatient_Click(object sender, EventArgs e)
        {
            SelectPatientComponentParameters selectPatientComponentParameters = new SelectPatientComponentParameters
            {
                PatientId = PageState.GetStringOrEmpty(Constants.SessionVariables.PatientId)
            };
            RedirectToSelectPatient(null, selectPatientComponentParameters);
        }

        protected void lnkDiagnosis_Click(object sender, EventArgs e)
        {
            Server.Transfer(Constants.PageNames.SELECT_DX);
        }

        protected void lnkMedication_Click(object sender, EventArgs e)
        {
            Server.Transfer(Constants.PageNames.REDIRECT_TO_ANGULAR + "?componentName=" + Constants.PageNames.SELECT_MEDICATION);
        }

        protected void lnkbtnMyAccount_Click(object sender, EventArgs e)
        {
            //moved to angular
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
            return tableHeight;
        }

        protected void lnkEditPatient_Click(object sender, EventArgs e)
        {
            Response.Redirect(Constants.PageNames.ADD_PATIENT + "?Mode=Edit");
        }
        protected void lbEditDx_Click(object sender, EventArgs e)
        {
            Response.Redirect(Constants.PageNames.PATIENT_DIAGNOSIS);
        }

        protected void lnkAlleryEdit_Click(object sender, EventArgs e)
        {
            Response.Redirect(Constants.PageNames.PATIENT_ALLERGY);
        }

        // ARN Merged Code - JP
        protected void lnkViewReferral_Click(object sender, EventArgs e)
        {
            SelectPatientComponentParameters selectPatientComponentParameters = new SelectPatientComponentParameters
            {
                PatientId = PageState.GetStringOrEmpty(Constants.SessionVariables.PatientId)
            };
            RedirectToSelectPatient(null, selectPatientComponentParameters);
        }
        public void RedirectToSelectPatient(string queryString, SelectPatientComponentParameters selectPatientComponentParameters)
        {
            if (queryString.Contains("StartOver=Y") || PageState.GetBooleanOrFalse(Constants.SessionVariables.AppComponentAlreadyInitialized))//since it is coming from Angular component
            {
                //WHEN APP COMPONENT ALREADY EXISTS
                var componentParameters = new JavaScriptSerializer().Serialize(selectPatientComponentParameters);
                Response.Redirect(Constants.PageNames.REDIRECT_TO_ANGULAR + "?componentName=" + Constants.PageNames.SELECT_PATIENT + "&componentParameters=" + componentParameters);
            }
            else
            {
                //LOGIN WORKFLOW WHEN NO APP COMPONENT IS THERE
                PageState[Constants.SessionVariables.AppComponentAlreadyInitialized] = true;
                Response.Redirect(RedirectHelper.GetRedirectToSPAUrl(string.Empty, false));
            }
        }
        protected void lbMyTasks_Click(object sender, EventArgs e)
        {
            if (Session["ISPROVIDER"] != null)
            {
                if (Convert.ToBoolean(Session["ISPROVIDER"]))
                {
                    Response.Redirect(Constants.PageNames.DOC_REFILL_MENU);
                }
            }
            else
            {
                Response.Redirect(Constants.PageNames.LIST_SEND_SCRIPTS);
            }
        }
        public void UpdatePatientHeader(Guid patientId)
        {
            PageState[Constants.SessionVariables.PatientId] = patientId.ToString();
            UpdatePatientHeader();
        }
        
        public void UpdatePatientHeader()
        {
            displayPatientHeader();
            patientPanel.Update();
        }
        public bool hasPatient()
        {
            return Session["PATIENTID"] != null;
        }

        public bool AllowPatientEdit
        {
            get { return allowPatientEdit; }
            set { allowPatientEdit = value; }
        }

        public bool AllowAllergyEdit
        {
            get { return allowAllergyEdit; }
            set { allowAllergyEdit = value; }
        }
        public bool AllowPharmacyEdit
        {
            get { return allowPharmacyEdit; }
            set { allowPharmacyEdit = value; }
        }
        public bool AllowDiagnosisEdit
        {
            get { return allowDiagnosisEdit; }
            set { allowDiagnosisEdit = value; }
        }

        protected void lnkEditPharmacy_Click(object sender, EventArgs e)
        {
            string from = ((WebControl)sender).Parent.Page.Request.AppRelativeCurrentExecutionFilePath;
            if (from.StartsWith("~/"))
            {
                from = from.Substring(2);
            }

            Response.Redirect(Constants.PageNames.PHARMACY + "?Mode=Edit&From=" + from);
        }
        protected void lnkEditMOPharmacy_Click(object sender, EventArgs e)
        {
            string from = ((WebControl)sender).Parent.Page.Request.AppRelativeCurrentExecutionFilePath;
            if (from.StartsWith("~/"))
            {
                from = from.Substring(2);
            }

            Response.Redirect(Constants.PageNames.PHARMACY + "?Mode=Edit&From=" + from + "&SetMO=true");
        }

        protected void btnSites_Click(object sender, EventArgs e)
        {
            string from = ((WebControl)sender).Parent.Page.Request.AppRelativeCurrentExecutionFilePath;
            if (from.StartsWith("~/"))
            {
                from = from.Substring(2);
            }

            Session["SITE_TOOLTIP_TEXT"] = null;

            Response.Redirect(Constants.PageNames.SELECT_ACCOUNT_AND_SITE + "?TargetURL=" + from);
        }
        protected void lnkRemPharmacy_Click(object sender, EventArgs e)
        {
            Patient.UpdatePharmacyID(
                    Session["PatientID"].ToString(),
                    System.Guid.Empty.ToString(),
                    false,
                    base.DBID);

            EPSBroker.AuditLogPatientInsert(
                ePrescribeSvc.AuditAction.PATIENT_RETAIL_PHARMACY_DELETE,
                base.SessionLicenseID,
                base.SessionUserID,
                base.SessionPatientID,
                Request.UserIpAddress(),
                base.DBID);

            Session["LASTPHARMACYNAME"] = null;
            Session["PHARMACYID"] = null;
            Session["LASTPHARMACYID"] = null;
            displayPatientHeader();
            imgPharmacyRating.Visible = false;
        }
        protected void lnkRemMOPharm_Click(object sender, EventArgs e)
        {
            Patient.UpdatePharmacyID(
                    Session["PatientID"].ToString(),
                    System.Guid.Empty.ToString(),
                    true,
                    base.DBID);

            EPSBroker.AuditLogPatientInsert(
                ePrescribeSvc.AuditAction.PATIENT_MAIL_PHARMACY_DELETE,
                base.SessionLicenseID,
                base.SessionUserID,
                base.SessionPatientID,
                Request.UserIpAddress(),
                base.DBID);

            Session["MOB_NABP"] = null;
            Session["PatientHasMOBCoverage"] = null;
            displayPatientHeader();
            imgMOPRating.Visible = false;
        }

        private void setClinicalViewerVisibility()
        {
            divClinicalViewer.Visible = false;
            hlClinicalViewer.Enabled = false;

            if (base.SessionLicense.EnterpriseClient.ShowClinicalViewer && !base.IsSSOUser)
            {
                divClinicalViewer.Visible = true;
            }
        }

        protected void masterScriptManager_AsyncPostBackError(object sender, AsyncPostBackErrorEventArgs e)
        {
            ScriptManagerCommon.LogException(e.Exception);
        }

        /// <summary>
        /// return Site Tooltip Text for a given site
        /// </summary>
        /// <param name="licenseID">Site License ID</param>
        /// <param name="siteID">Site ID</param>
        /// <returns></returns>
        private static string getSiteTooltipText(string licenseID, int siteID)
        {
            StringBuilder siteTooltipText = new StringBuilder();

            if (licenseID != null && siteID != 0)
            {
                ePrescribeSvc.LicenseSite licenseSite = EPSBroker.GetLicenseSiteByID(licenseID, siteID);
                siteTooltipText.Append(licenseSite.Address1 + "\n");

                if (licenseSite.Address2 != null && licenseSite.Address2 != "")
                {
                    siteTooltipText.Append(licenseSite.Address2 + "\n");
                }

                siteTooltipText.Append(licenseSite.City + ", ");
                siteTooltipText.Append(licenseSite.State + " ");
                siteTooltipText.Append(licenseSite.ZIPCode + "\n");
                siteTooltipText.Append(Allscripts.Impact.Utilities.StringHelper.FormatPhone(licenseSite.PhoneAreaCode, licenseSite.PhoneNumber));
            }

            return siteTooltipText.ToString();
        }

        protected void pharmInfoObjDataSource_Selected(object sender, ObjectDataSourceStatusEventArgs e)
        {
            DataSet ds = (DataSet)e.ReturnValue;
            if (ds != null && ds.Tables.Count > 0)
            {
                DataTable dt = ds.Tables[0];
                if (dt.Rows.Count > 0)
                {
                    string pharmacyID = dt.Rows[0].ItemArray[1].ToString();
                    dt.Columns.Add("EPCS", typeof(string));

                    if (dt.Rows[0]["EpcsEnabled"].ToString() == "1")
                    {
                        dt.Rows[0]["EPCS"] = "YES";
                    }
                    else
                    {
                        dt.Rows[0]["EPCS"] = "NO";
                    }

                    detailsViewPharmInfo.DataSource = dt;
                    detailsViewPharmInfo.DataBind();
                    detailsViewPharmInfo.Visible = true;

                    Label phoneCtrl = (Label)detailsViewPharmInfo.FindControl("lblPhone");
                    string phone = "";

                    try
                    {
                        phone = dt.Rows[0]["PhoneAreaCode"].ToString() + dt.Rows[0]["PhoneNumber"].ToString();
                        phoneCtrl.Text = String.Format("{0:(###) ###-####}", Convert.ToInt64(phone));
                    }
                    catch
                    {
                        phoneCtrl.Text = phone;
                    }

                    Label faxCtrl = (Label)detailsViewPharmInfo.FindControl("lblFax");
                    string fax = "";

                    try
                    {
                        fax = dt.Rows[0]["FaxAreaCode"].ToString() + dt.Rows[0]["FaxNumber"].ToString();
                        faxCtrl.Text = String.Format("{0:(###) ###-####}", Convert.ToInt64(fax));
                    }
                    catch
                    {
                        faxCtrl.Text = fax;
                    }

                    Label transMethodCtrl = (Label)detailsViewPharmInfo.FindControl("lblTransMethod");
                    if (dt.Rows[0]["TransMethod"] != DBNull.Value && dt.Rows[0]["TransMethod"].ToString().Trim() != "")
                    {
                        if (dt.Rows[0]["TransMethod"].ToString().ToUpper() == "FAX")
                        {
                            transMethodCtrl.Text = "Fax";
                        }
                        else
                        {
                            transMethodCtrl.Text = "Electronic";
                        }
                    }
                }
            }
        }

        protected void MPharmInfoObjDataSource_Selected(object sender, ObjectDataSourceStatusEventArgs e)
        {
            DataSet ds = (DataSet)e.ReturnValue;
            if (ds != null && ds.Tables.Count > 0)
            {
                DataTable dt = ds.Tables[0];
                if (dt.Rows.Count > 0)
                {
                    string pharmacyID = dt.Rows[0].ItemArray[1].ToString();
                    dt.Columns.Add("EPCS", typeof(string));

                    if (dt.Rows[0]["EpcsEnabled"].ToString() == "1")
                    {
                        dt.Rows[0]["EPCS"] = "YES";
                    }
                    else
                    {
                        dt.Rows[0]["EPCS"] = "NO";
                    }

                    datailViewMOPharm.DataSource = dt;
                    datailViewMOPharm.DataBind();
                    datailViewMOPharm.Visible = true;

                    Label phoneCtrl = (Label)datailViewMOPharm.FindControl("lblMPhone");
                    string phone = "";

                    try
                    {
                        phone = dt.Rows[0]["PhoneAreaCode"].ToString() + dt.Rows[0]["PhoneNumber"].ToString();
                        phoneCtrl.Text = String.Format("{0:(###) ###-####}", Convert.ToInt64(phone));
                    }
                    catch
                    {
                        phoneCtrl.Text = phone;
                    }

                    Label faxCtrl = (Label)datailViewMOPharm.FindControl("lblMFax");
                    string fax = "";

                    try
                    {
                        fax = dt.Rows[0]["FaxAreaCode"].ToString() + dt.Rows[0]["FaxNumber"].ToString();
                        faxCtrl.Text = String.Format("{0:(###) ###-####}", Convert.ToInt64(fax));
                    }
                    catch
                    {
                        faxCtrl.Text = fax;
                    }


                    Label transMethodCtrl = (Label)datailViewMOPharm.FindControl("lblMTransMethod");

                    if (dt.Rows[0]["TransMethod"] != DBNull.Value && dt.Rows[0]["TransMethod"].ToString().Trim() != "")
                    {
                        if (dt.Rows[0]["TransMethod"].ToString().ToUpper() == "FAX")
                        {
                            transMethodCtrl.Text = "Fax";
                        }
                        else
                        {
                            transMethodCtrl.Text = "Electronic";
                        }
                    }
                }
            }
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
    }

}
