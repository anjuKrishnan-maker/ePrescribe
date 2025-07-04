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
using Telerik.Web.UI;
using Allscripts.Impact;
using eRxWeb.State;
using eRxWeb.AppCode;
using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.DatabaseSelector;
using System.Web.Security.AntiXss;
using System.Text;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace eRxWeb
{
    public partial class RedirectToAngular : BasePage
    {

        public string RedirectPage { get; set; }

        public string RetrieveClientScriptToInvokeAngularComponent(string componentName, string componentParameters)
        {
            string clientScriptToInvokeAngularComponent = string.Empty;
            if (!string.IsNullOrEmpty(componentName))
            {
                if (string.IsNullOrEmpty(componentParameters))
                {
                    clientScriptToInvokeAngularComponent = "window.parent.NavigateToAngularComponent('" + componentName +
                                                           "',null);";
                }
                else
                {
                    clientScriptToInvokeAngularComponent = "window.parent.NavigateToAngularComponent('" + componentName +
                                                           "','" + componentParameters + "');";
                }

            }
            return clientScriptToInvokeAngularComponent;
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["USERID"] == null)
            {
                Response.Redirect(Constants.PageNames.LOGIN);
            }
            if (!IsPostBack)
            {
                string componentName = Request.QueryString["componentName"];
                string componentParameters = Request.QueryString["componentParameters"];
                string clientScriptToInvokeAngularComponent = RetrieveClientScriptToInvokeAngularComponent(
                  AntiXssEncoder.HtmlEncode(componentName, false), componentParameters.ToJsonEncode());
                ClientScript.RegisterClientScriptBlock(this.GetType(), "", clientScriptToInvokeAngularComponent,
                    true);
            }

        }        
    }
}