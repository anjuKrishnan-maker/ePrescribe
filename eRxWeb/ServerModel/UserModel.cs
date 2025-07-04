
using eRxWeb.ServerModel;
using eRxWeb.AppCode.AngularHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static Allscripts.ePrescribe.Common.Constants;
using UserCategory = Allscripts.ePrescribe.Common.Constants.UserCategory;
using eRxWeb.ePrescribeSvc;

namespace eRxWeb.ServerModel
{
    public class UserModel
    {
        public bool IsAdmin { get; set; }
        public UserMode UserMode { get; set; }
        public string UserID { get; set; }
        public UserInformationModel UserInformation { get; set; }
        public UserSecurityModel UserSecurity { get; set; }
        public UserSettingsModel UserSettings { get; set; }
        public UserCredentialsModel UserCredentials { get; set; }
        public UserPreferencesModel UserPreferences { get; set; }
        public UserMobileMessageSettingsModel UserMobileMessageSettings { get; set; }
        public UserEPCSSettingsModel UserEPCSSettings { get; set; }
        public UserStateLicensesModel UserStateLicenses { get; set; }
        public UserDEALicensesModel UserDEALicenses { get; set; }
        public string PracticeState { get; set; }
    }

    public class UserInformationModel
    {
        public string FirstName { get; set; }
        public string MI { get; set; }
        public string LastName { get; set; }
        public string LoginID { get; set; }
        public bool IsDisplayLoginID { get; set; }
        public string WorkEmail { get; set; }
        public bool IsEditedBySomeoneElse { get; set; }
        public bool IsLocked { get; set; }
        public bool IsActive { get; set; }
        public bool IsForcePasswordChange { get; set; }
        public bool IsRestrictedMode { get; set; }

    }
    public class UserSecurityModel
    {
        public bool IsAdmin { get; set; }
        public bool IsAdminEnabled { get; set; }
        public bool IsShowChangePassword { get; set; }
        public bool IsShowEditSecretQuestions { get; set; }
        public SecretQuestion[] SecretQuestionsList { get; set; }
        public SecretQuestion[] UserSecretQuestions { get; set; }
        public bool IsRestrictedMode { get; set; }
    }


    public class UserCredentialsModel
    {
        public string Title { get; set; }
        public string Suffix { get; set; }
        public string NPI { get; set; }
        public string CurrentNPI { get; set; }
        public string Specialty1 { get; set; } = string.Empty;
        public string Specialty2 { get; set; } = string.Empty;
        public string Assmca { get; set; }
        public List<DropDownListElement> SpecialtyList { get; set; }
        public bool IsPRSite { get; set; }
        public bool IsRestrictedMode { get; set; }
    }

    public class UserPreferencesModel
    {
        public string DefaultFaxSiteID { get; set; }
        public List<DropDownListElement> SiteList { get; set; }
        public bool RTBPreference { get; set; }
        public bool AutoTherapeuticAlternativesPreference { get; set; }
        public bool RTPSPreference { get; set; }
        public bool PPTPreference { get; set; }
        public bool PPTOffersPreference { get; set; }
        public bool IsShowRxFavourites { get; set; }
        public bool IsFaxSiteInvalid { get; set; }
        public bool IsRestrictedMode { get; set; }
    }


    public class UserSettingsModel
    {
        public UserCategory UserType { get; set; }
        public List<UserSupervisingProviderModel> UserSupervisingProvidersList { get; set; }
        public SupervisingProviderSelectionMode SupervisingProviderSelectionMode { get; set; }
        public bool IsRestrictedMode { get; set; }
        public string Name { get; set; }

        public UserSettingsModel()
        {
            UserSupervisingProvidersList = new List<UserSupervisingProviderModel>();
        }
    }

    public class UserMobileMessageSettingsModel
    {
        public string UserID { get; set; }
        public UserCategory UserType { get; set; }
        public bool IsSMSEnabled { get; set; }
        public string PhoneNumber { get; set; }
        public bool SmsAlertNewRxError { get; set; }
        public bool SmsAlertRefillRxError { get; set; }
        public bool SmsAlertChangeRxError { get; set; }
        public bool SmsAlertCancelRxError { get; set; }
        public bool SmsAlertTransmissionError { get; set; }
        public bool SmsAlertChangeRxRequest { get; set; }
        public bool SmsAlertDEALicenseExpiration { get; set; }
        public bool SmsAlertServiceUpdate { get; set; }
        public bool IsNumberAvailable { get; set; }
        public bool IsChanged { get; set; }
        public bool IsSMSAlertDailyCountReached { get; set; }

    }

    public class UserStateLicensesModel
    {
        public string[][] StatesAndLicenseTypes { get; set; }
        public StateLicense[] UserStateLicenses { get; set; }
        public string UserID { get; set; }
        public UserCategory UserType { get; set; }
        public UserMode UserMode { get; set; }
        public string PracticeState { get; set; }
        public bool IsRestrictedMode { get; set; }
    }

    public class UserDEALicensesModel
    {
        public DEALicense[] DEALicenses { get; set; }
        public string UserID { get; set; }
        public UserMode UserMode { get; set; }
        public bool IsRestrictedMode { get; set; }
    }

    public enum UserMode
    {
        SelfEdit = 0,
        EditOtherUser = 1,
        AddOtherUser = 2
    }

    public class SaveUserResponseModel
    {
        public bool IsSaveSuccessful { get; set; }
        public List<MessageModel> MessageList { get; set; }
        public UserActivationInfoModel UserActivationInfo { get; set; }
        public string RedirectUrl { get; set; }

        public SaveUserResponseModel()
        {
            MessageList = new List<MessageModel>();
        }
    }

    public enum SupervisingProviderSelectionMode
    {
        All = 0,
        Select = 1
    }

    public class UserSupervisingProviderModel
    {
        public string ProviderId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool Selected { get; set; }
        public string NPI { get; set; }
        public string DEA { get; set; }
    }

    public class UserActivationInfoModel
    {
        public string PersonalEmail { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ActivationCode { get; set; }
        public string RegistrationUrl { get; set; }
        public UserCommunicationStatusModel UserCommunicationStatus { get; set; }
    }

    public class UserCommunicationStatusModel
    {
        public bool IsPrintChecked { get; set; }
        public bool IsEmailChecked { get; set; }
        public bool IsEmailSentSuccessfully { get; set; }
        public bool IsEmailAttempted { get; set; }
    }

}
