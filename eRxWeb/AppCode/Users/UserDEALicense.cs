using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using eRxWeb.State;
using eRxWeb.ServerModel;

namespace eRxWeb.AppCode.Users
{
    public class UserDEALicense
    {
        public DEALicense[] GetDEALicenses(IStateContainer session, string userId)
        {
            var dbId = ApiHelper.GetDBID(session);
            DEALicense[] dEALicenses = ConvertToServerModel(EPSBroker.GetProviderDEALicenses(userId, dbId));
            return dEALicenses;
        }

        private DEALicense[] ConvertToServerModel(ePrescribeSvc.DEALicense[] dEALicenses)
        {
            DEALicense[] dEALicensesArrray;
            dEALicensesArrray = (from deaLicense in dEALicenses
                               select new DEALicense()
                               {
                                   ProviderId = deaLicense.ProviderID,
                                   DEALicenseID = deaLicense.DEALicenseID,
                                   DEANumber = deaLicense.DEANumber,
                                   DEAExpirationDate = Convert.ToString(deaLicense.DEAExpirationDate.ToShortDateString()),
                                   DEAIAllowed = deaLicense.DEAIAllowed,
                                   DEAIIAllowed = deaLicense.DEAIIAllowed,
                                   DEAIIIAllowed = deaLicense.DEAIIIAllowed,
                                   DEAIVAllowed = deaLicense.DEAIVAllowed,
                                   DEAVAllowed = deaLicense.DEAVAllowed,
                                   DEALicenseTypeId = (DeaLicenseType)deaLicense.DeaLicenseTypeId
                               }).ToArray();
            return dEALicensesArrray;
        }

        public void AddUserDEALicense(IStateContainer session, string userId, DEALicense dEALicense)
        {
            var dbid = ApiHelper.GetDBID(session);
            var sessionUserId = ApiHelper.GetSessionUserID(session);
            var shieldUserName = ApiHelper.GetSessionShieldUserName(session);
            var appInstanceID = ApiHelper.GetEprescribeExternalAppInstanceID(session);
            var securityToken = ApiHelper.GetShieldSecurityToken(session);
            EPSBroker.InsertDEALicenses(userId, null, dEALicense.DEANumber, Convert.ToDateTime(dEALicense.DEAExpirationDate), dEALicense.DEAIAllowed.ToString(),
                                        dEALicense.DEAIIAllowed.ToString(), dEALicense.DEAIIIAllowed.ToString(), dEALicense.DEAIVAllowed.ToString(), dEALicense.DEAVAllowed.ToString(),
                                       (Allscripts.ePrescribe.Objects.DeaLicenseType)dEALicense.DEALicenseTypeId, sessionUserId, shieldUserName, securityToken, appInstanceID, dbid);
        }

        public void UpdateDEALicense(IStateContainer session, string userId, DEALicense dEALicense)
        {
            var dbid = ApiHelper.GetDBID(session);
            var sessionUserId = ApiHelper.GetSessionUserID(session);
            var shieldUserName = ApiHelper.GetSessionShieldUserName(session);
            var appInstanceID = ApiHelper.GetEprescribeExternalAppInstanceID(session);
            var securityToken = ApiHelper.GetShieldSecurityToken(session);
            EPSBroker.UpdateDEALicenses(dEALicense.ProviderId, dEALicense.DEALicenseID.ToString(), dEALicense.DEANumber, Convert.ToDateTime(dEALicense.DEAExpirationDate),
                                        dEALicense.DEAIAllowed.ToString(), dEALicense.DEAIIAllowed.ToString(), dEALicense.DEAIIIAllowed.ToString(), dEALicense.DEAIVAllowed.ToString(),
                                        dEALicense.DEAVAllowed.ToString(), (Allscripts.ePrescribe.Objects.DeaLicenseType)dEALicense.DEALicenseTypeId, sessionUserId, appInstanceID, shieldUserName, securityToken, dbid);
        }

        public void DeleteDEALicense(IStateContainer pageState, string userId, DEALicense dEALicense)
        {
            var dbid = ApiHelper.GetDBID(pageState);
            var sessionUserId = ApiHelper.GetSessionUserID(pageState);
            var shieldUserName = ApiHelper.GetSessionShieldUserName(pageState);
            var appInstanceID = ApiHelper.GetEprescribeExternalAppInstanceID(pageState);
            var securityToken = ApiHelper.GetShieldSecurityToken(pageState);
            EPSBroker.DeleteProviderDEALicense(dEALicense.ProviderId, dEALicense.DEALicenseID.ToString(), sessionUserId, appInstanceID, 
                                               shieldUserName, securityToken, dbid);
        }
    }
}