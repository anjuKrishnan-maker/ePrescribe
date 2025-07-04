using Allscripts.ePrescribe.Common;
using Allscripts.Impact;
using eRxWeb.AppCode.StateUtils;
using eRxWeb.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eRxWeb.AppCode
{
    public class Settings
    {
        public static bool IsSendSMSEnabled(IStateContainer state)
        {
            var SessionLicense = UserInfo.GetSessionLicense(state);
            return (SessionLicense.EnterpriseClient.ShowSMS &&
                (Convert.ToChar(Allscripts.Impact.ConfigKeys.SMSAlertForDeluxeOnly) == 'N' ||
                (Convert.ToChar(Allscripts.Impact.ConfigKeys.SMSAlertForDeluxeOnly) == 'Y' &&
                (SessionLicense.LicenseDeluxeStatus == Constants.DeluxeFeatureStatus.On ||
           SessionLicense.LicenseDeluxeStatus == Constants.DeluxeFeatureStatus.AlwaysOn ||
           SessionLicense.LicenseDeluxeStatus == Constants.DeluxeFeatureStatus.Cancelled))));
        }

        public static ApplicationLicense GetSessionLicense(IStateContainer state)
        {
            return UserInfo.GetSessionLicense(state);
        }
        
        public static bool IsLicenseShieldEnabled(IStateContainer state)
        {
            bool isEnabled = false;

            if (state["IsLicenseShieldEnabled"] != null)
            {
                isEnabled = Convert.ToBoolean(state["IsLicenseShieldEnabled"]);
            }

            return isEnabled;
        }
    }
}