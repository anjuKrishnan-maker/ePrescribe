using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eRxWeb.ServerModel
{
    public class DiagnosisInfo
    {
        public string Dx { get; set; }
        public bool MoreActiveProblem { get; set; }
        public List<ActiveDignosis> ActiveDiagnosis { get; set; }
    }
}