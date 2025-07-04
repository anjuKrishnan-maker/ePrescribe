using Allscripts.ePrescribe.Common;
using eRxWeb.ePrescribeSvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eRxWeb.ServerModel.Registration
{
    public class LinkUserModel
    {
        public ErrorReason ErrorReason { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string AccountName { get; set; }
        public bool IsValid { get; set; }
        public Constants.Workflow WorkflowType { get; set; }

    }
}