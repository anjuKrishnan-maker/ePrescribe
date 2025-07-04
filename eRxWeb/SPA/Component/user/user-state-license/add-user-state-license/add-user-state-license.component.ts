import { Component, OnInit, Input, Output, EventEmitter, ViewChild } from "@angular/core";
import { UserService } from '../../../../services/service.import.def';
import { StateLicense, UserStateLicenseRequest, UserStateLicensesModel, MessageModel, MessageIcon } from '../../../../model/model.import.def';
import { isNullOrWhiteSpace } from '../../../../tools/utils/null-whitespace-validator';
import { convertDateToString } from '../../../../tools/utils/date-validator';
import { UserCategory, UserMode } from "../../../../model/user/user.model";
import { LicenseConstants } from "../../../../tools/constants";
import { NgForm } from '@angular/forms';



@Component({
    selector: 'erx-add-user-state-license',
    templateUrl: './add-user-state-license.template.html',
    styleUrls: ['./add-user-state-license.style.css']
})
export class AddUserStateLicenseComponent implements OnInit {
    states: string[]
    licensetypes: string[]
    edit: boolean;
    @Input() userStateLicense: StateLicense;
    showForm: boolean;    
    @Output() status = new EventEmitter<boolean>();
    @Output() updatedLicense = new EventEmitter<StateLicense>();
    originalUserStateLicense: StateLicense;
    today = new Date().toLocaleDateString();
    @Input() userStateLicenseRequest: UserStateLicenseRequest;
    @Input() userStateLicensesModel: UserStateLicensesModel;
    MessageStripArray: Array<MessageModel> = [];
    @ViewChild('userStateLicenseForm') userStateLicenseForm: NgForm;

    constructor(private userSvc: UserService) {
        
    }

    ngOnInit() {
        this.loadStateAndLicenseTypes();
        this.originalUserStateLicense = {} as StateLicense;
    }        

    private loadStateAndLicenseTypes() {
        this.states = this.userStateLicensesModel.StatesAndLicenseTypes[0];
        this.licensetypes = this.userStateLicensesModel.StatesAndLicenseTypes[1];     
    }

    update() {
        this.clearMessageStripArray();
        this.userStateLicenseRequest.oldUserStateLicense = this.originalUserStateLicense;
        this.userStateLicenseRequest.currentUserStateLicense = this.userStateLicense;
        if (this.validateLicense()) {    
            if (this.userStateLicensesModel.UserMode == UserMode.AddOtherUser) {
                this.updatedLicense.emit(this.userStateLicense);
                this.MessageStripArray.push(new MessageModel("State License " + this.userStateLicense.LicenseNo + " successfully updated.", MessageIcon.Success, null, false));            
                this.showForm = false;
            }
            else {                
                this.userSvc.updateUserStateLicense(this.userStateLicenseRequest).subscribe((response) => {
                    if (response) {
                        this.MessageStripArray.push(new MessageModel(response.Message, response.Icon, null, response.ShowCloseButton));
                        if (response.Icon == MessageIcon.Success) {
                            this.status.emit(true);
                            this.showForm = false;
                        }
                    }
                });
            } 
        }
    }

    insert() {
        this.clearMessageStripArray();
        if (this.validateLicense()) {            
            if (this.userStateLicensesModel.UserMode == UserMode.AddOtherUser) {
                this.userStateLicensesModel.UserStateLicenses.push(this.userStateLicense);
                this.MessageStripArray.push(new MessageModel("State License " + this.userStateLicense.LicenseNo + " successfully added.", MessageIcon.Success, null, false));
                this.showForm = false;
            }
            else {
                this.userStateLicenseRequest.currentUserStateLicense = this.userStateLicense;
                this.userSvc.addUserStateLicense(this.userStateLicenseRequest).subscribe((response) => {
                    if (response) {
                        this.MessageStripArray.push(new MessageModel(response.Message, response.Icon, null, response.ShowCloseButton));
                        if (response.Icon == MessageIcon.Success) {
                            this.status.emit(true);
                            this.showForm = false;
                        }
                    }
                });
            }
        }
    }

    clearMessageStripArray() {
        while (this.MessageStripArray.length > 0) { this.MessageStripArray.pop(); }
    }

    cancel() {
        this.clearMessageStripArray();
        if (this.edit) {
            this.userStateLicense = this.originalUserStateLicense;
            this.status.emit(false);
        }
        else {
            this.userStateLicense = {} as StateLicense;
        }
        this.showForm = false;
        if (this.userStateLicensesModel.UserMode != UserMode.AddOtherUser) {
            this.status.emit(true);
        }
    }    

    validateLicense(): boolean {        
        let message: string = '';
        let maxDate = new Date("1/1/2100");
        this.userStateLicense.ExpiringDate = convertDateToString(this.userStateLicense.DatePickerDate);

        if (isNullOrWhiteSpace(this.userStateLicense.State) || isNullOrWhiteSpace(this.userStateLicense.LicenseNo) || isNullOrWhiteSpace(this.userStateLicense.ExpiringDate) || isNullOrWhiteSpace(this.userStateLicense.LicenseType)){            
            if (isNullOrWhiteSpace(this.userStateLicense.State) || (this.userStateLicense.State == "")) {                
                message = 'State is required.';
                this.MessageStripArray.push(new MessageModel(message, MessageIcon.Error, null, false));
                return false;
            }

            if (isNullOrWhiteSpace(this.userStateLicense.LicenseNo) || (this.userStateLicense.LicenseNo == "")) {
                message = 'License Number is required.';
                this.MessageStripArray.push(new MessageModel(message, MessageIcon.Error, null, false));
                return false;
            }

            if (isNullOrWhiteSpace(this.userStateLicense.LicenseType) || (this.userStateLicense.LicenseType == "")) {
                message = 'License Type is required.';
                this.MessageStripArray.push(new MessageModel(message, MessageIcon.Error, null, false));
                return false;
            }

            if (isNullOrWhiteSpace(this.userStateLicense.ExpiringDate)) {
                message = 'Expiring Date is required.';
                this.MessageStripArray.push(new MessageModel(message, MessageIcon.Error, null, false));
                return false;
            }
        }
        else {
            if (!this.isValidLicenseNumber(this.userStateLicense)) {
                message = "Please enter a valid state license without special characters.";
                this.MessageStripArray.push(new MessageModel(message, MessageIcon.Error, null, false));
                return false;
            }
            if (this.isDuplicateLicense(this.userStateLicense)) {
                message = "A state license for " + this.userStateLicense.State + " of type " +
                    this.userStateLicense.LicenseType + " already exists. Only one state license can be entered per state.";
                this.MessageStripArray.push(new MessageModel(message, MessageIcon.Error, null, false));
                return false;
            }
            if ((this.userStateLicense.State == LicenseConstants.OhioStateCode)
                && (this.userStateLicense.LicenseNo.indexOf(LicenseConstants.CertificateToPrescribeCode) == -1)
                && (this.userStateLicensesModel.UserType == UserCategory.PHYSICIAN_ASSISTANT_SUPERVISED || this.userStateLicensesModel.UserType == UserCategory.PHYSICIAN_ASSISTANT)
                ) {
                message = "In the state of OHIO, mid-level prescribers must provide a CTP code in the state license field prior to prescribing.  If you have and would like to include your OHIO state license number, type that first followed by ' CTP ' then your CTP code.";
                this.MessageStripArray.push(new MessageModel(message, MessageIcon.Error, null, false));
                return false;
            }
            if (new Date(this.userStateLicense.ExpiringDate) < new Date(Date.now()) || new Date(this.userStateLicense.ExpiringDate) > maxDate) {
                message = "Invalid license expiration date. License expiration date must be greater than today.";
                this.MessageStripArray.push(new MessageModel(message, MessageIcon.Error, null, false));
                return false;
            }           
        }
        return true;
    }        

    private isDuplicateLicense(license: StateLicense): boolean {       

        let reference = JSON.parse(JSON.stringify(this.userStateLicensesModel.UserStateLicenses));
        if (this.edit) {
            let instance = reference.findIndex((i) => { return (i.LicenseNo == this.originalUserStateLicense.LicenseNo); });
            if (instance > -1) {
                reference.splice(instance, 1);
            }
        }

        if (reference.length > 0) {
            for (let i = 0; i < reference.length; i++) {
                if ((reference[i].State == license.State) && (reference[i].LicenseType == license.LicenseType))
                    return true;
            }            
        }
        return false;
    }

    private isValidLicenseNumber(license: StateLicense): boolean {
        let regexp = new RegExp("^[A-Za-z0-9\s]{0,100}$");
        return regexp.test(license.LicenseNo);
    }
}