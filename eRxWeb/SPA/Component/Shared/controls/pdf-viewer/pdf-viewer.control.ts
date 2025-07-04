import { Component, Input, Output, EventEmitter } from '@angular/core';
@Component({
    selector: 'erx-pdf-viewer',
    templateUrl: './pdf-viewer.template.html',
    styleUrls: ['./pdf-viewer.style.css']
})

export class PdfViewerControl {
    public computedUrl: string = null;
    @Output() onPdfLoad = new EventEmitter<{}>();

    @Input('PdfUrl')
    set PdfUrl(value: any) {
        if (value !== undefined)
            this.computedUrl = "../../../../../PDFViewer.aspx?pdf=" + value + "&v=" + Math.random();
        else
            this.computedUrl = null;
    }
  
  
    PageChange(pageNo: number) {
        this.onPdfLoad.emit({});
    }
}