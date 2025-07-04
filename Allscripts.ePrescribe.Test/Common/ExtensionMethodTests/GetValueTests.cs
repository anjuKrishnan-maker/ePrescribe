using System;
using System.Data;
using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.ExtensionMethods;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Allscripts.ePrescribe.Test.Common.ExtensionMethodTests
{
    [TestClass]
    public class GetValueTests
    {
        [TestMethod]
        public void should_return_empty_string_if_column_does_not_exist()
        {
            //arrange
            var dt = new DataTable("HI");
            dt.Columns.Add("Col1");
            var dr = dt.NewRow();

            //act
            var result = dr.GetValue("me");

            //assert
            Assert.AreEqual(string.Empty, result);
        }

        [TestMethod]
        public void should_return_null_if_column_value_is_null()
        {
            //arrange
            var dt = new DataTable("HI");
            dt.Columns.Add("Col1");
            var dr = dt.NewRow();

            //act
            var result = dr.GetValue("Col1");

            //assert
            Assert.AreEqual(null, result);
        }

        [TestMethod]
        public void should_return_null_if_column_value_is_db_null()
        {
            //arrange
            var dt = new DataTable("HI");
            dt.Columns.Add("Col1");
            var dr = dt.NewRow();
            dr["Col1"] = DBNull.Value;
            dt.Rows.Add(dr);

            //act
            var result = dr.GetValue("Col1");

            //assert
            Assert.AreEqual(null, result);
        }

        [TestMethod]
        public void should_return_value()
        {
            //arrange
            var value = "1234";
            var dt = new DataTable("HI");
            dt.Columns.Add("Col1");
            var dr = dt.NewRow();
            dr["Col1"] = value;
            dt.Rows.Add(dr);

            //act
            var result = dr.GetValue("Col1");

            //assert
            Assert.AreEqual(value, result);
        }
    }
}
