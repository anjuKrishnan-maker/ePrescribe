using System;
using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.DatabaseSelector;
using Allscripts.Impact.Interfaces;
using eRxWeb.AppCode;
using eRxWeb.AppCode.Interfaces;
using eRxWeb.State;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;

namespace Allscripts.ePrescribe.Test.ERxWebTests.AppCodeTests.SpecialtyMedTests
{
    [TestClass]
    public class SpecialtyMedSendMailOrderPharmacyTests
    {
        [TestMethod]
        public void ProcessMailOrder_Should_Send_to_Pharmacy_if_Mail_Order_Pharmacy_Available()
        {
            //arrange
            var pageStateMock = MockRepository.GenerateMock<IStateContainer>();
            pageStateMock.Stub(x => x.GetStringOrEmpty(Constants.SessionVariables.PatientHasMOBCoverage)).Return("Y");
            pageStateMock.Stub(x => x.GetStringOrEmpty(Constants.SessionVariables.MOB_NABP)).Return("Y");
            pageStateMock.Stub(x => x.GetStringOrEmpty(Constants.SessionVariables.MOB_PHARMACY_ID)).Return("16516E56-8287-4A83-AE28-2A44D2793F6A");

            var specialtyMedMock = MockRepository.GenerateMock<eRxWeb.AppCode.Interfaces.ISpecialtyMed>();

            string selectedRxID = "16516E56-8287-4A83-AE28-2A44D2793F6A";
            int selectedRefills = 0;
            int selectedDaysSupply = 10;
            string ipaddress = "10.10.10.10";

            //act
            SpecialtyMed specialtyMed = new SpecialtyMed();
            specialtyMed.ProcessMailOrder(selectedRxID, selectedRefills, selectedDaysSupply,
                pageStateMock, ipaddress, specialtyMedMock);

            //assert
            specialtyMedMock.AssertWasCalled(x => x.SendPrescription(
                Arg<string>.Is.Anything,
                Arg<string>.Is.Anything,
                Arg<int>.Is.Anything,
                Arg<int>.Is.Anything,
                Arg<string>.Is.Anything,
                Arg<bool>.Is.Anything,
                Arg<IStateContainer>.Is.Anything,
                Arg<string>.Is.Anything,
                Arg<IScriptMessage>.Is.Anything,
                Arg<IPrescription>.Is.Anything,
                Arg<eRxWeb.AppCode.Interfaces.IEPSBroker>.Is.Anything,
                Arg<ConnectionStringPointer>.Is.Anything));
        }

        [TestMethod]
        public void ProcessMailOrder_Should_Not_Send_to_Pharmacy_if_Mail_Order_Pharmacy_Not_Available()
        {
            //arrange
            var pageStateMock = MockRepository.GenerateMock<IStateContainer>();
            pageStateMock.Stub(x => x.GetStringOrEmpty(Constants.SessionVariables.PatientHasMOBCoverage)).Return("N");
            pageStateMock.Stub(x => x.GetStringOrEmpty(Constants.SessionVariables.MOB_NABP)).Return("N");
            pageStateMock.Stub(x => x.GetStringOrEmpty(Constants.SessionVariables.MOB_PHARMACY_ID)).Return("16516E56-8287-4A83-AE28-2A44D2793F6A");

            var specialtyMedMock = MockRepository.GenerateMock<eRxWeb.AppCode.Interfaces.ISpecialtyMed>();

            string selectedRxID = "16516E56-8287-4A83-AE28-2A44D2793F6A";
            int selectedRefills = 0;
            int selectedDaysSupply = 10;
            string ipaddress = "10.10.10.10";

            //act
            SpecialtyMed specialtyMed = new SpecialtyMed();
            specialtyMed.ProcessMailOrder(selectedRxID, selectedRefills, selectedDaysSupply,
                pageStateMock, ipaddress, specialtyMedMock);

            //assert
            specialtyMedMock.AssertWasNotCalled(x => x.SendPrescription(
                Arg<string>.Is.Anything,
                Arg<string>.Is.Anything,
                Arg<int>.Is.Anything,
                Arg<int>.Is.Anything,
                Arg<string>.Is.Anything,
                Arg<bool>.Is.Anything,
                Arg<IStateContainer>.Is.Anything,
                Arg<string>.Is.Anything,
                Arg<IScriptMessage>.Is.Anything,
                Arg<IPrescription>.Is.Anything,
                Arg<eRxWeb.AppCode.Interfaces.IEPSBroker>.Is.Anything,
                Arg<ConnectionStringPointer>.Is.Anything));
        }


        [TestMethod]
        public void should_return_false_if_Patient_not_EligibleForMailOrderBenefit()
        {
            //arrange
            var pageStateMock = MockRepository.GenerateMock<IStateContainer>();
            pageStateMock.Stub(x => x.GetStringOrEmpty(Constants.SessionVariables.PatientHasMOBCoverage)).Return("Y");
            pageStateMock.Stub(x => x.GetStringOrEmpty(Constants.SessionVariables.MOB_NABP)).Return("X");

            //act
            SpecialtyMed specialtyMed = new SpecialtyMed();
            bool returnValue = specialtyMed.IsPatientEligibleForMailOrderBenefit(pageStateMock);

            //assert
            Assert.AreEqual(false, returnValue);
        }

        [TestMethod]
        public void should_return_true_if_Patient_EligibleForMailOrderBenefit()
        {
            //arrange
            var pageStateMock = MockRepository.GenerateMock<IStateContainer>();
            pageStateMock.Stub(x => x.GetStringOrEmpty(Constants.SessionVariables.PatientHasMOBCoverage)).Return("Y");
            pageStateMock.Stub(x => x.GetStringOrEmpty(Constants.SessionVariables.MOB_NABP)).Return(string.Empty);

            //act
            SpecialtyMed specialtyMed = new SpecialtyMed();
            bool returnValue = specialtyMed.IsPatientEligibleForMailOrderBenefit(pageStateMock);

            //assert
            Assert.AreEqual(true, returnValue);
        }
    }
}
