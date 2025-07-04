using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;

namespace eRxWeb
{
public partial class Controls_DeluxeEPCSCommonPanels : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        ePrescribeSvc.ePrescribeSvc eps = new ePrescribeSvc.ePrescribeSvc();
        eps.Url = ConfigurationManager.AppSettings["ePrescribeSvc.eprescribesvc"].ToString();        
        eulaContent.Controls.Clear();
        eulaContent.Controls.Add(new LiteralControl(eps.GetEULA()));
    }
}
}