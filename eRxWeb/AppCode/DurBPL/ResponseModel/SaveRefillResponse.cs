using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eRxWeb.AppCode.DurBPL.ResponseModel
{
    public class SaveRefillResponse
    {
        public string RxID { get; set; }
        public bool IsGatherWarningsAndInfoToWriteToDB { get; set; }
        public bool IsRefreshActiveMeds { get; set; }
        public string Url { get; set; }
        public bool IsCsMedWorkFlow { get; set; }
        public bool IsstarsAlign { get; set; }
        public bool IsCSMedRefillRequestNotAllowed { get; set; }
        public bool IsUserAPrescribingUserWithCredentials { get; set; }
    }
}