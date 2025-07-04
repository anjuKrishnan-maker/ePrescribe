using Allscripts.ePrescribe.DatabaseSelector;
using Allscripts.Impact;
using ScriptMessage = Allscripts.Impact.ScriptMsg.ScriptMessage;
using Constants = Allscripts.ePrescribe.Common.Constants;
using System.Data;
using Allscripts.Impact.Interfaces;
using static Allscripts.Impact.IgnoreReason;
using eRxWeb.AppCode.DurBPL.RequestModel;
using Allscripts.ePrescribe.ExtensionMethods;
using System;
using System.Text;
using Allscripts.ePrescribe.Medispan.Clinical.Model.Settings;
using System.Collections;
using Allscripts.Impact.ScriptMsg;
using Allscripts.ePrescribe.Medispan.Clinical.Model.Request;
using System.Collections.Generic;
using Allscripts.ePrescribe.Medispan.Clinical.Model.Response;
using static Allscripts.ePrescribe.Common.Constants;
using Allscripts.ePrescribe.Common;
using eRxWeb.AppCode.StateUtils;

namespace eRxWeb.AppCode.DurBPL
{
    public class ImpactDurWraper : IImpactDurWraper
    {

        public string RejectScriptMessage(string smId, string reasonCode,
            string message, string userID, string licenseID, string prescriberOrderNumber, string shieldSecurityToken, int siteID, ConnectionStringPointer dbID)
        {
            string rejectMessage = ScriptMessage.RejectMessage(
               smId,
                reasonCode,
                message,
                userID,
                licenseID,
                prescriberOrderNumber,
                shieldSecurityToken,
                siteID,
                dbID);
            return rejectMessage;
        }
        public void SaveDurWarnings(DataTable durWarning, ConnectionStringPointer dbID)
        {
            DUR.DurWarningSetInsert(durWarning, dbID);
        }

        public DataSet LoadPharmacy(string pharmacyId, ConnectionStringPointer dbId)
        {
            return Allscripts.Impact.Pharmacy.LoadPharmacy(pharmacyId, dbId);
        }

        public LoadPharamacyResponse LoadPharmacy(string taskScriptMessageId, string sessionLicenseID, string sessionUserID, ConnectionStringPointer dbId)
        {
            LoadPharamacyResponse response = new LoadPharamacyResponse();
            response.scriptMessage = CreateNewScriptMessage(taskScriptMessageId, sessionLicenseID, sessionUserID, dbId);
            if (response.scriptMessage != null)
            {
                response.pharmDS = Allscripts.Impact.Pharmacy.LoadPharmacy(response.scriptMessage.DBPharmacyID, dbId);
            }
            return response;
        }
        public string LoadPharamcyDetail(string taskScriptMessageId, string sessionLicenseID, string sessionUserID, ref DataSet pharmDS, ref ScriptMessage sm, string pharmacyKey, ConnectionStringPointer dbId)
        {
            LoadPharamacyResponse loadPharamacyResponse = LoadPharmacy(taskScriptMessageId, sessionLicenseID, sessionUserID, dbId);
            if (loadPharamacyResponse != null)
            {
                pharmDS = loadPharamacyResponse.pharmDS;
                sm = loadPharamacyResponse.scriptMessage;
            }

            return pharmDS.Tables.Count > 0 ? Convert.ToString(pharmDS.Tables[0].Rows[0][pharmacyKey]) : string.Empty;
        }
        public LoadPharamacyResponse LoadPharmacy(ConnectionStringPointer dbId, ScriptMessage sm)
        {
            LoadPharamacyResponse response = new LoadPharamacyResponse();
            if (sm != null)
            {
                response.pharmDS = Allscripts.Impact.Pharmacy.LoadPharmacy(sm.DBPharmacyID, dbId);
            }
            return response;
        }
        public string LoadPharamcyDetail(ConnectionStringPointer dbId, ref DataSet pharmDS, ref ScriptMessage sm, string pharamacyKey)
        {
            LoadPharamacyResponse loadPharamacyResponse = LoadPharmacy(dbId, sm);
            if (sm != null) loadPharamacyResponse.scriptMessage = sm;
            if (loadPharamacyResponse != null)
            {
                pharmDS = loadPharamacyResponse.pharmDS;
            }

            return pharmDS.Tables.Count > 0 ? Convert.ToString(pharmDS.Tables[0].Rows[0][pharamacyKey]) : string.Empty;
        }
        public string GetStateControlledSubstanceCode(string ddi, string practiceState, ConnectionStringPointer dbID)
        {
            return Prescription.GetStateControlledSubstanceCode(ddi, practiceState, dbID);
        }
        public string ReconcileControlledSubstanceCodes(string fedCSCode, string stateCSCodeForPharmacy, string stateCSCodeForPractice)
        {
            return Prescription.ReconcileControlledSubstanceCodes(fedCSCode, stateCSCodeForPharmacy, stateCSCodeForPractice);
        }
        public bool IsCSMedEPCSEligible(string fedCSCode, string stateCSCodeForPharmacy, string stateCSCodeForPractice)
        {
            return Prescription.IsCSMedEPCSEligible(fedCSCode, stateCSCodeForPharmacy, stateCSCodeForPractice);
        }
        public string GetStateControlledSubstanceCode(string ddi, string practiceState, string pharmacyState, ConnectionStringPointer dbID)
        {
            return Prescription.GetStateControlledSubstanceCode(ddi, practiceState, pharmacyState, dbID);
        }

        public string ReconcileControlledSubstanceCodes(string medispanCSCode, string stateCSCode)
        {
            return Prescription.ReconcileControlledSubstanceCodes(medispanCSCode, stateCSCode);
        }

        public DataTable GetEPCSAuthorizedSchedulesForPharmacy(string PharmacyId, ConnectionStringPointer dbID)
        {
            return Allscripts.Impact.Pharmacy.GetEPCSAuthorizedSchedulesForPharmacy(PharmacyId, dbID);
        }

        public string ApproveMessage(string taskScriptMessageId, string sessionLicenseID, string sessionUserID, Rx currentRx, decimal metricQuantity, string providerId, string prescriptionTransmissionMethod, string delegateProviderID, int siteId, string shieldSecurityToken, ConnectionStringPointer dbId)
        {
            return ScriptMessage.ApproveMessage(CreateNewScriptMessage(taskScriptMessageId, sessionLicenseID, sessionUserID, dbId), currentRx.DDI, currentRx.MedicationName,
                                                   currentRx.DaysSupply, metricQuantity, currentRx.Refills, currentRx.SigText, currentRx.DAW, providerId, currentRx.Notes,
                                                   Constants.PrescriptionTransmissionMethod.SENT, sessionLicenseID, sessionUserID, siteId, shieldSecurityToken,
                                                   currentRx.IsCompoundMed, currentRx.HasSupplyItem, delegateProviderID, currentRx.MDD, currentRx.SigID, dbId);
        }

        public ScriptMessage CreateNewScriptMessage(string taskScriptMessageId, string sessionLicenseID, string sessionUserID, ConnectionStringPointer dbId)
        {
            return new ScriptMessage(taskScriptMessageId, sessionLicenseID, sessionUserID, dbId);
        }
        public void SetHeaderInfo(string sessionUserID, string rxID, ScriptMessage sm, Prescription rx, string providerID, int siteId, ConnectionStringPointer dbId)
        {
            rx.SetHeaderInformation(sessionUserID, rxID, DateTime.UtcNow.ToString(),
                sm.DBPatientID, providerID, Guid.Empty.ToString(),
                string.Empty, string.Empty, string.Empty, Constants.PrescriptionType.NEW, false, string.Empty,
                siteId, Constants.ERX_NOW_RX, null, dbId);
        }
        public void AddMedication(string sessionUserID, Rx currentRx, string performFormulary, Allscripts.ePrescribe.Objects.DURSettings durSettings, StringBuilder pharmComments, string deaNumber, ScriptMessage sm, Prescription rx, string stateCSCodeForPharmacy, string providerID, PrescriptionWorkFlow rxWorkFlow, string externalFacilityCd, string externalGroupID)
        {
            string formattedFreeTextSig = Allscripts.Impact.Sig.ToSigFormat(currentRx.SigText);
            Allscripts.ePrescribe.Objects.FreeFormSigInfo sig = Allscripts.Impact.Sig.SaveFreeFormSigTextOrGetInfo(formattedFreeTextSig, Allscripts.ePrescribe.DatabaseSelector.ConnectionStringPointer.SHARED_DB);
            string SigID = sig.SigID;
            int sigTypeId = sig.SigTypeID;

            rx.AddMedication(
                           sessionUserID, //EAK added
                            0, // RxNumber
                            sm.DBDDID,
                            currentRx.MedicationName, //sm.RxDrugDescription,
                            currentRx.RouteOfAdminCode, // routeOfAdminCode
                            currentRx.DosageFormCode, // sm.RxDosageFormCode, // dosageFormCode
                            currentRx.Strength,
                            currentRx.StrengthUOM,
                            SigID,
                            formattedFreeTextSig, // sigText
                            currentRx.Refills, //refills
                            currentRx.Quantity,
                            currentRx.DaysSupply,
                            string.Empty, //gppc
                            Convert.ToDecimal(1), //packageSize
                            "EA", //packageUOM
                            1, //packageQuantity
                            "EA", //packageDescription
                            currentRx.DAW, //daw
                            DateTime.Today.ToString(),
                            Constants.PrescriptionStatus.NEW, //status
                            Constants.PrescriptionTransmissionMethod.SENT, //transmissionMethod
                            string.Empty, //originalDDI
                            0, //originalFormStatusCode
                            "N", //originalIsListed
                            0, //formStatusCode
                            "N", //isListed
                            Allscripts.Impact.FormularyStatus.NONE, //formularyStatus
                            performFormulary, //_performFormulary
                            durSettings.CheckPerformDose.ToChar(),
                            durSettings.CheckDrugToDrugInteraction.ToChar(),
                            durSettings.CheckDuplicateTherapy.ToChar(),
                            durSettings.CheckPar.ToChar(),
                            sm.PracticeState, //rxFormat
                            pharmComments.ToString(), //notes
                            Guid.Empty.ToString(), //originalRxID
                            0, //originalLineNumber
                            "R", //creationType
                            Guid.Empty.ToString(),
                            null, //ICD9 EAK
                            currentRx.ControlledSubstanceCode,  // medispan (or federal) controlled substance code
                            providerID,
                            null, // lastFillDate
                            null, // authorizeByID
                            rxWorkFlow,
                            externalFacilityCd,
                            externalGroupID,
                            currentRx.CoverageID, //coverageID
                            -1, // alternativeIgnoreReason
                            stateCSCodeForPharmacy,
                            deaNumber,
                            sigTypeId);
        }
        public void SaveRxInfo(int siteId, string sessionLicenseID, string sessionUserID, ConnectionStringPointer dbId, string rxID, Prescription rx, string deaNumber, bool? isCSRegistryChecked, Rx currentRx)
        {
            rx.Save(siteId, sessionLicenseID, sessionUserID, true, isCSRegistryChecked, dbId);

            if (currentRx.IsFreeFormMedControlSubstance)
            {
                Prescription.SaveCSDetails(rxID, currentRx.ScheduleUsed, Constants.ControlledSubstanceTypes.FREEFORM, deaNumber, isCSRegistryChecked, dbId);
            }
        }
        public string CreateDUREVTScriptMessage(string rxID, int lineNumber, string sessionLicenseID, string sessionUserID, string securityShieldToken, ConnectionStringPointer dbId)
        {
            return ScriptMessage.CreateDUREVTScriptMessage(rxID, lineNumber, sessionLicenseID, sessionUserID, securityShieldToken, dbId);
        }
        public string ApproveRXCHGMessage(RxTaskModel rxTask, string providerId, string prescriptionTransmissionMethod)
        {
            return ScriptMessage.ApproveRXCHGMessage(rxTask, providerId, prescriptionTransmissionMethod);
        }

        public void ApprovePrescription(string rxID, int lineNumber, string sessionLicenseID, string sessionUserID, ConnectionStringPointer dbId)
        {
            Prescription.ApprovePrescription(rxID, lineNumber, sessionLicenseID, sessionUserID, dbId);
        }
        public void UpdateRxTask(long taskID, Constants.PrescriptionTaskType prescriptionTaskType, Constants.PrescriptionTaskStatus prescriptionTaskStatus, Constants.PrescriptionStatus prescriptionStatus, string relatedUserId, string comments, string sessionUserId, ConnectionStringPointer dbId)
        {
            Prescription.UpdateRxTask(taskID, prescriptionTaskType, prescriptionTaskStatus, prescriptionStatus, relatedUserId, comments, sessionUserId, dbId);
        }
        public long SendThisEPCSMessage(string kvpValue, string SessionLicenseID, string SessionUserID, ConnectionStringPointer dbId)
        {
            return ScriptMessage.SendThisEPCSMessage(kvpValue, SessionLicenseID, SessionUserID, dbId);
        }
        public ePrescribeSvc.AuditLogPatientRxResponse AuditLogPatientRxInsert(ePrescribeSvc.AuditAction auditAction, string sessionLicenseID, string sessionUserID, string patientID, string rxID, string UserHostAddress, ConnectionStringPointer dbId, string createdUTC = null)
        {
            return EPSBroker.AuditLogPatientRxInsert(
                auditAction,
                sessionLicenseID,
                sessionUserID,
                patientID,
                UserHostAddress,
                rxID,
                dbId,
                createdUTC);
        }
        public DURCheckRequest ConstructDurCheckRequest(string patientDob, string patientGender, List<Rx> currentRxs, List<string> activeRxList, List<Allscripts.ePrescribe.Medispan.Clinical.Model.Allergy> allergies, Allscripts.ePrescribe.Objects.DURSettings durSettings)
        {
            return DURMedispanUtils.ConstructDurCheckRequest(patientDob, patientGender, currentRxs, activeRxList, allergies, durSettings);
        }
        public DURCheckResponse PerformDurCheck(DURCheckRequest request)
        {
            DURCheckResponse durCheckResponse = MedispanServiceBroker.PerformDurCheck(request);
            return durCheckResponse;
        }
        public string SendOutboundInfoScriptMessage(string rxID, int lineNumber, string sessionLicenseID, string sessionUserID, string shieldSecurityToken, string standing, ConnectionStringPointer dbId)
        {
            string smid = ScriptMessage.CreateDUREVTScriptMessage(rxID, lineNumber, sessionLicenseID, sessionUserID, shieldSecurityToken, dbId);

            if ((standing == "1") && (!string.IsNullOrEmpty(smid)))
            {
                ScriptMessage.SendOutboundInfoScriptMessage(smid, sessionLicenseID, sessionUserID, dbId);
            }
            return smid;
        }
        public DataTable GetIgnoreReasons(IgnoreCategory category, Constants.UserCategory UserType, ConnectionStringPointer dbID)
        {
            return IgnoreReason.GetIgnoreReasons(category, UserType, dbID);
        }
        public void UpdateRxDetailStatus(string licienceId, string userId, string rxID, string rxStatus, ConnectionStringPointer dbId)
        {
            Prescription.UpdateRxDetailStatus(licienceId, userId, rxID, rxStatus, dbId);
        }
        public DataSet ChGetRXDetails(string rxID, ConnectionStringPointer dbId)
        {
            return Prescription.ChGetRXDetails(rxID, dbId);
        }
        public long SendRenewalScriptMessageForEPCS(string scriptMessageID, string rxID, string sessionLicenseID, string sessionUserID, string taskScriptMessageId, string notes, ConnectionStringPointer dbId)
        {
            return ScriptMessage.SendRenewalScriptMessageForEPCS(scriptMessageID, rxID, sessionLicenseID, sessionUserID, taskScriptMessageId, notes, dbId);
        }
        public long SendCHGRESScriptMessageForEPCS(string scriptMessageID, string rxID, string sessionLicenseID, string sessionUserID, string rxTaskScriptMessageGUID, string notes, ConnectionStringPointer dbId)
        {
            return ScriptMessage.SendCHGRESScriptMessageForEPCS(scriptMessageID, rxID, sessionLicenseID, sessionUserID, rxTaskScriptMessageGUID, notes, dbId);
        }
        public SystemConfig SystemConfig()
        {
            return new SystemConfig();
        }
        public string ApproveMessage(ScriptMessage sm, string DDID, string freeFormMedName, int daysSupply,
            decimal quantity, int refills, string sig, bool daw, string providerID, string comments,
            string transmitmethod, string licenseID, string userID, int siteID, string shieldSecurityToken,
            bool isCompound, bool isSupply, string delegateProviderID, string mddValue, string sigID,
            ConnectionStringPointer dbID)
        {
            return ScriptMessage.ApproveMessage(sm, DDID, freeFormMedName, daysSupply, quantity, refills,
                sig, daw, providerID, comments, transmitmethod, licenseID, userID, siteID, shieldSecurityToken, isCompound, isSupply, delegateProviderID, mddValue, sigID, dbID);
        }
        public string GetShieldSecurityToken(State.IStateContainer state)
        {
            return ShieldInfo.GetShieldSecurityToken(state);
        }
        public void GetRefillRequest(SubmitDurRequest submitDurRequest, ScriptMessage sm)
        {
            if (sm != null) submitDurRequest.ProviderId = sm.ProviderID;
        }
        public void GetEPCSAuthorizedSchedulesForPharmacy(string reconciledControlledSubstanceCode, DataSet dsPharmacy, string key, SubmitDurRequest durRequest, ConnectionStringPointer dbId)
        {
            if (Convert.ToString(dsPharmacy.Tables[0].Rows[0][key]) == "1")
            {
                //
                // get EPCS authorized schedules for pharmacy
                //
                List<string> authorizedSchedules = new List<string>();

                DataTable dtSchedules = GetEPCSAuthorizedSchedulesForPharmacy(dsPharmacy.Tables[0].Rows[0][Constants.PharmacyID].ToString(), dbId);

                foreach (DataRow drSchedule in dtSchedules.Rows)
                {
                    authorizedSchedules.Add(drSchedule[0].ToString());
                }

                //
                // check if the state in which the pharmacy is in, is EPCS authorized for the 
                // CS schedule of the selected med
                //
                if (authorizedSchedules.Contains(reconciledControlledSubstanceCode))
                {
                    durRequest.starsAlign = true;
                }
            }


        }
    }
    public class LoadPharamacyResponse
    {
        public LoadPharamacyResponse()
        {
            this.pharmDS = new DataSet();
        }

        public DataSet pharmDS { get; set; }
        public ScriptMessage scriptMessage { get; set; }


    }







}