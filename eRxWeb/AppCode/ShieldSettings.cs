using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Allscripts.ePrescribe.Common;
using eRxWeb.State;

namespace eRxWeb.AppCode
{
    public class ShieldSettings
    {
        public static string ePrescribeExternalAppInstanceId(IStateContainer pageState)
        {
            if (pageState[Constants.SessionVariables.AppInstanceID] == null)
            {
                pageState[Constants.SessionVariables.AppInstanceID] = string.Concat("ERX-", pageState[Constants.SessionVariables.AHSAccountID].ToString(), "A");
            }
            return pageState[Constants.SessionVariables.AppInstanceID].ToString();
        }

        public static int GetShieldInternalAppInstanceID(IStateContainer state)
        {
            if (state["ShieldInternalAppInstanceID"] == null)
            {
                state["ShieldInternalAppInstanceID"] = EPSBroker.GetShieldInternalAppInstanceID(state["AHSAccountID"].ToString());
            }

            return Convert.ToInt32(state["ShieldInternalAppInstanceID"]);
        }

        public static string GetShieldExternalTenantID(IStateContainer state)
        {
            if (!state.ContainsKey("ShieldExternalTenantID"))
            {
                state["ShieldExternalTenantID"] = EPSBroker.GetShieldExternalTenantID(state["AHSAccountID"].ToString());
            }

            return state.GetStringOrEmpty("ShieldExternalTenantID");
        }


        public static int GetShieldInternalEnvironmentID(IStateContainer state)
        {
            if (state["ShieldInternalEnvironmentID"] == null)
            {
                state["ShieldInternalEnvironmentID"] = EPSBroker.GetShieldInternalEnvironmentID(state["AHSAccountID"].ToString());
            }

            return Convert.ToInt32(state["ShieldInternalEnvironmentID"]);
        }

        public static int GetShieldInternalAppID(IStateContainer state)
        {
            //if it's null, we haven't tried to get it yet; if it's empty or less then zero, then we tried before and got an error so we should try again
            if (state[Constants.SessionVariables.ShieldInternalAppID] == null ||
                    string.IsNullOrWhiteSpace(state[Constants.SessionVariables.ShieldInternalAppID].ToString()) ||
                    int.Parse(state[Constants.SessionVariables.ShieldInternalAppID].ToString()) < 0)
            {
                state[Constants.SessionVariables.ShieldInternalAppID] = EPSBroker.GetShieldInternalAppID();
            }

            return Convert.ToInt32(state[Constants.SessionVariables.ShieldInternalAppID]);
        }

        public static ePrescribeSvc.Role[] GetShieldAppRoles(IStateContainer state)
        {
            ePrescribeSvc.Role[] shieldAppRoles;

            if (state[Constants.SessionVariables.ShieldAppRoles] == null)
            {
                shieldAppRoles = EPSBroker.GetRolesForApplication(ShieldSettings.GetShieldInternalAppID(state));
                state[Constants.SessionVariables.ShieldAppRoles] = shieldAppRoles;
            }
            else
            {
                shieldAppRoles = (ePrescribeSvc.Role[])state[Constants.SessionVariables.ShieldAppRoles];
            }

            return shieldAppRoles;
        }
    }
}