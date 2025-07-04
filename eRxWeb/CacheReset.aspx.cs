using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Allscripts.Impact;
using Allscripts.ePrescribe.Common;

namespace eRxWeb
{
public partial class CacheReset : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string updated = "Nothing";
        string idiot = string.Empty;
        if (Request.QueryString["ItemResetID"] != null)
        {
            switch (Request.QueryString["ItemResetID"])
            {
                case "ImportantInfo-LVFSDFSVLKJDS":
                    SystemConfig.ResetImportantInfo();
                    updated = "Important Info";
                    break;
                case "Help-LVFSDFSVLKJDS":
                    SystemConfig.ResetHelp();
                    updated = "Help";
                    break;
                case "SponsoredLinks-LVFSDFSVLKJDS":
                    SponsoredLink.ResetSponsoredLinks();
                    updated = "Sponsored Links";
                    break;
                case "StatePrintFormats-LVFSDFSVLKJDS":
                    SystemConfig.ResetStates();
                    updated = "State Print Formats";
                    break;
                case "AppVersion-LVFSDFSVLKJDS":
                    ePrescribeSvc.ePrescribeSvc eps = new ePrescribeSvc.ePrescribeSvc();
                    eps.Url = System.Configuration.ConfigurationManager.AppSettings["ePrescribeSvc.eprescribesvc"].ToString();
                    ePrescribeSvc.CacheResetRequest cacheResetReq = new ePrescribeSvc.CacheResetRequest();
                    cacheResetReq.ItemResetID = "AppVersion";
                    eps.CacheReset(cacheResetReq);
                    SystemConfig.ResetAppVersion();
                    updated = "App Version";
                    break;
                case "EULA-LVFSDFSVLKJDS":
                    SystemConfig.ResetEULA();
                    updated = "EULA";
                    break;
                case "IgnoreReasons-LVFSDFSVLKJDS":
                    IgnoreReason.ResetIgnoreReasons();
                    updated = "Ignore Reasons";
                    break;
                case "Modules-LVFSDFSVLKJDS":
                    Module.ResetModules();
                    updated = "Modules";
                    break;
                case "ALL-LVFSDFSVLKJDS":
                    ePrescribeSvc.ePrescribeSvc epsALL = new ePrescribeSvc.ePrescribeSvc();
                    epsALL.Url = System.Configuration.ConfigurationManager.AppSettings["ePrescribeSvc.eprescribesvc"].ToString();
                    ePrescribeSvc.CacheResetRequest cacheResetReqALL = new ePrescribeSvc.CacheResetRequest();
                    cacheResetReqALL.ItemResetID = "ALL";
                    epsALL.CacheReset(cacheResetReqALL);

                    SystemConfig.ResetImportantInfo();
                    SystemConfig.ResetHelp();
                    SponsoredLink.ResetSponsoredLinks();
                    SystemConfig.ResetStates();                    
                    SystemConfig.ResetAppVersion();
                    IgnoreReason.ResetIgnoreReasons();
                    Module.ResetModules();
                    ConfigKeys.ResetConfigKeys();

                    updated = "All";
                    break;
                case "DENIALREASON-LVFSDFSVLKJDS":
                    Cache.Remove("DenialReason");
                    updated = "Denial Reasons";
                    break;
                case "ConfigKeys-LVFSDFSVLKJDS":
                    ePrescribeSvc.ePrescribeSvc epsConfigKeys = new ePrescribeSvc.ePrescribeSvc();
                    epsConfigKeys.Url = System.Configuration.ConfigurationManager.AppSettings["ePrescribeSvc.eprescribesvc"].ToString();
                    ePrescribeSvc.CacheResetRequest cacheResetReqConfigKeys = new ePrescribeSvc.CacheResetRequest();
                    cacheResetReqConfigKeys.ItemResetID = "ConfigKeys";
                    epsConfigKeys.CacheReset(cacheResetReqConfigKeys);

                    ConfigKeys.ResetConfigKeys();
                    updated = "ConfigKeys";
                    break;
                   
                default:
                    idiot = "..idiot";
                    break;
            }
        }

        resetInfo.InnerText = updated + " has been reset." + idiot;         
    }
}

}