using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eRxWeb.AppCode.DurBPL.RequestModel
{
    public class EPCSDigitalSigningRequest
    {
        public EPCSDigitalSigningRequest(DigitalSigningEventArgs dsEventArgs)
        {
            this.DsEventArgs = dsEventArgs;
        }
        public HttpRequest Request { get; set; }
        public DigitalSigningEventArgs DsEventArgs { get; set; }
        public bool IsApprovalRequestWorkflow { get; set; }
    }
}