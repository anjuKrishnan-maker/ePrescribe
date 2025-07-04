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

namespace eRxWeb
{
public partial class ProcessLink : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Request.QueryString["clientid"] != null &&
            Request.QueryString["linkid"] != null &&
            Request.QueryString["linkurl"] != null &&
            Request.QueryString["userid"] != null)
        {
            Allscripts.Impact.Partner.LinkLogInsert(
                Request.QueryString["clientid"].ToString(),
                Request.QueryString["linkid"].ToString(),
                Request.QueryString["linkurl"].ToString(),
                Request.QueryString["userid"].ToString(),
                base.DBID);

            Response.Redirect(Constants.PageNames.UrlForRedirection(Request.QueryString["linkurl"].ToString()));
        }
    }
}

}