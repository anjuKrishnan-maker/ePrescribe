using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eRxWeb.ServerModel.Request
{
    public class MedicationSelectedRequest
    {
        public string DDI { get; set; }
        public int FormularyStatus { get; set; }
        public string MedName { get; set; }
        public string taskScriptMessageId { get; set; }
        
    }

    public class MessageQueueRequest
    {
        public string scriptMessageID { get; set; }
    }
}