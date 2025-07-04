import { Component, OnInit, Input, ViewChild, Output, EventEmitter } from "@angular/core";
import { UserInformationModel } from "../../../model/user/user.model";
import { FormGroup, FormBuilder, NgForm } from "@angular/forms";

@Component({
    selector: 'erx-user-information',
    templateUrl: './user-information.template.html',
    styleUrls: ['./../user.style.css']   
})
export class UserInformationComponent implements OnInit {           
    @Input() userInformation: UserInformationModel;
    @Output() isUserInformationFormValid: EventEmitter<boolean> = new EventEmitter<boolean>();
    @ViewChild('userInformationForm') public userInformationForm: NgForm;
      
    public namePattern = "^([a-zA-Z]+[\\s-'.]{0,35})*";
    public confirmEmail: string;
    public inactive: boolean; 
    constructor() {

    }

    ngOnInit() {
        this.inactive = !this.userInformation.IsActive;
        this.confirmEmail = this.userInformation.WorkEmail;
        this.userInformationForm.statusChanges.subscribe(value => {
            if (this.userInformationForm.dirty === true) {
                if (value === 'VALID') {
                    this.isUserInformationFormValid.emit(true);
                }
                else {
                    this.isUserInformationFormValid.emit(false);
                }
            }
        });
    }    

    isActiveStatusChanged() {
        this.userInformation.IsActive = !this.inactive;
    }
}