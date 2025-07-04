import { async, ComponentFixture, TestBed, fakeAsync, tick } from '@angular/core/testing';
import { ShowCaptchaComponent } from './show-captcha.component';
import { CaptchaService } from '../../service/captcha.service';
import { HttpModule } from '@angular/http';
import { Observable, Observer } from 'rxjs';
import { BrowserModule, By} from '@angular/platform-browser';
import { RouterTestingModule } from '@angular/router/testing';
import { BrowserDynamicTestingModule, platformBrowserDynamicTesting } from '@angular/platform-browser-dynamic/testing';
import { FormsModule } from '@angular/forms';


describe('ShowCaptchaComponent', () => {
    let component: ShowCaptchaComponent;
    let fixture: ComponentFixture<ShowCaptchaComponent>;  
    let response: any;    
    class MockCaptchaService{                
        getCaptcha() {
            return Observable.create((observer: Observer<any>) => {
                observer.next(response);
            });
        }
    }
    
    beforeEach(async(() => {
        TestBed.resetTestEnvironment();
        TestBed.initTestEnvironment(BrowserDynamicTestingModule,
            platformBrowserDynamicTesting());
        TestBed.configureTestingModule({
            imports: [HttpModule, BrowserModule, FormsModule, RouterTestingModule],
            declarations: [ShowCaptchaComponent],
            providers: [{ provide: CaptchaService, useClass: MockCaptchaService },           
            { provide: 'window', useFactory: (() => { return { appcontext: { version: "333" } }; }) }
            ]
        }).compileComponents();
    }));
    beforeEach(() => {
        fixture = TestBed.createComponent(ShowCaptchaComponent);
        component = fixture.debugElement.componentInstance; 
       });
    it('should_show_captcha_component', () => {      
        expect(component).toBeTruthy();
    });
    it('textbox_has_empty_entry_then_captchaText_should_be_empty', fakeAsync(() => {       
        fixture.componentInstance.captchaText = "";    
        const captchaentry = fixture.debugElement.query(By.css('input[name="txtCapchaResponse"]')).nativeElement;     
        tick();        
        expect(captchaentry.value).toEqual("");           
    }));
    //it('should_assign_true_if_capcha_changed', () => {
    //    component.onCaptchaChanged();
    //    expect(component.isValidCaptcha).toBeTruthy();
    //});
    it('textbox_has_same_value', fakeAsync(() => {   
        let captchaentry = fixture.debugElement.query(By.css('input[name="txtCapchaResponse"]')).nativeElement;  
        component.captchaText = "captcha";       
        fixture.detectChanges();  
        tick();       
        expect(captchaentry.value).toEqual("captcha");
    }));
});
