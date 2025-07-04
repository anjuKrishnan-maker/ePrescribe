using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Allscripts.ePrescribe.Common;

namespace eRxWeb.ServerModel
{
    public class VerificationCodeSMSModel
    {
        public string userID { get; set; }
        public string phoneNumber { get; set; }
        public int otp { get; set; }
        public string message { get; set; }
        public Constants.SMSAlertUserActionType status { get; set; } 
    }
}