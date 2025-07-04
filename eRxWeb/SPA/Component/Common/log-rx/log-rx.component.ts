import { Component, ElementRef, OnInit } from '@angular/core';
import { BaseComponent } from '../base.component';
import { LogRxService, EventService, ContentLoadService } from '../../../services/service.import.def';
import { EVENT_COMPONENT_ID, current_context } from '../../../tools/constants';
import { InitalContentPayload } from '../../../model/model.import.def';
import { Router, NavigationEnd, RouterEvent } from '@angular/router';

@Component({
    selector: 'erx-log-rx',
    templateUrl: './log-rx.template.html',
    styleUrls: ['./log-rx.style.css']
})
export class LogRxComponent extends BaseComponent implements OnInit {
    page: string = null;
    addHeight: number = 500;
    isTieEnabled: boolean = false;

    constructor(private logRxSvc: LogRxService, cd: ElementRef, private svE: EventService, private router: Router, private contentLoadService: ContentLoadService ) {
        super(cd);
        
    }

    ngOnInit() {
        this.router.events.subscribe((pageEvent: RouterEvent) => {
            if (this.contentLoadService.initalContentPayload.SitePayload) {
                this.isTieEnabled = current_context.isTieEnabled;
                this.setPageValue(current_context.PageName);
            }

            if (pageEvent instanceof NavigationEnd) {
                if (pageEvent.url)
                    this.visible = false;
                let routeParts = pageEvent.urlAfterRedirects.split('/');
                let currentPage: string = routeParts.pop() || routeParts.pop();  // handle potential trailing slash
                this.setPageValue(currentPage);
            }
        });
    }

    setPageValue(val: any) {
        this.page = null;
        let params = "";
        if (window.location.search.length > 0) {
            let queryUrl = window.location.search;
            if (queryUrl.startsWith('?page=')) {
                queryUrl = queryUrl.slice(queryUrl.indexOf('&') + 1);
                params = queryUrl;
            }
            else {
                params = window.location.search.slice(1);
            }
        }
        if (val != undefined && current_context.isTieEnabled)
            this.page = 'LogRxDisplay.aspx?page=' + this.GetPageName(val) + '&' + params;
        else
            this.page = null;
    }

    content: any;
    frameLoad(v: any) {

        if (this.page != null) {
            this.visible = true;
            if (v.target.contentDocument.body.scrollHeight)
                this.addHeight = v.target.contentDocument.body.scrollHeight + 30;// add buffer height
        }
        else {
            this.visible = false;
            this.addHeight = 0;
        }
    }
    LoadLogRx(data) {
        this.logRxSvc.GetContent(data).subscribe(response => {
            this.content = response;
            if (response.length > 0) {
                this.Show();

            } else {
                this.Hide();
            }
            this.EndLoading();
        }
        );
    }

    GetPageName(val: string) {
        if (val.includes('aspx')) {
            return val.slice(0, val.indexOf('.'));
        } else {
            return val;
        }
    }
}
