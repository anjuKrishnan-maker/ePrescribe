using System;
using Allscripts.Impact.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Allscripts.ePrescribe.Test.ERxWebTests.AppCodeTests.ApproveRefillTaskTests
{
    [TestClass]
    public class IsValidPaCodeTests
    {
        [TestMethod]
        public void should_return_true_if_text_doesnt_contain_invalid_chars()
        {
            //arrange
            var authCode = "kkdiisnnckskdd8832937728./";

            //act
            var result = ChangeRxTask.IsValidPaCode(authCode);

            //assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void should_return_false_if_text_contains_invalid_chars()
        {
            //arrange
            var authCode = "kkdiisnnckskdd88329%^#&*(@)";

            //act
            var result = ChangeRxTask.IsValidPaCode(authCode);

            //assert
            Assert.IsFalse(result);
        }
    }
}
