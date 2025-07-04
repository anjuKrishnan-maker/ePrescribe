import { Component, OnInit, ViewChild, Input, Output, EventEmitter } from "@angular/core";
import { DEALicense, UserDeaLicensesModel, DEALicenseRequest, MessageModel, MessageIcon } from "../../../model/model.import.def";
import { DeaLicenseService } from "../../../services/service.import.def";
import { AddUserDeaLicenseComponent } from "./add-user-dea-license/add-user-dea-license.component";
import { UserMode } from "../../../model/user/user.model";


@Component({
    selector: 'erx-user-dea-license',
    templateUrl: './user-dea-license.template.html',
    styleUrls: ['./user-dea-license.style.css']
})
export class UserDeaLicenseComponent implements OnInit {
    @ViewChild(AddUserDeaLicenseComponent) addUserDeaLicenseComponent: AddUserDeaLicenseComponent;
    @Input() userDeaLicensesModel: UserDeaLicensesModel;
    @Output() hasDEALicense = new EventEmitter<boolean>();
    DEALicenseRequest = {} as DEALicenseRequest;
    DeaLicense: DEALicense;    
    originalList: DEALicense[];
    constructor(private deaLicenseService:DeaLicenseService) {

    }

    ngOnInit() {
        this.DEALicenseRequest.userID = this.userDeaLicensesModel.UserID;
        this.userDeaLicensesModel.DEALicenses = this.userDeaLicensesModel.DEALicenses == null ? [] : this.userDeaLicensesModel.DEALicenses;
    }

    private loadDeaLicenses() {
        this.deaLicenseService.GetDeaLicenseDetails(this.DEALicenseRequest).subscribe(response => {            
            this.userDeaLicensesModel.DEALicenses = response;                        
        })
    }       

    editDeaLicense(deaLicense: DEALicense) {
        if (!this.userDeaLicensesModel.IsRestrictedMode) {
            this.addUserDeaLicenseComponent.clearMessageStripArray();
            this.addUserDeaLicenseComponent.isEdit = this.addUserDeaLicenseComponent.showForm = true;
            deaLicense.DatePickerDate = new Date(deaLicense.DEAExpirationDate);
            this.addUserDeaLicenseComponent.originalDEALicense = JSON.parse(JSON.stringify(deaLicense));
            this.DeaLicense = JSON.parse(JSON.stringify(deaLicense));
            if (this.userDeaLicensesModel.UserMode == UserMode.AddOtherUser) {
                this.originalList = JSON.parse(JSON.stringify(this.userDeaLicensesModel.DEALicenses));
            }
        }
    }

    ConvertDEALicenseTypeIdToText(DEALicenseTypeId:number) {
        if (DEALicenseTypeId == 1 ) {
            return "Primary";
        }
        else if (DEALicenseTypeId == 2) {
            return "Additional";
        }
        else if (DEALicenseTypeId == 3) {
            return "NADEAN";
        }
    }

    statusHandler($event: boolean) {
        if ($event && this.userDeaLicensesModel.UserMode != UserMode.AddOtherUser) {
            this.loadDeaLicenses();
        }
        else {
            this.userDeaLicensesModel.DEALicenses = this.originalList;
        }
    }

    updateHandler($event: DEALicense) {
        this.updateToModel($event);
    }

    addDeaLicense() {
        if (!this.userDeaLicensesModel.IsRestrictedMode) {
            this.addUserDeaLicenseComponent.clearMessageStripArray();
            this.DeaLicense = {} as DEALicense;
            this.DeaLicense.DatePickerDate = new Date();
            this.DeaLicense.DEALicenseTypeId = 1; //This should be enum. Not doing this now as regression.
            this.addUserDeaLicenseComponent.showForm = true;
            this.addUserDeaLicenseComponent.isEdit = false;
        }
    }

    deleteDeaLicense(deaLicense: DEALicense) {
        if (!this.userDeaLicensesModel.IsRestrictedMode) {
            this.addUserDeaLicenseComponent.clearMessageStripArray();
            this.addUserDeaLicenseComponent.showForm = false;
            this.DEALicenseRequest.dEALicense = deaLicense;
            if (confirm("Are you sure you want to delete this DEA license?")) {
                if (this.userDeaLicensesModel.UserMode == UserMode.AddOtherUser) {
                    this.userDeaLicensesModel.DEALicenses.splice(this.userDeaLicensesModel.DEALicenses.indexOf(deaLicense), 1);
                    this.hasDEALicense.emit(this.userDeaLicensesModel.DEALicenses.length > 0);
                    this.addUserDeaLicenseComponent.MessageStripArray.push(new MessageModel("DEA License " + deaLicense.DEANumber + " successfully deleted.", MessageIcon.Success, null, false));
                }
                else {
                    this.deaLicenseService.DeleteDeaLicenseDetails(this.DEALicenseRequest).subscribe(response => {
                        if (response) {
                            this.addUserDeaLicenseComponent.MessageStripArray.push(new MessageModel(response.Message, response.Icon, null, false));
                            this.userDeaLicensesModel.DEALicenses.splice(this.userDeaLicensesModel.DEALicenses.indexOf(deaLicense), 1);
                            this.hasDEALicense.emit(this.userDeaLicensesModel.DEALicenses.length > 0);
                            this.loadDeaLicenses();
                        }
                    });
                }
            }
        }
    }    

    updateToModel(deaLicense: DEALicense) {
        let instance = this.userDeaLicensesModel.DEALicenses.findIndex((i) => { return i.DEANumber == this.addUserDeaLicenseComponent.originalDEALicense.DEANumber });
        if (instance > -1)
            this.userDeaLicensesModel.DEALicenses[instance] = JSON.parse(JSON.stringify(deaLicense));        
    }

    addedDEALicense($event: boolean) {
        this.hasDEALicense.emit($event);
    }
}