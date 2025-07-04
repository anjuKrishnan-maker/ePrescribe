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
    public partial class Controls_AdControl : BaseControl
    {
        public enum AdMode
        {
            MODAL = 0,
            FULL_PAGE = 1,
            FULL_PAGE_TEASER = 2
        }

        public bool Show
        {
            get { return adPanel.Style["display"] == "inline"; }
            set
            {
                if (!value)
                    adPanel.Style["display"] = "none";
                else
                    adPanel.Style["display"] = "inline";
            }
        }
        bool _redirectFromSelectMedLink = false;
        public bool RedirectFromSelectMedLink
        {
            get { return _redirectFromSelectMedLink; }
            set { _redirectFromSelectMedLink = value; }
        }
        private int _skipTime = 30;
        public int SkipTime
        {
            get { return _skipTime; }
            set { _skipTime = value; }
        }

        private Module.ModuleType _featuredModule = Module.ModuleType.DELUXE;
        public new Module.ModuleType FeaturedModule
        {
            get { return _featuredModule; }
            set
            {
                _featuredModule = value;
                Module feature = new Module(_featuredModule, base.DBID);

                if (feature.Type == Module.ModuleType.DELUXE)
                {
                    if (SessionLicense.LicenseDeluxeStatus == Constants.DeluxeFeatureStatus.Off
                        || AppCode.CompulsoryBasicUtil.IsCompulsoryBasicPurchased(SessionLicense.IsEnterpriseCompulsoryBasicOrForceCompulsoryBasic(), SessionLicense.IsPricingStructureCompulsoryBasic()))
                    {
                        if (_displayMode == AdMode.FULL_PAGE)
                        {
                            adContent.InnerHtml = feature.InterstitialHTML;
                        }
                        else
                        {

                            if (_redirectFromSelectMedLink == true)
                            {
                                string TeaserAdHTML = feature.TeaserAdHTML.Replace("href=\"/DeluxeFeatureSelectionPage.aspx?From=Default.aspx\"", "href=#");
                                adContent.InnerHtml = TeaserAdHTML.Replace("<a", "<a onclick=\"closePopupAndRedirectToDeluxe('" + SessionLicense.LicenseDeluxeStatus + "')\"");
                            }
                            else
                            {
                                adContent.InnerHtml = feature.TeaserAdHTML.Replace("<P>", "<p onclick=\"adClicked()\">");
                                //Temp hardcoding as this issue is for help site only and due to temporary iFrame.
                                if (isRequestFromHelpSite())
                                {
                                    adContent.InnerHtml = adContent.InnerHtml.Replace("/DeluxeFeatureSelectionPage.aspx", "/"+ Constants.PageNames.REDIRECT_TO_ANGULAR +"?componentName=DeluxeFeatureSelectionComponent");
                                }
                                else
                                {
                                    adContent.InnerHtml = adContent.InnerHtml.Replace("href=\"/DeluxeFeatureSelectionPage.aspx?From=Default.aspx\"", "href=" + Constants.PageNames.REDIRECT_TO_ANGULAR + "?componentName=DeluxeFeatureSelectionComponent");
                                }
                            }
                        }
                    }
                    else if (SessionLicense.LicenseDeluxeStatus == Constants.DeluxeFeatureStatus.Disabled)
                    {

                        if (_redirectFromSelectMedLink == true)
                        {
                            string TeaserAdHTML = feature.TeaserAdHTML.Replace("href=\"/DeluxeAccountManagement.aspx?From=Default.aspx\"", "href=#");
                            adContent.InnerHtml = TeaserAdHTML.Replace("<a", "<a onclick=\"closePopupAndRedirectToDeluxe('" + SessionLicense.LicenseDeluxeStatus + "')\"");
                        }

                        else
                        {
                            adContent.InnerHtml = feature.DisabledAdHTML;
                            if (isRequestFromHelpSite())
                            {
                                adContent.InnerHtml = adContent.InnerHtml.Replace("/DeluxeAccountManagement.aspx", "/SPA.aspx?Page=DeluxeAccountManagement.aspx");
                            }
                        }
                    }
                }
                else
                {
                    if (_displayMode == AdMode.FULL_PAGE)
                    {
                        adContent.InnerHtml = feature.InterstitialHTML;
                    }
                    else
                    {
                        adContent.InnerHtml = feature.TeaserAdHTML;
                    }
                }

                featureModule.InnerText = feature.Name;
                HttpCookie eRxNowModuleAdCookie;
                string cookieName = "eRxNow" + feature.ModuleID;
                if (Request.Cookies[cookieName] != null)
                {
                    eRxNowModuleAdCookie = Request.Cookies.Get(cookieName);
                }
                else
                {
                    eRxNowModuleAdCookie = new HttpCookie(cookieName);
                }

                eRxNowModuleAdCookie.Expires = DateTime.Parse(DateTime.Today.ToShortDateString()).AddDays(feature.AdDaysInterval);

                Response.Cookies.Remove(eRxNowModuleAdCookie.Name);
                Response.Cookies.Add(eRxNowModuleAdCookie);
            }
        }

        private bool isRequestFromHelpSite()
        {
            bool returnvalue = false;
            if(Request.Url.OriginalString.ToLower().Contains("import.aspx"))
            {
                returnvalue = true;
            }
            //If more pages are required, add to else if
            return returnvalue;
        }

        private AdMode _displayMode = AdMode.FULL_PAGE;
        public AdMode DisplayMode
        {
            get { return _displayMode; }
            set
            {
                _displayMode = value;
                switch (_displayMode)
                {
                    case AdMode.FULL_PAGE:
                    case AdMode.FULL_PAGE_TEASER:
                        adPanel.Attributes["class"] = "interstitialAdMain";
                        adContent.Attributes["class"] = "interstitialAdContent";
                        break;
                    case AdMode.MODAL:
                        adPanel.Attributes["class"] = "interstitialAdModal";
                        adContent.Attributes["class"] = "interstitialAdModalContent";
                        closeButton.Visible = false;
                        break;
                }
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                iSkipTime.Value = _skipTime.ToString();
                if (_skipTime > 0)
                {
                    closeText.InnerText = "click here to skip or wait " + _skipTime.ToString() + " seconds";
                }
                else
                {
                    closeSpan.Visible = false;
                    closeButton.Visible = false;
                }
            }
        }
        protected void closeButton_Click(object sender, EventArgs e)
        {
            Session["InterstitialAd"] = null;
            if (string.IsNullOrWhiteSpace(Request.QueryString["TargetURL"]))
            {
                DefaultRedirect(false);
            }
            else
            {
                string UrlForRedirection = Constants.PageNames.UrlForRedirection(Request.QueryString["TargetURL"].ToString());
                if (string.IsNullOrEmpty(UrlForRedirection))
                {
                    DefaultRedirect(false);
                }
                else
                {
                    Response.Redirect(UrlForRedirection);
                }
            }
        }
        protected void adButton_Click(object sender, EventArgs e)
        {
            string url = ConfigurationManager.AppSettings["appurl"].ToString();
            string returnPage = string.Empty;

            if (base.SessionLicense.LicenseDeluxeStatus == Constants.DeluxeFeatureStatus.Off)
            {
                if (!string.IsNullOrEmpty(Request.AppRelativeCurrentExecutionFilePath))
                {
                    returnPage = "&From=" + Request.AppRelativeCurrentExecutionFilePath;
                }
                Module feature = new Module(_featuredModule, base.DBID);
                Response.Redirect(url + "/" + Constants.PageNames.INTEGRATION_SOLUTIONS_LIST + "?Module=" + feature.ModuleID + returnPage);
            }
            else
            {
                if (!string.IsNullOrEmpty(Request.AppRelativeCurrentExecutionFilePath))
                {
                    returnPage = "?From=" + Request.AppRelativeCurrentExecutionFilePath;
                }
                Response.Redirect(url + "/" + Constants.PageNames.DELUXE_ACCOUNT_MANAGEMENT + returnPage);
            }
        }
    }

}