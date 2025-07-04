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
    public class ProcessRxChgPriorAuthApprovalUnitTests
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
        public void should_return_when_PriorAuthCode_Is_Wrong()
        {
            //Arrange
            TaskResponse taskResponseExpected = new TaskResponse
            {
                ExpectedDUR = DURWorkflowExpected.NO,
                ExpectedEPCS = EPCSWorkflowExpected.NO,
                RefillMessage = Constants.TaskingMessages.PRIOR_AUTH_CODE_ERROR
            };
            RxTaskModel pharmacyTask = new RxTaskModel();
            pharmacyTask.PriorAuthCode = "UNKNOWN";
            pharmacyTask.ScriptMessageGUID = Convert.ToString(new Guid());
            XmlDocument msgDocInput = GetInputXmlDoc();
            XmlDocument msgDocExpected = GetExpectedXmlDoc();
            ScriptMessage scriptMessage = new ScriptMessage();
            scriptMessage.LoadDoc(msgDocInput.InnerXml);
            pharmacyTask.ScriptMessage = scriptMessage;
            ChangeRxTask changeRxTask = new ChangeRxTask(pharmacyTask);

            //Act
            TaskResponse taskResponseActual = changeRxTask.ProcessRXCHGPriorAuthApproval().TaskResponse;

            //Assert
            Assert.AreEqual(taskResponseExpected, taskResponseExpected);
        }
        [TestMethod]
        public void should_return_invalid_task_when_null_PharmacyTask_supplied()
        {
            //Arrange
            TaskResponse taskResponseExpected = new TaskResponse
            {
                ExpectedDUR = DURWorkflowExpected.NO,
                ExpectedEPCS = EPCSWorkflowExpected.NO,
                RefillMessage = "ChangeRxTask ProcessRXCHGPriorAuthApproval: invalid task."
            };
            ChangeRxTask changeRxTask = new ChangeRxTask(null);

            //Act
            TaskResponse taskResponseActual = changeRxTask.ProcessRXCHGPriorAuthApproval().TaskResponse;

            //Assert
            Assert.AreEqual(taskResponseExpected, taskResponseExpected);
        }
    }
}
