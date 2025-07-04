using Allscripts.ePrescribe.Shared.Logging;
using eRxWeb.AppCode;
using eRxWeb.ServerModel;
using eRxWeb.State;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using Allscripts.Impact;
using static Allscripts.Impact.IgnoreReason;
using System.Collections;
using Allscripts.ePrescribe.Common;
using Allscripts.Impact.Interfaces;
using eRxWeb.AppCode.PptPlusBPL;

namespace eRxWeb.Controller
{
    public class FormularyOverrideApiController : ApiController
    {
        private IStateContainer session;
        IIgnoreReason reason;
        private static readonly ILoggerEx logger = LoggerEx.GetLogger();
        public FormularyOverrideApiController()
        {
            this.session = new StateContainer(HttpContext.Current.Session);
            this.reason = new IgnoreReason();
        }
        public FormularyOverrideApiController(IStateContainer state, IIgnoreReason igReason)
        {
            this.session = state;
            this.reason = igReason;
        }
        [HttpPost]
        public ApiResponse GetFormularyOverideIgnoreReasons()
        {
            using (var timer = logger.StartTimer("GetFormularyOverideIgnoreReasons"))
            {
                var response = new ApiResponse();
                try
                {
                    response.Payload = getFormularyOverideIgnoreReasons(session, reason);
                }

                catch (Exception ex)
                {
                    var errorMessage = ApiHelper.AuditException(ex.ToString(), session);
                    logger.Error("GetFormularyOverideIgnoreReasons Exception: " + ex.ToString());
                    response.ErrorContext = new ErrorContextModel()
                    {
                        Error = ErrorTypeEnum.ServerError,
                        Message = errorMessage
                    };
                    timer.Message = string.Format(errorMessage.ToLogString());
                    return response;
                }
                timer.Message = string.Format("<Response>{0}</Response>", response.ToLogString());
                return response;
            }
        }

        private static FormularyOverideModel getFormularyOverideIgnoreReasons(IStateContainer pageState, IIgnoreReason reason)
        {
            IgnoreReasonsModel irm = new IgnoreReasonsModel();
            FormularyOverideModel fom = new FormularyOverideModel();

            var dbID = ApiHelper.GetDBID(pageState);
            IgnoreCategory category = IgnoreCategory.FORMULARY_ALTS;
            var dt = reason.GetIgnoreReasons(category, dbID);

            irm.IgnoreReasons = dt.AsEnumerable().Select(r => r.Field<string>("ReasonDescription")).ToArray();

            fom.IgnoreReasons = irm;
            fom.Medication = getOverrideRxList(pageState);
            return fom;
        }

        private static MedicationModel[] getOverrideRxList(IStateContainer pageState)
        {
            ArrayList overrideList = pageState.Cast(Constants.SessionVariables.OverrideRxList, default(ArrayList));
            List<MedicationModel> mmObj = new List<MedicationModel>();

            if (overrideList != null)
            {
                foreach (Rx rx in overrideList)
                {
                    MedicationModel mm = new MedicationModel();
                    mm.RxID = rx.RxID;
                    mm.Medication = rx.MedicationName + " " + rx.Strength + " " + rx.StrengthUOM + " " + rx.DosageFormCode + " " + rx.RouteOfAdminCode;
                    mmObj.Add(mm);
                }
            }
            return mmObj.ToArray();
        }

        [HttpPost]
        public ApiResponse ClearOverrideRxListFromSession()
        {
            using (var timer = logger.StartTimer("ClearOverrideRxListFromSession"))
            {
                IStateContainer session = new StateContainer(HttpContext.Current.Session);
                var response = new ApiResponse();
                try
                {
                    session["OverrideRxList"] = null;
                    response.Payload = true;
                }
                catch (Exception ex)
                {
                    var errorMessage = ApiHelper.AuditException(ex.ToString(), session);
                    logger.Error("ClearOverrideRxListFromSession Exception: " + ex.ToString());
                    response.ErrorContext = new ErrorContextModel()
                    {
                        Error = ErrorTypeEnum.ServerError,
                        Message = errorMessage
                    };
                    timer.Message = string.Format(errorMessage.ToLogString());
                    return response;
                }
                timer.Message = string.Format("<Response>{0}</Response>", response.ToLogString());
                return response;
            }
        }

        [HttpPost]
        public ApiResponse FormularyOverrideProcessMedication([FromBody] SelectedOverrideReasonModel[] SelectedOverrideReason)
        {
            using (var timer = logger.StartTimer("FormularyOverrideProcessMedication"))
            {
                var response = new ApiResponse();
                try
                {
                    ArrayList overrideList = session.Cast(Constants.SessionVariables.OverrideRxList, default(ArrayList));

                    for (int i = 0; i < SelectedOverrideReason.Length; i++)
                    {
                        foreach (Rx rx in overrideList)
                        {
                            if (rx.RxID == SelectedOverrideReason[i].RxID)
                            {
                                rx.AlternativeIgnoreReason = SelectedOverrideReason[i].OverideReason - 1;
                            }
                        }
                    }

                    session["OverrideRxList"] = overrideList;
                }
                catch (Exception ex)
                {
                    var errorMessage = ApiHelper.AuditException(ex.ToString(), session);
                    logger.Error("FormularyOverrideProcessMedication Exception: " + ex.ToString());
                    response.ErrorContext = new ErrorContextModel()
                    {
                        Error = ErrorTypeEnum.ServerError,
                        Message = errorMessage
                    };
                    timer.Message = string.Format(errorMessage.ToLogString());
                    return response;
                }
                timer.Message = string.Format("<Response>{0}</Response> <Request>{1}</Request>", response.ToLogString(), SelectedOverrideReason.ToLogString());
                return response;
            }
        }
    }
}