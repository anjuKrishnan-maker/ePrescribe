using System;
using System.Collections.Generic;
using eRxWeb;
using eRxWeb.AppCode;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Allscripts.ePrescribe.Test.ERxWebTests.AppCodeTests.SsoTests
{
    [TestClass]
    public class FindPartnerByIssuerTests
    {
        [TestMethod]
        public void should_return_partner_that_matches_issuer_sent_in()
        {
            //arrange
            var issuer = "America";
            var partner1 = new Partner
            {
                Issuer = "KingTut"
            };

            var partner2 = new Partner
            {
                Issuer = issuer
            };

            Partners partners = new Partners(new  List<Partner> {partner1, partner2});

            //act
            var result = Sso.FindPartnerByIssuer(partners, issuer);

            //assert
            Assert.AreEqual(partner2, result);
        }

        [TestMethod]
        public void should_throw_exception_if_partner_is_not_found()
        {
            //arrange
            bool exThrown = false;
            var partner1 = new Partner
            {
                Issuer = "KingTut"
            };

            var partner2 = new Partner
            {
                Issuer = "America"
            };

            Partners partners = new Partners(new List<Partner> { partner1, partner2 });

            //act
            try
            {
                var result = Sso.FindPartnerByIssuer(partners, "Jumanji");
            }
            catch
            {
                exThrown = true;
            }
            Assert.IsTrue(exThrown);
        }
    }
}
