using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eRxWeb.ServerModel
{    
    public class DEALicense
    {
        public string ProviderId { get; set; }
        public string DEALicenseID { get; set; }
        public string DEANumber { get; set; }
        public string DEAExpirationDate { get; set; }
        public bool DEAIAllowed { get; set; }
        public bool DEAIIAllowed { get; set; }
        public bool DEAIIIAllowed { get; set; }
        public bool DEAIVAllowed { get; set; }
        public bool DEAVAllowed { get; set; }
        public DeaLicenseType DEALicenseTypeId { get; set; }
    }

    public enum DeaLicenseType
    {
        Unknown=0,
        DefaultDEA=1,
        DEA=2,
        NADEAN=3
    }
}