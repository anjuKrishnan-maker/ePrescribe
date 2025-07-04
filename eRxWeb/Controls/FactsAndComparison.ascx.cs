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
using System.Xml;
using System.Xml.Xsl;
using Telerik.Web.UI;
using Allscripts.Impact;
using Allscripts.ePrescribe.Common;

namespace eRxWeb
{
public partial class FactsAndComparison : BaseControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (linksHit.Value != "")
        {
            int linkCount = linksHit.Value.Split('|').Length;
            Module deluxeModule = new Module(Module.ModuleType.DELUXE, base.SessionLicenseID, base.SessionUserID, base.DBID);
            string WkType = IsLexicompEnabled ? "Lexicomp - CONTEXUAL" : "IFC - CONTEXUAL";
            for (int i = 0; i < linkCount; i++)
            {
                deluxeModule.InsertModuleAudit(WkType, base.DBID);
            }
            linksHit.Value = "";
        }
    }
}

}