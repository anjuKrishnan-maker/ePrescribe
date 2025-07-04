using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using eRxWeb.ePrescribeSvc;

namespace eRxWeb
{

    public class DueDiligenceEventArgs : EventArgs
    {
        public bool Success { get; set; }
        public bool IsCancel { get; set; }
        public string Message { get; set; }
        public bool forceLogout { get; set; }
        public string OtpSecurityToken { get; set; }
        public string IdentitySecurityToken { get; set; }
        public List<UserNameWithUserGuidPair> UsersAccepted { get; set; }

        public DueDiligenceEventArgs()
        {
            
        }

        public DueDiligenceEventArgs(bool success)
        {
            Success = success;
        }
    }
}