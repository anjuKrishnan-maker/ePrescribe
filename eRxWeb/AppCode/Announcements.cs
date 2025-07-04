using System;
using System.Collections.Generic;
using System.Linq;
using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.Objects;
using eRxWeb.AppCode.Interfaces;
using Allscripts.ePrescribe.Shared.Logging;

namespace eRxWeb.AppCode
{
    public class Announcements
    {
        private static readonly ILoggerEx logger = LoggerEx.GetLogger();

        public static List<ServiceAlert> CheckForServiceAlertsToShow(string accountId, string currentSiteZone, string requestUserAgent,
            string enterpriseClientId, ILogin iLogin, IBrowserUtil iBrowserUtil, int pricingStructure = -1)
        {
            // check if service alert needs to be displayed!!!
            string dtCurrentTimeZone = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, currentSiteZone).Date.ToString();

            var serviceAlerts = iLogin.ShowServiceAlerts(enterpriseClientId, dtCurrentTimeZone);

            List<ServiceAlert> serviceAlertsToBeShown = new List<ServiceAlert>();

            SetEnterpriseServiceAlerts(serviceAlerts, serviceAlertsToBeShown);

            SetStateLicenseServiceAlerts(serviceAlertsToBeShown, iLogin, enterpriseClientId, dtCurrentTimeZone, accountId);

            SetBrowserServiceAlerts(serviceAlerts, serviceAlertsToBeShown, requestUserAgent, iBrowserUtil);

            SetPricingServiceAlerts(serviceAlertsToBeShown, pricingStructure, dtCurrentTimeZone, iLogin);

            return serviceAlertsToBeShown;
        }

        private static void SetPricingServiceAlerts(List<ServiceAlert> displayServiceAlerts, int pricingStructure, string dtCurrentTimeZone, ILogin iLogin)
        {
            if (pricingStructure == -1)
                return;
            if (pricingStructure == (int)Constants.DeluxePricingStructure.Basic)
                pricingStructure = Convert.ToInt32(Constants.DeluxePricingStructure.BasicAlternative);
            var servicePricingAlert = iLogin.ShowServiceAlertsPricing(pricingStructure, dtCurrentTimeZone);
            if (!servicePricingAlert.Any())
                return;
            SetFirstAlertAndLog(displayServiceAlerts, servicePricingAlert, "Pricing Alert");
        }

        private static void SetEnterpriseServiceAlerts(List<ServiceAlert> allServiceAlerts, List<ServiceAlert> displayServiceAlerts)
        {
            var enterpriseAlerts = allServiceAlerts.Where(serviceAlert => serviceAlert.AlertTargetType == ObjectConstants.ServiceAlertTargetType.ENTERPRISE_CLIENT).ToList();

            if (!enterpriseAlerts.Any())
                return;

            displayServiceAlerts.Add(enterpriseAlerts[0]);
            logger.Debug($"Enterprise alert shown {enterpriseAlerts[0].Title}");
        }

        private static void SetBrowserServiceAlerts(List<ServiceAlert> allServiceAlerts, List<ServiceAlert> displayServiceAlerts, string requestUserAgent, IBrowserUtil iBrowserUtil)
        {
            var browserAlert = allServiceAlerts.Where(serviceAlert => serviceAlert.AlertTargetType == ObjectConstants.ServiceAlertTargetType.BROWSER_ALERT).ToList();

            if (browserAlert.Any() && iBrowserUtil.IsBrowserUpgradeNeeded(requestUserAgent?.ToLower(), UserAgentParser.GetLayoutEngineFromUserAgent(requestUserAgent)))
            {
                displayServiceAlerts.Add(browserAlert[0]);
                logger.Debug($"Enterprise alert shown {browserAlert[0].Title}");
            }
        }

        private static void SetStateLicenseServiceAlerts(List<ServiceAlert> displayServiceAlerts, ILogin iLogin, string enterpriseClientId, string dtCurrentTimeZone, string accountId)
        {
            var serviceAlertsLicenseState = iLogin.ShowServiceAlertsLicense(enterpriseClientId, dtCurrentTimeZone, accountId);
            if (serviceAlertsLicenseState == null || !serviceAlertsLicenseState.Any())
                return;

            SetFirstAlertAndLog(displayServiceAlerts, serviceAlertsLicenseState, "State Alert");

        }

        private static void SetFirstAlertAndLog(List<ServiceAlert> displayServiceAlerts, List<ServiceAlert> serviceAlerts, string alertTitle)
        {
            ServiceAlert serviceAlert = serviceAlerts[0];
            serviceAlert.Title = $"{alertTitle}: {serviceAlert.Title}";
            displayServiceAlerts.Add(serviceAlert);
            logger.Debug($"{alertTitle} shown {serviceAlert.Title}");
        }
    }
}