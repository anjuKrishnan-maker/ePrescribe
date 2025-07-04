using System.Linq;
using System.Xml;
using Allscripts.Impact;
using Allscripts.Impact.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Allscripts.ePrescribe.Test
{
    [TestClass]
    public class TaskManagerGroupRawTasksTest
    {
        private ServiceTaskInfo task1 = new ServiceTaskInfo()
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

        private ServiceTaskInfo task2 = new ServiceTaskInfo()
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

        private ServiceTaskInfo task3 = new ServiceTaskInfo()
        {
            PatientID = "C948570F-141B-4FF1-AF77-9E0E30FB32CF",
            LicenseID = "EE5E445F-F883-46B6-8E63-0006A0771126",
            MailboxID = "301",
            MessageData = new XmlDocument(),
            PhysicianID = "302",
            RetryCount = 0,
            ScriptMessageID = "303",
            ServiceTaskID = 304,
            Status = 0
        };

        private ServiceTaskInfo task10 = new ServiceTaskInfo()
        {
            PatientID = "3C8B34B2-8CDD-41D4-AC2B-C18DDCAC1312",
            LicenseID = "C2DAA01A-D5E0-41FF-A280-7A97D0FAFA39",
            MailboxID = "401",
            MessageData = new XmlDocument(),
            PhysicianID = "402",
            RetryCount = 0,
            ScriptMessageID = "403",
            ServiceTaskID = 404,
            Status = 0
        };


        private ServiceTaskInfo task11 = new ServiceTaskInfo()
        {
            PatientID = "3C8B34B2-8CDD-41D4-AC2B-C18DDCAC1312",
            LicenseID = "C2DAA01A-D5E0-41FF-A280-7A97D0FAFA39",
            MailboxID = "501",
            MessageData = new XmlDocument(),
            PhysicianID = "502",
            RetryCount = 0,
            ScriptMessageID = "503",
            ServiceTaskID = 504,
            Status = 0
        };
 
 
        [TestMethod]
        public void should_return_single_parent_if_all_tasks_are_siblings()
        {
            // arrange

            var flatTasks = new[] {task1, task2, task3}; 
            
            //act

            var tasks = TaskManager.GroupRawTasks(flatTasks);

            // assert

            Assert.AreEqual(1, tasks.Count());
            Assert.AreEqual(true, tasks.First().HasChildTasks);
            Assert.AreEqual(3, tasks.First().ChildTasks.Count());
        }

        [TestMethod]
        public void should_return_two_parents_if_all_tasks_are_from_two_patients_multiple_child_tasks()
        {
            // arrange

            var flatTasks = new[] { task1, task2, task10, task11};

            //act

            var tasks = TaskManager.GroupRawTasks(flatTasks);

            // assert

            Assert.AreEqual(2, tasks.Count());
            Assert.AreEqual(true, tasks.First().HasChildTasks);
            Assert.AreEqual(2, tasks.First().ChildTasks.Count());
        }

        [TestMethod]
        public void should_return_two_parents_if_all_tasks_are_from_two_patients_single_child_task()
        {
            // arrange

            var flatTasks = new[] { task1, task10};

            //act

            var tasks = TaskManager.GroupRawTasks(flatTasks);

            // assert

            Assert.AreEqual(2, tasks.Count());
            Assert.AreEqual(true, tasks.First().HasChildTasks);
            Assert.AreEqual(1, tasks.First().ChildTasks.Count());
        }
    }
}