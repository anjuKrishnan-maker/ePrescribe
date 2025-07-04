import { Component, Input, Output, EventEmitter } from '@angular/core';
import { current_context, EVENT_COMPONENT_ID } from '../../../../tools/constants';

@Component({
    selector: 'erx-deluxe-modal-popup-control',
    templateUrl: './deluxe-modal-popup.template.html',
    styles: [`
        .fix-footer-height{
            height:39px!Important;
        }
        .fix-header-height{
            height:39px!Important;
        }
        .modal-content-component{
            border-radius: 10px!Important;
            width:100%;
        }
    `]
})
export class DeluxeModalPopupControl {

    @Input() title: string;
    @Input() ModalId: string;
    @Input() Width: string = "auto";
    @Input() Height: string = "auto";
    @Input() backdrop: string = "";

    @Output() onModalClosed = new EventEmitter();
    get innerId(): string {
        return this.ModalId + 'inner';
    }
    OpenPopup() {
        $("#" + this.innerId).modal('toggle');
        $("#" + this.innerId).on('hidden.bs.modal', (e) => {
            this.onModalClosed.emit(this.innerId);
        });
        $("#" + this.innerId).data('bs.modal').options.backdrop = this.backdrop;
    }
    ClosePopup() {
        $("#" + this.innerId).modal('hide');
    }

}