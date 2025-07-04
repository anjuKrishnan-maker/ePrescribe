using System;
using System.Data;
using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.DatabaseSelector;
using Allscripts.Impact;
using Allscripts.Impact.Interfaces;
using eRxWeb;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using Pharmacy = Allscripts.Impact.Pharmacy;

namespace Allscripts.ePrescribe.Test.ScriptPadTests
{
    [TestClass]
    public class ShouldShowPharmacySigAlertUnitTests
    {
        [TestMethod]
        public void should_return_empty_string_if_rx_null()
        {
            //Arrange
            ScriptPad scriptPad = new ScriptPad();

            //Act
            string ShowPharmacyAlertText = scriptPad.PharmacyAlertMessage(null, string.Empty,
                string.Empty, ConnectionStringPointer.DEBUG_DB, new Pharmacy());

            //Assert
            Assert.AreEqual(ShowPharmacyAlertText, string.Empty);
        }
        [TestMethod]
        public void should_return_sig_alert_if_SigTextLength_greater_than_140_and_destination_Is_SendPhyWRetail_And_Pharmacy_Is_Not_NCPDP2017071()
        {
            //Arrange
            ScriptPad scriptPad = new ScriptPad();
            Rx rx = new Rx();
            rx.SigText = "This is test.This is test.This is test.This is test.This is test.This is test.This is test.This is test.This is test.This is test.This is tes";
            rx.PharmacyID = "PHARM12345";
            rx.Notes = string.Empty;            //Arrange
            var pharmacyMock = MockRepository.GenerateMock<IPharmacy>();
            DataTable table = new DataTable();

            // Declare DataColumn and DataRow variables.
            DataSet ds = new DataSet();
            DataColumn column;
            DataRow row;

            // Create new DataColumn, set DataType, ColumnName and add to DataTable.    
            column = new DataColumn();
            column.ColumnName = "Version";
            table.Columns.Add(column);

            // Create new DataRow objects and add to DataTable.    
            row = table.NewRow();
            row["Version"] = "v10.6";
            table.Rows.Add(row);
            ds.Tables.Add(table);
            pharmacyMock.Stub(x => x.LoadPharmacyById(rx.PharmacyID, new ConnectionStringPointer())).Return(ds);

            //Act
            var mockSession = MockRepository.GenerateStub<eRxWeb.State.IStateContainer>();
            mockSession.Stub(_ => _.GetBooleanOrFalse(Constants.SessionVariables.ShouldShowPpt)).Return(true);

            //Act
            string ShowPharmacyAlertText = scriptPad.PharmacyAlertMessage(rx, Constants.PharmacySigAlertConstants.SendToPhysicianWithRetail,  string.Empty, new ConnectionStringPointer(), pharmacyMock);

            //Assert
            Assert.AreEqual(ShowPharmacyAlertText, Constants.ErrorMessages.SigAlertOver140ToNot6XPharmacy);
        }

        [TestMethod]
        public void should_return_sig_alert_if_SigTextLength_greater_than_140_and_required_nadean_not_given_and_destination_Is_SendToPharmacy_And_Pharmacy_Is_Not_NCPDP2017071()
        {
            //Arrange
            ScriptPad scriptPad = new ScriptPad();
            Rx rx = new Rx();
            rx.SigText = "This is test.This is test.This is test.This is test.This is test.This is test.This is test.This is test.This is test.This is test.This is tes";
            rx.Notes = "This should be over 140 charThis should be over 140 charThis should be over 140 charThis should be over 140 charThis should be over 140 charThis should be over 140 charThis should be over 140 charThis should be over 140 charThis should be over 140 char";
            rx.ControlledSubstanceCode = "2";
            rx.PharmacyID = "PHARM12345";
            //Arrange
            var pharmacyMock = MockRepository.GenerateMock<IPharmacy>();
            DataTable table = new DataTable();

            // Declare DataColumn and DataRow variables.
            DataSet ds = new DataSet();
            DataColumn column;
            DataRow row;

            // Create new DataColumn, set DataType, ColumnName and add to DataTable.    
            column = new DataColumn();
            column.ColumnName = "Version";
            table.Columns.Add(column);

            // Create new DataRow objects and add to DataTable.    
            row = table.NewRow();
            row["Version"] = "v10.6";
            table.Rows.Add(row);
            ds.Tables.Add(table);
            pharmacyMock.Stub(x => x.LoadPharmacyById(rx.PharmacyID, new ConnectionStringPointer())).Return(ds);

            //Act


            //Act
            string ShowPharmacyAlertText = scriptPad.PharmacyAlertMessage(rx, Constants.PharmacySigAlertConstants.SendToPharmacy, "XS123456", new ConnectionStringPointer(), pharmacyMock);

            //Assert
            Assert.AreEqual(ShowPharmacyAlertText, Constants.ErrorMessages.SigAlertOver140ToNot6XPharmacy + " " + Constants.ErrorMessages.NADEANPrependOverflowInNotes);
        }

        [TestMethod]
        public void should_return_sig_alert_if_SigTextLength_less_than_140_and_required_nadean_not_given_and_destination_Is_SendToPharmacy_And_Pharmacy_Is_Not_NCPDP2017071()
        {
            //Arrange
            ScriptPad scriptPad = new ScriptPad();
            Rx rx = new Rx();
            rx.SigText = "This is test.";
            rx.Notes = "sample notes XS123";
            rx.ControlledSubstanceCode = "2";
            rx.PharmacyID = "PHARM12345";
            //Arrange
            var pharmacyMock = MockRepository.GenerateMock<IPharmacy>();
            DataTable table = new DataTable();

            // Declare DataColumn and DataRow variables.
            DataSet ds = new DataSet();
            DataColumn column;
            DataRow row;

            // Create new DataColumn, set DataType, ColumnName and add to DataTable.    
            column = new DataColumn();
            column.ColumnName = "Version";
            table.Columns.Add(column);

            // Create new DataRow objects and add to DataTable.    
            row = table.NewRow();
            row["Version"] = "v10.6";
            table.Rows.Add(row);
            ds.Tables.Add(table);
            pharmacyMock.Stub(x => x.LoadPharmacyById(rx.PharmacyID, new ConnectionStringPointer())).Return(ds);

            //Act
            string ShowPharmacyAlertText = scriptPad.PharmacyAlertMessage(rx, Constants.PharmacySigAlertConstants.SendToPharmacy, "XS123456", new ConnectionStringPointer(), pharmacyMock);

            //Assert
            Assert.AreEqual(ShowPharmacyAlertText, " " + Constants.ErrorMessages.NADEANPrependOverflowInNotes);
        }

        [TestMethod]
        public void should_not_populate_sig_alert_if_SigTextLength_less_than_140_and_required_nadean_is_given_and_destination_Is_SendToPharmacy_And_Pharmacy_Is_Not_NCPDP2017071()
        {
            //Arrange
            ScriptPad scriptPad = new ScriptPad();
            Rx rx = new Rx();
            rx.SigText = "This is test.";
            rx.Notes = "sample notes XS123456";
            rx.ControlledSubstanceCode = "2";
            rx.PharmacyID = "PHARM12345";
            //Arrange
            var pharmacyMock = MockRepository.GenerateMock<IPharmacy>();
            DataTable table = new DataTable();

            // Declare DataColumn and DataRow variables.
            DataSet ds = new DataSet();
            DataColumn column;
            DataRow row;

            // Create new DataColumn, set DataType, ColumnName and add to DataTable.    
            column = new DataColumn();
            column.ColumnName = "Version";
            table.Columns.Add(column);

            // Create new DataRow objects and add to DataTable.    
            row = table.NewRow();
            row["Version"] = "v10.6";
            table.Rows.Add(row);
            ds.Tables.Add(table);
            pharmacyMock.Stub(x => x.LoadPharmacyById(rx.PharmacyID, new ConnectionStringPointer())).Return(ds);

            //Act
            string ShowPharmacyAlertText = scriptPad.PharmacyAlertMessage(rx, Constants.PharmacySigAlertConstants.SendToPharmacy, "XS123456", new ConnectionStringPointer(), pharmacyMock);

            //Assert
            Assert.AreEqual(ShowPharmacyAlertText, string.Empty);
        }

        [TestMethod]
        public void should_not_populate_sig_alert_if_SigTextLength_greater_than_140_and_nadean_not_given_and_destination_Is_SendToPharmacy_And_Pharmacy_Is_NCPDP2017071()
        {
            //Arrange
            ScriptPad scriptPad = new ScriptPad();
            Rx rx = new Rx();
            rx.SigText = "This is test.";
            rx.Notes = "sample notes XS123456";
            rx.ControlledSubstanceCode = "2";
            rx.PharmacyID = "PHARM12345";
            //Arrange
            var pharmacyMock = MockRepository.GenerateMock<IPharmacy>();
            DataTable table = new DataTable();

            // Declare DataColumn and DataRow variables.
            DataSet ds = new DataSet();
            DataColumn column;
            DataRow row;

            // Create new DataColumn, set DataType, ColumnName and add to DataTable.    
            column = new DataColumn();
            column.ColumnName = "Version";
            table.Columns.Add(column);

            // Create new DataRow objects and add to DataTable.    
            row = table.NewRow();
            row["Version"] = "v6.0";
            table.Rows.Add(row);
            ds.Tables.Add(table);
            pharmacyMock.Stub(x => x.LoadPharmacyById(rx.PharmacyID, new ConnectionStringPointer())).Return(ds);

            //Act
            string ShowPharmacyAlertText = scriptPad.PharmacyAlertMessage(rx, Constants.PharmacySigAlertConstants.SendToPharmacy, string.Empty, new ConnectionStringPointer(), pharmacyMock);

            //Assert
            Assert.AreEqual(ShowPharmacyAlertText, string.Empty);
        }
    }
}
