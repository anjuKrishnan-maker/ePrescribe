using System;

namespace eRxWeb.ServerModel
{
    public class CancelRxModel
    {
        
    }


    public class SendCancelRxRequestModel
    {
        public string RxID { get; set; }
        public Guid OriginalNewRxTrnsCtrlNo { get; set; }
    }

}