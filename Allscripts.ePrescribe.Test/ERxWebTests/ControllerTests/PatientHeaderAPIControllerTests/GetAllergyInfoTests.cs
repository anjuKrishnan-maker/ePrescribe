using System;
using System.Collections.Generic;
using System.Data;
using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.Data;
using Allscripts.ePrescribe.DatabaseSelector;
using Allscripts.ePrescribe.ExtensionMethods;
using eRxWeb.Controllers;
using eRxWeb.State;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using IPatient = Allscripts.Impact.Interfaces.IPatient;

namespace Allscripts.ePrescribe.Test.ERxWebTests.ControllerTests.PatientHeaderAPIControllerTests
{
    [TestClass]
    public class GetAllergyInfoTests
    {
        private DataSet AllergyDs
        {
            get
            {
                var ds = new DataSet();
                var dt = new DataTable();
                dt.Columns.AddRange("AllergyName", "StartDate");

                var nr = dt.NewRow();
                nr["AllergyName"] = "NameMe";
                dt.Rows.Add(nr);
                ds.Tables.Add(dt);
                return ds;
            }
        }

        [TestMethod]
        public void should_set_allergy_to_none_entered_if_session_is_null()
        {
            //arrange
            var sessionMock = MockRepository.GenerateMock<IStateContainer>();
            sessionMock.Stub(_ => _.GetStringOrEmpty(Constants.SessionVariables.Allergy)).Return(null);
            sessionMock.Stub(_ => _.GetStringOrEmpty(Constants.SessionVariables.PatientNka)).Return("N");

            var dataMock = MockRepository.GenerateMock<IPatient>();

            //act
            var result = PatientHeaderAPIController.GetAllergyInfo(sessionMock, dataMock);

            //assert
            Assert.AreEqual("None entered", result.Allergy);
        }

        [TestMethod]
        public void should_return_nka()
        {
            //arrange
            var sessionMock = MockRepository.GenerateMock<IStateContainer>();
            sessionMock.Stub(_ => _.GetStringOrEmpty(Constants.SessionVariables.Allergy)).Return(null);
            sessionMock.Stub(_ => _.GetStringOrEmpty(Constants.SessionVariables.PatientNka)).Return("Y");

            var dataMock = MockRepository.GenerateMock<IPatient>();

            //act
            var result = PatientHeaderAPIController.GetAllergyInfo(sessionMock, dataMock);

            //assert
            Assert.AreEqual("No Known Allergies", result.Allergy);
        }

        [TestMethod]
        public void should_set_allergy_session_if_less_than_limit()
        {
            //arrange
            var dx = "Something small";
            var sessionMock = MockRepository.GenerateMock<IStateContainer>();
            sessionMock.Stub(_ => _.GetStringOrEmpty(Constants.SessionVariables.Allergy)).Return(dx);
            sessionMock.Stub(_ => _.GetStringOrEmpty(Constants.SessionVariables.PatientNka)).Return("N");

            var dataMock = MockRepository.GenerateMock<IPatient>();

            //act
            var result = PatientHeaderAPIController.GetAllergyInfo(sessionMock, dataMock);

            //assert
            Assert.AreEqual(dx, result.Allergy);
        }

        [TestMethod]
        public void should_set_allergy_substring_if_over_limit()
        {
            //arrange
            var dx = "Something largeSasfdddlaksjdflkajsdfuiekljasdkjfh83928239283klajhsdfjaklsdjf;laksdjfi490823904uoijkal;skdjf";
            var sessionMock = MockRepository.GenerateMock<IStateContainer>();
            sessionMock.Stub(_ => _.GetStringOrEmpty(Constants.SessionVariables.Allergy)).Return(dx);
            sessionMock.Stub(_ => _.GetStringOrEmpty(Constants.SessionVariables.PatientNka)).Return("N");

            var dataMock = MockRepository.GenerateMock<IPatient>();
            dataMock.Stub(_ => _.GetPatientAllergy(null, null, null, ConnectionStringPointer.SHARED_DB)).IgnoreArguments().Return(AllergyDs);

            //act
            var result = PatientHeaderAPIController.GetAllergyInfo(sessionMock, dataMock);

            //assert
            Assert.AreEqual(dx.Substring(0, 100), result.Allergy);
        }

        [TestMethod]
        public void should_set_more_allergy_to_true()
        {
            //arrange
            var dx = "Something largeSasfdddlaksjdflkajsdfuiekljasdkjfh83928239283klajhsdfjaklsdjf;laksdjfi490823904uoijkal;skdjf";
            var sessionMock = MockRepository.GenerateMock<IStateContainer>();
            sessionMock.Stub(_ => _.GetStringOrEmpty(Constants.SessionVariables.Allergy)).Return(dx);
            sessionMock.Stub(_ => _.GetStringOrEmpty(Constants.SessionVariables.PatientNka)).Return("N");

            var dataMock = MockRepository.GenerateMock<IPatient>();
            dataMock.Stub(_ => _.GetPatientAllergy(null, null, null, ConnectionStringPointer.SHARED_DB)).IgnoreArguments().Return(AllergyDs);

            //act
            var result = PatientHeaderAPIController.GetAllergyInfo(sessionMock, dataMock);

            //assert
            Assert.IsTrue(result.MoreActiveAllergy);
        }


        [TestMethod]
        public void should_set_active_allergy_to_data_returned_from_db()
        {
            //arrange
            var dx = "Something largeSasfdddlaksjdflkajsdfuiekljasdkjfh83928239283klajhsdfjaklsdjf;laksdjfi490823904uoijkal;skdjf";
            var sessionMock = MockRepository.GenerateMock<IStateContainer>();
            sessionMock.Stub(_ => _.GetStringOrEmpty(Constants.SessionVariables.Allergy)).Return(dx);
            sessionMock.Stub(_ => _.GetStringOrEmpty(Constants.SessionVariables.PatientNka)).Return("N");

            var dataMock = MockRepository.GenerateMock<IPatient>();
            dataMock.Stub(_ => _.GetPatientAllergy(null, null, null, ConnectionStringPointer.SHARED_DB)).IgnoreArguments().Return(AllergyDs);

            //act
            var result = PatientHeaderAPIController.GetAllergyInfo(sessionMock, dataMock);

            //assert
            Assert.AreEqual("NameMe", result.ActiveAllergies[0].Name);
        }
    }
}
