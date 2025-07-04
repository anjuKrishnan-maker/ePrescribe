using Allscripts.ePrescribe.Common;
using eRxWeb.AppCode;
using eRxWeb.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TieServiceClient;

namespace eRxWeb
{
    public partial class LogRxDisplay : BasePage
    {
        public IPlacementResponse PlacementResponse { get; private set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            IStateContainer pageState = new StateContainer(HttpContext.Current.Session);
            var ppage = Request.QueryString["page"];
            if (ApiHelper.IsTieAdPlacementEnabled && pageState.GetBooleanOrFalse(Constants.SessionVariables.ShouldShowAds))
            {
                PlacementResponse = TieUtility.GetAdForMasterPage($"/{ppage}", Request.Cookies, new TieUtility());
            }
        }
    }
}