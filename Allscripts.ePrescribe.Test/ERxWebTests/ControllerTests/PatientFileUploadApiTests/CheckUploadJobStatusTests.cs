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
    public class CheckUploadJobStatusTests
    {
        private IStateContainer _pageState;
        [TestInitialize]
        public void init()
        {
            _pageState = MockRepository.GenerateStub<IStateContainer>();
        }

        [TestMethod, TestCategory("Patient Upload API")]
        public void Should_return_type_patient_upload_response()
        {
            // arrange
            var patientUploadController = new PatientUploadController();
            _pageState[Constants.SessionVariables.LicenseId] = Guid.NewGuid().ToString();
            patientUploadController.PageState = _pageState;

            //act
            var response = patientUploadController.GetJobStatus();

            //assert
            Assert.IsInstanceOfType(response.Payload, typeof(PatientUploadResponse));
        }

        [TestMethod, TestCategory("Patient Upload API")]
        public void Should_return_bad_request_response()
        {
            // arrange
            var patientUploadController = new PatientUploadController();
            _pageState[Constants.SessionVariables.LicenseId] = Guid.NewGuid().ToString();
            patientUploadController.PageState = _pageState;

            //act
            var response = patientUploadController.GetJobStatus();

            //unbox and assert
            Assert.IsInstanceOfType(response.Payload, typeof(PatientUploadResponse));
        }
    }
}
