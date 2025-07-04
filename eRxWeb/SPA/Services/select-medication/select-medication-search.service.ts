import { Injectable, Inject, forwardRef } from '@angular/core';
import { ErrorService } from '../service.import.def';
import { BaseService } from '../base.service';
import { SelectMedicationSearchStartUpParameters, RetrieveTaskTypeParameters } from '../../model/model.import.def';
import { HttpClient } from '@angular/common/http';

@Injectable()
export class SelectMedicationSearchService extends BaseService {
    constructor(http: HttpClient, @Inject(forwardRef(() => ErrorService)) svc: ErrorService) {
        super(http, svc);
    }
    SearchMedicationName(searchText: string){
        return this.InvokeApiMethod<string[]>('/api/SelectMedicationSearchApi/SearchMedicationName', searchText);
    } 

    GetPrebuiltPrescriptionGroupNames(){
        return this.InvokeApiMethod<string[]>('/api/SelectMedicationSearchApi/GetPrebuiltPrescriptionGroupNames', null);
    } 

    GetStartUpParameters(){
        return this.InvokeApiMethod<SelectMedicationSearchStartUpParameters>('/api/SelectMedicationSearchApi/GetStartUpParameters', null);
    }
    RetrieveTaskType() {
        return this.InvokeApiMethod<RetrieveTaskTypeParameters>('/api/SelectMedicationSearchApi/RetrieveTaskTypeParameters', null);
    }
}