
using System;
using eRxWeb.ePrescribeSvc;

namespace eRxWeb.AppCode
{
    [Serializable]
    public class AuthResponseDTO
    {
        public bool IsLoginFailed { get; set; }
        public string LoginErrorMessage { get; set; }
        public bool IsShieldPasswordExpired { get; set; }
        public bool HasLoggedIn { get; set; }
        public bool IsRegistrantUser { get; set; }
        public bool HaveShieldAccountAndPendingRestration { get; set; }//Have a valid shield login and a registration in process
        public AuthenticateAndAuthorizeUserResponse AuthenticateAndAuthorizeUserResponse { get; set; }
    }
}