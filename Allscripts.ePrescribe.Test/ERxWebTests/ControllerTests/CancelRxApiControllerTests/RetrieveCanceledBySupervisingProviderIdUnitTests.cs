using System;
using Allscripts.ePrescribe.Common;
using eRxWeb.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Allscripts.ePrescribe.Test.ERxWebTests.ControllerTests.CancelRxApiControllerTests
{
    [TestClass]
    public class RetrieveCanceledBySupervisingProviderIdUnitTests
    {
        [TestMethod]
        //App incorrectly sets the supervising provider Id as delegate provider I but based
        //on discussion with team in SCRUM, suggestion is to document this instead of fixing the 
        //session supervising provider id
        public void should_return_Delegate_Provider_Id_If_Logged_In_User_Is_Pa_With_Supervision()
        {
            //Arrange
            Guid delegateProviderGuid = new Guid();
            Guid supervisingProviderGuid = new Guid();


            //Act
            Guid canceledBySupervisingProviderId = CancelRxApiController.RetrieveCanceledBySupervisingProviderId(Constants.UserCategory.PHYSICIAN_ASSISTANT_SUPERVISED, delegateProviderGuid, supervisingProviderGuid);

            //Assert
            Assert.AreEqual(canceledBySupervisingProviderId, delegateProviderGuid);
        }

        [TestMethod]
        public void should_return_Supervising_Provider_Id_If_Logged_In_User_Is_POB()
        {
            //Arrange
            Guid delegateProviderGuid = new Guid();
            Guid supervisingProviderGuid = new Guid();


            //Act
            Guid canceledBySupervisingProviderId = CancelRxApiController.RetrieveCanceledBySupervisingProviderId(Constants.UserCategory.POB_SUPER, delegateProviderGuid, supervisingProviderGuid);

            //Assert
            Assert.AreEqual(canceledBySupervisingProviderId, supervisingProviderGuid);
        }
    }
}
