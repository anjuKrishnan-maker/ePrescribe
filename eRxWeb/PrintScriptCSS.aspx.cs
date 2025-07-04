using System;
using System.Linq;
using Allscripts.ePrescribe.Common;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using Allscripts.ePrescribe.ExtensionMethods;
using eRxWeb.ePrescribeSvc;
using ConnectionStringPointer = eRxWeb.ePrescribeSvc.ConnectionStringPointer;
using eRxWeb.State;
using Authorize = eRxWeb.AppCode.Authorize;
using System.IO;
using System.Web;

namespace eRxWeb
{
public partial class PrintScriptCSS : System.Web.UI.Page //this page does not work when BasePage is inherited!
{
        public IStateContainer PageState { get; set; }
        private void Page_PreInit(object sender, EventArgs e)
        {
            this.PageState = new StateContainer(Session);
            OnAuthorization();
        }
        protected void Page_Load(object sender, EventArgs e)
        {
        if (!Page.IsPostBack)
        {
            System.Net.ServicePointManager.CertificatePolicy = new MyPrintScriptPolicy();
            try
            {
                if (Session["RxIDArray"] != null)   // Made changes on July 4th 2006
                {
                    PrintMode printMode;
                    String[] rxarray = Session["RxIDArray"] as string[];
                    if ((Request.QueryString["PrintBy"] != null && Request.QueryString["PrintBy"] == "4Row"))
                    {
                        printMode = PrintMode.FOUR_ROW;
                    }
                    else if ((Request.QueryString["PrintBy"] != null && Request.QueryString["PrintBy"] == "4Up"))
                    {
                        printMode = PrintMode.FOUR_UP;
                    }
                    else
                    {
                        printMode = PrintMode.ONE_UP;
                    }

                        var request = EPSBroker.ServiceRequest<PrintRxRequest>();
                        request.DelegateProviderId = Convert.ToString(Session[Constants.SessionVariables.DelegateProviderId]).ToGuidOr0x0();
                        request.LicenseId = Convert.ToString(Session[Constants.SessionVariables.LicenseId]).ToGuidOr0x0();
                        request.State = string.IsNullOrWhiteSpace(Request.QueryString["StateOverride"]) ? Convert.ToString(Session[Constants.SessionVariables.PracticeState]) : Request.QueryString["StateOverride"];
                        request.PrintAction = PrintAction.OUTPUT_HTML;
                        request.PrintMode = printMode;
                        request.RxIdList = rxarray?.Select(_ => _.ToGuidOr0x0()).ToArray();
                        request.SiteId = Convert.ToInt32(Session[Constants.SessionVariables.SiteId]);
                        request.TimeZone = Convert.ToString(Session["TimeZone"]);
                        request.UserType = Session["UserType"].ToEnum<Constants.UserCategory>().ToString().ToEnum<UserCategory>();
                        request.DbID = (ConnectionStringPointer?) ((Allscripts.ePrescribe.DatabaseSelector.ConnectionStringPointer)Session[Constants.SessionVariables.DbId]);

                        var response = EPSBroker.PrintRxs(request);

                        lnkStyleSheet.Attributes["href"] = response.StyleSheet;
                        scriptContent.InnerHtml = response.Html;
                }
            }
            catch (Exception ex) // Explicitly catching the error here .
            {
                    throw ex;
            }

        }
    }

        private void OnAuthorization()
        {
            string strCurrentPage = GetCurrentPageName();
            if (Authorize.Pages.ShouldSkipAuthorization(strCurrentPage))
                return;
            bool isAuthorized = Authorize.AuthorizationManager.Process(PageState, strCurrentPage);

            if (!isAuthorized)
                Response.Redirect("~/" + Constants.PageNames.EXCEPTION, true);
        }

        private string GetCurrentPageName()
        {
            return Path.GetFileName(HttpContext.Current.Request.Path);
        }


        //Dhiraj 110
    }

   

    public class MyPrintScriptPolicy : ICertificatePolicy
{
    public bool CheckValidationResult(
          ServicePoint srvPoint
        , X509Certificate certificate
        , WebRequest request
        , int certificateProblem)
    {
        return true;
    } // end CheckValidationResult
} 

}