using System;
using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.Objects;
using eRxWeb;
using eRxWeb.PageInterfaces;
using eRxWeb.State;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;

namespace Allscripts.ePrescribe.Test.ERxWebTests.ScriptPadTests
{
    [TestClass]
    public class ShouldShowPatMissingInfoTests
    {
        [TestMethod]
        public void should_return_true_if_address1_is_missing_adult_patient()
        {
            //arrange
            var scriptPad = MockRepository.GenerateMock<IScriptPad>();

            var sessionMock = MockRepository.GenerateMock<IStateContainer>();
            sessionMock.Stub(_ => _.GetStringOrEmpty(Constants.SessionVariables.PatientAddress1)).Return("");
            sessionMock.Stub(_ => _.GetStringOrEmpty(Constants.SessionVariables.PatientCity)).Return("City");
            sessionMock.Stub(_ => _.GetStringOrEmpty(Constants.SessionVariables.PatientZip)).Return("88011");
            sessionMock.Stub(_ => _.GetStringOrEmpty(Constants.SessionVariables.PatientState)).Return("NM");
            sessionMock.Stub(_ => _.GetInt(Constants.SessionVariables.PatientYearsOld, 0)).Return(24);

            sessionMock.Stub(_ => _.Cast(Arg<string>.Is.Equal(Constants.SessionVariables.PatientHeight), Arg<Height>.Is.Anything)).Return(new Height("33"));
            sessionMock.Stub(_ => _.Cast(Arg<string>.Is.Equal(Constants.SessionVariables.PatientWeight), Arg<Weight>.Is.Anything)).Return(new Weight("34"));

            //act
            var result = ScriptPad.ShouldShowPatMissingInfo(sessionMock);

            //assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void should_return_true_if_city_is_missing_adult_patient()
        {
            //arrange
            var scriptPad = MockRepository.GenerateMock<IScriptPad>();

            var sessionMock = MockRepository.GenerateMock<IStateContainer>();
            sessionMock.Stub(_ => _.GetStringOrEmpty(Constants.SessionVariables.PatientAddress1)).Return("Address1");
            sessionMock.Stub(_ => _.GetStringOrEmpty(Constants.SessionVariables.PatientCity)).Return("");
            sessionMock.Stub(_ => _.GetStringOrEmpty(Constants.SessionVariables.PatientZip)).Return("88011");
            sessionMock.Stub(_ => _.GetStringOrEmpty(Constants.SessionVariables.PatientState)).Return("NM");
            sessionMock.Stub(_ => _.GetInt(Constants.SessionVariables.PatientYearsOld, 0)).Return(24);

            sessionMock.Stub(_ => _.Cast(Arg<string>.Is.Equal(Constants.SessionVariables.PatientHeight), Arg<Height>.Is.Anything)).Return(new Height("33"));
            sessionMock.Stub(_ => _.Cast(Arg<string>.Is.Equal(Constants.SessionVariables.PatientWeight), Arg<Weight>.Is.Anything)).Return(new Weight("34"));

            //act
            var result = ScriptPad.ShouldShowPatMissingInfo(sessionMock);

            //assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void should_return_true_if_zip_is_missing_adult_patient()
        {
            //arrange
            var scriptPad = MockRepository.GenerateMock<IScriptPad>();

            var sessionMock = MockRepository.GenerateMock<IStateContainer>();
            sessionMock.Stub(_ => _.GetStringOrEmpty(Constants.SessionVariables.PatientAddress1)).Return("Address1");
            sessionMock.Stub(_ => _.GetStringOrEmpty(Constants.SessionVariables.PatientCity)).Return("City");
            sessionMock.Stub(_ => _.GetStringOrEmpty(Constants.SessionVariables.PatientZip)).Return("");
            sessionMock.Stub(_ => _.GetStringOrEmpty(Constants.SessionVariables.PatientState)).Return("NM");
            sessionMock.Stub(_ => _.GetInt(Constants.SessionVariables.PatientYearsOld, 0)).Return(24);

            sessionMock.Stub(_ => _.Cast(Arg<string>.Is.Equal(Constants.SessionVariables.PatientHeight), Arg<Height>.Is.Anything)).Return(new Height("33"));
            sessionMock.Stub(_ => _.Cast(Arg<string>.Is.Equal(Constants.SessionVariables.PatientWeight), Arg<Weight>.Is.Anything)).Return(new Weight("34"));

            //act
            var result = ScriptPad.ShouldShowPatMissingInfo(sessionMock);

            //assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void should_return_true_if_state_is_missing_adult_patient()
        {
            //arrange
            var scriptPad = MockRepository.GenerateMock<IScriptPad>();

            var sessionMock = MockRepository.GenerateMock<IStateContainer>();
            sessionMock.Stub(_ => _.GetStringOrEmpty(Constants.SessionVariables.PatientAddress1)).Return("Address1");
            sessionMock.Stub(_ => _.GetStringOrEmpty(Constants.SessionVariables.PatientCity)).Return("City");
            sessionMock.Stub(_ => _.GetStringOrEmpty(Constants.SessionVariables.PatientZip)).Return("88011");
            sessionMock.Stub(_ => _.GetStringOrEmpty(Constants.SessionVariables.PatientState)).Return("");
            sessionMock.Stub(_ => _.GetInt(Constants.SessionVariables.PatientYearsOld, 0)).Return(24);

            sessionMock.Stub(_ => _.Cast(Arg<string>.Is.Equal(Constants.SessionVariables.PatientHeight), Arg<Height>.Is.Anything)).Return(new Height("33"));
            sessionMock.Stub(_ => _.Cast(Arg<string>.Is.Equal(Constants.SessionVariables.PatientWeight), Arg<Weight>.Is.Anything)).Return(new Weight("34"));

            //act
            var result = ScriptPad.ShouldShowPatMissingInfo(sessionMock);

            //assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void should_return_true_if_state_zip_address_city_missing_valid_observations_adult_patient()
        {
            //arrange
            var scriptPad = MockRepository.GenerateMock<IScriptPad>();

            var sessionMock = MockRepository.GenerateMock<IStateContainer>();
            sessionMock.Stub(_ => _.GetStringOrEmpty(Constants.SessionVariables.PatientAddress1)).Return("");
            sessionMock.Stub(_ => _.GetStringOrEmpty(Constants.SessionVariables.PatientCity)).Return("");
            sessionMock.Stub(_ => _.GetStringOrEmpty(Constants.SessionVariables.PatientZip)).Return("");
            sessionMock.Stub(_ => _.GetStringOrEmpty(Constants.SessionVariables.PatientState)).Return("");
            sessionMock.Stub(_ => _.GetInt(Constants.SessionVariables.PatientYearsOld, 0)).Return(24);

            sessionMock.Stub(_ => _.Cast(Arg<string>.Is.Equal(Constants.SessionVariables.PatientHeight), Arg<Height>.Is.Anything)).Return(new Height("34"));
            sessionMock.Stub(_ => _.Cast(Arg<string>.Is.Equal(Constants.SessionVariables.PatientWeight), Arg<Weight>.Is.Anything)).Return(new Weight("43"));

            //act
            var result = ScriptPad.ShouldShowPatMissingInfo(sessionMock);

            //assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void should_return_true_if_address1_is_missing_observations_valid_ped_patient()
        {
            //arrange
            var scriptPad = MockRepository.GenerateMock<IScriptPad>();

            var sessionMock = MockRepository.GenerateMock<IStateContainer>();
            sessionMock.Stub(_ => _.GetStringOrEmpty(Constants.SessionVariables.PatientAddress1)).Return("");
            sessionMock.Stub(_ => _.GetStringOrEmpty(Constants.SessionVariables.PatientCity)).Return("City");
            sessionMock.Stub(_ => _.GetStringOrEmpty(Constants.SessionVariables.PatientZip)).Return("88011");
            sessionMock.Stub(_ => _.GetStringOrEmpty(Constants.SessionVariables.PatientState)).Return("NM");
            sessionMock.Stub(_ => _.GetInt(Constants.SessionVariables.PatientYearsOld, 0)).Return(2);

            sessionMock.Stub(_ => _.Cast(Arg<string>.Is.Equal(Constants.SessionVariables.PatientHeight), Arg<Height>.Is.Anything)).Return(new Height("33"));
            sessionMock.Stub(_ => _.Cast(Arg<string>.Is.Equal(Constants.SessionVariables.PatientWeight), Arg<Weight>.Is.Anything)).Return(new Weight("34"));

            //act
            var result = ScriptPad.ShouldShowPatMissingInfo(sessionMock);

            //assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void should_return_true_if_city_is_missing_observations_valid_ped_patient()
        {
            //arrange
            var scriptPad = MockRepository.GenerateMock<IScriptPad>();

            var sessionMock = MockRepository.GenerateMock<IStateContainer>();
            sessionMock.Stub(_ => _.GetStringOrEmpty(Constants.SessionVariables.PatientAddress1)).Return("Address1");
            sessionMock.Stub(_ => _.GetStringOrEmpty(Constants.SessionVariables.PatientCity)).Return("");
            sessionMock.Stub(_ => _.GetStringOrEmpty(Constants.SessionVariables.PatientZip)).Return("88011");
            sessionMock.Stub(_ => _.GetStringOrEmpty(Constants.SessionVariables.PatientState)).Return("NM");
            sessionMock.Stub(_ => _.GetInt(Constants.SessionVariables.PatientYearsOld, 0)).Return(24);

            sessionMock.Stub(_ => _.Cast(Arg<string>.Is.Equal(Constants.SessionVariables.PatientHeight), Arg<Height>.Is.Anything)).Return(new Height("33"));
            sessionMock.Stub(_ => _.Cast(Arg<string>.Is.Equal(Constants.SessionVariables.PatientWeight), Arg<Weight>.Is.Anything)).Return(new Weight("34"));

            //act
            var result = ScriptPad.ShouldShowPatMissingInfo(sessionMock);

            //assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void should_return_true_if_zip_is_missing_observations_valid_ped_patient()
        {
            //arrange
            var scriptPad = MockRepository.GenerateMock<IScriptPad>();

            var sessionMock = MockRepository.GenerateMock<IStateContainer>();
            sessionMock.Stub(_ => _.GetStringOrEmpty(Constants.SessionVariables.PatientAddress1)).Return("Address1");
            sessionMock.Stub(_ => _.GetStringOrEmpty(Constants.SessionVariables.PatientCity)).Return("City");
            sessionMock.Stub(_ => _.GetStringOrEmpty(Constants.SessionVariables.PatientZip)).Return("");
            sessionMock.Stub(_ => _.GetStringOrEmpty(Constants.SessionVariables.PatientState)).Return("NM");
            sessionMock.Stub(_ => _.GetInt(Constants.SessionVariables.PatientYearsOld, 0)).Return(24);

            sessionMock.Stub(_ => _.Cast(Arg<string>.Is.Equal(Constants.SessionVariables.PatientHeight), Arg<Height>.Is.Anything)).Return(new Height("33"));
            sessionMock.Stub(_ => _.Cast(Arg<string>.Is.Equal(Constants.SessionVariables.PatientWeight), Arg<Weight>.Is.Anything)).Return(new Weight("34"));

            //act
            var result = ScriptPad.ShouldShowPatMissingInfo(sessionMock);

            //assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void should_return_true_if_state_is_missing_observations_valid_ped_patient()
        {
            //arrange
            var scriptPad = MockRepository.GenerateMock<IScriptPad>();

            var sessionMock = MockRepository.GenerateMock<IStateContainer>();
            sessionMock.Stub(_ => _.GetStringOrEmpty(Constants.SessionVariables.PatientAddress1)).Return("Address1");
            sessionMock.Stub(_ => _.GetStringOrEmpty(Constants.SessionVariables.PatientCity)).Return("City");
            sessionMock.Stub(_ => _.GetStringOrEmpty(Constants.SessionVariables.PatientZip)).Return("88011");
            sessionMock.Stub(_ => _.GetStringOrEmpty(Constants.SessionVariables.PatientState)).Return("");
            sessionMock.Stub(_ => _.GetInt(Constants.SessionVariables.PatientYearsOld, 0)).Return(24);

            sessionMock.Stub(_ => _.Cast(Arg<string>.Is.Equal(Constants.SessionVariables.PatientHeight), Arg<Height>.Is.Anything)).Return(new Height("33"));
            sessionMock.Stub(_ => _.Cast(Arg<string>.Is.Equal(Constants.SessionVariables.PatientWeight), Arg<Weight>.Is.Anything)).Return(new Weight("34"));

            //act
            var result = ScriptPad.ShouldShowPatMissingInfo(sessionMock);

            //assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void should_return_true_if_state_zip_address_city_missing_ped_patient()
        {
            //arrange
            var scriptPad = MockRepository.GenerateMock<IScriptPad>();

            var sessionMock = MockRepository.GenerateMock<IStateContainer>();
            sessionMock.Stub(_ => _.GetStringOrEmpty(Constants.SessionVariables.PatientAddress1)).Return("");
            sessionMock.Stub(_ => _.GetStringOrEmpty(Constants.SessionVariables.PatientCity)).Return("");
            sessionMock.Stub(_ => _.GetStringOrEmpty(Constants.SessionVariables.PatientZip)).Return("");
            sessionMock.Stub(_ => _.GetStringOrEmpty(Constants.SessionVariables.PatientState)).Return("");
            sessionMock.Stub(_ => _.GetInt(Constants.SessionVariables.PatientYearsOld, 0)).Return(2);

            sessionMock.Stub(_ => _.Cast(Arg<string>.Is.Equal(Constants.SessionVariables.PatientHeight), Arg<Height>.Is.Anything)).Return(new Height("33"));
            sessionMock.Stub(_ => _.Cast(Arg<string>.Is.Equal(Constants.SessionVariables.PatientWeight), Arg<Weight>.Is.Anything)).Return(new Weight("34"));

            //act
            var result = ScriptPad.ShouldShowPatMissingInfo(sessionMock);

            //assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void should_return_false_if_state_zip_address_city_all_valid_no_height_adult_patient()
        {
            //arrange
            var scriptPad = MockRepository.GenerateMock<IScriptPad>();

            var sessionMock = MockRepository.GenerateMock<IStateContainer>();
            sessionMock.Stub(_ => _.GetStringOrEmpty(Constants.SessionVariables.PatientAddress1)).Return("Adress1");
            sessionMock.Stub(_ => _.GetStringOrEmpty(Constants.SessionVariables.PatientCity)).Return("City");
            sessionMock.Stub(_ => _.GetStringOrEmpty(Constants.SessionVariables.PatientZip)).Return("88011");
            sessionMock.Stub(_ => _.GetStringOrEmpty(Constants.SessionVariables.PatientState)).Return("NM");
            sessionMock.Stub(_ => _.GetInt(Constants.SessionVariables.PatientYearsOld, 0)).Return(24);

            sessionMock.Stub(_ => _.Cast(Arg<string>.Is.Equal(Constants.SessionVariables.PatientHeight), Arg<Height>.Is.Anything)).Return(new Height(""));
            sessionMock.Stub(_ => _.Cast(Arg<string>.Is.Equal(Constants.SessionVariables.PatientWeight), Arg<Weight>.Is.Anything)).Return(new Weight(""));

            //act
            var result = ScriptPad.ShouldShowPatMissingInfo(sessionMock);

            //assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void should_return_false_if_state_zip_address_city_all_valid_wieght_height_vald_ped_patient()
        {
            //arrange
            var scriptPad = MockRepository.GenerateMock<IScriptPad>();

            var sessionMock = MockRepository.GenerateMock<IStateContainer>();
            sessionMock.Stub(_ => _.GetStringOrEmpty(Constants.SessionVariables.PatientAddress1)).Return("Adress1");
            sessionMock.Stub(_ => _.GetStringOrEmpty(Constants.SessionVariables.PatientCity)).Return("City");
            sessionMock.Stub(_ => _.GetStringOrEmpty(Constants.SessionVariables.PatientZip)).Return("88011");
            sessionMock.Stub(_ => _.GetStringOrEmpty(Constants.SessionVariables.PatientState)).Return("NM");
            sessionMock.Stub(_ => _.GetInt(Constants.SessionVariables.PatientYearsOld, 0)).Return(2);

            sessionMock.Stub(_ => _.Cast(Arg<string>.Is.Equal(Constants.SessionVariables.PatientHeight), Arg<Height>.Is.Anything)).Return(new Height("12"));
            sessionMock.Stub(_ => _.Cast(Arg<string>.Is.Equal(Constants.SessionVariables.PatientWeight), Arg<Weight>.Is.Anything)).Return(new Weight("21"));

            //act
            var result = ScriptPad.ShouldShowPatMissingInfo(sessionMock);

            //assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void should_return_false_if_state_zip_address_city_all_valid_adult_patient()
        {
            //arrange
            var scriptPad = MockRepository.GenerateMock<IScriptPad>();

            var sessionMock = MockRepository.GenerateMock<IStateContainer>();
            sessionMock.Stub(_ => _.GetStringOrEmpty(Constants.SessionVariables.PatientAddress1)).Return("Address1");
            sessionMock.Stub(_ => _.GetStringOrEmpty(Constants.SessionVariables.PatientCity)).Return("City");
            sessionMock.Stub(_ => _.GetStringOrEmpty(Constants.SessionVariables.PatientZip)).Return("Zip");
            sessionMock.Stub(_ => _.GetStringOrEmpty(Constants.SessionVariables.PatientState)).Return("State");
            sessionMock.Stub(_ => _.GetInt(Constants.SessionVariables.PatientYearsOld, 0)).Return(24);

            //act
            var result = ScriptPad.ShouldShowPatMissingInfo(sessionMock);

            //assert
            Assert.IsFalse(result);
        }
    }
}