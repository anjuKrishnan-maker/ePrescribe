import { async, TestBed, ComponentFixture } from '@angular/core/testing';
import { ChangeDetectorRef } from '@angular/core';
import { } from 'jasmine';
import { Observable } from 'rxjs/Observable';
import { Observer } from 'rxjs/Observer';
import { PrivacyOverrideReasonComponent } from '../component/common/right-panel/privacy-override-reason/privacy-override-reason.component';
import { EventService, PrivacyOverrideService } from '../services/service.import.def';
import { PrivacyPatientInfoDTO } from '../model/model.import.def';
import { BrowserDynamicTestingModule, platformBrowserDynamicTesting } from '@angular/platform-browser-dynamic/testing';

class MockPrivacyOverrideService {
    GetOverrideReason() {
        return Observable.create((observer: Observer<void>) => {
            observer.next("privacy override reason");
        });
    }
}

class MockEventServiceService {
}
//Tested except the 
describe('PrivacyOverrideReasonComponent- Class & Component', () => {
    let privacyOverrideComponent: PrivacyOverrideReasonComponent;
    let fixture: ComponentFixture<PrivacyOverrideReasonComponent>;
    let el: HTMLElement; // the DOM element with the privacy override reason
    beforeAll(() => {
        //Nothing for now!
    });

    beforeEach(async(() => {
        TestBed.resetTestEnvironment();
        TestBed.initTestEnvironment(BrowserDynamicTestingModule, platformBrowserDynamicTesting());

        //Test bed configuration
        TestBed.configureTestingModule({
            declarations: [PrivacyOverrideReasonComponent],
            providers: [PrivacyOverrideReasonComponent, ChangeDetectorRef, { provide: PrivacyOverrideService, useClass: MockPrivacyOverrideService },
                { provide: EventService, useclass: MockEventServiceService }],
        });

        fixture = TestBed.createComponent(PrivacyOverrideReasonComponent);
        privacyOverrideComponent = fixture.componentInstance;
    }));

    it('should not have privacy override reason [is undefined]  after construction', () => {
        expect(privacyOverrideComponent.privacyOverrideReason).toBeUndefined();
    });

    it('should not have privacy override reason: is null if patient id is empty', () => {
        let patientInfo = new PrivacyPatientInfoDTO();
        patientInfo.PatientId = "";
        privacyOverrideComponent.loadPrivacyOverrideReason(patientInfo);
        expect(privacyOverrideComponent.privacyOverrideReason).toBeNull();
    });

    it('should have privacy override reason: when patient id is not empty', () => {
        let patientInfo = new PrivacyPatientInfoDTO();
        patientInfo.PatientId = "1";
        privacyOverrideComponent.loadPrivacyOverrideReason(patientInfo);
        expect(privacyOverrideComponent.privacyOverrideReason).toContain("privacy override reason");
    });

    it('should have privacy override reason in view: when patient id is not empty', () => {
        let patientInfo = new PrivacyPatientInfoDTO();
        patientInfo.PatientId = "1";
        privacyOverrideComponent.loadPrivacyOverrideReason(patientInfo);
        el = fixture.nativeElement.querySelector('.message_privacy-override-reason');

        const content = el.textContent;
        expect(content).toContain('privacy override reason', content);
    });

    it('should not have reason div rendered in view: when the new patient id passed is empty when reason already rendered once', () => {
        privacyOverrideComponent.privacyOverrideReason = "Already existing";
        fixture.detectChanges();
        el = fixture.nativeElement.querySelector('.message_privacy-override-reason');
        const content = el.textContent;
        expect(content).toContain('Already existing', content);

        let patientInfo = new PrivacyPatientInfoDTO();
        patientInfo.PatientId = "";
        privacyOverrideComponent.loadPrivacyOverrideReason(patientInfo);
        el = fixture.nativeElement.querySelector('.message_privacy-override-reason');
        expect(el).toBeNull();
    });

});