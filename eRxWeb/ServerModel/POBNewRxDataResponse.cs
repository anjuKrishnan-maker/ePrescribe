using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using eRxWeb.Controller;

namespace eRxWeb.ServerModel
{
    public class POBNewRxDataResponse
    {
        public bool IsPaSupervised { get; set; }
        public string Message { get; set; }
        public MessageIcon MessageIcon { get; set; }
        public Dictionary<string, string> ProvidersForSupervisedPA { get; set; }
    }
}