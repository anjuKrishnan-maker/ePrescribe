import { } from 'jasmine';
import { ComponentFixture } from '@angular/core/test';
import { PatientUploadComponent } from '../component/patient-upload/patient-upload.component';
import { PatientUploadService } from '../services/service.import.def';
import { MessageModel, MessageIcon } from '../model/message.model';
import { PatientUploadResponse, ImportBatchModel, ApiResponse } from '../model/model.import.def';
import { of } from 'rxjs';

fdescribe('Patient-Upload-Component', () => {
    let component: PatientUploadComponent;
    let service: PatientUploadService;
    let fixture: ComponentFixture<PatientUploadComponent>

    let errorsvc: any;
    let http: any;
    let router: any;

    //Runs once per test feature
    beforeAll(() => {

    });

    beforeEach(() => {
        service = new PatientUploadService(http, errorsvc);
        component = new PatientUploadComponent(service, router);
        component.StaticMessageStrip = [];
    });

    it('should create patient upload component', () => {
        expect(component).toBeTruthy();
    });

    it('should create patient upload servoce', () => {
        expect(service).toBeTruthy();
    });


    it('should invoke initializePatientUpload on component init', () => {
        //arrange
        spyOn(component, 'initializePatientUpload');
    
        //act
        component.initializePatientUpload();

        //assert
        expect(component.initializePatientUpload).toHaveBeenCalled();
    });

    it('should reset patient upload controls', function () {
        //act
        component.resetControls();

        //assert
        expect(component.AllowUpload).toBeTruthy();
        expect(component.fileSelected).toBeFalsy();
        expect(component.fileValid).toBeFalsy();
    });

    it('should validate file and init input value variable', function () {
        //arrange
        const mockFile = new File(['foo'], 'filename', { type: 'text/plain' });
        const mockEvt = { target: { files: [mockFile] } };

        //act
        component.fileChangeEvent(mockEvt);

        //arrange
        expect(component.fileSelected).toBeTruthy();
        expect(component.fileValid).toBeTruthy();
    });

    it('should invalidate file and init input value variable with file size too large', function () {
        //arrange
        const mockFile = new File(['foo'], 'filename', { type: 'text/plain' });
        Object.defineProperty(
            mockFile, 'size', { value: 1000001, writable: false });
        const mockEvt = { target: { files: [mockFile] } };

        //act
        component.fileChangeEvent(mockEvt);

        //arrange
        expect(component.fileSelected).toBeTruthy();
        expect(component.fileValid).toBeFalsy();
        expect(component.ErrorMessage).toEqual("File size exceeds limit");
    });

    it('should invalidate file type on selected file with file is invalid msg', function () {
        //arrange
        const mockFile = new File(['foo'], 'filename', { type: 'text/html' });
        Object.defineProperty(
            mockFile, 'size', { value: 999999, writable: false });
        const mockEvt = { target: { files: [mockFile] } };

        //act
        component.fileChangeEvent(mockEvt);

        //assert
        expect(component.fileSelected).toBeTruthy();
        expect(component.fileValid).toBeFalsy();
        expect(component.ErrorMessage).toEqual("File selected is invalid");
    });

    it('button click generateReport should call and generate a job failed record report', function () {
        //arrange 
        spyOn(component, 'generateJobReport');
        let jobID: number = 1;

        //act
        component.generateJobReport();

        //assert
        expect(component.generateJobReport).toHaveBeenCalled();
    });

    it('should display appropriate error msg in message strip', function () {
        //arrange
        let msg: string = "This is a relevant error msg";
        let allowClickAway: boolean = false; 

        //act
        component.displayErrorMsg(msg, allowClickAway);

        //assert
        expect(component.StaticMessageStrip).toContain(jasmine.objectContaining({
            Message: msg,
            Icon: MessageIcon.Error,
            ShowCloseButton: allowClickAway
        }))
    });

    it('should handle API response appropriately and display upload success msg', function () {
        //arrange 
        let patientUploadResponse: PatientUploadResponse = {
            JobInFlight: true,
            UploadSuccess: true,
            ImportBatchJobHistory: [],
            CurrentJob: new ImportBatchModel(),
            ErrorMessage: null
        };
        let msg: string = "   File has been uploaded successfully. Please check back later for status.";
        let allowClickAway: boolean = true; 

        //act
        component.handleResponse(patientUploadResponse);

        //assert
        expect(component.CurrentJob).toEqual(patientUploadResponse.CurrentJob);
        expect(component.StaticMessageStrip).toContain(jasmine.objectContaining({
            Message: msg,
            Icon: MessageIcon.Success,
            ShowCloseButton: allowClickAway
        }))
    });

    it('should handle API response appropriately and display upload error msg', function () {
        //arrange 
        spyOn(component, 'displayErrorMsg');
        let patientUploadResponse: PatientUploadResponse = {
            JobInFlight: false,
            UploadSuccess: false,
            ImportBatchJobHistory: [],
            CurrentJob: new ImportBatchModel(),
            ErrorMessage: "There was an error somewhere"
        };
        let allowClickAway: boolean = true;

        //act
        component.handleResponse(patientUploadResponse);

        //assert
        expect(component.displayErrorMsg).toHaveBeenCalledWith(patientUploadResponse.ErrorMessage, allowClickAway);
        expect(component.ErrorMessage).toEqual(patientUploadResponse.ErrorMessage);
    });

    it('should POST patient binary data to API endpoint', function () {
        //arrange
        const patientUploadResponse: PatientUploadResponse = {
            JobInFlight: true,
            UploadSuccess: true,
            ImportBatchJobHistory: [],
            CurrentJob: new ImportBatchModel,
            ErrorMessage: ''
        }

        let response;
        spyOn(service, 'UploadFile').and.returnValue(of(patientUploadResponse));

        //act
        service.UploadFile("some blob of patient data").subscribe(res => {
            response = res;
        });

        //assert
        expect(response).toEqual(patientUploadResponse);
    });

    it('should GET list of patient import batch jobs from API endpoint', function () {
        //arrange
        const patientUploadResponse: PatientUploadResponse = {
            JobInFlight: false,
            UploadSuccess: false,
            ImportBatchJobHistory: [{
                ID: 1,
                LicenseID: null,
                ProcessBegin: new Date,
                ProcessEnd: new Date,
                BatchSize: 1000,
                ImportBatchStatusID: 1,
                StatusDescription: '',
                ErrorLines: ''
            }],
            CurrentJob: null,
            ErrorMessage: ''
        }

        let response;
        spyOn(service, 'CheckJobStatus').and.returnValue(of(patientUploadResponse));

        //act
        service.CheckJobStatus().subscribe(res => {
            response = res;
        });

        //assert
        expect(response).toEqual(patientUploadResponse);
    });

    it('should GET a file CSV blob for user to download', function () {
        //arrange
        const obj = { data: 'this is some dataset' };
        const blob = new Blob([JSON.stringify(obj, null, 2)], { type: 'application/json' });

        let response;
        let jobID = 1;
        spyOn(service, 'GenerateJobReport').and.returnValue(of(blob));

        //act
        service.GenerateJobReport(jobID).subscribe(res => {
            response = res;
        });

        //assert
        expect(response).toEqual(blob);
    });
});