using Allscripts.ePrescribe.DatabaseSelector;
using Allscripts.ePrescribe.Shared.Logging;
using eRxWeb.AppCode;
using eRxWeb.ServerModel;
using eRxWeb.ServerModel.Request;
using eRxWeb.State;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eRxWeb.Controller
{
    public partial class CopayCoverageInfoApiController : ApiController
    {
        private static ILoggerEx logger = LoggerEx.GetLogger();

        [HttpPost]
        public  ApiResponse GetCopayCoverageInfo(MedicationSelectedRequest Med)
        {
            IStateContainer pageState = new StateContainer(HttpContext.Current.Session);
            var response = new ApiResponse();
            CopayCoverage _ls = new CopayCoverage();
            try
            {
                using (var timer = logger.StartTimer("GetCopayCoverageInfo"))
                {
                    _ls = GetCopayCoverageList(Med, pageState);
                    timer.Message = $"<DDI>{Med.DDI}</DDI><FormularyStatus>{Med.FormularyStatus}</FormularyStatus><CopayCoverage>{_ls.ToLogString()}</CopayCoverage>";
                    response.Payload = _ls;
                }
            }
            catch (Exception ex)
            {
                var errorMessage = ApiHelper.AuditException(ex.ToString(), pageState);
                logger.Error("GetCopayCoverageInfo Exception: " + ex.ToString());

                response.ErrorContext = new ErrorContextModel()
                {
                    Error = ErrorTypeEnum.ServerError,
                    Message = errorMessage
                };
            }
            return response;
        }

        [HttpPost]
        public ApiResponse GetCopayCoverageInfoFromSession()
        {
            IStateContainer pageState = new StateContainer(HttpContext.Current.Session);
           
            var response = new ApiResponse();
            try
            {
                CopayCoverage _ls = new CopayCoverage();
                using (var timer = logger.StartTimer("GetCopayCoverageInfoFromSession"))
                {
                    _ls.Copay = pageState.GetStringOrEmpty("SelectedCoverageAndCopay");
                    response.Payload = _ls;
                    timer.Message = $"<CopayCoverage>{_ls.ToLogString()}</CopayCoverage>";
                }
            }
            catch (Exception ex)
            {
                var errorMessage = ApiHelper.AuditException(ex.ToString(), pageState);
                logger.Error("GetCopayCoverageInfoFromSession Exception: " + ex.ToString());

                response.ErrorContext = new ErrorContextModel()
                {
                    Error = ErrorTypeEnum.ServerError,
                    Message = errorMessage
                };
            }
            return response;
           
        }
        internal static CopayCoverage GetCopayCoverageList(MedicationSelectedRequest med, IStateContainer pageState)
        {
           CopayCoverage _ls = new CopayCoverage();
            DataTable dt = GetCopayAndCoverage(pageState, med.DDI, med.FormularyStatus);
            StringBuilder copaySB = new StringBuilder();
            if (dt.Rows.Count > 0)
            {
                bool firstRow = true;

                foreach (DataRow dr in dt.Rows)
                {
                    if (firstRow)
                    {
                        if (dr["Copay"] != Convert.DBNull)
                        {
                            copaySB.Append(dr["Copay"].ToString());
                        }

                        firstRow = false;
                    }

                    if (dr["CoverageDrugSpecific"] != Convert.DBNull)
                    {
                        copaySB.Append(dr["CoverageDrugSpecific"].ToString().Replace("Prior authorization required.", "<b style=\"color: #335dbb;\">Prior authorization required.</b>"));
                    }

                    if (dr["CoverageSummaryLevel"] != Convert.DBNull)
                    {
                        copaySB.Append(dr["CoverageSummaryLevel"].ToString());
                    }
                }
            }
            else
            {
                copaySB.Append("No co-pay information available.");
            }
            if (copaySB.Length == 0)
            {
                copaySB.Append("No co-pay information available.");
            }
            _ls.Copay = copaySB.ToString();
            pageState["SelectedCoverageAndCopay"] = _ls.Copay;
            return _ls;
            // panelCC.Controls.Add(new LiteralControl(copaySB.ToString()));
            //Session["SelectedCoverageAndCopay"] = copaySB.ToString();
        }

        private static DataTable GetCopayAndCoverage(IStateContainer pageState, string ddi, int formularyStatus)
        {
            return Allscripts.Impact.Medication.GetCopayAndCoverage(
                 pageState.GetStringOrEmpty("COVERAGEID"),
                 pageState.GetStringOrEmpty("COPAYID"), ddi,
                 pageState.GetStringOrEmpty("FORMULARYID"), formularyStatus, 0,
                 pageState.GetStringOrEmpty("InfoSourcePayerID"),
                 pageState.GetStringOrEmpty("SelectedCoverageID"),
                 pageState.Cast<ConnectionStringPointer>("DBID", ConnectionStringPointer.ERXDB_SERVER_1)
                 );
        }
    }
    
}

