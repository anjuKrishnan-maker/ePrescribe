import { async, ComponentFixture, TestBed, fakeAsync, tick } from '@angular/core/testing';
import { UserAccountNameComponent } from './user-account-name.component';
import { FormsModule } from '@angular/forms';
import { HttpModule } from '@angular/http';
import { UserCreateService } from '../../service/user-create.service';
import { BrowserModule, By } from '@angular/platform-browser';
import { RouterTestingModule } from '@angular/router/testing';
import { BrowserDynamicTestingModule, platformBrowserDynamicTesting } from '@angular/platform-browser-dynamic/testing';

describe('UserAccountNameComponent', () => {
    let component: UserAccountNameComponent;
    let fixture: ComponentFixture<UserAccountNameComponent>;
    class MockUserCreateService {
        getInitialPageData() {            
            return false;
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
          declarations: [UserAccountNameComponent],
          providers: [     
              { provide:UserCreateService, useClass: MockUserCreateService },
              { provide: 'window', useFactory: (() => { return { appcontext: { version: "333" } }; }) }                        
          ]
      }).compileComponents();
  }));
    it('should_have_password_empty_when_phone_number_is_null', fakeAsync(() => {
        const fixture = TestBed.createComponent(UserAccountNameComponent);
        const password = fixture.debugElement.query(By.css('input[name="password1"]')).nativeElement
        fixture.componentInstance.userAccount.password = "";
        fixture.detectChanges();
        tick();
        expect(password.value).toEqual("");
    }));

    it('should_have_accontname_patternMismatch_false_if_the_accontname_is_invalid_format', fakeAsync(() => {
        fixture = TestBed.createComponent(UserAccountNameComponent);
        fixture.componentInstance.userAccount.shieldUserName = "12345abd;;>";
        fixture.detectChanges();
        tick();
        expect(fixture.nativeElement.firstElementChild.txtContactaccontname.validity.patternMismatch).toBeTruthy();
    }));
    
    it('should_have_accontname_patternMismatch_false_if_the_accontname_is_valid_format', fakeAsync(() => {
        fixture = TestBed.createComponent(UserAccountNameComponent);
        fixture.componentInstance.userAccount.shieldUserName = "TestProvider";
        fixture.detectChanges();
        tick();
        expect(fixture.nativeElement.firstElementChild.txtContactaccontname.validity.patternMismatch).toBeTruthy();
    }));
});
