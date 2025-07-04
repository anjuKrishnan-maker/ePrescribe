using System;
using System.Collections.Generic;
using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.Data;
using Allscripts.ePrescribe.DatabaseSelector;
using eRxWeb.Controllers;
using eRxWeb.State;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;

namespace Allscripts.ePrescribe.Test.ERxWebTests.ControllerTests.PatientHeaderAPIControllerTests
{
    [TestClass]
    public class GetDiagnosisInfoTests
    {
        [TestMethod]
        public void should_set_dx_to_none_entered_if_session_is_null()
        {
            //arrange
            var sessionMock = MockRepository.GenerateMock<IStateContainer>();
            sessionMock.Stub(_ => _.GetStringOrEmpty(Constants.SessionVariables.ActiveDx)).Return(null);

            var dataMock = MockRepository.GenerateMock<IPatientDiagnosisProvider>();
            
            //act
            var result = PatientHeaderAPIController.GetDiagnosisInfo(sessionMock, dataMock);

            //assert
            Assert.AreEqual("None entered", result.Dx);
        }

        [TestMethod]
        public void should_set_dx_session_if_less_than_limit()
        {
            //arrange
            var dx = "Something small";
            var sessionMock = MockRepository.GenerateMock<IStateContainer>();
            sessionMock.Stub(_ => _.GetStringOrEmpty(Constants.SessionVariables.ActiveDx)).Return(dx);

            var dataMock = MockRepository.GenerateMock<IPatientDiagnosisProvider>();

            //act
            var result = PatientHeaderAPIController.GetDiagnosisInfo(sessionMock, dataMock);

            //assert
            Assert.AreEqual(dx, result.Dx);
        }

        [TestMethod]
        public void should_set_dx_substring_if_over_limit()
        {
            //arrange
            var dx = "Something largeSasfdddlaksjdflkajsdfuiekljasdkjfh83928239283klajhsdfjaklsdjf;laksdjfi490823904uoijkal;skdjf";
            var sessionMock = MockRepository.GenerateMock<IStateContainer>();
            sessionMock.Stub(_ => _.GetStringOrEmpty(Constants.SessionVariables.ActiveDx)).Return(dx);

            var dataMock = MockRepository.GenerateMock<IPatientDiagnosisProvider>();
            dataMock.Stub(_ => _.GetActiveDiagnosis(null, ConnectionStringPointer.SHARED_DB)).IgnoreArguments().Return(new List<PatientDiagnosis> { new PatientDiagnosis { Description = "FoundMe" } });

            //act
            var result = PatientHeaderAPIController.GetDiagnosisInfo(sessionMock, dataMock);

            //assert
            Assert.AreEqual(dx.Substring(0, 100), result.Dx);
        }

        [TestMethod]
        public void should_set_more_diag_to_true()
        {
            //arrange
            var dx = "Something largeSasfdddlaksjdflkajsdfuiekljasdkjfh83928239283klajhsdfjaklsdjf;laksdjfi490823904uoijkal;skdjf";
            var sessionMock = MockRepository.GenerateMock<IStateContainer>();
            sessionMock.Stub(_ => _.GetStringOrEmpty(Constants.SessionVariables.ActiveDx)).Return(dx);

            var dataMock = MockRepository.GenerateMock<IPatientDiagnosisProvider>();
            dataMock.Stub(_ => _.GetActiveDiagnosis(null, ConnectionStringPointer.SHARED_DB)).IgnoreArguments().Return(new List<PatientDiagnosis> { new PatientDiagnosis { Description = "FoundMe" } });

            //act
            var result = PatientHeaderAPIController.GetDiagnosisInfo(sessionMock, dataMock);

            //assert
            Assert.IsTrue(result.MoreActiveProblem);
        }


        [TestMethod]
        public void should_set_active_diagnosis_to_data_returned_from_db()
        {
            //arrange
            var dx = "Something largeSasfdddlaksjdflkajsdfuiekljasdkjfh83928239283klajhsdfjaklsdjf;laksdjfi490823904uoijkal;skdjf";
            var sessionMock = MockRepository.GenerateMock<IStateContainer>();
            sessionMock.Stub(_ => _.GetStringOrEmpty(Constants.SessionVariables.ActiveDx)).Return(dx);

            var dataMock = MockRepository.GenerateMock<IPatientDiagnosisProvider>();
            dataMock.Stub(_ => _.GetActiveDiagnosis(null, ConnectionStringPointer.SHARED_DB)).IgnoreArguments().Return(new List<PatientDiagnosis>{new PatientDiagnosis{Description = "FoundMe"}});

            //act
            var result = PatientHeaderAPIController.GetDiagnosisInfo(sessionMock, dataMock);

            //assert
            Assert.AreEqual("FoundMe", result.ActiveDiagnosis[0].Diagnosis);
        }
    }
}
