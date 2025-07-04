import { UserMode } from "./user/user.model";

export interface DEALicense {
    DEALicenseID: string;
    DEALicenseTypeId: DeaLicenseType;
    DEANumber: string;
    DEAExpirationDate: string;
    DatePickerDate: Date;
    DEAIAllowed: boolean;
    DEAIIAllowed: boolean;
    DEAIIIAllowed: boolean;
    DEAIVAllowed: boolean;
    DEAVAllowed: boolean;
    IsDefault: boolean;
}


export enum DeaLicenseType {    
    Primary = 1,
    Additional = 2,
    NADEAN = 3
}

export interface UserDeaLicensesModel{
    DEALicenses: DEALicense[];
    UserID: string;
    UserMode: UserMode;
    IsRestrictedMode: boolean;
}

export interface DEALicenseRequest {
    userID: string;
    dEALicense: DEALicense;
}