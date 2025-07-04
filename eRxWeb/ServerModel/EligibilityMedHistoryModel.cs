using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eRxWeb.ServerModel
{
    public class EligibilityMedHistoryModel
    {
        public string Type { get; set; }
        public string ProcessedDate { get; set; }
        public string Message { get; set; }
        public string AuditID { get; set; }
    }
}