import { Injectable } from '@angular/core';
import { BaseService } from '../base.service';
import { ReportMenuModel } from '../../model/model.import.def';
import { HttpClient } from '@angular/common/http';
import { API_ROUTE_NAME } from '../../tools/constants';


@Injectable()
export class ReportService extends BaseService {
    constructor(http: HttpClient) {
        super(http);
    }

    public GetReportsList() {
        return this.InvokeApiMethod<ReportMenuModel>(API_ROUTE_NAME.REPORT_LIST_FETCHING, null);
    }
}