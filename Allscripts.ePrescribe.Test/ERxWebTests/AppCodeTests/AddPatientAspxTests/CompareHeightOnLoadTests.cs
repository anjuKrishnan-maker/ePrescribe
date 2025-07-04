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
    public class CompareHeightOnLoadTests
    {
        [TestMethod]
        public void should_call_audit_method_if_heights_are_different()
        {
            //arrange
            var newHeight = "100.3";
            var heightOnLoad = "135";
            var epsMock = MockRepository.GenerateMock<IEPSBroker>();
            epsMock.Expect(x => x.AuditLogPatientInsert(AuditAction.PATIENT_HEIGHT_MODIFIED, null, null, null, null, ConnectionStringPointer.SHARED_DB)).IgnoreArguments();

            var sessionMock = MockRepository.GenerateMock<IStateContainer>();

            //act
            HeightUtil.LogIfModified(newHeight, heightOnLoad, "", "", epsMock, sessionMock);

            //assert
            epsMock.VerifyAllExpectations();
        }

        [TestMethod]
        public void should_not_call_audit_method_if_heights_are_same()
        {
            //arrange
            var newHeight = "205";
            var heightOnLoad = "205";
            var epsMock = MockRepository.GenerateMock<IEPSBroker>();
            epsMock.Stub(x => x.AuditLogPatientInsert(AuditAction.PATIENT_HEIGHT_MODIFIED, null, null, null, null, ConnectionStringPointer.SHARED_DB)).IgnoreArguments();

            var sessionMock = MockRepository.GenerateMock<IStateContainer>();

            //act
            HeightUtil.LogIfModified(newHeight, heightOnLoad, "", "", epsMock, sessionMock);

            //assert
            epsMock.AssertWasNotCalled(x => x.AuditLogPatientInsert(AuditAction.PATIENT_HEIGHT_MODIFIED, null, null, null, null, ConnectionStringPointer.SHARED_DB));
        }
    }
}
