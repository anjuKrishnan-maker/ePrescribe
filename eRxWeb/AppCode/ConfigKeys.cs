using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Reflection;
using Allscripts.ePrescribe.Common;
using eRxWeb.AppCode.Interfaces;
using Allscripts.ePrescribe.DatabaseSelector;
using Allscripts.ePrescribe.ExtensionMethods;
using Allscripts.Impact;

namespace eRxWeb.AppCode
{
    /// <summary>
    /// Contains web.config key values.
    /// </summary>
    public class ConfigKeys : IConfigKeys
    {
        #region Private Members

        private static Dictionary<string, string> _ConfigKeys = new Dictionary<string, string>();
        #endregion

        #region Method

        private static string GetConfigKey(string keyName)
        {
            // double-checked locking.
            if (_ConfigKeys.Count == 0)
                lock (_ConfigKeys)
                {
                    if (_ConfigKeys.Count == 0)
                    {
                        getWebConfigKeys();
                        if (_autoRefreshHandler == null)
                        {
                            _autoRefreshHandler = new ConfigKeyAutoRefreshHandler(Allscripts.ePrescribe.Objects.SystemConfig.ePrescribeApplication.MainApplication, ResetConfigKeys);
                        }
                    }
                }

            return _ConfigKeys[keyName];
        }


        private static void getWebConfigKeys()
        {
            int appID = Convert.ToInt32(Allscripts.ePrescribe.Objects.SystemConfig.ePrescribeApplication.MainApplication);
            DataTable dtKeys = SystemConfig.GetWebConfigKeys(appID, ConnectionStringPointer.SHARED_DB);

            foreach (DataRow dr in dtKeys.Rows)
            {
                _ConfigKeys.Add(dr["Name"].ToString().Trim(), dr["Value"].ToString().Trim());
            }
        }
        

        /// <summary>
        /// This method will reset the config keys.
        /// This will allow cached to be re-filled.
        /// </summary>
        public static void ResetConfigKeys()
        {
            lock (_ConfigKeys)
            {
                _ConfigKeys.Clear();
            }
        }

        #endregion
        private string _IdologyPwd;

        private static ConfigKeyAutoRefreshHandler _autoRefreshHandler;

        public string IDologyPwd
        {
            get { return GetConfigKey("IDologyPwd"); }
            set { _IdologyPwd = value; }
        }

        public static string PdfLicenseKey => GetConfigKey("PDFLicenseKey");

    }
}