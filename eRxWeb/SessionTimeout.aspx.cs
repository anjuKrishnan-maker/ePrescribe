using System;
using Allscripts.Impact;
using Allscripts.ePrescribe.Common;

namespace eRxWeb
{
    public partial class SessionTimeOut : BasePage
    {
        protected void Page_Init(object sender, EventArgs e)
        {
            lnkDefaultStyleSheet.Href += "?version=" + SessionAppVersion;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (CurrentUser == null)
            {
                Response.Redirect(Constants.PageNames.LOGOUT);
            }
        }
    }
}