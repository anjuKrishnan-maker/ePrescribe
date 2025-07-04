using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.Objects.PPTPlus;
using Allscripts.ePrescribe.Shared.Logging;
using Allscripts.Impact;
using eRxWeb.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Allscripts.ePrescribe.Data;
using Allscripts.ePrescribe.ExtensionMethods;
using Rx = Allscripts.Impact.Rx;
using System.Web.Script.Serialization;
using ConnectionStringPointer = Allscripts.ePrescribe.DatabaseSelector.ConnectionStringPointer;
using GetPPTPlusSamlTokenResponse = eRxWeb.ePrescribeSvc.GetPPTPlusSamlTokenResponse;
using System.Threading.Tasks;
using System.Threading;
using System.Data;
using Allscripts.ePrescribe.Objects;
using Allscripts.Impact.PPTPlusBPL;
using Newtonsoft.Json;
using static Allscripts.ePrescribe.Common.Constants;
using System.Text;
using Allscripts.Impact.Interfaces;
using eRxWeb.ServerModel.Request;
using Hl7.Fhir.Model;
using Newtonsoft.Json.Linq;
using IMedication = Allscripts.ePrescribe.Data.IMedication;
using Task = System.Threading.Tasks.Task;
using eRxWeb.ePrescribeSvc;
using Allscripts.ePrescribe.Objects.CommonComponent;
using Allscripts.ePrescribe.Data.CommonComponent;
using eRxWeb.AppCode.Interfaces;
using eRxWeb.AppCode.Rtps;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Allscripts.ePrescribe.Test")]

namespace eRxWeb.AppCode.PptPlusBPL
{
    public class PPTPlus
    {
        private static readonly ILoggerEx logger = LoggerEx.GetLogger();

        public static void SetScriptPadMedCouponInfo(PptPlusRequestInfo requestInfo, PptPlusScriptPadMed scriptPadMed,
                                             IPptPlus pptPlus, IPptPlusServiceBroker pptPlusServiceBroker, string pptSamlToken,
                                             ICommonComponentData commonComponentData,  IStateContainer session, ConnectionStringPointer dbId)
        {
            string couponFhirRequest = string.Empty;
            string ackResponse = string.Empty;
            try
            {

                if (scriptPadMed.IsCouponAvailable)
                {
                    couponFhirRequest = pptPlus.CreateCouponRequestFhir(new PptPlusBundler(requestInfo), scriptPadMed.CouponRequestInfo, scriptPadMed.TransactionId);
                    ackResponse = pptPlusServiceBroker.RetrieveCouponInfo(couponFhirRequest, pptSamlToken);

                    if (!string.IsNullOrWhiteSpace(ackResponse))
                    {
                        scriptPadMed.CouponResponse = ackResponse;

                        commonComponentData.InsertTransactionDetail(couponFhirRequest, ackResponse,
                         scriptPadMed.TransactionId, Convert.ToInt32(Constants.CommonApiTransactionTypes.PPT_GetOfferContent), dbId);
                    }
                }

            }
            catch (Exception e)
            {
                string exceptionDetails = $"Exception retrieving coupon info: <E>{e}</E><request>{couponFhirRequest}</request> <response>{ackResponse}</response>";
                logger.Debug(exceptionDetails);
                AddException(exceptionDetails, session);
            }
        }

        public static void SetAllScriptPadMedCouponRequestInfo(IStateContainer session, IPptPlus pptPlus)
        {
            var responseContainer = session.Cast(Constants.SessionVariables.PptPlusResponses, default(PptPlusResponseContainer));

            if (responseContainer?.ScriptPadMeds == null)
                return;

            foreach (var scriptPadMed in responseContainer?.ScriptPadMeds)
            {
                pptPlus.ParseCouponInfo(scriptPadMed);
            }
            session[Constants.SessionVariables.PptPlusResponses] = responseContainer;
        }

        public static void SetAllScriptPadMedPriorAuthFlag(IStateContainer session, IPptPlus pptPlus)
        {
            var responseContainer = session.Cast(Constants.SessionVariables.PptPlusResponses, default(PptPlusResponseContainer));

            if (responseContainer?.ScriptPadMeds == null)
                return;

            foreach (var scriptPadMed in responseContainer?.ScriptPadMeds)
            {
                pptPlus.ParseRtbPriorAuthFlag(scriptPadMed);
            }
            session[Constants.SessionVariables.PptPlusResponses] = responseContainer;
        }
        public static string AddException(string exception, IStateContainer session)
        {
            string exceptionID = Audit.AddException(session.GetGuidOr0x0("USERID").ToString(),
                session.GetGuidOr0x0("LICENSEID").ToString(), exception,
                string.Empty, string.Empty, string.Empty,
                (ConnectionStringPointer)session["DBID"]);
            return exceptionID;
        }

        public static bool IsPPTDetailInfoAvailable(IStateContainer session, IPptPlus pptPlus)
        {
            var responseContainer = session.Cast(Constants.SessionVariables.PptPlusResponses, default(PptPlusResponseContainer));
            GetAllCandidateMedPPTInfo(session);
            GetAllScriptPadMedPPTInfo(session);
            return !ShouldDisableWindow(session) && pptPlus.AtleastOnePPTPlusResponseHasCards(responseContainer);
        }

        public static void ClearScriptPadCandidates(IStateContainer session)
        {
            if (!IsPPTPlusPreferenceOn(session))
                return;

            var responseContainer = session.Cast(Constants.SessionVariables.PptPlusResponses, default(PptPlusResponseContainer));
            responseContainer?.Candidates?.Clear();
            session[Constants.SessionVariables.PptPlusResponses] = responseContainer;
        }

        public static void RemoveMedFromScriptPad(IStateContainer session, string transactionID, List<PptPlusScriptPadMed> scriptPadMeds)
        {
            Guid? rxID = scriptPadMeds?.FirstOrDefault(_ => _.TransactionId == transactionID)?.RxId;
            ScriptPadUtil.RemoveScript(session, Convert.ToString(rxID));
        }

        public static string GetPharmacyIDFromCandidateMed(IStateContainer session, IPptPlus pptPlus, int medListIndex, string sitePharmacy)
        {
            if (!IsPPTPlusPreferenceOn(session))
                return sitePharmacy;
            var responseContainer = session.Cast(Constants.SessionVariables.PptPlusResponses, default(PptPlusResponseContainer));
            return pptPlus.GetPharmacyIDForMed(responseContainer, medListIndex, sitePharmacy);
        }

        public static void CopyCandidatesToScriptPadMeds(IStateContainer session, IPptPlus pptPlus, int medListIndex, Guid rxId)
        {
            if (!IsPPTPlusPreferenceOn(session))
                return;

            var responseContainer = session.Cast(Constants.SessionVariables.PptPlusResponses, default(PptPlusResponseContainer));
            pptPlus.CopyCandidatesToScriptPadMeds(ref responseContainer, medListIndex, rxId);
            session[Constants.SessionVariables.PptPlusResponses] = responseContainer;
        }

        public static void CopyAllCandidatesToScriptPadMeds(IStateContainer session, IPptPlus pptPlus)
        {
            if (!IsPPTPlusPreferenceOn(session))
                return;

            var responseContainer = session.Cast(Constants.SessionVariables.PptPlusResponses, default(PptPlusResponseContainer));

            if (responseContainer?.Candidates != null)
            {
                while(responseContainer.Candidates.Count > 0)
                {
                    var candidate = responseContainer.Candidates.First();
                    pptPlus.CopyCandidatesToScriptPadMeds(ref responseContainer, candidate.MedListIndex, candidate.RxId);
                }
            }

            session[Constants.SessionVariables.PptPlusResponses] = responseContainer;
        }

        public static void UpdateRxIdToCandidatesMed(IStateContainer session, int medListIndex, Guid rxId)
        {
            if (!IsPPTPlusPreferenceOn(session))
                return;

            var responseContainer = session.Cast(Constants.SessionVariables.PptPlusResponses, default(PptPlusResponseContainer));
            var candidate = responseContainer?.Candidates?.FirstOrDefault(_ => _.MedListIndex == medListIndex);

            if (candidate != null)
            {
                candidate.RxId = rxId;
            }
        }

        public static bool IsPPTPlusPreferenceOn(IStateContainer session)
        {
            var sessionLicense = session.Cast("SessionLicense", default(ApplicationLicense));
            var provAllowsPpt = session.GetBooleanOrFalse(Constants.SessionVariables.ShouldShowPpt);
            var provAllowsRtbi = session.GetBooleanOrFalse(Constants.SessionVariables.ShouldShowRtbi);
            return (sessionLicense.EnterpriseClient.ShowPPTPlus && (provAllowsPpt || provAllowsRtbi)) && !ShouldDisableWindow(session);
        }

        public static bool IsPPTPlusEnterpriseOn(IStateContainer session)
        {
            var sessionLicense = session.Cast("SessionLicense", default(ApplicationLicense));
            return sessionLicense.EnterpriseClient.ShowPPTPlus;
        }
        public static bool IsEpaOn(IStateContainer session)
        {
            var sessionLicense = session.Cast("SessionLicense", default(ApplicationLicense));
            return sessionLicense.EnterpriseClient.ShowEPA && session.GetBooleanOrFalse(Constants.LicensePreferences.SHOW_EPA);
        }

        public static void CopyExistingMedsToScriptPadMeds(IStateContainer session, List<Rx> rxList, IPptPlus pptPlus)
        {
            if (!IsPPTPlusPreferenceOn(session))
                return;

            var responseContainer = session.Cast(Constants.SessionVariables.PptPlusResponses, default(PptPlusResponseContainer));
            var dbId = session.Cast(Constants.SessionVariables.DbId, default(ConnectionStringPointer));
            var requestInfo = GetPptRequestInfo(session);
            pptPlus.CopyExistingMedsToScriptPadMeds(ref responseContainer, requestInfo, rxList, new PptPlusData(), dbId);
            session[Constants.SessionVariables.PptPlusResponses] = responseContainer;
        }

        public static void RemoveScriptPadMedByRxID(IStateContainer session, IPptPlus pptPlus, Guid rxId)
        {
            var responseContainer = session.Cast(Constants.SessionVariables.PptPlusResponses, default(PptPlusResponseContainer));
            pptPlus.RemoveScriptPadMedByRxID(ref responseContainer, rxId);
            session[Constants.SessionVariables.PptPlusResponses] = responseContainer;
        }

        public static bool IsPptCouponAvailable(IStateContainer session, Guid rxId)
        {
            string couponPrice;
            return IsPptCouponAvailable(session, rxId, out couponPrice);
        }
        public static bool IsPptCouponAvailable(IStateContainer session, Guid rxId, out string couponPrice)
        {
            couponPrice = string.Empty;
            if (!IsPPTPlusPreferenceOn(session))
                return false;

            var provAllowsPpt = session.GetBooleanOrFalse(Constants.SessionVariables.ShouldShowPpt);
            if (provAllowsPpt)
            {

                var responseContainer = session.Cast(Constants.SessionVariables.PptPlusResponses, default(PptPlusResponseContainer));
                var scriptPadMed = responseContainer?.ScriptPadMeds?.FirstOrDefault(_ => _.RxId == rxId);

                if (scriptPadMed != null)
                {
                    couponPrice = scriptPadMed.CouponPrice;
                    return scriptPadMed.IsCouponAvailable;
                }
            }
            return false;
        }

        public static bool IsPptInitiationPriorAuthRx(IStateContainer session, Guid rxId)
        {
            if (!IsPPTPlusPreferenceOn(session))
                return false;

            var provAllowsPpt = session.GetBooleanOrFalse(Constants.SessionVariables.ShouldShowRtbi);
            if (provAllowsPpt)
            {
                var responseContainer = session.Cast(Constants.SessionVariables.PptPlusResponses, default(PptPlusResponseContainer));
                var scriptPadMed = responseContainer?.ScriptPadMeds?.FirstOrDefault(_ => _.RxId == rxId);

                if (scriptPadMed != null)
                {
                    return scriptPadMed.IsInitiatePriorAuthToggled;
                }
            }
            return false;
        }

        public static bool IsPptPriorAuthRx(IStateContainer session, Guid rxId)
        {
            if (!IsPPTPlusPreferenceOn(session))
                return false;

            var provAllowsPpt = session.GetBooleanOrFalse(Constants.SessionVariables.ShouldShowRtbi);
            if (provAllowsPpt)
            {
                var responseContainer = session.Cast(Constants.SessionVariables.PptPlusResponses, default(PptPlusResponseContainer));
                var scriptPadMed = responseContainer?.ScriptPadMeds?.FirstOrDefault(_ => _.RxId == rxId);

                if (scriptPadMed != null)
                {
                    return scriptPadMed.IsPriorAuth;
                }
            }
            return false;
        }

        public static PPTPriorAuthStatus GetPptPriorAuthRxStatus(IStateContainer session, Guid rxId)
        {
            return IsPptPriorAuthRx(session, rxId) ? PPTPriorAuthStatus.PA_REQUIRED : PPTPriorAuthStatus.NONE;
        }

        public static bool IsPptPriorAuthOptionAvailable(IStateContainer session, Guid rxId, out bool shouldToggleEpa)
        {
            shouldToggleEpa = false;
            if (!IsPPTPlusPreferenceOn(session))
                return false;

            var provAllowsPpt = session.GetBooleanOrFalse(Constants.SessionVariables.ShouldShowRtbi);
            if (!provAllowsPpt)
                return false;

            var responseContainer = session.Cast(Constants.SessionVariables.PptPlusResponses, default(PptPlusResponseContainer));
            var scriptPadMed = responseContainer?.ScriptPadMeds?.FirstOrDefault(_ => _.RxId == rxId);
            if (scriptPadMed == null)
                return false;

            if(!scriptPadMed.IsPriorAuth)
                return false;

            shouldToggleEpa = scriptPadMed.IsInitiatePriorAuthToggled;
            
            return true;
        }
        public static string GetPptCouponContent(IStateContainer session, IPptPlus pptPlus, Guid rxId)
        {
            IPptPlusServiceBroker pptBroker = new PptPlusServiceBroker();
            string couponContent = string.Empty;
            var responseContainer = session.Cast(Constants.SessionVariables.PptPlusResponses, default(PptPlusResponseContainer));
            var requestInfo = GetPptRequestInfo(session);
            var pptToken = GetPptPlusSamlToken(session);
            var dbId = session.Cast(Constants.SessionVariables.DbId, default(ConnectionStringPointer));
            var scriptPadMed = responseContainer?.ScriptPadMeds?.FirstOrDefault(_ => _.RxId == rxId);

            if (scriptPadMed != null)
            {
                if (string.IsNullOrWhiteSpace(scriptPadMed.CouponResponse))
                {
                    SetScriptPadMedCouponInfo(requestInfo, scriptPadMed, pptPlus, new PptPlusServiceBroker(),
                        pptToken.Base64SamlToken, new CommonComponentData(), session, dbId);
                }
                couponContent = pptPlus.ParseCouponValue(scriptPadMed.CouponResponse);
            }
            return couponContent;
        }

        public static GoodRxCouponPharmacyNotesModel RetrieveGoodRxPharmacyNotesModel(IStateContainer session, IPptPlus pptPlus, Guid rxId)
        {
            IPptPlusServiceBroker pptBroker = new PptPlusServiceBroker();
            var responseContainer = session.Cast(Constants.SessionVariables.PptPlusResponses, default(PptPlusResponseContainer));
            var requestInfo = GetPptRequestInfo(session);
            var pptToken = GetPptPlusSamlToken(session);
            var dbId = session.Cast(Constants.SessionVariables.DbId, default(ConnectionStringPointer));
            var scriptPadMed = responseContainer?.ScriptPadMeds?.FirstOrDefault(_ => _.RxId == rxId);

            if (scriptPadMed != null)
            {
                if (string.IsNullOrWhiteSpace(scriptPadMed.CouponResponse))
                {
                    SetScriptPadMedCouponInfo(requestInfo, scriptPadMed, pptPlus, new PptPlusServiceBroker(),
                        pptToken.Base64SamlToken, new CommonComponentData(), session, dbId);
                }
                var goodRxCouponPharmacyNotesModel = scriptPadMed != null ? pptPlus.RetrieveGoodRxCouponPharmacyNotesModel(scriptPadMed.CouponResponse) : null;
                return goodRxCouponPharmacyNotesModel;
            }

            return null;
        }
        public static int HandleInitPricing(PptPlusSummaryRequest summaryRequest, IStateContainer session,
            IPptPlus pptPlus, IPptPlusServiceBroker pptBroker,
            Guid? updatedPharmacyID, IPptPlusData pptPlusData,
            ICommonComponentData commonComponentData,
            IRtpsHelper rtpsHelper)
        {
            int responseDelayMilliSeconds = 0;
            var ipAddress = HttpContext.Current?.Request.UserIpAddress();
            var dbId = session.Cast(Constants.SessionVariables.DbId, default(ConnectionStringPointer));
            var requestInfo = GetPptRequestInfo(session);
            var pptToken = GetPptPlusSamlToken(session);
            var userId = session.GetGuidOr0x0(Constants.SessionVariables.UserId).ToString();
            var licenseId = session.GetGuidOr0x0(Constants.SessionVariables.LicenseId).ToString();
            var isRtps = session.GetBooleanOrFalse(Constants.SessionVariables.ShouldShowRtps);
            string icd10Code = session.GetStringOrEmpty(Constants.SessionVariables.ICD10CODE).Trim();


            if (string.IsNullOrWhiteSpace(pptToken?.Base64SamlToken))
            {
                logger.Debug("HandleInitPricing: Shield PPT Token is null");
                return 0;
            }

            if (requestInfo == null)
            {
                logger.Debug("HandleInitPricing: Insufficient request Information");
                return 0;
            }

            var responseContainer = session.Cast(Constants.SessionVariables.PptPlusResponses, new PptPlusResponseContainer()) ?? new PptPlusResponseContainer();

            requestInfo.IpAddress = ipAddress;
            PptPlusRequestInfo clonedRequestInfo = (PptPlusRequestInfo)requestInfo.Clone();
            clonedRequestInfo.MedicationInfo = pptPlus.CreateMedicationInfo(summaryRequest.DDI, summaryRequest.GPPC, summaryRequest.PackUom, summaryRequest.Quantity, summaryRequest.PackSize, summaryRequest.PackQuantity, summaryRequest.Refills,
                                                                                summaryRequest.DaysSupply, Convert.ToBoolean(summaryRequest.IsDaw), icd10Code, dbId, pptPlusData);

            Guid pharmacyID = requestInfo?.PharmacyInfo != null ? requestInfo.PharmacyInfo.Id : Guid.Empty;
            string pharmacyNcpdp = requestInfo?.PharmacyInfo != null ? requestInfo.PharmacyInfo.Ncpdp : string.Empty;
            string pharmacyNpi = requestInfo?.PharmacyInfo != null ? requestInfo.PharmacyInfo.Npi : string.Empty;

            bool isPharmacyChangedRefreshNeeded = false;
            if ((updatedPharmacyID != null) && ((Guid)updatedPharmacyID != Guid.Empty) && ((Guid)updatedPharmacyID != pharmacyID))
            {
                var pharmInfo = pptPlusData.GetPharmacyInfo((Guid)(updatedPharmacyID), dbId);
                pharmacyID = (Guid)updatedPharmacyID;
                pharmacyNcpdp = pharmInfo != null ? pharmInfo.Ncpdp : string.Empty;
                pharmacyNpi = pharmInfo != null ? pharmInfo.Npi : string.Empty;

                if (pharmInfo != null && !pharmInfo.IsMailOrderPharmacy)
                {
                    clonedRequestInfo.PharmacyInfo = pharmInfo;
                    isPharmacyChangedRefreshNeeded = true;
                }
            }
            bool isScriptPadCandidateChanged = pptPlus.IsScriptPadCandidateMedInfoChanged(responseContainer, Convert.ToInt32(summaryRequest.MedSearchIndex), clonedRequestInfo.MedicationInfo.Name, clonedRequestInfo.MedicationInfo.Strength + " " + clonedRequestInfo.MedicationInfo.StrengthUom,
                    summaryRequest.DDI, summaryRequest.GPPC, summaryRequest.PackUom, summaryRequest.Quantity, summaryRequest.PackSize, summaryRequest.PackQuantity, summaryRequest.Refills, summaryRequest.DaysSupply, Convert.ToBoolean(summaryRequest.IsDaw),
                    clonedRequestInfo?.PatientInfo?.RtbiInfo?.CoverageId != null ? clonedRequestInfo.PatientInfo.RtbiInfo.CoverageId : 0, clonedRequestInfo.MedicationInfo.Icd10Code);

            if (!isScriptPadCandidateChanged && !isPharmacyChangedRefreshNeeded)
            {
                int medIndex = Convert.ToInt32(summaryRequest.MedSearchIndex);
                return ComputePricingRequestDelayTime(DateTime.UtcNow,
                    pptPlus.GetCandidateMedTrnsRecievedTime(responseContainer, medIndex),
                    pptPlus.GetCandidateMedPricingResponseDelayTime(responseContainer, medIndex));
            }

            string previousTrnsId = pptPlus.GetCandidateMedTransactionId(responseContainer, Convert.ToInt32(summaryRequest.MedSearchIndex));
            string fhirRequest = string.Empty;
            string ackResponse = string.Empty;
            pptPlus.RemoveCandidateMedTransactionId(responseContainer, Convert.ToInt32(summaryRequest.MedSearchIndex));
            using (var timer = logger.StartTimer("ConstructPptRequestAndGetAck"))
            {
                fhirRequest = pptPlus.CreatePricingInquiryFhir(new PptPlusBundler(clonedRequestInfo), previousTrnsId, userId, licenseId, dbId);
                timer.Message = $"<request>{fhirRequest}</request>";
                if (!string.IsNullOrWhiteSpace(fhirRequest))
                    ackResponse = pptBroker.InitiatePricingInquiry(fhirRequest, pptToken.Base64SamlToken);
                timer.Message += $"<response>{ackResponse}</response>";
            }

            var ackReponseInfo = pptPlus.ParseAckReponse(ackResponse, pptPlus, userId, licenseId, dbId);

            if (!string.IsNullOrWhiteSpace(ackReponseInfo.TransactionId))
            {
                var candidate = pptPlus.AddScriptPadCandidate(responseContainer, ackReponseInfo.TransactionId, ackReponseInfo.IsValidResponse, Convert.ToInt32(summaryRequest.MedSearchIndex),
                    clonedRequestInfo.MedicationInfo.Name, clonedRequestInfo.MedicationInfo.Strength + " " + clonedRequestInfo.MedicationInfo.StrengthUom,
                    pharmacyID, pharmacyNpi, pharmacyNcpdp, summaryRequest.DDI, clonedRequestInfo.MedicationInfo.Ndc, summaryRequest.GPPC, summaryRequest.PackUom, summaryRequest.Quantity, summaryRequest.PackSize,
                    summaryRequest.PackQuantity, summaryRequest.Refills, summaryRequest.DaysSupply, Convert.ToBoolean(summaryRequest.IsDaw), clonedRequestInfo?.PatientInfo?.RtbiInfo?.CoverageId != null ? clonedRequestInfo.PatientInfo.RtbiInfo.CoverageId : 0,
                    ackReponseInfo.PricingRequestWaitTime, clonedRequestInfo.MedicationInfo.Icd10Code);

                responseDelayMilliSeconds = ComputePricingRequestDelayTime(DateTime.UtcNow, candidate.TrnsReceivedTime, candidate.PricingResponseDelayMilliseconds);
                session[Constants.SessionVariables.PptPlusResponses] = responseContainer;
                commonComponentData.InsertTransactionHeader(ackReponseInfo.TransactionId, previousTrnsId, Convert.ToInt32(Constants.CommonApiRequestTypes.PPT), userId.ToString(), requestInfo.PatientInfo.Id.ToString(), dbId);
                commonComponentData.InsertTransactionDetail(fhirRequest, ackResponse, ackReponseInfo.TransactionId, Convert.ToInt32(Constants.CommonApiTransactionTypes.PPT_InitiatePricinginquiry), dbId);
            }

            return responseDelayMilliSeconds;
        }

        public static PptPlusRequestInfo GetPptRequestInfo(IStateContainer session)
        {
            var pptInfo = new PptPlusRequestInfo
            {
                SiteInfo = session.Cast(Constants.SessionVariables.CommonCompSiteInfo, default(SiteInfo)),
                ProviderInfo = session.Cast(Constants.SessionVariables.CommonCompProviderInfo, default(ProviderInfo)),
                PatientInfo = session.Cast(Constants.SessionVariables.CommonCompPatientInfo, default(PatientInfo)),
                IpAddress = HttpContext.Current?.Request.UserIpAddress(), 
                PharmacyInfo = session.Cast(Constants.SessionVariables.PptPharmacyInfo, default(PharmacyInfo)),
                eRxAppVersion = session.Cast(Constants.SessionVariables.eRxAppVersion, default(string))
            };
            if (pptInfo.PatientInfo != null) pptInfo.PatientInfo.RtbiInfo = session.Cast(Constants.SessionVariables.PptRtbiInfo, default(RtbiInfo));
            return pptInfo;
        }

        public static string GetMedNameByNDC(List<MedicationIdentifier> medicationIdentifier, ConnectionStringPointer dbId)
        {
            string ndcCode = medicationIdentifier.FirstOrDefault(_ => _.IdType == "NDC").IdValue;
            DataSet ds = Allscripts.ePrescribe.Data.Medication.LoadByNDC(ndcCode, dbId);
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                return ds.Tables[0].Rows[0]["MedicationName"].ToString().Trim();
            return string.Empty;

        }

        public static void RequestTransactionForScriptPadMed(IStateContainer session, Rx rx, IPptPlusData pptPlusData, IPptPlus pptPlus, 
            IMedication medication, IPptPlusServiceBroker pptBroker, ICommonComponentData commonComponentData)
        {
            if (!IsPPTPlusPreferenceOn(session))
                return;

            var requestInfo = GetPptRequestInfo(session);
            requestInfo.IpAddress = HttpContext.Current?.Request.UserIpAddress();
            var pptToken = GetPptPlusSamlToken(session);
            var responses = session.Cast(Constants.SessionVariables.PptPlusResponses, default(PptPlusResponseContainer));
            var userId = session.GetGuidOr0x0(Constants.SessionVariables.UserId).ToString();
            var licenseId = session.GetGuidOr0x0(Constants.SessionVariables.LicenseId).ToString();
            var dbId = session.Cast(Constants.SessionVariables.DbId, default(ConnectionStringPointer));
            string icd10Code = session.GetStringOrEmpty(Constants.SessionVariables.ICD10CODE).Trim();

            if (!string.IsNullOrWhiteSpace(pptToken?.Base64SamlToken))
            {
                PptPlusScriptPadMed scriptPadMed = responses?.ScriptPadMeds?.FirstOrDefault(_ => _.RxId == rx.RxID.ToGuidOr0x0());

                var dbid = session.Cast(Constants.SessionVariables.DbId, default(ConnectionStringPointer));
                PptPlusRequestInfo clonedRequestInfo = (PptPlusRequestInfo)requestInfo.Clone();
                clonedRequestInfo.MedicationInfo = pptPlus.CreateMedicationInfo(rx.DDI, rx.GPPC, rx.PackageUOM, rx.Quantity.ToString(), 
                    rx.PackageSize.ToString(), rx.PackageQuantity.ToString(), rx.Refills.ToString(), 
                    rx.DaysSupply.ToString(), rx.DAW, icd10Code, dbid, pptPlusData);

                Guid pharmacyID = requestInfo?.PharmacyInfo != null ? requestInfo.PharmacyInfo.Id : Guid.Empty;
                string pharmacyNcpdp = requestInfo?.PharmacyInfo != null ? requestInfo.PharmacyInfo.Ncpdp : string.Empty;
                string pharmacyNpi = requestInfo?.PharmacyInfo != null ? requestInfo.PharmacyInfo.Npi : string.Empty;

                Guid scriptPadPharmacyID = scriptPadMed?.PharmacyID != null ? scriptPadMed.PharmacyID : Guid.Empty;

                if (scriptPadPharmacyID != Guid.Empty && scriptPadPharmacyID != pharmacyID)
                {
                    var pharmInfo = pptPlusData.GetPharmacyInfo((Guid)(scriptPadPharmacyID), dbid);
                    pharmacyID = scriptPadPharmacyID;
                    pharmacyNcpdp = pharmInfo != null ? pharmInfo.Ncpdp : string.Empty;
                    pharmacyNpi = pharmInfo != null ? pharmInfo.Npi : string.Empty;

                    if (pharmInfo != null && !pharmInfo.IsMailOrderPharmacy)
                    {
                        clonedRequestInfo.PharmacyInfo = pharmInfo;
                    }
                }

                string packUom = string.IsNullOrWhiteSpace(rx.PackageUOM) || rx.PackageUOM.Trim().ToUpper() == "EA" ? rx.DosageFormDescription : rx.PackageUOM;
                if (!pptPlus.IsScriptPadMedInfoChanged(scriptPadMed, clonedRequestInfo.MedicationInfo.Name, clonedRequestInfo.MedicationInfo.Strength + " " + clonedRequestInfo.MedicationInfo.StrengthUom,
                    rx.DDI, rx.GPPC, packUom, rx.Quantity.ToString(), rx.PackageSize.ToString(), rx.PackageQuantity.ToString(), rx.Refills.ToString(), rx.DaysSupply.ToString(), rx.DAW,
                        clonedRequestInfo?.PatientInfo?.RtbiInfo?.CoverageId != null ? clonedRequestInfo.PatientInfo.RtbiInfo.CoverageId : 0, rx.ICD10Code))
                {
                    return;
                }
                string previousTrnsId = session.GetStringOrEmpty(Constants.SessionVariables.PptPlusMedChangeTransactionId);
                session.Remove(Constants.SessionVariables.PptPlusMedChangeTransactionId);
                previousTrnsId = string.IsNullOrWhiteSpace(previousTrnsId) ? pptPlus.GetScriptPadMedTransactionId(scriptPadMed) : previousTrnsId;

                var fhirRequest = pptPlus.CreatePricingInquiryFhir(new PptPlusBundler(clonedRequestInfo), previousTrnsId, userId, licenseId.ToString(), dbid);
                var ackResponse = pptBroker.InitiatePricingInquiry(fhirRequest, pptToken.Base64SamlToken);

                var ackReponseInfo = pptPlus.ParseAckReponse(ackResponse, pptPlus, userId, licenseId, dbId);

                if (!string.IsNullOrWhiteSpace(ackReponseInfo.TransactionId))
                {
                    pptPlus.UpdateScriptPadMedTransaction(responses, ackReponseInfo.TransactionId, ackReponseInfo.IsValidResponse, rx.RxID.ToGuidOr0x0(), clonedRequestInfo.MedicationInfo.Name, rx.Strength + rx.StrengthUOM, pharmacyID, pharmacyNpi, pharmacyNcpdp,
                        rx.DDI, rx.GPPC, rx.PackageUOM, rx.Quantity.ToString(), rx.PackageSize.ToString(), rx.PackageQuantity.ToString(),
                        rx.Refills.ToString(), rx.DaysSupply.ToString(), rx.DAW,
                        clonedRequestInfo?.PatientInfo?.RtbiInfo?.CoverageId != null ? clonedRequestInfo.PatientInfo.RtbiInfo.CoverageId : 0,
                        ackReponseInfo.PricingRequestWaitTime, rx.ICD10Code);

                    commonComponentData.InsertTransactionHeader(ackReponseInfo.TransactionId, previousTrnsId, Convert.ToInt32(Constants.CommonApiRequestTypes.PPT), userId, clonedRequestInfo.PatientInfo.Id.ToString(), dbid);
                    commonComponentData.InsertTransactionDetail(fhirRequest, ackResponse, ackReponseInfo.TransactionId, Convert.ToInt32(Constants.CommonApiTransactionTypes.PPT_InitiatePricinginquiry), dbid);
                }

                session[Constants.SessionVariables.PptPlusResponses] = responses;
            }
        }


        public static PPTPlusSummaryUIResponse RetrieveSummaryUiFromPricingInfo(int medIndex, IStateContainer session, IPptPlus pptPlus, 
            IPptPlusServiceBroker pptPlusServiceBroker, ICommonComponentData commonComponentData, IRtpsHelper rtpsHelper, IRtpsDisposition rtpsDisposition)
        {
            PPTPlusSummaryUIResponse response = new PPTPlusSummaryUIResponse();
            var pptToken = GetPptPlusSamlToken(session);
            var responseContainer = session.Cast(Constants.SessionVariables.PptPlusResponses, default(PptPlusResponseContainer));
            var requestInfo = GetPptRequestInfo(session);
            var userId = session.GetGuidOr0x0(Constants.SessionVariables.UserId);
            var licenseId = session.GetGuidOr0x0(Constants.SessionVariables.LicenseId);
            var patientId = session.GetGuidOr0x0(Constants.SessionVariables.PatientId);
            var dbId = session.Cast(Constants.SessionVariables.DbId, default(ConnectionStringPointer));
            string candidatePricingResponse = string.Empty;

            response.SummaryHtml = string.Empty;
            response.MedIndex = medIndex.ToString();

            if(ShouldDisableWindow(session))
            {
                return response;
            }
            try
            {
                var candidate = responseContainer?.Candidates?.FirstOrDefault(_ => _.MedListIndex == medIndex);
                if (candidate != null)
                {
                    if (!string.IsNullOrWhiteSpace(candidate.TransactionId) && candidate.IsTransactionResponseOk)
                    {
                        if (string.IsNullOrWhiteSpace(candidate.PricingResponse) && !string.IsNullOrWhiteSpace(pptToken?.Base64SamlToken))
                        {
                            candidate.PricingResponse = pptPlusServiceBroker.RetrievePricingInfo(candidate?.TransactionId, requestInfo.SiteInfo.AccountId, HttpContext.Current.Request.UserIpAddress(), pptToken.Base64SamlToken);
                            pptPlus.ParsePricingInfo(candidate, userId.ToString(), licenseId.ToString(), IsEpaOn(session), dbId);
                            commonComponentData.InsertTransactionDetail(string.Empty, candidate.PricingResponse, candidate.TransactionId, Convert.ToInt32(Constants.CommonApiTransactionTypes.PPT_GetPricingInfo), dbId);

                            rtpsHelper.CheckAndSendDisposition(
                            userId.ToString(),
                            licenseId.ToString(),
                            patientId.ToString(),
                            candidate.RelatesToTxId,
                            Disposition.D,
                            rtpsDisposition,
                            dbId);
                        }
                        response.SummaryHtml = candidate.SummaryHtml;
                    }
                }
            }
            catch (Exception e)
            {
                string exceptionDetails = $"Exception retrieving pricing info: <E>{e}</E><candidatePricingResponse> {candidatePricingResponse}</candidatePricingResponse>";
                logger.Debug(exceptionDetails);
                AddException(exceptionDetails, session);
            }

            session[Constants.SessionVariables.PptPlusResponses] = responseContainer;
            logger.Debug($"<RetrieveSummaryUiFromPricingInfo>{responseContainer.ToLogString()}</RetrieveSummaryUiFromPricingInfo>");
            return response;
        }

        public static void GetAllCandidateMedPPTInfo(IStateContainer session)
        {
            var responseContainer = session.Cast(Constants.SessionVariables.PptPlusResponses, default(PptPlusResponseContainer));
            var candidateMedsWithoutResponse = responseContainer?.Candidates?.Where(_ => string.IsNullOrWhiteSpace(_.PricingResponse));

            if ((candidateMedsWithoutResponse == null)
                || (candidateMedsWithoutResponse.Count() <= 0))
                return;

            var requestInfo = GetPptRequestInfo(session);
            requestInfo.IpAddress = HttpContext.Current?.Request.UserIpAddress();
            var dbId = session.Cast(Constants.SessionVariables.DbId, default(ConnectionStringPointer));
            var pptToken = GetPptPlusSamlToken(session);
            var userId = session.GetGuidOr0x0(Constants.SessionVariables.UserId);
            var licenseId = session.GetGuidOr0x0(Constants.SessionVariables.LicenseId);

            var taskArray = new List<Task>();
            var pptTaskResponses = new List<PptPlusScriptPadCandidate>();
            var context = HttpContext.Current;
            bool isRtps = session.GetBooleanOrFalse(Constants.SessionVariables.ShouldShowRtps);

            foreach (PptPlusScriptPadCandidate candidateMed in candidateMedsWithoutResponse)
            {
                PptInfo cpmi = new PptInfo()
                {
                    RequestInfo = (PptPlusRequestInfo)requestInfo.Clone(),
                    MedInfo = (PptPlusScriptPadCandidate)candidateMed.Clone(),
                    PptPlusServiceBroker = new PptPlusServiceBroker(),
                    PptBase64SamlToken = pptToken.Base64SamlToken,
                    CommonComponentData = new CommonComponentData(),
                    DbId = dbId,
                    UserId = userId.ToString(),
                    LicenseId = licenseId.ToString(),
                    PptPlus = new PptPlus(),
                    IsEpaOn = IsEpaOn(session),
                    IsRtps = isRtps,
                    RtpsHelper = new RtpsHelper(),
                    RtpsDisposition = new EPSBroker(),
                };

                taskArray.Add(Task.Run(() =>
                {
                    pptTaskResponses.Add(GetCandidateMedPPTInfo(cpmi, context));
                }));
            }

            Task.WaitAll(taskArray.ToArray());

            foreach (var response in pptTaskResponses)
            {
                var containerCandidateMed = responseContainer?.Candidates?.FirstOrDefault(_ => _.MedListIndex == response.MedListIndex);
                if (containerCandidateMed != null)
                {
                    responseContainer?.Candidates?.Remove(containerCandidateMed);
                    responseContainer?.Candidates?.Add(response);
                }
            }
            session[Constants.SessionVariables.PptPlusResponses] = responseContainer;
        }

        public static void UpdatePharmacyIdToRx(IStateContainer pageState, string rxID, ConnectionStringPointer dbID)
        {
            var responseContainer = pageState.Cast(Constants.SessionVariables.PptPlusResponses, default(PptPlusResponseContainer));
            var med = responseContainer?.ScriptPadMeds?.FirstOrDefault(_ => _.RxId.ToString() == rxID);
            var requestInfo = GetPptRequestInfo(pageState);

            Guid defaultPharmacyID = requestInfo?.PharmacyInfo != null ? requestInfo.PharmacyInfo.Id : Guid.Empty;

            if (!string.IsNullOrWhiteSpace(med?.PharmacyID.ToString()) && !med.PharmacyID.Equals(defaultPharmacyID))
            {
                Allscripts.Impact.Prescription.UpdatePharmacyID(rxID, med.PharmacyID.ToString(), false, false, true, dbID);
            }
        }

        public static bool CheckForPharmacyChange(IStateContainer pageState, ConnectionStringPointer dbID)
        {
            if (!IsPPTPlusPreferenceOn(pageState))
                return false;
            var responseContainer = pageState.Cast(Constants.SessionVariables.PptPlusResponses, default(PptPlusResponseContainer));
            var requestInfo = GetPptRequestInfo(pageState);
            Guid defaultPharmacyID = requestInfo?.PharmacyInfo != null ? requestInfo.PharmacyInfo.Id : Guid.Empty;

            if (responseContainer != null)
            {
                foreach (PptPlusScriptPadMed scriptPadMed in responseContainer?.ScriptPadMeds)
                {
                    if (!scriptPadMed.PharmacyID.Equals(defaultPharmacyID))
                        return true;
                }
            }
            return false;
        }

        public static void UpdateTransactionIdToRx(IStateContainer pageState, string rxID, IPptPlusData pptPlusData, ConnectionStringPointer dbID)
        {
            var responseContainer = pageState.Cast(Constants.SessionVariables.PptPlusResponses, default(PptPlusResponseContainer));
            var med = responseContainer?.ScriptPadMeds?.FirstOrDefault(_ => _.RxId.ToString() == rxID);

            if (med != null && !string.IsNullOrWhiteSpace(med.TransactionId.ToString()))
            {
                pptPlusData.UpdateTransactionIdToRx(rxID, med.TransactionId.ToString(), dbID);
            }
        }

        public static PptPlusScriptPadCandidate GetCandidateMedPPTInfo(Object candidateMedInfoObj, HttpContext context)
        {
            HttpContext.Current = context;
            PptInfo candidateMedInfo = (PptInfo)candidateMedInfoObj;
            PptPlusRequestInfo reqInfo = candidateMedInfo.RequestInfo;
            PptPlusScriptPadCandidate candidateMed = (PptPlusScriptPadCandidate)candidateMedInfo.MedInfo;

            if (!string.IsNullOrWhiteSpace(candidateMed.TransactionId) && candidateMed.IsTransactionResponseOk)
            {
                int threadSleepTime = ComputePricingRequestDelayTime(DateTime.UtcNow, 
                    candidateMed.TrnsReceivedTime,
                    candidateMed.PricingResponseDelayMilliseconds);

                if (threadSleepTime > 0)
                {
                    Thread.Sleep(threadSleepTime);
                }

                candidateMed.PricingResponse = candidateMedInfo.PptPlusServiceBroker.RetrievePricingInfo(candidateMed.TransactionId,
                                                                            reqInfo.SiteInfo.AccountId,
                                                                            reqInfo.IpAddress, candidateMedInfo.PptBase64SamlToken);
                candidateMedInfo.PptPlus.ParsePricingInfo(candidateMed, candidateMedInfo.UserId, candidateMedInfo.LicenseId, candidateMedInfo.IsEpaOn, candidateMedInfo.DbId);
                candidateMedInfo.CommonComponentData.InsertTransactionDetail(string.Empty, candidateMed.PricingResponse, candidateMed.TransactionId, Convert.ToInt32(Constants.CommonApiTransactionTypes.PPT_GetPricingInfo), candidateMedInfo.DbId);

                candidateMedInfo.RtpsHelper.CheckAndSendDisposition(
                    candidateMedInfo.UserId,
                    candidateMedInfo.LicenseId,
                    reqInfo.PatientInfo.Id.ToString(),
                    candidateMed.RelatesToTxId,
                    Disposition.D,
                    candidateMedInfo.RtpsDisposition,
                    candidateMedInfo.DbId
                    );
            }

            return candidateMed;
        }

        public static void GetAllScriptPadMedPPTInfo(IStateContainer session)
        {
            if (!IsPPTPlusPreferenceOn(session))
                return;

            var responseContainer = session.Cast(Constants.SessionVariables.PptPlusResponses, default(PptPlusResponseContainer));
            var scriptPadMedsWithoutResponse = responseContainer?.ScriptPadMeds?.Where(_ => string.IsNullOrWhiteSpace(_.PricingResponse)).Where(_ => !string.IsNullOrWhiteSpace(_.DDI));

            if ((scriptPadMedsWithoutResponse == null)
                || (scriptPadMedsWithoutResponse.Count() <= 0))
                return;

            var requestInfo = GetPptRequestInfo(session);
            requestInfo.IpAddress = HttpContext.Current?.Request.UserIpAddress();
            var pptToken = GetPptPlusSamlToken(session);

            if (pptToken == null)
                return;

            var dbId = session.Cast(Constants.SessionVariables.DbId, default(ConnectionStringPointer));
            var userId = session.GetGuidOr0x0(Constants.SessionVariables.UserId);
            var licenseId = session.GetGuidOr0x0(Constants.SessionVariables.LicenseId);
            bool isRtps = session.GetBooleanOrFalse(Constants.SessionVariables.ShouldShowRtps);

            var taskArray = new List<Task>();
            var pptTaskResponses = new List<PptPlusScriptPadMed>();
            var context = HttpContext.Current;
            foreach (PptPlusScriptPadMed scriptPadMed in scriptPadMedsWithoutResponse)
            {
                PptInfo spmi = new PptInfo()
                {
                    RequestInfo = (PptPlusRequestInfo)requestInfo.Clone(),
                    MedInfo = (PptPlusScriptPadMed)scriptPadMed.Clone(),
                    PptPlus = new PptPlus(),
                    PptPlusData = new PptPlusData(),
                    CommonComponentData = new CommonComponentData(),
                    Medication = new Allscripts.ePrescribe.Data.Medication(),
                    PptPlusServiceBroker = new PptPlusServiceBroker(),
                    DbId = dbId,
                    PptBase64SamlToken = pptToken.Base64SamlToken,
                    UserId = userId.ToString(),
                    LicenseId = licenseId.ToString(),
                    IsEpaOn = IsEpaOn(session),
                    IsRtps = isRtps,
                    RtpsHelper = new RtpsHelper(),
                    RtpsDisposition = new EPSBroker(),
                };

                taskArray.Add(Task.Run(() =>
                {
                    pptTaskResponses.Add(GetScriptPadMedPPTInfo(spmi, context));
                }));
            }

            try
            {
                 Task.WaitAll(taskArray.ToArray());

                foreach (var response in pptTaskResponses)
                {
                    var containerScriptPadMed = responseContainer?.ScriptPadMeds?.FirstOrDefault(_ => _.RxId == response.RxId);
                    if (containerScriptPadMed != null)
                    {
                        responseContainer?.ScriptPadMeds?.Remove(containerScriptPadMed);
                        responseContainer?.ScriptPadMeds?.Add(response);
                    }
                }

                session[Constants.SessionVariables.PptPlusResponses] = responseContainer;
            }
            catch (AggregateException e)
            {
                string exceptionDetails = $"Exception retrieving Script Pad Med PPT info: <E>{e}</E>";
                logger.Debug(exceptionDetails);
                AddException(exceptionDetails, session);
            }
        }

        public static PptPlusScriptPadMed GetScriptPadMedPPTInfo(Object scriptPadMedInfoObj, 
            HttpContext context)
        {
            HttpContext.Current = context;
            PptInfo scriptPadMedInfo = (PptInfo)scriptPadMedInfoObj;
            PptPlusRequestInfo reqInfo = scriptPadMedInfo.RequestInfo;
            PptPlusScriptPadMed scriptPadMed = (PptPlusScriptPadMed)scriptPadMedInfo.MedInfo;

            if (string.IsNullOrWhiteSpace(scriptPadMed.TransactionId))  // if we don't have it already,  get it
            {
                //Make the first PPT request
                reqInfo.MedicationInfo = scriptPadMedInfo.PptPlus.CreateMedicationInfo(scriptPadMed.DDI,
                    scriptPadMed.GPPC,
                    scriptPadMed.PackageUOM,
                    scriptPadMed.Quantity,
                    scriptPadMed.PackSize,
                    scriptPadMed.PackQuantity,
                    scriptPadMed.Refills,
                    scriptPadMed.DaysSupply,
                    scriptPadMed.IsDaw,
                    scriptPadMed.ICD10Code,
                    scriptPadMedInfo.DbId,
                    scriptPadMedInfo.PptPlusData);

                scriptPadMed.DrugName = reqInfo.MedicationInfo.Name;

                Guid pharmacyID = reqInfo?.PharmacyInfo != null ? reqInfo.PharmacyInfo.Id : Guid.Empty;
                Guid scriptPadPharmacyID = scriptPadMed?.PharmacyID != null ? scriptPadMed.PharmacyID : Guid.Empty;

                if (scriptPadPharmacyID != Guid.Empty && scriptPadPharmacyID != pharmacyID)
                {
                    var pharmInfo = scriptPadMedInfo.PptPlusData.GetPharmacyInfo((Guid)(scriptPadPharmacyID), scriptPadMedInfo.DbId);
                    if (pharmInfo != null && !pharmInfo.IsMailOrderPharmacy)
                    {
                        reqInfo.PharmacyInfo = pharmInfo;
                    }
                }

                reqInfo.PatientInfo.RtbiInfo = scriptPadMedInfo.PptPlusData.GetPatientRtbiInto(reqInfo.PatientInfo.Id,
                                                                                                scriptPadMed.CoverageID.ToString(),
                                                                                                scriptPadMedInfo.DbId);

                string prevTransactionId = scriptPadMed.LastTransactionId;
                var fhirRequest = scriptPadMedInfo.PptPlus.CreatePricingInquiryFhir(new PptPlusBundler(reqInfo), prevTransactionId, scriptPadMedInfo.UserId, Guid.Empty.ToString(), scriptPadMedInfo.DbId);
                var ackResponse = scriptPadMedInfo.PptPlusServiceBroker.InitiatePricingInquiry(fhirRequest, scriptPadMedInfo.PptBase64SamlToken);

                var ackReponseInfo = scriptPadMedInfo.PptPlus.ParseAckReponse(ackResponse, scriptPadMedInfo.PptPlus, 
                    scriptPadMedInfo.UserId, scriptPadMedInfo.LicenseId, scriptPadMedInfo.DbId);


                if (!string.IsNullOrWhiteSpace(ackReponseInfo.TransactionId))
                {
                    scriptPadMed.TransactionId = ackReponseInfo.TransactionId;
                    scriptPadMed.TrnsReceivedTime = DateTime.UtcNow;
                    scriptPadMed.IsTransactionResponseOk = ackReponseInfo.IsValidResponse;
                    scriptPadMed.PricingResponseDelayMilliseconds = ackReponseInfo.PricingRequestWaitTime;
                    scriptPadMedInfo.CommonComponentData.InsertTransactionHeader(ackReponseInfo.TransactionId, prevTransactionId, Convert.ToInt32(Constants.CommonApiRequestTypes.PPT), scriptPadMedInfo.UserId, reqInfo.PatientInfo.Id.ToString(), scriptPadMedInfo.DbId);
                    scriptPadMedInfo.CommonComponentData.InsertTransactionDetail(fhirRequest, ackResponse, ackReponseInfo.TransactionId, Convert.ToInt32(Constants.CommonApiTransactionTypes.PPT_InitiatePricinginquiry), scriptPadMedInfo.DbId);
                    scriptPadMedInfo.PptPlusData.UpdateTransactionIdToRx(scriptPadMed.RxId.ToString(), ackReponseInfo.TransactionId, scriptPadMedInfo.DbId);
                }

                Thread.Sleep(scriptPadMed.PricingResponseDelayMilliseconds);
            }

            if (!string.IsNullOrWhiteSpace(scriptPadMed.TransactionId) 
                && scriptPadMed.IsTransactionResponseOk) //Make second request if first request is success
            {
                int threadSleepTime = ComputePricingRequestDelayTime(DateTime.UtcNow, scriptPadMed.TrnsReceivedTime,
                   scriptPadMed.PricingResponseDelayMilliseconds);

                if (threadSleepTime > 0)
                {
                    Thread.Sleep(threadSleepTime);
                }

                scriptPadMed.PricingResponse = scriptPadMedInfo.PptPlusServiceBroker.RetrievePricingInfo(
                    scriptPadMed.TransactionId,
                    reqInfo.SiteInfo.AccountId,
                    reqInfo.IpAddress, scriptPadMedInfo.PptBase64SamlToken);

                scriptPadMedInfo.PptPlus.ParsePricingInfo(scriptPadMed, scriptPadMedInfo.UserId,
                    scriptPadMedInfo.LicenseId, scriptPadMedInfo.IsEpaOn, scriptPadMedInfo.DbId);

                scriptPadMedInfo.CommonComponentData.InsertTransactionDetail(string.Empty, scriptPadMed.PricingResponse,
                   scriptPadMed.TransactionId, Convert.ToInt32(Constants.CommonApiTransactionTypes.PPT_GetPricingInfo),
                   scriptPadMedInfo.DbId);

                scriptPadMedInfo.RtpsHelper.CheckAndSendDisposition(
                    scriptPadMedInfo.UserId,
                    scriptPadMedInfo.LicenseId,
                    reqInfo.PatientInfo.Id.ToString(),
                    scriptPadMed.RelatesToTxId,
                    Disposition.D,
                    scriptPadMedInfo.RtpsDisposition,
                    scriptPadMedInfo.DbId
                    );

               
            }

            return scriptPadMed;
        }

        public static int ComputePricingRequestDelayTime(DateTime higherDt, DateTime lowerDt, int maxDelayMilliSeconds)
        {
            if (higherDt != null && lowerDt != null)
            {
                if (higherDt <= lowerDt)
                    return maxDelayMilliSeconds;

                int delayMilliSeconds = Convert.ToInt32((higherDt - lowerDt).TotalMilliseconds);
                if ((delayMilliSeconds >= 0) && (delayMilliSeconds < maxDelayMilliSeconds))
                {
                    return (maxDelayMilliSeconds - delayMilliSeconds);
                }
            }
            return 0;
        }

        public static List<string> RetrieveAllScriptPadMedSummaryUi(IStateContainer session, IPptPlus pptPlus)
        {
            var responseContainer = session.Cast(Constants.SessionVariables.PptPlusResponses, default(PptPlusResponseContainer));
            string userId = session.GetGuidOr0x0(SessionVariables.UserId).ToString();
            string licenseId = session.GetGuidOr0x0(SessionVariables.LicenseId).ToString();
            ConnectionStringPointer dbId = session.Cast(Constants.SessionVariables.DbId, default(ConnectionStringPointer));

            List<string> summaryHtmls = new List<string>();
            foreach (var scriptPadMed in responseContainer?.ScriptPadMeds?.OrderByDescending(_ => _.UpdatedTime))
            {
                string summaryHtml = scriptPadMed.SummaryHtml;
                summaryHtml = string.IsNullOrWhiteSpace(summaryHtml)
                    ? getBlankSummaryHtml(scriptPadMed.DrugName, scriptPadMed.DrugStrength)
                    : summaryHtml;

                summaryHtmls.Add(summaryHtml);
            }
            return summaryHtmls;
        }

        private static string getBlankSummaryHtml(string drugName, string drugStrength)
        {
            StringBuilder str = new StringBuilder();
            str.Append("<div class='summary' style='border-width:1px; border-style:solid; border-color:#665999; min-height:124px; background-color:#fff; position: relative'>");
            str.Append("<div class='header' style='color:#fff;background-color:#665999;margin-bottom:0;height:24px;'>");
            str.Append("<div style='padding-left: 2px;color:#fff;background-color:#665999;margin-bottom:0;height:24px;'>");
            str.Append("<div style='padding-left: 2px;color: #fff;background-color: #665999;margin-bottom: 0;height: 24px;overflow: hidden;white-space: nowrap;text-overflow: ellipsis;line-height: 23px;width: 95%;'>");
            str.Append($"<span style='font-weight: bold; white-space: nowrap; padding-left: 3px; color: #fff; background-color: #665999; margin-bottom: 0; line-height: 23px; cursor: default;' title='{drugName}, {drugStrength}'> {drugName}, {drugStrength} </span>");
            str.Append("</div>");
            str.Append("</div>");
            str.Append("</div>");
            str.Append("<div class='row' style='padding-top:2px; padding-bottom:4px; padding-right:18px; padding-left:17px; font-family:HelveticaNeue,Arial; height:19px; color:#333;'>");
            str.Append("<div class='caption' style='width:60%;float:left;font-size:12px;'>Insurance</div>");
            str.Append("<div class='value' style='float:right; width:35%; text-align:right; font-size:12px; position: relative; margin-right:3px;'> <span>N/A</span></div>");
            str.Append("</div>");
            str.Append("<div class='row' style='padding-top:2px; padding-bottom:4px; padding-right:18px; padding-left:17px; font-family:HelveticaNeue,Arial; height:19px; color:#333;'>");
            str.Append("<div class='caption' style='width:60%; float:left; font-size:12px;'>Ins. 90-day</div>");
            str.Append("<div class='value' style='float:right; width:35%; text-align:right; font-size:12px; position: relative; margin-right:3px;'><span>N/A</span></div>");
            str.Append("</div>");
            str.Append("<div class='row' style='padding-top:2px; padding-bottom:4px; padding-right:18px; padding-left:17px; font-family:HelveticaNeue,Arial; height:19px; color:#333;'>");
            str.Append("<div class='caption' style='width:60%; float:left; font-size:12px;'>Cash w/ Offer</div>");
            str.Append("<div class='value' style='float:right; width:35%; text-align:right; font-size:12px; position: relative; margin-right:3px;'><span>N/A</span></div>");
            str.Append("</div>");
            str.Append("<div class='row' style='padding-top:2px; padding-bottom:4px; padding-right:18px; padding-left:17px; font-family:HelveticaNeue,Arial; height:19px; color:#333;'>");
            str.Append("<div class='caption' style='width:60%; float:left; font-size:12px;'>Low As</div>");
            str.Append("<div class='value' style='float:right; width:35%; text-align:right; font-size:12px; position: relative; margin-right:3px;'><span>N/A</span></div>");
            str.Append("</div>");
            str.Append("</div>");
            return str.ToString();
        }
        public static PPTPlusDetailsUserChangesResponse PptDetailsHandleOKClick(PPTPlusUserChanges request, 
            IStateContainer session, 
            IRtpsDisposition rtpsDisposition, 
            IRtpsHelper rtpsHelper)
        {
            var response = new PPTPlusDetailsUserChangesResponse();
            response.Status = PPTPlusDetailsUserChangeStatus.NoChange;
            response.PageContext = request.pageContext;
            string userChangesString = JsonConvert.SerializeObject(request.userChanges);
            logger.Debug($"PPTPlusDetailsUserChanges: <BubbledEvent> { userChangesString } </BubbledEvent>");


            PPTDetailsUserEvents userEvents;
            bool medChangeAlreadyFound = false;
            bool atleastOnePharmacyChange = false;

            var pptPlusResponse = session.Cast(Constants.SessionVariables.PptPlusResponses, default(PptPlusResponseContainer));
            var dbId = session.Cast(Constants.SessionVariables.DbId, default(ConnectionStringPointer));
            long commonUIRequestId = session.GetLong(Constants.SessionVariables.PptPlusCommonUiRequestId, -1);

            try
            {
              
                userEvents = request.userChanges;
                if (userEvents?.SelectionEvents != null && userEvents.SelectionEvents.Count > 0)
                {
                    foreach (var selectionEvent in userEvents.SelectionEvents)
                    {
                        foreach (var eventData in selectionEvent.EventData)
                        {
                            if (eventData.EventType == EventType.AlternativeMed)
                            {
                                if (medChangeAlreadyFound)
                                {
                                    break;
                                }

                                AlternateMedDispositionRequestCRx(session, 
                                    selectionEvent.TransactionID, 
                                    pptPlusResponse, 
                                    rtpsDisposition, 
                                    rtpsHelper);

                                if (request.pageContext ==  PptDetailContext.CandidateMed)
                                {
                                    PPTPlus.ClearScriptPadCandidates(session);
                                }
                                else if (request.pageContext == PptDetailContext.ScriptPadMed)
                                {
                                    PPTPlus.RemoveMedFromScriptPad(session, selectionEvent.TransactionID, pptPlusResponse.ScriptPadMeds);
                                }
                                medChangeAlreadyFound = true;

                                string medname = PPTPlus.GetMedNameByNDC(eventData.SelectedMed.MedicationIdentifier, dbId);
                                if (string.IsNullOrEmpty(medname))
                                {
                                    string medMissingText = "Medication cannot be found for " + eventData.SelectedMed.MedicationName + " with NDC: " + eventData.SelectedMed.MedicationIdentifier.FirstOrDefault(_ => _.IdType == "NDC").IdValue;
                                    string exceptionID = AddException(medMissingText, session);
                                    response.Status = PPTPlusDetailsUserChangeStatus.Fail;
                                    response.Message = $"We apologize, but the " + medMissingText + " Please select a different medication or Cancel to close this popup.";
                                    logger.Debug($"PPTPlusDetailsUserChanges: <Exception> { medMissingText } </Exception>");
                                }
                                else
                                {
                                    response.DrugName = medname;
                                    response.Status = PPTPlusDetailsUserChangeStatus.MedChange;
                                    session[Constants.SessionVariables.PptPlusMedChangeTransactionId] = selectionEvent.TransactionID;
                                }
                            }
                            else if (eventData.EventType == EventType.PharmacyChange)
                            {
                                var pharmacy = PPTPlus.GetPharmacy(session, eventData.SelectedPharmacy.PharmacyIdentifier);
                                if (pharmacy != null)
                                {
                                    bool isPharmacyChangeSuccessful = PPTPlus.UpdateNewPharmacyID(session, selectionEvent.TransactionID, pptPlusResponse, request.pageContext, medChangeAlreadyFound, response, Guid.Parse(pharmacy.PharmacyID), pharmacy.NABP, pharmacy.NPI);
                                    atleastOnePharmacyChange = atleastOnePharmacyChange || isPharmacyChangeSuccessful;
                                }
                                else
                                {
                                    response.Status = (medChangeAlreadyFound || atleastOnePharmacyChange) ? response.Status : PPTPlusDetailsUserChangeStatus.PharmacyMissing;
                                    response.Message = "Sorry, the pharmacy couldn't be found.";
                                }
                            }
                            else if (eventData.EventType == EventType.None)
                            {
                                response.Status = PPTPlusDetailsUserChangeStatus.NoChange;
                            }
                        }
                    }
                }
                else
                {
                    response.Status = PPTPlusDetailsUserChangeStatus.NoChange;
                }

                if (!medChangeAlreadyFound && atleastOnePharmacyChange)
                {
                    response.Status = PPTPlusDetailsUserChangeStatus.PharmacyChangeOnly;
                }

                if (userEvents?.PaButtonEvents != null && userEvents.PaButtonEvents.Count > 0)
                {
                    foreach (var paButtonEvent in userEvents.PaButtonEvents)
                    {
                        UpdatePptMedPaInfo(pptPlusResponse, paButtonEvent);
                    }
                    if(response.Status == PPTPlusDetailsUserChangeStatus.NoChange)
                    {
                        response.Status = PPTPlusDetailsUserChangeStatus.PriorAuthChangeOnly;
                    }
                }
            }
            catch (Exception ex)
            {
                string exceptionID = AddException(ex.ToString(), session);
                response.Status = PPTPlusDetailsUserChangeStatus.Fail;
                response.Message = $"We apologize, but an error has occurred. Error Reference ID = " + exceptionID + ". You can click OK to retry or Cancel to close this popup.";
                logger.Debug($"PPTPlusDetailsUserChanges: <Exception> { ex.ToString() } </Exception>");
            }

            if (commonUIRequestId != -1)
            {
                CommonComponentData commonComponentData = new CommonComponentData();
                commonComponentData.InsertCommonUIEventData(commonUIRequestId, userChangesString, dbId);
            }

            return response;
        }

        public static GetMatchingPharmacyResponse GetPharmacy(IStateContainer session, PharmacyIdentifier pharmacyIdentifier)
        {
            var dbID = session.Cast(Constants.SessionVariables.DbId, default(ConnectionStringPointer));
            var pharmacy = EPSBroker.GetMatchingPharmacy(pharmacyIdentifier.IdValue, dbID);
            return pharmacy;
        }

        public static bool UpdateNewPharmacyID(IStateContainer session, string transID, PptPlusResponseContainer pptPlusResponse,
            Constants.PptDetailContext pageContext, bool medChangeAlreadyFound, PPTPlusDetailsUserChangesResponse response, Guid pharmacyID, string pharmacyNcpdp, string pharmacyNpi)
        {
            bool isPharmacyChanged = false;
            var dbID = session.Cast(Constants.SessionVariables.DbId, default(ConnectionStringPointer));

            if (pageContext ==  PptDetailContext.CandidateMed)
            {
                if (pptPlusResponse.Candidates.FirstOrDefault(_ => _.TransactionId.ToLower() == transID.ToLower()) != null)
                {
                    if (!medChangeAlreadyFound)
                    {
                        var candidateMed = pptPlusResponse.Candidates.FirstOrDefault(_ => _.TransactionId.ToLower() == transID.ToLower());
                        candidateMed.PharmacyID = pharmacyID;
                        candidateMed.PharmacyNcpdp = !string.IsNullOrWhiteSpace(pharmacyNcpdp) ? pharmacyNcpdp.Trim() : string.Empty;
                        candidateMed.PharmacyNpi = !string.IsNullOrWhiteSpace(pharmacyNpi) ? pharmacyNpi.Trim() : string.Empty;
                        response.MedSearchIndexes.Add(candidateMed.MedListIndex.ToString());
                        isPharmacyChanged = true;
                    }
                }
                else if (pptPlusResponse.ScriptPadMeds.FirstOrDefault(_ => _.TransactionId.ToLower() == transID.ToLower()) != null)
                {
                    var med = pptPlusResponse.ScriptPadMeds.FirstOrDefault(_ => _.TransactionId.ToLower() == transID.ToLower());
                    Allscripts.Impact.Prescription.UpdatePharmacyID(med.RxId.ToString(), pharmacyID.ToString(), false, false, true, dbID);
                    isPharmacyChanged = true;
                }
            }
            else if (pageContext ==  PptDetailContext.ScriptPadMed)
            {
                var med = pptPlusResponse.ScriptPadMeds.FirstOrDefault(_ => _.TransactionId.ToLower() == transID.ToLower());
                Allscripts.Impact.Prescription.UpdatePharmacyID(med.RxId.ToString(), pharmacyID.ToString(), false, false, true, dbID);
                isPharmacyChanged = true;
            }
            session[Constants.SessionVariables.PptPlusResponses] = pptPlusResponse;

            return isPharmacyChanged;
        }

        public static void UpdatePptMedPaInfo(PptPlusResponseContainer pptPlusResponseContainer, PaButtonEvent paBtnEvent)
        {
            PptPlusResponseBase med = null;
            if (pptPlusResponseContainer?.Candidates?.FirstOrDefault(_ => _.TransactionId.ToLower() == paBtnEvent.TransactionID.ToLower()) != null)
            {
                med = pptPlusResponseContainer.Candidates.FirstOrDefault(_ => _.TransactionId.ToLower() == paBtnEvent.TransactionID.ToLower());
            }
            else if (pptPlusResponseContainer?.ScriptPadMeds?.FirstOrDefault(_ => _.TransactionId.ToLower() == paBtnEvent.TransactionID.ToLower()) != null)
            {
                med = pptPlusResponseContainer.ScriptPadMeds.FirstOrDefault(_ => _.TransactionId.ToLower() == paBtnEvent.TransactionID.ToLower());
            }

            if (med == null)
                return;
            med.IsInitiatePriorAuthToggled = paBtnEvent.ButtonState == PaButtonState.PaInitiated;
        }
        //
        public static string RetrieveSurescriptsResponseMessageIdOrEmpty(Guid rxId, PptPlusResponseContainer responseContainer, Guid userId, Guid licenseId, ConnectionStringPointer dbId, IAudit audit)
        {
            try
            {
                var scriptPadMed = responseContainer?.ScriptPadMeds?.FirstOrDefault(_ => _.RxId == rxId);

                if (!string.IsNullOrWhiteSpace(scriptPadMed?.PricingResponse))
                {
                    var pricingToken = JToken.Parse(scriptPadMed.PricingResponse);

                    if (pricingToken["cards"].HasValues)
                    {
                        var card = pricingToken["cards"].SelectToken($"[?(@source.label  == '{FhirConstants.SureScriptsPartnerResponseLabel}')]");
                        if (card != null)
                        {
                            var headerToken = card["suggestions"][0]["actions"][0]["resource"]["entry"].SelectToken($"[?(@resource.resourceType  == '{ResourceType.MessageHeader}')]");
                            var headerResponseCode = headerToken["resource"]["response"]["code"].ToString();
                            if (headerResponseCode.Equals("ok", StringComparison.OrdinalIgnoreCase))
                            {
                                var extensionUrl = headerToken.SelectToken($".resource.extension[?(@url == '{FhirConstants.ExtensionUrl.PartnerResponseId}')]");
                                if (extensionUrl != null && !string.IsNullOrWhiteSpace(Convert.ToString(extensionUrl["valueString"])))
                                {
                                    return Convert.ToString(extensionUrl["valueString"]);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                var msg = $"RetrieveSurescriptsResponseMessageIdOrEmptyException(): <ResponseContainer>{responseContainer.StringifyObject()}</ResponseContainer><RxId>{rxId}</RxId><Exception>{e.ToString()}<Exception>";
                audit.AddException(userId.ToString(), licenseId.ToString(), msg, string.Empty, string.Empty, string.Empty, dbId);
                logger.Error(msg);
            }
            return string.Empty;
        }

        public static void SendUsageReport(IStateContainer session, PptPlus pptPlus, Guid rxId, bool isGoodRxCouponPrinted, 
            bool isGoodRxCouponSentToPharmacy, IPptPlusServiceBroker pptPlusServiceBroker, IPptPlusData pptPlusData, 
            ICommonComponentData commonComponentData)
        {
            string usageReportFhirRequest = string.Empty;
            string ackResponse = string.Empty;
            try
            {
                var responseContainer = session.Cast(Constants.SessionVariables.PptPlusResponses, default(PptPlusResponseContainer));
                var scriptPadMed = responseContainer?.ScriptPadMeds?.FirstOrDefault(_ => _.RxId == rxId);
                var requestInfo = GetPptRequestInfo(session);
                var pptToken = GetPptPlusSamlToken(session);
                var dbId = session.Cast(Constants.SessionVariables.DbId, default(ConnectionStringPointer));

                if (scriptPadMed != null)
                {
                    if ((!string.IsNullOrWhiteSpace(scriptPadMed.PharmacyNcpdp))
                            || (!string.IsNullOrWhiteSpace(scriptPadMed.PharmacyNpi)))
                    {
                        bool isGoodRxCouponAvailable = scriptPadMed.IsCouponAvailable;
                        bool isGoodRxCouponDistributed = (isGoodRxCouponPrinted || isGoodRxCouponSentToPharmacy);
                        var pricingUsageInfo = pptPlus.ParseUsageInfo(scriptPadMed.PricingResponse, scriptPadMed.PharmacyNcpdp, scriptPadMed.PharmacyNpi, isGoodRxCouponAvailable, isGoodRxCouponDistributed);

                        if (!string.IsNullOrWhiteSpace(pricingUsageInfo))
                        {
                            requestInfo.MedicationInfo = pptPlus.CreateMedicationInfo(scriptPadMed.DDI,
                                scriptPadMed.GPPC,
                                scriptPadMed.PackageUOM,
                                scriptPadMed.Quantity,
                                scriptPadMed.PackSize,
                                scriptPadMed.PackQuantity,
                                scriptPadMed.Refills,
                                scriptPadMed.DaysSupply,
                                scriptPadMed.IsDaw,
                                scriptPadMed.ICD10Code,
                                dbId,
                                pptPlusData
                                );

                            usageReportFhirRequest = pptPlus.CreateUsageReportRequestFhir(new PptPlusBundler(requestInfo),
                                                                                                pricingUsageInfo,
                                                                                                scriptPadMed.TransactionId,
                                                                                                GetCouponDispositionValue(session, isGoodRxCouponAvailable, isGoodRxCouponPrinted, isGoodRxCouponSentToPharmacy),
                                                                                                GetCouponDistributionValue(isGoodRxCouponPrinted, isGoodRxCouponSentToPharmacy, isGoodRxCouponAvailable),
                                                                                                dbId);
                            ackResponse = pptPlusServiceBroker.SubmitUsageReportRequest(usageReportFhirRequest, pptToken.Base64SamlToken);

                            if (!string.IsNullOrWhiteSpace(ackResponse))
                            {
                                commonComponentData.InsertTransactionDetail(usageReportFhirRequest, ackResponse,
                                 scriptPadMed.TransactionId, Convert.ToInt32(Constants.CommonApiTransactionTypes.PPT_ReportSelection), dbId);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                string exceptionDetails = $"Exception retrieving coupon info: <E>{e}</E> <request>{usageReportFhirRequest}</request> <response>{ackResponse}</response>";
                logger.Debug(exceptionDetails);
                AddException(exceptionDetails, session);
            }
        }

        public static string GetCouponDispositionValue(IStateContainer session, bool isGoodRxCouponAvailable, bool isGoodRxCouponPrinted, bool isGoodRxCouponSentToPharmacy)
        {
            if (!isGoodRxCouponAvailable)
            {
                return string.Empty;
            }
            if(isGoodRxCouponSentToPharmacy)
            {
                return FhirConstants.CouponDisposition.Activelydistributed;
            }
            bool provAllowsPpt = session.GetBooleanOrFalse(Constants.SessionVariables.ShouldShowPpt);
            bool provPrintOfferAutomatically = provAllowsPpt && session.GetBooleanOrFalse(Constants.SessionVariables.ShouldPrintOfferAutomatically);

            if (provPrintOfferAutomatically)
            {
                return isGoodRxCouponPrinted ? FhirConstants.CouponDisposition.PassivelyDistributed
                                             : FhirConstants.CouponDisposition.ActivelyNotDistributed;
            }
            else
            {
                return isGoodRxCouponPrinted ? FhirConstants.CouponDisposition.Activelydistributed
                                             : FhirConstants.CouponDisposition.PassivelyNotDistributed;
            }
        }
        public static string GetCouponDistributionValue(bool isGoodRxCouponPrinted, bool isGoodRxCouponSentToPharmacy, bool isGoodRxCouponAvailable)
        {
            if (!isGoodRxCouponAvailable)
                return FhirConstants.CouponDistribution.None;
            string couponDistributionValue = FhirConstants.CouponDistribution.None;
            if(isGoodRxCouponPrinted && isGoodRxCouponSentToPharmacy)
            {
                couponDistributionValue = FhirConstants.CouponDistribution.PrintAndSend;
            }
            else if(isGoodRxCouponPrinted && !isGoodRxCouponSentToPharmacy)
            {
                couponDistributionValue = FhirConstants.CouponDistribution.Print;
            }
            else if (!isGoodRxCouponPrinted && isGoodRxCouponSentToPharmacy)
            {
                couponDistributionValue = FhirConstants.CouponDistribution.SendToPharmacy;
            }
            return couponDistributionValue;
        }

        public static void AuditCouponStatus(IStateContainer session, Guid rxId, 
            bool isPrintUnchecked, bool isCouponPrinted,
            bool isSentToPharmacyChecked, bool isCouponSentToPharmacy,
            PptPlusData pptPlusData)
        {
            var responseContainer = session.Cast(Constants.SessionVariables.PptPlusResponses, default(PptPlusResponseContainer));
            var scriptPadMed = responseContainer?.ScriptPadMeds?.FirstOrDefault(_ => _.RxId == rxId);
            var dbId = session.Cast(Constants.SessionVariables.DbId, default(ConnectionStringPointer));
            bool isUserEnabledForCouponPrinting = session.GetBooleanOrFalse(Constants.SessionVariables.ShouldPrintOfferAutomatically);

            if (scriptPadMed != null)
            {
                pptPlusData.AuditCouponStatus(scriptPadMed.TransactionId, 
                    isUserEnabledForCouponPrinting, isPrintUnchecked, isCouponPrinted,
                    isSentToPharmacyChecked, isCouponSentToPharmacy, dbId);
            }
        }


        public static GetPPTPlusSamlTokenResponse GetPptPlusSamlToken(IStateContainer session)
        {
            var sessionLicense = session.Cast("SessionLicense", default(ApplicationLicense));
            var pptToken = session.Cast(Constants.SessionVariables.pptPlusTokenObject, default(GetPPTPlusSamlTokenResponse));

            if (!string.IsNullOrWhiteSpace(pptToken?.Base64SamlToken))
            {
                if (DateTime.UtcNow > pptToken.TokenRefreshTime)
                {
                    pptToken = EPSBroker.RefreshPPTPlusSamlToken(pptToken.RawSamlToken, session.GetStringOrEmpty("ShieldExternalTenantID"));
                    session[Constants.SessionVariables.pptPlusTokenObject] = pptToken;
                }
            }
            else
            {
                SetPptPlusSamlToken(session);
                pptToken = session.Cast(Constants.SessionVariables.pptPlusTokenObject, default(GetPPTPlusSamlTokenResponse));
            }
            return pptToken;
        }


        public static void SetPptPlusSamlToken(IStateContainer session)
        {
            var sessionLicense = session.Cast("SessionLicense", default(ApplicationLicense));
            bool? showPPTPlus = sessionLicense?.EnterpriseClient?.ShowPPTPlus;

            if (showPPTPlus.HasValue && showPPTPlus == true)
            {
                string shieldIdentityToken = session.GetStringOrEmpty(Constants.SessionVariables.ShieldIdentityToken);
                string shieldExteralTenantId = session.GetStringOrEmpty("ShieldExternalTenantID");

                if (string.IsNullOrWhiteSpace(shieldIdentityToken) || string.IsNullOrWhiteSpace(shieldExteralTenantId))
                {
                    logger.Debug("SetPptPlusSamlToken: Could not able to set PPT Security Token");
                    return;
                }

                var pptPlusTokenObject = EPSBroker.GetPPTPlusSamlToken(shieldIdentityToken,
                                            shieldExteralTenantId);
                session[Constants.SessionVariables.pptPlusTokenObject] = pptPlusTokenObject;

                logger.Debug("SetPptPlusSamlToken: PPT Security Token is set");
            }
        }

        public static DataTable GetEPCSCheckAndAuthorizedSchedulesForPharmacy(Guid? pharmacyID, PptPlusData pptData, ConnectionStringPointer dbID)
        {
            return pptData.GetEPCSCheckAndAuthorizedSchedulesForPharmacy(pharmacyID, dbID);
        }

        public static void UpdateDetailShowStatus(IStateContainer session)
        {
            var pptContainer = session.Cast(Constants.SessionVariables.PptPlusResponses, default(PptPlusResponseContainer));

            if (pptContainer?.Candidates != null)
            {
                foreach (var candidate in pptContainer?.Candidates)
                {
                    candidate.IsAutoDetailsAlreadyDisplayed = true;
                }
            }

            if (pptContainer?.ScriptPadMeds != null)
            {
                foreach (var scriptPadMed in pptContainer?.ScriptPadMeds)
                {
                    scriptPadMed.IsAutoDetailsAlreadyDisplayed = true;
                }
            }

            session[Constants.SessionVariables.PptPlusResponses] = pptContainer;
        }

        public static bool IsAutoShowDetailScreen(IStateContainer session)
        {
            if (!IsPPTPlusPreferenceOn(session))
                return false;

            var shouldShowAlternativeAutomatically = session.GetBooleanOrFalse(Constants.SessionVariables.ShouldShowTherapeuticAlternativeAutomatically);
            if (!shouldShowAlternativeAutomatically)
                return false;

            var pptContainer = session.Cast(Constants.SessionVariables.PptPlusResponses, default(PptPlusResponseContainer));

            if (pptContainer?.Candidates != null)
            {
                foreach (var candidate in pptContainer?.Candidates)
                {
                    if (candidate.IsAutoDisplayDetails && !candidate.IsAutoDetailsAlreadyDisplayed)
                        return true;
                }
            }

            if (pptContainer?.ScriptPadMeds != null)
            {
                foreach (var scriptPadMed in pptContainer?.ScriptPadMeds)
                {
                    if (scriptPadMed.IsAutoDisplayDetails && !scriptPadMed.IsAutoDetailsAlreadyDisplayed)
                        return true;
                }
            }

            return false;
        }

        public static string GetCommonHints(IStateContainer session, PptPlus pptPlus)
        {
            var pptContainer = session.Cast(Constants.SessionVariables.PptPlusResponses, default(PptPlusResponseContainer));
            return pptPlus.GetCommonHints(pptContainer);
        }

        public static bool AlternateMedDispositionRequestCRx(
            IStateContainer session,
            string transID, 
            PptPlusResponseContainer pptPlusResponse, 
            IRtpsDisposition rtpsDisposition,
            IRtpsHelper rtpsHelper)
        {
            if (session.GetBooleanOrFalse(Constants.SessionVariables.ShouldShowRtps))
            {
                var userId = session.GetGuidOr0x0(Constants.SessionVariables.UserId);
                var licenseId = session.GetGuidOr0x0(Constants.SessionVariables.LicenseId);
                var patientId = session.GetGuidOr0x0(Constants.SessionVariables.PatientId);
                var dbId = session.Cast(Constants.SessionVariables.DbId, default(ConnectionStringPointer));

                string relatesToTxId = "";
                if (pptPlusResponse.Candidates.FirstOrDefault(_ => _.TransactionId.ToLower() == transID.ToLower()) != null)
                {
                    relatesToTxId = pptPlusResponse.Candidates.FirstOrDefault(_ => _.TransactionId.ToLower() == transID.ToLower()).RelatesToTxId;
                }
                else if (pptPlusResponse.ScriptPadMeds.FirstOrDefault(_ => _.TransactionId.ToLower() == transID.ToLower()) != null)
                {
                    relatesToTxId = pptPlusResponse.ScriptPadMeds.FirstOrDefault(_ => _.TransactionId.ToLower() == transID.ToLower()).RelatesToTxId;
                }

                rtpsHelper.CheckAndSendDisposition(
                     userId.ToString(),
                     licenseId.ToString(),
                     patientId.ToString(),
                     relatesToTxId,
                     Disposition.A,
                     rtpsDisposition,
                     dbId);
                return true;

            }
            return false;
        }
        internal static bool ShouldDisableWindow(IStateContainer targetSession)
        {
            return targetSession.GetStringOrEmpty(Constants.SessionVariables.TaskType) == Constants.MessageTypes.CHANGERX_REQUEST ||
                    targetSession.GetStringOrEmpty(Constants.SessionVariables.TaskType) == Constants.MessageTypes.REFILL_REQUEST ||
                    targetSession.GetStringOrEmpty(Constants.SessionVariables.ChangeRxRequestedMedCs) == "Allscripts.Impact.Tasks.ChangeRxRequestedMedCs";
        }
    }

    public class PptInfo
    {
        public PptPlusResponseBase MedInfo { get; set; }
        public PptPlusRequestInfo RequestInfo { get; set; }
        public IPptPlusData PptPlusData { get; set; }
        public ICommonComponentData CommonComponentData { get; set; }
        public IPptPlus PptPlus { get; set; }
        public IPptPlusServiceBroker PptPlusServiceBroker { get; set; }
        public IMedication Medication { get; set; }
        public ConnectionStringPointer DbId { get; set; }
        public string PptBase64SamlToken { get; set; }
         public string UserId { get; set; }
        public string LicenseId { get; set; }
        public bool IsEpaOn { get; set; }
        public bool IsRtps { get; set; }
        public IRtpsDisposition RtpsDisposition { get; set; }
        public IRtpsHelper RtpsHelper { get; set; }
    }
}