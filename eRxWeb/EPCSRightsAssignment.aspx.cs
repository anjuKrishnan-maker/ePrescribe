using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Allscripts.ePrescribe.Common;


namespace eRxWeb
{
    public partial class EPCSRightsAssignment : BasePage
    {
        protected const string ReportID = "EPCSRightsAssignment";

        protected void Page_Load(object sender, EventArgs e)
        {
            Server.Transfer(Constants.PageNames.MULTIPLE_VIEW_REPORT + "?ReportID=" + ReportID.Trim());
        }

        protected void Page_LoadComplete(object sender, EventArgs e)
        {
            ((PhysicianMasterPageBlank)Master).hideTabs();
        }
    }
}