using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using eRxWeb.State;
using Allscripts.ePrescribe.Common;
using Allscripts.Impact.ePrescribeSvc;
using ConnectionStringPointer = Allscripts.ePrescribe.DatabaseSelector.ConnectionStringPointer;
using eRxWeb.AppCode.StateUtils;

namespace eRxWeb.AppCode
{
    public class MenuApiHelper : IMenuApiHelper
    {
        public static bool IsPatientLockDownMode(string ssoMode, string patientId)
        {
            return !string.IsNullOrEmpty(ssoMode) &&
                   ssoMode == Constants.SSOMode.PATIENTLOCKDOWNMODE &&
                   !string.IsNullOrEmpty(patientId);
        }
        public static string GetTaskCount(string licenseId, string userID, string patientId, string ssoMode, ConnectionStringPointer dbId, string sessionUserId,IStateContainer pageState)
        {
            int tasks;
            if (IsPatientLockDownMode(ssoMode, patientId) )
            {
                tasks = Allscripts.Impact.Patient.GetTaskCountForPatient(licenseId, patientId, dbId);
            }
            else if(IsEpaPatientLockDownMode(ssoMode, patientId))
            {
                if (UserInfo.IsPOBUser(pageState))
                {
                    tasks = Allscripts.Impact.ePA.GetePATaskCount(ApiHelper.GetSessionLicenseID(pageState), ApiHelper.GetSessionDelegateProviderID(pageState), ApiHelper.GetSessionUserID(pageState), ApiHelper.GetSessionPatientId(pageState), pageState.GetStringOrEmpty(Constants.SessionVariables.SSOMode), ApiHelper.GetDBID(pageState), ApiHelper.GetSessionUserID(pageState));

                }
                else
                {
                    tasks = Allscripts.Impact.ePA.GetePATaskCount(ApiHelper.GetSessionLicenseID(pageState), string.Empty,string.Empty, ApiHelper.GetSessionPatientId(pageState), pageState.GetStringOrEmpty(Constants.SessionVariables.SSOMode), ApiHelper.GetDBID(pageState), ApiHelper.GetSessionUserID(pageState));

                }

            }
            else
            {
                tasks = Allscripts.Impact.Provider.GetTaskCountForProvider(licenseId, userID, dbId, sessionUserId);
            }
            if (tasks > 0)
            {
                return Convert.ToString(tasks);
            }
            else
                return string.Empty;
        }

        public static bool IsEpaPatientLockDownMode(string ssoMode, string patientId)
        {
            return !string.IsNullOrEmpty(ssoMode) &&
                   ssoMode == Constants.SSOMode.EPAPATIENTLOCKDOWNTASKMODE &&
                   !string.IsNullOrEmpty(patientId);
        }

        string IMenuApiHelper.GetTaskCount(string licenseId, string userID, string patientId, string ssoMode, ConnectionStringPointer dbId, string sessionUserId, IStateContainer pageState)
        {
            return GetTaskCount(licenseId, userID, patientId, ssoMode, dbId, sessionUserId, pageState);
        }
    }
}