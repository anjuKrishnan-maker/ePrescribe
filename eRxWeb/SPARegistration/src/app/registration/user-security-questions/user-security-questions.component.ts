/// <reference path="user-security-questions.model.ts" />
/// <reference path="user-security-questions.model.ts" />
import { Component, OnInit, Inject, Output,EventEmitter, ViewChild } from '@angular/core';
import { UserCreateService } from '../../service/user-create.service';
import { SecurityQuestionAnswerModel, ShieldSecretQuestion } from './user-security-questions.model';

@Component({
    selector: 'app-user-security-questions',
    templateUrl: './user-security-questions.component.html',
    styleUrls: ['./user-security-questions.component.css']
})
export class UserSecurityQuestionsComponent implements OnInit {
    securityQuestionAnswerModel: SecurityQuestionAnswerModel = new SecurityQuestionAnswerModel()
    ShieldQuestions: ShieldSecretQuestion[];
    @ViewChild('securityQuestionForm') securityQuestionForm: any;

    makeDirty() {
        this.securityQuestionForm.controls.ansOne.markAsDirty();
        this.securityQuestionForm.controls.ansTwo.markAsDirty();
        this.securityQuestionForm.controls.ansThree.markAsDirty();
    }

    validateSecurityQuestions() {
        if (this.securityQuestionAnswerModel.selectedShieldQuestionOne == this.securityQuestionAnswerModel.selectedShieldQuestionTwo
            || this.securityQuestionAnswerModel.selectedShieldQuestionOne == this.securityQuestionAnswerModel.selectedShieldQuestionThree
            || this.securityQuestionAnswerModel.selectedShieldQuestionTwo == this.securityQuestionAnswerModel.selectedShieldQuestionThree) {
            return false;
        }
        else {
            return true;
        }
    }

    validateSecurityAnswers() {
        if (this.securityQuestionAnswerModel.securityAnswerOne == this.securityQuestionAnswerModel.securityAnswerTwo
            || this.securityQuestionAnswerModel.securityAnswerOne == this.securityQuestionAnswerModel.securityAnswerThree
            || this.securityQuestionAnswerModel.securityAnswerTwo == this.securityQuestionAnswerModel.securityAnswerThree) {
            return false;
        }
        else {
            return true;
        }
    }

    GetSecurityQuestionAnswerDetails() {
        this.makeDirty();
        this.securityQuestionAnswerModel.isValid = this.securityQuestionForm.valid
            && this.validateSecurityQuestions()
            && this.validateSecurityAnswers();
        return this.securityQuestionAnswerModel;
    }

    constructor(private userCreateService: UserCreateService) {
    }
    ngOnInit() {
        this.getInitialPageData();
    }

    getInitialPageData() {
        this.userCreateService.getSecurityQuestions().subscribe(result => {
            this.setShieldQuestionsDefaultValues(result.SecurityQuestions);
        });
    }

    setShieldQuestionsDefaultValues(securityquestions: any) {
        if (securityquestions) {
            this.ShieldQuestions = securityquestions as ShieldSecretQuestion[];
            this.securityQuestionAnswerModel.selectedShieldQuestionOne = this.ShieldQuestions.find(x => x.questionIDField == 1).questionIDField;
            this.securityQuestionAnswerModel.selectedShieldQuestionTwo = this.ShieldQuestions.find(x => x.questionIDField == 2).questionIDField;
            this.securityQuestionAnswerModel.selectedShieldQuestionThree = this.ShieldQuestions.find(x => x.questionIDField == 3).questionIDField;
        }
    }

}