using System;
using Allscripts.Impact.Tasks;
using eRxWeb.AppCode;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using eRxWeb.AppCode.Tasks;

namespace Allscripts.ePrescribe.Test.ERxWebTests.AppCodeTests.ChangeRxTests
{
    [TestClass]
    public class BuildChangeRequestLinkTests
    {
        [TestMethod]
        public void should_create_change_request_as_linkbutton_if_isapproveEnabled_is_true()
        {
            //arrange
            var guid = new Guid("57CF5378-A078-41A2-9B76-E41BCEFE14CD");

            //act
            var result = ChangeRxTask.BuildChangeRequestLink(guid, true, 1);

            //assert
            Assert.AreEqual("<a onclick=\"return SetGridBindRequirment();\" type=\"RXCHG\" href=\"Sig.aspx?reqRx=1&amp;smid=57cf5378-a078-41a2-9b76-e41bcefe14cd&amp;tasktype=RXCHG\" style=\"color:Gray;font-weight:normal;\">Change Request</a>",
                result);
        }

        [TestMethod]
        public void should_create_change_request_as_label_if_isapproveEnabled_is_false()
        {
            //arrange
            var guid = new Guid("57CF5378-A078-41A2-9B76-E41BCEFE14CD");

            //act
            var result = ChangeRxTask.BuildChangeRequestLink(guid, false, 1);

            //assert
            Assert.AreEqual("<span style=\"color:LightGrey;font-weight:normal;text-decoration:underline;\">Change Request</span>",
                result);
        }
    }
}
