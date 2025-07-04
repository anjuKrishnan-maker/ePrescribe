using System;
using System.Collections.Generic;
using eRxWeb;
using eRxWeb.AppCode;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Allscripts.ePrescribe.Test.ERxWebTests.AppCodeTests.CancelRxTests
{
    [TestClass]
    public class GetScriptsToCancelTests
    {
        [TestMethod]
        public void should_return_empty_list_if_cancelRxEligibleList_is_null()
        {
            //arrange
            var completedScritps = new List<string> {"One"};

            //act
            var result = CancelRx.GetScriptsToCancel(completedScritps, null);

            //assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void should_return_empty_list_if_cancelRxEligibleList_is_empty()
        {
            //arrange
            var completedScritps = new List<string> { "One" };

            //act
            var result = CancelRx.GetScriptsToCancel(completedScritps, new List<CancelRxEligibleScript>());

            //assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void should_return_empty_list_if_completedScripts_is_null()
        {
            //arrange
            var cancelRxEligibleList = new List<CancelRxEligibleScript>
            {
                new CancelRxEligibleScript (Guid.NewGuid(), Guid.NewGuid(), "")
            };

            //act
            var result = CancelRx.GetScriptsToCancel(null, cancelRxEligibleList);

            //assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void should_return_empty_list_if_completedScripts_is_empty()
        {
            //arrange
            var cancelRxEligibleList = new List<CancelRxEligibleScript>
            {
                new CancelRxEligibleScript (Guid.NewGuid(),Guid.NewGuid(), "")
            };

            //act
            var result = CancelRx.GetScriptsToCancel(new List<string>(), cancelRxEligibleList);

            //assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void should_not_add_script_to_dictionary_if_it_is_not_contained_in_eligible_scripts()
        {
            //arrange
            var eligibleRxId = Guid.NewGuid();
            var nonEligibleRxId = Guid.NewGuid().ToString();

            var cancelRxEligibleList = new List<CancelRxEligibleScript>
            {
                new CancelRxEligibleScript (eligibleRxId, Guid.NewGuid(), "")
            };

            var completedScripts = new List<string> {nonEligibleRxId};
            //act
            var result = CancelRx.GetScriptsToCancel(completedScripts, cancelRxEligibleList);

            //assert
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void should_add_script_to_dictionary_if_it_is_contained_in_eligible_scripts()
        {
            //arrange
            var eligibleRxId = Guid.NewGuid();

            var cancelRxEligibleList = new List<CancelRxEligibleScript>
            {
                new CancelRxEligibleScript (eligibleRxId, Guid.NewGuid(), "")
            };

            var completedScripts = new List<string> { eligibleRxId.ToString() };
            //act
            var result = CancelRx.GetScriptsToCancel(completedScripts, cancelRxEligibleList);

            //assert
            Assert.AreEqual(1, result.Count);
        }
    }
}
