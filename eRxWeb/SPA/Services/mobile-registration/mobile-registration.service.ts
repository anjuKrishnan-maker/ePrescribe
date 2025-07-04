import { Injectable } from '@angular/core';
import { BaseService } from './../base.service';
import { HttpClient } from '@angular/common/http';
import { UserMobileMessageSettingsModel, VerificationCodeSMSModel } from '../../model/user/user.model';
import { EPSResponse } from '../../model/model.import.def';


@Injectable()
export class MobileRegistrationService extends BaseService {
    constructor(http: HttpClient) {
        super(http);
    }

    SendMessageTwilio(verificationCodeSMS: VerificationCodeSMSModel) {
        return this.InvokeApiMethod<EPSResponse>('api/UserApi/SendMessageTwilio', verificationCodeSMS);
    }

    SaveUserMobileNumber(verificationCodeSMS: VerificationCodeSMSModel) {
        return this.InvokeApiMethod<boolean>('api/UserApi/SaveUserMobileNumber', verificationCodeSMS);
    }

    DeleteUserMobileNumber(verificationCodeSMS: VerificationCodeSMSModel) {
        return this.InvokeApiMethod<boolean>('api/UserApi/DeleteUserMobileNumber', verificationCodeSMS);
    }

    GetUserMobileMessageSettings(verificationCodeSMS: VerificationCodeSMSModel) {
        return this.InvokeApiMethod<UserMobileMessageSettingsModel>('api/UserApi/GetUserMobileMessageSettings', verificationCodeSMS);
    }

    MobileMessageSettingAuditlog(verificationCodeSMS: VerificationCodeSMSModel) {
        return this.InvokeApiMethod('/api/UserApi/MobileMessageSettingAuditlog', verificationCodeSMS)
    }
}