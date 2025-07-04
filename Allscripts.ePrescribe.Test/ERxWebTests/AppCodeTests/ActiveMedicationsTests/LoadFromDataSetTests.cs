using System;
using System.Text;
using System.Data;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Allscripts.ePrescribe.Data;
using Allscripts.ePrescribe.DatabaseSelector;
using Allscripts.Impact;
using Rhino.Mocks;
using eRxWeb;
using eRxWeb.State;
using eRxWeb.AppCode;

namespace Allscripts.ePrescribe.Test.ERxWebTests.AppCodeTests.BasePageTests
{
    [TestClass]
    public class LoadFromDataSetTests
    {

        [TestMethod]
        public void updateStringBuilderList_should_update_StringBuilder_and_list_if_valid_dataset_supplied()
        {
            //Arrange 
            ActiveMedications activeMedications = new ActiveMedications();
            StringBuilder activeMedicationsExpected = new StringBuilder();
            List<string> activeMedDDIListExpected = new List<string>();
            activeMedicationsExpected.Append("0123456, amoxicillin");
            activeMedDDIListExpected.AddRange(new []{ "1", "2"});
            activeMedications.ActiveMedDDIList = activeMedDDIListExpected;
            activeMedications.ActiveMedicationsString = activeMedicationsExpected.ToString();
            int countBeforeUpdate = activeMedications.ActiveMedDDIList.Count;
            string strBeforeUpdate = activeMedications.ActiveMedicationsString;
            List<string> listBeforeUpdate = activeMedications.ActiveMedDDIList;

            // Create two DataTable instances.            
            DataTable table1 = new DataTable("patients");
            table1.Columns.Add("name");
            table1.Columns.Add("id");
            table1.Rows.Add("sam", 1);
            table1.Rows.Add("mark", 2);

            DataTable table2 = new DataTable("Medications");
            table2.Columns.Add("DDI");
            table2.Columns.Add("MedicationName");
            table2.Rows.Add(1, "0123456");
            table2.Rows.Add(2, "amoxicillin");

            // Create a DataSet and put both tables in it.            
            DataSet set = new DataSet("office");
            set.Tables.Add(table1);
            set.Tables.Add(table2);

            //Act                        
            activeMedications.LoadFromDataSet(set);
            int countAfterUpdate = activeMedications.ActiveMedDDIList.Count;
            string strAfterUpdate = activeMedications.ActiveMedicationsString;
            List<string> listAfterUpdate = activeMedications.ActiveMedDDIList;

            //Assert                        
            Assert.AreEqual(countBeforeUpdate, countAfterUpdate);
            
            //Assert 
            Assert.AreEqual(strBeforeUpdate, strAfterUpdate);
        }

        [TestMethod, ExpectedException(typeof(ArgumentException))]
        public void updateStringBuilderList_should_throw_exception_if_null_dataset_supplied()
        {
            //Arrange
            ActiveMedications activeMedications = new ActiveMedications();

            // Create a DataSet and put both tables in it.
            DataSet set = null;

            //Act            
            activeMedications.LoadFromDataSet(set);
            
        }
    }
}
