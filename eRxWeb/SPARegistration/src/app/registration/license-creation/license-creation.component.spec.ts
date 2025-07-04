import { ComponentFixture, TestBed, fakeAsync, tick, async } from '@angular/core/testing';
import { LicenseCreationComponent } from './license-creation.component';
import { LicenseCreateService } from '../../service/license-create.service';
import { BrowserDynamicTestingModule, platformBrowserDynamicTesting } from '@angular/platform-browser-dynamic/testing';
import { FormsModule, NgForm } from '@angular/forms';
import { UserCreateService } from '../../service/user-create.service';
import { Observable, Observer } from 'rxjs';
import { StateInfo } from './license-creation.model';
import { By } from '@angular/platform-browser';
import { LoaderService } from '../../service/loader.service';
const stateList: any =
    [
        { state: "IL", description: "Illinos" },
        { state: 'AL', description: 'Alaska' }
    ];

class LicenseCreationStubService { }
class UserCreationStubService {
    getInitialPageData() {
        return Observable.create((observer: Observer<StateInfo[]>) => {
            observer.next(stateList);
        });
    }
}
class LoaderStubService {
    overrideLoading() {
        return Observable.create((observer: Observer<boolean>) => {
            observer.next(false);
        });
    }
}

function windowStubFactory() { return { appcontext: { version: "333" } }; }
let component: LicenseCreationComponent;
let fixture: ComponentFixture<LicenseCreationComponent>;
describe('LicenseCreationComponent', () => {
    beforeEach(async () => {
        TestBed.resetTestEnvironment();
        TestBed.initTestEnvironment(BrowserDynamicTestingModule, platformBrowserDynamicTesting());

        TestBed.configureTestingModule({
            imports: [FormsModule],
            declarations: [LicenseCreationComponent],
            providers: [{ provide: LicenseCreateService, useClass: LicenseCreationStubService },
            { provide: UserCreateService, useClass: UserCreationStubService },
            { provide: LoaderService, useClass: LoaderStubService },
            { provide: 'window', useFactory: windowStubFactory }]
        })
            .compileComponents();
    });
    it('should_have_License_creation_component', async(() => {
        fixture = TestBed.createComponent(LicenseCreationComponent);
        component = fixture.debugElement.componentInstance;
        expect(component).toBeTruthy();
    }));
    it('should_have_license_creation_title_as_\'Practice Primary Location Information\'', () => {
        fixture = TestBed.createComponent(LicenseCreationComponent);
        let element = fixture.nativeElement;
        var title = element.querySelector('#license-creation-title').textContent;
        expect(title).toEqual(' Practice Primary Location Information ');
    });
    // validation whether the models(assign value in textbox and validate the same value in models)
    it('should_have_practicename_empty_when_practicename_is_null', fakeAsync(() => {
        fixture = TestBed.createComponent(LicenseCreationComponent);
        const practiceName = fixture.debugElement.query(By.css('input[name="SiteName"]')).nativeElement
        fixture.componentInstance.licenseInfo.PracticeName = "";
        fixture.detectChanges();
        tick();
        expect(practiceName.value).toEqual("");
    }));
    it('should_have_practicename_with_same_value', fakeAsync(() => {
        const fixture = TestBed.createComponent(LicenseCreationComponent);
        const practiceName = fixture.debugElement.query(By.css('input[name="SiteName"]')).nativeElement
        fixture.componentInstance.licenseInfo.PracticeName = "PracticeName";
        fixture.detectChanges();
        tick();
        expect(practiceName.value).toEqual("PracticeName");
    }));
    it('should_have_Address1_empty_when_Address1_is_null', fakeAsync(() => {
        const fixture = TestBed.createComponent(LicenseCreationComponent);
        const address1 = fixture.debugElement.query(By.css('input[name="Address1"]')).nativeElement
        fixture.componentInstance.licenseInfo.Address = "";
        fixture.detectChanges();
        tick();
        expect(address1.value).toEqual("");
    }));
    it('should_have_Address1_with_specified_value', fakeAsync(() => {
        const fixture = TestBed.createComponent(LicenseCreationComponent);
        const address1 = fixture.debugElement.query(By.css('input[name="Address1"]')).nativeElement
        fixture.componentInstance.licenseInfo.Address = "Address1";
        fixture.detectChanges();
        tick();
        expect(address1.value).toEqual("Address1");
    }));
    it('should_have_Address2_empty_when_Address2_is_null', fakeAsync(() => {
        const fixture = TestBed.createComponent(LicenseCreationComponent);
        const address2 = fixture.debugElement.query(By.css('input[name="Address2"]')).nativeElement
        fixture.componentInstance.licenseInfo.Address2 = "";
        fixture.detectChanges();
        tick();
        expect(address2.value).toEqual("");
    }));
    it('should_have_Address2_with_specified_value', fakeAsync(() => {
        const fixture = TestBed.createComponent(LicenseCreationComponent);
        const address2 = fixture.debugElement.query(By.css('input[name="Address2"]')).nativeElement
        fixture.componentInstance.licenseInfo.Address2 = "Address2";
        fixture.detectChanges();
        tick();
        expect(address2.value).toEqual("Address2");
    }));
    it('should_have_city_empty_when_city_is_null', fakeAsync(() => {
        const fixture = TestBed.createComponent(LicenseCreationComponent);
        const city = fixture.debugElement.query(By.css('input[name="City"]')).nativeElement
        fixture.componentInstance.licenseInfo.City = "";
        fixture.detectChanges();
        tick();
        expect(city.value).toEqual("");
    }));
    it('should_have_city_with_specified_value', fakeAsync(() => {
        const fixture = TestBed.createComponent(LicenseCreationComponent);
        const city = fixture.debugElement.query(By.css('input[name="City"]')).nativeElement
        fixture.componentInstance.licenseInfo.City = "City";
        fixture.detectChanges();
        tick();
        expect(city.value).toEqual("City");
    }));
    it('should_have_phonenumber_empty_when_phone_number_is_null', fakeAsync(() => {
        const fixture = TestBed.createComponent(LicenseCreationComponent);
        const phoneNumber = fixture.debugElement.query(By.css('input[name="Phone"]')).nativeElement
        fixture.componentInstance.licenseInfo.PhoneNumber = "";
        fixture.detectChanges();
        tick();
        expect(phoneNumber.value).toEqual("");
    }));
    it('should_have_phonenumber_in_specified_format', fakeAsync(() => {
        const fixture = TestBed.createComponent(LicenseCreationComponent);
        const phoneNumber = fixture.debugElement.query(By.css('input[name="Phone"]')).nativeElement
        fixture.componentInstance.licenseInfo.PhoneNumber = "2457498789";
        fixture.detectChanges();
        tick();
        expect(phoneNumber.value).toEqual("2457498789");
    }));
    it('should_have_zipcode_empty_when_zipcode_is_null', fakeAsync(() => {
        const fixture = TestBed.createComponent(LicenseCreationComponent);
        const zipCode = fixture.debugElement.query(By.css('input[name="Zip"]')).nativeElement
        fixture.componentInstance.licenseInfo.Zipcode = "";
        fixture.detectChanges();
        tick();
        expect(zipCode.value).toEqual("");
    }));
    it('should_have_zipcode_in_specified_format', fakeAsync(() => {
        const fixture = TestBed.createComponent(LicenseCreationComponent);
        const zipCode = fixture.debugElement.query(By.css('input[name="Zip"]')).nativeElement
        fixture.componentInstance.licenseInfo.Zipcode = "12345";
        fixture.detectChanges();
        tick();
        expect(zipCode.value).toEqual("12345");
    }));
    it('should_have_faxnumber_empty_when_faxnumber_is_null', fakeAsync(() => {
        const fixture = TestBed.createComponent(LicenseCreationComponent);
        const faxnumber = fixture.debugElement.query(By.css('input[name="Fax"]')).nativeElement
        fixture.componentInstance.licenseInfo.FaxNumber = "";
        fixture.detectChanges();
        tick();
        expect(faxnumber.value).toEqual("");
    }));
    it('should_have_faxnumber_in_specified_format', fakeAsync(() => {
        const fixture = TestBed.createComponent(LicenseCreationComponent);
        const faxnumber = fixture.debugElement.query(By.css('input[name="Fax"]')).nativeElement
        fixture.componentInstance.licenseInfo.FaxNumber = "12345";
        fixture.detectChanges();
        tick();
        expect(faxnumber.value).toEqual("12345");
    }));
    // Form Validation(Form.valid)
    it('should_have_form_valid_true_when_required_models_are_passed', fakeAsync(() => {
        fixture = TestBed.createComponent(LicenseCreationComponent);
        fixture.componentInstance.licenseInfo.PracticeName = "PracticeName";
        fixture.componentInstance.licenseInfo.Address = "Address";
        fixture.componentInstance.licenseInfo.Address2 = "";
        fixture.componentInstance.licenseInfo.City = "City";
        fixture.componentInstance.licenseInfo.FaxNumber = "2457498789";
        fixture.componentInstance.licenseInfo.PhoneNumber = "2457498789";
        fixture.componentInstance.licenseInfo.Zipcode = "12345";
        fixture.componentInstance.licenseInfo.State = "Illinos";
        fixture.detectChanges();
        tick();
        const form = fixture.debugElement.children[0].injector.get(NgForm);
        expect(form.value.SiteName).toEqual("PracticeName");
        expect(form.value.Address1).toEqual("Address");
        expect(form.value.Address2).toEqual("");
        expect(form.value.City).toEqual("City");
        expect(form.value.Fax).toEqual("2457498789");
        expect(form.value.Phone).toEqual("2457498789");
        expect(form.value.Zip).toEqual("12345");
        expect(form.value.State).toEqual("Illinos");
        expect(form.valid).toBeTruthy();
    }));
    it('should_have_form_valid_false_when_state_is_missed', fakeAsync(() => {
        fixture = TestBed.createComponent(LicenseCreationComponent);
        fixture.componentInstance.licenseInfo.PracticeName = "PracticeName";
        fixture.componentInstance.licenseInfo.Address = "Address";
        fixture.componentInstance.licenseInfo.Address2 = "";
        fixture.componentInstance.licenseInfo.City = "City";
        fixture.componentInstance.licenseInfo.FaxNumber = "2457498789";
        fixture.componentInstance.licenseInfo.PhoneNumber = "2457498789";
        fixture.componentInstance.licenseInfo.Zipcode = "12345";
        fixture.detectChanges();
        tick();
        const form = fixture.debugElement.children[0].injector.get(NgForm);
        expect(form.value.SiteName).toEqual("PracticeName");
        expect(form.value.Address1).toEqual("Address");
        expect(form.value.Address2).toEqual("");
        expect(form.value.City).toEqual("City");
        expect(form.value.Fax).toEqual("2457498789");
        expect(form.value.Phone).toEqual("2457498789");
        expect(form.value.Zip).toEqual("12345");
        expect(form.valid).toBeFalsy();
    }));

    // Pattern validation for Fax,Phone,Zip
    it('should_have_zipcode_patternMismatch_flase_if_the_zipcode_is_valid', fakeAsync(() => {
        fixture = TestBed.createComponent(LicenseCreationComponent);
        fixture.componentInstance.licenseInfo.Zipcode = "12345";
        fixture.detectChanges();
        tick();
        expect(fixture.nativeElement.firstElementChild.txtZip.validity.patternMismatch).toBeFalsy();
        expect(fixture.nativeElement.firstElementChild.txtZip.validationMessage).toEqual('');
    }));

    it('should_have_zipcode_patternMismatch_true_if_the_zipcode_contains_letters', fakeAsync(() => {
        fixture = TestBed.createComponent(LicenseCreationComponent);
        fixture.componentInstance.licenseInfo.Zipcode = "TEST";
        fixture.detectChanges();
        tick();
        expect(fixture.nativeElement.firstElementChild.txtZip.validity.patternMismatch).toBeTruthy();
        expect(fixture.nativeElement.firstElementChild.txtZip.validationMessage).toEqual("Please match the requested format.");
    }));

    it('should_have_zipcode_patternMismatch_false_if_the_zipcode_is_null', fakeAsync(() => {
        fixture = TestBed.createComponent(LicenseCreationComponent);
        fixture.componentInstance.licenseInfo.Zipcode = "";
        fixture.detectChanges();
        tick();
        expect(fixture.nativeElement.firstElementChild.txtZip.validity.patternMismatch).toBeFalsy();
        expect(fixture.nativeElement.firstElementChild.txtZip.validationMessage).toEqual("Please fill out this field.");
    }));

    it('should_have_zipcode_patternMismatch_true_if_the_zipcode_length_is_not_5_or_9', fakeAsync(() => {
        fixture = TestBed.createComponent(LicenseCreationComponent);
        fixture.componentInstance.licenseInfo.Zipcode = "123456789686";
        fixture.detectChanges();
        tick();
        expect(fixture.nativeElement.firstElementChild.txtZip.validity.patternMismatch).toBeTruthy();
    }));
    it('should_have_faxumber_patternMismatch_true_if_the_faxnumber_is_invalid_format', fakeAsync(() => {
        fixture = TestBed.createComponent(LicenseCreationComponent);
        fixture.componentInstance.licenseInfo.FaxNumber = "12345";
        const pattern = fixture.nativeElement.firstChild.txtFax.pattern;
        fixture.detectChanges();
        tick();
        expect(fixture.nativeElement.firstElementChild.txtFax.validity.patternMismatch).toBeTruthy();
    }));
    it('should_have_Faxnumber_patternMismatch_false_if_the_faxnumber_is_empty', fakeAsync(() => {
        fixture = TestBed.createComponent(LicenseCreationComponent);
        fixture.componentInstance.licenseInfo.FaxNumber = "";
        fixture.detectChanges();
        tick();
        expect(fixture.nativeElement.firstElementChild.txtFax.validity.patternMismatch).toBeFalsy();
        expect(fixture.nativeElement.firstElementChild.txtFax.validationMessage).toEqual("Please fill out this field.");
    }));
    it('should_have_faxnumber_patternMismatch_false_if_the_faxnumber_is_valid_format', fakeAsync(() => {
        fixture = TestBed.createComponent(LicenseCreationComponent);
        fixture.componentInstance.licenseInfo.FaxNumber = "1234567890";
        fixture.detectChanges();
        tick();
        expect(fixture.nativeElement.firstElementChild.txtFax.validity.patternMismatch).toBeFalsy();
    }));
    it('should_have_phonenumber_patternMismatch_false_if_the_phonenumber_is_empty', fakeAsync(() => {
        fixture = TestBed.createComponent(LicenseCreationComponent);
        fixture.componentInstance.licenseInfo.PhoneNumber = "";
        fixture.detectChanges();
        tick();
        expect(fixture.nativeElement.firstElementChild.txtPhone.validity.patternMismatch).toBeFalsy();
        expect(fixture.nativeElement.firstElementChild.txtPhone.validationMessage).toEqual("Please fill out this field.");
    }));
    it('should_have_phonenumber_patternMismatch_true_if_the_phonenumber_is_invalid_format', fakeAsync(() => {
        fixture = TestBed.createComponent(LicenseCreationComponent);
        fixture.componentInstance.licenseInfo.PhoneNumber = "12345";
        fixture.detectChanges();
        tick();
        expect(fixture.nativeElement.firstElementChild.txtPhone.validity.patternMismatch).toBeTruthy();
    }));
    it('should_have_phonenumber_patternMismatch_false_if_the_phonenumber_is_valid_format', fakeAsync(() => {
        fixture = TestBed.createComponent(LicenseCreationComponent);
        fixture.componentInstance.licenseInfo.PhoneNumber = "1234567890";
        fixture.detectChanges();
        tick();
        expect(fixture.nativeElement.firstElementChild.txtPhone.validity.patternMismatch).toBeFalsy();
    }));

    // validates required, maxlength, default validation message
    it('should_have_practicename_required_to_true', fakeAsync(() => {
        fixture = TestBed.createComponent(LicenseCreationComponent);
        fixture.componentInstance.licenseInfo.PracticeName = "";
        fixture.detectChanges();
        tick();
        expect(fixture.nativeElement.firstElementChild.elements.SiteName.maxLength).toEqual(70);
        expect(fixture.nativeElement.firstElementChild.elements.SiteName.validationMessage).toEqual("Please fill out this field.");
        expect(fixture.nativeElement.firstElementChild.elements.SiteName.required).toBeTruthy();
    }));
    it('should_have_address1_required_to_true', fakeAsync(() => {
        fixture = TestBed.createComponent(LicenseCreationComponent);
        fixture.componentInstance.licenseInfo.Address = "";
        fixture.detectChanges();
        tick();
        expect(fixture.nativeElement.firstElementChild.elements.txtAddress1.maxLength).toEqual(35);
        expect(fixture.nativeElement.firstElementChild.elements.txtAddress1.validationMessage).toEqual("Please fill out this field.");
        expect(fixture.nativeElement.firstElementChild.elements.txtAddress1.required).toBeTruthy();
    }));
    it('should_have_address2_required_to_false', fakeAsync(() => {
        fixture = TestBed.createComponent(LicenseCreationComponent);
        fixture.componentInstance.licenseInfo.Address2 = "";
        fixture.detectChanges();
        tick();
        expect(fixture.nativeElement.firstElementChild.elements.txtAddress2.maxLength).toEqual(35);
        expect(fixture.nativeElement.firstElementChild.elements.txtAddress2.validationMessage).toEqual("");
        expect(fixture.nativeElement.firstElementChild.elements.txtAddress2.required).toBeFalsy();
    }));
    it('should_have_city_required_to_true', fakeAsync(() => {
        fixture = TestBed.createComponent(LicenseCreationComponent);
        fixture.componentInstance.licenseInfo.City = "";
        fixture.detectChanges();
        tick();
        expect(fixture.nativeElement.firstElementChild.elements.City.maxLength).toEqual(20);
        expect(fixture.nativeElement.firstElementChild.elements.City.validationMessage).toEqual("Please fill out this field.");
        expect(fixture.nativeElement.firstElementChild.elements.City.required).toBeTruthy();
    }));
    it('should_have_zipcode_required_to_true', fakeAsync(() => {
        fixture = TestBed.createComponent(LicenseCreationComponent);
        fixture.componentInstance.licenseInfo.Zipcode = "";
        fixture.detectChanges();
        tick();
        expect(fixture.nativeElement.firstElementChild.elements.Zip.maxLength).toEqual(9);
        expect(fixture.nativeElement.firstElementChild.elements.Zip.validationMessage).toEqual("Please fill out this field.");
        expect(fixture.nativeElement.firstElementChild.elements.Zip.required).toBeTruthy();
    }));
    it('should_have_phonenumber_required_to_true', fakeAsync(() => {
        fixture = TestBed.createComponent(LicenseCreationComponent);
        fixture.componentInstance.licenseInfo.PhoneNumber = "";
        fixture.detectChanges();
        tick();
        expect(fixture.nativeElement.firstElementChild.elements.Phone.maxLength).toEqual(14);
        expect(fixture.nativeElement.firstElementChild.elements.Phone.validationMessage).toEqual("Please fill out this field.");
        expect(fixture.nativeElement.firstElementChild.elements.Phone.required).toBeTruthy();
    }));
    it('should_have_faxnumber_required_to_true', fakeAsync(() => {
        fixture = TestBed.createComponent(LicenseCreationComponent);
        fixture.componentInstance.licenseInfo.FaxNumber = "";
        fixture.detectChanges();
        tick();
        expect(fixture.nativeElement.firstElementChild.elements.Fax.maxLength).toEqual(14);
        expect(fixture.nativeElement.firstElementChild.elements.Fax.validationMessage).toEqual("Please fill out this field.");
        expect(fixture.nativeElement.firstElementChild.elements.Fax.required).toBeTruthy();
    }));
    // Validating ng-valid/ng-invalid  -- ng-touched/ng-untouched --ng-pristine/ng-dirty
    it('should_have_error_message_when_phonenumber_is_invalid', async(() => {
        const fixture = TestBed.createComponent(LicenseCreationComponent);
        fixture.detectChanges();
        fixture.whenStable().then(() => {
            const phonenumber = fixture.debugElement.query(By.css('input[name="Phone"]')).nativeElement;
            phonenumber.dispatchEvent(new Event('input'));
            phonenumber.value = '987456';
            phonenumber.dispatchEvent(new Event('blur'));
            fixture.detectChanges();
            expect(fixture.debugElement.nativeElement.firstChild[6].classList[1]).toEqual("ng-touched");
            expect(fixture.debugElement.nativeElement.firstChild[6].classList[2]).toEqual("ng-dirty");
            expect(fixture.debugElement.nativeElement.firstChild[6].classList[3]).toEqual("ng-invalid");
            expect(fixture.debugElement.query(By.css('span[id="phoneMandatoryError"]')).nativeElement.textContent).toEqual(" Enter a valid phone number. ");
        });
    }));
    it('should_not_have_error_message_when_phonenumber_is_valid', async(() => {
        const fixture = TestBed.createComponent(LicenseCreationComponent);
        fixture.detectChanges();
        fixture.whenStable().then(() => {
            const phonenumber = fixture.debugElement.query(By.css('input[name="Phone"]')).nativeElement;
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

    it('should_have_error_message_when_faxnumber_is_invalid', async(() => {
        const fixture = TestBed.createComponent(LicenseCreationComponent);
        fixture.detectChanges();
        fixture.whenStable().then(() => {
            const faxnumber = fixture.debugElement.query(By.css('input[name="Fax"]')).nativeElement;
            faxnumber.dispatchEvent(new Event('input'));
            faxnumber.value = '987456';
            faxnumber.dispatchEvent(new Event('blur'));
            faxnumber.dispatchEvent(new Event('compositionstart'));
            faxnumber.dispatchEvent(new Event('compositionend'));
            fixture.detectChanges();
            expect(fixture.debugElement.nativeElement.firstChild[7].classList[1]).toEqual("ng-touched");
            expect(fixture.debugElement.nativeElement.firstChild[7].classList[2]).toEqual("ng-dirty");
            expect(fixture.debugElement.nativeElement.firstChild[7].classList[3]).toEqual("ng-invalid");
            expect(fixture.debugElement.query(By.css('span[id="faxMandatoryError"]')).nativeElement.textContent).toEqual(" Enter a valid fax number ");
        });
    }));
    it('should_not_have_error_message_when_faxnumber_is_valid', async(() => {
        const fixture = TestBed.createComponent(LicenseCreationComponent);
        fixture.detectChanges();
        fixture.whenStable().then(() => {
            const faxnumber = fixture.debugElement.query(By.css('input[name="Fax"]')).nativeElement;
            faxnumber.dispatchEvent(new Event('input'));
            faxnumber.value = '1234567890';
            faxnumber.dispatchEvent(new Event('blur'));
            faxnumber.dispatchEvent(new Event('compositionstart'));
            faxnumber.dispatchEvent(new Event('compositionend'));
            fixture.detectChanges();
            expect(fixture.debugElement.nativeElement.firstChild[7].classList[1]).toEqual("ng-valid");
            expect(fixture.debugElement.nativeElement.firstChild[7].classList[2]).toEqual("ng-touched");
            expect(fixture.debugElement.nativeElement.firstChild[7].classList[3]).toEqual("ng-dirty");
        });
    }));
    it('should_have_error_message_when_zipnumber_is_invalid', async(() => {
        const fixture = TestBed.createComponent(LicenseCreationComponent);
        fixture.detectChanges();
        fixture.whenStable().then(() => {
            const zip = fixture.debugElement.query(By.css('input[name="Zip"]')).nativeElement;
            zip.dispatchEvent(new Event('input'));
            zip.value = '987456';
            zip.dispatchEvent(new Event('blur'));
            zip.dispatchEvent(new Event('compositionstart'));
            zip.dispatchEvent(new Event('compositionend'));
            fixture.detectChanges();
            expect(fixture.debugElement.nativeElement.firstChild[5].classList[1]).toEqual("ng-touched");
            expect(fixture.debugElement.nativeElement.firstChild[5].classList[2]).toEqual("ng-dirty");
            expect(fixture.debugElement.nativeElement.firstChild[5].classList[3]).toEqual("ng-invalid");
            expect(fixture.debugElement.query(By.css('span[id="zipCodePatternError"]')).nativeElement.textContent).toEqual(" Enter a valid 5 or 9 digit ZIP code ");
        });
    }));
    it('should_not_have_error_message_when_faxnumber_is_valid', async(() => {
        const fixture = TestBed.createComponent(LicenseCreationComponent);
        fixture.detectChanges();
        fixture.whenStable().then(() => {
            const zip = fixture.debugElement.query(By.css('input[name="Zip"]')).nativeElement;
            zip.dispatchEvent(new Event('input'));
            zip.value = '12345';
            zip.dispatchEvent(new Event('blur'));
            zip.dispatchEvent(new Event('compositionstart'));
            zip.dispatchEvent(new Event('compositionend'));
            fixture.detectChanges();
            expect(fixture.debugElement.nativeElement.firstChild[5].classList[1]).toEqual("ng-valid");
            expect(fixture.debugElement.nativeElement.firstChild[5].classList[2]).toEqual("ng-touched");
            expect(fixture.debugElement.nativeElement.firstChild[5].classList[3]).toEqual("ng-dirty");
        });
    }));
    it('should_have_error_message_when_faxnumber_is_null', async(() => {
        const fixture = TestBed.createComponent(LicenseCreationComponent);
        fixture.detectChanges();
        fixture.whenStable().then(() => {
            const zip = fixture.debugElement.query(By.css('input[name="Zip"]')).nativeElement;
            zip.dispatchEvent(new Event('input'));
            zip.value = '';
            zip.dispatchEvent(new Event('blur'));
            zip.dispatchEvent(new Event('compositionstart'));
            zip.dispatchEvent(new Event('compositionend'));
            fixture.detectChanges();
            expect(fixture.debugElement.nativeElement.firstChild[5].classList[1]).toEqual("ng-touched");
            expect(fixture.debugElement.nativeElement.firstChild[5].classList[2]).toEqual("ng-dirty");
            expect(fixture.debugElement.nativeElement.firstChild[5].classList[3]).toEqual("ng-invalid");
            expect(fixture.debugElement.query(By.css('span[id="zipMandatoryError"]')).nativeElement.textContent).toEqual(" Enter a valid ZIP Code. ");
        });
    }));
    it('should_have_error_message_when_city_is_null', async(() => {
        const fixture = TestBed.createComponent(LicenseCreationComponent);
        fixture.detectChanges();
        fixture.whenStable().then(() => {
            const city = fixture.debugElement.query(By.css('input[name="City"]')).nativeElement;
            city.dispatchEvent(new Event('input'));
            city.value = '';
            city.dispatchEvent(new Event('blur'));
            city.dispatchEvent(new Event('compositionstart'));
            city.dispatchEvent(new Event('compositionend'));
            fixture.detectChanges();
            expect(fixture.debugElement.nativeElement.firstChild[3].classList[1]).toEqual("ng-touched");
            expect(fixture.debugElement.nativeElement.firstChild[3].classList[2]).toEqual("ng-dirty");
            expect(fixture.debugElement.nativeElement.firstChild[3].classList[3]).toEqual("ng-invalid");
            expect(fixture.debugElement.query(By.css('span[id="cityMandatoryError"]')).nativeElement.textContent).toEqual(" Enter a valid city. ");
        });
    }));
    it('should_not_have_error_message_when_city_is_valid', async(() => {
        const fixture = TestBed.createComponent(LicenseCreationComponent);
        fixture.detectChanges();
        fixture.whenStable().then(() => {
            const city = fixture.debugElement.query(By.css('input[name="City"]')).nativeElement;
            city.dispatchEvent(new Event('input'));
            city.value = 'City';
            city.dispatchEvent(new Event('blur'));
            city.dispatchEvent(new Event('compositionstart'));
            city.dispatchEvent(new Event('compositionend'));
            fixture.detectChanges();
            expect(fixture.debugElement.nativeElement.firstChild[3].classList[1]).toEqual("ng-valid");
            expect(fixture.debugElement.nativeElement.firstChild[3].classList[2]).toEqual("ng-touched");
            expect(fixture.debugElement.nativeElement.firstChild[3].classList[3]).toEqual("ng-dirty");
        });
    }));
    it('should_have_error_message_when_Address1_is_null', async(() => {
        const fixture = TestBed.createComponent(LicenseCreationComponent);
        fixture.detectChanges();
        fixture.whenStable().then(() => {
            const address1 = fixture.debugElement.query(By.css('input[name="Address1"]')).nativeElement;
            address1.dispatchEvent(new Event('input'));
            address1.value = '';
            address1.dispatchEvent(new Event('blur'));
            address1.dispatchEvent(new Event('compositionstart'));
            address1.dispatchEvent(new Event('compositionend'));
            fixture.detectChanges();
            expect(fixture.debugElement.nativeElement.firstChild[1].classList[1]).toEqual("ng-touched");
            expect(fixture.debugElement.nativeElement.firstChild[1].classList[2]).toEqual("ng-dirty");
            expect(fixture.debugElement.nativeElement.firstChild[1].classList[3]).toEqual("ng-invalid");
            expect(fixture.debugElement.query(By.css('span[id="address1MandatoryError"]')).nativeElement.textContent).toEqual(" Address is required. ");
        });
    }));
    it('should_not_have_error_message_when_address1_is_valid', async(() => {
        const fixture = TestBed.createComponent(LicenseCreationComponent);
        fixture.detectChanges();
        fixture.whenStable().then(() => {
            const address1 = fixture.debugElement.query(By.css('input[name="Address1"]')).nativeElement;
            address1.dispatchEvent(new Event('input'));
            address1.value = 'Address1';
            address1.dispatchEvent(new Event('blur'));
            address1.dispatchEvent(new Event('compositionstart'));
            address1.dispatchEvent(new Event('compositionend'));
            fixture.detectChanges();
            expect(fixture.debugElement.nativeElement.firstChild[1].classList[1]).toEqual("ng-valid");
            expect(fixture.debugElement.nativeElement.firstChild[1].classList[2]).toEqual("ng-touched");
            expect(fixture.debugElement.nativeElement.firstChild[1].classList[3]).toEqual("ng-dirty");
        });
    }));
    it('should_have_error_message_when_Address1_is_null', async(() => {
        const fixture = TestBed.createComponent(LicenseCreationComponent);
        fixture.detectChanges();
        fixture.whenStable().then(() => {
            const address1 = fixture.debugElement.query(By.css('input[name="Address1"]')).nativeElement;
            address1.dispatchEvent(new Event('input'));
            address1.value = '';
            address1.dispatchEvent(new Event('blur'));
            address1.dispatchEvent(new Event('compositionstart'));
            address1.dispatchEvent(new Event('compositionend'));
            fixture.detectChanges();
            expect(fixture.debugElement.nativeElement.firstChild[1].classList[1]).toEqual("ng-touched");
            expect(fixture.debugElement.nativeElement.firstChild[1].classList[2]).toEqual("ng-dirty");
            expect(fixture.debugElement.nativeElement.firstChild[1].classList[3]).toEqual("ng-invalid");
            expect(fixture.debugElement.query(By.css('span[id="address1MandatoryError"]')).nativeElement.textContent).toEqual(" Address is required. ");
        });
    }));
    it('should_not_have_error_message_when_practice_site_is_valid', async(() => {
        const fixture = TestBed.createComponent(LicenseCreationComponent);
        fixture.detectChanges();
        fixture.whenStable().then(() => {
            const practiceSite = fixture.debugElement.query(By.css('input[name="SiteName"]')).nativeElement;
            practiceSite.dispatchEvent(new Event('input'));
            practiceSite.value = 'PracticeSite';
            practiceSite.dispatchEvent(new Event('blur'));
            practiceSite.dispatchEvent(new Event('compositionstart'));
            practiceSite.dispatchEvent(new Event('compositionend'));
            fixture.detectChanges();
            expect(fixture.debugElement.nativeElement.firstChild[0].classList[1]).toEqual("ng-valid");
            expect(fixture.debugElement.nativeElement.firstChild[0].classList[2]).toEqual("ng-touched");
            expect(fixture.debugElement.nativeElement.firstChild[0].classList[3]).toEqual("ng-dirty");
        });
    }));
    it('should_have_error_message_when_Practice_site_is_null', async(() => {
        const fixture = TestBed.createComponent(LicenseCreationComponent);
        fixture.detectChanges();
        fixture.whenStable().then(() => {
            const practiceSite = fixture.debugElement.query(By.css('input[name="SiteName"]')).nativeElement;
            practiceSite.dispatchEvent(new Event('input'));
            practiceSite.value = '';
            practiceSite.dispatchEvent(new Event('blur'));
            practiceSite.dispatchEvent(new Event('compositionstart'));
            practiceSite.dispatchEvent(new Event('compositionend'));
            fixture.detectChanges();
            expect(fixture.debugElement.nativeElement.firstChild[0].classList[1]).toEqual("ng-touched");
            expect(fixture.debugElement.nativeElement.firstChild[0].classList[2]).toEqual("ng-dirty");
            expect(fixture.debugElement.nativeElement.firstChild[0].classList[3]).toEqual("ng-invalid");
            expect(fixture.debugElement.query(By.css('span[id="siteNameMandatoryError"]')).nativeElement.textContent).toEqual(" Practice Name is required. ");
        });
    }));

    it('should_not_have_an_error_message_when_state_is_not_null', fakeAsync(() => {
        const fixture = TestBed.createComponent(LicenseCreationComponent);
        fixture.whenStable().then(() => {
            fixture.componentInstance.stateList = stateList;
            fixture.detectChanges();
            const state = fixture.debugElement.query(By.css('select')).nativeElement;
            state.value = state.options[1].value;
            state.dispatchEvent(new Event('change'));
            fixture.detectChanges();
            expect(state.value).toEqual("AL");
            expect(fixture.debugElement.nativeElement.firstChild[4].validationMessage).toEqual("");
        });
    }));
    it('should_have_error_message_when_state_is_null', async(() => {
        const fixture = TestBed.createComponent(LicenseCreationComponent);
        fixture.whenStable().then(() => {
            fixture.componentInstance.stateList = stateList;
            fixture.detectChanges();
            const state = fixture.debugElement.query(By.css('select')).nativeElement;
            state.dispatchEvent(new Event('change'));
            fixture.detectChanges();
            expect(state.value).toEqual("");
            expect(fixture.debugElement.nativeElement.firstChild[4].validationMessage).toEqual("Please select an item in the list.");
        });
    }));
});









