import { Injectable, Inject, forwardRef } from '@angular/core';
import { BaseService } from '../base.service';
import { ErrorService } from '../service.import.def';
import { ExtractRequest, ExtractType } from '../../model/model.import.def';
import { HttpClient, HttpHeaders, HttpResponse } from '@angular/common/http';
@Injectable()
export class ChartExtractService extends BaseService {
    constructor(http: HttpClient, @Inject(forwardRef(() => ErrorService)) svc: ErrorService) {
        super(http, svc);
    }

    GenerateDownloadFile(id, license, startTicks, endTicks, type) {
        return this.http.get('/api/ChartExtractAPI/GenerateDownloadFile', { params: { 'id': id, 'license': license, 'startTicks': startTicks, 'endTicks': endTicks, 'type': type }, observe: 'response', responseType: 'blob' }).pipe();
    }

    GetRequests(type){
        return this.http.get('/api/ChartExtractAPI/GetRequests', { params: {'type': type}, observe: 'response', responseType: 'json' }).pipe();
    }
}