import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ElementRef, Pipe, PipeTransform } from '@angular/core';
import { BrowserDynamicTestingModule, platformBrowserDynamicTesting } from '@angular/platform-browser-dynamic/testing';
import { PDMPDetailsComponent } from '../component/shared/pdmp-details/pdmp-details.component';
import { EventService, PDMPService } from '../services/service.import.def';
import { DomSanitizer } from '@angular/platform-browser';
import { By } from '@angular/platform-browser';

let pdmpDetailComponent: PDMPDetailsComponent;
let fixture: ComponentFixture<PDMPDetailsComponent>;

class MockEventServiceService {
    invokeEvent<T>(ID: string, data: T | any) {
        return null;
    }

    subscribeEvent<T>(ID: string, callBack: (data: T) => void, listSubscription: Array<any> = null) {
        return;
    }
}

class MockElementRef {

}

class MockPDMPService {

}



@Pipe({ name: 'safeUrl' })
class MockPipe implements PipeTransform {
    constructor(private s: DomSanitizer) {

    }
    transform(value: string): string {
        //Do stuff here, if you want
        return this.s.bypassSecurityTrustResourceUrl(value);
    }
}

describe('PDMPDetailsComponent', () => {
    beforeEach(async () => {
        TestBed.resetTestEnvironment();
        TestBed.initTestEnvironment(BrowserDynamicTestingModule, platformBrowserDynamicTesting());

        TestBed.configureTestingModule({
            imports: [],
            declarations: [PDMPDetailsComponent, MockPipe],
            providers: [{ provide: EventService, useClass: MockEventServiceService },
                { provide: ElementRef, useClass: MockElementRef },
                { provide: PDMPService, useFactory: MockPDMPService }]
        })
            .compileComponents();

        fixture = TestBed.createComponent(PDMPDetailsComponent);
        pdmpDetailComponent = fixture.componentInstance;
    });

    xit('should popup pdmp details modal', () => {
        //TO do

       pdmpDetailComponent.loadPDMPDetails();
       const btn = fixture.debugElement.query(By.css('#pdmp-details-modal'));
        
       expect(btn.nativeElement.style.display).toBe('block');
    });
});