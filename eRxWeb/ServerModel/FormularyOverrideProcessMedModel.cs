using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eRxWeb.ServerModel
{
    public class FormularyOverrideProcessMedModel
    {
        public FormularyOverrideProcessMedModel(ScriptPadModel scriptPadModel)
        {
            ScriptPadModel = scriptPadModel;
        }


        public FormularyOverrideProcessMedModel(string redirectUrl)
        {
            RedirectUrl = redirectUrl;
        }

        public string RedirectUrl { get; set; }
        public ScriptPadModel ScriptPadModel { get; set; }
    }
}