import { Component, OnInit, ViewChild, Input } from '@angular/core';
import { ExistingUserAccount } from './user-existing-account.model';


@Component({
    selector: 'user-existing-account',
    templateUrl: './user-existing-account.component.html',
    styleUrls: ['./user-existing-account.component.css']

})
export class UserExistingAccountComponent implements OnInit {
    ShowInValidExistingShieldAccountError: boolean;
    userExistingAccount: ExistingUserAccount = new ExistingUserAccount();
    @ViewChild('userExisting') userExisting: any;
    constructor() {
    }
  
    ngOnInit() {
    }

    makeDirty() {
        this.userExisting.controls.existingShieldUserName.markAsDirty();
        this.userExisting.controls.existingShieldUserPassword.markAsDirty();
    }

    GetExistingAccountDetails(): ExistingUserAccount {
        this.makeDirty();
        this.userExistingAccount.isValid = this.userExisting.valid;
        return this.userExistingAccount;
    }
    onCredentialChanged() {
        this.ShowInValidExistingShieldAccountError = false;
    }

}