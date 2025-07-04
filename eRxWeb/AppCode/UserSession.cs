using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.DatabaseSelector;
using Allscripts.ePrescribe.Objects;
using Allscripts.Impact.Utilities;
using eRxWeb.AppCode.Interfaces;
using eRxWeb.AppCode.PdmpBPL;
using eRxWeb.State;

namespace eRxWeb.AppCode
{
    public class UserSession
    {
        public static SamlTokenRefresh AttemptSamlTokenRefresh(string externalAppInstanceId, IStateContainer session, IConfigurationManager configManager, IEPSBroker epsBroker)
        {
            var refreshResult = new SamlTokenRefresh{Result = SamlRefreshResult.FORCE_LOGOUT};

            var isTokenRefreshEnabled = configManager.GetValue(Constants.AppConfigVariables.IsTokenRefreshEnabled);

            if (session != null) {
                if (isTokenRefreshEnabled != null && isTokenRefreshEnabled.Equals("True", StringComparison.OrdinalIgnoreCase)
                     && !session.GetBooleanOrFalse(Constants.SessionVariables.IsBackDoorUser))
                {
                    var shieldToken = session.GetStringOrEmpty(Constants.SessionVariables.ShieldSecurityToken);
                    if (session[Constants.SessionVariables.RefreshAuthTokenDateTimeUTC] == null
                        || string.IsNullOrWhiteSpace(shieldToken))
                    {
                        return refreshResult;
                    }

                    if (DateTime.UtcNow > session.GetDateTimeOrMin(Constants.SessionVariables.RefreshAuthTokenDateTimeUTC))
                    {
                        var response = epsBroker.RefreshSamlToken(shieldToken, HttpContext.Current.Request.UserIpAddress(),  externalAppInstanceId,
                            session.Cast(Constants.SessionVariables.DbId, ConnectionStringPointer.ERXDB_DEFAULT));

                        if (!response.Success) return refreshResult;

                        session[Constants.SessionVariables.ShieldSecurityToken] = response.RefreshedSecurityToken;

                        var tokenLifetime = response.TokenValidToDateTimeUTC - response.TokenValidFromDateTimeUTC;
                        session[Constants.SessionVariables.RefreshAuthTokenDateTimeUTC] = response.TokenValidFromDateTimeUTC.AddMinutes(tokenLifetime.TotalMinutes / 2);

                        if (!response.AreClaimCollectionsIdentical.RetrieveBooleanValue())
                        {
                            refreshResult.Result = SamlRefreshResult.REAUTHENTICATION_REQUIRED;
                            return refreshResult;
                        }

                        if (response.RefreshedSecurityToken == null)
                        {
                            refreshResult.Result = SamlRefreshResult.FORCE_LOGOUT;
                            return refreshResult;
                        }

                        refreshResult.NewExpirationTimeMs = (int) (response.TokenValidToDateTimeUTC - DateTime.UtcNow).TotalMilliseconds;
                        refreshResult.Result = SamlRefreshResult.SESSION_EXTENDED;
                        PDMP.GetPDMPSamlToken(session);

                        return refreshResult;
                    }
                }
            }

            refreshResult.Result = SamlRefreshResult.NO_CHANGE;
            return refreshResult;
        }
}
}