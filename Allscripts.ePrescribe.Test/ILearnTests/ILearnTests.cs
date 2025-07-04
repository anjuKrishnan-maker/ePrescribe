using Allscripts.Impact.Ilearn;
using eRxWeb.State;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Allscripts.ePrescribe.Test.ILearnTests
{
    [TestClass]
    public class ILearnTests
    {
        [TestClass]
        public class GetILearnVideoNotificationTests
        {
            [TestMethod]
            public void should_return_a_jsonFormatString_with_numberOfVideos_parameter()
            {
                //arrange
                var pageState = MockRepository.GenerateStub<IStateContainer>();
                pageState.Stub(x => x.GetStringOrEmpty("UserCode")).Return("eRxUser1");
                pageState.Stub(x => x.GetStringOrEmpty("LicenseID")).Return("97314e2c-bba9-4430-a178-acba4cc0f21d");

                var ilcm = MockRepository.GenerateStub<IiLearnConfigurationManager>();
                ilcm.Stub(x => x.BaseURL).Return("");

                var ilc = MockRepository.GenerateStub<IiLearnClient>();
                ilc.Stub(x => x.GetServiceResponse("/api/index.php/notifications/?UserName=eRxUser1&ClientId=97314e2c-bba9-4430-a178-acba4cc0f21d"))
                            .Return("{\"notificationId\":0,\"notificationTitle\":\"No Notifications\",\"numberOfVideos\":0,\"versionRelease\":0}");
                var ilsm = new ILearnServiceManager(ilc);

                //act
                string result = eRxWeb.Controller.ILearnAPIController.GetILearnVideoNotification(pageState, ilsm);

                //assert         
                string expected = "{\"notificationId\":0,\"notificationTitle\":\"No Notifications\",\"numberOfVideos\":0,\"versionRelease\":0}";       
                Assert.AreEqual(expected, result);

            }
        }
    }
}
