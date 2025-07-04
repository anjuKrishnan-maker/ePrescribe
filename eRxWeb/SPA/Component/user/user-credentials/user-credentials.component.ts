import { Component, OnInit, Input, EventEmitter, Output, ViewChild } from "@angular/core";
import { UserCredentialsModel } from "../../../model/user/user.model";
import { NgForm } from '@angular/forms';

@Component({
    selector: 'erx-user-credentials',
    templateUrl: './user-credentials.template.html',
    styleUrls: ['./../user.style.css']
})
export class UserCredentialsComponent implements OnInit {
    @Input() userCredentials: UserCredentialsModel;
    @Input() hasDEALicense: boolean;
    @Output() isUserCredentialsFormValid: EventEmitter<boolean> = new EventEmitter<boolean>();
    @ViewChild('userCredentialsForm') public userCredentialsForm: NgForm;
    suffixPattern = "^([a-zA-Z]+[\\s-'.]{0,6})*";

    constructor() {

    }
    ngOnInit() {
        this.userCredentialsForm.statusChanges.subscribe(value => {
            if (this.userCredentialsForm.dirty === true) {
                if (value === 'VALID') {
                    this.isUserCredentialsFormValid.emit(true);
                }
                else {
                    this.isUserCredentialsFormValid.emit(false);
                }
            }
        });
    }  
    
}