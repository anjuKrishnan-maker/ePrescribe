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

namespace eRxWeb
{
public partial class Help_InterfaceRequestConfirmation : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ((HelpMasterPageNew)this.Master).CurrentPage = HelpMasterPageNew.HelpPage.INTERFACE_CONFIRM;
        }
    }
}

}