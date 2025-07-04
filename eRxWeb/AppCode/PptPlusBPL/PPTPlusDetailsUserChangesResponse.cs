using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Allscripts.ePrescribe.Common;

namespace eRxWeb.AppCode.PptPlusBPL
{
    public class PPTPlusDetailsUserChangesResponse
    {
        public PPTPlusDetailsUserChangeStatus Status { get; set; }
        public string DrugName { get; set; }
        public string Message { get; set; }
        public Constants.PptDetailContext PageContext { get; set; }
        public List<string> MedSearchIndexes { get; set; }
        public PPTPlusDetailsUserChangesResponse()
        {
            Status = PPTPlusDetailsUserChangeStatus.Fail;
            DrugName = string.Empty;
            Message = string.Empty;
            PageContext = Constants.PptDetailContext.Unknown;
            MedSearchIndexes = new List<string>();
        }
    }

    public enum PPTPlusDetailsUserChangeStatus
    {
        Fail,
        MedChange,
        NoChange,
        PharmacyChangeOnly,
        PharmacyMissing,
        PriorAuthChangeOnly
    }
}