import { async, ComponentFixture, TestBed, fakeAsync, tick } from '@angular/core/testing';

import { UserCreationComponent } from './user-creation.component';
import { MaxTryFailureComponent } from '../max-try-failure-popup/max-try-failure.component';
import { FormsModule } from '@angular/forms';
import { HttpModule } from '@angular/http';
import { UserCreateService } from '../../service/user-create.service';
import { Observable, Observer } from 'rxjs';
import { LicenseCreateService } from '../../service/license-create.service';
import { SafeResourceUrl,DomSanitizer,BrowserModule, By, SafeUrl } from '@angular/platform-browser';
import { RouterTestingModule } from '@angular/router/testing';
import { SecretAnswers, State } from './user-creation.model';
import { Router } from '@angular/router';
import { BrowserDynamicTestingModule, platformBrowserDynamicTesting } from '@angular/platform-browser-dynamic/testing';

describe('UserCreationComponent', () => {
    let component: UserCreationComponent;
    let fixture: ComponentFixture<UserCreationComponent>;
    let stateList: State[];
    let response: any;
    let securityQuestions: any;
    
    class MockUserCreateService {

        getInitialPageData() {            
            return Observable.create((observer: Observer<State[]>) => {
                observer.next(stateList);
            });
        }

        getCaptcha() {
           return Observable.create((observer: Observer<any>) => {
               observer.next(response);
            });
        }
    }

    class MockLicenseCreateService {
        getStateList() {
            return Observable.create((observer: Observer<State[]>) => {
                observer.next(stateList);
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

    class MockDomSanitizer
    {
        bypassSecurityTrustHtml(value: string) {
            return 'safsString';
        }

        sanitize() { return 'safeString'; }
    }

    beforeEach(async(() => {
        TestBed.resetTestEnvironment();
        TestBed.initTestEnvironment(BrowserDynamicTestingModule,
            platformBrowserDynamicTesting());        
      TestBed.configureTestingModule({
          imports: [HttpModule, FormsModule, BrowserModule, RouterTestingModule],
          declarations: [UserCreationComponent, MaxTryFailureComponent],
          providers: [{ provide: LicenseCreateService, useClass: MockLicenseCreateService },     
              { provide:UserCreateService, useClass: MockUserCreateService },
              { provide: 'window', useFactory: (() => { return { appcontext: { version: "333" } }; }) }                        
          ]
      }).compileComponents();
  }));

    beforeEach(() => {
        stateList = [{
            state: "IL",
            description: "Illinos"
        },
        {
            state: "AL",
            description: "Alaska"
            }];

        securityQuestions = [{  },
        ]
    });
    it('should_have_user_creation_component', async(() => {
        fixture = TestBed.createComponent(UserCreationComponent);
        component = fixture.debugElement.componentInstance;
        expect(component).toBeTruthy();
    }));
    


    it('should_assign_true_if_npi_changed', () => {
        fixture = TestBed.createComponent(UserCreationComponent);
        component = fixture.componentInstance;
        component.onNpiChanged();
        expect(component.isValidNPI).toBeTruthy();
    });

    it('should_assign_true_if_DEA_changed', () => {
        fixture = TestBed.createComponent(UserCreationComponent);
        component = fixture.componentInstance;
        component.onDEAChanged();
        expect(component.isValidDEA).toBeTruthy();
        expect(component.isDEAExpDateRequired).toBeFalsy();
        expect(component.isDEAScheduleRequired).toBeFalsy();
    });


    it('should_return_secret_question_Answer', () => {
        let secretAnsweList: SecretAnswers[]
        fixture = TestBed.createComponent(UserCreationComponent);
        component = fixture.componentInstance;
        component.SelectedShieldQuestion1 = 1;
        component.SelectedShieldQuestion2 = 2;
        component.SelectedShieldQuestion3 = 3;
        component.userDetail.securityAnswer1 = "One";
        component.userDetail.securityAnswer2 = "Two";
        component.userDetail.securityAnswer3 = "Three";
        secretAnsweList = component.generateScretAnswer();
        expect(secretAnsweList.length).toEqual(3);
        expect(secretAnsweList[0].answer).toEqual("One");
        expect(secretAnsweList[1].answer).toEqual("Two");
        expect(secretAnsweList[2].answer).toEqual("Three");
    });
    it('should_have_user_creation_title_as_\'Create New Account\'', () => {
        fixture = TestBed.createComponent(UserCreationComponent);
        let element = fixture.nativeElement;
        var title = element.querySelector('#user-creation-title').textContent;
        expect(title).toEqual(' Create New Account ');
    });
    // validation whether the models(assign value in textbox and validate the same value in models)
    it('should_have_firstname_empty_when_firstname_is_null', fakeAsync(() => {        
        fixture = TestBed.createComponent(UserCreationComponent);
        const firstName = fixture.debugElement.query(By.css('input[name="firstname"]')).nativeElement;          
        fixture.componentInstance.userDetail.firstname = "";
        fixture.detectChanges();
        tick();
       expect(firstName.value).toEqual("");
    }));
    it('should_have_firstname_with_same_value', fakeAsync(() => {
        const fixture = TestBed.createComponent(UserCreationComponent);       
        const firstName = fixture.debugElement.query(By.css('input[name="firstname"]')).nativeElement;       
        fixture.componentInstance.userDetail.firstname = "firstName";
        fixture.detectChanges();       
        tick();
        console.log(firstName.value);
        expect(firstName.value).toEqual("firstName");
    }));
    it('should_have_middlename_empty_when_middlename_is_null', fakeAsync(() => {
        const fixture = TestBed.createComponent(UserCreationComponent);
        const middleName = fixture.debugElement.query(By.css('input[name="middleName"]')).nativeElement
        fixture.componentInstance.userDetail.middleName = "";
        fixture.detectChanges();
        tick();
        expect(middleName.value).toEqual("");
    }));
    it('should_have_middlename_with_same_value', fakeAsync(() => {
        const fixture = TestBed.createComponent(UserCreationComponent);
        const middleName = fixture.debugElement.query(By.css('input[name="middleName"]')).nativeElement
        fixture.componentInstance.userDetail.middleName = "middleName";
        fixture.detectChanges();
        tick();
        expect(middleName.value).toEqual("middleName");
    }));
    it('should_have_lastname_empty_when_lastname_is_null', fakeAsync(() => {
        fixture = TestBed.createComponent(UserCreationComponent);
        const lastName = fixture.debugElement.query(By.css('input[name="lastName"]')).nativeElement
        fixture.componentInstance.userDetail.lastName = "";
        fixture.detectChanges();
        tick();
        expect(lastName.value).toEqual("");
    }));
    it('should_have_lastname_with_same_value', fakeAsync(() => {
        const fixture = TestBed.createComponent(UserCreationComponent);
        const lastName = fixture.debugElement.query(By.css('input[name="lastName"]')).nativeElement
        fixture.componentInstance.userDetail.lastName = "lastName";
        fixture.detectChanges();
        tick();
        expect(lastName.value).toEqual("lastName");
    }));
    it('should_have_suffix_with_same_value', fakeAsync(() => {
        const fixture = TestBed.createComponent(UserCreationComponent);
        const suffix = fixture.debugElement.query(By.css('input[name="suffix"]')).nativeElement
        fixture.componentInstance.userDetail.suffix = "Dr";
        fixture.detectChanges();
        tick();
        expect(suffix.value).toEqual("Dr");
    }));
    it('should_have_suffix_empty_when_suffix_is_null', fakeAsync(() => {
        fixture = TestBed.createComponent(UserCreationComponent);
        const suffix = fixture.debugElement.query(By.css('input[name="lastName"]')).nativeElement
        fixture.componentInstance.userDetail.suffix = "";
        fixture.detectChanges();
        tick();
        expect(suffix.value).toEqual("");
    }));
    it('should_have_personal_email_empty_when_personal_email_is_null', fakeAsync(() => {
        fixture = TestBed.createComponent(UserCreationComponent);
        const personalEmail = fixture.debugElement.query(By.css('input[name="personalEmail"]')).nativeElement
        fixture.componentInstance.userDetail.personalEmail = "";
        fixture.detectChanges();
        tick();
        expect(personalEmail.value).toEqual("");
    }));
    it('should_have_personal_email_with_same_value', fakeAsync(() => {
        fixture = TestBed.createComponent(UserCreationComponent);
        const personalEmail = fixture.debugElement.query(By.css('input[name="personalEmail"]')).nativeElement
        fixture.componentInstance.userDetail.personalEmail = "test@allscripts.com";
        fixture.detectChanges();
        tick();
        expect(personalEmail.value).toEqual("test@allscripts.com");
    }));
    it('should_have_home_address_empty_when_home_address_is_null', fakeAsync(() => {
        fixture = TestBed.createComponent(UserCreationComponent);
        const homeAddress = fixture.debugElement.query(By.css('input[name="HomeAddress"]')).nativeElement
        fixture.componentInstance.userDetail.HomeAddress = "";
        fixture.detectChanges();
        tick();
        expect(homeAddress.value).toEqual("");
    }));
    it('should_have_home_address_with_same_value', fakeAsync(() => {
        fixture = TestBed.createComponent(UserCreationComponent);
        const homeAddress = fixture.debugElement.query(By.css('input[name="HomeAddress"]')).nativeElement
        fixture.componentInstance.userDetail.HomeAddress = "Home Address";
        fixture.detectChanges();
        tick();
        expect(homeAddress.value).toEqual("Home Address");
    }));
    it('should_have_home_address2_empty_when_home_address2_is_null', fakeAsync(() => {
        fixture = TestBed.createComponent(UserCreationComponent);
        const homeAddress2 = fixture.debugElement.query(By.css('input[name="HomeAddress2"]')).nativeElement
        fixture.componentInstance.userDetail.HomeAddress2 = "";
        fixture.detectChanges();
        tick();
        expect(homeAddress2.value).toEqual("");
    }));
    it('should_have_home_address2_with_same_value', fakeAsync(() => {
        fixture = TestBed.createComponent(UserCreationComponent);
        const homeAddress2 = fixture.debugElement.query(By.css('input[name="HomeAddress2"]')).nativeElement
        fixture.componentInstance.userDetail.HomeAddress2 = "Home Address2";
        fixture.detectChanges();
        tick();
        expect(homeAddress2.value).toEqual("Home Address2");
    }));
    it('should_have_city_empty_when_city_is_null', fakeAsync(() => {
        fixture = TestBed.createComponent(UserCreationComponent);
        const city = fixture.debugElement.query(By.css('input[name="City"]')).nativeElement
        fixture.componentInstance.userDetail.city = "";
        fixture.detectChanges();
        tick();
        expect(city.value).toEqual("");
    }));
    xit('should_have_city_with_same_value', fakeAsync(() => {
        const fixture = TestBed.createComponent(UserCreationComponent);
        const city = fixture.debugElement.query(By.css('input[name="City"]')).nativeElement        
        fixture.componentInstance.userDetail.city = "City";
        fixture.detectChanges();
        tick();
        expect(city.value).toEqual("City");
    }));
    it('should_not_have_an_error_message_when_state_is_not_null', fakeAsync(() => {
        const fixture = TestBed.createComponent(UserCreationComponent);       
        fixture.detectChanges();
        fixture.whenStable().then(() => {
            fixture.componentInstance.states = stateList;
            fixture.detectChanges();            
            const state = fixture.debugElement.query(By.css('select[name="State"]')).nativeElement;            
            state.value = state.options[1].value;
            fixture.detectChanges();
            tick();                         
            expect(state.value).toEqual("IL");
            expect(fixture.debugElement.nativeElement.firstChild[4].validationMessage).toEqual("");
        });
    }));
    it('should_have_error_message_when_state_is_null', async(() => {
        const fixture = TestBed.createComponent(UserCreationComponent);
        fixture.whenStable().then(() => {
            fixture.componentInstance.states = stateList;
            fixture.detectChanges();
            const state = fixture.debugElement.query(By.css('select[name="State"]')).nativeElement;
            state.dispatchEvent(new Event('change'));
            fixture.detectChanges();
            expect(state.value).toEqual("0: undefined");
            expect(fixture.debugElement.nativeElement.firstChild[4].validationMessage).toEqual("");
        });
    }));
    it('should_have_zipcode_empty_when_zipcode_is_null', fakeAsync(() => {
        const fixture = TestBed.createComponent(UserCreationComponent);
        const zipCode = fixture.debugElement.query(By.css('input[name="zipCode"]')).nativeElement
        fixture.componentInstance.userDetail.zipCode = "";
        fixture.detectChanges();
        tick();
        expect(zipCode.value).toEqual("");
    }));
    it('should_have_zipcode_in_specified_format', fakeAsync(() => {
        const fixture = TestBed.createComponent(UserCreationComponent);
        const zipCode = fixture.debugElement.query(By.css('input[name="zipCode"]')).nativeElement
        fixture.componentInstance.userDetail.zipCode = "12345";
        fixture.detectChanges();
        tick();
        expect(zipCode.value).toEqual("12345");
    }));    
    it('should_have_npi_empty_when_npi_is_null', fakeAsync(() => {
        fixture = TestBed.createComponent(UserCreationComponent);
        const npi = fixture.debugElement.query(By.css('input[name="npi"]')).nativeElement
        fixture.componentInstance.userDetail.npi = "";
        fixture.detectChanges();
        tick();
        expect(npi.value).toEqual("");
    }));
    it('should_have_npi_with_same_value', fakeAsync(() => {
        const fixture = TestBed.createComponent(UserCreationComponent);        
        const npi = fixture.debugElement.query(By.css('input[name="npi"]')).nativeElement        
        fixture.componentInstance.userDetail.npi = "1234567890";
        fixture.detectChanges();
        tick();
        expect(npi.value).toEqual("1234567890");
    }));
    it('should_have_dea_empty_when_dea_is_null', fakeAsync(() => {
        fixture = TestBed.createComponent(UserCreationComponent);
        const dea = fixture.debugElement.query(By.css('input[name="deaNumber"]')).nativeElement
        fixture.componentInstance.userDetail.deaNumber = "";
        fixture.detectChanges();
        tick();
        expect(dea.value).toEqual("");
    }));
    it('should_have_dea_with_same_value', fakeAsync(() => {
        const fixture = TestBed.createComponent(UserCreationComponent);
        const dea = fixture.debugElement.query(By.css('input[name="deaNumber"]')).nativeElement
        fixture.componentInstance.userDetail.deaNumber = "1234567890";
        fixture.detectChanges();
        tick();
        expect(dea.value).toEqual("1234567890");
    }));

    it('should_have_phonenumber_empty_when_phone_number_is_null', fakeAsync(() => {
        const fixture = TestBed.createComponent(UserCreationComponent);
        const phoneNumber = fixture.debugElement.query(By.css('input[name="contactPhoneNumber"]')).nativeElement
        fixture.componentInstance.userDetail.contactPhoneNumber = "";
        fixture.detectChanges();
        tick();
        expect(phoneNumber.value).toEqual("");
    }));
    it('should_have_phonenumber_in_specified_format', fakeAsync(() => {
        const fixture = TestBed.createComponent(UserCreationComponent);
        const phoneNumber = fixture.debugElement.query(By.css('input[name="contactPhoneNumber"]')).nativeElement
        fixture.componentInstance.userDetail.contactPhoneNumber = "2457498789";
        fixture.detectChanges();
        tick();
        expect(phoneNumber.value).toEqual("2457498789");
    }));
    it('should_have_phonenumber_patternMismatch_false_if_the_phonenumber_is_empty', fakeAsync(() => {
        fixture = TestBed.createComponent(UserCreationComponent);
        fixture.componentInstance.userDetail.contactPhoneNumber = "";
        fixture.detectChanges();
        tick();
        expect(fixture.nativeElement.firstElementChild.txtContactPhoneNumber.validity.patternMismatch).toBeFalsy();
        expect(fixture.nativeElement.firstElementChild.txtContactPhoneNumber.validationMessage).toEqual("Please fill out this field.");
    }));
    it('should_have_phonenumber_patternMismatch_true_if_the_phonenumber_is_invalid_format', fakeAsync(() => {
        fixture = TestBed.createComponent(UserCreationComponent);
        fixture.componentInstance.userDetail.contactPhoneNumber = "12345";
        fixture.detectChanges();
        tick();
        expect(fixture.nativeElement.firstElementChild.txtContactPhoneNumber.validity.patternMismatch).toBeTruthy();
    }));
    it('should_have_phonenumber_patternMismatch_false_if_the_phonenumber_is_valid_format', fakeAsync(() => {
        fixture = TestBed.createComponent(UserCreationComponent);
        fixture.componentInstance.userDetail.contactPhoneNumber = "1234567890";
        fixture.detectChanges();
        tick();
        expect(fixture.nativeElement.firstElementChild.txtContactPhoneNumber.validity.patternMismatch).toBeFalsy();
    }));

    it('should_have_phonenumber_required_to_true', fakeAsync(() => {
        fixture = TestBed.createComponent(UserCreationComponent);
        fixture.componentInstance.userDetail.contactPhoneNumber = "";
        fixture.detectChanges();
        tick();
        expect(fixture.nativeElement.firstElementChild.elements.txtContactPhoneNumber.maxLength).toEqual(14);
        expect(fixture.nativeElement.firstElementChild.elements.txtContactPhoneNumber.validationMessage).toEqual("Please fill out this field.");
        expect(fixture.nativeElement.firstElementChild.elements.txtContactPhoneNumber.required).toBeTruthy();
    }));
    it('should_have_error_message_when_phonenumber_is_invalid', async(() => {
        const fixture = TestBed.createComponent(UserCreationComponent);
        fixture.detectChanges();
        fixture.whenStable().then(() => {
            const phonenumber = fixture.debugElement.query(By.css('input[name="contactPhoneNumber"]')).nativeElement;
            phonenumber.dispatchEvent(new Event('input'));
            phonenumber.value = '987456';
            phonenumber.dispatchEvent(new Event('blur'));
            fixture.detectChanges();
            expect(fixture.debugElement.nativeElement.firstChild[6].classList[1]).toEqual("ng-touched");
            expect(fixture.debugElement.nativeElement.firstChild[6].classList[2]).toEqual("ng-dirty");
            expect(fixture.debugElement.nativeElement.firstChild[6].classList[3]).toEqual("ng-invalid");
            expect(fixture.debugElement.query(By.css('span[id="contactPhoneNumberError"]')).nativeElement.textContent).toEqual(" Enter a valid contact phone number. ");
        });
    }));
    it('should_not_have_error_message_when_phonenumber_is_valid', async(() => {
        const fixture = TestBed.createComponent(UserCreationComponent);
        fixture.detectChanges();
        fixture.whenStable().then(() => {
            const phonenumber = fixture.debugElement.query(By.css('input[name="contactPhoneNumber"]')).nativeElement;
            phonenumber.dispatchEvent(new Event('input'));
            phonenumber.value = '1234567890';
            phonenumber.dispatchEvent(new Event('blur'));
            phonenumber.dispatchEvent(new Event('compositionstart'));
            phonenumber.dispatchEvent(new Event('compositionend'));
            fixture.detectChanges();
            expect(fixture.debugElement.nativeElement.firstChild[6].classList[1]).toEqual("ng-valid");
            expect(fixture.debugElement.nativeElement.firstChild[6].classList[2]).toEqual("ng-touched");
            expect(fixture.debugElement.nativeElement.firstChild[6].classList[3]).toEqual("ng-dirty");
        });
    }));
});
