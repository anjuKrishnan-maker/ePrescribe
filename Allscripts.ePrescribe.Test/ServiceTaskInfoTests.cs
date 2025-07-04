using System;
using System.Linq;
using System.Security.Cryptography;
using System.Web.UI.WebControls;
using System.Xml;
using Allscripts.Impact.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Allscripts.ePrescribe.Test
{
    [TestClass]
    public class ServiceTaskInfoTests
    {
        [TestMethod]
        public void should_return_HasChildren_false_if_no_children()
        {
            var sti = new ServiceTaskInfo();

            Assert.AreEqual(false, sti.HasChildTasks);
        }

        [TestMethod, ExpectedException(typeof(ArgumentException))]
        public void should_throw_exception_if_wrong_child_is_added()
        {
            var task1 = new ServiceTaskInfo()
            {
                PatientID = "C948570F-141B-4FF1-AF77-9E0E30FB32CF",
                LicenseID = "EE5E445F-F883-46B6-8E63-0006A0771126",
                MailboxID = "101",
                MessageData = new XmlDocument(),
                PhysicianID = "102",
                RetryCount = 0,
                ScriptMessageID = "103",
                ServiceTaskID = 104,
                Status = 0
            };

            var task10 = new ServiceTaskInfo()
            {
                PatientID = "3C8B34B2-8CDD-41D4-AC2B-C18DDCAC1312",// different patientID
                LicenseID = "C2DAA01A-D5E0-41FF-A280-7A97D0FAFA39", // different LicenseID
                MailboxID = "401",
                MessageData = new XmlDocument(),
                PhysicianID = "402",
                RetryCount = 0,
                ScriptMessageID = "403",
                ServiceTaskID = 404,
                Status = 0
            };

            task1.AddChildTask(task10);
        }

        [TestMethod, ExpectedException(typeof(ArgumentException))]
        public void should_throw_exception_if_wrong_child_is_added_with_patientID_only_mismatch()
        {
            var task1 = new ServiceTaskInfo()
            {
                PatientID = "C948570F-141B-4FF1-AF77-9E0E30FB32CF",
                LicenseID = "EE5E445F-F883-46B6-8E63-0006A0771126",
                MailboxID = "101",
                MessageData = new XmlDocument(),
                PhysicianID = "102",
                RetryCount = 0,
                ScriptMessageID = "103",
                ServiceTaskID = 104,
                Status = 0
            };

            var task10 = new ServiceTaskInfo()
            {
                PatientID = "3C8B34B2-8CDD-41D4-AC2B-C18DDCAC1312",// different patientID
                LicenseID = "EE5E445F-F883-46B6-8E63-0006A0771126",
                MailboxID = "401",
                MessageData = new XmlDocument(),
                PhysicianID = "402",
                RetryCount = 0,
                ScriptMessageID = "403",
                ServiceTaskID = 404,
                Status = 0
            };

            task1.AddChildTask(task10);
        }

        [TestMethod]
        public void should_add_valid_child_to_parent()
        {
            var task1 = new ServiceTaskInfo()
            {
                PatientID = "C948570F-141B-4FF1-AF77-9E0E30FB32CF",
                LicenseID = "EE5E445F-F883-46B6-8E63-0006A0771126",
                MailboxID = "101",
                MessageData = new XmlDocument(),
                PhysicianID = "102",
                RetryCount = 0,
                ScriptMessageID = "103",
                ServiceTaskID = 104,
                Status = 0
            };

            var task2 = new ServiceTaskInfo()
            {
                PatientID = "C948570F-141B-4FF1-AF77-9E0E30FB32CF",
                LicenseID = "EE5E445F-F883-46B6-8E63-0006A0771126",
                MailboxID = "201",
                MessageData = new XmlDocument(),
                PhysicianID = "202",
                RetryCount = 0,
                ScriptMessageID = "203",
                ServiceTaskID = 204,
                Status = 0
            };

            task1.AddChildTask(task2);

            Assert.AreEqual(true, task1.HasChildTasks);
            Assert.AreEqual(1, task1.ChildTasks.Count());


        }

        [TestMethod]
        public void should_return_license_and_patient_as_grouping_key()
        {
            var task1 = new ServiceTaskInfo()
            {
                PatientID = "C948570F-141B-4FF1-AF77-9E0E30FB32CF",
                LicenseID = "EE5E445F-F883-46B6-8E63-0006A0771126",
                MailboxID = "101",
                MessageData = new XmlDocument(),
                PhysicianID = "102",
                RetryCount = 0,
                ScriptMessageID = "103",
                ServiceTaskID = 104,
                Status = 0
            };

            Assert.AreEqual("EE5E445F-F883-46B6-8E63-0006A0771126_C948570F-141B-4FF1-AF77-9E0E30FB32CF", task1.GroupingKey);
        }
    }
}