using eRxWeb.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Allscripts.ePrescribe.Common;
using static Allscripts.ePrescribe.Common.Constants;
using eRxWeb.ePrescribeSvc;


namespace eRxWeb.AppCode.Users
{
    public class EditUserSecretQuestions
    {
        public SaveSecretQuestionAnswersResponse SaveSecretQuestionAnswers(SaveSecretQuestionAnswersRequest request, IStateContainer session)
        {
            return EPSBroker.SaveSecretQuestionAnswers(request.SecretAnswers,
                                                       session.GetStringOrEmpty(SessionVariables.SessionShieldUserName),
                                                       request.Password,
                                                       session.GetStringOrEmpty(SessionVariables.ShieldSecurityToken),
                                                       session.GetStringOrEmpty(SessionVariables.ShieldExternalTenantID),
                                                       HttpContext.Current.Request.UserIpAddress(),
                                                       ApiHelper.GetDBID(session));
        }

        public SecretQuestion[] GetUserShieldSecretQuestions(IStateContainer session)
        {
            string shieldSecurityToken = StateUtils.ShieldInfo.GetShieldSecurityToken(session); ;
            string shieldExternalTenantID = session.ContainsKey(SessionVariables.ShieldExternalTenantID) ? session.GetStringOrEmpty(SessionVariables.ShieldExternalTenantID) : EPSBroker.GetShieldExternalTenantID(session.GetStringOrEmpty(SessionVariables.AHSAccountID)); 
            return EPSBroker.GetUserShieldSecretQuestions(shieldSecurityToken, shieldExternalTenantID).ToArray();
        }

        public SecretQuestion[] GetAllShieldSecretQuestions()
        {
            return EPSBroker.GetAllShieldSecretQuestions().ToArray();
        }
    }
}