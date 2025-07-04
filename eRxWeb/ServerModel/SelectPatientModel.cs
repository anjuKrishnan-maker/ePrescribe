using System;
using System.Data;
using System.Collections.Generic;
using Allscripts.ePrescribe.Objects;
using eRxWeb.AppCode;
using HOME_ADDRESS_CHECK_STATUS = Allscripts.Impact.ePrescribeSvc.HOME_ADDRESS_CHECK_STATUS;

namespace eRxWeb.ServerModel
{
    public class SearchPatientResponse
    {
        public SearchPatientResponse()
        {
            this.Patients = new List<PatientItemModel>();
            this.Messages = new List<MessageModel>();
        }

        public List<PatientItemModel> Patients { get; set; }
        public List<MessageModel> Messages { get; internal set; }
    }

    public class PatientItemModel
    {
        public PatientItemModel()
        {
        }

        public PatientItemModel(DataRow drPatient, bool isSimilarPatient = false)
        {
            PatientID = Convert.ToString(drPatient["PatientID"]);
            MRN = isSimilarPatient ? string.Empty : Convert.ToString(drPatient["MRN"]);
            Name = Convert.ToString(drPatient["Name"]);
            DOB = Convert.ToString(drPatient["DOB"]);
            Address = Convert.ToString(drPatient["Address"]);
            Phone = Convert.ToString(drPatient["Phone"]);
            StatusID = isSimilarPatient ? "1" : Convert.ToString(drPatient["StatusID"]);
            IsVIPPatient = isSimilarPatient ? false : Convert.ToBoolean(Convert.ToString(drPatient["IsVIPPatient"]));
            IsRestrictedPatient = isSimilarPatient
                ? false
                : Convert.ToBoolean(Convert.ToString(drPatient["IsRestrictedPatient"]));
        }
        public string PatientID { get; set; }
        public string MRN { get; set; }
        public string Name { get; set; }
        public string DOB { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string StatusID { get; set; }
        public bool IsVIPPatient { get; set; }
        public bool IsRestrictedPatient { get; set; }
    }

    public class SelectPatientStartupParameters
    {
        public bool IsAddDiagnosisVisible { get; set; }
        public bool IsSsoLockdownMode { get; set; }
        public bool IsPrivacyOverrideDisplayed { get; set; }
        public bool IsWelcomeTourDisplayed { get; set; }
        public bool IsDeluxeTeaserAdDisplayed { get; set; }
        public bool IsCreditCardExpiring { get; set; }
        public bool IsShowAddPatient { get; set; }
        public string PatientID { get; set; }
        public string DelegateProviderId { get; set; }
        public string UserID { get; set; }
        public SelectPatientUserType UserType { get; set; }
        public SelectPatientUserCategory UserCategory { get; set; }
        public List<SelectPatientProvider> Providers { get; set; }
        public WelcomeTourModel WelcomeTourModel { get; set; }
        public DeluxeTeaserAdModel DeluxeTeaserAdModel { get; set; }
        public SearchPatientResponse SearchPatientResponse { get; set; }
        public MessageModel Message { get; set; }
    }

    public enum HomeAddressCheckStatus
    {

        /// <remarks/>
        NOT_VERIFIED,

        /// <remarks/>
        VERIFIED,

        /// <remarks/>
        LOCKED,

        /// <remarks/>
        MANUAL,

        /// <remarks/>
        BULK_WAITING,
    }
    public class SelectPatientProvider
    {
        public string ProviderName { get; set; }
        public string ProviderId { get; set; }
        public SelectPatientUserCategory UserCategory { get; set; }
    }

    public enum SelectPatientUserType
    {
        Provider = 1,
        PAwithSupervision = 2,
        Staff = 3
    }

    public enum SelectPatientUserCategory
    {
        GENERAL_USER = 0,
        PROVIDER = 1,
        POB_LIMITED = 998,
        POB_REGULAR = 999,
        PHYSICIAN_ASSISTANT = 1000,
        PHYSICIAN_ASSISTANT_SUPERVISED = 1001,
        POB_SUPER = 1002
    }
}