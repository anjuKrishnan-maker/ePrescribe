using System;
using Allscripts.ePrescribe.Common;
using eRxWeb;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Allscripts.ePrescribe.Test.ERxWebTests.AppCodeTests.EPSBrokerTests
{
    [TestClass]
    public class ConvertEnumFromCommonToEPSTests
    {
        [TestMethod]
        public void should_convert_approval_request_same_id()
        {
            var taskType = Constants.PrescriptionTaskType.APPROVAL_REQUEST;
            var taskTypeId = Convert.ToInt16(taskType);

            var castedTask = EPSBroker.ConvertEnumFromCommonToEPS(taskType);
            var castedTaskId = Convert.ToInt16(castedTask);

            Assert.AreEqual(taskTypeId, castedTaskId);
            Assert.AreEqual(Convert.ToString(taskType), Convert.ToString(castedTask));
        }

        [TestMethod]
        public void should_convert_rejection_notice_same_id()
        {
            var taskType = Constants.PrescriptionTaskType.REJECTION_NOTIFICATION;
            var taskTypeId = Convert.ToInt16(taskType);

            var castedTask = EPSBroker.ConvertEnumFromCommonToEPS(taskType);
            var castedTaskId = Convert.ToInt16(castedTask);

            Assert.AreEqual(taskTypeId, castedTaskId);
            Assert.AreEqual(Convert.ToString(taskType), Convert.ToString(castedTask));
        }

        [TestMethod]
        public void should_convert_renewal_request_same_id()
        {
            var taskType = Constants.PrescriptionTaskType.RENEWAL_REQUEST;
            var taskTypeId = Convert.ToInt16(taskType);

            var castedTask = EPSBroker.ConvertEnumFromCommonToEPS(taskType);
            var castedTaskId = Convert.ToInt16(castedTask);

            Assert.AreEqual(taskTypeId, castedTaskId);
            Assert.AreEqual(Convert.ToString(taskType), Convert.ToString(castedTask));
        }

        [TestMethod]
        public void should_convert_script_error_same_id()
        {
            var taskType = Constants.PrescriptionTaskType.SCRIPT_ERROR;
            var taskTypeId = Convert.ToInt16(taskType);

            var castedTask = EPSBroker.ConvertEnumFromCommonToEPS(taskType);
            var castedTaskId = Convert.ToInt16(castedTask);

            Assert.AreEqual(taskTypeId, castedTaskId);
            Assert.AreEqual(Convert.ToString(taskType), Convert.ToString(castedTask));
        }

        [TestMethod]
        public void should_convert_send_to_admin_same_id()
        {
            var taskType = Constants.PrescriptionTaskType.SEND_TO_ADMIN;
            var taskTypeId = Convert.ToInt16(taskType);

            var castedTask = EPSBroker.ConvertEnumFromCommonToEPS(taskType);
            var castedTaskId = Convert.ToInt16(castedTask);

            Assert.AreEqual(taskTypeId, castedTaskId);
            Assert.AreEqual(Convert.ToString(taskType), Convert.ToString(castedTask));
        }

        [TestMethod]
        public void should_convert_refreq_same_id()
        {
            var taskType = Constants.PrescriptionTaskType.REFREQ;
            var taskTypeId = Convert.ToInt16(taskType);

            var castedTask = EPSBroker.ConvertEnumFromCommonToEPS(taskType);
            var castedTaskId = Convert.ToInt16(castedTask);

            Assert.AreEqual(taskTypeId, castedTaskId);
            Assert.AreEqual(Convert.ToString(taskType), Convert.ToString(castedTask));
        }

        [TestMethod]
        public void should_convert_epa_same_id()
        {
            var taskType = Constants.PrescriptionTaskType.EPA;
            var taskTypeId = Convert.ToInt16(taskType);

            var castedTask = EPSBroker.ConvertEnumFromCommonToEPS(taskType);
            var castedTaskId = Convert.ToInt16(castedTask);

            Assert.AreEqual(taskTypeId, castedTaskId);
            Assert.AreEqual(Convert.ToString(taskType), Convert.ToString(castedTask));
        }

        [TestMethod]
        public void should_convert_changerx()
        {
            var taskType = Constants.PrescriptionTaskType.RXCHG;
            var taskTypeId = Convert.ToInt16(taskType);

            var castedTask = EPSBroker.ConvertEnumFromCommonToEPS(taskType);
            var castedTaskId = Convert.ToInt16(castedTask);

            Assert.AreNotEqual(taskTypeId, castedTaskId);
            Assert.AreEqual(Convert.ToString(taskType), Convert.ToString(castedTask));
        }

        [TestMethod]
        public void should_convert_prior_auth()
        {
            var taskType = Constants.PrescriptionTaskType.RXCHG_PRIORAUTH;
            var taskTypeId = Convert.ToInt16(taskType);

            var castedTask = EPSBroker.ConvertEnumFromCommonToEPS(taskType);
            var castedTaskId = Convert.ToInt16(castedTask);

            Assert.AreNotEqual(taskTypeId, castedTaskId);
            Assert.AreEqual(Convert.ToString(taskType), Convert.ToString(castedTask));
        }

        [TestMethod]
        public void should_convert_specialty_med_assist()
        {
            var taskType = Constants.PrescriptionTaskType.SPECIALTY_MED_ASSIST_RX;
            var taskTypeId = Convert.ToInt16(taskType);

            var castedTask = EPSBroker.ConvertEnumFromCommonToEPS(taskType);
            var castedTaskId = Convert.ToInt16(castedTask);

            Assert.AreNotEqual(taskTypeId, castedTaskId);
            Assert.AreEqual(Convert.ToString(taskType), Convert.ToString(castedTask));
        }

        [TestMethod]
        public void should_convert_default()
        {
            var taskType = Constants.PrescriptionTaskType.DEFAULT;
            var taskTypeId = Convert.ToInt16(taskType);

            var castedTask = EPSBroker.ConvertEnumFromCommonToEPS(taskType);
            var castedTaskId = Convert.ToInt16(castedTask);

            Assert.AreNotEqual(taskTypeId, castedTaskId);
            Assert.AreEqual(Convert.ToString(taskType), Convert.ToString(castedTask));
        }
    }
}
