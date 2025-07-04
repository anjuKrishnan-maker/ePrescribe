import { Injectable } from '@angular/core';
import { DataService } from './data.service';
import { Observable } from 'rxjs';
import { LinkAccountRequestModel, Workflow, LinkExistingUserModel } from '../registration/activate-user/link-account-request-model'
import { ActivationCodeModel, ValidateActivationCodeModel } from '../registration/activate-user/activation-code.model';
@Injectable()
export class ActivationCodeService {
    constructor(private dataService: DataService) {
    }
    
    validateActivationCode(activationCodeModel: ActivationCodeModel): Observable<ValidateActivationCodeModel> {
        return this.dataService.post("api/anonymous/user-activation/validate-activation-code/", activationCodeModel);
    }
    intializeWorkFlow(data: any): Observable<Workflow> {
        return this.dataService.post("api/anonymous/user-activation/intialize-workflow/", data);
    }
    linkToExistingUser(linkAccountModel: LinkAccountRequestModel): Observable<LinkExistingUserModel> {
        return this.dataService.post("api/anonymous/user-activation/link-existing-user/", linkAccountModel);
    }
}