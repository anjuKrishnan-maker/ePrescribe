import { Component, ViewChild } from '@angular/core';
import { UserPassword } from '../user-password/user-password.model';


@Component({
    selector: 'app-user-password',
    templateUrl: './user-password.component.html',
    styleUrls: ['./user-password.component.css']
})
export class UserPasswordComponent  {
    
    @ViewChild('pwdForm') pwdForm: any;
    userPassWord: UserPassword = new UserPassword();

    constructor() {
            
    }

    makeDirty() {
        this.pwdForm.controls.password1.markAsDirty();
        this.pwdForm.controls.confirmPassword1.markAsDirty();
    }

    GetPasswordDetails() {
        this.makeDirty();
        this.userPassWord.isValid = this.pwdForm.valid;
        return this.userPassWord;
    }

    validatePassword() {
        let txtPasswordValue = this.userPassWord.password as string;
        if (!txtPasswordValue.PasswordValidator()) {
            this.pwdForm.controls["password1"].setErrors({
                "invalid": "Invalid Password"
            });
        } else {
            this.pwdForm.controls["password1"].setErrors(null);
        }
    }

    validateConfirmPassword() {
        let txtPasswordValue = this.userPassWord.password as string;
        let txtConfirmPasswordValue = this.userPassWord.confirmPassword as string;

        if (txtConfirmPasswordValue != txtPasswordValue) {
            this.pwdForm.controls["confirmPassword1"].setErrors({
                "pattern": "Passwords do not match"
            });
            return;
        }
        else {
            this.pwdForm.controls["confirmPassword1"].setErrors(null);
            return;
        }
    }
}