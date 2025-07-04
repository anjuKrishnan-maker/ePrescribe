using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.Objects.Registrant;
using Allscripts.ePrescribe.Shared.Logging;
using eRxWeb.AppCode;
using System;
using System.Collections.Generic;
using System.Web.Security;
using static Allscripts.ePrescribe.Common.Constants;

namespace eRxWeb
{
    public partial class TwoNUserMediator : LoginBasePage
    {
        private static ILoggerEx logger = LoggerEx.GetLogger();
        protected void Page_Init(object sender, EventArgs e)
        {
            string redirectUrl = $"~/{PageNames.LOGOUT.ToLower()}?reauthenticate=true";
            if (PageState.GetBooleanOrFalse(Constants.SessionVariables.AutoLogin2nUser))
            {
                PageState.Remove(Constants.SessionVariables.AutoLogin2nUser);
                string redirectUrl2n = PageState.GetStringOrEmpty(Constants.SessionVariables.RedirectUrl2n);
                PageState.Remove(Constants.SessionVariables.RedirectUrl2n);

                if (!string.IsNullOrWhiteSpace(redirectUrl2n))
                    redirectUrl = redirectUrl2n;
            }
           RedirectUser(redirectUrl);
        }
    }
}