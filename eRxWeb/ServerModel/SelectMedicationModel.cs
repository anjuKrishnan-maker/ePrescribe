using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eRxWeb.ServerModel
{
    public class SelectMedicationModel
    {
        public SelectMedicationReturnAction ReturnAction { get; set; }
        public ScriptPadModel ScriptPadModel { get; set; }
        public string RedirectUrl { get; set; }
        public MessageModel MessageModel { get; set; }
    }
}