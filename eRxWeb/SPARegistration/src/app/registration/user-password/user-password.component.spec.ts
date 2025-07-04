import { fakeAsync, TestBed, tick, async } from "@angular/core/testing";
import { UserPasswordComponent } from "./user-password.component";
import { By } from "@angular/platform-browser";

describe('UserPasswordComponent', () => {
    
    it('should_have_password_empty_when_phone_number_is_null', fakeAsync(() => {
        const fixture = TestBed.createComponent(UserPasswordComponent);
        const password = fixture.debugElement.query(By.css('input[name="password1"]')).nativeElement
        fixture.componentInstance.userPassWord.password = "";
        fixture.detectChanges();
        tick();
        expect(password.value).toEqual("");
    }));
    it('should_have_password_in_specified_format', fakeAsync(() => {
        const fixture = TestBed.createComponent(UserPasswordComponent);
        const password = fixture.debugElement.query(By.css('input[name="password1"]')).nativeElement
        fixture.componentInstance.userPassWord.password  = "Test@123";
        fixture.detectChanges();
        tick();
        expect(password.value).toEqual("Test@123");
    }));
    it('should_have_password_patternMismatch_false_if_the_password_is_empty', fakeAsync(() => {
        const fixture = TestBed.createComponent(UserPasswordComponent);
        fixture.componentInstance.userPassWord.password = "";
        fixture.detectChanges();
        tick();
        expect(fixture.nativeElement.firstElementChild.password1.validity.patternMismatch).toBeFalsy();
        expect(fixture.nativeElement.firstElementChild.password1.validationMessage).toEqual("Password is required.");
    }));
    it('should_have_password_patternMismatch_true_if_the_password_is_invalid_format', fakeAsync(() => {
        const fixture = TestBed.createComponent(UserPasswordComponent);
        fixture.componentInstance.userPassWord.password = "12345";
        fixture.detectChanges();
        tick();
        expect(fixture.nativeElement.firstElementChild.password1.validity.patternMismatch).toBeTruthy();
    }));
    it('should_have_password_patternMismatch_false_if_the_password_is_valid_format', fakeAsync(() => {
        const fixture = TestBed.createComponent(UserPasswordComponent);
        fixture.componentInstance.userPassWord.password = "abcd";
        fixture.detectChanges();
        tick();
        expect(fixture.nativeElement.firstElementChild.password1.validity.patternMismatch).toBeFalsy();
    }));

    it('should_have_password_required_to_true', fakeAsync(() => {
        const fixture = TestBed.createComponent(UserPasswordComponent);
        fixture.componentInstance.userPassWord.password = "";
        fixture.detectChanges();
        tick();
        expect(fixture.nativeElement.firstElementChild.elements.password1.maxLength).toEqual(25);
        expect(fixture.nativeElement.firstElementChild.elements.password1.validationMessage).toEqual("Password is required.");
        expect(fixture.nativeElement.firstElementChild.elements.password1.required).toBeTruthy();
    }));
    it('should_have_error_message_when_password_is_invalid', async(() => {
        const fixture = TestBed.createComponent(UserPasswordComponent);
        fixture.detectChanges();
        fixture.whenStable().then(() => {
            const password = fixture.debugElement.query(By.css('input[name="password1"]')).nativeElement;
            password.dispatchEvent(new Event('input'));
            password.value = '987456';
            password.dispatchEvent(new Event('blur'));
            fixture.detectChanges();
            expect(fixture.debugElement.nativeElement.firstChild[6].classList[1]).toEqual("ng-touched");
            expect(fixture.debugElement.nativeElement.firstChild[6].classList[2]).toEqual("ng-dirty");
            expect(fixture.debugElement.nativeElement.firstChild[6].classList[3]).toEqual("ng-invalid");
            expect(fixture.debugElement.query(By.css('span[id="passwordPatternError1"]')).nativeElement.textContent).toEqual("Password does not meet criteria mentioned below.");
        });
    }));
    it('should_not_have_error_message_when_password_is_valid', async(() => {
        const fixture = TestBed.createComponent(UserPasswordComponent);
        fixture.detectChanges();
        fixture.whenStable().then(() => {
            const password = fixture.debugElement.query(By.css('input[name="passwordPatternError1"]')).nativeElement;
            password.dispatchEvent(new Event('input'));
            password.value = 'Test@123';
            password.dispatchEvent(new Event('blur'));
            password.dispatchEvent(new Event('compositionstart'));
            password.dispatchEvent(new Event('compositionend'));
            fixture.detectChanges();
            expect(fixture.debugElement.nativeElement.firstChild[6].classList[1]).toEqual("ng-valid");
            expect(fixture.debugElement.nativeElement.firstChild[6].classList[2]).toEqual("ng-touched");
            expect(fixture.debugElement.nativeElement.firstChild[6].classList[3]).toEqual("ng-dirty");
        });
    }));
});
