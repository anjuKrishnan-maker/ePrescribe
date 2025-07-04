using System;
using eRxWeb.AppCode;
using eRxWeb.AppCode.Interfaces;
using eRxWeb.State;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;

namespace Allscripts.ePrescribe.Test.AppCodeTests.UserPropertiesTests
{
    [TestClass]
    public class GetBoolOrFalseUserPropertyValueTests
    {
        [TestMethod]
        public void should_return_false_if_value_is_null()
        {
            //arrange
            var userPropMock = MockRepository.GenerateMock<IUserProperties>();
            userPropMock.Stub(x => x.GetUserPropertyValue(null, null)).IgnoreArguments().Return(null);

            //act
            var result = UserProperties.GetBoolOrFalseUserPropertyValue("", null, userPropMock);

            //assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void should_return_false_if_value_is_false()
        {
            //arrange
            var userPropMock = MockRepository.GenerateMock<IUserProperties>();
            userPropMock.Stub(x => x.GetUserPropertyValue(null, null)).IgnoreArguments().Return(bool.FalseString);

            //act
            var result = UserProperties.GetBoolOrFalseUserPropertyValue("", null, userPropMock);

            //assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void should_return_true_if_value_is_true()
        {
            //arrange
            var userPropMock = MockRepository.GenerateMock<IUserProperties>();
            userPropMock.Stub(x => x.GetUserPropertyValue(null, null)).IgnoreArguments().Return(bool.TrueString);

            //act
            var result = UserProperties.GetBoolOrFalseUserPropertyValue("", null, userPropMock);

            //assert
            Assert.IsTrue(result);
        }
    }
}
