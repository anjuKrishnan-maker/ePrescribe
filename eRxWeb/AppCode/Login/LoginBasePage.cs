using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.Shared.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eRxWeb.AppCode
{
    public class LoginBasePage : BasePage
    {
        private static ILoggerEx logger = LoggerEx.GetLogger();
        protected void RedirectUser(string redirect_url)
        {
            //If no license or expired dea.. go to edit page
            if (redirect_url.ToLower().Contains("status=expdea") || redirect_url.ToLower().Contains("status=nolic") || redirect_url.ToLower().Contains("status=nonpi"))
            {
                if (String.Compare(redirect_url.Substring(0, 2), "~/", true) == 0)
                {
                    redirect_url = redirect_url.Substring(2, redirect_url.Length - 2);  //Remove "~/"
                }
                logger.Debug($"Login: RedirectToSpa {redirect_url} Contains status=expdea || status=nolic || status=nonpi");
                Response.Redirect(RedirectHelper.GetRedirectToSPAUrl(redirect_url, false));
            }

            //check for interstitial ad
            if (IsDefaultRedirectRequired(redirect_url))
            {
                base.RedirectToInterstitialAdIfNeeded(string.Empty);
                DefaultRedirect(false);
            }

            //Check for spa navigation
            if (redirect_url.Contains(Constants.PageNames.SPA_LANDING))
            {
                logger.Debug($"Login: Redirect url contains spa_landing");
                Response.Redirect(Constants.PageNames.SPA_LANDING);
            }
            else
            {
                Response.Redirect(Constants.PageNames.UrlForRedirection(redirect_url));
            }

        }

        public bool IsDefaultRedirectRequired(string redirect_url)
        {
            bool isDefaultRedirect = false;
            int index = redirect_url.IndexOf('?');
            string redirectUrlSubString = redirect_url.Substring(0, index > 0 ? index : redirect_url.Length);
            //DefaultRedirect handles so no need to refactor 
            if (redirect_url.Trim().Length == 0
            || redirectUrlSubString.Trim().Contains(Constants.PageNames.SELECT_PATIENT.ToLower())) //If url is for SelectPatient component
            {
                isDefaultRedirect = true;
            }
            return isDefaultRedirect;
        }
    }
}