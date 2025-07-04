import { EligAndMedHxStatusComponent } from '../component/common/shared/elig-and-med-hx/elig-and-medhx-status.component';
import { TestBed, ComponentFixture, async } from '@angular/core/testing';
import { BrowserDynamicTestingModule, platformBrowserDynamicTesting } from '@angular/platform-browser-dynamic/testing';
import { Observable } from 'rxjs/Observable';
import { Observer } from 'rxjs/Observer';
import { DebugElement } from '@angular/core';
import { By } from '@angular/platform-browser';
import { HttpModule } from '@angular/http';
import { EventService, EligAndMedHxStatusService, ComponentCommonService } from '../services/service.import.def';
import { EligAndMedHxStatusModel, MessageModel, ApiResponse, ErrorContextModel, ErrorTypeEnum } from '../model/model.import.def';
import 'jquery';
import 'bootstrap/dist/js/bootstrap';
import 'bootstrap/dist/css/bootstrap.min.css';
import { } from 'jasmine';

xdescribe('EligAndMedHsStatusMessage unit tests', () => {

    let component: EligAndMedHxStatusComponent;
    let fixture: ComponentFixture<EligAndMedHxStatusComponent>;
    let apiResponseModel: ApiResponse;
    let elmhs: EligAndMedHxStatusService;
    let errorContextModel: ErrorContextModel;
    let Messages: Array<MessageModel>;

    errorContextModel = null;

    let eligibilityMedHistoryModel = [
        {
            AuditID: "19987",
            Message: "Request sent successfully",
            ProcessedDate: new Date,
            Type: "Eligibility and Med History Request Sent"
        }
    ];

    apiResponseModel = {
        ErrorContext: errorContextModel,
        Payload: eligibilityMedHistoryModel
    }


    class EligibilityMedHistoryMockService {
        GetEligibilityAndMedHistoryStatus() {
            return Observable.create((observer: Observer<ApiResponse>) => {
                observer.next(apiResponseModel);
            });
        }
    }


    beforeEach(async(() => {

        TestBed.resetTestEnvironment();
        TestBed.initTestEnvironment(
            BrowserDynamicTestingModule, platformBrowserDynamicTesting());

        //Test bed configuration
        TestBed.configureTestingModule({
            declarations: [EligAndMedHxStatusComponent],
            providers: [EventService, ComponentCommonService, EligAndMedHxStatusService, { provide: EligAndMedHxStatusService, useClass: EligibilityMedHistoryMockService }],
            imports: [HttpModule],
        }).compileComponents();

    }));

    it('show-eligibility-and-med-history-status', async(() => {
        fixture = TestBed.createComponent(EligAndMedHxStatusComponent);
        component = fixture.debugElement.componentInstance;
        elmhs = TestBed.get(EligAndMedHxStatusService);
        component.GetEligibilityAndMedHistoryStatus();
        fixture.whenStable().then(() => {
            expect(component.EligibityMedHistoryItems[0].AuditID).toEqual(eligibilityMedHistoryModel[0].AuditID);
            expect(component.EligibityMedHistoryItems[0].Type).toEqual(eligibilityMedHistoryModel[0].Type);
            expect(component.EligibityMedHistoryItems[0].Message).toEqual(eligibilityMedHistoryModel[0].Message);
        });
    }));

});

