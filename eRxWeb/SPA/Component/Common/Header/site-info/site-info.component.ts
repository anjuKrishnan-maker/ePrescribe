/// <reference path="../../../../services/service.import.def.ts" />
import { Component, Input, OnInit } from '@angular/core';
import { UserInfoService, PatientService, EventService, ContentLoadService } from '../../../../services/service.import.def';
import { EVENT_COMPONENT_ID, current_context } from '../../../../tools/constants';
import { NavigationModel, User, SiteInfo, InitalContentPayload } from "../../../../model/model.import.def";
import Constants = require("../../../../tools/constants");
import { Router, NavigationEnd, RouterEvent } from '@angular/router';


@Component({
    selector: 'erx-header-right-info',
    templateUrl: './site-info.template.html',
    styleUrls: ['./site-info.style.css']
})
export class SiteInfoComponent implements OnInit {
    @Input() NavigationModel: NavigationModel;
    isRestrictedMenu = false;
    siteInfo: SiteInfo;
    isEditable: boolean = false;
    visible: boolean = true;

    get isException(): boolean {
        return current_context.isException;
    }

    constructor(private userInfoService: UserInfoService,
        private ptSvc: PatientService, private evtSvc: EventService, private router: Router, private contentLoadService: ContentLoadService) {
        this.siteInfo = new SiteInfo();
        this.siteInfo.user = new User();               
    }

    ngOnInit(): void {

        this.siteInfo = this.contentLoadService.initalContentPayload.SitePayload;
        this.isRestrictedMenu = this.siteInfo.IsRestrictedMenu;

        if (this.siteInfo != undefined) {
            current_context.isTieEnabled = this.siteInfo.IsTieAdsEnabled;
            this.setIsEditable();
        }

        this.router.events.subscribe((event: RouterEvent) => {            
            if (event instanceof NavigationEnd) {
                this.isEditable = event.urlAfterRedirects.indexOf(Constants.ROUTE_NAME.SelectPatient.toLowerCase()) > -1;
                let routeParts = event.urlAfterRedirects.split('/');
                let currentPage: string = routeParts.pop() || routeParts.pop();  // handle potential trailing slash
                this.displaySiteInfoForContext(currentPage);
            }
        });
        
    }

    private displaySiteInfoForContext(currentPage: string): void {
        switch (currentPage.toLowerCase()) {
            case Constants.PAGE_NAME.HomeAddress.toLowerCase():
                this.visible = false;
                break;

            default:
                this.visible = true;
                break;
        }
    }

    setIsEditable() {
        if (current_context.PageName.toLowerCase() === Constants.PAGE_NAME.SelectPatient.toLowerCase()
        ) {
            this.isEditable = true;
        }
        else {
            this.isEditable = false;
        }
    }

    navigateToEditUser() {
        this.router.navigateByUrl(Constants.ROUTE_NAME.EditUser, { state: { cameFrom: 'SiteInfo' } });
    }

    RedirectToSiteSelection() {
        if (!this.isException)
            window.location.href = this.siteInfo.SelectSiteUrl;
        return false
    }   
}
