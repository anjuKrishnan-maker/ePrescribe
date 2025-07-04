
import { Location } from "@angular/common";
import { TestBed, fakeAsync, tick } from "@angular/core/testing";
import { RouterTestingModule } from "@angular/router/testing";
import { Router } from "@angular/router";
import { AppComponent } from "../app.component";
import { registrationRoutes } from "./registration-routing.module";
import { BrowserDynamicTestingModule, platformBrowserDynamicTesting } from "@angular/platform-browser-dynamic/testing";
import { LicenseCreationComponent } from "./license-creation/license-creation.component";
import { SubscriptionComponent } from "./subscription/subscription.component";
import { WelcomePageComponent } from "./welcome-page/welcome-page.component";
import { UserCreationComponent } from "./user-creation/user-creation.component";
import { HttpModule } from "@angular/http";
import { FormsModule } from "@angular/forms";
import { CanActivateRegistrationStep, RegistrantContext} from "./can-load-registration.guard";
import { MessageService } from "../service/message.service";
import { RegistrationService } from "../service/registration.service";
import { Observable, Observer } from "rxjs";
import { HttpErrorResponse } from "@angular/common/http";
import { ErrorRegistrationComponent } from "./error-registration/error-registration.component";


describe("Routes:- Registration & CanActivateRegistrationStep", () => {
    let location: Location;
    let router: Router;
    let fixture;
    let mockRegistrationService: MockRegistrationService;

    class MockRegistrationService {
        getRegistrantStatus() {
            return Observable.create((observer: Observer<any>) => {
                observer.next(<RegistrantContext>{ IsUserCreated: true });
            });
        }
    }

    beforeEach(() => {
        TestBed.resetTestEnvironment();
        TestBed.initTestEnvironment(BrowserDynamicTestingModule, platformBrowserDynamicTesting());
        mockRegistrationService = new MockRegistrationService();
        TestBed.overrideProvider(RegistrationService, { useValue: mockRegistrationService });

        TestBed.configureTestingModule({
            declarations: [LicenseCreationComponent, SubscriptionComponent, WelcomePageComponent, UserCreationComponent, ErrorRegistrationComponent],
            imports: [RouterTestingModule.withRoutes(registrationRoutes), HttpModule, FormsModule],
            providers: [{ provide: CanActivateRegistrationStep, useClass: CanActivateRegistrationStep },
            { provide: MessageService, useClass: MessageService },
            { provide: RegistrationService, useClass: MockRegistrationService },
            { provide: 'window', useFactory: (() => { return {}; }) }]
        }).compileComponents();

        router = TestBed.get(Router);
        location = TestBed.get(Location);
    });



    it("fakeAsync works", fakeAsync(() => {
        let promise = new Promise(resolve => {
            setTimeout(resolve, 10);
        });
        let done = false;
        promise.then(() => (done = true));
        tick(50);
        expect(done).toBeTruthy();
    }));

    it('navigate to "Register.aspx" redirects you to register/subscriptions', fakeAsync(() => {
        router.navigate(["Register.aspx"]).then(() => {
            expect(location.path()).toContain("/register/subscriptions");
        });
    }));

    it('navigate to "register" redirects you to register/subscriptions', fakeAsync(() => {
        router.navigate(["register"]).then(() => {
            expect(location.path()).toContain("/register/subscriptions");
        });
    }));

    it('navigate to "register/subscriptions" redirects you to register/subscriptions', fakeAsync(() => {
        router.navigate(["register/subscriptions"]).then(() => {
            expect(location.path()).toContain("/register/subscriptions");
        });
    }));

    it('navigate to "register/createuser" redirects you to register/createuser when RegistrationService return IsUserCreated as false', fakeAsync(() => {
        spyOn(mockRegistrationService, 'getRegistrantStatus').and.returnValue(Observable.create((observer: Observer<any>) => {
            observer.next(<RegistrantContext>{ IsUserCreated: false });
        }));
       
        router.navigate(["register/createuser"]).then(() => {
            expect(location.path()).toContain("/register/createuser");
        });
    }));

    it('navigate to "register/createuser" redirects you to register/welcome when RegistrationService return IsUserCreated as true', fakeAsync(() => {
        spyOn(mockRegistrationService, 'getRegistrantStatus').and.returnValue(Observable.create((observer: Observer<any>) => {
            observer.next(<RegistrantContext>{ IsUserCreated: true });
        }));
        router.navigate(["register/createuser"]).then(() => {
            expect(location.path()).toContain("/register/welcome");
        });
    }));

    it('navigate to "register/createuser" redirects you to register/createlicense when RegistrationService return IsLOA3StatusConfirmed as true', fakeAsync(() => {
        spyOn(mockRegistrationService, 'getRegistrantStatus').and.returnValue(Observable.create((observer: Observer<any>) => {
            observer.next(<RegistrantContext>{ IsUserCreated: true, IsLOA3StatusConfirmed: true });
        }));

        router.navigate(["register/createuser"]).then(() => {
            expect(location.path()).toContain("/register/createlicense");
        });
    }));

    it('on navigate to "register/welcome" redirects you to register/createlicense when RegistrationService return IsLOA3StatusConfirmed as true', fakeAsync(() => {
        spyOn(mockRegistrationService, 'getRegistrantStatus').and.returnValue(Observable.create((observer: Observer<any>) => {
            observer.next(<RegistrantContext>{ IsUserCreated: true, IsLOA3StatusConfirmed: true });
        }));

        router.navigate(["register/welcome"]).then(() => {
            expect(location.path()).toContain("/register/createlicense");
        });
    }));

    it('on navigate to "register/createuser" when status check : RegistrationService return 401 unauthorized error will takes to "register/createuser"', fakeAsync(() => {
        spyOn(mockRegistrationService, 'getRegistrantStatus').and.returnValue(Observable.create((observer: Observer<any>) => {
            throw new HttpErrorResponse({ status: 401, statusText: "not authenticated" });
        }));

        router.navigate(["register/createuser"]).then(() => {
            expect(location.path()).toContain("/register/createuser");
        });
    }));

    it('on navigate to "register/createuser" when status check : RegistrationService return null will takes to "register/createuser"', fakeAsync(() => {
        spyOn(mockRegistrationService, 'getRegistrantStatus').and.returnValue(Observable.create((observer: Observer<any>) => {
            observer.next(null);
        }));

        router.navigate(["register/createuser"]).then(() => {
            expect(location.path()).toContain("/register/createuser");
        });
    }));

    it('on navigate to "register/welcome" when status check : RegistrationService return 401 unauthorized error:should show "not authorized" message', fakeAsync(() => {
        spyOn(mockRegistrationService, 'getRegistrantStatus').and.returnValue(Observable.create((observer: Observer<any>) => {
            throw new HttpErrorResponse({ status: 401, statusText: "not authenticated" });
        }));

        router.navigate(["register/welcome"]).then(() => {
            let messageService = TestBed.get(MessageService);
            expect(location.path()).toContain("/register/error");
            expect(messageService.message).toBe("Not authorized");
        });
    }));

    it('on navigate to "register/createlicense" when status check : RegistrationService return null :should show "not authorized" message', fakeAsync(() => {
        spyOn(mockRegistrationService, 'getRegistrantStatus').and.returnValue(Observable.create((observer: Observer<any>) => {
            observer.next(null);
        }));

        router.navigate(["register/welcome"]).then(() => {
            let messageService = TestBed.get(MessageService);
            expect(location.path()).toContain("/register/error");
            expect(messageService.message).toBe("Not authorized");
        });
    }));
});