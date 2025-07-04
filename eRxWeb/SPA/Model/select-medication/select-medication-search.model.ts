import { SelectMedicationRequestType } from '../../model/model.import.def';
export class SelectMedicationSearchModel {
    MedSearchText: string;
    MedicationSearchOption: SelectMedicationRequestType;
    PreBuiltGroupName: string;
    Validate: boolean = false;

    public constructor(medicationSearchText: string, medicationSearchOption: SelectMedicationRequestType, preBuiltGroupName: string) {
        this.MedSearchText = medicationSearchText;
        this.MedicationSearchOption = medicationSearchOption;
        this.PreBuiltGroupName = preBuiltGroupName;
    }
}

export class SelectMedicationSearchArgs {
    MedicationSearchCriteria: SelectMedicationSearchModel;
    public constructor(medSearchCriteria: SelectMedicationSearchModel) {
        this.MedicationSearchCriteria = medSearchCriteria;
    }
}
export enum PrescriptionTaskType {
    APPROVAL_REQUEST = 0,
    REJECTION_NOTIFICATION,
    RENEWAL_REQUEST,    //internal renewal
    SCRIPT_ERROR, //ERROR
    SEND_TO_ADMIN,
    REFREQ, //from pharmacy
    SEND_TO_DISPENSE,
    EPA, //electronic prior authorization
    RXCHG = 8,
    RXCHG_PRIORAUTH = 9, //Subtask of a ChangeRx
    SPECIALTY_MED_ASSIST_RX = 10, //SpecialtyMed, specialty med
    DEFAULT = 100 //Used as default value when casting
}
export class SelectMedicationSearchStartUpParameters {
    DoctorHistoryOptionDisplayText: string;
    IsShowPreBuiltGroup: boolean;
    IsSearchDisabled: boolean;
}
export class RetrieveTaskTypeParameters {
    PrescriptionTaskType: PrescriptionTaskType;
    IsCsRefReqWorkflow: boolean;
    IsCsRxChgWorkflow: boolean;
    IsReconcileREFREQNonCS: boolean;
}


