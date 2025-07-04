using System;
using System.Data;
using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.ExtensionMethods;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Allscripts.ePrescribe.Test.Common.ExtensionMethodTests
{
    [TestClass]
    public class GetFirstRowValueTests
    {
        [TestMethod]
        public void should_return_empty_string_if_table_doesnt_exist()
        {
            //arrange
            var ds = new DataSet();

            //act
            var result = ds.GetFirstRowValue("NoTable");

            //assert
            Assert.AreEqual(string.Empty, result);
        }

        [TestMethod]
        public void should_return_empty_string_if_row_doesnt_exist()
        {
            //arrange
            var dt = new DataTable();
            var ds = new DataSet();
            ds.Tables.Add(dt);

            //act
            var result = ds.GetFirstRowValue("NoTable");

            //assert
            Assert.AreEqual(string.Empty, result);
        }

        [TestMethod]
        public void should_return_empty_string_if_column_doesnt_exist()
        {
            //arrange
            var dt = new DataTable();
            dt.Columns.Add("WrongColumn");
            var dr = dt.NewRow();
            dt.Rows.Add(dr);
            var ds = new DataSet();
            ds.Tables.Add(dt);

            //act
            var result = ds.GetFirstRowValue("Column");

            //assert
            Assert.AreEqual(string.Empty, result);
        }

        [TestMethod]
        public void should_return_string_value_if_exist()
        {
            //arrange
            var dt = new DataTable();
            dt.Columns.Add("GetThisValue");
            var dr = dt.NewRow();
            dr["GetThisValue"] = "CorrectValue";
            dt.Rows.Add(dr);
            var ds = new DataSet();
            ds.Tables.Add(dt);

            //act
            var result = ds.GetFirstRowValue("GetThisValue");

            //assert
            Assert.AreEqual("CorrectValue", result);
        }
    }
}
