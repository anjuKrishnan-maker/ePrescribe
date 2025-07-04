import { Injectable }     from '@angular/core';
import {BaseService} from '../base.service';
import { HttpClient } from '@angular/common/http';

@Injectable()
export class LogRxService extends BaseService {
    constructor(http: HttpClient) {
        super(http);
    }
    GetContent(data) {
        return this.InvokeApiMethod<string>('/api/LogRxApi/GetLogRxContent', data);
    }


}