import { async, ComponentFixture, TestBed, fakeAsync,tick  } from '@angular/core/testing';
import { UserActivationPopup } from './user-activation-popup.component';
import { HttpModule } from '@angular/http';
import { BrowserModule, By } from '@angular/platform-browser';
import { RouterTestingModule } from '@angular/router/testing';
import { BrowserDynamicTestingModule, platformBrowserDynamicTesting } from '@angular/platform-browser-dynamic/testing';
import { FormsModule } from '@angular/forms';
import { ActivationCodeService } from '../../../service/activation-code.service';
import { ShowCaptchaComponent } from '../../show-captcha/show-captcha.component';
import { LinkExistingAccount } from '../link-existing-account/link-existing-account-component';
import { LinkAccountConfirmationPopupComponent } from '../link-account-confirmation-popup/link-account-confirmation-popup-component';
import { Observable, Observer } from 'rxjs';
import { Router } from '@angular/router';
fdescribe('UserActivationPopup', () => {
    let component: UserActivationPopup;
    let fixture: ComponentFixture<UserActivationPopup>;
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
            declarations: [UserActivationPopup, ShowCaptchaComponent, LinkExistingAccount, LinkAccountConfirmationPopupComponent],
            providers: [{ provide: ActivationCodeService, useClass: MockActivationCodeService },
                { provide: 'window', useFactory: (() => { return { appcontext: { version: "333" } }; }) },
                { provide: Router, useClass: MockRouter }]
        }).compileComponents();
    }));
    beforeEach(() => {
        fixture = TestBed.createComponent(UserActivationPopup);
        component = fixture.debugElement.componentInstance;
    });
    it('should_have_user_activation_popup_component', async(() => {
        fixture = TestBed.createComponent(UserActivationPopup);
        component = fixture.debugElement.componentInstance;
        expect(component).toBeTruthy();
    }));
    //it('should_have_user_activation_component_contain_header_as_\'Please enter the activation code provided by an Administrator.\'', () => {
    //    fixture = TestBed.createComponent(UserActivationPopup);
    //    let element = fixture.nativeElement;
    //    var title = element.querySelector('#divHeader >').textContent;
    //    expect(title).toEqual('Please enter the activation code provided by an Administrator.');
    //});
  
    //it('should_have_user_activation_component_div_contain_button_as_\'Activate\'', () => {
    //    fixture = TestBed.createComponent(UserActivationPopup);
    //    let element = fixture.nativeElement;
    //    console.log(element);
    //    var buttonName = element.querySelector('#btnActivate')
    //    // var buttonName = element.querySelector('#btnActivate').textContent;
    //    console.log(buttonName);
    //    expect(buttonName).toEqual('Activate');
    //});
    it('should_have_user_activation_component_popup_div_contain_button_as_\'Activate\'', fakeAsync(() => {
        //let element = fixture.nativeElement;
        component.showModalLayout = true;
        component.showModal = true;
        
            
        component.showModal = true;
        //fixture.componentInstance.showModalLayout = true;
        //fixture.componentInstance.showModal = true;
        console.log('before detect changes')
        //fixture.detectChanges();
        console.log('after detect changes')
        tick();
        console.log('after tick')
        let buttonName=fixture.debugElement.query(By.css('input[name="btnActivate"]')).nativeElement;
        //let buttonName = element.querySelector('#btnActivate')
        //console.log(element);
        console.log(buttonName);
        //var buttonName = element.querySelector('#btnActivate').textContent;
        expect(buttonName).toEqual('Activate');
    }));

    //it('should_have_firstname_with_same_value', fakeAsync(() => {
    //    const fixture = TestBed.createComponent(UserCreationComponent);
    //    const firstName = fixture.debugElement.query(By.css('input[name="firstname"]')).nativeElement;
    //    fixture.componentInstance.userDetail.firstname = "firstName";
    //    fixture.detectChanges();
    //    tick();
    //    console.log(firstName.value);
    //    expect(firstName.value).toEqual("firstName");
    //}));
});
