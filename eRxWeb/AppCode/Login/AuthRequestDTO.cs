using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eRxWeb.AppCode
{
    public class AuthRequestDTO
    {
        public string IpAddress { get; set; }
        public Allscripts.ePrescribe.DatabaseSelector.ConnectionStringPointer DbId { get; set; }

        public string UserName { get; set; }

        public bool IsPasswordResetSuccess { get; set; }
    }
}