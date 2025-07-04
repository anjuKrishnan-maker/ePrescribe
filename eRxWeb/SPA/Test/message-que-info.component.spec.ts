import { async, TestBed, ComponentFixture } from '@angular/core/testing';
import { ChangeDetectorRef } from '@angular/core';
import { } from 'jasmine';
import { Observable } from 'rxjs/Observable';
import { Observer } from 'rxjs/Observer';
import { MessageQueInfoModel } from '../model/model.import.def';
import { MessageQueInfoComponent } from '../component/common/right-panel/right-panel.component.import.def';
import { BrowserDynamicTestingModule, platformBrowserDynamicTesting } from '@angular/platform-browser-dynamic/​testing';
import { MessageQueRxService, EventService, ErrorService, ComponentCommonService } from '../services/service.import.def';


describe("Message Info Component", () => {

    let messageQueueInfoComponent: MessageQueInfoComponent;
    let fixture: ComponentFixture<MessageQueInfoComponent>;
    let e1: HTMLElement;

    let messageQueueInfoModel = new MessageQueInfoModel();
    messageQueueInfoModel.Patient = "test";
    messageQueueInfoModel.Address = "652 Main";
    messageQueueInfoModel.City = "Bangalore";
    messageQueueInfoModel.State = "CA";
    messageQueueInfoModel.ZIP = "12345";
    messageQueueInfoModel.Phone = "7875456456";
    messageQueueInfoModel.Gender = "M";
    messageQueueInfoModel.RxDate = "2019-10-14T05:59:12-04:00";
    messageQueueInfoModel.RxDrug = "Lisinopril 20 MG TABS";
    messageQueueInfoModel.Sig = "TAKE 1 TABLET DAILY AS DIRECTED.";
    messageQueueInfoModel.Quantity = "1000";
    messageQueueInfoModel.Refills = "2";
    messageQueueInfoModel.DAW = "No";
    messageQueueInfoModel.Notes = "Testing";
    messageQueueInfoModel.PharmacyName = "CA PHARMACY 10.6MU";
    messageQueueInfoModel.DBPharmacyNetworkID = "9911557";
    messageQueueInfoModel.PharmacyAddress1 = "65432 CABERNET TURN";
    messageQueueInfoModel.PharmacyCity = "SONOMA";
    messageQueueInfoModel.PharmacyState = "CA";
    messageQueueInfoModel.PharmacyZip = "12345";
    messageQueueInfoModel.PharmacyPhoneNumber = "7072107071";
    messageQueueInfoModel.PharmacyFaxNumber = "7072107071";
    messageQueueInfoModel.ConfirmedBy = "Ravoos Nayudamma";
    messageQueueInfoModel.ConfirmedOn = "10/14/2019";
       
    let evnntSvc: EventService = new EventService();
    let errorsvc: ErrorService = new ErrorService(evnntSvc);

    class MockMessageQueueService extends MessageQueRxService{
        GetMessageQueInfo(data) {
            return Observable.create((observer: Observer<void>) => {
                if (data.scriptMessageID == "123")
                    observer.next(messageQueueInfoModel);               
            });
        }
    }
    
    class MockChangeDetectorRef extends ChangeDetectorRef {
        detectChanges() {
        }
    }

    class MockComponentCommonService extends ComponentCommonService {

    }

    
    beforeEach(() => {        
        messageQueueInfoComponent = new MessageQueInfoComponent(new MockMessageQueueService(null, errorsvc),null , evnntSvc, new MockComponentCommonService());        
    })

    it('Should have message queue info when scriptMessageID is not null', async(() => {
        messageQueueInfoComponent.LoadInfo({ scriptMessageID: "123" });
        expect(messageQueueInfoComponent.messageQueInfo.Patient).toEqual(messageQueueInfoModel.Patient);
        expect(messageQueueInfoComponent.messageQueInfo.Address).toEqual(messageQueueInfoModel.Address);
        expect(messageQueueInfoComponent.messageQueInfo.City).toEqual(messageQueueInfoModel.City);
        expect(messageQueueInfoComponent.messageQueInfo.State).toEqual(messageQueueInfoModel.State);
        expect(messageQueueInfoComponent.messageQueInfo.ZIP).toEqual(messageQueueInfoModel.ZIP);
        expect(messageQueueInfoComponent.messageQueInfo.Phone).toEqual(messageQueueInfoModel.Phone);
        expect(messageQueueInfoComponent.messageQueInfo.Gender).toEqual(messageQueueInfoModel.Gender);
        expect(messageQueueInfoComponent.messageQueInfo.RxDate).toEqual(messageQueueInfoModel.RxDate);
        expect(messageQueueInfoComponent.messageQueInfo.RxDrug).toEqual(messageQueueInfoModel.RxDrug);
        expect(messageQueueInfoComponent.messageQueInfo.Sig).toEqual(messageQueueInfoModel.Sig);
        expect(messageQueueInfoComponent.messageQueInfo.Quantity).toEqual(messageQueueInfoModel.Quantity);
        expect(messageQueueInfoComponent.messageQueInfo.Refills).toEqual(messageQueueInfoModel.Refills);        
        expect(messageQueueInfoComponent.messageQueInfo.DAW).toEqual(messageQueueInfoModel.DAW);        
        expect(messageQueueInfoComponent.messageQueInfo.Notes).toEqual(messageQueueInfoModel.Notes);        
        expect(messageQueueInfoComponent.messageQueInfo.PharmacyName).toEqual(messageQueueInfoModel.PharmacyName);        
        expect(messageQueueInfoComponent.messageQueInfo.PharmacyAddress1).toEqual(messageQueueInfoModel.PharmacyAddress1);    
        expect(messageQueueInfoComponent.messageQueInfo.DBPharmacyNetworkID).toEqual(messageQueueInfoModel.DBPharmacyNetworkID);    
        expect(messageQueueInfoComponent.messageQueInfo.PharmacyCity).toEqual(messageQueueInfoModel.PharmacyCity);    
        expect(messageQueueInfoComponent.messageQueInfo.PharmacyState).toEqual(messageQueueInfoModel.PharmacyState);
        expect(messageQueueInfoComponent.messageQueInfo.PharmacyZip).toEqual(messageQueueInfoModel.PharmacyZip);
        expect(messageQueueInfoComponent.messageQueInfo.PharmacyPhoneNumber).toEqual(messageQueueInfoModel.PharmacyPhoneNumber);
        expect(messageQueueInfoComponent.messageQueInfo.PharmacyFaxNumber).toEqual(messageQueueInfoModel.PharmacyFaxNumber);
        expect(messageQueueInfoComponent.messageQueInfo.ConfirmedBy).toEqual(messageQueueInfoModel.ConfirmedBy);
        expect(messageQueueInfoComponent.messageQueInfo.ConfirmedOn).toEqual(messageQueueInfoModel.ConfirmedOn);
    }));

    it('should have message queue info as null when scriptMessageID is null', async(() => {
        messageQueueInfoComponent.SetMessageQueInfo(null);
        expect(messageQueueInfoComponent.messageQueInfo).toBeNull();
    }));
});
