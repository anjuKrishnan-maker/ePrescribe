import {  GridPaginationModel} from '../model.import.def';
export enum SelectMedicationRequestType {
    PatientHistory = 1,
    ProviderHistory = 5,
    AllMedication = 10,
    PreBuiltGroup = 15
}

export enum PatientRecordStatus {
    Active = 1,
    InActive = 5,
    Both = 10
}

export enum SortOrder {
    Ascending = 1,
    Descending = 2
}
export class SelectMedicationDataRequest {
    SearchText: string;
    PatientRecordStatus: PatientRecordStatus;
    GroupName: string;
    RequestFor: SelectMedicationRequestType;
    Page: GridPaginationModel;
}