using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.DatabaseSelector;
using Allscripts.Impact;
using Allscripts.Impact.Interfaces;
using Allscripts.Impact.PDI;
using eRxWeb.ServerModel;
using eRxWeb.State;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Runtime.CompilerServices;

namespace Allscripts.ePrescribe.Test.ERxWebTests.AppCodeTests.PatientFileUploadTests
{
    [TestClass]
    public class PatientFileUploadTests
    {
        private IStateContainer _session;
        private IPDI_ImportBatch _pdiImportBatch;
        private IPDI_ImportStagingPatient _pdiImportStagingPatient;

        [TestInitialize]
        public void Init()
        {
            _session = MockRepository.GenerateMock<IStateContainer>();
            _session.Stub(_ => _.Cast("SessionLicense", default(ApplicationLicense)))
                .Return(new ApplicationLicense { LicenseDeluxeStatus = Constants.DeluxeFeatureStatus.On });

            _pdiImportBatch = MockRepository.GenerateMock<IPDI_ImportBatch>();
            _pdiImportStagingPatient = MockRepository.GenerateMock<IPDI_ImportStagingPatient>();
        }

        [TestMethod]
        public void should_allow_user_to_see_patient_upload_feature()
        {
            // arrange
            _session.Stub(x => x.GetBooleanOrFalse(Constants.LicensePreferences.PatientUpload)).Return(true);
            ConfigKeys.TestInitialize(new Dictionary<string, string> { { "PatientDemographicUpload", "Y" } });

            //act
            var shouldShowResponse = PatientFileUpload.IsShowPatientUpload(_session);

            //assert
            Assert.IsTrue(shouldShowResponse);
        }

        [TestMethod]
        public void should_not_allow_user_to_see_patient_upload()
        {
            // arrange
            _session.Stub(x => x.GetBooleanOrFalse(Constants.LicensePreferences.PatientUpload)).Return(false);
            ConfigKeys.TestInitialize(new Dictionary<string, string> { { "PatientDemographicUpload", "N" } });

            //act
            var shouldShowResponse = PatientFileUpload.IsShowPatientUpload(_session);

            //assert
            Assert.IsFalse(shouldShowResponse);
        }

        [TestMethod]
        public void should_return_successful_upload_response_on_save_patient_data()
        {
            // arrange
            _session.Stub(x => x.GetStringOrEmpty(Constants.SessionVariables.LicenseId)).Return(new Guid().ToString());
            _session.Stub(_ => _.Cast(null, default(ConnectionStringPointer))).IgnoreArguments().Return(ConnectionStringPointer.AUDIT_ERXDB_SERVER_2);
            string patientData = @"data:text/plain;base64,Q2hhcnRJRAlMYXN0TmFtZQlGaXJzdE5hbWUJQWRkcmVzczEJQ2l0eQlTdGF0ZQlaaXAJU2V4CURPQg0KSFdJNzg3NwlSaXNlbglSaWNraQk3MTk2OSBLZWR6aWUgUGxhY2UJU2FudGEgQmFyYmFyYQlDQQk5MzE1MAlNCTEyLzExLzE5OTY=";

            //act
            var response = PatientFileUpload.SavePatientData(patientData, _session, _pdiImportStagingPatient);

            //assert
            _pdiImportStagingPatient.AssertWasCalled(_ => _.CreatePatientUploadBatchJob(Arg<PDI_ImportBatch>.Is.TypeOf, Arg<string>.Is.NotNull, Arg<ConnectionStringPointer>.Is.Equal(ConnectionStringPointer.AUDIT_ERXDB_SERVER_2)));
            Assert.AreEqual(response.PatientRecords.Count, 1);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), "Invalid File Contents")]
        public void should_return_invalid_file_contents_exception_upload_response_on_save_patient_data()
        {
            // arrange
            _session.Stub(x => x.GetStringOrEmpty(Constants.SessionVariables.LicenseId)).Return(new Guid().ToString());
            _session.Stub(_ => _.Cast(null, default(ConnectionStringPointer))).IgnoreArguments().Return(ConnectionStringPointer.AUDIT_ERXDB_SERVER_2);
            string patientData = string.Empty;

            //act
            PatientFileUpload.SavePatientData(patientData, _session, _pdiImportStagingPatient);
        }

        [TestMethod]
        public void should_call_get_job_history_with_licenseId_and_dbId()
        {
            // arrange
            _session.Stub(x => x.GetStringOrEmpty(Constants.SessionVariables.LicenseId)).Return("e79c6843-50e6-4a3d-82c1-eab63e4beb25");
            _session.Stub(_ => _.Cast(null, default(ConnectionStringPointer))).IgnoreArguments().Return(ConnectionStringPointer.AUDIT_ERXDB_SERVER_2);

            //act
            PatientFileUpload.CheckJobStatus(_session, _pdiImportBatch);

            //assert
            _pdiImportBatch.AssertWasCalled(_ => _.CheckForExistingBatchJobs(Arg<string>.Is.Equal("e79c6843-50e6-4a3d-82c1-eab63e4beb25"), Arg<ConnectionStringPointer>.Is.Equal(ConnectionStringPointer.AUDIT_ERXDB_SERVER_2)));
        }

        [TestMethod]
        public void should_call_get_patient_upload_report_with_licenseId_and_dbId()
        {
            // arrange
            _session.Stub(x => x.GetStringOrEmpty(Constants.SessionVariables.LicenseId)).Return("e79c6843-50e6-4a3d-82c1-eab63e4beb25");
            _session.Stub(_ => _.Cast(null, default(ConnectionStringPointer))).IgnoreArguments().Return(ConnectionStringPointer.AUDIT_ERXDB_SERVER_2);
            var batchDetails = new PDI_ImportBatch { ID = 147 };

            //act
            PatientFileUpload.GenerateJobHistoryReport(_session, batchDetails, _pdiImportBatch);

            //assert
            _pdiImportBatch.AssertWasCalled(_ => _.GenerateJobReport(Arg<PDI_ImportBatch>.Is.Same(batchDetails), Arg<ConnectionStringPointer>.Is.Equal(ConnectionStringPointer.AUDIT_ERXDB_SERVER_2)));
        }


        [TestMethod]
        public void should_invoke_job_processing_after_pdi_patient_table_insert()
        {
            // arrange
            _session.Stub(x => x.GetStringOrEmpty(Constants.SessionVariables.LicenseId)).Return("e79c6843-50e6-4a3d-82c1-eab63e4beb25");
            _session.Stub(x => x.GetStringOrEmpty(Constants.SessionVariables.UserId)).Return("6b27066f-626e-4ca9-85ee-edad857267c0");
            _session.Stub(_ => _.Cast(null, default(ConnectionStringPointer))).IgnoreArguments().Return(ConnectionStringPointer.AUDIT_ERXDB_SERVER_2);
            var pdiBatchID = 147;

            //act
            PatientFileUpload.InvokePatientProcessingJob(pdiBatchID, _session, _pdiImportBatch);

            //assert
            _pdiImportBatch.AssertWasCalled(_ => _.ProcessPDIBatchJob(Arg<int>.Is.Equal(pdiBatchID), Arg<string>.Is.Equal("e79c6843-50e6-4a3d-82c1-eab63e4beb25"), Arg<string>.Is.Equal("6b27066f-626e-4ca9-85ee-edad857267c0"), Arg<ConnectionStringPointer>.Is.Equal(ConnectionStringPointer.AUDIT_ERXDB_SERVER_2)));
        }
    }
}