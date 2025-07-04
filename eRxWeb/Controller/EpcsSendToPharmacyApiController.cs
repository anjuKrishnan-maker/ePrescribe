using eRxWeb.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Http;
using System.Web.UI;
using System.Web.UI.WebControls;
using Allscripts.Impact;
using System.Collections;
using eRxWeb.ServerModel;
using eRxWeb.AppCode;
using Allscripts.ePrescribe.Shared.Logging;

namespace eRxWeb.Controller
{
    public partial class GetEpcsSendToPharmacyApiController : ApiController
    {
        private static ILoggerEx logger = LoggerEx.GetLogger();

        [HttpPost]
        public  ApiResponse GetEpcsSendToPharmacy()
        {
            IStateContainer pageState = new StateContainer(HttpContext.Current.Session);
            var response = new ApiResponse();
            try
            {
                EpcsSendToPharmacyModel model = new EpcsSendToPharmacyModel();
                using (var timer = logger.StartTimer("GetEpcsSendToPharmacy"))
                {
                    model = getEpcsSendToPharm(pageState);
                    timer.Message = $"<Response>{model.ToLogString()}</Response>";
                }
                response.Payload = model;
            }
            catch (Exception ex)
            {
                var errorMessage = ApiHelper.AuditException(ex.ToString(), pageState);
                logger.Error("GetEpcsSendToPharmacy Exception: " + ex.ToString());

                response.ErrorContext = new ErrorContextModel()
                {
                    Error = ErrorTypeEnum.ServerError,
                    Message = errorMessage
                };
            }
            return response;

        }

        internal static EpcsSendToPharmacyModel getEpcsSendToPharm(IStateContainer session)
        {
            return new EpcsSendToPharmacyModel {IsGetEpcsSendToPharmacyVisible = CheckControlSubstance(session) && GetEpcsEligibleMedList(session).Count > 0};
        }

        private static bool CheckControlSubstance(IStateContainer pageState)
        {
            bool hasCSMeds = false;

            if (pageState.GetBoolean("ISCONTROLSUBSTANCE",false))
            {
                hasCSMeds = true;
            }

            return hasCSMeds;
        }
        private static List<string> GetEpcsEligibleMedList(IStateContainer pageState)
        {

            if (pageState["EPCSELIGIBLEMEDLIST"] == null)
            {
                pageState["EPCSELIGIBLEMEDLIST"] = new List<string>();
            }

            return (List<string>)pageState["EPCSELIGIBLEMEDLIST"];


        }

        public class EpcsSendToPharmacyModel
        {
            public bool IsGetEpcsSendToPharmacyVisible { get; set; }
        }
    }
}