using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Allscripts.Impact.ePrescribeSvc;

namespace eRxWeb.ServerModel
{
    public class CheckAddressVerificationModel
    {
        public string RedirectionPage { get; set; }
        public HOME_ADDRESS_CHECK_STATUS HomeAddressCheckStatus { get; set;}
    }
}