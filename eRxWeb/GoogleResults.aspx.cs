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
using eRxWeb;
using eRxWeb.AppCode;

namespace Allscripts.Web.UI
{
	public partial class GoogleResults : BasePage 
	{

		protected void Page_Load(object sender, EventArgs e)
		{
            // Google Analytics
            PlacementResponse = TieUtility.GetAdPlacement(Request.Cookies, new TieUtility(), new TIEServiceManager(), new ConfigurationManager());
        }
	}
}