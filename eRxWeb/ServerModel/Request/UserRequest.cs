using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eRxWeb.ServerModel.Request
{
    public class UserRequest
    {
        public UserMode UserMode { get; set; }
        public string UserID { get; set; }
        public bool IsNoLic { get; set; }

    }
}