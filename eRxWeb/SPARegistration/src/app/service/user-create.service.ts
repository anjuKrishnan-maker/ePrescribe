import { Injectable, Inject } from '@angular/core';
import { DataService } from './data.service';
import { Observable } from 'rxjs';
import { ValidateRegistrantModel, CreateRegistrantUserRequest } from '../registration/user-creation/user-creation.model';
import { AccountDetails,  ValidateCreateUser } from '../registration/account-creation/account-creation.model';

@Injectable()
export class UserCreateService {
    private supportMailAddress: string;
    constructor(private dataService: DataService, @Inject('window') private window: any) {
        this.supportMailAddress = this.window?.appcontext?.supportMailAddress;
    }

    getInitialPageData() {
        return this.dataService.get("api/anonymous/user-registration/setup-information");
    }

    getSecurityQuestions() {
        return this.dataService.get("api/anonymous/user-registration/setup-security-questions");
    }

    validateShieldUserName(userName: string): Observable<boolean> {
        return this.dataService.get<boolean>("api/anonymous/user-registration/checkuser/" + userName);
    }

    getCaptcha() {
        return this.dataService.get("api/anonymous/user-registration/getcaptcha");
    }

    saveRegistrationData(userData: CreateRegistrantUserRequest): Observable<ValidateRegistrantModel> {
        let customErrorMessage = `We encountered an error while trying to register. Kindly contact the ePrescribe support at (${this.supportMailAddress}), include your email address (${userData.RegistrantUser.personalEmail}) for assistance.`;
        return this.dataService.post("api/anonymous/user-registration/create-registrant", userData, customErrorMessage);
    }

    saveUserData(userData: AccountDetails): Observable<ValidateCreateUser> {
        let customErrorMessage = `We encountered an error while trying to register. Kindly contact the ePrescribe support at (${this.supportMailAddress}), include your email address (${userData.personalEmail}) for assistance.`;
        return this.dataService.post("api/anonymous/user-registration/create-user", userData, customErrorMessage);
    }
} 