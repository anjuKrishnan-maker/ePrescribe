using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using TieServiceClient;
using System.Collections.Generic;
using System.Collections.Specialized;
using Allscripts.ePrescribe.Common;
using Allscripts.Impact;
using Allscripts.Impact.Utilities;
using eRxWeb.AppCode;

namespace eRxWeb
{
public partial class EULADecline : BasePage
{
    public IPlacementResponse PlacementResponse { get; set; }
        public string SessionAppVersion => Convert.ToString(Session[Constants.SessionVariables.AppVersion]);
        protected void Page_Init(object sender, EventArgs e)
        {
            lnkDefaultStyleSheet.Href += "?version=" + SessionAppVersion;
        }
        protected void Page_Load(object sender, EventArgs e)
    {
        // Google Analytics
        PlacementResponse = TieUtility.GetAdPlacement(Request.Cookies, new TieUtility(), new TIEServiceManager(), new ConfigurationManager());
    }
}

}