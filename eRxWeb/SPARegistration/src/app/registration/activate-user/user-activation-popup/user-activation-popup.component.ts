import { Component, Input,ViewChild } from '@angular/core';
import { ShowCaptchaComponent } from '../../show-captcha/show-captcha.component'
import { LinkExistingAccount } from '../link-existing-account/link-existing-account-component'
import { ActivationCodeService } from '../../../service/activation-code.service'
import { Router } from '@angular/router';
import { Workflow } from '../link-account-request-model'
import { NgForm } from '@angular/forms';
import { CaptchaModel } from '../../show-captcha/show-captcha.model';
import { ActivationCodeModel, ValidateActivationCodeModel } from '../activation-code.model';

@Component({
    selector: 'user-activation-popup',
    templateUrl: './user-activation-popup.component.html',
    styleUrls: ['./user-activation-popup.component.css']
})
export class UserActivationPopup  {
    @ViewChild('captchaControl') captchaControl: ShowCaptchaComponent;
    @ViewChild('linkExistingAccountControl') linkedAccount: LinkExistingAccount;
    activationCodeModel: ActivationCodeModel = new ActivationCodeModel();
    ShowModal: boolean;
    WorkflowType: Workflow;
    ShowLinkedAccount: boolean;

    isValidCaptcha: boolean = false;

    constructor(private activationService: ActivationCodeService, private router: Router) {
    }

    isUserActivationFormValid(data: any) {
        this.markAllControlAsTouched(data);
        this.getCaptchaDetails();
        return data.valid
            && this.isValidCaptcha;
    }

    markAllControlAsTouched(data: any) {
        Object.keys(data.controls).forEach(field => {
            const control = data.controls[field];
            control.markAsTouched({ onlySelf: true });
        })
    }

    activateUser(activationForm: any) {        
        if (this.isUserActivationFormValid(activationForm)) {
            this.activationService.validateActivationCode(this.activationCodeModel).subscribe((validateActivationCodeModel: ValidateActivationCodeModel) => {
                if (validateActivationCodeModel.IsValid) {
                    activationForm.controls["txtActivationCode"].setErrors(null);
                    this.ShowModal = false;
                    if (this.ShowLinkedAccount) {
                        this.showLinkedAccountPopup();
                    }
                    else {
                        this.router.navigate(["register/createAccount"]);
                    }
                }
                else {
                    if (!validateActivationCodeModel.IsValidCaptcha) {
                        this.captchaControl.ShowInValidCaptchaError = true;
                    }
                    else {
                        if (activationForm.controls["txtActivationCode"].errors == null) {
                            activationForm.controls["txtActivationCode"].setErrors({
                                "used": "invalid"
                            });
                        }
                    }
                    this.ShowModal = true;
                }
            });
        }
    }
    hide() {
        this.activationCodeModel = new ActivationCodeModel();
        this.ShowModal = false;
    }
    showLinkedAccountPopup()
    {
        this.linkedAccount.IsLinkedUser = true;    
    }

    getCaptchaDetails() {
        let captchaModel: CaptchaModel;
        captchaModel = this.captchaControl.GetCaptchaDetails()
        this.activationCodeModel.CaptchaText = captchaModel.captchaText;
        this.isValidCaptcha = captchaModel.isValid;
    }
}
