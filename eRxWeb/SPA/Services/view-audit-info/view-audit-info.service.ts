import { Injectable, Inject, forwardRef } from '@angular/core';
import { BaseService } from '../base.service';
import { ErrorService } from '../service.import.def';
import { HttpClient } from '@angular/common/http';
@Injectable()
export class AuditViewInfoService extends BaseService {
    constructor(http: HttpClient, @Inject(forwardRef(() => ErrorService)) svc: ErrorService) {
        super(http, svc);
    }

    GetReportAuditRecordExtendedInfo(auditLogId: string) {
        return this.InvokeApiMethod<string>('/api/AuditViewInfoApi/GetAuditLogUserExtendedInfo', auditLogId);
    }
}