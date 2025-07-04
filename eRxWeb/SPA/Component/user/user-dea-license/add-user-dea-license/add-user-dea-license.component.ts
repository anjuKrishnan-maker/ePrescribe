import { Component, OnInit, Input, EventEmitter, Output, ViewChild } from "@angular/core";
import { DEALicense, MessageModel, DEALicenseRequest, MessageIcon, UserDeaLicensesModel, DeaLicenseType } from "../../../../model/model.import.def";
import { DeaLicenseService } from "../../../../services/service.import.def";
import { isNullOrWhiteSpace } from "../../../../tools/utils/null-whitespace-validator";
import { convertDateToString } from '../../../../tools/utils/date-validator';
import { UserMode } from "../../../../model/user/user.model";
import { NgForm } from '@angular/forms';


@Component({
    selector:'erx-add-user-dea-license',
    templateUrl: './add-user-dea-license.template.html',
    styleUrls:['./add-user-dea-license.style.css']
})
export class AddUserDeaLicenseComponent implements OnInit{
    showForm: boolean
    isEdit: boolean;
    @Input() DeaLicense: DEALicense;
    @Input() DEALicenseRequest: DEALicenseRequest;
    @Output() status = new EventEmitter<boolean>();
    @Output() updatedLicense = new EventEmitter<DEALicense>();
    @Output() addedLicense = new EventEmitter<boolean>();
    originalDEALicense: DEALicense;
    @Input() userDeaLicensesModel: UserDeaLicensesModel;
    MessageStripArray: Array<MessageModel> = [];
    @ViewChild('userDEALicenseForm') userDEALicenseForm: NgForm;

    constructor(private deaLicenseService: DeaLicenseService) {
    }

    ngOnInit() {
        this.DeaLicense = {} as DEALicense;
    }  

    handleChange(value) {
        if (value == 3) {
            this.DeaLicense.DEAIAllowed = true;
            this.DeaLicense.DEAIIAllowed = true;
            this.DeaLicense.DEAIIIAllowed = true;
            this.DeaLicense.DEAIVAllowed = true;
            this.DeaLicense.DEAVAllowed = true;
        }
    }
   
    insertDEALicense() {        
        this.clearMessageStripArray();
        if (this.validateDEALicense()) {            
            if (this.userDeaLicensesModel.UserMode == UserMode.AddOtherUser) {
                this.userDeaLicensesModel.DEALicenses.push(this.DeaLicense);
                this.addedLicense.emit(true);
                this.MessageStripArray.push(new MessageModel("DEA License " + this.DeaLicense.DEANumber + " successfully added.", MessageIcon.Success, null, false));
                this.showForm = false;
            }
            else { 
                this.DEALicenseRequest.dEALicense = this.DeaLicense
                this.deaLicenseService.InsertDeaLicenseDetails(this.DEALicenseRequest).subscribe(response => {
                    if (response) {
                        this.addedLicense.emit(true);
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

    updateDEALicense() {        
        this.clearMessageStripArray();
        if (this.validateDEALicense()) {
            if (this.userDeaLicensesModel.UserMode == UserMode.AddOtherUser) {
                this.updatedLicense.emit(this.DeaLicense);
                this.MessageStripArray.push(new MessageModel("DEA License " + this.DeaLicense.DEANumber + " successfully updated.", MessageIcon.Success, null, false));
                this.showForm = false;
            }
            else {
                this.DEALicenseRequest.dEALicense = this.DeaLicense;
                this.deaLicenseService.UpdateDeaLicenseDetails(this.DEALicenseRequest).subscribe(response => {
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

    cancel() {
        this.clearMessageStripArray();
        if (this.isEdit) {
            this.DeaLicense = this.originalDEALicense;            
            this.status.emit(false);
        }
        else {
            this.DeaLicense = {} as DEALicense;
        }
        if (this.userDeaLicensesModel.UserMode != UserMode.AddOtherUser) {
            this.status.emit(true);
        }
        this.showForm = false;
    }

    validateDEALicense(): boolean {        
        let maxDate = new Date("1/1/2100");
        let statusMessage: string = '';
        this.DeaLicense.DEAExpirationDate = convertDateToString(this.DeaLicense.DatePickerDate);
        let dEAValidationRegEx = new RegExp("^[a-zA-Z][a-zA-Z]\d*");
        if (this.DeaLicense.DEALicenseTypeId == DeaLicenseType.NADEAN) {
            dEAValidationRegEx = new RegExp("^[xX][a-zA-Z]\d*");
        }

        if (isNullOrWhiteSpace(this.DeaLicense.DEANumber)) {            
            statusMessage = "DEA License Number is required.";   
            this.MessageStripArray.push(new MessageModel(statusMessage, MessageIcon.Error, null, false));
            return false;            
        }
        else if (isNullOrWhiteSpace(this.DeaLicense.DEAExpirationDate)) {
            statusMessage = "DEA Expiration Date is required.";
            this.MessageStripArray.push(new MessageModel(statusMessage, MessageIcon.Error, null, false));
            return false;
        }
        else if (!dEAValidationRegEx.test(this.DeaLicense.DEANumber)) {
            statusMessage = "Invalid DEA. DEA number should begin with two letters followed by 7 numbers.";
            this.MessageStripArray.push(new MessageModel(statusMessage, MessageIcon.Error, null, false));
            return false;
        }
        else if (!this.sanityCheck(this.DeaLicense.DEANumber)) {
            statusMessage = "Invalid DEA. Please enter a valid DEA number.";
            this.MessageStripArray.push(new MessageModel(statusMessage, MessageIcon.Error, null, false));
            return false;
        }
        else if (this.isDuplicateDEALicense(this.DeaLicense.DEANumber)) { // (rawDea.Length > 9 && Provider.IsDuplicateInstitutionalDea(rawDea, deaLicenseID, new Guid(SessionLicenseID), DBID))            
            statusMessage = "A Similar DEA number already exists.";
            this.MessageStripArray.push(new MessageModel(statusMessage, MessageIcon.Error, null, false));
            return false;
        }
        else if (!this.DeaLicense.DEAIIAllowed && !this.DeaLicense.DEAIIIAllowed && !this.DeaLicense.DEAIVAllowed && !this.DeaLicense.DEAVAllowed) {            
            statusMessage = "DEA schedule is required. Please select at least one DEA schedule.";
            this.MessageStripArray.push(new MessageModel(statusMessage, MessageIcon.Error, null, false));
            return false;
        }
        else if (new Date(this.DeaLicense.DEAExpirationDate) <= new Date(Date.now()) || new Date(this.DeaLicense.DEAExpirationDate) > maxDate) {            
            statusMessage = "Invalid license expiration date. License expiration date must be greater than today.";
            this.MessageStripArray.push(new MessageModel(statusMessage, MessageIcon.Error, null, false));
            return false;
        }                   
        else if (this.userDEALicenseForm.invalid) return false;
        return true;
    }    

    sanityCheck(dEANumber: string): boolean {
        if (dEANumber)
            return true;
        if (dEANumber.length < 9)
            return false;

        if (dEANumber.length > 9 && dEANumber.substring(9, 10) != "-")
            return false;

        if (dEANumber.length > 25)
            return false;

        let dea: string = dEANumber.substring(0, 9); // Actual DEA number

        if (dea.substring(2,9) == "0000000")
            return false;

        if (isNaN(Number(dea.substring(2,9))))
            return false;

        // example#    BT2361234
        // 1.   add the 1st, 3rd and 5th digit--- 2 + 6 + 2 =  10
        // 2.   add the 2nd, 4th and 6th digit and multiply sum by 2 --- 3 + 1 + 3 = 7 x 2 = 14
        // 3.   add two sums-- 10 + 14 =  24
        // 4.   last digit of that sum must match last digit of DEA# to be a valid DEA# -- in this case both are 4   (BT2361234 ,   24)

        let sum1 = 0;
        let sum2 = 0;
        let checkSum = 0;

        let digits = dea.substring(2, 9).split('').map(Number); 

        // 1st, 3rd, 5th digits
        sum1 = digits[0] + digits[2] + digits[4];

        // 2nd, 4th, 6th digits
        sum2 = digits[1] + digits[3] + digits[5];

        checkSum = sum1 + (sum2 * 2);

        let checkDigit = checkSum.toString().substring(checkSum.toString().length - 1);
        let deaLastDigit = dea.substring(dea.length - 1);

        return checkDigit == deaLastDigit;
    }


    isDuplicateDEALicense(dEANumber: string)
    {
        let reference: DEALicense[] = JSON.parse(JSON.stringify(this.userDeaLicensesModel.DEALicenses));
        if (this.isEdit) {
            let instance = reference.findIndex((i) => { return i.DEANumber == dEANumber });
            if (instance > -1)
                reference.splice(instance, 1);
        }
        if (reference.length > 0) {
            for (let i = 0; i < reference.length; i++) {
                if (reference[i].DEANumber == dEANumber)
                    return true;
            }                             
        }        
        return false;
    }    
}