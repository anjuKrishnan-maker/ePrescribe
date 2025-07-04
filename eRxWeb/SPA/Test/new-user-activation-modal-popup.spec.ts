import { TestBed, async, ComponentFixture, fakeAsync, tick } from "../../node_modules/@angular/core/testing"
import { NewUserActivationModalPopupComponent } from "../component/new-user-activation-modal-popup/new-user-activation-modal-popup.component";
import { NewUserActivationModalPopupService, EventService, ErrorService } from '../services/service.import.def';
import { Observable } from 'rxjs/Observable';
import { Observer } from 'rxjs/Observer';
import { NewUserCommunicationStatus } from "../model/new-user-activation/new-user-activation-modalpopup-model";



describe("New User Activation Popup", () => {


    let Url: string = "https://eprescribeint.allscripts.com/activate/default.aspx";
    let evnntSvc: EventService = new EventService();
    let errorsvc: ErrorService = new ErrorService(evnntSvc, null);
    let component: NewUserActivationModalPopupComponent;
    let fixture: ComponentFixture<NewUserActivationModalPopupComponent>;

    class MockNewUserActivationModalPopupService extends NewUserActivationModalPopupService {
        getActivationUrl() {

            return Observable.create((observer: Observer<string>) => {
                observer.next(Url);
            });
        }
    }

    beforeEach(() => {
        component = new NewUserActivationModalPopupComponent(new MockNewUserActivationModalPopupService(null, errorsvc), null);
        component.ngOnInit();
    })

    it('should have url', async(() => {
        component.getActivationUrl();
        expect(component.activationUrl).toEqual(Url);
    }));

    it('showModalpopup should be false when OpenModalPopup is not called', async(() => {
        
        expect(component.showModalPopup).toBeFalsy();
    }));

    it('showModalpopup should be true when OpenModalPopup is called', async(() => {
        component.openNewUserActivationModalPopup();
        expect(component.showModalPopup).toBeTruthy();
    }));

    it('should return true when Email is selected', async(() => {                
        component.newUserInfo.newUserCommunicationStatus.IsEmailChecked = true;
        component.newUserInfo.newUserCommunicationStatus.IsPrintChecked = false;
        component.validateEmailorPrintChecked();
        expect(component.validateEmailorPrintChecked).toBeTruthy();
    }));
    it('should return true when print is selected', async(() => {
        
        component.newUserInfo.newUserCommunicationStatus.IsPrintChecked = true;
        component.newUserInfo.newUserCommunicationStatus.IsEmailChecked = false;
        component.validateEmailorPrintChecked();
        expect(component.validateEmailorPrintChecked).toBeTruthy();
    }));
    it('should return true when both are selected', async(() => {                
        component.validateEmailorPrintChecked();
        expect(component.validateEmailorPrintChecked).toBeTruthy();
    }));
    it('should return false when not selected', async(() => {               
        component.newUserInfo.newUserCommunicationStatus.IsEmailChecked = false;
        component.newUserInfo.newUserCommunicationStatus.IsPrintChecked = false;        
        expect(component.validateEmailorPrintChecked()).toBeFalsy();
    }));

    

});