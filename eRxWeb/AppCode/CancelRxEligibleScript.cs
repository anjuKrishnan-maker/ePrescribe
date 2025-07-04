using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eRxWeb
{
    
    public class CancelRxEligibleScript
    {
        public Guid RxId { get; set; }

        public Guid OriginalNewRxTrnsCtrlNo{ get; set; }

        public string Medication { get; set; }

        public CancelRxEligibleScript(Guid rxId, Guid originalNewRxTnsCtrlNo, string medication)
        {
            RxId = rxId;
            OriginalNewRxTrnsCtrlNo = originalNewRxTnsCtrlNo;
            Medication = medication;
        }
    }
}