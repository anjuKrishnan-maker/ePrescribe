using Allscripts.ePrescribe.Common;
using Allscripts.Impact;
using eRxWeb.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web;

namespace eRxWeb.AppCode
{
    public static class BillingUtil
    {
        public static bool ShouldUserFinishAPayment(IStateContainer session)
        {
            var sessionLicense = Settings.GetSessionLicense(session);

            return ApplicationLicense.IsForcedEnterpriseLicenseInForcedState(
                                    sessionLicense.EnterpriseDeluxeFeatureStatus,
                                    sessionLicense.LicenseDeluxeStatus) || 
                CompulsoryBasicUtil.ForceCompulsoryRestrictions(
                                    sessionLicense.IsEnterpriseCompulsoryBasicOrForceCompulsoryBasic(),
                                    sessionLicense.IsPricingStructureBasic(),
                                    sessionLicense.LicenseDeluxeStatus,
                                    Allscripts.Impact.ConfigKeys.CompulsoryBasicStartDate,
                                    sessionLicense.IsEnterpriseForceCompulsoryBasic(),
                                    CompulsoryBasicUtil.IsLicenseInFreeTrialPeriod(StateUtils.UserInfo.GetLicenseCreationDate(session), Allscripts.Impact.ConfigKeys.EPrescribeTrialDays));
        }        
    }
}