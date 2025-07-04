using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Allscripts.ePrescribe.Common;
using Allscripts.Impact.PPTPlusBPL;

namespace eRxWeb.ServerModel.Request
{
    public class PptPlusSummaryRequest
    {
        public string[] RemovedIndexes { get; set; }
        public string DDI { get; set; }
        public string GPPC { get; set; }
        public string PackUom { get; set; }
        public string Quantity { get; set; }
        public string PackSize { get; set; }
        public string PackQuantity { get; set; }
        public string Refills { get; set; }
        public string DaysSupply { get; set; }
        public string IsDaw { get; set; }
        public string MedSearchIndex { get; set; }
    }

    public class PPTPlusUserChanges
    {
        public PPTDetailsUserEvents userChanges { get; set; }
        public Constants.PptDetailContext pageContext { get; set; }
    }
}