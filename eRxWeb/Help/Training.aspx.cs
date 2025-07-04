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
using Allscripts.Impact;

namespace eRxWeb
{
public partial class Help_Training : BasePage
{

    protected void  Page_Init (object sender, EventArgs e) 
    {
        ViewStateUserKey = Session.SessionID; // Added ViewStateUserKey to prevent One-Click attack (to prevent vulnerability  POST Parameters Accepted as GET Parameters)
 
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        this.Form.Attributes.Add("autocomplete", "off");
        if (!IsPostBack)
        {
            ((HelpMasterPage)this.Master).CurrentPage = HelpMasterPage.HelpPage.TRAINING;            
        }
    }
}

}