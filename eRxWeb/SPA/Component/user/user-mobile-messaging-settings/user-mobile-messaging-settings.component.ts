import { Component, OnInit, Input, ViewChild } from "@angular/core";
import { UserMobileMessageSettingsModel, UserCategory } from "../../../model/user/user.model";
import { UserMobileRegistrationComponent } from "./user-mobile-registration/user-mobile-registration.component";

@Component({
    selector: 'erx-user-mobile-messaging-settings',
    templateUrl: './user-mobile-messaging-settings.template.html',
    styleUrls: ['./../user.style.css']
})
export class UserMobileMessagingSettingsComponent implements OnInit {
    @Input() userMobileMessageSettings: UserMobileMessageSettingsModel;
    @ViewChild(UserMobileRegistrationComponent) UserMobileRegistration: UserMobileRegistrationComponent;
    constructor() {

    }
    ngOnInit() {

    }

    updateParentCheckbox() {
        if (this.userMobileMessageSettings.SmsAlertNewRxError || this.userMobileMessageSettings.SmsAlertRefillRxError ||
            this.userMobileMessageSettings.SmsAlertChangeRxError || this.userMobileMessageSettings.SmsAlertCancelRxError) {
            this.userMobileMessageSettings.SmsAlertTransmissionError = true;
        } else {
            this.userMobileMessageSettings.SmsAlertTransmissionError = false;
        }
        this.isChanged();
    }

    updateChildCheckbox() {
        if (this.userMobileMessageSettings.SmsAlertTransmissionError === true) {
            this.setChildCheckBox(true);
        }
        else {
            this.setChildCheckBox(false);
        }
        this.isChanged();
    }

    setChildCheckBox(value: boolean) {
        this.userMobileMessageSettings.SmsAlertNewRxError = value;
        this.userMobileMessageSettings.SmsAlertRefillRxError = value;
        this.userMobileMessageSettings.SmsAlertChangeRxError = value;
        this.userMobileMessageSettings.SmsAlertCancelRxError = value;
    }

    isChanged() {
        this.userMobileMessageSettings.IsChanged = true;
    }

    registerMobile() {
        this.UserMobileRegistration.showS1Popup();
    }

    editMobile() {
        this.UserMobileRegistration.showS1Popup();
    }

    deleteMobile() {
        this.UserMobileRegistration.showDelete();
    }

    checkMobileAlertCheckboxes(check: boolean) {
        this.userMobileMessageSettings.SmsAlertTransmissionError = check;
        this.userMobileMessageSettings.SmsAlertNewRxError = check;
        this.userMobileMessageSettings.SmsAlertNewRxError = check;
        this.userMobileMessageSettings.SmsAlertRefillRxError = check;
        this.userMobileMessageSettings.SmsAlertChangeRxError = check;
        this.userMobileMessageSettings.SmsAlertCancelRxError = check;
        this.userMobileMessageSettings.SmsAlertChangeRxRequest = check;
        this.userMobileMessageSettings.SmsAlertDEALicenseExpiration = check;
        this.userMobileMessageSettings.SmsAlertServiceUpdate = check;
    }

    updateHandler($event: UserMobileMessageSettingsModel) {
        this.userMobileMessageSettings.PhoneNumber = $event.PhoneNumber;
        this.userMobileMessageSettings.IsNumberAvailable = $event.IsNumberAvailable;
        this.userMobileMessageSettings.IsChanged = $event.IsChanged;

        if (this.isUserPrescriber()) {
            this.checkMobileAlertCheckboxes($event.IsNumberAvailable);
        }
        else {
            this.userMobileMessageSettings.SmsAlertServiceUpdate = $event.IsNumberAvailable;
        }        
    }

    isUserPrescriber(): boolean{
        let userType = this.userMobileMessageSettings.UserType;
        return userType === UserCategory.PHYSICIAN_ASSISTANT ||
            userType === UserCategory.PHYSICIAN_ASSISTANT_SUPERVISED ||
            userType === UserCategory.PROVIDER;
    }
}