using System;
using Allscripts.Impact.Interfaces;
using ComponentSpace.SAML.Assertions;
using eRxWeb.AppCode;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;

namespace Allscripts.ePrescribe.Test.ERxWebTests.AppCodeTests.SsoTests
{
    [TestClass]
    public class ValidateTimeOnSsoPostTests
    {

        [TestMethod]
        public void should_return_true_if_user_is_keynote_guid()
        {
            //arrange
            var keyNoteGuid = "123452";
            var nameId = new NameIdentifier();
            nameId.Value = keyNoteGuid;

            var systemConfigMock = MockRepository.GenerateMock<ISystemConfig>();
            systemConfigMock.Stub(x => x.GetAppSetting(null)).IgnoreArguments().Return(keyNoteGuid);

            //act
            var result = Sso.IsKeynoteSSoUser(nameId, systemConfigMock);

            //assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void should_return_false_if_user_is_not_keynote_guid()
        {
            //arrange
            var keyNoteGuid = "123452";
            var nameId = new NameIdentifier();
            nameId.Value = keyNoteGuid;

            var systemConfigMock = MockRepository.GenerateMock<ISystemConfig>();
            systemConfigMock.Stub(x => x.GetAppSetting(null)).IgnoreArguments().Return("678901");

            //act
            var result = Sso.IsKeynoteSSoUser(nameId, systemConfigMock);

            //assert
            Assert.IsFalse(result);

        }

        [TestMethod]
        public void should_return_true_if_nameid_value_is_populated()
        {
            //arrange
            var nameId = new NameIdentifier();
            nameId.Value = "123456";

            //act
            var result = Sso.IsNameIdentifierValuePopulated(nameId);

            //assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void should_return_false_if_nameId_value_is_empty()
        {
            //arrange
            var nameId = new NameIdentifier();
            nameId.Value = "";

            //act
            var result = Sso.IsNameIdentifierValuePopulated(nameId);
            
            //assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void should_return_false_if_assert_conditions_are_null()
        {
            //arrange
            var assertion = new Assertion();

            //act
            var result = Sso.IsConditionsInSsoPost(assertion);

            //assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void should_return_false_if_assertion_is_not_within_10_minutes()
        {
            //arrange
            var assertion = new Assertion();
            assertion.Conditions = new Conditions(DateTime.UtcNow.AddMinutes(11), DateTime.UtcNow);

            //act
            var result = Sso.IsTimeValidOnSsoPost(assertion);

            //assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void should_return_true_if_assertion_is_within_10_minutes()
        {
            //arrange
            var assertion = new Assertion();
            assertion.Conditions = new Conditions(DateTime.UtcNow.AddMinutes(10), DateTime.UtcNow);

            //act
            var result = Sso.IsTimeValidOnSsoPost(assertion);

            //assert
            Assert.IsTrue(result);
        }
    }
}
