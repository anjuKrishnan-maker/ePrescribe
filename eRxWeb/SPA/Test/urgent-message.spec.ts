import { async } from '@angular/core/testing';
import { } from 'jasmine';
import { Observable } from 'rxjs/Observable';
import { Observer } from 'rxjs/Observer';
import { UrgentMessageComponent } from '../component/common/right-panel/urgent-message/urgent-message.component';
import { UrgentMessageModel } from '../model/model.import.def';
import { ComponentCommonService, ErrorService, EventService, UrgentMessageService } from '../services/service.import.def';
import { RouterTestingModule } from "@angular/router/testing";

describe('UrgemtMessage Unit test', () => {
    let component: UrgentMessageComponent;

    let urgentMessageResponceModel = new UrgentMessageModel();
    urgentMessageResponceModel.EnableEprescribeMsg = "";
    urgentMessageResponceModel.FailedLicenseMessage = "You have failed Rx messages";
    urgentMessageResponceModel.FailedLicenseMessageUrl = "FailedLicenseMessageUrl";
    urgentMessageResponceModel.FailedRegistrationMessage = "There are  failed registration messages."
    urgentMessageResponceModel.FailedRxUserMessage = "You have  failed Rx messages";
    urgentMessageResponceModel.FailedRxUserMessageUrl = "ailedRxUserMessageUrl";
    urgentMessageResponceModel.FailedegistrationMessageLinkText = "View failed messages";
    urgentMessageResponceModel.IsDeaExpiryVisible = false;
    urgentMessageResponceModel.IsDeluxMessageVisible = false;
    urgentMessageResponceModel.IsDeniedRefReqMessagesVisible = false;
    urgentMessageResponceModel.IsEpcsApproverVisible = false;
    urgentMessageResponceModel.IsEpcsCanEnrollVisible = false;
    urgentMessageResponceModel.IsFailedLicenseMessageVisible = true;
    urgentMessageResponceModel.IsFailedRxMessageVisible = false;
    urgentMessageResponceModel.IsFailedUserMessageVisible = true;
    urgentMessageResponceModel.IsPasswordExpiryNotice = false;
    urgentMessageResponceModel.IsStateLicenseExpiryVisible = false;
    urgentMessageResponceModel.IsTrialPeriodExpiryVisible = false;
    urgentMessageResponceModel.IsrRgistrationMessagesVisible = true;
    urgentMessageResponceModel.TrialPeriodExpiryMessage = "";

    let evnntSvc: EventService = new EventService();
    let errorsvc: ErrorService = new ErrorService(evnntSvc, null);

    class MockUrgentMessageService extends UrgentMessageService {
        GetUrgentMessage() {
            return Observable.create((observer: Observer<void>) => {
                observer.next(urgentMessageResponceModel);
            });
        }
    }

    class MockRouter extends RouterTestingModule {
        getCurrentNavigation() {
        }
    }

    beforeEach(async(() => {
        component = new UrgentMessageComponent(null, new MockUrgentMessageService(null, errorsvc), evnntSvc, new ComponentCommonService(), new MockRouter());
    }));

    it('load-urgent-message', async(() => {
        component.loadUrgentMessage();
        expect(component.urgentMessage.IsFailedLicenseMessageVisible).toBeTruthy;
        expect(component.urgentMessage.IsFailedUserMessageVisible).toBeTruthy;
        expect(component.urgentMessage.IsrRgistrationMessagesVisible).toBeTruthy;

        expect(component.urgentMessage.IsDeluxMessageVisible).toBeFalsy;
        expect(component.urgentMessage.IsDeaExpiryVisible).toBeFalsy;
        expect(component.urgentMessage.IsDeniedRefReqMessagesVisible).toBeFalsy;
        expect(component.urgentMessage.IsEpcsApproverVisible).toBeFalsy;
        expect(component.urgentMessage.IsEpcsCanEnrollVisible).toBeFalsy;
        expect(component.urgentMessage.IsFailedRxMessageVisible).toBeFalsy;
        expect(component.urgentMessage.IsPasswordExpiryNotice).toBeFalsy;
        expect(component.urgentMessage.IsStateLicenseExpiryVisible).toBeFalsy;
        expect(component.urgentMessage.IsTrialPeriodExpiryVisible).toBeFalsy;

        expect(component.urgentMessage.EnableEprescribeMsg).toEqual(urgentMessageResponceModel.EnableEprescribeMsg);
        expect(component.urgentMessage.FailedLicenseMessage).toEqual(urgentMessageResponceModel.FailedLicenseMessage);
        expect(component.urgentMessage.FailedLicenseMessageUrl).toEqual(urgentMessageResponceModel.FailedLicenseMessageUrl);
        expect(component.urgentMessage.FailedRegistrationMessage).toEqual(urgentMessageResponceModel.FailedRegistrationMessage);
        expect(component.urgentMessage.FailedRxUserMessage).toEqual(urgentMessageResponceModel.FailedRxUserMessage);
        expect(component.urgentMessage.FailedRxUserMessageUrl).toEqual(urgentMessageResponceModel.FailedRxUserMessageUrl);
        expect(component.urgentMessage.FailedegistrationMessageLinkText).toEqual(urgentMessageResponceModel.FailedegistrationMessageLinkText);
    }));
});