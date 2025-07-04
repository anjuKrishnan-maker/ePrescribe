using Allscripts.ePrescribe.Common;
using Allscripts.Impact;
using Allscripts.Impact.Interfaces;
using eRxWeb.State;

namespace eRxWeb.AppCode
{
    public class SupervisingProvider
    {
        public static string SetInSession(RxUser provider, IStateContainer session, IDeaScheduleUtility deaUtil)
        {
            if (!provider.HasNPI) return "Please select a provider with a valid NPI.";

            var deaSchedules = deaUtil.ExtractAllowed(provider);
            session[Constants.SessionVariables.SupProviderDeaSchedules] = deaSchedules;
            session[Constants.SessionVariables.SupProviderMinDea] = deaUtil.RetrieveMin(deaSchedules);

            session[Constants.SessionVariables.SupervisingProviderId] = provider.UserID;

            return null;
        }
    }
}