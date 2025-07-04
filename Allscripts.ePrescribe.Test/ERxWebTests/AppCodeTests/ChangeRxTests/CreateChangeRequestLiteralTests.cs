using System;
using Allscripts.Impact.Tasks;
using eRxWeb.AppCode;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using eRxWeb.AppCode.Tasks;

namespace Allscripts.ePrescribe.Test.ERxWebTests.AppCodeTests.ChangeRxTests
{
    [TestClass]
    public class CreateChangeRequestLiteralTests
    {
        [TestMethod]
        public void should_create_change_request_label()
        {
            //act
            var result = ChangeRxTask.CreateChangeRequestLabel();

            //assert
            Assert.AreEqual("<span style=\"color:LightGrey;font-weight:normal;text-decoration:underline;\">Change Request</span>", result);
        }
    }
}
