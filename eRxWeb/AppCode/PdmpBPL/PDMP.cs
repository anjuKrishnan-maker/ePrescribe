using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.Objects.CommonComponent;
using Allscripts.ePrescribe.Objects.PDMP;
using Allscripts.ePrescribe.Shared.Logging;
using Allscripts.Impact;
using Allscripts.Impact.PdmpBPL;
using GetPDMPSamlTokenResponse = eRxWeb.ePrescribeSvc.GetPDMPSamlTokenResponse;
using eRxWeb.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Allscripts.ePrescribe.DatabaseSelector;
using static Allscripts.ePrescribe.Objects.PDMP.PdmpConstants;
using Allscripts.ePrescribe.Data.CommonComponent;
using Allscripts.ePrescribe.Data.PDMP;

namespace eRxWeb.AppCode.PdmpBPL
{
    public class PDMP
    {
        private static readonly ILoggerEx logger = LoggerEx.GetLogger();

        public static bool IsShowPDMP(IStateContainer session)
        {
            var sessionLicense = session.Cast(Constants.SessionVariables.SessionLicense, 
                                              default(ApplicationLicense));

            bool isPdmpEnabledForAccount = IsPdmpEnabledForAccount(session);

            bool licensePreferecnce = session.GetBooleanOrFalse(Constants.LicensePreferences.SHOW_PDMP);
            var userType = session.Cast(Constants.SessionVariables.UserType, 
                                        Constants.UserCategory.GENERAL_USER);

            bool isNpiUser = (userType == Constants.UserCategory.PROVIDER
                        || userType == Constants.UserCategory.PHYSICIAN_ASSISTANT
                        || userType == Constants.UserCategory.PHYSICIAN_ASSISTANT_SUPERVISED);

            return (isPdmpEnabledForAccount
                    && licensePreferecnce
                    && isNpiUser);
        }

        public static bool IsPdmpEnabledForAccount(IStateContainer session)
        {
            var sessionLicense = session.Cast(Constants.SessionVariables.SessionLicense,
                                             default(ApplicationLicense));

            bool isEnterpriseEpcsApplyToLicense = EPCSWorkflowUtils.IsEnterpriseEpcsLicense(sessionLicense,
                        session.GetBooleanOrFalse(Constants.SessionVariables.IsEnterpriseEpcsEnabled));
            bool isLicenseEPCSPurchased = EPCSWorkflowUtils.IsLicenseEpcsPurchased(sessionLicense);

            return (sessionLicense.EnterpriseClient.ShowPDMP 
                    && (isEnterpriseEpcsApplyToLicense 
                        || isLicenseEPCSPurchased));
        }

        public static bool IsLicenseDefaultSiteStatePdmpEnabled(IStateContainer session, IPdmpData pdmpData)
        {
            var licenseId = session.GetGuidOr0x0(Constants.SessionVariables.LicenseId).ToString();
            var dbId = session.Cast(Constants.SessionVariables.DbId, default(ConnectionStringPointer));

            return pdmpData.IsLicenseDefaultSiteStatePdmpEnabled(licenseId, dbId);
        }

        public static bool IsPdmpEnabledForEnterprise(IStateContainer session)
        {
            var sessionLicense = session.Cast(Constants.SessionVariables.SessionLicense,
                                             default(ApplicationLicense));
            return sessionLicense.EnterpriseClient.ShowPDMP;
        }

        public static PdmpUISummary GetPdmpSummary(IStateContainer session, IPdmp pdmpBpl, 
            IPdmpServiceBroker pdmpServiceBroker, ICommonComponentData commonComponentData)
        {
            PdmpInfo pdmpInfo = new PdmpInfo();
            PdmpUISummary pdmpUISummary = new PdmpUISummary();
            pdmpUISummary.SummaryStatus = PdmpPatientResponseStatus.Unknown;

            session.Remove(Constants.SessionVariables.PdmpInfo);
            var ipAddress = HttpContext.Current?.Request.UserIpAddress();
            var dbId = session.Cast(Constants.SessionVariables.DbId, default(ConnectionStringPointer));
            var requestInfo = GetPdmpRequestInfo(session);
            var pdmpToken = GetPDMPSamlToken(session);
            var userId = session.GetGuidOr0x0(Constants.SessionVariables.UserId).ToString();
            var licenseId = session.GetGuidOr0x0(Constants.SessionVariables.LicenseId).ToString();
            if (string.IsNullOrWhiteSpace(pdmpToken?.Base64SamlToken))
            {
                logger.Debug("GetPdmpSummary: Shield PPT Token is null");
                return pdmpUISummary;
            }

            if (requestInfo == null)
            {
                logger.Debug("GetPdmpSummary: Insufficient request Information");
                return pdmpUISummary;
            }

            string fhirRequest = string.Empty;
            using (var timer = logger.StartTimer("ConstructPdmpPatientRequestAndGetAck"))
            {
                fhirRequest = pdmpBpl.CreatePatientRetrieveRequestFhir(new PdmpBundler(requestInfo), userId, licenseId, dbId);
                timer.Message = $"<request>{fhirRequest}</request>";
                if (!string.IsNullOrWhiteSpace(fhirRequest))
                {
                    pdmpInfo.PatientRetrieveResponse = pdmpServiceBroker.PatientRetrieveResponse(fhirRequest, pdmpToken.Base64SamlToken);
                }
                timer.Message += $"<response>{pdmpInfo.PatientRetrieveResponse}</response>";
            }
            pdmpBpl.ParsePatientRetrieveResponse(pdmpInfo, userId, licenseId, dbId);
            session[Constants.SessionVariables.PdmpInfo] = pdmpInfo;

            if (!string.IsNullOrWhiteSpace(pdmpInfo.TransactionId))
            {
                commonComponentData.InsertTransactionHeader(pdmpInfo.TransactionId, string.Empty, Convert.ToInt32(Constants.CommonApiRequestTypes.PDMP),
                    userId.ToString(), requestInfo.PatientInfo.Id.ToString(), dbId);
                commonComponentData.InsertTransactionDetail(fhirRequest, pdmpInfo.PatientRetrieveResponse, pdmpInfo.TransactionId, 
                    Convert.ToInt32(Constants.CommonApiTransactionTypes.PDMP_PatientRetrieve), dbId);
            }
             
            pdmpUISummary.TransactionId = pdmpInfo.TransactionId;
            pdmpUISummary.SummaryStatus = pdmpInfo.PatientRetrieveResponseStatus;
            pdmpUISummary.SummaryHtml = pdmpInfo.SummaryHtml;

            return pdmpUISummary;
        }

        public static string GetPdmpReportRequest(IStateContainer session, IPdmp pdmpBpl)
        {
            var dbId = session.Cast(Constants.SessionVariables.DbId, default(ConnectionStringPointer));
            var requestInfo = GetPdmpRequestInfo(session);
            var userId = session.GetGuidOr0x0(Constants.SessionVariables.UserId).ToString();
            var licenseId = session.GetGuidOr0x0(Constants.SessionVariables.LicenseId).ToString();
            var pdmpInfo = session.Cast(Constants.SessionVariables.PdmpInfo, default(PdmpInfo));
            string fhirRequest = string.Empty;
            fhirRequest = pdmpBpl.CreatePatientReportRetrieveRequestFhir(new PdmpBundler(requestInfo), 
                pdmpInfo.TransactionId, userId, licenseId, dbId);
            return fhirRequest;
        }
        public static void SetPDMPToken(IStateContainer session)
        {
            if (IsShowPDMP(session))
            {
                string shieldIdentityToken = session.GetStringOrEmpty(Constants.SessionVariables.ShieldIdentityToken);
                string shieldExteralTenantId = session.GetStringOrEmpty(Constants.SessionVariables.ShieldExternalTenantID);

                if (string.IsNullOrWhiteSpace(shieldIdentityToken) || string.IsNullOrWhiteSpace(shieldExteralTenantId))
                {
                    logger.Debug("SetPDMPToken: Could not able to set PDMP Security Token");
                    return;
                }

                var pdmpTokenObject = EPSBroker.GetPDMPSamlToken(shieldIdentityToken,
                                            shieldExteralTenantId);
                session[Constants.SessionVariables.pdmpTokenObject] = pdmpTokenObject;

                logger.Debug("SetPDMPToken: PDMP Security Token is set");
            }
        }

        public static GetPDMPSamlTokenResponse GetPDMPSamlToken(IStateContainer session)
        {
            var pdmpToken = session.Cast(Constants.SessionVariables.pdmpTokenObject, default(GetPDMPSamlTokenResponse));

            if (!string.IsNullOrWhiteSpace(pdmpToken?.Base64SamlToken))
            {
                if (DateTime.UtcNow > pdmpToken.TokenRefreshTime)
                {
                    pdmpToken = EPSBroker.RefreshPDMPSamlToken(pdmpToken.RawSamlToken, session.GetStringOrEmpty(Constants.SessionVariables.ShieldExternalTenantID));
                    session[Constants.SessionVariables.pptPlusTokenObject] = pdmpToken;
                }
            }
            else
            {
                SetPDMPToken(session);
                pdmpToken = session.Cast(Constants.SessionVariables.pdmpTokenObject, default(GetPDMPSamlTokenResponse));
            }
            return pdmpToken;
        }

        private static PdmpRequestInfo GetPdmpRequestInfo(IStateContainer session)
        {
            var pdmpReqInfo = new PdmpRequestInfo
            {
                SiteInfo = session.Cast(Constants.SessionVariables.CommonCompSiteInfo, default(SiteInfo)),
                ProviderInfo = session.Cast(Constants.SessionVariables.CommonCompProviderInfo, default(ProviderInfo)),
                PatientInfo = session.Cast(Constants.SessionVariables.CommonCompPatientInfo, default(PatientInfo)),
                IpAddress = HttpContext.Current.Request.UserIpAddress(), 
                eRxAppVersion = session.Cast(Constants.SessionVariables.eRxAppVersion, default(string))
            };

            return pdmpReqInfo;
        }

        public static void PDMPDetailsButtonHandler(string userEventName, IStateContainer session)
        {
            var pdmpInfo = session.Cast(Constants.SessionVariables.PdmpInfo, default(PdmpInfo));
            var dbId = session.Cast(Constants.SessionVariables.DbId, default(ConnectionStringPointer));

            if (userEventName == "Pdmp-Confirmed")
            {
                pdmpInfo.IsStateRegistryChecked = true;
            }
            long commonUIRequestId = session.GetLong(Constants.SessionVariables.PdmpCommonUiRequestId, -1);
            if (commonUIRequestId != -1 && !string.IsNullOrEmpty(userEventName))
            {
                CommonComponentData commonComponentData = new CommonComponentData();
                commonComponentData.InsertCommonUIEventData(commonUIRequestId, userEventName, dbId);
            }
        }
        public static bool GetStateRegistryCheckedState(IStateContainer session, out bool checkedValue)
        {
            var pdmpInfo = session.Cast(Constants.SessionVariables.PdmpInfo, default(PdmpInfo));
            checkedValue = false;
            if (pdmpInfo != default(PdmpInfo))
            {
                checkedValue = pdmpInfo.IsStateRegistryChecked;
                return true;
            }
            return false;
        }

        public static void InsertReportViewStatus(IStateContainer session, IPdmpData pdmpData)
        {
            var dbId = session.Cast(Constants.SessionVariables.DbId, default(ConnectionStringPointer));
            var pdmpInfo = session.Cast(Constants.SessionVariables.PdmpInfo, default(PdmpInfo));
            if (pdmpInfo != default(PdmpInfo))
            {
                pdmpData.InsertReportViewStatus(pdmpInfo.TransactionId, dbId);
            }
        }
    }
}