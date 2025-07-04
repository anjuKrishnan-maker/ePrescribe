using System;
using ComponentSpace.SAML.Assertions;
using eRxWeb.AppCode;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Allscripts.ePrescribe.Test.ERxWebTests.AppCodeTests.SsoTests
{
    [TestClass]
    public class VerifyUserGuidTests
    {
        [TestMethod]
        public void should_return_guid_from_authenticationStatement()
        {
            //arrange
            var sGuid = "58ECDDB2-50E9-4CF7-880E-653B1ACB326A";
            var authStatement = new AuthenticationStatement {Subject = new Subject {NameIdentifier = new NameIdentifier {Value = sGuid} } };

            //act
            var result = Sso.VerifyUserGuid(authStatement);

            //assert
            Assert.AreEqual(new Guid(sGuid), result);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void should_throw_exception_if_authStatement_value_is_not_valid_guid()
        {
            //arrange
            var sGuid = "NotAGuid";
            var authStatement = new AuthenticationStatement { Subject = new Subject { NameIdentifier = new NameIdentifier { Value = sGuid } } };

            //act
            Sso.VerifyUserGuid(authStatement);
        }
    }
}
