using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Allscripts.ePrescribe.Objects;
using eRxWeb.AppCode;
using System.Data;

namespace Allscripts.ePrescribe.Test.ERxWebTests.AppCodeTests.SpecialtyMedTests
{
    [TestClass]
    public class NPIFromDelegateProviderUnitTests
    {
        [TestMethod]
        public void should_return_empty_string_if_DataTable_null()
        {
            //Arrange
            string expectedVal = string.Empty;

            //Act
            string actualVal = new SpecialtyMed().NPIFromDelegateProvider(null);

            //Assert
            Assert.AreEqual(expectedVal, actualVal);
        }

        [TestMethod]
        public void should_return_empty_string_if_DataTable_rowcount_zero()
        {
            //Arrange
            string expectedVal = string.Empty;

            //Act
            string actualVal = new SpecialtyMed().NPIFromDelegateProvider(new DataTable());

            //Assert
            Assert.AreEqual(expectedVal, actualVal);
        }

        [TestMethod]
        public void should_return_valid_string_if_valid_DataTable_Supplied()
        {
            //Arrange
            DataTable dtProviderDetails = new DataTable();
            dtProviderDetails.Columns.Add("BlahColumn", typeof(string));
            dtProviderDetails.Columns.Add("NPI", typeof(string));
            DataRow dr = dtProviderDetails.NewRow();
            dr["BlahColumn"] = "Blah";
            dr["NPI"] = "10050378";
            dtProviderDetails.Rows.Add(dr);
            string expectedVal = "10050378";

            //Act
            string actualVal = new SpecialtyMed().NPIFromDelegateProvider(dtProviderDetails);

            //Assert
            Assert.AreEqual(expectedVal, actualVal);
        }
    }
}
