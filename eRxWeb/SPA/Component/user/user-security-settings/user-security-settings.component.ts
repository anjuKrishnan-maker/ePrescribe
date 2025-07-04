import { Component, OnInit, Input, ViewChild } from "@angular/core";
import { UserSecurityModel } from "../../../model/user/user.model";
import { EditSecretQuestionsModalPopupComponent } from "./edit-secret-questions-modal-popup/edit-secret-questions-modal-popup.component";
import { ChangePasswordPopupComponent } from "../../shared/change-password-popup/change-password-popup.component";

@Component({
    selector: 'erx-user-security-settings',
    templateUrl: './user-security-settings.template.html',
    styleUrls: ['./user-security-settings.style.css']
})
export class UserSecuritySettingsComponent implements OnInit {
    @Input() userSecurity: UserSecurityModel;
    @ViewChild(ChangePasswordPopupComponent) changePasswordPopup: ChangePasswordPopupComponent;
    @ViewChild(EditSecretQuestionsModalPopupComponent) editSecretQuestions: EditSecretQuestionsModalPopupComponent;

    constructor() {

    }
    ngOnInit() {

    }

    openChangePasswordPopup() {
        this.changePasswordPopup.ShowModal();
    }

    openEditSecretQuestionsPopup() {
        this.editSecretQuestions.showModal();
    }
}