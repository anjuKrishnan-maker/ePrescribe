using Allscripts.Impact;
using eRxWeb.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace eRxWeb.AppCode.StateUtils
{
    public static class RxInfo
    {
        public static bool IsShowRxInfo(IStateContainer state)
        {
            if ((UserInfo.GetSessionLicense(state).EnterpriseClient.ShowRxInfo) && (state["SHOW_RX_INFO"] != null))
            {
                return Convert.ToBoolean(state["SHOW_RX_INFO"]);
            }
            else
            {
                return false;
            }
        }
        public static void CheckForRxInfo(IStateContainer state,Rx rx)
        {
            if (IsShowRxInfo(state))
            {
                if (EPSBroker.IsRxInfoAvailableForDDI(rx.DDI, ApiHelper.GetDBID(state)))
                {
                    try
                    {
                        // spawn a thread to call for getting RxInfo
                        Task task = Task.Run(() => requestRxInfo(rx, state));
                        task.Wait(2000);
                    }
                    catch (Exception ex)
                    {
                        Audit.AddException(UserInfo.GetSessionUserID(state), UserInfo.GetSessionLicenseID(state), "Attempt to spawn thread for RxInfo Requestfailed : " + ex.ToString(), string.Empty, string.Empty, string.Empty, ApiHelper.GetDBID(state));
                    }
                }
            }
        }
        private static void requestRxInfo(object rx, IStateContainer state)
        {
            EPSBroker.RequestRxInfo(((Rx)rx).RxID, UserInfo.GetSessionLicenseID(state), UserInfo.GetSessionUserID(state), ApiHelper.GetDBID(state));
        }
    }
}