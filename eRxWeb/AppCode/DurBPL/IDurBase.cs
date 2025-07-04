using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.DatabaseSelector;
using Allscripts.ePrescribe.Medispan.Clinical.Model.Response;
using Allscripts.ePrescribe.Objects;
using Allscripts.Impact;
using Allscripts.Impact.Tasks;
using eRxWeb.ServerModel;
using eRxWeb.State;
using System;
using System.Collections.Generic;


namespace eRxWeb.AppCode.DurBPL
{
   public interface IDurBase
    {
        List<string> MedsWithDURs { get; set; }
        Allscripts.Impact.RxUser DelegateProvider { get;}
        void ClearMedicationInfo(bool clearDx);
        ePrescribeSvc.AuditLogPatientRxResponse AuditLogPatientRxInsert(ePrescribeSvc.AuditAction auditAction, string patientID, string rxID, string UserHostAddress, string createdUTC = null);
        int SessionSiteID { get;}
        DURCheckResponse DURWarnings { get; set; }
        ChangeRxRequestedMedCs ChangeRxRequestedMedCs { get; set; }      
        List<string> SiteEPCSAuthorizedSchedules { get;}       
        ApplicationLicense SessionLicense { get; }
        bool IsEnterpriseClientEPCSEnabled { get; }
        bool IsLicenseEPCSPurchased { get; }
        bool IsLicenseShieldEnabled { get; }
        bool IsUserEPCSEnabled { get;}
        bool DoesUserHavePermission(string permissionUri);   
        bool CanTryEPCS { get; }
        RxTaskModel RxTask { get; set; }
        IStateContainer PageState { get; set; }      
        string ShieldSecurityToken { get; }       
        List<Allscripts.Impact.Rx> ScriptPadMeds { get; }
        
         Constants.UserCategory SessionUserType { get;}
        bool IsUserAPrescribingUserWithCredentials { get;}

        /// <summary>
        /// Returns the first rx in the RxList Session variable
        /// </summary>
        Allscripts.Impact.Rx CurrentRx { get; }
        
        bool CanApplyFinancialOffers { get; }
       
        DURSettings DURSettings { get; }
        bool IsPOBUser { get; }
        string SessionPatientID { get; }
         List<KeyValuePair<Guid, int>> DDIToRxIDMap { get; set; }
        List<Allscripts.ePrescribe.Medispan.Clinical.Model.Allergy> DurPatientAllergies { get; }
        string SessionLicenseID { get; }
        string SessionUserID { get; }
        ConnectionStringPointer DBID { get; }
        string AppSettings(string key);
        string RedirectToSelectPatient(string queryString, SelectPatientComponentParameters selectPatientComponentParameters);
        
    }
}
