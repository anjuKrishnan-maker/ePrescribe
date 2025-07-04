using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eRxWeb.ServerModel
{
    public class ComponentRedirectionModel
    {
        //Sso Mode Parameters
        public string SsoMode { get; set; }
        public string SsoPatientId { get; set; }
        public string DefaultPatientLockdownPage { get; set; }
        public string CameFrom { get; set; }
    }
}