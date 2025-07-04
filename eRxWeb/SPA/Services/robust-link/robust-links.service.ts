import { Injectable,Inject,forwardRef }     from '@angular/core';
import { ErrorService } from '../service.import.def';
import { BaseService } from '../base.service';
import { RobustLinkMesssagel } from '../../model/model.import.def';
import { HttpClient } from '@angular/common/http';
@Injectable()
export class RobustLinksService extends BaseService {
    constructor(http: HttpClient, @Inject(forwardRef(() => ErrorService)) svc: ErrorService) {
        super(http);
    }
    GetRobustLinks(data) {
        return this.InvokeApiMethod<RobustLinkMesssagel[]>('/api/RobustLinkApi/GetRobustLinks', data);
    }
}