import { Component, ChangeDetectorRef } from '@angular/core';
import { BaseComponent } from '../../base.component';
import { EventService,  AuditViewInfoService } from '../../../../services/service.import.def';
import { EVENT_COMPONENT_ID } from '../../../../tools/constants';
import { AuditLogInfoDTO } from '../../../../model/model.import.def';

@Component({
    selector: 'erx-audit-log-info',
    templateUrl: './audit-log-info.template.html'
})

export class AuditLogInfoComponent extends BaseComponent {

    public ReportRowExtendedInfo: string = null;

    constructor(private svE: EventService,
        private aviService: AuditViewInfoService,
        private cd: ChangeDetectorRef) {
        super(null, svE);

        this.svE.subscribeEvent(EVENT_COMPONENT_ID.RightPanelLoaded, () => {
            this.ReportRowExtendedInfo = null;
        });
    }

    public loadAuditLogInfo(auditLogInfo: AuditLogInfoDTO) {
        if (auditLogInfo.AuditLogId == undefined || auditLogInfo.AuditLogId == null) {
            this.ReportRowExtendedInfo = null;
            this.cd.detectChanges();
        }
        else {
            this.StartLoading();
            this.aviService.GetReportAuditRecordExtendedInfo(auditLogInfo.AuditLogId).subscribe((r) => {
                this.ReportRowExtendedInfo = r;
                this.EndLoading();
                this.cd.detectChanges();
            });
        }
    }
}