using Allscripts.ePrescribe.DatabaseSelector;
using eRxWeb.AppCode;
using eRxWeb.ServerModel;
using eRxWeb.State;
using System;
using System.Collections.Generic;
using Allscripts.ePrescribe.Shared.Logging;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Http;

namespace eRxWeb.Controller
{
    public partial class ImportantInfoApiController : ApiController
    {
        private static ILoggerEx logger = LoggerEx.GetLogger();

        [HttpPost]
        public ApiResponse LoadImportantInfo()
        {
            IStateContainer pageState = new StateContainer(HttpContext.Current.Session);
            List<ImportantInfoModel> impInfos = new List<ImportantInfoModel>();
            var response = new ApiResponse();
            try
            {
                using (var timer = logger.StartTimer("LoadImportantInfo"))
                {
                    impInfos = checkImportantInfo(pageState);
                    response.Payload = impInfos;
                    var impInfosData = string.Join(",", impInfos.Select(u => " InfoBody: " + u.InfoBody + " InfoTitle: " + u.InfoTitle + " IsToday: " + u.IsToday + " StartDate: " + u.StartDate));
                    timer.Message = $"<impInfosData>{impInfosData}</impInfosData>";
                }
            }
            catch (Exception ex)
            {
                var errorMessage = ApiHelper.AuditException(ex.ToString(), pageState);
                logger.Error("LoadImportantInfo Exception: " + ex.ToString());

                response.ErrorContext = new ErrorContextModel()
                {
                    Error = ErrorTypeEnum.ServerError,
                    Message = errorMessage
                };
            }
            return response;
        }
        internal static List<ImportantInfoModel> checkImportantInfo(IStateContainer pageState)
        {
            DataRowCollection drc = Allscripts.Impact.SystemConfig.GetImportantInfo(true, false, false, pageState.Cast<ConnectionStringPointer>("DBID", ConnectionStringPointer.ERXDB_SERVER_1));
            List<ImportantInfoModel> impInfos = new List<ImportantInfoModel>();
            if (drc != null && drc.Count > 0)
            {
                foreach (DataRow dr in drc)
                {
                    ImportantInfoModel data = new ImportantInfoModel();
                    if (Convert.ToDateTime(dr["StartDate"]).ToShortDateString() == DateTime.Today.ToShortDateString())
                    {
                        data.IsToday = true;
                    }
                    else
                    {
                        data.IsToday = false;
                    }
                    data.StartDate = data.IsToday ? "Today" : Convert.ToDateTime(dr["StartDate"]).ToShortDateString();
                    data.InfoTitle = dr["InfoTitle"].ToString();
                    data.InfoBody = dr["InfoBody"].ToString();
                    impInfos.Add(data);
                    //if (data.IsToday)
                    //{
                    //    thismsg += "<i>Today</i><br><B>" + dr["InfoTitle"].ToString() + "</B><BR>";
                    //}
                    //else
                    //{
                    //    thismsg += "<i>" + Convert.ToDateTime(dr["StartDate"]).ToShortDateString() + "</i><BR><B>" + dr["InfoTitle"].ToString() + "</B><BR>";
                    //}
                    //thismsg += dr["InfoBody"].ToString();

                    //if (drc.Count > 1)
                    //   thismsg += "<HR>";

                    //if the database instructs us to always show this info during the life of the message, show it.
                    //if (dr["ExpandOnRightPanel"] != Convert.DBNull && Convert.ToInt32(dr["ExpandOnRightPanel"]) > 0)
                    //{
                    //    cpeExtender.Collapsed = false;
                    //}

                    ////if the news came out today, show it for sure
                    //if (isToday)
                    //{
                    //    cpeExtender.Collapsed = false;
                    //}
                }
                // panelImportant.Controls.Add(new LiteralControl(thismsg));
                // panelImportant.Visible = true;
                // rightHandPanelHeader.Visible = true;
            }
            else
            {
                //panelImportant.Visible = false;
                //rightHandPanelHeader.Visible = false;
            }
            return impInfos;
        }
    }

}