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
public partial class StartOver : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {        
        base.ClearMedicationInfo(false);

        if (Request.QueryString["savepat"] != null && Request.QueryString["savepat"].ToString() == "y")
        {
            base.RefreshPatientActiveDiagnosis();
            base.RefreshPatientActiveMeds();
        }
        else
        {
            base.ClearPatientInfo();
        }

        string msg = "";
        if (Request.QueryString["msg"] != null && Request.QueryString["msg"].ToString() != "")
        {
            msg = Request.QueryString["msg"].ToString();
        }
        DefaultRedirect("StartOver=Y&Msg=" + Server.UrlEncode(msg.ToString().Replace("&amp;", "&")));
    }
}

}