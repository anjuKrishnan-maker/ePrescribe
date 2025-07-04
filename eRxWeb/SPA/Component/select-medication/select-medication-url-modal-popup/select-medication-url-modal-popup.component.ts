import { Component, AfterViewInit} from '@angular/core';
import { ComponentCommonService, EventService } from '../../../services/service.import.def';
@Component({
    selector: 'erx-select-medication-url-popup-content',
    templateUrl: './select-medication-url-modal-popup.template.html'
})

export class SelectMedicationUrlModalPopupComponent implements AfterViewInit {
    public _src: string;
    public _title: string;
    private mdlEl: any;
    public isLoaded: Boolean = false;
    constructor(private compSvc: ComponentCommonService, private evSvc: EventService) {
        compSvc.AddWindowFunction("setUrlModalPopup", (data) => {
            this._src = data.src;
            this._title = data.title;
            this.isLoaded = false;
            this.mdlEl.modal('show');
        });        
    }

    openPopupModal(data: any) {
    this._src = data.src;
    this._title = data.title;
    this.isLoaded = false;
    this.mdlEl.modal('show');
    };

    ContentLoad() {
        this.isLoaded = true;
    }
    ngAfterViewInit() {
        this.mdlEl = $('#mdlUrl').modal('hide');
        this.mdlEl.on('hidden.bs.modal', () => {
            this._src = undefined;
      });
    }
}
