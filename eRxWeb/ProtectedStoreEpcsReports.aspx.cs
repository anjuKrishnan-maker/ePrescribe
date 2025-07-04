using Allscripts.ePrescribe.Shared.Logging;
using Allscripts.Impact;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eRxWeb
{
    
    public partial class ProtectedStoreEpcsReports: BasePage 
    {
        private static readonly ILoggerEx logger = LoggerEx.GetLogger();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                try
                {
                    formToBePosted.Style["display"] = "Inline";
                    formToBePosted.Action = ConfigKeys.EPCSProtectedStorePortalUrl;

                    SSOTokenInBase64String.Value = Convert.ToBase64String(Encoding.UTF8.GetBytes(PageState.GetStringOrEmpty("ShieldSecurityToken")));
                    DateTime dateForReportMonth = DateTime.Now.AddMonths(-1); // always attempt to get report for previous month
                    ReportType.Value = "Monthly";
                    Month.Value = dateForReportMonth.ToString("MMMM");
                    Year.Value = dateForReportMonth.Year.ToString();
                    TimeZone.Value = PageState.GetString("TimeZone", TimeZoneInfo.Local.StandardName);
                }
                catch (Exception ex)
                {
                    logger.Debug($"Error when getting EPCS Report Parameters {ex}");
                }
            }
            

        }

   }
}