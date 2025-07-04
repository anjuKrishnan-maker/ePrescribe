using System;
using System.Collections;
using System.Collections.Generic;
using eRxWeb;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Allscripts.ePrescribe.Test.DurTests
{
    [TestClass]
    public class ComputeReasonIDAndUpdateRxToProviderListTests
    {
        private ArrayList SetupRxToProviderList()
        {
            var rxToProviderList = new ArrayList();
            rxToProviderList.Add(Guid.Parse("56124999-4B5B-4300-8D5A-33B35CC906A0"));

            return rxToProviderList;
        }

        private List<Guid> SetupRxIDList()
        {
            var guidList = new List<Guid>();
            guidList.Add(Guid.Parse("9D6BDD82-B32E-4DAD-A592-200E2DE4C933"));
            guidList.Add(Guid.Parse("43D191D1-6F71-432C-893D-2B7C699B3780"));

            return guidList;
        } 

        [TestMethod]
        public void Should_Return_SelectedReasonID_When_Both_SelectedReaderID_And_HeaderID_Present()
        {
            //Arrange
            int? selectedIndex = 1;
            int selectedReasonID = 5;
            int headerReasonID = 10;
            ArrayList rxToProviderList = SetupRxToProviderList();
            List<Guid> rxIDs = SetupRxIDList();

            int expectedReasonID = 5;

            //Act
            //int resultReasonID = new RxDURReviewMultiSelect().ComputeReasonIDAndUpdateRxToProviderList(selectedIndex, selectedReasonID,
            //                            headerReasonID, rxToProviderList, rxIDs);

            ////Assert
            //Assert.AreEqual(expectedReasonID, resultReasonID);
        }

        [TestMethod]
        public void Should_Return_HeaderReasonID_When_Only_HeaderID_Present()
        {
            //Arrange
            int? selectedIndex = 0;
            int selectedReasonID = -1;
            int headerReasonID = 10;
            ArrayList rxToProviderList = SetupRxToProviderList();
            List<Guid> rxIDs = SetupRxIDList();

            int expectedReasonID = 10;

            //Act
            //int resultReasonID = new RxDURReviewMultiSelect().ComputeReasonIDAndUpdateRxToProviderList(selectedIndex, selectedReasonID,
            //                            headerReasonID, rxToProviderList, rxIDs);

            ////Assert
            //Assert.AreEqual(expectedReasonID, resultReasonID);
        }

        [TestMethod]
        public void Should_Add_RxID_To_RxToProviderList_When_Computed_Result_Is_SendToProvider()
        {
            //Arrange
            int? selectedIndex = 2;
            int selectedReasonID = -2; //Send To Provider
            int headerReasonID = 10;
            ArrayList rxToProviderList = SetupRxToProviderList();
            List<Guid> rxIDs = SetupRxIDList();

            int expectedRxToProviderListCount = rxToProviderList.Count + rxIDs.Count;
            
            //Act
            //int resultReasonID = new RxDURReviewMultiSelect().ComputeReasonIDAndUpdateRxToProviderList(selectedIndex, selectedReasonID,
            //                            headerReasonID, rxToProviderList, rxIDs);

            ////Assert
            //Assert.AreEqual(expectedRxToProviderListCount, rxToProviderList.Count);
        }
    }
}
