using Allscripts.ePrescribe.DatabaseSelector;
using System.Data;
using Allscripts.ePrescribe.Common;
using Allscripts.Impact;
using System.Text;
using Allscripts.Impact.ScriptMsg;
using Allscripts.ePrescribe.Medispan.Clinical.Model.Request;
using System.Collections.Generic;
using ePrescribe = Allscripts.ePrescribe;
using Allscripts.ePrescribe.Medispan.Clinical.Model.Response;
using IgnoreReason=Allscripts.Impact.IgnoreReason;
using static Allscripts.ePrescribe.Common.Constants;

namespace eRxWeb.AppCode.DurBPL
{
    public interface IImpactDurWraper
    {

        string RejectScriptMessage(string smId, string reasonCode, string message, string userID, string licenseID, string prescriberOrderNumber, string shieldSecurityToken, int siteID, ConnectionStringPointer dbID);    
        
        void SaveDurWarnings(DataTable durRequest, ConnectionStringPointer dbID);
        DataSet LoadPharmacy(string pharmacyId, ConnectionStringPointer dbId);
        string GetStateControlledSubstanceCode(string ddi, string practiceState, ConnectionStringPointer dbID);
        string ReconcileControlledSubstanceCodes(string fedCSCode, string stateCSCodeForPharmacy, string stateCSCodeForPractice);
        bool IsCSMedEPCSEligible(string fedCSCode, string stateCSCodeForPharmacy, string stateCSCodeForPractice);
        DataTable GetEPCSAuthorizedSchedulesForPharmacy(string PharmacyId, ConnectionStringPointer dbID);
        Allscripts.Impact.ScriptMsg.ScriptMessage CreateNewScriptMessage(string taskScriptMessageId, string sessionLicenseID, string sessionUserID, ConnectionStringPointer dbId);
        LoadPharamacyResponse LoadPharmacy(string taskScriptMessageId, string sessionLicenseID, string sessionUserID, ConnectionStringPointer dbId);
        void SetHeaderInfo(string sessionUserID, string rxID, Allscripts.Impact.ScriptMsg.ScriptMessage sm, Allscripts.Impact.Prescription rx, string providerID, int siteId, ConnectionStringPointer dbId);
        void AddMedication(string sessionUserID, Rx currentRx, string performFormulary, Allscripts.ePrescribe.Objects.DURSettings durSettings, StringBuilder pharmComments, string deaNumber, ScriptMessage sm, Prescription rx, string stateCSCodeForPharmacy, string providerID, PrescriptionWorkFlow rxWorkFlow, string externalFacilityCd, string externalGroupID);
        void SaveRxInfo(int siteId, string sessionLicenseID, string sessionUserID, ConnectionStringPointer dbId, string rxID, Prescription rx, string deaNumber, bool? isCSRegistryChecked, Rx currentRx);
        string CreateDUREVTScriptMessage(string rxID, int lineNumber, string sessionLicenseID, string sessionUserID, string securityShieldToken, ConnectionStringPointer dbId);
        string ApproveRXCHGMessage(RxTaskModel rxTask, string providerId, string prescriptionTransmissionMethod);
        string ApproveMessage(string taskScriptMessageId, string sessionLicenseID, string sessionUserID, Rx currentRx, decimal metricQuantity, string providerId, string prescriptionTransmissionMethod, string delegateProviderID, int siteId, string shieldSecurityToken, ConnectionStringPointer dbId);
        void ApprovePrescription(string rxID, int lineNumber, string sessionLicenseID, string sessionUserID, ConnectionStringPointer dbId);
        void UpdateRxTask(long taskID, Constants.PrescriptionTaskType prescriptionTaskType, Constants.PrescriptionTaskStatus prescriptionTaskStatus, Constants.PrescriptionStatus prescriptionStatus, string relatedUserId, string comments, string sessionUserId, ConnectionStringPointer dbId);
        ePrescribeSvc.AuditLogPatientRxResponse AuditLogPatientRxInsert(ePrescribeSvc.AuditAction auditAction, string sessionLicenseID, string sessionUserID, string patientID, string rxID, string UserHostAddress, ConnectionStringPointer dbId, string createdUTC = null);
        DURCheckRequest ConstructDurCheckRequest(string patientDob, string patientGender, List<Rx> currentRxs, List<string> activeRxList, List<ePrescribe.Medispan.Clinical.Model.Allergy> allergies, ePrescribe.Objects.DURSettings durSettings);
        DURCheckResponse PerformDurCheck(DURCheckRequest request);
        DataTable GetIgnoreReasons(IgnoreReason.IgnoreCategory category, UserCategory usertype, ConnectionStringPointer DBID);
        long SendThisEPCSMessage(string kvpValue, string SessionLicenseID, string SessionUserID, ConnectionStringPointer dbId);
        void UpdateRxDetailStatus(string licienceId, string userId, string rxID, string rxStatus, ConnectionStringPointer dbId);
        DataSet ChGetRXDetails(string rxID, ConnectionStringPointer dbId);
        long SendRenewalScriptMessageForEPCS(string scriptMessageID, string rxID, string sessionLicenseID, string sessionUserID, string taskScriptMessageId, string notes, ConnectionStringPointer dbId);
        long SendCHGRESScriptMessageForEPCS(string scriptMessageID, string rxID, string sessionLicenseID, string sessionUserID, string rxTaskScriptMessageGUID, string notes, ConnectionStringPointer dbId);
        SystemConfig SystemConfig();
        string SendOutboundInfoScriptMessage(string rxID, int lineNumber, string sessionLicenseID, string sessionUserID, string shieldSecurityToken, string standing, ConnectionStringPointer dbId);
        string GetStateControlledSubstanceCode(string ddi, string practiceState, string pharmacyState, ConnectionStringPointer dbID);
        string ReconcileControlledSubstanceCodes(string medispanCSCode, string stateCSCode);
        string GetShieldSecurityToken(State.IStateContainer state);
        string ApproveMessage(ScriptMessage sm, string DDID, string freeFormMedName, int daysSupply, decimal quantity, int refills, string sig, bool daw, string providerID, string comments, string transmitmethod, string licenseID, string userID, int siteID, string shieldSecurityToken, bool isCompound, bool isSupply, string delegateProviderID, string mddValue, string sigID, ConnectionStringPointer dbID);
        void GetRefillRequest(RequestModel.SubmitDurRequest submitDurRequest, ScriptMessage sm);
        string LoadPharamcyDetail(ConnectionStringPointer dbId, ref DataSet pharmDS, ref ScriptMessage sm, string pharamacyKey);
        string LoadPharamcyDetail(string taskScriptMessageId, string sessionLicenseID, string sessionUserID, ref DataSet pharmDS, ref ScriptMessage sm, string pharmacyKey, ConnectionStringPointer dbId);
        void GetEPCSAuthorizedSchedulesForPharmacy(string reconciledControlledSubstanceCode, DataSet dsPharmacy, string key, RequestModel.SubmitDurRequest durRequest, ConnectionStringPointer dbId);
    }
}