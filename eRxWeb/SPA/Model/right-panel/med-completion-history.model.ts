class MedCompletionHistoryModel {
    Medication: string;
    RxID: string;   
    Checked: boolean;
}

export { MedCompletionHistoryModel };


export enum MedCompletionHistoryAction {
    Cancel = 0,
    Continue = 1,
    CompleteAndContinue= 2,
}

export class MedCompletionHistoryArgs {
    Action: MedCompletionHistoryAction;
    Data: any;
}