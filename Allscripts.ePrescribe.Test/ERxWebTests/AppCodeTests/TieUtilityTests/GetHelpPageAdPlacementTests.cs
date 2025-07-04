using System;
using System.Web;
using Allscripts.ePrescribe.Common;
using Allscripts.Impact.Interfaces;
using Allscripts.Impact.Utilities;
using eRxWeb.AppCode;
using eRxWeb.AppCode.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;

namespace Allscripts.ePrescribe.Test.ERxWebTests.AppCodeTests.TieUtilityTests
{
    [TestClass]
    public class GetHelpPageAdPlacementTests
    {
        [TestMethod]
        public void should_use_pageName_from_absoluteUrlParser_to_get_location_name()
        {
            //arrange
            var tieUtilityMock = MockRepository.GenerateMock<ITieUtility>();
            tieUtilityMock.Stub(x => x.ParseAbsoluteUrlForPageName("")).IgnoreArguments().Return("login");

            //act
            TieUtility.GetHelpPageAdPlacement("", new HttpCookieCollection(), tieUtilityMock);

            //assert
            var arguments = tieUtilityMock.GetArgumentsForCallsMadeOn(x => x.GetHelpPageTieLocationName(Arg<string>.Is.Anything));
            Assert.AreEqual("login", arguments[0][0]);
        }

        [TestMethod]
        public void should_use_locationName_from_tieLocation_method_to_get_ad_placement()
        {
            //arrange
            var tieUtilityMock = MockRepository.GenerateMock<ITieUtility>();
            tieUtilityMock.Stub(x => x.GetHelpPageTieLocationName("")).IgnoreArguments().Return(Constants.TIELocationPage.Login_Page);

            //act
            TieUtility.GetHelpPageAdPlacement("", new HttpCookieCollection(), tieUtilityMock);

            //assert
            var arguments = tieUtilityMock.GetArgumentsForCallsMadeOn(x =>
                        x.GetAdPlacement(Arg<string>.Is.Anything,
                                         Arg<HttpCookieCollection>.Is.Anything,
                                         Arg<ITieUtility>.Is.Anything,
                                         Arg<ITIEServiceManager>.Is.Anything,
                                         Arg<IConfigurationManager>.Is.Anything));

            Assert.AreEqual(Constants.TIELocationPage.Login_Page, arguments[0][0]);
        }
    }
}
