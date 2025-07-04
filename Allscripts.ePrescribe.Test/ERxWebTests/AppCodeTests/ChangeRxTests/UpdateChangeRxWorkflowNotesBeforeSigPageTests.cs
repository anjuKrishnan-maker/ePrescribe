using System;
using Allscripts.Impact;
using Allscripts.Impact.Interfaces;
using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.Objects;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Allscripts.ePrescribe.DatabaseSelector;
using Allscripts.Impact.Tasks;
using eRxWeb.AppCode;
using Rhino.Mocks;
using Rx = Allscripts.Impact.Rx;
using ScriptMessage = Allscripts.Impact.ScriptMsg.ScriptMessage;
using eRxWeb.AppCode.Tasks;

namespace Allscripts.ePrescribe.Test.ERxWebTests.AppCodeTests.ChangeRxTests
{
    [TestClass]
    public class UpdateChangeRxWorkflowNotesBeforeSigPageTests
    {
        [TestMethod]
        public void should_Update_RxNotes_From_PhysicianComments()
        {
            //Arrange
            RxTaskModel changeRx = new RxTaskModel
            {
                ScriptMessage = null,
                ScriptMessageGUID = null,
                RequestedRxIndexSelected = 0,
                RequestedRx = null,
                Rx = new Rx(),
                UserId = new Guid(),
                DbId = ConnectionStringPointer.SHARED_DB,
                LicenseId = new Guid(),
                SiteId = 1,
                ShieldSecurityToken = "Sample ShieldSecurityToken",
                ExternalFacilityCd = "Sample External FacilityID",
                ExternalGroupID = "Sample ExternalGroupID",
                UserType = Constants.UserCategory.GENERAL_USER,
                DelegateProviderId = new Guid(),
                PhysicianComments = "TEST NOTES"
            };
            //Act
            new ChangeRxTask(changeRx).UpdateChangeRxWorkflowNotesBeforeSigPage();

            //Assert
            var rx = changeRx.Rx as Rx;
            Assert.AreEqual("TEST NOTES", rx.Notes );
        }
    }
}
