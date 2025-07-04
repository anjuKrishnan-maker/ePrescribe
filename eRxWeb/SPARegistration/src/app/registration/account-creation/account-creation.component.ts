import { Component, OnInit, Inject, Input, ViewChild } from '@angular/core';
import { AccountDetails, ValidateCreateUser, SecretAnswers, SecretQuestionsAns } from '../account-creation/account-creation.model';
import { DomSanitizer } from '@angular/platform-browser';
import { UserCreateService } from '../../service/user-create.service';
import { Router, NavigationExtras } from '@angular/router';
import '../../Utils/String.Extension';
import { ShowCaptchaComponent } from '../../registration/show-captcha/show-captcha.component'
import { UserPasswordComponent } from '../user-password/user-password.component';
import { UserAccountNameComponent } from '../user-account-name/user-account-name.component';
import { UserSecurityQuestionsComponent } from '../user-security-questions/user-security-questions.component';
import { LoaderService } from '../../service/loader.service';
import { CaptchaModel } from '../show-captcha/show-captcha.model';
import { UserAccountName } from '../user-account-name/user-account-name.model';
import { UserPassword } from '../user-password/user-password.model';
import { SecurityQuestionAnswerModel } from '../user-security-questions/user-security-questions.model';

@Component({
    selector: 'app-create-account',
    templateUrl: './account-creation.component.html',
    styleUrls: ['./account-creation.component.css']
})
export class AccountCreationComponent implements OnInit {
    errorMessage: string;
    isValidationFailed: boolean=false;
    userAccount: AccountDetails = new AccountDetails();
    isPasswordValid: boolean = false;
    isSecurityQuestionAnswerValid: boolean = false;
    isUserNameValid: boolean = true;
    userExistsErrorMessage: string;
    userNameLabel: string;
    isValidCaptcha: boolean = false;
    captcha: string = "";
    componentState: NavigationExtras["state"];
    namePattern = "^([a-zA-Z]+[\\s-'.]{0,35})*";
    @ViewChild(ShowCaptchaComponent) captchaComponent: ShowCaptchaComponent;
    @ViewChild('pwdChild') pwdChild: UserPasswordComponent;
    @ViewChild('userNameChild') userNameChild: UserAccountNameComponent;
    @ViewChild('securityQuestion') securityQuestion: UserSecurityQuestionsComponent;
    @ViewChild('captchaControl') captchaControl: ShowCaptchaComponent;

    
    constructor(private userCreateService: UserCreateService,
        private loaderService: LoaderService,
        @Inject('window') private window: any, ) {
    }
    ngOnInit() {
        this.userExistsErrorMessage = "Username is not available";
        this.userNameLabel = "Username";
    }

    validateShieldUserAccount() {

        this.getUserAccountNameDetails();
        this.getPasswordDetails();
        this.getSecurityQuestionAnswerDetails();
        return (this.isPasswordValid && this.isUserNameValid && this.isSecurityQuestionAnswerValid);

    }

    isUserAccountFormValid(data: any) {
        this.markFormControlsTouched(data);
        this.getCaptchaDetails();
        let isShieldUserAccountValid = this.validateShieldUserAccount();
        return data.valid
            && isShieldUserAccountValid
            && this.isValidCaptcha;
    }
     markFormControlsTouched(userAccountForm: any) {
        Object.keys(userAccountForm.controls).forEach(field => {
            const control = userAccountForm.controls[field];
            control.markAsTouched({ onlySelf: true });
        });
    }
    onSubmit(userAccountForm: any) {
        this.isValidationFailed = false;
        if (this.isUserAccountFormValid(userAccountForm)) {
            this.userAccount.secretQuestionsField = this.generateScretAnswer();
            this.userCreateService
                .saveUserData(this.userAccount).subscribe((validateCreateUser: ValidateCreateUser) => {
                    if (validateCreateUser.IsDataSaved) {
                        this.loaderService.show(true);
                        this.window.open(validateCreateUser.RedirectUrl, "_self");
                    }
                    else {
                        this.isValidationFailed = true;
                        if (!validateCreateUser.IsValidCaptcha) {
                            this.captchaControl.ShowInValidCaptchaError = true;
                        }
                        else {
                            this.errorMessage = validateCreateUser.ErrorMessage;
                        }
                    }
                });
        }
    }

    generateScretAnswer() {
        let secretAnsweList: SecretAnswers[] = [];
        let secretAnswer: SecretAnswers = new SecretAnswers();
        let secretAnswer2: SecretAnswers = new SecretAnswers();
        let secretAnswer3: SecretAnswers = new SecretAnswers();
        secretAnswer.questionIDField = Number(this.userAccount.securityQuestion1);
        secretAnswer.answerField = this.userAccount.securityAnswer1;
        secretAnsweList.push(secretAnswer);
        secretAnswer2.questionIDField = Number(this.userAccount.securityQuestion2);
        secretAnswer2.answerField = this.userAccount.securityAnswer2;
        secretAnsweList.push(secretAnswer2);
        secretAnswer3.questionIDField = Number(this.userAccount.securityQuestion3);
        secretAnswer3.answerField = this.userAccount.securityAnswer3;
        secretAnsweList.push(secretAnswer3);
        return secretAnsweList;
    }

    getCaptchaDetails() {
        let captchaModel: CaptchaModel;
        captchaModel = this.captchaControl.GetCaptchaDetails()
        this.userAccount.CapchaText = captchaModel.captchaText;
        this.isValidCaptcha = captchaModel.isValid;
    }

    getUserAccountNameDetails() {
        let userAccountName: UserAccountName;
        userAccountName = this.userNameChild.GetUserAccountNameDetails();
        this.userAccount.userName = userAccountName.shieldUserName;
        this.isUserNameValid = userAccountName.isValid;
    }

    getPasswordDetails() {
        let userPassword: UserPassword;
        userPassword = this.pwdChild.GetPasswordDetails();
        this.userAccount.password = userPassword.password;
        this.userAccount.confirmPassword = userPassword.confirmPassword;
        this.isPasswordValid = userPassword.isValid;
    }

    getSecurityQuestionAnswerDetails() {
        let securityQuestionAnswerModel: SecurityQuestionAnswerModel;
        securityQuestionAnswerModel = this.securityQuestion.GetSecurityQuestionAnswerDetails();
        this.userAccount.securityQuestion1 = securityQuestionAnswerModel.selectedShieldQuestionOne;
        this.userAccount.securityQuestion2 = securityQuestionAnswerModel.selectedShieldQuestionTwo;
        this.userAccount.securityQuestion3 = securityQuestionAnswerModel.selectedShieldQuestionThree;
        this.userAccount.securityAnswer1 = securityQuestionAnswerModel.securityAnswerOne;
        this.userAccount.securityAnswer2 = securityQuestionAnswerModel.securityAnswerTwo;
        this.userAccount.securityAnswer3 = securityQuestionAnswerModel.securityAnswerThree;
        this.isSecurityQuestionAnswerValid = securityQuestionAnswerModel.isValid;
    }

}