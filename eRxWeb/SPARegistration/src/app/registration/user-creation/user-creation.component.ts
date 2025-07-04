import { Component, OnInit, Input, Inject, ViewChild } from '@angular/core';
import { UserDetails, shieldSecretQuestion, SecretAnswers, State, Speciality, CreateRegistrantUserRequest, ValidateRegistrantModel } from '../user-creation/user-creation.model';
import { DomSanitizer } from '@angular/platform-browser';
import { UserCreateService } from '../../service/user-create.service';
import { Router, NavigationExtras } from '@angular/router';
import '../../Utils/String.Extension';
import { ShowCaptchaComponent } from '../../registration/show-captcha/show-captcha.component'
import { UserPasswordComponent } from '../user-password/user-password.component';
import { UserAccountNameComponent } from '../user-account-name/user-account-name.component';
import { UserSecurityQuestionsComponent } from '../user-security-questions/user-security-questions.component';
import { ExistingUserAccount } from '../user-existing-account/user-existing-account.model';
import { UserExistingAccountComponent    } from '../user-existing-account/user-existing-account.component';
import { CaptchaModel } from '../show-captcha/show-captcha.model';
import { IsStringNullUndefinedEmpty } from '../../utils/string-validator';
import { UserAccountName } from '../user-account-name/user-account-name.model';
import { UserPassword } from '../user-password/user-password.model';
import { SecurityQuestionAnswerModel } from '../user-security-questions/user-security-questions.model';

@Component({
    selector: 'app-user-creation',
    templateUrl: './user-creation.component.html',
    styleUrls: ['./user-creation.component.css']
    
})
export class UserCreationComponent implements OnInit {
    sldUserName: string;
    userDetail: UserDetails;
    registrantuserdata: CreateRegistrantUserRequest = new CreateRegistrantUserRequest();
    states: State[];
    SpecialityList: Speciality[];
    ShieldQuestions: any[];
    SelectedShieldQuestion1: any;
    SelectedShieldQuestion2: any;
    SelectedShieldQuestion3: any;
    isValidUser: boolean = false;
    isValidNPI: boolean = true;
    showInvalidDeaError: boolean = false;
    isValidCaptcha: boolean = false; 
    isDEARequired: boolean = false;
    isDEAExpDateRequired: boolean = false;
    isDEAScheduleRequired: boolean = false;
    idmeTerms: string;
    mainAppLink: string;
    isValidationFailed: boolean = false;
    isLinkExistingShieldUser: boolean = false;
    namePattern = "^([a-zA-Z]+[\\s-'.]{0,35})*";
    suffixPattern = "^([a-zA-Z]+[\\s-'.]{0,6})*";
    datePattern = "^(0?[1-9]|1[0-2])\\-(0?[1-9]|1\\d|2\\d|3[01])-\\d{4}$";
    zipCodePattern = "^(\\d{5})(?:\\d{4})?$";
    cityPattern = "^([a-zA-Z]+[\\s-'.]{0,20})*";
    shieldUserNamePattern = "^([a-zA-Z0-9]+[-,_.]{0,30})*";
    phoneFaxPattern = "\\(?\\d{3}\\)?-? *\\d{3}-? *-?\\d{4}";
    noLeadingSpacePattern = "^[^ ]([\\s\\S])*";
    componentState: NavigationExtras["state"];
    showModal = false;
    isPasswordValid: boolean=false;
    isSecurityQuestionAnswerValid: boolean = false;
    isUserNameValid: boolean = false;
    isExistingAccountValid: boolean = false;
    userExistsErrorMessage: string;
    userNameLabel: string;
    @ViewChild('pwdChild') pwdChild: UserPasswordComponent;
    @ViewChild('userNameChild') userNameChild: UserAccountNameComponent;
    @ViewChild('securityQuestion') securityQuestion: UserSecurityQuestionsComponent;
    @ViewChild('userExisting') userExisting: UserExistingAccountComponent;
    @ViewChild('captchaControl') captchaControl: ShowCaptchaComponent;
  
    constructor(private userCreateService: UserCreateService, private domSanitizer: DomSanitizer, private router: Router,
        @Inject('window') private window: any, ) {
        this.componentState = this.router.getCurrentNavigation()?.extras?.state;
    }
    ngOnInit() {
        this.userDetail = new UserDetails();
        this.userDetail.deaScheduleII = false;
        this.userDetail.deaScheduleIII = false;
        this.userDetail.deaScheduleIV = false;
        this.userDetail.deaScheduleV = false;

        this.getInitialPageData();
       
        this.idmeTerms = "https://www.id.me/terms"; //Fetch this from server call - db config

        if (this.componentState !== undefined && this.componentState !== null) {
            this.userDetail.pricingStructure = this.componentState.SelectedPricingStructure;
        }
        this.mainAppLink = this.window?.appcontext?.login;
        this.userExistsErrorMessage = "Username already exists. Kindly provide another username.";
        this.userNameLabel = "Username";
    }
    getInitialPageData() {
        this.userCreateService.getInitialPageData().subscribe(result => {

            this.states = result.States as State[];
            this.SpecialityList = result.Speciality as Speciality[];
        });
    }    
    onNpiChanged() {
        this.isValidNPI = true;
    }

    setDeaValidation() {
        this.showInvalidDeaError = false;
        let isDEARequired = false;
        let isDEAExpDateRequired = false;
        let isDEAScheduleRequired = false;
        if (!IsStringNullUndefinedEmpty(this.userDetail.deaNumber)
            || !IsStringNullUndefinedEmpty(this.userDetail.deaExpirationDate)
            || this.userDetail.deaScheduleII || this.userDetail.deaScheduleIII || this.userDetail.deaScheduleIV || this.userDetail.deaScheduleV) {
            if (IsStringNullUndefinedEmpty(this.userDetail.deaExpirationDate)) {
                isDEAExpDateRequired = true;
            }
            if (!(this.userDetail.deaScheduleII || this.userDetail.deaScheduleIII || this.userDetail.deaScheduleIV || this.userDetail.deaScheduleV)) {
                isDEAScheduleRequired = true;
            }
            if (IsStringNullUndefinedEmpty(this.userDetail.deaNumber)) {
                isDEARequired = true;
            }
        }

        this.isDEARequired = isDEARequired;
        this.isDEAExpDateRequired = isDEAExpDateRequired;
        this.isDEAScheduleRequired = isDEAScheduleRequired;
    }

    validateDEADetails() {
        this.setDeaValidation();
        return !(this.isDEARequired && this.isDEAExpDateRequired && this.isDEAScheduleRequired)
    }

    generateScretAnswer() {
        let secretAnsweList: SecretAnswers[] = [];
        if (!this.isLinkExistingShieldUser) {
            let secretAnswer: SecretAnswers = new SecretAnswers();
            let secretAnswer2: SecretAnswers = new SecretAnswers();
            let secretAnswer3: SecretAnswers = new SecretAnswers();
            secretAnswer.questionId = Number(this.userDetail.securityQuestion1);
            secretAnswer.answer = this.userDetail.securityAnswer1;
            secretAnsweList.push(secretAnswer);
            secretAnswer2.questionId = Number(this.userDetail.securityQuestion2);
            secretAnswer2.answer = this.userDetail.securityAnswer2;
            secretAnsweList.push(secretAnswer2);
            secretAnswer3.questionId = Number(this.userDetail.securityQuestion3);
            secretAnswer3.answer = this.userDetail.securityAnswer3;
            secretAnsweList.push(secretAnswer3);
        }
        return secretAnsweList;
    }

    setApiError(validateRegistrant: ValidateRegistrantModel) {
        this.isValidationFailed = true;
        if (!validateRegistrant.IsValidCaptcha) {
            this.captchaControl.ShowInValidCaptchaError = true;
            this.isValidNPI = true;
            this.showInvalidDeaError = false;
        }
        else if (!validateRegistrant.IsValid && validateRegistrant.IsMaxRetryFinished) {
            this.showModal = true;
        }
        else {
            this.captchaControl.ShowInValidCaptchaError = false;
            this.isValidNPI = validateRegistrant.IsValidNpi;
            this.userExisting.ShowInValidExistingShieldAccountError = !validateRegistrant.IsValidExistingShieldUser;
            this.showInvalidDeaError = false;
            if (validateRegistrant.IsDeaCheck && !validateRegistrant.IsValidDEA) {
                this.showInvalidDeaError = true;
            }
        }
    }

    validateShieldUserAccount() {
        if (this.isLinkExistingShieldUser) {
            this.getExistingAccountDetails();
            return this.isExistingAccountValid;
        }
        else {
            this.getUserAccountNameDetails();
            this.getPasswordDetails();
            this.getSecurityQuestionAnswerDetails();
            return (this.isPasswordValid && this.isUserNameValid && this.isSecurityQuestionAnswerValid);
        }
    }
  
    isUserCreationFormValid(data: any) {
        this.markAllControlAsTouched(data);
        this.getCaptchaDetails();
        let isDeaValid = this.validateDEADetails();
        let isShieldUserAccountValid = this.validateShieldUserAccount();
        let isFrmValid = data.valid
            && isDeaValid
            && isShieldUserAccountValid
            && this.isValidCaptcha;
        this.isValidationFailed = !isFrmValid;
        return isFrmValid;
    }

    markAllControlAsTouched(data: any) {
        Object.keys(data.controls).forEach(field => {
            const control = data.controls[field];
            control.markAsTouched({ onlySelf: true });
        })
    }

    onSubmit(data: any) {             
        if (this.isUserCreationFormValid(data)) {
            this.registrantuserdata.RegistrantUser = this.userDetail;
            this.registrantuserdata.SecretAnswers = this.generateScretAnswer();
            this.registrantuserdata.Captcha = this.userDetail.txtCapchaResponse;
            this.registrantuserdata.IsLinkExistingShieldUser = this.isLinkExistingShieldUser;

            this.userCreateService
                .saveRegistrationData(this.registrantuserdata)
                .subscribe((validateRegistrant: ValidateRegistrantModel) => {
                    if (validateRegistrant.IsValid) {
                        if (this.isLinkExistingShieldUser) {
                            this.router.navigate(["register/createlicense"]);
                        }
                        else {
                            this.router.navigate(["register/welcome"]);
                        }
                    }
                    else {
                        this.setApiError(validateRegistrant);
                    }
                });
        }
    }


    getCaptchaDetails() {
        let captchaModel: CaptchaModel;
        captchaModel = this.captchaControl.GetCaptchaDetails()
        this.userDetail.txtCapchaResponse = captchaModel.captchaText;
        this.isValidCaptcha = captchaModel.isValid;
    }

    getUserAccountNameDetails() {
        let userAccountName: UserAccountName;
        userAccountName = this.userNameChild.GetUserAccountNameDetails();
        this.userDetail.shieldUserName = userAccountName.shieldUserName;
        this.isUserNameValid = userAccountName.isValid;
    }

    getPasswordDetails() {
        let userPassword: UserPassword;
        userPassword = this.pwdChild.GetPasswordDetails();
        this.userDetail.password = userPassword.password;
        this.userDetail.confirmPassword = userPassword.confirmPassword;
        this.isPasswordValid = userPassword.isValid;
    }

    getSecurityQuestionAnswerDetails() {
        let securityQuestionAnswerModel: SecurityQuestionAnswerModel;
        securityQuestionAnswerModel = this.securityQuestion.GetSecurityQuestionAnswerDetails();
        this.userDetail.securityQuestion1 = securityQuestionAnswerModel.selectedShieldQuestionOne;
        this.userDetail.securityQuestion2 = securityQuestionAnswerModel.selectedShieldQuestionTwo;
        this.userDetail.securityQuestion3 = securityQuestionAnswerModel.selectedShieldQuestionThree;
        this.userDetail.securityAnswer1 = securityQuestionAnswerModel.securityAnswerOne;
        this.userDetail.securityAnswer2 = securityQuestionAnswerModel.securityAnswerTwo;
        this.userDetail.securityAnswer3 = securityQuestionAnswerModel.securityAnswerThree;
        this.isSecurityQuestionAnswerValid = securityQuestionAnswerModel.isValid;
    }

    getExistingAccountDetails() {
        let existingUserAccount: ExistingUserAccount;
        existingUserAccount = this.userExisting.GetExistingAccountDetails();
        this.userDetail.shieldUserName = existingUserAccount.shieldUserName;
        this.userDetail.password = existingUserAccount.password;
        this.isExistingAccountValid = existingUserAccount.isValid;
    }
    
   
}