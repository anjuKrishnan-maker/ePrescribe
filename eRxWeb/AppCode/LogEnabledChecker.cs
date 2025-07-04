using System;
using System.Web;
using Allscripts.ePrescribe.Shared.Data;
using Allscripts.ePrescribe.Shared.Logging;

namespace eRxWeb.AppCode
{
    public class LogEnabledChecker
    {
        public static void CheckLoggingForUser(string userAttributeString, UserAttributeType attrType)
        {
            switch (attrType)
            {
                case UserAttributeType.UserGuid:
                {
                    Guid userGuid;
                    if (Guid.TryParse(userAttributeString, out userGuid))
                    {
                        if (EPSBroker.IsUserLoggingEnabled(userGuid))
                        {
                            SetLoggingEnabled();
                        }
                    }
                }
                    break;
                case UserAttributeType.UserName:
                {
                    if (EPSBroker.IsUserLoggingEnabled(userAttributeString))
                    {
                        SetLoggingEnabled();
                    }
                }
                    break;
            }
        }

        public enum UserAttributeType
        {
            UserName,
            UserGuid
        }

        private static void SetLoggingEnabled()
        {
            if (HttpContext.Current != null && HttpContext.Current.Session != null)
                HttpContext.Current.Session[LogConstants.ENABLE_LOGGING_KEY] = true; ;
        }
    }


}