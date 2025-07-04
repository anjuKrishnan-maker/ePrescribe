import {UserCredential} from './user-credential.model';
import {DEALicense} from './dea-license.model';
import {StateLicense} from './user-state-license.model';

export class UserInfo {
    FirstName: string;
    LastName: string;
    MI: string;
    LoginId: string;
    Email: string;
    ConfirmEmail: string;
    IsActive: Boolean;
    name = this.FirstName + ' ' + this.LastName;
    lastLogin:string;
}
export enum UserType {
    Provider =1,
    PAwoSup,
    PAwithSup,
    POBnoReview,
    POBSomeReview,
    POBAllReview,
    Staff
}
export class UserEPCSSetting {
    IsApprover: Boolean;
    IsEPCSOn:Boolean;
}
export class User {
    constructor() {
        this.Login = new UserInfo();
        this.UserCredential = new UserCredential();
    }
    Login: UserInfo;
    UserType: UserType;
    IsAdmin: Boolean;
    EPCSPermission: UserEPCSSetting;
    UserCredential: UserCredential;
    DEALicenses: DEALicense[];
    StateLicenses: StateLicense[];
}