using System;
using Allscripts.ePrescribe.DatabaseSelector;

namespace eRxWeb
{
/// <summary>
/// Summary description for DMSBroker
/// </summary>
public class DMSBroker
{
    private const string DMS_APP_CONFIG_URL = "DataManagerSvc.DataManagerSvc";

	public DMSBroker()
	{
		
	}

    public static ConnectionStringPointer GetDBIDByUserGUID(string userGUID)
    {
        DataManagerSvc.DataManagerSvc dms = new DataManagerSvc.DataManagerSvc();
        dms.Url = getAppSettingsValue(DMS_APP_CONFIG_URL);

        DataManagerSvc.EntityIdentifier entityID = new DataManagerSvc.EntityIdentifier()
        {
            Type = DataManagerSvc.IdentifierType.UserGUID,
            Value = userGUID
        };

        DataManagerSvc.ConnectionStringPointer dbID = dms.GetDatabasePointer(entityID);

        return (ConnectionStringPointer)dbID;
    }

    private static string getAppSettingsValue(string key)
    {
        return System.Configuration.ConfigurationManager.AppSettings[key].ToString();
    }
}
}