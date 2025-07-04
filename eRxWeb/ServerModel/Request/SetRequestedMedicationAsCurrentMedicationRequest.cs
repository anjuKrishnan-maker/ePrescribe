using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eRxWeb.ServerModel.Request
{
    public class SetRequestedMedicationAsCurrentMedicationRequest
    {
        public string ScriptMessageGuid { get; set; }
        public string RequestedRxDrugDescription { get; set; }
        public string RxDetails { get; set; }
    }
}