using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eRxWeb.AppCode.DurBPL
{
    public class CSMedRefillNotAllowedPrintRefillRequest : ICSMedRefillNotAllowedPrintRefillRequest
    {
        public string QueryString { get; set; }
        public string UserHostAddress { get; set; }
    }
}