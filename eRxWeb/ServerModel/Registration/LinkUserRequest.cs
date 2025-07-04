using eRxWeb.AppCode.Interfaces;
using eRxWeb.ePrescribeSvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace eRxWeb.ServerModel.Registration
{
    public class LinkUserRequest
    {
        public string ShieldUserName { get; set; }
        public string ShieldPassword { get; set; }
    }
}