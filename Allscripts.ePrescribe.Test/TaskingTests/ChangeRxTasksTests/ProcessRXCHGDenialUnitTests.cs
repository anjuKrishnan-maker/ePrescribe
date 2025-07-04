using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using eRxWeb.AppCode.Tasks;
using Allscripts.Impact.Interfaces;
using Allscripts.Impact;
using Allscripts.ePrescribe.Common;
using System.Data;
using System.Xml;
using System.Reflection;
using System.IO;
using System.Text;
using System.Xml.Xsl;
using Allscripts.Impact.Tasks;
using Rhino.Mocks;
using ScriptMessage = Allscripts.Impact.ScriptMsg.ScriptMessage;

namespace Allscripts.ePrescribe.Test.TaskingTests.ChangeRxTasksTests
{
    [TestClass]
    public class ProcessRXCHGDenialUnitTests
    {
        XmlDocument GetInputXmlDoc()
        {
            Assembly thisAssembly = Assembly.GetExecutingAssembly();
            var resourceName = "Allscripts.ePrescribe.Test.TaskingTests.CHGRX_SampleScriptMessageInput.xml";
            string result;
            using (Stream stream = thisAssembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                result = reader.ReadToEnd();
            }
            XmlDocument msgDocInput = new XmlDocument();
            msgDocInput.Load(new StringReader(result));
            return msgDocInput;
        }

        XmlDocument GetExpectedXmlDoc()
        {
            Assembly thisAssembly = Assembly.GetExecutingAssembly();
            string result;
            var resourceName = "Allscripts.ePrescribe.Test.TaskingTests.CHGRX_SampleScriptMessageConverted.xml";
            using (Stream stream = thisAssembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                result = reader.ReadToEnd();
            }
            XmlDocument msgDocExpected = new XmlDocument();
            msgDocExpected.Load(new StringReader(result));
            return msgDocExpected;
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
            var scriptMesssageMock = MockRepository.GenerateMock<IScriptMessage>();
            scriptMesssageMock.Stub(x => x.RejectRXCHGMessage(
                                                    pharmacyTask,
                                                    pharmacyTask.DenialCode,
                                                    pharmacyTask.DenialText,
                                                    Convert.ToString(pharmacyTask.UserId),
                                                    Convert.ToString(pharmacyTask.LicenseId),
                                                    Convert.ToString(pharmacyTask.RxTaskId),
                                                    Convert.ToString(pharmacyTask.ShieldSecurityToken),
                                                    Convert.ToInt32(pharmacyTask.SiteId),
                                                    pharmacyTask.DbId
                                                )).IgnoreArguments().Return("");

            //Act
            TaskResponse taskResponseActual = changeRxTask.ProcessRXCHGDenial(pharmacyTask, row, scriptMesssageMock).TaskResponse;

            //Assert
            Assert.AreEqual(taskResponseExpected.ExpectedDUR, DURWorkflowExpected.NO);
            Assert.AreEqual(taskResponseExpected.ExpectedEPCS, EPCSWorkflowExpected.NO);
        }
        [TestMethod]
        public void should_update_RequestType_when_valid_task_supplied()
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
            XmlDocument msgDocInput = GetInputXmlDoc();
            XmlDocument msgDocExpected = GetExpectedXmlDoc();
            ScriptMessage scriptMessage = new ScriptMessage();
            scriptMessage.LoadDoc(msgDocInput.InnerXml);
            pharmacyTask.ShieldSecurityToken = string.Empty;
            ChangeRxTask changeRxTask = new ChangeRxTask(pharmacyTask);
            var scriptMesssageMock = MockRepository.GenerateMock<IScriptMessage>();
            scriptMesssageMock.Stub(x => x.RejectRXCHGMessage(
                                                    pharmacyTask,
                                                    pharmacyTask.DenialCode,
                                                    pharmacyTask.DenialText,
                                                    Convert.ToString(pharmacyTask.UserId),
                                                    Convert.ToString(pharmacyTask.LicenseId),
                                                    Convert.ToString(pharmacyTask.RxTaskId),
                                                    Convert.ToString(pharmacyTask.ShieldSecurityToken),
                                                    Convert.ToInt32(pharmacyTask.SiteId),
                                                    pharmacyTask.DbId
                                                )).IgnoreArguments().Return("");

            //Act
            RxTaskModel responseTask = changeRxTask.ProcessRXCHGDenial(pharmacyTask, row, scriptMesssageMock);

            //Assert
            Assert.AreEqual(responseTask.RxRequestType, RequestType.DENY);
        }
    }
}
