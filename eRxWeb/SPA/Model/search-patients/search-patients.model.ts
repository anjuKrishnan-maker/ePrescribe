import Messagemodel = require("../message.model");
import MessageModel = Messagemodel.MessageModel;

export class SearchPatientStartupParameters {
    IsAddDiagnosisVisible: boolean;
    IsSsoLockdownMode: boolean;
    IsIdProofingRequired: boolean;
    IsWelcomeTourDisplayed: boolean;
    IsDeluxeTeaserAdDisplayed: boolean;
    IsCreditCardExpiring: boolean;
    IsPrivacyOverrideDisplayed: boolean;
    IsPasswordSetupRequiredForSSOUser: boolean;
    IsShowAddPatient: boolean;
    PatientID: string;
    DelegateProviderId: string;
    UserID: string;
    UserType: SearchPatientUserType;
    UserCategory: SearchPatientUserCategory;
    Providers: Array<DelegateProvider>;
    WelcomeTourModel: WelcomeTourModel;
    DeluxeTeaserAdModel: DeluxeTeaserAdModel;
    SearchPatientResponse: SearchPatientResponse;
    Message: MessageModel;
}
export class WelcomeTourModel {
    public TourType: number;
    public SourceUrl: string;    
}
export class DeluxeTeaserAdModel {
    public TeaserAdContent: Array<TeaserAdResponse>;
    public IsCompulsaryBasic: boolean;
    public AdType: string;
    public AdHtml: string;
    public Cookie: string;
}

export class TeaserAdResponse {
    public contentField: string;
    public idField: number;
}
export class GetStartupParametersRequest {
    PatientDemographics: PatientDemographics;
}
export class SearchPatientDataRequest {
    //PatientID: string;
    LastName: string;
    FirstName: string;
    DateOfBirth: string;
    ChartID: string;
    WildCard: string;
    //HasVIPPatients: string; -- this we can get from in the API layer from session.license.hasVIPPatients
    IncludeInactive: boolean;
    UserType: SearchPatientUserType;
    PatientGuid: string;
}

export class SearchPatientResponse {
    public Patients: PatientItemModel[]
    //Messages: 
}

export class PatientItemModel {
    // items from service
    PatientID: string;
    MRN: string;
    Name; string;
    DOB: Date;
    Address: number;
    Phone: string;
    StatusID: string;
    IsVIPPatient: boolean;
    IsRestrictedPatient: boolean;
    Selected: boolean;
}

export class SetProviderInformationRequest {
    ProviderId: string;
}

export class LoadProvidersForSupervisedPARequest {
    ProviderId: string;
}

export class DelegateProvider {
    ProviderName: string;
    ProviderId: string;
    UserCategory: SearchPatientUserCategory;
}

export enum SearchPatientUserType {
    Provider = 1,
    PAwithSupervision = 2,
    Staff = 3
}
export class SelectPatientComponentParameters {
    PatientId: string;
    PatientName: string;
    MessageVisibility: boolean;
    MessageIcon: string;
    MessageTooltip: string;
    MessageText: string;
    PatientDemographics: PatientDemographics;
}
export class PatientDemographics {
    LastName : string;
    PaternalName : string;
    MaternalName : string;
    FirstName : string;
    MiddleName : string;
    Dob : string;		
    Mrn : string;
    Address1 : string;
    Address2 : string;
    City : string;
    State : string;
    Zip : string;
    Phone : string;
    MobilePhone : string;
    Gender : string;
    Email : string;
    IsPatientHistoryExcluded : string;
    PreferredLanguageID : string;
    Weight : string;
    IsHealthPlanDisclosable : string;
}

export enum SearchPatientUserCategory {
    GENERAL_USER = 0,
    PROVIDER = 1,
    POB_LIMITED = 998,
    POB_REGULAR = 999,
    PHYSICIAN_ASSISTANT = 1000,
    PHYSICIAN_ASSISTANT_SUPERVISED = 1001,
    POB_SUPER = 1002
}

export class CheckInPatientDataRequest
{
    ProviderId: string
}

export class CheckInPatientDataResponse
{
    CheckedInMessage: string;
    CheckedInMessageVisibility: boolean;
    CheckedInMessageIcon: string;
}

export enum NextPage {
    SelectPatient = 1,
    SelectMed = 2,
    ReviewHistory = 3
}