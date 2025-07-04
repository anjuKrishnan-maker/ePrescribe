using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eRxWeb.ServerModel
{
    class MedicationSelectedPayload
    {
        public List<SponsoredLinkModel> RobustLinkPayload { get; set; }
        public CopayCoverage CopayCoveragePayload { get; set; }
        public List<FormularyAlternative> FormularyAlternativesPayload { get; set; }
        public List<GenericAlternative> GenericAlternativesPayload { get; set; }
    }
}