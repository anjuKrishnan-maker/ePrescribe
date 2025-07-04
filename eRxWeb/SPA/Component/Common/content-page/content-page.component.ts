import { Component, Input, Output, EventEmitter, ElementRef, ViewChild, OnInit } from '@angular/core';
import { SafeUrl, DomSanitizer } from '@angular/platform-browser';
import { EVENT_COMPONENT_ID, PAGE_NAME } from '../../../tools/constants';
import { EventService, SpinnerService } from '../../../services/service.import.def';
import { FunctionDetail } from "../../../model/model.import.def";
import { Router, NavigationExtras, ActivatedRoute, ActivatedRouteSnapshot } from '@angular/router';

@Component({
    selector: 'erx-content-place',
    templateUrl: './content-page.template.html',
    styleUrls: ['./content-page.style.css']

})
export class ContentPageComponent implements OnInit {

    public _src: SafeUrl;
    public validUrl: boolean;
    public currentState: NavigationExtras["state"];    

    @Input('src')
    set src(value: any) {
        if (value !== undefined) {
            this.validUrl = true;
            this._src = this.sanitizer.bypassSecurityTrustResourceUrl(value);
        }
    }

    @Output() onContentLoad: EventEmitter<any> = new EventEmitter();

    @ViewChild('contentFrame') contentFrame: ElementRef;

    constructor(private sanitizer: DomSanitizer,
        private evtSvc: EventService,
        private router: Router,
        private spinnerSvc: SpinnerService) {

        this.currentState = this.router.getCurrentNavigation().extras.state

        this.evtSvc.subscribeEvent(EVENT_COMPONENT_ID.CallFrameContentJavaScript, (val: FunctionDetail) => {
            if (this.contentFrame !== undefined
                && this.contentFrame !== null
                && this.contentFrame.nativeElement.contentWindow !== undefined
                && this.contentFrame.nativeElement.contentWindow !== null) {
                var fn = this.contentFrame.nativeElement.contentWindow[val.Name];
                if (fn !== undefined && fn !== null) {
                    this.contentFrame.nativeElement.contentWindow[val.Name].apply(val.Caller, val.Param);
                }
            }

        });
    }

    ngOnInit(): void {
        this.spinnerSvc.showPageSpinner();//as soon as iframe src is set. 
        this.src = this.currentState.navigateTo;        
    }
}
