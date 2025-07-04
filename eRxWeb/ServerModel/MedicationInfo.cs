using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eRxWeb.ServerModel
{
    public class MedicationInfo
    {
        public bool MoreActiveMedVisible { get; internal set; }
        public List<ActiveMeds> ActiveMeds { get; set; }
        public string ActiveMed { get; internal set; }
    }
}