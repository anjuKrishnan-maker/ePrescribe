using eRxWeb.AppCode.Interfaces;
using eRxWeb.ePrescribeSvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace eRxWeb.ServerModel.Registration
{
    public class ValidateActivatonCodeModel
    {
        public bool IsValid { get; set; }
        public bool IsValidCaptcha { get; set; }
    }
}