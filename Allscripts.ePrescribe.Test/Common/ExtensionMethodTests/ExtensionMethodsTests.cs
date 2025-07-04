using System;
using System.Data;

namespace Allscripts.ePrescribe.Test.Common.ExtensionMethodTests
{
    internal class SomeClass
    {
        public string SomeString { get; set; }
        public int SomeInt { get; set; }
        public DateTime SomeDateTime { get; set; }
        public DataTable SomeTable { get { return GetTable(); } }

        private static DataTable GetTable()
        {
            var table = new DataTable();

            table.Columns.Add(new DataColumn("Test1", typeof(string)));
            table.Columns.Add(new DataColumn("Test2", typeof(int)));

            for (int i = 0; i < 3; i++)
            {
                var row = table.NewRow();
                row["Test1"] = "value" + i;
                row["Test2"] = i;

                table.Rows.Add(row);
            }

            table.AcceptChanges();

            return table;
        }


    }
}