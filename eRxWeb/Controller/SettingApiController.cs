using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Http;
using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.Data.PDMP;
using Allscripts.ePrescribe.ExtensionMethods;
using Allscripts.ePrescribe.Objects;
using Allscripts.ePrescribe.Shared.Logging;
using Allscripts.Impact;
using eRxWeb.AppCode;
using eRxWeb.AppCode.PdmpBPL;
using eRxWeb.ServerModel;
using eRxWeb.State;
using ConfigKeys = Allscripts.Impact.ConfigKeys;
using UserInfo = eRxWeb.AppCode.StateUtils.UserInfo;

namespace eRxWeb.Controller
{
    public class SettingsApiController : ApiController
    {
        private static readonly ILoggerEx logger = LoggerEx.GetLogger();
        [HttpPost]
        public ApiResponse RetrieveLinks()
        {
            IStateContainer session = new StateContainer(HttpContext.Current.Session);
            var response = new ApiResponse();
            try
            {
                using (var timer = logger.StartTimer("GetRobustLinks"))
                {
                    var ssoMode = session.GetStringOrEmpty(Constants.SessionVariables.SSOMode);
                    var sessionLicense = UserInfo.GetSessionLicense(session);
                    var enterpriseEpcs = Converter.ToEnabledDisabled(session.GetBooleanOrFalse(Constants.SessionVariables.IsEnterpriseEpcsEnabled));
                    var licenseEpcs = Converter.ToEnabledDisabled(EPCSWorkflowUtils.IsLicenseEpcsPurchased(sessionLicense));
                    var prebuildPrescriptions = Converter.ToVisibility(session.GetBooleanOrFalse(Constants.SessionVariables.ShowPrebuiltPrescriptions));

                    var links = new List<SettingsLink>();
                    links.AddNotNull(GenerateAddUserLink(Converter.ToEnabledDisabled(sessionLicense.EnterpriseClient.AddUser)));
                    links.AddNotNull(GenerateEditUserLink());
                    links.AddNotNull(GenerateDurSettingsLink(Converter.ToVisibility(sessionLicense.EnterpriseClient.ShowDURSettings)));
                    links.AddNotNull(GenerateSiteManagementLink());
                    links.AddNotNull(GenerateMergePatientsLink(Converter.ToEnabledDisabled(sessionLicense.EnterpriseClient.MergePatients)));
                    links.AddNotNull(GenerateManageEpcsLink(enterpriseEpcs, licenseEpcs));
                    links.AddNotNull(GenerateViewEpcsDroiLinks(enterpriseEpcs, licenseEpcs));
                    links.AddNotNull(GeneratePrivacyLink(ssoMode));
                    links.AddNotNull(GeneratePreBuildRxLink(sessionLicense, prebuildPrescriptions, Converter.ToEnabledDisabled(sessionLicense.EnterpriseClient.EnablePrebuiltPrescriptions)));
                    links.AddNotNull(GeneratePdmpEnrollmentLink(session, new PdmpData()));
                    links.AddNotNull(GeneratePatientUpload(session));
                    links.AddNotNull(GenerateChartExtractLink(session));

                    response.Payload = links;
                    timer.Message = links.ToLogString();
                }
            }
            catch (Exception ex)
            {
                var errorMessage = ApiHelper.AuditException(ex.ToString(), session);
                logger.Error("RetrieveLinks Exception: " + ex.ToString());

                response.ErrorContext = new ErrorContextModel()
                {
                    Error = ErrorTypeEnum.ServerError,
                    Message = errorMessage
                };
            }
            return response;
        }

        public static SettingsLink GeneratePreBuildRxLink(ApplicationLicense sessionLicense, Visibility prebuiltPrescriptions, EnabledDisabled enterprisePrebuiltPrescriptions)
        {
            SettingsLink link = null;

            if (enterprisePrebuiltPrescriptions == EnabledDisabled.Enabled &&
                prebuiltPrescriptions == Visibility.Visible &&
                (sessionLicense.LicenseDeluxeStatus == Constants.DeluxeFeatureStatus.On ||
                 sessionLicense.LicenseDeluxeStatus == Constants.DeluxeFeatureStatus.AlwaysOn ||
                 sessionLicense.LicenseDeluxeStatus == Constants.DeluxeFeatureStatus.Cancelled))
            {
                link = new SettingsLink("Manage Pre-Built Prescriptions", Constants.PageNames.PRE_BUILT_PRESCRIPTIONS);
            }

            return link;
        }

        public static SettingsLink GenerateViewEpcsDroiLinks(EnabledDisabled enterpriseEpcs, EnabledDisabled licenseEpcs)
        {
            SettingsLink link = null;

            if (enterpriseEpcs == EnabledDisabled.Enabled || licenseEpcs == EnabledDisabled.Enabled)
            {
                link = new SettingsLink("View EPCS Daily Reports Of Incidents", ConfigKeys.EpcsDroiURL, LinkLaunchType.NewWindow);
            }

            return link;
        }

        public static SettingsLink GenerateManageEpcsLink(EnabledDisabled enterpriseEpcs, EnabledDisabled licenseEpcs)
        {
            SettingsLink link = null;

            if (enterpriseEpcs == EnabledDisabled.Enabled || licenseEpcs == EnabledDisabled.Enabled)
            {
                link = new SettingsLink("Manage EPCS", Constants.PageNames.EPCS_REGISTRATION + "?From=" + AngularStringUtil.CreateUrl(Constants.PageNames.SETTINGS));
            }

            return link;
        }

        public static SettingsLink GenerateMergePatientsLink(EnabledDisabled mergePatients)
        {
            SettingsLink link = null;

            if (mergePatients == EnabledDisabled.Enabled)
            {
                link = new SettingsLink("Merge Patients", Constants.PageNames.MERGE_PATIENTS);
            }

            return link;
        }

        public static SettingsLink GenerateSiteManagementLink()
        {
            return new SettingsLink("Site Management", Constants.PageNames.SITE_MANAGEMENT);
        }

        public static SettingsLink GenerateDurSettingsLink(Visibility durSettings)
        {
            SettingsLink link = null;

            if (durSettings == Visibility.Visible)
            {
                link = new SettingsLink("DUR Settings", Constants.PageNames.USER_PREFERENCES);
            }

            return link;
        }

        public static SettingsLink GenerateEditUserLink()
        {
            return new SettingsLink("Edit Users", Constants.PageNames.EDIT_USERS);
        }

        public static SettingsLink GenerateAddUserLink(EnabledDisabled addUser)
        {
            SettingsLink link = null;

            if (addUser == EnabledDisabled.Enabled)
            {
                link = new SettingsLink("Add User", $"{Constants.PageNames.EDIT_USER}?Mode=Add");
            }

            return link;
        }

        public static SettingsLink GeneratePrivacyLink(string ssoMode)
        {
            SettingsLink link = null;

            if (ssoMode != Constants.SSOMode.UTILITYMODE)
            {
                link = new SettingsLink("Privacy", Constants.PageNames.PRIVACY_PATIENT_SEARCH);
            }

            return link;
        }

        public static SettingsLink GeneratePdmpEnrollmentLink(IStateContainer session, IPdmpData pdmpData)
        {
            SettingsLink link = null;
            if (PDMP.IsPdmpEnabledForEnterprise(session) && PDMP.IsLicenseDefaultSiteStatePdmpEnabled(session, pdmpData))
            {
                bool isPdmpEnabledForAccount = PDMP.IsPdmpEnabledForAccount(session);

                if (isPdmpEnabledForAccount)
                {
                    link = new SettingsLink("PDMP Enrollment Form", $"{Constants.UIModals.PDMP_ENROLLMENT}", LinkLaunchType.Modal);
                }
                else
                {
                    link = new SettingsLink("PDMP Enrollment Form", $"{Constants.UIModals.PDMP_EPCS_TEASER}", LinkLaunchType.Modal);
                }
            }
            return link;
        }
        
        public static SettingsLink GeneratePatientUpload (IStateContainer session)
        {
            SettingsLink link = null;
            if (PatientFileUpload.IsShowPatientUpload(session))
            {
                return new SettingsLink("Patient Upload", Constants.PageNames.PATIENT_UPLOAD);
            }
            return link;
        }

        public static SettingsLink GenerateChartExtractLink (IStateContainer session)
        {
            SettingsLink link = null;
            if (ChartExtract.IsShowChartExtract(session))
            {
                return new SettingsLink("Chart Extract Download", Constants.PageNames.CHART_EXTRACT_DOWNLOAD);
            }
            return link;
        }
    }
}