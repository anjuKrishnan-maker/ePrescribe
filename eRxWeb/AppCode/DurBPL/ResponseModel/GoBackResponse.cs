using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eRxWeb.AppCode.DurBPL.ResponseModel
{
    public class GoBackResponse
    {
        public string Url { get; set; }
        public bool RedirectToSelectPatient { get; set; }
    }
}