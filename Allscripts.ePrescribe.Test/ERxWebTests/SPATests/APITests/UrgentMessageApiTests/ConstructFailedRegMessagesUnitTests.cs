using System;
using System.Collections.Generic;
using System.Data;
using Allscripts.ePrescribe.DatabaseSelector;
using Allscripts.ePrescribe.Objects;
using Allscripts.ePrescribe.Objects.StoredProcReturnObjects;
using Allscripts.Impact.Interfaces;
using eRxWeb;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using Rhino.Mocks.Constraints;
using eRxWeb.ServerModel;
using eRxWeb.Controllers;

namespace Allscripts.ePrescribe.Test.ERxWebTests.SPATests.APITests.UrgentMessageApiTests
{
    [TestClass]
    public class ConstructFailedRegMessagesUnitTests
    {
        [TestMethod]
        public void should_return_an_empty_list_if_RegMessages_DataTable_is_null()
        {
            //arrange
            DataTable dtFailedRegMessages = null;

            //act
            var result = UrgentMessageApiController.ConstructFailedRegMessages(dtFailedRegMessages);

            //assert
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void should_return_an_empty_list_if_RegMessages_DataTable_is_empty_but_not_null()
        {
            //arrange
            DataTable dtFailedRegMessages = new DataTable();

            //act
            var result = UrgentMessageApiController.ConstructFailedRegMessages(dtFailedRegMessages);

            //assert
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void should_return_a_valid_list_if_RegMessages_DataTable_is_not_empty_and_not_null()
        {
            //arrange
            List<FaildRegMsg> failedRegMessagesList = new List<FaildRegMsg>();
            DataTable dtFailedRegMessages = new DataTable();
            dtFailedRegMessages.Columns.Add("FirstName", typeof(string));
            dtFailedRegMessages.Columns.Add("LastName", typeof(string));
            dtFailedRegMessages.Columns.Add("RequestCreated", typeof(DateTime));
            dtFailedRegMessages.Columns.Add("RequestID", typeof(string));

            DataRow dr = dtFailedRegMessages.NewRow();
            dr["FirstName"] = "Ankit";
            dr["LastName"] = "Singh";
            dr["RequestCreated"] = "1/1/2016 12:00:00 AM";
            dr["RequestID"] = "012345";

            dtFailedRegMessages.Rows.Add(dr);
            

            //act
            var result = UrgentMessageApiController.ConstructFailedRegMessages(dtFailedRegMessages);

            //assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Ankit Singh", result[0].Name);
            Assert.AreEqual("1/1/2016 12:00:00 AM", result[0].RequestCreated);
            Assert.AreEqual("012345", result[0].RequestID);
        }

    }
}
