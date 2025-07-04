using Allscripts.ePrescribe.Common;
using eRxWeb.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eRxWeb.AppCode.Authorize
{
    public static class PageAddtionlRules
    {
        private static Func<IStateContainer, bool> CheckShowIntegrationSolutionsIsSet = (state) =>
        {
            var sessionLicesnse = Settings.GetSessionLicense(state);
            return sessionLicesnse.EnterpriseClient.ShowIntegrationSolutions;
        };

        private static Func<IStateContainer, bool> CheckFeatureConfigured = (state) =>
        {
            if (!state.ContainsKey(Constants.SessionVariables.SessionLicense))
                return false;
            var sessionLicesnse = Settings.GetSessionLicense(state);
            var featureStatus = sessionLicesnse
                                    .GetFeatureStatus(Constants.DeluxeFeatureType.iFC);

            if (!(featureStatus == Constants.DeluxeFeatureStatus.On ||
                 featureStatus == Constants.DeluxeFeatureStatus.Disabled ||
                 featureStatus == Constants.DeluxeFeatureStatus.Off))
                return false;
            return true;
        };

        public static Dictionary<string, List<Func<IStateContainer, bool>>> PageAdditionalRule =
            new Dictionary<string, List<Func<IStateContainer, bool>>>
             {
                {Constants.PageNames.INTEGRATION_SOLUTIONS_LIST.ToLower(),new List<Func<IStateContainer, bool>>{CheckShowIntegrationSolutionsIsSet}},
                {Constants.PageNames.LIBRARY.ToLower(),new List<Func<IStateContainer, bool>>{CheckFeatureConfigured}}
             };
    }
}