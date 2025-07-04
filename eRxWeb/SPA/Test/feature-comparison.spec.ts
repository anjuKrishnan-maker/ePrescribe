import { FeatureComparisonComponent } from '../component/feature-comparison/feature-comparison.component';
import { TestBed, ComponentFixture, async } from '@angular/core/testing';
import { BrowserDynamicTestingModule, platformBrowserDynamicTesting } from '@angular/platform-browser-dynamic/testing';
import { Observable } from 'rxjs/Observable';
import { Observer } from 'rxjs/Observer';
import { DebugElement } from '@angular/core';
import { By } from '@angular/platform-browser';
import { HttpModule } from '@angular/http';
import { FeatureComparisonService, EventService, ComponentCommonService, ErrorService } from '../services/service.import.def';
import { FeatureComparisonModel } from '../model/model.import.def';
import 'jquery';
import 'bootstrap/dist/js/bootstrap';
import 'bootstrap/dist/css/bootstrap.min.css';

xdescribe('GetFeatureComparisonUrl unit tests', () => {

    let ImageUrl: string;
    let component: FeatureComparisonComponent;
    let fixture: ComponentFixture<FeatureComparisonComponent>;
    let element: HTMLElement;
    let fcs: FeatureComparisonService;

    class MockFeatureComparisonService {
        GetFeatureComparisonUrl() {
            return Observable.create((observer: Observer<string>) => {
                observer.next(ImageUrl);
            });
        }
    }

    beforeEach(async(() => {
        ImageUrl = "images/FeatureComparison/FeatureComparisonGrid.PNG";

        TestBed.resetTestEnvironment();
        TestBed.initTestEnvironment(
            BrowserDynamicTestingModule, platformBrowserDynamicTesting());

        //Test bed configuration
        TestBed.configureTestingModule({
            declarations: [FeatureComparisonComponent],
            providers: [ComponentCommonService, EventService, ErrorService, { provide: FeatureComparisonService, useClass: MockFeatureComparisonService }],
            imports: [HttpModule],
        }).compileComponents();

        //fixture = TestBed.createComponent(FeatureComparisonComponent);
        //component = fixture.componentInstance;
        //fcs = TestBed.get(FeatureComparisonService);
        //TestBed.compileComponents()
        //    .then(() => {
        //        fixture = TestBed.createComponent(FeatureComparisonComponent);
        //        component = fixture.debugElement.componentInstance;
        //        element = fixture.debugElement.nativeElement;
        //    });
    }));

    it('should_get_feature_comparison_image_source', async(() => {
        fixture = TestBed.createComponent(FeatureComparisonComponent);
        component = fixture.debugElement.componentInstance;
        fcs = TestBed.get(FeatureComparisonService);
        component.GetFeatureComparisonUrl();
        fixture.whenStable().then(() => {
            expect(component.featureComparisonModel.ImageUrl).toEqual(ImageUrl);
        });
    }));

    it('should_have_feature_comparision_component', () => {
        fixture = TestBed.createComponent(FeatureComparisonComponent);
        component = fixture.debugElement.componentInstance;
        expect(component).toBeTruthy();
    });
});




