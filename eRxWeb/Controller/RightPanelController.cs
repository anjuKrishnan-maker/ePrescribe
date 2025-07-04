using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.Shared.Logging;
using eRxWeb.AppCode;
using eRxWeb.AppCode.Interfaces;
using eRxWeb.Controllers;
using eRxWeb.ServerModel;

[assembly: InternalsVisibleTo("Allscripts.ePrescribe.Test")]
namespace eRxWeb.Controller
{
    public partial class RightPanelController : ApiController
    {
        private static readonly ILoggerEx logger = LoggerEx.GetLogger();

        internal enum Panels
        {
            UrgentMessages = 0,
            EpcsNotice = 1,
            ImportantInfo = 2,
            EAuthMessages = 3,
            RightBox = 4,
            HelpContent = 5,
            RobustLink = 6,
            FormularyAlternatives = 7,
            GenericAlternatives = 8,
            ScriptPad = 9,
            CopayCoverage = 10,
            PharmacyInfo = 11,
            PptSummary = 12,
            NewPharmacy = 13,
            SigCopay = 14,
            MessageQueue = 16,
            EpcsSendToPharm = 17,
            ProgramAlert = 18,
            SpecialtyMedInfo = 19,
            PrivacyOverrideReason = 20,
            AuditLogInfo = 21,
            PdmpSummary = 22,
            GetEpcs = 23
        }

        internal static Dictionary<string, HashSet<Panels>> RightPanels => new Dictionary<string, HashSet<Panels>> 
        {
            {Constants.PageNames.SELECT_PATIENT.ToLower(), new HashSet<Panels> { Panels.UrgentMessages, Panels.EpcsNotice, Panels.ImportantInfo, Panels.EAuthMessages, Panels.RightBox, Panels.HelpContent}},
            {Constants.PageNames.SELECT_MEDICATION.ToLower(), new HashSet<Panels>{ Panels.RobustLink, Panels.FormularyAlternatives, Panels.GenericAlternatives, Panels.ScriptPad, Panels.RightBox, Panels.CopayCoverage, Panels.PptSummary, Panels.HelpContent, Panels.ProgramAlert} },
            {Constants.PageNames.APPROVE_REFILL_TASK.ToLower(), new HashSet<Panels> {Panels.FormularyAlternatives, Panels.GenericAlternatives, Panels.CopayCoverage, Panels.HelpContent} },
            {Constants.PageNames.PHARMACY_REFILL_DETAILS.ToLower(), new HashSet<Panels>{Panels.FormularyAlternatives, Panels.GenericAlternatives} },
            {Constants.PageNames.DOC_REFILL_MENU.ToLower(), new HashSet<Panels>{Panels.UrgentMessages, Panels.EAuthMessages, Panels.HelpContent} },
            {Constants.PageNames.LIST_SEND_SCRIPTS.ToLower(), new HashSet<Panels>{Panels.UrgentMessages, Panels.EAuthMessages, Panels.HelpContent} },
            {Constants.PageNames.TASK_SUMMARY.ToLower(), new HashSet<Panels>{Panels.UrgentMessages, Panels.EAuthMessages} },
            {Constants.PageNames.REVIEW_HISTORY.ToLower(), new HashSet<Panels>{Panels.EpcsNotice, Panels.ImportantInfo, Panels.HelpContent} },
            {Constants.PageNames.NURSE_SIG.ToLower(), new HashSet<Panels>{Panels.ScriptPad, Panels.EAuthMessages, Panels.SigCopay, Panels.HelpContent} },
            {Constants.PageNames.SIG.ToLower(), new HashSet<Panels>{Panels.ScriptPad, Panels.SigCopay, Panels.HelpContent} },
            {Constants.PageNames.RX_DUR_REVIEW_MULTI_SELECT.ToLower(), new HashSet<Panels>{Panels.ScriptPad, Panels.CopayCoverage} },
            {Constants.PageNames.PHARMACY.ToLower(), new HashSet<Panels>{Panels.PharmacyInfo, Panels.NewPharmacy, Panels.HelpContent} },
            {Constants.PageNames.MESSAGE_QUEUE_TX.ToLower(), new HashSet<Panels>{Panels.MessageQueue, Panels.HelpContent} },
            {Constants.PageNames.SCRIPT_PAD.ToLower(), new HashSet<Panels>{Panels.EpcsSendToPharm, Panels.SpecialtyMedInfo, Panels.PdmpSummary, Panels.HelpContent, Panels.GetEpcs} },
            {Constants.PageNames.VIEW_AUDIT_LOG.ToLower(), new HashSet<Panels>{Panels.PrivacyOverrideReason, Panels.AuditLogInfo} },
            {Constants.PageNames.MY_PROFILE.ToLower(), new HashSet<Panels>{Panels.HelpContent} },
            {Constants.PageNames.SETTINGS.ToLower(), new HashSet<Panels>{Panels.HelpContent} },
            {Constants.PageNames.NURSE_PHARMACY.ToLower(), new HashSet<Panels>{Panels.HelpContent} },
            {Constants.PageNames.REPORTS.ToLower(), new HashSet<Panels>{Panels.HelpContent} },
            {Constants.PageNames.TASK_SCRIPT_LIST.ToLower(), new HashSet<Panels>{Panels.HelpContent} },
            {Constants.PageNames.MULTIPLE_VIEW.ToLower(), new HashSet<Panels>{Panels.HelpContent} },
            {Constants.PageNames.MULTIPLE_VIEW_CSS.ToLower(), new HashSet<Panels>{Panels.HelpContent} },
            {Constants.PageNames.CHANGE_PATIENT_INSURANCE.ToLower(), new HashSet<Panels>{Panels.HelpContent} },
            {Constants.PageNames.CHANGE_SHIELD_USER_PASSWORD.ToLower(), new HashSet<Panels>{Panels.HelpContent} },
            {Constants.PageNames.EDIT_USER.ToLower(), new HashSet<Panels>{Panels.HelpContent} },
            {Constants.PageNames.NURSE_FREE_FORM_DRUG.ToLower(), new HashSet<Panels>{Panels.HelpContent} },
            {Constants.PageNames.FREE_FORM_DRUG.ToLower(), new HashSet<Panels>{Panels.HelpContent} },
            {Constants.PageNames.PATIENT_MED_HISTORY.ToLower(), new HashSet<Panels>{Panels.HelpContent} },
            {Constants.PageNames.PHARMACY_REFILL_SUMMARY.ToLower(), new HashSet<Panels>{Panels.HelpContent} },
            {Constants.PageNames.MY_EPCS_REPORTS.ToLower(), new HashSet<Panels>{Panels.HelpContent} },
            {Constants.PageNames.PRE_BUILT_MED_SELECT_SIG.ToLower(), new HashSet<Panels>{Panels.HelpContent} },
            {Constants.PageNames.RX_DUR_REVIEW.ToLower(), new HashSet<Panels>{Panels.HelpContent} },
            {Constants.PageNames.SELECT_DX.ToLower(), new HashSet<Panels>{Panels.HelpContent} },
            {Constants.PageNames.SITE_MANAGEMENT.ToLower(), new HashSet<Panels>{Panels.HelpContent} },
            {Constants.PageNames.APPROVE_SCRIPT_MESSAGE_PATIENT.ToLower(), new HashSet<Panels>{Panels.HelpContent} },
            {Constants.PageNames.PATIENT_DIAGNOSIS.ToLower(), new HashSet<Panels>{Panels.HelpContent} },
            {Constants.PageNames.PATIENT_ALLERGY.ToLower(), new HashSet<Panels>{Panels.HelpContent} },
            {Constants.PageNames.PATIENT_UPLOAD.ToLower(), new HashSet<Panels>{Panels.HelpContent} }
           
        };

        [HttpPost]
        public ApiResponse RetrievePayload([FromBody]string pageName)
        {
            using (var timer = logger.StartTimer("RetrievePayload"))
            {
                var asyncContext = new AsyncContext(HttpContext.Current, LocalLogContext.LogContextInfo, pageName, "ContentLoad");

                return new ApiResponse { Payload = RetrievePayload(asyncContext) };
            }

        }

        internal static RightPanelPayload RetrievePayload(AsyncContext asyncContext)
        {
            var result = BuildAsyncTaskList(new LoadAsyncUtil(asyncContext, logger));

            try
            {
                Task.WaitAll(result.Item1.ToArray());
            }
            catch (AggregateException e)
            {
                ApiHelper.AuditException("RightPanelPayloadException: " + e, asyncContext.Session);
            }

            return result.Item2;
        }


        internal static Tuple<List<Task>, RightPanelPayload> BuildAsyncTaskList(ILoadAsyncUtil asyncUtil)
        {
            var asyncTasks = new List<Task>();
            var payload = new RightPanelPayload { ScriptPadPayload = new List<ScriptPadModel>() };

            if (!RightPanels.ContainsKey(asyncUtil.AsyncContext.PageName.ToLower())) return new Tuple<List<Task>, RightPanelPayload>(asyncTasks, payload);

            var panelsToLoad = RightPanels[asyncUtil.AsyncContext.PageName.ToLower()];

            if (panelsToLoad.Contains(Panels.UrgentMessages))
            {
                asyncTasks.Add(asyncUtil.CreateTask(UrgentMessageApiController.checkUserMessages, p => payload.UrgentMessagePayload = p));
            }

            if (panelsToLoad.Contains(Panels.EpcsNotice))
            {
                asyncTasks.Add(asyncUtil.CreateTask(EPCSApiController.GetAvailableReportsCount, p => payload.EPCSNoticePayload = p));
            }

            if (panelsToLoad.Contains(Panels.ImportantInfo))
            {
                asyncTasks.Add(asyncUtil.CreateTask(ImportantInfoApiController.checkImportantInfo, p => payload.ImportantInfoPayload = p));
            }

            if (panelsToLoad.Contains(Panels.EAuthMessages))
            {
                asyncTasks.Add(asyncUtil.CreateTask(EAuthMessageApiController.getEauthMessage, p => payload.EAuthMessagePayload = p));
            }

            if (panelsToLoad.Contains(Panels.RightBox))
            {
                asyncTasks.Add(asyncUtil.CreateTask(RightBoxApiController.PrepareRightBoxData, p => payload.RightBoxPayload = p));
            }

            if (panelsToLoad.Contains(Panels.HelpContent))
            {
                asyncTasks.Add(asyncUtil.CreateTask(HelpContentApiController.GetHelpContent, p => payload.HelpContentPayload = p));
            }

            if (panelsToLoad.Contains(Panels.ScriptPad))
            {
                asyncTasks.Add(asyncUtil.CreateTask(ScriptPAdApiController.GetCurrentScriptPadData, p => payload.ScriptPadPayload = p));
            }

            if (panelsToLoad.Contains(Panels.SigCopay))
            {
                asyncTasks.Add(asyncUtil.CreateTask(FormularyAlternativesApiController.getFormularyAlternativesFromSession, p => payload.SigCopayPayload = p));
            }

            if (panelsToLoad.Contains(Panels.EpcsSendToPharm))
            {
                asyncTasks.Add(asyncUtil.CreateTask(GetEpcsSendToPharmacyApiController.getEpcsSendToPharm, p => payload.EpcsSendToPharmPayload = p));
            }

            if (panelsToLoad.Contains(Panels.SpecialtyMedInfo))
            {
                asyncTasks.Add(asyncUtil.CreateTask(ProgramAlertApiController.isScriptPadHasSpecMed, p => payload.SpecialtyMedInfoPayload = p));
            }

            if (panelsToLoad.Contains(Panels.GetEpcs))
            {
                asyncTasks.Add(asyncUtil.CreateTask(EPCSApiController.displayEpcsLink, p => payload.GetEpcsPayload = p));
            }


            return new Tuple<List<Task>, RightPanelPayload>(asyncTasks, payload);
        }
    }
}