using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eRxWeb.AppCode.DurBPL.ResponseModel
{
    public class IgnoreReasonsResponse
    {
        public int ReasonID { get; set; }
        public string ReasonDescription { get; set; }
        public int Category { get; set; }
        public char Active { get; set; }
        public string Created { get; set; }
        public string Modified { get; set; }
    }
}