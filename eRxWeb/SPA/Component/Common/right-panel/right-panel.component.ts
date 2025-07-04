
import { Component, Input, ViewChild, forwardRef, OnInit } from '@angular/core';
import { Location } from '@angular/common';
import { NavigationModel } from '../../../model/navigation.model';
import { ComponentCommonService, EventService } from '../../../services/service.import.def';
import { EVENT_COMPONENT_ID, NAVIGATION_EVENT_ID, current_context, PAGE_NAME, ROUTE_NAME } from '../../../tools/constants';
import { UrgentMessageModel } from '../../../model/model.import.def';
import { PrivacyOverrideReasonComponent, AuditLogInfoComponent, MessageQueInfoComponent } from './right-panel.component.import.def';
import { PrivacyPatientInfoDTO, AuditLogInfoDTO, MessageQueRequest, PharmacyRequestModel } from '../../../model/model.import.def';
import { PharmacyComponent } from './pharmacy/pharmacy.component';
import { PharmacySigAlertComponent } from './pharmacy-sig-alert-popup/pharmacy-sig-alert.component';
import { Router } from '@angular/router';
import RouteNameExtractor, { } from '../../../tools/utils/Route-Name-Extractor';


@Component({
    selector: 'erx-right-panel',
    templateUrl: './right-panel.template.html'
})
export class RightPanelComponent implements OnInit {
    //public page: string = "";
    public urgentMessageModel: UrgentMessageModel;
    public urgentMessageCountChange = false;

    @Input() page: string;
    @Input() NavigationModel: NavigationModel;
    @ViewChild(forwardRef(() => PrivacyOverrideReasonComponent)) privacyOverrideReasonComponent: PrivacyOverrideReasonComponent;
    @ViewChild(forwardRef(() => AuditLogInfoComponent)) auditLogInfoComponent: AuditLogInfoComponent;
    @ViewChild(PharmacyComponent) pharmacyComponent: PharmacyComponent;
    @ViewChild(forwardRef(() => MessageQueInfoComponent)) messageQueInfoComponent: MessageQueInfoComponent;
    @ViewChild(PharmacySigAlertComponent) pharmacySigAlertComponent: PharmacySigAlertComponent;

    constructor(private compSvc: ComponentCommonService,
        private svE: EventService, private location: Location, private router: Router) {
        let thisObj = this;

        this.compSvc.AddWindowFunction('PharmacySelected', function (value) {
            thisObj.PharmacySelected(value);
        });

        this.compSvc.AddWindowFunction('MessageQueueGetScriptData', function (value) {
            thisObj.MessageQueueGetSelectedScript(value);
        });
        this.compSvc.AddWindowFunction('RedirectContent', (value) => {
            this.NavigateMainContent(value);
        });

        this.compSvc.AddWindowFunction('TogglePrivacyOverride', (data) => {
            let patientInfo = new PrivacyPatientInfoDTO();
            patientInfo.PatientId = data.patientId;
            patientInfo.UserId = data.userId;
            patientInfo.CreatedUtc = data.createdUtc;
            this.privacyOverrideReasonComponent.loadPrivacyOverrideReason(patientInfo);
        });

        this.compSvc.AddWindowFunction('AuditLogRowSelected', (data) => {
            let auditLogInfo = new AuditLogInfoDTO();
            auditLogInfo.AuditLogId = data.auditLogId;
            this.auditLogInfoComponent.loadAuditLogInfo(auditLogInfo)
        });

        this.compSvc.AddWindowFunction('ShowPharmacySigAlert', (componentParameters) => {
            this.displayPharmacySigAlert(componentParameters);
        });
    }

    ngOnInit() {
        current_context.PageName = this.page;
    }

    MessageQueueGetSelectedScript(value: MessageQueRequest) {
        this.messageQueInfoComponent.SetMessageQueInfo(value);
    }

    NavigateMainContent(event: string) {
        let navigateParam: string;
        if (event != undefined)
            if (event.endsWith("aspx") && event.indexOf('?') < 0) {
                navigateParam = event + '?ver=' + Math.random();
            }
            else {
                navigateParam = event + '&ver=' + Math.random();
            }

        this.router.navigateByUrl(this.RouteNameSelection(event), { state: { navigateTo: navigateParam } })
    }

    RouteNameSelection(url: string) {
        return RouteNameExtractor.ExtractRoute(url.toLowerCase());
    }

    PharmacySelected(pharmacyRequestModel: PharmacyRequestModel) {
        this.pharmacyComponent.LoadPharmacy(pharmacyRequestModel);
    }

    FailedRegMessageCountChanged(val: any) {
        this.urgentMessageCountChange = false;
        setTimeout(() => this.urgentMessageCountChange = true, 0);
    }

    displayPharmacySigAlert(componentParameters: string) {
        this.pharmacySigAlertComponent.showPharmacySigAlert(componentParameters);
    }
}
