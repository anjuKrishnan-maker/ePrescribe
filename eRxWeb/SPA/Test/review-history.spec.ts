import { TestBed,async, ComponentFixture } from '@angular/core/testing';
import { } from 'jasmine';
import { Observable } from 'rxjs/Observable';
import { Observer } from 'rxjs/Observer';
import { RouterTestingModule } from "@angular/router/testing";
import { NavigationExtras, ActivatedRoute } from '@angular/router';
import { ReviewHistoryComponent } from '../component/review-history/review-history.component';
import { ReviewHistoryService, ComponentCommonService, EventService, ErrorService, PatientService, CancelRxService, PatientMedRecService, RxDetailService, ComponentRedirectionService } from '../services/service.import.def';
import { SupervisingProviderModel } from '../model/sup-prov.model';
import { ReviewHistoryProvider } from '../model/model.import.def';
import { SupervisingProviderPrompt } from '../component/common/shared/supervising-provider-prompt/supervising-provider-prompt.component';
import { RxDetailComponent } from '../component/common/right-panel/rx-detail/rx-detail.component';


describe('Review History Components', () => {
    let reviewHistoryComponent: ReviewHistoryComponent
    let supervisingProviderPrompt: SupervisingProviderPrompt
    let fixture: ComponentFixture<ReviewHistoryComponent>;
    let component: ReviewHistoryComponent;

    let supervisingProvider: SupervisingProviderModel[] = [
        {
            ProviderID: "1",
            ProviderName: "Provider1"
        },
        {
            ProviderID: "2",
            ProviderName: "Provider2"
        }];

    let evnntSvc: EventService = new EventService();
    let errorsvc: ErrorService = new ErrorService(evnntSvc);
    let http: any;

    class MockReviewHistoryService extends ReviewHistoryService {
        GetSupervisingProviders() {
            return Observable.create((observer: Observer<SupervisingProviderModel[]>) => {
                observer.next(supervisingProvider);
            });
        }
    }

    class MockPatientService extends PatientService {

    }

    class MockCancelRxService extends CancelRxService {

    }

    class MockPatientMedService extends PatientMedRecService {

    }

    class MockRxDetailService extends RxDetailService
    {

    }

    class MockNavigationExtra {
        extras = class {
            state: any;
        }
    }


    class MockRouter extends RouterTestingModule {
        getCurrentNavigation(): MockNavigationExtra
        {
            let natigationExtra = new MockNavigationExtra();
            return natigationExtra;
        }
    }

    class MockActivatedRoute extends ActivatedRoute {

    }

    beforeAll(() => {
        component = new ReviewHistoryComponent(new MockReviewHistoryService(http), new MockPatientService(http, errorsvc, evnntSvc), evnntSvc, new MockCancelRxService(http), new MockPatientMedService(http, errorsvc), new MockRouter(), new MockActivatedRoute);
    });


    beforeEach(async(() => {

    }));


    it('Validate Supervising Provider Prompt - should return supervising providers in Supervising Provider Prompt', () => {

        //Arrange
        let _CurrentDelegateProviderSelection = new ReviewHistoryProvider();
        _CurrentDelegateProviderSelection.ProviderId = "1";
        _CurrentDelegateProviderSelection.ProviderName = "Rajesh";
        _CurrentDelegateProviderSelection.UserTypeID = 1001;

        let _supervisingProviderPrompt: SupervisingProviderPrompt;

 
        _supervisingProviderPrompt = new SupervisingProviderPrompt(evnntSvc, new MockReviewHistoryService(http));
        component.CurrentDelegateProviderSelection = _CurrentDelegateProviderSelection;
        component.supervisingProviderPrompt = _supervisingProviderPrompt;

        //Act
        component.NewRxClicked();

        //Assert        
        expect(component.supervisingProviderPrompt.providers.length).toEqual(2);
        expect(component.supervisingProviderPrompt.providers[0].ProviderID).toEqual("1");
        expect(component.supervisingProviderPrompt.providers[0].ProviderName).toEqual("Provider1");
        expect(component.supervisingProviderPrompt.providers[1].ProviderID).toEqual("2");
        expect(component.supervisingProviderPrompt.providers[1].ProviderName).toEqual("Provider2");
    });

    it('ShowRxDetails invokes OnHistoryRxSelected', () => {
        //arrange 
        let _rxDetailComponent: RxDetailComponent;    

        _rxDetailComponent = new RxDetailComponent(new MockRxDetailService(http, errorsvc), evnntSvc, new ComponentCommonService);

        //act
        let testHistoryItem= <any>{
            Selected: true,
            RxID : 'C970F337-E9AA-4763-B39F-E30DC019FFD9'
        };
        spyOn(_rxDetailComponent, 'LoadRxDetails');
        component.ShowRxDetails(testHistoryItem);
        
        //assert
        expect(_rxDetailComponent.LoadRxDetails).toHaveBeenCalled();
       
    });

})