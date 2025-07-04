import { MedCompletionHistoryModel, CancelRxEligibleScript } from '../../model/model.import.def';

export class ScriptPadModel {
    Description: string;
    rxId: string;
    IsBrandNameMed: boolean;
    RxEditUrl: string;
    ReviewUrl: string;
    showMediHistoryCompletionPopUp: boolean;
    MedCompletionHistory: MedCompletionHistoryModel[];
    CancelRxScripts: CancelRxEligibleScript[];
}