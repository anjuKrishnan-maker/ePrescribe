using Allscripts.Impact.ScriptMsg;


namespace eRxWeb.AppCode.DurBPL.RequestModel
{
    public class SubmitDurRequest
    {
        public bool starsAlign { get; set; }
        public ScriptMessage RxScriptMessage { get; set; }
        public string UserHostAddress { get; set; }
        public bool IsCapturedReasons { get; set; }
        public string ProviderId { get; set; }
    }
}