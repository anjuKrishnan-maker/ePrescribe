using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eRxWeb.ServerModel
{
    public class CheckInPatientDataResponse
    {
        public string CheckedInMessage { get; set; }
        public bool CheckedInMessageVisibility { get; set; }
        public string CheckedInMessageIcon { get; set; }
    }
}