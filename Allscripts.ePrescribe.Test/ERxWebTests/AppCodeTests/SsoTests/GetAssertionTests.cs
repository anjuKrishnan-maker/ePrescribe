using System;
using ComponentSpace.SAML.Assertions;
using ComponentSpace.SAML.Protocol;
using eRxWeb.AppCode;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Allscripts.ePrescribe.Test.ERxWebTests.AppCodeTests.SsoTests
{
    [TestClass]
    public class GetAssertionTests
    {
        [TestMethod]
        public void should_throw_exception_if_saml_response_status_is_not_successful()
        {
            //arrange
            var response = new Response();
            response.Status = new Status();
            var isException = false;

            //act
            try
            {
                Sso.GetAssertion(response);
            }
            catch (ArgumentException)
            {
                isException = true;
            }

            //assert
            Assert.IsTrue(isException);
        }

        [TestMethod]
        public void should_not_throw_exception_if_saml_response_status_is_successful()
        {
            //arrange
            var response = new Response();
            response.Status = new Status("Success");
            var isException = false;

            //act
            try
            {
                Sso.GetAssertion(response);
            }
            catch (ArgumentException)
            {
                isException = true;
            }

            //assert
            Assert.IsFalse(isException);
        }

        [TestMethod]
        public void should_return_first_assertion_object_from_assertion_if_it_is_not_xmlelement()
        {
            //arrange
            var response = new Response();
            response.Status = new Status("Success");
            var assertion1 = new Assertion {Issuer = "Assert1"};
            var assertion2 = new Assertion { Issuer = "Assert2" };
            response.Assertions.Add(assertion1);
            response.Assertions.Add(assertion2);

            //act
            var result = Sso.GetAssertion(response);

            //assert
            Assert.AreEqual(assertion1.Issuer, result.Issuer);
        }

        [TestMethod]
        public void should_return_null_if_no_assertions_can_be_found()
        {
            //arrange
            var response = new Response();
            response.Status = new Status("Success");

            //act
            var result = Sso.GetAssertion(response);

            //assert
            Assert.AreEqual(null,result);
        }
    }
}
