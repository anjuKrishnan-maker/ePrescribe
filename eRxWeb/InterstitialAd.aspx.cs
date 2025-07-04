using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Allscripts.Impact;
using Allscripts.Impact.Utilities;
using eRxWeb.AppCode;

namespace eRxWeb
{
public partial class InterstitialAd : BasePage
{
        protected void Page_Init(object sender, EventArgs e)
        {
            lnkDefaultStyleSheet.Href += "?version=" + SessionAppVersion;
        }
        protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            adControl.FeaturedModule = base.FeaturedModule.Type;
            base.FeaturedModule = null;
        }
            // Google Analytics
        PlacementResponse = TieUtility.GetAdPlacement(Request.Cookies, new TieUtility(), new TIEServiceManager(), new ConfigurationManager());
    }
}

}