using System;
using System.Web.UI.WebControls;
using eRxWeb;
using eRxWeb.AppCode;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Allscripts.ePrescribe.Test.ERxWebTests.AppCodeTests.SigAspxTests
{
    [TestClass]
    public class CheckAndSetControlsForEpcsTests
    {
        [TestMethod]
        public void should_enable_button()
        {
            //arrange
            var canTryEpcs = true;
            var csCode = "3";
            var btnApproveRequest =  new Button();
            var ucMessage = new Controls_Message();

            //act
            SigAspx.CheckAndSetControlsForEpcs(canTryEpcs, csCode, btnApproveRequest, ucMessage);

            //assert
            Assert.IsTrue(btnApproveRequest.Enabled);
        }

        [TestMethod]
        public void should_enable_button1()
        {
            //arrange
            var canTryEpcs = false;
            var csCode = "";
            var btnApproveRequest = new Button();
            var ucMessage = new Controls_Message();

            //act
            SigAspx.CheckAndSetControlsForEpcs(canTryEpcs, csCode, btnApproveRequest, ucMessage);

            //assert
            Assert.IsTrue(btnApproveRequest.Enabled);
        }

        [TestMethod]
        public void should_enable_button2()
        {
            //arrange
            var canTryEpcs = true;
            var csCode = "";
            var btnApproveRequest = new Button();
            var ucMessage = new Controls_Message();

            //act
            SigAspx.CheckAndSetControlsForEpcs(canTryEpcs, csCode, btnApproveRequest, ucMessage);

            //assert
            Assert.IsTrue(btnApproveRequest.Enabled);
        }

        [TestMethod]
        public void should_make_ucMessage_hidden()
        {
            //arrange
            var canTryEpcs = true;
            var csCode = "3";
            var btnApproveRequest = new Button();
            var ucMessage = new Controls_Message();

            //act
            SigAspx.CheckAndSetControlsForEpcs(canTryEpcs, csCode, btnApproveRequest, ucMessage);

            //assert
            Assert.IsFalse(ucMessage.Visible);
        }

        [TestMethod]
        public void should_make_ucMessage_hidden1()
        {
            //arrange
            var canTryEpcs = false;
            var csCode = "";
            var btnApproveRequest = new Button();
            var ucMessage = new Controls_Message();

            //act
            SigAspx.CheckAndSetControlsForEpcs(canTryEpcs, csCode, btnApproveRequest, ucMessage);

            //assert
            Assert.IsFalse(ucMessage.Visible);
        }

        [TestMethod]
        public void should_make_ucMessage_hidden2()
        {
            //arrange
            var canTryEpcs = true;
            var csCode = "";
            var btnApproveRequest = new Button();
            var ucMessage = new Controls_Message();

            //act
            SigAspx.CheckAndSetControlsForEpcs(canTryEpcs, csCode, btnApproveRequest, ucMessage);

            //assert
            Assert.IsFalse(ucMessage.Visible);
        }

        [TestMethod]
        public void should_set_ucMessage()
        {
            //arrange
            var canTryEpcs = false;
            var csCode = "2";
            var btnApproveRequest = new Button();
            var ucMessage = new Controls_Message(true);

            //act
            SigAspx.CheckAndSetControlsForEpcs(canTryEpcs, csCode, btnApproveRequest, ucMessage);

            //assert
            Assert.IsTrue(ucMessage.Visible);
        }
    }
}
