import { Component, ElementRef, ViewChild, Input } from '@angular/core';
import { BaseComponent } from '../../base.component';
import { DeniedRefReqMessageComponent } from '../../shared/denied-ref-req-messages/denied-ref-req-messages.component';
import { FailedRxMessageComponent } from '../../shared/failed-rx-message/failed-rx-message.component';
import { UrgentMessageService, EventService, ChangePasswordService, ComponentCommonService } from '../../../../services/service.import.def';
import { UrgentMessageModel, ChangePasswordModel, RightPanelPayload } from '../../../../model/model.import.def';
import { EVENT_COMPONENT_ID, NAVIGATION_EVENT_ID, current_context, PAGE_NAME, ROUTE_NAME } from '../../../../tools/constants';
import { Router } from '@angular/router';
import { ChangePasswordPopupComponent } from '../../../shared/change-password-popup/change-password-popup.component';
@Component({
    selector: 'erx-urgent-message',
    templateUrl: './urgent-message.template.html',
    providers: [UrgentMessageService, ChangePasswordService]
})
export class UrgentMessageComponent extends BaseComponent {
    changePassword: ChangePasswordModel = new ChangePasswordModel();    
    @ViewChild(ChangePasswordPopupComponent) changePasswordPOPup: ChangePasswordPopupComponent;
    @ViewChild(DeniedRefReqMessageComponent) denRefReq: DeniedRefReqMessageComponent;
    @ViewChild(FailedRxMessageComponent) failedRx: FailedRxMessageComponent;

    @Input()
    set isUrgentMessageCountChanged(urgentMessageCountChange: boolean) {
        if (urgentMessageCountChange)
            this.CallChildFunction(null);
    }

    constructor(cd: ElementRef, private urgMssgSvc: UrgentMessageService, private svE: EventService, private compSvc: ComponentCommonService, private router: Router) {
        super(cd, svE);

        this.svE.subscribeEvent(EVENT_COMPONENT_ID.ContentRefreshing, () => {
            this.Hide();
        });

        this.svE.subscribeEvent(EVENT_COMPONENT_ID.RightPanelLoaded, (rightPanelPayload: RightPanelPayload) => {
            if (this.compSvc.CheckVisibility(this.id, current_context.PageName)) {
                this.urgentMessage = rightPanelPayload.UrgentMessagePayload;
                if (this.urgentMessage != undefined) {
                    this.Show();
                }
            }
            else {
                this.Hide();
            }
        });
                
    }
    OpenChangePassword() {
        this.changePasswordPOPup.ShowModal();
    }
    urgentMessage: UrgentMessageModel = new UrgentMessageModel();
    CallChildFunction(arg: any) {
        this.Hide();
        this.StartLoading();
        this.loadUrgentMessage();
    }
    RedirectEpcsRegistration(url: string) {
        window.open(url);
    }


    RedirectToBillingPage() {
        this.router.navigateByUrl(ROUTE_NAME.DeluxeFeatures, { state: { navigateTo: PAGE_NAME.DeluxeFeatureSelection } });
    }

    RedirectToViewAccount() {
        this.router.navigateByUrl(ROUTE_NAME.ManageAccount, { state: { navigateTo: PAGE_NAME.DeluxeAccountManagement } });
    }
    RedirectToMessages() {
        this.router.navigateByUrl(ROUTE_NAME.MessageQueueTx, { state: { navigateTo: this.urgentMessage.FailedRxUserMessageUrl } });
    }
    RedirectToLicenseMessage() {
        this.router.navigateByUrl(ROUTE_NAME.MessageQueueTx, { state: { navigateTo: this.urgentMessage.FailedLicenseMessageUrl } });
    }
    RedirectToEditUser() {
        this.router.navigateByUrl(ROUTE_NAME.EditUser, { state: { navigateTo: PAGE_NAME.EditUser, mode: 'edit', cameFrom: PAGE_NAME.SelectPatient } });
    }
    RedirectToManageEpcs() {
        this.router.navigateByUrl(ROUTE_NAME.ManageAccount, { state: { navigateTo: PAGE_NAME.EpcsRegManageMode } });
    }
    OpenFailedReg() {
        this.svE.invokeEvent(EVENT_COMPONENT_ID.FailedRegistrationMessageModalOpen, null);
    }
    OpenDeniedREFREQ() {
        this.denRefReq.OpenDeniedRefReqMessagePopup();
    }

    OpenFailedRx() {
        this.failedRx.OpenFailedRxMessagePopup();
    }

    MessageCountChanged() {
        this.CallChildFunction(null);
    }

    loadUrgentMessage() {
        this.urgMssgSvc.GetUrgentMessage().subscribe(response => {
            this.urgentMessage = response;
            if (this.urgentMessage != undefined) {
                this.Show();
            }
            this.EndLoading();
        }
        );
    }
}
