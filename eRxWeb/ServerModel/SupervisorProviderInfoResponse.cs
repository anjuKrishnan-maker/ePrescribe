using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eRxWeb.ServerModel
{
    public class SupervisorProviderInfoResponse
    {
        public bool IsSupervisorProviderInfoSet { get; set; }
        public string Message { get; set; }
        public MessageIcon MessageIcon { get; set; }
    }
}