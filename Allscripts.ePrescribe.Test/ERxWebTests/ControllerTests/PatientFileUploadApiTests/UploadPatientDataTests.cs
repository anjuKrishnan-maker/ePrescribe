using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using Rhino.Mocks;
using eRxWeb.State;
using eRxWeb.Controller;
using eRxWeb.ServerModel;
using System.Net;
using Allscripts.ePrescribe.Common;

namespace Allscripts.ePrescribe.Test.ERxWebTests.ControllerTests.PatientFileUploadApiTests
{
    [TestClass]
    public class UploadPatientDataTests
    {
        private IStateContainer _pageState;
        [TestInitialize]
        public void init()
        {
            _pageState = MockRepository.GenerateStub<IStateContainer>();
        }
        [TestMethod, TestCategory("Patient Upload API")]
        public void should_return_type_patient_upload_response()
        {
            // arrange
            var patientUploadController = new PatientUploadController();
            var patientData = @"data:text/plain;base64,Rmlyc3ROYW1lCUxhc3ROYW1lCUFkZHJlc3MxCUNpdHkJU3RhdGUJWmlwCUdlbmRlcglET0INCkNlc2FyZQlUZXdlbHNvbgk3IFV0YWggUGFyawlXZXN0IFBhbG0gQmVhY2gJRkwJMzM0MTEJTQkwMy8yMS8xOTY3DQpNaXRjaGFlbAlBdWJlcnkJMjMxMSBCcm93bmluZyBKdW5jdGlvbglEYWxsYXMJVFgJNzUyNDEJTQkwOS8yNS8yMDAxDQpIYXJsZXkJUG9sbGUJMTUgU3dhbGxvdyBDcm9zc2luZwlTcHJpbmdmaWVsZAlJTAk2Mjc3NglGCTExLzE0LzE5Mjg=";
            _pageState[Constants.SessionVariables.LicenseId] = Guid.NewGuid().ToString();
            patientUploadController.PageState = _pageState;

            //act
            var response = patientUploadController.UploadFile(patientData);

            //assert
            Assert.IsInstanceOfType(response.Payload, typeof(PatientUploadResponse));
        }

        [TestMethod, TestCategory("Patient Upload API")]
        public void should_return_upload_patient_data_bad_server_request()
        {
            // arrange
            var patientUploadController = new PatientUploadController();
            _pageState[Constants.SessionVariables.LicenseId] = Guid.NewGuid().ToString();
            patientUploadController.PageState = _pageState;

            //act
            var response = patientUploadController.UploadFile(string.Empty);

            //assert
            Assert.IsInstanceOfType(response.Payload, typeof(PatientUploadResponse));
        }
    }
}
