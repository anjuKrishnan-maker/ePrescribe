using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web;
using Allscripts.Impact.Interfaces;
using Allscripts.Impact.Utilities;
using eRxWeb.AppCode;
using eRxWeb.AppCode.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using TieServiceClient;

namespace Allscripts.ePrescribe.Test.ERxWebTests.AppCodeTests.TieUtilityTests
{
    [TestClass]
    public class GetAdPlacementTests
    {
        [TestMethod]
        public void should_return_placementResponse_as_null_if_IsTieAdPlacementEnabled_is_false()
        {
            //arrange
            var locationId = "";
            var tieUtilityMock = MockRepository.GenerateMock<ITieUtility>();
            var tieServiceManagerMock = MockRepository.GenerateMock<ITIEServiceManager>();

            var configurationMock = MockRepository.GenerateMock<IConfigurationManager>();
            configurationMock.Stub(x => x.GetValue("IsTieAdPlacementEnabled")).Return("false");

            //act
            var result = TieUtility.GetAdPlacement(locationId, new HttpCookieCollection(), tieUtilityMock, tieServiceManagerMock, configurationMock);

            //assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void should_return_placementResponse_as_null_if_adOptOut_is_Y()
        {
            //arrange
            var locationId = "";
            var cookie = new HttpCookie("eRxTIECookie");
            cookie.Values.Add(new NameValueCollection{{"AdOptOut", "Y"}});
            var tieUtilityMock = MockRepository.GenerateMock<ITieUtility>();
            var tieServiceManagerMock = MockRepository.GenerateMock<ITIEServiceManager>();

            var configurationMock = MockRepository.GenerateMock<IConfigurationManager>();
            configurationMock.Stub(x => x.GetValue("IsTieAdPlacementEnabled")).Return("True");

            //act
            var result = TieUtility.GetAdPlacement(locationId, new HttpCookieCollection(), tieUtilityMock, tieServiceManagerMock, configurationMock);

            //assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void should_return_placementResponse_with_targeting_keys_if_erxtiecookie_is_not_null()
        {
            //arrange
            var locationId = "";
            var cookie = new HttpCookie("eRxTIECookie");

            var targetingKeys = new Dictionary<string, IEnumerable<string>> { { "DeaNumbers", new List<string> { "1", "2" } } };
            var tieUtilityMock = MockRepository.GenerateMock<ITieUtility>();
            tieUtilityMock.Stub(x => x.GetTargetKeys(null)).IgnoreArguments().Return(targetingKeys);

            var tieServiceManagerMock = MockRepository.GenerateMock<ITIEServiceManager>();

            var configurationMock = MockRepository.GenerateMock<IConfigurationManager>();
            configurationMock.Stub(x => x.GetValue("IsTieAdPlacementEnabled")).Return("true");

            //act
            TieUtility.GetAdPlacement(locationId, new HttpCookieCollection { cookie }, tieUtilityMock, tieServiceManagerMock, configurationMock);

            //assert
            var arguments = tieServiceManagerMock.GetArgumentsForCallsMadeOn(x => x.TIEPlacementResponse(Arg<string>.Is.Anything, Arg<Dictionary<string, IEnumerable<string>>>.Is.Anything, Arg<string>.Is.Anything));
            Assert.AreEqual(targetingKeys, arguments[0][1]);
        }

        [TestMethod]
        public void should_return_placementResponse_with_empty_targeting_keys_if_erxtiecookie_is_null()
        {
            //arrange
            var locationId = "";
            var cookie = new HttpCookie("eRxTIECookie");

            var targetingKeys = new Dictionary<string, IEnumerable<string>> ();
            var tieUtilityMock = MockRepository.GenerateMock<ITieUtility>();
            tieUtilityMock.Stub(x => x.GetTargetKeys(null)).IgnoreArguments().Return(targetingKeys);

            var tieServiceManagerMock = MockRepository.GenerateMock<ITIEServiceManager>();

            var configurationMock = MockRepository.GenerateMock<IConfigurationManager>();
            configurationMock.Stub(x => x.GetValue("IsTieAdPlacementEnabled")).Return("true");

            //act
            TieUtility.GetAdPlacement(locationId, new HttpCookieCollection { cookie }, tieUtilityMock, tieServiceManagerMock, configurationMock);

            //assert
            var arguments = tieServiceManagerMock.GetArgumentsForCallsMadeOn(x => x.TIEPlacementResponse(Arg<string>.Is.Anything, Arg<Dictionary<string, IEnumerable<string>>>.Is.Anything, Arg<string>.Is.Anything));
            Assert.AreEqual(targetingKeys, arguments[0][1]);
        }

        [TestMethod]
        public void should_return_placementResponse_if_IsTieAdPlacementEnabled_is_true()
        {
            //arrange
            var locationId = "";
            var cookie = new HttpCookie("eRxTIECookie");

            var targetingKeys = new Dictionary<string, IEnumerable<string>>();
            var tieUtilityMock = MockRepository.GenerateMock<ITieUtility>();
            tieUtilityMock.Stub(x => x.GetTargetKeys(null)).IgnoreArguments().Return(targetingKeys);

            var tieServiceManagerMock = MockRepository.GenerateMock<ITIEServiceManager>();
            tieServiceManagerMock.Stub(x => x.TIEPlacementResponse(null, null, null)).IgnoreArguments().Return(new DefaultPlacementResponse());

            var configurationMock = MockRepository.GenerateMock<IConfigurationManager>();
            configurationMock.Stub(x => x.GetValue("IsTieAdPlacementEnabled")).Return("true");

            //act
            var result = TieUtility.GetAdPlacement(locationId, new HttpCookieCollection { cookie }, tieUtilityMock, tieServiceManagerMock, configurationMock);

            //assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void should_call_get_ad_placements_with_empty_string_as_locationId()
        {
            //arrange
            var cookie = new HttpCookie("eRxTIECookie");
            
            var tieUtilityMock = MockRepository.GenerateMock<ITieUtility>();
            
            var tieServiceManagerMock = MockRepository.GenerateMock<ITIEServiceManager>();

            var configurationMock = MockRepository.GenerateMock<IConfigurationManager>();

            //act
            TieUtility.GetAdPlacement(new HttpCookieCollection { cookie }, tieUtilityMock, tieServiceManagerMock, configurationMock);

            //assert
            var arguments = tieUtilityMock.GetArgumentsForCallsMadeOn(x =>
                x.GetAdPlacement(Arg<string>.Is.Anything,
                                 Arg<HttpCookieCollection>.Is.Anything,
                                 Arg<ITieUtility>.Is.Anything,
                                 Arg<ITIEServiceManager>.Is.Anything,
                                 Arg<IConfigurationManager>.Is.Anything));

            Assert.AreEqual(string.Empty, arguments[0][0]);
        }
    }
}
