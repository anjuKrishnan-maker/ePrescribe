using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eRxWeb.ServerModel
{
    [Serializable]
    public class FailedRefReqModel
    {
        public string RequestID { get; set; }
        public string PatientName { get; set; }
        public string PatientDOB { get; set; }
        public string DrugDescription { get; set; }
        public string Refills { get; set; }
        public string PharmacyName { get; set; }
        public string PharmacyAddress { get; set; }
        public string PharmacyPhone { get; set; }
        public string PharmacyFax { get; set; }
    }
    [Serializable]
    public class DeniedRefReqMessages {
        public DeniedRefReqMessages()
        {
            this.DeniedRefReqs = new List<FailedRefReqModel>();
            this.RefReqErrors = new List<FailedRefReqModel>();
        }
        public IList<FailedRefReqModel> RefReqErrors { get; set; }
        public IList<FailedRefReqModel> DeniedRefReqs { get; set; }
    }
}