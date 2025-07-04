import { async, ComponentFixture, TestBed, fakeAsync, tick } from '@angular/core/testing';


import { UserExistingAccountComponent } from '../user-existing-account/user-existing-account.component';
import { FormsModule } from '@angular/forms';
import { HttpModule } from '@angular/http';
import { UserCreateService } from '../../service/user-create.service';
import { Observable, Observer } from 'rxjs';
import { LicenseCreateService } from '../../service/license-create.service';
import { SafeResourceUrl,DomSanitizer,BrowserModule, By, SafeUrl } from '@angular/platform-browser';
import { RouterTestingModule } from '@angular/router/testing';
import { Router } from '@angular/router';
import { BrowserDynamicTestingModule, platformBrowserDynamicTesting } from '@angular/platform-browser-dynamic/testing';

describe('UserExistingAccountComponent', () => {
    let component: UserExistingAccountComponent;
    let fixture: ComponentFixture<UserExistingAccountComponent>;
   
    
    class MockUserCreateService {

        getInitialPageData() {            
          
        }

      
    }

    class MockLicenseCreateService {
       
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
          declarations: [UserExistingAccountComponent],
          providers: [{ provide: LicenseCreateService, useClass: MockLicenseCreateService },     
              { provide:UserCreateService, useClass: MockUserCreateService },
              { provide: 'window', useFactory: (() => { return { appcontext: { version: "333" } }; }) }                        
          ]
      }).compileComponents();
  }));

});
