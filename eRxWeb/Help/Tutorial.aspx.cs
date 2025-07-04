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
public partial class Help_Tutorial : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            this.Form.Attributes.Add("autocomplete", "off");
            ((HelpMasterPage)this.Master).CurrentPage = HelpMasterPage.HelpPage.TUTORIALS;
            //sideAccordion.Visible = !string.IsNullOrEmpty(SessionUserID);
            if (string.IsNullOrEmpty(SessionUserID))
            {
                pTutorials.InnerText = "Login above, for access to the ePrescribe Training Tutorials";
                pTutorials.Style.Add("font-weight", "Bold");
            }
            else
            {
                Response.Redirect("/media/tutorials/FullVideo/curriculum/1.html");
            }

        }
    }
}

}