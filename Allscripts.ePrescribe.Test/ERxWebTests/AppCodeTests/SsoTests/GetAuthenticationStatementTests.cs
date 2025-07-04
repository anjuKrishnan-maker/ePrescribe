using System;
using System.Collections.Generic;
using ComponentSpace.SAML.Assertions;
using eRxWeb.AppCode;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Allscripts.ePrescribe.Test.ERxWebTests.AppCodeTests.SsoTests
{
    [TestClass]
    public class GetAuthenticationStatementTests
    {
        [TestMethod]
        public void should_return_authenticationStatement_and_not_throw_exception_if_one_is_present()
        {
            //arrange
            var assertion = new Assertion();

            var authStatement0 = new AuthenticationStatement("0");
            var authStatement1 = new AuthenticationStatement("1");

            assertion.Statements = new List<IStatement> {authStatement0, authStatement1};
            
            //act
            var actual = Sso.GetAuthenticationStatement(assertion);

            //assert
            Assert.AreEqual(authStatement0, actual);
        }

        [TestMethod]
        public void should_throw_exception_if_getauthenticationstatements_is_null()
        {
            //arrange
            var assertion = new Assertion();
            bool exThrown = false;

            //act
            try
            {
                Sso.GetAuthenticationStatement(assertion);
            }
            catch
            {
                exThrown = true;
            }
            Assert.IsTrue(exThrown);
        }

        [TestMethod]
        public void should_throw_exception_if_getauthenticationstatements_returns_count_of_0()
        {
            //arrange
            var assertion = new Assertion();
            assertion.Statements = new List<IStatement>();
            bool exThrown = false;

            //act
            try
            {
                Sso.GetAuthenticationStatement(assertion);
            }
            catch
            {
                exThrown = true;
            }
            Assert.IsTrue(exThrown);
        }
    }
}
