import { async, ComponentFixture, TestBed, fakeAsync, tick } from '@angular/core/testing';
import { UserActivationComponent } from './user-activation.component';
import { BrowserDynamicTestingModule, platformBrowserDynamicTesting } from '@angular/platform-browser-dynamic/testing';
import { UserActivationPopup } from '../user-activation-popup/user-activation-popup.component';
import { FormsModule } from '@angular/forms';
import { ShowCaptchaComponent } from '../../show-captcha/show-captcha.component';
import { LinkExistingAccount } from '../link-existing-account/link-existing-account-component';
import { LinkAccountConfirmationPopupComponent } from '../link-account-confirmation-popup/link-account-confirmation-popup-component';
import { RouterTestingModule } from '@angular/router/testing';
import { ActivationCodeService } from '../../../service/activation-code.service';
import { CaptchaService } from '../../../service/captcha.service';
import { BrowserModule } from '@angular/platform-browser';
import { Observable } from 'rxjs';
import { ActivatedRoute } from '@angular/router';


describe('UserActivationComponent', () => {
    let component: UserActivationComponent;
    let fixture: ComponentFixture<UserActivationComponent>;

    class MockActivationCodeService {

    }

    class MockCaptchaService {

    }

    class MaockActivatedRoute {

    }

    beforeEach(async(() => {
        TestBed.resetTestEnvironment();
        TestBed.initTestEnvironment(BrowserDynamicTestingModule,
            platformBrowserDynamicTesting());
        TestBed.configureTestingModule({
            imports: [FormsModule, RouterTestingModule, BrowserModule],
            declarations: [UserActivationComponent, UserActivationPopup, ShowCaptchaComponent, LinkExistingAccount, LinkAccountConfirmationPopupComponent],
            providers: [{ provide: ActivationCodeService, useClass: MockActivationCodeService },
                { provide: CaptchaService, useClass: MockCaptchaService },
                { provide: 'window', useFactory: (() => { return { appcontext: { version: "333" } }; }) }
                       ]
        }).compileComponents();
    }));

    it('should_have_user_activation_component', async(() => {
        fixture = TestBed.createComponent(UserActivationComponent);
        component = fixture.debugElement.componentInstance;
        expect(component).toBeTruthy();
    }));

    it('should_have_user_activation_component_contain_header_as_\'Welcome to the Veradigm Security Account Activation Wizard!\'', () => {
        fixture = TestBed.createComponent(UserActivationComponent);
        let element = fixture.nativeElement;
        var title = element.querySelector('#divTitleText > span').textContent;
        expect(title).toEqual('Welcome to the Veradigm Security Account Activation Wizard!');
    });

    it('should_have_user_activation_component_enroll_now_div_contain_header_as_\'Enroll Now\'', () => {
        fixture = TestBed.createComponent(UserActivationComponent);
        let element = fixture.nativeElement;
        var title = element.querySelector('#divEnrollNow > span').textContent;
        expect(title).toEqual('Enroll Now');
    });

    it('should_have_user_activation_component_link_accounts_now_div_contain_header_as_\'Link Accounts\'', () => {
        fixture = TestBed.createComponent(UserActivationComponent);
        let element = fixture.nativeElement;
        var title = element.querySelector('#divLinkAccounts > span').textContent;
        expect(title).toEqual('Link Accounts');
    });

    it('should_have_user_activation_component_enroll_now_div_contain_button_as_\'Sign Up\'', () => {
        fixture = TestBed.createComponent(UserActivationComponent);
        let element = fixture.nativeElement;
        var buttonName = element.querySelector('#btnSignUp').textContent;
        expect(buttonName).toEqual('Sign Up');
    });

    it('should_have_user_activation_component_link_accounts_now_div_contain_button_as_\'Link Accounts\'', () => {
        fixture = TestBed.createComponent(UserActivationComponent);
        let element = fixture.nativeElement;
        var buttonName = element.querySelector('#btnLinkAccounts').textContent;
        expect(buttonName).toEqual('Link Accounts');
    });
});