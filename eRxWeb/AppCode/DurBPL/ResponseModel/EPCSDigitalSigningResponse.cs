using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eRxWeb.AppCode.DurBPL.ResponseModel
{
    public class EPCSDigitalSigningResponse
    {
        public string Url { get; set; }
        public string MessageText { get; set; }
        public Controls_Message.MessageType MessageIconType  { get; set; }
        public bool DisplayMainMessage { get; set; }

        public bool IsGatherWarningsAndInfoToWriteToDB { get; set; }
        public string RxId { get; set; }
        public bool UpdatePatientActiveMeds { get; set; }

        public bool IsEventSuccess { get; set; }
    }
}