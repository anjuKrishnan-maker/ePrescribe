using System;
using System.Collections.Generic;
using Allscripts.ePrescribe.Objects;
using eRxWeb.AppCode;
using eRxWeb.AppCode.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;

namespace Allscripts.ePrescribe.Test.AppCodeTests.AnnouncementsTests
{
    [TestClass]
    public class CheckForServiceAlertsToShowTests
    {
        [TestMethod]
        public void should_convert_current_utc_datetime_to_eastern_timezone()
        {
            //arrange
            var accountId = "123455";
            var currentTimeZone = "Eastern Standard Time";
            var requestUserAgent = "234567";
            var enterpriseId = "234567";


            var loginMock = MockRepository.GenerateStub<ILogin>();
            loginMock.Stub(x => x.ShowServiceAlerts(null, null)).IgnoreArguments().Return(new List<ServiceAlert>());
            loginMock.Stub(x => x.ShowServiceAlertsLicense(null, null, null)).IgnoreArguments().Return(new List<ServiceAlert>());

            var browserUtilMock = MockRepository.GenerateStub<IBrowserUtil>();

            //act
            Announcements.CheckForServiceAlertsToShow(accountId, currentTimeZone, requestUserAgent, enterpriseId, loginMock, browserUtilMock);

            var easternCurrentTime = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, currentTimeZone).Date.ToString();
            //assert
            Assert.AreEqual(easternCurrentTime, loginMock.GetArgumentsForCallsMadeOn(x => x.ShowServiceAlerts(Arg<string>.Is.Anything, Arg<string>.Is.Anything))[0][1]);
        }

        [TestMethod]
        public void should_add_enterprise_alert_when_there_is_one_returned_from_ShowServiceAlerts()
        {
            //arrange
            var accountId = "123455";
            var currentTimeZone = "Eastern Standard Time";
            var requestUserAgent = "234567";
            var enterpriseId = "234567";
            var enterpriseServiceAlert = new ServiceAlert
            {
                Title = "Test ME Service",
                AlertTargetType = ObjectConstants.ServiceAlertTargetType.ENTERPRISE_CLIENT
            };

            var loginMock = MockRepository.GenerateStub<ILogin>();
            loginMock.Stub(x => x.ShowServiceAlerts(null, null)).IgnoreArguments().Return(new List<ServiceAlert> { enterpriseServiceAlert });
            loginMock.Stub(x => x.ShowServiceAlertsLicense(null, null, null)).IgnoreArguments().Return(new List<ServiceAlert>());

            var browserUtilMock = MockRepository.GenerateStub<IBrowserUtil>();

            //act
            var result = Announcements.CheckForServiceAlertsToShow(accountId, currentTimeZone, requestUserAgent, enterpriseId, loginMock, browserUtilMock);

            //assert
            Assert.AreEqual(enterpriseServiceAlert, result[0]);
        }

        [TestMethod]
        public void should_not_add_enterprise_alert_when_there_is_not_one_returned_from_ShowServiceAlerts()
        {
            //arrange
            var accountId = "123455";
            var currentTimeZone = "Eastern Standard Time";
            var requestUserAgent = "234567";
            var enterpriseId = "234567";

            var loginMock = MockRepository.GenerateStub<ILogin>();
            loginMock.Stub(x => x.ShowServiceAlerts(null, null)).IgnoreArguments().Return(new List<ServiceAlert>());
            loginMock.Stub(x => x.ShowServiceAlertsLicense(null, null, null)).IgnoreArguments().Return(new List<ServiceAlert>());

            var browserUtilMock = MockRepository.GenerateStub<IBrowserUtil>();

            //act
            var result = Announcements.CheckForServiceAlertsToShow(accountId, currentTimeZone, requestUserAgent, enterpriseId, loginMock, browserUtilMock);

            //assert
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void should_add_browser_alert_when_there_is_one_returned_from_ShowServiceAlerts_and_IsBrowserUpgradeNeeded_returns_true()
        {
            //arrange
            var accountId = "123455";
            var currentTimeZone = "Eastern Standard Time";
            var requestUserAgent = "234567";
            var enterpriseId = "234567";
            var browserAlert = new ServiceAlert
            {
                Title = "Test ME Service",
                AlertTargetType = ObjectConstants.ServiceAlertTargetType.BROWSER_ALERT
            };

            var loginMock = MockRepository.GenerateStub<ILogin>();
            loginMock.Stub(x => x.ShowServiceAlerts(null, null)).IgnoreArguments().Return(new List<ServiceAlert> { browserAlert });
            loginMock.Stub(x => x.ShowServiceAlertsLicense(null, null, null)).IgnoreArguments().Return(new List<ServiceAlert>());

            var browserUtilMock = MockRepository.GenerateStub<IBrowserUtil>();
            browserUtilMock.Stub(x => x.IsBrowserUpgradeNeeded(null, null)).IgnoreArguments().Return(true);

            //act
            var result = Announcements.CheckForServiceAlertsToShow(accountId, currentTimeZone, requestUserAgent, enterpriseId, loginMock, browserUtilMock);

            //assert
            Assert.AreEqual(browserAlert, result[0]);
        }

        [TestMethod]
        public void should_not_add_browser_alert_when_there_is_one_returned_from_ShowServiceAlerts_and_IsBrowserUpgradeNeeded_returns_false()
        {
            //arrange
            var accountId = "123455";
            var currentTimeZone = "Eastern Standard Time";
            var requestUserAgent = "234567";
            var enterpriseId = "234567";
            var browserAlert = new ServiceAlert
            {
                Title = "Test ME Service",
                AlertTargetType = ObjectConstants.ServiceAlertTargetType.BROWSER_ALERT
            };

            var loginMock = MockRepository.GenerateStub<ILogin>();
            loginMock.Stub(x => x.ShowServiceAlerts(null, null)).IgnoreArguments().Return(new List<ServiceAlert> { browserAlert });
            loginMock.Stub(x => x.ShowServiceAlertsLicense(null, null, null)).IgnoreArguments().Return(new List<ServiceAlert>());

            var browserUtilMock = MockRepository.GenerateStub<IBrowserUtil>();
            browserUtilMock.Stub(x => x.IsBrowserUpgradeNeeded(null, null)).IgnoreArguments().Return(false);

            //act
            var result = Announcements.CheckForServiceAlertsToShow(accountId, currentTimeZone, requestUserAgent, enterpriseId, loginMock, browserUtilMock);

            //assert
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void should_not_add_browser_alert_when_there_is_not_one_returned_from_ShowServiceAlerts_and_IsBrowserUpgradeNeeded_returns_true()
        {
            //arrange
            var accountId = "123455";
            var currentTimeZone = "Eastern Standard Time";
            var requestUserAgent = "234567";
            var enterpriseId = "234567";
            var browserAlert = new ServiceAlert
            {
                Title = "Test ME Service",
                AlertTargetType = ObjectConstants.ServiceAlertTargetType.BROWSER_ALERT
            };

            var loginMock = MockRepository.GenerateStub<ILogin>();
            loginMock.Stub(x => x.ShowServiceAlerts(null, null)).IgnoreArguments().Return(new List<ServiceAlert>());
            loginMock.Stub(x => x.ShowServiceAlertsLicense(null, null, null)).IgnoreArguments().Return(new List<ServiceAlert>());

            var browserUtilMock = MockRepository.GenerateStub<IBrowserUtil>();
            browserUtilMock.Stub(x => x.IsBrowserUpgradeNeeded(null, null)).IgnoreArguments().Return(true);

            //act
            var result = Announcements.CheckForServiceAlertsToShow(accountId, currentTimeZone, requestUserAgent, enterpriseId, loginMock, browserUtilMock);

            //assert
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void should_add_license_state_alert_if_one_is_returned_from_ShowServiceAlertsLicense()
        {
            //arrange
            var accountId = "123455";
            var currentTimeZone = "Eastern Standard Time";
            var requestUserAgent = "234567";
            var enterpriseId = "234567";
            var stateAlert = new ServiceAlert
            {
                Title = "Test ME State alert",
                AlertTargetType = ObjectConstants.ServiceAlertTargetType.STATE
            };

            var loginMock = MockRepository.GenerateStub<ILogin>();
            loginMock.Stub(x => x.ShowServiceAlerts(null, null)).IgnoreArguments().Return(new List<ServiceAlert>());
            loginMock.Stub(x => x.ShowServiceAlertsLicense(null, null, null)).IgnoreArguments().Return(new List<ServiceAlert> { stateAlert });

            var browserUtilMock = MockRepository.GenerateStub<IBrowserUtil>();

            //act
            var result = Announcements.CheckForServiceAlertsToShow(accountId, currentTimeZone, requestUserAgent, enterpriseId, loginMock, browserUtilMock);

            //assert
            Assert.AreEqual(stateAlert, result[0]);
        }

        [TestMethod]
        public void should_not_add_license_state_alert_if_one_is_not_returned_from_ShowServiceAlertsLicense()
        {
            //arrange
            var accountId = "123455";
            var currentTimeZone = "Eastern Standard Time";
            var requestUserAgent = "234567";
            var enterpriseId = "234567";

            var loginMock = MockRepository.GenerateStub<ILogin>();
            loginMock.Stub(x => x.ShowServiceAlerts(null, null)).IgnoreArguments().Return(new List<ServiceAlert>());
            loginMock.Stub(x => x.ShowServiceAlertsLicense(null, null, null)).IgnoreArguments().Return(new List<ServiceAlert>());

            var browserUtilMock = MockRepository.GenerateStub<IBrowserUtil>();

            //act
            var result = Announcements.CheckForServiceAlertsToShow(accountId, currentTimeZone, requestUserAgent, enterpriseId, loginMock, browserUtilMock);

            //assert
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void should_add_state_alert_text_to_title_of_state_alert()
        {
            //arrange
            var accountId = "123455";
            var currentTimeZone = "Eastern Standard Time";
            var requestUserAgent = "234567";
            var enterpriseId = "234567";
            var stateAlert = new ServiceAlert
            {
                Title = "Test ME State alert",
                AlertTargetType = ObjectConstants.ServiceAlertTargetType.STATE
            };

            var loginMock = MockRepository.GenerateStub<ILogin>();
            loginMock.Stub(x => x.ShowServiceAlerts(null, null)).IgnoreArguments().Return(new List<ServiceAlert>());
            loginMock.Stub(x => x.ShowServiceAlertsLicense(null, null, null)).IgnoreArguments().Return(new List<ServiceAlert> { stateAlert });

            var browserUtilMock = MockRepository.GenerateStub<IBrowserUtil>();

            //act
            var result = Announcements.CheckForServiceAlertsToShow(accountId, currentTimeZone, requestUserAgent, enterpriseId, loginMock, browserUtilMock);

            //assert
            Assert.AreEqual("State Alert: Test ME State alert", result[0].Title);
        }

        [TestMethod]
        public void should_add_pricing_alert_text_to_title_of_pricing_alert()
        {
            //arrange
            var accountId = "123455";
            var currentTimeZone = "Eastern Standard Time";
            var requestUserAgent = "234567";
            var enterpriseId = "234567";
            var pricingAlert = new ServiceAlert
            {
                Title = "Test ME Pricing alert",
                AlertTargetType = ObjectConstants.ServiceAlertTargetType.PRICING_STRUCTURE
            };


            var pricingStructure = 11;

            var loginMock = MockRepository.GenerateStub<ILogin>();
            loginMock.Stub(x => x.ShowServiceAlerts(null, null)).IgnoreArguments().Return(new List<ServiceAlert>());
            loginMock.Stub(x => x.ShowServiceAlertsPricing(pricingStructure, null)).IgnoreArguments().Return(new List<ServiceAlert> { pricingAlert });

            var browserUtilMock = MockRepository.GenerateStub<IBrowserUtil>();

            //act
            var result = Announcements.CheckForServiceAlertsToShow(accountId, currentTimeZone, requestUserAgent, enterpriseId, loginMock, browserUtilMock, pricingStructure);

            //assert
            Assert.AreEqual("Pricing Alert: Test ME Pricing alert", result[0].Title);

        }

        [TestMethod]
        public void should_add_pricing_alert_when_there_is_one_returned_from_ShowServiceAlerts()
        {
            //arrange
            var accountId = "123455";
            var currentTimeZone = "Eastern Standard Time";
            var requestUserAgent = "234567";
            var enterpriseId = "234567";
            var priceAlert = new ServiceAlert
            {
                Title = "Test ME Pricing alert",
                AlertTargetType = ObjectConstants.ServiceAlertTargetType.PRICING_STRUCTURE
            };
            var pricingStructure = 11;

            var loginMock = MockRepository.GenerateStub<ILogin>();
            loginMock.Stub(x => x.ShowServiceAlerts(null, null)).IgnoreArguments().Return(new List<ServiceAlert>());
            loginMock.Stub(x => x.ShowServiceAlertsPricing(pricingStructure, null)).IgnoreArguments().Return(new List<ServiceAlert> { priceAlert });

            var browserUtilMock = MockRepository.GenerateStub<IBrowserUtil>();

            //act
            var result = Announcements.CheckForServiceAlertsToShow(accountId, currentTimeZone, requestUserAgent, enterpriseId, loginMock, browserUtilMock, pricingStructure);

            //assert
            Assert.AreEqual(priceAlert, result[0]);
        }

        [TestMethod]
        public void should_not_add_pricing_alert_when_there_is_not_one_returned_from_ShowServiceAlerts()
        {
            //arrange
            var accountId = "123455";
            var currentTimeZone = "Eastern Standard Time";
            var requestUserAgent = "234567";
            var enterpriseId = "234567";

            var pricingStructure = 11;

            var loginMock = MockRepository.GenerateStub<ILogin>();
            loginMock.Stub(x => x.ShowServiceAlerts(null, null)).IgnoreArguments().Return(new List<ServiceAlert>());
            loginMock.Stub(x => x.ShowServiceAlertsPricing(pricingStructure, null)).IgnoreArguments().Return(new List<ServiceAlert>());

            var browserUtilMock = MockRepository.GenerateStub<IBrowserUtil>();

            //act
            var result = Announcements.CheckForServiceAlertsToShow(accountId, currentTimeZone, requestUserAgent, enterpriseId, loginMock, browserUtilMock, pricingStructure);

            //assert
            Assert.AreEqual(0, result.Count);
        }
    }
}
