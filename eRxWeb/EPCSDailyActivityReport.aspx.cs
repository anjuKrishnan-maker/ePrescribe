using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Allscripts.ePrescribe.Common;
using eRxWeb.ServerModel;
using System.Web.Script.Serialization;
using eRxWeb.AppCode;

namespace eRxWeb
{
    public partial class EPCSDailyActivityReport : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                ArrayList last7Days = new ArrayList();

                for (int day = 1; day < 8; day++)
                {
                    last7Days.Add(DateTime.Today.AddDays(-day).ToShortDateString());
                }

                rptLast7Days.DataSource = last7Days;
                rptLast7Days.DataBind();

                //radReportDateCalendar.RangeMaxDate = DateTime.Today.AddDays(-1);
                calReportDate.SelectionMode = CalendarSelectionMode.Day;
            }
        }

        protected void Page_LoadComplete(object sender, EventArgs e)
        {
            ((PhysicianMasterPageBlank)Master).hideTabs();
        }

        protected void lbReportDate_Command(object sender, CommandEventArgs e)
        {
            sendToReportPage(e.CommandArgument.ToString());
        }

        protected void calReportDate_SelectionChanged(object sender, EventArgs e)
        {
            sendToReportPage(calReportDate.SelectedDate.ToShortDateString());
        }

        protected void calReportDate_DayRender(object sender, DayRenderEventArgs e)
        {
            e.Day.IsSelectable = e.Day.Date < DateTime.Today;
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            SelectPatientComponentParameters selectPatientComponentParameters = new SelectPatientComponentParameters
            {
                PatientId = Convert.ToString(Session[Constants.SessionVariables.PatientId])
            };
            RedirectToSelectPatient(null, selectPatientComponentParameters);
        }
        public void RedirectToSelectPatient(string queryString, SelectPatientComponentParameters selectPatientComponentParameters)
        {
            if ((!string.IsNullOrWhiteSpace(queryString) && queryString.Contains("StartOver=Y")) || 
                Convert.ToBoolean(Session[Constants.SessionVariables.AppComponentAlreadyInitialized]))//since it is coming from Angular component
            {
                //WHEN APP COMPONENT ALREADY EXISTS
                var componentParameters = new JavaScriptSerializer().Serialize(selectPatientComponentParameters);
                Response.Redirect(Constants.PageNames.REDIRECT_TO_ANGULAR + "?componentName=" + Constants.PageNames.SELECT_PATIENT + "&componentParameters=" + componentParameters);
            }
            else
            {
                //LOGIN WORKFLOW WHEN NO APP COMPONENT IS THERE
                Session[Constants.SessionVariables.AppComponentAlreadyInitialized] = true;
                Response.Redirect(RedirectHelper.GetRedirectToSPAUrl(string.Empty, false));
            }
        }
        private void sendToReportPage(string reportDate)
        {
            Session["EPCSStartDate"] = reportDate;

            Response.Redirect(Constants.PageNames.EPCS_REPORT + "?From=" + Constants.PageNames.EPCS_DAILY_ACTIVITY_REPORT);
        }
    }
}