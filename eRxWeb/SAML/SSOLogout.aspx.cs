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
public partial class SSOLogout : BasePage 
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            for (int i = 0; i < Request.Cookies.Count; i++)
                Response.Cookies[Request.Cookies[i].Name].Expires = DateTime.Today.AddYears(-1);
            Session.Abandon();
/*            if (Request.QueryString["Timeout"] != null && Request.QueryString["Timeout"] == "YES")
                Response.Redirect("login.aspx?Timeout=YES");
            else
                Response.Redirect("login.aspx");*/

        }
    }
}

}