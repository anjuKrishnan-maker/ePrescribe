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
using System.Diagnostics;
using Allscripts.Impact;
using static Allscripts.ePrescribe.Common.Constants;

namespace eRxWeb
{
public partial class Exception1 : BasePage 
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            var message = "Error could not be retrieved from Session or SessionDBError object";

            if (Session[SessionVariables.CURRENT_ERROR] == null)
            {
                if (!string.IsNullOrEmpty(Global.CurrentSessionDBError))
                {
                    message = $"SessionDB Error: {Global.CurrentSessionDBError}";
                    Global.CurrentSessionDBError = string.Empty;
                }
            }
            else
            {
                message = Session[SessionVariables.CURRENT_ERROR].ToString();
            }

            Global.LogError(message);


            SessionTracker.Dump(Session["CurrentErrorID"].ToString(), base.DBID);

            if (Session["CurrentErrorID"] != null)
            {
                lblErrorID.Text = string.Concat("Error Reference ID = ", Session["CurrentErrorID"].ToString());
            }
        }
    }

    protected void Page_LoadComplete(object sender, EventArgs e)
    {
        ((PhysicianMasterPageBlank)Master).hideTabs();
    }

    protected void btnBack_Click(object sender, EventArgs e)
    {
        if (Session["urlpath"] != null)
            Server.Transfer(Session["urlpath"].ToString());
    }
}

}