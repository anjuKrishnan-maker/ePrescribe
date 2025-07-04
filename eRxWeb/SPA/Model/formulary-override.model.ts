export class IgnoreReasonsModel {
    IgnoreReasons: string[]
}   

export class MedicationModel {
    RxID: string;
    Medication: string;
}

export class FormularyOverrideModel {
    IgnoreReasons: IgnoreReasonsModel
    Medication: MedicationModel[]
}

export class SelectedOverrideReasonModel {
    RxID: string
    OverideReason: number
}

export enum FormularyOverideAction {
    Cancel = 0,
    Process = 1,
}

export class FormularyOverideArgs {
    Action: FormularyOverideAction;
    Data: any;
}