// Revision History
/******************************************************************************
**Change History
*******************************************************************************
**Date:      Author:                    Description:
**-----------------------------------------------------------------------------
* 02/08/2010   Anand Kumar Krishnan     Defect#3335 - Task count is retained when Tool tab is selected. 
********************************************************************************/
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
using Allscripts.ePrescribe.Common;
using Allscripts.Impact;

namespace eRxWeb
{
public partial class Library : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            int tasks = 0;

            int tableHeight = ((PhysicianMasterPageBlank)Master).getTableHeight();
           

            if (base.SessionLicense.GetFeatureStatus(Constants.DeluxeFeatureType.iFC) == Constants.DeluxeFeatureStatus.On)
            {
                Module deluxeModule = new Module(Module.ModuleType.DELUXE, base.SessionLicenseID, base.SessionUserID, base.DBID);
                if (IsLexicompEnabled)
                {
                    deluxeModule.InsertModuleAudit("Lexicomp - LIBRARY", base.DBID);
                    string lexicompURL = string.Empty;
                    if (Request.QueryString["search"] != null)
                    {
                        lexicompURL = string.Format(ConfigKeys.LexicompSearchBoxURL, HttpUtility.UrlDecode(Request.QueryString["search"]));
                    }
                    else
                    {
                        lexicompURL = ConfigKeys.LexicompLibrarySearchURL;
                    }
                    string Lexicomp_SSO = "<iframe name='iframe1' id='iframe1' src='" + lexicompURL.ToHTMLEncode() + "' align='top' width='100%' height='" + tableHeight + "' frameborder='0' title='Library'>If you can see this, your browser does not support iframes! </iframe>";
                    librarySSO.Controls.Add(new LiteralControl(Lexicomp_SSO));
                }
                else
                {
                    deluxeModule.InsertModuleAudit("IFC - LIBRARY", base.DBID);

                    ((PhysicianMasterPageBlank)Master).toggleTabs("library", tasks);

                        string factsComparisonsURL = string.Empty;
                        if (Request.QueryString["search"] != null)
                        {
                            factsComparisonsURL = string.Format(ConfigKeys.FactsComparisonsSearchBoxURL, HttpUtility.UrlDecode(Request.QueryString["search"]));
                        }
                        else
                        {
                            factsComparisonsURL = ConfigKeys.FactsComparisonsLibrarySearchURL;
                        }
                        string WK_SSO = "<iframe name='iframe1' id='iframe1' src='" + factsComparisonsURL.ToHTMLEncode() + "' align='top' width='100%' height='" + tableHeight + "' frameborder='0' title='Library'>If you can see this, your browser does not support iframes! </iframe>";
                        librarySSO.Controls.Add(new LiteralControl(WK_SSO));
                }
            }
            else if (SessionLicense.GetFeatureStatus(Constants.DeluxeFeatureType.iFC) == Constants.DeluxeFeatureStatus.Disabled)
            {
                adControl.FeaturedModule = Module.ModuleType.DELUXE;
                adControl.Show = true;
            }
            else if (SessionLicense.GetFeatureStatus(Constants.DeluxeFeatureType.iFC) == Constants.DeluxeFeatureStatus.Off)
            {
                adControl.FeaturedModule = Module.ModuleType.DELUXE;
                adControl.Show = true;
            }
                        
            ((PhysicianMasterPageBlank)Master).toggleTabs("library", tasks);
        }
    }
    protected void Page_LoadComplete(object sender, EventArgs e)
    {
        int tasks = 0;
        if (Session["LICENSEID"] != null)
        {
            string licenseID = Session["LICENSEID"].ToString();

            if (Session["UserType"] != null && (Convert.ToInt32(Session["UserType"]) == 1 || Convert.ToInt32(Session["UserType"]) == 1000 || Convert.ToInt32(Session["UserType"]) == 1001))
            {
                tasks = Allscripts.Impact.Provider.GetTaskCountForProvider(licenseID, Session["USERID"].ToString(), base.DBID, base.SessionUserID);
            }
            else
            {               
                    if (base.IsPOBUser & !base.IsPOBViewAllProviders)
                    {
                        // get task count only for selected Providers associated to POB
                        tasks = TaskManager.GetTaskListScriptCount(licenseID, new Guid(base.SessionUserID), (int)Constants.PrescriptionTaskType.SEND_TO_ADMIN, base.DBID, base.SessionUserID);
                    }
                    else
                    {
                        // get task count all "assistant" tasks
                        tasks = TaskManager.GetTaskListScriptCount(licenseID, (int)Constants.PrescriptionTaskType.SEND_TO_ADMIN, base.DBID, base.SessionUserID);
                    }
            }
        }

        ((PhysicianMasterPageBlank)Master).toggleTabs("library", tasks);
    }
}

}