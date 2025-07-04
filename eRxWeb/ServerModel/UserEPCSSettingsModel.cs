using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eRxWeb.ServerModel
{
    public class UserEPCSSettingsModel
    {
        public bool IsEnterpriseClientEPCSEnabled { get; set; }
        public bool IsLicenseEPCSPurchased { get; set; }
        public bool IsLicenseShieldEnabled { get; set; }
        public bool IsEPCSApprover { get; set; }
        public bool ChkEPCSApproverEnabled { get; set; }
        public bool ShowEPCSRegistartionLink { get; set; }
        public string EPCSPermissionLabel { get; set; }
        public bool ZentryUser { get; set; }
        public bool CspUser { get; set; }
        public bool ShowManageSecondFactorForms { get; set; }
        public string SecondFactorFormUrl { get; set; }
        public string SecondFactorHelpUrl { get; set; }
        public string EpcsRegistrationLink { get; set; }
        public string EpcsRegistrationLinkLabel { get; set; }
        public bool ShowEPCSSecondFactor { get; set; }
        public bool ShowManageEpcsApprovalsLink { get; set; }
        public string EPCSSecondFactorLabel { get; set; }
    }
}