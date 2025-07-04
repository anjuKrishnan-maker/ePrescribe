using System;
using eRxWeb.AppCode;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Allscripts.ePrescribe.Test.ERxWebTests.AppCodeTests.SsoTests
{
    [TestClass]
    public class IsEpaTaskModeTests
    {
        [TestMethod]
        public void should_return_true_if_is_epa_task_mode()
        {
            //act
            var result = Sso.IsEpaTaskMode("epapatientlockdowntaskmode");

            //assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void should_return_false_if_it_is_not_epa_task_mode()
        {
            //act
            var result = Sso.IsEpaTaskMode("asdf");

            //assert
            Assert.IsFalse(result);
        }
    }
}
