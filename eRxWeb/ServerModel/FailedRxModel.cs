using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eRxWeb.ServerModel
{
    [Serializable]
    public class FailedRxModel
    {
        public string RequestID { get; set; }
        public string RequestText { get; set; }
    }

    [Serializable]
    public class FailedRxMessage
    {
        public FailedRxMessage()
        {
            FailedRxMessages = new List<FailedRxModel>();
        }
        public IList<FailedRxModel> FailedRxMessages { get; set; }
    }
}