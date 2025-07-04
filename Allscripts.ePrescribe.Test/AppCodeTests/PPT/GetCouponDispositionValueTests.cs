using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.Objects.CommonComponent;
using Allscripts.ePrescribe.Objects.PPTPlus;
using eRxWeb.AppCode.PptPlusBPL;
using eRxWeb.State;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Allscripts.ePrescribe.Test.AppCodeTests.PPT
{
    [TestClass]
    public class GetCouponDispositionValueTests
    {
        private IStateContainer _session;


        [TestInitialize]
        public void init()
        {
            _session = MockRepository.GenerateStub<IStateContainer>();

        }

        [TestMethod, TestCategory("PPT")]
        public void should_return_passivelyDistributed_if_ppt_and_printAutomatically_preference_true_and_coupon_printed()
        {
            //arrange
            _session.Stub(_ => _.GetBooleanOrFalse(Constants.SessionVariables.ShouldShowPpt)).Return(true);
            _session.Stub(_ => _.GetBooleanOrFalse(Constants.SessionVariables.ShouldPrintOfferAutomatically)).Return(true);

            //act
            var actual = PPTPlus.GetCouponDispositionValue(_session, true, true, false);

            //assert
            Assert.AreEqual(FhirConstants.CouponDisposition.PassivelyDistributed, actual);
        }

        [TestMethod, TestCategory("PPT")]
        public void should_return_activelydistributed_if_ppt_preference_true_printAutomatically_preference_false_and_coupon_not_printed()
        {
            //arrange
            _session.Stub(_ => _.GetBooleanOrFalse(Constants.SessionVariables.ShouldShowPpt)).Return(true);
            _session.Stub(_ => _.GetBooleanOrFalse(Constants.SessionVariables.ShouldPrintOfferAutomatically)).Return(false);

            //act
            var actual = PPTPlus.GetCouponDispositionValue(_session, true, true, false);

            //assert
            Assert.AreEqual(FhirConstants.CouponDisposition.Activelydistributed, actual);
        }


        [TestMethod, TestCategory("PPT")]
        public void should_return_passivelyNotDistributed_if_ppt_and_printAutomatically_preference_false_and_coupon_not_printed()
        {
            //arrange
            _session.Stub(_ => _.GetBooleanOrFalse(Constants.SessionVariables.ShouldShowPpt)).Return(false);
            _session.Stub(_ => _.GetBooleanOrFalse(Constants.SessionVariables.ShouldPrintOfferAutomatically)).Return(false);

            //act
            var actual = PPTPlus.GetCouponDispositionValue(_session, true, false, false);

            //assert
            Assert.AreEqual(FhirConstants.CouponDisposition.PassivelyNotDistributed, actual);
        }

        [TestMethod, TestCategory("PPT")]
        public void should_return_activelyNotDistributed_if_ppt_and_printAutomatically_preference_true_and_coupon_not_printed()
        {
            //arrange
            _session.Stub(_ => _.GetBooleanOrFalse(Constants.SessionVariables.ShouldShowPpt)).Return(true);
            _session.Stub(_ => _.GetBooleanOrFalse(Constants.SessionVariables.ShouldPrintOfferAutomatically)).Return(true);

            //act
            var actual = PPTPlus.GetCouponDispositionValue(_session, true, false, false);

            //assert
            Assert.AreEqual(FhirConstants.CouponDisposition.ActivelyNotDistributed, actual);
        }


        [TestMethod, TestCategory("PPT")]
        public void should_return_passivelyNotDistributed_if_ppt_preference_false_printAutomatically_preference_true_and_coupon_not_printed()
        {
            //arrange
            _session.Stub(_ => _.GetBooleanOrFalse(Constants.SessionVariables.ShouldShowPpt)).Return(false);
            _session.Stub(_ => _.GetBooleanOrFalse(Constants.SessionVariables.ShouldPrintOfferAutomatically)).Return(true);

            //act
            var actual = PPTPlus.GetCouponDispositionValue(_session, true, false, false);

            //assert
            Assert.AreEqual(FhirConstants.CouponDisposition.PassivelyNotDistributed, actual);
        }

        [TestMethod, TestCategory("PPT")]
        public void should_return_passivelyNotDistributed_if_no_coupon()
        {
            //arrange
            _session.Stub(_ => _.GetBooleanOrFalse(Constants.SessionVariables.ShouldShowPpt)).Return(false);
            _session.Stub(_ => _.GetBooleanOrFalse(Constants.SessionVariables.ShouldPrintOfferAutomatically)).Return(false);

            //act
            var actual = PPTPlus.GetCouponDispositionValue(_session, true, false, false);

            //assert
            Assert.AreEqual(FhirConstants.CouponDisposition.PassivelyNotDistributed, actual);
        }
    }
}
