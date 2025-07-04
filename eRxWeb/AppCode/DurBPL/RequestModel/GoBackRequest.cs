using Allscripts.Impact.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eRxWeb.AppCode.DurBPL.RequestModel
{
    public class GoBackRequest
    {
        public GoBackRequest(ChangeRxRequestedMedCs changeRxRequestedMedCs)
        {
            ChangeRxRequestedMedCs = changeRxRequestedMedCs;
        }

        public ChangeRxRequestedMedCs ChangeRxRequestedMedCs { get; set; }
    }
}