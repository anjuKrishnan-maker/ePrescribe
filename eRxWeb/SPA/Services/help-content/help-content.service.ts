import { Injectable, Inject, forwardRef} from '@angular/core';
import { BaseService } from '../base.service';
import { ErrorService } from '../service.import.def';
import { HttpClient } from '@angular/common/http';
@Injectable()


export class HelpContentService extends BaseService {
    constructor(http: HttpClient, @Inject(forwardRef(() => ErrorService)) svc: ErrorService) {
        super(http,svc);
    }
    GetHelpContent(data: string) {
        return this.InvokeApiMethod<string>('/api/HelpContentApi/GetHelp', data);
    }
}