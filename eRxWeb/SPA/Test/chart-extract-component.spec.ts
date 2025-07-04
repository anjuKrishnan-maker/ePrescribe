import { HttpTestingController } from '@angular/common/http/testing';
import { ChartExtractComponent } from '../component/chart-extract-download/chart-extract.component';
import { ExtractType } from '../model/model.import.def';
import { ChartExtractService } from '../services/service.import.def';
import { of } from 'rxjs';

fdescribe('Chart Extract Tests:', () => {
    let component: ChartExtractComponent;
    let service: ChartExtractService;

    let errorsvc: any;
    let http: any;
    let router: any;

    let mockExtractArray: MockReturn;
    let mockDownloadResponse: MockDownloadResponse;

    class MockExtractRequest {
        constructor() { };
        public id = "id";
        public license = "license";
        public startTime = 'beginning';
        public endTime = 'end';
        public type = 'type';

        public GetStartTimeInTicks() {
            return 1;
        }
        public GetEndTimeInTicks() {
            return 2;
        }
    }

    class MockDownloadResponse {
        constructor() { };
        public status = 200;
    }

    class MockReturn {
        constructor() { };
        public status = 200;
        mockRequest = new MockExtractRequest();
        public body = [this.mockRequest];
    }


    describe('The constructor', () => {
        beforeEach(() => {
            service = new ChartExtractService(http, errorsvc);
            component = new ChartExtractComponent(service, router);
        });

        it('should create object', () => {
            
            expect(component).toBeTruthy();
        });

        
    });

    describe('ngOnInit', () => {
        beforeEach(() => {
            service = new ChartExtractService(http, errorsvc);
            component = new ChartExtractComponent(service, router);
            spyOn(component, 'GetAllRequests');
            spyOn(component, 'initializeChartExtract');
        });

        it('should call getAllRequests', () => {
            component.ngOnInit();
            expect(component.GetAllRequests).toHaveBeenCalled();
        });

        it('should all initializeChartExtract', () => {
            component.ngOnInit();
            expect(component.initializeChartExtract).toHaveBeenCalled();
        });
    });

    describe('GetAllRequests', () => {
        beforeEach(() => {
            service = new ChartExtractService(http, errorsvc);
            component = new ChartExtractComponent(service, router);
            spyOn(component, 'GetRequests');
            component.GetAllRequests();
        });

        //Update this test if we add any extra columns to the chart extract page
        it('should call getRequests exactly 2 times (patient demo and pharm info)', () => {
            expect(component.GetRequests).toHaveBeenCalledTimes(2);
        });

        it('should call getRequest for patient demo', () => {
            expect(component.GetRequests).toHaveBeenCalledWith(ExtractType.PATIENT_DEMOGRAPHICS, "patient demographics");
        });

        it('should call getRequest for pharmacy info', () => {
            expect(component.GetRequests).toHaveBeenCalledWith(ExtractType.PHARMACY, "pharmacy reports");
        });
    });

    describe('GetRequests', () => {
        let testNumber: number = 3;
        let testString: string = 'test';

        beforeEach(() => {
            service = new ChartExtractService(http, errorsvc);
            component = new ChartExtractComponent(service, router);

            mockExtractArray = new MockReturn();

            spyOn(service, 'GetRequests').and.returnValue(of(mockExtractArray));
            
        });

        it('should call GetRequests service with appropriate number/enum', () => {
            component.GetRequests(testNumber, testString);
            expect(service.GetRequests).toHaveBeenCalledWith(testNumber);
        });

        it('should update patientDemo array', () => {
            expect(component.demographicsRequests).toBeFalsy();
            component.GetRequests(ExtractType.PATIENT_DEMOGRAPHICS, testString);
            expect(component.demographicsRequests.length).toEqual(1);
        });

        it('should update pharmInfo array', () => {
            expect(component.pharmacyRequests).toBeFalsy();
            component.GetRequests(ExtractType.PHARMACY, testString);
            expect(component.pharmacyRequests.length).toEqual(1);
        });

        it('should call error creation if status of 204 is returned from service call', () => {
            mockExtractArray.status = 204;
            spyOn(component, 'displayErrorMsg');
            component.GetRequests(testNumber, testString);
            expect(component.displayErrorMsg).toHaveBeenCalledWith("No chart information found for " + testString, false);
        });
    });

    describe('Handle Click', () => {
        let testFileName = "testFileName";
        let mockExtract;

        beforeEach(() => {
            service = new ChartExtractService(http, errorsvc);
            component = new ChartExtractComponent(service, router);

            mockExtract = new MockExtractRequest();
            mockDownloadResponse = new MockDownloadResponse();

            spyOn(service, 'GenerateDownloadFile').and.returnValue(of(mockDownloadResponse));
            spyOn(component, 'createFileName').and.returnValue(testFileName);
            spyOn(component, 'createFileDownload');
        });

        it('should call create download file with good data', () => {
            component.HandleClick(mockExtract);
            expect(component.createFileDownload).toHaveBeenCalledWith(mockDownloadResponse, testFileName + '.zip');
        });

        it('should create error message if status returns as 204', () => {
            mockDownloadResponse.status = 204;
            spyOn(component, 'displayErrorMsg')
            component.HandleClick(mockExtract);
            expect(component.displayErrorMsg).toHaveBeenCalledWith("No relevant chart information found.", false);
        });
    });
});