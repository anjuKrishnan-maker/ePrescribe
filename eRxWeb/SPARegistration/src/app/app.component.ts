import { Component, Inject, AfterViewInit } from '@angular/core';
import { Router, NavigationStart, NavigationEnd, NavigationCancel, NavigationError } from '@angular/router';
import { LoaderService } from './service/loader.service';
import { Subject } from 'rxjs';
import { MessageService } from './service/message.service';

@Component({
    selector: 'app-root',
    templateUrl: './app.component.html',
    styleUrls: ['./app.component.css']
})

export class AppComponent implements AfterViewInit {
    public version: string;
    public supportMailAddress: string;
    public supportMailAddressHref: string;
    public loadingNavigation: Subject<boolean> = new Subject<boolean>();

    constructor(@Inject('window') private window: any,
        private router: Router,
        public loaderService: LoaderService,
        private messageService: MessageService) {
        this.version = this.window?.appcontext?.version;
        this.supportMailAddress = this.window?.appcontext?.supportMailAddress;
        this.supportMailAddressHref = "mailto:" + this.supportMailAddress + "?Subject=ePrescribe%20Registration";
    }

    ngAfterViewInit() {
        this.router.events
            .subscribe((event) => {
                if (event instanceof NavigationStart) {
                    this.loadingNavigation.next(true);
                }
                else if (event instanceof NavigationEnd ||
                    event instanceof NavigationCancel ||
                    event instanceof NavigationError) {
                    this.loadingNavigation.next(false);
                }
            });
    }
}
