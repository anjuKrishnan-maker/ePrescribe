using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eRxWeb.ServerModel
{
    public class AllergyInfo
    {
        public string Allergy { get; set; }
        public List<ActiveAllergy> ActiveAllergies { get; set; }
        public bool MoreActiveAllergy { get; set; }
    }
}