import { Component, ViewChild, OnInit } from '@angular/core';
import { SettingsService, EventService } from '../../services/service.import.def';
import { NAVIGATION_EVENT_ID, PAGE_NAME, EVENT_COMPONENT_ID, ROUTE_NAME } from '../../tools/constants';
import { SettingsLinkModel, MessageModel, MessageIcon, LinkLaunchType } from '../../model/model.import.def';
import { pdmpEpcsTeaserComponent } from '../pdmp-epcs-teaser/pdmp-epcs-teaser.component';
import { PdmpEnrollmentComponent } from '../pdmp-enrollment/pdmp-enrollment.component';
import { Router, NavigationExtras } from '@angular/router';
import { modelGroupProvider } from '@angular/forms/src/directives/ng_model_group';
@Component({
    selector: 'erx-settings',
    templateUrl: './settings.template.html'
})

export class SettingsComponent implements OnInit {
    @ViewChild(pdmpEpcsTeaserComponent) pdmpteaser: pdmpEpcsTeaserComponent;
    @ViewChild(PdmpEnrollmentComponent) pdmpEnrollment: PdmpEnrollmentComponent;
    public SettingsLinks: SettingsLinkModel[];
    public Messages: Array<MessageModel>;
    public LinkLaunchType = LinkLaunchType;
    public currentState: NavigationExtras["state"];
    public componentParameters: MessageModel;
    constructor(private settingSvc: SettingsService,
        private evSvc: EventService,
        private router: Router) {
        this.currentState = this.router.getCurrentNavigation()?.extras?.state;
    }

    ngOnInit(): void {

        if (this.currentState && this.currentState.messageModel) {
            this.componentParameters = JSON.parse(this.currentState.messageModel) as MessageModel;
            this.Messages = new Array<MessageModel>();
            if (this.componentParameters != undefined && this.componentParameters.Message !== '') {
                this.Messages.push(this.componentParameters);
            }
        }



        this.settingSvc.RetrieveLinks()
            .subscribe((response: SettingsLinkModel[]) => {
                this.SettingsLinks = response;
            });
    }

    LinkClicked(actionUrl: string) {
        if (actionUrl.indexOf(PAGE_NAME.EditUser + '?Mode=Add') > -1) {
            this.router.navigateByUrl(ROUTE_NAME.EditUser, { state: { mode: 'add' } });
        }
        else {
            this.router.navigateByUrl(`${ROUTE_NAME.Settings}/${actionUrl.replace(".aspx", '').toLowerCase()}`,
                {
                    state: {
                        navigateTo: actionUrl
                    }
                })
        }
    }

    TriggerUIEvent(event: string) {
        if (event == PAGE_NAME.PdmpEnrollmentForm) {
            this.pdmpEnrollment.ShowPdmpEnrollmentForm();
        } else if (event == PAGE_NAME.PdmpEpcsTeaser) {
            this.pdmpteaser.ShowPdmpEnrollmentForm();
        } else {
            this.evSvc.invokeEvent(event, null);
        }
    }

    onClose(isVisible: boolean) {
        this.router.navigateByUrl(ROUTE_NAME.DeluxeFeatures);
    }
}