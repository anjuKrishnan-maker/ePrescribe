using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eRxWeb.ServerModel
{
    public class ScriptPadModel
    {
        internal bool ShowucCancelRx;

        public ScriptPadModel()
        {
            MedCompletionHistory = new List<MedCompletionHistory>();
        }
        public string Description { get; set; }
        public string rxId { get; set; }
        public bool IsBrandNameMed { get; set; }
        public string RxEditUrl { get; internal set; }
        public string ReviewUrl { get; set; }
        public string RedirectUrl { get; set; }
        public bool showMediHistoryCompletionPopUp { get; internal set; }
        public IList<MedCompletionHistory> MedCompletionHistory { get; set; }
        public List<CancelRxEligibleScript> CancelRxScripts { get; set; }
    }

    public class MedCompletionHistory
    {
        public string Medication { get; set; }
        public string RxID { get; set; }
    }
}