using System;
using System.Data;
using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.DatabaseSelector;
using Allscripts.Impact;
using Allscripts.Impact.Interfaces;
using Allscripts.Impact.Tasks.Interfaces;
using eRxWeb.AppCode;
using eRxWeb.AppCode.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;

namespace Allscripts.ePrescribe.Test.ERxWebTests.AppCodeTests.ApproveRefillTaskTests
{
    [TestClass]
    public class GetDenyMessageTests
    {
        [TestMethod]
        public void should_get_patient_name_from_patDr_if_it_is_not_null()
        {
            //arrange
            var pharmTask = new RxTaskModel {DenialText = "Because I didn't want to"};

            var dt = new DataTable();
            dt.Columns.Add("PatientName");
            var patDr = dt.NewRow();
            patDr["Patientname"] = "Ethan O'Brien";

            var approveRefillMock = MockRepository.GenerateMock<IApproveRefillTask>();
            approveRefillMock.Stub(x => x.GetDenyMessageTypeText(Constants.PrescriptionTaskType.APPROVAL_REQUEST)).Return("Change Rx");

            var scriptMessagemock = MockRepository.GenerateMock<IScriptMessage>();

            //act
            var result = ApproveRefillTask.GetDenyMessage(pharmTask, patDr, Guid.Empty, Guid.Empty, approveRefillMock, scriptMessagemock, ConnectionStringPointer.AUDIT_ERXDB_SERVER_1);

            //assert
            Assert.AreEqual("Change Rx denied (Because I didn't want to) for Ethan O'Brien.", result);
        }

        [TestMethod]
        public void should_get_patient_name_from_GetPatientName_method_if_patDr_is_null()
        {
            //arrange
            var pharmTask = new RxTaskModel
            {
                DenialText = "Because I didn't want to",
                ScriptMessageGUID = "62CCB711-3FE6-4828-BE64-1157D8FF08F5"
            };
            
            var approveRefillMock = MockRepository.GenerateMock<IApproveRefillTask>();
            approveRefillMock.Stub(x => x.GetDenyMessageTypeText(Constants.PrescriptionTaskType.APPROVAL_REQUEST)).Return("Change Rx");

            var scriptMessagemock = MockRepository.GenerateMock<IScriptMessage>();
            scriptMessagemock.Stub(x => x.GetPatientName(new Guid(), new Guid(), new Guid(), ConnectionStringPointer.SHARED_DB)).IgnoreArguments().Return("Ethan O'Brien");
            DataRow patdr = null;
            //act
            var result = ApproveRefillTask.GetDenyMessage(pharmTask, patdr, Guid.Empty, Guid.Empty, approveRefillMock, scriptMessagemock, ConnectionStringPointer.AUDIT_ERXDB_SERVER_1);
            //assert
            Assert.AreEqual("Change Rx denied (Because I didn't want to) for Ethan O'Brien.", result);
        }
    }
}
