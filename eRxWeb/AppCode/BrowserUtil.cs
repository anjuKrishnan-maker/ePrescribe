using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Allscripts.ePrescribe.Common;
using eRxWeb.AppCode.Interfaces;

namespace eRxWeb.AppCode
{
    public class BrowserUtil : IBrowserUtil
    {
        public bool IsBrowserUpgradeNeeded(string userAgent, string layoutEngine)
        {
            if (layoutEngine.Contains("trident")) //IE + Edge
            {
                double version;
                if (Double.TryParse(layoutEngine.Replace("trident", ""), out version))
                {
                    return version.CompareTo(5.0) < 0;
                }
                return true; //very low version, force upgrade warning
            }

            if (layoutEngine.Contains("gecko")) //Firefox
            {
                var version = Convert.ToDouble(layoutEngine.Replace("gecko", ""));

                if (version.Equals(20100101)) //desktop
                {
                    var firefoxIndex = userAgent.IndexOf("firefox");
                    version = Convert.ToDouble(userAgent.Substring(firefoxIndex, userAgent.Length - firefoxIndex).Replace("firefox/", ""));
                }

                return version.CompareTo(39.0) < 0;
            }

            if (userAgent.Contains("chrome"))
            {
                var chromeIndex = userAgent.IndexOf("chrome");
                var longVersion = userAgent.Substring(chromeIndex, userAgent.IndexOf(" ", chromeIndex) - chromeIndex).Replace("chrome/", "");
                var version = Convert.ToDouble(longVersion.Substring(0, longVersion.IndexOf(".", longVersion.IndexOf("."))));
                return version.CompareTo(44.0) < 0;
            }

            if (userAgent.Contains("version"))//Safari
            {
                var versionIndex = userAgent.IndexOf("version");
                var longVersion = userAgent.Substring(versionIndex, userAgent.IndexOf(" ", versionIndex) - versionIndex).Replace("version/", "");
                var version = Convert.ToDouble(longVersion.Substring(0, longVersion.IndexOf(".", longVersion.IndexOf("."))));
                return version.CompareTo(7.0) < 0;
            }
            return false;
        }

        public static int GetBrowserHeight(string rawHeight)
        {
            int height;
            int.TryParse(rawHeight, out height);

            //if the height is less than 250, we have an issue (possibly with the particular browser of the user)
            if (height < 505)
                height = 1000;
        
            return height;
        }
    }
}