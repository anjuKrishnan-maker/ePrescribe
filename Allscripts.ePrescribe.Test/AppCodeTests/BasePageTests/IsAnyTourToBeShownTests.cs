using Allscripts.ePrescribe.Common;
using eRxWeb.ePrescribeSvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Allscripts.ePrescribe.Common.Constants;

namespace Allscripts.ePrescribe.Test.AppCodeTests.BasePageTests
{
    [TestClass]
    public class IsAnyTourToBeShownTests
    {
        [TestMethod]
        public void should_return_true_when_newUserTourEnabled_and_newuserack_notfound()
        {
            //arrange
            string newUserPath = "/some/path/index.html";
            string newReleasePath = "/someother/path/index.html";
            WelcomeTourNewUser showNewUserTour = WelcomeTourNewUser.ON;
            WelcomeTourNewRelease showNewReleaseTour = WelcomeTourNewRelease.OFF;
            string tourPath;
            int tourType;

            GetMessageTrackingAcksResponse acks = new GetMessageTrackingAcksResponse();
            acks.UserMessageTrackingAckList = new UserMessageTrackingAck[0];            

            //act
            bool result = eRxWeb.BasePage.IsAnyTourToBeShown(acks, newUserPath, newReleasePath, showNewUserTour, showNewReleaseTour, out tourPath, out tourType);

            //assert
            bool expected = true;
            Assert.AreEqual(expected, result);
            Assert.AreEqual(newUserPath, tourPath);
            Assert.AreEqual((int)Constants.WelcomeTourType.NewUser, tourType);
        }

        [TestMethod]
        public void should_return_true_when_newReleaseTourEnabled_and_newreleaseack_notfound()
        {
            //arrange
            string newUserPath = "/some/path/index.html";
            string newReleasePath = "/someother/path/index.html";
            WelcomeTourNewUser showNewUserTour = WelcomeTourNewUser.ON;
            WelcomeTourNewRelease showNewReleaseTour = WelcomeTourNewRelease.ON;
            string tourPath;
            int tourType;

            GetMessageTrackingAcksResponse acks = new GetMessageTrackingAcksResponse();
            acks.UserMessageTrackingAckList = new UserMessageTrackingAck[1];
            acks.UserMessageTrackingAckList[0] = new UserMessageTrackingAck();
            acks.UserMessageTrackingAckList[0].ConfigKey = new Guid("d7687d09-07ea-458d-8546-97d6c195f89d");    //New User Ack not New Release

            //act
            bool result = eRxWeb.BasePage.IsAnyTourToBeShown(acks, newUserPath, newReleasePath, showNewUserTour, showNewReleaseTour, out tourPath, out tourType);

            //assert
            bool expected = true;
            Assert.AreEqual(expected, result);
            Assert.AreEqual(newReleasePath, tourPath);  //Path is newRelease as new user ack already exists
            Assert.AreEqual((int)Constants.WelcomeTourType.NewRelease, tourType);
        }

        [TestMethod]
        public void should_return_false_when_both_newUserTour_and_newReleaseTour_disabled()
        {
            //arrange
            string newUserPath = "";
            string newReleasePath = " ";
            WelcomeTourNewUser showNewUserTour = WelcomeTourNewUser.OFF;
            WelcomeTourNewRelease showNewReleaseTour = WelcomeTourNewRelease.OFF;
            string tourPath;
            int tourType;

            GetMessageTrackingAcksResponse acks = new GetMessageTrackingAcksResponse();
            acks.UserMessageTrackingAckList = new UserMessageTrackingAck[0];            

            //act
            bool result = eRxWeb.BasePage.IsAnyTourToBeShown(acks, newUserPath, newReleasePath, showNewUserTour, showNewReleaseTour, out tourPath, out tourType);

            //assert
            bool expected = false;
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void should_return_false_when_tours_enabled_and_acks_exists()
        {
            //arrange
            string newUserPath = "/some/path/index.html";
            string newReleasePath = "/someother/path/index.html";
            WelcomeTourNewUser showNewUserTour = WelcomeTourNewUser.ON;
            WelcomeTourNewRelease showNewReleaseTour = WelcomeTourNewRelease.ON;
            string tourPath;
            int tourType;

            GetMessageTrackingAcksResponse acks = new GetMessageTrackingAcksResponse();
            acks.UserMessageTrackingAckList = new UserMessageTrackingAck[2];
            acks.UserMessageTrackingAckList[0] = new UserMessageTrackingAck();
            acks.UserMessageTrackingAckList[0].ConfigKey = new Guid("d7687d09-07ea-458d-8546-97d6c195f89d");    //New User Ack
            acks.UserMessageTrackingAckList[1] = new UserMessageTrackingAck();
            acks.UserMessageTrackingAckList[1].ConfigKey = new Guid("7593cd82-5b81-4ede-a80e-05a6223f2cc4");    //New Release Ack

            //act
            bool result = eRxWeb.BasePage.IsAnyTourToBeShown(acks, newUserPath, newReleasePath, showNewUserTour, showNewReleaseTour, out tourPath, out tourType);

            //assert
            bool expected = false;
            Assert.AreEqual(expected, result);
        }
    }
}
