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

namespace Allscripts.ePrescribe.Test.TaskingTests.ChangeRxTasksTests
{
    [TestClass]
    public class ProcessRXCHGApprovalUnitTests
    {

        [TestMethod]
        public void should_return_invalid_task_when_null_PharmacyTask_supplied()
        {
            //Arrange
            TaskResponse taskResponseExpected = new TaskResponse
            {
                ExpectedDUR = DURWorkflowExpected.NO,
                ExpectedEPCS = EPCSWorkflowExpected.NO,
                RefillMessage = "ChangeRxTask ProcessRXCHGApproval: invalid task."
            };
            ChangeRxTask changeRxTask = new ChangeRxTask(null);

            //Act
            TaskResponse taskResponseActual = changeRxTask.ProcessRXCHGApproval().TaskResponse;

            //Assert
            Assert.AreEqual(taskResponseExpected, taskResponseExpected);
        }
    }
    [TestClass]
    public class ProcessRXCHGDURUnitTests
    {

        [TestMethod]
        public void should_return_invalid_task_when_null_PharmacyTask_supplied()
        {
            //Arrange
            TaskResponse taskResponseExpected = new TaskResponse
            {
                ExpectedDUR = DURWorkflowExpected.NO,
                ExpectedEPCS = EPCSWorkflowExpected.NO,
                RefillMessage = "ChangeRxTask ProcessRxCHGDUR: invalid task."
            };
            ChangeRxTask changeRxTask = new ChangeRxTask(null);

            //Act
            TaskResponse taskResponseActual = changeRxTask.ProcessRxCHGDUR(DURWorkflowExpected.YES).TaskResponse;

            //Assert
            Assert.AreEqual(taskResponseExpected, taskResponseExpected);
        }
        [TestMethod]
        public void should_return_DURParameters_filled_task_when_true_dur_supplied()
        {
            //Arrange
            TaskResponse taskResponseExpected = new TaskResponse
            {
                ExpectedDUR = DURWorkflowExpected.YES,
                ExpectedEPCS = EPCSWorkflowExpected.UNKNOWN,
                RefillMessage = string.Empty
            };
            ChangeRxTask changeRxTask = new ChangeRxTask(new RxTaskModel());

            //Act
            TaskResponse taskResponseActual = changeRxTask.ProcessRxCHGDUR(DURWorkflowExpected.YES).TaskResponse;

            //Assert
            Assert.AreEqual(taskResponseExpected, taskResponseExpected);
        }
    }
    [TestClass]
    public class ProcessRXCHGEPCSUnitTests
    {

        [TestMethod]
        public void should_return_invalid_task_when_null_PharmacyTask_supplied()
        {
            //Arrange
            TaskResponse taskResponseExpected = new TaskResponse
            {
                ExpectedDUR = DURWorkflowExpected.NO,
                ExpectedEPCS = EPCSWorkflowExpected.NO,
                RefillMessage = "ChangeRxTask ProcessRxCHGEPCS: invalid task."
            };
            ChangeRxTask changeRxTask = new ChangeRxTask(null);

            //Act
            TaskResponse taskResponseActual = changeRxTask.ProcessRxCHGEPCS(true, EPCSWorkflowExpected.YES).TaskResponse;

            //Assert
            Assert.AreEqual(taskResponseExpected, taskResponseExpected);
        }
        [TestMethod]
        public void should_not_approveRXCHG_and_return_EPCSParameters_filled_task_when_CSmed_supplied_and_EPCSWorkflow_Expected()
        {
            //Arrange
            TaskResponse taskResponseExpected = new TaskResponse
            {
                ExpectedDUR = DURWorkflowExpected.UNKNOWN,
                ExpectedEPCS = EPCSWorkflowExpected.YES,
                RefillMessage = "Change Request for SampleMed approved for Ankit Singh."
            };
            RxTaskModel pharmacyTask = new RxTaskModel();
            pharmacyTask.IsControlledSubstance = false;
            pharmacyTask.Rx = new Rx();
            pharmacyTask.Rx.MedicationName = "SampleMed";
            pharmacyTask.PatientFirstName = "Ankit";
            pharmacyTask.PatientLastName = "Singh";
            ChangeRxTask changeRxTask = new ChangeRxTask(pharmacyTask);

            //Act
            TaskResponse taskResponseActual = changeRxTask.ProcessRxCHGEPCS(true, EPCSWorkflowExpected.YES).TaskResponse;

            //Assert
            Assert.AreEqual(taskResponseExpected, taskResponseExpected);
        }
        [TestMethod]
        public void should_not_approveRXCHG_and_return_EPCSParameters_filled_task_when_CSmed_supplied_and_not_EPCSWorkflow_Expected()
        {
            //Arrange
            TaskResponse taskResponseExpected = new TaskResponse
            {
                ExpectedDUR = DURWorkflowExpected.UNKNOWN,
                ExpectedEPCS = EPCSWorkflowExpected.NO,
                RefillMessage = "Change Request for SampleMed cannot be approved for Ankit Singh."
            };
            RxTaskModel pharmacyTask = new RxTaskModel();
            pharmacyTask.IsControlledSubstance = false;
            pharmacyTask.Rx = new Rx();
            pharmacyTask.Rx.MedicationName = "SampleMed";
            pharmacyTask.PatientFirstName = "Ankit";
            pharmacyTask.PatientLastName = "Singh";
            ChangeRxTask changeRxTask = new ChangeRxTask(pharmacyTask);

            //Act
            TaskResponse taskResponseActual = changeRxTask.ProcessRxCHGEPCS(true, EPCSWorkflowExpected.NO).TaskResponse;

            //Assert
            Assert.AreEqual(taskResponseExpected, taskResponseExpected);
        }
    }
}
