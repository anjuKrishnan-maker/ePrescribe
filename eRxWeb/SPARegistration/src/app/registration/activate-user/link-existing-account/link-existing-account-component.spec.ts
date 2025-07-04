import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { HttpModule } from '@angular/http';
import { BrowserModule, By } from '@angular/platform-browser';
import { RouterTestingModule } from '@angular/router/testing';
import { BrowserDynamicTestingModule, platformBrowserDynamicTesting } from '@angular/platform-browser-dynamic/testing';
import { FormsModule } from '@angular/forms';
import { LinkExistingAccount } from '../link-existing-account/link-existing-account-component';
import { Observable, Observer } from 'rxjs';
import { ActivationCodeService } from '../../../service/activation-code.service';
import { ShowCaptchaComponent } from '../../show-captcha/show-captcha.component';
import { LinkAccountConfirmationPopupComponent } from '../link-account-confirmation-popup/link-account-confirmation-popup-component';
import { Router } from '@angular/router';

describe('LinkAccountConfirmationPopupComponent', () => {
    let component: LinkExistingAccount;
    let fixture: ComponentFixture<LinkExistingAccount>;
    let response: any;

    class MockActivationCodeService {
        activateUser() {
            return Observable.create((observer: Observer<any>) => {
                observer.next(response);
            });
        }
    }

    class MockRouter {
        navigate(str: any): Promise<Boolean> {
            return new Promise(() => {
                true;
            })
        }
    }

    beforeEach(async(() => {
        TestBed.resetTestEnvironment();
        TestBed.initTestEnvironment(BrowserDynamicTestingModule,
            platformBrowserDynamicTesting());
        TestBed.configureTestingModule({
            imports: [HttpModule, BrowserModule, FormsModule, RouterTestingModule],
            declarations: [LinkExistingAccount, ShowCaptchaComponent, LinkExistingAccount, LinkAccountConfirmationPopupComponent],
            providers: [{ provide: ActivationCodeService, useClass: MockActivationCodeService },
            { provide: 'window', useFactory: (() => { return { appcontext: { version: "333" } }; }) },
            { provide: Router, useClass: MockRouter }]
        }).compileComponents();
    }));
    beforeEach(() => {
        fixture = TestBed.createComponent(LinkExistingAccount);
        component = fixture.debugElement.componentInstance;
    });
    it('should_have_link_existing_user_component', async(() => {
        fixture = TestBed.createComponent(LinkExistingAccount);
        component = fixture.debugElement.componentInstance;
        expect(component).toBeTruthy();
    }));
});