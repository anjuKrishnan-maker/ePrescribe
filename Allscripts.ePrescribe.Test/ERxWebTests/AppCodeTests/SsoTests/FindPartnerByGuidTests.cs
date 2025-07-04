using System;
using System.Collections.Generic;
using eRxWeb;
using eRxWeb.AppCode;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Allscripts.ePrescribe.Test.ERxWebTests.AppCodeTests.SsoTests
{
    [TestClass]
    public class FindPartnerByGuidTests
    {
        [TestMethod]
        public void should_return_partner_that_matches_guid_sent_in()
        {
            //arrange
            var guid = "833BBDC2-20E0-4899-9F5F-64F4AC557E76";

            var partner1 = new Partner
            {
                ID = guid
            };

            var partner2 = new Partner
            {
                ID = "4B4DCCF3-62F6-4FB4-AB23-1C17B7DA4B2E"
            };

            Partners partners = new Partners(new List<Partner> { partner1, partner2 });

            //act
            var result = Sso.FindPartnerByGuid(partners, new Guid(guid));

            //assert
            Assert.AreEqual(partner1, result);
        }

        [TestMethod]
        public void should_throw_exception_if_partner_is_not_found()
        {
            //arrange
            bool exThrown = false;
            var partner1 = new Partner
            {
                ID = "234"
            };

            var partner2 = new Partner
            {
                ID = "234"
            };

            Partners partners = new Partners(new List<Partner> { partner1, partner2 });

            //act
            try
            {
                Sso.FindPartnerByGuid(partners, new Guid());
            }
            catch
            {
                exThrown = true;
            }
            Assert.IsTrue(exThrown);
        }
    }
}
