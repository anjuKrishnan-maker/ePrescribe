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
using TieServiceClient;
using System.Collections.Generic;
using System.Collections.Specialized;
using Allscripts.Impact;
using eRxWeb.AppCode;

namespace eRxWeb
{
public partial class DeluxeTermsConditions : BasePage
{
    public IPlacementResponse PlacementResponse { get; set; }

    protected void Page_Load(object sender, EventArgs e)
    {
        PlacementResponse = TieUtility.GetAdPlacement(Request.Cookies, new TieUtility(), new TIEServiceManager(), new Allscripts.Impact.Utilities.ConfigurationManager());

        ePrescribeSvc.ePrescribeSvc eps = new ePrescribeSvc.ePrescribeSvc();
        eps.Url = ConfigurationManager.AppSettings["ePrescribeSvc.eprescribesvc"].ToString();
        eulaContent.Controls.Clear();
        eulaContent.Controls.Add(new LiteralControl(eps.GetEULA()));
    }
}

}