import { async, ComponentFixture, TestBed, fakeAsync, tick } from '@angular/core/testing';
import { AccountCreationComponent } from './account-creation.component';
import { FormsModule } from '@angular/forms';
import { HttpModule } from '@angular/http';
import { UserCreateService } from '../../service/user-create.service';
import {  BrowserModule, By,  } from '@angular/platform-browser';
import { RouterTestingModule } from '@angular/router/testing';
import { BrowserDynamicTestingModule, platformBrowserDynamicTesting } from '@angular/platform-browser-dynamic/testing';

describe('AccountCreationComponent', () => {
    let component: AccountCreationComponent;
    let fixture: ComponentFixture<AccountCreationComponent>;
    
    class MockUserCreateService {
    }
    beforeEach(async(() => {
        TestBed.resetTestEnvironment();
        TestBed.initTestEnvironment(BrowserDynamicTestingModule,
            platformBrowserDynamicTesting());
        TestBed.configureTestingModule({
            imports: [HttpModule, FormsModule, BrowserModule, RouterTestingModule],
            declarations: [AccountCreationComponent],
            providers: [
            { provide: UserCreateService, useClass: MockUserCreateService },
            { provide: 'window', useFactory: (() => { return { appcontext: { version: "333" } }; }) }
            ]
        }).compileComponents();
    }));
    it('should_have_user_creation_component', async(() => {
        fixture = TestBed.createComponent(AccountCreationComponent);
        component = fixture.debugElement.componentInstance;
        expect(component).toBeTruthy();
    }));
    it('should_have_account_creation_title_as_\'Create New Account\'', () => {
        fixture = TestBed.createComponent(AccountCreationComponent);
        let element = fixture.nativeElement;
        var title = element.querySelector('#user-creation-title').textContent;
        expect(title).toEqual(' Create New Account ');
    });
    it('should_have_firstname_empty_when_firstname_is_null', fakeAsync(() => {
        fixture = TestBed.createComponent(AccountCreationComponent);
        const firstName = fixture.debugElement.query(By.css('input[name="firstname"]')).nativeElement;
        fixture.componentInstance.userAccount.firstname = "";
        fixture.detectChanges();
        tick();
        expect(firstName.value).toEqual("");
    }));
    it('should_have_firstname_with_same_value', fakeAsync(() => {
        const fixture = TestBed.createComponent(AccountCreationComponent);
        const firstName = fixture.debugElement.query(By.css('input[name="firstname"]')).nativeElement;
        fixture.componentInstance.userAccount.firstname = "firstName";
        fixture.detectChanges();
        tick();
        console.log(firstName.value);
        expect(firstName.value).toEqual("firstName");
    }));
    it('should_have_middlename_empty_when_middlename_is_null', fakeAsync(() => {
        const fixture = TestBed.createComponent(AccountCreationComponent);
        const middleName = fixture.debugElement.query(By.css('input[name="middleName"]')).nativeElement
        fixture.componentInstance.userAccount.middleName = "";
        fixture.detectChanges();
        tick();
        expect(middleName.value).toEqual("");
    }));
    it('should_have_middlename_with_same_value', fakeAsync(() => {
        const fixture = TestBed.createComponent(AccountCreationComponent);
        const middleName = fixture.debugElement.query(By.css('input[name="middleName"]')).nativeElement
        fixture.componentInstance.userAccount.middleName = "middleName";
        fixture.detectChanges();
        tick();
        expect(middleName.value).toEqual("middleName");
    }));
    it('should_have_lastname_empty_when_lastname_is_null', fakeAsync(() => {
        fixture = TestBed.createComponent(AccountCreationComponent);
        const lastName = fixture.debugElement.query(By.css('input[name="lastName"]')).nativeElement
        fixture.componentInstance.userAccount.lastName = "";
        fixture.detectChanges();
        tick();
        expect(lastName.value).toEqual("");
    }));
    it('should_have_lastname_with_same_value', fakeAsync(() => {
        const fixture = TestBed.createComponent(AccountCreationComponent);
        const lastName = fixture.debugElement.query(By.css('input[name="lastName"]')).nativeElement
        fixture.componentInstance.userAccount.lastName = "lastName";
        fixture.detectChanges();
        tick();
        expect(lastName.value).toEqual("lastName");
    }));
    it('should_have_suffix_with_same_value', fakeAsync(() => {
        const fixture = TestBed.createComponent(AccountCreationComponent);
        const suffix = fixture.debugElement.query(By.css('input[name="suffix"]')).nativeElement
        fixture.componentInstance.userAccount.suffix = "Dr";
        fixture.detectChanges();
        tick();
        expect(suffix.value).toEqual("Dr");
    }));
    it('should_have_suffix_empty_when_suffix_is_null', fakeAsync(() => {
        fixture = TestBed.createComponent(AccountCreationComponent);
        const suffix = fixture.debugElement.query(By.css('input[name="lastName"]')).nativeElement
        fixture.componentInstance.userAccount.suffix = "";
        fixture.detectChanges();
        tick();
        expect(suffix.value).toEqual("");
    }));
    it('should_have_personal_email_empty_when_personal_email_is_null', fakeAsync(() => {
        fixture = TestBed.createComponent(AccountCreationComponent);
        const personalEmail = fixture.debugElement.query(By.css('input[name="personalEmail"]')).nativeElement
        fixture.componentInstance.userAccount.personalEmail = "";
        fixture.detectChanges();
        tick();
        expect(personalEmail.value).toEqual("");
    }));
    it('should_have_personal_email_with_same_value', fakeAsync(() => {
        fixture = TestBed.createComponent(AccountCreationComponent);
        const personalEmail = fixture.debugElement.query(By.css('input[name="personalEmail"]')).nativeElement
        fixture.componentInstance.userAccount.personalEmail = "test@allscripts.com";
        fixture.detectChanges();
        tick();
        expect(personalEmail.value).toEqual("test@allscripts.com");
    }));
  
});
