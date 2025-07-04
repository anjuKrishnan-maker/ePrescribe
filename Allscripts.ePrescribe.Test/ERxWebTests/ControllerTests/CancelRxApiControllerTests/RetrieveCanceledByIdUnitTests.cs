using System;
using System.Web;
using System.Web.SessionState;
using Allscripts.ePrescribe.Common;
using eRxWeb.Controllers;
using eRxWeb.State;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Allscripts.ePrescribe.Test.ERxWebTests.ControllerTests.CancelRxApiControllerTests
{
    [TestClass]
    public class RetrieveCanceledByIdUnitTests
    {
        [TestMethod]
        public void should_always_return_Id_Of_Logged_In_User()
        {
            //Arrange
            Guid loggedInUserGuid = new Guid();


            //Act
            Guid canceledByProviderId = CancelRxApiController.RetrieveCanceledById(loggedInUserGuid);

            //Assert
            Assert.AreEqual(canceledByProviderId, loggedInUserGuid);
        }
    }
}
