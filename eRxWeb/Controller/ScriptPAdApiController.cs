using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.DatabaseSelector;
using Allscripts.ePrescribe.Shared.Logging;
using Allscripts.Impact;
using eRxWeb.AppCode;
using eRxWeb.AppCode.PptPlusBPL;
using eRxWeb.ServerModel;
using eRxWeb.State;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using CancelRx = eRxWeb.AppCode.CancelRx;
using System.Web.Http;
using eRxWeb.ServerModel.Request;
using Allscripts.ePrescribe.ExtensionMethods;
using System.Collections;

namespace eRxWeb.Controllers
{
    public partial class ScriptPAdApiController : ApiController
    {
        private static readonly ILoggerEx logger = LoggerEx.GetLogger();

        [HttpPost]
        public ApiResponse GetCurrentScriptPad([FromBody]string page)
        {
            using (var timer = logger.StartTimer("GetCurrentScriptPad"))
            {
                IStateContainer pageState = new StateContainer(HttpContext.Current.Session);


                var response = new ApiResponse();
                try
                {

                    response.Payload = GetCurrentScriptPadData(page, pageState);
                    

                }
                catch (Exception ex)
                {
                    var errorMessage = ApiHelper.AuditException(ex.ToString(), pageState);
                    logger.Error("GetCurrentScriptPad Exception: " + ex.ToString());

                    response.ErrorContext = new ErrorContextModel()
                    {
                        Error = ErrorTypeEnum.UserMessage,
                        Message = errorMessage
                    };
                    timer.Message = string.Format("<ErrorContext>{0}</ErrorContext>", response.ErrorContext.ToLogString());
                }
                timer.Message = string.Format("<Response>{0}</Response>", response.ToLogString());
                return response;
            }
        }
        internal static List<ScriptPadModel> GetCurrentScriptPadData(string page, IStateContainer pageState)
        {
            var scripts = new List<ScriptPadModel>();
            try
            {
                string userID;

                if (pageState.ContainsKey("DelegateProviderID") && !pageState.GetBooleanOrFalse("IsPASupervised"))
                    userID = pageState.GetStringOrEmpty("DelegateProviderID");
                else
                    userID = pageState.GetStringOrEmpty("UserID");

                var ScriptPadMeds = ScriptPadUtil.GetScriptPadMeds(pageState);
                if (ScriptPadMeds != null && ScriptPadMeds.Count > 0 &&
                        !(pageState.GetStringOrEmpty(Constants.SessionVariables.TaskType.ToUpper())==Constants.MessageTypes.REFILL_REQUEST ||
                        pageState.GetStringOrEmpty(Constants.SessionVariables.TaskType.ToUpper())==Constants.MessageTypes.CHANGERX_REQUEST))
                {
                    for (int i = 0; i < ScriptPadMeds.Count; i++)
                    {
                        ScriptPadModel scriptModel = new ScriptPadModel();
                        Rx scriptPadMed = (ScriptPadMeds[i] as Rx);
                        scriptModel.IsBrandNameMed = scriptPadMed.IsBrandNameMed;
                        string scriptDescription = scriptPadMed.FullDrugDescription;
                        if (pageState[Constants.SessionVariables.TaskScriptMessageId] != null)
                        {
                            scriptDescription = scriptPadMed.FullDrugDescription.Replace(" - REFILL ", " - DISPENSE ");
                        }
                        scriptModel.Description = scriptDescription;
                        scriptModel.rxId = scriptPadMed.RxID;
                        var navigateToPage = GetNavigationPage(page);
                        //ApiHelper.GetUserType(pageState) == Constants.UserCategory.POB_LIMITED || ApiHelper.GetUserType(pageState) == Constants.UserCategory.POB_REGULAR || ApiHelper.GetUserType(pageState) == Constants.UserCategory.POB_SUPER ?
                        // Constants.PageNames.NURSE_FULL_SCRIPT : Constants.PageNames.SELECT_MEDICATION;

                        if (pageState.GetBooleanOrFalse("ISPROVIDER") || pageState.GetBooleanOrFalse("IsPA"))
                        {
                            scriptModel.RxEditUrl =
                                $"{Constants.PageNames.SIG}?Mode=Edit&RxId={scriptModel.rxId}&To={navigateToPage}";
                        }
                        else if (pageState.GetBooleanOrFalse("IsDelegateProvider") || pageState.GetBooleanOrFalse("IsPASupervised"))
                        {
                            scriptModel.RxEditUrl =
                                $"{Constants.PageNames.NURSE_SIG}?Mode=Edit&RxId={scriptModel.rxId}&To={navigateToPage}";
                        }
                        scripts.Add(scriptModel);

                    }
                }
            }
            catch (Exception ex)
            {
                var msg = ApiHelper.AuditException(ex.ToString(), pageState);
                logger.Error($"GetCurrentScriptPadData: {msg} Exception: {ex}");
            }
            return scripts;
        }
        private static string GetNavigationPage(string page)
        {

            if (!string.IsNullOrEmpty(page))
            {
                if (page.Equals(Constants.PageNames.SELECT_MEDICATION, StringComparison.OrdinalIgnoreCase) ||
                     page.Equals(Constants.PageNames.FREE_FORM_DRUG, StringComparison.OrdinalIgnoreCase) ||
                     page.Equals(Constants.PageNames.NURSE_FREE_FORM_DRUG, StringComparison.OrdinalIgnoreCase))

                    page = Constants.PageNames.REDIRECT_TO_ANGULAR + "?componentName=" + Constants.PageNames.SELECT_MEDICATION;
            }
                return page;
           
        }
        [HttpPost]
        public ApiResponse RemoveScript([FromBody]string rxID)
        {
            using (var timer = logger.StartTimer("RemoveScript"))
            {
                IStateContainer pageState = new StateContainer(HttpContext.Current.Session);
                var response = new ApiResponse();
                try
                {
                    if (!string.IsNullOrWhiteSpace(rxID))
                    {
                        ScriptPadUtil.RemoveScript(pageState, rxID);
                    }
                    else
                    {
                        logger.Debug("Rxid is missing");
                        response.ErrorContext = new ErrorContextModel()
                        {
                            Error = ErrorTypeEnum.UserMessage,
                            Message = "Rxid is missing"
                        };
                    }
                }
                catch (Exception ex)
                {
                    var errorMessage = ApiHelper.AuditException(ex.ToString(), pageState);
                    logger.Error("RemoveScript Exception: " + ex.ToString());

                    response.ErrorContext = new ErrorContextModel()
                    {
                        Error = ErrorTypeEnum.ServerError,
                        Message = errorMessage
                    };
                    timer.Message = string.Format("<ErrorContext>{0}</ErrorContext>", response.ErrorContext.ToLogString());
                }
                timer.Message = string.Format("<Request><Rxid>{0}</RxId></Request><Response>{1}</Response>", rxID, response.ToLogString());
                return response;
            }
        }

        #region Med history completion


        [HttpPost]
        public ApiResponse ReviewScript([FromBody]string pageName)
        {
            using (var timer = logger.StartTimer("ReviewScript"))
            {
                IStateContainer pageState = new StateContainer(HttpContext.Current.Session);
                var response = new ApiResponse();
                pageState["CHECK_DUR"] = true;
                try
                {
                    var model = new ScriptPadModel();

                    pageState[Constants.SessionVariables.ReviewScriptPadRedirection] = true;
                    var ddiList = string.Join(",", from rx in ScriptPadUtil.GetScriptPadMeds(pageState) select rx.DDI);

                    DataSet dsActiveScripts = Prescription.CheckPatientRxHistoryForMultiSelect(ddiList, ApiHelper.GetSessionPatientId(pageState), ApiHelper.GetDBID(pageState));

                    if (dsActiveScripts.Tables[0].Rows.Count > 0)
                    {
                        model.showMediHistoryCompletionPopUp = true;
                        for (var i = 0; i < dsActiveScripts.Tables[0].Rows.Count; i++)
                        {
                            model.MedCompletionHistory.Add(new MedCompletionHistory()
                            {
                                Medication = dsActiveScripts.Tables[0].Rows[i]["Medication"].ToString(),
                                RxID = dsActiveScripts.Tables[0].Rows[i]["RxID"].ToString()
                            });
                        }
                    }

                    response.Payload = model;
                }
                catch (Exception ex)
                {
                    var errorMessage = ApiHelper.AuditException(ex.ToString(), pageState);
                    logger.Error("ReviewScript Exception: " + ex.ToString());

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
        public ApiResponse CompleteScriptpadMedHistory(SelectMedicationDataRequest request)
        {
            using (var timer = logger.StartTimer("CompleteScriptpadMedHistory"))
            {
                IStateContainer pageState = new StateContainer(HttpContext.Current.Session);
                var response = new ApiResponse();
                try
                {
                    ScriptPadModel model = new ScriptPadModel();
                    if (request.medCompleteHistories.Count() > 0)
                    {
                        ArrayList tempList = new ArrayList();
                        ArrayList rxList = pageState.Cast("RxList", new ArrayList());
                        tempList.AddRange(rxList);
                        tempList.AddRange(ScriptPadUtil.GetScriptPadMeds(pageState));
                        var ddiList = string.Join(",", from rx in tempList.Cast<Rx>().ToList() select rx.DDI);
                        DataSet dsActiveScripts = Prescription.CheckPatientRxHistoryForMultiSelect(ddiList, ApiHelper.GetSessionPatientId(pageState), ApiHelper.GetDBID(pageState));
                        List<CancelRxEligibleScript> CancelRxEligibleList = new List<CancelRxEligibleScript>();

                        foreach (DataRow script in dsActiveScripts.Tables[0].Rows)
                        {
                            string rxId = script["RxID"].ToString().Trim();
                            string rxStartDate = script["Created"].ToString().Trim();
                            string transMethod = script["TransmissionMethod"].ToString().Trim();
                            var originalNewRxTrnsCtrlNo = Convert.ToString(script["OriginalNewRxTrnsCtrlNo"]).ToGuidOr0x0();
                            string controlledSubstanceCode = script["ControlledSubstanceCode"].ToString().Trim();
                            string isPharmacyCancelRxEligible = script["IsPharmacyCancelRxEligible"].ToString().Trim();

                            // check if any scripts being completed can be canceled
                            if (AppCode.CancelRx.IsScriptCancelRxEligile(transMethod, rxStartDate, controlledSubstanceCode, isPharmacyCancelRxEligible, originalNewRxTrnsCtrlNo))
                            {
                                // need to add RxID to match completed scripts
                                CancelRxEligibleList.Add(new CancelRxEligibleScript(rxId.ToGuidOr0x0(), originalNewRxTrnsCtrlNo, script["Medication"].ToString()));
                            }
                        }

                        var completedHx = (from v in request.medCompleteHistories
                                           select v.RxID).ToList();
                        var scriptsToCancel = AppCode.CancelRx.GetScriptsToCancel(completedHx, CancelRxEligibleList);
                        foreach (var li in request.medCompleteHistories)
                        {
                            Prescription.Complete(li.RxID, ApiHelper.GetSessionUserID(pageState), ApiHelper.GetSessionLicenseID(pageState), pageState.GetStringOrEmpty("ExtFacilityCd"),
                                pageState.GetStringOrEmpty("ExtGroupID"), ApiHelper.GetDBID(pageState));
                        }
                        if (scriptsToCancel.Count > 0)
                        {
                            logger.Debug("Cancel rx Window");
                            model.ShowucCancelRx = true;
                            model.CancelRxScripts = scriptsToCancel;
                        }
                        else
                        {
                            model.CancelRxScripts = new List<CancelRxEligibleScript>();
                        }
                        RefreshPatientActiveMeds(pageState);
                    }
                    response.Payload = model;
                }
                catch (Exception ex)
                {
                    var errorMessage = ApiHelper.AuditException(ex.ToString(), pageState);
                    logger.Error("CompleteScriptpadMedHistory Exception: " + ex.ToString());

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

        private static void RefreshPatientActiveMeds(IStateContainer pageState)
        {
            if (!string.IsNullOrEmpty(ApiHelper.GetSessionPatientId(pageState)))
            {
                //Retrieve the patient's distinct active medications 
                DataSet activeMeds = Patient.GetPatientActiveMedications(ApiHelper.GetSessionPatientId(pageState), ApiHelper.GetSessionLicenseID(pageState), ApiHelper.GetSessionUserID(pageState), ApiHelper.GetDBID(pageState));

                if (activeMeds.Tables["Medications"].Rows.Count > 0)
                {
                    StringBuilder activeMedications = new StringBuilder();
                    List<string> activeMedDDIList = new List<string>();

                    foreach (DataRow dr in activeMeds.Tables["Medications"].Rows)
                    {
                        if (activeMedications.Length > 0)
                        {
                            activeMedications.Append(", ");
                        }

                        activeMedications.Append(dr["MedicationName"].ToString().Trim());
                        activeMedDDIList.Add(dr["DDI"].ToString());
                    }

                    pageState["ACTIVEMEDICATIONS"] = activeMedications.ToString();
                    pageState["ACTIVEMEDDDILIST"] = activeMedDDIList;
                }
                else
                {
                    pageState.Remove("ACTIVEMEDICATIONS");
                    pageState.Remove("ACTIVEMEDDDILIST");
                }
            }
        }
        #endregion

        private static bool SatisfyVisibliltyCondition(IStateContainer pageState)
        {
            var visible = true;
            if (string.IsNullOrWhiteSpace(pageState.GetStringOrEmpty("FORMULARYID")))
                visible = false;
            return visible;
        }
        private static List<Rx> GetCurrentScriptPadMeds(IStateContainer pageState)
        {

            List<Rx> rxList = new List<Rx>();
            string userID = string.Empty;
            using (var timer = logger.StartTimer("GetCurrentScriptPadMeds"))
            {
                userID = pageState.GetStringOrEmpty("USERID");

                if (pageState["CURRENT_SCRIPT_PAD_MEDS"] == null)
                {
                    DataTable dt = CHPatient.GetScriptPad(pageState.GetStringOrEmpty("PATIENTID"), pageState.GetStringOrEmpty("LICENSEID"), userID, pageState.GetStringOrEmpty("PRACTICESTATE"), pageState.Cast<ConnectionStringPointer>("DBID", ConnectionStringPointer.ERXDB_SERVER_1));

                    foreach (DataRow dr in dt.Rows)
                    {
                        Rx rx = new Rx(dr);
                        rxList.Add(rx);
                    }

                    pageState["CURRENT_SCRIPT_PAD_MEDS"] = rxList;
                }
                else
                {
                    rxList = pageState.Cast<List<Rx>>("CURRENT_SCRIPT_PAD_MEDS", rxList);
                }
                timer.Message = $"<userID>{userID}</userID><rxList>{rxList}</rxList>";
            }
            return rxList;

        }
        
    }

}