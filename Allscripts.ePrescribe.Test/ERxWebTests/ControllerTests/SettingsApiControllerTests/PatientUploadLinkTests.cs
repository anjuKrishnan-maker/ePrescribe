using System;
using System.Collections.Generic;
using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.Objects;
using Allscripts.Impact;
using eRxWeb.AppCode;
using eRxWeb.Controller;
using eRxWeb.State;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using ConfigKeys = Allscripts.Impact.ConfigKeys;

namespace Allscripts.ePrescribe.Test.ERxWebTests.ControllerTests.SettingsApiControllerTests
{
    [TestClass]
    public class PatientUploadLinkTests
    {
        [TestMethod]
        public void should_add_link()
        {
            //arrange
            var al = new ApplicationLicense { LicenseDeluxeStatus = Constants.DeluxeFeatureStatus.On };
            var pageStateMock = MockRepository.GenerateMock<IStateContainer>();
            pageStateMock.Stub(x => x.Cast(Constants.SessionVariables.SessionLicense, default(ApplicationLicense))).Return(al);
            pageStateMock.Stub(x => x.GetBooleanOrFalse(Constants.LicensePreferences.PatientUpload)).Return(true);
            ConfigKeys.TestInitialize(new Dictionary<string, string> { { "PatientDemographicUpload", "Y" } });

            //act
            var link = SettingsApiController.GeneratePatientUpload(pageStateMock);

            //assert
            Assert.IsNotNull(link);
            Assert.AreEqual("Patient Upload", link.Label);
            Assert.AreEqual((Constants.PageNames.PATIENT_UPLOAD), link.ActionUrl);
        }

        [TestMethod]
        public void should_not_add_link()
        {
            //arrange
            var al = new ApplicationLicense { LicenseDeluxeStatus = Constants.DeluxeFeatureStatus.Off };
            var pageStateMock = MockRepository.GenerateMock<IStateContainer>();
            pageStateMock.Stub(x => x.Cast(Constants.SessionVariables.SessionLicense, default(ApplicationLicense))).Return(al);
            pageStateMock.Stub(x => x.GetBooleanOrFalse(Constants.LicensePreferences.PatientUpload)).Return(true);
            ConfigKeys.TestInitialize(new Dictionary<string, string> { { "PatientDemographicUpload", "N" } });


            //act
            var link = SettingsApiController.GeneratePatientUpload(pageStateMock);

            //assert
            Assert.IsNull(link);
        }

        [TestMethod]
        public void should_not_add_link_feature_hidden()
        {
            //arrange            
            var al = new ApplicationLicense { LicenseDeluxeStatus = Constants.DeluxeFeatureStatus.On };
            var pageStateMock = MockRepository.GenerateMock<IStateContainer>();
            pageStateMock.Stub(x => x.Cast(Constants.SessionVariables.SessionLicense, default(ApplicationLicense))).Return(al);
            pageStateMock.Stub(x => x.GetBooleanOrFalse(Constants.LicensePreferences.PatientUpload)).Return(false);
            ConfigKeys.TestInitialize(new Dictionary<string, string> { { "PatientDemographicUpload", "N" } });

            //act
            var link = SettingsApiController.GeneratePatientUpload(pageStateMock);

            //assert
            Assert.IsNull(link);
        }

        [TestMethod]
        public void should_not_add_link_not_deluxe_user()
        {
            //arrange
            var al = new ApplicationLicense { LicenseDeluxeStatus = Constants.DeluxeFeatureStatus.Off };
            var pageStateMock = MockRepository.GenerateMock<IStateContainer>();
            pageStateMock.Stub(x => x.Cast(Constants.SessionVariables.SessionLicense, default(ApplicationLicense))).Return(al);
            pageStateMock.Stub(x => x.GetBooleanOrFalse(Constants.LicensePreferences.PatientUpload)).Return(true);
            ConfigKeys.TestInitialize(new Dictionary<string, string> { { "PatientDemographicUpload", "Y" } });

            //act
            var link = SettingsApiController.GeneratePatientUpload(pageStateMock);

            //assert
            Assert.IsNull(link);
        }
    }
}