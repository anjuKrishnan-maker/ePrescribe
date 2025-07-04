using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.DatabaseSelector;
using eRxWeb.AppCode.Authorize;
using eRxWeb.State;

namespace eRxWeb
{
    public partial class PrintRxInfo : System.Web.UI.Page //this page does not work when BasePage is inherited!
    {
        public IStateContainer PageState { get; set; }
        private void Page_PreInit(object sender, EventArgs e)
        {
            this.PageState = new StateContainer(Session);
            OnAuthorization();
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            string htmlO = string.Empty;
            int rxInfoHtmlContentCounter = 0;

            List<string> rxInfoDetailsIdList = new List<string>();
            rxInfoDetailsIdList = (List<string>)Session["RxInfoDetailsIdList"];

            foreach (var  rxInfoDetailsId in rxInfoDetailsIdList)
            {
                ++rxInfoHtmlContentCounter;
                if (rxInfoHtmlContentCounter > 1)
                {
                    htmlO += "<div style='page-break-after:always; height:1px; }'></div>";
                }

                string rxInfoHtmlContent = EPSBroker.GetRxInfoHtmlContent(rxInfoDetailsId, (ConnectionStringPointer)Session["DBID"]);
                htmlO += System.Web.HttpUtility.HtmlDecode(rxInfoHtmlContent);
            }
           

            litHtmlOutput.Text = htmlO;
        }

        private void OnAuthorization()
        {
            string strCurrentPage = GetCurrentPageName();
            if (Pages.ShouldSkipAuthorization(strCurrentPage))
                return;
            bool isAuthorized = AuthorizationManager.Process(PageState, strCurrentPage);

            if (!isAuthorized)
                Response.Redirect("~/" + Constants.PageNames.EXCEPTION, true);
        }

        private string GetCurrentPageName()
        {
            return Path.GetFileName(HttpContext.Current.Request.Path);
        }
    }
}