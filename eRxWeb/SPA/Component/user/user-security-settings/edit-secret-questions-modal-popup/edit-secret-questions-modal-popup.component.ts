import { Component, OnInit, Input, ViewChild } from "@angular/core";
import { NgForm } from "@angular/forms";
import { UserService } from "../../../../services/user.service";
import { SecretQuestion, SecretQuestionAnswer, SecretQuestionAnswerRequest } from "../../../../model/secret-question-answer.model";

@Component({
    selector: 'erx-edit-secret-questions-modal-popup',
    templateUrl: './edit-secret-questions-modal-popup.template.html',
    styleUrls: ['./edit-secret-questions-modal-popup.style.css']
})

export class EditSecretQuestionsModalPopupComponent implements OnInit {
    @ViewChild('f') f: NgForm;
    public showModalPopup: boolean = false;
    @Input() secretQuestionsList: SecretQuestion[];
    @Input() userSecretQuestions: SecretQuestion[];
    public errorMessage: string = '';
    public successMessage: string = '';
    public q1: number; public a1: string = '';
    public q2: number; public a2: string = '';
    public q3: number; public a3: string = '';
    public pwd: string;
    public secretQuestionAnswers: SecretQuestionAnswer[] = [];
    public secretQuestionAnswerRequest: SecretQuestionAnswerRequest;
    submitted: boolean = false;
    constructor(private userSvc: UserService) {

    }

    ngOnInit() {       
        this.pwd = '';
        if (this.userSecretQuestions) {
            this.q1 = this.secretQuestionsList.find(_ => _.questionIDField == this.userSecretQuestions[0].questionIDField).questionIDField;
            this.q2 = this.secretQuestionsList.find(_ => _.questionIDField == this.userSecretQuestions[1].questionIDField).questionIDField;
            this.q3 = this.secretQuestionsList.find(_ => _.questionIDField == this.userSecretQuestions[2].questionIDField).questionIDField;
        }
        else {
            this.q1 = this.secretQuestionsList[0].questionIDField;
            this.q2 = this.secretQuestionsList[1].questionIDField;
            this.q3 = this.secretQuestionsList[2].questionIDField;
        }
    }

    showModal() {          
        this.showModalPopup = true;
    }

    submit() {   
        event.stopPropagation();
        this.errorMessage = '';
        this.successMessage = '';
        this.secretQuestionAnswers = [];
        this.secretQuestionAnswers.push({ questionIDField: this.q1, answerField: this.a1 } as SecretQuestionAnswer);
        this.secretQuestionAnswers.push({ questionIDField: this.q2, answerField: this.a2 } as SecretQuestionAnswer);
        this.secretQuestionAnswers.push({ questionIDField: this.q3, answerField: this.a3 } as SecretQuestionAnswer);
        this.secretQuestionAnswerRequest = {

            secretAnswersField: this.secretQuestionAnswers,
            passwordField: this.pwd,
            userNameField: '',
            iPAddressField: ''
        };
        this.submitted = true;
        this.userSvc.saveSecretQuestionAnswers(this.secretQuestionAnswerRequest).subscribe((response) => {
            if (response.successField) {
                this.resetSecretAnswers();                
                this.successMessage = 'Your Secret Answers saved successfully.';
            }
            else {
                this.errorMessage = response.messagesField[0];
            }
            this.submitted = false;            
        });
        
    }

    resetSecretAnswers() {
        this.a1 = '';
        this.a2 = '';
        this.a3 = '';
        this.errorMessage = '';
    }

    cancel() {
        this.errorMessage = '';
        this.successMessage = '';
        this.resetSecretAnswers();
        this.pwd = '';
        this.showModalPopup = false;
    }

    validate(d: NgForm): boolean {
        if (this.pwd == '') {
            d.controls["password"].markAsDirty();
            return false;
        }
        if (!d.valid) { return false; }              

        if (this.isNullOrWhiteSpace(this.a1) || this.isNullOrWhiteSpace(this.a2) || this.isNullOrWhiteSpace(this.a3)) {
            this.errorMessage = 'You must answer all three secret questions.'
            return false;
        }

        if (this.q1 == this.q2 || this.q2 == this.q3 || this.q3 == this.q1) {
            this.errorMessage = 'Recovery questions must be unique.';
            return false;
        }

        if (this.a1 == this.a2 || this.a2 == this.a3 || this.a3 == this.a1) {
            this.errorMessage = 'Recovery answers must be unique.';
            return false;
        }

        let regexp = new RegExp("^.*(?=.{1,}).*$");        
        if (!(regexp.test(this.a1) && regexp.test(this.a2) && regexp.test(this.a3))) {
            this.errorMessage = 'All secret answers must have atleast one character.';
            return false;
        }

        return true;

    }

    isNullOrWhiteSpace(value: string) {
        if (value == null) return true;

        for (let i = 0; i < value.length; i++) {
            if (!(value[i]==' '|| value[i]=='')) return false;
        }
        return true;
    }
}