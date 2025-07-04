using System;
using System.Data;
using Allscripts.ePrescribe.Common;
using Allscripts.Impact;
using Allscripts.Impact.Interfaces;
using Allscripts.Impact.ScriptMsg;
using Allscripts.Impact.ScriptMsg.AmeMomDataModel;
using eRxWeb.AppCode.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using ScriptMessage = Allscripts.ePrescribe.Objects.ScriptMessage;

namespace Allscripts.ePrescribe.Test.ImpactTests.ScriptMessageTests
{
    [TestClass]
    public class SwitchNodeCreateUnitTests
    {
        [TestMethod]
        public void should_return_SwitchNode_with_no_fields_populated_if_datarow_is_null()
        {
            //Arrange

            //Act
            SwitchModel switchModel = SwitchModel.Create(null, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, new Audit());

            //Assert
            Assert.AreEqual(switchModel.AppId, null);
        }

        [TestMethod]
        public void should_return_SwitchNode_with_calculated_IsItEPCS_field_()
        {
            //Arrange/ Create new DataTable and DataSource objects.
            DataTable table = new DataTable();
            table.TableName = "Switch";

            // Declare DataColumn and DataRow variables.
            DataColumn column;
            DataRow row;

            // Create new DataColumn, set DataType, ColumnName and add to DataTable.    
            column = new DataColumn();
            column.ColumnName = "AppID";
            table.Columns.Add(column);
            // Create new DataColumn, set DataType, ColumnName and add to DataTable.    
            column = new DataColumn();
            column.ColumnName = "RecipientID";
            table.Columns.Add(column);
            // Create new DataColumn, set DataType, ColumnName and add to DataTable.    
            column = new DataColumn();
            column.ColumnName = "ScriptSwID";
            table.Columns.Add(column);
            // Create new DataColumn, set DataType, ColumnName and add to DataTable.    
            column = new DataColumn();
            column.ColumnName = "SenderID";
            table.Columns.Add(column);
            // Create new DataColumn, set DataType, ColumnName and add to DataTable.    
            column = new DataColumn();
            column.ColumnName = "TestIndicator";
            table.Columns.Add(column);
            // Create new DataColumn, set DataType, ColumnName and add to DataTable.    
            column = new DataColumn();
            column.ColumnName = "MessageReferenceNumber";
            table.Columns.Add(column);

            // Create new DataRow objects and add to DataTable.    
            row = table.NewRow();
            row["AppID"] = "1.0";
            row["RecipientID"] = "000-000-000";
            row["ScriptSwID"] = "2.0";
            row["SenderID"] = "111-111-111";
            row["TestIndicator"] = "T1.0";
            row["MessageReferenceNumber"] = "InDbMRN";
            table.Rows.Add(row);

            //Mock some dependency data
            var contextAuditMock = MockRepository.GenerateStub<IContextualAudit>();
            contextAuditMock.Stub(x => x.AddException("Blah")).IgnoreArguments().Return("Blah");

            //Act
            SwitchModel switchModel = SwitchModel.Create(row, Constants.MessageTypes.REFILL_RESPONSE, string.Empty, string.Empty, "OriginalMRN", string.Empty, string.Empty, string.Empty, "2", string.Empty, "4/19/2019 6:58:39 PM", contextAuditMock);

            //Assert
            Assert.AreEqual("2019-04-19T18:04:39-04:00", switchModel.SentDateTime);
            Assert.AreEqual("Y",switchModel.IsItEPCS);
        }

        [TestMethod]
        public void should_return_SwitchNode_with_sent_date_fields_formatted()
        {
            //Arrange/ Create new DataTable and DataSource objects.
            DataTable table = new DataTable();
            table.TableName = "Switch";

            // Declare DataColumn and DataRow variables.
            DataColumn column;
            DataRow row;

            // Create new DataColumn, set DataType, ColumnName and add to DataTable.    
            column = new DataColumn();
            column.ColumnName = "AppID";
            table.Columns.Add(column);
            // Create new DataColumn, set DataType, ColumnName and add to DataTable.    
            column = new DataColumn();
            column.ColumnName = "RecipientID";
            table.Columns.Add(column);
            // Create new DataColumn, set DataType, ColumnName and add to DataTable.    
            column = new DataColumn();
            column.ColumnName = "ScriptSwID";
            table.Columns.Add(column);
            // Create new DataColumn, set DataType, ColumnName and add to DataTable.    
            column = new DataColumn();
            column.ColumnName = "SenderID";
            table.Columns.Add(column);
            // Create new DataColumn, set DataType, ColumnName and add to DataTable.    
            column = new DataColumn();
            column.ColumnName = "TestIndicator";
            table.Columns.Add(column);
            // Create new DataColumn, set DataType, ColumnName and add to DataTable.    
            column = new DataColumn();
            column.ColumnName = "MessageReferenceNumber";
            table.Columns.Add(column);

            // Create new DataRow objects and add to DataTable.    
            row = table.NewRow();
            row["AppID"] = "1.0";
            row["RecipientID"] = "000-000-000";
            row["ScriptSwID"] = "2.0";
            row["SenderID"] = "111-111-111";
            row["TestIndicator"] = "T1.0";
            row["MessageReferenceNumber"] = "InDbMRN";
            table.Rows.Add(row);

            //Mock some dependency data
            var contextAuditMock = MockRepository.GenerateStub<IContextualAudit>();
            contextAuditMock.Stub(x => x.AddException("Blah")).IgnoreArguments().Return("Blah");

            //Act
            SwitchModel switchModel = SwitchModel.Create(row, Constants.MessageTypes.REFILL_RESPONSE, string.Empty, string.Empty, "OriginalMRN", string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, "4/19/2019 6:58:39 PM", contextAuditMock);

            //Assert
            Assert.AreEqual(switchModel.AppId, "1.0");
            Assert.AreEqual(switchModel.AppVersion, string.Empty);

            Assert.AreEqual(switchModel.SentDateTime, "2019-04-19T18:04:39-04:00");
        }

        [TestMethod]
        public void should_return_SwitchNode_with_fields_populated_from_datarow()
        {
            //Arrange/ Create new DataTable and DataSource objects.
            DataTable table = new DataTable();
            table.TableName = "Switch";

            // Declare DataColumn and DataRow variables.
            DataColumn column;
            DataRow row;

            // Create new DataColumn, set DataType, ColumnName and add to DataTable.    
            column = new DataColumn();
            column.ColumnName = "AppID";
            table.Columns.Add(column);
            // Create new DataColumn, set DataType, ColumnName and add to DataTable.    
            column = new DataColumn();
            column.ColumnName = "RecipientID";
            table.Columns.Add(column);
            // Create new DataColumn, set DataType, ColumnName and add to DataTable.    
            column = new DataColumn();
            column.ColumnName = "ScriptSwID";
            table.Columns.Add(column);
            // Create new DataColumn, set DataType, ColumnName and add to DataTable.    
            column = new DataColumn();
            column.ColumnName = "SenderID";
            table.Columns.Add(column);
            // Create new DataColumn, set DataType, ColumnName and add to DataTable.    
            column = new DataColumn();
            column.ColumnName = "TestIndicator";
            table.Columns.Add(column);
            // Create new DataColumn, set DataType, ColumnName and add to DataTable.    
            column = new DataColumn();
            column.ColumnName = "MessageReferenceNumber";
            table.Columns.Add(column);

            // Create new DataRow objects and add to DataTable.    
            row = table.NewRow();
            row["AppID"] = "1.0";
            row["RecipientID"] = "000-000-000";
            row["ScriptSwID"] = "2.0";
            row["SenderID"] = "111-111-111";
            row["TestIndicator"] = "T1.0";
            row["MessageReferenceNumber"] = "InDbMRN";
            table.Rows.Add(row);

            //Mock some dependency data
            var contextAuditMock = MockRepository.GenerateStub<IContextualAudit>();
            contextAuditMock.Stub(x => x.AddException("Blah")).IgnoreArguments().Return("Blah");
            
            //Act
            SwitchModel switchModel = SwitchModel.Create(row, Constants.MessageTypes.REFILL_RESPONSE, string.Empty, string.Empty, "OriginalMRN", string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, "4/19/2019 6:58:39 PM", contextAuditMock);



            //Assert
            Assert.AreEqual(switchModel.AppId, "1.0");
            Assert.AreEqual(switchModel.AppVersion, string.Empty);
            Assert.AreEqual(switchModel.MessageReferenceNumber, "OriginalMRN");
            Assert.AreEqual(switchModel.RecipientId, "000-000-000");
            Assert.AreEqual(switchModel.ScriptSwId, "2.0");
            Assert.AreEqual(switchModel.SenderId, "111-111-111");
            Assert.AreEqual(switchModel.TestIndicator, "T1.0");
            Assert.AreEqual(switchModel.SentDateTime, "2019-04-19T18:04:39-04:00");
        }
    }
}
