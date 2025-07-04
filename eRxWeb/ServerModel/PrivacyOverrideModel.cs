using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eRxWeb.ServerModel
{
    public class PrivacyOverrideModel
    {
        public bool Success { get; set; }
        public string CancelURL { get; set; }
        public string RedirectURL { get; set; }

        public string PageName { get; set; }

        public bool IsRestrictedUser { get; set; }
    }
}