
import { Injectable }     from '@angular/core';
import { Http, Response,  Headers, RequestOptions} from '@angular/http';
import { Observable }     from 'rxjs/Observable';
import { BaseService } from './base.service';
import { Patient, PatientResponse, IPharmacyInfo, IDiagnosisInfo, IAllergyInfo, IMedicationInfo} from '../model/patient.model';
import { NavigationModel } from '../model/navigation.model';
import { EventService, ErrorService } from './service.import.def';
import { EVENT_COMPONENT_ID } from  '../tools/constants';
import { HttpClient } from '@angular/common/http';


@Injectable()
export class PatientService extends BaseService {
    public SelectedPatient: Patient;
    PopupNavigation:NavigationModel;
    constructor(public http: HttpClient, ers: ErrorService, private evtSvc: EventService) {
        super(http, ers);
        this.SelectedPatient = new Patient();
    }
    GetPatientHeader(patientid: string) {
        //Gets called twice
        //1. ContentLoadEvent
        //2. MenuSelectionEvent
        return this.InvokeApiMethod<Patient>('/api/PatientHeaderAPI/GetPatientHeaderData' ,  patientid);
    }

    GetPatientFromSession() {
        return this.InvokeApiMethod<Patient>('/api/PatientHeaderAPI/GetPatientFromSession', null);
    }

    ResetSelectedPatient() {
        this.SelectedPatient = new Patient();
    }
   
    GetCurrentPatient() {
        return this.InvokeApiMethod<PatientResponse>('/api/PatientHeaderAPI/GetCurrentPatient', null);
    }

    GetPatientPharmacy() {
        return this.InvokeApiMethod<IPharmacyInfo>('/api/PatientHeaderAPI/GetPatientPharmacy', null);
    }

    GetPatientDiagnosis() {
        return this.InvokeApiMethod<IDiagnosisInfo>('/api/PatientHeaderAPI/GetPatientDiagnosis', null);
    }

    GetPatientAllergies() {
        return this.InvokeApiMethod<IAllergyInfo>('/api/PatientHeaderAPI/GetPatientAllergies', null);
    }

    GetPatientActiveMeds() {
        return this.InvokeApiMethod<IMedicationInfo>('/api/PatientHeaderAPI/GetPatientActiveMeds', null);
    }
}