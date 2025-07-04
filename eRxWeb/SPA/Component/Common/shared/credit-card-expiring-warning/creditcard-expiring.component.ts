import { Component, Input, ViewChild } from '@angular/core';
import { EventService } from '../../../../services/service.import.def';
import { ModalPopupControl } from '../../../shared/controls/modal-popup/modal-popup.control';
import { ROUTE_NAME, PAGE_NAME} from "../../../../tools/constants";
import { Router } from '@angular/router';

@Component({
    selector: 'erx-creditcard-expiring',
    templateUrl: './creditcard-expiring.template.html',
    styleUrls: []
})

export class CreditCardExpiringComponent{
    @Input() ModalId: string;
    @ViewChild(ModalPopupControl) modalPopup: ModalPopupControl;


    constructor(private evSvc: EventService, private router: Router) {
    }

    public OpenModal() {
        this.modalPopup.OpenPopup();
    }

    //
    // Navigation/Action events
    //
    public btnUpdateBillingNowClick() {
        this.modalPopup.ClosePopup();
        this.router.navigateByUrl(ROUTE_NAME.DeluxeFeatures, { state: { navigateTo: PAGE_NAME.DeluxeBillingPage } })
    }

    public btnOkThanksClick() {
        this.modalPopup.ClosePopup();
    }
}