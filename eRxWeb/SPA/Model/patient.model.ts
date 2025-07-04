export class Patient {
    ActiveMed: string
    Allergy: string
    DOB: string
    Dx: string;
    FirstName: string = '[No Patient Selected]';
    LastName: string;
    LastPharmacyName: string;
    MaiIndicator: number;
    MaiIndicatorImageUrl: string;
    MaiIndicatorToolTip: string;
    IsMoEpcs: boolean;
    MoreActiveMedVisible: boolean;
    MoreMailOrderPharmVisible: boolean;
    MoreActiveProblem: boolean;
    MoreActiveAllergy: boolean;
    MoreRetailPharm:boolean;
    MRN: string;
    IsRetailEpcs: boolean;
    RestrictedUserImageUrl: string;
    IsRestrictedUser: boolean;
    IsRestrictedPatient: boolean;
    IsVipPatient: boolean;
    RealName: string;
    PrefMOP: string;
    RemMOPharmVisible: boolean;
    RemPharmacyVisible: boolean;
    WeightKgs: string;
    WeightLbs: string;
    WeightOzs: string;
    HeightLabel: string;
    WeightLabel: string;
    AllowAllergyEdit: boolean;
    AllowPharmacyEdit: boolean;
    AllowDiagnosisEdit: boolean;
    ActiveDignosises: ActiveDignosis[];
    ActiveAllergies: ActiveAllergy[];
    ActiveMeds: ActiveMeds[];
    ToolTip: string;
    AllowPatientEdit: boolean;
    PatientID: string;
}

export class PatientResponse {
    PatientId: string;
}

export class ActiveDignosis {
    Diagnosis: string;
    StartDate: string;
}
export class ActiveAllergy {
    Name: string;
    StartDate: string;
}


export class ActiveMeds {
    Name: string;
    StartDate: string;
}

export class EligAndMedHxStatusModel {
    Type: string;
    ProcessedDate: string;
    Message: string;
    AuditID: string;
}

export class PatientMedRecDetailModel {
    ActionType: ActionType;
    Type: string;
    ReconciliationMessage: string;
    DoesPatientHaveValidMedAndAllergy: boolean;
}

export enum ActionType {
    Save = 0,
    SavePrescribe = 1,
}

export interface IPharmacyInfo {
    LastPharmacyName: string;
    IsMoEpcs: boolean;
    MoreMailOrderPharmVisible: boolean;
    MoreRetailPharm: boolean;
    IsRetailEpcs: boolean;
    PrefMOP: string;
    RemMOPharmVisible: boolean;
    RemPharmacyVisible: boolean;
}

export interface IDiagnosisInfo {
    Dx: string;
    MoreActiveProblem: boolean;
    ActiveDiagnosis: ActiveDignosis[];
}

export interface IAllergyInfo {
    Allergy: string;
    MoreActiveAllergy: boolean;
    ActiveAllergies: ActiveAllergy[];
}

export interface IMedicationInfo {
    ActiveMed: string;
    MoreActiveMedVisible: boolean;
    ActiveMeds: ActiveMeds[];
}