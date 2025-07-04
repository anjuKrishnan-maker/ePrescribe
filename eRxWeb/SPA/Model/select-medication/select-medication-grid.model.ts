import { SelectMedicationMedModel, GridPaginationModel, MessageModel, SelectMedicationRequestType, PatientRecordStatus} from '../model.import.def';
export class SelectMedicationGridModel {
    IsPageination: boolean;
    IsAllMedSearch: boolean = false;
    Meds: SelectMedicationMedModel[];
    Messages: MessageModel[];
    RequestFor: SelectMedicationRequestType;
    PatientRecordStatus: PatientRecordStatus;
}

export class SelectMedicationGridPopulationCompletionArgs {
    Messages: MessageModel[];
    RequestFor: SelectMedicationRequestType;
    PatientRecordStatus: PatientRecordStatus;
}

export class SelectMedicationSelections {
    SelectedMeds: SelectMedicationMedModel[];
    EventContext: SelectMedicationEventContext;
}

export class SelectMedicationEventContext {
    CurrentMed: SelectMedicationMedModel;
    CurrentMedIndex: number;
    GridEvent: SelectMedicationGridEvent;
}

export enum SelectMedicationGridEvent {
    MedSelected = 0,
    MedDeselected = 1,
    MedModified = 2,
    AllMedSelected = 3,
    AllMedDeSelected = 4
}