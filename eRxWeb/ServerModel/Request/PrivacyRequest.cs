using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eRxWeb.ServerModel.Request
{
    public class PrivacyRequest
    {
        public string PageName { get; set; }
        public string OverrideText { get; set; }
        public string patientId { get; set; }
        public string userId { get; set; }
        public string createdUtc { get; set; }

    }
}