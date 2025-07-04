using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using TieServiceClient;
using System.Collections.Specialized;
using Allscripts.ePrescribe.Common;
using Allscripts.Impact;
using eRxWeb.State;
using eRxWeb.AppCode;

namespace eRxWeb
{
public partial class AdPrivacyPolicy : BasePage
{
    public IPlacementResponse PlacementResponse { get; set; }

        public string SessionAppVersion => Convert.ToString(Session[Constants.SessionVariables.AppVersion]);
        protected void Page_Init(object sender, EventArgs e)
        {
            lnkDefaultStyleSheet.Href += "?version=" + SessionAppVersion;
        }
        protected void Page_Load(object sender, EventArgs e)
    {
        ePrescribeSvc.ePrescribeSvc eps = new ePrescribeSvc.ePrescribeSvc();
        eps.Url = ConfigurationManager.AppSettings["ePrescribeSvc.eprescribesvc"].ToString();
        eulaContent.Controls.Clear();
        eulaContent.Controls.Add(new LiteralControl(eps.GetAdPrivacyPolicy()));

        PlacementResponse = TieUtility.GetAdPlacement(Request.Cookies, new TieUtility(), new TIEServiceManager(), new Allscripts.Impact.Utilities.ConfigurationManager());
    }

    
}
}