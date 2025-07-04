using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using eRxWeb.AppCode.Tasks;
using Allscripts.Impact.Interfaces;
using Allscripts.Impact;
using Allscripts.ePrescribe.Common;
using System.Data;
using Rhino.Mocks;
using Allscripts.ePrescribe.DatabaseSelector;
using Allscripts.Impact.Tasks;
using ScriptMessage = Allscripts.Impact.ScriptMsg.ScriptMessage;

namespace Allscripts.ePrescribe.Test.TaskingTests.ChangeRxTasksTests
{
    [TestClass]
    public class ProcessRXCHGPriorAuthDenialUnitTests
    {
        private const string xml = "<AllscriptsMessageEnvelope><MessageContent><PrescriptionMessage><Patient><FirstName>Ethan</FirstName><LastName>OBrien</LastName></Patient></PrescriptionMessage></MessageContent></AllscriptsMessageEnvelope>";

        [TestMethod]
        public void should_return_invalid_message_when_null_task_supplied()
        {
            //Arrange
            TaskResponse taskResponseExpected = new TaskResponse
            {
                ExpectedDUR = DURWorkflowExpected.NO,
                ExpectedEPCS = EPCSWorkflowExpected.NO,
                RefillMessage = "Invalid Pharmacy Task"
            };
            ChangeRxTask changeRxTask = new ChangeRxTask();

            //Act
            TaskResponse taskResponseActual = changeRxTask.ProcessRXCHGPriorAuthDenial(null, null, null, null).TaskResponse;

            //Assert
            Assert.AreEqual(taskResponseExpected, taskResponseExpected);
        }
        [TestMethod]        
        public void should_return_nodur_noepcs_message_when_valid_task_supplied()
        {
            //Arrange
            TaskResponse taskResponseExpected = new TaskResponse
            {
                ExpectedDUR = DURWorkflowExpected.NO,
                ExpectedEPCS = EPCSWorkflowExpected.NO,
                RefillMessage = "Invalid Pharmacy Task"
            };
            DataTable table;
            table = new DataTable();
            DataRow row;
            row = table.NewRow();
            DataColumn column1 = new DataColumn();
            DataColumn column2 = new DataColumn();
            column1.ColumnName = "PatientName";
            column2.ColumnName = "lName";
            table.Columns.Add(column1);
            table.Columns.Add(column2);
            // Then add the new row to the collection.
            row["PatientName"] = "John";
            row["lName"] = "Smith";
            table.Rows.Add(row);
            RxTaskModel pharmacyTask = new RxTaskModel();
            pharmacyTask.ScriptMessageGUID = Convert.ToString(new Guid());
            ChangeRxTask changeRxTask = new ChangeRxTask(pharmacyTask);
            //arrange
            var pharmTask = new RxTaskModel { ScriptMessageGUID = "7B360D87-9BEB-433B-835F-7BB9C0FEA66B" };
            var sm = ScriptMessage.CreateScriptMessageForTest(xml);
            var rx = new Rx();
            var newScriptMsg = new Guid("74553B14-4781-4C66-8BA8-C8AFDD1F16DF");

            var scriptMessageMock = MockRepository.GenerateMock<IScriptMessage>();
            scriptMessageMock.Stub(x => x.GetScriptMessage(new Guid(), new Guid(), new Guid(), ConnectionStringPointer.SHARED_DB)).IgnoreArguments().Return(sm);
            scriptMessageMock.Stub(x => x.createRXCHGRES_DenialMessage(null, null, null, null, null, null, null, null, null, null, 0, null, ConnectionStringPointer.SHARED_DB, Constants.IsEPCSMed.NO)).IgnoreArguments().Return(newScriptMsg);

            var rxMock = MockRepository.GenerateMock<IRx>();
            rxMock.Stub(x => x.CreateRx(null)).IgnoreArguments().Return(rx);


            //Act
            TaskResponse taskResponseActual = changeRxTask.ProcessRXCHGPriorAuthDenial(pharmacyTask, row, scriptMessageMock, rxMock).TaskResponse;

            //Assert
            Assert.AreEqual(taskResponseExpected.ExpectedDUR, DURWorkflowExpected.NO);
            Assert.AreEqual(taskResponseExpected.ExpectedEPCS, EPCSWorkflowExpected.NO);
        }
    }
}
