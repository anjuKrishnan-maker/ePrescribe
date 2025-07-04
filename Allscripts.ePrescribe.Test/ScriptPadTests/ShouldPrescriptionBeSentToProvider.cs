using System;
using System.Collections;
using eRxWeb;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Allscripts.ePrescribe.Test.ScriptPadTests
{
    [TestClass]
    public class ShouldPrescriptionBeSentToProvider
    {
        private ArrayList SetupRxToProviderList()
        {
            var rxToProviderList = new ArrayList();
            rxToProviderList.Add(Guid.Parse("56124999-4B5B-4300-8D5A-33B35CC906A0"));
            rxToProviderList.Add(Guid.Parse("9D6BDD82-B32E-4DAD-A592-200E2DE4C933"));
            rxToProviderList.Add(Guid.Parse("43D191D1-6F71-432C-893D-2B7C699B3780"));

            return rxToProviderList;
        }

        [TestMethod]
        public void should_send_to_provider_when_all_review_is_true()
        {
            bool isAllReview = true;
            ArrayList rxToProviderList = SetupRxToProviderList();
            string rxID = "AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAAA";

            bool expected = true;

            bool isSendToProvider = new ScriptPad().ShouldPrescriptionBeSentToProvider(isAllReview, rxToProviderList, rxID);

            Assert.AreEqual(expected, isSendToProvider);
        }

        [TestMethod]
        public void should_send_to_provider_when_rxid_in_rxToProviderList_and_isAllReview_false()
        {
            bool isAllReview = false;
            ArrayList rxToProviderList = SetupRxToProviderList();
            string rxID = "9D6BDD82-B32E-4DAD-A592-200E2DE4C933";

            bool expected = true;

            bool isSendToProvider = new ScriptPad().ShouldPrescriptionBeSentToProvider(isAllReview, rxToProviderList, rxID);

            Assert.AreEqual(expected, isSendToProvider);
        }

        [TestMethod]
        public void should_send_to_provider_when_rxid_in_rxToProviderList_and_isAllReview_true()
        {
            bool isAllReview = true;
            ArrayList rxToProviderList = SetupRxToProviderList();
            string rxID = "9D6BDD82-B32E-4DAD-A592-200E2DE4C933";

            bool expected = true;

            bool isSendToProvider = new ScriptPad().ShouldPrescriptionBeSentToProvider(isAllReview, rxToProviderList, rxID);

            Assert.AreEqual(expected, isSendToProvider);
        }

        [TestMethod]
        public void should_not_send_to_provider_when_rxid_isNotIn_rxToProviderList_and_isAllReview_false()
        {
            bool isAllReview = false;
            ArrayList rxToProviderList = SetupRxToProviderList();
            string rxID = "AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAAA";

            bool expected = false;

            bool isSendToProvider = new ScriptPad().ShouldPrescriptionBeSentToProvider(isAllReview, rxToProviderList, rxID);

            Assert.AreEqual(expected, isSendToProvider);
        }
    }
}
