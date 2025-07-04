using System;
using System.Data;
using Allscripts.ePrescribe.DatabaseSelector;
using Allscripts.ePrescribe.Objects;
using Allscripts.Impact.Interfaces;
using eRxWeb.AppCode;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;

namespace Allscripts.ePrescribe.Test.ERxWebTests.AppCodeTests.SsoTests
{
    [TestClass]
    public class GetPatientGuidGetWithAttributeTests
    {
        [TestMethod]
        public void should_return_patient_guid_if_patientId_object_holds_patientGuid_value()
        {
            //arrange
            var pGuid = new Guid("EB5C8FC5-02F5-4CE2-9B1E-3DF73BA0D58D");
            var ssoAttribute = new SsoAttributes{PatientId = pGuid};
            var isMultPatFound = false;
            var patientMock = MockRepository.GenerateMock<IPatient>();
            patientMock.Stub(x => x.SearchByChartID(null, null, ConnectionStringPointer.SHARED_DB))
                .IgnoreArguments()
                .Return(null);

            //act
            var result = Sso.GetPatientGuidGetWithAttribute(ssoAttribute, out isMultPatFound, patientMock, ConnectionStringPointer.AUDIT_ERXDB_SERVER_1);

            //assert
            Assert.AreEqual(pGuid, result);
        }

        [TestMethod]
        public void should_return_patient_guid_if_patientId_object_holds_both_patientGuid_and_Mrn()
        {
            //arrange
            var pGuid = new Guid("EB5C8FC5-02F5-4CE2-9B1E-3DF73BA0D58D");
            var ssoAttribute = new SsoAttributes { PatientId = pGuid, PatientMrn = "AHS-001", LicenseId = new Guid("AD25464A-2AEB-4410-86D3-D70ABD526876")};
            var isMultPatFound = false;
            var patientMock = MockRepository.GenerateMock<IPatient>();
            patientMock.Stub(x => x.SearchByChartID(null, null, ConnectionStringPointer.SHARED_DB))
                .IgnoreArguments()
                .Return(null);

            //act
            var result = Sso.GetPatientGuidGetWithAttribute(ssoAttribute, out isMultPatFound, patientMock, ConnectionStringPointer.AUDIT_ERXDB_SERVER_1);

            //assert
            Assert.AreEqual(pGuid, result);
        }

        [TestMethod]
        public void should_return_patient_guid_from_database_query_if_only_one_row_is_returned()
        {
            //arrange
            var pGuid = new Guid("EB5C8FC5-02F5-4CE2-9B1E-3DF73BA0D58D");
            var ssoAttribute = new SsoAttributes { PatientId = pGuid, PatientMrn = "AHS-001", LicenseId = new Guid("AD25464A-2AEB-4410-86D3-D70ABD526876") };
            var isMultPatFound = false;

            var patientDs = new DataSet();
            patientDs.Tables.Add("PatientData");
            patientDs.Tables[0].Columns.Add("PatientID");
            var row1 = patientDs.Tables[0].NewRow();
            row1["PatientID"] = pGuid.ToString();
            patientDs.Tables[0].Rows.Add(row1);


            var patientMock = MockRepository.GenerateMock<IPatient>();
            patientMock.Stub(x => x.SearchByChartID(null, null, ConnectionStringPointer.SHARED_DB))
                .IgnoreArguments()
                .Return(patientDs);

            //act
            var result = Sso.GetPatientGuidGetWithAttribute(ssoAttribute, out isMultPatFound, patientMock, ConnectionStringPointer.AUDIT_ERXDB_SERVER_1);

            //assert
            Assert.AreEqual(pGuid, result);
        }

        [TestMethod]
        public void should_return_empty_guid_and_bool_is_true_if_more_than_one_row_is_returned_from_patient_query()
        {
            //arrange
            var pGuid = new Guid("EB5C8FC5-02F5-4CE2-9B1E-3DF73BA0D58D");
            var ssoAttribute = new SsoAttributes { PatientMrn = "AHS-001", LicenseId = new Guid("AD25464A-2AEB-4410-86D3-D70ABD526876") };
            var isMultPatFound = false;

            var patientDs = new DataSet();
            patientDs.Tables.Add("PatientData");
            patientDs.Tables[0].Columns.Add("PatientID");
            var row1 = patientDs.Tables[0].NewRow();
            row1["PatientID"] = pGuid.ToString();
            var row2 = patientDs.Tables[0].NewRow();
            row2["PatientID"] = new Guid("DF2EF701-9A72-4898-9065-9FB358D8B90F");
            patientDs.Tables[0].Rows.Add(row1);
            patientDs.Tables[0].Rows.Add(row2);


            var patientMock = MockRepository.GenerateMock<IPatient>();
            patientMock.Stub(x => x.SearchByChartID(null, null, ConnectionStringPointer.SHARED_DB))
                .IgnoreArguments()
                .Return(patientDs);

            //act
            var result = Sso.GetPatientGuidGetWithAttribute(ssoAttribute, out isMultPatFound, patientMock, ConnectionStringPointer.AUDIT_ERXDB_SERVER_1);

            //assert
            Assert.AreEqual(Guid.Empty, result);
            Assert.IsTrue(isMultPatFound);
        }
    }
}
