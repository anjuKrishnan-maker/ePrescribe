using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using eRxWeb.AppCode.Interfaces;
using System.Configuration;

namespace eRxWeb.AppCode
{
    public class AppConfig: IAppConfig
    {
        public const string K_GA_ACCOUNT_ID = "GaAccountID";
        public const string K_IS_GA_ENABLED = "IsGaEnabled";
        public string GetAppSettings(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }

        public T GetAppSettings<T>(string key)
        {
            var val = ConfigurationManager.AppSettings[key];
            if (!string.IsNullOrEmpty(val))
            {
                var convertedValue = Convert.ChangeType(val, typeof(T));
                return (T)convertedValue;
            }
            else
            {
                return default(T);
            }
        }
    }
}