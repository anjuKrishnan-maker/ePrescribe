//Revision History
//HA/Dhiraj 10 Oct. 2006 : 110 Qunatity units seem inappropriate.
//HA/Dhiraj 13 Oct 06    : 141 Quantity in words not displayed properly in the script 
//HA/JJ 24 Oct 06        : A new script file (AL.xsl) was added 
//HA/AKS March08         :If its controlled substance then display DEANO or else dont display.
/******************************************************************************
* Change History
* Date:      Author:                    Description:
* -----------------------------------------------------------------------------
* 06/14/2009   Dharani Reddem           ASSMCA field added for Puerto Rico controlled substance prescription .
*                                       
*******************************************************************************/

using System;
using System.Data;
using System.IO;
using System.Xml;
using System.Xml.Xsl;
using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.ExtensionMethods;
using Allscripts.Impact;
using Authorize = eRxWeb.AppCode.Authorize;
using eRxWeb.ePrescribeSvc;
using eRxWeb.State;
using System.Web;

namespace eRxWeb
{
public partial class PrintScript : System.Web.UI.Page //this page does not work when BasePage is inherited!
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
            try
            {
                if (Session["MultipleRxID"] != null)
                {
                    PrintMode printMode;
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
                    request.RxIdList = new []{ Session["MultipleRxID"].ToGuid() };
                    request.SiteId = Convert.ToInt32(Session[Constants.SessionVariables.SiteId]);
                    request.TimeZone = Convert.ToString(Session["TimeZone"]);
                    request.UserType = Session["UserType"].ToEnum<Constants.UserCategory>().ToString().ToEnum<UserCategory>();
                    request.DbID = (ConnectionStringPointer?)((Allscripts.ePrescribe.DatabaseSelector.ConnectionStringPointer)Session[Constants.SessionVariables.DbId]);

                    var response = EPSBroker.PrintRxs(request);

                    Response.Write(response.Html);
                }
            }
            catch (Exception ex)
            {
                Response.Write("Error in Printing" + ex.Message.ToString());
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
    }
}