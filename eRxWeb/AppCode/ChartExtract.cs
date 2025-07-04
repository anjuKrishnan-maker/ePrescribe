using Allscripts.ePrescribe.Common;
using Allscripts.Impact;
using eRxWeb.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eRxWeb.ServerModel
{
    public static class ChartExtract
    {
        public static bool IsShowChartExtract(IStateContainer session)
        {
            var sessionLicense = session.Cast(Constants.SessionVariables.SessionLicense, default(ApplicationLicense));

            bool isChartExtractDownloadLinkEnabled = session.GetBooleanOrFalse(Constants.LicensePreferences.PatientChartExtract);

            return (sessionLicense.LicenseDeluxeStatus ==
                Constants.DeluxeFeatureStatus.On &&
                Allscripts.Impact.ConfigKeys.ChartExtractDownload &&
                isChartExtractDownloadLinkEnabled);

        }
    }
}