using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eRxWeb.ServerModel
{
    public class SelectPatientComponentParameters
    {
        public string PatientId { get; set; }
        public string PatientName { get; set; }
        public bool MessageVisibility { get; set; }
        public string MessageIcon { get; set; }
        public string MessageTooltip { get; set; }
        public string MessageText { get; set; }
    }
}