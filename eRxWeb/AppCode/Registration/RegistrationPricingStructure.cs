using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eRxWeb.AppCode.Registration
{
    public class RegistrationPricingStructure
    {
        public string BasicPrice { get; set; }
        public string DeluxePrice { get; set; }
        public string DeluxeLogRxPrice { get; set; }
        public string DeluxeEpcsPrice { get; set; }
        public string DeluxeEpcsLogRxPrice { get; set; }
        public string EPCSSetupPrice { get; set; }
        public bool EnterprisePricing { get; set; }
    }
}