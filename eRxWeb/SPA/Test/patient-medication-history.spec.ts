import { async, TestBed, ComponentFixture } from '@angular/core/testing';
import { ChangeDetectorRef } from '@angular/core';
import { } from 'jasmine';
import { Observable } from 'rxjs/Observable';
import { Observer } from 'rxjs/Observer';
import { EventService, ErrorService, ComponentCommonService, PatientMedHistoryService } from '../services/service.import.def';
import { PatientMedHistoryComponent } from '../component/common/shared/PatientMedicationHistory/patient-medication-history.component';
import { PatientMedHistoryModel } from '../model/model.import.def';
import { BrowserDynamicTestingModule, platformBrowserDynamicTesting } from '@angular/platform-browser-dynamic/testing';

class MockEventService {
}

describe("Patient Medication History Component", () => {

    //let patientMedHistoryComponent: PatientMedHistoryComponent;
    //let fixture: ComponentFixture<PatientMedHistoryComponent>;
    //let el: HTMLElement;
    //let evnntSvc: EventService;
    //let patientMedHistoryModel = new PatientMedHistoryModel();

    //patientMedHistoryModel.rxDate='10/10/2019';
    //patientMedHistoryModel.diagnosis = 'Not Selected';
    //patientMedHistoryModel.status = 'Complete';
    //patientMedHistoryModel.source = 'ePrescribe';
    //patientMedHistoryModel.medDetail = 'Aspir';

    //class MockPatientMedHistoryService extends PatientMedHistoryService {
    //    GetPatientMedHistory() {
    //        return Observable.create((observer: Observer<void>) => {
    //            observer.next(patientMedHistoryModel);
    //        });
    //    }
    //}
    
    //beforeEach(() => {
    //    TestBed.resetTestEnvironment();
    //    TestBed.initTestEnvironment(BrowserDynamicTestingModule, platformBrowserDynamicTesting());

    //    //Test bed configuration
    //    TestBed.configureTestingModule({
    //        declarations: [PatientMedHistoryComponent],
    //        providers: [PatientMedHistoryComponent, ChangeDetectorRef, { provide: PatientMedHistoryService, useClass: MockPatientMedHistoryService },
    //            { provide: EventService, useclass: MockEventService }],
    //    });

    //    fixture = TestBed.createComponent(PatientMedHistoryComponent);
    //    patientMedHistoryComponent = fixture.componentInstance;
    //})

    //fit('PatientMedicationHistory', async(() => {
    //    //Arrange

    //    //Act 
    //    patientMedHistoryComponent.showPatientMedHistory();

    //    //Assert
    //    //el = fixture.nativeElement;
    //    //debugger;
    //    //expect()
    //}));
});
