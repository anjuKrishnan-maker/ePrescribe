using System;
using Rx = Allscripts.Impact.Rx;
using ScriptMessage = Allscripts.Impact.ScriptMsg.ScriptMessage;
using Allscripts.Impact;
using Allscripts.Impact.Interfaces;
using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.Objects;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using eRxWeb.AppCode;
using System.Collections.Generic;

namespace Allscripts.ePrescribe.Test.ERxWebTests.AppCodeTests.ApproveRefillTaskTests
{
    [TestClass]
    public class ShouldLaunchEPCSWorkflowTests
    {
        [TestMethod]
        public void should_Return_False_If_Null_RxList_Supplied()
        {
            //Arrange
            List<Rx> rxList = new List<Rx>();

            //Act
            bool bActual = new EPCSWorkflowUtils().ShouldLaunchEPCSWorkflow(false, null);

            //Assert
            Assert.AreEqual(false, bActual);

        }

        [TestMethod]
        public void should_Return_True_If_Valid_RxList_Supplied()
        {
            //Arrange
            List<Rx> rxList = new List<Rx>();
            Rx rx = new Rx();
            rxList.Add(rx);

            //Act

            bool bActual = new EPCSWorkflowUtils().ShouldLaunchEPCSWorkflow(true, rxList);

            //Assert
            Assert.AreEqual(true, bActual);

        }
    }

    [TestClass]
    public class EpcsMedListWorkflowTests
    {
        [TestMethod]
        public void should_Return_Valid_List_If_Valid_ChangeRx_Rx_Supplied()
        {
            //Arrange
            Rx rx = new Rx();
            List<Rx> rxList = new List<Rx>();

            List<Rx> rxListExpected = new List<Rx>();
            rxListExpected.Add(rx);

            //Act
            rxList = new EPCSWorkflowUtils().UpdateAndReturnEpcsMedList(rxList, rx);

            //Assert
            Assert.AreEqual(rxListExpected[0], rxList[0]);
            Assert.AreEqual(rxListExpected.Count, rxList.Count);

        }

        [TestMethod, ExpectedException(typeof(ArgumentException))]
        public void should_Throw_Exception_If_Null_ChangeRx_Rx_Supplied()
        {
            //Arrange
            Rx rx = null;
            List<Rx> rxList = new List<Rx>();

            List<Rx> rxListExpected = new List<Rx>();

            //Act
            rxList = new EPCSWorkflowUtils().UpdateAndReturnEpcsMedList(rxList, rx);

        }

        [TestMethod]
        public void should_return_pharm_and_rx_details_for_redirect_to_select_med()
        {
            //arrange 
            var pharmDetails = "Brooklyn @ Gates Pharmacy, 92 Gates Ave , Brooklyn, NY, (347) 425-1221";
            var rxDetails = "Amphetamine-Dextroamphetamine 10 MG Oral Tablet, SIG: TAKE 1 TABLET DAILY., Quantity: 15, Days Supply: 15, Fills: 15, Original Rx Date: 7/15/2019, Last Fill Date: 7/15/2019";
            var drugName = "Amphetamine-Dextroamphetamine 10 MG Oral Tablet";
            var expected = "RedirectToAngular.aspx?componentName=SelectMedication&componentParameters={%22RefillPharmacy%22%3a+%22Brooklyn+%40+Gates+Pharmacy%2c+92+Gates+Ave+%2c+Brooklyn%2c+NY%2c+(347)+425-1221%22%2c%22RxDetails%22%3a+%22Amphetamine-Dextroamphetamine+10+MG+Oral+Tablet%2c+SIG%3a+TAKE+1+TABLET+DAILY.%2c+Quantity%3a+15%2c+Days+Supply%3a+15%2c+Fills%3a+15%2c+Original+Rx+Date%3a+7%2f15%2f2019%2c+Last+Fill+Date%3a+7%2f15%2f2019%22%2c%22from%22%3a+%22ApproveRefillTask.aspx%22%2c%22SearchText%22%3a+%22Amphetamine-Dextroamphetamine%22}";

            //act 
            var result = CsMedUtil.RedirectForCSMed(pharmDetails, rxDetails, drugName);

            //assert
            Assert.AreEqual(expected, result);
        }
    }
}
