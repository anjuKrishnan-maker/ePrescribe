import { Injectable, Inject, forwardRef } from '@angular/core'
import { Http } from '@angular/http';
import { DeluxeFeatureSelection, ForceCompulsoryRestrictions, Products, DeluxeFeature } from '../../model/model.import.def';
import { BaseService } from '../base.service';
import { ErrorService } from '../service.import.def';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';

@Injectable()
export class DeluxeFeatureSelectionService extends BaseService {    
    constructor(http: HttpClient, @Inject(forwardRef(() => ErrorService)) svc: ErrorService) {
        super(http, svc);
    }


    GetDeluxeFeatureSelectionInitialData() {
        return this.InvokeApiMethod<DeluxeFeature>('/api/DeluxeFeatureSelectionApi/GetDeluxeFeatureSelectionInitialData', null);
    }
   
    SetSelectedDeluxeFeature(selectedDeluxeFeature) {        
        return this.InvokeApiMethod<boolean>('/api/DeluxeFeatureSelectionApi/SetSelectedDeluxeFeature', selectedDeluxeFeature); 
    }


}
