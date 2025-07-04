using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eRxWeb.ServerModel.Request
{
    public class UserStateLicenseRequest
    {
        public StateLicense currentUserStateLicense { get; set; }        
        public StateLicense oldUserStateLicense { get; set; }
        public string userID { get; set; }
    }
}