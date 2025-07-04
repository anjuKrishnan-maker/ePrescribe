import { async } from '@angular/core/testing';
import { ChangeDetectorRef } from '@angular/core';
import { } from 'jasmine';
import { Observable } from 'rxjs/Observable';
import { Observer } from 'rxjs/Observer';
import { AuditLogInfoComponent } from '../component/common/right-panel/audit-log-info/audit-log-info.component';
import { AuditLogInfoDTO } from '../model/model.import.def';
import { ComponentCommonService, ErrorService, EventService, AuditViewInfoService } from '../services/service.import.def';

describe('Audit Log Info Unit test', () => {
    let component: AuditLogInfoComponent;

    let evnntSvc: EventService = new EventService();
    let errorsvc: ErrorService = new ErrorService(evnntSvc,null);
   // let cd: ChangeDetectorRef = new ChangeDetectorRef();

    class MockAuditViewInfoService extends AuditViewInfoService {
        GetReportAuditRecordExtendedInfo() {
            return Observable.create((observer: Observer<void>) => {
                observer.next("Audit record detais HTML");
            });
        }
    }

    class MockChangeDetectorRef extends ChangeDetectorRef {
        detectChanges() {
        }
    }

    beforeEach(async(() => {
        component = new AuditLogInfoComponent(evnntSvc, new MockAuditViewInfoService(null, errorsvc), new MockChangeDetectorRef());
    }));

    it('ReportRowExtendedInfo initiaziled with null', async(() => {
        expect(component.ReportRowExtendedInfo).toEqual(null);
    }));

    it('ReportRowExtendedInfo is null when auditLogId is null', async(() => {
        let auditLogInfoModel = new AuditLogInfoDTO();
        auditLogInfoModel.AuditLogId = null;

        component.loadAuditLogInfo(auditLogInfoModel);

        expect(component.ReportRowExtendedInfo).toEqual(null);
    }));

    it('load ReportRowExtendedInfo when auditLogId is not null', async(() => {
        let auditLogInfoModel = new AuditLogInfoDTO();
        auditLogInfoModel.AuditLogId = "123";

        component.loadAuditLogInfo(auditLogInfoModel);

        expect(component.ReportRowExtendedInfo.length > 0).toBeTruthy;
    }));
});