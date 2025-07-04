import { UserCategory, UserMode } from "./user/user.model";

export interface UserStateLicensesModel{
    StatesAndLicenseTypes: string[][];
    UserStateLicenses: StateLicense[];
    UserID: string;
    UserType: UserCategory;
    UserMode: UserMode;
    PracticeState: string;
    IsRestrictedMode: boolean;
}

export interface StateLicense {
    State: string;
    ExpiringDate: string;
    LicenseNo: string;
    LicenseType: string;
    DatePickerDate: Date;
}

export interface UserStateLicenseRequest {
    currentUserStateLicense: StateLicense;
    oldUserStateLicense: StateLicense;
    userID: string;
}