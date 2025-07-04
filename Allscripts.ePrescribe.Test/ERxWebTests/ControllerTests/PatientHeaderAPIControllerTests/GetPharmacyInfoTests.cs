using System;
using Allscripts.ePrescribe.Common;
using eRxWeb.Controllers;
using eRxWeb.State;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;

namespace Allscripts.ePrescribe.Test.ERxWebTests.ControllerTests.PatientHeaderAPIControllerTests
{
    [TestClass]
    public class GetPharmacyInfoTests
    {
        [TestMethod]
        public void should_set_lastPharmName_to_none_entered()
        {
            //arrange
            var sessionMock = MockRepository.GenerateMock<IStateContainer>();
            sessionMock.Stub(_ => _.GetStringOrEmpty(Constants.SessionVariables.LastPharmacyName)).Return(null);

            //act
            var result = PatientHeaderAPIController.GetPharmacyInfo(sessionMock);

            //assert
            Assert.AreEqual("None entered", result.LastPharmacyName);
        }

        [TestMethod]
        public void should_set_values_if_lastPharmName_is_too_long()
        {
            //arrange
            var reallyLong = "Some really long pharmacy name and a bunch of data. And just a bit more";
            var sessionMock = MockRepository.GenerateMock<IStateContainer>();
            sessionMock.Stub(_ => _.GetStringOrEmpty(Constants.SessionVariables.LastPharmacyName)).Return(reallyLong);

            //act
            var result = PatientHeaderAPIController.GetPharmacyInfo(sessionMock);

            //assert
            Assert.AreEqual(reallyLong.Substring(0, 48), result.LastPharmacyName);
            Assert.IsTrue(result.MoreRetailPharm);
        }

        [TestMethod]
        public void should_return_lastPharmName_if_correct_size()
        {
            //arrange
            var name = "ShortName";
            var sessionMock = MockRepository.GenerateMock<IStateContainer>();
            sessionMock.Stub(_ => _.GetStringOrEmpty(Constants.SessionVariables.LastPharmacyName)).Return(name);

            //act
            var result = PatientHeaderAPIController.GetPharmacyInfo(sessionMock);

            //assert
            Assert.AreEqual(name, result.LastPharmacyName);
            Assert.IsFalse(result.MoreRetailPharm);
        }

        [TestMethod]
        public void should_set_set_remove_visible_to_true()
        {
            //arrange
            var name = "ShortName";
            var sessionMock = MockRepository.GenerateMock<IStateContainer>();
            sessionMock.Stub(_ => _.GetStringOrEmpty(Constants.SessionVariables.LastPharmacyName)).Return(name);
            sessionMock.Stub(_ => _.GetBooleanOrFalse(Constants.SessionVariables.EditPharmacy)).Return(true);

            //act
            var result = PatientHeaderAPIController.GetPharmacyInfo(sessionMock);

            //assert
            Assert.IsTrue(result.RemPharmacyVisible);
        }

        [TestMethod]
        public void should_set_set_remove_visible_to_false()
        {
            //arrange
            var name = "ShortName";
            var sessionMock = MockRepository.GenerateMock<IStateContainer>();
            sessionMock.Stub(_ => _.GetStringOrEmpty(Constants.SessionVariables.LastPharmacyName)).Return(name);
            sessionMock.Stub(_ => _.GetBooleanOrFalse(Constants.SessionVariables.EditPharmacy)).Return(false);

            //act
            var result = PatientHeaderAPIController.GetPharmacyInfo(sessionMock);

            //assert
            Assert.IsFalse(result.RemPharmacyVisible);
        }

        [TestMethod]
        public void should_set_set_is_retail_epcs_to_true()
        {
            //arrange
            var name = "ShortName";
            var sessionMock = MockRepository.GenerateMock<IStateContainer>();
            sessionMock.Stub(_ => _.GetStringOrEmpty(Constants.SessionVariables.LastPharmacyName)).Return(name);
            sessionMock.Stub(_ => _.GetBooleanOrFalse(Constants.SessionVariables.IsRetailEpcsEnabled)).Return(true);

            //act
            var result = PatientHeaderAPIController.GetPharmacyInfo(sessionMock);

            //assert
            Assert.IsTrue(result.IsRetailEpcs);
        }

        [TestMethod]
        public void should_set_set_is_retail_epcs_to_false()
        {
            //arrange
            var name = "ShortName";
            var sessionMock = MockRepository.GenerateMock<IStateContainer>();
            sessionMock.Stub(_ => _.GetStringOrEmpty(Constants.SessionVariables.LastPharmacyName)).Return(name);
            sessionMock.Stub(_ => _.GetBooleanOrFalse(Constants.SessionVariables.IsRetailEpcsEnabled)).Return(false);

            //act
            var result = PatientHeaderAPIController.GetPharmacyInfo(sessionMock);

            //assert
            Assert.IsFalse(result.IsRetailEpcs);
        }

        [TestMethod]
        public void should_set_prefMop_to_none_entered_if_nabp_is_empy()
        {
            //arrange
            var sessionMock = MockRepository.GenerateMock<IStateContainer>();
            sessionMock.Stub(_ => _.GetStringOrEmpty(Constants.SessionVariables.MOB_NABP)).Return(null);
            sessionMock.Stub(_ => _.GetStringOrEmpty(Constants.SessionVariables.MobName)).Return("NameMe");

            //act
            var result = PatientHeaderAPIController.GetPharmacyInfo(sessionMock);

            //assert
            Assert.AreEqual("None entered", result.PrefMOP);
        }

        [TestMethod]
        public void should_set_prefMop_to_none_entered_if_mob_name_is_empy()
        {
            //arrange
            var sessionMock = MockRepository.GenerateMock<IStateContainer>();
            sessionMock.Stub(_ => _.GetStringOrEmpty(Constants.SessionVariables.MOB_NABP)).Return("NameMe");
            sessionMock.Stub(_ => _.GetStringOrEmpty(Constants.SessionVariables.MobName)).Return(null);

            //act
            var result = PatientHeaderAPIController.GetPharmacyInfo(sessionMock);

            //assert
            Assert.AreEqual("None entered", result.PrefMOP);
        }

        [TestMethod]
        public void should_set_values_if_mobName_is_too_long()
        {
            //arrange
            var reallyLong = "Some really long pharmacy name and a bunch of data. And just a bit more";
            var sessionMock = MockRepository.GenerateMock<IStateContainer>();
            sessionMock.Stub(_ => _.GetStringOrEmpty(Constants.SessionVariables.MOB_NABP)).Return("NameMe");
            sessionMock.Stub(_ => _.GetStringOrEmpty(Constants.SessionVariables.MobName)).Return(reallyLong);

            //act
            var result = PatientHeaderAPIController.GetPharmacyInfo(sessionMock);

            //assert
            Assert.AreEqual(reallyLong.Substring(0, 48), result.PrefMOP);
            Assert.IsTrue(result.MoreMailOrderPharmVisible);
        }

        [TestMethod]
        public void should_return_mobName_if_correct_size()
        {
            //arrange
            var name = "ShortName";
            var sessionMock = MockRepository.GenerateMock<IStateContainer>();
            sessionMock.Stub(_ => _.GetStringOrEmpty(Constants.SessionVariables.MOB_NABP)).Return("NameMe");
            sessionMock.Stub(_ => _.GetStringOrEmpty(Constants.SessionVariables.MobName)).Return(name);

            //act
            var result = PatientHeaderAPIController.GetPharmacyInfo(sessionMock);

            //assert
            Assert.AreEqual(name, result.PrefMOP);
            Assert.IsFalse(result.MoreMailOrderPharmVisible);
        }

        [TestMethod]
        public void should_set_set_mo_remove_visible_to_true()
        {
            //arrange
            var name = "ShortName";
            var sessionMock = MockRepository.GenerateMock<IStateContainer>();
            sessionMock.Stub(_ => _.GetStringOrEmpty(Constants.SessionVariables.MOB_NABP)).Return("NameMe");
            sessionMock.Stub(_ => _.GetStringOrEmpty(Constants.SessionVariables.MobName)).Return(name);
            sessionMock.Stub(_ => _.GetBooleanOrFalse(Constants.SessionVariables.EditPharmacy)).Return(true);

            //act
            var result = PatientHeaderAPIController.GetPharmacyInfo(sessionMock);

            //assert
            Assert.IsTrue(result.RemMOPharmVisible);
        }

        [TestMethod]
        public void should_set_set_mo_remove_visible_to_false()
        {
            //arrange
            var name = "ShortName";
            var sessionMock = MockRepository.GenerateMock<IStateContainer>();
            sessionMock.Stub(_ => _.GetStringOrEmpty(Constants.SessionVariables.MOB_NABP)).Return("NameMe");
            sessionMock.Stub(_ => _.GetStringOrEmpty(Constants.SessionVariables.MobName)).Return(name);
            sessionMock.Stub(_ => _.GetBooleanOrFalse(Constants.SessionVariables.EditPharmacy)).Return(false);

            //act
            var result = PatientHeaderAPIController.GetPharmacyInfo(sessionMock);

            //assert
            Assert.IsFalse(result.RemMOPharmVisible);
        }

        [TestMethod]
        public void should_set_set_is_mo_epcs_to_true()
        {
            //arrange
            var name = "ShortName";
            var sessionMock = MockRepository.GenerateMock<IStateContainer>();
            sessionMock.Stub(_ => _.GetStringOrEmpty(Constants.SessionVariables.MOB_NABP)).Return("NameMe");
            sessionMock.Stub(_ => _.GetStringOrEmpty(Constants.SessionVariables.MobName)).Return(name);
            sessionMock.Stub(_ => _.GetBooleanOrFalse(Constants.SessionVariables.IsMobEpcsEnabled)).Return(true);

            //act
            var result = PatientHeaderAPIController.GetPharmacyInfo(sessionMock);

            //assert
            Assert.IsTrue(result.IsMoEpcs);
        }

        [TestMethod]
        public void should_set_set_is_mo_epcs_to_false()
        {
            //arrange
            var name = "ShortName";
            var sessionMock = MockRepository.GenerateMock<IStateContainer>();
            sessionMock.Stub(_ => _.GetStringOrEmpty(Constants.SessionVariables.MOB_NABP)).Return("NameMe");
            sessionMock.Stub(_ => _.GetStringOrEmpty(Constants.SessionVariables.MobName)).Return(name);
            sessionMock.Stub(_ => _.GetBooleanOrFalse(Constants.SessionVariables.IsMobEpcsEnabled)).Return(false);

            //act
            var result = PatientHeaderAPIController.GetPharmacyInfo(sessionMock);

            //assert
            Assert.IsFalse(result.IsMoEpcs);
        }
    }
}
