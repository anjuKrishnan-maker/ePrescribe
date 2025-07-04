using System;
using Allscripts.ePrescribe.Common;

namespace eRxWeb
{
    public partial class ManualIDProofingForm : BasePage
    {
        protected void Page_Init(object sender, EventArgs e)
        {
            lnkDefaultStyleSheet.Href += "?version=" + SessionAppVersion;
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (base.CurrentUser == null)
            {
                Response.Redirect(Constants.PageNames.LOGOUT);
            }
        }
    }
}