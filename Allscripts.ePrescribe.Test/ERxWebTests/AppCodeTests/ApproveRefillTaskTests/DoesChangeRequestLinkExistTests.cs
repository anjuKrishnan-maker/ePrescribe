using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Allscripts.ePrescribe.Common;
using eRxWeb.AppCode;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Allscripts.ePrescribe.Test.ERxWebTests.AppCodeTests.ApproveRefillTaskTests
{
    [TestClass]
    public class DoesChangeRequestLinkExistTests
    {
        [TestMethod]
        public void should_return_false_if_control_collection_is_null()
        {
            //arrange

            //act
            var approveRefillTask = new ApproveRefillTask();
            var result = approveRefillTask.DoesChangeRequestLinkExist(null);

            //assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void should_return_false_if_control_collection_is_empty()
        {
            //arrange
            var controls = new ControlCollection(new Control());

            //act
            var approveRefillTask = new ApproveRefillTask();
            var result = approveRefillTask.DoesChangeRequestLinkExist(controls);

            //assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void should_return_false_if_control_collection_does_not_contain_link_buttons()
        {
            //arrange
            var controls = new ControlCollection(new Control());
            var textbox = new TextBox();

            controls.Add(textbox);

            //act
            var approveRefillTask = new ApproveRefillTask();
            var result = approveRefillTask.DoesChangeRequestLinkExist(controls);

            //assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void should_return_false_if_control_collection_does_not_contain_change_request_or_change_request_no_break()
        {
            //arrange
            var controls = new ControlCollection(new Control());
            var linkButton = new LinkButton { Text = Constants.ActionButtonText.CHANGE_PATIENT};
            controls.Add(linkButton);

            //act
            var approveRefillTask = new ApproveRefillTask();
            var result = approveRefillTask.DoesChangeRequestLinkExist(controls);

            //assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void should_return_true_if_control_collection_does_contain_change_request_link_button()
        {
            //arrange
            var controls = new ControlCollection(new Control());
            var linkButton = new LinkButton { Text = Constants.ActionButtonText.CHANGE_REQUEST };
            controls.Add(linkButton);

            //act
            var approveRefillTask = new ApproveRefillTask();
            var result = approveRefillTask.DoesChangeRequestLinkExist(controls);

            //assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void should_return_true_if_control_collection_does_contain_change_request_no_break_link_button()
        {
            //arrange
            var controls = new ControlCollection(new Control());
            var linkButton = new LinkButton { Text = Constants.ActionButtonText.CHANGE_REQUEST_NO_BREAK };
            controls.Add(linkButton);

            //act
            var approveRefillTask = new ApproveRefillTask();
            var result = approveRefillTask.DoesChangeRequestLinkExist(controls);

            //assert
            Assert.IsTrue(result);
        }
    }
}
