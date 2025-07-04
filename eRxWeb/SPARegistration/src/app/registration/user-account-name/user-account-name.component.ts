import { Component, OnInit, Inject, Input, EventEmitter, Output, ViewChild } from '@angular/core';
import { } from '../user-creation/user-creation.model';
import { UserCreateService } from '../../service/user-create.service';
import { UserAccountName } from './user-account-name.model';


@Component({
    selector: 'app-user-account',
    templateUrl: './user-account-name.component.html',
    styleUrls: ['./user-account-name.component.css']

})
export class UserAccountNameComponent implements OnInit {
    shieldUserNamePattern = "^([a-zA-Z0-9]+[-,_.]{0,30})*";
    isUserExists: boolean = true;
    userAccountName: UserAccountName = new UserAccountName();
    
    @ViewChild('userAccForm') userAccForm: any;
    
    constructor(private userCreateService: UserCreateService) {
    }

    ngOnInit() {
    }

    @Input() userExistsErrorMessage: string;
    @Input() userNameLabel: string;
   
    validateUserName() {
        this.isUserExists = true;
        //Initialize variables
        let sldUserName: string = this.userAccountName.shieldUserName;
        if (sldUserName && !this.userAccForm.controls["newShieldUserName"].errors) {
            this.userCreateService.validateShieldUserName(sldUserName)
                .subscribe((userExists: boolean) => {
                    if (userExists) {
                        this.isUserExists = true;
                        this.userAccForm.controls["newShieldUserName"].setErrors({
                            "uniqueShieldUserName": "Invalid Username"
                        });
                    }
                    else {
                        this.isUserExists = false;
                        this.userAccForm.controls["newShieldUserName"].setErrors(null);
                    }
                });
        }
    }

    GetUserAccountNameDetails(): UserAccountName {
        this.userAccForm.controls.newShieldUserName.markAsDirty();
        this.userAccountName.isValid = this.userAccForm.valid;
        return this.userAccountName;
    }
   
}