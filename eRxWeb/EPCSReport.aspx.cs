using System;
using System.Data;
using Allscripts.ePrescribe.Common;
using Allscripts.Impact;
using eRxWeb;
using Microsoft.Reporting.WebForms;
using eRxWeb.ServerModel;
using eRxWeb.State;
using eRxWeb.ePrescribeSvc;
using RxUser = Allscripts.Impact.RxUser;
using Provider = Allscripts.Impact.Provider;
using ConnectionStringPointer = Allscripts.ePrescribe.DatabaseSelector.ConnectionStringPointer;

namespace Allscripts.Web.UI
{
     
	public partial class EPCSReport : BasePage
	{
        private static IStateContainer PageState;
        protected void Page_Load(object sender, EventArgs e)
        {
            PageState = new StateContainer(Session);
            btnPrint.Attributes.Add("onclick", "return CheckPrint();");
            string providerDetails = string.Empty;
            var precriberId = PageState.GetStringOrEmpty("USERID");
            var timeZone = PageState.GetStringOrEmpty("TimeZone");
            var EpcsStarDate = PageState.GetStringOrEmpty("EPCSStartDate");
            var EpcsEndDate = PageState.GetStringOrEmpty("EPCSEndDate");

            if (!IsPostBack)
            { 
                if (base.IsLicenseShieldEnabled)
                {
                    RxUser rxUser = new RxUser(base.SessionUserID, base.DBID);
                    providerDetails = rxUser.Title + " " + rxUser.FirstName + " " + rxUser.LastName + " " + rxUser.Suffix;
                }

                if (Request.QueryString["From"] != null && Request.QueryString["From"].ToString() == Constants.PageNames.MY_EPCS_REPORTS)
                {
                    DataTable dtZones = SystemConfig.GetTimeZones(base.DBID);

                    if (Request.QueryString.Count == 1 && Session["EPCSStartDate"] != null && Session["EPCSEndDate"] != null)
                    {
                        // Ad Hoc Report
                        this.rptvwrCSReports.Visible = true;
                        rptvwrCSReports.LocalReport.DataSources.Clear();
                        rptvwrCSReports.LocalReport.DataSources.Add(new ReportDataSource("AdhocReport", "epcsAdhocReport"));
                        rptvwrCSReports.LocalReport.DataSources.Add(new ReportDataSource("CurrentLocalTime", "AdhocCurrentAsOf"));
                        rptvwrCSReports.LocalReport.ReportPath = "Reports\\GetAdhocReport.rdlc";
                        rptvwrCSReports.LocalReport.Refresh();
                        string fromDate = Convert.ToDateTime(EpcsStarDate).ToShortDateString();
                        string thruDate = Convert.ToDateTime(EpcsEndDate).ToShortDateString();
                        var shieldUserName = PageState.Cast("AuthenticatedShieldUsers", default(AuthenticatedShieldUser[]))?[0].ShieldLoginID;
                        var ePrescribeUserName = PageState.Cast("AuthenticatedShieldUsers", default(AuthenticatedShieldUser[]))?[0].EPrescribeUserName;
                        epcsAdhocReport.SelectParameters["ShieldUserName"].DefaultValue = shieldUserName;
                        epcsAdhocReport.SelectParameters["ePrescribeUserName"].DefaultValue = ePrescribeUserName;                        
                        
                        ReportParameter[] adhocParams = new ReportParameter[]
                        {
                            new ReportParameter("PrescriberId", precriberId, true),
                            new ReportParameter("FromDate", fromDate, true),
                            new ReportParameter("ThruDate", thruDate, true),
                            new ReportParameter("Timezone", timeZone, true),
                            new ReportParameter("ShieldUserName", shieldUserName, true),
                            new ReportParameter("ePrescribeUserName", ePrescribeUserName, true)
                    };

                        this.rptvwrCSReports.LocalReport.Refresh();
                        this.rptvwrCSReports.LocalReport.SetParameters(adhocParams);
                    }
                    else
                    {

                        this.rptvwrCSReports.Visible = true;
                        rptvwrCSReports.LocalReport.DataSources.Clear();
                        rptvwrCSReports.LocalReport.DataSources.Add(new ReportDataSource("MonthlyReportDataTable", "EpcsMonthlyReportDs"));
                        rptvwrCSReports.LocalReport.DataSources.Add(new ReportDataSource("ReportExecutionsTimeTable", "EpcsReporExecutionTimeTable"));
                        rptvwrCSReports.LocalReport.ReportPath = "Reports\\EpcsMonthlyReport.rdlc";
                        rptvwrCSReports.LocalReport.Refresh();
                        // Monthly Report



                        string executionID = Request.QueryString["ID"];
                        string header = Request.QueryString["Header"];
                        ReportParameter[] parameters = new ReportParameter[6];
                        parameters[0] = new ReportParameter("ReportExecutionID", executionID, false);
                        parameters[1] = new ReportParameter("ReportHeader", header, false);
                        parameters[2] = new ReportParameter("CurrentDateTime", TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, timeZone).ToString(), false);
                        parameters[3] = new ReportParameter("TimeZoneFullName", timeZone, false);
                        parameters[4] = new ReportParameter("Timezone", dtZones.Select("StandardName = '" + timeZone + "'")[0]["DisplayAbbreviation"].ToString(), false);
                        parameters[5] = new ReportParameter("ProviderInfo", providerDetails, false);
                        rptvwrCSReports.LocalReport.SetParameters(parameters);
                        rptvwrCSReports.LocalReport.Refresh();
                        Provider.CreateReportViewAttempt(Convert.ToInt32(executionID), 1,ConnectionStringPointer.REPLICA_DB);
                    }
                }
                else if (Request.QueryString["From"] != null && Request.QueryString["From"].ToString() == Constants.PageNames.EPCS_DAILY_ACTIVITY_REPORT)
                {
                    // Daily Activity report
                    ((PhysicianMasterPageBlank)Master).hideTabs();

                    this.rptvwrCSReports.Visible = true;
                    rptvwrCSReports.LocalReport.DataSources.Clear();
                    rptvwrCSReports.LocalReport.DataSources.Add(new ReportDataSource("EpcsDailyReportDataTable", "EpcsDailyReportDs"));                   
                    rptvwrCSReports.LocalReport.ReportPath = "Reports\\EpcsDailyReport.rdlc";
                    rptvwrCSReports.LocalReport.Refresh();                  

                    ReportParameter[] parameters = new ReportParameter[4];
                    parameters[0] = new ReportParameter("ProviderID", precriberId, false);
                    parameters[1] = new ReportParameter("CreateDate", EpcsStarDate, false);
                    parameters[2] = new ReportParameter("Timezone", timeZone, false);
                    parameters[3] = new ReportParameter("ProviderFullName", providerDetails, false);    
                    try
                    {
                        rptvwrCSReports.LocalReport.SetParameters(parameters);
                        rptvwrCSReports.LocalReport.Refresh();                        
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }

        private int GetePATaskCount()
        {
            int returnValue = 0;

            if (base.IsPOBUser)
            {
                returnValue = ePA.GetePATaskCount(base.SessionLicenseID, base.SessionDelegateProviderID, base.SessionUserID, base.SessionPatientID, base.SessionSSOMode, base.DBID, base.SessionUserID);
            }
            else
            {
                returnValue = ePA.GetePATaskCount(base.SessionLicenseID, string.Empty, string.Empty, base.SessionPatientID, base.SessionSSOMode, base.DBID, base.SessionUserID);
            }

            return returnValue;
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            if (Request.QueryString["From"] != null)
            {
                Response.Redirect(Constants.PageNames.UrlForRedirection(Request.QueryString["From"]));
            }
            else
            {
                SelectPatientComponentParameters selectPatientComponentParameters = new SelectPatientComponentParameters
                {
                    PatientId = PageState.GetStringOrEmpty(Constants.SessionVariables.PatientId)
                };
                RedirectToSelectPatient(null, selectPatientComponentParameters);
            }
        }
        
        private void setupTabs()
        {
            int tasks = 0;

            if (Session["LICENSEID"] != null)
            {
                string licenseID = Session["LICENSEID"].ToString();

                if (base.IsUserAPrescribingUserWithCredentials)
                {
                    
                        tasks = Impact.Provider.GetTaskCountForProvider(licenseID, Session["USERID"].ToString(), base.DBID, base.SessionUserID);
                    
                }
                else
                {
                   
                        if (base.IsPOBUser & !base.IsPOBViewAllProviders)
                        {
                            // get task count only for selected Providers associated to POB
                            tasks = TaskManager.GetTaskListScriptCount(licenseID, new Guid(base.SessionUserID), (int)Constants.PrescriptionTaskType.SEND_TO_ADMIN, base.DBID, base.SessionUserID);
                        }
                        else
                        {
                            // get task count all "assistant" tasks
                            tasks = TaskManager.GetTaskListScriptCount(licenseID, (int)Constants.PrescriptionTaskType.SEND_TO_ADMIN, base.DBID, base.SessionUserID);
                        }
                    }              
            }

            ((PhysicianMasterPageBlank)Master).toggleTabs("myerx", tasks);
        }
    }
}
