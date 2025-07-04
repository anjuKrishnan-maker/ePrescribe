using System;
using System.Data;
using System.IO;
using Allscripts.ePrescribe.Common;
using Allscripts.Impact;
using eRxWeb;
using eRxWeb.AppCode;
using Microsoft.Reporting.WebForms;
using eRxWeb.State;
using eRxWeb.ePrescribeSvc;
using RxUser = Allscripts.Impact.RxUser;


namespace Allscripts.Web.UI
{
    public partial class PrintCSReports : BasePage
    {
        private IStateContainer pageState;

        protected void Page_PreInit(object sender, EventArgs e)
        {
            pageState = new StateContainer(Session);
        }
      

        protected void Page_Load(object sender, EventArgs e)
        {
            rptvwrCSReportPrint.DocumentMapCollapsed = true;
            var precriberId = PageState.GetStringOrEmpty("USERID");
            var timeZone = PageState.GetStringOrEmpty("TimeZone");
            var EpcsStarDate = PageState.GetStringOrEmpty("EPCSStartDate");
            var EpcsEndDate = PageState.GetStringOrEmpty("EPCSEndDate");

            if (!IsPostBack)
            {
               
                string providerDetails = string.Empty;

                if (base.IsLicenseShieldEnabled)
                {
                    RxUser rxUser = new RxUser(base.SessionUserID, base.DBID);
                    providerDetails = rxUser.Title + " " + rxUser.FirstName + " " + rxUser.LastName + " " + rxUser.Suffix;
                }
                try
                {                   
                    EpcsReport objEpcsReport = new EpcsReport();

                    if (Request.QueryString["From"] != null && Request.QueryString["From"].ToString() == Constants.PageNames.MY_EPCS_REPORTS)
                    {
                        DataTable dtZones = SystemConfig.GetTimeZones(base.DBID);

                        if (Request.QueryString.Count == 1 && !string.IsNullOrEmpty(EpcsStarDate) && !string.IsNullOrEmpty(EpcsEndDate))
                        {
                            // Ad Hoc Report                          

                            string fromDate = Convert.ToDateTime(EpcsStarDate).ToShortDateString();
                            string thruDate = Convert.ToDateTime(EpcsEndDate).ToShortDateString();
                            var shieldUserName = PageState.Cast("AuthenticatedShieldUsers", default(AuthenticatedShieldUser[]))?[0].ShieldLoginID;
                            var ePrescribeUserName = PageState.Cast("AuthenticatedShieldUsers", default(AuthenticatedShieldUser[]))?[0].EPrescribeUserName;
                            DataSet dsAdhoc = objEpcsReport.GetAdhocReport(precriberId, fromDate, thruDate, timeZone, shieldUserName, ePrescribeUserName);
                            DataSet dsCurrentDateTime = objEpcsReport.GetCurrentAsOfLocal(timeZone);
                        
                            this.rptvwrCSReportPrint.Visible = true;
                            rptvwrCSReportPrint.LocalReport.DataSources.Clear();
                            rptvwrCSReportPrint.LocalReport.DataSources.Add(new ReportDataSource("AdhocReport", dsAdhoc.Tables[0]));
                            rptvwrCSReportPrint.LocalReport.DataSources.Add(new ReportDataSource("CurrentLocalTime", dsCurrentDateTime.Tables[0]));
                            rptvwrCSReportPrint.LocalReport.ReportPath = "Reports\\GetAdhocReport.rdlc";
                            rptvwrCSReportPrint.LocalReport.Refresh();


                            ReportParameter[] adhocParams = new ReportParameter[]
                              {
                            new ReportParameter("PrescriberId", precriberId, true),
                            new ReportParameter("FromDate", fromDate, true),
                            new ReportParameter("ThruDate", thruDate, true),
                            new ReportParameter("Timezone", timeZone, true),
                            new ReportParameter("ShieldUserName", shieldUserName, true),
                            new ReportParameter("ePrescribeUserName", ePrescribeUserName, true)
                          };

                            this.rptvwrCSReportPrint.LocalReport.Refresh();
                            this.rptvwrCSReportPrint.LocalReport.SetParameters(adhocParams);
                        }
                        else
                        {
                            string executionID = Request.QueryString["ID"];
                            string header = Request.QueryString["Header"];
                            
                            DataSet dsMonthlyReport= objEpcsReport.GetMonthlyReport(executionID, timeZone);
                            DataSet dsReportGenerationTime = objEpcsReport.GetMonthlyReportGenerationTime(executionID, timeZone);

                            this.rptvwrCSReportPrint.Visible = true;
                            rptvwrCSReportPrint.LocalReport.DataSources.Clear();
                            rptvwrCSReportPrint.LocalReport.DataSources.Add(new ReportDataSource("MonthlyReportDataTable", dsMonthlyReport.Tables[0]));
                            rptvwrCSReportPrint.LocalReport.DataSources.Add(new ReportDataSource("ReportExecutionsTimeTable", dsReportGenerationTime.Tables[0]));
                            rptvwrCSReportPrint.LocalReport.ReportPath = "Reports\\EpcsMonthlyReport.rdlc";
                            rptvwrCSReportPrint.LocalReport.Refresh();
                            // Monthly Report
                            
                            ReportParameter[] parameters = new ReportParameter[6];
                            parameters[0] = new ReportParameter("ReportExecutionID", executionID, false);
                            parameters[1] = new ReportParameter("ReportHeader", header, false);
                            parameters[2] = new ReportParameter("CurrentDateTime", TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, timeZone).ToString(), false);
                            parameters[3] = new ReportParameter("TimeZoneFullName", timeZone, false);
                            parameters[4] = new ReportParameter("Timezone", dtZones.Select("StandardName = '" + timeZone + "'")[0]["DisplayAbbreviation"].ToString(), false);
                            parameters[5] = new ReportParameter("ProviderInfo", providerDetails, false);
                            rptvwrCSReportPrint.LocalReport.SetParameters(parameters);
                            rptvwrCSReportPrint.LocalReport.Refresh();
                        }
                    }
                    else if (Request.QueryString["From"] != null && Request.QueryString["From"].ToString() == Constants.PageNames.EPCS_DAILY_ACTIVITY_REPORT)
                    {
                        // Daily Activity report

                        DateTime createDate = Convert.ToDateTime(EpcsStarDate);
                        DataSet dsDailyReport = objEpcsReport.GetEpcsDailyActivityReport(precriberId, createDate, timeZone, base.DBID);

                        this.rptvwrCSReportPrint.Visible = true;
                        rptvwrCSReportPrint.LocalReport.DataSources.Clear();
                        rptvwrCSReportPrint.LocalReport.DataSources.Add(new ReportDataSource("EpcsDailyReportDataTable", dsDailyReport.Tables[0]));
                        rptvwrCSReportPrint.LocalReport.ReportPath = "Reports\\EpcsDailyReport.rdlc";
                        rptvwrCSReportPrint.LocalReport.Refresh();

                        ReportParameter[] parameters = new ReportParameter[4];
                        parameters[0] = new ReportParameter("ProviderID", base.SessionUserID, false);
                        parameters[1] = new ReportParameter("CreateDate", EpcsStarDate, false);
                        parameters[2] = new ReportParameter("Timezone", timeZone, false);
                        parameters[3] = new ReportParameter("ProviderFullName", providerDetails, false);

                        rptvwrCSReportPrint.LocalReport.SetParameters(parameters);
                        rptvwrCSReportPrint.LocalReport.Refresh();
                        
                    }
                    PrintReports();
                }
                catch (Exception ex)
                {
                    Response.Write("Error in Printing " + ex.Message.ToString());
                }


            }
            // Google Analytics
            PlacementResponse = TieUtility.GetAdPlacement(Request.Cookies, new TieUtility(), new TIEServiceManager(), new Allscripts.Impact.Utilities.ConfigurationManager());
        }

        // To Render the Report Viewer Control report to PDF for printing
        protected void PrintReports()
        {
           
            Warning[] warnings;
            string reporttype = "PDF";
            string mimeType;
            string encoding;
            string extension;
            string[] streamids;
            string deviceInfo = string.Empty;
            deviceInfo = "<DeviceInfo>" +
               "<OutputFormat>PDF</OutputFormat>" +
               "<EmbedFonts>None</EmbedFonts>"+
               "<PageWidth>12.5in</PageWidth>" +
               "<PageHeight>8.5in</PageHeight>" +
               "<MarginTop>.3in</MarginTop>" +
               "<MarginLeft>1in</MarginLeft>" +
               "<MarginRight>1in</MarginRight>" +
               "<MarginBottom>.1in</MarginBottom>" +
               "</DeviceInfo>";

            byte[] bytes = rptvwrCSReportPrint.LocalReport.Render(reporttype, deviceInfo, out mimeType, out encoding, out extension,
                                                        out streamids, out warnings);

            MemoryStream oStream = new MemoryStream(bytes);
            oStream.Read(bytes, 0, bytes.Length - 1);

            Response.Clear();
            Session["printPdf"] = oStream;
            redr();
        }
        private void redr()
        {
            ClientScript.RegisterStartupScript(this.GetType(), null, $"printPdf('{Constants.PageNames.PRINT_PDF}');", true);
        }
    }
}