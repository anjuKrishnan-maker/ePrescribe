using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.DatabaseSelector;
using Allscripts.ePrescribe.Shared.Logging;
using Allscripts.Impact;
using Allscripts.Impact.Interfaces;
using eRxWeb.AppCode;
using eRxWeb.AppCode.PptPlusBPL;
using eRxWeb.ServerModel;
using eRxWeb.ServerModel.Request;
using eRxWeb.State;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Http;
using Allscripts.ePrescribe.Data;
using IPatient = Allscripts.Impact.Interfaces.IPatient;
using Medication = Allscripts.Impact.Medication;
using Patient = Allscripts.Impact.Patient;
using PatientCoverage = eRxWeb.AppCode.PatientCoverage;
using Prescription = Allscripts.Impact.Prescription;
using Allscripts.ePrescribe.Data.Model;
using Allscripts.ePrescribe.Objects;
using Allscripts.Impact.Tasks;
using Allscripts.Impact.Utilities;
using Rx = Allscripts.Impact.Rx;
using RxUser = Allscripts.Impact.RxUser;
using ScriptMessage = Allscripts.Impact.ScriptMsg.ScriptMessage;
using TaskUtils = Allscripts.Impact.Tasks.TaskUtils;


namespace eRxWeb.Controller
{

    public class SelectMedicationApiController : ApiController
    {
        private static readonly ILoggerEx logger = LoggerEx.GetLogger();
        IPrescription iPrescription = new Prescription();
        IStateContainer pageState;

        public IStateContainer PageState
        {
            get
            {
                if (pageState == null)
                    pageState = new StateContainer(HttpContext.Current.Session);
                return pageState;
            }
            set
            {
                pageState = value;
            }
        }

        private ApplicationLicense SessionLicense
        {
            get
            {
                if (PageState["SessionLicense"] == null)
                {
                    if (PageState["DBID"] == null)
                    {
                        PageState["SessionLicense"] = new ApplicationLicense(string.Empty, 0, string.Empty, ConnectionStringPointer.ERXDB_DEFAULT);
                    }
                    else
                    {
                        PageState["SessionLicense"] = new ApplicationLicense(string.Empty, 0, string.Empty, (ConnectionStringPointer)PageState["DBID"]);
                    }
                }

                return (ApplicationLicense)PageState["SessionLicense"];
            }
        }

        public List<Rx> ScriptPadMeds
        {
            get
            {
                List<Rx> rxList = new List<Rx>();
             
                if (PageState[Constants.SessionVariables.CurrentScriptPadMeds] == null)
                {
                    string userID = string.Empty;
                    userID = PageState.GetGuidOr0x0(Constants.SessionVariables.UserId).ToString();

                    DataTable scripts = CHPatient.GetScriptPad(PageState.GetGuidOr0x0(Constants.SessionVariables.PatientId).ToString(), 
                        PageState.GetGuidOr0x0(Constants.SessionVariables.LicenseId).ToString(), userID, 
                        PageState[Constants.SessionVariables.PracticeState].ToString(), ApiHelper.GetDBID(PageState));

                    foreach (DataRow script in scripts.Rows)
                    {
                        Rx rx = new Rx(script);
                        rxList.Add(rx);
                    }

                    PageState[Constants.SessionVariables.CurrentScriptPadMeds] = rxList;
                }
                else
                {
                    rxList = PageState[Constants.SessionVariables.CurrentScriptPadMeds] as List<Rx>;
                }

                return rxList;
            }
            set
            {
                PageState["CURRENT_SCRIPT_PAD_MEDS"] = value;
            }
        }

        public RxUser SupervisingProvider => AppCode.StateUtils.UserInfo.GetSupervisingProvider(PageState);
        public RxUser DelegateProvider => AppCode.StateUtils.UserInfo.GetDelegateProvider(PageState);

        [HttpPost]
        public ApiResponse InsertLibraryAudit()
        {
            using (var timer = logger.StartTimer("InsertLibraryAudit"))
            {
                var response = new ApiResponse();
                try
                {
                    var licenseID = ApiHelper.GetSessionLicenseID(PageState);
                    var dbId = ApiHelper.GetDBID(PageState);
                    var userId = ApiHelper.GetSessionUserID(pageState);

                    var deluxeModule = new Allscripts.Impact.Module(Allscripts.Impact.Module.ModuleType.DELUXE, licenseID, userId, dbId);

                    var context = ApiHelper.LexicompView(PageState) == EnabledDisabled.Enabled ? "Lexicomp - CONTEXUAL" : "IFC - CONTEXUAL";
                    deluxeModule.InsertModuleAudit(context, dbId);

                    timer.Message = $"{response.Payload}";
                }
                catch (Exception ex)
                {
                    logger.Error(ex.ToString());
                    response.ErrorContext = new ErrorContextModel() { Error = ErrorTypeEnum.ServerError, Message = ex.ToLogString() };
                }
                return response;
            }
        }


        [HttpPost]
        public ApiResponse HasScriptPadMed()
        {
            using (var timer = logger.StartTimer("HasScriptPadMed"))
            {
                var response = new ApiResponse();
                try
                {
                    if (ScriptPadMeds.Count > 0)
                        response.Payload = true;
                    else
                        response.Payload = false;

                    timer.Message = $"HasScriptPadMed = {response.Payload}";
                }
                catch (Exception ex)
                {
                    logger.Error(ex.ToString());
                    response.ErrorContext = new ErrorContextModel() { Error = ErrorTypeEnum.ServerError, Message = ex.ToLogString() };
                }
                return response;
            }
        }

        public PatientCoverageHeaderList GetPatientCoverages()
        {
            PatientCoverageHeaderList response;
            var patientId = PageState.GetGuidOr0x0(Constants.SessionVariables.PatientId);
            var dbId = ApiHelper.GetDBID(PageState);
            var selectedCoverageId = PageState.GetStringOrEmpty(Constants.SessionVariables.SelectedCoverageId);
            var prevCoverageId = selectedCoverageId;

            using (var timer = logger.StartTimer("LoadPatientCoverageSelectionList"))
            {
                var covList = Allscripts.Impact.PatientCoverage.LoadPatientCoverageSelectionList(patientId, selectedCoverageId, new Allscripts.ePrescribe.Data.PatientCoverage(), dbId);
                timer.Message = covList.ToLogString();
                response = covList;

                var selectedCoverage = covList?.PatientCoverageHeaders?.FirstOrDefault(_ => _.ID == selectedCoverageId);
                if (selectedCoverage == null)
                {
                    if(covList?.PatientCoverageHeaders != null
                    && covList?.PatientCoverageHeaders.Count > 0)
                    {
                        selectedCoverageId = covList?.PatientCoverageHeaders[0].ID;
                    }
                    else
                    {
                        selectedCoverageId = null;
                    }
                }
               
                if (prevCoverageId != selectedCoverageId)
                {
                    UpdatePatientSelectedCoverageRequest updatePatientSelectedCoverageRequest = new UpdatePatientSelectedCoverageRequest();
                    updatePatientSelectedCoverageRequest.CoverageId = selectedCoverageId;
                    PatientCoverage.UpdatePatientCoverage(updatePatientSelectedCoverageRequest, PageState, new Allscripts.Impact.PatientCoverage(), new PptPlus());
                }
            }
            logger.Debug("<Response>" + response.ToLogString() + "</Response>");
            return response;
        }

        [HttpPost]
        public ApiResponse UpdatePatientSelectedCoverage(UpdatePatientSelectedCoverageRequest request)
        {
            var response = new ApiResponse();
            try
            {
                using (var timer = logger.StartTimer("UpdatePatientSelectedCoverage"))
                {
                    timer.Message = request.ToLogString();
                    PatientCoverage.UpdatePatientCoverage(request, PageState, new Allscripts.Impact.PatientCoverage(), new PptPlus());
                }
            }
            catch (Exception ex)
            {
                var errorMessage = ApiHelper.AuditException(ex.ToString(), PageState);
                logger.Error("UpdatePatientSelectedCoverage Exception: " + ex.ToString());
                response.ErrorContext = new ErrorContextModel()
                {
                    Error = ErrorTypeEnum.ServerError,
                    Message = errorMessage
                };
            }
            logger.Debug("<Response>" + response.ToLogString() + "</Response>");
            return response;
        }

        [HttpPost]
        public ApiResponse GetSelectMedicationData(SelectMedicationDataRequest request)
        {
            var response = new ApiResponse();
            try
            {
                using (var timer = logger.StartTimer("UpdatePatientSelectedCoverage"))
                {
                    #region Setup Variable

                    var payLoad = new SelectMedicationGridModel();
                    PatientRecordStatus prs = request.PatientRecordStatus;

                    var messageMod = new MessageModel();
                    List<FullScriptHxMedDataModel> selectMedicationHxMedDataModelList = new List<FullScriptHxMedDataModel>();
                    List<FullScriptMedDataModel> selectMedicationMedDataModelList = new List<FullScriptMedDataModel>();


                    payLoad.RequestFor = request.RequestFor;
                    request.sessionState = PageState;
                    List<MessageModel> messages = new List<MessageModel>();
                  
                    #endregion

                    if (request.RequestFor == SelectMedicationRequestType.PatientHistory)
                    {
                        selectMedicationHxMedDataModelList = GetPatientHistory(request, ref messages, out prs);

                        if (selectMedicationHxMedDataModelList.Count == 0)
                        {
                            request.RequestFor = SelectMedicationRequestType.ProviderHistory;
                        }
                    }

                    if (request.RequestFor == SelectMedicationRequestType.ProviderHistory)
                    {
                        selectMedicationHxMedDataModelList = GetProviderHistory(request, ref messages);

                        if (selectMedicationHxMedDataModelList.Count == 0)
                        {
                            request.RequestFor = SelectMedicationRequestType.AllMedication;
                        }
                    }

                    if (request.RequestFor == SelectMedicationRequestType.PreBuiltGroup)
                    {
                        selectMedicationHxMedDataModelList = GetPrebuiltPrescriptionMeds(request, ref messages);

                        if (selectMedicationHxMedDataModelList.Count == 0 && !string.IsNullOrWhiteSpace(request.SearchText))
                        {
                            request.RequestFor = SelectMedicationRequestType.AllMedication;
                        }
                    }

                    if (request.RequestFor == SelectMedicationRequestType.AllMedication)
                    {
                        selectMedicationMedDataModelList = getAllMeds(request, ref messages);
                    }


                    payLoad.PatientRecordStatus = prs;
                    payLoad.RequestFor = request.RequestFor;
                    var meds = new List<SelectMedicationMedModel>();
                    Hashtable quickDURs = new Hashtable();
                    if (request.RequestFor == SelectMedicationRequestType.AllMedication)
                    {
                        foreach (var selectMedicationMedDataModel in selectMedicationMedDataModelList)
                        {
                            meds.Add(SelectMedicationMedModel.CreateFromDataAllMedRow(selectMedicationMedDataModel, ref quickDURs,
                                PageState));
                        }
                    }
                    else
                    {
                        foreach (var selectMedicationHxMedDataModel in selectMedicationHxMedDataModelList)
                        {
                            meds.Add(SelectMedicationMedModel.CreateFromDataRow(selectMedicationHxMedDataModel, ref quickDURs,
                                PageState));
                        }
                    }

                    payLoad.Meds.AddRange(meds);

                    timer.Message = $"<Request>{request.ToLogString()}</Request><Response>{payLoad.ToLogString()}</Response>";
                    payLoad.Messages = messages;
                    response.Payload = payLoad;
                }
            }
            catch (Exception ex)
            {
                var errorMessage = ApiHelper.AuditException(ex.ToString(), PageState);
                logger.Error("GetSelectMedicationData Exception: " + ex.ToString());
                response.ErrorContext = new ErrorContextModel()
                {
                    Error = ErrorTypeEnum.ServerError,
                    Message = errorMessage
                };
            }
            logger.Debug("<Response>" + response.ToLogString() + "</Response>");
            PPTPlus.ClearScriptPadCandidates(PageState);
            return response;
        }

        private static List<FullScriptHxMedDataModel> GetPrebuiltPrescriptionMeds(SelectMedicationDataRequest request, ref List<MessageModel> messages)
        {
            List<FullScriptHxMedDataModel> selectMedicationHxMedDataModelList = new List<FullScriptHxMedDataModel>();

            selectMedicationHxMedDataModelList = new Allscripts.Impact.PreBuildPrescription.PreBuildPrescription().GetFullScriptPrebuiltPrescriptionMeds(
                 request.GroupName,
                 request.LicenseId, request.Siteid, request.ProviderId, request.ICD9Code, request.FormularyID, request.FormularyActive,
                 request.OtcCoverage, request.GenericDrugPolicy, request.UnlistedDrugPolicy, request.PatientID, request.SelectedCoverageID,
                 request.GenericEquivalent, request.SearchText, request.DbID);

            if (selectMedicationHxMedDataModelList.Count == 0 && !string.IsNullOrWhiteSpace(request.SearchText))
            {
                messages.Add(new MessageModel() { Message = $"No medications match '{request.GroupName}' Group.", Icon = MessageIcon.Information });
            }
            else
            {
                var sigIdList = selectMedicationHxMedDataModelList.Select(s => s.SigId).Distinct();

                // get sig text from sdr.SIG and dbo.Free_From_Sig tables in mother db and update object item in selectMedicationHxMedDataModelList
                var allSigTexts = Allscripts.Impact.Sig.GetFreeFormSigText(sigIdList.ToArray<string>(), ConnectionStringPointer.SHARED_DB);
                allSigTexts.PrimaryKey = new DataColumn[] { allSigTexts.Columns["SIGID"] };

                foreach (FullScriptHxMedDataModel rx in selectMedicationHxMedDataModelList)
                {
                    rx.SigText = allSigTexts.Rows.Find(rx.SigId)["SIGText"].ToString();
                    rx.Medication = rx.Medication + ", " + rx.SigText;
                    rx.MedicationBack = rx.MedicationBack + ", " + rx.SigText;
                }
            }

            return selectMedicationHxMedDataModelList;
        }

        private List<FullScriptMedDataModel> getAllMeds(SelectMedicationDataRequest request, ref List<MessageModel> messages)
        {
            List<FullScriptMedDataModel> selectMedicationMedDataModelList = new List<FullScriptMedDataModel>();

            selectMedicationMedDataModelList = Allscripts.Impact.Medication.GetFullScriptAllMeds(
                request.SearchText,
                 request.FormularyID,
                 request.FormularyActive,
                 request.OtcCoverage,
                 request.GenericDrugPolicy,
                 request.UnlistedDrugPolicy,
                 request.SelectedCoverageID,
                 request.GenericEquivalent,
                 request.LicenseId,
                 request.Siteid.ToString(),
                 request.DbID);

            string originalSearch = request.SearchText;
            string medName = request.SearchText;

            //if we didn't get any results for the first search, try again with less search criteria
            if (selectMedicationMedDataModelList.Count == 0 && medName.Trim().IndexOf(" ") >= 0)
            {
                medName = medName.Substring(0, medName.LastIndexOf(" ")).Trim();
                selectMedicationMedDataModelList = Allscripts.Impact.Medication.GetFullScriptAllMeds(
                 medName,
                 request.FormularyID,
                 request.FormularyActive,
                 request.OtcCoverage,
                 request.GenericDrugPolicy,
                 request.UnlistedDrugPolicy,
                 request.SelectedCoverageID,
                 request.GenericEquivalent,
                 request.LicenseId,
                 request.Siteid.ToString(),
                 request.DbID);

                if (selectMedicationMedDataModelList.Count == 0)
                {
                    messages.Add(new MessageModel() { Message = $"No results were found for your search ({ originalSearch}).", Icon = MessageIcon.Information });
                }
                else
                {
                    messages.Add(new MessageModel()
                    {
                        Message = $"No results were found for your search ({originalSearch}). {selectMedicationMedDataModelList.Count} results were found for ({medName}).",
                        Icon = MessageIcon.Information
                    });
                }
            }

            if (selectMedicationMedDataModelList.Count > 50)
            {
                messages.Add(new MessageModel()
                {
                    Message = $"Your search returned more than 50 results. Please consider refining your search.(e.g. {medName} 250)",
                    Icon = MessageIcon.Information
                });
            }

            return selectMedicationMedDataModelList;
        }

        private List<FullScriptHxMedDataModel> GetPatientHistory(SelectMedicationDataRequest request, ref List<MessageModel> messages, out PatientRecordStatus patRecordStatus)
        {
            patRecordStatus = request.PatientRecordStatus;
            List<FullScriptHxMedDataModel> selectMedicationHxMedDataModelList = new List<FullScriptHxMedDataModel>();
            selectMedicationHxMedDataModelList = Allscripts.Impact.Medication.GetFullScriptHistoryMeds(request.LicenseId, request.Siteid, request.ProviderId, request.ICD10Code, request.SearchText,
                            request.FormularyID, request.FormularyActive, request.OtcCoverage, request.GenericDrugPolicy, request.UnlistedDrugPolicy, request.PatientID, request.SelectedCoverageID,
                            request.GenericEquivalent, patRecordStatus.ToString(), request.DbID);

            if (selectMedicationHxMedDataModelList.Count == 0 && request.PatientRecordStatus != PatientRecordStatus.Both)
            {
                patRecordStatus = PatientRecordStatus.Both;
                selectMedicationHxMedDataModelList = Allscripts.Impact.Medication.GetFullScriptHistoryMeds(request.LicenseId, request.Siteid, request.ProviderId, request.ICD10Code, request.SearchText,
                            request.FormularyID, request.FormularyActive, request.OtcCoverage, request.GenericDrugPolicy, request.UnlistedDrugPolicy, request.PatientID, request.SelectedCoverageID,
                            request.GenericEquivalent, patRecordStatus.ToString(), request.DbID);
            }

            if (selectMedicationHxMedDataModelList.Count == 0 && !string.IsNullOrWhiteSpace(request.SearchText))
            {
                messages.Add(new MessageModel() { Message = "No medications match this patient's history.", Icon = MessageIcon.Information });
            }
            else
            {
                var sigIdList = selectMedicationHxMedDataModelList.Select(s => s.SigId).Distinct();

                // get sig text from sdr.SIG and dbo.Free_From_Sig tables in mother db and update object item in selectMedicationHxMedDataModelList
                var allSigTexts = Allscripts.Impact.Sig.GetFreeFormSigText(sigIdList.ToArray<string>(), ConnectionStringPointer.SHARED_DB);
                allSigTexts.PrimaryKey = new DataColumn[] { allSigTexts.Columns["SIGID"] };

                foreach (FullScriptHxMedDataModel rx in selectMedicationHxMedDataModelList)
                {

                    DataRow allSigTextsRow = allSigTexts.Rows.Find(rx.SigId);
                    if (allSigTextsRow == null || (allSigTextsRow["SIGText"] == null))
                        continue;

                    string sigTxt = Convert.ToString(allSigTextsRow["SIGText"]);

                    rx.SigText = sigTxt;
                    rx.Medication = rx.Medication + ", " + rx.SigText;
                    rx.MedicationBack = rx.MedicationBack + ", " + rx.SigText;
                }
            }

            if (selectMedicationHxMedDataModelList.Count > 50)
            {
                string message = string.IsNullOrWhiteSpace(request.SearchText) ? "This patient's history contains" : "Your search returned";
                messages.Add(new MessageModel()
                {
                    Message = $"{message} more than 50 results. Consider searching with more specific terms.",
                    Icon = MessageIcon.Information
                });

            }
            return selectMedicationHxMedDataModelList;
        }

        private List<FullScriptHxMedDataModel> GetProviderHistory(SelectMedicationDataRequest request, ref List<MessageModel> messages)
        {
            List<FullScriptHxMedDataModel> selectMedicationHxMedDataModelList = new List<FullScriptHxMedDataModel>();
            selectMedicationHxMedDataModelList = Allscripts.Impact.Medication.GetFullScriptHistoryMeds(request.LicenseId, request.Siteid, request.ProviderId, request.ICD10Code, request.SearchText,
                            request.FormularyID, request.FormularyActive, request.OtcCoverage, request.GenericDrugPolicy, request.UnlistedDrugPolicy, string.Empty, request.SelectedCoverageID,
                            request.GenericEquivalent, request.PatientRecordStatus.ToString(), request.DbID);

            if (selectMedicationHxMedDataModelList.Count == 0 && !string.IsNullOrWhiteSpace(request.SearchText))
            {
                messages.Add(new MessageModel() { Message = GetProviderHistorySearchNoMedMsg(), Icon = MessageIcon.Information });
            }
            else
            {
                var sigIdList = selectMedicationHxMedDataModelList.Select(s => s.SigId).Distinct();

                // get sig text from sdr.SIG and dbo.Free_From_Sig tables in mother db and update object item in selectMedicationHxMedDataModelList
                var allSigTexts = Allscripts.Impact.Sig.GetFreeFormSigText(sigIdList.ToArray<string>(), ConnectionStringPointer.SHARED_DB);
                allSigTexts.PrimaryKey = new DataColumn[] { allSigTexts.Columns["SIGID"] };

                foreach (FullScriptHxMedDataModel rx in selectMedicationHxMedDataModelList)
                {
                    DataRow allSigTextsRow = allSigTexts.Rows.Find(rx.SigId);
                    if (allSigTextsRow == null || (allSigTextsRow["SIGText"] == null))
                        continue;
                    rx.SigText = Convert.ToString(allSigTextsRow["SIGText"]);
                    rx.Medication = rx.Medication + ", " + rx.SigText;
                    rx.MedicationBack = rx.MedicationBack + ", " + rx.SigText;
                }
            }

            if (selectMedicationHxMedDataModelList.Count > 50)
            {
                string message = string.IsNullOrWhiteSpace(request.SearchText) ? "Your usual Rx History contains" : "Your search returned";
                messages.Add(new MessageModel()
                {
                    Message = $"{message} more than 50 results. Consider searching with more specific terms.",
                    Icon = MessageIcon.Information
                });

            }
            return selectMedicationHxMedDataModelList;
        }

        public string GetProviderHistorySearchNoMedMsg()
        {
            switch (PageState.Cast<Constants.UserCategory>("UserType", Constants.UserCategory.GENERAL_USER))
            {
                case Constants.UserCategory.PROVIDER:
                case Constants.UserCategory.PHYSICIAN_ASSISTANT:
                case Constants.UserCategory.PHYSICIAN_ASSISTANT_SUPERVISED:
                    return "No medications match your favorites.";
                default:
                    return "No medications match Dr's favorites.";
            }
        }


        [HttpPost]
        public void ClearSelectedDx()
        {
            PageState.Remove("Diagnosis");
            PageState.Remove("ICD10CODE");
        }
        [HttpPost]
        public ApiResponse AddMedToScriptPad()
        {
            using (var timer = logger.StartTimer("AddMedToScriptPad"))
            {
                var response = new ApiResponse();
                try
                {
                    AppCode.AddRx.ScriptHelper rxP = new AppCode.AddRx.ScriptHelper();
                    PPTPlus.CopyAllCandidatesToScriptPadMeds(PageState, new PptPlus());
                    rxP.savePrescriptions();
                }
                catch (Exception ex)
                {
                    var errorMessage = ApiHelper.AuditException(ex.ToString(), PageState);
                    logger.Error("AddMedToScriptPad Exception: " + ex.ToString());
                    response.ErrorContext = new ErrorContextModel()
                    {
                        Error = ErrorTypeEnum.ServerError,
                        Message = errorMessage
                    };
                    timer.Message = string.Format("<ErrorContext>{0}</ErrorContext>", response.ErrorContext.ToLogString());
                }
                timer.Message = string.Format("<Response>{0}<Response>", response.ToLogString());
                return response;
            }
        }

        [HttpPost]
        public ApiResponse ValidateMedSection(SelectMedicationMedModel[] selectedMeds)
        {
            using (var timer = logger.StartTimer("ValidateMedSection"))
            {
                var response = new ApiResponse();
                try
                {
                    bool isValid;
                    var msgmod = ValidateMedSelection(selectedMeds, out isValid);
                    if (!isValid || msgmod.Message != null)
                    {
                        response.Payload = msgmod;
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex.ToString());
                    response.ErrorContext = new ErrorContextModel {Error = ErrorTypeEnum.ServerError, Message = ex.ToLogString()};
                    timer.Message = $"<ErrorContext>{response.ErrorContext.ToLogString()}</ErrorContext>";
                }

                timer.Message = $"<Request>{selectedMeds.ToLogString()}<Request> <Response>{response.ToLogString()}<Response>";
                return response;
            }
        }

        [HttpPost]
        public ApiResponse AddToScriptPad(SelectMedicationMedModel[] selectedMeds)
        {
            using (var timer = logger.StartTimer("AddToScriptPad"))
            {
                var response = new ApiResponse();
                PageState["CHECK_DUR"] = true;
                bool isValid = false;
                string page = Constants.PageNames.SCRIPT_PAD + "?from=" + Constants.PageNames.REDIRECT_TO_ANGULAR + "?componentName=" + Constants.PageNames.SELECT_MEDICATION;
                try
                {
                    MessageModel msgmod = ValidateMedSelection(selectedMeds, out isValid);
                    if (isValid && msgmod.Message == null)
                    {
                        int overrideCount = 0;
                        ArrayList rxList = GetScriptInfo(ref overrideCount, selectedMeds);
                        if (overrideCount > 0)
                        {
                            response.Payload = new SelectMedicationModel {ReturnAction = SelectMedicationReturnAction.ShowFormulary };
                        }
                        else
                        {
                            AddMedToScriptPad();
                        }
                    }
                    else
                    {
                        response.Payload = new SelectMedicationModel {MessageModel = msgmod};
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex.ToString());
                    response.ErrorContext = new ErrorContextModel() { Error = ErrorTypeEnum.ServerError, Message = ex.ToLogString() };
                    timer.Message = string.Format("<ErrorContext>{0}</ErrorContext>", response.ErrorContext.ToLogString());
                }
                timer.Message = string.Format("<Request>{0}<Request> <Response>{1}<Response>", selectedMeds.ToLogString(), response.ToLogString());
                return response;
            }
        }

        [HttpPost]
        public ApiResponse GetScriptPadMedicationHistory()
        {
            var response = new ApiResponse();
            using (var timer = logger.StartTimer("GetMedicationHistoryCompletion"))
            {
                try
                {
                    ArrayList rxList = (ArrayList)PageState["RxList"];
                    ScriptPadModel scriptMod = CheckDURMultiSelect(rxList);
                    if (scriptMod.showMediHistoryCompletionPopUp)
                    {
                        response.Payload = scriptMod;
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex.ToString());
                    response.ErrorContext = new ErrorContextModel() { Error = ErrorTypeEnum.ServerError, Message = ex.ToLogString() };
                    timer.Message = string.Format("<ErrorContext>{0}</ErrorContext>", response.ErrorContext.ToLogString());
                }
                timer.Message = string.Format("<Request><Request> <Response>{0}<Response>", response.ToLogString());
                return response;
            }
        }

        [HttpPost]
        public ApiResponse AddAndReview(SelectMedicationMedModel[] selectedMeds)
        {
            var response = new ApiResponse();
            PageState["CHECK_DUR"] = true;
            bool isValid = false;
            using (var timer = logger.StartTimer("AddAndReview"))
            {
                try
                {
                    MessageModel msgmod = ValidateMedSelection(selectedMeds, out isValid);
                    if (isValid && msgmod.Message == null)
                    {
                        int overrideCount = 0;
                        ArrayList rxList = GetScriptInfo(ref overrideCount, selectedMeds);
                        if (overrideCount > 0)
                        {
                            response.Payload = new SelectMedicationModel { ReturnAction = SelectMedicationReturnAction.ShowFormulary };
                        }
                        else
                        {
                            ScriptPadModel scriptMod = CheckDURMultiSelect(rxList);
                            if (scriptMod.showMediHistoryCompletionPopUp)
                            {
                                response.Payload = new SelectMedicationModel { ScriptPadModel = scriptMod};
                                PageState["FromSelectMedicationGrid"] = true;
                            }
                            else
                            {
                                AddMedToScriptPad();
                            }
                        }
                    }
                    else
                    {
                        response.Payload = new SelectMedicationModel { MessageModel = msgmod};
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex.ToString());
                    response.ErrorContext = new ErrorContextModel() { Error = ErrorTypeEnum.ServerError, Message = ex.ToLogString() };
                    timer.Message = string.Format("<ErrorContext>{0}</ErrorContext>", response.ErrorContext.ToLogString());
                }
                timer.Message = string.Format("<Request>{0}<Request> <Response>{1}<Response>", selectedMeds.ToLogString(), response.ToLogString());
                return response;
            }
        }

        private ScriptPadModel CheckDURMultiSelect(ArrayList rxList)
        {
            ArrayList tempList = new ArrayList();
            tempList.AddRange(rxList);
            tempList.AddRange(ScriptPadUtil.GetScriptPadMeds(PageState));

            var model = new ScriptPadModel();

            PageState["ReviewScriptPad"] = true;
            var ddiList = string.Join(",", from rx in tempList.Cast<Rx>().ToList() select rx.DDI);

            DataSet dsActiveScripts = Prescription.CheckPatientRxHistoryForMultiSelect(ddiList, ApiHelper.GetSessionPatientId(PageState), ApiHelper.GetDBID(PageState));

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

            return model;

        }

        [HttpPost]
        public ApiResponse SelectSig(SelectMedicationMedModel selectedMed)
        {
            using (var timer = logger.StartTimer("AddAndReview"))
            {
                MessageModel msgmod = new MessageModel();
                var response = new ApiResponse();
                PageState["CHECK_DUR"] = true;
                int controlSubstanceCode;
                bool bHasControlSubstanceCode;
                try
                {
                    if (string.IsNullOrEmpty(selectedMed.ControlledSubstanceCode))
                    {
                        controlSubstanceCode = int.MinValue; //Default to a invalid control substance code. 
                        bHasControlSubstanceCode = false;
                    }
                    else
                    {
                        try
                        {
                            controlSubstanceCode = Convert.ToInt32(selectedMed.ControlledSubstanceCode);
                            bHasControlSubstanceCode = true;
                        }
                        catch
                        {
                            controlSubstanceCode = int.MinValue; //Default to a invalid control substance code. 
                            bHasControlSubstanceCode = false;
                        }

                        using (var masterPage = ApiHelper.GetMasterPage(pageState))
                        {
                            if (masterPage.RxTask != null && TaskUtils.IsNonCsToCsChangeRxWorkflow(controlSubstanceCode, PageState.GetGuidOr0x0(Constants.SessionVariables.TaskScriptMessageId), masterPage.RxTask.TaskType))
                            {
                                msgmod.Message = " You cannot respond to a Non-Controlled Substance Change Request with a Controlled Substance Medication";
                                msgmod.Icon = MessageIcon.Error;
                                msgmod.Tag = Constants.MessageModelTag.NonCSTaskToCSMed;
                                response.Payload = new SelectMedicationModel { MessageModel = msgmod };
                                return response;
                            }
                        }
                        if (pageState.GetBooleanOrFalse(Constants.SessionVariables.IsReconcileREFREQNonCS))
                        {
                            msgmod.Message = " You cannot select a controlled substance while reconciling a non-controlled medication.  Please deny the request and send a new script.";
                            msgmod.Icon = MessageIcon.Error;
                            msgmod.Tag = Constants.MessageModelTag.ReconcileNonCSToCS;
                            response.Payload = new SelectMedicationModel { MessageModel = msgmod };
                            return response;
                        }
                    }

                    if (bHasControlSubstanceCode && PageState.GetBooleanOrFalse(Constants.SessionVariables.HasExpiredDEA))
                    {
                        msgmod.Message = "Your " + PageState.GetStringOrEmpty("DEA") +
                                         " DEA is expired. It will need to be updated in order to proceed";
                        msgmod.Icon = MessageIcon.Error;
                        response.Payload = new SelectMedicationModel {MessageModel = msgmod};
                        return response;
                    }

                    bool PA_DEASchedAllowed = false;
                    bool Supervisor_DEASchedAllowed = false;
                    bool hasScheduleValid = false;
                    
                    Constants.UserCategory userType = AppCode.StateUtils.UserInfo.GetSessionUserType(PageState);
                    if (userType == Constants.UserCategory.PROVIDER ||
                        userType == Constants.UserCategory.PHYSICIAN_ASSISTANT)
                    {

                        if ((!bHasControlSubstanceCode) ||
                            ((List<string>) (PageState["DEASCHEDULESALLOWED"])).Contains(controlSubstanceCode.ToString()))
                        {
                            response = createRx(selectedMed, response);
                        }
                        else
                        {
                            response.Payload =
                                new SelectMedicationModel {MessageModel = ShowCSError(msgmod, controlSubstanceCode)};
                        }
                    }
                    else
                    {

                        if (bHasControlSubstanceCode)
                        {
                            if (PageState["DEASCHEDULESALLOWED"] != null)
                            {
                                PA_DEASchedAllowed =
                                    ((List<string>) (PageState["DEASCHEDULESALLOWED"])).Contains(controlSubstanceCode
                                        .ToString());
                            }

                            if (PageState["DEASCHEDULESALLOWED_SUPERVISOR"] != null)
                            {
                                Supervisor_DEASchedAllowed =
                                    ((List<string>) (PageState["DEASCHEDULESALLOWED_SUPERVISOR"])).Contains(
                                        controlSubstanceCode.ToString());
                                if ((SupervisingProvider != null && SupervisingProvider.IsDEAExpired() == true) ||
                                    (DelegateProvider != null && DelegateProvider.IsDEAExpired()))
                                {
                                    msgmod.Message =
                                        "You can not prescribe this medication. Provider's DEA is expired ";
                                    msgmod.Icon = MessageIcon.Error;
                                    response.Payload = new SelectMedicationModel {MessageModel = msgmod};
                                    return response;
                                }
                            }

                            if (userType == Constants.UserCategory.PHYSICIAN_ASSISTANT_SUPERVISED)
                            {
                                //PA and super visior both should have the schedule to proceed. 
                                if ((PA_DEASchedAllowed && Supervisor_DEASchedAllowed) ||
                                    bHasControlSubstanceCode == false)
                                {
                                    hasScheduleValid = true;
                                }
                            }
                            else
                            {
                                //POB will check supervisor's schedule
                                if (Supervisor_DEASchedAllowed || bHasControlSubstanceCode == false)
                                {
                                    hasScheduleValid = true;
                                }
                            }

                            if (hasScheduleValid)
                            {
                                response = createRx(selectedMed, response);
                            }
                            else
                            {
                                response.Payload = new SelectMedicationModel
                                {
                                    MessageModel = ShowCSError(msgmod, controlSubstanceCode)
                                };
                            }
                        }
                        else
                        {
                            response = createRx(selectedMed, response);
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex.ToString());
                    response.ErrorContext = new ErrorContextModel()
                    {
                        Error = ErrorTypeEnum.ServerError,
                        Message = ex.ToLogString()
                    };
                    timer.Message = string.Format("<ErrorContext>{0}</ErrorContext>",
                        response.ErrorContext.ToLogString());
                }

                timer.Message = string.Format("<Request>{0}<Request> <Response>{1}<Response>",
                        selectedMed.ToLogString(), response.ToLogString());
                    return response;
                }
            }

        [HttpPost]
        public ApiResponse CompleteSelectSig()
        {
            using (var timer = logger.StartTimer("CompleteSelectSig"))
            {
                var response = new ApiResponse();
                try
                {
                    PPTPlus.CopyAllCandidatesToScriptPadMeds(PageState, new PptPlus());

                    Constants.UserCategory userType = AppCode.StateUtils.UserInfo.GetSessionUserType(PageState);
                    if (userType == Constants.UserCategory.PROVIDER || userType == Constants.UserCategory.PHYSICIAN_ASSISTANT || userType == Constants.UserCategory.PHYSICIAN_ASSISTANT_SUPERVISED 
                        || PageState.Cast(Constants.SessionVariables.TaskType, Constants.PrescriptionTaskType.DEFAULT)== Constants.PrescriptionTaskType.RXCHG
                        || PageState.Cast(Constants.SessionVariables.TaskType, Constants.PrescriptionTaskType.DEFAULT) == Constants.PrescriptionTaskType.RXCHG_PRIORAUTH)//ChangeRx Logic goes through SIG even for POB :(

                    {
                        response.Payload = Constants.PageNames.SIG;
                    }
                    else
                    {
                        response.Payload = Constants.PageNames.NURSE_SIG;
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex.ToString());
                    response.ErrorContext = new ErrorContextModel() { Error = ErrorTypeEnum.ServerError, Message = ex.ToLogString() };
                    timer.Message = string.Format("<ErrorContext>{0}</ErrorContext>", response.ErrorContext.ToLogString());
                }
                timer.Message = string.Format("<Request><Request><Response>{0}<Response>", response.ToLogString());
                return response;
            }
        }
        private ApiResponse createRx(SelectMedicationMedModel selectedMed, ApiResponse response)
        {
            ArrayList rxList = new ArrayList();
            Rx rx = new Rx();
            rx.RxID = Guid.NewGuid().ToString();
            rx.FormularyStatus = selectedMed.FormularyStatus.ToString();

            if (!string.IsNullOrEmpty(selectedMed.FormStatusCode))
                rx.SourceFormularyStatus = Convert.ToInt32(selectedMed.FormStatusCode.ToString());

            rx.LevelOfPreferedness = selectedMed.LevelOfPreferedness == null ? string.Empty : selectedMed.LevelOfPreferedness.ToString();
            rx.IsOTC = selectedMed.IsOTC ? "Y" : "N";
            rx.DDI = selectedMed.DDI;
            rx.MedicationName = selectedMed.MedicationName;
            PageState[Constants.SessionVariables.GPI] = selectedMed.GPI;
            rx.NDCNumber = selectedMed.NDC;
            rx.Strength = selectedMed.Strength.ToString();
            rx.StrengthUOM = selectedMed.StrengthUOM.ToString();
            rx.DosageFormCode = selectedMed.DosageFormCode;
            rx.DosageFormDescription = selectedMed.DosageForm.ToString();
            rx.RouteOfAdminCode = selectedMed.RouteofAdminCode;
            rx.RouteOfAdminDescription = selectedMed.RouteofAdmin;
            rx.ControlledSubstanceCode = selectedMed.ControlledSubstanceCode;
            rx.ICD10Code = PageState.GetStringOrEmpty("ICD10CODE");

            if (!string.IsNullOrEmpty(rx.ICD10Code))
            {
                //base.AuditLogPatientInsert(ePrescribeSvc.AuditAction.PATIENT_DX_CREATED, base.SessionPatientID);
            }

            rx.IsBrandNameMed = selectedMed.IsGeneric.ToString() == "N";

            rx.CoverageID = 0;
            if (PageState["SelectedCoverageID"] != null)
            {
                rx.CoverageID = Convert.ToInt64(PageState["SelectedCoverageID"].ToString());
            }

            if (PageState["FormularyID"] != null)
            {
                rx.FormularyID = PageState["FormularyID"].ToString().Trim();
            }

            if (PageState["PlanID"] != null)
            {
                rx.PlanID = PageState["PlanID"].ToString().Trim();
            }

            if (!string.IsNullOrEmpty(selectedMed.PriorAuth.ToString()))
            {
                rx.PriorAuthRequired = bool.Parse(selectedMed.PriorAuth.ToString());
            }

            if (PageState["ORIGINALDDI"] != null && !string.IsNullOrEmpty(PageState["ORIGINALDDI"].ToString()))
            {
                rx.OriginalDDI = PageState["ORIGINALDDI"].ToString();
            }
            rx.PreDURPAR = selectedMed.PreDURPAR;
            rx.PreDURDrugDrug = selectedMed.PreDURDrugDrug;
            rx.PreDURDUP = selectedMed.PreDURDUP;

            rx.PreDURDisease = "N";
            rx.PreDURDose = "N";
            rx.PreDURDrugAlcohol = "N";
            rx.PreDURDrugFood = "N";

            rx.IsCouponAvailable = false;
            if (!string.IsNullOrEmpty(selectedMed.IsCouponAvailable.ToString()))
            {
                rx.IsCouponAvailable = Convert.ToBoolean(selectedMed.IsCouponAvailable.ToString());
            }

            rx.IsSpecialtyMed = selectedMed.IsSpecialtyMed;

            List<string> deaSchedules = new List<string>();
            if (
                (Constants.UserCategory)PageState[Constants.SessionVariables.UserType] == Constants.UserCategory.PROVIDER ||
                (Constants.UserCategory)PageState[Constants.SessionVariables.UserType] == Constants.UserCategory.PHYSICIAN_ASSISTANT
                )
            {
                deaSchedules = (List<string>)(PageState["DEASCHEDULESALLOWED"]);
            }
            else
            {
                deaSchedules = (List<string>)(PageState["DEASCHEDULESALLOWED_SUPERVISOR"]);
            }


            if (iPrescription.IsValidMassOpiate(
                    PageState.GetStringOrEmpty(Constants.SessionVariables.PracticeState),
                    selectedMed.GPI != null ? selectedMed.GPI : string.Empty,
                    rx.ControlledSubstanceCode,
                    Convert.ToBoolean(PageState["HasExpiredDEA"]),
                    deaSchedules))
            {
                rx.Notes = Constants.PartialFillUponPatientRequest;
            }

            rxList.Add(rx);
            PPTPlus.UpdateRxIdToCandidatesMed(PageState, selectedMed.index, Guid.Parse(rx.RxID));

            PageState["RxList"] = rxList;

            //At this point rx is set
            UpdateRxChangeWorkflowSession();

            if (SessionLicense.EnterpriseClient.ForceFormularyWarning && PageState["FORM_REQUIRE_REASON"] != null && PageState["FORM_REQUIRE_REASON"].ToString() == "Y")
            {
                string altPlanID = string.Empty;
                if (PageState["ALTPLANID"] != null)
                {
                    altPlanID = PageState["ALTPLANID"].ToString();
                }
                string copayID = string.Empty;
                if (PageState["COPAYID"] != null)
                {
                    copayID = PageState["COPAYID"].ToString();
                }

                if (Medication.GetBetterFormularyAlternativesExist(rx.DDI, rx.FormularyID, rx.PlanID, altPlanID, selectedMed.FormularyStatus, copayID, ApiHelper.GetDBID(PageState)))
                {
                    PageState["OverrideRxList"] = rxList;
                    response.Payload = new SelectMedicationModel {ReturnAction = SelectMedicationReturnAction.ShowFormulary};
                }
                else
                {
                    response.Payload = new SelectMedicationModel {RedirectUrl = Constants.PageNames.SIG};
                }
            }
            else
            {
                response.Payload = new SelectMedicationModel {RedirectUrl = Constants.PageNames.SIG};
            }

            return response;
        }

        public MessageModel ShowCSError(MessageModel msgmod, int controlSubstanceCode)
        {
            msgmod.Message = "Your profile is not set up to prescribe schedule";
            msgmod.Icon = MessageIcon.Error;


            if (controlSubstanceCode == 1)
                msgmod.Message += " I";
            else if (controlSubstanceCode == 2)
                msgmod.Message += " II";
            else if (controlSubstanceCode == 3)
                msgmod.Message += " III";
            else if (controlSubstanceCode == 4)
                msgmod.Message += " IV";
            else if (controlSubstanceCode == 5)
                msgmod.Message += " V";

            msgmod.Message += " medications.";

            return msgmod;
        }
       
        private MessageModel ValidateMedSelection(SelectMedicationMedModel[] selectedMeds, out bool isValid)
        {
            MessageModel msgmod = new MessageModel();
            foreach (SelectMedicationMedModel med in selectedMeds)
            {
                string ControlledSubstanceCode = med.ControlledSubstanceCode != null ? med.ControlledSubstanceCode : string.Empty;

                int controlSubstanceCode;
                bool bHasControlSubstanceCode;
                if (ControlledSubstanceCode.Trim() == string.Empty || ControlledSubstanceCode.Trim() == "U")
                {
                    controlSubstanceCode = int.MinValue;
                    bHasControlSubstanceCode = false;
                }
                else
                {
                    try
                    {
                        controlSubstanceCode = Convert.ToInt32(ControlledSubstanceCode);
                        bHasControlSubstanceCode = true;
                    }
                    catch (FormatException)
                    {
                        controlSubstanceCode = int.MinValue; //Default to a invalid control substance code. 
                        bHasControlSubstanceCode = false;
                    }
                }

                Constants.UserCategory userType = AppCode.StateUtils.UserInfo.GetSessionUserType(PageState);

                if (userType == Constants.UserCategory.PROVIDER)
                {
                    bool DEASchedAllowed = false;

                    if (((List<string>)(PageState["DEASCHEDULESALLOWED"])).Contains(controlSubstanceCode.ToString()) ||
                        bHasControlSubstanceCode == false)
                    {
                        DEASchedAllowed = true;
                    }
                    //Check if user's profile is set up to prescribe the scheduled medication. 
                    if (DEASchedAllowed)
                    {
                        isValid = true;
                    }
                    else
                    {
                        isValid = false;
                        msgmod = ShowCSError(msgmod, controlSubstanceCode);

                        //User should not allow to prescribe scheduled drugs
                        break;
                    }

                    if (bHasControlSubstanceCode && Convert.ToBoolean(PageState["HasExpiredDEA"]))
                    {
                        isValid = false;
                        msgmod.Message = "Your " + PageState["DEA"].ToString() +
                                                " DEA is expired. It will need to be updated in order to proceed";
                        msgmod.Icon = MessageIcon.Error;
                    }
                }
                else
                {
                    isValid = true;
                    if (bHasControlSubstanceCode)
                    {
                        bool PA_DEASchedAllowed = false;
                        bool Supervisor_DEASchedAllowed = false;


                        if (PageState["DEASCHEDULESALLOWED"] != null)
                        {
                            PA_DEASchedAllowed = ((List<string>)(PageState["DEASCHEDULESALLOWED"])).Contains(controlSubstanceCode.ToString());
                        }

                        if (PageState["DEASCHEDULESALLOWED_SUPERVISOR"] != null)
                        {
                            Supervisor_DEASchedAllowed = ((List<string>)(PageState["DEASCHEDULESALLOWED_SUPERVISOR"])).Contains(controlSubstanceCode.ToString());

                            if ((SupervisingProvider != null && SupervisingProvider.IsDEAExpired() == true) || (DelegateProvider != null && DelegateProvider.IsDEAExpired()))
                            {
                                msgmod.Message = "You can not prescribe this medication. Provider's DEA is expired ";
                                msgmod.Icon = MessageIcon.Error;
                                isValid = false;
                                break;
                            }
                        }

                        if (userType == Constants.UserCategory.PHYSICIAN_ASSISTANT_SUPERVISED)
                        {
                            isValid = (PA_DEASchedAllowed && Supervisor_DEASchedAllowed);
                        }
                        else if (userType == Constants.UserCategory.PHYSICIAN_ASSISTANT)
                        {
                            isValid = PA_DEASchedAllowed;
                        }
                        else
                        {
                            isValid = Supervisor_DEASchedAllowed;
                        }

                        if (!isValid)
                        {
                            msgmod = ShowCSError(msgmod, controlSubstanceCode);
                            //User should not be allowed to prescribe scheduled drugs
                            break;
                        }
                    }
                }

                var medValidator = new MedicationValidator();
                //Validation for Quantity
                string txtquantity = med.Quantity;
                if (isValid)
                {
                    try
                    {
                        if (!medValidator.IsValidQuantity(txtquantity))
                        {
                            msgmod.Message = Constants.ErrorMessages.MedicationQuantityInvalid;
                            msgmod.Icon = MessageIcon.Error;
                            isValid = false;
                        }
                        else
                        {
                            var qty = Convert.ToDecimal(txtquantity);

                            decimal packageSize;
                            if (
                                !decimal.TryParse(med.PackageSize, out packageSize))
                                packageSize = 1;

                            int packageQuantity;
                            if (
                                !int.TryParse(med.PackageQuantity, out packageQuantity))
                                packageQuantity = 1;

                            if (!medValidator.IsValidCalculatedQuantity(qty, packageQuantity, packageSize))
                            {
                                msgmod.Message = " The calculated quantity of one of your selected medications is > 9999. Please review each of your selected medications for accuracy of package size selection.";
                                msgmod.Icon = MessageIcon.Error;
                                isValid = false;
                            }
                        }
                    }
                    catch
                    {
                        msgmod.Message = " Please enter numbers only for Quantity. ";
                        msgmod.Icon = MessageIcon.Error;
                        isValid = false;
                    }
                }

                //Validation for Refills
                if (isValid)
                {
                    int txtRefills = med.Refill;
                    try
                    {
                        if (ControlledSubstanceCode.Trim() == "2")
                        {
                            if (!medValidator.IsValidCs2Refills(txtRefills))
                            {
                                msgmod.Message = Constants.ErrorMessages.MedicationCs2RefillsInvalid;
                                msgmod.Icon = MessageIcon.Error;
                                isValid = false;
                            }
                        }
                        else if (ControlledSubstanceCode.Trim() == "3" || ControlledSubstanceCode.Trim() == "4" ||
                                 ControlledSubstanceCode.Trim() == "5")
                        {
                            if (!medValidator.IsValidCs3Or4Or5Refills(txtRefills))
                            {
                                msgmod.Message = Constants.ErrorMessages.MedicationCs3Or4Or5RefillInvalid;
                                msgmod.Icon = MessageIcon.Error;
                                isValid = false;
                            }
                        }

                        if (!medValidator.IsValidRefills(txtRefills))
                        {
                            msgmod.Message = Constants.ErrorMessages.MedicationRefillsInvalid;
                            msgmod.Icon = MessageIcon.Error;
                            isValid = false;
                        }
                    }
                    catch
                    {
                        msgmod.Message = Constants.ErrorMessages.MedicationRefillsInvalid;
                        msgmod.Icon = MessageIcon.Error;
                        isValid = false;
                    }
                }

                //Validation for Days
                if (isValid)
                {
                    int txtDays = med.DayOfSupply;
                    try
                    {
                        if (ControlledSubstanceCode.Trim() == "2")
                        {
                            if (!medValidator.IsValidCs2DaysSupply(txtDays))
                            {
                                msgmod.Message = Constants.ErrorMessages.MedicationCs2DaysSupplyInvalid;
                                msgmod.Icon = MessageIcon.Error;
                                isValid = false;
                            }
                        }
                        else if (ControlledSubstanceCode.Trim() == "3" || ControlledSubstanceCode.Trim() == "4" ||
                                 ControlledSubstanceCode.Trim() == "5")
                        {
                            string errorMsg;
                            if (!medValidator.IsValidCs3Or4Or5DaysSupply(txtDays, out errorMsg))
                            {
                                msgmod.Message = errorMsg;
                                msgmod.Icon = MessageIcon.Error;
                                isValid = false;
                            }
                        }
                        else
                        {
                            if (!medValidator.IsValidDaysSupply(txtDays))
                            {
                                msgmod.Message = Constants.ErrorMessages.MedicationDaysSupplyInvalid;
                                msgmod.Icon = MessageIcon.Error;
                                isValid = false;
                            }
                        }
                    }
                    catch
                    {
                        msgmod.Message = Constants.ErrorMessages.MedicationDaysSupplyInvalid;
                        msgmod.Icon = MessageIcon.Error;
                        isValid = false;
                    }
                }

                if (!isValid)
                {
                    msgmod.Tag = Constants.MessageModelTag.MedInvalid;
                    return msgmod;
                }
            }
            isValid = true;
            msgmod.Tag = Constants.MessageModelTag.MedInvalid;
            return msgmod;
        }

        private ArrayList GetScriptInfo(ref int overrideCount, SelectMedicationMedModel[] selectedMeds)
        {
            Rx rx = new Rx();
            ArrayList rxList = new ArrayList();
            ArrayList overrideList = new ArrayList();

            logger.Debug("GetScriptInfo()");
            foreach (SelectMedicationMedModel med in selectedMeds)
            {
                rx = new Rx();
                rx.RxID = Guid.NewGuid().ToString();
                rx.DDI = med.DDI;

                //logger.Debug($"SelectedIndex = {gridItem.ItemIndex} | RxID = {rx.RxID} | DDI = {rx.DDI}");

                rx.FormularyStatus = med.FormularyStatus.ToString();
                if (
                    !string.IsNullOrEmpty(med.FormStatusCode))
                    rx.SourceFormularyStatus = Convert.ToInt32(med.FormStatusCode.ToString());

                rx.LevelOfPreferedness = med.LevelOfPreferedness == null ? string.Empty : med.LevelOfPreferedness.ToString();
                rx.IsOTC = med.IsOTC.ToString();
                rx.ControlledSubstanceCode = med.ControlledSubstanceCode != null ? med.ControlledSubstanceCode : string.Empty;

                //Set the package information of the script
                rx.GPPC = med.GPPC.ToString();
                if (!string.IsNullOrEmpty(med.PackageSize))
                {
                    rx.PackageSize = Convert.ToDecimal(med.PackageSize);
                }

                if (!string.IsNullOrEmpty(med.PackageQuantity))
                {
                    rx.PackageQuantity = Convert.ToInt32(med.PackageQuantity.ToString());
                }

                rx.PackageUOM = med.PackageUOM.ToString();
                rx.PackageDescription = med.PackageDescription.ToString();


                rx.Quantity = Convert.ToDecimal(med.Quantity);
                rx.Refills = Convert.ToInt32(med.Refill);
                rx.DAW = med.DAW;
                rx.DaysSupply = Convert.ToInt32(med.DayOfSupply);

                rx.MedicationName = med.MedicationName;
                rx.Strength = med.Strength;
                rx.StrengthUOM = med.StrengthUOM;
                rx.RouteOfAdminCode = med.RouteofAdminCode;
                rx.RouteOfAdminDescription = med.RouteofAdmin;
                rx.DosageFormCode = med.DosageFormCode;
                rx.DosageFormDescription = med.DosageForm;
                string stringMDD = String.Empty;
                rx.SigID = med.SIGID.ToString();
                rx.SigText =
                    GetParsedSigText(med.SIGText.ToString(),
                        out stringMDD, rx.ControlledSubstanceCode);
                rx.SigTypeId = med.SIGTypeID;
                rx.MDD = stringMDD;
                rx.ICD10Code = PageState.GetStringOrEmpty("ICD10CODE");

                //if (!string.IsNullOrEmpty(rx.ICD10Code))
                //{
                //    base.AuditLogPatientInsert(ePrescribeSvc.AuditAction.PATIENT_DX_CREATED, base.SessionPatientID);
                //}

                rx.CoverageID = (PageState["SelectedCoverageID"] != null)
                    ? Convert.ToInt64(PageState["SelectedCoverageID"].ToString())
                    : 0; //mist

                if (PageState["FormularyID"] != null)
                    rx.FormularyID = PageState["FormularyID"].ToString().Trim();

                if (PageState["PlanID"] != null)
                    rx.PlanID = PageState["PlanID"].ToString().Trim();

                if (!string.IsNullOrEmpty(med.PriorAuth.ToString()))
                {
                    rx.PriorAuthRequired = bool.Parse(med.PriorAuth.ToString());
                }

                rx.NDCNumber = med.NDC.ToString();

                // TODO ************************

                //Label lblTemp = gridItem.FindControl("lblFormuAltsShown") as Label;
                rx.FormularyAlternativeShown = string.Empty;

                rx.PreDURPAR = med.PreDURPAR;
                rx.PreDURDrugDrug = med.PreDURDrugDrug;
                rx.PreDURDUP = med.PreDURDUP;

                // ************************

                rx.PreDURDisease = "N";
                rx.PreDURDose = "N";
                rx.PreDURDrugAlcohol = "N";
                rx.PreDURDrugFood = "N";

                rx.IsCouponAvailable = med.IsCouponAvailable;
                rx.IsSpecialtyMed = med.IsSpecialtyMed;

                List<string> deaSchedules = new List<string>();
                if (
                    (Constants.UserCategory)PageState[Constants.SessionVariables.UserType] == Constants.UserCategory.PROVIDER ||
                    (Constants.UserCategory)PageState[Constants.SessionVariables.UserType] == Constants.UserCategory.PHYSICIAN_ASSISTANT
                    )
                {
                    deaSchedules = (List<string>)(PageState["DEASCHEDULESALLOWED"]);
                }
                else
                {
                    deaSchedules = (List<string>)(PageState["DEASCHEDULESALLOWED_SUPERVISOR"]);
                }

                if (iPrescription.IsValidMassOpiate(
                    PageState.GetStringOrEmpty(Constants.SessionVariables.PracticeState),
                    med.GPI != null ? med.GPI : string.Empty,
                    rx.ControlledSubstanceCode,
                    Convert.ToBoolean(PageState["HasExpiredDEA"]),
                    deaSchedules))
                {
                    rx.Notes = Constants.PartialFillUponPatientRequest;
                }

                rxList.Add(rx);

                if (SessionLicense.EnterpriseClient.ForceFormularyWarning && PageState["FORM_REQUIRE_REASON"] != null &&
                    PageState["FORM_REQUIRE_REASON"].ToString() == "Y")
                {
                    int formularyStatus = -1;
                    int.TryParse(
                        med.FormularyStatus.ToString(),
                        out formularyStatus);
                    string altPlanID = string.Empty;
                    if (PageState["ALTPLANID"] != null)
                    {
                        altPlanID = PageState["ALTPLANID"].ToString();
                    }
                    string copayID = string.Empty;
                    if (PageState["COPAYID"] != null)
                    {
                        copayID = PageState["COPAYID"].ToString();
                    }
                    if (Medication.GetBetterFormularyAlternativesExist(rx.DDI, rx.FormularyID, rx.PlanID, altPlanID,
                        formularyStatus, copayID, ApiHelper.GetDBID(PageState)))
                    {
                        overrideList.Add(rx);
                    }
                }
                PPTPlus.UpdateRxIdToCandidatesMed(PageState, med.index, Guid.Parse(rx.RxID));

            }

            overrideCount = overrideList.Count;
            PageState["OverrideRxList"] = overrideList;
            PageState["RxList"] = rxList;
            return rxList;
        }

        public string GetParsedSigText(string sigTextMDD, out string MDD, string controlSubstanceCode)
        {
            MDD = String.Empty;
            if (String.IsNullOrWhiteSpace(sigTextMDD))
            {
                return String.Empty;
            }

            string parsedSigText = String.Empty;
            int indexMDD = sigTextMDD.IndexOf("MDD:");
            if (indexMDD > 0)
            {
                parsedSigText = sigTextMDD.Substring(0, indexMDD);
                string mddString = sigTextMDD.Substring(indexMDD + 4);
                int indexPerDay = mddString.IndexOf("Per Day");
                bool addMDD = IsMDDSiteEnabled(controlSubstanceCode);
                if (addMDD && indexPerDay > 0)
                {
                    MDD = mddString.Substring(0, indexPerDay);
                }
            }
            else
            {
                parsedSigText = sigTextMDD;
            }

            return parsedSigText;
        }
        public bool IsMDDSiteEnabled(string controlSubstanceCode)
        {
            bool isMDD = PageState.GetBooleanOrFalse("ALLOWMDD");
            if (isMDD)
            {
                bool csMedOnly = PageState.GetBooleanOrFalse("CSMEDSONLY");
                if (csMedOnly)
                {
                    if (String.IsNullOrWhiteSpace(controlSubstanceCode))
                    {
                        isMDD = false;
                    }
                }
            }

            return isMDD;
        }
        private void UpdateRxChangeWorkflowSession()
        {
            var masterPage = ApiHelper.GetMasterPage(PageState);
            if (masterPage?.RxTask != null && masterPage.RxTask.TaskType == Constants.PrescriptionTaskType.RXCHG)
            {
                RxTaskModel rxObj = masterPage.RxTask;
                var sm = rxObj.ScriptMessage as ScriptMessage;
                var messageIDStr = rxObj.ScriptMessageGUID as string;
                int reqIndex = rxObj.RequestedRxIndexSelected;
                PageState[Constants.SessionVariables.RxIdToDiscontinue] = sm?.DBRxID;
                //Coming from approve task screen wanting to change a ChangeRxMed
                masterPage.RxTask = new RxTaskModel
                {
                    ScriptMessage = sm,
                    ScriptMessageGUID = messageIDStr,
                    RequestedRxIndexSelected = reqIndex,
                    RequestedRx = Allscripts.ePrescribe.Objects.RequestedRx.GetRequestedRxByOrdinal(sm.XmlMessage, reqIndex),
                    Rx = ApiHelper.GetCurrentRx(PageState),
                    UserId = PageState.GetGuidOr0x0(Constants.SessionVariables.UserId),
                    DbId = ApiHelper.GetDBID(PageState),
                    LicenseId = PageState.GetGuidOr0x0(Constants.SessionVariables.LicenseId),
                    SiteId = PageState.GetInt(Constants.SessionVariables.SiteId, 1),
                    ShieldSecurityToken = PageState.GetStringOrEmpty(Constants.SessionVariables.ShieldSecurityToken),
                    ExternalFacilityCd = PageState.GetStringOrEmpty(Constants.SessionVariables.ExternalFacilityCd),
                    ExternalGroupID = PageState.GetStringOrEmpty(Constants.SessionVariables.ExternalGroupID),
                    UserType = PageState.Cast(Constants.SessionVariables.UserType, Constants.UserCategory.GENERAL_USER),
                    DelegateProviderId = PageState.GetGuidOr0x0(Constants.SessionVariables.DelegateProviderId)
                };
            }
        }

        [HttpPost]
        public ApiResponse GetPatientCoveragesInfo()
        {
            using (var timer = logger.StartTimer("GetPatientCoveragesInfo"))
            {
                var response = new ApiResponse();
                try
                {
                    response.Payload = GetPatientCoverages();
                }
                catch (Exception ex)
                {
                    var errorMessage = ApiHelper.AuditException(ex.ToString(), PageState);
                    logger.Error("GetPatientCoveragesInfo Exception " + ex.ToString());
                    response.ErrorContext = new ErrorContextModel()
                    {
                        Error = ErrorTypeEnum.ServerError,
                        Message = errorMessage
                    };
                }
                timer.Message = string.Format("<Response>{0}</Response>", response.ToLogString());
                return response;
            }
        }
        [HttpPost]
        public ApiResponse GetSelectMedicationStartUpData()
        {
            using (var timer = logger.StartTimer("GetSelectMedicationStartUpData"))
            {
                SelectMedicationStartUpModel selectMedicationStartUpModel  = new SelectMedicationStartUpModel();
                var response = new ApiResponse();

                try
                {
                    selectMedicationStartUpModel.Messages = GetSelectMedicatinStartUpMessages();
                    selectMedicationStartUpModel.PatientCoverages = GetPatientCoverages();
                    selectMedicationStartUpModel.PatientDiagnosis =  PageState.GetStringOrEmpty("Diagnosis");
                    response.Payload = selectMedicationStartUpModel;
                    PageState.Remove(Constants.SessionVariables.ReviewScriptPadRedirection);
                    PPTPlus.CopyExistingMedsToScriptPadMeds(PageState, ScriptPadMeds, new PptPlus());
                }
                catch (Exception ex)
                {
                    var errorMessage = ApiHelper.AuditException(ex.ToString(), PageState);
                    logger.Error("GetselectMedicationStartUpData Exception " + ex.ToString());
                    response.ErrorContext = new ErrorContextModel()
                    {
                        Error = ErrorTypeEnum.ServerError,
                        Message = errorMessage
                    };
                }
                timer.Message = string.Format("<Response>{0}</Response>", response.ToLogString());
                return response;
            }
        }

        public List<MessageModel> GetSelectMedicatinStartUpMessages()
        {

            List<MessageModel> messages = new List<MessageModel>();
            MessageModel messageMod = new MessageModel();
            if (AppCode.StateUtils.UserInfo.IsPOBUser(PageState))
            {
                if (AppCode.StateUtils.UserInfo.GetSupervisingProviderName(PageState) != string.Empty)
                {
                    messages.Add(new MessageModel()
                    {
                        Message = "Prescription written on behalf of " + AppCode.StateUtils.UserInfo.GetDelegateProviderName(PageState) + " under the supervision of " + AppCode.StateUtils.UserInfo.GetSupervisingProviderName(PageState),
                        Icon = MessageIcon.Information,
                        ShowCloseButton = false
                    });
                }
                else
                {
                    messages.Add(new MessageModel()
                    {
                        Message = string.Format("Prescription written on behalf of {0}." , AppCode.StateUtils.UserInfo.GetDelegateProviderName(PageState)),
                        Icon = MessageIcon.Information,
                        ShowCloseButton = false
                    });
                }
            }
            else
            {
                if ((Convert.ToBoolean(PageState["IsPA"]) || Convert.ToBoolean(PageState["IsPASupervised"])) && AppCode.StateUtils.UserInfo.GetSessionUserType(PageState) != Constants.UserCategory.PHYSICIAN_ASSISTANT)
                {
                    messages.Add(new MessageModel()
                    {
                        Message = string.Format("Prescription being written under the supervision of {0}." , AppCode.StateUtils.UserInfo.GetDelegateProviderName(PageState)),
                        Icon = MessageIcon.Information,
                        ShowCloseButton = false
                    });
                }
                else
                {
                    if (AppCode.StateUtils.UserInfo.GetSessionUserType(PageState) != Constants.UserCategory.PROVIDER && AppCode.StateUtils.UserInfo.GetSessionUserType(PageState) != Constants.UserCategory.PHYSICIAN_ASSISTANT)
                    {

                        messages.Add(new MessageModel()
                        {
                            Message =string.Format("Prescription written on behalf of {0}." , AppCode.StateUtils.UserInfo.GetDelegateProviderName(PageState)),
                            Icon = MessageIcon.Information,
                            ShowCloseButton = false
                        });
                    }
                }
            }
            return messages;
        }

        [HttpPost]
        public ApiResponse SetRequestedMedicationAsCurrentMedication(SetRequestedMedicationAsCurrentMedicationRequest request)
        {
            using (var timer = logger.StartTimer("SetRequestedMedicationAsCurrentMedication"))
            {
                var response = new ApiResponse();
                try
                {
                    SetRequestedMedicationAsCurrentMedication(request.ScriptMessageGuid, request.RequestedRxDrugDescription, request.RxDetails, new Prescription());
                }
                catch (Exception ex)
                {
                    var errorMessage = ApiHelper.AuditException(ex.ToString(), PageState);
                    logger.Error("SetRequestedMedicationAsCurrentMedication Exception: " + ex.ToString());
                    response.ErrorContext = new ErrorContextModel()
                    {
                        Error = ErrorTypeEnum.ServerError,
                        Message = errorMessage
                    };
                    timer.Message = string.Format("<ErrorContext>{0}</ErrorContext>", response.ErrorContext.ToLogString());
                }
                timer.Message = string.Format("<Response>{0}<Response>", response.ToLogString());
                return response;
            }
        }
        public void SetRequestedMedicationAsCurrentMedication(string scriptMessageGuid, string requestedRxDrugDescription, string rxDetails, IPrescription prescription)
        {
            Rx rx = new Rx();
            ArrayList rxList = new ArrayList();
            ScriptMessage sm = new ScriptMessage(scriptMessageGuid, PageState["LICENSEID"].ToString(), PageState["USERID"].ToString(), (ConnectionStringPointer)PageState[Constants.SessionVariables.DbId]);

            rx.ScriptMessageID = scriptMessageGuid;

            PageState["PATIENTID"] = sm.DBPatientID;
            PageState["PHARMACYID"] = sm.DBPharmacyID;
            PageState["SentTo"] = sm.PharmacyName;
            PageState.Remove(Constants.SessionVariables.IsCsRefReqWorkflow); //reset workflow
            PageState.Remove(Constants.SessionVariables.ChangeRxRequestedMedCs);//reset workflow

            rx.ProviderID = sm.ProviderID;
            rx.SigText = sm.RxSIGText;

            if (!String.IsNullOrWhiteSpace(sm.DispensedRxRefills) && !sm.RxRefills.Equals("PRN"))
            {
                rx.Refills = Convert.ToInt32(sm.DispensedRxRefills);
            }

            rx.DAW = sm.DispensedDaw == "Y";

            //ok, if there's no DDI we'll have to search for it
            if (string.IsNullOrEmpty(sm.DBDDID))
            {
                var pharmDetails = StringHelper.GetPharmacyName(
                    sm.PharmacyName,
                    sm.PharmacyAddress1,
                    sm.PharmacyAddress2,
                    sm.PharmacyCity,
                    sm.PharmacyState,
                    sm.PharmacyZip,
                    sm.PharmacyPhoneNumber);
            }

            rx.DDI = sm.DBDDID;

            if (!string.IsNullOrEmpty(rx.DDI))
            {
                string coverageId = PageState.GetString("SelectedCoverageID", "0");
                string formularyId = PageState.GetStringOrEmpty("FormularyID");
                string otcCoverage = PageState.GetStringOrEmpty("OTCCoverage");
                int genericDrugPolicy = PageState.GetInt("GenericDrugPolicy", 0);
                int unListedDrugPolicy = PageState.GetInt("UnListedDrugPolicy", 0); ;

                ePrescribeSvc.Medication medicationInfo = EPSBroker.LoadMedicationByDDIAndCoverage(rx.DDI,
                    coverageId,
                    formularyId,
                    otcCoverage,
                    genericDrugPolicy,
                    unListedDrugPolicy,
                    (ConnectionStringPointer)PageState[Constants.SessionVariables.DbId]);

                if (medicationInfo != null)
                {
                    rx.MedicationName = medicationInfo.MedicationName;
                    rx.Strength = medicationInfo.Strength;
                    rx.ControlledSubstanceCode = medicationInfo.ControlledSubstanceCode;
                    rx.StrengthUOM = medicationInfo.StrengthUOM;
                    rx.RouteOfAdminCode = medicationInfo.RouteOfAdminCode;
                    rx.RouteOfAdminDescription = medicationInfo.RouteOfAdmin;
                    rx.DosageFormCode = medicationInfo.DosageFormCode;
                    rx.DosageFormDescription = medicationInfo.DosageForm;

                    rx.StateControlledSubstanceCode = Prescription.GetStateControlledSubstanceCode(rx.DDI, PageState["PRACTICESTATE"].ToString(), sm.PharmacyState, (ConnectionStringPointer)PageState[Constants.SessionVariables.DbId]);

                    if (PageState["FormularyActive"] != null && PageState["FormularyActive"].ToString() == "Y")
                    {
                        rx.FormularyStatus = medicationInfo.FormularyStatus;
                        rx.LevelOfPreferedness = medicationInfo.LevelOfPreferedness;
                        rx.IsOTC = medicationInfo.IsOTC;
                    }
                    rx.CoverageID = (PageState["SelectedCoverageID"] != null) ? Convert.ToInt64(PageState["SelectedCoverageID"].ToString()) : 0;
                    rx.FormularyID = (PageState["FormularyID"] != null) ? PageState["FormularyID"].ToString().Trim() : null;
                    rx.PlanID = (PageState["PlanID"] != null) ? PageState["PlanID"].ToString().Trim() : null;
                }

                if (prescription.IsValidMassOpiate(
                    PageState.GetStringOrEmpty(Constants.SessionVariables.PracticeState),
                    medicationInfo.GPI,
                    rx.ControlledSubstanceCode,
                    Convert.ToBoolean(PageState["HasExpiredDEA"]),
                    (List<string>)(PageState["DEASCHEDULESALLOWED"]))
                )
                {
                    rx.Notes = Constants.PartialFillUponPatientRequest;
                }
            }

            rxList.Add(rx);
            PageState[Constants.SessionVariables.TaskScriptMessageId] = scriptMessageGuid;

            PageState["RxList"] = rxList;
            var masterPage = ApiHelper.GetMasterPage(PageState);
            masterPage.ChangeRxRequestedMedCs = new ChangeRxRequestedMedCs();
            masterPage.ChangeRxRequestedMedCs.RequestedRxDrugDescription = requestedRxDrugDescription;
            masterPage.ChangeRxRequestedMedCs.ScriptMessageGuid = scriptMessageGuid;
            masterPage.ChangeRxRequestedMedCs.RxDetails = rxDetails;
            //Also clear script pad for current patient
            ScriptPadUtil.RemoveAllRxFromScriptPad(PageState);
        }
    }
}

