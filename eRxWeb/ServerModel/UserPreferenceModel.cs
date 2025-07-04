using System;
using System.Data;
using Allscripts.ePrescribe.ExtensionMethods;
using Allscripts.ePrescribe.Objects.StoredProcReturnObjects;

namespace eRxWeb.ServerModel
{
    public class UserPreferenceModel
    {
        public bool IsPPTPlusEnabled { get; set; }
        public bool IsShowPDMP { get; set; }
    }
}