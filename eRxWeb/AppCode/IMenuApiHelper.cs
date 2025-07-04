using Allscripts.ePrescribe.DatabaseSelector;
using eRxWeb.State;

namespace eRxWeb.AppCode
{
    public interface IMenuApiHelper
    {
        string GetTaskCount(string licenseId, string userID, string patientId, string ssoMode, ConnectionStringPointer dbId, string sessionUserId,IStateContainer pageState);
    }
}