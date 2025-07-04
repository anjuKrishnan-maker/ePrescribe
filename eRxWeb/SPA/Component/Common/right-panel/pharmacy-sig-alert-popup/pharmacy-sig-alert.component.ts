import { Component, AfterViewInit, ViewChild } from '@angular/core';
import { EventService } from '../../../../services/service.import.def';
import { ModalPopupControl } from '../../../shared/controls/modal-popup/modal-popup.control';
import { EVENT_COMPONENT_ID } from '../../../../tools/constants';

@Component({
    selector: 'erx-pharmacy-sig-alert',
    templateUrl: './pharmacy-sig-alert.template.html'
})
export class PharmacySigAlertComponent implements AfterViewInit {

    @ViewChild(ModalPopupControl) modalPopup: ModalPopupControl;
    public message: string = "The pharmacy that you are sending the prescription to does not support Sigs over 140 characters.  Please revise your Sig or deliver the prescription in another manner.";

    ngAfterViewInit() {
        this.modalPopup.ClosePopup();
    }

    constructor() {        
    }

    //
    // Navigation/Action events
    //
    public btnOkClicked() {
        this.modalPopup.ClosePopup();
    }

    public showPharmacySigAlert(val: string) {
        if (val) {
            this.message = val;
        }
        this.modalPopup.OpenPopup();
    }
}