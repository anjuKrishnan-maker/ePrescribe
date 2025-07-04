using Allscripts.ePrescribe.Objects.CommonComponent;
using Allscripts.ePrescribe.Objects.PPTPlus;
using eRxWeb.AppCode.PptPlusBPL;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Allscripts.ePrescribe.Test.AppCodeTests.PPT
{
    [TestClass]
    public class GetCouponDistributionValueTests
    {

        [TestMethod, TestCategory("PPT")]
        public void should_return_print_if_coupon_was_only_printed()
        {
            //arrange

            //act
            var actual = PPTPlus.GetCouponDistributionValue(true, false, true);

            //assert
            Assert.AreEqual(FhirConstants.CouponDistribution.Print, actual);
        }


        [TestMethod, TestCategory("PPT")]
        public void should_return_none_if_coupon_not_printed()
        {
            //arrange

            //act
            var actual = PPTPlus.GetCouponDistributionValue(false, false, true);

            //assert
            Assert.AreEqual(FhirConstants.CouponDistribution.None, actual);
        }

        [TestMethod, TestCategory("PPT")]
        public void should_return_PrintSendToPharmacy_if_coupon_Printed_And_SentToPharmacy()
        {
            //arrange

            //act
            var actual = PPTPlus.GetCouponDistributionValue(true, true, true);

            //assert
            Assert.AreEqual(FhirConstants.CouponDistribution.PrintAndSend, actual);
        }

        [TestMethod, TestCategory("PPT")]
        public void should_return_SentToPharmacy_if_coupon_only_SentToPharmacy()
        {
            //arrange

            //act
            var actual = PPTPlus.GetCouponDistributionValue(false, true, true);

            //assert
            Assert.AreEqual(FhirConstants.CouponDistribution.SendToPharmacy, actual);
        }

        [TestMethod, TestCategory("PPT")]
        public void should_return_None_if_coupon_NotAvailable()
        {
            //arrange

            //act
            var actual = PPTPlus.GetCouponDistributionValue(false, true, false);

            //assert
            Assert.AreEqual(FhirConstants.CouponDistribution.None, actual);
        }
    }
}
