import { Component, AfterViewInit } from '@angular/core';
import { ComponentCommonService } from '../../../services/service.import.def';

@Component({
    selector: 'erx-external-html-through-overlay',
    templateUrl: './external-html-through-overlay.template.html',
    styleUrls: ['./external-html-through-overlay.style.css']
})

export class ExternalHtmlThroughOverlayComponent implements AfterViewInit {
    public _html: string;
    public _title: string;
    private mdlEl: any;
    private frameSrc: string;
    private isLoaded: Boolean = false;
    constructor(private compSvc: ComponentCommonService) {
        compSvc.AddWindowFunction("showExternalHtmlOverlay", (data) => {
            this._html = atob(data.b64Html);
            this._title = data.title;
            //Data binding not used because setting src/srcdoc attributes only works in Chrome and not IE browser
            let iframe = document.getElementById("displayFrame") as HTMLIFrameElement;
            iframe.contentWindow.document.body.innerHTML = "";
            iframe.contentWindow.document.write(this._html);

            this.mdlEl.modal('show');
        });
    }

    ngAfterViewInit() {
        this.mdlEl = $('#external-mdlSimple').modal('hide');
        this.mdlEl.on('hidden.bs.modal', () => {
            this._html = undefined;
        });
    }
}