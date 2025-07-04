using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Allscripts.ePrescribe.Common;

namespace eRxWeb.AppCode
{
    public class Angular
    {
        public static HashSet<string> PageNames => new HashSet<string>
        {
            Constants.PageNames.REVIEW_HISTORY.ToLower(),
            Constants.PageNames.SETTINGS.ToLower(),
            Constants.PageNames.SELECT_MEDICATION.ToLower(),
            Constants.PageNames.SELECT_PATIENT.ToLower(),
            Constants.PageNames.NURSE_FULL_SCRIPT.ToLower(),
            Constants.PageNames.REPORTS.ToLower()
        };
    }
}