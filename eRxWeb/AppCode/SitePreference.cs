using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Allscripts.ePrescribe.DatabaseSelector;

namespace eRxWeb
{
/// <summary>
/// Summary description for SitePreference
/// </summary>
public class SitePreference
{
    public SitePreference()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    public static void SetPreference(string licenseID, int siteID, string preferenceName, string preferenceValue, ConnectionStringPointer dbID)
    {
        Allscripts.ePrescribe.Data.Preference.SetSitePreference(licenseID, siteID, preferenceName, preferenceValue, dbID);
    }
    public static string GetPreference(string licenseID, int siteID, string preferenceName, ConnectionStringPointer dbID)
    {
        object ret = Allscripts.ePrescribe.Data.Preference.GetSitePreference(licenseID, siteID, preferenceName, dbID);

        return ret != null ? ret.ToString() : null;
    }
       
}

}