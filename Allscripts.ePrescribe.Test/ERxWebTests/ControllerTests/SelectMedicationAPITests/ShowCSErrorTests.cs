using Allscripts.ePrescribe.Common;
using Allscripts.Impact;
using eRxWeb.Controller;
using eRxWeb.ServerModel;
using eRxWeb.State;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Allscripts.ePrescribe.Test.ERxWebTests.ControllerTests.SelectMedicationAPITests
{
    [TestClass]
    public class ShowCSErrorTests
    {
        [TestMethod, TestCategory("Select Medication")]
        public void should_return_error_msg_based_on_med_cs_schedule()
        {
            // arrange
            var selectMedController = new SelectMedicationApiController();
            MessageModel msgModel = new MessageModel();

            //act
            var csError1 = selectMedController.ShowCSError(msgModel, 1);
            msgModel = new MessageModel();
            var csError2 = selectMedController.ShowCSError(msgModel, 2);
            msgModel = new MessageModel();
            var csError3 = selectMedController.ShowCSError(msgModel, 3);
            msgModel = new MessageModel();
            var csError4 = selectMedController.ShowCSError(msgModel, 4);
            msgModel = new MessageModel();
            var csError5 = selectMedController.ShowCSError(msgModel, 5);
            msgModel = new MessageModel();
            var csErrorOutOfBound = selectMedController.ShowCSError(msgModel, 6);

            //assert
            Assert.AreEqual("Your profile is not set up to prescribe schedule I medications.", csError1.Message);
            Assert.AreEqual("Your profile is not set up to prescribe schedule II medications.", csError2.Message);
            Assert.AreEqual("Your profile is not set up to prescribe schedule III medications.", csError3.Message);
            Assert.AreEqual("Your profile is not set up to prescribe schedule IV medications.", csError4.Message);
            Assert.AreEqual("Your profile is not set up to prescribe schedule V medications.", csError5.Message);
            Assert.AreEqual("Your profile is not set up to prescribe schedule medications.", csErrorOutOfBound.Message);

        }
    }
}
