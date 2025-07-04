using System;
using System.Data;

namespace eRxWeb.AppCode.Tasks
{
    [Serializable]
    public class RefillTaskLight
    {
        public bool IsOTC { get; set; }
        public int FormularyStatus { get; set; }
        public string ScriptMessageID { get; set; }

        public static RefillTaskLight FromDataRow(DataRow row)
        {
            var rtl = new RefillTaskLight();
            rtl.IsOTC = row["IsOTC"].ToString() == "Y";
            rtl.FormularyStatus = Convert.ToInt32("0" + row["FormularyStatus"]);
            rtl.ScriptMessageID = row["ScriptMessageID"].ToString();

            return rtl;
        }
    }
}