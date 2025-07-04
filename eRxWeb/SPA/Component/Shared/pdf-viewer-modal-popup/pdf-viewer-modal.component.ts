import { Component, AfterViewInit, ViewChild } from '@angular/core';
import { ComponentCommonService, EventService } from '../../../services/service.import.def';
import { ModalPopupControl } from '../controls/modal-popup/modal-popup.control';
import {PdfViewerControl } from '../controls/pdf-viewer/pdf-viewer.control';
import { PdfOverlayArgs } from '../../../model/model.import.def';

@Component({
    selector: 'erx-pdf-viewer-modal-popup',
    templateUrl: './pdf-viewer-modal.template.html'
})

export class PdfViewerModalComponent implements AfterViewInit {
    @ViewChild(ModalPopupControl) modalPopup: ModalPopupControl;
    @ViewChild(PdfViewerControl) pdfViewer: PdfViewerControl;
    private isLoaded: Boolean = false;
    constructor(private compSvc: ComponentCommonService, private evSvc: EventService) {
        compSvc.AddWindowFunction("ShowPdfOverlay", (args: PdfOverlayArgs) => {
            this.pdfViewer.PdfUrl = args.PdfUrl;
            this.modalPopup.title = args.PopupTitle;
            this.modalPopup.OpenPopup();
        });        
    }
    ModalClosed(event) {
        this.pdfViewer.PdfUrl = undefined;
        //console.log(event);
    }
    ContentLoad() {
        this.isLoaded = true;
    }
    ngAfterViewInit() {
      
    }
}
