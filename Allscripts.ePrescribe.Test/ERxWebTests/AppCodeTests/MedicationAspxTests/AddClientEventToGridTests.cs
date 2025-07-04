using System;
using eRxWeb.AppCode;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.Web.UI;

namespace Allscripts.ePrescribe.Test.ERxWebTests.AppCodeTests.MedicationAspxTests
{
    [TestClass]
    public class AddClientEventToGridTests
    {
        [TestMethod]
        public void should_set_onRowClick_client_event()
        {
            //arrange
            var grid = new RadGrid();

            //act
            MedicationAspx.AddClientRowClickEvent(grid);

            //assert
            Assert.AreEqual("RowClick", grid.ClientSettings.ClientEvents.OnRowClick);
        }
    }
}
