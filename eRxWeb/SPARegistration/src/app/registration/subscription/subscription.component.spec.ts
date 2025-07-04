import { async, ComponentFixture, TestBed, tick } from '@angular/core/testing';

import { SubscriptionComponent } from './subscription.component';
import { RouterTestingModule } from '@angular/router/testing';
import { AppContext } from '../../api-response.model';
import { BrowserDynamicTestingModule, platformBrowserDynamicTesting } from '@angular/platform-browser-dynamic/testing';
import { Router } from '@angular/router';
import { Location } from "@angular/common";
import { LicenseCreationComponent } from '../license-creation/license-creation.component';
import { WelcomePageComponent } from '../welcome-page/welcome-page.component';
import { UserCreationComponent } from '../user-creation/user-creation.component';
import { FormsModule } from '@angular/forms';
import { HttpModule } from '@angular/http';
import { registrationRoutes } from '../registration-routing.module';
import { RegistrantContext } from '../can-load-registration.guard';
import { Observable, Observer } from 'rxjs';
import { RegistrationService } from '../../service/registration.service';
describe('SubscriptionComponent', () => {
    let subscriptionComponent: any;//throwing a compile error.Is fine to keep any in test case.
    let fixture: ComponentFixture<SubscriptionComponent>;
    let router: Router;
    let location: Location;
    let mockRegistrationService: MockRegistrationService;

    class MockRegistrationService {
        getRegistrantStatus() {
            return Observable.create((observer: Observer<any>) => {
                observer.next(<RegistrantContext>{ IsUserCreated: true });
            });
        }
    }

    beforeEach(async(() => {
        TestBed.resetTestEnvironment();
        TestBed.initTestEnvironment(BrowserDynamicTestingModule, platformBrowserDynamicTesting());
        mockRegistrationService = new MockRegistrationService();
        TestBed.configureTestingModule({
            imports: [RouterTestingModule.withRoutes(registrationRoutes), HttpModule, FormsModule],
            declarations: [LicenseCreationComponent, SubscriptionComponent, WelcomePageComponent, UserCreationComponent],//required as we are importing the actual routes
            providers: [
                { provide: 'window', useFactory: (() => { return { appcontext: <AppContext>{ basicPrice: "1", deluxePrice: "2", epcsPrice: "3" } }; }) },
                { provide: RegistrationService, useClass: MockRegistrationService }
            ]
        }).compileComponents();
        router = TestBed.get(Router);
        location = TestBed.get(Location);
    }));

    beforeEach(() => {
        fixture = TestBed.createComponent(SubscriptionComponent);
        subscriptionComponent = fixture.debugElement.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(subscriptionComponent).toBeTruthy();
    });

    it('should have price set from windows context', () => {
        let subscription = subscriptionComponent.model;
        expect(subscription.price.basic).toBe("1");
        expect(subscription.price.deluxe).toBe("2");
        expect(subscription.price.epcs).toBe("3");
    });

    it('should render price as set from windows context', () => {

        const compiled = fixture.debugElement.nativeElement;

        expect(compiled.querySelector('#subscription-basicPrice .subscription-price-number').textContent).toContain('1');
        expect(compiled.querySelector('#subscription-deluxePrice .subscription-price-number').textContent).toContain('2');
        expect(compiled.querySelector('#subscription-epcsPrice .subscription-price-number').textContent).toContain('3');
    });

    //TODO: complete this. Not sure how to adress this.
    xit('should redirect to create user when url contains query string eid', () => {

        spyOn(mockRegistrationService, 'getRegistrantStatus').and.returnValue(Observable.create((observer: Observer<any>) => {
            observer.next(<RegistrantContext>{ IsUserCreated: false });
        }));

        spyOn(subscriptionComponent.router, 'navigate');

        router.navigate(["register"], { queryParams: { eid: 'XXXXXX' } }).then(() => {
            console.log(location.path());
            fixture.detectChanges();
            tick();
            console.log(location.path());
            expect(subscriptionComponent.router.navigate).toHaveBeenCalledWith(['/register/createuser']);
            //expect(location.path()).toContain("/register/createuser");
        });
    });
});
