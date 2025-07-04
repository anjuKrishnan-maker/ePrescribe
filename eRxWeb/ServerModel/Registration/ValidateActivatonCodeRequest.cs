using eRxWeb.AppCode.Interfaces;
using eRxWeb.ePrescribeSvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace eRxWeb.ServerModel.Registration
{
    public class ValidateActivatonCodeRequest
    {
        public string ActivationCode { get; set; }
        public string CaptchaText { get; set; }
    }
}