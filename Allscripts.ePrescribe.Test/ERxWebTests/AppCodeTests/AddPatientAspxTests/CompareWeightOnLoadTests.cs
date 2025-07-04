using System;
using eRxWeb.AppCode;
using eRxWeb.AppCode.Interfaces;
using eRxWeb.ePrescribeSvc;
using eRxWeb.State;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using ConnectionStringPointer = Allscripts.ePrescribe.DatabaseSelector.ConnectionStringPointer;

namespace Allscripts.ePrescribe.Test.ERxWebTests.AppCodeTests.AddPatientAspxTests
{
    [TestClass]
    public class CompareWeightOnLoadTests
    {
        [TestMethod]
        public void should_call_audit_method_if_weights_are_different()
        {
            //arrange
            var newWeight = "34.3";
            var weightOnLoad = "35";
            var epsMock = MockRepository.GenerateMock<IEPSBroker>();
            epsMock.Expect(x => x.AuditLogPatientInsert(AuditAction.PATIENT_HEADER_VIEW, null, null, null, null, ConnectionStringPointer.SHARED_DB)).IgnoreArguments();

            var sessionMock = MockRepository.GenerateMock<IStateContainer>();

            //act
            WeightUtil.LogIfModified(newWeight, weightOnLoad, "", "", epsMock, sessionMock);
             
            //assert
            epsMock.VerifyAllExpectations();
        }

        [TestMethod]
        public void should_not_call_audit_method_if_weights_are_same()
        {
            //arrange
            var newWeight = "34.3";
            var weightOnLoad = "34.3";
            var epsMock = MockRepository.GenerateMock<IEPSBroker>();
            epsMock.Stub(x => x.AuditLogPatientInsert(AuditAction.PATIENT_HEADER_VIEW, null, null, null, null, ConnectionStringPointer.SHARED_DB)).IgnoreArguments();

            var sessionMock = MockRepository.GenerateMock<IStateContainer>();

            //act
            WeightUtil.LogIfModified(newWeight, weightOnLoad, "", "", epsMock, sessionMock);

            //assert
            epsMock.AssertWasNotCalled(x => x.AuditLogPatientInsert(AuditAction.PATIENT_HEADER_VIEW, null, null, null, null, ConnectionStringPointer.SHARED_DB));
        }
    }
}
