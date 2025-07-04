using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eRxWeb
{
public partial class Controls_GoogleReCAPTCHA : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        recaptcha.PrivateKey = System.Configuration.ConfigurationManager.AppSettings["GoogleReCAPTCHAPrivateKey"].ToString();
        recaptcha.PublicKey = System.Configuration.ConfigurationManager.AppSettings["GoogleReCAPTCHAPublicKey"].ToString();
    }
}
}