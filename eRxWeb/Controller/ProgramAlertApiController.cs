using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.Shared.Logging;
using Allscripts.Impact;
using eRxWeb.AppCode;
using eRxWeb.ServerModel;
using eRxWeb.State;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Http;

namespace eRxWeb.Controllers
{
    public partial class ProgramAlertApiController : ApiController
    {
        private static ILoggerEx logger = LoggerEx.GetLogger();

        [HttpPost]
        public ApiResponse IsProviderEnrolledInSpecialtyMed()
        {
            using (var timer = logger.StartTimer("IsProviderEnrolledInSpecialtyMed"))
            {
                IStateContainer pageState = new StateContainer(HttpContext.Current.Session);
                var response = new ApiResponse();
                try
                {
                    bool isProviderEnrolledInSpecialtyMed = false;
                    if (pageState.ContainsKey(Constants.SessionVariables.IsProviderEnrolledInSpecialtyMed))
                    {
                        isProviderEnrolledInSpecialtyMed =
                            pageState.GetBooleanOrFalse(Constants.SessionVariables.IsProviderEnrolledInSpecialtyMed);
                    }
                    else
                    {
                        isProviderEnrolledInSpecialtyMed = SpecialtyMed.IsProviderEnrolledInSpecialtyMed(pageState);
                    }

                    response.Payload = isProviderEnrolledInSpecialtyMed;
                }
                catch (Exception ex)
                {
                    var errorMessage = ApiHelper.AuditException(ex.ToString(), pageState);
                    logger.Error("IsProviderEnrolledInSpecialtyMed Exception: " + ex.ToString());

                    response.ErrorContext = new ErrorContextModel()
                    {
                        Error = ErrorTypeEnum.ServerError,
                        Message = errorMessage
                    };
                    timer.Message = string.Format("<ErrorContext>{0}</ErrorContext>", response.ErrorContext.ToLogString());
                }
                timer.Message = string.Format("<Response>{0}</Response>", response.ToLogString());
                return response;
            }
        }
        [HttpPost]
        public ApiResponse IsScriptPadHasSpecmed()
        {
            using (var timer = logger.StartTimer("IsScriptPadHasSpecmed"))
            {
                IStateContainer pageState = new StateContainer(HttpContext.Current.Session);

                var response = new ApiResponse();
                try
                {
                    response.Payload = isScriptPadHasSpecMed(pageState);

                }
                catch (Exception ex)
                {
                    var errorMessage = ApiHelper.AuditException(ex.ToString(), pageState);
                    logger.Error("IsScriptPadHasSpecmed Exception: " + ex.ToString());

                    response.ErrorContext = new ErrorContextModel()
                    {
                        Error = ErrorTypeEnum.ServerError,
                        Message = errorMessage
                    };
                    timer.Message = string.Format("<ErrorContext>{0}</ErrorContext>", response.ErrorContext.ToLogString());
                }
                timer.Message = string.Format("<Response>{0}</Response>", response.ToLogString());
                return response;
            }
        }

        internal static bool isScriptPadHasSpecMed(IStateContainer session)
        {
            var sc = GetScriptPadMeds(session);
            var isSpecMed = (from v in sc
                where v.IsSpecialtyMed == true
                select v).FirstOrDefault();
            return isSpecMed != null && session.GetBooleanOrFalse(Constants.SessionVariables.IsProviderEnrolledInSpecialtyMed);
        }

        private static List<Rx> GetScriptPadMeds(IStateContainer pageState)
        {

            string userID = string.Empty;
            userID = ApiHelper.GetSessionUserID(pageState);
            var rxList = pageState.Cast<List<Rx>>("CURRENT_SCRIPT_PAD_MEDS", new List<Rx>());
            if (rxList == null || rxList.Count == 0)
            {
                DataTable dt = CHPatient.GetScriptPad(ApiHelper.GetSessionPatientId(pageState), ApiHelper.GetSessionLicenseID(pageState),
                    userID, ApiHelper.GetSessionPracticeState(pageState), ApiHelper.GetDBID(pageState));

                foreach (DataRow dr in dt.Rows)
                {
                    Rx rx = new Rx(dr);
                    rxList.Add(rx);
                }

            }

            return rxList;

        }
    }
}