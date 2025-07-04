using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.Shared.Logging;
using eRxWeb.State;
using Microsoft.Web.Services3.Design;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.SessionState;
using static Allscripts.ePrescribe.Common.Constants;

namespace eRxWeb.AppCode.Authorize
{
    public static class AuthorizationManager
    {
        private static ILoggerEx _loggerEx;

        public static ILoggerEx Logger
        {
            get { return _loggerEx ?? LoggerEx.GetLogger(); }
            set { _loggerEx = value; }
        }

        public static bool Process(IStateContainer state, string primaryResource, string secondaryResource = "")
        {
            bool requireAuthorization = Allscripts.Impact.ConfigKeys.IsAuthorizationEnabled;

            if (!requireAuthorization)
                return true;
            AuthorizationStatus authorizationStatus;
            bool isResourceHavingAccess = false;
            UserCategory userCategory = UserCategory.NONE;
            UserPrivilege userPrevilege = UserPrivilege.ANY;
            try
            {
                if (string.IsNullOrEmpty(primaryResource))
                    throw new Exception($"{primaryResource} is empty.Hence rule cannot be determined.");

                if (state.Count == 0)                
                    return false;                    

                var userType = state.GetString(SessionVariables.UserType, null);

                if (userType != null)
                {
                    Constants.UserCategory userRole = (Constants.UserCategory)Enum.Parse(typeof(Constants.UserCategory), userType, true);
                    userCategory = (UserCategory)Enum.Parse(typeof(UserCategory), userRole.ToString());
                }

                userPrevilege = state.GetBooleanOrFalse(SessionVariables.IsAdmin) ? UserPrivilege.ADMIN : UserPrivilege.ANY;
                authorizationStatus = CheckUserRoleAccess(userCategory, userPrevilege, primaryResource, secondaryResource);

                isResourceHavingAccess = authorizationStatus.IsAuthorized;
                if (!isResourceHavingAccess)
                {
                    throw new Exception($"User with role {userCategory} and having privilege {userPrevilege}" +
                                        $" with user ID : {state.GetStringOrEmpty(SessionVariables.UserId)} is denied access " +
                                        $"to {primaryResource}/{secondaryResource} configured with rule {authorizationStatus.AuthorizationRule}");
                }

                if (isResourceHavingAccess)
                    isResourceHavingAccess = RunAdditionalPermissions(state, primaryResource);

                if (!isResourceHavingAccess)
                    throw new Exception($"User with user ID : {state.GetStringOrEmpty(SessionVariables.UserId)} is denied access " +
                                        $"to {primaryResource}/{secondaryResource} missing additional permission check.");

            }
            catch (Exception ex)
            {
                Logger.Debug($"Denied access to {primaryResource}/{secondaryResource}" +
                              $"due to exception {ex.Message}");
                LogException(state, ex.Message);
                return false;
            }

            if (isResourceHavingAccess)
                Logger.Debug($"Allowed access to {primaryResource}/{secondaryResource}" +
                              $"for user with role {userCategory} and privilege {userPrevilege}");

            return isResourceHavingAccess;
        }

        private static void LogException(IStateContainer state, string errorMessage)
        {
            ApiHelper.AuditException(errorMessage, state);
        }

        private static bool IsAuthorized(UserCategory currentUserCategory, UserPrivilege currentUserPrivilege, IList<Rule> rules)
        {
            var rulesToCheck = rules;
            //If rule is confugured NONE. Just authentication is enough no further role check
            if (rulesToCheck.Any((x) => x.Role.HasFlag(UserCategory.NONE)))
                return true;
            return rulesToCheck.Any((x) =>
            {
                if (x.Privilege.HasFlag(UserPrivilege.ANY))
                    return x.Role.HasFlag(currentUserCategory);
                return x.Role.HasFlag(currentUserCategory) && x.Privilege.HasFlag(currentUserPrivilege);
            });
        }


        internal static AuthorizationStatus CheckUserRoleAccess(UserCategory userCategory, UserPrivilege userPrivilege,
                                                                string primaryResource, string secondaryResouce = "")
        {
            var rules = AppilcationRules.Rules;
            var rule = rules[primaryResource];
            IList<Rule> primaryPermission = rule.Value;

            IList<Rule> secondaryPermission = rule[secondaryResouce].Value;

            if (primaryPermission == null && secondaryPermission == null)
            {
                string exceptionMissingSecondaryResource = string.IsNullOrEmpty(secondaryResouce) ? "" : $"nor for {secondaryResouce}";
                throw new Exception($"No rule configured for {primaryResource} {exceptionMissingSecondaryResource}");
            }

            bool isPrimaryRuleAllowing = false;
            if (secondaryPermission == null)//When secondary rule not set.
            {
                isPrimaryRuleAllowing = IsAuthorized(userCategory, userPrivilege, primaryPermission);
                return new AuthorizationStatus
                {
                    IsAuthorized = isPrimaryRuleAllowing,
                    AuthorizationRule = GenerateReadableRule(primaryPermission)
                };
            }

            if (primaryPermission != null)
                isPrimaryRuleAllowing = IsAuthorized(userCategory, userPrivilege, primaryPermission);

            bool isSecondaryRuleAllowing = IsAuthorized(userCategory, userPrivilege, secondaryPermission);

            return new AuthorizationStatus
            {
                IsAuthorized = isSecondaryRuleAllowing,
                AuthorizationRule = GenerateReadableRule(secondaryPermission)

            };
        }

        private static string GenerateReadableRule(IList<Rule> rules)
        {
            return rules.Aggregate(new StringBuilder(), (current, next) => current.AppendFormat(", {0}:{1}", next.Role, next.Privilege),
                                                                    sb => sb.Length > 2 ? sb.Remove(0, 2).ToString() : "");
        }


        private static bool RunAdditionalPermissions(IStateContainer state, string resource)
        {
            List<Func<IStateContainer, bool>> rules;
            bool hasRule = PageAddtionlRules.PageAdditionalRule.TryGetValue(resource, out rules);//Currently the rule additional is only for Page.
            if (!hasRule)
                return true;

            bool isRuleSuccess = false;

            foreach (var rule in rules)
            {
                isRuleSuccess = rule(state);
                if (!isRuleSuccess)
                    break;
            }
            return isRuleSuccess;
        }
    }

    internal class AuthorizationStatus
    {
        public bool IsAuthorized { get; set; }
        public string AuthorizationRule { get; set; }
    }
}