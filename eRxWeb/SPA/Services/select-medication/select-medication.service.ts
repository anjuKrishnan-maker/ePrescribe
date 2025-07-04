import { Injectable, Inject, forwardRef } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { ErrorService } from '../service.import.def';
import { BaseService } from '../base.service';
import {
    SelectMedicationGridModel, SelectMedicationMedModel, SelectMedicationModel, SelectMedicationDataRequest, PatientCoverageHeader, MessageModel,
    UpdatePatientSelectedCoverageRequest, ScriptPadModel, SelectMedicationStartUpModel, PatientCoverageHeaderList, SetRequestedMedicationAsCurrentMedicationRequest
} from '../../model/model.import.def';
import { Subscriber } from 'rxjs';
import { HttpClient } from '@angular/common/http';

@Injectable()
export class SelectMedicationService extends BaseService {
    constructor(http: HttpClient, @Inject(forwardRef(() => ErrorService)) svc: ErrorService) {
        super(http, svc);
    }
    GetPatientCoverages() {
        return this.InvokeApiMethod<PatientCoverageHeader[]>('/api/SelectMedicationApi/GetPatientCoverages', null);
    }

    UpdatePatientSelectedCoverage(req: UpdatePatientSelectedCoverageRequest) {
        return this.InvokeApiMethod('/api/SelectMedicationApi/UpdatePatientSelectedCoverage', req);
    }

    InsertLibraryAudit() {
        return this.InvokeApiMethod('/api/SelectMedicationApi/InsertLibraryAudit', null);
    }

    GetSelectMedicationData(req: SelectMedicationDataRequest) {
        return this.InvokeApiMethod<SelectMedicationGridModel>('/api/SelectMedicationApi/GetSelectMedicationData', req);
    }

    GetActiveDiagnosis() {
        return this.InvokeApiMethod<string>('/api/SelectMedicationApi/GetPatientDiagnosis', null);
    }

    ClearSelectedDx() {
        return this.InvokeApiMethod('/api/SelectMedicationApi/ClearSelectedDx', null);
    }

    AddToScriptPad(req: SelectMedicationMedModel[]) {
        return this.InvokeApiMethod<SelectMedicationModel>('/api/SelectMedicationApi/AddToScriptPad', req);
    }
    AddAndReview(req: SelectMedicationMedModel[]) {
        return this.InvokeApiMethod<SelectMedicationModel>('/api/SelectMedicationApi/AddAndReview', req);
    }

    GetScriptPadMedicationHistory() {
        return this.InvokeApiMethod<ScriptPadModel>('/api/SelectMedicationApi/GetScriptPadMedicationHistory', null);
    }

    SelectSig(req: SelectMedicationMedModel) {
        return this.InvokeApiMethod<SelectMedicationModel>('/api/SelectMedicationApi/SelectSig', req);
    }

    CompleteSelectSig() {
        return this.InvokeApiMethod<string>('/api/SelectMedicationApi/CompleteSelectSig', null);
    }
    AddMedToScript() {
        return this.InvokeApiMethod('/api/SelectMedicationApi/AddMedToScriptPad', null);
    }

    GetSelectMedicationStartUpData(): Observable<SelectMedicationStartUpModel> {
        return this.InvokeApiMethod<SelectMedicationStartUpModel>('/api/SelectMedicationApi/GetSelectMedicationStartUpData', null);
    }

    ValidateMedSection(req: SelectMedicationMedModel[]) {
        return this.InvokeApiMethod<MessageModel>('/api/SelectMedicationApi/ValidateMedSection', req);
    }
    SetRequestedMedicationAsCurrentMedication(req: SetRequestedMedicationAsCurrentMedicationRequest) {
        return this.InvokeApiMethod<SetRequestedMedicationAsCurrentMedicationRequest>('/api/SelectMedicationApi/SetRequestedMedicationAsCurrentMedication', req);
    }

    GetPatientCoveragesInfo(): Observable<PatientCoverageHeaderList> {
        return this.InvokeApiMethod<PatientCoverageHeaderList>('/api/SelectMedicationApi/GetPatientCoveragesInfo', null);
    }

}