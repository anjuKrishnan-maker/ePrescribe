using System;
using Allscripts.ePrescribe.Common;
using Allscripts.Impact;

namespace eRxWeb
{
    public partial class AddressVerifyError : BasePage
    {
        protected void Page_Init(object sender, EventArgs e)
        {
            lnkDefaultStyleSheet.Href += "?version=" + SessionAppVersion;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
           
        }
    }
}