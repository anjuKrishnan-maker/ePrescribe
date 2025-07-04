using System;
using System.Collections.Generic;

namespace eRxWeb
{
    public class CancelRxEventArgs : EventArgs
   {
        public List<CancelRxEligibleScript> CanceledScripts { get; set; }

        public CancelRxActionType CancelRxAction { get; set; }

        public enum CancelRxActionType
        {
            CancelScripts,
            ContinueButDoNotCancelScripts,
            ActionCanceled
        }

        public CancelRxEventArgs()
        {
            CanceledScripts = new List<CancelRxEligibleScript>();
        }
    }
}