using System.Collections.Generic;
using eRxWeb.Controller;

namespace eRxWeb.ServerModel
{
    class RightPanelPayload
    {
        public EPCSNotice EPCSNoticePayload { get; set; } 
        public UrgentMessageModel UrgentMessagePayload { get; set; }
        public List<ImportantInfoModel> ImportantInfoPayload { get; set; }
        public EAuthMessageModel EAuthMessagePayload { get; set; }
        public RightBoxModel RightBoxPayload { get; set; }
        public string HelpContentPayload { get; set; }
        public List<ScriptPadModel> ScriptPadPayload { get; set; }
        public FormularyAlternative SigCopayPayload { get; set; }
        public GetEpcsSendToPharmacyApiController.EpcsSendToPharmacyModel EpcsSendToPharmPayload { get; set; }
        public bool SpecialtyMedInfoPayload { get; set; }
        public bool GetEpcsPayload { get; set; }
    }
}