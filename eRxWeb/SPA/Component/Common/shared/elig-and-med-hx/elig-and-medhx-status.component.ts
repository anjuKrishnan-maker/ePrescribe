import { Component, Input, ViewChild } from '@angular/core';
import { EligibilityMedHistoryModel, ReviewHistory, ApiResponse, MessageModel, MessageIcon } from '../../../../model/model.import.def';
import { current_context } from '../../../../tools/constants';
import { EventService, EligAndMedHxStatusService } from '../../../../services/service.import.def';
import { EVENT_COMPONENT_ID } from "../../../../tools/constants";
import { ModalPopupControl } from '../../../shared/controls/modal-popup/modal-popup.control';

@Component({
    selector: 'eligibility-medhistory-status',
    templateUrl: './elig-and-medhx-status.template.html',
    styleUrls: ['./elig-and-medhx-status.style.css']
})
export class EligAndMedHxStatusComponent {
    @Input() ModalId: string;
    EligibityMedHistoryItems: Array<EligibilityMedHistoryModel>;
    Messages: Array<MessageModel>;
    isDataLoading: boolean;
    @ViewChild(ModalPopupControl) modalPopup: ModalPopupControl;

    constructor(private evSvc: EventService, private eligAndMedHxStatusService: EligAndMedHxStatusService) {
    }

   

    Show() {
        this.modalPopup.OpenPopup();
        this.GetEligibilityAndMedHistoryStatus();
    }

    GetEligibilityAndMedHistoryStatus() {
        this.isDataLoading = true;
        this.eligAndMedHxStatusService.GetEligibilityAndMedHistoryStatus().subscribe((response) => {
            this.EligibityMedHistoryItems = response;
            this.isDataLoading = false;
        }, (err) => { }, () => {
            this.isDataLoading = false;
        });
    }

    cancel() { this.modalPopup.ClosePopup(); }
}
