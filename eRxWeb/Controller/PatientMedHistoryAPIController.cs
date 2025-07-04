using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.DatabaseSelector;
using Allscripts.ePrescribe.ExtensionMethods;
using Allscripts.ePrescribe.Shared.Logging;
using Allscripts.Impact;
using eRxWeb.AppCode;
using eRxWeb.AppCode.StateUtils;
using eRxWeb.ServerModel;
using eRxWeb.State;
using System;
using System.Data;
using System.Web;
using System.Web.Http;
using System.Collections.Generic;
using Allscripts.Impact.Interfaces;

namespace eRxWeb.Controller
{
    public class PatientMedHistoryAPIController : ApiController
    {
        private static readonly ILoggerEx logger = LoggerEx.GetLogger();
        private IPrescription prescription;
        private IStateContainer PageState;
        public PatientMedHistoryAPIController()
        {
            this.prescription = new Prescription();
            this.PageState = new StateContainer(HttpContext.Current.Session);
        }
        public PatientMedHistoryAPIController(IPrescription prescription, IStateContainer PageState)
        {
            this.prescription = prescription;
            this.PageState = PageState;
        }
        [HttpPost]
        public ApiResponse GetPatientMedicationHistoryData()
        {
            ApiResponse response = null;
            using (var timer = logger.StartTimer("GetPatientHeaderData"))
            {
                response = GetHistoryMedication(PageState, prescription);

                timer.Message = $"<Response>{response.ToLogString()}</Response>";
            }

            return response;
        }

        public static ApiResponse GetHistoryMedication(IStateContainer PageState, IPrescription prescription)
        {
            var response = new ApiResponse();

            try
            {
                var pHdr = GetPatientHistoryMedication(PageState, prescription);
                response.Payload = pHdr;
            }

            catch (Exception ex)
            {
                var errorMessage = ApiHelper.AuditException(ex.ToString(), PageState);
                logger.Error("GetHistoryMedication Exception: " + ex.ToString());
                response.ErrorContext = new ErrorContextModel()
                {
                    Error = ErrorTypeEnum.ServerError,
                    Message = errorMessage
                };
                return response;
            }
            logger.Debug("<Response>" + response.ToLogString() + "</Response>");
            return response;
        }

        public static dynamic GetPatientHistoryMedication(IStateContainer pageState, IPrescription prescription)
        {
            List <PatientMedHistoryModel> patMedHistorylst = new List<PatientMedHistoryModel>();
            DataSet historyMed = prescription.ChGetPatReviewHistory(pageState.GetStringOrEmpty("PatientID"), pageState.GetStringOrEmpty("LicenseID"), ApiHelper.GetDBID(pageState));

            foreach (DataRow dr in historyMed.Tables[0].Rows)
            {
                var patMedHistory = new PatientMedHistoryModel();

                patMedHistory.rxDate = dr["RxDate"].ToString();
                patMedHistory.diagnosis = dr["Dx"].ToString();
                patMedHistory.source = dr["RxSourceDescription"].ToString();
                patMedHistory.medDetail = dr["Prescription"].ToString();

                string rxID = dr["RxID"].ToString();
                int status = Convert.ToInt32(dr["status"].ToString());

                string pharmacy = dr["Pharmacy"].ToString();
                string comments = dr["comments"].ToString();
                string rxUpdatedDate = dr["Updated"].ToString().Trim();
                string rxSource = dr["RxSource"].ToString().Trim();
                string rxType = dr["Type"].ToString().Trim();
                string transMethod = dr["TransmissionMethod"].ToString().Trim();
                string controlledSubstanceCode = dr["ControlledSubstanceCode"].ToString().Trim();
                Constants.DrugHistoryType drugHistoryType = dr["DrugHistoryType"].ToEnum<Constants.DrugHistoryType>(Constants.DrugHistoryType.None);
                Constants.PrescriptionStatus rxStatus = (Constants.PrescriptionStatus)status;
                string transmissionStatus = dr["TransmissionStatus"].ToString();

                patMedHistory.status = RxUtils.RenderRxDescription(rxID,
                    pharmacy.ToUpper().Equals("Y"),
                    comments,
                    rxSource,
                    rxType,
                    rxStatus,
                    transMethod,
                    controlledSubstanceCode,
                    rxUpdatedDate,
                    transmissionStatus,
                    Convert.ToInt16(pageState["SiteID"]),
                    pageState.GetGuidOr0x0("LICENSEID"),
                    drugHistoryType,
                    ApiHelper.GetDBID(pageState)
                    );

                patMedHistorylst.Add(patMedHistory);

            }
            
            return patMedHistorylst;
        }
    }
}