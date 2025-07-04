using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Allscripts.ePrescribe.Common;
using Allscripts.Impact;
using System.Threading;

namespace eRxWeb
{
public partial class SponsoredLinkPage : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            if (Request.QueryString["SeqNum"] != null && Request.QueryString["MessageAuditID"] != null && Request.QueryString["Partner"] != null)
            {
                SponsoredLink clickLink = new SponsoredLink();
                SponsoredLink link = clickLink.GetLinkItem(Convert.ToInt32(Request.QueryString["MessageAuditID"]), (Constants.SponsoredLinkPartner)Convert.ToInt32(Request.QueryString["Partner"]));

                int lineNumber = 0; 
                lineNumber = Convert.ToInt32(Request.QueryString["SeqNum"].ToString());
                SponsoredLink.InsertSponsoredLinkAuditLog(Convert.ToInt32(Request.QueryString["MessageAuditID"].ToString()), Constants.SponsoredLinkLogType.CLICK, base.SessionUserID);
                ThreadPool.QueueUserWorkItem(new WaitCallback(updateCounter), link.MessageID);
                Response.Redirect(link.MessageLines[lineNumber].URL);
            }
        }
    }

    private void updateCounter(object messageID)
    {
        SponsoredLink.UpdateSponsoredLinks((int)messageID);
    }
}

}