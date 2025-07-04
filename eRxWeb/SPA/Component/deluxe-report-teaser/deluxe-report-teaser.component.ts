import { Component, Output, EventEmitter, Input } from '@angular/core';
import { ViewChild } from '@angular/core';
import { BaseComponent } from '../common/base.component';
import { DeluxeModalPopupControl } from '../shared/controls/deluxe-modal-popup/deluxe-modal-popup.control';
@Component({
    selector: 'erx-deluxe-report-teaser',
    templateUrl: './deluxe-report-teaser.template.html',
    styleUrls: ['./deluxe-report-teaser.style.css']
})
export class DeluxeReportComponent extends BaseComponent {
    @ViewChild(DeluxeModalPopupControl) modalPopup: DeluxeModalPopupControl;
    @Output() closeModalEvent = new EventEmitter<boolean>();



    @Input() set showDeluxeAlert(value: boolean) {
        if (value)
            this.ShowDeluxeForm();
    }

    constructor() {
        super(null, null);
    }

    onCloseModal(event: any) {
        this.modalPopup.ClosePopup();
        this.EndLoading();
        this.closeModalEvent.emit(false);
    }

    Close() {
        this.modalPopup.ClosePopup();
        this.EndLoading();
    }

    ShowDeluxeForm() {
        this.modalPopup.OpenPopup();
        this.StartLoading();
    }
}




