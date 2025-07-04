using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using Allscripts.ePrescribe.Common;
using eRxWeb.AppCode.Interfaces;
using StateLicense = eRxWeb.ServerModel.StateLicense;
using eRxWeb.State;

namespace eRxWeb.AppCode.Users
{
    public class UserStateLicense: IUserStateLicense
    {
        public StateLicense[] GetStateLicenses(IStateContainer session, string userId)
        {            
            var dbId = ApiHelper.GetDBID(session);
            if (userId == String.Empty)
                userId = session.GetStringOrEmpty(Constants.SessionVariables.UserId);
            DataSet ds = Allscripts.Impact.RxUser.GetStateLicenses(userId, dbId);
            List<StateLicense> stateLicenses = new List<StateLicense>();
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                StateLicense sl = new StateLicense();
                sl.State = row["State"].ToString();
                sl.LicenseType = row["TypeDescription"].ToString();
                sl.LicenseNo = row["LicenseNumber"].ToString();
                sl.ExpiringDate = row["ExpirationDate"].ToString();
                stateLicenses.Add(sl);
            }
            return stateLicenses.ToArray();
        }

        public string[][] GetStatesAndLicenseTypes(IStateContainer session)
        {            
            List<string[]> response = new List<string[]>();
            var dbId = ApiHelper.GetDBID(session);
            DataTable statesTable = Allscripts.Impact.RxUser.ChGetState(dbId);
            DataTable licenseTypesTable = Allscripts.Impact.RxUser.GetProviderLicenseTypes(dbId);
            response.Add(statesTable.Rows.OfType<DataRow>().Select(dr => (string)dr["state"]).ToArray());
            response.Add(licenseTypesTable.Rows.OfType<DataRow>().Select(dr => (string)dr["TypeDescription"]).ToArray());
            return response.ToArray();
        }

        public void AddProviderLicense(IStateContainer session, string userId, StateLicense license)
        {            
            var dbId = ApiHelper.GetDBID(session);
            if (userId == String.Empty)
                userId = session.GetStringOrEmpty(Constants.SessionVariables.UserId);
            var sessionUserId = session.GetStringOrEmpty(Constants.SessionVariables.UserId);

            Allscripts.Impact.RxUser.UpdateProviderLicense(license.State, license.LicenseNo, Convert.ToDateTime(license.ExpiringDate), userId, sessionUserId, license.LicenseType, dbId);
        }

        public void UpdateProviderLicense(IStateContainer session, string userId, StateLicense originalLicense, StateLicense updatedLicense)
        {            
            var dbId = ApiHelper.GetDBID(session);
            if (userId == String.Empty)
                userId = session.GetStringOrEmpty(Constants.SessionVariables.UserId);
            var sessionUserId = session.GetStringOrEmpty(Constants.SessionVariables.UserId);
            DateTime newExpiringDate = DateTime.Parse(updatedLicense.ExpiringDate);            

            Allscripts.Impact.RxUser.UpdateProviderLicense(updatedLicense.State, updatedLicense.LicenseNo, newExpiringDate,
                                                           userId, sessionUserId, updatedLicense.LicenseType, originalLicense.State,
                                                           originalLicense.LicenseNo, null, originalLicense.LicenseType, dbId);
        }

        public void DeleteProviderLicense(IStateContainer session, string userId, StateLicense license)
        {            
            var dbId = ApiHelper.GetDBID(session);
            if (userId == String.Empty)
                userId = session.GetStringOrEmpty(Constants.SessionVariables.UserId);
            var sessionUserId = session.GetStringOrEmpty(Constants.SessionVariables.UserId);

            Allscripts.Impact.RxUser.DeleteProviderLicense(userId, license.LicenseNo, license.State, sessionUserId, license.LicenseType, dbId);
        }
    }
}