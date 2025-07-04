import { PptPlusSummaryRequest } from "../model.import.def";

export class MedicationDTO {
    DDI: string;
    formularyStatus: number;
    medName: string;
    taskScriptMessageId: string;
    isSpecialtyMedication?: boolean;
    pptPlusSummaryRequest: PptPlusSummaryRequest[];
    isSelectedMedicationsModified: boolean;
}

export interface SelectedMedicationRows {
    selectedMedicationRowIndex?: number[];//Additional info least relevant for most on the #OnMedicationSelected# event.
}
