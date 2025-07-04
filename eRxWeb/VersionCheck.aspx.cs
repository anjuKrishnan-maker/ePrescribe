using Allscripts.ePrescribe.Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eRxWeb
{
    public partial class VersionCheck : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ePrescribeSvc.ePrescribeSvc eps = new ePrescribeSvc.ePrescribeSvc();
            eps.Url = ConfigurationManager.AppSettings["ePrescribeSvc.eprescribesvc"];
            ePrescribeSvc.ApplicationVersionRequest appVerReq = new ePrescribeSvc.ApplicationVersionRequest();
            appVerReq.ApplicationID = ePrescribeSvc.ePrescribeApplication.MainApplication;
            Name.Text = eps.GetFullAppversion(appVerReq).ToHTMLEncode();
        }
    }
}