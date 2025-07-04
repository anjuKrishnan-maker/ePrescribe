import { Component, ViewChild, ElementRef, Renderer2 } from '@angular/core';
import { PatientUploadService } from '../../services/service.import.def';
import { MessageModel, MessageIcon, PatientUploadResponse, ImportBatchModel } from '../../model/model.import.def';
import { Router } from '@angular/router';
import { ROUTE_NAME } from '../../tools/constants';
@Component({
    selector: 'erx-patient-upload',
    templateUrl: './patient-upload.template.html',
    styleUrls: ['./patient-upload.style.css']
})

export class PatientUploadComponent {
    private messages: Array<MessageModel>;
    private fileInput;
    public StaticMessageStrip: Array<MessageModel> = [];
    fileSelected = false;
    fileValid = false;
    MaxFileSize = 1000000;
    AllowedFileTypes = ['text/plain'];
    ErrorMessage: string;
    isPageLoading: boolean = true;
    AllowUpload: boolean = false;
    CurrentJob: ImportBatchModel = null;
    JobHistory: ImportBatchModel[];
    @ViewChild('fileUploadCtrl') fileInputElement: ElementRef;

    constructor(private puSvc: PatientUploadService, private router: Router) { }

    ngOnInit() {
        this.initializePatientUpload();
    }

    initializePatientUpload()
    {
        this.StaticMessageStrip = [];
        this.JobHistory = [];
        let messageModel: MessageModel;
        this.messages = new Array<MessageModel>();
        if (messageModel != undefined && messageModel.Message !== '') {
            this.messages.push(messageModel);
        }

        //check if any jobs are in flight
        this.puSvc.CheckJobStatus().subscribe((response: PatientUploadResponse) => {
            if (response.CurrentJob) {
                this.AllowUpload = false;
                this.StaticMessageStrip.push(new MessageModel("   Job currently being processed; Please check back later", MessageIcon.Information, "selectMedicationStartUpMessages", false));
                this.fileSelected = this.fileValid = false;
                if (response.ImportBatchJobHistory && response.CurrentJob)
                    this.CurrentJob = response.ImportBatchJobHistory[0];
            }
            else {
                this.AllowUpload = true;
            }
            this.JobHistory = response.ImportBatchJobHistory;
            this.isPageLoading = false;
        });
    }

    navigateToSettings() {
        this.router.navigate([ROUTE_NAME.Settings]);
    }

    resetControls() {
        this.fileSelected = this.fileValid = false;
        if (this.fileInputElement)
            this.fileInputElement.nativeElement.value = null;
        if (this.ErrorMessage) {
            this.ErrorMessage = this.CurrentJob = null;
            this.StaticMessageStrip = [];
        }
        this.AllowUpload = true;
    }

    fileChangeEvent(fileInput: any) {
        this.fileInput = fileInput.target.files[0];
        if (this.isFileValid()) {
            if (fileInput.target.files && fileInput.target.files[0]) {
                if (this.ErrorMessage) {
                    this.ErrorMessage = null;
                    this.StaticMessageStrip = [];
                }
                this.fileSelected = this.fileValid = true;
                this.fileInput = fileInput.target.files[0];
            } else {
                this.resetControls();
            }
        }
        else {
            this.displayErrorMsg(this.ErrorMessage, true);
            this.fileValid = false;
            this.fileSelected = true;
        }
    }

    isFileValid() {
        if (this.fileInput) {
            if (this.fileInput && this.fileInput.size >= this.MaxFileSize) {
                this.ErrorMessage = "File size exceeds limit";
                return false;
            }
            if (!this.AllowedFileTypes.includes(this.fileInput.type)) {
                this.ErrorMessage = "File selected is invalid";
                return false;
            }
            return true;
        }
    }

    onCurrentJobChanged(selectedJob) {
        if (this.ErrorMessage) {
            this.ErrorMessage = null;
            this.StaticMessageStrip = [];
        }
        this.CurrentJob = selectedJob;
    }

    submitUploadFile(PatientData) {
        this.puSvc.UploadFile(PatientData).subscribe(
            (res) => {
                this.isPageLoading = false;
                this.handleResponse(res);
            },
            (err) => {
                console.log(err);
            }
        );
    }

    createPatientFile() {
        this.isPageLoading = true;
        var reader = new FileReader();
        reader.onload = () => {
            var PatientData = reader.result as string;
            this.submitUploadFile(PatientData);
        }
        reader.readAsDataURL(this.fileInput);
    }

    createFileDownload(data, fileName) {
        let blob = new Blob([data], { type: 'octet/stream' });
        if (window.navigator.msSaveOrOpenBlob) //IE & Edge
        {
            window.navigator.msSaveBlob(blob, fileName);
        }
        else //Chrome & FF
        {
            const url = window.URL.createObjectURL(blob);
            const anchor = document.createElement("a");
            anchor.href = url;
            anchor.download = fileName;
            document.body.appendChild(anchor); 
            anchor.click();
            document.body.removeChild(anchor);
        }
    }

    generateJobReport() {
        this.isPageLoading = true;
        this.puSvc.GenerateJobReport(this.CurrentJob.ID).subscribe(
            (res) => {
                this.isPageLoading = false;
                this.createFileDownload(res, "RejectedRecords_" + this.CurrentJob.ProcessBegin.toLocaleString() + '.csv');
            },
            (err) => {
                this.isPageLoading = false;
                this.displayErrorMsg(err, false);
            }
        );
    }

    handleResponse(response) {
        if (response.CurrentJob) {
            this.AllowUpload = this.fileSelected = this.fileValid = false;
            this.CurrentJob = response.CurrentJob;
            if (response.ErrorMessage) {
                this.ErrorMessage = response.ErrorMessage;
                this.displayErrorMsg(this.ErrorMessage, true);
            }
            else {
                this.StaticMessageStrip.push(new MessageModel(
                    "   File has been uploaded successfully. Please check back later for status.",
                    MessageIcon.Success,
                    "selectMedicationStartUpMessages",
                    true));
            }
        }
    }

    displayErrorMsg(errorMsg, allowClickoff) {
        this.StaticMessageStrip = [];
        this.StaticMessageStrip.push(new MessageModel(
            errorMsg,
            MessageIcon.Error,
            "selectMedicationStartUpMessages",
            allowClickoff));
    }
}