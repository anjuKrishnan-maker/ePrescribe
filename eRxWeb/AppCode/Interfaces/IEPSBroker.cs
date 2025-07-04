using eRxWeb.ePrescribeSvc;
using Allscripts.ePrescribe.Common;
using ConnectionStringPointer = Allscripts.ePrescribe.DatabaseSelector.ConnectionStringPointer;
using eRxWeb.State;
using System;

namespace eRxWeb.AppCode.Interfaces
{
    public interface IEPSBroker
    {
        int GetShieldInternalTenantID(string accountID);
        GetNewActivationCodeResponse GetNewActivationCode(int shieldInternalTenantID, string userName);
        void AuditLogPatientInsert(AuditAction patientWeightModified, string getStringOrEmpty, string s, string sessionPatientId, string userHostAddress, ConnectionStringPointer dbid);
        AuditLogPatientRxResponse AuditLogPatientRxInsert(ePrescribeSvc.AuditAction auditAction, string licenseID, string userID, string patientID, string ipAddress, string rxId, ConnectionStringPointer dbID, string createdUTC = null);
        void UpdatePrescriptionStatus(string LicenseId, string rxID, int lineNumber, Constants.PrescriptionStatus prescriptionStatus, string userID, ConnectionStringPointer dbID);

        /// <summary>
        /// Calls EPS which calls Shield to get a fresh SAML token for the logged in user.
        /// </summary>
        /// <param name="currentSamlToken">String representation of the current SAML token</param>
        /// <param name="ipAddress">IP address of the logged in user</param>
        /// <param name="appInstanceID">External AppInstanceID that's stored on Shield.</param>
        /// <param name="dbID">DBID of the logged in user</param>
        /// <returns>String representation of the new SAML token</returns>
        RefreshSamlTokenResponse RefreshSamlToken(string currentSamlToken, string ipAddress, string appInstanceID, ConnectionStringPointer dbID);

        GetPdmpEnrollmentFormInfoResponse GetPdmpEnrollmentFormInfo(string userGuid, string licenseId, ConnectionStringPointer dbID);
        GetUserShieldCspStatusInfoResponse GetUserShieldCspStatusInfo(ConnectionStringPointer dbid,string shieldSecurityToken,string shieldExteralTenantId,string userId,string licenseId);

        AuthenticateAndAuthorizeUserResponse AuthenticateAndAuthorizeUser(string userName, string password, string ipAddress);

        AuthenticateAndAuthorizeUserResponse AuthorizeUser(string userName, string shieldIdentityToken, string ipAddress);

        bool IsUserLoggingEnabled(Guid userGuid);

        void AddILearnUser(string userName, string clientId, string firstName, string lastName, ConnectionStringPointer DbID);
        string GetIdProofingUrl(ePrescribeSvc.ShieldTenantIDProofingModel idProofingMode, string userName);
    }
}