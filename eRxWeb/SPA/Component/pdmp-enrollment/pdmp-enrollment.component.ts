import { Component, ElementRef, ChangeDetectorRef, ViewChild } from '@angular/core';
import { EventService, ComponentCommonService, PDMPService } from '../../services/service.import.def';
import { PdmpEnrollmentModel, PdmpEnrollmentSubmissionResponse } from '../../model/model.import.def';
import { BaseComponent } from '../common/base.component';
import { ModalPopupControl } from '../shared/controls/modal-popup/modal-popup.control';
import { FormsModule, NgForm } from '@angular/forms';

@Component({
    selector: 'erx-pdmp-enrollment',
    templateUrl: './pdmp-enrollment.template.html',
    styleUrls: ['./pdmp-enrollment.style.css']
})

export class PdmpEnrollmentComponent extends BaseComponent {
    pdmpEnrollmentModel: PdmpEnrollmentModel = new PdmpEnrollmentModel();
    @ViewChild(ModalPopupControl) modalPopup: ModalPopupControl;
    EnrollmentSuccessMessage: string = '';
    FormSubmitMessage: string = '';
    ShowForm: boolean = true;
    IsMessagePositive: boolean = true;
    ShowValidationMessage: boolean = false;

    constructor(private comonSvc: ComponentCommonService, private svE: EventService, cd: ElementRef, private cdRef: ChangeDetectorRef, private svPdmp: PDMPService
    ) {
        super(cd, svE);
    }

    ShowPdmpEnrollmentForm() {
        this.CleanUp();
        this.GetPdmpEnrollmentFormData();
        this.modalPopup.OpenPopup();
        this.StartLoading();
    }

    CleanUp() {
        this.pdmpEnrollmentModel = new PdmpEnrollmentModel();
        this.ShowValidationMessage = false;
        this.ShowForm = true;
        this.EnrollmentSuccessMessage = '';
        this.FormSubmitMessage = '';
        this.IsMessagePositive = true;
    }

    GetPdmpEnrollmentFormData() {
        this.svPdmp.GetPdmpEnrollmentFormInfo().subscribe((response) => {
            if (response !== null) {
                this.pdmpEnrollmentModel = response;
                this.cdRef.detectChanges();
            }
            this.ShowValidationMessage = true;
            this.EndLoading();
        });
    }

    SubmitButtonClick(form: NgForm) {
        if (form.valid) {
            this.IsMessagePositive = true;
            this.FormSubmitMessage = 'Submitting Form, Please Wait';
            this.SubmitForm(this.pdmpEnrollmentModel);
        }
    }

    SubmitForm(model: PdmpEnrollmentModel) {
        this.svPdmp.SubmitPdmpEnrollmentForm(model).subscribe((response) => {
            if (response !== null) {
                if (response.IsSubmissionSuccessful) {
                    this.ShowForm = false;
                    this.FormSubmitMessage = '';
                    this.EnrollmentSuccessMessage = response.EnrollmentSuccessMessage;
                }
                else {
                    this.IsMessagePositive = false;
                    this.FormSubmitMessage = 'There was an issue while submitting PDMP Enrollment Form';
                }
            }
            this.EndLoading();
        });
    }

    Close() {
        this.modalPopup.ClosePopup();
    }
}