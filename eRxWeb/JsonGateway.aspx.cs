using System;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Services;
using System.Web.UI;
using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.Data;
using Allscripts.ePrescribe.DatabaseSelector;
using Allscripts.ePrescribe.ExtensionMethods;
using Allscripts.ePrescribe.Objects;
using eRxWeb.AppCode;
using eRxWeb.State;

namespace eRxWeb
{
    public partial class JsonGateway : Page
    {
        /********    TEMPLATE    *********************
            [WebMethod(EnableSession = true)]
            public static void MethodName(jsonObject param)
            {
                var state = new StateContainer(HttpContext.Current.Session);
                using (var timer = logger.StartTimer("MethodName"))
                {
                    try
                    {
                        jsonObject response = ClassName.MethodName(state, param);
                        timer.Message($"<Request>{param}</Request><Response>response</Response>")
                    }
                    catch (Exception e)
                    {
                        logger.Error(e.ToString());
                        Audit.AddException(state.GetStringOrEmpty(Constants.SessionVariables.UserId), state.GetStringOrEmpty(Constants.SessionVariables.LicenseId),$"Exception during JsonGatway request: MethodName() - {e}", string.Empty, string.Empty, string.Empty, state.Cast(Constants.SessionVariables.DbId, ConnectionStringPointer.ERXDB_DEFAULT));
                        var jsonReturn = new JsonObject
                        {
                            {"success", false.ToString()},
                            {"message", e.Message }
                        };
                        return jsonReturn;
                    }
                }
            }
        */
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        [WebMethod(EnableSession = true)]
        public static string GetSpecialtyMedEnrollment()
        {
            var specMedSession = HttpContext.Current.Session[Constants.SessionVariables.SpecialtyMed];
            string jsonString = string.Empty;
            if (specMedSession != null)
            {
                jsonString = new JavaScriptSerializer().Serialize(specMedSession);
            }
            return jsonString;
        }

        [WebMethod(EnableSession = true)]
        public static void CheckSpecialtyMedEnrollment()
        {
            var pageState = new StateContainer(HttpContext.Current.Session);
            SpecialtyMed.IsProviderEnrolledInSpecialtyMed(pageState);                                     ;
        }

        [WebMethod(EnableSession = true)]
        public static void ConfirmOffer(string rxTaskID)
        {
            var pageState = new StateContainer(HttpContext.Current.Session);
            new SpecialtyMed().ConfirmOffer(pageState, rxTaskID, (ConnectionStringPointer)pageState[Constants.SessionVariables.DbId]); 
        }

        [WebMethod(EnableSession = true)]
        public static void GetShieldPasswordExpirationDays()
        {
            var pageState = new StateContainer(HttpContext.Current.Session);

            // when the user is a SSO user that allows username/password, go get the days left until the password expires if it isn't already in session
            if (pageState.GetBooleanOrFalse("GetSsoUserPasswordExpirationDays") && string.IsNullOrWhiteSpace(pageState.GetStringOrEmpty(Constants.SessionVariables.DaysLeftBeforePasswordExpires)))
            {
                pageState[Constants.SessionVariables.DaysLeftBeforePasswordExpires] = Sso.GetShieldPasswordExpirationDays(pageState.GetStringOrEmpty("SessionShieldUserName"));
            }
        }

        [WebMethod(EnableSession = true)]
        public static void AuditReportPdfExport(string extensionInfo)
        {
            var pageState = new StateContainer(HttpContext.Current.Session);            
            var auditInfo = CreateReportAuditObject(extensionInfo.DecodeHtml(), AuditAction.REPORT_TO_PDF, pageState);
            new AuditLog().AddReportEntry(auditInfo);
        }

        [WebMethod(EnableSession = true)]
        public static void AuditReportWordExport(string extensionInfo)
        {
            var pageState = new StateContainer(HttpContext.Current.Session);            
            var auditInfo = CreateReportAuditObject(extensionInfo.DecodeHtml(), AuditAction.REPORT_TO_WORD, pageState);
            new AuditLog().AddReportEntry(auditInfo);
        }

        [WebMethod(EnableSession = true)]
        public static void AuditReportPrinted(string extensionInfo)
        {
            var pageState = new StateContainer(HttpContext.Current.Session);            
            var auditInfo = CreateReportAuditObject(extensionInfo.DecodeHtml(), AuditAction.REPORT_PRINTED, pageState);
            new AuditLog().AddReportEntry(auditInfo);
        }

        [WebMethod(EnableSession = true)]
        public static void AuditReportExcelExport(string extensionInfo)
        {
            var pageState = new StateContainer(HttpContext.Current.Session);            
            var auditInfo = CreateReportAuditObject(extensionInfo.DecodeHtml(), AuditAction.REPORT_TO_EXCEL, pageState);
            new AuditLog().AddReportEntry(auditInfo);
        }
                
        private static ReportAuditExtendedInfo CreateReportAuditObject(string extensionInfo, AuditAction action, StateContainer pageState)
        {
            var auditInfo = new ReportAuditExtendedInfo
            {
                Action = action,
                DbId = (ConnectionStringPointer) pageState[Constants.SessionVariables.DbId],
                ExtendedInfo = extensionInfo.Replace(AuditAction.REPORT_VIEWED.ToString(), action.ToString()),
                IpAddress = HttpContext.Current.Request.UserIpAddress(), 
                LicenseId = pageState.GetGuidOr0x0(Constants.SessionVariables.LicenseId),
                UserId = pageState.GetGuidOr0x0(Constants.SessionVariables.UserId)
            };
            return auditInfo;
        }
    }
}