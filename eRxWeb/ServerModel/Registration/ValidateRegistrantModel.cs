using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eRxWeb.ServerModel.Registration
{
    public class ValidateRegistrantModel
    {
        public bool IsValid { get; set; }
        public bool IsValidCaptcha { get; set; }
        public bool IsValidNpi { get; set; }
        public bool IsDeaCheck { get; set; }
        public bool IsValidDEA { get; set; }
        public bool isValidExistingShieldUser {get;set;}
        public bool IsMaxRetryFinished { get; set; }
    }
}