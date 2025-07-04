import { TestBed, async } from '@angular/core/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { AppComponent } from './app.component';
import { BrowserDynamicTestingModule, platformBrowserDynamicTesting } from '@angular/platform-browser-dynamic/testing';
import { LoaderService } from './service/loader.service';
import { MessageService } from './service/message.service';


describe('AppComponent', () => {

    class MockUserLoaderService {


    }

    class MockMessageService {


    }

    beforeEach(async(() => {
        TestBed.resetTestEnvironment();
        TestBed.initTestEnvironment(BrowserDynamicTestingModule, platformBrowserDynamicTesting());
        TestBed.configureTestingModule({
            imports: [
                RouterTestingModule
            ],
            declarations: [
                AppComponent
            ],
            providers: [
                { provide: 'window', useFactory: (() => { return { appcontext: { version: "333", supportMailAddress:"aaa@aaa.com" } }; }) },
                { provide: LoaderService, useClass: MockUserLoaderService },
                { provide: MessageService, useClass: MockMessageService }
            ]
        }).compileComponents();
    }));

    it('should create the app', () => {
        const fixture = TestBed.createComponent(AppComponent);
        const app = fixture.debugElement.componentInstance;
        expect(app).toBeTruthy();
    });

    it(`should have version from window context`, () => {
        const fixture = TestBed.createComponent(AppComponent);
        const app = fixture.debugElement.componentInstance;
        expect(app.version).toEqual('333');
    });

    it('should render version', () => {
        const fixture = TestBed.createComponent(AppComponent);
        fixture.detectChanges();
        const compiled = fixture.debugElement.nativeElement;
        expect(compiled.querySelector('.reg-app-version').textContent).toContain('333');
    });

    it('should render support email', () => {
        const fixture = TestBed.createComponent(AppComponent);
        fixture.detectChanges();
        const compiled = fixture.debugElement.nativeElement;        
        expect(compiled.querySelector('.reg-supportlink-anchor').getAttribute('href')).toContain('mailto:aaa@aaa.com');
        expect(compiled.querySelector('.reg-supportlink').textContent).toContain('aaa@aaa.com');
    });
});

