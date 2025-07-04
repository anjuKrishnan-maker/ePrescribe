using Allscripts.ePrescribe.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eRxWeb.AppCode
{
    public class ComponentRedirection
    {
        public bool IsPasswordSetupRequiredForSSOUser { get; set; }
        public string DefaultPatientLockdownPage { get; set; }
        public Constants.UserCategory UserCategory { get; set; }
        public int SiteId { get; set; }
        public bool DisableIntroductoryPopup { get; set; }
        public Guid UserId { get; set; }
        public string RetrieveRedirectionComponent(string ssoMode)
        {
            //Standard SSO Mode
            string redirectedComponent = Constants.PageNames.SELECT_PATIENT;
            if (IsPasswordSetupRequiredForSSOUser)
            {
                redirectedComponent = string.Concat(Constants.PageNames.FORCE_PASSWORD_SETUP, "?targetURL=", Constants.PageNames.SELECT_PATIENT);
                return redirectedComponent;
            }

            return redirectedComponent;
        }
    }
}