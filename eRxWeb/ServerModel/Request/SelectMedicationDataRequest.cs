using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.DatabaseSelector;
using eRxWeb.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eRxWeb.ServerModel.Request
{
    public enum SelectMedicationRequestType
    {
        PatientHistory = 1,
        ProviderHistory = 5,
        AllMedication = 10,
        PreBuiltGroup = 15
    }

    public enum PatientRecordStatus
    {
        Active = 1,
        InActive = 5,
        Both = 10
    }
    public class SelectMedicationDataRequest
    {
        public SelectMedicationDataRequest()
        {
            RequestFor = SelectMedicationRequestType.PatientHistory;
        }
        public string SearchText { get; set; }

        public PatientRecordStatus PatientRecordStatus { get; set; }
        public string GroupName { get; set; }
        public bool FullRefresh { get; set; }
        public SelectMedicationRequestType RequestFor { get; set; }
        public GridPaginationModel Page { get; set; }
   
        public IList<MedCompletionHistory> medCompleteHistories { get; set; }

        #region Session Data
        public IStateContainer sessionState { get; set; }
        public int Siteid
        {
            get
            {
                if (sessionState != null)
                    return sessionState.GetInt(Constants.SessionVariables.SiteId, -1);
                else
                    return 0;
            }
        }
        public string LicenseId
        {
            get
            {
                if (sessionState != null)
                    return sessionState.GetStringOrEmpty(Constants.SessionVariables.LicenseId);
                else
                    return string.Empty;
            }
        }
        public string ProviderId
        {
            get
            {
                if (sessionState != null)
                {
                    if (eRxWeb.AppCode.StateUtils.UserInfo.IsPOBUser(sessionState))
                    {
                        return sessionState.GetStringOrEmpty("DelegateProviderID");
                    }
                    else
                    {
                        return sessionState.GetStringOrEmpty(Constants.SessionVariables.UserId);
                    }
                }
                   
                else
                    return string.Empty;
            }
        }
        public string ICD10Code
        {
            get
            {
                if (sessionState != null) return sessionState.GetStringOrEmpty("ICD10CODE");
                else
                    return string.Empty;
            }
        }
        public string ICD9Code
        {
            get
            {
                if (sessionState != null) return sessionState.GetStringOrEmpty("ICD9CODE");
                else
                    return string.Empty;
            }
        }
        public string FormularyID
        {
            get
            {
                if (sessionState != null)
                    return sessionState.GetStringOrEmpty("FormularyID");
                else
                    return string.Empty;
            }
        }
        public string FormularyActive
        {
            get
            {
                if (sessionState != null) return sessionState.GetStringOrEmpty("FormularyActive");
                else
                    return string.Empty;
            }
        }
        public int OtcCoverage
        {
            get
            {
                if (sessionState != null) return sessionState.GetInt("OTCCoverage", -1);
                else
                    return 0;
            }
        }
        public int GenericDrugPolicy
        {
            get
            {
                if (sessionState != null) return sessionState.GetInt("GenericDrugPolicy", 0);
                else
                    return 0;
            }
        }
        public int UnlistedDrugPolicy
        {
            get
            {
                if (sessionState != null) return sessionState.GetInt("UnListedDrugPolicy", 0);
                else
                    return 0;
            }
        }
        public string PatientID
        {
            get
            {
                if (sessionState != null) return sessionState.GetStringOrEmpty(Constants.SessionVariables.PatientId);
                else
                    return string.Empty;
            }
        }
        public string SelectedCoverageID
        {
            get
            {
                if (sessionState != null) return sessionState.GetStringOrEmpty("SelectedCoverageID");
                else
                    return string.Empty;
            }
        }
        public string GenericEquivalent
        {
            get
            {
                if (sessionState != null) return sessionState.GetStringOrEmpty("GenericEquivalentSearch");
                else
                    return string.Empty;
            }
        }
        public ConnectionStringPointer DbID
        {
            get
            {
                if (sessionState != null) return sessionState.Cast(Constants.SessionVariables.DbId, ConnectionStringPointer.ERXDB_DEFAULT);
                else
                    return ConnectionStringPointer.ERXDB_DEFAULT;
            }
        }
        #endregion
    }
}