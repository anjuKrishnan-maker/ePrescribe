using System;
using Allscripts.ePrescribe.Common;
using eRxWeb.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Allscripts.ePrescribe.Test.ERxWebTests.ControllerTests.CancelRxApiControllerTests
{
    [TestClass]
    public class RetrieveCanceledByProviderIdUnitTests
    {
        [TestMethod]
        public void should_return_Id_of_logged_in_user_if_logged_in_user_is_Provider()
        {
            //Arrange
            Guid loggedInUserGuid = new Guid();
            Guid delegateProviderGuid = new Guid();


            //Act
            Guid canceledByProviderId = CancelRxApiController.RetrieveCanceledByProviderId(Constants.UserCategory.PROVIDER, loggedInUserGuid, delegateProviderGuid);

            //Assert
            Assert.AreEqual(canceledByProviderId, loggedInUserGuid);
        }

        [TestMethod]
        public void should_return_Id_of_logged_in_user_if_logged_in_user_is_PA_No_Supervision()
        {
            //Arrange
            Guid loggedInUserGuid = new Guid();
            Guid delegateProviderGuid = new Guid();


            //Act
            Guid canceledByProviderId = CancelRxApiController.RetrieveCanceledByProviderId(Constants.UserCategory.PHYSICIAN_ASSISTANT, loggedInUserGuid, delegateProviderGuid);

            //Assert
            Assert.AreEqual(canceledByProviderId, loggedInUserGuid);
        }

        [TestMethod]
        public void should_return_Id_of_logged_in_user_if_logged_in_user_is_PA_With_Supervision()
        {
            //Arrange
            Guid loggedInUserGuid = new Guid();
            Guid delegateProviderGuid = new Guid();


            //Act
            Guid canceledByProviderId = CancelRxApiController.RetrieveCanceledByProviderId(Constants.UserCategory.PHYSICIAN_ASSISTANT_SUPERVISED, loggedInUserGuid, delegateProviderGuid);

            //Assert
            Assert.AreEqual(canceledByProviderId, loggedInUserGuid);
        }

        [TestMethod]
        public void should_return_Id_of_delegate_Provider_user_if_logged_in_user_POB_Limited()
        {
            //Arrange
            Guid loggedInUserGuid = new Guid();
            Guid delegateProviderGuid = new Guid();


            //Act
            Guid canceledByProviderId = CancelRxApiController.RetrieveCanceledByProviderId(Constants.UserCategory.POB_LIMITED, loggedInUserGuid, delegateProviderGuid);

            //Assert
            Assert.AreEqual(canceledByProviderId, delegateProviderGuid);
        }

        [TestMethod]
        public void should_return_Id_of_delegate_Provider_user_if_logged_in_user_POB_Super()
        {
            //Arrange
            Guid loggedInUserGuid = new Guid();
            Guid delegateProviderGuid = new Guid();


            //Act
            Guid canceledByProviderId = CancelRxApiController.RetrieveCanceledByProviderId(Constants.UserCategory.POB_SUPER, loggedInUserGuid, delegateProviderGuid);

            //Assert
            Assert.AreEqual(canceledByProviderId, delegateProviderGuid);
        }

        [TestMethod]
        public void should_return_Id_of_delegate_Provider_user_if_logged_in_user_POB_Regular()
        {
            //Arrange
            Guid loggedInUserGuid = new Guid();
            Guid delegateProviderGuid = new Guid();


            //Act
            Guid canceledByProviderId = CancelRxApiController.RetrieveCanceledByProviderId(Constants.UserCategory.POB_REGULAR, loggedInUserGuid, delegateProviderGuid);

            //Assert
            Assert.AreEqual(canceledByProviderId, delegateProviderGuid);
        }

        [TestMethod]
        public void should_return_null_if_logged_in_user_General_User()
        {
            //Arrange
            Guid loggedInUserGuid = new Guid();
            Guid delegateProviderGuid = Guid.Empty;


            //Act
            Guid canceledByProviderId = CancelRxApiController.RetrieveCanceledByProviderId(Constants.UserCategory.GENERAL_USER, loggedInUserGuid, delegateProviderGuid);

            //Assert
            Assert.AreEqual(canceledByProviderId, Guid.Empty);
        }

    }
}
