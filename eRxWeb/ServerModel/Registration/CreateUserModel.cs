using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eRxWeb.ServerModel.Registration
{
    public class CreateUserModel
    {
        public bool IsDataSaved { get; set; }
        public string RedirectUrl { get; set; }
        public string ErrorMessage { get; set; }
        public bool IsValidCaptcha { get; set; }
    }
}