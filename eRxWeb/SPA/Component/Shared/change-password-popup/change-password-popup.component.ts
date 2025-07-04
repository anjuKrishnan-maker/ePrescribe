import { Component, OnInit, ViewChild } from '@angular/core'
import { NgForm } from '@angular/forms';
import { ChangePasswordService, ComponentCommonService, SpinnerService } from '../../../services/service.import.def';
import { ChangePasswordModel } from '../../../model/model.import.def';


@Component({
    selector: 'erx-change-password-popup',
    templateUrl: './change-password-popup.template.html',
    styleUrls: ['./change-password-popup.style.css'],
    providers: [ChangePasswordService]
})

export class ChangePasswordPopupComponent implements OnInit {
    @ViewChild('changePasswordForm') changePasswordForm: NgForm;
    changePwd: ChangePasswordModel;
    isValid: boolean = false;
    submitted: boolean= false;
    showModalPopup: boolean = false;
    displayError: boolean = true;
    constructor(private changePwdSvc: ChangePasswordService, private compSvc: ComponentCommonService,private spinnerSvc: SpinnerService) {
        this.compSvc.AddWindowFunction('ChangePasswordPopup', () => {
            this.spinnerSvc.hidePagespinner();
            this.ShowModal();
        })
    }

    ngOnInit() {
        this.changePwd = new ChangePasswordModel();
        this.changePwd.Status = '';
    }

    ShowModal() {
        this.showModalPopup = true;
    }

    validate(changePasswordForm: NgForm) {
        Object.keys(changePasswordForm.controls).forEach(field => {
            const control = changePasswordForm.controls[field];
            control.markAsTouched({ onlySelf: true });
        })

        if (!changePasswordForm.valid) {
            return false;
        }
        if (this.changePwd.OldPassword == this.changePwd.NewPassword) {
            return false;
        }
        if (!(this.changePwd.NewPassword == this.changePwd.ConfirmPassword)) {
            return false;
        }
        
        return true;
    }

    cancel(d: NgForm) {
        d.resetForm();
        this.changePwd.Status = '';
        this.showModalPopup = false;
    }

    submit(changePasswordForm: NgForm) {
        event.stopPropagation();
        this.changePwd.Status = ' ';
        this.submitted = true;
        let validForm = this.validate(changePasswordForm);
        if (validForm)
        {
            this.changePwdSvc.ChangePassword(this.changePwd.OldPassword, this.changePwd.NewPassword).subscribe((response) => {
                this.changePwd = new ChangePasswordModel();
                if (!this.showModalPopup)this.showModalPopup = true;
                this.submitted = false;
                this.changePwd.Success = response.Success;
                this.changePwd.Status = response.Status;
                changePasswordForm.resetForm();
            });
        }
        else
        {
            this.submitted = false; 
        }    
    }
}
