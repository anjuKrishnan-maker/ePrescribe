using System;
using System.Data;
using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.DatabaseSelector;
using Allscripts.ePrescribe.ExtensionMethods;
using Allscripts.Impact.Interfaces;
using eRxWeb.Controllers;
using eRxWeb.State;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;

namespace Allscripts.ePrescribe.Test.ERxWebTests.ControllerTests.PatientHeaderAPIControllerTests
{
    [TestClass]
    public class GetPatientActiveMedsTests
    {
        private DataSet MedDs
        {
            get
            {
                var ds = new DataSet();
                var dt = new DataTable();
                dt.Columns.AddRange("Medication", "StartDate");

                var nr = dt.NewRow();
                nr["Medication"] = "NameMe";
                dt.Rows.Add(nr);
                ds.Tables.Add(dt);
                return ds;
            }
        }

        [TestMethod]
        public void should_return_empty_active_med_if_patientID_is_null()
        {
            //arrange
            var sessionMock = MockRepository.GenerateMock<IStateContainer>();
            sessionMock.Stub(_ => _.GetStringOrEmpty(Constants.SessionVariables.ActiveMedications)).Return(null);
            sessionMock.Stub(_ => _.GetBooleanOrFalse(Constants.SessionVariables.PatientNoActiveMed)).Return(false);
            sessionMock.Stub(_ => _.GetStringOrEmpty(Constants.SessionVariables.PatientId)).Return(null);

            var dataMock = MockRepository.GenerateMock<IPatient>();

            //act
            var result = PatientHeaderAPIController.GetPatientActiveMeds(sessionMock, dataMock);

            //assert
            Assert.IsNull(result.ActiveMed);
        }

        [TestMethod]
        public void should_set_med_to_none_entered_if_session_is_null()
        {
            //arrange
            var sessionMock = MockRepository.GenerateMock<IStateContainer>();
            sessionMock.Stub(_ => _.GetStringOrEmpty(Constants.SessionVariables.ActiveMedications)).Return(null);
            sessionMock.Stub(_ => _.GetBooleanOrFalse(Constants.SessionVariables.PatientNoActiveMed)).Return(false);
            sessionMock.Stub(_ => _.GetStringOrEmpty(Constants.SessionVariables.PatientId)).Return("NotNull");

            var dataMock = MockRepository.GenerateMock<IPatient>();

            //act
            var result = PatientHeaderAPIController.GetPatientActiveMeds(sessionMock, dataMock);

            //assert
            Assert.AreEqual("None entered", result.ActiveMed);
        }

        [TestMethod]
        public void should_return_no_active_meds()
        {
            //arrange
            var sessionMock = MockRepository.GenerateMock<IStateContainer>();
            sessionMock.Stub(_ => _.GetStringOrEmpty(Constants.SessionVariables.ActiveMedications)).Return(null);
            sessionMock.Stub(_ => _.GetBooleanOrFalse(Constants.SessionVariables.PatientNoActiveMed)).Return(true);
            sessionMock.Stub(_ => _.GetStringOrEmpty(Constants.SessionVariables.PatientId)).Return("NotNull");

            var dataMock = MockRepository.GenerateMock<IPatient>();

            //act
            var result = PatientHeaderAPIController.GetPatientActiveMeds(sessionMock, dataMock);

            //assert
            Assert.AreEqual("No Active Medications", result.ActiveMed);
        }

        [TestMethod]
        public void should_set_med_session_if_less_than_limit()
        {
            //arrange
            var dx = "Something small";
            var sessionMock = MockRepository.GenerateMock<IStateContainer>();
            sessionMock.Stub(_ => _.GetStringOrEmpty(Constants.SessionVariables.ActiveMedications)).Return(dx);
            sessionMock.Stub(_ => _.GetBooleanOrFalse(Constants.SessionVariables.PatientNoActiveMed)).Return(false);
            sessionMock.Stub(_ => _.GetStringOrEmpty(Constants.SessionVariables.PatientId)).Return("NotNull");

            var dataMock = MockRepository.GenerateMock<IPatient>();

            //act
            var result = PatientHeaderAPIController.GetPatientActiveMeds(sessionMock, dataMock);

            //assert
            Assert.AreEqual(dx, result.ActiveMed);
        }

        [TestMethod]
        public void should_set_med_substring_if_over_limit()
        {
            //arrange
            var dx = "Something largeSasfdddlaksjdflkajsdfuiekljasdkjfh83928239283klajhsdfjaklsdjf;laksdjfi490823904uoijkal;skdjf";
            var sessionMock = MockRepository.GenerateMock<IStateContainer>();
            sessionMock.Stub(_ => _.GetStringOrEmpty(Constants.SessionVariables.ActiveMedications)).Return(dx);
            sessionMock.Stub(_ => _.GetBooleanOrFalse(Constants.SessionVariables.PatientNoActiveMed)).Return(false);
            sessionMock.Stub(_ => _.GetStringOrEmpty(Constants.SessionVariables.PatientId)).Return("NotNull");

            var dataMock = MockRepository.GenerateMock<IPatient>();
            dataMock.Stub(_ => _.GetPatientActiveMedication(null, null, null, ConnectionStringPointer.SHARED_DB)).IgnoreArguments().Return(MedDs);

            //act
            var result = PatientHeaderAPIController.GetPatientActiveMeds(sessionMock, dataMock);

            //assert
            Assert.AreEqual(dx.Substring(0, 100), result.ActiveMed);
        }

        [TestMethod]
        public void should_set_more_med_to_true()
        {
            //arrange
            var dx = "Something largeSasfdddlaksjdflkajsdfuiekljasdkjfh83928239283klajhsdfjaklsdjf;laksdjfi490823904uoijkal;skdjf";
            var sessionMock = MockRepository.GenerateMock<IStateContainer>();
            sessionMock.Stub(_ => _.GetStringOrEmpty(Constants.SessionVariables.ActiveMedications)).Return(dx);
            sessionMock.Stub(_ => _.GetBooleanOrFalse(Constants.SessionVariables.PatientNoActiveMed)).Return(false);
            sessionMock.Stub(_ => _.GetStringOrEmpty(Constants.SessionVariables.PatientId)).Return("NotNull");

            var dataMock = MockRepository.GenerateMock<IPatient>();
            dataMock.Stub(_ => _.GetPatientActiveMedication(null, null, null, ConnectionStringPointer.SHARED_DB)).IgnoreArguments().Return(MedDs);

            //act
            var result = PatientHeaderAPIController.GetPatientActiveMeds(sessionMock, dataMock);

            //assert
            Assert.IsTrue(result.MoreActiveMedVisible);
        }


        [TestMethod]
        public void should_set_active_med_to_data_returned_from_db()
        {
            //arrange
            var dx = "Something largeSasfdddlaksjdflkajsdfuiekljasdkjfh83928239283klajhsdfjaklsdjf;laksdjfi490823904uoijkal;skdjf";
            var sessionMock = MockRepository.GenerateMock<IStateContainer>();
            sessionMock.Stub(_ => _.GetStringOrEmpty(Constants.SessionVariables.ActiveMedications)).Return(dx);
            sessionMock.Stub(_ => _.GetBooleanOrFalse(Constants.SessionVariables.PatientNoActiveMed)).Return(false);
            sessionMock.Stub(_ => _.GetStringOrEmpty(Constants.SessionVariables.PatientId)).Return("NotNull");

            var dataMock = MockRepository.GenerateMock<IPatient>();
            dataMock.Stub(_ => _.GetPatientActiveMedication(null, null, null, ConnectionStringPointer.SHARED_DB)).IgnoreArguments().Return(MedDs);

            //act
            var result = PatientHeaderAPIController.GetPatientActiveMeds(sessionMock, dataMock);

            //assert
            Assert.AreEqual("NameMe", result.ActiveMeds[0].Name);
        }
    }
}
