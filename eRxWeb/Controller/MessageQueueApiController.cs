using Allscripts.Impact;
using eRxWeb.AppCode;
using eRxWeb.ServerModel;
using eRxWeb.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.DatabaseSelector;
using Allscripts.ePrescribe.ExtensionMethods;
using Allscripts.ePrescribe.Objects;
using Allscripts.Impact.Interfaces;
using CancelRx = Allscripts.Impact.CancelRx;
using ScriptMessage = Allscripts.Impact.ScriptMsg.ScriptMessage;
using Allscripts.ePrescribe.Shared.Logging;
using System.Web.Http;
using eRxWeb.ServerModel.Request;

namespace eRxWeb.Controllers
{
    public partial class MessageQueueApiController : ApiController
    {
        private static ILoggerEx logger = LoggerEx.GetLogger();

        [HttpPost]
        public ApiResponse GetMessageQueInfo(MessageQueueRequest request)
        {
            using (var timer = logger.StartTimer("GetMessageQueInfo"))
            {
                IStateContainer pageState = new StateContainer(HttpContext.Current.Session);
                MessageQueInfoModel data = new MessageQueInfoModel();
                var response = new ApiResponse();
                try
                {
                    data = GetMessageQueInfo(pageState, request.scriptMessageID);
                    response.Payload = data;
                }
                catch (Exception ex)
                {
                    var errorMessage = ApiHelper.AuditException(ex.ToString(), pageState);
                    logger.Error("GetMessageQueInfo Exception: " + ex.ToString());

                    response.ErrorContext = new ErrorContextModel()
                    {
                        Error = ErrorTypeEnum.ServerError,
                        Message = errorMessage
                    };
                    timer.Message = string.Format("<ErrorContext>{0}</ErrorContext>", response.ErrorContext.ToLogString());
                }
                timer.Message = string.Format("<Request>{0}</Request><Response>{1}</Response>", request.scriptMessageID,response.ToLogString());
                return response;
            }
        }

        private static MessageQueInfoModel GetMessageQueInfo(IStateContainer session,string scriptMessageID)
        {
            MessageQueInfoModel data = new MessageQueInfoModel();
            if (!string.IsNullOrWhiteSpace(scriptMessageID))
            {
                var licenseId = ApiHelper.GetSessionLicenseID(session);
                var dbId = ApiHelper.GetDBID(session);
                var siteId = ApiHelper.GetSessionSiteID(session);
                var userId = ApiHelper.GetSessionUserID(session);

                ScriptMessage sm = new ScriptMessage(scriptMessageID, licenseId, userId, dbId);
                if (!isCSOrNewRx(sm))
                {
                    data.RefillsDisplayText = "Dispenses";
                }
                else
                {
                    data.RefillsDisplayText = "Refills";
                }

                var errorConfirmInfo = RxError.GetErrorConfirmInfo(sm.DBRxID, siteId, licenseId.ToGuidOr0x0(), dbId);
                if (!string.IsNullOrWhiteSpace(errorConfirmInfo?.Name) || !string.IsNullOrWhiteSpace(errorConfirmInfo?.Name))
                {
                    data.ConfirmedBy = errorConfirmInfo.Name;
                    data.ConfirmedOn = errorConfirmInfo.On.ToShortDateString();
                }

                if (sm.MessageType.Trim().Equals(Constants.MessageTypes.CANRX, StringComparison.OrdinalIgnoreCase))
                {
                    data.PharmacyNotes = new CancelRx().GetPharmNotesLabel(scriptMessageID, dbId, new ScriptMessage());
                }

                if ((sm.MessageType.Equals(Constants.MessageTypes.VERIFY, StringComparison.OrdinalIgnoreCase) ||
                    sm.MessageType.Equals(Constants.MessageTypes.CANRES, StringComparison.OrdinalIgnoreCase))
                    && sm.LinkedTransCtrlNo.Length > 0)
                {
                    sm = new ScriptMessage(sm.LinkedTransCtrlNo, licenseId, userId, dbId);
                }
                data.Patient = sm.PatientFirstName + " " + sm.PatientLastName;
                data.Address = sm.PatientAddress1;
                data.City = sm.PatientCity;
                data.State = sm.PatientState;
                data.ZIP = sm.PatientZip;
                data.Phone = Allscripts.Impact.Utilities.StringHelper.FormatPhone(sm.PatientPhoneNumber);
                data.Gender = sm.PatientGender;

                data.RxDate = sm.RxDateCreated;
                data.RxDrug = sm.DBDrugDescription;
                data.Sig = sm.RxSIGText;
                data.Quantity = sm.RxQuantity;

                bool isCSREFREQ = !string.IsNullOrWhiteSpace(sm.RxControlledSubstanceCode) && sm.MessageType.Equals(Constants.MessageTypes.REFILL_RESPONSE, StringComparison.OrdinalIgnoreCase);
                data.Refills = isCSREFREQ ? (int.Parse(sm.RxRefills) - 1).ToString() : sm.RxRefills;
                
                data.DAW = sm.RxDAW;
                data.Notes = sm.RxPharmacyNotes;
                data.PharmacyName = sm.PharmacyName;
                if (sm.PharmacistFirstName.Length > 0)
                {
                    data.Pharmacist = sm.PharmacistFirstName + " " + sm.PharmacistLastName;
                }
                data.DBPharmacyNetworkID = sm.DBPharmacyNetworkID;
                data.PharmacyAddress1 = sm.PharmacyAddress1;
                if (sm.PharmacyAddress2.Length > 0)
                {
                    data.PharmacyAddress2 = sm.PharmacyAddress2;
                }
                data.PharmacyCity = sm.PharmacyCity;
                data.PharmacyState = sm.PharmacyState;
                data.PharmacyZip = sm.PharmacyZip;
                data.PharmacyPhoneNumber = Allscripts.Impact.Utilities.StringHelper.FormatPhone(sm.PharmacyPhoneNumber);
                data.PharmacyFaxNumber = Allscripts.Impact.Utilities.StringHelper.FormatPhone(sm.PharmacyFaxNumber);
            }
            return data;
        }

        private static bool isCSOrNewRx(ScriptMessage sm)
        {
            bool isChgRxOrRefRes = sm.MessageType.Equals(Constants.MessageTypes.CHANGERX_RESPONSE, StringComparison.OrdinalIgnoreCase) ||
                                 sm.MessageType.Equals(Constants.MessageTypes.REFILL_RESPONSE, StringComparison.OrdinalIgnoreCase);

            bool isCS = !string.IsNullOrWhiteSpace(sm.RxControlledSubstanceCode);

            return isCS || !isChgRxOrRefRes;
        }
    }

}