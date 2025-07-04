using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eRxWeb.ServerModel
{
    public class UrgentMessageModel
    {
        public bool IsDeluxMessageVisible { get; set; }
        public bool IsFailedRxMessageVisible { get; set; }
        public bool IsFailedUserMessageVisible { get; set; }
        public bool IsFailedLicenseMessageVisible { get; set; }
        public bool IsDeniedRefReqMessagesVisible { get; set; }
        public bool IsrRgistrationMessagesVisible { get; set; }
        public bool IsStateLicenseExpiryVisible { get; set; }
        public bool IsDeaExpiryVisible { get; set; }
        public bool IsTrialPeriodExpiryVisible { get; set; }
        public string StateLicenseExpiryMessage { get; set; }
        public string DeaExpiryMessage { get; set; }
        public string TrialPeriodExpiryMessage { get; set; }

        public string FailedRxMessage { get; set; }
        public string FailedRxMessageLinkText { get; set; }
        public string FailedRxMessageLinkUrl { get; set; }

        public string FailedRegistrationMessage { get; set; }
        public string FailedegistrationMessageLinkText { get; set; }
        public string FailedRegistrationMessageLinkUrl { get; set; }

        public string DeniedRefReqMessages { get; set; }
        public string DeniedRefReqMessagesLinkText { get; set; }
        public string DeniedRefReqMessagesLinkUrl { get; set; }
        public string EnableEprescribeMsg { get; set; }
        public string FailedRxUserMessage { get; set; }
        public string FailedRxUserMessageUrl { get; set; }
        public string FailedLicenseMessage { get; set; }
        public string FailedLicenseMessageUrl { get; set; }
        public bool IsEpcsCanEnrollVisible { get; internal set; }
        public bool IsEpcsApproverVisible { get; internal set; }
        public string EpcsCanEnrollText { get; internal set; }
        public string EpcsCanEnrollUrl { get; internal set; }

        public bool IsPasswordExpiryNotice { get; set; }

        public string DaysLeftBeforePasswordExpires { get; set; }
    }

    public class FaildRegMsg
    {
        public string RequestCreated { get; set; }
        public string Name { get; set; }
        public string RequestID { get; internal set; }
    }
}