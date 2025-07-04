using System.Collections.Generic;
using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.Objects;
using eRxWeb.AppCode;
using eRxWeb.AppCode.Interfaces;
using eRxWeb.State;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;

namespace Allscripts.ePrescribe.Test.AppCodeTests.UserPropertiesTests
{
    [TestClass]
    public class GetUserPropertyValueTests
    {
        private IUserProperties _userProperties = new UserProperties();
        [TestMethod]
        public void should_return_emptyString_if_session_is_null()
        {
            //arrange
            var mockState = MockRepository.GenerateStub<IStateContainer>();

            //act
            var result = _userProperties.GetUserPropertyValue(Constants.UserPropertyNames.COMPROMISED_IDENTITY, mockState);

            //assert
            Assert.AreEqual(string.Empty, result);
        }

        [TestMethod]
        public void should_return_emptyString_if_property_name_is_not_found()
        {
            //arrange
            var props = new List<Property>
            {
                new Property("1", "FirstProp", "True")
            };
            var mockState = MockRepository.GenerateStub<IStateContainer>();
            mockState[Constants.SessionVariables.UserAppProperties] = props;

            //act
            var result = _userProperties.GetUserPropertyValue(Constants.UserPropertyNames.COMPROMISED_IDENTITY, mockState);

            //assert
            Assert.AreEqual(string.Empty, result);
        }

        [TestMethod]
        public void should_return_property_value_if_property_name_is_found()
        {
            //arrange
            var prop1 = new eRxWeb.ePrescribeSvc.Property
            {
                PropertyID = "1",
                PropertyName = Constants.UserPropertyNames.DONT_SHOW_EPA_POPUP,
                PropertyStatus = bool.TrueString
            };

            var props = new [] { prop1 };

            var mockState = MockRepository.GenerateStub<IStateContainer>();
            mockState[Constants.SessionVariables.UserAppProperties] = props;

            //act
            var result = _userProperties.GetUserPropertyValue(Constants.UserPropertyNames.DONT_SHOW_EPA_POPUP, mockState);

            //assert
            Assert.AreEqual(bool.TrueString, result);
        }
    }
}
