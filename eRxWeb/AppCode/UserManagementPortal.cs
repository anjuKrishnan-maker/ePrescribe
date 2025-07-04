using System.Linq;
using Allscripts.ePrescribe.Common;
using eRxWeb.ePrescribeSvc;
using ConnectionStringPointer = Allscripts.ePrescribe.DatabaseSelector.ConnectionStringPointer;

namespace eRxWeb.AppCode
{
    public class UserManagementPortal
    {
        public static bool UpdateLicenseToUMPByLicenseID(string licenseID, ConnectionStringPointer dbID )
        {
            return EPSBroker.UpdateLicenseInUMPByLicenseID(licenseID, (ePrescribeSvc.ConnectionStringPointer) dbID);
        }

        public static bool AddShieldUserToUMP(string userGUID, ConnectionStringPointer dbID)
        {
            var response =  EPSBroker.GetRxUser(ValueType.UserGUID, userGUID, null, null, null, dbID);

            return EPSBroker.AddUserToUMP(response.RxUser.LicenseID, response.RxUser.ShieldUserName, response.RxUser.FirstName, response.RxUser.LastName,
                response.RxUser.Email, userGUID, response.RxUser.AppRoles.Any(x => x == Constants.UserRoleNames.ADMIN), dbID);
        }

        public static bool AddUserToUMP(string licenseID, string userName, string firstName, string lastName, string email, string userGuid, bool isAdmin, ConnectionStringPointer dbID)
        {
            return EPSBroker.AddUserToUMP(licenseID, userName, firstName, lastName, email, userGuid, isAdmin, dbID);
        }
    }
}