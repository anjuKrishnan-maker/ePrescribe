import { Component, Input } from '@angular/core';
import { SafeUrl, DomSanitizer } from '@angular/platform-browser';
import { GoogleAnalyticsService } from '../../../services/service.import.def';
@Component({
    selector: 'erx-popup-content',
    templateUrl: './modal-popup.template.html',
    styles: [`
    #frameMdlPopup{
        height: 100%; 
        width: 100%; 
        border: none; 
        overflow-x: hidden;
    }
`]

})
export class ModalPopupComponent {
    public _src: SafeUrl;
    @Input('src')
    set src(value: any) {
        if (value !== undefined)
            this._src = this.sanitizer.bypassSecurityTrustResourceUrl(value);
        else
            this._src = null;
    }
    constructor(private sanitizer: DomSanitizer, private gaService: GoogleAnalyticsService) { }
    @Input() title: string;

    modalPopupLoaded(e: any) {
      
        try {
            if (e.target.contentDocument.URL != 'about:blank') {
                let str: string;
                let frm = e.target.contentDocument.getElementsByTagName('form');
                if (frm.length > 0) {
                    if (frm[0].action) {
                        str = frm[0].action;
                    }
                }
                var quryIndex = str.lastIndexOf('?');
                if (quryIndex > -1) {
                    str = str.substring(0, quryIndex);
                }
                var index = str.lastIndexOf('/');
                var length = str.length;
                var res = str.substring(index + 1, length);
                this.gaService.SendPageView(res);
                (<any>window).appInsights.trackPageView(res);
            }
        }
        catch (e) { }
    }
}
