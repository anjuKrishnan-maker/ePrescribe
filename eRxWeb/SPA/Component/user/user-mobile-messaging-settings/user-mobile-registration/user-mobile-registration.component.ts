import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core'
import { MobileRegistrationService } from '../../../../services/service.import.def';
import { VerificationCodeSMSModel, UserMobileMessageSettingsModel, UserCategory, SMSAlertUserActionType } from '../../../../model/user/user.model';
import { NgForm } from '@angular/forms';


@Component({
    selector: 'erx-user-mobile-registration',
    templateUrl: './user-mobile-registration.template.html',
    styleUrls: ['./user-mobile-registration.style.css']
})

export class UserMobileRegistrationComponent implements OnInit {
    @Input() userMobileMessageSettings: UserMobileMessageSettingsModel;    
    termsAndConditionsCheck: boolean;
    showS1ModalPopup: boolean = false;
    showS2ModalPopup: boolean = false;
    showSMSTempDownPopup: boolean = false;
    showDeletePopup: boolean = false;
    verificationCodeSMS = {} as VerificationCodeSMSModel;
    resending: boolean;
    enteredOTP: string = '';
    @Output() newUserMobileMessageSettings = new EventEmitter<UserMobileMessageSettingsModel>();
    s2Message: string;
    otpErrorMessage: string;

    constructor(private mobileRegSvc: MobileRegistrationService) { }

    ngOnInit() {
        this.verificationCodeSMS.userID = this.userMobileMessageSettings.UserID;        
    }    

    /* Step 1 Popup methods */
    showS1Popup() {
        if (this.userMobileMessageSettings.IsNumberAvailable) {
            this.verificationCodeSMS.phoneNumber = this.userMobileMessageSettings.PhoneNumber;
        }
        else {
            this.verificationCodeSMS.phoneNumber = '';
        }

        if (this.userMobileMessageSettings.IsSMSAlertDailyCountReached) {
            this.verificationCodeSMS.status = SMSAlertUserActionType.SMSLimitExceeded;            
            this.mobileRegSvc.MobileMessageSettingAuditlog(this.verificationCodeSMS).subscribe(() => {
                this.showSMSTempDownPopup = true;
            });            
        }
        else {            
            this.showS1ModalPopup = true;
        }
        this.termsAndConditionsCheck = false;
    }
    closeSMSTempDown() {
        this.showSMSTempDownPopup = false;
    }
    closeS1Popup() {
        this.showS1ModalPopup = false;
    }

    private generateRandom() {
        let num = Math.floor(Math.random() * 10000);
        if (num < 1000)
            num += 1000;
        return num;
    }

    registerMobileS1Continue(s1: NgForm) {
        this.verificationCodeSMS.otp = this.generateRandom();

        this.verificationCodeSMS.message = "Welcome to ePrescribe SMS Alerts. Your verification code is: " + this.verificationCodeSMS.otp + ". Reply STOP to opt out of future messages.";       
        this.mobileRegSvc.SendMessageTwilio(this.verificationCodeSMS).subscribe(response => {
            if (response.successField) {
                this.closeS1Popup();
                this.showS2Popup();
                this.s2Message = "We sent a verification code to " + this.verificationCodeSMS.phoneNumber + ". Please check your mobile device for a new text message received within the next 5 minutes and enter the code into the field below."
            }
        });
        this.verificationCodeSMS.status = SMSAlertUserActionType.AcceptTerms;
        this.mobileRegSvc.MobileMessageSettingAuditlog(this.verificationCodeSMS).subscribe(() => { });
        this.termsAndConditionsCheck = false;
    }

    registerMobileS1Cancel() {
        this.verificationCodeSMS.phoneNumber = '';
        this.closeS1Popup();
    }

    /* Step 2 Popup methods*/
    showS2Popup() {
        this.enteredOTP = '';
        this.otpErrorMessage = '';
        this.showS2ModalPopup = true;
    }

    closeS2Popup() {
        this.showS2ModalPopup = false;
    }

    registerMobileS2Cancel() {
        this.verificationCodeSMS.phoneNumber = "";
        this.closeS2Popup();
    }

    registerMobileResendCode() {
        this.otpErrorMessage = "";
        this.enteredOTP = "";
        this.resending = true;
        this.mobileRegSvc.SendMessageTwilio(this.verificationCodeSMS).subscribe(response => {
            if (response.successField) {
                this.s2Message = "Verification code was resent to " + this.verificationCodeSMS.phoneNumber+". Please allow up to 5 minutes to receive the message on your mobile device.Enter the code into the field below."
                this.resending = false;
            }
        });
    }

    validateOTP() {
        if (this.enteredOTP == this.verificationCodeSMS.otp.toString())
            return true;
        else
            return false;
            }

    registerMobileVerify() { 
        this.otpErrorMessage = "";
        if (this.validateOTP()) {            
            this.mobileRegSvc.SaveUserMobileNumber(this.verificationCodeSMS).subscribe(isInserted => {
                if (isInserted) {                    
                    
                    this.mobileRegSvc.GetUserMobileMessageSettings(this.verificationCodeSMS).subscribe((response) => {                        
                        this.userMobileMessageSettings = response;
                        this.userMobileMessageSettings.IsChanged = true;
                        this.newUserMobileMessageSettings.emit(this.userMobileMessageSettings);
                    });

                    
                    this.verificationCodeSMS.status = SMSAlertUserActionType.CompleteRegistration;
                    this.mobileRegSvc.MobileMessageSettingAuditlog(this.verificationCodeSMS).subscribe(() => { });
                }
                else {
                    this.verificationCodeSMS.status = SMSAlertUserActionType.UpdateRegistration;
                    this.mobileRegSvc.MobileMessageSettingAuditlog(this.verificationCodeSMS).subscribe(() => { });
                    this.userMobileMessageSettings.PhoneNumber = this.verificationCodeSMS.phoneNumber;
                    this.userMobileMessageSettings.IsNumberAvailable = true;
                }
            });
            this.closeS2Popup();
        }
        else {
            this.otpErrorMessage = "Code is incorrect.Try again";
        }
    }
    

    /* Delete Popup method */
    showDelete() { this.showDeletePopup = true; }

    cancelDelete() { this.showDeletePopup = false; }

    continueDelete() {        
        this.mobileRegSvc.DeleteUserMobileNumber(this.verificationCodeSMS).subscribe((response) => {
            if (response) {
                this.userMobileMessageSettings.PhoneNumber = '';
                this.userMobileMessageSettings.IsNumberAvailable = false;
                this.userMobileMessageSettings.IsChanged = true;
                this.newUserMobileMessageSettings.emit(this.userMobileMessageSettings);
                this.showDeletePopup = false;                
            }
        });
        this.verificationCodeSMS.status = SMSAlertUserActionType.DeleteRegistration;
        this.mobileRegSvc.MobileMessageSettingAuditlog(this.verificationCodeSMS).subscribe(() => { });        
    }

    /*mobile number field methods*/

    numericKeyPressOnly(e: KeyboardEvent) {
        var keyCode = window.event ? e.keyCode : e.which;

        if (keyCode != null) {
            //Numeric, tab, backspace are valid key strokes
            if (keyCode == 0 ||
                keyCode == 8 ||
                keyCode == 9 ||
                (keyCode >= 48 && keyCode <= 57)) {
                return true;
            }
        }
        return false;
    }

    parseNumberInput() {
        if (this.verificationCodeSMS.phoneNumber.length > 0) {
            this.verificationCodeSMS.phoneNumber = this.verificationCodeSMS.phoneNumber.replace(/[^0-9]/g, "");
        }
    }

    formatPhoneInput() {
        //take the input value first trim off all the non numeric characters
        this.verificationCodeSMS.phoneNumber = this.verificationCodeSMS.phoneNumber.replace(/[^0-9]/g, "");
        if (this.verificationCodeSMS.phoneNumber.length >= 10) {
            var formattedPhone = this.verificationCodeSMS.phoneNumber.substring(0, 3) + "-" + this.verificationCodeSMS.phoneNumber.substring(3, 6) + "-" + this.verificationCodeSMS.phoneNumber.substring(6, 10);
            this.verificationCodeSMS.phoneNumber = formattedPhone;
        }
    }
}