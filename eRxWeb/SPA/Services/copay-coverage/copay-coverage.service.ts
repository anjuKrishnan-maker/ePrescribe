import { Injectable, Inject, forwardRef }     from '@angular/core';
import {BaseService} from '../base.service';
import { ErrorService } from '../service.import.def';
import { CopayCoverageModel } from '../../model/model.import.def';
import { HttpClient } from '@angular/common/http';

@Injectable()
export class CopayCoverageServcie extends BaseService {
    constructor(http: HttpClient, @Inject(forwardRef(() => ErrorService)) svc: ErrorService) {
        super(http,svc);
    }
    GetCopayCoverageInfo(arg:any) {
        return this.InvokeApiMethod<CopayCoverageModel>('/api/CopayCoverageInfoApi/GetCopayCoverageInfo', arg);
    }
    GetCopayCoverageInfoFromSession() {
        return this.InvokeApiMethod<CopayCoverageModel>('/api/CopayCoverageInfoApi/GetCopayCoverageInfoFromSession', null);
    }
}