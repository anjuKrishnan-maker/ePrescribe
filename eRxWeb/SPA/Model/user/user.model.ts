import { DropDownListElement } from "../angular-ui-helper/drop-down-list-element.model";
import { UserEPCSSettingModel } from "../epcs-settings.model";
import { UserStateLicensesModel } from "../user-state-license.model";
import { UserDeaLicensesModel } from "../dea-license.model";
import { MessageModel } from "../message.model";
import { SecretQuestion } from "../secret-question-answer.model";

export interface UserModel {
    IsAdmin: boolean;
    UserMode: UserMode;
    UserID: string;
    UserInformation: UserInformationModel;
    UserSecurity: UserSecurityModel;
    UserSettings: UserSettingsModel;
    UserCredentials: UserCredentialsModel;
    UserPreferences: UserPreferencesModel;
    UserMobileMessageSettings: UserMobileMessageSettingsModel;
    UserEPCSSettings: UserEPCSSettingModel;
    UserStateLicenses: UserStateLicensesModel;
    UserDEALicenses: UserDeaLicensesModel;
    PracticeState: string;
}

export interface UserRequest {
    UserMode: UserMode;
    UserID: string;
    IsNoLic: boolean;
}

export enum UserMode {
    SelfEdit = 0,
    EditOtherUser = 1,
    AddOtherUser = 2
}

export enum UserCategory {
    GENERAL_USER = 0,
    PROVIDER = 1,
    POB_LIMITED = 998,
    POB_REGULAR = 999,
    PHYSICIAN_ASSISTANT = 1000,
    PHYSICIAN_ASSISTANT_SUPERVISED = 1001,
    POB_SUPER = 1002
}

export enum SupervisingProviderSelectionMode {
    All = 0,
    Select = 1
}

export interface UserInformationModel {
    FirstName: string;
    MI: string;
    LastName: string;
    LoginID: string;
    IsDisplayLoginID: boolean;
    IsEditedBySomeoneElse: boolean;
    WorkEmail: string;
    IsLocked: boolean;
    IsActive: boolean;
    IsForcePasswordChange: boolean;    
    IsRestrictedMode: boolean;
}

export interface UserSecurityModel {
    IsAdmin: string;
    IsAdminEnabled: string;
    IsShowChangePassword: boolean;
    IsShowEditSecretQuestions: boolean;
    SecretQuestionsList: SecretQuestion[];
    UserSecretQuestions: SecretQuestion[];
    IsRestrictedMode: boolean;
}

export interface UserCredentialsModel {
    Title: string;
    Suffix: string;
    NPI: string;
    CurrentNPI: string;
    Specialty1: string;
    Specialty2: string;
    SpecialtyList: DropDownListElement[];
    Assmca: string;
    IsPRSite: boolean;
    IsRestrictedMode: boolean;
}

export interface UserPreferencesModel {
    DefaultFaxSiteID: string;
    SiteList: DropDownListElement[];
    RTBPreference: boolean;
    AutoTherapeuticAlternativesPreference: boolean;
    RTPSPreference: boolean;
    PPTPreference: boolean;
    PPTOffersPreference: boolean;
    IsShowRxFavourites: boolean;
    IsFaxSiteInvalid: boolean;
}

export interface UserMobileMessageSettingsModel {
    UserID: string;
    UserType: UserCategory;
    IsSMSEnabled: boolean;
    PhoneNumber: string;
    SmsAlertNewRxError: boolean;
    SmsAlertRefillRxError: boolean;
    SmsAlertChangeRxError: boolean;
    SmsAlertCancelRxError: boolean;
    SmsAlertTransmissionError: boolean;
    SmsAlertChangeRxRequest: boolean;
    SmsAlertDEALicenseExpiration: boolean;
    SmsAlertServiceUpdate: boolean;
    IsNumberAvailable: boolean;
    IsChanged: boolean;
    IsSMSAlertDailyCountReached: boolean;
}

export interface UserSettingsModel {
    UserType: UserCategory;
    UserSupervisingProvidersList: UserSupervisingProviderModel[];
    SupervisingProviderSelectionMode: SupervisingProviderSelectionMode;
    IsRestrictedMode: boolean;
    Name: string;
}

export interface Speciality {
    SpecialtyCode: string;
    SpecialtyDescription: string;
}

export interface UserSupervisingProviderModel {
    ProviderId: string;
    Selected: boolean;
    FirstName: string;
    LastName: string;
    NPI: string;
    DEA: string;
}

export interface SaveUserResponseModel {
    IsSaveSuccessful: boolean;
    MessageList: MessageModel[];
    UserActivationInfo: UserActivationInfoModel;
    RedirectUrl: string;
}

export interface VerificationCodeSMSModel {
    userID: string;
    phoneNumber: string;
    otp: number;
    message: string;
    status: SMSAlertUserActionType;
}

export interface UserCommunicationStatusModel {
    IsPrintChecked: boolean;
    IsEmailChecked: boolean;
    IsEmailSentSuccessfully: boolean;
    IsEmailAttempted: boolean;

}

export interface UserActivationInfoModel {
    PersonalEmail: string;
    FirstName: string;
    LastName: string;
    ActivationCode: string;
    RegistrationUrl: string;
    UserCommunicationStatus: UserCommunicationStatusModel;
}

export enum SMSAlertUserActionType {
    AcceptTerms = 1,
    CompleteRegistration = 2,
    NewRxErrorAlertOn = 3,
    NewRxErrorAlertOff = 4,
    DeleteRegistration = 5,
    SMSLimitExceeded = 6,
    UpdateRegistration = 7,
    RefillRxErrorAlertOn = 8,
    RefillRxErrorAlertOff = 9,
    ChangeRxErrorAlertOn = 10,
    ChangeRxErrorAlertOff = 11,
    CancelRxErrorAlertOn = 12,
    CancelRxErrorAlertOff = 13,
    ChangeRxRequestAlertOn = 14,
    ChangeRxRequestAlertOff = 15,
    DEALicenseExpirationAlertOn = 16,
    DEALicenseExpirationAlertOff = 17,
    ServiceUpdateAlertOn = 18,
    ServiceUpdateAlertOff = 19,
}

export interface UserBackButtonModel {
    UserID: string;
    To: string;
    UserMode: UserMode;
    NoNpi: boolean;
    ExpDea: boolean;
    NoLic: boolean;
    CameFrom: string;
    RedirectUrl: string;
    IsSaveSuccessful: boolean;
}