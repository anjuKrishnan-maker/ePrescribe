using eRxWeb.State;


namespace eRxWeb.AppCode.StateUtils
{
    public static class ShieldInfo
    {
        public static string GetShieldSecurityToken(IStateContainer state)
        {
            string retVal = null;

            if (System.Configuration.ConfigurationManager.AppSettings["SAMLTokenStorageLocation"].ToString().Equals("Session"))
            {
                if (state["ShieldSecurityToken"] != null)
                {
                    retVal = state["ShieldSecurityToken"].ToString();
                }
            }
            return retVal;
        } 
    }
}