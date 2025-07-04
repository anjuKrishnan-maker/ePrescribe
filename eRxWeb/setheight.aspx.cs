#define TRACE
using System;
using System.Data;
using System.Collections;
using System.Globalization;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.Shared.Logging;
using eRxWeb.AppCode;
using NLog;
using TieServiceClient;
using Allscripts.Impact;
using Allscripts.Impact.Utilities;
using eRxWeb.ServerModel;

namespace eRxWeb
{
    public partial class setheight : BasePage
    {
        private static ILoggerEx logger = LoggerEx.GetLogger();
        protected void Page_Load(object sender, EventArgs e)
        {
            System.Diagnostics.Trace.Write(Constants.PageNames.SET_HEIGHT + " Page_Load()", Session.SessionID);

            if (IsPostBack)
            {
                if (Request.Form["ht"] == null)
                {
                    Response.Redirect(Constants.PageNames.LOGIN);
                }
                else
                {
                    PageState["PAGEHEIGHT"] = BrowserUtil.GetBrowserHeight(Convert.ToString(Request.Form["ht"]));

                    int width = Convert.ToInt32(hiddenBrowserWidth.Value);
                    PageState["PAGEWIDTH"] = width;

                    logger.Debug($"PageHeight = {PageState["PAGEHEIGHT"]} | PageWidth = {width}");

                    base.BrowserAuditLogInsert(hiddenScreenHeight.Value, hiddenScreenWidth.Value, hiddenBrowserHeight.Value,
                        hiddenBrowserWidth.Value, "ePrescribe-SSO");

                    if (Request.QueryString["dest"] == null)
                    {
                        Response.Redirect(Constants.PageNames.LOGIN);
                    }
                    else
                    {
                        var dest = Constants.PageNames.UrlForRedirection(Request.QueryString["dest"]);
                        System.Diagnostics.Trace.Write(
                            Constants.PageNames.SET_HEIGHT + " Response.Redirect() to " +
                            dest, Session.SessionID);
                        Response.Redirect(dest);
                    }
                }
            }
            else
            {
                form1.Attributes["Action"] = Constants.PageNames.SET_HEIGHT + "?dest=" + Constants.PageNames.UrlForRedirection(Request.QueryString["dest"]);
            }

            // Google Analytics
            PlacementResponse = TieUtility.GetAdPlacement(Request.Cookies, new TieUtility(), new TIEServiceManager(), new ConfigurationManager());
        }
    }
}