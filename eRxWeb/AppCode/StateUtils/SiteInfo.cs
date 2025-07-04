using eRxWeb.State;


namespace eRxWeb.AppCode.StateUtils
{
    public static class SiteInfo
    {
        public static int GetSessionSiteID(IStateContainer state)
        {
            int temp = -1;
            if (state["SiteID"] == null)
                return temp;

            if (!int.TryParse(state["SITEID"].ToString(), out temp))
                temp = -1;

            return temp;
        }
    }
}