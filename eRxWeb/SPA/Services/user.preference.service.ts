import { Injectable } from '@angular/core';
import { Http, Response, Headers, RequestOptions } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import { BaseService } from './base.service';
import { UserPreferenceModel } from '../model/model.import.def';
import { HttpClient } from '@angular/common/http';

@Injectable()
export class UserPreferenceService extends BaseService {
    constructor(http: HttpClient) {
        super(http);
    }

    GetUserPreference() {
        return this.InvokeApiMethod<UserPreferenceModel>('/api/UserPreferenceApi/GetUserPreference', null);
    } 
}