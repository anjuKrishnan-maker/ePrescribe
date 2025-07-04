import { Injectable } from '@angular/core';
import { BaseService } from '../base.service';
import { } from '../../model/model.import.def';
import { HttpClient } from '@angular/common/http';

@Injectable()
export class WelcomeTourService extends BaseService {
    constructor(http: HttpClient) {
        super(http);
    }

    public WelcomeTourDoNotShowAgain(tourType: number) {
        return this.InvokeApiMethod('/api/WelcomeTourApi/WelcomeTourDoNotShowAgain', { tourType });
    }
}