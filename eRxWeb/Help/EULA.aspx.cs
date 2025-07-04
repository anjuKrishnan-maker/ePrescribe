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
public partial class Help_EULA : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        ePrescribeSvc.ePrescribeSvc eps = new ePrescribeSvc.ePrescribeSvc();
        eps.Url = ConfigurationManager.AppSettings["ePrescribeSvc.eprescribesvc"].ToString();
        ((HelpMasterPage)this.Master).CurrentPage = HelpMasterPage.HelpPage.EULA;
        eulaContent.Controls.Clear();
        eulaContent.Controls.Add(new LiteralControl(eps.GetEULA()));
    }
}

}