using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eRxWeb.AppCode.Authorize
{
    public static class AppilcationRules
    {
        public static NestedDictionary<IList<Rule>> _rule = new NestedDictionary<IList<Rule>>();
        public static NestedDictionary<IList<Rule>> Rules
        {
            get
            {
                if (_rule.Count == 0)
                {
                    lock (_rule)
                    {
                        if (_rule.Count == 0)//Well an extra check.
                        {
                            ApiRules.SetRules();
                            Pages.SetRules();
                        }
                    }
                }
                return _rule;
            }
        }
    }
    public class Rule
    {
        public UserCategory Role { get; set; }
        public UserPrivilege Privilege { get; set; }
    }

    public static class RuleMapper
    {
        internal static IList<Rule> SetRule(params (UserCategory role, UserPrivilege? privilege)[] rule)
        {
            List<Rule> rules = new List<Rule>();
            for (int i = 0; i < rule.Length; i++)
            {
                rules.Add(new Rule() { Role = rule[i].role, Privilege = rule[i].privilege.Value });
            }

            return rules;
        }
    }
}