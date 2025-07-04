import { Component, ViewChild, OnInit, Input } from "@angular/core";
import { StateLicense, UserStateLicenseRequest, UserStateLicensesModel, MessageModel, MessageIcon } from '../../../model/model.import.def';
import { UserService } from '../../../services/service.import.def';
import { AddUserStateLicenseComponent } from '../user-state-license/add-user-state-license/add-user-state-license.component';
import { UserMode } from "../../../model/user/user.model";

@Component({
    selector: 'erx-user-state-license',
    templateUrl: './user-state-license.template.html',
    styleUrls: ['./user-state-license.style.css']
})
export class UserStateLicenseComponent implements OnInit {

    @ViewChild(AddUserStateLicenseComponent) addUserStateLicense: AddUserStateLicenseComponent;
    @Input() userStateLicensesModel: UserStateLicensesModel;
    userStateLicense: StateLicense;    
    userStateLicenseRequest = {} as UserStateLicenseRequest;
    originalList: StateLicense[];

    constructor(private userSvc: UserService) {
    }
    ngOnInit() {
        this.userStateLicenseRequest.userID = this.userStateLicensesModel.UserID;
        this.userStateLicensesModel.UserStateLicenses = this.userStateLicensesModel.UserStateLicenses == null ? [] : this.userStateLicensesModel.UserStateLicenses;                 
    }

    private loadStateLicenses() {                
        this.userSvc.getUserStateLicenses(this.userStateLicenseRequest).subscribe((response) => {
            this.userStateLicensesModel.UserStateLicenses = response;            
        });  
        
    }

    editLicense(license: StateLicense) {
        if (!this.userStateLicensesModel.IsRestrictedMode) {
            this.addUserStateLicense.clearMessageStripArray();
            this.userStateLicense = JSON.parse(JSON.stringify(license));
            this.addUserStateLicense.originalUserStateLicense = JSON.parse(JSON.stringify(license));
            this.userStateLicense.DatePickerDate = new Date(license.ExpiringDate);
            this.addUserStateLicense.showForm = true;
            this.addUserStateLicense.edit = true;
            if (this.userStateLicensesModel.UserMode == UserMode.AddOtherUser) {
                this.originalList = JSON.parse(JSON.stringify(this.userStateLicensesModel.UserStateLicenses));
            }
        }
    }

    deleteLicense(license: StateLicense) {
        if (!this.userStateLicensesModel.IsRestrictedMode) {
            this.addUserStateLicense.clearMessageStripArray();
            this.addUserStateLicense.showForm = false;
            this.userStateLicenseRequest.currentUserStateLicense = license;
            if (confirm("Are you sure you want to delete this state license?")) {
                if (this.userStateLicensesModel.UserMode == UserMode.AddOtherUser) {
                    this.addUserStateLicense.MessageStripArray.push(new MessageModel("State License " + license.LicenseNo + " successfully deleted.", MessageIcon.Success, null, false));
                    this.userStateLicensesModel.UserStateLicenses.splice(this.userStateLicensesModel.UserStateLicenses.indexOf(license), 1);
                }
                else {
                    this.userSvc.deleteUserStateLicense(this.userStateLicenseRequest).subscribe((response) => {
                        if (response) {
                            this.addUserStateLicense.MessageStripArray.push(new MessageModel(response.Message, response.Icon, null, false));
                            this.loadStateLicenses();
                        }
                    });
                }
            }
        }
    }     

    addLicense() {
        if (!this.userStateLicensesModel.IsRestrictedMode) {
            this.addUserStateLicense.clearMessageStripArray();
            this.userStateLicense = {} as StateLicense;
            this.userStateLicense.DatePickerDate = new Date();
            this.userStateLicense.State = this.userStateLicensesModel.PracticeState;
            this.userStateLicense.LicenseType = this.userStateLicensesModel.StatesAndLicenseTypes[1][0];
            this.addUserStateLicense.showForm = true;
            this.addUserStateLicense.edit = false;
        }
    }

    statusHandler($event: boolean) {
        if ($event && this.userStateLicensesModel.UserMode != UserMode.AddOtherUser) {
            this.loadStateLicenses();
        }
        else {
            this.userStateLicensesModel.UserStateLicenses = this.originalList;
        }
    }    

    updateHandler($event: StateLicense) {
        this.updateToModel($event);
    }

    updateToModel(stateLicense: StateLicense) {
        let instance = this.userStateLicensesModel.UserStateLicenses.findIndex((i) => { return i.LicenseNo == this.addUserStateLicense.originalUserStateLicense.LicenseNo });
        if (instance > -1)
            this.userStateLicensesModel.UserStateLicenses[instance] = JSON.parse(JSON.stringify(stateLicense));
    }
}