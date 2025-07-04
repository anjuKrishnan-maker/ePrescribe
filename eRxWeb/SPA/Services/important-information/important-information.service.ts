import { Injectable, Inject, forwardRef}     from '@angular/core';
import { ErrorService } from '../service.import.def';
import { BaseService } from '../base.service';
import { ImportantInfoModel } from '../../model/model.import.def';
import { HttpClient } from '@angular/common/http';
@Injectable()
export class ImportantInformationService extends BaseService {
    constructor(http: HttpClient, @Inject(forwardRef(() => ErrorService)) svc: ErrorService) {
        super(http,svc);
    }
    LoadImpInfo() {
        return this.InvokeApiMethod<ImportantInfoModel[]>('/api/ImportantInfoApi/LoadImportantInfo', null);
    }
}