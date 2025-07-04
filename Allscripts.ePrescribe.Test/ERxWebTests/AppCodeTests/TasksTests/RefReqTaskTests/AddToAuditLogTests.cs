using System;
using Allscripts.Impact;
using Allscripts.Impact.Interfaces;
using Allscripts.Impact.Tasks;
using eRxWeb.AppCode.Interfaces;
using eRxWeb.AppCode.Tasks;
using eRxWeb.ePrescribeSvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using ConnectionStringPointer = Allscripts.ePrescribe.DatabaseSelector.ConnectionStringPointer;
using Rx = Allscripts.Impact.Rx;
using AuditAction = Allscripts.ePrescribe.Objects.AuditAction;
using AuditLogPatientRxResponse = Allscripts.ePrescribe.Objects.AuditLogPatientRxResponse;

namespace Allscripts.ePrescribe.Test.ERxWebTests.AppCodeTests.TasksTests.RefReqTaskTests
{
    [TestClass]
    public class AddToAuditLogTests
    {
        [TestMethod]
        public void should_call_AuditLogPatientRxInsert()
        {
            //arrange
            var currentTask = new RxTaskModel { Rx = new Rx() };

            var rxResponse = new AuditLogPatientRxResponse();

            var epsBrokerMock = MockRepository.GenerateMock<Impact.Interfaces.IEPSBroker>();
            epsBrokerMock.Stub(x => x.AuditLogPatientRxInsert(AuditAction.PATIENT_HEADER_VIEW, null, null, null, null, null, ConnectionStringPointer.SHARED_DB)).IgnoreArguments().Return(rxResponse);

            //act
            new AuditLogAgent(epsBrokerMock, new Audit()).AddToAuditLog(currentTask, 0);

            //assert
            epsBrokerMock.AssertWasCalled(x => x.AuditLogPatientRxInsert(
                Arg<AuditAction>.Is.Anything,
                Arg<string>.Is.Anything,
                Arg<string>.Is.Anything,
                Arg<string>.Is.Anything,
                Arg<string>.Is.Anything,
                Arg<string>.Is.Anything,
                Arg<ConnectionStringPointer>.Is.Anything,
                Arg<string>.Is.Anything));
        }

        [TestMethod]
        public void should_not_call_InsertAuditLogPatientServiceTask_if_response_is_success_and_serviceTaskId_is_neg1()
        {
            //arrange
            var currentTask = new RxTaskModel { Rx = new Rx() };

            var rxResponse = new AuditLogPatientRxResponse{ IsSuccess = true};

            var epsBrokerMock = MockRepository.GenerateMock<Impact.Interfaces.IEPSBroker>();
            epsBrokerMock.Stub(x => x.AuditLogPatientRxInsert(AuditAction.PATIENT_HEADER_VIEW, null, null, null, null, null, ConnectionStringPointer.SHARED_DB)).IgnoreArguments().Return(rxResponse);

            var auditMock = MockRepository.GenerateMock<IAudit>();
            auditMock.Stub(x => x.InsertAuditLogPatientServiceTask(0, null, ConnectionStringPointer.SHARED_DB)).IgnoreArguments();

            //act
            new AuditLogAgent(epsBrokerMock, auditMock).AddToAuditLog(currentTask, -1);

            //assert
            auditMock.AssertWasNotCalled(x => x.InsertAuditLogPatientServiceTask(
                Arg<long>.Is.Anything,
                Arg<string>.Is.Anything,
                Arg<ConnectionStringPointer>.Is.Anything));
        }

        [TestMethod]
        public void should_not_call_InsertAuditLogPatientServiceTask_if_response_is_success_false_and_serviceTaskId_is_not_neg1()
        {
            //arrange
            var currentTask = new RxTaskModel { Rx = new Rx() };

            var rxResponse = new AuditLogPatientRxResponse { IsSuccess = false };

            var epsBrokerMock = MockRepository.GenerateMock<Impact.Interfaces.IEPSBroker>();
            epsBrokerMock.Stub(x => x.AuditLogPatientRxInsert(AuditAction.PATIENT_HEADER_VIEW, null, null, null, null, null, ConnectionStringPointer.SHARED_DB)).IgnoreArguments().Return(rxResponse);

            var auditMock = MockRepository.GenerateMock<IAudit>();
            auditMock.Stub(x => x.InsertAuditLogPatientServiceTask(0, null, ConnectionStringPointer.SHARED_DB)).IgnoreArguments();

            //act
            new AuditLogAgent(epsBrokerMock, auditMock).AddToAuditLog(currentTask, 1234);

            //assert
            auditMock.AssertWasNotCalled(x => x.InsertAuditLogPatientServiceTask(
                Arg<long>.Is.Anything,
                Arg<string>.Is.Anything,
                Arg<ConnectionStringPointer>.Is.Anything));
        }

        [TestMethod]
        public void should_call_InsertAuditLogPatientServiceTask_if_response_is_success_and_serviceTaskId_is_not_neg1()
        {
            //arrange
            var currentTask = new RxTaskModel { Rx = new Rx() };

            var rxResponse = new AuditLogPatientRxResponse { IsSuccess = true };

            var epsBrokerMock = MockRepository.GenerateMock<Impact.Interfaces.IEPSBroker>();
            epsBrokerMock.Stub(x => x.AuditLogPatientRxInsert(AuditAction.PATIENT_HEADER_VIEW, null, null, null, null, null, ConnectionStringPointer.SHARED_DB)).IgnoreArguments().Return(rxResponse);

            var auditMock = MockRepository.GenerateMock<IAudit>();
            auditMock.Stub(x => x.InsertAuditLogPatientServiceTask(0, null, ConnectionStringPointer.SHARED_DB)).IgnoreArguments();

            //act
            new AuditLogAgent(epsBrokerMock, auditMock).AddToAuditLog(currentTask, 123);

            //assert
            auditMock.AssertWasCalled(x => x.InsertAuditLogPatientServiceTask(
                Arg<long>.Is.Anything,
                Arg<string>.Is.Anything,
                Arg<ConnectionStringPointer>.Is.Anything));
        }
    }
}
