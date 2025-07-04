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
    public class GetRXCHGApprovalDisplayMessageTests
    {
        [TestMethod]
        public void should_return_empty_string_when_ChangeRxWorkflow_is_null()
        {
            //Arrange
            RxTaskModel changeRx = null;
            //Act
            string actualString = new ChangeRxTask().GetRXCHGApprovalDisplayMessage(changeRx, "Ankit", "Singh");

            //Assert
            Assert.AreEqual(string.Empty, actualString);
        }

        [TestMethod]
        public void should_return_valid_Small_string_when_rx_is_null_And_patientname_null()
        {
            //Arrange
            RxTaskModel changeRx = new RxTaskModel
            {
                ScriptMessage = null,
                ScriptMessageGUID = null,
                RequestedRxIndexSelected = 0,
                RequestedRx = null,
                Rx = null,/*ACTUAL TEST*/
                UserId = new Guid(),
                DbId = ConnectionStringPointer.SHARED_DB,
                LicenseId = new Guid(),
                SiteId = 1,
                ShieldSecurityToken = "Sample ShieldSecurityToken",
                ExternalFacilityCd = "Sample External FacilityID",
                ExternalGroupID = "Sample ExternalGroupID",
                UserType = Constants.UserCategory.GENERAL_USER,
                DelegateProviderId = new Guid()
            };
            //Act
            string actualString = new ChangeRxTask().GetRXCHGApprovalDisplayMessage(changeRx, string.Empty, null);

            //Assert
            Assert.AreEqual("Change Request approved", actualString);
        }

        [TestMethod]
        public void should_return_valid_Custom_string_when_rx_is_not_null_And_internal_variables_vary()
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
                DelegateProviderId = new Guid()
            };
            var rx = changeRx.Rx as Rx;
            rx.MedicationName = "ADDERALL";
            //Act
            string actualString = new ChangeRxTask().GetRXCHGApprovalDisplayMessage(changeRx, "Ankit", "Singh");

            //Assert
            //TC1 Medication name not null
            Assert.AreEqual("Change Request for ADDERALL approved for Ankit Singh", actualString);

            //TC2 Medication Name is null
            rx.MedicationName = string.Empty;
            actualString = new ChangeRxTask().GetRXCHGApprovalDisplayMessage(changeRx, "Ankit", "Singh");
            Assert.AreEqual("Change Request approved for Ankit Singh", actualString);

            //TC3 Medication Name is not null and patient name is null
            rx.MedicationName = "ADDERALL";
            actualString = new ChangeRxTask().GetRXCHGApprovalDisplayMessage(changeRx, null, null);
            Assert.AreEqual("Change Request for ADDERALL approved", actualString);

            //TC4 Medication Name is  null and patient name is null
            rx.MedicationName = null;
            actualString = new ChangeRxTask().GetRXCHGApprovalDisplayMessage(changeRx, null, null);
            Assert.AreEqual("Change Request approved", actualString);
        }
    }
}
