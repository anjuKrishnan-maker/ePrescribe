import { Component, AfterViewInit} from '@angular/core';
import { ComponentCommonService } from '../../../services/service.import.def';
@Component({
    selector: 'erx-html-through-overlay',
    templateUrl: './html-through-overlay.template.html',
    styleUrls: ['./html-through-overlay.style.css']
})

export class HtmlThroughOverlayComponent implements AfterViewInit {
    public _html: string;
    public _title: string;
    private mdlEl: any;
    private isLoaded: Boolean=false;
    constructor(private compSvc: ComponentCommonService) {
        compSvc.AddWindowFunction("showHtmlOverlay", (data) => {
            this._html = atob(data.b64Html);
            this._title = data.title;
            this.mdlEl.modal('show');
        });
    }

    ngAfterViewInit() {
        this.mdlEl = $('#mdlSimple').modal('hide');
        this.mdlEl.on('hidden.bs.modal', () => {
            this._html = undefined;
      });
    }
}