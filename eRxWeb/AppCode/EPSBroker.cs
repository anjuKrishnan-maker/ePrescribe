using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.Caching;
using System.Web.Services.Description;
using Allscripts.ePrescribe.Objects;
using Allscripts.ePrescribe.Shared.Logging;
using Allscripts.Impact.Utilities;
using Allscripts.ePrescribe.Common;
using System.Web.Services.Protocols;
using System.Web.SessionState;
using Allscripts.ePrescribe.Shared.Data;
using eRxWeb.AppCode;
using eRxWeb.AppCode.SpecialtyMedWorkflow;
using eRxWeb.AppCode.Interfaces;
using eRxWeb.ePrescribeSvc;
using eRxWeb.State;
using Org.BouncyCastle.Asn1.X509.Qualified;
using Org.BouncyCastle.Ocsp;
using Org.BouncyCastle.Utilities.Net;
using AuditAction = eRxWeb.ePrescribeSvc.AuditAction;
using AuditInfo = eRxWeb.ePrescribeSvc.AuditInfo;
using ConnectionStringPointer = Allscripts.ePrescribe.DatabaseSelector.ConnectionStringPointer;
using LoggingInfo = eRxWeb.ePrescribeSvc.LoggingInfo;
using OTPForm = eRxWeb.ePrescribeSvc.OTPForm;
using SecretQuestion = eRxWeb.ePrescribeSvc.SecretQuestion;
using ShieldTraitInfo = eRxWeb.ePrescribeSvc.ShieldTraitInfo;
using ShieldTraitName = eRxWeb.ePrescribeSvc.ShieldTraitName;
using UserNameWithUserGuidPair = eRxWeb.ePrescribeSvc.UserNameWithUserGuidPair;
using Allscripts.Impact.Ilearn;
using System.Data;
using ActivateAndUpdateUserRequest = eRxWeb.ePrescribeSvc.ActivateAndUpdateUserRequest;
using ActivateAndUpdateUserResponse = eRxWeb.ePrescribeSvc.ActivateAndUpdateUserResponse;
using ACRSubmitPixFeedRequest = eRxWeb.ePrescribeSvc.ACRSubmitPixFeedRequest;
using AddUserToUMPRequest = eRxWeb.ePrescribeSvc.AddUserToUMPRequest;
using ApplicationEnum = eRxWeb.ePrescribeSvc.ApplicationEnum;
using AuditLicenseInsertRequest = eRxWeb.ePrescribeSvc.AuditLicenseInsertRequest;
using AuditLogPatientRxCSResponse = eRxWeb.ePrescribeSvc.AuditLogPatientRxCSResponse;
using AuditLogPatientRxResponse = eRxWeb.ePrescribeSvc.AuditLogPatientRxResponse;
using AuditPatientInsertRequest = eRxWeb.ePrescribeSvc.AuditPatientInsertRequest;
using AuditPatientRxCSInsertRequest = eRxWeb.ePrescribeSvc.AuditPatientRxCSInsertRequest;
using AuditPatientRxInsertRequest = eRxWeb.ePrescribeSvc.AuditPatientRxInsertRequest;
using AuditShieldInsertRequest = eRxWeb.ePrescribeSvc.AuditShieldInsertRequest;
using AuditUserInsertRequest = eRxWeb.ePrescribeSvc.AuditUserInsertRequest;
using AuthenticateAndAuthorizeUserRequest = eRxWeb.ePrescribeSvc.AuthenticateAndAuthorizeUserRequest;
using AuthenticateAndAuthorizeUserResponse = eRxWeb.ePrescribeSvc.AuthenticateAndAuthorizeUserResponse;
using AuthenticateOtpFormsForSigningResponse = eRxWeb.ePrescribeSvc.AuthenticateOtpFormsForSigningResponse;
using ChangePasswordRequest = eRxWeb.ePrescribeSvc.ChangePasswordRequest;
using ClinicalViewerLaunchRequest = eRxWeb.ePrescribeSvc.ClinicalViewerLaunchRequest;
using ClinicalViewerLaunchResponse = eRxWeb.ePrescribeSvc.ClinicalViewerLaunchResponse;
using CreateBackdoorUserAuditLogEntryRequest = eRxWeb.ePrescribeSvc.CreateBackdoorUserAuditLogEntryRequest;
using ConvertFromHtmlToPdfRequest = eRxWeb.ePrescribeSvc.ConvertFromHtmlToPdfRequest;
using ConvertFromHtmlToPdfResponse = eRxWeb.ePrescribeSvc.ConvertFromHtmlToPdfResponse;
using CreateShieldTenantRequest = eRxWeb.ePrescribeSvc.CreateShieldTenantRequest;
using CreateShieldTenantResponse = eRxWeb.ePrescribeSvc.CreateShieldTenantResponse;
using CreateUserRequest = eRxWeb.ePrescribeSvc.CreateUserRequest;
using CreateUserResponse = eRxWeb.ePrescribeSvc.CreateUserResponse;
using DeleteScriptMessagesRequest = eRxWeb.ePrescribeSvc.DeleteScriptMessagesRequest;
using DXCConfirmOfferCombinedRequest = eRxWeb.ePrescribeSvc.DXCConfirmOfferCombinedRequest;
using DXCConfirmOfferRequest = eRxWeb.ePrescribeSvc.DXCConfirmOfferRequest;
using DXCGetStatusUpdateByActivityIDRequest = eRxWeb.ePrescribeSvc.DXCGetStatusUpdateByActivityIDRequest;
using DXCPatientQueryResultRequest = eRxWeb.ePrescribeSvc.DXCPatientQueryResultRequest;
using DXCProcessInitiationRequest = eRxWeb.ePrescribeSvc.DXCProcessInitiationRequest;
using DXCRecommendationContextRequest = eRxWeb.ePrescribeSvc.DXCRecommendationContextRequest;
using ePANCPDPTaskType = eRxWeb.ePrescribeSvc.ePANCPDPTaskType;
using ePARequestType = eRxWeb.ePrescribeSvc.ePARequestType;
using ePATaskStatus = eRxWeb.ePrescribeSvc.ePATaskStatus;
using ePATaskType = eRxWeb.ePrescribeSvc.ePATaskType;
using ePATransPriority = eRxWeb.ePrescribeSvc.ePATransPriority;
using ePrescribeApplication = eRxWeb.ePrescribeSvc.ePrescribeApplication;
using ePrescribeSvcRequest = eRxWeb.ePrescribeSvc.ePrescribeSvcRequest;
using ePrescribeSvcResponse = eRxWeb.ePrescribeSvc.ePrescribeSvcResponse;
using ECouponRefillPrintResponse = eRxWeb.ePrescribeSvc.ECouponRefillPrintResponse;
using ECouponRefillRequest = eRxWeb.ePrescribeSvc.ECouponRefillRequest;
using ECouponRequest = eRxWeb.ePrescribeSvc.ECouponRequest;
using EnrollProviderRequest = eRxWeb.ePrescribeSvc.EnrollProviderRequest;
using EnrollProviderResponse = eRxWeb.ePrescribeSvc.EnrollProviderResponse;
using EPAMsgRequest = eRxWeb.ePrescribeSvc.EPAMsgRequest;
using EPARequest = eRxWeb.ePrescribeSvc.EPARequest;
using EPARequestsAdditionalInfoRequest = eRxWeb.ePrescribeSvc.EPARequestsAdditionalInfoRequest;
using EPAServiceResponse = eRxWeb.ePrescribeSvc.EPAServiceResponse;
using EPATaskRequest = eRxWeb.ePrescribeSvc.EPATaskRequest;
using EPAType = eRxWeb.ePrescribeSvc.EPAType;
using FilterTypeEnum = eRxWeb.ePrescribeSvc.FilterTypeEnum;
using GetAllProvidersAndTraitsForLicenseRequest = eRxWeb.ePrescribeSvc.GetAllProvidersAndTraitsForLicenseRequest;
using GetAllSecretQuestionsResponse = eRxWeb.ePrescribeSvc.GetAllSecretQuestionsResponse;
using GetBackdoorSitesResponse = eRxWeb.ePrescribeSvc.GetBackdoorSitesResponse;
using GetePAStatusRequest = eRxWeb.ePrescribeSvc.GetePAStatusRequest;
using GetECouponDetailByRxIDResponse = eRxWeb.ePrescribeSvc.GetECouponDetailByRxIDResponse;
using GetECouponDetailsLiteByIDResponse = eRxWeb.ePrescribeSvc.GetECouponDetailsLiteByIDResponse;
using GetEligAndMedHxStatusRequest = eRxWeb.ePrescribeSvc.GetEligAndMedHxStatusRequest;
using GetEnterpriseePAStatusRequest = eRxWeb.ePrescribeSvc.GetEnterpriseePAStatusRequest;
using GetIdProofingUrlRequest = eRxWeb.ePrescribeSvc.GetIdProofingUrlRequest;
using GetUserShieldCspStatusInfoRequest = eRxWeb.ePrescribeSvc.GetUserShieldCspStatusInfoRequest;
using GetNewActivationCodeResponse = eRxWeb.ePrescribeSvc.GetNewActivationCodeResponse;
using GetMatchingPharmacyRequest = eRxWeb.ePrescribeSvc.GetMatchingPharmacyRequest;
using GetMatchingPharmacyResponse = eRxWeb.ePrescribeSvc.GetMatchingPharmacyResponse;
using GetNewPasswordRequest = eRxWeb.ePrescribeSvc.GetNewPasswordRequest;
using GetNewPasswordResponse = eRxWeb.ePrescribeSvc.GetNewPasswordResponse;
using GetNewUniqueUserNameRequest = eRxWeb.ePrescribeSvc.GetNewUniqueUserNameRequest;
using GetRxInfoDetailsLiteByRxIDRequest = eRxWeb.ePrescribeSvc.GetRxInfoDetailsLiteByRxIDRequest;
using GetRxInfoDetailsLiteByRxIDResponse = eRxWeb.ePrescribeSvc.GetRxInfoDetailsLiteByRxIDResponse;
using GetRxInfoHtmlContentRequest = eRxWeb.ePrescribeSvc.GetRxInfoHtmlContentRequest;
using GetShieldAuthenicateOTPFormsResponse = eRxWeb.ePrescribeSvc.GetShieldAuthenicateOTPFormsResponse;
using GetShieldAuthenicateOTPRequest = eRxWeb.ePrescribeSvc.GetShieldAuthenicateOTPRequest;
using GetShieldInternalAppInstanceIDRequest = eRxWeb.ePrescribeSvc.GetShieldInternalAppInstanceIDRequest;
using GetShieldInternalEnvironmentIDRequest = eRxWeb.ePrescribeSvc.GetShieldInternalEnvironmentIDRequest;
using GetShieldInternalTenantIDRequest = eRxWeb.ePrescribeSvc.GetShieldInternalTenantIDRequest;
using GetShieldOTPFormsRequest = eRxWeb.ePrescribeSvc.GetShieldOTPFormsRequest;
using GetShieldOTPFormsResponse = eRxWeb.ePrescribeSvc.GetShieldOTPFormsResponse;
using GetShieldSendOTPRequest = eRxWeb.ePrescribeSvc.GetShieldSendOTPRequest;
using GetShieldSendOTPResponse = eRxWeb.ePrescribeSvc.GetShieldSendOTPResponse;
using GetShieldUserTraitsRequest = eRxWeb.ePrescribeSvc.GetShieldUserTraitsRequest;
using GetSpecialtyMedRequest = eRxWeb.ePrescribeSvc.GetSpecialtyMedRequest;
using GetUserRequest = eRxWeb.ePrescribeSvc.GetUserRequest;
using GetUserResponse = eRxWeb.ePrescribeSvc.GetUserResponse;
using GetUsersFromePrescribeRequest = eRxWeb.ePrescribeSvc.GetUsersFromePrescribeRequest;
using GetUsersFromePrescribeResponse = eRxWeb.ePrescribeSvc.GetUsersFromePrescribeResponse;
using GetUsersRequest = eRxWeb.ePrescribeSvc.GetUsersRequest;
using GetUsersResponse = eRxWeb.ePrescribeSvc.GetUsersResponse;
using SearchShieldUsersRequest = eRxWeb.ePrescribeSvc.SearchShieldUsersRequest;
using GetUserSecretQuestionsRequest = eRxWeb.ePrescribeSvc.GetUserSecretQuestionsRequest;
using HubServiceLevel = eRxWeb.ePrescribeSvc.HubServiceLevel;
using InsertAllUsersInLicenseForHubEnrollmentRequest = eRxWeb.ePrescribeSvc.InsertAllUsersInLicenseForHubEnrollmentRequest;
using IsValidPasswordRequest = eRxWeb.ePrescribeSvc.IsValidPasswordRequest;
using IsValidPasswordResponse = eRxWeb.ePrescribeSvc.IsValidPasswordResponse;
using LicenseRequest = eRxWeb.ePrescribeSvc.LicenseRequest;
using LicenseSiteRequest = eRxWeb.ePrescribeSvc.LicenseSiteRequest;
using LinkAndAuthorizedFederatedUserRequest = eRxWeb.ePrescribeSvc.LinkAndAuthorizedFederatedUserRequest;
using LinkExistingShieldUserToeRxUserRequest = eRxWeb.ePrescribeSvc.LinkExistingShieldUserToeRxUserRequest;
using LinkExistingShieldUserToeRxUserResponse = eRxWeb.ePrescribeSvc.LinkExistingShieldUserToeRxUserResponse;
using LoadMedicationRequest = eRxWeb.ePrescribeSvc.LoadMedicationRequest;
using LoadMedicationResponse = eRxWeb.ePrescribeSvc.LoadMedicationResponse;
using LoadMessageQueueTxRequest = eRxWeb.ePrescribeSvc.LoadMessageQueueTxRequest;
using LoadMessageQueueTxResponse = eRxWeb.ePrescribeSvc.LoadMessageQueueTxResponse;
using PdfDocumentSettingsType = eRxWeb.ePrescribeSvc.PdfDocumentSettingsType;
using PrescriptionStatus = eRxWeb.ePrescribeSvc.PrescriptionStatus;
using ProviderWithShieldTraits = eRxWeb.ePrescribeSvc.ProviderWithShieldTraits;
using PrintRxRequest = eRxWeb.ePrescribeSvc.PrintRxRequest;
using PrintRxResponse = eRxWeb.ePrescribeSvc.PrintRxResponse;
using RefreshSamlTokenRequest = eRxWeb.ePrescribeSvc.RefreshSamlTokenRequest;
using RefreshSamlTokenResponse = eRxWeb.ePrescribeSvc.RefreshSamlTokenResponse;
using RxInfoRequest = eRxWeb.ePrescribeSvc.RxInfoRequest;
using RxOptions = eRxWeb.ePrescribeSvc.RxOptions;
using RxOptionsUsedRequest = eRxWeb.ePrescribeSvc.RxOptionsUsedRequest;
using RxRequest = eRxWeb.ePrescribeSvc.RxRequest;
using SaveDEALicenseRequest = eRxWeb.ePrescribeSvc.SaveDEALicenseRequest;
using SavePatientRequest = eRxWeb.ePrescribeSvc.SavePatientRequest;
using SaveSecretQuestionAnswersRequest = eRxWeb.ePrescribeSvc.SaveSecretQuestionAnswersRequest;
using SaveSecretQuestionAnswersResponse = eRxWeb.ePrescribeSvc.SaveSecretQuestionAnswersResponse;
using SaveUserAppPropertyRequest = eRxWeb.ePrescribeSvc.SaveUserAppPropertyRequest;
using SearchUsersRequest = eRxWeb.ePrescribeSvc.SearchUsersRequest;
using SecretAnswer = eRxWeb.ePrescribeSvc.SecretAnswer;
using SetDEARegistrantStatusRequest = eRxWeb.ePrescribeSvc.SetDEARegistrantStatusRequest;
using SetProviderTraitsRequest = eRxWeb.ePrescribeSvc.SetProviderTraitsRequest;
using SetShieldUserTraitRequest = eRxWeb.ePrescribeSvc.SetShieldUserTraitRequest;
using ShieldContact = eRxWeb.ePrescribeSvc.ShieldContact;
using ShieldUserStatus = eRxWeb.ePrescribeSvc.ShieldUserStatus;
using SignPrescriptionsRequest = eRxWeb.ePrescribeSvc.SignPrescriptionsRequest;
using SignPrescriptionsResponse = eRxWeb.ePrescribeSvc.SignPrescriptionsResponse;
using SMSAlertsRequest = eRxWeb.ePrescribeSvc.SMSAlertsRequest;
using UpdateSecUsersInfoFromShieldRequest = eRxWeb.ePrescribeSvc.UpdateSecUsersInfoFromShieldRequest;
using UpdateUserInUMPRequest = eRxWeb.ePrescribeSvc.UpdateUserInUMPRequest;
using UpdateUserRequest = eRxWeb.ePrescribeSvc.UpdateUserRequest;
using UserLoggingEnabledRequest = eRxWeb.ePrescribeSvc.UserLoggingEnabledRequest;
using UserCategory = Allscripts.ePrescribe.Common.Constants.UserCategory;
using ResetForgottenPasswordResponse = eRxWeb.ePrescribeSvc.ResetForgottenPasswordResponse;
using ResetForgottenPasswordRequest = eRxWeb.ePrescribeSvc.ResetForgottenPasswordRequest;
using GetStateLicenseAndDeaExpiryDateForUserResponse = eRxWeb.ePrescribeSvc.GetStateLicenseAndDeaExpiryDateForUserResponse;
using GetStateLicenseAndDeaExpiryDateForUserRequest = eRxWeb.ePrescribeSvc.GetStateLicenseAndDeaExpiryDateForUserRequest;
using LicenseCreationDateRequest = eRxWeb.ePrescribeSvc.LicenseCreationDateRequest;
using LicenseCreationDateResponse = eRxWeb.ePrescribeSvc.LicenseCreationDateResponse;
using FilterTypeEnumSearchUsers = eRxWeb.ePrescribeSvc.FilterTypeEnumSearchUsers;
using GetMessageTrackingAcksResponse = eRxWeb.ePrescribeSvc.GetMessageTrackingAcksResponse;
using Status = eRxWeb.ePrescribeSvc.Status;
using Allscripts.ePrescribe.Data.Model;
using Allscripts.ePrescribe.Data.Model.PDMP;
using Allscripts.ePrescribe.DatabaseSelector;
using Allscripts.Impact;
using Allscripts.Impact.ePrescribeSvc;
using DeaLicenseType = Allscripts.ePrescribe.Objects.DeaLicenseType;
using Disposition = eRxWeb.ePrescribeSvc.Disposition;
using GetUserShieldCspStatusInfoResponse = eRxWeb.ePrescribeSvc.GetUserShieldCspStatusInfoResponse;
using RtpsCRxDispositionRequest = eRxWeb.ePrescribeSvc.RtpsCRxDispositionRequest;
using UpdateShieldTenantRequest = eRxWeb.ePrescribeSvc.UpdateShieldTenantRequest;
using GetServiceAccountShieldSamlTokenResult = Allscripts.Impact.ePrescribeSvc.GetServiceAccountShieldSamlTokenResult;
using PrescriptionTaskType = Allscripts.Impact.ePrescribeSvc.PrescriptionTaskType;
using SendOutboundInfoScriptMessageCompletedEventHandler = Allscripts.Impact.ePrescribeSvc.SendOutboundInfoScriptMessageCompletedEventHandler;
using RegistrantPractice = Allscripts.ePrescribe.Objects.Registrant.RegistrantPractice;
using RegistrantInfo = Allscripts.ePrescribe.Objects.Registrant.RegistrantInfo;
using Allscripts.ePrescribe.Objects.Registrant;
using eRxWeb.ServerModel;
using static Allscripts.ePrescribe.Common.Constants;
using Microsoft.Web.Services3.Referral;
using CompleteRegistrationRequest = eRxWeb.ePrescribeSvc.CompleteRegistrationRequest;
using CreateRegistrantUserRequest = eRxWeb.ePrescribeSvc.CreateRegistrantUserRequest;
using CreateRegistrantUserResponse = eRxWeb.ePrescribeSvc.CreateRegistrantUserResponse;
using GetRegistrantIdProofingLoaRequest = eRxWeb.ePrescribeSvc.GetRegistrantIdProofingLoaRequest;
using GetRegistrantIdProofingLoaResponse = eRxWeb.ePrescribeSvc.GetRegistrantIdProofingLoaResponse;
using LogRegistrationStepRequest = eRxWeb.ePrescribeSvc.LogRegistrationStepRequest;
using ShieldAuditEventResponse = eRxWeb.ePrescribeSvc.ShieldAuditEventResponse;
using eRxWeb.ServerModel.Registration;

namespace eRxWeb
{
    /// <summary>
    /// Summary description for EPSBroker
    /// </summary>
    public class EPSBroker : IEPSBroker, IRtpsDisposition
    {
        private static IStateContainer _pageState;
        public static IStateContainer PageState
        {
            get { return _pageState ?? (_pageState = new StateContainer(HttpContext.Current.Session)); }
        }

        private static readonly ILoggerEx logger = LoggerEx.GetLogger();

        public static ePrescribeSvc.ePrescribeSvc GetEps()
        {
            ePrescribeSvc.ePrescribeSvc eps = new ePrescribeSvc.ePrescribeSvc();
            eps.Url = appSettings("ePrescribeSvc.ePrescribeSvc");
            return eps;
        }

        public static T ServiceRequest<T>() where T : ePrescribeSvcRequest, new()
        {
            return new T
            {
                LoggingInfo = new LoggingInfo().Init()
            };
        }

        public static void UpdateAsServiceRequest<T>(ref T request) where T: ePrescribeSvcRequest
        {
            request.LoggingInfo = new LoggingInfo().Init();
        }

        public EPSBroker()
        {
        }

        public static GetUserResponse GetRxUser(
            ePrescribeSvc.ValueType valueType,
            string value,
            string licenseID,
            string auditUserID,
            string auditLicenseID,
            ConnectionStringPointer dbID)
        {
            using (var timer = logger.StartTimer("GetRxUser"))
            {
                var getUserReq = ServiceRequest<GetUserRequest>();
                getUserReq.AuditUserID = auditUserID;
                getUserReq.AuditLicenseID = auditLicenseID;
                getUserReq.DbID = (ePrescribeSvc.ConnectionStringPointer)dbID;
                getUserReq.Type = valueType;
                getUserReq.Value = value;

                var response = GetEps().GetUser(getUserReq);

                timer.Message = string.Format("<Request>{0}</Request><Response>{1}</Response>", getUserReq.ToLogString(), response.ToLogString());

                return response;
            }
        }

        public static ePrescribeSvc.RxUser GetRxUserBasic(
            ePrescribeSvc.ValueType valueType,
            string value,
            string auditUserID,
            ConnectionStringPointer dbID)
        {
            using (var timer = logger.StartTimer("GetRxUserBasic"))
            {
                var getUserReq = ServiceRequest<GetUserRequest>();
                getUserReq.AuditUserID = auditUserID;
                getUserReq.DbID = (ePrescribeSvc.ConnectionStringPointer)dbID;
                getUserReq.Type = valueType;
                getUserReq.Value = value;

                GetUserResponse response = GetEps().GetUserBasic(getUserReq);

                timer.Message = string.Format("<Request>{0}</Request><Response>{1}</Response>", getUserReq.ToLogString(), response.ToLogString());


                return response.RxUser;
            }
        }

        public static ePrescribeSvc.License GetLicenseByID(string licenseID)
        {
            using (var timer = logger.StartTimer("GetLicenseByID"))
            {
                ePrescribeSvc.License lic = new ePrescribeSvc.License();
                AuditInfo audit = new AuditInfo();
                audit.LicenseID = lic.LicenseID = licenseID;

                var licReq = ServiceRequest<LicenseRequest>();
                licReq.License = lic;
                licReq.reqAuditInfo = audit;

                var response = GetEps().GetLicenseByID(licReq);
                timer.Message = string.Format("<Request>{0}</Request><Response>{1}</Response>", licReq.ToLogString(), response.ToLogString());

                return response;
            }

        }
        public static ePrescribeSvc.StatusMessageResult GetStatusUpdate(DXCRecommendationContext recommendationContext, ConnectionStringPointer dbID)
        {
            using (var timer = logger.StartTimer("GetStatusUpdate"))
            {
                var requestContext = ServiceRequest<DXCRecommendationContextRequest>();
                if (recommendationContext != null)
                {
                    requestContext.LicenseID = recommendationContext.LicenseID;
                    requestContext.DbID = (ePrescribeSvc.ConnectionStringPointer)recommendationContext.DbID;
                    requestContext.UserID = recommendationContext.UserID;
                    requestContext.PatientMRN = recommendationContext.PatientMRN;
                    requestContext.PatientID = recommendationContext.PatientID;
                    requestContext.PatientDOB = recommendationContext.PatientDOB;
                    requestContext.PatientFirstName = recommendationContext.PatientFirstName;
                    requestContext.PatientLastName = recommendationContext.PatientLastName;
                    requestContext.PatientGender = recommendationContext.PatientGender;
                    requestContext.PatientCity = recommendationContext.PatientCity;
                    requestContext.PatientPostalCode = recommendationContext.PatientPostalCode;
                    requestContext.PatientState = recommendationContext.PatientState;
                    requestContext.PatientStreetAddress1 = recommendationContext.PatientStreetAddress1;
                    requestContext.PatientStreetAddress2 = recommendationContext.PatientStreetAddress2;
                    requestContext.PatientTelephone = recommendationContext.PatientTelephone;
                    requestContext.PatientMobile = recommendationContext.PatientMobile;
                    requestContext.PatientEmail = recommendationContext.PatientEmail;

                    requestContext.ProviderFirstName = recommendationContext.ProviderFirstName;
                    requestContext.ProviderLastName = recommendationContext.ProviderLastName;
                    requestContext.ProviderDegree = recommendationContext.ProviderDegree;
                    requestContext.ProviderID = recommendationContext.ProviderID;
                    requestContext.DEANumber = recommendationContext.DEANumber;

                    requestContext.OrganizationCity = recommendationContext.OrganizationCity;
                    requestContext.OrganizationName = recommendationContext.OrganizationName;
                    requestContext.OrganizationPostalCode = recommendationContext.OrganizationPostalCode;
                    requestContext.OrganizationState = recommendationContext.OrganizationState;
                    requestContext.OrganizationStreetAddress1 = recommendationContext.OrganizationStreetAddress1;
                    requestContext.OrganizationStreetAddress2 = recommendationContext.OrganizationStreetAddress2;
                    requestContext.OrganizationPhone = recommendationContext.OrganizationPhone;
                }

                var response = GetEps().GetStatusUpdate(requestContext);
                timer.Message = string.Format("<Request>{0}</Request><Response>{1}</Response>", requestContext, response.ToLogString());

                return response;
            }
        }

        public static void GetStatusByActivityIdAndUpdateDatabase(DXCRecommendationContext recommendationContext, int activityId, ConnectionStringPointer dbId)
        {
            using (var timer = logger.StartTimer("GetStatusByActivityIdAndUpdateDatabase"))
            {
                var request = ServiceRequest<DXCGetStatusUpdateByActivityIDRequest>();
                if (recommendationContext != null)
                {
                    request.DXCRecommendationContextRequest = new DXCRecommendationContextRequest
                    {
                        LicenseID = recommendationContext.LicenseID,
                        UserID = recommendationContext.UserID,
                        DbID = (ePrescribeSvc.ConnectionStringPointer)recommendationContext.DbID,

                        PatientMRN = recommendationContext.PatientMRN,
                        PatientID = recommendationContext.PatientID,
                        PatientDOB = recommendationContext.PatientDOB,
                        PatientFirstName = recommendationContext.PatientFirstName,
                        PatientLastName = recommendationContext.PatientLastName,
                        PatientGender = recommendationContext.PatientGender,
                        PatientCity = recommendationContext.PatientCity,
                        PatientPostalCode = recommendationContext.PatientPostalCode,
                        PatientState = recommendationContext.PatientState,
                        PatientStreetAddress1 = recommendationContext.PatientStreetAddress1,
                        PatientStreetAddress2 = recommendationContext.PatientStreetAddress2,
                        PatientTelephone = recommendationContext.PatientTelephone,
                        PatientMobile = recommendationContext.PatientMobile,
                        PatientEmail = recommendationContext.PatientEmail,

                        ProviderFirstName = recommendationContext.ProviderFirstName,
                        ProviderLastName = recommendationContext.ProviderLastName,
                        ProviderDegree = recommendationContext.ProviderDegree,
                        ProviderID = recommendationContext.ProviderID,
                        DEANumber = recommendationContext.DEANumber,

                        OrganizationCity = recommendationContext.OrganizationCity,
                        OrganizationName = recommendationContext.OrganizationName,
                        OrganizationPostalCode = recommendationContext.OrganizationPostalCode,
                        OrganizationState = recommendationContext.OrganizationState,
                        OrganizationStreetAddress1 = recommendationContext.OrganizationStreetAddress1,
                        OrganizationStreetAddress2 = recommendationContext.OrganizationStreetAddress2,
                        OrganizationPhone = recommendationContext.OrganizationPhone
                    };
                }
                request.ActivityID = activityId;
                request.DbID = (ePrescribeSvc.ConnectionStringPointer?)dbId;

                GetEps().GetStatusByActivityIdAndUpdateDatabase(request);
                timer.Message = $"<Request>{request.ToLogString()}</Request>";
            }
        }

        public static ePrescribeSvc.InitiationResult ProcessInitiationRequest(DXCRecommendationContext recommendationContext, DXCPatientQueryResult patientData, ConnectionStringPointer dbID)
        {
            using (var timer = logger.StartTimer("ProcessInitiationRequest"))
            {
                var requestProcessInitiation = ServiceRequest<DXCProcessInitiationRequest>();
                requestProcessInitiation.DXCRecommendationContextRequest = new DXCRecommendationContextRequest();
                requestProcessInitiation.DXCPatientQueryResultRequest = new DXCPatientQueryResultRequest();
                if (recommendationContext != null)
                {
                    requestProcessInitiation.DXCRecommendationContextRequest.LicenseID = recommendationContext.LicenseID;
                    requestProcessInitiation.DXCRecommendationContextRequest.UserID = recommendationContext.UserID;
                    requestProcessInitiation.DXCRecommendationContextRequest.DbID = (ePrescribeSvc.ConnectionStringPointer)recommendationContext.DbID;

                    requestProcessInitiation.DXCRecommendationContextRequest.PatientMRN = recommendationContext.PatientMRN;
                    requestProcessInitiation.DXCRecommendationContextRequest.PatientID = recommendationContext.PatientID;
                    requestProcessInitiation.DXCRecommendationContextRequest.PatientDOB = recommendationContext.PatientDOB;
                    requestProcessInitiation.DXCRecommendationContextRequest.PatientFirstName = recommendationContext.PatientFirstName;
                    requestProcessInitiation.DXCRecommendationContextRequest.PatientLastName = recommendationContext.PatientLastName;
                    requestProcessInitiation.DXCRecommendationContextRequest.PatientGender = recommendationContext.PatientGender;
                    requestProcessInitiation.DXCRecommendationContextRequest.PatientCity = recommendationContext.PatientCity;
                    requestProcessInitiation.DXCRecommendationContextRequest.PatientPostalCode = recommendationContext.PatientPostalCode;
                    requestProcessInitiation.DXCRecommendationContextRequest.PatientState = recommendationContext.PatientState;
                    requestProcessInitiation.DXCRecommendationContextRequest.PatientStreetAddress1 = recommendationContext.PatientStreetAddress1;
                    requestProcessInitiation.DXCRecommendationContextRequest.PatientStreetAddress2 = recommendationContext.PatientStreetAddress2;
                    requestProcessInitiation.DXCRecommendationContextRequest.PatientTelephone = recommendationContext.PatientTelephone;
                    requestProcessInitiation.DXCRecommendationContextRequest.PatientMobile = recommendationContext.PatientMobile;
                    requestProcessInitiation.DXCRecommendationContextRequest.PatientEmail = recommendationContext.PatientEmail;

                    requestProcessInitiation.DXCRecommendationContextRequest.ProviderFirstName = recommendationContext.ProviderFirstName;
                    requestProcessInitiation.DXCRecommendationContextRequest.ProviderLastName = recommendationContext.ProviderLastName;
                    requestProcessInitiation.DXCRecommendationContextRequest.ProviderDegree = recommendationContext.ProviderDegree;
                    requestProcessInitiation.DXCRecommendationContextRequest.ProviderID = recommendationContext.ProviderID;
                    requestProcessInitiation.DXCRecommendationContextRequest.DEANumber = recommendationContext.DEANumber;

                    requestProcessInitiation.DXCRecommendationContextRequest.OrganizationCity = recommendationContext.OrganizationCity;
                    requestProcessInitiation.DXCRecommendationContextRequest.OrganizationName = recommendationContext.OrganizationName;
                    requestProcessInitiation.DXCRecommendationContextRequest.OrganizationPostalCode = recommendationContext.OrganizationPostalCode;
                    requestProcessInitiation.DXCRecommendationContextRequest.OrganizationState = recommendationContext.OrganizationState;
                    requestProcessInitiation.DXCRecommendationContextRequest.OrganizationStreetAddress1 = recommendationContext.OrganizationStreetAddress1;
                    requestProcessInitiation.DXCRecommendationContextRequest.OrganizationStreetAddress2 = recommendationContext.OrganizationStreetAddress2;
                    requestProcessInitiation.DXCRecommendationContextRequest.OrganizationPhone = recommendationContext.OrganizationPhone;

                }
                if (patientData != null)
                {
                    requestProcessInitiation.DXCPatientQueryResultRequest.IsEndDateSpecified1 = patientData.EndDateSpecified;
                    requestProcessInitiation.DXCPatientQueryResultRequest.MedicationCodeValue = patientData.MedicationCodeValue;
                    requestProcessInitiation.DXCPatientQueryResultRequest.BrandName = patientData.BrandName;
                    requestProcessInitiation.DXCPatientQueryResultRequest.PrescriptionID = patientData.PrescriptionID;
                    requestProcessInitiation.DXCPatientQueryResultRequest.MedicationName = patientData.MedicationName;
                    requestProcessInitiation.DXCPatientQueryResultRequest.DaysSupply = patientData.DaysSupply;
                    requestProcessInitiation.DXCPatientQueryResultRequest.DAW = patientData.DAW;
                    requestProcessInitiation.DXCPatientQueryResultRequest.Quantity = patientData.Quantity;
                    requestProcessInitiation.DXCPatientQueryResultRequest.RefillsTotal = patientData.RefillsTotal;
                    requestProcessInitiation.DXCPatientQueryResultRequest.DeliveryMethod = patientData.DeliveryMethod;
                    requestProcessInitiation.DXCPatientQueryResultRequest.Administered = patientData.Administered;
                    requestProcessInitiation.DXCPatientQueryResultRequest.Frequency = patientData.Frequency;
                    requestProcessInitiation.DXCPatientQueryResultRequest.Dose = patientData.Dose;
                    requestProcessInitiation.DXCPatientQueryResultRequest.FreeSigText = patientData.FreeSigText;
                    requestProcessInitiation.DXCPatientQueryResultRequest.PharmacyID = patientData.PharmacyID;
                    requestProcessInitiation.DXCPatientQueryResultRequest.PharmacyName = patientData.PharmacyName;
                    requestProcessInitiation.DXCPatientQueryResultRequest.OrderDateTime = patientData.OrderDateTime;
                    requestProcessInitiation.DXCPatientQueryResultRequest.StartDateTime = patientData.StartDateTime;
                    requestProcessInitiation.DXCPatientQueryResultRequest.InsuranceInformationSourceName = patientData.InsuranceInformationSourceName;
                    requestProcessInitiation.DXCPatientQueryResultRequest.InsuranceInformationSourceID = patientData.InsuranceInformationSourceID;

                    requestProcessInitiation.DXCPatientQueryResultRequest.DiagnosisCodingSystem = patientData.DiagnosisCodingSystem;
                    requestProcessInitiation.DXCPatientQueryResultRequest.DiagnosisCodingValue = patientData.DiagnosisCodingValue;

                }

                var response = GetEps().ProcessInitiationRequest(requestProcessInitiation);
                timer.Message = string.Format("<Request>{0}</Request><Response>{1}</Response>", requestProcessInitiation, response.ToLogString());

                return response;
            }
        }

        public static void ConfirmRequestForSpecialtyMeds(DXCRecommendationContext recommendationContext, DXCConfirmOffer confirmOffer, ConnectionStringPointer dbID)
        {
            using (var timer = logger.StartTimer("ConfirmOffer"))
            {
                var confirmOfferCombinedRequest = ServiceRequest<DXCConfirmOfferCombinedRequest>();
                confirmOfferCombinedRequest.DXCRecommendationContextRequest = new DXCRecommendationContextRequest();
                if (recommendationContext != null)
                {
                    confirmOfferCombinedRequest.DXCRecommendationContextRequest = new DXCRecommendationContextRequest();
                    confirmOfferCombinedRequest.DXCRecommendationContextRequest.LicenseID = recommendationContext.LicenseID;
                    confirmOfferCombinedRequest.DXCRecommendationContextRequest.UserID = recommendationContext.UserID;
                    confirmOfferCombinedRequest.DbID = (ePrescribeSvc.ConnectionStringPointer)recommendationContext.DbID;

                    confirmOfferCombinedRequest.DXCRecommendationContextRequest.PatientMRN = recommendationContext.PatientMRN;
                    confirmOfferCombinedRequest.DXCRecommendationContextRequest.PatientID = recommendationContext.PatientID;
                    confirmOfferCombinedRequest.DXCRecommendationContextRequest.PatientDOB = recommendationContext.PatientDOB;
                    confirmOfferCombinedRequest.DXCRecommendationContextRequest.PatientFirstName = recommendationContext.PatientFirstName;
                    confirmOfferCombinedRequest.DXCRecommendationContextRequest.PatientLastName = recommendationContext.PatientLastName;
                    confirmOfferCombinedRequest.DXCRecommendationContextRequest.PatientGender = recommendationContext.PatientGender;
                    confirmOfferCombinedRequest.DXCRecommendationContextRequest.PatientCity = recommendationContext.PatientCity;
                    confirmOfferCombinedRequest.DXCRecommendationContextRequest.PatientPostalCode = recommendationContext.PatientPostalCode;
                    confirmOfferCombinedRequest.DXCRecommendationContextRequest.PatientState = recommendationContext.PatientState;
                    confirmOfferCombinedRequest.DXCRecommendationContextRequest.PatientStreetAddress1 = recommendationContext.PatientStreetAddress1;
                    confirmOfferCombinedRequest.DXCRecommendationContextRequest.PatientStreetAddress2 = recommendationContext.PatientStreetAddress2;
                    confirmOfferCombinedRequest.DXCRecommendationContextRequest.PatientTelephone = recommendationContext.PatientTelephone;
                    confirmOfferCombinedRequest.DXCRecommendationContextRequest.PatientMobile = recommendationContext.PatientMobile;
                    confirmOfferCombinedRequest.DXCRecommendationContextRequest.PatientEmail = recommendationContext.PatientEmail;

                    confirmOfferCombinedRequest.DXCRecommendationContextRequest.ProviderFirstName = recommendationContext.ProviderFirstName;
                    confirmOfferCombinedRequest.DXCRecommendationContextRequest.ProviderLastName = recommendationContext.ProviderLastName;
                    confirmOfferCombinedRequest.DXCRecommendationContextRequest.ProviderDegree = recommendationContext.ProviderDegree;
                    confirmOfferCombinedRequest.DXCRecommendationContextRequest.ProviderID = recommendationContext.ProviderID;
                    confirmOfferCombinedRequest.DXCRecommendationContextRequest.DEANumber = recommendationContext.DEANumber;

                    confirmOfferCombinedRequest.DXCRecommendationContextRequest.OrganizationCity = recommendationContext.OrganizationCity;
                    confirmOfferCombinedRequest.DXCRecommendationContextRequest.OrganizationName = recommendationContext.OrganizationName;
                    confirmOfferCombinedRequest.DXCRecommendationContextRequest.OrganizationPostalCode = recommendationContext.OrganizationPostalCode;
                    confirmOfferCombinedRequest.DXCRecommendationContextRequest.OrganizationState = recommendationContext.OrganizationState;
                    confirmOfferCombinedRequest.DXCRecommendationContextRequest.OrganizationStreetAddress1 = recommendationContext.OrganizationStreetAddress1;
                    confirmOfferCombinedRequest.DXCRecommendationContextRequest.OrganizationStreetAddress2 = recommendationContext.OrganizationStreetAddress2;
                    confirmOfferCombinedRequest.DXCRecommendationContextRequest.OrganizationPhone = recommendationContext.OrganizationPhone;
                }
                if (confirmOffer != null)
                {
                    confirmOfferCombinedRequest.DXCConfirmOfferRequest = new DXCConfirmOfferRequest();
                    confirmOfferCombinedRequest.DXCConfirmOfferRequest.ActivityID = confirmOffer.ActivityID;
                    confirmOfferCombinedRequest.DXCConfirmOfferRequest.ConfirmOffer = confirmOffer.ConfirmOffer;
                    confirmOfferCombinedRequest.DXCConfirmOfferRequest.PatientEmail = confirmOffer.PatientEmail;
                    confirmOfferCombinedRequest.DXCConfirmOfferRequest.PatientMobilePhone = confirmOffer.PatientMobilePhone;
                    confirmOfferCombinedRequest.DXCConfirmOfferRequest.TimeStamp = confirmOffer.TimeStamp;
                }

                GetEps().ConfirmOffer(confirmOfferCombinedRequest);
                timer.Message = string.Format("<Request>{0}</Request>", confirmOfferCombinedRequest);
            }
        }
        public static bool IsProviderEnrolledInSpecialtyMed(string LicenseId, string NPIForSpecialtyMed, ConnectionStringPointer DbId)
        {
            using (var timer = logger.StartTimer("IsProviderEnrolledInSpecialtyMed"))
            {
                var request = ServiceRequest<GetSpecialtyMedRequest>();
                request.LicenseId = LicenseId;
                request.NPIForSpecialtyMed = NPIForSpecialtyMed;
                request.DbID = (ePrescribeSvc.ConnectionStringPointer)DbId;

                var response = GetEps().IsProviderEnrolledInSpecialtyMed(request);
                timer.Message = string.Format("<Request>{0}</Request><Response>{1}</Response>", request.ToLogString(), response.ToLogString());

                return response;
            }
        }

        public static ePrescribeSvc.License GetLicenseByAccountID(string accountID, ConnectionStringPointer dbID)
        {
            using (var timer = logger.StartTimer("GetLicenseByAccountID"))
            {
                ePrescribeSvc.License lic = new ePrescribeSvc.License();
                AuditInfo audit = new AuditInfo();
                lic.AccountID = accountID;

                var licReq = ServiceRequest<LicenseRequest>();
                licReq.License = lic;
                licReq.reqAuditInfo = audit;
                licReq.DbID = (ePrescribeSvc.ConnectionStringPointer)dbID;

                var response = GetEps().GetLicenseByID(licReq);
                timer.Message = string.Format("<Request>{0}</Request><Response>{1}</Response>", licReq.ToLogString(), response.ToLogString());


                return response;
            }
        }

        public static ePrescribeSvc.PatientEligAndMedHxStatus[] GetEligAndMedHxStatus(string patientID, string licenseID, int siteID, ConnectionStringPointer dbID)
        {
            using (var timer = logger.StartTimer("GetEligAndMedHxStatus"))
            {
                var req = ServiceRequest<GetEligAndMedHxStatusRequest>();
                req.PatientGUID = patientID;
                req.LicenseID = licenseID;
                req.SiteID = siteID;
                req.DbID = (ePrescribeSvc.ConnectionStringPointer)dbID;

                var res = GetEps().GetEligAndMedHxStatus(req);
                timer.Message = string.Format("<Request>{0}</Request><Response>{1}</Response>", res.ToLogString(), res.ToLogString());
                return res.PatientEligAndMedHxStatuses;
            }
        }

        public static ePrescribeSvc.LicenseSite GetLicenseSiteByID(string licenseID, int siteID)
        {
            using (var timer = logger.StartTimer("GetLicenseSiteByID"))
            {
                AuditInfo audit = new AuditInfo();
                audit.LicenseID = licenseID;

                var licSiteReq = ServiceRequest<LicenseSiteRequest>();
                licSiteReq.LicenseID = licenseID;
                licSiteReq.LicenseSiteID = siteID;
                licSiteReq.reqAuditInfo = audit;

                var response = GetEps().GetLicenseSiteByID(licSiteReq);
                timer.Message = string.Format("<Request>{0}</Request><Response>{1}</Response>", licSiteReq.ToLogString(), response.ToLogString());

                return response;

            }
        }

        public static LicenseCreationDateResponse GetLicenseCreationDate(string licenseID)
        {
            using (var timer = logger.StartTimer("GetLicenseCreationDate"))
            {
                var response = GetEps().GetLicenseCreationDate(new LicenseCreationDateRequest
                {
                    LicenseID = Guid.Parse(licenseID)
                });
                timer.Message = string.Format("<Response>{0}</Response>", response.ToLogString());
                return response;
            }
        }

        public static bool SavePatient(string licenseID, string userId, string patientID, string chartID,
            string lastName, string firstName, string middleInitial, string address1, string address2,
            string city, string state, string zip, string phone, string dob, string account,
            string gender, int active, string notes, string ext_PMS_Code, string comments, string email,
            string paternalName, string maternalName, string pharmacyNABP, string pharmacyState,
            string mobilePhone, bool? isPatientHistoryExcluded, string preferredLanguage, string weight, string height, bool? isHealthPlanDisclosable, ConnectionStringPointer dbID)
        {
            using (var timer = logger.StartTimer("SavePatient"))
            {
                ePrescribeSvc.Patient pat = new ePrescribeSvc.Patient
                {
                    LicenseID = licenseID,
                    PatientID = patientID,
                    MRN = chartID,
                    LastName = lastName.Trim(),
                    FirstName = firstName.Trim(),
                    MiddleInitial = middleInitial
                };

                ePrescribeSvc.Address patAddress = new ePrescribeSvc.Address
                {
                    Address1 = address1,
                    Address2 = address2,
                    City = city,
                    State = state,
                    ZIPCode = zip
                };

                pat.Address = patAddress;
                pat.IsPatientHistoryExcluded = isPatientHistoryExcluded;
                pat.PreferredLanguage = preferredLanguage;
                pat.IsHealthPlanDisclosable = isHealthPlanDisclosable;

                if (phone != null && phone.Length > 0 && phone.Length > 9)
                {
                    ePrescribeSvc.Phone patPhone = new ePrescribeSvc.Phone();
                    phone = StringHelper.CleanNumeric(phone);

                    if (phone.Length == 10)
                    {
                        patPhone.AreaCode = phone.Substring(0, 3);
                        patPhone.BaseNumber = phone.Substring(3, 7);
                        patPhone.Type = ePrescribeSvc.PhoneType.Home;
                        pat.HomePhone = patPhone;
                    }
                }

                if (mobilePhone != null && mobilePhone.Length > 0 && mobilePhone.Length > 9)
                {
                    ePrescribeSvc.Phone patMobilePhone = new ePrescribeSvc.Phone();
                    mobilePhone = StringHelper.CleanNumeric(mobilePhone);

                    if (mobilePhone.Length == 10)
                    {
                        patMobilePhone.AreaCode = mobilePhone.Substring(0, 3);
                        patMobilePhone.BaseNumber = mobilePhone.Substring(3, 7);
                        patMobilePhone.Type = ePrescribeSvc.PhoneType.Mobile;
                        pat.MobilePhone = patMobilePhone;
                    }
                }

                pat.DOB = DateTime.Parse(dob);
                pat.Account = account;

                ePrescribeSvc.Gender patGender = new ePrescribeSvc.Gender { GenderCode = gender };
                pat.Gender = patGender;

                double wht;
                Double.TryParse(weight, out wht);
                var epsWeight = new ePrescribeSvc.Weight { Kgs = wht == 0.0 ? "0" : weight };
                pat.Weight = epsWeight;

                double hht;
                Double.TryParse(height, out hht);
                var epsHeight = new ePrescribeSvc.Height { Cm = hht == 0.0 ? "0" : height };
                pat.Height = epsHeight;

                pat.Notes = notes;
                pat.Active = active == 1;
                pat.Email = email;
                pat.PaternalName = paternalName;
                pat.MaternalName = maternalName;

                pat.PharmNABP = pharmacyNABP;
                pat.PharmState = pharmacyState;

                var savePatReq = ServiceRequest<SavePatientRequest>();
                savePatReq.UserID = userId;
                savePatReq.Patient = pat;
                savePatReq.DbID = (ePrescribeSvc.ConnectionStringPointer)dbID;

                timer.Message = string.Format("<Request>{0}</Request>", savePatReq.ToLogString());

                return GetEps().SavePatient(savePatReq);
            }
        }

        public static GetMatchingPharmacyResponse GetMatchingPharmacy(string pharmacyNpiOrNabp, ConnectionStringPointer dbID)
        {
            using (var timer = logger.StartTimer("GetMatchingPharmacy"))
            {
                GetMatchingPharmacyRequest request = new GetMatchingPharmacyRequest
                {
                    PharmacyNpiOrNabp = pharmacyNpiOrNabp,
                    DbID = (ePrescribeSvc.ConnectionStringPointer)dbID
                };
                GetMatchingPharmacyResponse response = GetEps().GetMatchingPharmacy(request);
                timer.Message = string.Format("<Response>{0}</Response>", response.ToLogString());
                return response;
            }
        }

        public static ePrescribeSvc.LicenseSite[] GetAllBackdoorSites(string licenseID, string userID)
        {
            using (var timer = logger.StartTimer("GetAllBackdoorSites"))
            {
                //Based on current GUID lets determine the correct backdoor function to call

                GetBackdoorSitesResponse response = GetEps().GetAllBackdoorSites(licenseID, userID);
                timer.Message = string.Format("<Response>{0}</Response>", response.ToLogString());

                return response.LicenseSites;
            }
        }

        public static void CreateBackdoorUserAuditLogEntry(string userID, string selectedLicenseID, int selectedSiteID, string sessionID, string app, ConnectionStringPointer dbID)
        {

            using (var timer = logger.StartTimer("CreateBackdoorUserAuditLogEntry"))
            {
                var req = ServiceRequest<CreateBackdoorUserAuditLogEntryRequest>();
                req.UserID = userID;
                req.SelectedLicenseID = selectedLicenseID;
                req.SelectedSiteID = selectedSiteID;
                req.SessionID = sessionID;
                req.App = app;
                req.DbID = (ePrescribeSvc.ConnectionStringPointer)dbID;

                timer.Message = string.Format("<Request>{0}</Request>", req.ToLogString());

                GetEps().CreateBackdoorUserAuditLogEntry(req);
            }
        }

        public static void AuditLogPatientInsert(ePrescribeSvc.AuditAction auditAction, string licenseID, string userID, string patientID, string ipAddress, ConnectionStringPointer dbID)
        {
            using (var timer = logger.StartTimer("AuditLogPatientInsert"))
            {
                var req = ServiceRequest<AuditPatientInsertRequest>();
                req.AuditAction = auditAction;
                req.ApplicationID = ePrescribeApplication.MainApplication;
                req.LicenseID = licenseID;
                req.UserID = userID;
                req.PatientID = patientID;
                req.IPAddress = ipAddress;
                req.DbID = (ePrescribeSvc.ConnectionStringPointer)dbID;

                timer.Message = string.Format("<Request>{0}</Request>", req.ToLogString());
                GetEps().AuditLogPatientInsert(req);
            }
        }

        AuditLogPatientRxResponse IEPSBroker.AuditLogPatientRxInsert(AuditAction auditAction, string licenseID, string userID, string patientID, string ipAddress, string rxId, ConnectionStringPointer dbID, string createdUTC)
        {
            return AuditLogPatientRxInsert(auditAction, licenseID, userID, patientID, ipAddress, rxId, dbID, createdUTC);
        }

        public static AuditLogPatientRxResponse AuditLogPatientRxInsert(ePrescribeSvc.AuditAction auditAction, string licenseID, string userID, string patientID, string ipAddress, string rxId, ConnectionStringPointer dbID, string createdUTC = null)
        {
            using (var timer = logger.StartTimer("AuditLogPatientRxInsert2"))
            {
                var req = ServiceRequest<AuditPatientRxInsertRequest>();
                req.AuditAction = auditAction;
                req.ApplicationID = ePrescribeApplication.MainApplication;
                req.LicenseID = licenseID;
                req.UserID = userID;
                req.PatientID = patientID;
                req.IPAddress = ipAddress;
                req.RxID = string.IsNullOrWhiteSpace(rxId) ? Guid.Empty.ToString() : rxId;
                req.CreatedUTC = string.IsNullOrEmpty(createdUTC) ? DateTime.UtcNow.ToString() : createdUTC;
                req.DbID = (ePrescribeSvc.ConnectionStringPointer)dbID;

                timer.Message = string.Format("<Request>{0}</Request>", req.ToLogString());

                return GetEps().AuditLogPatientRxInsert(req);
            }
        }

        public static AuditLogPatientRxCSResponse AuditLogPatientRxCSInsert(ePrescribeSvc.AuditAction auditAction, long AuditLogPatientRxID, bool isSuccessful, string licenseID, string userID, string patientID, string ipAddress, ConnectionStringPointer dbID, string createdUTC = null)
        {
            using (var timer = logger.StartTimer("AuditLogPatientRxCSInsert"))
            {
                var req = ServiceRequest<AuditPatientRxCSInsertRequest>();
                req.AuditAction = auditAction;
                req.ApplicationID = ePrescribeApplication.MainApplication;
                req.LicenseID = licenseID;
                req.UserID = userID;
                req.PatientID = patientID;
                req.IPAddress = ipAddress;
                req.AuditLogPatientRxID = AuditLogPatientRxID;
                req.IsSuccessful = isSuccessful;
                req.CreatedUTC = string.IsNullOrEmpty(createdUTC) ? DateTime.UtcNow.ToString() : createdUTC;
                req.DbID = (ePrescribeSvc.ConnectionStringPointer)dbID;

                timer.Message = string.Format("<Request>{0}</Request>", req.ToLogString());

                return GetEps().AuditLogPatientRxCSInsert(req);
            }
        }

        public static void AuditLogUserInsert(ePrescribeSvc.AuditAction auditAction, string licenseID, string userID, string userIDActionPerformedOn, string ipAddress, ConnectionStringPointer dbID)
        {
            using (var timer = logger.StartTimer("AuditLogUserInsert"))
            {
                var req = ServiceRequest<AuditUserInsertRequest>();
                req.AuditAction = auditAction;
                req.ApplicationID = ePrescribeApplication.MainApplication;
                req.LicenseID = licenseID;
                req.UserID = userID;
                req.UserIDActionPerformedOn = userIDActionPerformedOn;
                req.IPAddress = ipAddress;
                req.DbID = (ePrescribeSvc.ConnectionStringPointer)dbID;

                timer.Message = string.Format("<Request>{0}</Request>", req.ToLogString());
                GetEps().AuditLogUserInsert(req);
            }
        }

        public static void AuditLogLicenseInsert(ePrescribeSvc.AuditAction auditAction, string licenseID, string userID, int siteIDActionPerformedOn, string ipAddress, ConnectionStringPointer dbID)
        {
            using (var timer = logger.StartTimer("AuditLogLicenseInsert"))
            {
                var req = ServiceRequest<AuditLicenseInsertRequest>();
                req.AuditAction = auditAction;
                req.ApplicationID = ePrescribeApplication.MainApplication;
                req.LicenseID = licenseID;
                req.UserID = userID;
                req.SiteIDActionPerformedOn = siteIDActionPerformedOn;
                req.IPAddress = ipAddress;
                req.DbID = (ePrescribeSvc.ConnectionStringPointer)dbID;

                timer.Message = string.Format("<Request>{0}</Request>", req.ToLogString());
                GetEps().AuditLogLicenseInsert(req);
            }
        }

        private static string appSettings(string key)
        {
            return System.Configuration.ConfigurationManager.AppSettings[key].ToString();
        }

        public static void UpdateShieldTenant(string licenseID, string licenseName, int ShieldInternalTenantID, string UserId, ConnectionStringPointer DBID)
        {
            using (var timer = logger.StartTimer("UpdateShieldTenant"))
            {
                var updateShieldTenantRequest = ServiceRequest<UpdateShieldTenantRequest>();
                updateShieldTenantRequest.LicenseName = licenseName;
                updateShieldTenantRequest.ShieldInternalTenantID = ShieldInternalTenantID;
                updateShieldTenantRequest.DbID = (ePrescribeSvc.ConnectionStringPointer)DBID;
                updateShieldTenantRequest.reqAuditInfo = new AuditInfo() { LicenseID = licenseID, UserID = UserId };
                timer.Message = string.Format("<Request>{0}</Request>", updateShieldTenantRequest.ToLogString());
                GetEps().UpdateShieldTenant(updateShieldTenantRequest);
            }
        }

        public static string GetNewUniqueUserName(string firstName, string middleInitial, string lastName)
        {
            using (var timer = logger.StartTimer("GetNewUniqueUserName"))
            {
                var getNewUniqueUserNameRequest = ServiceRequest<GetNewUniqueUserNameRequest>();
                getNewUniqueUserNameRequest.FirstName = firstName;
                getNewUniqueUserNameRequest.MiddleName = middleInitial;
                getNewUniqueUserNameRequest.LastName = lastName;

                string newUniqueUserName = GetEps().GetNewUniqueUserName(getNewUniqueUserNameRequest);

                timer.Message = string.Format("<Request>{0}</Request><Response>{1}</Response>", getNewUniqueUserNameRequest.ToLogString(), newUniqueUserName);


                return newUniqueUserName;
            }
        }

        public static int GetShieldInternalAppID()
        {
            using (var timer = logger.StartTimer("GetShieldInternalAppID"))
            {
                var appid = ApplicationEnum.ePrescribe;

                int shieldInternalAppID = GetEps().GetShieldInternalAppID(appid);
                timer.Message = string.Format("<Request>{0}</Request><Response>{1}</Response>", appid, shieldInternalAppID);

                return shieldInternalAppID;
            }
        }

        public static int GetShieldInternalEnvironmentID(string accountID)
        {
            using (var timer = logger.StartTimer("GetShieldInternalEnvironmentID"))
            {
                var envReq = ServiceRequest<GetShieldInternalEnvironmentIDRequest>();
                envReq.ApplicationEnum = ApplicationEnum.ePrescribe;
                envReq.AccountID = accountID;

                int shieldInternalEnvironmentID = GetEps().GetShieldInternalEnvironmentID(envReq);

                timer.Message = string.Format("<Request>{0}</Request><Response>{1}</Response>", envReq.ToLogString(), shieldInternalEnvironmentID);


                return shieldInternalEnvironmentID;
            }
        }

        public static int GetShieldInternalAppInstanceID(string accountID)
        {
            using (var timer = logger.StartTimer("GetShieldInternalAppInstanceID"))
            {
                var appInstanceReq = ServiceRequest<GetShieldInternalAppInstanceIDRequest>();
                appInstanceReq.ApplicationEnum = ApplicationEnum.ePrescribe;
                appInstanceReq.AccountID = accountID;

                int shieldInternalAppInstanceID = GetEps().GetShieldInternalAppInstanceID(appInstanceReq);

                timer.Message = string.Format("<Request>{0}</Request><Response>{1}</Response>", appInstanceReq.ToLogString(), shieldInternalAppInstanceID);

                return shieldInternalAppInstanceID;
            }
        }

        public static ePrescribeSvc.Role[] GetRolesForApplication(int shieldInternalAppID)
        {
            using (var timer = logger.StartTimer("GetRolesForApplication"))
            {
                var response = GetEps().GetRolesForApplication(shieldInternalAppID);
                timer.Message = string.Format("<Request>{0}</Request><Response>{1}</Response>", shieldInternalAppID, response.ToLogString());

                return response;
            }
        }

        AuthenticateAndAuthorizeUserResponse IEPSBroker.AuthenticateAndAuthorizeUser(string userName, string password, string ipAddress)
        {
            return AuthenticateAndAuthorizeUser(userName, password, ipAddress);
        }

        public static AuthenticateAndAuthorizeUserResponse AuthenticateAndAuthorizeUser(string userName, string password, string ipAddress)
        {
            using (var timer = logger.StartTimer("AuthenticateAndAuthorizeUser"))
            {
                var authenticateAndAuthorizeUserRequest = ServiceRequest<AuthenticateAndAuthorizeUserRequest>();
                authenticateAndAuthorizeUserRequest.UserName = userName;
                authenticateAndAuthorizeUserRequest.Password = password;
                authenticateAndAuthorizeUserRequest.IPAddress = ipAddress;
                authenticateAndAuthorizeUserRequest.CallingApplication = ePrescribeApplication.MainApplication;

                var response = GetEps().AuthenticateAndAuthorizeUser(authenticateAndAuthorizeUserRequest);

               // do not log password!
                timer.Message = string.Format("<Request>{0}</Request><Response>{1}</Response>", userName, response.ToLogString());


                return response;
            }
        }

        AuthenticateAndAuthorizeUserResponse IEPSBroker.AuthorizeUser(string userName, string shieldIdentityToken, string ipAddress)
        {
            return AuthorizeUser(userName, shieldIdentityToken, ipAddress);
        }

        public static AuthenticateAndAuthorizeUserResponse AuthorizeUser(string userName, string shieldIdentityToken, string ipAddress)
        {
            using (var timer = logger.StartTimer("AuthenticateAndAuthorizeUser"))
            {
                var authenticateAndAuthorizeUserRequest = ServiceRequest<AuthenticateAndAuthorizeUserRequest>();
                authenticateAndAuthorizeUserRequest.UserName = userName;
                authenticateAndAuthorizeUserRequest.ShieldIdentityToken = shieldIdentityToken;
                authenticateAndAuthorizeUserRequest.IPAddress = ipAddress;
                authenticateAndAuthorizeUserRequest.CallingApplication = ePrescribeApplication.MainApplication;

               var response = GetEps().AuthorizeUser(authenticateAndAuthorizeUserRequest);

                timer.Message = string.Format("<Request>{0}</Request><Response>{1}</Response>", userName, response.ToLogString());

                return response;
            }
        }

        /// <summary>
        /// Calls EPS which calls Shield to get a fresh SAML token for the logged in user.
        /// </summary>
        /// <param name="currentSamlToken">String representation of the current SAML token</param>
        /// <param name="ipAddress">IP address of the logged in user</param>
        /// <param name="appInstanceID">External AppInstanceID that's stored on Shield.</param>
        /// <param name="dbID">DBID of the logged in user</param>
        /// <returns>String representation of the new SAML token</returns>
        public RefreshSamlTokenResponse RefreshSamlToken(string currentSamlToken, string ipAddress, string appInstanceID, ConnectionStringPointer dbID)
        {
            using (var timer = logger.StartTimer("RefreshSamlToken"))
            {
                var refreshSamlTokenRequest = ServiceRequest<RefreshSamlTokenRequest>();
                refreshSamlTokenRequest.SecurityToken = currentSamlToken;
                refreshSamlTokenRequest.IPAddress = ipAddress;
                refreshSamlTokenRequest.AppInstanceID = appInstanceID;
                refreshSamlTokenRequest.CompareClaimCollections = true;
                refreshSamlTokenRequest.DBID = (ePrescribeSvc.ConnectionStringPointer)dbID;

                timer.Message = $"<OldSaml>{currentSamlToken}</OldSaml>";
                var refreshSamlTokenResponse = GetEps().RefreshSamlToken(refreshSamlTokenRequest);
                timer.Message += $"<Response>{refreshSamlTokenResponse.ToLogString()}</Response>";

                return refreshSamlTokenResponse;
            }
        }

        public static int GetShieldInternalTenantID(string accountID)
        {
            using (var timer = logger.StartTimer("GetShieldInternalTenantID"))
            {
                var request = ServiceRequest<GetShieldInternalTenantIDRequest>();
                request.ApplicationEnum = ApplicationEnum.ePrescribe;
                request.AccountID = accountID;

                var response = GetEps().GetShieldInternalTenantID(request);
                timer.Message = string.Format("<Request>{0}</Request><Response>{1}</Response>", request.ToLogString(), response);

                return response;
            }
        }

        GetNewActivationCodeResponse IEPSBroker.GetNewActivationCode(int shieldInternalTenantID, string userName)
        {
            return GetNewActivationCode(shieldInternalTenantID, userName);
        }

        void IEPSBroker.AuditLogPatientInsert(AuditAction patientWeightModified, string getStringOrEmpty, string s, string sessionPatientId,
            string userHostAddress, ConnectionStringPointer dbid)
        {
            AuditLogPatientInsert(patientWeightModified, getStringOrEmpty, s, sessionPatientId, userHostAddress, dbid);
        }

        public static GetUsersResponse GetUsers(int shieldInternalTenantID, string firstName, string lastName, Status status, int pageSize, int pageIndex,
            string accountID, bool isRolesRequired, ConnectionStringPointer dbID)
        {
            using (var timer = logger.StartTimer("GetUsers"))
            {
                var request = ServiceRequest<SearchShieldUsersRequest>();
                request.Application = ApplicationEnum.ePrescribe;
                request.FilterTypeSearchUser = FilterTypeEnumSearchUsers.TenantID;
                request.FirstName = firstName;
                request.LastName = lastName;
                request.PageSize = pageSize;
                request.PageIndex = pageIndex;
                request.AccountID = accountID;
                request.userStatus = status;
                request.AccountDBID = (ePrescribeSvc.ConnectionStringPointer)dbID;
                request.TenantFilterValue = shieldInternalTenantID.ToString();
                request.IsRolesRequired = isRolesRequired;

                var response = GetEps().SearchShieldUsers(request);
                timer.Message = string.Format("<Request>{0}</Request><Response>{1}</Response>", request.ToLogString(),
                    response.ToLogString());

                return response;
            }
        }

        public static GetUsersFromePrescribeResponse GetUsersFromePrescribe(string accountID, bool isRolesRequired, ConnectionStringPointer dbID)
        {
            using (var timer = logger.StartTimer("GetUsersFromePrescribe"))
            {
                var request = ServiceRequest<GetUsersFromePrescribeRequest>();
                request.AccountID = accountID;
                request.DBID = (ePrescribeSvc.ConnectionStringPointer)dbID;
                request.IsRolesRequired = isRolesRequired;

                var response = GetEps().GetUsersFromePrescribe(request);
                timer.Message = string.Format("<Request>{0}</Request><Response>{1}</Response>", request.ToLogString(), response.ToLogString());


                return response;
            }
        }

        public static ePrescribeSvc.GetiAssistSamlTokenResponse GetiAssistSamlToken(string identityToken, string shieldTenantId)
        {
            using (var timer = logger.StartTimer("GetiAssistSamlToken"))
            {
                var request = ServiceRequest<ePrescribeSvc.GetiAssistSamlTokenRequest>();
                request.IdentityToken = identityToken;
                request.ShieldTenantId = shieldTenantId;

                var response = GetEps().GetiAssistSamlToken(request);
                timer.Message = $"<Request>{request.ToLogString()}</Request><Response>{response.ToLogString()}</Response>";

                return response;
            }
        }

        int IEPSBroker.GetShieldInternalTenantID(string accountID)
        {
            return GetShieldInternalTenantID(accountID);
        }

        public static GetNewActivationCodeResponse GetNewActivationCode(int shieldInternalTenantID, string userName)
        {
            using (var timer = logger.StartTimer("GetNewActivationCode"))
            {
                var request = ServiceRequest<GetUsersRequest>();
                request.FilterType = FilterTypeEnum.TenantID;
                request.TenantFilterValue = shieldInternalTenantID.ToString();
                request.LegacyUserNameFilterValue = userName;
                request.Application = ApplicationEnum.ePrescribe;

                var response = GetEps().GetNewShieldActivationCode(request);
                timer.Message = string.Format("<Request>{0}</Request><Response>{1}</Response>", request.ToLogString(), response.ToLogString());

                return response;
            }
        }

        public static GetNewPasswordResponse ResetAndGeneratePasswordForUser(int shieldInternalTenantID, int shieldUserProfileID)
        {
            using (var timer = logger.StartTimer("ResetAndGeneratePasswordForUser"))
            {
                var request = ServiceRequest<GetNewPasswordRequest>();
                request.TenantID = shieldInternalTenantID;
                request.ShieldUserProfileID = shieldUserProfileID;

                return GetEps().ResetAndGeneratePasswordForUser(request);
            }
        }

        public static void SaveUserAppProperty(string userGUID, string propertyName, string propertyValue,
            ConnectionStringPointer dbID)
        {
            using (var timer = logger.StartTimer("SaveUserAppProperty"))
            {
                var request = ServiceRequest<SaveUserAppPropertyRequest>();
                request.UserGUID = userGUID;
                request.PropertyName = propertyName;
                request.PropertyValue = propertyValue;
                request.DBID = (ePrescribeSvc.ConnectionStringPointer)dbID;

                timer.Message = string.Format("<Request>{0}</Request>", request.ToLogString());
                GetEps().SaveUserAppProperty(request);
            }
        }

        public static void AuditLogShieldInsert(string hostIP, string moduleName, string message, string userName)
        {
            using (var timer = logger.StartTimer("AuditLogShieldInsert"))
            {
                var auditShieldInsertRequest = ServiceRequest<AuditShieldInsertRequest>();
                auditShieldInsertRequest.AppName = "ERX_MainApp";
                auditShieldInsertRequest.HostIP = hostIP;
                auditShieldInsertRequest.ModuleName = moduleName;
                auditShieldInsertRequest.Message = message;
                auditShieldInsertRequest.UserName = userName;

                timer.Message = string.Format("<Request>{0}</Request>", auditShieldInsertRequest.ToLogString());
                GetEps().AuditLogShieldInsert(auditShieldInsertRequest);
            }
        }

        public static bool IsValidPassword(string password)
        {
            using (var timer = logger.StartTimer("IsValidPassword"))
            {
                var req = ServiceRequest<IsValidPasswordRequest>();
                req.Password = password;

                IsValidPasswordResponse res = GetEps().IsValidPassword(req);

                return res.Success;
            }
        }

        public static int GetUserCountForLicense(string accountID, ConnectionStringPointer dbID)
        {
            using (var timer = logger.StartTimer("GetUserCountForLicense"))
            {
                var request = ServiceRequest<GetUsersRequest>();
                request.Application = ApplicationEnum.ePrescribe;
                request.AccountID = accountID;
                request.AccountDBID = (ePrescribeSvc.ConnectionStringPointer)dbID;

                var response = GetEps().GetUserCountForLicense(request);
                timer.Message = string.Format("<Request>{0}</Request><Response>{1}</Response>", request, response);

                return response;
            }
        }

        public static AuthenticateAndAuthorizeUserResponse LinkAndAuthorizeFederatedUser(string userGUID, string ipAddress)
        {
            using (var timer = logger.StartTimer("LinkAndAuthorizeFederatedUser"))
            {
                var linkReq = ServiceRequest<LinkAndAuthorizedFederatedUserRequest>();
                linkReq.UserGUID = userGUID;
                linkReq.IPAddress = ipAddress;

                var response = GetEps().LinkAndAuthorizeFederatedUser(linkReq);

                timer.Message = string.Format("<Request>{0}</Request><Response>{1}</Response>", linkReq.ToLogString(), response.ToLogString());

                return response;
            }
        }

        public static CreateShieldTenantResponse CreateShieldTenant(string accountID, string licenseID, string licenseName, int shieldApplicationID, string contactFirstName, string contactLastName, string contactEmail, string contactPhone)
        {
            using (var timer = logger.StartTimer("CreateShieldTenant"))
            {
                var createRequest = ServiceRequest<CreateShieldTenantRequest>();
                createRequest.AccountID = accountID;
                createRequest.LicenseID = licenseID;
                createRequest.LicenseName = licenseName;
                createRequest.ShieldContact = new ShieldContact()
                {
                    FirstName = contactFirstName,
                    LastName = contactLastName,
                    Phone = contactPhone,
                    Email = contactEmail
                };
                createRequest.ShieldApplicationID = shieldApplicationID;

                var response = GetEps().CreateShieldTenant(createRequest);
                timer.Message = string.Format("<Request>{0}</Request><Response>{1}</Response>", createRequest.ToLogString(), response.ToLogString());

                return response;
            }
        }

        public static CreateShieldTenantResponse CreateShieldTenant(string accountID, string licenseID, string licenseName, int shieldApplicationID)
        {
            using (var timer = logger.StartTimer("CreateShieldTenant2"))
            {
                var createRequest = ServiceRequest<CreateShieldTenantRequest>();
                createRequest.AccountID = accountID;
                createRequest.LicenseName = licenseName;
                createRequest.LicenseID = licenseID;
                createRequest.ShieldContact = null;
                createRequest.ShieldApplicationID = shieldApplicationID;

                var response = GetEps().CreateShieldTenant(createRequest);
                timer.Message = string.Format("<Request>{0}</Request><Response>{1}</Response>", createRequest.ToLogString(), response.ToLogString());

                return response;
            }
        }

        public static ePrescribeSvcResponse ChangeUserPassword(string userName, string oldPassword, string newPassword, string samlTokenString, string externalTenantID, ConnectionStringPointer dbID)
        {
            using (var timer = logger.StartTimer("ChangeUserPassword"))
            {
                var changePassword = ServiceRequest<ChangePasswordRequest>();
                changePassword.UserName = userName;
                changePassword.OldPassword = oldPassword;
                changePassword.NewPassword = newPassword;
                changePassword.SAMLTokenString = samlTokenString;
                changePassword.ExternalTenantID = externalTenantID;
                changePassword.DbID = (ePrescribeSvc.ConnectionStringPointer)dbID;

                return GetEps().ChangeUserPassword(changePassword);
            }
        }

        public static List<SecretQuestion> GetAllShieldSecretQuestions()
        {
            using (var timer = logger.StartTimer("GetAllShieldSecretQuestions"))
            {
                var response = new GetAllSecretQuestionsResponse();

                response = GetEps().GetAllShieldSecretQuestions();

                timer.Message = string.Format("<Response>{0}</Response>", response.ToLogString());

                return response.SecretQuestions.ToList();
            }
        }

        public static SecretQuestion[] GetUserShieldSecretQuestions(string shieldSecurityToken, string shieldExternalTenantId)
        {
            using (var timer = logger.StartTimer("GetUserShieldSecretQuestions"))
            {
                var getUserSecretQuestionsRequest = ServiceRequest<GetUserSecretQuestionsRequest>();
                getUserSecretQuestionsRequest.ExternalTenantID = shieldExternalTenantId;
                getUserSecretQuestionsRequest.SAMLTokenString = shieldSecurityToken;

                var getUserSecretQuestionsResponse = GetEps().GetUserShieldSecretQuestions(getUserSecretQuestionsRequest);
                return getUserSecretQuestionsResponse.SecretQuestions;
            }
        }

        public static SecretQuestion[] GetUserShieldSecretQuestionsByUsername(string username)
        {
            using (var timer = logger.StartTimer("GetUserShieldSecretQuestionsByUsername"))
            {
                var getUserSecretQuestionsRequest = ServiceRequest<GetUserSecretQuestionsRequest>();
                getUserSecretQuestionsRequest.ShieldUsername = username;

                var getUserSecretQuestionsResponse = GetEps().GetUserShieldSecretQuestions(getUserSecretQuestionsRequest);
                return getUserSecretQuestionsResponse.SecretQuestions;
            }
        }

        public static ResetForgottenPasswordResponse ResetForgottenPassword(string username, string newPassword, List<SecretAnswer> answers)
        {
            using (var timer = logger.StartTimer("ResetForgottenPassword"))
            {
                var request = ServiceRequest<ResetForgottenPasswordRequest>();
                request.UserName = username;
                request.NewPassword = newPassword;
                request.SecretAnswers = answers.ToArray();

                var response = GetEps().ResetForgottenPassword(request);

                request.SecretAnswers = new SecretAnswer[0];
                request.NewPassword = "";
                timer.Message = $"<Request>{request.ToLogString()}</Request><Response>{response.ToLogString()}</Response>";
                return response;
            }
        }

        public static SaveSecretQuestionAnswersResponse SaveSecretQuestionAnswers(ePrescribeSvc.SecretAnswer[] secretAnswers, string shieldUserName, string shieldPassword, string shieldSecurityToken, string shieldExternalTenantId, string ipAddress, ConnectionStringPointer dbID)
        {
            using (var timer = logger.StartTimer("SaveSecretQuestionAnswers"))
            {
                var saveSecretQuestionAnswersRequest = ServiceRequest<SaveSecretQuestionAnswersRequest>();
                saveSecretQuestionAnswersRequest.SecretAnswers = secretAnswers;
                saveSecretQuestionAnswersRequest.UserName = shieldUserName;
                saveSecretQuestionAnswersRequest.SAMLTokenString = shieldSecurityToken;
                saveSecretQuestionAnswersRequest.ExternalTenantID = shieldExternalTenantId;
                saveSecretQuestionAnswersRequest.Password = shieldPassword;
                saveSecretQuestionAnswersRequest.IPAddress = ipAddress;
                saveSecretQuestionAnswersRequest.DbID = (ePrescribeSvc.ConnectionStringPointer)dbID;

                return GetEps().SaveSecretQuestionAnswers(saveSecretQuestionAnswersRequest);
            }
        }

        public static EnrollProviderResponse EnrollProvider(string sFirstName, string sLastName, bool isActive,
            string sOfficeAddressLine1, string sOfficeAddressLine2, string sOfficeCity,
            string sOfficeState, string sOfficeZipCode, string sEmail, string sSitePhone, string sSiteFax,
            string sOfficeTitle,
            string sDEANo, string sUPIN, string UserID, int AccountID, bool enableRenewals, bool epcsRegistered,
            ConnectionStringPointer dbID, string licenseID)
        {

            using (var timer = logger.StartTimer("EnrollProvider"))
            {
                var request = ServiceRequest<EnrollProviderRequest>();
                request.FirstName = sFirstName;
                request.LastName = sLastName;
                request.Address1 = sOfficeAddressLine1;
                request.Address2 = sOfficeAddressLine2;
                request.City = sOfficeCity;
                request.State = sOfficeState;
                request.ZipCode = sOfficeZipCode;
                request.Email = sEmail;
                request.SitePhone = sSitePhone;
                request.SiteFax = sSiteFax;
                request.SiteName = sOfficeTitle;
                request.DEA = sDEANo;
                request.NPI = sUPIN;
                request.UserID = UserID;
                request.AccountID = AccountID.ToString();
                request.DbID = (ePrescribeSvc.ConnectionStringPointer)dbID;
                request.reqAuditInfo = new AuditInfo { LicenseID = licenseID };
                request.isActive = isActive;

                var serviceLevels = new List<HubServiceLevel>();
                if (enableRenewals) serviceLevels.Add(HubServiceLevel.RENEWALS);
                if (epcsRegistered) serviceLevels.Add(HubServiceLevel.EPCS);

                request.ServiceLevels = serviceLevels.ToArray();

                request.Source = ePrescribeApplication.MainApplication;

                var response = GetEps().EnrollProvider(request);
                timer.Message = string.Format("<Request>{0}</Request><Response>{1}</Response>", request.ToLogString(),
                    response.ToLogString());
                return response;
            }
        }

        public static void UpdateSecUsersInfoFromShield(
            string userGUID,
            string ePrescriberUserName,
            string shieldUserName,
            string firstName,
            string lastName,
            Allscripts.ePrescribe.Objects.ShieldUserStatus shieldUserStatus,
            ePrescribeSvc.Role[] appRoles,
            ConnectionStringPointer dbID)
        {
            using (var timer = logger.StartTimer("UpdateSecUsersInfoFromShield"))
            {
                var request = ServiceRequest<UpdateSecUsersInfoFromShieldRequest>();
                request.ePrescribeUserID = userGUID;
                request.ePrescribeUserName = ePrescriberUserName;
                request.ShieldUserName = shieldUserName;
                request.FirstName = firstName;
                request.LastName = lastName;
                request.ShieldStatus = (ShieldUserStatus)shieldUserStatus;
                request.AppRoles = appRoles;
                request.ForceAppRoleUpdate = false;
                request.ForceAppPermissionUpdate = false;
                request.DbID = (ePrescribeSvc.ConnectionStringPointer)dbID;

                timer.Message = string.Format("<Request>{0}</Request>", request.ToLogString());
                GetEps().UpdateSecUsersInfoAnd3rdParties(request);
            }
        }

        public static ePrescribeSvc.RxUser[] SearchShieldUsers(string firstName, string lastName, string userName)
        {

            try
            {
                using (var timer = logger.StartTimer("SearchUsers"))
                {
                    var searchAllUsersRequest = ServiceRequest<SearchUsersRequest>();
                    searchAllUsersRequest.FirstName = firstName;
                    searchAllUsersRequest.LastName = lastName;
                    searchAllUsersRequest.ShieldUserName = userName;

                    var searchAllUsersResponse = GetEps().SearchUsersInShield(searchAllUsersRequest);

                    timer.Message = string.Format("<Request>{0}</Request><Response>{1}</Response>",
                        searchAllUsersRequest.ToLogString(), searchAllUsersResponse.ToLogString());

                    return searchAllUsersResponse.RxUsers;
                }
            }
            catch (SoapException)
            {
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }


        /// <summary>
        /// This will add User to database and update its state licenses and user groups.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static CreateUserResponse AddUser(CreateUserRequest request)
        {
            using (var timer = logger.StartTimer("AddUser"))
            {
                var response = GetEps().AddUser(request);
                timer.Message = string.Format("<Request>{0}</Request><Response>{1}</Response>", request.ToLogString(), response.ToLogString());

                return response;
            }
        }

        public static ePrescribeSvcResponse UpdateUser(UpdateUserRequest updateUserRequest)
        {
            using (var timer = logger.StartTimer("UpdateUser"))
            {
                var response = GetEps().UpdateUser(updateUserRequest);
                timer.Message = string.Format("<Request>{0}</Request><Response>{1}</Response>", updateUserRequest.ToLogString(), response.ToLogString());
                return response;
            }
        }

        public static void RxOptionsUsedInsert(string rxID, Constants.RxOptions rxOption, ConnectionStringPointer dbID)
        {
            using (var timer = logger.StartTimer("RxOptionsUsedInsert"))
            {
                var rxOptionsUsedRequest = ServiceRequest<RxOptionsUsedRequest>();
                rxOptionsUsedRequest.DbID = (ePrescribeSvc.ConnectionStringPointer)dbID;
                rxOptionsUsedRequest.RxID = rxID;
                rxOptionsUsedRequest.RxOption = (RxOptions)rxOption;

                timer.Message = string.Format("<Request>{0}</Request>", rxOptionsUsedRequest.ToLogString());

                GetEps().RxOptionsUsedInsert(rxOptionsUsedRequest);
            }
        }

        public static void DeleteScriptMessages(List<string> scriptMessageIDs, ConnectionStringPointer dbID)
        {
            using (var timer = logger.StartTimer("DeleteScriptMessages"))
            {
                var req = ServiceRequest<DeleteScriptMessagesRequest>();
                req.ScriptMessageIDs = scriptMessageIDs.ToArray();
                req.DBID = (ePrescribeSvc.ConnectionStringPointer)dbID;

                timer.Message = string.Format("<Request>{0}</Request>", req.ToLogString());

                GetEps().DeleteScriptMessages(req);
            }
        }

        public static class EPA
        {
            public static string SendToEPATaskList(string licenseID, string patientID, string rxID,
                string pharmacyID, Constants.ePATaskType ePATaskType, Constants.ePATaskStatus ePATaskStatus,
                string providerID, string userID, Constants.ePANCPDPTaskType ePANCPDPTaskType, Constants.EPAType ePAType, ConnectionStringPointer dbID)
            {
                using (var timer = logger.StartTimer("SendToEPATaskList"))
                {
                    var sendToEPATaskListResponse = new EPAServiceResponse();

                    var sendToEPATaskListRequest = ServiceRequest<EPATaskRequest>();
                    sendToEPATaskListRequest.DbID = (ePrescribeSvc.ConnectionStringPointer)dbID;
                    sendToEPATaskListRequest.reqAuditInfo = new AuditInfo { LicenseID = licenseID, UserID = userID };
                    sendToEPATaskListRequest.PatientID = patientID;
                    sendToEPATaskListRequest.RxID = rxID;
                    sendToEPATaskListRequest.PharmacyID = pharmacyID;
                    sendToEPATaskListRequest.EPATaskType = (ePATaskType)ePATaskType;
                    sendToEPATaskListRequest.EPATaskStatus = (ePATaskStatus)ePATaskStatus;
                    sendToEPATaskListRequest.EPANCPDPTaskType = (ePANCPDPTaskType)ePANCPDPTaskType;
                    sendToEPATaskListRequest.ProviderID = providerID;
                    sendToEPATaskListRequest.EPAType = (EPAType)ePAType;
                    sendToEPATaskListRequest.UpdateRxStatus = true;

                    sendToEPATaskListResponse = GetEps().SendToEPATaskList(sendToEPATaskListRequest);

                    timer.Message = string.Format("<Request>{0}</Request><Response>{1}</Response>", sendToEPATaskListRequest.ToLogString(), sendToEPATaskListResponse.ToLogString());

                    return sendToEPATaskListResponse.ReturnID;
                }
            }

            public static string SendToEPARequests(string ePATaskID,
                Constants.ePATransPriority ePATransPriorityID, Constants.ePARequestType ePARequestTypeID, string userID,
                ConnectionStringPointer dbID)
            {
                using (var timer = logger.StartTimer("SendToEPARequests"))
                {
                    var sendToEPAResponse = new EPAServiceResponse();

                    var sendToEPARequest = ServiceRequest<EPARequest>();
                    sendToEPARequest.DbID = (ePrescribeSvc.ConnectionStringPointer)dbID;
                    sendToEPARequest.reqAuditInfo = new AuditInfo { UserID = userID };
                    sendToEPARequest.EPATaskID = ePATaskID;
                    sendToEPARequest.EPATransPriorityID = (ePATransPriority)ePATransPriorityID;
                    sendToEPARequest.EPARequestTypeID = (ePARequestType)ePARequestTypeID;

                    sendToEPAResponse = GetEps().SendToEPARequest(sendToEPARequest);

                    timer.Message = string.Format("<Request>{0}</Request><Response>{1}</Response>", sendToEPARequest.ToLogString(), sendToEPAResponse.ToLogString());

                    return sendToEPAResponse.ReturnID;
                }
            }

            public static void SaveEPAQuestionSetRequest(string licenseID, string userID, string ePATaskID, string ePARequestID, ConnectionStringPointer dbID)
            {
                using (var timer = logger.StartTimer("SaveEPAQuestionSetRequest"))
                {
                    var saveEPAQuestionSetRequest = ServiceRequest<EPAMsgRequest>();
                    saveEPAQuestionSetRequest.DbID = (ePrescribeSvc.ConnectionStringPointer)dbID;
                    saveEPAQuestionSetRequest.reqAuditInfo = new AuditInfo { LicenseID = licenseID, UserID = userID };
                    saveEPAQuestionSetRequest.EPATaskID = ePATaskID;
                    saveEPAQuestionSetRequest.EPARequestID = ePARequestID;

                    timer.Message = string.Format("<Request>{0}</Request>", saveEPAQuestionSetRequest.ToLogString());

                    GetEps().SaveEPAQuestionSetRequest(saveEPAQuestionSetRequest);
                }
            }

            public static void SaveEPAInitiationRequest(string licenseID, string userID, string ePATaskID, string ePARequestID, string shieldSecurityToken, ConnectionStringPointer dbID)
            {
                using (var timer = logger.StartTimer("SaveEPAInitiationRequest"))
                {
                    var paInitiationRequest = ServiceRequest<EPAMsgRequest>();
                    paInitiationRequest.DbID = (ePrescribeSvc.ConnectionStringPointer)dbID;
                    paInitiationRequest.reqAuditInfo = new AuditInfo { LicenseID = licenseID, UserID = userID };
                    paInitiationRequest.EPATaskID = ePATaskID;
                    paInitiationRequest.EPARequestID = ePARequestID;
                    paInitiationRequest.ShieldSecurityToken = shieldSecurityToken;

                    timer.Message = string.Format("<Request>{0}</Request>", paInitiationRequest.ToLogString());

                    GetEps().SendPAInitiationRequest(paInitiationRequest);
                }
            }

            public static string InsertEPARequestsAdditionalInfo(string ePARequestID, int PBMID, string firstName,
                string lastName, string address, string state, DateTime dob, char gender,
                string phoneNumber, string zipCode, string healthPlan, string memberID, string userID, ConnectionStringPointer dbID)
            {
                using (var timer = logger.StartTimer("InsertEPARequestsAdditionalInfo"))
                {
                    var sendToEPAResponse = new EPAServiceResponse();

                    var insertEPARequestsAdditionalInfoRequest = ServiceRequest<EPARequestsAdditionalInfoRequest>();
                    insertEPARequestsAdditionalInfoRequest.DbID = (ePrescribeSvc.ConnectionStringPointer)dbID;
                    insertEPARequestsAdditionalInfoRequest.reqAuditInfo = new AuditInfo { UserID = userID };
                    insertEPARequestsAdditionalInfoRequest.EPARequestID = ePARequestID;
                    insertEPARequestsAdditionalInfoRequest.PBMID = PBMID;
                    insertEPARequestsAdditionalInfoRequest.HealthPlan = healthPlan;
                    insertEPARequestsAdditionalInfoRequest.MemberID = memberID;
                    insertEPARequestsAdditionalInfoRequest.Patient = new ePrescribeSvc.Patient
                    {
                        FirstName = firstName,
                        LastName = lastName,
                        Gender = new ePrescribeSvc.Gender { GenderCode = gender.ToString() },
                        DOB = dob,
                        Address = new ePrescribeSvc.Address
                        {
                            Address1 = address,
                            State = state,
                            ZIPCode = zipCode
                        },
                        HomePhone = new ePrescribeSvc.Phone { BaseNumber = phoneNumber }
                    };

                    sendToEPAResponse = GetEps().InsertEPARequestsAdditionalInfo(insertEPARequestsAdditionalInfoRequest);

                    timer.Message = string.Format("<Request>{0}</Request><Response>{1}</Response>", insertEPARequestsAdditionalInfoRequest.ToLogString(), sendToEPAResponse.ToLogString());

                    return sendToEPAResponse.ReturnID;
                }
            }

            public static void UpdateTaskStatus(string ePATaskID, Constants.ePATaskStatus ePATaskStatus, ConnectionStringPointer dbID)
            {
                using (var timer = logger.StartTimer("UpdateTaskStatus"))
                {
                    var updateEPATaskStatusRequest = ServiceRequest<EPATaskRequest>();
                    updateEPATaskStatusRequest.DbID = (ePrescribeSvc.ConnectionStringPointer)dbID;
                    updateEPATaskStatusRequest.EPATaskID = ePATaskID;
                    updateEPATaskStatusRequest.EPATaskStatus = (ePATaskStatus)ePATaskStatus;

                    timer.Message = string.Format("<Request>{0}</Request>", updateEPATaskStatusRequest.ToLogString());

                    GetEps().UpdateEPATaskStatus(updateEPATaskStatusRequest);
                }
            }

            public static void SaveEPACancelRequest(string licenseID, string userID, string ePATaskID, string ePARequestID, string shieldSecurityToken, ConnectionStringPointer dbID)
            {
                using (var timer = logger.StartTimer("SaveEPACancelRequest"))
                {
                    var cancelRequest = ServiceRequest<EPAMsgRequest>();
                    cancelRequest.DbID = (ePrescribeSvc.ConnectionStringPointer)dbID;
                    cancelRequest.reqAuditInfo = new AuditInfo { LicenseID = licenseID, UserID = userID };
                    cancelRequest.EPATaskID = ePATaskID;
                    cancelRequest.EPARequestID = ePARequestID;
                    cancelRequest.ShieldSecurityToken = shieldSecurityToken;

                    timer.Message = string.Format("<Request>{0}</Request>", cancelRequest.ToLogString());

                    GetEps().SendePACancelRequest(cancelRequest);
                }
            }

            public static void SaveEPADecisionRequest(string licenseID, string userID, string ePATaskID, string ePARequestID, ConnectionStringPointer dbID)
            {
                using (var timer = logger.StartTimer("SaveEPADecisionRequest"))
                {
                    var saveEPAQuestionSetRequest = ServiceRequest<EPAMsgRequest>();
                    saveEPAQuestionSetRequest.DbID = (ePrescribeSvc.ConnectionStringPointer)dbID;
                    saveEPAQuestionSetRequest.reqAuditInfo = new AuditInfo { LicenseID = licenseID, UserID = userID };
                    saveEPAQuestionSetRequest.EPATaskID = ePATaskID;
                    saveEPAQuestionSetRequest.EPARequestID = ePARequestID;

                    timer.Message = string.Format("<Request>{0}</Request>", saveEPAQuestionSetRequest.ToLogString());

                    GetEps().SaveEPADecisionRequest(saveEPAQuestionSetRequest);
                }
            }

            public static void SendPAReqquest(string licenseID, string userID, string ePATaskID, string ePARequestID, string shieldSecurityToken, ConnectionStringPointer dbID)
            {
                using (var timer = logger.StartTimer("SendPAReqquest"))
                {
                    var ePARequest = ServiceRequest<EPAMsgRequest>();
                    ePARequest.DbID = (ePrescribeSvc.ConnectionStringPointer)dbID;
                    ePARequest.reqAuditInfo = new AuditInfo { LicenseID = licenseID, UserID = userID };
                    ePARequest.EPATaskID = ePATaskID;
                    ePARequest.EPARequestID = ePARequestID;
                    ePARequest.ShieldSecurityToken = shieldSecurityToken;

                    GetEps().SendPARequest(ePARequest);

                    timer.Message = string.Format("<Request>{0}</Request>", ePARequest.ToLogString());
                }
            }

            public static bool GetePAPortalStatusByClientID(string enterpriseClientID)
            {
                var request = ServiceRequest<GetePAStatusRequest>();
                request.EnterpriseClientID = enterpriseClientID;

                return GetEps().GetEnterpriseePAStatusByClientID(request);

            }

            public static string GetServiceAccountShieldToken()
            {
                using (var timer = logger.StartTimer("GetServiceAccountShieldToken"))
                {
                    var response = GetEps().GetServiceAccountShieldToken();
                    timer.Message = string.Format("<Response>{0}</Response>", response);
                    return response;
                }
            }

            public static void SendEpaCancelRequest(string ePATaskID, string licenseID, string userID, string securityToken,
                ConnectionStringPointer dbID)
            {
                using (var timer = logger.StartTimer("SendEpaCancelRequest"))
                {
                    string ePARequestID = SendToEPARequests(ePATaskID, Constants.ePATransPriority.NOT_URGENT, Constants.ePARequestType.EPACANCEL_REQUEST, userID, dbID);

                    UpdateTaskStatus(ePATaskID, Constants.ePATaskStatus.EPA_CANCEL_REQUESTED, dbID);

                    SaveEPACancelRequest(licenseID, userID, ePATaskID, ePARequestID, securityToken, dbID);
                }
            }
        }

        #region Provider Attributes

        public static ePrescribeSvc.DEALicense[] GetProviderDEALicenses(string userID, ConnectionStringPointer dbID)
        {
            using (var timer = logger.StartTimer("GetProviderDEALicenses"))
            {
                var result = new ePrescribeSvc.DEALicense[0];

                var response = GetEps().GetProviderDEALicenses(userID, (ePrescribeSvc.ConnectionStringPointer)dbID);

                if (response != null && response.lstDEALicense != null)
                {
                    result = response.lstDEALicense;
                }
                timer.Message = string.Format("<Request>{0}</Request><Response>{1}</Response>", userID, result.ToLogString());
                return result;
            }
        }

        public static void InsertDEALicenses(string providerID, string deaLicenseID, string deaNumber, DateTime deaExpirationDate, string deaIAllowed, string deaIIAllowed, string deaIIIAllowed, string deaIVAllowed, string deaVAllowed, DeaLicenseType deaLicenseTypeId, string userID, string ShieldUserName, string SecurityToken, string AppInstanceId, ConnectionStringPointer dbID)
        {
            using (var timer = logger.StartTimer("InsertDEALicenses"))
            {
                ePrescribeSvc.DEALicense deaLicense = new ePrescribeSvc.DEALicense()
                {
                    ProviderID = providerID,
                    DEALicenseID = Guid.NewGuid().ToString(),
                    DEANumber = deaNumber,
                    DEAExpirationDate = deaExpirationDate,
                    DEAIIAllowed = bool.Parse(deaIIAllowed),
                    DEAIIIAllowed = bool.Parse(deaIIIAllowed),
                    DEAIVAllowed = bool.Parse(deaIVAllowed),
                    DEAVAllowed = bool.Parse(deaVAllowed),
                    DeaLicenseTypeId = (ePrescribeSvc.DeaLicenseType)deaLicenseTypeId,
                };

                var deaLicenseRequest = ServiceRequest<SaveDEALicenseRequest>();
                deaLicenseRequest.deaLicense = deaLicense;
                deaLicenseRequest.userId = userID;
                deaLicenseRequest.AppInstanceId = AppInstanceId;
                deaLicenseRequest.ShieldUserName = ShieldUserName;
                deaLicenseRequest.SecurityToken = SecurityToken;

                timer.Message = string.Format("<Request>{0}</Request>", deaLicenseRequest.ToLogString());

                GetEps().UpdateDEALicenses(deaLicenseRequest, (ePrescribeSvc.ConnectionStringPointer)dbID);
            }
        }

        public static void CompareAllValues()
        {

        }
        public static void UpdateDEALicenses(string oldProviderID, string oldDEALicenseID, string DEANumber, DateTime DEAExpirationDate, string DEAIAllowed, string DEAIIAllowed, string DEAIIIAllowed, string DEAIVAllowed, string DEAVAllowed, DeaLicenseType DEALicenseTypeID, string UserID, string AppInstanceId, string ShieldUserName, string SecurityToken, ConnectionStringPointer dbID)
        {
            using (var timer = logger.StartTimer("UpdateDEALicenses"))
            {
                ePrescribeSvc.DEALicense deaLicense = new ePrescribeSvc.DEALicense()
                {
                    ProviderID = oldProviderID,
                    DEALicenseID = oldDEALicenseID,
                    DEANumber = DEANumber,
                    DEAExpirationDate = DEAExpirationDate,
                    DEAIIAllowed = bool.Parse(DEAIIAllowed),
                    DEAIIIAllowed = bool.Parse(DEAIIIAllowed),
                    DEAIVAllowed = bool.Parse(DEAIVAllowed),
                    DEAVAllowed = bool.Parse(DEAVAllowed),
                    DeaLicenseTypeId = (ePrescribeSvc.DeaLicenseType)DEALicenseTypeID
                };

                var deaLicenseRequest = ServiceRequest<SaveDEALicenseRequest>();
                deaLicenseRequest.deaLicense = deaLicense;
                deaLicenseRequest.userId = UserID;
                deaLicenseRequest.AppInstanceId = AppInstanceId;
                deaLicenseRequest.ShieldUserName = ShieldUserName;
                deaLicenseRequest.SecurityToken = SecurityToken;

                timer.Message = string.Format("<Request>{0}</Request>", deaLicenseRequest.ToLogString());

                GetEps().UpdateDEALicenses(deaLicenseRequest, (ePrescribeSvc.ConnectionStringPointer)dbID);
            }
        }

        public static void DeleteProviderDEALicense(string providerID, string deaLicenseID, string auditUserID, string appInstanceId, string shieldUserName, string shieldToken, ConnectionStringPointer dbID)
        {
            using (var timer = logger.StartTimer("DeleteProviderDEALicense"))
            {
                ePrescribeSvc.DEALicense deaLicense = new ePrescribeSvc.DEALicense()
                {
                    ProviderID = providerID,
                    DEALicenseID = deaLicenseID
                };

                var deaLicenseRequest = ServiceRequest<SaveDEALicenseRequest>();
                deaLicenseRequest.deaLicense = deaLicense;
                deaLicenseRequest.userId = auditUserID;
                deaLicenseRequest.AppInstanceId = appInstanceId;
                deaLicenseRequest.ShieldUserName = shieldUserName;
                deaLicenseRequest.SecurityToken = shieldToken;

                timer.Message = string.Format("<Request><ProviderID>{0}</ProviderID><DeaLicenseID>{1}</DeaLicenseID></Request>", providerID, deaLicenseID);

                GetEps().DeleteDEALicense(deaLicenseRequest, (ePrescribeSvc.ConnectionStringPointer)dbID);
            }
        }

        #endregion
        public static int GetAdminCountForLicense(string licenseID, ConnectionStringPointer dbID)
        {
            using (var timer = logger.StartTimer("GetAdminCountForLicense"))
            {
                int intAdminCount = 0;
                object adminCount = GetEps().GetAdminCountForLicense(licenseID, (ePrescribeSvc.ConnectionStringPointer)dbID);

                if (adminCount != null)
                    intAdminCount = (int)adminCount;

                timer.Message = string.Format("<Request>{0}</Request><Response>{1}</Response>", licenseID, intAdminCount);

                return intAdminCount;

            }
        }
        public static ePrescribeSvc.DenialReason[] GetDenialReasons(Constants.PrescriptionTaskType taskType, ConnectionStringPointer dbId)
        {
            var request = ServiceRequest<ePrescribeSvc.GetDenialReasonsRequest>();

            request.TaskType = ConvertEnumFromCommonToEPS(taskType);

            request.DbID = (ePrescribeSvc.ConnectionStringPointer?)dbId;

            using (var timer = logger.StartTimer("GetDenialReasons"))
            {
                var response = GetEps().GetDenialReasons(request).DenialReasons;
                timer.Message = $"<Request>{request.ToLogString()}</Request><Response>{response.ToLogString()}</Response>";

                return response;
            }
        }

        public static ePrescribeSvc.Medication LoadMedicationByDDIAndCoverage(string DDI, string coverageId, string formularyID,
            string otcCoverage, int genericDrugPolicy, int unlistedDrugPolicy, ConnectionStringPointer dbID)
        {
            using (var timer = logger.StartTimer("LoadMedicationByDDIAndCoverage"))
            {
                return GetEps().LoadMedicationByDDIAndCoverage(DDI, coverageId, formularyID, otcCoverage, genericDrugPolicy, unlistedDrugPolicy, (ePrescribeSvc.ConnectionStringPointer)dbID).Medication;
            }
        }

        public static ePrescribeSvc.Medication[] LoadMedicationByName(string medName, ConnectionStringPointer dbID)
        {
            using (var timer = logger.StartTimer("LoadMedicationByName"))
            {
                LoadMedicationResponse response = null;

                var loadMedicationRequest = ServiceRequest<LoadMedicationRequest>();
                loadMedicationRequest.Name = medName;
                loadMedicationRequest.dbID = (ePrescribeSvc.ConnectionStringPointer)dbID;

                response = GetEps().LoadMedicationByName(loadMedicationRequest);

                return response != null ? response.Medications : null;
            }
        }
        public static void InsertAllProvidersEnrollmentInLicense(string licenseID, int taskType, ConnectionStringPointer dbID)
        {
            using (var timer = logger.StartTimer("InsertAllProvidersEnrollmentInLicense"))
            {
                var insertAllUsersInLicenseForHubEnrollmentReq = ServiceRequest<InsertAllUsersInLicenseForHubEnrollmentRequest>();
                insertAllUsersInLicenseForHubEnrollmentReq.reqAuditInfo = new AuditInfo
                {
                    LicenseID = licenseID,
                };
                insertAllUsersInLicenseForHubEnrollmentReq.DbID = (ePrescribeSvc.ConnectionStringPointer)dbID;
                insertAllUsersInLicenseForHubEnrollmentReq.TaskType = taskType;

                GetEps().InsertAllUsersInLicenseForHubEnrollment(insertAllUsersInLicenseForHubEnrollmentReq);

                timer.Message = string.Format("<Request>{0}</Request>", insertAllUsersInLicenseForHubEnrollmentReq.ToLogString());
            }
        }

        public static void UpdatePrescriptionStatus(string LicenseId, string rxID, int lineNumber, Constants.PrescriptionStatus prescriptionStatus, string userID, ConnectionStringPointer dbID)
        {
            using (var timer = logger.StartTimer("UpdatePrescriptionStatus"))
            {
                var request = ServiceRequest<RxRequest>();
                request.Rx = new ePrescribeSvc.Rx
                {
                    LicenseID = LicenseId,
                    RxID = rxID,
                    Status = (PrescriptionStatus)prescriptionStatus
                };
                request.reqAuditInfo = new AuditInfo
                {
                    UserID = userID
                };
                request.DbID = (ePrescribeSvc.ConnectionStringPointer)dbID;

                timer.Message = string.Format("<Request>{0}</Request>", request.ToLogString());

                GetEps().UpdatePrescriptionStatus(request);
            }
        }
        void IEPSBroker.UpdatePrescriptionStatus(string LicenseId, string rxID, int lineNumber, Constants.PrescriptionStatus prescriptionStatus, string userID, ConnectionStringPointer dbID)
        {
            UpdatePrescriptionStatus(LicenseId, rxID, lineNumber, prescriptionStatus, userID, dbID);
        }

        public static string GetClinicalViewerSSOLink(string patientID, string systemOID, string userFirstName, string userLastName, string userName, string userRole, string communityURL)
        {
            using (var timer = logger.StartTimer("GetClinicalViewerSSOLink"))
            {
                var request = ServiceRequest<ClinicalViewerLaunchRequest>();
                request.PatientID = patientID;
                request.SystemOID = systemOID;
                request.UserFirstName = userFirstName;
                request.UserLastName = userLastName;
                request.UserName = userName;
                request.UserRole = userRole;
                request.CommunityURL = communityURL;

                ClinicalViewerLaunchResponse response = new ClinicalViewerLaunchResponse();
                response = GetEps().LaunchClinicalViewer(request);

                timer.Message = string.Format("<Request>{0}</Request><Response>{1}</Response>", request.ToLogString(), response.ToLogString());

                return response.HtmlSource;
            }
        }


        #region EPCS methods

        /// <summary>
        /// Calls EPS to sign each script message entry in the scriptMessageIDs. Inserts signed script message to database.
        /// </summary>
        public static SignPrescriptionsResponse SignPrescriptionAndInsertSignedScriptMessage(string securityToken, string[] scriptMessageIDs, string licenseID, string userID, string shieldUserName, string eRxUsername, string identitySecurityToken, string appInstanceId, Guid shieldCorrelationId, ConnectionStringPointer dbID)
        {
            using (var timer = logger.StartTimer("SignPrescriptionAndInsertSignedScriptMessage"))
            {
                var signingRequest = ServiceRequest<SignPrescriptionsRequest>();
                signingRequest.DbID = (ePrescribeSvc.ConnectionStringPointer)dbID;
                signingRequest.ScriptMessageIDs = scriptMessageIDs;
                signingRequest.OtpSecurityToken = securityToken;
                signingRequest.ShieldUserName = shieldUserName;
                signingRequest.EprescribeUserName = eRxUsername;
                signingRequest.IdentitySecurityToken = identitySecurityToken;
                signingRequest.CorrelationId = shieldCorrelationId;
                signingRequest.appInstanceId = appInstanceId;
                signingRequest.reqAuditInfo = new AuditInfo
                {
                    LicenseID = licenseID,
                    UserID = userID
                };

                var response = GetEps().SignPrescriptionAndInsertSignedScriptMessage(signingRequest);
                timer.Message = string.Format("<Request>{0}</Request><Response>{1}</Response>", signingRequest.ToLogString(), response.ToLogString());

                return response;
            }
        }

        public static GetShieldOTPFormsResponse GetOtpFormsForSigning(string shieldSecurityToken, string ipAddress, string externalAppInstanceID, string shieldUsername)
        {
            using (var timer = logger.StartTimer("GetOtpFormsForSigning"))
            {
                var request = ServiceRequest<GetShieldOTPFormsRequest>();
                request.SecurityToken = shieldSecurityToken;
                request.IPAddress = ipAddress;
                request.AppInstanceId = externalAppInstanceID;
                request.UserName = shieldUsername;

                var response = GetEps().GetOtpFormsForSigning(request);
                timer.Message = string.Format("<Request>{0}</Request><Response>{1}</Response>", request.ToLogString(), response.ToLogString());

                return response;
            }
        }

        /// <summary>
        /// Calls Verizon to trigger and OTP to be delivered to the user (e.g. via SMS text or email)
        /// </summary>
        public static bool RequestOTP(string licenseID, string userGUID, string shieldUserName, OTPForm otpForm, string transactionID, string ipAddress,
            string securityToken, string externalAppInstanceID, ConnectionStringPointer dbID, out string exceptionID)
        {
            using (var timer = logger.StartTimer("RequestOTP"))
            {
                exceptionID = null;

                var req = ServiceRequest<GetShieldSendOTPRequest>();
                req.reqAuditInfo = new AuditInfo
                {
                    LicenseID = licenseID,
                    UserID = userGUID
                };
                req.ShieldUserName = shieldUserName;
                req.FormId = otpForm.FormId;
                req.Description = otpForm.Description;
                req.AllowsRequest = otpForm.AllowsRequest;
                req.DeviceType = otpForm.DeviceType;
                req.DisplayName = otpForm.DisplayName;
                req.TransactionId = transactionID;
                req.IPAddress = ipAddress;
                req.SecurityToken = securityToken;
                req.AppInstanceId = externalAppInstanceID;
                req.Type = otpForm.Type.ToString();
                req.SubType = otpForm.SubType;
                req.DbID = (ePrescribeSvc.ConnectionStringPointer)dbID;

                var response = GetEps().RequestOTP(req);

                timer.Message = string.Format("<Request>{0}</Request><Response>{1}</Response>", req.ToLogString(), response.ToLogString());

                if (!response.Success)
                {
                    exceptionID = response.ExceptionID;
                }

                return response.Success;
            }
        }

        public static void SetDEARegistrantStatus(string shieldUserName, bool isDEAregistrant, ConnectionStringPointer dbID)
        {
            using (var timer = logger.StartTimer("SetDEARegistrantStatus"))
            {
                var request = ServiceRequest<SetDEARegistrantStatusRequest>();
                request.ShieldLoginID = shieldUserName;
                request.IsDEARegistrant = isDEAregistrant;
                request.DbID = (ePrescribeSvc.ConnectionStringPointer)dbID;

                timer.Message = string.Format("<Request>{0}</Request>", request.ToLogString());

                GetEps().SetDEARegistrantStatus(request);
            }
        }

        #endregion

        #region Message Queue Transaction
        /// <summary>
        /// Load Message Queue Transactions
        /// </summary>
        /// <param name="licenseID"></param>
        /// <param name="siteID"></param>
        /// <param name="view"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="authorizerID"></param>
        /// <param name="originatorID"></param>
        /// <param name="patientID"></param>
        /// <param name="sortExpression"></param>
        /// <param name="dbID"></param>
        /// <returns></returns>
        public static ePrescribeSvc.MessageQueue[] LoadMessageQueueTx(
            string licenseID,
            int siteID,
            string statusID,
            DateTime startDate,
            DateTime endDate,
            string authorizerID,
            string originatorID,
            string patientID,
            string sortExpression,
            ConnectionStringPointer dbID,
            string userID,
            UserCategory userType
            )
        {
            using (var timer = logger.StartTimer("LoadMessageQueueTx"))
            {
                LoadMessageQueueTxResponse response = null;

                var request = ServiceRequest<LoadMessageQueueTxRequest>();
                request.LicenseID = licenseID;
                request.SiteID = siteID;
                request.StatusID = int.Parse(statusID);
                request.StartDate = startDate;
                request.EndDate = endDate;
                request.AuthorizerID = authorizerID;
                request.OriginatorID = originatorID;
                request.PatientID = patientID;
                request.dbID = (ePrescribeSvc.ConnectionStringPointer)dbID;
                request.userID = userID;
                request.userType = (int)userType;

                response = GetEps().LoadMessageQueueTx(request);
                timer.Message = string.Format("<Request>{0}</Request><Response>{1}</Response>", request.ToLogString(), response.ToLogString());

                return (response != null ? response.MessageQueueItems : null);
            }
        }
        #endregion


        public static bool GetEnterpriseePAStatusByPartnerID(string partnerId)
        {
            using (var timer = logger.StartTimer("GetEnterpriseePAStatusByPartnerID"))
            {
                var enterpriseePAStatusRequest = ServiceRequest<GetEnterpriseePAStatusRequest>();
                enterpriseePAStatusRequest.PartnerID = partnerId;

                var response = GetEps().GetEnterpriseePAStatusByPartnerID(enterpriseePAStatusRequest);
                timer.Message = string.Format("<Request>{0}</Request><Response>{1}</Response>", enterpriseePAStatusRequest.ToLogString(), response.ToLogString());

                return response;
            }
        }

        #region UserManagementPortal
        public static bool UpdateLicenseInUMPByLicenseID(string licenseID, ePrescribeSvc.ConnectionStringPointer dbID)
        {
            using (var timer = logger.StartTimer("UpdateLicenseInUMPByLicenseID"))
            {
                var response = GetEps().UpdateLicenseInUMPByLicenseID(licenseID, dbID);
                timer.Message = string.Format("<Request>{0}</Request><Response>{1}</Response>", licenseID, response.ToLogString());

                return response;
            }
        }

        public static bool AddUserToUMP(string licenseID, string username, string firstName, string lastName, string email, string userGuid, bool isAdmin, ConnectionStringPointer dbID)
        {
            using (var timer = logger.StartTimer("AddUserToUMP"))
            {
                var request = ServiceRequest<AddUserToUMPRequest>();
                request.LicenseID = licenseID;
                request.UserName = username;
                request.FirstName = firstName;
                request.LastName = lastName;
                request.EmailAddress = email;
                request.UserGUID = userGuid;
                request.IsAdmin = isAdmin;
                request.DbID = (ePrescribeSvc.ConnectionStringPointer)dbID;

                var response = GetEps().AddUserToUMP(request).Success;
                timer.Message = string.Format("<Request>{0}</Request><Response>{1}</Response>", request.ToLogString(), response.ToLogString());

                return response;
            }
        }

        public static bool UpdateUserInUMP(string licenseID, string userName, string firstName, string lastName, string email, bool isActive, string userGuid, bool isAdmin, string npi, string currentNpi, ConnectionStringPointer dbID)
        {
            using (var timer = logger.StartTimer("UpdateUserInUMP"))
            {
                var request = ServiceRequest<UpdateUserInUMPRequest>();
                request.LicenseID = licenseID;
                request.UserName = userName;
                request.FirstName = firstName;
                request.LastName = lastName;
                request.EmailAddress = email;
                request.IsActive = isActive;
                request.UserGUID = userGuid;
                request.IsAdmin = isAdmin;
                request.NewNpi = npi;
                request.CurrentNpi = currentNpi;
                request.DbID = (ePrescribeSvc.ConnectionStringPointer?)dbID;

                var response = GetEps().UpdateUserInUMP(request).Success;

                timer.Message = string.Format("<Request>{0}</Request><Response>{1}</Response>", request.ToLogString(), response.ToLogString());

                return response;
            }
        }

        #endregion
        #region DXC ECoupon
        public static bool IsECouponAvailableForDDI(string ddi, ConnectionStringPointer dbId)
        {
            using (var timer = logger.StartTimer("IsECouponAvailableForDDI"))
            {
                var response = GetEps().IsECouponAvailableForDDI(ddi, (ePrescribeSvc.ConnectionStringPointer)dbId);
                timer.Message = $"<Request>{ddi}</Request><Response>{response.ToLogString()}</Response>";

                return response;
            }
        }

        public static bool RequestECoupon(string rxId, Constants.ECouponWorkFlowType eCouponWorkFlowType, string licenseId, string userId, ConnectionStringPointer dbId)
        {
            using (var timer = logger.StartTimer("RequestECoupon"))
            {
                var eCouponRequest = ServiceRequest<ECouponRequest>();
                eCouponRequest.DbID = (ePrescribeSvc.ConnectionStringPointer)dbId;
                eCouponRequest.reqAuditInfo = new AuditInfo { LicenseID = licenseId, UserID = userId };
                eCouponRequest.RxID = rxId;
                eCouponRequest.ECouponWorkFlowType = (int)eCouponWorkFlowType;

                var response = GetEps().GetECoupon(eCouponRequest).Success;
                timer.Message = $"<Request>{eCouponRequest.ToLogString()}</Request><Response>{response.ToLogString()}</Response>";

                return response;
            }
        }

        public static GetECouponDetailByRxIDResponse GetECouponDetailByRxID(string rxId, string licenseId, string userId, ConnectionStringPointer dbId)
        {
            using (var timer = logger.StartTimer("GetECouponDetailByRxID"))
            {
                var eCouponRequest = ServiceRequest<ECouponRequest>();
                eCouponRequest.DbID = (ePrescribeSvc.ConnectionStringPointer)dbId;
                eCouponRequest.reqAuditInfo = new AuditInfo { LicenseID = licenseId, UserID = userId };
                eCouponRequest.RxID = rxId;

                var response = GetEps().GetECouponDetailByRxID(eCouponRequest);
                timer.Message = $"<Request>{eCouponRequest.ToLogString()}</Request><Response>{response.ToLogString()}</Response>";

                return response;
            }
        }


        public static bool ConfirmECouponDelivery(string eCouponDetailId, Constants.ECouponOfferDeliveryType deliveryType, string licenseId, string userId, ConnectionStringPointer dbId)
        {
            using (var timer = logger.StartTimer("ConfirmECouponDelivery"))
            {
                var eCouponRequest = ServiceRequest<ECouponRequest>();
                eCouponRequest.DbID = (ePrescribeSvc.ConnectionStringPointer)dbId;
                eCouponRequest.reqAuditInfo = new AuditInfo { LicenseID = licenseId, UserID = userId };
                eCouponRequest.ECouponDetailID = Convert.ToInt64(eCouponDetailId.Trim());
                eCouponRequest.ECouponDeliveryTypeID = (int)deliveryType;

                var response = GetEps().ECouponConfirmOffer(eCouponRequest).Success;

                timer.Message = $"<Request>{eCouponRequest.ToLogString()}</Request><Response>{response.ToLogString()}</Response>";

                return response;
            }
        }

        public static ePrescribeSvc.PrescriptionTaskType ConvertEnumFromCommonToEPS(Constants.PrescriptionTaskType taskType)
        {
            ePrescribeSvc.PrescriptionTaskType castedTaskType;
            Enum.TryParse(Convert.ToString(taskType), out castedTaskType);
            return castedTaskType;
        }

        public static GetECouponDetailsLiteByIDResponse GetECouponDetailsLiteByID(string eCouponDetailId, string licenseId, string userId, ConnectionStringPointer dbId)
        {
            using (var timer = logger.StartTimer("GetECouponDetailsLiteByID"))
            {
                var eCouponRequest = ServiceRequest<ECouponRequest>();
                eCouponRequest.DbID = (ePrescribeSvc.ConnectionStringPointer)dbId;
                eCouponRequest.reqAuditInfo = new AuditInfo { LicenseID = licenseId, UserID = userId };
                eCouponRequest.ECouponDetailID = Convert.ToInt64(eCouponDetailId.Trim());

                var response = GetEps().GetECouponDetailsLiteByID(eCouponRequest);
                timer.Message = $"<Request>{eCouponRequest.ToLogString()}</Request><Response>{response.ToLogString()}</Response>";

                return response;
            }
        }

        public static bool AreECouponNotesToRxPharmacyNotesUpdated(string rxID, string eCouponNotes, string licenseId, string userId, ConnectionStringPointer dbId)
        {
            using (var timer = logger.StartTimer("UpdateECouponNotesToRxPharmacyNotes"))
            {
                var eCouponRequest = ServiceRequest<ECouponRequest>();
                eCouponRequest.RxID = rxID;
                eCouponRequest.ECouponPharmacyNotes = eCouponNotes;
                eCouponRequest.DbID = (ePrescribeSvc.ConnectionStringPointer)dbId;
                eCouponRequest.reqAuditInfo = new AuditInfo { LicenseID = licenseId, UserID = userId };

                var response = GetEps().AreECouponNotesToRxPharmacyNotesUpdated(eCouponRequest);
                timer.Message = $"<Request>{eCouponRequest.ToLogString()}</Request><Response>{response.ToLogString()}</Response>";

                return response;
            }
        }

        public static ECouponRefillPrintResponse ECouponPrintRefillRequest(string rxID, string ddi, bool canApplyFinancialOffers, Constants.AutoSendCoupons autoSendCoupons, ConnectionStringPointer dbID)
        {
            using (var timer = logger.StartTimer("ECouponPrintRefillRequest"))
            {
                var request = new ECouponRefillRequest();
                var response = new ECouponRefillPrintResponse();
                request.RxID = rxID;
                request.DDID = ddi;
                request.DbID = (ePrescribeSvc.ConnectionStringPointer)dbID;
                request.CanApplyFinancialOffers = canApplyFinancialOffers;
                request.AutoSendCoupons = (ePrescribeSvc.AutoSendCoupons)autoSendCoupons;

                response = GetEps().ECouponRefillPrintRequest(request);
                timer.Message = $"<Request>{request.ToLogString()}</Request><Response>{response.ToLogString()}</Response>";

                return response;
            }
        }
        #endregion

        #region RxInfo
        public static bool IsRxInfoAvailableForDDI(string ddi, ConnectionStringPointer dbId)
        {
            using (var timer = logger.StartTimer("IsRxInfoAvailableForDDI"))
            {
                var response = GetEps().IsRxInfoAvailableForDDI(ddi, (ePrescribeSvc.ConnectionStringPointer)dbId);
                timer.Message = string.Format("<Request>{0}</Request><Response>{1}</Response>", ddi, response.ToLogString());

                return response;
            }
        }

        public static bool RequestRxInfo(string rxId, string licenseId, string userId, ConnectionStringPointer dbId)
        {
            using (var timer = logger.StartTimer("RequestRxInfo"))
            {
                var dxcClientUserRequest = ServiceRequest<RxInfoRequest>();
                dxcClientUserRequest.DbID = (ePrescribeSvc.ConnectionStringPointer)dbId;
                dxcClientUserRequest.reqAuditInfo = new AuditInfo { LicenseID = licenseId, UserID = userId };
                dxcClientUserRequest.rxID = rxId;


                var response = GetEps().GetRxInfo(dxcClientUserRequest).Success;

                return response;
            }
        }


        public static GetRxInfoDetailsLiteByRxIDResponse GetRxInfoDetailsLiteByRxID(string rxId, string licenseId, string userId, ConnectionStringPointer dbId)
        {
            using (var timer = logger.StartTimer("GetRxInfoDetailsLiteByRxID"))
            {
                var getRxInfoDetailsLiteByRxIDRequest = ServiceRequest<GetRxInfoDetailsLiteByRxIDRequest>();
                getRxInfoDetailsLiteByRxIDRequest.DbID = (ePrescribeSvc.ConnectionStringPointer)dbId;
                getRxInfoDetailsLiteByRxIDRequest.reqAuditInfo = new AuditInfo { LicenseID = licenseId, UserID = userId };
                getRxInfoDetailsLiteByRxIDRequest.RxId = rxId;

                var response = GetEps().GetRxInfoDetailsLiteByRxID(getRxInfoDetailsLiteByRxIDRequest);
                timer.Message = string.Format("<Request>{0}</Request><Response>{1}</Response>", getRxInfoDetailsLiteByRxIDRequest.ToLogString(), response.ToLogString());

                return response;
            }
        }

        public static string GetRxInfoHtmlContent(string rxInfoDetailId, ConnectionStringPointer dbId)
        {
            using (var timer = logger.StartTimer("GetRxInfoHtmlContent"))
            {
                var getRxInfoHtmlContentRequest = ServiceRequest<GetRxInfoHtmlContentRequest>();
                getRxInfoHtmlContentRequest.DbID = (ePrescribeSvc.ConnectionStringPointer)dbId;
                getRxInfoHtmlContentRequest.reqAuditInfo = new AuditInfo();
                getRxInfoHtmlContentRequest.RxInfoDetailId = rxInfoDetailId;

                var response = GetEps().GetRxInfoHtmlContent(getRxInfoHtmlContentRequest);
                timer.Message = string.Format("<Request>{0}</Request><Response>{1}</Response>", getRxInfoHtmlContentRequest.ToLogString(), response.ToLogString());

                return response;
            }
        }

        public static bool ConfirmRxInfoDelivery(string rxInfoDetailsId, Constants.RxInfoOfferDeliveryType deliveryType, string licenseId, string userId, ConnectionStringPointer dbId)
        {
            using (var timer = logger.StartTimer("ConfirmRxInfoDelivery"))
            {
                var dxcClientUserRequest = ServiceRequest<RxInfoRequest>();
                dxcClientUserRequest.DbID = (ePrescribeSvc.ConnectionStringPointer)dbId;
                dxcClientUserRequest.reqAuditInfo = new AuditInfo { LicenseID = licenseId, UserID = userId };
                dxcClientUserRequest.rxInfoDetailsID = Convert.ToInt64(rxInfoDetailsId.Trim());
                dxcClientUserRequest.rxInfoDeliveryTypeID = (int)deliveryType;

                var response = GetEps().RxInfoConfirmOffer(dxcClientUserRequest).Success;

                timer.Message = string.Format("<Request>{0}</Request><Response>{1}</Response>", dxcClientUserRequest.ToLogString(), response.ToLogString());

                return response;
            }
        }

        #endregion

        #region SMSAlert
        public static ePrescribeSvc.ePrescribeSvcResponse SendMessageTwilio(string toPhoneNumber, string message)
        {
            using (var timer = logger.StartTimer("SendMessageTwilio"))
            {
                var smsAlertsRequest = ServiceRequest<SMSAlertsRequest>();
                smsAlertsRequest.ToPhoneNumber = toPhoneNumber;
                smsAlertsRequest.Message = message;
                var smsAlertsresponse = GetEps().SendMessageTwilio(smsAlertsRequest);
                timer.Message = $"<Request>{smsAlertsRequest.ToLogString()}</Request><Response>{smsAlertsresponse.ToLogString()}</Response>";
                return smsAlertsresponse;
            }
        }

        public static bool IsSMSAlertDailyCountReached()
        {
            using (var timer = logger.StartTimer("IsSMSAlertDailyCountReached"))
            {
                var erxSvcRequest = ServiceRequest<ePrescribeSvcRequest>();
                var isSMSAlertDailyCountReached = GetEps().IsSMSAlertDailyCountReached(erxSvcRequest);
                timer.Message = $"<Request></Request><Response>{isSMSAlertDailyCountReached.ToLogString()}</Response>";
                return isSMSAlertDailyCountReached;
            }
        }

        #endregion

        bool IEPSBroker.IsUserLoggingEnabled(Guid userGuid)
        {
            return IsUserLoggingEnabled(userGuid);
        }
        public static bool IsUserLoggingEnabled(Guid userGuid)
        {
            UserLoggingEnabledRequest request = new UserLoggingEnabledRequest() { UserGuid = userGuid };

            var response = GetEps().IsUserLoggingEnabled(request);

            return response;
        }


        public static bool IsUserLoggingEnabled(string userName)
        {
            //using (var timer = logger.StartTimer("IsUserLoggingEnabled2"))
            //{
            UserLoggingEnabledRequest request = new UserLoggingEnabledRequest() { UserName = userName };

            var response = GetEps().IsUserLoggingEnabled(request);
            // timer.Message = string.Format("<Request>{0}</Request><Response>{1}</Response>", request.ToLogString(), response.ToLogString());

            return response;
            //}
        }

        public static List<ProviderWithShieldTraits> GetAllProvidersAndTraitsForLicense(string licenseGuid, string deaNumber, string appInstanceId, string securityToken)
        {
            using (var timer = logger.StartTimer("GetAllProvidersAndTraitsForLicense"))
            {
                var request = ServiceRequest<GetAllProvidersAndTraitsForLicenseRequest>();
                request.reqAuditInfo = new AuditInfo { LicenseID = licenseGuid };
                request.AppInstanceId = appInstanceId;
                request.SecurityToken = securityToken;
                request.DeaNumber = deaNumber;

                var response = GetEps().GetAllProvidersAndTraitsForLicense(request);

                timer.Message = string.Format("<Request>{0}</Request><Response>{1}</Response>", request.ToLogString(), response.ToLogString());

                return response.Providers.ToList();
            }
        }

        public static bool SetTraitForMultipleProviders(List<UserNameWithUserGuidPair> providerUserNames, ShieldTraitInfo traitInfo, string appInstanceId, string identitySecurityToken, string otpToken, string shieldSecurityToken, string sessionLicenseID, string sessionUserID, bool isInstitutional)
        {
            using (var timer = logger.StartTimer("SetTraitForMultipleProviders"))
            {
                var request = ServiceRequest<SetProviderTraitsRequest>();
                request.Users = providerUserNames.ToArray();
                request.AppInstanceId = appInstanceId;
                request.IdentitySecurityToken = identitySecurityToken;
                request.ShieldTraitName = traitInfo.TraitName;
                request.ShieldTraitValue = traitInfo.TraitValueEnum;
                request.OtpToken = otpToken;
                request.SecurityToken = shieldSecurityToken;
                request.reqAuditInfo = new AuditInfo
                {
                    UserID = sessionUserID,
                    LicenseID = sessionLicenseID
                };
                request.IsInstitutional = isInstitutional;

                var response = GetEps().SetProviderTraits(request);

                timer.Message = string.Format("<Request>{0}</Request><Response>{1}</Response>", request.ToLogString(), response.ToLogString());

                return response.Success;
            }
        }

        public static bool SetTraitForUser(string shieldUserName, ShieldTraitInfo traitInfo,
         string appInstanceId, string securityToken)
        {
            using (var timer = logger.StartTimer("SetTraitForUser"))
            {
                var request = ServiceRequest<SetShieldUserTraitRequest>();
                request.ShieldUserName = shieldUserName;
                request.AppInstanceId = appInstanceId;
                request.SecurityToken = securityToken;
                request.TraitName = traitInfo.TraitName;
                request.TraitValue = traitInfo.TraitValueEnum;

                var response = GetEps().SetShieldUserTraitUsingUserToken(request);

                timer.Message = string.Format("<Request>{0}</Request><Response>{1}</Response>", request.ToLogString(), response.ToLogString());

                return response.Success;
            }
        }

        public static List<ShieldTraitInfo> GetShieldUserTraits(string shieldUserName, List<ShieldTraitName> traitNames,
         string appInstanceId, string securityToken)
        {
            using (var timer = logger.StartTimer("GetShieldUserTraits"))
            {
                var request = ServiceRequest<GetShieldUserTraitsRequest>();
                request.ShieldUserName = shieldUserName;
                request.AppInstanceId = appInstanceId;
                request.SecurityToken = securityToken;
                request.ShieldTraits = traitNames.ToArray();
                request.reqAuditInfo = new AuditInfo { UserName = PageState.GetStringOrEmpty("EprescribeUsername") };

                var response = GetEps().GetShieldUserTraitUsingUserToken(request);

                timer.Message = string.Format("<Request>{0}</Request><Response>{1}</Response>", request.ToLogString(), response.ToLogString());

                if (response.ShieldTraits != null)
                {
                    return response.ShieldTraits.ToList();
                }

                return new List<ShieldTraitInfo>();
            }
        }
        public string GetIdProofingUrl(ePrescribeSvc.ShieldTenantIDProofingModel idProofingMode, string userName)
        {
            using (var timer = logger.StartTimer("GetIdProofingUrl"))
            {
                var request = ServiceRequest<GetIdProofingUrlRequest>();

               request.IdProofingMode = idProofingMode; 

               request.reqAuditInfo = new AuditInfo { UserName = userName };

                timer.Message = $"<Request>{request.ToLogString()}</Request>";

                var response = GetEps().GetIdProofingUrl(request);

                timer.Message += $"<Response>{response.ToLogString()}</Response>";

                return response.IdProofingUrl;
            }
        }

        GetUserShieldCspStatusInfoResponse IEPSBroker.GetUserShieldCspStatusInfo(ConnectionStringPointer dbID, string identityToken, string shieldExternalTenantId, string userId, string licenseId)
        {
            return GetUserShieldCspStatusInfo(dbID, identityToken, shieldExternalTenantId, userId, licenseId);
        }

        public static GetUserShieldCspStatusInfoResponse GetUserShieldCspStatusInfo(ConnectionStringPointer dbID, string identityToken, string shieldExternalTenantId, string userId, string licenseId)
        {
            using (var timer = logger.StartTimer("GetUserShieldCspStatusInfo"))
            {
                var request = ServiceRequest<GetUserShieldCspStatusInfoRequest>();
                request.DbID = (ePrescribeSvc.ConnectionStringPointer)dbID;
                request.IdentityToken = identityToken;
                request.ShieldExternalTenantId = shieldExternalTenantId;
                request.reqAuditInfo = new AuditInfo { LicenseID = licenseId, UserID = userId };
                var response = GetEps().GetUserShieldCspStatusInfo(request);
                //var response = new GetUserShieldCspStatusInfoResponse();
                //response.IsTenantShieldCSPAppInstanceAvailable = true;
                //response.IsUserEnrolledToZentryCsp = true;
                //response.IsUserEnrolledWithIdMe = false;
                timer.Message = string.Format("<Request>{0}</Request><Response>{1}</Response>", request.ToLogString(), response.ToLogString());
                return response;
            }

        }

        public static GetShieldOTPFormsResponse GetOTPForms(string securityToken, string appInstanceId)
        {
            using (var timer = logger.StartTimer("GetOTPForms"))
            {
                var request = ServiceRequest<GetShieldOTPFormsRequest>();
                request.SecurityToken = securityToken;
                request.AppInstanceId = appInstanceId;

                var response = GetEps().GetShieldOtpForms(request);

                timer.Message = string.Format("<Request>{0}</Request><Response>{1}</Response>", request, response);

                return response;
            }
        }

        public static GetShieldSendOTPResponse SendOTPRequest(string securityToken, string appInstanceId, string transactionId, ePrescribeSvc.OTPForm otpForm)
        {
            using (var timer = logger.StartTimer("SendOTPRequest"))
            {
                var request = ServiceRequest<GetShieldSendOTPRequest>();
                request.SecurityToken = securityToken;
                request.AppInstanceId = appInstanceId;
                request.TransactionId = transactionId;
                request.AllowsRequest = otpForm.AllowsRequest;
                request.Description = otpForm.Description;
                request.DeviceType = otpForm.DeviceType;
                request.DisplayName = otpForm.DisplayName;
                request.FormId = otpForm.FormId;
                request.SubType = otpForm.SubType;
                request.Type = otpForm.Type.ToString();

                var response = GetEps().GetShieldSendOtp(request);

                timer.Message = string.Format("<Request>{0}</Request><Response>{1}</Response>", request, response);

                return response;
            }
        }

        public static GetShieldAuthenicateOTPFormsResponse AuthenicateOTP(string transactionId, string otp, string identityName, OTPForm otpForm, string userName, string password, string sessionLicenseID, string sessionUserID)
        {
            using (var timer = logger.StartTimer("AuthenicateOTP"))
            {
                var request = ServiceRequest<GetShieldAuthenicateOTPRequest>();
                request.IdentityName = identityName;
                request.TransactionId = transactionId;
                request.DeviceType = otpForm.DeviceType;
                request.FormId = otpForm.FormId;
                request.SubType = otpForm.SubType;
                request.OTP = otp;
                request.UserName = userName;
                request.Password = password;
                request.reqAuditInfo = new AuditInfo
                {
                    LicenseID = sessionLicenseID,
                    UserID = sessionUserID
                };

                var response = GetEps().AuthenicateOTP(request);

                request.Password = string.Empty;
                timer.Message = string.Format("<Request>{0}</Request><Response>{1}</Response>", request, response);

                return response;
            }
        }

        public static AuthenticateOtpFormsForSigningResponse AuthenicateOTPForSigning(string transactionId, string otp, string identityName, OTPForm otpForm, string userName, string password, string sessionLicenseID, string sessionUserID, string ipAddress, string appInstanceId, Guid correlationId)
        {
            using (var timer = logger.StartTimer("AuthenicateOTPForSigning"))
            {
                var request = ServiceRequest<GetShieldAuthenicateOTPRequest>();
                request.IdentityName = identityName;
                request.TransactionId = transactionId;
                request.DeviceType = otpForm.DeviceType;
                request.FormId = otpForm.FormId;
                request.SubType = otpForm.SubType;
                request.OTP = otp;
                request.UserName = userName;
                request.Password = password;
                request.IPAddress = ipAddress;
                request.AppInstanceId = appInstanceId;
                request.reqAuditInfo = new AuditInfo
                {
                    LicenseID = sessionLicenseID,
                    UserID = sessionUserID
                };
                request.CorrelationId = correlationId;

                var response = GetEps().AuthenticateOtpForSigning(request);

                request.Password = string.Empty;
                timer.Message = string.Format("<Request>{0}</Request><Response>{1}</Response>", request, response);

                return response;
            }
        }


        public static bool DoesActivationCodeExist(string activationCode)
        {
            using (var timer = logger.StartTimer("DoesActivationCodeExist"))
            {
                var response = GetEps().DoesActivationCodeExist(activationCode);

                timer.Message = string.Format("<Request>{0}</Request><Response>{1}</Response>", activationCode, response);

                return response;
            }
        }

        public static bool DoesUsernameExist(string username)
        {
            using (var timer = logger.StartTimer("DoesUsernameExist"))
            {
                var response = GetEps().UserNameExists(username);

                timer.Message = string.Format("<Request>{0}</Request><Response>{1}</Response>", username, response);

                return response;
            }
        }

        public static ActivateAndUpdateUserResponse ActivateAndUpdateShield(ActivateAndUpdateUserRequest request)
        {
            using (var timer = logger.StartTimer("ActivateAndUpdateUserInShield"))
            {
                UpdateAsServiceRequest(ref request);
                var response = GetEps().ActivateAndUpdateUser(request);
                request.Password = string.Empty;
                timer.Message = string.Format("<Request>{0}</Request><Response>{1}</Response>", request.ToLogString(), response.ToLogString());
                return response;
            }
        }

        public static ActivateAndUpdateUserResponse ActivateAndUpdateUtilitiesUser(ActivateAndUpdateUserRequest request)
        {
            using (var timer = logger.StartTimer("ActivateAndUpdateUtilitiesUser"))
            {
                UpdateAsServiceRequest(ref request);
                var response = GetEps().ActivateAndUpdateUtilitiesUser(request);
                request.Password = string.Empty;
                timer.Message = string.Format("<Request>{0}</Request><Response>{1}</Response>", request.ToLogString(), response.ToLogString());
                return response;
            }
        }

        public static LinkExistingShieldUserToeRxUserResponse LinkExistingShieldUserToeRxUser(string activationCode, string ipAddress, string shieldUserName, string password)
        {
            var request = ServiceRequest<LinkExistingShieldUserToeRxUserRequest>();
            request.ActivationCode = activationCode;
            request.IpAddress = ipAddress;
            request.ShieldUserName = shieldUserName;
            request.Password = password;

            using (var timer = logger.StartTimer("LinkExistingShieldUserToeRxUser"))
            {
                var response = GetEps().LinkExistingShieldUserToeRxUser(request);

                request.Password = string.Empty;
                timer.Message = string.Format("<Request>{0}</Request><Response>{1}</Response>", request.ToLogString(), response.ToLogString());

                return response;
            }
        }

        public static LinkExistingShieldUserToeRxUserResponse LinkExistingShieldUserToUtilitiesUser(string activationCode, string ipAddress, string shieldUserName, string password)
        {
            var request = ServiceRequest<LinkExistingShieldUserToeRxUserRequest>();
            request.ActivationCode = activationCode;
            request.IpAddress = ipAddress;
            request.ShieldUserName = shieldUserName;
            request.Password = password;

            using (var timer = logger.StartTimer("LinkExistingShieldUserToUtilitiesUser"))
            {
                var response = GetEps().LinkExistingShieldUserToUtilitiesUser(request);

                request.Password = string.Empty;
                timer.Message = string.Format("<Request>{0}</Request><Response>{1}</Response>", request.ToLogString(), response.ToLogString());

                return response;
            }
        }
        public static string GetShieldExternalTenantID(string accountID)
        {
            using (var timer = logger.StartTimer("GetShieldExternalTenantID"))
            {
                var shieldExternalTenantID = GetEps().GetShieldExternalTenantID(accountID);
                timer.Message = string.Format("<Request>{0}</Request><Response>{1}</Response>", accountID, shieldExternalTenantID);
                return shieldExternalTenantID;
            }
        }

        public static List<ePrescribeSvc.TeaserAdResponse> GetTeaserAdFromServiceAlert(DateTime currentDate, string sessionLicenseID, string sessionPracticeState, string enterpriseClientID, int pricingStructure, bool isFirstBusinessDay, bool isAdmin)
        {
            using (var timer = logger.StartTimer("GetTeaserAdFromServiceAlert"))
            {
                var request = new ePrescribeSvc.TeaserAdRequest
                {
                    CurrentDate = currentDate,
                    LicenseID = sessionLicenseID,
                    PracticeState = sessionPracticeState,
                    EnterpriseClientID = enterpriseClientID,
                    PricingStructure = pricingStructure,
                    IsFirstBusinessDay = isFirstBusinessDay,
                    IsAdmin = isAdmin
                };
                var response = GetEps().GetTeaserAdFromServiceAlert(request).ToList();

                timer.Message = string.Format("<Request>{0}</Request><Response>{1}</Response>", request.ToLogString(), response.ToLogString());
                return response;
            }
        }

        public static GetStateLicenseAndDeaExpiryDateForUserResponse GetStateLicenseAndDeaExpiryDateForUser(Guid userGuid, string practiseState, ConnectionStringPointer dbID)
        {
            using (var timer = logger.StartTimer("SaveSecretQuestionAnswers"))
            {
                var getStateLicenseExpiryDateForUserRequest = ServiceRequest<GetStateLicenseAndDeaExpiryDateForUserRequest>();
                getStateLicenseExpiryDateForUserRequest.UserGuid = userGuid;
                getStateLicenseExpiryDateForUserRequest.PractiseState = practiseState;
                getStateLicenseExpiryDateForUserRequest.DbID = (ePrescribeSvc.ConnectionStringPointer)dbID;
                return GetEps().GetStateLicenseAndDeaExpiryDateForUser(getStateLicenseExpiryDateForUserRequest);
            }
        }

        public static void AddILearnUser(string userName, string clientId, string firstName, string lastName, ConnectionStringPointer DbID)
        {
            try
            {
                GetEps().AddUserToIlearn(new ePrescribeSvc.ILearnRequest()
                {
                    UserName = userName,
                    FirstName = firstName,
                    ClientId = clientId,
                    LastName = lastName,
                    DbID = (ePrescribeSvc.ConnectionStringPointer)DbID
                });
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
            }
        }

        void IEPSBroker.AddILearnUser(string userName, string clientId, string firstName, string lastName, ConnectionStringPointer DbID)
        {
            AddILearnUser(userName, clientId, firstName, lastName, DbID);
        }

        public static int GetShieldPasswordExpirationDays(string shieldLoginId)
        {
            int daysleft = 999;
            try
            {
                daysleft = GetEps().GetShieldPasswordExpirationDate(shieldLoginId);
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
            }

            return daysleft;
        }

        #region UserMessageTrackingAck
        public static void AddMessageTrackingAck(Guid configKey, Guid userGuid)
        {
            try
            {
                GetEps().AddMessageTrackingAck(new ePrescribeSvc.AddMessageTrackingAckRequest()
                {
                    ConfigKeyID = configKey,
                    UserGuid = userGuid
                });
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
            }
        }

        public static GetMessageTrackingAcksResponse GetMessageTrackingAcks(Guid userGuid)
        {
            var response = new GetMessageTrackingAcksResponse();
            try
            {
                response = GetEps().GetMessageTrackingAcksOfUser(new ePrescribeSvc.GetMessageTrackingAckRequest()
                {
                    UserGuid = userGuid
                });
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
            }
            return response;
        }
        #endregion

        #region PPT Plus
        public static ePrescribeSvc.GetPPTPlusSamlTokenResponse GetPPTPlusSamlToken(string identityToken, string shieldTenantId)
        {
            using (var timer = logger.StartTimer("GetPPTPlusSamlToken"))
            {
                var request = ServiceRequest<ePrescribeSvc.GetPPTPlusSamlTokenRequest>();
                request.IdentityToken = identityToken;
                request.ShieldTenantId = shieldTenantId;

                var response = GetEps().GetPPTPlusSamlToken(request);
                timer.Message = $"<Request>{request.ToLogString()}</Request><Response>{response.ToLogString()}</Response>";

                return response;
            }
        }

        public static ePrescribeSvc.GetPDMPSamlTokenResponse GetPDMPSamlToken(string identityToken, string shieldTenantId)
        {
            using (var timer = logger.StartTimer("GetPDMPSamlToken"))
            {
                var request = ServiceRequest<ePrescribeSvc.GetPDMPSamlTokenRequest>();
                request.IdentityToken = identityToken;
                request.ShieldTenantId = shieldTenantId;

                timer.Message = $"<Request>{request.ToLogString()}</Request>";
                var response = GetEps().GetPDMPSamlToken(request);
                timer.Message = $"<Response>{response.ToLogString()}</Response>";

                return response;
            }
        }

        public static ePrescribeSvc.GetPPTPlusSamlTokenResponse RefreshPPTPlusSamlToken(string identityToken, string shieldTenantId)
        {
            using (var timer = logger.StartTimer("GetPPTPlusSamlToken"))
            {
                var request = ServiceRequest<ePrescribeSvc.GetPPTPlusSamlTokenRequest>();
                request.IdentityToken = identityToken;
                request.ShieldTenantId = shieldTenantId;

                var response = GetEps().RefreshPPTPlusSamlToken(request);
                timer.Message = $"<Request>{request.ToLogString()}</Request><Response>{response.ToLogString()}</Response>";

                return response;
            }
        }

        #endregion

        #region PDMP
        public static Int64 InsertAudits(string userGuid, string licienceId, ServerModel.PdmpEnrollmentModel enrollmentData, ConnectionStringPointer dbID)
        {
            PdmpEnrollmentAuditModel pdmpEnrollmentAuditModel = MapAuditModel(userGuid, licienceId, enrollmentData);

            return Allscripts.ePrescribe.Data.PDMP.PdmpData.InsertAudits(pdmpEnrollmentAuditModel, dbID);
        }

        private static PdmpEnrollmentAuditModel MapAuditModel(string userGuid, string licienceId, ServerModel.PdmpEnrollmentModel enrollmentData)
        {
            PdmpEnrollmentAuditModel pdmpEnrollmentAuditModel = new PdmpEnrollmentAuditModel();
            pdmpEnrollmentAuditModel.LicienceId = licienceId;
            pdmpEnrollmentAuditModel.NumberOfProviders = enrollmentData.NumberOfProviders;
            pdmpEnrollmentAuditModel.Requestor.UserId = userGuid;
            pdmpEnrollmentAuditModel.Requestor.UserName = enrollmentData.ContactNameAndTitle;
            pdmpEnrollmentAuditModel.Requestor.ContactEmail = enrollmentData.ContactEmail;
            pdmpEnrollmentAuditModel.Requestor.ContactPhoneNo = enrollmentData.ContactPhoneNo;
            pdmpEnrollmentAuditModel.Requestor.ContactFax = enrollmentData.ContactFax;
            pdmpEnrollmentAuditModel.AccountAddress.Address1 = enrollmentData.Address1;
            pdmpEnrollmentAuditModel.AccountAddress.Address2 = enrollmentData.Address2;
            pdmpEnrollmentAuditModel.AccountAddress.City = enrollmentData.City;
            pdmpEnrollmentAuditModel.AccountAddress.State = enrollmentData.State;
            pdmpEnrollmentAuditModel.AccountAddress.ZipCode = enrollmentData.ZipCode;
            pdmpEnrollmentAuditModel.AccountName = enrollmentData.AccountName;
            pdmpEnrollmentAuditModel.TermsAndConditionsUserAcceptance = enrollmentData.TermsAndConditionsUserAcceptance;
            pdmpEnrollmentAuditModel.TermsAndConditionsID = enrollmentData.TermsAndConditionsID;
            return pdmpEnrollmentAuditModel;
        }

        public static ePrescribeSvc.GetPDMPSamlTokenResponse RefreshPDMPSamlToken(string identityToken, string shieldTenantId)
        {
            using (var timer = logger.StartTimer("GetPDMPSamlToken"))
            {
                var request = ServiceRequest<ePrescribeSvc.GetPDMPSamlTokenRequest>();
                request.IdentityToken = identityToken;
                request.ShieldTenantId = shieldTenantId;
                timer.Message = $"<Request>{request.ToLogString()}</Request>";
                var response = GetEps().RefreshPDMPSamlToken(request);
                timer.Message = $"<Response>{response.ToLogString()}</Response>";

                return response;
            }
        }

        public ePrescribeSvc.GetPdmpEnrollmentFormInfoResponse GetPdmpEnrollmentFormInfo(string userGuid, string licenseId, ConnectionStringPointer dbID)
        {
            using (var timer = logger.StartTimer("GetPdmpEnrollmentFormInfo"))
            {
                var request = ServiceRequest<ePrescribeSvc.GetPdmpEnrollmentFormInfoRequest>();
                request.UserGuid = userGuid;
                request.LicenseID = licenseId;
                request.DbID = (ePrescribeSvc.ConnectionStringPointer)dbID;
                timer.Message = $"<Request>{request.ToLogString()}</Request>";
                var response = GetEps().GetPdmpEnrollmentFormInfo(request);
                timer.Message = $"<Response>{response.ToLogString()}</Response>";

                return response;
            }
        }
        #endregion



        public static PrintRxResponse PrintRxs(PrintRxRequest request)
        {
            using (var timer = logger.StartTimer("PrintRx"))
            {
                timer.Message = $"<Request>{request.ToLogString()}</Request>";
                var response = GetEps().PrintRxs(request);
                timer.Message += $"<Response>{response.ToLogString()}</Response>";

                return response;
            }

        }

        public static ConvertFromHtmlToPdfResponse ConvertFromHtmlToPdf(string html, PdfDocumentSettingsType type)
        {
            var request = ServiceRequest<ConvertFromHtmlToPdfRequest>();
            request.HtmlString = html;
            request.PdfDocSettings = type;

            using (var timer = logger.StartTimer("ConvertFromHtmlToPdf"))
            {
                timer.Message = $"<Request>{request.ToLogString()}</Request>";
                var response = GetEps().ConvertFromHtmlToPdf(request);
                timer.Message += $"<Response>{response.ToLogString()}</Response>";

                return response;
            }
        }

        public ePrescribeSvcResponse RtpsSendDisposition(string userId,
            string licenseId,
            string patientId,
            string transactionId,
            string relatesToTransactionId,
            Disposition disposition,
            ConnectionStringPointer dbId)
        {
            RtpsCRxDispositionRequest rtpsCRxDispositionRequest = ServiceRequest<RtpsCRxDispositionRequest>();
            rtpsCRxDispositionRequest.reqAuditInfo = new AuditInfo { LicenseID = licenseId, UserID = userId };
            rtpsCRxDispositionRequest.PatientId = patientId;
            rtpsCRxDispositionRequest.TxId = transactionId;
            rtpsCRxDispositionRequest.RelatesToTxId = relatesToTransactionId;
            rtpsCRxDispositionRequest.SentTime = DateTime.UtcNow;
            rtpsCRxDispositionRequest.Disposition = disposition;
            rtpsCRxDispositionRequest.DbID = (ePrescribeSvc.ConnectionStringPointer)dbId;

            using (var timer = logger.StartTimer("RtpsCrxMedDisposition"))
            {
                var response = GetEps().RtpsSendDisposition(rtpsCRxDispositionRequest);
                return response;
            }
        }

        public static GetServiceAccountShieldSamlTokenResult RequestShieldAuthToken(string targetIdentifier,
        string licenseID, string userGuid, ConnectionStringPointer dbID)
        {
            var shieldGetServiceAccountShieldSamlTokenToken =
                    new GetServiceAccountShieldSamlTokenResult();

            try
            {
                var result = GetEps().GetServiceAccountShieldSamlToken(targetIdentifier);
                shieldGetServiceAccountShieldSamlTokenToken.samlString = result.samlString;
                shieldGetServiceAccountShieldSamlTokenToken.validTo = result.validTo;
            }

            catch (Exception ex)
            {
                Audit.AddException(userGuid, licenseID,
                string.Concat("Exception requesting shieldGetServiceAccountShieldSamlTokenToken: ", ex.ToString()),
                null, null, null, dbID);
            }

            return shieldGetServiceAccountShieldSamlTokenToken;
        }

        public static ShieldAuditEventResponse LogShieldAuditEvent(Guid userId, Guid licenseId, Guid rxId, string userToken, string appInstanceId, string shieldExternalTenantId, Guid shieldCorrelationId,
            bool outcome, string outcomeMessage, ConnectionStringPointer dbId)
        {
            using (var timer = logger.StartTimer("LogShieldAuditEvent"))
            {
                var request = ServiceRequest<ePrescribeSvc.ShieldAuditEventRequest>();
                request.UserToken = userToken;
                request.UserId = userId;
                request.RxId = rxId;
                request.AppInstanceId = appInstanceId;
                request.ExternalTenantId = shieldExternalTenantId;
                request.CorrelationId = shieldCorrelationId;
                request.DbID = (ePrescribeSvc.ConnectionStringPointer)dbId;
                request.reqAuditInfo = new AuditInfo { LicenseID = licenseId.ToString() };
                request.Outcome = outcome;
                request.OutcomeMessage = outcomeMessage;

                return GetEps().LogShieldAuditEvent(request);
            }
        }

        #region Registration        
        public static CreateRegistrantUserResponse CreateRegistrantUser(RegistrantUserRequest registrantUserRequest, string ipAddress)
        {
            var registrant = registrantUserRequest.RegistrantUser;
            CreateRegistrantUserRequest createRegistrantUserRequest = ServiceRequest<CreateRegistrantUserRequest>();
            List<SecretAnswer> secretAnswerList = new List<SecretAnswer>();
            createRegistrantUserRequest.RegistrantUser = new ePrescribeSvc.RegistrantUser()
            {
                City = registrant.City,
                DeaExpirationDate = registrant.DeaExpirationDate,
                DeaNumber = registrant.DeaNumber,
                DeaScheduleII = registrant.DeaScheduleII == "true" ? "Y" : "N",
                DeaScheduleIII = registrant.DeaScheduleIII == "true" ? "Y" : "N",
                DeaScheduleIV = registrant.DeaScheduleIV == "true" ? "Y" : "N",
                DeaScheduleV = registrant.DeaScheduleV == "true" ? "Y" : "N",
                FirstName = registrant.FirstName,
                HomeAddress = registrant.HomeAddress,
                HomeAddress2 = registrant.HomeAddress2,
                LastName = registrant.LastName,
                MiddleName = registrant.MiddleName,
                NPI = registrant.NPI,
                Password = registrant.Password,
                PersonalEmail = registrant.PersonalEmail,
                ContactPhoneNumber = StringHelper.CleanNumeric(registrant.ContactPhoneNumber),
                ShieldUserName = registrant.ShieldUserName,
                Speciality1 = registrant.Speciality1,
                Speciality2 = registrant.Speciality2,
                State = registrant.State,
                StateLicenseExpirationDate = registrant.StateLicenseExpirationDate,
                StateLicenseIssueState = registrant.StateLicenseIssueState,
                StateLicenseNumber = registrant.StateLicenseNumber,
                Suffix = registrant.Suffix,
                Title = registrant.Title,
                Zipcode = registrant.Zipcode,
                EnterpriseClientId = registrant.EnterpriseClientId,
                PricingStructure = registrant.PricingStructure
            };

            for (int i = 0; i < registrantUserRequest.SecretAnswers.Count(); i++)
            {
                var secretAnswer = new ePrescribeSvc.SecretAnswer();
                secretAnswer.QuestionID = registrantUserRequest.SecretAnswers[i].QuestionID;
                secretAnswer.Answer = registrantUserRequest.SecretAnswers[i].Answer;
                secretAnswerList.Add(secretAnswer);
            }

            createRegistrantUserRequest.SecretAnswers = secretAnswerList.ToArray();
            createRegistrantUserRequest.IPAddress = ipAddress;
            createRegistrantUserRequest.IsLinkExistingShieldUser = registrantUserRequest.IsLinkExistingShieldUser;
            using (var timer = logger.StartTimer("UpdateRegistrantPractice"))
            {
                CreateRegistrantUserResponse createRegistrantUserResponse = GetEps().CreateRegistrantUser(createRegistrantUserRequest);
                timer.Message = $"<Request>{createRegistrantUserRequest.ToLogString()}</Request><Response>{createRegistrantUserResponse.ToLogString()}</Response>";
                return createRegistrantUserResponse;
            }
        }

        public static ePrescribeSvc.ePrescribeSvcResponse CompleteRegistration(RegistrantPractice registrantPractice, RegistrantInfo registrantInfo, string shieldIdentityToken)
        {
            var registrantPracticeData = registrantPractice;
            CompleteRegistrationRequest completeRegistrationRequest = ServiceRequest<CompleteRegistrationRequest>();
            completeRegistrationRequest.RegistrantPractice = new ePrescribeSvc.RegistrantPractice()
            {
                RegistrantId = registrantInfo.RegistrantId,
                PracticeName = registrantPracticeData.PracticeName,
                Address = registrantPracticeData.Address,
                Address2 = registrantPracticeData.Address2,
                City = registrantPracticeData.City,
                State = registrantPracticeData.State,
                PhoneNumber = StringHelper.CleanNumeric(registrantPracticeData.PhoneNumber),
                Zipcode = registrantPracticeData.Zipcode,
                FaxNumber = StringHelper.CleanNumeric(registrantPracticeData.FaxNumber)
            };

            completeRegistrationRequest.RegistrantId = registrantInfo.RegistrantId;
            completeRegistrationRequest.ShieldObjectId = registrantInfo.ShieldObjectId;
            completeRegistrationRequest.ShieldIdentityToken = shieldIdentityToken;
            using (var timer = logger.StartTimer("UpdateRegistrantPractice"))
            {
                var updateRegistrantPracticeResponse = GetEps().CompleteRegistration(completeRegistrationRequest);
                timer.Message = $"<Request>{completeRegistrationRequest.ToLogString()}</Request><Response>{updateRegistrantPracticeResponse.ToLogString()}</Response>";
                return updateRegistrantPracticeResponse;
            }
        }

        public static GetRegistrantIdProofingLoaResponse GetRegistrantIdProofingLevelOfAssurance(long registrantId, string shieldIdentityToken)
        {
            using (var timer = logger.StartTimer("GetRegistrantIdProofingLevelOfAssurance"))
            {
                GetRegistrantIdProofingLoaRequest getRegistrantIdProofingLoaRequest = ServiceRequest<GetRegistrantIdProofingLoaRequest>();
                getRegistrantIdProofingLoaRequest.RegistrantId = registrantId;
                getRegistrantIdProofingLoaRequest.ShieldIdentityToken = shieldIdentityToken;

                GetRegistrantIdProofingLoaResponse response = GetEps().GetRegistrantIdProofingLevelOfAssurance(getRegistrantIdProofingLoaRequest);
                timer.Message = $"<Request>{getRegistrantIdProofingLoaRequest.ToLogString()}</Request><Response>{response.ToLogString()}</Response>";
                return response;
            }
        }

        public static void LogRegistrationStep(long registrantId, bool isSuccess, string stepDescription, Constants.RegistrationStep registrationStep)
        {
            LogRegistrationStepRequest logRegistrationStepRequest = ServiceRequest<LogRegistrationStepRequest>();
            logRegistrationStepRequest.RegistrantId = registrantId;
            logRegistrationStepRequest.RegistrationStepDescription = stepDescription;
            logRegistrationStepRequest.IsSuccessful = isSuccess;
            logRegistrationStepRequest.RegistrationStep = (ePrescribeSvc.RegistrationStep)Enum.Parse(typeof(ePrescribeSvc.RegistrationStep), registrationStep.ToString());

            GetEps().LogRegistrationStep(logRegistrationStepRequest);
        }

       

        #endregion
    }
}
