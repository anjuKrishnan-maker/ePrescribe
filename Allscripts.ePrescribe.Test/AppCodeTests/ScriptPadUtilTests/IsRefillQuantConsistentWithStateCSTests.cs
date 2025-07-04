using System;
using System.Data;
using Allscripts.Impact;
using Allscripts.Impact.Interfaces;
using eRxWeb.AppCode;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;

namespace Allscripts.ePrescribe.Test.AppCodeTests.ScriptPadUtilTests
{
    [TestClass]
    public class IsRefillQuantConsistentWithStateCSTests
    {
        [TestMethod]
        public void non_CS_fed_CS3_state_refills_less_than_5()
        {
            //arrange
            Rx rx = new Rx();
            rx.ControlledSubstanceCode = null;
            rx.Refills = 4;
            string dbStateCSCode = "3";
            string spoofedState = "NY";

            var pharmacyMock = MockRepository.GenerateStub<IPharmacy>();
            var prescriptionMock = MockRepository.GenerateStub<IPrescription>();

            DataSet spoofedDataSet = new DataSet();
            DataTable spoofedPharmTable = spoofedDataSet.Tables.Add("Pharmacy");
            spoofedPharmTable.Columns.Add("State");
            DataRow spoofedRow = spoofedPharmTable.Rows.Add(spoofedState);

            pharmacyMock.Stub(x => x.LoadPharmacyById("", new DatabaseSelector.ConnectionStringPointer())).IgnoreArguments().Return(spoofedDataSet);
            prescriptionMock.Stub(x => x.GetStateControlledSubstanceCode("", "", "", new DatabaseSelector.ConnectionStringPointer())).IgnoreArguments().Return(dbStateCSCode);

            //action
            bool isRefillQuantConsistentWithStateCSRules = ScriptPadUtil.IsRefillQuantConsistentWithStateCS(rx, pharmacyMock, prescriptionMock, new DatabaseSelector.ConnectionStringPointer(), out string stateCSCode, out string destinationState, out string refillText);

            //assert
            Assert.IsTrue(isRefillQuantConsistentWithStateCSRules);
        }
        [TestMethod]
        public void non_CS_fed_CS3_state_refills_greater_than_5()
        {
            //arrange
            Rx rx = new Rx();
            rx.ControlledSubstanceCode = null;
            rx.Refills = 6;
            string dbStateCSCode = "3";
            string spoofedState = "NY";

            var pharmacyMock = MockRepository.GenerateStub<IPharmacy>();
            var prescriptionMock = MockRepository.GenerateStub<IPrescription>();

            DataSet spoofedDataSet = new DataSet();
            DataTable spoofedPharmTable = spoofedDataSet.Tables.Add("Pharmacy");
            spoofedPharmTable.Columns.Add("State");
            DataRow spoofedRow = spoofedPharmTable.Rows.Add(spoofedState);

            pharmacyMock.Stub(x => x.LoadPharmacyById("", new DatabaseSelector.ConnectionStringPointer())).IgnoreArguments().Return(spoofedDataSet);
            prescriptionMock.Stub(x => x.GetStateControlledSubstanceCode("", "", "", new DatabaseSelector.ConnectionStringPointer())).IgnoreArguments().Return(dbStateCSCode);

            //action
            bool isRefillQuantConsistentWithStateCSRules = ScriptPadUtil.IsRefillQuantConsistentWithStateCS(rx, pharmacyMock, prescriptionMock, new DatabaseSelector.ConnectionStringPointer(), out string stateCSCode, out string destinationState, out string refillText);

            //assert
            Assert.IsFalse(isRefillQuantConsistentWithStateCSRules);
            Assert.AreEqual(dbStateCSCode, stateCSCode);
            Assert.AreEqual(spoofedState, destinationState);
            Assert.AreEqual("no more than 5", refillText);
        }
        [TestMethod]
        public void CS2_state_but_higher_fed_refills_more_than_0()
        {
            //arrange
            Rx rx = new Rx();
            rx.ControlledSubstanceCode = "4";
            rx.Refills = 2;
            string dbStateCSCode = "2";
            string spoofedState = "NY";

            var pharmacyMock = MockRepository.GenerateStub<IPharmacy>();
            var prescriptionMock = MockRepository.GenerateStub<IPrescription>();

            DataSet spoofedDataSet = new DataSet();
            DataTable spoofedPharmTable = spoofedDataSet.Tables.Add("Pharmacy");
            spoofedPharmTable.Columns.Add("State");
            DataRow spoofedRow = spoofedPharmTable.Rows.Add(spoofedState);

            pharmacyMock.Stub(x => x.LoadPharmacyById("", new DatabaseSelector.ConnectionStringPointer())).IgnoreArguments().Return(spoofedDataSet);
            prescriptionMock.Stub(x => x.GetStateControlledSubstanceCode("", "", "", new DatabaseSelector.ConnectionStringPointer())).IgnoreArguments().Return(dbStateCSCode);

            //action
            bool isRefillQuantConsistentWithStateCSRules = ScriptPadUtil.IsRefillQuantConsistentWithStateCS(rx, pharmacyMock, prescriptionMock, new DatabaseSelector.ConnectionStringPointer(), out string stateCSCode, out string destinationState, out string refillText);

            //assert
            Assert.IsFalse(isRefillQuantConsistentWithStateCSRules);
            Assert.AreEqual(dbStateCSCode, stateCSCode);
            Assert.AreEqual(spoofedState, destinationState);
            Assert.AreEqual("exactly 0", refillText);
        }

        [TestMethod]
        public void CS2_state_but_higher_fed_0_refills()
        {
            //arrange
            Rx rx = new Rx();
            rx.ControlledSubstanceCode = "4";
            rx.Refills = 0;
            string dbStateCSCode = "2";
            string spoofedState = "NY";

            var pharmacyMock = MockRepository.GenerateStub<IPharmacy>();
            var prescriptionMock = MockRepository.GenerateStub<IPrescription>();

            DataSet spoofedDataSet = new DataSet();
            DataTable spoofedPharmTable = spoofedDataSet.Tables.Add("Pharmacy");
            spoofedPharmTable.Columns.Add("State");
            DataRow spoofedRow = spoofedPharmTable.Rows.Add(spoofedState);

            pharmacyMock.Stub(x => x.LoadPharmacyById("", new DatabaseSelector.ConnectionStringPointer())).IgnoreArguments().Return(spoofedDataSet);
            prescriptionMock.Stub(x => x.GetStateControlledSubstanceCode("", "", "", new DatabaseSelector.ConnectionStringPointer())).IgnoreArguments().Return(dbStateCSCode);

            //action
            bool isRefillQuantConsistentWithStateCSRules = ScriptPadUtil.IsRefillQuantConsistentWithStateCS(rx, pharmacyMock, prescriptionMock, new DatabaseSelector.ConnectionStringPointer(), out string stateCSCode, out string destinationState, out string refillText);

            //assert
            Assert.IsTrue(isRefillQuantConsistentWithStateCSRules);
        }
    }
}
